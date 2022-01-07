using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness.AutoComm {
	public class Arrivals {
		private IEnumerable<ApptReminderSent> ListArrivalsSent=new List<ApptReminderSent>();
		private IEnumerable<ApptReminderRule> ListApptReminderRules=new List<ApptReminderRule>();
		private TagReplacer _tagReplacer;

		private Arrivals() {
			_tagReplacer=new ArrivalsTagReplacer();
		}

		public static Arrivals LoadArrivals() {
			return new Arrivals();
		}

		///<summary>If necessary, gets ApptReminderSents and corresponding ApptReminderRules for given appointments from database.</summary>
		public static Arrivals LoadArrivals(List<long> listClinicNums,List<long> listApptNums) {
			Arrivals arrivals=LoadArrivals();
			if(!PrefC.HasClinicsEnabled) {
				listClinicNums=new List<long> { 0 };
			}
			List<long> listSignedUpClinics=listClinicNums.Where(x => ClinicPrefs.GetBool(PrefName.ApptConfirmAutoSignedUp,x)).ToList();
			//Only do the work of looking up ApptReminderSents and ApptReminderRules if the appropriate eServices are enabled and in use.
			if(!listApptNums.IsNullOrEmpty() && listSignedUpClinics.Any() && PrefC.GetBool(PrefName.ApptArrivalAutoEnabled)) {
				arrivals.ListApptReminderRules=GetApptReminderRules(listSignedUpClinics);
				if(arrivals.ListApptReminderRules.Any()) {
					List<long> listApptReminderRuleNums=arrivals.ListApptReminderRules.Select(x => x.ApptReminderRuleNum).ToList();
					//This clinic has at least one Reminder Rule with an arrival/come-in template defined.  We now know we need ApptReminderSent data.
					arrivals.ListArrivalsSent=ApptReminderSents.GetForApt(listApptNums.ToArray())
						//Arrivals only, not eReminders.
						.Where(x => ListTools.In(x.ApptReminderRuleNum,listApptReminderRuleNums)).ToList();
				}
			}
			return arrivals;
		}

		///<summary>Gets Arrival ApptReminderRules, including rules for clinics that are using default rules.</summary>
		private static List<ApptReminderRule> GetApptReminderRules(List<long> listClinicNums) {
			List<ApptReminderRule> listRules=ApptReminderRules.GetForTypes(ApptReminderType.Arrival);
			List<ApptReminderRule> listRulesDefault=listRules.Where(x => x.ClinicNum==0).ToList();
			//Make sure a rule is included for clinics using defaults.
			foreach(long clinicNum in listClinicNums) {
				ClinicPref clinicPref=ClinicPrefs.GetPref(PrefName.ApptArrivalUseDefaults,clinicNum);
				if(clinicPref!=null && PIn.Bool(clinicPref.ValueString)) {
					listRules.AddRange(listRulesDefault.Select(x => x.CopyWithClinicNum(clinicNum)));
				}
			}
			return listRules.Where(x => 				
				x.IsEnabled //Rule must be enabled
				&& ListTools.In(x.ClinicNum,listClinicNums)//For these clinics
				&& (
					(x.IsAutoReplyEnabled && !string.IsNullOrWhiteSpace(x.TemplateAutoReply))//AutoReply is enabled and has a template
					|| !string.IsNullOrWhiteSpace(x.TemplateComeInMessage)//or ComeIn has a template.
				)				
			).ToList();
		}

		///<summary>Processes an inboud SMS and determines if it is an "I have arrived" message and office is setup to send an automatic Arrival 
		///Response.</summary>
		public static void ProcessArrival(SmsFromMobile sms) {
			if(sms.MsgTotal!=1 || sms.MsgText.ToLower().Trim()!=ArrivalsTagReplacer.ARRIVED_CODE.ToLower().Trim()) {//Not an "Arrived" sms.
				return;
			}
			//It's possible a dependent and guarantor both have appointments on the same day, but have different wireless phone numbers.
			//We don't want the dependent to mark the guarantor appointment as 'Arrived' as well.
			List<Patient> listPatients=Patients.GetFamily(sms.PatNum).ListPats.ToList();
			listPatients.RemoveAll(x => PhoneNumbers.RemoveNonDigitsAndTrimStart(x.WirelessPhone)!=PhoneNumbers.RemoveNonDigitsAndTrimStart(sms.MobilePhoneNumber));
			long[] arrayPatNums=listPatients.Select(x => x.PatNum).ToArray();
			List<Appointment> listAppointments=Appointments.GetAppointmentsForPat(arrayPatNums);
			ProcessArrivalAsync(sms.PatNum,sms.ClinicNum,sms.MobilePhoneNumber,listAppointments,true);
		}

		///<summary>Determines if office is setup to send an automatic Arrival Response for the given patient.</summary>
		public static void ProcessArrival(long patNumForResponse,long clinicNum,List<Appointment> listAppts) {
			ProcessArrivalAsync(patNumForResponse,clinicNum,null,listAppts,false);
		}

		///<summary>Determines if office is setup to send an automatic Arrival Response for the given patient.</summary>
		private static void ProcessArrivalAsync(long patNumForResponse,long clinicNum,string mobilePhoneNumber,List<Appointment> listAppts,bool doAlert) {
			if(patNumForResponse<=0) {//Invalid patNum.
				return;
			}
			//Run Arrival Processing on a thread, because we do not want this action, which includes a web call, to slow down the UI, as everything is
			//happening behind the scenes anyway.
			ODThread arrivalThread=new ODThread(o => {
				List<Appointment> listTodayAppts=listAppts
					.Where(x => ListTools.In(x.AptStatus,ApptStatus.Scheduled))
					.Where(x => x.ClinicNum==clinicNum && x.AptDateTime.Date==DateTime_.Today).ToList();
				Arrivals arrival=LoadArrivals(ListTools.FromSingle(clinicNum),listTodayAppts.Select(x => x.AptNum).ToList());
				arrival.ProcessArrival(patNumForResponse,clinicNum,mobilePhoneNumber,listTodayAppts,doAlert);
			});
			arrivalThread.AddExceptionHandler((ex) => 
				Logger.WriteError(MiscUtils.GetExceptionText(ex),ODFileUtils.CombinePaths(nameof(Arrivals),nameof(ProcessArrival))));
			arrivalThread.Name=nameof(ProcessArrival)+$"_PatNum{patNumForResponse}";
			arrivalThread.GroupName=nameof(ProcessArrival);
			arrivalThread.Start();
		}

		///<summary>Determines if the patient corresponding to the SmsFromMobile can be sent an Arrival Response and marked as Arrived.</summary>
		private void ProcessArrival(long patNum,long clinicNum,string mobilePhoneNumber,List<Appointment> listAppts,bool doAlert) {
			List<Appointment> listApptsToday=listAppts.Where(x => x.ClinicNum==clinicNum && x.AptDateTime.Date==DateTime_.Today).OrderBy(x => x.AptDateTime).ToList();
			string logSubDir=ODFileUtils.CombinePaths(nameof(Arrivals),nameof(ProcessArrival),clinicNum.ToString());
			if(listApptsToday.Count==0) {
				Logger.WriteError($"PatNum: {patNum} does not have any appointments at ClinicNum {clinicNum} today.",logSubDir);
				return;
			}
			List<Appointment> listApptsAutomationEnabled=listApptsToday
				//Check if (given clinic exists and has automation enabled) or (HQ "clinic" and Arrivals are enabled)
				.Where(x => Clinics.GetClinic(x.ClinicNum)?.IsConfirmEnabled??(x.ClinicNum==0 && PrefC.GetBool(PrefName.ApptArrivalAutoEnabled)))
				.ToList();
			if(listApptsAutomationEnabled.Count==0) {
				Logger.WriteError($"PatNum: {patNum} has appointments at ClinicNum {clinicNum} today, but automation is not enabled for this clinic.",logSubDir);
				return;
			}
			List<ApptResponse> listApptResponses=GetApptResponses(listApptsAutomationEnabled);
			MarkArrived(listApptResponses,doAlert);//All appoinments should be marked Arrived.
			listApptResponses.RemoveAll(x => string.IsNullOrWhiteSpace(x.Response));//In case the office does not want to send Arrival Response SMS.
			if(!listApptResponses.IsNullOrEmpty()) {
				//There is a configured Arrival Response for this patient, send sms.
				string message=AppendEClipboardTokens(listApptResponses,logSubDir);
				TrySendArrivalResponseSms(patNum,mobilePhoneNumber,clinicNum,message,logSubDir);
			}
		}

		private string AppendEClipboardTokens(List<ApptResponse> listApptResponses,string logSubDir) {				
			if(listApptResponses.IsNullOrEmpty()) {
				return "";
			}
			ApptResponse apptResponse=listApptResponses.First();
			string message=apptResponse.Response;
			try {
				long clinicNum=PrefC.HasClinicsEnabled ? apptResponse.Appointment.ClinicNum : 0;
				//Validate preferences are setup to use this feature.
				if(Byod.IsSetup(clinicNum,out string err)
					&& ClinicPrefs.GetBool(PrefName.EClipboardAppendByodToArrivalResponseSms,clinicNum)) 
				{
					List<Appointment> listAppts=listApptResponses.Select(x => x.Appointment).ToList();
					List<PatComm> listPatComms=listApptResponses.Select(x => x.PatComm).ToList();
					string checkInMsg=Byod.GetCheckInMsg(listAppts,listPatComms);
					if(!string.IsNullOrWhiteSpace(checkInMsg)) {
						message+='\n'+checkInMsg;
					}
				}
			}
			catch(Exception ex) {
				Logger.WriteError(MiscUtils.GetExceptionText(ex),logSubDir);
			}
			return message;
		}

		///<summary>Sets appointments.Confirmed to the ArrivedTimeTrigger.</summary>
		private void MarkArrived(IEnumerable<ApptResponse> listAppts,bool doAlert) {
			long arrivedTrigger=PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger);
			List<ApptResponse> listArriving=listAppts.Where(x => x.Appointment.Confirmed!=arrivedTrigger).ToList();
			foreach(ApptResponse appt in listArriving) {
				//This update will trigger eClipboard to generate the appropriate check-in sheets if the appointment is not already marked as arrived.
				//If the clinic is setup for eClipboard checking, the appropriate token needs to be included in the Arrival Response sms.
				Appointments.SetConfirmed(appt.Appointment,arrivedTrigger);
				SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,appt.Appointment.PatNum,"Appointment confirmation status changed from "
					+Defs.GetName(DefCat.ApptConfirmed,appt.Appointment.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,arrivedTrigger)
					+" due to an Arrival text.",appt.Appointment.AptNum,LogSources.AutoConfirmations,appt.Appointment.DateTStamp);
				EServiceLogs.MakeLogEntry(eServiceAction.ArrivalReceived,eServiceType.Arrivals,FKeyType.ApptNum,patNum:appt.Appointment.PatNum
					,FKey:appt.Appointment.AptNum,clinicNum:appt.Appointment.ClinicNum);
			}
			if(doAlert) {
				CreateArrivalAlert(listArriving);
			}
		}

		///<summary>Creates a PatientArrived alert for all given appointments. </summary>
		private void CreateArrivalAlert(List<ApptResponse> listArriving) {
			foreach(ApptResponse appt in listArriving) {
				AlertItem alert=new AlertItem() {
					ClinicNum=appt.Appointment.ClinicNum,
					Description=appt.PatComm.GetFirstOrPreferred()
						+" "+Lans.g(this,"arrived at")+" "+DateTime.Now.ToShortTimeString()						
						+" "+DateTime.Now.ToString(PrefC.PatientCommunicationDateFormat)
						+" "+Lans.g(this,"for appointment at")+" "+appt.Appointment.AptDateTime.ToShortTimeString()
						+" "+appt.Appointment.AptDateTime.ToString(PrefC.PatientCommunicationDateFormat),
					Type=AlertType.PatientArrival,
					Actions=ActionType.MarkAsRead|ActionType.Delete|ActionType.OpenForm,
					FormToOpen=FormType.FormApptEdit,
					Severity=SeverityType.Low,
					FKey=appt.Appointment.AptNum
				};
				AlertItems.Insert(alert);
			}
		}

		///<summary>Attempts to send the Arrival Response sms.  Logs on failure.</summary>
		private bool TrySendArrivalResponseSms(long patNum,string wirelessPhone,long clinicNum,string message,string logDir) {
			bool retVal=false;
			try {
				if(wirelessPhone is null) {
					//try to find a usable phone number for the patient.
					foreach(PatComm pat in Patients.GetPatComms(ListTools.FromSingle(patNum),Clinics.GetClinic(clinicNum))
						.OrderByDescending(x => x.PatNum==patNum)) 
					{
						if(pat.IsSmsAnOption) {
							wirelessPhone=pat.SmsPhone;//Stripped of formatting.
							break;
						}
					}
				}
				if(wirelessPhone is null) {
					Logger.WriteError($"Unable to find a WirelessPhone for PatNum: {patNum}.",logDir);
					return false;
				}
				SmsToMobile sent=SmsToMobiles.SendSmsSingle(patNum,wirelessPhone,message,clinicNum,SmsMessageSource.Arrival);
				Logger.WriteLine($"Sent {JsonConvert.SerializeObject(sent)}",logDir);
				retVal=true;
			}
			catch(Exception ex) {
				string err=$"Failed to send Arrival Response '{message}' to PatNum: {patNum}, WirelessPhone: {wirelessPhone}. "
					+MiscUtils.GetExceptionText(ex);
				Logger.WriteError(err,logDir);
			}
			return retVal;
		}

		///<summary>Determines if the given aptNum corresponds to an ApptReminderRule that has a corresponding ComeInMessageTemplate.  Does not make
		///database calls.</summary>
		public bool HasComeInMsg(long aptNum) {
			ApptReminderRule rule=GetArrivalRule(aptNum);
			return !string.IsNullOrWhiteSpace(rule?.TemplateComeInMessage??"");
		}

		///<summary>Gets an ApptReminderRule with a ComeInMessageTemplate that corresponds to the given aptNum. Does not make database calls.</summary>
		private ApptReminderRule GetArrivalRule(long aptNum) {
			IEnumerable<long> listReminderRuleNums=ListArrivalsSent.Where(x => x.ApptNum==aptNum).Select(x => x.ApptReminderRuleNum);
			ApptReminderRule rule=ListApptReminderRules.FirstOrDefault(x => ListTools.In(x.ApptReminderRuleNum,listReminderRuleNums));
			return rule;
		}

		///<summary>Returns true if an appropriate 'Come In' message template was found for the given appointment. Makes database calls.</summary>
		public bool TryGetComeInMsg(long aptNum,out string message) {
			return TryGetComeInMsg(Appointments.GetOneApt(aptNum),out message);
		}
			
		///<summary>Returns true if an appropriate 'Come In' message template was found for the given appointment.</summary>
		public bool TryGetComeInMsg(Appointment appt,out string message) {
			PatComm patComm=Patients.GetPatComms(new List<long> { appt.PatNum },null).FirstOrDefault();
			return TryGetMsgFromTemplate(appt,patComm,(rule) => rule?.TemplateComeInMessage??"",out message);
		}

		///<summary>Returns a list of Appointment/PatComm/Arrival Responses for the given Appointments if the Appointment was in a Confirmed status that 
		///is allowed to receive Arrival Responses.  The Response field will be non-empty an Arrival Response template was found.</summary>
		private List<ApptResponse> GetApptResponses(List<Appointment> listAppts) {
			List<ApptResponse> listResponses=new List<ApptResponse>();
			List<long> listConfirmStatusToSkip=PrefC.GetString(PrefName.ApptConfirmExcludeArrivalResponse)
				.Split(",",StringSplitOptions.RemoveEmptyEntries)
				.Select(x => PIn.Long(x))
				.ToList();
			//It is expected here that all Appointments are for the same clinic.
			Clinic clinic=(listAppts.Any(x => x.ClinicNum==0)) ? Clinics.GetPracticeAsClinicZero() : Clinics.GetClinic(listAppts.First().ClinicNum);
			List<PatComm> listPatComms=Patients.GetPatComms(listAppts.Select(x => x.PatNum).ToList(),clinic);
			//We will mark all appointments with Confirmed not in the "skip list" as arrived, and send response SMS where configured.
			foreach(Appointment appt in listAppts.Where(x => !listConfirmStatusToSkip.Contains(x.Confirmed))) {
				PatComm patComm=listPatComms.FirstOrDefault(x => x.PatNum==appt.PatNum);
				string getAutoReplyTemplate(ApptReminderRule rule) {
					if(rule?.IsAutoReplyEnabled??false) {
						return rule.TemplateAutoReply;
					}
					return "";//disabled
				}
				//Later, we will have to filter out ApptResponses that do not have a Response/message.
				TryGetMsgFromTemplate(appt,patComm,getAutoReplyTemplate,out string message);
				listResponses.Add(new ApptResponse(appt,patComm,message));
			}
			return listResponses;
		}

		private bool TryGetMsgFromTemplate(Appointment appt,PatComm patComm,Func<ApptReminderRule,string> getTemplate,out string message) {
			ApptReminderRule rule=null;
			string msg="";
			string logDir=ODFileUtils.CombinePaths(nameof(Arrivals),nameof(TryGetMsgFromTemplate),appt.ClinicNum.ToString());
			try {
				rule=GetArrivalRule(appt.AptNum);
				string template=getTemplate(rule);
				if(string.IsNullOrWhiteSpace(template)) {
					string info=$"Unable to find template for Appointment.AptNum: {appt.AptNum}";
					Logger.WriteLine(info,logDir);
				}
				else {
					Clinic clinic=(appt.ClinicNum==0) ? Clinics.GetPracticeAsClinicZero() : Clinics.GetClinic(appt.ClinicNum);
					msg=_tagReplacer.ReplaceTags(template,new ApptLite(appt,patComm),clinic,false);
				}
			}
			catch(Exception ex) {
				Logger.WriteError(MiscUtils.GetExceptionText(ex),logDir);
			}
			message=msg;
			return !string.IsNullOrWhiteSpace(message);
		}

		private class ApptResponse {
			public Appointment Appointment;
			public PatComm PatComm;
			public string Response;
				
			public ApptResponse(Appointment appt,PatComm patComm,string response) {
				Appointment=appt;
				PatComm=patComm;
				Response=response;
			}
		}
	}

}
