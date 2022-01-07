using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class AsapCommT {

		///<summary></summary>
		/// <param name="funcCreateShortGUID">If not null, then this function will be called to set AsapComm.ShortGUID. 
		/// This simulates having already asked HQ for the short guid and allows this step to be skipped for the purpose of unit test.</param>
		/// <param name="dateTimeSmsScheduled">Allows due date of SMS send to be set manually. This is used to simulate an SMS that will be sent later. Allows email to be sent first.</param>
		public static AsapComm CreateAsapComm(Appointment appt,Schedule sched,Func<string> funcCreateShortGUID = null,
			DateTime dateTimeSmsScheduled = default(DateTime)) 
		{
			AsapComm asapComm=new AsapComm {
				ClinicNum=appt.ClinicNum,
				DateTimeExpire=sched.SchedDate.Date.Add(sched.StartTime),
				DateTimeOrig=appt.AptDateTime,
				DateTimeSmsScheduled=(dateTimeSmsScheduled==default(DateTime) ? DateTime.Now : dateTimeSmsScheduled),
				EmailSendStatus=AutoCommStatus.SendNotAttempted,
				FKey=appt.AptNum,
				Note="",
				PatNum=appt.PatNum,
				ResponseStatus=AsapRSVPStatus.AwaitingTransmit,
				ScheduleNum=sched.ScheduleNum,
				ShortGUID=(funcCreateShortGUID!=null ? funcCreateShortGUID() : ""),
				SmsSendStatus=AutoCommStatus.SendNotAttempted,
				TemplateEmail=PrefC.GetString(PrefName.WebSchedAsapEmailTemplate),
				TemplateEmailSubj=PrefC.GetString(PrefName.WebSchedAsapEmailSubj),
				TemplateText=PrefC.GetString(PrefName.WebSchedAsapTextTemplate)
			};
			if(appt.AptStatus==ApptStatus.UnschedList) {
				asapComm.FKeyType=AsapCommFKeyType.UnscheduledAppt;
			}
			else if(appt.AptStatus==ApptStatus.Planned) {
				asapComm.FKeyType=AsapCommFKeyType.PlannedAppt;
			}
			else if(appt.AptStatus==ApptStatus.Broken) {
				asapComm.FKeyType=AsapCommFKeyType.Broken;
			}
			else {
				asapComm.FKeyType=AsapCommFKeyType.ScheduledAppt;
			}
			AsapComms.Insert(asapComm);
			return asapComm;
		}

		///<summary></summary>
		/// <param name="funcCreateShortGUID">If not null, then this function will be called to set AsapComm.ShortGUID. 
		/// This simulates having already asked HQ for the short guid and allows this step to be skipped for the purpose of unit test.</param>
		/// <param name="dateTimeSmsScheduled">Allows due date of SMS send to be set manually. This is used to simulate an SMS that will be sent later. Allows email to be sent first.</param>
		public static AsapComm CreateAsapCommRecall(Recall recall,Schedule sched,long clinicNum,Func<string> funcCreateShortGUID=null,
			DateTime dateTimeSmsScheduled = default(DateTime)) 
		{
			AsapComm asapComm=new AsapComm {
				ClinicNum=clinicNum,
				DateTimeExpire=sched.SchedDate.Date.Add(sched.StartTime),
				DateTimeOrig=recall.DateDue,
				DateTimeSmsScheduled=(dateTimeSmsScheduled==default(DateTime) ? DateTime.Now : dateTimeSmsScheduled),
				EmailSendStatus=AutoCommStatus.SendNotAttempted,
				FKey=recall.RecallNum,
				FKeyType=AsapCommFKeyType.Recall,
				Note="",
				PatNum=recall.PatNum,
				ResponseStatus=AsapRSVPStatus.AwaitingTransmit,
				ScheduleNum=sched.ScheduleNum,
				ShortGUID=(funcCreateShortGUID!=null ? funcCreateShortGUID() : ""),
				SmsSendStatus=AutoCommStatus.SendNotAttempted,
				TemplateEmail=PrefC.GetString(PrefName.WebSchedAsapEmailTemplate),
				TemplateEmailSubj=PrefC.GetString(PrefName.WebSchedAsapEmailSubj),
				TemplateText=PrefC.GetString(PrefName.WebSchedAsapTextTemplate)
			};
			AsapComms.Insert(asapComm);
			return asapComm;
		}
	}
}
