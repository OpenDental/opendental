using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OpenDentalGraph.Cache;
using OpenDentalGraph.Enumerations;
using System.Drawing;
using CodeBase;

namespace OpenDentalGraph {
	public partial class GraphQuantityOverTimeFilter : UserControl, IODGraphPrinter, IDashboardDockContainer {
		#region Private Data
		private DashboardCellType _cellType=DashboardCellType.NotDefined;
		private IncomeGraphOptionsCtrl _incomeOptionsCtrl=new IncomeGraphOptionsCtrl();
		private ProductionGraphOptionsCtrl _productionOptionsCtrl=new ProductionGraphOptionsCtrl();
		private BrokenApptGraphOptionsCtrl _brokenApptsCtrl=new BrokenApptGraphOptionsCtrl();
		private HqMessagesRealTimeOptionsCtrl _hqMsgRealTimeCtrl=new HqMessagesRealTimeOptionsCtrl();
		#endregion

		#region Delegates
		public delegate List<GraphQuantityOverTime.GraphPointBase> GetGraphPointsForHQArgs(GraphQuantityOverTimeFilter filterCtrl);
		public GetGraphPointsForHQArgs _onGetODGraphPointsArgs;
		///<summary>Allows override of default behavior of graph_OnGetGetColor(). Must be set by constructor.</summary>
		public GraphQuantityOverTime.OnGetColorFromSeriesGraphTypeArgs _onGetSeriesColorOverride;
		#endregion

