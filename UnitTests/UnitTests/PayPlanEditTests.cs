using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
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
			List<Procedure> listProcs = new List<Procedure> {proc1,proc2 };
			PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,30,DateTime.Today.AddMonths(-3),0,listProcs);
			Payment payment=PaymentT.MakePayment(pat.PatNum,30,DateTime.Today.AddMonths(-2),payplan.PayPlanNum);//make a payment for the plan
			List<PayPlanCharge> listCharges=PayPlanCharges.GetForPayPlan(payplan.PayPlanNum);
			double totalFutureNegAdjs=PayPlanT.GetTotalNegFutureAdjs(listCharges);
			List<PayPlanCharge> listChargesAndCredits=PayPlanEdit.CreatePayPlanAdjustments(-62,listCharges,totalFutureNegAdjs);//make adjustments for the plan.
			listChargesAndCredits.Add(PayPlanChargeT.CreateNegativeCreditForAdj(pat.PatNum,payplan.PayPlanNum,-62));//add the tx credit for the adjustment
			//Balance should equal 100. $192 of completed tx - $30 payment + $-62 adjustment. 
			PayPlanCharge closeOutCharge=PayPlanEdit.CalculatePatPayPlanCloseoutCharge(listChargesAndCredits,listProcs,payplan,today);
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
		public void PayPlanEdit_CloseOutPatPayPlan_NoCredits() {
			Patient patient=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			DateTime dateTimePayPlan=DateTime.Today;
			//Make a patient payment plan worth $1000 that has no credits.
			PayPlan payPlan=PayPlanT.CreatePayPlanNoCharges(patient.PatNum,1000,dateTimePayPlan);
			//Create a manual amortization schedule where the $1000 is split up into 5 payments.
			//Have the first payment of $200 due today. The remaining $800 will be due in the future.
			List<PayPlanCharge> listPayPlanCharges=new List<PayPlanCharge>();
			List<Procedure> listProcs=new List<Procedure>();
			for(int i=0;i<5;i++) {
				listPayPlanCharges.Add(new PayPlanCharge() {
					ChargeDate=DateTime.Today.AddMonths(listPayPlanCharges.Count),
					ChargeType=PayPlanChargeType.Debit,
					PatNum=patient.PatNum,
					PayPlanNum=payPlan.PayPlanNum,
					Principal=200,
					ProvNum=patient.PriProv,
				});
			}
			//A 'Close Out Charge' of $800 should be suggested when the user closes out this payment plan.
			PayPlanCharge payPlanChargeCloseOut=PayPlanEdit.CalculatePatPayPlanCloseoutCharge(listPayPlanCharges,listProcs,payPlan,DateTime.Today);
			Assert.AreEqual(800,payPlanChargeCloseOut.Principal);
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
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payCur);
			results.ListPaySplitsSuggested.ForEach(x => PaySplits.Insert(x));
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
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			//The auto-split system should suggest three PaySplits; $240, $10, and $500 (unearned).
			Assert.AreEqual(3,results.ListPaySplitsSuggested.Count);
			Assert.AreEqual(1,results.ListPaySplitsSuggested.Count(x => x.SplitAmt==240));
			Assert.AreEqual(1,results.ListPaySplitsSuggested.Count(x => x.SplitAmt==10));
			Assert.AreEqual(1,results.ListPaySplitsSuggested.Count(x => x.SplitAmt==500 && x.UnearnedType > 0));
			//Act like the user manually moved the money from unearned (delete that split) and inflated the principal split ($240 turns into $740)
			PaySplit paySplitPrincipal=results.ListPaySplitsSuggested.First(x => x.SplitAmt==240);
			PaySplit paySplitInterest=results.ListPaySplitsSuggested.First(x => x.SplitAmt==10);
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

		#region Dynamic Payment Plan Rebalancer
		[TestMethod]
		public void PayPlanEdit_CalculatePrincipalRemaining_OnePaymentAllPrincipal() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. No interest.
			 * issue a charge for $20.
			 * Make a payment for all principal of $20.
			 * Make sure that our principal remaining is $80.
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=20;
			int APR=0;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustmentss to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,downPayment,APR,periodPayAmount,listProcs,listAdj);
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetListForPayplan(payPlan.PayPlanNum);
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(payPlan,listPayPlanLinks);
			List<PaySplit> listPaySplitsForPayPlan=new List<PaySplit>();
			List<PayPlanCharge> listExpectedCharges=new List<PayPlanCharge>();
			List<PayPlanCharge> listChargesInDb=new List<PayPlanCharge>();
			double principalRem=PayPlanEdit.CalculatePrincipalRemaining(terms.PrincipalAmount,listPaySplitsForPayPlan,listChargesInDb,listExpectedCharges,false);
			Assert.AreEqual(listProcs.Sum(x=>x.ProcFeeTotal),principalRem);
			PayPlanCharge payPlanCharge=PayPlanChargeT.CreateOne(payPlan.PlanNum,payPlan.Guarantor,payPlan.PatNum,DateTime.Today.AddDays(-7),periodPayAmount,doInsert:false);
			listExpectedCharges.Add(payPlanCharge);
			PaymentT.MakePayment(pat.PatNum,periodPayAmount,payPlanNum:payPlan.PayPlanNum,procNum:proc.ProcNum,listSplits:listPaySplitsForPayPlan);
			listPaySplitsForPayPlan.ForEach(x=>x.PayPlanDebitType=PayPlanDebitTypes.Principal);
			principalRem=PayPlanEdit.CalculatePrincipalRemaining(terms.PrincipalAmount,listPaySplitsForPayPlan,listChargesInDb,listExpectedCharges,false);
			Assert.AreEqual(listProcs.Sum(x=>x.ProcFeeTotal)-periodPayAmount,principalRem);
		}

		[TestMethod]
		public void PayPlanEdit_CalculatePrincipalRemaining_PrincipalRemainingManyPaymentsAllPrincipal() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. No interest.
			 * issue a charge for $20 principal. $0 interest.
			 * Make a payment for all principal of $20.
			 * Make sure that our principal remaining is $80.
			 * Then iterate over the entirety of creating $20 principal charges to make sure it calculates each period appropriately.
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=20;
			double actualPaymentAmount=20;
			int APR=0;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,downPayment,APR,periodPayAmount,listProcs,listAdj);
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetListForPayplan(payPlan.PayPlanNum);
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(payPlan,listPayPlanLinks);
			List<PaySplit> listPaySplitsForPayPlan=new List<PaySplit>();
			List<PayPlanCharge> listExpectedCharges=new List<PayPlanCharge>();
			List<PayPlanCharge> listChargesInDb=new List<PayPlanCharge>();
			double principalRem=PayPlanEdit.CalculatePrincipalRemaining(terms.PrincipalAmount,listPaySplitsForPayPlan,listChargesInDb,listExpectedCharges,false);
			Assert.AreEqual(listProcs.Sum(x=>x.ProcFeeTotal),principalRem);
			for(int i = 0;i<5;i++) {
				PayPlanCharge payPlanCharge=PayPlanChargeT.CreateOne(payPlan.PlanNum,payPlan.Guarantor,payPlan.PatNum,DateTime.Today.AddDays(-7*(5-i)),periodPayAmount,doInsert:false);
				listExpectedCharges.Add(payPlanCharge);
				PaymentT.MakePayment(pat.PatNum,actualPaymentAmount,payPlanNum:payPlan.PayPlanNum,procNum:proc.ProcNum,listSplits:listPaySplitsForPayPlan);
				listPaySplitsForPayPlan.ForEach(x=>x.PayPlanDebitType=PayPlanDebitTypes.Principal);
				principalRem=PayPlanEdit.CalculatePrincipalRemaining(terms.PrincipalAmount,listPaySplitsForPayPlan,listChargesInDb,listExpectedCharges,false);
				Assert.AreEqual(listProcs.Sum(x=>x.ProcFeeTotal)-actualPaymentAmount*(i+1),principalRem);
			}
		}

		[TestMethod]
		public void PayPlanEdit_CalculatePrincipalRemaining_PrincipalRemainingManyPaymentsAllInterest() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. No interest.
			 * issue a charge for $20 principal. $0 interest.
			 * Make a payment for all interest of $20.
			 * Make sure that our principal remaining is $80.
			 * Then iterate over the entirety of creating $20 principal charges, and making $20 interest payments
			 * to make sure it calculates each period appropriately.
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=20;
			double actualPaymentAmount=20;
			int APR=0;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustmentss to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,downPayment,APR,periodPayAmount,listProcs,listAdj);
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetListForPayplan(payPlan.PayPlanNum);
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(payPlan,listPayPlanLinks);
			List<PaySplit> listPaySplitsForPayPlan=new List<PaySplit>();
			List<PayPlanCharge> listExpectedCharges=new List<PayPlanCharge>();
			List<PayPlanCharge> listChargesInDb=new List<PayPlanCharge>();
			double principalRem=PayPlanEdit.CalculatePrincipalRemaining(terms.PrincipalAmount,listPaySplitsForPayPlan,listChargesInDb,listExpectedCharges,false);
			Assert.AreEqual(listProcs.Sum(x=>x.ProcFeeTotal),principalRem);
			for(int i = 0;i<5;i++) {
				PayPlanCharge payPlanCharge=PayPlanChargeT.CreateOne(payPlan.PlanNum,payPlan.Guarantor,payPlan.PatNum,DateTime.Today.AddDays(-7*(5-i)),periodPayAmount,doInsert:false);
				listExpectedCharges.Add(payPlanCharge);
				PaymentT.MakePayment(pat.PatNum,actualPaymentAmount,payPlanNum:payPlan.PayPlanNum,procNum:proc.ProcNum,listSplits:listPaySplitsForPayPlan);
				listPaySplitsForPayPlan.ForEach(x=>x.PayPlanDebitType=PayPlanDebitTypes.Interest);
				principalRem=PayPlanEdit.CalculatePrincipalRemaining(terms.PrincipalAmount,listPaySplitsForPayPlan,listChargesInDb,listExpectedCharges,false);
				Assert.AreEqual(listProcs.Sum(x=>x.ProcFeeTotal)-actualPaymentAmount*(i+1),principalRem);
			}
		}

		[TestMethod]
		public void PayPlanEdit_CalculatePrincipalRemaining_PrincipalRemainingManyPaymentsAllUnknown() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. No interest.
			 * issue a charge for $20 principal. $0 interest.
			 * Make a payment for all unknown of $20.
			 * Make sure that our principal remaining is $80.
			 * Then iterate over the entirety of creating $20 principal charges, and making $20 interest payments
			 * to make sure it calculates each period appropriately.
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=20;
			double actualPaymentAmount=20;
			int APR=0;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustmentss to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,downPayment,APR,periodPayAmount,listProcs,listAdj);
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetListForPayplan(payPlan.PayPlanNum);
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(payPlan,listPayPlanLinks);
			List<PaySplit> listPaySplitsForPayPlan=new List<PaySplit>();
			List<PayPlanCharge> listExpectedCharges=new List<PayPlanCharge>();
			List<PayPlanCharge> listChargesInDb=new List<PayPlanCharge>();
			double principalRem=PayPlanEdit.CalculatePrincipalRemaining(terms.PrincipalAmount,listPaySplitsForPayPlan,listChargesInDb,listExpectedCharges,false);
			Assert.AreEqual(listProcs.Sum(x=>x.ProcFeeTotal),principalRem);
			for(int i = 0;i<5;i++) {
				PayPlanCharge payPlanCharge=PayPlanChargeT.CreateOne(payPlan.PlanNum,payPlan.Guarantor,payPlan.PatNum,DateTime.Today.AddDays(-7*(5-i)),periodPayAmount,doInsert:false);
				listExpectedCharges.Add(payPlanCharge);
				PaymentT.MakePayment(pat.PatNum,actualPaymentAmount,payPlanNum:payPlan.PayPlanNum,procNum:proc.ProcNum,listSplits:listPaySplitsForPayPlan);
				listPaySplitsForPayPlan.ForEach(x=>x.PayPlanDebitType=PayPlanDebitTypes.Unknown);
				principalRem=PayPlanEdit.CalculatePrincipalRemaining(terms.PrincipalAmount,listPaySplitsForPayPlan,listChargesInDb,listExpectedCharges,false);
				Assert.AreEqual(listProcs.Sum(x=>x.ProcFeeTotal)-actualPaymentAmount*(i+1),principalRem);
			}
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_NoBalanceNeeded() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100 and 12% APR with a $30 payment amount.
			 * Issue a charge for the first month.
			 * Make a payment for the entire first month's amount due (fully pay the outstanding balance on the plan).
			 * Invoke the DPP rebalancing logic and make sure no new splits are suggested, as the plan is balanced.
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=30;
			int APR=12;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,downPayment,APR,periodPayAmount,listProcs,listAdj,runService:false);
			//Make Payment
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,periodPayAmount,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment,payPlanNum:payPlan.PayPlanNum);
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			//Get Production Entries
			PayPlanEdit.PayPlanRecalculationData recalcData=PayPlanT.GetRecalculationData(payPlan,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);
			List<PaySplit> listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			List<PayPlanCharge> listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//Assert that we didn't create any splits as there was no imbalance
			Assert.AreEqual(recalcData.ListPaySplits.Count,listSplitsDB.Count);
			//Assert that no value was added, simply moved
			Assert.AreEqual(recalcData.ListPaySplits.Sum(x=>x.SplitAmt),listSplitsDB.Sum(x=>x.SplitAmt));
			//Assert there is no more overpaid interest
			Assert.AreEqual(recalcData.ListPayPlanCharges.Sum(x=>x.Interest),listSplitsDB.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));
			//Assert no new charges were created
			Assert.AreEqual(recalcData.ListPayPlanCharges.Count,listChargesDB.Count);
			//Assert that that charges are the same
			Assert.AreEqual(recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal),listChargesDB.Sum(x=>x.Interest+x.Principal));
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_RebalanceNoUnknownSplits() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. 12% APR.
			 * issue the first months charge.
			 * Make a payment for $30. Let the autosplit system determine how much for interest / principal.
			 * Make sure no new value was added to the plan, i.e. money was only moved.
			 * Make sure interest is no longer over paid.
			 * Make sure we moved the splits to the existing charge. (This scenario shouldn't issue a new charge)
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=30;
			int APR=12;
			long dynamicPaymentPlanPrePaymentDef=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			recalcData.Pat=pat;
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(recalcData.Pat.PatNum,recalcData.Pat.Guarantor,DateTime.Today.AddMonths(-1),downPayment,APR,periodPayAmount,listProcs,listAdj,frequency: PayPlanFrequency.Monthly,runService:false);
			//Make Payment
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,periodPayAmount,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment,payPlanNum:recalcData.PayPlan.PayPlanNum);
			results.ListPaySplitsSuggested.FirstOrDefault(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=30;
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			//Get RecalcData and rebalance plan by pay on principal
			recalcData=PayPlanT.GetRecalculationData(payPlan,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);

			List<PaySplit> listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			List<PayPlanCharge> listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//Assert that no value was added, simply moved
			Assert.AreEqual(recalcData.ListPaySplits.Sum(x=>x.SplitAmt),listSplitsDB.Sum(x=>x.SplitAmt));
			//Assert there is no more overpaid interest
			Assert.AreEqual(1,listSplitsDB.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			double totalInInterest=recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt);
			double totalInPrincipal=recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).Sum(x=>x.SplitAmt);
			double totalInterestCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Interest);
			double totalPrincipalCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Principal);
			//$79.00 on the account
			Assert.AreEqual(59,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(29,totalMovedToHiddenUnearned);
			Assert.AreEqual(29,totalMovedFromHiddenUnearned);
			Assert.AreEqual(0,totalInInterest-totalInterestCharged);
			Assert.AreEqual(0,totalInPrincipal-totalPrincipalCharged);
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_RebalanceSomeUnknownSplits() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. 12% APR.
			 * issue the two months charge.
			 * Make a payment for $30. Let the autosplit system determine how much for interest / principal, and then override as 'unknown' splits.
			 * Make a payment for $35. $29 principal, $6 interest. This will be an overpayment.
			 * Rebalance on principal, and assert that $65 is locked in principal.
			 * Make sure interest is no longer over paid.
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=30;
			int APR=12;
			long dynamicPaymentPlanPrePaymentDef=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			recalcData.Pat=pat;
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(recalcData.Pat.PatNum,recalcData.Pat.Guarantor,DateTime.Today.AddMonths(-1),downPayment,APR,periodPayAmount,listProcs,listAdj,
				frequency:PayPlanFrequency.Monthly,runService:true);

			//Make Payment for first charge
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,periodPayAmount,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment,payPlanNum:payPlan.PayPlanNum);
			results.ListPaySplitsSuggested.ForEach(x=>x.PayPlanDebitType=PayPlanDebitTypes.Unknown);
			PaySplits.InsertMany(results.ListPaySplitsSuggested);

			//Make Payment for second charge
			payment=PaymentT.MakePaymentNoSplits(pat.PatNum,periodPayAmount,payDate:DateTime.Today);
			results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment,payPlanNum:payPlan.PayPlanNum);
			results.ListPaySplitsSuggested.FirstOrDefault(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=6;
			PaySplits.InsertMany(results.ListPaySplitsSuggested);

			//Get RecalcData and rebalance plan by pay on principal
			recalcData=PayPlanT.GetRecalculationData(payPlan,pat,Patients.GetFamily(pat.PatNum));
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);
			
			//Test it
			List<PaySplit> listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			List<PayPlanCharge> listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			Assert.AreEqual(recalcData.ListPaySplits.Sum(x=>x.SplitAmt),listSplitsDB.Sum(x=>x.SplitAmt));
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			double totalInInterest=recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt);
			double totalInPrincipal=recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).Sum(x=>x.SplitAmt);
			double totalInterestCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Interest);
			double totalPrincipalCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Principal);
			//$65.00 on the account
			Assert.AreEqual(65,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(5,totalMovedToHiddenUnearned);
			Assert.AreEqual(5,totalMovedFromHiddenUnearned);
			Assert.IsTrue(totalInInterest<=totalInterestCharged);
			Assert.IsTrue(totalInPrincipal<=totalPrincipalCharged);
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_RebalanceSomeUnknownSplitsUnderpay() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. 12% APR.
			 * issue the two months charge.
			 * Make a payment for $30. Let the autosplit system determine how much for interest / principal, and then override as 'unknown' splits.
			 * Make a payment for $35. $29 principal, $6 interest. This will be an overpayment.
			 * Update one of the unknown splits to under pay by $5.
			 * Rebalance on principal, and assert that $65 is locked in principal.
			 * There should be no underpaid charges now. The $5 underpayment should go to principal.
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=30;
			int APR=12;
			long dynamicPaymentPlanPrePaymentDef=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			recalcData.Pat=pat;
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(recalcData.Pat.PatNum,recalcData.Pat.Guarantor,DateTime.Today.AddMonths(-1),downPayment,APR,periodPayAmount,listProcs,listAdj,frequency: PayPlanFrequency.Monthly,runService:true);

			//Make Payment for first charge
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,periodPayAmount,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment,payPlanNum:payPlan.PayPlanNum);
			results.ListPaySplitsSuggested.ForEach(x=>x.PayPlanDebitType=PayPlanDebitTypes.Unknown);
			PaySplits.InsertMany(results.ListPaySplitsSuggested);

			//Make Payment for second charge
			payment=PaymentT.MakePaymentNoSplits(pat.PatNum,periodPayAmount,payDate:DateTime.Today);
			results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment,payPlanNum:payPlan.PayPlanNum);
			results.ListPaySplitsSuggested.FirstOrDefault(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=6;
			PaySplits.InsertMany(results.ListPaySplitsSuggested);

			//Now Underpay the unknown splits by $5
			recalcData=PayPlanT.GetRecalculationData(payPlan,pat,Patients.GetFamily(pat.PatNum));
			PaySplit underPayingSplit=recalcData.ListPaySplits.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Unknown && x.SplitAmt >= 29);
			underPayingSplit.SplitAmt=underPayingSplit.SplitAmt-5;
			PaySplits.Update(underPayingSplit);

			//Get Production Entries
			recalcData=PayPlanT.GetRecalculationData(payPlan,pat,Patients.GetFamily(pat.PatNum));
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);

			//Test it
			List<PaySplit> listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			List<PayPlanCharge> listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			Assert.AreEqual(recalcData.ListPaySplits.Sum(x=>x.SplitAmt),listSplitsDB.Sum(x=>x.SplitAmt));
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			double totalInInterest=recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt);
			double totalInPrincipal=recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).Sum(x=>x.SplitAmt);
			double totalInterestCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Interest);
			double totalPrincipalCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Principal);
			//$65.00 on the account
			Assert.AreEqual(60,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(0,totalMovedToHiddenUnearned);
			Assert.AreEqual(0,totalMovedFromHiddenUnearned);
			Assert.IsTrue(totalInInterest<=totalInterestCharged);
			Assert.IsTrue(totalInPrincipal<=totalPrincipalCharged);
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_RebalanceSomeUnknownSplitsNeedsNewCharge() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. 12% APR.
			 * issue the two months charge.
			 * Make a payment for $30. Let the autosplit system determine how much for interest / principal, and then override as 'unknown' splits.
			 * Make a payment for $35. $29 principal, $35 interest. This will be an overpayment.
			 * Rebalance on principal, and assert that $65 is locked in principal.
			 * The $34 overpayment should go to a new principal charge.
			 *--------------------------------------------------------------------------*/
			double downPayment=0;
			double periodPayAmount=30;
			int APR=12;
			long dynamicPaymentPlanPrePaymentDef=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);

			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today.AddMonths(-1),downPayment,APR,periodPayAmount,listProcs,listAdj,frequency: PayPlanFrequency.Monthly,runService:true);

			//Make Payment for first charge
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,periodPayAmount,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment,payPlanNum:payPlan.PayPlanNum);
			results.ListPaySplitsSuggested.ForEach(x=>x.PayPlanDebitType=PayPlanDebitTypes.Unknown);
			PaySplits.InsertMany(results.ListPaySplitsSuggested);

			//Make Payment for second charge
			payment=PaymentT.MakePaymentNoSplits(pat.PatNum,periodPayAmount,payDate:DateTime.Today);
			results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment,payPlanNum:payPlan.PayPlanNum);
			results.ListPaySplitsSuggested.FirstOrDefault(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=35;
			PaySplits.InsertMany(results.ListPaySplitsSuggested);

			//Get Production Entries
			recalcData=PayPlanT.GetRecalculationData(payPlan,pat,Patients.GetFamily(pat.PatNum));
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);

			List<PaySplit> listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			List<PayPlanCharge> listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//Assert that no value was added, simply moved
			Assert.AreEqual(recalcData.ListPaySplits.Sum(x=>x.SplitAmt),listSplitsDB.Sum(x=>x.SplitAmt));
			//Assert there is no more overpaid interest
			Assert.IsTrue(!listChargesDB.Any(x=>x.Interest+x.Principal < listSplitsDB.Where(y=>y.PayPlanDebitType==PayPlanDebitTypes.Interest && y.PayPlanChargeNum==x.PayPlanChargeNum).Sum(y=>y.SplitAmt)));
			//Assert exactly one new charge was made
			Assert.AreEqual(recalcData.ListPayPlanCharges.Count+1,listChargesDB.Count);
			//Assert a new principal charge for $34 was made with $0 interest
			Assert.AreEqual(1,listChargesDB.FindAll(x=>x.Principal==34 && x.Interest==0).Count);
			recalcData=PayPlanT.GetRecalculationData(payPlan,pat,Patients.GetFamily(pat.PatNum));
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			double totalInInterest=recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt);
			double totalInPrincipal=recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).Sum(x=>x.SplitAmt);
			double totalInterestCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Interest);
			double totalPrincipalCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Principal);
			//$94.00 on the account
			Assert.AreEqual(94,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(34,totalMovedToHiddenUnearned);
			Assert.AreEqual(34,totalMovedFromHiddenUnearned);
			Assert.IsTrue(totalInInterest<=totalInterestCharged);
			Assert.IsTrue(totalInPrincipal<=totalPrincipalCharged);
		}

		[TestMethod]
		public void PayPlanEdit_ApplyPrepaymentToDynamicPaymentPlan_PreviousPrincipalPaid() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. No interest.
			 * issue one charge for $29 principal, $1.00 interest.
			 * Make a payment for 29 principal, 10 interest.
			 * Rebalance on prepay
			 * Issue another charge for $29.38 principal, $0.62 interest.
			 * Apply prepayment to the charge.
			 * Assert only 3 transfers were made. 1 to unearned, and 2 from unearned.
			 * Assert again we didn't create any aditional value.
			 * Assert we paid interest first, and part of the principal.
			 *--------------------------------------------------------------------------*/
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			double downPayment=0;
			double periodPayAmount=30;
			double periodInterestAmount=5;
			double interestOffSet=5;
			double periodPayActualPrincipal=periodPayAmount-interestOffSet;
			double periodPayActualInterest=periodInterestAmount+interestOffSet;
			int APR=12;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlanDynamic=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,downPayment,APR,periodPayAmount,listProcs,listAdj,frequency: PayPlanFrequency.Monthly);
			//Make Payment
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=10; //overpays by $9
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			PayPlanEdit.PayPlanRecalculationData recalcData=PayPlanT.GetRecalculationData(payPlanDynamic,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			List<PaySplit> listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			List<PayPlanCharge> listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//Assert that no value was added, simply moved
			Assert.AreEqual(recalcData.ListPaySplits.Sum(x=>x.SplitAmt),listSplitsDB.Sum(x=>x.SplitAmt));
			//Assert there is no more overpaid interest
			Assert.AreEqual(true,recalcData.ListPayPlanCharges.Sum(x=>x.Interest)==listSplitsDB.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));
			//Assert no new charges were made.
			Assert.AreEqual(recalcData.ListPayPlanCharges.Count,listChargesDB.Count);
			//Assert there is a paysplit in the hidden unearned prepayment for the DPP, of $9
			Assert.AreEqual(1,listSplitsDB.Count(x=>x.UnearnedType!=0 && x.SplitAmt==9));
			//Issue new PayPlanCharges
			List<PayPlanCharge> listPayPlanChargesForPlan=PayPlanEdit.GetListExpectedCharges(listChargesDB,recalcData.Terms,Patients.GetFamily(pat.PatNum),recalcData.ListPayPlanLinks,recalcData.PayPlan,true);
			foreach(PayPlanCharge payPlanCharge in listPayPlanChargesForPlan) {
				PayPlanCharges.Insert(payPlanCharge);
			}
			recalcData.ListPayPlanCharges=listPayPlanChargesForPlan;
			double hiddenUnearnedTotal=PayPlanEdit.GetDynamicPayPlanPrepaymentAmount(listSplitsDB);
			Assert.AreEqual(9,hiddenUnearnedTotal);
			PayPlanEdit.ApplyPrepaymentToDynamicPaymentPlan(pat.PatNum,hiddenUnearnedTotal,listPayPlanChargesForPlan);
			listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//We took $9 from earned and moved it to hidden unearned, then 0.71 from unearned to earned, and 8.29 from unearned to earned. We should have 3 hidden unearned paysplits
			Assert.AreEqual(3,listSplitsDB.Count(x=>x.UnearnedType==PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType)));
			//Assert that we never created value, only moved money around.
			Assert.AreEqual(39,listSplitsDB.Sum(x=>x.SplitAmt));
			//Assert we applied $9 to the new charge created today
			PayPlanCharge chargeCreatedNow=listChargesDB.First(x=>x.ChargeDate==listChargesDB.Max(y=>y.ChargeDate));
			Assert.AreEqual(9,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum).Sum(x=>x.SplitAmt));
			Assert.AreEqual(0.71,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum && x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));
			Assert.AreEqual(8.29,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum && x.PayPlanDebitType==PayPlanDebitTypes.Principal).Sum(x=>x.SplitAmt));
		}

		[TestMethod]
		public void PayPlanEdit_ApplyPrepaymentToDynamicPaymentPlan_PreviousPrincipalUnpaid() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. No interest.
			 * issue one charge for $29 principal, $1.00 interest.
			 * Make a payment for 10 principal, 30 interest.
			 * Rebalance on prepay
			 * Assert we didn't create any aditional value.
			 * Issue another charge for $29.39 principal, $0.61 interest.
			 * Apply prepayment to the charge.
			 * Assert only 3 transfers were made. 1 to unearned, and 2 from unearned.
			 * Assert we paid interest first, and part of the principal.
			 *--------------------------------------------------------------------------*/
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			double downPayment=0;
			double periodPayAmount=30;
			int APR=12;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlanDynamic=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today.AddMonths(-1),downPayment,APR,periodPayAmount,listProcs,listAdj,frequency:PayPlanFrequency.Monthly,runService:false);
			//Make Payment
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x => x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=30; //overpays by $29
			results.ListPaySplitsSuggested.First(x => x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=10; //underpays by $19
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			PayPlanEdit.PayPlanRecalculationData recalcData=PayPlanT.GetRecalculationData(payPlanDynamic,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			List<PaySplit> listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			List<PayPlanCharge> listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//Assert that no value was added, simply moved
			Assert.AreEqual(recalcData.ListPaySplits.Sum(x=>x.SplitAmt),listSplitsDB.Sum(x=>x.SplitAmt));
			//Assert there is no more overpaid interest
			Assert.AreEqual(recalcData.ListPayPlanCharges.Sum(x=>x.Interest),listSplitsDB.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));
			//Assert no new charges were made.
			Assert.AreEqual(recalcData.ListPayPlanCharges.Count,listChargesDB.Count);
			//Assert there is a paysplit in the hidden unearned prepayment for the DPP, of $29
			Assert.AreEqual(1,listSplitsDB.Count(x=>x.UnearnedType!=0 && x.SplitAmt==10));
			//Issue new PayPlanCharges
			List<PayPlanCharge> listPayPlanChargesForPlan=PayPlanEdit.GetListExpectedCharges(listChargesDB,recalcData.Terms,Patients.GetFamily(pat.PatNum),recalcData.ListPayPlanLinks,recalcData.PayPlan,true);
			foreach(PayPlanCharge payPlanCharge in listPayPlanChargesForPlan) {
				PayPlanCharges.Insert(payPlanCharge);
			}
			recalcData.ListPayPlanCharges=listPayPlanChargesForPlan;
			double hiddenUnearnedTotal=PayPlanEdit.GetDynamicPayPlanPrepaymentAmount(listSplitsDB);
			Assert.AreEqual(10,hiddenUnearnedTotal);
			PayPlanEdit.ApplyPrepaymentToDynamicPaymentPlan(pat.PatNum,hiddenUnearnedTotal,listPayPlanChargesForPlan);
			listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//We took $29 from earned and moved it to hidden unearned, then 0.60 from unearned to earned, and 8.40 from unearned to earned. We should have 3 hidden unearned paysplits
			Assert.AreEqual(3,listSplitsDB.Count(x=>x.UnearnedType==PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType)));
			//Assert that we never created value, only moved money around.
			Assert.AreEqual(true,CompareDouble.IsEqual(listSplitsDB.Sum(x=>x.SplitAmt),40));
			//Assert we applied $29 to the new charge created today
			PayPlanCharge chargeCreatedNow=listChargesDB.First(x=>x.ChargeDate==listChargesDB.Max(y=>y.ChargeDate));
			Assert.AreEqual(10,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum).Sum(x=>x.SplitAmt));
			Assert.AreEqual(0.71,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum && x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));
			Assert.AreEqual(9.29,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum && x.PayPlanDebitType==PayPlanDebitTypes.Principal).Sum(x=>x.SplitAmt));
		}

		[TestMethod]
		public void PayPlanEdit_ApplyPrepaymentToDynamicPaymentPlan_PreviousPrincipalUnpaidOverPaidInterestAndPrincipal() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with only one procedure of $100. No interest.
			 * issue one charge for $29 principal, $1.00 interest.
			 * Make a payment for 10 principal, 30 interest.
			 * Rebalance on prepay
			 * Assert we didn't create any aditional value.
			 * Issue another charge for $29.39 principal, $0.61 interest.
			 * Apply prepayment to the charge.
			 * Assert only 3 transfers were made. 1 to unearned, and 2 from unearned.
			 * Assert we paid interest first, and part of the principal.
			 *--------------------------------------------------------------------------*/
			//set up dynamic pay plan prefs
			PrefT.UpdateDateT(PrefName.DynamicPayPlanLastDateTime,DateTime.MinValue);
			PrefT.UpdateDateT(PrefName.DynamicPayPlanRunTime,DateTime.Now);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			double downPayment=0;
			double periodPayAmount=30;
			int APR=12;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("PPECPRPR123");
			//Make procedures to attach to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			Procedure proc=ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.C,"1",100);
			listProcs.Add(proc);
			//Make adjustments to attach to the payment plan
			List<Adjustment> listAdj=new List<Adjustment>();
			PayPlan payPlanDynamic=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today.AddMonths(-1),downPayment,APR,periodPayAmount,listProcs,listAdj,frequency:PayPlanFrequency.Monthly,runService:false);
			//Make Payment
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x => x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=3; //overpays by $2
			results.ListPaySplitsSuggested.First(x => x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=40; //overpays by $11
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			PayPlanEdit.PayPlanRecalculationData recalcData=PayPlanT.GetRecalculationData(payPlanDynamic,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			List<PaySplit> listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			List<PayPlanCharge> listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//Assert that no value was added, simply moved
			Assert.AreEqual(recalcData.ListPaySplits.Sum(x=>x.SplitAmt),listSplitsDB.Sum(x=>x.SplitAmt));
			//Assert there is no more overpaid interest
			Assert.AreEqual(true,recalcData.ListPayPlanCharges.Sum(x=>x.Interest)==listSplitsDB.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));
			//Assert there is no more overpaid principal
			Assert.AreEqual(true,recalcData.ListPayPlanCharges.Sum(x=>x.Principal)==listSplitsDB.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).Sum(x=>x.SplitAmt));
			//Assert no new charges were made.
			Assert.AreEqual(recalcData.ListPayPlanCharges.Count,listChargesDB.Count);
			//Assert there is a paysplit in the hidden unearned prepayment for the DPP, of $29
			Assert.AreEqual(2,listSplitsDB.Count(x=>x.UnearnedType!=0 && (x.SplitAmt==11 || x.SplitAmt==2)));
			//Issue new PayPlanCharges
			List<PayPlanCharge> listPayPlanChargesForPlan=PayPlanEdit.GetListExpectedCharges(listChargesDB,recalcData.Terms,Patients.GetFamily(pat.PatNum),recalcData.ListPayPlanLinks,recalcData.PayPlan,true);
			foreach(PayPlanCharge payPlanCharge in listPayPlanChargesForPlan) {
				PayPlanCharges.Insert(payPlanCharge);
			}
			recalcData.ListPayPlanCharges=listPayPlanChargesForPlan;
			double hiddenUnearnedTotal=PayPlanEdit.GetDynamicPayPlanPrepaymentAmount(listSplitsDB);
			Assert.AreEqual(13,hiddenUnearnedTotal);
			PayPlanEdit.ApplyPrepaymentToDynamicPaymentPlan(pat.PatNum,hiddenUnearnedTotal,listPayPlanChargesForPlan);
			listSplitsDB=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			listChargesDB=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//We took $13 from earned and moved it to hidden unearned (x2 one for interest, one for principal), then 0.61 from unearned to earned, and 12.40 from unearned to earned. We should have 3 hidden unearned paysplits
			Assert.AreEqual(4,listSplitsDB.Count(x=>x.UnearnedType==PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType)));
			//Assert that we never created value, only moved money around.
			Assert.AreEqual(43,listSplitsDB.Sum(x=>x.SplitAmt));
			//Assert we applied $13 to the new charge created today
			PayPlanCharge chargeCreatedNow=listChargesDB.First(x=>x.ChargeDate==listChargesDB.Max(y=>y.ChargeDate));
			Assert.AreEqual(13,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum).Sum(x=>x.SplitAmt));
			Assert.AreEqual(0.71,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum && x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));
			Assert.AreEqual(12.29,listSplitsDB.Where(x=>x.PayPlanChargeNum==chargeCreatedNow.PayPlanChargeNum && x.PayPlanDebitType==PayPlanDebitTypes.Principal).Sum(x=>x.SplitAmt));
		}

		[TestMethod]
		public void PayPlanEdit_ApplyPrepaymentToDynamicPaymentPlan_TreatAsComplete_CloseOut() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (TP Treat as Complete) with three procedures, $50, $50, and $100. %12 apr.
			 * Issue one charge for $38.00 principal, $2.00 interest.
			 * Make a payment for $40, auto splits to $38 principal and $2 interest.
			 * Increase the interest split to $40, thus $38 overpayment.
			 * Rebalance on prepayment. Should move $38 to hidden unearned.
			 * Assert we've done a transfer of $38 into the unearned prepayment category.
			 * Close out the dynamic payment plan. 
			 * Assert we have only allocated what we could to earned (Total: $52.00)
			 * Assert there are no charges with leftover deficits.
			 * Assert the rest of the prepayment was moved into regular unearned.
			 * Assert there is nothing left in the hidden unearned type for this payment plan.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",50,DateTime.Today.AddMonths(2)));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,40,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,40,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $38.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			recalcData.ListPaySplits=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==38 && x.UnearnedType!=0));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==-38 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Interest));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt>0 && x.UnearnedType!=0));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt<0 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Interest));
			Assert.AreEqual(recalcData.ListPayPlanCharges.Sum(x=>x.Interest),recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));

			//Close the plan out and verify that all prepayments were moved to the charges.
			PayPlanT.CloseDynamicPaymentPlan(dynamicPayPlanAwait,fam);
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			Assert.AreEqual(52,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(26,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_ApplyPrepaymentToDynamicPaymentPlan_AwaitComplete_CloseOut() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Await Complete) with three procedures, $50, $50, and $100. %12 apr.
			 * Issue first months charge. Payments are $40.
			 * Make a payment for $40, auto splits to $39 principal and $1 interest.
			 * Increase the interest split to $40, thus $39 overpayment.
			 * Rebalance on prepayment. Should move $39 to hidden unearned.
			 * Assert we've done a transfer of $39 into the unearned prepayment category.
			 * Close out the dynamic payment plan. 
			 * Assert we have only allocated what we could to earned (Total: $51.00)
			 * Assert there are no charges with leftover deficits.
			 * Assert the rest of the prepayment was moved into regular unearned.
			 * Assert there is nothing left in the hidden unearned type for this payment plan.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,40,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,40,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			recalcData.ListPaySplits=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==39 && x.UnearnedType!=0));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==-39 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Interest));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt>0 && x.UnearnedType!=0));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt<0 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Interest));
			Assert.AreEqual(recalcData.ListPayPlanCharges.Sum(x=>x.Interest),recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));

			//Close the plan out and verify that all prepayments were moved to the charges.
			PayPlanT.CloseDynamicPaymentPlan(dynamicPayPlanAwait,fam);
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			Assert.AreEqual(51,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(28,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_ApplyPrepaymentToDynamicPaymentPlan_ExtraHiddenUnearned_CloseOut_MoveToRegularUnearned() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan with 1 procedures $100.
			 * Make a $200 payment plan prepayment.
			 * Issue first months charge. Payments of 100.
			 * Apply the charges to the first months charge of $100.
			 * Close out the dynamic payment plan. 
			 * Assert that the close out transfers the remaining hidden unearned to regular unearned so the user can use the income transfer.
			 *--------------------------------------------------------------------------*/

			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the 
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today.AddMonths(1)));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,0,100,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete);

			//Create a DynamicPayPlanPrePayment of $200. 
			long prepayUnearnedType=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);
			Payment paymentDynamicPrePay=PaymentT.MakePayment(pat.PatNum,200,payDate:DateTime.Now,procNum:listProcs[0].ProcNum,payPlanNum:dynamicPayPlan.PayPlanNum,unearnedType:prepayUnearnedType);
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);

			//Apply the prepayment to the dynamic payplan
			PayPlanEdit.ApplyPrepaymentToDynamicPaymentPlan(pat.PatNum,paymentDynamicPrePay.PayAmt,listPayPlanCharges);

			//Close the plan out and verify that all prepayments were moved to the charges.
			PayPlanT.CloseDynamicPaymentPlan(dynamicPayPlan,fam);
			List<PaySplit> listUnearned=PaySplits.GetUnearnedForAccount(new List<long> {pat.PatNum});

			//Assert that the dynamic prepayment amounts were transfers to regular unearned.
			Assert.AreEqual(0,listUnearned.FindAll(x => x.UnearnedType==prepayUnearnedType).Sum(x => x.SplitAmt));
		}
		#endregion
		#region Dynamic Payment Plan Charge issuer
		[TestMethod]
		public void PayPlanEdit_IssueChargesDueForDynamicPaymentPlans_PrepaymentsAppliedWhenNextChargesAreIssued() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Await Complete) with three procedures, $50, $50, and $100. %12 apr.
			 * Issue one charge for $38.00 principal, $1.00 interest.
			 * Make a payment for $40, auto splits to $39 principal and $1 interest.
			 * Increase the interest split to $40, thus $39 overpayment.
			 * Rebalance on prepayment. Should move $39 to hidden unearned.
			 * Assert we've done a transfer of $39 into the unearned prepayment category.
			 * Close out the dynamic payment plan. 
			 * Assert we have only allocated what we could to earned (Total: $51.00)
			 * Assert there are no charges with leftover deficits.
			 * Assert the rest of the prepayment was moved into regular unearned.
			 * Assert there is nothing left in the hidden unearned type for this payment plan.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			DateTime dateTime=DateTime.Today.AddMonths(-1);
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,dateTime,0,12,40,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete,runService:false);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,40,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			recalcData.ListPaySplits=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==39 && x.UnearnedType!=0));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==-39 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Interest));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt>0 && x.UnearnedType!=0));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt<0 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Interest));
			Assert.AreEqual(recalcData.ListPayPlanCharges.Sum(x=>x.Interest),recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));

			//Close the plan out and verify that all prepayments were moved to the charges.
			DateTime_.SetNow(() => dateTime.AddMonths(1));//We have to increment our dateTime by 1 month to issue the next charge.
			PayPlanEdit.IssueChargesDueForDynamicPaymentPlans(new List<PayPlan>{ dynamicPayPlanAwait },new CodeBase.LogWriter());
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			Assert.AreEqual(79,totalPaid);
			Assert.AreEqual(1,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(39,totalMovedToHiddenUnearned);
			Assert.AreEqual(39,totalMovedFromHiddenUnearned);
			DateTime_.ResetNow();
		}

		[TestMethod]
		public void PayPlanEdit_IssueChargesDueForDynamicPaymentPlans_ClosedPlansAreSkipped() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Await Complete) with three procedures, $50, $50, and $100. %12 apr.
			 * Issue one charge for $38.00 principal, $1.00 interest.
			 * Make a payment for $40, auto splits to $39 principal and $1 interest.
			 * Increase the interest split to $40, thus $39 overpayment.
			 * Rebalance on prepayment. Should move $39 to hidden unearned.
			 * Assert we've done a transfer of $39 into the unearned prepayment category.
			 * Close out the dynamic payment plan. 
			 * Assert we have only allocated what we could to earned (Total: $51.00)
			 * Assert there are no charges with leftover deficits.
			 * Assert the rest of the prepayment was moved into regular unearned.
			 * Assert there is nothing left in the hidden unearned type for this payment plan.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			DateTime dateTime=DateTime.Today.AddMonths(-1);
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,dateTime,0,12,30,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete,runService:false);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=39; //Overpays by $10.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			recalcData.ListPaySplits=PaySplits.GetForPayPlans(new List<long>{ recalcData.PayPlan.PayPlanNum });
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==39 && x.UnearnedType!=0));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==-39 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Interest));
			Assert.AreEqual(2,recalcData.ListPaySplits.Count(x=>x.SplitAmt>0 && x.UnearnedType!=0));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==-39 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Interest));
			Assert.AreEqual(1,recalcData.ListPaySplits.Count(x=>x.SplitAmt==-10 && x.UnearnedType==0 && x.PayPlanDebitType==PayPlanDebitTypes.Principal));
			Assert.AreEqual(recalcData.ListPayPlanCharges.Sum(x=>x.Interest),recalcData.ListPaySplits.Where(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).Sum(x=>x.SplitAmt));

			//Close the plan out and verify that all prepayments were moved to the charges.
			DateTime_.SetNow(() => dateTime.AddMonths(1));//We have to increment our dateTime by 1 month to issue the next charge.
			PayPlanEdit.IssueChargesDueForDynamicPaymentPlans(new List<PayPlan>{ dynamicPayPlanAwait },new CodeBase.LogWriter());
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$79.00 on the account
			Assert.AreEqual(60,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(19,totalDPPUnearned);
			Assert.AreEqual(49,totalMovedToHiddenUnearned);
			Assert.AreEqual(30,totalMovedFromHiddenUnearned);
			DateTime_.ResetNow();
		}

		[TestMethod]
		public void PayPlanEdit_BalanceOverpaidChargesForDynamicPaymentPlans_PlanOverPaymentMovedToUnearned() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Await Complete) with three procedures, $50 c, $50 c, and $100 tp. %12 apr.
			 * Issue the first months charges. $30 payments.
			 * Make a payment for $30, auto splits handle split generation.
			 * Increase the interest split to $40, thus $39 overpayment.
			 * Increase the principal split to $99, thus $70 overpayment.
			 * Rebalance on principal. Should move up to the plans value into hidden unearned.
			 * Should move the remaineder to regular unearned.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-1),0,12,30,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete,runService:false);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=99; //Overpays by $70.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.PayPlanChargeNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$79.00 on the account
			Assert.AreEqual(101,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(38,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(109,totalMovedToHiddenUnearned);
			Assert.AreEqual(109,totalMovedFromHiddenUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_BalanceOverpaidChargesForDynamicPaymentPlans_PlanOverPaymentMovedToUnearnedTP() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Treat as Complete) with two procedures, $50 c, and $50 tp, %12 apr.
			 * Issue the first months charges. $30 payments.
			 * Make a payment for $30, auto splits handle split generation.
			 * Increase the interest split to $40, thus $39 overpayment.
			 * Increase the principal split to $99, thus $70 overpayment.
			 * Rebalance on prepayment. Should move up to the plans value into hidden unearned.
			 * Should move the remaineder to regular unearned.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",50,DateTime.Today.AddMonths(2)));
			//listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-1),0,12,30,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete,runService:false);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=99; //Overpays by $70.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$79.00 on the account
			Assert.AreEqual(101,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(38,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(109,totalMovedToHiddenUnearned);
			Assert.AreEqual(109,totalMovedFromHiddenUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_BalanceOverpaidChargesForDynamicPaymentPlans_PlanOverPaymentMovedToUnearnedPrepay() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Await Complete) with three procedures, $50 c, $50 c, and $100 tp. %12 apr.
			 * Issue the first months charges. $30 payments.
			 * Make a payment for $30, auto splits handle split generation.
			 * Increase the interest split to $40, thus $39 overpayment.
			 * Increase the principal split to $99, thus $70 overpayment.
			 * Rebalance on principal. Should move up to the plans value into principalcharges.
			 * Should move the remaineder to regular unearned.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-1),0,12,30,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete,runService:false);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=99; //Overpays by $70.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$139.00 on the account
			Assert.AreEqual(30,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(36.75,totalInUnearned);
			Assert.AreEqual(72.25,totalDPPUnearned);
			Assert.AreEqual(109,totalMovedToHiddenUnearned);
			Assert.AreEqual(36.75,totalMovedFromHiddenUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_BalanceOverpaidChargesForDynamicPaymentPlans_PlanOverPaymentMovedToUnearnedTPPrepay() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Treat as Complete) with three procedures, $50, and $50 tp, %12 apr.
			 * Issue The first months charges.
			 * Make a payment for $30, let autosplit handle split generation.
			 * Increase the interest split to $40, thus $39 overpayment.
			 * Increase the principal split to $99, thus $70 overpayment.
			 * Rebalance on prepayment.
			 * Assert we've transfered anything in excess of the plans future value to regular unearned.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",50,DateTime.Today.AddMonths(2)));
			//listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-1),0,12,30,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete,runService:false);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=99; //Overpays by $70.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$79.00 on the account
			Assert.AreEqual(30,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(36.75,totalInUnearned);
			Assert.AreEqual(72.25,totalDPPUnearned);
			Assert.AreEqual(109,totalMovedToHiddenUnearned);
			Assert.AreEqual(36.75,totalMovedFromHiddenUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_PlanOverPaymentMovedToUnearnedPrepayTPAllUnknownSplits() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Treat as Complete) with three procedures, $50, and $50 tp, %12 apr.
			 * Issue The first months charges.
			 * Make a payment for $30, let autosplit handle split generation.
			 * Increase the interest split to $40, thus $39 overpayment.
			 * Increase the principal split to $99, thus $70 overpayment.
			 * Set both splits to unknown payplandebittypes.
			 * Rebalance on prepayment.
			 * Assert we've transfered anything in excess of the plans future value to regular unearned.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",50,DateTime.Today.AddMonths(2)));
			//listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-1),0,12,30,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete,runService:false);

			//Make a payment, over pay interest and principal.
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=99; //Overpays by $70.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).PayPlanDebitType=PayPlanDebitTypes.Unknown;
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).PayPlanDebitType=PayPlanDebitTypes.Unknown;
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$139.00 on the account
			Assert.AreEqual(30,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(36.75,totalInUnearned);
			Assert.AreEqual(72.25,totalDPPUnearned);
			Assert.AreEqual(109,totalMovedToHiddenUnearned);
			Assert.AreEqual(36.75,totalMovedFromHiddenUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_PlanOverPaymentMovedToUnearnedPrincipalTPAllUnknownSplits() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Treat as Complete) with two procedures, $50, and $50 tp, %12 apr.
			 * Issue the first months charge.
			 * Make a payment that completely overpays the plan.
			 * Rebalance the plan on principal.
			 * Make sure that the remaineder of the plan has been charged out, and anything that exceeded the plans value was moved to regular unearned.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",50,DateTime.Today.AddMonths(2)));
			//listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-1),0,12,30,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete,runService:false);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $2.00 interest, and $38.00 principal. Pay Total ends up being $78.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,30,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=99; //Overpays by $70.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).PayPlanDebitType=PayPlanDebitTypes.Unknown;
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=40; //overpays by $39.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).PayPlanDebitType=PayPlanDebitTypes.Unknown;
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			double totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			double totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			double totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			double totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			double totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$139.00 on the account
			Assert.AreEqual(101,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(38,totalInUnearned);
			Assert.AreEqual(0,totalDPPUnearned);
			Assert.AreEqual(109,totalMovedToHiddenUnearned);
			Assert.AreEqual(109,totalMovedFromHiddenUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_PlanOverPaymentMovedToUnearnedMultipleIterations() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Treat as Complete) with two procedures, $50, and $50 Tp, %12 apr.
			 * Issue the first months charge, and pay off interest and principal +$2 on each.
			 * Rebalance the plan on prepay.
			 * Issue the next months charge, and exceed interest to pay off exactly the rest of the principal of the plan.
			 * Rebalance the plan on prepay.
			 * Verify we dont add any splits to regular unearned.
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
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",50,DateTime.Today.AddMonths(2)));
			//listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(3)));
			DateTime dateTime=DateTime.Today.AddMonths(-1);
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,dateTime,0,12,10,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete,runService:false);

			//Make a payment, over pay interest. Charges should be issued for D0200 at $1.00 interest, and $9.00 principal. Pay Total ends up being $14.00
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,10,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=11; //Overpays by $2.00
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Interest).SplitAmt=3; //Overpays by $2.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);

			//Balance out the plan
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);

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
			Assert.AreEqual(10,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(4,totalDPPUnearned);
			Assert.AreEqual(4,totalMovedToHiddenUnearned);
			Assert.AreEqual(0,totalMovedFromHiddenUnearned);

			//Issue the next set of charges
			DateTime_.SetNow(() => dateTime.AddMonths(1));//We have to increment our dateTime by 1 month to issue the next charge.
			PayPlanEdit.IssueChargesDueForDynamicPaymentPlans(new List<PayPlan>{ dynamicPayPlanAwait },new CodeBase.LogWriter());

			//Make a payment, over pay interest.
			payment=PaymentT.MakePaymentNoSplits(pat.PatNum,6,payDate:DateTime.Today);
			results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.FirstOrDefault(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=87.91;
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);

			//Balance out the plan
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },false);

			//Refresh recalculation data.
			recalcData.ListPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			recalcData.ListPayPlanCharges=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			//Total allocated to a charge
			totalPaid=recalcData.ListPaySplits.Where(x=>x.UnearnedType==0 && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			//Total charged deficit
			totalLeftDue=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)-totalPaid;
			//Total in regular unearned
			totalInUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType!=0 && x.UnearnedType!=dynamicPaymentPlanPrePaymentDef).Sum(x=>x.SplitAmt);
			//Total in hidden unearned
			totalDPPUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.PayPlanNum!=0).Sum(x=>x.SplitAmt);
			//Total moved to hidden unearned
			totalMovedToHiddenUnearned=recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt>0).Sum(x=>x.SplitAmt);
			//Total moved from hidden unearned
			totalMovedFromHiddenUnearned=-recalcData.ListPaySplits.Where(x=>x.UnearnedType==dynamicPaymentPlanPrePaymentDef && x.SplitAmt<0).Sum(x=>x.SplitAmt);
			//$101.91 on the account
			Assert.AreEqual(20,totalPaid);
			Assert.AreEqual(0,totalLeftDue);
			Assert.AreEqual(0,totalInUnearned);
			Assert.AreEqual(81.91,totalDPPUnearned);
			Assert.AreEqual(85.91,totalMovedToHiddenUnearned);
			Assert.AreEqual(4,totalMovedFromHiddenUnearned);
			DateTime_.ResetNow();
		}

		[TestMethod]
		public void PayPlanEdit_CreateTransferForDynamicPaymentPlans_PlanOverPaymentMovedToUnpaidHistoricPrincipalFullyChargedOut() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (Treat as Complete) with three procedures, $50, $50 and $50, %0 apr.
			 * Issue the first two months charges, and pay off all principal +$5, on one split.
			 * Rebalance the plan on principal.
			 * Assure that a new charge is created and that payment splits are associated to it.
			 * This test is for the scenario where there is still charges asking for money, on a production thats been fully charged out.
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
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			results.ListPaySplitsSuggested.First(x=>x.PayPlanDebitType==PayPlanDebitTypes.Principal).SplitAmt=95; //Overpays by $5.00
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Get recalculation data and rebalance on prepay.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);

			//Balance out the plan
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(new List<PayPlanEdit.PayPlanRecalculationData>{ recalcData },true);

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
			Assert.AreEqual(5,totalMovedToHiddenUnearned);
			Assert.AreEqual(5,totalMovedFromHiddenUnearned);
		}

		[TestMethod]
		public void PayPlanEdit_GetCloseoutChargesForDynamicPaymentPlan_CloseOut() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (TP Treat as Complete) with three procedures, $139.1, $483, and $174.8. %10 apr.
			 * Issue one down payment charge for $96.90 principal.
			 * Make a payment for $96.90, auto splits to $96.90 principal.
			 * Add insurance to the patient, make claims procs for:
			 *		D0150: InsPay = 59.30;  Writeoff = 20.50; 
			 *		D0210: InsPay = 74.50;  Writeoff = 25.80;
			 *		D1110: InsPay = 205.80; Writeoff = 71.40;
			 * Recieve the ins payment.
			 * Close out the dynamic payment plan. 
			 * Assert that none of the closeout charges are tied to the unlinked procedures
			 * Assert that the closeout charges sum to the value of the D0150 procedure.
			 *--------------------------------------------------------------------------*/
			//Test variables
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			long dynamicPaymentPlanPrePaymentDef=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);
			List<Procedure> listProcsExpectedToBeDetached=new List<Procedure>();

			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",139.1,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",483,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",174.8,DateTime.Today));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),96.90,10,100,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete,runService:true);

			//Make a payment for the down payment charge.
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,96.90,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Attach insurance as the customer spaced out having it.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,planType:"p",feeSchedNum:ppoFeeSchedNum);
			BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Restorative,50);
			foreach(Procedure proc in listProcs) {
				FeeT.CreateFee(ppoFeeSchedNum,proc.CodeNum,amount:proc.ProcFee/2);
			}
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			listProcs=Procedures.Refresh(pat.PatNum);
			ins.RefreshBenefits();
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			foreach(ClaimProc claimProc in listClaimProcs) {
				claimProc.Percentage=50;
				Procedure procForClaim=listProcs.First(x=>x.ProcNum==claimProc.ProcNum);
				claimProc.InsPayEst=procForClaim.ProcFee/2;
				claimProc.BaseEst=claimProc.InsPayEst;
				double writeoff=0;
				double payamt=0;
				switch(ProcedureCodes.GetProcCodeFromDb(procForClaim.CodeNum).ProcCode){
					case "D0150":
						writeoff=20.5;
						payamt=59.30;
						break;
					case "D0210":
						listProcsExpectedToBeDetached.Add(procForClaim);
						writeoff=25.8;
						payamt=74.5;
						break;
					case "D1110":
						listProcsExpectedToBeDetached.Add(procForClaim);
						writeoff=71.4;
						payamt=205.8;
						break;
				}
				claimProc.InsPayAmt=payamt;
				claimProc.WriteOff=writeoff;
				ClaimProcs.Update(claimProc);
			}
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);

			//Recieve that bad boyo and finalize it cause woof we want that moneyyyyy
			ClaimT.ReceiveClaim(claim,listClaimProcs,true);

			//Close the plan out and verify that all prepayments were moved to the charges.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			List<PayPlanCharge> listPayPlanChargesBeforeCloseout=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			PayPlanT.CloseDynamicPaymentPlan(dynamicPayPlanAwait,fam);
			List<PayPlanCharge> listPayPlanChargesAfterCloseout=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			Assert.IsTrue(!listPayPlanChargesAfterCloseout.Exists(x=> listProcsExpectedToBeDetached.Select(y => y.ProcNum).ToList().Contains(x.ProcNum)));
			Assert.IsTrue(CompareDouble.IsEqual(139.1,listPayPlanChargesAfterCloseout.Sum(x=>x.Interest+x.Principal)));
		}

		public void PayPlanEdit_GetCloseoutChargesForDynamicPaymentPlan_CloseOutNochargeNeeded() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (TP Treat as Complete) with three procedures, $139.1, $483, and $174.8. %10 apr.
			 * Issue one down payment charge for $96.90 principal.
			 * Make a payment for $96.90, auto splits to $96.90 principal.
			 * Add insurance to the patient, make claims procs for:
			 *		D0150: InsPay = 59.30;  Writeoff = 20.50; 
			 *		D0210: InsPay = 74.50;  Writeoff = 25.80;
			 *		D1110: InsPay = 205.80; Writeoff = 71.40;
			 * Recieve the ins payment.
			 * Close out the dynamic payment plan. 
			 * Assert that none of the closeout charges are tied to the unlinked procedures
			 * Assert that the closeout charges sum to the value of the D0150 procedure.
			 *--------------------------------------------------------------------------*/
			//Test variables
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			long dynamicPaymentPlanPrePaymentDef=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);
			List<Procedure> listProcsExpectedToBeDetached=new List<Procedure>();

			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",96.90,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",483,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",174.8,DateTime.Today));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),96.90,10,100,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete,runService:true);

			//Make a payment for the down payment charge.
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,96.90,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			PaySplits.InsertMany(results.ListPaySplitsSuggested);
			
			//Attach insurance as the customer spaced out having it.
			string suffix=MethodBase.GetCurrentMethod().Name;
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,planType:"p",feeSchedNum:ppoFeeSchedNum);
			BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Restorative,50);
			foreach(Procedure proc in listProcs) {
				FeeT.CreateFee(ppoFeeSchedNum,proc.CodeNum,amount:proc.ProcFee/2);
			}
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			listProcs=Procedures.Refresh(pat.PatNum);
			ins.RefreshBenefits();
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			foreach(ClaimProc claimProc in listClaimProcs) {
				claimProc.Percentage=50;
				Procedure procForClaim=listProcs.First(x=>x.ProcNum==claimProc.ProcNum);
				claimProc.InsPayEst=procForClaim.ProcFee/2;
				claimProc.BaseEst=claimProc.InsPayEst;
				double writeoff=0;
				double payamt=0;
				switch(ProcedureCodes.GetProcCodeFromDb(procForClaim.CodeNum).ProcCode){
					case "D0150":
						writeoff=20.5;
						payamt=59.30;
						break;
					case "D0210":
						listProcsExpectedToBeDetached.Add(procForClaim);
						writeoff=25.8;
						payamt=74.5;
						break;
					case "D1110":
						listProcsExpectedToBeDetached.Add(procForClaim);
						writeoff=71.4;
						payamt=205.8;
						break;
				}
				claimProc.InsPayAmt=payamt;
				claimProc.WriteOff=writeoff;
				ClaimProcs.Update(claimProc);
			}
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);

			//Recieve that bad boyo and finalize it cause woof we want that moneyyyyy
			ClaimT.ReceiveClaim(claim,listClaimProcs,true);

			//Close the plan out and verify that all prepayments were moved to the charges.
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlanAwait,pat,fam);
			List<PayPlanCharge> listPayPlanChargesBeforeCloseout=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			PayPlanT.CloseDynamicPaymentPlan(dynamicPayPlanAwait,fam);
			List<PayPlanCharge> listPayPlanChargesAfterCloseout=PayPlanCharges.GetForPayPlan(recalcData.PayPlan.PayPlanNum);
			Assert.IsTrue(!listPayPlanChargesAfterCloseout.Exists(x=> listProcsExpectedToBeDetached.Select(y => y.ProcNum).ToList().Contains(x.ProcNum)));
			Assert.IsTrue(CompareDouble.IsEqual(96.90,listPayPlanChargesAfterCloseout.Sum(x=>x.Interest+x.Principal)));
			Assert.AreEqual(listPayPlanChargesBeforeCloseout.Count,listPayPlanChargesAfterCloseout.Count);
		}
		#endregion
	}
}
