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
using WpfControls;
using WpfControls.UI;

namespace OpenDental {
	/// <summary>The editor is for the EFormField even though we're really editing the EFormFieldDef. This editor is not patient facing.</summary>
	public partial class FrmEFormCheckBoxEdit : FrmODBase {
		///<summary>This is the object being edited.</summary>
		public EFormField EFormFieldCur;
		///<summary>We need access to a few other fields of the EFormDef.</summary>
		public EFormDef EFormDefCur;
		///<summary>Siblings</summary>
		public List<EFormField> _listEFormFields;
		///<summary></summary>
		public bool IsPreviousStackable;
		private bool _alreadyAsked;

		///<summary></summary>
		public FrmEFormCheckBoxEdit() {
			InitializeComponent();
			Load+=FrmEFormsRadioButtonsEdit_Load;
			PreviewKeyDown+=FrmEFormRadioButtonsEdit_PreviewKeyDown;
			comboDbLink.SelectionTrulyChanged+=ComboDbLink_SelectionTrulyChanged;
			textMedAllerProb.LostKeyboardFocus+=TextMedAllerProb_LostKeyboardFocus;
		}

		private void FrmEFormsRadioButtonsEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			textLabel.Text=EFormFieldCur.ValueLabel;
			List<string> listAvailCheckBox=EFormFieldsAvailable.GetList_CheckBox();
			comboDbLink.Items.AddList(listAvailCheckBox);
			if(EFormFieldCur.DbLink==""){//None
				comboDbLink.SelectedIndex=0;
			}
			else if(EFormFieldCur.DbLink.StartsWith("allergy:")){
				comboDbLink.SelectedItem="allergy:";
				textMedAllerProb.Text=EFormFieldCur.DbLink.Substring(8);
			}
			else if(EFormFieldCur.DbLink.StartsWith("problem:")){
				comboDbLink.SelectedItem="problem:";
				textMedAllerProb.Text=EFormFieldCur.DbLink.Substring(8);
			}
			else{
				comboDbLink.SelectedItem=EFormFieldCur.DbLink;
			}
			SetVisAllerProb();
			checkIsHorizStacking.Checked=EFormFieldCur.IsHorizStacking;
			if(!IsPreviousStackable){
				labelStackable.Text="previous field is not stackable";
				checkIsHorizStacking.IsEnabled=false;
			}
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.CondValueStrConverter(_listEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
			List<EFormField> listEFormFieldsChildren=_listEFormFields.FindAll(x=>x.ConditionalParent==EFormFieldCur.ValueLabel.Substring(0,Math.Min(EFormFieldCur.ValueLabel.Length,255)));
			textCountChildren.Text=listEFormFieldsChildren.Count.ToString();
		}

