using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class APIKeys{

		///<summary>Saves the list of APIKeys to the database. Deletes all existing ones first.</summary>
		public static void SaveListAPIKeys(List<APIKey> listAPIKeys) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAPIKeys);
				return;
			}
			string command="DELETE FROM apikey";
			Db.NonQ(command);
			for(int i=0;i<listAPIKeys.Count;i++) {
				Insert(listAPIKeys[i]);
			}
		}

		///<summary>Inserts an APIKey into database. </summary>
		public static long Insert(APIKey aPIKey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				aPIKey.APIKeyNum=Meth.GetLong(MethodBase.GetCurrentMethod(),aPIKey);
				return aPIKey.APIKeyNum;
			}
			return Crud.APIKeyCrud.Insert(aPIKey);
		}

		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class APIKeyCache : CacheListAbs<APIKey> {
			protected override List<APIKey> GetCacheFromDb() {
				string command="SELECT * FROM apikey";
				return Crud.APIKeyCrud.SelectMany(command);
			}
			protected override List<APIKey> TableToList(DataTable table) {
				return Crud.APIKeyCrud.TableToList(table);
			}
			protected override APIKey Copy(APIKey aPIKey) {
				return aPIKey.Copy();
			}
			protected override DataTable ListToTable(List<APIKey> listAPIKeys) {
				return Crud.APIKeyCrud.ListToTable(listAPIKeys,"APIKey");
			}
			protected override void FillCacheIfNeeded() {
				APIKeys.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static APIKeyCache _aPIKeyCache=new APIKeyCache();

		public static List<APIKey> GetDeepCopy(bool isShort=false) {
			return _aPIKeyCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _aPIKeyCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<APIKey> match,bool isShort=false) {
			return _aPIKeyCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<APIKey> match,bool isShort=false) {
			return _aPIKeyCache.GetFindIndex(match,isShort);
		}

		public static APIKey GetFirst(bool isShort=false) {
			return _aPIKeyCache.GetFirst(isShort);
		}

		public static APIKey GetFirst(Func<APIKey,bool> match,bool isShort=false) {
			return _aPIKeyCache.GetFirst(match,isShort);
		}

		public static APIKey GetFirstOrDefault(Func<APIKey,bool> match,bool isShort=false) {
			return _aPIKeyCache.GetFirstOrDefault(match,isShort);
		}

		public static APIKey GetLast(bool isShort=false) {
			return _aPIKeyCache.GetLast(isShort);
		}

		public static APIKey GetLastOrDefault(Func<APIKey,bool> match,bool isShort=false) {
			return _aPIKeyCache.GetLastOrDefault(match,isShort);
		}

		public static List<APIKey> GetWhere(Predicate<APIKey> match,bool isShort=false) {
			return _aPIKeyCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_aPIKeyCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_aPIKeyCache.FillCacheFromTable(table);
				return table;
			}
			return _aPIKeyCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<APIKey> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<APIKey>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM apikey WHERE PatNum = "+POut.Long(patNum);
			return Crud.APIKeyCrud.SelectMany(command);
		}
		
		///<summary>Gets one APIKey from the db.</summary>
		public static APIKey GetOne(long aPIKeyNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<APIKey>(MethodBase.GetCurrentMethod(),aPIKeyNum);
			}
			return Crud.APIKeyCrud.SelectOne(aPIKeyNum);
		}
		#endregion Methods - Get
		*/
		#region Methods - Modify


		/*
		///<summary></summary>
		public static void Update(APIKey aPIKey){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aPIKey);
				return;
			}
			Crud.APIKeyCrud.Update(aPIKey);
		}
		///<summary></summary>
		public static void Delete(long aPIKeyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aPIKeyNum);
				return;
			}
			Crud.APIKeyCrud.Delete(aPIKeyNum);
		}
		*/
		#endregion Methods - Modify

		/*
		#region Methods - Misc
		

		
		#endregion Methods - Misc
		*/



	}
}