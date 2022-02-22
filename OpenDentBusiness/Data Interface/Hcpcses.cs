using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Hcpcses{
		//If this table type will exist as cached data, uncomment the CachePattern region below.
		/*
		#region CachePattern

		private class HcpcsCache : CacheListAbs<Hcpcs> {
			protected override List<Hcpcs> GetCacheFromDb() {
				string command="SELECT * FROM Hcpcs ORDER BY ItemOrder";
				return Crud.HcpcsCrud.SelectMany(command);
			}
			protected override List<Hcpcs> TableToList(DataTable table) {
				return Crud.HcpcsCrud.TableToList(table);
			}
			protected override Hcpcs Copy(Hcpcs Hcpcs) {
				return Hcpcs.Clone();
			}
			protected override DataTable ListToTable(List<Hcpcs> listHcpcss) {
				return Crud.HcpcsCrud.ListToTable(listHcpcss,"Hcpcs");
			}
			protected override void FillCacheIfNeeded() {
				Hcpcss.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Hcpcs Hcpcs) {
				return !Hcpcs.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static HcpcsCache _HcpcsCache=new HcpcsCache();

		///<summary>A list of all Hcpcss. Returns a deep copy.</summary>
		public static List<Hcpcs> ListDeep {
			get {
				return _HcpcsCache.ListDeep;
			}
		}

		///<summary>A list of all visible Hcpcss. Returns a deep copy.</summary>
		public static List<Hcpcs> ListShortDeep {
			get {
				return _HcpcsCache.ListShortDeep;
			}
		}

		///<summary>A list of all Hcpcss. Returns a shallow copy.</summary>
		public static List<Hcpcs> ListShallow {
			get {
				return _HcpcsCache.ListShallow;
			}
		}

		///<summary>A list of all visible Hcpcss. Returns a shallow copy.</summary>
		public static List<Hcpcs> ListShort {
			get {
				return _HcpcsCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_HcpcsCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_HcpcsCache.FillCacheFromTable(table);
				return table;
			}
			return _HcpcsCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary></summary>
		public static long Insert(Hcpcs hcpcs){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				hcpcs.HcpcsNum=Meth.GetLong(MethodBase.GetCurrentMethod(),hcpcs);
				return hcpcs.HcpcsNum;
			}
			return Crud.HcpcsCrud.Insert(hcpcs);
		}

		///<summary></summary>
		public static void Update(Hcpcs hcpcs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hcpcs);
				return;
			}
			Crud.HcpcsCrud.Update(hcpcs);
		}

		public static List<Hcpcs> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Hcpcs>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM hcpcs";
			return Crud.HcpcsCrud.SelectMany(command);
		}

		///<summary>Returns a list of just the codes for use in update or insert logic.</summary>
		public static List<string> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT HcpcsCode FROM hcpcs";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++){
				retVal.Add(table.Rows[i][0].ToString());
			}
			return retVal;
		}

		///<summary>Returns the total count of HCPCS codes.  HCPCS codes cannot be hidden.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM hcpcs";
			return PIn.Long(Db.GetCount(command));
		}

		///<summary>Returns the Hcpcs of the code passed in by looking in cache.  If code does not exist, returns null.</summary>
		public static Hcpcs GetByCode(string hcpcsCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Hcpcs>(MethodBase.GetCurrentMethod(),hcpcsCode);
			}
			string command="SELECT * FROM hcpcs WHERE HcpcsCode='"+POut.String(hcpcsCode)+"'";
			return Crud.HcpcsCrud.SelectOne(command);
		}

		///<summary>Directly from db.</summary>
		public static bool CodeExists(string hcpcsCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),hcpcsCode);
			}
			string command="SELECT COUNT(*) FROM hcpcs WHERE HcpcsCode='"+POut.String(hcpcsCode)+"'";
			string count=Db.GetCount(command);
			if(count=="0") {
				return false;
			}
			return true;
		}

		public static List<Hcpcs> GetBySearchText(string searchText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Hcpcs>>(MethodBase.GetCurrentMethod(),searchText);
			}
			string[] searchTokens=searchText.Split(' ');
			string command=@"SELECT * FROM hcpcs ";
			for(int i=0;i<searchTokens.Length;i++) {
				command+=(i==0?"WHERE ":"AND ")+"(HcpcsCode LIKE '%"+POut.String(searchTokens[i])+"%' OR DescriptionShort LIKE '%"+POut.String(searchTokens[i])+"%') ";
			}
			return Crud.HcpcsCrud.SelectMany(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Hcpcs> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Hcpcs>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM hcpcs WHERE PatNum = "+POut.Long(patNum);
			return Crud.HcpcsCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long hcpcsNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hcpcsNum);
				return;
			}
			string command= "DELETE FROM hcpcs WHERE HcpcsNum = "+POut.Long(hcpcsNum);
			Db.NonQ(command);
		}
		*/
	}
}