using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDentBusiness.AutoComm {
	///<summary>Combines all necessary fields for a given clinic needed for AutoComm.</summary>
	public class ClinicRule{
		///<summary>The number of days out we need to query to account for send windows.</summary>
		public const int QUERY_DATE_BUFFER=1;
		///<summary>True by default. Each implementer can set this from InitClinicRules(). Value of false will skip ProcessRosters().
		///Implementers may want to make use of PreProcessClinicRule() for disabled ClinicRules.</summary>
		public bool IsEnabledByHQ=true;
		public SmsMessageSource MessageSource { get; private set; }
		public Clinic RuleClinic { get; private set; }
		///<summary>All rules in this list are grouped by date and are essentially the same rule, but with different languages.</summary>
		public List<AutoCommRule> ListRules { get; private set; }
		public EmailAddress ClinicEmail { get; private set; }
		public DateTime DateToday { get; private set; }
		public List<DateTime> QueryDates { get; private set; }
		public long ClinicNum { get { return RuleClinic.ClinicNum; } }
		public bool HasTodayAppointments { 
			get { return DefaultRule.IsSameDay && DefaultRule.IsValidDuration && Roster.IsValid && Roster.ListAutoCommObjs.Count>0; } 
		}
		public bool HasFutureDateAppointments { 
			get {	return DefaultRule.IsFutureDay && DefaultRule.IsValidDuration && Roster.IsValid && Roster.ListAutoCommObjs.Count>0; } 
		}
		public bool IsAfterAppt { get { return DefaultRule.TSPrior < TimeSpan.Zero; } }	
		///<summary>Default rule will have all of the same data as the other list of assocaited languages, except for the text fields.
		///No language is associated to the default rule except in the cases where we are overriding to send a specific language. </summary>
		public AutoCommRule DefaultRule {
			get {	
				//Should always have a rule with no language set. This is the default rule.
				AutoCommRule rule=ListRules.FirstOrDefault(x => x.Language=="");
				if(rule==null) {
					rule=ListRules.First();
				}
				return rule;
			}
		}
					
		///<summary>Computed after ClinicRule is constructed and all appointments have been queried from db.</summary>
		public SingleDateRoster Roster { get;	set; }

		public ClinicRule(DateTime dateNow,SmsMessageSource messageSource,Clinic ruleClinic,EmailAddress clinicEmail,List<AutoCommRule> listAutoCommRule,
			bool isEnabledByHQ,bool doesSameDayIncludeYesterday=false) 
		{
			MessageSource=messageSource;
			DateToday=dateNow.Date;
			RuleClinic=ruleClinic??new Clinic();
			ClinicEmail=clinicEmail??new EmailAddress();
			ListRules=listAutoCommRule??new List<AutoCommRule>();
			QueryDates=new List<DateTime>();
			//At least 1 day, which would be today (midnight this morning through midnight tonight).
			if(DefaultRule.IsSameDay && DefaultRule.IsValidDuration) { 
				QueryDates.Add(DateToday);
				if(IsAfterAppt || doesSameDayIncludeYesterday) {
					QueryDates.Add(DateToday.AddDays(-QUERY_DATE_BUFFER));//Include yesterday to catch appointments that were after the end of the send window.
				}
			}
			if(DefaultRule.IsFutureDay && DefaultRule.IsValidDuration) {
				//Add every day from tomorrow up until the NumDaysInFuture.
				//These dates will need to be very carefully grouped into 1 single grouping which belongs to the TSPrior of the FutureDay rule.
				//Processing as multiple groupings will not work and will lead to appointments being processed in perpetuity (this was a bug introduced in 17.2.6 and fixed in 17.2.9).
				for(int i=1;i<=DefaultRule.NumDaysInFuture;i++) {
					QueryDates.Add(DateToday.AddDays(i));
				}
			}
			if(DefaultRule.IsPastDay && DefaultRule.IsValidDuration) {
				//Add every day from yesterday back until the NumDaysInPast. Add one more day to catch appointments that were after the end of the send window.
				for(int i=1;i<=DefaultRule.NumDaysInPast+QUERY_DATE_BUFFER;i++) {
					QueryDates.Add(DateToday.AddDays(-i));
				}
			}
			IsEnabledByHQ=isEnabledByHQ;
		}

		///<summary>Helper class to give structure to appointment lists by date for each ClinicRule.</summary>
		public class SingleDateRoster {
			public DateTime DateOfEvent;
			public int NumDaysInFuture;
			public List<AutoCommObj> ListAutoCommObjs;
			public bool IsValid=false;
		}
	}

	///<summary>Holds the rules for the messages to be sent. This class is almost an exact copy of OpenDentBusiness.ApptReminderRule.</summary>
	public class AutoCommRule {
		///<summary>The primary key of the rule.</summary>
		public long PriKey;
		///<summary>The clinic that the rule belongs to.</summary>
		public long ClinicNum;
		///<summary>The number of days in the future this TSPrior is for.</summary>
		public int NumDaysInFuture;
		///<summary>The number of days in the past this TSPrior is for.</summary>
		public int NumDaysInPast;
		///<summary>True if this rule is enabled and the TSPrior is valid.</summary>
		public bool IsValidDuration;
		///<summary>The amount of time before the event that this rule is for. If the rule is for future events, this field will be positive. If it
		///is for past events, this field will be negative.</summary>
		public TimeSpan TSPrior;
		///<summary>True if this rule is for events in a future day. If this is true, IsPastDay and IsSameDay must be false.</summary>
		public bool IsFutureDay;
		///<summary>True if this rule is for events in a past day. If this is true, IsFutureDay and IsSameDay must be false.</summary>
		public bool IsPastDay;
		///<summary>True if this rule is for events for today. If this is true, IsPastDay and IsFutureDay must be false.</summary>
		public bool IsSameDay;
		///<summary>If true, then send both SMS and email.</summary>
		public bool IsSendAll;
		///<summary>Comma Delimited List of comm types. Enum values of ApptComm.CommType. 0=preferred,1=sms,2=email.</summary>
		public string SendOrder="";
		///<summary>If using SMS, this template will be used to generate the body of the text message.</summary>
		public string TemplateSMS;
		///<summary>Used when aggregating multiple events together into a single message.</summary>
		public string TemplateSMSAggShared;
		///<summary>Used when aggregating multiple events together into a single message.</summary>
		public string TemplateSMSAggPerAppt;
		///<summary>If using email, this template will be used to generate the body of the email.</summary>
		public string TemplateEmail="";
		///<summary>Used when aggregating multiple events together into a single message.</summary>
		public string TemplateEmailAggPerAppt="";
		///<summary>Used when aggregating multiple events together into a single message.</summary>
		public string TemplateEmailAggShared="";
		///<summary>Used when aggregating multiple events together into a single message.</summary>
		public string TemplateEmailSubjAggShared="";
		///<summary>Used when aggregating multiple events together into a single message.</summary>
		public string TemplateEmailSubject="";
		///<summary>The time before the appointment in which this reminder should NOT be sent. E.g., if this value is 2 days, and an appt is created one
		///day in the future, a reminder will not be sent.</summary>
		public TimeSpan DoNotSendWithin;
		///<summary>The language that this rule if for. Can be blank if default.</summary>
		public string Language="";
		///<summary>The type of HTML being used (if any) for this email template. </summary>
		public EmailType EmailTemplateType;
		///<summary>Type of rule that the apt reminder rule is.</summary>
		public ApptReminderType RuleType;
		///<summary>Links to the cooresponding email hosting template for mass emails</summary>
		public long EmailHostingTemplateNum;
		///<summary>True when wanting to send a message to a minor for their birthday. False when no messasge should be sent.</summary>
		public bool IsSendForMinorsBirthday;
		///<summary>Age that the user has defined as being a minor.</summary>
		public int MinorAge;

		public AutoCommRule() { }

		public AutoCommRule(ApptReminderRule aptRemRule) {
			PriKey=aptRemRule.ApptReminderRuleNum;
			ClinicNum=aptRemRule.ClinicNum;
			NumDaysInFuture=aptRemRule.NumDaysInFuture;
			NumDaysInPast=aptRemRule.NumDaysInPast;
			IsValidDuration=aptRemRule.IsValidDuration;
			TSPrior=aptRemRule.TSPrior;
			IsFutureDay=aptRemRule.IsFutureDay;
			IsSameDay=aptRemRule.IsSameDay;
			IsPastDay=aptRemRule.IsPastDay;
			IsSendAll=aptRemRule.IsSendAll;
			SendOrder=aptRemRule.SendOrder;
			TemplateSMS=aptRemRule.TemplateSMS;
			TemplateSMSAggShared=aptRemRule.TemplateSMSAggShared;
			TemplateSMSAggPerAppt=aptRemRule.TemplateSMSAggPerAppt;
			TemplateEmail=aptRemRule.TemplateEmail;
			TemplateEmailAggPerAppt=aptRemRule.TemplateEmailAggPerAppt;
			TemplateEmailAggShared=aptRemRule.TemplateEmailAggShared;
			TemplateEmailSubjAggShared=aptRemRule.TemplateEmailSubjAggShared;
			TemplateEmailSubject=aptRemRule.TemplateEmailSubject;
			DoNotSendWithin=aptRemRule.DoNotSendWithin;
			Language=aptRemRule.Language;
			EmailTemplateType=aptRemRule.EmailTemplateType;
			RuleType=aptRemRule.TypeCur;
			EmailHostingTemplateNum=aptRemRule.EmailHostingTemplateNum;
			IsSendForMinorsBirthday=aptRemRule.IsSendForMinorsBirthday;
			MinorAge=aptRemRule.MinorAge;
		}
	}

}
