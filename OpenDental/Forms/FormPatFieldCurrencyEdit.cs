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
		private PatField _fieldCur;
		private PatField _fieldOld;

		public FormPatFieldCurrencyEdit(PatField field) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_fieldCur=field;
			_fieldOld=_fieldCur.Copy();
		}

		private void FormPatFieldCurrencyEdit_Load(object sender,EventArgs e) {
			labelName.Text=_fieldCur.FieldName;
			textFieldCurrency.Text=_fieldCur.FieldValue;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textFieldCurrency.IsValid()) {
				MsgBox.Show(this,"Invalid currency");
				return;
			}			
			_fieldCur.FieldValue=textFieldCurrency.Text;
			if(_fieldCur.FieldValue==""){//if blank, then delete
				if(IsNew) {
					DialogResult=DialogResult.Cancel;
					return;
				}
				PatFields.Delete(_fieldCur);
				if(_fieldOld.FieldValue!="") {//We don't need to make a log for field values that were blank because the user simply clicked cancel.
					PatFields.MakeDeleteLogEntry(_fieldOld);
				}
				DialogResult=DialogResult.OK;
				return;
			}
			if(IsNew){
				PatFields.Insert(_fieldCur);
			}
			else{
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