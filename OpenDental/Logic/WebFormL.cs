using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using OpenDentBusiness.WebTypes.WebForms;
using OpenDental.UI;
using System.Globalization;

namespace OpenDental {
	public class WebFormL {
		public static string SynchUrlStaging="https://10.10.1.196/WebHostSynch/SheetsSynch.asmx";
		public static string SynchUrlDev="http://localhost:2923/SheetsSynch.asmx";
		
		public static void IgnoreCertificateErrors() {
			ServicePointManager.ServerCertificateValidationCallback+=
			delegate (object sender,X509Certificate certificate,X509Chain chain,System.Net.Security.SslPolicyErrors sslPolicyErrors) {
				return true;//accept any certificate if debugging
			};
		}
		
		public static void LoadImagesToSheetDef(SheetDef sheetDefCur) {
			for(int j=0;j<sheetDefCur.SheetFieldDefs.Count;j++) {
				try {
					if(sheetDefCur.SheetFieldDefs[j].FieldType==SheetFieldType.Image) {
						string filePath=SheetUtil.GetImagePath();
						string fileName=sheetDefCur.SheetFieldDefs[j].FieldName;
						string filePathAndName=ODFileUtils.CombinePaths(filePath,fileName);
						Image img=null;
						ImageFormat imgFormat=null;
						if(sheetDefCur.SheetFieldDefs[j].ImageField!=null) {//The image has already been downloaded.
							img=new Bitmap(sheetDefCur.SheetFieldDefs[j].ImageField);
							imgFormat=ImageFormat.Bmp;
						}
						else if(sheetDefCur.SheetFieldDefs[j].FieldName=="Patient Info.gif") {
							img=OpenDentBusiness.Properties.Resources.Patient_Info;
							imgFormat=img.RawFormat;
						}
						else if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && File.Exists(filePathAndName)) {
							img=Image.FromFile(filePathAndName);
							imgFormat=img.RawFormat;
						}
						else if(CloudStorage.IsCloudStorage) {
							OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(filePath,fileName);
							if(state==null || state.FileContent==null) {
								throw new Exception(Lan.g(CloudStorage.LanThis,"Unable to download image."));
							}
							else {
								using(MemoryStream stream=new MemoryStream(state.FileContent)) {
									img=new Bitmap(Image.FromStream(stream));
								}
								imgFormat=ImageFormat.Bmp;
							}
						}
						if(img==null) {//Image is missing
							throw new ODException($"The file {fileName} could not be found in {filePath}");
						}
						//sheetDefCur.SheetFieldDefs[j].ImageData=POut.Bitmap(new Bitmap(img),ImageFormat.Png);//Because that's what we did before. Review this later. 
						long fileByteSize=0;
						using(MemoryStream ms=new MemoryStream()) {
							img.Save(ms,imgFormat); // done solely to compute the file size of the image
							fileByteSize=ms.Length;
						}
						if(fileByteSize>2000000) {
							//for large images greater that ~2MB use jpeg format for compression. Large images in the 4MB + range have difficulty being displayed. It could be an issue with MYSQL or ASP.NET
							sheetDefCur.SheetFieldDefs[j].ImageData=POut.Bitmap(new Bitmap(img),ImageFormat.Jpeg);
						}
						else {
							sheetDefCur.SheetFieldDefs[j].ImageData=POut.Bitmap(new Bitmap(img),imgFormat);
						}
					}
					else {
						sheetDefCur.SheetFieldDefs[j].ImageData="";// because null is not allowed
					}
				}
				catch(Exception ex) {
					sheetDefCur.SheetFieldDefs[j].ImageData="";
					MessageBox.Show(ex.Message);
				}
			}
		}

		///<summary>Returns true if the sheet has a FName, LName, and Birthdate field.</summary>
		public static bool VerifyRequiredFieldsPresent(SheetDef sheetDef) {
			bool hasFName=false;
			bool hasLName=false;
			bool hasBirthdate=false;
			for(int j=0;j<sheetDef.SheetFieldDefs.Count;j++) {
				if(sheetDef.SheetFieldDefs[j].FieldType!=SheetFieldType.InputField) {
					continue;
				}
				if(sheetDef.SheetFieldDefs[j].FieldName.ToLower().In("fname","firstname")) {
					hasFName=true;
				}
				else if(sheetDef.SheetFieldDefs[j].FieldName.ToLower().In("lname","lastname")) {
					hasLName=true;
				}
				else if(sheetDef.SheetFieldDefs[j].FieldName.ToLower().In("bdate","birthdate")) {
					hasBirthdate=true;
				}
			}
			if(!hasFName || !hasLName || !hasBirthdate) {
				MessageBox.Show(Lan.g("WebForms","The sheet called")+" \""+sheetDef.Description+"\" "
					+Lan.g("WebForms","does not contain all three required fields: LName, FName, and Birthdate."));
				return false;
			}
			return true;
		}

