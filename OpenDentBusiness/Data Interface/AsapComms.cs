using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AsapComms {
		///<summary>Do not send a text to a patient if it is less than this many minutes before the start of time slot.</summary>
		public const int TextMinMinutesBefore=30;

		#region Get Methods

		///<summary>Gets one AsapComm from the db.</summary>
		public static AsapComm GetOne(long asapCommNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<AsapComm>(MethodBase.GetCurrentMethod(),asapCommNum);
			}
			return Crud.AsapCommCrud.SelectOne(asapCommNum);
		}

		///<summary>Gets a list of all AsapComms matching the passed in parameters.</summary>
		///<param name="listSQLWheres">To get all AsapComms, don't include this parameter.</param>
		private static List<AsapComm> GetMany(List<SQLWhere> listSQLWheres=null) {
			Meth.NoCheckMiddleTierRole();//Private method.
			string command="SELECT * FROM asapcomm ";
			if(listSQLWheres!=null && listSQLWheres.Count > 0) {
				command+="WHERE "+string.Join(" AND ",listSQLWheres);
			}
			return Crud.AsapCommCrud.SelectMany(command);
		}

		///<summary>Gets a list of all AsapComms for the given patients.</summary>
		public static List<AsapComm> GetForPats(List<long> listPatNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AsapComm>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			List<SQLWhere> listSQLWheres=new List<SQLWhere>();
			SQLWhere sqlWhere=SQLWhere.CreateIn(nameof(AsapComm.PatNum),listPatNums);
			listSQLWheres.Add(sqlWhere);
			return GetMany(listSQLWheres);
		}

		///<summary>Gets a list of all AsapComms that have not been attempted to send.</summary>
		public static List<AsapComm> GetReadyToSend(DateTime dateTimeNow) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AsapComm>>(MethodBase.GetCurrentMethod(),dateTimeNow);
			}
			//Using a UNION in order to use the indexes on SmsSendStatus and EmailSendStatus.
			string command=@"SELECT * 
				FROM asapcomm 
				WHERE (asapcomm.SmsSendStatus="+POut.Int((int)AutoCommStatus.SendNotAttempted)+@"
				AND asapcomm.DateTimeSmsScheduled<="+POut.DateT(dateTimeNow)+@")
				UNION
				SELECT * 
				FROM asapcomm 				
				WHERE asapcomm.EmailSendStatus="+POut.Int((int)AutoCommStatus.SendNotAttempted);			
			return Crud.AsapCommCrud.SelectMany(command);
		}

		///<summary>Gets a list of AsapComms (along with a few more fields) for use in the Web Sched History window. To view for all patients or clinics, 
		///pass in null for those parameters.</summary>
		public static List<AsapCommHist> GetHist(DateTime dateFrom,DateTime dateTo,List<long> listPatNums=null,List<long> listClinicNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AsapCommHist>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listPatNums,listClinicNums);
			}
			string command=@"
				SELECT asapcomm.*,"+DbHelper.Concat("patient.LName","', '","patient.FName")+@" PatientName,COALESCE(schedule.StartTime,'00:00:00') StartTime,
				COALESCE(schedule.StopTime,'00:00:00') StopTime,COALESCE(schedule.SchedDate,'0001-01-01') SchedDate,
				COALESCE(emailmessage.BodyText,'') EmailMessageText,COALESCE(smstomobile.MsgText,'') SMSMessageText
				FROM asapcomm
				INNER JOIN patient ON patient.PatNum=asapcomm.PatNum
				LEFT JOIN schedule ON schedule.ScheduleNum=asapcomm.ScheduleNum
				LEFT JOIN emailmessage ON emailmessage.EmailMessageNum=asapcomm.EmailMessageNum
				LEFT JOIN smstomobile ON smstomobile.GuidMessage=asapcomm.GuidMessageToMobile 
				WHERE "+DbHelper.BetweenDates("asapcomm.DateTimeEntry",dateFrom,dateTo)+" ";
			if(listPatNums!=null) {
				if(listPatNums.Count==0) {
					return new List<AsapCommHist>();
				}
				command+="AND asapcomm.PatNum IN("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+") ";
			}
			if(listClinicNums!=null) {
				if(listClinicNums.Count==0) {
					return new List<AsapCommHist>();
				}
				command+="AND asapcomm.ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			DataTable table=Db.GetTable(command);
			List<AsapCommHist> listAsapCommHists=Crud.AsapCommCrud.TableToList(table).Select(x => new AsapCommHist() { AsapComm=x }).ToList();
			for(int i=0;i<listAsapCommHists.Count;i++) {
				listAsapCommHists[i].PatientName=PIn.String(table.Rows[i]["PatientName"].ToString());
				listAsapCommHists[i].DateTimeSlotStart=PIn.Date(table.Rows[i]["SchedDate"].ToString()).Add(PIn.Time(table.Rows[i]["StartTime"].ToString()));
				listAsapCommHists[i].DateTimeSlotEnd=PIn.Date(table.Rows[i]["SchedDate"].ToString()).Add(PIn.Time(table.Rows[i]["StopTime"].ToString()));
				listAsapCommHists[i].EmailMessageText=PIn.String(table.Rows[i]["EmailMessageText"].ToString());
				listAsapCommHists[i].SMSMessageText=PIn.String(table.Rows[i]["SMSMessageText"].ToString());
			}
			return listAsapCommHists;
		}

		///<summary>Gets a List of AsapComms that meets a set of criteria for the API.</summary>
		public static List<AsapComm> GetAsapCommsForApi(int limit,int offset,long clinicNum,DateTime dateTStart,DateTime dateTEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AsapComm>>(MethodBase.GetCurrentMethod(),limit,offset,clinicNum,dateTStart,dateTEnd);
			}
			string command="SELECT * from asapcomm "
				+"WHERE DateTimeEntry >= "+POut.DateT(dateTStart)+" "
				+"AND DateTimeEntry < "+POut.DateT(dateTEnd)+" ";
			if(clinicNum>-1) {
				command+="AND ClinicNum="+POut.Long(clinicNum)+" ";
			}
			command+="ORDER BY AsapCommNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.AsapCommCrud.SelectMany(command);
		}

		#endregion

		#region Insert

		///<summary></summary>
		public static long Insert(AsapComm asapComm) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				asapComm.AsapCommNum=Meth.GetLong(MethodBase.GetCurrentMethod(),asapComm);
				return asapComm.AsapCommNum;
			}
			return Crud.AsapCommCrud.Insert(asapComm);
		}

		///<summary></summary>
		public static void InsertMany(List<AsapComm> listAsapComms) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAsapComms);
				return;
			}
			Crud.AsapCommCrud.InsertMany(listAsapComms);
		}

		///<summary>Inserts these AsapComms into the database. Also creates a block on the schedule recording this communication.</summary>
		public static void InsertForSending(List<AsapComm> listAsapComms,DateTime dateTSlotStart,DateTime dateTSlotEnd,long opNum) {
			Meth.NoCheckMiddleTierRole();
			int countTextsToBeSent=listAsapComms.Count(x => x.SmsSendStatus!=AutoCommStatus.DoNotSend);
			int countEmailsToBeSent=listAsapComms.Count(x => x.EmailSendStatus!=AutoCommStatus.DoNotSend);
			//Create a slot on the appointment schedule.
			Schedule schedule=new Schedule();
			schedule.SchedDate=dateTSlotStart.Date;
			schedule.SchedType=ScheduleType.WebSchedASAP;
			schedule.StartTime=dateTSlotStart.TimeOfDay;
			if(dateTSlotEnd.Date > dateTSlotStart.Date) {
				schedule.StopTime=new TimeSpan(23,59,59);//Last second of the day
			}
			else {
				schedule.StopTime=dateTSlotEnd.TimeOfDay;
			}
			schedule.Ops=new List<long> { opNum };
			schedule.Note=countTextsToBeSent+" "+Lans.g("ContrAppt","text"+(countTextsToBeSent==1 ? "" : "s")+" to be sent")+"\r\n"
				+countEmailsToBeSent+" "+Lans.g("ContrAppt","email"+(countEmailsToBeSent==1 ? "" : "s")+" to be sent");
			Schedules.Insert(schedule,validate:false);
			for(int i=0;i<listAsapComms.Count();i++){
				listAsapComms[i].ScheduleNum=schedule.ScheduleNum;
			}
			InsertMany(listAsapComms);
		}

		#endregion

		#region Update
		///<summary></summary>
		public static void Update(AsapComm asapComm) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),asapComm);
				return;
			}
			Crud.AsapCommCrud.Update(asapComm);
		}

		public static void Update(AsapComm asapComm,AsapComm asapCommOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),asapComm,asapCommOld);
				return;
			}
			Crud.AsapCommCrud.Update(asapComm,asapCommOld);
		}

		public static void SetRsvpStatus(AsapRSVPStatus asapRsvpStatus,List<string> listShortGuids) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),asapRsvpStatus,listShortGuids);
				return;
			}
			if(listShortGuids.Count==0) {
				return;
			}
			string command="UPDATE asapcomm SET ResponseStatus="+POut.Int((int)asapRsvpStatus)
				+" WHERE ShortGUID IN('"+string.Join("','",listShortGuids.Select(x => POut.String(x)))+"')";
			Db.NonQ(command);
		}

		#endregion

		#region Misc Methods

		///<summary>Replaces the template with the passed in arguments.</summary>
		public static string ReplacesTemplateTags(string template,long clinicNum=-1,DateTime dateTime=new DateTime(),string nameF=null,
			string asapUrl=null,bool isHtmlEmail=false) 
		{
			Meth.NoCheckMiddleTierRole();
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.Append(template);
			//Note: RegReplace is case insensitive by default.
			if(dateTime.Year > 1880) {
				StringTools.RegReplace(stringBuilder,"\\[Date]",dateTime.ToString(PrefC.PatientCommunicationDateFormat));
				StringTools.RegReplace(stringBuilder,"\\[Time]",dateTime.ToShortTimeString());
			}
			if(clinicNum > -1) {
				Clinic clinic=Clinics.GetClinic(clinicNum)??Clinics.GetPracticeAsClinicZero();
				Clinics.ReplaceOffice(stringBuilder,clinic,isHtmlEmail,replaceDisclaimer:isHtmlEmail);
			}
			if(nameF!=null) {
				StringTools.RegReplace(stringBuilder,"\\[NameF]",nameF);
			}
			if(asapUrl!=null) {
				StringTools.RegReplace(stringBuilder,"\\[AsapURL]",asapUrl);
			}
			return stringBuilder.ToString();
		}

		///<summary>Creates a list of AsapComms for sending.</summary>
		public static AsapListSender CreateSendList(List<Appointment> listAppointments,List<Recall> listRecalls,List<PatComm> listPatComms,SendMode sendMode,
			string templateText,string templateEmail,string emailSubject,DateTime dateTSlotStart,DateTime dateTStartSend,long clinicNum,bool isRawHtml) 
		{
			Meth.NoCheckMiddleTierRole();
			AsapListSender asapListSender=new AsapListSender(sendMode,listPatComms,clinicNum,dateTSlotStart,dateTStartSend);
			//Order matters here. We will send messages to appointments that are unscheduled first, then scheduled appointments, then recalls. This is
			//because we would prefer to create a brand new appointment than create a hole in the schedule where another appointment was scheduled.
			//We're doing recalls last because cleanings would be lower priority than other types of dental work.
			List<Appointment> listAppointmentsOrdered=listAppointments.OrderBy(x => x.AptStatus!=ApptStatus.UnschedList)
				.ThenBy(x => x.AptStatus!=ApptStatus.Planned)
				.ThenByDescending(x => x.AptDateTime).ToList();
			for(int i=0;i<listAppointmentsOrdered.Count();i++){
				AsapComm asapComm=new AsapComm();
				asapComm.DateTimeOrig=listAppointmentsOrdered[i].AptDateTime;
				asapComm.FKey=listAppointmentsOrdered[i].AptNum;
				asapComm.ClinicNum=clinicNum;
				asapComm.DateTimeExpire=dateTSlotStart.AddDays(7);//Give a 7 day buffer so that the link will still be active a little longer.
				switch(listAppointmentsOrdered[i].AptStatus) {
					case ApptStatus.Scheduled:
						asapComm.FKeyType=AsapCommFKeyType.ScheduledAppt;
						break;
					case ApptStatus.UnschedList:
						asapComm.FKeyType=AsapCommFKeyType.UnscheduledAppt;
						break;
					case ApptStatus.Broken:
						asapComm.FKeyType=AsapCommFKeyType.Broken;
						break;
					case ApptStatus.Planned:
					default:
						asapComm.FKeyType=AsapCommFKeyType.PlannedAppt;
						break;
				}
				if(asapListSender.ShouldSendText(listAppointmentsOrdered[i].PatNum,listAppointmentsOrdered[i].AptNum,asapComm.FKeyType)) {//This will record in the Note why the patient can't be sent a text.
					asapComm.DateTimeSmsScheduled=asapListSender.GetNextTextSendTime();
					asapComm.SmsSendStatus=AutoCommStatus.SendNotAttempted;
					asapComm.TemplateText=templateText;
					asapListSender.CountTextsToSend++;
				}
				else {
					asapComm.SmsSendStatus=AutoCommStatus.DoNotSend;
				}
				if(asapListSender.ShouldSendEmail(listAppointmentsOrdered[i].PatNum,listAppointmentsOrdered[i].AptNum,asapComm.FKeyType)) {//This will record in the Note why the patient can't be sent a email.
					asapComm.EmailSendStatus=AutoCommStatus.SendNotAttempted;
					asapComm.TemplateEmail=templateEmail;
					asapComm.TemplateEmailSubj=emailSubject;
					asapComm.EmailTemplateType=EmailType.Html;
					if(isRawHtml){
						asapComm.EmailTemplateType=EmailType.RawHtml;
					}
					asapListSender.CountEmailsToSend++;
				}
				else {
					asapComm.EmailSendStatus=AutoCommStatus.DoNotSend;
				}
				asapComm.PatNum=listAppointmentsOrdered[i].PatNum;
				if(asapComm.SmsSendStatus==AutoCommStatus.DoNotSend && asapComm.EmailSendStatus==AutoCommStatus.DoNotSend) {
					asapComm.ResponseStatus=AsapRSVPStatus.UnableToSend;
				}
				else {
					asapComm.ResponseStatus=AsapRSVPStatus.AwaitingTransmit;
				}
				asapListSender.ListAsapComms.Add(asapComm);
			}
			asapListSender.CopyNotes();
			//Now do recalls
			List<Recall>listRecallsOrdered=listRecalls.OrderByDescending(x => x.DateDue).ToList();
			for(int i=0;i<listRecallsOrdered.Count();i++){
				AsapComm asapComm=new AsapComm();
				asapComm.DateTimeOrig=listRecallsOrdered[i].DateDue;
				asapComm.FKey=listRecallsOrdered[i].RecallNum;
				asapComm.FKeyType=AsapCommFKeyType.Recall;
				asapComm.ClinicNum=clinicNum;
				asapComm.DateTimeExpire=dateTSlotStart.AddDays(7);//Give a 7 day buffer so that the link will still be active a little longer.
				if(asapListSender.ShouldSendText(listRecallsOrdered[i].PatNum,listRecallsOrdered[i].RecallNum,AsapCommFKeyType.Recall)) {//This will record in the Note why the patient can't be sent a text.
					asapComm.DateTimeSmsScheduled=asapListSender.GetNextTextSendTime();
					asapComm.SmsSendStatus=AutoCommStatus.SendNotAttempted;
					asapComm.TemplateText=templateText;
					asapListSender.CountTextsToSend++;
				}
				else {
					asapComm.SmsSendStatus=AutoCommStatus.DoNotSend;
				}
				if(asapListSender.ShouldSendEmail(listRecallsOrdered[i].PatNum,listRecallsOrdered[i].RecallNum,AsapCommFKeyType.Recall)) {//This will record in the Note why the patient can't be sent a email.
					asapComm.EmailSendStatus=AutoCommStatus.SendNotAttempted;
					asapComm.TemplateEmail=templateEmail;
					asapComm.TemplateEmailSubj=emailSubject;
					asapListSender.CountEmailsToSend++;
				}
				else {
					asapComm.EmailSendStatus=AutoCommStatus.DoNotSend;
				}
				asapComm.PatNum=listRecallsOrdered[i].PatNum;
				if(asapComm.SmsSendStatus==AutoCommStatus.DoNotSend && asapComm.EmailSendStatus==AutoCommStatus.DoNotSend) {
					asapComm.ResponseStatus=AsapRSVPStatus.UnableToSend;
				}
				else {
					asapComm.ResponseStatus=AsapRSVPStatus.AwaitingTransmit;
				}
				asapListSender.ListAsapComms.Add(asapComm);
			}
			return asapListSender;
		}

		///<summary>Updates the Schedule note with the number of sent and waiting to send AsapComms.</summary>
		public static void UpdateSchedule(long scheduleNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),scheduleNum);
				return;
			}
			List<Schedule> listSchedules=Schedules.GetByScheduleNum(new List<long> { scheduleNum });
			if(listSchedules.Count==0) {
				return;
			}
			Schedule schedule=listSchedules[0];
			List<SQLWhere> listSQLWheres=new List<SQLWhere>();
			SQLWhere sqlWhere=SQLWhere.Create(nameof(AsapComm.ScheduleNum),ComparisonOperator.Equals,scheduleNum);
			listSQLWheres.Add(sqlWhere);
			List<AsapComm> listAsapComms=GetMany(listSQLWheres);
			int countTextsSent=listAsapComms.Count(x => x.SmsSendStatus==AutoCommStatus.SendSuccessful);
			int countTextsToBeSent=listAsapComms.Count(x => x.SmsSendStatus==AutoCommStatus.SendNotAttempted);
			int countEmailsSent=listAsapComms.Count(x => x.EmailSendStatus==AutoCommStatus.SendSuccessful);
			int countEmailsToBeSent=listAsapComms.Count(x => x.EmailSendStatus==AutoCommStatus.SendNotAttempted);
			schedule.Note=countTextsSent+" "+Lans.g("ContrAppt","text"+(countTextsSent==1 ? "" : "s")+" sent,")+" "
				+countTextsToBeSent+" "+Lans.g("ContrAppt","text"+(countTextsToBeSent==1 ? "" : "s")+" to be sent")+"\r\n"
				+countEmailsSent+" "+Lans.g("ContrAppt","email"+(countEmailsSent==1 ? "" : "s")+" sent")+" "
				+countEmailsToBeSent+" "+Lans.g("ContrAppt","email"+(countEmailsToBeSent==1 ? "" : "s")+" to be sent");
			Schedules.Update(schedule);
		}
		
		public static AsapComm GetByShortGuid(string shortGuid) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<AsapComm>(MethodBase.GetCurrentMethod(),shortGuid);
			}
			string command=@"SELECT *
				FROM asapcomm
				WHERE asapcomm.ShortGUID='"+POut.String(shortGuid)+@"'
				ORDER BY asapcomm.DateTimeExpire DESC";
			command=DbHelper.LimitOrderBy(command,1);//In the very unlikely event that the same short guid winds up in the database twice.
			return Crud.AsapCommCrud.SelectOne(command);
		}

		#endregion

		///<summary>The mode by which these AsapComms will be sent out.</summary>
		public enum SendMode {
			TextAndEmail,
			Text,
			Email,
			PreferredContact,
		}

		#region Helper classes

		[Serializable]
		public class AsapCommHist {
			public AsapComm AsapComm;
			public string PatientName;
			public DateTime DateTimeSlotStart;
			public DateTime DateTimeSlotEnd;
			public string EmailMessageText;
			public string SMSMessageText;
		}


		///<summary>Helper class used to create a list of AsapComms to send.</summary>
		public class AsapListSender {
			///<summary>The AsapComms to be sent.</summary>
			public List<AsapComm> ListAsapComms;
			///<summary>A breakdown of who is and isn't receiving what.</summary>
			private List<PatientDetail> _listPatientDetails;
			///<summary>The number of texts that are going to be sent.</summary>
			public int CountTextsToSend { get; internal set; }
			///<summary>The number of emails that are going to be sent.</summary>
			public int CountEmailsToSend { get; internal set; }
			///<summary>The time when texts will start to be sent.</summary>
			public DateTime DateTimeStartSendText { get; private set; }
			///<summary>The time when the emails will be sent.</summary>
			public DateTime DateTimeSendEmail { get; private set; }
			///<summary>The number of minutes that will elapse between texts being sent out.</summary>
			public int MinutesBetweenTexts { get; private set; }
			///<summary>True if it is currently outside the automatic send window.</summary>
			public bool IsOutsideSendWindow=false;
			///<summary>The date time all texts need to be sent by. Based on PrefName.AutomaticCommunicationTimeEnd. May be today or tomorrow.</summary>
			public DateTime DateTimeTextSendEnd;
			private List<PatComm> _listPatComms;
			///<summary>Key: PatNum, Value: All AsapComms for the patient.</summary>
			private List<AsapComm> _listAsapComms;
			private SendMode _sendMode;
			private int _maxTextsPerDay;
			private DateTime _dateTimeSlotStart;
			private const string _lanThis="FormWebSchedASAPSend";			

			///<summary>Initialize the sender helper for the given PatComms and appointments.</summary>
			///<param name="clinicNum">The clinic that is doing the sending.</param>
			///<param name="dateTimeSlotStart">The date time of the time slot for which this list is being sent.</param>
			///<param name="dateTimeStartSend">The date time when the list should be sent out. This time will be adjusted if necessary.</param>
			internal AsapListSender(SendMode sendMode,List<PatComm> listPatComms,long clinicNum,DateTime dateTimeSlotStart,
				DateTime dateTimeStartSend) 
			{
				_sendMode=sendMode;
				//listPatComms is one per appointment, but this could include multiple per PatNum.
				_listPatComms=listPatComms;
				_listPatientDetails=listPatComms.Select(x=>new PatientDetail(x)).Distinct().ToList();
				_listAsapComms=GetForPats(listPatComms.Select(x=>x.PatNum).ToList());
				TimeSpan timeSpanAutoCommStart=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeStart).TimeOfDay;
				TimeSpan timeSpanAutoCommEnd=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeEnd).TimeOfDay;
				DateTimeSendEmail=dateTimeStartSend;//All emails will be sent immediately.
				DateTimeStartSendText=dateTimeStartSend;
				if(PrefC.DoRestrictAutoSendWindow) {
					//If the time to start sending is before the automatic send window, set the time to start to the beginning of the send window.
					if(DateTimeStartSendText.TimeOfDay < timeSpanAutoCommStart) {
						DateTimeStartSendText=DateTimeStartSendText.Date.Add(timeSpanAutoCommStart);
						IsOutsideSendWindow=true;
					}
					else if(DateTimeStartSendText.TimeOfDay > timeSpanAutoCommEnd) {
						//If the time to start sending is after the automatic send window, set the time to start to the beginning of the send window the next day.
						DateTimeStartSendText=DateTimeStartSendText.Date.AddDays(1).Add(timeSpanAutoCommStart);
						IsOutsideSendWindow=true;
					}
				}
				string strMaxTextsPrefVal=ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapTextLimit,clinicNum);
				_maxTextsPerDay=PIn.Int(strMaxTextsPrefVal); //The pref may be set to blank to have no limit
				if(String.IsNullOrWhiteSpace(strMaxTextsPrefVal)){
					_maxTextsPerDay=int.MaxValue;
				}
				DateTimeTextSendEnd=DateTimeStartSendText.Date.Add(timeSpanAutoCommEnd);
				_dateTimeSlotStart=dateTimeSlotStart;
				SetMinutesBetweenTexts(dateTimeSlotStart);
				ListAsapComms=new List<AsapComm>();
			}

			public List<PatientDetail> GetListPatientDetails(){
				return _listPatientDetails;
			}

			///<summary>Sets the number of minutes between texts.</summary>
			private void SetMinutesBetweenTexts(DateTime dateTimeSlotStart) {
				Meth.NoCheckMiddleTierRole();
				int hoursUntilSlotStart=(int)(dateTimeSlotStart-DateTimeStartSendText).TotalHours;
				if(hoursUntilSlotStart < 2) {
					MinutesBetweenTexts=1;
				}
				else if(hoursUntilSlotStart.Between(2,11)) {
					MinutesBetweenTexts=2;
				}
				else if(hoursUntilSlotStart.Between(12,47)) {
					MinutesBetweenTexts=4;
				}
				else {
					MinutesBetweenTexts=8;
				}
			}
			
			///<summary>Returns true if the patient should be sent a text. If false, the reason why the patient can't receive a text is 
			///added to the details dictionary.</summary>
			internal bool ShouldSendText(long patNum,long fkey, AsapCommFKeyType fkeyType) {
				Meth.NoCheckMiddleTierRole();
				PatComm patComm=_listPatComms.Find(x=>x.PatNum==patNum);
				if(patComm==null) {
					return false;
				}
				PatientDetail patientDetail=_listPatientDetails.Find(x=>x.PatNum==patNum);
				if(patientDetail==null) {
					patientDetail=new PatientDetail();
					patientDetail.PatNum=patNum;
					_listPatientDetails.Add(patientDetail);
				}
				patientDetail.IsSendingText=false;
				if(_sendMode==SendMode.Email) {
					return false;//No need to note the reason.
				}
				List<AsapComm> listAsapCommsPat=_listAsapComms.FindAll(x=>x.PatNum==patNum);
				if(listAsapCommsPat.Count>0) {
					if(listAsapCommsPat.Any(x => x.FKey==fkey && x.FKeyType==fkeyType && x.ResponseStatus==AsapRSVPStatus.DeclinedStopComm))
					{
						string text_type=fkeyType==AsapCommFKeyType.Recall ? "recall" : "appointment";
						patientDetail.AppendNote(Lans.g(_lanThis,"Not sending text because this patient has requested to not be texted or emailed about this "
							+text_type+"."));
						return false;
					}
					int countTextsSent=listAsapCommsPat.Count(x => (x.SmsSendStatus==AutoCommStatus.SendNotAttempted && x.DateTimeSmsScheduled.Date==DateTimeStartSendText.Date)
						|| (x.SmsSendStatus==AutoCommStatus.SendSuccessful && x.DateTimeSmsSent.Date==DateTimeStartSendText.Date));
					if(countTextsSent>=_maxTextsPerDay) {
						patientDetail.AppendNote(Lans.g(_lanThis,"Not sending text because this patient has received")+" "+_maxTextsPerDay+" "
							+Lans.g(_lanThis,"texts today."));
						return false;
					}
				}
				bool isWithin30Minutes=(GetNextTextSendTime() < _dateTimeSlotStart && (_dateTimeSlotStart-GetNextTextSendTime()).TotalMinutes < TextMinMinutesBefore);
				bool isAfterSlot=(GetNextTextSendTime() > _dateTimeSlotStart);
				if(isWithin30Minutes) {
					patientDetail.AppendNote(Lans.g(_lanThis,"Not sending text because the text would be sent less than")+" "+TextMinMinutesBefore+" "
						+Lans.g(_lanThis,"minutes before the time slot."));
					return false;
				}
				if(isAfterSlot) {
					patientDetail.AppendNote(Lans.g(_lanThis,"Not sending text because the text would be sent after the time slot."));
					return false;
				}
				if(_sendMode==SendMode.Email) {
					return false;
				}
				if(_sendMode==SendMode.PreferredContact && patComm.PreferContactMethod!=ContactMethod.TextMessage) {
					patientDetail.AppendNote(Lans.g(_lanThis,"Not sending text because this patient's preferred contact method is not text message."));
					return false;
				}
				if(!patComm.IsSmsAnOption) {
					patientDetail.AppendNote(Lans.g(_lanThis,patComm.GetReasonCantText(CommOptOutType.WebSchedASAP)));
					return false;
				}
				patientDetail.IsSendingText=true;
				return true;
			}

			///<summary>Returns true if the patient should be sent an email. If false, the reason why the patient can't receive an email is 
			///added to the details dictionary.</summary>
			internal bool ShouldSendEmail(long patNum,long fkey,AsapCommFKeyType fkeyType) {
				PatComm patComm=_listPatComms.Find(x=>x.PatNum==patNum);
				if(patComm==null) {
					return false;
				}
				PatientDetail patientDetail=_listPatientDetails.Find(x=>x.PatNum==patNum);
				if(patientDetail==null) {
					patientDetail=new PatientDetail();
					patientDetail.PatNum=patNum;
					_listPatientDetails.Add(patientDetail);
				}
				patientDetail.IsSendingEmail=false;
				if(_sendMode==SendMode.Text) {
					return false;//No need to note the reason.
				}
				List<AsapComm> listAsapCommsPat=_listAsapComms.FindAll(x=>x.PatNum==patNum);
				if(listAsapCommsPat.Count>0) {
					if(listAsapCommsPat.Any(x => x.FKey==fkey && x.FKeyType==fkeyType && x.ResponseStatus==AsapRSVPStatus.DeclinedStopComm))
					{
						string email_type=fkeyType==AsapCommFKeyType.Recall ? "recall" : "appointment";
						patientDetail.AppendNote(Lans.g(_lanThis,"Not sending email because this patient has requested to not be texted or emailed about this "
							+email_type+"."));
						return false;
					}
				}
				bool isWithin30Minutes=(DateTimeSendEmail < _dateTimeSlotStart && (_dateTimeSlotStart-DateTimeSendEmail).TotalMinutes < TextMinMinutesBefore);
				bool isAfterSlot=(DateTimeSendEmail > _dateTimeSlotStart);
				if(isWithin30Minutes) {
					patientDetail.AppendNote(Lans.g(_lanThis,"Not sending email because the email would be sent less than")+" "+TextMinMinutesBefore+" "
						+Lans.g(_lanThis,"minutes before the time slot."));
					return false;
				}
				if(isAfterSlot) {
					patientDetail.AppendNote(Lans.g(_lanThis,"Not sending email because the email would be sent after the time slot."));
					return false;
				}
				if(_sendMode==SendMode.Text) {
					return false;
				}
				if(_sendMode==SendMode.PreferredContact && patComm.PreferContactMethod!=ContactMethod.Email) {
					patientDetail.AppendNote(Lans.g(_lanThis,"Not sending email because this patient's preferred contact method is not email."));
					return false;
				}
				if(!patComm.IsEmailAnOption) {
					patientDetail.AppendNote(Lans.g(_lanThis,patComm.GetReasonCantEmail(CommOptOutType.WebSchedASAP)));
					return false;
				}
				patientDetail.IsSendingEmail=true;
				return true;
			}

			///<summary>Returns the time when the next text should be sent out.</summary>
			internal DateTime GetNextTextSendTime() {
				DateTime dateTimeSend=DateTimeStartSendText.AddMinutes(MinutesBetweenTexts*CountTextsToSend);
				if(PrefC.DoRestrictAutoSendWindow && dateTimeSend > DateTimeTextSendEnd) {
					dateTimeSend=DateTimeTextSendEnd;
				}
				return dateTimeSend;
			}

			///<summary>Copies the notes from the ListPatientDetails to the actual list of actual AsapComms.</summary>
			internal void CopyNotes() {
				for(int i=0;i<ListAsapComms.Count();i++){
					PatientDetail patientDetail=_listPatientDetails.Find(x=>x.PatNum==ListAsapComms[i].PatNum);
					if(patientDetail==null){
						continue;
					}
					ListAsapComms[i].Note+=patientDetail.Note;
				}
			}

			///<summary>An object used to hold details about specific patients.</summary>
			public class PatientDetail {
				public long PatNum;
				public string PatName;
				public bool IsSendingText;
				public bool IsSendingEmail;
				public string Note="";

				public PatientDetail() {
				}

				public PatientDetail(PatComm patComm) {
					if(patComm==null) {
						return;
					}
					PatName=patComm.LName+", "+patComm.FName;
					PatNum=patComm.PatNum;
				}

				internal void AppendNote(string note) {
					if(Note.Contains(note)) {
						return;//Don't include the same note twice. This could happen if the same patient is in the ASAP list twice.
					}
					if(!string.IsNullOrEmpty(Note)) {
						Note+="\r\n";
					}
					Note+=note;
				}
			}
		}

		///<summary>This class is used to check if appointments can fit in a given time slot.</summary>
		public class ApptAvailabilityChecker {
			///<summary>Appointments that have been previously gotten from the database.</summary>
			private List<Appointment> _listAppointments;
			///<summary>The list of appointment dates and operatories that have been gotten from the database.</summary>
			private List<DateTOpNum> _listDateTOpNums;

			public ApptAvailabilityChecker() {
				_listAppointments=new List<Appointment>();
				_listDateTOpNums=new List<DateTOpNum>();
			}

			///<summary>This constructor will store the appointments for the passed in dates and operatories.</summary>
			///<param name="listDateOps">DateTime is the AptDate, long is the OperatoryNum.</param>
			public ApptAvailabilityChecker(List<DateTOpNum> listDateTOpNums) {
				_listDateTOpNums=listDateTOpNums;
				_listAppointments=Appointments.GetApptsForDatesOps(listDateTOpNums);
			}

			///<summary>Returns true if the recall will fit in the time slot and there are no other appointments in the slot.</summary>
			public bool IsApptSlotAvailable(Recall recall,long opNum,DateTime dateTimeSlotStart,DateTime dateTimeSlotEnd) {
				Meth.NoCheckMiddleTierRole();
				int minutes=RecallTypes.GetTimePattern(recall.RecallTypeNum).Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement);
				return IsApptSlotAvailable(minutes,opNum,dateTimeSlotStart,dateTimeSlotEnd);
			}

			///<summary>Returns true if the appointment will fit in the time slot and there are no other appointments in the slot.</summary>
			public bool IsApptSlotAvailable(Appointment appointment,long opNum,DateTime dateTimeSlotStart,DateTime dateTimeSlotEnd) {
				Meth.NoCheckMiddleTierRole();
				return IsApptSlotAvailable(appointment.Length,opNum,dateTimeSlotStart,dateTimeSlotEnd);
			}

			///<summary>Returns true if the time length requested will fit in the time slot and there are no other appointments in the slot.</summary>
			public bool IsApptSlotAvailable(int minutes,long opNum,DateTime dateTimeSlotStart,DateTime dateTimeSlotEnd) {
				Meth.NoCheckMiddleTierRole();
				if(!_listDateTOpNums.Any(x => x.DateTAppt==dateTimeSlotStart.Date && x.OpNum==opNum)) {
					DateTOpNum dateTOpNum = new DateTOpNum();
					dateTOpNum.DateTAppt=dateTimeSlotStart;
					dateTOpNum.OpNum=opNum;
					List<DateTOpNum> listDateTOpNums = new List<DateTOpNum>(){dateTOpNum};
					_listAppointments.AddRange(Appointments.GetApptsForDatesOps(listDateTOpNums));
					_listDateTOpNums.Add(dateTOpNum);
				}
				DateTime dateTimeSlotEndNew=ODMathLib.Min(dateTimeSlotStart.AddMinutes(minutes),dateTimeSlotEnd);
				if(_listAppointments.FindAll(x => x.Op==opNum)
					.Any(x => MiscUtils.DoSlotsOverlap(x.AptDateTime,x.AptDateTime.AddMinutes(x.Length),dateTimeSlotStart,dateTimeSlotEndNew)))
				{
					return false;
				}
				return true;
			}
		}

		public class DateTOpNum{
			public DateTime DateTAppt;
			public long OpNum;
		}

		#endregion

		/*
Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<AsapComm> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AsapComm>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM asapcomm WHERE PatNum = "+POut.Long(patNum);
			return Crud.AsapCommCrud.SelectMany(command);
		}

				
	#region Delete
///<summary></summary>
public static void Delete(long asapCommNum) {
	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		Meth.GetVoid(MethodBase.GetCurrentMethod(),asapCommNum);
		return;
	}
	Crud.AsapCommCrud.Delete(asapCommNum);
}
	#endregion
#endregion

*/
	}
}