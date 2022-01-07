using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace OpenDentBusiness.AutoComm {
	///<summary>This class contains an aggregate of fields that are useful in Web Sched AutoComms.</summary>
	public class WebSchedAgg:AutoCommObj {
		//PrimaryKey will be set to WebSchedRecall.RecallNum
		[XmlIgnore]
		public int NumReminders;
		[XmlIgnore]
		public ContactMethod PreferRecallMethod;
		[XmlIgnore]
		public long WebSchedRecallNum;
		public string EmailSubjTemplate="";
		public string EmailTextTemplate="";
		public string MsgTextToMobile="";
		public string MsgTextToMobileTemplate="";
		public string EmailSubj="";
		public string EmailText="";
		public RSVPStatusCodes RSVPStatus;
		public string GuidMessageToMobile="";
		public string ResponseDescript="";
		public WebSchedRecallSource Source=WebSchedRecallSource.Undefined;
		public bool IsHtmlEmail=true;
		public EmailType EmailTemplateType;
		///<summary>The time the row was entered into the webschedrecall table.</summary>
		[XmlIgnore]
		public DateTime DateTimeEntry;
		///<summary>If true, this WebSchedRecall represents a communication item sent for multiple recalls.</summary>
		public bool IsMultiple {
			get {
				return ListWebSchedRecallFams.Count > 1;
			}
		}

		///<summary>True if any template has a URL tag in it.</summary>
		[XmlIgnore]
		public bool HasURLTags {
			get {
				return WebSchedRecalls.HasURLTag(MsgTextToMobileTemplate) || WebSchedRecalls.HasURLTag(EmailTextTemplate)
					|| WebSchedRecalls.HasURLTag(EmailSubjTemplate);
			}
		}

		///<summary>Stores the information for the WebSchedRecalls that </summary>
		public List<WebSchedRecallFam> ListWebSchedRecallFams=new List<WebSchedRecallFam>();

		///<summary>One recall notification for a family member (or a person from a different family that has the same phone or email as the primary
		///WebSchedRecall).</summary>
		public class WebSchedRecallFam {
			public string NameF;
			public long RecallNum;
			public string ShortGUID;
			[XmlIgnore]
			public string ShortUrl;
			[XmlIgnore]
			public long PatNum;
			[XmlIgnore]
			public DateTime DateDue;
			[XmlIgnore]
			public int NumReminders;
			[XmlIgnore]
			public long WebSchedRecallNum;

			public WebSchedRecallFam() { } //For serialization

			public WebSchedRecallFam(string nameF,long recallNum,long patNum,int numReminders,long webSchedRecallNum,DateTime dateDue) {
				NameF=nameF;
				RecallNum=recallNum;
				PatNum=patNum;
				NumReminders=numReminders;
				WebSchedRecallNum=webSchedRecallNum;
				DateDue=dateDue;
			}

			public WebSchedRecallFam Copy() {
				return (WebSchedRecallFam)MemberwiseClone();
			}
		}

		public WebSchedAgg() { } //For serialization

		public WebSchedAgg(WebSchedRecall wsRecall,DateTime dtSend) {
			PrimaryKey=wsRecall.RecallNum;
			PatNum=wsRecall.PatNum;
			DateTimeEvent=wsRecall.DateDue;
			DtSend=dtSend;
			IsDtSendFinal=true;
			ClinicNum=wsRecall.ClinicNum;
			NumReminders=wsRecall.ReminderCount;
			PreferRecallMethod=wsRecall.PreferRecallMethod;
			WebSchedRecallNum=wsRecall.WebSchedRecallNum;
			EmailSendStatus=wsRecall.EmailSendStatus;
			SMSSendStatus=wsRecall.SmsSendStatus;
			Source=wsRecall.Source;
			DateTimeEntry=wsRecall.DateTimeEntry;
		}

		public new WebSchedAgg Copy() {
			WebSchedAgg wsAgg=(WebSchedAgg)MemberwiseClone();
			wsAgg.ListWebSchedRecallFams=ListWebSchedRecallFams.Select(x => x.Copy()).ToList();
			return wsAgg;
		}

		///<summary>Converts the WebSchedAgg to a WebSchedRecall.</summary>
		private WebSchedRecall ToWebSchedRecall() {
			WebSchedRecall wsRecall=new WebSchedRecall {
				WebSchedRecallNum=WebSchedRecallNum,
				ClinicNum=ClinicNum,
				PatNum=PatNum,
				RecallNum=PrimaryKey,
				DateDue=DateTimeEvent,
				ReminderCount=NumReminders,
				PreferRecallMethod=PreferRecallMethod,
				PhonePat=PhoneContact,
				EmailPat=EmailContact,
				MsgTextToMobileTemplate=MsgTextToMobileTemplate,
				MsgTextToMobile=MsgTextToMobile,
				EmailSubjTemplate=EmailSubjTemplate,
				EmailSubj=EmailSubj,
				EmailTextTemplate=EmailTextTemplate,
				EmailText=EmailText,
				GuidMessageToMobile=GuidMessageToMobile,
				ShortGUIDSms="",
				ShortGUIDEmail="",
				ResponseDescript=ResponseDescript,
				EmailSendStatus=EmailSendStatus,
				SmsSendStatus=SMSSendStatus,
				IsForEmail=TrySendEmail,
				IsForSms=TrySendSMS,
				Source=Source,
			};
			return wsRecall;
		}

		///<summary>Converts ListWebSchedRecallFams to a list of WebSchedRecalls. Always returns at least one.</summary>
		public List<WebSchedRecall> ToListWebSchedRecalls() {
			if(ListWebSchedRecallFams==null || ListWebSchedRecallFams.Count==0) {
				return new List<WebSchedRecall> { ToWebSchedRecall() };
			}
			List<WebSchedRecall> listWebSchedRecalls=new List<WebSchedRecall>();
			foreach(WebSchedRecallFam wsRecallFam in ListWebSchedRecallFams) {
				WebSchedRecall wsRecall=new WebSchedRecall {
					WebSchedRecallNum=wsRecallFam.WebSchedRecallNum,
					ClinicNum=ClinicNum,
					PatNum=wsRecallFam.PatNum,
					RecallNum=wsRecallFam.RecallNum,
					DateDue=wsRecallFam.DateDue,
					ReminderCount=wsRecallFam.NumReminders,
					PreferRecallMethod=PreferRecallMethod,
					PhonePat=PhoneContact,
					EmailPat=EmailContact,
					MsgTextToMobileTemplate=MsgTextToMobileTemplate,
					MsgTextToMobile=MsgTextToMobile,
					EmailSubjTemplate=EmailSubjTemplate,
					EmailSubj=EmailSubj,
					EmailTextTemplate=EmailTextTemplate,
					EmailText=EmailText,
					GuidMessageToMobile=GuidMessageToMobile,
					ShortGUIDSms=TrySendSMS ? wsRecallFam.ShortGUID : "",
					ShortGUIDEmail=TrySendEmail ? wsRecallFam.ShortGUID : "",
					ResponseDescript=ResponseDescript,
					IsForEmail=TrySendEmail,
					IsForSms=TrySendSMS,
					EmailSendStatus=EmailSendStatus,
					SmsSendStatus=SMSSendStatus,
					Source=Source,
				};
				listWebSchedRecalls.Add(wsRecall);
			}
			return listWebSchedRecalls;
		}

		public override void SetPatientContact(PatComm patComm,Dictionary<long,PatComm> dictPatComms) {
			base.SetPatientContact(patComm,dictPatComms);
			//If patComm.Patnum doesn't match the ACO patnum then our patComm should be the guarantor for the ACO.
			//Leave the contact info as the guarantor but change the name back to the patient.
			if(patComm.PatNum!=this.PatNum) {
				if(dictPatComms.TryGetValue(this.PatNum,out PatComm patientPatComm)) {
					NameF=patientPatComm.GetFirstOrPreferred();
				}
			}
		}
	}
}
