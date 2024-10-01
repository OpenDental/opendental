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
	public partial class FormPatFieldDateEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		private PatField _patField;
		private PatField _patFieldOld;

		///<summary></summary>
		public FormPatFieldDateEdit(PatField patField)
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

		private void FormPatFieldDateEdit_Load(object sender, System.EventArgs e) {
			labelName.Text=_patField.FieldName;
			textFieldDate.Text=_patField.FieldValue;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textFieldDate.IsValid()) {
				MsgBox.Show(this,"Invalid date");
				return;
			}
			_patField.FieldValue=textFieldDate.Text;
			if(_patField.FieldValue==""){//if blank, then delete
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
			if(IsNew){
				PatFields.Insert(_patField);
			}
			else{
				PatFields.Update(_patField);
				PatFields.MakeEditLogEntry(_patFieldOld,_patField);
			}
			DialogResult=DialogResult.OK;
		}

	}
}