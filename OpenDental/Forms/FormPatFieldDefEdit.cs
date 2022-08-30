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
		public PatFieldDef PatFieldDefCur;
		private string _fieldNameOld;

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
			textName.Text=PatFieldDefCur.FieldName;
			textPickList.Visible=false;
			textName.ReadOnly=false;
			comboFieldType.Items.Clear();
			//InCaseOfEmergency DEPRECATED. (Only used 16.3.1,deprecated by 16.3.4)
			List<PatFieldType> listPatFieldTypes=Enum.GetValues(typeof(PatFieldType)).Cast<PatFieldType>().Where(x => x!=PatFieldType.InCaseOfEmergency).ToList();
			comboFieldType.Items.Clear();
			comboFieldType.Items.AddList(listPatFieldTypes,x => x.GetDescription());
			comboFieldType.SetSelectedEnum(PatFieldDefCur.FieldType);
			if(!IsNew){
				_fieldNameOld=PatFieldDefCur.FieldName;
				checkHidden.Checked=PatFieldDefCur.IsHidden;
			}
			if(comboFieldType.GetSelected<PatFieldType>()==PatFieldType.PickList) {
				textPickList.Visible=true;
				labelWarning.Visible=true;
				textPickList.Text=PatFieldDefCur.PickList;
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
			//if Deleting carecredit type and it will leave 0 care credit types, don't allow.
			if(PatFieldDefCur.FieldType==PatFieldType.CareCreditStatus 
				&& PatFieldDefs.GetDeepCopy(isShort:true)
					.Where(x => x.PatFieldDefNum!=PatFieldDefCur.PatFieldDefNum && x.FieldType==PatFieldType.CareCreditStatus).Count()==0) 
			{
				MsgBox.Show(this,"Not allowed to delete because this is the last field def with the type CareCreditStatus.");
				return;
			}
			try{
				PatFieldDefs.Delete(PatFieldDefCur);//Throws exception if in use
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			FieldDefLinks.DeleteForFieldDefNum(PatFieldDefCur.PatFieldDefNum,FieldDefTypes.Patient);//Delete any FieldDefLinks to this PatFieldDef
			PatFieldDefCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(_fieldNameOld!=textName.Text && !IsNew) {
				if(PatFieldDefCur.FieldType==PatFieldType.CareCreditStatus && PatFields.IsFieldNameInUse(_fieldNameOld)) {
					MsgBox.Show(this,"CareCredit field name currently being used. Cannot rename.");
					textName.Text=_fieldNameOld;
					return;
				}
				if(!PrefC.GetBool(PrefName.DisplayRenamedPatFields)
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"Changing the field name will cause existing information for this field to be hidden.  Continue?")) 
				{
					textName.Text=_fieldNameOld;
					return;
				}
				if(PatFieldDefs.GetExists(x => x.FieldName==textName.Text)) {
					MsgBox.Show(this,"Field name currently being used.");
					return;
				}
			}
			PatFieldDefCur.FieldName=textName.Text;
			PatFieldDefCur.IsHidden=checkHidden.Checked;
			PatFieldDefCur.FieldType=comboFieldType.GetSelected<PatFieldType>();
			if(PatFieldDefCur.FieldType==PatFieldType.PickList) {
				if(textPickList.Text=="") {
					MsgBox.Show(this,"List cannot be blank.");
					return;
				}
				PatFieldDefCur.PickList=textPickList.Text;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	

		


	}
}





















