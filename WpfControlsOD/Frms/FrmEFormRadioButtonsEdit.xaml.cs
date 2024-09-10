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
		///<summary>Siblings</summary>
		public List<EFormField> _listEFormFields;
		///<summary>This is the list showing in the grid. Gets pushed back into our object when user clicks save.</summary>
		private List<VisDb> _listVisDbs;
		private bool _alreadyAsked;

		///<summary></summary>
		public FrmEFormRadioButtonsEdit() {
			InitializeComponent();
			Load+=FrmEFormsRadioButtonsEdit_Load;
			PreviewKeyDown+=FrmEFormRadioButtonsEdit_PreviewKeyDown;
			comboDbLink.SelectionTrulyChanged+=ComboDbLink_SelectionTrulyChanged;
			checkLabelLeft.Click+=CheckLabelLeft_Click;
			checkLabelRight.Click+=CheckLabelRight_Click;
			gridMain.CellTextChanged+=GridMain_CellTextChanged;
			gridMain.CellSelectionCommitted+=GridMain_CellSelectionCommitted;
		}

		private void FrmEFormsRadioButtonsEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			textLabel.Text=EFormFieldCur.ValueLabel;
			checkLabelLeft.Checked=EFormFieldCur.LabelAlign==EnumEFormLabelAlign.LeftLeft;
			checkLabelRight.Checked=EFormFieldCur.LabelAlign==EnumEFormLabelAlign.Right;
			textVIntWidth.Value=EFormFieldCur.Width;
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
			SetVisibilities();
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			int spaceBelowDefault=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
			labelSpaceDefault.Text=Lang.g(this,"leave blank to use the default value of ")+spaceBelowDefault.ToString();
			if(EFormFieldCur.SpaceBelow==-1){
				textSpaceBelow.Text="";
			}
			else{
				textSpaceBelow.Text=EFormFieldCur.SpaceBelow.ToString();
			}
			textReportableName.Text=EFormFieldCur.ReportableName;
			//only set list from obj one time upon opening.
			_listVisDbs=new List<VisDb>();
			List<string> listVisOrig=new List<string>();
			List<string> listDbOrig=new List<string>();
			if(!string.IsNullOrEmpty(EFormFieldCur.PickListVis)){
				listVisOrig=EFormFieldCur.PickListVis.Split(',').ToList();
			}
			if(!string.IsNullOrEmpty(EFormFieldCur.PickListDb)){
				listDbOrig=EFormFieldCur.PickListDb.Split(',').ToList();
			}
			if(listVisOrig.Count==listDbOrig.Count){
				for(int i=0;i<listVisOrig.Count;i++){
					VisDb visDb=new VisDb();
					visDb.Vis=listVisOrig[i];
					visDb.Db=listDbOrig[i];
					_listVisDbs.Add(visDb);
				}
			}
			else{
				//Should never happen. We handle it by not filling the grid with anything at all.
			}
			FillGrid();
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.ConvertCondDbToVis(_listEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
			List<EFormField> listEFormFieldsChildren=_listEFormFields.FindAll(
				x=>x.ConditionalParent==EFormFieldCur.ValueLabel.Substring(0,Math.Min(EFormFieldCur.ValueLabel.Length,255))
				&& x.ConditionalParent!="" //for a new radiobutton, ValueLabel might be blank
			);
			textCountChildren.Text=listEFormFieldsChildren.Count.ToString();
		}

		private class VisDb{
			///<summary>This is what the patient will see.</summary>
			public string Vis;
			///<summary>This is the value that gets stored in db, frequently a string version of an enum.</summary>
			public string Db;
		}

		///<summary>This sets visibilities for various situations.</summary>
		private void SetVisibilities(){
			if(checkLabelLeft.Checked==true){
				labelWidth.Visible=true;
				textVIntWidth.Visible=true;
				labelWidthComment.Visible=true;
			}
			else{
				labelWidth.Visible=false;
				textVIntWidth.Visible=false;
				labelWidthComment.Visible=false;
				textVIntWidth.Value=0;;
			}
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
				gridColumn.IsWidthDynamic=true;
				gridMain.Columns.Add(gridColumn);
			}
			else{
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
				gridColumn.IsWidthDynamic=true;
				gridMain.Columns.Add(gridColumn);
			}
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listVisDbs.Count;i++){
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(_listVisDbs[i].Vis);
				if(comboDbLink.SelectedIndex>0){//db link
					GridCell gridCell=new GridCell(_listVisDbs[i].Db);
					gridRow.Cells.Add(gridCell);
				}
				//gridRow.Tag=;//not needed because grid always exactly matches list.
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void CheckLabelLeft_Click(object sender,EventArgs e) {
			if(checkLabelLeft.Checked==true){
				checkLabelRight.Checked=false;
			}
			SetVisibilities();
		}

		private void CheckLabelRight_Click(object sender,EventArgs e) {
			if(checkLabelRight.Checked==true){
				checkLabelLeft.Checked=false;
			}
			SetVisibilities();
		}

		private void comboDbLink_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_listVisDbs.Count>0){
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
			_listVisDbs=new List<VisDb>();
			string fieldName=comboDbLink.GetSelected<string>();
			List<string> listVisOrig=EFormFieldsAvailable.GetRadioVisDefault(fieldName);
			List<string> listDbOrig=EFormFieldsAvailable.GetRadioDbDefault(fieldName);
			for(int i=0;i<listVisOrig.Count;i++){
				VisDb visDb=new VisDb();
				visDb.Vis=listVisOrig[i];
				visDb.Db=listDbOrig[i];
				_listVisDbs.Add(visDb);
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
				_listVisDbs[gridMain.SelectedCell.Row].Db=gridMain.ListGridRows[gridMain.SelectedCell.Row].Cells[gridMain.SelectedCell.Col].Text;
			}
		}

		private void GridMain_CellTextChanged(object sender,EventArgs e) {
			//We need to do this with each text change because a button can grab focus and clear out the grid, and that will be too late.
			if(gridMain.SelectedCell.Col==0){
				//string txt=gridMain.ListGridRows[colRow.Row].Cells[colRow.Col].Text;
				_listVisDbs[gridMain.SelectedCell.Row].Vis=gridMain.ListGridRows[gridMain.SelectedCell.Row].Cells[gridMain.SelectedCell.Col].Text;
			}
		}

		private void butDeleteRow_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show("Please select a row first.");
				return;
			}
			_listVisDbs.RemoveAt(idx);
			FillGrid();
		}

		private void butAddRow_Click(object sender,EventArgs e) {
			VisDb visDb=new VisDb();
			visDb.Vis="Item"+(_listVisDbs.Count+1).ToString();
			visDb.Db="";
			_listVisDbs.Add(visDb);
			FillGrid();
			gridMain.SetSelected(new ColRow(0,_listVisDbs.Count-1));
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
			VisDb visDb=_listVisDbs[idx-1];
			_listVisDbs[idx-1]=_listVisDbs[idx];
			_listVisDbs[idx]=visDb;
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
			if(idx==_listVisDbs.Count-1){
				return;
			}
			VisDb visDb=_listVisDbs[idx+1];
			_listVisDbs[idx+1]=_listVisDbs[idx];
			_listVisDbs[idx]=visDb;
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
			textCondParent.Text=frmEFormFieldPicker.ParentSelected;
		}

		private void butPickValue_Click(object sender,EventArgs e) {
			textCondValue.Text=EFormL.PickCondValue(_listEFormFields,textCondParent.Text,textCondValue.Text);
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
			if(_listVisDbs.Count<2){
				MsgBox.Show("Must have at least two items in pick list.");
				return;
			}
			for(int i=0;i<_listVisDbs.Count;i++){
				if(_listVisDbs[i].Db.Contains(",")
					|| _listVisDbs[i].Vis.Contains(","))
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
			EFormField eFormField=_listEFormFields.Find(x=>x.ValueLabel==textCondParent.Text);
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
			if(checkLabelLeft.Checked==true){
				EFormFieldCur.LabelAlign=EnumEFormLabelAlign.LeftLeft;
				EFormFieldCur.Width=textVIntWidth.Value;
			}
			else if(checkLabelRight.Checked==true){
				EFormFieldCur.LabelAlign=EnumEFormLabelAlign.Right;
				EFormFieldCur.Width=0;
			}
			else{
				EFormFieldCur.LabelAlign=EnumEFormLabelAlign.TopLeft;
				EFormFieldCur.Width=0;
			}
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
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.SpaceBelow=spaceBelow;
			EFormFieldCur.ReportableName=textReportableName.Text;
			EFormFieldCur.PickListVis="";
			EFormFieldCur.PickListDb="";
			for(int i=0;i<_listVisDbs.Count;i++){
				if(i>0){
					EFormFieldCur.PickListVis+=",";
					EFormFieldCur.PickListDb+=",";
				}
				EFormFieldCur.PickListVis+=_listVisDbs[i].Vis;
				EFormFieldCur.PickListDb+=_listVisDbs[i].Db;
			}
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.ConvertCondVisToDb(_listEFormFields,textCondParent.Text,textCondValue.Text);
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
	}
}