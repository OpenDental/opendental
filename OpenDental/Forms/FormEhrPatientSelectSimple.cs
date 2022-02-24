using System;
using System.Data;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrPatientSelectSimple:FormODBase {
		public long SelectedPatNum;
		private DataTable table;
		public string FName;
		public string LName;

		public FormEhrPatientSelectSimple() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormPatientSelectSimple_Load(object sender,EventArgs e) {
			textFName.Text=FName;
			textLName.Text=LName;
			FillGrid();
		}

		private void FillGrid() {
			PtTableSearchParams ptTableSearchParams=new PtTableSearchParams(false,textLName.Text,textFName.Text,"","",false,"","","","","",0,false,false,
				DateTime.MinValue,0,"","","","","","","");
			table=Patients.GetPtDataTable(ptTableSearchParams);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("PatNum",70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("LName",120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("FName",120);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["PatNum"].ToString());
				row.Cells.Add(table.Rows[i]["LName"].ToString());
				row.Cells.Add(table.Rows[i]["FName"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PatSelected();
		}

		private void butSearch_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void PatSelected() {
			SelectedPatNum=PIn.Long(table.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString());
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MessageBox.Show("Please select a patient first.");
				return;
			}
			PatSelected();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
		
		
	}
}
