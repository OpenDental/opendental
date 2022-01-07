using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.HieQueues_Tests {
	[TestClass]
	public class HieQueuesTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			HieClinicT.ClearHieClinicTable();
			HieQueueT.ClearHieQueueTable();
			EhrMeasureEventT.ClearEhrMeasureEventTable();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		///<summary>HieClinic not enabled. Zero HieQueues processed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_HieClinicNotEnabled_ZeroProcessed() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=1;
			HieClinic hieClinic=HieClinicT.CreateAndInsert(TimeSpan.FromHours(9),clinicNum:0,isEnabled:false);//Not enabled
			List<HieQueue> listHieQueues=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinic },numOfPatsWithQueues);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPats=EhrMeasureEventT.GetForPatsForType(listHieQueues.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcess=HieQueues.GetAll();
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(0,listEhrMeasureEventsForPats.Count);
			Assert.AreEqual(numOfPatsWithQueues,listHieQueuesAfterProcess.Count);
		}

		///<summary>HieClinic enabled but processing window not between the hieclinics processing time. Zero hiequeues processsed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_HieClinicEnabledNotTimeToProcess_ZeroProcessed() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=1;
			TimeSpan timeSpanToProcess=TimeSpan.FromHours(7);//7AM
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,6,59,0);//6:59 AM
			HieClinic hieClinic=HieClinicT.CreateAndInsert(timeSpanToProcess,flag:HieCarrierFlags.AllCarriers,isEnabled:true);//Enabled
			//DateTime now is before HIE clinics process time
			DateTime_.SetNow(() => dateTimeOfTest);
			List<HieQueue> listHieQueues=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinic },numOfPatsWithQueues);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPats=EhrMeasureEventT.GetForPatsForType(listHieQueues.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcess=HieQueues.GetAll();
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(0,listEhrMeasureEventsForPats.Count);
			Assert.AreEqual(numOfPatsWithQueues,listHieQueuesAfterProcess.Count);
		}

		///<summary>One HieClinic with HieCarrierFlags=AllPatients enabled and processing window between the hieclinics processing time. One HieQueue processed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_HieClinicEnabledWithAllPatientsTimeToProcess_OneProcessed() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=1;
			TimeSpan timeSpanToProcess=TimeSpan.FromHours(7);//7AM
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			HieClinic hieClinic=HieClinicT.CreateAndInsert(timeSpanToProcess,flag:HieCarrierFlags.AllCarriers,isEnabled:true);//Enabled
			//DateTime now is between HIE clinics process time
			DateTime_.SetNow(() => dateTimeOfTest);
			List<HieQueue> listHieQueues=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinic },numOfPatsWithQueues);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPats=EhrMeasureEventT.GetForPatsForType(listHieQueues.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcess=HieQueues.GetAll();
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(1,listEhrMeasureEventsForPats.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcess.Count);
		}

		///<summary>2 HieClinics with HieCarrierFlags=AllPatients with different processing times. Check that the hiequeues get processed correctly. Only the hiequeue associated to the hieclinic within processing window should be processed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_MultipleHieClinicWithFlagAllPatientsEnabled_OnlyOneHieClinicTimeToProcess_OneHieQueueProcessed() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=1;
			TimeSpan timeSpanToProcessA=TimeSpan.FromHours(7);//7AM
			TimeSpan timeSpanToProcessB=TimeSpan.FromHours(9);//9AM
			//Time of test will be between processing window for hieclinicA only
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			long clinicNumA=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicA=HieClinicT.CreateAndInsert(timeSpanToProcessA,flag:HieCarrierFlags.AllCarriers,clinicNum:clinicNumA,isEnabled:true);//Enabled
			long clinicNumB=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"B").ClinicNum;
			HieClinic hieClinicB=HieClinicT.CreateAndInsert(timeSpanToProcessB,flag:HieCarrierFlags.AllCarriers,clinicNum:clinicNumB,isEnabled:true);//Enabled
			//DateTime now is between HIE clinicA process time only
			DateTime_.SetNow(() => dateTimeOfTest);
			//Create hiequeues for each of the hieclinics
			List<HieQueue> listHieQueuesA=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicA },numOfPatsWithQueues);
			List<HieQueue> listHieQueuesB=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicB },numOfPatsWithQueues);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsA=EhrMeasureEventT.GetForPatsForType(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsB=EhrMeasureEventT.GetForPatsForType(listHieQueuesB.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessA=HieQueueT.GetAllForPats(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessB=HieQueueT.GetAllForPats(listHieQueuesB.Select(x => x.PatNum).ToList());
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(1,listEhrMeasureEventsForPatsA.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessA.Count);
			Assert.AreEqual(0,listEhrMeasureEventsForPatsB.Count);
			Assert.AreEqual(1,listHieQueuesAfterProcessB.Count);
		}

		///<summary>2 HieClinics with HieCarrierFlags=AllPatients enabled each with different processing time. Run two rounds of processing which fall between both hieclinic processing windows. After both rounds, all hiequeues should get processed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_MultipleHieClinicWithFlagAllPatientsEnabled_TwoRoundsOfProcess_SecondRoundShouldProcessAllHieQueues() {
			//First, setup the test scenario.
			int numOfPatsWithQueuesA=1;
			TimeSpan timeSpanToProcessA=TimeSpan.FromHours(7);//7AM
			TimeSpan timeSpanToProcessB=TimeSpan.FromHours(9);//9AM
			//Time of test will be between processing window for hieclinicA only
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			long clinicNumA=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicA=HieClinicT.CreateAndInsert(timeSpanToProcessA,flag:HieCarrierFlags.AllCarriers,clinicNum:clinicNumA,isEnabled:true);//Enabled
			long clinicNumB=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"B").ClinicNum;
			HieClinic hieClinicB=HieClinicT.CreateAndInsert(timeSpanToProcessB,flag:HieCarrierFlags.AllCarriers,clinicNum:clinicNumB,isEnabled:true);//Enabled
			//DateTime now is between HIE clinicA process time only
			DateTime_.SetNow(() => dateTimeOfTest);
			//Create hiequeues for each of the hieclinics
			List<HieQueue> listHieQueuesA=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicA },numOfPatsWithQueuesA);
			List<HieQueue> listHieQueuesB=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicB },numOfPatsWithQueuesA);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsA=EhrMeasureEventT.GetForPatsForType(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsB=EhrMeasureEventT.GetForPatsForType(listHieQueuesB.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessA=HieQueueT.GetAllForPats(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessB=HieQueueT.GetAllForPats(listHieQueuesB.Select(x => x.PatNum).ToList());
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(1,listEhrMeasureEventsForPatsA.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessA.Count);
			Assert.AreEqual(0,listEhrMeasureEventsForPatsB.Count);
			Assert.AreEqual(1,listHieQueuesAfterProcessB.Count);
			int numOfPatsWithQueuesB=2;
			listHieQueuesB.AddRange(HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicB },numOfPatsWithQueuesB,doCreateMedicaidInsplan:true));
			dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,9,15,0);//9:15 AM
			//DateTime now is between hieclinicB process time
			DateTime_.SetNow(() => dateTimeOfTest);
			HieQueues.ProcessQueues();
			listEhrMeasureEventsForPatsB=EhrMeasureEventT.GetForPatsForType(listHieQueuesB.Select(x => x.PatNum).ToList());
			listHieQueuesAfterProcessB=HieQueueT.GetAllForPats(listHieQueuesB.Select(x => x.PatNum).ToList());
			Assert.AreEqual(numOfPatsWithQueuesA+numOfPatsWithQueuesB,listEhrMeasureEventsForPatsB.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessB.Count);
		}

		///<summary>One HieClinic with HieCarrierFlags=Medicaid enabled and processing window between the hieclinics processing time. Zero patients have medicaid carrier. HieQueue will not get processed but should still be removed from the hiequeue table..</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_HieClinicEnabledWithFlagMedicaidTimeToProcess_PatientDoesNotHaveMedicaidPlan() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=1;
			TimeSpan timeSpanToProcess=TimeSpan.FromHours(7);//7AM
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			HieClinic hieClinic=HieClinicT.CreateAndInsert(timeSpanToProcess,flag:HieCarrierFlags.Medicaid,isEnabled:true);//Enabled
			//DateTime now is between HIE clinics process time
			DateTime_.SetNow(() => dateTimeOfTest);
			//Patient will be created with no corresponding medicaid plan. No hiequeues will be processed. 
			List<HieQueue> listHieQueues=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinic },numOfPatsWithQueues,doCreateMedicaidInsplan:false);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPats=EhrMeasureEventT.GetForPatsForType(listHieQueues.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcess=HieQueues.GetAll();
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(0,listEhrMeasureEventsForPats.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcess.Count);
		}

		///<summary>One HieClinic with HieCarrierFlags=Medicaid enabled and processing window between the hieclinics processing time. One medicaid HieQueue processed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_HieClinicEnabledWithFlagMedicaidTimeToProcess_PatientDoesHaveMedicaidPlan() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=1;
			TimeSpan timeSpanToProcess=TimeSpan.FromHours(7);//7AM
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			HieClinic hieClinic=HieClinicT.CreateAndInsert(timeSpanToProcess,flag:HieCarrierFlags.Medicaid,isEnabled:true);//Enabled
			//DateTime now is between HIE clinics process time
			DateTime_.SetNow(() => dateTimeOfTest);
			//Patient will get created with medicaid insplan
			List<HieQueue> listHieQueues=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinic },numOfPatsWithQueues,doCreateMedicaidInsplan:true);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPats=EhrMeasureEventT.GetForPatsForType(listHieQueues.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcess=HieQueues.GetAll();
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(1,listEhrMeasureEventsForPats.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcess.Count);
		}

		///<summary>Two HieClinic with HieCarrierFlags=Medicaid enabled and only one with the processing window between the hieclinics processing time. Only One HieQueue processed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_MultipleHieClinicEnabledWithFlagMedicaidTimeToProcess_OnlyOneHieClinicTimeToProcess_OneHieQueueProcessed() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=1;
			TimeSpan timeSpanToProcessA=TimeSpan.FromHours(7);//7AM
			TimeSpan timeSpanToProcessB=TimeSpan.FromHours(9);//9AM
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			long clinicNumA=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicA=HieClinicT.CreateAndInsert(timeSpanToProcessA,clinicNum:clinicNumA,flag:HieCarrierFlags.Medicaid,isEnabled:true);//Enabled
			long clinicNumB=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicB=HieClinicT.CreateAndInsert(timeSpanToProcessB,clinicNum:clinicNumB,flag:HieCarrierFlags.Medicaid,isEnabled:true);//Enabled
			//DateTime now is between HIE clinics process time
			DateTime_.SetNow(() => dateTimeOfTest);
			//Patient will get created with medicaid insplan
			List<HieQueue> listHieQueuesA=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicA },numOfPatsWithQueues,doCreateMedicaidInsplan:true);
			List<HieQueue> listHieQueuesB=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicB },numOfPatsWithQueues,doCreateMedicaidInsplan:true);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsA=EhrMeasureEventT.GetForPatsForType(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsB=EhrMeasureEventT.GetForPatsForType(listHieQueuesB.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessA=HieQueueT.GetAllForPats(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessB=HieQueueT.GetAllForPats(listHieQueuesB.Select(x => x.PatNum).ToList());
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(1,listEhrMeasureEventsForPatsA.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessA.Count);
			Assert.AreEqual(0,listEhrMeasureEventsForPatsB.Count);
			Assert.AreEqual(1,listHieQueuesAfterProcessB.Count);
		}

		///<summary>Two HieClinic with HieCarrierFlags=Medicaid enabled each with different processing time. Run two rounds of processing which fall between both hieclinic processing windows. After both rounds, all hiequeues should get processed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_MultipleHieClinicEnabledWithFlagMedicaidTimeToProcess_TwoRoundsOfProcess_SecondRoundShouldProcessAllHieQueues() {
			//First, setup the test scenario.
			int numOfPatsWithQueuesA=1;
			TimeSpan timeSpanToProcessA=TimeSpan.FromHours(7);//7AM
			TimeSpan timeSpanToProcessB=TimeSpan.FromHours(9);//9AM
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			long clinicNumA=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicA=HieClinicT.CreateAndInsert(timeSpanToProcessA,clinicNum:clinicNumA,flag:HieCarrierFlags.Medicaid,isEnabled:true);//Enabled
			long clinicNumB=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicB=HieClinicT.CreateAndInsert(timeSpanToProcessB,clinicNum:clinicNumB,flag:HieCarrierFlags.Medicaid,isEnabled:true);//Enabled
			//DateTime now is between HIE clinics process time
			DateTime_.SetNow(() => dateTimeOfTest);
			//Patient will get created with medicaid insplan
			List<HieQueue> listHieQueuesA=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicA },numOfPatsWithQueuesA,doCreateMedicaidInsplan:true);
			List<HieQueue> listHieQueuesB=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicB },numOfPatsWithQueuesA,doCreateMedicaidInsplan:true);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsA=EhrMeasureEventT.GetForPatsForType(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsB=EhrMeasureEventT.GetForPatsForType(listHieQueuesB.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessA=HieQueueT.GetAllForPats(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessB=HieQueueT.GetAllForPats(listHieQueuesB.Select(x => x.PatNum).ToList());
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(numOfPatsWithQueuesA,listEhrMeasureEventsForPatsA.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessA.Count);
			Assert.AreEqual(0,listEhrMeasureEventsForPatsB.Count);
			Assert.AreEqual(1,listHieQueuesAfterProcessB.Count);
			//Change the processing time to the window of hieClinicB
			//add more hiequeues to process the second time.
			int numOfPatsWithQueuesB=2;
			listHieQueuesB.AddRange(HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicB },numOfPatsWithQueuesB,doCreateMedicaidInsplan:true));
			dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,9,15,0);//9:15 AM
			//DateTime now is between hieclinicB process time
			DateTime_.SetNow(() => dateTimeOfTest);
			HieQueues.ProcessQueues();
			listEhrMeasureEventsForPatsB=EhrMeasureEventT.GetForPatsForType(listHieQueuesB.Select(x => x.PatNum).ToList());
			listHieQueuesAfterProcessB=HieQueueT.GetAllForPats(listHieQueuesB.Select(x => x.PatNum).ToList());
			Assert.AreEqual(numOfPatsWithQueuesA+numOfPatsWithQueuesB,listEhrMeasureEventsForPatsB.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessB.Count);
		}

		///<summary>Multiple HieClinic with both HieCarrierFlags in (Medicaid, AllPatient) enabled. All within processing window. Process all hiequeues.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_MultipleHieClinicEnabledWithFlagMedicaidAndAllPatientTimeToProcess_AllHieQueueProcessed() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=4;
			TimeSpan timeSpanToProcessA=TimeSpan.FromHours(7);//7AM
			TimeSpan timeSpanToProcessB=TimeSpan.FromHours(7);//7AM
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			long clinicNumA=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicA=HieClinicT.CreateAndInsert(timeSpanToProcessA,clinicNum:clinicNumA,flag:HieCarrierFlags.Medicaid,isEnabled:true);//Enabled
			long clinicNumB=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicB=HieClinicT.CreateAndInsert(timeSpanToProcessB,clinicNum:clinicNumB,flag:HieCarrierFlags.AllCarriers,isEnabled:true);//Enabled
			//DateTime now is between HIE clinics process time
			DateTime_.SetNow(() => dateTimeOfTest);
			//Patient will get created with medicaid insplan
			List<HieQueue> listHieQueuesA=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicA },numOfPatsWithQueues,doCreateMedicaidInsplan:true);
			List<HieQueue> listHieQueuesB=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicB },numOfPatsWithQueues,doCreateMedicaidInsplan:true);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsA=EhrMeasureEventT.GetForPatsForType(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsB=EhrMeasureEventT.GetForPatsForType(listHieQueuesB.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessA=HieQueueT.GetAllForPats(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessB=HieQueueT.GetAllForPats(listHieQueuesB.Select(x => x.PatNum).ToList());
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(numOfPatsWithQueues,listEhrMeasureEventsForPatsA.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessA.Count);
			Assert.AreEqual(numOfPatsWithQueues,listEhrMeasureEventsForPatsB.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessB.Count);
		}

		///<summary>Multiple HieClinic with both HieCarrierFlags in (Medicaid, AllPatient) enabled. Only one within processing window. Only hiequeues between processing window processed.</summary>
		[TestMethod]
		public void HieQueue_ProcessThread_MultipleHieClinicEnabledWithFlagMedicaidAndAllPatient_OnlyOneHieClinicTimeToProcess() {
			//First, setup the test scenario.
			int numOfPatsWithQueues=4;
			TimeSpan timeSpanToProcessA=TimeSpan.FromHours(7);//7AM
			TimeSpan timeSpanToProcessB=TimeSpan.FromHours(9);//9AM
			DateTime dateTimeOfTest=new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Date.Day,7,15,0);//7:15 AM
			long clinicNumA=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicA=HieClinicT.CreateAndInsert(timeSpanToProcessA,clinicNum:clinicNumA,flag:HieCarrierFlags.Medicaid,isEnabled:true);//Enabled
			long clinicNumB=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"A").ClinicNum;
			HieClinic hieClinicB=HieClinicT.CreateAndInsert(timeSpanToProcessB,clinicNum:clinicNumB,flag:HieCarrierFlags.AllCarriers,isEnabled:true);//Enabled
			//DateTime now is between HIE clinics process time
			DateTime_.SetNow(() => dateTimeOfTest);
			//Patient will get created with medicaid insplan
			List<HieQueue> listHieQueuesA=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicA },numOfPatsWithQueues,doCreateMedicaidInsplan:true);
			List<HieQueue> listHieQueuesB=HieQueueT.CreateForHieClinics(new List<HieClinic>() { hieClinicB },numOfPatsWithQueues,doCreateMedicaidInsplan:true);
			//Next, perform the thing you're trying to test.
			HieQueues.ProcessQueues();
			//Then, get anything necessary from the database to see if the test is correct.
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsA=EhrMeasureEventT.GetForPatsForType(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<EhrMeasureEvent> listEhrMeasureEventsForPatsB=EhrMeasureEventT.GetForPatsForType(listHieQueuesB.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessA=HieQueueT.GetAllForPats(listHieQueuesA.Select(x => x.PatNum).ToList());
			List<HieQueue> listHieQueuesAfterProcessB=HieQueueT.GetAllForPats(listHieQueuesB.Select(x => x.PatNum).ToList());
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(numOfPatsWithQueues,listEhrMeasureEventsForPatsA.Count);
			Assert.AreEqual(0,listHieQueuesAfterProcessA.Count);
			Assert.AreEqual(0,listEhrMeasureEventsForPatsB.Count);
			Assert.AreEqual(numOfPatsWithQueues,listHieQueuesAfterProcessB.Count);
		}

	}
}
