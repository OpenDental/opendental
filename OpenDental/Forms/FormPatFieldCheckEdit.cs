using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPatFieldCheckEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		private PatField _patField;
		private PatField _patFieldOld;

		///<summary></summary>
		public FormPatFieldCheckEdit(PatField patField)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patField=patField;
			_patFieldOld=_patField.Copy();
		}

		private void FormPatFieldCheckEdit_Load(object sender, System.EventArgs e) {
			labelName.Text=_patField.FieldName;
			checkFieldValue.Checked=PIn.Bool(_patField.FieldValue);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!checkFieldValue.Checked){//if blank, then delete
				if(IsNew) {
					DialogResult=DialogResult.Cancel;
					return;
				}
				PatFields.Delete(_patField);
				if(_patFieldOld.FieldValue!="") {//We don't need to make a log for field values that were blank because the user simply clicked cancel.
					PatFields.MakeDeleteLogEntry(_patFieldOld);
				}
				DialogResult=DialogResult.OK;
				return;
			}
			_patField.FieldValue="1";
			if(IsNew){
				PatFields.Insert(_patField);
			}
			else{
				//this should never happen
				PatFields.Update(_patField);
				PatFields.MakeEditLogEntry(_patFieldOld,_patField);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPatFieldDefEdit_FormClosing(object sender,FormClosingEventArgs e) {
			
		}

	

		


	}
}





















