using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoCases{
		#region Get Methods
		///<summary>Gets one OrthoCase from the db.</summary>
		public static OrthoCase GetOne(long orthoCaseNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OrthoCase>(MethodBase.GetCurrentMethod(),orthoCaseNum);
			}
			return Crud.OrthoCaseCrud.SelectOne(orthoCaseNum);
		}

		///<summary>Gets all Ortho Cases for a patient.</summary>
		public static List<OrthoCase> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoCase>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthocase WHERE orthocase.PatNum = "+POut.Long(patNum);
			return Crud.OrthoCaseCrud.SelectMany(command);
		}

		///<summary>Gets all Ortho Cases for a list of OrthoCaseNums.</summary>
		public static List<OrthoCase> GetMany(List<long> listOrthoCaseNums) {
			if(listOrthoCaseNums.Count==0) {
				return new List<OrthoCase>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoCase>>(MethodBase.GetCurrentMethod(),listOrthoCaseNums);
			}
			string command=$"SELECT * FROM orthocase WHERE orthocase.OrthoCaseNum IN({string.Join(",",listOrthoCaseNums)})";
			return Crud.OrthoCaseCrud.SelectMany(command);
		}

		///<summary>Gets all Ortho Cases for a list of PatNums.</summary>
		public static List<OrthoCase> GetManyForPats(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				return new List<OrthoCase>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoCase>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command=$"SELECT * FROM orthocase WHERE orthocase.PatNum IN({string.Join(",",listPatNums)})";
			return Crud.OrthoCaseCrud.SelectMany(command);
		}

		///<summary>Gets a Patients Active OrthoCase. Patient can only have one active OrthoCase so it is OK to return 1.</summary>
		public static OrthoCase GetActiveForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OrthoCase>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$"SELECT * FROM orthocase WHERE orthocase.PatNum={POut.Long(patNum)} AND orthocase.IsActive={POut.Bool(true)}";
			return Crud.OrthoCaseCrud.SelectOne(command);
		}

		///<summary>Gets a list of active ortho cases for several patients. There should only be 1 active ortho case per patient.</summary>
		public static List<OrthoCase> GetActiveForPats(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				return new List<OrthoCase>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoCase>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command=$"SELECT * FROM orthocase WHERE orthocase.IsActive={POut.Bool(true)} AND orthocase.PatNum IN({string.Join(",",listPatNums)})";
			return Crud.OrthoCaseCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Insert
		///<summary>Insert an OrthoCase into the database. Returns OrthoCaseNum.</summary>
		public static long Insert(OrthoCase orthoCase) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				orthoCase.OrthoCaseNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoCase);
				return orthoCase.OrthoCaseNum;
			}
			return Crud.OrthoCaseCrud.Insert(orthoCase);
		}
		#endregion Insert

		#region Update
		///<summary>Update only data that is different in newOrthoCase.</summary>
		public static void Update(OrthoCase newOrthoCase,OrthoCase oldOrthoCase) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),newOrthoCase,oldOrthoCase);
				return;
			}
			Crud.OrthoCaseCrud.Update(newOrthoCase,oldOrthoCase);
		}

		///<summary>Activates an OrthoCase and its associated OrthoSchedule and OrthoPlanLink. Sets all other OrthoCases for Pat inactive.
		///Returns the refreshed list of OrthoCases.</summary>
		public static List<OrthoCase> Activate(OrthoCase orthoCaseToActivate,long patNum) {
			//No need to check RemotingRole; no call to db.
			OrthoPlanLink scheduleOrthoPlanLink=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseToActivate.OrthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			OrthoSchedule orthoSchedule=OrthoSchedules.GetOne(scheduleOrthoPlanLink.FKey);
			SetActiveState(orthoCaseToActivate,scheduleOrthoPlanLink,orthoSchedule,true);
			DeactivateOthersForPat(orthoCaseToActivate.OrthoCaseNum,orthoSchedule.OrthoScheduleNum,patNum);
			return Refresh(patNum);
		}

		///<summary>Set all objects related to orthocases for a patient inactive besides the ones passed in.</summary>
		public static void DeactivateOthersForPat(long activeOrthoCaseNum,long activeOrthoScheduleNum,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),activeOrthoCaseNum,activeOrthoScheduleNum,patNum);
				return;
			}
			//Get all orthocase nums to deactivate.
			List<long> listOrthoCaseNums=Refresh(patNum).Where(x => x.OrthoCaseNum!=activeOrthoCaseNum).Select(x => x.OrthoCaseNum).ToList();
			if(listOrthoCaseNums.Count<=0) {
				return;
			}
			//Set all other orthocases inactive besides one being activated
			string command=$@"UPDATE orthocase SET orthocase.IsActive={POut.Bool(false)}
				WHERE orthocase.OrthoCaseNum IN({string.Join(",",listOrthoCaseNums)})";
			Db.NonQ(command);
			//Set OrthoPlanLinks inactive
			command=$@"UPDATE orthoplanlink SET orthoplanlink.IsActive={POut.Bool(false)}
				WHERE orthoplanlink.OrthoCaseNum IN({string.Join(",",listOrthoCaseNums)})";
			Db.NonQ(command);
			//Get All OrthoPlanLinks to deactivate
			List<long> listOrthoScheduleNums=
				OrthoPlanLinks.GetAllForOrthoCasesByType(listOrthoCaseNums,OrthoPlanLinkType.OrthoSchedule).Select(x => x.FKey).ToList();
			if(listOrthoScheduleNums.Count<=0) {
				return;
			}
			//Set OrthoSchedules inactive
			command=$@"UPDATE orthoschedule SET orthoschedule.IsActive={POut.Bool(false)}
				WHERE orthoschedule.OrthoScheduleNum IN({string.Join(",",listOrthoScheduleNums)})";
			Db.NonQ(command);
		}

		///<summary>Update the IsActive property for the OrthoCase, OrthoSchedule, and OrthoPlanLink between them.
		///Old ortho case can be passed in if other fields need to be updated so that update doesn't have to be called twice.</summary>
		public static void SetActiveState(OrthoCase orthoCase,OrthoPlanLink scheduleOrthoPlanLink,OrthoSchedule orthoSchedule,bool isActive
			,OrthoCase oldOrthoCase=null) {
			//No remoting role check; no call to db
			if(oldOrthoCase==null) {
				oldOrthoCase=orthoCase.Copy();
			}
			OrthoSchedule oldOrthoSchedule=orthoSchedule.Copy();
			OrthoPlanLink oldScheduleOrthoPlanLink=scheduleOrthoPlanLink.Copy();
			orthoCase.IsActive=isActive;
			orthoSchedule.IsActive=isActive;
			scheduleOrthoPlanLink.IsActive=isActive;
			Update(orthoCase,oldOrthoCase);
			OrthoSchedules.Update(orthoSchedule,oldOrthoSchedule);
			OrthoPlanLinks.Update(scheduleOrthoPlanLink,oldScheduleOrthoPlanLink);
		}

		///<summary>Sets the BandingDate or DebondDate for an OrthoCase.</summary>
		public static void UpdateDatesByLinkedProc(OrthoProcLink procLink,Procedure proc) {
			//No remoting role check; no call to db
			if(procLink.ProcLinkType==OrthoProcType.Visit) {
				return;
			}
			OrthoCase orthoCase=GetOne(procLink.OrthoCaseNum);
			OrthoCase oldOrthoCase=orthoCase.Copy();
			//Update banding date only if banding proc is complete or it is treatment planned and attached to an appointment.
			if(procLink.ProcLinkType==OrthoProcType.Banding && proc.ProcStatus==ProcStat.C || (proc.ProcStatus==ProcStat.TP && proc.AptNum!=0)) {
				orthoCase.BandingDate=proc.ProcDate;
			}
			else if(procLink.ProcLinkType==OrthoProcType.Debond) {
				orthoCase.DebondDate=proc.ProcDate;
			}
			Update(orthoCase,oldOrthoCase);
		}

		/////<summary></summary>
		//public static void Update(OrthoCase orthoCase) {
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoCase);
		//		return;
		//	}
		//	Crud.OrthoCaseCrud.Update(orthoCase);
		//}
		#endregion Update

		#region Delete
		/////<summary>Deletes an OrthoCase from the database, does not delete all items associated to the ortho case, call DeleteAllAssociated.</summary>
		//public static void Delete(long orthoCaseNum) {
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoCaseNum);
		//		return;
		//	}
		//	Crud.OrthoCaseCrud.Delete(orthoCaseNum);
		//}

		///<summary>Throws exceptions. Deletes the OrthoCase and all items associated to the ortho case.</summary>
		public static void Delete(long orthoCaseNum,OrthoSchedule orthoSchedule=null,OrthoPlanLink schedulePlanLink=null
			,List<OrthoProcLink> listProcLinks=null, OrthoPlanLink orthoPlanLinkPatPayPlan=null) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//checked here to save time below
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoCaseNum,orthoSchedule,schedulePlanLink,listProcLinks,orthoPlanLinkPatPayPlan);
				return;
			}
			//Get associated objects if they were not passed in.
			if(schedulePlanLink==null) {
				schedulePlanLink=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			}
			if(schedulePlanLink!=null && orthoSchedule==null) {
				orthoSchedule=OrthoSchedules.GetOne(schedulePlanLink.FKey);
			}
			if(listProcLinks==null) {
				listProcLinks=OrthoProcLinks.GetManyByOrthoCase(orthoCaseNum);
			}
			if(orthoPlanLinkPatPayPlan==null) {
				orthoPlanLinkPatPayPlan=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum,OrthoPlanLinkType.PatPayPlan);
			}
			//Check that all objects are actually associated by primary keys.
			string errorText="Error: Failed to delete ortho case. Attempted to delete";
			if(schedulePlanLink!=null && schedulePlanLink.OrthoCaseNum!=orthoCaseNum) {
				throw new ApplicationException(Lans.g(
					"OrthoCases",$"{errorText} an ortho plan link for an ortho schedule that does not belong to the ortho case."));
			}
			if(orthoSchedule!=null && orthoSchedule.OrthoScheduleNum!=schedulePlanLink.FKey) {
				throw new ApplicationException(Lans.g("OrthoCases",$"{errorText} an ortho schedule that does not belong to the ortho case."));
			}
			foreach(OrthoProcLink procLink in listProcLinks) {
				if(procLink.OrthoCaseNum!=orthoCaseNum) {
					throw new ApplicationException(Lans.g("OrthoCases",$"{errorText} an ortho procedure link that does not belong to the ortho case."));
				}
			}
			if(orthoPlanLinkPatPayPlan!=null && orthoPlanLinkPatPayPlan.OrthoCaseNum!=orthoCaseNum) {
				throw new ApplicationException(Lans.g(
					"Orthocases",$"{errorText} an ortho plan link for a patient payment plan that does not belong to the ortho case."));
			}
			//Delete objects
			Crud.OrthoCaseCrud.Delete(orthoCaseNum);
			Crud.OrthoScheduleCrud.Delete(orthoSchedule.OrthoScheduleNum);
			Crud.OrthoPlanLinkCrud.Delete(schedulePlanLink.OrthoPlanLinkNum);
			OrthoProcLinks.DeleteMany(listProcLinks.Select(x => x.OrthoProcLinkNum).ToList());
			if(orthoPlanLinkPatPayPlan!=null) {
				Crud.OrthoPlanLinkCrud.Delete(orthoPlanLinkPatPayPlan.OrthoPlanLinkNum);
			}
		}
		#endregion Delete

		#region Misc Methods
		///<summary>Parses comma delimited list of procCodes from the specified OrthoCase proc type preference
		///(OrthoBandingCodes, OrthoDebondCodes, OrthoVisitCodes). Returns as list of proc codes.</summary>
		public static List<string> GetListProcTypeProcCodes(PrefName procType) {
			//No remoting role check; no call to db
			return PrefC.GetString(procType).Split(',').Select(x => x.Trim()).ToList();
		}

		///<summary>Returns true if any of the preferences: OrthoBandingCodes, OrthoDebondCodes, OrthoVisitCodes aren't blank.</summary>
		public static bool HasOrthoCasesEnabled() {
			return (PrefC.GetStringSilent(PrefName.OrthoBandingCodes)!=""	
				|| PrefC.GetStringSilent(PrefName.OrthoVisitCodes)!=""	
				|| PrefC.GetStringSilent(PrefName.OrthoDebondCodes)!="");
		}

		///<summary>For a passed in list of procs, this fills a list of all OrthoProcLinks, a dictionary of OrthoProcLinks associated to procs
		///in listProcs, a dictionary of OrthoCases associated to these OrthoProcLinks, and a dictionary of OrthoSchedules for the OrthoCases.</summary>
		public static void GetDataForListProcs(ref List<OrthoProcLink> listOrthoProcLinksAllForPats,
			ref Dictionary<long,OrthoProcLink> dictOrthoProcLinks,ref Dictionary<long,OrthoCase> dictOrthoCases,
			ref Dictionary<long,OrthoSchedule> dictOrthoSchedules,List<Procedure> listProcs) 
		{
			//No remoting role check; no call to db and ref parameters.
			dictOrthoCases=GetManyForPats(listProcs.Select(x => x.PatNum).Distinct().ToList()).ToDictionary(x => x.OrthoCaseNum,x => x);
			listOrthoProcLinksAllForPats=OrthoProcLinks.GetManyByOrthoCases(dictOrthoCases.Keys.ToList());
			dictOrthoProcLinks=
				listOrthoProcLinksAllForPats.Where(x => listProcs.Select(y => y.ProcNum).ToList().Contains(x.ProcNum)).ToDictionary(x => x.ProcNum,x => x);
			if(dictOrthoProcLinks.Count>0) {
				Dictionary<long,OrthoPlanLink> dictSchedulePlanLinks=
					OrthoPlanLinks.GetAllForOrthoCasesByType(dictOrthoCases.Keys.ToList(),OrthoPlanLinkType.OrthoSchedule).ToDictionary(x => x.FKey,x => x);
				List<OrthoSchedule> listOrthoSchedules=OrthoSchedules.GetMany(dictSchedulePlanLinks.Keys.ToList());
				dictOrthoSchedules=listOrthoSchedules.ToDictionary(x => dictSchedulePlanLinks[x.OrthoScheduleNum].OrthoCaseNum,x => x);
			}
		}

		///<summary>This fills a list of all OrthoProcLinks, a dictionary of OrthoProcLinks, a dictionary of OrthoCases associated to these OrthoProcLinks,
		///and a dictionary of OrthoSchedules for the OrthoCases.</summary>
		public static void GetDataForAllProcLinks(ref List<OrthoProcLink> listOrthoProcLinksAll,
			ref Dictionary<long,OrthoProcLink> dictOrthoProcLinks,ref Dictionary<long,OrthoCase> dictOrthoCases,
			ref Dictionary<long,OrthoSchedule> dictOrthoSchedules) 
		{
			//No remoting role check; no call to db
			listOrthoProcLinksAll=OrthoProcLinks.GetAll();
			if(listOrthoProcLinksAll.Count>0) {
				dictOrthoProcLinks=listOrthoProcLinksAll.ToDictionary(x => x.ProcNum,x => x);
				dictOrthoCases=GetMany(dictOrthoProcLinks.Values.Select(x => x.OrthoCaseNum).Distinct().ToList()).ToDictionary(x => x.OrthoCaseNum,x => x);
				Dictionary<long,OrthoPlanLink> dictSchedulePlanLinks=
					OrthoPlanLinks.GetAllForOrthoCasesByType(dictOrthoCases.Keys.ToList(),OrthoPlanLinkType.OrthoSchedule).ToDictionary(x => x.FKey,x => x);
				List<OrthoSchedule> listOrthoSchedules=OrthoSchedules.GetMany(dictSchedulePlanLinks.Keys.ToList());
				dictOrthoSchedules=listOrthoSchedules.ToDictionary(x => dictSchedulePlanLinks[x.OrthoScheduleNum].OrthoCaseNum,x => x);
			}
		}

		///<summary>Fills ref parameters for an orthoProcLink, orthoCase, orthoSchedule, and list of orthoProcLinks for the orthoCase.
		///These objects are used in several places to call Procedures.ComputeEstimates()</summary>
		public static void FillOrthoCaseObjectsForProc(long procNum,ref OrthoProcLink orthoProcLink,ref OrthoCase orthoCase,ref OrthoSchedule orthoSchedule,
			ref List<OrthoProcLink> listOrthoProcLinksForOrthoCase,Dictionary<long,OrthoProcLink> dictOrthoProcLinksForProcList,
			Dictionary<long,OrthoCase> dictOrthoCases,Dictionary<long,OrthoSchedule> dictOrthoSchedules,List<OrthoProcLink> listOrthoProcLinksAll) 
		{
			//No remoting role check; no call to db
			listOrthoProcLinksForOrthoCase=null;
			dictOrthoProcLinksForProcList.TryGetValue(procNum,out orthoProcLink);
			//If proc is linked to an OrthoCase, get other OrthoCase data needed to update estimates.
			if(orthoProcLink!=null) {
				long orthoCaseNum=orthoProcLink.OrthoCaseNum;
				dictOrthoCases.TryGetValue(orthoCaseNum,out orthoCase);
				dictOrthoSchedules.TryGetValue(orthoCaseNum,out orthoSchedule);
				listOrthoProcLinksForOrthoCase=listOrthoProcLinksAll.Where(x => x.OrthoCaseNum==orthoCaseNum).ToList();
			}
		}
		#endregion Misc Methods
	}
}