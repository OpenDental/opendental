using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDentalGraph {
	///<summary>Connection to db is required before creating an instance of this form.</summary>
	public partial class FormDashboardEditTab:Form {
		#region Private Data
		private Random _rand=new Random();
		private string _dashboardGroupName="Default";
		private EventHandler _onSetDb=null;
		#endregion

		#region Properties
		///<summary>Get or set the DashboardGroupName that will be used to group these DashboardLayout(s) and DashboardCell(s) in the database.</summary>
		public string DashboardGroupName {
			get { return _dashboardGroupName; }
			set {
				_dashboardGroupName=value;
				SetTitle();
			}
		}
		///<summary>Get or set flag to modify UI for editing. Typically linked to a user permission.</summary>
		public bool IsEditMode {
			get { return !splitContainer1.Panel1Collapsed; }
			set {
				splitContainer1.Panel1Collapsed=!value;
				dashboardTabControl.IsEditMode=IsEditMode;
				setupToolStripMenuItem.Text=IsEditMode ? "Exit Setup" : "Setup";
				menuItemDefaultGraphs.Visible=IsEditMode;
				menuItemResetAR.Visible=IsEditMode;
				SetTitle();
			}
		}
		#endregion

		#region Ctor/Init
		///<summary></summary>
		/// <param name="onSetDb">Pass in a callback IF and only IF you want to provide a different db context for DashboardLayouts s-class.
		/// This is only used in BroadcastMonitor as the db context is typically already set in OD proper.</param>
		public FormDashboardEditTab(long provNum,bool useProvFilter,EventHandler onSetDb=null) {			
			InitializeComponent();
			_onSetDb=onSetDb;
			Cache.DashboardCache.ProvNum=provNum;
			Cache.DashboardCache.UseProvFilter=useProvFilter;
			//Text is loaded here because loading it from the designer forces the designer to use a resource.resx file. This is slow for some reason but loading it here is quick.
			labelHelp.Text=@"Drag a graph type to a cell on the graphic reports editor.

Drag any existing graph in the editor to any empty cell within the editor. Note: The target cell must be empty.

Remove or add an entire row/column/tab by clicking the corresponding delete/add row/column/tab button. Note: All cells must be empty for a given row/column/tab before deleting.

Edit an individual graph's settings by clicking the edit button for that cell.

Double-click tab header to rename tab.";
			SetTitle();
			listItems.Items.Clear();
			//Do not add HQMessages for our customers' release version.
			//DashboardCellType.HQMessagesOut, DashboardCellType.HQMessagesSentReceived, DashboardCellType.HQMessagesBilling
			listItems.Items.Add(new DashboardListItem() { CellType=DashboardCellType.ProductionGraph,Display="Production",});
			listItems.Items.Add(new DashboardListItem() { CellType=DashboardCellType.IncomeGraph,Display="Income",});
			listItems.Items.Add(new DashboardListItem() { CellType=DashboardCellType.AccountsReceivableGraph,Display="A/R",});
			listItems.Items.Add(new DashboardListItem() { CellType=DashboardCellType.NewPatientsGraph,Display="New Patients",});
			listItems.Items.Add(new DashboardListItem() { CellType=DashboardCellType.BrokenApptGraph,Display="Broken Appointments",});
			//It is very important that the tab layout and subsequent tabpages be added in the ctor and NOT in the load event (as is the typical OD pattern).
			//Performing the layout here speeds this process by up considerably (I have seen as fast as 8x faster). -samo 1/25/16
			//https://social.msdn.microsoft.com/Forums/windows/en-US/12cc5748-21c8-4494-b975-8b05c7513979/tabcontrol-very-slow-with-lots-of-tabs
			RefreshData(false);
			if(!PrefC.HasClinicsEnabled) {
				addClinicDefaultToolStripMenuItem.Visible=false;
			}
			AddDefaultTabs();
		}

		private void RefreshData(bool invalidateFirst) {
			if(_onSetDb!=null) {
				_onSetDb(this,new EventArgs());
			}
			dashboardTabControl.SetDashboardLayout(DashboardLayouts.GetDashboardLayout(DashboardGroupName),invalidateFirst);
		}
				
		private void SetTitle() {
			string title="Graphic Reports - "+DashboardGroupName;
			if(IsEditMode) {
				title+=" (Edit)";
			}
			this.Text=title;
		}
		#endregion

		private void listItems_MouseDown(object sender,MouseEventArgs e) {
			int i=listItems.IndexFromPoint(new Point(e.X,e.Y));
			if(i<0) {
				return;
			}
			if(listItems.SelectedItem==null || !(listItems.SelectedItem is DashboardListItem)) {
				return;
			}
			DashboardDockContainer holder=new GraphQuantityOverTimeFilter(((DashboardListItem)listItems.SelectedItem).CellType).CreateDashboardDockContainer();
			holder.Contr.DoDragDrop(holder,DragDropEffects.All);
		}
				
		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!IsEditMode) {
				if(!Security.IsAuthorized(Permissions.GraphicalReportSetup)) {
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.GraphicalReportSetup,0,"Accessed graphical reports setup controls.");
			}
			IsEditMode=!IsEditMode;
		}

		private void refreshDataToolStripMenuItem_Click(object sender,EventArgs e) {
			if(dashboardTabControl.HasUnsavedChanges) {
				if(MessageBox.Show("You have unsaved changes. Click OK to continue and discard changes.","Discard Changes?",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
					return;
				}
			}
			RefreshData(true);
			AddDefaultTabs();
		}

		///<summary>Adds default tabs if no tabs are found.</summary>
		private void AddDefaultTabs() {
			if(dashboardTabControl.TabPageCount==0) {
				dashboardTabControl.AddDefaultsTabPractice(false);
				if(PrefC.HasClinicsEnabled) {
					dashboardTabControl.AddDefaultsTabByGrouping(false,GroupingOptionsCtrl.Grouping.clinic);
				}
				dashboardTabControl.AddDefaultsTabByGrouping(false,GroupingOptionsCtrl.Grouping.provider);
			}
		}

		private void butSaveChanges_Click(object sender,EventArgs e) {
			List<DashboardLayout> layouts;
			dashboardTabControl.GetDashboardLayout(out layouts);
			DashboardLayouts.SetDashboardLayout(layouts,DashboardGroupName);
			//Save occurred. Let the tab control know that there are no changes remaining before refreshing data below. 
			//Omitting this step would cause the tab control to prompt user to save changes before continuing.
			dashboardTabControl.HasUnsavedChanges=false;
			RefreshData(false);
			MessageBox.Show("Graphic Report group saved: "+DashboardGroupName+".");
		}
		
		private void FormDashboardEditTab_FormClosing(object sender,FormClosingEventArgs e) {
			if(dashboardTabControl.HasUnsavedChanges) {
				if(MessageBox.Show("You have unsaved changes. Click OK to continue and discard changes.","Discard Changes?",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
					e.Cancel=true;
					return;
				}
			}
		}
		
		private class DashboardListItem {
			public DashboardCellType CellType;
			public string Display;
			public override string ToString() {
				return Display;
			}
		}

		///<summary>Adds the default practice tab to the current tabs.  MenuItem only visible in Edit mode.</summary>
		private void addPracticeDefaultToolStripMenuItem_Click(object sender,EventArgs e) {
			dashboardTabControl.AddDefaultsTabPractice(true);
		}

		///<summary>Adds the default clinic tab to the current tabs.  MenuItem only visible in Edit mode.</summary>
		private void addClinicDefaultToolStripMenuItem_Click(object sender,EventArgs e) {
			dashboardTabControl.AddDefaultsTabByGrouping(true,GroupingOptionsCtrl.Grouping.clinic);
		}

		///<summary>Adds the default provider tab to the current tabs.  MenuItem only visible in Edit mode.</summary>
		private void addProviderDefaultToolStripMenuItem_Click(object sender,EventArgs e) {
			dashboardTabControl.AddDefaultsTabByGrouping(true,GroupingOptionsCtrl.Grouping.provider);
		}

		private void menuItemResetAR_Click(object sender,EventArgs e) {
			if(MessageBox.Show("Are you sure you want to refresh your AR reports? This could take a long time.",
				"Continue?",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
				return;
			}
			if(dashboardTabControl.HasUnsavedChanges) {
				if(MessageBox.Show("You have unsaved changes. Click OK to continue and discard changes.","Discard Changes?",
					MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
					return;
				}
			}
			DashboardARs.Truncate();
			RefreshData(true);
			AddDefaultTabs();
		}
	}
}
