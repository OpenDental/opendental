using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using OpenDentBusiness.UI;
using CodeBase;
using System.Linq;

namespace OpenDental{
	public class AppointmentL {
		///<summary>Used by UI when it needs a recall appointment placed on the pinboard ready to schedule.  This method creates the appointment and 
		///attaches all appropriate procedures.  It's up to the calling class to then place the appointment on the pinboard.  If the appointment doesn't 
		///get scheduled, it's important to delete it.  If a recallNum is not 0 or -1, then it will create an appt of that recalltype. Otherwise it will
		///only use either a Perio or Prophy recall type.</summary>
		public static Appointment CreateRecallApt(Patient patCur,List<InsPlan> planList,long recallNum,List<InsSub> subList
			,DateTime aptDateTime=default(DateTime))
		{
			List<Recall> recallList=Recalls.GetList(patCur.PatNum);
			Recall recallCur=null;
			if(recallNum>0) {
				recallCur=Recalls.GetRecall(recallNum);
			}
			else{
				for(int i=0;i<recallList.Count;i++){
					if(recallList[i].RecallTypeNum==RecallTypes.PerioType || recallList[i].RecallTypeNum==RecallTypes.ProphyType){
						if(!recallList[i].IsDisabled){
							recallCur=recallList[i];
						}
						break;
					}
				}
			}
			if(recallCur==null){
				//Typically never happens because everyone has a recall.  However, it can happen when patients have custom recalls due
				throw new ApplicationException(Lan.g("AppointmentL","No special type recall is due."));
			}
			if(recallCur.DateScheduled.Date>DateTime.Today) {
				throw new ApplicationException(Lan.g("AppointmentL","Recall has already been scheduled for ")+recallCur.DateScheduled.ToShortDateString());
			}
			Appointment aptCur=new Appointment();
			aptCur.AptDateTime=aptDateTime;		
			List<string> procs=RecallTypes.GetProcs(recallCur.RecallTypeNum);
			List<Procedure> listProcs=Appointments.FillAppointmentForRecall(aptCur,recallCur,recallList,patCur,procs,planList,subList);
			for(int i=0;i<listProcs.Count;i++) {
				if(Programs.UsingOrion) {
					using FormProcEdit FormP=new FormProcEdit(listProcs[i],patCur.Copy(),Patients.GetFamily(patCur.PatNum));
					FormP.IsNew=true;
					FormP.ShowDialog();
					if(FormP.DialogResult==DialogResult.Cancel) {
						//any created claimprocs are automatically deleted from within procEdit window.
						try {
							Procedures.Delete(listProcs[i].ProcNum);//also deletes the claimprocs
						}
						catch(Exception ex) {
							MessageBox.Show(ex.Message);
						}
					}
					else {
						//Do not synch. Recalls based on ScheduleByDate reports in Orion mode.
						//Recalls.Synch(PatCur.PatNum);
					}
				}
			}
			return aptCur;
		}

		///<summary>Returns true if PrefName.BrokenApptProcedure is greater than 0.</summary>
		public static bool HasBrokenApptProcs() {
			return PrefC.GetLong(PrefName.BrokenApptProcedure)>0;
		}

