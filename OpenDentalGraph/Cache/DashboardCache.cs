using System;
using System.Collections.Generic;
using System.Linq;
using OpenDentBusiness;
using System.Data;
using CodeBase;

namespace OpenDentalGraph.Cache {
	public class DashboardCache {
		private static Random _rand=new Random();
		private static bool _useProvFilter=false;
		private static long _provNum=0;

		///<summary>Register for this event once per app instance IF and only IF you want to provide a different db context for your cache threads.
		///This will typically be unnecessary if the app itself already has a db context. This will typically only be used by BroadcastMonitor.</summary>
		public static EventHandler OnSetDb;

		public static bool UseProvFilter {
			get { return _useProvFilter; }
			set { _useProvFilter=value; }
		}

		public static long ProvNum
		{
			get { return _provNum; }
			set { _provNum=value; }
		}

		#region Private Caches
		private static DashboardCacheNewPatient _patients=new DashboardCacheNewPatient();
		private static DashboardCacheCompletedProc _completedProcs=new DashboardCacheCompletedProc();
		private static DashboardCacheWriteoff _writeoffs=new DashboardCacheWriteoff();
		private static DashboardCacheAdjustment _adjustments=new DashboardCacheAdjustment();
		private static DashboardCachePaySplit _paySplits=new DashboardCachePaySplit();
		private static DashboardCacheClaimPayment _claimPayments=new DashboardCacheClaimPayment();
		private static DashboardCacheAR _aR=new DashboardCacheAR();
		private static DashboardCacheProvider _providers=new DashboardCacheProvider();
		private static DashboardCacheBrokenAppt _brokenAppts=new DashboardCacheBrokenAppt();
		private static DashboardCacheBrokenProcedure _brokenProcs=new DashboardCacheBrokenProcedure();
		private static DashboardCacheBrokenAdj _brokenAdjs=new DashboardCacheBrokenAdj();

		private static DashboardCacheClinic _clinics=new DashboardCacheClinic();
		#endregion

		#region Public Caches
		public static DashboardCacheNewPatient Patients {
			get { return _patients; }
		}
		public static DashboardCacheCompletedProc CompletedProcs {
			get { return _completedProcs; }
		}
		public static DashboardCacheWriteoff Writeoffs {
			get { return _writeoffs; }
		}
		public static DashboardCacheAdjustment Adjustments {
			get { return _adjustments; }
		}
		public static DashboardCachePaySplit PaySplits {
			get { return _paySplits; }
		}
		public static DashboardCacheClaimPayment ClaimPayments {
			get { return _claimPayments; }
		}
		public static DashboardCacheAR AR {
			get { return _aR; }
		}
		public static DashboardCacheProvider Providers {
			get { return _providers; }
		}
		public static DashboardCacheBrokenAppt BrokenAppts {
			get { return _brokenAppts; }
		}
		public static DashboardCacheBrokenProcedure BrokenProcs{
			get { return _brokenProcs; }
		}
		public static DashboardCacheBrokenAdj BrokenAdjs
		{
			get { return _brokenAdjs; }
		}
		public static DashboardCacheClinic Clinics {
			get { return _clinics; }
		}
		#endregion	

		///<summary>Refresh the cache for all cells associated with all layouts given. If waitToReturn==true then block until finished.
		///If waitToReturn==false then runs async and returns immediately. Optionally wait for the method to return or run async.
		///onExit event will be fired after async version has completed. Will not fire when sync version is run as this is a blocking call so when it returns it is done.
		///If invalidateFirst==true then cache will be invalidated and forcefully refreshed. Use this sparingly.</summary>
		public static void RefreshLayoutsIfInvalid(List<DashboardLayout> layouts,bool waitToReturn,bool invalidateFirst,EventHandler onExit=null) {
			List<DashboardCellType> cellTypes=Enum.GetValues(typeof(DashboardCellType)).Cast<DashboardCellType>().ToList();
			foreach(DashboardCellType cellType in cellTypes) {
				List<DashboardFilter> filters=layouts
					.SelectMany(x => x.Cells)
					.Where(x => x.CellType==cellType)
					.Select(x => ODGraphSettingsBase.Deserialize(ODGraphJson.Deserialize(x.CellSettings).GraphJson).Filter)
					.ToList();
				if(filters.Count<=0) {
					continue;
				}
				RefreshCellTypeIfInvalid(
					cellType,
					new DashboardFilter() { DateFrom=filters.Min(x => x.DateFrom),DateTo=filters.Max(x => x.DateTo),UseDateFilter=filters.All(x => x.UseDateFilter) },
					waitToReturn,
					invalidateFirst,
					onExit);
			}
		}

