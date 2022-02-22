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
			DataTable table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),false,DateTime.Now.AddDays(-10),DateTime.Now,false,false);
			Assert.IsNotNull(table);
			Assert.IsTrue(table.Rows.Count > 0);
			Assert.IsTrue(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
			//Run the procs not billed report with "includeMedProcs" set to true.
			//The patient should be returned due to the dental insurance estimates OR the fact that they have medical insurance.
			table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),true,DateTime.Now.AddDays(-10),DateTime.Now,false,false);
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
			DataTable table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),false,DateTime.Now.AddDays(-10),DateTime.Now,false,false);
			Assert.IsNotNull(table);
			Assert.IsFalse(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
			//Run the procs not billed report with "includeMedProcs" set to true.
			//The patient should be returned due to the medical insurance estimates.
			table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),true,DateTime.Now.AddDays(-10),DateTime.Now,false,false);
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
			//The patient should not be returned when 'showProcsBeforeIns' is set to false.
			DataTable table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),false,DateTime.Now.AddDays(-10),DateTime.Now,false,false);
			Assert.IsNotNull(table);
			Assert.IsFalse(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
			//The patient should be returned when 'showProcsBeforeIns' is set to true.
			table=RpProcNotBilledIns.GetProcsNotBilled(new List<long>(),true,DateTime.Now.AddDays(-10),DateTime.Now,true,false);
			Assert.IsNotNull(table);
			Assert.IsTrue(table.Rows.Count > 0);
			Assert.IsTrue(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Contains(patient.PatNum));
		}

	}
}
