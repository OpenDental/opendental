using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using Health.Direct.Common.Extensions;

namespace OpenDentBusiness {
	///<summary>Use this to link Procedures to OrthoCases when setting Procedures complete or updating OrthoCase data to match changes in a linked Procedure.
	///Also links Procedures to dynamic payment plans if the plan is linked to the patient's active OrthoCase. Procedures, OrthoCases, and PayPlanLinks and other
	///associated data may be modified during these processes.///</summary>
    public class OrthoCaseProcedureLinker {
		///<summary>Key used to search for patient's linker when working with collections of them.</summary>
		public long PatNum=0;
		///<summary>Patients active OrthoCase. Null if patient doesn't have one.</summary>
		public OrthoCase ActiveOrthoCase=null;
		///<summary>All OrthoProcLinks for the active OrthoCase. Empty if they don't have an active OrthoCase.</summary>
		public List<OrthoProcLink> ListOrthoProcLinks=new List<OrthoProcLink>();
		///<summary>Active OrthoCase's OrthoSchedule. Null if they don't have an active OrthoCase.</summary>
		public OrthoSchedule OrthoSchedule=null;
		///<summary>Banding Procedure for the active OrthoCase. Null if they don't have an active OrthoCase or active OrthoCase is a transfer.</summary>
		private Procedure _bandingProcedure=null;
		///<summary>Dynamic payment plan linked to the active OrthoCase. Null if they don't have an active OrthoCase or a pay plan isn't linked.</summary>
		private PayPlan _linkedPayPlan=null;
		///<summary>All credits for patient's Procedures on patient payment plans. Left empty if a pay plan isn't linked to the active OrthoCase.</summary>
		private List<PayPlanCharge> _listAllProcPayPlanCreditsForPat=new List<PayPlanCharge>();
		///<summary>All links for patient's Procedures to dynamic payment plans. Left empty if a pay plan isn't linked to the active OrthoCase.</summary>
		private List<PayPlanLink> _listAllProcPayPlanLinksForPat=new List<PayPlanLink>();
		///<summary>Link between the active OrthoCase and its OrthoSchedule.</summary>
		private OrthoPlanLink _orthoSchedulePlanLink=null;

		///<summary>Not intended for use. Serialization only. Use one of the static create methods instead.</summary>
		public OrthoCaseProcedureLinker() {	}	

		///<summary>Pass this an old Procedure and an updated one. Links the Procedure to an OrthoCase if needed. Updates dates for the OrthoCase
		///if the Procedure.ProcDate has changed for Procedures already linked. Returns the OrthoProcLink.<summary>
		public static OrthoProcLink CreateOrUpdateOrthoProcLink(Procedure procedureOld,Procedure procedure) {
			OrthoProcLink orthoProcLink=OrthoProcLinks.GetByProcNum(procedure.ProcNum);
			if(procedureOld.ProcStatus!=ProcStat.C && procedure.ProcStatus==ProcStat.C) {
				OrthoCaseProcedureLinker orthoCaseProcedureLinker=CreateOneForPatient(procedure.PatNum);
				orthoProcLink=orthoCaseProcedureLinker.LinkProcedureToActiveOrthoCaseIfNeeded(procedure);
			}
			else if(orthoProcLink!=null && procedureOld.ProcDate!=procedure.ProcDate) {
				OrthoCases.UpdateDatesByLinkedProc(orthoProcLink,procedure);
			}
			return orthoProcLink;
		}

		///<summary>Creates an OrthoCaseProcedureLinker for a single patient.</summary>
		public static OrthoCaseProcedureLinker CreateOneForPatient(long patNum) {
			List<OrthoCaseProcedureLinker> listOrthoCaseProcedureLinkers=CreateManyForPatients(new List<long> { patNum });
			//Creation methods below ensure that list is never null or empty. A patient can only have one active OrthoCase,
			//so the list will only contain that active OrthoCase or a blank OrthoCaseProcedureLinker.
			return listOrthoCaseProcedureLinkers[0];
		}

