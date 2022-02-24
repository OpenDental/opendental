using System;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrMedicalOrders:FormODBase {
		public Patient _patCur;
		private DataTable table;
		///<summary>If this is true after exiting, then launch MedicationPat dialog.</summary>
		public bool LaunchMedicationPat;
		/// <summary>If LaunchMedicationPat is true after exiting, then this specifies which MedicationPat to open.  Will never be zero.</summary>
		public long LaunchMedicationPatNum;

		public FormEhrMedicalOrders() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMedicalOrders_Load(object sender,EventArgs e) {
			FillGridMedOrders();
		}

		private void FillGridMedOrders() {
			gridMedOrders.BeginUpdate();
			gridMedOrders.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridMedOrders.ListGridColumns.Add(col);
			col=new GridColumn("Type",80);
			gridMedOrders.ListGridColumns.Add(col);
			col=new GridColumn("Prov",70);
			gridMedOrders.ListGridColumns.Add(col);
			col=new GridColumn("Instructions",280);
			gridMedOrders.ListGridColumns.Add(col);
			col=new GridColumn("Status",100);
			gridMedOrders.ListGridColumns.Add(col);
			table=MedicalOrders.GetOrderTable(_patCur.PatNum, checkBoxShowDiscontinued.Checked);
			gridMedOrders.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["date"].ToString());
				row.Cells.Add(table.Rows[i]["type"].ToString());
				row.Cells.Add(table.Rows[i]["prov"].ToString());
				row.Cells.Add(table.Rows[i]["description"].ToString());
				row.Cells.Add(table.Rows[i]["status"].ToString());
				gridMedOrders.ListGridRows.Add(row);
			}
			gridMedOrders.EndUpdate();
		}

		private void gridMedOrders_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long medicalOrderNum=PIn.Long(table.Rows[e.Row]["MedicalOrderNum"].ToString());
			MedicalOrder ord=MedicalOrders.GetOne(medicalOrderNum);
			if(ord.MedOrderType==MedicalOrderType.Laboratory) {
				using FormEhrMedicalOrderLabEdit FormMlab=new FormEhrMedicalOrderLabEdit();
				FormMlab.MedOrderCur=ord;
				FormMlab.ShowDialog();
			}
			else {//Rad
				using FormEhrMedicalOrderRadEdit FormMrad=new FormEhrMedicalOrderRadEdit();
				FormMrad.MedOrderCur=ord;
				FormMrad.ShowDialog();
			}
			FillGridMedOrders();
		}

		private void checkBoxShowDiscontinued_Click(object sender,EventArgs e) {
			FillGridMedOrders();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		






	}
}