		///<summary>Attempts to add or update the passed in sheet def. If the add or update fails because of a request being too large, then the content
		///will be split and sent in pieces. This method returns whether the add or update was successful.</summary>
		///<param name="currentForm">The form calling this method. Used to modify the cursor.</param>
		///<param name="sheetDef">The SheetDef object being addeds or updated.</param>
		///<param name="isNew">Indicates whether the object is new. If this is false, the object will be updated.</param>
		///<param name="listWebFormSheetDefs">A list of webforms correlated to the passed in sheetDef. This should be set when updating a sheetdef.</param>
		public static bool TryAddOrUpdateSheetDef(Form currentForm,SheetDef sheetDef,bool isNew,List<WebForms_SheetDef> listWebFormSheetDefs = null) {
			try {				
				SheetsSynchProxy.TimeoutOverride=300000;//for slow connections more timeout is provided. The  default is 100 seconds i.e 100000
				if(!isNew) {
					if(listWebFormSheetDefs==null) {//This should be set when isNew is not true, but will check here just in case.
						return false;
					}
					foreach(WebForms_SheetDef webSheetDef in listWebFormSheetDefs) {
						WebForms_SheetDefs.UpdateSheetDef(webSheetDef.WebSheetDefID,sheetDef,doCatchExceptions: false);
					}
				}
				else {
					WebForms_SheetDefs.TryUploadSheetDef(sheetDef);
				}
				return true;
			}
			catch(WebException wex) {
				if(((HttpWebResponse)wex.Response).StatusCode==HttpStatusCode.RequestEntityTooLarge) {
					int chunkSize=1024*1024;//Start with 1 MB chunk size if package needs to be broken up in size.
					bool isTooLargeRequest=true;
					while(isTooLargeRequest && chunkSize>=1024) {//loop until successful request transfer or chunk sizes get smaller than 1 KB
						try {
							if(!isNew) {
								foreach(WebForms_SheetDef webSheetDef in listWebFormSheetDefs) {
									WebForms_SheetDefs.UpdateSheetDefChunked(webSheetDef.WebSheetDefID,sheetDef,chunkSize);
								}
							}
							else {
								WebForms_SheetDefs.TryUploadSheetDefChunked(sheetDef,chunkSize);
							}
						}
						catch(WebException wex2) {
							if(((HttpWebResponse)wex2.Response).StatusCode==HttpStatusCode.RequestEntityTooLarge) {
								chunkSize/=2; //Chunksize is divided by two each iteration after the first
								continue; //if still too large, continue to while loop
							}
							else {
								FriendlyException.Show(wex.Message,wex);
								return false;
							}
						}
						catch(Exception ex) {
							currentForm.Cursor=Cursors.Default;
							FriendlyException.Show(ex.Message,ex);
							return false;
						}
						isTooLargeRequest=false;
					}
					if(isTooLargeRequest) {//If breaking from loop due to chunksize getting too small, send error message
						currentForm.Cursor=Cursors.Default;
						MsgBox.Show("The sheet you are trying to upload failed after attempts were made to resize the request.");
						return false;
					}
					else {
						return true;
					}
				}
				else {
					FriendlyException.Show(wex.Message,wex);
					return false;
				}
			}
			catch(Exception ex) {
				currentForm.Cursor=Cursors.Default;
				FriendlyException.Show(ex.Message,ex);
				return false;
			}
		}

		///<summary>Returns false if the user wants to cancel out of the retrieval process. Returns true if the retrieval process completed.
		///The lists of messages will be filled with any messages during the retrieval process regardless if the entire process completed or not.</summary>
		public static bool TryRetrieveAndSaveData(string cultureName,List<long> listClinicNums,int countPerBatch,out List<string> listMsgsCEMT,out List<string> listMsgsWebForms,out List<string>listMsgsDownloadedWebForms) {
			listMsgsCEMT=new List<string>();
			listMsgsWebForms=new List<string>();
			listMsgsDownloadedWebForms=new List<string>();
			if(!TryRetrievePatientTransfersCEMT(ref listMsgsCEMT)) {
				return false;//User wants to cancel out of the CEMT patient transfer importing process.
			}
			if(!TryRetrieveWebForms(cultureName,listClinicNums,countPerBatch,ref listMsgsWebForms)) {
				return false;//There was an error or the user wants to cancel out of the web forms retrieval process.
			}
			if(!TryRetrieveUnmatchedWebForms(listClinicNums,ref listMsgsDownloadedWebForms)) {
				return false;
			}
			return true;
		}

