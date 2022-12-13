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
	public partial class FormPatFieldPickEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		private PatField _patField;
		private PatField _patFieldOld;

		///<summary></summary>
		public FormPatFieldPickEdit(PatField patField)
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

		private void FormPatFieldPickEdit_Load(object sender, System.EventArgs e) {
			labelName.Text=_patField.FieldName;
			string value="";
			value=PatFieldDefs.GetPickListByFieldName(_patField.FieldName);
			string[] stringArrayValues=value.Split(new string[] { "\r\n" },StringSplitOptions.None);
			listBoxPick.Items.AddList(stringArrayValues,x => x.ToString());
			if(!IsNew) {
				listBoxPick.SelectedItem=_patField.FieldValue;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listBoxPick.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item in the list first.");
				return;
			}
			_patField.FieldValue=listBoxPick.SelectedItem.ToString();
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





















