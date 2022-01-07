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
		private List<Procedure> _listTpBandingProcs=new List<Procedure>();
		///<summary>The procedure selected from the grid.</summary>
		public Procedure SelectedProcedure;

		public FormProcBandingSelect(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patNum=patNum;
		}

		private void FormProcBandingSelect_Load(object sender,EventArgs e) {
			_listTpBandingProcs=Procedures.GetProcsForFormProcBandingSelect(_patNum);
			FillGrid();
		}

		private void FillGrid() {
			gridTpBandingProcs.BeginUpdate();
			gridTpBandingProcs.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableBandingProcs","Code"),70);
			gridTpBandingProcs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableBandingProcs","Description"),140){ IsWidthDynamic=true };
			gridTpBandingProcs.ListGridColumns.Add(col);
			gridTpBandingProcs.ListGridRows.Clear();
			GridRow row;
			foreach(Procedure proc in _listTpBandingProcs) {
				row=new GridRow();
				ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				row.Cells.Add(procCode.ProcCode);
				row.Cells.Add(procCode.Descript);
				row.Tag=proc;
				gridTpBandingProcs.ListGridRows.Add(row);
			}
			gridTpBandingProcs.EndUpdate();
		}

		private void GridTpBandingProcs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SelectProcedure((Procedure)gridTpBandingProcs.ListGridRows[e.Row].Tag);
			DialogResult=DialogResult.OK;
		}

		private void SelectProcedure(Procedure selectedProc) {
			if(selectedProc.Discount!=0) {
				MsgBox.Show(this,"Banding Procedures with discounts cannot be attached to an ortho case.");
				return;
			}
			SelectedProcedure=selectedProc;
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