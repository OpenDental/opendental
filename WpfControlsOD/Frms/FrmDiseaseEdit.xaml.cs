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

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmDiseaseEdit:FrmODBase {
		private Disease _disease;

		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FrmDiseaseEdit(Disease disease)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
			_disease=disease;
		}

		private void FrmDiseaseEdit_Loaded(object sender,RoutedEventArgs e) {
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
				string nameProblemStatusTranslated=Lans.g("enumProblemStatus",stringArrayProblemStatusNames[i]);
				comboStatus.Items.Add(nameProblemStatusTranslated);
			}
			comboStatus.SelectedIndex=(int)_disease.ProbStatus;
			textNote.Text=_disease.PatNote;
			if(_disease.DateStart.Year>1880) {
				textVDateStart.Text=_disease.DateStart.ToShortDateString();
			}
			if(_disease.DateStop.Year>1880) {
				textVDateStop.Text=_disease.DateStop.ToShortDateString();
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
				string nameFunctionalStatusTranslated=Lans.g("enumFunctionalStatus",stringArrayFunctionalStatusNames[i]);
				comboEhrFunctionalStatus.Items.Add(nameFunctionalStatusTranslated);
			}
			comboEhrFunctionalStatus.SelectedIndex=(int)_disease.FunctionStatus;//The default value is 'Problem'
			if(!Security.IsAuthorized(Permissions.PatProblemListEdit)) {
				butSave.IsEnabled=false;
				butDelete.IsEnabled=false;
			}
		}

		private void butTodayStart_Click(object sender,EventArgs e) {
			textVDateStart.Text=DateTime.Now.ToShortDateString();
			_disease.DateStart=DateTime.Now;
		}

		private void butTodayStop_Click(object sender,EventArgs e) {
			textVDateStop.Text=DateTime.Now.ToShortDateString();
			_disease.DateStop=DateTime.Now;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				//This code is never hit in current implementation 09/26/2013.
				IsDialogOK=false;
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
			IsDialogOK=true;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textVDateStart.IsValid() || !textVDateStop.IsValid()) {
				MsgBox.Show(this,"Please fix date.");
				return;
			}
			_disease=Diseases.SetDiseaseFields(_disease,PIn.Date(textVDateStart.Text),PIn.Date(textVDateStop.Text),(ProblemStatus)comboStatus.SelectedIndex,textNote.Text,
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
			IsDialogOK=true;
		}
	}
}





















