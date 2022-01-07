using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.ClaimProcs_Tests {
	[TestClass]
	public class ClaimProcsTests:TestBase {
		const double _ucrFee=50;
		const double _fee=25;
		const int _coveragePercent=50;
		const double _patPortionFromFee=_fee*((double)_coveragePercent/100.0);
		const double _blankFee=-1;

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			UnitTestsCore.DatabaseTools.ClearDb();
		}

		#region FixedBenefits

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffFalse_PPO60_FixedBenefitFeeBlank() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,false);
			double procFee=100;
			double ppoFee=60;
			double fixedBenefitFee=-1;//blank
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(60,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(-1,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(40,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffTrue_PPO60_FixedBenefitFeeBlank() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,true);
			double procFee=100;
			double ppoFee=60;
			double fixedBenefitFee=-1;//blank
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(60,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(40,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffFalse_PPO60_FixedBenefitFeeZero() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,false);
			double procFee=100;
			double ppoFee=60;
			double fixedBenefitFee=0;
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(60,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(40,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffTrue_PPO60_FixedBenefitFeeZero() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,true);
			double procFee=100;
			double ppoFee=60;
			double fixedBenefitFee=0;
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(60,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(40,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffFalse_PPOBlank_FixedBenefitFeeBlank() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,false);
			double procFee=100;
			double ppoFee=-1;//blank
			double fixedBenefitFee=-1;//blank
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(100,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(-1,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_FixedBenefitBlankLikeZeroOffTrue_PPOBlank_FixedBenefitFeeBlank() {
			PrefT.UpdateBool(PrefName.FixedBenefitBlankLikeZero,true);
			double procFee=100;
			double ppoFee=-1;//blank
			double fixedBenefitFee=-1;//blank
			ComputeBaseEstFixedBenefits(MethodBase.GetCurrentMethod().Name,procFee,ppoFee,fixedBenefitFee,-1,-1,-1,0,0,0
				,((assertItem) =>
				{
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(100,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.WriteOffEst);
				}));
		}

		///<summary>Creates a procedure and computes estimates for a patient with a fixed benefit PPO plan.</summary>
		private void ComputeBaseEstFixedBenefits(string suffix,double procFee,double ppoFee,double fixedBenefitFee,double copayOverride,double allowedOverride,int percentOverride
			,double paidOtherInsTot,double paidOtherInsBase,double writeOffOtherIns,Action<BenefitsAssertItem> assertAct)
		{
			Patient pat=PatientT.CreatePatient(suffix);
			string procStr="D0150";
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			long catPercFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Category % "+suffix);
			long fixedBenefitFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.FixedBenefit,"Fixed Benefit "+suffix);
			if(ppoFee>-1) {
				FeeT.CreateFee(ppoFeeSchedNum,procCode.CodeNum,ppoFee);
			}
			if(fixedBenefitFee>-1) {
				FeeT.CreateFee(fixedBenefitFeeSchedNum,procCode.CodeNum,fixedBenefitFee);
			}
			InsuranceT.AddInsurance(pat,suffix,"p",ppoFeeSchedNum,copayFeeSchedNum: fixedBenefitFeeSchedNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,copayOverride,allowedOverride,percentOverride);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,priPlan,priPatPlan.PatPlanNum,listBens,histList,loopList,listPatPlans,paidOtherInsTot
				,paidOtherInsBase,pat.Age,writeOffOtherIns,listPlans,listSubs,listSubLinks,false,null,null);
			assertAct(new BenefitsAssertItem() {
				Procedure=proc,
				PrimaryClaimProc=priClaimProc,
			});
		}

		#endregion

		#region PPO
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_BlankFee_PPO() {
			AssertExclusions("p",feeSchedFee: _blankFee,hasExclusion: false,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion: _ucrFee*((double)_coveragePercent/100.0));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_ZeroFee_PPO() {
			AssertExclusions("p",feeSchedFee: 0,hasExclusion: false,eProcFee: _ucrFee,eWriteOff: _ucrFee,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_NormalFee_PPO() {
			AssertExclusions("p",feeSchedFee: _fee,hasExclusion: false,eProcFee: _ucrFee,eWriteOff: _ucrFee-_fee,ePatPortion: _patPortionFromFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_PPO() {
			AssertExclusions("p",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion: _ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_PPO() {
			AssertExclusions("p",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: _ucrFee,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_PPO() {
			AssertExclusions("p",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: _ucrFee-_fee,ePatPortion: _ucrFee-(_ucrFee-_fee));
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_PPO_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("p",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_PPO_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("p",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_PPO_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("p",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}
		#endregion

		///<summary>When a procedure has a category covered at 0%, it should be treated as an exclusion and not have a writeoff.</summary>
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_With0PercentCoverage() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR "+suffix);
			FeeSchedT.UpdateUCRFeeSched(pat,ucrFeeSchedNum);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,"p",ppoFeeSchedNum,exclusionRule:ExclusionRule.UseUcrFee);
			ins.AddBenefit(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Crowns,0));
			string procStr="D2740";//crown proc
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			FeeT.CreateFee(ucrFeeSchedNum,procCode.CodeNum,600);
			FeeT.CreateFee(ppoFeeSchedNum,procCode.CodeNum,400);
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",600);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.PriInsPlan.PlanNum,ins.PriInsSub.InsSubNum,DateTime.Today,
				-1,-1,-1,ClaimProcStatus.Estimate);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,ins.PriInsPlan,ins.PriPatPlan.PatPlanNum,ins.ListBenefits,histList,loopList,ins.ListPatPlans,0
				,0,pat.Age,0,ins.ListInsPlans,ins.ListInsSubs,ins.ListSubLinks,false,null,null);
			Assert.AreEqual(0,priClaimProc.WriteOffEst,0.001);
		}

		///<summary>Tests to ensure that CalculateAndUpdate doesn't change the ClaimFee when ClaimStatus is "S" sent or "R" received./summary>
		[TestMethod]
		public void ClaimProcs_CalculateAndUpdate_AndSentStatus() {
			string suffix="29";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2391");
			ProcedureCodes.Update(originalProcCode);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR Fees"+suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO Downgrades"+suffix);
			long  prov=ProviderT.CreateProvider(suffix,feeSchedNum:ucrFeeSchedNum);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsPlan planOld=plan.Copy();
			plan.PlanType="p";
			plan.FeeSched=ppoFeeSchedNum;
			plan.ClaimsUseUCR=true;
			InsPlans.Update(plan,planOld);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - crown
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2391",ProcStat.C,"1",800,provNum:prov);//Tooth 1
			ProcedureT.SetPriority(proc1,0);//Priority 1
			//proc2 - crown
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2391",ProcStat.C,"9",800,provNum:prov);//Tooth 9
			ProcedureT.SetPriority(proc2,1);//Priority 2
			//Lists:
			FeeT.CreateFee(ucrFeeSchedNum,originalProcCode.CodeNum,800);
			FeeT.CreateFee(ppoFeeSchedNum,originalProcCode.CodeNum,700);
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			ProcedureLogic.SortProcedures(ref ProcList);//To order by Priority
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,ProcList,benefitList,subList);//Creates the claim in the same manner as the account module, including estimates.
			Claim claimOld=claim.Copy();
			claim.ClaimStatus="W";
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			claimProcs.ForEach(x=>claimProcListOld.Add(x.Copy()));
			Provider providerObj=Providers.GetProv(prov);
			providerObj.FeeSched=ppoFeeSchedNum;
			ProviderT.Update(providerObj);
			FeeT.UpdateFee(ppoFeeSchedNum,originalProcCode.CodeNum,850);
			Claims.CalculateAndUpdate(ProcList,planList,claim,patPlans,benefitList,pat,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			claimProcs.ForEach(x=>Assert.AreNotEqual(x.FeeBilled,claimProcListOld.Find(y=>x.ClaimProcNum==y.ClaimProcNum).FeeBilled));
			Assert.AreNotEqual(claim.ClaimFee,claimOld.ClaimFee);
			claimOld=claim.Copy();
			claimProcListOld.Clear();
			claimProcs.ForEach(x=>claimProcListOld.Add(x));
			FeeT.UpdateFee(ppoFeeSchedNum,originalProcCode.CodeNum,900);
			claim.ClaimStatus="S";
			Claims.CalculateAndUpdate(ProcList,planList,claim,patPlans,benefitList,pat,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(claim.ClaimFee,claimOld.ClaimFee);
			claimProcs.ForEach(x=>Assert.AreEqual(x.FeeBilled,claimProcListOld.Find(y=>x.ClaimProcNum==y.ClaimProcNum).FeeBilled));			
		}

		#region Medicaid
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_BlankFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: _blankFee,hasExclusion: false,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_ZeroFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: 0,hasExclusion: false,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_NormalFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: _fee,hasExclusion: false,eProcFee: _fee,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: 0,hasExclusion: true,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_Medicaid() {
			AssertExclusions("f",feeSchedFee: _fee,hasExclusion: true,eProcFee: _fee,eWriteOff: -1,ePatPortion: _fee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_Medicaid_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("f",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_Medicaid_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("f",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_Medicaid_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("f",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}
		#endregion

		#region Capitation
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_BlankFee_Capitation() {
			AssertExclusions("c",feeSchedFee: _blankFee,hasExclusion: false,eProcFee: 0,eWriteOff: 0,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_ZeroFee_Capitation() {
			AssertExclusions("c",feeSchedFee: 0,hasExclusion: false,eProcFee: 0,eWriteOff: 0,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_NormalFee_Capitation() {
			AssertExclusions("c",feeSchedFee: _fee,hasExclusion: false,eProcFee: _fee,eWriteOff: _fee,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_Capitation() {
			AssertExclusions("c",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: 0,eWriteOff: 0,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_Capitation() {
			AssertExclusions("c",feeSchedFee: 0,hasExclusion: true,eProcFee: 0,eWriteOff: 0,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_Capitation() {
			AssertExclusions("c",feeSchedFee: _fee,hasExclusion: true,eProcFee: _fee,eWriteOff: _fee,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_Capitation_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("c",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_Capitation_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("c",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_Capitation_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("c",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: 0,ePatPortion:_ucrFee);
		}

		[TestMethod]
		///<summary>We have a DBM named ClaimProcDateNotMatchCapComplete which fixes this issue if present in historic data.</summary>
		public void ClaimProcs_ComputeEstimates_Capitation() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,"Medicaid","c");
			DateTime dateToday=DateTime.Today;//Store the value so it does not change when used in muiltiple places below (in case run at midnight).
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.TP,"",50,dateToday.AddMonths(-1));
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			Assert.AreEqual(listClaimProcs[0].Status,ClaimProcStatus.CapEstimate);//Must be CapEstimate for TP procedures.
			Assert.AreEqual(listClaimProcs[0].ProcDate,proc.ProcDate);//ProcDate must be synchronized.
			Assert.AreEqual(listClaimProcs[0].DateCP,proc.ProcDate);//DateCP (Payment Date) starts as the ProcDate for TP procs and is updated when completed.
			Procedure procOld=proc.Copy();
			proc.ProcStatus=ProcStat.C;
			proc.ProcDate=dateToday;//When we set procedures complete anywhere in the program, we also set the ProcDate to today.
			Procedures.Update(proc,procOld);
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,false,
				insInfo.ListInsPlans,insInfo.ListPatPlans,insInfo.ListBenefits,pat.Age,insInfo.ListInsSubs);
			List<ClaimProc> listCompClaimProcs=ClaimProcs.RefreshForProc(proc.ProcNum);
			Assert.AreEqual(listCompClaimProcs[0].Status,ClaimProcStatus.CapComplete);//Must be CapComplete for complete procedures.
			Assert.AreEqual(listCompClaimProcs[0].ProcDate,dateToday);//ProcDate must be set to today's date when completed.
			Assert.AreEqual(listCompClaimProcs[0].DateCP,dateToday);//DateCP (Payment Date) must be set to today's date when completed.
		}

		#endregion

		#region CatPercent
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_BlankFee_CatPercent() {
			AssertExclusions("",feeSchedFee: _blankFee,hasExclusion: false,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_ZeroFee_CatPercent() {
			AssertExclusions("",feeSchedFee: 0,hasExclusion: false,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_NoExclusion_NormalFee_CatPercent() {
			AssertExclusions("",feeSchedFee: _fee,hasExclusion: false,eProcFee: _fee,eWriteOff: -1,ePatPortion: _patPortionFromFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_CatPercent() {
			AssertExclusions("",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_CatPercent() {
			AssertExclusions("",feeSchedFee: 0,hasExclusion: true,eProcFee: 0,eWriteOff: -1,ePatPortion: 0);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_CatPercent() {
			AssertExclusions("",feeSchedFee: _fee,hasExclusion: true,eProcFee: _fee,eWriteOff: -1,ePatPortion: _fee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_BlankFee_CatPercent_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("",feeSchedFee: _blankFee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_ZeroFee_CatPercent_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("",feeSchedFee: 0,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NormalFee_CatPercent_ExcludedUseUCR() {
			PrefT.UpdateBool(PrefName.InsPlanUseUcrFeeForExclusions,true);
			AssertExclusions("",feeSchedFee: _fee,hasExclusion: true,eProcFee: _ucrFee,eWriteOff: -1,ePatPortion:_ucrFee);
		}
		#endregion

		#region NoBillins Preference
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_Exclusion_NoBillIns() {
			PrefT.UpdateBool(PrefName.InsPlanExclusionsMarkDoNotBillIns,true);
			AssertExclusions("",feeSchedFee: _fee,hasExclusion: true,eProcFee: _fee,eWriteOff: -1,ePatPortion: _fee,eNoBillIns: true);
		}
		#endregion

		#region InsEstRecalcReceived
		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_InsEstRecalcReceived_True() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=_coveragePercent;
			const double ucrFee=_ucrFee;
			string procStr="D0145";
			string planType="";//percentage
			double feeSchedFee=_fee;
			double eBaseEst=feeSchedFee*(coveragePercent/100d);//Expected BaseEst
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			#region Provider Ucr Fee Setup
			long provFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			long provNum=ProviderT.CreateProvider($"Provider {suffix}",feeSchedNum:provFeeSchedNum);
			FeeT.CreateFee(provFeeSchedNum,procCode.CodeNum,ucrFee);
			#endregion
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			#region Fee Schedule Setup
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			if(feeSchedFee>-1) {
				FeeT.CreateFee(feeSchedNum,procCode.CodeNum,feeSchedFee);
			}
			#endregion
			#region InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Received);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			PrefT.UpdateBool(PrefName.InsEstRecalcReceived,true);//Set InsEstRecalcReceived preference to true.
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,priPlan,priPatPlan.PatPlanNum,listBens,histList,loopList,listPatPlans,0
				,0,pat.Age,0,listPlans,listSubs,listSubLinks,false,null,null);
			Assert.AreEqual(eBaseEst,priClaimProc.BaseEst);
		}

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_InsEstRecalcReceived_False() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=_coveragePercent;
			const double ucrFee=_ucrFee;
			string procStr="D0145";
			string planType="";//percentage
			double feeSchedFee=_fee;
			double eBaseEst=0;//Expected BaseEst will not be recalculated for Received ClaimProc, therefore, will stay 0.
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			#region Provider Ucr Fee Setup
			long provFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			long provNum=ProviderT.CreateProvider($"Provider {suffix}",feeSchedNum:provFeeSchedNum);
			FeeT.CreateFee(provFeeSchedNum,procCode.CodeNum,ucrFee);
			#endregion
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			#region Fee Schedule Setup
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			if(feeSchedFee>-1) {
				FeeT.CreateFee(feeSchedNum,procCode.CodeNum,feeSchedFee);
			}
			#endregion
			#region InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Received);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			PrefT.UpdateBool(PrefName.InsEstRecalcReceived,false);//Set InsEstRecalcReceived preference to false.
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,priPlan,priPatPlan.PatPlanNum,listBens,histList,loopList,listPatPlans,0
				,0,pat.Age,0,listPlans,listSubs,listSubLinks,false,null,null);
			Assert.AreEqual(eBaseEst,priClaimProc.BaseEst);
		}
		#endregion InsEstRecalcReceived

		#region Secondary PPO

		[TestMethod]
		public void ClaimProcs_ComputeBaseEst_PPOSecondaryWriteoffs() {
			PrefT.UpdateBool(PrefName.InsPPOsecWriteoffs,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			int coveragePercentPri=70;
			int coveragePercentSec=80;
			int procFee=100;
			int secAllowed=80;
			string procStr="D0145";
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			Patient pat=PatientT.CreatePatient(suffix);
			long feeSchedNumPri=FeeSchedT.CreateFeeSched(FeeScheduleType.OutNetwork,"FeeSchedPri "+suffix);
			long feeSchedNumSec=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSchedSec "+suffix);
			FeeT.CreateFee(feeSchedNumSec,procCode.CodeNum,secAllowed);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"Pri "+suffix,"");
			ins.AddInsurance(pat,"Sec "+suffix,"p",feeSchedNumSec,2,false,cobRule: EnumCobRule.Standard);
			ins.AddBenefit(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,coveragePercentPri));
			ins.AddBenefit(BenefitT.CreateCategoryPercent(ins.SecInsPlan.PlanNum,EbenefitCategory.Diagnostic,coveragePercentSec));
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.PriInsPlan.PlanNum,ins.PriInsSub.InsSubNum,DateTime.Today,
				-1,-1,-1,ClaimProcStatus.Estimate);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,ins.PriInsPlan,ins.PriPatPlan.PatPlanNum,ins.ListBenefits,histList,loopList,ins.ListPatPlans,0
				,0,pat.Age,0,ins.ListInsPlans,ins.ListInsSubs,ins.ListSubLinks,false,null,null);
			Assert.AreEqual(70,priClaimProc.InsPayEst);
			Assert.AreEqual(-1,priClaimProc.WriteOffEst);
			ClaimProc secClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.SecInsPlan.PlanNum,ins.SecInsSub.InsSubNum,DateTime.Today,
				-1,-1,-1,ClaimProcStatus.Estimate);
			ClaimProcs.ComputeBaseEst(secClaimProc,proc,ins.SecInsPlan,ins.SecPatPlan.PatPlanNum,ins.ListBenefits,histList,loopList,ins.ListPatPlans,
				priClaimProc.InsPayEst,priClaimProc.InsPayEst,pat.Age,0,ins.ListInsPlans,ins.ListInsSubs,ins.ListSubLinks,false,null,null);
			Assert.AreEqual(30,secClaimProc.InsPayEst);
			Assert.AreEqual(0,secClaimProc.WriteOffEst);
		}

		#endregion Secondary PPO

		///<summary>This unit test is the first one that looks at the values showing in the claimproc window.
		///This catches situations where the only "bug" is just a display issue in that window.
		///Validates the values in the claimproc window when opened from the Chart module.</summary>
		[TestMethod]
		[Documentation.VersionAdded("12.3")]
		[Documentation.Numbering(Documentation.EnumTestNum.ClaimProcs_FormClaimProc_TextBoxValuesFromChartModule)]
		[Documentation.Description(@"<table>
  <tr>
    <td>Proc</td>
    <td>Fee</td>
    <td>Ded</td>
    <td>Est</td>
    <td>PatPort</td>
  </tr>
  <tr>
    <td>1</td>
    <td>$800.00</td>
    <td>$25.00</td>
    <td>$387.50</td>
    <td>$412.50</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$800.00</td>
    <td>$0.00</td>
    <td>$400.00</td>
    <td>$400.00</td>
  </tr>
</table>
The patient has one insurance plan, category percentage, subscriber self. Benefits: annual max $1300, 50% coverage on crowns, and a $25 general deductible. Both TP procs are crowns, first TP proc is D2790  for $800 on #1 with priority 1, second TP proc is D2790 on #9 for $800 with priority 2. First, the estimates are pre-calculated by visiting the TP module. Then, when opening a procedure from Chart Module, followed by opening the claimproc from inside of the Edit Procedure window, the amounts must match those listed in the table above. This unit test is the first one that looks at the values showing in the claimproc window.  This catches situations where the only &quot;bug&quot; is just a display issue in that window.")]
		public void ClaimProcs_FormClaimProc_TextBoxValuesFromChartModule() {
			string suffix="28";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,1300);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,25);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - crown
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.TP,"1",800);//Tooth 1
			ProcedureT.SetPriority(proc1,0);//Priority 1
			//proc2 - crown
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.TP,"9",800);//Tooth 9
			ProcedureT.SetPriority(proc2,1);//Priority 2
			//Lists:
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			//Mimick the TP module estimate calculations when the TP module is loaded. We expect the user to refresh the TP module to calculate insurance estimates for all other areas of the program.
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//Save changes in the list to the database, just like the TP module does when loaded. Then the values can be referenced elsewhere in the program instead of recalculating.
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Validate the estimates within the Edit Claim Proc window are correct when opened from inside of the Chart module by passing in a null histlist and a null looplist just like the Chart module would.
			List<ClaimProcHist> histListNull=null;
			List<ClaimProcHist> loopListNull=null;
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP1=new FormClaimProc(claimProc1,proc1,fam,pat,planList,histListNull,ref loopListNull,patPlans,false,subList);
			formCP1.Initialize();
			Assert.AreEqual("25.00",formCP1.GetTextValue("textDedEst"));
			Assert.AreEqual("412.50",formCP1.GetTextValue("textPatPortion1"));
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP2=new FormClaimProc(claimProc2,proc2,fam,pat,planList,histListNull,ref loopListNull,patPlans,false,subList);
			formCP2.Initialize();
			Assert.AreEqual("0.00",formCP2.GetTextValue("textDedEst"));
			Assert.AreEqual("400.00",formCP2.GetTextValue("textPatPortion1"));
		}

		///<summary>Validates the values in the claimproc window when opened from the Claim Edit window.</summary>
		[TestMethod]
		[Documentation.Description(@"<table>
  <tr>
    <td>Proc</td>
    <td>Fee</td>
    <td>Ded</td>
    <td>Est</td>
    <td>PatPort</td>
  </tr>
  <tr>
    <td>1</td>
    <td>$800.00</td>
    <td>$25.00</td>
    <td>$387.50</td>
    <td>$412.50</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$800.00</td>
    <td>$0.00</td>
    <td>$400.00</td>
    <td>$400.00</td>
  </tr>
</table>
The patient has one insurance plan, category percentage, subscriber self. Benefits: annual max $1300, 50% coverage on crowns and a $25 general deductible. Both complete crowns are on the same claim, first proc is D2790  for $800 on #1, second proc is D2790 on #9 for $800. The claimproc estimates are calculated when the claim is created. Then, when opening a claimproc from inside of the Edit Claim window, the amounts must match those listed in the table above.")]
		[Documentation.Numbering(Documentation.EnumTestNum.ClaimProcs_FormClaimProc_TextBoxValuesFromClaimEditWindow)]
		[Documentation.VersionAdded("12.3")]
		public void ClaimProcs_FormClaimProc_TextBoxValuesFromClaimEditWindow() {
			string suffix="29";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,1300);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,25);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - crown
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.C,"1",800);//Tooth 1
			ProcedureT.SetPriority(proc1,0);//Priority 1
			//proc2 - crown
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.C,"9",800);//Tooth 9
			ProcedureT.SetPriority(proc2,1);//Priority 2
			//Lists:
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			ProcedureLogic.SortProcedures(ref ProcList);//To order by Priority
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,ProcList,benefitList,subList);//Creates the claim in the same manner as the account module, including estimates.
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Validate the estimates as they would appear inside of the Claim Proc Edit window when opened from inside of the Edit Claim window by passing in the null histlist and null looplist that the Claim Edit window would send in.
			List<ClaimProcHist> histList=null;
			List<ClaimProcHist> loopList=null;
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP1=new FormClaimProc(claimProc1,proc1,fam,pat,planList,histList,ref loopList,patPlans,false,subList);
			formCP1.IsInClaim=true;
			formCP1.Initialize();
			Assert.AreEqual("25.00",formCP1.GetTextValue("textDedEst"));
			Assert.AreEqual("412.50",formCP1.GetTextValue("textPatPortion1"));
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP2=new FormClaimProc(claimProc2,proc2,fam,pat,planList,histList,ref loopList,patPlans,false,subList);
			formCP2.IsInClaim=true;
			formCP2.Initialize();
			Assert.AreEqual("0.00",formCP2.GetTextValue("textDedEst"));
			Assert.AreEqual("400.00",formCP2.GetTextValue("textPatPortion1"));
		}

		///<summary>Validates the values in the claimproc window when opened from the TP module.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClaimProcs_FormClaimProc_TextBoxValuesFromTreatPlanModule)]
		[Documentation.VersionAdded("12.3")]
		[Documentation.Description(@"<table>
  <tr>
    <td>Proc</td>
    <td>Fee</td>
    <td>Ded</td>
    <td>Est</td>
    <td>PatPort</td>
  </tr>
  <tr>
    <td>1</td>
    <td>$800.00</td>
    <td>$25.00</td>
    <td>$387.50</td>
    <td>$412.50</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$800.00</td>
    <td>$0.00</td>
    <td>$400.00</td>
    <td>$400.00</td>
  </tr>
</table>
The patient has one insurance plan, category percentage, subscriber self. Benefits: annual max $1300, 50% coverage on crowns and a $25 general deductible. <span> Both TP procs are crowns, first TP proc is D2790  for $800 on #1 with priority 1, second TP proc is D2790 on #9 for $800 with priority 2.</span> First, the estimates are pre-calculated by visiting the TP module. Then, when opening a procedure from TP Module, followed by opening the claimproc from inside of the Edit Procedure window, the amounts must match those listed in the table above.")]
		public void ClaimProcs_FormClaimProc_TextBoxValuesFromTreatPlanModule() {
			string suffix="30";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,1300);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,25);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - crown
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.TP,"1",800);//Tooth 1
			ProcedureT.SetPriority(proc1,0);//Priority 1
			//proc2 - crown
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2790",ProcStat.TP,"9",800);//Tooth 9
			ProcedureT.SetPriority(proc2,1);//Priority 2
			//Lists:
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			//Mimick the TP module estimate calculations when the TP module is loaded.
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//Save changes in the list to the database, just like the TP module does when loaded.
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Validate the estimates within the Edit Claim Proc window are correct when opened from inside of the TP module by passing in same histlist and loop list that the TP module would.
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);//The history list is fetched when the TP module is loaded and is passed in the same for all claimprocs.
			loopList=new List<ClaimProcHist>();//Always empty for the first claimproc.
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,subNum);
			FormClaimProc formCP1=new FormClaimProc(claimProc1,proc1,fam,pat,planList,histList,ref loopList,patPlans,false,subList);
			formCP1.Initialize();
			Assert.AreEqual("25.00",formCP1.GetTextValue("textDedEst"));
			Assert.AreEqual("412.50",formCP1.GetTextValue("textPatPortion1"));
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);//The history list is fetched when the TP module is loaded and is passed in the same for all claimprocs.
			loopList=new List<ClaimProcHist>();
			loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,proc1.ProcNum,proc1.CodeNum));
			FormClaimProc formCP2=new FormClaimProc(claimProc2,proc2,fam,pat,planList,histList,ref loopList,patPlans,false,subList);
			formCP2.Initialize();
			Assert.AreEqual("0.00",formCP2.GetTextValue("textDedEst"));
			Assert.AreEqual("400.00",formCP2.GetTextValue("textPatPortion1"));
		}

		/// <summary>
		/// Sets up a plan based on the passed in plan type/fee schedule fee/if the proc should be an exclusion.
		/// eProcFee/eWriteOff/ePatPortion are the expected values of the assertions
		/// </summary>
		private void AssertExclusions(string planType,double feeSchedFee,bool hasExclusion,double eProcFee,double eWriteOff,double ePatPortion,bool eNoBillIns=false) {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=50;
			const int ucrFee=50;
			string procStr="D0145";
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			#region Provider Ucr Fee Setup
			long provFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			long provNum=ProviderT.CreateProvider($"Provider Exclusion {suffix}",feeSchedNum:provFeeSchedNum);
			FeeT.CreateFee(provFeeSchedNum,procCode.CodeNum,ucrFee);
			#endregion
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum);
			#region Fee Schedule Setup
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"FeeSched "+suffix);
			if(feeSchedFee>-1) {
				FeeT.CreateFee(feeSchedNum,procCode.CodeNum,feeSchedFee);
			}
			#endregion
			#region InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			if(hasExclusion) {
				BenefitT.CreateExclusion(priPlan.PlanNum,procCode.CodeNum);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ClaimProcStatus cps=ClaimProcStatus.NotReceived;
			if(planType=="c") {
				cps=ClaimProcStatus.CapEstimate;
			}
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,cps);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			ClaimProcs.ComputeBaseEst(priClaimProc,proc,priPlan,priPatPlan.PatPlanNum,listBens,histList,loopList,listPatPlans,0
				,0,pat.Age,0,listPlans,listSubs,listSubLinks,false,null,null);
			#region Assert
			BenefitsAssertItem assertItem=new BenefitsAssertItem();
			assertItem.PrimaryClaimProc=priClaimProc;
			assertItem.Procedure=proc;
			Assert.AreEqual(eProcFee,assertItem.Procedure.ProcFee,"ProcFee calculation");
			Assert.AreEqual(eWriteOff,assertItem.PrimaryClaimProc.WriteOffEst,"WriteOffEst calculation");
			double patPort=(double)ClaimProcs.GetPatPortion(assertItem.Procedure,new List<ClaimProc>() { assertItem.PrimaryClaimProc });
			Assert.AreEqual(ePatPortion,patPort,"PatPortion calculation");
			Assert.AreEqual(eNoBillIns,assertItem.PrimaryClaimProc.NoBillIns,"NoBillIns");
			#endregion
		}
		
		#region Canada
		///<summary>tba
		///</summary>
		[TestMethod]
		public void Canada_ComputeEstimatesForAll_ChangingStatusDoesNotDuplicate() {
			CultureInfo cultureOld=Thread.CurrentThread.CurrentCulture;
			CultureInfo uiCultureOld=Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-CA");

			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);//Have a Patient

			Carrier carrier=CarrierT.CreateCarrier(suffix);//Who has an Insurance Carrier
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);

			string procStr="D0145";
			ProcedureCodeT.AddIfNotPresent(procStr,false);
			string labProcStr="99111";//Should be 99111
			ProcedureCodeT.AddIfNotPresent(labProcStr,true);
			double procFee=100;
			string toothNum="";
			//Procedure 1 - Parent Proc
			Procedure procParent=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,toothNum,procFee);
			//Procedure 2 - Lab Fee
			Procedure procLabFee=ProcedureT.CreateProcedure(pat,labProcStr,ProcStat.C,toothNum,procFee);
			Procedure procOld=procLabFee.Copy();
			procLabFee.ProcNumLab=procParent.ProcNum;
			Procedures.Update(procLabFee,procOld);

			string claimType="P";//create claim
			ins.ListAllProcs=Procedures.Refresh(pat.PatNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			int coveragePercent=50;
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,procParent.CodeNum,coveragePercent));
			Claim claim=ClaimT.CreateClaim(claimType,ins.ListPatPlans,ins.ListInsPlans,ins.ListAllClaimProcs,ins.ListAllProcs,pat,ins.ListAllProcs,ins.ListBenefits,ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);//set claim received (should computeestimates)
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			
			Assert.IsTrue(ins.ListAllClaimProcs.Count==2);//Assert only one claimproc per proc
			
			ClaimProc claimProcParent=ins.ListAllClaimProcs.First(x => x.ProcNum==procParent.ProcNum);
			claimProcParent.Status=ClaimProcStatus.NotReceived;//set procparent not received
			ClaimProcs.Update(claimProcParent);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProcs.UpdatePertinentLabStatuses(claimProcParent,ins.PriInsPlan);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);

			Assert.AreEqual(2,ins.ListAllClaimProcs.Count);//Assert only one claimproc per proc
			
			claimProcParent.Status=ClaimProcStatus.Received;//set procparent received
			ClaimProcs.Update(claimProcParent);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProcs.UpdatePertinentLabStatuses(claimProcParent,ins.PriInsPlan);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);

			Assert.AreEqual(2,ins.ListAllClaimProcs.Count);//Assert only one claimproc per proc

			//reset threads
			Thread.CurrentThread.CurrentCulture=cultureOld;
			Thread.CurrentThread.CurrentUICulture=uiCultureOld;
		}

		[TestMethod]
		public void Canada_ComputeEstimatesForAll_SentClaimDoesNotUpdateLabFeeBilled() {
			CultureInfo cultureOld=Thread.CurrentThread.CurrentCulture;
			CultureInfo uiCultureOld=Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-CA");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);//Have a Patient
			Carrier carrier=CarrierT.CreateCarrier(suffix);//Who has an Insurance Carrier
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			string procStr="D0145";
			ProcedureCodeT.AddIfNotPresent(procStr,false);
			string labProcStr="99111";//Should be 99111
			ProcedureCodeT.AddIfNotPresent(labProcStr,true);
			double procFee=100;
			string toothNum="";
			//Procedure 1 - Parent Proc
			Procedure procParent=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,toothNum,procFee);
			//Procedure 2 - Lab Fee
			Procedure procLabFee=ProcedureT.CreateProcedure(pat,labProcStr,ProcStat.C,toothNum,procFee);
			Procedure procOld=procLabFee.Copy();
			procLabFee.ProcNumLab=procParent.ProcNum;
			Procedures.Update(procLabFee,procOld);
			string claimType="P";//create claim
			ins.ListAllProcs=Procedures.Refresh(pat.PatNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			int coveragePercent=50;
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,procParent.CodeNum,coveragePercent));
			Claim claim=ClaimT.CreateClaim(claimType,ins.ListPatPlans,ins.ListInsPlans,ins.ListAllClaimProcs,ins.ListAllProcs,pat,ins.ListAllProcs,ins.ListBenefits,
				ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			//Assert the proc fee is equal to the fee billed
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ClaimProc labProc=ins.ListAllClaimProcs.Find(x => x.CodeSent==labProcStr);
			Assert.AreEqual(labProc.FeeBilled,procFee);
			//Double the lab fee
			procOld=procLabFee.Copy();
			procLabFee.ProcFee=procFee*2;
			Procedures.Update(procLabFee,procOld);
			//Recalculate estimates
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			labProc=ins.ListAllClaimProcs.Find(x => x.CodeSent==labProcStr);
			Assert.AreEqual(labProc.FeeBilled,procFee*2);
			//Receive claim, and try to update the fee again
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);
			procOld=procLabFee.Copy();
			procLabFee.ProcFee=procFee*3;
			Procedures.Update(procLabFee,procOld);
			//Validate old fee
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			labProc=ins.ListAllClaimProcs.Find(x => x.CodeSent==labProcStr);
			Assert.AreEqual(labProc.FeeBilled,procFee*2);
			//reset threads
			Thread.CurrentThread.CurrentCulture=cultureOld;
			Thread.CurrentThread.CurrentUICulture=uiCultureOld;
		}

		///<summary>Check that labs will use the benefit categroy of the parent proc.</summary>
		[TestMethod]
		public void Canada_ComputeEstimatesForAll_LabUsesParentBenefit() {
			CultureInfo cultureOld=Thread.CurrentThread.CurrentCulture;
			CultureInfo uiCultureOld=Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-CA");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);//Have a Patient
			Carrier carrier=CarrierT.CreateCarrier(suffix);//Who has an Insurance Carrier
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			double crownAnnualMax=20;
			//Create an annual max that will get hit.
			BenefitT.CreateAnnualMax(ins.ListInsPlans[0].PlanNum,90);
			BenefitT.CreateCategoryPercent(ins.ListInsPlans[0].PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateLimitation(ins.ListInsPlans[0].PlanNum,EbenefitCategory.Crowns,crownAnnualMax);
			ins.RefreshBenefits();
			//Add crown proc and lab
			string procStr="D2783"; //Crown
			ProcedureCodeT.AddIfNotPresent(procStr,false);
			string labProcStr="99111";//Should be 99111
			ProcedureCodeT.AddIfNotPresent(labProcStr,true);
			double procFee=100;
			string toothNum="";
			//Procedure 1 - Parent Proc
			Procedure procParent=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,toothNum,procFee);
			//Procedure 2 - Lab Fee
			Procedure procLabFee=ProcedureT.CreateProcedure(pat,labProcStr,ProcStat.C,toothNum,procFee);
			Procedure procOld=procLabFee.Copy();
			procLabFee.ProcNumLab=procParent.ProcNum;
			Procedures.Update(procLabFee,procOld);
			string claimType="P";//create claim
			ins.ListAllProcs=Procedures.Refresh(pat.PatNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Claim claim=ClaimT.CreateClaim(claimType,ins.ListPatPlans,ins.ListInsPlans,ins.ListAllClaimProcs,ins.ListAllProcs,pat,ins.ListAllProcs,ins.ListBenefits,ins.ListInsSubs);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			//Check only the crown benefit used
			double insUsed=ins.ListAllClaimProcs.Sum(x => x.InsPayEst);
			Assert.AreEqual(crownAnnualMax,insUsed);
			//reset threads
			Thread.CurrentThread.CurrentCulture=cultureOld;
			Thread.CurrentThread.CurrentUICulture=uiCultureOld;
		}

		///<summary>Check that labs will use the benefit categroy of the parent proc.</summary>
		[TestMethod]
		public void Canada_ComputeEstimatesForAll_SecondaryLabReducesByPrimaryEst() {
			CultureInfo cultureOld=Thread.CurrentThread.CurrentCulture;
			CultureInfo uiCultureOld=Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-CA");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);//Have a Patient
			Carrier carrier=CarrierT.CreateCarrier(suffix);//Who has an Insurance Carrier
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			//Create an annual max that will get hit.
			BenefitT.CreateAnnualMax(ins.ListInsPlans[0].PlanNum,1500);
			BenefitT.CreateCategoryPercent(ins.ListInsPlans[0].PlanNum,EbenefitCategory.Crowns,50);
			ins.RefreshBenefits();
			Carrier carrier2=CarrierT.CreateCarrier("2"+suffix);//Who has an Insurance Carrier
			InsuranceInfo ins2=InsuranceT.AddInsurance(pat,carrier2.CarrierName,ordinal:2);
			//Create an annual max that will get hit.
			BenefitT.CreateAnnualMax(ins2.ListInsPlans[0].PlanNum,1500);
			BenefitT.CreateCategoryPercent(ins2.ListInsPlans[0].PlanNum,EbenefitCategory.Crowns,50);
			ins2.RefreshBenefits();
			ins2.ListInsPlans.Add(ins.ListInsPlans[0]);
			ins2.ListInsSubs.Add(ins.ListInsSubs[0]);
			ins2.ListPatPlans.Add(ins.ListPatPlans[0]);
			//Add crown proc and lab
			string procStr="D2783"; //Crown
			ProcedureCodeT.AddIfNotPresent(procStr,false);
			string labProcStr="99111";//Should be 99111
			ProcedureCodeT.AddIfNotPresent(labProcStr,true);
			double procFee=100;
			string toothNum="";
			//Procedure 1 - Parent Proc
			Procedure procParent=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,toothNum,procFee);
			//Procedure 2 - Lab Fee
			Procedure procLabFee=ProcedureT.CreateProcedure(pat,labProcStr,ProcStat.C,toothNum,procFee);
			Procedure procOld=procLabFee.Copy();
			procLabFee.ProcNumLab=procParent.ProcNum;
			Procedures.Update(procLabFee,procOld);
			string claimType="P";//create claim
			ins.ListAllProcs=Procedures.Refresh(pat.PatNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ins2.ListAllProcs=Procedures.Refresh(pat.PatNum);
			ins2.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Claim claim=ClaimT.CreateClaim(claimType,ins.ListPatPlans,ins.ListInsPlans,ins.ListAllClaimProcs,ins.ListAllProcs,pat,ins.ListAllProcs,ins.ListBenefits,ins.ListInsSubs);
			Claim claim2=ClaimT.CreateClaim("S",ins2.ListPatPlans,ins2.ListInsPlans,ins2.ListAllClaimProcs,ins2.ListAllProcs,pat,ins2.ListAllProcs,ins2.ListBenefits,ins2.ListInsSubs);
			ins2.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			//Check only the crown benefit used
			double insUsed=ins.ListAllClaimProcs.Sum(x => x.InsPayEst);
			insUsed=ins2.ListAllClaimProcs.Sum(x => x.InsPayEst);
			Assert.AreEqual(100,insUsed);
			//reset threads
			Thread.CurrentThread.CurrentCulture=cultureOld;
			Thread.CurrentThread.CurrentUICulture=uiCultureOld;
		}

		[TestMethod]
		public void Canada_DeleteAfterValidating_DeleteOnlyLabsForCurClaimProc() {
			CultureInfo cultureOld=Thread.CurrentThread.CurrentCulture;
			CultureInfo uiCultureOld=Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-CA");
			//Setup
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);//Have a Patient
			Carrier carrier=CarrierT.CreateCarrier(suffix);//Who has an Insurance Carrier
			string procStr="D0145";
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			ProcedureCodeT.AddIfNotPresent(procStr,false);
			string labProcStr="99111";//Should be 99111
			ProcedureCodeT.AddIfNotPresent(labProcStr,true);
			double procFee=100;
			string toothNum="";
			//Create procs and labs to be deleted
			Procedure procParentDelete=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,toothNum,procFee);
			Procedure procLabFee1Delete=ProcedureT.CreateProcedure(pat,labProcStr,ProcStat.C,toothNum,procFee,procNumLab:procParentDelete.ProcNum);
			Procedure procLabFee2Delete=ProcedureT.CreateProcedure(pat,labProcStr,ProcStat.C,toothNum,procFee,procNumLab:procParentDelete.ProcNum);
			List<Procedure> listProcsForClaimDelete=new List<Procedure>() { procParentDelete,procLabFee1Delete,procLabFee2Delete };
			Claim parentClaimDelete=ClaimT.CreateClaim(listProcsForClaimDelete,ins);
		  ClaimProc claimProcParentDelete=ClaimProcT.CreateClaimProc(pat.PatNum,procParentDelete.ProcNum,carrier.CarrierNum,
				pat.PatNum,claimNum:parentClaimDelete.ClaimNum);
			//Create procs and labs to not be deleted
			Procedure procParentKeep=ProcedureT.CreateProcedure(pat,"D2161",ProcStat.C,toothNum,50);
			Procedure procLabKeep=ProcedureT.CreateProcedure(pat,labProcStr,ProcStat.C,toothNum,50,procNumLab:procParentKeep.ProcNum);
			List<Procedure> listProcsForClaimDontDelete=new List<Procedure>(){ procParentKeep,procLabKeep };
			Claim parentClaimDontDelete=ClaimT.CreateClaim(listProcsForClaimDontDelete,ins);
			ClaimProc claimProcParentDontDelete=ClaimProcT.CreateClaimProc(pat.PatNum,procParentKeep.ProcNum,carrier.CarrierNum,
				pat.PatNum,claimNum:parentClaimDontDelete.ClaimNum);
			//Setup lists for comparison
			List<ClaimProc> listClaimProcsExpected=ClaimProcs.RefreshForClaim(parentClaimDontDelete.ClaimNum);
			List<ClaimProc> listClaimProcsBeforeDeleting=ClaimProcs.Refresh(pat.PatNum);
			ClaimProcs.DeleteAfterValidating(claimProcParentDelete);
			List<ClaimProc> listClaimProcsAfterDeleting=ClaimProcs.Refresh(pat.PatNum);
			//A-B=0
			Assert.AreEqual(0,listClaimProcsExpected.Select(x => x.ClaimProcNum).Except(listClaimProcsAfterDeleting.Select(x => x.ClaimProcNum)).Count());
			//B-A=0
			Assert.AreEqual(listClaimProcsAfterDeleting.Select(x=>x.ClaimProcNum).Except(listClaimProcsBeforeDeleting.Select(x=>x.ClaimProcNum)).Count(),0);
			//Should have only removed the parent claim and the two labs associated with it
			Assert.AreEqual(listClaimProcsBeforeDeleting.Count()-3,listClaimProcsAfterDeleting.Count());
			//Expecting 2 claimprocs as a final result
			Assert.AreEqual(2,listClaimProcsExpected.Select(x=>x.ClaimProcNum).Count());
			//reset threads
			Thread.CurrentThread.CurrentCulture=cultureOld;
			Thread.CurrentThread.CurrentUICulture=uiCultureOld;
		}

		[TestMethod]
		public void Canada_DeleteAfterValidating_DeletingTotalPaymentLeavesParentProcAlone() {
			//Put into Canada mode
			CultureInfo cultureOld=Thread.CurrentThread.CurrentCulture;
			CultureInfo uiCultureOld=Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-CA");
			//Setup
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);//Have a Patient
			Carrier carrier=CarrierT.CreateCarrier(suffix);//Who has an Insurance Carrier
			string procStr="D0145";
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			ProcedureCodeT.AddIfNotPresent(procStr,false);
			string labProcStr="99111";//Should be 99111
			ProcedureCodeT.AddIfNotPresent(labProcStr,true);
			double procFee=100;
			string toothNum="";
			//Create items that should be deleted
			Procedure procParentDontDelete=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,toothNum,procFee);
			Procedure procLabFee1DontDelete=ProcedureT.CreateProcedure(pat,labProcStr,ProcStat.C,toothNum,procFee,procNumLab:procParentDontDelete.ProcNum);
			List<Procedure> listProcsForClaimDelete=new List<Procedure>() { procParentDontDelete,procLabFee1DontDelete };
			Claim parentClaimDelete=ClaimT.CreateClaim(listProcsForClaimDelete,ins);
		  ClaimProc claimProcParentDontDelete=ClaimProcT.CreateClaimProc(pat.PatNum,procParentDontDelete.ProcNum,carrier.CarrierNum,
				pat.PatNum,claimNum:parentClaimDelete.ClaimNum);
			//Create Pay as Total for Claim
			ClaimProc payAsTotal=ClaimProcT.CreateClaimProc(pat.PatNum,procNum:0,carrier.CarrierNum,
				pat.PatNum,claimNum:parentClaimDelete.ClaimNum);
			ClaimProcs.DeleteAfterValidating(payAsTotal);//Deleting a pay as total so that should be the only thing that is removed from database
			List<long> listExpected=ClaimProcs.Refresh(pat.PatNum).Select(x=>x.ClaimProcNum).ToList();
			Assert.IsTrue(listExpected.Contains(claimProcParentDontDelete.ClaimProcNum));
			//reset threads
			Thread.CurrentThread.CurrentCulture=cultureOld;
			Thread.CurrentThread.CurrentUICulture=uiCultureOld;
		}
		#endregion

		#region Claim Pay As Total Transfer
		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_GeneralBaseCase() {
			//This logic currently runs when opening the income transfer window. Transfers all claims as total for family as supplementals to procedures.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,proc.CodeNum,50));
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc},ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);
			//create new as total claim proc payment.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,100,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			Assert.AreEqual(2,ins.ListAllClaimProcs.Count);
			Assert.AreEqual(1,ins.ListAllClaimProcs.FindAll(x => x.ProcNum==0 && x.InsPayAmt==100).Count);//1 as total
			Assert.AreEqual(1,ins.ListAllClaimProcs.FindAll(x => x.ProcNum==proc.ProcNum && x.InsPayAmt==0).Count);
			//Transfer
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			//listValid will contain the changes that were just made. 
			Assert.AreEqual(2,listValid.Count);
			Assert.AreEqual(0,listValid.FindAll(x => x.IsTransfer==false).Count);
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==0 && x.InsPayAmt==-100).Count);//offset for transfer
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc.ProcNum && x.InsPayAmt==100).Count);//allocation to claim procedure
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_TransferDoesNotReTransferAsTotals() {
			//To ensure that after this logic has ran, it doesn't make any more transfers again.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,proc.CodeNum,50));
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc},ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);
			//create new as total claim proc payment.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,100,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			//Transfer
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			Assert.AreEqual(2,listValid.Count);//listValid will contain the changes that were just made.
			//attempt to transfer again, and verify that none are made.
			listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			Assert.AreEqual(0,listValid.Count);
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_OnlyTransferRecievedAsTotals() {
			//To ensure that after this logic has ran, it doesn't make any more transfers again.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,proc.CodeNum,50));
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc},ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			//create new as total claim proc payment.
			ClaimProc asTotal=ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,100,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			asTotal.Status=ClaimProcStatus.NotReceived;
			ClaimProcs.Update(asTotal);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			//Transfer
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			Assert.AreEqual(0,listValid.Count);//nothing should have transferred
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_NegativeClaimAsTotalTransfersCorrectly() {
			//Should be a rare case, but we do allow users to create negative claim pay as totals.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,proc.CodeNum,50));
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc},ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);
			//create new as total claim proc payment.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,-50,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			//Transfer
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			Assert.AreEqual(2,listValid.Count);//listValid will contain the changes that were just made.
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==0 && x.InsPayAmt==50).Count);//offset for transfer
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc.ProcNum && x.InsPayAmt==-50).Count);//allocation to claim procedure
			decimal patPortion=ClaimProcs.GetPatPortion(proc,ClaimProcs.Refresh(ins.Pat.PatNum));
			Assert.AreEqual(150,patPortion);
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_ClaimAsTotalWithNoValidMatchingProceduresAreInvalid() {
			//This logic currently runs when opening the income transfer window. Transfers all claims as total for family as supplementals to procedures.
			//ClaimPayAsTotal for provider that doesn't match should still transfer
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			long provPrime=ProviderT.CreateProvider("LSPrime");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provPrime);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,proc.CodeNum,50));
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc},ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);
			//create new as total claim proc payment.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,100,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			Assert.AreEqual(2,ins.ListAllClaimProcs.Count);
			Assert.AreEqual(1,ins.ListAllClaimProcs.FindAll(x => x.ProcNum==0 && x.InsPayAmt==100).Count);//1 as total
			Assert.AreEqual(1,ins.ListAllClaimProcs.FindAll(x => x.ProcNum==proc.ProcNum && x.InsPayAmt==0).Count);
			//Transfer
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			Assert.AreEqual(2,listValid.Count);
			Assert.AreEqual(1,listValid.FindAll(x => x.InsPayAmt==100 && x.ProcNum==proc.ProcNum).Count);
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_TransfersForInsPayAndWriteOffsBase() {
			//This logic currently runs when opening the income transfer window. Transfers all claims as total for family as supplementals to procedures.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,proc.CodeNum,50));
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc},ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);
			//create new as total claim proc payment. This one has a write off which will put the procedure as overpaid, but that is fine.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,90,ins.PriInsSub.InsSubNum,0,20,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			Assert.AreEqual(2,ins.ListAllClaimProcs.Count);
			Assert.AreEqual(1,ins.ListAllClaimProcs.FindAll(x => x.ProcNum==0 && x.InsPayAmt==90 && x.WriteOff==20).Count);//1 as total
			Assert.AreEqual(1,ins.ListAllClaimProcs.FindAll(x => x.ProcNum==proc.ProcNum && x.InsPayAmt==0).Count);
			//Transfer
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			//listTransferred will contain the changes that were just made.
			Assert.AreEqual(2,listValid.Count);
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==0 && x.InsPayAmt==-90 && x.WriteOff==-20).Count);//offset for transfer
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc.ProcNum && x.InsPayAmt==90 && x.WriteOff==20).Count);//allocation to claim procedure
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_MultipleProcsAndProvsTransferCorrectly() {
			//just a more complicated happy scenario, meant to look more like a possible claim as user could have.
			//Used to not allow transferring between providers, but now we do
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			long otherProvider=ProviderT.CreateProvider("OtherLS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.GetCodeNum("D0220"),100));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.CreateProcCode("D0221").CodeNum,80));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.CreateProcCode("D0222").CodeNum,67));
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0221",ProcStat.C,"",125,DateTime.Today,provNum:otherProvider);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0222",ProcStat.C,"",150,DateTime.Today,provNum:provNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc1,proc2,proc3},ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ins.ListAllClaimProcs.ForEach(x => x.InsPayEst=100);
			ins.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).WriteOffEst=25;
			ins.ListAllClaimProcs.First(x => x.ProcNum==proc3.ProcNum).WriteOffEst=50;
			ins.ListAllClaimProcs.ForEach(x => ClaimProcs.Update(x));
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);
			//create new as total claim proc payment. This one has a write off which will put the procedure as overpaid, but that is fine.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,300,ins.PriInsSub.InsSubNum,0,75,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			//Assert.AreEqual(1,ins.ListAllClaimProcs.FindAll(x => x.ProcNum==proc1.ProcNum && x.InsPayEst==100).Count);
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			//Should have transferred no money to the incorrect provider, 225 to proc 1 and 150 to proc 3.
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc1.ProcNum && x.InsPayAmt==100 && x.WriteOff==0).Count);
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc2.ProcNum && x.InsPayAmt==100 && x.WriteOff==25).Count);
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc3.ProcNum && x.InsPayAmt==100 && x.WriteOff==50).Count);
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_MultipleClaimsAsTotalOnSameClaimTransferCorrectly() {
			//just a more complicated happy scenario, meant to look more like a possible claim as user could have.
			//Used to not allow transferring between providers, but now we do
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			long otherProvider=ProviderT.CreateProvider("OtherLS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.GetCodeNum("D0220"),100));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.CreateProcCode("D0221").CodeNum,80));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.CreateProcCode("D0222").CodeNum,67));
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0221",ProcStat.C,"",125,DateTime.Today,provNum:otherProvider);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0222",ProcStat.C,"",150,DateTime.Today,provNum:provNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc1,proc2,proc3},ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			ins.ListAllClaimProcs.ForEach(x => x.InsPayEst=100);
			ins.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).WriteOffEst=25;
			ins.ListAllClaimProcs.First(x => x.ProcNum==proc3.ProcNum).WriteOffEst=50;
			ins.ListAllClaimProcs.ForEach(x => ClaimProcs.Update(x));
			ClaimT.ReceiveClaim(claim,ins.ListAllClaimProcs);
			//create new as total claim proc payment. This one has a write off which will put the procedure as overpaid, but that is fine.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,200,ins.PriInsSub.InsSubNum,0,25,claim.ClaimNum);
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,otherProvider,100,ins.PriInsSub.InsSubNum,0,50,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			//Assert.AreEqual(1,ins.ListAllClaimProcs.FindAll(x => x.ProcNum==proc1.ProcNum && x.InsPayEst==100).Count);
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums).ListInsertedClaimProcs;
			//Should have transferred no money to the incorrect provider, 225 to proc 1 and 150 to proc 3.
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc1.ProcNum && x.InsPayAmt==100 && x.WriteOff==0).Count);
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc2.ProcNum && x.InsPayAmt==100 && x.WriteOff==25).Count);
			Assert.AreEqual(1,listValid.FindAll(x => x.ProcNum==proc3.ProcNum && x.InsPayAmt==100 && x.WriteOff==50).Count);
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_ClaimsAsTotalWithNoClaimProcsTransferToUnearned() {
			//unique bug found in converted customers that have claims with pay as totals and no procedures on the claim.
			PrefT.UpdateBool(PrefName.AllowPrepayProvider,true);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=Patients.GetFamily(pat.PatNum).ListPats.Select(x => x.PatNum).ToList();
			long provNum=ProviderT.CreateProvider("LS");
			long otherProvider=ProviderT.CreateProvider("OtherLS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.GetCodeNum("D0220"),100));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.CreateProcCode("D0221").CodeNum,80));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriPatPlan.PatPlanNum,ProcedureCodeT.CreateProcCode("D0222").CodeNum,67));
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Claim claim=new Claim();
			claim.PatNum=pat.PatNum;
			claim.ClaimStatus="R";
			claim.ProvBill=provNum;
			claim.ProvTreat=provNum;
			Claims.Insert(claim);
			//create new as total claim proc payment. This one has a write off which will put the procedure as overpaid, but that is fine.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,200,ins.PriInsSub.InsSubNum,0,25,claim.ClaimNum);
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,otherProvider,100,ins.PriInsSub.InsSubNum,0,50,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			ClaimTransferResult results=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums);
			//Should have made negative supplementals to the pay as totals and then created unearned money
			Assert.AreEqual(1,results.ListInsertedPaySplits.FindAll(x => x.UnearnedType!=0 && x.ProvNum==provNum && x.SplitAmt==225).Count);
			Assert.AreEqual(1,results.ListInsertedPaySplits.FindAll(x => x.UnearnedType!=0 && x.ProvNum==otherProvider && x.SplitAmt==150).Count);
			Assert.AreEqual(2,results.ListInsertedClaimProcs.Count);
			Assert.AreEqual(2,results.ListInsertedClaimProcs.FindAll(x => x.InsPayAmt<0).Count);//only claimprocs inserted should be offsets.
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_ClaimsAsTotalWithNoClaimProcsTransferToUnearnedNoProv() {
			//unique bug found in converted customers that have claims with pay as totals and no procedures on the claim.
			PrefT.UpdateBool(PrefName.AllowPrepayProvider,false);
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=new List<long> { pat.PatNum };
			long provNum=ProviderT.CreateProvider("LS");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"BestCarrier");
			Claim claim=new Claim();
			claim.PatNum=pat.PatNum;
			claim.ClaimStatus="R";
			claim.ProvBill=provNum;
			claim.ProvTreat=provNum;
			Claims.Insert(claim);
			//create new as total claim proc payment.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,200,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			ClaimTransferResult results=ClaimProcs.TransferClaimsAsTotalToProcedures(listFamilyPatNums);
			//Should have made a prepayment not attached to a provider
			Assert.AreEqual(1,results.ListInsertedPaySplits.Count);
			Assert.AreNotEqual(0,results.ListInsertedPaySplits[0].UnearnedType);
			Assert.AreEqual(0,results.ListInsertedPaySplits[0].ProvNum);
			Assert.AreEqual(200,results.ListInsertedPaySplits[0].SplitAmt);
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_SecondaryClaimConsidersOtherInsPayments() {
			//Create two procedures and two claims. Primary will cover 50% of the procedures and will pay by procedure.
			//The secondary claim will have estimates for less than 50% of the procedures but will receive an As Total payment for the other 50%.
			//Therefore, the 'Txfr' supplementals that are created should be for the rest of the procedure amounts.
			//The bug that required this unit test was that the first procedure was getting 'overpaid' due to not considering what primary ins covered.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			//Create two procedures that primary ins should cover 50% and secondary will cover them at 40%.
			Procedure proc=ProcedureT.CreateProcedure(pat,"SCCOIP1",ProcStat.C,"",126,DateTime.Today,provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"SCCOIP2",ProcStat.C,"",241,DateTime.Today,provNum:provNum);
			//Create primary and secondary insurance. 
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,$"Pri-{suffix}",ordinal:1);
			ins.AddInsurance(pat,$"Sec-{suffix}",ordinal:2);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc2.CodeNum,50));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.SecInsPlan.PlanNum,proc.CodeNum,40));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.SecInsPlan.PlanNum,proc2.CodeNum,40));
			//Create primary and secondary claims.
			Claim claimPri=ClaimT.CreateClaim(new List<Procedure>{ proc,proc2 },ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Claim claimSec=ClaimT.CreateClaim(new List<Procedure>{ proc,proc2 },ins,claimType:"S");
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			//Receive primary and pay by procedure utilizing the estimated 50%.
			ClaimT.ReceiveClaim(claimPri,ins.ListAllClaimProcs.FindAll(x => x.ClaimNum==claimPri.ClaimNum),doAddPayAmount:true);
			//Receive secondary but this time pay by total and ignore the estimates by covering the other 50%.
			ClaimT.ReceiveClaim(claimSec,ins.ListAllClaimProcs.FindAll(x => x.ClaimNum==claimSec.ClaimNum),doAddPayAmount:false);
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.SecInsPlan.PlanNum,provNum,183.50,ins.SecInsSub.InsSubNum,0,0,claimSec.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			//Assert the insurance estimates are as desired prior to making the 'Txfr' supplemental payments for the as total payment on the sec claim.
			Assert.AreEqual(5,ins.ListAllClaimProcs.Count);
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc.ProcNum 
				&& x.ClaimNum==claimPri.ClaimNum
				&& x.InsPayEst==63
				&& x.InsPayAmt==63));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc.ProcNum
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==50.40
				&& x.InsPayAmt==0));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc2.ProcNum
				&& x.ClaimNum==claimPri.ClaimNum
				&& x.InsPayEst==120.50
				&& x.InsPayAmt==120.50));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc2.ProcNum
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==96.40
				&& x.InsPayAmt==0));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==0
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==0
				&& x.InsPayAmt==183.50));
			//Invoke the method that will automatically make supplemental 'Txfr' claimprocs in order to distribute the As Total payment to claimprocs.
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(fam.GetPatNums()).ListInsertedClaimProcs;
			//Assert that the supplemental payments were created correctly.
			Assert.AreEqual(3,listValid.Count);
			Assert.IsTrue(listValid.All(x => x.IsTransfer));
			Assert.AreEqual(1,listValid.Count(x => x.ProcNum==0 && x.InsPayAmt==-183.50));//offset for transfer
			Assert.AreEqual(1,listValid.Count(x => x.ProcNum==proc.ProcNum && x.InsPayAmt==63));//allocation to claim procedure 1
			Assert.AreEqual(1,listValid.Count(x => x.ProcNum==proc2.ProcNum && x.InsPayAmt==120.50));//allocation to claim procedure 2
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_SecondaryClaimConsidersOtherInsPaymentsWithOverpayment() {
			//Create two procedures and two claims. Primary will cover 50% of the procedures and will pay by procedure.
			//The secondary claim will have estimates for less than 50% of the procedures but will receive an As Total payment for the other 50%.
			//Therefore, the 'Txfr' supplementals that are created should be for the rest of the procedure amounts.
			//The bug that required this unit test was that the first procedure was getting 'overpaid' due to not considering what primary ins covered.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			//Create two procedures that primary ins should cover 50% and secondary will cover them at 40%.
			Procedure proc=ProcedureT.CreateProcedure(pat,"SCCOIP1",ProcStat.C,"",126,DateTime.Today,provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"SCCOIP2",ProcStat.C,"",241,DateTime.Today,provNum:provNum);
			//Create primary and secondary insurance. 
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,$"Pri-{suffix}",ordinal:1);
			ins.AddInsurance(pat,$"Sec-{suffix}",ordinal:2);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc2.CodeNum,50));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.SecInsPlan.PlanNum,proc.CodeNum,40));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.SecInsPlan.PlanNum,proc2.CodeNum,40));
			//Create primary and secondary claims.
			Claim claimPri=ClaimT.CreateClaim(new List<Procedure>{ proc,proc2 },ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Claim claimSec=ClaimT.CreateClaim(new List<Procedure>{ proc,proc2 },ins,claimType:"S");
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			//Receive primary and pay by procedure utilizing the estimated 50%.
			ClaimT.ReceiveClaim(claimPri,ins.ListAllClaimProcs.FindAll(x => x.ClaimNum==claimPri.ClaimNum),doAddPayAmount:true);
			//Receive secondary but this time pay by total and ignore the estimates by covering the other 50%.
			ClaimT.ReceiveClaim(claimSec,ins.ListAllClaimProcs.FindAll(x => x.ClaimNum==claimSec.ClaimNum),doAddPayAmount:false);
			//Overpay the claim which should cause the supplemental Txfr for the first procedure on the claim to get the leftovers.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.SecInsPlan.PlanNum,provNum,200,ins.SecInsSub.InsSubNum,0,0,claimSec.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			//Assert the insurance estimates are as desired prior to making the 'Txfr' supplemental payments for the as total payment on the sec claim.
			Assert.AreEqual(5,ins.ListAllClaimProcs.Count);
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc.ProcNum 
				&& x.ClaimNum==claimPri.ClaimNum
				&& x.InsPayEst==63
				&& x.InsPayAmt==63));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc.ProcNum
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==50.40
				&& x.InsPayAmt==0));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc2.ProcNum
				&& x.ClaimNum==claimPri.ClaimNum
				&& x.InsPayEst==120.50
				&& x.InsPayAmt==120.50));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc2.ProcNum
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==96.40
				&& x.InsPayAmt==0));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==0
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==0
				&& x.InsPayAmt==200));
			//Invoke the method that will automatically make supplemental 'Txfr' claimprocs in order to distribute the As Total payment to claimprocs.
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(fam.GetPatNums()).ListInsertedClaimProcs;
			//Assert that the supplemental payments were created correctly.
			Assert.AreEqual(3,listValid.Count);
			Assert.IsTrue(listValid.All(x => x.IsTransfer));
			Assert.AreEqual(1,listValid.Count(x => x.ProcNum==0 && x.InsPayAmt==-200));//offset for transfer
			Assert.AreEqual(1,listValid.Count(x => x.ProcNum==proc.ProcNum && x.InsPayAmt==79.50));//allocation to claim procedure 1
			Assert.AreEqual(1,listValid.Count(x => x.ProcNum==proc2.ProcNum && x.InsPayAmt==120.50));//allocation to claim procedure 2
		}

		[TestMethod]
		public void ClaimProcs_TransferClaimsAsTotalToProcedures_SecondaryClaimConsidersOtherAsTotalInsPayments() {
			//Create two procedures and two claims. Primary will cover 50% and the secondary claim cover less than 50% of the procedures.
			//Both claims will receive an As Total payment for 50%.
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Family fam=Patients.GetFamily(pat.PatNum);
			long provNum=ProviderT.CreateProvider(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			//Create two procedures that primary ins should cover 50% and secondary will cover them at 40%.
			Procedure proc=ProcedureT.CreateProcedure(pat,"SCCOIP1",ProcStat.C,"",126,DateTime.Today,provNum:provNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"SCCOIP2",ProcStat.C,"",241,DateTime.Today,provNum:provNum);
			//Create primary and secondary insurance. 
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,$"Pri-{suffix}",ordinal:1);
			ins.AddInsurance(pat,$"Sec-{suffix}",ordinal:2);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc2.CodeNum,50));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.SecInsPlan.PlanNum,proc.CodeNum,40));
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.SecInsPlan.PlanNum,proc2.CodeNum,40));
			//Create primary and secondary claims.
			Claim claimPri=ClaimT.CreateClaim(new List<Procedure>{ proc,proc2 },ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Claim claimSec=ClaimT.CreateClaim(new List<Procedure>{ proc,proc2 },ins,claimType:"S");
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			//Receive primary and insert a singular as total payment for the estimated 50%.
			ClaimT.ReceiveClaim(claimPri,ins.ListAllClaimProcs.FindAll(x => x.ClaimNum==claimPri.ClaimNum),doAddPayAmount:false);
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,183.50,ins.PriInsSub.InsSubNum,0,0,claimPri.ClaimNum);
			//Receive secondary that will also insert a singular as total payment for the other 50%.
			ClaimT.ReceiveClaim(claimSec,ins.ListAllClaimProcs.FindAll(x => x.ClaimNum==claimSec.ClaimNum),doAddPayAmount:false);
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.SecInsPlan.PlanNum,provNum,183.50,ins.SecInsSub.InsSubNum,0,0,claimSec.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			//Assert the insurance estimates are as desired prior to making the 'Txfr' supplemental payments for the as total payment on the sec claim.
			Assert.AreEqual(6,ins.ListAllClaimProcs.Count);
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc.ProcNum 
				&& x.ClaimNum==claimPri.ClaimNum
				&& x.InsPayEst==63
				&& x.InsPayAmt==0));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc.ProcNum
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==50.40
				&& x.InsPayAmt==0));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc2.ProcNum
				&& x.ClaimNum==claimPri.ClaimNum
				&& x.InsPayEst==120.50
				&& x.InsPayAmt==0));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==proc2.ProcNum
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==96.40
				&& x.InsPayAmt==0));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==0
				&& x.ClaimNum==claimPri.ClaimNum
				&& x.InsPayEst==0
				&& x.InsPayAmt==183.50));
			Assert.AreEqual(1,ins.ListAllClaimProcs.Count(x => x.ProcNum==0
				&& x.ClaimNum==claimSec.ClaimNum
				&& x.InsPayEst==0
				&& x.InsPayAmt==183.50));
			//Invoke the method that will automatically make supplemental 'Txfr' claimprocs in order to distribute the As Total payment to claimprocs.
			List<ClaimProc> listValid=ClaimProcs.TransferClaimsAsTotalToProcedures(fam.GetPatNums()).ListInsertedClaimProcs;
			//Assert that the supplemental payments were created correctly.
			Assert.AreEqual(6,listValid.Count);
			Assert.IsTrue(listValid.All(x => x.IsTransfer));
			Assert.AreEqual(1,listValid.Count(x => x.ClaimNum==claimPri.ClaimNum && x.ProcNum==0 && x.InsPayAmt==-183.50));//offset for transfer
			Assert.AreEqual(1,listValid.Count(x => x.ClaimNum==claimPri.ClaimNum && x.ProcNum==proc.ProcNum && x.InsPayAmt==63));//allocation to claim procedure 1
			Assert.AreEqual(1,listValid.Count(x => x.ClaimNum==claimPri.ClaimNum && x.ProcNum==proc2.ProcNum && x.InsPayAmt==120.50));//allocation to claim procedure 2
			Assert.AreEqual(1,listValid.Count(x => x.ClaimNum==claimSec.ClaimNum && x.ProcNum==0 && x.InsPayAmt==-183.50));//offset for transfer
			Assert.AreEqual(1,listValid.Count(x => x.ClaimNum==claimSec.ClaimNum && x.ProcNum==proc.ProcNum && x.InsPayAmt==63));//allocation to claim procedure 1
			Assert.AreEqual(1,listValid.Count(x => x.ClaimNum==claimSec.ClaimNum && x.ProcNum==proc2.ProcNum && x.InsPayAmt==120.50));//allocation to claim procedure 2
		}

		[TestMethod]
		public void ClaimProcs_FixClaimsNoProcedures_ClaimsWithNoProceduresCreateProcedureAndClaimProc() {
			//General base case testing to make sure that claims existing with no procedures (only AsTotals) get a procedure and claimproc created for them.
			//This should happen regardless if a transfer has already occured on the claim or not. 
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=new List<long> { pat.PatNum };
			long provNum=ProviderT.CreateProvider("LS");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"BestCarrier");
			Claim claim=new Claim();
			claim.PatNum=pat.PatNum;
			claim.ClaimStatus="R";
			claim.ProvBill=provNum;
			claim.ProvTreat=provNum;
			Claims.Insert(claim);
			//create new AsTotal payment for the claim.
			ClaimProc asTotal=ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,100,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			ClaimProcs.FixClaimsNoProcedures(new List<long> {pat.PatNum });
			ProcedureCode fixCode=ProcedureCodes.GetFirstOrDefault(x => x.ProcCode=="ZZZFIX");
			Assert.IsNotNull(fixCode);
			Procedure dummyProcedure=Procedures.GetCompleteForPats(new List<long>{pat.PatNum})
				.FirstOrDefault(x => x.ProcFee==0 && x.CodeNum==fixCode.CodeNum);
			Assert.IsNotNull(dummyProcedure);
			Assert.AreEqual(1,ClaimProcs.GetForProcs(new List<long>{ dummyProcedure.ProcNum })
				.Count(x => x.ClaimNum==claim.ClaimNum
					&& x.ClinicNum==dummyProcedure.ClinicNum
					&& x.DateCP==asTotal.DateCP
					&& x.PatNum==dummyProcedure.PatNum
					&& x.ProvNum==dummyProcedure.ProvNum
					&& x.InsPayEst==0));
		}

		[TestMethod]
		public void ClaimProcs_FixClaimsNoProcedures_ClaimsWithNoProcedurePreviouslyFinalizedAndTransferred() {
			//In this case the claim that has no procedure based claimprocs has already been finalized by the user and is likely for a date far in the past.
			//This case is for an account where a transfer has already taken place. Because of this, the procedure and claimproc should get created, but another
			//transfer between the as total and procedure should not occur. 
			//We need to make sure that the procdure created and the claimproc both have the backdated date, and that another transfer has not happened.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=new List<long> { pat.PatNum };
			long provNum=ProviderT.CreateProvider("LS");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",65,provNum:provNum);//specifically not attached to claim
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"BestCarrier");
			#region Create Claim
			Claim claim=new Claim();
			claim.PatNum=pat.PatNum;
			claim.ClaimStatus="R";
			claim.ProvBill=provNum;
			claim.ProvTreat=provNum;
			Claims.Insert(claim);
			#endregion
			#region Create Single As Total
			//create initial as total for claim
			ClaimProc asTotal=ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,100,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			#endregion
			#region Finalized ClaimPayment
			ClaimPayment claimpay=new ClaimPayment();
			claimpay.CheckAmt=100;
			claimpay.CheckDate=asTotal.DateCP;
			ClaimPayments.Insert(claimpay);
			asTotal.ClaimPaymentNum=claimpay.ClaimPaymentNum;
			ClaimProcs.Update(asTotal);
			#endregion
			#region Perform Original Claim Transfer
			//Perform initial claim transfer to simulate the state of the DB before the fix which created the dummy procedure and claimproc.
			//This will create a patient payment as well
			ClaimProcs.TransferClaimsAsTotalToProcedures(new List<long>{pat.PatNum});
			List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			Assert.AreEqual(2,listClaimProcsForClaim.Count);//should be 2, the initial and the negative transfer.
			#endregion
			//Now make another transfer to apply the newer code 
			Payment txfrPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.IncomeTransferData txfrResults=PaymentT.BalanceAndIncomeTransfer(pat.PatNum,Patients.GetFamily(pat.PatNum),txfrPayment);
			Procedure dummyProcedure=Procedures.GetCompleteForPats(new List<long>{pat.PatNum}).Where(x => x.ProcFee==0).FirstOrDefault();
			Assert.AreEqual(asTotal.DateCP,dummyProcedure.ProcDate);
			Assert.AreEqual(0,dummyProcedure.ProcFee);
			listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			Assert.AreEqual(1,listClaimProcsForClaim.FindAll(x => x.ProcNum==dummyProcedure.ProcNum).Count);//only the initial should have been created. 
			Assert.AreEqual(3,listClaimProcsForClaim.Count);//2 previous plus the new dummy
			Assert.AreEqual(2,txfrResults.ListSplitsCur.Count);//moves the unearned patient payment to the procedure
			Assert.AreEqual(1,txfrResults.ListSplitsCur.Count(x => x.UnearnedType!=0 && x.SplitAmt==-65));
			Assert.AreEqual(1,txfrResults.ListSplitsCur.Count(x => x.ProcNum==proc.ProcNum && x.SplitAmt==65));
		}

		[TestMethod]
		public void ClaimProcs_FixClaimsNoProcedures_ClaimsWithNoProcedurePreviouslyFinalizedAndTransferredAndPatientPaymentDeleted() {
			//In this case the claim that has no procedure based claimprocs has already been finalized by the user and is likely for a date far in the past.
			//This case is for an account where a transfer has already taken place AND where the user deleted the patient payment that was made.
			//Because of this, the account in unbalanced. We can't do anything to fix this, the user needs to fix the inbalance manually.
			//We will create the procedure and claimproc, but another transfer between the as total and procedure should not occur. 
			//We need to make sure that the procdure created and the claimproc both have the backdated date, and that another transfer has not happened.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=new List<long> { pat.PatNum };
			long provNum=ProviderT.CreateProvider("LS");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",65,provNum:provNum);//specifically not attached to claim
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"BestCarrier");
			#region Create Claim
			Claim claim=new Claim();
			claim.PatNum=pat.PatNum;
			claim.ClaimStatus="R";
			claim.ProvBill=provNum;
			claim.ProvTreat=provNum;
			Claims.Insert(claim);
			#endregion
			#region Create Single As Total
			//create initial as total for claim
			ClaimProc asTotal=ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,100,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			#endregion
			#region Finalize Claim Payment
			ClaimPayment claimpay=new ClaimPayment();
			claimpay.CheckAmt=100;
			claimpay.CheckDate=asTotal.DateCP;
			ClaimPayments.Insert(claimpay);
			asTotal.ClaimPaymentNum=claimpay.ClaimPaymentNum;
			ClaimProcs.Update(asTotal);
			#endregion
			#region Perform Original Claim Transfer
			//Perform initial claim transfer to simulate the state of the DB before the fix which created the dummy procedure and claimproc.
			ClaimProcs.TransferClaimsAsTotalToProcedures(new List<long>{pat.PatNum});
			List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			Assert.AreEqual(2,listClaimProcsForClaim.Count);//should be 2, the initial and the negative transfer.
			#endregion
			#region Delete Transferred Patient Payment
			//Simulate the user going in and deleting the patient payment that was just created. 
			List<Payment> listPaymentsForPat=Payments.Refresh(pat.PatNum);
			Assert.AreEqual(1,listPaymentsForPat.Count);
			Payments.Delete(listPaymentsForPat.First().PayNum);
			#endregion
			//Now make another transfer to apply the newer code 
			Payment txfrPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.IncomeTransferData txfrResults=PaymentT.BalanceAndIncomeTransfer(pat.PatNum,Patients.GetFamily(pat.PatNum),txfrPayment);
			Procedure dummyProcedure=Procedures.GetCompleteForPats(new List<long>{pat.PatNum}).Where(x => x.ProcFee==0).FirstOrDefault();
			Assert.AreEqual(asTotal.DateCP,dummyProcedure.ProcDate);
			Assert.AreEqual(0,dummyProcedure.ProcFee);
			listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			Assert.AreEqual(1,listClaimProcsForClaim.FindAll(x => x.ProcNum==dummyProcedure.ProcNum).Count);//only the initial should have been created. 
			Assert.AreEqual(3,listClaimProcsForClaim.Count);//2 previous plus the new dummy
			Assert.AreEqual(0,txfrResults.ListSplitsCur.Count);//We expect nothing to transfer since the user deleted the patient payment. 
			//Their account is unbalanced and they will need to fix this manually.
		}

		[TestMethod]
		public void ClaimProcs_FixClaimsNoProcedures_ClaimsWithNoProceduresRecentlyFinalizedTransferred() {
			//In this case the claim that has no procedure based claimprocs has not yet been finalized by the user, but a transfer has already taken place.
			//The procedure and claimproc should get created, but another transfer between the as total and procedure should not occur. 
			//We need to make sure that the procdure created and the claimproc both have the backdated date, and that another transfer has not happened.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=new List<long> { pat.PatNum };
			long provNum=ProviderT.CreateProvider("LS");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",65,provNum:provNum);//specifically not attached to claim
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"BestCarrier");
			#region Create Claim
			Claim claim=new Claim();
			claim.PatNum=pat.PatNum;
			claim.ClaimStatus="R";
			claim.ProvBill=provNum;
			claim.ProvTreat=provNum;
			Claims.Insert(claim);
			#endregion
			#region Create Single As Total
			//create initial as total for claim
			ClaimProc asTotal=ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,100,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			#endregion
			#region Perform Original Claim Transfer
			//Perform initial claim transfer to simulate the state of the DB before the fix which created the dummy procedure and claimproc.
			ClaimProcs.TransferClaimsAsTotalToProcedures(new List<long>{pat.PatNum});
			List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			Assert.AreEqual(2,listClaimProcsForClaim.Count);//should be 2, the initial and the negative transfer.
			#endregion
			#region Finalize Claim Payment
			ClaimPayment claimpay=new ClaimPayment();
			claimpay.CheckAmt=0;
			claimpay.CheckDate=asTotal.DateCP;
			ClaimPayments.Insert(claimpay);
			asTotal.ClaimPaymentNum=claimpay.ClaimPaymentNum;
			ClaimProc negTransfer=listClaimProcsForClaim.First(x => x.IsTransfer && x.InsPayAmt < 0);
			negTransfer.ClaimPaymentNum=claimpay.ClaimPaymentNum;
			ClaimProcs.Update(negTransfer);
			ClaimProcs.Update(asTotal);
			#endregion
			//Now make another transfer to apply the newer code 
			Payment txfrPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.IncomeTransferData txfrResults=PaymentT.BalanceAndIncomeTransfer(pat.PatNum,Patients.GetFamily(pat.PatNum),txfrPayment);
			Procedure dummyProcedure=Procedures.GetCompleteForPats(new List<long>{pat.PatNum}).Where(x => x.ProcFee==0).FirstOrDefault();
			Assert.AreEqual(asTotal.DateCP,dummyProcedure.ProcDate);
			Assert.AreEqual(0,dummyProcedure.ProcFee);
			listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			Assert.AreEqual(1,listClaimProcsForClaim.Count(x => x.IsTransfer));//should only have the 1 negative
			Assert.AreEqual(1,listClaimProcsForClaim.FindAll(x => x.ProcNum==dummyProcedure.ProcNum).Count);//only the initial should have been created. 
			Assert.AreEqual(3,listClaimProcsForClaim.Count);//2 previous plus the new dummy
			Assert.AreEqual(2,txfrResults.ListSplitsCur.Count);//moves the unearned patient payment to the procedure
			Assert.AreEqual(1,txfrResults.ListSplitsCur.Count(x => x.UnearnedType!=0 && x.SplitAmt==-65));
			Assert.AreEqual(1,txfrResults.ListSplitsCur.Count(x => x.ProcNum==proc.ProcNum && x.SplitAmt==65));
		}

		[TestMethod]
		public void ClaimProcs_FixClaimsNoProcedures_ClaimsWithNoProceduresNotYetTransferred() {
			//In this case, this is an account that has a claim with a no procedure claimprocs, but it has not yet been transferred.
			//This claim is for the past (finalized), we want to make the claimproc and procedure and then transfer the as total into the procedure. 
			//After that, we need to make an income transfer which should move the money from the $0 proc to the original proc that actually has value.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=new List<long> { pat.PatNum };
			long provNum=ProviderT.CreateProvider("LS");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"BestCarrier");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",65,provNum:provNum);//specifically not attached to claim
			#region Create Claim
			Claim claim=new Claim();
			claim.PatNum=pat.PatNum;
			claim.ClaimStatus="R";
			claim.ProvBill=provNum;
			claim.ProvTreat=provNum;
			Claims.Insert(claim);
			#endregion
			#region Create Single As Total
			//create new AsTotal payment for the claim.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum,65,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			#endregion
			Payment txfrPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.IncomeTransferData txfrResults=PaymentT.BalanceAndIncomeTransfer(pat.PatNum);
			Procedure dummyProcedure=Procedures.GetCompleteForPats(new List<long>{pat.PatNum}).Where(x => x.ProcFee==0).FirstOrDefault();
			List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			//should be 2 dummy claimprocs the first is the initial 0, and the second gets created during the transfer to hold the As Total amount. 
			Assert.AreEqual(2,listClaimProcsForClaim.FindAll(x => x.ProcNum==dummyProcedure.ProcNum).Count);
			Assert.AreEqual(2,listClaimProcsForClaim.Count(x => x.IsTransfer));//negative offset and positive to the dummy procedure
		}

		[TestMethod]
		public void ClaimProcs_FixClaimsNoProcedures_ClaimsWithNoProceduresNotYetTransferredDifferentProviders() {
			//In this case, this is an account that has a claim with a no procedure claimprocs, but it has not yet been transferred.
			//This claim also has two as totals on it for different providers. We will still create two procedures, but they will only transfer to the first
			//provider. After the claim transfer happens, we need to run the income transfer to ensure that the second provider does eventually get their money.
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			List<long> listFamilyPatNums=new List<long> { pat.PatNum };
			long provNum1=ProviderT.CreateProvider("LS1");
			long provNum2=ProviderT.CreateProvider("LS2");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,"BestCarrier");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",53,provNum:provNum1);//specifically not attached to claim
			Procedure procProv2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",25,provNum:provNum2);//specifically not attached to claim
			#region Create Claim
			Claim claim=new Claim();
			claim.PatNum=pat.PatNum;
			claim.ClaimStatus="R";
			claim.ProvBill=provNum1;
			claim.ProvTreat=provNum1;
			Claims.Insert(claim);
			#endregion
			#region Create Original As Totals
			//create new AsTotal payment for the claim.
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum1,53,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,ins.PriInsPlan.PlanNum,provNum2,25,ins.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(ins.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			#endregion
			Payment txfrPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,0);
			PaymentEdit.IncomeTransferData txfrResults=PaymentT.BalanceAndIncomeTransfer(pat.PatNum);
			ProcedureCode codeFix=ProcedureCodes.GetOne("ZZZFIX");
			List<Procedure> listDummyProcedures=Procedures.GetCompleteForPats(new List<long>{pat.PatNum}).FindAll(x => x.CodeNum==codeFix.CodeNum);
			Assert.AreEqual(2,listDummyProcedures.Count);
			Procedure dummyProcedure1=listDummyProcedures.First(x => x.ProvNum==provNum1);
			Procedure dummyProcedure2=listDummyProcedures.First(x => x.ProvNum==provNum2);
			List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			//Since ClaimProcs.FixClaimsNoProcedures() does not order the claimprocs in any particular order it will pick one at random.
			//Therefore, one of the dummy procedures will have two claimprocs and the other will only have one.
			if(listClaimProcsForClaim.Count(x => x.ProcNum==dummyProcedure1.ProcNum)==2) {
				Assert.AreEqual(2,listClaimProcsForClaim.Count(x => x.ProcNum==dummyProcedure1.ProcNum));
				Assert.AreEqual(1,listClaimProcsForClaim.Count(x => x.ProcNum==dummyProcedure2.ProcNum));
			}
			else {
				Assert.AreEqual(1,listClaimProcsForClaim.Count(x => x.ProcNum==dummyProcedure1.ProcNum));
				Assert.AreEqual(2,listClaimProcsForClaim.Count(x => x.ProcNum==dummyProcedure2.ProcNum));
			}
			//should be 4 dummy claimprocs. 1 will get created initially for each provider (2 total). Then another 1 supplemental will get created 
			//for transferring the money into since there is not ins est on the original dummy claimprocs. 
			Assert.AreEqual(3,listClaimProcsForClaim.Count(x => x.IsTransfer));//negative offset and positive to the dummy procedure
		}
		#endregion

		#region Apply AsTotal Payments
		///<summary>Makes a ApplyAsTotalPayment at the same InsPayEst value of the claim proc.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_AtInsPayEstSumPayment() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			double paymentToMake=100;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(arrayClaimProcs[0].InsPayAmt,100);
			Assert.AreEqual(arrayClaimProcs[0].Status,ClaimProcStatus.Received);
		}

		///<summary>Tests the case where a user pays below the summed amount of multiple claim procs.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_BelowInsPayEstSumPayment1() {
			List<Procedure> listProcedures=new List<Procedure>();
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			double paymentToMake=100;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(100,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(0,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Tests the case where a user pays below the summed amount of multiple claim procs.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_BelowInsPayEstSumPayment2() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			claimProc.DateCP.AddDays(1);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"D0210",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			listProcedures.Add(proc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			Claim claim=ClaimT.CreateClaim(listProcedures,ins);
			Procedures.ComputeEstimatesForAll(pat.PatNum,ins.ListAllClaimProcs,ins.ListAllProcs,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,pat.Age,
				ins.ListInsSubs);
			double paymentToMake=100;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=ClaimProcs.RefreshForClaim(claim.ClaimNum).ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(50,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(50,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Tests the case where a user has created a claim, set it to recieved, added a supplemental, and set everything else to not recieved.
		///Lastly the user then clicks pay as total again.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_SupplementalInclusion() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			claimProc.LineNumber=2;
			claimProc.DateCP=DateTime.Now.AddDays(-2);
			listClaimProcs.Add(claimProc); 
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,insPayAmt:25,cps:ClaimProcStatus.Supplemental);
			claimProc.LineNumber=1;
			claimProc.DateCP=DateTime.Now.AddDays(-1);
			listClaimProcs.Add(claimProc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,insPayAmt:75);
			claimProc.LineNumber=1;
			claimProc.DateCP=DateTime.Now.AddDays(-1);
			listClaimProcs.Add(claimProc);
			double paymentToMake=100;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(75,arrayClaimProcs[2].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[2].Status);
			Assert.AreEqual(25,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Supplemental,arrayClaimProcs[1].Status);
			Assert.AreEqual(100,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
		}

		///<summary>Tests the case that a user makes an 'As Total' payment, then sets the claimprocs to not received, and makes more 'As Total' payments.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_MulitpleApplyAsTotalPayments() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,insPayAmt:0);
			claimProc.DateCP=DateTime.Now.AddDays(1);
			claimProc.LineNumber=2;
			listClaimProcs.Add(claimProc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,insPayAmt:0);
			claimProc.DateCP=DateTime.Now;
			claimProc.LineNumber=1;
			listClaimProcs.Add(claimProc);
			double paymentToMake=100;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			arrayClaimProcs[0].Status=ClaimProcStatus.NotReceived;
			arrayClaimProcs[1].Status=ClaimProcStatus.NotReceived;
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			arrayClaimProcs[0].Status=ClaimProcStatus.NotReceived;
			arrayClaimProcs[1].Status=ClaimProcStatus.NotReceived;
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(100,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
			Assert.AreEqual(200,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
		}

		///<summary>Tests the case where a claim proc has a previous insPayAmt value.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_PreviousInsPayment() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,75));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,insPayAmt:50,feeBilled:100);
			claimProc.DateCP=DateTime.Now.AddDays(1);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"", ProcStat.C,"",100);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			listProcedures.Add(proc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,insPayAmt:0,feeBilled:100);
			claimProc.DateCP=DateTime.Now;
			listClaimProcs.Add(claimProc);
			double paymentToMake=100;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			//We need to check if we want pay as total to block out recieved payments
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(75,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
			Assert.AreEqual(75,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
		}

		///<summary>Tests the case where the user pay amount is over the InsPayEst, and below the Procedure Fee.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_AtProcFeePayment() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",200,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			double paymentToMake=150;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(150,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
		}

		///<summary>Tests the case where the user pay amount is over the sum of the InsPayEst, and below the sum of the Procedure Fee.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_BelowProcFeeSumPayment() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",200,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,feeBilled:proc.ProcFee);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"D0210", ProcStat.C,"",250);
			listProcedures.Add(proc);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,40));
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,feeBilled:proc.ProcFee);
			listClaimProcs.Add(claimProc);
			double paymentToMake=250;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(150,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(100,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Tests the case where the user pay amount is over the sum of the InsPayEst, and at the sum of the Procedure Fee.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_AtProcFeeSumPayment() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",200,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"", ProcStat.C,"",250);
			listProcedures.Add(proc);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,40));
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			double paymentToMake=450;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(200,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(250,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Tests the case where the user pay amount is over the sum of the InsPayEst, and over the sum of the Procedure Fee.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_Overpayment() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",200,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,50));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"", ProcStat.C,"",250);
			listProcedures.Add(proc);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,40));
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			double paymentToMake=550;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(300,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(250,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Tests the case where the user pay amount is over the sum of the InsPayEst, and over the sum of the Procedure Fee.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_EdgeCaseZeroInsPayEst() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",200,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,0));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"D0210", ProcStat.C,"",100);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,0));
			listProcedures.Add(proc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			double paymentToMake=300;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(200,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(100,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Tests the case where the user pay amount is over the sum of the InsPayEst, and over the sum of the Procedure Fee.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_EdgeCaseZeroInsPayEstAndProc() {
			List<Procedure> listProcedures=new List<Procedure>();
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"", ProcStat.C,"",0);
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"", ProcStat.C,"",0);
			listProcedures.Add(proc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			double paymentToMake=550;
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(550,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(0,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Tests the case where the ProcFee is negative as is the InsPayEst.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_EdgeCaseNegativeInsPayEstAndProc() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",-100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"", ProcStat.C,"",-100);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			listProcedures.Add(proc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0);
			listClaimProcs.Add(claimProc);
			double paymentToMake=550;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(550,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(0,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Tests the case where the ProcFee is negative as is the InsPayEst and a negative InsPayAmt that is less than InsPayEst.</summary>
		[TestMethod]
		public void ClaimProcs_ApplyAsTotalPayment_EdgeCaseNegativeInsPayEstAndProcWithNegativeInsPayAmt() {
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",-100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(proc);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			ClaimProc claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,insPayAmt:-150);
			listClaimProcs.Add(claimProc);
			proc=ProcedureT.CreateProcedure(pat,"", ProcStat.C,"",-100);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			listProcedures.Add(proc);
			claimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,0,0,insPayAmt:-150);
			listClaimProcs.Add(claimProc);
			double paymentToMake=550;
			UpdateInsEst(listClaimProcs,listProcedures,pat,ins);
			ClaimProc[] arrayClaimProcs=listClaimProcs.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			Assert.AreEqual(350,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(-100,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClaimProcs_ApplyAsTotalPayment_InsuranceEstimateGreaterThanFeeBilled)]
		[Documentation.VersionAdded("21.1")]
		[Documentation.Description("Patient has Category Percentage primary insurance, subscriber self. The 'Claim Pay by Total splits automatically' preference is enabled. Two procedures are Tp'd, D0220 with procedure fee of $100 and D0150 with a fee of $25. Both procedures are covered at 100%. A Pre-Auth is sent and received with the insurance estimated to cover $125 on D0220. Complete both procedures and send a claim. Receiving the claim and clicking the Pay as Total button should correctly apply the $125 between both procedures.")]
		public void ClaimProcs_ApplyAsTotalPayment_InsuranceEstimateGreaterThanFeeBilled() {
			//Update Preference to a 'Claim pay by total splits automatically' is true
			Prefs.UpdateBool(PrefName.ClaimPayByTotalSplitsAuto,true);
			Prefs.RefreshCache();
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,100));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",25,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc2.CodeNum,100));
			List<Procedure> listProcedures=new List<Procedure>();
			List<ClaimProc> _listClaimProcsForProc=new List<ClaimProc>();
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(ins.ListInsPlans);
			listProcedures.Add(proc);
			listProcedures.Add(proc2);
			//Create claimprocs for each of the procedures
			//This is the same way we create claimprocs on the OK click in the edit procedure edit window.
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(pat.PatNum,ins.ListBenefits,ins.ListPatPlans,ins.ListInsPlans,-1,DateTime.Today,ins.ListInsSubs);
			Procedures.ComputeEstimates(proc,pat.PatNum,ref _listClaimProcsForProc,false,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,
				histList,new List<ClaimProcHist> { },true,pat.Age,ins.ListInsSubs,null,false,false,listSubLinks,false,null);
			Procedures.ComputeEstimates(proc2,pat.PatNum,ref _listClaimProcsForProc,false,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,
				histList,new List<ClaimProcHist> { },true,pat.Age,ins.ListInsSubs,null,false,false,listSubLinks,false,null);
			//Create A preauth claim and associate each of the procedures.
			Claim claimPreAuth=ClaimT.CreateClaim(listProcedures,ins,claimType:"PreAuth");
			List<ClaimProc> listClaimProcsAll=ClaimProcs.GetForProcs(listProcedures.Select(x => x.ProcNum).ToList());
			List<ClaimProc> listClaimProcsPreAuth=ClaimProcs.GetForProcs(listProcedures.Select(x => x.ProcNum).ToList()).FindAll(x => x.ClaimNum==claimPreAuth.ClaimNum);
			double totalProcFees=listProcedures.Sum(x => x.ProcFeeTotal);
			//pretend preauth got received. This is what we do when we receive the claim and enter the ins est amount.
			//Add the entire insurance payment amount to the first procedure
			for(int i=0;i<listClaimProcsPreAuth.Count;i++) {
				listClaimProcsPreAuth[i].Status=ClaimProcStatus.Preauth;
				if(i==0) {
					//Add the total procedure fee estimates for the claim to the first procedure and set the InsEstTotalOverride so that recalculations cannot change this fact.
					listClaimProcsPreAuth[i].InsPayEst=totalProcFees;
					listClaimProcsPreAuth[i].InsEstTotalOverride=totalProcFees;
					listClaimProcsPreAuth[i].FeeBilled=proc.ProcFee;
				}
				else {
					listClaimProcsPreAuth[i].InsPayEst=0;
					listClaimProcsPreAuth[i].InsEstTotalOverride=0;
					listClaimProcsPreAuth[i].FeeBilled=proc2.ProcFee;
				}
				ClaimProcs.Update(listClaimProcsPreAuth[i]);
			}
			//Create a regular claim for the procedures. 
			//This is the claim that will get used to create the Pay As Total
			Claim claim=ClaimT.CreateClaim(listProcedures,ins);
			listClaimProcsAll=ClaimProcs.GetForProcs(listProcedures.Select(x => x.ProcNum).ToList());
			//Remove the extra claim proces that the CreateClaim method created.
			//The claim proces were created above already.
			ClaimProcs.DeleteMany(listClaimProcsAll.FindAll(x => x.ClaimNum==claim.ClaimNum));
			List<ClaimProc> listClaimProcsNotAttached=listClaimProcsAll.FindAll(x => x.ClaimNum==0);
			//Associate the claimprocs to the regular claim.
			for(int i=0;i<listClaimProcsNotAttached.Count;i++) {
				listClaimProcsNotAttached[i].ClaimNum=claim.ClaimNum;
				listClaimProcsNotAttached[i].Status=ClaimProcStatus.NotReceived;
				ClaimProcs.Update(listClaimProcsNotAttached[i]);
			}
			UpdateInsEst(listClaimProcsAll,listProcedures,pat,ins);
			double paymentToMake=totalProcFees;
			listClaimProcsAll=ClaimProcs.GetForProcs(listProcedures.Select(x => x.ProcNum).ToList()).FindAll(x => x.Status!=ClaimProcStatus.Preauth);
			ClaimProc[] arrayClaimProcs=listClaimProcsAll.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			//Check to make sure the auto split preference is used and the procedures get paid correctly.
			Assert.AreEqual(100,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(25,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ClaimProcs_ApplyAsTotalPayment_TotalAmtPaidGreaterThanTotalProcFees)]
		[Documentation.VersionAdded("21.1")]
		[Documentation.Description("Patient has Category Percentage primary insurance, subscriber self. The 'Claim Pay by Total splits automatically' preference is enabled. Two procedures are Tp'd, D0220 with procedure fee of $100 and D0150 with a fee of $25. Both procedures are covered at 100%. A Pre-Auth is sent and received with the insurance estimated to cover $135 on D0220. Complete both procedures and send a claim. Receiving the claim and clicking the Pay as Total button should correctly apply the $125 between both procedures and the overpayment of $10 should be applied to the first procedure on the claim.")]
		public void ClaimProcs_ApplyAsTotalPayment_TotalAmtPaidGreaterThanTotalProcFees() {
			//Update Preference to a 'Claim pay by total splits automatically' is true
			Prefs.UpdateBool(PrefName.ClaimPayByTotalSplitsAuto,true);
			Prefs.RefreshCache();
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider("LS");
			Carrier carrier=CarrierT.CreateCarrier("BestCarrier");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",100,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc.CodeNum,80));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0150",ProcStat.C,"",25,DateTime.Today,provNum:provNum);
			ins.AddBenefit(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,proc2.CodeNum,80));
			List<Procedure> listProcedures=new List<Procedure>();
			List<ClaimProc> _listClaimProcsForProc=new List<ClaimProc>();
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(ins.ListInsPlans);
			listProcedures.Add(proc);
			listProcedures.Add(proc2);
			//Create claimprocs for each of the procedures
			//This is the same way we create claimprocs on the OK click in the edit procedure edit window.
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(pat.PatNum,ins.ListBenefits,ins.ListPatPlans,ins.ListInsPlans,-1,DateTime.Today,ins.ListInsSubs);
			Procedures.ComputeEstimates(proc,pat.PatNum,ref _listClaimProcsForProc,false,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,
				histList,new List<ClaimProcHist> { },true,pat.Age,ins.ListInsSubs,null,false,false,listSubLinks,false,null);
			Procedures.ComputeEstimates(proc2,pat.PatNum,ref _listClaimProcsForProc,false,ins.ListInsPlans,ins.ListPatPlans,ins.ListBenefits,
				histList,new List<ClaimProcHist> { },true,pat.Age,ins.ListInsSubs,null,false,false,listSubLinks,false,null);
			//Create A preauth claim and associate each of the procedures.
			Claim claimPreAuth=ClaimT.CreateClaim(listProcedures,ins,claimType:"PreAuth");
			List<ClaimProc> listClaimProcsAll=ClaimProcs.GetForProcs(listProcedures.Select(x => x.ProcNum).ToList());
			List<ClaimProc> listClaimProcsPreAuth=ClaimProcs.GetForProcs(listProcedures.Select(x => x.ProcNum).ToList()).FindAll(x => x.ClaimNum==claimPreAuth.ClaimNum);
			double totalProcFees=listProcedures.Sum(x => x.ProcFeeTotal);
			//pretend preauth got received. This is what we do when we receive the claim and enter the ins est amount.
			//Add the entire insurance payment amount to the first procedure
			for(int i = 0;i<listClaimProcsPreAuth.Count;i++) {
				listClaimProcsPreAuth[i].Status=ClaimProcStatus.Preauth;
				if(i==0) {
					//Add the total procedure fee estimates for the claim to the first procedure and set the InsEstTotalOverride so that recalculations cannot change this fact.
					listClaimProcsPreAuth[i].InsPayEst=totalProcFees;
					listClaimProcsPreAuth[i].InsEstTotalOverride=totalProcFees;
					listClaimProcsPreAuth[i].FeeBilled=proc.ProcFee;
				}
				else {
					listClaimProcsPreAuth[i].InsPayEst=0;
					listClaimProcsPreAuth[i].InsEstTotalOverride=0;
					listClaimProcsPreAuth[i].FeeBilled=proc2.ProcFee;
				}
				ClaimProcs.Update(listClaimProcsPreAuth[i]);
			}
			//Create a regular claim for the procedures. 
			//This is the claim that will get used to create the Pay As Total
			Claim claim=ClaimT.CreateClaim(listProcedures,ins);
			listClaimProcsAll=ClaimProcs.GetForProcs(listProcedures.Select(x => x.ProcNum).ToList());
			//Remove the extra claim proces that the CreateClaim method created.
			//The claim proces were created above already.
			ClaimProcs.DeleteMany(listClaimProcsAll.FindAll(x => x.ClaimNum==claim.ClaimNum));
			List<ClaimProc> listClaimProcsNotAttached=listClaimProcsAll.FindAll(x => x.ClaimNum==0);
			//Associate the claimprocs to the regular claim.
			for(int i = 0;i<listClaimProcsNotAttached.Count;i++) {
				listClaimProcsNotAttached[i].ClaimNum=claim.ClaimNum;
				listClaimProcsNotAttached[i].Status=ClaimProcStatus.NotReceived;
				ClaimProcs.Update(listClaimProcsNotAttached[i]);
			}
			UpdateInsEst(listClaimProcsAll,listProcedures,pat,ins);
			//$25 overpayment. The overpayment should go to the first procedure
			double paymentToMake=totalProcFees+25;
			listClaimProcsAll=ClaimProcs.GetForProcs(listProcedures.Select(x => x.ProcNum).ToList()).FindAll(x => x.Status!=ClaimProcStatus.Preauth);
			ClaimProc[] arrayClaimProcs=listClaimProcsAll.ToArray();
			ClaimProcs.ApplyAsTotalPayment(ref arrayClaimProcs,paymentToMake,listProcedures);
			//Check to make sure the auto split preference is used and the procedures get paid correctly.
			Assert.AreEqual(125,arrayClaimProcs[0].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[0].Status);
			Assert.AreEqual(25,arrayClaimProcs[1].InsPayAmt);
			Assert.AreEqual(ClaimProcStatus.Received,arrayClaimProcs[1].Status);
		}

		///<summary>Simple helper function that sets the Estimate values for a list of ClaimProcs.</summary>
		private void UpdateInsEst(List<ClaimProc> listClaimProcs, List<Procedure> listProcedures,Patient pat, InsuranceInfo ins) {
			for(int i = 0;i<listClaimProcs.Count;i++) {
				ClaimProc cp=listClaimProcs[i];
				Procedure proc=listProcedures.Select(x=>x).Where(x=>x.ProcNum==cp.ProcNum).FirstOrDefault();
				ClaimProcs.ComputeBaseEst(cp,proc,ins.PriInsPlan,ins.PriPatPlan.PatPlanNum,ins.ListBenefits,null,null,ins.ListPatPlans,0,0,pat.Age,
					0,ins.ListInsPlans,ins.ListInsSubs,ins.ListSubLinks,false,null,null);
				ClaimProcs.Update(cp);
			}
		}
		#endregion

		private class BenefitsAssertItem {
			public Procedure Procedure;
			public ClaimProc PrimaryClaimProc;

			public BenefitsAssertItem() { }

		}
	}
}
