using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness.AutoComm {
	///<summary>This class contains an aggregate of fields that are useful in Web Sched AutoComms.</summary>
	public class WebSchedAgg:AutoCommObj {
		//PrimaryKey will be set to WebSchedRecall.RecallNum
		[XmlIgnore]
		public int NumReminders;
		[XmlIgnore]
		public long WebSchedRecallNum;
#region Deprecated as of 22.2. Must remain defined for back-compatibility deserialization and handling at HQ for versions older than 22.2.
		///<summary>Deprecated.</summary>
		public string EmailSubjTemplate="";
		///<summary>Deprecated.</summary>
		public string EmailTextTemplate="";
		///<summary>Deprecated.</summary>
		public string MsgTextToMobile="";
		///<summary>Deprecated.</summary>
		public string MsgTextToMobileTemplate="";
		///<summary>Deprecated.</summary>
		public string EmailSubj="";
		///<summary>Deprecated.</summary>
		public string EmailText="";
		///<summary>Deprecated.The recipient SMS phone number. If non-blank then assume this number can be texted.</summary>
		public string PhoneContact;
		///<summary>Deprecated.The recipient email. If non-blank then assume this email can be sent.</summary>
		public string EmailContact;
		///<summary>Deprecated.Indicates that an SMS should be attempted for this patient.</summary>
		public bool TrySendSMS;
		///<summary>Deprecated.Indicates that an email should be attempted for this patient.</summary>
		public bool TrySendEmail;
		///<summary>Deprecated.he send status of the SMS.</summary>
		public AutoCommStatus SMSSendStatus=AutoCommStatus.Undefined;
		///<summary>Deprecated.The send status of the email.</summary>
		public AutoCommStatus EmailSendStatus=AutoCommStatus.Undefined;
		///<summary>Deprecated.</summary>
		public RSVPStatusCodes RSVPStatus;
#endregion
		public string GuidMessageToMobile="";
		public string ResponseDescript="";
		public WebSchedRecallSource Source=WebSchedRecallSource.Undefined;
		public bool IsHtmlEmail=true;
		[XmlIgnore]
		public EmailType EmailTemplateType;
		///<summary>The time the row was entered into the webschedrecall table.</summary>
		[XmlIgnore]
		public DateTime DateTimeEntry;
		[XmlIgnore]
		public long CommlogNum;
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

		public override long AptNum => 0;

		///<summary>Deprecated.  Used to store data for aggregated recall messages.  This field is still around because legacy web code
		///has to reference it for older versions of OD Desktop.  This data is now tracked in WebSchedRecall.ListWebSchedRecallFams.</summary>
		public List<WebSchedRecallFam> ListWebSchedRecallFams=new List<WebSchedRecallFam>();

		public override void InsertCommlog(CommItemTypeAuto commTypeAuto,CommItemMode mode,CommItemSource source,string message) {			
			long defNumNewStatus=mode switch {
				CommItemMode.Text => PrefC.GetLong(PrefName.RecallStatusTexted),
				CommItemMode.Email => PrefC.GetLong(PrefName.RecallStatusEmailed),
				_ => 0
			};
			//Update the Recall row to indicate which type of message was sent. (may be merging multiple message types into a combo type because a different
			//AutoComm thread may have already sent for a different message type than this thread.
			Recalls.UpdateStatus(PrimaryKey,defNumNewStatus);//WebSchedAgg.PrimaryKey=RecallNum
			//Insert a Recall Commlog, which will then be used to eliminate this patient from the RecallList.
			Commlog commlog=Commlogs.InsertForRecallOrReactivation(PatNum,mode,NumReminders,defNumNewStatus,source,Security.CurUser?.UserNum??0,DateTime_.Now
				,commTypeAuto);
			CommlogNum=commlog.CommlogNum;
		}

		///<summary>One recall notification for a family member (or a person from a different family that has the same phone or email as the primary
		///WebSchedRecall).</summary>
		public class WebSchedRecallFam {
			public string NameF;
			public long RecallNum;
			public string ShortGUID;
			public string ShortUrl;
			public long PatNum;
			public DateTime DateDue;
			public int NumReminders;
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
			DateTimeEvent=wsRecall.DateDue.Date.AddHours(dtSend.Hour);
			IsDtSendFinal=true;
			ClinicNum=wsRecall.ClinicNum;
			NumReminders=wsRecall.ReminderCount;
			WebSchedRecallNum=wsRecall.WebSchedRecallNum;
			//All other versions of AutoCommObj start at Undefined, so be consistent (even though WebSchedRecall.SendStatus in the db is SendNotAttempted).
			SendStatus=wsRecall.SendStatus==AutoCommStatus.SendNotAttempted ? AutoCommStatus.Undefined : wsRecall.SendStatus;
			Source=wsRecall.Source;
			DateTimeEntry=wsRecall.DateTimeEntry;
		}

		///<summary>Converts the WebSchedAgg to a WebSchedRecall.</summary>
		public WebSchedRecall ToWebSchedRecall() {
			WebSchedRecall wsRecall=new WebSchedRecall {
				WebSchedRecallNum=WebSchedRecallNum,
				ClinicNum=ClinicNum,
				PatNum=PatNum,
				RecallNum=PrimaryKey,
				DateDue=DateTimeEvent,
				ReminderCount=NumReminders,
				GuidMessageToMobile=GuidMessageToMobile,
				ResponseDescript=ResponseDescript,
				Source=Source,
				CommlogNum=CommlogNum,
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
					GuidMessageToMobile=GuidMessageToMobile,
					ResponseDescript=ResponseDescript,
					Source=Source,
					CommlogNum=CommlogNum,
				};
				listWebSchedRecalls.Add(wsRecall);
			}
			return listWebSchedRecalls;
		}
	}
}
