using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CodeBase;
using OpenDentBusiness;
using OpenDental.Thinfinity;
using Newtonsoft.Json;

namespace OpenDental {
	///<summary></summary>
	public class MedicationL {
		///<summary>A subset of Medication class's user-friendly members.</summary>
		private class MedicationExport {
			///<summary>Name of the medication.</summary>
			public string MedName;
			///<summary>Name of the generic. Not an actual member or DB column in Medication, but is the user-friendly form.</summary>
			public string GenericName;
			///<summary>Notes.</summary>
			public string Notes;
			///<summary>RxNorm Code identifier.</summary>
			public long RxCui;

			public MedicationExport(Medication medication=null) {
				if(medication==null) {
					return;
				}
				MedName=medication.MedName;
				GenericName=Medications.GetGenericName(medication.GenericNum);
				Notes=medication.Notes;
				RxCui=medication.RxCui;
			}
		}

		///<summary>Throws Exception. Downloads default medications list from OpenDental.com; returns filename of temp file.</summary>
		public static string DownloadDefaultMedicationsFile() {
			string tempFile=PrefC.GetRandomTempFile(".tmp");
			using(WebClient webClient=new WebClient()) {
				webClient.DownloadFile("http://www.opendental.com/medications/DefaultMedications.txt",tempFile);
			}
			return tempFile;
		}

		///<summary>Inserts any new medications in listNewMeds, as well as updating any existing medications in listExistingMeds in conflict with 
		///the corresponding new medication.</summary>
		public static int ImportMedications(List<Medication> listImportMeds,List<Medication> listMedicationsExisting) {
			int countImportedMedications=0;
			for(int i=0;i<listImportMeds.Count;i++){
				//Find any duplicate existing medications with the new medication
				if(IsDuplicateMed(listImportMeds[i],listMedicationsExisting)) {
					continue;//medNew already exists, skip it.
				}
				InsertNewMed(listImportMeds[i],listMedicationsExisting);
				countImportedMedications++;
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0
				,Lans.g("Medications","Imported")+" "+POut.Int(countImportedMedications)+" "+Lans.g("Medications","medications.")
			);
			return countImportedMedications;
		}

		///<summary>Determines if med is a duplicate of another Medication in listMedsExisting.
		///Given medGenNamePair is a medication that we are checking and the given generic name if set.
		///A duplicate is defined as MedName is equal, GenericName is equal, RxCui is equal and either Notes is equal or not defined.
		///A new medication with all properties being equal to an existing medication except with a blank Notes property is considered to be a 
		///duplicate, as it is likely the existing Medication is simply a user edited version of the same Medication.</summary>
		private static bool IsDuplicateMed(Medication medication,List<Medication> listMedicationsExisting) {
			bool isNoteChecked=true;
			//If everything is identical, except med.Notes is blank while x.Notes is not blank, we consider this to be a duplicate.
			if(string.IsNullOrEmpty(medication.Notes)) {
				isNoteChecked=false;
			}
			if(!listMedicationsExisting.Any(x=>x.MedName.Trim().ToLower()==medication.MedName.Trim().ToLower())){
				return false;
			}
			if(!listMedicationsExisting.Any(x=>Medications.GetGenericName(x.GenericNum).Trim().ToLower()==medication.GenericName.Trim().ToLower())){
				return false;
			}
			if(!listMedicationsExisting.Any(x=>x.RxCui==medication.RxCui)){
				return false;
			}
			if(!isNoteChecked){
				return true;
			}
			if(!listMedicationsExisting.Any(x=>x.Notes.Trim().ToLower()==medication.Notes.Trim().ToLower())){
				return false;
			}
			return true;
		}

		///<summary>Inserts. Assigns genericNum if it finds a name in the list that matches the imported genericName.</summary>
		private static void InsertNewMed(Medication medication,List<Medication> listMedicationsExisting) {
			long genericNum=0;
			Medication medicationExisting=listMedicationsExisting.Find(x=>x.MedName==medication.GenericName);
			if(medicationExisting!=null){
				genericNum=medicationExisting.GenericNum;
			}
			if(genericNum!=0) {//Found a match.
				medication.GenericNum=genericNum;
			}
			Medications.Insert(medication);//Assigns new primary key.
			if(genericNum==0) {//Found no match initially, assume given medication is the generic.
				medication.GenericNum=medication.MedicationNum;
				Medications.Update(medication);
			}
			listMedicationsExisting.Add(medication);//Keep in memory list and database in sync.
		}
		
