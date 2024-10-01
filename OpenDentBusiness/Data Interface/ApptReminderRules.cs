using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness.AutoComm;

namespace OpenDentBusiness{

	///<summary></summary>
	public class ApptReminderRules{
		#region Get Methods

		///<summary>Gets all, sorts by TSPrior Desc, Should never be more than 3 (per clinic if this is implemented for clinics.)</summary>
		public static List<ApptReminderRule> GetForTypes(params ApptReminderType[] apptReminderTypesArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ApptReminderRule>>(MethodBase.GetCurrentMethod(),apptReminderTypesArray);
			}
			if(apptReminderTypesArray.Length==0) {
				return new List<ApptReminderRule>();
			}
			string command="SELECT * FROM apptreminderrule WHERE TypeCur IN("+string.Join(",",apptReminderTypesArray.Select(x => POut.Int((int)x)))+")";
			return Crud.ApptReminderRuleCrud.SelectMany(command);
		}

		///<summary>Gets all from the DB for the given clinic and types.</summary>
		public static List<ApptReminderRule> GetForClinicAndTypes(long clinicNum,params ApptReminderType[] apptReminderTypesArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ApptReminderRule>>(MethodBase.GetCurrentMethod(),clinicNum,apptReminderTypesArray);
			}
			if(apptReminderTypesArray.Length==0) {
				return new List<ApptReminderRule>();
			}
			string command = "SELECT * FROM apptreminderrule WHERE ClinicNum="+POut.Long(clinicNum)
				+" AND TypeCur IN("+string.Join(",",apptReminderTypesArray.Select(x => POut.Int((int)x)))+")";
			return Crud.ApptReminderRuleCrud.SelectMany(command).ToList();
		}
		#endregion

		#region Misc Methods

		///<summary>Returns whether appt reminders are enabled and at least one rule with TSPrior set.</summary>
		public static bool UsesApptReminders() {
			bool isEnabled=PrefC.GetBool(PrefName.ApptRemindAutoEnabled);
			if(!isEnabled) {
				return false;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			return Db.GetLong("SELECT count(*) FROM ApptReminderRule WHERE TSPrior>0")>0;
		}

		#endregion

		///<summary>Gets all, sorts by TSPrior Desc, Should never be more than 3 (per clinic if this is implemented for clinics.)</summary>
		public static List<ApptReminderRule> GetAll(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ApptReminderRule>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM apptreminderrule ";
			return Crud.ApptReminderRuleCrud.SelectMany(command).OrderByDescending(x => new[] { 1,2,0 }.ToList().IndexOf((int)x.TypeCur)).ToList();
		}

		///<summary>16.3.29 is more strict about reminder rule setup. Prompt the user and allow them to exit the update if desired. Get all currently enabled reminder rules.
		///Returns 2 element list of bool. 
		///[0] indicates if any single clinic/practice has more than 1 same day reminder. 
		///[1] indicates if any single clinic/practice has more than 1 future day reminder.</summary>
		public static List<bool> Get_16_3_29_ConversionFlags() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<bool>>(System.Reflection.MethodBase.GetCurrentMethod());
			}
			//The code below uses anonymous types, but we won't fix that because this is just part of a conversion script.
			//We can't use CRUD here as we may be in between versions so use DataTable directly.
			string command="SELECT ApptReminderRuleNum, TypeCur, TSPrior, ClinicNum FROM apptreminderrule WHERE TypeCur=0";
			var groups=Db.GetTable(command).Select().Select(x => new {
				ApptReminderRuleNum = PIn.Long(x[0].ToString()),
				TypeCur = PIn.Int(x[1].ToString()),
				TSPrior = TimeSpan.FromTicks(PIn.Long(x[2].ToString())),
				ClinicNum = PIn.Long(x[3].ToString())
			})
			//All rules grouped by clinic and whether they are same day or future day.
			.GroupBy(x => new { ClincNum=x.ClinicNum, IsSameDay=x.TSPrior.TotalDays<1 });
			return new List<bool>() {
				//Any 1 single clinic has more than 1 same day reminder.
				groups.Any(x => x.Key.IsSameDay && x.Count()>1),
				//Any 1 single clinic has more than 1 future day reminder.
				groups.Any(x => !x.Key.IsSameDay && x.Count()>1)
			};
		}

		public static List<ApptReminderRule> GetClinicDefaults() {
			Meth.NoCheckMiddleTierRole();
			return GetForClinic(0);
		}

		///<summary>Gets all from the DB, sorts by TSPrior Desc.</summary>
		public static List<ApptReminderRule> GetForClinic(long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ApptReminderRule>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command = "SELECT * FROM apptreminderrule WHERE ClinicNum="+POut.Long(clinicNum);
			return Crud.ApptReminderRuleCrud.SelectMany(command).OrderByDescending(x => x.TSPrior.TotalMinutes).ToList();
		}
		
		public static bool SyncByClinicAndTypes(List<ApptReminderRule> listApptReminderRulesNew,long clinicNum,params ApptReminderType[] apptReminderTypesArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listApptReminderRulesNew,clinicNum,apptReminderTypesArray);
			}
			if(apptReminderTypesArray.Length==0) {
				return false;
			}
			List<ApptReminderRule> listApptReminderRulesOld=ApptReminderRules.GetForClinicAndTypes(clinicNum,apptReminderTypesArray);//ClinicNum can be 0
			if(Crud.ApptReminderRuleCrud.Sync(listApptReminderRulesNew,listApptReminderRulesOld)) {
				SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,string.Join(", ",apptReminderTypesArray.Select(x => x.GetDescription()))
					+" rules changed for ClinicNum: "+clinicNum.ToString()+".");
				return true;
			}
			return false;
		}

		///<summary>Gets one ApptReminderRule from the db.</summary>
		public static ApptReminderRule GetOne(long apptReminderRuleNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ApptReminderRule>(MethodBase.GetCurrentMethod(),apptReminderRuleNum);
			}
			return Crud.ApptReminderRuleCrud.SelectOne(apptReminderRuleNum);
		}

		///<summary></summary>
		public static long Insert(ApptReminderRule apptReminderRule){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				apptReminderRule.ApptReminderRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),apptReminderRule);
				return apptReminderRule.ApptReminderRuleNum;
			}
			return Crud.ApptReminderRuleCrud.Insert(apptReminderRule);
		}

		///<summary></summary>
		public static void Update(ApptReminderRule apptReminderRule){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderRule);
				return;
			}
			Crud.ApptReminderRuleCrud.Update(apptReminderRule);
		}

		///<summary></summary>
		public static void Delete(long apptReminderRuleNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderRuleNum);
				return;
			}
			Crud.ApptReminderRuleCrud.Delete(apptReminderRuleNum);
		}

		///<summary>Update Appointment.Confirmed. Returns true if update was allowed. Returns false if it was prevented.</summary>
		public static ConfStatusUpdate UpdateAppointmentConfirmationStatus(ConfirmationRequest confirmationRequest,long defNumApptConfirmed,
			string commaListOfExcludedDefNums,Action<string> logOnError=null) 
		{
			Meth.NoCheckMiddleTierRole();
			Appointment appointment=Appointments.GetOneApt(confirmationRequest.ApptNum);
			if(appointment==null) {
				return ConfStatusUpdate.Failure;
			}
			if(appointment.Confirmed==defNumApptConfirmed) {
				return ConfStatusUpdate.Redundant;//Already updated, no logging/updating needed.
			}
			Appointment appointmentOld=appointment.Copy();
			void logError(string reason) {
				string err=$"Unable to update Appointment confirmation status {reason}. AptNum: {confirmationRequest.ApptNum}; ConfirmationRequest: {confirmationRequest.ConfirmationRequestNum}"
					+"Attempted to change from "+Defs.GetName(DefCat.ApptConfirmed,appointmentOld.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,defNumApptConfirmed)
					+" due to an eConfirmation.";
				SecurityLogs.MakeLogEntry(EnumPermType.ApptConfirmStatusEdit,appointment.PatNum,err,appointment.AptNum,LogSources.AutoConfirmations,appointmentOld.DateTStamp);
				logOnError?.Invoke(err);
			}
			List<long> preventChangeFrom=commaListOfExcludedDefNums.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
			if(preventChangeFrom.Contains(appointment.Confirmed)) { //This appointment is in a confirmation state that can no longer be updated.
				logError("from "+Defs.GetName(DefCat.ApptConfirmed,appointmentOld.Confirmed));
				return ConfStatusUpdate.Failure;
			}
			else if(appointment.AptStatus.In(ApptStatus.Broken,ApptStatus.UnschedList,ApptStatus.Complete)) {//Broken,Unscheduled, and Complete appointments should not be confirmed.
				logError("when AptStatus is "+appointment.AptStatus.GetDescription());
				return ConfStatusUpdate.Failure;
			}
			else if(confirmationRequest.ApptDateTime!=appointment.AptDateTime) {//Appointments moved from their confirmationrequest original time should not be confirmed.
				logError($"when AptDateTime has changed since sending Confirmation.  AptDateTimeOrig: { confirmationRequest.ApptDateTime }; AptDatetime: { appointment.AptDateTime }");
				return ConfStatusUpdate.Failure;
			}
			//Keep the update small.
			appointment.Confirmed=defNumApptConfirmed;
			Appointments.Update(appointment,appointmentOld);//Appointments S-Class handles Signalods
			SecurityLogs.MakeLogEntry(EnumPermType.ApptConfirmStatusEdit,appointment.PatNum,"Appointment confirmation status changed from "
				+Defs.GetName(DefCat.ApptConfirmed,appointmentOld.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,appointment.Confirmed)
				+" due to an eConfirmation.",appointment.AptNum,LogSources.AutoConfirmations,appointmentOld.DateTStamp);
			EServiceLogs.MakeLogEntry(eServiceAction.CONFConfirmedAppt,eServiceType.ApptConfirmations,FKeyType.ApptNum,patNum:appointment.PatNum,FKey:appointment.AptNum,
				clinicNum:appointment.ClinicNum);
			return ConfStatusUpdate.Success;
		}

		public static ApptReminderRule CreateDefaultReminderRule(ApptReminderType apptReminderTypeRule,long clinicNum = 0,bool isBeforeAppointment = true) {
			Meth.NoCheckMiddleTierRole();
			ApptReminderRule apptReminderRule=null;
			bool canUseCalendarTag=PrefC.GetBool(PrefName.ApptConfirmAutoEnabled);
			if(PrefC.HasClinicsEnabled){
				canUseCalendarTag=ClinicPrefs.GetBool(PrefName.ApptConfirmAutoEnabled,clinicNum);
			}
			string strAddToCalendar="";
			if(canUseCalendarTag){
				strAddToCalendar= " To add this to your calendar, visit [AddToCalendar].";	
			}
			string strAddToCalendarPerAppt="";
			if(canUseCalendarTag){
				strAddToCalendarPerAppt=" Add to calendar: [AddToCalendar]";
			}
			switch(apptReminderTypeRule) {
				case ApptReminderType.Reminder:
					apptReminderRule=new ApptReminderRule() {
						ClinicNum=clinicNum,//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
						TypeCur=ApptReminderType.Reminder,
						TSPrior=TimeSpan.FromHours(3),
						TemplateSMS="Appointment Reminder: [NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]. [Premed]If you have questions call [ClinicPhone]."
						+strAddToCalendar,//default message
						TemplateEmail=@"[NameF],

Your appointment is scheduled for [ApptTime] on [ApptDate] at [OfficeName]."+strAddToCalendar
						+@" [Premed]If you have questions, call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailSubject="Appointment Reminder",//default subject
						TemplateSMSAggShared="Appointment Reminder:\n[Appts]\n [Premed]If you have questions call [ClinicPhone].",
						TemplateSMSAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]."+strAddToCalendarPerAppt,
						TemplateEmailSubjAggShared="Appointment Reminder",
						TemplateEmailAggShared=@"[Appts]
[Premed]
If you have questions, call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]."+strAddToCalendarPerAppt,
						//SendOrder="0,1,2" //part of ctor
					};
					break;
				case ApptReminderType.ConfirmationFutureDay:
					apptReminderRule=new ApptReminderRule();
					apptReminderRule.ClinicNum=clinicNum;//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
					apptReminderRule.TypeCur=ApptReminderType.ConfirmationFutureDay;
					apptReminderRule.TSPrior=TimeSpan.FromDays(7);
					apptReminderRule.TemplateSMS="[NameF] is scheduled for [ApptTime] on [ApptDate] at [OfficeName]. Reply [ConfirmCode] to confirm or call [OfficePhone]."+strAddToCalendar;//default message
					apptReminderRule.TemplateEmail=@"[NameF], 

