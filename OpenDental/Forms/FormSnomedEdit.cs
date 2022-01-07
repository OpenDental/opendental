using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Users are not allowed to edit or delete individual SNOMED codes so this is really a view only form.</summary>
	public partial class FormSnomedEdit:FormODBase {
		private Snomed SnomedCur;
		//public bool IsNew;

		public FormSnomedEdit(Snomed snomedCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			SnomedCur=snomedCur;
		}

		private void FormSnomedEdit_Load(object sender,EventArgs e) {
			//if(!IsNew) {
			//	textCode.Enabled=false;
			//}
			textCode.Text=SnomedCur.SnomedCode;
			textDescription.Text=SnomedCur.Description;
		}
/*
		private void buttonDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try {
				Snomeds.Delete(SnomedCur.SnomedNum);
				DialogResult=DialogResult.OK;
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
			}
		}

		 Users are not allowed to add or edit SNOMED codes.
		private void butOK_Click(object sender,EventArgs e) {
			SnomedCur.SnomedCode=textCode.Text;
			SnomedCur.Description=textDescription.Text;
			if(IsNew) {//Used the "+Add" button to open this form.
				if(ICD9s.CodeExists(SnomedCur.SnomedCode)) {//Must enter a unique code.
					MsgBox.Show(this,"You must choose a unique code.");
					return;
				}
				Snomeds.Insert(SnomedCur);
			}
			else {
				Snomeds.Update(SnomedCur);
			}
			DialogResult=DialogResult.OK;
		}
		*/

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}