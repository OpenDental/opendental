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
		private PatField _fieldCur;
		private PatField _fieldOld;

		///<summary></summary>
		public FormPatFieldDateEdit(PatField field)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_fieldCur=field;
			_fieldOld=_fieldCur.Copy();
		}

		private void FormPatFieldDateEdit_Load(object sender, System.EventArgs e) {
			labelName.Text=_fieldCur.FieldName;
			textFieldDate.Text=_fieldCur.FieldValue;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textFieldDate.IsValid()) {
				MsgBox.Show(this,"Invalid date");
				return;
			}
			_fieldCur.FieldValue=textFieldDate.Text;
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

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPatFieldDefEdit_FormClosing(object sender,FormClosingEventArgs e) {
			
		}

	

		


	}
}





















