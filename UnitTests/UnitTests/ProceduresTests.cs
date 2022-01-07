using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Procedures_Test {
	[TestClass]
	public class ProceduresTest:TestBase {
		
		#region Medicaid COB
		///<summary>Secondary insurance has a COB rule of Medicaid. Tests that secondary writeoff is correct.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_MedicaidCOBSecWO1() {
			ComputeEstimatesMedicaidCOB(MethodBase.GetCurrentMethod().Name,100,70,20,50,100,
				((claimProcPri,claimProcSec,proc) => {
					Assert.AreEqual(35,claimProcSec.WriteOffEst);
				})
			);
		}

		///<summary>Secondary insurance has a COB rule of Medicaid. Tests that patient portion is correct.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_MedicaidCOBPatPort1() {
			ComputeEstimatesMedicaidCOB(MethodBase.GetCurrentMethod().Name,100,70,20,50,100,
				((claimProcPri,claimProcSec,proc) => {
					double patPort=proc.ProcFeeTotal-claimProcPri.InsEstTotal-claimProcPri.WriteOffEst
						-claimProcSec.InsEstTotal-claimProcSec.WriteOffEst;
					Assert.AreEqual(0,patPort);
				})
			);
		}

		///<summary>Secondary insurance has a COB rule of Medicaid. Tests that secondary writeoff is correct.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_MedicaidCOBSecWO2() {
			ComputeEstimatesMedicaidCOB(MethodBase.GetCurrentMethod().Name,100,40,30,50,100,
				((claimProcPri,claimProcSec,proc) => {
					Assert.AreEqual(10,claimProcSec.WriteOffEst);
				})
			);
		}

		///<summary>Secondary insurance has a COB rule of Medicaid. Tests that secondary ins pay is correct.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_MedicaidCOBSecInsPay2() {
			ComputeEstimatesMedicaidCOB(MethodBase.GetCurrentMethod().Name,100,40,30,50,100,
				((claimProcPri,claimProcSec,proc) => {
					Assert.AreEqual(10,claimProcSec.InsPayEst);
				})
			);
		}

		///<summary>Secondary insurance has a COB rule of Medicaid. Tests that patient portion is correct.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_MedicaidCOBPatPort2() {
			ComputeEstimatesMedicaidCOB(MethodBase.GetCurrentMethod().Name,100,40,30,50,100,
				((claimProcPri,claimProcSec,proc) => {
					double patPort=proc.ProcFeeTotal-claimProcPri.InsPayEst-claimProcPri.WriteOffEst
						-claimProcSec.InsPayEst-claimProcSec.WriteOffEst;
					Assert.AreEqual(0,patPort);
				})
			);
		}

		///<summary>Creates a procedure and computes estimates for a patient where the secondary insurance has a COB rule of Medicaid.</summary>
		private void ComputeEstimatesMedicaidCOB(string suffix,double procFee,double priAllowed,double secAllowed,int priPercentCovered,
			int secPercentCovered,Action<ClaimProc/*Primary*/,ClaimProc/*Secondary*/,Procedure> assertAct) 
		{
			Patient pat=PatientT.CreatePatient(suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceT.AddInsurance(pat,suffix,"p",ppoFeeSchedNum);
			long medicaidFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceT.AddInsurance(pat,suffix,"p",medicaidFeeSchedNum,2,cobRule: EnumCobRule.SecondaryMedicaid);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			BenefitT.CreateCategoryPercent(priPlan.PlanNum,EbenefitCategory.Diagnostic,priPercentCovered);
			InsPlan secPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Secondary,listPatPlans,listPlans,listSubs);
			BenefitT.CreateCategoryPercent(secPlan.PlanNum,EbenefitCategory.Diagnostic,secPercentCovered);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			string procStr="D0150";
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			FeeT.CreateFee(ppoFeeSchedNum,procCode.CodeNum,priAllowed);
			FeeT.CreateFee(medicaidFeeSchedNum,procCode.CodeNum,secAllowed);
			Procedures.ComputeEstimates(proc,pat.PatNum,new List<ClaimProc>(),false,listPlans,listPatPlans,listBens,pat.Age,listSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			assertAct(listClaimProcs.FirstOrDefault(x => x.PlanNum==priPlan.PlanNum),listClaimProcs.FirstOrDefault(x => x.PlanNum==secPlan.PlanNum),proc);
		}

		[TestMethod]
		public void Procedures_ComputeEstimates_PrimaryInsuranceMedicaidCOB() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix,"p",ppoFeeSchedNum,1,cobRule: EnumCobRule.SecondaryMedicaid);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,insInfo.ListPatPlans,insInfo.ListInsPlans,insInfo.ListInsSubs);
			BenefitT.CreateCategoryPercent(priPlan.PlanNum,EbenefitCategory.Diagnostic,percent: 50);
			List<Benefit> listBens=Benefits.Refresh(insInfo.ListPatPlans,insInfo.ListInsSubs);
			string procStr="D0150";
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",125);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			FeeT.CreateFee(ppoFeeSchedNum,procCode.CodeNum,amount: 80);
			Procedures.ComputeEstimates(proc,pat.PatNum,new List<ClaimProc>(),false,insInfo.ListInsPlans,insInfo.ListPatPlans,listBens,pat.Age,
				insInfo.ListInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(40,listClaimProcs[0].InsEstTotal,0.001);
			Assert.AreEqual(45,listClaimProcs[0].WriteOffEst,0.001);
		}

		#endregion Medicaid COB

		#region Fixed Benefits

		///<summary>.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_FixedBenefitNoFixedBenefitFeeAmtNoPpoFee() {
			ComputeEstimatesFixedBenefits(MethodBase.GetCurrentMethod().Name,100,-1,0,-1,false,0,false,0,
				((assertItem) => {
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(100,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.WriteOffEst);
				})
			);
		}

		///<summary>.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_FixedBenefitNoFixedBenefitFeeAmt() {
			ComputeEstimatesFixedBenefits(MethodBase.GetCurrentMethod().Name,100,55,0,-1,false,0,false,0,
				((assertItem) => {
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(55,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(45,assertItem.PrimaryClaimProc.WriteOffEst);
				})
			);
		}

		///<summary>.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_FixedBenefitNoPpoFee() {
			ComputeEstimatesFixedBenefits(MethodBase.GetCurrentMethod().Name,100,-1,12,-1,false,0,false,0,
				((assertItem) => {
					Assert.AreEqual(12,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(88,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(0,assertItem.PrimaryClaimProc.WriteOffEst);
				})
			);
		}

		///<summary>.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_FixedBenefitWithAllFees() {
			ComputeEstimatesFixedBenefits(MethodBase.GetCurrentMethod().Name,100,55,12,-1,false,0,false,0,
				((assertItem) => {
					Assert.AreEqual(12,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(43,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(45,assertItem.PrimaryClaimProc.WriteOffEst);
				})
			);
		}

		///<summary>.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_FixedBenefitWithAllFeesAndSubstitution() {
			ComputeEstimatesFixedBenefits(MethodBase.GetCurrentMethod().Name,100,55,12,-1,false,0,true,5,
				((assertItem) => {
					Assert.AreEqual(5,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(50,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(45,assertItem.PrimaryClaimProc.WriteOffEst);
				})
			);
		}

		///<summary>.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_FixedBenefitWithPercentOverride() {
			ComputeEstimatesFixedBenefits(MethodBase.GetCurrentMethod().Name,100,55,12,25,false,0,false,0,
				((assertItem) => {
					Assert.AreEqual(3,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(43,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(45,assertItem.PrimaryClaimProc.WriteOffEst);
				})
			);
		}

		///<summary>.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_FixedBenefitWithSecondaryIns() {
			ComputeEstimatesFixedBenefits(MethodBase.GetCurrentMethod().Name,100,55,12,-1,true,15,false,0,
				((assertItem) => {
					Assert.AreEqual(12,assertItem.PrimaryClaimProc.InsEstTotal);
					Assert.AreEqual(43,assertItem.PrimaryClaimProc.CopayAmt);
					Assert.AreEqual(45,assertItem.PrimaryClaimProc.WriteOffEst);
					Assert.AreEqual(43,assertItem.SecondaryClaimProc.InsEstTotal);
				})
			);
		}

		///<summary>Creates a procedure and computes estimates for a patient where the secondary insurance has a COB rule of Medicaid.</summary>
		private void ComputeEstimatesFixedBenefits(string suffix,double procFee,double ppoFee,double fixedBenefitFee
			,int priPercentCoveredOverride,bool hasSecondary,double secFee,bool hasSubstitution,double fixedBenefitFeeSub,Action<FixedBenefitAssertItem> assertAct)
		{
			Patient pat=PatientT.CreatePatient(suffix);
			string procStr="D0150";
			string procStrSubst="D0160";
			ProcedureCodeT.AddIfNotPresent("D0150");
			ProcedureCodeT.AddIfNotPresent("D0160");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			procCode.SubstitutionCode=hasSubstitution ? procStrSubst : "";
			ProcedureCodes.Update(procCode);
			Cache.Refresh(InvalidType.ProcCodes);
			ProcedureCode procCodeSubst=ProcedureCodeT.CreateProcCode(procStrSubst);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			long catPercFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Category % "+suffix);
			long fixedBenefitFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.FixedBenefit,"Fixed Benefit "+suffix);
			if(ppoFee>-1) {
				FeeT.CreateFee(ppoFeeSchedNum,procCode.CodeNum,ppoFee);
			}
			FeeT.CreateFee(ppoFeeSchedNum,procCodeSubst.CodeNum,ppoFee);
			FeeT.CreateFee(fixedBenefitFeeSchedNum,procCode.CodeNum,fixedBenefitFee);
			FeeT.CreateFee(fixedBenefitFeeSchedNum,procCodeSubst.CodeNum,fixedBenefitFeeSub);
			InsuranceT.AddInsurance(pat,suffix,"p",ppoFeeSchedNum,copayFeeSchedNum: fixedBenefitFeeSchedNum);
			if(hasSecondary) {
				FeeT.CreateFee(catPercFeeSchedNum,procCode.CodeNum,secFee);
				InsuranceT.AddInsurance(pat,suffix,"",catPercFeeSchedNum,2,false,EnumCobRule.Standard);
			}
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			InsPlan secPlan=null;
			if(hasSecondary) {
				secPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Secondary,listPatPlans,listPlans,listSubs);
				//TODO:  Add diagnostic code benefit for 100%
				BenefitT.CreateCategoryPercent(secPlan.PlanNum,EbenefitCategory.Diagnostic,100);
			}
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			Procedures.ComputeEstimates(proc,pat.PatNum,new List<ClaimProc>(),false,listPlans,listPatPlans,listBens,pat.Age,listSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			if(priPercentCoveredOverride>0) {
				foreach(ClaimProc cpCur in listClaimProcs) {
					cpCur.PercentOverride=priPercentCoveredOverride;
					ClaimProcs.Update(cpCur);
				}
				Procedures.ComputeEstimates(proc,pat.PatNum,new List<ClaimProc>(),false,listPlans,listPatPlans,listBens,pat.Age,listSubs);
				listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			}
			foreach(ClaimProc cpCur in listClaimProcs) {
				cpCur.PercentOverride=priPercentCoveredOverride;
				ClaimProcs.Update(cpCur);
			}
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,false,listPlans,listPatPlans,listBens,pat.Age,listSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			assertAct(new FixedBenefitAssertItem() {
				Procedure=proc,
				PrimaryClaimProc=listClaimProcs.FirstOrDefault(x => x.PlanNum==priPlan.PlanNum),
				SecondaryClaimProc=secPlan==null ? null : listClaimProcs.FirstOrDefault(x => x.PlanNum==secPlan.PlanNum),
			});
		}

		private class FixedBenefitAssertItem {
			public Procedure Procedure;
			public ClaimProc PrimaryClaimProc;
			public ClaimProc SecondaryClaimProc;
		}

		#endregion

		#region GetProcFee

		///<summary>The procedure fee for a medical insurance ppo is the UCR fee.</summary>
		[TestMethod]
		public void Procedures_GetProcFee_WithMedicalInsurance() {
			double procFee=GetProcFee(MethodBase.GetCurrentMethod().Name,false);
			Assert.AreEqual(300,procFee);
		}

		///<summary>The procedure fee for a medical insurance ppo is the UCR fee of the medical code.</summary>
		[TestMethod]
		public void Procedures_GetProcFee_WithMedicalCode() {
			double procFee=GetProcFee(MethodBase.GetCurrentMethod().Name,true);
			Assert.AreEqual(300,procFee);
		}

		///<summary>Creates a procedure and returns its procedure fee.</summary>
		private double GetProcFee(string suffix,bool doUseMedicalCode) {
			Prefs.UpdateBool(PrefName.InsPpoAlwaysUseUcrFee,true);
			Prefs.UpdateBool(PrefName.MedicalFeeUsedForNewProcs,doUseMedicalCode);
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR "+suffix);
			FeeSchedT.UpdateUCRFeeSched(pat,ucrFeeSchedNum);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO "+suffix);
			InsuranceT.AddInsurance(pat,suffix,"p",ppoFeeSchedNum,1,true);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			string procStr="D0150";
			string procStrMed="D0120";
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			ProcedureCode procCodeMed=ProcedureCodeT.CreateProcCode(procStrMed);
			procCode.MedicalCode=procCodeMed.ProcCode;
			FeeT.CreateFee(ucrFeeSchedNum,procCode.CodeNum,300);
			FeeT.CreateFee(ppoFeeSchedNum,procCode.CodeNum,120);
			FeeT.CreateFee(ucrFeeSchedNum,procCodeMed.CodeNum,175);
			FeeT.CreateFee(ppoFeeSchedNum,procCodeMed.CodeNum,85);
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",300);
			return Procedures.GetProcFee(pat,listPatPlans,listSubs,listPlans,procCode.CodeNum,proc.ProvNum,proc.ClinicNum,procCode.MedicalCode);
		}

		///<summary>When a procedure has a category covered at 0%, it should be treated as an exclusion and use UCR fee.</summary>
		[TestMethod]
		public void Procedures_GetProcFee_With0PercentCoverage() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR "+suffix);
			FeeSchedT.UpdateUCRFeeSched(pat,ucrFeeSchedNum);
			long insFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.OutNetwork,"Ins "+suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,"",insFeeSchedNum,exclusionRule:ExclusionRule.UseUcrFee);
			ins.AddBenefit(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Crowns,0));
			string procStr="D2740";//crown proc
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			FeeT.CreateFee(ucrFeeSchedNum,procCode.CodeNum,600);
			FeeT.CreateFee(insFeeSchedNum,procCode.CodeNum,400);
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",600);
			proc.ProcFee=Procedures.GetProcFee(pat,ins.ListPatPlans,ins.ListInsSubs,ins.ListInsPlans,procCode.CodeNum,proc.ProvNum,proc.ClinicNum,
				procCode.MedicalCode);
			Assert.AreEqual(600,proc.ProcFee);//Should use UCR fee
		}

		#endregion GetProcFee

		#region IsSameProcedureArea
		[TestMethod]
		public void Procedures_IsSameProcedureArea_SameToothNums() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("2","2","","","","",TreatmentArea.Tooth));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_DifferentToothNums() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("","1","","","","",TreatmentArea.Tooth));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_PartialMatchToothNums() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("22","2","","","","",TreatmentArea.Tooth));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_EmptyProcCurToothRange() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("","","2,4,7","","","",TreatmentArea.ToothRange));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_PartialMatchToothRanges() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("","","7,16,22","2","","",TreatmentArea.ToothRange));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_ToothRangeMatchingTails() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("","","7,3","15,7,3","","",TreatmentArea.ToothRange));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_ToothRangeMatchingElementsOutOfOrder() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("","","15,7,3,4","7,15","","",TreatmentArea.ToothRange));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_OneToothRangeBlank() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("","","1,4","","","",TreatmentArea.Tooth));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_InvalidToothRangeEntry() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("","","1,,5,","","","",TreatmentArea.ToothRange));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_InvalidEntryWithMatch() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("","","1,,4","4","","",TreatmentArea.ToothRange));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_SameToothNumEmptySurf() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("4","4","","","","M",TreatmentArea.Tooth));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_SameToothNumSameSurf() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("9","9","","","L","L",TreatmentArea.Tooth));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_SameToothNumPartialSurfMatch() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("5","5","","","ML","M",TreatmentArea.Tooth));
		}
		
		[TestMethod]
		public void Procedures_IsSameProcedureArea_SameToothNumEmptySurfs() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("14","14","","","","",TreatmentArea.Tooth));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_HasNoArea() {
			//E.g. exams and BW's will not have any 'procedure areas' and should return true
			Assert.IsTrue(Procedures.IsSameProcedureArea("","","","","","",TreatmentArea.Tooth));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_ArchesNotMatching() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("","","","","U","L",TreatmentArea.Arch));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_QuadsNotMatching() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("","","","","UL","UR",TreatmentArea.Quad));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_SextantsNotMatching() {
			Assert.IsFalse(Procedures.IsSameProcedureArea("","","","","1","2",TreatmentArea.Sextant));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_Arches() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("","","","","U","U",TreatmentArea.Arch));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_Quads() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("","","","","UL","UL",TreatmentArea.Quad));
		}

		[TestMethod]
		public void Procedures_IsSameProcedureArea_Sextants() {
			Assert.IsTrue(Procedures.IsSameProcedureArea("","","","","1","1",TreatmentArea.Sextant));
		}

		#endregion

		#region InsHist

		///<summary>Update procedure date on an existing procedure. Test that the procedure and claimproc has the updated procedure date.</summary>
		[TestMethod]
		public void Procedures_InsertOrUpdateInsHistProcedure_ExistingProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime date=DateTime.Today;
			Procedure proc=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.EO,"",0,procDate:date,surf:"UL");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Create a Claimproc with status InsHist
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,date,-1,-1,-1,ClaimProcStatus.InsHist);
			//Change the proc date
			date=date.AddDays(4);
			List<ClaimProc> listClaimProcsForEoProcs;
			Dictionary<PrefName,Procedure> listEoProcs=Procedures.GetDictInsHistProcs(pat.PatNum,priClaimProc.InsSubNum,out listClaimProcsForEoProcs);
			//The procedure and claimproc need the new date.
			Procedures.InsertOrUpdateInsHistProcedure(pat,PrefName.InsHistPerioULCodes,date,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,listEoProcs[PrefName.InsHistPerioULCodes],listClaimProcsForEoProcs);
			Procedure procFromDb=Procedures.GetOneProc(proc.ProcNum,false);
			Assert.AreEqual(date,procFromDb.ProcDate);
			ClaimProc claimProcFromDb=ClaimProcs.GetForProcs(new List<long> { proc.ProcNum }).FirstOrDefault();
			Assert.AreEqual(date,claimProcFromDb.ProcDate);
		}

		///<summary>Update procedure date on an existing Completed procedure. Test that a new procedure and InsHist claimproc is created with the new procedure date.</summary>
		[TestMethod]
		public void Procedures_InsertOrUpdateInsHistProcedure_ExistingCProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime date=DateTime.Today;
			Procedure proc=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.C,"",0,procDate:date,surf:"UL");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListAllProcs.Add(proc);
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc },ins);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claim,listClaimProcs);
			//Change the proc date
			date=date.AddDays(4);
			List<ClaimProc> listClaimProcsForEoProcs;
			Dictionary<PrefName,Procedure> dictEOAndCProcs=Procedures.GetDictInsHistProcs(pat.PatNum,ins.PriInsSub.InsSubNum,out listClaimProcsForEoProcs);
			//The procedure and claimproc need the new date.
			Procedures.InsertOrUpdateInsHistProcedure(pat,PrefName.InsHistPerioULCodes,date,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,dictEOAndCProcs[PrefName.InsHistPerioULCodes],listClaimProcsForEoProcs);
			Procedure procFromDb=Procedures.Refresh(pat.PatNum).FirstOrDefault(x=>x.ProcDate.Date==date.Date);
			Assert.AreEqual(date.Date,procFromDb.ProcDate.Date);
			Assert.AreEqual(ProcStat.EO,procFromDb.ProcStatus);
			ClaimProc claimProcFromDb=ClaimProcs.GetForProcs(new List<long> { procFromDb.ProcNum }).FirstOrDefault();
			Assert.AreEqual(date.Date,claimProcFromDb.ProcDate.Date);
			Assert.AreEqual(ClaimProcStatus.InsHist,claimProcFromDb.Status);
		}

		///<summary>Update procedure date on an existing Completed procedure. Test that a new procedure and InsHist claimproc is created with the new procedure date.</summary>
		[TestMethod]
		public void Procedures_GetDictInsHistProcs_ExistingCProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime date=DateTime.Today;
			Procedure proc=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.C,"",0,procDate:date,surf:"UL");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListAllProcs.Add(proc);
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc },ins);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claim,listClaimProcs);
			List<ClaimProc> listClaimProcsForEoProcs;
			Dictionary<PrefName,Procedure> dictEOAndCProcs=Procedures.GetDictInsHistProcs(pat.PatNum,ins.PriInsSub.InsSubNum,out listClaimProcsForEoProcs);
			Procedure procFromDict=dictEOAndCProcs[PrefName.InsHistPerioULCodes];
			Assert.AreEqual(date.Date,procFromDict.ProcDate.Date);
			Assert.AreEqual(ProcStat.C,procFromDict.ProcStatus);
		}

		///<summary>New procedures is added. Test that the new procecdure has the correct date,status, and inssubnum.</summary>
		[TestMethod]
		public void Procedures_InsertOrUpdateInsHistProcedure_NewProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			DateTime date=DateTime.Today;
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			List<ClaimProc> listClaimProcsForEoProcs;
			Dictionary<PrefName,Procedure> listEoProcs=Procedures.GetDictInsHistProcs(pat.PatNum,ins.ListInsSubs.FirstOrDefault().InsSubNum,out listClaimProcsForEoProcs);
			//The procedure and claimproc need the new date.
			Procedures.InsertOrUpdateInsHistProcedure(pat,PrefName.InsHistProphyCodes,date,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,listEoProcs[PrefName.InsHistProphyCodes],listClaimProcsForEoProcs);
			Procedure procFromDb=Procedures.GetProcsByStatusForPat(pat.PatNum,ProcStat.EO).FirstOrDefault();
			Assert.AreEqual(date,procFromDb.ProcDate);
			Assert.AreEqual(ProcStat.EO,procFromDb.ProcStatus);
			ClaimProc claimProcFromDb=ClaimProcs.GetForProcs(new List<long> { procFromDb.ProcNum }).FirstOrDefault();
			Assert.AreEqual(date,claimProcFromDb.ProcDate);
			Assert.AreEqual(ClaimProcStatus.InsHist,claimProcFromDb.Status);
			Assert.AreEqual(ins.ListInsSubs.FirstOrDefault().InsSubNum,claimProcFromDb.InsSubNum);
		}
		#endregion

		#region RpProcOverpaid
		[TestMethod]
		public void Procedures_RpProcOverpaid_HasOverPay() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,DateTime.Today);
			insInfo.ListAllProcs.Add(proc1);
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			foreach(ClaimProc cp in listClaimProcs) {
				cp.WriteOff=33;
				cp.InsPayAmt=25;
			}
			ClaimT.ReceiveClaim(claim,listClaimProcs);
			//Procedure has credits equal to 58
			AdjustmentT.MakeAdjustment(pat.PatNum,-20,procDate: proc1.ProcDate,procNum: proc1.ProcNum);//-adjustment=-20
			PaymentT.MakePayment(pat.PatNum,11,proc1.ProcDate,provNum: proc1.ProvNum,procNum: proc1.ProcNum,clinicNum: proc1.ClinicNum);//Pat pay=11
			//ProcFee=88. Total credits=89. Overpayment=-1
			DataTable table=RpProcOverpaid.GetOverPaidProcs(pat.PatNum,new List<long> { proc1.ProvNum },new List<long> { proc1.ClinicNum },proc1.ProcDate,proc1.ProcDate);
			Assert.AreEqual(1,table.Rows.Count);
			Assert.IsTrue(PIn.Double(table.Rows[0]["Overpay"].ToString())==-1);
		}

		[TestMethod]
		public void Procedures_RpProcOverpaid_NotOverPay() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,DateTime.Today);
			insInfo.ListAllProcs.Add(proc1);
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			foreach(ClaimProc cp in listClaimProcs) {
				cp.WriteOff=33;
				cp.InsPayAmt=25;
			}
			ClaimT.ReceiveClaim(claim,listClaimProcs);
			//Procedure has credits equal to 58
			AdjustmentT.MakeAdjustment(pat.PatNum,-20,procDate: proc1.ProcDate,procNum: proc1.ProcNum);//-adjustment=-20
			PaymentT.MakePayment(pat.PatNum,10,proc1.ProcDate,provNum: proc1.ProvNum,procNum: proc1.ProcNum,clinicNum: proc1.ClinicNum);//Pat pay=10
			//ProcFee=88. Total credits=88. Overpayment=0
			DataTable table=RpProcOverpaid.GetOverPaidProcs(pat.PatNum,new List<long> { proc1.ProvNum },new List<long> { proc1.ClinicNum },proc1.ProcDate,proc1.ProcDate);
			Assert.AreEqual(0,table.Rows.Count);
		}
		#endregion

		#region ShouldFeesChange

		///<summary>Patient has a category insurance plan with a fee schedule attached. When changing the provider, makes sure that it doesn't prompt
		///to change the fee.</summary>
		[TestMethod]
		public void Procedures_ShouldFeesChange_CatPercent() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateInt(PrefName.ProcFeeUpdatePrompt,2);//Prompt if fee changes
			Patient pat=PatientT.CreatePatient(suffix);
			long feeSchedIns=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			long feeSchedProv2=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			long provNum1=ProviderT.CreateProvider("Prov1");
			long provNum2=ProviderT.CreateProvider("Prov2",feeSchedNum:feeSchedProv2);
			long codeNum=ProcedureCodeT.GetCodeNum("D1110");
			FeeT.CreateFee(feeSchedIns,codeNum,100);
			FeeT.CreateFee(feeSchedProv2,codeNum,110);
			List<Fee> listFees=Fees.GetListForScheds(feeSchedIns,feeSched2:feeSchedProv2);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix,feeSchedNum:feeSchedIns);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",100,new DateTime(2018,1,10),provNum:provNum1);
			Procedure procOld=proc.Copy();
			proc.ProvNum=provNum2;
			string promptText="";
			ProcFeeHelper procFeeHelper=new ProcFeeHelper(pat.PatNum);
			//Proc fee should stay the same because the old fee is the same as the insurance fee schedule ($100). 
			Assert.IsFalse(Procedures.ShouldFeesChange(new List<Procedure> { proc },new List<Procedure> { procOld },ref promptText,procFeeHelper));
		}

		///<summary>Patient has a category insurance plan with a fee schedule attached where the providers have fee overrides. When changing the 
		///provider, makes sure that it doesn't prompt to change the fee.</summary>
		[TestMethod]
		public void Procedures_ShouldFeesChange_CatPercentProvOverride() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateInt(PrefName.ProcFeeUpdatePrompt,2);//Prompt if fee changes
			Patient pat=PatientT.CreatePatient(suffix);
			long provNum1=ProviderT.CreateProvider("Prov1");
			long provNum2=ProviderT.CreateProvider("Prov2");
			long feeSchedIns=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix,isGlobal:true);
			long codeNum=ProcedureCodeT.GetCodeNum("D1110");
			FeeT.CreateFee(feeSchedIns,codeNum,100,provNum: provNum1);
			FeeT.CreateFee(feeSchedIns,codeNum,90,provNum: provNum2);
			List<Fee> listFees=Fees.GetListForScheds(feeSchedIns,prov1:provNum1,feeSched2:feeSchedIns,prov2:provNum2);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix,feeSchedNum:feeSchedIns);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",100,new DateTime(2018,1,10),provNum:provNum1);
			Procedure procOld=proc.Copy();
			proc.ProvNum=provNum2;
			string promptText="";
			ProcFeeHelper procFeeHelper=new ProcFeeHelper(pat.PatNum);
			//Proc fee should change because the new provider has an override for the insurance fee schedule that is different than the current fee.
			Assert.IsTrue(Procedures.ShouldFeesChange(new List<Procedure> { proc },new List<Procedure> { procOld },ref promptText,procFeeHelper));
		}

		#endregion ShouldFeesChange

		#region FrequencyLimitations
		///<summary>Tests that the implementation of BenefitLogic.ComputeLimitationDate() inside of HasMetFrequencyLimitation() does not alter logic 
		///for a valid proc on a leap year.</summary>
		[TestMethod]
		public void Procedures_HasMetFrequencyLimitation_LeapYear() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D1110
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.Years,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.NumberInLast12Months)
			);
			DateTime proc1Date=new DateTime(2019,2,28);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,proc1Date);
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			DateTime proc2Date=new DateTime(2020,2,29);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,proc2Date);
			//This will compute estimates.
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			List<ClaimProcHist> listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc2.ProcDate,insInfo.ListInsSubs
			);
			//2nd proc has not reached frequency limitation.
			Assert.IsFalse(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc2.ProcNum),listHistList,insInfo.ListBenefits,proc2,
				ProcedureCodes.GetProcCode(proc2.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
			DateTime proc3Date=new DateTime(2020,2,28);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",66,proc3Date);
			insInfo.ListAllProcs.Add(proc3);
			//This will compute estimates.
			listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc3.ProcDate,insInfo.ListInsSubs
			);
			//3rd proc has reached frequency limitation.
			Assert.IsTrue(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc3.ProcNum),listHistList,insInfo.ListBenefits,proc3,
				ProcedureCodes.GetProcCode(proc3.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
		}

		///<summary>Tests that the implementation of BenefitLogic.ComputeLimitationDate() inside of HasMetFrequencyLimitation() does not alter logic
		///when the BenefitQuantity is NumberOfServices and the BenefitTimePeriod is NumberInLast12Months.</summary>
		[TestMethod]
		public void Procedures_HasMetFrequencyLimitation_NumberOfServices_NumberInLast12Months() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D1110
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.NumberInLast12Months)
			);
			DateTime proc1Date=new DateTime(2019,2,28);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,proc1Date);
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			DateTime proc2Date=new DateTime(2020,2,29);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,proc2Date);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			List<ClaimProcHist> listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc2.ProcDate,insInfo.ListInsSubs
			);
			//2nd proc has not reached frequency limitation.
			Assert.IsFalse(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc2.ProcNum),listHistList,insInfo.ListBenefits,proc2,
				ProcedureCodes.GetProcCode(proc2.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
			DateTime proc3Date=new DateTime(2019,6,28);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",66,proc3Date);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc3.ProcDate,insInfo.ListInsSubs
			);
			//3rd proc has reached frequency limitation.
			Assert.IsTrue(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc3.ProcNum),listHistList,insInfo.ListBenefits,proc3,
				ProcedureCodes.GetProcCode(proc3.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
		}

		///<summary>Tests that the implementation of BenefitLogic.ComputeLimitationDate() inside of HasMetFrequencyLimitation() does not alter logic
		///when the BenefitQuantity is NumberOfServices and the BenefitTimePeriod is CalendarYear.</summary>
		[TestMethod]
		public void Procedures_HasMetFrequencyLimitation_NumberOfServices_CalendarYear() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D1110
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.CalendarYear)
			);
			DateTime proc1Date=new DateTime(2019,1,1);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,proc1Date);
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			DateTime proc2Date=new DateTime(2020,1,2);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,proc2Date);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			List<ClaimProcHist> listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc2.ProcDate,insInfo.ListInsSubs
			);
			//2nd proc has not reached frequency limitation.
			Assert.IsFalse(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc2.ProcNum),listHistList,insInfo.ListBenefits,proc2,
				ProcedureCodes.GetProcCode(proc2.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
			DateTime proc3Date=new DateTime(2019,12,31);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",66,proc3Date);
			insInfo.ListAllProcs.Add(proc3);
			//This will compute estimates.
			listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc3.ProcDate,insInfo.ListInsSubs
			);
			//3rd proc has reached frequency limitation.
			Assert.IsTrue(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc3.ProcNum),listHistList,insInfo.ListBenefits,proc3,
				ProcedureCodes.GetProcCode(proc3.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
		}

		///<summary>Tests that the implementation of BenefitLogic.ComputeLimitationDate() inside of HasMetFrequencyLimitation() does not alter logic
		///when the BenefitQuantity is NumberOfServices and the BenefitTimePeriod is ServiceYear.</summary>
		[TestMethod]
		public void Procedures_HasMetFrequencyLimitation_NumberOfServices_ServiceYear() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D1110
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.ServiceYear)
			);
			insInfo.PriInsPlan.MonthRenew=(byte)((DateTime.Today.Month+1) % 12);
			DateTime proc1Date=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddYears(-1).AddMonths(1);//Last year.
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,proc1Date);
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			DateTime proc2Date=proc1Date.AddYears(1).AddDays(1);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,proc2Date);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			List<ClaimProcHist> listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc2.ProcDate,insInfo.ListInsSubs
			);
			//2nd proc has not reached frequency limitation.
			Assert.IsFalse(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc2.ProcNum),listHistList,insInfo.ListBenefits,proc2,
				ProcedureCodes.GetProcCode(proc2.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
			DateTime proc3Date=proc1Date.AddYears(1);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,proc3Date);
			insInfo.ListAllProcs.Add(proc3);
			//This will compute estimates.
			listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc3.ProcDate,insInfo.ListInsSubs
			);
			//2nd proc has reached frequency limitation.
			Assert.IsTrue(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc3.ProcNum),listHistList,insInfo.ListBenefits,proc3,
				ProcedureCodes.GetProcCode(proc3.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
		}

		///<summary>Tests that the implementation of BenefitLogic.ComputeLimitationDate() inside of HasMetFrequencyLimitation() does not alter logic
		///when the BenefitQuantity is NumberOfServices and the BenefitTimePeriod is LifeTime (and would fall into the "else" case of the 
		///BenefitLogic.ComputeLimitationDate() method.</summary>
		[TestMethod]
		public void Procedures_HasMetFrequencyLimitation_NumberOfServices_Other() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D1110
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.Lifetime)
			);
			DateTime proc1Date=new DateTime(2019,1,1);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,proc1Date);
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			DateTime proc2Date=new DateTime(2020,1,2);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,proc2Date);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			List<ClaimProcHist> listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc2.ProcDate,insInfo.ListInsSubs
			);
			//2nd proc has not reached frequency limitation.
			Assert.IsFalse(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc2.ProcNum),listHistList,insInfo.ListBenefits,proc2,
				ProcedureCodes.GetProcCode(proc2.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
			DateTime proc3Date=new DateTime(2019,12,31);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",66,proc3Date);
			insInfo.ListAllProcs.Add(proc3);
			//This will compute estimates.
			listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc3.ProcDate,insInfo.ListInsSubs
			);
			//3rd proc has not reached frequency limitation because LifeTime always returns false.
			Assert.IsFalse(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc3.ProcNum),listHistList,insInfo.ListBenefits,proc3,
				ProcedureCodes.GetProcCode(proc3.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
		}

		///<summary>Creates two procs, one with a date of Feb 28th, another with March 30th. With our old logic in 
		///HasMetFrequencyLimitation (as of 10-06-2019) this logic would return true because we looked "backwards" from the processed procedure. 
		///This means that we'd look back from March 30th, get Feb 28th, then see if we could charge ahead and say "no we can't" even though we technicaly 
		///can since a month has passed (Feb 28-March 28).  Now we adjust the calculated date forward a few days.</summary>
		[TestMethod]
		public void Procedures_HasMetFrequencyLimitation_DifferentLengthMonth() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D1110
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.Months,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.CalendarYear)
			);
			DateTime proc1Date=new DateTime(2019,2,28);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,proc1Date);
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			DateTime proc2Date=new DateTime(2019,3,30);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,proc2Date);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			List<ClaimProcHist> listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc2.ProcDate,insInfo.ListInsSubs
			);
			//2nd proc has not reached frequency limitation.
			Assert.IsFalse(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc2.ProcNum),listHistList,insInfo.ListBenefits,proc2,
				ProcedureCodes.GetProcCode(proc2.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
			DateTime proc3Date=new DateTime(2019,3,28);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,proc3Date);
			insInfo.ListAllProcs.Add(proc3);
			//This will compute estimates.
			listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			listHistList=ClaimProcs.GetHistList(
				pat.PatNum,insInfo.ListBenefits,insInfo.ListPatPlans,insInfo.ListInsPlans,proc3.ProcDate,insInfo.ListInsSubs
			);
			//2nd proc has reached frequency limitation.
			Assert.IsTrue(Procedures.HasMetFrequencyLimitation(
				listClaimProcs.FirstOrDefault(x => x.ProcNum==proc3.ProcNum),listHistList,insInfo.ListBenefits,proc3,
				ProcedureCodes.GetProcCode(proc3.CodeNum),insInfo.PriInsPlan,insInfo.ListInsPlans)
			);
		}
		#endregion

		[TestMethod]
		public void Procedures_SetCompleteInAppt_MetFrequencyLimit() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,
				BenefitTimePeriod.CalendarYear));
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,new DateTime(2018,1,10));
			insInfo.ListAllProcs.Add(proc1);
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claim,listClaimProcs);
			Operatory op=OperatoryT.CreateOperatory(suffix);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,new DateTime(2018,2,1),op.OperatoryNum,pat.PriProv);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,new DateTime(2018,2,1),aptNum:apt.AptNum);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			Procedures.SetCompleteInAppt(apt,insInfo.ListInsPlans,insInfo.ListPatPlans,pat,insInfo.ListInsSubs,false);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//2nd proc has reached frequency limitation.
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);
		}

		[TestMethod]
		public void Procedures_NotReceivedCP_MetFrequencyLimit() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D1110
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,
				BenefitTimePeriod.CalendarYear));
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,DateTime.Today.AddDays(-1));
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,DateTime.Today);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			//2nd proc has reached frequency limitation.
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);
		}

		[TestMethod]
		public void Procedures_NotReceivedCP_BenefitTimePeriod_NumberInLast12Months_MetFrequencyLimit() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D1110
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D1110",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,
				BenefitTimePeriod.NumberInLast12Months));
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,DateTime.Today.AddDays(-1));
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",77,DateTime.Today);
			insInfo.ListAllProcs.Add(proc2);
			//This will compute estimates.
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			//2nd proc has reached frequency limitation.
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);
		}

		///<summary>A completed procedure is attached to a completed appointment. Then a different TP procedure is attached to the same appointment and
		///the method to complete the procedures on the appointment is called. The test then verifies that the deductible is calculated correctly for
		///each procedure.</summary>
		[TestMethod]
		public void Procedures_SetCompleteInApptInList_CompletedProcAttachedToClaimDeductible() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			insInfo.ListBenefits.Add(BenefitT.CreateDeductibleGeneral(insInfo.PriInsPlan.PlanNum,BenefitCoverageLevel.Individual,50));
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddHours(9),0,pat.PriProv,aptStatus: ApptStatus.Complete);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,DateTime.Today,aptNum: apt.AptNum);
			insInfo.ListAllProcs.Add(proc1);
			//Creates a cp with status of NotReceived
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(50,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).DedEst);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2740",ProcStat.TP,"",77,DateTime.Today,aptNum: apt.AptNum);
			insInfo.ListAllProcs.Add(proc2);
			List<Procedure> listProcs=new List<Procedure> { proc1, proc2 };
			//Also computes estimates
			Procedures.SetCompleteInApptInList(apt,insInfo.ListInsPlans,insInfo.ListPatPlans,pat,listProcs,insInfo.ListInsSubs,Security.CurUser);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(50,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).DedEst);
			Assert.AreEqual(0,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).DedEst);
		}

		///<summary>Two completed procedures are attached to a completed appointment. Then a different TP procedure is attached to the same appointment 
		///and the method to complete the procedures on the appointment is called. The test then verifies that the deductible is calculated correctly for
		///each procedure.</summary>
		[TestMethod]
		public void Procedures_SetCompleteInApptInList_MultipleCompletedProcAttachedToClaimDeductible() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			insInfo.ListBenefits.Add(BenefitT.CreateDeductibleGeneral(insInfo.PriInsPlan.PlanNum,BenefitCoverageLevel.Individual,50));
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddHours(9),0,pat.PriProv,aptStatus: ApptStatus.Complete);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",88,DateTime.Today,aptNum: apt.AptNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",99,DateTime.Today,aptNum: apt.AptNum);
			insInfo.ListAllProcs.Add(proc1);
			insInfo.ListAllProcs.Add(proc2);
			//Creates a claim with the proc charted 2nd as the 1st proc on the claim.
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc2,proc1 },insInfo);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(50,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).DedEst);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D2740",ProcStat.TP,"",77,DateTime.Today,aptNum: apt.AptNum);
			insInfo.ListAllProcs.Add(proc3);
			List<Procedure> listProcs=new List<Procedure> { proc1, proc2, proc3 };
			//Also computes estimates
			Procedures.SetCompleteInApptInList(apt,insInfo.ListInsPlans,insInfo.ListPatPlans,pat,listProcs,insInfo.ListInsSubs,Security.CurUser);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).DedEst);
			Assert.AreEqual(50,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).DedEst);
			Assert.AreEqual(0,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc3.ProcNum).DedEst);
		}

		///<summary>Two completed procedures are attached to a completed appointment. Then a different TP procedure is attached to the same appointment 
		///and the method to complete the procedures on the appointment is called. The test then verifies that the InsPayEst is calculated correctly for
		///each procedure.</summary>
		[TestMethod]
		public void Procedures_SetCompleteInApptInList_MultipleCompletedProcAttachedToClaim() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			Patient pat = PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			BenefitT.CreateAnnualMax(insInfo.PriInsPlan.PlanNum,500);
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.Crowns,50));
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddHours(9),0,pat.PriProv,aptStatus: ApptStatus.Complete);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today,aptNum: apt.AptNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",120,DateTime.Today,aptNum: apt.AptNum);
			insInfo.ListAllProcs.Add(proc1);
			insInfo.ListAllProcs.Add(proc2);
			//Creates a claim with the proc charted 2nd as the 1st proc on the claim.
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc2,proc1 },insInfo);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).InsPayEst);
			Assert.AreEqual(120,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);
			Procedure proc3 = ProcedureT.CreateProcedure(pat,"D2740",ProcStat.TP,"",400,DateTime.Today,aptNum: apt.AptNum);
			insInfo.ListAllProcs.Add(proc3);
			List<Procedure> listProcs = new List<Procedure> { proc1,proc2,proc3 };
			//Also computes estimates
			Procedures.SetCompleteInApptInList(apt,insInfo.ListInsPlans,insInfo.ListPatPlans,pat,listProcs,insInfo.ListInsSubs,Security.CurUser);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).InsPayEst);
			Assert.AreEqual(120,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);
			Assert.AreEqual(200,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);
		}

		///<summary>Two completed procedures are attached to a completed appointment. Then a different TP procedure is attached to the same appointment 
		///and the method to complete the procedures on the appointment is called.  Meanwhile, insurance payment has been received since it last computed estimates.
		///The test then verifies that the InsPayEst is calculated correctly for each procedure.</summary>
		[TestMethod]
		public void Procedures_SetCompleteInApptInList_MultipleCompletedProcAttachedToClaim_OtherInsuranceReceived() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			BenefitT.CreateAnnualMax(insInfo.PriInsPlan.PlanNum,500);
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			insInfo.ListBenefits.Add(BenefitT.CreateCategoryPercent(insInfo.PriInsPlan.PlanNum,EbenefitCategory.Crowns,50));
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddHours(9),0,pat.PriProv,aptStatus: ApptStatus.Complete);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100,DateTime.Today,aptNum: apt.AptNum);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",120,DateTime.Today,aptNum: apt.AptNum);
			insInfo.ListAllProcs.Add(proc1);
			insInfo.ListAllProcs.Add(proc2);
			//Creates a claim with the proc charted 2nd as the 1st proc on the claim.
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc2,proc1 },insInfo);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).InsPayEst);
			Assert.AreEqual(120,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);
			//Claim was received before a new procedure was added to the appointment. Amount paid was $380
			Procedure procRecieved=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",380,DateTime.Today);
			insInfo.ListAllProcs.Add(procRecieved);
			Claim claimReceived=ClaimT.CreateClaim(new List<Procedure> { procRecieved },insInfo);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claimReceived,insInfo.ListAllClaimProcs.Where(x => x.ClaimNum==claimReceived.ClaimNum).ToList(),doAddPayAmount:true);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Patient only has $120 benefit remaining. Add new procedure to the appointment
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D2740",ProcStat.TP,"",400,DateTime.Today,aptNum: apt.AptNum);
			insInfo.ListAllProcs.Add(proc3);
			List<Procedure> listProcs=new List<Procedure> { proc1, proc2, proc3 };
			//Also computes estimates
			Procedures.SetCompleteInApptInList(apt,insInfo.ListInsPlans,insInfo.ListPatPlans,pat,listProcs,insInfo.ListInsSubs,Security.CurUser);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Total InsPayEst for the 3 procs on the appointment should be the remaining $120.
			Assert.AreEqual(120,insInfo.ListAllClaimProcs.Where(x=>ListTools.In(x.ProcNum,listProcs.Select(y=>y.ProcNum).ToList())).Sum(x => x.InsPayEst));
		}

		///<summary>Charts a completed procedure for prov2 (with insurance coverage and frequency limited to 1/year) and creates a claim.  Creates an
		///appointment, with a different provider, with this procedure attached and recompletes it.  Asserts estimates do not change and are not 
		///frequency limited.</summary>
		[TestMethod]
		public void Procedures_SetCompleteInApptInList_FrequencyLimitationOnProvChange() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:1);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			insInfo.ListBenefits.Add(BenefitT.CreatePercentForProc(insInfo.PriInsPlan.PlanNum,ProcedureCodeT.GetCodeNum("D4341"),50));
			insInfo.ListBenefits.Add(BenefitT.CreateFrequencyProc(insInfo.PriInsPlan.PlanNum,"D4341",BenefitQuantity.Months,12,BenefitTimePeriod.None));
			//Complete a procedure that is frequency limited to once per year, using a different provider than pat's priprov.
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.C,"",100,DateTime.Today,provNum:2);
			//Create a claim for that procedure.
			Claim claim=ClaimT.CreateClaim(new List<Procedure> { proc1 },insInfo);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(50,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).InsPayEst);//Correct ins est.
			Assert.AreEqual("",insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).EstimateNote);//No frequency limitation reached.
			//Create a scheduled appointment using pat's priprov.
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddHours(9),0,pat.PriProv,aptStatus: ApptStatus.Scheduled);
			//Attach already completed and claim attached procedure to this appointment.
			proc1.AptNum=apt.AptNum;
			Procedures.SetCompleteInApptInList(apt,insInfo.ListInsPlans,insInfo.ListPatPlans,pat,new List<Procedure>() {proc1},insInfo.ListInsSubs
				,Security.CurUser);
			insInfo.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(50,insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).InsPayEst);//Correct ins est.
			Assert.AreEqual("",insInfo.ListAllClaimProcs.First(x => x.ProcNum==proc1.ProcNum).EstimateNote);//No frequency limitation reached.
		}

		#region Proc_ProviderMismatch
		///<summary>Appt is completed, testing to see if any providers are changed when using procs
		///with a diffrent status but same provider as appt.</summary>
		[TestMethod]
		public void Procedures_UpdateProcsInApptHelper_ProviderMismatch_DiffProcStatusWithNoMismatch() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long prov1=ProviderT.CreateProvider(suffix);
			long prov2=ProviderT.CreateProvider(suffix);
			long provNumExpected=prov1;
			Procedure proc1after;
			Procedure proc2after;
			//Procs are TP and Completed but with same provider as apt, no change
			Procedures_UpdateProcsInApptHelper_Helper(prov1,prov1,ProcStat.TP,ProcStat.C,out proc1after,out proc2after);
			Assert.AreEqual(provNumExpected,proc1after.ProvNum);
			Assert.AreEqual(provNumExpected,proc2after.ProvNum);
		}

		///<summary>Appt is completed, testing if provider is changed on attached procs when both
		///proc statuses are TP and they have the same provider as on the appt</summary>
		[TestMethod]
		public void Procedures_UpdateProcsInApptHelper_ProviderMismatch_TPProcStatusWithNoMismatch() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long prov1=ProviderT.CreateProvider(suffix);
			long prov2=ProviderT.CreateProvider(suffix);
			long provNumExpected=prov1;
			Procedure proc1after;
			Procedure proc2after;
			//Procs are TP and Completed but with same provider as apt, no change
			Procedures_UpdateProcsInApptHelper_Helper(prov1,prov1,ProcStat.TP,ProcStat.TP,out proc1after,out proc2after);
			Assert.AreEqual(provNumExpected,proc1after.ProvNum);
			Assert.AreEqual(provNumExpected,proc2after.ProvNum);
		}

		///<summary>Appt is completed, testing if provider is changed on attached procs when both
		///proc statuses are Complete and they have the same provider as on the appt.</summary>
		[TestMethod]
		public void Procedures_UpdateProcsInApptHelper_ProviderMismatch_CProcStatusWithNoMismatch() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long prov1=ProviderT.CreateProvider(suffix);
			long prov2=ProviderT.CreateProvider(suffix);
			long provNumExpected=prov1;
			Procedure proc1after;
			Procedure proc2after;
			//All procs match apt provider and have same status, no change
			Procedures_UpdateProcsInApptHelper_Helper(prov1,prov1,ProcStat.C,ProcStat.C,out proc1after,out proc2after);
			Assert.AreEqual(provNumExpected,proc1after.ProvNum);
			Assert.AreEqual(provNumExpected,proc2after.ProvNum);
		}

		///<summary>Appt is completed, testing if provider is changed on attached procs when TP proc
		///does not match provider but completed proc does. This test covers if the user agrees to change the
		///proc's provider or refuses.</summary>
		[TestMethod]
		public void Procedures_UpdateProcsInApptHelper_ProviderMismatch_DiffProcStatusWithCMismatch_UpdatingProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long prov1=ProviderT.CreateProvider(suffix);
			long prov2=ProviderT.CreateProvider(suffix);
			long provNumExpected1=prov1;
			long provNumExpected2=prov2;
			Procedure proc1after;
			Procedure proc2after;
			//If Yes, do change provider on completed procs
			Procedures_UpdateProcsInApptHelper_Helper(prov1,prov2,ProcStat.TP,ProcStat.C,out proc1after,out proc2after,YN.Yes);
			Assert.AreEqual(provNumExpected1,proc1after.ProvNum);
			Assert.AreEqual(provNumExpected1,proc2after.ProvNum);
			//If No, do not change provider on completed procs
			Procedures_UpdateProcsInApptHelper_Helper(prov1,prov2,ProcStat.TP,ProcStat.C,out proc1after,out proc2after,YN.No);
			Assert.AreEqual(provNumExpected1,proc1after.ProvNum);
			Assert.AreEqual(provNumExpected2,proc2after.ProvNum);
		}

		///<summary>Appt is completed, testing if provider is changed on attached procs when TP proc
		///does not match provider but completed proc does. This test covers if the user agrees to change the
		///proc's provider or refuses.</summary>
		[TestMethod]
		public void Procedures_UpdateProcsInApptHelper_ProviderMismatch_DiffProcStatusWithTPMismatch_UpdatingProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long prov1=ProviderT.CreateProvider(suffix);
			long prov2=ProviderT.CreateProvider(suffix);
			long provNumExpected=prov1;
			Procedure proc1after;
			Procedure proc2after;
			Procedures_UpdateProcsInApptHelper_Helper(prov1,prov2,ProcStat.C,ProcStat.TP,out proc1after,out proc2after);
			Assert.AreEqual(provNumExpected,proc1after.ProvNum);
			Assert.AreEqual(provNumExpected,proc2after.ProvNum);
		}

		///<summary>Appt is completed, testing if provider is changed on attached procs when all procs are 
		///mismatched from the apt provider and are completed procs. This test covers if the user agrees to change the
		///proc's provider or refuses.</summary>
		[TestMethod]
		public void Procedures_UpdateProcsInApptHelper_ProviderMismatch_SameProcStatusWithAllCMismatch_UpdatingProc() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long prov1=ProviderT.CreateProvider(suffix);
			long prov2=ProviderT.CreateProvider(suffix);
			long provNumExpected=prov2;
			Procedure proc1after;
			Procedure proc2after;
			//No change to provider on completed procs when there are no TP procs
			Procedures_UpdateProcsInApptHelper_Helper(prov2,prov2,ProcStat.C,ProcStat.C,out proc1after,out proc2after,YN.No);
			Assert.AreEqual(provNumExpected,proc1after.ProvNum);
			Assert.AreEqual(provNumExpected,proc2after.ProvNum);
		}

		/// <summary>A helper method for the Procedures_UpdateProcsInApptHelper methods since a lot of their necessary
		/// data is duplicated through the methods.</summary>
		private void Procedures_UpdateProcsInApptHelper_Helper(long prov1,long prov2,ProcStat procstatus1,ProcStat procstatus2,out Procedure proc1after
				,out Procedure proc2after,YN doChangeProvOnProcsOverride=YN.Unknown)
		{
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,prov1);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			List<Procedure> listProcs=new List<Procedure>();
			ProcedureCode code=ProcedureCodeT.CreateProcCode(MiscUtils.CreateRandomAlphaNumericString(15));
			Assert.AreEqual(0,code.ProvNumDefault);
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddHours(9),0,prov1,aptStatus:ApptStatus.Complete);
			Procedure proc1=ProcedureT.CreateProcedure(pat,code.ProcCode,procstatus1,"",100,DateTime.Today,aptNum:apt.AptNum,provNum:prov1);
			Procedure proc2=ProcedureT.CreateProcedure(pat,code.ProcCode,procstatus2,"",120,DateTime.Today,aptNum:apt.AptNum,provNum:prov2);
			listProcs.Add(proc1);
			listProcs.Add(proc2);
			bool removeCompletedProcs;
			List<int> listProcSelectedIndices=new List<int>() { 0,1 };//Spoof selecting both procs.
			if(doChangeProvOnProcsOverride!=YN.Unknown) {//Override set
				removeCompletedProcs=(doChangeProvOnProcsOverride==YN.Yes?false:true);
			}
			else { 
				removeCompletedProcs=OpenDental.ProcedureL.DoRemoveCompletedProcs(apt,listProcs,true);
			}
			Procedures.UpdateProcsInApptHelper(listProcs,pat,apt,apt.Copy(),ins.ListInsPlans,ins.ListInsSubs,
				listProcSelectedIndices,removeCompletedProcs,false);//doUpdateProcFee set to false because test do not consider ProcFee.
			proc1after=Procedures.GetOneProc(proc1.ProcNum,false);
			proc2after=Procedures.GetOneProc(proc2.ProcNum,false);
		}
		#endregion

		///<summary>Primary claim is received, and since Pref.InsEstRecalcReceived is false, its ClaimProc estimates should not be recalculated.  
		///However, the secondary estimates should still recalcuate since they are not received.  Secondary claimprocs must factor in the paid estimates
		///from the primary claim.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_InsEstRecalcReceivedPriNotReceivedSec_False() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=100;
			const double ucrFee=50;
			string procStr="D0120";
			string planType="p";//PPO percentage
			double feeSchedFee=40;
			//Expected that estimates will not be recalculated for Received ClaimProc.  However, secondary should calculate, factoring in estimates from primary.
			double estimatedSecBaseEst=0;
			double estimatedSecInsEstTotal=0;
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
			#region Primary InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum,ordinal:1);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listInsPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listInsPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			#region Secondary InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum,ordinal:2);
			listSubs=InsSubT.GetInsSubs(pat);
			listInsPlans=InsPlans.RefreshForSubList(listSubs);
			listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan secPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Secondary,listPatPlans,listInsPlans,listSubs);
			InsSub secSub=InsSubT.GetSubForPriSecMed(PriSecMed.Secondary,listPatPlans,listInsPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(secPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listInsPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Estimate);
			ClaimProc secClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,secPlan.PlanNum,secSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Estimate);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProcs=new List<ClaimProc>() { priClaimProc, secClaimProc };
			PrefT.UpdateBool(PrefName.InsEstRecalcReceived,false);//Set InsEstRecalcReceived preference to false.
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,true,listInsPlans,listPatPlans,listBens,pat.Age,listSubs);
			priClaimProc.Status=ClaimProcStatus.Received;//Set the primary claimproc received.
			priClaimProc.InsPayAmt=feeSchedFee*(coveragePercent/100d);
			priClaimProc.WriteOff=(ucrFee-feeSchedFee);//(ucr-schedfee)
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,false,listInsPlans,listPatPlans,listBens,pat.Age,listSubs);
			Assert.AreEqual(estimatedSecBaseEst,secClaimProc.BaseEst);
			Assert.AreEqual(estimatedSecInsEstTotal,secClaimProc.InsEstTotal);
		}

		///<summary>Primary claim is received, and since Pref.InsEstRecalcReceived is false, its ClaimProc estimates should not be recalculated.  
		///However, the secondary estimates should still recalcuate since they are not received.  Secondary claimprocs must factor in the paid estimates
		///from the primary claim.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_InsEstRecalcReceivedPriNotReceivedSecTwentyFivePercentCoverage_False() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=25;
			const double ucrFee=50;
			string procStr="D0120";
			string planType="p";//PPO percentage
			double feeSchedFee=40;
			//Expected that estimates will not be recalculated for Received ClaimProc.  However, secondary should calculate, factoring in estimates from primary.
			double estimatedSecBaseEst=10;
			double estimatedSecInsEstTotal=10;
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
			#region Primary InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum,ordinal:1);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listInsPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listInsPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			#region Secondary InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum,ordinal:2);
			listSubs=InsSubT.GetInsSubs(pat);
			listInsPlans=InsPlans.RefreshForSubList(listSubs);
			listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan secPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Secondary,listPatPlans,listInsPlans,listSubs);
			InsSub secSub=InsSubT.GetSubForPriSecMed(PriSecMed.Secondary,listPatPlans,listInsPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(secPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listInsPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Estimate);
			ClaimProc secClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,secPlan.PlanNum,secSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Estimate);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProcs=new List<ClaimProc>() { priClaimProc, secClaimProc };
			PrefT.UpdateBool(PrefName.InsEstRecalcReceived,false);//Set InsEstRecalcReceived preference to false.
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,true,listInsPlans,listPatPlans,listBens,pat.Age,listSubs);
			priClaimProc.Status=ClaimProcStatus.Received;//Set the primary claimproc received.
			priClaimProc.InsPayAmt=feeSchedFee*(coveragePercent/100d);
			priClaimProc.WriteOff=(ucrFee-feeSchedFee);//(ucr-schedfee)
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,false,listInsPlans,listPatPlans,listBens,pat.Age,listSubs);
			Assert.AreEqual(estimatedSecBaseEst,secClaimProc.BaseEst);
			Assert.AreEqual(estimatedSecInsEstTotal,secClaimProc.InsEstTotal);
		}

		///<summary>Primary claim is received, and since Pref.InsEstRecalcReceived is true, its ClaimProc estimates should be recalculated.  
		///Additionally, the secondary estimates should still recalcuate since they are not received.  Secondary claimprocs must factor in the paid 
		///estimates from the primary claim.</summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_InsEstRecalcReceivedPriNotReceivedSec_True() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			const int coveragePercent=100;
			const double ucrFee=50;
			string procStr="D0120";
			string planType="p";//PPO percentage
			double feeSchedFee=40;
			//Expected that estimates will not be recalculated for Received ClaimProc.  However, secondary should calculate, factoring in estimates from primary.
			double estimatedSecBaseEst=0;
			double estimatedSecInsEstTotal=0;
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
			#region Primary InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum,ordinal:1);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan priPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listInsPlans,listSubs);
			InsSub priSub=InsSubT.GetSubForPriSecMed(PriSecMed.Primary,listPatPlans,listInsPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			#region Secondary InsPlan Setup
			InsuranceT.AddInsurance(pat,suffix,planType,feeSchedNum,ordinal:2);
			listSubs=InsSubT.GetInsSubs(pat);
			listInsPlans=InsPlans.RefreshForSubList(listSubs);
			listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan secPlan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Secondary,listPatPlans,listInsPlans,listSubs);
			InsSub secSub=InsSubT.GetSubForPriSecMed(PriSecMed.Secondary,listPatPlans,listInsPlans,listSubs);
			if(planType!="f" && planType!="c") {//Only add a 50 percent coverage to the plan for the example proc if not Medicaid/Flat Copay or capitation
				BenefitT.CreatePercentForProc(secPlan.PlanNum,procCode.CodeNum,coveragePercent);
			}
			#endregion
			PatPlan priPatPlan=listPatPlans.FirstOrDefault(x => x.InsSubNum==priSub.InsSubNum);
			double procFee=Procedures.GetProcFee(pat,listPatPlans,listSubs,listInsPlans,procCode.CodeNum,pat.PriProv,0,"");
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.C,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Estimate);
			ClaimProc secClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,secPlan.PlanNum,secSub.InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.Estimate);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			List<SubstitutionLink> listSubLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProcs=new List<ClaimProc>() { priClaimProc, secClaimProc };
			PrefT.UpdateBool(PrefName.InsEstRecalcReceived,true);//Set InsEstRecalcReceived preference to true.
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,true,listInsPlans,listPatPlans,listBens,pat.Age,listSubs);

			priClaimProc.Status=ClaimProcStatus.Received;//Set the primary claimproc received.
			priClaimProc.InsPayAmt=feeSchedFee*(coveragePercent/100d);
			priClaimProc.WriteOff=(ucrFee-feeSchedFee);//(ucr-schedfee)
			Procedures.ComputeEstimates(proc,pat.PatNum,listClaimProcs,false,listInsPlans,listPatPlans,listBens,pat.Age,listSubs);
			Assert.AreEqual(estimatedSecBaseEst,secClaimProc.BaseEst);
			Assert.AreEqual(estimatedSecInsEstTotal,secClaimProc.InsEstTotal);
		}

		///<summary>Patient has two insurance plans, both PPO, subscriber self for both. The only benefit entered for both insurance plans is 50% coverage on the crowns category. One procedure is treatment planned, a D2750 PFM crown on #8. COB is Basic; see Tests 17 and 18 for other COB types. If the three fees are entered for each scenario, then the estimates and writeoffs shown in the Treatment Plan module will be:
		///<para>Fee &#x09; &#x09; &#x09; &#x09; = &#x09; &#x09; $1200</para> 
		///<para>Allowed1 &#x09; &#x09; = &#x09; &#x09; $900</para>  
		///<para>Allowed2 &#x09; &#x09; = &#x09; &#x09; $650</para>  
		///</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_Allowed1Allowed2)]
		[Documentation.VersionAdded("7.1")]
		[Documentation.Description(@"Patient has two insurance plans, both PPO, subscriber self for both. The only benefit entered for both insurance plans is 50% coverage on the crowns category. One procedure is treatment planned, a D2750 PFM crown on #8. COB is Basic; see Tests 17 and 18 for other COB types. If the three fees are entered for each scenario, then the estimates and writeoffs shown in the Treatment Plan module will be:
        <table>
          <tr>
            <td>Fee</td>
            <td>Allowed1</td>
            <td>Allowed2</td>
            <td>InsPay1</td>
            <td>InsPay2</td>
            <td>Writeoff1</td>
            <td>Writeoff2</td>
            <td>(implied pat port)</td>
          </tr>
          <tr>
            <td>$1,200.00</td>
            <td>$900.00</td>
            <td>$650.00</td>
            <td>$450.00</td>
            <td>$200.00</td>
            <td>$300.00</td>
            <td>$0.00</td>
            <td>$250.00</td>
          </tr>
        </table>
")]
		public void Procedures_ComputeEstimates_Allowed1Allowed2() {
			string suffix="1";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedNum1=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			long feeSchedNum2=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix+"b");
			//Standard Fee
			long codeNum=ProcedureCodeT.GetCodeNum("D2750");
			Fee fee=Fees.GetFee(codeNum,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum;
				fee.FeeSched=53;
				fee.Amount=1200;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=1200;
				Fees.Update(fee);
			}
			//PPO fees
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum1;
			fee.Amount=900;
			Fees.Insert(fee);
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum2;
			fee.Amount=650;
			Fees.Insert(fee);
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum1).PlanNum;
			long planNum2=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum2).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.Crowns,50);
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.Crowns,50);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"8",Fees.GetAmount0(codeNum,53));//crown on 8
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			//if(specificTest==0 || specificTest==1){
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(450,claimProc.InsEstTotal);
			Assert.AreEqual(300,claimProc.WriteOffEst);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(200,claimProc.InsEstTotal);
			Assert.AreEqual(0,claimProc.WriteOffEst);
			//Test 2----------------------------------------------------------------------------------------------------
			//if(specificTest==0 || specificTest==2){
			//s	witch the fees
			fee=Fees.GetFee(codeNum,feeSchedNum1,0,0);
			fee.Amount=650;
			Fees.Update(fee);
			fee=Fees.GetFee(codeNum,feeSchedNum2,0,0);
			fee.Amount=900;
			Fees.Update(fee);
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			//Validate
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(325,claimProc.InsEstTotal);
			Assert.AreEqual(425,claimProc.WriteOffEst);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(450,claimProc.InsEstTotal);
			Assert.AreEqual(0,claimProc.WriteOffEst);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_ZeroCoverageOverAnnualMax)]
		[Documentation.VersionAdded("7.1")]
		[Documentation.Description(@"Patient has one insurance plan, category percentage, subscriber self. Benefits include annual max of 1000, crowns 100%, Diagnostic 100%, BW frequency: every 1 year. Two procedures are treatment planned: a crown for $1100, going over the annual max, and a 4BW D0274. The 4BW must show zero coverage.")]
		public void Procedures_ComputeEstimates_ZeroCoverageOverAnnualMax() {
			string suffix="3";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);//guarantor is subscriber
			BenefitT.CreateAnnualMax(plan.PlanNum,1000);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//proc1 - Crown
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"8",1100);
			ProcedureT.SetPriority(proc1,0);
			//proc2 - 4BW
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.TP,"8",50);
			ProcedureT.SetPriority(proc2,1);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(0,claimProc.InsEstTotal);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_FamilyUnderAnnualMax)]
		[Documentation.VersionAdded("7.1")]
		[Documentation.Description(@"2 patients in one family. Same insurance. The only benefits are: $1000 individual annual max, $2500 family annual max, and 100% crowns. Add a crown for $830. Crown should show insurance estimate of $830 with no comment about 'over annual max'.")]
		public void Procedures_ComputeEstimates_FamilyUnderAnnualMax() {
			string suffix="4";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Patient pat2=PatientT.CreatePatient(suffix);
			PatientT.SetGuarantor(pat2,pat.PatNum);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			long planNum=plan.PlanNum;
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,planNum);//guarantor is subscriber
			long subNum=sub.InsSubNum;
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			PatPlanT.CreatePatPlan(1,pat2.PatNum,subNum);//both patients have the same plan
			BenefitT.CreateAnnualMax(planNum,1000);
			BenefitT.CreateAnnualMaxFamily(planNum,2500);
			BenefitT.CreateCategoryPercent(planNum,EbenefitCategory.Crowns,100);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"8",830);
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum,subNum);
			Assert.AreEqual(830,claimProc.InsEstTotal);
			Assert.AreEqual("",claimProc.EstimateNote);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_IndividualAndFamilyMax)]
		[Documentation.VersionAdded("7.1")]
		[Documentation.Description("2 patients in one family. Same insurance. The only benefits are: $1000 individual annual max, $2500 family annual max, and 100% crowns. Add an insurance used amount of $2000 to one patient. Then have one TP procedure on the other patient, a crown for $830. The insurance on the second patient should show an estimate of $500 due to family max.")]
		public void Procedures_ComputeEstimates_IndividualAndFamilyMax() {
			string suffix="5";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			Patient pat2=PatientT.CreatePatient(suffix);
			PatientT.SetGuarantor(pat2,pat.PatNum);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			long planNum=plan.PlanNum;
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,planNum);//guarantor is subscriber
			long subNum=sub.InsSubNum;
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			PatPlanT.CreatePatPlan(1,pat2.PatNum,subNum);//both patients have the same plan
			BenefitT.CreateAnnualMax(planNum,1000);
			BenefitT.CreateAnnualMaxFamily(planNum,2500);
			BenefitT.CreateCategoryPercent(planNum,EbenefitCategory.Crowns,100);
			ClaimProcT.AddInsUsedAdjustment(pat2.PatNum,planNum,2000,subNum,0);//Adjustment goes on the second patient
			Procedure proc=ProcedureT.CreateProcedure(pat2,"D2750",ProcStat.TP,"8",830);//crown and testing is for the first patient
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
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum,subNum);
			Assert.AreEqual(500,claimProc.InsEstTotal);
			Assert.AreEqual("Over family annual max",claimProc.EstimateNote);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_PreventitiveDiagnosticDeductibleOnlyOnce)]
		[Documentation.VersionAdded("7.2")]
		[Documentation.Description(@"1 patient. 1 insplan, 6 benefits: 1000 annual max, 100% preventive, 100% diagnostic, $50 deduct general, $25 deduct preventive, and $25 deduct diagnostic. One TP procedure for $60, code D0120 (perExam). Second TP procedure for $70, code D1110 (prophy). Second procedure should show no deductible. This is because we need to treat the $25 deductible as a single deductible that applies to preventive/diagnostic. It's not two deductibles. This is only a temporary solution, and the better solution will be to support code ranges that include both preventive and diagnostic, and then to enter a single $25 deductible for that range.")]
		[TestMethod]
		public void Procedures_ComputeEstimates_PreventitiveDiagnosticDeductibleOnlyOnce() {
			string suffix="7";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,1000);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.RoutinePreventive,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.RoutinePreventive,25);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.Diagnostic,25);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - PerExam
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.TP,"",60);
			ProcedureT.SetPriority(proc1,0);
			//proc2 - Prophy
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.TP,"",70);
			ProcedureT.SetPriority(proc2,1);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			Assert.AreEqual(0,claimProc.DedEst);
		}

		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_LimitationsOverrideGeneralLimitations)]
		[Documentation.VersionAdded("7.2.39")]
		[Documentation.Description(" Related to #6. Patient has one insurance plan, subscriber self. Benefits: annual max $200. Other benefit added for limitation on D2161 of $2000. Restorative 80%. Two procedures are treatment planned. First is D2161 for $300, insurance estimate of $240. Second procedure is a D2160 for $300. Insurance estimate on second procedure in the TP should be $200 because the first procedure does not count towards the regular annual max. It instead has its own annual max.")]
		///<summary></summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_LimitationsOverrideGeneralLimitations() {
			string suffix="9";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,200);
			BenefitT.CreateLimitationProc(plan.PlanNum,"D2161",2000);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Restorative,80);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - D2161 (4-surf amalgam)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2161",ProcStat.TP,"3",300);
			ProcedureT.SetPriority(proc1,0);
			//proc2 - D2160 (3-surf amalgam)
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2160",ProcStat.TP,"4",300);
			ProcedureT.SetPriority(proc2,1);
			//Lists
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
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			Assert.AreEqual(200,claimProc.InsEstTotal);
		}

		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_AnnualMaxReachedZeroCoverage)]
		[Documentation.VersionAdded("7.2.43")]
		[Documentation.Description("(this tests the case where preventive frequency is causing errors in the calculation) Patient has one insurance plan, subscriber self. Benefits: annual max $400. 100% coverage on routine preventive. Limitation, preventive every 2 years. Three space maintainers (D1515) added to TP, $500 each. First proc should show $400 coverage, and second proc $0 coverage.")]
		///<summary></summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_AnnualMaxReachedZeroCoverage() {
			string suffix="10";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,400);
			BenefitT.CreateFrequencyCategory(plan.PlanNum,EbenefitCategory.RoutinePreventive,BenefitQuantity.Years,2);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.RoutinePreventive,100);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//procs - D1515 (space maintainers)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D1515",ProcStat.TP,"3",500);
			ProcedureT.SetPriority(proc1,0);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1515",ProcStat.TP,"3",500);
			ProcedureT.SetPriority(proc2,1);
			//Procedure proc3=ProcedureT.CreateProcedure(pat,"D1515",ProcStat.TP,"3",500);
			//ProcedureT.SetPriority(proc3,2);
			//Lists
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
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,subNum);
			Assert.AreEqual(400,claimProc1.InsEstTotal);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			Assert.AreEqual(0,claimProc2.InsEstTotal);
		}

		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_FamilyMaxNoIndividualMax)]
		[Documentation.VersionAdded("7.5.5")]
		[Documentation.Description("Patient has one insurance plan, subscriber self. Benefits: annual family max $400. No individual max. 100% coverage on restorative. Two amalgams added to TP, $500 each. First proc should show $400 coverage, and second proc $0 coverage.")]
		///<summary></summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_FamilyMaxNoIndividualMax() {
			string suffix="11";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMaxFamily(plan.PlanNum,400);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Restorative,100);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//procs - D2140 (amalgum fillings)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2140",ProcStat.TP,"18",500);
			//Procedure proc1=ProcedureT.CreateProcedure(pat,"D1515",ProcStat.TP,"3",500);
			ProcedureT.SetPriority(proc1,0);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2140",ProcStat.TP,"19",500);
			//Procedure proc2=ProcedureT.CreateProcedure(pat,"D1515",ProcStat.TP,"3",500);
			ProcedureT.SetPriority(proc2,1);
			//Procedure proc3=ProcedureT.CreateProcedure(pat,"D1515",ProcStat.TP,"3",500);
			//ProcedureT.SetPriority(proc3,2);
			//Lists
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
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,patPlans[0].InsSubNum);
			Assert.AreEqual(400,claimProc1.InsEstTotal);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,patPlans[0].InsSubNum);
			Assert.AreEqual(0,claimProc2.InsEstTotal);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_WriteoffPPOsPriSecSamePlan)]
		[Documentation.VersionAdded("7.8.16")]
		[Documentation.Description(@"Patient has spouse.  Both have insurance with the same PPO plan.  The patient has secondary insurance  through the spouse.  Benefits: annual max $1200. Deductible $0.  100% coverage on a crown.  One crown added to TP for $1400.  Allowed $1100.
				<table>
          <tr>
            <td>Fee</td>
            <td>Allowed1</td>
            <td>Allowed2</td>
            <td>InsPay1</td>
            <td>InsPay2</td>
            <td>Writeoff1</td>
            <td>Writeoff2</td>
            <td>(implied pat port)</td>
          </tr>
          <tr>
            <td>$1,400.00</td>
            <td>$1100.00</td>
            <td>$1100.00</td>
            <td>$1100.00</td>
            <td>$0.00</td>
            <td>$300.00</td>
            <td>$0.00</td>
            <td>$0.00</td>
          </tr>
        </table>
			")]
		[TestMethod]
		public void Procedures_ComputeEstimates_WriteoffPPOsPriSecSamePlan() {
			string suffix="12";
			Patient pat=PatientT.CreatePatient(suffix);
			Patient pat2=PatientT.CreatePatient(suffix);
			PatientT.SetGuarantor(pat2,pat.PatNum);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			//Standard Fee
			long codeNum=ProcedureCodeT.GetCodeNum("D2750");
			Fee fee=Fees.GetFee(codeNum,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum;
				fee.FeeSched=53;
				fee.Amount=1400;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=1400;
				Fees.Update(fee);
			}
			//PPO fees
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum;
			fee.Amount=1100;
			Fees.Insert(fee);
			InsPlan plan=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum);
			long planNum=plan.PlanNum;
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,planNum);//patient is subscriber for plan 1
			long subNum=sub.InsSubNum;
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			InsSub sub2=InsSubT.CreateInsSub(pat2.PatNum,planNum);//spouse is subscriber for plan 2
			long subNum2=sub2.InsSubNum;
			PatPlanT.CreatePatPlan(2,pat.PatNum,subNum2);//patient also has spouse's coverage
			BenefitT.CreateAnnualMax(planNum,1200);
			BenefitT.CreateDeductibleGeneral(planNum,BenefitCoverageLevel.Individual,0);
			BenefitT.CreateCategoryPercent(planNum,EbenefitCategory.Crowns,100);//2700-2799
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"19",1400);
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();//empty, not used for calcs.
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();//empty, not used for calcs.
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			Procedures.ComputeEstimates(ProcListTP[0],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
				histList,loopList,false,pat.Age,subList);
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc.ProcNum,plan.PlanNum,subNum);
			Assert.AreEqual(1100,claimProc1.InsEstTotal);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc.ProcNum,plan.PlanNum,subNum2);
			Assert.AreEqual(0,claimProc2.InsEstTotal);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_PriEstNotAffectedBySecClaim)]
		[Documentation.VersionAdded("11.0.27")]
		[Documentation.Description("Patient has 2 PPO insurance plans, subscriber self for both. Each plan has benefits: annual max $1200, 80% coverage on restorative. 1 proc: D2160 amalg on #19, $1279 status of TP. Allowed1=$1279, allowed2=$110. Proc set complete, attached to primary claim and secondary claim. Primary estimate should be $1023.20.")]
		[TestMethod]
		public void Procedures_ComputeEstimates_PriEstNotAffectedBySecClaim() {
			string suffix="14";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedNum1=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			long feeSchedNum2=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix+"b");
			//Standard Fee
			long codeNum=ProcedureCodeT.GetCodeNum("D2160");
			Fee fee=Fees.GetFee(codeNum,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum;
				fee.FeeSched=53;
				fee.Amount=1279;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=1279;
				Fees.Update(fee);
			}
			//PPO fees
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum1;
			fee.Amount=1279;
			Fees.Insert(fee);
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum2;
			fee.Amount=110;
			Fees.Insert(fee);
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum1).PlanNum;
			long planNum2=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum2).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.Restorative,80);
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.Restorative,80);
			BenefitT.CreateAnnualMax(planNum1,1200);
			BenefitT.CreateAnnualMax(planNum2,1200);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2160",ProcStat.TP,"19",Fees.GetAmount0(codeNum,53));//amalgam on 19
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();//empty, not used for calcs.
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();//empty, not used for calcs.
			List<Procedure> procList=Procedures.Refresh(patNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(procList);//sorted by priority, then toothnum
			//Set complete and attach to claim
			ProcedureT.SetComplete(proc,pat,planList,patPlans,claimProcs,benefitList,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			List<Procedure> procsForClaim=new List<Procedure>();
			procsForClaim.Add(proc);
			Claim claim1=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,procList,pat,procsForClaim,benefitList,subList);
			Claim claim2=ClaimT.CreateClaim("S",patPlans,planList,claimProcs,procList,pat,procsForClaim,benefitList,subList);
			//Validate
			Procedures.ComputeEstimates(ProcListTP[0],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
				histList,loopList,false,pat.Age,subList);
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc.ProcNum,planNum1,subNum1);
			Assert.AreEqual(1023.20,claimProc1.InsEstTotal);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_DeductibleOverrides)]
		[Documentation.VersionAdded("7.0")]
		[Documentation.Description(@"Not version specific; it has always worked this way. The reason for adding this unit test was to ensure that it keeps working.
Patient has one insurance plan, subscriber self. Benefits: annual max $1000, deductible $50, 100% coverage on preventive/diagnostic/xray, $0 deductible on preventive/diagnostic/xray, 80% coverage on all 4 general categories, 1 manually entered benefit for $45 deductible on code D0330 pano. First TP proc is D0330 for $95, second TP proc is D2150 on #30 for $200. In TP module, $45 deductible shows on the D0330, and a $5 deductible shows on the D2150.")]
		[TestMethod]
		public void Procedures_ComputeEstimates_DeductibleOverrides() {
			string suffix="15";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			BenefitT.CreateAnnualMax(plan.PlanNum,1000);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.RoutinePreventive,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.RoutinePreventive,0);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.Diagnostic,0);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Restorative,80);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Endodontics,80);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Periodontics,80);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.OralSurgery,80);
			BenefitT.CreateDeductible(plan.PlanNum,"D0330",45);
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//proc1 - Pano
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0330",ProcStat.TP,"",95);
			ProcedureT.SetPriority(proc1,0);
			//proc2 - Amalg
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2150",ProcStat.TP,"30",200);
			ProcedureT.SetPriority(proc2,1);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,sub.InsSubNum);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(45,claimProc1.DedEst);
			Assert.AreEqual(5,claimProc2.DedEst);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_CategoryDeductiblesShouldNotExceedRegularDeductible)]
		[Documentation.VersionAdded("11.1.3")]
		[Documentation.Description("Patient has one insurance plan, subscriber self. Benefits: annual max $1000, deductible $50, 100% coverage on preventive/diagnostic/xray, $0 deductible on preventive/diagnostic/xray, 80% coverage on all 4 general categories, 1 manually entered benefit for $45 deductible on code D0330, $25 deductible on D0220, add treatment plannned procedure D0330 fee $100, D0220 fee $75. In TP module, $45 deductible should show on D0330, and $5 on D0220.")]
		public void Procedures_ComputeEstimates_CategoryDeductiblesShouldNotExceedRegularDeductible() {
			string suffix="16";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);//guarantor is subscriber
			//BenefitT.CreateAnnualMax(plan.PlanNum,1000);//Irrelevant benefits bog down debugging.
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			//BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.RoutinePreventive,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			//BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.RoutinePreventive,0);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.Diagnostic,0);
			//BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Restorative,80);
			//BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Endodontics,80);
			//BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Periodontics,80);
			//BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.OralSurgery,80);
			BenefitT.CreateDeductible(plan.PlanNum,"D0330",45);
			BenefitT.CreateDeductible(plan.PlanNum,"D0220",25);
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//proc1 - Pano
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0330",ProcStat.TP,"",100);
			ProcedureT.SetPriority(proc1,0);
			//proc2 - Intraoral - periapical first film
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",75);
			ProcedureT.SetPriority(proc2,1);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,sub.InsSubNum);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(45,claimProc1.DedEst);
			Assert.AreEqual(5,claimProc2.DedEst);
		}

		///<summary>The patient has one insurance plan with general deductible of $50, 100% coverage on Diagnostic, and a $10 xray deductible. 
		///We treatment plan the patient for a cleaning (D0110) and an xray (D0220). The cleaning is estimated to take up $40 of the patient's deductible
		///and the xray is estimated to take up $10 of the patient's deductible. We mark the cleaning complete and leave the xray treatment planned.
		///A claim for the cleaning is submitted, and the deductible comes out to $50. This means that, even though the patient has a $10 xray deductible,
		///they have already hit their max deductible for the year and should owe nothing. The xray deductible estimate is $0.
		///</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_CategoryDeductiblesShouldNotExceedRegularDeductibleClaim)]
		[Documentation.VersionAdded("21.1")]
		[Documentation.Description("The patient has one insurance plan with general deductible of $50, 100% coverage on Diagnostic, and a $10 xray deductible. We treatment plan the patient for a cleaning (D0110) and an xray (D0220). The cleaning is estimated to take up $40 of the patient's deductible and the xray is estimated to take up $10 of the patient's deductible. We mark the cleaning complete and leave the xray treatment planned. A claim for the cleaning is submitted, and the deductible comes out to $50. This means that, even though the patient has a $10 xray deductible,they have already hit their max deductible for the year and should owe nothing. The xray deductible estimate is $0.")]
		public void Procedures_ComputeEstimates_CategoryDeductiblesShouldNotExceedRegularDeductibleClaim() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);//guarantor is subscriber
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.DiagnosticXRay,10);
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//proc1 - Prophy
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0110",ProcStat.TP,"",75,priority:1);
			//proc2 - Intraoral - periapical first film
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",25,priority:0);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,sub.InsSubNum);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(40,claimProc1.DedEst);
			Assert.AreEqual(10,claimProc2.DedEst);
			//Complete the first procedure and that should take up the entire $50 deductible, leaving nothing for the xray procedure
			ProcedureT.SetComplete(proc1,pat,planList,patPlans,claimProcs,benefitList,subList);
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,new List<Procedure>() { proc1 },benefitList,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claim,claimProcs.FindAll(x => x.ClaimProcNum==claimProc1.ClaimProcNum));
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);
			loopList=new List<ClaimProcHist>();
			//Validate
			Procedures.ComputeEstimates(proc2,pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(0,claimProc.DedEst);
		}

		///<summary></summary>
		[TestMethod]
		public void Procedures_ComputeEstimates_ProceduresWithCategoryDeductiblesAdhereToGeneralDeductibles() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			//$50 general deductible, $10 XRay deductible, 100% coverage
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.DiagnosticXRay,10);
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//proc1 - Prophy with $45 fee
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0110",ProcStat.TP,"",45,priority:1);
			//proc2 - XRay with $25 fee
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",25,priority:0);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,isInitialEntry:false,planList,patPlans,benefitList,
					histList,loopList,saveToDb:false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,sub.InsSubNum);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(40,claimProc1.DedEst);
			Assert.AreEqual(10,claimProc2.DedEst);
			//Complete the prophy and create a claim, leaving $5 for the xray procedure
			ProcedureT.SetComplete(proc1,pat,planList,patPlans,claimProcs,benefitList,subList);
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,new List<Procedure>() { proc1 },benefitList,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claim,claimProcs.FindAll(x => x.ClaimProcNum==claimProc1.ClaimProcNum));
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);
			loopList=new List<ClaimProcHist>();
			//Validate
			Procedures.ComputeEstimates(proc2,pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(5,claimProc.DedEst);
		}

		[TestMethod]
		public void Procedures_ComputeEstimates_FamilyDeductiblesBehaveTheSameAsIndividualDeductibles() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			//$200 general deductible, $50 XRay deductible, 100% coverage for individual
			//$50 general deductible, $10 XRay deductible, 100% coverage for family
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,200);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.DiagnosticXRay,50);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Family,50);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.DiagnosticXRay,10,BenefitCoverageLevel.Family);
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//proc1 - Prophy with $45 fee
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0110",ProcStat.TP,"",45,priority:1);
			//proc2 - XRay with $25 fee
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",25,priority:0);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,isInitialEntry:false,planList,patPlans,benefitList,
					histList,loopList,saveToDb:false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,sub.InsSubNum);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(40,claimProc1.DedEst);
			Assert.AreEqual(10,claimProc2.DedEst);
			//Complete the prophy and create a claim, leaving $5 for the xray procedure
			ProcedureT.SetComplete(proc1,pat,planList,patPlans,claimProcs,benefitList,subList);
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,new List<Procedure>() { proc1 },benefitList,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claim,claimProcs.FindAll(x => x.ClaimProcNum==claimProc1.ClaimProcNum));
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);
			loopList=new List<ClaimProcHist>();
			//Validate
			Procedures.ComputeEstimates(proc2,pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(5,claimProc.DedEst);
		}

		///<summary>The patient has one insurance plan with general deductible of $100, 100% coverage on Diagnostic, and a $50 xray deductible. 
		///We treatment plan the patient for two xrays (D0330 and D0220). The estimate is calculated and shows that we expect the patient to pay
		///$25 for one xray and $25 for another, hitting their xray deductible. We set one xray complete and create a claim which will require 
		///the patient pay their max xray deductible of $50. Finally, we treatment plan a cleaning procedure, complete it, and submit a claim.
		///The deductible comes out to $50. This means that this $50 that went towards their xray deductible also goes towards their general deductible.
		///<para>XRay: &#x09; $50 deductible&#x09;&#x09;-&#x09; $50 patient portion paid &#x09; = &#x09; $0 owed for patient's other xray</para>
		///<para>General:&#x09; $100 deductible &#x09;-&#x09; $50 xray deductible met &#x09; = &#x09; $50 owed for the patient's general deductible</para>
		///</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_CategoryDeductiblesDontExceedLimits)]
		[Documentation.VersionAdded("21.1.8")]
		[Documentation.Description(@"The patient has one insurance plan with general deductible of $100, 100% coverage on Diagnostic, and a $50 xray deductible. We treatment plan the patient for two xrays (D0330 and D0220). The estimate is calculated and shows that we expect the patient to pay $25 for one xray and $25 for another, hitting their xray deductible. We set one xray complete and create a claim which will require  the patient pay their max xray deductible of $50. Finally, we treatment plan a cleaning procedure, complete it, and submit a claim. The deductible comes out to $50. This means that this $50 that went towards their xray deductible also goes towards their general deductible. XRay: &#x09; $50 deductible&#x09;&#x09;-&#x09; $50 patient portion paid &#x09; = &#x09; $0 owed for patient's other xray General:&#x09; $100 deductible &#x09;-&#x09; $50 xray deductible met &#x09; = &#x09; $50 owed for the patient's general deductible")]

		public void Procedures_ComputeEstimates_CategoryDeductiblesDontExceedLimits() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);//guarantor is subscriber
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.DiagnosticXRay,50);
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//proc1 - Prophy
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0330",ProcStat.TP,"",75);
			ProcedureT.SetPriority(proc1,1);
			//proc2 - Intraoral - periapical first film
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",25);
			ProcedureT.SetPriority(proc2,0);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,sub.InsSubNum);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(25,claimProc1.DedEst);
			Assert.AreEqual(25,claimProc2.DedEst);
			//Complete the first procedure and that will use up the entire $50 XRay deductible.
			ProcedureT.SetComplete(proc1,pat,planList,patPlans,claimProcs,benefitList,subList);
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,new List<Procedure>() { proc1 },benefitList,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claim,claimProcs.FindAll(x => x.ClaimProcNum==claimProc1.ClaimProcNum));
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);
			loopList=new List<ClaimProcHist>();
			//Validate
			Procedures.ComputeEstimates(proc2,pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(0,claimProc.DedEst);
			//Create a cleaning and calculate the deductible for it
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0110",ProcStat.TP,"",75);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ProcList=Procedures.Refresh(pat.PatNum);
			loopList=new List<ClaimProcHist>();
			ProcListTP=Procedures.GetListTPandTPi(ProcList);
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			ClaimProc claimProc3=ClaimProcs.GetEstimate(claimProcs,proc3.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(50,claimProc3.DedEst);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_COBStandardTwoPPOs)]
		[Documentation.VersionAdded("12.0")]
		[Documentation.Description(@"  Patient has two insurance plans, both PPO, subscriber self for both. The only benefit entered for both insurance plans is 50% coverage on the crowns category.  One procedure is treatment planned, a D2750 PFM crown on #8. This table shows two different fee schedule scenarios.
        <table>
          <tr>
            <td>#</td>
            <td>Fee</td>
            <td>Allowed1</td>
            <td>Allowed2</td>
            <td>InsPay1</td>
            <td>InsPay2</td>
            <td>Writeoff1</td>
            <td>Writeoff2</td>
            <td>(implied pat port)</td>
          </tr>
          <tr>
            <td>1</td>
            <td>$1,200.00</td>
            <td>$900.00</td>
            <td>$650.00</td>
            <td>$450.00</td>
            <td>$325.00</td>
            <td>$300.00</td>
            <td>$0.00</td>
            <td>$125.00</td>
          </tr>
          <tr>
            <td>2</td>
            <td>$1,200.00</td>
            <td>$650.00</td>
            <td>$900.00</td>
            <td>$325.00</td>
            <td>$325.00</td>
            <td>$550.00</td>
            <td>$0.00</td>
            <td>$0.00</td>
          </tr>
        </table>
			")]




		public void Procedures_ComputeEstimates_COBStandardTwoPPOs() {
			string suffix="17";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedNum1=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			long feeSchedNum2=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix+"b");
			//Standard Fee
			long codeNum=ProcedureCodeT.GetCodeNum("D2750");
			Fee fee=Fees.GetFee(codeNum,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum;
				fee.FeeSched=53;
				fee.Amount=1200;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=1200;
				Fees.Update(fee);
			}
			//PPO fees
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum1;
			fee.Amount=900;
			Fees.Insert(fee);
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum2;
			fee.Amount=650;
			Fees.Insert(fee);
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum1,EnumCobRule.Standard).PlanNum;
			long planNum2=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum2,EnumCobRule.Standard).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.Crowns,50);
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.Crowns,50);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"8",Fees.GetAmount0(codeNum,53));//crown on 8
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			//Test 17 Part 1 (copied from Unit Test 1)----------------------------------------------------------------------------------------------------
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(450,claimProc.InsEstTotal);
			Assert.AreEqual(300,claimProc.WriteOffEst);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(325,claimProc.InsEstTotal);
			Assert.AreEqual(0,claimProc.WriteOffEst);
			//Test 17 Part 2 (copied from Unit Test 2)----------------------------------------------------------------------------------------------------
			//switch the fees
			fee=Fees.GetFee(codeNum,feeSchedNum1,0,0);
			fee.Amount=650;
			Fees.Update(fee);
			fee=Fees.GetFee(codeNum,feeSchedNum2,0,0);
			fee.Amount=900;
			Fees.Update(fee);
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			//Validate
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(325,claimProc.InsEstTotal);
			Assert.AreEqual(550,claimProc.WriteOffEst);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(325,claimProc.InsEstTotal);
			Assert.AreEqual(0,claimProc.WriteOffEst);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_COBCarveOutCategoryPercentagePlan)]
		[Documentation.VersionAdded("12.0")]
		[Documentation.Description("Patient has two insurance plans, subscriber self for both. Plan 1 has 50% coverage on the crowns category and plan 2 has 75% coverage. One procedure is treatment planned, a D2750 PFM crown on #8 for $1200. Primary estimate should be $600, and secondary $300.")]
		public void Procedures_ComputeEstimates_COBCarveOutCategoryPercentagePlan() {
			string suffix="18";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlan(carrier.CarrierNum,EnumCobRule.CarveOut).PlanNum;
			long planNum2=InsPlanT.CreateInsPlan(carrier.CarrierNum,EnumCobRule.CarveOut).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.Crowns,50);
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.Crowns,75);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"8",1200);//crown on 8
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			//Test 18 Part 1 (copied from Unit Test 1)----------------------------------------------------------------------------------------------------
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(600,claimProc.InsEstTotal);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(300,claimProc.InsEstTotal);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_MultipleDeductibles)]
		[Documentation.VersionAdded("12.0")]
		[Documentation.Description(
			@"<table>
          <tr>
            <td>Fee</td>
            <td>Ded1</td>
            <td>Est1</td>
            <td>PatPort1</td>
            <td>Ded2</td>
            <td>Est2</td>
            <td>patient out of pocket</td>
          </tr>
          <tr>
            <td>$150.00</td>
            <td>$50.00</td>
            <td>$50.00</td>
            <td>$100.00</td>
            <td>$50.00</td>
            <td>$50.00</td>
            <td>$50.00</td>
          </tr>
        </table>
        Patient has 
          two insurance plans, subscriber self for both. Both plans standard COB. Both plans have diagnostic 50% and a $50 general deductible. A limited exam, D0120 is treatment planned for $150. Primary estimate is calculated as (150-50) x 0.5 = $50. Secondary estimate with standard COB is the lesser of:
          1. The amount that it would have paid in the absence of any other coverage. (150-50) x 0.5 = $50
          2. The patient's portion under the primary plan. $100
          So $50.  
           Patient out of pocket = 150 - 50 - 50 = 50.")]
		public void Procedures_ComputeEstimates_MultipleDeductibles() {
			string suffix="19";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier1=CarrierT.CreateCarrier(suffix);
			Carrier carrier2=CarrierT.CreateCarrier(suffix);
			InsPlan plan1=InsPlanT.CreateInsPlan(carrier1.CarrierNum);
			InsPlan plan2=InsPlanT.CreateInsPlan(carrier2.CarrierNum);
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,plan1.PlanNum);
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,plan2.PlanNum);
			long subNum1=sub1.InsSubNum;
			long subNum2=sub2.InsSubNum;
			//plans
			BenefitT.CreateCategoryPercent(plan1.PlanNum,EbenefitCategory.Diagnostic,50);
			BenefitT.CreateCategoryPercent(plan2.PlanNum,EbenefitCategory.Diagnostic,50);
			BenefitT.CreateDeductibleGeneral(plan1.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateDeductibleGeneral(plan2.PlanNum,BenefitCoverageLevel.Individual,50);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum1);
			PatPlanT.CreatePatPlan(2,pat.PatNum,subNum2);
			//proc1 - PerExam
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.TP,"",150);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc.ProcNum,plan1.PlanNum,subNum1);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc.ProcNum,plan2.PlanNum,subNum2);
			Assert.AreEqual(50,claimProc1.DedEst);
			Assert.AreEqual(50,claimProc1.InsEstTotal);
			Assert.AreEqual(50,claimProc2.DedEst);
			Assert.AreEqual(50,claimProc2.InsEstTotal);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_DeductiblesForProcsNotCovered)]
		[Documentation.Description("Patient has one insurance plan, subscriber self. Benefits: annual max $1000, deductible $50. One treatment plannned procedure D0120 fee $55. In the TP module, deductible should be $0.")]
		public void Procedures_ComputeEstimates_DeductiblesForProcsNotCovered() {
			string suffix="21";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);//guarantor is subscriber
			BenefitT.CreateAnnualMax(plan.PlanNum,1000);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//proc - Exam
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.TP,"",55);
			ProcedureT.SetPriority(proc1,0);
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
			List<Procedure>	ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Validate
			for(int i=0;i<ProcListTP.Length;i++){
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,sub.InsSubNum);
			Assert.AreEqual(0,claimProc1.DedEst);
		}

		///<summary>Validates that procedure specific deductibles take general deductibles into consideration.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_GeneralDeductiblesConsideredWithProcedureSpecificDeductibles)]
		[Documentation.VersionAdded("12.3.45")]
		[Documentation.Description("Validates that procedure specific deductibles take general deductibles into consideration.")]
		public void Procedures_ComputeEstimates_GeneralDeductiblesConsideredWithProcedureSpecificDeductibles() {
			string suffix="34";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMax(plan.PlanNum,1000);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.RoutinePreventive,100);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.RoutinePreventive,0);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateDeductible(plan.PlanNum,"D1351",50);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - PerExam
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",45);
			//proc2 - Sealant
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1351",ProcStat.TP,"5",54);
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
			Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			//Attach to claim
			ClaimProcT.AddInsPaid(pat.PatNum,plan.PlanNum,proc1.ProcNum,0,subNum,45,0);
			//Validate
			histList=ClaimProcs.GetHistList(pat.PatNum,benefitList,patPlans,planList,DateTime.Today,subList);
			Procedures.ComputeEstimates(proc2,pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
				histList,loopList,false,pat.Age,subList);
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan.PlanNum,subNum);
			Assert.AreEqual(5,claimProc.DedEst);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_COBStandardDualPPOWriteoffZero)]
		[Documentation.VersionAdded("14.3")]
		[Documentation.Description(@"Similar to unit test 17. Ensures that the secondary writeoff is zero when the primary writeoff is zero.
Patient has two insurance plans, both PPO with Standard COB rule, subscriber self for both.  The primary insurance covers 50% perio and secondary covers 80% perio.  One perio procedure is treatment planned, a D4341 scaling &amp; root planing, any quadrant.")]
		public void Procedures_ComputeEstimates_COBStandardDualPPOWriteoffZero() {
			string suffix="36";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedNum1=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			long feeSchedNum2=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix+"b");
			//Standard Fee (we only insert this value to test that it is not used in the calculations).
			long codeNum=ProcedureCodeT.GetCodeNum("D4341");
			Fee fee=Fees.GetFee(codeNum,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum;
				fee.FeeSched=53;
				fee.Amount=1200;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=1200;
				Fees.Update(fee);
			}
			//PPO fees
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum1;
			fee.Amount=206;
			Fees.Insert(fee);
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum2;
			fee.Amount=117;
			Fees.Insert(fee);
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum1,EnumCobRule.Standard).PlanNum;
			long planNum2=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum2,EnumCobRule.Standard).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.Periodontics,50);
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.Periodontics,80);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.TP,"",206);//Scaling in undefined/any quadrant.
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(103,claimProc.InsEstTotal);
			Assert.AreEqual(0,claimProc.WriteOffEst);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(93.6,claimProc.InsEstTotal);
			Assert.AreEqual(0,claimProc.WriteOffEst);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_PPOProceduresMultipleUnits)]
		[Documentation.VersionAdded("15.2")]
		[Documentation.Description(@"Patient has one PPO insurance plan, subscriber self.  The insurance covers 80% Diagnostic, and has a $10 individual general deductible.  The insurance plan has a copay of $5 and an allowed amount of $40 for procedure code D0270.  One procedure is complete, a D0270 single bitewing.  The procedure unit quantity is 3.  The fee for each D0270 is $50.
<table>
  <tr>
    <td>Copay</td>
    <td>Deduct</td>
    <td>Fee</td>
    <td>Allowed</td>
    <td>InsEst</td>
    <td>Writeoff</td>
    <td>PatientPortion</td>
  </tr>
  <tr>
    <td>5*3=15</td>
    <td>10 </td>
    <td>50*3=150</td>
    <td>40*3=120</td>
    <td>95*0.8=76</td>
    <td>150-120=30</td>
    <td>150-76-30=44</td>
  </tr>
</table>")]
		[TestMethod]
		public void Procedures_ComputeEstimates_PPOProceduresMultipleUnits() {
			string suffix="37";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedNum1=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			//Standard Fee (we only insert this value to test that it is not used in the calculations).
			long codeNum=ProcedureCodeT.GetCodeNum("D0270");//1BW
			//PPO fee
			Fee fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum1;
			fee.Amount=40;
			Fees.Insert(fee);
			//Copay fee schedule
			long feeSchedNumCopay=FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay,suffix);
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNumCopay;
			fee.Amount=5;
			Fees.Insert(fee);
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum1,EnumCobRule.Basic).PlanNum;
			BenefitT.CreateDeductibleGeneral(planNum1,BenefitCoverageLevel.Individual,10);
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.DiagnosticXRay,80);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",50);//1BW
			Procedure procOld=proc.Copy();
			proc.UnitQty=3;
			Procedures.Update(proc,procOld);//1BW x 3
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			InsPlan insPlan1=InsPlans.GetPlan(planNum1,planList);
			InsPlan planOld = insPlan1.Copy();
			insPlan1.CopayFeeSched=feeSchedNumCopay;
			InsPlans.Update(insPlan1,planOld);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(76,claimProc.InsEstTotal);
			Assert.AreEqual(30,claimProc.WriteOffEst);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_CategoryPercentageProceduresMultipleUnits)]
		[Documentation.VersionAdded("15.2")]
		[Documentation.Description(@"Patient has one Category Percentage insurance plan, subscriber self.  The insurance covers 80% Diagnostic.  One procedure is complete, a D0270 single bitewing.  The procedure unit quantity is 2.  The fee for each D0270 is $50.
<table>
  <tr>
    <td>Fee</td>
    <td>InsEst</td>
  </tr>
  <tr>
    <td>50*2=100</td>
    <td>100*0.8=80</td>
  </tr>
</table>")]
		[TestMethod]
		public void Procedures_ComputeEstimates_CategoryPercentageProceduresMultipleUnits() {
			string suffix="38";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlan(carrier.CarrierNum).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.DiagnosticXRay,80);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",50);//1BW
			Procedure procOld=proc.Copy();
			proc.UnitQty=2;
			Procedures.Update(proc,procOld);//1BW x 2
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(80,claimProc.InsEstTotal);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_PPOProceduresMultipleUnitsWriteoff)]
		[Documentation.VersionAdded("15.2")]
		[Documentation.Description(@"Patient has two PPO Percentage plans, both plans with subscriber self.  Both primary and secondary cover 80% Diagnostic.  The primary insurance plan has an allowed amount of $40 for a D0270 and the secondary plan has an allowed amount of $30 for a D0270.  One procedure is treatment planned, a D0270 single bitewing.  The procedure unit quantity is 4.  The fee for each D0270 is $50.
<table>
  <tr>
    <td>Fee</td>
    <td>Allowed1</td>
    <td>Allowed2</td>
    <td>InsPay1</td>
    <td>InsPay2</td>
    <td>Writeoff1</td>
    <td>Writeoff2</td>
  </tr>
  <tr>
    <td>50*4=200 </td>
    <td>40*4=160</td>
    <td>30*4=120</td>
    <td>160*0.8=128</td>
    <td>0</td>
    <td> 200-160=40</td>
    <td>0</td>
  </tr>
</table>")]
		[TestMethod]
		public void Procedures_ComputeEstimates_PPOProceduresMultipleUnitsWriteoff() {
			string suffix="39";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedNum1=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			long feeSchedNum2=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix+"b");
			//Standard Fee (we only insert this value to test that it is not used in the calculations).
			long codeNum=ProcedureCodeT.GetCodeNum("D0270");
			//PPO fees
			Fee fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum1;
			fee.Amount=40;
			Fees.Insert(fee);
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum2;
			fee.Amount=30;
			Fees.Insert(fee);
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum1,EnumCobRule.Basic).PlanNum;
			long planNum2=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum2,EnumCobRule.Basic).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.DiagnosticXRay,80);
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.DiagnosticXRay,80);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.TP,"",50);//Scaling in undefined/any quadrant.
			Procedure procOld=proc.Copy();
			proc.UnitQty=4;
			Procedures.Update(proc,procOld);//1BW x 4
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(128,claimProc.InsEstTotal);
			Assert.AreEqual(40,claimProc.WriteOffEst);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(0,claimProc.InsEstTotal);
			Assert.AreEqual(0,claimProc.WriteOffEst);
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_InsPPOsecWriteoffsPreference)]
		[Documentation.VersionAdded("15.2")]
		[Documentation.Description(@"Patient has a Category Percentage primary insurance plan and PPO secondary plan with Basic COB rule, both plans with subscriber self.  Primary covers 80% Diagnostic, secondary covers 100% Diagnostic.  The primary insurance has a general deductible of $50 and an allowed amount of $152 for a treatment planned D0272 two bitewings procedure, and secondary has an allowed amount of $87.99 for D0272.  The fee on the treatment planned D0272 is $236.
<table>
  <tr>
    <td>Fee</td>
    <td>Deduct</td>
    <td>Allowed1</td>
    <td>Allowed2</td>
    <td>InsPay1</td>
    <td>InsPay2</td>
    <td>Writeoff1</td>
    <td>Writeoff2</td>
    <td>PatPortion</td>
  </tr>
  <tr>
    <td>236</td>
    <td>50</td>
    <td>152</td>
    <td>87.99</td>
    <td>(152-50)*0.8+81.6</td>
    <td>87.99-81.6=6.39</td>
    <td>0</td>
    <td>236-81.60-6.39+148.01</td>
    <td>0</td>
  </tr>
</table>
The preference used in this unit test is not recommended and is rarely used.
The preference is documented on <a href='../manual/modulesetupfamily.html'>Family Module Preferences</a>.
If instead the preference was off for this unit test, then Writeoff2 would have been 0 and the Pat Portion would have been 148.01.
")]
		[TestMethod]
		public void Procedures_ComputeEstimates_InsPPOsecWriteoffsPreference() {
			string suffix="40";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedAllowedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.OutNetwork,suffix+"-allowed");
			long feeSchedNum2=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix+"b");
			//Standard Fee (we only insert this value to test that it is not used in the calculations).
			long codeNum=ProcedureCodeT.GetCodeNum("D0272");
			Fee fee=Fees.GetFee(codeNum,feeSchedAllowedNum,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum;
				fee.FeeSched=feeSchedAllowedNum;
				fee.Amount=152;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=152;
				Fees.Update(fee);
			}
			//PPO fees
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum2;
			fee.Amount=87.99;
			Fees.Insert(fee);
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			//Plan 1 - Category Percentage
			long planNum1=InsPlanT.CreateInsPlan(carrier.CarrierNum).PlanNum;
			InsPlan insPlan1=InsPlans.RefreshOne(planNum1);
			InsPlan planOld = insPlan1.Copy();
			insPlan1.FeeSched=0;
			insPlan1.AllowedFeeSched=feeSchedAllowedNum;
			InsPlans.Update(insPlan1,planOld);
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.DiagnosticXRay,80);
			BenefitT.CreateDeductibleGeneral(planNum1,BenefitCoverageLevel.Individual,50);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			//Plan 2 - PPO
			long planNum2=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum2,EnumCobRule.Basic).PlanNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.DiagnosticXRay,100);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0272",ProcStat.TP,"",236);
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			//if(specificTest==0 || specificTest==40) {
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			//Test insurance numbers without calculating secondary PPO insurance writeoffs
			Assert.AreEqual(81.6,claimProc.InsEstTotal);
			Assert.AreEqual(-1,claimProc.WriteOffEst);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(6.39,claimProc.InsEstTotal);
			Assert.AreEqual(0,claimProc.WriteOffEst);
			//Now test insurance numbers with calculating secondary PPO insurance writeoffs
			Prefs.UpdateBool(PrefName.InsPPOsecWriteoffs,true);
			Procedures.ComputeEstimates(proc,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum1,subNum1);
			Assert.AreEqual(81.6,claimProc.InsEstTotal);
			Assert.AreEqual(-1,claimProc.WriteOffEst);
			claimProc=ClaimProcs.GetEstimate(claimProcs,procNum,planNum2,subNum2);
			Assert.AreEqual(6.39,claimProc.InsEstTotal);
			Assert.AreEqual(148.01,claimProc.WriteOffEst);
			Prefs.UpdateBool(PrefName.InsPPOsecWriteoffs,false);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_OverrideUnderFamilyMaxEstimateNote)]
		public void Procedures_ComputeEstimates_OverrideUnderFamilyMaxEstimateNote() {
			string suffix="49";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMaxFamily(plan.PlanNum,400);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Restorative,100);
			PatPlan pplan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//procs - D2140 (amalgum fillings)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2140",ProcStat.TP,"18",500);
			//Lists
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
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//change override
			claimProcs[0].InsEstTotalOverride=399;
			ClaimProcs.Update(claimProcs[0]);
			//Lists2
			List<ClaimProc> claimProcs2=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld2=ClaimProcs.Refresh(pat.PatNum);
			Family fam2=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList2=InsSubs.RefreshForFam(fam2);
			List<InsPlan> planList2=InsPlans.RefreshForSubList(subList2);
			List<PatPlan> patPlans2=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList2=Benefits.Refresh(patPlans2,subList2);
			List<ClaimProcHist> histList2=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList2=new List<ClaimProcHist>();
			List<Procedure> ProcList2=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP2=Procedures.GetListTPandTPi(ProcList2);//sorted by priority, then toothnum
			//Validate again
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP2[i],pat.PatNum,ref claimProcs2,false,planList2,patPlans2,benefitList2,
					histList2,loopList2,false,pat.Age,subList2);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs2,ProcListTP2[i].ProcNum,ProcListTP2[i].CodeNum));
			}
			ClaimProcs.Synch(ref claimProcs2,claimProcListOld2);
			claimProcs2=ClaimProcs.Refresh(pat.PatNum);
			//Check to see if note still says over annual max
			Assert.AreEqual("",claimProcs2[0].EstimateNote);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_OverrideOverFamilyMaxEstimateNote)]
		public void Procedures_ComputeEstimates_OverrideOverFamilyMaxEstimateNote() {
			string suffix="50";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMaxFamily(plan.PlanNum,400);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Restorative,100);
			PatPlan pplan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//procs - D2140 (amalgum fillings)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2140",ProcStat.TP,"18",500);
			//Lists
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
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//change override
			claimProcs[0].InsEstTotalOverride=401;
			ClaimProcs.Update(claimProcs[0]);
			//Lists2
			List<ClaimProc> claimProcs2=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld2=ClaimProcs.Refresh(pat.PatNum);
			Family fam2=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList2=InsSubs.RefreshForFam(fam2);
			List<InsPlan> planList2=InsPlans.RefreshForSubList(subList2);
			List<PatPlan> patPlans2=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList2=Benefits.Refresh(patPlans2,subList2);
			List<ClaimProcHist> histList2=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList2=new List<ClaimProcHist>();
			List<Procedure> ProcList2=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP2=Procedures.GetListTPandTPi(ProcList2);//sorted by priority, then toothnum
			//Validate again
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP2[i],pat.PatNum,ref claimProcs2,false,planList2,patPlans2,benefitList2,
					histList2,loopList2,false,pat.Age,subList2);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs2,ProcListTP2[i].ProcNum,ProcListTP2[i].CodeNum));
			}
			ClaimProcs.Synch(ref claimProcs2,claimProcListOld2);
			claimProcs2=ClaimProcs.Refresh(pat.PatNum);
			//Check to see if note still says over annual max
			Assert.AreEqual("Over family max",claimProcs2[0].EstimateNote);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_MedicalDeductible)]
		public void Procedures_ComputeEstimates_MedicalDeductible() {
			string suffix="51";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsPlan planOld = plan.Copy();
			plan.IsMedical=true;
			InsPlans.Update(plan,planOld);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.None,50.00);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.OralSurgery,80);
			PatPlan pplan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//procs - D7140 (extraction)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D7140",ProcStat.TP,"18",500);
			//Lists
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
			Procedures.ComputeEstimates(ProcList[0],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
				histList,loopList,false,pat.Age,subList);
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			Claim ClaimCur=ClaimT.CreateClaim("Med",patPlans,planList,claimProcs,ProcList,pat,ProcList,benefitList,subList);
			List<ClaimProc> ClaimProcList=ClaimProcs.Refresh(pat.PatNum);
			ClaimCur.ClaimStatus="W";
			ClaimCur.DateSent=DateTime.Today;
			Claims.CalculateAndUpdate(ProcList,planList,ClaimCur,patPlans,benefitList,pat,subList);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Check to see if deductible applied correctly
			Assert.AreEqual(50.00,claimProcs[0].DedApplied);
		}

		///<summary>A patient has Medicaid/FlatCopay as secondary insurance. It should use its own fee schedule when calculating its coverage.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_MedicalFlatCopaySecondaryFeeSchedule)]
		[Documentation.Description("A patient has Medicaid/FlatCopay as secondary insurance. It should use its own fee schedule when calculating its coverage.")]

		public void Procedures_ComputeEstimates_MedicalFlatCopaySecondaryFeeSchedule() {
			//need one patient
			//one office fee schedule
			//crowns cost 1500
			//one PPO ins fee sched
			//crowns cost 1000
			//one medicaid fee sched
			//crowns cost 0.00
			//one ppo insurance
			//covers crowns at 50%
			//annual max of 500
			//one medicaid ins
			//crown is charted
			//output should be:
			//PPO covers 500
			//writeoff is 500
			//Medicaid covers 0.00
			//writeoff is 500.00
			long officeFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Office");
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO");
			long medicaidFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Medicaid");
			long copayFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay,"CoPay");
			long codeNum=ProcedureCodeT.GetCodeNum("D2750");
			long provNum=ProviderT.CreateProvider("Prov","","",officeFeeSchedNum);
			Patient pat=PatientT.CreatePatient("60",provNum);
			FeeT.CreateFee(officeFeeSchedNum,codeNum,1500); //office fee is 1500.
			FeeT.CreateFee(ppoFeeSchedNum,codeNum,1100); //ppo fee is 1100. writeoff should be 400
			FeeT.CreateFee(medicaidFeeSchedNum,codeNum,30); //medicaid fee is 30.
			FeeT.CreateFee(copayFeeSchedNum,codeNum,15); //copay is 15, so ins est should be 30 - 15 = 15.
																									 //Carrier
			Carrier ppoCarrier=CarrierT.CreateCarrier("PPO");
			Carrier medicaidCarrier=CarrierT.CreateCarrier("Medicaid");
			long planPPO=InsPlanT.CreateInsPlanPPO(ppoCarrier.CarrierNum,ppoFeeSchedNum).PlanNum;
			long planMedi=InsPlanT.CreateInsPlanMediFlatCopay(medicaidCarrier.CarrierNum,medicaidFeeSchedNum,copayFeeSchedNum).PlanNum;
			InsSub subPPO=InsSubT.CreateInsSub(pat.PatNum,planPPO);
			long subNumPPO=subPPO.InsSubNum;
			InsSub subMedi=InsSubT.CreateInsSub(pat.PatNum,planMedi);
			long subNumMedi=subMedi.InsSubNum;
			BenefitT.CreateCategoryPercent(planPPO,EbenefitCategory.Crowns,50);
			BenefitT.CreateAnnualMax(planPPO,500);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNumPPO);
			PatPlanT.CreatePatPlan(2,pat.PatNum,subNumMedi);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"14",Fees.GetAmount0(codeNum,officeFeeSchedNum));
			long procNum=proc.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//this is what's really being tested.
			//must pass in the empty histList and loopList (instead of null) or annual max's don't get considered.
			Procedures.ComputeEstimates(proc,pat.PatNum,ref claimProcs,true,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			List<ClaimProc> listResult=ClaimProcs.RefreshForProc(procNum);
			ClaimProc ppoClaimProc=listResult.Where(x => x.PlanNum == planPPO).First();
			ClaimProc medicaidClaimProc=listResult.Where(x => x.PlanNum == planMedi).First();
			Assert.AreEqual(500,ppoClaimProc.InsEstTotal);
			Assert.AreEqual(15,medicaidClaimProc.InsEstTotal);
			Assert.AreEqual(-1,medicaidClaimProc.WriteOffEst);
			Assert.AreEqual(400,ppoClaimProc.WriteOffEst);
			Assert.AreEqual(15,medicaidClaimProc.CopayAmt);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_CategoryPercentageCanadianLabFees)]

		public void Procedures_ComputeEstimates_CategoryPercentageCanadianLabFees() {
			CultureInfo cultureOld=Thread.CurrentThread.CurrentCulture;
			CultureInfo uiCultureOld=Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-CA");
			//Mimics TestThirtyEight(...)
			string suffix="73";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			//Set up insurace.
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum=InsPlanT.CreateInsPlan(carrier.CarrierNum).PlanNum;
			InsSub sub=InsSubT.CreateInsSub(patNum,planNum);
			long subNum1=sub.InsSubNum;
			BenefitT.CreateCategoryPercent(planNum,EbenefitCategory.General,50);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			//Procedure 1 - Parent Proc
			Procedure procParent=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.TP,"",100);
			Procedure procOld=procParent.Copy();
			Procedures.Update(procParent,procOld);
			//Procedure 2 - Lab Fee
			Procedure procLabFee=ProcedureT.CreateProcedure(pat,"D0272",ProcStat.TP,"",10);
			procOld=procLabFee.Copy();
			procLabFee.ProcNumLab=procParent.ProcNum;
			Procedures.Update(procLabFee,procOld);
			long procNum=procParent.ProcNum;
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			Procedures.ComputeEstimates(procParent,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);//Triggers claimProc creation for lab too.
			Thread.CurrentThread.CurrentCulture=cultureOld;
			Thread.CurrentThread.CurrentUICulture=uiCultureOld;
			claimProcs=ClaimProcs.Refresh(patNum);
			ClaimProc claimProcParent=ClaimProcs.GetEstimate(claimProcs,procNum,planNum,subNum1);
			ClaimProc claimProcLab=ClaimProcs.GetEstimate(claimProcs,procLabFee.ProcNum,planNum,subNum1);
			Assert.AreEqual(50,claimProcParent.InsEstTotal);
			Assert.AreEqual(5,claimProcLab.InsEstTotal);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_OrthoMaxSeparateFamilyAndIndividualMaxes)]

		public void Procedures_ComputeEstimates_OrthoMaxSeparateFamilyAndIndividualMaxes() {
			string suffix="75";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMaxFamily(plan.PlanNum,400);
			BenefitT.CreateAnnualMax(plan.PlanNum,200);
			BenefitT.CreateOrthoMax(plan.PlanNum,1500);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Orthodontics,100);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//procs - D2140 (amalgum fillings)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D8090",ProcStat.TP,"",2000);//Comprehensive ortho
			ProcedureT.SetPriority(proc1,0);
			//Lists
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
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,patPlans[0].InsSubNum);
			Assert.AreEqual(1500,claimProc1.InsEstTotal);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_OrthoFamilyMaxOverLargerIndividual)]
		public void Procedures_ComputeEstimates_OrthoFamilyMaxOverLargerIndividual() {
			string suffix="76";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateAnnualMaxFamily(plan.PlanNum,400);
			BenefitT.CreateAnnualMax(plan.PlanNum,200);
			BenefitT.CreateOrthoMax(plan.PlanNum,1500);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Orthodontics,100);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			BenefitT.CreateOrthoFamilyMax(plan.PlanNum,1200);
			//procs - D2140 (amalgum fillings)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D8090",ProcStat.TP,"",2000);//Comprehensive ortho
			ProcedureT.SetPriority(proc1,0);
			//Lists
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
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc1.ProcNum,plan.PlanNum,patPlans[0].InsSubNum);
			Assert.AreEqual(1200,claimProc1.InsEstTotal);
		}

		///<summary>Making sure that deductible estimates from both insurances are not applied to the same procedure when calculating deductibles for
		///subsequent procedures.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_DeductiblesDualInsuranceNotNegative)]
		[Documentation.Description("Making sure that deductible estimates from both insurances are not applied to the same procedure when calculating deductibles for subsequent procedures.")]
		public void Procedures_ComputeEstimates_DeductiblesDualInsuranceNotNegative() {
			string suffix="80";
			Patient pat=PatientT.CreatePatient(suffix);
			//Add primary and secondary insurance with a $100 deductible
			Carrier carrier1=CarrierT.CreateCarrier(suffix+"_pri");
			InsPlan plan1=InsPlanT.CreateInsPlan(carrier1.CarrierNum);
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,plan1.PlanNum);
			BenefitT.CreateDeductibleGeneral(plan1.PlanNum,BenefitCoverageLevel.Individual,100);
			BenefitT.CreateCategoryPercent(plan1.PlanNum,EbenefitCategory.General,80);
			PatPlanT.CreatePatPlan(1,pat.PatNum,sub1.InsSubNum);
			Carrier carrier2=CarrierT.CreateCarrier(suffix+"_sec");
			InsPlan plan2=InsPlanT.CreateInsPlan(carrier2.CarrierNum);
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,plan2.PlanNum);
			BenefitT.CreateDeductibleGeneral(plan2.PlanNum,BenefitCoverageLevel.Individual,100);
			BenefitT.CreateCategoryPercent(plan2.PlanNum,EbenefitCategory.General,70);
			PatPlanT.CreatePatPlan(2,pat.PatNum,sub2.InsSubNum);
			//Add two procedures
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2391",ProcStat.TP,"28",90);//Composite
			ProcedureT.SetPriority(proc1,1);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2391",ProcStat.TP,"29",90);//Composite
			ProcedureT.SetPriority(proc2,1);
			//Lists
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
			//Calculate estimates
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
					histList,loopList,false,pat.Age,subList);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			claimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Validate the deductibles on the second procedure
			ClaimProc claimProc1=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan1.PlanNum,sub1.InsSubNum);
			ClaimProc claimProc2=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,plan2.PlanNum,sub2.InsSubNum);
			string errorMessage="";
			if(claimProc1.DedEst!=10) {
				errorMessage+="Deductible 1 was "+claimProc1.DedEst+". Should be 10.\r\n";
			}
			if(claimProc1.DedEst!=10) {
				errorMessage+="Deductible 2 was "+claimProc2.DedEst+". Should be 10.\r\n";
			}
			Assert.AreEqual("",errorMessage);
		}

		///<summary>Makes sure that if two claimprocs are created for the same frequency limitation group and one is marked received, the 
		///estimate claimproc will show as 0 coverage because the limitation is met.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_FrequencyLimitationMet)]
		[Documentation.Description("Makes sure that if two claimprocs are created for the same frequency limitation group and one is marked received, the estimate claimproc will show as 0 coverage because the limitation is met.")]

		public void Procedures_ComputeEstimates_FrequencyLimitationMet() {
			string suffix="81";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedNum1=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			//Standard Fee
			long codeNum=ProcedureCodeT.GetCodeNum("D0270");
			Fee fee=Fees.GetFee(codeNum,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum;
				fee.FeeSched=53;
				fee.Amount=1200;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=1200;
				Fees.Update(fee);
			}
			long codeNum2=ProcedureCodeT.GetCodeNum("D0272");
			Fee fee2=Fees.GetFee(codeNum2,53,0,0);
			if(fee2==null) {
				fee2=new Fee();
				fee2.CodeNum=codeNum2;
				fee2.FeeSched=53;
				fee2.Amount=100;
				Fees.Insert(fee2);
			}
			else {
				fee2.Amount=100;
				Fees.Update(fee2);
			}
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum1).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			//BW Frequency group - D0270, D0272, D0273, D0274
			BenefitT.CreateFrequencyProc(planNum1,"D0270",BenefitQuantity.Months,1);
			BenefitT.CreateFrequencyProc(planNum1,"D0272",BenefitQuantity.Months,1);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0270",ProcStat.C,"",Fees.GetAmount0(codeNum,53),DateTime.Today.AddDays(-1));
			ClaimProcT.AddInsPaid(proc.PatNum,planNum1,proc.ProcNum,proc.ProcFee,subNum1,0,0,proc.ProcDate);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0272",ProcStat.TP,"",Fees.GetAmount0(codeNum2,53));
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(patNum);
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(patNum,benefitList,patPlans,new List<InsPlan>() {InsPlans.GetPlan(planNum1,null)},proc2.ProcDate,subList);
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate
			ClaimProc claimProc;
			//Doesn't create a claimproc for the second one.
			Procedures.ComputeEstimates(proc2,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc=ClaimProcs.GetEstimate(claimProcs,proc2.ProcNum,planNum1,subNum1);
			//I don't think allowed can be easily tested on the fly, and it's not that important.
			//if(claimProc.InsEstTotal!=0) {
			//	throw new Exception("Estimate Should be 0. \r\n");
			//}
			Assert.AreEqual(0,claimProc.InsEstTotal);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_MultipleProceduresOneClaimExceedAnnualMax)]

		public void Procedures_ComputeEstimates_MultipleProceduresOneClaimExceedAnnualMax() {
			string suffix="83";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedPPO=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			//Standard Fee
			long codeNum1=ProcedureCodeT.GetCodeNum("D2740");
			Fee fee=Fees.GetFee(codeNum1,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum1;
				fee.FeeSched=53;
				fee.Amount=2000;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=2000;
				Fees.Update(fee);
			}
			//PPO fee
			fee=new Fee();
			fee.CodeNum=codeNum1;
			fee.FeeSched=feeSchedPPO;
			fee.Amount=900;
			Fees.Insert(fee);
			long codeNum2=ProcedureCodeT.GetCodeNum("D2750");
			fee=Fees.GetFee(codeNum2,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum2;
				fee.FeeSched=53;
				fee.Amount=1000;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=1000;
				Fees.Update(fee);
			}
			//PPO fee
			fee=new Fee();
			fee.CodeNum=codeNum2;
			fee.FeeSched=feeSchedPPO;
			fee.Amount=500;
			Fees.Insert(fee);
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedPPO).PlanNum;
			long planNum2=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedPPO).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			BenefitT.CreateAnnualMax(planNum1,1000);
			BenefitT.CreateDeductibleGeneral(planNum1,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateCategoryPercent(planNum1,EbenefitCategory.Crowns,100);
			BenefitT.CreateAnnualMax(planNum2,250);
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.Crowns,100);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc2740=ProcedureT.CreateProcedure(pat,"D2740",ProcStat.TP,"7",Fees.GetAmount0(codeNum1,53));//crown on 7
			Procedure proc2750=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"8",Fees.GetAmount0(codeNum2,53));//crown on 8
			//Lists
			List<Procedure> procedures=new List<Procedure>() { proc2740,proc2750 };
			List<ClaimProc> claimProcs=new List<ClaimProc>();
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Validate the estimates and write off amounts before creating a claim.
			Procedures.ComputeEstimates(proc2740,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			Procedures.ComputeEstimates(proc2750,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			ClaimProc claimProc2740Pri=ClaimProcs.GetEstimate(claimProcs,proc2740.ProcNum,planNum1,subNum1);
			ClaimProc claimProc2750Pri=ClaimProcs.GetEstimate(claimProcs,proc2750.ProcNum,planNum1,subNum1);
			Assert.AreEqual(850,claimProc2740Pri.InsEstTotal);
			Assert.AreEqual(1100,claimProc2740Pri.WriteOffEst);
			Assert.AreEqual(450,claimProc2750Pri.InsEstTotal);
			Assert.AreEqual(500,claimProc2750Pri.WriteOffEst);
			//Create a claim for the two procedures.
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,procedures,pat,procedures,benefitList,subList);
			//Grab the new estimates, they should be corrected so that they know about eachother (annual max).
			claimProcs=ClaimProcs.Refresh(patNum);
			claimProc2740Pri=ClaimProcs.GetEstimate(claimProcs,proc2740.ProcNum,planNum1,subNum1);
			claimProc2750Pri=ClaimProcs.GetEstimate(claimProcs,proc2750.ProcNum,planNum1,subNum1);
			Assert.AreEqual(850,claimProc2740Pri.InsEstTotal);
			Assert.AreEqual(1100,claimProc2740Pri.WriteOffEst);
			Assert.AreEqual(150,claimProc2750Pri.InsEstTotal);
			Assert.AreEqual(500,claimProc2750Pri.WriteOffEst);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_COBCarveOutSecondaryInsurance)]

		public void Procedures_ComputeEstimates_COBCarveOutSecondaryInsurance() {
			string suffix="84";
			Patient pat=PatientT.CreatePatient(suffix);
			long patNum=pat.PatNum;
			long feeSchedPPO=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			//Standard Fee
			long codeNum1=ProcedureCodeT.GetCodeNum("D2740");
			Fee fee=Fees.GetFee(codeNum1,53,0,0);
			if(fee==null) {
				fee=new Fee();
				fee.CodeNum=codeNum1;
				fee.FeeSched=53;
				fee.Amount=2000;
				Fees.Insert(fee);
			}
			else {
				fee.Amount=2000;
				Fees.Update(fee);
			}
			//Carrier
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			long planNum1=InsPlanT.CreateInsPlan(carrier.CarrierNum).PlanNum;
			long planNum2=InsPlanT.CreateInsPlan(carrier.CarrierNum,EnumCobRule.CarveOut).PlanNum;
			InsSub sub1=InsSubT.CreateInsSub(pat.PatNum,planNum1);
			long subNum1=sub1.InsSubNum;
			InsSub sub2=InsSubT.CreateInsSub(pat.PatNum,planNum2);
			long subNum2=sub2.InsSubNum;
			PrefT.UpdateBool(PrefName.InsEstRecalcReceived,true);
			BenefitT.CreateDeductibleGeneral(planNum2,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateCategoryPercent(planNum2,EbenefitCategory.Crowns,80);
			PatPlanT.CreatePatPlan(1,patNum,subNum1);
			PatPlanT.CreatePatPlan(2,patNum,subNum2);
			Procedure proc2740=ProcedureT.CreateProcedure(pat,"D2740",ProcStat.C,"7",1200);//crown on 7
			List<Procedure> procedures=new List<Procedure>() { proc2740 };
			List<ClaimProc> claimProcs=new List<ClaimProc>();
			Family fam=Patients.GetFamily(patNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(patNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			Procedures.ComputeEstimates(proc2740,patNum,ref claimProcs,false,planList,patPlans,benefitList,histList,loopList,true,pat.Age,subList);
			//Create both claims for the procedure.
			Claim claim1=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,procedures,pat,procedures,benefitList,subList);
			ClaimProcT.AddInsPaid(patNum,planNum1,proc2740.ProcNum,750,subNum1,0,0);
			Claims.CalculateAndUpdate(procedures,planList,claim1,patPlans,benefitList,pat,subList);
			Claim claim2=ClaimT.CreateClaim("S",patPlans,planList,claimProcs,procedures,pat,procedures,benefitList,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			ClaimProc claimProc2740Sec=ClaimProcs.GetEstimate(claimProcs,proc2740.ProcNum,planNum2,subNum2);
			//Secondary InsEst = (Secondary Allowed - Secondary Deductible) * Secondary Percentage - PaidOther
			//170 = (1200 - 50) * .8 - 750
			Assert.AreEqual(170,claimProc2740Sec.InsEstTotal);
			Assert.AreEqual(50,claimProc2740Sec.DedEst);
		}

		///<summary></summary>
		[TestMethod]
		
		public void Procedures_ComputeEstimates_SalesTaxCalculation_NoInsurance() {
			Prefs.UpdateDouble(PrefName.SalesTaxPercentage,10.00);
			string suffix="85";
			Patient pat=PatientT.CreatePatient(suffix);
			//procs - D7140 (extraction)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D7140",ProcStat.TP,"18",500);
			ProcedureCodeT.SetIsTaxed("D7140");
			//Update the preference for sales tax percent
			//Lists
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			Procedures.ComputeEstimates(proc1,pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
				histList,loopList,false,pat.Age,subList);
			//Check to see if deductible applied correctly
			Assert.AreEqual(50.00,proc1.TaxAmt);
			Prefs.UpdateDouble(PrefName.SalesTaxPercentage,0);
		}

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Procedures_ComputeEstimates_SalesTaxCalculation_EstimateWriteOff)]
		public void Procedures_ComputeEstimates_SalesTaxCalculation_EstimateWriteOff() {
			Prefs.UpdateDouble(PrefName.SalesTaxPercentage,10.00);
			string suffix="86";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsPlan planOld=plan.Copy();
			InsPlans.Update(plan,planOld);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.None,50.00);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.OralSurgery,80);
			//procs - D7140 (extraction)
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D7140",ProcStat.TP,"18",50);
			ProcedureCodeT.SetIsTaxed("D7140");
			//Lists
			ClaimProcT.CreateClaimProc(pat.PatNum,proc1.ProcNum,plan.PlanNum,sub.InsSubNum,cps:ClaimProcStatus.Estimate,writeOffEstOverride:5.00);
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			Procedures.ComputeEstimates(proc1,pat.PatNum,ref claimProcs,false,planList,patPlans,benefitList,
				histList,loopList,false,pat.Age,subList);
			Assert.AreEqual(4.50,proc1.TaxAmt);
			Prefs.UpdateDouble(PrefName.SalesTaxPercentage,0);
		}

		[TestMethod]
		public void Procedures_GetDiscountAmountForDiscountPlan_AnnualMaxDoesntExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:5,annualMax:15);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> {  };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
				Double test=Procedures.GetDiscountAmountForDiscountPlan(procHist[i],out _,out _);
			}
			DiscountPlanT.ClearDiscountPlanPrefs();
			Assert.AreEqual(true,procHist.All(x=>Procedures.GetDiscountAmountForDiscountPlan(x,out _,out _)==5));
		}

		[TestMethod]
		public void Procedures_GetDiscountAmountForDiscountPlan_AnnualMaxDoesExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:5,annualMax:11);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { proc3 };
			List<Procedure> procHistExceedsLimit=new List<Procedure> {  };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				string freqMsg, annualMaxMsg;
				double test=Procedures.GetDiscountAmountForDiscountPlan(procHist[i],out freqMsg,out annualMaxMsg);
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			DiscountPlanT.ClearDiscountPlanPrefs();
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count, procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
		}

		[TestMethod]
		public void Procedures_GetDiscountAmountForDiscountPlan_FrequencyLimitsDontExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:5,annualMax:11);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { proc3 };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc4 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				double test=Procedures.GetDiscountAmountForDiscountPlan(procHist[i],out _,out _);
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			DiscountPlanT.ClearDiscountPlanPrefs();
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count, procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
		}

		[TestMethod]
		public void Procedures_GetDiscountAmountForDiscountPlan_FrequencyLimitsExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:2,paFreqLimit:1,annualMax:25);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc3,proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				double test=Procedures.GetDiscountAmountForDiscountPlan(procHist[i],out _,out _);
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);	
			}
			DiscountPlanT.ClearDiscountPlanPrefs();
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count, procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
		}

		[TestMethod]
		public void Procedures_GetDiscountAmountForDiscountPlan_AnnualMaxDoesntExceedQuantity() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:5,annualMax:35);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			proc1.UnitQty=5;
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> {  };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				string freqMsg, annualMaxMsg;
				double test=Procedures.GetDiscountAmountForDiscountPlan(procHist[i],out freqMsg,out annualMaxMsg);
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			DiscountPlanT.ClearDiscountPlanPrefs();
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt>=5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count, procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
		}

		[TestMethod]
		public void Procedures_GetDiscountAmountForDiscountPlan_AnnualMaxDoesExceedQuantity() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:5,annualMax:30);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			proc1.UnitQty=5;
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc3 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				string freqMsg, annualMaxMsg;
				double test=Procedures.GetDiscountAmountForDiscountPlan(procHist[i],out freqMsg,out annualMaxMsg);
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			DiscountPlanT.ClearDiscountPlanPrefs();
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt>=5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count, procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
		}
	}
}

