using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.AgingData_Tests {
	[TestClass]
	public class AgingDataTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			PatientT.ClearPatientTable();
			StatementT.ClearStatementTable();
			ProcedureT.ClearProcedureTable();
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		///<summary>Make sure that patients show up in the billing list (aging list) when there has been no statement.</summary>
		[TestMethod]
		public void AgingData_GetAgingData_PayPlanBillInAdvanceDays_NoStatement() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create a payment plan where the first charge date is in the future.
			DateTime dateTimePayPlanCharge=DateTime.Today.AddDays(5);
			DateTime dateStatement=DateTime.Today;
			PayPlanT.CreatePayPlan(patient.PatNum,1000,500,dateTimePayPlanCharge,provNum);
			//Make sure that the preference PayPlansBillInAdvanceDays is set to a day range that encompasses the first payment plan charge date.
			PrefT.UpdateLong(PrefName.PayPlansBillInAdvanceDays,10);
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,false,false,false,new List<long>());
			//Assert that the patient has been returned due to owing money on a payment plan that falls within the "bill in advance days" preference.
			Assert.IsTrue(dictPatAgingData.ContainsKey(patient.PatNum),"No aging data was returned for the patient.");
			//Assert all pertinent PatAgingData for this unit test.
			Assert.IsNotNull(dictPatAgingData[patient.PatNum].ListPatAgingTransactions);
			PatAgingTransaction patAgingTransaction=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.PayPlanCharge);
			Assert.IsNotNull(patAgingTransaction);
			Assert.AreEqual(dateTimePayPlanCharge,patAgingTransaction.DateLastTrans);
			//Do NOT insert a statement.  This should cause the patient to be included in the PatAging list returned.
			//StatementT.CreateStatement(patient.PatNum,mode_:StatementMode.Mail,isSent:true,dateSent:dateStatement);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTrans=new SerializableDictionary<long, List<PatAgingTransaction>>();
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
				dictPatAgingTrans[kvp.Key]=kvp.Value.ListPatAgingTransactions;
			}
			//Assert that GetDateLastTrans() returns the date we are expecting.
			Assert.AreEqual(DateTime.MinValue,AgingData.GetDateLastTrans(dictPatAgingTrans[patient.PatNum],dateStatement).Date);
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today.AddMonths(-1),new List<long>(),false,false,0,false,false,new List<long>(),false,false,
				new List<long>(),new List<long>(),dictPatAgingTrans);
			//Assert that the patient will not get a new statement due to the statement that was created above.
			Assert.IsTrue(listPatAging.Any(x => x.PatNum==patient.PatNum),"The patient was supposed to be present in the AgingList.");
		}

		///<summary>Make sure that patients do not show up in the billing list (aging list) when there has been a statement within
		///the PayPlansBillInAdvanceDays date range (assume the statement within the date range encompasses the payment plan charge).</summary>
		[TestMethod]
		public void AgingData_GetAgingData_PayPlanBillInAdvanceDays_WithStatement() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create a payment plan where the first charge date is in the future.
			DateTime dateTimePayPlanCharge=DateTime.Today.AddDays(5);
			DateTime dateStatement=DateTime.Today;
			PayPlanT.CreatePayPlan(patient.PatNum,1000,500,dateTimePayPlanCharge,provNum);
			//Make sure that the preference PayPlansBillInAdvanceDays is set to a day range that encompasses the first payment plan charge date.
			PrefT.UpdateLong(PrefName.PayPlansBillInAdvanceDays,10);
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,false,false,false,new List<long>());
			//Assert that the patient has been returned due to owing money on a payment plan that falls within the "bill in advance days" preference.
			Assert.IsTrue(dictPatAgingData.ContainsKey(patient.PatNum),"No aging data was returned for the patient.");
			//Assert all pertinent PatAgingData for this unit test.
			Assert.IsNotNull(dictPatAgingData[patient.PatNum].ListPatAgingTransactions);
			PatAgingTransaction patAgingTransaction=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.PayPlanCharge);
			Assert.IsNotNull(patAgingTransaction);
			Assert.AreEqual(dateTimePayPlanCharge,patAgingTransaction.DateLastTrans);
			//Insert a statement that was sent today.  This should cause the patient to be excluded from the PatAging list returned.
			StatementT.CreateStatement(patient.PatNum,mode_:StatementMode.Mail,isSent:true,dateSent:dateStatement);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTrans=new SerializableDictionary<long, List<PatAgingTransaction>>();
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
				dictPatAgingTrans[kvp.Key]=kvp.Value.ListPatAgingTransactions;
			}
			//Assert that GetDateLastTrans() returns the date we are expecting.
			Assert.AreEqual(DateTime.MinValue,AgingData.GetDateLastTrans(dictPatAgingTrans[patient.PatNum],dateStatement).Date);
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today.AddMonths(-1),new List<long>(),false,false,0,false,false,new List<long>(),false,false,
				new List<long>(),new List<long>(),dictPatAgingTrans);
			//Assert that the patient will not get a new statement due to the statement that was created above.
			Assert.IsFalse(listPatAging.Any(x => x.PatNum==patient.PatNum),"The patient was not supposed to be present in the AgingList.");
		}

		///<summary>Make sure that patients show up in the billing list (aging list) when there has been a statement within the PayPlansBillInAdvanceDays
		///date range (they should cancel each other out) BUT there is a completed procedure (or claimproc) AFTER the statement.</summary>
		[TestMethod]
		public void AgingData_GetAgingData_PayPlanBillInAdvanceDays_WithPendingProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			long provNum=ProviderT.CreateProvider(suffix);
			DateTime datePayPlan=DateTime.Today.AddDays(5);
			DateTime datePayPlanCreate=DateTime.Today.AddMonths(-1);//Payment Plan was created a month ago.
			DateTime dateProc=DateTime.Today;
			DateTime dateStatement=DateTime.Today.AddDays(-5);
			//Create a payment plan where the first charge date in the future.
			PayPlanT.CreatePayPlan(patient.PatNum,1000,500,datePayPlan,provNum);
			//Create a completed procedure that was completed today, before the first payplan charge date.
			ProcedureT.CreateProcedure(patient,"D1100",ProcStat.C,"",5,dateProc);
			//Insert a statement that was sent during the "bill in advance days" for the payment plan charge above.
			StatementT.CreateStatement(patient.PatNum,mode_:StatementMode.Mail,isSent:true,dateSent:dateStatement);
			//Make sure that the preference PayPlansBillInAdvanceDays is set to a day range that encompasses the first payment plan charge date.
			PrefT.UpdateLong(PrefName.PayPlansBillInAdvanceDays,10);
			//This scenario is exploiting the fact that the statement created 5 days ago was technically created for the payment plan (in advance).
			//Because of this fact, the patient shouldn't show up in the billing list until something new happens after the statement date.
			//The procedure that was completed today should cause the patient to show up in the billing list (something new happened).
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,false,false,false,new List<long>());
			//Assert that the patient has been returned due to the completed procedure.
			Assert.IsTrue(dictPatAgingData.ContainsKey(patient.PatNum),"No aging data was returned for the patient.");
			//Assert all pertinent PatAgingData for this unit test.
			Assert.IsNotNull(dictPatAgingData[patient.PatNum].ListPatAgingTransactions);
			PatAgingTransaction patAgingTransactionPP=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.PayPlanCharge);
			//Act like the payment plan was created a month ago.
			patAgingTransactionPP.SecDateTEntryTrans=datePayPlanCreate;
			PatAgingTransaction patAgingTransactionProc=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.Procedure);
			Assert.IsNotNull(patAgingTransactionPP);
			Assert.IsNotNull(patAgingTransactionProc);
			Assert.AreEqual(datePayPlan,patAgingTransactionPP.DateLastTrans);
			Assert.AreEqual(dateProc,patAgingTransactionProc.DateLastTrans);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTrans=new SerializableDictionary<long,List<PatAgingTransaction>>();
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
				dictPatAgingTrans[kvp.Key]=kvp.Value.ListPatAgingTransactions;
			}
			//The last transaction date should be the procedure date and not the pay plan charge date (even though pay plan is later).
			Assert.AreEqual(dateProc,AgingData.GetDateLastTrans(dictPatAgingTrans[patient.PatNum],dateStatement).Date);
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today.AddMonths(-1),new List<long>(),false,false,0,false,false,new List<long>(),
				false,false,new List<long>(),new List<long>(),dictPatAgingTrans);
			//Assert that the patient has been flagged to get a new statement due to procedure that was completed above.
			Assert.IsTrue(listPatAging.Any(x => x.PatNum==patient.PatNum),"The expected patient was not present in the AgingList.");
		}

		///<summary>Make sure that patients show up in the billing list (aging list) when there has been a statement within the PayPlansBillInAdvanceDays
		///date range (they should cancel each other out) BUT there is a completed procedure (or claimproc) AFTER the statement.</summary>
		[TestMethod]
		public void AgingData_GetAgingData_PayPlanBillInAdvanceDays_WithNewPayPlan() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			long provNum=ProviderT.CreateProvider(suffix);
			DateTime datePayPlanCharge=DateTime.Today.AddDays(5);
			DateTime datePayPlanCreate=DateTime.Today;//The payment plan that we are about to create will automatically have this date as the SecTDateEntry
			DateTime dateProc=DateTime.Today.AddDays(-1);
			DateTime dateStatement=DateTime.Today.AddDays(-1);
			//Create a payment plan where the first charge date in the future.
			PayPlanT.CreatePayPlan(patient.PatNum,1000,500,datePayPlanCharge,provNum);
			//Create a completed procedure that was completed yesterday, before the first payplan charge date AND before the payment plan creation date.
			ProcedureT.CreateProcedure(patient,"D1100",ProcStat.C,"",5,dateProc);
			//Insert a statement that was sent during the "bill in advance days" for the payment plan charge AND before the payment plan creation date.
			StatementT.CreateStatement(patient.PatNum,mode_:StatementMode.Mail,isSent:true,dateSent:dateStatement);
			//Make sure that the preference PayPlansBillInAdvanceDays is set to a day range that encompasses the first payment plan charge date.
			PrefT.UpdateLong(PrefName.PayPlansBillInAdvanceDays,10);
			//This scenario is exploiting the fact that the statement created yesterday was NOT technically created for the payment plan (in advance).
			//Because of this fact, the patient should show up in the billing list because something new has happened after the statement date.
			//The new payment plan should not be associated to the previous statement due to the SecTDateEntry.
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,false,false,false,new List<long>());
			//Assert that the patient has been returned due to owing money on the payment plan that was created.
			Assert.IsTrue(dictPatAgingData.ContainsKey(patient.PatNum),"No aging data was returned for the patient.");
			Assert.IsNotNull(dictPatAgingData[patient.PatNum].ListPatAgingTransactions);
			PatAgingTransaction patAgingTransactionPP=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.PayPlanCharge);
			PatAgingTransaction patAgingTransactionProc=dictPatAgingData[patient.PatNum].ListPatAgingTransactions
				.FirstOrDefault(x => x.TransactionType==PatAgingTransaction.TransactionTypes.Procedure);
			Assert.IsNotNull(patAgingTransactionPP);
			Assert.IsNotNull(patAgingTransactionProc);
			Assert.AreEqual(datePayPlanCharge,patAgingTransactionPP.DateLastTrans);
			Assert.AreEqual(dateProc,patAgingTransactionProc.DateLastTrans);
			SerializableDictionary<long,List<PatAgingTransaction>> dictPatAgingTrans=new SerializableDictionary<long,List<PatAgingTransaction>>();
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {
				dictPatAgingTrans[kvp.Key]=kvp.Value.ListPatAgingTransactions;
			}
			//The last transaction date should be the charge date of the pay plan charge which indicates that the statement doesn't apply
			//to the payment plan because the payment plan was created AFTER the statement that just so happens to fall within the "bill in advance days".
			Assert.AreEqual(datePayPlanCharge,AgingData.GetDateLastTrans(dictPatAgingTrans[patient.PatNum],dateStatement).Date);
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today.AddMonths(-1),new List<long>(),false,false,0,false,false,new List<long>(),
				false,false,new List<long>(),new List<long>(),dictPatAgingTrans);
			Assert.IsTrue(listPatAging.Any(x => x.PatNum==patient.PatNum),"The expected patient was not present in the AgingList.");
		}

		[TestMethod]
		public void AgingData_GetAgingData_DynamicPayPlanExplicitAdjustment() {
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			Patient pat=PatientT.CreatePatient(suffix);
			//Create a procedure made due during the 61_90 bucket and explicitly attach an adjustment to said procedure.
			DateTime procDate=DateTime.Today.AddDays(-70);
			Procedure proc=ProcedureT.CreateProcedure(pat,"DPPEA1",ProcStat.C,"",100,procDate: procDate,provNum: provNum);
			//Adjustments that are explicitly linked to procedures should inflate the value of the procedure.
			//Create an adjustment made due during the 31_60 bucket 
			DateTime adjDate=DateTime.Today.AddDays(-40);
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,50,adjDate: adjDate,procNum: proc.ProcNum,provNum: provNum);
			//Create a dynamic payment plan that is only attached to the procedure.
			//Act like the payment plan was created and made due during the 0_30 bucket.
			DateTime datePayPlan=DateTime.Today.AddDays(-10);
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,datePayPlan,0,0,0,new List<Procedure>(){ proc },
				new List<Adjustment>{ },frequency:PayPlanFrequency.Monthly,payCount:1);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			List<PatAging> listPatAgingSimple=Patients.GetAgingListSimple(null,new List<long>(){ pat.Guarantor });
			Assert.AreEqual(1,listPatAgingSimple.Count);
			Assert.AreEqual(1,listPatAgingSimple.Count(x => x.BalTotal==150
				&& x.Bal_0_30==150  //Payment plan - all due as of 10 days ago
				&& x.Bal_31_60==0   //Adjustment is technically in this bucket but the credit from the payment plan is applied to this bucket.
				&& x.Bal_61_90==0));//Procedure is technically in this bucket but the credit from the payment plan is applied to this bucket.
		}

		[TestMethod]
		public void AgingData_GetAgingData_DynamicPaymentPlanImplicitAdjustmentIgnore() {
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			long provNum2=ProviderT.CreateProvider($"{suffix}-2");
			Patient pat=PatientT.CreatePatient(suffix);
			//Create a procedure made due during the 61_90 bucket and implicitly attach an adjustment to said procedure.
			DateTime procDate=DateTime.Today.AddDays(-70);
			Procedure proc=ProcedureT.CreateProcedure(pat,"DPPIA1",ProcStat.C,"",100,procDate,provNum:provNum);
			//Adjustments that are implicitly linked to procedures should NOT inflate the value of the procedure.
			//Create an adjustment made due during the 31_60 bucket 
			DateTime adjDate=DateTime.Today.AddDays(-40);
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,50,adjDate: adjDate,procNum: proc.ProcNum,provNum: provNum2);
			//Create a dynamic payment plan that is only attached to the procedure.
			//Act like the payment plan was created and made due during the 0_30 bucket.
			DateTime datePayPlan=DateTime.Today.AddDays(-10);
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,datePayPlan,0,0,0,new List<Procedure>(){ proc },new List<Adjustment>{ },
				frequency:PayPlanFrequency.Monthly,payCount:1);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			List<PatAging> listPatAgingSimple=Patients.GetAgingListSimple(null,new List<long>(){ pat.Guarantor });
			Assert.AreEqual(1,listPatAgingSimple.Count);
			Assert.AreEqual(1,listPatAgingSimple.Count(x => x.BalTotal==150
				&& x.Bal_0_30==100  //Payment plan - all due as of 10 days ago
				&& x.Bal_31_60==50  //Adjustment is technically in this bucket which there isn't enough credit from the payment plan to cover.
				&& x.Bal_61_90==0));//Procedure is technically in this bucket but the credit from the payment plan is applied to this bucket.
		}

		[TestMethod]
		public void AgingData_GetAgingData_DynamicPaymentPlanImplicitAdjustmentAttached() {
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			long provNum2=ProviderT.CreateProvider($"{suffix}-2");
			Patient pat=PatientT.CreatePatient(suffix);
			//Create a procedure made due during the 61_90 bucket and implicitly attach an adjustment to said procedure.
			DateTime procDate=DateTime.Today.AddDays(-70);
			Procedure proc=ProcedureT.CreateProcedure(pat,"DPPIA1",ProcStat.C,"",100,procDate,provNum:provNum);
			//Adjustments that are implicitly linked to procedures should NOT inflate the value of the procedure.
			//Create an adjustment made due during the 31_60 bucket 
			DateTime adjDate=DateTime.Today.AddDays(-40);
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,50,adjDate: adjDate,procNum: proc.ProcNum,provNum: provNum2);
			//Create a dynamic payment plan that is attached to the procedure and the adjustment.
			//Act like the payment plan was created and made due during the 0_30 bucket.
			DateTime datePayPlan=DateTime.Today.AddDays(-10);
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,datePayPlan,0,0,0,new List<Procedure>(){ proc },
				new List<Adjustment>{ adj },frequency:PayPlanFrequency.Monthly,payCount:1);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			List<PatAging> listPatAgingSimple=Patients.GetAgingListSimple(null,new List<long>(){ pat.Guarantor });
			Assert.AreEqual(1,listPatAgingSimple.Count);
			Assert.AreEqual(1,listPatAgingSimple.Count(x => x.BalTotal==150
				&& x.Bal_0_30==150  //Payment plan - all due as of 10 days ago
				&& x.Bal_31_60==0   //Adjustment is technically in this bucket but the credit from the payment plan is applied to this bucket.
				&& x.Bal_61_90==0));//Procedure is technically in this bucket but the credit from the payment plan is applied to this bucket.
		}

		///<summary>Tests that the patients returned in GetAgingList() and GetAgingListSimple() are identical.</summary>
		[TestMethod]
		public void AgingData_GetAgingList_GetAgingListSimple_Compare() {
			PatientT.CreatePatWithProcAndStatement(2,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,50);
			List<Def> listBillTypeDef=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			List<long> listBillTypeDefNum=listBillTypeDef.Select(x => x.DefNum).ToList();
			//Compare the results of GetAgingList() and GetAgingListSimple() methods (output should be identical)
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today,listBillTypeDefNum,false,false,0,false,false,new List<long>(),false,false
				,new List<long>(),new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),false,false,true);
			List<PatAging> listPatAgingSimple=Patients.GetAgingListSimple(listBillTypeDefNum,new List<long> { });//Ordered by PatNum, for thread concurrency
			Assert.IsTrue(listPatAging.Count!=0);
			Assert.IsTrue(listPatAgingSimple.Count!=0);
			Assert.AreEqual(listPatAging.Count,listPatAgingSimple.Count);
			//Ensure both methods return the exact same patients.
			for(int i=0;i<listPatAging.Count;i++) {
				Assert.AreEqual(listPatAging[i].PatNum,listPatAgingSimple[i].PatNum);
			}
		}

		/// <summary>Tests the excludeAddr behavior in GetAgingList().</summary>
		[TestMethod]
		public void AgingData_GetAgingList_ExcludeBadAddress() {
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,50);//Bad Address (no zip code)
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,true,PatientStatus.Patient,StatementMode.Mail,false,50);//Valid Address
			List<Def> listBillTypeDef=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			List<long> listBillTypeDefNum=listBillTypeDef.Select(x => x.DefNum).ToList();
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today,listBillTypeDefNum,true,false,0
				,false,false,new List<long>(),false,false,new List<long>(),
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),false,false,true);
			Assert.AreEqual(listPatAging.Count,1);
			Assert.IsTrue(listPatAging[0].Zip!="");
		}

		/// <summary>Tests the excludeInactive bool in GetAgingList().</summary>
		[TestMethod]
		public void AgingData_GetAgingList_ExcludeInactiveFamilies() {
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Inactive,StatementMode.Mail,false,50);//Inactive patient
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,50);//Non-inactive patient
			List<Def> listBillTypeDef=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			List<long> listBillTypeDefNum=listBillTypeDef.Select(x => x.DefNum).ToList();
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today,listBillTypeDefNum,false,false,0
				,true,false,new List<long>(),false,false,new List<long>(),
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),false,false,true);
			Assert.AreEqual(listPatAging.Count,1);
			Assert.IsTrue(listPatAging[0].PatStatus!=PatientStatus.Inactive);
		}

		/// <summary>Tests the excludeInsPending parameter in GetAgingList().</summary>
		[TestMethod]
		public void AgingData_GetAgingList_ExcludeInsPending() {
			PatientT.CreatePatWithProcAndStatement(2,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,50);
			SerializableDictionary<long,PatAgingData> dictPatAgingData=AgingData.GetAgingData(false,true,true,false,false,new List<long>());
			dictPatAgingData.ToList()[0].Value.HasPendingIns=true;//Set the first Aging Patient to have pending insurance
			List<long> listPendingInsPatNums=new List<long>();			
			foreach(KeyValuePair<long,PatAgingData> kvp in dictPatAgingData) {//Grab the patnum since GetAgingList() requires a list of patnums
				if(kvp.Value.HasPendingIns) {
					listPendingInsPatNums.Add(kvp.Key);
				}
			}
			List<Def> listBillTypeDef=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			List<long> listBillTypeDefNum=listBillTypeDef.Select(x => x.DefNum).ToList();
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today,listBillTypeDefNum,false,false,0
				,true,false,new List<long>(),false,false,listPendingInsPatNums,
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),false,false,true);
			Assert.AreEqual(listPatAging.Count,1);
			Assert.IsTrue(listPatAging[0].HasInsPending==false);
		}

		/// <summary>Tests the ignoreInPerson parameter in GetAgingList().</summary>
		[TestMethod]
		public void AgingData_GetAgingList_ExcludeInPersonStatements() {
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.InPerson,false,50);//In Person Statement
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,50);//Mail Statement
			List<Def> listBillTypeDef=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			List<long> listBillTypeDefNum=listBillTypeDef.Select(x => x.DefNum).ToList();
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today,listBillTypeDefNum,false,false,0
				,false,false,new List<long>(),false,false,new List<long>(),
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),false,true,true);
			Assert.AreEqual(listPatAging.Count,2);
			listPatAging=Patients.GetAgingList("",DateTime.Today,listBillTypeDefNum,false,false,0
				,false,true,new List<long>(),false,false,new List<long>(),
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),false,true,true);
			Assert.AreEqual(listPatAging.Count,1);
			Statement statement=StatementT.GetStatementsForPat(listPatAging[0].PatNum).First();
			Assert.IsTrue(statement.Mode_!=StatementMode.InPerson);
		}

		/// <summary>Filter out accounts without Truth In Lending.</summary>
		[TestMethod]
		public void AgingData_GetAgingList_ExcludeNoTruthInLending() {
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,50);//No Signed Truth in Lending
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,true,50);//Signed Truth in Lending
			List<Def> listBillTypeDef=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			List<long> listBillTypeDefNum=listBillTypeDef.Select(x => x.DefNum).ToList();
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today,listBillTypeDefNum,false,false,0
				,false,false,new List<long>(),false,false,new List<long>(),
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),true,false,true);
			Assert.AreNotEqual(listPatAging.Count,0);
			listPatAging.ForEach(x=>Assert.IsTrue(x.HasSignedTil));
		}

		/// <summary>Tests the excludeLessThan parameter of GetAgingList().</summary>
		[TestMethod]
		public void AgingData_GetAgingList_ExcludeBalLessThan() {
			double balMin=50;
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,balMin-1);//Less than Balance
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,balMin);//Equal to Balance
			PatientT.CreatePatWithProcAndStatement(1,DateTime.Today,false,PatientStatus.Patient,StatementMode.Mail,false,balMin+1);//Greater than Balance
			List<Def> listBillTypeDef=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			List<long> listBillTypeDefNum=listBillTypeDef.Select(x => x.DefNum).ToList();
			List<PatAging> listPatAging=Patients.GetAgingList("",DateTime.Today,listBillTypeDefNum,false,false,balMin,false,false,new List<long>()
				,false,false,new List<long>(),new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),false,false,true);
			Assert.AreEqual(listPatAging.Count,2);
			Assert.IsTrue(listPatAging[0].BalTotal>=balMin);
			Assert.IsTrue(listPatAging[1].BalTotal>=balMin);
		}

		/// <summary>Tests the parameter filterSinceLastFinancialStatement in GetAgingList().</summary>
		[TestMethod]
		public void AgingData_GetAgingList_FilterAccountsNotBilledSince() {
			DateTime dateTimeSinceDate=DateTime.Today;
			//Create dummy Patients with sent dates from today and the last two days
			PatientT.CreatePatWithProcAndStatement(1,dateTimeSinceDate.AddDays(-1),false,PatientStatus.Patient,StatementMode.Mail,false,50);
			PatientT.CreatePatWithProcAndStatement(1,dateTimeSinceDate,false,PatientStatus.Patient,StatementMode.Mail,false,50);
			PatientT.CreatePatWithProcAndStatement(1,dateTimeSinceDate.AddDays(1),false,PatientStatus.Patient,StatementMode.Mail,false,50);
			List<Def> listBillTypeDef=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			List<long> listBillTypeDefNum=listBillTypeDef.Select(x => x.DefNum).ToList();
			List<PatAging> listPatAging=Patients.GetAgingList("",dateTimeSinceDate,listBillTypeDefNum,false,false,0
				,false,false,new List<long>(),false,false,new List<long>(),
				new List<long>(),new SerializableDictionary<long,List<PatAgingTransaction>>(),false,true,true);
			Assert.AreEqual(listPatAging.Count,2);
			Assert.IsTrue(listPatAging[0].DateLastStatement>=dateTimeSinceDate);
			Assert.IsTrue(listPatAging[1].DateLastStatement>=dateTimeSinceDate);
		}
	}
}
