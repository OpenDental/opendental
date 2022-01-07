using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;
using OpenDental;
using System.Linq;

namespace UnitTestsCore {
	public class ClaimT {
		/// <summary>claimType="P" (primary ins),"S" (secondary ins),"PreAuth","W" (waiting to send), "Med" (medical).</summary>
		public static Claim CreateClaim(string claimType,List<PatPlan> PatPlanList,List<InsPlan> InsPlanList,List<ClaimProc> ClaimProcList,
			List<Procedure> procsForPat,Patient pat,List<Procedure> procsForClaim,List<Benefit> benefitList,List<InsSub> SubList,
			bool calculateLineNumber=true,string claimIdentifier="")
		{
			//Claim ClaimCur=CreateClaim("P",PatPlanList,InsPlanList,ClaimProcList,procsForPat);
			InsPlan PlanCur1=new InsPlan();
			InsSub SubCur1=new InsSub();
			InsPlan PlanCur2=new InsPlan();
			InsSub SubCur2=new InsSub();
			switch(claimType) {
				case "P":
					SubCur1=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,1),SubList);
					PlanCur1=InsPlans.GetPlan(SubCur1.PlanNum,InsPlanList);
					SubCur2=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,2),SubList);
					//PlanCur2=InsPlans.GetPlan(SubCur.PlanNum,InsPlanList);//can end up null
					break;
				case "S":
					SubCur1=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,2),SubList);
					PlanCur1=InsPlans.GetPlan(SubCur1.PlanNum,InsPlanList);
					SubCur2=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,1),SubList);
					//PlanCur2=InsPlans.GetPlan(SubCur.PlanNum,InsPlanList);//can end up null
					break;
				case "PreAuth":
					SubCur1=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,1),SubList);
					PlanCur1=InsPlans.GetPlan(SubCur1.PlanNum,InsPlanList);
					SubCur2=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,2),SubList);
					break;
				case "W":
					SubCur1=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,1),SubList);
					PlanCur1=InsPlans.GetPlan(SubCur1.PlanNum,InsPlanList);
					SubCur2=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,2),SubList);
					break;
				case "Med":
					SubCur1=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Medical,PatPlanList,InsPlanList,SubList)),SubList);
					PlanCur1=InsPlans.GetPlan(SubCur1.PlanNum,InsPlanList);
					SubCur2=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,PatPlanList,InsPlanList,SubList)),SubList);
					break;
			}
			//DataTable table=DataSetMain.Tables["account"];
			Procedure proc;
			//proc=Procedures.GetProcFromList(procsForPat,PIn.Long(table.Rows[gridAccount.SelectedIndices[0]]["ProcNum"].ToString()));
			//long clinicNum=proc.ClinicNum;
			ClaimProc[] claimProcs=new ClaimProc[procsForClaim.Count];//1:1 with procs
			long procNum;
			for(int i=0;i<procsForClaim.Count;i++) {//loop through selected procs
				//and try to find an estimate that can be used
				procNum=procsForClaim[i].ProcNum;
				claimProcs[i]=Procedures.GetClaimProcEstimate(procNum,ClaimProcList,PlanCur1,SubCur1.InsSubNum);
			}
			for(int i=0;i<claimProcs.Length;i++) {//loop through each claimProc
				//and create any missing estimates. This handles claims to 3rd and 4th ins co's.
				if(claimProcs[i]==null) {
					claimProcs[i]=new ClaimProc();
					proc=procsForClaim[i];
					ClaimProcs.CreateEst(claimProcs[i],proc,PlanCur1,SubCur1);
				}
			}
			Claim claim=new Claim();
			Claims.Insert(claim);//to retreive a key for new Claim.ClaimNum
			claim.PatNum=pat.PatNum;
			claim.DateService=claimProcs[claimProcs.Length-1].ProcDate;
			claim.ClinicNum=procsForClaim[0].ClinicNum;
			claim.DateSent=DateTime.Today;
			claim.ClaimStatus="W";
			claim.AttachedFlags="Mail";
			if(!string.IsNullOrEmpty(claimIdentifier)) { 
				claim.ClaimIdentifier=claimIdentifier;
			}
			//datereceived
			switch(claimType) {
				case "P":
					claim.PlanNum=SubCur1.PlanNum;
					claim.InsSubNum=PatPlans.GetInsSubNum(PatPlanList,1);
					claim.PatRelat=PatPlans.GetRelat(PatPlanList,1);
					claim.ClaimType="P";
					claim.PlanNum2=SubCur2.PlanNum;//might be 0 if no sec ins
					claim.InsSubNum2=PatPlans.GetInsSubNum(PatPlanList,2);
					claim.PatRelat2=PatPlans.GetRelat(PatPlanList,2);
					break;
				case "S":
					claim.PlanNum=SubCur1.PlanNum;
					claim.InsSubNum=PatPlans.GetInsSubNum(PatPlanList,2);
					claim.PatRelat=PatPlans.GetRelat(PatPlanList,2);
					claim.ClaimType="S";
					claim.PlanNum2=SubCur2.PlanNum;
					claim.InsSubNum2=PatPlans.GetInsSubNum(PatPlanList,1);
					claim.PatRelat2=PatPlans.GetRelat(PatPlanList,1);
					break;
				case "W":
					claim.PlanNum=SubCur1.PlanNum;
					claim.InsSubNum=PatPlans.GetInsSubNum(PatPlanList,2);
					claim.PatRelat=PatPlans.GetRelat(PatPlanList,2);
					claim.ClaimType="P";
					claim.ClaimStatus="W";
					claim.PlanNum2=SubCur2.PlanNum;
					claim.InsSubNum2=PatPlans.GetInsSubNum(PatPlanList,1);
					claim.PatRelat2=PatPlans.GetRelat(PatPlanList,1);
					break;
				case "Med":
					claim.PlanNum=SubCur1.PlanNum;
					claim.InsSubNum=PatPlans.GetInsSubNum(PatPlanList,1);
					claim.PatRelat=PatPlans.GetRelat(PatPlanList,1);
					claim.ClaimType="Other";
					claim.PlanNum2=SubCur2.PlanNum;//might be 0 if no other ins
					claim.InsSubNum2=PatPlans.GetInsSubNum(PatPlanList,2);
					claim.PatRelat2=PatPlans.GetRelat(PatPlanList,2);
					break;
			}
			claim.ProvTreat=procsForClaim[0].ProvNum;
			claim.IsProsthesis="I";
			claim.ProvBill=Providers.GetBillingProvNum(claim.ProvTreat,claim.ClinicNum);
			claim.EmployRelated=YN.No;
			//attach procedures
			Procedure ProcCur;
			for(int i=0;i<claimProcs.Length;i++) {
				ProcCur=procsForClaim[i];
				claimProcs[i].ClaimNum=claim.ClaimNum;
				claimProcs[i].Status=ClaimProcStatus.NotReceived;//status for claims unsent or sent.
				//writeoff handled in ClaimL.CalculateAndUpdate()
				claimProcs[i].CodeSent=ProcedureCodes.GetProcCode(ProcCur.CodeNum).ProcCode;
				if(claimProcs[i].CodeSent.Length>5 && claimProcs[i].CodeSent.Substring(0,1)=="D") {
					claimProcs[i].CodeSent=claimProcs[i].CodeSent.Substring(0,5);
				}
				if(calculateLineNumber) {
					claimProcs[i].LineNumber=(byte)(i+1);
				}
				ClaimProcs.Update(claimProcs[i]);
			}
			ClaimProcList=ClaimProcs.Refresh(pat.PatNum);
			Claims.CalculateAndUpdate(procsForPat,InsPlanList,claim,PatPlanList,benefitList,pat,SubList);
			claim.DateSentOrig=claim.DateSent;
			claim.ClaimStatus="S";
			Claims.Update(claim);
			return claim;
		}

		public static Claim CreateClaim(List<Procedure> listProcsForClaim,InsuranceInfo insInfo,string claimType="P",string claimIdentifier="") {
			//In case the tester forgot to add these procs to the insInfo object:
			List<Procedure> listProcsAll=insInfo.ListAllProcs.Select(x => x.Copy()).ToList();
			listProcsAll.AddRange(listProcsForClaim.FindAll(x => !insInfo.ListAllProcs.Any(y => y.ProcNum==x.ProcNum)));
			return CreateClaim(claimType,insInfo.ListPatPlans,insInfo.ListInsPlans,insInfo.ListAllClaimProcs,listProcsAll,insInfo.Pat,listProcsForClaim,
				insInfo.ListBenefits,insInfo.ListInsSubs,claimIdentifier:claimIdentifier);
		}

		public static void ReceiveClaim(Claim claim,List<ClaimProc> listClaimProcs,bool doAddPayAmount=false) {
			claim.ClaimStatus="R";
			Claims.Update(claim);
			foreach(ClaimProc cp in listClaimProcs) {
				cp.Status=ClaimProcStatus.Received;
				if(doAddPayAmount) {
					cp.InsPayAmt=cp.InsPayEst;
				}
				ClaimProcs.Update(cp);
			}
		}

		///<summary>Deletes everything from the claim table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearClaimTable() {
			string command="DELETE FROM claim";
			DataCore.NonQ(command);
		}

	}
}
