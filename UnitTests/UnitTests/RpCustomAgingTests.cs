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

namespace UnitTests.RpCustomAgingTests_Tests {
	[TestClass]
	public class RpCustomAgingTests:TestBase {

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			AdjustmentT.ClearAdjustmentTable();
			ClaimT.ClearClaimTable();
			ClaimProcT.ClearClaimProcTable();
			PatientT.ClearPatientTable();
			PayPlanT.ClearPayPlanTable();
			PayPlanChargeT.ClearPayPlanChargeTable();
			ProcedureT.ClearProcedureTable();
		}

		[TestMethod]
		public void RpCustomAging_GetAgingList_AgeOfAccountAny() {
			InitializePatientData();
			AgingOptions agingOptions=GetAgingOptions(ageAccount:AgeOfAccount.Any);
			List<AgingPat> listAgingPats=RpCustomAging.GetAgingList(agingOptions);
			Assert.AreEqual(4,listAgingPats.Count);
			Assert.IsTrue(listAgingPats.Count(x => x.BalZeroThirty  > 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalThirtySixty > 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalSixtyNinety > 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalOverNinety  > 0)==1);
		}

		[TestMethod]
		public void RpCustomAging_GetAgingList_AgeOfAccountOver30() {
			InitializePatientData();
			AgingOptions agingOptions=GetAgingOptions(ageAccount:AgeOfAccount.Over30);
			List<AgingPat> listAgingPats=RpCustomAging.GetAgingList(agingOptions);
			Assert.AreEqual(3,listAgingPats.Count);
			Assert.IsTrue(listAgingPats.Count(x => x.BalZeroThirty  > 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalThirtySixty > 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalSixtyNinety > 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalOverNinety  > 0)==1);
		}

		[TestMethod]
		public void RpCustomAging_GetAgingList_AgeOfAccountOver60() {
			InitializePatientData();
			AgingOptions agingOptions=GetAgingOptions(ageAccount:AgeOfAccount.Over60);
			List<AgingPat> listAgingPats=RpCustomAging.GetAgingList(agingOptions);
			Assert.AreEqual(2,listAgingPats.Count);
			Assert.IsTrue(listAgingPats.Count(x => x.BalZeroThirty  > 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalThirtySixty > 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalSixtyNinety > 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalOverNinety  > 0)==1);
		}

		[TestMethod]
		public void RpCustomAging_GetAgingList_AgeOfAccountOver90() {
			InitializePatientData();
			AgingOptions agingOptions=GetAgingOptions(ageAccount:AgeOfAccount.Over90);
			List<AgingPat> listAgingPats=RpCustomAging.GetAgingList(agingOptions);
			Assert.AreEqual(1,listAgingPats.Count);
			Assert.IsTrue(listAgingPats.Count(x => x.BalZeroThirty  > 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalThirtySixty > 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalSixtyNinety > 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalOverNinety  > 0)==1);
		}

		[TestMethod]
		public void RpCustomAging_GetAgingList_AgeOfAccountAnyAgeCredits() {
			InitializePatientData(hasNegativeBalance:true);
			AgingOptions agingOptions=GetAgingOptions(ageAccount:AgeOfAccount.Any,ageCredits:true,negativeBalOptions:AgingOptions.NegativeBalAgingOptions.ShowOnly);
			List<AgingPat> listAgingPats=RpCustomAging.GetAgingList(agingOptions);
			Assert.AreEqual(4,listAgingPats.Count);
			Assert.IsTrue(listAgingPats.Count(x => x.BalZeroThirty  < 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalThirtySixty < 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalSixtyNinety < 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalOverNinety  < 0)==1);
		}

		[TestMethod]
		public void RpCustomAging_GetAgingList_AgeOfAccountOver30AgeCredits() {
			InitializePatientData(hasNegativeBalance:true);
			AgingOptions agingOptions=GetAgingOptions(ageAccount:AgeOfAccount.Over30,ageCredits:true,negativeBalOptions:AgingOptions.NegativeBalAgingOptions.ShowOnly);
			List<AgingPat> listAgingPats=RpCustomAging.GetAgingList(agingOptions);
			Assert.AreEqual(3,listAgingPats.Count);
			Assert.IsTrue(listAgingPats.Count(x => x.BalZeroThirty  < 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalThirtySixty < 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalSixtyNinety < 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalOverNinety  < 0)==1);
		}

		[TestMethod]
		public void RpCustomAging_GetAgingList_AgeOfAccountOver60AgeCredits() {
			InitializePatientData(hasNegativeBalance:true);
			AgingOptions agingOptions=GetAgingOptions(ageAccount:AgeOfAccount.Over60,ageCredits:true,negativeBalOptions:AgingOptions.NegativeBalAgingOptions.ShowOnly);
			List<AgingPat> listAgingPats=RpCustomAging.GetAgingList(agingOptions);
			Assert.AreEqual(2,listAgingPats.Count);
			Assert.IsTrue(listAgingPats.Count(x => x.BalZeroThirty  < 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalThirtySixty < 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalSixtyNinety < 0)==1);
			Assert.IsTrue(listAgingPats.Count(x => x.BalOverNinety  < 0)==1);
		}

