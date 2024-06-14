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
using CodeBase;
using WpfControls;

namespace OpenDental {
	/// <summary>The editor is for the EFormField even though we're really editing the EFormFieldDef. This editor is not patient facing.</summary>
	public partial class FrmEFormTextBoxEdit : FrmODBase {
		///<summary>This is the object being edited.</summary>
		public EFormField EFormFieldCur;
		///<summary>All the siblings</summary>
		public List<EFormField> _listEFormFields;

		///<summary></summary>
		public FrmEFormTextBoxEdit() {
			InitializeComponent();
			Load+=FrmEFormsTextBoxEdit_Load;
			PreviewKeyDown+=FrmEFormTextBoxEdit_PreviewKeyDown;
			checkIsHorizontal.Click+=CheckIsHorizontal_Click;
			textVIntWidth.TextChanged+=TextVIntWidth_TextChanged;
		}

		private void FrmEFormsTextBoxEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			textLabel.Text=EFormFieldCur.ValueLabel;
			List<string> listAvailTextBox=EFormFieldsAvailable.GetList_TextBox();
			comboDbLink.Items.AddList(listAvailTextBox);
			int idxSelect=listAvailTextBox.IndexOf(EFormFieldCur.DbLink);
			if(idxSelect==-1) {//this handles "" showing as "None"
				comboDbLink.SelectedIndex=0;//None
			}
			else {
				comboDbLink.SelectedIndex=idxSelect;
			}
			checkIsHorizontal.Checked=EFormFieldCur.IsHorizStacking;
			textVIntWidth.Value=EFormFieldCur.Width;
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			checkIsTextWrap.Checked=EFormFieldCur.IsTextWrap;
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.CondValueStrConverter(_listEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);//This is used to make checkbox values, "X" and "", more user readable by converting them to "Checked" and "Unchecked".
			SetLabelRed();
			textLabel.Focus();
		}

		private void CheckIsHorizontal_Click(object sender,EventArgs e) {
			SetLabelRed();
		}

		private void TextVIntWidth_TextChanged(object sender,EventArgs e) {
			SetLabelRed();
		}

		private void SetLabelRed(){
			if(checkIsHorizontal.Checked==true
				&& textVIntWidth.IsValid()
				&& textVIntWidth.Value==0)
			{
				labelRed.Visible=true;
			}
			else{
				labelRed.Visible=false;
			}
		}

		private void butPickParent_Click(object sender,EventArgs e) {
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=_listEFormFields;
			frmEFormFieldPicker.ListSelectedIndices=new List<int>(_listEFormFields.IndexOf(EFormFieldCur));//can be -1
			//Prevents self selection as parent
			frmEFormFieldPicker.ShowDialog();
			if(frmEFormFieldPicker.IsDialogCancel){
				return;
			}
			textCondParent.Text=frmEFormFieldPicker.LabelSelected;
		}

		private void butPickValue_Click(object sender,EventArgs e) {
			if(textCondParent.Text==""){
				MsgBox.Show("Please enter a name in the Parent field first.");
				return;
			}
			EFormConditionValueSetter conditionValueSetter=EFormL.SetCondValue(_listEFormFields,textCondParent.Text,textCondValue.Text);
			if(conditionValueSetter.ErrorMsg!="") {
				MsgBox.Show(conditionValueSetter.ErrorMsg);
				return;
			}
			textCondValue.Text=conditionValueSetter.SelectedValue;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//no need to verify with user because they have another chance to cancel in the parent window.
			EFormFieldCur=null;
			IsDialogOK=true;
		}

		private void FrmEFormTextBoxEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!textVIntWidth.IsValid()
				|| !textVIntFontScale.IsValid())
			{
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			//If the parent is a radiobutton, they have to select a value.
			EFormField eFormField=_listEFormFields.Find(x=>x.ValueLabel==textCondParent.Text);
			if(eFormField!=null && eFormField.FieldType==EnumEFormFieldType.RadioButtons) {
				if(textCondValue.Text.IsNullOrEmpty()) {
					MsgBox.Show("Please select a value for your parent field.");
					return;
				}
			}
			//end of validation
			EFormFieldCur.ValueLabel=textLabel.Text;
			if(comboDbLink.SelectedIndex==0){//None
				EFormFieldCur.DbLink="";
			}
			else{
				EFormFieldCur.DbLink=comboDbLink.GetSelected<string>();
			}
			EFormFieldCur.IsHorizStacking=checkIsHorizontal.Checked==true;
			EFormFieldCur.Width=textVIntWidth.Value;
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.IsTextWrap=checkIsTextWrap.Checked==true;
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.CondValueStrConverter(_listEFormFields,textCondParent.Text,textCondValue.Text);//This is used to convert the user readable checkbox values, "Checked" and "Unchecked", into "X" and "" which are what we store in the database. 
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}

		
	}
}