		///<summary>Refresh the cache(s) associated with the given cellType. If waitToReturn==true then block until finished.
		///If waitToReturn==false then runs async and returns immediately. Optionally wait for the method to return or run async.
		///onExit event will be fired after async version has completed. Will not fire when sync version is run as this is a blocking call so when it returns it is done.
		///If invalidateFirst==true then cache will be invalidated and forcefully refreshed. Use this sparingly.</summary>
		public static void RefreshCellTypeIfInvalid(DashboardCellType cellType,DashboardFilter filter,bool waitToReturn,bool invalidateFirst,EventHandler onExit = null) {
			//Create a random group name so we can arbitrarily group and wait on the threads we are about to start.
			string groupName=cellType.ToString()+_rand.Next();
			try {
				switch(cellType) {
					case DashboardCellType.HQPhone:
					case DashboardCellType.HQMtMessage:
					case DashboardCellType.HQBillingUsageAccess:
					case DashboardCellType.HQMoMessage:
					case DashboardCellType.HQBillingInboundOutbound:
					case DashboardCellType.HQSignups:
						//Broadcast monitor does not use the ODGraph cache.
						return;
				}
				//Always fill certain caches first. These will not be threaded as they need to be available to the threads which will run below.
				//It doesn't hurt to block momentarily here as the queries will run very quickly.			
				Providers.Run(new DashboardFilter() { UseDateFilter=false },invalidateFirst);
				Clinics.Run(new DashboardFilter() { UseDateFilter=false },invalidateFirst);
				//Start certain cache threads depending on which cellType we are interested in. Each cache will have its own thread.
				switch(cellType) {
					case DashboardCellType.ProductionGraph:
						FillCacheThreaded(CompletedProcs,filter,groupName,invalidateFirst);
						FillCacheThreaded(Writeoffs,filter,groupName,invalidateFirst);
						FillCacheThreaded(Adjustments,filter,groupName,invalidateFirst);
						break;
					case DashboardCellType.IncomeGraph:
						FillCacheThreaded(PaySplits,filter,groupName,invalidateFirst);
						FillCacheThreaded(ClaimPayments,filter,groupName,invalidateFirst);
						break;
					case DashboardCellType.AccountsReceivableGraph:
						FillCacheThreaded(AR,filter,groupName,invalidateFirst);
						break;
					case DashboardCellType.NewPatientsGraph:
						FillCacheThreaded(Patients,filter,groupName,invalidateFirst);
						break;
					case DashboardCellType.BrokenApptGraph:
						FillCacheThreaded(BrokenAppts,filter,groupName,invalidateFirst);
						FillCacheThreaded(BrokenProcs,filter,groupName,invalidateFirst);
						FillCacheThreaded(BrokenAdjs,filter,groupName,invalidateFirst);
						break;
					case DashboardCellType.NotDefined:
					default:
						throw new Exception("Unsupported DashboardCellType: "+cellType.ToString());
				}				
			}
			finally {
				if(waitToReturn) { //Block until all threads have completed.
					ODThread.JoinThreadsByGroupName(System.Threading.Timeout.Infinite,groupName,true);
				}
				else if(onExit!=null) { //Exit immediately but fire event later once all threads have completed.
					ODThread.AddGroupNameExitHandler(groupName,onExit);
				}
			}
		}

