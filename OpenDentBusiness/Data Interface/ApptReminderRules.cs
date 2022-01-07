using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Globalization;
using CodeBase;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using OpenDentBusiness.AutoComm;

namespace OpenDentBusiness{

	///<summary></summary>
	public class ApptReminderRules{
		#region Get Methods

		///<summary>Gets all, sorts by TSPrior Desc, Should never be more than 3 (per clinic if this is implemented for clinics.)</summary>
		public static List<ApptReminderRule> GetForTypes(params ApptReminderType[] arrTypes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptReminderRule>>(MethodBase.GetCurrentMethod(),arrTypes);
			}
			if(arrTypes.Length==0) {
				return new List<ApptReminderRule>();
			}
			string command="SELECT * FROM apptreminderrule WHERE TypeCur IN("+string.Join(",",arrTypes.Select(x => POut.Int((int)x)))+")";
			return Crud.ApptReminderRuleCrud.SelectMany(command);
		}

		///<summary>Gets all from the DB for the given clinic and types.</summary>
		public static List<ApptReminderRule> GetForClinicAndTypes(long clinicNum,params ApptReminderType[] arrTypes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptReminderRule>>(MethodBase.GetCurrentMethod(),clinicNum,arrTypes);
			}
			if(arrTypes.Length==0) {
				return new List<ApptReminderRule>();
			}
			string command = "SELECT * FROM apptreminderrule WHERE ClinicNum="+POut.Long(clinicNum)
				+" AND TypeCur IN("+string.Join(",",arrTypes.Select(x => POut.Int((int)x)))+")";
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			return Db.GetLong("SELECT count(*) FROM ApptReminderRule WHERE TSPrior>0")>0;
		}

		#endregion

		///<summary>Gets all, sorts by TSPrior Desc, Should never be more than 3 (per clinic if this is implemented for clinics.)</summary>
		public static List<ApptReminderRule> GetAll(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<bool>>(System.Reflection.MethodBase.GetCurrentMethod());
			}
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
			return GetForClinic(0);
		}

		///<summary>Gets all from the DB, sorts by TSPrior Desc.</summary>
		public static List<ApptReminderRule> GetForClinic(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptReminderRule>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command = "SELECT * FROM apptreminderrule WHERE ClinicNum="+POut.Long(clinicNum);
			return Crud.ApptReminderRuleCrud.SelectMany(command).OrderByDescending(x => x.TSPrior.TotalMinutes).ToList();
		}
		
		public static bool SyncByClinicAndTypes(List<ApptReminderRule> listNew,long clinicNum,params ApptReminderType[] arrTypes) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,clinicNum,arrTypes);
			}
			if(arrTypes.Length==0) {
				return false;
			}
			List<ApptReminderRule> listOld=ApptReminderRules.GetForClinicAndTypes(clinicNum,arrTypes);//ClinicNum can be 0
			if(Crud.ApptReminderRuleCrud.Sync(listNew,listOld)) {
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,string.Join(", ",arrTypes.Select(x => x.GetDescription()))
					+" rules changed for ClinicNum: "+clinicNum.ToString()+".");
				return true;
			}
			return false;
		}

		///<summary>Gets one ApptReminderRule from the db.</summary>
		public static ApptReminderRule GetOne(long apptReminderRuleNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ApptReminderRule>(MethodBase.GetCurrentMethod(),apptReminderRuleNum);
			}
			return Crud.ApptReminderRuleCrud.SelectOne(apptReminderRuleNum);
		}

		///<summary></summary>
		public static long Insert(ApptReminderRule apptReminderRule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				apptReminderRule.ApptReminderRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),apptReminderRule);
				return apptReminderRule.ApptReminderRuleNum;
			}
			return Crud.ApptReminderRuleCrud.Insert(apptReminderRule);
		}

		///<summary></summary>
		public static void Update(ApptReminderRule apptReminderRule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderRule);
				return;
			}
			Crud.ApptReminderRuleCrud.Update(apptReminderRule);
		}

		///<summary></summary>
		public static void Delete(long apptReminderRuleNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptReminderRuleNum);
				return;
			}
			Crud.ApptReminderRuleCrud.Delete(apptReminderRuleNum);
		}

		///<summary>Update Appointment.Confirmed. Returns true if update was allowed. Returns false if it was prevented.</summary>
		public static ConfStatusUpdate UpdateAppointmentConfirmationStatus(ConfirmationRequest request,long confirmDefNum,string commaListOfExcludedDefNums
			,Action<string> logOnError=null) 
		{
			Appointment aptCur=Appointments.GetOneApt(request.ApptNum);
			if(aptCur==null) {
				return ConfStatusUpdate.Failure;
			}
			if(aptCur.Confirmed==confirmDefNum) {				
				return ConfStatusUpdate.Redundant;//Already updated, no logging/updating needed.
			}
			Appointment aptOld=aptCur.Copy();
			void logError(string reason) {
				string err=$"Unable to update Appointment confirmation status {reason}. AptNum: {request.ApptNum}; ConfirmationRequest: {request.ConfirmationRequestNum}"
					+"Attempted to change from "+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,confirmDefNum)
					+" due to an eConfirmation.";
				SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,aptCur.PatNum,err,aptCur.AptNum,LogSources.AutoConfirmations,aptOld.DateTStamp);
				logOnError?.Invoke(err);
			}
			List<long> preventChangeFrom=commaListOfExcludedDefNums.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
			if(preventChangeFrom.Contains(aptCur.Confirmed)) { //This appointment is in a confirmation state that can no longer be updated.
				logError("from "+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed));
				return ConfStatusUpdate.Failure;
			}
			else if(ListTools.In(aptCur.AptStatus,ApptStatus.Broken,ApptStatus.UnschedList,ApptStatus.Complete)) {//Broken,Unscheduled, and Complete appointments should not be confirmed.
				logError("when AptStatus is "+aptCur.AptStatus.GetDescription());
				return ConfStatusUpdate.Failure;
			}
			else if(request.AptDateTimeOrig!=aptCur.AptDateTime) {//Appointments moved from their confirmationrequest original time should not be confirmed.
				logError($"when AptDateTime has changed since sending Confirmation.  AptDateTimeOrig: { request.AptDateTimeOrig }; AptDatetime: { aptCur.AptDateTime }");
				return ConfStatusUpdate.Failure;
			}
			//Keep the update small.
			aptCur.Confirmed=confirmDefNum;
			Appointments.Update(aptCur,aptOld);//Appointments S-Class handles Signalods
			SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,aptCur.PatNum,"Appointment confirmation status changed from "
				+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,aptCur.Confirmed)
				+" due to an eConfirmation.",aptCur.AptNum,LogSources.AutoConfirmations,aptOld.DateTStamp);
			EServiceLogs.MakeLogEntry(eServiceAction.CONFConfirmedAppt,eServiceType.ApptConfirmations,FKeyType.ApptNum,patNum:aptCur.PatNum,FKey:aptCur.AptNum,
				clinicNum:aptCur.ClinicNum);
			return ConfStatusUpdate.Success;		
		}

		public static ApptReminderRule CreateDefaultReminderRule(ApptReminderType ruleType,long clinicNum = 0,bool isBeforeAppointment = true) {
			ApptReminderRule rule=null;
			switch(ruleType) {
				case ApptReminderType.Reminder:
					rule=new ApptReminderRule() {
						ClinicNum=clinicNum,//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
						TypeCur=ApptReminderType.Reminder,
						TSPrior=TimeSpan.FromHours(3),
						TemplateSMS="Appointment Reminder: [NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]. If you have questions call [ClinicPhone].",//default message
						TemplateEmail=@"[NameF],

Your appointment is scheduled for [ApptTime] on [ApptDate] at [OfficeName]. If you have questions, call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailSubject="Appointment Reminder",//default subject
						TemplateSMSAggShared="Appointment Reminder:\n[Appts]\nIf you have questions call [ClinicPhone].",
						TemplateSMSAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].",
						TemplateEmailSubjAggShared="Appointment Reminder",
						TemplateEmailAggShared=@"[Appts]
If you have questions, call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].",
						//SendOrder="0,1,2" //part of ctor
					};
					break;
				case ApptReminderType.ConfirmationFutureDay:
					rule=new ApptReminderRule() {
						ClinicNum=clinicNum,//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
						TypeCur=ApptReminderType.ConfirmationFutureDay,
						TSPrior=TimeSpan.FromDays(7),
						TemplateSMS="[NameF] is scheduled for [ApptTime] on [ApptDate] at [OfficeName]. Reply [ConfirmCode] to confirm or call [OfficePhone].",//default message
						TemplateEmail=@"[NameF], 

Your appointment is scheduled for [ApptTime] on [ApptDate] at [OfficeName]. Click <a href=""[ConfirmURL]"">[ConfirmURL]</a> to confirm "+
@"or call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailSubject="Appointment Confirmation",//default subject
						TemplateSMSAggShared="[Appts]\nReply [ConfirmCode] to confirm or call [OfficePhone].",
						TemplateSMSAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].",
						TemplateEmailSubjAggShared="Appointment Confirmation",
						TemplateEmailAggShared=@"[Appts]
