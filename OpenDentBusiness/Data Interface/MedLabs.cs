using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeBase;
using OpenDentBusiness.FileIO;
using OpenDentBusiness.HL7;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MedLabs{
		///<summary>Gets one MedLab from the db.</summary>
		public static MedLab GetOne(long medLabNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<MedLab>(MethodBase.GetCurrentMethod(),medLabNum);
			}
			return Crud.MedLabCrud.SelectOne(medLabNum);
		}

		///<summary>Get all MedLab objects for a specific patient from the database.  Can return an empty list.</summary>
		public static List<MedLab> GetForPatient(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MedLab>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM medlab WHERE PatNum="+POut.Long(patNum);
			return Crud.MedLabCrud.SelectMany(command);
		}

		public static int GetCountForPatient(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) FROM medlab WHERE PatNum="+POut.Long(patNum);
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Get unique MedLab orders, grouped by PatNum, ProvNum, and SpecimenID.  Also returns the most recent DateTime the results
		///were released from the lab and a list of test descriptions ordered.  If includeNoPat==true, the lab orders not attached to a patient will be 
		///included.  Filtered by MedLabs for the list of clinics supplied based on the medlab.PatAccountNum=clinic.MedLabAccountNum.  ClinicNum 0 will
		///be for those medlabs with PatAccountNum that does not match any of the MedLabAccountNums set for a clinic.  listSelectedClinics is already
		///filtered to only those clinics for which the current user has permission to access based on ClinicIsRestricted.  If clinics are not enabled,
		///listSelectedClinics will contain 0 and all medlabs will be returned.</summary>
		public static List<MedLab> GetOrdersForPatient(Patient patient,bool includeNoPat,bool onlyNoPat,DateTime dateReportedStart,DateTime dateReportedEnd,
			List<Clinic> listClinicsSelected)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MedLab>>(MethodBase.GetCurrentMethod(),patient,includeNoPat,onlyNoPat,dateReportedStart,dateReportedEnd,
					listClinicsSelected);
			}
			//include all patients unless a patient is specified.
			string patNumClause="medlab.PatNum>0";
			if(patient!=null) {
				patNumClause="medlab.PatNum="+POut.Long(patient.PatNum);
			}
			//do not include patnum=0 unless specified.
			if(includeNoPat) {
				patNumClause+=" OR medlab.PatNum=0";
			}
			if(onlyNoPat) {
				patNumClause="medlab.PatNum=0";
			}
			List<string> listWhereClauseStrs=new List<string>();
			if(PrefC.HasClinicsEnabled) {
				List<string> listAllClinicAcctNums=Clinics.GetWhere(x => !string.IsNullOrWhiteSpace(x.MedLabAccountNum)).Select(x => x.MedLabAccountNum).ToList();
				if(listClinicsSelected.Any(x => x.ClinicNum==0) && listAllClinicAcctNums.Count>0) {//include "Unassigned" medlabs
					listWhereClauseStrs.Add("medlab.PatAccountNum NOT IN ("+string.Join(",",listAllClinicAcctNums)+")");
				}
				listClinicsSelected.RemoveAll(x => x.ClinicNum<=0 || string.IsNullOrWhiteSpace(x.MedLabAccountNum));
				if(listClinicsSelected.Count>0) {
					listWhereClauseStrs.Add("medlab.PatAccountNum IN ("+string.Join(",",listClinicsSelected.Select(x => x.MedLabAccountNum))+")");
				}
			}
			string command="SELECT MAX(CASE WHEN medlab.DateTimeReported=maxDate.DateTimeReported THEN MedLabNum ELSE 0 END) AS MedLabNum,"
				+"SendingApp,SendingFacility,medlab.PatNum,medlab.ProvNum,PatIDLab,PatIDAlt,PatAge,PatAccountNum,PatFasting,medlab.SpecimenID,"
				+"SpecimenIDFiller,ObsTestID,ObsTestLoinc,ObsTestLoincText,DateTimeCollected,TotalVolume,ActionCode,ClinicalInfo,"
				+"MIN(DateTimeEntered) AS DateTimeEntered,OrderingProvNPI,OrderingProvLocalID,OrderingProvLName,OrderingProvFName,SpecimenIDAlt,"
				+"maxDate.DateTimeReported,MIN(CASE WHEN medlab.DateTimeReported=maxDate.DateTimeReported THEN ResultStatus ELSE NULL END) AS ResultStatus,"
				+"ParentObsID,ParentObsTestID,NotePat,NoteLab,FileName,"
				+"MIN(CASE WHEN medlab.DateTimeReported=maxDate.DateTimeReported THEN OriginalPIDSegment ELSE NULL END) AS OriginalPIDSegment,"
				+DbHelper.GroupConcat("ObsTestDescript",distinct:true,separator:"\r\n")+" AS ObsTestDescript "
				+"FROM medlab "
				+"INNER JOIN ("
					+"SELECT PatNum,ProvNum,SpecimenID,MAX(DateTimeReported) AS DateTimeReported "
					+"FROM medlab "
					+"WHERE ("+patNumClause+") " //Ex: WHERE (medlab.PatNum>0 OR medlab.Patnum=0)
					+"GROUP BY PatNum,ProvNum,SpecimenID "
					+"HAVING "+DbHelper.DtimeToDate("MAX(DateTimeReported)")+" BETWEEN "+POut.Date(dateReportedStart)+" AND "+POut.Date(dateReportedEnd)
				+") maxDate ON maxDate.PatNum=medlab.PatNum AND maxDate.ProvNum=medlab.ProvNum AND maxDate.SpecimenID=medlab.SpecimenID ";
			if(PrefC.HasClinicsEnabled && listWhereClauseStrs.Count>0) {
				command+="WHERE ("+string.Join(" OR ",listWhereClauseStrs)+") ";
			}
			command+="GROUP BY medlab.PatNum,medlab.ProvNum,medlab.SpecimenID "
				+"ORDER BY maxDate.DateTimeReported DESC,medlab.SpecimenID,MedLabNum";//most recently received lab on top, with all for a specific specimen together
			return Crud.MedLabCrud.SelectMany(command);
		}

		///<summary>Get MedLabs for a specific patient and a specific SpecimenID, SpecimenIDFiller combination.
		///Ordered by DateTimeReported descending, MedLabNum descending so the most recently reported/processed message is first in the list.
		///If using random primary keys, this information may be incorectly ordered, but that is only an annoyance and this function should still work.</summary>
		public static List<MedLab> GetForPatAndSpecimen(long patNum,string specimenID,string specimenIDFiller) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MedLab>>(MethodBase.GetCurrentMethod(),patNum,specimenID,specimenIDFiller);
			}
			string command="SELECT * FROM medlab WHERE PatNum="+POut.Long(patNum)+" "
				+"AND SpecimenID='"+POut.String(specimenID)+"' "
				+"AND SpecimenIDFiller='"+POut.String(specimenIDFiller)+"' "
				+"ORDER BY DateTimeReported DESC,MedLabNum DESC";
			return Crud.MedLabCrud.SelectMany(command);
		}

		public static void UpdateFileNames(List<long> listMedLabNums,string fileNameNew) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listMedLabNums,fileNameNew);
				return;
			}
			string command="UPDATE medlab SET FileName='"+POut.String(fileNameNew)+"' WHERE MedLabNum IN("+string.Join(",",listMedLabNums)+")";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(MedLab medLab){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				medLab.MedLabNum=Meth.GetLong(MethodBase.GetCurrentMethod(),medLab);
				return medLab.MedLabNum;
			}
			return Crud.MedLabCrud.Insert(medLab);
		}

		///<summary></summary>
		public static void Update(MedLab medLab){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medLab);
				return;
			}
			Crud.MedLabCrud.Update(medLab);
		}

		///<summary>Sets the PatNum column on MedLabs with MedLabNum in list.  Used when manually assigning/moving MedLabs to a patient.</summary>
		public static void UpdateAllPatNums(List<long> listMedLabNums,long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listMedLabNums,patNum);
				return;
			}
			if(listMedLabNums.Count<1) {
				return;
			}
			string command="UPDATE medlab SET PatNum="+POut.Long(patNum)+" WHERE MedLabNum IN("+String.Join(",",listMedLabNums)+")";
			Db.NonQ(command);
		}

		///<summary>Reprocess the original HL7 msgs for any MedLabs with PatNum 0, creates the embedded PDF files from the base64 text in the ZEF segments
		///<para>The old method used when parsing MedLab HL7 msgs was to wait to extract these files until the msg was manually associated with a patient.
		///Associating the MedLabs to a patient and reprocessing the HL7 messages using middle tier was very slow.</para>
		///<para>The new method is to create the PDF files and save them in the image folder in a subdirectory called "MedLabEmbeddedFiles" if a patient
		///isn't located from the details in the PID segment of the message.  Associating the MedLabs to a pat is now just a matter of moving the files to
		///the pat's image folder and updating the PatNum columns.  All files are now extracted and stored, either in a pat's folder or in the
		///"MedLabEmbeddedFiles" folder, by the HL7 service.</para>
		///<para>This will reprocess all HL7 messages for MedLabs with PatNum=0 and replace the MedLab, MedLabResult, MedLabSpecimen, and MedLabFacAttach
		///rows as well as create any embedded files and insert document table rows.  The document table rows will have PatNum=0, just like the MedLabs,
		///if a pat is still not located with the details in the PID segment.  Once the user manually attaches the MedLab to a patient, all rows will be
		///updated with the correct PatNum and the embedded PDFs will be moved to the pat's image folder.  The document.FileName column will contain the
		///name of the file, regardless of where it is located.  The file name will be updated to a relevant name for the folder in which it is located.
		///i.e. in the MedLabEmbeddedFiles directory it may be named 3YG8Z420150909100527.pdf, but once moved to a pat's folder it will be renamed to
		///something like PatientAustin375.pdf and the document.FileName column will be the current name.</para>
		///<para>If storing images in the db, the document table rows will contain the base64 text version of the PDFs with PatNum=0 and will be updated
		///with the correct PatNum once associated.  The FileName will be just the extension ".pdf" until it is associated with a patient at which time it
		///will be updated to something like PatientAustin375.pdf.</para></summary>
		public static int Reconcile() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM medlab WHERE PatNum=0";
			List<MedLab> listMedLabs=Crud.MedLabCrud.SelectMany(command);
			if(listMedLabs.Count<1) {
				return 0;
			}
			List<long> listMedLabNumsNew=new List<long>();//used to delete old MedLab objects after creating these new ones from the HL7 message text
			int failedCount=0;
			foreach(string relativePath in listMedLabs.Select(x => x.FileName).Distinct().ToList()) {
				string fileText="";
				try {
					if(PrefC.AtoZfolderUsed!=DataStorageType.InDatabase) {
						fileText=FileAtoZ.ReadAllText(FileAtoZ.CombinePaths(ImageStore.GetPreferredAtoZpath(),relativePath));
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//To avoid a warning message.  The ex is needed to ensure all exceptions are caught.
					failedCount++;
					continue;
				}
				MessageHL7 messageHL7=new MessageHL7(fileText);
				List<long> listMedLabNums=MessageParserMedLab.Process(messageHL7,relativePath,false);//re-creates the documents from the ZEF segments
				if(listMedLabNums==null || listMedLabNums.Count<1) {
					failedCount++;
					continue;//not sure what to do, just move on?
				}
				listMedLabNumsNew.AddRange(listMedLabNums);
				MedLabs.UpdateFileNames(listMedLabNums,relativePath);
			}
			//Delete all MedLabs, MedLabResults, MedLabSpecimens, and MedLabFacAttaches except the ones just created
			//Don't delete until we successfully process the messages and have valid new MedLab objects
			foreach(MedLab medLab in listMedLabs) {
				failedCount+=DeleteLabsAndResults(medLab,listMedLabNumsNew);
			}
			return failedCount;
		}

		///<summary>Cascading delete that deletes all MedLab, MedLabResult, MedLabSpecimen, and MedLabFacAttach.
		///Also deletes any embedded PDFs that are linked to by the MedLabResults.
		///The MedLabs and all associated results, specimens, and FacAttaches referenced by the MedLabNums in listExcludeMedLabNums will not be deleted.
		///Used for deleting old entries and keeping new ones.  The list may be empty and then all will be deleted.</summary>
		public static int DeleteLabsAndResults(MedLab medLab,List<long> listExcludeMedLabNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),medLab,listExcludeMedLabNums);
			}
			List<MedLab> listMedLabsOld=MedLabs.GetForPatAndSpecimen(medLab.PatNum,medLab.SpecimenID,medLab.SpecimenIDFiller);//patNum could be 0
			if(listExcludeMedLabNums!=null) {
				listMedLabsOld=listMedLabsOld.FindAll(x => !listExcludeMedLabNums.Contains(x.MedLabNum));
			}
			if(listMedLabsOld.Count<1) {
				return 0;
			}
			int failedCount=0;
			List<long> listLabNumsOld=listMedLabsOld.Select(x => x.MedLabNum).ToList();
			List<MedLabResult> listMedLabResultsOld=listMedLabsOld.SelectMany(x => x.ListMedLabResults).ToList();//sends one query to the db per MedLab
			MedLabFacAttaches.DeleteAllForLabsOrResults(listLabNumsOld,listMedLabResultsOld.Select(x => x.MedLabResultNum).ToList());
			MedLabSpecimens.DeleteAllForLabs(listLabNumsOld);//MedLabSpecimens have a FK to MedLabNum
			MedLabResults.DeleteAllForMedLabs(listLabNumsOld);//MedLabResults have a FK to MedLabNum
			MedLabs.DeleteAll(listLabNumsOld);
			foreach(Document document in Documents.GetByNums(listMedLabResultsOld.Select(x => x.DocNum).ToList())) {
				Patient patient=Patients.GetPat(document.PatNum);
				if(patient==null) {
					Documents.Delete(document);
					continue;
				}
				try {
					ImageStore.DeleteDocuments(new List<Document> { document },ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath()));
				}
				catch(Exception ex) {
					ex.DoNothing();//To avoid a warning message.  The ex is needed to ensure all exceptions are caught.
					failedCount++;
				}
			}
			return failedCount;
		}

		///<summary>Translates enum values into human readable strings.</summary>
		public static string GetStatusDescript(ResultStatus resultStatus) {
			Meth.NoCheckMiddleTierRole();
			switch(resultStatus) {
				case ResultStatus.C:
					return "Corrected";
				case ResultStatus.F:
					return "Final";
				case ResultStatus.I:
					return "Incomplete";
				case ResultStatus.P:
					return "Preliminary";
				case ResultStatus.X:
					return "Canceled";
				default:
					return "";
			}
		}

		///<summary>Delete all of the MedLab objects by MedLabNum.</summary>
		public static void DeleteAll(List<long> listMedLabNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listMedLabNums);
				return;
			}
			string command= "DELETE FROM medlab WHERE MedLabNum IN("+String.Join(",",listMedLabNums)+")";
			Db.NonQ(command);
		}
	}
}