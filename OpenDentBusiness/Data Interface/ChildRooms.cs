using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ChildRooms{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ChildRoomCache : CacheListAbs<ChildRoom> {
			protected override List<ChildRoom> GetCacheFromDb() {
				string command="SELECT * FROM childroom";
				return Crud.ChildRoomCrud.SelectMany(command);
			}
			protected override List<ChildRoom> TableToList(DataTable table) {
				return Crud.ChildRoomCrud.TableToList(table);
			}
			protected override ChildRoom Copy(ChildRoom childRoom) {
				return childRoom.Copy();
			}
			protected override DataTable ListToTable(List<ChildRoom> listChildRooms) {
				return Crud.ChildRoomCrud.ListToTable(listChildRooms,"ChildRoom");
			}
			protected override void FillCacheIfNeeded() {
				ChildRooms.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ChildRoomCache _childRoomCache=new ChildRoomCache();

		public static void ClearCache() {
			_childRoomCache.ClearCache();
		}

		public static List<ChildRoom> GetDeepCopy(bool isShort=false) {
			return _childRoomCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _childRoomCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ChildRoom> match,bool isShort=false) {
			return _childRoomCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ChildRoom> match,bool isShort=false) {
			return _childRoomCache.GetFindIndex(match,isShort);
		}

		public static ChildRoom GetFirst(bool isShort=false) {
			return _childRoomCache.GetFirst(isShort);
		}

		public static ChildRoom GetFirst(Func<ChildRoom,bool> match,bool isShort=false) {
			return _childRoomCache.GetFirst(match,isShort);
		}

		public static ChildRoom GetFirstOrDefault(Func<ChildRoom,bool> match,bool isShort=false) {
			return _childRoomCache.GetFirstOrDefault(match,isShort);
		}

		public static ChildRoom GetLast(bool isShort=false) {
			return _childRoomCache.GetLast(isShort);
		}

		public static ChildRoom GetLastOrDefault(Func<ChildRoom,bool> match,bool isShort=false) {
			return _childRoomCache.GetLastOrDefault(match,isShort);
		}

		public static List<ChildRoom> GetWhere(Predicate<ChildRoom> match,bool isShort=false) {
			return _childRoomCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_childRoomCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_childRoomCache.FillCacheFromTable(table);
				return table;
			}
			return _childRoomCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		public static string GetRoomId(long childRoomNum){
			//No need to check MiddleTierRole; no call to db.
			ChildRoom childRoom=GetFirstOrDefault(x=>x.ChildRoomNum==childRoomNum);
			if(childRoom is null){
				return "";
			}
			return childRoom.RoomId;
		}
		
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<ChildRoom> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoom>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM childroom WHERE PatNum = "+POut.Long(patNum);
			return Crud.ChildRoomCrud.SelectMany(command);
		}
		
		///<summary>Gets one ChildRoom from the db.</summary>
		public static ChildRoom GetOne(long childRoomNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ChildRoom>(MethodBase.GetCurrentMethod(),childRoomNum);
			}
			return Crud.ChildRoomCrud.SelectOne(childRoomNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static void Delete(long childRoomNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childRoomNum);
				return;
			}
			Crud.ChildRoomCrud.Delete(childRoomNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc
		*/

		///<summary></summary>
		public static List<ChildRoom> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoom>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM childroom ORDER BY RoomId";
			return Crud.ChildRoomCrud.TableToList(Db.GetTable(command));
		}

		///<summary></summary>
		public static long Insert(ChildRoom childRoom){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				childRoom.ChildRoomNum=Meth.GetLong(MethodBase.GetCurrentMethod(),childRoom);
				return childRoom.ChildRoomNum;
			}
			return Crud.ChildRoomCrud.Insert(childRoom);
		}

		///<summary></summary>
		public static void Update(ChildRoom childRoom){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childRoom);
				return;
			}
			Crud.ChildRoomCrud.Update(childRoom);
		}

	}
}