using System;

namespace OpenDentBusiness.AutoComm {
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
		public string TemplateSMS="";
		///<summary>Used when aggregating multiple events together into a single message.</summary>
		public string TemplateSMSAggShared="";
		///<summary>Used when aggregating multiple events together into a single message.</summary>
		public string TemplateSMSAggPerAppt="";
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
		///<summary>Enum:SendMultipleInvites . Whether we are able to send multiple invites.</summary>
		public SendMultipleInvites SendMultipleInvites;
		///<summary>Used in conjunction with CanSendMultipleInvites. We will not send an invite if a patient has visited Patient Portal within this timespan.</summary>
		public TimeSpan TimeSpanMultipleInvites;

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
			SendMultipleInvites=aptRemRule.SendMultipleInvites;
			TimeSpanMultipleInvites=aptRemRule.TimeSpanMultipleInvites;
		}
	}

}
