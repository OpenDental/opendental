using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPatFieldEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public bool IsLaunchedFromOrtho;
		private PatField _patField;
		private PatField _patFieldOld;

		///<summary></summary>
		public FormPatFieldEdit(PatField patField)
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

		private void FormPatFieldEdit_Load(object sender, System.EventArgs e) {
			labelName.Text=_patField.FieldName;
			textValue.Text=_patField.FieldValue;
			if(IsLaunchedFromOrtho) {
				butUseAutoNote.Visible=true;
			}
		}

		private void butUseAutoNote_Click(object sender,EventArgs e) {
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			frmAutoNoteCompose.ShowDialog();
			if(frmAutoNoteCompose.IsDialogOK) {
				textValue.AppendText(frmAutoNoteCompose.StrCompletedNote);
			}
		}

		/*private void buttonDelete_Click(object sender,EventArgs e) {
			
		}*/

		private void butOK_Click(object sender, System.EventArgs e) {
			_patField.FieldValue=textValue.Text;
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

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPatFieldDefEdit_FormClosing(object sender,FormClosingEventArgs e) {
			/*if(DialogResult==DialogResult.OK){
				return;
			}
			if(IsNew) {
				PatFields.Delete(Field);
			}*/
		}
	

		


	}
}





















