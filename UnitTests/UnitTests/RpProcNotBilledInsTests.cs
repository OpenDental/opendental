using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.RpProcNotBilledIns_Tests {
	[TestClass]
	public class RpProcNotBilledInsTests:TestBase {

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

		///<summary></summary>
		[TestMethod]
		public void RpProcNotBilledIns_GetProcsNotBilled_DentalAndMedicalIns() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("T7781");
			//Create a primary dental insurance plan
			InsuranceInfo insuranceInfo=InsuranceT.AddInsurance(patient,carrier.CarrierName,ordinal: 1);
			insuranceInfo.AddBenefit(BenefitT.CreatePercentForProc(insuranceInfo.PriInsPlan.PlanNum,procedureCode.CodeNum,80));
			//Create a secondary medical insurance plan
			insuranceInfo.AddInsurance(patient,carrier.CarrierName,ordinal: 2,isMedical: true);
			insuranceInfo.AddBenefit(BenefitT.CreatePercentForProc(insuranceInfo.MedInsPlan.PlanNum,procedureCode.CodeNum,60));
			Procedure procedure=ProcedureT.CreateProcedure(patient,procedureCode.ProcCode,ProcStat.TP,"",55,procDate: DateTime.Now.AddDays(-3));
			ProcedureT.ComputeEstimates(patient,insuranceInfo);
			ProcedureT.SetComplete(procedure,patient,insuranceInfo);
			//Run the procs not billed report with "includeMedProcs" set to false.
			//The patient should be returned due to the dental insurance estimates.
			DataTable table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),false,DateTime.Now.AddDays(-10),DateTime.Now,EnumShowProcsBeforeIns.None,false);
			Assert.IsNotNull(table);
			Assert.IsTrue(table.Rows.Count > 0);
			Assert.IsTrue(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
			//Run the procs not billed report with "includeMedProcs" set to true.
			//The patient should be returned due to the dental insurance estimates OR the fact that they have medical insurance.
			table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),true,DateTime.Now.AddDays(-10),DateTime.Now,EnumShowProcsBeforeIns.None,false);
			Assert.IsNotNull(table);
			Assert.IsTrue(table.Rows.Count > 0);
			Assert.IsTrue(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
		}

		///<summary></summary>
		[TestMethod]
		public void RpProcNotBilledIns_GetProcsNotBilled_MedicalInsOnly() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("T7782");
			//Create a primary medical insurance plan
			InsuranceInfo insuranceInfo=InsuranceT.AddInsurance(patient,carrier.CarrierName,ordinal: 1,isMedical: true);
			insuranceInfo.AddBenefit(BenefitT.CreatePercentForProc(insuranceInfo.MedInsPlan.PlanNum,procedureCode.CodeNum,80));
			Procedure procedure=ProcedureT.CreateProcedure(patient,procedureCode.ProcCode,ProcStat.TP,"",55,procDate: DateTime.Now.AddDays(-3));
			ProcedureT.ComputeEstimates(patient,insuranceInfo);
			ProcedureT.SetComplete(procedure,patient,insuranceInfo);
			//Run the procs not billed report with "includeMedProcs" set to false.
			//The patient should not be returned due to not having any dental insurance estimates.
			DataTable table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),false,DateTime.Now.AddDays(-10),DateTime.Now,EnumShowProcsBeforeIns.None,false);
			Assert.IsNotNull(table);
			Assert.IsFalse(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
			//Run the procs not billed report with "includeMedProcs" set to true.
			//The patient should be returned due to the medical insurance estimates.
			table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),true,DateTime.Now.AddDays(-10),DateTime.Now,EnumShowProcsBeforeIns.None,false);
			Assert.IsNotNull(table);
			Assert.IsTrue(table.Rows.Count > 0);
			Assert.IsTrue(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
		}

		///<summary></summary>
		[TestMethod]
		public void RpProcNotBilledIns_GetProcsNotBilled_ShowProcsBeforeIns() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("T7782");
			Procedure procedure=ProcedureT.CreateProcedure(patient,procedureCode.ProcCode,ProcStat.C,"",55,procDate: DateTime.Now.AddDays(-3));
			//Add insurance after the procedure has been set complete.
			InsuranceInfo insuranceInfo=InsuranceT.AddInsurance(patient,carrier.CarrierName,ordinal: 1,isMedical: true);
			//The patient should not be returned when 'showProcsBeforeIns' is set to None.
			DataTable table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),false,DateTime.Now.AddDays(-10),DateTime.Now,EnumShowProcsBeforeIns.None,false);
			Assert.IsNotNull(table);
			Assert.IsFalse(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
			//The patient should be returned when 'showProcsBeforeIns' is set to All.
			table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),true,DateTime.Now.AddDays(-10),DateTime.Now,EnumShowProcsBeforeIns.All,false);
			Assert.IsNotNull(table);
			Assert.IsTrue(table.Rows.Count > 0);
			Assert.IsTrue(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
		}

		///<summary></summary>
		[TestMethod]
		public void RpProcNotBilledIns_GetProcsNotBilled_ShowProcsBeforeIns_WithinEffective() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient patient=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("T7782");
			//Procedure completed 3 months ago.
			Procedure procedure=ProcedureT.CreateProcedure(patient,procedureCode.ProcCode,ProcStat.C,"",55,procDate: DateTime.Today.AddMonths(-3));
			//Add insurance after the procedure has been set complete.
			InsuranceInfo insuranceInfo=InsuranceT.AddInsurance(patient,carrier.CarrierName,ordinal:1);
			Carrier carrier2=CarrierT.CreateCarrier(suffix);
			insuranceInfo.AddInsurance(patient,carrier2.CarrierName,ordinal:2);
			//The patient should not be returned when 'showProcsBeforeIns' is set to None, because the procedure was added before insurance.
			VerifyReport(patient.PatNum,EnumShowProcsBeforeIns.None,hasProc:false);
			//The patient should be returned when 'showProcsBeforeIns' is set to Effective
			//and no Effective or Termination dates have been entered.
			VerifyReport(patient.PatNum,EnumShowProcsBeforeIns.Effectve,hasProc:true);
			//The patient should not be returned when 'showProcsBeforeIns' is set to Effective
			//and the procedure is before the effective dates of all plans for the patient.
			InsSubT.UpdateEffectiveDates(insuranceInfo.PriInsSub,DateTime.Today.AddMonths(-2),DateTime.MinValue);
			InsSubT.UpdateEffectiveDates(insuranceInfo.SecInsSub,DateTime.Today.AddMonths(-1),DateTime.MinValue);
			VerifyReport(patient.PatNum,EnumShowProcsBeforeIns.Effectve,hasProc:false);
			//The patient should be returned when 'showProcsBeforeIns' is set to Effective
			//and the procedure is after at least one plan effective date and no termination dates have been entered.
			InsSubT.UpdateEffectiveDates(insuranceInfo.PriInsSub,DateTime.Today.AddMonths(-6),DateTime.MinValue);
			InsSubT.UpdateEffectiveDates(insuranceInfo.SecInsSub,DateTime.Today.AddMonths(-1),DateTime.MinValue);
			VerifyReport(patient.PatNum,EnumShowProcsBeforeIns.Effectve,hasProc:true);
			//The patient should not be returned when 'showProcsBeforeIns' is set to Effective
			//and the procedure is after the termination dates of all plans for the patient.
			InsSubT.UpdateEffectiveDates(insuranceInfo.PriInsSub,DateTime.MinValue,DateTime.Today.AddMonths(-4));
			InsSubT.UpdateEffectiveDates(insuranceInfo.SecInsSub,DateTime.MinValue,DateTime.Today.AddMonths(-5));
			VerifyReport(patient.PatNum,EnumShowProcsBeforeIns.Effectve,hasProc:false);
			//The patient should be returned when 'showProcsBeforeIns' is set to Effective
			//and the procedure is before at least one plan termination date and no effective dates have been entered.
			InsSubT.UpdateEffectiveDates(insuranceInfo.PriInsSub,DateTime.MinValue,DateTime.Today.AddMonths(-2));
			InsSubT.UpdateEffectiveDates(insuranceInfo.SecInsSub,DateTime.MinValue,DateTime.Today.AddMonths(-4));
			VerifyReport(patient.PatNum,EnumShowProcsBeforeIns.Effectve,hasProc:true);
			//The patient should not be returned when 'showProcsBeforeIns' is set to Effective
			//and the procedure is in the gap between effective dates for the patient's plans.
			InsSubT.UpdateEffectiveDates(insuranceInfo.PriInsSub,DateTime.Today.AddMonths(-11),DateTime.Today.AddMonths(-7));
			InsSubT.UpdateEffectiveDates(insuranceInfo.SecInsSub,DateTime.Today.AddMonths(-1),DateTime.Today.AddMonths(3));
			VerifyReport(patient.PatNum,EnumShowProcsBeforeIns.Effectve,hasProc:false);
			//The patient should be returned when 'showProcsBeforeIns' is set to Effective
			//and the procedure is within the effective dates of at least one of the patient's plans.
			InsSubT.UpdateEffectiveDates(insuranceInfo.PriInsSub,DateTime.Today.AddMonths(-5),DateTime.Today.AddMonths(-2));
			InsSubT.UpdateEffectiveDates(insuranceInfo.SecInsSub,DateTime.Today.AddMonths(-1),DateTime.Today.AddMonths(1));
			VerifyReport(patient.PatNum,EnumShowProcsBeforeIns.Effectve,hasProc:true);
		}

		///<summary>Set hasProc to true if the report is expected to return a procedure.</summary>
		private void VerifyReport(long patNum,EnumShowProcsBeforeIns showProcsBeforeIns,bool hasProc) {
			DataTable table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),false,DateTime.Today.AddYears(-1).AddDays(1),DateTime.Today,showProcsBeforeIns,false);
			Assert.IsNotNull(table);
			Assert.AreEqual(hasProc,table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patNum));
		}

	}
}
