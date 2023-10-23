using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class InsuranceT {
		public static InsuranceInfo AddInsurance(Patient pat,string carrierName,string planType="",long feeSchedNum=0,int ordinal=1,bool isMedical=false,
			EnumCobRule cobRule=EnumCobRule.Basic,long copayFeeSchedNum=0,int monthRenew=0,string subscriberID="1234",
			ExclusionRule exclusionRule=ExclusionRule.PracticeDefault) 
		{
			InsuranceInfo ins=new InsuranceInfo();
			ins.AddInsurance(pat,carrierName,planType,feeSchedNum,ordinal,isMedical,cobRule,copayFeeSchedNum,monthRenew,subscriberID,exclusionRule);
			return ins;
		}

	}

	public class InsuranceInfo {
		public List<PatPlan> ListPatPlans=new List<PatPlan>();
		public List<InsSub> ListInsSubs=new List<InsSub>();
		public List<InsPlan> ListInsPlans=new List<InsPlan>();
		public List<Carrier> ListCarriers=new List<Carrier>();
		public List<Benefit> ListBenefits=new List<Benefit>();
		public Patient Pat;
		public List<Procedure> ListAllProcs=new List<Procedure>();
		public List<ClaimProc> ListAllClaimProcs=new List<ClaimProc>();
		public List<SubstitutionLink> ListSubLinks=new List<SubstitutionLink>();
		public List<Claim> ListClaims=new List<Claim>();

		public PatPlan PriPatPlan {
			get {
				return ListPatPlans.FirstOrDefault(x => x.Ordinal==PatPlans.GetOrdinal(PriSecMed.Primary,ListPatPlans,ListInsPlans,ListInsSubs));
			}
		}

		public InsSub PriInsSub {
			get {
				long subNum=PatPlans.GetInsSubNum(ListPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,ListPatPlans,ListInsPlans,ListInsSubs));
				return InsSubs.GetSub(subNum,ListInsSubs);
			}
		}

		public InsPlan PriInsPlan {
			get {
				return InsPlans.GetPlan(PriInsSub.PlanNum,ListInsPlans);
			}
		}

		public PatPlan SecPatPlan {
			get {
				return ListPatPlans.FirstOrDefault(x => x.Ordinal==PatPlans.GetOrdinal(PriSecMed.Secondary,ListPatPlans,ListInsPlans,ListInsSubs));
			}
		}

		public InsSub SecInsSub {
			get {
				long subNum=PatPlans.GetInsSubNum(ListPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,ListPatPlans,ListInsPlans,ListInsSubs));
				return InsSubs.GetSub(subNum,ListInsSubs);
			}
		}

		public InsPlan SecInsPlan {
			get {
				return InsPlans.GetPlan(SecInsSub.PlanNum,ListInsPlans);
			}
		}

		public PatPlan MedPatPlan {
			get {
				return ListPatPlans.FirstOrDefault(x => x.Ordinal==PatPlans.GetOrdinal(PriSecMed.Medical,ListPatPlans,ListInsPlans,ListInsSubs));
			}
		}

		public InsSub MedInsSub {
			get {
				long subNum=PatPlans.GetInsSubNum(ListPatPlans,PatPlans.GetOrdinal(PriSecMed.Medical,ListPatPlans,ListInsPlans,ListInsSubs));
				return InsSubs.GetSub(subNum,ListInsSubs);
			}
		}

		public InsPlan MedInsPlan {
			get {
				return InsPlans.GetPlan(MedInsSub.PlanNum,ListInsPlans);
			}
		}

		public void AddInsurance(Patient pat,string carrierName,string planType="",long feeSchedNum=0,int ordinal=1,bool isMedical=false,
			EnumCobRule cobRule=EnumCobRule.Basic,long copayFeeSchedNum=0,int monthRenew=0,string subscriberID="1234",
			ExclusionRule exclusionRule=ExclusionRule.PracticeDefault) 
		{
			Carrier carrier=CarrierT.CreateCarrier(carrierName);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum,cobRule);
			InsPlan planOld=plan.Copy();
			plan.PlanType=planType;
			plan.MonthRenew=(byte)monthRenew;
			plan.FeeSched=feeSchedNum;
			plan.IsMedical=isMedical;
			plan.CopayFeeSched=copayFeeSchedNum;
			plan.ExclusionFeeRule=exclusionRule;
			InsPlans.Update(plan,planOld);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum,subscriberID);
			PatPlan patPlan=PatPlanT.CreatePatPlan((byte)ordinal,pat.PatNum,sub.InsSubNum);
			ListCarriers.Add(carrier);
			ListInsPlans.Add(plan);
			ListInsSubs.Add(sub);
			ListPatPlans.Add(patPlan);
			Pat=pat;
		}

		public void AddBenefit(Benefit benefit) {
			ListBenefits.Add(benefit);
		}

		public void AddPayAsTotal(double insPayAmt,long claimNum=0,long provNum=0,double dedApplied=0,double writeOff=0) {
			Claim claim;
			if(claimNum > 0) {
				claim=ListClaims.FirstOrDefault(x => x.ClaimNum==claimNum);
			}
			else {
				claim=ListClaims.FirstOrDefault();
			}
			if(claim==null) {
				return;
			}
			if(provNum==0) {
				provNum=Pat.PriProv;
			}
			ListAllClaimProcs.Add(ClaimProcT.AddInsPaidAsTotal(Pat.PatNum,claim.PlanNum,provNum,insPayAmt,claim.InsSubNum,dedApplied,writeOff,claim.ClaimNum));
		}

		public void RefreshBenefits() {
			ListBenefits=Benefits.Refresh(ListPatPlans,ListInsSubs);
		}

		///<summary>Computes the estimates for all of the procedures associated to ListAllProcs (saves to db).</summary>
		public void ComputeEstimates() {
			if(ListAllProcs.IsNullOrEmpty()) {
				return;
			}
			for(int i=0;i<ListAllProcs.Count;i++) {
				ComputeEstimatesForProc(ListAllProcs[i]);
			}
		}

		///<summary>Adds the procedure passed in to ListAllProcs if not already present and then computes the estimate for the procedure (saves to db).</summary>
		public void ComputeEstimatesForProc(Procedure proc) {
			ComputeEstimatesForProcs(new List<Procedure>() { proc });
		}

		///<summary>Adds the procedures passed in to ListAllProcs if not already present and then computes the estimate for each procedure (saves to db).</summary>
		public void ComputeEstimatesForProcs(List<Procedure> listProcedures) {
			ListAllProcs=ListAllProcs.Union(listProcedures).ToList();
			for(int i=0;i<listProcedures.Count;i++) {
				Procedures.ComputeEstimates(listProcedures[i],Pat.PatNum,ref ListAllClaimProcs,true,ListInsPlans,ListPatPlans,ListBenefits,new List<ClaimProcHist>(),
					new List<ClaimProcHist>(),true,Pat.Age,ListInsSubs);
			}
		}

		///<summary>Creates a claim for all of the procedures present.</summary>
		public Claim CreateClaim() {
			Claim claim=ClaimT.CreateClaim(ListAllProcs,this);
			ListClaims.Add(claim);
			return claim;
		}

		///<summary>Completes all procedures associated to claimprocs that are associated to known claims and then receives all of those claims.</summary>
		public void ReceiveClaims(bool doAddPayAmount=false) {
			for(int i=0;i<ListClaims.Count;i++) {
				List<ClaimProc> listClaimProcs=ListAllClaimProcs.FindAll(x => x.ClaimNum==ListClaims[i].ClaimNum);
				List<Procedure> listProcedures=ListAllProcs.FindAll(x => listClaimProcs.Select(y => y.ProcNum).Contains(x.ProcNum));
				for(int p=0;p<listProcedures.Count;p++) {
					ProcedureT.SetComplete(listProcedures[p],Pat,this);
				}
				ClaimT.ReceiveClaim(ListClaims[i],listClaimProcs,doSetInsPayAmt:doAddPayAmount);
			}
		}
	}
}
