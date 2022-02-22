using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPatFieldDefEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public PatFieldDef FieldDef;
		private string OldFieldName;

		///<summary></summary>
		public FormPatFieldDefEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatFieldDefEdit_Load(object sender, System.EventArgs e) {
			textName.Text=FieldDef.FieldName;
			textPickList.Visible=false;
			textName.ReadOnly=false;
			comboFieldType.Items.Clear();
			//InCaseOfEmergency DEPRECATED. (Only used 16.3.1,deprecated by 16.3.4)
			List<PatFieldType> listType=Enum.GetValues(typeof(PatFieldType)).Cast<PatFieldType>().Where(x => x!=PatFieldType.InCaseOfEmergency).ToList();
			comboFieldType.Items.Clear();
			comboFieldType.Items.AddList(listType,x => x.GetDescription());
			comboFieldType.SetSelectedEnum(FieldDef.FieldType);
			if(!IsNew){
				OldFieldName=FieldDef.FieldName;
				checkHidden.Checked=FieldDef.IsHidden;
			}
			if(comboFieldType.GetSelected<PatFieldType>()==PatFieldType.PickList) {
				textPickList.Visible=true;
				labelWarning.Visible=true;
				textPickList.Text=FieldDef.PickList;
			}
			if(comboFieldType.GetSelected<PatFieldType>()==PatFieldType.CareCreditStatus) {
				comboFieldType.Enabled=false;
			}
		}

		private void comboFieldType_SelectedIndexChanged(object sender,EventArgs e) {
			if(!IsNew){
				//todo: check existing values to make sure that it makes sense to change the type.  Especially when moving to currency or date.
			}
			textPickList.Visible=false;
			labelWarning.Visible=false;
			textName.ReadOnly=false;
			if(comboFieldType.GetSelected<PatFieldType>()==PatFieldType.PickList) {
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
				//if Deleting carecredit type and it will leave 0 care credit types, don't allow.
				if(FieldDef.FieldType==PatFieldType.CareCreditStatus 
					&& PatFieldDefs.GetDeepCopy(isShort:true)
						.Where(x => x.PatFieldDefNum!=FieldDef.PatFieldDefNum && x.FieldType==PatFieldType.CareCreditStatus).Count()==0) 
				{
					MsgBox.Show(this,"Not allowed to delete because this is the last field def with the type CareCreditStatus.");
					return;
				}
				PatFieldDefs.Delete(FieldDef);//Throws if in use.
				FieldDefLinks.DeleteForFieldDefNum(FieldDef.PatFieldDefNum,FieldDefTypes.Patient);//Delete any FieldDefLinks to this PatFieldDef
				FieldDef=null;
				DialogResult=DialogResult.OK;
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(OldFieldName!=textName.Text && !IsNew) {
				if(FieldDef.FieldType==PatFieldType.CareCreditStatus && PatFields.IsFieldNameInUse(OldFieldName)) {
					MsgBox.Show(this,"CareCredit field name currently being used. Cannot rename.");
					textName.Text=OldFieldName;
					return;
				}
				if(!PrefC.GetBool(PrefName.DisplayRenamedPatFields)
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"Changing the field name will cause existing information for this field to be hidden.  Continue?")) 
				{
					textName.Text=OldFieldName;
					return;
				}
				if(PatFieldDefs.GetExists(x => x.FieldName==textName.Text)) {
					MsgBox.Show(this,"Field name currently being used.");
					return;
				}
			}
			FieldDef.FieldName=textName.Text;
			FieldDef.IsHidden=checkHidden.Checked;
			FieldDef.FieldType=comboFieldType.GetSelected<PatFieldType>();
			if(FieldDef.FieldType==PatFieldType.PickList) {
				if(textPickList.Text=="") {
					MsgBox.Show(this,"List cannot be blank.");
					return;
				}
				FieldDef.PickList=textPickList.Text;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	

		


	}
}





















