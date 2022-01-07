using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormReqStudentOne : FormODBase {
		public long ProvNum;
		private Provider prov;
		private DataTable table;
		//public bool IsSelectionMode;
		//<summary>If IsSelectionMode and DialogResult.OK, then this will contain the selected req.</summary>
		//public int SelectedReqStudentNum;

		///<summary></summary>
		public FormReqStudentOne()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReqStudentOne_Load(object sender,EventArgs e) {
			//if(IsSelectionMode){
				
			//}
			//else{
				//labelSelection.Visible=false;
				//butOK.Visible=false;
				//butCancel.Text=Lan.g(this,"Close");
			//}
			prov=Providers.GetProv(ProvNum);
			Text=Lan.g(this,"Student Requirements - ")+Providers.GetLongDesc(ProvNum);
			FillGrid();
		}

		private void FillGrid(){
			table=ReqStudents.RefreshOneStudent(ProvNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableReqStudentOne","Course"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableReqStudentOne","Requirement"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableReqStudentOne","Done"),40);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableReqStudentOne","Patient"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableReqStudentOne","Appointment"),190);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["course"].ToString());
				row.Cells.Add(table.Rows[i]["requirement"].ToString());
				row.Cells.Add(table.Rows[i]["done"].ToString());
				row.Cells.Add(table.Rows[i]["patient"].ToString());
				row.Cells.Add(table.Rows[i]["appointment"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//if(IsSelectionMode){
			//	if(table.Rows[e.Row]["appointment"].ToString()!=""){
			//		MsgBox.Show(this,"Already attached to an appointment.");
			//		return;
			//	}
			//	SelectedReqStudentNum=PIn.PInt(table.Rows[e.Row]["ReqStudentNum"].ToString());
			//	DialogResult=DialogResult.OK;
			//}
			//else{
				using FormReqStudentEdit FormRSE=new FormReqStudentEdit();
				FormRSE.ReqCur=ReqStudents.GetOne(PIn.Long(table.Rows[e.Row]["ReqStudentNum"].ToString()));
				FormRSE.ShowDialog();
				if(FormRSE.DialogResult!=DialogResult.OK) {
					return;
				}
				FillGrid();
			//}
		}

		//private void butOK_Click(object sender, System.EventArgs e) {
			//not accessible
			/*if(IsSelectionMode){
				if(gridMain.GetSelectedIndex()==-1){
					MsgBox.Show(this,"Please select a requirement first.");
					return;
				}
				if(table.Rows[gridMain.GetSelectedIndex()]["appointment"].ToString()!="") {
					MsgBox.Show(this,"Selected requirement is already attached to an appointment.");
					return;
				}
				SelectedReqStudentNum=PIn.PInt(table.Rows[gridMain.GetSelectedIndex()]["ReqStudentNum"].ToString());
				DialogResult=DialogResult.OK;
			}*/
			//should never get to here.
		//}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















