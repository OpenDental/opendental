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
		public static bool ChangePassword(bool isForcedLogOff,bool doRefreshSecurityCache=true) {
			//no security blocking because everyone is allowed to change their own password.
			if(Security.CurUser.UserNumCEMT!=0) {
				MsgBox.Show("FormOpenDental","Use the CEMT tool to change your password.");
				return false;
			}
			using FormUserPassword FormU=new FormUserPassword(false,Security.CurUser.UserName);
			FormU.ShowDialog();
			if(FormU.DialogResult==DialogResult.Cancel) {
				if(isForcedLogOff) {
					FormOpenDental FormOD=Application.OpenForms.OfType<FormOpenDental>().ToList()[0];//There always should be exactly 1.
					FormOD.LogOffNow(true);
				}
				return false;
			}
			bool isPasswordStrong=FormU.PasswordIsStrong;
			try {
				Userods.UpdatePassword(Security.CurUser,FormU.LoginDetails,isPasswordStrong);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return false;
			}
			Security.CurUser.PasswordIsStrong=FormU.PasswordIsStrong;
			Security.CurUser.LoginDetails=FormU.LoginDetails;
			Security.PasswordTyped=FormU.PasswordTyped;
			if(doRefreshSecurityCache) {
				DataValid.SetInvalid(InvalidType.Security);
			}
			return true;
		}

	}
}
