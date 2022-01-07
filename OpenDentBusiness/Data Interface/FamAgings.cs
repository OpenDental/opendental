using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary>This class will likely never be used.  The famaging table is used to store intermediate calculations for aging and once the patient
	///table is updated the data is never accessed again.  A new aging calculation begins with truncating this table.  All edit commands, i.e. truncate,
	///insert, etc., take place in the queries in Ledgers.ComputeAging.</summary>
	public class FamAgings {		
		#region Insert

		///<summary>Inserts many into the FamAging table.  Uses the existing pri key since these are basically copies of the patient table data and should
		///always be inserted using the patient.PatNum as the pri key value in order to join on the patient table for update later.</summary>
		public static void InsertMany(List<FamAging> listFamAgings) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listFamAgings);
				return;
			}
			Crud.FamAgingCrud.InsertMany(listFamAgings,true);//true to use existing pri key so these will match the patient.PatNums for joining later
		}

		#endregion

		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class FamAgingCache : CacheListAbs<FamAging> {
			protected override List<FamAging> GetCacheFromDb() {
				string command="SELECT * FROM FamAging ORDER BY ItemOrder";
				return Crud.FamAgingCrud.SelectMany(command);
			}
			protected override List<FamAging> TableToList(DataTable table) {
				return Crud.FamAgingCrud.TableToList(table);
			}
			protected override FamAging Copy(FamAging FamAging) {
				return FamAging.Clone();
			}
			protected override DataTable ListToTable(List<FamAging> listFamAgings) {
				return Crud.FamAgingCrud.ListToTable(listFamAgings,"FamAging");
			}
			protected override void FillCacheIfNeeded() {
				FamAgings.GetTableFromCache(false);
			}
			protected override bool IsInListShort(FamAging FamAging) {
				return !FamAging.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static FamAgingCache _FamAgingCache=new FamAgingCache();

		///<summary>A list of all FamAgings. Returns a deep copy.</summary>
		public static List<FamAging> ListDeep {
			get {
				return _FamAgingCache.ListDeep;
			}
		}

		///<summary>A list of all visible FamAgings. Returns a deep copy.</summary>
		public static List<FamAging> ListShortDeep {
			get {
				return _FamAgingCache.ListShortDeep;
			}
		}

		///<summary>A list of all FamAgings. Returns a shallow copy.</summary>
		public static List<FamAging> ListShallow {
			get {
				return _FamAgingCache.ListShallow;
			}
		}

		///<summary>A list of all visible FamAgings. Returns a shallow copy.</summary>
		public static List<FamAging> ListShort {
			get {
				return _FamAgingCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_FamAgingCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_FamAgingCache.FillCacheFromTable(table);
				return table;
			}
			return _FamAgingCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<FamAging> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<FamAging>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM famaging WHERE PatNum = "+POut.Long(patNum);
			return Crud.FamAgingCrud.SelectMany(command);
		}

		///<summary>Gets one FamAging from the db.</summary>
		public static FamAging GetOne(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<FamAging>(MethodBase.GetCurrentMethod(),patNum);
			}
			return Crud.FamAgingCrud.SelectOne(patNum);
		}

		///<summary></summary>
		public static long Insert(FamAging famAging){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				famAging.PatNum=Meth.GetLong(MethodBase.GetCurrentMethod(),famAging);
				return famAging.PatNum;
			}
			return Crud.FamAgingCrud.Insert(famAging);
		}

		///<summary></summary>
		public static void Update(FamAging famAging){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),famAging);
				return;
			}
			Crud.FamAgingCrud.Update(famAging);
		}

		///<summary></summary>
		public static void Delete(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			Crud.FamAgingCrud.Delete(patNum);
		}

		

		
		*/
	}
}