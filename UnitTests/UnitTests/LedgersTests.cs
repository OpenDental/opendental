using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using UnitTestsCore;
using System.Linq;
using CodeBase;

namespace UnitTests.Ledgers_Tests {
	[TestClass]
	public class LedgersTests:TestBase {
		///<summary>Tests that ComputeAgingForPaysplitsAllocatedToDiffPats computes aging for other patients.</summary>
		[TestMethod]
		public void LedgersTests_ComputeAgingForPaysplitsAllocatedToDiffPats() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patDad=PatientT.CreatePatient(fName:"Father",suffix:suffix);//Father-guarantor
			Patient patChild=PatientT.CreatePatient(fName:"Child",suffix:suffix);//Child
			PatientT.SetGuarantor(patChild,patDad.PatNum);
			Procedure proc1=ProcedureT.CreateProcedure(patChild,"D1110",ProcStat.C,"",50,DateTime.Today.AddDays(-45));
			Procedure proc2=ProcedureT.CreateProcedure(patChild,"D0120",ProcStat.C,"",40,DateTime.Today.AddDays(-45));
			Ledgers.ComputeAging(patDad.PatNum,DateTime.Today);
			patDad=Patients.GetPat(patDad.PatNum);
			Assert.AreEqual(90,patDad.Bal_31_60);
			Patient patMom=PatientT.CreatePatient(fName:"Mom",suffix:suffix);//Mom is not associated to the father and childs account
			long patNum=patMom.PatNum;
			//complete procedures for patMom.
			Procedure proc3=ProcedureT.CreateProcedure(patMom,"D1110",ProcStat.C,"",50,DateTime.Today.AddDays(-1));
			Procedure proc4=ProcedureT.CreateProcedure(patMom,"D0120",ProcStat.C,"",40,DateTime.Today.AddDays(-1));
			//Compute aging. Check that the aging is correct before we continue
			Ledgers.ComputeAging(patNum,DateTime.Today);
			patMom=Patients.GetPat(patNum);
			Assert.AreEqual(90,patMom.Bal_0_30);
			//patMom will now make a payment for her procedures and patChilds
			Payment pay=PaymentT.MakePaymentNoSplits(patNum,100,clinicNum:proc3.ClinicNum);
			List<PaySplit> listPaySplits=new List<PaySplit>();
			listPaySplits.Add(PaySplitT.CreateSplit(proc3.ClinicNum,patNum,pay.PayNum,0,DateTime.Today,proc3.ProcNum,proc3.ProvNum,50,0));
			//Create a paysplit for patChild. PatChild is from a different family.
			listPaySplits.Add(PaySplitT.CreateSplit(proc1.ClinicNum,patChild.PatNum,pay.PayNum,0,DateTime.Today,proc1.ProcNum,proc1.ProvNum,50,0));
			//This should compute the aging for the 			
			string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(patNum,listPaySplits);
			Assert.IsTrue(string.IsNullOrEmpty(strErrorMsg));
			patDad=Patients.GetPat(patDad.PatNum);
			Assert.AreEqual(40,patDad.Bal_31_60);
			//Compute patMom aging to verify that it is correct. 
			Ledgers.ComputeAging(patNum,DateTime.Today);
			patMom=Patients.GetPat(patNum);
			Assert.AreEqual(40,patMom.Bal_0_30);
		}

		private void AssertMoney(double expectedAmt,double actualAmt) {
			Assert.AreEqual(Math.Round(expectedAmt,2),Math.Round(actualAmt,2));
		}

		private void PatAssertBalances(long patNum,double bal_0_30,double bal_31_60,double bal_61_90,double balOver90,double payPlanDue) {
			Patient pat=Patients.GetPat(patNum);//Refresh the patient from the database to get the calculated balances.
			AssertMoney(bal_0_30,pat.Bal_0_30);
			AssertMoney(bal_31_60,pat.Bal_31_60);
			AssertMoney(bal_61_90,pat.Bal_61_90);
			AssertMoney(balOver90,pat.BalOver90);
			AssertMoney(bal_0_30+bal_31_60+bal_61_90+balOver90,pat.BalTotal);
			AssertMoney(payPlanDue,pat.PayPlanDue);
		}

