using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Loincs{
		/*
		#region CachePattern

		private class LoincCache : CacheListAbs<Loinc> {
			protected override List<Loinc> GetCacheFromDb() {
				string command="SELECT * FROM Loinc ORDER BY ItemOrder";
				return Crud.LoincCrud.SelectMany(command);
			}
			protected override List<Loinc> TableToList(DataTable table) {
				return Crud.LoincCrud.TableToList(table);
			}
			protected override Loinc Copy(Loinc Loinc) {
				return Loinc.Clone();
			}
			protected override DataTable ListToTable(List<Loinc> listLoincs) {
				return Crud.LoincCrud.ListToTable(listLoincs,"Loinc");
			}
			protected override void FillCacheIfNeeded() {
				Loincs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Loinc Loinc) {
				return !Loinc.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static LoincCache _LoincCache=new LoincCache();

		///<summary>A list of all Loincs. Returns a deep copy.</summary>
		public static List<Loinc> ListDeep {
			get {
				return _LoincCache.ListDeep;
			}
		}

		///<summary>A list of all visible Loincs. Returns a deep copy.</summary>
		public static List<Loinc> ListShortDeep {
			get {
				return _LoincCache.ListShortDeep;
			}
		}

		///<summary>A list of all Loincs. Returns a shallow copy.</summary>
		public static List<Loinc> ListShallow {
			get {
				return _LoincCache.ListShallow;
			}
		}

		///<summary>A list of all visible Loincs. Returns a shallow copy.</summary>
		public static List<Loinc> ListShort {
			get {
				return _LoincCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_LoincCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_LoincCache.FillCacheFromTable(table);
				return table;
			}
			return _LoincCache.GetTableFromCache(doRefreshCache);
		}

		#endregion*/

		///<summary></summary>
		public static long Insert(Loinc lOINC){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				lOINC.LoincNum=Meth.GetLong(MethodBase.GetCurrentMethod(),lOINC);
				return lOINC.LoincNum;
			}
			return Crud.LoincCrud.Insert(lOINC);
		}

		///<summary></summary>
		public static void Update(Loinc loinc) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),loinc);
				return;
			}
			Crud.LoincCrud.Update(loinc);
		}

		///<summary></summary>
		public static List<Loinc> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Loinc>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM loinc";
			return Crud.LoincCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<Loinc> GetBySearchString(string searchText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Loinc>>(MethodBase.GetCurrentMethod(),searchText);
			} 
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="SELECT * FROM loinc WHERE LoincCode LIKE '%"+POut.String(searchText)+"%' OR NameLongCommon LIKE '%"+POut.String(searchText)
					+"%' ORDER BY RankCommonTests=0, RankCommonTests";//common tests are at top of list.
			}
			else {//oracle
				command="SELECT * FROM loinc WHERE LoincCode LIKE '%"+POut.String(searchText)+"%' OR NameLongCommon LIKE '%"+POut.String(searchText)+"%'";
			}
			return Crud.LoincCrud.SelectMany(command);
		}

		///<summary>Returns the count of all LOINC codes.  LOINC codes cannot be hidden.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM loinc";
			return PIn.Long(Db.GetCount(command));
		}

		///<summary>Gets one Loinc from the db based on LoincCode, returns null if not found.</summary>
		public static Loinc GetByCode(string loincCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Loinc>(MethodBase.GetCurrentMethod(),loincCode);
			}
			string command="SELECT * FROM loinc WHERE LoincCode='"+POut.String(loincCode)+"'";
			List<Loinc> retVal=Crud.LoincCrud.SelectMany(command);
			if(retVal.Count>0) {
				return retVal[0];
			}
			return null;
		}

		///<summary>Gets a list of Loinc objects from the db based on codeList.  codeList is a comma-delimited list of LoincCodes in the format "code,code,code,code".  Returns an empty list if none in the loinc table.</summary>
		public static List<Loinc> GetForCodeList(string codeList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Loinc>>(MethodBase.GetCurrentMethod(),codeList);
			}
			string[] codes=codeList.Split(',');
			string command="SELECT * FROM loinc WHERE LoincCode IN(";
			for(int i=0;i<codes.Length;i++) {
				if(i>0) {
					command+=",";
				}
				command+="'"+POut.String(codes[i])+"'";
			}
			command+=") ";
			return Crud.LoincCrud.SelectMany(command);
		}

		///<summary>CAUTION, this empties the entire loinc table. "DELETE FROM loinc"</summary>
		public static void DeleteAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="DELETE FROM loinc";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE loinc AUTO_INCREMENT = 1";//resets the primary key to start counting from 1 again.
				Db.NonQ(command);
			}
			return;
		}

		///<summary>Returns a list of just the codes for use in update or insert logic.</summary>
		public static List<string> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT LoincCode FROM loinc";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				retVal.Add(table.Rows[i].ItemArray[0].ToString());
			}
			return retVal;
		}

		///<summary>Directly from db.</summary>
		public static bool CodeExists(string LoincCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),LoincCode);
			}
			string command="SELECT COUNT(*) FROM loinc WHERE LoincCode='"+POut.String(LoincCode)+"'";
			string count=Db.GetCount(command);
			if(count=="0") {
				return false;
			}
			return true;
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Loinc> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Loinc>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM loinc WHERE PatNum = "+POut.Long(patNum);
			return Crud.LoincCrud.SelectMany(command);
		}

		///<summary>Gets one Loinc from the db.</summary>
		public static Loinc GetOne(long lOINCNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Loinc>(MethodBase.GetCurrentMethod(),lOINCNum);
			}
			return Crud.LoincCrud.SelectOne(lOINCNum);
		}

		///<summary></summary>
		public static void Delete(long lOINCNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),lOINCNum);
				return;
			}
			string command= "DELETE FROM loinc WHERE LoincNum = "+POut.Long(lOINCNum);
			Db.NonQ(command);
		}
		*/
	}
}