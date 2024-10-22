using OpenDental;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControls.UI;
using CodeBase;

namespace WpfControls {
	public class EFormL {
		/// <summary>Shows InputBox to user to let them pick a value. Pass in an existingValue to prefill.</summary>
		public static string PickCondValue(List<EFormField> listEFormFields,string condParent,string existingValue) {
			if(condParent==""){
				MsgBox.Show("Please enter a name in the Parent field first.");
				return existingValue;
			}
			List<EFormField> listEFormFieldsParent=listEFormFields.FindAll(x=>x.ValueLabel==condParent);
			if(listEFormFieldsParent.Count==0){
				MsgBox.Show("Parent field name could not be found.");
				return existingValue;
			}
			if(listEFormFieldsParent.Count>1){
				MsgBox.Show("There are duplicate fields with that parent name.");
				return existingValue;
			}
			EFormField eFormFieldParent=listEFormFieldsParent[0];
			if(eFormFieldParent.FieldType==EnumEFormFieldType.RadioButtons){
				List<string> listPick=eFormFieldParent.PickListVis.Split(',').ToList();
				if(eFormFieldParent.DbLink!=""){
					//parent has a db link, so we might need to use some of those values if the Vis items are empty
					List<string> listPickDb=eFormFieldParent.PickListDb.Split(',').ToList();
					for(int i=0;i<listPick.Count;i++){
						if(listPick[i]==""){
							listPick[i]=listPickDb[i];
						}
					}
				}
				int idxSelected=listPick.IndexOf(existingValue);//can be -1
				InputBoxParam inputBoxParam=new InputBoxParam();
				inputBoxParam.InputBoxType_=InputBoxType.ListBox;
				inputBoxParam.LabelText="";
				inputBoxParam.ListSelections=listPick;
				inputBoxParam.SelectedIndex=idxSelected;
				InputBox inputBox=new InputBox(inputBoxParam);
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel){
					return existingValue;
				}
				//yes, this works even if no selected index. inputBox forces selected index
				return listPick[inputBox.SelectedIndex];
			}
			if(eFormFieldParent.FieldType==EnumEFormFieldType.CheckBox){
				List<string> listStrCheck=new List<string>();
				listStrCheck.Add("Checked");
				listStrCheck.Add("Unchecked");
				int idxSelected=listStrCheck.IndexOf(existingValue);//can be -1;
				InputBox inputBox=new InputBox(prompt:"",listSelections:listStrCheck,selectedIndex:idxSelected);
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					return existingValue;
				}
				return listStrCheck[inputBox.SelectedIndex];
			}
			if(eFormFieldParent.FieldType==EnumEFormFieldType.DateField){
				List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
				InputBoxParam inputBoxParamYounger=new InputBoxParam();
				inputBoxParamYounger.InputBoxType_=InputBoxType.RadioButton;
				inputBoxParamYounger.Text="Younger than";
				listInputBoxParams.Add(inputBoxParamYounger);
				InputBoxParam inputBoxParamOlder=new InputBoxParam();
				inputBoxParamOlder.InputBoxType_=InputBoxType.RadioButton;
				inputBoxParamOlder.Text="Older than";
				listInputBoxParams.Add(inputBoxParamOlder);
				InputBoxParam inputBoxAge=new InputBoxParam();
				inputBoxAge.InputBoxType_=InputBoxType.TextBox;
				inputBoxAge.LabelText="Age";
				listInputBoxParams.Add(inputBoxAge);
				InputBox inputBox=new InputBox(listInputBoxParams);
				//inputBox.SizeInitial=new System.Windows.Size(200,200);
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					return existingValue;
				}
				RadioButton radioButtonYounger=(RadioButton)inputBox.ListControls[0];
				RadioButton radioButtonOlder=(RadioButton)inputBox.ListControls[1];
				TextBox textAge=(TextBox)inputBox.ListControls[2];
				string retVal="";
				if(radioButtonYounger.Checked){
					retVal+="<";
				}
				if(radioButtonOlder.Checked){
					retVal+=">";
				}
				retVal+=textAge.Text;
				return retVal;
			}
			return existingValue;
		}

		///<summary>This handles the problem of parent labels that are longer than 255. Also if the parent is a checkBox, then this converts "" or "X" to Checked or Unchecked to make it more user friendly for display. It doesn't alter other types.</summary>
		public static string ConvertCondDbToVis(List<EFormField> listEFormFields,string parent,string dbVal){
			//if(parent==""){//this would have worked, but I would rather roll it into the predicate below so that the same predicate can be used elsewhere
			//	return dbVal;
			//}
			//So the trick here is that the parent value coming from the db won't be >255, but eFormFieldParent.ValueLabel certainly could be
			EFormField eFormFieldParent=listEFormFields.Find(x=>
				x.ValueLabel!=""
				&& x.ValueLabel.Substring(0,Math.Min(x.ValueLabel.Length,255))==parent
				&& x.FieldType.In(EnumEFormFieldType.CheckBox,EnumEFormFieldType.RadioButtons,EnumEFormFieldType.DateField));
			//note that in the linq above, we did not validate dbVal in any way. We're just trying to find the parent.
			if(eFormFieldParent is null){
				return dbVal;
			}
			if(eFormFieldParent.FieldType!=EnumEFormFieldType.CheckBox) {
				return dbVal;
			}
			if(dbVal==""){
				return "Unchecked";
			}
			if(dbVal=="X"){
				return "Checked";
			}
			return dbVal;
		}

		///<summary>This handles the problem of parent labels that are longer than 255. Also if the parent is a checkBox, then this converts Checked or Unchecked to "" or "X" for storage in db. It doesn't alter other types.</summary>
		public static string ConvertCondVisToDb(List<EFormField> listEFormFields,string parent,string visVal){
			EFormField eFormFieldParent=listEFormFields.Find(x=>
				x.ValueLabel!=""
				&& x.ValueLabel.Substring(0,Math.Min(x.ValueLabel.Length,255))==parent
				&& x.FieldType.In(EnumEFormFieldType.CheckBox,EnumEFormFieldType.RadioButtons,EnumEFormFieldType.DateField));
			if(eFormFieldParent is null){
				return visVal;
			}
			if(eFormFieldParent.FieldType!=EnumEFormFieldType.CheckBox) {
				return visVal;
			}
			if(visVal=="Unchecked"){
				return "";
			}
			if(visVal=="Checked"){
				return "X";
			}
			return "";
		}

		///<summary>Fills a combo language box on a variety of eForm windows. Sets it visible false if the office has not set up any languages.</summary>
		public static void FillComboLanguage(ComboBox comboLanguage){
			comboLanguage.Items.Clear();
			comboLanguage.Items.Add(Lang.g("EFormLanguage","Default"));
			comboLanguage.SelectedIndex=0;
			List<string> listLangs=LanguagePats.GetLanguagesForCombo();
			if(listLangs.Count==0){
				comboLanguage.Visible=false;
				return;
			}
			for(int i = 0;i<listLangs.Count;i++){
				comboLanguage.Items.Add(listLangs[i]);
			}
		}
	}
}
