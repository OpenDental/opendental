using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AlertReads{
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class AlertReadCache : CacheListAbs<AlertRead> {
			protected override List<AlertRead> GetCacheFromDb() {
				string command="SELECT * FROM AlertRead ORDER BY ItemOrder";
				return Crud.AlertReadCrud.SelectMany(command);
			}
			protected override List<AlertRead> TableToList(DataTable table) {
				return Crud.AlertReadCrud.TableToList(table);
			}
			protected override AlertRead Copy(AlertRead AlertRead) {
				return AlertRead.Clone();
			}
			protected override DataTable ListToTable(List<AlertRead> listAlertReads) {
				return Crud.AlertReadCrud.ListToTable(listAlertReads,"AlertRead");
			}
			protected override void FillCacheIfNeeded() {
				AlertReads.GetTableFromCache(false);
			}
			protected override bool IsInListShort(AlertRead AlertRead) {
				return !AlertRead.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AlertReadCache _AlertReadCache=new AlertReadCache();

		///<summary>A list of all AlertReads. Returns a deep copy.</summary>
		public static List<AlertRead> ListDeep {
			get {
				return _AlertReadCache.ListDeep;
			}
		}

		///<summary>A list of all visible AlertReads. Returns a deep copy.</summary>
		public static List<AlertRead> ListShortDeep {
			get {
				return _AlertReadCache.ListShortDeep;
			}
		}

		///<summary>A list of all AlertReads. Returns a shallow copy.</summary>
		public static List<AlertRead> ListShallow {
			get {
				return _AlertReadCache.ListShallow;
			}
		}

		///<summary>A list of all visible AlertReads. Returns a shallow copy.</summary>
		public static List<AlertRead> ListShort {
			get {
				return _AlertReadCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_AlertReadCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_AlertReadCache.FillCacheFromTable(table);
				return table;
			}
			return _AlertReadCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary></summary>
		public static List<AlertRead> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AlertRead>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM alertread WHERE UserNum = "+POut.Long(patNum);
			return Crud.AlertReadCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<AlertRead> RefreshForAlertNums(long patNum,List<long> listAlertItemNums){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AlertRead>>(MethodBase.GetCurrentMethod(),patNum,listAlertItemNums);
			}
			if(listAlertItemNums==null || listAlertItemNums.Count==0) {
				return new List<AlertRead>();
			}
			string command="SELECT * FROM alertread WHERE UserNum = "+POut.Long(patNum)+ " ";
			command+="AND  AlertItemNum IN ("+String.Join(",",listAlertItemNums)+")";
			return Crud.AlertReadCrud.SelectMany(command);
		}

		///<summary>Gets one AlertRead from the db.</summary>
		public static AlertRead GetOne(long alertReadNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<AlertRead>(MethodBase.GetCurrentMethod(),alertReadNum);
			}
			return Crud.AlertReadCrud.SelectOne(alertReadNum);
		}

		///<summary></summary>
		public static long Insert(AlertRead alertRead){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				alertRead.AlertReadNum=Meth.GetLong(MethodBase.GetCurrentMethod(),alertRead);
				return alertRead.AlertReadNum;
			}
			return Crud.AlertReadCrud.Insert(alertRead);
		}

		///<summary></summary>
		public static void Update(AlertRead alertRead){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertRead);
				return;
			}
			Crud.AlertReadCrud.Update(alertRead);
		}

		///<summary></summary>
		public static void Delete(long alertReadNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertReadNum);
				return;
			}
			Crud.AlertReadCrud.Delete(alertReadNum);
		}

		///<summary></summary>
		public static void DeleteForAlertItem(long alertItemNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),alertItemNum);
				return;
			}
			string command="DELETE FROM alertread "
				+"WHERE AlertItemNum = "+POut.Long(alertItemNum);
			Db.NonQ(command);
		}

		///<summary>Deletes all alertreads for the listAlertItemNums.  Used by the OpenDentalService AlertRadiologyProceduresThread.</summary>
		public static void DeleteForAlertItems(List<long> listAlertItemNums) {
			if(listAlertItemNums==null || listAlertItemNums.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAlertItemNums);
				return;
			}
			string command="DELETE FROM alertread "
				+"WHERE AlertItemNum IN ("+string.Join(",",listAlertItemNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
		///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new Apptcomm items.</summary>
		public static bool Sync(List<AlertRead> listNew,List<AlertRead> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.AlertReadCrud.Sync(listNew,listOld);
		}
	}
}