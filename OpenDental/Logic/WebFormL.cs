using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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

namespace OpenDental {
	public class WebFormL {
		public static string SynchUrlStaging="https://10.10.1.196/WebHostSynch/SheetsSynch.asmx";
		public static string SynchUrlDev="http://localhost:2923/SheetsSynch.asmx";
		
		public static void IgnoreCertificateErrors() {
			ServicePointManager.ServerCertificateValidationCallback+=
			delegate (object sender,X509Certificate x509Certificate,X509Chain x509Chain,System.Net.Security.SslPolicyErrors sslPolicyErrors) {
				return true;//accept any certificate if debugging
			};
		}
		
		public static void LoadImagesToSheetDef(SheetDef sheetDef) {
			for(int j=0;j<sheetDef.SheetFieldDefs.Count;j++) {
				if(sheetDef.SheetFieldDefs[j].FieldType!=SheetFieldType.Image) {
					sheetDef.SheetFieldDefs[j].ImageData="";// because null is not allowed
					continue;
				}
				string filePath=null;
				try{
					filePath=SheetUtil.GetImagePath();
				}
				catch(Exception ex) {
					sheetDef.SheetFieldDefs[j].ImageData="";
					MessageBox.Show(ex.Message);
					return;
				}
				string fileName=sheetDef.SheetFieldDefs[j].FieldName;
				string filePathAndName=ODFileUtils.CombinePaths(filePath,fileName);
				Image image=null;//disposed
				ImageFormat imageFormat=null;
				if(sheetDef.SheetFieldDefs[j].ImageField!=null) {//The image has already been downloaded.
					image=new Bitmap(sheetDef.SheetFieldDefs[j].ImageField);
					imageFormat=ImageFormat.Bmp;
				}
				else if(sheetDef.SheetFieldDefs[j].FieldName=="Patient Info.gif") {
					image=OpenDentBusiness.Properties.Resources.Patient_Info;
					imageFormat=image.RawFormat;
				}
				else if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && File.Exists(filePathAndName)) {
					try{
						image=Image.FromFile(filePathAndName);
					}
					catch(Exception ex) {
						sheetDef.SheetFieldDefs[j].ImageData="";
						MessageBox.Show(ex.Message);
						return;
					}
					imageFormat=image.RawFormat;
				}
				else if(CloudStorage.IsCloudStorage) {
					OpenDentalCloud.Core.TaskStateDownload taskStateDownload=CloudStorage.Download(filePath,fileName);
					if(taskStateDownload==null || taskStateDownload.FileContent==null) {
						sheetDef.SheetFieldDefs[j].ImageData="";
						MessageBox.Show(Lan.g(CloudStorage.LanThis,"Unable to download image."));
						return;
					}
					else {
						using MemoryStream memoryStream=new MemoryStream(taskStateDownload.FileContent);
						image=new Bitmap(Image.FromStream(memoryStream));
						imageFormat=ImageFormat.Bmp;
					}
				}
				if(image==null) {//Image is missing
					sheetDef.SheetFieldDefs[j].ImageData="";
					MessageBox.Show($"The file {fileName} could not be found in {filePath}");
					return;
				}
				//sheetDefCur.SheetFieldDefs[j].ImageData=POut.Bitmap(new Bitmap(img),ImageFormat.Png);//Because that's what we did before. Review this later. 
				long lengthFileBytes=0;
				using MemoryStream memoryStreamFileSize=new MemoryStream();
				image.Save(memoryStreamFileSize,imageFormat); // done solely to compute the file size of the image
				lengthFileBytes=memoryStreamFileSize.Length;
				if(lengthFileBytes>2000000) {
					//for large images greater that ~2MB use jpeg format for compression. Large images in the 4MB + range have difficulty being displayed. It could b a issue with MYSQL or ASP.NET
					sheetDef.SheetFieldDefs[j].ImageData=POut.Bitmap((Bitmap)image,ImageFormat.Jpeg);
				}
				else {
					sheetDef.SheetFieldDefs[j].ImageData=POut.Bitmap((Bitmap)image,imageFormat);
				}
				image.Dispose();
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
				if(isNew) {
					WebForms_SheetDefs.TryUploadSheetDef(sheetDef);
				}
				else {
					if(listWebFormSheetDefs==null) {//This should be set when isNew is not true, but will check here just in case.
						return false;
					}
					for(int i=0;i<listWebFormSheetDefs.Count;i++){
						WebForms_SheetDefs.UpdateSheetDef(listWebFormSheetDefs[i].WebSheetDefID,sheetDef,doCatchExceptions:false);
					}
				}
				return true;
			}
			catch(WebException webEx) {
				if(((HttpWebResponse)webEx.Response).StatusCode!=HttpStatusCode.RequestEntityTooLarge) {
					FriendlyException.Show(webEx.Message,webEx);
					return false;
				}
				int chunkSize=1024*1024;//Start with 1 MB chunk size if package needs to be broken up in size.
				bool isTooLargeRequest=true;
				while(isTooLargeRequest && chunkSize>=1024) {//loop until successful request transfer or chunk sizes get smaller than 1 KB
					try {
						if(isNew) {
							WebForms_SheetDefs.TryUploadSheetDefChunked(sheetDef,chunkSize);
						}
						else {
							for(int i=0;i<listWebFormSheetDefs.Count;i++){
								WebForms_SheetDefs.UpdateSheetDefChunked(listWebFormSheetDefs[i].WebSheetDefID,sheetDef,chunkSize);
							}
						}
					}
					catch(WebException webEx2) {
						if(((HttpWebResponse)webEx2.Response).StatusCode==HttpStatusCode.RequestEntityTooLarge) {
							chunkSize/=2; //Chunksize is divided by two each iteration after the first
							continue; //if still too large, continue to while loop
						}
						else {
							FriendlyException.Show(webEx.Message,webEx);
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
			catch(Exception ex) {
				currentForm.Cursor=Cursors.Default;
				FriendlyException.Show(ex.Message,ex);
				return false;
			}
		}

		///<summary>Returns false if the user wants to cancel out of the retrieval process. Returns true if the retrieval process completed.
		///The listErrors parameter will be filled with any messages from the import portion of the retrieval process regardless if the entire process completed or not.</summary>
		public static Result TryRetrievePatientTransfersCEMT() {
			Result result = new Result();
			//Now get the patients that were transferred from the CEMT tool. These sheets will have a PatNum=0;
			List<Sheet> listCemtSheets=Sheets.GetTransferSheets();
			List<long> listCemtSheetsToDelete=new List<long>();
			for(int i=0;i<listCemtSheets.Count;i++) {
				//Continue if the SheetNum is either marked as skipped or is going to be deleted. 
				if(listCemtSheetsToDelete.Contains(listCemtSheets[i].SheetNum)) {
					continue;
				}
				try {
					if(!DidImportSheet(listCemtSheets[i],listCemtSheets,ref listCemtSheetsToDelete)) {
						result.ListMsgs.Add(Lan.g("FormWebForms","User manually cancelled CEMT Patient Transfer importing."));
						result.IsSuccess=false;
						return result;//User wants to cancel import.
					}
				}
				catch(Exception e) {
					result.ListMsgs.Add(e.Message);
				}
			}
			for(int i=0;i<listCemtSheetsToDelete.Count;i++) {
				Sheets.Delete(listCemtSheetsToDelete[i]);//Does not delete the sheet but sets the Sheet.IsDeleted=true.
			}
			result.IsSuccess=true;
			return result;
		}

		///<summary>Returns false if there was an error during validation or if the user wants to cancel out of the retrieval process. Returns true if the retrieval process completed.
		///The listErrors parameter will be filled with any error messages from the import portion of the retrieval process regardless if the entire process completed or not.</summary>
		public static Result TryRetrieveWebForms(string cultureName,List<long> listClinicNums,int countPerBatch) {
			Result result=new Result();
			if(countPerBatch<=0) {
				result.ListMsgs.Add(Lan.g("FormWebForms","Invalid batch size."));
				result.IsSuccess=false;
				return result;
			}
			if(WebUtils.GetDentalOfficeID()==0) {
				result.ListMsgs.Add(Lan.g("FormWebForms","Either the registration key provided by the dental office is incorrect or the Host Server Address cannot be found."));
				result.IsSuccess=false;
				return result;
			}
			SheetsSynchProxy.TimeoutOverride=300000;//5 minutes.  Default is 100000 (1.66667 minutes).
			List<long> listSheetIDs;
			try {
				//Get the register of all pending web form SheetIDs for the selected clinics.
				listSheetIDs=WebForms_Sheets.GetSheetIDs(listClinicNums:listClinicNums);
			}
			catch(Exception ex) {
				result.ListMsgs.Add(Lan.g("FormWebForms","There was a problem downloading the register of pending web forms:")+" "+ex.Message);
				result.IsSuccess=false;
				return result;
			}
			if(listSheetIDs.IsNullOrEmpty()) {
				result.ListMsgs.Add(Lan.g("FormWebForms","No pending Web Forms."));
				result.IsSuccess=true;//Nothing to retrieve.
				return result;
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
					result.ListMsgs.Add(Lan.g("FormWebForms","There was a problem downloading pending web forms for batch")+$" {(i + 1)} / {countTotalBatches}");
					result.ListMsgs.Add("  ^"+Lan.g("FormWebForms","SheetIDs in batch:")+" "+string.Join(", ",listSheetIDsForBatch));
					result.ListMsgs.Add("  ^"+Lan.g("FormWebForms","Error message:")+" "+ex.Message);
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
							result.ListMsgs.Add(Lan.g("FormWebForms","User manually cancelled Web Form importing."));
							result.IsSuccess=false;//User wants to cancel out of the importing process.
							return result;
						}
					}
					catch(Exception e) {
						result.ListMsgs.Add(e.Message);
					}
				}
				if(listSheetIdsForDeletion.Count > 0) {
					if(!WebForms_Sheets.DeleteSheetData(listSheetIdsForDeletion.Distinct().ToList())) {
						result.ListMsgs.Add(Lan.g("FormWebForms","There was a problem telling HQ that the web forms were retrieved."));
						result.ListMsgs.Add("  ^"+Lan.g("FormWebForms","The following web forms will be downloaded again the next time forms are retrieved."));
						result.ListMsgs.Add("  ^"+Lan.g("FormWebForms","SheetIDs:")+" "+string.Join(", ",listSheetIdsForDeletion));
						result.IsSuccess=false;//This is a critical problem that the user needs to be made aware of due to the fact that it will cause duplicate web forms to be downloaded.
						return result;
					}
				}
			}
			result.IsSuccess=true;
			return result;
		}

