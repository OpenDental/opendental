using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Bson;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitTestsCore;

namespace UnitTests.DiscountPlanTests {
	[TestClass]
	public class DiscountPlanTests:TestBase {
			//_changed|=Prefs.UpdateString(PrefName.DiscountPlanExamCodes,textDiscountExamCodes.Text);
			//_changed|=Prefs.UpdateString(PrefName.DiscountPlanPerioCodes,textDiscountPerioCodes.Text);
			//_changed|=Prefs.UpdateString(PrefName.DiscountPlanProphyCodes,textDiscountProphyCodes.Text);
			//_changed|=Prefs.UpdateString(PrefName.DiscountPlanFluorideCodes,textDiscountFluorideCodes.Text);
			//_changed|=Prefs.UpdateString(PrefName.DiscountPlanXrayCodes,textDiscountXrayCodes.Text);
			//_changed|=Prefs.UpdateString(PrefName.DiscountPlanLimitedCodes,textDiscountLimitedCodes.Text);
			//_changed|=Prefs.UpdateString(PrefName.DiscountPlanPACodes,textDiscountPACodes.Text);

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_OneYearSegment_DoesntExceed() {
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100);
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100);
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100);
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100);
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode);

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_OneYearSegment_DoesExceed() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_OneYearSegment_NotChecked() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_MultipleSegments_AllDontExceed() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_MultiplsSegment_SomeExceed() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_MultiplsSegment_AllExceed() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_OneYearSegment_DoesntExceed() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_OneYearSegment_DoesExceed() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_OneYearSegment_NotChecked() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_MultipleSegments_AllDontExceed() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_MultiplsSegment_SomeExceed() {

		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_MultiplsSegment_AllExceed() {

		}
	}
}
