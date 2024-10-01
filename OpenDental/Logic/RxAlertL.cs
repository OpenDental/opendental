using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public class RxAlertL {
		///<summary>Returns false if user does not wish to continue after seeing alert.</summary>
		public static bool DisplayAlerts(long patNum,long rxDefNum){
			List<RxAlert> listRxAlerts=null;
			//if(rxDefNum==0){
			//	alertList=RxAlerts.RefreshByRxCui(rxCui);//for CPOE
			//}
			//else{
			listRxAlerts=RxAlerts.Refresh(rxDefNum);//for Rx
			//}
			List<Disease> listDiseases=Diseases.Refresh(patNum);
			List<Allergy> listAllergies=Allergies.Refresh(patNum);
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(patNum,includeDiscontinued:false);//Exclude discontinued, only active meds.
			List<string> listDiseaseMatches=new List<string>();
			List<string> listAllergiesMatches=new List<string>();
			List<string> listMedicationsMatches=new List<string>();
			List<string> listCustomMessages=new List<string>();
			bool showHighSigOnly=PrefC.GetBool(PrefName.EhrRxAlertHighSeverity);
			for(int i=0;i<listRxAlerts.Count;i++){
				for(int j=0;j<listDiseases.Count;j++){
					//This does not look for matches with icd9s.
					if(listRxAlerts[i].DiseaseDefNum==listDiseases[j].DiseaseDefNum && listDiseases[j].ProbStatus==0){//ProbStatus is active.
						if(listRxAlerts[i].NotificationMsg=="") {
							listDiseaseMatches.Add(DiseaseDefs.GetName(listDiseases[j].DiseaseDefNum));
						}
						else {
							listCustomMessages.Add(listRxAlerts[i].NotificationMsg);
						}
					}
				}
				for(int j=0;j<listAllergies.Count;j++) {
					if(listRxAlerts[i].AllergyDefNum==listAllergies[j].AllergyDefNum && listAllergies[j].StatusIsActive) {
						if(listRxAlerts[i].NotificationMsg=="") {
							listAllergiesMatches.Add(AllergyDefs.GetOne(listRxAlerts[i].AllergyDefNum).Description);
						}
						else {
							listCustomMessages.Add(listRxAlerts[i].NotificationMsg);
						}
					}
				}
				for(int j=0;j<listMedicationPats.Count;j++) {
					bool isMedInteraction=false;
					Medication medForAlert=Medications.GetMedication(listRxAlerts[i].MedicationNum);
					if(medForAlert==null) {
						continue;//MedicationNum will be 0 for all other alerts that are not medication alerts.
					}
					if(listMedicationPats[j].MedicationNum!=0 && listRxAlerts[i].MedicationNum==listMedicationPats[j].MedicationNum) {//Medication from medication list.
						isMedInteraction=true;
					}
					else if(listMedicationPats[j].MedicationNum==0 && medForAlert.RxCui!=0 && listMedicationPats[j].RxCui==medForAlert.RxCui) {//Medication from NewCrop. Unfortunately, neither of these RxCuis are required.
						isMedInteraction=true;
					}
					if(!isMedInteraction) {
						continue;//No known interaction.
					}
					//Medication interaction.
					if(showHighSigOnly && !listRxAlerts[i].IsHighSignificance) {//if set to only show high significance alerts and this is not a high significance interaction, do not show alert
						continue;//Low significance alert.
					}
					if(listRxAlerts[i].NotificationMsg=="") {
						Medications.RefreshCache();
						listMedicationsMatches.Add(Medications.GetMedication(listRxAlerts[i].MedicationNum).MedName);
					}
					else {
						listCustomMessages.Add(listRxAlerts[i].NotificationMsg);
					}
				}
			}
			//these matches do not include ones that have custom messages.
			if(listDiseaseMatches.Count==0
				&& listAllergiesMatches.Count==0
				&& listMedicationsMatches.Count==0)
			{
				for(int i=0;i<listCustomMessages.Count;i++){
					if(MessageBox.Show(listCustomMessages[i]+"\r\n"+Lan.g("RxAlertL","Continue anyway?"),"Alert",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation)!=DialogResult.OK){
						return false;
					}
				}
				return true;
			}
			string alertStr="";
			for(int i=0;i<listDiseaseMatches.Count;i++) {
				if(i==0) {
					alertStr+=Lan.g("RxAlertL","This patient has the following medical problems: ");
				}
				alertStr+=listDiseaseMatches[i];
				if((i+1)==listDiseaseMatches.Count) {
					alertStr+=".\r\n";
				}
				else {
					alertStr+=", ";
				}
			}
			for(int i=0;i<listAllergiesMatches.Count;i++) {
				if(i==0 && listDiseaseMatches.Count>0) {
					alertStr+="and the following allergies: ";
				}
				else if(i==0) {
					alertStr=Lan.g("RxAlertL","This patient has the following allergies: ");
				}
				alertStr+=listAllergiesMatches[i];
				if((i+1)==listAllergiesMatches.Count) {
					alertStr+=".\r\n";
				}
				else {
					alertStr+=", ";
				}
			}
			for(int i=0;i<listMedicationsMatches.Count;i++) {
				if(i==0 && (listDiseaseMatches.Count>0 || listAllergiesMatches.Count>0)) {
					alertStr+="and is taking the following medications: ";
				}
				else if(i==0) {
					alertStr=Lan.g("RxAlertL","This patient is taking the following medications: ");
				}
				alertStr+=listMedicationsMatches[i];
				if((i+1)==listMedicationsMatches.Count) {
					alertStr+=".\r\n";
				}
				else {
					alertStr+=", ";
				}
			}
			alertStr+="\r\n"+Lan.g("RxAlertL","Continue anyway?");
			if(MessageBox.Show(alertStr,"Alert",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation)!=DialogResult.OK) {
				return false;
			}
			for(int i=0;i<listCustomMessages.Count;i++){
				if(MessageBox.Show(listCustomMessages[i]+"\r\n"+Lan.g("RxAlertL","Continue anyway?"),"Alert",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation)!=DialogResult.OK){
					return false;
				}
			}
			return true;
		}






	}
}
