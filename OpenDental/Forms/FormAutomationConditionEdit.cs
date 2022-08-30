using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormAutomationConditionEdit:FormODBase {
		public bool IsNew;
		public AutomationCondition AutomationConditionCur;

		public FormAutomationConditionEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutomationConditionEdit_Load(object sender,EventArgs e) {
			for(int i=0;i<Enum.GetValues(typeof(AutoCondField)).Length;i++) {
				listCompareField.Items.Add(Lan.g("enumAutoCondField",((AutoCondField)i).GetDescription()));
				listCompareField.SelectedIndex=0;
			}
			for(int i=0;i<Enum.GetNames(typeof(AutoCondComparison)).Length;i++) {
				if(Enum.GetNames(typeof(AutoCondComparison))[i]=="None") {//Skip none as an option for the comparison field
					continue;
				}
				listComparison.Items.Add(Lan.g("enumAutoCondComparison",Enum.GetNames(typeof(AutoCondComparison))[i]));
				listComparison.SelectedIndex=0;
			}
			if(IsNew) {
				return;
			}
			textCompareString.Text=AutomationConditionCur.CompareString;
			listCompareField.SelectedIndex=(int)AutomationConditionCur.CompareField;
			if(!AutomationConditionCur.CompareField.In(AutoCondField.InsuranceNotEffective,AutoCondField.IsControlled,AutoCondField.IsProcRequired,
				AutoCondField.IsPatientInstructionPresent))
			{
				listComparison.SelectedIndex=(int)AutomationConditionCur.Comparison;
			}
			ShowOrHideFields((AutoCondField)listCompareField.SelectedIndex);
		}

		///<summary>Show or hides the form fileds based of needs.</summary>
		private void ShowOrHideFields(AutoCondField autoCondField) {
			if(autoCondField==AutoCondField.BillingType) {
				butBillingType.Visible=true;
				labelCompareString.Text=Lan.g(this,"Text (Type or pick from list) ");
			}
			else {
				butBillingType.Visible=false;
				labelCompareString.Text=Lan.g(this,"Text");
			}
			if(autoCondField.In(AutoCondField.InsuranceNotEffective,AutoCondField.IsControlled,AutoCondField.IsProcRequired
				,AutoCondField.IsPatientInstructionPresent)) 
			{//Hide fields
				labelWarning.Visible=true;
				if(autoCondField==AutoCondField.InsuranceNotEffective) {
					labelWarning.Text="At any point in time this rule is triggered, the current date and time will be compared to the "
						+"effective date range of the current patient's primary insurance.  If it does not fall within the range then the "
						+"action for this automation will be triggered.";
				}
				else {
					labelWarning.Text="This automation field will only work when using the RxCreate trigger.";
				}
				labelComparison.Visible=false;
				labelCompareString.Visible=false;
				listComparison.Visible=false;
				textCompareString.Visible=false;
			}
			else {//Show fields
				labelWarning.Visible=false;
				labelComparison.Visible=true;
				labelCompareString.Visible=true;
				listComparison.Visible=true;
				textCompareString.Visible=true;
			}
		}

		///<summary>Logic might get more complex as fields and comparisons are added so a seperate function was made.</summary>
		private bool ReasonableLogic() {
			AutoCondComparison autoCondComparison=(AutoCondComparison)listComparison.SelectedIndex;
			AutoCondField autoCondField=(AutoCondField)listCompareField.SelectedIndex;
			//So far Age is only thing that allows GreaterThan or LessThan.
			if(autoCondField!=AutoCondField.Age) {
				if(autoCondComparison==AutoCondComparison.GreaterThan || autoCondComparison==AutoCondComparison.LessThan) {
					return false;
				}
			}
			else {
				int age;
				//Make sure that the user typed in an integer and not a word.
				if(!int.TryParse(textCompareString.Text,out age)) {
					return false;
				}
			}
			return true;
		}

		private void listCompareField_Click(object sender,EventArgs e) {
			ShowOrHideFields((AutoCondField)listCompareField.SelectedIndex);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this condition?")) {
				return;
			}
			try {
				AutomationConditions.Delete(AutomationConditionCur.AutomationConditionNum);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butBillingType_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.BillingTypes);
			formDefinitionPicker.HasShowHiddenOption=false;
			formDefinitionPicker.IsMultiSelectionMode=false;
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult==DialogResult.OK) {
				textCompareString.Text=formDefinitionPicker.ListDefsSelected?[0]?.ItemName??"";
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if((AutoCondField)listCompareField.SelectedIndex==AutoCondField.InsuranceNotEffective
				|| (AutoCondField)listCompareField.SelectedIndex==AutoCondField.IsProcRequired
				|| (AutoCondField)listCompareField.SelectedIndex==AutoCondField.IsControlled
				|| (AutoCondField)listCompareField.SelectedIndex==AutoCondField.IsPatientInstructionPresent) 
			{
				AutomationConditionCur.CompareString=""; 
				AutomationConditionCur.Comparison=AutoCondComparison.None;
			}
			else {
				if(textCompareString.Text.Trim()=="") {
					MsgBox.Show(this,"Text not allowed to be blank.");
					return;
				}
				if(!ReasonableLogic()) {
					MsgBox.Show(this,"Comparison does not make sense with chosen field.");
					return;
				}
				if((AutoCondField)listCompareField.SelectedIndex==AutoCondField.Gender
					&& !(textCompareString.Text.ToLower()=="m" || textCompareString.Text.ToLower()=="f")) 
				{
					MsgBox.Show(this,"Allowed gender values are M or F.");
					return;
				}
				AutomationConditionCur.CompareString=textCompareString.Text;
				AutomationConditionCur.Comparison=(AutoCondComparison)listComparison.SelectedIndex;
			}
			AutomationConditionCur.CompareField=(AutoCondField)listCompareField.SelectedIndex;
			if(IsNew) {
				AutomationConditions.Insert(AutomationConditionCur);
			}
			else {
				AutomationConditions.Update(AutomationConditionCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}