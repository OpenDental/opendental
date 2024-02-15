using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class PatFieldPickItems {
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class PatFieldPickItemCache : CacheListAbs<PatFieldPickItem> {
			protected override List<PatFieldPickItem> GetCacheFromDb() {
				string command="SELECT * FROM patfieldpickitem";
				return Crud.PatFieldPickItemCrud.SelectMany(command);
			}
			protected override List<PatFieldPickItem> TableToList(DataTable table) {
				return Crud.PatFieldPickItemCrud.TableToList(table);
			}
			protected override PatFieldPickItem Copy(PatFieldPickItem patFieldPickItem) {
				return patFieldPickItem.Copy();
			}
			protected override DataTable ListToTable(List<PatFieldPickItem> listPatFieldPickItems) {
				return Crud.PatFieldPickItemCrud.ListToTable(listPatFieldPickItems,"PatFieldPickItem");
			}
			protected override void FillCacheIfNeeded() {
				PatFieldPickItems.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PatFieldPickItemCache _patFieldPickItemCache=new PatFieldPickItemCache();

		public static void ClearCache() {
			_patFieldPickItemCache.ClearCache();
		}

		public static List<PatFieldPickItem> GetDeepCopy(bool isShort=false) {
			return _patFieldPickItemCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _patFieldPickItemCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<PatFieldPickItem> match,bool isShort=false) {
			return _patFieldPickItemCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<PatFieldPickItem> match,bool isShort=false) {
			return _patFieldPickItemCache.GetFindIndex(match,isShort);
		}

		public static PatFieldPickItem GetFirst(bool isShort=false) {
			return _patFieldPickItemCache.GetFirst(isShort);
		}

		public static PatFieldPickItem GetFirst(Func<PatFieldPickItem,bool> match,bool isShort=false) {
			return _patFieldPickItemCache.GetFirst(match,isShort);
		}

		public static PatFieldPickItem GetFirstOrDefault(Func<PatFieldPickItem,bool> match,bool isShort=false) {
			return _patFieldPickItemCache.GetFirstOrDefault(match,isShort);
		}

		public static PatFieldPickItem GetLast(bool isShort=false) {
			return _patFieldPickItemCache.GetLast(isShort);
		}

		public static PatFieldPickItem GetLastOrDefault(Func<PatFieldPickItem,bool> match,bool isShort=false) {
			return _patFieldPickItemCache.GetLastOrDefault(match,isShort);
		}

		public static List<PatFieldPickItem> GetWhere(Predicate<PatFieldPickItem> match,bool isShort=false) {
			return _patFieldPickItemCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_patFieldPickItemCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_patFieldPickItemCache.FillCacheFromTable(table);
				return table;
			}
			return _patFieldPickItemCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(PatFieldPickItem patFieldPickItem) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				patFieldPickItem.PatFieldPickItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),patFieldPickItem);
				return patFieldPickItem.PatFieldPickItemNum;
			}
			return Crud.PatFieldPickItemCrud.Insert(patFieldPickItem);
		}

		///<summary></summary>
		public static void Update(PatFieldPickItem patFieldPickItem) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patFieldPickItem);
				return;
			}
			Crud.PatFieldPickItemCrud.Update(patFieldPickItem);
		}

		///<summary></summary>
		public static void Delete(long patFieldPickItemNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patFieldPickItemNum);
				return;
			}
			Crud.PatFieldPickItemCrud.Delete(patFieldPickItemNum);
		}
		#endregion Methods - Modify

/*		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<PatFieldPickItem> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PatFieldPickItem>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM patfieldpickitem WHERE PatNum = "+POut.Long(patNum);
			return Crud.PatFieldPickItemCrud.SelectMany(command);
		}

		///<summary>Gets one PatFieldPickItem from the db.</summary>
		public static PatFieldPickItem GetOne(long patFieldPickItemNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<PatFieldPickItem>(MethodBase.GetCurrentMethod(),patFieldPickItemNum);
			}
			return Crud.PatFieldPickItemCrud.SelectOne(patFieldPickItemNum);
		}
		#endregion Methods - Get*/
	}
}