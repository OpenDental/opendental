using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormInvoiceItemSelect:FormODBase {
		private DataTable _tableSuperFamAcct;
		private GridOD _gridMain;
		private long _patNum;
		///<summary></summary>
		public List<DataRow> ListDataRowsSelected=new List<DataRow>();

		public FormInvoiceItemSelect(long patNum) {
			_patNum=patNum;
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormInvoiceItemSelect_Load(object sender, System.EventArgs e) {
			_tableSuperFamAcct=Patients.GetSuperFamProcAdjustsPPCharges(_patNum);
			FillGrid();
		}

		private void FillGrid(){
			_gridMain.BeginUpdate();
			_gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableInvoiceItems","Date"),70);
			_gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","PatName"),100);
			_gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Prov"),55);
			_gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Code"),55);
			_gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Tooth"),50);
			_gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Description"),150);
			_gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Fee"),60,HorizontalAlignment.Right);
			_gridMain.Columns.Add(col);
			_gridMain.ListGridRows.Clear();
			GridRow row;
			List<ProcedureCode> listProcedureCodes=ProcedureCodes.GetAllCodes();
			for(int i=0;i<_tableSuperFamAcct.Rows.Count;i++) {
				if(checkIsFilteringZeroAmount.Checked && PIn.Double(_tableSuperFamAcct.Rows[i]["Amount"].ToString())==0){
					continue;
				}
				row=new GridRow();
				row.Cells.Add(PIn.DateT(_tableSuperFamAcct.Rows[i]["Date"].ToString()).ToShortDateString());
				row.Cells.Add(_tableSuperFamAcct.Rows[i]["PatName"].ToString());
				row.Cells.Add(Providers.GetAbbr(PIn.Long(_tableSuperFamAcct.Rows[i]["Prov"].ToString())));
				if(!string.IsNullOrWhiteSpace(_tableSuperFamAcct.Rows[i]["AdjType"].ToString())){	//It's an adjustment
					row.Cells.Add(Lan.g(this,"Adjust"));//Adjustment
					row.Cells.Add(Tooth.Display(_tableSuperFamAcct.Rows[i]["Tooth"].ToString()));
					row.Cells.Add(Defs.GetName(DefCat.AdjTypes,PIn.Long(_tableSuperFamAcct.Rows[i]["AdjType"].ToString())));//Adjustment type
				}
				else if(!string.IsNullOrWhiteSpace(_tableSuperFamAcct.Rows[i]["ChargeType"].ToString())) {	//It's a payplan charge
					if(PrefC.GetInt(PrefName.PayPlansVersion)!=(int)PayPlanVersions.AgeCreditsAndDebits) {
						continue;//They can only attach debits to invoices and they can only do so if they're on version 2.
					}
					row.Cells.Add(Lan.g(this, "Pay Plan"));
					row.Cells.Add(Tooth.Display(_tableSuperFamAcct.Rows[i]["Tooth"].ToString()));
					row.Cells.Add(PIn.Enum<PayPlanChargeType>(PIn.Int(_tableSuperFamAcct.Rows[i]["ChargeType"].ToString())).GetDescription());//Pay Plan charge type
				}
				else{//It's a procedure
					ProcedureCode procedureCode=ProcedureCodes.GetProcCode(PIn.Long(_tableSuperFamAcct.Rows[i]["Code"].ToString()),listProcedureCodes);
					row.Cells.Add(procedureCode.ProcCode);
					row.Cells.Add(Tooth.Display(_tableSuperFamAcct.Rows[i]["Tooth"].ToString()));
					row.Cells.Add(procedureCode.Descript);
				}
				row.Cells.Add(PIn.Double(_tableSuperFamAcct.Rows[i]["Amount"].ToString()).ToString("F"));
				row.Tag=_tableSuperFamAcct.Rows[i];
				_gridMain.ListGridRows.Add(row);
			}
			_gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DataRow dataRow=(DataRow)_gridMain.ListGridRows[e.Row].Tag;
			ListDataRowsSelected.Clear();
			ListDataRowsSelected.Add(dataRow);
			DialogResult=DialogResult.OK;
		}

		private void checkIsFilteringZeroAmount_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butAll_Click(object sender,System.EventArgs e) {
			_gridMain.SetAll(true);
		}

		private void butNone_Click(object sender,System.EventArgs e) {
			_gridMain.SetAll(false);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(_gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			ListDataRowsSelected.Clear();
			for(int i=0;i<_gridMain.SelectedIndices.Length;i++) {
				DataRow dataRow=(DataRow)_gridMain.ListGridRows[_gridMain.SelectedIndices[i]].Tag;
				ListDataRowsSelected.Add(dataRow);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}