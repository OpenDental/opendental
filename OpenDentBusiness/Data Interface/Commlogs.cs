using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Commlogs {
		#region Get Methods

		///<summary>Returns the list of CommItemTypeAutos. Filters out the IsODHQ-only CommItemTypeAuto when the user is not in HQ.</summary>
		public static List<CommItemTypeAuto> GetCommItemTypes() {
			List<CommItemTypeAuto> listRet=Enum.GetValues(typeof(CommItemTypeAuto)).Cast<CommItemTypeAuto>().ToList();
			listRet.RemoveAll(x => !PrefC.IsODHQ && GenericTools.IsODHQ(x)); //only remove the HQ commlog type(s) if we are not in HQ
			return listRet;
		}

		#endregion

		#region Misc Methods
		///<summary>Returns true if there are any rows that have a Note with char length greater than 65,535</summary>
		public static bool HasAnyLongNotes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM commlog WHERE CHAR_LENGTH(commlog.Note)>65535";
			return (Db.GetCount(command)!="0");
		}

		///<summary>Tailored for OD HQ.  Gets the most recent commlog.CommDateTime for a given patNum of type "support call or chat".
		///Returns DateTime.MinValue if no entry found.</summary>
		public static DateTime GetDateTimeOfLastEntryForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod(),patNum);
			}
			//no need for Oracle compatibility
			string command="SELECT CommDateTime "
				+"FROM commlog "
				+"WHERE PatNum="+POut.Long(patNum)+" "
				+"AND (CommType=292 OR CommType=441) "//support call or chat, DefNums
				+"AND CommSource="+POut.Int((int)CommItemSource.User)+" "
				+"ORDER BY CommDateTime DESC "
				+"LIMIT 1";
			return PIn.DateT(Db.GetScalar(command));
		}
		#endregion

		///<summary>Gets all items for the current patient ordered by date.</summary>
		public static List<Commlog> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Commlog>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM commlog"
				+" WHERE PatNum = '"+patNum+"'"
				+" ORDER BY CommDateTime";
			return Crud.CommlogCrud.SelectMany(command);
		}

		///<summary>Gets one commlog item from database.</summary>
		public static Commlog GetOne(long commlogNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Commlog>(MethodBase.GetCurrentMethod(),commlogNum);
			}
			return Crud.CommlogCrud.SelectOne(commlogNum);
		}

		///<summary>If a commlog exists with today's date for the current user and has no stop time, then that commlog is returned so it can be reopened.  Otherwise, return null.</summary>
		public static Commlog GetIncompleteEntry(long userNum,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Commlog>(MethodBase.GetCurrentMethod(),userNum,patNum);
			}
			//no need for Oracle compatibility
			string command="SELECT * FROM commlog WHERE DATE(CommDateTime)=CURDATE() "
				+"AND UserNum="+POut.Long(userNum)+" "
				+"AND PatNum="+POut.Long(patNum)+" "
				+"AND (CommType=292 OR CommType=441) "//support call or chat, DefNums
				+"AND Mode_="+POut.Int((int)CommItemMode.Phone)+" "//mode=phone
				+"AND DateTimeEnd < '1880-01-01' LIMIT 1";
			return Crud.CommlogCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(Commlog comm) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				comm.CommlogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),comm);
				return comm.CommlogNum;
			}
			return Crud.CommlogCrud.Insert(comm);
		}

		///<summary></summary>
		public static void Update(Commlog comm) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),comm);
				return;
			}
			Crud.CommlogCrud.Update(comm);
		}

		///<summary>Updates only the changed fields (if any).</summary>
		public static bool Update(Commlog comm,Commlog oldCommlog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),comm,oldCommlog);
			}
			return Crud.CommlogCrud.Update(comm,oldCommlog);
		}

		///<summary></summary>
		public static void Delete(Commlog comm) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),comm);
				return;
			}
			string command="SELECT COUNT(*) FROM smsfrommobile WHERE CommlogNum="+POut.Long(comm.CommlogNum);
			if(Db.GetCount(command)!="0") {
				throw new Exception(Lans.g("CommLogs","Not allowed to delete a commlog attached to a text message."));
			}
			Crud.CommlogCrud.Delete(comm.CommlogNum);
		}

		///<summary>Used when printing or emailing recall to make a commlog entry without any display.</summary>
		public static void InsertForRecallOrReactivation(long patNum,CommItemMode _mode,int numberOfReminders,long defNumNewStatus,CommItemTypeAuto type=CommItemTypeAuto.RECALL) {
			//No need to check RemotingRole; no call to db.
			InsertForRecallOrReactivation(patNum,_mode,numberOfReminders,defNumNewStatus,CommItemSource.User,Security.CurUser.UserNum//Recall commlog not associated to the Web Sched app.
				,DateTime.Now,type);
		}

		///<summary>Used when printing or emailing recall to make a commlog entry without any display.  
		///Set commSource to the corresponding entity that is making this recall.  E.g. Web Sched.
		///If the commSource is a 3rd party, set it to ProgramLink and make an overload that accepts the ProgramNum.</summary>
		public static Commlog InsertForRecallOrReactivation(long patNum,CommItemMode _mode,int numberOfReminders,long defNumNewStatus,CommItemSource commSource,long userNum,DateTime dateTimeNow,CommItemTypeAuto type=CommItemTypeAuto.RECALL) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Commlog>(MethodBase.GetCurrentMethod(),patNum,_mode,numberOfReminders,defNumNewStatus,commSource,userNum,
					dateTimeNow,type);
			}
			long commType=Commlogs.GetTypeAuto(type);
			string commTypeStr=type==CommItemTypeAuto.RECALL?"Recall":"Reactivation";
			Commlog com=GetTodayCommlog(patNum,_mode,type);
			if(com!=null) {
				return com;
			}
			com=new Commlog();
			com.PatNum=patNum;
			com.CommDateTime=dateTimeNow;
			com.CommType=commType;
			com.Mode_=_mode;
			com.SentOrReceived=CommSentOrReceived.Sent;
			com.Note="";
			if(numberOfReminders==0){
				com.Note=Lans.g("FormRecallList",$"{commTypeStr} reminder.");
			}
			else if(numberOfReminders==1) {
				com.Note=Lans.g("FormRecallList",$"Second {commTypeStr} reminder.");
			}
			else if(numberOfReminders==2) {
				com.Note=Lans.g("FormRecallList",$"Third {commTypeStr} reminder.");
			}
			else {
				com.Note=Lans.g("FormRecallList",$"{commTypeStr} reminder:")+" "+(numberOfReminders+1).ToString();
			}
			if(defNumNewStatus==0) {
				com.Note+="  "+Lans.g("Commlogs","Status None");
			}
			else {
				com.Note+="  "+Defs.GetName(DefCat.RecallUnschedStatus,defNumNewStatus);
			}
			com.UserNum=userNum;
			com.CommSource=commSource;
			com.CommlogNum=Insert(com);
			EhrMeasureEvent newMeasureEvent=new EhrMeasureEvent();
			newMeasureEvent.DateTEvent=com.CommDateTime;
			newMeasureEvent.EventType=EhrMeasureEventType.ReminderSent;
			newMeasureEvent.PatNum=com.PatNum;
			newMeasureEvent.MoreInfo=com.Note;
			EhrMeasureEvents.Insert(newMeasureEvent);
			return com;
		}

		///<summary>Gets an existing Commlog sent today for patNum, _mode, and type.  Returns null if not found or no defs are setup for type.</summary>
		public static Commlog GetTodayCommlog(long patNum,CommItemMode _mode,CommItemTypeAuto type) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Commlog>(MethodBase.GetCurrentMethod(),patNum,_mode,type);
			}
			long commType=Commlogs.GetTypeAuto(type);
			string command;
			string datesql="CURDATE()";
			if(commType==0) {
				return null;
			}
			command="SELECT * FROM commlog WHERE ";
			command+=DbHelper.DtimeToDate("CommDateTime")+" = "+datesql;
			command+=" AND PatNum="+POut.Long(patNum)+" AND CommType="+POut.Long(commType)
				+" AND Mode_="+POut.Long((int)_mode)
				+" AND SentOrReceived=1";
			List<Commlog> listComms=Crud.CommlogCrud.SelectMany(command).OrderByDescending(x => x.CommDateTime).ToList();
			return listComms.FirstOrDefault();
		}

		///<Summary>Returns a defnum.  If no match, then it returns the first one in the list in that category.
		///If there are no defs in the category, 0 is returned.</Summary>
		public static long GetTypeAuto(CommItemTypeAuto typeauto) {
			//No need to check RemotingRole; no call to db.
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.CommLogTypes);
			Def def=listDefs.FirstOrDefault(x => x.ItemValue==typeauto.ToString());
			if(def!=null) {
				return def.DefNum;
			}
			if(listDefs.Count > 0) {
				return listDefs[0].DefNum;
			}
			return 0;
		}

		public static int GetRecallUndoCount(DateTime date) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),date);
			}
			string command="SELECT COUNT(*) FROM commlog "
				+"WHERE "+DbHelper.DtimeToDate("CommDateTime")+" = "+POut.Date(date)+" "
				+"AND (SELECT ItemValue FROM definition WHERE definition.DefNum=commlog.CommType) ='"+CommItemTypeAuto.RECALL.ToString()+"'";
			return PIn.Int(Db.GetScalar(command));
		}

		public static void RecallUndo(DateTime date) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),date);
				return;
			}
			string command="DELETE FROM commlog "
				+"WHERE "+DbHelper.DtimeToDate("CommDateTime")+" = "+POut.Date(date)+" "
				+"AND (SELECT ItemValue FROM definition WHERE definition.DefNum=commlog.CommType) ='"+CommItemTypeAuto.RECALL.ToString()+"'";
			Db.NonQ(command);
		}

		///<summary>Returns the message used to ask if the user would like to save the appointment/patient note as a commlog when deleting an appointment/patient note.  Only returns up to the first 30 characters of the note.</summary>
		public static string GetDeleteApptCommlogMessage(string noteText,ApptStatus apptStatus) {
			//No need to check RemotingRole; no call to db.
			string commlogMsgText="";
			if(noteText!="") {
				if(apptStatus==ApptStatus.PtNote || apptStatus==ApptStatus.PtNoteCompleted){
					commlogMsgText=Lans.g("Commlogs","Save patient note in CommLog?")+"\r\n"+"\r\n";
				}
				else{
					commlogMsgText=Lans.g("Commlogs","Save appointment note in CommLog?")+"\r\n"+"\r\n";
				}
				//Show up to 30 characters of the note because they can get rather large thus pushing the buttons off the screen.
				commlogMsgText+=noteText.Substring(0,Math.Min(noteText.Length,30));
				commlogMsgText+=(noteText.Length>30)?"...":"";//Append ... to the end of the message so that they know there is more to the note than what is displayed.
			}
			return commlogMsgText;
		}

		///<summary>Gets all commlogs for family that contain a DateTimeEnd entry.  Used internally to keep track of how long calls lasted.</summary>
		public static List<Commlog> GetTimedCommlogsForPat(long guarantor) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Commlog>>(MethodBase.GetCurrentMethod(),guarantor);
			}
			string command="SELECT commlog.* FROM commlog "
				+"INNER JOIN patient ON commlog.PatNum=patient.PatNum AND patient.Guarantor="+POut.Long(guarantor)+" "
				+"WHERE "+DbHelper.Year("commlog.DateTimeEnd")+">1";
			return Crud.CommlogCrud.SelectMany(command);
		}

		///<summary>This returns whether the commlog is an automated type that should be shortened in display.</summary>
		public static bool IsAutomated(string commType,CommItemSource commItemSource) {
			if(!Enum.TryParse(commType,true,out CommItemTypeAuto type)) {
				return false;
			}
			CommItemTypeAuto[] filterTypes=new[] {CommItemTypeAuto.FHIR};
			return commItemSource!=CommItemSource.User || filterTypes.Contains(type);
		}

		///<summary>Returns the first line in a note, trimmed to 38 characters if there is no newline.</summary>
		public static string GetNoteFirstLine(string value) {
			int index=value.IndexOf(Environment.NewLine);
			if(index==-1 || index>38) {//If there is no newline within bounds
				if(value.Length>38) {
					return value.Substring(0,38) + "(...)";
				}
				else {
					return value;
				}
			}
			else {//Trim at the newline.
				return value.Substring(0,index) + "(...)";
			}
		}
	}

	///<summary></summary>
	public enum CommItemTypeAuto {
		///<summary>0</summary>
		[ShortDescription("APPT"),Description("Appointent")]
		APPT,
		///<summary>1</summary>
		[ShortDescription("FIN"),Description("Financial")]
		FIN,
		///<summary>2</summary>
		[ShortDescription("RECALL"),Description("Recall")]
		RECALL,
		///<summary>3</summary>
		[ShortDescription("MISC"),Description("Miscellaneous")]
		MISC,
		///<summary>4</summary>
		[ShortDescription("TEXT"),Description("Text Communication (E-mail, Sms, etc.)")]
		TEXT,
		///<summary>5</summary>
		[ShortDescription("ODHQ"),Description("Open Dental HQ-Generated"),IsODHQ]
		ODHQ,
		///<summary>6</summary>
		[ShortDescription("REACT"),Description("Reactivation")]
		REACT,
		///<summary>6</summary>
		[ShortDescription("FHIR"),Description("FHIR API")]
		FHIR,
	}

}

















