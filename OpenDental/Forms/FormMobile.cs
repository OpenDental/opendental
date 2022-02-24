using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;


namespace OpenDental {
	///<summary>THIS FORM HAS BEEN DEPRECATED!!! All functionality that previously existed in this form has been moved to FormPatientPortalSetup.</summary>
	public partial class FormMobile:FormODBase {
		private static MobileWeb.Mobile mb=new MobileWeb.Mobile();
		private static int BatchSize=100;
		/////<summary>All statements of a patient are not uploaded. The limit is defined by the recent [statementLimitPerPatient] records</summary>
		//private static int statementLimitPerPatient=5;
		///<summary>This variable prevents the synching methods from being called when a previous synch is in progress.</summary>
		private static bool IsSynching;
		///<summary>This variable prevents multiple error message boxes from popping up if mobile synch server is not available.</summary>
		private static bool IsServerAvail=true;
		///<summary>True if a pref was saved and the other workstations need to have their cache refreshed when this form closes.</summary>
		private bool changed;
		///<summary>If this variable is true then records are uploaded one at a time so that an error in uploading can be traced down to a single record</summary>
		private static bool IsTroubleshootMode=false;
		private static FormProgress FormP;

		private enum SynchEntity {
			patient,
			appointment,
			prescription,
			provider,
			pharmacy,
			labpanel,
			labresult,
			medication,
			medicationpat,
			allergy,
			allergydef,
			disease,
			diseasedef,
			icd9,
			statement,
			document,
			recall,
			deletedobject,
			patientdel
		}

		public FormMobile() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMobileSetup_Load(object sender,EventArgs e) {
			textMobileSyncServerURL.Text=PrefC.GetString(PrefName.MobileSyncServerURL);
			textSynchMinutes.Text=PrefC.GetInt(PrefName.MobileSyncIntervalMinutes).ToString();
			textDateBefore.Text=PrefC.GetDate(PrefName.MobileExcludeApptsBeforeDate).ToShortDateString();
			textMobileSynchWorkStation.Text=PrefC.GetString(PrefName.MobileSyncWorkstationName);
			textMobileUserName.Text=PrefC.GetString(PrefName.MobileUserName);
			textMobilePassword.Text="";//not stored locally, and not pulled from web server
			DateTime lastRun=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun);
			if(lastRun.Year>1880) {
				textDateTimeLastRun.Text=lastRun.ToShortDateString()+" "+lastRun.ToShortTimeString();
			}
			//Web server is not contacted when loading this form.  That would be too slow.
			//CreateAppointments(5);
		}

		private void butCurrentWorkstation_Click(object sender,EventArgs e) {
			textMobileSynchWorkStation.Text=System.Environment.MachineName.ToUpper();
		}

		private void butSave_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			if(!SavePrefs()) {
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done");
		}