		#region Properties
		public bool CanEdit {
			get {
				switch(CellType) {
					case DashboardCellType.ProductionGraph:
					case DashboardCellType.IncomeGraph:
					case DashboardCellType.NewPatientsGraph:
					case DashboardCellType.BrokenApptGraph:
					case DashboardCellType.AccountsReceivableGraph:
					case DashboardCellType.HQMtMessage:
					case DashboardCellType.HQBillingUsageAccess:
					case DashboardCellType.HQSignups:
					case DashboardCellType.HQPhone:
					case DashboardCellType.HQMoMessage:
					case DashboardCellType.HQBillingInboundOutbound:
						return true;
					default:
						return false;
				}
			}
		}
		[Category("Graph")]
		public bool ShowFilters {
			get { return graph.ShowFilters; }
			set {
				graph.ShowFilters=value;
				switch(CellType) {
					case DashboardCellType.ProductionGraph:
					case DashboardCellType.IncomeGraph:
					case DashboardCellType.BrokenApptGraph:
					case DashboardCellType.NewPatientsGraph:
					case DashboardCellType.HQMtMessage:
						splitContainer.Panel1Collapsed=!graph.ShowFilters;
						break;
					case DashboardCellType.HQBillingUsageAccess:
					case DashboardCellType.HQSignups:
					case DashboardCellType.HQPhone:
					case DashboardCellType.HQMoMessage:
					case DashboardCellType.HQBillingInboundOutbound:
					case DashboardCellType.AccountsReceivableGraph:
					default:
						break;					
				}
			}
		}
		[Category("Graph")]
		public DashboardCellType CellType {
			get { return _cellType; }
			set {
				_cellType=value;
				BaseGraphOptionsCtrl filterCtrl=null;
				switch(CellType) {
					case DashboardCellType.ProductionGraph:
						filterCtrl=_productionOptionsCtrl;
						graph.GraphTitle="Production";
						graph.MoneyItemDescription="Production $";
						graph.CountItemDescription="Count Procedures";
						graph.RemoveQuantityType(QuantityType.decimalPoint);
						graph.GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months;
						graph.SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
						graph.BreakdownPref=BreakdownType.none;
						graph.LegendDock=LegendDockType.None;
						graph.QuickRangePref=QuickRange.last12Months;
						graph.QtyType=QuantityType.money;
						break;
					case DashboardCellType.IncomeGraph:
						filterCtrl=_incomeOptionsCtrl;
						graph.GraphTitle="Income";
						graph.MoneyItemDescription="Income $";
						graph.RemoveQuantityType(QuantityType.count);
						graph.RemoveQuantityType(QuantityType.decimalPoint);
						graph.GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months;
						graph.SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
						graph.BreakdownPref=BreakdownType.none;
						graph.LegendDock=LegendDockType.None;
						graph.QuickRangePref=QuickRange.last12Months;
						graph.QtyType=QuantityType.money;
						break;
					case DashboardCellType.BrokenApptGraph:
						filterCtrl=_brokenApptsCtrl;
						graph.GraphTitle="Broken Appointments";
						graph.MoneyItemDescription="Fees";
						graph.CountItemDescription="Count";
						graph.RemoveQuantityType(QuantityType.decimalPoint);
						graph.GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months;
						graph.SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
						graph.BreakdownPref=BreakdownType.none;
						graph.LegendDock=LegendDockType.None;
						graph.QuickRangePref=QuickRange.last12Months;
						graph.QtyType=QuantityType.count;
						break;
					case DashboardCellType.AccountsReceivableGraph:
						graph.GraphTitle="Accounts Receivable";
						graph.MoneyItemDescription="Receivable $";
						graph.RemoveQuantityType(QuantityType.count);
						graph.RemoveQuantityType(QuantityType.decimalPoint);
						graph.GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months;
						graph.SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
						graph.BreakdownPref=BreakdownType.none;
						graph.LegendDock=LegendDockType.None;
						graph.QuickRangePref=QuickRange.last12Months;
						graph.QtyType=QuantityType.money;
						break;
					case DashboardCellType.NewPatientsGraph:
						filterCtrl=new BaseGraphOptionsCtrl();
						graph.GraphTitle="New Patients";
						graph.RemoveQuantityType(QuantityType.money);
						graph.RemoveQuantityType(QuantityType.decimalPoint);
						graph.CountItemDescription="Count Patients";
						graph.GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months;
						graph.SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
						graph.BreakdownPref=BreakdownType.none;
						graph.LegendDock=LegendDockType.None;
						graph.QuickRangePref=QuickRange.last12Months;
						graph.QtyType=QuantityType.count;
						break;
					case DashboardCellType.HQMtMessage:
						filterCtrl=_hqMsgRealTimeCtrl;
						break;
					case DashboardCellType.HQPhone:
					case DashboardCellType.HQMoMessage:
					case DashboardCellType.HQSignups:
					case DashboardCellType.HQBillingUsageAccess:
					case DashboardCellType.HQBillingInboundOutbound:
						break;
					default:
						throw new Exception("Unsupported CellType: "+CellType.ToString());
				}
				splitContainerOptions.Panel2.Controls.Clear();
				if(filterCtrl==null) {
					splitContainer.Panel1Collapsed=true;
				}
				else {
					splitContainer.Panel1Collapsed=false;
					splitContainer.SplitterDistance=Math.Max(filterCtrl.GetPanelHeight(),this.groupingOptionsCtrl1.Height);
					filterCtrl.Dock=DockStyle.Fill;
					splitContainerOptions.Panel2.Controls.Add(filterCtrl);
					splitContainerOptions.Panel1Collapsed=!filterCtrl.HasGroupOptions;
				}
			}
		}
		public GroupingOptionsCtrl.Grouping CurGrouping
		{
			get
			{
				return groupingOptionsCtrl1.CurGrouping;
			}
			set
			{
				groupingOptionsCtrl1.CurGrouping=value;
			}
		}

		///<summary>Used in the Broadcast Monitor. Do not delete.</summary>
		public HqMessagesRealTimeOptionsCtrl HqMessagesRealTimeCtrl
		{
			get
			{
				return _hqMsgRealTimeCtrl;
			}
		}
		
		public ConfirmationGraphType HQConfirmationGraphType {
			get; set;
		}

		///<summary>Returns the Graph associated to this filter. From there the chart can be accessed.</summary>
		public GraphQuantityOverTime Graph
		{
			get { return graph; }
		}
		#endregion

		#region Ctor
		public GraphQuantityOverTimeFilter() :this (DashboardCellType.ProductionGraph) { }

		public GraphQuantityOverTimeFilter(DashboardCellType cellType,string jsonSettings="",GraphQuantityOverTime.OnGetColorFromSeriesGraphTypeArgs onGetSeriesColor =null,GetGraphPointsForHQArgs onGetODGraphPointsArgs=null) {
			InitializeComponent();
			//We will turn IsLoading off elsewhere but make sure it is set here to prevent trying to perform FilterData() to soon.
			graph.IsLoading=true;
			//Important that CellType is set before other properties as it gives default view.
			CellType=cellType;
			ShowFilters=false;
			graph.LegendDock=LegendDockType.None;
			SetFilterAndGraphSettings(jsonSettings);
			_incomeOptionsCtrl.InputsChanged+=OnFormInputsChanged;
			_productionOptionsCtrl.InputsChanged+=OnFormInputsChanged;
			_brokenApptsCtrl.InputsChanged+=OnFormInputsChanged;
			_hqMsgRealTimeCtrl.InputsChanged+=OnFormInputsChanged;
			_onGetSeriesColorOverride=onGetSeriesColor;
			_onGetODGraphPointsArgs=onGetODGraphPointsArgs;
			graph.GetRawData=OnGraphGetRawData;
		}
		#endregion

