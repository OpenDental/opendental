using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;

namespace OpenDentBusiness{

	public class Accounts {
		#region Cache Pattern

		private class AccountCache:CacheListAbs<Account> {
			protected override Account Copy(Account account) {
				return account.Clone();
			}

			protected override void FillCacheIfNeeded() {
				Accounts.GetTableFromCache(false);
			}

			protected override List<Account> GetCacheFromDb() {
				string command="SELECT * FROM account ORDER BY AcctType,Description";
				return Crud.AccountCrud.SelectMany(command);
			}

			protected override DataTable ListToTable(List<Account> listAccounts) {
				return Crud.AccountCrud.ListToTable(listAccounts,"Account");
			}

			protected override List<Account> TableToList(DataTable table) {
				return Crud.AccountCrud.TableToList(table);
			}

			protected override bool IsInListShort(Account account) {
				return !account.Inactive;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AccountCache _accountCache=new AccountCache();

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		public static List<Account> GetDeepCopy(bool isShort=false) {
			return _accountCache.GetDeepCopy(isShort);
		}

		public static Account GetFirstOrDefault(Func<Account,bool> match,bool isShort=false) {
			return _accountCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_accountCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_accountCache.FillCacheFromTable(table);
				return table;
			}
			return _accountCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(Account acct) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				acct.AccountNum=Meth.GetLong(MethodBase.GetCurrentMethod(),acct);
				return acct.AccountNum;
			}
			return Crud.AccountCrud.Insert(acct);
		}

