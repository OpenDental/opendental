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
		private PatField _fieldCur;
		private PatField _fieldOld;

		///<summary></summary>
		public FormPatFieldPickEdit(PatField field)
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

		private void FormPatFieldPickEdit_Load(object sender, System.EventArgs e) {
			labelName.Text=_fieldCur.FieldName;
			string value="";
			value=PatFieldDefs.GetPickListByFieldName(_fieldCur.FieldName);
			string[] valueArray=value.Split(new string[] { "\r\n" },StringSplitOptions.None);
			listBoxPick.Items.AddList(valueArray,x => x.ToString());
			if(!IsNew) {
				listBoxPick.SelectedItem=_fieldCur.FieldValue;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listBoxPick.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item in the list first.");
				return;
			}
			_fieldCur.FieldValue=listBoxPick.SelectedItem.ToString();
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





















