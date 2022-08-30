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

namespace UnitTests.PayPlans_Tests {
	[TestClass]
	public class PayPlansTests:TestBase {

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

		///<summary>Ensures that running GetOverPaidPayPlans correctly finds the overpaid PayPlan and returns it.</summary>
		[TestMethod]
		public void PayPlans_GetOverpaidPayPlans_CorrectlyReturnsOverPaidPayPlan() {
			//Setup
			long provNum=ProviderT.CreateProvider("LS");
			Patient pat=PatientT.CreatePatient(fName:"Austin",lName:"Patient",priProvNum:provNum);
			Carrier carrier=CarrierT.CreateCarrier("Blue Cross");
			InsPlan insplan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			InsSubT.CreateInsSub(pat.PatNum,insplan.PlanNum,insInfo.PriInsSub.SubscriberID);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",4100,DateTime.Today.AddMonths(-1),provNum:provNum);
			Benefit benefit=BenefitT.CreatePercentForProc(insInfo.PriInsPlan.PlanNum,proc.CodeNum,50);
			insInfo.AddBenefit(benefit);
			insInfo.ListAllProcs=Procedures.Refresh(pat.PatNum);
			//Make a dynamic payment plan where the entire amount of the procedure is due right now (today).
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,0,0,2050,
				insInfo.ListAllProcs,new List<Adjustment>{ });
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			Assert.AreEqual(1,listPayPlanCharges.Count);
			//Patient pays their portion.
			PaymentT.MakePayment(pat.PatNum,2050,payDate: DateTime.Now,procNum: proc.ProcNum,payPlanNum: dynamicPayPlan.PayPlanNum,
				payPlanChargeNum: listPayPlanCharges[0].PayPlanChargeNum);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			insInfo.ListAllProcs=Procedures.Refresh(pat.PatNum);
			//Insurance pays their portion.
			Claim claim=ClaimT.CreateClaim("P",insInfo.ListPatPlans,insInfo.ListInsPlans,insInfo.ListAllClaimProcs,insInfo.ListAllProcs,pat,
				new List<Procedure>{proc},insInfo.ListBenefits,insInfo.ListInsSubs);
			ClaimT.ReceiveClaim(claim,insInfo.ListAllClaimProcs,doAddPayAmount:true);
			//Payment plan should not be overpaid.
			List<PayPlan> listOverPaidDPP=PayPlans.GetOverChargedPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			Assert.AreEqual(0,listOverPaidDPP.Count);
		}

