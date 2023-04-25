using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormRxCombine:FormODBase {
		public List<RxDef> ListRxDefs;
		public RxDef RxDefCur;

		public FormRxCombine() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRxCombine_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRxSetup","Drug"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Controlled"),70,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Sig"),320);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Disp"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Refills"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Notes"),0);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListRxDefs.Count;i++){
				row=new GridRow();
				row.Cells.Add(ListRxDefs[i].Drug);
				if(ListRxDefs[i].IsControlled){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(ListRxDefs[i].Sig);
				row.Cells.Add(ListRxDefs[i].Disp);
				row.Cells.Add(ListRxDefs[i].Refills);
				row.Cells.Add(ListRxDefs[i].Notes);
				row.Tag=ListRxDefs[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			RxDefCur=gridMain.SelectedTag<RxDef>();
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			RxDefCur=gridMain.SelectedTag<RxDef>();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}