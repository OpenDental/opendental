using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public class SecurityL {
		
		///<summary>Called to change the password for Security.CurUser.
		///Returns true if password was changed successfully.
		///Set isForcedLogOff to force the program to log the user off if they cancel out of the Change Password window.</summary>
		public static bool ChangePassword(bool isForcedLogOff,bool willRefreshSecurityCache=true) {
			//no security blocking because everyone is allowed to change their own password.
			if(Security.CurUser.UserNumCEMT!=0) {
				MsgBox.Show("FormOpenDental","Use the CEMT tool to change your password.");
				return false;
			}
			using FormUserPassword formUserPassword=new FormUserPassword(isCreate:false,Security.CurUser.UserName);
			formUserPassword.ShowDialog();
			if(formUserPassword.DialogResult==DialogResult.Cancel) {
				if(isForcedLogOff) {
					FormOpenDental formOpenDental=Application.OpenForms.OfType<FormOpenDental>().ToList()[0];//There always should be exactly 1.
					formOpenDental.LogOffNow(true);
				}
				return false;
			}
			bool isPasswordStrong=formUserPassword.IsPasswordStrong;
			try {
				Userods.UpdatePassword(Security.CurUser,formUserPassword.PasswordContainer_,isPasswordStrong);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return false;
			}
			Security.CurUser.PasswordIsStrong=formUserPassword.IsPasswordStrong;
			Security.CurUser.SetPassword(formUserPassword.PasswordContainer_);
			Security.PasswordTyped=formUserPassword.PasswordTyped;
			if(willRefreshSecurityCache) {
				DataValid.SetInvalid(InvalidType.Security);
			}
			return true;
		}

	}
}
