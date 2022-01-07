using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailSetupEHR:FormODBase {
		public FormEmailSetupEHR() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmailSetupEHR_Load(object sender,EventArgs e) {
			textPOPserver.Text=PrefC.GetString(PrefName.EHREmailPOPserver);
			textUsername.Text=PrefC.GetString(PrefName.EHREmailFromAddress);
			textPassword.Text=PrefC.GetString(PrefName.EHREmailPassword);
			textPort.Text=PrefC.GetString(PrefName.EHREmailPort);
		}

		private void butOK_Click(object sender,EventArgs e) {
			Prefs.UpdateString(PrefName.EHREmailPOPserver,textPOPserver.Text);
			Prefs.UpdateString(PrefName.EHREmailFromAddress,textUsername.Text);
			Prefs.UpdateString(PrefName.EHREmailPassword,textPassword.Text);
			try{
				Prefs.UpdateLong(PrefName.EHREmailPort,PIn.Long(textPort.Text));
			}
			catch{
				MsgBox.Show(this,"invalid port number.");
			}
			DataValid.SetInvalid(InvalidType.Prefs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}