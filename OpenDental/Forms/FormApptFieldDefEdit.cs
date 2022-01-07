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
	public partial class FormApptFieldDefEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		private ApptFieldDef _apptFieldDef;
		private string _fieldNameOld;

		///<summary></summary>
		public FormApptFieldDefEdit(ApptFieldDef apptFieldDef)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_apptFieldDef=apptFieldDef;
		}

		private void FormApptFieldDefEdit_Load(object sender,System.EventArgs e) {
			textName.Text=_apptFieldDef.FieldName;
			textPickList.Visible=false;
			comboFieldType.Items.Clear();
			comboFieldType.Items.AddList<string>(Enum.GetNames(typeof(ApptFieldType)));
			comboFieldType.SelectedIndex=(int)_apptFieldDef.FieldType;
			if(!IsNew){
				_fieldNameOld=_apptFieldDef.FieldName;
			}
			if(comboFieldType.SelectedIndex==(int)ApptFieldType.PickList) {
				textPickList.Visible=true;
				labelWarning.Visible=true;
				textPickList.Text=_apptFieldDef.PickList;
			}
		}

		private void comboFieldType_SelectedIndexChanged(object sender,EventArgs e) {
			textPickList.Visible=false;
			labelWarning.Visible=false;
			if(comboFieldType.SelectedIndex==(int)ApptFieldType.PickList) {
				textPickList.Visible=true;
				labelWarning.Visible=true;
			}
		}

		private void buttonDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			try{
				ApptFieldDefs.Delete(_apptFieldDef);//Throws if in use.
				FieldDefLinks.DeleteForFieldDefNum(_apptFieldDef.ApptFieldDefNum,FieldDefTypes.Appointment);//Delete any FieldDefLinks to this _apptFieldDef
				DialogResult=DialogResult.OK;
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(_fieldNameOld!=textName.Text) {
				if(ApptFieldDefs.GetExists(x => x.FieldName==textName.Text)) {
					MsgBox.Show(this,"Field name currently being used.");
					return;
				}
			}
			_apptFieldDef.FieldName=textName.Text;
			_apptFieldDef.FieldType=(ApptFieldType)comboFieldType.SelectedIndex;
			if(_apptFieldDef.FieldType==ApptFieldType.PickList) {
			  if(textPickList.Text=="") {
			    MsgBox.Show(this,"List cannot be blank.");
			    return;
			  }
			  _apptFieldDef.PickList=textPickList.Text;
			}
			if(IsNew) {
			  ApptFieldDefs.Insert(_apptFieldDef);
			}
			else {
			  ApptFieldDefs.Update(_apptFieldDef,_fieldNameOld);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	

		


	}
}





















