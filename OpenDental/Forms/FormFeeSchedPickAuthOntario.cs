using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormFeeSchedPickAuthOntario:FormODBase {

		public string ODAMemberNumber {
			get {
				return textODAMemberNumber.Text;
			}
		}

		public string ODAMemberPassword {
			get {
				return textODAMemberPassword.Text;
			}
		}

		public FormFeeSchedPickAuthOntario() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFeeSchedPickAuthOntario_Load(object sender,EventArgs e) {
			textODAMemberNumber.Text=PrefC.GetString(PrefName.CanadaODAMemberNumber);
			textODAMemberPassword.Text=PrefC.GetString(PrefName.CanadaODAMemberPass);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textODAMemberNumber.Text=="") {
				MsgBox.Show(this,"ODA Member Number cannot be blank.");
				return;
			}
			if(textODAMemberPassword.Text=="") {
				MsgBox.Show(this,"ODA Member Password cannot be blank.");
				return;
			}
			Prefs.UpdateString(PrefName.CanadaODAMemberNumber,textODAMemberNumber.Text);
			Prefs.UpdateString(PrefName.CanadaODAMemberPass,textODAMemberPassword.Text);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}