		private void ComboDbLink_SelectionTrulyChanged(object sender,EventArgs e) {
			if((string)comboDbLink.SelectedItem=="allergy:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormAllergySetup);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					textMedAllerProb.Text="";
					SetVisAllerProb();
					textMedAllerProb.Focus();
					return;
				}
				long allergyDefNumSelected=formLauncher.GetField<long>("AllergyDefNumSelected");
				AllergyDef allergyDef=AllergyDefs.GetOne(allergyDefNumSelected);//from db
				textMedAllerProb.Text=allergyDef.Description;
				textLabel.Text=allergyDef.Description;
				SetVisAllerProb();
				textMedAllerProb.Focus();
				return;
			}
			if((string)comboDbLink.SelectedItem=="problem:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormDiseaseDefs);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					textMedAllerProb.Text="";
					SetVisAllerProb();
					textMedAllerProb.Focus();
					return;
				}
				DiseaseDef diseaseDef=formLauncher.GetField<List<DiseaseDef>>("ListDiseaseDefsSelected")[0];
				textMedAllerProb.Text=diseaseDef.DiseaseName;
				textLabel.Text=diseaseDef.DiseaseName;
				SetVisAllerProb();
				textMedAllerProb.Focus();
				return;
			}
			//all others
			if(textLabel.Text==""){
				textLabel.Text=(string)comboDbLink.SelectedItem;
			}
			SetVisAllerProb();
		}

		private void butChange_Click(object sender,EventArgs e) {
			if((string)comboDbLink.SelectedItem=="allergy:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormAllergySetup);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					textMedAllerProb.Text="";
					SetVisAllerProb();
					textMedAllerProb.Focus();
					return;
				}
				long allergyDefNumSelected=formLauncher.GetField<long>("AllergyDefNumSelected");
				AllergyDef allergyDef=AllergyDefs.GetOne(allergyDefNumSelected);
				textMedAllerProb.Text=allergyDef.Description;
				textLabel.Text=allergyDef.Description;
				SetVisAllerProb();
				textMedAllerProb.Focus();
				return;
			}
			if((string)comboDbLink.SelectedItem=="problem:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormDiseaseDefs);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					textMedAllerProb.Text="";
					SetVisAllerProb();
					textMedAllerProb.Focus();
					return;
				}
				DiseaseDef diseaseDef=formLauncher.GetField<List<DiseaseDef>>("ListDiseaseDefsSelected")[0];
				textMedAllerProb.Text=diseaseDef.DiseaseName;
				textLabel.Text=diseaseDef.DiseaseName;
				SetVisAllerProb();
				textMedAllerProb.Focus();
				return;
			}
			//all others:
			SetVisAllerProb();
		}

		private void TextMedAllerProb_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//AskChangeLabelToMatch();
			//This was too annoying.
			//Lots of edge cases where I couldn't suppress it properly
		}

		private void AskChangeLabelToMatch(){
			if(textMedAllerProb.Text==textLabel.Text){
				return;
			}
			if(textMedAllerProb.Text==""){
				return;
			}
			if(_alreadyAsked){
				return;
			}
			string str= "";
			if((string)comboDbLink.SelectedItem=="allergy:") {
				str="allergy";
			}
			else if((string)comboDbLink.SelectedItem=="problem:"){
				str="problem";
			}
			else{
				return;
			}
			_alreadyAsked=true;
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"Change label to match "+str+"?")){
				return;
			}
			textLabel.Text=textMedAllerProb.Text;
		}

		///<summary>This sets visibility based on selected index of comboDbLink.</summary>
		private void SetVisAllerProb(){
			if((string)comboDbLink.SelectedItem=="allergy:"){
				labelAllergProb.Visible=true;
				textMedAllerProb.Visible=true;
				butChange.Visible=true;
				labelAllergProb.Text="Allergy";
				checkIsRequired.Visible=false;
				return;
			}
			if((string)comboDbLink.SelectedItem=="problem:"){
				labelAllergProb.Visible=true;
				textMedAllerProb.Visible=true;
				butChange.Visible=true;
				labelAllergProb.Text="Problem";
				checkIsRequired.Visible=false;
				return;
			}
			//all others
			labelAllergProb.Visible=false;
			textMedAllerProb.Visible=false;
			butChange.Visible=false;
			textMedAllerProb.Text="";
			checkIsRequired.Visible=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//no need to verify with user because they have another chance to cancel in the parent window.
			EFormFieldCur=null;
			IsDialogOK=true;
		}

		private void butPickParent_Click(object sender,EventArgs e) {
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=_listEFormFields;
			int idx=_listEFormFields.IndexOf(EFormFieldCur);
			frmEFormFieldPicker.ListSelectedIndices.Add(idx);//Prevents self selection as parent
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

		private void FrmEFormRadioButtonsEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			AskChangeLabelToMatch();
			if(!textVIntFontScale.IsValid()) {
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			if(textMedAllerProb.Text==""){
				if((string)comboDbLink.SelectedItem=="allergy:"){
					MsgBox.Show("Please enter an allergy name first.");
					return;
				}
				if((string)comboDbLink.SelectedItem=="problem:"){
					MsgBox.Show("Please enter a problem name first.");
					return;
				}
			}
			//end of validation
			EFormFieldCur.ValueLabel=textLabel.Text;
			if(comboDbLink.SelectedIndex==0){//None
				EFormFieldCur.DbLink="";
			}
			else if((string)comboDbLink.SelectedItem=="allergy:"){
				EFormFieldCur.DbLink="allergy:"+textMedAllerProb.Text;
			}
			else if((string)comboDbLink.SelectedItem=="problem:"){
				EFormFieldCur.DbLink="problem:"+textMedAllerProb.Text;
			}
			else{
				EFormFieldCur.DbLink=(string)comboDbLink.SelectedItem;
			}
			EFormFieldCur.IsHorizStacking=checkIsHorizStacking.Checked==true;
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true && checkIsRequired.Visible;
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.CondValueStrConverter(_listEFormFields,textCondParent.Text,textCondValue.Text);
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}

		
	}
}