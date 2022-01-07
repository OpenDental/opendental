using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormDiseaseEdit : FormODBase {
		private Disease DiseaseCur;

		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormDiseaseEdit(Disease diseaseCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			DiseaseCur=diseaseCur;
		}

		private void FormDiseaseEdit_Load(object sender,EventArgs e) {
			DiseaseDef diseasedef=DiseaseDefs.GetItem(DiseaseCur.DiseaseDefNum);//guaranteed to have one
			textProblem.Text=diseasedef.DiseaseName;
			string i9descript=ICD9s.GetCodeAndDescription(diseasedef.ICD9Code);
			if(i9descript=="") {
				textIcd9.Text=diseasedef.ICD9Code;
			}
			else {
				textIcd9.Text=i9descript;
			}
			string i10descript=Icd10s.GetCodeAndDescription(diseasedef.Icd10Code);
			if(i10descript=="") {
				textIcd10.Text=diseasedef.Icd10Code;
			}
			else {
				textIcd10.Text=i10descript;
			}
			comboStatus.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(ProblemStatus)).Length;i++) {
				comboStatus.Items.Add(Enum.GetNames(typeof(ProblemStatus))[i]);
			}
			comboStatus.SelectedIndex=(int)DiseaseCur.ProbStatus;
			textNote.Text=DiseaseCur.PatNote;
			if(DiseaseCur.DateStart.Year>1880) {
				textDateStart.Text=DiseaseCur.DateStart.ToShortDateString();
			}
			if(DiseaseCur.DateStop.Year>1880) {
				textDateStop.Text=DiseaseCur.DateStop.ToShortDateString();
			}
			comboSnomedProblemType.Items.Clear();
			comboSnomedProblemType.Items.Add("Problem");//0 - Default value.  Problem (finding).  55607006
			comboSnomedProblemType.Items.Add("Finding");//1 - Clinical finding (finding).  404684003
			comboSnomedProblemType.Items.Add("Complaint");//2 - Complaint (finding).  409586006
			comboSnomedProblemType.Items.Add("Diagnosis");//3 - Diagnosis interpretation (observable entity).  282291009
			comboSnomedProblemType.Items.Add("Condition");//4 - Disease (disorder).  64572001
			comboSnomedProblemType.Items.Add("FunctionalLimitation");//5 - Finding of functional performance and activity (finding).  248536006
			comboSnomedProblemType.Items.Add("Symptom");//6 - Finding reported by subject or history provider (finding).  418799008
			if(DiseaseCur.SnomedProblemType=="404684003") {//Finding
				comboSnomedProblemType.SelectedIndex=1;
			}
			else if(DiseaseCur.SnomedProblemType=="409586006") {//Complaint
				comboSnomedProblemType.SelectedIndex=2;
			}
			else if(DiseaseCur.SnomedProblemType=="282291009") {//Diagnosis
				comboSnomedProblemType.SelectedIndex=3;
			}
			else if(DiseaseCur.SnomedProblemType=="64572001") {//Condition
				comboSnomedProblemType.SelectedIndex=4;
			}
			else if(DiseaseCur.SnomedProblemType=="248536006") {//FunctionalLimitation
				comboSnomedProblemType.SelectedIndex=5;
			}
			else if(DiseaseCur.SnomedProblemType=="418799008") {//Symptom
				comboSnomedProblemType.SelectedIndex=6;
			}
			else {//Problem
				comboSnomedProblemType.SelectedIndex=0;
			}
			comboEhrFunctionalStatus.Items.Clear();
			string[] arrayFunctionalStatusNames=Enum.GetNames(typeof(FunctionalStatus));
			for(int i=0;i<arrayFunctionalStatusNames.Length;i++) {
				comboEhrFunctionalStatus.Items.Add(Lan.g(this,arrayFunctionalStatusNames[i]));
			}
			comboEhrFunctionalStatus.SelectedIndex=(int)DiseaseCur.FunctionStatus;//The default value is 'Problem'
		}

		private void butTodayStart_Click(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Now.ToShortDateString();
			DiseaseCur.DateStart=DateTime.Now;
		}

		private void butTodayStop_Click(object sender,EventArgs e) {
			textDateStop.Text=DateTime.Now.ToShortDateString();
			DiseaseCur.DateStop=DateTime.Now;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				//This code is never hit in current implementation 09/26/2013.
				DialogResult=DialogResult.Cancel;
				return;
			}
			List<Vitalsign> listVitals=Vitalsigns.GetListFromPregDiseaseNum(DiseaseCur.DiseaseNum);
			if(listVitals.Count>0) {//if attached to vital sign exam, block delete
				string dates="";
				for(int i=0;i<listVitals.Count;i++) {
					if(i>5) {
						break;
					}
					dates+="\r\n"+listVitals[i].DateTaken.ToShortDateString();
				}
				MsgBox.Show(this,"Not allowed to delete this problem.  It is attached to "+listVitals.Count.ToString()+"vital sign exams with dates including:"+dates+".");
				return;
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
					return;
				}
			}
			SecurityLogs.MakeLogEntry(Permissions.PatProblemListEdit,DiseaseCur.PatNum,DiseaseDefs.GetName(DiseaseCur.DiseaseDefNum)+" deleted");
			Diseases.Delete(DiseaseCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateStart.IsValid() || !textDateStop.IsValid()) {
				MsgBox.Show(this,"Please fix date.");
				return;
			}
			DiseaseCur.DateStart=PIn.Date(textDateStart.Text);
			DiseaseCur.DateStop=PIn.Date(textDateStop.Text);
			DiseaseCur.ProbStatus=(ProblemStatus)comboStatus.SelectedIndex;
			DiseaseCur.PatNote=textNote.Text;
			if(comboSnomedProblemType.SelectedIndex==1) {//Finding
				DiseaseCur.SnomedProblemType="404684003";
			}
			else if(comboSnomedProblemType.SelectedIndex==2) {//Complaint
				DiseaseCur.SnomedProblemType="409586006";
			}
			else if(comboSnomedProblemType.SelectedIndex==3) {//Dignosis
				DiseaseCur.SnomedProblemType="282291009";
			}
			else if(comboSnomedProblemType.SelectedIndex==4) {//Condition
				DiseaseCur.SnomedProblemType="64572001";
			}
			else if(comboSnomedProblemType.SelectedIndex==5) {//FunctionalLimitation
				DiseaseCur.SnomedProblemType="248536006";
			}
			else if(comboSnomedProblemType.SelectedIndex==6) {//Symptom
				DiseaseCur.SnomedProblemType="418799008";
			}
			else {//Problem
				DiseaseCur.SnomedProblemType="55607006";
			}
			DiseaseCur.FunctionStatus=(FunctionalStatus)comboEhrFunctionalStatus.SelectedIndex;
			if(IsNew){
				//This code is never hit in current implementation 09/26/2013.
				Diseases.Insert(DiseaseCur);
				SecurityLogs.MakeLogEntry(Permissions.PatProblemListEdit,DiseaseCur.PatNum,DiseaseDefs.GetName(DiseaseCur.DiseaseDefNum)+" added");
			}
			else{
				//See if this problem is the pregnancy linked to a vitalsign exam
				List<Vitalsign> listVitalsAttached=Vitalsigns.GetListFromPregDiseaseNum(DiseaseCur.DiseaseNum);
				if(listVitalsAttached.Count>0) {
					//See if the vitalsign exam date is now outside of the active dates of the disease (pregnancy)
					string dates="";
					for(int i=0;i<listVitalsAttached.Count;i++) {
						if(listVitalsAttached[i].DateTaken<DiseaseCur.DateStart || (DiseaseCur.DateStop.Year>1880 && listVitalsAttached[i].DateTaken>DiseaseCur.DateStop)) {
							dates+="\r\n"+listVitalsAttached[i].DateTaken.ToShortDateString();
						}
					}
					//If vitalsign exam is now outside the dates of the problem, tell the user they must fix the dates of the pregnancy dx
					if(dates.Length>0) {
						MsgBox.Show(this,"This problem is attached to 1 or more vital sign exams as a pregnancy diagnosis with dates:"+dates+"\r\nNot allowed to change the active dates of the diagnosis to be outside the dates of the exam(s).  You must first remove the diagnosis from the vital sign exam(s).");
						return;
					}
				}
				Diseases.Update(DiseaseCur);
				SecurityLogs.MakeLogEntry(Permissions.PatProblemListEdit,DiseaseCur.PatNum,DiseaseDefs.GetName(DiseaseCur.DiseaseDefNum)+" edited");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		


	}
}





















