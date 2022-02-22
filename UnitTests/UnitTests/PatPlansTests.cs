using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.PatPlans_Tests {
	[TestClass]
	public class PatPlansTests:TestBase {

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
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void PatPlans_GetOrthoNextClaimDate_CorrectNumberOfClaimsCreatedForMonthlyFrequency() {
			OrthoAutoProcFrequency frequency=OrthoAutoProcFrequency.Monthly;
			int monthsTreatment=3;
			DateTime dateFirstOrthoProc1=new DateTime(2019,12,31);
			DateTime dateTreatmentEnd1=dateFirstOrthoProc1.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates1=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc1,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc2=new DateTime(2020,1,1);
			DateTime dateTreatmentEnd2=dateFirstOrthoProc2.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates2=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc2,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc3=new DateTime(2020,1,2);
			DateTime dateTreatmentEnd3=dateFirstOrthoProc3.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates3=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc3,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc4=new DateTime(2020,1,15);
			DateTime dateTreatmentEnd4=dateFirstOrthoProc4.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates4=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc4,frequency,monthsTreatment);
			Assert.IsTrue(dateTreatmentEnd1 >= listClaimDates1.Last());
			Assert.AreEqual(3,listClaimDates1.Count);
			Assert.IsTrue(dateTreatmentEnd2 >= listClaimDates2.Last());
			Assert.AreEqual(3,listClaimDates2.Count);
			Assert.IsTrue(dateTreatmentEnd3 >= listClaimDates3.Last());
			Assert.AreEqual(3,listClaimDates3.Count);
			Assert.IsTrue(dateTreatmentEnd4 >= listClaimDates4.Last());
			Assert.AreEqual(3,listClaimDates4.Count);
		}

		[TestMethod]
		public void PatPlans_GetOrthoNextClaimDate_CorrectNumberOfClaimsCreatedForQuarterlyFrequency() {
			OrthoAutoProcFrequency frequency=OrthoAutoProcFrequency.Quarterly;
			int monthsTreatment=9;
			DateTime dateFirstOrthoProc1=new DateTime(2019,12,31);
			DateTime dateTreatmentEnd1=dateFirstOrthoProc1.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates1=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc1,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc2=new DateTime(2020,1,1);
			DateTime dateTreatmentEnd2=dateFirstOrthoProc2.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates2=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc2,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc3=new DateTime(2020,1,2);
			DateTime dateTreatmentEnd3=dateFirstOrthoProc3.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates3=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc3,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc4=new DateTime(2020,1,15);
			DateTime dateTreatmentEnd4=dateFirstOrthoProc4.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates4=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc4,frequency,monthsTreatment);
			Assert.IsTrue(dateTreatmentEnd1 >= listClaimDates1.Last());
			Assert.AreEqual(3,listClaimDates1.Count);
			Assert.IsTrue(dateTreatmentEnd2 >= listClaimDates2.Last());
			Assert.AreEqual(3,listClaimDates2.Count);
			Assert.IsTrue(dateTreatmentEnd3 >= listClaimDates3.Last());
			Assert.AreEqual(3,listClaimDates3.Count);
			Assert.IsTrue(dateTreatmentEnd4 >= listClaimDates4.Last());
			Assert.AreEqual(3,listClaimDates4.Count);
		}

		[TestMethod]
		public void PatPlans_GetOrthoNextClaimDate_CorrectNumberOfClaimsCreatedForSemiAnnualFrequency() {
			OrthoAutoProcFrequency frequency=OrthoAutoProcFrequency.SemiAnnual;
			int monthsTreatment=18;
			DateTime dateFirstOrthoProc1=new DateTime(2019,12,31);
			DateTime dateTreatmentEnd1=dateFirstOrthoProc1.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates1=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc1,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc2=new DateTime(2020,1,1);
			DateTime dateTreatmentEnd2=dateFirstOrthoProc2.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates2=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc2,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc3=new DateTime(2020,1,2);
			DateTime dateTreatmentEnd3=dateFirstOrthoProc3.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates3=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc3,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc4=new DateTime(2020,1,15);
			DateTime dateTreatmentEnd4=dateFirstOrthoProc4.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates4=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc4,frequency,monthsTreatment);
			Assert.IsTrue(dateTreatmentEnd1 >= listClaimDates1.Last());
			Assert.AreEqual(3,listClaimDates1.Count);
			Assert.IsTrue(dateTreatmentEnd2 >= listClaimDates2.Last());
			Assert.AreEqual(3,listClaimDates2.Count);
			Assert.IsTrue(dateTreatmentEnd3 >= listClaimDates3.Last());
			Assert.AreEqual(3,listClaimDates3.Count);
			Assert.IsTrue(dateTreatmentEnd4 >= listClaimDates4.Last());
			Assert.AreEqual(3,listClaimDates4.Count);
		}

		[TestMethod]
		public void PatPlans_GetOrthoNextClaimDate_CorrectNumberOfClaimsCreatedForAnnualFrequency() {
			OrthoAutoProcFrequency frequency=OrthoAutoProcFrequency.Annual;
			int monthsTreatment=36;
			DateTime dateFirstOrthoProc1=new DateTime(2019,12,31);
			DateTime dateTreatmentEnd1=dateFirstOrthoProc1.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates1=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc1,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc2=new DateTime(2020,1,1);
			DateTime dateTreatmentEnd2=dateFirstOrthoProc2.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates2=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc2,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc3=new DateTime(2020,1,2);
			DateTime dateTreatmentEnd3=dateFirstOrthoProc3.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates3=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc3,frequency,monthsTreatment);
			DateTime dateFirstOrthoProc4=new DateTime(2020,1,15);
			DateTime dateTreatmentEnd4=dateFirstOrthoProc4.AddMonths(monthsTreatment);
			List<DateTime> listClaimDates4=PatPlanT.GenerateAutoOrthoClaimDatesHelper(dateFirstOrthoProc4,frequency,monthsTreatment);
			Assert.IsTrue(dateTreatmentEnd1 >= listClaimDates1.Last());
			Assert.AreEqual(3,listClaimDates1.Count);
			Assert.IsTrue(dateTreatmentEnd2 >= listClaimDates2.Last());
			Assert.AreEqual(3,listClaimDates2.Count);
			Assert.IsTrue(dateTreatmentEnd3 >= listClaimDates3.Last());
			Assert.AreEqual(3,listClaimDates3.Count);
			Assert.IsTrue(dateTreatmentEnd4 >= listClaimDates4.Last());
			Assert.AreEqual(3,listClaimDates4.Count);
		}


	}
}
