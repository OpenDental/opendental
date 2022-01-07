using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary>Handles database commands related to the apptview table in the database.</summary>
	public class ApptViews {
		public const long APPTVIEWNUM_NONE=0;
		#region Get Methods
		///<summary>Optionally pass in a clinic to filter the list of appointment views returned.</summary>
		public static List<ApptView> GetForClinic(long clinicNum=0,bool isShort=true) {
			//No need to check RemotingRole; no call to db.
			if(clinicNum > 0) {
				return GetWhere(x => x.ClinicNum==clinicNum,isShort);
			}
			else {
				return GetDeepCopy(isShort);
			}
		}

		///<summary>Gets an ApptView from the cache.  If apptviewnum is not valid, then it returns null.</summary>
		public static ApptView GetApptView(long apptViewNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ApptViewNum==apptViewNum);
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(ApptView apptView) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				apptView.ApptViewNum=Meth.GetLong(MethodBase.GetCurrentMethod(),apptView);
				return apptView.ApptViewNum;
			}
			return Crud.ApptViewCrud.Insert(apptView);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(ApptView apptView){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptView);
				return;
			}
			Crud.ApptViewCrud.Update(apptView);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(ApptView Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command="DELETE FROM apptview WHERE ApptViewNum = '"
				+POut.Long(Cur.ApptViewNum)+"'";
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods
		///<summary>Determines if the ApptView is the "None" view.  Null is considered to be the "None" view for compatibility with other methods that
		///expect the "None" view to be a null ApptView.</summary>
		public static bool IsNoneView(ApptView view) {
			//No need to check RemotingRole; no call to db.
			return view==null || view.ApptViewNum==APPTVIEWNUM_NONE;
		}
		#endregion

		#region Cache Pattern
		private class ApptViewCache : CacheListAbs<ApptView> {
			protected override List<ApptView> GetCacheFromDb() {
				string command="SELECT * FROM apptview ORDER BY ClinicNum,ItemOrder";
				return Crud.ApptViewCrud.SelectMany(command);
			}
			protected override List<ApptView> TableToList(DataTable table) {
				return Crud.ApptViewCrud.TableToList(table);
			}
			protected override ApptView Copy(ApptView apptView) {
				return apptView.Copy();
			}
			protected override DataTable ListToTable(List<ApptView> listApptViews) {
				return Crud.ApptViewCrud.ListToTable(listApptViews,"ApptView");
			}
			protected override void FillCacheIfNeeded() {
				ApptViews.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ApptViewCache _apptViewCache=new ApptViewCache();

		public static List<ApptView> GetDeepCopy(bool isShort=false) {
			return _apptViewCache.GetDeepCopy(isShort);
		}

		private static ApptView GetFirstOrDefault(Func<ApptView,bool> match,bool isShort=false) {
			return _apptViewCache.GetFirstOrDefault(match,isShort);
		}

		public static List<ApptView> GetWhere(Predicate<ApptView> match,bool isShort=false) {
			return _apptViewCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_apptViewCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_apptViewCache.FillCacheFromTable(table);
				return table;
			}
			return _apptViewCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern
	}

}









