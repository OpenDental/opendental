using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Diseases {
		///<summary>This returns a single disease, but a patient may have multiple instances of the same disease.  For example, they may have multiple pregnancy instances with the same DiseaseDefNum.  This will return a single instance of the disease, chosen at random by MySQL.  Would be better to use GetDiseasesForPatient below which returns a list of diseases with this DiseaseDefNum for the patient.</summary>
		public static Disease GetSpecificDiseaseForPatient(long patNum,long diseaseDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Disease>(MethodBase.GetCurrentMethod(),patNum,diseaseDefNum);
			}
			string command="SELECT * FROM disease WHERE PatNum="+POut.Long(patNum)
				+" AND DiseaseDefNum="+POut.Long(diseaseDefNum);
			return Crud.DiseaseCrud.SelectOne(command);
		}
		
		///<summary>Gets a list of every disease for the patient that has the specified DiseaseDefNum.  Set showActiveOnly true to only show active Diseases based on status (i.e. it could have a stop date but still be active, or marked inactive with no stop date).</summary>
		public static List<Disease> GetDiseasesForPatient(long patNum,long diseaseDefNum,bool showActiveOnly) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Disease>>(MethodBase.GetCurrentMethod(),patNum,diseaseDefNum,showActiveOnly);
			}
			string command="SELECT * FROM disease WHERE PatNum="+POut.Long(patNum)
				+" AND DiseaseDefNum="+POut.Long(diseaseDefNum);
			if(showActiveOnly) {
				command+=" AND ProbStatus="+POut.Int((int)ProblemStatus.Active);
			}
			return Crud.DiseaseCrud.SelectMany(command);
		}		
		
		///<summary>Returns a list of PatNums that have a disease from the PatNums that are passed in.</summary>
		public static List<long> GetPatientsWithDisease(List<long> listPatNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums.Count==0) {
				return new List<long>();
			}
			string command="SELECT DISTINCT PatNum FROM disease WHERE PatNum IN ("+string.Join(",",listPatNums)+") "
				+"AND disease.DiseaseDefNum != "+POut.Long(PrefC.GetLong(PrefName.ProblemsIndicateNone));
			return Db.GetListLong(command);
		}

		///<summary>Gets one disease by DiseaseNum from the db.</summary>
		public static Disease GetOne(long diseaseNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Disease>(MethodBase.GetCurrentMethod(),diseaseNum);
			}
			return Crud.DiseaseCrud.SelectOne(diseaseNum);
		}

		///<summary>Gets a Disease from the database. Returns null if no disease is found.</summary>
		public static Disease GetDiseaseForApi(long diseaseNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Disease>(MethodBase.GetCurrentMethod(),diseaseNum);
			}
			string command="SELECT * FROM disease WHERE DiseaseNum="+POut.Long(diseaseNum);
			return Crud.DiseaseCrud.SelectOne(command);
		}

		///<summary>Gets all Diseases from the database. Returns empty list if not found.</summary>
		public static List<Disease> GetDiseasesForApi(int limit,int offset,long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Disease>>(MethodBase.GetCurrentMethod(),limit,offset,patNum);
			}
			string command="SELECT * FROM disease ";
			if(patNum>0) {
				command+="WHERE PatNum="+POut.Long(patNum)+" ";
			}
			command+="ORDER BY DiseaseNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.DiseaseCrud.SelectMany(command);
		}

		///<summary>Gets a list of all Diseases for a given patient.  Includes hidden. Sorted by diseasedef.ItemOrder.</summary>
		public static List<Disease> Refresh(long patNum) {
			//No need to check MiddleTierRole; no call to db.
			return Refresh(patNum,false);
		}

		///<summary>Gets a list of all Diseases for a given patient. Set showActive true to only show active Diseases.</summary>
		public static List<Disease> Refresh(long patNum,bool showActiveOnly) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Disease>>(MethodBase.GetCurrentMethod(),patNum,showActiveOnly);
			}
			string command="SELECT disease.* FROM disease "
				+"WHERE PatNum="+POut.Long(patNum);
			if(showActiveOnly) {
				command+=" AND ProbStatus="+POut.Int((int)ProblemStatus.Active);
			}
			return Crud.DiseaseCrud.SelectMany(command);
		}

		//The above and below methods are awful.
		//They need to be combined and have patNum plus two bools.

		///<summary>Gets a list of all Diseases for a given patient. Show innactive returns all, otherwise only resolved and active problems.</summary>
		public static List<Disease> Refresh(bool showInnactive,long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Disease>>(MethodBase.GetCurrentMethod(),showInnactive,patNum);
			}
			string command="SELECT disease.* FROM disease "
				+"WHERE PatNum="+POut.Long(patNum);
			if(!showInnactive) {
				command+=" AND (ProbStatus="+POut.Int((int)ProblemStatus.Active)+" OR ProbStatus="+POut.Int((int)ProblemStatus.Resolved)+")";
			}
			return Crud.DiseaseCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<Disease> GetPatientData(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Disease>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM disease "
				+"WHERE PatNum="+POut.Long(patNum);
			return Crud.DiseaseCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(Disease disease) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),disease);
				return;
			}
			Crud.DiseaseCrud.Update(disease);
		}

		///<summary></summary>
		public static void Update(Disease disease,Disease oldDisease) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),disease,oldDisease);
				return;
			}
			Crud.DiseaseCrud.Update(disease,oldDisease);
		}

		///<summary></summary>
		public static long Insert(Disease disease) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				disease.DiseaseNum=Meth.GetLong(MethodBase.GetCurrentMethod(),disease);
				return disease.DiseaseNum;
			}
			return Crud.DiseaseCrud.Insert(disease);
		}

		///<summary></summary>
		public static void Delete(Disease disease) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),disease);
				return;
			}
			string command="DELETE FROM disease WHERE DiseaseNum ="+POut.Long(disease.DiseaseNum);
			Db.NonQ(command);
		}

		///<summary>Deletes all diseases for one patient.</summary>
		public static void DeleteAllForPt(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="DELETE FROM disease WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}

		public static List<long> GetChangedSinceDiseaseNums(DateTime changedSince,List<long> eligibleForUploadPatNumList) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince,eligibleForUploadPatNumList);
			}
			string strEligibleForUploadPatNums="";
			DataTable table;
			if(eligibleForUploadPatNumList.Count>0) {
				for(int i=0;i<eligibleForUploadPatNumList.Count;i++) {
					if(i>0) {
						strEligibleForUploadPatNums+="OR ";
					}
					strEligibleForUploadPatNums+="PatNum='"+eligibleForUploadPatNumList[i].ToString()+"' ";
				}
				string command="SELECT DiseaseNum FROM disease WHERE DateTStamp > "+POut.DateT(changedSince)+" AND ("+strEligibleForUploadPatNums+")";
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<long> diseasenums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				diseasenums.Add(PIn.Long(table.Rows[i]["DiseaseNum"].ToString()));
			}
			return diseasenums;
		}

		///<summary>Used along with GetChangedSinceDiseaseNums</summary>
		public static List<Disease> GetMultDiseases(List<long> diseaseNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Disease>>(MethodBase.GetCurrentMethod(),diseaseNums);
			}
			string strDiseaseNums="";
			DataTable table;
			if(diseaseNums.Count>0) {
				for(int i=0;i<diseaseNums.Count;i++) {
					if(i>0) {
						strDiseaseNums+="OR ";
					}
					strDiseaseNums+="DiseaseNum='"+diseaseNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM disease WHERE "+strDiseaseNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			Disease[] multDiseases=Crud.DiseaseCrud.TableToList(table).ToArray();
			List<Disease> diseaseList=new List<Disease>(multDiseases);
			return diseaseList;
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all diseases of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE disease SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all diseases of a patient that are the status specified.</summary>
		public static void ResetTimeStamps(long patNum,ProblemStatus status) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,status);
				return;
			}
			string command="UPDATE disease SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
				command+=" AND ProbStatus = "+POut.Int((int)status);
			Db.NonQ(command);
		}
		
		///<summary>Checks if disease(problem) can be deleted. If can't delete, throws exception with error containing the number of vital signs the disease is attached to and the 
		///dates of the vital sign exams. If disease can be deleted, does not throw exception and simply returns.</summary>
		public static void VerifyCanDelete(long diseaseNum) {
			List<Vitalsign> listVitalsigns=Vitalsigns.GetListFromPregDiseaseNum(diseaseNum);
			if(listVitalsigns.Count>0) {//if attached to vital sign exam, block delete
				string strDates="";
				for(int i=0;i<listVitalsigns.Count;i++) {
					if(i>5) {
						break;
					}
					strDates+="\r\n"+listVitalsigns[i].DateTaken.ToShortDateString();
				}
				throw new Exception("Not allowed to delete this problem. It is attached to "+listVitalsigns.Count.ToString()+" vital sign exams with dates including: "+strDates+".");
			}
		}	

		///<summary>Checks if disease(problem) can be updated. If can't be updated, throws exception with message containing the dates of the preganancy vital sign exams whose dates no longer fall
		///within the active dates of the disease. If disease can be updated, does not throw exception and simply returns.</summary>
		public static void VerifyCanUpdate(Disease disease) {
			//See if this problem is the pregnancy linked to a vitalsign exam
			List<Vitalsign> listVitalsigns=Vitalsigns.GetListFromPregDiseaseNum(disease.DiseaseNum);
			if(listVitalsigns.Count>0) {
				//See if the vitalsign exam date is now outside of the active dates of the disease (pregnancy)
				string strDates="";
				for(int i=0;i<listVitalsigns.Count;i++) {
					if(listVitalsigns[i].DateTaken<disease.DateStart 
						|| (disease.DateStop.Year>1880 && listVitalsigns[i].DateTaken>disease.DateStop)) {
						strDates+="\r\n"+listVitalsigns[i].DateTaken.ToShortDateString();
					}
				}
				//If vitalsign exam is now outside the dates of the problem, tell the user they must fix the dates of the pregnancy dx
				if(strDates.Length>0) {
					throw new Exception("This problem is attached to 1 or more vital sign exams as a pregnancy diagnosis with dates:"+strDates+"\r\nNot allowed to change the active dates of " +
						"the diagnosis to be outside the dates of the exam(s).  You must first remove the diagnosis from the vital sign exam(s).");
				}
			}
		}

		///<summary>Takes in vitalsign object and sets its fields to the other passed in arguments. Does not set all of the vitalsign object's fields.</summary>
		public static Disease SetDiseaseFields(Disease disease,DateTime dateStart,DateTime dateStop,ProblemStatus probStatus,string patNote,SnomedProblemTypes snomedProblemType,
			FunctionalStatus functionStatus) {
			disease.DateStart=dateStart;
			disease.DateStop=dateStop;
			disease.ProbStatus=probStatus;
			disease.PatNote=patNote;
			disease.FunctionStatus=functionStatus;
			switch(snomedProblemType) { 
				case SnomedProblemTypes.Finding:
					disease.SnomedProblemType="404684003";
					break;
				case SnomedProblemTypes.Complaint:
					disease.SnomedProblemType="409586006";
					break;
				case SnomedProblemTypes.Diagnosis:
					disease.SnomedProblemType="282291009";
					break;
				case SnomedProblemTypes.Condition:
					disease.SnomedProblemType="64572001";
					break;
				case SnomedProblemTypes.FunctionalLimitation:
					disease.SnomedProblemType="248536006";
					break;
				case SnomedProblemTypes.Symptom:
					disease.SnomedProblemType="418799008";
					break;
				case SnomedProblemTypes.Problem:
					disease.SnomedProblemType="55607006";
					break;
			}
			return disease;
		}	
	}

		



		
	

	

	


}










