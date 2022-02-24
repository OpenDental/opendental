using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary>Cache pattern only used for updates.</summary>
	public class Cdcrecs{
		//If this table type will exist as cached data, uncomment the CachePattern region below.
		/*
		#region CachePattern

		private class CdcrecCache : CacheListAbs<Cdcrec> {
			protected override List<Cdcrec> GetCacheFromDb() {
				string command="SELECT * FROM Cdcrec ORDER BY ItemOrder";
				return Crud.CdcrecCrud.SelectMany(command);
			}
			protected override List<Cdcrec> TableToList(DataTable table) {
				return Crud.CdcrecCrud.TableToList(table);
			}
			protected override Cdcrec Copy(Cdcrec Cdcrec) {
				return Cdcrec.Clone();
			}
			protected override DataTable ListToTable(List<Cdcrec> listCdcrecs) {
				return Crud.CdcrecCrud.ListToTable(listCdcrecs,"Cdcrec");
			}
			protected override void FillCacheIfNeeded() {
				Cdcrecs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Cdcrec Cdcrec) {
				return !Cdcrec.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static CdcrecCache _CdcrecCache=new CdcrecCache();

		///<summary>A list of all Cdcrecs. Returns a deep copy.</summary>
		public static List<Cdcrec> ListDeep {
			get {
				return _CdcrecCache.ListDeep;
			}
		}

		///<summary>A list of all visible Cdcrecs. Returns a deep copy.</summary>
		public static List<Cdcrec> ListShortDeep {
			get {
				return _CdcrecCache.ListShortDeep;
			}
		}

		///<summary>A list of all Cdcrecs. Returns a shallow copy.</summary>
		public static List<Cdcrec> ListShallow {
			get {
				return _CdcrecCache.ListShallow;
			}
		}

		///<summary>A list of all visible Cdcrecs. Returns a shallow copy.</summary>
		public static List<Cdcrec> ListShort {
			get {
				return _CdcrecCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_CdcrecCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_CdcrecCache.FillCacheFromTable(table);
				return table;
			}
			return _CdcrecCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary></summary>
		public static long Insert(Cdcrec cdcrec){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				cdcrec.CdcrecNum=Meth.GetLong(MethodBase.GetCurrentMethod(),cdcrec);
				return cdcrec.CdcrecNum;
			}
			return Crud.CdcrecCrud.Insert(cdcrec);
		}

		///<summary></summary>
		public static void Update(Cdcrec cdcrec){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cdcrec);
				return;
			}
			Crud.CdcrecCrud.Update(cdcrec);
		}
		
		public static List<Cdcrec> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cdcrec>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM cdcrec";
			return Crud.CdcrecCrud.SelectMany(command);
		}

		///<summary>Returns a list of just the codes for use in update or insert logic.</summary>
		public static List<string> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT CdcRecCode FROM cdcrec";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++){
				retVal.Add(table.Rows[i].ItemArray[0].ToString());
			}
			return retVal;
		}		

		///<summary>Returns the total count of CDCREC codes.  CDCREC codes cannot be hidden.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM cdcrec";
			return PIn.Long(Db.GetCount(command));
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Cdcrec> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cdcrec>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM cdcrec WHERE PatNum = "+POut.Long(patNum);
			return Crud.CdcrecCrud.SelectMany(command);
		}

		///<summary>Gets one Cdcrec from the db.</summary>
		public static Cdcrec GetOne(long cdcrecNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Cdcrec>(MethodBase.GetCurrentMethod(),cdcrecNum);
			}
			return Crud.CdcrecCrud.SelectOne(cdcrecNum);
		}

		///<summary></summary>
		public static void Delete(long cdcrecNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cdcrecNum);
				return;
			}
			string command= "DELETE FROM cdcrec WHERE CdcrecNum = "+POut.Long(cdcrecNum);
			Db.NonQ(command);
		}
		*/
	}
}