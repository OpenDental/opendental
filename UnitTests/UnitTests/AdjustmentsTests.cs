using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Adjustments_Tests {
	[TestClass]
	public class AdjustmentsTests:TestBase {
		protected static Provider _salesTaxProvider=new Provider();
		protected static long _salesTaxProviderOld;
		protected static Provider _defaultPracticeProvider=new Provider();
		protected static long _defaultPracticeProviderOld;
		protected static Provider _clinicProv=new Provider();

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		protected static void SetupTest() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			AdjustmentT.ClearAdjustmentTable();
			_salesTaxProviderOld=PrefC.GetLong(PrefName.SalesTaxDefaultProvider);
			_salesTaxProvider.ProvNum=Providers.Insert(new Provider());
			_defaultPracticeProviderOld=PrefC.GetLong(PrefName.PracticeDefaultProv);
			_defaultPracticeProvider.ProvNum=Providers.Insert(new Provider());
			_clinicProv.ProvNum=Providers.Insert(new Provider());
			Providers.RefreshCache();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		protected static void TearDownTest() {
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,_defaultPracticeProviderOld);
			Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,_salesTaxProviderOld);
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		#region CreateAdjustmentForSalesTax
		[TestMethod]
		///<summary>Tests that non-taxable procedures are skipped.</summary>
		public void Adjustments_CreateAdjustmentForSalesTax_SkipsNonTaxedProcedures() {
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",10.00);
			ProcedureCodeT.SetIsTaxed("D1110",false);
			ProcedureCodes.RefreshCache();
			Adjustments.CreateAdjustmentForSalesTax(proc);
			var test=Adjustments.GetSalesTaxForProc(proc.ProcNum);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(0,listAdjustments.Count);
		}

		[TestMethod]
		///<summary>Tests that the SalesTaxDefaultProvider is used on an automatic run if set.</summary>
		public void Adjustments_CreateAdjustmentForSalesTax_UsesSalesTaxDefaultProvider() {
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",10.00);
			ProcedureCodeT.SetIsTaxed("D0120");
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,_defaultPracticeProvider.ProvNum);
			Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,_salesTaxProvider.ProvNum);
			Adjustments.CreateAdjustmentForSalesTax(proc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
			Assert.AreEqual(_salesTaxProvider.ProvNum,listAdjustments[0].ProvNum);
		}

		[TestMethod]
		///<summary>Tests that if clinics are enabled and has a default provider, it will use that
		///over the practice default provider.</summary>
		public void Adjustments_CreateAdjustmentForSalesTax_UsesClinicProvIfNoSalesTaxDefaultProv() {
			Clinic clinic=ClinicT.CreateClinic();
			clinic.DefaultProv=_clinicProv.ProvNum;
			Clinics.Update(clinic);
			Clinics.RefreshCache();
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",10.00);
			ProcedureCodeT.SetIsTaxed("D0120");
			Procedure updatedProc=proc.Copy();
			updatedProc.ClinicNum=clinic.ClinicNum;
			Procedures.Update(updatedProc,proc);
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,_defaultPracticeProvider.ProvNum);
			Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,0);
			Adjustments.CreateAdjustmentForSalesTax(proc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
			Assert.AreEqual(_clinicProv.ProvNum,listAdjustments[0].ProvNum);
		}

		[TestMethod]
		///<summary>Tests that it will use the practice default provider if it's an automatic run
		///and there is no clinics set up / clinic default provider.</summary>
		public void Adjustments_CreateAdjustmentForSalesTax_UsesPracticeProvIfNoSalesTaxOrClinicDefaultProv() {
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",10.00);
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,_defaultPracticeProvider.ProvNum);
			Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,0);
			ProcedureCodeT.SetIsTaxed("D0120");
			Adjustments.CreateAdjustmentForSalesTax(proc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
			Assert.AreEqual(_defaultPracticeProvider.ProvNum,listAdjustments[0].ProvNum);
		}
		#endregion CreateAdjustmentForSalesTax

		#region Adjustments_AddSalesTaxIfNoneExists
		///<summary>Tests that when using this method, only one instance of sales tax will be created.</summary>
		[TestMethod]
		public void Adjustments_AddSalesTaxIfNoneExists_DoesNotCreateMultipleAdjustments() {
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",0);
			ProcedureCodeT.SetIsTaxed("D0120");
			Adjustments.AddSalesTaxIfNoneExists(proc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
			Adjustments.AddSalesTaxIfNoneExists(proc);
			listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
		}

		[TestMethod]
		public void Adjustments_AddSalesTaxIfNoneExists_DoesNotAddSalesTaxIfProcHasOrthoLink() {
			Patient pat=PatientT.CreatePatient();
			Procedure bandingProc=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.C,"",0);
			ProcedureCodeT.SetIsTaxed("D8080");
			OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,bandingProc);
			Adjustments.AddSalesTaxIfNoneExists(bandingProc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(bandingProc.ProcNum);
			Assert.AreEqual(listAdjustments.Count,0);
		}
		#endregion

	}
}
