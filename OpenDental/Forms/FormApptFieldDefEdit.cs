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
		private ApptFieldDef FieldDef;
		private string OldFieldName;

		///<summary></summary>
		public FormApptFieldDefEdit(ApptFieldDef fieldDef)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			FieldDef=fieldDef;
		}

		private void FormApptFieldDefEdit_Load(object sender,System.EventArgs e) {
			textName.Text=FieldDef.FieldName;
			textPickList.Visible=false;
			comboFieldType.Items.Clear();
			comboFieldType.Items.AddRange(Enum.GetNames(typeof(ApptFieldType)));
			comboFieldType.SelectedIndex=(int)FieldDef.FieldType;
			if(!IsNew){
				OldFieldName=FieldDef.FieldName;
			}
			if(comboFieldType.SelectedIndex==(int)ApptFieldType.PickList) {
				textPickList.Visible=true;
				labelWarning.Visible=true;
				textPickList.Text=FieldDef.PickList;
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
				ApptFieldDefs.Delete(FieldDef);//Throws if in use.
				FieldDefLinks.DeleteForFieldDefNum(FieldDef.ApptFieldDefNum,FieldDefTypes.Appointment);//Delete any FieldDefLinks to this ApptFieldDef
				DialogResult=DialogResult.OK;
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(OldFieldName!=textName.Text) {
				if(ApptFieldDefs.GetExists(x => x.FieldName==textName.Text)) {
					MsgBox.Show(this,"Field name currently being used.");
					return;
				}
			}
			FieldDef.FieldName=textName.Text;
			FieldDef.FieldType=(ApptFieldType)comboFieldType.SelectedIndex;
			if(FieldDef.FieldType==ApptFieldType.PickList) {
			  if(textPickList.Text=="") {
			    MsgBox.Show(this,"List cannot be blank.");
			    return;
			  }
			  FieldDef.PickList=textPickList.Text;
			}
			if(IsNew) {
			  ApptFieldDefs.Insert(FieldDef);
			}
			else {
			  ApptFieldDefs.Update(FieldDef,OldFieldName);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	

		


	}
}





















