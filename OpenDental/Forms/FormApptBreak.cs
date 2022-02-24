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

		private void PromptTextASAPList() {
			if(!PrefC.GetBool(PrefName.WebSchedAsapEnabled) || Appointments.RefreshASAP(0,0,_appt.ClinicNum,new List<ApptStatus>()).Count==0
				|| !MsgBox.Show("Appointment",MsgBoxButtons.YesNo,"Text patients on the ASAP List and offer them this opening?")) 
			{
				return;
			}
			DateTime dateTimeSlotStart;
			DateTime dateTimeSlotEnd;
			dateTimeSlotStart=_appt.AptDateTime.Date;//Midnight
			dateTimeSlotEnd=_appt.AptDateTime.Date.AddDays(1);//Midnight tomorrow
			//Loop through all other appts in the op to find a slot that will not overlap.
			List<Appointment> listApptsInOp=Appointments.GetAppointmentsForOpsByPeriod(new List<long> { _appt.Op },_appt.AptDateTime);
			foreach(Appointment otherAppt in listApptsInOp.Where(x => x.AptNum!=_appt.AptNum)) {
				DateTime dateEndApt=otherAppt.AptDateTime.AddMinutes(otherAppt.Pattern.Length*5);
				if(dateEndApt.Between(dateTimeSlotStart,_appt.AptDateTime)) {
					dateTimeSlotStart=dateEndApt;
				}
				if(otherAppt.AptDateTime.Between(_appt.AptDateTime,dateTimeSlotEnd)) {
					dateTimeSlotEnd=otherAppt.AptDateTime;
				}
			}
			dateTimeSlotStart=ODMathLib.Max(dateTimeSlotStart,_appt.AptDateTime.AddHours(-1));
			dateTimeSlotEnd=ODMathLib.Min(dateTimeSlotEnd,_appt.AptDateTime.AddHours(3));
			using FormASAP formASAP=new FormASAP(_appt.AptDateTime,dateTimeSlotStart,dateTimeSlotEnd,_appt.Op);
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
			PromptTextASAPList();
			FormApptBreakSelection=ApptBreakSelection.Unsched;
			DialogResult=DialogResult.OK;
		}

		private void butPinboard_Click(object sender,EventArgs e) {
			if(!ValidateSelection()) {
				return;
			}
			PromptTextASAPList();
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
			if(DialogResult==DialogResult.Cancel) {
				FormApptBreakSelection=ApptBreakSelection.None;//if used X at upper right
			}
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