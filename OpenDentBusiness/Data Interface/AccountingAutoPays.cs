using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AccountingAutoPays {
		#region Cache Pattern

		private class AccountingAutoPayCache:CacheListAbs<AccountingAutoPay> {
			protected override AccountingAutoPay Copy(AccountingAutoPay accountingAutoPay) {
				return accountingAutoPay.Clone();
			}

			protected override void FillCacheIfNeeded() {
				AccountingAutoPays.GetTableFromCache(false);
			}

			protected override List<AccountingAutoPay> GetCacheFromDb() {
				string command="SELECT * FROM accountingautopay";
				return Crud.AccountingAutoPayCrud.SelectMany(command);
			}

			protected override DataTable ListToTable(List<AccountingAutoPay> listAccountingAutoPays) {
				return Crud.AccountingAutoPayCrud.ListToTable(listAccountingAutoPays,"AccountingAutoPay");
			}

			protected override List<AccountingAutoPay> TableToList(DataTable table) {
				return Crud.AccountingAutoPayCrud.TableToList(table);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AccountingAutoPayCache _accountingAutoPayCache=new AccountingAutoPayCache();

		public static List<AccountingAutoPay> GetDeepCopy(bool isShort=false) {
			return _accountingAutoPayCache.GetDeepCopy(isShort);
		}

		public static AccountingAutoPay GetFirstOrDefault(Func<AccountingAutoPay,bool> match,bool isShort=false) {
			return _accountingAutoPayCache.GetFirstOrDefault(match,isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _accountingAutoPayCache.GetCount(isShort);
		}

		///<summary>Gets a list of all AccountingAutoPays.</summary>
		public static DataTable RefreshCache(){
			return GetTableFromCache(true);
		}

		public static void FillCacheFromTable(DataTable table) {
			_accountingAutoPayCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_accountingAutoPayCache.FillCacheFromTable(table);
				return table;
			}
			return _accountingAutoPayCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern
		
		///<summary></summary>
		public static long Insert(AccountingAutoPay pay) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				pay.AccountingAutoPayNum=Meth.GetLong(MethodBase.GetCurrentMethod(),pay);
				return pay.AccountingAutoPayNum;
			}
			return Crud.AccountingAutoPayCrud.Insert(pay);
		}

		///<summary>Converts the comma delimited list of AccountNums into full descriptions separated by carriage returns.</summary>
		public static string GetPickListDesc(AccountingAutoPay pay){
			//No need to check RemotingRole; no call to db.
			string[] numArray=pay.PickList.Split(new char[] { ',' });
			string retVal="";
			for(int i=0;i<numArray.Length;i++) {
				if(numArray[i]=="") {
					continue;
				}
				if(retVal!=""){
					retVal+="\r\n";
				}
				retVal+=Accounts.GetDescript(PIn.Long(numArray[i]));
			}
			return retVal;
		}

		///<summary>Converts the comma delimited list of AccountNums into an array of AccountNums.</summary>
		public static long[] GetPickListAccounts(AccountingAutoPay pay) {
			//No need to check RemotingRole; no call to db.
			string[] numArray=pay.PickList.Split(new char[] { ',' });
			ArrayList AL=new ArrayList();
			for(int i=0;i<numArray.Length;i++) {
				if(numArray[i]=="") {
					continue;
				}
				AL.Add(PIn.Long(numArray[i]));
			}
			long[] retVal=new long[AL.Count];
			AL.CopyTo(retVal);
			return retVal;
		}

		///<summary>Loops through the AList to find one with the specified payType (defNum).  If none is found, then it returns null.</summary>
		public static AccountingAutoPay GetForPayType(long payType) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.PayType==payType);
		}

		///<summary>Saves the list of accountingAutoPays to the database.  Deletes all existing ones first.</summary>
		public static void SaveList(List<AccountingAutoPay> list) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),list);
				return;
			}
			string command="DELETE FROM accountingautopay";
			Db.NonQ(command);
			for(int i=0;i<list.Count;i++){
				Insert(list[i]);
			}
		}
	}

}