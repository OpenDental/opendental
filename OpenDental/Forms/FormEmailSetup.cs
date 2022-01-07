using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormEmailSetup : FormODBase {

		///<summary></summary>
		public FormEmailSetup(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			Lan.C(this, new System.Windows.Forms.Control[]
			{
				textBox6,
				textBox1
			});
		}

		private void FormEmailSetup_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,true)){
				textPassword.PasswordChar='*';
			}
			textSMTPserver.Text=PrefC.GetString(PrefName.EmailSMTPserver);
			textUsername.Text=PrefC.GetString(PrefName.EmailUsername);
			textPassword.Text=PrefC.GetString(PrefName.EmailPassword);
			textPort.Text=PrefC.GetString(PrefName.EmailPort);
			checkSSL.Checked=PrefC.GetBool(PrefName.EmailUseSSL);
			textSender.Text=PrefC.GetString(PrefName.EmailSenderAddress);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			Prefs.UpdateString(PrefName.EmailSMTPserver,textSMTPserver.Text);
			Prefs.UpdateString(PrefName.EmailUsername,textUsername.Text);
			Prefs.UpdateString(PrefName.EmailPassword,textPassword.Text);
			try{
				Prefs.UpdateLong(PrefName.EmailPort,PIn.Long(textPort.Text));
			}
			catch{
				MsgBox.Show(this,"invalid port number.");
			}
			Prefs.UpdateBool(PrefName.EmailUseSSL,checkSSL.Checked);
			Prefs.UpdateString(PrefName.EmailSenderAddress,textSender.Text);
			DataValid.SetInvalid(InvalidType.Prefs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
