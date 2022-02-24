using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoProcLinks{
		#region Get Methods
		///<summary>Gets all orthoproclinks from DB.</summary>
		public static List<OrthoProcLink> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT orthoproclink.* FROM orthoproclink";
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}

		///<summary>Get a list of all OrthoProcLinks for an OrthoCase.</summary>
		public static List<OrthoProcLink> GetManyByOrthoCase(long orthoCaseNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod(),orthoCaseNum);
			}
			string command="SELECT * FROM orthoproclink WHERE orthoproclink.OrthoCaseNum = "+POut.Long(orthoCaseNum);
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}

		///<summary>Gets one OrthoProcLink of the specified OrthoProcType for an OrthoCase. This should only be used to get procedures of the 
		///Banding or Debond types as only one of each can be linked to an Orthocase.</summary>
		public static OrthoProcLink GetByType(long orthoCaseNum,OrthoProcType linkType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OrthoProcLink>(MethodBase.GetCurrentMethod(),orthoCaseNum,linkType);
			}
			string command=$@"SELECT * FROM orthoproclink WHERE orthoproclink.OrthoCaseNum={POut.Long(orthoCaseNum)}
				AND orthoproclink.ProcLinkType={POut.Int((int)linkType)}";
			return Crud.OrthoProcLinkCrud.SelectOne(command);
		}

		///<summary>Returns a list of OrthoProcLinks of the specified type that are associated to any OrthoCaseNum from the list passed in.</summary>
		public static List<OrthoProcLink> GetManyByOrthoCases(List<long> listOrthoCaseNums) {
			if(listOrthoCaseNums.Count<=0) {
				return new List<OrthoProcLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod(),listOrthoCaseNums);
			}
			string command=$"SELECT * FROM orthoproclink WHERE orthoproclink.OrthoCaseNum IN({string.Join(",",listOrthoCaseNums)})";
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}

		///<summary>Gets all OrthoProcLinks associated to any procedures in the list passed in.</summary>
		public static List<OrthoProcLink> GetManyForProcs(List<long> listProcNums) {
			if(listProcNums.Count<=0) {
				return new List<OrthoProcLink>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			string command=$@"SELECT * FROM orthoproclink
				WHERE orthoproclink.ProcNum IN({string.Join(",",listProcNums)})";
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}

		///<summary>Returns a single OrthoProcLink for the procNum. There should only be one in db per procedure.</summary>
		public static OrthoProcLink GetByProcNum(long procNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OrthoProcLink>(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT * FROM orthoproclink WHERE ProcNum="+POut.Long(procNum);
			return Crud.OrthoProcLinkCrud.SelectOne(command);
		}

		///<summary>Returns a list of all ProcLinks of the visit type associated to an OrthoCase.</summary>
		public static List<OrthoProcLink> GetVisitLinksForOrthoCase(long orthoCaseNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod(),orthoCaseNum);
			}
			string command=$@"SELECT * FROM orthoproclink WHERE orthoproclink.OrthoCaseNum={POut.Long(orthoCaseNum)}
			AND orthoproclink.ProcLinkType={POut.Int((int)OrthoProcType.Visit)}";
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Insert
		///<summary>Inserts an OrthoProcLink into the database. Returns the OrthoProcLinkNum.</summary>
		public static long Insert(OrthoProcLink orthoProcLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				orthoProcLink.OrthoProcLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoProcLink);
				return orthoProcLink.OrthoProcLinkNum;
			}
			return Crud.OrthoProcLinkCrud.Insert(orthoProcLink);
		}

		///<summary>If procedure gets linked to ortho case the procedure gets updated to DB
		///and the proc link is returned. Otherwise, return null.</summary>
		public static OrthoProcLink TryLinkProcForActiveOrthoCaseAndUpdate(OrthoCaseProcLinkingData linkingData,Procedure proc) {
			//No remoting role check; no call to db
			Procedure procOld=proc.Copy();
			OrthoProcLink procLink=TryLinkProcForActiveOrthoCase(linkingData,proc);
			bool isProcLinkedToOrthoCase=(procLink!=null);
			if(isProcLinkedToOrthoCase) {
				Procedures.Update(proc,procOld,isProcLinkedToOrthoCase:isProcLinkedToOrthoCase);
			}
			return procLink;
		}

		///<summary>Links procedure to ortho case if conditions of linkingData.WillProcLinkToOrthoCase() are met. Will return the link for the
		///procedure or null if the procedure is ineligible for linking. Links procedure to ortho case's pay plan if one exists</summary>
		public static OrthoProcLink TryLinkProcForActiveOrthoCase(OrthoCaseProcLinkingData linkingData,Procedure proc) {
			//No remoting role check; no call to db
			if(!linkingData.CanProcLinkToOrthoCase(proc)) {
				return null;
			}
			OrthoProcType procType;
			OrthoProcLink procLink;
			string procCode=ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode;
			//If proc being set complete is the banding, it is already linked. We just need to set the fee and update the date.
			if(linkingData.BandingProc!=null && linkingData.BandingProc.ProcNum==proc.ProcNum) {
				procType=OrthoProcType.Banding;
				linkingData.ActiveOrthoCase.BandingDate=proc.ProcDate;
				OrthoCases.Update(linkingData.ActiveOrthoCase,linkingData.OrthoCaseOld);
				procLink=linkingData.BandingProcLink;
			}
			//Link visit procedure
			else if(OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoVisitCodes).Contains(procCode)) {
				procType=OrthoProcType.Visit;
				procLink=CreateHelper(linkingData.ActiveOrthoCase.OrthoCaseNum,proc.ProcNum,OrthoProcType.Visit);
				procLink.OrthoProcLinkNum=Insert(procLink);
				linkingData.ListVisitProcLinks.Add(procLink);
				linkingData.ListProcLinksForCase.Add(procLink);
			}
			else {//Link Debond procedure
				if(!OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoDebondCodes).Contains(procCode)) {
					return null;//just an extra precaution to make sure that we are infact a debond procedure. 
				}
				procType=OrthoProcType.Debond;
				procLink=CreateHelper(linkingData.ActiveOrthoCase.OrthoCaseNum,proc.ProcNum,OrthoProcType.Debond);
				procLink.OrthoProcLinkNum=Insert(procLink);
				linkingData.ListProcLinksForCase.Add(procLink);
				linkingData.ActiveOrthoCase.DebondDate=proc.ProcDate;
				//deactivate ortho case as it is considered complete once debond is complete. This will also send our updated debond date to the DB.
				OrthoCases.SetActiveState(linkingData.ActiveOrthoCase,linkingData.SchedulePlanLink,linkingData.OrthoSchedule,false,linkingData.OrthoCaseOld);
			}
			SetProcFeeForLinkedProc(linkingData.ActiveOrthoCase,proc,procType,linkingData.ListVisitProcLinks,linkingData.SchedulePlanLink
				,linkingData.OrthoSchedule);
			linkingData.NewOrUpdatedProcLink=procLink;
			PayPlanLinks.TryLinkOrthoCaseProcToPayPlan(linkingData,proc);
			return procLink;
		}
		#endregion Insert

		#region Update
		/// <summary>Update only data that is different in newOrthoProcLink</summary>
		public static void Update(OrthoProcLink newOrthoProcLink,OrthoProcLink oldOrthoProcLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),newOrthoProcLink,oldOrthoProcLink);
				return;
			}
			Crud.OrthoProcLinkCrud.Update(newOrthoProcLink,oldOrthoProcLink);
		}
		#endregion Update

		#region Delete
		///<summary>Delete an OrthoProcLink from the database.</summary>
		public static void Delete(long orthoProcLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoProcLinkNum);
				return;
			}
			Crud.OrthoProcLinkCrud.Delete(orthoProcLinkNum);
		}

		///<summary>Deletes all ProcLinks in the provided list of OrthoProcLinkNums.</summary>
		public static void DeleteMany(List<long> listOrthoProcLinkNums) {
			if(listOrthoProcLinkNums.Count<=0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listOrthoProcLinkNums);
				return;
			}
			string command=$"DELETE FROM orthoproclink WHERE OrthoProcLinkNum IN({string.Join(",",listOrthoProcLinkNums)})";
			Db.NonQ(command);
		}
		#endregion Delete

		#region Misc Methods
		///<summary>Does not insert it in the DB. Returns an OrthoProcLink of the specified type for the OrthoCaseNum and procNum passed in.</summary>
		public static OrthoProcLink CreateHelper(long orthoCaseNum,long procNum,OrthoProcType procType) {
			//No remoting role check; no call to db
			OrthoProcLink orthoProcLink=new OrthoProcLink();
			orthoProcLink.OrthoCaseNum=orthoCaseNum;
			orthoProcLink.ProcNum=procNum;
			orthoProcLink.ProcLinkType=procType;
			orthoProcLink.SecUserNumEntry=Security.CurUser.UserNum;
			return orthoProcLink;
		}

		///<summary>Determines whether the passed in procedure is a Banding, Visit, or Debond procedure and
		///sets the ProcFee accordingly. Does not update the procedure in the database.</summary>
		public static void SetProcFeeForLinkedProc(OrthoCase orthoCase,Procedure proc,OrthoProcType procType,List<OrthoProcLink> listVisitProcLinks,
			OrthoPlanLink scheduleOrthoPlanLink=null,OrthoSchedule orthoSchedule=null)
		{
			//No remoting role check; no call to db
			if(scheduleOrthoPlanLink==null && orthoSchedule==null) {
				scheduleOrthoPlanLink=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCase.OrthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			}
			if(orthoSchedule==null) {
				orthoSchedule=OrthoSchedules.GetOne(scheduleOrthoPlanLink.FKey);
			}
			double procFee=0;
			switch(procType) {
				case OrthoProcType.Banding:
					procFee=orthoSchedule.BandingAmount;
					break;
				case OrthoProcType.Debond:
					procFee=orthoSchedule.DebondAmount;
					break;
				case OrthoProcType.Visit:
					double allVisitsAmount=Math.Round((orthoCase.Fee-orthoSchedule.BandingAmount-orthoSchedule.DebondAmount)*100)/100;
					int plannedVisitCount=OrthoSchedules.CalculatePlannedVisitsCount(orthoSchedule.BandingAmount,orthoSchedule.DebondAmount
						,orthoSchedule.VisitAmount,orthoCase.Fee);
					if(listVisitProcLinks.Count==plannedVisitCount) {
						procFee=Math.Round((allVisitsAmount-orthoSchedule.VisitAmount*(plannedVisitCount-1))*100)/100;
					}
					else if(listVisitProcLinks.Count<plannedVisitCount) {
						procFee=orthoSchedule.VisitAmount;
					}
					break;
			}
			proc.ProcFee=procFee;
			proc.BaseUnits=0;
			proc.UnitQty=1;
		}

		///<summary>Returns true if the procNum is contained in at least one OrthoProcLink.</summary>
		public static bool IsProcLinked(long procNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT * FROM orthoproclink WHERE orthoproclink.ProcNum="+POut.Long(procNum);
			return Crud.OrthoProcLinkCrud.SelectMany(command).Count>0;
		}
		#endregion Misc Methods

		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		///<summary>Gets one OrthoProcLink from the db.</summary>
		public static OrthoProcLink GetOne(long orthoProcLinkNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<OrthoProcLink>(MethodBase.GetCurrentMethod(),orthoProcLinkNum);
			}
			return Crud.OrthoProcLinkCrud.SelectOne(orthoProcLinkNum);
		}
		///<summary></summary>
		public static void Update(OrthoProcLink orthoProcLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoProcLink);
				return;
			}
			Crud.OrthoProcLinkCrud.Update(orthoProcLink);
		}
		*/
	}

	///<summary>Helper class to contain all of the necessary data for proc linking.</summary>
	public class OrthoCaseProcLinkingData {
		#region Public Variables
		///<summary>The active ortho case for the patient we are currently working with.</summary>
		public OrthoCase ActiveOrthoCase;
		///<summary>A copy of ActiveOrthoCase before any changes are made.</summary>
		public OrthoCase OrthoCaseOld;
		///<summary>List of proclinks for ActiveOrthoCase.</summary>
		public List<OrthoProcLink> ListProcLinksForCase;
		///<summary>The OrthoPlanLink for the ActiveOrthoCase's ortho schedule.</summary>
		public OrthoPlanLink SchedulePlanLink;
		///<summary>The ortho schedule for the ActiveOrthoCase.</summary>
		public OrthoSchedule OrthoSchedule;
		///<summary>Banding procedure for the ActiveOrthoCase. Will be null if ortho case is a transfer.</summary>
		public Procedure BandingProc;
		///<summary>List of visit procedures in ListProcLinksForCase.</summary>
		public List<OrthoProcLink> ListVisitProcLinks;
		///<summary>OrthoProcLink for the Banding procedure of the ActiveOrthoCase. Will be null if ortho case is a transfer.</summary>
		public OrthoProcLink BandingProcLink;
		///<summary>Filled with the proc link that gets created or updated when OrthoProcLinks.LinkProcForActiveOrthoCase() is called.</summary>
		public OrthoProcLink NewOrUpdatedProcLink;
		///<summary>Key is FKey(ProcNum). Holds all pay plan links for procedures that belong to patients in _listPatNums</summary>
		public Dictionary<long,PayPlanLink> DictPayPlanProcLinks;
		///<summary>Pay plan that is linked to the ActiveOrthoCase. Will be null if none exists.</summary>
		public PayPlan LinkedPayPlan;
		#endregion Public Variables
		#region Private Variables
		///<summary>List of patnums for which we will get and hold active ortho case data in memory to avoid querying in loops.</summary>
		private List<long> _listPatNums;
		///<summary>The current patient who's active ortho case data we are working with.</summary>
		private long _patNumCur;
		///<summary>Key is PatNum (only one active ortho case per pat). Holds active ortho cases for multiple patients.</summary>
		private Dictionary<long,OrthoCase> _dictActiveOrthoCases=new Dictionary<long,OrthoCase>();
		///<summary>Key is OrthoCaseNum. Holds a list of ortho proc links for each ortho case in DictOrthoCases.</summary>
		private Dictionary<long,List<OrthoProcLink>> _dictOrthoProcLinkLists;
		///<summary>Key is OrthoCaseNum. Holds a ortho schedule for each ortho case in DictOrthoCases.</summary>
		private Dictionary<long,OrthoSchedule> _dictOrthoSchedules;
		///<summary>Key is FKey(OrthoScheduleNum). Holds a ortho schedule link for each ortho case in DictOrthoCases.</summary>
		private Dictionary<long,OrthoPlanLink> _dictOrthoSchedulePlanLinks;
		///<summary>Key is ProcNum. Holds all banding procs for ortho cases in DictOrthoCases.</summary>
		private Dictionary<long,Procedure> _dictBandingProcs;
		///<summary>Key is PayPlanNum. Holds all Pay Plans for patients in _listPatNums.</summary>
		private Dictionary<long,PayPlan> _dictPatPayPlans;
		///<summary>Key is OrthoCaseNum. Holds all pay plan links for ortho cases in _dictOrthoCases.</summary>
		private Dictionary<long,OrthoPlanLink> _dictPatPayPlanOrthoPlanLinks;
		///<summary>Key and value are both ProcNum.
		///Holds all distinct ProcNums for procedures credited on non-dynamic pay plans that belong to pats from _listPatNums</summary>
		private Dictionary<long,long> _dictProcNumsForCreditedProcs;
		///<summary>OrthoPlanLink between pay plan and ActiveOrthoCase. Will be null if none exists.</summary>
		private OrthoPlanLink _activeCasePatPayPlanLink;
		#endregion Private Variables

		/// <summary>Gets procedure linking data for a single patient to link procedures to active ortho cases (if exists).</summary>
		public OrthoCaseProcLinkingData(long patNum) {
			_listPatNums=new List<long>() {patNum};
			FillPatDataFromDb();
		}

		///<summary>Gets data for multiple patients and stores it in dictionaries to prevent querying in loops.</summary>
		public OrthoCaseProcLinkingData(List<long> listPatNums) {
			_listPatNums=listPatNums.Distinct().ToList();
			FillPatDataFromDb();
		}

		///<summary>Returns true if procedure is complete, OrthoCase feature is enabled, pat has an active ortho case,
		///no debond procedures are linked to ortho case, any linked banding procedures are complete, and procedure is an ortho case procedure.</summary>
		public bool CanProcLinkToOrthoCase(Procedure proc) {
			_patNumCur=proc.PatNum;
			ClearPatData();
			FillPatDataFromDicts();
			if(proc.ProcStatus!=ProcStat.C) {//Only completed procs link to ortho cases.
				return false;
			}
			if(!OrthoCases.HasOrthoCasesEnabled()) {//Proc won't link if the ortho case feature is disabled.
				return false;
			}
			if(ActiveOrthoCase==null) {//Proc will only link if patient has an active ortho case.
				return false;
			}
			//If active ortho case already has a debond procedure completed return false. Ortho case is considered completed. 
			if(ListProcLinksForCase.Any(x => x.ProcLinkType==OrthoProcType.Debond)) {
				return false;
			}
			//If ortho case has an incomplete banding procedure linked, other procs can't be linked until it is completed.
			if(BandingProc!=null && proc.ProcNum!=BandingProc.ProcNum && BandingProc.ProcStatus!=ProcStat.C) {
				return false;
			}
			//If case is transfer or we already have a banding procedure and we're attempting to link another banding procedure, do not continue.
			if(((BandingProc!=null && proc.ProcNum!=BandingProc.ProcNum) || ActiveOrthoCase.IsTransfer) 
				&& OrthoCases.GetListProcTypeProcCodes(PrefName.OrthoBandingCodes).Contains(ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode)) 
			{
				return false;
			}
			//If proc is already linked and is not a banding procedure we will not link it again. True will be returned for banding procs
			//that are already linked because we will need to update the fee and date for them when they are set complete.
			if(ListProcLinksForCase.Where(x => x.ProcLinkType!=OrthoProcType.Banding).Select(x => x.ProcNum).ToList().Contains(proc.ProcNum)) {
				return false;
			}
			//Proc won't link because it is not an ortho case procedure.
			if(!Procedures.HasAnOrthoCaseProcCode(proc)) {
				return false;
			}
			return true;//Proc is eligible for linking to the active ortho case.
		}

		///<summary>Return true if procedure is eligible for linking to pay plan that is linked to ortho case.</summary>
		public bool CanProcLinkToPayPlan(Procedure proc) {
			_patNumCur=proc.PatNum;
			ClearPatData();
			FillPatDataFromDicts();
			if(ActiveOrthoCase==null) {//Don't link proc if patient doesn't have an active ortho case.
				return false;
			}
			if(!OrthoCases.HasOrthoCasesEnabled()) {//Proc won't link if the ortho case feature is disabled.
				return false;
			}
			//Can't link proc if no pay plan is linked to the active ortho case, or the pay plan is closed or locked.
			if(LinkedPayPlan==null || LinkedPayPlan.IsClosed || LinkedPayPlan.IsLocked) {
				return false;
			}
			DictPayPlanProcLinks.TryGetValue(proc.ProcNum,out PayPlanLink payPlanLinkForProc);
			if(payPlanLinkForProc!=null) {//Can't link proc if it is already linked to a dynamic pay plan.
				return false;
			}
			_dictProcNumsForCreditedProcs.TryGetValue(proc.ProcNum,out long procNumOfCreditedProc);
			if(procNumOfCreditedProc!=0) {//Can't link proc if it is already credited on a patient payment plan.
				return false;
			}
			//Don't link proc if it is not complete or it is not linked to the patient's active ortho case.
			//FUTURE TO DO: Will need to change this so that TP'd banding procs can link when dynamic pay plans allow TP'd procedures.
			if(proc.ProcStatus!=ProcStat.C || !ListProcLinksForCase.Select(x => x.ProcNum).ToList().Contains(proc.ProcNum)) {
				return false;
			}
			return true;
		}

		///<summary>Fills member dictionaries with data needed for linking procedures to active ortho cases.</summary>
		private void FillPatDataFromDb() {
			//No remoting role check; no call to db
			if(!OrthoCases.HasOrthoCasesEnabled()) {
				return;
			}
			_dictActiveOrthoCases=OrthoCases.GetActiveForPats(_listPatNums).ToDictionary(x => x.PatNum,x => x);
			List<long>listOrthoCaseNums=_dictActiveOrthoCases.Values.Select(x => x.OrthoCaseNum).ToList();
			_dictOrthoProcLinkLists=
				OrthoProcLinks.GetManyByOrthoCases(listOrthoCaseNums).GroupBy(x => x.OrthoCaseNum).ToDictionary(x => x.Key,x => x.ToList());
			_dictOrthoSchedulePlanLinks=
				OrthoPlanLinks.GetAllForOrthoCasesByType(listOrthoCaseNums,OrthoPlanLinkType.OrthoSchedule).ToDictionary(x => x.FKey,x => x);
			_dictOrthoSchedules=OrthoSchedules.GetMany(_dictOrthoSchedulePlanLinks.Keys.ToList())
				.ToDictionary(x => _dictOrthoSchedulePlanLinks[x.OrthoScheduleNum].OrthoCaseNum,x => x);
			_dictBandingProcs=Procedures.GetManyProc(_dictOrthoProcLinkLists.Values
					.SelectMany(x => x)
					.Where(x => x.ProcLinkType==OrthoProcType.Banding)
					.Select(x => x.ProcNum).ToList()
					,false)
				.ToDictionary(x => x.ProcNum,x => x);
			_dictPatPayPlans=PayPlans.GetAllPatPayPlansForPats(_listPatNums).ToDictionary(x => x.PayPlanNum,x => x);
			_dictPatPayPlanOrthoPlanLinks=
				OrthoPlanLinks.GetAllForOrthoCasesByType(listOrthoCaseNums,OrthoPlanLinkType.PatPayPlan).ToDictionary(x => x.OrthoCaseNum,x => x);
			DictPayPlanProcLinks=
				PayPlanLinks.GetForPayPlansAndLinkType(_dictPatPayPlans.Keys.ToList(),PayPlanLinkType.Procedure).ToDictionary(x => x.FKey,x => x);
			_dictProcNumsForCreditedProcs=
				PayPlanCharges.GetAllProcCreditsForPats(_listPatNums).GroupBy(x => x.ProcNum).ToDictionary(x => x.Key,x => x.Key);
		}

		///<summary>Clears variables that hold data for a specific patient's active ortho case.</summary>
		private void ClearPatData() {
			//No remoting role check; no call to db
			NewOrUpdatedProcLink=null;
			ActiveOrthoCase=null;
			OrthoCaseOld=null;
			ListProcLinksForCase=null;
			SchedulePlanLink=null;
			OrthoSchedule=null;
			BandingProc=null;
			ListVisitProcLinks=null;
			BandingProcLink=null;
			_activeCasePatPayPlanLink=null;
			LinkedPayPlan=null;
		}

		///<summary>Sets variables related to a patient's active ortho case from member dictionaries to avoid querying in loops.</summary>
		private void FillPatDataFromDicts() {
			//No remoting role check; no call to db
			_dictActiveOrthoCases.TryGetValue(_patNumCur,out ActiveOrthoCase);
			if(ActiveOrthoCase!=null) {
				OrthoCaseOld=ActiveOrthoCase.Copy();
				_dictOrthoSchedules.TryGetValue(ActiveOrthoCase.OrthoCaseNum,out OrthoSchedule);
				_dictOrthoSchedulePlanLinks.TryGetValue(OrthoSchedule.OrthoScheduleNum,out SchedulePlanLink);
				if(!_dictOrthoProcLinkLists.TryGetValue(ActiveOrthoCase.OrthoCaseNum,out ListProcLinksForCase)) {
					ListProcLinksForCase=new List<OrthoProcLink>();
					_dictOrthoProcLinkLists.Add(ActiveOrthoCase.OrthoCaseNum,ListProcLinksForCase);
				}
				ListVisitProcLinks=ListProcLinksForCase.FindAll(x => x.ProcLinkType==OrthoProcType.Visit);
				BandingProcLink=ListProcLinksForCase.FirstOrDefault(x => x.ProcLinkType==OrthoProcType.Banding);
				if(BandingProcLink!=null) {
					_dictBandingProcs.TryGetValue(BandingProcLink.ProcNum,out BandingProc);
				}
				_dictPatPayPlanOrthoPlanLinks.TryGetValue(ActiveOrthoCase.OrthoCaseNum,out _activeCasePatPayPlanLink);
				if(_activeCasePatPayPlanLink!=null) {
					_dictPatPayPlans.TryGetValue(_activeCasePatPayPlanLink.FKey,out LinkedPayPlan);
				}
			}
		}
	}
}