Your appointment is scheduled for [ApptTime] on [ApptDate] at [OfficeName]. Click <a href=""[ConfirmURL]"">[ConfirmURL]</a> to confirm "+
@"or call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>."+strAddToCalendar;
					apptReminderRule.TemplateEmailSubject="Appointment Confirmation";//default subject
					apptReminderRule.TemplateSMSAggShared="[Appts]\nReply [ConfirmCode] to confirm or call [OfficePhone].";
					apptReminderRule.TemplateSMSAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]."+strAddToCalendarPerAppt;
					apptReminderRule.TemplateEmailSubjAggShared="Appointment Confirmation";
					apptReminderRule.TemplateEmailAggShared=@"[Appts]
Click <a href=""[ConfirmURL]"">[ConfirmURL]</a> to confirm or call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.";
					apptReminderRule.TemplateEmailAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]."+strAddToCalendarPerAppt;
					//SendOrder="0,1,2" //part of ctor
					apptReminderRule.DoNotSendWithin=TimeSpan.FromDays(1).Add(TimeSpan.FromHours(10));
					apptReminderRule.TemplateAutoReply="Thank you for confirming your appointment with [OfficeName].  We look forward to seeing you."+strAddToCalendar;
					apptReminderRule.TemplateAutoReplyAgg="Thank you for confirming your appointments with [OfficeName].  We look forward to seeing you"+strAddToCalendarPerAppt;
					apptReminderRule.TemplateFailureAutoReply="There was an error confirming your appointment with [OfficeName]. Please call [OfficePhone] to confirm.";
					apptReminderRule.IsAutoReplyEnabled=true;
					break;
				case ApptReminderType.PatientPortalInvite:
					if(isBeforeAppointment) {
						apptReminderRule=new ApptReminderRule();
						apptReminderRule.ClinicNum=clinicNum;
						apptReminderRule.TypeCur=ApptReminderType.PatientPortalInvite;
						apptReminderRule.TSPrior=TimeSpan.FromDays(7);
						apptReminderRule.TimeSpanMultipleInvites=TimeSpan.FromDays(30);
						apptReminderRule.TemplateEmail=@"[NameF],
			
In preparation for your upcoming dental appointment at [OfficeName], we invite you to visit our <a href=""[PatientPortalURL]"">Patient Portal</a> to see your health information. "+@"
There you can view your scheduled appointments, view your treatment plan, send a message to your provider, and view your account balance. "+@"
If this is your first time using Patient Portal, use the temporary username and password below to log in:

Username: [UserName]
Password: [Password]

If you have any questions, please give us a call at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>, and we would be happy to answer any of your questions.";
						apptReminderRule.TemplateEmailSubject="Patient Portal Invitation";
						apptReminderRule.TemplateEmailSubjAggShared="Patient Portal Invitation";
						apptReminderRule.TemplateEmailAggShared=@"[NameF],
			
In preparation for your upcoming dental appointments at [OfficeName], we invite you to visit our <a href=""[PatientPortalURL]"">Patient Portal</a> to see your health information. "+@"
There you can view your scheduled appointments, view your treatment plan, send a message to your provider, and view your account balance. "+@"
If this is your first time using Patient Portal, use these temporary usernames and passwords below to log in:

[Credentials]
If you have any questions, please give us a call at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>, and we would be happy to answer any of your questions.";
						apptReminderRule.TemplateEmailAggPerAppt=@"[NameF]
User name: [UserName]
Password: [Password]
";
						apptReminderRule.SendOrder="2"; //Email only
						break;
					}
					else {//Same day
						apptReminderRule=new ApptReminderRule();
						apptReminderRule.ClinicNum=clinicNum;
						apptReminderRule.TypeCur=ApptReminderType.PatientPortalInvite;
						apptReminderRule.TSPrior=new TimeSpan(-1,0,0);//Send 1 hour after the appointment
						apptReminderRule.TimeSpanMultipleInvites=TimeSpan.FromDays(30);
						apptReminderRule.TemplateEmail=@"[NameF],
			
Thank you for coming in to visit [OfficeName] today. As a follow up to your appointment, we invite you to visit our <a href=""[PatientPortalURL]"">Patient Portal</a> to see your health information. "+@"
There you can view your scheduled appointments, view your treatment plan, send a message to your provider, and view your account balance. "+@"
If this is your first time using Patient Portal, use this temporary username and password to log in:

Username: [UserName]
Password: [Password]

If you have any questions, please give us a call at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>, and we would be happy to answer any of your questions.";
						apptReminderRule.TemplateEmailSubject="Patient Portal Invitation";
						apptReminderRule.TemplateEmailSubjAggShared="Patient Portal Invitation";
						apptReminderRule.TemplateEmailAggShared=@"[NameF],
			
Thank you for coming in to visit [OfficeName] today. As a follow up to your appointment, we invite you to visit our <a href=""[PatientPortalURL]"">Patient Portal</a> to see your health information. "+@"
There you can view your scheduled appointments, view your treatment plan, send a message to your provider, and view your account balance. "+@"
Visit <a href=""[PatientPortalURL]"">Patient Portal</a> to see your health information.
If this is your first time using Patient Portal, use these temporary usernames and passwords to log in:

[Credentials]
If you have any questions, please give us a call at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>, and we would be happy to answer any of your questions.";
						apptReminderRule.TemplateEmailAggPerAppt=@"[NameF]
User name: [UserName]
Password: [Password]
";
						apptReminderRule.SendOrder="2";//Email only
						break;
					}
				case ApptReminderType.ScheduleThankYou:
					apptReminderRule=new ApptReminderRule();
					apptReminderRule.ClinicNum=clinicNum;//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
					apptReminderRule.TypeCur=ApptReminderType.ScheduleThankYou;
					apptReminderRule.TSPrior=new TimeSpan(-1,0,0);//default to send thank you 1 hour after creating appointment.
					apptReminderRule.TemplateSMS="[NameF], thank you for scheduling with [OfficeName] on [ApptDate] at [ApptTime]."
						+strAddToCalendar;//default message
					apptReminderRule.TemplateEmail=@"[NameF],

