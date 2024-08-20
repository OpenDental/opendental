using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTestsCore;
using OpenDentBusiness;
using CodeBase;
using System.Data;
using PdfSharp.Pdf;
using System.IO;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using OpenDental;
using DataConnectionBase;

namespace UnitTests.Billing_Tests {
	///<summary>FormBilling was refactored for 24.2 and business logic was extracted to ODBiz\Logic\BillingL.cs.
	///This allows billing to be shared by OpenDentalService and also allows for unit testing (this file).
	///Scroll to bottom of this file for a writeup on all of the previously unreported bug/behaviors that were fixed through the course of this refactor.</summary>
	[TestClass]
	public class BillingTests:TestBase {
		private static LogWriterBillingTest _logWriterBilling;
		
		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			_logWriterBilling=_logWriterBilling??new LogWriterBillingTest(_log);
			ClearDbBilling();
		}

		private static void ClearDbBilling() {
			PatientT.ClearPatientTable();
			StatementT.ClearStatementTable();
			ProcedureT.ClearProcedureTable();
			ClinicT.ClearClinicTable();
			CommlogT.ClearCommLogTable();
			SmsToMobileT.ClearSmsToMobileTable();
			SecurityLogT.ClearSecurityLogsByPermType(EnumPermType.Billing);
			//Generate pdf if eBilling is on by default.
			PrefT.UpdateBool(PrefName.BillingElectCreatePDF,true);
			//eBilling off by default.
			PrefT.UpdateInt(PrefName.BillingUseElectronic,(int)BillingUseElectronicEnum.None);
			PrefT.UpdateBool(PrefName.BillingEmailIncludeAutograph,false);
			//Set to valid path so pdf will save.
			PrefT.UpdateString(PrefName.BillingElectStmtOutputPathPos,PrefC.GetTempFolderPath());
			PrefT.UpdateString(PrefName.BillingElectStmtOutputPathEds,PrefC.GetTempFolderPath());
			//Mock webservicehq. We will need it for GenerateShortGUIDs when we try to text statements.
			WebServiceMainHQProxy.MockWebServiceMainHQ=new WebServiceMainHQMockDemo() { };
			ClinicPrefT.ClearClinicPrefTable();
			EmailAutographT.ClearEmailAutographTable();
			EmailAddressT.ClearEmailAddressTable();
			Ebill ebill=Ebills.GetForClinic(0);
			ebill.RemitAddress=EbillAddress.PracticeBilling;
			OpenDentBusiness.Crud.EbillCrud.Update(ebill);
			PrefT.UpdateString(PrefName.PracticeBillingAddress,"addr1");
			PrefT.UpdateString(PrefName.PracticeBillingAddress2,"addr2");
			PrefT.UpdateString(PrefName.PracticeBillingCity,"city");
			PrefT.UpdateString(PrefName.PracticeBillingST,"OR");
			PrefT.UpdateString(PrefName.PracticeBillingZip,"97333");
			PrefT.UpdateString(PrefName.PracticeBillingPhone,"2345678901");
			//Tired of being prompted when I debug OD proper.
			Prefs.UpdateDateT(PrefName.BackupReminderLastDateRun,DateTime.Today);
			PrefT.UpdateBool(PrefName.PrintStatementsAlphabetically,false);
			//Run aging by default.
			PrefT.UpdateString(PrefName.DateLastAging,POut.Date(DateTime_.Today.AddDays(-1),false));
			//Clear again lock .
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");
			//Clear pay portal text message statement modes. Set in SetupBilling() for each statement mode if desired.
			Prefs.UpdateString(PrefName.BillingDefaultsModesToText,"");
			//BillingType defs will be setup in BillingTests.SetupBilling() since we need those in the same place we create patients.
		}

		///<summary>Assert that Statements.GetFamiliesForStatements is ok with dupe statement. 
		///Code looks like it should throw for dupe dictionary entries but Patients.GetFamilies de-dupes before that can happen.</summary>
		[TestMethod]
		public void Billing_GetFamiliesForStatements_DuplicatePatientStatement() {
			BillingSetup billingSetup=SetupBilling(false,numPatsPerFam:3,numStatementsPerPat:3);
			try {
				//Assert that dictionary code allows dupe patient statements.
				Statements.GetFamiliesForStatements(billingSetup.ListStatements);
			}
			catch(ArgumentException e) {
				Assert.Fail(e.Message);
			}			
		}

		///<summary>Assert that statement order is consistent when clinics are off and not consistent when clinics are on.</summary>
		[TestMethod]
		public void Billing_StatementOrder_ClinicsNO() {
			void assert(bool doSortByPatName) {
				ClearDbBilling();
				PrefT.UpdateBool(PrefName.PrintStatementsAlphabetically,doSortByPatName);
				BillingSetup billingSetup=SetupBilling(false,eBillingType:BillingUseElectronicEnum.EDS,numFamsPerClinic:3,numPatsPerFam:3,numStatementsPerPat:1,numClinics:3);
				DataTable dataTableUI=Statements.GetBilling(false,0,DateTime.MinValue,new DateTime(2200,1,1),new List<long>());
				List<long> listStatementNumsUI=dataTableUI.AsEnumerable().Select(x => PIn.Long(x["StatementNum"].ToString())).ToList();
				SendStatementsIO sendStatementsIO=new SendStatementsIOTest(listStatementNumsToSend:listStatementNumsUI);
				bool ret=BillingL.SendStatements(sendStatementsIO);
				//Billing is ok.
				Assert.IsTrue(ret);
				//Clinics are OFF so input/output statement order will match. PrintStatementsAlphabetically pref will always be disregarded.
				Assert.IsTrue(ListTools.TryCompareList(listStatementNumsUI,sendStatementsIO.ListStatementNumsSent,doEnforceOrderMatch: true));				
			}
			assert(false);
			assert(true);
		}

		///<summary>Assert that statement order is alphabetical when PrintStatementsAlphabetically pref is on and otherwise in grid sorted order for each clinic.</summary>
		[TestMethod]
		public void Billing_StatementOrder_ClinicsYES() {
			void assert(bool doSortByPatName) {
				ClearDbBilling();
				PrefT.UpdateBool(PrefName.PrintStatementsAlphabetically,doSortByPatName);
				BillingSetup billingSetup=SetupBilling(true,eBillingType:BillingUseElectronicEnum.EDS,numFamsPerClinic:5,numPatsPerFam:5,numStatementsPerPat:1,numClinics:3);
				DataTable dataTableUI=Statements.GetBilling(false,0,DateTime.MinValue,new DateTime(2200,1,1),new List<long>());
				List<long> listStatementNumsUI=dataTableUI.AsEnumerable().Select(x => PIn.Long(x["StatementNum"].ToString())).ToList();
				SendStatementsIO sendStatementsIO=new SendStatementsIOTest(listStatementNumsToSend:listStatementNumsUI);
				bool ret=BillingL.SendStatements(sendStatementsIO);
				//Billing is ok.
				Assert.IsTrue(ret);
				foreach(StatementBatch batch in sendStatementsIO.ListStatementBatches) {
					if(doSortByPatName) {
						//Alpha sorting. Order will change to match Pat LName, FName sort.	
						//Capture the patient order that statements were sent.
						List<Patient> listAllPats=billingSetup.ListFamilies.SelectMany(x => x.ListPats).ToList();
						List<Patient> listPatsInBatchInSendOrder=batch.ListStatements.Select(x => listAllPats.First(y => y.PatNum==x.PatNum)).ToList();
						//Order by LName, FName
						List<Patient> listPatsInBatchInAlphaOrder=listPatsInBatchInSendOrder.OrderBy(x => x.LName).ThenBy(x => x.LName).ToList();
						//Should be no change in order from send order to alpha order.
						Assert.IsTrue(ListTools.TryCompareList(listPatsInBatchInSendOrder,listPatsInBatchInAlphaOrder,(x,y) => x.PatNum==y.PatNum, doEnforceOrderMatch: true));
					}
					else { 
						//Statements from each clinic have now been grouped together so sorting is non determinant.
						List<long> listBatchStatementNumsSent=batch.ListStatements.Select(x => x.StatementNum).ToList();
						List<long> listBatchStatementNumsToSend=listStatementNumsUI.GetRange(listStatementNumsUI.IndexOf(listBatchStatementNumsSent[0]),listBatchStatementNumsSent.Count);
						//No longer sorted the same as in the grid.						
						Assert.IsFalse(ListTools.TryCompareList(listBatchStatementNumsToSend,listBatchStatementNumsSent,doEnforceOrderMatch: true));
					}
				}
			}
			assert(true);
			assert(false);			
		}

		///<summary>When clinics on, each patient's guarantor's clinic is a batch. No size limit on batches.
		///This is probably a bug as clinic-less batching is limited by eBilling vendor, no such limit for clinics.</summary>
		[TestMethod]
		public void Billing_GetBatchesForStatements_ClinicsYES() {
			void assert(BillingUseElectronicEnum eBillingType) {
				//Batch size limit is less than count statements.
				PrefT.UpdateInt(PrefName.BillingElectBatchMax,4);
				BillingSetup billingSetup=SetupBilling(true,eBillingType:eBillingType,numFamsPerClinic:3,numPatsPerFam:3,numStatementsPerPat:1,numClinics:3);
				List<StatementBatch> listStatementBatches=Statements.GetBatchesForStatements(
					billingSetup.ListStatements,billingSetup.ListFamilies.SelectMany(x => x.ListPats).ToList());
				//1 batch per clinic regardless of eBilling vendor.
				Assert.AreEqual(billingSetup.ListClinics.Count,listStatementBatches.Count);
				//Each clinic has 1 batch.
				Assert.IsTrue(ListTools.TryCompareList(billingSetup.ListClinics.Select(x => x.ClinicNum).ToList(),listStatementBatches.Select(x => x.ClinicNum).ToList()));
				//Each batch has 1 statement per family member in the clinic.
				Assert.IsTrue(listStatementBatches.All(x => x.ListStatements.Count==billingSetup.ListFamilies.SelectMany(y => y.ListPats).Count(y => y.ClinicNum==x.ClinicNum)));
			}
			//eBilling vendor does not matter. Batches are all based on clinic.
			assert(BillingUseElectronicEnum.EHG);
			assert(BillingUseElectronicEnum.EDS);
		}

