using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness{
	///<summary></summary>
	public class WebSchedRecalls{
		#region Get Methods

		///<summary>Gets the WebSchedRecall row for the passed in recallNum.  Will return null if the recallNum isn't found.</summary>
		public static WebSchedRecall GetLastForRecall(long recallNum) {
			return GetLastForRecalls(ListTools.FromSingle(recallNum)).FirstOrDefault();
		}

		///<summary>Gets the WebSchedRecall row for the passed in recallNum.  Will return null if the recallNum isn't found.</summary>
		public static List<WebSchedRecall> GetLastForRecalls(List<long> listRecallNums,bool isGroupedForMostRecent=false) {
			if(listRecallNums.IsNullOrEmpty()) {
				return new List<WebSchedRecall>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),listRecallNums,isGroupedForMostRecent);
			}
			string command="SELECT * FROM webschedrecall WHERE RecallNum IN ("+string.Join(",",listRecallNums.Distinct().Select(x => POut.Long(x)))+") ";
			if(isGroupedForMostRecent) {
				command+="GROUP BY RecallNum ";
			}
			command+="ORDER BY DateTimeEntry DESC";
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}

		#endregion

		///<summary>Gets all WebSchedRecalls for which a reminder has not been sent. To get for all clinics, don't include the listClinicNums or pass
		///in an empty list.</summary>
		public static List<WebSchedRecall> GetAllUnsent(List<long> listClinicNums=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			//We don't want to include rows that have a status of SendFailed or SendSuccess
			string command="SELECT * FROM webschedrecall WHERE DateTimeReminderSent < '1880-01-01' "
				+"AND EmailSendStatus IN("+POut.Int((int)AutoCommStatus.DoNotSend)+", "+POut.Int((int)AutoCommStatus.SendNotAttempted)+") "
				+"AND SmsSendStatus IN("+POut.Int((int)AutoCommStatus.DoNotSend)+", "+POut.Int((int)AutoCommStatus.SendNotAttempted)+") "
				+"AND (EmailSendStatus="+POut.Int((int)AutoCommStatus.SendNotAttempted)+" "
				+"OR SmsSendStatus="+POut.Int((int)AutoCommStatus.SendNotAttempted)+") ";
			if(listClinicNums!=null && listClinicNums.Count > 0) {
				command+="AND ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(WebSchedRecall webSchedRecall) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				webSchedRecall.WebSchedRecallNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webSchedRecall);
				return webSchedRecall.WebSchedRecallNum;
			}
			return Crud.WebSchedRecallCrud.Insert(webSchedRecall);
		}

		///<summary></summary>
		public static void Update(WebSchedRecall webSchedRecall) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webSchedRecall);
				return;
			}
			Crud.WebSchedRecallCrud.Update(webSchedRecall);
		}

		///<summary></summary>
		public static void Delete(long webSchedRecallNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webSchedRecallNum);
				return;
			}
			Crud.WebSchedRecallCrud.Delete(webSchedRecallNum);
		}

		public static List<WebSchedRecall> GetByGuidMessageToMobile(string guidMessageToMobile) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),guidMessageToMobile);
			}
			string command=$"SELECT * FROM webschedrecall WHERE GuidMessageToMobile='{POut.String(guidMessageToMobile)}'";
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}

		///<summary>Updates DateTimeReminderSent and makes EmailSendStatus to SendSuccessful if IsForEmail is true and SmsSendStatus if IsForSms is
		///true.</summary>
		public static void MarkReminderSent(WebSchedRecall webSchedRecall,DateTime dateTimeReminderSent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webSchedRecall,dateTimeReminderSent);
				return;
			}
			webSchedRecall.DateTimeReminderSent=dateTimeReminderSent;
			if(webSchedRecall.IsForEmail) {
				webSchedRecall.EmailSendStatus=AutoCommStatus.SendSuccessful;
			}
			else if(webSchedRecall.IsForSms) {		
				WebSchedRecall recall=Crud.WebSchedRecallCrud.SelectOne(webSchedRecall.WebSchedRecallNum);
				if(recall!=null && ListTools.In(recall.SmsSendStatus,AutoCommStatus.SendFailed,AutoCommStatus.SendSuccessful)) {
					//In case the sms delivery receipt arrives before AutoComm has set SmsSendStatus to SentAwaitingReceipt.
					webSchedRecall.SmsSendStatus=recall.SmsSendStatus;
				}
				else {
					//Will be marked as SendSuccessful upon delivery receipt.		
					webSchedRecall.SmsSendStatus=AutoCommStatus.SentAwaitingReceipt;
				}
			}
			WebSchedRecalls.Update(webSchedRecall);
		}

		///<summary>Sets EmailSendStatus to SendFailed if isForEmail is true. Sets SmsSendStatus to SendFailed if IsForSms is true.</summary>
		public static void MarkSendFailed(WebSchedRecall webSchedRecall,DateTime dateSendFailed) {
			//No need to check RemotingRole; no call to db.
			webSchedRecall.DateTimeSendFailed=dateSendFailed;
			if(webSchedRecall.IsForEmail) {
				webSchedRecall.EmailSendStatus=AutoCommStatus.SendFailed;
			}
			else if(webSchedRecall.IsForSms) {
				webSchedRecall.SmsSendStatus=AutoCommStatus.SendFailed;
			}
			WebSchedRecalls.Update(webSchedRecall);
		}

		///<summary>Returns true if any Web Sched Recall text or email template contains a URL tag.</summary>
		public static bool TemplatesHaveURLTags() {
			foreach(PrefName pref in new List<PrefName> {
				PrefName.WebSchedSubject,
				PrefName.WebSchedMessage,
				PrefName.WebSchedMessageText,
				PrefName.WebSchedAggregatedEmailBody,
				PrefName.WebSchedAggregatedEmailSubject,
				PrefName.WebSchedAggregatedTextMessage,
				PrefName.WebSchedSubject2,
				PrefName.WebSchedMessage2,
				PrefName.WebSchedMessageText2,
				PrefName.WebSchedSubject3,
				PrefName.WebSchedMessage3,
				PrefName.WebSchedMessageText3,
			}) 
			{
				if(HasURLTag(PrefC.GetString(pref))) {
					return true;
				}
			}
			return false;
		}

		///<summary>Returns true if the passed in template contains a URL tag.</summary>
		public static bool HasURLTag(string template) {
			return Regex.IsMatch(template,@"\[URL]|\[FamilyListURLs]",RegexOptions.IgnoreCase);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<WebSchedRecall> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM webschedrecall WHERE PatNum = "+POut.Long(patNum);
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}
		
		
		*/
	}
}