		#region ODGraphFilterAbs Overrides
		protected void OnInitDone(object sender,EventArgs e) {
			graph.TriggerGetData(sender);
		}

		protected void OnInit(bool forceCachRefresh) {
			//This is occurring in a thread so it is ok to wait for Refresh to return. The UI is already loading and available on the main thread.
			DashboardCache.RefreshCellTypeIfInvalid(CellType,graph.Filter,true,forceCachRefresh,null);
		}
		#endregion

		#region Previously from base class ODGraphFilterAbs and GraphQuantityOverTimeFilterT. These were base classes that were only getting in the way so moved all functionaly into concrete class.
		private ODThread _thread=null;

		///<summary>Starts the thread which will populate the graph's data. DB context should be set before calling this method.</summary>
		/// <param name="forceCacheRefesh">Will pass this flag along to the thread which retrieves the cache. If true then cache will be invalidated and re-initialiazed from the db. Use sparingly.</param>
		/// <param name="onException">Exception handler if something fails. If not set then all exceptions will be swallowed.</param>
		/// <param name="onThreadDone">Done event handler. Will be invoked when init thread has completed.</param>
		private void Init(bool forceCacheRefesh = false,ODThread.ExceptionDelegate onException = null,EventHandler onThreadDone = null) {
			if(_thread!=null) {
				return;
			}
			if(onThreadDone==null) {
				onThreadDone=new EventHandler(delegate (object o,EventArgs e) { });
			}
			if(onException==null) {
				onException=new ODThread.ExceptionDelegate((Exception e) => { });
			}
			_thread=new ODThread(new ODThread.WorkerDelegate((ODThread x) => {
				//The thread may have run and return before the window is even ready to display.
				if(this.IsHandleCreated) {
					OnThreadStartLocal(this,new EventArgs());
				}
				else {
					this.HandleCreated+=OnThreadStartLocal;
				}
				//Alert caller that it's time to start querying the db.
				OnInit(forceCacheRefesh);
			}));
			_thread.Name="GraphQuantityFilterOverTime.Init";
			_thread.AddExceptionHandler(onException);
			_thread.AddExitHandler(new ODThread.WorkerDelegate((ODThread x) => {
				try {
					_thread=null;
					//The thread may have run and return before the window is even ready to display.
					if(this.IsHandleCreated) {
						OnThreadExitLocal(this,new EventArgs());
					}
					else {
						this.HandleCreated+=OnThreadExitLocal;
					}
					//Alert caller that db querying is done.
					onThreadDone(this,new EventArgs());
				}
				catch(Exception) { }
			}));
			_thread.Start(true);
		}

		///<summary>Called when thread is started AND control handle has been created.</summary>
		private void OnThreadStartLocal(object sender,EventArgs e) {
			try {
				this.HandleCreated-=OnThreadStartLocal;
				this.Invoke((Action)delegate () {
					//Alert graph.
					graph.IsLoading=true;
				});
			}
			catch(Exception) { }
		}

		///<summary>Called after data has been initialized AND control handle has been created.</summary>
		private void OnThreadExitLocal(object sender,EventArgs e) {
			try {
				this.Invoke((Action)delegate () {
					if(!graph.IsLoading) { //Prevent re-entrance in case we get here more than once.
						return;
					}
					//Alert graph.
					graph.IsLoading=false;
					//Alert inheriting class.
					OnInitDone(this,new EventArgs());
				});
			}
			catch(Exception) { }
		}

		///<summary>Serializes GetGraphSettings() abstract method returned value. Won't normally need to override this.</summary>
		private string SerializeToJson() {
			return ODGraphSettingsBase.Serialize(GetGraphSettings());
		}

