using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormEServicesMobileSettings:FormODBase {
		WebServiceMainHQProxy.MobileSettingsAuth authIn;
		WebServiceMainHQProxy.MobileSettingsAuth authOut;
		private long _selectedClinic;
		private bool _isNew;

		public FormEServicesMobileSettings() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		#region Events
		private void FormEServicesMobileSettings_Load(object sender,EventArgs e) {
			authOut=new WebServiceMainHQProxy.MobileSettingsAuth();
			_selectedClinic=Clinics.ClinicNum;
			FillForm();
		}

		private void comboBoxClinicPicker1_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!CanDiscardChanges()) {
				comboBoxClinicPicker1.SelectedClinicNum=_selectedClinic;
				return;
			}
			_selectedClinic=comboBoxClinicPicker1.SelectedClinicNum;
			FillForm();
		}

		private void butVerifyAndSave_Click(object sender,EventArgs e) {
			string errMsg=ValidateForm();
			if(!string.IsNullOrEmpty(errMsg)) {
				MessageBox.Show(errMsg);
				return;
			}
			GetAuthOutFromForm();
			WebServiceMainHQProxy.MobileSettingsAuth authValidate;
			try {
				authValidate=WebServiceMainHQProxy.GetMobileSettings2FA(authOut,!_isNew);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"There was an issue with sending the 2FA code:")+" "+ex.Message);
				return;
			}
			//Validate against the 2FA code
			using FormEServices2FactorAuthentication form2FA=new FormEServices2FactorAuthentication();
			form2FA.auth=authValidate;
			form2FA.ShowDialog();
			if(form2FA.DialogResult!=DialogResult.OK) {
				return;
			}
			string password=textPassword.Text;
			try {
				string result=WebServiceMainHQProxy.UpsertMobileSettings(password,authOut,!_isNew);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"There was an issue updating your account:")+" "+ex.Message);
				return;
			}
			//If there isn't a clinic selector, just close the form since they're done.
			if(!PrefC.HasClinicsEnabled) {
				DialogResult=DialogResult.OK;
				return;
			}
			_isNew=false;
			authIn.Email=authOut.Email;
			authIn.PhoneNumber=authOut.PhoneNumber;
			authIn.UserName=authOut.UserName;
			SetElementsVisible();
		}
		#endregion

		#region Methods

		private void FillForm() {
			try {
				authIn=WebServiceMainHQProxy.GetMobileSettings(_selectedClinic);
			}
			catch (Exception e) {
				MessageBox.Show(Lan.g(this,"There was an issue updating your account:")+" "+e.Message);
				DialogResult=DialogResult.Cancel;
				return;
			}
			authOut.RegKeyNum=authIn.RegKeyNum;
			textUserName.Text=authIn.UserName;
			textEmail.Text=authIn.Email;
			textValidPhone.Text=authIn.PhoneNumber;
			textPassword.Text="";
			textConfirmPassword.Text="";
			_isNew=string.IsNullOrWhiteSpace(authIn.UserName);
			SetElementsVisible();
		}

		private void GetAuthOutFromForm() {
			authOut.Email=PIn.String(textEmail.Text);
			authOut.PhoneNumber=textValidPhone.Text;
			authOut.UserName=textUserName.Text;
			authOut.ClinicNum=_selectedClinic;
		}

		private void SetElementsVisible() {
			if(!Security.IsAuthorized(Permissions.EServicesSetup,true)) {
				//If no permissions, don't let them modify anything.
				butVerifyAndSave.Enabled=false;
				textUserName.Enabled=false;
				textPassword.Enabled=false;
				textConfirmPassword.Enabled=false;
				textValidPhone.Enabled=false;
				textEmail.Enabled=false;
				labelPermissionRequired.Visible=true;
				return;
			}
			if(_isNew) {
				textUserName.Enabled=true;
				labelPassword.Visible=true;
				textPassword.Visible=true;
				labelConfirmPassword.Visible=true;
				textConfirmPassword.Visible=true;
			}
			else {
				textUserName.Enabled=false;
				labelPassword.Visible=false;
				textPassword.Visible=false;
				labelConfirmPassword.Visible=false;
				textConfirmPassword.Visible=false;
			}
		}

		#region Validation Helpers
		private string ValidateForm() {
			if(string.IsNullOrWhiteSpace(textEmail.Text)) {
				return "Email cannot be blank.\r\n";
			}
			if(_isNew) {
				if(textPassword.Text!=textConfirmPassword.Text) {
					return "Passwords must match.\r\n";
				}
				string errMsg=ValidatePassword(textPassword.Text);
				if(!string.IsNullOrWhiteSpace(errMsg)) {
					return errMsg;
				}
				errMsg=ValidateUserName(textUserName.Text);
				if(!string.IsNullOrWhiteSpace(errMsg)) {
					return errMsg;
				}
			}
			return "";
		}

		private string ValidatePassword(string password) {
			if(password.Length<6) {
				return "Password must be at least 6 characters long";
			}
			Regex regExp=new Regex("[A-Z]");
			if(regExp.Matches(password).Count==0) {
				return "Password must contain at least 1 uppercase letter";
			}
			regExp=new Regex("\\d");
			if(regExp.Matches(password).Count==0) {
				return "Password must contain at least 1 number";
			}
			regExp=new Regex("[@$!%*#?&_]");
			if(regExp.Matches(password).Count==0) {
				return "Password must contain at least 1 special character";
			}
			return "";
		}

		private string ValidateUserName(string userName) {
			if(userName.Length<6) {
				return "User Name must be at least 6 characters long";
			}
			Regex regExp=new Regex("^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
			if(regExp.Matches(userName).Count>0) {
				return "User Name cannot be an email address";
			}
			return "";
		}
		#endregion

		private bool CanDiscardChanges() {
			GetAuthOutFromForm();
			if(!_isNew && (authOut.Email!=authIn.Email || authOut.PhoneNumber!=authIn.PhoneNumber || authOut.UserName!=authIn.UserName)) {
				return MsgBox.Show(this,MsgBoxButtons.YesNo,"Unsaved changes will be discarded. Continue?");
			}
			return true;
		}
		#endregion

		private void butCancel_Click(object sender,EventArgs e) {
			if(CanDiscardChanges()) {
				DialogResult=DialogResult.Cancel;
			}
		}
	}
}