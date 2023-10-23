using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Navigation;

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

		public static void ClearCache() {
			_accountCache.ClearCache();
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		public static List<Account> GetDeepCopy(bool isShort=false) {
			return _accountCache.GetDeepCopy(isShort);
		}

		public static Account GetFirstOrDefault(Func<Account,bool> funcMatch,bool isShort=false) {
			return _accountCache.GetFirstOrDefault(funcMatch,isShort);
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
		public static long Insert(Account account) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				account.AccountNum=Meth.GetLong(MethodBase.GetCurrentMethod(),account);
				return account.AccountNum;
			}
			return Crud.AccountCrud.Insert(account);
		}

		///<summary>Does not update existing journal splits. To that use the other overload for this method.</summary>
		public static void Update(Account account) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),account);
				return;
			}
			Crud.AccountCrud.Update(account);
		}

		///<summary>Also updates existing journal entry splits linked to this account that have not been locked.</summary>
		public static void Update(Account account,Account accountOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),account,accountOld);
				return;
			}
			Crud.AccountCrud.Update(account,accountOld);
			if(account.Description==accountOld.Description) {
				return;//No need to update splits on attached journal entries.
			}
			//The account was renamed, so update journalentry.Splits.
			string command=@"SELECT je2.*,account.Description
					FROM journalentry 
					INNER JOIN journalentry je2 ON je2.TransactionNum=journalentry.TransactionNum
					INNER JOIN account ON account.AccountNum=je2.AccountNum
					WHERE journalentry.AccountNum="+POut.Long(account.AccountNum)+@"
					AND journalentry.DateDisplayed > "+POut.Date(PrefC.GetDate(PrefName.AccountingLockDate))+@"
					ORDER BY je2.TransactionNum";//to group them
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return;
			}
			List<JournalEntry> listJournalEntries=Crud.JournalEntryCrud.TableToList(table);
			for(int i=0;i<listJournalEntries.Count;i++){
				listJournalEntries[i].DescriptionAccount=table.Rows[i]["Description"].ToString();
			}
			//Now we will loop through all the journal entries and find the other journal entries that are attached to the same transaction and update
			//those splits.
			List<long> listTransactionNums=listJournalEntries.Select(x=>x.TransactionNum).Distinct().ToList();
			for(int t=0;t<listTransactionNums.Count;t++) {
				List<JournalEntry> listJournalEntriesForTrans=listJournalEntries.FindAll(x=>x.TransactionNum==listTransactionNums[t]);
				for(int j=0;j<listJournalEntriesForTrans.Count;j++){
					if(listJournalEntriesForTrans[j].AccountNum==account.AccountNum){
						continue;
					}
					if(listJournalEntriesForTrans.Count==2) {
						//When a transaction only has two splits, the Splits column will simply be the name of the account of the other split.
						listJournalEntriesForTrans[j].Splits=account.Description;
						JournalEntries.Update(listJournalEntriesForTrans[j]);
						continue;
					}
					//When a transaction has three or more splits, the Splits column will be the names of the account and the amount of the other splits.
					//Ex.: 
					//Patient Fee Income 85.00
					//Supplies 110.00
					string updatedSplits="";
					for(int k=0;k<listJournalEntriesForTrans.Count;k++){
						if(listJournalEntriesForTrans[k].JournalEntryNum==listJournalEntriesForTrans[j].JournalEntryNum){
							//skipping self because we only want the other 2+ in the group
							continue;
						}
						string splitAmt=listJournalEntriesForTrans[k].CreditAmt.ToString("n");
						if(listJournalEntriesForTrans[k].DebitAmt>0){
							splitAmt=listJournalEntriesForTrans[k].DebitAmt.ToString("n");
						}
						updatedSplits+=listJournalEntriesForTrans[k].DescriptionAccount+" ";
						updatedSplits+=splitAmt;
							updatedSplits+="\r\n";
						}
					listJournalEntriesForTrans[j].Splits=updatedSplits;
					JournalEntries.Update(listJournalEntriesForTrans[j]);
				}//for j
			}//for t
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
		public static void Delete(Account account) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),account);
				return;
			}
			//check to see if account has any journal entries
			string command="SELECT COUNT(*) FROM journalentry WHERE AccountNum="+POut.Long(account.AccountNum);
			if(Db.GetCount(command)!="0") {
				throw new ApplicationException(Lans.g("FormAccountEdit",
					"Not allowed to delete an account with existing journal entries."));
			}
			//Check various preference entries
			command="SELECT ValueString FROM preference WHERE PrefName='AccountingDepositAccounts'";
			string result=Db.GetCount(command);
			string[] stringArray=result.Split(new char[] { ',' });
			for(int i=0;i<stringArray.Length;i++) {
				if(stringArray[i]==account.AccountNum.ToString()) {
					throw new ApplicationException(Lans.g("FormAccountEdit","Account is in use in the setup section."));
				}
			}
			command="SELECT ValueString FROM preference WHERE PrefName='AccountingIncomeAccount'";
			result=Db.GetCount(command);
			if(result==account.AccountNum.ToString()) {
				throw new ApplicationException(Lans.g("FormAccountEdit","Account is in use in the setup section."));
			}
			command="SELECT ValueString FROM preference WHERE PrefName='AccountingCashIncomeAccount'";
			result=Db.GetCount(command);
			if(result==account.AccountNum.ToString()) {
				throw new ApplicationException(Lans.g("FormAccountEdit","Account is in use in the setup section."));
			}
			//check AccountingAutoPay entries
			List<AccountingAutoPay> listAccountingAutoPays=AccountingAutoPays.GetDeepCopy();
			for(int i=0;i<listAccountingAutoPays.Count;i++) {
				stringArray=listAccountingAutoPays[i].PickList.Split(new char[] { ',' });
				for(int s=0;s<stringArray.Length;s++) {
					if(stringArray[s]==account.AccountNum.ToString()) {
						throw new ApplicationException(Lans.g("FormAccountEdit","Account is in use in the setup section."));
					}
				}
			}
			command="DELETE FROM account WHERE AccountNum = "+POut.Long(account.AccountNum);
			Db.NonQ(command);
		}

		///<summary>Used to test the sign on debits and credits for the five different account types</summary>
		public static bool DebitIsPos(AccountType accountType) {
			//No need to check MiddleTierRole; no call to db.
			switch(accountType) {
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
		public static double GetBalance(long accountNum,AccountType accountType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<double>(MethodBase.GetCurrentMethod(),accountNum,accountType);
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
			if(DebitIsPos(accountType)) {
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
				Program programQbo=Programs.GetCur(ProgramName.QuickBooksOnline);
				ProgramProperty programPropertyQboDepositAccounts=ProgramProperties.GetPropForProgByDesc(programQbo.ProgramNum,"Deposit Accounts");
				if(programPropertyQboDepositAccounts.PropertyValue=="") {
					return false;
				}
				ProgramProperty programPropertyQboIncomeAccounts=ProgramProperties.GetPropForProgByDesc(programQbo.ProgramNum,"Income Accounts");
				if(programPropertyQboIncomeAccounts.PropertyValue=="") {
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
			string[] stringArrayDep=depStr.Split(new char[] { ',' });
			List<string> listStrings=new List<string>();
			for(int i=0;i<stringArrayDep.Length;i++) {
				if(stringArrayDep[i]=="") {
					continue;
				}
				listStrings.Add(stringArrayDep[i]);
			}
			return listStrings;
		}

		///<summary></summary>
		public static List<string> GetIncomeAccountsQB() {
			//No need to check MiddleTierRole; no call to db.
			string incomeStr=PrefC.GetString(PrefName.QuickBooksIncomeAccount);
			string[] stringArrayIncome=incomeStr.Split(new char[] { ',' });
			List<string> listStrings=new List<string>();
			for(int i=0;i<stringArrayIncome.Length;i++) {
				if(stringArrayIncome[i]=="") {
					continue;
				}
				listStrings.Add(stringArrayIncome[i]);
			}
			return listStrings;
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
			decimal amtBalanceRE=PIn.Decimal(strBal);
			return amtBalanceRE;
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
			decimal amtBalanceRE=GetRE_PreviousYears(dateAsOf);
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
					row["balance"]=(credit-debit+amtBalanceRE).ToString("N");
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

		public static DataTable GetAccountTotalByType(DateTime dateStart,DateTime dateEnd,AccountType accountType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,accountType);
			}
			string strSumTotal
				="";
			if(accountType==AccountType.Expense) {
				strSumTotal="SUM(ROUND(DebitAmt,3)-ROUND(CreditAmt,3))";
			}
			else {//Income instead of expense
				strSumTotal="SUM(ROUND(CreditAmt,3)-ROUND(DebitAmt,3))";
			}
			string command="SELECT Description, "+strSumTotal+" SumTotal, AcctType "
				+"FROM account, journalentry "
				+"WHERE account.AccountNum=journalentry.AccountNum AND DateDisplayed >= "+POut.Date(dateStart)+" "
				+"AND DateDisplayed <= "+POut.Date(dateEnd)+" "
				+"AND AcctType="+POut.Int((int)accountType)+" "
				+"GROUP BY account.AccountNum "
				+"ORDER BY Description, DateDisplayed ";
			return Db.GetTable(command);
		}

		///<Summary>asOfDate is typically 12/31/...  </Summary>
		public static double NetIncomeThisYear(DateTime dateAsOf) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDouble(MethodBase.GetCurrentMethod(),dateAsOf);
			}
			DateTime dateFirstOfYear=new DateTime(dateAsOf.Year,1,1);
			string command="SELECT SUM(ROUND(CreditAmt,3)), SUM(ROUND(DebitAmt,3)), AcctType "
			+"FROM journalentry,account "
			+"WHERE journalentry.AccountNum=account.AccountNum "
			+"AND DateDisplayed >= "+POut.Date(dateFirstOfYear)
			+" AND DateDisplayed <= "+POut.Date(dateAsOf)
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




