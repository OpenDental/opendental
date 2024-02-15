using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ERoutingDefLinks{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ERoutingDefLinkCache : CacheListAbs<ERoutingDefLink> {
			protected override List<ERoutingDefLink> GetCacheFromDb() {
				string command= "SELECT * FROM eroutingdeflink";
				return Crud.ERoutingDefLinkCrud.SelectMany(command);
			}
			protected override List<ERoutingDefLink> TableToList(DataTable table) {
				return Crud.ERoutingDefLinkCrud.TableToList(table);
			}
			protected override ERoutingDefLink Copy(ERoutingDefLink eRoutingDefLink) {
				return eRoutingDefLink.Copy();
			}
			protected override DataTable ListToTable(List<ERoutingDefLink> listERoutingDefLinks) {
				return Crud.ERoutingDefLinkCrud.ListToTable(listERoutingDefLinks,"ERoutingDefLink");
			}
			protected override void FillCacheIfNeeded() {
				ERoutingDefLinks.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ERoutingDefLinkCache _eRoutingDefLinkCache=new ERoutingDefLinkCache();

		public static List<ERoutingDefLink> GetDeepCopy(bool isShort=false) {
			return _eRoutingDefLinkCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _eRoutingDefLinkCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ERoutingDefLink> match,bool isShort=false) {
			return _eRoutingDefLinkCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ERoutingDefLink> match,bool isShort=false) {
			return _eRoutingDefLinkCache.GetFindIndex(match,isShort);
		}

		public static ERoutingDefLink GetFirst(bool isShort=false) {
			return _eRoutingDefLinkCache.GetFirst(isShort);
		}

		public static ERoutingDefLink GetFirst(Func<ERoutingDefLink,bool> match,bool isShort=false) {
			return _eRoutingDefLinkCache.GetFirst(match,isShort);
		}

		public static ERoutingDefLink GetFirstOrDefault(Func<ERoutingDefLink,bool> match,bool isShort=false) {
			return _eRoutingDefLinkCache.GetFirstOrDefault(match,isShort);
		}

		public static ERoutingDefLink GetLast(bool isShort=false) {
			return _eRoutingDefLinkCache.GetLast(isShort);
		}

		public static ERoutingDefLink GetLastOrDefault(Func<ERoutingDefLink,bool> match,bool isShort=false) {
			return _eRoutingDefLinkCache.GetLastOrDefault(match,isShort);
		}

		public static List<ERoutingDefLink> GetWhere(Predicate<ERoutingDefLink> match,bool isShort=false) {
			return _eRoutingDefLinkCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_eRoutingDefLinkCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_eRoutingDefLinkCache.FillCacheFromTable(table);
				return table;
			}
			return _eRoutingDefLinkCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_eRoutingDefLinkCache.ClearCache();
		}
		#endregion Cache Pattern

		#region Methods - Get
		///<summary></summary>
		public static List<ERoutingDefLink> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingDefLink>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command = "SELECT * FROM eroutingdeflink WHERE PatNum = " + POut.Long(patNum);
			return Crud.ERoutingDefLinkCrud.SelectMany(command);
		}

		///<summary>Gets a list of each type of def link a give erouting has. Used in FormERoutingEdit to know what kinds of links are selected.</summary>
		public static List<ERoutingDefLink> GetListERoutingTypesForERoutingDefNum(long eRoutingDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingDefLink>>(MethodBase.GetCurrentMethod(),eRoutingDefNum);
			}
			string command = $"SELECT * FROM eroutingdeflink WHERE ERoutingDefNum = {POut.Long(eRoutingDefNum)} GROUP BY ERoutingType";
			return Crud.ERoutingDefLinkCrud.SelectMany(command).ToList();
		}

		///<summary>Gets one ERoutingDefLink from the db.</summary>
		public static ERoutingDefLink GetOne(long eRoutingDefLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ERoutingDefLink>(MethodBase.GetCurrentMethod(),eRoutingDefLinkNum);
			}
			return Crud.ERoutingDefLinkCrud.SelectOne(eRoutingDefLinkNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ERoutingDefLink eRoutingDefLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				eRoutingDefLink.ERoutingDefLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eRoutingDefLink);
				return eRoutingDefLink.ERoutingDefLinkNum;
			}
			return Crud.ERoutingDefLinkCrud.Insert(eRoutingDefLink);
		}

		///<summary></summary>
		public static void Update(ERoutingDefLink eRoutingDefLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eRoutingDefLink);
				return;
			}
			Crud.ERoutingDefLinkCrud.Update(eRoutingDefLink);
		}
		///<summary></summary>
		public static void Delete(long eRoutingDefLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eRoutingDefLinkNum);
				return;
			}
			Crud.ERoutingDefLinkCrud.Delete(eRoutingDefLinkNum);
		}
		
		/// <summary>Deletes everything for a given ERoutingDefNum. Used during setup to create a clean slate and only save what is selected in form.</summary>
		public static void DeleteAll(long eRoutingDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eRoutingDefNum);
				return;
			}
			string command = $"DELETE FROM eroutingdeflink WHERE ERoutingDefNum = {POut.Long(eRoutingDefNum)}";
			Db.NonQ(command);
		}

		#endregion Methods - Modify
		#region Methods - Misc



		#endregion Methods - Misc




	}
}