using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDentalGraph {
	public partial class DashboardTabCtrl:UserControl {
		private bool _isEditMode=false;
		private bool _hasUnsavedChanges=false;

		///<summary>Determines whether the user is in edit mode. Edit mode allows users to Add/Remove tabs.</summary>
		public bool IsEditMode {
			get { return _isEditMode; }
			set {
				_isEditMode=value;
				TabPage tabPage; DashboardPanelCtrl dashboardPanel;
				if(IsEditMode) { //Insert 'add' tab if it's not already there.
					tabControl.ImageList=tabImageList;
					if(tabControl.TabCount==0 || GetDashboardPanel(tabControl.TabCount-1,out tabPage,out dashboardPanel)) {
						//Last page is a DashboardPanelCtrl so the 'add' tab is not already there, insert it at the end.
						tabControl.TabPages.Add(new TabPage() { ImageIndex=0,BackColor=SystemColors.Control,Text="Add Tab",});
					}
					butPrintPage.Visible=false;
				}
				else { //Remove 'add' tab if it's not already there.
					tabControl.ImageList=null;
					if(tabControl.TabCount>0 && !GetDashboardPanel(tabControl.TabCount-1,out tabPage,out dashboardPanel)) {
						//Last page exists and it is not a DashboardPanelCtrl it is the 'add' tab. Remove it.
						tabControl.TabPages.RemoveAt(tabControl.TabCount-1);
					}
					butPrintPage.Visible=true;
				}
				for(int tabIndex = 0;tabIndex<tabControl.TabCount;tabIndex++) {
					if(GetDashboardPanel(tabIndex,out tabPage,out dashboardPanel)) {
						dashboardPanel.IsEditMode=IsEditMode;
						SetTabPageImageIndex(tabPage,IsEditMode ? 1 : -1);
					}					
				}
			}
		}
		///<summary>Setter only works to set to false. Setting to true does nothing. This is handled internally by the control itself.
		///Getter returns true if new 1) Any tab pages have HasUnsavedChanges==true 2) New tab added 3) Tab deleted.</summary>
		public bool HasUnsavedChanges {
			get {
				if(_hasUnsavedChanges) { //Tab pages have been added or removed.
					return true;
				}
				//Check individual tab pages for modifications.
				int tabCount=IsEditMode?tabControl.TabCount-1:tabControl.TabCount;
				for(int i = 0;i<tabCount;++i) {
					TabPage tabPage; DashboardPanelCtrl dashboardPanel;
					if(!GetDashboardPanel(i,out tabPage,out dashboardPanel)) {
						continue;
					}
					if(dashboardPanel.HasUnsavedChanges) {
						return true;
					}
				}
				return false;
			}
			set {
				if(value) { //Only allow turn off (not on).
					return;
				}
				//Check individual tab pages for modifications.
				int tabCount=IsEditMode?tabControl.TabCount-1:tabControl.TabCount;
				for(int i = 0;i<tabCount;++i) {
					TabPage tabPage; DashboardPanelCtrl dashboardPanel;
					if(!GetDashboardPanel(i,out tabPage,out dashboardPanel)) {
						continue;
					}
					dashboardPanel.HasUnsavedChanges=false;
				}
				_hasUnsavedChanges=false;
			}
		}

		///<summary>Number of tabs.</summary>
		public int TabPageCount
		{
			get
			{
				return tabControl.TabCount;
			}
		}

		///<summary>Gets the graphs for all tab pages.</summary>
		public List<GraphQuantityOverTimeFilter> Graphs {
			get {
				List<GraphQuantityOverTimeFilter> ret=new List<GraphQuantityOverTimeFilter>();
				TabPage tabPage;
				DashboardPanelCtrl dashboardPanel;
				for(int tabIndex = 0;tabIndex<tabControl.TabCount;tabIndex++) {
					if(!GetDashboardPanel(tabIndex,out tabPage,out dashboardPanel)) {
						continue;
					}
					ret.AddRange(dashboardPanel.Graphs);
				}
				return ret;
			}
		}

		public DashboardTabCtrl() {
			InitializeComponent();
			AddTabPage();		
		}

		#region Public Methods
		///<summary>Gets all dashboard layouts associated to this tab control.</summary>
		public void GetDashboardLayout(out List<DashboardLayout> layouts) {
			layouts=new List<DashboardLayout>();
			int tabCount=IsEditMode?tabControl.TabCount-1:tabControl.TabCount;
			for(int i=0;i<tabCount;++i) {
				TabPage tabPage; DashboardPanelCtrl dashboardPanel;
				if(!GetDashboardPanel(i,out tabPage,out dashboardPanel)) {
					continue;
				}
				DashboardLayout layout=new DashboardLayout() {
					IsNew=true,
					DashboardTabName=tabPage.Text,
					DashboardTabOrder=dashboardPanel.TabOrder,
					DashboardRows=dashboardPanel.Rows,
					DashboardColumns=dashboardPanel.Columns,
					//The rest of the fields are filled below if this was an existing db row.
					DashboardLayoutNum=0,
					UserNum=0,
					UserGroupNum=0,
				};
				if(dashboardPanel.DbItem!=null) { //This was an existing db row so update.
					layout.IsNew=false;
					layout.DashboardLayoutNum=dashboardPanel.DbItem.DashboardLayoutNum;
					layout.UserNum=dashboardPanel.DbItem.UserNum;
					layout.UserGroupNum=dashboardPanel.DbItem.UserGroupNum;
				}
				layout.Cells=dashboardPanel.Cells;
				layouts.Add(layout);
			}
		}

		///<summary>Any unsaved changes will be overwritten. Prompt the user to save changes before calling this method.</summary>
		public void SetDashboardLayout(List<DashboardLayout> layouts,bool invalidateFirst,GraphQuantityOverTime.OnGetColorFromSeriesGraphTypeArgs onGetSeriesColor =null,GraphQuantityOverTimeFilter.GetGraphPointsForHQArgs onGetODGraphPointsArgs=null) {
			//This may need to be uncommented if user is not being prompted to save changes in certain cases.
			//_hasUnsavedChanges=false;
			if(IsEditMode) {
				while(tabControl.TabCount>1) { //Leave the 'add' tab.
					tabControl.TabPages.RemoveAt(0);
				}
			}
			else {
				while(tabControl.TabCount>0) { //Remove all tabs.
					tabControl.TabPages.RemoveAt(0);
				}
			}			
			//This will start cache threads.
			Cache.DashboardCache.RefreshLayoutsIfInvalid(layouts,false,invalidateFirst);
			this.SuspendLayout();	
			layouts.OrderBy(x => x.DashboardTabOrder).ToList().ForEach(x => {
				DashboardPanelCtrl dashboardPanel=new DashboardPanelCtrl(x);
				dashboardPanel.SetCellLayout(x.DashboardRows,x.DashboardColumns,x.Cells,onGetSeriesColor,onGetODGraphPointsArgs);
				AddTabPage(x.DashboardTabName,dashboardPanel);
			});
			if(tabControl.TabCount>=1) {
				tabControl.SelectedIndex=0;
			}
			this.ResumeLayout();
		}

		#region Default Tabs
		///<summary>Will add the pre-defined default practice tab.
		///Only one default practice tab can exist at a time, but the user may rename it to add multiple.
		///This method does NOT save the tab to the database. The user will still have to save changes and will have the option of discarding this tab.</summary>
		public void AddDefaultsTabPractice(bool hasPrompt) {
			if(!ValidateTabName("Practice Defaults")){
				return;
			}
			//get current layouts
			List<DashboardLayout> layoutsCur;
			GetDashboardLayout(out layoutsCur);			
			//create layout
			DashboardLayout layout = new DashboardLayout() {
				DashboardColumns=3,
				DashboardRows=2,
				DashboardGroupName="Default",
				DashboardTabName="Practice Defaults",
				//assigned default layout to first tab.
				DashboardTabOrder=layoutsCur.Count,
				UserGroupNum=0,
				UserNum=0,
				IsNew=true,
			};
			int i=0;
			//define cell settings. anything not set here will be the defaults defined in GraphQuantityOverTimeFilter
			for(int row = 0;row < layout.DashboardRows;row++) {
				for(int col = 0;col < layout.DashboardColumns;col++) {
					DashboardCell cell = new DashboardCell() {
						CellRow=row,
						CellColumn=col,
					};
					switch(i++) {
						case 0:
							cell.CellType=DashboardCellType.ProductionGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									IncludeCompleteProcs=true,
									IncludeAdjustements=true,
									IncludeWriteoffs=true,
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Production - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.money,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.None,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.none,
								})
							});
							break;
						case 1:
							cell.CellType=DashboardCellType.IncomeGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									IncludePaySplits=true,
									IncludeInsuranceClaims=true,
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Income - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.money,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.None,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.none,
								})
							});
							break;
						case 2:
							cell.CellType=DashboardCellType.BrokenApptGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									CurRunFor=BrokenApptGraphOptionsCtrl.RunFor.appointment
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Broken Appointments - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.count,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Weeks,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.None,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.none,
								})
							});
							break;
						case 3:
							cell.CellType=DashboardCellType.AccountsReceivableGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson="",
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Accounts Receivable - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.money,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.None,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.none,
								})
							});
							break;
						case 4:
							cell.CellType=DashboardCellType.NewPatientsGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson="",
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="New Patients - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.count,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.None,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.none,
								})
							});
							break;
						case 5:
						default:
							cell.CellType=DashboardCellType.ProductionGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									IncludeCompleteProcs=true,
									IncludeAdjustements=true,
									IncludeWriteoffs=true,
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Production - Last 30 Days",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.money,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Days,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.None,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last30Days,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.none,
								})
							});
							break;
					}
					layout.Cells.Add(cell);
				}
			}
			layoutsCur.Add(layout);
			//set dashboard tab control layouts
			SetDashboardLayout(layoutsCur,false);
			_hasUnsavedChanges=_hasUnsavedChanges||hasPrompt;
		}

		///<summary>Will add the pre-defined default clinics tab. 
		///Only one default clinics tab can exist at a time, but the user may rename it to add multiple.
		///This method does NOT save the tab to the database. The user will still have to save changes and will have the option of discarding this tab.</summary>
		public void AddDefaultsTabByGrouping(bool hasPrompt,GroupingOptionsCtrl.Grouping grouping) {
			string strGrouping=char.ToUpper(grouping.ToString()[0])+grouping.ToString().Substring(1);
			if(!ValidateTabName(strGrouping+" Defaults")) {
				return;
			}
			//get current layouts
			List<DashboardLayout> layoutsCur;
			GetDashboardLayout(out layoutsCur);
			//create layout
			DashboardLayout layout = new DashboardLayout() {
				DashboardColumns=3,
				DashboardRows=2,
				DashboardGroupName="Default",
				DashboardTabName=strGrouping+" Defaults",
				//assigned default layout to first tab.
				DashboardTabOrder=layoutsCur.Count,
				UserGroupNum=0,
				UserNum=0,
				IsNew=true,
			};
			int i=0;
			for(int row = 0;row<layout.DashboardRows;row++) {
				for(int col = 0;col<layout.DashboardColumns;col++) {
					DashboardCell cell = new DashboardCell() {
						CellRow=row,
						CellColumn=col,
					};
					switch(i++) {
						default:
						case 0:
							cell.CellType=DashboardCellType.ProductionGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									CurGrouping=grouping,
									IncludeCompleteProcs=true,
									IncludeAdjustements=true,
									IncludeWriteoffs=true,
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Production - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.money,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.Bottom,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.items,
									BreakdownVal=10,
								})
							});
							break;
						case 1:
							cell.CellType=DashboardCellType.IncomeGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									CurGrouping=grouping,
									IncludePaySplits=true,
									IncludeInsuranceClaims=true,
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Income - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.money,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.Bottom,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.items,
									BreakdownVal=10,
								})
							});
							break;
						case 2:
							cell.CellType=DashboardCellType.BrokenApptGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									CurGrouping=grouping,
									CurRunFor=BrokenApptGraphOptionsCtrl.RunFor.appointment,
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Broken Appointments - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.count,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.Bottom,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.items,
									BreakdownVal=10,
								})
							});
							break;
						case 3:
							cell.CellType=DashboardCellType.ProductionGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									CurGrouping=grouping,
									IncludeCompleteProcs=true,
									IncludeAdjustements=true,
									IncludeWriteoffs=true,
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Production - Last 30 Days",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.money,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Days,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.Bottom,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last30Days,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.items,
									BreakdownVal=10,
								})
							});
							break;
						case 4:
							cell.CellType=DashboardCellType.IncomeGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									CurGrouping=grouping,
									IncludePaySplits=true,
									IncludeInsuranceClaims=true,
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="Income - Last 30 Days",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.money,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Days,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.Bottom,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last30Days,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.items,
									BreakdownVal=10,
								})
							});
							break;
						case 5:
							cell.CellType=DashboardCellType.NewPatientsGraph;
							cell.CellSettings=ODGraphJson.Serialize(new ODGraphJson() {
								FilterJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTimeFilter.GraphQuantityOverTimeFilterSettings() {
									CurGrouping=grouping
								}),
								GraphJson=ODGraphSettingsBase.Serialize(new GraphQuantityOverTime.QuantityOverTimeGraphSettings() {
									Title="New Patients - Last 12 Months",
									QtyType=OpenDentalGraph.Enumerations.QuantityType.count,
									SeriesType=System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn,
									GroupByType=System.Windows.Forms.DataVisualization.Charting.IntervalType.Months,
									LegendDock=OpenDentalGraph.Enumerations.LegendDockType.Bottom,
									QuickRangePref=OpenDentalGraph.Enumerations.QuickRange.last12Months,
									BreakdownPref=OpenDentalGraph.Enumerations.BreakdownType.items,
									BreakdownVal=10,
								})
							});
							break;
					}
					layout.Cells.Add(cell);
				}
			}
			layoutsCur.Add(layout);
			//set dashboard tab control layouts
			SetDashboardLayout(layoutsCur,true);
			_hasUnsavedChanges=_hasUnsavedChanges||hasPrompt;
		}

		#endregion
		#endregion

		#region TabPage Helpers
		///<summary>Gets the tabPage and DashboardPanelControl for the given tab page index.</summary>
		private bool GetDashboardPanel(int tabIndex,out TabPage tabPage,out DashboardPanelCtrl dashboardPanel) {
			tabPage=tabControl.TabPages[tabIndex];
			dashboardPanel=null;
			if(tabPage.Controls.Count<=0||!(tabPage.Controls[0] is DashboardPanelCtrl)) {
				return false;
			}
			dashboardPanel=(DashboardPanelCtrl)tabPage.Controls[0];
			return true;
		}

		private TabPage AddTabPage(string tabText="",DashboardPanelCtrl dashboardPanel=null) {
			int i=1;
			if(string.IsNullOrEmpty(tabText)) {
				tabText="Tab "+i.ToString();
			}
			while(tabControl.TabPages.ContainsKey(tabText)) {
				i++;
				tabText="Tab "+i.ToString();
			}
			if(dashboardPanel==null) {
				dashboardPanel=new DashboardPanelCtrl();
			}
			TabPage tabPage=new TabPage(tabText);
			tabPage.Name=tabText;
			tabPage.ImageIndex=IsEditMode?1:-1; //Show the delete button if edit mode.
			tabPage.BackColor=SystemColors.Control;
			dashboardPanel.Dock=DockStyle.Fill;
			dashboardPanel.Name=tabText;
			dashboardPanel.IsEditMode=IsEditMode;
			tabPage.Controls.Add(dashboardPanel);
			tabControl.CreateControl();
			tabControl.TabPages.Insert(IsEditMode?tabControl.TabCount-1:tabControl.TabCount,tabPage); //Insert before the 'add' tab if in edit mode.
			tabControl.SelectedTab=tabPage;
			RefreshTabOrdering();
			return tabPage;
		}

		private void DeleteTabPage(int tabIndex) {
			if(!IsEditMode) {
				return;
			}
			TabPage tabPage; DashboardPanelCtrl dashboardPanel;
			if(!GetDashboardPanel(tabIndex,out tabPage,out dashboardPanel)) {
				return;
			}
			if(tabControl.TabCount==2) {
				MessageBox.Show("Dashboard must contain a minimum of 1 tab.");
				return;
			}
			if(!dashboardPanel.CanDelete) {
				MessageBox.Show("Tab '"+dashboardPanel.Name+"' has items. Remove all items from tab before continuing.");
				return;
			}
			tabControl.TabPages.RemoveAt(tabIndex);
			tabControl.SelectedIndex=Math.Max(tabIndex-1,0);
			RefreshTabOrdering();
		}

		private void RefreshTabOrdering() {
			for(int tabIndex=0;tabIndex<tabControl.TabCount;tabIndex++) {
				TabPage tabPage; DashboardPanelCtrl dashboardPanel;
				if(!GetDashboardPanel(tabIndex,out tabPage,out dashboardPanel)) {
					return;
				}
				dashboardPanel.TabOrder=tabIndex;
			}
		}

		private void SetTabPageImageIndex(TabPage tab,int imageIndex) {
			if(tab.ImageIndex!=imageIndex) {
				tab.ImageIndex=imageIndex;
			}
		}

		private bool IsLocationInTabHeaderBounds(int index,Point location) {
			return tabControl.GetTabRect(index).Contains(location);
		}

		private bool IsLocationInIconButtonBounds(int index,Point location) {
			if(!IsLocationInTabHeaderBounds(index,location)) {
				return false;
			}
			Rectangle rectHeader=tabControl.GetTabRect(index);
			Rectangle rectImage=new Rectangle(new Point(rectHeader.Location.X+6,rectHeader.Location.Y+2),new Size(tabImageList.ImageSize.Width,tabImageList.ImageSize.Width));
			return rectImage.Contains(location);
		}

		private bool ValidateTabName(string tabName) {
			if(tabControl.TabPages.ContainsKey(tabName)) {
				MessageBox.Show("Tab name '"+tabName+"' already exists.");
				return false;
			}
			return true;
		}
		#endregion

		#region Events
		private void tabControl_MouseDoubleClick(object sender,MouseEventArgs e) {
			if(!IsEditMode) {
				return;
			}
			if(IsLocationInTabHeaderBounds(tabControl.TabCount-1,e.Location)) { //Over the 'add' tab.
				return;
			}
			for(int i=0;i<tabControl.TabCount;++i) {
				if(IsLocationInTabHeaderBounds(i,e.Location)) {
					TabPage tabPage=tabControl.TabPages[i];
					using FormDashboardNamePrompt fdnp=new FormDashboardNamePrompt(tabPage.Name,new FormDashboardNamePrompt.ValidateTabNameArgs(ValidateTabName));
					if(fdnp.ShowDialog(this)!=DialogResult.OK) {
						return;
					}
					if(tabPage.Name==fdnp.TabName) {
						return;
					}
					tabPage.Name=fdnp.TabName;
					tabPage.Text=fdnp.TabName;
					tabPage.Controls[0].Name=fdnp.TabName;
					return;
				}
			}
		}

		private void tabControl_MouseClick(object sender,MouseEventArgs e) {
			if(!IsEditMode) {
				return;
			}
			for(int i=0;i<(tabControl.TabCount);i++) {
				if(i==tabControl.TabCount-1) {  
					if(IsLocationInTabHeaderBounds(i,e.Location)) { //We are over the 'add' tab header.
						AddTabPage();
						_hasUnsavedChanges=true;
					}
				}
				else if(IsLocationInIconButtonBounds(i,e.Location)) { //We are over the 'delete' icon.
					DeleteTabPage(i);
					_hasUnsavedChanges=true;
				}
			}
		}

		private void tabControl_MouseMove(object sender,MouseEventArgs e) {
			if(!IsEditMode) {
				return;
			}	
			for(int i = 0;i<(tabControl.TabCount-1);++i) {
				//0 - add no hover
				//1 - delete no hover
				//2 - delete with hover
				TabPage tabPage; DashboardPanelCtrl dashboardPanel;
				if(!GetDashboardPanel(i,out tabPage,out dashboardPanel)) {
					continue;
				}
				if(IsLocationInIconButtonBounds(i,e.Location)) { //Hovering over 'delete' icon.
					SetTabPageImageIndex(tabPage,2);
					dashboardPanel.SetHightlightedAllCells(true);
				}
				else { //Not hovering over 'delete' icon.
					SetTabPageImageIndex(tabPage,1);
					dashboardPanel.SetHightlightedAllCells(false);
				}
			}
		}

		private void butPrintPage_Click(object sender,EventArgs e) {
			//print all graphs on the tab control.
			DashboardPanelCtrl dashboardPanel;
			TabPage pageCur;
			if(!GetDashboardPanel(tabControl.SelectedIndex,out pageCur,out dashboardPanel)) {
				MessageBox.Show("Can't print this page.  Please try printing the graphs individually.");
				return;
			}
			using FormPrintImage FormPS = new FormPrintImage(dashboardPanel);
			FormPS.ShowDialog();
		}

		private void tabControl_Selecting(object sender,TabControlCancelEventArgs e) {
			TabPage tabPage; DashboardPanelCtrl dashboardPanel;
			if(IsEditMode&&!GetDashboardPanel(e.TabPageIndex,out tabPage,out dashboardPanel)) { //Prevent 'add' tab from being selected.
				e.Cancel=true;
			}
		}
		#endregion
	}
}
