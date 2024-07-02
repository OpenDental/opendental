using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Children{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ChildCache : CacheListAbs<Child> {
			protected override List<Child> GetCacheFromDb() {
				string command="SELECT * FROM child";
				return Crud.ChildCrud.SelectMany(command);
			}
			protected override List<Child> TableToList(DataTable table) {
				return Crud.ChildCrud.TableToList(table);
			}
			protected override Child Copy(Child child) {
				return child.Copy();
			}
			protected override DataTable ListToTable(List<Child> listChilds) {
				return Crud.ChildCrud.ListToTable(listChilds,"Child");
			}
			protected override void FillCacheIfNeeded() {
				Childs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ChildCache _childCache=new ChildCache();

		public static void ClearCache() {
			_childCache.ClearCache();
		}

		public static List<Child> GetDeepCopy(bool isShort=false) {
			return _childCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _childCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<Child> match,bool isShort=false) {
			return _childCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<Child> match,bool isShort=false) {
			return _childCache.GetFindIndex(match,isShort);
		}

		public static Child GetFirst(bool isShort=false) {
			return _childCache.GetFirst(isShort);
		}

		public static Child GetFirst(Func<Child,bool> match,bool isShort=false) {
			return _childCache.GetFirst(match,isShort);
		}

		public static Child GetFirstOrDefault(Func<Child,bool> match,bool isShort=false) {
			return _childCache.GetFirstOrDefault(match,isShort);
		}

		public static Child GetLast(bool isShort=false) {
			return _childCache.GetLast(isShort);
		}

		public static Child GetLastOrDefault(Func<Child,bool> match,bool isShort=false) {
			return _childCache.GetLastOrDefault(match,isShort);
		}

		public static List<Child> GetWhere(Predicate<Child> match,bool isShort=false) {
			return _childCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_childCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_childCache.FillCacheFromTable(table);
				return table;
			}
			return _childCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<Child> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Child>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM child WHERE PatNum = "+POut.Long(patNum);
			return Crud.ChildCrud.SelectMany(command);
		}
		
		#endregion Methods - Get
		*/

		///<summary>Gets one Child from the db.</summary>
		public static Child GetOne(long childNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<Child>(MethodBase.GetCurrentMethod(),childNum);
			}
			return Crud.ChildCrud.SelectOne(childNum);
		}

		///<summary>Returns a list containing all children.</summary>
		public static List<Child> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Child>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM child ORDER BY FName";
			return Crud.ChildCrud.SelectMany(command);
		}

		///<summary>Returns the full name of the child passed in. Returns an empty string if child is null.</summary>
		public static string GetName(Child child) {
			//No remoting role check; no call to db
			if(child==null) {
				return "";
			}
			return child.FName+" "+child.LName;
		}

		#region Methods - Modify

		///<summary></summary>
		public static long Insert(Child child){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				child.ChildNum=Meth.GetLong(MethodBase.GetCurrentMethod(),child);
				return child.ChildNum;
			}
			return Crud.ChildCrud.Insert(child);
		}

		///<summary></summary>
		public static void Update(Child child){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),child);
				return;
			}
			Crud.ChildCrud.Update(child);
		}

		///<summary></summary>
		public static void Delete(long childNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childNum);
				return;
			}
			Crud.ChildCrud.Delete(childNum);
		}

		#endregion Methods - Modify

	}
}