using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.AccountModules_Tests {
	[TestClass]
	public class AccountModulesTests:TestBase {
		private Statement _stmt=new Statement {
			SinglePatient=true,
			Intermingled=false,
			IsInvoice=false,
			StatementType=StmtType.NotSet,
			DateRangeFrom=DateTime.MinValue,
			DateRangeTo=DateTime.MaxValue,
		};

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
		public void AccountModules_GetListUnpaidAccountCharges_PreferSplitsCur() {
			//Users could be in the middle of editing a payment and its attached splits.
			//The SplitAmt of any splits that have already been inserted into the database should utilize the value on the splits in memory.
			//This is so that we don't show strange values to the user while they are in the middle of manually manipulating a payment.
			//E.g. A PaySplit is in the database for $1800.  The user opens up the payment window and changes it to $1200
			//in order to create a new split for $600 to something else.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			Procedure procedure=ProcedureT.CreateProcedure(patient,"PSC20",ProcStat.C,"",2000);
			PaySplit paySplit=PaySplitT.CreateOne(patient.PatNum,1800,0,0,procNum:procedure.ProcNum);
			PaySplit paySplitCopy=paySplit.Copy();
			//Act like the user just changed the value of the pay split in memory to $1200
			paySplitCopy.SplitAmt=1200;
			List<AccountEntry> listAccountEntries=AccountModules.GetListUnpaidAccountCharges(new List<Procedure>() { procedure },new List<Adjustment>(),
				new List<PaySplit>() { paySplit },new List<ClaimProc>(),new List<PayPlanCharge>(),new List<ClaimProc>(),CreditCalcType.AllocatedOnly,
				new List<PaySplit>() { paySplitCopy });
			Assert.AreEqual(1,listAccountEntries.Count);
			Assert.AreEqual(2000,listAccountEntries.First().AmountOriginal);
			Assert.AreEqual(800,listAccountEntries.First().AmountEnd);//Should be $800 instead of $200.
		}

		[TestMethod]
		public void AccountModules_GetAccount_DynamicPaymentPlanExplicitAdjustment() {
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			AccountingFamilies families=new AccountingFamilies(suffix);
			Patient pat=families.MyFamily.Guarantor;
			//Create a procedure and explicitly attach an adjustment to said procedure.
			Procedure proc=ProcedureT.CreateProcedure(pat,"DPPIA1",ProcStat.C,"",100,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Adjustments that are explicitly linked to procedures should inflate the value of the procedure.
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,50,procNum: proc.ProcNum,provNum: provNum);
			//Create a dynamic payment plan that is only attached to the procedure.
			DateTime datePayPlan=DateTime.Today;
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,datePayPlan,0,0,10,new List<Procedure>(){ proc },new List<Adjustment>{ },
				frequency:PayPlanFrequency.Monthly);
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert that the payment plan charge credit entry is for the amount of the procedure and that includes the adjustment amount.
			Assert.AreEqual(3,account.Rows.Count);
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"0"
				&& x["ProcNum"].ToString()==$"{proc.ProcNum}"
				&& x["PayPlanNum"].ToString()==$"0"
				&& x["chargesDouble"].ToString()==$"{proc.ProcFee}"));
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"{adj.AdjNum}"
				&& x["ProcNum"].ToString()==$"0"
				&& x["PayPlanNum"].ToString()==$"0"
				&& x["chargesDouble"].ToString()==$"{adj.AdjAmt}"));
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"0"
				&& x["ProcNum"].ToString()==$"0"
				&& x["PayPlanNum"].ToString()==$"{dynamicPayPlan.PayPlanNum}"
				&& x["creditsDouble"].ToString()==$"150"));
		}

		[TestMethod]
		public void AccountModules_GetAccount_DynamicPaymentPlanImplicitAdjustmentIgnore() {
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			long provNum2=ProviderT.CreateProvider($"{suffix}-2");
			AccountingFamilies families=new AccountingFamilies(suffix);
			Patient pat=families.MyFamily.Guarantor;
			//Create a procedure and implicitly attach an adjustment to said procedure.
			Procedure proc=ProcedureT.CreateProcedure(pat,"DPPIA1",ProcStat.C,"",100,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Adjustments that are implicitly linked to procedures should NOT inflate the value of the procedure.
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,50,procNum: proc.ProcNum,provNum: provNum2);
			//Create a dynamic payment plan that is only attached to the procedure.
			DateTime datePayPlan=DateTime.Today;
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,datePayPlan,0,0,10,new List<Procedure>(){ proc },new List<Adjustment>{ },
				frequency:PayPlanFrequency.Monthly);
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert that the payment plan charge credit entry is for the amount of the procedure and that it does NOT include the adjustment amount.
			Assert.AreEqual(3,account.Rows.Count);
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"0"
				&& x["ProcNum"].ToString()==$"{proc.ProcNum}"
				&& x["PayPlanNum"].ToString()==$"0"
				&& x["chargesDouble"].ToString()==$"{proc.ProcFee}"));
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"{adj.AdjNum}"
				&& x["ProcNum"].ToString()==$"0"
				&& x["PayPlanNum"].ToString()==$"0"
				&& x["chargesDouble"].ToString()==$"{adj.AdjAmt}"));
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"0"
				&& x["ProcNum"].ToString()==$"0"
				&& x["PayPlanNum"].ToString()==$"{dynamicPayPlan.PayPlanNum}"
				&& x["creditsDouble"].ToString()==$"100"));
		}

		[TestMethod]
		public void AccountModules_GetAccount_DynamicPaymentPlanImplicitAdjustmentAttached() {
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider($"{suffix}-1");
			long provNum2=ProviderT.CreateProvider($"{suffix}-2");
			AccountingFamilies families=new AccountingFamilies(suffix);
			Patient pat=families.MyFamily.Guarantor;
			//Create a procedure and implicitly attach an adjustment to said procedure.
			Procedure proc=ProcedureT.CreateProcedure(pat,"DPPIA1",ProcStat.C,"",100,DateTime.Today.AddMonths(-1),provNum:provNum);
			//Adjustments that are implicitly linked to procedures should NOT inflate the value of the procedure.
			Adjustment adj=AdjustmentT.MakeAdjustment(pat.PatNum,50,procNum: proc.ProcNum,provNum: provNum2);
			//Create a dynamic payment plan that is only attached to the procedure.
			DateTime datePayPlan=DateTime.Today;
			PayPlan dynamicPayPlan=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,datePayPlan,0,0,10,new List<Procedure>(){ proc },new List<Adjustment>{ adj },
				frequency:PayPlanFrequency.Monthly);
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert that the payment plan charge credit entry is for the amount of the procedure and that it does NOT include the adjustment amount.
			Assert.AreEqual(4,account.Rows.Count);
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"0"
				&& x["ProcNum"].ToString()==$"{proc.ProcNum}"
				&& x["PayPlanNum"].ToString()==$"0"
				&& x["chargesDouble"].ToString()==$"{proc.ProcFee}"));
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"{adj.AdjNum}"
				&& x["ProcNum"].ToString()==$"0"
				&& x["PayPlanNum"].ToString()==$"0"
				&& x["chargesDouble"].ToString()==$"{adj.AdjAmt}"));
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"0"
				&& x["ProcNum"].ToString()==$"0"
				&& x["PayPlanNum"].ToString()==$"{dynamicPayPlan.PayPlanNum}"
				&& x["creditsDouble"].ToString()==$"100"));
			Assert.AreEqual(1,account.Select().Count(x => x["AdjNum"].ToString()==$"0"
				&& x["ProcNum"].ToString()==$"0"
				&& x["PayPlanNum"].ToString()==$"{dynamicPayPlan.PayPlanNum}"
				&& x["creditsDouble"].ToString()==$"50"));
		}

		///<summary>Payment made on behalf of a patient in a completely different family.  A row should be included in the patient's account.</summary>
		[TestMethod]
		public void AccountModules_GetAccount_PaidToDifferentFamily() {
			//Arrange
			AccountingFamilies families=new AccountingFamilies(MethodBase.GetCurrentMethod().Name);
			Pay pay=MakePayment(payer:families.MyFamily.Guarantor,100,to:families.UnrelatedFamily.Guarantor);
			//Act
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert
			Assert.AreEqual(1,account.Rows.Count);
			Assert.AreEqual(families.MyFamily.Guarantor.PatNum,PIn.Long(account.Rows[0]["PatNum"].ToString()));
			Assert.AreEqual(pay.Payment.PayNum,PIn.Long(account.Rows[0]["PayNum"].ToString()));			
			Assert.AreEqual(0,PIn.Decimal(account.Rows[0]["credits"].ToString()));//Since split to another family, the credits should be $0 for this pat.
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains("(split to another family)"));
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains(pay.Payment.PayAmt.ToString("n")));
		}
		
		///<summary>Payment made on behalf of a patient in the same superfamily, but not in the payer's family.  A row should be included in the patient's account.</summary>
		[TestMethod]
		public void AccountModules_GetAccount_PaidToSuperfamily() {
			//Arrange
			AccountingFamilies families=new AccountingFamilies(MethodBase.GetCurrentMethod().Name);
			Pay pay=MakePayment(payer:families.MyFamily.Guarantor,100,to:families.ExtendedFamily.Guarantor);
			//Act
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert
			Assert.AreEqual(1,account.Rows.Count);
			Assert.AreEqual(families.MyFamily.Guarantor.PatNum,PIn.Long(account.Rows[0]["PatNum"].ToString()));
			Assert.AreEqual(pay.Payment.PayNum,PIn.Long(account.Rows[0]["PayNum"].ToString()));			
			Assert.AreEqual(0,PIn.Decimal(account.Rows[0]["credits"].ToString()));//Since split to another family, the credits should be $0 for this pat.
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains("(split to another family)"));
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains(pay.Payment.PayAmt.ToString("n")));
		}
		
		///<summary>Payment made to a payment plan for a patient in a completely different family.  A row should be included in the patient's account.</summary>
		[TestMethod]
		public void AccountModules_GetAccount_PaidToPayPlanDifferentFamily() {
			//Arrange
			AccountingFamilies families=new AccountingFamilies(MethodBase.GetCurrentMethod().Name);
			double amt=100;
			Pay pay=MakePayment(payer:families.MyFamily.Guarantor,amt,to:CreatePayPlan(families.UnrelatedFamily.Guarantor,amt));
			//Act
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert
			Assert.AreEqual(1,account.Rows.Count);
			Assert.AreEqual(families.MyFamily.Guarantor.PatNum,PIn.Long(account.Rows[0]["PatNum"].ToString()));
			Assert.AreEqual(pay.Payment.PayNum,PIn.Long(account.Rows[0]["PayNum"].ToString()));			
			Assert.AreEqual(0,PIn.Decimal(account.Rows[0]["credits"].ToString()));//Since split to another family, the credits should be $0 for this pat.
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains("(split to another family)"));
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains(pay.Payment.PayAmt.ToString("n")));
		}

		///<summary>Payment made to a payment plan for a patient in the superfamily but not in the payer's family.  A row should be included in the patient's account.</summary>
		[TestMethod]
		public void AccountModules_GetAccount_PaidToPayPlanSuperfamily() {
			//Arrange
			AccountingFamilies families=new AccountingFamilies(MethodBase.GetCurrentMethod().Name);
			double amt=100;
			Pay pay=MakePayment(payer:families.MyFamily.Guarantor,amt,to:CreatePayPlan(families.ExtendedFamily.Guarantor,amt));
			//Act
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert
			Assert.AreEqual(1,account.Rows.Count);
			Assert.AreEqual(families.MyFamily.Guarantor.PatNum,PIn.Long(account.Rows[0]["PatNum"].ToString()));
			Assert.AreEqual(pay.Payment.PayNum,PIn.Long(account.Rows[0]["PayNum"].ToString()));			
			Assert.AreEqual(0,PIn.Decimal(account.Rows[0]["credits"].ToString()));//Since split to another family, the credits should be $0 for this pat.
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains("(split to another family)"));
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains(pay.Payment.PayAmt.ToString("n")));
		}

		///<summary>If a payment is split to someone in the family, and someone outside the family, only one row should display.</summary>
		[TestMethod]
		public void AccountModules_GetAccount_PaidInFamilyAndDifferentFamily() {
			//Arrange
			AccountingFamilies families=new AccountingFamilies(MethodBase.GetCurrentMethod().Name);
			Pay pay=MakePayment(payer:families.MyFamily.Guarantor,100,families.MyFamily.Guarantor,families.UnrelatedFamily.Guarantor);
			//Act
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert
			Assert.AreEqual(1,account.Rows.Count);
			Assert.AreEqual(families.MyFamily.Guarantor.PatNum,PIn.Long(account.Rows[0]["PatNum"].ToString()));
			Assert.AreEqual(pay.Payment.PayNum,PIn.Long(account.Rows[0]["PayNum"].ToString()));			
			Assert.AreEqual((decimal)pay.ListPaySplits.First(x => x.PatNum==families.MyFamily.Guarantor.PatNum).SplitAmt//One split is inside the family.
				,PIn.Decimal(account.Rows[0]["credits"].ToString()));
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains("(split)"));//Indicates this payment is split (user can double click to see to whom)
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains(pay.Payment.PayAmt.ToString("n")));
		}

		///<summary>If a payment is split to someone in the family, and to a payment plan for someone outside the family, only one row should display.</summary>
		[TestMethod]
		public void AccountModules_GetAccount_PaidInFamilyAndPayPlanDifferentFamily() {
			//Arrange
			AccountingFamilies families=new AccountingFamilies(MethodBase.GetCurrentMethod().Name);
			double amt=100;
			Pay pay=MakePayment(payer:families.MyFamily.Guarantor,amt,families.MyFamily.Guarantor,CreatePayPlan(families.UnrelatedFamily.Guarantor,amt/2));
			//Act
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert
			Assert.AreEqual(1,account.Rows.Count);
			Assert.AreEqual(families.MyFamily.Guarantor.PatNum,PIn.Long(account.Rows[0]["PatNum"].ToString()));
			Assert.AreEqual(pay.Payment.PayNum,PIn.Long(account.Rows[0]["PayNum"].ToString()));			
			Assert.AreEqual((decimal)pay.ListPaySplits.First(x => x.PatNum==families.MyFamily.Guarantor.PatNum).SplitAmt//One split is inside the family.
				,PIn.Decimal(account.Rows[0]["credits"].ToString()));
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains("(split)"));//Indicates this payment is split (user can double click to see to whom)
			Assert.IsTrue(PIn.String(account.Rows[0]["description"].ToString()).Contains(pay.Payment.PayAmt.ToString("n")));
		}

		///<summary>If a payment is split to a payment plan for someone in the family, and to a payment plan for someone outside the family, only one row should display.</summary>
		[TestMethod]
		public void AccountModules_GetAccount_PaidToPayPlanInFamilyAndPayPlanDifferentFamily() {
			//Arrange
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);//Causes PayPlans to be included in results.
			AccountingFamilies families=new AccountingFamilies(MethodBase.GetCurrentMethod().Name);
			double amt=100;
			Pay pay=MakePayment(payer:families.MyFamily.Guarantor,amt,CreatePayPlan(families.MyFamily.Guarantor,amt/2),CreatePayPlan(families.UnrelatedFamily.Guarantor,amt/2));
			//Act
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert
			Assert.AreEqual(2,account.Rows.Count);
			DataRow rowPayment=account.Rows.AsEnumerable<DataRow>().First(x => PIn.Long(x["PayPlanNum"].ToString())==0 && PIn.Long(x["PayNum"].ToString())!=0);
			Assert.AreEqual(families.MyFamily.Guarantor.PatNum,PIn.Long(rowPayment["PatNum"].ToString()));
			Assert.AreEqual(pay.Payment.PayNum,PIn.Long(rowPayment["PayNum"].ToString()));			
			Assert.AreEqual((decimal)pay.ListPaySplits.First(x => x.PatNum==families.MyFamily.Guarantor.PatNum).SplitAmt//One split is inside the family.
				,PIn.Decimal(rowPayment["credits"].ToString()));
			Assert.IsTrue(PIn.String(rowPayment["description"].ToString()).Contains("(Attached to payment plan)"));
			Assert.IsTrue(PIn.String(rowPayment["description"].ToString()).Contains("(split)"));//Indicates this payment is split (user can double click to see to whom)
			Assert.IsTrue(PIn.String(rowPayment["description"].ToString()).Contains(pay.Payment.PayAmt.ToString("n")));
		}

		///<summary>If a payment is split to a payment plan for someone in the family, and to someone outside the family, only one row should display.</summary>
		[TestMethod]
		public void AccountModules_GetAccount_PaidToPayPlanInFamilyAndDifferentFamily() {
			//Arrange
			PrefT.UpdateInt(PrefName.PayPlansVersion,(int)PayPlanVersions.AgeCreditsAndDebits);//Causes PayPlans to be included in results.
			AccountingFamilies families=new AccountingFamilies(MethodBase.GetCurrentMethod().Name);
			double amt=100;
			Pay pay=MakePayment(payer:families.MyFamily.Guarantor,amt,CreatePayPlan(families.MyFamily.Guarantor,amt/2),families.UnrelatedFamily.Guarantor);
			//Act
			DataTable account=AccountModules.GetAccount(families.MyFamily.Guarantor.PatNum,_stmt).Tables["account"];
			//Assert
			Assert.AreEqual(2,account.Rows.Count);//Payment row and PayPlan row
			DataRow rowPayment=account.Rows.AsEnumerable<DataRow>().First(x => PIn.Long(x["PayPlanNum"].ToString())==0 && PIn.Long(x["PayNum"].ToString())!=0);
			Assert.AreEqual(families.MyFamily.Guarantor.PatNum,PIn.Long(rowPayment["PatNum"].ToString()));
			Assert.AreEqual(pay.Payment.PayNum,PIn.Long(rowPayment["PayNum"].ToString()));			
			Assert.AreEqual((decimal)pay.ListPaySplits.First(x => x.PatNum==families.MyFamily.Guarantor.PatNum).SplitAmt//One split is inside the family.
				,PIn.Decimal(rowPayment["credits"].ToString()));
			Assert.IsTrue(PIn.String(rowPayment["description"].ToString()).Contains("(Attached to payment plan)"));
			Assert.IsTrue(PIn.String(rowPayment["description"].ToString()).Contains("(split)"));//Indicates this payment is split (user can double click to see to whom)
			Assert.IsTrue(PIn.String(rowPayment["description"].ToString()).Contains(pay.Payment.PayAmt.ToString("n")));
		}

		private Pay MakePayment(Patient payer,double amount,params object[] to) {
			Payment payment=PaymentT.MakePaymentNoSplits(payer.PatNum,amount,payType:Defs.GetDefsForCategory(DefCat.PaymentTypes,true).First().DefNum);
			return new Pay(payment,to.Select(x => (x is Patient pat) ?
				PaySplitT.CreateOne(pat.PatNum,amount/(double)to.Length,payment.PayNum,0) 
				: (x is PayPlan payPlan) ? PaySplitT.CreateOne(payPlan.PatNum,amount/(double)to.Length,payment.PayNum,0,payPlanNum:payPlan.PayPlanNum) : null));
		}

		private  PayPlan CreatePayPlan(Patient pat,double payPlanAmt,double payPlanPayment=100,DateTime payPlanStart=default,long payPlanProvNum=0) {
			return PayPlanT.CreatePayPlan(pat.PatNum,payPlanAmt,payPlanPayment,payPlanStart,payPlanProvNum,guarantorNum:pat.Guarantor);
		}

		private class Pay {
			public Payment Payment;
			public List<PaySplit> ListPaySplits;

			public Pay(Payment payment,IEnumerable<PaySplit> listPaySplits) {
				Payment=payment;
				ListPaySplits=listPaySplits.ToList();
			}
		}

		private class AccountingFamilies {
			public List<Family> SuperFamily { get; }
			public Family MyFamily { get; }
			public Family ExtendedFamily { get; }
			public Family UnrelatedFamily { get; }

			public AccountingFamilies(string suffix) {
				Patient superfamilyHead=PatientT.CreatePatient(lName:"(Superfamily Head)"+suffix);
				Patient copy=superfamilyHead.Copy();
				superfamilyHead.SuperFamily=superfamilyHead.PatNum;
				Patients.Update(superfamilyHead,copy);
				Patient superfamilyMember=PatientT.CreatePatient(lName:"(Superfamily Member)"+suffix,guarantor:superfamilyHead.PatNum,superfamily:superfamilyHead.PatNum);
				Patient guarantor=PatientT.CreatePatient(lName:"(In Family Guarantor)"+suffix,superfamily:superfamilyHead.PatNum);
				Patient familyMember=PatientT.CreatePatient(lName:"(In Family Member)"+suffix,guarantor:guarantor.PatNum,superfamily:superfamilyHead.PatNum);
				Patient unrelatedFamilyMember=PatientT.CreatePatient(lName:"(Unrelated Family Member)"+suffix);
				MyFamily=new Family(new List<Patient> { guarantor, familyMember });
				ExtendedFamily = new Family(new List<Patient> { superfamilyHead,superfamilyMember });
				UnrelatedFamily=new Family(new List<Patient> { unrelatedFamilyMember });
				SuperFamily=new List<Family> { MyFamily, ExtendedFamily };
			}			
		}
	}
}