		///<summary>Sets given appt.AptStatus to broken.
		///Provide procCode that should be charted, can be null but will not chart a broken procedure.
		///Also considers various broken procedure based prefs.
		///Makes its own securitylog entries.</summary>
		public static void BreakApptHelper(Appointment appt,Patient pat,ProcedureCode procCode) {
			//suppressHistory is true due to below logic creating a log with a specific HistAppointmentAction instead of the generic changed.
			DateTime datePrevious=appt.DateTStamp;
			bool suppressHistory=false;
			if(procCode!=null) {
				suppressHistory=(ListTools.In(procCode.ProcCode,"D9986","D9987"));
			}
			Appointments.SetAptStatus(appt,ApptStatus.Broken,suppressHistory); //Appointments S-Class handles Signalods
			if(appt.AptStatus!=ApptStatus.Complete) { //seperate log entry for completed appointments.
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,pat.PatNum,
					appt.ProcDescript+", "+appt.AptDateTime.ToString()
					+", Broken from the Appts module.",appt.AptNum,datePrevious);
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,pat.PatNum,
					appt.ProcDescript+", "+appt.AptDateTime.ToString()
					+", Broken from the Appts module.",appt.AptNum,datePrevious);
			}
			#region HL7
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				//S15 - Appt Cancellation event
				MessageHL7 messageHL7=MessageConstructor.GenerateSIU(pat,Patients.GetPat(pat.Guarantor),EventTypeHL7.S15,appt);
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=appt.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=pat.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MessageBox.Show("Appointments",messageHL7.ToString());
					}
				}
			}
			#endregion
			#region HieQueue
			if(HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(pat.PatNum));
			}
			#endregion
			List<Procedure> listProcedures=new List<Procedure>();
			//splits should only exist on procs if they are using tp pre-payments
			List<PaySplit> listSplitsForApptProcs=new List<PaySplit>();
			bool isNonRefundable=false;
			double brokenProcAmount=0;
			Procedure brokenProcedure=new Procedure();
			bool wasBrokenProcDeleted=false;
			if(PrefC.GetYN(PrefName.PrePayAllowedForTpProcs)) {
				listProcedures=Procedures.GetProcsForSingle(appt.AptNum,false);
				if(listProcedures.Count > 0) {
					listSplitsForApptProcs=PaySplits.GetPaySplitsFromProcs(listProcedures.Select(x => x.ProcNum).ToList());
				}
			}
			#region Charting the proc
			if(procCode!=null) {
				switch(procCode.ProcCode) { 
					case "D9986"://Missed
						HistAppointments.CreateHistoryEntry(appt.AptNum,HistAppointmentAction.Missed);
					break;
					case "D9987"://Cancelled
						HistAppointments.CreateHistoryEntry(appt.AptNum,HistAppointmentAction.Cancelled);
					break;
				}
				brokenProcedure.PatNum=pat.PatNum;
				brokenProcedure.ProvNum=(procCode.ProvNumDefault>0 ? procCode.ProvNumDefault : appt.ProvNum);
				brokenProcedure.CodeNum=procCode.CodeNum;
				brokenProcedure.ProcDate=DateTime.Today;
				brokenProcedure.DateEntryC=DateTime.Now;
				brokenProcedure.DateTP=DateTime.Today;
				brokenProcedure.ProcStatus=ProcStat.C;
				brokenProcedure.ClinicNum=appt.ClinicNum;
				brokenProcedure.UserNum=Security.CurUser.UserNum;
				brokenProcedure.Note=Lans.g("AppointmentEdit","Appt BROKEN for")+" "+appt.ProcDescript+"  "+appt.AptDateTime.ToString();
				brokenProcedure.PlaceService=(PlaceOfService)PrefC.GetInt(PrefName.DefaultProcedurePlaceService);//Default proc place of service for the Practice is used. 
				List<InsSub> listInsSubs=InsSubs.RefreshForFam(Patients.GetFamily(pat.PatNum));
				List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
				List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
				InsPlan insPlanPrimary=null;
				InsSub insSubPrimary=null;
				if(listPatPlans.Count>0) {
					insSubPrimary=InsSubs.GetSub(listPatPlans[0].InsSubNum,listInsSubs);
					insPlanPrimary=InsPlans.GetPlan(insSubPrimary.PlanNum,listInsPlans);
				}
				double procFee;
				long feeSch;
				if(insPlanPrimary==null||procCode.NoBillIns) {
					feeSch=FeeScheds.GetFeeSched(0,pat.FeeSched,brokenProcedure.ProvNum);
				}
				else {//Only take into account the patient's insurance fee schedule if the D9986 procedure is not marked as NoBillIns
					feeSch=FeeScheds.GetFeeSched(insPlanPrimary.FeeSched,pat.FeeSched,brokenProcedure.ProvNum);
				}
				procFee=Fees.GetAmount0(brokenProcedure.CodeNum,feeSch,brokenProcedure.ClinicNum,brokenProcedure.ProvNum);
				if(insPlanPrimary!=null&&insPlanPrimary.PlanType=="p"&&!insPlanPrimary.IsMedical) {//PPO
					double provFee=Fees.GetAmount0(brokenProcedure.CodeNum,Providers.GetProv(brokenProcedure.ProvNum).FeeSched,brokenProcedure.ClinicNum,
					brokenProcedure.ProvNum);
					brokenProcedure.ProcFee=Math.Max(provFee,procFee);
				}
				else if(listSplitsForApptProcs.Count>0 && PrefC.GetBool(PrefName.TpPrePayIsNonRefundable) && procCode.ProcCode=="D9986") {
					//if there are pre-payments, non-refundable pre-payments is turned on, and the broken appointment is a missed code then auto-fill 
					//the window with the sum of the procs for the appointment. Transfer money below after broken procedure is confirmed by the user.
					brokenProcedure.ProcFee=listSplitsForApptProcs.Sum(x => x.SplitAmt);
					isNonRefundable=true;
				}
				else {
					brokenProcedure.ProcFee=procFee;
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					brokenProcedure.SiteNum=pat.SiteNum;
				}
				Procedures.Insert(brokenProcedure);
				//Now make a claimproc if the patient has insurance.  We do this now for consistency because a claimproc could get created in the future.
				List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
				List<ClaimProc> listClaimProcsForProc=ClaimProcs.RefreshForProc(brokenProcedure.ProcNum);
				Procedures.ComputeEstimates(brokenProcedure,pat.PatNum,listClaimProcsForProc,false,listInsPlans,listPatPlans,listBenefits,pat.Age,listInsSubs);
				using FormProcBroken FormPB=new FormProcBroken(brokenProcedure,isNonRefundable);
				FormPB.IsNew=true;
				FormPB.ShowDialog();
				brokenProcAmount=FormPB.AmountTotal;
				wasBrokenProcDeleted=FormPB.IsProcDeleted;
				ProcedureL.AfterProcsSetComplete(ListTools.FromSingle(brokenProcedure));
			}
			#endregion
			#region BrokenApptAdjustment
			if(PrefC.GetBool(PrefName.BrokenApptAdjustment)) {
				Adjustment AdjustmentCur=new Adjustment();
				AdjustmentCur.DateEntry=DateTime.Today;
				AdjustmentCur.AdjDate=DateTime.Today;
				AdjustmentCur.ProcDate=DateTime.Today;
				AdjustmentCur.ProvNum=appt.ProvNum;
				AdjustmentCur.PatNum=pat.PatNum;
				AdjustmentCur.AdjType=PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType);
				AdjustmentCur.ClinicNum=appt.ClinicNum;
				using FormAdjust FormA=new FormAdjust(pat,AdjustmentCur);
				FormA.IsNew=true;
				FormA.ShowDialog();
			}
			#endregion
			#region BrokenApptCommLog
			if(PrefC.GetBool(PrefName.BrokenApptCommLog)) {
				Commlog commlogCur=new Commlog();
				commlogCur.PatNum=pat.PatNum;
				commlogCur.CommDateTime=DateTime.Now;
				commlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
				commlogCur.Note=Lan.g("Appointment","Appt BROKEN for")+" "+appt.ProcDescript+"  "+appt.AptDateTime.ToString();
				commlogCur.Mode_=CommItemMode.None;
				commlogCur.UserNum=Security.CurUser.UserNum;
				commlogCur.IsNew=true;
				using FormCommItem FormCI=new FormCommItem(commlogCur);
				FormCI.ShowDialog();
			}
			#endregion
			#region Transfer money from TP Procedures if necessary
			//Note this MUST come after FormProcBroken since clicking cancel in that window will delete the procedure.
			if(isNonRefundable && !wasBrokenProcDeleted && listSplitsForApptProcs.Count>0) {
				//transfer what the user specified in the broken appointment window.
				//transfer up to the amount specified by the user
				foreach(Procedure proc in listProcedures) {
					if(brokenProcAmount==0) {
						break;
					}
					List<PaySplit> listSplitsForAppointmentProcedure=listSplitsForApptProcs.FindAll(x => x.ProcNum==proc.ProcNum);
					foreach(PaySplit split in listSplitsForAppointmentProcedure) {
						if(brokenProcAmount==0) {
							break;
						}							
						double amt=Math.Min(brokenProcAmount,split.SplitAmt);
						Payments.CreateTransferForTpProcs(proc,new List<PaySplit>{split},brokenProcedure,amt);
						double amtPaidOnApt=listSplitsForApptProcs.Sum(x => x.SplitAmt);
						if(amtPaidOnApt>amt) {
							//If the original prepayment amount is greater than the amt being specified for the appointment break, transfer
							//the difference to an Unallocated Unearned Paysplit on the account.
							double remainingAmt=amtPaidOnApt-amt;
							//We have to create a new transfer payment here to correlate to the split.
							Payment txfrPayment=new Payment();
							txfrPayment.PayAmt=0;
							txfrPayment.PayDate=DateTime.Today;
							txfrPayment.ClinicNum=split.ClinicNum;
							txfrPayment.PayNote="Automatic transfer from treatment planned procedure prepayment.";
							txfrPayment.PatNum=split.PatNum;//ultimately where the payment ends up.
							txfrPayment.PayType=0;
							Payments.Insert(txfrPayment);
							PaymentEdit.IncomeTransferData transferData=PaymentEdit.IncomeTransferData.CreateTransfer(split,txfrPayment.PayNum,true,remainingAmt);
							PaySplits.InsertMany(txfrPayment.PayNum,transferData.ListSplitsCur);
							SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,txfrPayment.PatNum,"Automatic transfer of funds for treatment plan procedure pre-payments.");
						}
						brokenProcAmount-=amt;
					}
				}
			}
			//if broken appointment procedure was deleted (user cancelled out of the window) just keep money on the original procedure.
			#endregion
			AppointmentEvent.Fire(ODEventType.AppointmentEdited,appt);
			AutomationL.Trigger(AutomationTrigger.BreakAppointment,null,pat.PatNum);
			Recalls.SynchScheduledApptFull(appt.PatNum);
		}

		/// <summary>If an appointment is broken, prompts the user if they would like to text the ASAP list to offer the newly opened time slot. Prompt only appears 
		/// if WebSchedAsapEnabled pref is true, AND there are appointments in the ASAP list, and AsapPromptEnabled pref is true. Returns false if any of these 
		/// conditions are false, or user chooses not to text ASAP list, otherwise true. 
		/// </summary>
		public static bool PromptTextAsapList(long clinicNum) {
			if(!PrefC.GetBool(PrefName.WebSchedAsapEnabled)) {
				return false;//If websched asap is disabled
			}
			if(!PIn.Bool(ClinicPrefs.GetPrefValue(PrefName.AsapPromptEnabled,clinicNum))) {
				return false;//If the prompt is disabled
			}
			if(Appointments.RefreshASAP(0,0,clinicNum,new List<ApptStatus>()).Count==0) {
				return false;//If no ASAP appointments available
			}
			if(!MsgBox.Show("Appointment",MsgBoxButtons.YesNo,"Text patients on the ASAP List and offer them this opening?")) {
				return false;
			}
			return true;
		}

		///<summary>Calculates the start and end time that is used by websched to offer an open timeslot to patients on the ASAP List.</summary>
		public static DateRange GetAsapRange(long opNum,DateTime dateTimeSelected,long aptNum,List<Schedule> listSchedules) {
			DateTime dateTimeSlotStart=dateTimeSelected.Date;//Midnight contrAppt=dateTimeClicked not DateSelected
			DateTime dateTimeSlotEnd=dateTimeSelected.Date.AddDays(1);//Midnight tomorrow
			//Get appoinments for the period of the entire day of dateTimeSelected
			List<Appointment> listAppointmentsInOp=Appointments.GetAppointmentsForOpsByPeriod(new List<long> { opNum },dateTimeSelected,dateTimeSelected);
			for(int i=0;i<listAppointmentsInOp.Count;i++) {
				Appointment apptCur=listAppointmentsInOp[i];
				if(apptCur.AptNum==aptNum || apptCur.AptDateTime.Date!=dateTimeSelected.Date) {
					continue;
				}
				DateTime dateEndApt=apptCur.AptDateTime.AddMinutes(apptCur.Pattern.Length*5);
				if(dateEndApt.Between(dateTimeSlotStart,dateTimeSelected)) {
					dateTimeSlotStart=dateEndApt;
				}
				if(apptCur.AptDateTime.Between(dateTimeSelected,dateTimeSlotEnd)) {
					dateTimeSlotEnd=apptCur.AptDateTime;
				}
			}
			//Next, loop through blockouts that don't allow scheduling and adjust the slot size
			List<Def> listDefsBlockoutTypes=Defs.GetDefsForCategory(DefCat.BlockoutTypes);
			foreach(Schedule blockout in Schedules.GetForType(listSchedules,ScheduleType.Blockout,0)
				.Where(x => x.SchedDate==dateTimeSelected.Date && x.Ops.Contains(opNum))) 
			{
				if(Schedules.CanScheduleInBlockout(blockout.BlockoutType,listDefsBlockoutTypes)) {
					continue;
				}
				DateTime dateTimeBlockoutStop=blockout.SchedDate.Add(blockout.StopTime);
				if(dateTimeBlockoutStop.Between(dateTimeSlotStart,dateTimeSelected)) {
					//Move start time to be later to account for this blockout.
					dateTimeSlotStart=dateTimeBlockoutStop;
				}
				DateTime dateTimeBlockoutStart=blockout.SchedDate.Add(blockout.StartTime);
				if(dateTimeBlockoutStart.Between(dateTimeSelected,dateTimeSlotEnd)) {
					//Move stop time to be earlier to account for this blockout.
					dateTimeSlotEnd=dateTimeBlockoutStart;
				}
				if(dateTimeSelected.Between(dateTimeBlockoutStart,dateTimeBlockoutStop,isUpperBoundInclusive: false)) {
					throw new ODException("Unable to schedule appointments on blockouts marked 'Block appointments scheduling'.");
				}
			}
			dateTimeSlotStart=ODMathLib.Max(dateTimeSlotStart,dateTimeSelected.AddHours(-1));
			dateTimeSlotEnd=ODMathLib.Min(dateTimeSlotEnd,dateTimeSelected.AddHours(3));
			return new DateRange(dateTimeSlotStart,dateTimeSlotEnd);
		}

		public static bool ValidateApptToPinboard(Appointment appt) {
			if(!Security.IsAuthorized(Permissions.AppointmentMove)) {
				return false;
			}
			if(appt.AptStatus==ApptStatus.Complete) {
				MsgBox.Show("Appointments","Not allowed to move completed appointments.");
				return false;
			}
			if(PatRestrictionL.IsRestricted(appt.PatNum,PatRestrict.ApptSchedule)) {
				return false;
			}
			return true;
		}

		/// <summary>Helper method to send given appt to pinboard.
		/// Refreshes Appointment module.
		/// Also does some appointment and security validation.</summary>
		public static void CopyAptToPinboardHelper(Appointment appt) {
			GotoModule.PinToAppt(new List<long>() { appt.AptNum },appt.PatNum);
		}

		public static bool ValidateApptUnsched(Appointment appt) {
			if((appt.AptStatus!=ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentMove)) //seperate permissions for complete appts.
				||(appt.AptStatus==ApptStatus.Complete && !Security.IsAuthorized(Permissions.AppointmentCompleteEdit))) {
				return false;
			}
			if(PatRestrictionL.IsRestricted(appt.PatNum,PatRestrict.ApptSchedule)) {
				return false;
			}
			if(appt.AptStatus==ApptStatus.PtNote | appt.AptStatus==ApptStatus.PtNoteCompleted) {
				return false;
			}
			return true;
		}

		/// <summary>Helper method to send given appt to the unscheduled list.
		/// Creates SecurityLogs and considers HL7.</summary>
		public static void SetApptUnschedHelper(Appointment appt,Patient pat=null,bool doFireApptEvent=true) {
			DateTime datePrevious=appt.DateTStamp;
			Appointments.SetAptStatus(appt,ApptStatus.UnschedList); //Appointments S-Class handles Signalods
			#region SecurityLogs
			if(appt.AptStatus!=ApptStatus.Complete) { //seperate log entry for editing completed appts.
				SecurityLogs.MakeLogEntry(Permissions.AppointmentMove,appt.PatNum,
					appt.ProcDescript+", "+appt.AptDateTime.ToString()+", Sent to Unscheduled List",
					appt.AptNum,datePrevious);
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,appt.PatNum,
					appt.ProcDescript+", "+appt.AptDateTime.ToString()+", Sent to Unscheduled List",
					appt.AptNum,datePrevious);
			}
			#endregion
			#region HL7
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				if(pat==null) {
					pat=Patients.GetPat(appt.PatNum);
				}
				//S15 - Appt Cancellation event
				MessageHL7 messageHL7=MessageConstructor.GenerateSIU(pat,Patients.GetPat(pat.Guarantor),EventTypeHL7.S15,appt);
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=appt.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=pat.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MessageBox.Show("Appointments",messageHL7.ToString());
					}
				}
			}
			#endregion
			#region HieQueue
			if(HieClinics.IsEnabled()) {
				if(pat==null) {
					pat=Patients.GetPat(appt.PatNum);
				}
				HieQueues.Insert(new HieQueue(pat.PatNum));
			}
			#endregion
			if(doFireApptEvent) {
				AppointmentEvent.Fire(ODEventType.AppointmentEdited,appt);
			}
			Recalls.SynchScheduledApptFull(appt.PatNum);
		}

		///<summary>Creats a new appointment for the given patient.  A valid patient must be passed in.
		///Set useApptDrawingSettings to true if the user double clicked on the appointment schedule in order to make a new appointment.
		///It will utilize the global static properties to help set required fields for "Scheduled" appointments.
		///Otherwise, simply sets the corresponding PatNum and then the status to "Unscheduled".</summary>
		public static Appointment MakeNewAppointment(Patient PatCur,bool useApptDrawingSettings,DateTime? dateTNew=null,long? opNumNew=null) {
			//Appointments.MakeNewAppointment may or may not use apptDateTime depending on useApptDrawingSettings,
			//however it's safer to just pass in the appropriate datetime verses DateTime.MinVal.
			DateTime apptDateTime=DateTime.MinValue;
			long opNum=0;
			if(dateTNew!=null) {
				apptDateTime=dateTNew.Value;
			}
			if(opNumNew!=null) {
				opNum=opNumNew.Value;
			}
			//Make the appointment in memory
			Appointment apptCur=Appointments.MakeNewAppointment(PatCur,apptDateTime,opNum,useApptDrawingSettings);
			if(PatCur.AskToArriveEarly>0 && useApptDrawingSettings) {
				MessageBox.Show(Lan.g("FormApptsOther","Ask patient to arrive")+" "+PatCur.AskToArriveEarly
					+" "+Lan.g("FormApptsOther","minutes early at")+" "+apptCur.DateTimeAskedToArrive.ToShortTimeString()+".");
			}
			return apptCur;
		}

		///<summary>Looks up PatientLinks for previous merge.  Asks user if they want to switch to the correct patient.  Returns false if no merge or they want to keep this patient anyway.  Returns true if the user switched to a different patient.</summary>
		public static bool PromptForMerge(Patient patCur,out Patient newPatCur) {
			newPatCur=patCur;
			if(patCur==null) {
				return false;
			}
			List<PatientLink> listMergedPats=PatientLinks.GetLinks(patCur.PatNum,PatientLinkType.Merge);
			if(!PatientLinks.WasPatientMerged(patCur.PatNum,listMergedPats)) {
				return false;
			}
			//This patient has been merged before.  Get a list of all patients that this patient has been merged into.
			List<Patient> listPats=Patients.GetMultPats(listMergedPats.Where(x => x.PatNumTo!=patCur.PatNum)
					.Select(x => x.PatNumTo).ToList()).ToList();
			//Notify the user that the currently selected patient has been merged before and then ask them if they want to switch to the correct patient.
			foreach(Patient pat in listPats) {
				if(ListTools.In(pat.PatStatus,PatientStatus.Patient,PatientStatus.Inactive)
					&& (MessageBox.Show(Lan.g("ContrAppt","The currently selected patient has been merged into another patient.\r\n"
					+"Switch to patient")+" "+pat.GetNameLF()+" #"+pat.PatNum.ToString()+"?","",MessageBoxButtons.YesNo)==DialogResult.Yes)) 
				{
					newPatCur=pat;
					return true;
				}
			}
			//The user has declined every possible patient that the current patient was merged to.  Let them keep the merge from patient selected.
			return false;
		}

		///<summary></summary>
		public static PlannedApptStatus CreatePlannedAppt(Patient pat,int itemOrder,List<long> listPreSelectedProcNums=null) {
			if(pat==null) {
				MsgBox.Show("Appointments","Error creating planned appointment.  No patient is currently selected.");
				return PlannedApptStatus.Failure;
			}
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return PlannedApptStatus.Failure;
			}
			if(PatRestrictionL.IsRestricted(pat.PatNum,PatRestrict.ApptSchedule)) {
				return PlannedApptStatus.Failure;
			}
			if(PromptForMerge(pat,out pat)) {
				FormOpenDental.S_Contr_PatientSelected(pat,true,false);
			}
			if(ListTools.In(pat.PatStatus,PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show("Appointments","Appointments cannot be scheduled for "+pat.PatStatus.ToString().ToLower()+" patients.");
				return PlannedApptStatus.Failure;
			}
			Appointment AptCur=new Appointment();
			AptCur.PatNum=pat.PatNum;
			AptCur.ProvNum=pat.PriProv;
			AptCur.ClinicNum=pat.ClinicNum;
			AptCur.AptStatus=ApptStatus.Planned;
			AptCur.AptDateTime=DateTime.Today;
			List<Procedure> listProcs=Procedures.GetManyProc(listPreSelectedProcNums,false);//Returns empty list if null.
			//If listProcs is empty then AptCur.Pattern defaults to PrefName.AppointmentWithoutProcsDefaultLength value.
			//See Appointments.GetApptTimePatternForNoProcs().
			AptCur.Pattern=Appointments.CalculatePattern(pat,AptCur.ProvNum,AptCur.ProvHyg,listProcs);
			AptCur.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			Appointments.Insert(AptCur);
			PlannedAppt plannedAppt=new PlannedAppt();
			plannedAppt.AptNum=AptCur.AptNum;
			plannedAppt.PatNum=pat.PatNum;
			plannedAppt.ItemOrder=itemOrder;
			PlannedAppts.Insert(plannedAppt);
			using FormApptEdit FormApptEdit=new FormApptEdit(AptCur.AptNum,listPreSelectedProcNums:listPreSelectedProcNums);
			FormApptEdit.IsNew=true;
			FormApptEdit.ShowDialog();
			if(FormApptEdit.DialogResult!=DialogResult.OK){
				return PlannedApptStatus.FillGridNeeded;
			}
			//Only set the appointment hygienist to this patient's secondary provider if one was not manually set within the edit window.
			if(AptCur.ProvHyg < 1) {
				List<Procedure> myProcList=Procedures.GetProcsForSingle(AptCur.AptNum,true);
				bool allProcsHyg=(myProcList.Count>0 && myProcList.Select(x=>ProcedureCodes.GetProcCode(x.CodeNum)).ToList().All(x=>x.IsHygiene));
				//Automatically set the appointments hygienist to the secondary provider of the patient if one is set.
				if(allProcsHyg && pat.SecProv!=0) {
					Appointment aptOld=AptCur.Copy();
					AptCur.ProvNum=pat.SecProv;
					Appointments.Update(AptCur,aptOld);
				}
			}
			Patient patOld=pat.Copy();
			pat.PlannedIsDone=false;
			Patients.Update(pat,patOld);
			FormOpenDental.S_RefreshCurrentModule(isClinicRefresh:false);//if procs were added in appt, then this will display them
			return PlannedApptStatus.Success;
		}

		/// <summary>Checks for specialty mismatch between pat and op. Then prompts user according to behavior defined by 
		/// PrefName.ApptSchedEnforceSpecialty.  Returns true if the Appointment is allowed to be scheduled, false otherwise.</summary>
		public static bool IsSpecialtyMismatchAllowed(long patNum,long clinicNum) {
			try {
				Appointments.HasSpecialtyConflict(patNum,clinicNum);//throws exception if we need to prompt user
			}
			catch(ODException odex) {//Warn/Block
				switch((ApptSchedEnforceSpecialty)odex.ErrorCode) {
					case ApptSchedEnforceSpecialty.Warn:
						if(!MsgBox.Show("Appointment",MsgBoxButtons.YesNo,odex.Message+"\r\nSchedule appointment anyway?","Specialty Mismatch")){
							return false;
						}
						break;
					case ApptSchedEnforceSpecialty.Block:
						MsgBox.Show("Appointment",odex.Message,"Specialty Mismatch");
						return false;
				}
			}
			return true;
		}

		/// <summary>Tests the appointment to see if it is acceptable to send it to the pinboard.  Also asks user appropriate questions to verify that's
		/// what they want to do.  Returns false if it will not be going to pinboard after all.</summary>
		public static bool OKtoSendToPinboard(ApptOther AptCur,List<ApptOther> listApptOthers,Control owner) {
			if(AptCur.AptStatus==ApptStatus.Planned) {//if is a Planned appointment
				bool PlannedIsSched=false;
				for(int i=0;i<listApptOthers.Count;i++) {
					if(listApptOthers[i].NextAptNum==AptCur.AptNum) {//if the planned appointment is already sched
						PlannedIsSched=true;
					}
				}
				if(PlannedIsSched) {
					if(!MsgBox.Show(owner,MsgBoxButtons.OKCancel,"The Planned appointment is already scheduled.  Do you wish to continue?")) {
						return false;
					}
				}
			}
			else{//if appointment is not Planned
				switch(AptCur.AptStatus) {
					case ApptStatus.Complete:
						MsgBox.Show(owner,"Not allowed to move a completed appointment from here.");
						return false;
					case ApptStatus.Scheduled:
						if(!MsgBox.Show(owner,MsgBoxButtons.OKCancel,"Do you really want to move a previously scheduled appointment?")) {
							return false;
						}
						break;
					case ApptStatus.Broken://status gets changed after dragging off pinboard.
					case ApptStatus.None:
					case ApptStatus.UnschedList://status gets changed after dragging off pinboard.
						break;
				}
			}
			//if it's a planned appointment, the planned appointment will end up on the pinboard.  The copy will be made after dragging it off the pinboard.
			return true;
		}

		///<summary>If changing to 'Arrived' trigger, check if using eClipboard and setup to popup Kiosk Manager, or attempt to process the patient's
		///arrival as if they had texted 'A' to indicate 'Arrived'.</summary>
		public static void ShowKioskManagerIfNeeded(Appointment oldAppt,long newConfirmed) {
			if(newConfirmed==oldAppt.Confirmed || newConfirmed!=PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)) {
				//If no change to Appointment.Confirmed, or new status is not the Arrived trigger, nothing to do.
				return;
			}
			long clinicNum=PrefC.HasClinicsEnabled ? oldAppt.ClinicNum : 0;
			if(MobileAppDevices.IsClinicSignedUpForEClipboard(clinicNum) && ClinicPrefs.GetBool(PrefName.EClipboardPopupKioskOnCheckIn,clinicNum)) {
				Appointment newAppt=oldAppt.Copy();
				newAppt.Confirmed=newConfirmed;
				using FormTerminalManager formTM=new FormTerminalManager(appt:newAppt);
				formTM.ShowDialog();
			}
			else {
				//Manually marked as Arrived, if they had been sent an Arrival sms, try to process and send the Arrival Response.
				//Pass oldAppt so it still has the old confirmation status; which has already updated in db.
				OpenDentBusiness.AutoComm.Arrivals.ProcessArrival(oldAppt.PatNum,oldAppt.ClinicNum,ListTools.FromSingle(oldAppt));
			}
		}

		///<summary>Attempts to send a BYOD/Check-In link for the given appointment.  Prompts user appropriately on success(Text Message winodw) or 
		///failure.</summary>
		public static void SendByodLink(Appointment appt) {
			void displayError(string error) {
				MsgBox.Show(nameof(OpenDentBusiness.AutoComm.Byod),$"Unable to {MenuItemNames.SendEClipboardByod}.  "+error);
			}
			string message=null;
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => {
					if(appt is null) {
						throw new ODException(Lan.g(nameof(OpenDentBusiness.AutoComm.Byod),"Unable to send for an invalid appointment."));
					}
					long clinicNum=PrefC.HasClinicsEnabled ? appt.ClinicNum : 0;
					if(OpenDentBusiness.AutoComm.Byod.IsEnabledForConfirmed(appt.Confirmed,appt.ClinicNum,out string err)) {
						List<PatComm> listPatComms=Patients.GetPatComms(new List<long> { appt.PatNum },Clinics.GetClinic(clinicNum),isGetFamily:false);
						//This is not automated messaging, so we don't need to consider CommOptOuts, just IsSmsAnOption
						if(!(listPatComms.FirstOrDefault(x => x.PatNum==appt.PatNum)?.IsSmsAnOption??false)) {
							throw new ODException(Lan.g(nameof(OpenDentBusiness.AutoComm.Byod),"Patient is not setup to receive text messages."));
						}
						message=OpenDentBusiness.AutoComm.Byod.GetCheckInMsg(new List<Appointment> { appt },listPatComms);						
					}
					else {
						throw new ODException(err);
					}
				};
				progressOD.StartingMessage=Lans.g(nameof(OpenDentBusiness.AutoComm.Byod),"Generating eClipboard links...");
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return;
				}
				if(string.IsNullOrWhiteSpace(message)) {
					throw new ODException(Lan.g(nameof(OpenDentBusiness.AutoComm.Byod),"Unable to generate links for the appointment."));
				}
				FormOpenDental.S_TxtMsg_Click(appt.PatNum,message);
			}
			catch(ODException odex) {
				displayError(odex.Message);
			}
			catch(Exception ex) {
				displayError("An error occurred:\n"+MiscUtils.GetExceptionText(ex));
			}
		}
        
		///<summary>Returns true if the PrefName.ApptPreventChangesToCompleted is true, the appointment has completed procedures attached, and the appointment 
		///has a completed status. Otherwise it will return false.</summary>
		public static bool DoPreventChangesToCompletedAppt(Appointment apt,PreventChangesApptAction action,List<Procedure> listAttachedProcs=null) {
			return DoPreventChangesToCompletedAppt(apt,action,out string msg,listAttachedProcs,false);
    }

		///<summary>Returns true if the PrefName.ApptPreventChangesToCompleted is true, the appointment has completed procedures attached, and the appointment 
		///has a completed status. Otherwise it will return false.</summary>
		public static bool DoPreventChangesToCompletedAppt(Appointment apt,PreventChangesApptAction action,out string msg,List<Procedure> listAttachedProcs=null) {
			return DoPreventChangesToCompletedAppt(apt,action,out msg,listAttachedProcs,true);
    }
        
		///<summary>Returns true if the PrefName.ApptPreventChangesToCompleted is true, the appointment has completed procedures attached, and the appointment 
		///has a completed status. Otherwise it will return false.</summary>
		public static bool DoPreventChangesToCompletedAppt(Appointment apt,PreventChangesApptAction action,out string msg,List<Procedure> listAttachedProcs=null,bool doSupressMsg=false) {
      msg=null;
			if(apt==null) {
				return true;
			}
			if(!PrefC.GetBool(PrefName.ApptPreventChangesToCompleted) //The preference is turned off.
				|| (apt.AptStatus!=ApptStatus.Complete)//The appointment status is not complete. Allow changes.
				|| !Appointments.HasCompletedProcsAttached(apt.AptNum,listAttachedProcs))//Apt does not have completed procedures attached
			{
				return false;
			}
			//Prepare the message text that the user will see before we return true.
			string strAction;
			switch(action) {
				case PreventChangesApptAction.Break:
					strAction="break the appointment";
					break;
				case PreventChangesApptAction.Delete:
					strAction="delete the appointment";
					break;
				case PreventChangesApptAction.Status:
					strAction="change the appointment status";
					break;
				case PreventChangesApptAction.Procedures:
					strAction="detach completed procedures";
						break;
				default://CompletedApptAction.Unsched
					strAction="send the appointment to the unscheduled list";
					break;
			}
      msg=$"Not allowed to {strAction} when there are completed procedures attached to this appointment.\r\n\r\nChange the status of the "
				+"completed procedures to Treatment Planned if they were not completed, or delete the procedures before trying again.";
      if(!doSupressMsg){
				MsgBox.Show(msg);
      }
			return true;
		}
	}

	public enum PlannedApptStatus {
    ///<summary>1 - Used when failed validation.</summary>
    Failure,
    ///<summary>2 - Used when planned appt was created.</summary>
    Success,
		///<summary>3 - Used when planned appt was not created but we might need to fill a grid.</summary>
    FillGridNeeded
  }

	///<summary>Used with the PrefName.ApptPreventChangesToCompleted to determine if the action taken on an appointment is allowed. These
	///are the only actions we care about.</summary>
	public enum PreventChangesApptAction {
		///<summary>0 - Used when a completed appointment is broken.</summary>
		Break,
		///<summary>1 - Used when a completed appointment is deleted.</summary>
		Delete,
		///<summary>2 - Used when a completed apopintment status is changed.</summary>
		Status,
		///<summary>3 - Used when a completed apopintment is sent to the unscheduled list.</summary>
		Unsched,
		///<summary>4 - Used when attempting to detach a completed proc from a completed appt.</summary>
		Procedures
	}
}
