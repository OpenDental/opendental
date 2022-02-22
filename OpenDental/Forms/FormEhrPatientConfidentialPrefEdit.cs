using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrPatientConfidentialPrefEdit:FormODBase {
		public Patient PatCur;
		///<summary>A copy of the original patient object, as it was when this form was first opened.</summary>
		private Patient PatOld;

		public FormEhrPatientConfidentialPrefEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormPatientConfidentialPrefEdit_Load(object sender,EventArgs e) {
			for(int i=0;i<Enum.GetNames(typeof(ContactMethod)).Length;i++) {
				comboConfidentialContact.Items.Add(Enum.GetNames(typeof(ContactMethod))[i]);
			}			
			PatOld=PatCur.Copy();
			comboConfidentialContact.SelectedIndex=(int)PatCur.PreferContactConfidential;	
		}

		private void butOK_Click(object sender,EventArgs e) {
			PatCur.PreferContactConfidential=(ContactMethod)comboConfidentialContact.SelectedIndex;
			Patients.Update(PatCur,PatOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


		

	

	


	}
}
