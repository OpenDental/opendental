using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.PaySplits_Tests {
	[TestClass]
	public class PaySplitsTests:TestBase {

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
		public void PaySplits_GetUnearnedForFam_ManuallyAllocatingUnearnedForFamily() {
			//Test scenario. A user has manually allocated unearned but did not attach the final linking split to the procedure pay split. 
			//Orginal prepayment is linked on the negative allocating split, but the postive allocating split does not have the negative split attached.
			//We need to check to make sure the unearned is calculating correctly, previously is was double counting some things and not getting correct 
			//unearned amount. 
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Family fam=new Family(new List<Patient> {pat});
			Def unearned=DefT.CreateDefinition(DefCat.PaySplitUnearnedType,suffix);
			//Create 2 separate prepayments (bug would only show when more than 1 was present)
			PaySplit prepay1=PaySplitT.CreatePrepayment(pat.PatNum,65,DateTime.Today.AddMonths(-3));
			PaySplit prepay2=PaySplitT.CreatePrepayment(pat.PatNum,5,DateTime.Today.AddMonths(-3));
			//make the procedures for the prepayments
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",65,DateTime.Today);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",5,DateTime.Today);
			//Manually allocate the prepayments to the procedures.
			Payment pay1=PaymentT.MakePaymentNoSplits(pat.PatNum,0,DateTime.Today);
			PaySplitT.CreateSplit(0,pat.PatNum,pay1.PayNum,0,prepay1.SecDateTEdit,0,0,-65,unearned.DefNum,prepay1.SplitNum);
			PaySplitT.CreateSplit(0,pat.PatNum,pay1.PayNum,0,proc1.ProcDate,proc1.ProcNum,proc1.ProvNum,65,0);
			Payment pay2=PaymentT.MakePaymentNoSplits(pat.PatNum,0,DateTime.Today);
			PaySplitT.CreateSplit(0,pat.PatNum,pay2.PayNum,0,prepay2.SecDateTEdit,0,0,-5,unearned.DefNum,prepay2.SplitNum);
			PaySplitT.CreateSplit(0,pat.PatNum,pay2.PayNum,0,proc2.ProcDate,proc2.ProcNum,proc2.ProvNum,5,0);
			decimal unearnedAmt=PaySplits.GetTotalAmountOfUnearnedForPats(fam.GetPatNums());
			Assert.AreEqual(0,unearnedAmt);
		}

		[TestMethod]
		public void PaySplits_GetSplitsForPrepay_TransferTpPreallocationsToAllocatedMoneyOnceCompleted() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov");
			Procedure treatPlanProc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,provNum:provNum);
			long hiddenUnearnedType=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType).FirstOrDefault(x => x.ItemValue!="").DefNum;
			Payment paidTpPreAllocation=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today.AddDays(-1),0,provNum,treatPlanProc.ProcNum,1,0,
				hiddenUnearnedType);
			ProcedureT.SetComplete(treatPlanProc,pat,new InsuranceInfo());
			//setting procedure complete should have made a transfer taking the unearned on the proc, and making it allocated.
			List<PaySplit> listSplitsOnProc=PaySplits.GetPaySplitsFromProc(treatPlanProc.ProcNum);
			Assert.AreEqual(1,listSplitsOnProc.Count);//In the end only one split ends up being attached to the procedure.
			//check to make sure the original prepayment got the procedure disassociated from it. 
			Assert.AreEqual(0,PaySplits.GetForPayment(paidTpPreAllocation.PayNum).First().ProcNum);
			Assert.AreEqual(50,listSplitsOnProc.Sum(x => x.SplitAmt));
		}

		[TestMethod]
		public void PaySplits_GetPaySplitsFromProc_TransferTpPreAllocationToBrokenProcedure() {
			PrefT.UpdateBool(PrefName.TpPrePayIsNonRefundable,true);
			PrefT.UpdateInt(PrefName.PrePayAllowedForTpProcs,(int)YN.Yes);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov");
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,1,provNum);
			Procedure treatPlanProc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,provNum:provNum,aptNum:appt.AptNum);
			long hiddenUnearnedType=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType).FirstOrDefault(x => x.ItemValue!="").DefNum;
			Payment paidTpPreAllocation=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today.AddDays(-1),0,provNum,treatPlanProc.ProcNum,1,0,
				hiddenUnearnedType);
			//D9986 == missed appointment
			ProcedureCode procCode=new ProcedureCode{CodeNum=treatPlanProc.CodeNum,ProcCode="D9986",NoBillIns=true,ProvNumDefault=provNum};
			Procedure brokenProcedure=AppointmentT.BreakAppointment(appt,pat,procCode,50);
			List<PaySplit> listSplitsForBrokenProc=PaySplits.GetPaySplitsFromProc(brokenProcedure.ProcNum);
			Assert.AreEqual(50,listSplitsForBrokenProc.Sum(x => x.SplitAmt));
			Assert.AreEqual(0,PaySplits.GetPaySplitsFromProc(treatPlanProc.ProcNum).Sum(x => x.SplitAmt));
		}

		[TestMethod]
		public void PaySplits_GetPaySplitsFromProc_NoRefundTransfersKeepOriginalPrePaymentAmount() {
			//bug for this was when a customer was transferring a no-refund tp pre pay to a broken procedure and only transferred some of the money.
			//if a user was on don't enforce specifically, they would make the transfer and the original split would be modified with no other split created.
			PrefT.UpdateInt(PrefName.PrePayAllowedForTpProcs,(int)YN.Yes);
			PrefT.UpdateBool(PrefName.TpPrePayIsNonRefundable,true);
			long hiddenUnearnedType=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType).FirstOrDefault(x => x.ItemValue!="").DefNum;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long provNum=ProviderT.CreateProvider("prov");
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddDays(1),1,provNum);
			Procedure treatPlanProc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,provNum:provNum,aptNum:appt.AptNum);
			Payment pay=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today,provNum:provNum,procNum:treatPlanProc.ProcNum,unearnedType:hiddenUnearnedType);
			//D9986 == missed appointment
			ProcedureCode procCode=new ProcedureCode{CodeNum=treatPlanProc.CodeNum,ProcCode="D9986",NoBillIns=true,ProvNumDefault=provNum};
			Procedure brokenProcedure=AppointmentT.BreakAppointment(appt,pat,procCode,30);
			List<PaySplit> listSplitsForBrokenProc=PaySplits.GetPaySplitsFromProc(brokenProcedure.ProcNum);
			Assert.AreEqual(0,PaySplits.GetPaySplitsFromProc(treatPlanProc.ProcNum).Sum(x => x.SplitAmt));//proc isn't complete. Should have no money.
			Assert.AreEqual(30,listSplitsForBrokenProc.Sum(x => x.SplitAmt));
			List<PaySplit> listSplitsForPayment=PaySplits.GetForPayment(pay.PayNum);
			Assert.AreEqual(50,listSplitsForPayment.Sum(x => x.SplitAmt));//payment should be worth it's original value.
		}

	}
}