		private void CheckAgingProcLifo(long patNum,double bal_0_30,double bal_31_60,double bal_61_90,double balOver90,double payPlanDue,YN prefVal,
			DateTime asOfDate=default)
		{
			if(asOfDate==default) {
				asOfDate=DateTime.Today;
			}
			int agingProcLifoPrev=PrefC.GetInt(PrefName.AgingProcLifo);
			try {
				PrefT.UpdateInt(PrefName.AgingProcLifo,(int)prefVal);
				Ledgers.ComputeAging(patNum,asOfDate);
				PatAssertBalances(patNum,bal_0_30,bal_31_60,bal_61_90,balOver90,payPlanDue);
			}
			finally {
				PrefT.UpdateInt(PrefName.AgingProcLifo,agingProcLifoPrev);
			}
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_Case1() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_Case1",suffix:suffix);
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Adjustment adj55=AdjustmentT.MakeAdjustment(pat.PatNum,10,DateTime.Today.AddDays(-55),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj2=AdjustmentT.MakeAdjustment(pat.PatNum,-8,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			CheckAgingProcLifo(pat.PatNum,0,10,92,0,0,YN.Yes);
			CheckAgingProcLifo(pat.PatNum,0,10,92,0,0,YN.No);
			CheckAgingProcLifo(pat.PatNum,0,10,92,0,0,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_Case2() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_Case2",suffix:suffix);
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Procedure proc55=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",150,DateTime.Today.AddDays(-55));
			Adjustment adj2=AdjustmentT.MakeAdjustment(pat.PatNum,-100,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			CheckAgingProcLifo(pat.PatNum,0,150,0,0,0,YN.Yes);
			CheckAgingProcLifo(pat.PatNum,0,150,0,0,0,YN.No);
			CheckAgingProcLifo(pat.PatNum,0,150,0,0,0,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_Case3() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_Case3",suffix:suffix);
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Procedure proc55=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",150,DateTime.Today.AddDays(-55));
			Adjustment adj2=AdjustmentT.MakeAdjustment(pat.PatNum,-100,DateTime.Today.AddDays(-2));
			CheckAgingProcLifo(pat.PatNum,0,150,0,0,0,YN.Yes);
			CheckAgingProcLifo(pat.PatNum,0,150,0,0,0,YN.No);
			CheckAgingProcLifo(pat.PatNum,0,150,0,0,0,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_Case4() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_Case4",suffix:suffix);
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Procedure proc55=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",150,DateTime.Today.AddDays(-55));
			Adjustment adj2=AdjustmentT.MakeAdjustment(pat.PatNum,-100,DateTime.Today.AddDays(-2),proc55.ProcDate,proc55.ProcNum);
			CheckAgingProcLifo(pat.PatNum,0,50,100,0,0,YN.Yes);
			CheckAgingProcLifo(pat.PatNum,0,150,0,0,0,YN.No);
			CheckAgingProcLifo(pat.PatNum,0,150,0,0,0,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_Case5() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_Case5",suffix:suffix);
			Procedure proc95=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",1000,DateTime.Today.AddDays(-95));
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Adjustment adj40=AdjustmentT.MakeAdjustment(pat.PatNum,10,DateTime.Today.AddDays(-40),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj2=AdjustmentT.MakeAdjustment(pat.PatNum,-800,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			CheckAgingProcLifo(pat.PatNum,0,10,0,300,0,YN.Yes);
			CheckAgingProcLifo(pat.PatNum,0,10,100,200,0,YN.No);
			CheckAgingProcLifo(pat.PatNum,0,10,100,200,0,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_Case5B() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_Case5B",suffix:suffix);
			Procedure proc95=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",1000,DateTime.Today.AddDays(-95));
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Adjustment adj40_1=AdjustmentT.MakeAdjustment(pat.PatNum,4,DateTime.Today.AddDays(-40),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj40_2=AdjustmentT.MakeAdjustment(pat.PatNum,6,DateTime.Today.AddDays(-40),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj2_1=AdjustmentT.MakeAdjustment(pat.PatNum,-200,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj2_2=AdjustmentT.MakeAdjustment(pat.PatNum,-250,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj2_3=AdjustmentT.MakeAdjustment(pat.PatNum,-350,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			CheckAgingProcLifo(pat.PatNum,0,10,0,300,0,YN.Yes);
			CheckAgingProcLifo(pat.PatNum,0,10,100,200,0,YN.No);
			CheckAgingProcLifo(pat.PatNum,0,10,100,200,0,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_Case5C() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_Case5C",suffix:suffix);
			Procedure proc95=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",1000,DateTime.Today.AddDays(-95));
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Adjustment adj40=AdjustmentT.MakeAdjustment(pat.PatNum,-800,DateTime.Today.AddDays(-40),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj2=AdjustmentT.MakeAdjustment(pat.PatNum,10,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			CheckAgingProcLifo(pat.PatNum,10,0,0,300,0,YN.Yes);
			CheckAgingProcLifo(pat.PatNum,10,0,100,200,0,YN.No);
			CheckAgingProcLifo(pat.PatNum,10,0,100,200,0,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_Case6() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_Case6",suffix:suffix);
			Procedure proc95=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",1000,DateTime.Today.AddDays(-95));
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Adjustment adj2_1=AdjustmentT.MakeAdjustment(pat.PatNum,10,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj2_2=AdjustmentT.MakeAdjustment(pat.PatNum,-30,DateTime.Today.AddDays(-2),proc85.ProcDate,proc85.ProcNum);
			CheckAgingProcLifo(pat.PatNum,0,0,80,1000,0,YN.Yes);
			CheckAgingProcLifo(pat.PatNum,10,0,100,970,0,YN.No);
			CheckAgingProcLifo(pat.PatNum,10,0,100,970,0,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_DateLastPay() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_DateLastPay",suffix:suffix);
			Procedure proc95=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",1000,DateTime.Today.AddDays(-95));
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			Payment pay50=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today.AddDays(-50));
			Adjustment adj40_1=AdjustmentT.MakeAdjustment(pat.PatNum,4,DateTime.Today.AddDays(-40),proc85.ProcDate,proc85.ProcNum);
			Adjustment adj40_2=AdjustmentT.MakeAdjustment(pat.PatNum,6,DateTime.Today.AddDays(-40),proc85.ProcDate,proc85.ProcNum);
			Payment pay2=PaymentT.MakePayment(pat.PatNum,50,DateTime.Today.AddDays(-2));
			int agingProcLifoPrev=PrefC.GetInt(PrefName.AgingProcLifo);
			try {
				PrefT.UpdateInt(PrefName.AgingProcLifo,(int)YN.No);
				Dictionary <long,DataRow> dictAging=Ledgers.GetAgingGuarTransTable(DateTime.Today,new List <long> { pat.Guarantor },hasDateLastPay:true);
				Assert.AreEqual(PIn.Date(dictAging[pat.Guarantor]["DateLastPay"].ToString()),pay2.PayDate);
				PrefT.UpdateInt(PrefName.AgingProcLifo,(int)YN.Yes);
				dictAging=Ledgers.GetAgingGuarTransTable(DateTime.Today,new List <long> { pat.Guarantor },hasDateLastPay:true);
				Assert.AreEqual(PIn.Date(dictAging[pat.Guarantor]["DateLastPay"].ToString()),pay2.PayDate);
			}
			finally {
				PrefT.UpdateInt(PrefName.AgingProcLifo,agingProcLifoPrev);
			}
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_InsWoEst_And_InsPayEst() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_InsEst",suffix:suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,"AgingInsEst");
			Procedure proc95=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",1000,DateTime.Today.AddDays(-95));
			ClaimProc cp95=new ClaimProc();
			ClaimProcs.CreateEst(cp95,proc95,insInfo.PriInsPlan,insInfo.PriInsSub);
			cp95.Status=ClaimProcStatus.NotReceived;
			cp95.InsEstTotal=800;
			cp95.InsPayEst=800;
			cp95.WriteOffEst=200;
			cp95.WriteOff=200;
			ClaimProcs.Update(cp95);
			Procedure proc85=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-85));
			ClaimProc cp85=new ClaimProc();
			ClaimProcs.CreateEst(cp85,proc85,insInfo.PriInsPlan,insInfo.PriInsSub);
			cp85.Status=ClaimProcStatus.NotReceived;
			cp85.InsEstTotal=60;
			cp85.InsPayEst=60;
			cp85.WriteOffEst=40;
			cp85.WriteOff=40;
			ClaimProcs.Update(cp85);
			int agingProcLifoPrev=PrefC.GetInt(PrefName.AgingProcLifo);
			try {
				PrefT.UpdateInt(PrefName.AgingProcLifo,(int)YN.No);
				Dictionary <long,DataRow> dictAging=Ledgers.GetAgingGuarTransTable(DateTime.Today,new List <long> { pat.Guarantor });
				Assert.AreEqual(PIn.Double(dictAging[pat.Guarantor]["InsPayEst"].ToString()),860);
				Assert.AreEqual(PIn.Double(dictAging[pat.Guarantor]["InsWoEst"].ToString()),240);
				PrefT.UpdateInt(PrefName.AgingProcLifo,(int)YN.Yes);
				dictAging=Ledgers.GetAgingGuarTransTable(DateTime.Today,new List <long> { pat.Guarantor });
				Assert.AreEqual(PIn.Double(dictAging[pat.Guarantor]["InsPayEst"].ToString()),860);
				Assert.AreEqual(PIn.Double(dictAging[pat.Guarantor]["InsWoEst"].ToString()),240);
			}
			finally {
				PrefT.UpdateInt(PrefName.AgingProcLifo,agingProcLifoPrev);
			}
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_PayPlan1() {
			DateTime asOfDate=new DateTime(2019,8,9);
			Patient pat=PatientT.CreatePatient(fName:"Schedule Based",lName:"UDP Ortho");
			Def defNeg=DefT.CreateDefinition(DefCat.AdjTypes,"Ortho Revenue","-");
			Def defPos=DefT.CreateDefinition(DefCat.AdjTypes,"Ortho Revenue","+");
			Def defPay=DefT.CreateDefinition(DefCat.PaymentTypes,"Check");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D8090",ProcStat.C,"",4000,asOfDate.AddMonths(-6).AddDays(-1));
			PayPlan payPlan=PayPlanT.CreatePayPlan(pat.PatNum,4000,166.67,asOfDate.AddMonths(-6).AddDays(-1),proc.ProvNum);
			Adjustment adj6=AdjustmentT.MakeAdjustment(pat.PatNum,-2800,asOfDate.AddMonths(-6).AddDays(-1),proc.ProcDate,proc.ProcNum,provNum:proc.ProvNum,adjType:defNeg.DefNum);
			Payment pay6=PaymentT.MakePayment(pat.PatNum,166.67,asOfDate.AddMonths(-6).AddDays(-1),payPlanNum:payPlan.PayPlanNum,provNum:proc.ProvNum,procNum:proc.ProcNum,payType:defPay.DefNum);
			PayPlanCharge ppc6=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-6).AddDays(-1),1200,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum);
			Adjustment adj5=AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-5).AddDays(-1),proc.ProcDate,proc.ProcNum,provNum:proc.ProvNum,adjType:defPos.DefNum);
			Payment pay5=PaymentT.MakePayment(pat.PatNum,166.67,asOfDate.AddMonths(-5).AddDays(-1),payPlanNum:payPlan.PayPlanNum,provNum:proc.ProvNum,procNum:proc.ProcNum,payType:defPay.DefNum);
			PayPlanCharge ppc5=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-5).AddDays(-1),121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum);
			Adjustment adj4=AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-4).AddDays(-1),proc.ProcDate,proc.ProcNum,provNum:proc.ProvNum,adjType:defPos.DefNum);
			PayPlanCharge ppc4=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-4).AddDays(-1),121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum);
			Adjustment adj3=AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-3).AddDays(-1),proc.ProcDate,proc.ProcNum,provNum:proc.ProvNum,adjType:defPos.DefNum);
			PayPlanCharge ppc3=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-3).AddDays(-1),121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum);
			Adjustment adj2=AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-2).AddDays(-1),proc.ProcDate,proc.ProcNum,provNum:proc.ProvNum,adjType:defPos.DefNum);
			PayPlanCharge ppc2=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-2).AddDays(-1),121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum);
			Adjustment adj1=AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-1).AddDays(-1),proc.ProcDate,proc.ProcNum,provNum:proc.ProvNum,adjType:defPos.DefNum);
			PayPlanCharge ppc1=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-1).AddDays(-1),121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum);
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,166.67,166.67,166.67,333.34,833.35,YN.Yes,asOfDate:asOfDate);
				CheckAgingProcLifo(pat.PatNum,166.67,288.41,288.41,89.86,833.35,YN.No,asOfDate:asOfDate);
				CheckAgingProcLifo(pat.PatNum,166.67,288.41,288.41,89.86,833.35,YN.Unknown,asOfDate:asOfDate);//Unset will behave the same as Off for now, until we change default behavior in future.
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_PayPlan2() {
			DateTime asOfDate=new DateTime(2019,8,9);
			Patient pat=PatientT.CreatePatient(fName:"Visit Based",lName:"UDP Ortho");
			Def defPay=DefT.CreateDefinition(DefCat.PaymentTypes,"Check");
			Procedure proc6=ProcedureT.CreateProcedure(pat,"D8090",ProcStat.C,"",1200,asOfDate.AddMonths(-6).AddDays(-1));
			PayPlan payPlan=PayPlanT.CreatePayPlan(pat.PatNum,4000,166.67,asOfDate.AddMonths(-6).AddDays(-1),proc6.ProvNum);
			Payment pay6=PaymentT.MakePayment(pat.PatNum,166.67,asOfDate.AddMonths(-6).AddDays(-1),payPlanNum:payPlan.PayPlanNum,provNum:proc6.ProvNum,procNum:proc6.ProcNum,payType:defPay.DefNum);
			PayPlanCharge ppc6=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-6).AddDays(-1),1200,0,"D8090:  CompOrthoAdlt",proc6.ProvNum,0,PayPlanChargeType.Credit,proc6.ProcNum);
			Procedure proc5=ProcedureT.CreateProcedure(pat,"D8670",ProcStat.C,"",121.74,asOfDate.AddMonths(-5).AddDays(-1));
			Payment pay5=PaymentT.MakePayment(pat.PatNum,166.67,asOfDate.AddMonths(-5).AddDays(-1),payPlanNum:payPlan.PayPlanNum,provNum:proc5.ProvNum,procNum:proc5.ProcNum,payType:defPay.DefNum);
			PayPlanCharge ppc5=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-5).AddDays(-1),121.74,0,"D8670:  OrthoAdj",proc5.ProvNum,0,PayPlanChargeType.Credit,proc5.ProcNum);
			Procedure proc4=ProcedureT.CreateProcedure(pat,"D8670",ProcStat.C,"",121.74,asOfDate.AddMonths(-4).AddDays(-1));
			PayPlanCharge ppc4=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-4).AddDays(-1),121.74,0,"D8670:  OrthoAdj",proc4.ProvNum,0,PayPlanChargeType.Credit,proc4.ProcNum);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D8670",ProcStat.C,"",121.74,asOfDate.AddMonths(-3).AddDays(-1));
			PayPlanCharge ppc3=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-3).AddDays(-1),121.74,0,"D8670:  OrthoAdj",proc3.ProvNum,0,PayPlanChargeType.Credit,proc3.ProcNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D8670",ProcStat.C,"",121.74,asOfDate.AddMonths(-2).AddDays(-1));
			PayPlanCharge ppc2=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-2).AddDays(-1),121.74,0,"D8670:  OrthoAdj",proc2.ProvNum,0,PayPlanChargeType.Credit,proc2.ProcNum);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D8670",ProcStat.C,"",121.74,asOfDate.AddMonths(-1).AddDays(-1));
			PayPlanCharge ppc1=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-1).AddDays(-1),121.74,0,"D8670:  OrthoAdj",proc1.ProvNum,0,PayPlanChargeType.Credit,proc1.ProcNum);
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,166.67,166.67,166.67,333.34,833.35,YN.Yes,asOfDate:asOfDate);
				CheckAgingProcLifo(pat.PatNum,166.67,288.41,288.41,89.86,833.35,YN.No,asOfDate:asOfDate);
				CheckAgingProcLifo(pat.PatNum,166.67,288.41,288.41,89.86,833.35,YN.Unknown,asOfDate:asOfDate);//Unset will behave the same as Off for now, until we change default behavior in future.
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		///<summary>This unit test was created to show what happens when the user attaches a greater amount of credit to a procedure than the procedure
		///fee.  For now, the excess credit is applied to the aging bucket that the procedure is in, and if the aging bucket is completely paid, then the
		///excess credit for the bucket is treated as a generic credit which is applied to the oldest balance.</summary>
		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_PayPlan3() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlan3",suffix:suffix);
			Procedure proc95=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",200,DateTime.Today.AddDays(-95));
			Procedure proc35=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today.AddDays(-35));
			PayPlan payPlan=PayPlanT.CreatePayPlan(pat.PatNum,100,50,DateTime.Today.AddDays(-35),proc35.ProvNum);
			Adjustment adj5=AdjustmentT.MakeAdjustment(pat.PatNum,-5.50,DateTime.Today.AddDays(-5),proc35.ProcDate,proc35.ProcNum);
			PayPlanCharge ppc5=PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,DateTime.Today.AddDays(-5),100,0,chargeType:PayPlanChargeType.Credit,procNum:proc35.ProcNum);
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,50,44.50,0,200,100,YN.Yes);
				CheckAgingProcLifo(pat.PatNum,50,150,0,94.50,100,YN.No);
				CheckAgingProcLifo(pat.PatNum,50,150,0,94.50,100,YN.Unknown);//Unset will behave the same as Off for now, until we change default behavior in future.
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		[TestMethod]
		public void LedgersTests_ComputeAgingProcLifo_LargeDb() {
			DateTime asOfDate=new DateTime(2019,8,9);
			Def defNeg=DefT.CreateDefinition(DefCat.AdjTypes,"Ortho Revenue","-");
			Def defPos=DefT.CreateDefinition(DefCat.AdjTypes,"Ortho Revenue","+");
			Def defPay=DefT.CreateDefinition(DefCat.PaymentTypes,"Check");
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			int countPats=1000;
			List <Patient> listPats=new List<Patient>();
			for(int i=0;i<countPats;i++) {
				Patient pat=PatientT.CreatePatient(fName:"Schedule Based",lName:"UDP Ortho LargeDb",doInsert:false);
				listPats.Add(pat);
			}
			PatientT.InsertMany(listPats);
			listPats=Patients.GetAllPatients();
			List<long> listPatNums=listPats.Select(x => x.PatNum).ToList();
			PatientT.SetGuarantorToSelf(listPatNums);
			listPats.ForEach(x => x.Guarantor=x.PatNum);
			List<Procedure> listProcs=new List<Procedure>();
			List<PayPlan> listPayPlans=new List<PayPlan>();
			List<PayPlanCharge> listPayPlanCharges=new List<PayPlanCharge>();
			foreach(Patient pat in listPats) {
				Procedure proc=ProcedureT.CreateProcedure(pat,"D8090",ProcStat.C,"",4000,asOfDate.AddMonths(-6).AddDays(-1),doInsert:false);
				listProcs.Add(proc);
				listPayPlans.Add(PayPlanT.CreatePayPlan(pat.PatNum,4000,166.67,asOfDate.AddMonths(-6).AddDays(-1),proc.ProvNum,doInsert:false,
					listPayPlanCharges));
			}
			ProcedureT.InsertMany(listProcs);
			List<Procedure> listProcsDb=Procedures.GetCompAndTpForPats(listPatNums);
			foreach(Procedure proc in listProcsDb) {
				listProcs.Where(x => x.PatNum==proc.PatNum).ForEach(x => x.ProcNum=proc.ProcNum);
			}
			PayPlans.InsertMany(listPayPlans);
			List<PayPlan> listPayPlansDb=PayPlans.GetForPats(listPatNums,-1);
			foreach(PayPlan payPlan in listPayPlansDb) {
				listPayPlans.Where(x => x.PatNum==payPlan.PatNum).ForEach(x => x.PayPlanNum=payPlan.PayPlanNum);
				listPayPlanCharges.Where(x => x.PatNum==payPlan.PatNum).ForEach(x => x.PayPlanNum=payPlan.PayPlanNum);
			}
			PayPlanCharges.InsertMany(listPayPlanCharges);
			List<Adjustment> listAdjustments=new List<Adjustment>();
			List<Payment> listPayments=new List<Payment>();
			List<PaySplit> listSplits=new List<PaySplit>();
			listPayPlanCharges.Clear();
			for(int i=0;i<countPats;i++) {
				Patient pat=listPats[i];
				Procedure proc=listProcs[i];
				PayPlan payPlan=listPayPlans[i];
				listAdjustments.Add(AdjustmentT.MakeAdjustment(pat.PatNum,-2800,asOfDate.AddMonths(-6).AddDays(-1),proc.ProcDate,proc.ProcNum,
					provNum:proc.ProvNum,adjType:defNeg.DefNum,doInsert:false));
				listPayments.Add(PaymentT.MakePayment(pat.PatNum,166.67,asOfDate.AddMonths(-6).AddDays(-1),payPlanNum:payPlan.PayPlanNum,
					provNum:proc.ProvNum,procNum:proc.ProcNum,payType:defPay.DefNum,doInsert:false,listSplits:listSplits));
				listPayPlanCharges.Add(PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-6).AddDays(-1),
					1200,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum,doInsert:false));
				listAdjustments.Add(AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-5).AddDays(-1),proc.ProcDate,proc.ProcNum,
					provNum:proc.ProvNum,adjType:defPos.DefNum,doInsert:false));
				listPayments.Add(PaymentT.MakePayment(pat.PatNum,166.67,asOfDate.AddMonths(-5).AddDays(-1),payPlanNum:payPlan.PayPlanNum,
					provNum:proc.ProvNum,procNum:proc.ProcNum,payType:defPay.DefNum,doInsert:false,listSplits:listSplits));
				listPayPlanCharges.Add(PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-5).AddDays(-1),
					121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum,doInsert:false));
				listAdjustments.Add(AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-4).AddDays(-1),proc.ProcDate,proc.ProcNum,
					provNum:proc.ProvNum,adjType:defPos.DefNum,doInsert:false));
				listPayPlanCharges.Add(PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-4).AddDays(-1),
					121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum,doInsert:false));
				listAdjustments.Add(AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-3).AddDays(-1),proc.ProcDate,proc.ProcNum,
					provNum:proc.ProvNum,adjType:defPos.DefNum,doInsert:false));
				listPayPlanCharges.Add(PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-3).AddDays(-1),
					121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum,doInsert:false));
				listAdjustments.Add(AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-2).AddDays(-1),proc.ProcDate,proc.ProcNum,
					provNum:proc.ProvNum,adjType:defPos.DefNum,doInsert:false));
				listPayPlanCharges.Add(PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-2).AddDays(-1),
					121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum,doInsert:false));
				listAdjustments.Add(AdjustmentT.MakeAdjustment(pat.PatNum,121.74,asOfDate.AddMonths(-1).AddDays(-1),proc.ProcDate,proc.ProcNum,
					provNum:proc.ProvNum,adjType:defPos.DefNum,doInsert:false));
				listPayPlanCharges.Add(PayPlanChargeT.CreateOne(payPlan.PayPlanNum,pat.Guarantor,pat.PatNum,asOfDate.AddMonths(-1).AddDays(-1),
					121.74,0,"D8090:  CompOrthoAdlt",proc.ProvNum,0,PayPlanChargeType.Credit,proc.ProcNum,doInsert:false));
			}
			AdjustmentT.InsertMany(listAdjustments);
			Payments.InsertMany(listPayments);
			PaySplits.InsertMany(listSplits);
			PayPlanCharges.InsertMany(listPayPlanCharges);
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			int agingProcLifoPrev=PrefC.GetInt(PrefName.AgingProcLifo);
			Stopwatch swOn=new Stopwatch();
			Stopwatch swOff=new Stopwatch();
			Stopwatch swUnset=new Stopwatch();
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				swOn.Start();
				PrefT.UpdateInt(PrefName.AgingProcLifo,(int)YN.Yes);
				Ledgers.ComputeAging(0,asOfDate);//Compute aging for all patients.
				foreach(Patient pat in listPats) {
					PatAssertBalances(pat.PatNum,166.67,166.67,166.67,333.34,833.35);
				}
				swOn.Stop();
				swOff.Start();
				PrefT.UpdateInt(PrefName.AgingProcLifo,(int)YN.No);
				Ledgers.ComputeAging(0,asOfDate);//Compute aging for all patients.
				foreach(Patient pat in listPats) {
					PatAssertBalances(pat.PatNum,166.67,288.41,288.41,89.86,833.35);
				}
				swOff.Stop();
				swUnset.Start();
				PrefT.UpdateInt(PrefName.AgingProcLifo,(int)YN.Unknown);
				Ledgers.ComputeAging(0,asOfDate);//Compute aging for all patients.
				foreach(Patient pat in listPats) {
					PatAssertBalances(pat.PatNum,166.67,288.41,288.41,89.86,833.35);
				}
				swUnset.Stop();
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
				PrefT.UpdateInt(PrefName.AgingProcLifo,agingProcLifoPrev);
			}
			//Fails if swOn run time is at least 10 times as large as the average of the runtimes for swUnset and swOff.
			Assert.IsTrue(swOn.Elapsed.TotalMilliseconds < (swUnset.Elapsed.TotalMilliseconds+swOff.Elapsed.TotalMilliseconds)*5,
				"Pref On: "+swOn.Elapsed+"\r\n"
				+"Pref Off: "+swOff.Elapsed+"\r\n"
				+"Pref Unset: "+swUnset.Elapsed);
		}

		[TestMethod]
		public void LedgersTests_ComputeAging_PayPlanDynamicSimpleAdjustment() {
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			long provNum=ProviderT.CreateProvider("Aging_PayPlanDynamic");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlanDynamic",suffix:suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure procUnattached=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",35,DateTime.Today.AddDays(-91),provNum:provNum);
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,45,DateTime.Today.AddDays(-61),provNum:provNum);
			listAdjs.Add(adj);
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddDays(-1),0,0,40,listProcs,listAdjs);
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(payPlan.PayPlanNum);
			Assert.AreEqual(40,listChargesDb.Sum(x => x.Principal));
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,40,0,35,0,40,YN.Yes);//new
				CheckAgingProcLifo(pat.PatNum,40,0,35,0,40,YN.No);//old
				CheckAgingProcLifo(pat.PatNum,40,0,35,0,40,YN.Unknown);
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		[TestMethod]
		public void LedgersTests_ComputeAging_PayPlanDynamic() {
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			long provNum=ProviderT.CreateProvider("Aging_PayPlanDynamic");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlanDynamic",suffix:suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",45,DateTime.Today.AddDays(-61),provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",55,DateTime.Today.AddDays(-32),provNum:provNum);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",65,DateTime.Today.AddDays(-15),provNum:provNum);
			Adjustment adjProc2=AdjustmentT.MakeAdjustment(pat.PatNum,10,proc2.ProcDate,proc2.ProcDate,proc2.ProcNum,provNum);
			Adjustment adjProc3=AdjustmentT.MakeAdjustment(pat.PatNum,20,proc3.ProcDate,proc3.ProcDate,proc3.ProcNum,provNum);
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,30,DateTime.Today.AddDays(-5),provNum:provNum);
			listProcs.AddRange(new List<Procedure> {proc1,proc2,proc3});
			listAdjs.AddRange(new List<Adjustment> {adj});
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddDays(-1),0,0,40,listProcs,listAdjs);
			//PayPlan payplan=PayPlanT.CreatePayPlanWithCredits(pat.PatNum,40,DateTime.Today,provNum:provNum,listProcs,195,pat.PatNum);
			//make two non payplan productions to put on the account
			Procedure procUnattached=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",35,DateTime.Today.AddDays(-91),provNum:provNum);
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(payPlan.PayPlanNum);
			Assert.AreEqual(40,listChargesDb.Sum(x => x.Principal));
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,70,0,0,5,40,YN.Yes);//new
				CheckAgingProcLifo(pat.PatNum,75,0,0,0,40,YN.No);//old
				CheckAgingProcLifo(pat.PatNum,75,0,0,0,40,YN.Unknown);
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		[TestMethod]
		public void LedgersTests_ComputeAging_PayPlanDynamicCredits() {
			//first need to create an unrelated pat that also has payment plan data to conflict with ours. This test is specifically to guard against a bug
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			long provNum=ProviderT.CreateProvider("Aging_PayPlanDynamicCredits");
			Patient unrelatedPat=PatientT.CreatePatient("unrelatedPatPayPlanDyn");
			Procedure procAttach=ProcedureT.CreateProcedure(unrelatedPat,"D0220",ProcStat.C,"",200,provNum:provNum);
			listProcs.Add(procAttach);
			listAdjs.Add(AdjustmentT.MakeAdjustment(unrelatedPat.PatNum,15,DateTime.Today.AddDays(-2),provNum:provNum));
			listAdjs.Add(AdjustmentT.MakeAdjustment(unrelatedPat.PatNum,5,DateTime.Today,DateTime.Today.AddDays(-2),procAttach.ProcNum,provNum));
			PayPlanT.CreateDynamicPaymentPlan(unrelatedPat.PatNum,unrelatedPat.PatNum,DateTime.Today.AddDays(1),0,0,20,listProcs,listAdjs);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlanDynamic",suffix:suffix);
			listProcs.Clear();
			listAdjs.Clear();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",65,DateTime.Today.AddDays(-1),provNum:provNum);
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,20,DateTime.Today.AddDays(-1),proc.ProcDate,proc.ProcNum,provNum);
			listProcs.Add(proc);
			listAdjs.Add(adj);
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddDays(1),0,0,30,listProcs,listAdjs);
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,0,0,0,0,0,YN.Yes);
				CheckAgingProcLifo(pat.PatNum,0,0,0,0,0,YN.No);
				Assert.AreEqual(pat.BalTotal,0);//everything on account should be credited
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		[TestMethod]
		public void LedgersTests_ComputeAging_PayPlanDynamicCreditsWithClaimProcs() {
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			long provNum=ProviderT.CreateProvider("Aging_PayPlanDynamic");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlanDynamic",suffix:suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc45=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",45,DateTime.Today.AddDays(-61),provNum:provNum);
			ClaimProc cp45=new ClaimProc();
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,"DynPayPlan");
			ClaimProcs.CreateEst(cp45,proc45,insInfo.PriInsPlan,insInfo.PriInsSub);
			cp45.Status=ClaimProcStatus.NotReceived;
			cp45.InsEstTotal=-1;
			cp45.InsPayEst=15;
			cp45.WriteOffEst=0;
			cp45.WriteOff=0;
			ClaimProcs.Update(cp45);
			listProcs.AddRange(new List<Procedure> {proc45});
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddDays(-1),0,0,30,listProcs,listAdjs);
			//make two non payplan productions to put on the account
			Procedure procUnattached=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",35,DateTime.Today.AddDays(-91),provNum:provNum);
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(payPlan.PayPlanNum);
			Assert.AreEqual(30,listChargesDb.Sum(x => x.Principal));
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,30,0,15,35,30,YN.Yes);//new - pay plan credit of $30 gets applied to cooresponding procedure
				CheckAgingProcLifo(pat.PatNum,30,0,45,5,30,YN.No);//old - pay plan credit gets applied to oldest production on the account
				CheckAgingProcLifo(pat.PatNum,30,0,45,5,30,YN.Unknown);
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		///<summary>This is the exact same unit test as LedgersTests_ComputeAging_PayPlanDynamicCreditsWithClaimProcs
		///except that there is an additional claimproc (a preauth) that is identical to the actual insurance estimate (pretty common IRL).
		///There was a bug when the ledger was not excluding these preauth claim procs which was causing the patient balance to inflate.</summary>
		[TestMethod]
		public void LedgersTests_ComputeAging_PayPlanDynamicCreditsWithClaimProcsWithPreAuth() {
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			long provNum=ProviderT.CreateProvider("Aging_PayPlanDynamic");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlanDynamic",suffix:suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc45=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",45,DateTime.Today.AddDays(-61),provNum:provNum);
			ClaimProc cp45=new ClaimProc();
			ClaimProc cp45_PreAuth=new ClaimProc();
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,"DynPayPlan");
			ClaimProcs.CreateEst(cp45,proc45,insInfo.PriInsPlan,insInfo.PriInsSub);
			cp45.Status=ClaimProcStatus.NotReceived;
			cp45.InsEstTotal=-1;
			cp45.InsPayEst=15;
			cp45.WriteOffEst=0;
			cp45.WriteOff=0;
			ClaimProcs.Update(cp45);
			ClaimProcs.CreateEst(cp45_PreAuth,proc45,insInfo.PriInsPlan,insInfo.PriInsSub,isPreauth: true);
			cp45_PreAuth.InsEstTotal=-1;
			cp45_PreAuth.InsPayEst=15;
			cp45_PreAuth.WriteOffEst=0;
			cp45_PreAuth.WriteOff=0;
			ClaimProcs.Update(cp45_PreAuth);
			listProcs.AddRange(new List<Procedure> {proc45});
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddDays(-1),0,0,30,listProcs,listAdjs);
			//make two non payplan productions to put on the account
			Procedure procUnattached=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",35,DateTime.Today.AddDays(-91),provNum:provNum);
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(payPlan.PayPlanNum);
			Assert.AreEqual(30,listChargesDb.Sum(x => x.Principal));
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,30,0,15,35,30,YN.Yes);//new - pay plan credit of $30 gets applied to cooresponding procedure
				CheckAgingProcLifo(pat.PatNum,30,0,45,5,30,YN.No);//old - pay plan credit gets applied to oldest production on the account
				CheckAgingProcLifo(pat.PatNum,30,0,45,5,30,YN.Unknown);
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		///<summary>Patient payments will get considered to reduce the amount of the procedure/adjustment that is attached to payment plan
		///ONLY when the payment is associated to the procNum/FKey AND NOT to the payplancharge/payplan. Simple case just to verify credits are
		///only being made for the amount attached to the payment plan.</summary>
		[TestMethod]
		public void LedgersTests_ComputeAging_PayPlanDynamicCreditsPatientPaymentsReduceTheAmountAttached() {
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			long provNum=ProviderT.CreateProvider("Aging_PayPlanDynamic");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlanDynamic",suffix:suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc68=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",68,DateTime.Today.AddDays(-61),provNum:provNum);
			Payment payProc68=PaymentT.MakePayment(pat.PatNum,22,procNum:proc68.ProcNum,provNum:proc68.ProvNum);
			listProcs.AddRange(new List<Procedure> {proc68});
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddDays(-1),0,0,46,listProcs,listAdjs);
			//Run pay plan logic to generate first set of charges
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(payPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{payPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(payPlan,listEntries);
			Assert.AreEqual(46,listChargesDb.Sum(x => x.Principal));//amount attached to payment plan does not include previous payment of $22
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,46,0,0,0,46,YN.Yes);//new
				CheckAgingProcLifo(pat.PatNum,46,0,0,0,46,YN.No);//old
				CheckAgingProcLifo(pat.PatNum,46,0,0,0,46,YN.Unknown);
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		///<summary>Patient payments will get considered to reduce the amount of the procedure/adjustment that is attached to payment plan
		///ONLY when the payment is associated to the procNum/FKey AND NOT to the payplancharge/payplan. Simple case just to verify credits are
		///only being made for the amount attached to the payment plan.</summary>
		[TestMethod]
		public void LedgersTests_ComputeAging_PayPlanDynamicCreditsPatientPaymentsToPayPlanDoNotReduceTheAmountAttached() {
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			long provNum=ProviderT.CreateProvider("Aging_PayPlanDynamic");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlanDynamic",suffix:suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc68=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",68,DateTime.Today.AddDays(-61),provNum:provNum);
			listProcs.AddRange(new List<Procedure> {proc68});
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddDays(-1),0,0,68,listProcs,listAdjs);
			Payment payProc68=PaymentT.MakePayment(pat.PatNum,22,procNum:proc68.ProcNum,provNum:proc68.ProvNum,payPlanNum:payPlan.PayPlanNum);
			//Run pay plan logic to generate first set of charges
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(payPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{payPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(payPlan,listEntries);
			Assert.AreEqual(68,listChargesDb.Sum(x => x.Principal));//full amount of procedure is attached to the payment plan
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,46,0,0,0,46,YN.Yes);//new - credits are still applied where they would have been, resulting in 46
				CheckAgingProcLifo(pat.PatNum,46,0,0,0,46,YN.No);//old
				CheckAgingProcLifo(pat.PatNum,46,0,0,0,46,YN.Unknown);
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

		//To guard against a bug where multiple payments would make the incorrect credit amount (duplicate rows)
		[TestMethod]
		public void LedgersTests_ComputeAging_PayPlanDynamicCreditsWithClaimProcsAndPatPay() {
			List<Procedure> listProcs=new List<Procedure>();
			List<Adjustment> listAdjs=new List<Adjustment>();
			long provNum=ProviderT.CreateProvider("Aging_PayPlanDynamic");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(fName:"Aging_PayPlanDynamic",suffix:suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			Procedure proc45=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",45,DateTime.Today.AddDays(-61),provNum:provNum);
			ClaimProc cp45=new ClaimProc();
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,"DynPayPlan");
			ClaimProcs.CreateEst(cp45,proc45,insInfo.PriInsPlan,insInfo.PriInsSub);
			cp45.Status=ClaimProcStatus.NotReceived;
			cp45.InsEstTotal=-1;
			cp45.InsPayEst=15;
			cp45.WriteOffEst=0;
			cp45.WriteOff=0;
			ClaimProcs.Update(cp45);//puts our pat portion at 30
			listProcs.AddRange(new List<Procedure> {proc45});
			PaymentT.MakePayment(pat.PatNum,7,procNum:proc45.ProcNum);
			PaymentT.MakePayment(pat.PatNum,3,procNum:proc45.ProcNum);//now pat portion is only at 20
			PayPlan payPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,pat.PatNum,DateTime.Today.AddDays(-1),0,0,20,listProcs,listAdjs);//1 payment
			//make two non payplan productions to put on the account
			Procedure procUnattached=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",35,DateTime.Today.AddDays(-91),provNum:provNum);
			//Run pay plan logic to generate first set of charges
			List<PayPlanCharge> listChargesDb=PayPlanCharges.GetForPayPlan(payPlan.PayPlanNum);
			List<PayPlanLink> listEntries=PayPlanLinks.GetForPayPlans(new List<long>{payPlan.PayPlanNum});
			PayPlanTerms terms=PayPlanT.GetTerms(payPlan,listEntries);
			Assert.AreEqual(20,listChargesDb.Sum(x => x.Principal));
			int payPlansVersionPrev=PrefC.GetInt(PrefName.PayPlansVersion);
			try {
				PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
				CheckAgingProcLifo(pat.PatNum,20,0,25,25,20,YN.Yes);//new - pay plan credit of $20 gets applied to cooresponding procedure
				CheckAgingProcLifo(pat.PatNum,20,0,45,5,20,YN.No);//old - pay plan credit gets applied to oldest production on the account
				CheckAgingProcLifo(pat.PatNum,20,0,45,5,20,YN.Unknown);
			}
			finally {
				PrefT.UpdateInt(PrefName.PayPlansVersion,payPlansVersionPrev);
			}
		}

	}
}

