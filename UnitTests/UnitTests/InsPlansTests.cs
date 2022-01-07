using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using System.Globalization;
using System.Threading;
using System;

namespace UnitTests.InsPlans_Tests {
	[TestClass]
	public class InsPlansTests:TestBase {

		private static List<ProcedureCode> _listProcCodes;
		private static List<ProcedureCode> _listProcCodesOrig;

		[ClassInitialize]
		public static void SetUp(TestContext context) {
			_listProcCodes=ProcedureCodes.GetAllCodes();
			_listProcCodesOrig=_listProcCodes.Select(x => x.Copy()).ToList();
		}

		[TestCleanup]
		public void TearDownTest() {
			//Setting substitution codes can mess up fees for other tests.
			_listProcCodesOrig.ForEach(x => ProcedureCodes.Update(x));
		}

		///<summary></summary>
		[TestMethod]
		public void InsPlans_ComputeEstimatesForSubscriber_CanadianLabFees() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			CultureInfo curCulture=CultureInfo.CurrentCulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			try {
				//Create a patient and treatment plan a procedure with a lab fee.
				Patient pat=PatientT.CreatePatient();
				ProcedureCodeT.AddIfNotPresent("14611");
				ProcedureCodeT.AddIfNotPresent("99111",isCanadianLab:true);
				Procedure proc=ProcedureT.CreateProcedure(pat,"14611",ProcStat.TP,"",250);
				Procedure procLab=ProcedureT.CreateProcedure(pat,"99111",ProcStat.TP,"",149,procNumLab:proc.ProcNum);
				//Create a new primary insurance plan for this patient.
				//It is important that we add the insurance plan after the procedure has already been created for this particular scenario.
				Carrier carrier=CarrierT.CreateCarrier(suffix);
				InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
				InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
				PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
				//Invoking ComputeEstimatesForAll() will simulate the logic of adding a new insurance plan from the Family module.
				//The bug that this unit test is preventing is that a duplicate claimproc was being created for the lab fee.
				//This was causing a faux line to show up when a claim was created for the procedure in question.
				//It ironically doesn't matter if the procedures above are even covered by insurance because they'll get claimprocs created regardless.
				InsPlans.ComputeEstimatesForSubscriber(sub.Subscriber);
				//Check to see how many claimproc enteries there are for the current patient.  There should only be two.
				List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
				Assert.AreEqual(2,listClaimProcs.Count);
			}
			finally {
				Thread.CurrentThread.CurrentCulture=curCulture;
			}
		}

