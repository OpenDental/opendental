using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PatientRaces{
		///<summary>Gets all PatientRace entries from the db for the specified patient and includes the non-db fields Description, IsEthnicity, and
		///HiearchicalCode.</summary>
		public static List<PatientRace> GetForPatient(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatientRace>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=@"SELECT patientrace.*,COALESCE(cdcrec.Description,'') Description,
				(CASE WHEN cdcrec.HeirarchicalCode LIKE 'E%' THEN 1 ELSE 0 END) IsEthnicity,
				COALESCE(cdcrec.HeirarchicalCode,'') HeirarchicalCode
				FROM patientrace 
				LEFT JOIN cdcrec ON cdcrec.CdcrecCode=patientrace.CdcrecCode
				WHERE PatNum="+POut.Long(patNum);
			DataTable table=Db.GetTable(command);
			List<PatientRace> listPatientRaces=Crud.PatientRaceCrud.TableToList(table);
			for(int i=0;i<table.Rows.Count;i++) {
				switch(listPatientRaces[i].CdcrecCode) {
					case PatientRace.DECLINE_SPECIFY_RACE_CODE:
						listPatientRaces[i].Description=Lans.g("PatientRaces","DECLINED TO SPECIFY");
						listPatientRaces[i].IsEthnicity=false;
						break;
					case PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE:
						listPatientRaces[i].Description=Lans.g("PatientRaces","DECLINED TO SPECIFY");
						listPatientRaces[i].IsEthnicity=true;
						break;
					case PatientRace.MULTI_RACE_CODE:
						listPatientRaces[i].Description=Lans.g("PatientRaces","MULTIRACIAL");
						listPatientRaces[i].IsEthnicity=false;
						break;
					default:
						listPatientRaces[i].Description=PIn.String(table.Rows[i]["Description"].ToString());
						listPatientRaces[i].IsEthnicity=(table.Rows[i]["IsEthnicity"].ToString()=="1");
						listPatientRaces[i].HeirarchicalCode=PIn.String(table.Rows[i]["HeirarchicalCode"].ToString());
						break;
				}
			}
			return listPatientRaces;
		}
		
		///<summary>Returns the PatientRaceOld enum based on the PatientRace entries for the patient passed in.  Calls GetPatRaceList to get the list of races.</summary>
		public static PatientRaceOld GetPatientRaceOldFromPatientRaces(long patNum) {
			//No need to check RemotingRole; no call to db.
			List<PatientRace> races=GetForPatient(patNum);
			if(races.Count==0) {
				return PatientRaceOld.Unknown;//Unknown is default for PatientRaceOld
			}
			if(races.Any(x => x.HeirarchicalCode=="R5" || x.HeirarchicalCode.StartsWith("R5."))) {
				if(races.Any(x => x.HeirarchicalCode=="E1" || x.HeirarchicalCode.StartsWith("E1."))) {
					return PatientRaceOld.HispanicLatino;
				}
				return PatientRaceOld.White;
			}
			if(races.Any(x => x.HeirarchicalCode=="R3" || x.HeirarchicalCode.StartsWith("R3."))) {
				if(races.Any(x => x.HeirarchicalCode=="E1" || x.HeirarchicalCode.StartsWith("E1."))) {
					return PatientRaceOld.BlackHispanic;
				}
				return PatientRaceOld.AfricanAmerican;
			}
			if(races.Any(x => x.HeirarchicalCode=="R4")) {
				return PatientRaceOld.Aboriginal;
			}
			if(races.Any(x => x.HeirarchicalCode=="R1" || x.HeirarchicalCode.StartsWith("R1."))) {
				return PatientRaceOld.AmericanIndian;
			}
			if(races.Any(x => x.HeirarchicalCode=="R2" || x.HeirarchicalCode.StartsWith("R2."))) {
				return PatientRaceOld.Asian;
			}
			if(races.Any(x => x.HeirarchicalCode=="R4" || x.HeirarchicalCode.StartsWith("R4."))) {
				return PatientRaceOld.HawaiiOrPacIsland;
			}
			if(races.Any(x => x.HeirarchicalCode=="R9")) {
				return PatientRaceOld.Other;
			}
			//Hispanic
			//DeclinedToSpecify
			return PatientRaceOld.Unknown;
		}

		///<summary>Gets a list of PatRaces that correspond to a PatientRaceOld enum.</summary>
		public static List<PatientRace> GetPatRacesFromPatientRaceOld(PatientRaceOld raceOld,long patNum) {
			List<PatientRace> retVal=new List<PatientRace>();
			switch(raceOld) {
				case PatientRaceOld.Unknown:
					//Do nothing.  No entry means "Unknown", the old default.
					break;
				case PatientRaceOld.Multiracial:
				case PatientRaceOld.Other:
					retVal.Add(new PatientRace(patNum,"2131-1"));//Other race
					break;
				case PatientRaceOld.HispanicLatino:
					retVal.Add(new PatientRace(patNum,"2106-3"));//White
					retVal.Add(new PatientRace(patNum,"2135-2"));//Hispanic
					break;
				case PatientRaceOld.AfricanAmerican:
					retVal.Add(new PatientRace(patNum,"2054-5"));//Black or African American
					break;
				case PatientRaceOld.White:
					retVal.Add(new PatientRace(patNum,"2106-3"));//White
					break;
				case PatientRaceOld.HawaiiOrPacIsland:
					retVal.Add(new PatientRace(patNum,"2076-8"));//Hawaiian or Pacific Islander
					break;
				case PatientRaceOld.AmericanIndian:
					retVal.Add(new PatientRace(patNum,"1002-5"));//AmericanIndian
					break;
				case PatientRaceOld.Asian:
					retVal.Add(new PatientRace(patNum,"2028-9"));//Black or African American
					break;
				case PatientRaceOld.Aboriginal:
					retVal.Add(new PatientRace(patNum,"2500-7"));//OTHER PACIFIC ISLANDER
					break;
				case PatientRaceOld.BlackHispanic:
					retVal.Add(new PatientRace(patNum,"2054-5"));//Black or African American
					retVal.Add(new PatientRace(patNum,"2135-2"));//Hispanic
					break;
			}
			return retVal;
		}

		///<summary>Returns a comma-delimited string of descriptions for the races passed in where IsEthinicity is false.</summary>
		public static string GetRaceDescription(List<PatientRace> listPatRaces) {
			//No remoting role check; no call to db
			if(listPatRaces.Count(x => !x.IsEthnicity)==0) {
				return "";
			}
			return string.Join(", ",listPatRaces.Where(x => !x.IsEthnicity).Select(x => x.Description));
		}
		
		///<summary>Returns a comma-delimited string of descriptions for the races passed in where IsEthinicity is true.</summary>
		public static string GetEthnicityDescription(List<PatientRace> listPatRaces) {
			//No remoting role check; no call to db
			if(listPatRaces.Count(x => x.IsEthnicity)==0) {
				return "";
			}
			return string.Join(", ",listPatRaces.Where(x => x.IsEthnicity).Select(x => x.Description));
		}
		
		///<summary>Inserts or Deletes neccesary PatientRace entries for the specified patient given the list of PatientRaces provided.</summary>
		public static void Reconcile(long patNum,List<PatientRace> listPatRaces) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,listPatRaces);
				return;
			}
			string command;
			if(listPatRaces.Count==0) { //DELETE all for the patient if listPatRaces is empty.
				command="DELETE FROM patientrace WHERE PatNum = "+POut.Long(patNum);//Can't use CRUD layer here because there might be multiple races for one patient.
				Db.NonQ(command);
				return;
			}
			List<PatientRace> listPatientRacesDB;
			command="SELECT * FROM patientrace WHERE PatNum = "+POut.Long(patNum);
			listPatientRacesDB=Crud.PatientRaceCrud.SelectMany(command);
			//delete excess rows
			for(int i=0;i<listPatientRacesDB.Count;i++) {
				if(!listPatRaces.Any(x => x.CdcrecCode==listPatientRacesDB[i].CdcrecCode)) {//if there is a PatientRace row that does not match the new list of PatientRaces, delete it
					Crud.PatientRaceCrud.Delete(listPatientRacesDB[i].PatientRaceNum);
				}
			}
			//delete duplicate rows
			for(int i=0;i<listPatientRacesDB.Count;i++) {
				if(!listPatRaces.Any(x => x.CdcrecCode==listPatientRacesDB[i].CdcrecCode)) {
					continue; //It was already deleted earlier
				}
				for(int j=i+1;j<listPatientRacesDB.Count;j++) {
					if(listPatientRacesDB[i].CdcrecCode==listPatientRacesDB[j].CdcrecCode) { //If there are duplicate races in the DB that weren't deleted before.
						Crud.PatientRaceCrud.Delete(listPatientRacesDB[j].PatientRaceNum);
					}
				}
			}
			//insert new rows
			for(int i=0;i<listPatRaces.Count;i++) {
				bool insertNeeded=true;
				for(int j=0;j<listPatientRacesDB.Count;j++) {
					if(listPatRaces[i].CdcrecCode==listPatientRacesDB[j].CdcrecCode) {
						insertNeeded=false;
					}
				}
				if(insertNeeded) {
					listPatRaces[i].PatNum=patNum;//Just to be safe
					Crud.PatientRaceCrud.Insert(listPatRaces[i]);
				}
				//next PatRace
			}
			//return;
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<PatientRace> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatientRace>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM patientrace WHERE PatNum = "+POut.Long(patNum);
			return Crud.PatientRaceCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(PatientRace patientRace){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				patientRace.PatientRaceNum=Meth.GetLong(MethodBase.GetCurrentMethod(),patientRace);
				return patientRace.PatientRaceNum;
			}
			return Crud.PatientRaceCrud.Insert(patientRace);
		}

		///<summary></summary>
		public static void Delete(long patientRaceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patientRaceNum);
				return;
			}
			string command= "DELETE FROM patientrace WHERE PatientRaceNum = "+POut.Long(patientRaceNum);
			Db.NonQ(command);
		}
		*/
	}
}