		///<summary>Set json settings for both the filter and the graph it is linked to. Input json should be a serialized ODGraphJson object.</summary>
		private void SetFilterAndGraphSettings(string jsonSettings) {
			if(string.IsNullOrEmpty(jsonSettings)) {
				return;
			}
			ODGraphJson graphJson=ODGraphJson.Deserialize(jsonSettings);
			if(graph!=null) {
				graph.DeserializeFromJson(graphJson.GraphJson);
			}
			DeserializeFromJson(graphJson.FilterJson);
		}

		///<summary>Get json settings for both the filter and the graph it is linked to. Output will be json in the form of a serialized ODGraphJson object.</summary>
		private string GetFilterAndGraphSettings() {
			return ODGraphJson.Serialize(new ODGraphJson() {
				FilterJson=SerializeToJson(),
				GraphJson=graph==null ? "" : graph.SerializeToJson(),
			});
		}

		public string GetCellSettings() {
			return GetFilterAndGraphSettings();
		}
		
		#endregion

		#region Form Events
		private void OnFormInputsChanged(object sender,EventArgs e) {
			graph.TriggerGetData(sender);
		}
		
		private List<GraphQuantityOverTime.GraphPointBase> OnGraphGetRawData() { 
			List<GraphQuantityOverTime.GraphPointBase> rawData=new List<GraphQuantityOverTime.GraphPointBase>();
			//Fill the dataset that we will send to the graph. The dataset will be filled according to user preferences.
			switch(CellType) {
				case DashboardCellType.ProductionGraph: {
						SetGroupItems(CurGrouping);
						if(_productionOptionsCtrl.IncludeAdjustments) {
							rawData.AddRange(DashboardCache.Adjustments.Cache.Select(x => GetDataPointForGrouping(x,CurGrouping)));
						}
						if(_productionOptionsCtrl.IncludeCompletedProcs) {
							rawData.AddRange(DashboardCache.CompletedProcs.Cache.Select(x => GetDataPointForGrouping(x,CurGrouping)));
							rawData.AddRange(DashboardCache.Writeoffs.Cache.Where(x => x.IsCap==true).Select(x => GetDataPointForGrouping(x,CurGrouping)));
						}
						if(_productionOptionsCtrl.IncludeWriteoffs) {
							rawData.AddRange(DashboardCache.Writeoffs.Cache.Where(x => x.IsCap==false).Select(x => GetDataPointForGrouping(x,CurGrouping)));
						}
					}
					break;
				case DashboardCellType.IncomeGraph: {
						SetGroupItems(CurGrouping);
						if(_incomeOptionsCtrl.IncludePaySplits) {
							rawData.AddRange(DashboardCache.PaySplits.Cache.Select(x => GetDataPointForGrouping(x,CurGrouping)));
						}
						if(_incomeOptionsCtrl.IncludeInsuranceClaimPayments) {
							rawData.AddRange(DashboardCache.ClaimPayments.Cache.Select(x => GetDataPointForGrouping(x,CurGrouping)));
						}
					}
					break;
				case DashboardCellType.AccountsReceivableGraph: {
						rawData.AddRange(DashboardCache.AR.Cache
							.Select(x => new GraphQuantityOverTime.GraphPointBase() {
								Val=x.BalTotal,
								Count=0,
								SeriesName="All",
								DateStamp=x.DateCalc,
							})
							.ToList());
					}
					break;
				case DashboardCellType.NewPatientsGraph: {
						SetGroupItems(CurGrouping);
						rawData.AddRange(DashboardCache.Patients.Cache.Select(x => GetDataPointForGrouping(x,CurGrouping)));
					}
					break;
				case DashboardCellType.BrokenApptGraph: {
						SetGroupItems(CurGrouping);
						switch(_brokenApptsCtrl.CurRunFor) {
							case BrokenApptGraphOptionsCtrl.RunFor.appointment:
								//money is not used when counting appointments
								graph.RemoveQuantityType(QuantityType.money);
								//use the broken appointment cache to get all relevant broken appts.
								rawData.AddRange(DashboardCache.BrokenAppts.Cache.Select(x => GetDataPointForGrouping(x,CurGrouping)));
								break;
							case BrokenApptGraphOptionsCtrl.RunFor.adjustment:
								//money should be added back in case the user looked at appointments beforehand. 
								graph.InsertQuantityType(QuantityType.money,"Fees",0);
								//use the broken adjustment cache to get all broken adjustments filtered by the selected adjType.
								rawData.AddRange(DashboardCache.BrokenAdjs.Cache.Where(x => x.AdjType==_brokenApptsCtrl.AdjTypeDefNumCur).Select(x => GetDataPointForGrouping(x,CurGrouping)));
								break;
							case BrokenApptGraphOptionsCtrl.RunFor.procedure:
								graph.InsertQuantityType(QuantityType.money,"Fees",0);
								//use the broken proc cache to get all relevant broken procedures.
								List<string> listProcCodes=new List<string>();
								switch(_brokenApptsCtrl.BrokenApptCodeCur) {
									case BrokenApptProcedure.None:
									case BrokenApptProcedure.Missed:
									listProcCodes.Add("D9986");
									break;
									case BrokenApptProcedure.Cancelled:
									listProcCodes.Add("D9987");
									break;
									case BrokenApptProcedure.Both:
									listProcCodes.Add("D9986");
									listProcCodes.Add("D9987");
									break;
								}
								rawData.AddRange(DashboardCache.BrokenProcs.Cache.Where(x => listProcCodes.Contains(x.ProcCode)).Select(x => GetDataPointForGrouping(x,CurGrouping)));
								break;
							default:
								throw new Exception("Unsupported CurRunFor: "+_brokenApptsCtrl.CurRunFor.ToString());
						}
					}
					break;
				case DashboardCellType.HQMtMessage:
				case DashboardCellType.HQBillingUsageAccess:
				case DashboardCellType.HQPhone:
				case DashboardCellType.HQSignups:
				case DashboardCellType.HQMoMessage:
				case DashboardCellType.HQBillingInboundOutbound:
					if(_onGetODGraphPointsArgs==null) {
						throw new Exception("OnGetODGraphPointsArgs delegate not set for CellType: "+CellType.ToString());
					}
					rawData=_onGetODGraphPointsArgs(this);
					break;
				default:
					throw new Exception("Unsupported CellType: "+CellType.ToString());
			}
			return rawData??new List<GraphQuantityOverTime.GraphPointBase>();
		}
		
