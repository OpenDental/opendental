using System.Collections.Generic;
using System.Linq;
using System.Windows;
using OpenDentBusiness;
using System;
using CodeBase;

namespace OpenDental {
	public class ProcedureL {
		///<summary>Sets all procedures for apt complete.  Flags procedures as CPOE as needed (when prov logged in).  Makes a log
		///entry for each completed proc.  Then fires the CompleteProcedure automation trigger.</summary>
		public static List<Procedure> SetCompleteInAppt(Appointment appointment,List<InsPlan> listInsPlans,List<PatPlan> listPatPlans,Patient patient,
			List<InsSub> listInsSub,bool removeCompletedProcs) 
		{
			List<Procedure> listProcedures=Procedures.SetCompleteInAppt(appointment,listInsPlans,listPatPlans,patient,listInsSub,removeCompletedProcs);
			List<string> listStrProcCodes=listProcedures.Select(x => ProcedureCodes.GetStringProcCode(x.CodeNum)).ToList();
			AutomationL.Trigger(EnumAutomationTrigger.ProcedureComplete,listStrProcCodes,appointment.PatNum);
			Procedures.AfterProcsSetComplete(listProcedures);
			return listProcedures;
		}

		///<summary>Returns empty string if no duplicates, otherwise returns duplicate procedure information.  In all places where this is called, we are guaranteed to have the eCW bridge turned on.  So this is an eCW peculiarity rather than an HL7 restriction.  Other HL7 interfaces will not be checking for duplicate procedures unless we intentionally add that as a feature later.</summary>
		public static string ProcsContainDuplicates(List<Procedure> listProcedures) {
			bool hasLongDCodes=false;
			HL7Def hL7Def=HL7Defs.GetOneDeepEnabled();
			if(hL7Def!=null) {
				hasLongDCodes=hL7Def.HasLongDCodes;
			}
			string info="";
			List<Procedure> listProceduresChecked=new List<Procedure>();
			for(int i=0;i<listProcedures.Count;i++) {
				Procedure procedure=listProcedures[i];
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcedures[i].CodeNum);
				string procCodeStr=procedureCode.ProcCode;
				if(procCodeStr.Length>5
					&& procCodeStr.StartsWith("D")
					&& !hasLongDCodes)
				{
					procCodeStr=procCodeStr.Substring(0,5);
				}
				for(int j=0;j<listProceduresChecked.Count;j++) {
					Procedure procedureDup=listProceduresChecked[j];
					ProcedureCode procedureCodeDup=ProcedureCodes.GetProcCode(listProceduresChecked[j].CodeNum);
					string procCodeDupStr=procedureCodeDup.ProcCode;
					if(procCodeDupStr.Length>5
						&& procCodeDupStr.StartsWith("D")
						&& !hasLongDCodes)
					{
						procCodeDupStr=procCodeDupStr.Substring(0,5);
					}
					if(procCodeDupStr!=procCodeStr) {
						continue;
					}
					if(procedureDup.ToothNum!=procedure.ToothNum) {
						continue;
					}
					if(procedureDup.ToothRange!=procedure.ToothRange) {
						continue;
					}
					if(procedureDup.ProcFee!=procedure.ProcFee) {
						continue;
					}
					if(procedureDup.Surf!=procedure.Surf) {
						continue;
					}
					if(info!="") {
						info+=", ";
					}
					info+=procCodeDupStr;
				}
				listProceduresChecked.Add(procedure);
			}
			if(info!="") {
				info=Lan.g("ProcedureL","Duplicate procedures")+": "+info;
			}
			return info;
		}

