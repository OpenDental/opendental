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
			WebSchedRecall webSchedRecall=listWebSchedRecalls.Find(x=>x.MessageType==CommType.Email && x.Message.Contains("http"));
			if(webSchedRecall==null) {
				webSchedRecall=listWebSchedRecalls.Find(x=>x.Message.Contains("http"));
			}
			//Ideally we want the email with http, if not we'll settle for a text with http, otherwise we'll just take what we can get
			if(webSchedRecall==null) {
				return listWebSchedRecalls.FirstOrDefault();
			}
			return webSchedRecall;
		}
		#endregion

		///<summary>Gets all WebSchedRecalls for which a reminder has not been sent. To get for all clinics, don't include the listClinicNums or pass
		///in an empty list.</summary>
		public static List<WebSchedRecall> GetAllUnsent(List<CommType> listCommTypes=null,List<long> listClinicNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),listCommTypes,listClinicNums);
			}
			//Default to All except Invalid if not specified.
			listCommTypes??=Enum.GetValues(typeof(CommType)).Cast<CommType>().ToList().FindAll(x => x!=CommType.Invalid);
			//We don't want to include rows that have a status of SendFailed or SendSuccess
			string command="SELECT * FROM webschedrecall WHERE DateTimeSent < '1880-01-01' "
				+$"AND MessageType IN ({string.Join(",",listCommTypes.Select(x => POut.Int((int)x)))}) "
				+$"AND SendStatus={POut.Int((int)AutoCommStatus.SendNotAttempted)} ";
			if(listClinicNums!=null && listClinicNums.Count > 0) {
				command+="AND ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}

		///<summary>Insert WebSchedRecalls so that the Auto Comm Web Sched thread can aggregate the recalls and send emails and texts.
		///Returns a list of errors to display to the user if anything went wrong otherwise returns empty list if everything was successful.</summary>
		public static List<string> InsertForRecallNums(List<long> listRecallNums,bool isGroupFamily,RecallListSort recallListSort,
			WebSchedRecallSource webSchedRecallSource,CommType commType,DateTime dateToday) {
			Meth.NoCheckMiddleTierRole();
			List<string> listErrors=new List<string>();
			if(listRecallNums==null || listRecallNums.Count < 1) {
				listErrors.Add(Lans.g("WebSched","No Recalls to schedule for ")+commType.GetDescription());
				return listErrors;
			}
			//Loop through the selected patients and insert WebSchedRecalls so that the Auto Comm Web Sched thread can aggregate the recalls and send
			//messages.
			//Without filtering on messagetype for this in Recalls.GetAddrTAbleForWebSched the last entry will be blocked from getting inserted. 
			DataTable tableAddrs=Recalls.GetAddrTableForWebSched(listRecallNums,isGroupFamily,recallListSort,ListTools.FromSingle(commType));
			List<WebSchedRecall> listWebSchedRecalls=new List<WebSchedRecall>();
			for(int i=0;i<tableAddrs.Rows.Count;i++) {
				DataRow dataRow=tableAddrs.Rows[i];
				long recallNum=PIn.Long(dataRow["RecallNum"].ToString());
				DateTime dateDue=PIn.Date(dataRow["dateDue"].ToString());
				if(dateDue.Year < 1880) {
					//It is common for offices to have patients with a blank recall date (they've never had a recall performed at the office).
					//Instead of showing 01/01/0001 in the email, we will simply show today's date because that is what the Web Sched time slots will start showing.
					dateDue=dateToday.Date;
				}
				listWebSchedRecalls.Add(new WebSchedRecall {
					RecallNum=recallNum,
					ClinicNum=PIn.Long(dataRow["ClinicNum"].ToString()),
					PatNum=PIn.Long(dataRow["PatNum"].ToString()),
					ReminderCount=PIn.Int(dataRow["numberOfReminders"].ToString()),		
					DateDue=dateDue,						
					DateTimeSent=DateTime.MinValue,
					Source=webSchedRecallSource,
					SendStatus=AutoCommStatus.SendNotAttempted,//AutoCommWebSchedRecall thread will try to send a message.
					MessageType=commType,
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
			return Crud.WebSchedRecallCrud.Update(webSchedRecall,webSchedRecallOld);
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

		public static List<WebSchedRecall> GetBySmsToMobile(SmsToMobile smsToMobile) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebSchedRecall>>(MethodBase.GetCurrentMethod(),smsToMobile);
			}
			string command=$"SELECT * FROM webschedrecall WHERE MessageFk={POut.Long(smsToMobile.SmsToMobileNum)} AND MessageType={POut.Int((int)CommType.Text)}";
			return Crud.WebSchedRecallCrud.SelectMany(command);
		}

		///<summary>Returns true if any Web Sched Recall text or email template contains a URL tag.</summary>
		public static bool TemplatesHaveURLTags() {
			List<PrefName> listPrefNames=new List<PrefName>();
			listPrefNames.Add(PrefName.WebSchedSubject);
			listPrefNames.Add(PrefName.WebSchedMessage);
			listPrefNames.Add(PrefName.WebSchedMessageText);
			listPrefNames.Add(PrefName.WebSchedAggregatedEmailBody);
			listPrefNames.Add(PrefName.WebSchedAggregatedEmailSubject);
			listPrefNames.Add(PrefName.WebSchedAggregatedTextMessage);
			listPrefNames.Add(PrefName.WebSchedSubject2);
			listPrefNames.Add(PrefName.WebSchedMessage2);
			listPrefNames.Add(PrefName.WebSchedMessageText2);
			listPrefNames.Add(PrefName.WebSchedSubject3);
			listPrefNames.Add(PrefName.WebSchedMessage3);
			listPrefNames.Add(PrefName.WebSchedMessageText3);
			//Check pref values first, because if translations have tags then it is likely the default preferences do too.
			for(int i=0;i<listPrefNames.Count;i++) {
				if(HasURLTag(PrefC.GetString(listPrefNames[i]))) {
					return true;
				}
			}
			//If no URL tags in pref values, then check translations. Only checking actively used languages.
			List<string> listLanguages=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(',').ToList();
			List<string> listPrefStrings=listPrefNames.Select(x => x.ToString()).ToList();
			List<LanguagePat> listLanguagePats=LanguagePats.GetListPrefTranslationsFromDb(listPrefStrings,listLanguages);
			for(int i=0;i<listLanguagePats.Count;i++) {
				if(HasURLTag(listLanguagePats[i].Translation)) {
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