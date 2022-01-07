using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ReminderRules{
		///<summary></summary>
		public static long Insert(ReminderRule reminderRule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				reminderRule.ReminderRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),reminderRule);
				return reminderRule.ReminderRuleNum;
			}
			return Crud.ReminderRuleCrud.Insert(reminderRule);
		}

		///<summary></summary>
		public static void Update(ReminderRule reminderRule) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reminderRule);
				return;
			}
			Crud.ReminderRuleCrud.Update(reminderRule);
		}

		///<summary></summary>
		public static void Delete(long reminderRuleNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reminderRuleNum);
				return;
			}
			string command= "DELETE FROM reminderrule WHERE ReminderRuleNum = "+POut.Long(reminderRuleNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static List<ReminderRule> SelectAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ReminderRule>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM reminderrule";
			return Crud.ReminderRuleCrud.SelectMany(command);
		}

		public static List<ReminderRule> GetRemindersForPatient(Patient PatCur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ReminderRule>>(MethodBase.GetCurrentMethod(),PatCur);
			}
			//Problem,Medication,Allergy,Age,Gender,LabResult
			List<ReminderRule> fullListReminders = Crud.ReminderRuleCrud.SelectMany("SELECT * FROM reminderrule");
			List<ReminderRule> retVal = new List<ReminderRule>();
			List<Disease> listProblems = Diseases.Refresh(PatCur.PatNum);
			List<Medication> listMedications = Medications.GetMedicationsByPat(PatCur.PatNum);
			List<Allergy> listAllergies = Allergies.Refresh(PatCur.PatNum);
			List<LabResult> listLabResults = LabResults.GetAllForPatient(PatCur.PatNum);
			for(int i=0;i<fullListReminders.Count;i++) {
				switch(fullListReminders[i].ReminderCriterion) {
					case EhrCriterion.Problem:
						for(int j=0;j<listProblems.Count;j++) {
							if(fullListReminders[i].CriterionFK==listProblems[j].DiseaseDefNum) {
								retVal.Add(fullListReminders[i]);
								break;
							}
						}
						break;
					case EhrCriterion.Medication:
						for(int j=0;j<listMedications.Count;j++) {
							if(fullListReminders[i].CriterionFK==listMedications[j].MedicationNum) {
								retVal.Add(fullListReminders[i]);
								break;
							}
						}
						break;
					case EhrCriterion.Allergy:
						for(int j=0;j<listAllergies.Count;j++) {
							if(fullListReminders[i].CriterionFK==listAllergies[j].AllergyDefNum) {
								retVal.Add(fullListReminders[i]);
								break;
							}
						}
						break;
					case EhrCriterion.Age:
						if(fullListReminders[i].CriterionValue[0]=='<') {
							if(PatCur.Age<int.Parse(fullListReminders[i].CriterionValue.Substring(1,fullListReminders[i].CriterionValue.Length-1))){
								retVal.Add(fullListReminders[i]);
							}
						}
						else if(fullListReminders[i].CriterionValue[0]=='>'){
							if(PatCur.Age>int.Parse(fullListReminders[i].CriterionValue.Substring(1,fullListReminders[i].CriterionValue.Length-1))){
								retVal.Add(fullListReminders[i]);
							}
						}
						else{
							//This section should never be reached
						}
						break;
					case EhrCriterion.Gender:
						if(PatCur.Gender.ToString().ToLower()==fullListReminders[i].CriterionValue.ToLower()){
							retVal.Add(fullListReminders[i]);
						}
						break;
					case EhrCriterion.LabResult:
						for(int j=0;j<listLabResults.Count;j++) {
							if(listLabResults[j].TestName.ToLower().Contains(fullListReminders[i].CriterionValue.ToLower())) {				
								retVal.Add(fullListReminders[i]);
								break;
							}
						}
						break;
					//case EhrCriterion.ICD9:
					//  for(int j=0;j<listProblems.Count;j++) {
					//    if(fullListReminders[i].CriterionFK==listProblems[j].DiseaseDefNum) {
					//      retVal.Add(fullListReminders[i]);
					//      break;
					//    }
					//  }
					//  break;
				}
			}
			return retVal;
		}


		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ReminderRule> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ReminderRule>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM reminderrule WHERE PatNum = "+POut.Long(patNum);
			return Crud.ReminderRuleCrud.SelectMany(command);
		}

		///<summary>Gets one ReminderRule from the db.</summary>
		public static ReminderRule GetOne(long reminderRuleNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ReminderRule>(MethodBase.GetCurrentMethod(),reminderRuleNum);
			}
			return Crud.ReminderRuleCrud.SelectOne(reminderRuleNum);
		}


		*/
	}
}