		///<summary>Gets the web forms that have already been automatically downloaded by the Open Dental Service.</summary>
		public static Result TryRetrieveUnmatchedWebForms(List<long> listClinicNums) {
			Result result = new Result();
			if(!PrefC.GetBool(PrefName.WebFormsDownloadAutomcatically)) {
				result.IsSuccess=true;//There shouldn't be any automatically downloaded web forms.
				return result;
			}
			List<Sheet> listSheetsDownloaded=Sheets.GetUnmatchedWebFormSheets(listClinicNums);
			//Keep track of patient import choices throughout the entire retrieval process.
			PatientImportChoice patientImportChoice=new PatientImportChoice();
			List<long> listSheetIDsToDelete=new List<long>();
			for(int i=0;i<listSheetsDownloaded.Count;i++) {
				//Continue if the SheetNum is either marked as skipped or is going to be deleted. 
				if(listSheetIDsToDelete.Contains(listSheetsDownloaded[i].SheetNum)) {
					continue;
				}
				try {
					if(!DidImportSheet(null,listSheetsDownloaded[i],null,listSheetsDownloaded,CultureInfo.CurrentCulture.Name,ref listSheetIDsToDelete,ref patientImportChoice)) {
						result.ListMsgs.Add(Lan.g("FormWebForms","User manually cancelled unmatched patient web forms importing."));
						result.IsSuccess=false;//User wants to cancel import.
						return result;
					}
				}
				catch(Exception e) {
					result.ListMsgs.Add(e.Message);
				}
			}
			for(int i=0;i<listSheetIDsToDelete.Count;i++) {
				Sheets.Delete(listSheetIDsToDelete[i]);//Does not delete the sheet but sets the Sheet.IsDeleted=true.
			}
			result.IsSuccess=true;
			return result;
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
			Patient patient=null;
			if(listMatchingPats.IsNullOrEmpty() || listMatchingPats.Count > 1) {//0 or > 1
				List<long> listWebSheetNumsForPat=new List<long>();
				if(isWebForms) {
					List<long> listSheetsToDelete=listSheetIdsForDeletion;
					listWebSheetNumsForPat=WebForms_Sheets.FindSheetsForPat(webFormsSheet,listWebSheets,cultureName)
						//Only include web sheets that have not already been imported
						.FindAll(x => !listSheetsToDelete.Contains(x));
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
				patient=Patients.GetPat(patNum);
				//Security log for OD automatically importing a sheet into a patient.
				string logText;
				if(isWebForms) {
					logText=Lan.g("FormWebForms","Web form import from:");
				}
				else {
					logText=Lan.g("FormWebForms","CEMT patient transfer import from:");
				}
				logText+=" "+lName+", "+fName+" "+bDate.ToShortDateString()+"\r\n"
					+Lan.g("FormWebForms","Auto imported into:")+" "+patient.LName+", "+patient.FName+" "+patient.Birthdate.ToShortDateString();
				SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,patNum,logText);
			}
			if(patNum==0) {
				patient=CreatePatient(lName,fName,bDate,webFormsSheet,sheet,cultureName);
				patNum=patient.PatNum;
				//Security log for user creating a new patient.
				string logText;
				if(isWebForms) {
					logText=Lan.g("FormWebForms","Web form import from:");
				}
				else {
					logText=Lan.g("FormWebForms","CEMT patient transfer import from:");
				}
				logText+=" "+lName+", "+fName+" "+bDate.ToShortDateString()+"\r\n"
					+Lan.g("FormWebForms","User created new pat:")+" "+patient.LName+", "+patient.FName+" "+patient.Birthdate.ToShortDateString();
				SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,patNum,logText);
			}
			else if(patient==null) {
				patient=Patients.GetPat(patNum);
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
					sheet.ClinicNum=patient.ClinicNum;
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
			Patient patientNew=new Patient();
			patientNew.LName=LastName;
			patientNew.FName=FirstName;
			patientNew.Birthdate=birthDate;
			if(isWebForm) {
				patientNew.ClinicNum=webFormSheet.ClinicNum;
			}
			else {
				patientNew.ClinicNum=sheet.ClinicNum;
			}
			patientNew.BillingType=PIn.Long(ClinicPrefs.GetPrefValue(PrefName.PracticeDefaultBillType,patientNew.ClinicNum));
			if(!PrefC.HasClinicsEnabled) {
				//Set the patients primary provider to the practice default provider.
				patientNew.PriProv=Providers.GetDefaultProvider().ProvNum;
			}
			else {//Using clinics.
						//Set the patients primary provider to the clinic default provider.
				patientNew.PriProv=Providers.GetDefaultProvider(Clinics.ClinicNum).ProvNum;
			}
			Type type=patientNew.GetType();
			FieldInfo[] fieldInfoArray=type.GetFields();
			for(int i=0;i<fieldInfoArray.Length;i++){
				// find match for fields in Patients in the SheetFields
				if(isWebForm) {
					//Additional check added when we come accross the "State" field for patient, and "State/StateNoValidation" field within the webform.
					List<WebForms_SheetField> listWebFormsSheetFields=webFormSheet.SheetFields.FindAll(x=>x.FieldName.ToLower()==fieldInfoArray[i].Name.ToLower()
						|| (x.FieldName.ToLower().StartsWith("state") && fieldInfoArray[i].Name==nameof(Patient.State)));
					if(listWebFormsSheetFields.Count()>0) {
						// this loop is used to fill a field that may generate multiple values for a single field in the patient.
						//for example the field gender has 2 eqivalent sheet fields in the SheetFields
						for(int j=0;j<listWebFormsSheetFields.Count;j++){
							FillPatientFields(patientNew,fieldInfoArray[i],listWebFormsSheetFields[j].FieldValue,
								listWebFormsSheetFields[j].RadioButtonValue,cultureName,isWebForm,false);
						}
					}
				}
				else {
					//Additional check added when we come accross the "State" field for patient, and "State/StateNoValidation" field within the sheet.
					List<SheetField> listSheetFields=sheet.SheetFields.FindAll(x=>x.FieldName.ToLower()==fieldInfoArray[i].Name.ToLower()
						|| (x.FieldName.ToLower().StartsWith("state") && fieldInfoArray[i].Name==nameof(Patient.State)));
					if(listSheetFields.Count()>0) {
						// this loop is used to fill a field that may generate mutiple values for a single field in the patient.
						//for example the field gender has 2 eqivalent sheet fields in the SheetFields
						for(int j=0;j<listSheetFields.Count;j++){
							FillPatientFields(patientNew,fieldInfoArray[i],listSheetFields[j].FieldValue,
								listSheetFields[j].RadioButtonValue,"",isWebForm,sheet.IsCemtTransfer);
						}
					}
				}
			}
			Patients.Insert(patientNew,false);
			string logText="Created from CEMT transfer.";
			if(isWebForm){
				logText="Created from Web Forms.";
			}
			SecurityLogs.MakeLogEntry(EnumPermType.PatientCreate,patientNew.PatNum,logText);
			//set Guarantor field the same as PatNum
			Patient patientOld=patientNew.Copy();
			patientNew.Guarantor=patientNew.PatNum;
			Patients.Update(patientNew,patientOld);
			//If there is an existing HL7 def enabled, send an ADT message if there is an outbound ADT message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				MessageHL7 messageHl7=MessageConstructor.GenerateADT(patientNew,patientNew,EventTypeHL7.A04);//patient is guarantor
				//Will be null if there is no outbound ADT message defined, so do nothing
				if(messageHl7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=0;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHl7.ToString();
					hl7Msg.PatNum=patientNew.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MessageBox.Show("FormWebForms",messageHl7.ToString());
					}
				}
			}
			if(HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(patientNew.PatNum));
			}
			return patientNew;
		}

		/// <summary>Compares values of the sheet with values that have been inserted into the db.  Returns false if the data was not saved properly.</summary>
		private static bool DataExistsInDb(Sheet sheet) {
			bool dataExistsInDb=true;
			if(sheet!=null) {
				long sheetNum=sheet.SheetNum;
				Sheet sheetFromDb=Sheets.GetSheet(sheetNum);
				if(sheetFromDb!=null) {
					dataExistsInDb=CompareSheets(sheetFromDb,sheet);
				}
			}
			return dataExistsInDb;
		}

		///<summary>This is not a generic sheet comparer.  It is actually a web form sheet field comparer.  Returns false if any sheet fields that the web form cares about are not equal.</summary>
		private static bool CompareSheets(Sheet sheetFromDb,Sheet sheetNew) {
			//the 2 sheets are sorted before comparison because in some cases SheetFields[i] refers to a different field in sheetFromDb than in newSheet
			Sheet sheetFromDbSorted=new Sheet();
			Sheet sheetNewSorted=new Sheet();
			sheetFromDbSorted.SheetFields=sheetFromDb.SheetFields.OrderBy(sf => sf.SheetFieldNum).ToList();
			sheetNewSorted.SheetFields=sheetNew.SheetFields.OrderBy(sf => sf.SheetFieldNum).ToList();
			for(int i = 0;i<sheetFromDbSorted.SheetFields.Count;i++) {
				//Explicitly compare the sheet field values that can be imported via web forms.
				//This makes it so that any future columns added to the sheetfield table will not affect this comparer.
				//When new columns are added, we can now decide on a per column basis if the column matters for comparisons.
				//We will always add new columns below and simply comment them out if we do not want to use them for comparison, this way we know that the column was considered.
				if(sheetFromDbSorted.SheetFields[i].SheetNum!=sheetNewSorted.SheetFields[i].SheetNum
					|| sheetFromDbSorted.SheetFields[i].FieldType!=sheetNewSorted.SheetFields[i].FieldType
					|| sheetFromDbSorted.SheetFields[i].FieldName!=sheetNewSorted.SheetFields[i].FieldName
					|| sheetFromDbSorted.SheetFields[i].FieldValue!=sheetNewSorted.SheetFields[i].FieldValue
					|| sheetFromDbSorted.SheetFields[i].FontSize!=sheetNewSorted.SheetFields[i].FontSize
					|| sheetFromDbSorted.SheetFields[i].FontName!=sheetNewSorted.SheetFields[i].FontName
					|| sheetFromDbSorted.SheetFields[i].FontIsBold!=sheetNewSorted.SheetFields[i].FontIsBold
					|| sheetFromDbSorted.SheetFields[i].XPos!=sheetNewSorted.SheetFields[i].XPos
					|| sheetFromDbSorted.SheetFields[i].YPos!=sheetNewSorted.SheetFields[i].YPos
					|| sheetFromDbSorted.SheetFields[i].Width!=sheetNewSorted.SheetFields[i].Width
					|| sheetFromDbSorted.SheetFields[i].Height!=sheetNewSorted.SheetFields[i].Height
					|| sheetFromDbSorted.SheetFields[i].GrowthBehavior!=sheetNewSorted.SheetFields[i].GrowthBehavior
					|| sheetFromDbSorted.SheetFields[i].RadioButtonValue!=sheetNewSorted.SheetFields[i].RadioButtonValue
					|| sheetFromDbSorted.SheetFields[i].RadioButtonGroup!=sheetNewSorted.SheetFields[i].RadioButtonGroup
					|| sheetFromDbSorted.SheetFields[i].IsRequired!=sheetNewSorted.SheetFields[i].IsRequired
					|| sheetFromDbSorted.SheetFields[i].TabOrder!=sheetNewSorted.SheetFields[i].TabOrder
					|| sheetFromDbSorted.SheetFields[i].ReportableName!=sheetNewSorted.SheetFields[i].ReportableName
					|| sheetFromDbSorted.SheetFields[i].TextAlign!=sheetNewSorted.SheetFields[i].TextAlign
					|| sheetFromDbSorted.SheetFields[i].ItemColor!=sheetNewSorted.SheetFields[i].ItemColor
					|| sheetFromDbSorted.SheetFields[i].UiLabelMobile!=sheetNewSorted.SheetFields[i].UiLabelMobile
					|| sheetFromDbSorted.SheetFields[i].UiLabelMobileRadioButton!=sheetNewSorted.SheetFields[i].UiLabelMobileRadioButton
					) {
					return false;//No need to keep looping, we know the sheets are not equal at this point.
				}
			}
			return true;//All web form sheet fields are equal.
		}

		/// <summary>
		private static void FillPatientFields(Patient patient,FieldInfo fieldInfo,string sheetWebFieldValue,string radioButtonValue,string cultureName,
			bool isWebForm,bool isCemtTransfer) 
		{
			try {
				//Jordan. This try is too big.  Different strategy should have been used.
				switch(fieldInfo.Name) {
					case "Birthdate":
						fieldInfo.SetValue(patient,SheetFields.GetBirthDate(sheetWebFieldValue,isWebForm,isCemtTransfer,cultureName:cultureName));
						break;
					case "Gender":
						if(radioButtonValue=="Male") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,PatientGender.Male);
							}
						}
						if(radioButtonValue=="Female") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,PatientGender.Female);
							}
						}
						break;
					case "Position":
						if(radioButtonValue=="Married") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,PatientPosition.Married);
							}
						}
						if(radioButtonValue=="Single") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,PatientPosition.Single);
							}
						}
						break;
					case "PreferContactMethod":
					case "PreferConfirmMethod":
					case "PreferRecallMethod":
						ContactMethod method;
						if(Enum.TryParse(sheetWebFieldValue,out method)) {
							fieldInfo.SetValue(patient,method);
						}
						if(radioButtonValue=="HmPhone" && sheetWebFieldValue=="X") {
							fieldInfo.SetValue(patient,ContactMethod.HmPhone);
						}
						if(radioButtonValue=="WkPhone" && sheetWebFieldValue=="X") {
							fieldInfo.SetValue(patient,ContactMethod.WkPhone);
						}
						if(radioButtonValue=="WirelessPh" && sheetWebFieldValue=="X") {
							fieldInfo.SetValue(patient,ContactMethod.WirelessPh);
						}
						if(radioButtonValue=="Email" && sheetWebFieldValue=="X") {
							fieldInfo.SetValue(patient,ContactMethod.Email);
						}
						break;
					case "StudentStatus":
						if(radioButtonValue=="Nonstudent") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,"");
							}
						}
						if(radioButtonValue=="Fulltime") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,"F");
							}
						}
						if(radioButtonValue=="Parttime") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,"P");
							}
						}
						break;
					case "ins1Relat":
					case "ins2Relat":
						if(radioButtonValue=="Self") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,Relat.Self);
							}
						}
						if(radioButtonValue=="Spouse") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,Relat.Spouse);
							}
						}
						if(radioButtonValue=="Child") {
							if(sheetWebFieldValue=="X") {
								fieldInfo.SetValue(patient,Relat.Child);
							}
						}
						break;
					default:
						fieldInfo.SetValue(patient,sheetWebFieldValue);
						break;
				}//switch case
			}
			catch(Exception e) {
				MessageBox.Show(fieldInfo.Name+e.Message);
			}
		}

		/// <summary> Helper class to determine which choice a user made for each patient when importing web forms.</summary>
		private class PatientImportChoice {
			///<summary>Stores the action that the user chose for a specific patient when retrieving web forms. Key is a combination of lName, fName, and bDate.</summary>
			private Dictionary<string, ChosenAction> _dictPatientChoices;
			//Jordan 2023-05-31-No dictionaries allowed.  A better choice would have been an object with all 5 fields:
			//ImportPatSkip (or whatever): LName, FName, BDate, PatNum, IsSkip.
			//Then, make a list of ImportPatSkips instead of a dictionary.

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
