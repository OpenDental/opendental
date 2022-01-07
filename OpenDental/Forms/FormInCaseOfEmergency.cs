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
		private PatField _fieldCur;
		private PatField _fieldOld;
		private System.Windows.Forms.Label label1;

		public FormInCaseOfEmergency(PatField field) 
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_fieldCur=field;
			_fieldOld=_fieldCur.Copy();
	}

		private void FormInCaseOfEmergency_Load(object sender,EventArgs e) {
			_fieldCur.IsNew=IsNew;
			string value="";
			if(_fieldCur.FieldValue==null) {
				value="";
			}
			else {
				value=_fieldCur.FieldValue;
			}
			string[] valueArray=value.Split(new string[] { "\r\n" },StringSplitOptions.None);
			for(int i=0;i<valueArray.Length;i++) {
				if(i==0) {//name
					textName.Text=valueArray[i];
				}
				else if(i==1) {//phone
					textPhone.Text=valueArray[i];
				}
				else {//note
					textNote.Text=valueArray[i];
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			_fieldCur.FieldValue=textName.Text+"\r\n"+textPhone.Text+"\r\n"+textNote.Text;
			if(string.IsNullOrWhiteSpace(_fieldCur.FieldValue)) {//if blank, then delete
				if(IsNew) {
					DialogResult=DialogResult.Cancel;
					return;
				}
				PatFields.Delete(_fieldCur);
				if(!string.IsNullOrWhiteSpace(_fieldOld.FieldValue)) {
					PatFields.MakeDeleteLogEntry(_fieldOld);
				}
				DialogResult=DialogResult.OK;
				return;
			}
			if(IsNew) {
				PatFields.Insert(_fieldCur);
			}
			else {
				PatFields.Update(_fieldCur);
				PatFields.MakeEditLogEntry(_fieldOld,_fieldCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}