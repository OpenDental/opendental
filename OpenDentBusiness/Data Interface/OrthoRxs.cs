using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoRxs{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class OrthoRxCache : CacheListAbs<OrthoRx> {
			protected override List<OrthoRx> GetCacheFromDb() {
				string command="SELECT * FROM orthorx ORDER BY ItemOrder";
				return Crud.OrthoRxCrud.SelectMany(command);
			}
			protected override List<OrthoRx> TableToList(DataTable table) {
				return Crud.OrthoRxCrud.TableToList(table);
			}
			protected override OrthoRx Copy(OrthoRx orthoRx) {
				return orthoRx.Copy();
			}
			protected override DataTable ListToTable(List<OrthoRx> listOrthoRxs) {
				return Crud.OrthoRxCrud.ListToTable(listOrthoRxs,"OrthoRx");
			}
			protected override void FillCacheIfNeeded() {
				OrthoRxs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static OrthoRxCache _orthoRxCache=new OrthoRxCache();

		public static List<OrthoRx> GetDeepCopy(bool isShort=false) {
			return _orthoRxCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _orthoRxCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<OrthoRx> match,bool isShort=false) {
			return _orthoRxCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<OrthoRx> match,bool isShort=false) {
			return _orthoRxCache.GetFindIndex(match,isShort);
		}

		public static OrthoRx GetFirst(bool isShort=false) {
			return _orthoRxCache.GetFirst(isShort);
		}

		public static OrthoRx GetFirst(Func<OrthoRx,bool> match,bool isShort=false) {
			return _orthoRxCache.GetFirst(match,isShort);
		}

		public static OrthoRx GetFirstOrDefault(Func<OrthoRx,bool> match,bool isShort=false) {
			return _orthoRxCache.GetFirstOrDefault(match,isShort);
		}

		public static OrthoRx GetLast(bool isShort=false) {
			return _orthoRxCache.GetLast(isShort);
		}

		public static OrthoRx GetLastOrDefault(Func<OrthoRx,bool> match,bool isShort=false) {
			return _orthoRxCache.GetLastOrDefault(match,isShort);
		}

		public static List<OrthoRx> GetWhere(Predicate<OrthoRx> match,bool isShort=false) {
			return _orthoRxCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_orthoRxCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_orthoRxCache.FillCacheFromTable(table);
				return table;
			}
			return _orthoRxCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		
		/*
		#region Methods - Get
		///<summary></summary>
		public static List<OrthoRx> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<OrthoRx>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthorx WHERE PatNum = "+POut.Long(patNum);
			return Crud.OrthoRxCrud.SelectMany(command);
		}
		
		///<summary>Gets one OrthoRx from the db.</summary>
		public static OrthoRx GetOne(long orthoRxNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<OrthoRx>(MethodBase.GetCurrentMethod(),orthoRxNum);
			}
			return Crud.OrthoRxCrud.SelectOne(orthoRxNum);
		}
		#endregion Methods - Get*/
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(OrthoRx orthoRx){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				orthoRx.OrthoRxNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoRx);
				return orthoRx.OrthoRxNum;
			}
			return Crud.OrthoRxCrud.Insert(orthoRx);
		}
		///<summary></summary>
		public static void Update(OrthoRx orthoRx){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoRx);
				return;
			}
			Crud.OrthoRxCrud.Update(orthoRx);
		}
		///<summary></summary>
		public static void Delete(long orthoRxNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoRxNum);
				return;
			}
			Crud.OrthoRxCrud.Delete(orthoRxNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc
		



	}
}