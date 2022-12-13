using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormScheduledProcesses:FormODBase {

		public FormScheduledProcesses() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		private void FormScheduledProcesses_Load(object sender,EventArgs e) {
			FillGrid();
			checkCheckAnnualMax.Checked=PrefC.GetBool(PrefName.InsBatchVerifyCheckAnnualMax);
			checkCheckDeductable.Checked=PrefC.GetBool(PrefName.InsBatchVerifyCheckDeductible);
			checkCreateAdjustments.Checked=PrefC.GetBool(PrefName.InsBatchVerifyCreateAdjustments);
			checkChangeInsHist.Checked=PrefC.GetBool(PrefName.InsBatchVerifyChangeInsHist);
			checkChangeEffectiveDates.Checked=PrefC.GetBool(PrefName.InsBatchVerifyChangeEffectiveDates);
		}

		private void FillGrid() {
			List<ScheduledProcess> listScheduledProcesses=ScheduledProcesses.Refresh();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Scheduled Action"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Frequency to Run"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Time To Run"),75);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Time of Last Run"),155);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for (int i=0;i<listScheduledProcesses.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listScheduledProcesses[i].ScheduledAction.GetDescription());
				row.Cells.Add(listScheduledProcesses[i].FrequencyToRun.GetDescription());
				row.Cells.Add(listScheduledProcesses[i].TimeToRun.ToShortTimeString());
				if(listScheduledProcesses[i].LastRanDateTime.Year > 1880) {
					row.Cells.Add(listScheduledProcesses[i].LastRanDateTime.ToString());
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=listScheduledProcesses[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			ScheduledProcess scheduledProcess=new ScheduledProcess();
			scheduledProcess.IsNew=true;
			using FormScheduledProcessesEdit formScheduledProcessesEdit=new FormScheduledProcessesEdit(scheduledProcess);
			if(formScheduledProcessesEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void GridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ScheduledProcess scheduledProcessSelected=gridMain.SelectedTag<ScheduledProcess>();
			using FormScheduledProcessesEdit formScheduledProcessesEdit=new FormScheduledProcessesEdit(scheduledProcessSelected);
			if(formScheduledProcessesEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormScheduledProcesses_FormClosing(object sender,FormClosingEventArgs e) {
			bool isChanged=false;
			isChanged |=Prefs.UpdateBool(PrefName.InsBatchVerifyCheckAnnualMax, checkCheckAnnualMax.Checked);
			isChanged |=Prefs.UpdateBool(PrefName.InsBatchVerifyCheckDeductible, checkCheckDeductable.Checked);
			isChanged |=Prefs.UpdateBool(PrefName.InsBatchVerifyCreateAdjustments, checkCreateAdjustments.Checked);
			isChanged |=Prefs.UpdateBool(PrefName.InsBatchVerifyChangeInsHist, checkChangeInsHist.Checked);
			isChanged |=Prefs.UpdateBool(PrefName.InsBatchVerifyChangeEffectiveDates, checkChangeEffectiveDates.Checked);
			if(isChanged) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
	}
}