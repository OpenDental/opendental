using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDashboardWidgets:FormODBase {
		///<summary>The dashboard that has been selected.</summary>
		public SheetDef SheetDefDashboardWidget;

		public FormDashboardWidgets(bool isCEMT=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormDashboard_Load(object sender,EventArgs e) {
			LayoutMenu();
			FillGrid();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",setupToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		private void FillGrid() {
			List<SheetDef> listDashboardWidgets=SheetDefs.GetCustomForType(SheetTypeEnum.PatientDashboardWidget)
				.Where(x => Security.IsAuthorized(Permissions.DashboardWidget,x.SheetDefNum,true)).ToList();
			List<SheetDef> listSelectedDashboards=gridMain.SelectedTags<SheetDef>();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Dashboard Name",0,HorizontalAlignment.Left));
			gridMain.ListGridRows.Clear();
			foreach(SheetDef sheetDashboardWidget in listDashboardWidgets) {
				GridRow row=new GridRow();
				row.Cells.Add(sheetDashboardWidget.Description);
				row.Tag=sheetDashboardWidget;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(ListTools.In(((SheetDef)gridMain.ListGridRows[i].Tag).SheetDefNum,listSelectedDashboards.Select(x => x.SheetDefNum))) {
					gridMain.SetSelected(i,true);
				}
			}
		}
		
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SheetDefDashboardWidget=gridMain.SelectedTag<SheetDef>();
			DialogResult=DialogResult.OK;
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDashboardWidgetSetup FormDS=new FormDashboardWidgetSetup();
			if(FormDS.ShowDialog()==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SheetDefDashboardWidget=gridMain.SelectedTag<SheetDef>();
			DialogResult=DialogResult.OK;
		}

    private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}