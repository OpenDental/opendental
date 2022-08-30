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
			foreach(ScheduledProcess schedProc in listScheduledProcesses) {
				row=new GridRow();
				row.Cells.Add(schedProc.ScheduledAction.GetDescription());
				row.Cells.Add(schedProc.FrequencyToRun.GetDescription());
				row.Cells.Add(schedProc.TimeToRun.ToShortTimeString());
				if(schedProc.LastRanDateTime.Year > 1880) {
					row.Cells.Add(schedProc.LastRanDateTime.ToString());
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=schedProc;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			ScheduledProcess schedProc=new ScheduledProcess();
			schedProc.IsNew=true;
			using FormScheduledProcessesEdit formScheduledProcessesEdit=new FormScheduledProcessesEdit(schedProc);
			if(formScheduledProcessesEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void GridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ScheduledProcess selectedSchedProc=gridMain.SelectedTag<ScheduledProcess>();
			using FormScheduledProcessesEdit formScheduledProcessesEdit=new FormScheduledProcessesEdit(selectedSchedProc);
			if(formScheduledProcessesEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormScheduledProcesses_FormClosing(object sender,FormClosingEventArgs e) {
			bool changed=false;
			changed |=Prefs.UpdateBool(PrefName.InsBatchVerifyCheckAnnualMax, checkCheckAnnualMax.Checked);
			changed |=Prefs.UpdateBool(PrefName.InsBatchVerifyCheckDeductible, checkCheckDeductable.Checked);
			changed |=Prefs.UpdateBool(PrefName.InsBatchVerifyCreateAdjustments, checkCreateAdjustments.Checked);
			changed |=Prefs.UpdateBool(PrefName.InsBatchVerifyChangeInsHist, checkChangeInsHist.Checked);
			changed |=Prefs.UpdateBool(PrefName.InsBatchVerifyChangeEffectiveDates, checkChangeEffectiveDates.Checked);
			if(changed) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
	}
}