		///<summary>Does not update existing journal splits. To that use the other overload for this method.</summary>
		public static void Update(Account acct) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),acct);
				return;
			}
			Crud.AccountCrud.Update(acct);
		}

		///<summary>Also updates existing journal entry splits linked to this account that have not been locked.</summary>
		public static void Update(Account acct,Account acctOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),acct,acctOld);
				return;
			}
			Crud.AccountCrud.Update(acct,acctOld);
			if(acct.Description==acctOld.Description) {
				return;//No need to update splits on attached journal entries.
			}
			//The account was renamed, so update journalentry.Splits.
			string command=@"SELECT je2.*,account.Description
					FROM journalentry 
					INNER JOIN journalentry je2 ON je2.TransactionNum=journalentry.TransactionNum
					INNER JOIN account ON account.AccountNum=je2.AccountNum
					WHERE journalentry.AccountNum="+POut.Long(acct.AccountNum)+@"
					AND journalentry.DateDisplayed > "+POut.Date(PrefC.GetDate(PrefName.AccountingLockDate))+@"
					ORDER BY je2.TransactionNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return;
			}
			List<JournalEntry> listJournalEntries=Crud.JournalEntryCrud.TableToList(table);
			//Construct a dictionary that has the description for each JournalEntryNum.
			Dictionary<long,string> dictJournalEntryDescriptions=table.Rows.Cast<DataRow>()
				.GroupBy(x => PIn.Long(x["JournalEntryNum"].ToString()))
				.ToDictionary(x => x.Key,x => PIn.String(x.First()["Description"].ToString()));
			//Now we will loop through all the journal entries and find the other journal entries that are attached to the same transaction and update
			//those splits.
			List<int> listIndexesForTrans=new List<int>();
			long curTransactionNum=listJournalEntries[0].TransactionNum;
			for(int i=0;i<listJournalEntries.Count;i++) {
				if(listJournalEntries[i].TransactionNum==curTransactionNum) {
					listIndexesForTrans.Add(i);
					continue;
				}
				UpdateJournalEntrySplits(listJournalEntries,listIndexesForTrans,dictJournalEntryDescriptions,acct);
				curTransactionNum=listJournalEntries[i].TransactionNum;
				listIndexesForTrans.Clear();
				listIndexesForTrans.Add(i);
			}
			UpdateJournalEntrySplits(listJournalEntries,listIndexesForTrans,dictJournalEntryDescriptions,acct);
		}

		///<summary>Updates the splits on the journal entries whose indexes are passed in.</summary>
		///<param name="listJournalEntries">All journal entries for a particular account.</param>
		///<param name="listIndexesForTrans">The index of the journal entries in listJournalEntries. These are the ones that will be updated.</param>
		///<param name="dictJournalEntryDescriptions">A dictionary where the key is the JournalEntryNum and the value is the journal entry's 
		///account description.</param>
		///<param name="acct">The account that whose description is being updates.</param>
		private static void UpdateJournalEntrySplits(List<JournalEntry> listJournalEntries,List<int> listIndexesForTrans,
			Dictionary<long,string> dictJournalEntryDescriptions,Account acct) 
		{
			//No need to check MiddleTierRole; no call to db.
			foreach(int index in listIndexesForTrans.Where(x => listJournalEntries[x].AccountNum != acct.AccountNum)) {
				JournalEntry journalEntry=listJournalEntries[index];
				if(listIndexesForTrans.Count <= 2) {
					//When a transaction only has two splits, the Splits column will simply be the name of the account of the other split.
					journalEntry.Splits=acct.Description;
				}
				else {
					//When a transaction has three or more splits, the Splits column will be the names of the account and the amount of the other splits.
					//Ex.: 
					//Patient Fee Income 85.00
					//Supplies 110.00
					journalEntry.Splits=string.Join("\r\n",listIndexesForTrans
						.Where(x => listJournalEntries[x].JournalEntryNum != journalEntry.JournalEntryNum)
						.Select(x => dictJournalEntryDescriptions[listJournalEntries[x].JournalEntryNum]+" "+
						(listJournalEntries[x].DebitAmt > 0 ?
							listJournalEntries[x].DebitAmt.ToString("n") :
							listJournalEntries[x].CreditAmt.ToString("n"))));
				}
				JournalEntries.Update(journalEntry);
			}
		}


		///<summary>Loops through listLong to find a description for the specified account.  0 returns an empty string.</summary>
		public static string GetDescript(long accountNum) {
			//No need to check MiddleTierRole; no call to db.
			Account account=GetFirstOrDefault(x => x.AccountNum==accountNum);
			return (account==null ? "" : account.Description);
		}

		///<summary>Loops through listLong to find an account.  Will return null if accountNum is 0.</summary>
		public static Account GetAccount(long accountNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetFirstOrDefault(x => x.AccountNum==accountNum);
		}

		///<summary>Throws exception if account is in use.</summary>
		public static void Delete(Account acct) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),acct);
				return;
			}
			//check to see if account has any journal entries
			string command="SELECT COUNT(*) FROM journalentry WHERE AccountNum="+POut.Long(acct.AccountNum);
			if(Db.GetCount(command)!="0") {
				throw new ApplicationException(Lans.g("FormAccountEdit",
					"Not allowed to delete an account with existing journal entries."));
			}
			//Check various preference entries
			command="SELECT ValueString FROM preference WHERE PrefName='AccountingDepositAccounts'";
			string result=Db.GetCount(command);
			string[] strArray=result.Split(new char[] { ',' });
			for(int i=0;i<strArray.Length;i++) {
				if(strArray[i]==acct.AccountNum.ToString()) {
					throw new ApplicationException(Lans.g("FormAccountEdit","Account is in use in the setup section."));
				}
			}
			command="SELECT ValueString FROM preference WHERE PrefName='AccountingIncomeAccount'";
			result=Db.GetCount(command);
			if(result==acct.AccountNum.ToString()) {
				throw new ApplicationException(Lans.g("FormAccountEdit","Account is in use in the setup section."));
			}
			command="SELECT ValueString FROM preference WHERE PrefName='AccountingCashIncomeAccount'";
			result=Db.GetCount(command);
			if(result==acct.AccountNum.ToString()) {
				throw new ApplicationException(Lans.g("FormAccountEdit","Account is in use in the setup section."));
			}
			//check AccountingAutoPay entries
			List<AccountingAutoPay> listAutoPays=AccountingAutoPays.GetDeepCopy();
			for(int i=0;i<listAutoPays.Count;i++) {
				strArray=listAutoPays[i].PickList.Split(new char[] { ',' });
				for(int s=0;s<strArray.Length;s++) {
					if(strArray[s]==acct.AccountNum.ToString()) {
						throw new ApplicationException(Lans.g("FormAccountEdit","Account is in use in the setup section."));
					}
				}
			}
			command="DELETE FROM account WHERE AccountNum = "+POut.Long(acct.AccountNum);
			Db.NonQ(command);
		}

		///<summary>Used to test the sign on debits and credits for the five different account types</summary>
		public static bool DebitIsPos(AccountType type) {
			//No need to check MiddleTierRole; no call to db.
			switch(type) {
				case AccountType.Asset:
				case AccountType.Expense:
					return true;
				case AccountType.Liability:
				case AccountType.Equity://because liabilities and equity are treated the same
				case AccountType.Income:
					return false;
			}
			return true;//will never happen
		}

		///<summary>Gets the balance of an account directly from the database.</summary>
		public static double GetBalance(long accountNum,AccountType acctType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<double>(MethodBase.GetCurrentMethod(),accountNum,acctType);
			}
			string command="SELECT SUM(DebitAmt),SUM(CreditAmt) FROM journalentry "
				+"WHERE AccountNum="+POut.Long(accountNum)
				+" GROUP BY AccountNum";
			DataTable table=Db.GetTable(command);
			double debit=0;
			double credit=0;
			if(table.Rows.Count>0) {
				debit=PIn.Double(table.Rows[0][0].ToString());
				credit=PIn.Double(table.Rows[0][1].ToString());
			}
			if(DebitIsPos(acctType)) {
				return debit-credit;
			}
			else {
				return credit-debit;
			}
			/*}
			catch {
				Debug.WriteLine(command);
				MessageBox.Show(command);
			}
			return 0;*/
		}

		///<summary>Checks the loaded prefs to see if user has setup deposit linking.  Returns true if so.</summary>
		public static bool DepositsLinked() {
			//No need to check MiddleTierRole; no call to db.
			int prefAccountingSoftware=PrefC.GetInt(PrefName.AccountingSoftware);
			if(prefAccountingSoftware==(int)AccountingSoftware.QuickBooks) {
				if(PrefC.GetString(PrefName.QuickBooksDepositAccounts)=="") {
					return false;
				}
				if(PrefC.GetString(PrefName.QuickBooksIncomeAccount)=="") {
					return false;
				}
			}
			else if(prefAccountingSoftware==(int)AccountingSoftware.QuickBooksOnline) {
				Program progQbo=Programs.GetCur(ProgramName.QuickBooksOnline);
				ProgramProperty programPropQboDepositAccounts=ProgramProperties.GetPropForProgByDesc(progQbo.ProgramNum,"Deposit Accounts");
				if(programPropQboDepositAccounts.PropertyValue=="") {
					return false;
				}
				ProgramProperty programPropQboIncomeAccounts=ProgramProperties.GetPropForProgByDesc(progQbo.ProgramNum,"Income Accounts");
				if(programPropQboIncomeAccounts.PropertyValue=="") {
					return false;
				}
			}
			else {
				if(PrefC.GetString(PrefName.AccountingDepositAccounts)=="") {
					return false;
				}
				if(PrefC.GetLong(PrefName.AccountingIncomeAccount)==0) {
					return false;
				}
			}
			//might add a few more checks later.
			return true;
		}

		///<summary>Checks the loaded prefs and accountingAutoPays to see if user has setup auto pay linking.  Returns true if so.</summary>
		public static bool PaymentsLinked() {
			//No need to check MiddleTierRole; no call to db.
			if(AccountingAutoPays.GetCount()==0) {
				return false;
			}
			if(PrefC.GetLong(PrefName.AccountingCashIncomeAccount)==0) {
				return false;
			}
			//might add a few more checks later.
			return true;
		}

		///<summary></summary>
		public static List<long> GetDepositAccounts() {
			//No need to check MiddleTierRole; no call to db.
			string depStr=PrefC.GetString(PrefName.AccountingDepositAccounts);
			List<string> listStrDepositAccounts=depStr.Split(new char[] { ',' }).ToList();
			List<long> listDepositAccounts=new List<long>();
			for(int i=0;i<listStrDepositAccounts.Count;i++) {
				if(listStrDepositAccounts[i]=="") {
					continue;
				}
				listDepositAccounts.Add(PIn.Long(listStrDepositAccounts[i]));
			}
			return listDepositAccounts;
		}

		///<summary></summary>
		public static List<string> GetDepositAccountsQB() {
			//No need to check MiddleTierRole; no call to db.
			string depStr=PrefC.GetString(PrefName.QuickBooksDepositAccounts);
			string[] depStrArray=depStr.Split(new char[] { ',' });
			List<string> retVal=new List<string>();
			for(int i=0;i<depStrArray.Length;i++) {
				if(depStrArray[i]=="") {
					continue;
				}
				retVal.Add(depStrArray[i]);
			}
			return retVal;
		}

		///<summary></summary>
		public static List<string> GetIncomeAccountsQB() {
			//No need to check MiddleTierRole; no call to db.
			string incomeStr=PrefC.GetString(PrefName.QuickBooksIncomeAccount);
			string[] incomeStrArray=incomeStr.Split(new char[] { ',' });
			List<string> retVal=new List<string>();
			for(int i=0;i<incomeStrArray.Length;i++) {
				if(incomeStrArray[i]=="") {
					continue;
				}
				retVal.Add(incomeStrArray[i]);
			}
			return retVal;
		}

		///<summary>Gets automatic Retained Earnings for all previous years combined into one number.  Does not include any for the current year showing.</summary>
		public static decimal GetRE_PreviousYears(DateTime dateAsOf) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<decimal>(MethodBase.GetCurrentMethod(),dateAsOf);
			}
			DateTime dateFirstofYear=new DateTime(dateAsOf.Year,1,1);
			//this works for both income and expenses, because we are subracting expenses, so signs cancel
			string command="SELECT SUM(CreditAmt-DebitAmt) "
				+"FROM account,journalentry "
				+"WHERE journalentry.AccountNum=account.AccountNum "
				+"AND DateDisplayed < "+POut.Date(dateFirstofYear)//all from previous years
				+" AND (AcctType="+(int)AccountType.Income+" OR AcctType="+(int)AccountType.Expense+")";
			string strBal=Db.GetCount(command);
			decimal balanceRE=PIn.Decimal(strBal);
			return balanceRE;
		}

		///<summary>Gets the full list to display in the Chart of Accounts, including balances.</summary>
		public static DataTable GetFullList(DateTime dateAsOf,bool showInactive) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateAsOf,showInactive);
			}
			DataTable table=new DataTable("Accounts");
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("type");
			table.Columns.Add("Description");
			table.Columns.Add("balance");
			table.Columns.Add("BankNumber");
			table.Columns.Add("inactive");
			table.Columns.Add("color");
			table.Columns.Add("AccountNum");
			//but we won't actually fill this table with rows until the very end.  It's more useful to use a List<> for now.
			List<DataRow> listRows=new List<DataRow>();
			DateTime dateFirstofYear=new DateTime(dateAsOf.Year,1,1);
			//First, get the retained earnings balance
			decimal balanceRE=GetRE_PreviousYears(dateAsOf);
			//Next, the entire history for the asset, liability, and equity accounts, including Retained Earnings-----------
			string command="SELECT account.AcctType, account.Description, account.AccountNum,account.IsRetainedEarnings, "
				+"SUM(DebitAmt) AS SumDebit, SUM(CreditAmt) AS SumCredit, account.BankNumber, account.Inactive, account.AccountColor "
				+"FROM account "
				+"LEFT JOIN journalentry ON journalentry.AccountNum=account.AccountNum AND "
				+"DateDisplayed <= "+POut.Date(dateAsOf)
				+" WHERE AcctType<=2 ";
			if(!showInactive) {
				command+="AND Inactive=0 ";
			}
			command+="GROUP BY account.AccountNum, account.AcctType, account.Description, account.BankNumber,"
				+"account.Inactive, account.AccountColor ORDER BY AcctType, Description";
			DataTable tableRaw=Db.GetTable(command);
			AccountType accountType;
			decimal debit;
			decimal credit;
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				row=table.NewRow();
				accountType=(AccountType)PIn.Long(tableRaw.Rows[i]["AcctType"].ToString());
				bool isRetainedEarnings=PIn.Bool(tableRaw.Rows[i]["IsRetainedEarnings"].ToString());
				row["type"]=Lans.g("enumAccountType",accountType.ToString());
				row["Description"]=tableRaw.Rows[i]["Description"].ToString();
				debit=PIn.Decimal(tableRaw.Rows[i]["SumDebit"].ToString());
				credit=PIn.Decimal(tableRaw.Rows[i]["SumCredit"].ToString());
				if(isRetainedEarnings){
					row["balance"]=(credit-debit+balanceRE).ToString("N");
				}
				else if(DebitIsPos(accountType)) {
					row["balance"]=(debit-credit).ToString("N");
				}
				else {
					row["balance"]=(credit-debit).ToString("N");
				}
				row["BankNumber"]=tableRaw.Rows[i]["BankNumber"].ToString();
				if(tableRaw.Rows[i]["Inactive"].ToString()=="0") {
					row["inactive"]="";
				}
				else {
					row["inactive"]="X";
				}
				row["color"]=tableRaw.Rows[i]["AccountColor"].ToString();//it will be an unsigned int at this point.
				row["AccountNum"]=tableRaw.Rows[i]["AccountNum"].ToString();
				listRows.Add(row);
			}
			//finally, income and expenses------------------------------------------------------------------------------
			command="SELECT account.AcctType, account.Description, account.AccountNum, "
				+"SUM(DebitAmt) AS SumDebit, SUM(CreditAmt) AS SumCredit, account.BankNumber, account.Inactive, account.AccountColor "
				+"FROM account "
				+"LEFT JOIN journalentry ON journalentry.AccountNum=account.AccountNum "
				+"AND DateDisplayed <= "+POut.Date(dateAsOf)
				+" AND DateDisplayed >= "+POut.Date(dateFirstofYear)//only for this year
				+" WHERE (AcctType=3 OR AcctType=4) ";
			if(!showInactive) {
				command+="AND Inactive=0 ";
			}
			command+="GROUP BY account.AccountNum, account.AcctType, account.Description, account.BankNumber,"
				+"account.Inactive, account.AccountColor ORDER BY AcctType, Description";
			tableRaw=Db.GetTable(command);
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				row=table.NewRow();
				accountType=(AccountType)PIn.Long(tableRaw.Rows[i]["AcctType"].ToString());
				row["type"]=Lans.g("enumAccountType",accountType.ToString());
				row["Description"]=tableRaw.Rows[i]["Description"].ToString();
				debit=PIn.Decimal(tableRaw.Rows[i]["SumDebit"].ToString());
				credit=PIn.Decimal(tableRaw.Rows[i]["SumCredit"].ToString());
				if(DebitIsPos(accountType)) {
					row["balance"]=(debit-credit).ToString("N");
				}
				else {
					row["balance"]=(credit-debit).ToString("N");
				}
				row["BankNumber"]=tableRaw.Rows[i]["BankNumber"].ToString();
				if(tableRaw.Rows[i]["Inactive"].ToString()=="0") {
					row["inactive"]="";
				}
				else {
					row["inactive"]="X";
				}
				row["color"]=tableRaw.Rows[i]["AccountColor"].ToString();//it will be an unsigned int at this point.
				row["AccountNum"]=tableRaw.Rows[i]["AccountNum"].ToString();
				listRows.Add(row);
			}
			for(int i=0;i<listRows.Count;i++) {
				table.Rows.Add(listRows[i]);
			}
			return table;
		}

		///<summary>Gets the full GeneralLedger list, except for previous auto Retained Earnings.</summary>
		public static DataTable GetGeneralLedger(DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string queryString=@"SELECT DATE("+POut.Date(new DateTime(dateStart.Year-1,12,31))+@") DateDisplayed,
				'' Memo,
				'' Splits,
				'' CheckNumber,
				startingbals.SumTotal DebitAmt,
				0 CreditAmt,
				'' Balance,
				startingbals.Description,
				startingbals.AcctType,
				startingbals.AccountNum
				FROM (
					SELECT account.AccountNum,
					account.Description,
					account.AcctType,
					ROUND(SUM(journalentry.DebitAmt-journalentry.CreditAmt),2) SumTotal
					FROM account
					INNER JOIN journalentry ON journalentry.AccountNum=account.AccountNum
					AND journalentry.DateDisplayed < "+POut.Date(dateStart)+@" 
					AND account.AcctType IN (0,1,2)/*assets,liablities,equity*/
					GROUP BY account.AccountNum
				) startingbals

				UNION ALL
	
				SELECT journalentry.DateDisplayed,
				journalentry.Memo,
				journalentry.Splits,
				journalentry.CheckNumber,
				journalentry.DebitAmt, 
				journalentry.CreditAmt,
				'' Balance,
				account.Description,
				account.AcctType,
				account.AccountNum 
				FROM account
				LEFT JOIN journalentry ON account.AccountNum=journalentry.AccountNum 
					AND journalentry.DateDisplayed >= "+POut.Date(dateStart)+@" 
					AND journalentry.DateDisplayed <= "+POut.Date(dateEnd)+@" 
				
				ORDER BY AcctType, Description, DateDisplayed;";
			return Db.GetTable(queryString);
		}

		///<summary>Used in balance sheet report. Only works for asset, liability, and equity, not income or expense.</summary>
		public static DataTable GetAccountTotalByType(DateTime dateAsOf,AccountType accountType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateAsOf,accountType);
			}
			string command="SELECT Description, ";
			if(accountType==AccountType.Asset) {
				command+="SUM(ROUND(DebitAmt,3)-ROUND(CreditAmt,3))";
			}
			else {//Liability or equity
				command+="SUM(ROUND(CreditAmt,3)-ROUND(DebitAmt,3))";
			}
			command+=" AS SumTotal, AcctType, IsRetainedEarnings,Inactive "//Inactive won't show
				+"FROM account, journalentry "
				+"WHERE account.AccountNum=journalentry.AccountNum "
				+"AND DateDisplayed <= "+POut.Date(dateAsOf)+" "
				+"AND AcctType="+POut.Int((int)accountType)+" "
				+"GROUP BY account.AccountNum "
				+"HAVING (SumTotal<>0 OR Inactive=0) "//either a bal or active
				+"ORDER BY Description, DateDisplayed ";
			DataTable table=Db.GetTable(command);
			if(accountType!=AccountType.Equity){
				return table;
			}
			//For equity, get the RE balance from all previous years
			decimal balanceRE=0;
			if(accountType==AccountType.Equity){
				balanceRE=GetRE_PreviousYears(dateAsOf);
			}
			for(int i=0;i<table.Rows.Count;i++){
				bool isRE=PIn.Bool(table.Rows[i]["IsRetainedEarnings"].ToString());
				if(!isRE){ 
					continue; //only one of the equity accounts is the RE
				}
				decimal amt=PIn.Decimal(table.Rows[i]["SumTotal"].ToString())
					+balanceRE;
				table.Rows[i]["SumTotal"]=amt;
				break;
			}
			return table;
		}

		public static DataTable GetAccountTotalByType(DateTime dateStart,DateTime dateEnd,AccountType acctType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,acctType);
			}
			string sumTotalStr="";
			if(acctType==AccountType.Expense) {
				sumTotalStr="SUM(ROUND(DebitAmt,3)-ROUND(CreditAmt,3))";
			}
			else {//Income instead of expense
				sumTotalStr="SUM(ROUND(CreditAmt,3)-ROUND(DebitAmt,3))";
			}
			string command="SELECT Description, "+sumTotalStr+" SumTotal, AcctType "
				+"FROM account, journalentry "
				+"WHERE account.AccountNum=journalentry.AccountNum AND DateDisplayed >= "+POut.Date(dateStart)+" "
				+"AND DateDisplayed <= "+POut.Date(dateEnd)+" "
				+"AND AcctType="+POut.Int((int)acctType)+" "
				+"GROUP BY account.AccountNum "
				+"ORDER BY Description, DateDisplayed ";
			return Db.GetTable(command);
		}

		///<Summary>asOfDate is typically 12/31/...  </Summary>
		public static double NetIncomeThisYear(DateTime asOfDate) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod(),asOfDate);
			}
			DateTime firstOfYear=new DateTime(asOfDate.Year,1,1);
			string command="SELECT SUM(ROUND(CreditAmt,3)), SUM(ROUND(DebitAmt,3)), AcctType "
			+"FROM journalentry,account "
			+"WHERE journalentry.AccountNum=account.AccountNum "
			+"AND DateDisplayed >= "+POut.Date(firstOfYear)
			+" AND DateDisplayed <= "+POut.Date(asOfDate)
			+" GROUP BY AcctType";
			DataTable table=Db.GetTable(command);
			double retVal=0;
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i][2].ToString()=="3"//income
					|| table.Rows[i][2].ToString()=="4")//expense
				{
					retVal+=PIn.Double(table.Rows[i][0].ToString());//add credit
					retVal-=PIn.Double(table.Rows[i][1].ToString());//subtract debit
					//if it's an expense, we are subtracting (income-expense), but the signs cancel.
				}
			}
			return retVal;
		}

		


	}
}




