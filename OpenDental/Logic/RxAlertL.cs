using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public class RxAlertL {
		///<summary>Returns false if user does not wish to continue after seeing alert.</summary>
		public static bool DisplayAlerts(long patNum,long rxDefNum){
			List<RxAlert> alertList=null;
			//if(rxDefNum==0){
			//	alertList=RxAlerts.RefreshByRxCui(rxCui);//for CPOE
			//}
			//else{
			alertList=RxAlerts.Refresh(rxDefNum);//for Rx
			//}
			List<Disease> diseases=Diseases.Refresh(patNum);
			List<Allergy> allergies=Allergies.Refresh(patNum);
			List<MedicationPat> medicationPats=MedicationPats.Refresh(patNum,false);//Exclude discontinued, only active meds.
			List<string> diseaseMatches=new List<string>();
			List<string> allergiesMatches=new List<string>();
			List<string> medicationsMatches=new List<string>();
			List<string> customMessages=new List<string>();
			bool showHighSigOnly=PrefC.GetBool(PrefName.EhrRxAlertHighSeverity);
			for(int i=0;i<alertList.Count;i++){
				for(int j=0;j<diseases.Count;j++){
					//This does not look for matches with icd9s.
					if(alertList[i].DiseaseDefNum==diseases[j].DiseaseDefNum && diseases[j].ProbStatus==0){//ProbStatus is active.
						if(alertList[i].NotificationMsg=="") {
							diseaseMatches.Add(DiseaseDefs.GetName(diseases[j].DiseaseDefNum));
						}
						else {
							customMessages.Add(alertList[i].NotificationMsg);
						}
					}
				}
				for(int j=0;j<allergies.Count;j++) {
					if(alertList[i].AllergyDefNum==allergies[j].AllergyDefNum && allergies[j].StatusIsActive) {
						if(alertList[i].NotificationMsg=="") {
							allergiesMatches.Add(AllergyDefs.GetOne(alertList[i].AllergyDefNum).Description);
						}
						else {
							customMessages.Add(alertList[i].NotificationMsg);
						}
					}
				}
				for(int j=0;j<medicationPats.Count;j++) {
					bool isMedInteraction=false;
					Medication medForAlert=Medications.GetMedication(alertList[i].MedicationNum);
					if(medForAlert==null) {
						continue;//MedicationNum will be 0 for all other alerts that are not medication alerts.
					}
					if(medicationPats[j].MedicationNum!=0 && alertList[i].MedicationNum==medicationPats[j].MedicationNum) {//Medication from medication list.
						isMedInteraction=true;
					}
					else if(medicationPats[j].MedicationNum==0 && medForAlert.RxCui!=0 && medicationPats[j].RxCui==medForAlert.RxCui) {//Medication from NewCrop. Unfortunately, neither of these RxCuis are required.
						isMedInteraction=true;
					}
					if(!isMedInteraction) {
						continue;//No known interaction.
					}
					//Medication interaction.
					if(showHighSigOnly && !alertList[i].IsHighSignificance) {//if set to only show high significance alerts and this is not a high significance interaction, do not show alert
						continue;//Low significance alert.
					}
					if(alertList[i].NotificationMsg=="") {
						Medications.RefreshCache();
						medicationsMatches.Add(Medications.GetMedication(alertList[i].MedicationNum).MedName);
					}
					else {
						customMessages.Add(alertList[i].NotificationMsg);
					}
				}
			}
			//these matches do not include ones that have custom messages.
			if(diseaseMatches.Count>0
				|| allergiesMatches.Count>0
				|| medicationsMatches.Count>0)
			{
				string alert="";
				for(int i=0;i<diseaseMatches.Count;i++) {
					if(i==0) {
						alert+=Lan.g("RxAlertL","This patient has the following medical problems: ");
					}
					alert+=diseaseMatches[i];
					if((i+1)==diseaseMatches.Count) {
						alert+=".\r\n";
					}
					else {
						alert+=", ";
					}
				}
				for(int i=0;i<allergiesMatches.Count;i++) {
					if(i==0 && diseaseMatches.Count>0) {
						alert+="and the following allergies: ";
					}
					else if(i==0) {
						alert=Lan.g("RxAlertL","This patient has the following allergies: ");
					}
					alert+=allergiesMatches[i];
					if((i+1)==allergiesMatches.Count) {
						alert+=".\r\n";
					}
					else {
						alert+=", ";
					}
				}
				for(int i=0;i<medicationsMatches.Count;i++) {
					if(i==0 && (diseaseMatches.Count>0 || allergiesMatches.Count>0)) {
						alert+="and is taking the following medications: ";
					}
					else if(i==0) {
						alert=Lan.g("RxAlertL","This patient is taking the following medications: ");
					}
					alert+=medicationsMatches[i];
					if((i+1)==medicationsMatches.Count) {
						alert+=".\r\n";
					}
					else {
						alert+=", ";
					}
				}
				alert+="\r\n"+Lan.g("RxAlertL","Continue anyway?");
				if(MessageBox.Show(alert,"Alert",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation)!=DialogResult.OK) {
					return false;
				}
			}
			for(int i=0;i<customMessages.Count;i++){
				if(MessageBox.Show(customMessages[i]+"\r\n"+Lan.g("RxAlertL","Continue anyway?"),"Alert",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation)!=DialogResult.OK){
					return false;
				}
			}
			return true;
		}






	}
}
