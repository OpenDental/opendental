using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ChildRoomLogs{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ChildRoomLogCache : CacheListAbs<ChildRoomLog> {
			protected override List<ChildRoomLog> GetCacheFromDb() {
				string command="SELECT * FROM childroomlog";
				return Crud.ChildRoomLogCrud.SelectMany(command);
			}
			protected override List<ChildRoomLog> TableToList(DataTable table) {
				return Crud.ChildRoomLogCrud.TableToList(table);
			}
			protected override ChildRoomLog Copy(ChildRoomLog childRoomLog) {
				return childRoomLog.Copy();
			}
			protected override DataTable ListToTable(List<ChildRoomLog> listChildRoomLogs) {
				return Crud.ChildRoomLogCrud.ListToTable(listChildRoomLogs,"ChildRoomLog");
			}
			protected override void FillCacheIfNeeded() {
				ChildRoomLogs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ChildRoomLogCache _childRoomLogCache=new ChildRoomLogCache();

		public static void ClearCache() {
			_childRoomLogCache.ClearCache();
		}

		public static List<ChildRoomLog> GetDeepCopy(bool isShort=false) {
			return _childRoomLogCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _childRoomLogCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ChildRoomLog> match,bool isShort=false) {
			return _childRoomLogCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ChildRoomLog> match,bool isShort=false) {
			return _childRoomLogCache.GetFindIndex(match,isShort);
		}

		public static ChildRoomLog GetFirst(bool isShort=false) {
			return _childRoomLogCache.GetFirst(isShort);
		}

		public static ChildRoomLog GetFirst(Func<ChildRoomLog,bool> match,bool isShort=false) {
			return _childRoomLogCache.GetFirst(match,isShort);
		}

		public static ChildRoomLog GetFirstOrDefault(Func<ChildRoomLog,bool> match,bool isShort=false) {
			return _childRoomLogCache.GetFirstOrDefault(match,isShort);
		}

		public static ChildRoomLog GetLast(bool isShort=false) {
			return _childRoomLogCache.GetLast(isShort);
		}

		public static ChildRoomLog GetLastOrDefault(Func<ChildRoomLog,bool> match,bool isShort=false) {
			return _childRoomLogCache.GetLastOrDefault(match,isShort);
		}

		public static List<ChildRoomLog> GetWhere(Predicate<ChildRoomLog> match,bool isShort=false) {
			return _childRoomLogCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_childRoomLogCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_childRoomLogCache.FillCacheFromTable(table);
				return table;
			}
			return _childRoomLogCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<ChildRoomLog> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoomLog>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM childroomlog WHERE PatNum = "+POut.Long(patNum);
			return Crud.ChildRoomLogCrud.SelectMany(command);
		}
		
		///<summary>Gets one ChildRoomLog from the db.</summary>
		public static ChildRoomLog GetOne(long childRoomLogNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ChildRoomLog>(MethodBase.GetCurrentMethod(),childRoomLogNum);
			}
			return Crud.ChildRoomLogCrud.SelectOne(childRoomLogNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static void Update(ChildRoomLog childRoomLog){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childRoomLog);
				return;
			}
			Crud.ChildRoomLogCrud.Update(childRoomLog);
		}
		///<summary></summary>
		public static void Delete(long childRoomLogNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childRoomLogNum);
				return;
			}
			Crud.ChildRoomLogCrud.Delete(childRoomLogNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc
		*/

		///<summary>Get all the logs for a specified ChildRoom and filtered by the given date.</summary>
		public static List<ChildRoomLog> GetChildRoomLogs(long childRoomNum,DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoomLog>>(MethodBase.GetCurrentMethod(),childRoomNum);
			}
			string command="SELECT * FROM childroomlog WHERE ChildRoomNum="+POut.Long(childRoomNum)
				+" AND CAST(DateTDisplayed as DATE)="+POut.Date(date)
				+" ORDER BY DateTDisplayed";
			return Crud.ChildRoomLogCrud.SelectMany(command);
		}

		///<summary>Get the most recent previous ratio change for a specific child room and date. Returns 0 if no previous ratio change was found.</summary>
		public static double GetPreviousRatio(long childRoomNum,DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<double>(MethodBase.GetCurrentMethod(),childRoomNum,date);
			}
			string command="SELECT * FROM childroomlog WHERE ChildRoomNum="+POut.Long(childRoomNum)//Specify room
				+" AND RatioChange!=0"//Specify RatioChange entry
				+" AND DateTDisplayed <"+POut.Date(date)//Find previous entries
				+" ORDER BY DateTDisplayed DESC"//Order so that the first entry is the most recent one
				+" LIMIT 1";
			ChildRoomLog childRoomLog=Crud.ChildRoomLogCrud.SelectOne(command);
			if(childRoomLog==null) {
				return 0;
			}
			return childRoomLog.RatioChange;
		}

		///<summary></summary>
		public static long Insert(ChildRoomLog childRoomLog){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				childRoomLog.ChildRoomLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),childRoomLog);
				return childRoomLog.ChildRoomLogNum;
			}
			return Crud.ChildRoomLogCrud.Insert(childRoomLog);
		}

	}
}