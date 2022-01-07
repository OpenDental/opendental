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

namespace UnitTests.Claims_Tests {
	[TestClass]
	public class ClaimsTests:TestBase {
		
		///<summary>This test is for making sure that writeoffs are correct even when given a preauth estimate before a claim estimate.</summary>
		[TestMethod]
		public void Claims_CalculateAndUpdate_PreauthOrderWriteoff() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//create the patient and insurance information
			Patient pat=PatientT.CreatePatient(suffix);
			//proc - Crown
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.C,"8",1000);
			long feeSchedNum1=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			FeeT.CreateFee(feeSchedNum1,proc.CodeNum,900);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan insPlan=InsPlanT.CreateInsPlanPPO(carrier.CarrierNum,feeSchedNum1);
			BenefitT.CreateAnnualMax(insPlan.PlanNum,1000);
			BenefitT.CreateCategoryPercent(insPlan.PlanNum,EbenefitCategory.Crowns,100);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			PatPlan pp=PatPlanT.CreatePatPlan(1,pat.PatNum,sub.InsSubNum);
			//create lists and variables required for ComputeEstimates()
			List<InsSub> SubList=InsSubs.RefreshForFam(Patients.GetFamily(pat.PatNum));
			List<InsPlan> listInsPlan=InsPlans.RefreshForSubList(SubList);
			List<PatPlan> listPatPlan=PatPlans.Refresh(pat.PatNum);
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlan,SubList);
			List<Procedure> listProcsForPat=Procedures.Refresh(pat.PatNum);
			List<Procedure> procsForClaim=new List<Procedure>();
			procsForClaim.Add(proc);
			//Create the claim and associated claimprocs
			//The order of these claimprocs is the whole point of the unit test.
			//Create Preauth
			ClaimProcs.CreateEst(new ClaimProc(),proc,insPlan,sub,0,500,true,true);
			//Create Estimate 
			ClaimProcs.CreateEst(new ClaimProc(),proc,insPlan,sub,1000,1000,true,false);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Claim claimWaiting=ClaimT.CreateClaim("W",listPatPlan,listInsPlan,listClaimProcs,listProcsForPat,pat,procsForClaim,listBenefits,SubList,false);
			Assert.AreEqual(100,claimWaiting.WriteOff,"WriteOff Amount");
		}

		///<summary></summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Claims_CalculateAndUpdate_Allowed1Allowed2CompletedProcedures)]
		[Documentation.VersionAdded("7.2")]
		[TestMethod]
		[Documentation.Description(@"Patient has two insurance plans, both PPO, subscriber self for both. The only benefits entered for both insurance plans are $1000 max and 50% coverage on the crowns category.  One procedure is treatment planned, a D2750 PFM crown on #8. The three fee schedules are set up as below. The procedure is completed and a primary claim is created. The claim should show a writeoff of $500.       
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
            <td>$600.00</td>
            <td>$800.00</td>
            <td>$300.00</td>
            <td>$400.00</td>
            <td>$500.00</td>
            <td>$0.00</td>
            <td>$0.00</td>
          </tr>
        </table>
			")]
		public void Claims_CalculateAndUpdate_Allowed1Allowed2CompletedProcedures() {
			string suffix="8";
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
			fee.Amount=600;
			Fees.Insert(fee);
			fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum2;
			fee.Amount=800;
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
			BenefitT.CreateAnnualMax(planNum1,1000);
			BenefitT.CreateAnnualMax(planNum2,1000);
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
			List<Procedure> procList=Procedures.Refresh(patNum);
			//Set complete and attach to claim
			ProcedureT.SetComplete(proc,pat,planList,patPlans,claimProcs,benefitList,subList);
			claimProcs=ClaimProcs.Refresh(patNum);
			List<Procedure> procsForClaim=new List<Procedure>();
			procsForClaim.Add(proc);
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,procList,pat,procsForClaim,benefitList,subList);
			//Validate
			Assert.AreEqual(500,claim.WriteOff);
		}

		///<summary>Downgrade insurance estimates #1. The PPO fee schedule has a blank fee for the downgraded code.</summary>
		[TestMethod]
		[Documentation.Description("Downgrade insurance estimates #1. The PPO fee schedule has a blank fee for the downgraded code.")]
		[Documentation.Numbering(Documentation.EnumTestNum.Claims_CalculateAndUpdate_ProcedureCodeDowngradeBlankFee)]
		public void Claims_CalculateAndUpdate_ProcedureCodeDowngradeBlankFee() {
			string suffix="60";
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR Fees"+suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO Downgrades"+suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Restorative,100);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2393");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2160");
			originalProcCode.SubstitutionCode="D2160";
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodes.Update(originalProcCode);
			FeeT.CreateFee(ucrFeeSchedNum,originalProcCode.CodeNum,300);
			FeeT.CreateFee(ucrFeeSchedNum,downgradeProcCode.CodeNum,100);
			FeeT.CreateFee(ppoFeeSchedNum,originalProcCode.CodeNum,120);
			//No fee entered for D2160 in PPO Downgrades
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2393",ProcStat.C,"1",300);//Tooth 1
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			InsPlan insPlan = planList[0];//Should only be one
			InsPlan planOld = insPlan.Copy();//Should only be one
			insPlan.PlanType="p";
			insPlan.FeeSched=ppoFeeSchedNum;
			InsPlans.Update(insPlan,planOld);
			//Creates the claim in the same manner as the account module, including estimates.
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,ProcList,benefitList,subList);
			ClaimProc clProc=ClaimProcs.Refresh(pat.PatNum)[0];//Should only be one
			Assert.AreEqual(120,clProc.InsEstTotal);
			Assert.AreEqual(180,clProc.WriteOff);
		}

		///<summary>Downgrade insurance estimates #2. The PPO fee schedule has a higher fee for the downgraded code than for the original code.</summary>
		[TestMethod]
		[Documentation.Description("Downgrade insurance estimates #2. The PPO fee schedule has a higher fee for the downgraded code than for the original code.")]
		[Documentation.Numbering(Documentation.EnumTestNum.Claims_CalculateAndUpdate_ProcedureCodeDowngradeHigherFee)]
		public void Claims_CalculateAndUpdate_ProcedureCodeDowngradeHigherFee() {
			string suffix="61";
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR Fees"+suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO Downgrades"+suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Restorative,100);
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2391");
			ProcedureCode downgradeProcCode=ProcedureCodeT.CreateProcCode("D2140");
			originalProcCode.SubstitutionCode="D2140";
			originalProcCode.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodes.Update(originalProcCode);
			FeeT.CreateFee(ucrFeeSchedNum,originalProcCode.CodeNum,140);
			FeeT.CreateFee(ucrFeeSchedNum,downgradeProcCode.CodeNum,120);
			FeeT.CreateFee(ppoFeeSchedNum,originalProcCode.CodeNum,80);
			FeeT.CreateFee(ppoFeeSchedNum,downgradeProcCode.CodeNum,100);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2391",ProcStat.C,"1",140);//Tooth 1
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			InsPlan insPlan=planList[0];//Should only be one
			InsPlan planOld = insPlan.Copy();
			insPlan.PlanType="p";
			insPlan.FeeSched=ppoFeeSchedNum;
			InsPlans.Update(insPlan,planOld);
			//Creates the claim in the same manner as the account module, including estimates.
			Claim claim=ClaimT.CreateClaim("P",patPlans,planList,claimProcs,ProcList,pat,ProcList,benefitList,subList);
			ClaimProc clProc=ClaimProcs.Refresh(pat.PatNum)[0];//Should only be one
			Assert.AreEqual(80,clProc.InsEstTotal);
			Assert.AreEqual(60,clProc.WriteOff);
		}

		///<summary></summary>
		[TestMethod]
		public void Claims_ValidatePOBoxAddress() {
			List<string> listValidPOBox=new List<string> {
				"PO Box 12345",
				"P O box 12345",
				"P. O. Box 12345",
				"P.O.Box 12345",
				"post box 12345",
				"post office box 12345",
				"P.O.B 12345",
				"P.O.B. 12345",
				"Post Box #12345",
				"Postal Box 12345",
				"P.O. Box 12345",
				"PO. Box 12345",
				"P.o box 12345",
				"Pobox 12345",
				"p.o. Box12345",
				"po-box12345",
				"p.o.-box 12345",
				"PO-Box 12345",
				"p-o-box 12345",
				"p-o box 12345",
				"box 12345",
				"Box12345",
				"Box-12345"
			};
			List<string> listInValidPOBox=new List<string> {
				"12345 Tapo Cannon Rd.",
				"12345 Tapo 1st Ave",
				"12345 Box Turtle Circle",
				"12345 Boxing Pass",
				"12345 Poblano Lane",
				"12345 P O Davis Drive",
				"12345 P O Boxing Drive",
				"12345 PO Boxing Drive",
				"12345 Postal Circle"
			};
			foreach(string address in listValidPOBox) {
				if(!X837_5010.HasPOBox(address)) {
					Assert.Fail(address);
				}
			}
			foreach(string address in listInValidPOBox) {
				if(X837_5010.HasPOBox(address)) {
					Assert.Fail(address);
				}
			}
		}

		#region Claim OutstandingChargesGrid
		///<summary>Test to ensure that query changes to GetOutstandingClaims filters out Income Transfers.</summary>
		[TestMethod]
		public void Claims_GetOutstandingClaims_NoTransfers() {
			//Set up Account
			long provNum=ProviderT.CreateProvider("DOC");
			Clinic clinic=ClinicT.CreateClinic("NCC1701D");
			Carrier carrier=CarrierT.CreateCarrier("Federation");
			Patient pat=PatientT.CreatePatient(fName:"Jean Luc", lName:"Picard",clinicNum:clinic.ClinicNum,priProvNum:provNum);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			//Set Completed Proc
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1111",ProcStat.C,"",200,provNum:provNum);
			insInfo.ListAllProcs.Add(proc);
			//Add Proc to Benefits
			insInfo.AddBenefit(BenefitT.CreatePercentForProc(insInfo.PriPatPlan.PatPlanNum,proc.CodeNum,50));
			//Create a Claim for Proc
			Claim claim=ClaimT.CreateClaim(new List<Procedure>{proc},insInfo);
			//Enter Payment using AsTotal & Receive Claim
			ClaimProcT.AddInsPaidAsTotal(pat.PatNum,insInfo.PriInsPlan.PlanNum,provNum,100,insInfo.PriInsSub.InsSubNum,0,0,claim.ClaimNum);
				insInfo.ListAllClaimProcs=ClaimProcs.Refresh(insInfo.Pat.PatNum);
			Procedures.ComputeEstimatesForAll(pat.PatNum,insInfo.ListAllClaimProcs,insInfo.ListAllProcs,insInfo.ListInsPlans,insInfo.ListPatPlans,insInfo.ListBenefits,pat.Age,
				insInfo.ListInsSubs);
			//Create Income Transfer [Referenced from PaymentsTests]
			Payment txfrPayment=PaymentT.MakePaymentNoSplits(pat.PatNum,50,clinicNum:clinic.ClinicNum);
			PaymentT.BalanceAndIncomeTransfer(pat.PatNum,Patients.GetFamily(pat.PatNum),txfrPayment);
			Assert.AreEqual(4,ClaimProcs.RefreshForClaim(claim.ClaimNum).Count);//Two Supplementals [Income Transfer], 1 Received, 1 Not Received [Insurance]
			//It is important to be aware that all Income Transfers create two supplemental Claimprocs. This is because the first will be a positive 
			//value [$500], the second will be a negative value [-$500] so that the net result is always $0.
			List<ClaimPaySplit> listCPSplit=Claims.GetOutstandingClaims(carrier.CarrierName,DateTime.MinValue);
			Assert.AreEqual(1,listCPSplit.Count);//Should create a ClaimPaySplit when there are other ClaimProcs in addition to IsTransfer ClaimProcs
			//Remove the Non-Income-Transfer ClaimProcs so the resulting Claim only has Income Transfers on it.  This shouldn't normally be possible, but a
			//bug allowed this to happen at one point.
			foreach(ClaimProc cp in ClaimProcs.RefreshForClaim(claim.ClaimNum)){
				if(!cp.IsTransfer){
					ClaimProcs.Delete(cp);
				}
			}
			Assert.AreEqual(2,ClaimProcs.RefreshForClaim(claim.ClaimNum).Count);//Only the Two Supplementals [Income Transfers]
			listCPSplit=Claims.GetOutstandingClaims(carrier.CarrierName,DateTime.MinValue);
			Assert.AreEqual(0,listCPSplit.Count);//Should not create a ClaimPaySplit when there is only IsTransfer ClaimProcs
		}
		#endregion

		[TestMethod]
		public void Claim_DropInsurancePlan_DeleteClaim_ReceivedClaimProcs_ZeroInsPayNonZeroWriteOff() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			long ucrFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"UCR Fees"+suffix);
			long ppoFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPO Downgrades"+suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			ProcedureCode originalProcCode=ProcedureCodeT.CreateProcCode("D2391");
			ProcedureCodes.Update(originalProcCode);
			FeeT.CreateFee(ucrFeeSchedNum,originalProcCode.CodeNum,140);
			FeeT.CreateFee(ppoFeeSchedNum,originalProcCode.CodeNum,80);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D2391",ProcStat.C,"1",140);//Tooth 1
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(fam);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			InsPlan insPlan=listInsPlans[0];//Should only be one
			InsPlan planOld = insPlan.Copy();
			insPlan.PlanType="p";
			insPlan.FeeSched=ppoFeeSchedNum;
			InsPlans.Update(insPlan,planOld);
			//Creates the claim in the same manner as the account module, including estimates.
			Claim claim=ClaimT.CreateClaim("P",listPatPlans,listInsPlans,listClaimProcs,listProcs,pat,listProcs,listBenefits,listInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(1,listClaimProcs.Count);
			ClaimT.ReceiveClaim(claim,listClaimProcs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);//Get the updated claimprocs for this patient. Count should still be 1
			Assert.AreEqual(1,listClaimProcs.Count);
			//Set the insPayAmt to $0
			listClaimProcs[0].InsPayAmt=0;
			//Set the WriteOff to $100(a value greater than $0)
			listClaimProcs[0].WriteOff=100;
			//Update claimproc. This should still be marked at received.
			ClaimProcs.Update(listClaimProcs[0]);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);//Get the updated claimprocs for this patient. Count should still be 1
			//Delete patplan
			PatPlans.Delete(listPatPlans[0].PatPlanNum);//Estimates recomputed within Delete()
			//Now delete claim. This will deletes claim in the same manner as FormClaimEdit.cs in the DeleteClaimHelper(...)
			//One thing to note is since the PatPlan was deleted, we do not re-calcualte claimprocs so we leave the claimprocs as they were.
			ClaimProcs.DeleteEstimatesForDroppedPatPlan(listClaimProcs);
			Claims.Delete(claim,null);
			//The claim should be deleted
			Assert.IsNull(Claims.GetClaim(claim.ClaimNum));
			//The one claimproc should be deleted
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.Count);
		}
	}
}
