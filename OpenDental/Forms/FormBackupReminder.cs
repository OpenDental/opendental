using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBackupReminder:FormODBase {
		public FormBackupReminder() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBackupReminder_Load(object sender,EventArgs e) {
			labelSupplementalBackupDisabled.Visible=!PrefC.GetBool(PrefName.SupplementalBackupEnabled);
			labelSupplementalBackupPath.Visible=string.IsNullOrEmpty(PrefC.GetString(PrefName.SupplementalBackupNetworkPath));
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!checkBackupMethodOnline.Checked
				&& !checkBackupMethodRemovable.Checked
				&& !checkBackupMethodNetwork.Checked
				&& !checkBackupMethodOther.Checked)
			{
				MsgBox.Show(this,"You are not allowed to continue using this program unless you are making daily backups.");
				return;
			}
			if(!checkRestoreHome.Checked
				&& !checkRestoreServer.Checked)
			{
				MsgBox.Show(this,"You are not allowed to continue using this program unless you have proof that your backups are good.");
				return;
			}
			if(!checkSecondaryMethodArchive.Checked
				&& !checkSecondaryMethodHardCopy.Checked)
			{
				MsgBox.Show(this,"You are not allowed to continue using this program unless you have a long-term strategy.");
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void FormBackupReminder_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult!=DialogResult.OK){//clicked on X at upper right
				MsgBox.Show(this,"Please answer the questions, then click OK.");
				e.Cancel=true;
			}
		}



	}
}