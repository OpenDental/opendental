using System;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;


namespace OpenDental.User_Controls.SetupWizard {
	public partial class UserControlSetupWizFeatures:SetupWizControl {
		public UserControlSetupWizFeatures() {
			InitializeComponent();
			this.OnControlDone += ControlDone;
		}

		private void UserControlSetupWizFeatures_Load(object sender,EventArgs e) {
			RefreshControls();
		}

		private void RefreshControls() {
			checkCapitation.Checked=!PrefC.GetBool(PrefName.EasyHideCapitation);
			checkMedicaid.Checked=!PrefC.GetBool(PrefName.EasyHideMedicaid);
			checkInsurance.Checked=!PrefC.GetBool(PrefName.EasyHideInsurance);
			checkClinical.Checked=!PrefC.GetBool(PrefName.EasyHideClinical);
			checkNoClinics.Checked=PrefC.HasClinicsEnabled;
			checkMedicalIns.Checked=PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance);
			checkEhr.Checked=PrefC.GetBool(PrefName.ShowFeatureEhr);
			IsDone=true;
		}

		private void labelInfo_MouseClick(object sender,MouseEventArgs e) {
			panelInfo.Controls.OfType<Label>().ToList().ForEach(x => x.ImageIndex=0);
			//foreach(Control item in panelInfo.Controls) {
			//	if(item.GetType() == typeof(Label)) {
			//		((Label)item).ImageIndex = 0;
			//	}
			//}
			((Label)sender).ImageIndex = 1;
			labelExplanation.Text = (string)((Label)sender).Tag;
		}

		private void butAdvanced_Click(object sender,EventArgs e) {
			using FormShowFeatures FormFS=new FormShowFeatures();
			FormFS.ShowDialog();
			RefreshControls();
		}

		private void ControlDone(object sender, EventArgs e) {
			if(
				Prefs.UpdateBool(PrefName.EasyHideCapitation,!checkCapitation.Checked)
				| Prefs.UpdateBool(PrefName.EasyHideMedicaid,!checkMedicaid.Checked)
				| Prefs.UpdateBool(PrefName.EasyHideInsurance,!checkInsurance.Checked)
				| Prefs.UpdateBool(PrefName.EasyHideClinical,!checkClinical.Checked)
				| Prefs.UpdateBool(PrefName.EasyNoClinics,!checkNoClinics.Checked)
				| Prefs.UpdateBool(PrefName.ShowFeatureMedicalInsurance,checkMedicalIns.Checked)
				| Prefs.UpdateBool(PrefName.ShowFeatureEhr,checkEhr.Checked)
			) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
	}
}