		///<summary>Ensures that running GetOverPaidPayPlans correctly discerns when a PayPlan is not overpaid then fails to return it.</summary>
		[TestMethod]
		public void PayPlans_GetOverpaidPayPlans_DoesNotReturnPropperlyPaidPayPlan() {
			//Setup
			long provNum=ProviderT.CreateProvider("LS");
			Patient pat=PatientT.CreatePatient(fName:"Austin",lName:"Patient",priProvNum:provNum);
			Carrier carrier=CarrierT.CreateCarrier("Blue Cross");
			InsPlan insplan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			InsSubT.CreateInsSub(pat.PatNum,insplan.PlanNum,insInfo.PriInsSub.SubscriberID);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",4100,DateTime.Today.AddMonths(-1),provNum:provNum);
			Benefit benefit=BenefitT.CreatePercentForProc(insInfo.PriInsPlan.PlanNum,proc.CodeNum,50);
			insInfo.AddBenefit(benefit);
			insInfo.ListAllProcs=Procedures.Refresh(pat.PatNum);
			//Make a dynamic payment plan where the entire amount of the procedure is due right now (today).
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,0,0,2050,
				insInfo.ListAllProcs,new List<Adjustment>{ });
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			Assert.AreEqual(1,listPayPlanCharges.Count);
			//Patient pays their portion.
			PaymentT.MakePayment(pat.PatNum,2050,payDate: DateTime.Now,procNum: proc.ProcNum,payPlanNum: dynamicPayPlan.PayPlanNum,
				payPlanChargeNum: listPayPlanCharges[0].PayPlanChargeNum);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			insInfo.ListAllProcs=Procedures.Refresh(pat.PatNum);
			//Insurance pays their portion.
			Claim claim=ClaimT.CreateClaim("P",insInfo.ListPatPlans,insInfo.ListInsPlans,insInfo.ListAllClaimProcs,insInfo.ListAllProcs,pat,
				new List<Procedure>{proc},insInfo.ListBenefits,insInfo.ListInsSubs);
			ClaimT.ReceiveClaim(claim,insInfo.ListAllClaimProcs,doAddPayAmount:true);
			//And then insurance overpays their portion.
			ClaimProcT.AddInsPaid(pat.PatNum,insplan.PlanNum,proc.ProcNum,50,insInfo.PriInsSub.InsSubNum,0,0);
			//Payment plan should be overpaid.
			List<PayPlan> listOverPaidDPP=PayPlans.GetOverChargedPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			Assert.AreEqual(1,listOverPaidDPP.Count);
		}

		///<summary>Ensures that running GetOverPaidPayPlans correctly discerns when a PayPlan is not overpaid even when there is an adjustment on the procedure.</summary>
		[TestMethod]
		public void PayPlans_GetOverpaidPayPlans_ConsidersAdjOnPayPlan() {
			//Setup
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient pat=PatientT.CreatePatient(fName:"Austin",lName:"Patient",priProvNum:provNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			Carrier carrier=CarrierT.CreateCarrier("Blue Cross");
			InsPlan insplan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			InsSubT.CreateInsSub(pat.PatNum,insplan.PlanNum,insInfo.PriInsSub.SubscriberID);
			//$4,100 proc due last month
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",4100,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Inflate the procedure by $950 with a positive adjustment.
			AdjustmentT.MakeAdjustment(pat.PatNum,950,procNum: proc.ProcNum,provNum: provNum);
			//Insurance has a benefit to cover 50% of the proc and doesn't care about adjustments.
			Benefit benefit=BenefitT.CreatePercentForProc(insInfo.PriInsPlan.PlanNum,proc.CodeNum,50);
			insInfo.AddBenefit(benefit);
			insInfo.ListAllProcs=Procedures.Refresh(pat.PatNum);
			ProcedureT.SetComplete(proc,pat,insInfo);
			//DPP total is for the remaining $2,050 + $950 = $3,000
			//However, weekly charges starting a month ago will only accumulate ~$1,680.
			//This means that the payment plan should never be considered over charged since insurance will only overpay by $10.
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,DateTime.Today.AddMonths(-1),0,0,280,insInfo.ListAllProcs,new List<Adjustment>{ });
			//Insurance pays their portion, but pays more than anticipated (by $10).
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			insInfo.ListAllClaimProcs.First().InsPayEst=2060;
			Claim claim=ClaimT.CreateClaim("P",insInfo.ListPatPlans,insInfo.ListInsPlans,insInfo.ListAllClaimProcs,insInfo.ListAllProcs,pat,
				new List<Procedure>{proc},insInfo.ListBenefits,insInfo.ListInsSubs);
			ClaimT.ReceiveClaim(claim,insInfo.ListAllClaimProcs,doAddPayAmount: true);
			//Make sure that the payment plan doesn't think it's over charged.
			List<PayPlan> listOverPaidDPP=PayPlans.GetOverChargedPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			Assert.AreEqual(0,listOverPaidDPP.Count);
		}

		[TestMethod]
		public void PayPlans_GetOverpaidPayPlans_ConsidersExplicitAdjOnPayPlan() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc=ProcedureT.CreateProcedure(pat,"CEAOPP1",ProcStat.C,"",100,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Adjustments that are explicitly linked to procedures should directly inflate the value of the procedure.
			AdjustmentT.MakeAdjustment(pat.PatNum,50,procNum: proc.ProcNum,provNum: provNum);
			DateTime datePayPlan=DateTime.Today;
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,datePayPlan,0,0,10,new List<Procedure>(){ proc },new List<Adjustment>{ },
				frequency:PayPlanFrequency.Monthly);
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			Assert.AreEqual(0,dynamicPayPlan.CompletedAmt);
			Assert.AreEqual(0,dynamicPayPlan.NumberOfPayments);
			Assert.AreEqual(1,listChargesDb.Count);
			Assert.AreEqual(1,listChargesDb.Count(x => x.ChargeType==PayPlanChargeType.Debit
				&& x.LinkType==PayPlanLinkType.Procedure
				&& x.FKey==proc.ProcNum
				&& x.Principal==10
				&& x.ChargeDate==datePayPlan));
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			//Since the procedure is actually for $150 dollars, there should be 14 $10 expected charges (since the first $10 is already in the db).
			Assert.AreEqual(14,listExpectedCharges.Count);
			for(int i=0;i<14;i++) {
				DateTime dateNextCharge=datePayPlan.AddMonths(i+1);
				Assert.AreEqual(dateNextCharge,listExpectedCharges[i].ChargeDate);
				Assert.AreEqual(10,listExpectedCharges[i].Principal);
			}
		}

		[TestMethod]
		public void PayPlans_GetOverpaidPayPlans_ConsidersImplicitAdjOnPayPlan() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			long provNum2=ProviderT.CreateProvider($"{suffix}-2");
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc=ProcedureT.CreateProcedure(pat,"CIAOPP1",ProcStat.C,"",100,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Adjustments that are implicitly linked to procedures should NOT inflate the value of the procedure.
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,50,procNum: proc.ProcNum,provNum: provNum2);
			DateTime datePayPlan=DateTime.Today;
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,datePayPlan,0,0,10,new List<Procedure>(){ proc },new List<Adjustment>{ adj },
				frequency:PayPlanFrequency.Monthly);
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			Assert.AreEqual(0,dynamicPayPlan.CompletedAmt);
			Assert.AreEqual(0,dynamicPayPlan.NumberOfPayments);
			Assert.AreEqual(1,listChargesDb.Count);
			Assert.AreEqual(1,listChargesDb.Count(x => x.ChargeType==PayPlanChargeType.Debit
				&& x.LinkType==PayPlanLinkType.Procedure
				&& x.FKey==proc.ProcNum
				&& x.Principal==10
				&& x.ChargeDate==datePayPlan));
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			//Since the procedure is actually for $100 dollars and the implicitly linked adjustment was manually added to the dynamic payment plan,
			//there should be 14 $10 expected charges (since the first $10 is already in the db).
			Assert.AreEqual(14,listExpectedCharges.Count);
			for(int i=0;i<14;i++) {
				DateTime dateNextCharge=datePayPlan.AddMonths(i+1);
				Assert.AreEqual(dateNextCharge,listExpectedCharges[i].ChargeDate);
				Assert.AreEqual(10,listExpectedCharges[i].Principal);
			}
		}

		[TestMethod]
		public void PayPlans_GetOverpaidPayPlans_IgnoreImplicitAdjOnPayPlan() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			long provNum2=ProviderT.CreateProvider($"{suffix}-2");
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc=ProcedureT.CreateProcedure(pat,"IIAOPP1",ProcStat.C,"",100,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Adjustments that are implicitly linked to procedures should NOT inflate the value of the procedure.
			AdjustmentT.MakeAdjustment(pat.PatNum,50,procNum: proc.ProcNum,provNum: provNum2);
			DateTime datePayPlan=DateTime.Today;
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,datePayPlan,0,0,10,new List<Procedure>(){ proc },new List<Adjustment>{ },
				frequency:PayPlanFrequency.Monthly);
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			Assert.AreEqual(0,dynamicPayPlan.CompletedAmt);
			Assert.AreEqual(0,dynamicPayPlan.NumberOfPayments);
			Assert.AreEqual(1,listChargesDb.Count);
			Assert.AreEqual(1,listChargesDb.Count(x => x.ChargeType==PayPlanChargeType.Debit
				&& x.LinkType==PayPlanLinkType.Procedure
				&& x.FKey==proc.ProcNum
				&& x.Principal==10
				&& x.ChargeDate==datePayPlan));
			PayPlanTerms terms=PayPlanT.GetTerms(dynamicPayPlan,listEntries);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(listChargesDb,terms,fam,listEntries,dynamicPayPlan,false);
			//Since the procedure is actually for $100 dollars, there should be 9 $10 expected charges (since the first $10 is already in the db).
			Assert.AreEqual(9,listExpectedCharges.Count);
			for(int i=0;i<9;i++) {
				DateTime dateNextCharge=datePayPlan.AddMonths(i+1);
				Assert.AreEqual(dateNextCharge,listExpectedCharges[i].ChargeDate);
				Assert.AreEqual(10,listExpectedCharges[i].Principal);
			}
		}

		///<summary>A bug was causing pay plans to be flagged as overcharged when more than half of a linked production entry was paid.</summary>
		[TestMethod]
		public void PayPlans_GetOverpaidPayPlans_DoNotReturnPayPlanWhenMajorityOfPrincipalIsPaid() {
			//Setup
			long provNum=ProviderT.CreateProvider("LS");
			Patient pat=PatientT.CreatePatient(fName:"Austin",lName:"Patient",priProvNum:provNum);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",4100,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Make a dynamic payment plan where over half of the production is due right now.
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.Guarantor,DateTime.Today,0,0,2060,
				new List<Procedure>{proc},new List<Adjustment>{ });
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetForPayPlan(dynamicPayPlan.PayPlanNum);
			Assert.AreEqual(1,listPayPlanCharges.Count);
			//Patient pays full amount that is due
			PaymentT.MakePayment(pat.PatNum,2060,payDate: DateTime.Now,procNum: proc.ProcNum,payPlanNum: dynamicPayPlan.PayPlanNum,
				payPlanChargeNum: listPayPlanCharges[0].PayPlanChargeNum);
			//Payment plan should not be overpaid.
			List<PayPlan> listOverPaidDPP=PayPlans.GetOverChargedPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			Assert.AreEqual(0,listOverPaidDPP.Count);
		}

		///<summary>A bug was causing pay plans to be flagged as overcharged when the pay splits applied to a production entry were greater
		///than the amount charged for that production entry. Pay plans should only be flagged as overcharged when the total for all debits exceeds
		///the total principal for all production entries.</summary>
		[TestMethod]
		public void PayPlans_GetOverpaidPayPlans_DoNotReturnPayPlanWhenSingleProductionEntryIsOvercharged() {
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
			PaymentT.MakePayment(pat.PatNum,58,payDate:DateTime.Now,procNum:proc2.ProcNum,payPlanNum:dynamicPayPlan.PayPlanNum,
				payPlanChargeNum:proc2PayPlanCharge.PayPlanChargeNum);
			PaymentT.MakePayment(pat.PatNum,12,payDate: DateTime.Now,procNum: proc1.ProcNum,payPlanNum: dynamicPayPlan.PayPlanNum,
				payPlanChargeNum: proc1PayPlanCharge.PayPlanChargeNum);
			//Payment plan should not be overpaid.
			List<PayPlan> listOverPaidDPP=PayPlans.GetOverChargedPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			Assert.AreEqual(0,listOverPaidDPP.Count);
		}

		[TestMethod]
		public void PayPlanEdit_GetProductionForLinks_PayPlanLinksConsiderPatientPayments() {
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",50,DateTime.Today));
			PaymentT.MakePayment(pat.PatNum,22,procNum:listProcs.First().ProcNum);
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),5,12,25,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2));
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			List<PayPlanProductionEntry> listEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
			Assert.AreEqual(28,listEntries.Sum(x => x.AmountOriginal));//amount attached to the payplan should only attach amount needing to still be paid
		}

		#region Dynamic Pay Plans Treatments Planned
		///<summary>Asserts that the pay plan links for Treatment planned procedures, on a dynamic payment plan set to await complete, are ignored.</summary>
		[TestMethod]
		public void PayPlansDynamic_GetProductionForLinks_PayPlanLinksConsiderPatientPayments() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (TP Await Complete) with three procedures, $139.1, $483, and $174.8. %0 apr.
			 * Make a TP unearned prepeayment of 96.90 for D0150.
			 * Assert no charges will be issued in the current state.
			 * Set D0150 as complete.
			 * Assert that 42.20 is expected to be charged out.
			 * Assert the sum of the splits, and charges on the account sum up to the proc fee of D0150
			 *--------------------------------------------------------------------------*/
			//Test variables
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			long hiddenUnearnedDef=PrefC.GetLong(PrefName.PrepaymentUnearnedType);

			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0150",ProcStat.TP,"",139.1,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",483,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",174.8,DateTime.Today));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-4),0,0,100,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete,runService:true);

			//Assert that the DPP wont issue any charges in its current state (all procs TP'd)
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlan,pat,fam);
			List<PayPlanCharge> listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			List<PayPlanProductionEntry> payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(recalcData.ListPayPlanLinks);
			double completedAmount=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(dynamicPayPlan,payPlanProductionEntries);
			Assert.AreEqual(0,recalcData.ListPayPlanCharges.Count+listPayPlanCharges.Count);
			Assert.AreEqual(0,completedAmount);

			//Set the first proc complete
			Procedure procOld=listProcs.First().Copy();
			listProcs.First().ProcStatus=ProcStat.C;
			Procedures.Update(listProcs.First(),procOld);
			listProcs.Remove(listProcs.First());

			//Assert that expected charges are now producing charges for the one completed proc.
			listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(recalcData.ListPayPlanLinks);
			completedAmount=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(dynamicPayPlan,payPlanProductionEntries);
			Assert.AreEqual(2,listPayPlanCharges.Count);
			Assert.IsTrue(CompareDouble.IsEqual(139.1,completedAmount));

			//Set the second proc complete
			procOld=listProcs.First().Copy();
			listProcs.First().ProcStatus=ProcStat.C;
			Procedures.Update(listProcs.First(),procOld);
			listProcs.Remove(listProcs.First());

			//Assert that expected charges are now producing charges for the one completed proc.
			listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(recalcData.ListPayPlanLinks);
			completedAmount=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(dynamicPayPlan,payPlanProductionEntries);
			Assert.AreEqual(8,listPayPlanCharges.Count);
			Assert.IsTrue(CompareDouble.IsEqual(622.1,completedAmount));

			//Set the third proc complete
			procOld=listProcs.First().Copy();
			listProcs.First().ProcStatus=ProcStat.C;
			Procedures.Update(listProcs.First(),procOld);
			listProcs.Remove(listProcs.First());

			//Assert that expected charges are now producing charges for the one completed proc.
			listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(recalcData.ListPayPlanLinks);
			completedAmount=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(dynamicPayPlan,payPlanProductionEntries);
			Assert.AreEqual(10,listPayPlanCharges.Count);
			Assert.IsTrue(CompareDouble.IsEqual(796.9,completedAmount));
		}

		///<summary>Asserts that no charges are issud, and that interest isn't accured for a DPP that is 50 years old.
		///When the production gets set to complete, then charges are issued, and interest starts being procured.</summary>
		[TestMethod]
		public void PayPlansDynamic_GetEstimatedSchedule_AwaitCompleteOneTreatmentPlanned() {
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			Procedure procOld=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",1000,DateTime.Today.AddDays(5));
			listProcs.Add(procOld);
			double downPayment=5;
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddYears(-50),downPayment,12,25,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddYears(-49));
			dynamicPayPlan.DynamicPayPlanTPOption=DynamicPayPlanTPOptions.AwaitComplete;
			PayPlans.Update(dynamicPayPlan);
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			List<PayPlanProductionEntry> listEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(dynamicPayPlan,listPayPlanLinks);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(new List<PayPlanCharge>(),terms,fam,listPayPlanLinks,dynamicPayPlan,true,false);
			Assert.AreEqual(0,listExpectedCharges.Count);//amount attached to the payplan should only attach amount needing to still be paid
			Procedure proc=procOld.Copy();
			//Now set the procedure to complete and test what the interest is
			proc.ProcStatus=ProcStat.C;
			Procedures.Update(proc,procOld);
			listEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
			listExpectedCharges=PayPlanEdit.GetListExpectedCharges(new List<PayPlanCharge>(),terms,fam,listPayPlanLinks,dynamicPayPlan,false,false);
			Assert.AreEqual(45,listExpectedCharges.Count);
			double totalCharge=listExpectedCharges.Select(x=>x.Principal).Sum();
			double totalInterest=listExpectedCharges.Select(x=>x.Interest).Sum();
			Assert.IsTrue(1000<(totalCharge+totalInterest));
		}

		[TestMethod]
		///<summary>A dynamic payment plan with a treatment planned and a completed treatment, with the 'Await Complete' option selected should
		///act the exact same as a dynamic payment plan with the same completed treatment and no treatment planned attached.</summary>
		public void PayPlansDynamic_GetEstimatedSchedule_AwaitCompleteIncludeCompleted() {
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",100,DateTime.Today.AddDays(5)));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),5,12,25,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2));
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			List<PayPlanProductionEntry> listEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(dynamicPayPlan,listPayPlanLinks);

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcsAwait=new List<Procedure>();
			List<Adjustment> listAdjsAwait=new List<Adjustment>();
			listProcsAwait.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,DateTime.Today.AddDays(5)));
			listProcsAwait.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",100,DateTime.Today.AddDays(5)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),5,12,25,listProcsAwait,listAdjsAwait
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2));
			dynamicPayPlanAwait.DynamicPayPlanTPOption=DynamicPayPlanTPOptions.AwaitComplete;
			PayPlans.Update(dynamicPayPlanAwait);
			List<PayPlanLink> listPayPlanLinksAwait=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlanAwait.PayPlanNum});
			List<PayPlanProductionEntry> listEntriesAwait=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinksAwait);
			PayPlanTerms termsAwait=PayPlanEdit.GetPayPlanTerms(dynamicPayPlanAwait,listPayPlanLinksAwait);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(new List<PayPlanCharge>(),terms,fam,listPayPlanLinks,dynamicPayPlan,false,false);
			List<PayPlanCharge> listExpectedChargesAwait=PayPlanEdit.GetListExpectedCharges(new List<PayPlanCharge>(),termsAwait,fam,listPayPlanLinksAwait,dynamicPayPlanAwait,false,false);
			Assert.AreEqual(listExpectedCharges.Count,listExpectedChargesAwait.Count);//amount attached to the payplan should only attach amount needing to still be paid
			double chargesPrinciple=listExpectedCharges.Select(x=>x.Principal).Sum();
			double chargesPrincipleAwait=listExpectedChargesAwait.Select(x=>x.Principal).Sum();
			Assert.AreEqual(chargesPrinciple,chargesPrincipleAwait);
			double chargesInterest=listExpectedCharges.Select(x=>x.Interest).Sum();
			double chargesInterestAwait=listExpectedChargesAwait.Select(x=>x.Interest).Sum();
			Assert.AreEqual(chargesInterest,chargesInterestAwait);
		}

		[TestMethod]
		///<summary>A dynamic payment plan with a treatment planned and a completed treatment, with the 'Await Complete' option selected should
		///act the exact same as a dynamic payment plan with the same completed treatments and no treatment planned attached.</summary>
		public void PayPlansDynamic_GetEstimatedSchedule_AwaitCompleteIncludeCompletedAndSetComplete() {
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",100,DateTime.Today.AddDays(5)));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",50,DateTime.Today.AddDays(5)));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),5,12,25,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2));
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			List<PayPlanProductionEntry> listEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(dynamicPayPlan,listPayPlanLinks);
			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcsAwait=new List<Procedure>();
			List<Adjustment> listAdjsAwait=new List<Adjustment>();
			listProcsAwait.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,DateTime.Today.AddDays(5)));
			listProcsAwait.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",100,DateTime.Today.AddDays(5)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),5,12,25,listProcsAwait,listAdjsAwait
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2));
			dynamicPayPlanAwait.DynamicPayPlanTPOption=DynamicPayPlanTPOptions.AwaitComplete;
			PayPlans.Update(dynamicPayPlanAwait);
			Procedure procOld=listProcsAwait[0].Copy();
			listProcsAwait[0].ProcStatus=ProcStat.C;
			Procedures.Update(listProcsAwait[0],procOld);
			List<PayPlanLink> listPayPlanLinksAwait=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlanAwait.PayPlanNum});
			List<PayPlanProductionEntry> listEntriesAwait=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinksAwait);
			PayPlanTerms termsAwait=PayPlanEdit.GetPayPlanTerms(dynamicPayPlanAwait,listPayPlanLinksAwait);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(new List<PayPlanCharge>(),terms,fam,listPayPlanLinks,dynamicPayPlan,false,false);
			List<PayPlanCharge> listExpectedChargesAwait=PayPlanEdit.GetListExpectedCharges(new List<PayPlanCharge>(),termsAwait,fam,listPayPlanLinksAwait,dynamicPayPlanAwait,false,false);
			Assert.AreEqual(listExpectedCharges.Count,listExpectedChargesAwait.Count);//amount attached to the payplan should only attach amount needing to still be paid
			double chargesPrinciple=listExpectedCharges.Select(x=>x.Principal).Sum();
			double chargesPrincipleAwait=listExpectedChargesAwait.Select(x=>x.Principal).Sum();
			Assert.AreEqual(chargesPrinciple,chargesPrincipleAwait);
			double chargesInterest=listExpectedCharges.Select(x=>x.Interest).Sum();
			double chargesInterestAwait=listExpectedChargesAwait.Select(x=>x.Interest).Sum();
			Assert.AreEqual(chargesInterest,chargesInterestAwait);
		}

		///<summary>Assert that a Treatment Planned with a prepayment will have the prepayment considered on the dynamic payment plan.</summary>
		[TestMethod]
		public void PayPlansDynamic_GetEstimatedSchedule_AwaitCompleteOneTreatmentPlannedHiddenPrepayment() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (TP Await Complete) with three procedures, $139.1, $483, and $174.8. %0 apr.
			 * Make a TP unearned prepeayment of 96.90 for D0150.
			 * Assert no charges will be issued in the current state.
			 * Set D0150 as complete.
			 * Assert that 42.20 is expected to be charged out.
			 * Assert the sum of the splits, and charges on the account sum up to the proc fee of D0150
			 *--------------------------------------------------------------------------*/
			//Test variables
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			long hiddenUnearnedDef=PrefC.GetLong(PrefName.PrepaymentUnearnedType);

			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0150",ProcStat.TP,"",139.1,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",483,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",174.8,DateTime.Today));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(-4),0,0,100,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete,runService:true);

			//Make a payment for the down payment charge.
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,96.90,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			foreach(PaySplit split in results.ListPaySplitsSuggested) {
				split.UnearnedType=hiddenUnearnedDef;
				split.ProcNum=listProcs.First().ProcNum;
			}
			PaySplits.InsertMany(results.ListPaySplitsSuggested);

			//Assert that the DPP wont issue any charges in its current state (all procs TP'd)
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlan,pat,fam);
			List<PayPlanCharge> listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			Assert.AreEqual(0,recalcData.ListPayPlanCharges.Count+listPayPlanCharges.Count);

			//Set D0150 complete
			Procedure oldProc=listProcs.First().Copy();
			listProcs.First().ProcStatus=ProcStat.C;
			Procedures.Update(listProcs.First(),oldProc);

			//Assert that the DPP wont issue any charges in its current state (all procs TP'd)
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlan,pat,fam);
			listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			List<PaySplit> listPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			double totalPaidOut=listPaySplits.Where(x=>x.ProcNum==listProcs.First().ProcNum).Sum(x=>x.SplitAmt);
			double totalToBeCharged=listPayPlanCharges.Where(x=>x.FKey==listProcs.First().ProcNum).Sum(x=>x.Interest+x.Principal);
			Assert.AreEqual(42.2,totalToBeCharged);
			Assert.AreEqual(96.90,totalPaidOut);
			Assert.IsTrue(CompareDouble.IsEqual(listProcs.First().ProcFee,totalToBeCharged+totalPaidOut));
		}

		///<summary>Assert that a treatment planned production will be treated as if it were complete in a DPP with 'treat as complete' mode selected.</summary>
		[TestMethod]
		public void PayPlansDynamic_GetEstimatedSchedule_TreatAsCompleteOneTreatmentPlanned() {
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");
			//create the produciton that will be attached to the payment plan
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",50,DateTime.Today.AddDays(5)));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(1),5,12,25,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2));
			dynamicPayPlan.DynamicPayPlanTPOption=DynamicPayPlanTPOptions.TreatAsComplete;
			PayPlans.Update(dynamicPayPlan);
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForPayPlans(new List<long>{dynamicPayPlan.PayPlanNum});
			List<PayPlanProductionEntry> listEntries=PayPlanProductionEntry.GetProductionForLinks(listPayPlanLinks);
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(dynamicPayPlan,listPayPlanLinks);
			List<PayPlanCharge> listExpectedCharges=PayPlanEdit.GetListExpectedCharges(new List<PayPlanCharge>(),terms,fam,listPayPlanLinks,dynamicPayPlan,false,false);
			Assert.AreEqual(2,listExpectedCharges.Count);//amount attached to the payplan should only attach amount needing to still be paid
		}

		///<summary>Assert that a treatment planned production will be treated as if it were complete in a DPP with 'treat as complete' mode selected.</summary>
		[TestMethod]
		public void PayPlansDynamic_GetEstimatedSchedule_TreatAsCompleteIncludeCompleted() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (TP Await Complete) with three procedures, $139.1, $483, and $174.8. %0 apr.
			 * Make a TP unearned prepeayment of 96.90 for D0150.
			 * Assert no charges will be issued in the current state.
			 * Set D0150 as complete.
			 * Assert that 42.20 is expected to be charged out.
			 * Assert the sum of the splits, and charges on the account sum up to the proc fee of D0150
			 *--------------------------------------------------------------------------*/
			//Test variables
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			long hiddenUnearnedDef=PrefC.GetLong(PrefName.PrepaymentUnearnedType);

			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0150",ProcStat.TP,"",139.1,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",483,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",174.8,DateTime.Today));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddMonths(0),0,0,100,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete,runService:true);

			//Assert that the DPP wont issue any charges in its current state (all procs TP'd)
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlan,pat,fam);
			List<PayPlanCharge> listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			List<PayPlanProductionEntry> payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(recalcData.ListPayPlanLinks);
			double completedAmount=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(dynamicPayPlan,payPlanProductionEntries);
			double chargedAmount=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)+listPayPlanCharges.Sum(x=>x.Interest+x.Principal);
			Assert.AreEqual(10,recalcData.ListPayPlanCharges.Count+listPayPlanCharges.Count);
			Assert.IsTrue(CompareDouble.IsEqual(796.9,chargedAmount));
			Assert.IsTrue(CompareDouble.IsEqual(796.9,completedAmount));

			//Set the first proc complete
			Procedure procOld=listProcs.First().Copy();
			listProcs.First().ProcStatus=ProcStat.C;
			Procedures.Update(listProcs.First(),procOld);
			listProcs.Remove(listProcs.First());

			//Assert that expected charges are now producing charges for the one completed proc.
			listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(recalcData.ListPayPlanLinks);
			completedAmount=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(dynamicPayPlan,payPlanProductionEntries);
			chargedAmount=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)+listPayPlanCharges.Sum(x=>x.Interest+x.Principal);
			Assert.AreEqual(9,listPayPlanCharges.Count);
			Assert.IsTrue(CompareDouble.IsEqual(796.9,chargedAmount));
			Assert.IsTrue(CompareDouble.IsEqual(796.9,completedAmount));

			//Set the second proc complete
			procOld=listProcs.First().Copy();
			listProcs.First().ProcStatus=ProcStat.C;
			Procedures.Update(listProcs.First(),procOld);
			listProcs.Remove(listProcs.First());

			//Assert that expected charges are now producing charges for the one completed proc.
			listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(recalcData.ListPayPlanLinks);
			completedAmount=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(dynamicPayPlan,payPlanProductionEntries);
			chargedAmount=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)+listPayPlanCharges.Sum(x=>x.Interest+x.Principal);
			Assert.AreEqual(9,listPayPlanCharges.Count);
			Assert.IsTrue(CompareDouble.IsEqual(796.9,chargedAmount));
			Assert.IsTrue(CompareDouble.IsEqual(796.9,completedAmount));

			//Set the third proc complete
			procOld=listProcs.First().Copy();
			listProcs.First().ProcStatus=ProcStat.C;
			Procedures.Update(listProcs.First(),procOld);
			listProcs.Remove(listProcs.First());

			//Assert that expected charges are now producing charges for the one completed proc.
			listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			payPlanProductionEntries=PayPlanProductionEntry.GetProductionForLinks(recalcData.ListPayPlanLinks);
			completedAmount=PayPlanProductionEntry.GetDynamicPayPlanCompletedAmount(dynamicPayPlan,payPlanProductionEntries);
			chargedAmount=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)+listPayPlanCharges.Sum(x=>x.Interest+x.Principal);
			Assert.AreEqual(9,listPayPlanCharges.Count);
			Assert.IsTrue(CompareDouble.IsEqual(796.9,chargedAmount));
			Assert.IsTrue(CompareDouble.IsEqual(796.9,completedAmount));
		}

		///<summary>Assert that a Treatment Planned with a prepayment will have the prepayment considered on the dynamic payment plan.</summary>
		[TestMethod]
		public void PayPlansDynamic_GetEstimatedSchedule_TreatAsCompleteOneTreatmentPlannedHiddenPrepayment() {
			/*--------------------------------------------------------------------------
			 * Dynamic Payment Plan (TP Await Complete) with three procedures, $139.1, $483, and $174.8. %0 apr.
			 * Make a TP unearned prepeayment of 96.90 for D0150.
			 * Assert no charges will be issued in the current state.
			 * Set D0150 as complete.
			 * Assert that 42.20 is expected to be charged out.
			 * Assert the sum of the splits, and charges on the account sum up to the proc fee of D0150
			 *--------------------------------------------------------------------------*/
			//Test variables
			PayPlanEdit.PayPlanRecalculationData recalcData=new PayPlanEdit.PayPlanRecalculationData();
			long hiddenUnearnedDef=PrefC.GetLong(PrefName.PrepaymentUnearnedType);

			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0150",ProcStat.TP,"",139.1,DateTime.Today));

			//Make a payment for the first proc.
			Payment payment=PaymentT.MakePaymentNoSplits(pat.PatNum,96.90,payDate:DateTime.Today);
			PaymentEdit.AutoSplit results=PaymentEdit.AutoSplitForPayment(pat.PatNum,payment);
			foreach(PaySplit split in results.ListPaySplitsSuggested) {
				split.UnearnedType=hiddenUnearnedDef;
				split.ProcNum=listProcs.First().ProcNum;
			}
			PaySplits.InsertMany(results.ListPaySplitsSuggested);

			//Keep charting the rest now.
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",483,DateTime.Today));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",174.8,DateTime.Today));
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,0,100,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete,runService:true);

			//Assert that the DPP wont issue any charges in its current state (all procs TP'd)
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlan,pat,fam);
			List<PayPlanCharge> listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			Assert.AreEqual(9,recalcData.ListPayPlanCharges.Count+listPayPlanCharges.Count);

			//Set D0150 complete
			Procedure oldProc=listProcs.First().Copy();
			listProcs.First().ProcStatus=ProcStat.C;
			Procedures.Update(listProcs.First(),oldProc);

			//Assert that the DPP wont issue any charges in its current state (all procs TP'd)
			recalcData=PayPlanT.GetRecalculationData(dynamicPayPlan,pat,fam);
			listPayPlanCharges=PayPlanEdit.GetListExpectedCharges(recalcData.ListPayPlanCharges,recalcData.Terms,fam,recalcData.ListPayPlanLinks,dynamicPayPlan,false);
			List<PaySplit> listPaySplits=PaySplits.GetForPats(new List<long>{ pat.PatNum });
			double totalPaidOut=listPaySplits.Where(x=>x.ProcNum==listProcs.First().ProcNum).Sum(x=>x.SplitAmt);
			double totalToBeCharged=recalcData.ListPayPlanCharges.Sum(x=>x.Interest+x.Principal)+listPayPlanCharges.Sum(x=>x.Interest+x.Principal);
			Assert.AreEqual(700,totalToBeCharged);
			Assert.AreEqual(96.90,totalPaidOut);
			Assert.IsTrue(CompareDouble.IsEqual(listProcs.Sum(x=>x.ProcFee),totalToBeCharged+totalPaidOut));
		}
		#endregion
	}
}
