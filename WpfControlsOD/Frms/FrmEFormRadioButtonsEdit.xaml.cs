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

		///<summary></summary>
		public FrmEFormRadioButtonsEdit() {
			InitializeComponent();
			Load+=FrmEFormsRadioButtonsEdit_Load;
			PreviewKeyDown+=FrmEFormRadioButtonsEdit_PreviewKeyDown;
			gridMain.CellTextChanged+=GridMain_CellTextChanged;
			gridMain.CellSelectionCommitted+=GridMain_CellSelectionCommitted;
		}

		private void FrmEFormsRadioButtonsEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			textLabel.Text=EFormFieldCur.ValueLabel;
			textVIntFontScale.Value=EFormFieldCur.FontScale;
			checkIsRequired.Checked=EFormFieldCur.IsRequired;
			textCondParent.Text=EFormFieldCur.ConditionalParent;
			textCondValue.Text=EFormL.CondValueStrConverter(_listEFormFields,EFormFieldCur.ConditionalParent,EFormFieldCur.ConditionalValue);
			List<string> listAvailRadio=EFormFieldsAvailable.GetList_RadioButtons();
			comboDbLink.Items.AddList(listAvailRadio);
			int idxSelect=listAvailRadio.IndexOf(EFormFieldCur.DbLink);
			if(idxSelect==-1){//this handles "" showing as "None"
				comboDbLink.SelectedIndex=0;//None
			}
			else{
				comboDbLink.SelectedIndex=idxSelect;
			}
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
			List<EFormField> listEFormFieldsChildren=_listEFormFields.FindAll(x=>x.ConditionalParent==EFormFieldCur.ValueLabel.Substring(0,Math.Min(EFormFieldCur.ValueLabel.Length,255)));
			textCountChildren.Text=listEFormFieldsChildren.Count.ToString();
		}

		private class VisDb{
			///<summary>This is what the patient will see.</summary>
			public string Vis;
			///<summary>This is the value that gets stored in db, frequently a string version of an enum.</summary>
			public string Db;
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
				List<string> listDbAll=EFormFieldsAvailable.GetRadioDbAll(comboDbLink.GetSelected<string>());
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

		private void comboDbLink_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_listVisDbs.Count>0){
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Pick list will be reset for this Db Link. Continue?")){
					return;
				}
			}
			ResetList();
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
			//todo: more validation

			//end of validation
			EFormFieldCur.ValueLabel=textLabel.Text;
			EFormFieldCur.FontScale=textVIntFontScale.Value;
			EFormFieldCur.IsRequired=checkIsRequired.Checked==true;
			EFormFieldCur.ConditionalParent=textCondParent.Text;
			EFormFieldCur.ConditionalValue=EFormL.CondValueStrConverter(_listEFormFields,textCondParent.Text,textCondValue.Text);
			if(comboDbLink.SelectedIndex==0){//None
				EFormFieldCur.DbLink="";
			}
			else{
				EFormFieldCur.DbLink=comboDbLink.GetSelected<string>();
			}
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
			//not saved to db here. That happens when clicking Save in parent window.
			IsDialogOK=true;
		}
  }
}