		///<summary>Creates many OrthoCaseProcedureLinkers for a multiple patients' active OrthoCases.
		///Blank OrthoCaseProcedureLinkers will be inserted for patients that don't have an active OrthoCase.
		///If the OrthoCase feature is off, Blanks are inserted for all patients.</summary>
		public static List<OrthoCaseProcedureLinker> CreateManyForPatients(List<long> listPatNums) {
			List<OrthoCaseProcedureLinker> listOrthoCaseProcedureLinkers=new List<OrthoCaseProcedureLinker>();
			if(!OrthoCases.HasOrthoCasesEnabled()) {
				return CreateBlankLinkersForPatients(listPatNums);
			}
			//Get data needed for all patients to avoid querying in loops.
			List<OrthoCase> listActiveOrthoCases=OrthoCases.GetActiveForPats(listPatNums);
			if(listActiveOrthoCases.IsNullOrEmpty()) {
				return CreateBlankLinkersForPatients(listPatNums);
			}
			//We don't need any other data for patients that don't have active OrthoCases.
			//Use this list of PatNums for further querying to filter on fewer PatNums.
			List<long> listPatNumsForActiveOrthoCases=listActiveOrthoCases.Select(x => x.PatNum).ToList();
			List<long> listOrthoCaseNums=listActiveOrthoCases.Select(x => x.OrthoCaseNum).ToList();
			List<OrthoProcLink> listOrthoProcLinks=OrthoProcLinks.GetManyByOrthoCases(listOrthoCaseNums);
			List<long> listBandingProcNums=listOrthoProcLinks
				.Where(x => x.ProcLinkType==OrthoProcType.Banding)
				.Select(x => x.ProcNum)
				.ToList();
			List<Procedure> listBandingProcedures=Procedures.GetManyProc(listBandingProcNums,false);
			List<OrthoPlanLink> listOrthoPlanLinks=OrthoPlanLinks.GetManyForOrthoCases(listOrthoCaseNums);
			List<long> listOrthoScheduleNums=listOrthoScheduleNums=listOrthoPlanLinks
				.Where(x => x.LinkType==OrthoPlanLinkType.OrthoSchedule)
				.Select(x => x.FKey)
				.ToList();
			List<OrthoSchedule> listOrthoSchedules=OrthoSchedules.GetMany(listOrthoScheduleNums);
			List<PayPlan> listPayPlans=PayPlans.GetAllPatPayPlansForPats(listPatNumsForActiveOrthoCases);
			List<long> listPayPlanNums=listPayPlans.Select(x => x.PayPlanNum).ToList();
			List<PayPlanLink> listProcPayPlanLinks=PayPlanLinks.GetForPayPlansAndLinkType(listPayPlanNums,PayPlanLinkType.Procedure);
			List<PayPlanCharge> listProcPayPlanCredits=PayPlanCharges.GetAllProcCreditsForPayPlans(listPayPlanNums);
			//Create an OrthoCaseProcedureLinker for each patient.
			//Full list of PatNums is used because we need to create blanks for patients without an active OrthoCase.
			for(int i=0;i<listPatNums.Count;i++) {
				long patNum=listPatNums[i];
				OrthoCaseProcedureLinker OrthoCaseProcedureLinker=CreateOneForPatient(patNum,listActiveOrthoCases,listOrthoProcLinks,listOrthoPlanLinks,
					listBandingProcedures,listOrthoSchedules,listPayPlans,listProcPayPlanLinks,listProcPayPlanCredits);
				listOrthoCaseProcedureLinkers.Add(OrthoCaseProcedureLinker);
			}
			return listOrthoCaseProcedureLinkers;
		}

		///<summary>Returns true if the OrthoCase feature is on, the patient has an active OrthoCase,
		///and the active OrthoCase and Procedure are in valid states for linking.</summary>
		public bool ShouldProcedureLinkToOrthoCase(Procedure procedure,string procCode=null) {
			if(!OrthoCases.HasOrthoCasesEnabled()) {
				return false;//Won't link if the OrthoCase feature is disabled.
			}
			if(ActiveOrthoCase==null) {
				return false;//Won't link if patient has no active OrthoCase.
			}
			if(ListOrthoProcLinks.Any(x => x.ProcLinkType==OrthoProcType.Debond)) {
				return false;//Won't link if active OrthoCase has a completed Debond. Indicates case is closed.
			}
			if(procedure.ProcStatus!=ProcStat.C) {
				return false;//Won't link incomplete Procedures.
			}
			if(procCode.IsNullOrEmpty()) {
				procCode=ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode;
			}
			if(!Procedures.IsAnOrthoCaseProcCode(procCode)) {
				return false;//Won't link Procedures that don't have an OrthoCase Procedure code.
			}
			bool doesCaseHaveBanding=(_bandingProcedure!=null);
			bool doesCaseHaveIncompleteBanding=(doesCaseHaveBanding && _bandingProcedure.ProcStatus!=ProcStat.C);
			bool isProcedureCompletedBanding=(doesCaseHaveBanding && procedure.ProcNum==_bandingProcedure.ProcNum);
			if(doesCaseHaveIncompleteBanding && !isProcedureCompletedBanding) {
				return false;//If OrthoCase has an incomplete banding Procedure linked, other procs can't be linked until it is completed.
			}
			bool doesProcedureHaveBandingCode=OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoBandingCodes).Contains(procCode);
			bool isProcedureWrongBanding=(doesCaseHaveBanding && !isProcedureCompletedBanding && doesProcedureHaveBandingCode);
			if(isProcedureWrongBanding) {
				return false;//Can't have more than one banding Procedure linked to case.
			}
			if(ActiveOrthoCase.IsTransfer && doesProcedureHaveBandingCode) {
				return false;//Can't link bandings to transfer cases.
			}
			List<long> listVisitProcNums=ListOrthoProcLinks
				.Where(x => x.ProcLinkType==OrthoProcType.Visit)
				.Select(x => x.ProcNum)
				.ToList();
			if(listVisitProcNums.Contains(procedure.ProcNum)) {
				return false;//Can't link the same visit more than once
			}
			return true;//Procedure and OrthoCase are eligible for linking.
		}

