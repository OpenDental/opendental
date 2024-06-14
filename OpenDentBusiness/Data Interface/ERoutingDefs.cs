using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ERoutingDefs{
		#region Cache Pattern
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also,consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ERoutingDefCache:CacheListAbs<ERoutingDef> {
			protected override List<ERoutingDef> GetCacheFromDb() {
				string command = "SELECT * FROM eroutingdef";
				return Crud.ERoutingDefCrud.SelectMany(command);
			}
			protected override List<ERoutingDef> TableToList(DataTable table) {
				return Crud.ERoutingDefCrud.TableToList(table);
			}
			protected override ERoutingDef Copy(ERoutingDef patientERoutingDef) {
				return patientERoutingDef.Copy();
			}

			protected override DataTable ListToTable(List<ERoutingDef> listERoutingDefs) {
				return Crud.ERoutingDefCrud.ListToTable(listERoutingDefs, "ERoutingDef");
			}
			protected override void FillCacheIfNeeded() {
				ERoutingDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ERoutingDefCache _eRoutingDefCache = new ERoutingDefCache();

		public static List<ERoutingDef> GetDeepCopy(bool isShort = false) {
			return _eRoutingDefCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort = false) {
			return _eRoutingDefCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ERoutingDef> match,bool isShort = false) {
			return _eRoutingDefCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ERoutingDef> match,bool isShort = false) {
			return _eRoutingDefCache.GetFindIndex(match,isShort);
		}

		public static ERoutingDef GetFirst(bool isShort = false) {
			return _eRoutingDefCache.GetFirst(isShort);
		}

		public static ERoutingDef GetFirst(Func<ERoutingDef,bool> match,bool isShort = false) {
			return _eRoutingDefCache.GetFirst(match,isShort);
		}

		public static ERoutingDef GetFirstOrDefault(Func<ERoutingDef,bool> match,bool isShort = false) {
			return _eRoutingDefCache.GetFirstOrDefault(match,isShort);
		}

		public static ERoutingDef GetLast(bool isShort = false) {
			return _eRoutingDefCache.GetLast(isShort);
		}

		public static ERoutingDef GetLastOrDefault(Func<ERoutingDef,bool> match,bool isShort = false) {
			return _eRoutingDefCache.GetLastOrDefault(match,isShort);
		}

		public static List<ERoutingDef> GetWhere(Predicate<ERoutingDef> match,bool isShort = false) {
			return _eRoutingDefCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_eRoutingDefCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable.Always refreshes the ClientMT's cache.</summary>
		///<param name = "doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table = Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_eRoutingDefCache.FillCacheFromTable(table);
				return table;
			}
			return _eRoutingDefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_eRoutingDefCache.ClearCache();
		}
		#endregion Cache Pattern

		
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<ERoutingDef> GetByClinic(long clinicNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingDef>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command= "SELECT * FROM eroutingdef WHERE clinicNum = " + POut.Long(clinicNum);
			return Crud.ERoutingDefCrud.SelectMany(command);
		}
		
		///<summary>Gets one ERoutingDef from the db.</summary>
		public static ERoutingDef GetOne(long eRoutingDefNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ERoutingDef>(MethodBase.GetCurrentMethod(),eRoutingDefNum);
			}
			return Crud.ERoutingDefCrud.SelectOne(eRoutingDefNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ERoutingDef eRoutingDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eRoutingDef.ERoutingDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eRoutingDef);
				return eRoutingDef.ERoutingDefNum;
			}
			return Crud.ERoutingDefCrud.Insert(eRoutingDef);
		}
		///<summary></summary>
		public static void Update(ERoutingDef eRoutingDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eRoutingDef);
				return;
			}
			Crud.ERoutingDefCrud.Update(eRoutingDef);
		}
		///<summary></summary>
		public static void Delete(long eRoutingDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eRoutingDefNum);
				return;
			}
			Crud.ERoutingDefCrud.Delete(eRoutingDefNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}