		private void SetGroupItems(GroupingOptionsCtrl.Grouping CurGrouping) {			
			switch(CurGrouping) {
				case GroupingOptionsCtrl.Grouping.provider:
					graph.UseBuiltInColors=false;
					graph.LegendTitle="Provider";
					break;
				case GroupingOptionsCtrl.Grouping.clinic:
					graph.LegendTitle="Clinic";
					graph.UseBuiltInColors=true;
					break;
				default:
					graph.LegendTitle="Group";
					graph.UseBuiltInColors=true;
					break;
			}
		}

		private GraphQuantityOverTime.GraphDataPointClinic GetDataPointForGrouping(GraphQuantityOverTime.GraphDataPointClinic x,GroupingOptionsCtrl.Grouping curGrouping) {
			switch(curGrouping) {
				case GroupingOptionsCtrl.Grouping.provider:
					return new GraphQuantityOverTime.GraphDataPointClinic() {
						DateStamp=x.DateStamp,
						SeriesName=DashboardCache.Providers.GetProvName(x.ProvNum),
						Val=x.Val,
						Count=x.Count
					};
				case GroupingOptionsCtrl.Grouping.clinic:
				default:
					return new GraphQuantityOverTime.GraphDataPointClinic() {
						DateStamp=x.DateStamp,
						SeriesName=DashboardCache.Clinics.GetClinicName(x.ClinicNum),
						Val=x.Val,
						Count=x.Count
					};
			}
		}

		private Color graph_OnGetGetColor(string seriesName) {
			if(_onGetSeriesColorOverride!=null) {
				return _onGetSeriesColorOverride(this,CellType,seriesName);
			}
			switch(CellType) {
				case DashboardCellType.HQMtMessage:
				case DashboardCellType.HQBillingUsageAccess:
				case DashboardCellType.HQPhone:
				case DashboardCellType.HQSignups:
				case DashboardCellType.HQMoMessage:
				case DashboardCellType.HQBillingInboundOutbound:
					throw new Exception("This CellType cannot return a provider color");
				case DashboardCellType.BrokenApptGraph:
				case DashboardCellType.NewPatientsGraph:
				case DashboardCellType.AccountsReceivableGraph:
				case DashboardCellType.IncomeGraph:
				case DashboardCellType.ProductionGraph:
				case DashboardCellType.NotDefined:
				default:
					return DashboardCache.Providers.GetProvColor(seriesName);
			}
		}

