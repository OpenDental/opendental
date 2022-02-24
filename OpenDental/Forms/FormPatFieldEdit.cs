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
		private PatField _fieldCur;
		private PatField _fieldOld;

		///<summary></summary>
		public FormPatFieldEdit(PatField field)
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

		private void FormPatFieldEdit_Load(object sender, System.EventArgs e) {
			labelName.Text=_fieldCur.FieldName;
			textValue.Text=_fieldCur.FieldValue;
			if(IsLaunchedFromOrtho) {
				butUseAutoNote.Visible=true;
			}
		}

		private void butUseAutoNote_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textValue.AppendText(FormA.CompletedNote);
			}
		}

		/*private void buttonDelete_Click(object sender,EventArgs e) {
			
		}*/

		private void butOK_Click(object sender, System.EventArgs e) {
			_fieldCur.FieldValue=textValue.Text;
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
			/*if(DialogResult==DialogResult.OK){
				return;
			}
			if(IsNew) {
				PatFields.Delete(Field);
			}*/
		}
	

		


	}
}





















