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
	/// <summary>This editor is for the EFormFieldDef, not the EFormField attached to a patient. There's no editor for that because it's only used as part of a patient facing layout.</summary>
	public partial class FrmEFormRadioButtonsEdit : FrmODBase {
		///<summary>This is the object being edited.</summary>
		public EFormField EFormFieldCur;
		///<summary></summary>
		public EFormDef EFormDefCur;
		///<summary>Siblings</summary>
		public List<EFormField> ListEFormFields;
		///<summary>Set this before opening this window. It's the current language being used in the parent form. Format is the text that's showing in the comboBox. Will be empty string if languages are not set up in pref LanguagesUsedByPatients or if the default language is being used in the parent FrmEFormDefs.</summary>
		public string LanguageShowing="";
		///<summary>This is the list showing in the grid. Gets pushed back into our object when user clicks save.</summary>
		private List<VisDbLang> _listVisDbLangs;
		private bool _alreadyAsked;
		///<summary>We don't fire off a signal to update the language cache on other computers until we hit Save in the form window. So each edit window has this variable to keep track of whether there are any new translations. This bubbles up to the parent.</summary>
		public bool IsChangedLanCache;

		///<summary></summary>
		public FrmEFormRadioButtonsEdit() {
			InitializeComponent();
			Load+=FrmEFormsRadioButtonsEdit_Load;
			PreviewKeyDown+=FrmEFormRadioButtonsEdit_PreviewKeyDown;
			comboDbLink.SelectionTrulyChanged+=ComboDbLink_SelectionTrulyChanged;
			radioLabelTop.Click+=(sender,e)=>SetVisibilities();
			gridMain.CellTextChanged+=GridMain_CellTextChanged;
			gridMain.CellSelectionCommitted+=GridMain_CellSelectionCommitted;
			checkIsWidthPercentage.Click+=CheckIsWidthPercentage_Click;
			checkIsHorizStacking.Click+=CheckIsHorizStacking_Click;
		}

		private void FrmEFormsRadioButtonsEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			List<string> listLangOrig=new List<string>();//stores the language translations for the labels in PickListVis.
			if(LanguageShowing==""){
				groupLanguage.Visible=false;
			}
			else{
				textLanguage.Text=LanguageShowing;
				string strLabels=EFormFieldCur.ValueLabel+","+EFormFieldCur.PickListVis;
				string strTranslations=LanguagePats.TranslateEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,strLabels);
				List<string> listTranslations=strTranslations.Split(',').ToList();//Ex: [label,button1,button2]
				textLabelTranslated.Text=listTranslations[0];
				for(int i=1;i<listTranslations.Count; i++){//add the translated button labels to listPickLang to use when populating _listVisDbLangs.
					listLangOrig.Add(listTranslations[i]);
				}
			}
			textLabel.Text=EFormFieldCur.ValueLabel;
			List<string> listAvailRadio=EFormFieldsAvailable.GetList_RadioButtons();
			comboDbLink.Items.AddList(listAvailRadio);
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
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			if(EFormFieldCur.LabelAlign==EnumEFormLabelAlign.TopLeft){
				radioLabelTop.Checked=true;
			}
			if(EFormFieldCur.LabelAlign==EnumEFormLabelAlign.LeftLeft){
				radioLabelLeft.Checked=true;
			}
			if(EFormFieldCur.LabelAlign==EnumEFormLabelAlign.Right){
				radioLabelRight.Checked=true;
			}
			SetVisibilities();
			textVIntWidthLabel.Value=EFormFieldCur.WidthLabel;
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
			List<EFormField> listEFormFieldsSiblings=EFormFields.GetSiblingsInStack(EFormFieldCur,ListEFormFields,checkIsHorizStacking.Checked==true);
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
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			textReportableName.Text=EFormFieldCur.ReportableName;
			//only set list from obj one time upon opening.
			_listVisDbLangs=new List<VisDbLang>();
			List<string> listVisOrig=new List<string>();
			List<string> listDbOrig=new List<string>();
			if(!string.IsNullOrEmpty(EFormFieldCur.PickListVis)){
				listVisOrig=EFormFieldCur.PickListVis.Split(',').ToList();
			}
			if(!string.IsNullOrEmpty(EFormFieldCur.PickListDb)){
				listDbOrig=EFormFieldCur.PickListDb.Split(',').ToList();
			}
			for(int i=0;i<listVisOrig.Count;i++){
				VisDbLang visDbLang=new VisDbLang();
				visDbLang.Vis=listVisOrig[i];
				visDbLang.Db=listDbOrig[i];
				if(LanguageShowing!=""){//Translating a language.
					visDbLang.Lang=listLangOrig[i];
				}
				_listVisDbLangs.Add(visDbLang);
			}
			FillGrid();
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.ConvertCondDbToVis(ListEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
			List<EFormField> listEFormFieldsChildren=ListEFormFields.FindAll(
				x=>x.ConditionalParent==EFormFieldCur.ValueLabel.Substring(0,Math.Min(EFormFieldCur.ValueLabel.Length,255))
				&& x.ConditionalParent!="" //for a new radiobutton, ValueLabel might be blank
			);
			textCountChildren.Text=listEFormFieldsChildren.Count.ToString();
		}

		private class VisDbLang{
			///<summary>This is what the patient will see.</summary>
			public string Vis;
			///<summary>This is the value that gets stored in db, frequently a string version of an enum.</summary>
			public string Db;
			///<summary>This is what the patient will see, but translated from Vis into another language.</summary>
			public string Lang;
		}

		///<summary>This sets visibilities for label position and allergies/problems.</summary>
		private void SetVisibilities(){
			if(radioLabelLeft.Checked==true){
				labelWidthLabel.Visible=true;
				textVIntWidthLabel.Visible=true;
			}
			else{
				labelWidthLabel.Visible=false;
				textVIntWidthLabel.Visible=false;
				textVIntWidthLabel.Value=0;;
			}
			//Then allergies problems
			if((string)comboDbLink.SelectedItem=="allergy:"){
				labelAllergProb.Visible=true;
				textMedAllerProb.Visible=true;
				butChange.Visible=true;
				labelAllergProb.Text="Allergy";
				checkIsRequired.Visible=true;
				return;
			}
			if((string)comboDbLink.SelectedItem=="problem:"){
				labelAllergProb.Visible=true;
				textMedAllerProb.Visible=true;
				butChange.Visible=true;
				labelAllergProb.Text="Problem";
				checkIsRequired.Visible=true;
				return;
			}
			//all others:
			labelAllergProb.Visible=false;
			textMedAllerProb.Visible=false;
			butChange.Visible=false;
			textMedAllerProb.Text="";
			checkIsRequired.Visible=true;
		}

		private void FillGrid(){
			//This never changes the list. Just makes the grid match the list.
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn;
			if(comboDbLink.SelectedIndex==0){//no db link
				gridColumn=new GridColumn("",170);
				gridColumn.IsEditable=true;
				if(LanguageShowing!=""){//translating language
					gridMain.Columns.Add(gridColumn);
					gridColumn=new GridColumn(Lans.g("TableEFormRadioButton","Translation"),170);
					gridColumn.IsEditable=true;
				}
				gridColumn.IsWidthDynamic=true;
				gridMain.Columns.Add(gridColumn);
			}
			else{//db link
				gridColumn=new GridColumn(Lans.g("TableEFormRadioButton","Visible to Patient"),170);
				gridColumn.IsEditable=true;
				gridMain.Columns.Add(gridColumn);
				gridColumn=new GridColumn(Lans.g("TableEFormRadioButton","As Stored in Db"),170);
				gridColumn.ListDisplayStrings=new List<string>();
				string strSelected=comboDbLink.GetSelected<string>();
				List<string> listDbAll=EFormFieldsAvailable.GetRadioDbAll(strSelected);
				for(int i=0;i<listDbAll.Count;i++){
					gridColumn.ListDisplayStrings.Add(listDbAll[i]);
				}
				if(LanguageShowing!=""){//translating language
					gridMain.Columns.Add(gridColumn);
					gridColumn=new GridColumn(Lans.g("TableEFormRadioButton","Translation"),170);
					gridColumn.IsEditable=true;
				}
				gridColumn.IsWidthDynamic=true;
				gridMain.Columns.Add(gridColumn);
			}
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listVisDbLangs.Count;i++){
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(_listVisDbLangs[i].Vis);
				if(comboDbLink.SelectedIndex>0){//db link
					GridCell gridCell=new GridCell(_listVisDbLangs[i].Db);
					gridRow.Cells.Add(gridCell);
				}
				if(LanguageShowing!=""){
					GridCell gridCell=new GridCell(_listVisDbLangs[i].Lang);
					gridRow.Cells.Add(gridCell);
				}
				//gridRow.Tag=;//not needed because grid always exactly matches list.
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void comboDbLink_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_listVisDbLangs.Count>0){
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Pick list will be reset for this Db Link. Continue?")){
					return;
				}
			}
			ResetList();
		}

		private void ComboDbLink_SelectionTrulyChanged(object sender,EventArgs e) {
			if((string)comboDbLink.SelectedItem=="allergy:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormAllergySetup);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					textMedAllerProb.Text="";
					SetVisibilities();
					textMedAllerProb.Focus();
					return;
				}
				long allergyDefNumSelected=formLauncher.GetField<long>("AllergyDefNumSelected");
				AllergyDef allergyDef=AllergyDefs.GetOne(allergyDefNumSelected);//from db
				textMedAllerProb.Text=allergyDef.Description;
				textLabel.Text=allergyDef.Description;
				SetVisibilities();
				textMedAllerProb.Focus();
				return;
			}
			if((string)comboDbLink.SelectedItem=="problem:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormDiseaseDefs);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					textMedAllerProb.Text="";
					SetVisibilities();
					textMedAllerProb.Focus();
					return;
				}
				DiseaseDef diseaseDef=formLauncher.GetField<List<DiseaseDef>>("ListDiseaseDefsSelected")[0];
				textMedAllerProb.Text=diseaseDef.DiseaseName;
				textLabel.Text=diseaseDef.DiseaseName;
				SetVisibilities();
				textMedAllerProb.Focus();
				return;
			}
			//all others
			if(textLabel.Text==""){
				textLabel.Text=(string)comboDbLink.SelectedItem;
			}
			SetVisibilities();
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

		private void butChange_Click(object sender,EventArgs e) {
			if((string)comboDbLink.SelectedItem=="allergy:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormAllergySetup);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					textMedAllerProb.Text="";
					SetVisibilities();
					textMedAllerProb.Focus();
					return;
				}
				long allergyDefNumSelected=formLauncher.GetField<long>("AllergyDefNumSelected");
				AllergyDef allergyDef=AllergyDefs.GetOne(allergyDefNumSelected);
				textMedAllerProb.Text=allergyDef.Description;
				textLabel.Text=allergyDef.Description;
				SetVisibilities();
				textMedAllerProb.Focus();
				return;
			}
			if((string)comboDbLink.SelectedItem=="problem:"){
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormDiseaseDefs);
				formLauncher.SetField("IsSelectionMode",true);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					textMedAllerProb.Text="";
					SetVisibilities();
					textMedAllerProb.Focus();
					return;
				}
				DiseaseDef diseaseDef=formLauncher.GetField<List<DiseaseDef>>("ListDiseaseDefsSelected")[0];
				textMedAllerProb.Text=diseaseDef.DiseaseName;
				textLabel.Text=diseaseDef.DiseaseName;
				SetVisibilities();
				textMedAllerProb.Focus();
				return;
			}
			//all others
			SetVisibilities();
		}

		private void ResetList(){
			_listVisDbLangs=new List<VisDbLang>();
			string fieldName=comboDbLink.GetSelected<string>();
			List<string> listVisOrig=EFormFieldsAvailable.GetRadioVisDefault(fieldName);
			List<string> listDbOrig=EFormFieldsAvailable.GetRadioDbDefault(fieldName);
			for(int i=0;i<listVisOrig.Count;i++){
				VisDbLang visDb=new VisDbLang();
				visDb.Vis=listVisOrig[i];
				visDb.Db=listDbOrig[i];
				if(LanguageShowing!=""){
					visDb.Lang=listVisOrig[i];//DbLink changed, reset translations to match default.
				}
				_listVisDbLangs.Add(visDb);
			}
			FillGrid();
		}

		private void butHelp_Click(object sender,EventArgs e) {
			string txt=@"Advanced features that can be ignored by most users:

To exclude a db option from being visible to the patient, remove that row. Example: Ins Relationship has 9 options, but only 4 of them are really used in dentistry. Just leave the other 5 off and force patient to pick one of the 4.

But you also don't need to force them to pick one. Example: For Marital Status, you might only show Married and Child, excluding Divorced and Single from the pick list. The unselected state then represents no change, so an existing patient could leave both radio buttons unchecked and their status would remain unchanged.

You can have a row with no db value. For example, a visible value of Separated might have no corresponding db value entered. In that case, an import would not cause any change to the existing db value.

Two radio buttons can represent one db item. Example: Gender Other in db can be expanded to show the patient both Nonbinary and Other. When patient picks either of these, it goes into the db as Other.

Any or all items are allowed to have no label by leaving that value in the first column empty. Example: Y/N radiobuttons for a series of allergies. Y/N label at top, but none of the radiobuttons need labels.";
			MsgBox.Show(txt);
		}

		private void GridMain_CellSelectionCommitted(object sender,GridClickEventArgs e) {
			if(gridMain.SelectedCell.Col==1){//not really needed
				_listVisDbLangs[gridMain.SelectedCell.Row].Db=gridMain.ListGridRows[gridMain.SelectedCell.Row].Cells[gridMain.SelectedCell.Col].Text;
			}
		}

		private void GridMain_CellTextChanged(object sender,EventArgs e) {
			//We need to do this with each text change because a button can grab focus and clear out the grid, and that will be too late.
			if(gridMain.SelectedCell.Col==0){
				//string txt=gridMain.ListGridRows[colRow.Row].Cells[colRow.Col].Text;
				_listVisDbLangs[gridMain.SelectedCell.Row].Vis=gridMain.ListGridRows[gridMain.SelectedCell.Row].Cells[gridMain.SelectedCell.Col].Text;
			}
			if(LanguageShowing!=""){
				if(comboDbLink.SelectedIndex==0 && gridMain.SelectedCell.Col==1){//No dblink.
					_listVisDbLangs[gridMain.SelectedCell.Row].Lang=gridMain.ListGridRows[gridMain.SelectedCell.Row].Cells[gridMain.SelectedCell.Col].Text;
				}
				else if(gridMain.SelectedCell.Col==2){//dblink in use.
					_listVisDbLangs[gridMain.SelectedCell.Row].Lang=gridMain.ListGridRows[gridMain.SelectedCell.Row].Cells[gridMain.SelectedCell.Col].Text;
				}
			}
		}

		private void butDeleteRow_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show("Please select a row first.");
				return;
			}
			_listVisDbLangs.RemoveAt(idx);
			FillGrid();
		}

		private void butAddRow_Click(object sender,EventArgs e) {
			VisDbLang visDbLang=new VisDbLang();
			visDbLang.Vis="Item"+(_listVisDbLangs.Count+1).ToString();
			visDbLang.Db="";
			if(LanguageShowing!=""){//Doesn't sync for languages not currently being translated.
				visDbLang.Lang="Item"+(_listVisDbLangs.Count+1).ToString();
			}
			_listVisDbLangs.Add(visDbLang);
			FillGrid();
			gridMain.SetSelected(new ColRow(0,_listVisDbLangs.Count-1));
		}

		private void butUp_Click(object sender,EventArgs e) {
			ColRow colRowSelected=gridMain.SelectedCell;
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(idx==0){
				return;
			}
			VisDbLang visDbLang=_listVisDbLangs[idx-1];
			_listVisDbLangs[idx-1]=_listVisDbLangs[idx];
			_listVisDbLangs[idx]=visDbLang;
			FillGrid();
			ColRow colRow=new ColRow(colRowSelected.Col,idx-1);
			gridMain.SetSelected(colRow);
		}

		private void butDown_Click(object sender,EventArgs e) {
			ColRow colRowSelected=gridMain.SelectedCell;
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(idx==_listVisDbLangs.Count-1){
				return;
			}
			VisDbLang visDb=_listVisDbLangs[idx+1];
			_listVisDbLangs[idx+1]=_listVisDbLangs[idx];
			_listVisDbLangs[idx]=visDb;
			FillGrid();
			ColRow colRow=new ColRow(colRowSelected.Col,idx+1);
			gridMain.SetSelected(colRow);
		}

		//private void butPickChildren_Click(object sender,EventArgs e) {
		//	FrmEFormFieldChildren frmEFormFieldChildren=new FrmEFormFieldChildren();
		//	frmEFormFieldChildren.ListEFormFields=_listEFormFields;
		//	frmEFormFieldChildren.ShowDialog();
		//	if(frmEFormFieldChildren.IsDialogCancel){
		//		return;
		//	}
		//	textCondParent.Text=frmEFormFieldChildren.LabelSelected;
		//}

		private void butDelete_Click(object sender,EventArgs e) {
			//no need to verify with user because they have another chance to cancel in the parent window.
			EFormFieldCur.IsDeleted=true;
			//RadioButtons are not h-stackable, so no need to check stacking for next field.
			IsDialogOK=true;
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

		private void FrmEFormRadioButtonsEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
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

		private void butSave_Click(object sender, EventArgs e) {
			AskChangeLabelToMatch();
			if(_listVisDbLangs.Count<2){
				MsgBox.Show("Must have at least two items in pick list.");
				return;
			}
			if(textLabel.Text.Contains(",")
				|| textLabelTranslated.Text.Contains(","))
			{
				MsgBox.Show("Labels cannot contain commas.");
				return;
			}
			for(int i=0;i<_listVisDbLangs.Count;i++){
				if(_listVisDbLangs[i].Db.Contains(",")
					|| _listVisDbLangs[i].Vis.Contains(",")
					|| LanguageShowing!="" && _listVisDbLangs[i].Lang.Contains(","))//Or, if translating a language and a translation contains a comma.
				{
					MsgBox.Show("Pick list items cannot contain commas.");
					return;
				}
			}
			if(!textVIntFontScale.IsValid()) {
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			//If the parent is a radiobutton, they have to select a value.
			EFormField eFormField=null;
			if(textCondParent.Text!=""){
				eFormField=ListEFormFields.Find(x=>x.ValueLabel==textCondParent.Text);
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
			string strTranslations=textLabelTranslated.Text+",";
			for(int i=0;i<_listVisDbLangs.Count;i++){
				if(i>0){
					strTranslations+=",";
				}
				strTranslations+=_listVisDbLangs[i].Lang;//ex: "ValueLabel,Button1,Button2"
			}
			IsChangedLanCache=LanguagePats.SaveTranslationEFormField(EFormFieldCur.EFormFieldDefNum,LanguageShowing,strTranslations);
			if(IsChangedLanCache){
				LanguagePats.RefreshCache();
			}
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
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true;
			if(radioLabelTop.Checked==true){
				EFormFieldCur.LabelAlign=EnumEFormLabelAlign.TopLeft;
				EFormFieldCur.WidthLabel=0;
			}
			if(radioLabelLeft.Checked==true){
				EFormFieldCur.LabelAlign=EnumEFormLabelAlign.LeftLeft;
				EFormFieldCur.WidthLabel=textVIntWidthLabel.Value;
			}
			if(radioLabelRight.Checked==true){
				EFormFieldCur.LabelAlign=EnumEFormLabelAlign.Right;
				EFormFieldCur.WidthLabel=0;
			}
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
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.ReportableName=textReportableName.Text;
			EFormFieldCur.PickListVis="";
			EFormFieldCur.PickListDb="";
			for(int i=0;i<_listVisDbLangs.Count;i++){
				if(i>0){
					EFormFieldCur.PickListVis+=",";
					EFormFieldCur.PickListDb+=",";
				}
				EFormFieldCur.PickListVis+=_listVisDbLangs[i].Vis;
				EFormFieldCur.PickListDb+=_listVisDbLangs[i].Db;
			}
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.ConvertCondVisToDb(ListEFormFields,textCondParent.Text,textCondValue.Text);
			LanguagePats.SyncRadioButtonTranslations(EFormFieldCur);//Ensures translations are in sync with PickListVis.
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
	}
}