		#endregion

		#region GraphDataPoint Conversions
		private GraphQuantityOverTime.GraphPointBase GetBrokenApptDataPoint(GraphQuantityOverTime.GraphDataPointClinic x) {
			switch(CurGrouping) {
				case GroupingOptionsCtrl.Grouping.provider:
					return new GraphQuantityOverTime.GraphPointBase() {
						DateStamp=x.DateStamp,
						SeriesName=DashboardCache.Providers.GetProvName(x.ProvNum),
						Val=x.Val,
						Count=x.Count
					};
				case GroupingOptionsCtrl.Grouping.clinic:
				default:
					return new GraphQuantityOverTime.GraphPointBase() {
						DateStamp=x.DateStamp,
						SeriesName=DashboardCache.Clinics.GetClinicName(x.ClinicNum),
						Val=x.Val,
						Count=x.Count
					};
			}
		}

		#endregion

		#region IDashboardDockContainer Implementation
		public DashboardDockContainer CreateDashboardDockContainer(TableBase dbItem=null) {
			string json="";
			DashboardDockContainer ret=new DashboardDockContainer(
				this,
				this.graph,				
				CanEdit?new EventHandler((s,ea) => {
					//Entering edit mode. 
					//Set graph to loading mode to show the loading icon.
					graph.IsLoading=true;
					//Spawn the cache thread(s) but don't block. 
					//Register for OnThreadExitLocal will invoke back to this thread when all the threads have exited and refill the form.
					DashboardCache.RefreshCellTypeIfInvalid(CellType,new DashboardFilter() { UseDateFilter=false },false,false,OnThreadExitLocal);
					//Allow filtering in edit mode.
					this.ShowFilters=true;
					//Save a copy of the current settings in case user clicks cancel.
					json=GetFilterAndGraphSettings();
				}):null,
				new EventHandler((s,ea) => {
					//Ok click. Just hide the filters.
					this.ShowFilters=false;
				}),
				new EventHandler((s,ea) => {
					//Cancel click. Just hide the filters and reset to previous settings.
					this.ShowFilters=false;
					SetFilterAndGraphSettings(json);
				}),
				new EventHandler((s,ea) => {
					//Spawn the init thread whenever this control gets dropped or dragged.
					Init();
				}),
				new EventHandler((s,ea) => {
					//Refresh button was clicked, spawn the init thread and force cache refresh.
					Init(true);
				}),
				dbItem);
			return ret;
		}

		public DashboardCellType GetCellType() {
			return CellType;
		}

		///<summary>This class should NEVER change variable names after being released. 
		///The variable names are stored as plain-text json and need to remain back-compatible.
		///Adding new fields is OK.</summary>
		public class GraphQuantityOverTimeFilterSettings:ODGraphSettingsBase {
			public bool IncludeCompleteProcs { get; set; }
			public bool IncludeAdjustements { get; set; }
			public bool IncludeWriteoffs { get; set; }
			public bool IncludePaySplits { get; set; }
			public bool IncludeInsuranceClaims { get; set; }
			public GroupingOptionsCtrl.Grouping CurGrouping { get;set;}
			public BrokenApptGraphOptionsCtrl.RunFor CurRunFor { get; set; }
			public long AdjTypeDefNum { get; set; }
			new public BrokenApptProcedure BrokenApptProcCode { get; set;}
			public HqMessagesRealTimeOptionsCtrl.HQGrouping HQGrouping { get; set; }
			public ConfirmationGraphType HQConfirmationGraphType { get; set; }

		}
		#endregion

