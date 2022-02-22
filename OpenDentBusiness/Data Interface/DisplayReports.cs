using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class DisplayReports{
		#region CachePattern

		private class DisplayReportCache:CacheListAbs<DisplayReport> {
			protected override DisplayReport Copy(DisplayReport displayReport) {
				return displayReport.Copy();
			}

			protected override void FillCacheIfNeeded() {
				DisplayReports.GetTableFromCache(false);
			}

			protected override List<DisplayReport> GetCacheFromDb() {
				string command="SELECT * FROM displayreport ORDER BY ItemOrder";
				return Crud.DisplayReportCrud.SelectMany(command);
			}

			protected override DataTable ListToTable(List<DisplayReport> listDisplayReports) {
				return Crud.DisplayReportCrud.ListToTable(listDisplayReports,"DisplayReport");
			}

			protected override List<DisplayReport> TableToList(DataTable table) {
				return Crud.DisplayReportCrud.TableToList(table);
			}

			protected override bool IsInListShort(DisplayReport displayReport) {
				return  !displayReport.IsHidden;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static DisplayReportCache _displayReportCache=new DisplayReportCache();

		public static List<DisplayReport> GetDeepCopy(bool isShort=false) {
			return _displayReportCache.GetDeepCopy(isShort);
		}

		public static List<DisplayReport> GetWhere(Predicate<DisplayReport> match,bool isShort=false) {
			return _displayReportCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_displayReportCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_displayReportCache.FillCacheFromTable(table);
				return table;
			}
			return _displayReportCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Returns all reports that should show in the main menu bar.  Ordered by Description.</summary>
		public static List<DisplayReport> GetSubMenuReports() {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.IsVisibleInSubMenu,true).OrderBy(x => x.Description).ToList();
		}

		///<summary>Get all display reports for the passed-in category.  Pass in true to retrieve hidden display reports.</summary>
		public static List<DisplayReport> GetForCategory(DisplayReportCategory category, bool showHidden) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.Category==category,!showHidden);
		}

		///<summary>Pass in true to also retrieve hidden display reports.</summary>
		public static List<DisplayReport> GetAll(bool showHidden) {
			//No need to check RemotingRole; no call to db.
			return GetDeepCopy(!showHidden);
		}

		///<summary>Use DisplayReports.ReportNames to pass this method an internal report name. Returns null if no matches are found.
		///Returns first result if multiple results are found (this shouldn't happen).</summary>
		public static DisplayReport GetByInternalName(string reportName) {
			//No need to check RemotingRole; no call to db.
			List<DisplayReport> listDisplayReports=GetWhere(x => x.InternalName==reportName);
			if(listDisplayReports.IsNullOrEmpty()) {
				return null;
			}
			return listDisplayReports[0];
		}

		///<summary>Gets all DisplayReports that are known to HQ (InternalName set) without using the local cache.</summary>
		public static List<DisplayReport> GetDisplayReportsNoCache() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DisplayReport>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM displayreport WHERE InternalName!=''";
			return Crud.DisplayReportCrud.SelectMany(command);

		}

		///<summary>Must pass in a list of all current display reports, even hidden ones.</summary>
		public static bool Sync(List<DisplayReport> listDisplayReport) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listDisplayReport);
			}
			return Crud.DisplayReportCrud.Sync(listDisplayReport,GetAll(true));//TODO: cache?
		}

		public class ReportNames {
			public const string UnfinalizedInsPay="ODUnfinalizedInsPay";
			public const string PatPortionUncollected="ODPatPortionUncollected";
			public const string WebSchedAppointments="ODWebSchedAppointments";
			public const string InsAging = "ODInsAging";
			public const string CustomAging = "ODCustomAging";
			public const string OutstandingInsClaims="ODOutstandingInsClaims";
			public const string ClaimsNotSent="ODClaimsNotSent";
			public const string TreatmentFinder="ODTreatmentFinder";
			public const string ReferredProcTracking="ODReferredProcTracking";
			public const string IncompleteProcNotes="ODIncompleteProcNotes";
			public const string ProcNotBilledIns="ODProcsNotBilled";
			///<summary>Production and Income More Options</summary>
			public const string ODMoreOptions="ODMoreOptions";
			public const string ODProcOverpaid="ODProcOverpaid";
			public const string DPPOvercharged="ODDynamicPayPlanOvercharged";
			public const string MonthlyProductionGoal="ODMonthlyProductionGoal";
			public const string EraAutoProcessed="ODEraAutoProcessed";
		}
	}
}