		///<summary>Returns false if validation failed.  This also makes sure the web service exists, the customer is paid, and the registration key is correct.</summary>
		private bool SavePrefs(){
			//validation
			if(!textSynchMinutes.IsValid() || !textDateBefore.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			//yes, workstation is allowed to be blank.  That's one way for user to turn off auto synch.
			//if(textMobileSynchWorkStation.Text=="") {
			//	MsgBox.Show(this,"WorkStation cannot be empty");
			//	return false;
			//}
			// the text field is read because the keyed in values have not been saved yet
			//if(textMobileSyncServerURL.Text.Contains("192.168.0.196") || textMobileSyncServerURL.Text.Contains("localhost")) {
			if(textMobileSyncServerURL.Text.Contains("10.10.1.196") || textMobileSyncServerURL.Text.Contains("localhost")) {
				IgnoreCertificateErrors();// done so that TestWebServiceExists() does not thow an error.
			}
			// if this is not done then an old non-functional url prevents any new url from being saved.
			Prefs.UpdateString(PrefName.MobileSyncServerURL,textMobileSyncServerURL.Text);
			if(!TestWebServiceExists()) {
				MsgBox.Show(this,"Web service not found.");
				return false;
			}
			if(mb.GetCustomerNum(PrefC.GetString(PrefName.RegistrationKey))==0) {
				MsgBox.Show(this,"Registration key is incorrect.");
				return false;
			}
			if(!VerifyPaidCustomer()) {
				return false;
			}
			//Minimum 10 char.  Must contain uppercase, lowercase, numbers, and symbols. Valid symbols are: !@#$%^&+= 
			//The set of symbols checked was far too small, not even including periods, commas, and parentheses.
			//So I rewrote it all.  New error messages say exactly what's wrong with it.
			if(textMobileUserName.Text!="") {//allowed to be blank
				if(textMobileUserName.Text.Length<10) {
					MsgBox.Show(this,"User Name must be at least 10 characters long.");
					return false;
				}
				if(!Regex.IsMatch(textMobileUserName.Text,"[A-Z]+")) {
					MsgBox.Show(this,"User Name must contain an uppercase letter.");
					return false;
				}
				if(!Regex.IsMatch(textMobileUserName.Text,"[a-z]+")) {
					MsgBox.Show(this,"User Name must contain an lowercase letter.");
					return false;
				}
				if(!Regex.IsMatch(textMobileUserName.Text,"[0-9]+")) {
					MsgBox.Show(this,"User Name must contain a number.");
					return false;
				}
				if(!Regex.IsMatch(textMobileUserName.Text,"[^0-9a-zA-Z]+")) {//absolutely anything except number, lower or upper.
					MsgBox.Show(this,"User Name must contain punctuation or symbols.");
					return false;
				}
			}
			if(textDateBefore.Text==""){//default to one year if empty
				textDateBefore.Text=DateTime.Today.AddYears(-1).ToShortDateString();
				//not going to bother informing user.  They can see it.
			}
			//save to db------------------------------------------------------------------------------------
			if(Prefs.UpdateString(PrefName.MobileSyncServerURL,textMobileSyncServerURL.Text)
				| Prefs.UpdateInt(PrefName.MobileSyncIntervalMinutes,PIn.Int(textSynchMinutes.Text))//blank entry allowed
				| Prefs.UpdateString(PrefName.MobileExcludeApptsBeforeDate,POut.Date(PIn.Date(textDateBefore.Text),false))//blank 
				| Prefs.UpdateString(PrefName.MobileSyncWorkstationName,textMobileSynchWorkStation.Text)
				| Prefs.UpdateString(PrefName.MobileUserName,textMobileUserName.Text)
			){
				changed=true;
				Prefs.RefreshCache();
			}
			//Username and password-----------------------------------------------------------------------------
			mb.SetMobileWebUserPassword(PrefC.GetString(PrefName.RegistrationKey),textMobileUserName.Text.Trim(),textMobilePassword.Text.Trim());
			return true;
		}

		///<summary>Uploads Preferences to the Patient Portal /Mobile Web.</summary>
		public static void UploadPreference(PrefName prefname) {
			if(PrefC.GetString(PrefName.RegistrationKey)=="") {
				return;//Prevents a bug when using the trial version with no registration key.  Practice edit, OK, was giving error.
			}
			try {
				if(TestWebServiceExists()) {
					Prefm prefm = Prefms.GetPrefm(prefname.ToString());
					//mb.SetPreference(PrefC.GetString(PrefName.RegistrationKey),prefm);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);//may not show if called from a thread but that does not matter - the failing of this method should not stop the  the code from proceeding.
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!SavePrefs()) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete all your data from our server?  This happens automatically before a full synch.")) {
				return;
			}
			mb.DeleteAllRecords(PrefC.GetString(PrefName.RegistrationKey));
			MsgBox.Show(this,"Done");
		}
		
		private void butFullSync_Click(object sender,EventArgs e) {
			if(!SavePrefs()) {
				return;
			}
			if(IsSynching) {
				MsgBox.Show(this,"A Synch is in progress at the moment. Please try again later.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will be time consuming. Continue anyway?")) {
				return;
			}
			//for full synch, delete all records then repopulate.
			mb.DeleteAllRecords(PrefC.GetString(PrefName.RegistrationKey));
			ShowProgressForm(DateTime.MinValue);
		}

		private void butSync_Click(object sender,EventArgs e) {
			if(!SavePrefs()) {
				return;
			}
			if(IsSynching) {
				MsgBox.Show(this,"A Synch is in progress at the moment. Please try again later.");
				return;
			}
			if(PrefC.GetDate(PrefName.MobileExcludeApptsBeforeDate).Year<1880) {
				MsgBox.Show(this,"Full synch has never been run before.");
				return;
			}
			DateTime changedSince=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun);
			ShowProgressForm(changedSince);
		}
		
