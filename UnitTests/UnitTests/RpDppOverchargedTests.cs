using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestsCore;

namespace UnitTests.UnitTests {
	[TestClass]
	public class RpDppOverchargedTests:TestBase {

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
		public void RpDppOverchargedTests_GetDPPOvercharged_NoOverCharged() {
			//Setup
			long provNum=ProviderT.CreateProvider("LS");
			Patient pat=PatientT.CreatePatient(fName:"Austin",lName:"Patient",priProvNum:provNum);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",64,DateTime.Today.AddMonths(-1),provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.C,"",58,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Make a dynamic payment plan where over half of the production is due right now.
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,35,0,35,
				new List<Procedure>{proc1,proc2},new List<Adjustment>{ },PayPlanFrequency.Monthly);
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			Assert.AreEqual(3,listPayPlanCharges.Count);
			PayPlanCharge proc2PayPlanCharge=listPayPlanCharges.FirstOrDefault(x => x.FKey==proc2.ProcNum);
			PayPlanCharge proc1PayPlanCharge=listPayPlanCharges.FirstOrDefault(x => x.FKey==proc1.ProcNum);
			//Patient pays full amount that is due
			PaymentT.MakePayment(pat.PatNum,64,payDate: DateTime.Now,procNum:proc1.ProcNum,payPlanNum: dynamicPayPlan.PayPlanNum,
				payPlanChargeNum: proc1PayPlanCharge.PayPlanChargeNum);
			PaymentT.MakePayment(pat.PatNum,58,payDate:DateTime.Now,procNum:proc2.ProcNum,payPlanNum:dynamicPayPlan.PayPlanNum,
				payPlanChargeNum:proc2PayPlanCharge.PayPlanChargeNum);
			//Payment plan should not be overpaid.
			DataTable table=RpDPPOvercharged.GetDPPOvercharged(DateTime.Today,DateTime.Today,
				new List<long>() { pat.ClinicNum },new List<long>() { pat.PriProv },pat.PatNum);
			Assert.AreEqual(0,table.Rows.Count);
		}

		[TestMethod]
		public void RpDppOverchargedTests_GetDPPOvercharged_Overcharged_ProcOverrideWithAdjustment() {
			//Setup
			long provNum=ProviderT.CreateProvider("LS");
			Patient pat=PatientT.CreatePatient(fName:"Austin",lName:"Patient",priProvNum:provNum);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",64,DateTime.Today.AddMonths(-1),provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.C,"",58,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Make a dynamic payment plan where over half of the production is due right now.
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,35,1,35,
				new List<Procedure>{proc1,proc2},new List<Adjustment>{ },PayPlanFrequency.Monthly);
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			Assert.AreEqual(3,listPayPlanCharges.Count);
			PayPlanCharge proc2PayPlanCharge=listPayPlanCharges.FirstOrDefault(x => x.FKey==proc2.ProcNum);
			PayPlanCharge proc1PayPlanCharge=listPayPlanCharges.FirstOrDefault(x => x.FKey==proc1.ProcNum);
			//Patient pays full amount that is due
			PaymentT.MakePayment(pat.PatNum,64,payDate: DateTime.Now,procNum:proc1.ProcNum,payPlanNum: dynamicPayPlan.PayPlanNum,
				payPlanChargeNum: proc1PayPlanCharge.PayPlanChargeNum);
			PaymentT.MakePayment(pat.PatNum,58,payDate:DateTime.Now,procNum:proc2.ProcNum,payPlanNum:dynamicPayPlan.PayPlanNum,
				payPlanChargeNum:proc2PayPlanCharge.PayPlanChargeNum);
			AdjustmentT.MakeAdjustment(pat.PatNum,-64,procNum:proc1.ProcNum);
			//Payment plan should be overcharged.
			DataTable table=RpDPPOvercharged.GetDPPOvercharged(DateTime.Today,DateTime.Today,
				new List<long>() { pat.ClinicNum },new List<long>() { pat.PriProv },pat.PatNum);
			Assert.AreEqual(1,table.Rows.Count);
		}
	}
}
