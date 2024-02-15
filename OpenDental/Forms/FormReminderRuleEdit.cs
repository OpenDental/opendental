using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormReminderRuleEdit:FormODBase {
		public ReminderRule ReminderRuleCur = new ReminderRule();
		//both the RuleCur.ReminderCriterion and RuleCur.CriterionFK will be altered below in response to user actions.
		public bool IsNew;

		public FormReminderRuleEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReminderRuleEdit_Load(object sender,EventArgs e) {
			for(int i=0;i<Enum.GetNames(typeof(EhrCriterion)).Length;i++) {
				comboReminderCriterion.Items.Add(Enum.GetNames(typeof(EhrCriterion))[i]);
			}
			comboReminderCriterion.SelectedIndex=(int)ReminderRuleCur.ReminderCriterion;
			textCriterionValue.Text=ReminderRuleCur.CriterionValue;
			textReminderMessage.Text=ReminderRuleCur.Message;
			FillFK();
		}

		private void FillFK() {
			if(ReminderRuleCur.CriterionFK==-1 || ReminderRuleCur.CriterionFK==0) {
				textCriterionFK.Text="";
				return;
			}
			switch((EhrCriterion)comboReminderCriterion.SelectedIndex) {
				case EhrCriterion.Problem:
					DiseaseDef diseaseDef=DiseaseDefs.GetItem(ReminderRuleCur.CriterionFK);
					textCriterionFK.Text=diseaseDef.DiseaseName;
					textICD9.Text=diseaseDef.ICD9Code;
					break;
				//case EhrCriterion.ICD9:
				//  textCriterionFK.Text=ICD9s.GetDescription(RuleCur.CriterionFK);
				//  break;
				case EhrCriterion.Medication:
					Medication tempMedication = Medications.GetMedication(ReminderRuleCur.CriterionFK);
					if(tempMedication.MedicationNum==tempMedication.GenericNum) {//handle generic medication names.
						textCriterionFK.Text=tempMedication.MedName;
					}
					else {
						textCriterionFK.Text=tempMedication.MedName+" / "+Medications.GetGenericName(tempMedication.GenericNum);
					}
					break;
				case EhrCriterion.Allergy:
					textCriterionFK.Text=AllergyDefs.GetOne(ReminderRuleCur.CriterionFK).Description;
					break;
				case EhrCriterion.Age:
				case EhrCriterion.Gender:
				case EhrCriterion.LabResult:
					//The FK boxes won't even be visible.
					break;
				default://This should not happen.
					break;
			}
		}

		private void comboReminderCriterion_SelectedIndexChanged(object sender,EventArgs e) {
			if(ReminderRuleCur.ReminderCriterion!=(EhrCriterion)comboReminderCriterion.SelectedIndex) {//if user just changed the type,
				ReminderRuleCur.CriterionFK=-1;//clear the FK data showing
				FillFK();
			}
			ReminderRuleCur.ReminderCriterion=(EhrCriterion)comboReminderCriterion.SelectedIndex;
			//if(RuleCur.ReminderCriterion==EhrCriterion.Problem || RuleCur.ReminderCriterion==EhrCriterion.Medication || RuleCur.ReminderCriterion==EhrCriterion.Allergy || RuleCur.ReminderCriterion==EhrCriterion.ICD9) {
			if(ReminderRuleCur.ReminderCriterion==EhrCriterion.Problem || ReminderRuleCur.ReminderCriterion==EhrCriterion.Medication || ReminderRuleCur.ReminderCriterion==EhrCriterion.Allergy) {
				labelCriterionFK.Text=ReminderRuleCur.ReminderCriterion.ToString();
				textCriterionValue.Visible=false;
				labelCriterionValue.Visible=false;
				labelExample.Visible=false;
				labelCriterionFK.Visible=true;
				textCriterionFK.Visible=true;
				butSelectFK.Visible=true;
				if(ReminderRuleCur.ReminderCriterion==EhrCriterion.Problem) {
					labelICD9.Visible=true;
					textICD9.Visible=true;
				}
				else {
					labelICD9.Visible=false;
					textICD9.Visible=false;
				}
			}
			else {//field only used when Age, Gender, or Labresult are selected.
				textCriterionValue.Visible=true;
				labelCriterionValue.Visible=true;
				labelExample.Visible=true;
				labelCriterionFK.Visible=false;
				textCriterionFK.Visible=false;
				butSelectFK.Visible=false;
			}
		}

		private void butSelectFK_Click(object sender,EventArgs e) {
			switch((EhrCriterion)comboReminderCriterion.SelectedIndex) {
				case EhrCriterion.Problem:
					using(FormDiseaseDefs formDiseaseDefs=new FormDiseaseDefs()) {
						formDiseaseDefs.IsSelectionMode=true;
						formDiseaseDefs.ShowDialog();
						if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
							ReminderRuleCur.CriterionFK=-1;
							return;
						}
						//the list should only ever contain one item.
						ReminderRuleCur.CriterionFK=formDiseaseDefs.ListDiseaseDefsSelected[0].DiseaseDefNum;
					}
					break;
				case EhrCriterion.Medication:
					using(FormMedications formMedications=new FormMedications()) {
						formMedications.IsSelectionMode=true;
						formMedications.ShowDialog();
						if(formMedications.DialogResult!=DialogResult.OK) {
							ReminderRuleCur.CriterionFK=-1;
							return;
						}
						ReminderRuleCur.CriterionFK=formMedications.SelectedMedicationNum;
					}
					break;
				case EhrCriterion.Allergy:
					using(FormAllergySetup formAllergySetup=new FormAllergySetup()) {
						formAllergySetup.IsSelectionMode=true;
						formAllergySetup.ShowDialog();
						if(formAllergySetup.DialogResult!=DialogResult.OK) {
							ReminderRuleCur.CriterionFK=-1;
							return;
						}
						ReminderRuleCur.CriterionFK=formAllergySetup.AllergyDefNumSelected;
					}
					break;
				//case EhrCriterion.ICD9:
				//  using(FormIcd9s FormICD9Select = new FormIcd9s()){
				//		FormICD9Select.IsSelectionMode=true;
				//		FormICD9Select.ShowDialog();
				//		if(FormICD9Select.DialogResult!=DialogResult.OK){
				//			RuleCur.CriterionFK=-1;
				//			return;
				//		}
				//		RuleCur.CriterionFK=ICD9s.GetByCode(FormICD9Select.SelectedIcd9Code).ICD9Num;
				//	}
				//  break;
				default:
					MessageBox.Show("You should never see this error message. Something has stopped working properly.");
					break;
			}
			FillFK();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				ReminderRules.Delete(ReminderRuleCur.ReminderRuleNum);
				DialogResult=DialogResult.OK;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			//Validate
			ReminderRuleCur.ReminderCriterion=(EhrCriterion)comboReminderCriterion.SelectedIndex;
			if(ReminderRuleCur.ReminderCriterion==EhrCriterion.Problem 
				|| ReminderRuleCur.ReminderCriterion==EhrCriterion.Medication 
				|| ReminderRuleCur.ReminderCriterion==EhrCriterion.Allergy)
				//|| RuleCur.ReminderCriterion==EhrCriterion.ICD9) 
			{
				ReminderRuleCur.CriterionValue="";
				if(ReminderRuleCur.CriterionFK==-1 || ReminderRuleCur.CriterionFK==0) {
					MessageBox.Show("Please select a valid "+ReminderRuleCur.ReminderCriterion.ToString().ToLower()+".");
					return;
				}
			}
			else if(ReminderRuleCur.ReminderCriterion==EhrCriterion.Gender){
				ReminderRuleCur.CriterionFK=0;
				if(textCriterionValue.Text.ToLower()!="male" && textCriterionValue.Text.ToLower()!="female") {
					MessageBox.Show("Please input male or female for gender value.");
					return;
				}
				ReminderRuleCur.CriterionValue=textCriterionValue.Text.ToLower();
			}
			else if(ReminderRuleCur.ReminderCriterion==EhrCriterion.LabResult) {
				ReminderRuleCur.CriterionFK=0;
				if(textCriterionValue.Text=="") {
					MessageBox.Show("Please input a valid lab result.");
					return;
				}
					ReminderRuleCur.CriterionValue=textCriterionValue.Text;
			}
			else {//Age
				if(textCriterionValue.Text.Length<2){
					MessageBox.Show("Criterion value must be comparator followed by an age. eg. \"<18\".");
					return;
				}
				if(textCriterionValue.Text[0]!='<' && textCriterionValue.Text[0]!='>'){
					MessageBox.Show("Age criterion must begin with either \"<\" or \">\".");
					return;
				}
				int tempAge;
				try {
					tempAge=int.Parse(textCriterionValue.Text.Substring(1,textCriterionValue.Text.Length-1));
				}
				catch {
					MessageBox.Show("Age criterion is not in the form of a valid age.");
					return;
				}
				if(tempAge<0 || tempAge>200) {
					MessageBox.Show("Age must be between 0 and 200.");
					return;
				}
				ReminderRuleCur.CriterionValue=textCriterionValue.Text;
			}
			if(textReminderMessage.Text.Length>255){
				MessageBox.Show("Reminder message must be shorter than 255 characters.");
				return;
			}
			ReminderRuleCur.Message=textReminderMessage.Text;
			if(ReminderRuleCur.Message=="") {
				MessageBox.Show("Reminder will be saved with no reminder message.");
			}
			if(IsNew) {
				ReminderRules.Insert(ReminderRuleCur);
			}
			else {
				ReminderRules.Update(ReminderRuleCur);
			}
			DialogResult=DialogResult.OK;
		}

	}
}