using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Cvxs{
		//If this table type will exist as cached data, uncomment the CachePattern region below.
		/*
		#region CachePattern

		private class CvxCache : CacheListAbs<Cvx> {
			protected override List<Cvx> GetCacheFromDb() {
				string command="SELECT * FROM Cvx ORDER BY ItemOrder";
				return Crud.CvxCrud.SelectMany(command);
			}
			protected override List<Cvx> TableToList(DataTable table) {
				return Crud.CvxCrud.TableToList(table);
			}
			protected override Cvx Copy(Cvx Cvx) {
				return Cvx.Clone();
			}
			protected override DataTable ListToTable(List<Cvx> listCvxs) {
				return Crud.CvxCrud.ListToTable(listCvxs,"Cvx");
			}
			protected override void FillCacheIfNeeded() {
				Cvxs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Cvx Cvx) {
				return !Cvx.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static CvxCache _CvxCache=new CvxCache();

		///<summary>A list of all Cvxs. Returns a deep copy.</summary>
		public static List<Cvx> ListDeep {
			get {
				return _CvxCache.ListDeep;
			}
		}

		///<summary>A list of all visible Cvxs. Returns a deep copy.</summary>
		public static List<Cvx> ListShortDeep {
			get {
				return _CvxCache.ListShortDeep;
			}
		}

		///<summary>A list of all Cvxs. Returns a shallow copy.</summary>
		public static List<Cvx> ListShallow {
			get {
				return _CvxCache.ListShallow;
			}
		}

		///<summary>A list of all visible Cvxs. Returns a shallow copy.</summary>
		public static List<Cvx> ListShort {
			get {
				return _CvxCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_CvxCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_CvxCache.FillCacheFromTable(table);
				return table;
			}
			return _CvxCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary></summary>
		public static long Insert(Cvx cvx){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				cvx.CvxNum=Meth.GetLong(MethodBase.GetCurrentMethod(),cvx);
				return cvx.CvxNum;
			}
			return Crud.CvxCrud.Insert(cvx);
		}

		///<summary></summary>
		public static void Update(Cvx cvx) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cvx);
				return;
			}
			Crud.CvxCrud.Update(cvx);
		}

		public static List<Cvx> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cvx>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM cvx";			
			return Crud.CvxCrud.SelectMany(command);
		}

		///<summary>Returns a list of just the codes for use in update or insert logic.</summary>
		public static List<string> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT CvxCode FROM cvx";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++){
				retVal.Add(table.Rows[i][0].ToString());
			}
			return retVal;
		}

		///<summary>Gets one Cvx object directly from the database by CodeValue.  If code does not exist, returns null.</summary>
		public static Cvx GetByCode(string cvxCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Cvx>(MethodBase.GetCurrentMethod(),cvxCode);
			}
			string command="SELECT * FROM Cvx WHERE CvxCode='"+POut.String(cvxCode)+"'";
			return Crud.CvxCrud.SelectOne(command);
		}

		///<summary>Gets one Cvx by CvxNum directly from the db.</summary>
		public static Cvx GetOneFromDb(string cvxCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Cvx>(MethodBase.GetCurrentMethod(),cvxCode);
			}
			string command="SELECT * FROM cvx WHERE CvxCode='"+POut.String(cvxCode)+"'";
			return Crud.CvxCrud.SelectOne(command);
		}

		///<summary>Directly from db.</summary>
		public static bool CodeExists(string cvxCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),cvxCode);
			}
			string command="SELECT COUNT(*) FROM cvx WHERE CvxCode='"+POut.String(cvxCode)+"'";
			string count=Db.GetCount(command);
			if(count=="0") {
				return false;
			}
			return true;
		}

		///<summary>Returns the total count of CVX codes.  CVS codes cannot be hidden, but might in the future be set active/inactive using the IsActive flag.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM cvx";
			return PIn.Long(Db.GetCount(command));
		}

		public static List<Cvx> GetBySearchText(string searchText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cvx>>(MethodBase.GetCurrentMethod(),searchText);
			}
			string[] searchTokens=searchText.Split(' ');
			string command=@"SELECT * FROM cvx ";
			for(int i=0;i<searchTokens.Length;i++) {
				command+=(i==0?"WHERE ":"AND ")+"(CvxCode LIKE '%"+POut.String(searchTokens[i])+"%' OR Description LIKE '%"+POut.String(searchTokens[i])+"%') ";
			}
			return Crud.CvxCrud.SelectMany(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Cvx> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cvx>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM cvx WHERE PatNum = "+POut.Long(patNum);
			return Crud.CvxCrud.SelectMany(command);
		}

		///<summary>Gets one Cvx from the db.</summary>
		public static Cvx GetOne(long cvxNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Cvx>(MethodBase.GetCurrentMethod(),cvxNum);
			}
			return Crud.CvxCrud.SelectOne(cvxNum);
		}

		///<summary></summary>
		public static void Delete(long cvxNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cvxNum);
				return;
			}
			string command= "DELETE FROM cvx WHERE CvxNum = "+POut.Long(cvxNum);
			Db.NonQ(command);
		}
		*/
	}
}