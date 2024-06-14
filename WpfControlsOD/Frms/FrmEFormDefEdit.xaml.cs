using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
/*
This Frm gets a lot of inspiration from FormSheetDefEdit.
Like in that form, edits to the fields do not get saved to the db as they are edited, but instead get saved when closing this Frm.
*/
	///<summary></summary>
	public partial class FrmEFormDefEdit : FrmODBase {
		///<summary>This is the object we are editing. After load, the list of attached EFormFieldDefs must be ignored. Instead, refer to CtrlEFormFill.ListEFormFields.</summary>
		public EFormDef EFormDefCur;
		private bool _isLoaded;
		///<summary>This gets set to true if the user clicked Delete on an internal form.</summary>
		public bool IsInternalDeleted;
		private ContextMenu contextMenu;

		#region Constructor
		///<summary></summary>
		public FrmEFormDefEdit() {
			InitializeComponent();
			Load+=FrmEFormDefEdit_Load;
			SizeChanged+=FrmEFormDefEdit_SizeChanged;
			//gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
			ctrlEFormFill.IsSetupMode=true;
			ctrlEFormFill.EventDoubleClickField+=CtrlEFormFill_EventDoubleClickField;
		}

		private void FrmEFormDefEdit_SizeChanged(object sender,SizeChangedEventArgs e) {
			if(!_isLoaded){
				return;
			}
			ctrlEFormFill.RefreshLayout();
		}
		#endregion Constructor

		#region Methods - private event handlers
		///<summary></summary>
		private void FrmEFormDefEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			ctrlEFormFill.ListEFormFields=EFormFields.FromListDefs(EFormDefCur.ListEFormFieldDefs);
			ctrlEFormFill.RefreshLayout();
			textDescription.Text=EFormDefCur.Description;
			contextMenu=new ContextMenu(this);
			//gridMain.ContextMenu=contextMenu;
			//contextMenu.Add(new MenuItem("Delete",menuDelete_Click));
			if(!EFormDefCur.IsInternal) {
				labelTitle.Text="This is a custom eForm, not internal.";
			}
			_isLoaded=true;
		}

		///<summary></summary>
		private void CtrlEFormFill_EventDoubleClickField(object sender,int idx) {
			if(ctrlEFormFill.ListEFormFields[idx].FieldType==EnumEFormFieldType.TextField){
				FrmEFormTextBoxEdit frmEFormTextBoxEdit=new FrmEFormTextBoxEdit();
				frmEFormTextBoxEdit.EFormFieldCur=ctrlEFormFill.ListEFormFields[idx];
				frmEFormTextBoxEdit._listEFormFields=ctrlEFormFill.ListEFormFields;
				frmEFormTextBoxEdit.ShowDialog();
				if(frmEFormTextBoxEdit.IsDialogCancel){
					return;
				}
				if(frmEFormTextBoxEdit.EFormFieldCur==null){
					ctrlEFormFill.Delete(idx);
					ctrlEFormFill.RefreshLayout();
					return;
				}
			}
			if(ctrlEFormFill.ListEFormFields[idx].FieldType==EnumEFormFieldType.Label){
				FrmEFormLabelEdit frmEFormLabelEdit=new FrmEFormLabelEdit();
				frmEFormLabelEdit.EFormFieldCur=ctrlEFormFill.ListEFormFields[idx];
				frmEFormLabelEdit.EFormDefCur=EFormDefCur;
				frmEFormLabelEdit.ShowDialog();
				if(frmEFormLabelEdit.IsDialogCancel){
					return;
				}
				if(frmEFormLabelEdit.EFormFieldCur==null){
					ctrlEFormFill.Delete(idx);
					ctrlEFormFill.RefreshLayout();
					return;
				}
			}
			if(ctrlEFormFill.ListEFormFields[idx].FieldType==EnumEFormFieldType.DateField){
				FrmEFormDateFieldEdit frmEFormDateFieldEdit=new FrmEFormDateFieldEdit();
				frmEFormDateFieldEdit.EFormFieldCur=ctrlEFormFill.ListEFormFields[idx];
				frmEFormDateFieldEdit.EFormDefCur=EFormDefCur;
				frmEFormDateFieldEdit.ShowDialog();
				if(frmEFormDateFieldEdit.IsDialogCancel){
					return;
				}
				if(frmEFormDateFieldEdit.EFormFieldCur==null){
					ctrlEFormFill.Delete(idx);
					ctrlEFormFill.RefreshLayout();
					return;
				}
			}
			if(ctrlEFormFill.ListEFormFields[idx].FieldType==EnumEFormFieldType.CheckBox){
				FrmEFormCheckBoxEdit frmEFormCheckBoxEdit=new FrmEFormCheckBoxEdit();
				frmEFormCheckBoxEdit.EFormFieldCur=ctrlEFormFill.ListEFormFields[idx];
				frmEFormCheckBoxEdit.EFormDefCur=EFormDefCur;
				frmEFormCheckBoxEdit.ShowDialog();
				if(frmEFormCheckBoxEdit.IsDialogCancel){
					return;
				}
				if(frmEFormCheckBoxEdit.EFormFieldCur==null){
					ctrlEFormFill.Delete(idx);
					ctrlEFormFill.RefreshLayout();
					return;
				}
			}
			if(ctrlEFormFill.ListEFormFields[idx].FieldType==EnumEFormFieldType.RadioButtons){
				FrmEFormRadioButtonsEdit frmEFormRadioButtonsEdit=new FrmEFormRadioButtonsEdit();
				frmEFormRadioButtonsEdit.EFormFieldCur=ctrlEFormFill.ListEFormFields[idx];
				frmEFormRadioButtonsEdit._listEFormFields=ctrlEFormFill.ListEFormFields;
				frmEFormRadioButtonsEdit.ShowDialog();
				if(frmEFormRadioButtonsEdit.IsDialogCancel){
					return;
				}
				if(frmEFormRadioButtonsEdit.EFormFieldCur==null){
					ctrlEFormFill.Delete(idx);
					ctrlEFormFill.RefreshLayout();
					return;
				}
			}
			if(ctrlEFormFill.ListEFormFields[idx].FieldType==EnumEFormFieldType.SigBox){
				FrmEFormSigBoxEdit frmEFormSigBoxEdit=new FrmEFormSigBoxEdit();
				frmEFormSigBoxEdit.EFormFieldCur=ctrlEFormFill.ListEFormFields[idx];
				frmEFormSigBoxEdit.EFormDefCur=EFormDefCur;
				frmEFormSigBoxEdit.ShowDialog();
				if(frmEFormSigBoxEdit.IsDialogCancel){
					return;
				}
				if(frmEFormSigBoxEdit.EFormFieldCur==null){
					ctrlEFormFill.Delete(idx);
					ctrlEFormFill.RefreshLayout();
					return;
				}
			}
			if(ctrlEFormFill.ListEFormFields[idx].FieldType==EnumEFormFieldType.MedicationList){
				FrmEFormMedicationListEdit frmEFormMedicationListEdit=new FrmEFormMedicationListEdit();
				frmEFormMedicationListEdit.EFormFieldCur=ctrlEFormFill.ListEFormFields[idx];
				frmEFormMedicationListEdit.EFormDefCur=EFormDefCur;
				frmEFormMedicationListEdit.ShowDialog();
				if(frmEFormMedicationListEdit.IsDialogCancel){
					return;
				}
				if(frmEFormMedicationListEdit.EFormFieldCur==null){
					ctrlEFormFill.Delete(idx);
					ctrlEFormFill.RefreshLayout();
					return;
				}
			}
			ctrlEFormFill.RefreshLayout();
		}

		private void AddNewField(EFormField eFormField) {
			if(ctrlEFormFill.ListEFormFields.Count==0) {//This handles when the form is empty and adding the first field.
				ctrlEFormFill.ListEFormFields.Add(eFormField);
				ctrlEFormFill.RefreshLayout();
				return;
			}
			int idx=ctrlEFormFill.GetSelectedIndex();
			bool wasFieldSelected=true;
			if(idx==-1){
				//Set the idx value to the end of the current page showing.
				wasFieldSelected=false;
				int pageShowing=ctrlEFormFill.GetPageShowing();//Will never be 0 because 1-based.
				int numberOfPageBreaks=0;
				for(int i=0;i<ctrlEFormFill.ListEFormFields.Count;i++) {
					if(ctrlEFormFill.ListEFormFields[i].FieldType==EnumEFormFieldType.PageBreak) {
						numberOfPageBreaks++;
					}
					if(numberOfPageBreaks==pageShowing) {
						idx=i;//the page break at the bottom of the page we are on.
						break;
					}
				}
				if(idx==-1) {//Still -1. This is becuase we are looking at the last page.
					idx=ctrlEFormFill.ListEFormFields.Count-1;
				}
			}
			ctrlEFormFill.ListEFormFields.Insert(idx,eFormField);
			ctrlEFormFill.RefreshLayout();
			if(wasFieldSelected){
				ctrlEFormFill.SetSelected(idx+1);
			}
			else{//No field was selected. Set selected to the new field.
				ctrlEFormFill.SetSelected(idx);
			}
		}
		#endregion Methods - private event handlers

		#region Methods - private Add buttons
		private void butTextField_Click(object sender,EventArgs e) {
			FrmEFormTextBoxEdit frmEFormTextBoxEdit=new FrmEFormTextBoxEdit();
			EFormField eFormField=new EFormField();
			eFormField.IsNew=true;
			eFormField.FontScale=100;
			eFormField.FieldType=EnumEFormFieldType.TextField;
			frmEFormTextBoxEdit.EFormFieldCur=eFormField;
			frmEFormTextBoxEdit._listEFormFields=ctrlEFormFill.ListEFormFields;
			frmEFormTextBoxEdit.ShowDialog();
			if(frmEFormTextBoxEdit.IsDialogCancel){
				return;
			}
			if(frmEFormTextBoxEdit.EFormFieldCur==null){
				//they clicked Delete for some reason, which is the same as cancel.
				return;
			}
			AddNewField(eFormField);//This will refresh the layout and set a selected field.
		}

		private void butLabel_Click(object sender,EventArgs e) {
			FrmEFormLabelEdit frmEFormLabelEdit=new FrmEFormLabelEdit();
			EFormField eFormField=new EFormField();
			eFormField.IsNew=true;
			eFormField.FontScale=100;
			eFormField.FieldType=EnumEFormFieldType.Label;
			frmEFormLabelEdit.EFormFieldCur=eFormField;
			frmEFormLabelEdit.EFormDefCur=EFormDefCur;
			frmEFormLabelEdit.ShowDialog();
			if(frmEFormLabelEdit.IsDialogCancel){
				return;
			}
			if(frmEFormLabelEdit.EFormFieldCur==null){
				//they clicked Delete for some reason, which is the same as cancel.
				return;
			}
			AddNewField(eFormField);//This will refresh the layout and set a selected field.
		}

		private void butDateField_Click(object sender,EventArgs e) {
			FrmEFormDateFieldEdit frmEFormDateFieldEdit=new FrmEFormDateFieldEdit();
			EFormField eFormField=new EFormField();
			eFormField.IsNew=true;
			eFormField.FontScale=100;
			eFormField.FieldType=EnumEFormFieldType.DateField;
			frmEFormDateFieldEdit.EFormFieldCur=eFormField;
			frmEFormDateFieldEdit.EFormDefCur=EFormDefCur;
			frmEFormDateFieldEdit.ShowDialog();
			if(frmEFormDateFieldEdit.IsDialogCancel){
				return;
			}
			if(frmEFormDateFieldEdit.EFormFieldCur==null){
				//they clicked Delete for some reason, which is the same as cancel.
				return;
			}
			AddNewField(eFormField);//This will refresh the layout and set a selected field.
		}

		private void butCheckBox_Click(object sender,EventArgs e) {
			FrmEFormCheckBoxEdit frmEFormCheckBoxEdit=new FrmEFormCheckBoxEdit();
			EFormField eFormField=new EFormField();
			eFormField.IsNew=true;
			eFormField.FontScale=100;
			eFormField.FieldType=EnumEFormFieldType.CheckBox;
			frmEFormCheckBoxEdit.EFormFieldCur=eFormField;
			frmEFormCheckBoxEdit.EFormDefCur=EFormDefCur;
			frmEFormCheckBoxEdit.ShowDialog();
			if(frmEFormCheckBoxEdit.IsDialogCancel){
				return;
			}
			if(frmEFormCheckBoxEdit.EFormFieldCur==null){
				//they clicked Delete for some reason, which is the same as cancel.
				return;
			}
			AddNewField(eFormField);//This will refresh the layout and set a selected field.
		}

		private void butRadioButtons_Click(object sender,EventArgs e) {
			FrmEFormRadioButtonsEdit frmEFormRadioButtonsEdit=new FrmEFormRadioButtonsEdit();
			EFormField eFormField=new EFormField();
			eFormField.IsNew=true;
			eFormField.FontScale=100;
			eFormField.FieldType=EnumEFormFieldType.RadioButtons;
			frmEFormRadioButtonsEdit.EFormFieldCur=eFormField;
			frmEFormRadioButtonsEdit._listEFormFields=ctrlEFormFill.ListEFormFields;
			frmEFormRadioButtonsEdit.ShowDialog();
			if(frmEFormRadioButtonsEdit.IsDialogCancel){
				return;
			}
			if(frmEFormRadioButtonsEdit.EFormFieldCur==null){
				//they clicked Delete for some reason, which is the same as cancel.
				return;
			}
			AddNewField(eFormField);//This will refresh the layout and set a selected field.
		}

		private void butSigBox_Click(object sender,EventArgs e) {
			FrmEFormSigBoxEdit frmEFormSigBoxEdit=new FrmEFormSigBoxEdit();
			EFormField eFormField=new EFormField();
			eFormField.IsNew=true;
			eFormField.FontScale=100;
			eFormField.FieldType= EnumEFormFieldType.SigBox;
			eFormField.ValueLabel="Signature";
			frmEFormSigBoxEdit.EFormFieldCur=eFormField;
			frmEFormSigBoxEdit.EFormDefCur=EFormDefCur;
			frmEFormSigBoxEdit.ShowDialog();
			if(frmEFormSigBoxEdit.IsDialogCancel){
				return;
			}
			if(frmEFormSigBoxEdit.EFormFieldCur==null){
				//they clicked Delete for some reason which is the same as cancel.
				return;
			}
			AddNewField(eFormField);//This will refresh the layout and set a selected field.
		}

		private void butMedicationList_Click(object sender,EventArgs e) {
			FrmEFormMedicationListEdit frmEFormMedicationListEdit=new FrmEFormMedicationListEdit();
			EFormField eFormField=new EFormField();
			eFormField.IsNew=true;
			eFormField.FontScale=100;
			eFormField.FieldType=EnumEFormFieldType.MedicationList;
			EFormMedListLayout eFormMedListLayout=new EFormMedListLayout();
			eFormField.ValueLabel=JsonConvert.SerializeObject(eFormMedListLayout);
			frmEFormMedicationListEdit.EFormFieldCur=eFormField;
			frmEFormMedicationListEdit.EFormDefCur=EFormDefCur;
			frmEFormMedicationListEdit.ShowDialog();
			if(frmEFormMedicationListEdit.IsDialogCancel){
				return;
			}
			if(frmEFormMedicationListEdit.EFormFieldCur==null){
				//they clicked Delete for some reason, which is the same as cancel.
				return;
			}
			AddNewField(eFormField);//This will refresh the layout and set a selected field.
		}

		private void butPageBreak_Click(object sender,EventArgs e) {
			if(ctrlEFormFill.ListEFormFields.Count==0) {//Don't allow a page break to be the first item in the grid.
				MsgBox.Show(this,"Cannot add a page break before adding fields first.");
				return;
			}
			EFormField eFormField =new EFormField();
			eFormField.IsNew=true;
			eFormField.FontScale=100;
			eFormField.FieldType=EnumEFormFieldType.PageBreak;
			AddNewField(eFormField);//This will refresh the layout and set a selected field.
		}
		#endregion Methods - private Add buttons

		#region Methods - private other buttons
		///<summary></summary>
		private void butEditProp_Click(object sender,EventArgs e) {
			FrmEFormDef frmEFormDef=new FrmEFormDef();
			frmEFormDef.EFormDefCur=EFormDefCur;
			frmEFormDef.ShowDialog();
			if(frmEFormDef.IsDialogCancel){
				return;
			}
			textDescription.Text=EFormDefCur.Description;
		}

		///<summary></summary>
		private void butSetCondition_Click(object sender,EventArgs e) {
			List<int> listSelectedIndices=ctrlEFormFill.GetSelectedIndices();
			if(listSelectedIndices.Count<2) {
				MsgBox.Show(this,"This is used to set the decision on multiple fields. Please select more than 1 field.");
				return;
			}
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=ctrlEFormFill.ListEFormFields;
			frmEFormFieldPicker.ShowValueSelector=true;
			frmEFormFieldPicker.ListSelectedIndices=listSelectedIndices;
			frmEFormFieldPicker.ShowDialog();
			if(frmEFormFieldPicker.IsDialogCancel){
				return;
			}
			for(int i=0;i<listSelectedIndices.Count;i++) {
				ctrlEFormFill.ListEFormFields[listSelectedIndices[i]].ConditionalParent=frmEFormFieldPicker.LabelSelected;
				ctrlEFormFill.ListEFormFields[listSelectedIndices[i]].ConditionalValue=frmEFormFieldPicker.ValueSelected;
			}
			ctrlEFormFill.RefreshLayout();
		}

		///<summary></summary>
		private void butSetPageCondition_Click(object sender,EventArgs e) {
			List<int> listSelectedIndices=new List<int>();
			//Get all of the field indices on the currently viewed page.
			for(int i=0;i<ctrlEFormFill.ListEFormFields.Count;i++) {
				if(ctrlEFormFill.ListEFormFields[i].FieldType==EnumEFormFieldType.PageBreak) {
					continue;
				}
				if(ctrlEFormFill.ListEFormFields[i].Page==ctrlEFormFill.GetPageShowing()) {
					listSelectedIndices.Add(i);
				}
			}
			FrmEFormFieldPicker frmEFormFieldPicker=new FrmEFormFieldPicker();
			frmEFormFieldPicker.ListEFormFields=ctrlEFormFill.ListEFormFields;
			frmEFormFieldPicker.ShowValueSelector=true;
			frmEFormFieldPicker.ListSelectedIndices=listSelectedIndices;
			frmEFormFieldPicker.ShowDialog();
			if(frmEFormFieldPicker.IsDialogCancel) {
				return;
			}
			for(int i=0;i<listSelectedIndices.Count;i++) {
				ctrlEFormFill.ListEFormFields[listSelectedIndices[i]].ConditionalParent=frmEFormFieldPicker.LabelSelected;
				ctrlEFormFill.ListEFormFields[listSelectedIndices[i]].ConditionalValue=frmEFormFieldPicker.ValueSelected;
			}
			ctrlEFormFill.RefreshLayout();
		}

		/*
		///<summary></summary>
		private void butUp_Click(object sender,EventArgs e) {
			List<int> listSelectedIndices=ctrlEFormFill.GetSelectedIndices();
			if(listSelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(listSelectedIndices.Contains(0)) {
				return;
			}
			for(int i=0;i<listSelectedIndices.Count;i++) { 
				EFormField eFormFieldAbove=ctrlEFormFill.ListEFormFields[listSelectedIndices[i]-1];
				ctrlEFormFill.ListEFormFields[listSelectedIndices[i]-1]=ctrlEFormFill.ListEFormFields[listSelectedIndices[i]];
				ctrlEFormFill.ListEFormFields[listSelectedIndices[i]]=eFormFieldAbove;
			}
			ctrlEFormFill.RefreshLayout();
			for(int i=0;i<listSelectedIndices.Count;i++) { 
				ctrlEFormFill.SetSelected(listSelectedIndices[i]-1);
			}
		}

		///<summary></summary>
		private void butDown_Click(object sender,EventArgs e) {
			List<int> listSelectedIndices=ctrlEFormFill.GetSelectedIndices();
			listSelectedIndices.Reverse();//Reverse the list so that we start with the lowest GridRow.
			if(listSelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			if(listSelectedIndices.Contains(ctrlEFormFill.ListEFormFields.Count-1)) {
				return;
			}
			for(int i=0;i<listSelectedIndices.Count;i++) {
				EFormField eFormFieldBelow=ctrlEFormFill.ListEFormFields[listSelectedIndices[i]+1];
				ctrlEFormFill.ListEFormFields[listSelectedIndices[i]+1]=ctrlEFormFill.ListEFormFields[listSelectedIndices[i]];
				ctrlEFormFill.ListEFormFields[listSelectedIndices[i]]=eFormFieldBelow;
			}
			ctrlEFormFill.RefreshLayout();
			for(int i=0;i<listSelectedIndices.Count;i++) { 
				ctrlEFormFill.SetSelected(listSelectedIndices[i]+1);
			}
		}*/

		///<summary></summary>
		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entire form?")){
				return;
			}
			if(EFormDefCur.IsInternal){
				IsInternalDeleted=true;
				IsDialogCancel=true;
				return;
			}
			if(EFormDefCur.IsNew){
				IsDialogCancel=true;
				return;
			}
			EFormFieldDefs.DeleteForForm(EFormDefCur.EFormDefNum);
			EFormDefs.Delete(EFormDefCur.EFormDefNum);
			IsDialogOK=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(EFormDefCur.Description)){
				MsgBox.Show(this,"Description is required");
				return;
			}
			for(int i=0;i<ctrlEFormFill.ListEFormFields.Count-1;i++){//the -1 here allows comparing i+1
				if(ctrlEFormFill.ListEFormFields[i].FieldType==EnumEFormFieldType.PageBreak 
					&& ctrlEFormFill.ListEFormFields[i+1].FieldType==EnumEFormFieldType.PageBreak)
				{
					MsgBox.Show(this,"Cannot have two page breaks back to back.");
				}
			}
			//End of validation
			//This is where everything goes to the database
			if(EFormDefCur.IsNew){
				EFormDefCur.DateTCreated=DateTime.Now;
				EFormDefs.Insert(EFormDefCur);
			}
			else{
				EFormDefs.Update(EFormDefCur);
			}
			//Delete all of the fieldDefs that are on the form from the database and re-insert them.
			EFormFieldDefs.DeleteForForm(EFormDefCur.EFormDefNum);
			for(int i=0;i<ctrlEFormFill.ListEFormFields.Count;i++) {
				EFormFieldDef eFormFieldDef=EFormFields.ToDef(ctrlEFormFill.ListEFormFields[i]);
				eFormFieldDef.EFormDefNum=EFormDefCur.EFormDefNum;
				eFormFieldDef.ItemOrder=i;
				EFormFieldDefs.Insert(eFormFieldDef);//ignores any existing PK when inserting
			}
			IsDialogOK=true;
		}

		#endregion Methods - private other buttons


	}
}