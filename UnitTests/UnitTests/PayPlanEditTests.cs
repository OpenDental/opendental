using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.PayPlanEdit_Tests {
	[TestClass]
	public class PayPlanEditTests:TestBase {

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
		public void PayPlanEdit_CreatePayPlanAdjustments_PayPlanWithNegAdjustmentsForPlansWithUnattachedCredits() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",135,DateTime.Today.AddMonths(-3));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),provNum:0,totalAmt:195);//totalAmt since unattached credits
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			double totalFutureNegAdjs=PayPlanT.GetTotalNegFutureAdjs(listCharges);
			List<PayPlanCharge> listChargesAndCredits=PayPlanEdit.CreatePayPlanAdjustments(-45,listCharges,totalFutureNegAdjs);
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal < 0).Count);//2 negative debits
			//Assert that the method only created adjustments for furthest out dates.
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit 
				&& x.ChargeDate.Month==DateTime.Today.AddMonths(2).Month).Count);//1 positive and 1 negative debit
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit 
				&& x.ChargeDate.Month==DateTime.Today.AddMonths(3).Month).Count);
		}

		[TestMethod]
		public void PayPlanEdit_CreatePayPlanAdjustments_PayPlanWithNegAdjustmentForPlansWithAttachedCredits() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",135,DateTime.Today.AddMonths(-3));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",60,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,new List<Procedure> {proc1,proc2 });
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			double totalFutureNegAdjs=PayPlanT.GetTotalNegFutureAdjs(listCharges);
			List<PayPlanCharge> listChargesAndCredits=PayPlanEdit.CreatePayPlanAdjustments(-45,listCharges,totalFutureNegAdjs);
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit && x.Principal < 0).Count);//2 negative debits
			//Assert that the method only created adjustments for furthest out dates.
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit 
				&& x.ChargeDate.Month==DateTime.Today.AddMonths(2).Month).Count);//1 positive and 1 negative debit
			Assert.AreEqual(2,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit 
				&& x.ChargeDate.Month==DateTime.Today.AddMonths(3).Month).Count);
		}

		[TestMethod]
		public void PayPlanEdit_CloseOutPatPayPlan_CloseOutPatPayPlanWithAdjustments() {
			DateTime today=new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day,0,0,0,DateTimeKind.Unspecified);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1120",ProcStat.C,"",100,DateTime.Today.AddMonths(-3));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",92,DateTime.Today.AddMonths(-3));
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,new List<Procedure> {proc1,proc2 });
			Payment payment=PaymentT.MakePayment(pat.PatNum,30,DateTime.Today.AddMonths(-2),payplan.PayPlanNum);//make a payment for the plan
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			double totalFutureNegAdjs=PayPlanT.GetTotalNegFutureAdjs(listCharges);
			List<PayPlanCharge> listChargesAndCredits=PayPlanEdit.CreatePayPlanAdjustments(-62,listCharges,totalFutureNegAdjs);//make adjustments for the plan.
			listChargesAndCredits.Add(PayPlanChargeT.CreateNegativeCreditForAdj(pat.PatNum,payplan.PayPlanNum,-62));//add the tx credit for the adjustment
			//Balance should equal 100. $192 of completed tx - $30 payment + $-62 adjustment. 
			PayPlanCharge closeOutCharge=PayPlanEdit.CloseOutPatPayPlan(listChargesAndCredits,payplan,today);
			//List<PayPlanCharge> listFinalCharges=listChargesAndCredits.RemoveAll(x => x.ChargeDate > today.Date);
			listChargesAndCredits.RemoveAll(x => x.ChargeDate > DateTime.Today);
			listChargesAndCredits.Add(closeOutCharge);
			double debitsDue=120;//4 pay plan charges of $30 each that have come due
			double creditsOutstanding=130;//Original $192 - $62 adjustment
			Assert.AreEqual(creditsOutstanding - debitsDue,closeOutCharge.Principal);//close out charge should equal 10 (remaining debits - credits)
			//total balance should equal 100 (-30 to take out the payment that was made). 
			Assert.AreEqual(100,listChargesAndCredits.FindAll(x => x.ChargeType==PayPlanChargeType.Debit).Sum(x => x.Principal)-30); 
		}

		[TestMethod]
		public void PayPlanEdit_CreateScheduleCharges_TotalInterestIsCorrectWhenDelayingInterest() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=new Family(new List<Patient>{pat});
			long provNum=ProviderT.CreateProvider("LS");
			PayPlan payPlan=PayPlanT.CreatePayPlanNoCharges(pat.PatNum,110,DateTime.Today,pat.PatNum);
			payPlan.DownPayment=10;
			payPlan.APR=12;
			payPlan.DateInterestStart=DateTime.Today.AddMonths(2);
			payPlan.ChargeFrequency=PayPlanFrequency.Monthly;
			payPlan.PaySchedule=PaymentSchedule.Monthly;
			payPlan.PayAmt=25;
			payPlan.DatePayPlanStart=DateTime.Today.AddMonths(1);
			PayPlans.Update(payPlan);
			PayPlanTerms terms=PayPlanT.GetTerms(payPlan,principalAmt:110);
			List<PayPlanCharge> listPayPlanCharges=new List<PayPlanCharge>();
			PayPlanEdit.CreateScheduleCharges(terms,payPlan,fam,provNum,0,listPayPlanCharges);
			Assert.AreEqual(1.54,listPayPlanCharges.Sum(x => x.Interest));
		}

		[TestMethod]
		public void PayPlanEdit_RecalculateScheduleCharges_TotalInterestIsCorrectWhenDelayingInterest() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=new Family(new List<Patient>{pat});
			long provNum=ProviderT.CreateProvider("LS");
			PayPlan payPlan=PayPlanT.CreatePayPlanNoCharges(pat.PatNum,110,DateTime.Today,pat.PatNum);
			payPlan.DownPayment=10;
			payPlan.APR=12;
			payPlan.DateInterestStart=DateTime.Today.AddMonths(2);
			payPlan.ChargeFrequency=PayPlanFrequency.Monthly;
			payPlan.PaySchedule=PaymentSchedule.Monthly;
			payPlan.PayAmt=25;
			payPlan.DatePayPlanStart=DateTime.Today.AddMonths(1);
			PayPlans.Update(payPlan);
			PayPlanTerms terms=PayPlanT.GetTerms(payPlan,principalAmt:110);
			List<PayPlanCharge> listPayPlanCharges=new List<PayPlanCharge>();
			PayPlanEdit.CreateScheduleCharges(terms,payPlan,fam,provNum,0,listPayPlanCharges);
			terms.PrincipalAmount=110;//Needs to be reset as CreateScheduleCharges deducts down payment.
			Payment payment=PaymentT.MakePayment(pat.PatNum,30,DateTime.Today,payPlan.PayPlanNum);
			List<PaySplit> listPaySplits=PaySplits.GetForPayPlans(new List<long>(){payPlan.PayPlanNum});
			//Prepaying and recalculating interest
			List<PayPlanCharge> listPayPlanChargesCopy=new List<PayPlanCharge>(listPayPlanCharges);
			PayPlanEdit.PayPlanRecalculationData recalcData=
				PayPlanEdit.PayPlanRecalculationData.CreateRecalculationData(terms,payPlan,fam,provNum,0,listPayPlanChargesCopy,listPaySplits,true,true);
			PayPlanEdit.RecalculateScheduleCharges(terms,recalcData);
			Assert.AreEqual((decimal)1.54,(decimal)listPayPlanChargesCopy.Sum(x => x.Interest));
			//Paying on principal and recalculating interest
			listPayPlanChargesCopy=new List<PayPlanCharge>(listPayPlanCharges);
			recalcData=PayPlanEdit.PayPlanRecalculationData.CreateRecalculationData(terms,payPlan,fam,provNum,0,listPayPlanChargesCopy,listPaySplits,false,true);
			PayPlanEdit.RecalculateScheduleCharges(terms,recalcData);
			Assert.AreEqual((decimal)0.92,(decimal)listPayPlanChargesCopy.Sum(x => x.Interest));
			//Prepaying and NOT recalculating interest
			listPayPlanChargesCopy=new List<PayPlanCharge>(listPayPlanCharges);
			recalcData=PayPlanEdit.PayPlanRecalculationData.CreateRecalculationData(terms,payPlan,fam,provNum,0,listPayPlanChargesCopy,listPaySplits,true,false);
			PayPlanEdit.RecalculateScheduleCharges(terms,recalcData);
			Assert.AreEqual((decimal)1.54,(decimal)listPayPlanChargesCopy.Sum(x => x.Interest));
			//Paying on principal and NOT recalculating interest
			listPayPlanChargesCopy=new List<PayPlanCharge>(listPayPlanCharges);
			recalcData=PayPlanEdit.PayPlanRecalculationData.CreateRecalculationData(terms,payPlan,fam,provNum,0,listPayPlanChargesCopy,listPaySplits,false,false);
			PayPlanEdit.RecalculateScheduleCharges(terms,recalcData);
			Assert.AreEqual((decimal)1.54,(decimal)listPayPlanChargesCopy.Sum(x => x.Interest));
		}

		[TestMethod]
		public void PayPlanEdit_GetListExpectedCharges_TotalInterestIsCorrectWhenDelayingInterest() {
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",50,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0221",ProcStat.C,"",45,DateTime.Today));
			listAdjs.Add(AdjustmentT.MakeAdjustment(pat.PatNum,10));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),5,12,25,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2));
			//run logic to generate charges
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			//assert expected results
			Assert.AreEqual((decimal)1.54,(decimal)listCharges.Sum(x => x.Interest));
		}

		[TestMethod]
		public void PayPlanEdit_GetListExpectedCharges_ChargesAreGeneratedCorrectly() {
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",165,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0221",ProcStat.C,"",25,DateTime.Today));
			listAdjs.Add(AdjustmentT.MakeAdjustment(pat.PatNum,10));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,5,0,22,listProcs,listAdjs);
			//run logic to generate charges (look at recurring charges tests for reference
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listChargesThisPeriod=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,true);
			//assert expected results
			Assert.AreEqual(22,listChargesThisPeriod.Sum(x => x.Principal));
		}

		[TestMethod]
		public void PayPlanEdit_GetListExpectedCharges_CorrectInterestCalculatedAfterDirectOverpayment() {
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",400,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0221",ProcStat.C,"",400,DateTime.Today));
			listAdjs.Add(AdjustmentT.MakeAdjustment(pat.PatNum,200));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,250,listProcs,listAdjs
				,PayPlanFrequency.Monthly);
			//run logic to generate charges (look at recurring charges tests for reference)
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			Assert.AreEqual(1,listChargesDb.Count);
			Assert.AreEqual(1,listChargesDb.Count(x => x.Principal==240 && x.Interest==10));
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			//assert expected results
			Dictionary<DateTime,List<PayPlanCharge>> dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate)
				.ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(4,dictPeriodDateCharges.Count);
			Assert.AreEqual(242.40,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Principal));
			Assert.AreEqual(7.60,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Interest));
			Assert.AreEqual(244.82,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal));
			Assert.AreEqual(5.18,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			Assert.AreEqual(247.27,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal));
			Assert.AreEqual(2.73,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			Assert.AreEqual(25.51,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal));
			Assert.AreEqual(0.26,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
			//Make a payment to the payment plan for $1000.
			Payment payment=PaymentT.MakePayment(pat.PatNum,1000,DateTime.Today,dynamicPayPlan.PayPlanNum);
			//Make sure that amortization schedule for expected charges is correct after overpayment.
			listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate).ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(4,dictPeriodDateCharges.Count);
			Assert.AreEqual(249.90,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Principal));
			Assert.AreEqual(0.10,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Interest));
			Assert.AreEqual(249.90,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal));
			Assert.AreEqual(0.10,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			Assert.AreEqual((decimal)249.90,(decimal)dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal));
			Assert.AreEqual(0.10,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			Assert.AreEqual(10.30,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal));
			Assert.AreEqual(0.10,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
			//Make sure that next charge gets posted correctly.
			List<PayPlanCharge> listChargesForSecondPeriod=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,true);
			Assert.AreEqual(249.90,listChargesForSecondPeriod.Sum(x => x.Principal));
			Assert.AreEqual(0.10,listChargesForSecondPeriod.Sum(x => x.Interest));
			//Make sure the amortization schedule after the second charge is correct.
			listChargesDb.AddRange(listChargesForSecondPeriod);
			listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate).ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(3,dictPeriodDateCharges.Count);
			Assert.AreEqual(249.90,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal));
			Assert.AreEqual(0.10,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			Assert.AreEqual((decimal)249.90,(decimal)dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal));
			Assert.AreEqual(0.10,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			Assert.AreEqual(10.30,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal));
			Assert.AreEqual(0.10,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
		}

		[TestMethod]
		public void PayPlanEdit_GetListExpectedCharges_CorrectInterestCalculatedAfterUnderpayment() {
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",400,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0221",ProcStat.C,"",400,DateTime.Today));
			listAdjs.Add(AdjustmentT.MakeAdjustment(pat.PatNum,200));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,250,listProcs,listAdjs
				,PayPlanFrequency.Monthly);
			//run logic to generate charges (look at recurring charges tests for reference)
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			Assert.AreEqual(1,listChargesDb.Count);
			Assert.AreEqual(1,listChargesDb.Count(x => x.Principal==240 && x.Interest==10));
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			//assert expected results
			Dictionary<DateTime,List<PayPlanCharge>> dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate)
				.ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(4,dictPeriodDateCharges.Count);
			Assert.AreEqual(242.40,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Principal));
			Assert.AreEqual(7.60,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Interest));
			Assert.AreEqual(244.82,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal));
			Assert.AreEqual(5.18,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			Assert.AreEqual(247.27,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal));
			Assert.AreEqual(2.73,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			Assert.AreEqual(25.51,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal));
			Assert.AreEqual(0.26,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
			//Make a payment to the payment plan for $100.
			//This should pay off the $100 principal and no interest because principal is paid first.
			Payment payment=PaymentT.MakePayment(pat.PatNum,100,DateTime.Today,dynamicPayPlan.PayPlanNum);
			//Make sure that amortization schedule for expected charges is correct after overpayment.
			listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate).ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(4,dictPeriodDateCharges.Count);
			//These will be the same as before, because we calculate expected interest with the assumption that the patient will pay the balance due.
			Assert.AreEqual(242.40,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Principal));
			Assert.AreEqual(7.60,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Interest));
			Assert.AreEqual(244.82,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal));
			Assert.AreEqual(5.18,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			Assert.AreEqual(247.27,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal));
			Assert.AreEqual(2.73,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			Assert.AreEqual(25.51,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal));
			Assert.AreEqual(0.26,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
			//Make sure that next charge gets posted correctly.
			List<PayPlanCharge> listChargesForSecondPeriod=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,true);
			Assert.AreEqual(240.90,listChargesForSecondPeriod.Sum(x => x.Principal));
			Assert.AreEqual(9.10,listChargesForSecondPeriod.Sum(x => x.Interest));
			//Make sure the amortization schedule after the second charge is correct.
			listChargesDb.AddRange(listChargesForSecondPeriod);
			listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate).ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(3,dictPeriodDateCharges.Count);
			Assert.AreEqual(244.81,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal));
			Assert.AreEqual(5.19,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			Assert.AreEqual(247.26,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal));
			Assert.AreEqual(2.74,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			Assert.AreEqual(27.03,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal));
			Assert.AreEqual(0.27,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
		}

		[TestMethod]
		public void PayPlanEdit_GetListExpectedCharges_EnsureThatInterestIsNotChargedOnUnpaidInterest() {
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",400,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0221",ProcStat.C,"",400,DateTime.Today));
			listAdjs.Add(AdjustmentT.MakeAdjustment(pat.PatNum,200));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,250,listProcs,listAdjs
				,PayPlanFrequency.Monthly);
			//run logic to generate charges (look at recurring charges tests for reference)
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listFirstCharge=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,true);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listFirstCharge,terms,fam,listEntries,dynamicPayPlan,true);
			//assert expected results
			Assert.AreEqual((decimal)listFirstCharge[0].Interest,(decimal)listExpectedCharges[0].Interest);
		}

		[TestMethod]
		public void PayPlanEdit_GetListExpectedCharges_PaymentsApplyToInterestAndPrincipal() {
			//-----------------------------------------------------------------------------------------------------------------------
			//1. Complete a procedure with a fee of $100 and create a dynamic payment plan for it.
			//2. Set the plans APR to 12, number of payments to 5, and date of first payment to today.
			//3. When the amortization schedule is created, the interest for the 2nd pay period will correctly be $0.80
			//4. Make a payment for the $20.61 that is due on the plan.
			//5. The interest for the 2nd pay period would be $0.79 and the total interest for the plan would have reduced from $3.02 to $2.98.
			//-----------------------------------------------------------------------------------------------------------------------
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			//set up payment plan
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>() {
				ProcedureT.CreateProcedure(pat,"PATIAP",ProcStat.C,"",100,DateTime.Today,provNum:provNum),
			};
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,0,listProcs,new List<Adjustment>(),
				frequency:PayPlanFrequency.Monthly,provNum:provNum,payCount:5);
			//run logic to generate charges (look at recurring charges tests for reference)
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			Assert.AreEqual(1,listChargesDb.Count);
			Assert.AreEqual(1,listChargesDb.Count(x => x.Principal==19.61 && x.Interest==1));
			List<PayPlanCharge> listNoPaymentCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			Assert.AreEqual(4,listNoPaymentCharges.Count);
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==19.81 && x.Interest==0.80));
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.00 && x.Interest==0.61));
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.20 && x.Interest==0.41));
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.38 && x.Interest==0.20));
			//Mimic the user making a payment for the amount due right now (first month of principal and interest).
			Payment payCur=PaymentT.MakePaymentNoSplits(pat.PatNum,20.61,DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payCur,new List<AccountEntry>(),false,false,null);
			results.ListAutoSplits.ForEach(x => PaySplits.Insert(x));
			//Now that a payment has been made, the interest should not have changed.
			//It was before when applying all payment plan payments to principal first (interest last) like the patient payment plans do (wrong).
			listNoPaymentCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			Assert.AreEqual(4,listNoPaymentCharges.Count);
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==19.81 && x.Interest==0.80));
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.00 && x.Interest==0.61));
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.20 && x.Interest==0.41));
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.38 && x.Interest==0.20));
			List<PayPlanCharge> listChargesForSecondPeriod=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,true);
			Assert.AreEqual(1,listChargesForSecondPeriod.Count(x => x.Principal==19.81 && x.Interest==0.80));
			listChargesDb.AddRange(listChargesForSecondPeriod);
			listNoPaymentCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			Assert.AreEqual(3,listNoPaymentCharges.Count);
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.00 && x.Interest==0.61));
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.20 && x.Interest==0.41));
			Assert.AreEqual(1,listNoPaymentCharges.Count(x => x.Principal==20.38 && x.Interest==0.20));
		}

		[TestMethod]
		public void PayPlanEdit_GetListExpectedCharges_OverPaymentsApplyToPrincipalAndInterestCorrectly() {
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			//set up payment plan
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",400,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0221",ProcStat.C,"",400,DateTime.Today));
			listAdjs.Add(AdjustmentT.MakeAdjustment(pat.PatNum,200));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,250,listProcs,listAdjs
				,PayPlanFrequency.Monthly);
			//run logic to generate charges (look at recurring charges tests for reference)
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			Assert.AreEqual(1,listChargesDb.Count);
			Assert.AreEqual(1,listChargesDb.Count(x => x.Principal==240 && x.Interest==10));
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			//assert expected results
			Dictionary<DateTime,List<PayPlanCharge>> dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate)
				.ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(4,dictPeriodDateCharges.Count);
			Assert.AreEqual(242.40,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Principal));
			Assert.AreEqual(7.60,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Interest));
			Assert.AreEqual(244.82,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal));
			Assert.AreEqual(5.18,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			Assert.AreEqual(247.27,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal));
			Assert.AreEqual(2.73,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			Assert.AreEqual(25.51,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal));
			Assert.AreEqual(0.26,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
			//Over pay the first PayPlanCharge credit that has come due by a large amount.
			//A PaySplit of $10 will go towards the interest that is due, thus leaving some principal left over.
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,750,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payment,new List<AccountEntry>(),false,false,null);
			//The auto-split system should suggest three PaySplits; $240, $10, and $500 (unearned).
			Assert.AreEqual(3,results.ListAutoSplits.Count);
			Assert.AreEqual(1,results.ListAutoSplits.Count(x => x.SplitAmt==240));
			Assert.AreEqual(1,results.ListAutoSplits.Count(x => x.SplitAmt==10));
			Assert.AreEqual(1,results.ListAutoSplits.Count(x => x.SplitAmt==500 && x.UnearnedType > 0));
			//Act like the user manually moved the money from unearned (delete that split) and inflated the principal split ($240 turns into $740)
			PaySplit paySplitPrincipal=results.ListAutoSplits.First(x => x.SplitAmt==240);
			PaySplit paySplitInterest=results.ListAutoSplits.First(x => x.SplitAmt==10);
			paySplitPrincipal.SplitAmt=740;
			PaySplits.Insert(paySplitPrincipal);
			PaySplits.Insert(paySplitInterest);
			//Make sure that amortization schedule for expected charges is correct after overpayment.
			listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate).ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(4,dictPeriodDateCharges.Count);
			//We paid $740 to principal, and $10 to interest. Meaning our principal remaining is $260:
			//$1000 - $740 = $260; Principal Remaining.
			//Since we over paid the first charge by $510, and our interest was paid even ($1000 * 0.01 = $10)
			//This means that our charges issued in the future, until our floating money is eaten up by those charges, should be static.
			//$740-$250 = $490; Unallocated overpayment.
			//Our first charge was made for a sum of $250, with $240 on principal, and $1000*(0.01)=$10 on interest.
			//$1000 - (MAX(($740 - 240),0) + 240) = $260; Principal Remaining.
			double monthlyPrincipal=dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Principal);
			Assert.AreEqual(247.40,Math.Round(monthlyPrincipal,2));
			Assert.AreEqual(2.60,dictPeriodDateCharges[DateTime.Today.AddMonths(1)].Sum(x => x.Interest));
			//$1000 - (MAX(($740 - 487.4),0) + 487.4) = $260; Principal Remaining.
			//$490-$250 = $240; Unallocated overpayment. 
			monthlyPrincipal=dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal);
			Assert.AreEqual(247.40,Math.Round(monthlyPrincipal,2));
			Assert.AreEqual(2.60,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			//$1000 - (MAX(($740 - 742.2),0) + 734.8) = $734.8; Principal Remaining.
			//$240-$250 = -$10; Unallocated overpayment. After this point our interest should start going lower. Our principal should be 260 for this calculation.
			monthlyPrincipal=dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal);
			Assert.AreEqual(247.40,Math.Round(monthlyPrincipal,2));
			Assert.AreEqual(2.60,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			//$1000 - (MAX(($740 - 982.2),0) + 982.2) = $17.8; Principal Remaining.
			monthlyPrincipal=dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal);
			Assert.AreEqual(17.8,Math.Round(monthlyPrincipal,2));
			Assert.AreEqual(0.18,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
			//$1000 - (MAX(($740 - 1000),0) + 1000) = $17.8; Principal Remaining.
			//Make sure that next charge gets posted correctly.
			List<PayPlanCharge> listChargesForSecondPeriod=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,true);
			Assert.AreEqual(247.40,listChargesForSecondPeriod.Sum(x => x.Principal));
			Assert.AreEqual(2.60,listChargesForSecondPeriod.Sum(x => x.Interest));
			//$1000 - (MAX(($740 - 487.4),0) + 487.4) = $260; Principal Remaining.
			//Make sure the amortization schedule after the second charge is correct.
			listChargesDb.AddRange(listChargesForSecondPeriod);
			listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			dictPeriodDateCharges=listExpectedCharges.GroupBy(x => x.ChargeDate).ToDictionary(x => x.Key,x => x.ToList());
			Assert.AreEqual(3,dictPeriodDateCharges.Count);
			Assert.AreEqual(247.4,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Principal));
			Assert.AreEqual(2.60,dictPeriodDateCharges[DateTime.Today.AddMonths(2)].Sum(x => x.Interest));
			//$1000 - (MAX(($740 - 742.2),0) + 734.8) = $734.8; Principal Remaining.
			Assert.AreEqual((decimal)247.4,(decimal)dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Principal));
			Assert.AreEqual(2.60,dictPeriodDateCharges[DateTime.Today.AddMonths(3)].Sum(x => x.Interest));
			//$1000 - (MAX(($740 - 982.2),0) + 982.2) = $17.8; Principal Remaining.
			Assert.AreEqual(17.8,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Principal));
			Assert.AreEqual(0.18,dictPeriodDateCharges[DateTime.Today.AddMonths(4)].Sum(x => x.Interest));
			//$1000 - (MAX(($740 - 1000),0) + 1000) = $17.8; Principal Remaining.
		}

		[TestMethod]
		public void PayPlanEdit_BalanceOverpaidChargeForDynamicPaymentPlan_PlanOverPaymentMovedToUnpaidHistoricPrincipalFullyChargedOut() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Treat as Complete) with three procedures, $50, $50 and $50, %0 apr.
			 * Issue the first two months charges, and pay off all principal +$5, on one split.
			 * Rebalance the plan on principal.
			 * Assure that a new charge is created and that payment splits are associated to it.
			 * This test is testing the scenario where there is still charges asking for money, on a production thats been fully charged out.
			 *--------------------------------------------------------------------------*/
			//Test variables
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			long dynamicPaymentPlanPrePaymentDef=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);

			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0200",ProcStat.C,"",50,DateTime.Today.AddMonths(1)));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",50,DateTime.Today.AddMonths(2)));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0230",ProcStat.C,"",50,DateTime.Today.AddMonths(2)));
			//listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-1),0,0,45,listProcs,listAdjs
				,PayPlanFrequency.Monthly,runService:true);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $1.00 interest, and $9.00 principal. Pay Total ends up being $14.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,45,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(new List<long>() { pat.PatNum },pat.PatNum,new List<PaySplit>(),
				payment,new List<AccountEntry>(),false,false,null);
			results.ListAutoSplits.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=95; //Overpays by $5.00
			PaySplits.InsertMany(results.ListAutoSplits);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);

			//Balance out the plan
			PayPlanEdit.BalanceOverpaidChargesForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);

			//Take an assertion break
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//Total allocated to a charge
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			//Total charged deficit
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			//Total in regular unearned
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			//Total in hidden unearned
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			//Total moved to hidden unearned
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			//Total moved from hidden unearned
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$14.00 on the account
			Assert.AreEqual(95,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(0,totalMovedToHiddenUnearned);
			Assert.AreEqual(0,totalMovedFromHiddenUnearned);
		}
	}
}
