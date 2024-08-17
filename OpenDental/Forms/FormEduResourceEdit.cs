﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEduResourceEdit:FormODBase {
		public bool IsNew;
		public EduResource EduResourceCur;

		public FormEduResourceEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormEduResourceEdit_Load(object sender,EventArgs e) {
			if(EduResourceCur.DiseaseDefNum!=0) {
				DiseaseDef diseaseDef=DiseaseDefs.GetItem(EduResourceCur.DiseaseDefNum);
				textProblem.Text=diseaseDef.DiseaseName;
				textICD9.Text=ICD9s.GetCodeAndDescription(diseaseDef.ICD9Code);
				textSnomed.Text=Snomeds.GetCodeAndDescription(diseaseDef.SnomedCode);
			}
			else if(EduResourceCur.MedicationNum!=0) {
				textMedication.Text=Medications.GetDescription(EduResourceCur.MedicationNum);
			}
			else if(EduResourceCur.SmokingSnoMed!="") {
				textTobaccoAssessment.Text=Snomeds.GetCodeAndDescription(EduResourceCur.SmokingSnoMed);
			}
			textLabResultsID.Text=EduResourceCur.LabResultID;
			textLabTestName.Text=EduResourceCur.LabResultName;
			textCompareValue.Text=EduResourceCur.LabResultCompare;
			textUrl.Text=EduResourceCur.ResourceUrl;
		}

		private void butProblemSelect_Click(object sender,EventArgs e) {
			using FormDiseaseDefs formDiseaseDefs = new FormDiseaseDefs();
			formDiseaseDefs.IsSelectionMode=true;
			formDiseaseDefs.ShowDialog();
			if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			DiseaseDef diseaseDef=formDiseaseDefs.ListDiseaseDefsSelected[0];
			if(diseaseDef==null) {
				return;
			}
			EduResourceCur.DiseaseDefNum=diseaseDef.DiseaseDefNum;
			EduResourceCur.MedicationNum=0;
			EduResourceCur.SmokingSnoMed="";
			EduResourceCur.LabResultID="";
			EduResourceCur.LabResultName="";
			EduResourceCur.LabResultCompare="";
			textProblem.Text=diseaseDef.DiseaseName;
			textICD9.Text=ICD9s.GetCodeAndDescription(diseaseDef.ICD9Code);
			textSnomed.Text=Snomeds.GetCodeAndDescription(diseaseDef.SnomedCode);
			textMedication.Text="";
			textTobaccoAssessment.Text="";
			textLabResultsID.Text="";
			textLabTestName.Text="";
			textCompareValue.Text="";
		}

		private void butMedicationSelect_Click(object sender,EventArgs e) {
			using FormMedications formMedications=new FormMedications();
			formMedications.IsSelectionMode=true;
			formMedications.ShowDialog();
			if(formMedications.DialogResult!=DialogResult.OK) {
				return;
			}
			EduResourceCur.DiseaseDefNum=0;
			EduResourceCur.MedicationNum=formMedications.SelectedMedicationNum;
			EduResourceCur.SmokingSnoMed="";
			EduResourceCur.LabResultID="";
			EduResourceCur.LabResultName="";
			EduResourceCur.LabResultCompare="";
			textProblem.Text="";
			textICD9.Text="";
			textSnomed.Text="";
			textMedication.Text=Medications.GetDescription(formMedications.SelectedMedicationNum);
			textTobaccoAssessment.Text="";
			textLabResultsID.Text="";
			textLabTestName.Text="";
			textCompareValue.Text="";
		}

		private void butTobaccoCodeSelect_Click(object sender,EventArgs e) {
			using FormSnomeds formSnomeds=new FormSnomeds();
			formSnomeds.IsSelectionMode=true;
			formSnomeds.ShowDialog();
			if(formSnomeds.DialogResult!=DialogResult.OK) {
				return;
			}
			EduResourceCur.DiseaseDefNum=0;
			EduResourceCur.MedicationNum=0;
			EduResourceCur.SmokingSnoMed=formSnomeds.SnomedSelected.SnomedCode;
			EduResourceCur.LabResultID="";
			EduResourceCur.LabResultName="";
			EduResourceCur.LabResultCompare="";
			textProblem.Text="";
			textICD9.Text="";
			textSnomed.Text="";
			textMedication.Text="";
			textTobaccoAssessment.Text=formSnomeds.SnomedSelected.SnomedCode+" - "+formSnomeds.SnomedSelected.Description;
			textLabResultsID.Text="";
			textLabTestName.Text="";
			textCompareValue.Text="";
		}

		private void textLabResults_Click(object sender,EventArgs e) {
			//attached to click for 3 different text boxes.
			EduResourceCur.DiseaseDefNum=0;
			EduResourceCur.MedicationNum=0;
			EduResourceCur.SmokingSnoMed="";
			textProblem.Text="";
			textICD9.Text="";
			textSnomed.Text="";
			textMedication.Text="";
			textTobaccoAssessment.Text="";
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this educational resource?")) {
				return;
			}
			EduResources.Delete(EduResourceCur.EduResourceNum);
			DialogResult=DialogResult.OK;
		}

		private void butOk_Click(object sender,EventArgs e) {
			//validation
			if(EduResourceCur.DiseaseDefNum==0 && EduResourceCur.MedicationNum==0 && EduResourceCur.SmokingSnoMed==""
				&& textLabResultsID.Text=="" && textLabTestName.Text=="" && textCompareValue.Text=="")
			{
				MessageBox.Show("Please Select a valid problem, medication, or lab result.");
				return;
			}
			if(EduResourceCur.DiseaseDefNum==0 && EduResourceCur.MedicationNum==0 && EduResourceCur.SmokingSnoMed=="") {
				if(textLabTestName.Text=="") {
					MessageBox.Show("Invalid test name for lab result.");
					return;
				}
				if(textCompareValue.Text.Length<2) {
					MessageBox.Show("Compare value must be comparator followed by a number. eg. \">120\".");
					return;
				}
				if(textCompareValue.Text[0]!='<' && textCompareValue.Text[0]!='>') {
					MessageBox.Show("Compare value must begin with either \"<\" or \">\".");
					return;
				}
				try {
					int.Parse(textCompareValue.Text.Substring(1));
				}
				catch {
					MessageBox.Show("Compare value is not a valid number.");
					return;
				}
			}
			if(textUrl.Text=="") {
				MessageBox.Show("Please input a valid recource URL.");
				return;
			}
			//done validating
			EduResourceCur.LabResultID=textLabResultsID.Text;
			EduResourceCur.LabResultName=textLabTestName.Text;
			EduResourceCur.LabResultCompare=textCompareValue.Text;
			EduResourceCur.ResourceUrl=textUrl.Text;
			if(IsNew) {
				EduResources.Insert(EduResourceCur);
			}
			else {
				EduResources.Update(EduResourceCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
