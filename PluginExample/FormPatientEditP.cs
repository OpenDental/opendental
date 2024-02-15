using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental;

namespace PluginExample {
	public partial class FormPatientEditP:Form {
		public Patient PatientCur;
		private Patient _patientOld;

		public FormPatientEditP() {
			InitializeComponent();
		}

		private void FormPatientEdit_Load(object sender,EventArgs e) {
			_patientOld=PatientCur.Copy();
			textPreferred.Text=PatientCur.Preferred;
		}

		private void butOK_Click(object sender,EventArgs e) {
			PatientCur.Preferred=textPreferred.Text;
			Patients.Update(PatientCur,_patientOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
