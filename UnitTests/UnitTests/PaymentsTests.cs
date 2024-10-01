using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnitTestsCore;

namespace UnitTests.PaymentsTests {
	[TestClass]
	public class PaymentsTests:TestBase {

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
		public void Payments_InsertReturnXWebPayment_ReturnAmtSameAsOriginal() {
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Family fam=new Family(new List<Patient> {pat});
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",65,DateTime.Today);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",5,DateTime.Today);
			Payment pay1=PaymentT.MakePaymentNoSplits(pat.PatNum,70,DateTime.Today);
			PaySplitT.CreateSplit(0,pat.PatNum,pay1.PayNum,0,proc1.ProcDate,proc1.ProcNum,proc1.ProvNum,65,0);
			PaySplitT.CreateSplit(0,pat.PatNum,pay1.PayNum,0,proc2.ProcDate,proc2.ProcNum,proc2.ProvNum,5,0);
			//Next, perform the thing you're trying to test.
			Payment paymentVoid=Payments.InsertReturnXWebPayment(pay1,"Test Return",-70);
			//Then, get anything necessary from the database to see if the test is correct.
			List<PaySplit> listPaySplitsReturn=PaySplits.GetForPayment(paymentVoid.PayNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(-70,paymentVoid.PayAmt);
			Assert.AreEqual(2,listPaySplitsReturn.Count);
			Assert.AreEqual(-65,listPaySplitsReturn.FirstOrDefault(x => x.ProcNum==proc1.ProcNum).SplitAmt);
			Assert.AreEqual(-5,listPaySplitsReturn.FirstOrDefault(x => x.ProcNum==proc2.ProcNum).SplitAmt);
			Assert.AreEqual(-70,listPaySplitsReturn.Select(x => x.SplitAmt).Sum());
		}

		[TestMethod]
		public void Payments_InsertReturnXWebPayment_ReturnAmtLessThanOriginal_Partial() {
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Family fam=new Family(new List<Patient> {pat});
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",65,DateTime.Today);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",5,DateTime.Today);
			Payment pay1=PaymentT.MakePaymentNoSplits(pat.PatNum,70,DateTime.Today);
			PaySplitT.CreateSplit(0,pat.PatNum,pay1.PayNum,0,proc1.ProcDate,proc1.ProcNum,proc1.ProvNum,65,0);
			PaySplitT.CreateSplit(0,pat.PatNum,pay1.PayNum,0,proc2.ProcDate,proc2.ProcNum,proc2.ProvNum,5,0);
			//Next, perform the thing you're trying to test.
			Payment paymentVoid=Payments.InsertReturnXWebPayment(pay1,"Test Return",-50);//partial return
			//Then, get anything necessary from the database to see if the test is correct.
			List<PaySplit> listPaySplitsReturn=PaySplits.GetForPayment(paymentVoid.PayNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(-50,paymentVoid.PayAmt);
			Assert.AreEqual(-50,listPaySplitsReturn.Select(x => x.SplitAmt).Sum());
		}
	}
}
