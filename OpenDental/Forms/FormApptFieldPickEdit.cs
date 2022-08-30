using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormApptFieldPickEdit:FormODBase {
		private ApptField _field;

		public FormApptFieldPickEdit(ApptField field) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_field=field;
		}

		private void FormApptFieldPickEdit_Load(object sender,EventArgs e) {
			labelName.Text=_field.FieldName;
			string value=ApptFieldDefs.GetPickListByFieldName(_field.FieldName);
			string[] valueArray=value.Split(new string[] { "\r\n" },StringSplitOptions.None);
			foreach(string s in valueArray) {
				listBoxPick.Items.Add(s);
			}
			if(!_field.IsNew) {
				listBoxPick.SelectedItem=_field.FieldValue;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listBoxPick.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item in the list first.");
				return;
			}
			_field.FieldValue=listBoxPick.SelectedItem.ToString();
			if(_field.FieldValue=="") {//If blank, then delete
				if(_field.IsNew) {
					DialogResult=DialogResult.Cancel;
					return;
				}
				ApptFields.DeleteFieldForAppt(_field.FieldName,_field.AptNum);
			}
			else {
				ApptFields.Upsert(_field);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}