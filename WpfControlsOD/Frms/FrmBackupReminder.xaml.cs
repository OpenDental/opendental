using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmBackupReminder:FrmODBase {
		public FrmBackupReminder() {
			InitializeComponent();
			Load+=FrmBackupReminder_Load;
			FormClosing+=FrmBackupReminder_FormClosing;
			PreviewKeyDown+=FrmBackupReminder_PreviewKeyDown;
		}

		private void FrmBackupReminder_Load(object sender,EventArgs e) {
			Lang.F(this);
			labelSupplementalBackupDisabled.Visible=false;
			labelSupplementalBackupPath.Visible=false;
			labelSupplementalBackupDisabled.Visible=!PrefC.GetBool(PrefName.SupplementalBackupEnabled);
			labelSupplementalBackupPath.Visible=string.IsNullOrEmpty(PrefC.GetString(PrefName.SupplementalBackupNetworkPath));
		}

		private void FrmBackupReminder_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(checkBackupMethodOnline.Checked==false
				&& checkBackupMethodRemovable.Checked==false
				&& checkBackupMethodNetwork.Checked==false
				&& checkBackupMethodOther.Checked==false)
			{
				MsgBox.Show(this,"You are not allowed to continue using this program unless you are making daily backups.");
				return;
			}
			if(checkRestoreHome.Checked==false
				&& checkRestoreServer.Checked==false)
			{
				MsgBox.Show(this,"You are not allowed to continue using this program unless you have proof that your backups are good.");
				return;
			}
			if(checkSecondaryMethodArchive.Checked==false
				&& checkSecondaryMethodHardCopy.Checked==false)
			{
				MsgBox.Show(this,"You are not allowed to continue using this program unless you have a long-term strategy.");
				return;
			}
			IsDialogOK=true;
		}

		private void FrmBackupReminder_FormClosing(object sender,CancelEventArgs e) {
			if(!IsDialogOK){//clicked on X at upper right
				MsgBox.Show(this,"Please answer the questions, then click Save.");
				e.Cancel=true;
			}
		}

	}
}