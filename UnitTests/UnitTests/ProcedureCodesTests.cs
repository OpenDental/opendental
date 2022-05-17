using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.ProcedureCodes_Tests {
	[TestClass]
	public class ProcedureCodesTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		[TestMethod]
		public void ProcedureCodes_GetSubstituteCodeNum_Always() {
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(MethodBase.GetCurrentMethod().Name);
			InsPlan plan=new InsPlan();
			plan.CarrierNum=carrier.CarrierNum;
			plan.PlanType="";
			plan.CobRule=EnumCobRule.Basic;
			plan.PlanNum=InsPlans.Insert(plan);
			//Add a substitution code on the procedure level.
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2330");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2140");
			originalProcCode.SubstitutionCode="D2140";
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodeT.Update(originalProcCode);
			//Next, perform the thing you're trying to test.
			Procedure proc=ProcedureT.CreateProcedure(pat,originalProcCode.ProcCode,ProcStat.C,"9",100);//Tooth 9
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(originalProcCode.ProcCode,proc.ToothNum,plan.PlanNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(downgradeProcCode.CodeNum,subCodeNum);
		}

		[TestMethod]
		public void ProcedureCodes_GetSubstituteCodeNum_Posterior() {
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(MethodBase.GetCurrentMethod().Name);
			InsPlan plan=new InsPlan();
			plan.CarrierNum=carrier.CarrierNum;
			plan.PlanType="";
			plan.CobRule=EnumCobRule.Basic;
			plan.PlanNum=InsPlans.Insert(plan);
			//Add a substitution code on the procedure level.
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2740");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2750");
			originalProcCode.SubstitutionCode="D2750";
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Posterior;
			ProcedureCodeT.Update(originalProcCode);
			//Posterior Procedure= ToothNum 4
			Procedure proc=ProcedureT.CreateProcedure(pat,originalProcCode.ProcCode,ProcStat.C,"4",100);//Tooth 4
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(originalProcCode.ProcCode,proc.ToothNum,plan.PlanNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(downgradeProcCode.CodeNum,subCodeNum);
		}

		[TestMethod]
		public void ProcedureCodes_GetSubstituteCodeNum_InsPlanOverrideAlways() {
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(MethodBase.GetCurrentMethod().Name);
			InsPlan plan=new InsPlan();
			plan.CarrierNum=carrier.CarrierNum;
			plan.PlanType="";
			plan.CobRule=EnumCobRule.Basic;
			plan.PlanNum=InsPlans.Insert(plan);
			//Add a substitution code on the procedure level.
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2330");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2140");
			originalProcCode.SubstitutionCode="D2140";
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodeT.Update(originalProcCode);
			//Add an override for the inplan above for the originalProcCode
			ProcedureCode downgradeProcCodeForIns=ProcedureCodeT.CreateProcCode("D2150");
			SubstitutionLinkT.CreateSubstitutionLink(originalProcCode.CodeNum,downgradeProcCodeForIns.ProcCode,SubstitutionCondition.Always,plan.PlanNum);
			//Next, perform the thing you're trying to test.
			Procedure proc=ProcedureT.CreateProcedure(pat,originalProcCode.ProcCode,ProcStat.C,"9",100);//Tooth 9
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(originalProcCode.ProcCode,proc.ToothNum,plan.PlanNum);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(downgradeProcCodeForIns.CodeNum,subCodeNum);
		}

		[TestMethod]
		public void ProcedureCodes_GetSubstituteCodeNum_ProcAlwaysInsPlanOverrideNever() {
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(MethodBase.GetCurrentMethod().Name);
			InsPlan plan=new InsPlan();
			plan.CarrierNum=carrier.CarrierNum;
			plan.PlanType="";
			plan.CobRule=EnumCobRule.Basic;
			plan.PlanNum=InsPlans.Insert(plan);
			//Add a substitution code on the procedure level.
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2330");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2140");
			originalProcCode.SubstitutionCode="D2140";
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodeT.Update(originalProcCode);
			//Add an override for the inplan above for the originalProcCode to never substitute
			ProcedureCode downgradeProcCodeForIns=ProcedureCodeT.CreateProcCode("D2150");
			SubstitutionLinkT.CreateSubstitutionLink(originalProcCode.CodeNum,downgradeProcCodeForIns.ProcCode,SubstitutionCondition.Never,plan.PlanNum);
			//Next, perform the thing you're trying to test.
			Procedure proc=ProcedureT.CreateProcedure(pat,originalProcCode.ProcCode,ProcStat.C,"9",100);//Tooth 9
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(originalProcCode.ProcCode,proc.ToothNum,plan.PlanNum);
			//The ins override is set to never so it should use downgradeProcCode.CodeNum
			Assert.AreEqual(originalProcCode.CodeNum,subCodeNum);
		}

		[TestMethod]
		public void ProcedureCodes_GetSubstituteCodeNum_InsPlanOverrideNever() {
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(MethodBase.GetCurrentMethod().Name);
			InsPlan plan=new InsPlan();
			plan.CarrierNum=carrier.CarrierNum;
			plan.PlanType="";
			plan.CobRule=EnumCobRule.Basic;
			plan.PlanNum=InsPlans.Insert(plan);
			//Add a substitution code on the procedure level that has SubstitutionCondition.Never.
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2330");
			//clear out any substitution codes on this procedure
			originalProcCode.SubstitutionCode="";
			ProcedureCodeT.Update(originalProcCode);
			//Add an override for the inplan above for the originalProcCode to never substitute
			ProcedureCode downgradeProcCodeForIns=ProcedureCodeT.CreateProcCode("D2150");
			SubstitutionLinkT.CreateSubstitutionLink(originalProcCode.CodeNum,downgradeProcCodeForIns.ProcCode,SubstitutionCondition.Never,plan.PlanNum);
			//Next, perform the thing you're trying to test.
			Procedure proc=ProcedureT.CreateProcedure(pat,originalProcCode.ProcCode,ProcStat.C,"9",100);//Tooth 9
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(originalProcCode.ProcCode,proc.ToothNum,plan.PlanNum);
			//The procedure level and ins override is set SubstitutionCondition.Never so it should use originalProcCode.CodeNum
			Assert.AreEqual(originalProcCode.CodeNum,subCodeNum);
		}

		[TestMethod]
		public void ProcedureCodes_GetSubstituteCodeNum_InsPlanOverridePosterior() {
			//First, setup the test scenario.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(MethodBase.GetCurrentMethod().Name);
			InsPlan plan=new InsPlan();
			plan.CarrierNum=carrier.CarrierNum;
			plan.PlanType="";
			plan.CobRule=EnumCobRule.Basic;
			plan.PlanNum=InsPlans.Insert(plan);
			//Procedure code does not have a substitution code
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2740");
			//clear out any substitution codes on this procedure
			originalProcCode.SubstitutionCode="";
			ProcedureCodeT.Update(originalProcCode);
			//Add an override for the inplan above for the originalProcCode to substitute if posterior
			ProcedureCode downgradeProcCodeForIns=ProcedureCodeT.CreateProcCode("D2750");
			SubstitutionLinkT.CreateSubstitutionLink(originalProcCode.CodeNum,downgradeProcCodeForIns.ProcCode,SubstitutionCondition.Posterior,plan.PlanNum);
			//Posterior procedure 
			Procedure proc=ProcedureT.CreateProcedure(pat,originalProcCode.ProcCode,ProcStat.C,"4",100);//Tooth 4
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(originalProcCode.ProcCode,proc.ToothNum,plan.PlanNum);
			//The ins override is set to substitute only if posterior.
			Assert.AreEqual(downgradeProcCodeForIns.CodeNum,subCodeNum);
		}
		
		[TestMethod]
		public void ProcedureCodes_GetSubstitutionCodeNum_InsPlanOverrideNeverWithGlobalSubstituion() {
			Patient pat=PatientT.CreatePatient("test");
			Carrier carrier=CarrierT.CreateCarrier("carrier");
			InsPlan plan=new InsPlan() {
				CarrierNum=carrier.CarrierNum,
				PlanType="",
				CobRule=EnumCobRule.Basic
			};
			plan.PlanNum=InsPlans.Insert(plan);
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2740");
			originalProcCode.SubstitutionCode="D6010";
			ProcedureCodeT.Update(originalProcCode);
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2391");
			SubstitutionLinkT.CreateSubstitutionLink(originalProcCode.CodeNum,downgradeProcCode.ProcCode,SubstitutionCondition.Never,plan.PlanNum);
			Procedure proc=ProcedureT.CreateProcedure(pat,originalProcCode.ProcCode,ProcStat.C,"4",100);//Tooth 4
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(originalProcCode.ProcCode,proc.ToothNum,plan.PlanNum);
			Assert.AreEqual(originalProcCode.CodeNum,subCodeNum);
		}

		[TestMethod]
		public void ProcedureCodes_GetProcCodesByTreatmentArea_MouthCodes() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			List<ProcedureCode> listMouthProcCodesOld=ProcedureCodes.GetProcCodesByTreatmentArea(false,TreatmentArea.Mouth);
			int i=0;
			do{
				i++;//Add a nonexisting proc code.
			}while(!ProcedureCodeT.AddIfNotPresent("D"+i.ToString().PadLeft(4,'0')) && i<10000);
			ProcedureCode newProc=ProcedureCodes.GetOne("D"+i.ToString().PadLeft(4,'0'));
			newProc.TreatArea=TreatmentArea.Mouth;
			newProc.Descript=suffix;
			ProcedureCodeT.Update(newProc);
			List<ProcedureCode> listMouthProcCodesNew=ProcedureCodes.GetProcCodesByTreatmentArea(false,TreatmentArea.Mouth);
			Assert.IsFalse(listMouthProcCodesNew.Any(x => x.TreatArea!=TreatmentArea.Mouth));
			Assert.IsTrue((listMouthProcCodesOld.Count+1)==listMouthProcCodesNew.Count);
			Assert.IsTrue(listMouthProcCodesNew.Exists(x => x.CodeNum==newProc.CodeNum));
		}

		[TestMethod]
		public void ProcedureCodes_GetProcCodesByTreatmentArea_HiddenCategories() {
			ProcedureCodeT.ClearProcedureCodeTable();
			//Create a category for procedures that is not hidden.
			Def procCodeCat=DefT.CreateDefinition(DefCat.ProcCodeCats,"NonHiddenProcs",isHidden:false);
			//Create a category for procedures that is hidden.
			Def procCodeCatHidden=DefT.CreateDefinition(DefCat.ProcCodeCats,"HiddenProcs",isHidden:true);
			//Create a procedure code within each category.
			ProcedureCodeT.CreateProcCode("HC001",procCat:procCodeCat.DefNum);
			ProcedureCodeT.CreateProcCode("HC002",procCat:procCodeCatHidden.DefNum);
			//Get a count of procedure codes within the non-hidden categories.
			List<ProcedureCode> listMouthProcCodesOld=ProcedureCodes.GetProcCodesByTreatmentArea(false,TreatmentArea.Mouth,TreatmentArea.None);
			//Hide the non-hidden category that was just created.
			procCodeCat.IsHidden=true;
			Defs.Update(procCodeCat);
			Defs.RefreshCache();
			//Get the new count procedure codes within non-hidden categories.
			List<ProcedureCode> listMouthProcCodesNew=ProcedureCodes.GetProcCodesByTreatmentArea(false,TreatmentArea.Mouth,TreatmentArea.None);
			//Assert that the counts are different. 
			Assert.AreNotEqual(listMouthProcCodesOld.Count,listMouthProcCodesNew.Count);
		}
	}
}
