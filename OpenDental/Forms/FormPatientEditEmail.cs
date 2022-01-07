using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPatientEditEmail:FormODBase {
		///<summary>Initilizes to the string passed into the form.  Gets set to what the user entered if DialogResult is OK.</summary>
		public string PatientEmails;
		private readonly int _maxChar=100;

		public FormPatientEditEmail(string email) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			PatientEmails=email;
		}

		private void FormPatientEditEmail_Load(object sender,EventArgs e) {
			textEmail.Text=PatientEmails;
			UpdateCharsRemaining();
		}

		private void FormPatientEditEmail_Shown(object sender,EventArgs e) {
			textEmail.Focus();
			textEmail.Select(PatientEmails.Length,PatientEmails.Length);
		}

		private void UpdateCharsRemaining() {
			int charsRem=Math.Max(_maxChar-textEmail.Text.Length,0);
			labelCharsRemaining.Text=$"Characters remaining: {charsRem}";
			labelCharsRemaining.ForeColor=(charsRem==0 ? Color.Red : Color.Black);
		}

		private void textEmail_TextChanged(object sender,EventArgs e) {
			UpdateCharsRemaining();
		}

		private void butOK_Click(object sender,EventArgs e) {
			PatientEmails=textEmail.Text;
			PatientEmails=PatientEmails.Replace("\r\n","");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}