		///<summary>Returns false if the user wants to cancel out of the retrieval process. Returns true if the retrieval process completed.
		///The listErrors parameter will be filled with any messages from the import portion of the retrieval process regardless if the entire process completed or not.</summary>
		public static bool TryRetrievePatientTransfersCEMT(ref List<string> listMsgs) {
			//Now get the patients that were transferred from the CEMT tool. These sheets will have a PatNum=0;
			List<Sheet> listSheetsFromCemtTool=Sheets.GetTransferSheets();
			List<long> listCemtSheetsToDelete=new List<long>();
			for(int i=0;i<listSheetsFromCemtTool.Count;i++) {
				//Continue if the SheetNum is either marked as skipped or is going to be deleted. 
				if(listCemtSheetsToDelete.Contains(listSheetsFromCemtTool[i].SheetNum)) {
					continue;
				}
				try {
					if(!DidImportSheet(listSheetsFromCemtTool[i],listSheetsFromCemtTool,ref listCemtSheetsToDelete)) {
						listMsgs.Add(Lan.g("FormWebForms","User manually cancelled CEMT Patient Transfer importing."));
						return false;//User wants to cancel import.
					}
				}
				catch(Exception e) {
					listMsgs.Add(e.Message);
				}
			}
			for(int i=0;i<listCemtSheetsToDelete.Count;i++) {
				Sheets.Delete(listCemtSheetsToDelete[i]);//Does not delete the sheet but sets the Sheet.IsDeleted=true.
			}
			return true;
		}

		///<summary>Returns false if there was an error during validation or if the user wants to cancel out of the retrieval process. Returns true if the retrieval process completed.
		///The listErrors parameter will be filled with any error messages from the import portion of the retrieval process regardless if the entire process completed or not.</summary>
		public static bool TryRetrieveWebForms(string cultureName,List<long> listClinicNums,int countPerBatch,ref List<string> listMsgs) {
			if(countPerBatch<=0) {
				listMsgs.Add(Lan.g("FormWebForms","Invalid batch size."));
				return false;
			}
			if(WebUtils.GetDentalOfficeID()==0) {
				listMsgs.Add(Lan.g("FormWebForms","Either the registration key provided by the dental office is incorrect or the Host Server Address cannot be found."));
				return false;
			}
			SheetsSynchProxy.TimeoutOverride=300000;//5 minutes.  Default is 100000 (1.66667 minutes).
			List<long> listSheetIDs;
			try {
				//Get the register of all pending web form SheetIDs for the selected clinics.
				listSheetIDs=WebForms_Sheets.GetSheetIDs(listClinicNums:listClinicNums);
			}
			catch(Exception ex) {
				listMsgs.Add(Lan.g("FormWebForms","There was a problem downloading the register of pending web forms:")+" "+ex.Message);
				return false;
			}
			if(listSheetIDs.IsNullOrEmpty()) {
				listMsgs.Add(Lan.g("FormWebForms","No pending Web Forms."));
				return true;//Nothing to retrieve.
			}
			//Figure out the total number of batches that will be required in order to download all pending web forms.
			int countTotalBatches=(listSheetIDs.Count + countPerBatch - 1) / countPerBatch;
			//Keep track of patient import choices throughout the entire retrieval process.
			PatientImportChoice patientImportChoice=new PatientImportChoice();
			//Download all of the pending web forms one batch at a time.
			for(int i=0;i<countTotalBatches;i++) {
				//Grab the SheetIDs for this specific batch.
				List<long> listSheetIDsForBatch=listSheetIDs.Skip(i * countPerBatch)
					.Take(countPerBatch)
					.ToList();
				//Make a web call to procure the web forms from HQ.
				List<WebForms_Sheet> listWebFormSheets;
				try {
					listWebFormSheets=WebForms_Sheets.GetSheets(listSheetIDs:listSheetIDsForBatch,listClinicNums:listClinicNums);
				}
				catch(Exception ex) {
					listMsgs.Add(Lan.g("FormWebForms","There was a problem downloading pending web forms for batch")+$" {(i + 1)} / {countTotalBatches}");
					listMsgs.Add("  ^"+Lan.g("FormWebForms","SheetIDs in batch:")+" "+string.Join(", ",listSheetIDsForBatch));
					listMsgs.Add("  ^"+Lan.g("FormWebForms","Error message:")+" "+ex.Message);
					continue;//Attempt to download subsequent batches just in case there is a specific problem with a specific batch.
				}
				//Process the web forms and then tell HQ to delete all of the web forms that have been successfully retrieved.
				List<long> listSheetIdsForDeletion=new List<long>();
				for(int k=0;k<listWebFormSheets.Count;k++) {
					if(listSheetIdsForDeletion.Contains(listWebFormSheets[k].SheetID)) {
						continue;//Marked for deletion already. Next.
					}
					try { //This try catch is put so that a defective downloaded sheet does not stop other sheets from being downloaded.
						if(!DidImportSheet(listWebFormSheets[k],null,listWebFormSheets,null,cultureName,ref listSheetIdsForDeletion,ref patientImportChoice)) {
							listMsgs.Add(Lan.g("FormWebForms","User manually cancelled Web Form importing."));
							return false;//User wants to cancel out of the importing process.
						}
					}
					catch(Exception e) {
						listMsgs.Add(e.Message);
					}
				}
				if(listSheetIdsForDeletion.Count > 0) {
					if(!WebForms_Sheets.DeleteSheetData(listSheetIdsForDeletion.Distinct().ToList())) {
						listMsgs.Add(Lan.g("FormWebForms","There was a problem telling HQ that the web forms were retrieved."));
						listMsgs.Add("  ^"+Lan.g("FormWebForms","The following web forms will be downloaded again the next time forms are retrieved."));
						listMsgs.Add("  ^"+Lan.g("FormWebForms","SheetIDs:")+" "+string.Join(", ",listSheetIdsForDeletion));
						return false;//This is a critical problem that the user needs to be made aware of due to the fact that it will cause duplicate web forms to be downloaded.
					}
				}
			}
			return true;
		}

