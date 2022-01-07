using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AlertCategoryLinks{
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class AlertCategoryLinkCache : CacheListAbs<AlertCategoryLink> {
			protected override List<AlertCategoryLink> GetCacheFromDb() {
				string command="SELECT * FROM alertcategorylink";
				return Crud.AlertCategoryLinkCrud.SelectMany(command);
			}
			protected override List<AlertCategoryLink> TableToList(DataTable table) {
				return Crud.AlertCategoryLinkCrud.TableToList(table);
			}
			protected override AlertCategoryLink Copy(AlertCategoryLink alertCategoryLink) {
				return alertCategoryLink.Copy();
			}
			protected override DataTable ListToTable(List<AlertCategoryLink> listAlertCategoryLinks) {
				return Crud.AlertCategoryLinkCrud.ListToTable(listAlertCategoryLinks,"AlertCategoryLink");
			}
			protected override void FillCacheIfNeeded() {
				AlertCategoryLinks.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AlertCategoryLinkCache _alertCategoryLinkCache=new AlertCategoryLinkCache();

		public static List<AlertCategoryLink> GetWhere(Predicate<AlertCategoryLink> match,bool isShort=false) {
			return _alertCategoryLinkCache.GetWhere(match,isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_alertCategoryLinkCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_alertCategoryLinkCache.FillCacheFromTable(table);
				return table;
			}
			return _alertCategoryLinkCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		#region Get Methods
		
		///<summary>Gets one AlertCategoryLink from the db.</summary>
		public static AlertCategoryLink GetOne(long alertCategoryLinkNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<AlertCategoryLink>(MethodBase.GetCurrentMethod(),alertCategoryLinkNum);
			}
			return Crud.AlertCategoryLinkCrud.SelectOne(alertCategoryLinkNum);
		}

		public static List<AlertCategoryLink> GetForCategory(long alertCategoryNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AlertCategoryLink>>(MethodBase.GetCurrentMethod(),alertCategoryNum);
			}
			if(alertCategoryNum==0) {
				return new List<AlertCategoryLink>();
			}
			string command="SELECT * FROM alertcategorylink WHERE AlertCategoryNum = "+POut.Long(alertCategoryNum);
			return Crud.AlertCategoryLinkCrud.SelectMany(command);
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(AlertCategoryLink alertCategoryLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				alertCategoryLink.AlertCategoryLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),alertCategoryLink);
				return alertCategoryLink.AlertCategoryLinkNum;
			}
			return Crud.AlertCategoryLinkCrud.Insert(alertCategoryLink);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(AlertCategoryLink alertCategoryLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertCategoryLink);
				return;
			}
			Crud.AlertCategoryLinkCrud.Update(alertCategoryLink);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long alertCategoryLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertCategoryLinkNum);
				return;
			}
			Crud.AlertCategoryLinkCrud.Delete(alertCategoryLinkNum);
		}

		public static void DeleteForCategory(long alertCategoryNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertCategoryNum);
				return;
			}
			if(alertCategoryNum==0) {
				return;
			}
			string command="DELETE FROM alertcategorylink "
				+"WHERE AlertCategoryNum = "+POut.Long(alertCategoryNum);
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods
		
		///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
		///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new AlertCategoryLink items.</summary>
		public static bool Sync(List<AlertCategoryLink> listNew,List<AlertCategoryLink> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.AlertCategoryLinkCrud.Sync(listNew,listOld);
		}
		#endregion
	}
}