		///<summary>Throws Exception.  Exports all medications to the passed in filename. Throws Exceptions.</summary>
		public static int ExportMedications(string filename,List<Medication> listMedications) {
			List<MedicationExport> listMedicationExports=new List<MedicationExport>();
			for(int i=0;i < listMedications.Count;i++) {
				listMedicationExports.Add(new MedicationExport(listMedications[i]));
			}
			string json=JsonConvert.SerializeObject(listMedicationExports,Formatting.Indented);
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filename,json);
			}
			else {
				File.WriteAllText(filename,json);//Allow Exception to trickle up.
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,
				Lans.g("Medications","Exported")+" "+POut.Int(listMedications.Count)+" "+Lans.g("Medications","medications to:")+" "+filename
			);
			return listMedications.Count;
		}

		///<summary>Throws exception.  Reads tab delimited medication information from given filename.
		///Returns the list of new medications with all generic medications before brand medications.
		///For V1 and V2, file required to be formatted such that each row contain: MedName\tGenericName\tNotes\tRxCui.
		///Newer version exports as JSON file, so as long as it contains MedName, GenericName, Notes, and RxCui fields, it will be compatible.
		///</summary>
		public static List<Medication> GetMedicationsFromFile(string filename,bool isTempFile=false) {
			List<Medication> listMedications=new List<Medication>();
			if(string.IsNullOrEmpty(filename)) {
				return listMedications;	
			}
			string medicationData=File.ReadAllText(filename);
			if(isTempFile) {
				File.Delete(filename);
			}
			if(string.IsNullOrWhiteSpace(medicationData)) {
				return listMedications;
			}
			List<MedicationExport> listMedicationsExport = new List<MedicationExport>();
			try {//New method: deserialize a JSON file.
				listMedicationsExport=JsonConvert.DeserializeObject<List<MedicationExport>>(medicationData);
			}
			catch(Exception) {//V1 and V2, our old custom export file.
				List<string[]> listMedLines=SplitLines(medicationData);
				for(int i=0;i<listMedLines.Count;i++){
					if(listMedLines[i].Length!=4) {
						throw new ODException(Lan.g("Medications","Invalid formatting detected in file."));
					}
					Medication medication=new Medication();
					medication.MedName=PIn.String(listMedLines[i][0]).Trim();//MedName
					medication.GenericName=PIn.String(listMedLines[i][1]).Trim();//GenericName
					medication.Notes=PIn.String(listMedLines[i][2]).Trim();//Notes
					medication.RxCui=PIn.Long(listMedLines[i][3]);//RxCui
					listMedications.Add(medication);
				}
				return SortMedGenericsFirst(listMedications);
			}
			for(int i=0;i < listMedicationsExport.Count;i++) {
				Medication medication=new Medication();
				medication.MedName=listMedicationsExport[i].MedName;
				medication.GenericName=listMedicationsExport[i].GenericName;
				medication.Notes=listMedicationsExport[i].Notes;
				medication.RxCui=listMedicationsExport[i].RxCui;
				listMedications.Add(medication);
			}
			return SortMedGenericsFirst(listMedications);
		}

		///<summary>Returns a list of string arrays for the provided data.
		///Lines are determined by new line characters and tabs between fields.</summary>
		private static List<string[]> SplitLines(string data) {
			List<string[]> listStringsLines=new List<string[]>();
			if(string.IsNullOrWhiteSpace(data)) {
				return listStringsLines;
			}
			if(data[0]!='"') {
				return SplitLinesOld(data);
			}
			bool isFieldStarted=false;
			string field="";
			List<string> listStringsMedLines=data.Split("\r\n",StringSplitOptions.RemoveEmptyEntries).ToList();
			for(int j=0;j<listStringsMedLines.Count;j++){
				List<string> listStringsFields=new List<string>();
				for(int i=0;i<listStringsMedLines[j].Length;i++) {
					char c=listStringsMedLines[j][i];
					if(!isFieldStarted) {
						if(c=='"') {//Start of a new field.
							isFieldStarted=true;
							continue;
						}
						else if(c=='\t') {
							continue;
						}
						else {//Character outside of an encapsulated field.  Invalid formatting..
							throw new Exception(Lan.g("Medications","Invalid formatting in Medication file."));
						}
					}
					if(c=='"' && (field.Length==0 || field[field.Length-1]!='\\')) {//End of a field.
						isFieldStarted=false;
						listStringsFields.Add(field.Replace("\\\"","\""));//Unescape any " in the field.
						field="";
						continue;
					}
					//Normal character inside a field.
					field+=c;
				}
				listStringsLines.Add(listStringsFields.ToArray());
			}
			return listStringsLines;
		}

		///<summary>Backwards compatible approach for medications exported before the change that encapsulates the exported data.</summary>
		private static List<string[]> SplitLinesOld(string data) {
			List<string[]> listStringsLines=new List<string[]>();
			//Backward compatible
			if(data.Contains("\r\n")){
				data=data.Replace("\r\n","\n");
			}
			string[] stringArray=data.Split('\n');
			for(int i=0;i<stringArray.Length;i++){//Remove any non Medication lines.
				string[] stringArrayFields=stringArray[i].Split('\t');
				if(stringArrayFields.Length<1 || string.IsNullOrEmpty(stringArrayFields[0])) {//Skip blank lines, blank MedicationName.
					continue;
				}
				listStringsLines.Add(stringArrayFields);
			}
			return listStringsLines;
		}

		///<summary>Custom sorting so that generic medications are above branded medications, then returns that list.</summary>
		private static List<Medication> SortMedGenericsFirst(List<Medication> listMedications) {
			List<Medication> listMedicationsGeneric=new List<Medication>();
			List<Medication> listMedicationsBranded=new List<Medication>();
			for(int i=0;i<listMedications.Count;i++){
				if(string.IsNullOrWhiteSpace(listMedications[i].GenericName)){//if generic is blank
					listMedicationsGeneric.Add(listMedications[i]);
					continue;
				}
				if(listMedications[i].MedName==listMedications[i].GenericName){//generic name matches
					listMedicationsGeneric.Add(listMedications[i]);
					continue;
				}
				listMedicationsBranded.Add(listMedications[i]);
			}
			listMedicationsGeneric.AddRange(listMedicationsBranded);
			return listMedicationsGeneric;
		}



	}
}