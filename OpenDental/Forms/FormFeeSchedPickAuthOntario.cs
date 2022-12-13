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

		private const string ONTARIO_DENTAL_ASSOCIATION="ODA";
		private const string BRITISH_COLUMBIA_DENTAL_ASSOCIATION="BCDA";

		private string _dentalAssociationName;

		public string getODAMemberNumber(){
			return textODAMemberNumber.Text;
		}

		public string getODAMemberPassword() {
			return textODAMemberPassword.Text;
		}

		public FormFeeSchedPickAuthOntario(string dentalAssociation) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_dentalAssociationName=dentalAssociation;
			if(string.IsNullOrWhiteSpace(_dentalAssociationName)) {
				_dentalAssociationName=ONTARIO_DENTAL_ASSOCIATION;//default to Ontario
			}
			Text=$"Fee Schedule Authorization for {(_dentalAssociationName==BRITISH_COLUMBIA_DENTAL_ASSOCIATION ? "British Columbia" : "Ontario")}";
		}

		private void FormFeeSchedPickAuthOntario_Load(object sender,EventArgs e) {
			if(_dentalAssociationName==ONTARIO_DENTAL_ASSOCIATION) {
				textODAMemberNumber.Text=PrefC.GetString(PrefName.CanadaODAMemberNumber);
				textODAMemberPassword.Text=PrefC.GetString(PrefName.CanadaODAMemberPass);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textODAMemberNumber.Text=="") {
				MsgBox.Show(this,$"{_dentalAssociationName} Member Number cannot be blank.");
				return;
			}
			if(textODAMemberPassword.Text=="") {
				MsgBox.Show(this,$"{_dentalAssociationName} Member Password cannot be blank.");
				return;
			}
			if(_dentalAssociationName==ONTARIO_DENTAL_ASSOCIATION) {
				Prefs.UpdateString(PrefName.CanadaODAMemberNumber,textODAMemberNumber.Text);
				Prefs.UpdateString(PrefName.CanadaODAMemberPass,textODAMemberPassword.Text);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}