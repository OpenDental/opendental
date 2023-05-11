using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ERoutingActionDefs{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ERoutingActionDefCache : CacheListAbs<ERoutingActionDef> {
			protected override List<ERoutingActionDef> GetCacheFromDb() {
				string command="SELECT * FROM eroutingactiondef";
				return Crud.ERoutingActionDefCrud.SelectMany(command);
			}
			protected override List<ERoutingActionDef> TableToList(DataTable table) {
				return Crud.ERoutingActionDefCrud.TableToList(table);
			}
			protected override ERoutingActionDef Copy(ERoutingActionDef eroutingActionDef) {
				return eroutingActionDef.Copy();
			}
			protected override DataTable ListToTable(List<ERoutingActionDef> listERoutingActionDefs) {
				return Crud.ERoutingActionDefCrud.ListToTable(listERoutingActionDefs,"ERoutingActionDef");
			}
			protected override void FillCacheIfNeeded() {
				ERoutingActionDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ERoutingActionDefCache _eroutingActionDefCache=new ERoutingActionDefCache();

		public static List<ERoutingActionDef> GetDeepCopy(bool isShort=false) {
			return _eroutingActionDefCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _eroutingActionDefCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ERoutingActionDef> match,bool isShort=false) {
			return _eroutingActionDefCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ERoutingActionDef> match,bool isShort=false) {
			return _eroutingActionDefCache.GetFindIndex(match,isShort);
		}

		public static ERoutingActionDef GetFirst(bool isShort=false) {
			return _eroutingActionDefCache.GetFirst(isShort);
		}

		public static ERoutingActionDef GetFirst(Func<ERoutingActionDef,bool> match,bool isShort=false) {
			return _eroutingActionDefCache.GetFirst(match,isShort);
		}

		public static ERoutingActionDef GetFirstOrDefault(Func<ERoutingActionDef,bool> match,bool isShort=false) {
			return _eroutingActionDefCache.GetFirstOrDefault(match,isShort);
		}

		public static ERoutingActionDef GetLast(bool isShort=false) {
			return _eroutingActionDefCache.GetLast(isShort);
		}

		public static ERoutingActionDef GetLastOrDefault(Func<ERoutingActionDef,bool> match,bool isShort=false) {
			return _eroutingActionDefCache.GetLastOrDefault(match,isShort);
		}

		public static List<ERoutingActionDef> GetWhere(Predicate<ERoutingActionDef> match,bool isShort=false) {
			return _eroutingActionDefCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_eroutingActionDefCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_eroutingActionDefCache.FillCacheFromTable(table);
				return table;
			}
			return _eroutingActionDefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_eroutingActionDefCache.ClearCache();
		}
		#endregion Cache Pattern

		//lOnly pull out the methods below as you need them.Otherwise, leave them commented out.
		#region Methods - Get
		///<summary>Returns all ActionsDefs associated with a passed in ERoutingDefNum. Items return in order based on ItemOrder.</summary>
		public static List<ERoutingActionDef> GetAllByERoutingDef(long eroutingDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingActionDef>>(MethodBase.GetCurrentMethod(),eroutingDefNum);
			}
			string command = $"SELECT * FROM eroutingactiondef WHERE eroutingdefnum = {POut.Long(eroutingDefNum)} ORDER BY ItemOrder" ;
			return Crud.ERoutingActionDefCrud.SelectMany(command);
		}

		///<summary>Gets one ERoutingActionDef from the db.</summary>
		public static ERoutingActionDef GetOne(long eroutingActionDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ERoutingActionDef>(MethodBase.GetCurrentMethod(),eroutingActionDefNum);
			}
			return Crud.ERoutingActionDefCrud.SelectOne(eroutingActionDefNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ERoutingActionDef eroutingActionDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				eroutingActionDef.ERoutingActionDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eroutingActionDef);
				return eroutingActionDef.ERoutingActionDefNum;
			}
			return Crud.ERoutingActionDefCrud.Insert(eroutingActionDef);
		}
		///<summary></summary>
		public static void Update(ERoutingActionDef eroutingActionDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingActionDef);
				return;
			}
			Crud.ERoutingActionDefCrud.Update(eroutingActionDef);
		}
		///<summary></summary>
		public static long Upsert(ERoutingActionDef eroutingActionDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				eroutingActionDef.ERoutingActionDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eroutingActionDef);
				return eroutingActionDef.ERoutingActionDefNum;
			}
			if(eroutingActionDef.ERoutingActionDefNum==0) {
				return Crud.ERoutingActionDefCrud.Insert(eroutingActionDef);
			}
			Crud.ERoutingActionDefCrud.Update(eroutingActionDef);
			return eroutingActionDef.ERoutingActionDefNum;
		}
		///<summary></summary>
		public static void Delete(long eroutingActionDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingActionDefNum);
				return;
			}
			Crud.ERoutingActionDefCrud.Delete(eroutingActionDefNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc



		#endregion Methods - Misc




	}
}