		///<summary>Start a thread dedicated to filling the given cache.</summary>
		private static void FillCacheThreaded<T>(DashboardCacheBase<T> cache,DashboardFilter filter,string groupName,bool invalidateFirst) {
			ODThread thread=new ODThread(new ODThread.WorkerDelegate((ODThread th) => {
				if(OnSetDb!=null) {
					OnSetDb(cache,new EventArgs());
				}
				cache.Run(filter,invalidateFirst);
			}));
			thread.GroupName=groupName;
			thread.Start(false);
		}
	}

	///<summary>Base class for implementing Graph cache.</summary>
	public abstract class DashboardCacheBase<T> {
		///<summary>Creates exactly 1 static T instance per type of T provided in the entire app. 
		///For example all instances of DataSetCache&lt;string&gt; will share one single List&lt;string&gt;.
		///http://stackoverflow.com/a/9647661.</summary>
		private static List<T> _cacheS;
		///<summary>Creates 1 list per instance. This will be a copy of the cache for this type (T).</summary>
		private List<T> _cache=new List<T>();
		///<summary>Gets the instance cache. This will be a copy of the cache for this type (T).</summary>
		public List<T> Cache { get { return _cache; } }
		///<summary>Concrete implementers must override this and provide the cache given the input criteria.</summary>
		protected abstract List<T> GetCache(DashboardFilter filter);
		///<summary>Give implementers one last chance to remove any bad data from the cache before it is stored.</summary>
		protected virtual void RemoveBadData(List<T> rawFromQuery) { }

		///<summary>True by default. Override this if you want to disallow date filtering in your query.</summary>
		protected virtual bool AllowQueryDateFilter() {
			return true;
		}
		
		///<summary>Fills the instance cache and the static cache (when necessary) using the given criteria. This IS thread-safe.
		///If invalidateFirst==true then cache will be invalidated and forcefully refreshed. Use this sparingly.</summary>
		public void Run(DashboardFilter filter,bool invalidateFirst) {
			lock (DashboardCacheLock<T>.Lock) {
				if(invalidateFirst) { //This will cause a force refresh.
					_cacheS=null;
				}
				if(!AllowQueryDateFilter()) {
					DashboardCacheLock<T>.FilterOptions.UseDateFilter=false;
				}
				DateTime dateFromOut; DateTime dateToOut; bool useProvFilter; long provNum;
				if(IsInvalid(filter,out dateFromOut,out dateToOut,out useProvFilter,out provNum)) { //The cache is invalid and should be filled using the new filter options.
					DashboardCacheLock<T>.FilterOptions.DateFrom=dateFromOut;
					DashboardCacheLock<T>.FilterOptions.DateTo=dateToOut;
					DashboardCacheLock<T>.FilterOptions.UseProvFilter=useProvFilter;
					DashboardCacheLock<T>.FilterOptions.ProvNum=provNum;					
					if(!filter.UseDateFilter&&DashboardCacheLock<T>.FilterOptions.UseDateFilter) {
						//Previously we used a date filter but now we are not. Set the flag indicating that we have retrieved the unfiltered cache.
						DashboardCacheLock<T>.FilterOptions.UseDateFilter=false;
					}
					//This is potentially a slow query so set the static cache here.
					_cacheS=GetCache(DashboardCacheLock<T>.FilterOptions);
					RemoveBadData(_cacheS);
					//This is a simple shallow copy so it is safe enough to set _cache inside of this lock. It makes _cache available even when _cacheS is unavailable due to slow query.
					_cache=new List<T>(_cacheS);
				}
			}
		}

