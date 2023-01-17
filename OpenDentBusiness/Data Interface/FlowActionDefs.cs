using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FlowActionDefs{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class FlowActionDefCache : CacheListAbs<FlowActionDef> {
			protected override List<FlowActionDef> GetCacheFromDb() {
				string command="SELECT * FROM flowactiondef";
				return Crud.FlowActionDefCrud.SelectMany(command);
			}
			protected override List<FlowActionDef> TableToList(DataTable table) {
				return Crud.FlowActionDefCrud.TableToList(table);
			}
			protected override FlowActionDef Copy(FlowActionDef flowActionDef) {
				return flowActionDef.Copy();
			}
			protected override DataTable ListToTable(List<FlowActionDef> listFlowActionDefs) {
				return Crud.FlowActionDefCrud.ListToTable(listFlowActionDefs,"FlowActionDef");
			}
			protected override void FillCacheIfNeeded() {
				FlowActionDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static FlowActionDefCache _flowActionDefCache=new FlowActionDefCache();

		public static List<FlowActionDef> GetDeepCopy(bool isShort=false) {
			return _flowActionDefCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _flowActionDefCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<FlowActionDef> match,bool isShort=false) {
			return _flowActionDefCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<FlowActionDef> match,bool isShort=false) {
			return _flowActionDefCache.GetFindIndex(match,isShort);
		}

		public static FlowActionDef GetFirst(bool isShort=false) {
			return _flowActionDefCache.GetFirst(isShort);
		}

		public static FlowActionDef GetFirst(Func<FlowActionDef,bool> match,bool isShort=false) {
			return _flowActionDefCache.GetFirst(match,isShort);
		}

		public static FlowActionDef GetFirstOrDefault(Func<FlowActionDef,bool> match,bool isShort=false) {
			return _flowActionDefCache.GetFirstOrDefault(match,isShort);
		}

		public static FlowActionDef GetLast(bool isShort=false) {
			return _flowActionDefCache.GetLast(isShort);
		}

		public static FlowActionDef GetLastOrDefault(Func<FlowActionDef,bool> match,bool isShort=false) {
			return _flowActionDefCache.GetLastOrDefault(match,isShort);
		}

		public static List<FlowActionDef> GetWhere(Predicate<FlowActionDef> match,bool isShort=false) {
			return _flowActionDefCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_flowActionDefCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_flowActionDefCache.FillCacheFromTable(table);
				return table;
			}
			return _flowActionDefCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		//lOnly pull out the methods below as you need them.Otherwise, leave them commented out.
		#region Methods - Get
		///<summary>Returns all ActionsDefs associated with a passed in FlowDefNum. Items return in order based on ItemOrder.</summary>
		public static List<FlowActionDef> GetAllByFlowDef(long flowDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FlowActionDef>>(MethodBase.GetCurrentMethod(),flowDefNum);
			}
			string command = $"SELECT * FROM flowactiondef WHERE flowdefnum = {POut.Long(flowDefNum)} ORDER BY ItemOrder" ;
			return Crud.FlowActionDefCrud.SelectMany(command);
		}

		///<summary>Gets one FlowActionDef from the db.</summary>
		public static FlowActionDef GetOne(long flowActionDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<FlowActionDef>(MethodBase.GetCurrentMethod(),flowActionDefNum);
			}
			return Crud.FlowActionDefCrud.SelectOne(flowActionDefNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(FlowActionDef flowActionDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				flowActionDef.FlowActionDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),flowActionDef);
				return flowActionDef.FlowActionDefNum;
			}
			return Crud.FlowActionDefCrud.Insert(flowActionDef);
		}
		///<summary></summary>
		public static void Update(FlowActionDef flowActionDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowActionDef);
				return;
			}
			Crud.FlowActionDefCrud.Update(flowActionDef);
		}
		///<summary></summary>
		public static long Upsert(FlowActionDef flowActionDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				flowActionDef.FlowActionDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),flowActionDef);
				return flowActionDef.FlowActionDefNum;
			}
			if(flowActionDef.FlowActionDefNum==0) {
				return Crud.FlowActionDefCrud.Insert(flowActionDef);
			}
			Crud.FlowActionDefCrud.Update(flowActionDef);
			return flowActionDef.FlowActionDefNum;
		}
		///<summary></summary>
		public static void Delete(long flowActionDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowActionDefNum);
				return;
			}
			Crud.FlowActionDefCrud.Delete(flowActionDefNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc



		#endregion Methods - Misc




	}
}