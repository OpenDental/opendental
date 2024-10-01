using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EduResources{
		///<summary></summary>
		public static List<EduResource> GenerateForPatient(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EduResource>>(MethodBase.GetCurrentMethod(),patNum);
			}
			List<Disease> listDiseases=Diseases.Refresh(patNum);
			List<MedicationPat> listMedicationPats=MedicationPats.Refresh(patNum,false);
			List<LabResult> listLabResults=LabResults.GetAllForPatient(patNum);
			List<EhrLabResult> listEhrLabResults=EhrLabResults.GetAllForPatient(patNum);
			List<EduResource> listEduResourcesAll=Crud.EduResourceCrud.SelectMany("SELECT * FROM eduresource");
			List<EhrMeasureEvent> listEhrMeasureEventsTobacco=EhrMeasureEvents.RefreshByType(patNum,EhrMeasureEventType.TobaccoUseAssessed)
				.FindAll(x => x.CodeSystemResult=="SNOMEDCT");
			List<EduResource> listEduResourcesRet = new List<EduResource>();
			for(int i=0;i<listEduResourcesAll.Count;i++) {
				if(listEduResourcesAll[i].DiseaseDefNum!=0 && listDiseases.Exists(x => x.DiseaseDefNum==listEduResourcesAll[i].DiseaseDefNum)){
					listEduResourcesRet.Add(listEduResourcesAll[i]);
					continue;
				}
				if(listEduResourcesAll[i].MedicationNum!=0 && listMedicationPats.Exists(x => x.MedicationNum==listEduResourcesAll[i].MedicationNum 
					|| (x.MedicationNum==0 && Medications.GetMedication(listEduResourcesAll[i].MedicationNum).RxCui==x.RxCui))) 
				{
					listEduResourcesRet.Add(listEduResourcesAll[i]);
					continue;
				}
				if(listEduResourcesAll[i].SmokingSnoMed!="" && listEhrMeasureEventsTobacco.Exists(x => x.CodeValueResult==listEduResourcesAll[i].SmokingSnoMed)) {
					listEduResourcesRet.Add(listEduResourcesAll[i]);
					continue;
				}
			}
			for(int i=0;i<listEduResourcesAll.Count;i++) {
				if(listEduResourcesAll[i].LabResultID==""){ 
					continue;
				}
				if(listEduResourcesRet.Contains(listEduResourcesAll[i])) {
					continue;//already added from loop above.
				}
				for(int j=0;j<listLabResults.Count;j++) {
					if(listLabResults[j].TestID!=listEduResourcesAll[i].LabResultID){
						continue;
					}
					if(listEduResourcesAll[i].LabResultCompare.StartsWith("<")){
						//PIn.Int not used because blank not allowed.
						try{
							if(int.Parse(listLabResults[j].ObsValue) < int.Parse(listEduResourcesAll[i].LabResultCompare.Substring(1))){
								listEduResourcesRet.Add(listEduResourcesAll[i]);
							}
						}
						catch{
							//This could only happen if the validation in either input didn't work.
						}
					}
					else if(listEduResourcesAll[i].LabResultCompare.StartsWith(">")){
						try {
							if(int.Parse(listLabResults[j].ObsValue) > int.Parse(listEduResourcesAll[i].LabResultCompare.Substring(1))) {
								listEduResourcesRet.Add(listEduResourcesAll[i]);
							}
						}
						catch {
							//This could only happen if the validation in either input didn't work.
						}
					}
				}//end listLabResults
				for(int j=0;j<listEhrLabResults.Count;j++) { //matches loinc only.
					if(listEhrLabResults[j].ObservationIdentifierID!=listEduResourcesAll[i].LabResultID){ 
						continue;
					}
					listEduResourcesRet.Add(listEduResourcesAll[i]);
				}//end listEhrLabResults
			}//end listEduResourcesAll
			return listEduResourcesRet;
		}

		///<summary></summary>
		public static List<EduResource> SelectAll(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EduResource>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM eduresource";
			return Crud.EduResourceCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long eduResourceNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eduResourceNum);
				return;
			}
			string command= "DELETE FROM eduresource WHERE EduResourceNum = "+POut.Long(eduResourceNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(EduResource eduResource) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				eduResource.EduResourceNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eduResource);
				return eduResource.EduResourceNum;
			}
			return Crud.EduResourceCrud.Insert(eduResource);
		}

		///<summary></summary>
		public static void Update(EduResource eduResource) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eduResource);
				return;
			}
			Crud.EduResourceCrud.Update(eduResource);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EduResource> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EduResource>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM eduresource WHERE PatNum = "+POut.Long(patNum);
			return Crud.EduResourceCrud.SelectMany(command);
		}

		///<summary>Gets one EduResource from the db.</summary>
		public static EduResource GetOne(long eduResourceNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EduResource>(MethodBase.GetCurrentMethod(),eduResourceNum);
			}
			return Crud.EduResourceCrud.SelectOne(eduResourceNum);
		}



		*/
	}
}