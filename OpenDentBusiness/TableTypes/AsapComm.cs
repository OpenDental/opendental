using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>Used to communicate to patients that are on the ASAP list.</summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class AsapComm:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AsapCommNum;
		///<summary>FK to the object for which this communication was made. Usually AptNum or RecallNum.</summary>
		public long FKey;
		///<summary>Enum:AsapCommFKeyType The type of object for which this communication was made.</summary>
		public AsapCommFKeyType FKeyType;
		///<summary>FK to schedule.ScheduleNum. The block on the schedule for which this communication was made.</summary>
		public long ScheduleNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to clinic.ClinicNum. The clinic that is sending this AsapComm.</summary>
		public long ClinicNum;
		///<summary>An identifier that is used to communicate with OD HQ regarding this communication item.</summary>
		public string ShortGUID;
		///<summary>When this communication item was entered into the database.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>When this communication item will expire.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeExpire;
		///<summary>The date and time when a text message is scheduled to be sent.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSmsScheduled; 
		///<summary>Enum:AutoCommStatus The status of sending the text for this communication.</summary>
		public AutoCommStatus SmsSendStatus;
		///<summary>Enum:AutoCommStatus The status of sending the email for this communication.</summary>
		public AutoCommStatus EmailSendStatus;
		///<summary>The date and time a text message was sent.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSmsSent;
		///<summary>The date and time an email was sent.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEmailSent;
		///<summary>FK to emailmessage.EmailMessageNum. The email message that was sent to the patient.</summary>
		public long EmailMessageNum;
		///<summary>Enum:AsapRSVPStatus How the patient has responded to this communication.</summary>
		public AsapRSVPStatus ResponseStatus;
		///<summary>The date and time of the appointment when this communication was made or the date and time of the recall date due.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeOrig;
		///<summary>The template that will be used when sending a text message.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string TemplateText;
		///<summary>The template that will be used when creating the body of the email message.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string TemplateEmail;
		///<summary>The template that will be used for the email subject line.</summary>
		public string TemplateEmailSubj;
		///<summary>Any notes regarding this communication item.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>FK to smstomobile.GuidMessage. Generated at HQ when the SMS is generated.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string GuidMessageToMobile;
		///<summary>Enum:EmailType Type of markup for the template.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.EnumAsString)]
		public EmailType EmailTemplateType;


		///<summary></summary>
		public AsapComm Copy() {
			return (AsapComm)this.MemberwiseClone();
		}
	}

	public enum AsapRSVPStatus {
		///<summary>0 - Neither text nor email was permitted to be sent.</summary>
		[Description("Unable to send")]
		UnableToSend,
		///<summary>1 - EConnector will pickup and send to HQ and change to pendingRsvp.</summary>
		[Description("Sending")]
		AwaitingTransmit,
		///<summary>2 - EConnector has sent this to HQ and will remain in this status until it is either terminated or receives a response from the 
		///patient.</summary>
		[Description("Sent")]
		PendingRsvp,
		///<summary>3 - The patient viewed the portal and took no action.</summary>
		[Description("Viewed")]
		Viewed,
		///<summary>4 - The patient viewed the portal but the slot was no longer available.</summary>
		[Description("Viewed but taken")]
		ViewedNotAvailable,
		///<summary>5 - The patient accepted the appointment and the appointment was successfully moved.</summary>
		[Description("Appointment made")]
		AcceptedAndMoved,
		///<summary>6 - The patient accepted the appointment but the appointment was not successfully moved.</summary>
		[Description("Accepted but taken")]
		AcceptedAndNotAvailable,
		///<summary>7 - The patient declined any open slots.</summary>
		[Description("Declined time slot")]
		Declined,
		///<summary>8 - The patient declined this slot but chose a different time slot.</summary>
		[Description("Chose different time")]
		ChoseDifferentSlot,
		///<summary>9 - Patient took no action by the time DateTimeExpired passed and the message was terminated.</summary>
		[Description("Expired")]
		Expired,
		///<summary>10 - HQ or EConnector was unable to send the message so it was terminated prematurely.</summary>
		[Description("Failed")]
		Failed,
		///<summary>11 - The patient declined and requested that we do not continue contacting them for this appointment.</summary>
		[Description("Declined, checked \"Do not contact\"")]
		DeclinedStopComm,
	}

	public enum AsapCommFKeyType {
		///<summary>0 - Should not be present in database.</summary>
		None,
		///<summary>1 - A scheduled appointment marked ASAP.</summary>
		ScheduledAppt,
		///<summary>2 - An unscheduled appointment marked ASAP.</summary>
		UnscheduledAppt,
		///<summary>3 - A planned appointment marked ASAP.</summary>
		PlannedAppt,
		///<summary>4 - A recall marked ASAP</summary>
		Recall,
		///<summary>5 - A broken appointment marked ASAP</summary>
		Broken,
	}


}