		[TestMethod]
		public void RpCustomAging_GetAgingList_AgeOfAccountOver90AgeCredits() {
			InitializePatientData(hasNegativeBalance:true);
			AgingOptions agingOptions=GetAgingOptions(ageAccount:AgeOfAccount.Over90,ageCredits:true,negativeBalOptions:AgingOptions.NegativeBalAgingOptions.ShowOnly);
			List<AgingPat> listAgingPats=RpCustomAging.GetAgingList(agingOptions);
			Assert.AreEqual(1,listAgingPats.Count);
			Assert.IsTrue(listAgingPats.Count(x => x.BalZeroThirty  < 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalThirtySixty < 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalSixtyNinety < 0)==0);
			Assert.IsTrue(listAgingPats.Count(x => x.BalOverNinety  < 0)==1);
		}

		private void InitializePatientData(bool hasNegativeBalance=false,DateTime dateToday=default) {
			double balance=100;
			if(hasNegativeBalance) {
				balance*=-1;
			}
			if(dateToday==DateTime.MinValue) {
				dateToday=DateTime.Today;
			}
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("CUSTAGE");
			//Create a patient with a balance within 0-30 days.
			Patient patientWithin30=PatientT.CreatePatient();
			ProcedureT.CreateProcedure(patientWithin30,procedureCode.ProcCode,ProcStat.C,"",balance,procDate:dateToday.AddDays(-10));
			//Create a patient with a balance over 30 days.
			Patient patientOver30=PatientT.CreatePatient();
			ProcedureT.CreateProcedure(patientOver30,procedureCode.ProcCode,ProcStat.C,"",balance,procDate:dateToday.AddDays(-40));
			//Create a patient with a balance over 60 days.
			Patient patientOver60=PatientT.CreatePatient();
			ProcedureT.CreateProcedure(patientOver60,procedureCode.ProcCode,ProcStat.C,"",balance,procDate:dateToday.AddDays(-70));
			//Create a patient with a balance over 90 days.
			Patient patientOver90=PatientT.CreatePatient();
			ProcedureT.CreateProcedure(patientOver90,procedureCode.ProcCode,ProcStat.C,"",balance,procDate:dateToday.AddDays(-100));
		}

		private AgingOptions GetAgingOptions(AgeOfAccount ageAccount=AgeOfAccount.Any,bool ageCredits=false,AgingOptions.AgingInclude agingInc=AgingOptions.AgingInclude.None,
			DateTime dateAsOf=default,bool excludeArchive=false,bool excludeBadAddress=false,bool excludeInactive=false,
			AgingOptions.FamilyGrouping famGroup=AgingOptions.FamilyGrouping.Family,List<Def> listBillTypes=null,List<Clinic> listClins=null,List<Provider> listProvs=null,
			AgingOptions.NegativeBalAgingOptions negativeBalOptions=AgingOptions.NegativeBalAgingOptions.Exclude,PPOWriteoffDateCalc writeoffOptions=PPOWriteoffDateCalc.InsPayDate)
		{
			if(dateAsOf==DateTime.MinValue) {
				dateAsOf=DateTime.Today;
			}
			if(agingInc==AgingOptions.AgingInclude.None) {
				agingInc=AgingOptions.AgingInclude.ProcedureFees
					| AgingOptions.AgingInclude.Adjustments
					| AgingOptions.AgingInclude.PayPlanCharges
					| AgingOptions.AgingInclude.PayPlanCredits
					| AgingOptions.AgingInclude.PatPayments
					| AgingOptions.AgingInclude.InsPayments
					| AgingOptions.AgingInclude.Writeoffs;
			}
			AgingOptions agingOptions=new AgingOptions();
			agingOptions.AgeAccount=ageAccount;
			agingOptions.AgeCredits=ageCredits;
			agingOptions.AgingInc=agingInc;
			agingOptions.DateAsOf=dateAsOf;
			agingOptions.ExcludeArchive=excludeArchive;
			agingOptions.ExcludeBadAddress=excludeBadAddress;
			agingOptions.ExcludeInactive=excludeInactive;
			agingOptions.FamGroup=AgingOptions.FamilyGrouping.Family;
			agingOptions.ListBillTypes=listBillTypes;
			agingOptions.ListClins=listClins;
			agingOptions.ListProvs=listProvs;
			agingOptions.NegativeBalOptions=negativeBalOptions;
			agingOptions.WriteoffOptions=writeoffOptions;
			return agingOptions;
		}

	}
}