		///<summary>Gets the web forms that have already been automatically downloaded by the Open Dental Service.</summary>
		private static bool TryRetrieveUnmatchedWebForms(List<long> listClinicNums,ref List<string> listMsgs) {
			if(!PrefC.GetBool(PrefName.WebFormsDownloadAutomcatically)) {
				return true;//There shouldn't be any automatically downloaded web forms.
			}
			List<Sheet> listSheetsDownloaded=Sheets.GetUnmatchedWebFormSheets(listClinicNums);
			List<long> listSheetIDsToDelete=new List<long>();
			for(int i=0;i<listSheetsDownloaded.Count;i++) {
				//Continue if the SheetNum is either marked as skipped or is going to be deleted. 
				if(listSheetIDsToDelete.Contains(listSheetsDownloaded[i].SheetNum)) {
					continue;
				}
				try {
					if(!DidImportSheet(listSheetsDownloaded[i],listSheetsDownloaded,ref listSheetIDsToDelete)) {
						listMsgs.Add(Lan.g("FormWebForms","User manually cancelled unmatched patient web forms importing."));
						return false;//User wants to cancel import.
					}
				}
				catch(Exception e) {
					listMsgs.Add(e.Message);
				}
			}
			for(int i=0;i<listSheetIDsToDelete.Count;i++) {
				Sheets.Delete(listSheetIDsToDelete[i]);//Does not delete the sheet but sets the Sheet.IsDeleted=true.
			}
			return true;
		}

		private static bool DidImportSheet(Sheet sheet,List<Sheet> listSheets,ref List<long> listSheetIdsForDeletion) {
			PatientImportChoice patientImportChoice=new PatientImportChoice();
			return DidImportSheet(null,sheet,null,listSheets,CultureInfo.CurrentCulture.Name,ref listSheetIdsForDeletion,ref patientImportChoice);
		}

