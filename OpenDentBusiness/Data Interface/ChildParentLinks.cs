using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ChildParentLinks{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ChildParentLinkCache : CacheListAbs<ChildParentLink> {
			protected override List<ChildParentLink> GetCacheFromDb() {
				string command="SELECT * FROM childparentlink";
				return Crud.ChildParentLinkCrud.SelectMany(command);
			}
			protected override List<ChildParentLink> TableToList(DataTable table) {
				return Crud.ChildParentLinkCrud.TableToList(table);
			}
			protected override ChildParentLink Copy(ChildParentLink childParentLink) {
				return childParentLink.Copy();
			}
			protected override DataTable ListToTable(List<ChildParentLink> listChildParentLinks) {
				return Crud.ChildParentLinkCrud.ListToTable(listChildParentLinks,"ChildParentLink");
			}
			protected override void FillCacheIfNeeded() {
				ChildParentLinks.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ChildParentLinkCache _childParentLinkCache=new ChildParentLinkCache();

		public static void ClearCache() {
			_childParentLinkCache.ClearCache();
		}

		public static List<ChildParentLink> GetDeepCopy(bool isShort=false) {
			return _childParentLinkCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _childParentLinkCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ChildParentLink> match,bool isShort=false) {
			return _childParentLinkCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ChildParentLink> match,bool isShort=false) {
			return _childParentLinkCache.GetFindIndex(match,isShort);
		}

		public static ChildParentLink GetFirst(bool isShort=false) {
			return _childParentLinkCache.GetFirst(isShort);
		}

		public static ChildParentLink GetFirst(Func<ChildParentLink,bool> match,bool isShort=false) {
			return _childParentLinkCache.GetFirst(match,isShort);
		}

		public static ChildParentLink GetFirstOrDefault(Func<ChildParentLink,bool> match,bool isShort=false) {
			return _childParentLinkCache.GetFirstOrDefault(match,isShort);
		}

		public static ChildParentLink GetLast(bool isShort=false) {
			return _childParentLinkCache.GetLast(isShort);
		}

		public static ChildParentLink GetLastOrDefault(Func<ChildParentLink,bool> match,bool isShort=false) {
			return _childParentLinkCache.GetLastOrDefault(match,isShort);
		}

		public static List<ChildParentLink> GetWhere(Predicate<ChildParentLink> match,bool isShort=false) {
			return _childParentLinkCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_childParentLinkCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_childParentLinkCache.FillCacheFromTable(table);
				return table;
			}
			return _childParentLinkCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<ChildParentLink> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildParentLink>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM childparentlink WHERE PatNum = "+POut.Long(patNum);
			return Crud.ChildParentLinkCrud.SelectMany(command);
		}
		
		///<summary>Gets one ChildParentLink from the db.</summary>
		public static ChildParentLink GetOne(long childParentLinkNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ChildParentLink>(MethodBase.GetCurrentMethod(),childParentLinkNum);
			}
			return Crud.ChildParentLinkCrud.SelectOne(childParentLinkNum);
		}
		#endregion Methods - Get
		*/

		#region Methods - Get
		///<summary>Returns a list of all ChildParentLinks with the given childNum.</summary>
		public static List<ChildParentLink> GetChildParentLinksByChildNum(long childNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildParentLink>>(MethodBase.GetCurrentMethod(),childNum);
			}
			string command="SELECT * FROM childparentlink WHERE ChildNum="+POut.Long(childNum);
			return Crud.ChildParentLinkCrud.SelectMany(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ChildParentLink childParentLink){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				childParentLink.ChildParentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),childParentLink);
				return childParentLink.ChildParentNum;
			}
			return Crud.ChildParentLinkCrud.Insert(childParentLink);
		}

		///<summary></summary>
		public static void Update(ChildParentLink childParentLink){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childParentLink);
				return;
			}
			Crud.ChildParentLinkCrud.Update(childParentLink);
		}

		///<summary></summary>
		public static void Delete(long childParentLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childParentLinkNum);
				return;
			}
			Crud.ChildParentLinkCrud.Delete(childParentLinkNum);
		}
		#endregion Methods - Modify


	}
}