		///<summary>Check static CacheLock fields and verify that they are still valid given the new filter criteria.
		///Returns true if cache needs to be refreshed. Otherwise returns false.</summary>
		private bool IsInvalid(DashboardFilter filterIn,out DateTime dateFromOut,out DateTime dateToOut,out bool useProvFilter,out long provNum) {
			dateFromOut=DashboardCacheLock<T>.FilterOptions.DateFrom;
			dateToOut=DashboardCacheLock<T>.FilterOptions.DateTo;
			useProvFilter=DashboardCacheLock<T>.FilterOptions.UseProvFilter;
			provNum=DashboardCacheLock<T>.FilterOptions.ProvNum;
			lock (DashboardCacheLock<T>.Lock) {
				if(_cacheS==null) { //Hasn't run yet so it is invalid.
					dateFromOut=filterIn.DateFrom;
					dateToOut=filterIn.DateTo;
					useProvFilter=filterIn.UseProvFilter;
					provNum=filterIn.ProvNum;
					return true;
				}
				if(DashboardCacheLock<T>.FilterOptions.UseProvFilter!=filterIn.UseProvFilter) {
					useProvFilter=filterIn.UseProvFilter;
					provNum=filterIn.ProvNum;
					return false;
				}
				if(DashboardCacheLock<T>.FilterOptions.ProvNum!=filterIn.ProvNum) {
					useProvFilter=filterIn.UseProvFilter;
					provNum=filterIn.ProvNum;
					return false;
				}
				if(!DashboardCacheLock<T>.FilterOptions.UseDateFilter) { 
					//Date filter has previously been turned off, which means we have already gotten the untiltered cache at least once.
					//There will be nothing else to get so cache is valid.
					return false;
				}
				bool ret=false;
				if(filterIn.DateFrom<DashboardCacheLock<T>.FilterOptions.DateFrom) { //New 'from' is before old 'from', invalidate.
					dateFromOut=filterIn.DateFrom;
					ret=true;
				}
				if(filterIn.DateTo>DashboardCacheLock<T>.FilterOptions.DateTo) { //New 'to' is after old 'to', invalidate.
					dateToOut=filterIn.DateTo;
					ret=true;
				}
				return ret;
			}
		}
	}

	///<summary>Base class for implementing Graph cache which gets it's cache from a simple db query. Allows outside caller to run the query without using DashboardCache framework.</summary>
	public abstract class DashboardCacheWithQueryUnprotected<T>:DashboardCacheWithQuery<T> where T : GraphQuantityOverTime.GraphPointBase {
		public virtual List<T> GetCacheUnthreaded(DashboardFilter filter) {
			return GetCache(filter);
		}
	}

	///<summary>Base class for implementing Graph cache which gets it's cache from a simple db query.</summary>
	public abstract class DashboardCacheWithQuery<T>:DashboardCacheBase<T> where T : GraphQuantityOverTime.GraphPointBase {
		protected virtual bool UseReportServer { get { return true; } }
		protected abstract string GetCommand(DashboardFilter filter);
		protected abstract T GetInstanceFromDataRow(DataRow x);

		protected override List<T> GetCache(DashboardFilter filter) {
			if(filter.UseProvFilter && filter.ProvNum==0) {
				return new List<T>();
			}
			return DashboardQueries.GetTable(GetCommand(filter),UseReportServer)
				.AsEnumerable()
				.Select(x => GetInstanceFromDataRow(x))
				.ToList();
		}

		protected override void RemoveBadData(List<T> rawFromQuery) {
			rawFromQuery.RemoveAll(x => x.DateStamp.Year<1880);
		}
	}

	///<summary>Allows lock arguments to be "typed" per T type provided. Must be locked using CacheLock&lt;T&gt;.Lock when accessing any static fields.
	///For example CacheLockList&lt;string&gt;.Lock is now distinct from CacheLockList&lt;int&gt;.Lock even though the Lock field itself is static. 
	///When paired with a type (T) the static fields become unqique per type (T).</summary>
	public class DashboardCacheLock<T> {
		public static object Lock=new object();
		public static DashboardFilter FilterOptions=new DashboardFilter();
	}

	///<summary>Each cache can be filtered according to these criteria.</summary>
	public class DashboardFilter {
		public DateTime DateFrom=DateTime.Now;
		public DateTime DateTo=DateTime.Now;
		public bool UseDateFilter=true;
		public long ProvNum=0;
		public bool UseProvFilter=false;

		public DashboardFilter() {
			UseProvFilter=DashboardCache.UseProvFilter;
			ProvNum=DashboardCache.ProvNum;
		}
	}
}
