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
		public static Appointment CreateRecallApt(Patient patient,List<InsPlan> listInsPlans,long recallNum,List<InsSub> listInsSubs
			,DateTime dateAppt=default(DateTime))
		{
			List<Recall> listRecalls=Recalls.GetList(patient.PatNum);
			Recall recall=null;
			if(recallNum>0) {
				recall=Recalls.GetRecall(recallNum);
			}
			else{
				for(int i=0;i<listRecalls.Count;i++){
					if(listRecalls[i].RecallTypeNum==RecallTypes.PerioType || listRecalls[i].RecallTypeNum==RecallTypes.ProphyType){
						if(!listRecalls[i].IsDisabled){
							recall=listRecalls[i];
						}
						break;
					}
				}
			}
			if(recall==null){
				//Typically never happens because everyone has a recall.  However, it can happen when patients have custom recalls due
				throw new ApplicationException(Lan.g("AppointmentL","No special type recall is due."));
			}
			if(recall.DateScheduled.Date>DateTime.Today) {
				throw new ApplicationException(Lan.g("AppointmentL","Recall has already been scheduled for ")+recall.DateScheduled.ToShortDateString());
			}
			Appointment appointment=new Appointment();
			appointment.AptDateTime=dateAppt;		
			List<string> listProcCodes=RecallTypes.GetProcs(recall.RecallTypeNum);
			List<Procedure> listProcedures=Appointments.FillAppointmentForRecall(appointment,recall,listRecalls,patient,listProcCodes,listInsPlans,listInsSubs);
			return appointment;
		}

		///<summary>Returns true if PrefName.BrokenApptProcedure is greater than 0.</summary>
		public static bool HasBrokenApptProcs() {
			return PrefC.GetLong(PrefName.BrokenApptProcedure)>0;
		}

		///<summary>Checks if one or multiple procedure(s) being deleted are required for the appointment type of an attached appointment. Accepts single procedure or list of multiple procedures as an array. The appointment type and list of procedures for the appointment may optionally be passed in if known but that will likely only be the case on FormApptEdit.cs</summary>
		public static string CheckRequiredProcForApptType(params Procedure[] procedureArrayToDelete){
			List<Procedure> listProceduresToDelete=procedureArrayToDelete.ToList();
			List<long> listApptNums=listProceduresToDelete.Select(x => x.AptNum).Distinct().ToList();
			listApptNums.AddRange(listProceduresToDelete.Select(x => x.PlannedAptNum).Distinct().ToList());
			listApptNums.RemoveAll(x => x<=0);
			List<Appointment> listAppointments=Appointments.GetMultApts(listApptNums);//Create a list of appointments to iterate through.
			List<AppointmentType> listAppointmentTypes=AppointmentTypes.GetDeepCopy();
			List<Procedure> listProceduresMultAppt=Procedures.GetProcsMultApts(listApptNums);//Will be needed for Procedures.GetProcsOneApt(...)
			List<string> listAppointmentTypeNames=new List<string>();
			List<string> listRequiredProcs=new List<string>();
			string allOrSome=""; //Will be used for the warning message.
			for(int a=0;a<listAppointments.Count();a++){
				AppointmentType appointmentType=listAppointmentTypes.Find(x => x.AppointmentTypeNum==listAppointments[a].AppointmentTypeNum);
				if(appointmentType==null || appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.None){//If the appt does not have an appttype, or appttype does not need any of the required proc codes to be attached.
					continue;	
				}
				List<long> listProcNumsToDelete=listProceduresToDelete.FindAll(x => x.AptNum==listAppointments[a].AptNum || x.PlannedAptNum==listAppointments[a].AptNum).Select(y => y.ProcNum).ToList();
				List<Procedure> listProceduresForAppt=Procedures.GetProcsOneApt(listAppointments[a].AptNum, listProceduresMultAppt);
				listProceduresForAppt.RemoveAll(x => listProcNumsToDelete.Contains(x.ProcNum));//Remove the procs we intend to delete from the Procedures on Appointment grid to simulate if the procs are successfully deleted.
				List<long> listCodeNumsRemaining=listProceduresForAppt.Select(x => x.CodeNum).ToList();
				List<string> listStrProcCodesRemaining = new List<string>();
				for(int p=0;p<listCodeNumsRemaining.Count();p++){
					listStrProcCodesRemaining.Add(ProcedureCodes.GetProcCode(listCodeNumsRemaining[p]).ProcCode);
				}
				//All procs in grid, minus for other appts, minus selected is simulated result. Verify simulated result meets appointment type requirements and if not then show message.
				bool isMissingRequiredProcs=false;
				int requiredCodesAttached=0;
				List<string> listProcCodesRequiredForApptType=appointmentType.CodeStrRequired.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();//Includes duplicates.
				for(int t=0;t<listProcCodesRequiredForApptType.Count;t++) {
					if(listStrProcCodesRemaining.Contains(listProcCodesRequiredForApptType[t])) {
						requiredCodesAttached++;
						listStrProcCodesRemaining.Remove(listProcCodesRequiredForApptType[t]);
					}
				}
				if(appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.All &&  requiredCodesAttached!=listProcCodesRequiredForApptType.Count) {
					isMissingRequiredProcs=true;
				}
				if(appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.AtLeastOne && requiredCodesAttached==0) {
					isMissingRequiredProcs=true;
				}
				//Gather Appointment Type Name(s) and Required Procedure(s) to use on message.
				if(isMissingRequiredProcs && !listAppointmentTypeNames.Any(x => x==appointmentType.AppointmentTypeName)){
					listAppointmentTypeNames.Add(appointmentType.AppointmentTypeName);
					listRequiredProcs.AddRange(listProcCodesRequiredForApptType);
					//Update allOrSome with the string that corresponds with its EnumRequiredProcCodesNeeded. It will be used on the warning message if there is just one appointment in the list.
					allOrSome=(appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.All)?"all":"at least one";
				}
			}
			if(listAppointmentTypeNames.Count==0){
				return "";
			}
			//The Appointment Type(s) [name(s)] require(s) [at least one | all] of the following procedures to be attached: proc1, proc2.
			if(listAppointmentTypeNames.Count>1){
				allOrSome="all or some"; //Default to this general string if there is more than one appointment in the list as there could be a mix of appointment types.
			}
			string errorMessage=("Appointment Type(s)"+" \""+String.Join("\", \"",listAppointmentTypeNames)+"\" "+"requires "+allOrSome+" of the following procedures:"
				+"\r\n"+String.Join(", ",listRequiredProcs)
				+"\r\n\n"+"To delete these procedures change the Appointment Type to None.");
			return errorMessage;
		}

		///<summary>Sets given appt.AptStatus to broken.
		///Provide procCode that should be charted, can be null but will not chart a broken procedure.
		///Also considers various broken procedure based prefs.
		///Makes its own securitylog entries.</summary>
		public static void BreakApptHelper(Appointment appointment,Patient patient,ProcedureCode procedureCode) {
			//suppressHistory is true due to below logic creating a log with a specific HistAppointmentAction instead of the generic changed.
			DateTime datePrevious=appointment.DateTStamp;
			bool suppressHistory=false;
			if(procedureCode!=null) {
				suppressHistory=(procedureCode.ProcCode.In("D9986","D9987"));
			}
			Appointments.SetAptStatus(appointment,ApptStatus.Broken,suppressHistory); //Appointments S-Class handles Signalods
			if(appointment.AptStatus!=ApptStatus.Complete) { //seperate log entry for completed appointments.
				SecurityLogs.MakeLogEntry(EnumPermType.AppointmentEdit,patient.PatNum,
					appointment.ProcDescript+", "+appointment.AptDateTime.ToString()
					+", Broken from the Appts module.",appointment.AptNum,datePrevious);
			}
			else {
				SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCompleteEdit,patient.PatNum,
					appointment.ProcDescript+", "+appointment.AptDateTime.ToString()
					+", Broken from the Appts module.",appointment.AptNum,datePrevious);
			}
			#region HL7
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				//S15 - Appt Cancellation event
				MessageHL7 messageHL7=MessageConstructor.GenerateSIU(patient,Patients.GetPat(patient.Guarantor),EventTypeHL7.S15,appointment);
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=appointment.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=patient.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MessageBox.Show("Appointments",messageHL7.ToString());
					}
				}
			}
			#endregion
			#region HieQueue
			if(HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(patient.PatNum));
			}
			#endregion
			List<Procedure> listProcedures=new List<Procedure>();
			//splits should only exist on procs if they are using tp pre-payments
			List<PaySplit> listPaySplitsForApptProcs=new List<PaySplit>();
			bool isNonRefundable=false;
			double amtBrokenProc=0;
			Procedure procedureBroken=new Procedure();
			bool wasBrokenProcDeleted=false;
			if(PrefC.GetYN(PrefName.PrePayAllowedForTpProcs)) {
				listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,false);
				if(listProcedures.Count > 0) {
					listPaySplitsForApptProcs=PaySplits.GetPaySplitsFromProcs(listProcedures.Select(x => x.ProcNum).ToList());
				}
			}
			#region Charting the proc
			if(procedureCode!=null) {
				switch(procedureCode.ProcCode) { 
					case "D9986"://Missed
						HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Missed);
					break;
					case "D9987"://Cancelled
						HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Cancelled);
					break;
				}
				procedureBroken.PatNum=patient.PatNum;
				procedureBroken.ProvNum=(appointment.ProvNum);
				if(procedureCode.ProvNumDefault>0){
					procedureBroken.ProvNum=(procedureCode.ProvNumDefault);
				}
				procedureBroken.CodeNum=procedureCode.CodeNum;
				procedureBroken.ProcDate=DateTime.Today;
				procedureBroken.DateEntryC=DateTime.Now;
				procedureBroken.DateTP=DateTime.Today;
				procedureBroken.ProcStatus=ProcStat.C;
				procedureBroken.ClinicNum=appointment.ClinicNum;
				procedureBroken.UserNum=Security.CurUser.UserNum;
				procedureBroken.Note=Lans.g("AppointmentEdit","Appt BROKEN for")+" "+appointment.ProcDescript+"  "+appointment.AptDateTime.ToString();
				procedureBroken.PlaceService=Clinics.GetPlaceService(procedureBroken.ClinicNum);
				List<InsSub> listInsSubs=InsSubs.RefreshForFam(Patients.GetFamily(patient.PatNum));
				List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
				List<PatPlan> listPatPlans=PatPlans.Refresh(patient.PatNum);
				InsPlan insPlanPrimary=null;
				InsSub insSubPrimary=null;
				if(listPatPlans.Count>0) {
					insSubPrimary=InsSubs.GetSub(listPatPlans[0].InsSubNum,listInsSubs);
					insPlanPrimary=InsPlans.GetPlan(insSubPrimary.PlanNum,listInsPlans);
				}
				double procFee;
				long feeSchedNumPat;
				bool noBillIns=InsPlanPreferences.NoBillIns(procedureCode,insPlanPrimary);
				if(insPlanPrimary==null || noBillIns) {
					feeSchedNumPat=FeeScheds.GetFeeSched(0,patient.FeeSched,procedureBroken.ProvNum);
				}
				else {//Only take into account the patient's insurance fee schedule if the D9986 procedure is not marked as NoBillIns
					feeSchedNumPat=FeeScheds.GetFeeSched(insPlanPrimary.FeeSched,patient.FeeSched,procedureBroken.ProvNum);
				}
				procFee=Fees.GetAmount0(procedureBroken.CodeNum,feeSchedNumPat,procedureBroken.ClinicNum,procedureBroken.ProvNum);
				if(insPlanPrimary!=null&&insPlanPrimary.PlanType=="p"&&!insPlanPrimary.IsMedical) {//PPO
					double provFee=Fees.GetAmount0(procedureBroken.CodeNum,Providers.GetProv(procedureBroken.ProvNum).FeeSched,procedureBroken.ClinicNum,
					procedureBroken.ProvNum);
					procedureBroken.ProcFee=Math.Max(provFee,procFee);
				}
				else if(listPaySplitsForApptProcs.Count>0 && PrefC.GetBool(PrefName.TpPrePayIsNonRefundable) && procedureCode.ProcCode=="D9986") {
					//if there are pre-payments, non-refundable pre-payments is turned on, and the broken appointment is a missed code then auto-fill 
					//the window with the sum of the procs for the appointment. Transfer money below after broken procedure is confirmed by the user.
					procedureBroken.ProcFee=listPaySplitsForApptProcs.Sum(x => x.SplitAmt);
					isNonRefundable=true;
				}
				else {
					procedureBroken.ProcFee=procFee;
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					procedureBroken.SiteNum=patient.SiteNum;
				}
				Procedures.Insert(procedureBroken);
				//Now make a claimproc if the patient has insurance.  We do this now for consistency because a claimproc could get created in the future.
				List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
				List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForProc(procedureBroken.ProcNum);
				Procedures.ComputeEstimates(procedureBroken,patient.PatNum,listClaimProcs,false,listInsPlans,listPatPlans,listBenefits,patient.Age,listInsSubs);
				using FormProcBroken formProcBroken=new FormProcBroken(procedureBroken,isNonRefundable);
				formProcBroken.IsNew=true;
				formProcBroken.ShowDialog();
				amtBrokenProc=formProcBroken.GetAmountTotal();
				wasBrokenProcDeleted=formProcBroken.IsProcDeleted;
				Procedures.AfterProcsSetComplete(ListTools.FromSingle(procedureBroken));
			}
			#endregion
			#region BrokenApptAdjustment
			if(PrefC.GetBool(PrefName.BrokenApptAdjustment)) {
				Adjustment adjustment=new Adjustment();
				adjustment.DateEntry=DateTime.Today;
				adjustment.AdjDate=DateTime.Today;
				adjustment.ProcDate=DateTime.Today;
				adjustment.ProvNum=appointment.ProvNum;
				adjustment.PatNum=patient.PatNum;
				adjustment.AdjType=PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType);
				adjustment.ClinicNum=appointment.ClinicNum;
				using FormAdjust formAdjust=new FormAdjust(patient,adjustment);
				formAdjust.IsNew=true;
				formAdjust.ShowDialog();
			}
			#endregion
			#region BrokenApptCommLog
			if(PrefC.GetBool(PrefName.BrokenApptCommLog)) {
				Commlog commLog=new Commlog();
				commLog.PatNum=patient.PatNum;
				commLog.CommDateTime=DateTime.Now;
				commLog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
				commLog.Note=Lan.g("Appointment","Appt BROKEN for")+" "+appointment.ProcDescript+"  "+appointment.AptDateTime.ToString();
				commLog.Mode_=CommItemMode.None;
				commLog.UserNum=Security.CurUser.UserNum;
				commLog.IsNew=true;
				using FormCommItem formCommItem=new FormCommItem(commLog);
				formCommItem.ShowDialog();
			}
			#endregion
			#region Transfer money from TP Procedures if necessary
			//Note this MUST come after FormProcBroken since clicking cancel in that window will delete the procedure.
			if(isNonRefundable && !wasBrokenProcDeleted && listPaySplitsForApptProcs.Count>0) {
				//transfer what the user specified in the broken appointment window.
				//transfer up to the amount specified by the user
				for(int p=0;p<listProcedures.Count;p++){
					if(amtBrokenProc==0) {
						break;
					}
					List<PaySplit> listPaySplits=listPaySplitsForApptProcs.FindAll(x => x.ProcNum==listProcedures[p].ProcNum);
					for(int s=0;s<listPaySplits.Count;s++){
						if(amtBrokenProc==0) {
							break;
						}							
						double splitAmt=Math.Min(amtBrokenProc,listPaySplits[s].SplitAmt);
						Payments.CreateTransferForTpProcs(listProcedures[p],new List<PaySplit>{listPaySplits[s]},procedureBroken,splitAmt);
						double amtPaidOnApt=listPaySplitsForApptProcs.Sum(x => x.SplitAmt);
						if(amtPaidOnApt>splitAmt) {
							//If the original prepayment amount is greater than the amt being specified for the appointment break, transfer
							//the difference to an Unallocated Unearned Paysplit on the account.
							double remainingAmt=amtPaidOnApt-splitAmt;
							//We have to create a new transfer payment here to correlate to the split.
							Payment payment=new Payment();
							payment.PayAmt=0;
							payment.PayDate=DateTime.Today;
							payment.ClinicNum=listPaySplits[s].ClinicNum;
							payment.PayNote="Automatic transfer from treatment planned procedure prepayment.";
							payment.PatNum=listPaySplits[s].PatNum;//ultimately where the payment ends up.
							payment.PayType=0;
							Payments.Insert(payment);
							PaymentEdit.IncomeTransferData incomeTransferData=PaymentEdit.IncomeTransferData.CreateTransfer(listPaySplits[s],payment.PayNum,true,remainingAmt);
							PaySplits.InsertMany(payment.PayNum,incomeTransferData.ListSplitsCur);
							SecurityLogs.MakeLogEntry(EnumPermType.PaymentCreate,payment.PatNum,"Automatic transfer of funds for treatment plan procedure pre-payments.");
							Signalods.SetInvalid(InvalidType.BillingList);
						}
						amtBrokenProc-=splitAmt;
					}
				}
			}
			//if broken appointment procedure was deleted (user cancelled out of the window) just keep money on the original procedure.
			#endregion
			AppointmentEvent.Fire(ODEventType.AppointmentEdited,appointment);
			AutomationL.Trigger(EnumAutomationTrigger.ApptBreak,null,patient.PatNum);
			Recalls.SynchScheduledApptFull(appointment.PatNum);
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

		///<summary>Throw exceptions. Calculates the start and end time that is used by websched to offer an open timeslot to patients on the ASAP List.</summary>
		public static DateRange GetAsapRange(long operatoryNum,DateTime dateTimeSelected,long aptNum,List<Schedule> listSchedules) {
			DateTime dateTimeSlotStart=dateTimeSelected.Date;//Midnight contrAppt=dateTimeClicked not DateSelected
			DateTime dateTimeSlotEnd=dateTimeSelected.Date.AddDays(1);//Midnight tomorrow
			//Get appoinments for the period of the entire day of dateTimeSelected
			List<Appointment> listAppointments=Appointments.GetAppointmentsForOpsByPeriod(new List<long> { operatoryNum },dateTimeSelected,dateTimeSelected);
			for(int i=0;i<listAppointments.Count;i++) {
				Appointment appointment=listAppointments[i];
				if(appointment.AptNum==aptNum || appointment.AptDateTime.Date!=dateTimeSelected.Date) {
					continue;
				}
				DateTime dateEndApt=appointment.AptDateTime.AddMinutes(appointment.Pattern.Length*5);
				if(dateEndApt.Between(dateTimeSlotStart,dateTimeSelected)) {
					dateTimeSlotStart=dateEndApt;
				}
				if(appointment.AptDateTime.Between(dateTimeSelected,dateTimeSlotEnd)) {
					dateTimeSlotEnd=appointment.AptDateTime;
				}
			}
			//Next, loop through blockouts that don't allow scheduling and adjust the slot size
			List<Def> listDefsBlockoutTypes=Defs.GetDefsForCategory(DefCat.BlockoutTypes);
			List<Schedule> listSchedulesBlockouts = Schedules.GetListForType(listSchedules,ScheduleType.Blockout,0)
				.Where(x => x.SchedDate==dateTimeSelected.Date && x.Ops.Contains(operatoryNum)).ToList();
			for(int i=0;i<listSchedulesBlockouts.Count;i++){
				if(Schedules.CanScheduleInBlockout(listSchedulesBlockouts[i].BlockoutType,listDefsBlockoutTypes)) {
					continue;
				}
				DateTime dateTimeBlockoutStop=listSchedulesBlockouts[i].SchedDate.Add(listSchedulesBlockouts[i].StopTime);
				if(dateTimeBlockoutStop.Between(dateTimeSlotStart,dateTimeSelected)) {
					//Move start time to be later to account for this blockout.
					dateTimeSlotStart=dateTimeBlockoutStop;
				}
				DateTime dateTimeBlockoutStart=listSchedulesBlockouts[i].SchedDate.Add(listSchedulesBlockouts[i].StartTime);
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

		public static bool ValidateApptToPinboard(Appointment appointment) {
			if(!Security.IsAuthorized(EnumPermType.AppointmentMove)) {
				return false;
			}
			if(appointment.AptStatus==ApptStatus.Complete) {
				MsgBox.Show("Appointments","Not allowed to move completed appointments.");
				return false;
			}
			if(PatRestrictionL.IsRestricted(appointment.PatNum,PatRestrict.ApptSchedule)) {
				return false;
			}
			return true;
		}

		/// <summary>Helper method to send given appt to pinboard.
		/// Refreshes Appointment module.
		/// Also does some appointment and security validation.</summary>
		public static void CopyAptToPinboardHelper(Appointment appointment) {
			GotoModule.PinToAppt(new List<long>() { appointment.AptNum },appointment.PatNum);
		}

		public static bool ValidateApptUnsched(Appointment appointment) {
			//separate permissions for complete appts.
			if(appointment.AptStatus==ApptStatus.Complete && !Security.IsAuthorized(EnumPermType.AppointmentCompleteEdit))
			{
				return false;
			}
			if(appointment.AptStatus!=ApptStatus.Complete && !Security.IsAuthorized(EnumPermType.AppointmentMove)){
				return false;
			}
			if(PatRestrictionL.IsRestricted(appointment.PatNum,PatRestrict.ApptSchedule)) {
				return false;
			}
			if(appointment.AptStatus==ApptStatus.PtNote | appointment.AptStatus==ApptStatus.PtNoteCompleted) {
				return false;
			}
			return true;
		}

		/// <summary>Helper method to send given appt to the unscheduled list.
		/// Creates SecurityLogs and considers HL7.</summary>
		public static void SetApptUnschedHelper(Appointment appointment,Patient patient=null,bool doFireApptEvent=true) {
			DateTime datePrevious=appointment.DateTStamp;
			Appointments.SetAptStatus(appointment,ApptStatus.UnschedList); //Appointments S-Class handles Signalods
			#region SecurityLogs
			if(appointment.AptStatus!=ApptStatus.Complete) { //seperate log entry for editing completed appts.
				SecurityLogs.MakeLogEntry(EnumPermType.AppointmentMove,appointment.PatNum,
					appointment.ProcDescript+", "+appointment.AptDateTime.ToString()+", Sent to Unscheduled List",
					appointment.AptNum,datePrevious);
			}
			else {
				SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCompleteEdit,appointment.PatNum,
					appointment.ProcDescript+", "+appointment.AptDateTime.ToString()+", Sent to Unscheduled List",
					appointment.AptNum,datePrevious);
			}
			#endregion
			#region HL7
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				if(patient==null) {
					patient=Patients.GetPat(appointment.PatNum);
				}
				//S15 - Appt Cancellation event
				MessageHL7 messageHL7=MessageConstructor.GenerateSIU(patient,Patients.GetPat(patient.Guarantor),EventTypeHL7.S15,appointment);
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=appointment.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=patient.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						MessageBox.Show("Appointments",messageHL7.ToString());
					}
				}
			}
			#endregion
			#region HieQueue
			if(HieClinics.IsEnabled()) {
				if(patient==null) {
					patient=Patients.GetPat(appointment.PatNum);
				}
				HieQueues.Insert(new HieQueue(patient.PatNum));
			}
			#endregion
			if(doFireApptEvent) {
				AppointmentEvent.Fire(ODEventType.AppointmentEdited,appointment);
			}
			Recalls.SynchScheduledApptFull(appointment.PatNum);
		}

		///<summary>Creats a new appointment for the given patient.  A valid patient must be passed in.
		///Set useApptDrawingSettings to true if the user double clicked on the appointment schedule in order to make a new appointment.
		///It will utilize the global static properties to help set required fields for "Scheduled" appointments.
		///Otherwise, simply sets the corresponding PatNum and then the status to "Unscheduled".</summary>
		public static Appointment MakeNewAppointment(Patient patient,bool useApptDrawingSettings,DateTime? dateTNew=null,long? opNumNew=null) {
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
			Appointment appointment=Appointments.MakeNewAppointment(patient,apptDateTime,opNum,useApptDrawingSettings);
			if(patient.AskToArriveEarly>0 && useApptDrawingSettings) {
				MessageBox.Show(Lan.g("FormApptsOther","Ask patient to arrive")+" "+patient.AskToArriveEarly
					+" "+Lan.g("FormApptsOther","minutes early at")+" "+appointment.DateTimeAskedToArrive.ToShortTimeString()+".");
			}
			return appointment;
		}

		///<summary>Checks to see if patient was previously merged. If so, prompts asking if you'd like to switch and returns the switched patient, otherwise returns null.</summary>
		public static Patient GetPatientMergePrompt(long patNum) {
			List<PatientLink> listPatientLinks=PatientLinks.GetLinks(patNum,PatientLinkType.Merge);
			if(!PatientLinks.WasPatientMerged(patNum,listPatientLinks)) {
				return null;
			}
			//This patient has been merged before.  Get a list of all patients that this patient has been merged into.
			List<Patient> listPatients=Patients.GetMultPats(listPatientLinks.Where(x => x.PatNumTo!=patNum).Select(x => x.PatNumTo).ToList()).ToList();
			//Notify the user that the currently selected patient has been merged before and then ask them if they want to switch to the correct patient.
			for(int i=0;i<listPatients.Count;i++) {
				if(!listPatients[i].PatStatus.In(PatientStatus.Patient,PatientStatus.Inactive)) {
					continue;
				}
				if(MsgBox.Show(MsgBoxButtons.YesNo,
					Lan.g("ContrAppt","The currently selected patient has been merged into another patient.\r\nSwitch to patient")
					+" "+listPatients[i].GetNameLF()+" #"+listPatients[i].PatNum.ToString()+"?")) {
					return listPatients[i];
				}
			}
			//The user has declined every possible patient that the current patient was merged to.  Let them keep the merge from patient selected.
			return null;
		}

		///<summary></summary>
		public static PlannedApptStatus CreatePlannedAppt(Patient patient,int itemOrder,List<long> listPreSelectedProcNums=null) {
			if(patient==null) {
				MsgBox.Show("Appointments","Error creating planned appointment.  No patient is currently selected.");
				return PlannedApptStatus.Failure;
			}
			if(!Security.IsAuthorized(EnumPermType.AppointmentCreate)) {
				return PlannedApptStatus.Failure;
			}
			if(PatRestrictionL.IsRestricted(patient.PatNum,PatRestrict.ApptSchedule)) {
				return PlannedApptStatus.Failure;
			}
			Patient patientMerged=AppointmentL.GetPatientMergePrompt(patient.PatNum);
			if(patientMerged!=null) {
				patient=patientMerged;
				FormOpenDental.S_Contr_PatientSelected(patient,isRefreshCurModule: true,isApptRefreshDataPat: false);
			}
			if(patient.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show("Appointments","Appointments cannot be scheduled for "+patient.PatStatus.ToString().ToLower()+" patients.");
				return PlannedApptStatus.Failure;
			}
			Appointment appointment=new Appointment();
			appointment.PatNum=patient.PatNum;
			appointment.ProvNum=patient.PriProv;
			appointment.ClinicNum=patient.ClinicNum;
			appointment.AptStatus=ApptStatus.Planned;
			appointment.AptDateTime=DateTime.Today;
			List<Procedure> listProcs=Procedures.GetManyProc(listPreSelectedProcNums,false);//Returns empty list if null.
			//If listProcs is empty then AptCur.Pattern defaults to PrefName.AppointmentWithoutProcsDefaultLength value.
			//See Appointments.GetApptTimePatternForNoProcs().
			appointment.Pattern=Appointments.CalculatePattern(patient,appointment.ProvNum,appointment.ProvHyg,listProcs);
			appointment.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			Appointments.Insert(appointment);
			PlannedAppt plannedAppt=new PlannedAppt();
			plannedAppt.AptNum=appointment.AptNum;
			plannedAppt.PatNum=patient.PatNum;
			plannedAppt.ItemOrder=itemOrder;
			PlannedAppts.Insert(plannedAppt);
			using FormApptEdit formApptEdit=new FormApptEdit(appointment.AptNum,listProcNumsPreSelected:listPreSelectedProcNums);
			formApptEdit.IsNew=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK){
				return PlannedApptStatus.FillGridNeeded;
			}
			//Only set the appointment hygienist to this patient's secondary provider if one was not manually set within the edit window.
			if(appointment.ProvHyg < 1) {
				List<Procedure> listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,true);
				bool isHygiene=(listProcedures.Count>0 && listProcedures.Select(x=>ProcedureCodes.GetProcCode(x.CodeNum)).ToList().All(x=>x.IsHygiene));
				//Automatically set the appointments hygienist to the secondary provider of the patient if one is set.
				if(isHygiene && patient.SecProv!=0) {
					Appointment appointmentOld=appointment.Copy();
					appointment.ProvNum=patient.SecProv;
					Appointments.Update(appointment,appointmentOld);
				}
			}
			Patient patientOld=patient.Copy();
			patient.PlannedIsDone=false;
			Patients.Update(patient,patientOld);
			FormOpenDental.S_RefreshCurrentModule(isClinicRefresh:false);//if procs were added in appt, then this will display them
			return PlannedApptStatus.Success;
		}

		/// <summary>Checks for specialty mismatch between pat and op. Then prompts user according to behavior defined by 
		/// PrefName.ApptSchedEnforceSpecialty.  Returns true if the Appointment is allowed to be scheduled, false otherwise.</summary>
		public static bool IsSpecialtyMismatchAllowed(long patNum,long clinicNum) {
			try {
				Appointments.HasSpecialtyConflict(patNum,clinicNum);//throws exception if we need to prompt user
			}
			catch(ODException oDException) {//Warn/Block
				switch((ApptSchedEnforceSpecialty)oDException.ErrorCode) {
					case ApptSchedEnforceSpecialty.Warn:
						if(!MsgBox.Show("Appointment",MsgBoxButtons.YesNo,oDException.Message+"\r\nSchedule appointment anyway?","Specialty Mismatch")){
							return false;
						}
						break;
					case ApptSchedEnforceSpecialty.Block:
						MsgBox.Show("Appointment",oDException.Message,"Specialty Mismatch");
						return false;
				}
			}
			return true;
		}

		/// <summary>Tests the appointment to see if it is acceptable to send it to the pinboard.  Also asks user appropriate questions to verify that's
		/// what they want to do.  Returns false if it will not be going to pinboard after all.</summary>
		public static bool OKtoSendToPinboard(ApptOther apptOther,List<ApptOther> listApptOthers,Control control) {
			if(apptOther.AptStatus==ApptStatus.Planned) {//if is a Planned appointment
				bool PlannedIsSched=false;
				for(int i=0;i<listApptOthers.Count;i++) {
					if(listApptOthers[i].NextAptNum==apptOther.AptNum) {//if the planned appointment is already sched
						PlannedIsSched=true;
					}
				}
				if(PlannedIsSched) {
					if(!MsgBox.Show(control,MsgBoxButtons.OKCancel,"The Planned appointment is already scheduled.  Do you wish to continue?")) {
						return false;
					}
				}
				return true;
			}
			switch(apptOther.AptStatus){
				case ApptStatus.Complete:
					MsgBox.Show(control,"Not allowed to move a completed appointment from here.");
					return false;
				case ApptStatus.Scheduled:
					if(!MsgBox.Show(control,MsgBoxButtons.YesNo,"Do you really want to move a previously scheduled appointment?")) {
						return false;
					}
					break;
				case ApptStatus.Broken://status gets changed after dragging off pinboard.
				case ApptStatus.None:
				case ApptStatus.UnschedList://status gets changed after dragging off pinboard.
					break;
				}
			//if it's a planned appointment, the planned appointment will end up on the pinboard. The copy will be made after dragging it off the pinboard.
			return true;
		}

		///<summary>If changing to 'Arrived' trigger, check if using eClipboard and setup to popup Kiosk Manager, or attempt to process the patient's
		///arrival as if they had texted 'A' to indicate 'Arrived'.</summary>
		public static void ShowKioskManagerIfNeeded(Appointment appointmentOld,long defNumConfirmedNew) {
			if(defNumConfirmedNew==appointmentOld.Confirmed || defNumConfirmedNew!=PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)) {
				//If no change to Appointment.Confirmed, or new status is not the Arrived trigger, nothing to do.
				return;
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled){
				clinicNum=appointmentOld.ClinicNum;
			}
			if(MobileAppDevices.IsClinicSignedUpForEClipboard(clinicNum) && ClinicPrefs.GetBool(PrefName.EClipboardPopupKioskOnCheckIn,clinicNum)) {
				Appointment appointmentNew=appointmentOld.Copy();
				appointmentNew.Confirmed=defNumConfirmedNew;
				using FormTerminalManager formTerminalManager=new FormTerminalManager(appointment:appointmentNew);
				formTerminalManager.ShowDialog();
				return;
			}
			//Manually marked as Arrived, if they had been sent an Arrival sms, try to process and send theArrival Response.
			//Pass oldAppt so it still has the old confirmation status; which has already updated in db.
			OpenDentBusiness.AutoComm.Arrivals.ProcessArrival(appointmentOld.PatNum,appointmentOld.ClinicNum,ListTools.FromSingle(appointmentOld));
		}

		///<summary>Attempts to send a BYOD/Check-In link for the given appointment.  Prompts user appropriately on success(Text Message winodw) or 
		///failure.</summary>
		public static void SendByodLink(Appointment appointment) {
			void displayError(string error) {
				MsgBox.Show(nameof(OpenDentBusiness.AutoComm.Byod),$"Unable to {MenuItemNames.SendEClipboardByod}.  "+error);
			}
			string message=null;
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => {
				if(appointment is null) {
					MessageBox.Show(Lan.g(nameof(OpenDentBusiness.AutoComm.Byod),"Unable to send for an invalid appointment."));
					return;
				}
				long clinicNum=0;
				if(PrefC.HasClinicsEnabled){
					clinicNum=appointment.ClinicNum;
				}
				if(OpenDentBusiness.AutoComm.Byod.IsEnabledForConfirmed(appointment.Confirmed,appointment.ClinicNum,out string err)) {
					List<PatComm> listPatComms=Patients.GetPatComms(new List<long> { appointment.PatNum },Clinics.GetClinic(clinicNum),isGetFamily:false);
					//This is not automated messaging, so we don't need to consider CommOptOuts, just IsSmsAnOption
					PatComm patComm=listPatComms.Find(x => x.PatNum==appointment.PatNum);
					if(patComm==null){
						MessageBox.Show(Lan.g(nameof(OpenDentBusiness.AutoComm.Byod),"Patient is not setup to receive text messages."));
						return;
					}
					if(!patComm.IsSmsAnOption){
						MessageBox.Show(Lan.g(nameof(OpenDentBusiness.AutoComm.Byod),"Patient is not setup to receive text messages."));
						return;
					}
					message=OpenDentBusiness.AutoComm.Byod.GetCheckInMsg(new List<Appointment> { appointment },listPatComms);
				}
				else {
					MessageBox.Show(err);
					return;
				}
			};
			progressOD.StartingMessage=Lans.g(nameof(OpenDentBusiness.AutoComm.Byod),"Generating eClipboard links...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			if(string.IsNullOrWhiteSpace(message)) {
				MessageBox.Show(Lan.g(nameof(OpenDentBusiness.AutoComm.Byod),"Unable to generate links for the appointment."));
				return;
			}
			FormOpenDental.S_TxtMsg_Click(appointment.PatNum,message);
		}

		///<summary>Returns true if the PrefName.ApptPreventChangesToCompleted is true, the appointment has completed procedures attached, and the appointment 
		///has a completed status. Otherwise it will return false.</summary>
		public static bool DoPreventChangesToCompletedAppt(Appointment appointment,PreventChangesApptAction preventChangesApptAction,List<Procedure> listProceduresAttached=null) {
			return DoPreventChangesToCompletedAppt(appointment,preventChangesApptAction,out string msg,listProceduresAttached,false);
    }

		///<summary>Returns true if the PrefName.ApptPreventChangesToCompleted is true, the appointment has completed procedures attached, and the appointment 
		///has a completed status. Otherwise it will return false.</summary>
		public static bool DoPreventChangesToCompletedAppt(Appointment appointment,PreventChangesApptAction preventChangesApptAction,out string msg,List<Procedure> listProceduresAttached=null) {
			return DoPreventChangesToCompletedAppt(appointment,preventChangesApptAction,out msg,listProceduresAttached,true);
    }
        
		///<summary>Returns true if the PrefName.ApptPreventChangesToCompleted is true, the appointment has completed procedures attached, and the appointment 
		///has a completed status. Otherwise it will return false.</summary>
		public static bool DoPreventChangesToCompletedAppt(Appointment appointment,PreventChangesApptAction preventChangesApptAction,out string msg,List<Procedure> listProceduresAttached=null,bool doSupressMsg=false) {
      msg=null;
			if(appointment==null) {
				return true;
			}
			if(!PrefC.GetBool(PrefName.ApptPreventChangesToCompleted) //The preference is turned off.
				|| (appointment.AptStatus!=ApptStatus.Complete)//The appointment status is not complete. Allow changes.
				|| !Appointments.HasCompletedProcsAttached(appointment.AptNum,listProceduresAttached))//Apt does not have completed procedures attached
			{
				return false;
			}
			//Prepare the message text that the user will see before we return true.
			string strAction;
			switch(preventChangesApptAction) {
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
