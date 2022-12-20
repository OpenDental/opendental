using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JournalEntries {
		///<summary>Used when displaying the splits for a transaction.</summary>
		public static List<JournalEntry> GetForTrans(long transactionNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<JournalEntry>>(MethodBase.GetCurrentMethod(),transactionNum);
			}
			string command=
				"SELECT * FROM journalentry "
				+"WHERE TransactionNum="+POut.Long(transactionNum);
			return Crud.JournalEntryCrud.SelectMany(command);
		}

		///<summary>Used to display a list of entries for one account. Even though we're passing in a dateFrom, we always get full list for assets, liabilities, and equity in order to get the running total, even if we don't show those rows.</summary>
		public static List<JournalEntry> GetForAccount(Account account,DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<JournalEntry>>(MethodBase.GetCurrentMethod(),account,dateFrom,dateTo);
			}
			string command;
			List<JournalEntry> listJournalEntries=new List<JournalEntry>();
			#region StartingBalanceRow
			//Get history balance and create a starting balance row.
			DateTime dateFirstOfYearFrom =new DateTime(dateFrom.Year,1,1);
			DateTime dateFirstOfYearTo=new DateTime(dateTo.Year,1,1);
			command="SELECT SUM(ROUND(CreditAmt,3)) SumCredit, "
				+"SUM(ROUND(DebitAmt,3)) SumDebit "
				+"FROM journalentry "
				+"WHERE AccountNum='"+POut.Long(account.AccountNum)+"' "
				+"AND DateDisplayed < "+POut.Date(dateFrom);
			if(account.AcctType.In(AccountType.Income,AccountType.Expense)){
				//For Income and Expense, if their dateFrom is not 1/1, then we might need a starting balance for part of a year.
				//Usually, with a 1/1 start date, this will result in no range, and a $0 starting row that doesn't show.
				command+=" AND DateDisplayed >= "+POut.Date(dateFirstOfYearFrom);
			}
			DataTable table=Db.GetTable(command);//always exactly one row
			double credit=PIn.Double(table.Rows[0]["SumCredit"].ToString());
			double debit=PIn.Double(table.Rows[0]["SumDebit"].ToString());
			double balStart = 0;
			if(Accounts.DebitIsPos(account.AcctType)) {
				balStart=debit-credit;
			}
			else {
				balStart=credit-debit;
			}
			double amtBalRE=0;
			if(account.IsRetainedEarnings){
				//Now we need an entry that's the sum all previous RE entries
				command="SELECT SUM(ROUND(CreditAmt,3))-SUM(ROUND(DebitAmt,3)) "
					+"FROM journalentry,account "
					+"WHERE journalentry.AccountNum=account.AccountNum "
					+"AND (account.AcctType='"+POut.Enum(AccountType.Income)+"' "
					+"OR account.AcctType='"+POut.Enum(AccountType.Expense)+"') "
					+"AND DateDisplayed < "+POut.Date(dateFirstOfYearFrom);
				amtBalRE=PIn.Double(Db.GetCount(command));//always a single cell
			}
			balStart+=amtBalRE;//no change for non-RE
			JournalEntry journalEntry=new JournalEntry();
			if(dateFrom.Year>1880){
				journalEntry.DateDisplayed=dateFrom.AddDays(-1);
			}
			journalEntry.Memo=Lans.g("FormJournal","(starting balance)");
			if(Accounts.DebitIsPos(account.AcctType)) {
				if(balStart>=0){
					journalEntry.DebitAmt=balStart;
				}
				else{
					journalEntry.CreditAmt=balStart;
				}
			}
			else{
				if(balStart>=0){
					journalEntry.CreditAmt=balStart;
				}
				else{
					journalEntry.DebitAmt=balStart;
				}
			}
			//The debit or credit will be used later to arrive at a starting bal to show
			listJournalEntries.Add(journalEntry);
			#endregion StartingBalanceRow
			#region RetainedEarningsAutoEntries
			if(account.IsRetainedEarnings){
				//For Retained Earnings, add the auto entries for each year
				//Only show the ones in our date range.
				//RE entries prior to our date range are already included in starting bal.
				//This will normally return no rows, unless date span is greater than one year.
				//dateFrom could be empty, so 1/1/1
				command="SELECT SUM(ROUND(CreditAmt,3))-SUM(ROUND(DebitAmt,3)) AS Amount, "
					+"YEAR(journalentry.DateDisplayed) AS yearDis "
					+"FROM journalentry,account "
					+"WHERE journalentry.AccountNum=account.AccountNum "
					+"AND (account.AcctType='"+POut.Enum(AccountType.Income)+"' "
					+"OR account.AcctType='"+POut.Enum(AccountType.Expense)+"') "
					+"AND DateDisplayed < "+POut.Date(dateFirstOfYearTo)+" "
					+"AND DateDisplayed >= "+POut.Date(dateFirstOfYearFrom)+" "
					+"GROUP BY yearDis";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					journalEntry=new JournalEntry();
					int year=PIn.Int(table.Rows[i]["yearDis"].ToString());
					journalEntry.DateDisplayed=new DateTime(year,12,31);
					double amount=PIn.Double(table.Rows[i]["Amount"].ToString());
					if(amount>0){
						journalEntry.CreditAmt=amount;
					}
					else{
						journalEntry.DebitAmt=-amount;
					}
					journalEntry.Memo=Lans.g("FormJournal","(auto)");
					listJournalEntries.Add(journalEntry);
				}
			}
			#endregion RetainedEarningsAutoEntries
			#region ExpenseIncomeAutoEntries
			//For income and expense accounts, if our range showing crosses any annual boundaries,
			//then we need to have an auto entry at each of those points to zero out the running balance
			if(account.AcctType.In(AccountType.Income,AccountType.Expense)){
				command="SELECT SUM(ROUND(CreditAmt,3))-SUM(ROUND(DebitAmt,3)) AS Amount, "
					+"YEAR(DateDisplayed) AS yearDis "
					+"FROM journalentry "
					+"WHERE AccountNum='"+POut.Long(account.AccountNum)+"' "
					+"AND DateDisplayed < "+POut.Date(dateFirstOfYearTo)+" "
					+"AND DateDisplayed >= "+POut.Date(dateFirstOfYearFrom)+" "
					+"GROUP BY yearDis";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					journalEntry=new JournalEntry();
					int year=PIn.Int(table.Rows[i]["yearDis"].ToString());
					journalEntry.DateDisplayed=new DateTime(year,12,31);
					double amount=PIn.Double(table.Rows[i]["Amount"].ToString());
					if(Accounts.DebitIsPos(account.AcctType)) {
						if(amount>0){
							journalEntry.DebitAmt=amount;
						}
						else{
							journalEntry.CreditAmt=-amount;
						}
					}
					else{
						if(amount>0){
							journalEntry.CreditAmt=amount;
						}
						else{
							journalEntry.DebitAmt=-amount;
						}
					}
					journalEntry.Memo=Lans.g("FormJournal","(auto)");
					listJournalEntries.Add(journalEntry);
				}
			}
			#endregion ExpenseIncomeAutoEntries
			command=
				"SELECT * FROM journalentry "
				+"WHERE AccountNum='"+POut.Long(account.AccountNum)+"' "
				+"AND DateDisplayed >= "+POut.Date(dateFrom)+" "
				+"AND DateDisplayed <= "+POut.Date(dateTo)+" "
				+"ORDER BY DateDisplayed";
			listJournalEntries.AddRange(Crud.JournalEntryCrud.SelectMany(command));
			listJournalEntries=listJournalEntries.OrderBy(x=>x.DateDisplayed)
				.ThenByDescending(x=>x.AccountNum).ToList();//this makes the auto entry come after other entries on that date
			return listJournalEntries;
		}

		///<summary>Used in reconcile window.</summary>
		public static List <JournalEntry> GetForReconcile(long accountNum,bool includeUncleared,long reconcileNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List <JournalEntry>>(MethodBase.GetCurrentMethod(),accountNum,includeUncleared,reconcileNum);
			}
			string command=
				"SELECT * FROM journalentry "
				+"WHERE AccountNum="+POut.Long(accountNum)
				+" AND (ReconcileNum="+POut.Long(reconcileNum);
			if(includeUncleared) {
				command+=" OR ReconcileNum=0)";
			}
			else {
				command+=")";
			}
			command+=" ORDER BY DateDisplayed";
			return Crud.JournalEntryCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(JournalEntry je) {
			je.SecUserNumEntry=Security.CurUser.UserNum;//Before middle tier check to catch user at workstation
			je.SecUserNumEdit=Security.CurUser.UserNum;
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				je.JournalEntryNum=Meth.GetLong(MethodBase.GetCurrentMethod(),je);
				return je.JournalEntryNum;
			}
			if(je.DebitAmt<0 || je.CreditAmt<0){
				throw new ApplicationException(Lans.g("JournalEntries","Error. Credit and debit must both be positive."));
			}
			return Crud.JournalEntryCrud.Insert(je);
		}

		///<summary></summary>
		public static void Update(JournalEntry je) {
			je.SecUserNumEdit=Security.CurUser.UserNum;//Before middle tier check to catch user at workstation
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),je);
				return;
			}
			if(je.DebitAmt<0 || je.CreditAmt<0) {
				throw new ApplicationException(Lans.g("JournalEntries","Error. Credit and debit must both be positive."));
			}
			Crud.JournalEntryCrud.Update(je);
		}

		///<summary></summary>
		public static void Delete(JournalEntry je) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),je);
				return;
			}
			//This method is only used once in synch below.  Validation needs to be done, but doing it inside the loop would be dangerous.
			//So validation is done in the UI as follows:
			//1. Deleting an entire transaction is validated in business layer.
			//2. When editing a transaction attached to reconcile, simple view is blocked.
			//3. Double clicking on grid lets you change JEs not attached to reconcile.
			//4. Double clicking on grid lets you change notes even if attached to reconcile.
			string command= "DELETE FROM journalentry WHERE JournalEntryNum = "+POut.Long(je.JournalEntryNum);
			Db.NonQ(command);
		}

		///<summary>Used in FormTransactionEdit to synch database with changes user made to the journalEntry list for a transaction.  Must supply an old list for comparison.  Only the differences are saved.  Surround with try/catch, because it will thrown an exception if any entries are negative.</summary>
		public static void UpdateList(List<JournalEntry> oldJournalList,List<JournalEntry> newJournalList) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<newJournalList.Count;i++){
				if(newJournalList[i].DebitAmt<0 || newJournalList[i].CreditAmt<0){
					throw new ApplicationException(Lans.g("JournalEntries","Error. Credit and debit must both be positive."));
				}
			}
			JournalEntry newJournalEntry;
			for(int i=0;i<oldJournalList.Count;i++) {//loop through the old list
				newJournalEntry=null;
				for(int j=0;j<newJournalList.Count;j++) {
					if(newJournalList[j]==null || newJournalList[j].JournalEntryNum==0) {
						continue;
					}
					if(oldJournalList[i].JournalEntryNum==newJournalList[j].JournalEntryNum) {
						newJournalEntry=newJournalList[j];
						break;
					}
				}
				if(newJournalEntry==null) {
					//journalentry with matching journalEntryNum was not found, so it must have been deleted
					Delete((JournalEntry)oldJournalList[i]);
					continue;
				}
				//journalentry was found with matching journalEntryNum, so check for changes
				if(newJournalEntry.AccountNum != oldJournalList[i].AccountNum
					|| newJournalEntry.DateDisplayed != oldJournalList[i].DateDisplayed
					|| newJournalEntry.DebitAmt != oldJournalList[i].DebitAmt
					|| newJournalEntry.CreditAmt != oldJournalList[i].CreditAmt
					|| newJournalEntry.Memo != oldJournalList[i].Memo
					|| newJournalEntry.Splits != oldJournalList[i].Splits
					|| newJournalEntry.CheckNumber!= oldJournalList[i].CheckNumber) 
				{
					Update(newJournalEntry);
				}
			}
			for(int i=0;i<newJournalList.Count;i++) {//loop through the new list
				if(newJournalList[i]==null) {
					continue;
				}
				if(newJournalList[i].JournalEntryNum!=0) {
					continue;
				}
				//entry with journalEntryNum=0, so it's new
				Insert(newJournalList[i]);
			}
		}

		///<summary>Called from FormTransactionEdit.</summary>
		public static bool AttachedToReconcile(List<JournalEntry> journalList){
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<journalList.Count;i++){
				if(journalList[i].ReconcileNum!=0){
					return true;
				}
			}
			return false;
		}

		///<summary>Called from FormTransactionEdit.</summary>
		public static DateTime GetReconcileDate(List<JournalEntry> journalList) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<journalList.Count;i++) {
				if(journalList[i].ReconcileNum!=0) {
					return Reconciles.GetOne(journalList[i].ReconcileNum).DateReconcile;
				}
			}
			return DateTime.MinValue;
		}

		///<summary>Called once from FormReconcileEdit when closing.  Saves the reconcileNum for every item in the list.</summary>
		public static void SaveList(List <JournalEntry> journalList,long reconcileNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),journalList,reconcileNum);
				return;
			}
			string command="UPDATE journalentry SET ReconcileNum=0 WHERE";
			string str="";
			for(int i=0;i<journalList.Count;i++){
				if(journalList[i].ReconcileNum==0){
					if(str!=""){
						str+=" OR";
					}
					str+=" JournalEntryNum="+POut.Long(journalList[i].JournalEntryNum);
				}
			}
			if(str!=""){
				command+=str;
				Db.NonQ(command);
			}
			command="UPDATE journalentry SET ReconcileNum="+POut.Long(reconcileNum)+" WHERE";
			str="";
			for(int i=0;i<journalList.Count;i++) {
				if(journalList[i].ReconcileNum==reconcileNum) {
					if(str!="") {
						str+=" OR";
					}
					str+=" JournalEntryNum="+POut.Long(journalList[i].JournalEntryNum);
				}
			}
			if(str!=""){
				command+=str;
				Db.NonQ(command);
			}
		}

		///<Summary>Returns true if the account was used in any journal entry.</Summary>
		public static bool IsInUse(long accountNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),accountNum);
			}
			string command="SELECT COUNT(*) FROM journalentry "
				+"WHERE AccountNum="+POut.Long(accountNum);
			string count=Db.GetCount(command);
			if(count=="0"){
				return false;
			}
			return true;
		}

		
	}
}




