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

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmEFormFieldChildren : FrmODBase {
		///<summary>This is set from the parent form.</summary>
		public List<EFormField> ListEFormFieldsAll;
		///<summary>Set before opening and used after closing.</summary>
		public List<EFormField> ListEFormFieldsLinked;
		///<summary>The reason for this is to handle duplicates. This list will contain the duplicate, but not the listBox.</summary>
		private List<string> _listStrings;

		///<summary></summary>
		public FrmEFormFieldChildren() {
			InitializeComponent();
			Load+=FrmEFormFieldPicker_Load;
			listBoxFieldLabels.MouseDoubleClick+=ListBoxFieldLabels_MouseDoubleClick;
		}


		private void FrmEFormFieldPicker_Load(object sender, EventArgs e) {
			Lang.F(this);
			//_listStrings=new List<string>();
			//for(int i=0;i<ListEFormFields.Count;i++) {
			//	if(!ListEFormFields[i].FieldType.In(
			//		EnumEFormFieldType.CheckBox,
			//		EnumEFormFieldType.RadioButtons))
			//	{
			//		continue;
			//	}
			//	_listStrings.Add(ListEFormFields[i].ValueLabel);
			//}
			//for(int i=0;i<_listStrings.Count;i++) {
			//	if(_listStrings.Count(x=> x==_listStrings[i])==1){ 
			//		listBoxFieldLabels.Items.Add(_listStrings[i]);
			//	}
			//	else{
			//		if(!listBoxDups.Items.Contains(_listStrings[i])){
			//			listBoxDups.Items.Add(_listStrings[i]);
			//		}
			//	}
			//}
			//if(_listStrings.Count==0){
			//	MsgBox.Show("There are no suitable parent fields to pick from.");
			//	//IsDialogCancel=true;//leave it open in case there are fields on the right
			//	return;
			//}

		}

		private void ListBoxFieldLabels_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			//if(listBoxFieldLabels.Items.Count<=0 || listBoxFieldLabels.SelectedIndex==-1) {
			//	return;
			//}
			//string selected=listBoxFieldLabels.GetSelected<string>();
			//if(_listStrings.Count(x=>x==selected)>1){
			//	MsgBox.Show("That label is used more than once. The parent field must have a unique label.");
			//	return;
			//}
			//LabelSelected=selected;
			//IsDialogOK=true;
		}

		private void butSave_Click(object sender, EventArgs e) {
			//if(listBoxFieldLabels.SelectedIndex==-1) {
			//	MsgBox.Show("Please select an item first.");
			//	return;
			//}
			//string selected=listBoxFieldLabels.GetSelected<string>();
			//if(_listStrings.Count(x=>x==selected)>1){
			//	MsgBox.Show("That label is used more than once. The parent field must have a unique label.");
			//	return;
			//}
			//LabelSelected=selected;
			//IsDialogOK=true;
		}
	}
}