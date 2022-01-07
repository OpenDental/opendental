using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormScheduledProcessesEdit:FormODBase {
		private ScheduledProcess _schedProcEdit;
		private ScheduledProcess _schedProcOld;

		public FormScheduledProcessesEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public FormScheduledProcessesEdit(ScheduledProcess schedProc) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_schedProcEdit=schedProc;
			_schedProcOld=_schedProcEdit.Copy();
		}

		private void FormScheduledProcessesEdit_Load(object sender,EventArgs e) {
			comboScheduledAction.Items.AddEnums<ScheduledActionEnum>();
			if(!_schedProcEdit.IsNew){
				comboScheduledAction.SetSelected((int)_schedProcEdit.ScheduledAction);
			}
			comboFrequency.Items.AddEnums<FrequencyToRunEnum>();
			if(!_schedProcEdit.IsNew){
				comboFrequency.SetSelected((int)_schedProcEdit.FrequencyToRun);
			}
			textTimeToRun.Clear();
			if(!_schedProcEdit.IsNew) {	
				textTimeToRun.Text=_schedProcEdit.TimeToRun.ToShortTimeString();
			}
		}
		
		private void SetScheduledProcessObject() {
			_schedProcEdit.ScheduledAction=comboScheduledAction.GetSelected<ScheduledActionEnum>(); 
			_schedProcEdit.FrequencyToRun=comboFrequency.GetSelected<FrequencyToRunEnum>();
			_schedProcEdit.TimeToRun=PIn.DateT(textTimeToRun.Text);
		}

		private bool ValidateFields() {
			if(comboScheduledAction.SelectedIndex==-1) {
				MessageBox.Show("A Scheduled Action must be selected.");
				return false;
			}
			if(comboFrequency.SelectedIndex==-1) {
				MessageBox.Show("A Frequency for the action must be selected.");
				return false;
			}
			if(textTimeToRun.Text=="" || !textTimeToRun.IsValid()) {
				MessageBox.Show("A valid time to run must be entered.");
				return false;
			}
			return true;
		}

		private bool IsScheduled() {
			//If editing a scheduled process and click ok without changing values, do not warn.
			if(_schedProcEdit.ScheduledProcessNum==_schedProcOld.ScheduledProcessNum &&
				_schedProcEdit.ScheduledAction==_schedProcOld.ScheduledAction &&
				_schedProcEdit.TimeToRun.TimeOfDay==_schedProcOld.TimeToRun.TimeOfDay &&
				_schedProcEdit.FrequencyToRun==_schedProcOld.FrequencyToRun) 
			{
				return false;
			}
			List<ScheduledProcess> listSchedProc=ScheduledProcesses.CheckAlreadyScheduled(_schedProcEdit.ScheduledAction,_schedProcEdit.FrequencyToRun,
				_schedProcEdit.TimeToRun);
			if(listSchedProc.Count<1) {
				return false;
			}
			return true;
		}

		private void butDeleteSchedProc_Click(object sender,EventArgs e) {
			if(!_schedProcOld.IsNew && MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will delete the currently selected Scheduled Process. Continue?",
				"Delete Scheduled Process."))
			{
				ScheduledProcesses.Delete(_schedProcOld.ScheduledProcessNum);
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateFields()) {
				return;
			}
			SetScheduledProcessObject();
			if(IsScheduled()) {
				MessageBox.Show("There is an identical Action already scheduled for that time and frequency.");
				return;
			}
			if(_schedProcEdit.IsNew) {
				ScheduledProcesses.Insert(_schedProcEdit);
			}
			else {
				ScheduledProcesses.Update(_schedProcEdit,_schedProcOld);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}