		///<summary>When clinics off, batching is limited by eBilling vendor. Make batch that is larger than batch pref (multiple batches).</summary>
		[TestMethod]
		public void Billing_GetBatchesForStatements_ClinicsNO() {
			void assert(BillingUseElectronicEnum eBillingType,List<int> listBatchNumsExpected) {
				//Batch size limit is less than count statements.
				PrefT.UpdateInt(PrefName.BillingElectBatchMax,4);
				BillingSetup billingSetup=SetupBilling(false,eBillingType:eBillingType,numFamsPerClinic:3,numPatsPerFam:3,numStatementsPerPat:1);
				List<StatementBatch> listStatementBatches=Statements.GetBatchesForStatements(
				billingSetup.ListStatements,billingSetup.ListFamilies.SelectMany(x => x.ListPats).ToList());
				//Number of batches depends on eBilling provider.
				Assert.AreEqual(listBatchNumsExpected.Count,listStatementBatches.Count);
				//No clinic num defined.
				Assert.IsTrue(listStatementBatches.All(x => x.ClinicNum==0));
				//1-based batch num.
				Assert.IsTrue(ListTools.TryCompareList(listStatementBatches.Select(x => x.BatchNum).ToList(),listBatchNumsExpected));
				//All statements in are in the batch.
				Assert.IsTrue(ListTools.TryCompareList(
					billingSetup.ListStatements,listStatementBatches.SelectMany(x => x.ListStatements).ToList(),(x,y) => { return x.StatementNum==y.StatementNum; }));
			}
			//EHG enforces batch size.
			assert(BillingUseElectronicEnum.EHG,new List<int> { 1,2,3 });
			//EDS does not enforce batch size.
			assert(BillingUseElectronicEnum.EDS,new List<int> { 1 });
		}

		///<summary>Assert that BillingL.RunAgingEnterprise enters security logs properly.</summary>
		[TestMethod]
		public void Billing_RunAging_SecurityLog_Source() {
			SecurityLogT.ClearSecurityLogsByPermType(EnumPermType.AgingRan);
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest();
			BillingL.RunAgingEnterprise(sendStatementsIO);
			SQLWhere sQLWhere=SQLWhere.CreateIn(nameof(SecurityLog.PermType),new List<int>{ (int)EnumPermType.AgingRan });
			List<SecurityLog> listSecurityLogs =SecurityLogs.GetMany(sQLWhere);
			Assert.AreEqual(2,listSecurityLogs.Count);
			Assert.IsTrue(listSecurityLogs.All(x => x.LogText.Contains(sendStatementsIO.Source)));
		}

