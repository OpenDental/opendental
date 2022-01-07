using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AlertCategories{
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class AlertCategoryCache : CacheListAbs<AlertCategory> {
			protected override List<AlertCategory> GetCacheFromDb() {
				string command="SELECT * FROM alertcategory";
				return Crud.AlertCategoryCrud.SelectMany(command);
			}
			protected override List<AlertCategory> TableToList(DataTable table) {
				return Crud.AlertCategoryCrud.TableToList(table);
			}
			protected override AlertCategory Copy(AlertCategory alertCategory) {
				return alertCategory.Copy();
			}
			protected override DataTable ListToTable(List<AlertCategory> listAlertCategories) {
				return Crud.AlertCategoryCrud.ListToTable(listAlertCategories,"AlertCategory");
			}
			protected override void FillCacheIfNeeded() {
				AlertCategories.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AlertCategoryCache _alertCategoryCache=new AlertCategoryCache();

		public static List<AlertCategory> GetDeepCopy(bool isShort=false) {
			return _alertCategoryCache.GetDeepCopy(isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_alertCategoryCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_alertCategoryCache.FillCacheFromTable(table);
				return table;
			}
			return _alertCategoryCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		#region Get Methods
		
		///<summary>Gets one AlertCategory from the db.</summary>
		public static AlertCategory GetOne(long alertCategoryNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<AlertCategory>(MethodBase.GetCurrentMethod(),alertCategoryNum);
			}
			return Crud.AlertCategoryCrud.SelectOne(alertCategoryNum);
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(AlertCategory alertCategory){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				alertCategory.AlertCategoryNum=Meth.GetLong(MethodBase.GetCurrentMethod(),alertCategory);
				return alertCategory.AlertCategoryNum;
			}
			return Crud.AlertCategoryCrud.Insert(alertCategory);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(AlertCategory alertCategory){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertCategory);
				return;
			}
			Crud.AlertCategoryCrud.Update(alertCategory);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long alertCategoryNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertCategoryNum);
				return;
			}
			Crud.AlertCategoryCrud.Delete(alertCategoryNum);
		}
		#endregion

		#region Misc Methods
		///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
		///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new AlertCategories items.</summary>
		public static bool Sync(List<AlertCategory> listNew,List<AlertCategory> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.AlertCategoryCrud.Sync(listNew,listOld);
		}
		#endregion
	}
}