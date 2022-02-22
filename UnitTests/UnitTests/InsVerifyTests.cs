using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.InsVerify_Tests {
	[TestClass]
	public class InsVerifyTests:TestBase {
		
		private static long _patNumTrusted;
		private static long _patNumTrustedSecond;
		private static long _patNumNotTrusted;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			AppointmentT.ClearAppointmentTable();//Make sure any other appts do not show on insurance verification list.
			ClearinghouseT.ClearClearinghouseTable();//Must be cleared because we expect the 270 request to fail when a real request is attempted.
			_patNumTrusted=PatientT.CreatePatient().PatNum;
			CreateNewCarrierAndPatPlan(_patNumTrusted,true);
			AppointmentT.CreateAppointment(_patNumTrusted,DateTime.Now.AddDays(2),0,0);//Causes patient patplan to show in insurance verification list.
			_patNumTrustedSecond=PatientT.CreatePatient().PatNum;
			CreateNewCarrierAndPatPlan(_patNumTrustedSecond,true);
			AppointmentT.CreateAppointment(_patNumTrustedSecond,DateTime.Now.AddDays(3),0,0);//Causes patient patplan to show in insurance verification list.
			_patNumNotTrusted=PatientT.CreatePatient().PatNum;
			CreateNewCarrierAndPatPlan(_patNumNotTrusted,false);
			AppointmentT.CreateAppointment(_patNumNotTrusted,DateTime.Now.AddDays(4),0,0);//Causes patient patplan to show in insurance verification list.
		}

		[TestMethod]
		public void InsVerifies_VerifyDateTest() {
			List<List<int>> testCases=new List<List<int>>();
			#region testCase setup
			for(int i=0;i<14;i++) {
				testCases.Add(null);
				switch(i) {
					case 0:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 1:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 2:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 3:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 4:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 5:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 6:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 7:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 8:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 9:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 10:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 11:
						testCases[i]=new List<int> {
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.NextScheduledVerifyDate,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime
						};
						break;
					case 12:
						testCases[i]=new List<int> { 
							(int)eventType.DateLastVerified,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime,
							(int)eventType.NextScheduledVerifyDate
						};
						break;
					case 13:
						testCases[i]=new List<int> { 
							(int)eventType.DateLastVerified,
							(int)eventType.ApptSchedDaysMaxDaysForVerification,
							(int)eventType.DateTimeInsuranceRenewalNeeded,
							(int)eventType.DateTimeApptWasScheduled,
							(int)eventType.DateTimeVerified,
							(int)eventType.AptDateTime,
							(int)eventType.NextScheduledVerifyDate
						};
						break;
					default:
						break;
				}
			}
			#endregion
			//This is a list of the possible dates times.
			//For each of the test cases the first items in the case list gets the first date, second gets the second etc...
			List<DateTime> dates=new List<DateTime> {
				new DateTime(2018,2,23,0,0,0),	//2-23-2018    0:00
				new DateTime(2018,2,23,1,0,0),	//2-23-2018		1:00
				new DateTime(2018,2,23,2,0,0),	//"       "		2:00
				new DateTime(2018,2,23,3,0,0),	//etc etc....
				new DateTime(2018,2,23,4,0,0),
				new DateTime(2018,2,23,5,0,0),
				new DateTime(2018,2,23,6,0,0)
			};
			//List of "Hours" we know are right. The hour corresponds to the index of the test case list that is correct.
			List<int> expectedResults=new List<int> {3,3,3,3,3,3,3,3,3,3,3,4,3,3};	//expectedResults[i] == results of passing test of testCases[i]
			//Loop through and verify test cases
			Console.WriteLine("See [[Insurance Verification - Hours Available for Verification]] for the visual diagram corresponding to the test number.");
			for(int i=0;i<testCases.Count;i++) { 
				List<int> items=testCases[i];
				DateTime daysForVerification=DateTime.MaxValue;	//Max value so they're out of the way
				DateTime appointmentDate=DateTime.MaxValue;
				DateTime needsVerifyDate=DateTime.MaxValue;
				DateTime lastVerifyDate=DateTime.MaxValue;
				DateTime benifitRenewalDate=DateTime.MaxValue;
				#region TimeCreation
				int currentDate=0;
				foreach(int index in items) {
					switch(index) {
						case (int)eventType.DateLastVerified:
							lastVerifyDate=dates[currentDate];
							currentDate++;
							break;
						case (int)eventType.NextScheduledVerifyDate:
							needsVerifyDate=dates[currentDate];
							currentDate++;
							break;
						case (int)eventType.DateTimeApptWasScheduled:
							appointmentDate=dates[currentDate];
							currentDate++;
							break;
						case (int)eventType.ApptSchedDaysMaxDaysForVerification:
							daysForVerification=dates[currentDate];
							currentDate++;
							break;
						case (int)eventType.DateTimeInsuranceRenewalNeeded:
							benifitRenewalDate=dates[currentDate];
							currentDate++;
							break;
						//These two aren't used in the calculation, so we just increment the datetime index
						case (int)eventType.AptDateTime:
							currentDate++;
							break;
						case (int)eventType.DateTimeVerified:
							currentDate++;
							break;
					}
				}
				#endregion
				DateTime resultDate=InsVerifies.VerifyDateCalulation(daysForVerification,appointmentDate,needsVerifyDate,benifitRenewalDate);
				Console.WriteLine((expectedResults[i]==resultDate.Hour ? "PASSED: " : "FAILED: ")+"Test "+(i+1));
				Assert.AreEqual(expectedResults[i],resultDate.Hour);//If the times are equal, then the result is correct.
			}
		}

		[TestMethod]
		public void InsVerifies_ValidateGroupNumber_NoGroupNumReceived() {
			string groupNumOd="93-00025";
			string groupNum271="";
			Assert.AreEqual("",InsVerifies.ValidateGroupNumber(groupNumOd,groupNum271));
		}

		[TestMethod]
		public void InsVerifies_ValidateGroupNumber_GroupNumReceivedStartsWith0s() {
			string groupNumOd="93-00025";
			string groupNum271="0093-00025";
			Assert.AreEqual("",InsVerifies.ValidateGroupNumber(groupNumOd,groupNum271));
		}

		[TestMethod]
		public void InsVerifies_ValidateGroupNumber_GroupNumReceivedStartsWithGroupNumInOd() {
			string groupNumOd="93-00025";
			string groupNum271="93-00025-01";
			Assert.AreEqual("",InsVerifies.ValidateGroupNumber(groupNumOd,groupNum271));
		}

		[TestMethod]
		public void InsVerifies_ValidateGroupNumber_GroupNumReceivedEndsWith0s() {
			string groupNumOd="93-00025";
			string groupNum271="93-00025000";
			Assert.AreEqual("",InsVerifies.ValidateGroupNumber(groupNumOd,groupNum271));
		}

		[TestMethod]
		public void InsVerifies_ValidateGroupNumber_GroupNumReceivedContainsGroupNumInOd() {
			string groupNumOd="93-00025";
			string groupNum271="1093-00025468";
			Assert.AreNotEqual("",InsVerifies.ValidateGroupNumber(groupNumOd,groupNum271));
		}

		[TestMethod]
		public void InsVerifies_ValidateGroupNumber_InvalidGroupNumInOd() {
			string groupNumOd="11";
			string groupNum271="1193-00025468";
			Assert.AreNotEqual("",InsVerifies.ValidateGroupNumber(groupNumOd,groupNum271));
		}

		[TestMethod]
		public void InsVerifies_ValidatePlanDates_NoStartAndEndDateReceived() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,0,provNum);
			//No plan dates were received from the 271. DateTime.MinVal is what X12Parse.ToDate() will return if the DTP segment does not specify a date.
			string errorStatus=InsVerifies.ValidatePlanDates(DateTime.MinValue,DateTime.MinValue,insSub,apt.AptNum);
			Assert.AreEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidatePlanDates_ValidEndDateReceived() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,0,provNum);
			//A valid end date was parsed out of the 271.
			DateTime dateEndFrom271=DateTime.Today.AddDays(1);
			InsVerifies.ValidatePlanDates(DateTime.MinValue,dateEndFrom271,insSub,apt.AptNum);
			insSub=InsSubs.GetOne(insSub.InsSubNum);
			//The insSub.DateTerm should have been updated in ValidatePlanDates()
			Assert.AreEqual(insSub.DateTerm,dateEndFrom271);
		}

		[TestMethod]
		public void InsVerifies_ValidatePlanDates_InvalidEndDateReceived() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,0,provNum);
			//An end date was parsed out of the 271, but it ends before our appointment date.
			DateTime dateEndFrom271=DateTime.Today.AddDays(-1);
			string errorStatus=InsVerifies.ValidatePlanDates(DateTime.MinValue,dateEndFrom271,insSub,apt.AptNum);
			//An error status should have been made because the date that was parsed from the 271 is invalid.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidatePlanDates_ValidStartDateReceived() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,0,provNum);
			//A valid start date was parsed out of the 271.
			DateTime dateStartFrom271=DateTime.Today.AddDays(-1);
			InsVerifies.ValidatePlanDates(dateStartFrom271,DateTime.MinValue,insSub,apt.AptNum);
			insSub=InsSubs.GetOne(insSub.InsSubNum);
			//The insSub.DateEffective should have been updated in ValidatePlanDates()
			Assert.AreEqual(insSub.DateEffective,dateStartFrom271);
		}

		[TestMethod]
		public void InsVerifies_ValidatePlanDates_InvalidStartDateReceived() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,0,provNum);
			//A start date was parsed out of the 271, but it starts after our appointment date.
			DateTime dateStartFrom271=DateTime.Today.AddDays(1);
			string errorStatus=InsVerifies.ValidatePlanDates(dateStartFrom271,DateTime.MinValue,insSub,apt.AptNum);
			//An error status should have been made because the date that was parsed from the 271 is invalid.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidatePlanDates_ValidStartAndEndDateReceived() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,0,provNum);
			//A valid start date was parsed out of the 271.
			DateTime dateStartFrom271=DateTime.Today.AddDays(-1);
			DateTime dateEndFrom271=DateTime.Today.AddYears(1);
			InsVerifies.ValidatePlanDates(dateStartFrom271,dateEndFrom271,insSub,apt.AptNum);
			insSub=InsSubs.GetOne(insSub.InsSubNum);
			//The insSub.DateEffective and insSub.DateTerm should have been updated in ValidatePlanDates() because they are valid.
			Assert.AreEqual(insSub.DateTerm,dateEndFrom271);
			Assert.AreEqual(insSub.DateEffective,dateStartFrom271);
		}

		[TestMethod]
		public void InsVerifies_ValidatePlanDates_InvalidStartAndEndDateReceived() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,0,provNum);
			//Invalid plan dates were parsed from the 271.
			DateTime dateStartFrom271=DateTime.Today.AddYears(-1);
			DateTime dateEndFrom271=DateTime.Today.AddDays(-1);
			string errorStatus=InsVerifies.ValidatePlanDates(dateStartFrom271,dateEndFrom271,insSub,apt.AptNum);
			//An error status should be received from ValidatePlanDates() since the parsed dates were older than our appointment date.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidatePlanDates_ValidStartDateMinValEndDate() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,0,provNum);
			//Invalid plan dates were parsed from the 271.
			DateTime dateStartFrom271=DateTime.Today.AddYears(-1);
			DateTime dateEndFrom271=DateTime.MinValue;
			string errorStatus=InsVerifies.ValidatePlanDates(dateStartFrom271,dateEndFrom271,insSub,apt.AptNum);
			//When a valid start date is received we will update the policy end date always, per NADG.
			Assert.AreEqual("",errorStatus);
			Assert.IsTrue(insSub.DateEffective==dateStartFrom271);
			Assert.IsTrue(insSub.DateTerm==DateTime.MinValue);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_NoBensReceivedFrom271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Because no benefits were specified in the 271 no validation should have happened in ValidateAnnualMaxAndGeneralDeductible().
			Assert.AreEqual(errorStatus,"");
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_IndAnnualMaxReceivedWithValueInOd() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,500);
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Value from 271 differs from what is in OD, an error status should be returned.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_IndAnnualMaxMultipleReceivedFrom271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271_1=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indAnnualMax271_2=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,200);
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271_1, indAnnualMax271_2, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Multiple annual max's were specified in the 271, because one matches the value in OD, no error status should be returned.
			Assert.AreEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_IndAnnualMaxValueZeroInOdNonzeroIn271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,0);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,500);
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Open Dental had an individual annual max of 0 but the 271 specified a non-zero amount (500 in this case). Thus an error status should be returned.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_FamilyAnnualMaxReceivedWithValueInOd() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,1500);
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//271 specified a different value than what was in OD, an error status should be returned.
			Assert.AreNotEqual("",errorStatus);
		}
		

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_FamilyAnnualMaxMultipleReceivedFrom271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271_1=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,1500);
			Benefit famAnnualMax271_2=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271_1, famAnnualMax271_2, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Multiple annual max's were specified in the 271, because one matches the value in OD, no error status should be returned.
			Assert.AreEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_FamilyAnnualMaxValueZeroInOdNonZeroIn271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,0);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,1500);
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Benefit amount in OD was 0 but 271 specifed a non-zero value. An error status should be returned in this scenario.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_IndGeneralDeductibleReceivedWithValueInOd() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,150);
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//271 specified a different value than what was in OD, an error status should be returned.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_IndGeneralDeductibleMultipleReceivedFrom271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271_1=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,150);
			Benefit indGeneralDeduct271_2=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271_1, indGeneralDeduct271_2, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Multiple individual general deductibles were specified in the 271, because one of the received benefits matches the value in OD, no error status should be returned.
			Assert.AreEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_IndGeneralDeductibleValueZeroInOdNonZeroIn271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,0);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,150);
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=null;
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Benefit amount in OD was 0 but 271 specifed a non-zero value. An error status should be returned in this scenario.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_FamilyGeneralDeductibleReceivedWithValueInOd() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,150);
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//271 specified a different value than what was in OD, an error status should be returned.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_FamilyGeneralDeductibleMultipleReceivedFrom271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271_1=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,200);
			Benefit famGeneralDeduct271_2=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,150);
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271_1, famGeneralDeduct271_2};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//271 specified a different value than what was in OD, an error status should be returned.
			Assert.AreEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_ValidateAnnualMaxAndGeneralDeductible_FamilyGeneralDeductibleValueZeroInOdNonZeroIn271() {
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create benefits for the patient in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,1000);
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,100);
			Benefit famAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,2000);
			Benefit famGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,0);
			//Update insplan just in case we need the link later on.
			indAnnualMax.PlanNum=insPlan.PlanNum;
			indGeneralDeduct.PlanNum=insPlan.PlanNum;
			famAnnualMax.PlanNum=insPlan.PlanNum;
			famGeneralDeduct.PlanNum=insPlan.PlanNum;
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax, indGeneralDeduct, famAnnualMax, famGeneralDeduct};
			//Create benefits that were 'received' from the 271.
			Benefit indAnnualMax271=null;
			Benefit indGeneralDeduct271=null;
			Benefit famAnnualMax271=null;
			Benefit famGeneralDeduct271=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Family,150);
			List<Benefit> listBensFrom271=new List<Benefit>(){indAnnualMax271, indGeneralDeduct271, famAnnualMax271, famGeneralDeduct271};
			string errorStatus=InsVerifies.ValidateAnnualMaxAndGeneralDeductible(listBensInOd,listBensFrom271);
			//Benefit amount in OD was 0 but 271 specifed a non-zero value. An error status should be returned in this scenario.
			Assert.AreNotEqual("",errorStatus);
		}

		[TestMethod]
		public void InsVerifies_CreateInsuranceAdjustmentIfNeeded_NoIndAnnualMaxInOd() {
			ClaimProcT.ClearClaimProcTable();//Just in case
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create EB271 object. The EB271 objects are taken from real 271 reponses using NADG's database.
			X12Separators separators=new X12Separators();//271's appear to follow the default X12 separators
			X12Segment segment=new X12Segment("EB*F*IND*35**DG PLUS, NON CONTRACTED*29*2000.00*****U~",separators);//Represents an individual annual max
			EB271 eb271=new EB271(segment,false,false);
			List<EB271> listEb271s=new List<EB271>(){eb271};
			//For this test there are no individual annual max or general deductible benefits in OD
			InsVerifies.CreateInsuranceAdjustmentIfNeeded(pat.PatNum,insPlan.PlanNum,insSub.InsSubNum,new List<Benefit>(),listEb271s);
			List<ClaimProc> listInsAdjs=ClaimProcs.Refresh(pat.PatNum);
			//Because there is no benefit value in Open Dental no insurance adjustment should have been made.
			Assert.IsTrue(listInsAdjs.Count==0);
		}

		[TestMethod]
		public void InsVerifies_CreateInsuranceAdjustmentIfNeeded_IndAnnualMaxRemainingAmtReceived() {
			ClaimProcT.ClearClaimProcTable();//Just in case
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create individual annual max in Open Dental
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,3000);
			List<Benefit> listBensInOd=new List<Benefit>(){indAnnualMax};
			//Create EB271 object. The EB271 objects are taken from real 271 reponses using NADG's database.
			X12Separators separators=new X12Separators();//271's appear to follow the default X12 separators
			X12Segment segment=new X12Segment("EB*F*IND*35**DG PLUS, NON CONTRACTED*29*2000.00*****U~",separators);//Represents an individual annual max
			EB271 eb271=new EB271(segment,false,false);
			List<EB271> listEb271s=new List<EB271>(){eb271};
			InsVerifies.CreateInsuranceAdjustmentIfNeeded(pat.PatNum,insPlan.PlanNum,insSub.InsSubNum,listBensInOd,listEb271s);
			List<ClaimProc> listInsAdjs=ClaimProcs.Refresh(pat.PatNum);
			//Only one insurance adjustment claimproc should have been made
			Assert.IsTrue(listInsAdjs.Count<=1);
			//Should have 1000 insurance used on the adjustment (benefitAmt-AmtRemaining)
			Assert.IsTrue(listInsAdjs[0].InsPayAmt==1000);
		}

		[TestMethod]
		public void InsVerifies_CreateInsuranceAdjustmentIfNeeded_NoIndGeneralDeductInOd() {
			ClaimProcT.ClearClaimProcTable();//Just in case
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create EB271 object. The EB271 objects are taken from real 271 reponses using NADG's database.
			X12Separators separators=new X12Separators();//271's appear to follow the default X12 separators
			X12Segment segment=new X12Segment("EB*C*IND*35**DG PLUS, NON CONTRACTED*29*50.00*****U~",separators);//Represents an individual general deductible
			EB271 eb271=new EB271(segment,false,false);
			List<EB271> listEb271s=new List<EB271>(){eb271};
			InsVerifies.CreateInsuranceAdjustmentIfNeeded(pat.PatNum,insPlan.PlanNum,insSub.InsSubNum,new List<Benefit>(),listEb271s);
			List<ClaimProc> listInsAdjs=ClaimProcs.Refresh(pat.PatNum);
			//Because there is no benefit value in Open Dental no insurance adjustment should have been made.
			Assert.IsTrue(listInsAdjs.Count==0);
		}

		[TestMethod]
		public void InsVerifies_CreateInsuranceAdjustmentIfNeeded_IndGeneralDeductRemainingAmtReceived() {
			ClaimProcT.ClearClaimProcTable();//Just in case
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create Open Dental benefits
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,200);
			List<Benefit> listBensInOd=new List<Benefit>(){indGeneralDeduct};
			//Create EB271 object. The EB271 objects are taken from real 271 reponses using NADG's database.
			X12Separators separators=new X12Separators();//271's appear to follow the default X12 separators
			X12Segment segment=new X12Segment("EB*C*IND*35**DG PLUS, NON CONTRACTED*29*50.00*****U~",separators);//Represents an individual annual max
			EB271 eb271=new EB271(segment,false,false);
			List<EB271> listEb271s=new List<EB271>(){eb271};
			//For this test there is are no individual annual max or general deductible benefits in OD
			InsVerifies.CreateInsuranceAdjustmentIfNeeded(pat.PatNum,insPlan.PlanNum,insSub.InsSubNum,listBensInOd,listEb271s);
			List<ClaimProc> listInsAdjs=ClaimProcs.Refresh(pat.PatNum);
			//Only one insurance adjustment claimproc should have been made
			Assert.IsTrue(listInsAdjs.Count<=1);
			//Should have 150 deductible used on the adjustment (benefitAmt-AmtRemaining)
			Assert.IsTrue(listInsAdjs[0].DedApplied==150);
		}

		[TestMethod]
		public void InsVerifies_CreateInsuranceAdjustmentIfNeeded_IndAnnnualMaxAndIndGeneralDeductRemainingAmtReceived() {
			ClaimProcT.ClearClaimProcTable();//Just in case
			string suffix=MethodInfo.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			long provNum=ProviderT.CreateProvider(suffix);
			//Create Open Dental benefits
			Benefit indGeneralDeduct=BenefitT.CreateDeductibleGeneral(insPlan.PlanNum,BenefitCoverageLevel.Individual,200);
			Benefit indAnnualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,3000);
			List<Benefit> listBensInOd=new List<Benefit>(){indGeneralDeduct,indAnnualMax};
			//Create EB271 object. The EB271 objects are taken from real 271 reponses using NADG's database.
			X12Separators separators=new X12Separators();//271's appear to follow the default X12 separators
			X12Segment segment1=new X12Segment("EB*F*IND*35**DG PLUS, NON CONTRACTED*29*2000.00*****U~",separators);//Represents an individual annual max
			X12Segment segment2=new X12Segment("EB*C*IND*35**DG PLUS, NON CONTRACTED*29*50.00*****U~",separators);//Represents an individual general deductible
			EB271 eb271_1=new EB271(segment1,false,false);
			EB271 eb271_2=new EB271(segment2,false,false);
			List<EB271> listEb271s=new List<EB271>(){eb271_1,eb271_2};
			InsVerifies.CreateInsuranceAdjustmentIfNeeded(pat.PatNum,insPlan.PlanNum,insSub.InsSubNum,listBensInOd,listEb271s);
			List<ClaimProc> listInsAdjs=ClaimProcs.Refresh(pat.PatNum);
			//Only one insurance adjustment claimproc should have been made
			Assert.IsTrue(listInsAdjs.Count<=1);
			//Should have 1000 insurance used on the adjustment (benefitAmt-AmtRemaining)
			Assert.IsTrue(listInsAdjs[0].InsPayAmt==1000);
			//Should have 150 on deductible used
			Assert.IsTrue(listInsAdjs[0].DedApplied==150);
		}

		///<summary>Inserts all DB insurance rows needed for a patient with an appointment to show up on for insurance verificaiton.</summary>
		private static void CreateNewCarrierAndPatPlan(long patNum,bool isTrusted){
			TrustedEtransTypes[] arrayTrustedVals=null;
			if(isTrusted) { 
				arrayTrustedVals=new TrustedEtransTypes[] { TrustedEtransTypes.RealTimeEligibility };
			}
			Carrier carrier=CarrierT.CreateCarrier("BatchInsVerifyCarrier_"+POut.Long(patNum),arrayTrustedEtrans:arrayTrustedVals);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patNum,insPlan.PlanNum);
			PatPlanT.CreatePatPlan(1,patNum,insSub.InsSubNum);
		}

		private enum eventType {
			DateLastVerified,
			NextScheduledVerifyDate,
			DateTimeApptWasScheduled,
			AptDateTime,
			ApptSchedDaysMaxDaysForVerification,
			DateTimeInsuranceRenewalNeeded,
			DateTimeVerified
		}
	}
}
