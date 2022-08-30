using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEClipboardSecurityEdit : FormODBase {
		public int SecurityFrequency=0;

		public FormEClipboardSecurityEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEClipboardSecurityEdit_Load(object sender,EventArgs e) {
			if(SecurityFrequency>0) {
				textFrequency.Text=SecurityFrequency.ToString();
			}
			checkSecurityEnabled.Checked=SecurityFrequency>0;
			textFrequency.Enabled=checkSecurityEnabled.Checked;
		}

		private void checkSecurityEnabled_CheckedChanged(object sender,EventArgs e) {
			textFrequency.Enabled=checkSecurityEnabled.Checked;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(checkSecurityEnabled.Checked) {
				if(!int.TryParse(textFrequency.Text,out SecurityFrequency)) {
					MsgBox.Show("Frequency is not a valid number.");
					return;
				}
			}
			else {
				SecurityFrequency=-1;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}