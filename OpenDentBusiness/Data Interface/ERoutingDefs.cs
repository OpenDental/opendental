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
				return Crud.ERoutingDefCrud.ListToTable(listERoutingDefs,"ERoutingDef");
			}
			protected override void FillCacheIfNeeded() {
				ERoutingDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ERoutingDefCache _ERoutingDefCache = new ERoutingDefCache();

		public static List<ERoutingDef> GetDeepCopy(bool isShort = false) {
			return _ERoutingDefCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort = false) {
			return _ERoutingDefCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ERoutingDef> match,bool isShort = false) {
			return _ERoutingDefCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ERoutingDef> match,bool isShort = false) {
			return _ERoutingDefCache.GetFindIndex(match,isShort);
		}

		public static ERoutingDef GetFirst(bool isShort = false) {
			return _ERoutingDefCache.GetFirst(isShort);
		}

		public static ERoutingDef GetFirst(Func<ERoutingDef,bool> match,bool isShort = false) {
			return _ERoutingDefCache.GetFirst(match,isShort);
		}

		public static ERoutingDef GetFirstOrDefault(Func<ERoutingDef,bool> match,bool isShort = false) {
			return _ERoutingDefCache.GetFirstOrDefault(match,isShort);
		}

		public static ERoutingDef GetLast(bool isShort = false) {
			return _ERoutingDefCache.GetLast(isShort);
		}

		public static ERoutingDef GetLastOrDefault(Func<ERoutingDef,bool> match,bool isShort = false) {
			return _ERoutingDefCache.GetLastOrDefault(match,isShort);
		}

		public static List<ERoutingDef> GetWhere(Predicate<ERoutingDef> match,bool isShort = false) {
			return _ERoutingDefCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_ERoutingDefCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable.Always refreshes the ClientMT's cache.</summary>
		///<param name = "doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table = Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_ERoutingDefCache.FillCacheFromTable(table);
				return table;
			}
			return _ERoutingDefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_ERoutingDefCache.ClearCache();
		}
		#endregion Cache Pattern

		
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<ERoutingDef> GetByClinic(long clinicNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingDef>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT * FROM eroutingdef WHERE clinicNum = "+POut.Long(clinicNum);
			return Crud.ERoutingDefCrud.SelectMany(command);
		}
		
		///<summary>Gets one PatientERoutingDef from the db.</summary>
		public static ERoutingDef GetOne(long patientERoutingDefNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ERoutingDef>(MethodBase.GetCurrentMethod(),patientERoutingDefNum);
			}
			return Crud.ERoutingDefCrud.SelectOne(patientERoutingDefNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ERoutingDef eroutingDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eroutingDef.ERoutingDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eroutingDef);
				return eroutingDef.ERoutingDefNum;
			}
			return Crud.ERoutingDefCrud.Insert(eroutingDef);
		}
		///<summary></summary>
		public static void Update(ERoutingDef eroutingDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingDef);
				return;
			}
			Crud.ERoutingDefCrud.Update(eroutingDef);
		}
		///<summary></summary>
		public static void Delete(long eroutingDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingDefNum);
				return;
			}
			Crud.ERoutingDefCrud.Delete(eroutingDefNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}