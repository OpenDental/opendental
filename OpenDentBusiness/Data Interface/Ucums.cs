using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Ucums{
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class UcumCache : CacheListAbs<Ucum> {
			protected override List<Ucum> GetCacheFromDb() {
				string command="SELECT * FROM Ucum ORDER BY ItemOrder";
				return Crud.UcumCrud.SelectMany(command);
			}
			protected override List<Ucum> TableToList(DataTable table) {
				return Crud.UcumCrud.TableToList(table);
			}
			protected override Ucum Copy(Ucum Ucum) {
				return Ucum.Clone();
			}
			protected override DataTable ListToTable(List<Ucum> listUcums) {
				return Crud.UcumCrud.ListToTable(listUcums,"Ucum");
			}
			protected override void FillCacheIfNeeded() {
				Ucums.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Ucum Ucum) {
				return !Ucum.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static UcumCache _UcumCache=new UcumCache();

		///<summary>A list of all Ucums. Returns a deep copy.</summary>
		public static List<Ucum> ListDeep {
			get {
				return _UcumCache.ListDeep;
			}
		}

		///<summary>A list of all visible Ucums. Returns a deep copy.</summary>
		public static List<Ucum> ListShortDeep {
			get {
				return _UcumCache.ListShortDeep;
			}
		}

		///<summary>A list of all Ucums. Returns a shallow copy.</summary>
		public static List<Ucum> ListShallow {
			get {
				return _UcumCache.ListShallow;
			}
		}

		///<summary>A list of all visible Ucums. Returns a shallow copy.</summary>
		public static List<Ucum> ListShort {
			get {
				return _UcumCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_UcumCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_UcumCache.FillCacheFromTable(table);
				return table;
			}
			return _UcumCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary></summary>
		public static long Insert(Ucum ucum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				ucum.UcumNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ucum);
				return ucum.UcumNum;
			}
			return Crud.UcumCrud.Insert(ucum);
		}

		///<summary></summary>
		public static void Update(Ucum ucum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ucum);
				return;
			}
			Crud.UcumCrud.Update(ucum);
		}

		public static List<Ucum> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Ucum>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM ucum ORDER BY UcumCode";
			return Crud.UcumCrud.SelectMany(command);
		}

		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM ucum";
			return PIn.Long(Db.GetCount(command));
		}

		///<summary>Returns a list of just the codes for use in update or insert logic.</summary>
		public static List<string> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT UcumCode FROM ucum";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				retVal.Add(table.Rows[i].ItemArray[0].ToString());
			}
			return retVal;
		}

		public static Ucum GetByCode(string ucumCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Ucum>(MethodBase.GetCurrentMethod(),ucumCode);
			}
			string command;
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				command="SELECT * FROM ucum WHERE UcumCode='"+POut.String(ucumCode)+"'";
			}
			else {
				//because when we search for UnumCode 'a' for 'year [time]' used for age we sometimes get 'A' for 'Ampere [electric current]'
				//since MySQL is case insensitive, so we compare the binary values of 'a' and 'A' which are 0x61 and 0x41 in Hex respectively.
				command="SELECT * FROM ucum WHERE CAST(UcumCode AS BINARY)=CAST('"+POut.String(ucumCode)+"' AS BINARY)";
			}
			return Crud.UcumCrud.SelectOne(command);
		}

		public static List<Ucum> GetBySearchText(string searchText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Ucum>>(MethodBase.GetCurrentMethod(),searchText);
			}
			string[] searchTokens=searchText.Split(' ');
			string command=@"SELECT * FROM ucum ";
			for(int i=0;i<searchTokens.Length;i++) {
				command+=(i==0?"WHERE ":"AND ")+"(UcumCode LIKE '%"+POut.String(searchTokens[i])+"%' OR Description LIKE '%"+POut.String(searchTokens[i])+"%') ";
			}
			return Crud.UcumCrud.SelectMany(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<UCUM> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UCUM>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ucum WHERE PatNum = "+POut.Long(patNum);
			return Crud.UCUMCrud.SelectMany(command);
		}

		///<summary>Gets one UCUM from the db.</summary>
		public static UCUM GetOne(long uCUMNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<UCUM>(MethodBase.GetCurrentMethod(),uCUMNum);
			}
			return Crud.UCUMCrud.SelectOne(uCUMNum);
		}

		///<summary></summary>
		public static void Delete(long uCUMNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),uCUMNum);
				return;
			}
			string command= "DELETE FROM ucum WHERE UCUMNum = "+POut.Long(uCUMNum);
			Db.NonQ(command);
		}
		*/
	}
}