		private void ShowProgressForm(DateTime changedSince){
			if(checkTroubleshooting.Checked) {
				IsTroubleshootMode=true;
			}
			else {
				IsTroubleshootMode=false;
			}
			DateTime timeSynchStarted=MiscData.GetNowDateTime();
			FormP=new FormProgress();
			FormP.MaxVal=100;//to keep the form from closing until the real MaxVal is set.
			FormP.NumberMultiplication=1;
			FormP.DisplayText="Preparing records for upload.";
			FormP.NumberFormat="F0";
			//start the thread that will perform the upload
			ThreadStart uploadDelegate= delegate { UploadWorker(changedSince,timeSynchStarted); };
			Thread workerThread=new Thread(uploadDelegate);
			workerThread.Start();
			//display the progress dialog to the user:
			FormP.ShowDialog();
			if(FormP.DialogResult==DialogResult.Cancel) {
				workerThread.Abort();
			}
			changed=true;
			textDateTimeLastRun.Text=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun).ToShortDateString()+" "+PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun).ToShortTimeString();
			FormP.Dispose();
		}


		///<summary>This is the function that the worker thread uses to actually perform the upload.  Can also call this method in the ordinary way if the data to be transferred is small.  The timeSynchStarted must be passed in to ensure that no records are skipped due to small time differences.</summary>
		private static void UploadWorker(DateTime changedSince,DateTime timeSynchStarted) {
			int totalCount=100;
			try {//Dennis: try catch may not work: Does not work in threads, not sure about this. Note that all methods inside this try catch block are without exception handling. This is done on purpose so that when an exception does occur it does not update PrefName.MobileSyncDateTimeLastRun
				//The handling of PrefName.MobileSynchNewTables79 should never be removed in future versions
				DateTime changedProv=changedSince;
				DateTime changedDeleted=changedSince;
				DateTime changedPat=changedSince;
				DateTime changedStatement=changedSince;
				DateTime changedDocument=changedSince;
				DateTime changedRecall=changedSince;
				if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables79Done,false)) {
					changedProv=DateTime.MinValue;
					changedDeleted=DateTime.MinValue;
				}
				if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables112Done,false)) {
				    changedPat=DateTime.MinValue;
					changedStatement=DateTime.MinValue;
					changedDocument=DateTime.MinValue;
				}
				if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables121Done,false)) {
					changedRecall=DateTime.MinValue;
					UploadPreference(PrefName.PracticeTitle); //done again because the previous upload did not include the prefnum
				}
				bool synchDelPat=true;
				if(PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun).Hour==timeSynchStarted.Hour) {
					synchDelPat=false;// synching delPatNumList is timeconsuming (15 seconds) for a dental office with around 5000 patients and it's mostly the same records that have to be deleted every time a synch happens. So it's done only once hourly.
				}
				//MobileWeb
				List<long> patNumList=Patientms.GetChangedSincePatNums(changedPat);
				List<long> aptNumList=Appointmentms.GetChangedSinceAptNums(changedSince,PrefC.GetDate(PrefName.MobileExcludeApptsBeforeDate));
				List<long> rxNumList=RxPatms.GetChangedSinceRxNums(changedSince);
				List<long> provNumList=Providerms.GetChangedSinceProvNums(changedProv);
				List<long> pharNumList=Pharmacyms.GetChangedSincePharmacyNums(changedSince);
				List<long> allergyDefNumList=AllergyDefms.GetChangedSinceAllergyDefNums(changedSince);
				List<long> allergyNumList=Allergyms.GetChangedSinceAllergyNums(changedSince);
				//exclusively Patient Portal
				/*
				List<long> eligibleForUploadPatNumList=Patientms.GetPatNumsEligibleForSynch();
				List<long> labPanelNumList=LabPanelms.GetChangedSinceLabPanelNums(changedSince,eligibleForUploadPatNumList);
				List<long> labResultNumList=LabResultms.GetChangedSinceLabResultNums(changedSince);
				List<long> medicationNumList=Medicationms.GetChangedSinceMedicationNums(changedSince);
				List<long> medicationPatNumList=MedicationPatms.GetChangedSinceMedicationPatNums(changedSince,eligibleForUploadPatNumList);
				List<long> diseaseDefNumList=DiseaseDefms.GetChangedSinceDiseaseDefNums(changedSince);
				List<long> diseaseNumList=Diseasems.GetChangedSinceDiseaseNums(changedSince,eligibleForUploadPatNumList);
				List<long> icd9NumList=ICD9ms.GetChangedSinceICD9Nums(changedSince);
				List<long> statementNumList=Statementms.GetChangedSinceStatementNums(changedStatement,eligibleForUploadPatNumList,statementLimitPerPatient);
				List<long> documentNumList=Documentms.GetChangedSinceDocumentNums(changedDocument,statementNumList);
				List<long> recallNumList=Recallms.GetChangedSinceRecallNums(changedRecall);*/
				List<long> delPatNumList=Patientms.GetPatNumsForDeletion();
				//List<DeletedObject> dO=DeletedObjects.GetDeletedSince(changedDeleted);dennis: delete this line later
				List<long> deletedObjectNumList=DeletedObjects.GetChangedSinceDeletedObjectNums(changedDeleted);//to delete appointments from mobile
				totalCount= patNumList.Count+aptNumList.Count+rxNumList.Count+provNumList.Count+pharNumList.Count
					//+labPanelNumList.Count+labResultNumList.Count+medicationNumList.Count+medicationPatNumList.Count
					//+allergyDefNumList.Count//+allergyNumList.Count+diseaseDefNumList.Count+diseaseNumList.Count+icd9NumList.Count
					//+statementNumList.Count+documentNumList.Count+recallNumList.Count
					+deletedObjectNumList.Count;
				if(synchDelPat) {
					totalCount+=delPatNumList.Count;
				}
				double currentVal=0;
				if(Application.OpenForms["FormProgress"]!=null) {// without this line the following error is thrown: "Invoke or BeginInvoke cannot be called on a control until the window handle has been created." or a null pointer exception is thrown when an automatic synch is done by the system.
					FormP.Invoke(new PassProgressDelegate(PassProgressToDialog),
						new object[] { currentVal,"?currentVal of ?maxVal records uploaded",totalCount,"" });
				}
				IsSynching=true;
				SynchGeneric(patNumList,SynchEntity.patient,totalCount,ref currentVal);
				SynchGeneric(aptNumList,SynchEntity.appointment,totalCount,ref currentVal);
				SynchGeneric(rxNumList,SynchEntity.prescription,totalCount,ref currentVal);
				SynchGeneric(provNumList,SynchEntity.provider,totalCount,ref currentVal);
				SynchGeneric(pharNumList,SynchEntity.pharmacy,totalCount,ref currentVal);
				//pat portal
				/*
				SynchGeneric(labPanelNumList,SynchEntity.labpanel,totalCount,ref currentVal);
				SynchGeneric(labResultNumList,SynchEntity.labresult,totalCount,ref currentVal);
				SynchGeneric(medicationNumList,SynchEntity.medication,totalCount,ref currentVal);
				SynchGeneric(medicationPatNumList,SynchEntity.medicationpat,totalCount,ref currentVal);
				SynchGeneric(allergyDefNumList,SynchEntity.allergydef,totalCount,ref currentVal);
				SynchGeneric(allergyNumList,SynchEntity.allergy,totalCount,ref currentVal);
				SynchGeneric(diseaseDefNumList,SynchEntity.diseasedef,totalCount,ref currentVal);
				SynchGeneric(diseaseNumList,SynchEntity.disease,totalCount,ref currentVal);
				SynchGeneric(icd9NumList,SynchEntity.icd9,totalCount,ref currentVal);
				SynchGeneric(statementNumList,SynchEntity.statement,totalCount,ref currentVal);
				SynchGeneric(documentNumList,SynchEntity.document,totalCount,ref currentVal);
				SynchGeneric(recallNumList,SynchEntity.recall,totalCount,ref currentVal);*/
				if(synchDelPat) {
					SynchGeneric(delPatNumList,SynchEntity.patientdel,totalCount,ref currentVal);
				}
				//DeleteObjects(dO,totalCount,ref currentVal);// this has to be done at this end because objects may have been created and deleted between synchs. If this function is place above then the such a deleted object will not be deleted from the server.
				SynchGeneric(deletedObjectNumList,SynchEntity.deletedobject,totalCount,ref currentVal);// this has to be done at this end because objects may have been created and deleted between synchs. If this function is place above then the such a deleted object will not be deleted from the server.
				if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables79Done,true)) {
				    Prefs.UpdateBool(PrefName.MobileSynchNewTables79Done,true);
				}
				if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables112Done,true)) {
				    Prefs.UpdateBool(PrefName.MobileSynchNewTables112Done,true);
				}
				if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables121Done,true)) {
					Prefs.UpdateBool(PrefName.MobileSynchNewTables121Done,true);
				}
				Prefs.UpdateDateT(PrefName.MobileSyncDateTimeLastRun,timeSynchStarted);
				IsSynching=false;
			}
			catch(Exception e) {
				IsSynching=false;// this will ensure that the synch can start again. If this variable remains true due to an exception then a synch will never take place automatically.
				if(Application.OpenForms["FormProgress"]!=null) {// without this line the following error is thrown: "Invoke or BeginInvoke cannot be called on a control until the window handle has been created." or a null pointer exception is thrown when an automatic synch is done by the system.
					FormP.Invoke(new PassProgressDelegate(PassProgressToDialog),
						new object[] { 0,"?currentVal of ?maxVal records uploaded",totalCount,e.Message });
				}
			}
		}

		///<summary>a general function to reduce the amount of code for uploading</summary>
		private static void SynchGeneric(List<long> PKNumList,SynchEntity entity,double totalCount,ref double currentVal) {
			//Dennis: a try catch block here has been avoid on purpose.
			/*
			List<long> BlockPKNumList=null;
			int localBatchSize=BatchSize;
			if(IsTroubleshootMode) {
				localBatchSize=1;
			}
			string AtoZpath=ImageStore.GetPreferredAtoZpath();
			for(int start=0;start<PKNumList.Count;start+=localBatchSize) {
				if((start+localBatchSize)>PKNumList.Count) {
					localBatchSize=PKNumList.Count-start;
				}
				try{
					BlockPKNumList=PKNumList.GetRange(start,localBatchSize);
					switch(entity) {
						case SynchEntity.patient:
							List<Patientm> changedPatientmList=Patientms.GetMultPats(BlockPKNumList);
							mb.SynchPatients(PrefC.GetString(PrefName.RegistrationKey),changedPatientmList.ToArray());
						break;
						case SynchEntity.appointment:
							List<Appointmentm> changedAppointmentmList=Appointmentms.GetMultApts(BlockPKNumList);
							mb.SynchAppointments(PrefC.GetString(PrefName.RegistrationKey),changedAppointmentmList.ToArray());
						break;
						case SynchEntity.prescription:
							List<RxPatm> changedRxList=RxPatms.GetMultRxPats(BlockPKNumList);
							mb.SynchPrescriptions(PrefC.GetString(PrefName.RegistrationKey),changedRxList.ToArray());
						break;
						case SynchEntity.provider:
							List<Providerm> changedProvList=Providerms.GetMultProviderms(BlockPKNumList);
							mb.SynchProviders(PrefC.GetString(PrefName.RegistrationKey),changedProvList.ToArray());
						break;
						case SynchEntity.pharmacy:
						List<Pharmacym> changedPharmacyList=Pharmacyms.GetMultPharmacyms(BlockPKNumList);
						mb.SynchPharmacies(PrefC.GetString(PrefName.RegistrationKey),changedPharmacyList.ToArray());
						break;
						case SynchEntity.labpanel:
							List<LabPanelm> ChangedLabPanelList=LabPanelms.GetMultLabPanelms(BlockPKNumList);
							mb.SynchLabPanels(PrefC.GetString(PrefName.RegistrationKey),ChangedLabPanelList.ToArray());
						break;
						case SynchEntity.labresult:
							List<LabResultm> ChangedLabResultList=LabResultms.GetMultLabResultms(BlockPKNumList);
							mb.SynchLabResults(PrefC.GetString(PrefName.RegistrationKey),ChangedLabResultList.ToArray());
						break;
						case SynchEntity.medication:
							List<Medicationm> ChangedMedicationList=Medicationms.GetMultMedicationms(BlockPKNumList);
							mb.SynchMedications(PrefC.GetString(PrefName.RegistrationKey),ChangedMedicationList.ToArray());
						break;
						case SynchEntity.medicationpat:
							List<MedicationPatm> ChangedMedicationPatList=MedicationPatms.GetMultMedicationPatms(BlockPKNumList);
							mb.SynchMedicationPats(PrefC.GetString(PrefName.RegistrationKey),ChangedMedicationPatList.ToArray());
						break;
						case SynchEntity.allergy:
							List<Allergym> ChangedAllergyList=Allergyms.GetMultAllergyms(BlockPKNumList);
							mb.SynchAllergies(PrefC.GetString(PrefName.RegistrationKey),ChangedAllergyList.ToArray());
						break;
						case SynchEntity.allergydef:
							List<AllergyDefm> ChangedAllergyDefList=AllergyDefms.GetMultAllergyDefms(BlockPKNumList);
							mb.SynchAllergyDefs(PrefC.GetString(PrefName.RegistrationKey),ChangedAllergyDefList.ToArray());
						break;
						case SynchEntity.disease:
							List<Diseasem> ChangedDiseaseList=Diseasems.GetMultDiseasems(BlockPKNumList);
							mb.SynchDiseases(PrefC.GetString(PrefName.RegistrationKey),ChangedDiseaseList.ToArray());
						break;
						case SynchEntity.diseasedef:
							List<DiseaseDefm> ChangedDiseaseDefList=DiseaseDefms.GetMultDiseaseDefms(BlockPKNumList);
							mb.SynchDiseaseDefs(PrefC.GetString(PrefName.RegistrationKey),ChangedDiseaseDefList.ToArray());
						break;
						case SynchEntity.icd9:
							List<ICD9m> ChangedICD9List=ICD9ms.GetMultICD9ms(BlockPKNumList);
							mb.SynchICD9s(PrefC.GetString(PrefName.RegistrationKey),ChangedICD9List.ToArray());
						break;
						case SynchEntity.statement:
						List<Statementm> ChangedStatementList=Statementms.GetMultStatementms(BlockPKNumList);
						mb.SynchStatements(PrefC.GetString(PrefName.RegistrationKey),ChangedStatementList.ToArray());
						break;
						case SynchEntity.document:
						List<Documentm> ChangedDocumentList=Documentms.GetMultDocumentms(BlockPKNumList,AtoZpath);
						mb.SynchDocuments(PrefC.GetString(PrefName.RegistrationKey),ChangedDocumentList.ToArray());
						break;
						case SynchEntity.recall:
						List<Recallm> ChangedRecallList=Recallms.GetMultRecallms(BlockPKNumList);
						mb.SynchRecalls(PrefC.GetString(PrefName.RegistrationKey),ChangedRecallList.ToArray());
						break;
						case SynchEntity.deletedobject:
						List<DeletedObject> ChangedDeleteObjectList=DeletedObjects.GetMultDeletedObjects(BlockPKNumList);
						mb.DeleteObjects(PrefC.GetString(PrefName.RegistrationKey),ChangedDeleteObjectList.ToArray());
						break;
						case SynchEntity.patientdel:
						mb.DeletePatientsRecords(PrefC.GetString(PrefName.RegistrationKey),BlockPKNumList.ToArray());
						break;
					}
					//progressIndicator.CurrentVal+=LocalBatchSize;//not allowed
					currentVal+=localBatchSize;
					if(Application.OpenForms["FormProgress"]!=null) {// without this line the following error is thrown: "Invoke or BeginInvoke cannot be called on a control until the window handle has been created." or a null pointer exception is thrown when an automatic synch is done by the system.
						FormP.Invoke(new PassProgressDelegate(PassProgressToDialog),
							new object[] {currentVal,"?currentVal of ?maxVal records uploaded",totalCount,"" });
					}
				}
				catch(Exception e) {
					if(IsTroubleshootMode) {
						string errorMessage=entity+ " with Primary Key = "+BlockPKNumList.First() + " failed to synch. " +  "\n" + e.Message;
						throw new Exception(errorMessage);
					}
					else {
						throw e;
					}
				}
			}//for loop ends here
			*/
		}

		///<summary>This method gets invoked from the worker thread.</summary>
		private static void PassProgressToDialog(double currentVal,string displayText,double maxVal,string errorMessage) {
			FormP.CurrentVal=currentVal;
			FormP.DisplayText=displayText;
			FormP.MaxVal=maxVal;
			FormP.ErrorMessage=errorMessage;
		}

		/*
		private static void DeleteObjects(List<DeletedObject> dO,double totalCount,ref double currentVal) {
			int LocalBatchSize=BatchSize;
			if(IsTroubleshootMode) {
				LocalBatchSize=1;
			}
			for(int start=0;start<dO.Count;start+=LocalBatchSize) {
				try {
				if((start+LocalBatchSize)>dO.Count) {
					mb.DeleteObjects(PrefC.GetString(PrefName.RegistrationKey),dO.ToArray()); //dennis check this - why is it not done in batches.
					LocalBatchSize=dO.Count-start;
				}
				currentVal+=BatchSize;
				if(Application.OpenForms["FormProgress"]!=null) {// without this line the following error is thrown: "Invoke or BeginInvoke cannot be called on a control until the window handle has been created." or a null pointer exception is thrown when an automatic synch is done by the system.
					FormP.Invoke(new PassProgressDelegate(PassProgressToDialog),
						new object[] {currentVal,"?currentVal of ?maxVal records uploaded",totalCount,"" });
				}
								}
				catch(Exception e) {
					if(IsTroubleshootMode) {
						//string errorMessage="DeleteObjects with Primary Key = "+BlockPKNumList.First() + " failed to synch. " +  "\n" + e.Message;
						//throw new Exception(errorMessage);
					}
					else {
						throw e;
					}
				}
			}//for loop ends here
			
		}
		*/	 
		/// <summary>An empty method to test if the webservice is up and running. This was made with the intention of testing the correctness of the webservice URL. If an incorrect webservice URL is used in a background thread the exception cannot be handled easily to a point where even a correct URL cannot be keyed in by the user. Because an exception in a background thread closes the Form which spawned it.</summary>
		private static bool TestWebServiceExists() {
			try {
				mb.Url=PrefC.GetString(PrefName.MobileSyncServerURL);
				if(mb.ServiceExists()) {
					return true;
				}
			}
			catch{
				return false;
			}
			return false;
		}

		private bool VerifyPaidCustomer() {
			//if(textMobileSyncServerURL.Text.Contains("192.168.0.196") || textMobileSyncServerURL.Text.Contains("localhost")) {
			if(textMobileSyncServerURL.Text.Contains("10.10.1.196") || textMobileSyncServerURL.Text.Contains("localhost")) {
				IgnoreCertificateErrors();
			}
			bool isPaidCustomer=mb.IsPaidCustomer(PrefC.GetString(PrefName.RegistrationKey));
			if(!isPaidCustomer) {
				textSynchMinutes.Text="0";
				Prefs.UpdateInt(PrefName.MobileSyncIntervalMinutes,0);
				changed=true;
				MsgBox.Show(this,"This feature requires a separate monthly payment.  Please call customer support.");
				return false;
			}
			return true;
		}

		///<summary>Called from FormOpenDental and from FormEhrOnlineAccess.  doForce is set to false to follow regular synching interval.</summary>
		public static void SynchFromMain(bool doForce) {
			if(Application.OpenForms["FormMobile"]!=null) {//tested.  This prevents main synch whenever this form is open.
				return;
			}
			if(IsSynching) {
				return;
			}
			DateTime timeSynchStarted=MiscData.GetNowDateTime();
			if(!doForce) {//if doForce, we skip checking the interval
				if(timeSynchStarted < PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun).AddMinutes(PrefC.GetInt(PrefName.MobileSyncIntervalMinutes))) {
					return;
				}
			}
			//if(PrefC.GetString(PrefName.MobileSyncServerURL).Contains("192.168.0.196") || PrefC.GetString(PrefName.MobileSyncServerURL).Contains("localhost")) {
			if(PrefC.GetString(PrefName.MobileSyncServerURL).Contains("10.10.1.196") || PrefC.GetString(PrefName.MobileSyncServerURL).Contains("localhost")) {
				IgnoreCertificateErrors();
			}
			if(!TestWebServiceExists()) {
				if(!doForce) {//if being used from FormOpenDental as part of timer
					if(IsServerAvail) {//this will only happen the first time to prevent multiple windows.
						IsServerAvail=false;
						DialogResult res=MessageBox.Show("Mobile synch server not available.  Synch failed.  Turn off synch?","",MessageBoxButtons.YesNo);
						if(res==DialogResult.Yes) {
							Prefs.UpdateInt(PrefName.MobileSyncIntervalMinutes,0);
						}
					}
				}
				return;
			}
			else {
				IsServerAvail=true;
			}
			DateTime changedSince=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun);			
			//FormProgress FormP=new FormProgress();//but we won't display it.
			//FormP.NumberFormat="";
			//FormP.DisplayText="";
			//start the thread that will perform the upload
			ThreadStart uploadDelegate= delegate { UploadWorker(changedSince,timeSynchStarted); };
			Thread workerThread=new Thread(uploadDelegate);
			workerThread.Start();
		}

		#region Testing
		///<summary>This allows the code to continue by not throwing an exception even if there is a problem with the security certificate.</summary>
		private static void IgnoreCertificateErrors() {
			System.Net.ServicePointManager.ServerCertificateValidationCallback+=
			delegate(object sender,System.Security.Cryptography.X509Certificates.X509Certificate certificate,
									System.Security.Cryptography.X509Certificates.X509Chain chain,
									System.Net.Security.SslPolicyErrors sslPolicyErrors) {
				return true;
			};
		}
		
		/// <summary>For testing only</summary>
		private static void CreatePatients(int PatientCount) {
			for(int i=0;i<PatientCount;i++) {
				Patient newPat=new Patient();
				newPat.LName="Mathew"+i;
				newPat.FName="Dennis"+i;
				newPat.Address="Address Line 1.Address Line 1___"+i;
				newPat.Address2="Address Line 2. Address Line 2__"+i;
				newPat.AddrNote="Lives off in far off Siberia Lives off in far off Siberia"+i;
				newPat.AdmitDate=new DateTime(1985,3,3).AddDays(i);
				newPat.ApptModNote="Flies from Siberia on specially chartered flight piloted by goblins:)"+i;
				newPat.AskToArriveEarly=1555;
				newPat.BillingType=3;
				newPat.ChartNumber="111111"+i;
				newPat.City="NL";
				newPat.ClinicNum=i;
				newPat.CreditType="A";
				newPat.DateFirstVisit=new DateTime(1985,3,3).AddDays(i);
				newPat.Email="dennis.mathew________________seb@siberiacrawlmail.com";
				newPat.HmPhone="416-222-5678";
				newPat.WkPhone="416-222-5678";
				newPat.Zip="M3L 2L9";
				newPat.WirelessPhone="416-222-5678";
				newPat.Birthdate=new DateTime(1970,3,3).AddDays(i);
				Patients.Insert(newPat,false);
				//set Guarantor field the same as PatNum
				Patient patOld=newPat.Copy();
				newPat.Guarantor=newPat.PatNum;
				Patients.Update(newPat,patOld);
			}
		}

		/// <summary>For testing only</summary>
		private static void CreateAppointments(int AppointmentCount) {
			long[] patNumArray=Patients.GetAllPatNums(true);
			DateTime appdate= DateTime.Now;
			for(int i=0;i<patNumArray.Length;i++) {
				appdate=appdate.AddMinutes(20);
				for(int j=0;j<AppointmentCount;j++) {
					Appointment apt=new Appointment();
					appdate=appdate.AddMinutes(20);
					apt.PatNum=patNumArray[i];
					apt.DateTimeArrived=appdate;
					apt.DateTimeAskedToArrive=appdate;
					apt.DateTimeDismissed=appdate;
					apt.DateTimeSeated=appdate;
					apt.AptDateTime=appdate;
					apt.Note="some notenote noten otenotenot enotenot enote"+j;
					apt.IsNewPatient=true;
					apt.ProvNum=3;
					apt.AptStatus=ApptStatus.Scheduled;
					apt.AptDateTime=appdate;
					apt.Op=2;
					apt.Pattern="//XX//////";
					apt.ProcDescript="4-BWX";
					apt.ProcsColored="<span color=\"-16777216\">4-BWX</span>";
					Appointments.Insert(apt);
				}
			}
		}

		/// <summary>For testing only</summary>
		private static void CreatePrescriptions(int PrescriptionCount) {
			long[] patNumArray=Patients.GetAllPatNums(true);
			for(int i=0;i<patNumArray.Length;i++) {
				for(int j=0;j<PrescriptionCount;j++) {
					RxPat rxpat= new RxPat();
					rxpat.Drug="VicodinA VicodinB VicodinC"+j;
					rxpat.Disp="50.50";
					rxpat.IsControlled=true;
					rxpat.PatNum=patNumArray[i];
					rxpat.RxDate=new DateTime(2010,12,1,11,0,0);
					RxPats.Insert(rxpat);
				}
			}
		}

		private static void CreateStatements(int StatementCount) {
			long[] patNumArray=Patients.GetAllPatNums(true);
			for(int i=0;i<patNumArray.Length;i++) {
				for(int j=0;j<StatementCount;j++) {
					Statement st= new Statement();
					st.DateSent=new DateTime(2010,12,1,11,0,0).AddDays(1+j);
					st.DocNum=i+j;
					st.PatNum=patNumArray[i];
					Statements.Insert(st);
				}
			}
		}

		#endregion Testing

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormMobile_FormClosed(object sender,FormClosedEventArgs e) {
			if(changed) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		


















	}
}