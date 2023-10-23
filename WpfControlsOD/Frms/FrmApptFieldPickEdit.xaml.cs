using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmApptFieldPickEdit:FrmODBase {
		private ApptField _field;

		public FrmApptFieldPickEdit(ApptField field) {
			InitializeComponent();
			//Lan.F(this);
			_field=field;
			Load+=FrmApptFieldPickEdit_Load;
		}

		private void FrmApptFieldPickEdit_Load(object sender,EventArgs e) {
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
					IsDialogOK=false;
					return;
				}
				ApptFields.DeleteFieldForAppt(_field.FieldName,_field.AptNum);
			}
			else {
				ApptFields.Upsert(_field);
			}
			IsDialogOK=true;
		}

	}
}