		#region IODHasGraphSettings Implementation
		public GraphQuantityOverTimeFilterSettings GetGraphSettings() {
			switch(CellType) {
				case DashboardCellType.ProductionGraph:
					return new GraphQuantityOverTimeFilterSettings() {
						IncludeAdjustements=_productionOptionsCtrl.IncludeAdjustments,
						IncludeCompleteProcs=_productionOptionsCtrl.IncludeCompletedProcs,
						IncludeWriteoffs=_productionOptionsCtrl.IncludeWriteoffs,
						CurGrouping=this.CurGrouping,
					};
				case DashboardCellType.IncomeGraph:
					return new GraphQuantityOverTimeFilterSettings() {
						IncludePaySplits=_incomeOptionsCtrl.IncludePaySplits,
						IncludeInsuranceClaims=_incomeOptionsCtrl.IncludeInsuranceClaimPayments,
						CurGrouping=this.CurGrouping,
					};
				case DashboardCellType.BrokenApptGraph:
					return new GraphQuantityOverTimeFilterSettings() {
						CurGrouping=this.CurGrouping,
						CurRunFor=_brokenApptsCtrl.CurRunFor,
						AdjTypeDefNum=_brokenApptsCtrl.AdjTypeDefNumCur,
						BrokenApptProcCode=_brokenApptsCtrl.BrokenApptCodeCur,
					};
				case DashboardCellType.NewPatientsGraph:
					return new GraphQuantityOverTimeFilterSettings() {
						CurGrouping=this.CurGrouping,
					};
				case DashboardCellType.AccountsReceivableGraph:
					//No custom filtering so do nothing.
					return new GraphQuantityOverTimeFilterSettings();
				case DashboardCellType.HQMtMessage: 
					return new GraphQuantityOverTimeFilterSettings() {
						HQGrouping=_hqMsgRealTimeCtrl.CurHQGroup,
					};
				case DashboardCellType.HQBillingUsageAccess:
				case DashboardCellType.HQPhone:
					return new GraphQuantityOverTimeFilterSettings() {
						HQConfirmationGraphType=this.HQConfirmationGraphType,
					};
				case DashboardCellType.HQMoMessage:
				case DashboardCellType.HQSignups:
				case DashboardCellType.HQBillingInboundOutbound:
					//No custom filtering so do nothing.
					return new GraphQuantityOverTimeFilterSettings();
				default:
					throw new Exception("Unsupported CellType: "+CellType.ToString());
			}
		}

		public void DeserializeFromJson(string json) {
			try {
				if(string.IsNullOrEmpty(json)) {
					return;
				}
				switch(CellType) {
					case DashboardCellType.HQMoMessage:
					case DashboardCellType.HQSignups:
					case DashboardCellType.HQBillingInboundOutbound:
						return;
				}
				GraphQuantityOverTimeFilterSettings settings=ODGraphSettingsBase.Deserialize<GraphQuantityOverTimeFilterSettings>(json);
				switch(CellType) {
					case DashboardCellType.ProductionGraph:
						_productionOptionsCtrl.IncludeAdjustments=settings.IncludeAdjustements;
						_productionOptionsCtrl.IncludeCompletedProcs=settings.IncludeCompleteProcs;
						_productionOptionsCtrl.IncludeWriteoffs=settings.IncludeWriteoffs;
						this.CurGrouping=settings.CurGrouping;
						break;
					case DashboardCellType.IncomeGraph:
						_incomeOptionsCtrl.IncludePaySplits=settings.IncludePaySplits;
						_incomeOptionsCtrl.IncludeInsuranceClaimPayments=settings.IncludeInsuranceClaims;
						this.CurGrouping=settings.CurGrouping;
						break;
					case DashboardCellType.BrokenApptGraph:
						this.CurGrouping=settings.CurGrouping;
						_brokenApptsCtrl.CurRunFor=settings.CurRunFor;
						_brokenApptsCtrl.AdjTypeDefNumCur=settings.AdjTypeDefNum;
						_brokenApptsCtrl.BrokenApptCodeCur=settings.BrokenApptProcCode;
						break;
					case DashboardCellType.NewPatientsGraph:
						this.CurGrouping=settings.CurGrouping;
						break;
					case DashboardCellType.AccountsReceivableGraph:
						break;
					case DashboardCellType.HQMtMessage:
						_hqMsgRealTimeCtrl.CurHQGroup=settings.HQGrouping;
						return;
					case DashboardCellType.HQBillingUsageAccess:
					case DashboardCellType.HQPhone:
						this.HQConfirmationGraphType=settings.HQConfirmationGraphType;
						break;
					case DashboardCellType.HQMoMessage:
					case DashboardCellType.HQSignups:
					case DashboardCellType.HQBillingInboundOutbound:
						return;
					default:
						throw new Exception("Unsupported CellType: "+CellType.ToString());
				}
			}
			catch(Exception e) {
				if(ODBuild.IsDebug()) {
					MessageBox.Show(e.Message);
				}
				else{
					e.DoNothing();
				}
			}
		}

		public void PrintPreview() {
			graph.PrintPreview();
		}
		#endregion
	}
}
