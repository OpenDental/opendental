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
		public EFormDef EFormDefCur;
		///<summary>All the siblings</summary>
		public List<EFormField> ListEFormFields;
		///<summary>Set this before opening this window. It's the current language being used in the parent form. Format is the text that's showing in the comboBox. Will be empty string if languages are not set up in pref LanguagesUsedByPatients or if the default language is being used in the parent FrmEFormDefs.</summary>
		public string LanguageShowing="";
		///<summary>We don't fire off a signal to update the language cache on other computers until we hit Save in the form window. So each edit window has this variable to keep track of whether there are any new translations. This bubbles up to the parent.</summary>
		public bool IsChangedLanCache;

		///<summary></summary>
		public FrmEFormTextBoxEdit() {
			InitializeComponent();
			Load+=FrmEFormsTextBoxEdit_Load;
			PreviewKeyDown+=FrmEFormTextBoxEdit_PreviewKeyDown;
			comboDbLink.SelectionTrulyChanged+=ComboDbLink_SelectionTrulyChanged;
			checkIsWidthPercentage.Click+=CheckIsWidthPercentage_Click;
			checkIsHorizStacking.Click+=CheckIsHorizStacking_Click;
		}

		private void FrmEFormsTextBoxEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(LanguageShowing==""){
				groupLanguage.Visible=false;
			}
			else{
				textLanguage.Text=LanguageShowing;
				textLabelTranslated.Text=LanguagePats.TranslateEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,EFormFieldCur.ValueLabel);
			}
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
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			checkBorder.Checked=EFormFieldCur.Border==EnumEFormBorder.ThreeD;
			textVIntWidth.Value=EFormFieldCur.Width;
			if(EFormFieldCur.IsWidthPercentage){
				labelWidth.Text="Width%";
				checkIsWidthPercentage.Checked=true;
				textVIntMinWidth.Value=EFormFieldCur.MinWidth;
			}
			else{
				labelMinWidth.Visible=false;
				textVIntMinWidth.Visible=false;
			}
			List<EFormField> listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,EFormFieldCur.IsHorizStacking);
			//this is just for loading. It will recalc each time CheckIsHorizStacking_Click is raised.
			//This is all siblings in a horizontal stack, not including the field passed in. If not in a h-stack, then this is an empty list.
			//Even if the current field is not stacking, it can be part of a stack group if the next field is set as stacking.
			//So this list gets recalculated each time the user checks or unchecks the stacking box.
			if(listEFormFieldsSiblings.Count==0){
				labelWidthIsPercentageNote.Visible=false;
			}
			checkIsHorizStacking.Checked=EFormFieldCur.IsHorizStacking;
			bool isPreviousStackable=EFormFields.IsPreviousStackable(EFormFieldCur,ListEFormFields);
			if(!isPreviousStackable){
				labelStackable.Text="previous field is not stackable";
				checkIsHorizStacking.IsEnabled=false;
			}
			bool isLastInHorizStack=EFormFields.IsLastInHorizStack(EFormFieldCur,ListEFormFields);
			if(isLastInHorizStack){
				int spaceBelowDefault=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
				if(EFormDefCur.SpaceBelowEachField!=-1){
					spaceBelowDefault=EFormDefCur.SpaceBelowEachField;//yes this or pref could be zero, which means zero
				}
				labelSpaceBelowDefault.Text=Lang.g(this,"leave blank to use the default value of ")+spaceBelowDefault.ToString();
				if(EFormFieldCur.SpaceBelow==-1){
					textSpaceBelow.Text="";
				}
				else{
					textSpaceBelow.Text=EFormFieldCur.SpaceBelow.ToString();
				}
			}
			else{
				labelSpaceBelowDefault.Text=Lang.g(this,"only the right-most field in this row may be set");
				textSpaceBelow.IsEnabled=false;
			}
			int spaceToRightDefault=PrefC.GetInt(PrefName.EformsSpaceToRightEachField);
			if(EFormDefCur.SpaceToRightEachField!=-1){
				spaceToRightDefault=EFormDefCur.SpaceToRightEachField;
			}
			labelSpaceToRightDefault.Text=Lang.g(this,"leave blank to use the default value of ")+spaceToRightDefault.ToString();
			if(EFormFieldCur.SpaceToRight==-1){
				textSpaceToRight.Text="";
			}
			else{
				textSpaceToRight.Text=EFormFieldCur.SpaceToRight.ToString();
			}
			checkIsTextWrap.Checked=EFormFieldCur.IsTextWrap;
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			textReportableName.Text=EFormFieldCur.ReportableName;
			checkIsLocked.Checked=EFormFieldCur.IsLocked;
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.ConvertCondDbToVis(ListEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
			textLabel.Focus();
		}

		private void ComboDbLink_SelectionTrulyChanged(object sender,EventArgs e) {
			if(textLabel.Text==""){
				textLabel.Text=(string)comboDbLink.SelectedItem;
			}
		}

		private void CheckIsHorizStacking_Click(object sender,EventArgs e) {
			List<EFormField> listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
			if(listEFormFieldsSiblings.Count>0){
				labelWidthIsPercentageNote.Visible=true;
			}
			else{
				labelWidthIsPercentageNote.Visible=false;
			}
		}

		private void CheckIsWidthPercentage_Click(object sender,EventArgs e) {
			if(checkIsWidthPercentage.Checked==true){
				labelWidth.Text="Width%";
				labelMinWidth.Visible=true;
				textVIntMinWidth.Visible=true;
			}
			else{
				labelWidth.Text="Width";
				labelMinWidth.Visible=false;
				textVIntMinWidth.Visible=false;
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
			//delete and cancel of a new field are equivalent.
			//We handle cancel outside this window, so we will also handle delete outside this window.
			//If not new, it won't get immediately deleted because they might not eventually click Save on the form itself.
			EFormFieldCur.IsDeleted=true;
			//if(EFormFieldCur.IsNew//not sure if this is needed.
			//if the field to the right is stacked and this one is not, then change the field to the right to not be stacked.
			int idx=ListEFormFields.IndexOf(EFormFieldCur);
			if(idx<ListEFormFields.Count-1 
				&& !ListEFormFields[idx].IsHorizStacking
				&& ListEFormFields[idx+1].IsHorizStacking)
			{
				ListEFormFields[idx+1].IsHorizStacking=false;
			}
			IsDialogOK=true;
		}

		private void FrmEFormTextBoxEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			if(!textVIntWidth.IsValid()
				|| !textVIntMinWidth.IsValid()
				|| !textVIntFontScale.IsValid())
			{
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			//If the parent is a radiobutton, they have to select a value.
			EFormField eFormField=ListEFormFields.Find(x=>x.ValueLabel==textCondParent.Text);
			if(!textCondParent.Text.IsNullOrEmpty() && eFormField is null){
				MsgBox.Show("That parent field does not exist.");
				return;
			}
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
			int spaceToRight=-1;
			if(textSpaceToRight.Text!=""){
				try{
					spaceToRight=Convert.ToInt32(textSpaceToRight.Text);
				}
				catch{
					MsgBox.Show(this,"Please fix error in Space to Right first.");
					return;
				}
				if(spaceToRight<0 || spaceToRight>200){
					MsgBox.Show(this,"Space to Right value is invalid.");
					return;
				}
			}
			//end of validation
			if(LanguageShowing!=""){
				IsChangedLanCache=LanguagePats.SaveTranslationEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,textLabelTranslated.Text);
				if(IsChangedLanCache){
					LanguagePats.RefreshCache();
				}
			}
			EFormFieldCur.ValueLabel=textLabel.Text;
			if(comboDbLink.SelectedIndex==0){//None
				EFormFieldCur.DbLink="";
			}
			else{
				EFormFieldCur.DbLink=comboDbLink.GetSelected<string>();
			}
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true;
			if(checkBorder.Checked==true){
				EFormFieldCur.Border=EnumEFormBorder.ThreeD;
			}
			else{
				EFormFieldCur.Border=EnumEFormBorder.None;
			}
			EFormFieldCur.Width=textVIntWidth.Value;
			EFormFieldCur.IsWidthPercentage=checkIsWidthPercentage.Checked==true;
			//change all siblings to match
			List<EFormField> listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
			for(int i=0;i<listEFormFieldsSiblings.Count;i++){
				listEFormFieldsSiblings[i].IsWidthPercentage=EFormFieldCur.IsWidthPercentage;
			}
			if(textVIntMinWidth.Visible){
				EFormFieldCur.MinWidth=textVIntMinWidth.Value;
			}
			else{
				EFormFieldCur.MinWidth=0;
			}
			EFormFieldCur.IsHorizStacking=checkIsHorizStacking.Checked==true;
			EFormFieldCur.SpaceBelow=spaceBelow;
			EFormFieldCur.SpaceToRight=spaceToRight;
			EFormFieldCur.IsTextWrap=checkIsTextWrap.Checked==true;
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.ReportableName=textReportableName.Text;
			EFormFieldCur.IsLocked=checkIsLocked.Checked==true;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.ConvertCondVisToDb(ListEFormFields,textCondParent.Text,textCondValue.Text);
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}

		
	}
}