using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ChildTeachers{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ChildTeacherCache : CacheListAbs<ChildTeacher> {
			protected override List<ChildTeacher> GetCacheFromDb() {
				string command="SELECT * FROM childteacher";
				return Crud.ChildTeacherCrud.SelectMany(command);
			}
			protected override List<ChildTeacher> TableToList(DataTable table) {
				return Crud.ChildTeacherCrud.TableToList(table);
			}
			protected override ChildTeacher Copy(ChildTeacher childTeacher) {
				return childTeacher.Copy();
			}
			protected override DataTable ListToTable(List<ChildTeacher> listChildTeachers) {
				return Crud.ChildTeacherCrud.ListToTable(listChildTeachers,"ChildTeacher");
			}
			protected override void FillCacheIfNeeded() {
				ChildTeachers.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ChildTeacherCache _childTeacherCache=new ChildTeacherCache();

		public static void ClearCache() {
			_childTeacherCache.ClearCache();
		}

		public static List<ChildTeacher> GetDeepCopy(bool isShort=false) {
			return _childTeacherCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _childTeacherCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ChildTeacher> match,bool isShort=false) {
			return _childTeacherCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ChildTeacher> match,bool isShort=false) {
			return _childTeacherCache.GetFindIndex(match,isShort);
		}

		public static ChildTeacher GetFirst(bool isShort=false) {
			return _childTeacherCache.GetFirst(isShort);
		}

		public static ChildTeacher GetFirst(Func<ChildTeacher,bool> match,bool isShort=false) {
			return _childTeacherCache.GetFirst(match,isShort);
		}

		public static ChildTeacher GetFirstOrDefault(Func<ChildTeacher,bool> match,bool isShort=false) {
			return _childTeacherCache.GetFirstOrDefault(match,isShort);
		}

		public static ChildTeacher GetLast(bool isShort=false) {
			return _childTeacherCache.GetLast(isShort);
		}

		public static ChildTeacher GetLastOrDefault(Func<ChildTeacher,bool> match,bool isShort=false) {
			return _childTeacherCache.GetLastOrDefault(match,isShort);
		}

		public static List<ChildTeacher> GetWhere(Predicate<ChildTeacher> match,bool isShort=false) {
			return _childTeacherCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_childTeacherCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_childTeacherCache.FillCacheFromTable(table);
				return table;
			}
			return _childTeacherCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<ChildTeacher> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildTeacher>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM childteacher WHERE PatNum = "+POut.Long(patNum);
			return Crud.ChildTeacherCrud.SelectMany(command);
		}
		
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ChildTeacher childTeacher){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				childTeacher.ChildTeacherNum=Meth.GetLong(MethodBase.GetCurrentMethod(),childTeacher);
				return childTeacher.ChildTeacherNum;
			}
			return Crud.ChildTeacherCrud.Insert(childTeacher);
		}
		///<summary></summary>
		public static void Update(ChildTeacher childTeacher){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childTeacher);
				return;
			}
			Crud.ChildTeacherCrud.Update(childTeacher);
		}
		///<summary></summary>
		public static void Delete(long childTeacherNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childTeacherNum);
				return;
			}
			Crud.ChildTeacherCrud.Delete(childTeacherNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc
		*/

		///<summary>Gets one ChildTeacher from the db.</summary>
		public static ChildTeacher GetOne(long childTeacherNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ChildTeacher>(MethodBase.GetCurrentMethod(),childTeacherNum);
			}
			return Crud.ChildTeacherCrud.SelectOne(childTeacherNum);
		}

		public static List<ChildTeacher> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildTeacher>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM childteacher";
			return Crud.ChildTeacherCrud.SelectMany(command);
		}


	}
}