using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormRxSetup : FormODBase {
		private RxDef[] RxDefList;

		///<summary></summary>
		public FormRxSetup(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRxSetup_Load(object sender, System.EventArgs e) {
			checkProcCodeRequired.Checked=PrefC.GetBool(PrefName.RxHasProc);
			FillGrid();
		}

		private void FillGrid(){
			RxDefList=RxDefs.Refresh();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRxSetup","Drug"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Controlled"),70,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Sig"),320);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Disp"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Refills"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRxSetup","Notes"),300);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<RxDefList.Length;i++){
				row=new GridRow();
				row.Cells.Add(RxDefList[i].Drug);
				if(RxDefList[i].IsControlled){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(RxDefList[i].Sig);
				row.Cells.Add(RxDefList[i].Disp);
				row.Cells.Add(RxDefList[i].Refills);
				row.Cells.Add(RxDefList[i].Notes);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void checkProcCodeRequired_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.RxHasProc,checkProcCodeRequired.Checked)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormRxDefEdit FormE=new FormRxDefEdit(RxDefList[e.Row]);
			FormE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			RxDef RxDefCur=new RxDef();
			RxDefs.Insert(RxDefCur);//It gets deleted if user clicks cancel
			using FormRxDefEdit FormE=new FormRxDefEdit(RxDefCur);
			FormE.IsNew=true;
			FormE.ShowDialog();
			FillGrid();
		}

		private void butAdd2_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select item first"));
				return;
			}
			RxDef RxDefCur=RxDefList[gridMain.GetSelectedIndex()].Copy();
			RxDefs.Insert(RxDefCur);//Now it has a new id.  It gets deleted if user clicks cancel. Alerts not copied.
			using FormRxDefEdit FormE=new FormRxDefEdit(RxDefCur);
			FormE.IsNew=true;
			FormE.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

	}
}
