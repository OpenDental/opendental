using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FlowDefs{
		#region Cache Pattern
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also,consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class FlowDefCache:CacheListAbs<FlowDef> {
			protected override List<FlowDef> GetCacheFromDb() {
				string command = "SELECT * FROM flowdef";
				return Crud.FlowDefCrud.SelectMany(command);
			}
			protected override List<FlowDef> TableToList(DataTable table) {
				return Crud.FlowDefCrud.TableToList(table);
			}
			protected override FlowDef Copy(FlowDef patientFlowDef) {
				return patientFlowDef.Copy();
			}

			protected override DataTable ListToTable(List<FlowDef> listFlowDefs) {
				return Crud.FlowDefCrud.ListToTable(listFlowDefs,"FlowDef");
			}
			protected override void FillCacheIfNeeded() {
				FlowDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static FlowDefCache _FlowDefCache = new FlowDefCache();

		public static List<FlowDef> GetDeepCopy(bool isShort = false) {
			return _FlowDefCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort = false) {
			return _FlowDefCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<FlowDef> match,bool isShort = false) {
			return _FlowDefCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<FlowDef> match,bool isShort = false) {
			return _FlowDefCache.GetFindIndex(match,isShort);
		}

		public static FlowDef GetFirst(bool isShort = false) {
			return _FlowDefCache.GetFirst(isShort);
		}

		public static FlowDef GetFirst(Func<FlowDef,bool> match,bool isShort = false) {
			return _FlowDefCache.GetFirst(match,isShort);
		}

		public static FlowDef GetFirstOrDefault(Func<FlowDef,bool> match,bool isShort = false) {
			return _FlowDefCache.GetFirstOrDefault(match,isShort);
		}

		public static FlowDef GetLast(bool isShort = false) {
			return _FlowDefCache.GetLast(isShort);
		}

		public static FlowDef GetLastOrDefault(Func<FlowDef,bool> match,bool isShort = false) {
			return _FlowDefCache.GetLastOrDefault(match,isShort);
		}

		public static List<FlowDef> GetWhere(Predicate<FlowDef> match,bool isShort = false) {
			return _FlowDefCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_FlowDefCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable.Always refreshes the ClientMT's cache.</summary>
		///<param name = "doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table = Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_FlowDefCache.FillCacheFromTable(table);
				return table;
			}
			return _FlowDefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_FlowDefCache.ClearCache();
		}
		#endregion Cache Pattern

		
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<FlowDef> GetByClinic(long clinicNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FlowDef>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT * FROM flowdef WHERE clinicNum = "+POut.Long(clinicNum);
			return Crud.FlowDefCrud.SelectMany(command);
		}
		
		///<summary>Gets one PatientFlowDef from the db.</summary>
		public static FlowDef GetOne(long patientFlowDefNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<FlowDef>(MethodBase.GetCurrentMethod(),patientFlowDefNum);
			}
			return Crud.FlowDefCrud.SelectOne(patientFlowDefNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(FlowDef flowDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				flowDef.FlowDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),flowDef);
				return flowDef.FlowDefNum;
			}
			return Crud.FlowDefCrud.Insert(flowDef);
		}
		///<summary></summary>
		public static void Update(FlowDef flowDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowDef);
				return;
			}
			Crud.FlowDefCrud.Update(flowDef);
		}
		///<summary></summary>
		public static void Delete(long flowDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowDefNum);
				return;
			}
			Crud.FlowDefCrud.Delete(flowDefNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}