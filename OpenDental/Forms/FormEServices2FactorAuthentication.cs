using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEServices2FactorAuthentication:FormODBase {
		public WebServiceMainHQProxy.MobileSettingsAuth MobileSettingsAuth;

		public FormEServices2FactorAuthentication() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(text2FactorAuthCode.Text!=MobileSettingsAuth.AuthCodeEmail && text2FactorAuthCode.Text!=MobileSettingsAuth.AuthCodePhone) {
				MessageBox.Show("The given code did not match. Enter a valid code, or hit Cancel to send a new code.");
				return;
			}
			DialogResult=DialogResult.OK;
		}

	}
}