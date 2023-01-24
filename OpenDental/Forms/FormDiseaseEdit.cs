using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormDiseaseEdit:FormODBase {
		private Disease _disease;

		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormDiseaseEdit(Disease disease)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_disease=disease;
		}

		private void FormDiseaseEdit_Load(object sender,EventArgs e) {
			DiseaseDef diseaseDef=DiseaseDefs.GetItem(_disease.DiseaseDefNum);//guaranteed to have one
			textProblem.Text=diseaseDef.DiseaseName;
			string snomedDesc=Snomeds.GetCodeAndDescription(diseaseDef.SnomedCode);
			if(snomedDesc=="") {
				textSnomed.Text=diseaseDef.SnomedCode;
			}
			else {
				textSnomed.Text=snomedDesc;
			}
			string i9descript=ICD9s.GetCodeAndDescription(diseaseDef.ICD9Code);
			if(i9descript=="") {
				textIcd9.Text=diseaseDef.ICD9Code;
			}
			else {
				textIcd9.Text=i9descript;
			}
			string i10descript=Icd10s.GetCodeAndDescription(diseaseDef.Icd10Code);
			if(i10descript=="") {
				textIcd10.Text=diseaseDef.Icd10Code;
			}
			else {
				textIcd10.Text=i10descript;
			}
			comboStatus.Items.Clear();
			string[] stringArrayProblemStatusNames=Enum.GetNames(typeof(ProblemStatus));
			for(int i=0;i<stringArrayProblemStatusNames.Length;i++) {
				string nameProblemStatusTranslated=Lan.g("enumProblemStatus",stringArrayProblemStatusNames[i]);
				comboStatus.Items.Add(nameProblemStatusTranslated);
			}
			comboStatus.SelectedIndex=(int)_disease.ProbStatus;
			textNote.Text=_disease.PatNote;
			if(_disease.DateStart.Year>1880) {
				textDateStart.Text=_disease.DateStart.ToShortDateString();
			}
			if(_disease.DateStop.Year>1880) {
				textDateStop.Text=_disease.DateStop.ToShortDateString();
			}
			comboSnomedProblemType.Items.Clear();
			comboSnomedProblemType.Items.AddEnums<SnomedProblemTypes>();
			if(_disease.SnomedProblemType=="404684003") {//Finding
				comboSnomedProblemType.SelectedIndex=1;
			}
			else if(_disease.SnomedProblemType=="409586006") {//Complaint
				comboSnomedProblemType.SelectedIndex=2;
			}
			else if(_disease.SnomedProblemType=="282291009") {//Diagnosis
				comboSnomedProblemType.SelectedIndex=3;
			}
			else if(_disease.SnomedProblemType=="64572001") {//Condition
				comboSnomedProblemType.SelectedIndex=4;
			}
			else if(_disease.SnomedProblemType=="248536006") {//FunctionalLimitation
				comboSnomedProblemType.SelectedIndex=5;
			}
			else if(_disease.SnomedProblemType=="418799008") {//Symptom
				comboSnomedProblemType.SelectedIndex=6;
			}
			else {//Problem
				comboSnomedProblemType.SelectedIndex=0;
			}
			comboEhrFunctionalStatus.Items.Clear();
			string[] stringArrayFunctionalStatusNames=Enum.GetNames(typeof(FunctionalStatus));
			for(int i=0;i<stringArrayFunctionalStatusNames.Length;i++) {
				string nameFunctionalStatusTranslated=Lan.g("enumFunctionalStatus",stringArrayFunctionalStatusNames[i]);
				comboEhrFunctionalStatus.Items.Add(nameFunctionalStatusTranslated);
			}
			comboEhrFunctionalStatus.SelectedIndex=(int)_disease.FunctionStatus;//The default value is 'Problem'
			if(!Security.IsAuthorized(Permissions.PatProblemListEdit)) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
		}

		private void butTodayStart_Click(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Now.ToShortDateString();
			_disease.DateStart=DateTime.Now;
		}

		private void butTodayStop_Click(object sender,EventArgs e) {
			textDateStop.Text=DateTime.Now.ToShortDateString();
			_disease.DateStop=DateTime.Now;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				//This code is never hit in current implementation 09/26/2013.
				DialogResult=DialogResult.Cancel;
				return;
			}
			try { 
				Diseases.VerifyCanDelete(_disease.DiseaseNum);
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}			
			SecurityLogs.MakeLogEntry(Permissions.PatProblemListEdit,_disease.PatNum,DiseaseDefs.GetName(_disease.DiseaseDefNum)+" deleted");
			Diseases.Delete(_disease);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateStart.IsValid() || !textDateStop.IsValid()) {
				MsgBox.Show(this,"Please fix date.");
				return;
			}
			_disease=Diseases.SetDiseaseFields(_disease,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),(ProblemStatus)comboStatus.SelectedIndex,textNote.Text,
				(SnomedProblemTypes)comboSnomedProblemType.SelectedIndex,(FunctionalStatus)comboEhrFunctionalStatus.SelectedIndex);
			if(IsNew){
				//This code is never hit in current implementation 09/26/2013.
				Diseases.Insert(_disease);
				SecurityLogs.MakeLogEntry(Permissions.PatProblemListEdit,_disease.PatNum,DiseaseDefs.GetName(_disease.DiseaseDefNum)+" added");
			}
			else{
				try { 
					Diseases.VerifyCanUpdate(_disease);
				}
				catch(Exception ex) {
					MsgBox.Show(ex.Message);
					return;
				}
				Diseases.Update(_disease);
				SecurityLogs.MakeLogEntry(Permissions.PatProblemListEdit,_disease.PatNum,DiseaseDefs.GetName(_disease.DiseaseDefNum)+" edited");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















