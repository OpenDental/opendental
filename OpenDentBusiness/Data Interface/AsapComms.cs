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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<AsapComm>(MethodBase.GetCurrentMethod(),asapCommNum);
			}
			return Crud.AsapCommCrud.SelectOne(asapCommNum);
		}

		///<summary>Gets a list of all AsapComms matching the passed in parameters.</summary>
		///<param name="listWheres">To get all AsapComms, don't include this parameter.</param>
		private static List<AsapComm> GetMany(List<SQLWhere> listWheres=null) {
			//No need to check RemotingRole; private method.
			string command="SELECT * FROM asapcomm ";
			if(listWheres!=null && listWheres.Count > 0) {
				command+="WHERE "+string.Join(" AND ",listWheres);
			}
			return Crud.AsapCommCrud.SelectMany(command);
		}

		///<summary>Gets a list of all AsapComms for the given patients.</summary>
		public static List<AsapComm> GetForPats(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AsapComm>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			return GetMany(new List<SQLWhere> {
				SQLWhere.CreateIn(nameof(AsapComm.PatNum),listPatNums)
			});
		}

		///<summary>Gets a list of all AsapComms that have not been attempted to send.</summary>
		public static List<AsapComm> GetReadyToSend(DateTime dtNow) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AsapComm>>(MethodBase.GetCurrentMethod(),dtNow);
			}
			//Using a UNION in order to use the indexes on SmsSendStatus and EmailSendStatus.
			string command=@"SELECT * 
				FROM asapcomm 
				WHERE (asapcomm.SmsSendStatus="+POut.Int((int)AutoCommStatus.SendNotAttempted)+@"
				AND asapcomm.DateTimeSmsScheduled<="+POut.DateT(dtNow)+@")
				UNION
				SELECT * 
				FROM asapcomm 				
				WHERE asapcomm.EmailSendStatus="+POut.Int((int)AutoCommStatus.SendNotAttempted);			
			return Crud.AsapCommCrud.SelectMany(command);
		}

		///<summary>Gets a list of AsapComms (along with a few more fields) for use in the Web Sched History window. To view for all patients or clinics, 
		///pass in null for those parameters.</summary>
		public static List<AsapCommHist> GetHist(DateTime dateFrom,DateTime dateTo,List<long> listPatNums=null,List<long> listClinicNums=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			List<AsapCommHist> listHists=Crud.AsapCommCrud.TableToList(table).Select(x => new AsapCommHist() { AsapComm=x }).ToList();
			for(int i=0;i<listHists.Count;i++) {
				listHists[i].PatientName=PIn.String(table.Rows[i]["PatientName"].ToString());
				listHists[i].DateTimeSlotStart=PIn.Date(table.Rows[i]["SchedDate"].ToString()).Add(PIn.Time(table.Rows[i]["StartTime"].ToString()));
				listHists[i].DateTimeSlotEnd=PIn.Date(table.Rows[i]["SchedDate"].ToString()).Add(PIn.Time(table.Rows[i]["StopTime"].ToString()));
				listHists[i].EmailMessageText=PIn.String(table.Rows[i]["EmailMessageText"].ToString());
				listHists[i].SMSMessageText=PIn.String(table.Rows[i]["SMSMessageText"].ToString());
			}
			return listHists;
		}

		#endregion

		#region Insert

		///<summary></summary>
		public static long Insert(AsapComm asapComm) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				asapComm.AsapCommNum=Meth.GetLong(MethodBase.GetCurrentMethod(),asapComm);
				return asapComm.AsapCommNum;
			}
			return Crud.AsapCommCrud.Insert(asapComm);
		}

		///<summary></summary>
		public static void InsertMany(List<AsapComm> listAsapComms) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAsapComms);
				return;
			}
			Crud.AsapCommCrud.InsertMany(listAsapComms);
		}

		///<summary>Inserts these AsapComms into the database. Also creates a block on the schedule recording this communication.</summary>
		public static void InsertForSending(List<AsapComm> listAsapComms,DateTime dtSlotStart,DateTime dtSlotEnd,long opNum) {
			//No need to check RemotingRole; no call to db.
			int textsToBeSent=listAsapComms.Count(x => x.SmsSendStatus!=AutoCommStatus.DoNotSend);
			int emailsToBeSent=listAsapComms.Count(x => x.EmailSendStatus!=AutoCommStatus.DoNotSend);
			//Create a slot on the appointment schedule.
			Schedule sched=new Schedule();
			sched.SchedDate=dtSlotStart.Date;
			sched.SchedType=ScheduleType.WebSchedASAP;
			sched.StartTime=dtSlotStart.TimeOfDay;
			if(dtSlotEnd.Date > dtSlotStart.Date) {
				sched.StopTime=new TimeSpan(23,59,59);//Last second of the day
			}
			else {
				sched.StopTime=dtSlotEnd.TimeOfDay;
			}
			sched.Ops=new List<long> { opNum };
			sched.Note=textsToBeSent+" "+Lans.g("ContrAppt","text"+(textsToBeSent==1 ? "" : "s")+" to be sent")+"\r\n"
				+emailsToBeSent+" "+Lans.g("ContrAppt","email"+(emailsToBeSent==1 ? "" : "s")+" to be sent");
			Schedules.Insert(sched,false);
			listAsapComms.ForEach(x => x.ScheduleNum=sched.ScheduleNum);
			InsertMany(listAsapComms);
		}

		#endregion

		#region Update
		///<summary></summary>
		public static void Update(AsapComm asapComm) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),asapComm);
				return;
			}
			Crud.AsapCommCrud.Update(asapComm);
		}

		public static void Update(AsapComm asapComm,AsapComm asapCommOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),asapComm,asapCommOld);
				return;
			}
			Crud.AsapCommCrud.Update(asapComm,asapCommOld);
		}

		public static void SetRsvpStatus(AsapRSVPStatus rsvpStatus,List<string> listShortGuids) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rsvpStatus,listShortGuids);
				return;
			}
			if(listShortGuids.Count==0) {
				return;
			}
			string command="UPDATE asapcomm SET ResponseStatus="+POut.Int((int)rsvpStatus)
				+" WHERE ShortGUID IN('"+string.Join("','",listShortGuids.Select(x => POut.String(x)))+"')";
			Db.NonQ(command);
		}

		#endregion

		#region Misc Methods

		///<summary>Replaces the template with the passed in arguments.</summary>
		public static string ReplacesTemplateTags(string template,long clinicNum=-1,DateTime dateTime=new DateTime(),string nameF=null,
			string asapUrl=null,bool isHtmlEmail=false) 
		{
			StringBuilder newTemplate=new StringBuilder();
			newTemplate.Append(template);
			//Note: RegReplace is case insensitive by default.
			if(dateTime.Year > 1880) {
				StringTools.RegReplace(newTemplate,"\\[Date]",dateTime.ToString(PrefC.PatientCommunicationDateFormat));
				StringTools.RegReplace(newTemplate,"\\[Time]",dateTime.ToShortTimeString());
			}
			if(clinicNum > -1) {
				Clinic clinic=Clinics.GetClinic(clinicNum)??Clinics.GetPracticeAsClinicZero();
				Clinics.ReplaceOffice(newTemplate,clinic,isHtmlEmail,doReplaceDisclaimer:isHtmlEmail);
			}
			if(nameF!=null) {
				StringTools.RegReplace(newTemplate,"\\[NameF]",nameF);
			}
			if(asapUrl!=null) {
				StringTools.RegReplace(newTemplate,"\\[AsapURL]",asapUrl);
			}
			return newTemplate.ToString();
		}

		///<summary>Creates a list of AsapComms for sending.</summary>
		public static AsapListSender CreateSendList(List<Appointment> listAppts,List<Recall> listRecalls,List<PatComm> listPatComms,SendMode sendMode,
			string textTemplate,string emailTemplate,string emailSubject,DateTime dtSlotStart,DateTime dtStartSend,long clinicNum,bool isRawHtml) 
		{
			//No need to check RemotingRole; no call to db.
			AsapListSender sender=new AsapListSender(sendMode,listPatComms,clinicNum,dtSlotStart,dtStartSend);
			//Order matters here. We will send messages to appointments that are unscheduled first, then scheduled appointments, then recalls. This is
			//because we would prefer to create a brand new appointment than create a hole in the schedule where another appointment was scheduled.
			//We're doing recalls last because cleanings would be lower priority than other types of dental work.
			foreach(Appointment appt in listAppts.OrderBy(x => x.AptStatus!=ApptStatus.UnschedList)
				.ThenBy(x => x.AptStatus!=ApptStatus.Planned)
				.ThenByDescending(x => x.AptDateTime)) 
			{
				AsapComm asapComm=new AsapComm();
				asapComm.DateTimeOrig=appt.AptDateTime;
				asapComm.FKey=appt.AptNum;
				asapComm.ClinicNum=clinicNum;
				asapComm.DateTimeExpire=dtSlotStart.AddDays(7);//Give a 7 day buffer so that the link will still be active a little longer.
				switch(appt.AptStatus) {
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
				if(sender.DoSendText(appt.PatNum,appt.AptNum,asapComm.FKeyType)) {//This will record in the Note why the patient can't be sent a text.
					asapComm.DateTimeSmsScheduled=sender.GetNextTextSendTime();
					asapComm.SmsSendStatus=AutoCommStatus.SendNotAttempted;
					asapComm.TemplateText=textTemplate;
					sender.CountTextsToSend++;
				}
				else {
					asapComm.SmsSendStatus=AutoCommStatus.DoNotSend;
				}
				if(sender.DoSendEmail(appt.PatNum,appt.AptNum,asapComm.FKeyType)) {//This will record in the Note why the patient can't be sent a email.
					asapComm.EmailSendStatus=AutoCommStatus.SendNotAttempted;
					asapComm.TemplateEmail=emailTemplate;
					asapComm.TemplateEmailSubj=emailSubject;
					asapComm.EmailTemplateType=isRawHtml?EmailType.RawHtml:EmailType.Html;
					sender.CountEmailsToSend++;
				}
				else {
					asapComm.EmailSendStatus=AutoCommStatus.DoNotSend;
				}
				asapComm.PatNum=appt.PatNum;
				if(asapComm.SmsSendStatus==AutoCommStatus.DoNotSend && asapComm.EmailSendStatus==AutoCommStatus.DoNotSend) {
					asapComm.ResponseStatus=AsapRSVPStatus.UnableToSend;
				}
				else {
					asapComm.ResponseStatus=AsapRSVPStatus.AwaitingTransmit;
				}
				sender.ListAsapComms.Add(asapComm);
			}
			sender.CopyNotes();
			//Now do recalls
			foreach(Recall recall in listRecalls.OrderByDescending(x => x.DateDue)) {
				AsapComm asapComm=new AsapComm();
				asapComm.DateTimeOrig=recall.DateDue;
				asapComm.FKey=recall.RecallNum;
				asapComm.FKeyType=AsapCommFKeyType.Recall;
				asapComm.ClinicNum=clinicNum;
				asapComm.DateTimeExpire=dtSlotStart.AddDays(7);//Give a 7 day buffer so that the link will still be active a little longer.
				if(sender.DoSendText(recall.PatNum,recall.RecallNum,AsapCommFKeyType.Recall)) {//This will record in the Note why the patient can't be sent a text.
					asapComm.DateTimeSmsScheduled=sender.GetNextTextSendTime();
					asapComm.SmsSendStatus=AutoCommStatus.SendNotAttempted;
					asapComm.TemplateText=textTemplate;
					sender.CountTextsToSend++;
				}
				else {
					asapComm.SmsSendStatus=AutoCommStatus.DoNotSend;
				}
				if(sender.DoSendEmail(recall.PatNum,recall.RecallNum,AsapCommFKeyType.Recall)) {//This will record in the Note why the patient can't be sent a email.
					asapComm.EmailSendStatus=AutoCommStatus.SendNotAttempted;
					asapComm.TemplateEmail=emailTemplate;
					asapComm.TemplateEmailSubj=emailSubject;
					sender.CountEmailsToSend++;
				}
				else {
					asapComm.EmailSendStatus=AutoCommStatus.DoNotSend;
				}
				asapComm.PatNum=recall.PatNum;
				if(asapComm.SmsSendStatus==AutoCommStatus.DoNotSend && asapComm.EmailSendStatus==AutoCommStatus.DoNotSend) {
					asapComm.ResponseStatus=AsapRSVPStatus.UnableToSend;
				}
				else {
					asapComm.ResponseStatus=AsapRSVPStatus.AwaitingTransmit;
				}
				sender.ListAsapComms.Add(asapComm);
			}
			return sender;
		}

		///<summary>Updates the Schedule note with the number of sent and waiting to send AsapComms.</summary>
		public static void UpdateSchedule(long scheduleNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),scheduleNum);
				return;
			}
			List<Schedule> listSchedules=Schedules.GetByScheduleNum(new List<long> { scheduleNum });
			if(listSchedules.Count==0) {
				return;
			}
			Schedule sched=listSchedules[0];
			List<SQLWhere> listWheres=new List<SQLWhere> {
				SQLWhere.Create(nameof(AsapComm.ScheduleNum),ComparisonOperator.Equals,scheduleNum)
			};
			List<AsapComm> listAsapComms=GetMany(listWheres);
			int textsSent=listAsapComms.Count(x => x.SmsSendStatus==AutoCommStatus.SendSuccessful);
			int textsToBeSent=listAsapComms.Count(x => x.SmsSendStatus==AutoCommStatus.SendNotAttempted);
			int emailsSent=listAsapComms.Count(x => x.EmailSendStatus==AutoCommStatus.SendSuccessful);
			int emailsToBeSent=listAsapComms.Count(x => x.EmailSendStatus==AutoCommStatus.SendNotAttempted);
			sched.Note=textsSent+" "+Lans.g("ContrAppt","text"+(textsSent==1 ? "" : "s")+" sent,")+" "
				+textsToBeSent+" "+Lans.g("ContrAppt","text"+(textsToBeSent==1 ? "" : "s")+" to be sent")+"\r\n"
				+emailsSent+" "+Lans.g("ContrAppt","email"+(emailsSent==1 ? "" : "s")+" sent")+" "
				+emailsToBeSent+" "+Lans.g("ContrAppt","email"+(emailsToBeSent==1 ? "" : "s")+" to be sent");
			Schedules.Update(sched);
		}
		
		public static AsapComm GetByShortGuid(string shortGuid) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			public List<PatientDetail> ListDetails {
				get {
					return _dictPatDetails.Values.ToList();
				}
			}
			///<summary>The number of texts that are going to be sent.</summary>
			public int CountTextsToSend { get; internal set; }
			///<summary>The number of emails that are going to be sent.</summary>
			public int CountEmailsToSend { get; internal set; }
			///<summary>The time when texts will start to be sent.</summary>
			public DateTime DtStartSendText { get; private set; }
			///<summary>The time when the emails will be sent.</summary>
			public DateTime DtSendEmail { get; private set; }
			///<summary>The number of minutes that will elapse between texts being sent out.</summary>
			public int MinutesBetweenTexts { get; private set; }
			///<summary>True if it is currently outside the automatic send window.</summary>
			public bool IsOutsideSendWindow=false;
			///<summary>The date time all texts need to be sent by. Based on PrefName.AutomaticCommunicationTimeEnd. May be today or tomorrow.</summary>
			public DateTime DtTextSendEnd;
			private Dictionary<long,PatComm> _dictPatComms;
			private Dictionary<long,PatientDetail> _dictPatDetails;
			///<summary>Key: PatNum, Value: All AsapComms for the patient.</summary>
			private Dictionary<long,List<AsapComm>> _dictPatAsapComms;
			private SendMode _sendMode;
			private int _maxTextsPerDay;
			private DateTime _dtSlotStart;
			private const string _lanThis="FormWebSchedASAPSend";			

			///<summary>Initialize the sender helper for the given PatComms and appointments.</summary>
			///<param name="clinicNum">The clinic that is doing the sending.</param>
			///<param name="dtSlotStart">The date time of the time slot for which this list is being sent.</param>
			///<param name="dtStartSend">The date time when the list should be sent out. This time will be adjusted if necessary.</param>
			internal AsapListSender(SendMode sendMode,List<PatComm> listPatComms,long clinicNum,DateTime dtSlotStart,
				DateTime dtStartSend) 
			{
				_sendMode=sendMode;
				_dictPatComms=listPatComms.GroupBy(x => x.PatNum).ToDictionary(x => x.Key,x => x.First());
				_dictPatDetails=listPatComms.GroupBy(x => x.PatNum).ToDictionary(x => x.Key,x => new PatientDetail(x.First()));
				_dictPatAsapComms=GetForPats(listPatComms.Select(x => x.PatNum).ToList()).GroupBy(x => x.PatNum).ToDictionary(x => x.Key,x => x.ToList());
				TimeSpan timeAutoCommStart=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeStart).TimeOfDay;
				TimeSpan timeAutoCommEnd=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeEnd).TimeOfDay;
				DtSendEmail=dtStartSend;//All emails will be sent immediately.
				DtStartSendText=dtStartSend;
				if(PrefC.DoRestrictAutoSendWindow) {
					//If the time to start sending is before the automatic send window, set the time to start to the beginning of the send window.
					if(DtStartSendText.TimeOfDay < timeAutoCommStart) {
						DtStartSendText=DtStartSendText.Date.Add(timeAutoCommStart);
						IsOutsideSendWindow=true;
					}
					else if(DtStartSendText.TimeOfDay > timeAutoCommEnd) {
						//If the time to start sending is after the automatic send window, set the time to start to the beginning of the send window the next day.
						DtStartSendText=DtStartSendText.Date.AddDays(1).Add(timeAutoCommStart);
						IsOutsideSendWindow=true;
					}
				}
				string maxTextsPrefVal=ClinicPrefs.GetPrefValue(PrefName.WebSchedAsapTextLimit,clinicNum);
				_maxTextsPerDay=String.IsNullOrWhiteSpace(maxTextsPrefVal) ? int.MaxValue : PIn.Int(maxTextsPrefVal); //The pref may be set to blank to have no limit
				DtTextSendEnd=DtStartSendText.Date.Add(timeAutoCommEnd);
				_dtSlotStart=dtSlotStart;
				SetMinutesBetweenTexts(dtSlotStart);
				ListAsapComms=new List<AsapComm>();
			}

			///<summary>Sets the number of minutes between texts.</summary>
			private void SetMinutesBetweenTexts(DateTime dtSlotStart) {
				int hoursUntilSlotStart=(int)(dtSlotStart-DtStartSendText).TotalHours;
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
			internal bool DoSendText(long patNum,long fkey, AsapCommFKeyType fkeyType) {
				PatComm patComm;
				_dictPatComms.TryGetValue(patNum,out patComm);
				if(patComm==null) {
					return false;
				}
				PatientDetail patDetail;
				_dictPatDetails.TryGetValue(patNum,out patDetail);
				if(patDetail==null) {
					patDetail=new PatientDetail {
						PatNum=patNum
					};
					_dictPatDetails[patNum]=patDetail;
				}
				//Local function to evaluate if the patient should be sent a text.
				Func<bool> funcSendText=new Func<bool>(() => {
					if(_sendMode==SendMode.Email) {
						return false;//No need to note the reason.
					}
					if(_dictPatAsapComms.ContainsKey(patNum)) {
						if(_dictPatAsapComms[patNum]
							.Any(x => x.FKey==fkey && x.FKeyType==fkeyType && x.ResponseStatus==AsapRSVPStatus.DeclinedStopComm))
						{
							string text_type=fkeyType==AsapCommFKeyType.Recall ? "recall" : "appointment";
							patDetail.AppendNote(Lans.g(_lanThis,"Not sending text because this patient has requested to not be texted or emailed about this "
								+text_type+"."));
							return false;
						}
						int textsSent=_dictPatAsapComms[patNum]
							.Count(x => (x.SmsSendStatus==AutoCommStatus.SendNotAttempted && x.DateTimeSmsScheduled.Date==DtStartSendText.Date)
							|| (x.SmsSendStatus==AutoCommStatus.SendSuccessful && x.DateTimeSmsSent.Date==DtStartSendText.Date));
						if(textsSent>=_maxTextsPerDay) {
							patDetail.AppendNote(Lans.g(_lanThis,"Not sending text because this patient has received")+" "+_maxTextsPerDay+" "
								+Lans.g(_lanThis,"texts today."));
							return false;
						}
					}
					bool isWithin30Minutes=(GetNextTextSendTime() < _dtSlotStart && (_dtSlotStart-GetNextTextSendTime()).TotalMinutes < TextMinMinutesBefore);
					bool isAfterSlot=(GetNextTextSendTime() > _dtSlotStart);
					if(isWithin30Minutes) {
						patDetail.AppendNote(Lans.g(_lanThis,"Not sending text because the text would be sent less than")+" "+TextMinMinutesBefore+" "
							+Lans.g(_lanThis,"before the time slot."));
						return false;
					}
					if(isAfterSlot) {
						patDetail.AppendNote(Lans.g(_lanThis,"Not sending text because the text would be sent after the time slot."));
						return false;
					}
					if(_sendMode==SendMode.Email) {
						return false;
					}
					if(_sendMode==SendMode.PreferredContact && patComm.PreferContactMethod!=ContactMethod.TextMessage) {
						patDetail.AppendNote(Lans.g(_lanThis,"Not sending text because this patient's preferred contact method is not text message."));
						return false;
					}
					if(!patComm.IsSmsAnOption) {
						patDetail.AppendNote(Lans.g(_lanThis,patComm.GetReasonCantText(CommOptOutType.WebSchedASAP)));
						return false;
					}
					return true;
				});
				bool doSendText=funcSendText();
				patDetail.IsSendingText=doSendText;
				return doSendText;
			}

			///<summary>Returns true if the patient should be sent an email. If false, the reason why the patient can't receive an email is 
			///added to the details dictionary.</summary>
			internal bool DoSendEmail(long patNum,long fkey,AsapCommFKeyType fkeyType) {
				PatComm patComm;
				_dictPatComms.TryGetValue(patNum,out patComm);
				if(patComm==null) {
					return false;
				}
				PatientDetail patDetail;
				_dictPatDetails.TryGetValue(patNum,out patDetail);
				if(patDetail==null) {
					patDetail=new PatientDetail {
						PatNum=patNum
					};
					_dictPatDetails[patNum]=patDetail;
				}
				//Local function to evaluate if the patient should be sent an email.
				Func<bool> funcSendEmail=new Func<bool>(() => {
					if(_sendMode==SendMode.Text) {
						return false;//No need to note the reason.
					}
					if(_dictPatAsapComms.ContainsKey(patNum) && _dictPatAsapComms[patNum]
						.Any(x => x.FKey==fkey && x.FKeyType==fkeyType && x.ResponseStatus==AsapRSVPStatus.DeclinedStopComm))
					{
						string email_type=fkeyType==AsapCommFKeyType.Recall ? "recall" : "appointment";
						patDetail.AppendNote(Lans.g(_lanThis,"Not sending email because this patient has requested to not be texted or emailed about this "
							+email_type+"."));
						return false;
					}
					bool isWithin30Minutes=(DtSendEmail < _dtSlotStart && (_dtSlotStart-DtSendEmail).TotalMinutes < TextMinMinutesBefore);
					bool isAfterSlot=(DtSendEmail > _dtSlotStart);
					if(isWithin30Minutes) {
						patDetail.AppendNote(Lans.g(_lanThis,"Not sending email because the email would be sent less than")+" "+TextMinMinutesBefore+" "
							+Lans.g(_lanThis,"before the time slot."));
						return false;
					}
					if(isAfterSlot) {
						patDetail.AppendNote(Lans.g(_lanThis,"Not sending email because the email would be sent after the time slot."));
						return false;
					}
					if(_sendMode==SendMode.Text) {
						return false;
					}
					if(_sendMode==SendMode.PreferredContact && patComm.PreferContactMethod!=ContactMethod.Email) {
						patDetail.AppendNote(Lans.g(_lanThis,"Not sending email because this patient's preferred contact method is not email."));
						return false;
					}
					if(!patComm.IsEmailAnOption) {
						patDetail.AppendNote(Lans.g(_lanThis,patComm.GetReasonCantEmail(CommOptOutType.WebSchedASAP)));
						return false;
					}
					return true;
				});
				bool doSendEmail=funcSendEmail();
				patDetail.IsSendingEmail=doSendEmail;
				return doSendEmail;
			}

			///<summary>Returns the time when the next text should be sent out.</summary>
			internal DateTime GetNextTextSendTime() {
				DateTime sendTime=DtStartSendText.AddMinutes(MinutesBetweenTexts*CountTextsToSend);
				if(PrefC.DoRestrictAutoSendWindow && sendTime > DtTextSendEnd) {
					sendTime=DtTextSendEnd;
				}
				return sendTime;
			}

			///<summary>Copies the notes from the ListDetails to the actual list of actual AsapComms.</summary>
			internal void CopyNotes() {
				foreach(AsapComm asapComm in ListAsapComms) {
					if(!_dictPatDetails.ContainsKey(asapComm.PatNum)) {
						continue;
					}
					asapComm.Note+=_dictPatDetails[asapComm.PatNum].Note;
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
					PatName=patComm.LName+", "+patComm.GetFirstOrPreferred();
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
			private List<Appointment> _listAppts;
			///<summary>The list of operatories and appointment dates that have been gotten from the database.</summary>
			private List<ODTuple<DateTime,long>> _listDateOps;

			public ApptAvailabilityChecker() {
				_listAppts=new List<Appointment>();
				_listDateOps=new List<ODTuple<DateTime,long>>();
			}

			///<summary>This constructor will store the appointments for the passed in dates and operatories.</summary>
			///<param name="listDateOps">DateTime is the AptDate, long is the OperatoryNum.</param>
			public ApptAvailabilityChecker(List<ODTuple<DateTime,long>> listDateOps) {
				_listDateOps=listDateOps;
				_listAppts=Appointments.GetApptsForDatesOps(listDateOps);
			}

			///<summary>Returns true if the recall will fit in the time slot and there are no other appointments in the slot.</summary>
			public bool IsApptSlotAvailable(Recall recall,long opNum,DateTime slotStart,DateTime slotEnd) {
				int minutes=RecallTypes.GetTimePattern(recall.RecallTypeNum).Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement);
				return IsApptSlotAvailable(minutes,opNum,slotStart,slotEnd);
			}

			///<summary>Returns true if the appointment will fit in the time slot and there are no other appointments in the slot.</summary>
			public bool IsApptSlotAvailable(Appointment appt,long opNum,DateTime slotStart,DateTime slotEnd) {
				return IsApptSlotAvailable(appt.Length,opNum,slotStart,slotEnd);
			}

			///<summary>Returns true if the time length requested will fit in the time slot and there are no other appointments in the slot.</summary>
			public bool IsApptSlotAvailable(int minutes,long opNum,DateTime slotStart,DateTime slotEnd) {
				if(!_listDateOps.Any(x => x.Item1==slotStart.Date && x.Item2==opNum)) {
					_listAppts.AddRange(Appointments.GetApptsForDatesOps(new List<ODTuple<DateTime,long>> { new ODTuple<DateTime,long>(slotStart.Date,opNum) }));
					_listDateOps.Add(new Tuple<DateTime,long>(slotStart.Date,opNum));
				}
				DateTime newSlotEnd=ODMathLib.Min(slotStart.AddMinutes(minutes),slotEnd);
				if(_listAppts.Where(x => x.Op==opNum)
					.Any(x => MiscUtils.DoSlotsOverlap(x.AptDateTime,x.AptDateTime.AddMinutes(x.Length),slotStart,newSlotEnd)))
				{
					return false;
				}
				return true;
			}
		}

		#endregion

		/*
Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<AsapComm> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AsapComm>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM asapcomm WHERE PatNum = "+POut.Long(patNum);
			return Crud.AsapCommCrud.SelectMany(command);
		}

				
	#region Delete
///<summary></summary>
public static void Delete(long asapCommNum) {
	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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