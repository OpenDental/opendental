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
	/// <summary>The editor is for the EFormField even though we're really editing the EFormFieldDef. This editor is not patient facing.</summary>
	public partial class FrmEFormCheckBoxEdit : FrmODBase {
		///<summary>This is the object being edited.</summary>
		public EFormField EFormFieldCur;
		///<summary>We need access to a few other fields of the EFormDef.</summary>
		public EFormDef EFormDefCur;
		///<summary></summary>
		public bool IsPreviousStackable;
		///<summary>Looks just like what would go in the db. If it starts with "allergy:", "problem:", or "med:", then this string also includes the selected allergy, etc. So it will look like "allergy:...", etc. If "None", then this will be empty string. This gets updated as the user types.</summary>
		private string _dbLink;

		///<summary></summary>
		public FrmEFormCheckBoxEdit() {
			InitializeComponent();
			Load+=FrmEFormsRadioButtonsEdit_Load;
			PreviewKeyDown+=FrmEFormRadioButtonsEdit_PreviewKeyDown;
			comboDbLink.SelectionChangeCommitted+=ComboDbLink_SelectionChangeCommitted;
			textMedAllerProb.TextChanged+=TextMedAllerProb_TextChanged;
		}

		private void FrmEFormsRadioButtonsEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			textLabel.Text=EFormFieldCur.ValueLabel;
			List<string> listAvailCheckBox=EFormFieldsAvailable.GetList_CheckBox();
			comboDbLink.Items.AddList(listAvailCheckBox);
			_dbLink=EFormFieldCur.DbLink;
			SetComboFromDbLink();
			SetMedAllerProbBox();
			checkIsHorizStacking.Checked=EFormFieldCur.IsHorizStacking;
			if(!IsPreviousStackable){
				labelStackable.Text="previous field is not stackable";
				checkIsHorizStacking.IsEnabled=false;
			}
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			if(_dbLink!="allergiesNone" && _dbLink!="problemsNone") {
				checkIsRequired.Visible=false;
			}
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
		}

		private void SetComboFromDbLink(){
			if(string.IsNullOrEmpty(_dbLink)){
				comboDbLink.SelectedIndex=0;//None
			}
			else if(_dbLink.StartsWith("allergy:")){
				comboDbLink.SelectedItem="allergy:";
			}
			else if(_dbLink.StartsWith("med:")){
				comboDbLink.SelectedItem="med:";
			}
			else if(_dbLink.StartsWith("problem:")){
				comboDbLink.SelectedItem="problem:";
			}
			else{
				comboDbLink.SelectedItem=_dbLink;
			}
		}

		private void ComboDbLink_SelectionChangeCommitted(object sender,EventArgs e) {
			//first, we must test for change because if it changed, we will be clearing the textbox.
			string strSelected=comboDbLink.GetSelected<string>();
			if(strSelected.StartsWith("allergy:") && _dbLink.StartsWith("allergy:")){
				return;
			}
			if(strSelected.StartsWith("med:") && _dbLink.StartsWith("med:")){
				return;
			}
			if(strSelected.StartsWith("problem:") && _dbLink.StartsWith("problem:")){
				return;
			}
			//now we know that they actually changed
			if(strSelected=="allergy:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormAllergySetup);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					//we have to revert their selection, or it would leave allergy with an inappropriate suffix
					SetComboFromDbLink();
					SetMedAllerProbBox();
					return;
				}
				long allergyDefNumSelected=formLauncher.GetField<long>("AllergyDefNumSelected");
				AllergyDef allergyDef=AllergyDefs.GetOne(allergyDefNumSelected);//from db
				_dbLink="allergy:"+allergyDef.Description;
			}
			else if(strSelected=="med:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormMedications);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					//we have to revert their selection, or it would leave allergy with an inappropriate suffix
					SetComboFromDbLink();
					SetMedAllerProbBox();
					return;
				}
				long medicationNum=formLauncher.GetField<long>("SelectedMedicationNum");
				Medication medication=Medications.GetOne(medicationNum);//from cache
				_dbLink="med:"+medication.MedName;
			}
			else if(strSelected=="problem:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormDiseaseDefs);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					//we have to revert their selection, or it would leave allergy with an inappropriate suffix
					SetComboFromDbLink();
					SetMedAllerProbBox();
					return;
				}
				DiseaseDef diseaseDef=formLauncher.GetField<List<DiseaseDef>>("ListDiseaseDefsSelected")[0];
				_dbLink="problem:"+diseaseDef.DiseaseName;
			}
			else if(strSelected=="None"){
				_dbLink="";
			}
			else{
				_dbLink=strSelected;
			}
			SetMedAllerProbBox();
		}

		private void TextMedAllerProb_TextChanged(object sender,EventArgs e) {
			if(comboDbLink.SelectedIndex==0){//None
				return;
			}
			else if(comboDbLink.SelectedItem.ToString()=="allergy:"){
				_dbLink="allergy:"+textMedAllerProb.Text;
			}
			else if(comboDbLink.SelectedItem.ToString()=="med:"){
				_dbLink="med:"+textMedAllerProb.Text;
			}
			else if(comboDbLink.SelectedItem.ToString()=="problem:"){
				_dbLink="problem:"+textMedAllerProb.Text;
			}
			else{
				comboDbLink.SelectedItem=_dbLink;
			}
		}

		private void butChange_Click(object sender,EventArgs e) {
			if(_dbLink.StartsWith("allergy:")){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormAllergySetup);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					return;
				}
				long allergyDefNumSelected=formLauncher.GetField<long>("AllergyDefNumSelected");
				AllergyDef allergyDef=AllergyDefs.GetOne(allergyDefNumSelected);
				_dbLink="allergy:"+allergyDef.Description;
			}
			else if(_dbLink.StartsWith("med:")){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormMedications);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					return;
				}
				long medicationNum=formLauncher.GetField<long>("SelectedMedicationNum");
				Medication medication=Medications.GetOne(medicationNum);//from cache
				_dbLink="med:"+medication.MedName;
			}
			else if(_dbLink.StartsWith("problem:")){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormDiseaseDefs);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					return;
				}
				DiseaseDef diseaseDef=formLauncher.GetField<List<DiseaseDef>>("ListDiseaseDefsSelected")[0];
				_dbLink="problem:"+diseaseDef.DiseaseName;
			}
			SetMedAllerProbBox();
		}

		///<summary>This sets visibility and fills the box based on _dbLink</summary>
		private void SetMedAllerProbBox(){
			if(string.IsNullOrEmpty(_dbLink)){
				labelMedAllerProb.Visible=false;
				textMedAllerProb.Visible=false;
				butChange.Visible=false;
				textMedAllerProb.Text="";
				checkIsRequired.Visible=true;
			}
			else if(_dbLink.StartsWith("allergy:")){
				labelMedAllerProb.Visible=true;
				textMedAllerProb.Visible=true;
				butChange.Visible=true;
				labelMedAllerProb.Text="Allergy";
				textMedAllerProb.Text=_dbLink.Substring(8);//this does trigger TextChanged, which is harmless
				checkIsRequired.Visible=false;
			}
			else if(_dbLink.StartsWith("med:")){
				labelMedAllerProb.Visible=true;
				textMedAllerProb.Visible=true;
				butChange.Visible=true;
				labelMedAllerProb.Text="Medication";
				textMedAllerProb.Text=_dbLink.Substring(4);
				checkIsRequired.Visible=false;
			}
			else if(_dbLink.StartsWith("problem:")){
				labelMedAllerProb.Visible=true;
				textMedAllerProb.Visible=true;
				butChange.Visible=true;
				labelMedAllerProb.Text="Problem";
				textMedAllerProb.Text=_dbLink.Substring(8);
				checkIsRequired.Visible=false;
			}
			else{//allergiesNone or problemsNone
				labelMedAllerProb.Visible=false;
				textMedAllerProb.Visible=false;
				butChange.Visible=false;
				textMedAllerProb.Text="";
				checkIsRequired.Visible=true;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//no need to verify with user because they have another chance to cancel in the parent window.
			EFormFieldCur=null;
			IsDialogOK=true;
		}

		private void FrmEFormRadioButtonsEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!textVIntFontScale.IsValid()) {
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			//end of validation
			EFormFieldCur.ValueLabel=textLabel.Text;
			EFormFieldCur.DbLink=_dbLink;
			EFormFieldCur.IsHorizStacking=checkIsHorizStacking.Checked==true;
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true && checkIsRequired.Visible;
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}

		
	}
}