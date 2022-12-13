using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness.AutoComm;
using OpenDentBusiness.WebTypes.AutoComm;

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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),listRecallNums,isGroupedForMostRecent);
			}
			string command="SELECT * FROM webschedrecall WHERE RecallNum IN ("+string.Join(",",listRecallNums.Distinct().Select(x => POut.Long(x)))+") ";
			if(isGroupedForMostRecent) {
				command+="GROUP BY RecallNum ";
			}
			command+="ORDER BY DateTimeEntry DESC";
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}

		///<summary>Returns a webschedrecall for the passed in recallNum. Prioritizes email with http in the body. </summary>
		public static WebSchedRecall GetRecallsForPatientPortalWithMessage(long recallNum) {
			List<WebSchedRecall> listWebSchedRecalls=GetLastForRecalls(ListTools.FromSingle(recallNum));
			AutoCommSents.SetMessageBody(listWebSchedRecalls);
			WebSchedRecall wsRecall=listWebSchedRecalls.FirstOrDefault(x=>x.MessageType==CommType.Email && x.Message.Contains("http"));
			if(wsRecall==null) {
				wsRecall=listWebSchedRecalls.FirstOrDefault(x=>x.Message.Contains("http"));
			}
			//Ideally we want the email with http, if not we'll settle for a text with http, otherwise we'll just take what we can get
			return wsRecall!=null ? wsRecall : listWebSchedRecalls.FirstOrDefault();
		}
		#endregion

		///<summary>Gets all WebSchedRecalls for which a reminder has not been sent. To get for all clinics, don't include the listClinicNums or pass
		///in an empty list.</summary>
		public static List<WebSchedRecall> GetAllUnsent(List<CommType> listMessageTypes=null,List<long> listClinicNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),listMessageTypes,listClinicNums);
			}
			//Default to All if not specified.
			listMessageTypes??=Enum.GetValues(typeof(CommType)).Cast<CommType>().ToList().FindAll(x => x!=CommType.Invalid);
			//We don't want to include rows that have a status of SendFailed or SendSuccess
			string command="SELECT * FROM webschedrecall WHERE DateTimeSent < '1880-01-01' "
				+$"AND MessageType IN ({string.Join(",",listMessageTypes.Select(x => POut.Int((int)x)))}) "
				+$"AND SendStatus={POut.Int((int)AutoCommStatus.SendNotAttempted)} ";
			if(listClinicNums!=null && listClinicNums.Count > 0) {
				command+="AND ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}

		///<summary>Insert WebSchedRecalls so that the Auto Comm Web Sched thread can aggregate the recalls and send emails and texts.
		///Returns a list of errors to display to the user if anything went wrong otherwise returns empty list if everything was successful.</summary>
		public static List<string> InsertForRecallNums(List<long> listRecallNums,bool isGroupFamily,RecallListSort sortBy,WebSchedRecallSource source,
			CommType messageType,DateTime today) {
			//No need to check MiddleTierRole; no call to db.
			List<string> listErrors=new List<string>();
			if(listRecallNums==null || listRecallNums.Count < 1) {
				listErrors.Add(Lans.g("WebSched","No Recalls to schedule for ")+messageType.GetDescription());
				return listErrors;
			}
			//Loop through the selected patients and insert WebSchedRecalls so that the Auto Comm Web Sched thread can aggregate the recalls and send
			//messages.
			//Without filtering on messagetype for this in Recalls.GetAddrTAbleForWebSched the last entry will be blocked from getting inserted. 
			DataTable addrTable=Recalls.GetAddrTableForWebSched(listRecallNums,isGroupFamily,sortBy,ListTools.FromSingle(messageType));
			List<WebSchedRecall> listWebSchedRecalls=new List<WebSchedRecall>();
			for(int i=0;i<addrTable.Rows.Count;i++) {
				DataRow row=addrTable.Rows[i];
				long recallNum=PIn.Long(row["RecallNum"].ToString());
				DateTime dueDate=PIn.Date(row["dateDue"].ToString());
				if(dueDate.Year < 1880) {
					//It is common for offices to have patients with a blank recall date (they've never had a recall performed at the office).
					//Instead of showing 01/01/0001 in the email, we will simply show today's date because that is what the Web Sched time slots will start showing.
					dueDate=today.Date;
				}
				listWebSchedRecalls.Add(new WebSchedRecall {
					RecallNum=recallNum,
					ClinicNum=PIn.Long(row["ClinicNum"].ToString()),
					PatNum=PIn.Long(row["PatNum"].ToString()),
					ReminderCount=PIn.Int(row["numberOfReminders"].ToString()),		
					DateDue=dueDate,						
					DateTimeSent=DateTime.MinValue,
					Source=source,
					SendStatus=AutoCommStatus.SendNotAttempted,//AutoCommWebSchedRecall thread will try to send a message.
					MessageType=messageType,
				});
			}
			WebSchedRecalls.InsertMany(listWebSchedRecalls);
			return listErrors;
		}

		///<summary></summary>
		public static long Insert(WebSchedRecall webSchedRecall) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				webSchedRecall.WebSchedRecallNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webSchedRecall);
				return webSchedRecall.WebSchedRecallNum;
			}
			return Crud.WebSchedRecallCrud.Insert(webSchedRecall);
		}

		///<summary></summary>
		public static void InsertMany(List<WebSchedRecall> listWebSchedRecalls) {
			if(listWebSchedRecalls.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listWebSchedRecalls);
				return;
			}
			Crud.WebSchedRecallCrud.InsertMany(listWebSchedRecalls);
		}

		///<summary></summary>
		public static bool Update(WebSchedRecall webSchedRecall,WebSchedRecall webSchedRecallOld=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),webSchedRecall,webSchedRecallOld);
			}
			if(webSchedRecallOld is null) {
				Crud.WebSchedRecallCrud.Update(webSchedRecall);
				return true;
			}
			else {
				return Crud.WebSchedRecallCrud.Update(webSchedRecall,webSchedRecallOld);
			}
		}

		///<summary></summary>
		public static void Delete(long webSchedRecallNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webSchedRecallNum);
				return;
			}
			Crud.WebSchedRecallCrud.Delete(webSchedRecallNum);
		}

		///<summary></summary>
		public static long DeleteOlderThan(DateTime dateUpperBound) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),dateUpperBound);
			}
			string command=$"DELETE FROM webschedrecall WHERE DateTimeEntry<{POut.DateT(dateUpperBound)}";
			return Db.NonQ(command);
		}

		public static List<WebSchedRecall> GetBySmsToMobile(SmsToMobile sms) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),sms);
			}
			string command=$"SELECT * FROM webschedrecall WHERE MessageFk={POut.Long(sms.SmsToMobileNum)} AND MessageType={POut.Int((int)CommType.Text)}";
			return Crud.WebSchedRecallCrud.SelectMany(command);
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM webschedrecall WHERE PatNum = "+POut.Long(patNum);
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}
		
		
		*/
	}
}