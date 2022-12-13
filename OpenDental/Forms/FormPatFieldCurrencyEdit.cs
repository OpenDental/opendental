using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPatFieldCurrencyEdit:FormODBase {
		public bool IsNew;
		private PatField _patField;
		private PatField _patFieldOld;

		public FormPatFieldCurrencyEdit(PatField patField) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patField=patField;
			_patFieldOld=_patField.Copy();
		}

		private void FormPatFieldCurrencyEdit_Load(object sender,EventArgs e) {
			labelName.Text=_patField.FieldName;
			textFieldCurrency.Text=_patField.FieldValue;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textFieldCurrency.IsValid()) {
				MsgBox.Show(this,"Invalid currency");
				return;
			}			
			_patField.FieldValue=textFieldCurrency.Text;
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

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}