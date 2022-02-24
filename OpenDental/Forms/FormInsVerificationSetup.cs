using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInsVerificationSetup:FormODBase {
		private bool _hasChanged;

		public FormInsVerificationSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsVerificationSetup_Load(object sender,EventArgs e) {
			textInsBenefitEligibilityDays.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDays));
			textPatientEnrollmentDays.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDays));
			textScheduledAppointmentDays.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyAppointmentScheduledDays));
			textPastDueDays.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyDaysFromPastDueAppt));
			checkInsVerifyUseCurrentUser.Checked=PrefC.GetBool(PrefName.InsVerifyDefaultToCurrentUser);
			checkInsVerifyExcludePatVerify.Checked=PrefC.GetBool(PrefName.InsVerifyExcludePatVerify);
			checkFutureDateBenefitYear.Checked=PrefC.GetBool(PrefName.InsVerifyFutureDateBenefitYear);
			if(!PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				checkExcludePatientClones.Visible=false;
			}
			else {
				checkExcludePatientClones.Checked=PrefC.GetBool(PrefName.InsVerifyExcludePatientClones);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textInsBenefitEligibilityDays.IsValid()) {
				MsgBox.Show(this,"The number entered for insurance benefit eligibility was not a valid number.  Please enter a valid number to continue.");
				return;
			}
			if(!textPatientEnrollmentDays.IsValid()) {
				MsgBox.Show(this,"The number entered for patient enrollment was not a valid number.  Please enter a valid number to continue.");
				return;
			}
			if(!textScheduledAppointmentDays.IsValid()) {
				MsgBox.Show(this,"The number entered for scheduled appointments was not a valid number.  Please enter a valid number to continue.");
				return;
			}
			if(!textPastDueDays.IsValid()) {
				MsgBox.Show(this,"The number entered for appointment days past due was not a valid number.  Please enter a valid number to continue.");
				return;
			}
			int insBenefitEligibilityDays=PIn.Int(textInsBenefitEligibilityDays.Text);
			int patientEnrollmentDays=PIn.Int(textPatientEnrollmentDays.Text);
			int scheduledAppointmentDays=PIn.Int(textScheduledAppointmentDays.Text);
			int pastDueDays=PIn.Int(textPastDueDays.Text);
			if(Prefs.UpdateInt(PrefName.InsVerifyBenefitEligibilityDays,insBenefitEligibilityDays)
				| Prefs.UpdateInt(PrefName.InsVerifyPatientEnrollmentDays,patientEnrollmentDays)
				| Prefs.UpdateInt(PrefName.InsVerifyAppointmentScheduledDays,scheduledAppointmentDays)
				| Prefs.UpdateInt(PrefName.InsVerifyDaysFromPastDueAppt,pastDueDays)
				| Prefs.UpdateBool(PrefName.InsVerifyExcludePatVerify,checkInsVerifyExcludePatVerify.Checked)
				| Prefs.UpdateBool(PrefName.InsVerifyExcludePatientClones,checkExcludePatientClones.Checked)
				| Prefs.UpdateBool(PrefName.InsVerifyFutureDateBenefitYear,checkFutureDateBenefitYear.Checked)
				| Prefs.UpdateBool(PrefName.InsVerifyDefaultToCurrentUser,checkInsVerifyUseCurrentUser.Checked)) 
			{
				_hasChanged=true;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormInsVerificationSetup_FormClosing(object sender,FormClosingEventArgs e) {
			if(_hasChanged) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
	}
}