Thank you for scheduling your appointment with [OfficeName] on [ApptDate] at [ApptTime]."+strAddToCalendar
						+@" If you have questions, call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.";
					apptReminderRule.TemplateEmailSubject="Appointment Thank You";//default subject
					apptReminderRule.TemplateSMSAggShared="Thank you for scheduling these appointments: [Appts]";
					apptReminderRule.TemplateSMSAggPerAppt="[NameF] for [ApptTime] on [ApptDate] at [ClinicName]."+strAddToCalendarPerAppt;
					apptReminderRule.TemplateEmailSubjAggShared="Appointment Thank You";
					apptReminderRule.TemplateEmailAggShared=@"Thank you for scheduling these appointments: 
[Appts]
If you have questions, call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.";
					apptReminderRule.TemplateEmailAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]."+strAddToCalendarPerAppt;
					//SendOrder="0,1,2" //part of ctor
					apptReminderRule.DoNotSendWithin=new TimeSpan(2,0,0);//Do not send within 2 hours of appointment.AptDateTime.
					break;
				case ApptReminderType.NewPatientThankYou:
					apptReminderRule=new ApptReminderRule();
					apptReminderRule.ClinicNum=clinicNum;//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
					apptReminderRule.TypeCur=ApptReminderType.NewPatientThankYou;
					apptReminderRule.TSPrior=new TimeSpan(-1,0,0);//default to send thank you 1 hour after creating appointment.
					apptReminderRule.TemplateSMS=@"[NameF] has an appointment coming up. Please fill out this form prior to the appointment [NewPatWebFormURL] ";//default message
					apptReminderRule.TemplateEmail=@"[NameF] has an appointment coming up. Please fill out this form prior to the appointment <a href=""[NewPatWebFormURL]"">[NewPatWebFormURL]</a> ";
					apptReminderRule.TemplateEmailSubject="New Patient Thank You";//default subject
					apptReminderRule.TemplateSMSAggShared="Thank you for scheduling your appointments. \nPlease fill out these forms for each patient: [Appts]";
					apptReminderRule.TemplateSMSAggPerAppt=@"[NameF] [NewPatWebFormURL]";
					apptReminderRule.TemplateEmailSubjAggShared="New Patient, Thank you";
					apptReminderRule.TemplateEmailAggShared=@"Thank you for scheduling your appointments. 
