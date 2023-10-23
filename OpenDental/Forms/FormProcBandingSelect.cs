using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormProcBandingSelect:FormODBase {
		///<summary>PatNum of currently selected patient.</summary>
		private long _patNum;
		///<summary>List of treatment planned banding procedures for the current patient.</summary>
		private List<Procedure> _listProceduresTpBanding=new List<Procedure>();
		///<summary>The procedure selected from the grid.</summary>
		public Procedure ProcedureSelected;

		public FormProcBandingSelect(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patNum=patNum;
		}

		private void FormProcBandingSelect_Load(object sender,EventArgs e) {
			_listProceduresTpBanding=Procedures.GetProcsForFormProcBandingSelect(_patNum);
			FillGrid();
		}

		private void FillGrid() {
			gridTpBandingProcs.BeginUpdate();
			gridTpBandingProcs.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableBandingProcs","Code"),70);
			gridTpBandingProcs.Columns.Add(col);
			col=new GridColumn(Lan.g("TableBandingProcs","Description"),140){ IsWidthDynamic=true };
			gridTpBandingProcs.Columns.Add(col);
			gridTpBandingProcs.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listProceduresTpBanding.Count;i++) {
				row=new GridRow();
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(_listProceduresTpBanding[i].CodeNum);
				row.Cells.Add(procedureCode.ProcCode);
				row.Cells.Add(procedureCode.Descript);
				row.Tag=_listProceduresTpBanding[i];
				gridTpBandingProcs.ListGridRows.Add(row);
			}
			gridTpBandingProcs.EndUpdate();
		}

		private void GridTpBandingProcs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SelectProcedure((Procedure)gridTpBandingProcs.ListGridRows[e.Row].Tag);
			DialogResult=DialogResult.OK;
		}

		private void SelectProcedure(Procedure procedure) {
			if(procedure.Discount!=0) {
				MsgBox.Show(this,"Banding Procedures with discounts cannot be attached to an ortho case.");
				return;
			}
			ProcedureSelected=procedure;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridTpBandingProcs.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a procedure first.");
				return;
			}
			SelectProcedure((Procedure)gridTpBandingProcs.ListGridRows[gridTpBandingProcs.GetSelectedIndex()].Tag);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}