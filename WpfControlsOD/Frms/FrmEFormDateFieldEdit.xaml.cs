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
	public partial class FrmEFormDateFieldEdit : FrmODBase {
		///<summary>This is the object being edited.</summary>
		public EFormField EFormFieldCur;
		///<summary>We need access to a few other fields of the EFormDef.</summary>
		public EFormDef EFormDefCur;
		///<summary>All the siblings</summary>
		public List<EFormField> ListEFormFields;
		///<summary>Set this before opening this window. It's the current language being used in the parent form. Format is the text that's showing in the comboBox. Will be empty string if languages are not set up in pref LanguagesUsedByPatients or if the default language is being used in the parent FrmEFormDefs.</summary>
		public string LanguageShowing="";
		///<summary>This is all sibings in a horizontal stack, not including the field passed in. If not in a h-stack, then this is an empty list. Even if the current field is not stacking, it can be part of a stack group if the next field is set as stacking. So this list gets recalculated each time the user checks or unchecks the stacking box. If this is a new field, then it is not yet in the list, but we do know where it will potientially go, based on IdxNew, and that's what we use to create this list.</summary>
		private List<EFormField> _listEFormFieldsSiblings;
		///<summary>We don't fire off a signal to update the language cache on other computers until we hit Save in the form window. So each edit window has this variable to keep track of whether there are any new translations. This bubbles up to the parent.</summary>
		public bool IsChangedLanCache;

		///<summary></summary>
		public FrmEFormDateFieldEdit() {
			InitializeComponent();
			Load+=FrmEFormsDateFieldEdit_Load;
			PreviewKeyDown+=FrmEFormDateFieldEdit_PreviewKeyDown;
			checkIsWidthPercentage.Click+=CheckIsWidthPercentage_Click;
			checkIsHorizStacking.Click+=CheckIsHorizStacking_Click;
		}

		private void FrmEFormsDateFieldEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(LanguageShowing==""){
				groupLanguage.Visible=false;
			}
			else{
				textLanguage.Text=LanguageShowing;
				textLabelTranslated.Text=LanguagePats.TranslateEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,EFormFieldCur.ValueLabel);
			}
			textLabel.Text=EFormFieldCur.ValueLabel;
			List<string> listAvailTextBox=EFormFieldsAvailable.GetList_DateField();
			comboDbLink.Items.AddList(listAvailTextBox);
			int idxSelect=listAvailTextBox.IndexOf(EFormFieldCur.DbLink);
			if(idxSelect==-1) {//this handles "" showing as "None"
				comboDbLink.SelectedIndex=0;//None
			}
			else {
				comboDbLink.SelectedIndex=idxSelect;
			}
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			//groupLayout start
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
			_listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
			//this is just for loading. It will recalc each time CheckIsHorizStacking_Click is raised.
			if(_listEFormFieldsSiblings.Count==0){
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
			//groupLayout end
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			textReportableName.Text=EFormFieldCur.ReportableName;
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.ConvertCondDbToVis(ListEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
			List<EFormField> listEFormFieldsChildren=ListEFormFields.FindAll(
				x=>x.ConditionalParent==EFormFieldCur.ValueLabel.Substring(0,Math.Min(EFormFieldCur.ValueLabel.Length,255))
				&& x.ConditionalParent!="" //for a new date, ValueLabel might be blank
			);
			textCountChildren.Text=listEFormFieldsChildren.Count.ToString();
			textLabel.Focus();
		}

		private void CheckIsHorizStacking_Click(object sender,EventArgs e) {
			_listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
			if(_listEFormFieldsSiblings.Count>0){
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
			textCondValue.Text=EFormL.PickCondValues(ListEFormFields,textCondParent.Text,textCondValue.Text);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//see comments in FrmEFormTextBoxEdit.butDelete_Click
			EFormFieldCur.IsDeleted=true;
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

		private void FrmEFormDateFieldEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
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
			//groupLayout start
			if(checkBorder.Checked==true){
				EFormFieldCur.Border=EnumEFormBorder.ThreeD;
			}
			else{
				EFormFieldCur.Border=EnumEFormBorder.None;
			}
			EFormFieldCur.Width=textVIntWidth.Value;
			EFormFieldCur.IsWidthPercentage=checkIsWidthPercentage.Checked==true;
			//change all siblings to match
			_listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
			for(int i=0;i<_listEFormFieldsSiblings.Count;i++){
				_listEFormFieldsSiblings[i].IsWidthPercentage=EFormFieldCur.IsWidthPercentage;
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
			//groupLayout end
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.ReportableName=textReportableName.Text;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.ConvertCondVisToDb(ListEFormFields,textCondParent.Text,textCondValue.Text);
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
	}
}