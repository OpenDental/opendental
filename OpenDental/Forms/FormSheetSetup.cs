using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSheetSetup:FormODBase {
		private bool changed;

		public FormSheetSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReportSetup_Load(object sender,EventArgs e) {
			checkPatientFormsShowConsent.Checked=PrefC.GetBool(PrefName.PatientFormsShowConsent);
		}
		
		private void butOK_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.PatientFormsShowConsent,checkPatientFormsShowConsent.Checked)
				) {
				changed=true;
			}
			if(changed) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}