using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormEServicesMobileSettings:FormODBase {
		private WebServiceMainHQProxy.MobileSettingsAuth _mobileSettingsAuthIn;
		private WebServiceMainHQProxy.MobileSettingsAuth _mobileSettingsAuthOut;
		private long _clinicNum;
		private bool _isNew;

		public FormEServicesMobileSettings() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		#region Events
		private void FormEServicesMobileSettings_Load(object sender,EventArgs e) {
			_mobileSettingsAuthOut=new WebServiceMainHQProxy.MobileSettingsAuth();
			_clinicNum=Clinics.ClinicNum;
			FillForm();
		}

		private void comboBoxClinicPicker1_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!CanDiscardChanges()) {
				comboBoxClinicPicker1.ClinicNumSelected=_clinicNum;
				return;
			}
			_clinicNum=comboBoxClinicPicker1.ClinicNumSelected;
			FillForm();
		}

		private void butVerifyAndSave_Click(object sender,EventArgs e) {
			string errMsg=ValidateForm();
			if(!string.IsNullOrEmpty(errMsg)) {
				MessageBox.Show(errMsg);
				return;
			}
			GetAuthOutFromForm();
			WebServiceMainHQProxy.MobileSettingsAuth mobileSettingAuth;
			try {
				mobileSettingAuth=WebServiceMainHQProxy.GetMobileSettings2FA(_mobileSettingsAuthOut,!_isNew);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"There was an issue with sending the 2FA code:")+" "+ex.Message);
				return;
			}
			//Validate against the 2FA code
			using FormEServices2FactorAuthentication formEServices2FactorAuthentication=new FormEServices2FactorAuthentication();
			formEServices2FactorAuthentication.MobileSettingsAuth=mobileSettingAuth;
			formEServices2FactorAuthentication.ShowDialog();
			if(formEServices2FactorAuthentication.DialogResult!=DialogResult.OK) {
				return;
			}
			string password=textPassword.Text;
			try {
				string result=WebServiceMainHQProxy.UpsertMobileSettings(password,_mobileSettingsAuthOut,!_isNew);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"There was an issue updating your account:")+" "+ex.Message);
				return;
			}
			//If there isn't a clinic selector, just close the form since they're done.
			if(!PrefC.HasClinicsEnabled) {
				DialogResult=DialogResult.OK;//see FormClosing
				return;
			}
			_isNew=false;
			_mobileSettingsAuthIn.Email=_mobileSettingsAuthOut.Email;
			_mobileSettingsAuthIn.PhoneNumber=_mobileSettingsAuthOut.PhoneNumber;
			_mobileSettingsAuthIn.UserName=_mobileSettingsAuthOut.UserName;
			SetElementsVisible();
		}
		#endregion

		#region Methods

		private void FillForm() {
			try {
				_mobileSettingsAuthIn=WebServiceMainHQProxy.GetMobileSettings(_clinicNum);
			}
			catch (Exception e) {
				MessageBox.Show(Lan.g(this,"There was an issue updating your account:")+" "+e.Message);
				DialogResult=DialogResult.OK;//see FormClosing
				return;
			}
			_mobileSettingsAuthOut.RegKeyNum=_mobileSettingsAuthIn.RegKeyNum;
			textUserName.Text=_mobileSettingsAuthIn.UserName;
			textEmail.Text=_mobileSettingsAuthIn.Email;
			textValidPhone.Text=_mobileSettingsAuthIn.PhoneNumber;
			textPassword.Text="";
			textConfirmPassword.Text="";
			_isNew=string.IsNullOrWhiteSpace(_mobileSettingsAuthIn.UserName);
			SetElementsVisible();
		}

		private void GetAuthOutFromForm() {
			_mobileSettingsAuthOut.Email=PIn.String(textEmail.Text);
			_mobileSettingsAuthOut.PhoneNumber=textValidPhone.Text;
			_mobileSettingsAuthOut.UserName=textUserName.Text;
			_mobileSettingsAuthOut.ClinicNum=_clinicNum;
		}

		private void SetElementsVisible() {
			if(!Security.IsAuthorized(EnumPermType.EServicesSetup,suppressMessage:true)) {
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
			Regex regex=new Regex("[A-Z]");
			if(regex.Matches(password).Count==0) {
				return "Password must contain at least 1 uppercase letter";
			}
			regex=new Regex("\\d");
			if(regex.Matches(password).Count==0) {
				return "Password must contain at least 1 number";
			}
			regex=new Regex("[@$!%*#?&_]");
			if(regex.Matches(password).Count==0) {
				return "Password must contain at least 1 special character";
			}
			return "";
		}

		private string ValidateUserName(string userName) {
			if(userName.Length<6) {
				return "User Name must be at least 6 characters long";
			}
			Regex regex=new Regex("^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
			if(regex.Matches(userName).Count>0) {
				return "User Name cannot be an email address";
			}
			return "";
		}
		#endregion

		private bool CanDiscardChanges() {
			GetAuthOutFromForm();
			if(!_isNew 
				&& (_mobileSettingsAuthOut.Email!=_mobileSettingsAuthIn.Email || _mobileSettingsAuthOut.PhoneNumber!=_mobileSettingsAuthIn.PhoneNumber || _mobileSettingsAuthOut.UserName!=_mobileSettingsAuthIn.UserName)) 
			{
				return MsgBox.Show(this,MsgBoxButtons.YesNo,"Unsaved changes will be discarded. Continue?");
			}
			return true;
		}
		#endregion

		private void FormEServicesMobileSettings_FormClosing(object sender,FormClosingEventArgs e) {
			//There are two places above where DialogResult is set to OK.
			//In both cases, the original code was written to kick out here instead of checking CanDiscardChanges.
			//So we are doing the same thing.
			if(DialogResult!=DialogResult.Cancel) {
				return;
			}
			//Click X or Esc:
			if(!CanDiscardChanges()) {
				e.Cancel=true;
			}
		}
	}
}