		///<summary>Links Procedure to patient's active OrthoCase if validation passes.
		///Banding:	Procedure is already linked, we just update the OrthoCase.BandingDate.
		///Visit:	Insert a link between Procedure and OrthoCase.
		///Debond:	Insert a link between Procedure and OrthoCase, update OrthoCase.DebondDate, and set OrthoCase inactive.
		///For all OrthoCase procedure types, the Procedure.ProcFee will be set based on predetermined values in the OrthoSchedule.
		///If the active OrthoCase has a linked payment plan and validation passes, we will link the procedure to it.
		///Procedure changes will be updated in DB if parameter flag is set.</summary>
		public OrthoProcLink LinkProcedureToActiveOrthoCaseIfNeeded(Procedure procedure,bool doUpdateProcedure=false) {
			//No remoting role check; no call to db
			string procCode=ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode;
			if(!ShouldProcedureLinkToOrthoCase(procedure,procCode)) {
				return null;
			}
			OrthoProcLink orthoProcLink=null;
			Procedure procedureOld=procedure.Copy();
			OrthoCase orthoCaseOld=ActiveOrthoCase.Copy();
			//If Procedure being set complete is the banding, it is already linked. We just need to update the BandingDate.
			if(_bandingProcedure!=null && _bandingProcedure.ProcNum==procedure.ProcNum) {
				ActiveOrthoCase.BandingDate=procedure.ProcDate;
				OrthoCases.Update(ActiveOrthoCase,orthoCaseOld);
				orthoProcLink=ListOrthoProcLinks.FirstOrDefault(x => x.ProcNum==procedure.ProcNum);
			}
			//Link visit Procedure
			else if(OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoVisitCodes).Contains(procCode)) {
				orthoProcLink=new OrthoProcLink {
					OrthoCaseNum=ActiveOrthoCase.OrthoCaseNum,
					ProcNum=procedure.ProcNum,
					ProcLinkType=OrthoProcType.Visit,
					SecUserNumEntry=Security.CurUser.UserNum
				};
				OrthoProcLinks.Insert(orthoProcLink);
				ListOrthoProcLinks.Add(orthoProcLink);
			}
			//Link Debond Procedure
			else if(OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoDebondCodes).Contains(procCode)) {
				orthoProcLink=new OrthoProcLink {
					OrthoCaseNum=ActiveOrthoCase.OrthoCaseNum,
					ProcNum=procedure.ProcNum,
					ProcLinkType=OrthoProcType.Debond,
					SecUserNumEntry=Security.CurUser.UserNum
				};
				OrthoProcLinks.Insert(orthoProcLink);
				ListOrthoProcLinks.Add(orthoProcLink);
				ActiveOrthoCase.DebondDate=procedure.ProcDate;
				//Completing debond closes OrthoCase. This will also save our updated DebondDate
				OrthoCases.SetActiveState(ActiveOrthoCase,_orthoSchedulePlanLink,OrthoSchedule,isActive:false,orthoCaseOld);
			}
			else {
				return null;
			}
			SetProcFeeForLinkedProc(procedure);
			LinkOrthoCaseProcedureToPayPlanIfNeeded(procedure);
			if(doUpdateProcedure) {
				Procedures.Update(procedure,procedureOld,isProcLinkedToOrthoCase:true);
			}
			return orthoProcLink;
		}

		///<summary>This constructor is private because it should only be used by creation methods.</summary>
		private OrthoCaseProcedureLinker(OrthoCase activeOrthoCase,List<OrthoProcLink> listOrthoProcLinks,OrthoSchedule orthoSchedule,
			Procedure bandingProcedure,PayPlan linkedPayPlan,List<PayPlanCharge> listAllProcPayPlanCreditsForPat,
			List<PayPlanLink> listAllProcPayPLanLinksForPat,OrthoPlanLink orthoSchedulePlanLink)
		{
			PatNum=activeOrthoCase.PatNum;
			ActiveOrthoCase=activeOrthoCase;
			ListOrthoProcLinks=listOrthoProcLinks;
			OrthoSchedule=orthoSchedule;
			_bandingProcedure=bandingProcedure;
			_linkedPayPlan=linkedPayPlan;
			_listAllProcPayPlanCreditsForPat=listAllProcPayPlanCreditsForPat;
			_listAllProcPayPlanLinksForPat=listAllProcPayPLanLinksForPat;
			_orthoSchedulePlanLink=orthoSchedulePlanLink;
		}

		///<summary>Returns a list of OrthoCaseProcedureLinkers that only have their Patnums set.</summary>
		private static List<OrthoCaseProcedureLinker> CreateBlankLinkersForPatients(List<long> listPatNums) {
			List<OrthoCaseProcedureLinker> listOrthoCaseProcedureLinkers= new List<OrthoCaseProcedureLinker>();
			for(int i=0;i<listPatNums.Count();i++) {
				listOrthoCaseProcedureLinkers.Add(new OrthoCaseProcedureLinker { PatNum=listPatNums[i] });
			}
			return listOrthoCaseProcedureLinkers;
		}

		///<summary>The lists passed in may have data needed to make OrthoCaseProcedureLinkers for multiple patients.
		///This class is just a helper for pulling the data out of those lists to create one for a single patient.</summary>
		private static OrthoCaseProcedureLinker CreateOneForPatient(long patNum,List<OrthoCase> listActiveOrthoCases,List<OrthoProcLink> listOrthoProcLinks,
			List<OrthoPlanLink> listOrthoPlanLinks,List<Procedure> listBandingProcedures,List<OrthoSchedule> listOrthoSchedules,List<PayPlan> listPayPlans,
			List<PayPlanLink> listProcPayPlanLinks,List<PayPlanCharge> listProcPayPlanCharges)
		{
			OrthoCase activeOrthoCase=listActiveOrthoCases.FirstOrDefault(x => x.PatNum==patNum);
			if(activeOrthoCase==null) {
				return new OrthoCaseProcedureLinker() { PatNum=patNum };
			}
			OrthoPlanLink orthoScheduleLink=listOrthoPlanLinks.FirstOrDefault(x => x.LinkType==OrthoPlanLinkType.OrthoSchedule && x.OrthoCaseNum==activeOrthoCase.OrthoCaseNum);
			if(orthoScheduleLink==null) { 
				return new OrthoCaseProcedureLinker() { PatNum=patNum };
			}
			OrthoSchedule orthoSchedule=listOrthoSchedules.FirstOrDefault(x => x.OrthoScheduleNum==orthoScheduleLink.FKey);
			if(orthoSchedule==null) {
				return new OrthoCaseProcedureLinker() { PatNum=patNum };
			}
			List<OrthoProcLink> listOrthoProcLinksForCase=listOrthoProcLinks.FindAll(x => x.OrthoCaseNum==activeOrthoCase.OrthoCaseNum);
			List<long> listLinkedProcNums=listOrthoProcLinksForCase.Select(x => x.ProcNum).ToList();
			Procedure bandingProcedure=listBandingProcedures.FirstOrDefault(x => listLinkedProcNums.Contains(x.ProcNum));
			OrthoPlanLink orthoPayPlanLink=listOrthoPlanLinks.FirstOrDefault(x => x.LinkType==OrthoPlanLinkType.PatPayPlan && x.OrthoCaseNum==activeOrthoCase.OrthoCaseNum);
			PayPlan linkedPayPlan=null;
			List<PayPlan> listPayPlansForPat=new List<PayPlan>();
			List<PayPlanLink> listProcPayPlanLinksForPat=new List<PayPlanLink>();
			List<PayPlanCharge> listProcPayPlanChargesForPat=new List<PayPlanCharge>();
			if(orthoPayPlanLink!=null) {
				listPayPlansForPat=listPayPlans.FindAll(x => x.PatNum==patNum);
				linkedPayPlan=listPayPlansForPat.FirstOrDefault(x => x.PayPlanNum==orthoPayPlanLink.FKey);
			}
			if(linkedPayPlan!=null) {//If a payplan is not linked to the OrthoCase, we don't care about other payplan data. Skip to avoid needless searching.
				List<long> listPayPlanNumsForPat=listPayPlansForPat.Select(x => x.PayPlanNum).ToList();
				listProcPayPlanLinksForPat=listProcPayPlanLinks.FindAll(x => listPayPlanNumsForPat.Contains(x.PayPlanNum));
				listProcPayPlanChargesForPat=listProcPayPlanCharges.FindAll(x => listPayPlanNumsForPat.Contains(x.PayPlanNum));
			}
			return new OrthoCaseProcedureLinker(activeOrthoCase,listOrthoProcLinksForCase,orthoSchedule,bandingProcedure,
				linkedPayPlan,listProcPayPlanChargesForPat,listProcPayPlanLinksForPat,orthoScheduleLink);
		}

		///<summary>Does not update the Procedure. Sets the ProcFee for a procedure linked to the active OrthoCase based on its OrthoProcType.
		///If the procedure is the final visit planned, it may be adjusted so that the total OrthoCase visit fee is 
		///not exceeded by sum of linked visit procedure fees.</summary>
		private void SetProcFeeForLinkedProc(Procedure procedure) {
			//No remoting role check; no call to db
			double procFee=0;
			OrthoProcLink orthoProcLink=ListOrthoProcLinks.FirstOrDefault(x => x.ProcNum==procedure.ProcNum);
			switch(orthoProcLink.ProcLinkType) {
				case OrthoProcType.Banding:
					procFee=OrthoSchedule.BandingAmount;
					break;
				case OrthoProcType.Debond:
					procFee=OrthoSchedule.DebondAmount;
					break;
				case OrthoProcType.Visit:
					double allVisitsAmount=Math.Round((ActiveOrthoCase.Fee-OrthoSchedule.BandingAmount-OrthoSchedule.DebondAmount)*100)/100;
					int plannedVisitCount=OrthoSchedules.CalculatePlannedVisitsCount(OrthoSchedule.BandingAmount,OrthoSchedule.DebondAmount
						,OrthoSchedule.VisitAmount,ActiveOrthoCase.Fee);
					List<OrthoProcLink> listVisitProcLinks=ListOrthoProcLinks.FindAll(x => x.ProcLinkType==OrthoProcType.Visit);
					if(listVisitProcLinks.Count==plannedVisitCount) {
						procFee=Math.Round((allVisitsAmount-OrthoSchedule.VisitAmount*(plannedVisitCount-1))*100)/100;
					}
					else if(listVisitProcLinks.Count<plannedVisitCount) {
						procFee=OrthoSchedule.VisitAmount;
					}
					break;
			}
			procedure.ProcFee=procFee;
			procedure.BaseUnits=0;
			procedure.UnitQty=1;
		}

		///<summary>Returns true if the active OrthoCase has a linked pay plan, and the pay plan and procedure both meet conditions for linking.</summary>
		private bool ShouldProcedureLinkToPayPlan(Procedure procedure) {
			if(_linkedPayPlan==null || _linkedPayPlan.IsClosed || _linkedPayPlan.IsLocked) {
				return false;//Can't link Procedure if no pay plan is linked to the active OrthoCase, or the pay plan is closed or locked.
			}
			if(_listAllProcPayPlanLinksForPat.Select(x => x.FKey).Contains(procedure.ProcNum)) {
				return false;//Can't link Procedure if it is already linked to a dynamic pay plan.
			}
			if(_listAllProcPayPlanCreditsForPat.Select(x => x.ProcNum).Contains(procedure.ProcNum)) {
				return false;//Can't link Procedure if it is already credited on a patient payment plan.
			}
			return true;//Procedure is eligible for linking to the OrthoCase's pay plan.
		}

		///<summary>Links the Procedure to the active OrthoCase's linked pay plan if validation is passed.</summary>
		private void LinkOrthoCaseProcedureToPayPlanIfNeeded(Procedure procedure) {
			if(!ShouldProcedureLinkToPayPlan(procedure)) {
				return;
			}
			PayPlanLink payPlanLink=new PayPlanLink { 
				PayPlanNum=_linkedPayPlan.PayPlanNum,
				LinkType=PayPlanLinkType.Procedure,
				FKey=procedure.ProcNum,
			};
			PayPlanLinks.Insert(payPlanLink);
			_listAllProcPayPlanLinksForPat.Add(payPlanLink);
		}
	}
}
