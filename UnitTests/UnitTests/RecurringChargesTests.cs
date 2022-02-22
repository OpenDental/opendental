using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnitTestsCore;

namespace UnitTests.RecurringCharges_Tests {
	[TestClass]
	public class RecurringChargesTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			CreditCardT.ClearCreditCardTable();
			RecurringChargeT.ClearRecurringChargeTable();
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.DoNotAge);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.DontEnforce);
			OpenDentBusiness.Program prog=Programs.GetProgram(Programs.GetProgramNum(ProgramName.Xcharge));
			prog.Enabled=false;
			prog.IsDisabledByHq=false;
			Programs.Update(prog);
			prog=Programs.GetProgram(Programs.GetProgramNum(ProgramName.PayConnect));
			prog.Enabled=true;
			prog.IsDisabledByHq=false;
			Programs.Update(prog);
			Programs.RefreshCache();
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		#region RecurringChargesTests for RecurringChargesAllowedWhenPatBal0
		///<summary>Tests that, when Pref RecurringChargesAllowedWhenNoPatBal is true and the CreditCard col CanChargeWhenNoBal is true
		/// Does it impact normal functionality for when the charge is equal to the Patient Balance due?</summary>
		[TestMethod]
		public void RecurringCharges_RecurringChargesAllowedWhenPatBal0_PaysBal() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=100;
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),0,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==1);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			Payment payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			pat=Patients.GetPat(pat.PatNum); 
			Assert.AreEqual((balStarting+proc.ProcFee)-chargeAmt,pat.BalTotal);
		}

		///<summary>Tests that, when Pref RecurringChargesAllowedWhenNoPatBal is true and the CreditCard col CanChargeWhenNoBal is true
		///Does the application create a prepayment for the account for the amt paid with the correct note.
		///This test uses only one payment to verify operations.</summary>
		[TestMethod]
		public void RecurringCharges_RecurringChargesAllowedWhenPatBal0_PrePaysZeroBalAcctOneCharge() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=100;
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),0,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==1);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			Payment payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			pat=Patients.GetPat(pat.PatNum); 
			Assert.AreEqual(payment.PayNote,"Recurring Charge");
			Assert.AreEqual((balStarting-chargeAmt),pat.BalTotal);
		}

		///<summary>Tests that, when Pref RecurringChargesAllowedWhenNoPatBal is true and the CreditCard col CanChargeWhenNoBal is true
		///Does the application create a prepayment for the account for the amt paid with the correct note.
		///This test uses multiple payments to verify operations.</summary>
		[TestMethod]
		public void RecurringCharges_RecurringChargesAllowedWhenPatBal0_PrePaysZeroBalAcctMultiCharge() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=100;
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-8),0,canChargeWhenZeroBal:true);
			CreditCard creditCard2=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-6),0,canChargeWhenZeroBal:true);
			CreditCard creditCard3=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-4),0,canChargeWhenZeroBal:true);
			CreditCard creditCard4=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-2),0,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==4);
			RecurringChargeData recCharge=new RecurringChargeData();
			Payment payment=new Payment();
			foreach(RecurringChargeData charge in listCharges) {
				recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
				charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
				payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			}
			pat=Patients.GetPat(pat.PatNum); 
			Assert.AreEqual(payment.PayNote,"Recurring Charge");
			Assert.AreEqual(Math.Round(balStarting-chargeAmt*4),Math.Round(pat.BalTotal));
		}

		///<summary>Tests that, when Pref RecurringChargesAllowedWhenNoPatBal is true and the CreditCard col CanChargeWhenNoBal is true
		///If the user has a valid procfee, and their recurring charge, then this test verifies that both are paid and the balance returns
		///to zero as expected. It also verifies the propper split.UnearnedType has been assigned.</summary>
		[TestMethod]
		public void RecurringCharges_RecurringChargesAllowedWhenPatBal0_PaysBalAndPrepaysRemaining() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=200;
			double payPlanTotal=chargeAmt/2;
			double payPlanPaymentAmt=payPlanTotal;
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",50);
			PayPlan payPlan=PayPlanT.CreatePayPlan(pat.PatNum,payPlanTotal,payPlanPaymentAmt,DateTime.Today.AddMonths(-8),pat.PriProv);
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==1);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			Payment payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			List<PaySplit> listSplits=PaySplits.GetForPayment(payment.PayNum);
			pat=Patients.GetPat(pat.PatNum);
			Assert.AreEqual(2,listSplits.Count);
			Assert.AreEqual(1,listSplits.Count(x => x.PayPlanNum==payPlan.PayPlanNum
				&& x.SplitAmt==100
				&& x.UnearnedType==0));
			Assert.AreEqual(1,listSplits.Count(x => x.PayPlanNum==0
				&& x.SplitAmt==100
				&& x.UnearnedType>0));
		}

		///<summary>Tests that, when Pref RecurringChargesAllowedWhenNoPatBal is true and the CreditCard col CanChargeWhenNoBal is true
		///If the user has a valid procfee, and their recurring charge, then this test verifies that both are paid and the balance returns
		///to zero as expected. It also verifies the propper split.UnearnedType has been assigned.</summary>
		[TestMethod]
		public void RecurringCharges_RecurringChargesAllowedWhenPatBal0_PaysBalAndPrepaysRemaining_EnforceFully() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=200;
			double payPlanTotal=chargeAmt/2;
			double payPlanPaymentAmt=payPlanTotal;
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",50);
			PayPlan payPlan=PayPlanT.CreatePayPlan(pat.PatNum,payPlanTotal,payPlanPaymentAmt,DateTime.Today.AddMonths(-8),pat.PriProv);
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==1);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			Payment payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			List<PaySplit> listSplits=PaySplits.GetForPayment(payment.PayNum);
			pat=Patients.GetPat(pat.PatNum);
			Assert.AreEqual(2,listSplits.Count);
			Assert.AreEqual(1,listSplits.Count(x => x.PayPlanNum==payPlan.PayPlanNum 
				&& x.SplitAmt==100
				&& x.UnearnedType==0));
			Assert.AreEqual(1,listSplits.Count(x => x.PayPlanNum==0 
				&& x.SplitAmt==100
				&& x.UnearnedType>0));
		}

		[TestMethod]
		public void RecurringCharges_PaymentDecline_DontEnforce() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.DontEnforce);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=200;
			double payPlanTotal=chargeAmt/2;
			double payPlanPaymentAmt=payPlanTotal;
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",50);
			PayPlan payPlan=PayPlanT.CreatePayPlan(pat.PatNum,payPlanTotal,payPlanPaymentAmt,DateTime.Today.AddMonths(-8),pat.PriProv);
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==1);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			recCharge.RecurringCharge.ChargeAmt=0;//pretend there was an error/declined payment.
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			Payment payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			Assert.IsNotNull(payment);
			List<PaySplit> listSplits=PaySplits.GetForPayment(payment.PayNum);
			pat=Patients.GetPat(pat.PatNum);
			Assert.AreEqual(1,listSplits.Count);
			Assert.AreEqual(0,listSplits.FirstOrDefault().SplitAmt);
		}

		[TestMethod]
		public void RecurringCharges_PaymentDecline_EnforceFully() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=200;
			double payPlanTotal=chargeAmt/2;
			double payPlanPaymentAmt=payPlanTotal;
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",50);
			PayPlan payPlan=PayPlanT.CreatePayPlan(pat.PatNum,payPlanTotal,payPlanPaymentAmt,DateTime.Today.AddMonths(-8),pat.PriProv);
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==1);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			recCharge.RecurringCharge.ChargeAmt=0;//pretend there was an error/declined payment.
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			Payment payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			Assert.IsNotNull(payment);
			List<PaySplit> listSplits=PaySplits.GetForPayment(payment.PayNum);
			pat=Patients.GetPat(pat.PatNum);
			Assert.AreEqual(1,listSplits.Count);
			Assert.AreEqual(0,listSplits.FirstOrDefault().SplitAmt);
		}

		///<summary>Tests that, when Pref RecurringChargesAllowedWhenNoPatBal is false but CreditCard col CanChargeWhenNoBal is true.
		///If the user has a zero starting balance, and a recurring charge, then this test verified that the resulting recurring charge
		///is not made and the balance remains as zero since the Pref is not on.</summary>
		[TestMethod]
		public void RecurringCharges_RecurringChargesAllowedWhenPatBal0_BalStaysZeroWhenPrefFalse() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,false);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=100;
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),0,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==0);
		}

		///<summary>Tests that, when Pref RecurringChargesAllowedWhenNoPatBal is true but CreditCard col CanChargeWhenNoBal is false.
		///If the user has a zero starting balance, and a recurring charge, then this test verified that the resulting recurring charge
		///is not made and the balance remains as zero since the col is not turned on.</summary>
		[TestMethod]
		public void RecurringCharges_RecurringChargesAllowedWhenPatBal0_BalStaysZeroWhenCardFalse() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=100;
			CreditCard creditCard = CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),0,canChargeWhenZeroBal:false);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			Assert.IsTrue(pat.BalTotal==balStarting);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==0);
		}

		///<summary>Tests that, when Pref RecurringChargesAllowedWhenNoPatBal is true and CreditCard col CanChargeWhenNoBal is true.
		///If the user has a zero starting balance, and we add a proc to pay for, the ChargeAmt and RepeatChargeAmt are the same instead of 
		///the ChargeAmt matching the TotalDue.</summary>
		[TestMethod]
		public void RecurringCharges_RecurringChargesAllowedWhenPatBal0_ChargeEqualsRepeatCharge() {
			PrefT.UpdateBool(PrefName.RecurringChargesAllowedWhenNoPatBal,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			double balStarting=pat.BalTotal;
			double chargeAmt=100;//This is the amount paid by the patient
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",200);
			CreditCard creditCard=CreditCardT.CreateCard(pat.PatNum,chargeAmt,DateTime.Today.AddMonths(-3),0,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			pat=Patients.GetPat(pat.PatNum);
			Assert.IsTrue(pat.BalTotal==proc.ProcFee);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,true);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.IsTrue(listCharges.Count()==1);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			pat=Patients.GetPat(pat.PatNum);
			Assert.AreEqual(chargeAmt,recCharge.RecurringCharge.ChargeAmt);
			Assert.AreEqual(chargeAmt,recCharge.RecurringCharge.RepeatAmt);
			Assert.AreEqual(balStarting+proc.ProcFee-chargeAmt,pat.BalTotal);
			//This test is to ensure that we are not charging the full family balance in this scenario (just the recurring charge amount on the credit card).
			Assert.AreNotEqual(recCharge.RecurringCharge.ChargeAmt,recCharge.RecurringCharge.FamBal);
		}
		#endregion

		///<summary>Tests that a recurring charge is created for a patient.</summary>
		[TestMethod]
		public void RecurringCharges_FillCharges_CreateCharge() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,DateTime.Today,0);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargerator charger=new RecurringChargerator(_log,false);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { });
			Assert.AreEqual(1,listCharges.Count(x => x.RecurringCharge.PatNum==pat.PatNum));
		}

		///<summary>Tests that a recurring charge is not created for a patient since there is an outstanding pending charge.</summary>
		[TestMethod]
		public void RecurringCharges_FillCharges_PendingCharge() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,DateTime.Today,0);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeT.CreateRecurringCharge(pat.PatNum,RecurringChargeStatus.NotYetCharged,50,cc.CreditCardNum);
			RecurringChargerator charger=new RecurringChargerator(_log,false);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { });
			Assert.AreEqual(0,listCharges.Count(x => x.RecurringCharge.PatNum==pat.PatNum));
		}

		///<summary>Tests that a recurring charge is created for a patient since the outstanding pending charge is a day old.</summary>
		[TestMethod]
		public void RecurringCharges_FillCharges_PendingChargeYesterday() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,DateTime.Today,0);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeT.CreateRecurringCharge(pat.PatNum,RecurringChargeStatus.NotYetCharged,50,cc.CreditCardNum,dateCharge:DateTime.Today.AddDays(-1));
			RecurringChargerator charger=new RecurringChargerator(_log,false);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { });
			Assert.AreEqual(1,listCharges.Count(x => x.RecurringCharge.PatNum==pat.PatNum));
		}

		///<summary>Tests that an adjustment attached to authorized procedures are taken into account.</summary>
		[TestMethod]
		public void RecurringCharges_FillCharges_AdjustmentsOnProcs() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,true);
			PrefT.UpdateBool(PrefName.DockPhonePanelShow,true);//Run as OD HQ
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,DateTime.Today,0,authorizedProcs:"001");
			ProcedureCodeT.AddIfNotPresent("001");
			ProcedureCodes.RefreshCache();
			Procedure proc=ProcedureT.CreateProcedure(pat,"001",ProcStat.C,"0",100);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",40);
			AdjustmentT.MakeAdjustment(pat.PatNum,-30,procNum:proc.ProcNum);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeT.CreateRecurringCharge(pat.PatNum,RecurringChargeStatus.NotYetCharged,50,cc.CreditCardNum,dateCharge: DateTime.Today.AddDays(-1));
			RecurringChargerator charger=new RecurringChargerator(_log,false);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { }).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.AreEqual(1,listCharges.Count());
			Assert.AreEqual(70,listCharges[0].RecurringCharge.ChargeAmt);
		}

		///<summary>Tests that multiple adjustments attached to authorized procedures are taken into account.</summary>
		[TestMethod]
		public void RecurringCharges_FillCharges_TwoAdjustmentsOnProcs() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,true);
			PrefT.UpdateBool(PrefName.DockPhonePanelShow,true);//Run as OD HQ
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,DateTime.Today,0,authorizedProcs:"001");
			ProcedureCodeT.AddIfNotPresent("001");
			ProcedureCodes.RefreshCache();
			Procedure proc=ProcedureT.CreateProcedure(pat,"001",ProcStat.C,"0",100);
			ProcedureT.CreateProcedure(pat,"001",ProcStat.C,"0",100);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",40);
			AdjustmentT.MakeAdjustment(pat.PatNum,-30,procNum: proc.ProcNum);
			AdjustmentT.MakeAdjustment(pat.PatNum,20,procNum: proc.ProcNum);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeT.CreateRecurringCharge(pat.PatNum,RecurringChargeStatus.NotYetCharged,50,cc.CreditCardNum,dateCharge: DateTime.Today.AddDays(-1));
			RecurringChargerator charger=new RecurringChargerator(_log,false);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { }).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			Assert.AreEqual(1,listCharges.Count());
			Assert.AreEqual(190,listCharges[0].RecurringCharge.ChargeAmt);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedDayOfMonth</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/17/2018</para>
		///<para>Today: 07/23/2018</para>
		///<para>Day(s) to be Charged: 16</para>
		///This case should not be charged. The 16th payment has already occurred for this month.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedDayOfMonth_ChargeOccurredSixDaysAgo() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,17),ToDayOfMonthFrequency(new List<int> { 16 }),100,new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedDayOfMonth</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/08/2018</para>
		///<para>Today: 07/23/2018</para>
		///<para>Day(s) to be Charged: 8,31</para>
		///This case should not be charged. The 8th payment for July occurred and the 31st of July has not happened yet.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedDayOfMonth_FirstChargeOccurredSecondDayHasNotArrived() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,8),ToDayOfMonthFrequency(new List<int> { 8,31 }),100,new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedDayOfMonth</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 05/31/2018</para>
		///<para>Today: 07/23/2018</para>
		///<para>Day(s) to be Charged: 31</para>
		///This case should be charged. While today is before the day it should be charged for this month (7-31-2018), the payment did
		///not occur last month.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedDayOfMonth_ThisMonthNotArrivedLastMonthDidNotHappen() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,5,31),ToDayOfMonthFrequency(new List<int> { 31 }),100,new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(1,table.Rows.Count);
			Assert.AreEqual("100",table.Rows[0]["ChargeAmt"].ToString());
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedDayOfMonth</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 05/16/2018</para>
		///<para>Today: 07/23/2018</para>
		///<para>Day(s) to be Charged: 1,15</para>
		///This case should be charged and the ChargeAmt should be doubled. Because neither July's 1st or 15th payment occurred, the card will be
		///charged double the amount.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedDayOfMonth_TwoDatesNeitherGotCharged() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,5,16),ToDayOfMonthFrequency(new List<int> { 1,15 }),100,
				new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(1,table.Rows.Count);
			Assert.AreEqual("200",table.Rows[0]["ChargeAmt"].ToString());
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedDayOfMonth</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/12/2018</para>
		///<para>Today: 07/23/2018</para>
		///<para>Day(s) to be Charged: 5,10,15,20,25,30</para>
		///This case should be charged and the ChargeAmt should be x2 as 5,10 occurred, 15,20 have not, and 25,30 should not be charged yet. The
		///x2 is for the 15th and the 20th.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedDayOfMonth_SixDatesTwoGotChargeTwoNotArrived() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,12),ToDayOfMonthFrequency(new List<int> { 5,10,15,20,25,30 }),100,
				new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(1,table.Rows.Count);
			Assert.AreEqual("200",table.Rows[0]["ChargeAmt"].ToString());
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedDayOfMonth</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 02/01/2018</para>
		///<para>Today: 03/01/2018</para>
		///<para>Day(s) to be Charged: 29,30,31</para>
		///This case should be charged and the ChargeAmt should be x3 as none of the three dates (all would be charged on 2-28-2018) got charged for 
		///February.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedDayOfMonth_FebruaryEdgeCase() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,2,1),ToDayOfMonthFrequency(new List<int> { 29,30,31 }),100,
				new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,3,1));
			Assert.AreEqual(1,table.Rows.Count);
			Assert.AreEqual("300",table.Rows[0]["ChargeAmt"].ToString());
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedDayOfMonth</para>
		///<para>Start Date: 02/02/2018</para>
		///<para>Latest Payment:</para>
		///<para>Today: 02/03/2018</para>
		///<para>Day(s) to be Charged: 16</para>
		///This case should not be charged. No payment was made last month. However, the 16th of february is the first day that should be charged.
		///</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedDayOfMonth_FirstPaymentBeforePaymentDate() {
			DataTable table=CreateRecurringChargeRow(new DateTime(),ToDayOfMonthFrequency(new List<int> { 16 }),100,
				new DateTime(2018,2,2));
			FilterRecurringChargeList(table,new DateTime(2018,2,3));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Every</para>
		///<para>Day: Sunday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/22/2018 (Sunday)</para>
		///<para>Today: 07/23/2018 (Monday)</para>
		///This case should not be charged. The most recent sunday to be charged is 7-22-2018 and that was when the latest payment occurred.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EverySundayPaymentHappened() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,22),ToWeekDayFrequency(DayOfWeekFrequency.Every,DayOfWeek.Sunday),100,
				new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Every</para>
		///<para>Day: Sunday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/22/2018 (Sunday)</para>
		///<para>Today: 07/23/2018 10:00 AM (Monday)</para>
		///This case should not be charged. The most recent sunday to be charged is 7-22-2018 and that was when the latest payment occurred.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EverySundayPaymentHappenedNow10AM() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,22),ToWeekDayFrequency(DayOfWeekFrequency.Every,DayOfWeek.Sunday),100,
				new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23,10,0,0));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: EveryOther</para>
		///<para>Day: Sunday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/22/2018 (Sunday)</para>
		///<para>Today: 07/23/2018 10:00 AM (Monday)</para>
		///This case should not be charged. The most recent sunday to be charged is 7-22-2018 and that was when the latest payment occurred.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EveryOtherSundayPaymentHappenedNow10AM() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,22),ToWeekDayFrequency(DayOfWeekFrequency.EveryOther,DayOfWeek.Sunday),100,
				new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23,10,0,0));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Fourth</para>
		///<para>Day: Sunday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/22/2018 (Sunday)</para>
		///<para>Today: 07/23/2018 10:00 AM (Monday)</para>
		///This case should not be charged. The most recent sunday to be charged is 7-22-2018 and that was when the latest payment occurred.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EveryFourthSundayPaymentHappenedNow10AM() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,22),ToWeekDayFrequency(DayOfWeekFrequency.Fourth,DayOfWeek.Sunday),100,
				new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23,10,0,0));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Every</para>
		///<para>Day: Sunday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/03/2018 (Tuesday)</para>
		///<para>Today: 07/23/2018 (Monday)</para>
		///This case should be charged and the ChargeAmt should be x3. For FixedWeekDay, only the previous month of payments can be charged. There
		///were 3 Sundays missed between today (7-23-2018) and the last payment (7-3-2018)</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EverySundayPaymentHappenedFourSundaysAgo() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,3),ToWeekDayFrequency(DayOfWeekFrequency.Every,DayOfWeek.Sunday),100,
				new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(1,table.Rows.Count);
			Assert.AreEqual("300",table.Rows[0]["ChargeAmt"].ToString());
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Every Other</para>
		///<para>Day: Thursday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 07/13/2018 (Friday)</para>
		///<para>Today: 07/23/2018 (Monday)</para>
		///This case should not be charged. The latest payment was closest to 2018-7-12 thursday. Using this every other cycle, the next payment
		///would be on 2018-7-26.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EveryOtherThursdayPaymentWithinTheLastTwoWeeks() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,7,13),ToWeekDayFrequency(DayOfWeekFrequency.EveryOther,DayOfWeek.Thursday),
				100,new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Every Other</para>
		///<para>Day: Friday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 01/12/2018 (Friday)</para>
		///<para>Today: 07/23/2018 (Monday)</para>
		///This case should be charged and the ChargeAmt should be doubled. No payment's have happened recently so the last month worth of 
		///payment's will be charged.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EveryOtherFridayPaymentWithinTheLastTwoWeeks() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,1,12),ToWeekDayFrequency(DayOfWeekFrequency.EveryOther,DayOfWeek.Friday),
				100,new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(1,table.Rows.Count);
			Assert.AreEqual("200",table.Rows[0]["ChargeAmt"].ToString());
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Fourth</para>
		///<para>Day: Tuesday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 06/27/2018 (Wednesday)</para>
		///<para>Today: 07/23/2018 (Monday)</para>
		///This case should not be charged. The fourth Tuesday of this month (7-24-2018) is after the current date. The payment for last month was
		///after the fourth Tuesday for that month, so no charge will occur.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_FourthTuesdayPaymentHappenedLastMonth() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,6,27),ToWeekDayFrequency(DayOfWeekFrequency.Fourth,DayOfWeek.Tuesday),
				100,new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: First</para>
		///<para>Day: Sunday</para>
		///<para>Start Date: 01/01/2018</para>
		///<para>Latest Payment: 06/03/2018 (Sunday)</para>
		///<para>Today: 07/23/2018 (Monday)</para>
		///This case should be charged. There was no charge on the first sunday for this month</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_FirstSundayPaymentNotHappenedThisMonth() {
			DataTable table=CreateRecurringChargeRow(new DateTime(2018,6,3),ToWeekDayFrequency(DayOfWeekFrequency.First,DayOfWeek.Sunday),
				100,new DateTime(2018,1,1));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(1,table.Rows.Count);
			Assert.AreEqual("100",table.Rows[0]["ChargeAmt"].ToString());
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Fifth</para>
		///<para>Day: Saturday</para>
		///<para>Start Date: 02/01/2018</para>
		///<para>Latest Payment:</para>
		///<para>Today: 02/22/2018 (Thursday)</para>
		///This case should not be charged. This month's "fifth saturday" (2-28-2018 in this case) has not occurred and last month's was before the 
		///start date.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_FifthSaturdayChargeStartedRecently() {
			DataTable table=CreateRecurringChargeRow(new DateTime(),ToWeekDayFrequency(DayOfWeekFrequency.Fifth,DayOfWeek.Saturday),
				100,new DateTime(2018,2,1));
			FilterRecurringChargeList(table,new DateTime(2018,2,22));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Every</para>
		///<para>Day: Wednesday</para>
		///<para>Start Date: 07/13/2018 (Friday)</para>
		///<para>Latest Payment:</para>
		///<para>Today: 07/23/2018 (Monday)</para>
		///This case will be charged. This is because there is one Wednesday between the start date and the current
		///day that need to be accounted for.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EveryWednesdayCreatedTwoFridaysAgo() {
			DataTable table=CreateRecurringChargeRow(new DateTime(),ToWeekDayFrequency(DayOfWeekFrequency.Every,DayOfWeek.Wednesday),
				100,new DateTime(2018,7,13));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(1,table.Rows.Count);
			Assert.AreEqual("100",table.Rows[0]["ChargeAmt"].ToString());
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedWeekDay</para>
		///<para>Day Frequency: Every</para>
		///<para>Day: Friday</para>
		///<para>Start Date: 07/23/2018 (Monday)</para>
		///<para>Latest Payment:</para>
		///<para>Today: 07/23/2018 (Monday)</para>
		///This case will not be charged. The charge was created today and the first friday will be after today on 7/27/2018.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedWeekDay_EveryFridayCreatedOnMonday() {
			DataTable table=CreateRecurringChargeRow(new DateTime(),ToWeekDayFrequency(DayOfWeekFrequency.Every,DayOfWeek.Friday),
				100,new DateTime(2018,7,23));
			FilterRecurringChargeList(table,new DateTime(2018,7,23));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests the method that filters cards with recurring charges that should be charged.
		///<para>Frequency Type: FixedDayOfMonth</para>
		///<para>Start Date: 10/03/2018</para>
		///<para>Latest Payment: </para>
		///<para>Today: 10/03/2018</para>
		///<para>Day(s) to be Charged: 1,18</para>
		///This case will not be charged. The 18th of the month has not come yet. The 1st has passed, but was before the start date.</summary>
		[TestMethod]
		public void RecurringCharges_FilterRecuringChargeList_FixedDayOfMonth_CreatedTodayFirstDateTwoDaysAgo() {
			DataTable table=CreateRecurringChargeRow(new DateTime(),ToDayOfMonthFrequency(new List<int> { 1,18 }),
				100,new DateTime(2018,10,3));
			FilterRecurringChargeList(table,new DateTime(2018,10,3));
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>Tests that sending recurring charges updates the preference which tracks last run datetime.</summary>
		[TestMethod]
		public void RecurringCharges_SendCharges_SetsRecurringChargesBeginDateTime() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			string strRecurringChargesBeginDateTimeExpected="";//Should be blank after recurring charges completes.
			string strRecurringChargesBeginDateTimeStarting="RecurringChargesBeginDateTime should be blank after running "+suffix;
			PrefT.UpdateString(PrefName.RecurringChargesBeginDateTime,strRecurringChargesBeginDateTimeStarting);
			RecurringChargerator charger=new RecurringChargerator(_log,false);
			charger.SendCharges(new List<RecurringChargeData>(),false);//Passing an empty list won't actually send anything.  Just updates pref.
			Assert.AreEqual(strRecurringChargesBeginDateTimeExpected,PrefC.GetString(PrefName.RecurringChargesBeginDateTime));
		}

		///<summary>Tests that a card that gets charged once per month sets the RecurringChargeDate on the payment properly.</summary>
		[TestMethod]
		public void RecurringCharges_CreatePayment_SetsRecurringChargeDate() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime cardStartDate=new DateTime(2019,01,25);
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,cardStartDate,0);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			DateTime runTime=new DateTime(2019,02,08,09,00,00);
			charger.SetNowDateTime=runTime;
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { });
			//Pretend the charge was processed successfully and make a payment.
			charger.CreatePayment(pat,listCharges.First(x => x.RecurringCharge.PatNum==pat.PatNum),"This is a real pretend charge.",50,"");
			Payment payment=Payments.Refresh(pat.PatNum).First(x => x.PayDate.Date==runTime.Date);
			Assert.AreEqual(cardStartDate.Date,payment.RecurringChargeDate.Date);
		}

		///<summary>Tests that a card that gets charged every other Friday sets the RecurringChargeDate on the payment properly.</summary>
		[TestMethod]
		public void RecurringCharges_CreatePayment_SetsRecurringChargeDateEveryOtherFriday() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime cardStartDate=new DateTime(2019,01,25);//Friday
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,cardStartDate,0,"",ChargeFrequencyType.FixedWeekDay,DayOfWeekFrequency.EveryOther,
				DayOfWeek.Friday);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeT.CreateRecurringCharge(pat.PatNum,RecurringChargeStatus.ChargeSuccessful,50,cc.CreditCardNum,cardStartDate);
			PaymentT.MakePayment(pat.PatNum,50,cardStartDate,isRecurringCharge: true,recurringChargeDate: cardStartDate);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			DateTime runTime=new DateTime(2019,02,08,09,00,00);//Friday
			charger.SetNowDateTime=runTime;
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { });
			//Pretend the charge was processed successfully and make a payment.
			charger.CreatePayment(pat,listCharges.First(x => x.RecurringCharge.PatNum==pat.PatNum),"This is a real pretend charge.",50,"");
			Payment payment=Payments.Refresh(pat.PatNum).First(x => x.PayDate.Date==runTime.Date);
			Assert.AreEqual(runTime.Date,payment.RecurringChargeDate.Date);
		}

		///<summary>Tests that a card that gets charged every Wednesday sets the RecurringChargeDate on the payment properly.</summary>
		[TestMethod]
		public void RecurringCharges_CreatePayment_SetsRecurringChargeDateEveryWednesday() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime cardStartDate=new DateTime(2019,01,23);//Wednesday
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,cardStartDate,0,"",ChargeFrequencyType.FixedWeekDay,DayOfWeekFrequency.Every,
				DayOfWeek.Wednesday);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeT.CreateRecurringCharge(pat.PatNum,RecurringChargeStatus.ChargeSuccessful,50,cc.CreditCardNum,cardStartDate);
			PaymentT.MakePayment(pat.PatNum,50,cardStartDate,isRecurringCharge: true,recurringChargeDate: cardStartDate);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			DateTime runTime=new DateTime(2019,01,30,09,00,00);//Wednesday
			charger.SetNowDateTime=runTime;
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { });
			//Pretend the charge was processed successfully and make a payment.
			charger.CreatePayment(pat,listCharges.First(x => x.RecurringCharge.PatNum==pat.PatNum),"This is a real pretend charge.",50,"");
			Payment payment=Payments.Refresh(pat.PatNum).First(x => x.PayDate.Date==runTime.Date);
			Assert.AreEqual(runTime.Date,payment.RecurringChargeDate.Date);
		}

		///<summary>Tests that a card that gets charged every Tuesday sets the RecurringChargeDate on the payment properly after missing a couple weeks.
		///</summary>
		[TestMethod]
		public void RecurringCharges_CreatePayment_SetsRecurringChargeDateEveryTuesday() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime cardStartDate=new DateTime(2019,01,22);//Tuesday
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,cardStartDate,0,"",ChargeFrequencyType.FixedWeekDay,DayOfWeekFrequency.Every,
				DayOfWeek.Tuesday);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			DateTime runTime=new DateTime(2019,02,07,09,00,00);//Wednesday
			charger.SetNowDateTime=runTime;
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { });
			//Pretend the charge was processed successfully and make a payment.
			charger.CreatePayment(pat,listCharges.First(x => x.RecurringCharge.PatNum==pat.PatNum),"This is a real pretend charge.",50,"");
			Payment payment=Payments.Refresh(pat.PatNum).First(x => x.PayDate.Date==runTime.Date);
			Assert.AreEqual(new DateTime(2019,02,05)/*Tuesday*/,payment.RecurringChargeDate.Date);
		}

		///<summary>Tests that a card that gets charged three fixed days per month sets the RecurringChargeDate on the payment properly after missing 
		///a couple charges.</summary>
		[TestMethod]
		public void RecurringCharges_CreatePayment_SetsRecurringChargeDateThreeFixedDays() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime cardStartDate=new DateTime(2019,01,01);
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,50,cardStartDate,0,"",ChargeFrequencyType.FixedDayOfMonth,daysOfMonth:"3,10,17");
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			DateTime runTime=new DateTime(2019,02,12,09,00,00);
			charger.SetNowDateTime=runTime;
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> { });
			//Pretend the charge was processed successfully and make a payment.
			charger.CreatePayment(pat,listCharges.First(x => x.RecurringCharge.PatNum==pat.PatNum),"This is a real pretend charge.",50,"");
			Payment payment=Payments.Refresh(pat.PatNum).First(x => x.PayDate.Date==runTime.Date);
			Assert.AreEqual(new DateTime(2019,02,10),payment.RecurringChargeDate.Date);
		}

		///<summary></summary>
		[TestMethod]
		public void RecurringCharges_CreatePayment_EnforceFully_AutoSplit() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			Patient pat=PatientT.CreatePatient(suffix);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",50,procDate:DateTime.Now.AddDays(-15));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",50,procDate:DateTime.Now.AddDays(-5));
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,75,DateTime.Today.AddMonths(-3),0,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			Payment payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			List<PaySplit> listSplits=PaySplits.GetForPayment(payment.PayNum);
			pat=Patients.GetPat(pat.PatNum);
			Assert.AreEqual(2,listSplits.Count);
			Assert.AreEqual(50,listSplits.FirstOrDefault(x => x.ProcNum==proc1.ProcNum)?.SplitAmt??0);
			Assert.AreEqual(25,listSplits.FirstOrDefault(x => x.ProcNum==proc2.ProcNum)?.SplitAmt??0);
			Assert.AreEqual(25,pat.Bal_0_30);
		}

		///<summary></summary>
		[TestMethod]
		public void RecurringCharges_CreatePayment_EnforceFully_AutoSplit_PaymentWithNoProcAssociation() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			Patient pat=PatientT.CreatePatient(suffix);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",50,procDate:DateTime.Now.AddDays(-35));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",50,procDate:DateTime.Now.AddDays(-20));
			//create a payment with no paysplit associated to it
			Payment payNon=PaymentT.MakePayment(pat.PatNum,25,payDate:DateTime.Now.AddDays(-3));
			CreditCard cc=CreditCardT.CreateCard(pat.PatNum,75,DateTime.Today.AddMonths(-3),0,canChargeWhenZeroBal:true);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum);
			RecurringChargeData recCharge=listCharges.FirstOrDefault(x => x.RecurringCharge.PatNum==pat.PatNum);
			charger.CreatePayment(pat,recCharge,"",recCharge.RecurringCharge.ChargeAmt,"");
			Payment payment=Payments.Refresh(pat.PatNum).FirstOrDefault(x => x.PayDate.Date==recCharge.RecurringChargeDate);
			List<PaySplit> listSplits=PaySplits.GetForPayment(payment.PayNum);
			pat=Patients.GetPat(pat.PatNum);
			Assert.AreEqual(2,listSplits.Count);
			Assert.AreEqual(25,listSplits.FirstOrDefault(x => x.ProcNum==proc1.ProcNum)?.SplitAmt??0);
			Assert.AreEqual(50,listSplits.FirstOrDefault(x => x.ProcNum==proc2.ProcNum)?.SplitAmt??0);
			Assert.AreEqual(0,pat.BalTotal);
		}

		///<summary>Pay plan due equals family balance which is less than repeat amount</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Family() {
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			//Setup
			Patient guar=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_Guar");
			//Family member 1
			long patNum=guar.PatNum;
			PayPlan payPlan=PayPlanT.CreatePayPlan(patNum,550,100,DateTime.Today.AddMonths(-6),guar.PriProv,guarantorNum:guar.PatNum);
			PaymentT.MakePayment(patNum,500,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			CreditCardT.CreateCard(guar.PatNum,50,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			//Family member 2
			Patient pat2=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_2",guarantor:guar.PatNum);
			long patNum2=pat2.PatNum;
			PayPlan payPlan2=PayPlanT.CreatePayPlan(patNum2,550,100,DateTime.Today.AddMonths(-6),pat2.PriProv,guarantorNum:guar.PatNum);
			PaymentT.MakePayment(patNum2,500,DateTime.Today.AddMonths(-3),payPlan2.PayPlanNum);
			CreditCardT.CreateCard(guar.PatNum,50,DateTime.Today.AddMonths(-3),payPlan2.PayPlanNum);
			//Family member 3
			Patient pat3=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"_3",guarantor:guar.PatNum);
			long patNum3=pat3.PatNum;
			PayPlan payPlan3=PayPlanT.CreatePayPlan(patNum3,550,100,DateTime.Today.AddMonths(-6),pat3.PriProv,guarantorNum:guar.PatNum);
			PaymentT.MakePayment(patNum3,500,DateTime.Today.AddMonths(-3),payPlan3.PayPlanNum);
			CreditCardT.CreateCard(guar.PatNum,50,DateTime.Today.AddMonths(-3),payPlan3.PayPlanNum);
			//change payplan version to 2
			Prefs.UpdateInt(PrefName.PayPlansVersion,2);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			//Family member 1
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {});
			for(int i=0;i<listCharges.Count;i++) {
				charger.CreatePayment(guar,listCharges[i],"",listCharges[i].RecurringCharge.ChargeAmt,"");
			}
			List<PaySplit> listSplits=PaySplits.GetForPats(new List<long>(){ patNum,patNum2,patNum3 });
			//Family member 1, 2, and 3 all made $500 payments to their respective payment plans which left a total of $50 on each plan due.
			//The recurring charge system should pick up on the $50 still due on each plan and make payments for them.
			//The guarantor has 3 credit cards set up for recurring charges that are each linked to a respective payment plan.
			//Therefore, the guarantor should end up with the following payment splits:
			Assert.AreEqual(6,listSplits.Count);
			//Family member 1
			Assert.AreEqual(1,listSplits.Count(x => x.PatNum==guar.PatNum && x.SplitAmt==500));//Manual split made for payment plan.
			Assert.AreEqual(1,listSplits.Count(x => x.PatNum==guar.PatNum && x.SplitAmt==50)); //Recurring charge split
			//Family member 2
			Assert.AreEqual(1,listSplits.Count(x => x.PatNum==pat2.PatNum && x.SplitAmt==500));//Manual split made for payment plan
			Assert.AreEqual(1,listSplits.Count(x => x.PatNum==pat2.PatNum && x.SplitAmt==50)); //Recurring charge split
			//Family member 3
			Assert.AreEqual(1,listSplits.Count(x => x.PatNum==pat3.PatNum && x.SplitAmt==500));//Manual split made for payment plan
			Assert.AreEqual(1,listSplits.Count(x => x.PatNum==pat3.PatNum && x.SplitAmt==50)); //Recurring charge split
		}

		///<summary></summary> 
		[TestMethod]
		public void RecurringCharges_CreatePayment_PaymentPlanEnforceFullyFIFO() {
			PrefT.UpdateBool(PrefName.RecurringChargesUseTransDate,true);
			PrefT.UpdateInt(PrefName.RigorousAccounting,(int)RigorousAccounting.EnforceFully);
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			//Make a completed procedure that predates a payment plan to emphasize the fact that credit cards flagged for payment plans must prefer them.
			ProcedureT.CreateProcedure(pat,"PPEFFIFO",ProcStat.C,"",100,procDate:DateTime.Now.AddYears(-1));
			//Make a payment plan that starts after the procedure was completed.
			PayPlan payPlan=PayPlanT.CreatePayPlan(pat.PatNum,550,100,DateTime.Today.AddMonths(-6),pat.PriProv);
			//Make a credit card that is flagged for recurring charges but is explicitly designed for making payments on the payment plan.
			CreditCardT.CreateCard(pat.PatNum,50,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			Ledgers.ComputeAging(pat.Guarantor,DateTime.Today);
			RecurringChargeratorTest charger=new RecurringChargeratorTest(_log,false);
			List<RecurringChargeData> listCharges=charger.FillCharges(new List<Clinic> {});
			Assert.AreEqual(1,listCharges.Count);
			charger.CreatePayment(pat,listCharges[0],"",listCharges[0].RecurringCharge.ChargeAmt,"");
			List<PaySplit> listSplits=PaySplits.GetForPats(new List<long>() { pat.PatNum });
			//Assert that the payment made was designated for the payment plan and not the procedure even thought the procedure is technically FIFO.
			Assert.AreEqual(1,listSplits.Count);
			Assert.AreEqual(1,listSplits.Count(x => x.PatNum==pat.PatNum 
				&& x.SplitAmt==50
				&& x.ProcNum==0
				&& x.PayPlanNum==payPlan.PayPlanNum));
		}

		///<summary>Pay plan due equals family balance which is less than repeat amount</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Version1_EqualsFamBalance() {
			//Setup
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long patNum=pat.PatNum;
			PayPlan payPlan=PayPlanT.CreatePayPlan(patNum,550,100,DateTime.Today.AddMonths(-6),pat.PriProv);
			PaymentT.MakePayment(patNum,500,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			CreditCardT.CreateCard(patNum,100,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			//Change payplan version to version 1
			Prefs.UpdateInt(PrefName.PayPlansVersion,1);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargerator charger=new RecurringChargeratorTest(_log,true);
			RecurringChargeData data=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum).FirstOrDefault();
			Assert.IsNotNull(data);
			//PayPlanVersion 1 Scenario 1: Pay plan due equals family balance which is less than repeat amount
			Assert.IsFalse(data.RecurringCharge.FamBal!=0
				|| data.RecurringCharge.PayPlanDue!=50
				|| data.RecurringCharge.TotalDue!=50
				|| data.RecurringCharge.ChargeAmt!=50);
		}

		///<summary>Pay plan due equals family balance which is less than repeat amount</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Version2_EqualsFamBalance() {
			//Setup
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long patNum=pat.PatNum;
			PayPlan payPlan=PayPlanT.CreatePayPlan(patNum,550,100,DateTime.Today.AddMonths(-6),pat.PriProv);
			PaymentT.MakePayment(patNum,500,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			CreditCardT.CreateCard(patNum,100,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			//change payplan version to 2
			Prefs.UpdateInt(PrefName.PayPlansVersion,2);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargerator charger=new RecurringChargeratorTest(_log,true);
			RecurringChargeData data=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum).FirstOrDefault();
			Assert.IsNotNull(data);
			//PayPlanVersion 2 Scenario 1: Pay plan due equals family balance which is less than repeat amount
			Assert.IsFalse(data.RecurringCharge.FamBal!=50
				|| data.RecurringCharge.PayPlanDue!=50
				|| data.RecurringCharge.TotalDue!=50
				|| data.RecurringCharge.ChargeAmt!=50);
		}

		///<summary>Pay plan due with negative family balance.</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Version1_NegativeFamilyBal() {
			//Setup
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long patNum=pat.PatNum;
			PayPlan	payPlan=PayPlanT.CreatePayPlan(patNum,300,75,DateTime.Today.AddMonths(-8),pat.PriProv);
			PaymentT.MakePayment(patNum,225,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			PaymentT.MakePayment(patNum,200,DateTime.Today.AddMonths(-3));
			CreditCardT.CreateCard(patNum,75,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			//Change payplan version to version 1
			Prefs.UpdateInt(PrefName.PayPlansVersion,1);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargerator charger=new RecurringChargeratorTest(_log,true);
			RecurringChargeData data=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum).FirstOrDefault();
			Assert.IsNotNull(data);
			//PayPlanVersion 1 Scenario 2: Pay plan due with negative family balance.
			Assert.IsFalse(data.RecurringCharge.FamBal!=-200
				|| data.RecurringCharge.PayPlanDue!=75
				|| data.RecurringCharge.TotalDue!=75
				|| data.RecurringCharge.ChargeAmt!=75);
		}

		///<summary>Pay plan due with negative family balance.</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Version2_NegativeFamilyBal() {
			//Setup
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long patNum=pat.PatNum;
			PayPlan payPlan=PayPlanT.CreatePayPlan(patNum,300,75,DateTime.Today.AddMonths(-8),pat.PriProv);
			PaymentT.MakePayment(patNum,225,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			PaymentT.MakePayment(patNum,200,DateTime.Today.AddMonths(-3));
			CreditCardT.CreateCard(patNum,75,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			//change payplan version to 2
			Prefs.UpdateInt(PrefName.PayPlansVersion,2);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargerator charger=new RecurringChargeratorTest(_log,true);
			RecurringChargeData data=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum).FirstOrDefault();
			Assert.IsNotNull(data);
			//PayPlanVersion 2 Scenario 2: Pay plan due with negative family balance.
			Assert.IsFalse(data.RecurringCharge.FamBal!=-125
				|| data.RecurringCharge.PayPlanDue!=75
				|| data.RecurringCharge.TotalDue!=75
				|| data.RecurringCharge.ChargeAmt!=75);
		}

		///<summary>Pay plan due less than family balance.</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Version1_LessThanFamilyBal() {
			//Setup
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long patNum=pat.PatNum;
			PayPlan payPlan=PayPlanT.CreatePayPlan(patNum,250,50,DateTime.Today.AddMonths(-12),pat.PriProv);
			PaymentT.MakePayment(patNum,200,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			CreditCardT.CreateCard(patNum,100,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			//Change payplan version to version 1
			Prefs.UpdateInt(PrefName.PayPlansVersion,1);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargerator charger=new RecurringChargeratorTest(_log,true);
			RecurringChargeData data=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum).FirstOrDefault();
			Assert.IsNotNull(data);
			//PayPlanVersion 1 Scenario 3: Pay plan due less than family balance
			Assert.IsFalse(data.RecurringCharge.FamBal!=100
				|| data.RecurringCharge.PayPlanDue!=50
				|| data.RecurringCharge.TotalDue!=150
				|| data.RecurringCharge.ChargeAmt!=100);
		}

		///<summary>Pay plan due less than family balance.</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Version2_LessThanFamilyBal() {
			//Setup
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long patNum=pat.PatNum;
			PayPlan payPlan=PayPlanT.CreatePayPlan(patNum,250,50,DateTime.Today.AddMonths(-12),pat.PriProv);
			PaymentT.MakePayment(patNum,200,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			CreditCardT.CreateCard(patNum,100,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"0",100);
			//Change payplan version to version 2
			Prefs.UpdateInt(PrefName.PayPlansVersion,2);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargerator charger=new RecurringChargeratorTest(_log,true);
			RecurringChargeData data=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum).FirstOrDefault();
			Assert.IsNotNull(data);
			//PayPlanVersion 2 Scenario 3: Pay plan due less than family balance
			Assert.IsFalse(data.RecurringCharge.FamBal!=150
				|| data.RecurringCharge.PayPlanDue!=50
				|| data.RecurringCharge.TotalDue!=150
				|| data.RecurringCharge.ChargeAmt!=100);
		}

		///<summary>Pay plan due more than repeat amount.</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Version1_MoreThanRepeatAmt() {
			//Setup
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long patNum=pat.PatNum;
			PayPlan payPlan=PayPlanT.CreatePayPlan(patNum,250,50,DateTime.Today.AddMonths(-12),pat.PriProv);
			PaymentT.MakePayment(patNum,100,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			CreditCardT.CreateCard(patNum,50,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			//Change payplan version to version 1
			Prefs.UpdateInt(PrefName.PayPlansVersion,1);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargerator charger=new RecurringChargeratorTest(_log,true);
			RecurringChargeData data=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum).FirstOrDefault();
			Assert.IsNotNull(data);
			//PayPlanVersion 1 Scenario 4: Pay plan due more than repeat amount
			Assert.IsFalse(data.RecurringCharge.FamBal!=0
				|| data.RecurringCharge.PayPlanDue!=150
				|| data.RecurringCharge.TotalDue!=150
				|| data.RecurringCharge.ChargeAmt!=50);
		}

		///<summary>Pay plan due more than repeat amount.</summary> 
		[TestMethod]
		public void RecurringCharges_PaymentPlan_Version2_MoreThanRepeatAmt() {
			//Setup
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			long patNum=pat.PatNum;
			PayPlan payPlan=PayPlanT.CreatePayPlan(patNum,250,50,DateTime.Today.AddMonths(-12),pat.PriProv);
			PaymentT.MakePayment(patNum,100,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			CreditCardT.CreateCard(patNum,50,DateTime.Today.AddMonths(-3),payPlan.PayPlanNum);
			//Change payplan version to version 2
			Prefs.UpdateInt(PrefName.PayPlansVersion,2);
			Prefs.RefreshCache();
			Ledgers.ComputeAging(0,DateTime.Today);
			RecurringChargerator charger=new RecurringChargeratorTest(_log,true);
			RecurringChargeData data=charger.FillCharges(new List<Clinic> {}).FindAll(x => x.RecurringCharge.PatNum==pat.PatNum).FirstOrDefault();
			Assert.IsNotNull(data);
			//PayPlanVersion 2 Scenario 4: Pay plan due more than repeat amount
			Assert.IsFalse(data.RecurringCharge.FamBal!=150
				|| data.RecurringCharge.PayPlanDue!=150
				|| data.RecurringCharge.TotalDue!=150
				|| data.RecurringCharge.ChargeAmt!=50);
		}

		private DataTable CreateRecurringChargeRow(DateTime dateLatestPayment,string chargeFrequency,decimal chargeAmt,DateTime dateStart) {
			DataTable table=new DataTable();
			table.Columns.Add("LatestPayment");
			table.Columns.Add("ChargeFrequency");
			table.Columns.Add("ChargeAmt");
			table.Columns.Add("DateStart");
			table.Columns.Add("RecurringChargeDate");
			DataRow row=table.NewRow();
			row["LatestPayment"]=POut.DateT(dateLatestPayment,false);
			row["ChargeFrequency"]=chargeFrequency;
			row["ChargeAmt"]=chargeAmt.ToString();
			row["DateStart"]=POut.DateT(dateStart,false);
			table.Rows.Add(row);
			return table;
		}

		///<summary>Generates a chargefrequency for a fixedDayOfMonth type with the list of days passed in.</summary>
		private string ToDayOfMonthFrequency(List<int> listDaysOfMonth) {
			return POut.Int((int)ChargeFrequencyType.FixedDayOfMonth)+"|"+string.Join(",",listDaysOfMonth);
		}

		///<summary>Generates a chargefrequency for a FixedWeekDay type with the day of week frequency and day of week passed in.</summary>
		private string ToWeekDayFrequency(DayOfWeekFrequency frequency,DayOfWeek day) {
			return POut.Int((int)ChargeFrequencyType.FixedWeekDay)+"|"+POut.Int((int)frequency)+"|"+POut.Int((int)day);
		}

		private void FilterRecurringChargeList(DataTable table,DateTime curDate) {
			CreditCards.FilterRecurringChargeList(table,curDate);
		}

		private class RecurringChargeratorTest:RecurringChargerator {
			public RecurringChargeratorTest(Logger.IWriteLine log,bool useXChargeClientProgram) : base(log,useXChargeClientProgram) {
				_progCur=new OpenDentBusiness.Program();//To avoid null reference exceptions
			}

			///<summary>Changes the 'now' time.</summary>
			public DateTime SetNowDateTime {
				set {
					_nowDateTime=value;
				}
			}

			public void CreatePayment(Patient patCur,RecurringChargeData recCharge,string note,double amount,string receipt) {
				base.CreatePayment(patCur,recCharge,note,amount,receipt,CreditCardSource.None);
			}
		}
	}
}