		/// <summary>Get the copay value for when there is no patient copay</summary>
		[TestMethod]
		public void InsPlans_GetCopay_Blank() {
			ProcedureCode procCode=_listProcCodes[0];
			InsPlan plan=GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name);
			double amt=InsPlans.GetCopay(procCode.CodeNum,plan.FeeSched,plan.CopayFeeSched,plan.CodeSubstNone,"",0,0,plan.PlanNum);
			Assert.AreEqual(-1,amt);
		}

		///<summary>Get the copay amount when there is no exact fee on the copay schedule but there is a fee in the default schedule.</summary>
		[TestMethod]
		public void InsPlans_GetCopay_NoExactFeeUseDefault() {
			ProcedureCode procCode=_listProcCodes[1];
			InsPlan plan=GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name);
			Fee feeDefault=FeeT.GetNewFee(plan.FeeSched,procCode.CodeNum,25);
			Prefs.UpdateBool(PrefName.CoPay_FeeSchedule_BlankLikeZero,false);
			double amt=InsPlans.GetCopay(procCode.CodeNum,plan.FeeSched,plan.CopayFeeSched,plan.CodeSubstNone,"",0,0,plan.PlanNum);
			Assert.AreEqual(feeDefault.Amount,amt);
		}

		///<summary>Get the copay amount where there is no exact fee and the Preference CoPay_FeeSchedule_BlankLikeZero is true.</summary>
		[TestMethod]
		public void InsPlans_GetCopay_NoExactFeeUseZero() {
			ProcedureCode procCode=_listProcCodes[2];
			InsPlan plan=GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name);
			Fee feeDefault=FeeT.GetNewFee(plan.FeeSched,procCode.CodeNum,35);
			Prefs.UpdateBool(PrefName.CoPay_FeeSchedule_BlankLikeZero,true);
			double amt=InsPlans.GetCopay(procCode.CodeNum,plan.FeeSched,plan.CopayFeeSched,plan.CodeSubstNone,"",0,0,plan.PlanNum);
			Assert.AreEqual(-1,amt);
			Prefs.UpdateBool(PrefName.CoPay_FeeSchedule_BlankLikeZero,false);
		}

		///<summary>Get the copay value for when there is no substitute fee and the exact copay fee exists.</summary>
		[TestMethod]
		public void InsPlans_GetCopay_ExactFee() {
			ProcedureCode procCode=_listProcCodes[3];
			InsPlan plan=GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name);
			Fee feeDefault=FeeT.GetNewFee(plan.FeeSched,procCode.CodeNum,50);
			Fee feeCopay=FeeT.GetNewFee(plan.CopayFeeSched,procCode.CodeNum,15);
			double amt=InsPlans.GetCopay(procCode.CodeNum,plan.FeeSched,plan.CopayFeeSched,plan.CodeSubstNone,"",0,0,plan.PlanNum);
			Assert.AreEqual(feeCopay.Amount,amt);
		}

		///<summary>Get the copay value for when there is a substitute fee.</summary>
		[TestMethod]
		public void InsPlans_GetCopay_SubstituteFee() {
			ProcedureCode procCode=_listProcCodes[4];
			procCode.SubstitutionCode=_listProcCodes[5].ProcCode;
			ProcedureCodes.Update(procCode);
			InsPlan plan=GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name,false);
			Fee feeDefault=FeeT.GetNewFee(plan.FeeSched,procCode.CodeNum,100);
			Fee feeSubstitute=FeeT.GetNewFee(plan.CopayFeeSched,ProcedureCodes.GetSubstituteCodeNum(procCode.ProcCode,"",plan.PlanNum),45);
			double amt=InsPlans.GetCopay(procCode.CodeNum,plan.FeeSched,plan.CopayFeeSched,plan.CodeSubstNone,"",0,0,plan.PlanNum);
			Assert.AreEqual(feeSubstitute.Amount,amt);
		}

		///<summary>Get the allowed amount for the procedure code for the PPO plan.</summary>
		[TestMethod]
		public void InsPlans_GetAllowed_PPOExact() {
			InsPlan plan=GeneratePPOPlan(MethodBase.GetCurrentMethod().Name);
			ProcedureCode procCode=_listProcCodes[6];
			Fee fee=FeeT.GetNewFee(plan.FeeSched,procCode.CodeNum,65);
			double allowed=InsPlans.GetAllowed(procCode.ProcCode,plan.FeeSched,plan.AllowedFeeSched,plan.CodeSubstNone,plan.PlanType,"",0,0,plan.PlanNum);
			Assert.AreEqual(fee.Amount,allowed);
		}

		///<summary>Get the allowed amount when there is a substitution code for the PPO plan.</summary>
		[TestMethod]
		public void InsPlans_GetAllowed_PPOSubstitute() {
			InsPlan plan=GeneratePPOPlan(MethodBase.GetCurrentMethod().Name,false);
			ProcedureCode procCode=_listProcCodes[7];
			procCode.SubstitutionCode=_listProcCodes[8].ProcCode;
			ProcedureCodes.Update(procCode);
			ProcedureCodes.RefreshCache();
			Fee feeOrig=FeeT.GetNewFee(plan.FeeSched,procCode.CodeNum,85);
			Fee feeSubs=FeeT.GetNewFee(plan.FeeSched,ProcedureCodes.GetSubstituteCodeNum(procCode.ProcCode,"",plan.PlanNum),20);
			double allowed=InsPlans.GetAllowed(procCode.ProcCode,plan.FeeSched,plan.AllowedFeeSched,plan.CodeSubstNone,plan.PlanType,"",0,0,plan.PlanNum);
			Assert.AreEqual(feeSubs.Amount,allowed);
		}

		///<summary>Get the allowed amount where there is a substitution code that is more expensive than the original code for the PPO plan.</summary>
		[TestMethod]
		public void InsPlans_GetAllowed_PPOSubstituteMoreExpensive() {
			InsPlan plan=GeneratePPOPlan(MethodBase.GetCurrentMethod().Name,false);
			ProcedureCode procCode=_listProcCodes[9];
			procCode.SubstitutionCode=_listProcCodes[10].ProcCode;
			ProcedureCodes.Update(procCode);
			Fee feeOrig=FeeT.GetNewFee(plan.FeeSched,procCode.CodeNum,85);
			Fee feeSubs=FeeT.GetNewFee(plan.FeeSched,ProcedureCodes.GetSubstituteCodeNum(procCode.SubstitutionCode,"",plan.PlanNum),200);
			double allowed=InsPlans.GetAllowed(procCode.ProcCode,plan.FeeSched,plan.AllowedFeeSched,plan.CodeSubstNone,plan.PlanType,"",0,0,plan.PlanNum);
			Assert.AreEqual(feeOrig.Amount,allowed);
		}

		///<summary>Get the allowed amount for a capitation plan that has an allowed fee schedule.</summary>
		[TestMethod]
		public void InsPlans_GetAllowed_CapAllowedFeeSched() {
			InsPlan plan=GenerateCapPlan(MethodBase.GetCurrentMethod().Name);
			ProcedureCode procCode=_listProcCodes[11];
			Fee feeAllowed=FeeT.GetNewFee(plan.AllowedFeeSched,procCode.CodeNum,70);
			double amt=InsPlans.GetAllowed(procCode.ProcCode,plan.FeeSched,plan.AllowedFeeSched,plan.CodeSubstNone,plan.PlanType,"",0,0,plan.PlanNum);
			Assert.AreEqual(feeAllowed.Amount,amt);
		}

		///<summary>Get the allowed amount for a capitation plan where there is no allowed fee schedule and there is no substitution code.</summary>
		[TestMethod]
		public void InsPlan_GetAllowed_CapNoAllowedNoSubs() {
			InsPlan plan=GenerateCapPlan(MethodBase.GetCurrentMethod().Name,false);
			ProcedureCode procCode=_listProcCodes[12];
			double amt=InsPlans.GetAllowed(procCode.ProcCode,plan.FeeSched,plan.AllowedFeeSched,plan.CodeSubstNone,plan.PlanType,"",0,0,plan.PlanNum);
			Assert.AreEqual(-1,amt);
		}

		///<summary>Get the allowed amount for a capitation plan where there is no fee schedule assigned to the plan</summary>
		[TestMethod]
		public void InsPlans_GetAllowed_NoFeeSched() {
			Carrier carrier=CarrierT.CreateCarrier(MethodBase.GetCurrentMethod().Name);
			InsPlan plan=new InsPlan();
			plan.CarrierNum=carrier.CarrierNum;
			plan.PlanType="";
			plan.CobRule=EnumCobRule.Basic;
			plan.PlanNum=InsPlans.Insert(plan);
			ProcedureCode procCode=_listProcCodes[13];
			procCode.SubstitutionCode=_listProcCodes[14].ProcCode;
			ProcedureCodes.Update(procCode);
			ProcedureCodes.RefreshCache();
			Provider prov=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			long provFeeSched=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name);
			prov.FeeSched=provFeeSched;
			Providers.Update(prov);
			Providers.RefreshCache();
			Fee defaultFee=FeeT.GetNewFee(Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv)).FeeSched,
				ProcedureCodes.GetSubstituteCodeNum(procCode.ProcCode,"",plan.PlanNum),80);
			double amt=InsPlans.GetAllowed(procCode.ProcCode,plan.FeeSched,plan.AllowedFeeSched,plan.CodeSubstNone,plan.PlanType,"",0,0,plan.PlanNum);
			Assert.AreEqual(defaultFee.Amount,amt);
		}

		#region Factory Methods

		private InsPlan GenerateMediFlatInsPlan(string suffix,bool codeSubstNone=true) {
			long baseFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Normal_"+suffix,true);
			long copayFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay,"Copay_"+suffix,true);
			Carrier carrier = CarrierT.CreateCarrier("Carrier_"+suffix);
			return InsPlanT.CreateInsPlanMediFlatCopay(carrier.CarrierNum,baseFeeSchedNum,copayFeeSchedNum,codeSubstNone);
		}

		private InsPlan GeneratePPOPlan(string suffix,bool codeSubstNone=true) {
			long baseFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Normal_"+suffix,true);
			Carrier carrier=CarrierT.CreateCarrier("Carrier_"+suffix);
			return InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,baseFeeSchedNum,codeSubstNone);
		}

		private InsPlan GenerateCapPlan(string suffix,bool createAllowed=true,bool codeSubstNone=true) {
			long baseFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Normal_"+suffix,true);
			long allowedFeeSchedNum=0;
			if(createAllowed) {
				allowedFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Allowed_"+suffix,true);
			}
			Carrier carrier=CarrierT.CreateCarrier("Carrier_"+suffix);
			return InsPlanT.CreateInsPlanCapitation(carrier.CarrierNum,baseFeeSchedNum,allowedFeeSchedNum,codeSubstNone);
		}

		#endregion

		///<summary>Creates a procedure on an ins plan that does not calculate PPO writeoffs for substituted codes.</summary>
		[TestMethod]
		public void InsPlan_PpoSubNoWriteoffs() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR Fees"+suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,planType:"p",feeSchedNum:ppoFeeSchedNum);
			ins.PriInsPlan.HasPpoSubstWriteoffs=false;
			InsPlans.Update(ins.PriInsPlan);
			BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Restorative,50);
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2330");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2140");
			originalProcCode.SubstitutionCode="D2140";
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodeT.Update(originalProcCode);
			FeeT.CreateFee(ucrFeeSchedNum,originalProcCode.CodeNum,100);
			FeeT.CreateFee(ucrFeeSchedNum,downgradeProcCode.CodeNum,80);
			FeeT.CreateFee(ppoFeeSchedNum,originalProcCode.CodeNum,60);
			FeeT.CreateFee(ppoFeeSchedNum,downgradeProcCode.CodeNum,50);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2330",ProcStat.C,"9",100);//Tooth 9
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			ins.RefreshBenefits();
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			ClaimProc clProc=ClaimProcs.Refresh(pat.PatNum)[0];//Should only be one
			Assert.AreEqual(50,clProc.Percentage);
			Assert.AreEqual(25,clProc.BaseEst);
			Assert.AreEqual(25,clProc.InsPayEst);
			Assert.AreEqual(-1,clProc.WriteOffEst);
		}

		///<summary>Creates a procedure on an ins plan that does calculate PPO writeoffs for substituted codes.</summary>
		[TestMethod]
		public void InsPlan_PpoSubWriteoffs() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR Fees"+suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,planType:"p",feeSchedNum:ppoFeeSchedNum);
			BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Restorative,50);
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2330");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2140");
			originalProcCode.SubstitutionCode="D2140";
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodeT.Update(originalProcCode);
			FeeT.CreateFee(ucrFeeSchedNum,originalProcCode.CodeNum,100);
			FeeT.CreateFee(ucrFeeSchedNum,downgradeProcCode.CodeNum,80);
			FeeT.CreateFee(ppoFeeSchedNum,originalProcCode.CodeNum,60);
			FeeT.CreateFee(ppoFeeSchedNum,downgradeProcCode.CodeNum,50);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2330",ProcStat.C,"8",100);//Tooth 8
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			ins.RefreshBenefits();
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			ClaimProc clProc=ClaimProcs.Refresh(pat.PatNum)[0];//Should only be one
			Assert.AreEqual(50,clProc.Percentage);
			Assert.AreEqual(25,clProc.BaseEst);
			Assert.AreEqual(25,clProc.InsPayEst);
			Assert.AreEqual(40,clProc.WriteOffEst);
		}

		///<summary>Creates a procedure on an ins plan that does not calculate PPO writeoffs for substituted codes where the procedure is not
		///substitued.</summary>
		[TestMethod]
		public void InsPlan_PpoNoSubWriteoffsNoSub() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR Fees"+suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,planType:"p",feeSchedNum:ppoFeeSchedNum);
			ins.PriInsPlan.HasPpoSubstWriteoffs=false;
			InsPlans.Update(ins.PriInsPlan);
			BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Restorative,50);
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2330");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2140");
			originalProcCode.SubstitutionCode="";//NOT substituting
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodeT.Update(originalProcCode);
			FeeT.CreateFee(ucrFeeSchedNum,originalProcCode.CodeNum,100);
			FeeT.CreateFee(ucrFeeSchedNum,downgradeProcCode.CodeNum,80);
			FeeT.CreateFee(ppoFeeSchedNum,originalProcCode.CodeNum,60);
			FeeT.CreateFee(ppoFeeSchedNum,downgradeProcCode.CodeNum,50);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2330",ProcStat.C,"9",100);//Tooth 9
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			ins.RefreshBenefits();
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			ClaimProc clProc=ClaimProcs.Refresh(pat.PatNum)[0];//Should only be one
			Assert.AreEqual(50,clProc.Percentage);
			Assert.AreEqual(30,clProc.BaseEst);
			Assert.AreEqual(30,clProc.InsPayEst);
			Assert.AreEqual(40,clProc.WriteOffEst);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.InsPlan_GetInsUsedDisplay_LimitationsOverride)]
		[Documentation.VersionAdded("7.1")]
		[Documentation.Description("Patient has one insurance plan, subscriber self. Benefits: annual max 1000, diagnostic max 1000. First completed procedure, an exam for $50, insurance paid $50.  Second procedure, a crown for $830, insurance paid $400. Ins used should show $400 and should not include the $50 because the ins used value should only be concerned with amounts that affect the annual max .")]
		[TestMethod]
		public void InsPlan_GetInsUsedDisplay_LimitationsOverride() {
			string suffix="6";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			long planNum=plan.PlanNum;
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,planNum);//guarantor is subscriber
			long subNum=sub.InsSubNum;
			long patPlanNum=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum).PatPlanNum;
			BenefitT.CreateAnnualMax(planNum,1000);
			BenefitT.CreateLimitation(planNum,EbenefitCategory.Diagnostic,1000);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50);//An exam
			long procNum=proc.ProcNum;
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.C,"8",830);//create a crown
			ClaimProcT.AddInsPaid(patNum,planNum,procNum,50,subNum,0,0);
			ClaimProcT.AddInsPaid(patNum,planNum,proc2.ProcNum,400,subNum,0,0);
			//Lists
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(patNum,benefitList,patPlans,planList,DateTime.Today,subList);
			//Validate
			double insUsed=InsPlans.GetInsUsedDisplay(histList,DateTime.Today,planNum,patPlanNum,-1,planList,benefitList,patNum,subNum);
			Assert.AreEqual(400,insUsed);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.InsPlan_GetInsUsedDisplay_OrthoProcsNotAffectInsUsed)]
		[Documentation.VersionAdded("7.9.9")]
		[Documentation.Description("Patient has one insurance plan, subscriber self. Benefits: annual max $100, ortho max $500, 100% on diagnostic and ortho.  2 procs: D0140 (limEx) $59, and D8090 (comprehensive ortho) $348. Each sent to ins on separate claim, received, paid 100%.  Insurance used should show $59.")]
		[TestMethod]
		public void InsPlan_GetInsUsedDisplay_OrthoProcsNotAffectInsUsed() {
			string suffix="13";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			BenefitT.CreateAnnualMax(plan.PlanNum,100);
			BenefitT.CreateOrthoMax(plan.PlanNum,500);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Orthodontics,100);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0140",ProcStat.C,"",59);//limEx
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D8090",ProcStat.C,"",348);//Comprehensive ortho
			ClaimProcT.AddInsPaid(pat.PatNum,plan.PlanNum,proc1.ProcNum,59,subNum,0,0);
			ClaimProcT.AddInsPaid(pat.PatNum,plan.PlanNum,proc2.ProcNum,348,subNum,0,0);
			//Lists
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);
			//Validate
			double insUsed=InsPlans.GetInsUsedDisplay(histList,DateTime.Today,plan.PlanNum,patPlan.PatPlanNum,-1,planList,benefitList,pat.PatNum,subNum);
			Assert.AreEqual(59,insUsed);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.InsPlan_GetDedRemainDisplay_IndividualAndFamilyDeductiblesInsRemaining)]
		[Documentation.VersionAdded("12.1")]
		[Documentation.Description("Three patients, all with the same insurance plan. Guarantor is subscriber. $75 individual deductible, $150 family deductible. Patient 3 has a $75 insurance adjustment for a previously applied deductible. Patient 2 has a procedure D2750 for $1280 that has been paid, including a deductible of $50. Patient 1, the guarantor, has a procedure treatment planned, D4355 for $135. In the guarantor's TP module, at the lower right, the deductible remaining should be $25. An internal test is also performed to verify that if the family deductible were ignored, that the deductible remaining would show $75.")]
		[TestMethod]
		public void InsPlan_GetDedRemainDisplay_IndividualAndFamilyDeductiblesInsRemaining() {
			string suffix="20";
			Patient pat=PatientT.CreatePatient(suffix);//guarantor
			long patNum=pat.PatNum;
			Patient pat2=PatientT.CreatePatient(suffix);
			PatientT.SetGuarantor(pat2,pat.PatNum);
			Patient pat3=PatientT.CreatePatient(suffix);
			PatientT.SetGuarantor(pat3,pat.PatNum);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			long planNum=plan.PlanNum;
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,planNum);//guarantor is subscriber
			long subNum=sub.InsSubNum;
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);//all three patients have the same plan
			PatPlan patPlan2=PatPlanT.CreatePatPlan(1,pat2.PatNum,subNum);//all three patients have the same plan
			PatPlan patPlan3=PatPlanT.CreatePatPlan(1,pat3.PatNum,subNum);//all three patients have the same plan
			BenefitT.CreateDeductibleGeneral(planNum,BenefitCoverageLevel.Individual,75);
			BenefitT.CreateDeductibleGeneral(planNum,BenefitCoverageLevel.Family,150);
			ClaimProcT.AddInsUsedAdjustment(pat3.PatNum,planNum,0,subNum,75);//Adjustment goes on the third patient
			Procedure proc=ProcedureT.CreateProcedure(pat2,"D2750",ProcStat.C,"20",1280);//proc for second patient with a deductible already applied.
			ClaimProcT.AddInsPaid(pat2.PatNum,planNum,proc.ProcNum,304,subNum,50,597);
			proc=ProcedureT.CreateProcedure(pat,"D4355",ProcStat.TP,"",135);//proc is for the first patient
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(patNum,benefitList,patPlans,planList,DateTime.Today,subList);
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			List<ClaimProcHist> HistList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);
			double dedFam=Benefits.GetDeductGeneralDisplay(benefitList,planNum,patPlan.PatPlanNum,BenefitCoverageLevel.Family);
			double ded=Benefits.GetDeductGeneralDisplay(benefitList,planNum,patPlan.PatPlanNum,BenefitCoverageLevel.Individual);
			double dedRem=InsPlans.GetDedRemainDisplay(HistList,DateTime.Today,planNum,patPlan.PatPlanNum,-1,planList,pat.PatNum,ded,dedFam);//test family and individual deductible together
			Assert.AreEqual(25,dedRem);
			dedRem=InsPlans.GetDedRemainDisplay(HistList,DateTime.Today,planNum,patPlan.PatPlanNum,-1,planList,pat.PatNum,ded,-1);//test individual deductible by itself
			Assert.AreEqual(75,dedRem);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.InsPlan_GetPendingDisplay_LimitationsOverrideGeneralLimitations)]
		[Documentation.VersionAdded("12.3.45")]
		[Documentation.Description("Patient has one insurance plan, subscriber self.  Benefits: annual max $1000. Other benefit added for limitation on routine preventive of $1000. Routine preventive 100%. A prophy D1110 for $125 is complete, attached to a claim, with insurance estimate of $125 and a claimproc status of NotReceived. Pending insurance at the lower right of the TP module  should be $0 because the procedure does not count towards the regular annual max. It instead has its own annual max.")]
		[TestMethod]
		public void InsPlan_GetPendingDisplay_LimitationsOverrideGeneralLimitations() {
			string suffix="31";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			long planNum=plan.PlanNum;
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,planNum);//guarantor is subscriber
			long subNum=sub.InsSubNum;
			long patPlanNum=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum).PatPlanNum;
			BenefitT.CreateAnnualMax(planNum,1000);
			BenefitT.CreateCategoryPercent(planNum,EbenefitCategory.RoutinePreventive,100);
			BenefitT.CreateLimitation(planNum,EbenefitCategory.RoutinePreventive,1000);//Changing this amount would affect patient portion vs ins portion.  But regardless of the amount, this should prevent any pending from showing in the box, which is for general pending only.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",125);//Prophy
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,ProcList,benefitList,subList);//Creates the claim in the same manner as the account module, including estimates and status NotReceived.
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(patNum,benefitList,patPlans,planList,DateTime.Today,subList);
			//Validate
			Assert.AreEqual(0,InsPlans.GetPendingDisplay(histList,DateTime.Today,plan,patPlanNum,-1,patNum,subNum,benefitList));
		}

	}
}
