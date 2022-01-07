using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;

namespace OpenDental {
	public partial class FormApptBreak:FormODBase {
		
		public ApptBreakSelection FormApptBreakSelection;
		public ProcedureCode SelectedProcCode;
		private readonly Appointment _appt;

		public FormApptBreak(Appointment appt) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_appt=appt;
		}

		private void FormApptBreak_Load(object sender,EventArgs e) {
			BrokenApptProcedure brokenApptProcs=(BrokenApptProcedure)PrefC.GetInt(PrefName.BrokenApptProcedure);
			radioMissed.Enabled=ListTools.In(brokenApptProcs,BrokenApptProcedure.Missed,BrokenApptProcedure.Both);
			radioCancelled.Enabled=ListTools.In(brokenApptProcs,BrokenApptProcedure.Cancelled,BrokenApptProcedure.Both);
			if(radioMissed.Enabled && !radioCancelled.Enabled) {
				radioMissed.Checked=true;
			}
			else if(!radioMissed.Enabled && radioCancelled.Enabled) {
				radioMissed.Checked=true;
			}
		}

		private bool ValidateSelection() {
			if(!radioMissed.Checked && !radioCancelled.Checked) {
				MsgBox.Show(this,"Please select a broken procedure type.");
				return false;
			}
			return true;
		}

		private void DisplayFormAsapForWebSched() {
			if(!AppointmentL.PromptTextAsapList(_appt.ClinicNum)) {
				return;
			}
			// Schedule.SchedDate is a Date, time is ignored.
			DataTable dataTableSchedules=Schedules.GetPeriodSchedule(_appt.AptDateTime,_appt.AptDateTime,new List<long> { _appt.Op },false);
			List<Schedule> listSchedules=Schedules.ConvertTableToList(dataTableSchedules);
			DateRange dateRange;
			try {
				dateRange=AppointmentL.GetAsapRange(_appt.Op,_appt.AptDateTime,_appt.AptNum,listSchedules);
			}
			catch(ODException ex) {
				MessageBox.Show(this,ex.Message);
				return;
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Unexpected error occurred."),ex);
				return;
			}
			using FormASAP formASAP=new FormASAP(_appt.AptDateTime,dateRange.Start,dateRange.End,_appt.Op);
			formASAP.ShowDialog();
		}

		private void butUnsched_Click(object sender,EventArgs e) {
			if(!ValidateSelection()) {
				return;
			}
			if(PrefC.GetBool(PrefName.UnscheduledListNoRecalls) && Appointments.IsRecallAppointment(_appt)) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Recall appointments cannot be sent to the Unscheduled List.\r\nDelete appointment instead?")) {
					FormApptBreakSelection=ApptBreakSelection.Delete;//Set to delete so the parent form can handle the delete. 
					DialogResult=DialogResult.Cancel;
				}
				return;
			}
			DisplayFormAsapForWebSched();
			FormApptBreakSelection=ApptBreakSelection.Unsched;
			DialogResult=DialogResult.OK;
		}

		private void butPinboard_Click(object sender,EventArgs e) {
			if(!ValidateSelection()) {
				return;
			}
			DisplayFormAsapForWebSched();
			FormApptBreakSelection=ApptBreakSelection.Pinboard;
			DialogResult=DialogResult.OK;
		}

		private void butApptBook_Click(object sender,EventArgs e) {
			if(!ValidateSelection()) {
				return;
			}
			FormApptBreakSelection=ApptBreakSelection.ApptBook;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			FormApptBreakSelection=ApptBreakSelection.None;
			DialogResult=DialogResult.Cancel;
		}

		private void FormApptBreak_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult!=DialogResult.OK) {
				return;
			}
			SelectedProcCode=radioMissed.Checked?ProcedureCodes.GetProcCode("D9986"):ProcedureCodes.GetProcCode("D9987");
		}
	}

	public enum ApptBreakSelection {
		///<summary>0 - Default.</summary>
		None,
		///<summary>1</summary>
		Unsched,
		///<summary>2</summary>
		Pinboard,
		///<summary>3</summary>
		ApptBook,
		///<summary>4</summary>
		Delete,
	}

}