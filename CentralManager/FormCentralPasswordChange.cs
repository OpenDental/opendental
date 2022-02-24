using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace CentralManager {
	public partial class FormCentralPasswordChange:Form {

		public FormCentralPasswordChange() {
			InitializeComponent();
		}

		private void FormCentralPasswordChange_Load(object sender,EventArgs e) {
			
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textAccessCode.Text!="I'm admin") {
				MessageBox.Show("Access code is incorrect.");
				return;
			}
			Prefs.UpdateString(PrefName.CentralManagerPassHash,Authentication.HashPasswordMD5(textPassword.Text));
			Prefs.RefreshCache();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}