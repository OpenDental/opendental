using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class AppointmentT {

		///<summary></summary>
		public static Appointment CreateAppointment(long patNum,DateTime aptDateTime,long opNum,long provNum,long provHyg=0,string pattern="//XXXX//"
			,long clinicNum=0,bool isHygiene=false,ApptStatus aptStatus=ApptStatus.Scheduled,ApptPriority priority=ApptPriority.Normal,string aptNote=""
			,long appointmentTypeNum=0,long confirmed=0,DateTime dateTimeAskedToArrive=default)
		{
			Appointment appointment=new Appointment();
			appointment.AptDateTime=aptDateTime;
			appointment.DateTimeAskedToArrive=dateTimeAskedToArrive;
			appointment.AptStatus=aptStatus;
			appointment.ClinicNum=clinicNum;
			appointment.IsHygiene=isHygiene;
			appointment.Op=opNum;
			appointment.PatNum=patNum;
			appointment.Pattern=pattern;
			appointment.ProvNum=provNum;
			appointment.ProvHyg=provHyg;
			appointment.Priority=priority;
			appointment.Note=aptNote;
			appointment.AppointmentTypeNum=appointmentTypeNum;
			appointment.Confirmed=confirmed;
			Appointments.Insert(appointment);
			return appointment;
		}

		public static Appointment CreateAppointmentFromRecall(Recall recall,Patient pat,DateTime aptDateTime,long opNum,long provNum) {
			RecallType recallType=RecallTypes.GetFirstOrDefault(x => x.RecallTypeNum==recall.RecallTypeNum);
			Appointment appt=CreateAppointment(pat.PatNum,aptDateTime,opNum,provNum,pattern:recallType.TimePattern,clinicNum:pat.ClinicNum);
			foreach(string procCode in RecallTypes.GetProcs(recallType.RecallTypeNum)) {
				ProcedureT.CreateProcedure(pat,procCode,ProcStat.TP,"",50,appt.AptDateTime,provNum:provNum,aptNum:appt.AptNum);
			}
			return appt;
		}

		///<summary>Deletes everything from the appointment table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearAppointmentTable() {
			string command="DELETE FROM appointment WHERE AptNum > 0";
			DataCore.NonQ(command);
		}

		///<summary>Optionally pass in daysForSchedule to create a schedule for the number of days for each provider. Schedules will all be default 8 to 4. </summary>
		public static AppointmentSearchData CreateDataForAppointmentSearch(long numProvs,long numOps,long numClinics,int daysForSchedule=0) {
			AppointmentSearchData appSearchData=new UnitTestsCore.AppointmentSearchData();
			appSearchData.Patient=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Pat");
			for(int i = 0;i < numOps;i++) {
				Operatory op=OperatoryT.CreateOperatory("abbr"+i,"opName"+i);
				appSearchData.ListOps.Add(op);
			}
			for(int i = 0;i < numClinics;i++) {
				Clinic clinic=ClinicT.CreateClinic("Clinic "+i);
				appSearchData.ListClinics.Add(clinic);
			}
			//loop through and create all the providers. 
			for(int i = 0;i < numProvs;i++) {
				long prov=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+i);
				appSearchData.ListProvNums.Add(prov);
				//create a general schedule for each prov. Can manipulate later.
				if(daysForSchedule>0) {
					for(int j = 0;j < daysForSchedule;j++) {
						if(DateTime.Today.AddDays(j).DayOfWeek==DayOfWeek.Saturday || DateTime.Today.AddDays(j).DayOfWeek==DayOfWeek.Sunday) {
							daysForSchedule++;//add another day to the loop since we want to skip but still add the schedule on a weekday
							continue;
						}
						Schedule sched=ScheduleT.CreateSchedule(DateTime.Today.AddDays(j)
							,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(j).Day,8,0,0).TimeOfDay
							,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(j).Day,16,0,0).TimeOfDay
							,ScheduleType.Provider,provNum:prov);//default to tomorrow from 8-4
					}
				}
			}
			return appSearchData;
		}

		///<summary>Optionally pass in daysForSchedule to create a schedule for the number of days for each provider. Schedules will all be default 8 to 4. 
		///Note this is for non dynamic scheduling. daysForSchedule will include today, so pass in one extra if search starts with tomorrow.</summary>
		public static AppointmentSearchData CreateScheduleAndOpsForProv(long numOps,long numClinics,long provNum,int daysForSchedule=0,long hygNum=0,
			bool isDynamic=false,bool skipWeekends=true)
		{
			AppointmentSearchData appSearchData=new UnitTestsCore.AppointmentSearchData();
			appSearchData.Patient=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Pat");
			for(int i = 0;i < numOps;i++) {
				Operatory op=OperatoryT.CreateOperatory("abbr"+i,"opName"+i,provDentist:provNum,provHygienist:hygNum);
				appSearchData.ListOps.Add(op);
			}
			for(int i = 0;i < numClinics;i++) {
				Clinic clinic=ClinicT.CreateClinic("Clinic "+i);
				appSearchData.ListClinics.Add(clinic);
			}
			//create a general schedule for each prov. Can manipulate later.
			if(daysForSchedule>0) {
				for(int j = 0;j < daysForSchedule;j++) {
					if(skipWeekends && (DateTime.Today.AddDays(j).DayOfWeek==DayOfWeek.Saturday || DateTime.Today.AddDays(j).DayOfWeek==DayOfWeek.Sunday)) {
						daysForSchedule++;//add another day to the loop since we want to skip but still add the schedule on a weekday
						continue;
					}
					Schedule sched;
					if(isDynamic) {
						//create schedule for provider but do not assign it to an operatory
						sched=ScheduleT.CreateSchedule(DateTime.Today.AddDays(j)
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(j).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(j).Day,16,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>() { });//default to 8-4
					}
					else {
						sched=ScheduleT.CreateSchedule(DateTime.Today.AddDays(j)
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(j).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(j).Day,16,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNum,listOpNums:appSearchData.ListOps.Select(x => x.OperatoryNum).ToList());//default to 8-4
					}
					
					appSearchData.ListSchedules.Add(sched);
				}
			}
			return appSearchData;
		}

		///<summary>Most of the logic for breaking an appointment. Pass in the brokenFee (the number the user enters in the brokenAppointment window),
		///Optionally pass in if the brokenappointment procedure is being deleted. Returns the broken procedure that was created.</summary>
		public static Procedure BreakAppointment(Appointment appt,Patient pat,ProcedureCode procCode,double feeOverride,bool isDeleteBrokenProc=false) 
		{
			//suppressHistory is true due to below logic creating a log with a specific HistAppointmentAction instead of the generic changed.
			DateTime datePrevious=appt.DateTStamp;
			bool suppressHistory=false;
			if(procCode!=null) {
				suppressHistory=procCode.ProcCode=="D9986" || procCode.ProcCode=="D9987";
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
			List<Procedure> listProcedures=Procedures.GetProcsForSingle(appt.AptNum,false);
			//splits should only exist on procs if they are using tp pre-payments
			List<PaySplit> listSplitsForApptProcs=new List<PaySplit>();
			bool isNonRefundable=false;
			double brokenProcAmount=0;
			if(listProcedures.Count > 0) {
				listSplitsForApptProcs=PaySplits.GetPaySplitsFromProcs(listProcedures.Select(x => x.ProcNum).ToList());
			}
			Procedure brokenProcedure=new Procedure();
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
				else {
					brokenProcedure.ProcFee=procFee;
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					brokenProcedure.SiteNum=pat.SiteNum;
				}
				Procedures.Insert(brokenProcedure);
				Procedure procOld=brokenProcedure.Copy();
				//Now make a claimproc if the patient has insurance.  We do this now for consistency because a claimproc could get created in the future.
				List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
				List<ClaimProc> listClaimProcsForProc=ClaimProcs.RefreshForProc(brokenProcedure.ProcNum);
				Procedures.ComputeEstimates(brokenProcedure,pat.PatNum,listClaimProcsForProc,false,listInsPlans,listPatPlans,listBenefits,pat.Age,listInsSubs);
				if(listSplitsForApptProcs.Count>0 && PrefC.GetBool(PrefName.TpPrePayIsNonRefundable) && procCode.ProcCode=="D9986") {
					//if there are pre-payments, non-refundable pre-payments is turned on, and the broken appointment is a missed code then auto-fill 
					//the window with the sum of the procs for the appointment. Transfer money below after broken procedure is confirmed by the user.
					//normally goes to the form to let the user speficy, this is the auto filled amount for the form. 
					brokenProcedure.ProcFee=feeOverride;//listSplitsForApptProcs.Sum(x => x.SplitAmt); 
					isNonRefundable=true;
				}
				if(isDeleteBrokenProc) {
					brokenProcedure.ProcStatus=ProcStat.D;
				}
				brokenProcedure.ProcFee=feeOverride;
				brokenProcAmount=feeOverride;
				Procedures.Update(brokenProcedure,procOld);
			}
			#endregion
			//Note this MUST come after FormProcBroken since clicking cancel in that window will delete the procedure.
			if(isNonRefundable && !isDeleteBrokenProc && listSplitsForApptProcs.Count>0) {
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
						brokenProcAmount-=amt;
					}
				}
			}
			return brokenProcedure;
		}

		public static void SetSecDateTEntry(Appointment appt,DateTime dateTime) {
			appt.SecDateTEntry=dateTime;
			string command=$"UPDATE appointment SET {nameof(Appointment.SecDateTEntry)}={POut.DateT(dateTime)} WHERE {nameof(Appointment.AptNum)}={appt.AptNum}";
			DataCore.NonQ(command);
		}
		
	}

	

	public class AppointmentSearchData {//this can maybe be moved to appointments tests and made private. Same with the helper method. 
		public Patient Patient;
		public List<long> ListProvNums=new List<long>();
		public List<Operatory> ListOps=new List<Operatory>();
		public List<Clinic> ListClinics=new List<Clinic>();
		public List<Schedule> ListSchedules=new List<Schedule>();//just for convenience when setting up test
	}
}