		///<summary>Returns true if the import was successful. Imports either a webform or a sheet that was transferred using the CEMT tool. 
		///Tries to find a matching patient using LName,FName,and DOB. If no matching patient is found automatically, it will allow the user to either 
		///create a new patient, select an existing patient,delete, or skip the sheet. Call using a try/catch.</summary>
		private static bool DidImportSheet(WebForms_Sheet webFormsSheet,Sheet sheet,List<WebForms_Sheet> listWebSheets,List<Sheet> listSheets,string cultureName,
			ref List<long> listSheetIdsForDeletion,ref PatientImportChoice patientImportChoice) 
		{
			bool isWebForms=webFormsSheet!=null && listWebSheets!=null;
			long patNum=0;
			string lName="";
			string fName="";
			List<string> listPhoneNumbers=new List<string>();
			string email="";
			DateTime bDate=DateTime.MinValue;
			if(isWebForms) {
				WebForms_Sheets.ParseWebFormSheet(webFormsSheet,cultureName,out lName,out fName,out bDate,out listPhoneNumbers,out email);
			}
			else {
				Sheets.ParseTransferSheet(sheet,out lName,out fName,out bDate,out listPhoneNumbers,out email);
			}
			PatientImportChoice.ChosenAction lastActionChosen=null;
			if(patientImportChoice!=null) {
				// Since only name and birthday are copied to subsequent forms,
				// those are the only fields we expect to all be the same for a list of forms retrieved all at once
				lastActionChosen=patientImportChoice.GetChoiceForPatient(lName,fName,bDate);
			}
			List<long> listMatchingPats=Patients.GetPatNumsByNameBirthdayEmailAndPhone(lName,fName,bDate,email,listPhoneNumbers);
			Patient pat=null;
			if(listMatchingPats.IsNullOrEmpty() || listMatchingPats.Count > 1) {//0 or > 1
				List<long> listWebSheetNumsForPat=new List<long>();
				if(isWebForms) {
					List<long> listSheetsToDelete=listSheetIdsForDeletion;
					listWebSheetNumsForPat=WebForms_Sheets.FindSheetsForPat(webFormsSheet,listWebSheets,cultureName)
						//Only include web sheets that have not already been imported
						.Where(x => !listSheetsToDelete.Contains(x)).ToList();
				}
				else {//Cemt Import
					listWebSheetNumsForPat=Sheets.FindSheetsForPat(sheet,listSheets);
				}
				// Only prompt the user if this is a new name/birthday combo
				if(lastActionChosen==null) {
					using FormPatientPickWebForm formPatientPickWebForm=new FormPatientPickWebForm(webFormsSheet,listWebSheetNumsForPat.Count,sheet);
					formPatientPickWebForm.LnameEntered=lName;
					formPatientPickWebForm.FnameEntered=fName;
					formPatientPickWebForm.DateBirthEntered=bDate;
					formPatientPickWebForm.HasMoreThanOneMatch=(listMatchingPats.Count > 1);
					formPatientPickWebForm.ShowDialog();
					if(formPatientPickWebForm.DialogResult==DialogResult.Cancel) {
						if(isWebForms) {
							//user wants to stop importing altogether
							//we will pick up where we left off here next time
							WebForms_Sheets.DeleteSheetData(listSheetIdsForDeletion.Distinct().ToList());
						}
						return false;// only false when user wants to stop importing
					}
					else if(formPatientPickWebForm.DialogResult==DialogResult.Ignore) {
						if(formPatientPickWebForm.IsDiscardAll()) {
							//user wants to delete all webforms for this patient. Mark them for deletion.
							listSheetIdsForDeletion.AddRange(listWebSheetNumsForPat);
						}
						// The user chose Skip
						else {
							// Store the choice so next time we just skip on this name/birthday combo
							if(patientImportChoice!=null) {
								patientImportChoice.SetSkipActionForPatient(lName,fName,bDate);
							}
						}
						MakeEServiceLog(isWebForms,patNum,0);//We know neither patNum nor sheetNum, but let's at least log that somebody filled out some sheet.
						return true;//continue on to the next one
					}
					patNum=formPatientPickWebForm.PatNumSelected;//might be zero to indicate new patient
					// If not zero, the user selected an existing patient, so store it and use it the next time this name/birthday combo appears.
					if((patientImportChoice!=null) && (patNum!=0)) {
						patientImportChoice.SetPatNumForPatient(lName,fName,bDate,patNum);
					}
				}
				else if(lastActionChosen.IsSkip) {
					// User chose "Skip" last time this name/birthday combo appeared
					MakeEServiceLog(isWebForms,patNum,0);//We know neither patNum nor sheetNum, but let's at least log that somebody filled out some sheet.
					return true;
				}
				else {
					// Last time, for this name/birthday combo, the user selected an existing patient; use that patNum
					patNum=lastActionChosen.PatNum;
				}
			}
			else {//Exactly one match was found so make a log entry what the match was.
				patNum=listMatchingPats[0];
				pat=Patients.GetPat(patNum);
				//Security log for OD automatically importing a sheet into a patient.
				string logText;
				if(isWebForms) {
					logText=Lan.g("FormWebForms","Web form import from:");
				}
				else {
					logText=Lan.g("FormWebForms","CEMT patient transfer import from:");
				}
				logText+=" "+lName+", "+fName+" "+bDate.ToShortDateString()+"\r\n"
					+Lan.g("FormWebForms","Auto imported into:")+" "+pat.LName+", "+pat.FName+" "+pat.Birthdate.ToShortDateString();
				SecurityLogs.MakeLogEntry(Permissions.SheetEdit,patNum,logText);
			}
			if(patNum==0) {
				pat=CreatePatient(lName,fName,bDate,webFormsSheet,sheet,cultureName);
				patNum=pat.PatNum;
				//Security log for user creating a new patient.
				string logText;
				if(isWebForms) {
					logText=Lan.g("FormWebForms","Web form import from:");
				}
				else {
					logText=Lan.g("FormWebForms","CEMT patient transfer import from:");
				}
				logText+=" "+lName+", "+fName+" "+bDate.ToShortDateString()+"\r\n"
					+Lan.g("FormWebForms","User created new pat:")+" "+pat.LName+", "+pat.FName+" "+pat.Birthdate.ToShortDateString();
				SecurityLogs.MakeLogEntry(Permissions.SheetEdit,patNum,logText);
			}
			else if(pat==null) {
				pat=Patients.GetPat(patNum);
			}
			//We should probably make a security log entry for a manually selected patient.
			if(isWebForms) {
				Sheet newSheet=SheetUtil.CreateSheetFromWebSheet(patNum,webFormsSheet);
				Sheets.SaveNewSheet(newSheet);
				if(DataExistsInDb(newSheet)) {
					listSheetIdsForDeletion.Add(webFormsSheet.SheetID);
				}
				MakeEServiceLog(isWebForms,patNum,newSheet.SheetNum);
			}
			else {//CEMT Patient Transfer
				//Sheet is ready to get updated with the patient.
				sheet.PatNum=patNum;
				sheet.DateTimeSheet=MiscData.GetNowDateTime();
				if(PrefC.HasClinicsEnabled) {
					sheet.ClinicNum=pat.ClinicNum;
				}
				sheet.IsWebForm=true;//This is so the sheet shows up in gridmain in this form. 
				Sheets.Update(sheet);
			}
			return true;
		}

