using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInCaseOfEmergency: FormODBase {
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label labelName;
		private ValidPhone textPhone;
		private System.Windows.Forms.Label labelPhone;
		private ODtextBox textNote;
		public bool IsNew;
		private PatField _patField;
		private PatField _patFieldOld;
		private System.Windows.Forms.Label label1;

		public FormInCaseOfEmergency(PatField patField) 
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patField=patField;
			_patFieldOld=_patField.Copy();
	}

		private void FormInCaseOfEmergency_Load(object sender,EventArgs e) {
			_patField.IsNew=IsNew;
			string fieldValue="";
			if(_patField.FieldValue==null) {
				fieldValue="";
			}
			else {
				fieldValue=_patField.FieldValue;
			}
			string[] stringArrayFieldvalue=fieldValue.Split(new string[] { "\r\n" },StringSplitOptions.None);
			for(int i=0;i<stringArrayFieldvalue.Length;i++) {
				if(i==0) {//name
					textName.Text=stringArrayFieldvalue[i];
				}
				else if(i==1) {//phone
					textPhone.Text=stringArrayFieldvalue[i];
				}
				else {//note
					textNote.Text=stringArrayFieldvalue[i];
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			_patField.FieldValue=textName.Text+"\r\n"+textPhone.Text+"\r\n"+textNote.Text;
			if(string.IsNullOrWhiteSpace(_patField.FieldValue)) {//if blank, then delete
				if(IsNew) {
					DialogResult=DialogResult.Cancel;
					return;
				}
				PatFields.Delete(_patField);
				if(!string.IsNullOrWhiteSpace(_patFieldOld.FieldValue)) {
					PatFields.MakeDeleteLogEntry(_patFieldOld);
				}
				DialogResult=DialogResult.OK;
				return;
			}
			if(IsNew) {
				PatFields.Insert(_patField);
			}
			else {
				PatFields.Update(_patField);
				PatFields.MakeEditLogEntry(_patFieldOld,_patField);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}