Click <a href=""[ConfirmURL]"">[ConfirmURL]</a> to confirm or call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].",
						//SendOrder="0,1,2" //part of ctor
						DoNotSendWithin=TimeSpan.FromDays(1).Add(TimeSpan.FromHours(10)),
						TemplateAutoReply="Thank you for confirming your appointment with [OfficeName].  We look forward to seeing you.",
						TemplateAutoReplyAgg="Thank you for confirming your appointments with [OfficeName].  We look forward to seeing you",
						TemplateFailureAutoReply="There was an error confirming your appointment with [OfficeName]. Please call [OfficePhone] to confirm.",
						IsAutoReplyEnabled=true,
					};
					break;
				case ApptReminderType.PatientPortalInvite:
					if(isBeforeAppointment) {
						rule=new ApptReminderRule() {
							ClinicNum=clinicNum,
							TypeCur=ApptReminderType.PatientPortalInvite,
							TSPrior=TimeSpan.FromDays(7),
							TemplateEmail=@"[NameF],
			
In preparation for your upcoming dental appointment at [OfficeName], we invite you to log in to our Patient Portal. "+@"
There you can view your scheduled appointments, view your treatment plan, send a message to your provider, and view your account balance. "+@"
Visit our <a href=""[PatientPortalURL]"">Patient Portal</a> and use this temporary user name and password to log in:

User name: [UserName]
Password: [Password]

If you have any questions, please give us a call at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>, and we would be happy to answer any of your questions.",
							TemplateEmailSubject="Patient Portal Invitation",
							TemplateEmailSubjAggShared="Patient Portal Invitation",
							TemplateEmailAggShared=@"[NameF],
			
In preparation for your upcoming dental appointments at [OfficeName], we invite you to log in to our Patient Portal. "+@"
There you can view your scheduled appointments, view your treatment plan, send a message to your provider, and view your account balance. "+@"
Visit our <a href=""[PatientPortalURL]"">Patient Portal</a> and use these temporary user names and passwords to log in:

[Credentials]
If you have any questions, please give us a call at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>, and we would be happy to answer any of your questions.",
							TemplateEmailAggPerAppt=@"[NameF]
User name: [UserName]
Password: [Password]
",
							SendOrder="2" //Email only
						};
						break;
					}
					else {//Same day
						rule=new ApptReminderRule() {
							ClinicNum=clinicNum,
							TypeCur=ApptReminderType.PatientPortalInvite,
							TSPrior=new TimeSpan(-1,0,0),//Send 1 hour after the appointment
							TemplateEmail=@"[NameF],
			
Thank you for coming in to visit [OfficeName] today. As a follow up to your appointment, we invite you to log in to our Patient Portal. "+@"
There you can view your scheduled appointments, view your treatment plan, send a message to your provider, and view your account balance. "+@"
Visit <a href=""[PatientPortalURL]"">Patient Portal</a> and use this temporary user name and password to log in:

User name: [UserName]
Password: [Password]

If you have any questions, please give us a call at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>, and we would be happy to answer any of your questions.",
							TemplateEmailSubject="Patient Portal Invitation",
							TemplateEmailSubjAggShared="Patient Portal Invitation",
							TemplateEmailAggShared=@"[NameF],
			
Thank you for coming in to visit [OfficeName] today. As a follow up to your appointment, we invite you to log in to our Patient Portal. "+@"
There you can view your scheduled appointments, view your treatment plan, send a message to your provider, and view your account balance. "+@"
Visit <a href=""[PatientPortalURL]"">Patient Portal</a> and use these temporary user names and passwords to log in:

[Credentials]
If you have any questions, please give us a call at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>, and we would be happy to answer any of your questions.",
							TemplateEmailAggPerAppt=@"[NameF]
User name: [UserName]
Password: [Password]
",
							SendOrder="2" //Email only
						};
						break;
					}
				case ApptReminderType.ScheduleThankYou:
					string strAddToCalendar=PrefC.GetBool(PrefName.ApptConfirmAutoEnabled) ? " To add this to your calendar, visit [AddToCalendar]." : "";
					string strAddToCalendarPerAppt=PrefC.GetBool(PrefName.ApptConfirmAutoEnabled) ? " Add to calendar: [AddToCalendar]" : "";
					rule=new ApptReminderRule() {
						ClinicNum=clinicNum,//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
						TypeCur=ApptReminderType.ScheduleThankYou,
						TSPrior=new TimeSpan(-1,0,0),//default to send thank you 1 hour after creating appointment.
						TemplateSMS="[NameF], thank you for scheduling with [OfficeName] on [ApptDate] at [ApptTime]."
							+strAddToCalendar,//default message
						TemplateEmail=@"[NameF],

Thank you for scheduling your appointment with [OfficeName] on [ApptDate] at [ApptTime]."+strAddToCalendar
							+@" If you have questions, call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailSubject="Appointment Thank You",//default subject
						TemplateSMSAggShared="Thank you for scheduling these appointments: [Appts]",
						TemplateSMSAggPerAppt="[NameF] for [ApptTime] on [ApptDate] at [ClinicName]."+strAddToCalendarPerAppt,
						TemplateEmailSubjAggShared="Appointment Thank You",
						TemplateEmailAggShared=@"Thank you for scheduling these appointments: 
[Appts]
If you have questions, call <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]."+strAddToCalendarPerAppt,
						//SendOrder="0,1,2" //part of ctor					
						DoNotSendWithin=new TimeSpan(2,0,0),//Do not send within 2 hours of appointment.AptDateTime.
					};
					break;					
				case ApptReminderType.Arrival:
					rule=new ApptReminderRule() {
						ClinicNum=clinicNum,//works with practice too because _listClinics[0] is a spoofed "Practice/Defaults" clinic with ClinicNum=0
						TypeCur=ApptReminderType.Arrival,
						TSPrior=TimeSpan.FromHours(3),
						TemplateSMS=$"[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName]. When you arrive, please respond with " +
							$"{ArrivalsTagReplacer.ARRIVED_TAG}. If you have questions call [ClinicPhone].",//default message
						TemplateEmail="",// N/A
						TemplateEmailSubject="",// N/A
						TemplateSMSAggShared=$"[Appts]\nWhen you arrive, please respond with {ArrivalsTagReplacer.ARRIVED_TAG}. If you " +
							"have questions call [ClinicPhone].",
						TemplateSMSAggPerAppt="[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].",
						TemplateEmailSubjAggShared="",// N/A
						TemplateEmailAggShared="",// N/A
						SendOrder=((int)CommType.Text).ToString(),//SMS Only
						TemplateAutoReply="Please remain outside the office.  You will be contacted shortly to come in for your appointment.",
						TemplateComeInMessage="Your appointment is ready. Please come in.",
						IsAutoReplyEnabled=true,
					};
					break;
				case ApptReminderType.Birthday:
					rule=new ApptReminderRule() {
						ClinicNum=clinicNum,
						TypeCur=ApptReminderType.Birthday,
						TSPrior=TimeSpan.FromDays(0),
						//No text gets stored in the templates fields. Text comes from Email Hosting Template.
						IsAutoReplyEnabled=false,
						EmailHostingTemplateNum=0,//set to 0 for now because this should only ever get called during testing.
						SendOrder=((int)CommType.Email).ToString(),//Email Only
					};
					break;
				case ApptReminderType.GeneralMessage:
					rule=new ApptReminderRule() {
						ClinicNum=clinicNum,
						TypeCur=ApptReminderType.GeneralMessage,
						TSPrior=TimeSpan.FromHours(-1),//Send 1 hour after the appointment
						SendOrder="0,1,2",//part of ctor
						TemplateSMS="Thank you for your visit to [OfficeName]. If you have questions call [OfficePhone].",//default message
						TemplateSMSAggShared="Thank you for your visit to [OfficeName]. If you have questions call [OfficePhone].",
						TemplateSMSAggPerAppt="",
						TemplateEmailSubject="Thank You For Your Visit",//default subject
						TemplateEmail=@"Thank you for your visit to [OfficeName]. We look forward to seeing you again. If you have any questions, please call us at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailSubjAggShared="Thank You For Your Visit",
						TemplateEmailAggShared=@"Thank you for your visit to [OfficeName]. We look forward to seeing you again. If you have any questions, please call us at <a href=""tel:[OfficePhone]"">[OfficePhone]</a>.",
						TemplateEmailAggPerAppt="",
						IsAutoReplyEnabled=false,
					};
					break;
			}
			if(PrefC.GetBool(PrefName.EmailDisclaimerIsOn)) {
				rule.TemplateEmail+="\r\n\r\n\r\n[EmailDisclaimer]";
				rule.TemplateEmailAggShared+="\r\n\r\n\r\n[EmailDisclaimer]";
			}
			return rule;
		}

		/// <summary>Returns the list of replacement tags available for the passed in ApptReminderRuleType.</summary>
		public static List<string> GetAvailableTags(ApptReminderType type) {
			List<string> retVal = new List<string>() {
				"[NameF]",
				"[ApptTime]",
				"[ApptTimeAskedArrive]",
				"[ApptDate]",
				"[ClinicName]",
				"[ClinicPhone]",
				"[ProvName]",
				"[ProvAbbr]",
				"[PracticeName]",
				"[PracticePhone]"
			};
			switch(type) {
				case ApptReminderType.ConfirmationFutureDay:
					retVal.AddRange(new[] {
						"[ConfirmCode]",
						"[ConfirmURL]"
					});
					break;
				case ApptReminderType.PatientPortalInvite:
					retVal.AddRange(new[] {
						"[UserName]",
						"[Password]",
						"[PatientPortalURL]"
					});
					break;
				case ApptReminderType.ScheduleThankYou:
					retVal.AddRange(new[] {
						ApptThankYouSents.ADD_TO_CALENDAR,
					});
					break;
				case ApptReminderType.Arrival:
					retVal.AddRange(new[] {
						ArrivalsTagReplacer.ARRIVED_TAG,
					});
					break;
			}
			retVal.Sort();//alphabetical
			return retVal;
		}	

		/// <summary>Returns the list of replacement tags available for the Aggregate Templates for the passed in ApptReminderRuleType.</summary>
		public static List<string> GetAvailableAggTags(ApptReminderType type) {
			List<string> retVal=GetAvailableTags(type);
			retVal.Add("[Appts]");//[Appts] is used for child nodes
			retVal.Sort();
			return retVal;
		}

		public enum ConfStatusUpdate {
			Success,
			Failure,
			Redundant,
		}
	}
}