using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormUserPassword : FormODBase {
		private bool IsCreate;
		public bool IsInSecurityWindow;
		public bool PasswordIsStrong;
		public string PasswordTyped;
		public PasswordContainer LoginDetails;
		private bool _isPasswordReset;
		private bool _isCopiedUser;

		///<summary>Set isPasswwordReset to true if creating rather than changing a password.</summary>
		public FormUserPassword(bool isCreate,string username, bool isPasswordReset=false,bool isCopiedUser=false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			IsCreate=isCreate;
			textUserName.Text=username;
			_isPasswordReset=isPasswordReset;
			_isCopiedUser=isCopiedUser;
		}

		private void FormUserPassword_Load(object sender, System.EventArgs e) {
			if(IsCreate){
				Text=Lan.g(this,"Create Password");
			}
			if(_isCopiedUser) {
				Text=Lan.g(this,"Create Password for Copied User");
			}
			if(IsInSecurityWindow) {
				labelCurrent.Visible=false;
				textCurrent.Visible=false;
			}
			if(_isPasswordReset) {
				labelCurrent.Text="New Password";
				labelNew.Text="Re-Enter Password";
			}
		}

		private void checkShow_Click(object sender,EventArgs e) {
			//char ch=textPassword.PasswordChar;
			if(checkShow.Checked) {
				textPassword.PasswordChar='\0';
			}
			else {
				textPassword.PasswordChar='*';
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(_isPasswordReset) {
				if(textPassword.Text!=textCurrent.Text || string.IsNullOrWhiteSpace(textPassword.Text)) {
					MsgBox.Show(this,"Passwords must match and not be empty.");
					return;
				}
			}
			else if(!IsInSecurityWindow
				&& !Authentication.CheckPassword(Security.CurUser,textCurrent.Text))
			{
				MsgBox.Show(this,"Current password incorrect.");
				return;
			}
			string explanation=Userods.IsPasswordStrong(textPassword.Text);
			if(PrefC.GetBool(PrefName.PasswordsMustBeStrong)) {
				if(explanation!="") {
					MessageBox.Show(explanation);
					return;
				}
			}
			//If the PasswordsMustBeStrong preference is off, still store whether or not the password is strong in case the preference is turned on later
			PasswordIsStrong=string.IsNullOrEmpty(explanation);
			if(Programs.UsingEcwTightOrFullMode()) {//Same check as FormLogOn
				LoginDetails=Authentication.GenerateLoginDetails(textPassword.Text,HashTypes.MD5_ECW);
			}
			else {
				LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
			}
			PasswordTyped=textPassword.Text; //update the stored typed password for middle tier refresh
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