		///<summary>Pause billing, delete some statements, resume billing. Assert that the deleted statements are skipped after resume.</summary>
		[TestMethod]
		public void Billing_PauseResume() {
			void assertPauseResume(List<long> listToSendBeforePause,List<long> listSentBeforePause,List<long> listToSendAfterPause,List<long> listToSkip) {
				bool isPaused=true;
				SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
					//Starting list.
					listStatementNumsToSend:listToSendBeforePause,
					//Mock sending these before pause.
					listStatementNumsSent:listSentBeforePause,
					functGetIsPaused: () => { return isPaused; },
					//Only pause once.
					actionSleepDuringPause:() => { isPaused=false; },
					funcGetIsCancelled:() => { return false; },
					funcGetBillingDataTable:() => {
						DataTable table=new DataTable();
						table.Columns.Add("StatementNum");
						//Add remaining statement nums.
						foreach (long statementNum in listToSendAfterPause){
							DataRow dataRow=table.NewRow();
							dataRow["StatementNum"]=POut.Long(statementNum);
							table.Rows.Add(dataRow);
						}
						return table;
					}
				);
				BillingL.BillingProgressPause(sendStatementsIO);
				//Skip only those that were not deleted.
				Assert.IsTrue(ListTools.TryCompareList(sendStatementsIO.ListStatementNumsToSkipAfterPause,listToSkip));
			}
			assertPauseResume(
				listToSendBeforePause: new List<long>() { 1,2,3,4 },
				listSentBeforePause: new List<long>() { 1, 4 },
				listToSendAfterPause: new List<long>() { 2 },
				//Skip 1 remaining.
				listToSkip: new List<long>() { 3 });
			assertPauseResume(
				listToSendBeforePause: new List<long>() { 1,2,3,4 },
				listSentBeforePause: new List<long>() { 1,4 },
				listToSendAfterPause: new List<long>() { 2,3 },
				//Skip none remaining.
				listToSkip: new List<long>());
			assertPauseResume(
				listToSendBeforePause: new List<long>() { 1,2,3,4 },
				listSentBeforePause: new List<long>() { 1,4 },
				listToSendAfterPause: new List<long>(),
				//Skip all remaining.
				listToSkip: new List<long>() { 2,3 });
		}

		///<summary>Prove that we can order a list of statements according to a second list of StatementNums that are in no particular order.</summary>
		[TestMethod]
		public void Billing_OrderStatements() {
			int numItemsInGrid=100;
			//StatementNums may be in any order coming from grid.
			List<Statement> listFromGrid=Enumerable.Range(1,numItemsInGrid)
				.Select(x => new Statement(){StatementNum=ODRandom.Next(),TagOD=x, })
				.ToList();

			//Convert original unordered list to primary keys only.
			List<long> listStatementNumsFromGrid=listFromGrid.Select(x => x.StatementNum).ToList();			
			//Simulate our grid list being ordered in a differnt way by a query (happens to be numerically by PK).
			List<Statement> listStatmentsOrderedByQuery=listFromGrid.OrderBy(x => x.StatementNum).ToList();
			//Create a sorting dictionary by looping through originaly grid list and capture each PK and it's original position in the grid.
			Dictionary<long,int> dictionaryStatementsOrder=new Dictionary<long, int>();
			for(int i = 0;i<listFromGrid.Count;i++) {
				dictionaryStatementsOrder[listStatementNumsFromGrid[i]]=i;
			}
			//(previous to 24.2) Order the previously ordered list back to the grid order using the dictionary.
			List<Statement> listSortByDict=listStatmentsOrderedByQuery.OrderBy(x => dictionaryStatementsOrder[x.StatementNum]).ToList();
			//(24.2+) Order the previously ordered list back to the grid order using the ordinal position of each PK from the original grid list..
			List<Statement> listSortByList=listStatmentsOrderedByQuery.OrderBy(x => listStatementNumsFromGrid.IndexOf(x.StatementNum)).ToList();
			//Assert that both sort methods yield the same order, which is the order of the original grid list.
			bool isQueryOutOfOrder=false;
			for(int i = 0;i<listSortByDict.Count;i++) {
				Assert.AreEqual(listSortByDict[i].StatementNum,listFromGrid[i].StatementNum);
				Assert.AreEqual(listSortByDict[i].StatementNum,listSortByList[i].StatementNum);
				//All items may not be out of order, but at least some should be.
				if(listSortByDict[i].StatementNum!=listStatmentsOrderedByQuery[i].StatementNum) {
					isQueryOutOfOrder=true;
				}
			}
			Assert.IsTrue(isQueryOutOfOrder);
		}

		///<summary>Answer no when prompted to run all billing to run for all history.</summary>
		[TestMethod]
		public void Billing_Popup_ContinueNO() {
			//Popup only shows for eBilling.
			BillingSetup billingSetup=SetupBilling(false,eBillingType:BillingUseElectronicEnum.EHG);
			bool didAsk=false;
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				funcGetIsHistoryStartMinDate:() =>{ didAsk=true; return true; },
				//Answer no when prompted to halt.
				funcAskQuestion: (s) =>{ return false; },
				//We shouldn't make it to first cancel request.
				funcGetIsCancelled:() =>{ Assert.Fail(); return true; });
			bool ret=BillingL.SendStatements(sendStatementsIO);
			//Billing failed entirely.
			Assert.IsFalse(ret);
			Assert.IsTrue(didAsk);
		}

		///<summary>Answer yes when prompted to run all billing to run for all history.</summary>
		[TestMethod]
		public void Billing_Popup_ContinueYES() {
			//Popup only shows for eBilling.
			BillingSetup billingSetup=SetupBilling(false,eBillingType:BillingUseElectronicEnum.EHG);
			bool didAsk=false;
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				funcGetIsHistoryStartMinDate:() =>{ didAsk=true; return true; },
				//Test is over so cancel on first cancel request.
				funcGetIsCancelled:() =>{ return true; });
			BillingL.SendStatements(sendStatementsIO);
			Assert.IsTrue(didAsk);
			List<SecurityLog> listAudits =SecurityLogs.Refresh(0,new List<EnumPermType> { EnumPermType.Billing},0).ToList();
			Assert.AreEqual(1, listAudits.Count);
		}

		///<summary>Practice/clinic email used to send Email statements is not valid. Do not attempt billing.</summary>
		[TestMethod]
		public void Billing_BadEmailSender() {
			BillingSetup billingSetup=SetupBilling(false,funcGetStatementMode:(a,b,c) =>{ return StatementMode.Email; });
			bool didAskForEmail=false;
			bool didContinueAfterFail=false;
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				funcGetSenderEmailAddress:(clinicNum) =>{ didAskForEmail=true; return null; },
				actionProgressEvent:(odEvent) => {
					if(odEvent.Tag is ProgressBarHelper pbh){
						didContinueAfterFail|=pbh.LabelValue.Contains("E-Bill");
					}
				});
			bool ret=BillingL.SendStatements(sendStatementsIO);
			//Billing failed entirely.
			Assert.IsFalse(ret);
			Assert.IsTrue(didAskForEmail);
			Assert.IsFalse(didContinueAfterFail);
		}

		///<summary>Practice/clinic email used to send Email statements is ok. Patient email is not valid. Continue billing, just skip patient.</summary>
		[TestMethod]
		public void Billing_BadEmailReceiver() {
			BillingSetup billingSetup=SetupBilling(false,numPatsPerFam:2,
				funcGetStatementMode:(a,b,c) =>{ return StatementMode.Email; },
				//1 valid pat email, 1 invalid pat email.
				funcGetPatientEmailAddress:(a,b,c) => { return c==0 ? "" : "pat@mail.com"; });
			bool didContinueAfterFail=false;
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				actionProgressEvent:(odEvent) => {
					if(odEvent.Tag is ProgressBarHelper pbh){
						didContinueAfterFail|=pbh.LabelValue.Contains("E-Bill");
					}
				});
			bool ret=BillingL.SendStatements(sendStatementsIO);
			//Billing is ok, 1 patient is bad.
			Assert.IsTrue(ret);			
			Assert.IsTrue(didContinueAfterFail);
			//Skipped 1.
			Assert.AreEqual(1,sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.BadEmailAddress));
			//Emailed 1.
			Assert.AreEqual(1,sendStatementsIO.CountStatementsEmailed);
		}

		///<summary>Statements generate ok. 1 patient wants sms, 1 does not.</summary>
		[TestMethod]
		public void Billing_SmsSendStatus() {
			BillingSetup billingSetup=SetupBilling(false,numPatsPerFam:2,
				funcGetStatementMode:(a,b,c) =>{ return StatementMode.Email; },
				funcGetSmsSendStatus:(a,b,c,d) => {
					return c==0 ? AutoCommStatus.SendNotAttempted : AutoCommStatus.DoNotSend;
				});
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList());
			bool ret=BillingL.SendStatements(sendStatementsIO);
			//Billing is ok, 1 patient is bad.
			Assert.IsTrue(ret);
			//No skip.
			Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count);
			//Emailed 2.
			Assert.AreEqual(2,sendStatementsIO.CountStatementsEmailed);
			//Only text 1.
			Assert.AreEqual(1,sendStatementsIO.CountStatmentsSentPayPortalText);
			//Only 1 text sent.
			List<SmsToMobile> listSmsToMobiles=SmsToMobileT.GetAll();
			Assert.AreEqual(1,listSmsToMobiles.Count);
			//Only 1 comm log.
			List<Commlog> listCommLogs=CommlogT.GetAll();
			Assert.AreEqual(1,listCommLogs.Count);
			//1 sms skipped, 1 sms sent ok.
			List<Statement> listStatements=StatementT.GetAllStatements();
			Assert.AreEqual(1,listStatements.Count(x => x.SmsSendStatus==AutoCommStatus.DoNotSend));
			Assert.AreEqual(1,listStatements.Count(x => x.SmsSendStatus==AutoCommStatus.SendSuccessful));
			//No patients skipped for bad sms setup (only for DoNotSend SmsSendStatus).
			Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.BadSmsSetup));
		}

		///<summary>Use secure email when clinic is setup, otherwise use non secure email. Skip patients who fail to email. Assert email autograph gets inserted when applicable.</summary>
		[TestMethod]
		public void Billing_HappyPath_Email() {			
			void assert(bool doIncludeAutograph,bool isHtmlAutograph) {
				ClearDbBilling();
				PrefT.UpdateBool(PrefName.BillingEmailIncludeAutograph,doIncludeAutograph);
				EmailAutographT.ClearEmailAutographTable();
				EmailAddressT.ClearEmailAddressTable();
				BillingSetup billingSetup=SetupBilling(true,numPatsPerFam:1,numClinics:3,isHtmlEmailAutograph:isHtmlAutograph,
				funcGetStatementMode:(a,b,c) =>{ return StatementMode.Email; },
				//1 clinic uses secure, other 2 not secure.
				funcGetClinicUsesSecureEmail:(clinicIndex) =>{ return clinicIndex==0; },
				//must set clinic 0 to use secure statements in order for billing statements to be sent securely over email
				funcGetClinicUsesSecureStatements: (clinicIndex) =>{return clinicIndex==0;});
				SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				actionSendEmail:(clinicNum,emailMessage,emailAddress,isSecure) => {
					int clinicIndex=billingSetup.ListClinics.FindIndex(x => x.ClinicNum==clinicNum);
					bool hasAutograph=emailMessage.BodyText.ToLower().Contains(billingSetup.ListEmailAutographs[clinicIndex].AutographText.ToLower());
					bool hasHtmlAutograph=hasAutograph && MarkupEdit.ContainsOdHtmlTags(emailMessage.HtmlText) && MarkupEdit.ContainsOdHtmlTags(emailMessage.BodyText);
					Assert.AreEqual(doIncludeAutograph,hasAutograph);
					Assert.AreEqual(isHtmlAutograph,hasHtmlAutograph);
					if(clinicIndex==0){
						Assert.IsTrue(isSecure);
						Assert.AreEqual(EmailSentOrReceived.SecureEmailSent,emailMessage.SentOrReceived);
					}
					else {
						Assert.IsFalse(isSecure);
						Assert.AreEqual(EmailSentOrReceived.Sent,emailMessage.SentOrReceived);
					}
					if(clinicIndex==2){ //Fail this email so we can assert that logic happened below.
						throw new Exception("Email failed (test)!");
					}
				});
				bool ret=BillingL.SendStatements(sendStatementsIO);
				Assert.IsTrue(ret);
				//1 failed email.
				Assert.AreEqual(1,sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.Misc));
				//Emailed 2.
				Assert.AreEqual(2,sendStatementsIO.CountStatementsEmailed);
				//All statements sent Email.
				Assert.AreEqual(0,sendStatementsIO.ListStatementBatches.Sum(x => x.ListEbillStatements.Count));
				Assert.AreEqual(0,sendStatementsIO.CountStatementsSentElectronic);
				Assert.AreEqual(0,sendStatementsIO.CountStatementsPrinted);
				//Sent 2 (1 failed).
				Assert.AreEqual(2,sendStatementsIO.ListStatementNumsSent.Count);
				//No patients added to the skip list.
				List<Statement> listStatementsAfter=StatementT.GetAllStatements();
				Assert.IsTrue(ListTools.TryCompareList(listStatementsAfter,billingSetup.ListStatements,(x,y) => { return x.StatementNum==y.StatementNum; }));
				//All statements sent.
				Assert.AreEqual(2,listStatementsAfter.Count(x => x.IsSent));
				//All text messages sent.
				Assert.AreEqual(2,sendStatementsIO.CountStatmentsSentPayPortalText);
				Assert.AreEqual(2,listStatementsAfter.Count(x => x.SmsSendStatus==AutoCommStatus.SendSuccessful));
				//No Master pdf generated for Email.
				Assert.IsNull(sendStatementsIO.PdfMasterDocument);
				//No xml file for Email.
				Assert.IsTrue(string.IsNullOrEmpty(sendStatementsIO.XmlFilePath));
				//Each statements gets a document in Email.
				Assert.IsTrue(sendStatementsIO.ListStatementBatches.SelectMany(x => x.ListStatements).All(x => x.DocNum>0));
			}
			assert(true,true);
			assert(true,false);
			assert(false,false);
			//false,true is not a valid test and would assert. skip it.
		}

		///<summary>Send multiple batches (1 per clinic) and assert that each batch is sent independently and reporting callback only happens once total.</summary>
		[TestMethod]
		public void Billing_HappyPath_Batches() {
			BillingSetup billingSetup=SetupBilling(true,numClinics:3);
			//Skip one patient to assert skipped patient messaging works.
			CommOptOutT.Create(billingSetup.GetAllPats().First(),CommOptOutType.Statements,CommOptOutMode.Text);
			int batchesProcessed=0;
			int reportCallbacks=0;
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				funcAskQuestion:(prompt) =>{
					if(prompt.ToLower().Contains("printed:")){
						reportCallbacks++;
					}
					return true; 
				},
				actionProgressEvent:(odEventArgs) =>{ 
					if(!(odEventArgs.Tag is ProgressBarHelper)){
						 Assert.Fail();
					}
					ProgressBarHelper pbh=(ProgressBarHelper)odEventArgs.Tag;
					if(pbh.ProgressBarEventType!=ProgBarEventType.TextMsg){
						return;
					}
					if(pbh.LabelValue.ToLower().Contains("preparing batch")){
						batchesProcessed++;
					}
				},
				actionPrompt:(message,isCopyPasteable) =>{
					Assert.IsTrue(isCopyPasteable);					
				});
			bool ret=BillingL.SendStatements(sendStatementsIO);
			Assert.IsTrue(ret);
			//All batches processed.
			Assert.AreEqual(3,billingSetup.ListClinics.Count);
			Assert.AreEqual(3,sendStatementsIO.CurStatementBatch.BatchNum);
			Assert.AreEqual(1,reportCallbacks);
		}

		///<summary>Cancel file selection and assert that subsequent batches continue but no longer ask for file selection.</summary>
		[TestMethod]
		public void Billing_Electronic_XmlFile_Cancel() {			
			void assert(BillingUseElectronicEnum eBillingType) {
				ClearDbBilling();
				//Must be empty in order to be prompted for save location.
				PrefT.UpdateString(PrefName.BillingElectStmtOutputPathPos,"");
				PrefT.UpdateString(PrefName.BillingElectStmtOutputPathEds,"");
				BillingSetup billingSetup=SetupBilling(true,eBillingType:eBillingType,numClinics: 2);
				int countSaveFilePrompts=0;
				SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				funcChooseSaveFile: (initialSaveDirectory) => { countSaveFilePrompts++; return new ChooseSaveFile() { IsSelectionOk=false, }; });
				bool ret=BillingL.SendStatements(sendStatementsIO);
				Assert.IsTrue(ret);
				//All batches processed.
				Assert.AreEqual(billingSetup.ListClinics.Count,sendStatementsIO.CurStatementBatch.BatchNum);
				//Only ask once.
				Assert.AreEqual(1,countSaveFilePrompts);
				//Xml path never got set.
				Assert.IsTrue(string.IsNullOrEmpty(sendStatementsIO.XmlFilePath));
				Assert.IsTrue(sendStatementsIO.DidUserCancelXmlFileSelection);
				//No patients added to the skip list.
				Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count);
				List<Statement> listStatementsAfter=StatementT.GetAllStatements();
				Assert.IsTrue(ListTools.TryCompareList(listStatementsAfter,billingSetup.ListStatements,(x,y) => { return x.StatementNum==y.StatementNum; }));
				//No statements sent.
				Assert.IsTrue(listStatementsAfter.All(x => !x.IsSent));
				//No text messages sent.
				Assert.AreEqual(0,sendStatementsIO.CountStatmentsSentPayPortalText);
				//No eBills sent.
				Assert.AreEqual(0,sendStatementsIO.CountStatementsSentElectronic);
				//No statements sent.
				Assert.AreEqual(0,sendStatementsIO.ListStatementNumsSent.Count);
			}
			assert(BillingUseElectronicEnum.ClaimX);
			assert(BillingUseElectronicEnum.POS);
			assert(BillingUseElectronicEnum.EDS);
			//EHG does not create xml file.
		}

		///<summary>Send multiple batches (1 per clinic) and force a critical error in first batch. Assert that other batches do not run.</summary>
		[TestMethod]
		public void Billing_Batches_BadBatch() {
			BillingSetup billingSetup=SetupBilling(true,numClinics:3,funcGetStatementMode:(a,b,c) =>{ return StatementMode.Email; });
			int batchesProcessed=0;
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				//Invalid sender email is a critical error.
				funcGetSenderEmailAddress:(clinicNum) => {return new EmailAddress(){ SMTPserver="" }; },				
				actionProgressEvent:(odEventArgs) =>{
					if(!(odEventArgs.Tag is ProgressBarHelper)){
						Assert.Fail();
					}
					ProgressBarHelper pbh=(ProgressBarHelper)odEventArgs.Tag;
					if(pbh.ProgressBarEventType!=ProgBarEventType.TextMsg){
						return;
					}
					if(pbh.LabelValue.ToLower().Contains("preparing batch")){
						batchesProcessed++;
					}
				});
			bool ret=BillingL.SendStatements(sendStatementsIO);
			Assert.IsFalse(ret);
			//Only 1 batch processed.
			Assert.AreEqual(3,billingSetup.ListClinics.Count);
			Assert.AreEqual(1,sendStatementsIO.CurStatementBatch.BatchNum);
		}

		///<summary>Assert that DentalXChange 404 error does not mark statements sent..</summary>
		[TestMethod]
		public void Billing_Electronic_EHG_404Error() {
			void assert(bool useClinics) {
				ClearDbBilling();
				BillingSetup billingSetup=SetupBilling(useClinics,eBillingType:BillingUseElectronicEnum.EHG,numFamsPerClinic:2, numPatsPerFam:2, numClinics: 2);
				bool didAsk=false;
				SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
					listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				funcSendEhgStatement:(xml,clinicNum) => { didAsk=true; throw new Exception("(404) Not Found"); });
				bool ret=BillingL.SendStatements(sendStatementsIO);
				Assert.IsTrue(ret);
				Assert.IsTrue(didAsk);
				Assert.AreEqual(0,sendStatementsIO.CountStatementsSentElectronic);
				//No statements sent.
				Assert.AreEqual(0,sendStatementsIO.ListStatementNumsSent.Count);
				//eBilling was allowed to make an attempt.
				Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.ListStatementBatches.Sum(x => x.ListEbillStatements.Count));
				//Patients aren't added to the skip list. Only 1 skip item is added, it is for PatNum 0 / Misc.
				Assert.AreEqual(1,sendStatementsIO.ListSkippedPatients.Count);
				List<Statement> listStatementsAfter=StatementT.GetAllStatements();
				Assert.IsTrue(ListTools.TryCompareList(listStatementsAfter,billingSetup.ListStatements,(x,y) => { return x.StatementNum==y.StatementNum; }));
				//No statements sent.
				Assert.AreEqual(0,listStatementsAfter.Count(x => x.IsSent));
				//No text messages sent.
				Assert.AreEqual(0,sendStatementsIO.CountStatmentsSentPayPortalText);
				Assert.AreEqual(0,listStatementsAfter.Count(x => x.SmsSendStatus.In(AutoCommStatus.SendFailed,AutoCommStatus.SendSuccessful)));
			}
			assert(false);
			assert(true);
		}

		///<summary>Assert that no pdf is created when BillingElectCreatePDF pref is true.</summary>
		[TestMethod]
		public void Billing_Electronic_SkipPdf() {			
			PrefT.UpdateBool(PrefName.BillingElectCreatePDF,false);
			BillingSetup billingSetup=SetupBilling(false,eBillingType:BillingUseElectronicEnum.EHG);
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList());
			bool ret=BillingL.SendStatements(sendStatementsIO);
			Assert.IsTrue(ret);
			//All sent electronically.
			Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.CountStatementsSentElectronic);
			Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.ListStatementNumsSent.Count);
			Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.ListStatementBatches.Sum(x => x.ListEbillStatements.Count));
			//No statements skipped.
			Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count);
			List<Statement> listStatementsAfter=StatementT.GetAllStatements();
			Assert.IsTrue(ListTools.TryCompareList(listStatementsAfter,billingSetup.ListStatements,(x,y) => { return x.StatementNum==y.StatementNum; }));
			//All statements sent.
			Assert.IsTrue(listStatementsAfter.All(x => x.IsSent));
			//All text messages sent.
			Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.CountStatmentsSentPayPortalText);
			Assert.AreEqual(billingSetup.ListStatements.Count,listStatementsAfter.Count(x => x.SmsSendStatus.In(AutoCommStatus.SendSuccessful)));
			//No statements are linked to documents.
			Assert.IsTrue(sendStatementsIO.ListStatementBatches.SelectMany(x => x.ListStatements).All(x => x.DocNum==0));
		}

		///<summary>Loop through all eBilling types and test each for clinics on/off. Assert that all statements are sent and each eBillingType acts as designed.</summary>
		[TestMethod]
		public void Billing_HappyPath_Electronic() {
			void assert(bool useClinics,BillingUseElectronicEnum eBillingType) {
				ClearDbBilling();
				BillingSetup billingSetup=SetupBilling(useClinics,eBillingType:eBillingType,numFamsPerClinic:2, numPatsPerFam:2, numClinics: 2);
				List<long> listEhgApiClinicNums=new List<long>();
				SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
					listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
					funcSendEhgStatement: (xml,clinicNum) => {
						//Only ehg calls this method.
						Assert.AreEqual(BillingUseElectronicEnum.EHG,eBillingType);
						Assert.IsTrue(MiscTestUtils.ValidateXml(xml));
						listEhgApiClinicNums.Add(clinicNum);
						return ""; 
					});
				bool ret=BillingL.SendStatements(sendStatementsIO);
				Assert.IsTrue(ret);
				//All statements sent electronically.
				Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.ListStatementBatches.Sum(x => x.ListEbillStatements.Count));
				Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.CountStatementsSentElectronic);
				Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.ListStatementNumsSent.Count);
				//No patients added to the skip list.
				Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count);
				List<Statement> listStatementsAfter=StatementT.GetAllStatements();
				Assert.IsTrue(ListTools.TryCompareList(listStatementsAfter,billingSetup.ListStatements,(x,y) => { return x.StatementNum==y.StatementNum; }));
				//All statements sent.
				Assert.IsTrue(listStatementsAfter.All(x => x.IsSent));
				//All text messages sent.
				Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.CountStatmentsSentPayPortalText);
				Assert.IsTrue(listStatementsAfter.All(x => x.SmsSendStatus==AutoCommStatus.SendSuccessful));
				//No master pdf for electronic billing.
				Assert.IsNull(sendStatementsIO.PdfMasterDocument);
				//No email for electronic billing.
				Assert.AreEqual(0,sendStatementsIO.CountStatementsEmailed);
				if(eBillingType.In(BillingUseElectronicEnum.POS,BillingUseElectronicEnum.ClaimX,BillingUseElectronicEnum.EDS)) {
					//These types all create similar xml file.
					Assert.IsFalse(string.IsNullOrEmpty(sendStatementsIO.XmlFilePath));
				}
				else {
					//EHG uses web api and does not create xml file.
					Assert.IsTrue(string.IsNullOrEmpty(sendStatementsIO.XmlFilePath));
					//Each clinic (batch) triggered a single api call (EHG only).
					Assert.IsTrue(ListTools.TryCompareList(listEhgApiClinicNums,billingSetup.ListClinics.Select(x => x.ClinicNum).ToList(),(x,y) => { return x==y; }));
				}
			}
			//Run each eBilling type through happy path for clinics on and clinics off.
			List<BillingUseElectronicEnum> listEBillingTypes=Enum.GetValues(typeof(BillingUseElectronicEnum)).Cast<BillingUseElectronicEnum>().ToList();
			for(int i = 0;i<listEBillingTypes.Count;i++) {
				if(listEBillingTypes[i]==BillingUseElectronicEnum.None) {
					continue;
				}
				assert(false,listEBillingTypes[i]);
				assert(true,listEBillingTypes[i]);
			}
		}

		///<summary>Attempt to run aging when AgingBeginDateTime pref is already not cleared. Assert that billing did not proceed.</summary>
		[TestMethod]
		public void Billing_Aging_Busy() {
			PrefT.UpdateString(PrefName.DateLastAging,POut.Date(DateTime_.Today.AddDays(-30),false));
			Assert.AreNotEqual(PrefC.GetDate(PrefName.DateLastAging),DateTime_.Today);
			PrefT.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(DateTime_.Now,false));
			bool isTooFar=false;
			bool includesPrompt=false;
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				actionPrompt: (prompt,notUsed) => { includesPrompt=prompt.ToLower().Contains("in order to print or send"); },
				funcComputeAging:(dateTimeToday) => { isTooFar=true; return false; });
			bool ret=BillingL.SendStatements(sendStatementsIO);
			//Aging failed.
			Assert.IsFalse(ret);
			//User was prompted.
			Assert.IsTrue(includesPrompt);
			//FuncComputeAging should not be called since we failed ealier.
			Assert.IsFalse(isTooFar);
			//Aging timestamp was not moved forward.
			Assert.AreNotEqual(PrefC.GetDate(PrefName.DateLastAging),DateTime_.Today);
		}

		///<summary>Fail Ledgers.ComputeAging and assert that billing did not proceed.</summary>
		[TestMethod]
		public void Billing_Aging_Fail() {
			PrefT.UpdateString(PrefName.DateLastAging,POut.Date(DateTime_.Today.AddDays(-30),false));
			Assert.AreNotEqual(PrefC.GetDate(PrefName.DateLastAging),DateTime_.Today);
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(funcComputeAging:(dateTimeToday) => { return false; });
			bool ret=BillingL.SendStatements(sendStatementsIO);
			//Aging failed.
			Assert.IsFalse(ret);
			//Aging timestamp was not moved forward.
			Assert.AreNotEqual(PrefC.GetDate(PrefName.DateLastAging),DateTime_.Today);
		}

		///<summary>Assert aging is not due to run and billing is allowed to proceed.</summary>
		[TestMethod]
		public void Billing_Aging_HappyPath() {
			PrefT.UpdateString(PrefName.DateLastAging,POut.Date(DateTime_.Today,false));
			Assert.AreEqual(PrefC.GetDate(PrefName.DateLastAging),DateTime_.Today);
			bool didRunAging=false;
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(funcComputeAging:(dateTimeToday) => { didRunAging=true; return false; });
			bool ret=BillingL.SendStatements(sendStatementsIO);
			//Billing ok.
			Assert.IsTrue(ret);
			//Aging was not due to run.
			Assert.IsFalse(didRunAging);			
		}

		///<summary>Mail and InPerson statement mode function identically.
		///Assert that master pdf is created and various other conditions of InPerson and Mail statements.</summary>
		[TestMethod]
		public void Billing_HappyPath_InPersonAndMail() {
			void assert(bool useClinics,StatementMode statementMode) {
				ClearDbBilling();
				BillingSetup billingSetup=SetupBilling(useClinics,numPatsPerFam:2, numClinics: 2,
					funcGetStatementMode: (clinicIndex,famIndex,patIndex) => { return statementMode; });
				int totalPages=0;
				SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
					listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
					funcGetPdfDocument: (rawBase64,savedPdfPath) => {
						//Give each statement a random number of pages and capture the total count so we can assert later.
						int pages=ODRandom.Next(1,3);
						totalPages+=pages;
						//Make a pdf doc with a few pages.
						PdfDocument pdfDocument=new PdfDocument();
						XFont font=new XFont("Verdana", 20, XFontStyle.BoldItalic);
						for(int i = 1;i<=pages;i++) {
							PdfPage pdfPage=pdfDocument.AddPage();
							using(XGraphics gfx=XGraphics.FromPdfPage(pdfPage)){
								gfx.DrawString($"Page {i}", font, XBrushes.Black, new XRect(0, 0, pdfPage.Width, pdfPage.Height), XStringFormats.Center);
							}
						}
						//Save to a memstream and subsequently re-open a new pdf from the stream, this time using Import mode (which allows BillingL.PrintBatch to read from the pdf).
						using(MemoryStream ms=new MemoryStream()){
							pdfDocument.Save(ms, false);
							ms.Position = 0;
							pdfDocument=PdfReader.Open(ms,PdfDocumentOpenMode.Import);
						}
						return pdfDocument;						
					},
					actionSendEmail: (a,b,c,d) => { Assert.Fail("Should not send email for InPerson statement mode"); });
				bool ret=BillingL.SendStatements(sendStatementsIO);
				Assert.IsTrue(ret);
				//All statements sent InPerson.
				Assert.AreEqual(0,sendStatementsIO.ListStatementBatches.Sum(x => x.ListEbillStatements.Count));
				Assert.AreEqual(0,sendStatementsIO.CountStatementsSentElectronic);
				Assert.AreEqual(0,sendStatementsIO.CountStatementsEmailed);
				Assert.AreEqual(sendStatementsIO.ListStatementNumsSent.Count,sendStatementsIO.CountStatementsPrinted);
				Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.ListStatementNumsSent.Count);
				//No patients added to the skip list.
				Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count);
				List<Statement> listStatementsAfter=StatementT.GetAllStatements();
				Assert.IsTrue(ListTools.TryCompareList(listStatementsAfter,billingSetup.ListStatements,(x,y) => { return x.StatementNum==y.StatementNum; }));
				//All statements sent.
				Assert.IsTrue(listStatementsAfter.All(x => x.IsSent));
				//All text messages sent.
				Assert.AreEqual(billingSetup.ListStatements.Count,sendStatementsIO.CountStatmentsSentPayPortalText);
				Assert.IsTrue(listStatementsAfter.All(x => x.SmsSendStatus==AutoCommStatus.SendSuccessful));
				//Master pdf generated for InPerson.
				Assert.IsNotNull(sendStatementsIO.PdfMasterDocument);
				Assert.AreEqual(sendStatementsIO.PdfMasterDocument.PageCount,totalPages);				
				//No xml file for InPerson.
				Assert.IsTrue(string.IsNullOrEmpty(sendStatementsIO.XmlFilePath));
				//Each statements gets a document in InPerson.
				Assert.IsTrue(sendStatementsIO.ListStatementBatches.SelectMany(x => x.ListStatements).All(x => x.DocNum > 0));
			}			
			assert(false,StatementMode.Mail);
			assert(true,StatementMode.Mail);
			assert(false,StatementMode.InPerson);
			assert(true,StatementMode.InPerson);
		}

		#region OpenDentalService		
		///<summary>All RunFromODService to generate statements and assert that emails are sent and text messages are sent (or not) according to statement mode settings applied.</summary>
		[TestMethod]
		public void Billing_ODService_Email() {
			void assert(bool useClinics,bool doSendText) {
				ClearDbBilling();
				//Do not generate statements here, that will be taken care of by BillingL.CreateStatementHelper().
				BillingSetup billingSetup=SetupBilling(useClinics,numClinics:3,numStatementsPerPat:0,
					listStatementModesToIncludeSms: doSendText ? new List<StatementMode>(){ StatementMode.Email } : new List<StatementMode>(),
					funcGetPatientBillingTypeIsEmail: (a,b,c) =>{ return true; });
				SendStatementsIO sendStatementsIO=null;
				BillingL.RunFromODService(_logWriterBilling,(sendStatementsIOMock) => {
					//Save a shallow copy so we can assert later.
					sendStatementsIO=sendStatementsIOMock;
					sendStatementsIOMock.ActionSendEmail=(clinicNumPat,emailMessage,emailAddress,useSecureEmail) => { };
					sendStatementsIOMock.FuncAskQuestion=(prompt) => { return true; };
					sendStatementsIOMock.Source="BillingTests UnitTest";
				});
				//All batches processed.
				Assert.AreEqual(useClinics ? 3 : 1,billingSetup.ListClinics.Count);
				Assert.AreEqual(useClinics ? 3 : 1,sendStatementsIO.CurStatementBatch.BatchNum);
				Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.Misc));
				Assert.AreEqual(0,sendStatementsIO.CountStatementsPrinted);
				Assert.AreEqual(useClinics ? 3 : 1,sendStatementsIO.CountStatementsEmailed);
				Assert.AreEqual(0,sendStatementsIO.CountStatementsSentElectronic);
				Assert.AreEqual(doSendText ? (useClinics ? 3 : 1) : 0,sendStatementsIO.CountStatmentsSentPayPortalText);
				Assert.AreEqual(doSendText ? (useClinics ? 3 : 1) : 0,SmsToMobiles.GetAllChangedSince(DateTime_.Today).Count);
				//No master pdf ODService (only created for Mail/Inperson, neither supported by ODService).
				Assert.IsNull(sendStatementsIO.PdfMasterDocument);
			}
			//Run through happy path for clinics on and clinics off and sms send yes/no.			
			assert(false,false);
			assert(false,true);
			assert(true,false);
			assert(true,true);
		}

		///<summary>All RunFromODService to generate statements and assert that electronics are sent and text messages are sent (or not) according to statement mode settings applied.</summary>
		[TestMethod]
		public void Billing_ODService_Electronic() {
			void assert(bool useClinics,bool doSendText,BillingUseElectronicEnum eBillingType) {
				ClearDbBilling();
				//Do not generate statements here, that will be taken care of by BillingL.CreateStatementHelper().
				BillingSetup billingSetup=SetupBilling(useClinics,eBillingType: eBillingType, numClinics:3,numStatementsPerPat:0,
					listStatementModesToIncludeSms: doSendText ? new List<StatementMode>(){ StatementMode.Electronic } : new List<StatementMode>(),
					funcGetPatientBillingTypeIsEmail: (a,b,c) =>{ return false; });
				List<long> listEhgApiClinicNums=new List<long>();
				SendStatementsIO sendStatementsIO=null;
				BillingL.RunFromODService(_logWriterBilling,(sendStatementsIOMock) => {
					//Save a shallow copy so we can assert later.
					sendStatementsIO=sendStatementsIOMock;
					sendStatementsIOMock.ActionSendEmail=(clinicNumPat,emailMessage,emailAddress,useSecureEmail) => { };
					sendStatementsIOMock.FuncAskQuestion=(prompt) => { return true; };
					sendStatementsIOMock.FuncSendEhgStatement=(xml,clinicNum) => {
						//Only ehg calls this method.
						Assert.AreEqual(BillingUseElectronicEnum.EHG,eBillingType);
						Assert.IsTrue(MiscTestUtils.ValidateXml(xml));
						listEhgApiClinicNums.Add(clinicNum);
						return "";
					};
					sendStatementsIOMock.Source="BillingTests UnitTest";
				});				
				//All batches processed.
				Assert.AreEqual(useClinics ? 3 : 1,billingSetup.ListClinics.Count);
				Assert.AreEqual(useClinics ? 3 : 1,sendStatementsIO.CurStatementBatch.BatchNum);
				if(eBillingType==BillingUseElectronicEnum.ClaimX) {
					//ClaimX was never going to work for ODService. It defaults to choosing an invalid path for the xml file. It has been this way since inception.
					//Not going to fix since ClaimX is sunsetted as of 12/31/22. https://www.opendental.com/manual/eclaimsclaimx.html
					//Misc System error is only added once so only 1 error no matter how many clinics/batches.
					Assert.AreEqual(1,sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.Misc));
					Assert.IsTrue(sendStatementsIO.ListSkippedPatients.First(x => x.Reason==SkipReason.Misc).Error.Contains("does not have a valid path"));
					return;
				}
				Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.Misc));
				Assert.AreEqual(0,sendStatementsIO.CountStatementsPrinted);
				Assert.AreEqual(0,sendStatementsIO.CountStatementsEmailed);
				Assert.AreEqual(useClinics ? 3 : 1,sendStatementsIO.CountStatementsSentElectronic);
				Assert.AreEqual(doSendText ? (useClinics ? 3 : 1) : 0,sendStatementsIO.CountStatmentsSentPayPortalText);
				Assert.AreEqual(doSendText ? (useClinics ? 3 : 1) : 0,SmsToMobiles.GetAllChangedSince(DateTime_.Today).Count);
				//No master pdf ODService (only created for Mail/Inperson, neither supported by ODService).
				Assert.IsNull(sendStatementsIO.PdfMasterDocument);
				if(eBillingType.In(BillingUseElectronicEnum.POS,BillingUseElectronicEnum.ClaimX,BillingUseElectronicEnum.EDS)) {
					//These types all create similar xml file.
					Assert.IsFalse(string.IsNullOrEmpty(sendStatementsIO.XmlFilePath));
				}
				else {
					//EHG uses web api and does not create xml file.
					Assert.IsTrue(string.IsNullOrEmpty(sendStatementsIO.XmlFilePath));
					//Each clinic (batch) triggered a single api call (EHG only).
					Assert.IsTrue(ListTools.TryCompareList(listEhgApiClinicNums,billingSetup.ListClinics.Select(x => x.ClinicNum).ToList(),(x,y) => { return x==y; }));
				}
			}
			//Run each eBilling type through happy path for clinics on and clinics off and sms send yes/no.
			List<BillingUseElectronicEnum> listEBillingTypes=Enum.GetValues(typeof(BillingUseElectronicEnum)).Cast<BillingUseElectronicEnum>().ToList();
			for(int i = 0;i<listEBillingTypes.Count;i++) {
				if(listEBillingTypes[i]==BillingUseElectronicEnum.None) {
					continue;
				}
				assert(false,false,listEBillingTypes[i]);
				assert(false,true,listEBillingTypes[i]);
				assert(true,false,listEBillingTypes[i]);
				assert(true,true,listEBillingTypes[i]);
			}
		}

		///<summary>Single batch that includes both Email and Electronic statements should order and send Email before sending Electronic.</summary>
		[TestMethod]
		public void Billing_ODService_ElectronicEmail_Mix_B31268() {			
			//Do not generate statements here, that will be taken care of by BillingL.CreateStatementHelper().
			BillingSetup billingSetup=SetupBilling(false,eBillingType: BillingUseElectronicEnum.POS, numFamsPerClinic:10, numStatementsPerPat:0,
				//Mix of email and electronic statements (every other family of 10 families).
				funcGetPatientBillingTypeIsEmail: (a,b,c) =>{ return b % 2==0; });
			SendStatementsIO sendStatementsIO=null;
			BillingL.RunFromODService(_logWriterBilling,(sendStatementsIOMock) => {
				//Save a shallow copy so we can assert later.
				sendStatementsIO=sendStatementsIOMock;
				sendStatementsIOMock.ActionSendEmail=(clinicNumPat,emailMessage,emailAddress,useSecureEmail) => { };
				sendStatementsIOMock.FuncAskQuestion=(prompt) => { return true; };
				sendStatementsIOMock.Source="BillingTests UnitTest";
			});
			//All batches processed.
			Assert.AreEqual(1,billingSetup.ListClinics.Count);
			Assert.AreEqual(1,sendStatementsIO.CurStatementBatch.BatchNum);
			Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.Misc));
			Assert.AreEqual(0,sendStatementsIO.CountStatementsPrinted);
			List<Patient> listAllPats=billingSetup.GetAllPats();
			Assert.AreEqual(listAllPats.Count/2,sendStatementsIO.CurStatementBatch.ListStatements.Count(x => x.Mode_==StatementMode.Email));
			Assert.AreEqual(listAllPats.Count/2,sendStatementsIO.CurStatementBatch.ListStatements.Count(x => x.Mode_==StatementMode.Electronic));
			Assert.AreEqual(listAllPats.Count/2,sendStatementsIO.CountStatementsEmailed);
			Assert.AreEqual(listAllPats.Count/2,sendStatementsIO.CountStatementsSentElectronic);
			//First group is email.
			sendStatementsIO.CurStatementBatch.ListStatements.Take(listAllPats.Count/2).All(x => x.Mode_==StatementMode.Email);
			//Last group is electronic.
			sendStatementsIO.CurStatementBatch.ListStatements.Skip(listAllPats.Count/2).All(x => x.Mode_==StatementMode.Electronic);
			Assert.AreEqual(0,sendStatementsIO.CountStatmentsSentPayPortalText);
			Assert.AreEqual(0,SmsToMobiles.GetAllChangedSince(DateTime_.Today).Count);
			//No master pdf ODService (only created for Mail/Inperson, neither supported by ODService).
			Assert.IsNull(sendStatementsIO.PdfMasterDocument);
		}

		///<summary>Don't set BillingUseElectronic and also don't use 'E' (email) billing type.
		///This will force BillingL.CreateStatementHelper() to generate Mail statements, which are not supported by RunFromODService.
		///These statements should be created and exist in Statement table but not be sent via RunFromODService.</summary>
		[TestMethod]
		public void Billing_ODService_NotElectronic_NotEmail() {
			//Do not generate statements here, that will be taken care of by BillingL.CreateStatementHelper().
			BillingSetup billingSetup=SetupBilling(false,eBillingType: BillingUseElectronicEnum.None,numStatementsPerPat:0,
				funcGetPatientBillingTypeIsEmail: (a,b,c) =>{ return false; });
			SendStatementsIO sendStatementsIO=null;
			BillingL.RunFromODService(_logWriterBilling,(sendStatementsIOMock) => {
				//Save a shallow copy so we can assert later.
				sendStatementsIO=sendStatementsIOMock;
				sendStatementsIOMock.ActionSendEmail=(clinicNumPat,emailMessage,emailAddress,useSecureEmail) => { };
				sendStatementsIOMock.FuncAskQuestion=(prompt) => { return true; };
				sendStatementsIOMock.Source="BillingTests UnitTest";
			});
			//All batches processed.
			Assert.AreEqual(1,billingSetup.ListClinics.Count);
			Assert.AreEqual(0,sendStatementsIO.CurStatementBatch.BatchNum);
			Assert.AreEqual(0,sendStatementsIO.ListStatementNumsToSend.Count);
			Assert.AreEqual(0,sendStatementsIO.ListStatementNumsSent.Count);
			Assert.AreEqual(0,sendStatementsIO.ListSkippedPatients.Count(x => x.Reason==SkipReason.Misc));
			Assert.AreEqual(0,sendStatementsIO.CountStatementsPrinted);
			Assert.AreEqual(0,sendStatementsIO.CountStatementsEmailed);
			Assert.AreEqual(0,sendStatementsIO.CountStatementsSentElectronic);
			Assert.AreEqual(0,sendStatementsIO.CountStatmentsSentPayPortalText);
			Assert.AreEqual(0,SmsToMobiles.GetAllChangedSince(DateTime_.Today).Count);
			//No master pdf ODService (only created for Mail/Inperson, neither supported by ODService).
			Assert.IsNull(sendStatementsIO.PdfMasterDocument);
			List<Statement> listStatements=StatementT.GetAllStatements();
			Assert.AreEqual(1,listStatements.Count);
			Assert.AreEqual(0,listStatements.Count(x => x.IsSent));
			//Not electronic and not email, defaults to Mail. See Statements.GetStatementMode().
			Assert.IsTrue(listStatements.All(x => x.Mode_==StatementMode.Mail));
		}

		///<summary>Test BillingDefaultsModesToText pref parser. This will be used in FormBilling, BillingL, etc.</summary>
		[TestMethod]
		public void Billing_StatementModes_Parse() {
			//Comma-delim list of int values behind StatementMode enum.
			List<StatementMode> listInput=new List<StatementMode>(){ StatementMode.Electronic, StatementMode.InPerson, StatementMode.Email };
			Prefs.UpdateString(PrefName.BillingDefaultsModesToText,string.Join(",",listInput.Select(x => POut.Int((int)x))));
			string prefVal=PrefC.GetString(PrefName.BillingDefaultsModesToText);
			//All good values.
			List<StatementMode> listOutput=EnumTools.ConvertListOfIntsToListOfEnums<StatementMode>(prefVal,true);
			Assert.IsTrue(ListTools.TryCompareList(listInput,listOutput,doEnforceOrderMatch:true));
			//Add bad value but don't throw.
			listOutput=EnumTools.ConvertListOfIntsToListOfEnums<StatementMode>(prefVal+",10",false);
			Assert.IsTrue(ListTools.TryCompareList(listInput,listOutput,doEnforceOrderMatch: true));
			//Add bad value and do throw.
			bool didThrow=false;
			try {
				EnumTools.ConvertListOfIntsToListOfEnums<StatementMode>(prefVal+",10",true);
			}
			catch(Exception ex) {
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
			//Add bad type but don't throw.
			listOutput=EnumTools.ConvertListOfIntsToListOfEnums<StatementMode>(prefVal+",sam",false);
			Assert.IsTrue(ListTools.TryCompareList(listInput,listOutput,doEnforceOrderMatch: true));
			//Add bad value and do throw.
			didThrow=false;
			try {
				EnumTools.ConvertListOfIntsToListOfEnums<StatementMode>(prefVal+",sam",true);
			}
			catch(Exception ex) {
				didThrow=true;
			}
			Assert.IsTrue(didThrow);
		}
		
		/// <summary>Test sending of secure statements. Check that whether or not EmailDefaultSendPlatform is set to secure, clinics can be set to send statements securely based on EmailStatementsSecure pref (if EmailSecureStatus is enabled for that clinic.</summary>
		[TestMethod]
		public void Billing_Email_Secure(){
			bool hasException=false;
			BillingSetup billingSetup=SetupBilling(true,numPatsPerFam:1,numClinics:5,
				funcGetStatementMode:(a,b,c) =>{ return StatementMode.Email; },
				funcGetClinicUsesSecureEmail:(clinicIndex) =>{ return true; },
				funcGetClinicUsesSecureStatements:(clinicIndex) => { return clinicIndex%2!=0; });//1 and 3 send statements secure
			//set clinics 3 and 4 to send email unsecure. Can't be set in SetupBilling because we only want this pref disabled
			//the code below is updating the old pref (EmailDefaultSendPlatform) to prove that the old pref is ignored when determining whether or not to send secure statements over email. Sending secure statements is determined by the new pref, EmailStatementsSecure, which is set above by funcGetClinicUsesSecureStatements
			ClinicPrefs.Upsert(PrefName.EmailDefaultSendPlatform,billingSetup.ListClinics[3].ClinicNum,EmailPlatform.Unsecure.ToString());
			ClinicPrefs.Upsert(PrefName.EmailDefaultSendPlatform,billingSetup.ListClinics[4].ClinicNum,EmailPlatform.Unsecure.ToString());
			SendStatementsIO sendStatementsIO = new SendStatementsIOTest(
				listStatementNumsToSend:billingSetup.ListStatements.Select(x => x.StatementNum).ToList(),
				actionSendEmail:(clinicNum,emailMessage,emailAddress,isSecure) => {
					int clinicIndex=billingSetup.ListClinics.FindIndex(x => x.ClinicNum==clinicNum);
					if(clinicIndex==1 || clinicIndex==3){
						try{
							Assert.IsTrue(isSecure);
							Assert.AreEqual(EmailSentOrReceived.SecureEmailSent,emailMessage.SentOrReceived);
						}
						catch(Exception ex){
							hasException=true;
						}
					}
					else {
						try{
							Assert.IsFalse(isSecure);
							Assert.AreEqual(EmailSentOrReceived.Sent,emailMessage.SentOrReceived);
						}
						catch(Exception ex){
							hasException=true;
						}
					}
				});
				BillingL.SendStatements(sendStatementsIO);
				Assert.IsFalse(hasException);//check that none of the asserts within the Action failed
		}
		#endregion

		private class LogWriterBillingTest : LogWriter {
			Logger.IWriteLine _log=null;
			public LogWriterBillingTest(Logger.IWriteLine log) { _log=log; }
			public override void WriteLine(string data,LogLevel logLevel,string subDirectory = "") { _log?.WriteLine(data,logLevel,subDirectory); }
		}

		private class SendStatementsIOTest : SendStatementsIO {
			///<summary></summary>
			/// <param name="listStatementNumsToSend">In production, comes from selected rows in FormBilling's statement grid.</param>
			/// <param name="listStatementNumsSent">In production, is generated as billing runs. 
			/// In test mode, we can provide this in certain scenarios to mock sent statement without actually having to run integration.</param>
			/// <param name="actionProgressEvent">Not expecting return so normally just swallowed in test mode.</param>
			/// <param name="actionPrompt">Not expecting return so normally just swallowed in test mode.</param>
			/// <param name="actionDeleteTempPdfFile">default deletes the file specified</param>
			/// <param name="actionSendEmail">Not expecting return so normally just swallowed in test mode.</param>
			/// <param name="actionSleepDuringPause">Not expecting return so normally just swallowed in test mode.</param>
			/// <param name="funcAskQuestion">default true</param>
			/// <param name="funcChooseSaveFile">default true/Statements.xml</param>
			/// <param name="funcGetBillingDataTable"></param>
			/// <param name="funcGetIsCancelled">default false</param>
			/// <param name="funcGetIsHistoryStartMinDate">default false</param>
			/// <param name="functGetIsPaused">default false</param>
			/// <param name="funcGetPatientPdfPath">default echo back filePath</param>
			/// <param name="funcGetPdfDocument"></param>
			/// <param name="funcGetSenderEmailAddress">default EmailAddresses.GetByClinic(clinicNum)</param>
			/// <param name="funcGetEmailAttachment"></param>
			/// <param name="funcSendEhgStatement">default empty string. Would otherwise provide error message generated by DentalXChange.</param>	
			/// <param name="funcComputeAging">default true.</param>	
			public SendStatementsIOTest(
				List<long> listStatementNumsToSend=null,
				LogWriter logWriter=null,
				List<long> listStatementNumsSent=null,
				Action<ODEventArgs> actionProgressEvent = null,
				Action<string,bool> actionPrompt = null,
				Action<string> actionDeleteTempPdfFile = null,
				Action<long,EmailMessage,EmailAddress,bool> actionSendEmail = null,
				Action actionSleepDuringPause = null,				
				Func<string,bool> funcAskQuestion = null,
				Func<string,ChooseSaveFile> funcChooseSaveFile = null,
				Func<DataTable> funcGetBillingDataTable = null,
				Func<bool> funcGetIsCancelled = null,
				Func<bool> funcGetIsHistoryStartMinDate=null,
				Func<bool> functGetIsPaused=null,
				Func<string,string,string> funcGetPatientPdfPath = null,
				Func<string,string,PdfDocument> funcGetPdfDocument = null,
				Func<long,EmailAddress> funcGetSenderEmailAddress=null,
				Func<string,Document,Patient,EmailAttach> funcGetEmailAttachment=null,
				Func<string,long,string> funcSendEhgStatement=null,
				Func<DateTime,bool> funcComputeAging=null) 
			{
				Source="BillingTests UnitTest";
				ListStatementNumsToSend=ODMethodsT.Coalesce(listStatementNumsToSend,new List<long>());				
				LogWriter=logWriter; //Null is ok for LogWriter, it will just skip logging.
				ListStatementNumsSent=ODMethodsT.Coalesce(listStatementNumsSent,new List<long>());
				ActionProgressEvent=ODMethodsT.Coalesce(actionProgressEvent,(args) => { });
				ActionPrompt=ODMethodsT.Coalesce(actionPrompt,(prompt,useCopyPaste) => { });
				ActionSleepDuringPause=ODMethodsT.Coalesce(actionSleepDuringPause,() => { });
				ActionSendEmail=ODMethodsT.Coalesce(actionSendEmail,(clinicNumPat,emailMessage,emailAddress,useSecureEmail) => { });
				ActionDeleteTempPdfFile=ODMethodsT.Coalesce(actionDeleteTempPdfFile,(tempPdfFile) => { File.Delete(tempPdfFile); });
				FuncGetIsHistoryStartMinDate=ODMethodsT.Coalesce(funcGetIsHistoryStartMinDate,() => { return false; });
				FuncAskQuestion=ODMethodsT.Coalesce(funcAskQuestion,(question) => { return true; });
				FuncChooseSaveFile=ODMethodsT.Coalesce(funcChooseSaveFile,(initialSaveDirectory) => { return new ChooseSaveFile() { IsSelectionOk=true, FileName="Statements.xml"}; });
				FunctGetIsPaused=ODMethodsT.Coalesce(functGetIsPaused,() => { return false; });
				FuncGetIsCancelled=ODMethodsT.Coalesce(funcGetIsCancelled,() => { return false; });
				FuncGetBillingDataTable=ODMethodsT.Coalesce(funcGetBillingDataTable,() => { return new DataTable(); });
				FuncGetSenderEmailAddress=ODMethodsT.Coalesce(funcGetSenderEmailAddress,(clinicNum) => { return EmailAddresses.GetByClinic(clinicNum); });
				FuncGetPatientPdfPath=ODMethodsT.Coalesce(funcGetPatientPdfPath,(tempPdfFile,filePath) => { return filePath; });
				FuncGetPdfDocument=ODMethodsT.Coalesce(funcGetPdfDocument,(rawBase64,savedPdfPath) => { return new PdfDocument(); });
				FuncGetEmailAttachment=ODMethodsT.Coalesce(funcGetEmailAttachment,(savedPdfPath,documentStatement,patient) => { return new EmailAttach(); });
				FuncSendEhgStatement=ODMethodsT.Coalesce(funcSendEhgStatement,(xmlData,clincNum) => { return ""; });
				FuncComputeAging=ODMethodsT.Coalesce(funcComputeAging,(dateTimeToday) => { return true; });
			}
		}

		private class BillingSetup {
			public List<Clinic> ListClinics=new List<Clinic>();
			public List<Family> ListFamilies=new List<Family>();
			public List<Statement> ListStatements=new List<Statement>();
			public List<EmailAddress> ListEmailAddresses=new List<EmailAddress>();
			public List<EmailAutograph> ListEmailAutographs=new List<EmailAutograph>();
			public Def DefBillingTypeStandard=null;
			public Def DefBillingTypeEmail=null;
			public List<Procedure> ListProcedures=new List<Procedure>();

			public List<Patient> GetAllPats() {
				return ListFamilies.SelectMany(x => x.ListPats).ToList();
			}
		}

		///<summary>Setting up BillingL boilerplate is cumbersome. This method helps it along.</summary>
		/// <param name="isClinicsOn">Turn clinic pref on/off.</param>
		/// <param name="eBillingType">Entire practice has exactly one electronic billing type</param>
		/// <param name="numFamsPerClinic">Each clinic (inluding practice if clinics off) can have a static number of families. Enhance this if you want variation per clinic.</param>
		/// <param name="numPatsPerFam">First patient of each family will always be guarantor. Increase this number to make larger families.
		/// If 1, then each family will have 1 patient (its own guarantor).</param>
		/// <param name="numStatementsPerPat">Number of statements (including 0 if desired) assigned to each family member. Enhance this if you want different count of statements per pat.</param>
		/// <param name="numClinics">If isClinicsOn=false, treats pracitce as one and only clinic.</param>
		/// <param name="listStatementModesToIncludeSms">Used by RunFromODService to determine which statement modes will have a text message sent in addition to statement generation.
		/// Updates PrefName.BillingDefaultsModesToText, which will be used by BillingL.CreateStatementHelper.</param>
		/// <param name="funcGetStatementMode">Callback to provide StatementMode for 0-based clinic/family/patient. 
		/// Allows for any number of generated patientst to have any number of different statement modes (like in real life).
		/// If eBillingType provided is not 'None' then default will be Electronic, otherwise default is InPerson.</param>
		/// <param name="funcGetSmsSendStatus">Callback to provide sms send preference of an individual statement for 0-based clinic/family/patient/statement. Default is SendNotAttempted.</param>
		/// <param name="funcGetClinicUsesSecureEmail">Callback to define if clinic uses secure email. Default is false.</param>
		/// <param name="funcGetPatientBillingTypeIsEmail">Callback to define if clinic sends email in lieu of mail or electronic. 
		/// Default is false (defer to pref BillingUseElectronic). See Statements.GetStatementMode() for details.</param>
		/// <param name="funcGetPatientProcsForAging">Callback to define procedures to be added for given patient. If not provided then patient will be given a single default completed procedure. 
		/// This makes patient appear in AgingData query. DO NOT insert this procedures into db if you return a list of procs here. SetupBilling will handle insert and add to ListProcedures.
		/// Return empty list or null from func if you want 0 procs added.</param>
		/// <returns></returns>
		private static BillingSetup SetupBilling(bool isClinicsOn,BillingUseElectronicEnum eBillingType = BillingUseElectronicEnum.None,int numFamsPerClinic = 1,
			int numPatsPerFam = 1,int numStatementsPerPat = 1,int numClinics = 1,bool isHtmlEmailAutograph=false,
			List<StatementMode> listStatementModesToIncludeSms = null,
			Func<int /*clinicindex 0 based*/,int /*famindex 0 based*/,int /*patindex 0 based*/,StatementMode> funcGetStatementMode = null,
			Func<int /*clinicindex 0 based*/,int /*famindex 0 based*/,int /*patindex 0 based*/,string> funcGetPatientEmailAddress = null,
			Func<int /*clinicindex 0 based*/,int /*famindex 0 based*/,int /*patindex 0 based*/,int /*statementindex 0 based*/,AutoCommStatus> funcGetSmsSendStatus = null,
			Func<int /*clinicindex 0 based*/,bool> funcGetClinicUsesSecureEmail=null,
			Func<int /*clinicindex 0 based*/,int /*famindex 0 based*/,int /*patindex 0 based*/,bool> funcGetPatientBillingTypeIsEmail = null,
			Func<int /*clinicindex 0 based*/,int /*famindex 0 based*/,int /*patindex 0 based*/,Patient,Clinic,List<Procedure>> funcGetPatientProcsForAging = null,
			Func<int /*clinicindex 0 based*/, bool> funcGetClinicUsesSecureStatements=null) {
			BillingSetup ret=new BillingSetup();
			//Setup billing types. Patients will be given a BillingType below.
			DefT.DeleteAllForCategory(DefCat.BillingTypes);
			ret.DefBillingTypeStandard=DefT.CreateDefinition(DefCat.BillingTypes,"Standard Account");
			//'E' in ItemValue means ignore Electronic vs Mail pref (BillingUseElectronic) and send email. 
			ret.DefBillingTypeEmail=DefT.CreateDefinition(DefCat.BillingTypes,"Email Account","E");
			//Default to standard account, not email. Statements.GetStatementMode() will 
			PrefT.UpdateLong(PrefName.PracticeDefaultBillType,ret.DefBillingTypeStandard.DefNum);
			PrefT.UpdateBool(PrefName.EasyNoClinics,!isClinicsOn);
			//Set the eBillingType. This is critical for pretty much any BillingL operations and changes behavior drastically.
			PrefT.UpdateString(PrefName.BillingUseElectronic,POut.Int((int)eBillingType));
			listStatementModesToIncludeSms=ODMethodsT.Coalesce(listStatementModesToIncludeSms,new List<StatementMode>());
			//Comma-delim list of int values behind StatementMode enum.
			Prefs.UpdateString(PrefName.BillingDefaultsModesToText,string.Join(",",listStatementModesToIncludeSms.Select(x => POut.Int((int)x))));
			//Default to InPerson or Electronic statement mode for all patients depending on pref setting.
			funcGetStatementMode=ODMethodsT.Coalesce(funcGetStatementMode,(clinicIndex,famIndex,patIndex) => { 
				return eBillingType==BillingUseElectronicEnum.None ? StatementMode.InPerson : StatementMode.Electronic; });
			//Default to valid email address.
			funcGetPatientEmailAddress=ODMethodsT.Coalesce(funcGetPatientEmailAddress,(clinicIndex,famIndex,patIndex) => {
				return $"PatEmail_{clinicIndex}_{famIndex}_{patIndex}@mail.com";
			});
			//Default to SendNotAttempted.
			funcGetSmsSendStatus=ODMethodsT.Coalesce(funcGetSmsSendStatus,(clinicIndex,famIndex,patIndex,statementIndex) => {
				return AutoCommStatus.SendNotAttempted;
			});
			//Default to non secure.
			funcGetClinicUsesSecureEmail=ODMethodsT.Coalesce(funcGetClinicUsesSecureEmail,(clinicIndex) => {
				return false;
			});
			//Default to practice default.
			funcGetPatientBillingTypeIsEmail=ODMethodsT.Coalesce(funcGetPatientBillingTypeIsEmail,(clinicIndex,famIndex,patIndex) => {
				return false;
			});
			//Default to a completed procedure on today's date.
			funcGetPatientProcsForAging=ODMethodsT.Coalesce(funcGetPatientProcsForAging,(clinicIndex,famIndex,patIndex,pat,clinic) => {
				return new List<Procedure>() { 
					ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",10.00,DateTime_.Today,doInsert: false,clinicNum: clinic?.ClinicNum??0,dateTStamp: DateTime_.Now) 
				};
			});
			//Default to non secure.
			funcGetClinicUsesSecureStatements=ODMethodsT.Coalesce(funcGetClinicUsesSecureStatements,(clinicIndex) => {
				return false;
			});
			if(!isClinicsOn) {
				numClinics=1;
			}
			//At least one clinic in the loop (might be just the practice clinic).
			numClinics=Math.Max(numClinics,0);
			for(int clinicIndex = 0;clinicIndex<numClinics;clinicIndex++) {
				EmailAddress email=EmailAddressT.CreateEmailAddress($"clinic_{clinicIndex}@odtest.com",$"clinic_{clinicIndex}");
				ret.ListEmailAddresses.Add(email);
				string autographText=$"AUTOGRAPH_clinic_{clinicIndex}";
				string autographDesc=$"PLAIN_{clinicIndex}";
				if(isHtmlEmailAutograph) {
					autographText=$"<span style=\"color: red;\">{autographText}</span>";
					autographDesc=$"HTML_{clinicIndex}";
				}
				ret.ListEmailAutographs.Add(EmailAutographT.CreateEmailAutograph(email.EmailUsername,autographText: autographText,description: autographDesc));
				Clinic clinicGuar=null;
				if(isClinicsOn) {
					clinicGuar=ClinicT.CreateClinic(description: "Clinic"+clinicIndex.ToString(),email.EmailAddressNum,isTextingEnabled: true,smsMonthlyLimit: 10_000,useSecureEmail: funcGetClinicUsesSecureEmail(clinicIndex),useSecureStatements:funcGetClinicUsesSecureStatements(clinicIndex));
				}
				else {
					clinicGuar=ClinicT.CreatePracticeClinic("Practice Clinic",email.EmailAddressNum,isTextingEnabled: true,smsMonthlyLimit: 10_000,useSecureEmail: funcGetClinicUsesSecureEmail(clinicIndex),useSecureStatements:funcGetClinicUsesSecureStatements(clinicIndex));
				}
				ret.ListClinics.Add(clinicGuar);
				for(int famIndex = 0;famIndex<numFamsPerClinic;famIndex++) {
					string lNamePrefix=MiscUtils.CreateRandomAlphaString(4);
					//First pat is guar
					Patient patGuar=PatientT.CreatePatient(clinicNum:clinicGuar.ClinicNum,email:funcGetPatientEmailAddress(clinicIndex,famIndex,0),
						lName:$"PatL_{lNamePrefix}_CN{clinicGuar.ClinicNum}_Fam{famIndex}",fName:$"PatF_CN{clinicGuar.ClinicNum}_Fam{famIndex}_Guar{MiscUtils.CreateRandomAlphaString(4)}",
						address: "add", city: "city",zip: "97333",state: "OR", wirelessPhone:"2345678901",
						billingType:funcGetPatientBillingTypeIsEmail(clinicIndex,famIndex,0) ? ret.DefBillingTypeEmail.DefNum : ret.DefBillingTypeStandard.DefNum);
					List<Patient> listPatsFamily=new List<Patient>();
					listPatsFamily.Add(patGuar);
					List<Procedure> procsGuar=funcGetPatientProcsForAging(clinicIndex,famIndex,0,patGuar,clinicGuar);
					if(!procsGuar.IsNullOrEmpty()) {
						for(int i=0;i<procsGuar.Count;i++) {
							Procedures.Insert(procsGuar[i]);
							ret.ListProcedures.Add(procsGuar[i]);
						}						
					}
					//Add statements for pat 0 for this family.
					for(int statementIndex = 0;statementIndex<numStatementsPerPat;statementIndex++) {
						ret.ListStatements.Add(StatementT.CreateStatement(patGuar.PatNum,mode_: funcGetStatementMode(clinicIndex,famIndex,0),
							smsSendStatus:funcGetSmsSendStatus(clinicIndex,famIndex,0,statementIndex)));
					}
					//Start at 1, rest of pats are rest of fam.
					for(int patIndex = 1;patIndex<numPatsPerFam;patIndex++) {
						Patient pat=PatientT.CreatePatient(clinicNum:clinicGuar.ClinicNum,email:funcGetPatientEmailAddress(clinicIndex,famIndex,patIndex),
							lName:patGuar.LName,fName:$"PatF_CN{clinicGuar.ClinicNum}_Fam{famIndex}_Child{MiscUtils.CreateRandomAlphaString(4)}",
							guarantor:patGuar.Guarantor,wirelessPhone:"2345678901",
							billingType:funcGetPatientBillingTypeIsEmail(clinicIndex,famIndex,patIndex) ? ret.DefBillingTypeEmail.DefNum : ret.DefBillingTypeStandard.DefNum);
						listPatsFamily.Add(pat);
						List<Procedure> procsPat=funcGetPatientProcsForAging(clinicIndex,famIndex,patIndex,pat,clinicGuar);
						if(!procsPat.IsNullOrEmpty()) {
							for(int i = 0;i<procsPat.Count;i++) {
								Procedures.Insert(procsPat[i]);
								ret.ListProcedures.Add(procsPat[i]);
							}
						}
						for(int statementIndex = 0;statementIndex<numStatementsPerPat;statementIndex++) {
							ret.ListStatements.Add(StatementT.CreateStatement(pat.PatNum,mode_: funcGetStatementMode(clinicIndex,famIndex,patIndex),
								smsSendStatus: funcGetSmsSendStatus(clinicIndex,famIndex,patIndex,statementIndex)));
						}
					}
					ret.ListFamilies.Add(new Family(listPatsFamily));
				}
			}
			return ret;
		}
	}
}

