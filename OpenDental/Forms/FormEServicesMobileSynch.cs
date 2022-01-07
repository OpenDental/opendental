using CodeBase;
using Microsoft.Win32;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Mobile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.Data;
using System.Linq;
using System.IO;
using WebServiceSerializer;
using OpenDentBusiness.WebServiceMainHQ;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;

namespace OpenDental {

	public partial class FormEServicesMobileSynch:FormODBase {
		private static MobileWeb.Mobile _mobile=new MobileWeb.Mobile();
		private static int _batchSize=100;
		///<summary>This variable prevents the synching methods from being called when a previous synch is in progress.</summary>
		private static bool _isSynching;
		///<summary>This variable prevents multiple error message boxes from popping up if mobile synch server is not available.</summary>
		private static bool _isServerAvail=true;
		///<summary>If this variable is true then records are uploaded one at a time so that an error in uploading can be traced down to a single record</summary>
		private static bool _isTroubleshootMode=false;
		
		public FormEServicesMobileSynch() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEServicesMobileSynch_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EServicesSetup,true)) {
				this.DisableAllExcept(new Control[]{butClose});
			}
			textMobileSyncServerURL.Text=PrefC.GetString(PrefName.MobileSyncServerURL);
			textSynchMinutes.Text=PrefC.GetInt(PrefName.MobileSyncIntervalMinutes).ToString();
			textDateBefore.Text=PrefC.GetDate(PrefName.MobileExcludeApptsBeforeDate).ToShortDateString();
			textMobileSynchWorkStation.Text=PrefC.GetString(PrefName.MobileSyncWorkstationName);
			textMobileUserName.Text=PrefC.GetString(PrefName.MobileUserName);
			textMobilePassword.Text="";//not stored locally, and not pulled from web server
			DateTime dateLastRun=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun);
			if(dateLastRun.Year>1880) {
				textDateTimeLastRun.Text=dateLastRun.ToShortDateString()+" "+dateLastRun.ToShortTimeString();
			}
			//Web server is not contacted when loading this form.  That would be too slow.
			//CreateAppointments(5);
		}

		private void butCurrentWorkstation_Click(object sender,EventArgs e) {
			textMobileSynchWorkStation.Text=System.Environment.MachineName.ToUpper();
		}

		private void butSaveMobileSynch_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			if(!SavePrefs()) {
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done");
		}

		///<summary>Returns false if validation failed.  This also makes sure the web service exists, the customer is paid, and the registration key is correct.</summary>
		private bool SavePrefs() {
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
			if(textMobileSyncServerURL.Text.Contains("10.10.1.196")||textMobileSyncServerURL.Text.Contains("localhost")) {
				IgnoreCertificateErrors();// done so that TestWebServiceExists() does not thow an error.
			}
			// if this is not done then an old non-functional url prevents any new url from being saved.
			Prefs.UpdateString(PrefName.MobileSyncServerURL,textMobileSyncServerURL.Text);
			if(!TestWebServiceExists()) {
				MsgBox.Show(this,"Web service not found.");
				return false;
			}
			if(_mobile.GetCustomerNum(PrefC.GetString(PrefName.RegistrationKey))==0) {
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
			if(textDateBefore.Text=="") {//default to one year if empty
				textDateBefore.Text=DateTime.Today.AddYears(-1).ToShortDateString();
				//not going to bother informing user.  They can see it.
			}
			//save to db------------------------------------------------------------------------------------
			Prefs.UpdateString(PrefName.MobileSyncServerURL,textMobileSyncServerURL.Text);
			Prefs.UpdateInt(PrefName.MobileSyncIntervalMinutes,PIn.Int(textSynchMinutes.Text));//blank entry allowed
			Prefs.UpdateString(PrefName.MobileExcludeApptsBeforeDate,POut.Date(PIn.Date(textDateBefore.Text),false));//blank 
			Prefs.UpdateString(PrefName.MobileSyncWorkstationName,textMobileSynchWorkStation.Text);
			Prefs.UpdateString(PrefName.MobileUserName,textMobileUserName.Text);
			//Username and password-----------------------------------------------------------------------------
			_mobile.SetMobileWebUserPassword(PrefC.GetString(PrefName.RegistrationKey),textMobileUserName.Text.Trim(),textMobilePassword.Text.Trim());
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
					//_mobile.SetPreference(PrefC.GetString(PrefName.RegistrationKey),prefm);
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
			_mobile.DeleteAllRecords(PrefC.GetString(PrefName.RegistrationKey));
			MsgBox.Show(this,"Done");
		}

		private void butFullSync_Click(object sender,EventArgs e) {
			if(!SavePrefs()) {
				return;
			}
			if(_isSynching) {
				MsgBox.Show(this,"A Synch is in progress at the moment. Please try again later.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will be time consuming. Continue anyway?")) {
				return;
			}
			//for full synch, delete all records then repopulate.
			_mobile.DeleteAllRecords(PrefC.GetString(PrefName.RegistrationKey));
			ShowProgressForm(DateTime.MinValue);
		}

		private void butSync_Click(object sender,EventArgs e) {
			if(!SavePrefs()) {
				return;
			}
			if(_isSynching) {
				MsgBox.Show(this,"A Synch is in progress at the moment. Please try again later.");
				return;
			}
			if(PrefC.GetDate(PrefName.MobileExcludeApptsBeforeDate).Year<1880) {
				MsgBox.Show(this,"Full synch has never been run before.");
				return;
			}
			DateTime dateChangedSince=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun);
			ShowProgressForm(dateChangedSince);
		}

		private void ShowProgressForm(DateTime dateChangedSince) {
			if(checkTroubleshooting.Checked) {
				_isTroubleshootMode=true;
			}
			else {
				_isTroubleshootMode=false;
			}
			DateTime dateSynchStarted=MiscData.GetNowDateTime();
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => UploadWorker(dateChangedSince,dateSynchStarted);
			progressOD.StartingMessage=Lan.g(this,"Preparing records for upload.");
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				_isSynching=false;// this will ensure that the synch can start again. If this variable remains true due to an exception then a synch will never take place automatically.
				MsgBox.Show(ex.Message);
				return;
			}
			if(progressOD.IsCancelled){
				return;
			}
			textDateTimeLastRun.Text=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun).ToShortDateString()+" "+PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun).ToShortTimeString();
		}


		///<summary>This is the function that the worker thread uses to actually perform the upload.  Can also call this method in the ordinary way if the data to be transferred is small.  The timeSynchStarted must be passed in to ensure that no records are skipped due to small time differences.</summary>
		private static void UploadWorker(DateTime dateChangedSince,DateTime dateTimeSynchStarted) {
			int totalCount=100;
			//The handling of PrefName.MobileSynchNewTables79 should never be removed in future versions
			DateTime dateChangedProv=dateChangedSince;
			DateTime dateChangedDeleted=dateChangedSince;
			DateTime dateChangedPat=dateChangedSince;
			DateTime dateChangedStatement=dateChangedSince;
			DateTime dateChangedDocument=dateChangedSince;
			DateTime dateChangedRecall=dateChangedSince;
			if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables79Done,false)) {
				dateChangedProv=DateTime.MinValue;
				dateChangedDeleted=DateTime.MinValue;
			}
			if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables112Done,false)) {
				dateChangedPat=DateTime.MinValue;
				dateChangedStatement=DateTime.MinValue;
				dateChangedDocument=DateTime.MinValue;
			}
			if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables121Done,false)) {
				dateChangedRecall=DateTime.MinValue;
				UploadPreference(PrefName.PracticeTitle); //done again because the previous upload did not include the prefnum
			}
			bool isSynchDelPat=true;
			if(PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun).Hour==dateTimeSynchStarted.Hour) {
				isSynchDelPat=false;// synching delPatNumList is timeconsuming (15 seconds) for a dental office with around 5000 patients and it's mostly the same records that have to be deleted every time a synch happens. So it's done only once hourly.
			}
			//MobileWeb
			List<long> listPatNums=Patientms.GetChangedSincePatNums(dateChangedPat);
			List<long> listAptNums=Appointmentms.GetChangedSinceAptNums(dateChangedSince,PrefC.GetDate(PrefName.MobileExcludeApptsBeforeDate));
			List<long> listRxNums=RxPatms.GetChangedSinceRxNums(dateChangedSince);
			List<long> listProvNums=Providerms.GetChangedSinceProvNums(dateChangedProv);
			List<long> listPharNums=Pharmacyms.GetChangedSincePharmacyNums(dateChangedSince);
			List<long> listAllergyDefNums=AllergyDefms.GetChangedSinceAllergyDefNums(dateChangedSince);
			List<long> listAllergyNums=Allergyms.GetChangedSinceAllergyNums(dateChangedSince);
			//exclusively Patient Portal
			/*
			List<long> listEligibleForUploadPatNums=Patientms.GetPatNumsEligibleForSynch();
			List<long> listLabPanelNums=LabPanelms.GetChangedSinceLabPanelNums(changedSince,listEligibleForUploadPatNums);
			List<long> listLabResultNums=LabResultms.GetChangedSinceLabResultNums(changedSince);
			List<long> listMedicationNums=Medicationms.GetChangedSinceMedicationNums(changedSince);
			List<long> listMedicationPatNums=MedicationPatms.GetChangedSinceMedicationPatNums(changedSince,listEligibleForUploadPatNums);
			List<long> listDiseaseDefNums=DiseaseDefms.GetChangedSinceDiseaseDefNums(changedSince);
			List<long> listDiseaseNums=Diseasems.GetChangedSinceDiseaseNums(changedSince,listEligibleForUploadPatNums);
			List<long> listIcd9Nums=ICD9ms.GetChangedSinceICD9Nums(changedSince);
			List<long> listStatementNums=Statementms.GetChangedSinceStatementNums(changedStatement,listEligibleForUploadPatNums,statementLimitPerPatient);
			List<long> listDocumentNums=Documentms.GetChangedSinceDocumentNums(changedDocument,listStatementNums);
			List<long> listRecallNums=Recallms.GetChangedSinceRecallNums(changedRecall);*/
			List<long> listPatNumsForDel=Patientms.GetPatNumsForDeletion();
			//List<DeletedObject> listDeletedObject=DeletedObjects.GetDeletedSince(changedDeleted);dennis: delete this line later
			List<long> listDeletedObjectNums=DeletedObjects.GetChangedSinceDeletedObjectNums(dateChangedDeleted);//to delete appointments from mobile
			totalCount=listPatNums.Count+listAptNums.Count+listRxNums.Count+listProvNums.Count+listPharNums.Count
				//+listLabPanelNums.Count+listLabResultNums.Count+listMedicationNums.Count+listMedicationPatNums.Count
				//+listAllergyDefNums.Count//+listAllergyNums.Count+listDiseaseDefNums.Count+listDiseaseNums.Count+listIcd9Nums.Count
				//+listStatementNums.Count+listDocumentNums.Count+listRecallNums.Count
				+listDeletedObjectNums.Count;
			if(isSynchDelPat) {
				totalCount+=listPatNumsForDel.Count;
			}
			double currentVal=0;
			_isSynching=true;
			SynchGeneric(listPatNums,SynchEntity.patient,totalCount,ref currentVal);
			SynchGeneric(listAptNums,SynchEntity.appointment,totalCount,ref currentVal);
			SynchGeneric(listRxNums,SynchEntity.prescription,totalCount,ref currentVal);
			SynchGeneric(listProvNums,SynchEntity.provider,totalCount,ref currentVal);
			SynchGeneric(listPharNums,SynchEntity.pharmacy,totalCount,ref currentVal);
			//pat portal
			/*
			SynchGeneric(listLabPanelNums,SynchEntity.labpanel,totalCount,ref currentVal);
			SynchGeneric(listLabResultNums,SynchEntity.labresult,totalCount,ref currentVal);
			SynchGeneric(listMedicationNums,SynchEntity.medication,totalCount,ref currentVal);
			SynchGeneric(listMedicationPatNums,SynchEntity.medicationpat,totalCount,ref currentVal);
			SynchGeneric(listAllergyDefNums,SynchEntity.allergydef,totalCount,ref currentVal);
			SynchGeneric(listAllergyNums,SynchEntity.allergy,totalCount,ref currentVal);
			SynchGeneric(listDiseaseDefNums,SynchEntity.diseasedef,totalCount,ref currentVal);
			SynchGeneric(listDiseaseNums,SynchEntity.disease,totalCount,ref currentVal);
			SynchGeneric(listIcd9Nums,SynchEntity.icd9,totalCount,ref currentVal);
			SynchGeneric(listStatementNums,SynchEntity.statement,totalCount,ref currentVal);
			SynchGeneric(listDocumentNums,SynchEntity.document,totalCount,ref currentVal);
			SynchGeneric(listRecallNums,SynchEntity.recall,totalCount,ref currentVal);*/
			if(isSynchDelPat) {
				SynchGeneric(listPatNumsForDel,SynchEntity.patientdel,totalCount,ref currentVal);
			}
			//DeleteObjects(dO,totalCount,ref currentVal);// this has to be done at this end because objects may have been created and deleted between synchs. If this function is place above then the such a deleted object will not be deleted from the server.
			SynchGeneric(listDeletedObjectNums,SynchEntity.deletedobject,totalCount,ref currentVal);// this has to be done at this end because objects may have been created and deleted between synchs. If this function is place above then the such a deleted object will not be deleted from the server.
			if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables79Done,true)) {
				Prefs.UpdateBool(PrefName.MobileSynchNewTables79Done,true);
			}
			if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables112Done,true)) {
				Prefs.UpdateBool(PrefName.MobileSynchNewTables112Done,true);
			}
			if(!PrefC.GetBoolSilent(PrefName.MobileSynchNewTables121Done,true)) {
				Prefs.UpdateBool(PrefName.MobileSynchNewTables121Done,true);
			}
			Prefs.UpdateDateT(PrefName.MobileSyncDateTimeLastRun,dateTimeSynchStarted);
			_isSynching=false;
		}

		///<summary>a general function to reduce the amount of code for uploading</summary>
		private static void SynchGeneric(List<long> listPKNum,SynchEntity entity,double totalCount,ref double currentVal) {
			/*
			//Dennis: a try catch block here has been avoid on purpose.
			List<long> listBlockPKNums=null;
			int localBatchSize=_batchSize;
			if(_isTroubleshootMode) {
				localBatchSize=1;
			}
			string AtoZpath=ImageStore.GetPreferredAtoZpath();
			for(int start = 0;start<PKNumList.Count;start+=localBatchSize) {
				if((start+localBatchSize)>PKNumList.Count) {
					localBatchSize=PKNumList.Count-start;
				}
				try {
					listBlockPKNums=PKNumList.GetRange(start,localBatchSize);
					switch(entity) {
						case SynchEntity.patient:
							List<Patientm> listPatientms=Patientms.GetMultPats(listBlockPKNums);
							_mobile.SynchPatients(PrefC.GetString(PrefName.RegistrationKey),listPatientms.ToArray());
							break;
						case SynchEntity.appointment:
							List<Appointmentm> listAppointmentms=Appointmentms.GetMultApts(listBlockPKNums);
							_mobile.SynchAppointments(PrefC.GetString(PrefName.RegistrationKey),listAppointmentms.ToArray());
							break;
						case SynchEntity.prescription:
							List<RxPatm> listRxPatm=RxPatms.GetMultRxPats(listBlockPKNums);
							_mobile.SynchPrescriptions(PrefC.GetString(PrefName.RegistrationKey),listRxPatm.ToArray());
							break;
						case SynchEntity.provider:
							List<Providerm> changedProvList=Providerms.GetMultProviderms(listBlockPKNums);
							_mobile.SynchProviders(PrefC.GetString(PrefName.RegistrationKey),changedProvList.ToArray());
							break;
						case SynchEntity.pharmacy:
							List<Pharmacym> listPharmacyms=Pharmacyms.GetMultPharmacyms(listBlockPKNums);
							_mobile.SynchPharmacies(PrefC.GetString(PrefName.RegistrationKey),listPharmacyms.ToArray());
							break;
						case SynchEntity.labpanel:
							List<LabPanelm> listLabPanelms=LabPanelms.GetMultLabPanelms(listBlockPKNums);
							_mobile.SynchLabPanels(PrefC.GetString(PrefName.RegistrationKey),listLabPanelms.ToArray());
							break;
						case SynchEntity.labresult:
							List<LabResultm> listLabResultms=LabResultms.GetMultLabResultms(listBlockPKNums);
							_mobile.SynchLabResults(PrefC.GetString(PrefName.RegistrationKey),listLabResultms.ToArray());
							break;
						case SynchEntity.medication:
							List<Medicationm> listMedicationms=Medicationms.GetMultMedicationms(listBlockPKNums);
							_mobile.SynchMedications(PrefC.GetString(PrefName.RegistrationKey),listMedicationms.ToArray());
							break;
						case SynchEntity.medicationpat:
							List<MedicationPatm> listMedicationPatms=MedicationPatms.GetMultMedicationPatms(listBlockPKNums);
							_mobile.SynchMedicationPats(PrefC.GetString(PrefName.RegistrationKey),listMedicationPatms.ToArray());
							break;
						case SynchEntity.allergy:
							List<Allergym> listAllergyms=Allergyms.GetMultAllergyms(listBlockPKNums);
							_mobile.SynchAllergies(PrefC.GetString(PrefName.RegistrationKey),listAllergyms.ToArray());
							break;
						case SynchEntity.allergydef:
							List<AllergyDefm> listAllergyDefms=AllergyDefms.GetMultAllergyDefms(listBlockPKNums);
							_mobile.SynchAllergyDefs(PrefC.GetString(PrefName.RegistrationKey),listAllergyDefms.ToArray());
							break;
						case SynchEntity.disease:
							List<Diseasem> listDiseasems=Diseasems.GetMultDiseasems(listBlockPKNums);
							_mobile.SynchDiseases(PrefC.GetString(PrefName.RegistrationKey),listDiseasems.ToArray());
							break;
						case SynchEntity.diseasedef:
							List<DiseaseDefm> listDiseaseDefms=DiseaseDefms.GetMultDiseaseDefms(listBlockPKNums);
							_mobile.SynchDiseaseDefs(PrefC.GetString(PrefName.RegistrationKey),listDiseaseDefms.ToArray());
							break;
						case SynchEntity.icd9:
							List<ICD9m> listICD9ms=ICD9ms.GetMultICD9ms(listBlockPKNums);
							_mobile.SynchICD9s(PrefC.GetString(PrefName.RegistrationKey),listICD9ms.ToArray());
							break;
						case SynchEntity.statement:
							List<Statementm> listStatementms=Statementms.GetMultStatementms(listBlockPKNums);
							_mobile.SynchStatements(PrefC.GetString(PrefName.RegistrationKey),listStatementms.ToArray());
							break;
						case SynchEntity.document:
							List<Documentm> listDocumentms=Documentms.GetMultDocumentms(listBlockPKNums,AtoZpath);
							_mobile.SynchDocuments(PrefC.GetString(PrefName.RegistrationKey),listDocumentms.ToArray());
							break;
						case SynchEntity.recall:
							List<Recallm> listRecallms=Recallms.GetMultRecallms(listBlockPKNums);
							_mobile.SynchRecalls(PrefC.GetString(PrefName.RegistrationKey),listRecallms.ToArray());
							break;
						case SynchEntity.deletedobject:
							List<DeletedObject> listDeleteObjects=DeletedObjects.GetMultDeletedObjects(listBlockPKNums);
							_mobile.DeleteObjects(PrefC.GetString(PrefName.RegistrationKey),listDeleteObjects.ToArray());
							break;
						case SynchEntity.patientdel:
							_mobile.DeletePatientsRecords(PrefC.GetString(PrefName.RegistrationKey),listBlockPKNums.ToArray());
							break;
					}
					//progressIndicator.CurrentVal+=LocalBatchSize;//not allowed
					currentVal+=localBatchSize;
					if(Application.OpenForms["FormProgress"]!=null) {// without this line the following error is thrown: "Invoke or BeginInvoke cannot be called on a control until the window handle has been created." or a null pointer exception is thrown when an automatic synch is done by the system.
						FormP.Invoke(new PassProgressDelegate(PassProgressToDialog),
							new object[] { currentVal,"?currentVal of ?maxVal records uploaded",totalCount,"" });
					}
				}
				catch(Exception e) {
					if(_isTroubleshootMode) {
						string errorMessage=entity+ " with Primary Key = "+listBlockPKNums[0].ToString()+" failed to synch. "+"\n"+e.Message;
						throw new Exception(errorMessage);
					}
					else {
						throw e;
					}
				}
			}//for loop ends here
			*/
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
					_mobile.DeleteObjects(PrefC.GetString(PrefName.RegistrationKey),dO.ToArray()); //dennis check this - why is it not done in batches.
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
				_mobile.Url=PrefC.GetString(PrefName.MobileSyncServerURL);
				if(_mobile.ServiceExists()) {
					return true;
				}
			}
			catch {
				return false;
			}
			return false;
		}

		private bool VerifyPaidCustomer() {
			//if(textMobileSyncServerURL.Text.Contains("192.168.0.196") || textMobileSyncServerURL.Text.Contains("localhost")) {
			if(textMobileSyncServerURL.Text.Contains("10.10.1.196")||textMobileSyncServerURL.Text.Contains("localhost")) {
				IgnoreCertificateErrors();
			}
			bool isPaidCustomer=_mobile.IsPaidCustomer(PrefC.GetString(PrefName.RegistrationKey));
			if(!isPaidCustomer) {
				textSynchMinutes.Text="0";
				Prefs.UpdateInt(PrefName.MobileSyncIntervalMinutes,0);
				MsgBox.Show(this,"This feature requires a separate monthly payment.  Please call customer support.");
				return false;
			}
			return true;
		}

		///<summary>Called from FormOpenDental and from FormEhrOnlineAccess.  doForce is set to false to follow regular synching interval.</summary>
		public static void SynchFromMain(bool doForce) {
			if(Application.OpenForms["FormPatientPortalSetup"]!=null) {//tested.  This prevents main synch whenever this form is open.
				return;
			}
			if(_isSynching) {
				return;
			}
			DateTime dateTimeSynchStarted=MiscData.GetNowDateTime();
			if(!doForce) {//if doForce, we skip checking the interval
				if(dateTimeSynchStarted<PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun).AddMinutes(PrefC.GetInt(PrefName.MobileSyncIntervalMinutes))) {
					return;
				}
			}
			//if(PrefC.GetString(PrefName.MobileSyncServerURL).Contains("192.168.0.196") || PrefC.GetString(PrefName.MobileSyncServerURL).Contains("localhost")) {
			if(PrefC.GetString(PrefName.MobileSyncServerURL).Contains("10.10.1.196")||PrefC.GetString(PrefName.MobileSyncServerURL).Contains("localhost")) {
				IgnoreCertificateErrors();
			}
			if(!TestWebServiceExists()) {
				if(!doForce) {//if being used from FormOpenDental as part of timer
					if(_isServerAvail) {//this will only happen the first time to prevent multiple windows.
						_isServerAvail=false;
						DialogResult res=MessageBox.Show("Mobile synch server not available.  Synch failed.  Turn off synch?","",MessageBoxButtons.YesNo);
						if(res==DialogResult.Yes) {
							Prefs.UpdateInt(PrefName.MobileSyncIntervalMinutes,0);
						}
					}
				}
				return;
			}
			else {
				_isServerAvail=true;
			}
			DateTime dateChangedSince=PrefC.GetDateT(PrefName.MobileSyncDateTimeLastRun);
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => UploadWorker(dateChangedSince,dateTimeSynchStarted);
			progressOD.StartingMessage="Preparing records for upload.";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
				//return;
			}
			//if(progressOD.IsCancelled){
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		#region Testing
		///<summary>This allows the code to continue by not throwing an exception even if there is a problem with the security certificate.</summary>
		private static void IgnoreCertificateErrors() {
			System.Net.ServicePointManager.ServerCertificateValidationCallback+=
			delegate (object sender,System.Security.Cryptography.X509Certificates.X509Certificate certificate,
									System.Security.Cryptography.X509Certificates.X509Chain chain,
									System.Net.Security.SslPolicyErrors sslPolicyErrors) {
										return true;
									};
		}

		/// <summary>For testing only</summary>
		private static void CreatePatients(int PatientCount) {
			for(int i = 0;i<PatientCount;i++) {
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
			long[] arrayPatNums=Patients.GetAllPatNums(true);
			DateTime dateAppt= DateTime.Now;
			for(int i = 0;i<arrayPatNums.Length;i++) {
				dateAppt=dateAppt.AddMinutes(20);
				for(int j = 0;j<AppointmentCount;j++) {
					Appointment appointment=new Appointment();
					dateAppt=dateAppt.AddMinutes(20);
					appointment.PatNum=arrayPatNums[i];
					appointment.DateTimeArrived=dateAppt;
					appointment.DateTimeAskedToArrive=dateAppt;
					appointment.DateTimeDismissed=dateAppt;
					appointment.DateTimeSeated=dateAppt;
					appointment.AptDateTime=dateAppt;
					appointment.Note="some notenote noten otenotenot enotenot enote"+j;
					appointment.IsNewPatient=true;
					appointment.ProvNum=3;
					appointment.AptStatus=ApptStatus.Scheduled;
					appointment.AptDateTime=dateAppt;
					appointment.Op=2;
					appointment.Pattern="//XX//////";
					appointment.ProcDescript="4-BWX";
					appointment.ProcsColored="<span color=\"-16777216\">4-BWX</span>";
					Appointments.Insert(appointment);
				}
			}
		}

		/// <summary>For testing only</summary>
		private static void CreatePrescriptions(int PrescriptionCount) {
			long[] arrayPatNums=Patients.GetAllPatNums(true);
			for(int i = 0;i<arrayPatNums.Length;i++) {
				for(int j = 0;j<PrescriptionCount;j++) {
					RxPat rxpat= new RxPat();
					rxpat.Drug="VicodinA VicodinB VicodinC"+j;
					rxpat.Disp="50.50";
					rxpat.IsControlled=true;
					rxpat.PatNum=arrayPatNums[i];
					rxpat.RxDate=new DateTime(2010,12,1,11,0,0);
					RxPats.Insert(rxpat);
				}
			}
		}

		private static void CreateStatements(int StatementCount) {
			long[] arrayPatNums=Patients.GetAllPatNums(true);
			for(int i = 0;i<arrayPatNums.Length;i++) {
				for(int j = 0;j<StatementCount;j++) {
					Statement statement= new Statement();
					statement.DateSent=new DateTime(2010,12,1,11,0,0).AddDays(1+j);
					statement.DocNum=i+j;
					statement.PatNum=arrayPatNums[i];
					Statements.Insert(statement);
				}
			}
		}
		#endregion Testing
		
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
	}
}