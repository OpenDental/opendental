using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

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
			comboSnomedProblemType.Items.Add("Problem");//0 - Default value.  Problem (finding).  55607006
			comboSnomedProblemType.Items.Add("Finding");//1 - Clinical finding (finding).  404684003
			comboSnomedProblemType.Items.Add("Complaint");//2 - Complaint (finding).  409586006
			comboSnomedProblemType.Items.Add("Diagnosis");//3 - Diagnosis interpretation (observable entity).  282291009
			comboSnomedProblemType.Items.Add("Condition");//4 - Disease (disorder).  64572001
			comboSnomedProblemType.Items.Add("FunctionalLimitation");//5 - Finding of functional performance and activity (finding).  248536006
			comboSnomedProblemType.Items.Add("Symptom");//6 - Finding reported by subject or history provider (finding).  418799008
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
			List<Vitalsign> listVitalsigns=Vitalsigns.GetListFromPregDiseaseNum(_disease.DiseaseNum);
			if(listVitalsigns.Count>0) {//if attached to vital sign exam, block delete
				string strDates="";
				for(int i=0;i<listVitalsigns.Count;i++) {
					if(i>5) {
						break;
					}
					strDates+="\r\n"+listVitalsigns[i].DateTaken.ToShortDateString();
				}
				MsgBox.Show(this,"Not allowed to delete this problem.  It is attached to "+listVitalsigns.Count.ToString()+" vital sign exams with dates including: "+strDates+".");
				return;
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
					return;
				}
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
			_disease.DateStart=PIn.Date(textDateStart.Text);
			_disease.DateStop=PIn.Date(textDateStop.Text);
			_disease.ProbStatus=(ProblemStatus)comboStatus.SelectedIndex;
			_disease.PatNote=textNote.Text;
			if(comboSnomedProblemType.SelectedIndex==1) {//Finding
				_disease.SnomedProblemType="404684003";
			}
			else if(comboSnomedProblemType.SelectedIndex==2) {//Complaint
				_disease.SnomedProblemType="409586006";
			}
			else if(comboSnomedProblemType.SelectedIndex==3) {//Dignosis
				_disease.SnomedProblemType="282291009";
			}
			else if(comboSnomedProblemType.SelectedIndex==4) {//Condition
				_disease.SnomedProblemType="64572001";
			}
			else if(comboSnomedProblemType.SelectedIndex==5) {//FunctionalLimitation
				_disease.SnomedProblemType="248536006";
			}
			else if(comboSnomedProblemType.SelectedIndex==6) {//Symptom
				_disease.SnomedProblemType="418799008";
			}
			else {//Problem
				_disease.SnomedProblemType="55607006";
			}
			_disease.FunctionStatus=(FunctionalStatus)comboEhrFunctionalStatus.SelectedIndex;
			if(IsNew){
				//This code is never hit in current implementation 09/26/2013.
				Diseases.Insert(_disease);
				SecurityLogs.MakeLogEntry(Permissions.PatProblemListEdit,_disease.PatNum,DiseaseDefs.GetName(_disease.DiseaseDefNum)+" added");
			}
			else{
				//See if this problem is the pregnancy linked to a vitalsign exam
				List<Vitalsign> listVitalsigns=Vitalsigns.GetListFromPregDiseaseNum(_disease.DiseaseNum);
				if(listVitalsigns.Count>0) {
					//See if the vitalsign exam date is now outside of the active dates of the disease (pregnancy)
					string strDates="";
					for(int i=0;i<listVitalsigns.Count;i++) {
						if(listVitalsigns[i].DateTaken<_disease.DateStart 
							|| (_disease.DateStop.Year>1880 && listVitalsigns[i].DateTaken>_disease.DateStop)) {
							strDates+="\r\n"+listVitalsigns[i].DateTaken.ToShortDateString();
						}
					}
					//If vitalsign exam is now outside the dates of the problem, tell the user they must fix the dates of the pregnancy dx
					if(strDates.Length>0) {
						MsgBox.Show(this,"This problem is attached to 1 or more vital sign exams as a pregnancy diagnosis with dates:"+strDates+"\r\nNot allowed to change the active dates of the diagnosis to be outside the dates of the exam(s).  You must first remove the diagnosis from the vital sign exam(s).");
						return;
					}
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





