		///<summary>Checks to see if the appointments provider has at least one mismatch provider on all the completed procedures attached to the appointment.
		///If so, checks to see if the user has permission to edit a completed procedure. If the user does, then the user has the option to change the provider to match.</summary>
		public static bool DoRemoveCompletedProcs(Appointment appointment,List<Procedure> listProceduresForAppt,bool checkForAllProcCompl=false) {
			if(listProceduresForAppt.Count==0) {
				return false;
			}
			if(checkForAllProcCompl && (appointment.AptStatus!=ApptStatus.Complete || listProceduresForAppt.All(x => x.ProcStatus==ProcStat.C))) {
				return false;
			}
			List<Procedure> listProceduresCompletedWithDifferentProv=new List<Procedure>();
			for(int i=0;i<listProceduresForAppt.Count;i++) {
				if(listProceduresForAppt[i].ProcStatus!=ProcStat.C || listProceduresForAppt[i].AptNum!=appointment.AptNum) {//should all be complete already. 
					continue;
				}
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProceduresForAppt[i].CodeNum);
				long provNum=Procedures.GetProvNumFromAppointment(appointment,listProceduresForAppt[i],procedureCode);
				if(provNum!=listProceduresForAppt[i].ProvNum) {
					listProceduresCompletedWithDifferentProv.Add(listProceduresForAppt[i]);
				}
			}
			if(listProceduresCompletedWithDifferentProv.Count==0) {
				return true;//All completed procs match appt, so ignore completed procedures when running Procedures.UpdateProcsInApptHelper()
			}
			List<long> listProcNums=listProceduresCompletedWithDifferentProv.Select(x => x.ProcNum).ToList();
			if(PrefC.GetBool(PrefName.ProcProvChangesClaimProcWithClaim)) {
				List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForProcs(listProcNums);
				if(listClaimProcs.Any(x => x.Status==ClaimProcStatus.Received
					|| x.Status==ClaimProcStatus.Supplemental
					|| x.Status==ClaimProcStatus.CapClaim)) 
				{
					MsgBox.Show("Procedures","The appointment provider does not match the provider on at least one procedure that is attached " 
						+"to a claim.\r\nThe provider on the procedure(s) cannot be changed.");
					return true;
				}
			}
			List<PaySplit> listPaySplits=PaySplits.GetPaySplitsFromProcs(listProcNums);
			if(listPaySplits.Count>0) {
				MsgBox.Show("Procedures","The appointment provider does not match the provider on at least one completed procedure.\r\n"
					+"The procedure provider cannot be changed to match the appointment provider because the paysplit provider would no longer match.  "
					+"Any change to the provider on the completed procedure(s) or paysplit(s) will have to be made manually.");
				return true;//paysplits exist on one of the completed procedures. Per Nathan, don't change the provider. User will need to change manually.
			}
			for(int i=0;i<listProceduresCompletedWithDifferentProv.Count;i++) {
				EnumPermType permissions=GroupPermissions.SwitchExistingPermissionIfNeeded(EnumPermType.ProcCompleteEdit,listProceduresCompletedWithDifferentProv[i]);
				DateTime dateTimeForPerm=Procedures.GetDateForPermCheck(listProceduresCompletedWithDifferentProv[i]);
				if(Security.IsGlobalDateLock(permissions,dateTimeForPerm)) {
					return true;
				}
				if(!Security.IsAuthorized(permissions,dateTimeForPerm,suppressMessage:true,suppressLockDateMessage:true)) {
					MessageBox.Show(Lan.g("Procedures","The appointment provider does not match the provider on at least one completed procedure.")+"\r\n"
						+Lans.g("Procedures","Not authorized for")+": "+GroupPermissions.GetDesc(permissions)+"\r\n"
						+Lan.g("Procedures","Any change to the provider on the completed procedure(s) will have to be made manually."));
					return true;//user does not have permission to change the provider. Don't change provider.
				}
			}
			//The appointment is set complete, completed procedures exist, and provider does not match appointment.
			//Ask if they would like to change the providers on the completed procedure to match the appointments provider
			if(!MsgBox.Show("Procedures",MsgBoxButtons.YesNo,"The appointment provider does not match the provider on at least one completed procedure.\r\n"
				+"Change the provider on the completed procedure(s) to match the provider on the appointment?"))
			{
				return true;//user does not want to change the providers
			}
			//user wants to change the provider on the completed procedure
			return false;
		}

		///<summary>Checks if proc is Complete and has ClaimProcs attached to a Claim, and if so, prompts the user that this is invalid.</summary>
		public static bool IsProcCompleteAttachedToClaim(Procedure procedure,List<ClaimProc> listClaimProcs,bool isSilent=false) {
			List<ClaimProc> listClaimProcsFiltered=listClaimProcs.FindAll(x => x.ProcNum==procedure.ProcNum);
			if(procedure.ProcStatus==ProcStat.C && Procedures.IsAttachedToClaim(procedure,listClaimProcsFiltered,isPreauthIncluded:false)) {
				//status cannot be changed for completed procedures attached to a claim, except we allow changing status for preauths.
				if(!isSilent) {
					MsgBox.Show("Procedures","This is a completed procedure that is attached to a claim.  You must remove the procedure from the claim"+
						" or delete the claim before editing the status.");
				}
				return true;
			}
			return false;
		}

		///<summary></summary>
		public static bool ValidateProvider(List<ClaimProc> listClaimProcs,long provNumSelected,long provNumForProc) {
			Action<string> actionOnFailure=(msg) => {
				MsgBox.Show(msg);
			};
			return Procedures.ValidateProvider(listClaimProcs,provNumSelected,provNumForProc,actionOnFailure);
		}

		///<summary>Only needs to be called when procOld.ProcStatus is C, EO or EC.</summary>
		public static bool CheckPermissionsAndGlobalLockDate(Procedure procedureOld, Procedure procedureNew, DateTime dateTimeProc,
			double procFeeOverride=double.MinValue,bool suppressMessage=false)
		{
			Action<string> actionNotAuth=(msg) => {
				if(!suppressMessage) {
					MsgBox.Show(msg);
				}
			};
			return Procedures.CheckPermissionsAndGlobalLockDate(procedureOld,procedureNew,dateTimeProc,Security.CurUser,procFeeOverride,actionNotAuth);
		}

	}
}
