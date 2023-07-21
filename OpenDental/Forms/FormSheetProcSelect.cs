using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormSheetProcSelect:FormODBase {
	
		public List<long> ListProcNumsSelected;
		public long PatNum;

		public FormSheetProcSelect() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetProcSelect_Load(object sender,EventArgs e) {
			FillGridProcs();
			gridProcs.ScrollToEnd();
		}

		private void FillGridProcs() {
			List<Procedure> listProcedures=Procedures.GetPatientData(PatNum);
			gridProcs.BeginUpdate();
			gridProcs.ListGridRows.Clear();
			gridProcs.Columns.Clear();
			gridProcs.Columns.Add(new GridColumn(Lan.g("TableProc","Date"),67,HorizontalAlignment.Left));
			gridProcs.Columns.Add(new GridColumn(Lan.g("TableProc","Th"),27,HorizontalAlignment.Left));
			gridProcs.Columns.Add(new GridColumn(Lan.g("TableProc","Surf"),40,HorizontalAlignment.Left));
			gridProcs.Columns.Add(new GridColumn(Lan.g("TableProc","Description"),318,HorizontalAlignment.Left));
			gridProcs.Columns.Add(new GridColumn(Lan.g("TableProc","Amount"),63,HorizontalAlignment.Right));
			gridProcs.Columns.Add(new GridColumn(Lan.g("TableProc","Code"),62,HorizontalAlignment.Center));
			GridRow row;
			for(int i=0;i<listProcedures.Count;i++) {
				row=new GridRow();
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcedures[i].CodeNum);
				row.Cells.Add(listProcedures[i].ProcDate.ToShortDateString());
				row.Cells.Add(Tooth.Display(listProcedures[i].ToothNum));
				string displaySurf;
				if(ProcedureCodes.GetProcCode(listProcedures[i].CodeNum).TreatArea==TreatmentArea.Sextant) {
					displaySurf=Tooth.GetSextant(listProcedures[i].Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
				}
				else {
					displaySurf=Tooth.SurfTidyFromDbToDisplay(listProcedures[i].Surf,listProcedures[i].ToothNum);
				}
				row.Cells.Add(displaySurf);
				string description="";
				if(procedureCode.LaymanTerm=="") {
					description=procedureCode.Descript;
				}
				else {
					description=procedureCode.LaymanTerm;
				}
				row.Cells.Add(description);
				row.Cells.Add(listProcedures[i].ProcFee.ToString("C"));
				row.Cells.Add(procedureCode.ProcCode);
				row.Tag=listProcedures[i];
				gridProcs.ListGridRows.Add(row);
			}
			gridProcs.EndUpdate();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridProcs.SelectedIndices.Length==0) {
				MsgBox.Show("Please select at least 1 procedure.");
				return;
			}
			ListProcNumsSelected=gridProcs.SelectedTags<Procedure>().Select(x =>x.ProcNum).ToList();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}