		///<summary></summary>
		private static void MakeEServiceLog(bool isWebForms,long patNum, long sheetNum) {
			if(isWebForms) {
				long clinicNum=Patients.GetLimForPats(new List<long>() { patNum },true).FirstOrDefault()?.ClinicNum??0;
				EServiceLogs.MakeLogEntry(eServiceAction.WFCompletedForm,eServiceType.WebForms,FKeyType.SheetNum,patNum:patNum,FKey:sheetNum,clinicNum:clinicNum);
			}
		}

		/// <summary>
		private static Patient CreatePatient(String LastName,String FirstName,DateTime birthDate,WebForms_Sheet webFormSheet,Sheet sheet, string cultureName) {
			bool isWebForm=webFormSheet!=null;
			Patient newPat=new Patient();
			newPat.LName=LastName;
			newPat.FName=FirstName;
			newPat.Birthdate=birthDate;
			if(isWebForm) {
				newPat.ClinicNum=webFormSheet.ClinicNum;
			}
			else {
				newPat.ClinicNum=sheet.ClinicNum;
			}
			newPat.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,newPat.ClinicNum));
			if(!PrefC.HasClinicsEnabled) {
				//Set the patients primary provider to the practice default provider.
				newPat.PriProv=Providers.GetDefaultProvider().ProvNum;
			}
			else {//Using clinics.
						//Set the patients primary provider to the clinic default provider.
				newPat.PriProv=Providers.GetDefaultProvider(Clinics.ClinicNum).ProvNum;
			}
			Type t=newPat.GetType();
			FieldInfo[] fi=t.GetFields();
			foreach(FieldInfo field in fi) {
				// find match for fields in Patients in the SheetFields
				if(isWebForm) {
					List<WebForms_SheetField> listWebFormsSheetFields=webFormSheet.SheetFields.FindAll(x=>x.FieldName.ToLower()==field.Name.ToLower());
					if(listWebFormsSheetFields.Count()>0) {
						// this loop is used to fill a field that may generate mutiple values for a single field in the patient.
						//for example the field gender has 2 eqivalent sheet fields in the SheetFields
						foreach(WebForms_SheetField webFormsSheetField in listWebFormsSheetFields) {
							FillPatientFields(newPat,field,webFormsSheetField.FieldValue,webFormsSheetField.RadioButtonValue,cultureName,isWebForm,false);
						}
					}
				}
				else {
					List<SheetField> listSheetFields=sheet.SheetFields.FindAll(x=>x.FieldName.ToLower()==field.Name.ToLower());
					if(listSheetFields.Count()>0) {
						// this loop is used to fill a field that may generate mutiple values for a single field in the patient.
						//for example the field gender has 2 eqivalent sheet fields in the SheetFields
						foreach(SheetField sheetField in listSheetFields) {
							FillPatientFields(newPat,field,sheetField.FieldValue,sheetField.RadioButtonValue,"",isWebForm,sheet.IsCemtTransfer);
						}
					}
				}
			}
			try {
				Patients.Insert(newPat,false);
				SecurityLogs.MakeLogEntry(Permissions.PatientCreate,newPat.PatNum,isWebForm ? "Created from Web Forms." : "Created from CEMT transfer.");
				//set Guarantor field the same as PatNum
				Patient patOld=newPat.Copy();
				newPat.Guarantor=newPat.PatNum;
				Patients.Update(newPat,patOld);
				//If there is an existing HL7 def enabled, send an ADT message if there is an outbound ADT message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					MessageHL7 messageHL7=MessageConstructor.GenerateADT(newPat,newPat,EventTypeHL7.A04);//patient is guarantor
					//Will be null if there is no outbound ADT message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=0;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=newPat.PatNum;
						HL7Msgs.Insert(hl7Msg);
						if(ODBuild.IsDebug()) {
							MessageBox.Show("FormWebForms",messageHL7.ToString());
						}
					}
				}
				if(HieClinics.IsEnabled()) {
					HieQueues.Insert(new HieQueue(newPat.PatNum));
				}
			}
			catch(Exception e) {
				MessageBox.Show(e.Message);
			}
			return newPat;
		}

		/// <summary>Compares values of the sheet with values that have been inserted into the db.  Returns false if the data was not saved properly.</summary>
		private static bool DataExistsInDb(Sheet sheet) {
			bool dataExistsInDb=true;
			if(sheet!=null) {
				long SheetNum=sheet.SheetNum;
				Sheet sheetFromDb=Sheets.GetSheet(SheetNum);
				if(sheetFromDb!=null) {
					dataExistsInDb=CompareSheets(sheetFromDb,sheet);
				}
			}
			return dataExistsInDb;
		}

		///<summary>This is not a generic sheet comparer.  It is actually a web form sheet field comparer.  Returns false if any sheet fields that the web form cares about are not equal.</summary>
		private static bool CompareSheets(Sheet sheetFromDb,Sheet newSheet) {
			//the 2 sheets are sorted before comparison because in some cases SheetFields[i] refers to a different field in sheetFromDb than in newSheet
			Sheet sortedSheetFromDb=new Sheet();
			Sheet sortedNewSheet=new Sheet();
			sortedSheetFromDb.SheetFields=sheetFromDb.SheetFields.OrderBy(sf => sf.SheetFieldNum).ToList();
			sortedNewSheet.SheetFields=newSheet.SheetFields.OrderBy(sf => sf.SheetFieldNum).ToList();
			for(int i = 0;i<sortedSheetFromDb.SheetFields.Count;i++) {
				//Explicitly compare the sheet field values that can be imported via web forms.
				//This makes it so that any future columns added to the sheetfield table will not affect this comparer.
				//When new columns are added, we can now decide on a per column basis if the column matters for comparisons.
				//We will always add new columns below and simply comment them out if we do not want to use them for comparison, this way we know that the column was considered.
				if(sortedSheetFromDb.SheetFields[i].SheetNum!=sortedNewSheet.SheetFields[i].SheetNum
					|| sortedSheetFromDb.SheetFields[i].FieldType!=sortedNewSheet.SheetFields[i].FieldType
					|| sortedSheetFromDb.SheetFields[i].FieldName!=sortedNewSheet.SheetFields[i].FieldName
					|| sortedSheetFromDb.SheetFields[i].FieldValue!=sortedNewSheet.SheetFields[i].FieldValue
					|| sortedSheetFromDb.SheetFields[i].FontSize!=sortedNewSheet.SheetFields[i].FontSize
					|| sortedSheetFromDb.SheetFields[i].FontName!=sortedNewSheet.SheetFields[i].FontName
					|| sortedSheetFromDb.SheetFields[i].FontIsBold!=sortedNewSheet.SheetFields[i].FontIsBold
					|| sortedSheetFromDb.SheetFields[i].XPos!=sortedNewSheet.SheetFields[i].XPos
					|| sortedSheetFromDb.SheetFields[i].YPos!=sortedNewSheet.SheetFields[i].YPos
					|| sortedSheetFromDb.SheetFields[i].Width!=sortedNewSheet.SheetFields[i].Width
					|| sortedSheetFromDb.SheetFields[i].Height!=sortedNewSheet.SheetFields[i].Height
					|| sortedSheetFromDb.SheetFields[i].GrowthBehavior!=sortedNewSheet.SheetFields[i].GrowthBehavior
					|| sortedSheetFromDb.SheetFields[i].RadioButtonValue!=sortedNewSheet.SheetFields[i].RadioButtonValue
					|| sortedSheetFromDb.SheetFields[i].RadioButtonGroup!=sortedNewSheet.SheetFields[i].RadioButtonGroup
					|| sortedSheetFromDb.SheetFields[i].IsRequired!=sortedNewSheet.SheetFields[i].IsRequired
					|| sortedSheetFromDb.SheetFields[i].TabOrder!=sortedNewSheet.SheetFields[i].TabOrder
					|| sortedSheetFromDb.SheetFields[i].ReportableName!=sortedNewSheet.SheetFields[i].ReportableName
					|| sortedSheetFromDb.SheetFields[i].TextAlign!=sortedNewSheet.SheetFields[i].TextAlign
					|| sortedSheetFromDb.SheetFields[i].ItemColor!=sortedNewSheet.SheetFields[i].ItemColor
					|| sortedSheetFromDb.SheetFields[i].UiLabelMobile!=sortedNewSheet.SheetFields[i].UiLabelMobile
					|| sortedSheetFromDb.SheetFields[i].UiLabelMobileRadioButton!=sortedNewSheet.SheetFields[i].UiLabelMobileRadioButton
					) {
					return false;//No need to keep looping, we know the sheets are not equal at this point.
				}
			}
			return true;//All web form sheet fields are equal.
		}

		/// <summary>
		private static void FillPatientFields(Patient newPat,FieldInfo field,string SheetWebFieldValue,string RadioButtonValue,string cultureName,
			bool isWebForm,bool isCemtTransfer) 
		{
			try {
				switch(field.Name) {
					case "Birthdate":
						field.SetValue(newPat,SheetFields.GetBirthDate(SheetWebFieldValue,isWebForm,isCemtTransfer,cultureName:cultureName));
						break;
					case "Gender":
						if(RadioButtonValue=="Male") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,PatientGender.Male);
							}
						}
						if(RadioButtonValue=="Female") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,PatientGender.Female);
							}
						}
						break;
					case "Position":
						if(RadioButtonValue=="Married") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,PatientPosition.Married);
							}
						}
						if(RadioButtonValue=="Single") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,PatientPosition.Single);
							}
						}
						break;
					case "PreferContactMethod":
					case "PreferConfirmMethod":
					case "PreferRecallMethod":
						ContactMethod method;
						if(Enum.TryParse(SheetWebFieldValue,out method)) {
							field.SetValue(newPat,method);
						}
						if(RadioButtonValue=="HmPhone" && SheetWebFieldValue=="X") {
							field.SetValue(newPat,ContactMethod.HmPhone);
						}
						if(RadioButtonValue=="WkPhone" && SheetWebFieldValue=="X") {
							field.SetValue(newPat,ContactMethod.WkPhone);
						}
						if(RadioButtonValue=="WirelessPh" && SheetWebFieldValue=="X") {
							field.SetValue(newPat,ContactMethod.WirelessPh);
						}
						if(RadioButtonValue=="Email" && SheetWebFieldValue=="X") {
							field.SetValue(newPat,ContactMethod.Email);
						}
						break;
					case "StudentStatus":
						if(RadioButtonValue=="Nonstudent") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,"");
							}
						}
						if(RadioButtonValue=="Fulltime") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,"F");
							}
						}
						if(RadioButtonValue=="Parttime") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,"P");
							}
						}
						break;
					case "ins1Relat":
					case "ins2Relat":
						if(RadioButtonValue=="Self") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,Relat.Self);
							}
						}
						if(RadioButtonValue=="Spouse") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,Relat.Spouse);
							}
						}
						if(RadioButtonValue=="Child") {
							if(SheetWebFieldValue=="X") {
								field.SetValue(newPat,Relat.Child);
							}
						}
						break;
					default:
						field.SetValue(newPat,SheetWebFieldValue);
						break;
				}//switch case
			}
			catch(Exception e) {
				MessageBox.Show(field.Name+e.Message);
			}
		}

		/// <summary> Helper class to determine which choice a user made for each patient when importing web forms.</summary>
		private class PatientImportChoice {
			///<summary>Stores the action that the user chose for a specific patient when retrieving web forms. Key is a combination of lName, fName, and bDate.</summary>
			private Dictionary<string, ChosenAction> _dictPatientChoices;

			public PatientImportChoice() {
				_dictPatientChoices=new Dictionary<string,ChosenAction>();
			}

			private string GetKeyForPatient(string lName,string fName,DateTime bDate) {
				return lName+fName+bDate.ToString("MMddyyyy");
			}

			/// <summary> Store the choice that a user made for some particular patient data in a web form </summary>
			public void SetSkipActionForPatient(string lName,string fName,DateTime bDate) {
				string patientData=GetKeyForPatient(lName,fName,bDate);
				_dictPatientChoices[patientData]=new ChosenAction(isSkip:true);
			}

			/// <summary> Store the choice that a user made for some particular patient data in a web form </summary>
			public void SetPatNumForPatient(string lName,string fName,DateTime bDate,long patNum) {
				string patientData=GetKeyForPatient(lName,fName,bDate);
				_dictPatientChoices[patientData]=new ChosenAction(isSkip:false,patNum:patNum);
			}

			///<summary>Retrieve which action the user previously took for this same patient data</summary>
			public ChosenAction GetChoiceForPatient(string lName,string fName,DateTime bDate) {
				string patientData=GetKeyForPatient(lName,fName,bDate);
				if(_dictPatientChoices.ContainsKey(patientData)) {
					return _dictPatientChoices[patientData];
				}
				else {
					return null;
				}
			}

			/// <summary> Stores an action a user took when importing a web form for a particular patient.
			/// IsSkip==true means to skip that patient. If IsSkip==false, PatNum is the patient to associate with the current form. </summary>
			public class ChosenAction {
				public long PatNum;
				public bool IsSkip;

				public ChosenAction(bool isSkip,long patNum=0) {
					IsSkip=isSkip;
					PatNum=patNum;
				}
			}
		}
	}

}
