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
		///<summary></summary>
		public bool IsPreviousStackable;
		///<summary>If set to true, then this field can have "space below" set.</summary>
		public bool IsLastInHorizStack;
		///<summary>All the siblings</summary>
		public List<EFormField> ListEFormFields;

		///<summary></summary>
		public FrmEFormTextBoxEdit() {
			InitializeComponent();
			Load+=FrmEFormsTextBoxEdit_Load;
			PreviewKeyDown+=FrmEFormTextBoxEdit_PreviewKeyDown;
			comboDbLink.SelectionTrulyChanged+=ComboDbLink_SelectionTrulyChanged;
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
			checkIsHorizStacking.Checked=EFormFieldCur.IsHorizStacking;
			if(!IsPreviousStackable){
				labelStackable.Text="previous field is not stackable";
				checkIsHorizStacking.IsEnabled=false;
			}
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			textVIntWidth.Value=EFormFieldCur.Width;
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			checkIsTextWrap.Checked=EFormFieldCur.IsTextWrap;
			if(IsLastInHorizStack){
				int spaceBelowDefault=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
				labelSpaceDefault.Text=Lang.g(this,"leave blank to use the default value of ")+spaceBelowDefault.ToString();
				if(EFormFieldCur.SpaceBelow==-1){
					textSpaceBelow.Text="";
				}
				else{
					textSpaceBelow.Text=EFormFieldCur.SpaceBelow.ToString();
				}
			}
			else{
				labelSpaceDefault.Text=Lang.g(this,"only the right-most field in this row may be set");
				textSpaceBelow.IsEnabled=false;
			}
			//checkIsLocked.Checked=EFormFieldCur.IsLocked;
			textReportableName.Text=EFormFieldCur.ReportableName;
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.ConvertCondDbToVis(ListEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
			textLabel.Focus();
		}

		private void ComboDbLink_SelectionTrulyChanged(object sender,EventArgs e) {
			if(textLabel.Text==""){
				textLabel.Text=(string)comboDbLink.SelectedItem;
			}
		}

		private void butPickParent_Click(object sender,EventArgs e) {
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=ListEFormFields;
			int idx=ListEFormFields.IndexOf(EFormFieldCur);
			frmEFormFieldPicker.ListSelectedIndices.Add(idx);//Prevents self selection as parent
			frmEFormFieldPicker.ShowDialog();
			if(frmEFormFieldPicker.IsDialogCancel){
				return;
			}
			textCondParent.Text=frmEFormFieldPicker.ParentSelected;
		}

		private void butPickValue_Click(object sender,EventArgs e) {
			textCondValue.Text=EFormL.PickCondValue(ListEFormFields,textCondParent.Text,textCondValue.Text);
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
			EFormField eFormField=ListEFormFields.Find(x=>x.ValueLabel==textCondParent.Text);
			if(eFormField!=null && eFormField.FieldType==EnumEFormFieldType.RadioButtons) {
				if(textCondValue.Text.IsNullOrEmpty()) {
					MsgBox.Show("Please select a value for your parent field.");
					return;
				}
			}
			int spaceBelow=-1;
			if(textSpaceBelow.Text!=""){
				try{
					spaceBelow=Convert.ToInt32(textSpaceBelow.Text);
				}
				catch{
					MsgBox.Show(this,"Please fix error in Space Below first.");
					return;
				}
				if(spaceBelow<0 || spaceBelow>200){
					MsgBox.Show(this,"Space Below value is invalid.");
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
			EFormFieldCur.IsHorizStacking=checkIsHorizStacking.Checked==true;
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true;
			EFormFieldCur.Width=textVIntWidth.Value;
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.IsTextWrap=checkIsTextWrap.Checked==true;
			//EFormFieldCur.IsLocked=checkIsLocked.Checked==true;
			EFormFieldCur.SpaceBelow=spaceBelow;
			EFormFieldCur.ReportableName=textReportableName.Text;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.ConvertCondVisToDb(ListEFormFields,textCondParent.Text,textCondValue.Text);
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}

		
	}
}