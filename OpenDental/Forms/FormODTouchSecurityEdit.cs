using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormODTouchSecurityEdit : FormODBase {
		private int _securityFrequency=0;

		public FormODTouchSecurityEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEClipboardSecurityEdit_Load(object sender,EventArgs e) {
			//Disable all controls if user does not have EServicesSetup permission.
			if(!Security.IsAuthorized(EnumPermType.EServicesSetup,suppressMessage:true)) {
				DisableAllExcept();
			}
			//This has been made into a global (non-clinic) pref, per Nathan/Sean/Sam.
			_securityFrequency=PrefC.GetInt(PrefName.EClipboardClinicalValidationFrequency);
			if(_securityFrequency>0) {
				textFrequency.Text=_securityFrequency.ToString();
			}
			checkSecurityEnabled.Checked=_securityFrequency>0;
			textFrequency.Enabled=checkSecurityEnabled.Checked;
		}

		private void checkSecurityEnabled_CheckedChanged(object sender,EventArgs e) {
			textFrequency.Enabled=checkSecurityEnabled.Checked;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(checkSecurityEnabled.Checked) {
				if(!int.TryParse(textFrequency.Text,out _securityFrequency)) {
					MsgBox.Show("Frequency is not a valid number.");
					return;
				}
			}
			else {
				_securityFrequency=-1;
			}
			Prefs.UpdateInt(PrefName.EClipboardClinicalValidationFrequency,_securityFrequency);
			DialogResult=DialogResult.OK;
		}

	}
}