using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
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
				DialogResult=DialogResult.Cancel;
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
			using PrincipalContext principalContext=new PrincipalContext(ContextType.Machine);
			if(principalContext==null) {
				MsgBox.Show(this,"No valid server or domain could be found.");
				return;
			}
			using UserPrincipal userPrincipal=UserPrincipal.FindByIdentity(principalContext,IdentityType.SamAccountName,Environment.UserName);
			if(userPrincipal==null) {
				MsgBox.Show(this,"No principals could be found for this user account.");
				return;
			}
			try {
				//This will change the password for the local user that OpenDental.exe is running under.
				userPrincipal.ChangePassword(textOldPass.Text,newPass);
				ChangePasswordLBE(newPass);
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

		///<summary>Checks if cloud setup is a load balanced environment and updates the cloud servers.</summary>
		private static void ChangePasswordLBE(string newPass) {
			string scriptPath=$"C:\\Scripts\\ChangePasswordLBE.ps1";
			//If the virtual machine has this script, then its an LBE
			if(!File.Exists(scriptPath)) {
				return;
			}
			string command=$"{scriptPath} {newPass} {Environment.UserName}";
			string	powerShellCommand=$"/C powershell -ExecutionPolicy Bypass -File {command}";
			using Process process=new Process();
			ProcessStartInfo startInfo=new ProcessStartInfo("CMD.exe") {
				CreateNoWindow=true,
				UseShellExecute=false,
				Arguments=powerShellCommand,
				RedirectStandardError=true
			};
			process.StartInfo=startInfo;
			process.Start();
			process.WaitForExit();
			if(process.ExitCode!=0) {
				FriendlyException.Show(process.StandardError.ReadLine(),new Exception(process.StandardError.ReadToEnd()));
			}
			return;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}