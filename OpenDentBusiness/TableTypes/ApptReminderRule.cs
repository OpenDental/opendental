using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness {
	///<summary> Appointment Reminder Rules are used to track the automated generation and sending of appointment reminders and confirmations. 
	/// Users are allowed to define up to two reminders and one confirmation (per clinic.) These can be sent out any number of Days, Hours, and/or 
	/// Minutes before a scheduled appointment.
	/// <para>PRACTICE - Appointment Reminder Rules will be saved and edited with clinicNum=0. This denotes the "Defaults" when using clinics, 
	/// but for a practice the defaults become the practice rules.</para>
	///<para>CLINICS - When using clinics, each clinic has a bool "IsConfirmEnabled" that determines if a particular clinic has automated reminders/confirmations
	///enabled. If not, no reminders will be sent out for the clinic. If enabled, and no rules are defined for the clinic, then the clinic will attempt to use the
	///defaults that have been defined with clinicNum==0. If a clinic is enabled and has at least one AppointmentReminderRule defined, then NO defaults will be
	///used for that clinic.</para>
	///<para>REMINDERS - reminders are sent out using the ApptComm system implemented by DG. These used to be stored as preferences for the practice only,
	///now users are allowed to define them on a per clinic basis. Reminders should be considered one way communications and should not be desingned with a
	///customer response in mind.</para>
	///<para>CONFIRMATIONS - confirmations are sent using the new automated-confirmation system implemented by RM (proper) and SO (web backend). Confirmations
	///are intended to allow end patients to respond to OpenDental via text or email and automatically confirm, or set to a desired status, the appointments
	///on the schedule.</para>
	/// </summary>
	[Serializable()]
	[CrudTable(IsSynchable =true)]
	public class ApptReminderRule : TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey = true)]
		public long ApptReminderRuleNum;
		///<summary>Enum:ApptReminderType </summary>
		public ApptReminderType TypeCur;
		///<summary>Time before appointment that this confirmation should be sent.</summary>
		[XmlIgnore]
		[CrudColumn(SpecialType = CrudSpecialColType.TimeSpanLong)]
		public TimeSpan TSPrior;
		///<summary>Comma Delimited List of comm types. Enum values of ApptComm.CommType. 0=pref,1=sms,2=email; Like the deprecated pref "ApptReminderSendOrder"</summary>
		public string SendOrder;
		///<summary>Set to True if both an email AND a text should be sent.</summary>
		public bool IsSendAll;
		///<summary>If using SMS, this template will be used to generate the body of the text message.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateSMS;
		///<summary>If using email, this template will be used to generate the subject of the email.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateEmailSubject;
		///<summary>If using email, this template will be used to generate the body of the email.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateEmail;
		///<summary>FK to clinic.ClinicNum.  Allows reminder rules to be configured on a per clinic basis. If ClinicNum==0 then it is the practice/HQ/default settings.</summary>
		public long ClinicNum;
		///<summary>Used when aggregating multiple appointments together into a single message.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateSMSAggShared;
		///<summary>Used when aggregating multiple appointments together into a single message.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateSMSAggPerAppt;
		///<summary>Used when aggregating multiple appointments together into a single message.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateEmailSubjAggShared;
		///<summary>Used when aggregating multiple appointments together into a single message.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateEmailAggShared;
		///<summary>Used when aggregating multiple appointments together into a single message.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateEmailAggPerAppt;
		///<summary>The time before the appointment in which this reminder should NOT be sent. E.g., if this value is 2 days, and an appt is created one
		///day in the future, a reminder will not be sent.</summary>
		[XmlIgnore]
		[CrudColumn(SpecialType = CrudSpecialColType.TimeSpanLong)]
		public TimeSpan DoNotSendWithin;
		///<summary>Enables/Disables the ApptReminderRule.</summary>
		public bool IsEnabled;
		///<summary>Used when auto replying single eConfirmations.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateAutoReply;
		///<summary>Used when auto replying multiple patient eConfirmations.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateAutoReplyAgg;
		///<summary>Used when auto replying to appointment confirmations that failed.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateFailureAutoReply;
		///<summary>Enables/Disables eConfirmation auto replies. Only for when the patient responds positively via text.</summary>
		public bool IsAutoReplyEnabled;
		///<summary>When set, matched by text against the patient's language. Typically eng (English), fra (French), spa (Spanish), or similar.  
		/// If it's a custom language, then it might look like Tahitian. 
		/// Empty string implies that this rule uses the default language of the practice.</summary>
		public string Language;
		///<summary>Enum. The Type of email for the template.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.EnumAsString)]
		public EmailType EmailTemplateType;
		///<summary>Enum:EmailType The type of email for the aggregated template. </summary>
		[CrudColumn(SpecialType = CrudSpecialColType.EnumAsString)]
		public EmailType AggEmailTemplateType;
		///<summary>Used when inviting patient to come into office.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string TemplateComeInMessage;
		///<summary>Boolean false by default. Controls if birthday messages will get sent to a minor for their birthday.</summary>
		public bool IsSendForMinorsBirthday;
		///<summary>FK to emailhostingtemplate.EmailHostingTemplateNum. If used, rules fields will be based from the template.</summary>
		public long EmailHostingTemplateNum;
		///<summary>When IsSendForMinorsBirthday is true, this is the age that defines what a minor is.</summary>
		public int MinorAge;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("TSPrior",typeof(long))]
		public long TSPriorXml {
			get {
				return TSPrior.Ticks;
			}
			set {
				TSPrior=TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("DoNotSendWithin",typeof(long))]
		public long DoNotSendWithinXml {
			get {
				return DoNotSendWithin.Ticks;
			}
			set {
				DoNotSendWithin=TimeSpan.FromTicks(value);
			}
		}

		public ApptReminderRule() {
			SendOrder="0,1,2";//default send order
			//Singleton templates
			TemplateSMS="";//default message set from FormApptReminderRuleEdit
			TemplateEmail="";//default message set from FormApptReminderRuleEdit
			TemplateEmailSubject="";
			//Aggregate Templates
			TemplateSMSAggShared="";
			TemplateSMSAggPerAppt="";
			TemplateEmailSubjAggShared="";
			TemplateEmailAggShared="";
			TemplateEmailAggPerAppt="";
			IsEnabled=true;//Maybe make this a case by case set.  Serializing/Deserializing is my concern.
			TemplateAutoReply="";
			TemplateAutoReplyAgg="";
			IsAutoReplyEnabled=false;
			Language="";
		}

		public bool IsValidDuration {
			get {
				if(TypeCur==ApptReminderType.Birthday) {
					return true;//Birthdays use clinic prefs to determine if enabled, they will be filtered later.
				}
				if(!IsEnabled) {
					return false;
				}
				if(TypeCur==ApptReminderType.ConfirmationFutureDay) {
					return TSPrior.Days>=1;
				}
				return true;
			}
		}
		public bool IsSameDay {
			get {
				return IsValidDuration && TSPrior.Days==0;
			}
		}

		public bool IsFutureDay {
			get {
				return IsValidDuration && TSPrior.TotalDays>=1;
			}
		}

		public bool IsPastDay {
			get {
				return IsValidDuration && TSPrior.TotalDays<=-1;
			}
		}

		///<summary>Only valid for IsFutureDay rule.</summary>
		public int NumDaysInFuture {
			get {
				if(!IsFutureDay) {
					return 0;
				}
				//Rounds 1.1 to 2. Anything with n days + x hours will be n+1 days.
				//This is necessary because we can have a case of 1 day and 23 hours (essentially 2 days), and don't want to skip sending for that rule.
				return (int)Math.Ceiling(TSPrior.TotalDays);
			}
		}

		///<summary>Only valid for IsSameDay rule.</summary>
		public int NumMinutesInFuture {
			get {
				if(!IsSameDay) {
					return 0;
				}
				return (int)TSPrior.TotalMinutes;
			}
		}

		///<summary>Only valid for IsPastDay rule.</summary>
		public int NumDaysInPast {
			get {
				if(!IsPastDay) {
					return 0;
				}
				//Rounds 1.1 to 2. Anything with n days + x hours will be n+1 days.
				//This is necessary because we can have a case of 1 day and 23 hours (essentially 2 days), and don't want to skip sending for that rule.
				return (int)Math.Ceiling(Math.Abs(TSPrior.TotalDays));
			}
		}

		public ApptReminderRule Copy() {
			ApptReminderRule retVal = (ApptReminderRule)this.MemberwiseClone();
			return retVal;
		}

		public ApptReminderRule CopyWithClinicNum(long clinicNum) {
			ApptReminderRule retVal = Copy();
			retVal.ClinicNum=clinicNum;
			return retVal;
		}
	}

	public enum ApptReminderType {
		///<summary>-1 - Used to define an Undefined ApptReminderType.</summary>
		Undefined=-1,
		///<summary>0 - Used to define the rules for when reminders should be sent out.</summary>
		Reminder,
		///<summary>1 - Defines rules for when confirmations should be sent out.</summary>
		[Description("Confirmation")]
		ConfirmationFutureDay,
		///<summary>2 - DEPRECATED. As of 17.4, all reminders have a status of Reminder.</summary>
		[Description("Reminder (Future)")]
		ReminderFutureDay,
		///<summary>3 - Send emails to patients with their credentials to the Patient Portal.</summary>
		[Description("Patient Portal Invites")]
		PatientPortalInvite,
		///<summary>4 - Defines rules for when Schedule Verify ("Thank You"s) should be sent out.</summary>
		[Description("Automated Thank-You")]
		ScheduleThankYou,
		///<summary>5 - Defines rules for when Arrival instructions should be sent out.</summary>
		[Description("Arrivals")]
		Arrival,
		///<summary>6 - Birthday. Defines rule for sending out automated birthday emails.</summary>
		[Description("Birthday")]
		Birthday,
		///<summary>7 - General Message. Defines rules for sending out automated messages after an appointment is set complete.</summary>
		[Description("General Message")]
		GeneralMessage,
	}

	[Flags]
	public enum ShortCodeTypeFlag {
		None=0b0,
		//IMPORTANT: Every value defined in this enum must have a ShortCodeAttribute with SmsMessageSource and EServicePrefName properly defined.
		//PrefIsEnabledValues/EnabledValueType are optional/only required if EServicePrefNames are not bool values.
		[ShortCode(SmsMessageSource=new SmsMessageSource[] { SmsMessageSource.ConfirmationRequest,SmsMessageSource.ConfirmationAutoReply }
			,EServicePrefNames=new PrefName[] { PrefName.ApptConfirmAutoSignedUp,PrefName.ApptConfirmAutoEnabled })]
		Confirmations=0b1,
		[ShortCode(SmsMessageSource = new SmsMessageSource[] { SmsMessageSource.Recall,SmsMessageSource.RecallAuto }
			//WebSchedRecall is determined to be enabled or not via a web call.
			,EServicePrefNames=new PrefName[] { PrefName.WebSchedAutomaticSendTextSetting }
			,PrefIsEnabledValues=new int[] { (int)WebSchedAutomaticSend.SendToText }
			,EnabledValueType=typeof(WebSchedAutomaticSend))]
		WebSchedRecall = 0b10,
		[ShortCode(SmsMessageSource = new SmsMessageSource[] { SmsMessageSource.Reminder }
			,EServicePrefNames=new PrefName[] { PrefName.ApptRemindAutoEnabled })]
		Reminders = 0b100,
		[ShortCode(SmsMessageSource = new SmsMessageSource[] { SmsMessageSource.ApptThankYou }
			,EServicePrefNames=new PrefName[] { PrefName.ApptThankYouAutoEnabled })]
		ThankYous = 0b1000,
		[ShortCode(SmsMessageSource = new SmsMessageSource[] { SmsMessageSource.WebSchedASAP }
			,EServicePrefNames=new PrefName[] { PrefName.WebSchedAsapEnabled })]
		WebSchedASAP = 0b10000,
		[ShortCode(SmsMessageSource = new SmsMessageSource[] { SmsMessageSource.Verify }
			,EServicePrefNames = new PrefName[] { PrefName.WebSchedVerifyASAPType,PrefName.WebSchedVerifyNewPatType,PrefName.WebSchedVerifyRecallType }
			,PrefIsEnabledValues=new int[] { (int)WebSchedVerifyType.Text,(int)WebSchedVerifyType.TextAndEmail }
			,EnabledValueType=typeof(WebSchedVerifyType))]
		WebSchedVerify = 0b100000,
		[ShortCode(SmsMessageSource = new SmsMessageSource[] { SmsMessageSource.VerifyWSNP }
			,EServicePrefNames=new PrefName[] { PrefName.WebSchedNewPatDoAuthText })]
		WebSchedNewPat = 0b1000000,
		[ShortCode(SmsMessageSource = new SmsMessageSource[] { SmsMessageSource.Statements }
			//BillingDefaultsModesToText is a comma separated string, PIn.Bool() will return false if empty, true otherwise.
			,EServicePrefNames=new PrefName[] { PrefName.BillingDefaultsModesToText })]
		BillingStatements = 0b10000000,
		[ShortCode(SmsMessageSource=new SmsMessageSource[] { SmsMessageSource.Confirmation }
			//NotApplicable means only Texting is required for this eService 
			,EServicePrefNames=new PrefName[] { PrefName.NotApplicable })]
		ManualConfirmations=0b100000000,
		[ShortCode(SmsMessageSource=new SmsMessageSource[] { SmsMessageSource.AsapManual }
			//NotApplicable means only Texting is required for this eService 
			,EServicePrefNames=new PrefName[] { PrefName.NotApplicable })]
		ASAPManual=0b1000000000,		
		[ShortCode(SmsMessageSource=new SmsMessageSource[] { SmsMessageSource.Arrival }
			,EServicePrefNames=new PrefName[] { PrefName.ApptConfirmAutoSignedUp,PrefName.ApptConfirmAutoEnabled })]
		Arrivals=0b10000000000,
		[ShortCode(SmsMessageSource=new SmsMessageSource[] { SmsMessageSource.GeneralMessage }
			,EServicePrefNames=new PrefName[] { PrefName.ApptGeneralMessageAutoEnabled })]
		GeneralMessage=0b100000000000,
	}

	public class ShortCodeAttribute:Attribute {
		private SmsMessageSource[] _smsMessageSource=new SmsMessageSource[] { OpenDentBusiness.SmsMessageSource.Undefined };
		private PrefName[] _eServicePrefNames=new PrefName[] { PrefName.NotApplicable };
		private Type _valueType;
		private int[] _prefIsEnabledValues=new int[] { };

		public SmsMessageSource[] SmsMessageSource{
			get{
				return _smsMessageSource;
			}
			set{
				_smsMessageSource=value;
			}
		}

		///<summary>The preference to check to determine if the eService for this Short Code implementation is enabled.  Preference is assumed to return
		///a bool, unless PrefIsEnabledEnumAsIntValues and EnabledValueType are defined.</summary>
		public PrefName[] EServicePrefNames {
			get {
				return _eServicePrefNames;
			}
			set {
				_eServicePrefNames=value;
			}
		}
	
		///<summary>If defined, indicates the values that will be considered "enabled" for the preference defined by EServicePrefName.</summary>
		public int[] PrefIsEnabledValues {
			get {
				return _prefIsEnabledValues;
			}
			set {
				_prefIsEnabledValues=value;
			}
		}

		///<summary>The Type of the Enum used for PrefIsEnabledEnumAsIntValues.  Must be defined if PrefIsEnabledEnumAsIntValues is defined.</summary>
		public Type EnabledValueType {
			get {
				return _valueType;
			}
			set {
				_valueType=value;
			}
		}

		public bool IsServiceEnabled(long clinicNum) {
			if(!PrefC.HasClinicsEnabled) {
				clinicNum=0;
			}
			return Clinics.IsTextingEnabled(clinicNum) 
				&& EServicePrefNames.Any(x => IsPrefEnabled(x,clinicNum,PrefIsEnabledValues,EnabledValueType));
		}

		///<summary>If prefName is not configured, returns true.  Otherwise, checks if "enabled" values have been defined.  If so, gets the 
		///value of the ClinicPref/Pref corresponding to prefName as an int and checks if this value is "enabled".  If "enabled" values are not
		///defined, considers </summary>
		private static bool IsPrefEnabled(PrefName prefName,long clinicNum,int[] intArrayEnabledValues,Type typeEnabledValue) {
			if(prefName==PrefName.NotApplicable) {
				return true;
			}
			if(typeEnabledValue is null || intArrayEnabledValues.IsNullOrEmpty()) {
				//No override type/values defined, so we can consider the preference to be a boolean.
				return ClinicPrefs.GetBool(prefName,clinicNum);
			}
			Array arrayObjectsEnumValues=Enum.GetValues(typeEnabledValue);
			string stringPrefValue=ClinicPrefs.GetPrefValue(prefName,clinicNum);
			int? nullableIntPref=null;
			for(int i = 0;i<arrayObjectsEnumValues.Length;i++) {
				object objectEnumValue=arrayObjectsEnumValues.GetValue(i);
				if(stringPrefValue==((int)objectEnumValue).ToString() || stringPrefValue==objectEnumValue.ToString()) {
					nullableIntPref=(int)objectEnumValue;
					break;
				}
			}
			if(nullableIntPref is null) {
				return false;
			}
			return ListTools.In(nullableIntPref.Value,intArrayEnabledValues);
		}

		///<summary>Gets the first Enum T with a ShortCodeAttribute such that SmsMessageSouce matches the given value.</summary>
		public static T GetFirstOrDefault<T>(SmsMessageSource smsMessageSource) where T:Enum {
			return Enum.GetValues(typeof(T)).AsEnumerable<T>()
				.FirstOrDefault(x => EnumTools.GetAttributeOrDefault<ShortCodeAttribute>(x).SmsMessageSource.Contains(smsMessageSource));
		}
	}

	///<summary></summary>
	public enum CommType {
		///<summary>0 - Use text OR email based on patient preference.</summary>
		Preferred = 0,
		///<summary>1 - Attempt to send text message, if successful do not send via email. (Unless, a SendAll bool is used, which usually negates the need for this enumeration.)</summary>
		Text = 1,
		///<summary>2 - Attempt to send email message, if successful do not send via text. (Unless, a SendAll bool is used, which usually negates the need for this enumeration.)</summary>
		Email = 2
	}

	///<summary></summary>
	public enum IntervalType {
		///<summary></summary>
		Daily,
		///<summary></summary>
		Hourly

	}
}
