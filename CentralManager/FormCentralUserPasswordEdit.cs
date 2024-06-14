using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CentralManager {
	public partial class FormCentralUserPasswordEdit:Form {
		public string HashedResult;
		public bool IsInSecurityWindow;
		private bool _isCreate;
		public bool IsPassWordStrong=false;
		public PasswordContainer LoginDetails;
		public string PasswordTyped;

		public FormCentralUserPasswordEdit(bool isCreate,string userName) {
			InitializeComponent();
			_isCreate=isCreate;
			textUserName.Text=userName;
		}

		private void FormCentralUserPasswordEdit_Load(object sender,EventArgs e) {
			if(_isCreate){
				Text="Create Password";
			}
			if(IsInSecurityWindow) {
				labelCurrent.Visible=false;
				textCurrent.Visible=false;
			}
		}

		private void checkShow_Click(object sender,EventArgs e) {
			if(checkShow.Checked) {
				textPassword.PasswordChar='\0';
			}
			else {
				textPassword.PasswordChar='*';
			}
		}
		
		//<summary>Mimics FormUserPassoword.butOk_Click()<summary>
		private void butOK_Click(object sender,EventArgs e) {
			Userod user=Userods.GetUserByName(textUserName.Text,false);
			if(!_isCreate && !IsInSecurityWindow) {
				string userPassCur="";
				if(user!=null) {
					userPassCur=user.PasswordHash;
				}
				//If user's current password is blank we dont care what they put for the old one.
				if(userPassCur!="" && !Authentication.CheckPassword(user,textCurrent.Text)) {
					MessageBox.Show(this,"Current password incorrect.");
					return;
				}
			}
			if(textPassword.Text==""){
				MessageBox.Show(this,"Passwords cannot be blank.");
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
			IsPassWordStrong=string.IsNullOrEmpty(explanation);
			PasswordTyped=textPassword.Text;//Update the last typed in for middle tier refresh
			if(Programs.UsingEcwTightOrFullMode()) {//Same check as FormLogOn
				LoginDetails=Authentication.GenerateLoginDetails(PasswordTyped,HashTypes.MD5_ECW);
			}
			else {
				LoginDetails=Authentication.GenerateLoginDetailsSHA512(PasswordTyped);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
