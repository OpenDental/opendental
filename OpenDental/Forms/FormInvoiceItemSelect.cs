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
		private GridOD gridMain;
		private long _patNum;
		///<summary>This dictionary contains all selected items from the grid when OK is pressed.
		///The string will either be "Adj" or "Proc" and the long will be the corresponding primary key.</summary>
		public Dictionary<string,List<long>> DictSelectedItems=new Dictionary<string,List<long>>();

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
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableInvoiceItems","Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","PatName"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Prov"),55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Code"),55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Tooth"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Description"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInvoiceItems","Fee"),60,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<ProcedureCode> listProcCodes=ProcedureCodes.GetAllCodes();
			foreach(DataRow tableRow in _tableSuperFamAcct.Rows) {
				row=new GridRow();
				row.Cells.Add(PIn.DateT(tableRow["Date"].ToString()).ToShortDateString());
				row.Cells.Add(tableRow["PatName"].ToString());
				row.Cells.Add(Providers.GetAbbr(PIn.Long(tableRow["Prov"].ToString())));
				if(!string.IsNullOrWhiteSpace(tableRow["AdjType"].ToString())){	//It's an adjustment
					row.Cells.Add(Lan.g(this,"Adjust"));//Adjustment
					row.Cells.Add(Tooth.ToInternat(tableRow["Tooth"].ToString()));
					row.Cells.Add(Defs.GetName(DefCat.AdjTypes,PIn.Long(tableRow["AdjType"].ToString())));//Adjustment type
				}
				else if(!string.IsNullOrWhiteSpace(tableRow["ChargeType"].ToString())) {	//It's a payplan charge
					if(PrefC.GetInt(PrefName.PayPlansVersion)!=(int)PayPlanVersions.AgeCreditsAndDebits) {
						continue;//They can only attach debits to invoices and they can only do so if they're on version 2.
					}
					row.Cells.Add(Lan.g(this, "Pay Plan"));
					row.Cells.Add(Tooth.ToInternat(tableRow["Tooth"].ToString()));
					row.Cells.Add(PIn.Enum<PayPlanChargeType>(PIn.Int(tableRow["ChargeType"].ToString())).GetDescription());//Pay Plan charge type
				}
				else {	//It's a procedure
					ProcedureCode procCode=ProcedureCodes.GetProcCode(PIn.Long(tableRow["Code"].ToString()),listProcCodes);
					row.Cells.Add(procCode.ProcCode);
					row.Cells.Add(Tooth.ToInternat(tableRow["Tooth"].ToString()));
					row.Cells.Add(procCode.Descript);
				}
				row.Cells.Add(PIn.Double(tableRow["Amount"].ToString()).ToString("F"));
				row.Tag=tableRow;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DataRow row=(DataRow)gridMain.ListGridRows[e.Row].Tag;
			string type="";
			if(!string.IsNullOrWhiteSpace(row["AdjType"].ToString())){	//It's an adjustment
				type="Adj";
			}
			else if(!string.IsNullOrWhiteSpace(row["ChargeType"].ToString())) {	//It's a payplan charge
				type="Pay Plan";
			}
			else {	//It's a procedure
				type="Proc";
			}
			DictSelectedItems.Clear();
			DictSelectedItems.Add(type,new List<long>() { PIn.Long(row["PriKey"].ToString()) });//Add the clicked-on entry
			DialogResult=DialogResult.OK;
		}

		private void butAll_Click(object sender,System.EventArgs e) {
			gridMain.SetAll(true);
		}

		private void butNone_Click(object sender,System.EventArgs e) {
			gridMain.SetAll(false);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				DataRow row=(DataRow)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				string type="";
				if(!string.IsNullOrWhiteSpace(row["AdjType"].ToString())){	//It's an adjustment
					type="Adj";
				}
				else if(!string.IsNullOrWhiteSpace(row["ChargeType"].ToString())) {	//It's a payplan charge
					type="Pay Plan";
				}
				else {	//It's a procedure
					type="Proc";
				}
				long priKey=PIn.Long(row["PriKey"].ToString());
				List<long> listPriKeys;
				if(DictSelectedItems.TryGetValue(type,out listPriKeys)) {//If an entry with Proc or Adj already exists, grab its list
					listPriKeys.Add(priKey);//Add the primary key to the list
				}
				else {//No entry with Proc or Adj
					DictSelectedItems.Add(type,new List<long>() { priKey });//Make a new dict entry
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}