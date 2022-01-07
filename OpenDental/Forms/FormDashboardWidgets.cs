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
			List<SheetDef> listSheetDefsWidgets=SheetDefs.GetCustomForType(SheetTypeEnum.PatientDashboardWidget);
			listSheetDefsWidgets=listSheetDefsWidgets.FindAll(x => Security.IsAuthorized(Permissions.DashboardWidget,x.SheetDefNum,true));
			List<SheetDef> listSheetDefsSelected=gridMain.SelectedTags<SheetDef>();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Dashboard Name",0,HorizontalAlignment.Left));
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listSheetDefsWidgets.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listSheetDefsWidgets[i].Description);
				row.Tag=listSheetDefsWidgets[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				long sheetDefNum=((SheetDef)gridMain.ListGridRows[i].Tag).SheetDefNum;
				List<long> listSheetDefNums=listSheetDefsSelected.Select(x => x.SheetDefNum).ToList();
				if(ListTools.In(sheetDefNum,listSheetDefNums)) {
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