Please fill out these forms for each patient:
[Appts]";
					apptReminderRule.TemplateEmailAggPerAppt=@"[NameF] <a href=""[NewPatWebFormURL]"">[NewPatWebFormURL]</a>";
					//SendOrder="0,1,2" //part of ctor
					apptReminderRule.DoNotSendWithin=new TimeSpan(2,0,0);//Do not send within 2 hours of appointment.AptDateTime.
					break;
				case ApptReminderType.Arrival:
					apptReminderRule=new ApptReminderRule();
					apptReminderRule.ClinicNum=clinicNum;//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
					apptReminderRule.TypeCur=ApptReminderType.Arrival;
					apptReminderRule.TSPrior=TimeSpan.FromHours(3);
					apptReminderRule.TemplateSMS=$"[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]. When you arrive, please respond with " +
						$"{ArrivalsTagReplacer.ARRIVED_TAG}. If you have questions call [ClinicPhone].";//default message
					apptReminderRule.TemplateEmail="";// N/A
					apptReminderRule.TemplateEmailSubject="";// N/A
					apptReminderRule.TemplateSMSAggShared=$"[Appts]\nWhen you arrive, please respond with {ArrivalsTagReplacer.ARRIVED_TAG}. If you " +
						"have questions call [ClinicPhone].";
					apptReminderRule.TemplateSMSAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].";
					apptReminderRule.TemplateEmailSubjAggShared="";// N/A
					apptReminderRule.TemplateEmailAggShared="";// N/A
					apptReminderRule.SendOrder=((int)CommType.Text).ToString();//SMS Only
					apptReminderRule.TemplateAutoReply="Please remain outside the office.  You will be contacted shortly to come in for your appointment.";
					apptReminderRule.TemplateComeInMessage="Your appointment is ready. Please come in.";
					apptReminderRule.IsAutoReplyEnabled=true;
					break;
				case ApptReminderType.Birthday:
					apptReminderRule=new ApptReminderRule();
					apptReminderRule.ClinicNum=clinicNum;
					apptReminderRule.TypeCur=ApptReminderType.Birthday;
					apptReminderRule.TSPrior=TimeSpan.FromDays(0);
					//No text gets stored in the templates fields. Text comes from Email Hosting Template.
					apptReminderRule.IsAutoReplyEnabled=false;
					apptReminderRule.EmailHostingTemplateNum=0;//set to 0 for now because this should only ever get called during testing.
					apptReminderRule.SendOrder=((int)CommType.Email).ToString();//Email Only
					break;
				case ApptReminderType.GeneralMessage:
					apptReminderRule=new ApptReminderRule();
					apptReminderRule.ClinicNum=clinicNum;
					apptReminderRule.TypeCur=ApptReminderType.GeneralMessage;
					apptReminderRule.TSPrior=TimeSpan.FromHours(-1);//Send 1 hour after the appointment
					apptReminderRule.SendOrder="0,1,2";//part of ctor
					apptReminderRule.TemplateSMS="Thank you for your visit to [OfficeName]. If you have questions call [OfficePhone].";//default message
					apptReminderRule.TemplateSMSAggShared="Thank you for your visit to [OfficeName]. If you have questions call [OfficePhone].";
					apptReminderRule.TemplateSMSAggPerAppt="";
					apptReminderRule.TemplateEmailSubject="Thank You For Your Visit";//default subject
					apptReminderRule.TemplateEmail=@"Thank you for your visit to [OfficeName]. We look forward to seeing you again. If you have any questions, please call us at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.";
					apptReminderRule.TemplateEmailSubjAggShared="Thank You For Your Visit";
					apptReminderRule.TemplateEmailAggShared=@"Thank you for your visit to [OfficeName]. We look forward to seeing you again. If you have any questions, please call us at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.";
					apptReminderRule.TemplateEmailAggPerAppt="";
					apptReminderRule.IsAutoReplyEnabled=false;
					break;
			}
			if(PrefC.GetBool(PrefName.EmailDisclaimerIsOn)) {
				apptReminderRule.TemplateEmail+="\r\n\r\n\r\n[EmailDisclaimer]";
				apptReminderRule.TemplateEmailAggShared+="\r\n\r\n\r\n[EmailDisclaimer]";
			}
			return apptReminderRule;
		}

		/// <summary>Returns the list of replacement tags available for the passed in ApptReminderRuleType.</summary>
		public static List<string> GetAvailableTags(ApptReminderType apptReminderType) {
			Meth.NoCheckMiddleTierRole();
			List<string> listStringsReplacementTags = new List<string>();
			listStringsReplacementTags.Add("[NameF]");
			listStringsReplacementTags.Add("[NamePreferredOrFirst]");
			listStringsReplacementTags.Add("[ClinicName]");
			listStringsReplacementTags.Add("[ClinicPhone]");
			listStringsReplacementTags.Add("[OfficeName]");
			listStringsReplacementTags.Add("[ProvName]");
			listStringsReplacementTags.Add("[ProvAbbr]");
			listStringsReplacementTags.Add("[PracticeName]");
			listStringsReplacementTags.Add("[PracticePhone]");
			listStringsReplacementTags.Add("[OfficePhone]");
			if(IsForAppointment(apptReminderType)) {
				listStringsReplacementTags.Add("[ApptTime]");
				listStringsReplacementTags.Add("[ApptTimeAskedArrive]");
				listStringsReplacementTags.Add("[ApptDate]");
			}
			switch(apptReminderType) {
				case ApptReminderType.Reminder:
					listStringsReplacementTags.Add("[Premed]");
					listStringsReplacementTags.Add(ApptThankYouSents.ADD_TO_CALENDAR);
					break;
				case ApptReminderType.ConfirmationFutureDay:
					listStringsReplacementTags.Add("[ConfirmCode]");
					listStringsReplacementTags.Add("[ConfirmURL]");
					listStringsReplacementTags.Add(ApptThankYouSents.ADD_TO_CALENDAR);
					break;
				case ApptReminderType.PatientPortalInvite:
					listStringsReplacementTags.Add("[UserName]");
					listStringsReplacementTags.Add("[Password]");
					listStringsReplacementTags.Add("[PatientPortalURL]");
					break;
				case ApptReminderType.ScheduleThankYou:
					listStringsReplacementTags.Add(ApptThankYouSents.ADD_TO_CALENDAR);
					break;
				case ApptReminderType.Arrival:
					listStringsReplacementTags.Add(ArrivalsTagReplacer.ARRIVED_TAG);
					break;
				case ApptReminderType.NewPatientThankYou:
					listStringsReplacementTags.Add(ApptNewPatThankYouSents.NEW_PAT_WEB_FORM_TAG);
					break;
				case ApptReminderType.PayPortalMsgToPay:
					listStringsReplacementTags.Add(MsgToPayTagReplacer.MSG_TO_PAY_TAG);
					listStringsReplacementTags.Add(MsgToPayTagReplacer.MONTHLY_CARD_TAG);
					listStringsReplacementTags.Add(MsgToPayTagReplacer.NAME_PREF_TAG);
					listStringsReplacementTags.Add(MsgToPayTagReplacer.PATNUM_TAG);
					listStringsReplacementTags.Add(MsgToPayTagReplacer.CURMONTH_TAG);
					listStringsReplacementTags.Add(MsgToPayTagReplacer.STATEMENT_URL_TAG);
					listStringsReplacementTags.Add(MsgToPayTagReplacer.STATEMENT_SHORT_TAG);
					listStringsReplacementTags.Add(MsgToPayTagReplacer.STATEMENT_BALANCE_TAG);
					listStringsReplacementTags.Add(MsgToPayTagReplacer.STATEMENT_INS_EST_TAG);
					break;
			}
			listStringsReplacementTags.Sort();//alphabetical
			return listStringsReplacementTags;
		}

		///<summary>Returns true if the remindertype is reliant on having a reminder. This is pretty much only for PaymentPortal Text To Pay currently.</summary>
		public static bool IsForAppointment(ApptReminderType apptReminderType) {
			return EnumTools.GetAttributeOrDefault<ReminderRuleTypeAttribute>(apptReminderType).IsForAppointment;
		}

		public static bool IsReminderTypeAlwaysSendBefore(ApptReminderType apptReminderType) {
			Meth.NoCheckMiddleTierRole();
			return apptReminderType.In(ApptReminderType.Reminder,
			ApptReminderType.ConfirmationFutureDay,
			ApptReminderType.Arrival,
			ApptReminderType.Birthday,
			ApptReminderType.ScheduleThankYou,
			ApptReminderType.NewPatientThankYou,
			ApptReminderType.WebSchedRecall);
		}

		public static bool IsReminderTypeAlwaysSendAfter(ApptReminderType apptReminderType) {
			Meth.NoCheckMiddleTierRole();
			return apptReminderType.In(ApptReminderType.GeneralMessage);
		}

		/// <summary>Returns the list of replacement tags available for the Aggregate Templates for the passed in ApptReminderRuleType.</summary>
		public static List<string> GetAvailableAggTags(ApptReminderType apptReminderType) {
			Meth.NoCheckMiddleTierRole();
			List<string> listReplacementTags=GetAvailableTags(apptReminderType);
			listReplacementTags.Add("[Appts]");//[Appts] is used for child nodes
			listReplacementTags.Sort();
			return listReplacementTags;
		}

		public static bool IsAddToCalendarTagSupported(ApptReminderType apptReminderType) {
			return apptReminderType.In(ApptReminderType.Reminder,ApptReminderType.ScheduleThankYou,ApptReminderType.ConfirmationFutureDay);
		}

		public enum ConfStatusUpdate {
			Success,
			Failure,
			Redundant,
		}
	}
}