using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ChildParents{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ChildParentCache : CacheListAbs<ChildParent> {
			protected override List<ChildParent> GetCacheFromDb() {
				string command="SELECT * FROM childparent";
				return Crud.ChildParentCrud.SelectMany(command);
			}
			protected override List<ChildParent> TableToList(DataTable table) {
				return Crud.ChildParentCrud.TableToList(table);
			}
			protected override ChildParent Copy(ChildParent childParent) {
				return childParent.Copy();
			}
			protected override DataTable ListToTable(List<ChildParent> listChildParents) {
				return Crud.ChildParentCrud.ListToTable(listChildParents,"ChildParent");
			}
			protected override void FillCacheIfNeeded() {
				ChildParents.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ChildParentCache _childParentCache=new ChildParentCache();

		public static void ClearCache() {
			_childParentCache.ClearCache();
		}

		public static List<ChildParent> GetDeepCopy(bool isShort=false) {
			return _childParentCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _childParentCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ChildParent> match,bool isShort=false) {
			return _childParentCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ChildParent> match,bool isShort=false) {
			return _childParentCache.GetFindIndex(match,isShort);
		}

		public static ChildParent GetFirst(bool isShort=false) {
			return _childParentCache.GetFirst(isShort);
		}

		public static ChildParent GetFirst(Func<ChildParent,bool> match,bool isShort=false) {
			return _childParentCache.GetFirst(match,isShort);
		}

		public static ChildParent GetFirstOrDefault(Func<ChildParent,bool> match,bool isShort=false) {
			return _childParentCache.GetFirstOrDefault(match,isShort);
		}

		public static ChildParent GetLast(bool isShort=false) {
			return _childParentCache.GetLast(isShort);
		}

		public static ChildParent GetLastOrDefault(Func<ChildParent,bool> match,bool isShort=false) {
			return _childParentCache.GetLastOrDefault(match,isShort);
		}

		public static List<ChildParent> GetWhere(Predicate<ChildParent> match,bool isShort=false) {
			return _childParentCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_childParentCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_childParentCache.FillCacheFromTable(table);
				return table;
			}
			return _childParentCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<ChildParent> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildParent>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM childparent WHERE PatNum = "+POut.Long(patNum);
			return Crud.ChildParentCrud.SelectMany(command);
		}
		
		///<summary>Gets one ChildParent from the db.</summary>
		public static ChildParent GetOne(long childParentNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ChildParent>(MethodBase.GetCurrentMethod(),childParentNum);
			}
			return Crud.ChildParentCrud.SelectOne(childParentNum);
		}
		#endregion Methods - Get
		*/

		#region Methods - Get
		///<summary>Returns a list of all ChildParents with the given childNum.</summary>
		public static List<ChildParent> GetChildParentsByChildNum(long childNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildParent>>(MethodBase.GetCurrentMethod(),childNum);
			}
			string command="SELECT * FROM childparent WHERE ChildNum="+POut.Long(childNum);
			return Crud.ChildParentCrud.SelectMany(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ChildParent childParent){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				childParent.ChildParentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),childParent);
				return childParent.ChildParentNum;
			}
			return Crud.ChildParentCrud.Insert(childParent);
		}

		///<summary></summary>
		public static void Update(ChildParent childParent){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childParent);
				return;
			}
			Crud.ChildParentCrud.Update(childParent);
		}

		///<summary></summary>
		public static void Delete(long childParentNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childParentNum);
				return;
			}
			Crud.ChildParentCrud.Delete(childParentNum);
		}
		#endregion Methods - Modify

	}
}