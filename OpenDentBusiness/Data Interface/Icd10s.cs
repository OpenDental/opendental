using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Icd10s{
		//If this table type will exist as cached data, uncomment the CachePattern region below.
		/*
		#region CachePattern

		private class Icd10Cache : CacheListAbs<Icd10> {
			protected override List<Icd10> GetCacheFromDb() {
				string command="SELECT * FROM Icd10 ORDER BY ItemOrder";
				return Crud.Icd10Crud.SelectMany(command);
			}
			protected override List<Icd10> TableToList(DataTable table) {
				return Crud.Icd10Crud.TableToList(table);
			}
			protected override Icd10 Copy(Icd10 Icd10) {
				return Icd10.Clone();
			}
			protected override DataTable ListToTable(List<Icd10> listIcd10s) {
				return Crud.Icd10Crud.ListToTable(listIcd10s,"Icd10");
			}
			protected override void FillCacheIfNeeded() {
				Icd10s.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Icd10 Icd10) {
				return !Icd10.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static Icd10Cache _Icd10Cache=new Icd10Cache();

		///<summary>A list of all Icd10s. Returns a deep copy.</summary>
		public static List<Icd10> ListDeep {
			get {
				return _Icd10Cache.ListDeep;
			}
		}

		///<summary>A list of all visible Icd10s. Returns a deep copy.</summary>
		public static List<Icd10> ListShortDeep {
			get {
				return _Icd10Cache.ListShortDeep;
			}
		}

		///<summary>A list of all Icd10s. Returns a shallow copy.</summary>
		public static List<Icd10> ListShallow {
			get {
				return _Icd10Cache.ListShallow;
			}
		}

		///<summary>A list of all visible Icd10s. Returns a shallow copy.</summary>
		public static List<Icd10> ListShort {
			get {
				return _Icd10Cache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_Icd10Cache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_Icd10Cache.FillCacheFromTable(table);
				return table;
			}
			return _Icd10Cache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary></summary>
		public static long Insert(Icd10 icd10){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				icd10.Icd10Num=Meth.GetLong(MethodBase.GetCurrentMethod(),icd10);
				return icd10.Icd10Num;
			}
			return Crud.Icd10Crud.Insert(icd10);
		}

		///<summary></summary>
		public static void Update(Icd10 icd10) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),icd10);
				return;
			}
			Crud.Icd10Crud.Update(icd10);
		}

		public static List<Icd10> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Icd10>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM icd10";
			return Crud.Icd10Crud.SelectMany(command);
		}

		///<summary>Returns a list of just the codes for use in update or insert logic.</summary>
		public static List<string> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT Icd10Code FROM icd10";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++){
				retVal.Add(table.Rows[i][0].ToString());
			}
			return retVal;
		}

		///<summary>Returns the total number of ICD10 codes.  Some rows in the ICD10 table based on the IsCode column.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM icd10 WHERE IsCode!=0";
			return PIn.Long(Db.GetCount(command));
		}
		
		///<summary>Gets one ICD10 object directly from the database by CodeValue.  If code does not exist, returns null.</summary>
		public static Icd10 GetByCode(string Icd10Code) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Icd10>(MethodBase.GetCurrentMethod(),Icd10Code);
			}
			string command="SELECT * FROM icd10 WHERE Icd10Code='"+POut.String(Icd10Code)+"'";
			return Crud.Icd10Crud.SelectOne(command);
		}

		///<summary>Gets all ICD10 objects directly from the database by CodeValues.  If codes don't exist, it will return an empty list.</summary>
		public static List<Icd10> GetByCodes(List<string> listIcd10Codes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Icd10>>(MethodBase.GetCurrentMethod(),listIcd10Codes);
			}
			if(listIcd10Codes==null || listIcd10Codes.Count==0) {
				return new List<Icd10>();
			}
			string command="SELECT * FROM icd10 WHERE Icd10Code IN('"+string.Join("','",listIcd10Codes)+"')";
			return Crud.Icd10Crud.SelectMany(command);
		}

		///<summary>Directly from db.</summary>
		public static bool CodeExists(string Icd10Code) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),Icd10Code);
			}
			string command="SELECT COUNT(*) FROM icd10 WHERE Icd10Code='"+POut.String(Icd10Code)+"'";
			string count=Db.GetCount(command);
			if(count=="0") {
				return false;
			}
			return true;
		}

		public static List<Icd10> GetBySearchText(string searchText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Icd10>>(MethodBase.GetCurrentMethod(),searchText);
			}
			string[] searchTokens=searchText.Split(' ');
			string command=@"SELECT * FROM icd10 ";
			for(int i=0;i<searchTokens.Length;i++) {
				command+=(i==0?"WHERE ":"AND ")+"(Icd10Code LIKE '%"+POut.String(searchTokens[i])+"%' OR Description LIKE '%"+POut.String(searchTokens[i])+"%') ";
			}
			return Crud.Icd10Crud.SelectMany(command);
		}

		///<summary>Gets one Icd10 from the db.</summary>
		public static Icd10 GetOne(long icd10Num){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Icd10>(MethodBase.GetCurrentMethod(),icd10Num);
			}
			return Crud.Icd10Crud.SelectOne(icd10Num);
		}

		///<summary>Returns the code and description of the icd10.</summary>
		public static string GetCodeAndDescription(string icd10Code) {
			if(string.IsNullOrEmpty(icd10Code)) {
				return "";
			}
			//No need to check RemotingRole; no call to db.
			Icd10 icd10=GetByCode(icd10Code);
			return (icd10==null ? "" : (icd10.Icd10Code+"-"+icd10.Description));
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Icd10> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Icd10>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM icd10 WHERE PatNum = "+POut.Long(patNum);
			return Crud.Icd10Crud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long icd10Num) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),icd10Num);
				return;
			}
			string command= "DELETE FROM icd10 WHERE Icd10Num = "+POut.Long(icd10Num);
			Db.NonQ(command);
		}
		*/
	}
}