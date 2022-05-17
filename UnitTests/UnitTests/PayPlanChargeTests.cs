using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestsCore;
using OpenDentBusiness;
using System.Reflection;
using CodeBase;

namespace UnitTests.PayPlanChargeTests {
	[TestClass]
	public class PayPlanChargeTests:TestBase {
		[TestMethod]
		public void PayPlanCharges_UpdateAttachedPayPlanCharges_UpdatingProcStatusTreatAsComplete() {
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0200",ProcStat.C,"",50,DateTime.Today.AddDays(-3)));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",50,DateTime.Today.AddMonths(1)));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(2)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,50,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2),dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete);

			//Verify 1 charge gets issued, and its for todays date.
			List<PayPlanCharge> listChargesInDb=PayPlanCharges.GetForPayPlan(dynamicPayPlanAwait.PayPlanNum);
			Assert.AreEqual(1,listChargesInDb.Count);
			Assert.AreEqual(1,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today).ToList().Count);

			//Set a proc to TP.
			Procedure proc = listProcs.FirstOrDefault(x=>x.ProcStatus==ProcStat.C);
			Procedure procOld= proc.Copy();
			proc.ProcStatus=ProcStat.TP;
			Procedures.Update(proc,procOld);

			//Verify the charge that was issued still has todays date.
			listChargesInDb=PayPlanCharges.GetForPayPlan(dynamicPayPlanAwait.PayPlanNum);
			Assert.AreEqual(1,listChargesInDb.Count);
			Assert.AreEqual(1,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today).ToList().Count);
		}

		[TestMethod]
		public void PayPlanCharges_UpdateAttachedPayPlanCharges_UpdatingProcStatusDoesntChangeDateAwaitComplete() {
			//set up payment plan
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider("LS");

			//create the produciton that will be attached to the payment plan with the await option selected, and treatment planned
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0200",ProcStat.C,"",50,DateTime.Today.AddDays(-3)));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0210",ProcStat.TP,"",50,DateTime.Today.AddMonths(1)));
			listProcs.Add(ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",100,DateTime.Today.AddMonths(2)));
			PayPlan dynamicPayPlanAwait=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today,0,12,50,listProcs,listAdjs
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2),dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.AwaitComplete);

			//Verify 1 charge gets issued, and its for todays date.
			List<PayPlanCharge> listChargesInDb=PayPlanCharges.GetForPayPlan(dynamicPayPlanAwait.PayPlanNum);
			Assert.AreEqual(1,listChargesInDb.Count);
			Assert.AreEqual(1,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today).ToList().Count);

			//Set a proc to TP.
			Procedure proc = listProcs.FirstOrDefault(x=>x.ProcStatus==ProcStat.C);
			Procedure procOld= proc.Copy();
			proc.ProcStatus=ProcStat.TP;
			Procedures.Update(proc,procOld);

			//Verify the charge that was issued still has todays date.
			listChargesInDb=PayPlanCharges.GetForPayPlan(dynamicPayPlanAwait.PayPlanNum);
			Assert.AreEqual(1,listChargesInDb.Count);
			Assert.AreEqual(1,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today).ToList().Count);
		}

		[TestMethod]
		public void PayPlanCharges_UpdateAttachedPayPlanCharges_UpdatingProcStatusDoesntChangeDate() {
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
				,PayPlanFrequency.Monthly,dateInterestStart:DateTime.Today.AddMonths(2),dynamicPayPlanTPOptions:DynamicPayPlanTPOptions.TreatAsComplete);

			//Verify 1 charge gets issued, and its for todays date.
			List<PayPlanCharge> listChargesInDb=PayPlanCharges.GetForPayPlan(dynamicPayPlanAwait.PayPlanNum);
			Assert.AreEqual(1,listChargesInDb.Count);
			Assert.AreEqual(1,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today).ToList().Count);

			//Set a proc to TP.
			Procedure proc = listProcs.FirstOrDefault(x=>x.ProcStatus==ProcStat.C);
			Procedure procOld= proc.Copy();
			proc.ProcStatus=ProcStat.TP;
			Procedures.Update(proc,procOld);

			//Verify the charge that was issued still has todays date.
			listChargesInDb=PayPlanCharges.GetForPayPlan(dynamicPayPlanAwait.PayPlanNum);
			Assert.AreEqual(1,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today).ToList().Count);
			Assert.AreEqual(1,listChargesInDb.Count);

			//Issue another period of charges
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForPayPlans(new List<long>{ dynamicPayPlanAwait.PayPlanNum });
			PayPlanTerms terms=PayPlanEdit.GetPayPlanTerms(dynamicPayPlanAwait,listPayPlanLinks);
			List<PayPlanCharge> listChargesThisPeriod=PayPlanEdit.GetListExpectedCharges(listChargesInDb,terms,Patients.GetFamily(pat.PatNum),listPayPlanLinks,dynamicPayPlanAwait,true);
			PayPlanCharges.InsertMany(listChargesThisPeriod);

			//Verify there is a charge for today, and next month.
			listChargesInDb=PayPlanCharges.GetForPayPlan(dynamicPayPlanAwait.PayPlanNum);
			Assert.AreEqual(3,listChargesInDb.Count);
			Assert.AreEqual(1,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today).ToList().Count);
			Assert.AreEqual(2,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today.AddMonths(1)).ToList().Count);
			
			//Update all procs to complete
			for(int i = 0;i<listProcs.Count;i++) {
				procOld=listProcs[i].Copy();
				listProcs[i].ProcStatus=ProcStat.C;
				Procedures.Update(listProcs[i],procOld);
			}
			
			//Verify the charge dates are the same.
			Assert.AreEqual(1,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today).ToList().Count);
			Assert.AreEqual(2,listChargesInDb.Where(x=>x.ChargeDate==DateTime.Today.AddMonths(1)).ToList().Count);
		}
	}
}
