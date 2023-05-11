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
				string command="SELECT * FROM eroutingdeflink";
				return Crud.ERoutingDefLinkCrud.SelectMany(command);
			}
			protected override List<ERoutingDefLink> TableToList(DataTable table) {
				return Crud.ERoutingDefLinkCrud.TableToList(table);
			}
			protected override ERoutingDefLink Copy(ERoutingDefLink patientERoutingDefLink) {
				return patientERoutingDefLink.Copy();
			}
			protected override DataTable ListToTable(List<ERoutingDefLink> listERoutingDefLinks) {
				return Crud.ERoutingDefLinkCrud.ListToTable(listERoutingDefLinks,"ERoutingDefLink");
			}
			protected override void FillCacheIfNeeded() {
				ERoutingDefLinks.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ERoutingDefLinkCache _eroutingDefLinkCache=new ERoutingDefLinkCache();

		public static List<ERoutingDefLink> GetDeepCopy(bool isShort=false) {
			return _eroutingDefLinkCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _eroutingDefLinkCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ERoutingDefLink> match,bool isShort=false) {
			return _eroutingDefLinkCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ERoutingDefLink> match,bool isShort=false) {
			return _eroutingDefLinkCache.GetFindIndex(match,isShort);
		}

		public static ERoutingDefLink GetFirst(bool isShort=false) {
			return _eroutingDefLinkCache.GetFirst(isShort);
		}

		public static ERoutingDefLink GetFirst(Func<ERoutingDefLink,bool> match,bool isShort=false) {
			return _eroutingDefLinkCache.GetFirst(match,isShort);
		}

		public static ERoutingDefLink GetFirstOrDefault(Func<ERoutingDefLink,bool> match,bool isShort=false) {
			return _eroutingDefLinkCache.GetFirstOrDefault(match,isShort);
		}

		public static ERoutingDefLink GetLast(bool isShort=false) {
			return _eroutingDefLinkCache.GetLast(isShort);
		}

		public static ERoutingDefLink GetLastOrDefault(Func<ERoutingDefLink,bool> match,bool isShort=false) {
			return _eroutingDefLinkCache.GetLastOrDefault(match,isShort);
		}

		public static List<ERoutingDefLink> GetWhere(Predicate<ERoutingDefLink> match,bool isShort=false) {
			return _eroutingDefLinkCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_eroutingDefLinkCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_eroutingDefLinkCache.FillCacheFromTable(table);
				return table;
			}
			return _eroutingDefLinkCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_eroutingDefLinkCache.ClearCache();
		}
		#endregion Cache Pattern

		#region Methods - Get
		///<summary></summary>
		public static List<ERoutingDefLink> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingDefLink>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command = "SELECT * FROM eroutingdeflink WHERE PatNum = "+POut.Long(patNum);
			return Crud.ERoutingDefLinkCrud.SelectMany(command);
		}

		///<summary>Gets a list of each type of def link a give patienterouting has. Used in FormERoutingEdit to know what kinds of links are selected.</summary>
		public static List<ERoutingDefLink> GetListERoutingTypesForERoutingDefNum(long eroutingDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingDefLink>>(MethodBase.GetCurrentMethod(),eroutingDefNum);
			}
			string command = $"SELECT * FROM eroutingdeflink WHERE ERoutingDefNum = {POut.Long(eroutingDefNum)} GROUP BY ERoutingType";
			return Crud.ERoutingDefLinkCrud.SelectMany(command).ToList();
		}

		///<summary>Gets one PatientERoutingDefLink from the db.</summary>
		public static ERoutingDefLink GetOne(long eroutingDefLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ERoutingDefLink>(MethodBase.GetCurrentMethod(),eroutingDefLinkNum);
			}
			return Crud.ERoutingDefLinkCrud.SelectOne(eroutingDefLinkNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ERoutingDefLink eroutingDefLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				eroutingDefLink.ERoutingDefLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eroutingDefLink);
				return eroutingDefLink.ERoutingDefLinkNum;
			}
			return Crud.ERoutingDefLinkCrud.Insert(eroutingDefLink);
		}

		///<summary></summary>
		public static void Update(ERoutingDefLink eroutingDefLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingDefLink);
				return;
			}
			Crud.ERoutingDefLinkCrud.Update(eroutingDefLink);
		}
		///<summary></summary>
		public static void Delete(long eroutingDefLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingDefLinkNum);
				return;
			}
			Crud.ERoutingDefLinkCrud.Delete(eroutingDefLinkNum);
		}
		
		/// <summary>Deletes everything for a given ERoutingDefNum. Used during setup to create a clean slate and only save what is selected in form.</summary>
		public static void DeleteAll(long eroutingDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingDefNum);
				return;
			}
			string command = $"DELETE FROM eroutingdeflink WHERE ERoutingDefNum = {POut.Long(eroutingDefNum)}";
			Db.NonQ(command);
		}

		#endregion Methods - Modify
		#region Methods - Misc



		#endregion Methods - Misc




	}
}