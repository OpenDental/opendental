using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormChangeCloudPassword:FormODBase {

		public FormChangeCloudPassword() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormChangeCloudPassword_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
		}

		private void checkShow_CheckedChanged(object sender,EventArgs e) {
			textNewPass.UseSystemPasswordChar=!checkShow.Checked;
		}

		private void butChange_Click(object sender,EventArgs e) {
			string newPass=textNewPass.Text;
			if(newPass.Length < 8) {
				MsgBox.Show(this,"Password must be at least 8 characters long.");
				return;
			}
			if(!newPass.Any(x => char.IsNumber(x))) {
				MsgBox.Show(this,"Password must contain at least one number.");
				return;
			}
			if(newPass.All(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x))) {
				MsgBox.Show(this,"Password must contain at least one special character.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
				//This will change the password for the local user that OpenDental.exe is running under.
				using PrincipalContext context=new PrincipalContext(ContextType.Machine);
				using UserPrincipal user=UserPrincipal.FindByIdentity(context,IdentityType.SamAccountName,Environment.UserName);
				user.ChangePassword(textOldPass.Text,newPass);
			}
			catch(Exception ex) {
				FriendlyException.Show("Unable to update password: "+ex.Message,ex);
				return;
			}
			finally {
				Cursor=Cursors.Default;
			}
			SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Changed Cloud office password.");
			Prefs.UpdateInt(PrefName.CloudPasswordNeedsReset,(int)YN.No);//No refresh needed because this is only checked on startup.
			MsgBox.Show(this,"Password changed.");
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}