/* Previously unreported bugs observed and fixed while refactoring and unit testing.
1)
In the original code... this line is called for all statement modes on all statements...
tempPdfFile=FormRpStatement.CreateStatementPdfSheets(statement,patient,family,dataSet,true); 
But this line is only called when in Email statement mode...
File.Delete(tempPdfFile); 
This leaves a ton of temp pdf files in the user's temp directory. The bug fix is to call File.Delete no matter which statement mode we are in.
FIX: File.Delete(tempPdfFile); now called for all statement modes

2) 
If electronic Billing happens to fail because DentalXChange gives us back an error or doesn't respond... 
We proceed to go ahead and send a text message to all patients in that batch who were scheduled to receive a statement on this billing run. 
FIX: Detect failed EHG api calls and prevent sending text message informing these patients that they have a new statement to view online.

3)
If electronic Billing happens to fail because DentalXChange gives us back an error or doesn't respond... 
All subequent batches are allowed to run and will likely suffer the same fate.
FIX: I decided not to fix this. A subsequent batch may succeed due to being a different clinic so having differnt (valid) input args. No harm to let it try as statements are left unsent.

4)
Counts reported back to UI regarding emailed/texted/ebill/etc could be wrong in some edge cases.
FIX: Be more careful about incrementing the counts only when necessary.

5)
User opting to cancel xml file/folder selection on first batch iteration would have previously auto-canceled folder selection for subsequent batches.
JosephT did a refactor in late '23 which removed this auto-cancel. So cancelling that prompt would force user to cancel the prompt in each subsequent batch iteration.
FIX: Retain the user cancellation selection and apply the cancellation logic to each batch iteration (as it had worked previously).

6)
EmailAutograph had previously only been included at the bottom of an email statement if clinic pref was set to Unsecure email (no autograph inserted for Secure email).
FIX: Talked with Derek and he was fine with adding the autograph to Secure email as well when pref called for it.

7)
Batching of statements only considers the BillingElectBatchMax pref when clinics are OFF.
So if clinics are turned ON and statements are being batched by guarantor ClinicNum, and that ClinicNum happens to have a batch that would otherwise exceed the pref limit,
that batch is not broken into multiple batches.
FIX: I decided not to fix this. The batching code has existed for years and is likely used by NADG. If it was an issue, they would have complained by now.

8)
ClaimX electronic billing prompts user to select an xml file generation full path and starts in C:\StatementX\ (code comment says vendor wants this path).
But we do not retain the folder after the selection is made. So we end up creating the xml file in the working directory of the program.
FIX: I decided not to fix this. ClaimX code is very old (> 10 years) and has worked this way for years. Customers may expect it to work like this.
FURTHER INVESTIGATION: ClaimX is no longer supported and was replaced by DentalXChange (EHG). https://www.opendental.com/manual/eclaimsclaimx.html
I'll still leave this folder selection bug but I'm not going to worry that it's there.

9)
ClaimX electronic billing was never going to work in OpenDentalService. The pref used to find desired file path pulls from a pref that does not represent a file path: BillingUseElectronic (int).
FIX: Now uses same hard-coded file path as desktop billing: C:\StatementX\. This won't work either per the bug noted above, but at least it is consistent.

10)
The order of which we send statements was broken. 
When clinics are OFF: We intend to send the statements in the exact order shown in the statement grid of FormBilling.
This works for all billing modes EXCEPT electronic billing. When electronic billing is ON, we would iterate through the statement list backwards and send statements in reverse.
This backwards iteration was originally put into place as a brute force means of sending statements in batches.
Batches were later ehanced to be more explicit, however, this backwards loop iteration was retained (inadvertantly).
FIX: No longer loop through statement list in reverse for electronic billing mode.

When clinics are ON: We intend to allow the user to set a pref, PrintStatementsAlphabetically. 
This ignores grid sorting and instead orders statements by patient LName, FName. 
Statements are later broken up into clinic batches and sorting within each clinic is retained (as intended).
However, when the PrintStatementsAlphabetically pref is OFF, there is neither grid sorting applied to the statements nor LName, FName sorting.
In this case, the send of statement order within each batch is non-determinant.
FIX: Retain the grid sorting after clinic batches have been established.

B31268 made an attempted fix to avoid OOM exceptions by simply ignoring user statements sort preferences and always sending emails before sending eBills for each batch.
This renders the sorting issues worked out above invalid when email is involved since later in BillingL we will just sort with Emails first and eBills last.
FIX: This bug fix was retained as Derek and Saul spent a lot of time on this attempted fix and the issue did not seem to come back after the fix.

11)
Various uses of MsgBox popup were showing behind FormBilling window.
FIX: Use MessageBox instead.

12)
OD Service will only generate statements of type Mail, Electronic, Email (per Statements.GetStatementMode). However, OD Service will only send statements of type Electronic, Email.
So 2 things... 1) InPerson statements are likely generated as Mail by OD Service. 2) OD Service generates more statement types (3) than it sends (2).
The original code explicitly deletes existing Email/Electronic statements before generating new statements. 
Then, regardless of what was generated and put in the Statement table, the original code would filter out all except Email/Electronic before sending statements.
FIX: This behavior was retained in the off chance that an office is using OD Service to generate/send Email/Electronic statements and is also dependent on InPerson statements residing in the statement table.
*/