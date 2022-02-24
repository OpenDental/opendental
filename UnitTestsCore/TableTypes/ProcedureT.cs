using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;
using System.Linq;

namespace UnitTestsCore {
	public class ProcedureT {
		///<summary>Returns the proc</summary>
		///<param name="procDate">If not included, will be set to DateTime.Now.</param>
		public static Procedure CreateProcedure(Patient pat,string procCodeStr,ProcStat procStatus,string toothNum,double procFee,
			DateTime procDate=default(DateTime),int priority=0,long plannedAptNum=0,long provNum=0,long aptNum=0,int baseUnits=0,string surf="",
			long procNumLab=0,bool doInsert=true, double discount=0)
		{
			Procedure proc=new Procedure();
			proc.CodeNum=ProcedureCodeT.CreateProcCode(procCodeStr).CodeNum;
			proc.PatNum=pat.PatNum;
			if(procDate==default(DateTime)) {
				proc.ProcDate=DateTime.Today;
			}
			else {
				proc.ProcDate=procDate;
			}
			proc.ProcStatus=procStatus;
			proc.ProvNum=provNum;
			if(provNum==0) {
				proc.ProvNum=pat.PriProv;
			}
			proc.ProcFee=procFee;
			proc.ProcNumLab=procNumLab;
			proc.ToothNum=toothNum;
			proc.Prosthesis="I";
			proc.Priority=Defs.GetDefsForCategory(DefCat.TxPriorities,true)[priority].DefNum;
			proc.AptNum=aptNum;
			proc.PlannedAptNum=plannedAptNum;
			proc.ClinicNum=pat.ClinicNum;
			proc.ToothRange="";
			proc.Surf=surf;
			proc.BaseUnits=baseUnits;
			proc.Discount=discount;
			if(doInsert) {
				Procedures.Insert(proc);
			}
			return proc;
		}

		///<summary>Deletes everything from the procedurelog table.</summary>
		public static void ClearProcedureTable() {
			string command="DELETE FROM procedurelog WHERE ProcNum > 0";
			DataCore.NonQ(command);
		}

		/*public static void SetToothNum(Procedure procedure,string toothNum){
			Procedure oldProcedure=procedure.Copy();
			procedure.ToothNum=toothNum;
			Procedures.Update(procedure,oldProcedure);
		}*/

		public static void SetPriority(Procedure procedure,int priority){
			Procedure oldProcedure=procedure.Copy();
			procedure.Priority=Defs.GetDefsForCategory(DefCat.TxPriorities,true)[priority].DefNum;
			Procedures.Update(procedure,oldProcedure);
		}

		public static void SetComplete(Procedure proc,Patient pat,InsuranceInfo insuranceInfo) {
			SetComplete(proc,pat,insuranceInfo.ListInsPlans,insuranceInfo.ListPatPlans,insuranceInfo.ListAllClaimProcs,insuranceInfo.ListBenefits,insuranceInfo.ListInsSubs);
		}

		public static void SetComplete(Procedure proc,Patient pat,List<InsPlan> planList,List<PatPlan> patPlanList,List<ClaimProc> claimProcList,List<Benefit> benefitList,List<InsSub> subList) {
			Procedure procOld=proc.Copy();
			ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
			proc.DateEntryC=DateTime.Now;
			proc.ProcStatus=ProcStat.C;
			Procedures.Update(proc,procOld);
			Procedures.ComputeEstimates(proc,proc.PatNum,claimProcList,false,planList,patPlanList,benefitList,pat.Age,subList);
		}

		///<summary>Computes the claimproc estimates for all TP procedures belonging to the specified patient.  Returns the claimprocs.</summary>
		public static List<ClaimProc> ComputeEstimates(Patient pat,InsuranceInfo insInfo) {
			return ComputeEstimates(pat,insInfo.ListPatPlans,insInfo.ListInsPlans,insInfo.ListInsSubs,insInfo.ListBenefits);
		}

		///<summary>Computes the claimproc estimates for all TP procedures belonging to the specified patient.  Returns the claimprocs.</summary>
		public static List<ClaimProc> ComputeEstimates(Patient pat,InsuranceInfo insInfo,DateTime dateCalc) {
			return ComputeEstimates(pat,insInfo.ListPatPlans,insInfo.ListInsPlans,insInfo.ListInsSubs,insInfo.ListBenefits,dateCalc);
		}
		
		///<summary>Computes the claimproc estimates for all TP procedures belonging to the specified patient.  Returns the claimprocs.</summary>
		public static List<ClaimProc> ComputeEstimates(Patient pat,List<PatPlan> listPatPlans,List<InsPlan> listPlans,List<InsSub> listSubs,
			List<Benefit> listBens) 
		{
			return ComputeEstimates(pat,listPatPlans,listPlans,listSubs,listBens,DateTime.Now);
		}

		///<summary>Computes the claimproc estimates for all TP procedures belonging to the specified patient.  Returns the claimprocs.</summary>
		public static List<ClaimProc> ComputeEstimates(Patient pat,List<PatPlan> listPatPlans,List<InsPlan> listPlans,List<InsSub> listSubs,
			List<Benefit> listBens,DateTime dateCalc) 
		{
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(pat.PatNum);
			List<ClaimProc> claimProcListOld=claimProcs.Select(x => x.Copy()).ToList();
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(pat.PatNum,listBens,listPatPlans,listPlans,dateCalc,listSubs);
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			List<Procedure> ProcList=Procedures.Refresh(pat.PatNum);
			Procedure[] ProcListTP=Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
			for(int i=0;i<ProcListTP.Length;i++) {
				Procedures.ComputeEstimates(ProcListTP[i],pat.PatNum,ref claimProcs,false,listPlans,listPatPlans,listBens,
					histList,loopList,false,pat.Age,listSubs);
				//then, add this information to loopList so that the next procedure is aware of it.
				loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref claimProcs,claimProcListOld);
			return ClaimProcs.Refresh(pat.PatNum);
		}

		public static void InsertMany(List<Procedure> listProcs) {
			OpenDentBusiness.Crud.ProcedureCrud.InsertMany(listProcs);
		}
	}
}