//====================================================================================================
//Some unit test ideas from Cameron 08/07/2019
//====================================================================================================
//Aging Unit Tests
//1. Procedures
//	a. With procs in each bucket, i.e. Over90, 61-90, 31-60, and 0-30 days old.
//	b. With UnitQty and BaseUnits
//2. Adjustments
//	a. Positive and negative adjustments.
//	b. With and without negative adj's aged.
//3. Claimprocs:
//	a. NotRcvd, Rcvd, Supplemental, CapClaim, CapComplete claims for all insurance types i.e. PPO, Cat %, Capitation, etc.
//	b. Include Supplemental, by proc, and total payments.
//	c. With and without W/O's aged and with and without a claimsnapshot (behavior will be different if no snapshot).
//	d. Rcvd and NotRcvd and also for Rcvd after the asOfDate.
//4. Paysplits
//	a. With splits to/from other family members and to/from another family.
//	b. Prepayments to/from other family members/another family, allocated and unallocated.
//5. Payment plans:
//	a. Patient payments and tracking ins payments.
//	b. Guar in same family and guar in different family.
//	c. With paysplits from pp patient, other family member, other family.
//	d. For each logic type (not aged (legacy), age credits and debits, age credits only, and no charges to account).
//	e. With "Age patient payments to payment plans" checked/unchecked.
//	f. With a closed payment plan.
//	g. With debits and credits on pp.
//6. Date last pay column.
//7. All Tests
//	a. With future dates procs/payments/adjustments etc (with pref to allow them set).
//	b. With asOfDate=today and asOfDate in the past.
//	c. For all patients and for a specific pat in the family.
//	d. Using enterprise aging and regular aging.
//	e. InsAging too!