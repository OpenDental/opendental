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
using CodeBase;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmEFormFieldPicker : FrmODBase {
		///<summary>This is set from the parent form.</summary>
		public List<EFormField> ListEFormFields;
		///<summary>Upon closing, this is the label from the selected eForm field.</summary>
		public string ParentSelected;
		///<summary>Upon closing, this is the value from the selected eForm field. This will only have a value when multiple fields are selected and setting the condition; otherwise, null.</summary>
		public string ValueSelected;
		///<summary>Set to true to show groupParentValue and change some wording. Used for multiple fields and for entire page.</summary>
		public bool IsMultiple;
		///<summary>These are the indices of the fields we are setting conditions for. Allows preventing showing self.</summary>
		public List<int> ListSelectedIndices=new List<int>();
		///<summary>The reason for this is to handle duplicates. This list will contain the duplicate, but not the listBox.</summary>
		private List<string> _listStrings;

		///<summary></summary>
		public FrmEFormFieldPicker() {
			InitializeComponent();
			Load+=FrmEFormFieldPicker_Load;
			//listBoxFieldLabels.MouseDoubleClick+=ListBoxFieldLabels_MouseDoubleClick;
			//listBoxFieldLabels.SelectionChangeCommitted+=ListBoxFieldLabels_SelectionChangeCommitted;
		}


		private void FrmEFormFieldPicker_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(IsMultiple){
				labelSelect.Text="Select the field below which will be the parent responsible for showing or hiding the fields. Only check boxes and radio buttons are available";
			}
			groupParentValue.Visible=IsMultiple;
			_listStrings=new List<string>();
			for(int i=0;i<ListEFormFields.Count;i++) {
				if(!ListEFormFields[i].FieldType.In(
					EnumEFormFieldType.CheckBox,
					EnumEFormFieldType.RadioButtons,
					EnumEFormFieldType.DateField))
				{
					continue;
				}
				if(ListEFormFields[i].ValueLabel==""){
					continue;
				}
				string parent=ListEFormFields[i].ValueLabel;
				if(parent.Length>255){
					parent=parent.Substring(0,255);
				}
				if(ListSelectedIndices.Contains(i)) {//Don't add the label if it was selected.
					continue;
				}
				_listStrings.Add(parent);
			}
			for(int i=0;i<_listStrings.Count;i++) {
				if(_listStrings.Count(x=> x==_listStrings[i])==1){ 
					listBoxFieldLabels.Items.Add(_listStrings[i]);
				}
				else{
					if(!listBoxDups.Items.Contains(_listStrings[i])){
						listBoxDups.Items.Add(_listStrings[i]);
					}
				}
			}
			if(_listStrings.Count==0){
				MsgBox.Show("There are no suitable parent fields to pick from.");
				//IsDialogCancel=true;//leave it open in case there are fields on the right
				return;
			}
		}

		private void butPickValue_Click(object sender,EventArgs e) {
			textCondValue.Text=EFormL.PickCondValue(ListEFormFields,listBoxFieldLabels.SelectedItem.ToString(),textCondValue.Text);
		}

		/*
		private void ListBoxFieldLabels_SelectionChangeCommitted(object sender,EventArgs e) {
			if(listBoxFieldLabels.Items.Count<=0 || listBoxFieldLabels.SelectedIndex==-1) {
				return;
			}
			string labelSelected=listBoxFieldLabels.GetSelected<string>();
			//Radiobuttons
			for(int i=0;i<ListEFormFields.Count;i++){
				if(ListEFormFields[i].FieldType!=EnumEFormFieldType.RadioButtons){
					continue;
				}
				if(ListEFormFields[i].ValueLabel!=labelSelected){
					continue;
				}
				List<string> listStrPick=ListEFormFields[i].PickListVis.Split(',').ToList();
				comboParentValue.Items.Clear();
				comboParentValue.Items.AddList(listStrPick);
				return;
			}
			//Checkboxes
			for(int i=0;i<ListEFormFields.Count;i++) {
				if(ListEFormFields[i].FieldType!=EnumEFormFieldType.CheckBox) {
					continue;
				}
				if(ListEFormFields[i].ValueLabel!=labelSelected) {
					continue;
				}
				comboParentValue.Items.Clear();
				comboParentValue.Items.Add("Checked");
				comboParentValue.Items.Add("Unchecked");
				return;
			}
			//Dates
//todo:
			for(int i=0;i<ListEFormFields.Count;i++) {
				if(ListEFormFields[i].FieldType!=EnumEFormFieldType.CheckBox) {
					continue;
				}
				if(ListEFormFields[i].ValueLabel!=labelSelected) {
					continue;
				}
				comboParentValue.Items.Clear();
				comboParentValue.Items.Add("Checked");
				comboParentValue.Items.Add("Unchecked");
				return;
			}
		}*/

		/*
		private void ListBoxFieldLabels_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			if(listBoxFieldLabels.Items.Count<=0 || listBoxFieldLabels.SelectedIndex==-1) {
				return;
			}
			if(groupParentValue.Visible && comboParentValue.SelectedIndex==-1) {
				MsgBox.Show("Please select a value before continuing");
				return;
			}
			string labelSelected=listBoxFieldLabels.GetSelected<string>();
			if(_listStrings.Count(x=>x==labelSelected)>1){//I don't think this is possible since listBoxFieldsLabels doesn't show duplicate labels as options to choose from.
				MsgBox.Show("That label is used more than once. The parent field must have a unique label.");
				return;
			}
			if(labelSelected.Length>255) {
				labelSelected=labelSelected.Substring(0,255);
			}
			string valueSelected=comboParentValue.GetStringSelectedItems();
			for(int i=0;i<ListEFormFields.Count;i++) {
				if(ListEFormFields[i].FieldType!=EnumEFormFieldType.CheckBox) {
					continue;
				}
				if(ListEFormFields[i].ValueLabel!=labelSelected) {
					continue;
				}
				if(valueSelected=="Checked") {//Change it to "X" to store it in the database and for validating.
					valueSelected="X";
				}
				else {//Unchecked
					valueSelected="";
				}
				break;
			}
			LabelSelected=labelSelected;
			ValueSelected=valueSelected;
			IsDialogOK=true;
		}*/

		private void butSave_Click(object sender, EventArgs e) {
			if(listBoxFieldLabels.SelectedIndex==-1) {
				MsgBox.Show("Please select an item first.");
				return;
			}
			//if(groupParentValue.Visible && textCondValue.Text=="") {//no, allow them to set it to blank
			//	MsgBox.Show("Please select a value before continuing");
			//	return;
			//}
			string parent=listBoxFieldLabels.GetSelected<string>();
			if(_listStrings.Count(x=>x==parent)>1){//I don't think this is possible since listBoxFieldsLabels doesn't show duplicate labels as options to choose from.
				MsgBox.Show("That label is used more than once. The parent field must have a unique label.");
				return;
			}
			if(parent.Length>255) {
				parent=parent.Substring(0,255);
			}
			ValueSelected=EFormL.ConvertCondVisToDb(ListEFormFields,parent,textCondValue.Text);
			ParentSelected=parent;
			IsDialogOK=true;
		}
	}
}