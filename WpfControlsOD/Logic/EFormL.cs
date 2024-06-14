using OpenDental;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControls {
	public class EFormL {
		/// <summary>Returns an error msg and a SelectedValue.</summary>
		public static EFormConditionValueSetter SetCondValue(List<EFormField> listEFormFields,string condParent,string condValue) {
			EFormConditionValueSetter conditionValueSetter=new EFormConditionValueSetter();
			//first radiobuttons
			for(int i=0;i<listEFormFields.Count;i++){
				if(listEFormFields[i].FieldType!=EnumEFormFieldType.RadioButtons){
					continue;
				}
				if(listEFormFields[i].ValueLabel!=condParent){
					continue;
				}
				List<string> listStrPick=listEFormFields[i].PickListVis.Split(',').ToList();
				int idxSelected=listStrPick.IndexOf(condValue);//can be -1
				InputBox inputBox=new InputBox("",listStrPick,idxSelected);
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel){
					return conditionValueSetter;
				}
				//yes, this works even if no selected index. inputBox forces selected index
				conditionValueSetter.SelectedValue=listStrPick[inputBox.SelectedIndex];
				return conditionValueSetter;
			}
			//then checkboxes (untested)
			for(int i=0;i<listEFormFields.Count;i++){
				if(listEFormFields[i].FieldType!=EnumEFormFieldType.CheckBox){
					continue;
				}
				if(listEFormFields[i].ValueLabel!=condParent){
					continue;
				}
				List<string> listStrCheck=new List<string>();
				listStrCheck.Add("Checked");
				listStrCheck.Add("Unchecked");
				int idxSelected=listStrCheck.IndexOf(condValue);//can be -1;
				InputBox inputBox=new InputBox("",listStrCheck,idxSelected);
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					return conditionValueSetter;
				}
				conditionValueSetter.SelectedValue=listStrCheck[inputBox.SelectedIndex];
				return conditionValueSetter;
			}
			conditionValueSetter.ErrorMsg="Parent field name could not be found.";
			return conditionValueSetter;
		}

		public static string CondValueStrConverter(List<EFormField> listEFormFields,string labelSelected,string valueSelected) {
			for(int i=0;i<listEFormFields.Count;i++) {
				if(listEFormFields[i].FieldType!=EnumEFormFieldType.CheckBox) {
					continue;
				}
				if(listEFormFields[i].ValueLabel!=labelSelected) {
					continue;
				}
				if(valueSelected=="Checked") {
					return "X";
				}
				if(valueSelected=="Unchecked") {
					return "";
				}
				if(valueSelected=="X") {
					return "Checked";
				}
				if(valueSelected=="") {
					return "Unchecked";
				}
			}
			return valueSelected;//If it's not a checkbox, don't change the valueSelected.
		}
	}

	public class EFormConditionValueSetter {
		public string ErrorMsg="";
		public string SelectedValue="";
	}
}
