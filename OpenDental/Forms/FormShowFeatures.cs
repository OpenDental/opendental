using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental{
	///<summary></summary>
	public partial class FormShowFeatures : FormODBase {
		private bool _isClinicsEnabledInDb=false;

		///<summary></summary>
		public FormShowFeatures()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormShowFeatures_Load(object sender, System.EventArgs e) {
			checkCapitation.Checked=!PrefC.GetBool(PrefName.EasyHideCapitation);
			checkMedicaid.Checked=!PrefC.GetBool(PrefName.EasyHideMedicaid);
			checkPublicHealth.Checked=!PrefC.GetBool(PrefName.EasyHidePublicHealth);
			checkDentalSchools.Checked=!PrefC.GetBool(PrefName.EasyHideDentalSchools);
			checkHospitals.Checked=!PrefC.GetBool(PrefName.EasyHideHospitals);
			checkInsurance.Checked=!PrefC.GetBool(PrefName.EasyHideInsurance);
			checkClinical.Checked=!PrefC.GetBool(PrefName.EasyHideClinical);
			checkBasicModules.Checked=PrefC.GetBool(PrefName.EasyBasicModules);
			_isClinicsEnabledInDb=!PrefC.GetBool(PrefName.EasyNoClinics);
			RestoreClinicCheckBox();
			checkRepeatCharges.Checked=!PrefC.GetBool(PrefName.EasyHideRepeatCharges);
			checkMedicalIns.Checked=PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance);
			checkEhr.Checked=PrefC.GetBool(PrefName.ShowFeatureEhr);
			checkSuperFam.Checked=PrefC.GetBool(PrefName.ShowFeatureSuperfamilies);
			checkPatClone.Checked=PrefC.GetBool(PrefName.ShowFeaturePatientClone);
			checkShowEnterprise.Checked=PrefC.GetBool(PrefName.ShowFeatureEnterprise);
			checkShowReactivations.Checked=PrefC.GetBool(PrefName.ShowFeatureReactivations);
			checkEraShowControlId.Checked=PrefC.GetBool(PrefName.EraShowControlIdFilter);
			bool isLateChargesFeatureActive=PrefC.GetBool(PrefName.ShowFeatureLateCharges);
			if(isLateChargesFeatureActive) {
				radioLateCharges.Checked=true;
				radioBillingFinance.Checked=false;
			}
			else {
				radioBillingFinance.Checked=true;
				radioLateCharges.Checked=false;
			}
		}

		private void checkRepeatCharges_Click(object sender,EventArgs e) {
			if(checkRepeatCharges.Checked) {
				MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
			}
		}

		private void checkEnableClinics_Click(object sender,EventArgs e) {
			if (!GetPasswordFromUser()) {
				RestoreClinicCheckBox();
				return;
			}
			string question="If you are subscribed to eServices, you may need to restart the eConnector when you turn clinics on or off. Continue?";
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,question)) {
				RestoreClinicCheckBox();
			}
		}

		private bool GetPasswordFromUser() {
			using InputBox inputBoxPassword=new InputBox("To prevent accidental changes, please enter password.  It can be found in our manual.");
			inputBoxPassword.ShowDialog();
			if(inputBoxPassword.DialogResult!=DialogResult.OK) {
				return false;
			}
			if(inputBoxPassword.textResult.Text!="abracadabra") {
				MsgBox.Show(this,"Incorrect password.");
				return false;
			}
			return true;
		}

		private void checkEhr_Click(object sender,EventArgs e) {
			if(checkEhr.Checked && !File.Exists(ODFileUtils.CombinePaths(Application.StartupPath,"EHR.dll"))){
				checkEhr.Checked=false;
				MsgBox.Show(this,"EHR.dll could not be found.");
				return;
			}
			MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
		}

		private void checkPublicHealth_Click(object sender,EventArgs e) {
			if(checkPublicHealth.Checked) {
				MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
			}
		}

		private void checkRestart_Click(object sender,EventArgs e) {
			MsgBox.Show("You will need to restart the program for the change to take effect.");
		}

		///<summary>Restores checkEnableClinics to original value when form was opened.</summary>
		private void RestoreClinicCheckBox() {
			checkEnableClinics.Checked=_isClinicsEnabledInDb;
		}

		///<summary>Validates that PrefName.EasyNoClinics is ok to be changed and changes it when necessary. Sends an alert to eConnector to perform the conversion.
		///If fails then restores checkEnableClinics to original value when form was opened.</summary>
		private bool IsClinicCheckBoxOk() {
			if(_isClinicsEnabledInDb==checkEnableClinics.Checked) { //No change.
				return true;
			}
			try {
				//Turn clinics on/off locally and send the signal to other workstations. This must happen before we call HQ so we tell HQ the new value.
				Prefs.UpdateBool(PrefName.EasyNoClinics,!checkEnableClinics.Checked);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			catch(Exception ex) {
				//Change it back to what the db has.
				RestoreClinicCheckBox();
				MessageBox.Show(ex.Message);
				return false;
			}	
			//Create an alert for the user to know they may need to restart the eConnector if they are subscribed to eServices
			AlertItem alertItem=new AlertItem();
			alertItem.Description=Lan.g(this,"Clinic Feature Changed, you may need to restart the eConnector if you are subscribed to eServices");
			alertItem.Type=AlertType.ClinicsChanged;
			alertItem.Severity=SeverityType.Low;
			alertItem.Actions=ActionType.OpenForm | ActionType.MarkAsRead | ActionType.Delete;
			alertItem.FormToOpen=FormType.FormEServicesEConnector;
			alertItem.ItemValue="Clinics turned "+(checkEnableClinics.Checked ? "On":"Off");
			AlertItems.Insert(alertItem);
			//Create an alert for the eConnector to perform the clinic conversion as needed.
			alertItem=new AlertItem();
			alertItem.Description="Clinics Changed";
			alertItem.Type=AlertType.ClinicsChangedInternal;
			alertItem.Severity=SeverityType.Normal;
			alertItem.Actions=ActionType.None;
			alertItem.ItemValue=checkEnableClinics.Checked ? "On":"Off";
			AlertItems.Insert(alertItem);
			return true;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!IsClinicCheckBoxOk()) {
				return;
			}
			bool hasChanged=false;
			hasChanged |= Prefs.UpdateBool(PrefName.EasyHideCapitation,!checkCapitation.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EasyHideMedicaid,!checkMedicaid.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EasyHidePublicHealth,!checkPublicHealth.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EasyHideDentalSchools,!checkDentalSchools.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EasyHideHospitals,!checkHospitals.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EasyHideInsurance,!checkInsurance.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EasyHideClinical,!checkClinical.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EasyBasicModules,checkBasicModules.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EasyHideRepeatCharges,!checkRepeatCharges.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.ShowFeatureMedicalInsurance,checkMedicalIns.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.ShowFeatureEhr,checkEhr.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.ShowFeatureSuperfamilies,checkSuperFam.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.ShowFeaturePatientClone,checkPatClone.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.ShowFeatureEnterprise,checkShowEnterprise.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.ShowFeatureReactivations,checkShowReactivations.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.EraShowControlIdFilter,checkEraShowControlId.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.ShowFeatureLateCharges,radioLateCharges.Checked);
			if(hasChanged) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(_isClinicsEnabledInDb!=checkEnableClinics.Checked) {
				MsgBox.Show(this,"You will need to restart the program for the change to take effect.");
			}
			//We should use ToolBut invalidation to redraw toolbars that could've been just enabled and stop forcing customers restarting.
			//DataValid.SetInvalid(InvalidType.ToolBut);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
