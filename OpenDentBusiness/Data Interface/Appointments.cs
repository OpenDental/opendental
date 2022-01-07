using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.HL7;

namespace OpenDentBusiness{

	///<summary>Appointment S-Class.  Sends signalods for Invalid Appointments whenever a call to the db is made.</summary>
	public class Appointments {
		#region Get Methods
		public static DataTable GetCommTable(string patNum,long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,aptNum);
			}
			DataTable table=new DataTable("Comm");
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("commDateTime",typeof(DateTime));
			table.Columns.Add("CommlogNum");
			table.Columns.Add("CommSource");
			table.Columns.Add("CommType");
			table.Columns.Add("EmailMessageNum");
			table.Columns.Add("Subject");
			table.Columns.Add("Note");
			table.Columns.Add("EmailMessageHideIn");
			string command="SELECT * FROM commlog WHERE PatNum="+patNum//+" AND IsStatementSent=0 "//don't include StatementSent
				+" ORDER BY CommDateTime";
			DataTable rawComm=Db.GetTable(command);
			for(int i=0;i<rawComm.Rows.Count;i++) {
				row=table.NewRow();
				row["commDateTime"]=PIn.DateT(rawComm.Rows[i]["commDateTime"].ToString()).ToShortDateString();
				row["CommlogNum"]=rawComm.Rows[i]["CommlogNum"].ToString();
				row["CommSource"]=rawComm.Rows[i]["CommSource"].ToString();
				row["CommType"]=rawComm.Rows[i]["CommType"].ToString();
				row["Note"]=rawComm.Rows[i]["Note"].ToString();
				row["EmailMessageNum"]=0;
				row["EmailMessageHideIn"]=0;
				table.Rows.Add(row);
			}
			command=@"SELECT emailmessage.MsgDateTime,emailmessage.Subject,emailmessage.EmailMessageNum,emailmessage.HideIn
				FROM emailmessage
				WHERE emailmessage.PatNum="+POut.String(patNum)+@"
				AND emailmessage.AptNum="+POut.Long(aptNum);
			rawComm.Clear();
			rawComm=Db.GetTable(command);
			for(int i=0;i<rawComm.Rows.Count;i++) {
				row=table.NewRow();
				row["commDateTime"]=PIn.DateT(rawComm.Rows[i]["MsgDateTime"].ToString()).ToShortDateString();
				row["EmailMessageNum"]=rawComm.Rows[i]["EmailMessageNum"].ToString();
				row["CommlogNum"]=0;
				row["CommSource"]="";
				row["Subject"]=rawComm.Rows[i]["Subject"].ToString();
				row["EmailMessageHideIn"]=rawComm.Rows[i]["HideIn"].ToString();
				table.Rows.Add(row);
			}
			table.DefaultView.Sort="commDateTime";
			return table.DefaultView.ToTable();
		}

		public static DataTable GetMiscTable(string aptNum,bool isPlanned) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),aptNum,isPlanned);
			}
			DataTable table=new DataTable("Misc");
			DataRow row;
			table.Columns.Add("LabCaseNum");
			table.Columns.Add("labDescript");
			table.Columns.Add("requirements");
			string command="SELECT LabCaseNum,DateTimeDue,DateTimeChecked,DateTimeRecd,DateTimeSent,"
				+"laboratory.Description FROM labcase,laboratory "
				+"WHERE labcase.LaboratoryNum=laboratory.LaboratoryNum AND ";
			if(isPlanned){
				command+="labcase.PlannedAptNum="+aptNum;
			}
			else {
				command+="labcase.AptNum="+aptNum;
			}
			DataTable raw=Db.GetTable(command);
			DateTime date;
			DateTime dateDue;
			//for(int i=0;i<raw.Rows.Count;i++) {//always return one row:
			row=table.NewRow();
			row["LabCaseNum"]="0";
			row["labDescript"]="";
			if(raw.Rows.Count>0){
				row["LabCaseNum"]=raw.Rows[0]["LabCaseNum"].ToString();
				row["labDescript"]=raw.Rows[0]["Description"].ToString();
				date=PIn.DateT(raw.Rows[0]["DateTimeChecked"].ToString());
				if(date.Year>1880){
					row["labDescript"]+=", "+Lans.g("FormApptEdit","Quality Checked");
				}
				else{
					date=PIn.DateT(raw.Rows[0]["DateTimeRecd"].ToString());
					if(date.Year>1880){
						row["labDescript"]+=", "+Lans.g("FormApptEdit","Received");
					}
					else{
						date=PIn.DateT(raw.Rows[0]["DateTimeSent"].ToString());
						if(date.Year>1880){
							row["labDescript"]+=", "+Lans.g("FormApptEdit","Sent");//sent but not received
						}
						else{
							row["labDescript"]+=", "+Lans.g("FormApptEdit","Not Sent");
						}
						dateDue=PIn.DateT(raw.Rows[0]["DateTimeDue"].ToString());
						if(dateDue.Year>1880) {
							row["labDescript"]+=", "+Lans.g("FormAppEdit","Due: ")+dateDue.ToString("ddd")+" "
								+dateDue.ToShortDateString()+" "+dateDue.ToShortTimeString();
						}
					}
				}
			}
			//requirements-------------------------------------------------------------------------------------------
			command="SELECT "
				+"reqstudent.Descript,LName,FName "
				+"FROM reqstudent,provider "//schoolcourse "
				+"WHERE reqstudent.ProvNum=provider.ProvNum "
				+"AND reqstudent.AptNum="+aptNum;
			raw=Db.GetTable(command);
			row["requirements"]="";
			for(int i=0;i<raw.Rows.Count;i++){
				if(i!=0){
					row["requirements"]+="\r\n";
				}
				row["requirements"]+=raw.Rows[i]["LName"].ToString()+", "+raw.Rows[i]["FName"].ToString()
					+": "+raw.Rows[i]["Descript"].ToString();
			}
			table.Rows.Add(row);
			return table;
		}

		///<summary></summary>
		public static List<AppointmentForApi> GetAppointmentsForApi(int limit,int offset,DateTime dateStart,DateTime dateEnd,DateTime dateTStamp,long clinicNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AppointmentForApi>>(MethodBase.GetCurrentMethod(),limit,offset,dateStart,dateEnd,dateTStamp,clinicNum);
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime >= "+POut.DateT(dateStart)+" "
				+"AND AptDateTime < "+POut.DateT(dateEnd)+" "
				+"AND DateTStamp >= "+POut.DateT(dateTStamp)+" ";
			if(clinicNum>-1){
				command+="AND ClinicNum="+POut.Long(clinicNum)+" ";
			}
			command+="ORDER BY AptDateTime,AptNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before appts for rigorous inclusion of appts
			List<Appointment> listAppointments=Crud.AppointmentCrud.TableToList(Db.GetTable(command));
			List<AppointmentForApi> listAppointmentForApis=new List<AppointmentForApi>();
			for(int i=0;i<listAppointments.Count;i++) {//list can be empty
				AppointmentForApi appointmentForApi=new AppointmentForApi();
				appointmentForApi.AppointmentCur=listAppointments[i];
				appointmentForApi.DateTimeServer=dateTimeServer;
				listAppointmentForApis.Add(appointmentForApi);
			}
			return listAppointmentForApis;
		}

		///<summary>Returns a list of appointments that are scheduled between start date and end datetime. 
		///The end of the appointment must also be in the period.</summary>
		public static List<Appointment> GetAppointmentsForPeriod(DateTime start,DateTime end,params ApptStatus[] arrayIgnoreStatuses) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),start,end,arrayIgnoreStatuses);
			}
			//jsalmon - leaving start.Date even though this doesn't make much sense.
			List<Appointment> retVal=GetAppointmentsStartingWithinPeriod(start.Date,end,arrayIgnoreStatuses);
			//Now that we have all appointments that start within our period, make sure that the entire appointment fits within.
			for(int i=retVal.Count-1;i>=0;i--) {
				if(retVal[i].AptDateTime.AddMinutes(retVal[i].Pattern.Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement))>end) {
					retVal.RemoveAt(i);
				}
			}
			return retVal;
		}

		///<summary>Returns a list of appointments that are scheduled between start date and end date.
		///This method only considers the AptDateTime and does not check to see if the appointment </summary>
		public static List<Appointment> GetAppointmentsStartingWithinPeriod(DateTime start,DateTime end,params ApptStatus[] arrayIgnoreStatuses) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),start,end,arrayIgnoreStatuses);
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime >= "+POut.DateT(start)+" "
				+"AND AptDateTime <= "+POut.DateT(end);
				if(arrayIgnoreStatuses.Length > 0) {
					command+="AND AptStatus NOT IN (";
					for(int i=0;i<arrayIgnoreStatuses.Length;i++) {
						if(i > 0) {
							command+=",";
						}
						command+=POut.Int((int)arrayIgnoreStatuses[i]);
					}
					command+=") ";
				}
			List<Appointment> retVal=Crud.AppointmentCrud.TableToList(Db.GetTable(command));
			return retVal;
		}

		///<summary>Gets all appointments scheduled in the operatories passed in that fall within the start and end dates.
		///Does not currently consider the time portion of the DateTimes passed in.</summary>
		public static List<Appointment> GetAppointmentsForOpsByPeriod(List<long> opNums,DateTime dateStart,DateTime dateEnd=new DateTime(),
			Logger.IWriteLine log=null,List<long> listProvNums=null) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),opNums,dateStart,dateEnd,log,listProvNums);
			}
			string command="SELECT * FROM appointment WHERE Op > 0 ";
			if(opNums!=null && opNums.Count > 0) {
				command+="AND Op IN("+String.Join(",",opNums)+") ";
			}
			//It is very important to format these filters as DateT. That will allow the index to be used. 
			//Truncate dateStart/dateEnd down to .Date in order to mimic the behavior of DbHelper.DtimeToDate().
			command+="AND AptStatus!="+POut.Int((int)ApptStatus.UnschedList)+" "
				+"AND AptDateTime>="+POut.DateT(dateStart.Date)+" ";
			if(dateEnd.Year > 1880) {
				command+="AND AptDateTime<="+POut.DateT(dateEnd.Date.AddDays(1))+" ";
			}
			if(listProvNums!=null) {
				List<long> listProvNumsFinal=listProvNums.FindAll(x => x>0);
				if(listProvNumsFinal.Count>0) {
					command+="AND (ProvNum IN ("+String.Join(",",listProvNumsFinal)+") OR ProvHyg IN ("+String.Join(",",listProvNumsFinal)+")) ";
				}
			}
			command+="ORDER BY AptDateTime,Op";//Ordering by AptDateTime then Op is important for speed when checking for collisions in Web Sched.
			log?.WriteLine("command: "+command,LogLevel.Verbose);
			return Crud.AppointmentCrud.SelectMany(command);
		}
		
		///<summary>Gets the appointments for the dates and operatories passed in.</summary>
		///<param name="listDateOps">DateTime is the AptDate, long is the OperatoryNum.</param>
		public static List<Appointment> GetApptsForDatesOps(List<ODTuple<DateTime,long>> listDateOps) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),listDateOps);
			}
			if(listDateOps.Count==0) {
				return new List<Appointment>();
			}
			string command="SELECT * FROM appointment WHERE AptStatus NOT IN("+POut.Int((int)ApptStatus.UnschedList)+","
				+POut.Int((int)ApptStatus.Planned)+") AND "
				+string.Join(" OR ",listDateOps.Select(x => DbHelper.BetweenDates("AptDateTime",x.Item1,x.Item1)+" AND Op="+POut.Long(x.Item2)));
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Returns a list containing every appointment associated to the provided patnum.</summary>
		public static List<Appointment> GetAppointmentsForPat(params long[] arrPatNums) {
			if(arrPatNums.IsNullOrEmpty()) {
				return new List<Appointment>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),arrPatNums);
			}
			string command="SELECT * FROM appointment WHERE PatNum IN("+string.Join(",",arrPatNums.Select(x => POut.Long(x)))+") ORDER BY AptDateTime";
			return Crud.AppointmentCrud.TableToList(Db.GetTable(command));
		}

		///<summary>Returns a dictionary containing the last completed appointment date of each patient.</summary>
		public static SerializableDictionary<long,DateTime> GetDateLastVisit() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,DateTime>(MethodBase.GetCurrentMethod());
			}
			SerializableDictionary<long,DateTime> retVal=new SerializableDictionary<long,DateTime>();
			string command="SELECT PatNum,MAX(AptDateTime) DateLastAppt "
					+"FROM appointment "
					+"WHERE "+DbHelper.DtimeToDate("AptDateTime")+"<="+DbHelper.Curdate()+" "
					+"GROUP BY PatNum";
			DataTable tableLastVisit=Db.GetTable(command);
			for(int i=0;i<tableLastVisit.Rows.Count;i++) {
				long patNum=PIn.Long(tableLastVisit.Rows[i]["PatNum"].ToString());
				DateTime dateLastAppt=PIn.DateT(tableLastVisit.Rows[i]["DateLastAppt"].ToString());
				retVal.Add(patNum,dateLastAppt);
			}
			return retVal;
		}

		///<summary>Returns a dictionary containing all information of every scheduled, completed appointment made from all non-deleted patients.  Usually used for bridges.</summary>
		public static SerializableDictionary<long,List<Appointment>> GetAptsForPats(DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,List<Appointment>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			SerializableDictionary<long,List<Appointment>> retVal=new SerializableDictionary<long,List<Appointment>>();
			string command="SELECT * "
					+"FROM appointment "
					+"WHERE AptStatus IN ("+POut.Int((int)ApptStatus.Scheduled)+","+POut.Int((int)ApptStatus.Complete)+") "
					+"AND "+DbHelper.DtimeToDate("AptDateTime")+">="+POut.Date(dateFrom)+" AND "+DbHelper.DtimeToDate("AptDateTime")+"<="+POut.Date(dateTo);
			List<Appointment> listApts=Crud.AppointmentCrud.SelectMany(command);
			for(int i=0;i<listApts.Count;i++) {
				if(retVal.ContainsKey(listApts[i].PatNum)) {
					retVal[listApts[i].PatNum].Add(listApts[i]);//Add the current appointment to the list of appointments for the patient.
				}
				else {
					retVal.Add(listApts[i].PatNum,new List<Appointment> { listApts[i] });//Initialize the list of appointments for the current patient and include the current appoinment.
				}
			}
			return retVal;
		}

		/// <summary>Get a dictionary of all procedure codes for all scheduled, ASAP, and completed appointments</summary>
		public static SerializableDictionary<long,List<long>> GetCodeNumsAllApts() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,List<long>>(MethodBase.GetCurrentMethod());
			}
			SerializableDictionary<long,List<long>> retVal=new SerializableDictionary<long,List<long>>();
			string command="SELECT appointment.AptNum,procedurelog.CodeNum "
				+"FROM appointment "
				+"LEFT JOIN procedurelog ON procedurelog.AptNum=appointment.AptNum";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				long aptNum=PIn.Long(table.Rows[i]["AptNum"].ToString());
				long codeNum=PIn.Long(table.Rows[i]["CodeNum"].ToString());
				if(retVal.ContainsKey(aptNum)) {
					retVal[aptNum].Add(codeNum);//Add the current CodeNum to the list of CodeNums for the appointment.
				}
				else {
					retVal.Add(aptNum,new List<long> { codeNum });//Initialize the list of CodeNums for the current appointment and include the current CodeNum.
				}
			}
			return retVal;
		}

		///<summary>Gets all appointments associated to the procedures passed in.  Returns an empty list if no procedure is linked to an appt.</summary>
		public static List<Appointment> GetAppointmentsForProcs(List<Procedure> listProcs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),listProcs);
			}
			if(listProcs.Count < 1) {
				return new List<Appointment>();
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptNum IN("+string.Join(",",listProcs.Select(x => x.AptNum).Distinct().ToList())+") "
					+"OR AptNum IN("+string.Join(",",listProcs.Select(x => x.PlannedAptNum).Distinct().ToList())+") "
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Returns a list of appointments that are scheduled (have scheduled or ASAP status) to start between start date and end date.</summary>
		public static List<Appointment> GetSchedApptsForPeriod(DateTime start,DateTime end) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),start,end);
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime >= "+POut.DateT(start)+" "
				+"AND AptDateTime <= "+POut.DateT(end)+" "
				+"AND AptStatus="+POut.Int((int)ApptStatus.Scheduled);
			List<Appointment> retVal=Crud.AppointmentCrud.TableToList(Db.GetTable(command));
			return retVal;
		}

		///<summary>Uses the input parameters to construct a List&lt;ApptSearchProviderSchedule&gt;. It is written to reduce the number of queries to the database.</summary>
		/// <param name="listProvNums">PrimaryKeys to Provider.</param>
		/// <param name="dateScheduleStart">The date that will start looking for provider schedule information.</param>
		/// <param name="dateScheduleStop">The date that will stop looking for provider schedule information.</param>
		/// <param name="listSchedules">A List of Schedules containing all of the schedules for the given day, or possibly more. 
		/// Intended to be all schedules between search start date and search start date plus 2 years. This is to reduce queries to DB.</param>
		/// <param name="listAppointments">A List of Appointments containing all of the schedules for the given day, or possibly more. 
		/// Intended to be all Appointments between search start date and search start date plus 2 years. This is to reduce queries to DB.</param>
		public static Dictionary<DateTime,List<ApptSearchProviderSchedule>> GetApptSearchProviderScheduleForProvidersAndDate(List<long> listProvNums
			,DateTime dateScheduleStart,DateTime dateScheduleStop,List<Schedule> listSchedules,List<Appointment> listAppointments)
		{//Not working properly when scheduled but no ops are set.
			//No need to check RemotingRole; no call to db.
			Dictionary<DateTime,List<ApptSearchProviderSchedule>> dictProviderSchedulesByDate=new Dictionary<DateTime,List<ApptSearchProviderSchedule>>();
			List<ApptSearchProviderSchedule> listProviderSchedules=new List<ApptSearchProviderSchedule>();
			if(dateScheduleStart.Date>=dateScheduleStop.Date) {
				listProviderSchedules=GetProviderScheduleForProvidersAndDate(listProvNums,dateScheduleStart.Date,listSchedules,listAppointments);
				dictProviderSchedulesByDate.Add(dateScheduleStart.Date,listProviderSchedules);
				return dictProviderSchedulesByDate;
			}
			//Loop through all the days between the start and stop date and return the ApptSearchProviderSchedule's for all days.
			for(int i=0;i<=(dateScheduleStop.Date-dateScheduleStart.Date).Days;i++) {
				listProviderSchedules=GetProviderScheduleForProvidersAndDate(listProvNums,dateScheduleStart.Date.AddDays(i),listSchedules,listAppointments);
				if(dictProviderSchedulesByDate.ContainsKey(dateScheduleStart.Date.AddDays(i))) {//Just in case.
					dictProviderSchedulesByDate[dateScheduleStart.Date.AddDays(i)]=listProviderSchedules;
				}
				else {
					dictProviderSchedulesByDate.Add(dateScheduleStart.Date.AddDays(i),listProviderSchedules);
				}
			}
			return dictProviderSchedulesByDate;
		}

		///<summary>Uses the input parameters to construct a List&lt;ApptSearchProviderSchedule&gt;.Written to reduce the number of queries to the database.</summary>
		/// <param name="listProvNums">PrimaryKeys to Provider.</param>
		/// <param name="schedDateEvaluating">The date to construct the schedule for.</param>
		/// <param name="listSchedules">A List of Schedules containing all of the schedules for the given day, or possibly more. 
		/// Intended to be all schedules between search start date and search start date plus 2 years. This is to reduce queries to DB.</param>
		/// <param name="listAppointments">A List of Appointments containing all of the schedules for the given day, or possibly more. 
		/// Intended to be all Appointments between search start date and search start date plus 2 years. This is to reduce queries to DB.</param>
		//js 2/2021, I disagree that 2 years of appointments is efficient, but will research later. Their assumption seems to be that there is not a tighter search range.
		public static List<ApptSearchProviderSchedule> GetProviderScheduleForProvidersAndDate(List<long> listProvNums
			,DateTime schedDateEvaluating,List<Schedule> listSchedules,List<Appointment> listAppointments) 
		{
			//No need to check RemotingRole; no call to db.
			List<ApptSearchProviderSchedule> retVal=new List<ApptSearchProviderSchedule>();
			List<Def> listBlockoutDefsAll=Defs.GetDefsForCategory(DefCat.BlockoutTypes,true);
			List<long> listBlockoutTypesDoNotSchedule=new List<long>();
			foreach(Def blockout in listBlockoutDefsAll) {
				if(blockout.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription())) {
					listBlockoutTypesDoNotSchedule.Add(blockout.DefNum);//do not return results for blockouts set to 'Do Not Schedule'
					continue;
				}
			}
			//Make a shallow copy of the list of schedules passed in so that we can order them in a unique fashion without affecting calling methods.
			List<Schedule> listSchedulesOrdered=listSchedules.OrderByDescending(x => ListTools.In(x.BlockoutType,listBlockoutTypesDoNotSchedule))
				.ThenBy(x => x.BlockoutType > 0).ToList();
			for(int i=0;i<listProvNums.Count;i++) {
				retVal.Add(new ApptSearchProviderSchedule());
				retVal[i].ProviderNum=listProvNums[i];
				retVal[i].SchedDate=schedDateEvaluating;
			}
			foreach(Schedule schedule in listSchedulesOrdered) {
				if(schedule.SchedDate.Date!=schedDateEvaluating) {//ignore schedules for different dates.
					continue;
				}
				if(listProvNums.Contains(schedule.ProvNum)) {//schedule applies to one of the selected providers
					int indexOfProvider = listProvNums.IndexOf(schedule.ProvNum);//cache the provider index
					int scheduleStartBlock = (int)schedule.StartTime.TotalMinutes/5;//cache the start time of the schedule
					int scheduleLengthInBlocks = (int)(schedule.StopTime-schedule.StartTime).TotalMinutes/5;//cache the length of the schedule
					for(int i=0;i<scheduleLengthInBlocks;i++) {
						if(schedule.BlockoutType > 0 && ListTools.In(schedule.BlockoutType,listBlockoutTypesDoNotSchedule)) {
							retVal[indexOfProvider].IsProvAvailable[scheduleStartBlock+i]=false;
							retVal[indexOfProvider].IsProvScheduled[scheduleStartBlock+i]=false;
						}
						else {
							retVal[indexOfProvider].IsProvAvailable[scheduleStartBlock+i]=true;//provider may have an appointment here
							retVal[indexOfProvider].IsProvScheduled[scheduleStartBlock+i]=true;//provider is scheduled today
						}
					}
				}
			}
			int numBlocksInDay=60*24/5;//Number of five minute increments in a day. Matches the length of the IsProvAvailableArray
			foreach(Appointment appointment in listAppointments) {
				if(appointment.AptDateTime.Date!=schedDateEvaluating) {
					continue;
				}
				if(!appointment.IsHygiene && listProvNums.Contains(appointment.ProvNum)) {//Not hygiene Modify provider bar based on ProvNum
					int indexOfProvider = listProvNums.IndexOf(appointment.ProvNum);
					int appointmentCurStartBlock = (int)appointment.AptDateTime.TimeOfDay.TotalMinutes/5;
					for(int i=0;i<appointment.Pattern.Length;i++) {
						if(appointment.Pattern[i]=='X') {
							if(appointmentCurStartBlock+i>=numBlocksInDay) {//if the appointment is scheduled over a day, prevents the search from breaking
								break;
							}
							retVal[indexOfProvider].IsProvAvailable[appointmentCurStartBlock+i]=false;
						}
					}
				}
				else if(appointment.IsHygiene && listProvNums.Contains(appointment.ProvHyg)) {//Modify provider bar based on ProvHyg
					int indexOfProvider = listProvNums.IndexOf(appointment.ProvHyg);
					int appointmentStartBlock = (int)appointment.AptDateTime.TimeOfDay.TotalMinutes/5;
					for(int i=0;i<appointment.Pattern.Length;i++) {
						if(appointment.Pattern[i]=='X') {
							if(appointmentStartBlock+i>=numBlocksInDay) {//if the appointment is scheduled over a day, prevents the search from breaking
								break;
							}
							retVal[indexOfProvider].IsProvAvailable[appointmentStartBlock+i]=false;
						}
					}
				}
			}
			return retVal;
		}

		///<summary>Gets the ProvNum for the last completed or scheduled appointment for a patient. If none, returns 0.</summary>
		public static long GetProvNumFromLastApptForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT ProvNum FROM appointment WHERE AptStatus IN ("+(int)ApptStatus.Complete+","+(int)ApptStatus.Scheduled+")"
				+" AND AptDateTime<="+POut.DateT(System.DateTime.Now)
				+" AND PatNum="+POut.Long(patNum)
				+" ORDER BY AptDateTime DESC LIMIT 1";
			string result=Db.GetScalar(command);
			if(String.IsNullOrWhiteSpace(result)) {
				return 0;
			}
			return PIn.Long(result);
		}

		///<summary>Gets the appt confirmation status for a single appt.</summary>
		public static long GetApptConfirmationStatus(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT Confirmed FROM appointment WHERE AptNum="+POut.Long(aptNum);
			return PIn.Long(Db.GetScalar(command));
		}
		
		///<summary></summary>
		public static DataSet GetApptEdit(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),aptNum);
			}
			DataSet retVal=new DataSet();
			retVal.Tables.Add(GetApptTable(aptNum));
			retVal.Tables.Add(GetPatTable(retVal.Tables["Appointment"].Rows[0]["PatNum"].ToString()
				,Appointments.GetOneApt(PIn.Long(retVal.Tables["Appointment"].Rows[0]["AptNum"].ToString()))));
			retVal.Tables.Add(GetProcTable(retVal.Tables["Appointment"].Rows[0]["PatNum"].ToString(),aptNum.ToString(),
				retVal.Tables["Appointment"].Rows[0]["AptStatus"].ToString(),
				retVal.Tables["Appointment"].Rows[0]["AptDateTime"].ToString()
				));
			retVal.Tables.Add(GetCommTable(retVal.Tables["Appointment"].Rows[0]["PatNum"].ToString(),aptNum));
			bool isPlanned=false;
			if(retVal.Tables["Appointment"].Rows[0]["AptStatus"].ToString()=="6"){
				isPlanned=true;
			}
			retVal.Tables.Add(GetMiscTable(aptNum.ToString(),isPlanned));
			retVal.Tables.Add(GetApptFields(aptNum));
			return retVal;
		}

		///<summary></summary>
		public static DataSet RefreshOneApt(long aptNum,bool isPlanned,List<long> listOpNums=null,List<long> listProvNums=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),aptNum,isPlanned,listOpNums,listProvNums);
			} 
			DataSet retVal=new DataSet();
			retVal.Tables.Add(GetPeriodApptsTable(DateTime.MinValue,DateTime.MinValue,aptNum,isPlanned,listOpNums,listProvNums));
			return retVal;
		}

		///<summary>If aptnum is specified, then the dates are ignored in the Appointment table queries; they are still used in the Adjustment table queries.  If getting data for one planned appt, then pass isPlanned=1.  The times of the dateStart and dateEnd are ignored.  This changes which procedures are retrieved.  Any ApptNums within listPinApptNums will get forcefully added to the DataTable.  Since dateStart and dateEnd are used to calcualte adjustment totals, when in 'Day' view mode, dateStart should be set to the beginning of the day and dateEnd set to the end of the day.  Set "doRunQueryOnNoOps" to false if an empty DataTable should be returned when listOpNums is null or empty.</summary>
		public static DataTable GetPeriodApptsTable(DateTime dateStart,DateTime dateEnd,long aptNum,bool isPlanned,List<long> listPinApptNums=null,
			List<long> listOpNums=null,List<long> listProvNums=null,bool doRunQueryOnNoOps=true,bool includeVerifyIns=false) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,aptNum,isPlanned,listPinApptNums,listOpNums,listProvNums,doRunQueryOnNoOps,includeVerifyIns);
			} 
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("Appointments");
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("adjustmentTotal");
			table.Columns.Add("age");
			table.Columns.Add("address");
			table.Columns.Add("addrNote");
			table.Columns.Add("apptModNote");
			table.Columns.Add("AppointmentTypeNum");
			table.Columns.Add("aptDate");
			table.Columns.Add("aptDay");
			table.Columns.Add("aptLength");
			table.Columns.Add("aptTime");
			//DataTables are documented as not being thread-safe for write operations.  Even with locking the creation/addition of each row, occasionally
			//data is not correctly written to the DataRow, most notably when converting a DateTime to a string (previous behavior).  Setting the type on 
			//these two columns greately reduces the frequency that these columns are not set correctly if there is a collision when each row is built 
			//asynchronously.  
			table.Columns.Add("AptDateTime",typeof(DateTime));
			table.Columns.Add("AptDateTimeArrived",typeof(DateTime));
			table.Columns.Add("AptNum");
			table.Columns.Add("AptStatus");
			table.Columns.Add("Priority");
			table.Columns.Add("Assistant");
			table.Columns.Add("assistantAbbr");
			table.Columns.Add("billingType");
			table.Columns.Add("Birthdate");
			table.Columns.Add("chartNumber");
			table.Columns.Add("chartNumAndName");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("ColorOverride");
			table.Columns.Add("confirmed");
			table.Columns.Add("Confirmed");
			table.Columns.Add("contactMethods");
			//table.Columns.Add("creditIns");
			table.Columns.Add("CreditType");
			table.Columns.Add("discountPlan");
			table.Columns.Add("Email");
			table.Columns.Add("estPatientPortion");
			table.Columns.Add("estPatientPortionRaw");
			table.Columns.Add("famFinUrgNote");
			table.Columns.Add("guardians");
			table.Columns.Add("hasDiscount[D]");
			table.Columns.Add("hasIns[I]");
			table.Columns.Add("hmPhone");
			table.Columns.Add("ImageFolder");
			table.Columns.Add("insurance");
			table.Columns.Add("insColor1");
			table.Columns.Add("insColor2");
			table.Columns.Add("insToSend[!]");
			table.Columns.Add("IsHygiene");
			table.Columns.Add("lab");
			table.Columns.Add("language");
			table.Columns.Add("medOrPremed[+]");
			table.Columns.Add("MedUrgNote");
			table.Columns.Add("netProduction");
			table.Columns.Add("netProductionVal");
			table.Columns.Add("Note");
			table.Columns.Add("Op");
			table.Columns.Add("patientName");
			table.Columns.Add("patientNameF");
			table.Columns.Add("patientNamePref");
			table.Columns.Add("PatNum");
			table.Columns.Add("patNum");
			table.Columns.Add("GuarNum");
			table.Columns.Add("patNumAndName");
			table.Columns.Add("Pattern");
			table.Columns.Add("PatternSecondary");
			table.Columns.Add("preMedFlag");
			table.Columns.Add("procs");
			table.Columns.Add("procsColored");
			table.Columns.Add("prophy/PerioPastDue[P]");
			table.Columns.Add("production");
			table.Columns.Add("productionVal");
			table.Columns.Add("ProvBarText");
			table.Columns.Add("provider");
			table.Columns.Add("ProvHyg");
			table.Columns.Add("ProvNum");
			table.Columns.Add("recallPastDue[R]");
			table.Columns.Add("referralFrom");
			table.Columns.Add("referralTo");
			table.Columns.Add("referralFromWithPhone");
			table.Columns.Add("referralToWithPhone");
			table.Columns.Add("timeAskedToArrive");
			table.Columns.Add("verifyIns[V]");
			table.Columns.Add("wkPhone");
			table.Columns.Add("wirelessPhone");
			table.Columns.Add("writeoffPPO");
			if(!doRunQueryOnNoOps && (listOpNums==null || listOpNums.Count==0)) {
				//If no operatories are defined in an appointment view, the query below will select all appointments in the date range, regardless of clinic,
				//operatory, etc.  This is particularly problematic for large organizations that may have thousands of appointments each day.  Since no ops
				//are defined in this appointment view, it makes sense to return an empty DataTable as no appointments should ever be returned for a view 
				//without ops.
				return table;
			}
			//QUERY 1: tableRaw: joins appointment, patient=======================================================================================================
			string command="SELECT DISTINCT patient.Address patAddress1,patient.Address2 patAddress2,patient.AddrNote patAddrNote,"
				+"patient.ApptModNote patApptModNote,appointment.AppointmentTypeNum,appointment.AptDateTime apptAptDateTime,appointment.DateTimeArrived apptAptDateTimeArrived,appointment.AptNum apptAptNum,"
				+"appointment.AptStatus apptAptStatus,appointment.Priority,appointment.Assistant apptAssistant,"
				+"patient.BillingType patBillingType,patient.BirthDate patBirthDate,patient.DateTimeDeceased patDateTimeDeceased,"
				+"appointment.InsPlan1,appointment.InsPlan2,appointment.ClinicNum,"
				+"patient.ChartNumber patChartNumber,patient.City patCity,appointment.ColorOverride apptColorOverride,appointment.Confirmed apptConfirmed,"
				+"patient.CreditType patCreditType,labcase.DateTimeChecked labcaseDateTimeChecked,"
				+"labcase.DateTimeDue labcaseDateTimeDue,labcase.DateTimeRecd labcaseDateTimeRecd,labcase.DateTimeSent labcaseDateTimeSent,appointment.DateTimeAskedToArrive apptDateTimeAskedToArrive,"
				+"patient.Email patEmail,guar.FamFinUrgNote guarFamFinUrgNote,patient.FName patFName,patient.Guarantor patGuarantor,"
				+"patient.HmPhone patHmPhone,patient.ImageFolder patImageFolder,appointment.IsHygiene apptIsHygiene,appointment.IsNewPatient apptIsNewPatient,"
				+"labcase.LabCaseNum labcaseLabCaseNum,patient.Language patLanguage,patient.LName patLName,patient.MedUrgNote patMedUrgNote,"
				+"patient.MiddleI patMiddleI,appointment.Note apptNote,appointment.Op apptOp,appointment.PatNum apptPatNum,"
				+"appointment.Pattern apptPattern, appointment.PatternSecondary apptPatternSecondary," 
				+"(CASE WHEN patplan.InsSubNum IS NULL THEN 0 ELSE 1 END) hasIns,patient.PreferConfirmMethod patPreferConfirmMethod,"
				+"patient.PreferContactMethod patPreferContactMethod,patient.Preferred patPreferred,"
				+"patient.PreferRecallMethod patPreferRecallMethod,patient.Premed patPremed,"
				+"appointment.ProcDescript apptProcDescript,appointment.ProcsColored apptProcsColored,appointment.ProvBarText,"
				+"appointment.ProvHyg apptProvHyg,appointment.ProvNum apptProvNum,"
				+"patient.State patState,patient.WirelessPhone patWirelessPhone,patient.WkPhone patWkPhone,patient.Zip patZip,"
				+"(CASE WHEN discountplansub.DiscountPlanNum IS NULL THEN 0 ELSE discountplansub.DiscountPlanNum END) discountPlan,"
				+"(CASE WHEN discountplansub.DiscountPlanNum IS NULL THEN 0 ELSE 1 END) hasDiscount "
				+"FROM appointment "
				+"LEFT JOIN patient ON patient.PatNum=appointment.PatNum ";
			if(isPlanned){
				command+="LEFT JOIN labcase ON labcase.PlannedAptNum=appointment.AptNum AND labcase.PlannedAptNum!=0 ";
			}
			else{
				command+="LEFT JOIN labcase ON labcase.AptNum=appointment.AptNum AND labcase.AptNum!=0 ";
			}
			command+="LEFT JOIN patient guar ON guar.PatNum=patient.Guarantor "
				+"LEFT JOIN patplan ON patplan.PatNum=patient.PatNum AND patplan.Ordinal=1 "
				+"LEFT JOIN discountplansub ON discountplansub.PatNum=patient.PatNum ";
			if(aptNum > 0) {//Only get information regarding this one appointment passed in.
				command+="WHERE appointment.AptNum="+POut.Long(aptNum);
			}
			else {//Get all information for the appointments for the date range and any appointments on the pinboard.
				command+="WHERE ((AptDateTime >= "+POut.Date(dateStart)+" "
					+"AND AptDateTime < "+POut.Date(dateEnd.AddDays(1))+" "
					+ "AND AptStatus IN ("+POut.Int((int)ApptStatus.Scheduled)
						+", "+POut.Int((int)ApptStatus.Complete)
						+", "+POut.Int((int)ApptStatus.Broken)
						+", "+POut.Int((int)ApptStatus.PtNote)
						+", "+POut.Int((int)ApptStatus.PtNoteCompleted)+")";
				if((listOpNums!=null && listOpNums.Count>0)
					&& (listProvNums!=null && listProvNums.Count>0)) {
					command+=" AND ("
							+"appointment.Op IN ("+String.Join(",",listOpNums)+") "
							+"OR (appointment.ProvNum IN ("+String.Join(",",listProvNums)+") OR appointment.ProvHyg IN ("+String.Join(",",listProvNums)+"))"
						+")";
				}
				else if(listOpNums!=null && listOpNums.Count>0) {
					command+=" AND appointment.Op IN ("+String.Join(",",listOpNums)+")";
				}
				else if(listProvNums!=null && listProvNums.Count>0) {
					command+=" AND (appointment.ProvNum IN ("+String.Join(",",listProvNums)+") OR appointment.ProvHyg IN ("+String.Join(",",listProvNums)+"))";
				}
				command+=")";
				if(listPinApptNums!=null && listPinApptNums.Count>0) {
					command+="OR appointment.AptNum IN ("+String.Join(",",listPinApptNums)+")";
				}
				command+=")";
			}
			DataTable tableRaw=dcon.GetTable(command);
			//rawProc table was historically used for other purposes.  It is currently only used for production--------------------------
			//rawProcLab table is only used for Canada and goes hand in hand with the rawProc table, also only used for production.
			DataTable tableRawProc;
			DataTable rawProcLab=null;
			if(tableRaw.Rows.Count==0){
				tableRawProc=new DataTable();
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					rawProcLab=new DataTable();
				}
			}
			else{
				//QUERY 2: tableRawProc: joins procedurelog, claimproc=========================================================================================================
				command="SELECT AptNum,PlannedAptNum,"
					+"ProcFee*(UnitQty+BaseUnits) ProcFee,procedurelog.CodeNum,procedurelog.ClinicNum,procedurelog.ProvNum,"
					+"SUM(CASE WHEN claimproc.Status IN("+POut.Int((int)ClaimProcStatus.CapComplete)+","+POut.Int((int)ClaimProcStatus.CapEstimate)+") THEN 0 "
					+"WHEN claimproc.ClaimNum>0 THEN claimproc.WriteOff "
					+"WHEN WriteOffEstOverride!=-1 THEN WriteOffEstOverride "
					+"WHEN WriteOffEst!=-1 THEN WriteOffEst "
					+"ELSE 0 END) writeoffPPO,"
					+"SUM(CASE WHEN claimproc.Status NOT IN("+POut.Int((int)ClaimProcStatus.CapComplete)+","
						+POut.Int((int)ClaimProcStatus.CapEstimate)+") THEN 0 "
					+"WHEN claimproc.ClaimNum>0 THEN claimproc.WriteOff "
					+"WHEN WriteOffEstOverride!=-1 THEN WriteOffEstOverride "
					+"WHEN WriteOffEst!=-1 THEN WriteOffEst "
					+"ELSE 0 END) writeoffCap,"
					+"SUM(CASE WHEN claimproc.Status IN( "+POut.Int((int)ClaimProcStatus.NotReceived)+","+POut.Int((int)ClaimProcStatus.Estimate)+") THEN claimproc.InsPayEst "
					+"WHEN claimproc.Status IN("+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+") "
					+"THEN claimproc.InsPayAmt "
					+"ELSE 0 END) AS insAmt, "
					+"procedurelog.ProcNum,procedurelog.ProcStatus,procedurelog.Discount,procedurelog.DiscountPlanAmt "
					+"FROM procedurelog "
					+"LEFT JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum "
					+"AND claimproc.Status NOT IN ("+(int)ClaimProcStatus.CapClaim +","+POut.Int((int)ClaimProcStatus.Preauth)+") "
					+"WHERE ProcNumLab=0 AND ";
				if(isPlanned) {
					command+="PlannedAptNum!=0 AND PlannedAptNum ";
				} 
				else {
					command+="AptNum!=0 AND AptNum ";
				}
				command+="IN(";//this was far too slow:SELECT a.AptNum FROM appointment a WHERE ";
				if(aptNum==0) {
					for(int a=0;a<tableRaw.Rows.Count;a++){
						if(a>0){
							command+=",";
						}
						command+=tableRaw.Rows[a]["apptAptNum"].ToString();
					}
				}
				else {
					command+=POut.Long(aptNum);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command+=") GROUP BY procedurelog.ProcNum";
				}
				else {//Oracle
					command+=") GROUP BY procedurelog.ProcNum,AptNum,PlannedAptNum,ProcFee";
				}
				tableRawProc=dcon.GetTable(command);
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && tableRawProc.Rows.Count>0) {//Canadian. en-CA or fr-CA
					command="SELECT procedurelog.ProcNum,ProcNumLab,ProcFee*(UnitQty+BaseUnits) ProcFee,"
						+"SUM(CASE WHEN claimproc.Status IN("+POut.Int((int)ClaimProcStatus.CapComplete)+","
							+POut.Int((int)ClaimProcStatus.CapEstimate)+") THEN 0 "
						+"WHEN WriteOffEstOverride!=-1 THEN WriteOffEstOverride "
						+"WHEN WriteOffEst!=-1 THEN WriteOffEst "
						+"ELSE 0 END) writeoffPPO,"
						+"SUM(CASE WHEN claimproc.Status NOT IN("+POut.Int((int)ClaimProcStatus.CapComplete)+","
							+POut.Int((int)ClaimProcStatus.CapEstimate)+") THEN 0 "
						+"WHEN WriteOffEstOverride!=-1 THEN WriteOffEstOverride "
						+"WHEN WriteOffEst!=-1 THEN WriteOffEst "
						+"ELSE 0 END) writeoffCap, "						
						+"SUM(CASE WHEN claimproc.Status IN( "+POut.Int((int)ClaimProcStatus.NotReceived)+","+POut.Int((int)ClaimProcStatus.Estimate)+") THEN claimproc.InsPayEst "
						+"WHEN claimproc.Status IN("+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+") "
						+"THEN claimproc.InsPayAmt "
						+"ELSE 0 END) AS insAmt "
						+"FROM procedurelog "
						+"LEFT JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum "
						+"WHERE ProcStatus != "+POut.Int((int)ProcStat.D)+" "
						+"AND ProcNumLab IN (";
					for(int i=0;i<tableRawProc.Rows.Count;i++) {
						if(i>0) {
							command+=",";
						}
						command+=tableRawProc.Rows[i]["ProcNum"].ToString();
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command+=") GROUP BY procedurelog.ProcNum";
					}
					else {//Oracle
						command+=") GROUP BY procedurelog.ProcNum,ProcNumLab,ProcFee";
					}
					rawProcLab=dcon.GetTable(command);
				}
			}
			List<long> listPatNums=new List<long>();
			List<long> listPlanNums=new List<long>();
			List<long> listGuarantorsWithIns=new List<long>();
			foreach(DataRow rowRaw in tableRaw.Rows) {
				listPatNums.Add(PIn.Long(rowRaw["apptPatNum"].ToString()));
				listPlanNums.Add(PIn.Long(rowRaw["InsPlan1"].ToString()));
				listPlanNums.Add(PIn.Long(rowRaw["InsPlan2"].ToString()));
				if(rowRaw["hasIns"].ToString()!="0") {
					listGuarantorsWithIns.Add(PIn.Long(rowRaw["patGuarantor"].ToString()));
				}
			}
			listPatNums=listPatNums.Distinct().ToList();
			listPlanNums=listPlanNums.FindAll(x => x > 0).Distinct().ToList();//remove 0 from pats without ins.
			listGuarantorsWithIns=listGuarantorsWithIns.Distinct().ToList();
			//QUERY 3: tableRawInsProc: joins patient, procedurelog, claimproc=============================================================================================
			//rawInsProc table is usually skipped. Too slow------------------------------------------------------------------------------
			DataTable tableRawInsProc=null;
			if(PrefC.GetBool(PrefName.ApptExclamationShowForUnsentIns) && listGuarantorsWithIns.Count>0) {
				//procs for flag, InsNotSent
				command ="SELECT patient.PatNum, patient.Guarantor "
					+"FROM patient,procedurelog,claimproc "
					+"WHERE claimproc.procnum=procedurelog.procnum "
					+"AND patient.PatNum=procedurelog.PatNum "
					+"AND claimproc.NoBillIns=0 "
					+"AND procedurelog.ProcFee>0 "
					+"AND claimproc.Status=6 "//estimate
					+"AND patient.Guarantor IN ("+string.Join(",",listGuarantorsWithIns)+") " //reduced runtime from 24sec / 0.9sec to 14sec/0.04sec uncached and cached respectively
					+"AND ((CASE WHEN claimproc.InsEstTotalOverride>-1 THEN claimproc.InsEstTotalOverride ELSE claimproc.InsEstTotal END) > 0) "
					+"AND procedurelog.procstatus=2 "
					+"AND procedurelog.ProcDate >= "+POut.Date(DateTime.Now.AddYears(-1))+" "//I'm sure this is the slow part.  Should be easy to make faster with less range
					+"AND procedurelog.ProcDate <= "+POut.Date(DateTime.Now)+ " "
					+"GROUP BY patient.PatNum, patient.Guarantor"; 
				tableRawInsProc=dcon.GetTable(command);
			}
			//Guardians-------------------------------------------------------------------------------------------------------------------
			//QUERY 4: tableRawGuardians: joins guardian, patient ====================================================================================================
			command="SELECT PatNumChild,PatNumGuardian,Relationship,patient.FName,patient.Preferred "
				+"FROM guardian "
				+"LEFT JOIN patient ON patient.PatNum=guardian.PatNumGuardian "
				+"WHERE IsGuardian<>0 AND PatNumChild IN (";
			if(tableRaw.Rows.Count==0){
				command+="0";
			}
			else for(int i=0;i<tableRaw.Rows.Count;i++) {
				if(i>0) {
					command+=",";
				}
				command+=tableRaw.Rows[i]["apptPatNum"].ToString();
			}
			command+=") ORDER BY Relationship";
			DataTable tableRawGuardians=dcon.GetTable(command);
			//QUERY 5: tableCarriers: insplan (Carriers is cached)=====================================================================================================
			DataTable tableCarriers=InsPlans.GetCarrierNames(listPlanNums);
			Dictionary<long,string> dictCarriers=tableCarriers.Select().ToDictionary(x => PIn.Long(x["PlanNum"].ToString()),x => PIn.String(x["CarrierName"].ToString()));
			Dictionary<long,string> dictCarrierColors=tableCarriers.Select().ToDictionary(x => PIn.Long(x["PlanNum"].ToString()),x => x["CarrierColor"].ToString());
			//QUERY 6: dictDiscountPlans: discountplan=================================================================================================================
			Dictionary<long,DiscountPlan> dictDiscountPlans=DiscountPlans.GetAll(true).ToDictionary(x => x.DiscountPlanNum);
			//QUERY 7: listPatsWithDisease: disease=================================================================================================================
			List<long> listPatsWithDisease=Diseases.GetPatientsWithDisease(listPatNums);
			//QUERY 8: listPatsWithAllergy: allergy=================================================================================================================
			List<long> listPatsWithAllergy=Allergies.GetPatientsWithAllergy(listPatNums);
			Dictionary<long,List<ApptBubbleReferralInfo>> dictPatientRefInfos=new Dictionary<long,List<ApptBubbleReferralInfo>>();
			List<long> listRefNums=new List<long>();
			//QUERY 9: listRefAttaches: refattach=================================================================================================================
			List<RefAttach> listRefAttaches=RefAttaches.GetRefAttaches(listPatNums);
			for(int i=0;i<listRefAttaches.Count;i++) {
				if(!listRefNums.Contains(listRefAttaches[i].ReferralNum)) {
					listRefNums.Add(listRefAttaches[i].ReferralNum);
				}
			}
			List<Referral> listReferrals=Referrals.GetReferrals(listRefNums);//cached
			for(int j=0;j<listRefAttaches.Count;j++) {
				string nameLF="";
				string phoneNum="";
				for(int k=0;k<listReferrals.Count;k++) {
					if(listRefAttaches[j].ReferralNum==listReferrals[k].ReferralNum) {
						nameLF=listReferrals[k].LName+", "+listReferrals[k].FName;
						phoneNum=TelephoneNumbers.AutoFormat(listReferrals[k].Telephone);
						if(phoneNum=="") {
							phoneNum=Lans.g("Appointments","(No Phone Number)");
						}
					}
				}
				if(!dictPatientRefInfos.ContainsKey(listRefAttaches[j].PatNum)) {//New entry
					dictPatientRefInfos.Add(listRefAttaches[j].PatNum,new List<ApptBubbleReferralInfo>());
				}
				//Add all referrals nameLF's to the refFrom dict
				dictPatientRefInfos[listRefAttaches[j].PatNum].Add(new ApptBubbleReferralInfo(listRefAttaches[j].RefType,nameLF,phoneNum));
			}
			//QUERY 10: listRecallsPastDue: recall=================================================================================================================
			List<Recall> listRecallsPastDue=Recalls.GetPastDueForPats(dateStart,listPatNums);
			//QUERY 11: listAdjustments: adjustment, appointment union adjustment=====================================================================================
			command=Adjustments.GetQueryAdjustmentsForAppointments(dateStart,dateEnd,listOpNums,doGetSum:false);
			if(tableRawProc!=null && tableRawProc.Rows.Count > 0) {
				string procNums=string.Join(",",tableRawProc.AsEnumerable().Select(x => x["ProcNum"].ToString()));
				if(rawProcLab!=null && rawProcLab.Rows.Count>0) {
					procNums+=","+string.Join(",",rawProcLab.AsEnumerable().Select(x => x["ProcNum"].ToString()));
				}
				command+=@" UNION 
					SELECT * FROM adjustment WHERE ProcNum IN("+procNums+")";
			}
			List<Adjustment> listAdjustments=Crud.AdjustmentCrud.SelectMany(command);
			//This will be set to all rows out of convenience. It will only be accessed from row-0.
			decimal adjustmentAmt=listAdjustments.Sum(x => (decimal)x.AdjAmt);
			//QUERY 12: tableInsVerify: insverify, patplan, insplan, inssub=====================================================================================
			DataTable tableInsVerify=null;
			if(includeVerifyIns){
				string insPlanNums=string.Join(",",listPlanNums);
				string patNums=string.Join(",",listPatNums);//always valid
				command="SELECT FKey, DateLastVerified, VerifyType, patplan.PatNum, insplan.HideFromVerifyList, inssub.PlanNum FROM insverify"
					+" LEFT JOIN patplan ON patplan.PatPlanNum=insverify.FKey AND insverify.VerifyType="+POut.Enum<VerifyTypes>(VerifyTypes.PatientEnrollment)
					+" LEFT JOIN insplan ON insplan.PlanNum=insverify.FKey AND insverify.VerifyType="+POut.Enum<VerifyTypes>(VerifyTypes.InsuranceBenefit)
					+" LEFT JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum"
					+" WHERE (insverify.FKey IN("+insPlanNums+") AND insverify.VerifyType="+POut.Enum<VerifyTypes>(VerifyTypes.InsuranceBenefit)+")"
					+" OR (patplan.PatNum IN("+patNums+") AND insverify.VerifyType="+POut.Enum<VerifyTypes>(VerifyTypes.PatientEnrollment)+")";
				if(insPlanNums!="") {//if no insPlans, then there can't be any patPlans.
					tableInsVerify=dcon.GetTable(command);
				}
			}
			List<Action> actions=new List<Action>();
			//DataTable is not thread-safe so protect it with a local lock.
			object locker=new object();
			foreach(DataRow rowRaw in tableRaw.Rows) {
				//Each of these actions will be stored a executed in parallel below via ODThread.ThreadPool().
				actions.Add(new Action(() => {
					DataRow row;
					DateTime aptDate;
					DateTime aptDateArrived;
					TimeSpan span;
					int hours;
					int minutes;
					DateTime labDate;
					DateTime labDueDate;
					DateTime birthdate;
					DateTime timeAskedToArrive;
					decimal production;
					decimal writeoffPPO;
					decimal insAmt;
					lock (locker) {
						row=table.NewRow();
					}
					#region Make Row
					row["address"]=Patients.GetAddressFull(rowRaw["patAddress1"].ToString(),rowRaw["patAddress2"].ToString(),
					rowRaw["patCity"].ToString(),rowRaw["patState"].ToString(),rowRaw["patZip"].ToString());
					row["addrNote"]="";
					if(rowRaw["patAddrNote"].ToString()!="") {
						row["addrNote"]=Lans.g("Appointments","AddrNote: ")+rowRaw["patAddrNote"].ToString();
					}
					aptDate=PIn.DateT(rowRaw["apptAptDateTime"].ToString());
					aptDateArrived=PIn.DateT(rowRaw["apptAptDateTimeArrived"].ToString());
					void trySetDateTime(string field,DateTime dateTime,int limit=10) {
						int attempts=1;
						row[field]=dateTime;
						while(PIn.DateT(row[field].ToString())!=dateTime && attempts<=limit) {
							//In the event of an asynchronous collision that caused this field to not be set correctly, re-attempt a limited number of times.
							row[field]=dateTime;
							attempts++;
						}
					}
					trySetDateTime("AptDateTime",aptDate);
					trySetDateTime("AptDateTimeArrived",aptDateArrived);
					birthdate=PIn.Date(rowRaw["patBirthdate"].ToString());
					DateTime dateTimeDeceased=PIn.Date(rowRaw["patDateTimeDeceased"].ToString());
					DateTime dateTimeTo=DateTime.Now;
					if(dateTimeDeceased.Year>1880) {
						dateTimeTo=dateTimeDeceased;
					}
					row["age"]="";
					if(birthdate.AddYears(18)<dateTimeTo) {
						row["age"]=Lans.g("Appointments","Age: ");//only show if older than 18
					}
					if(birthdate.Year>1880) {
						row["age"]+=PatientLogic.DateToAgeString(birthdate,dateTimeTo);
					}
					else {
						row["age"]+="?";
					}
					row["apptModNote"]="";
					if(rowRaw["patApptModNote"].ToString()!="") {
						row["apptModNote"]=Lans.g("Appointments","ApptModNote: ")+rowRaw["patApptModNote"].ToString();
					}
					row["aptDate"]=aptDate.ToShortDateString();
					row["aptDay"]=aptDate.ToString("dddd");
					span=TimeSpan.FromMinutes(rowRaw["apptPattern"].ToString().Length*5);
					hours=span.Hours;
					minutes=span.Minutes;
					if(hours==0) {
						row["aptLength"]=minutes.ToString()+Lans.g("Appointments"," Min");
					}
					else if(hours==1) {
						row["aptLength"]=hours.ToString()+Lans.g("Appointments"," Hr, ")
							+minutes.ToString()+Lans.g("Appointments"," Min");
					}
					else {
						row["aptLength"]=hours.ToString()+Lans.g("Appointments"," Hrs, ")
							+minutes.ToString()+Lans.g("Appointments"," Min");
					}
					row["aptTime"]=aptDate.ToShortTimeString();
					row["AptNum"]=rowRaw["apptAptNum"].ToString();
					row["AptStatus"]=rowRaw["apptAptStatus"].ToString();
					row["Assistant"]=rowRaw["apptAssistant"].ToString();
					row["assistantAbbr"]="";
					if(row["Assistant"].ToString()!="0") {
						row["assistantAbbr"]=Employees.GetAbbr(PIn.Long(rowRaw["apptAssistant"].ToString()));
					}
					row["AppointmentTypeNum"]=rowRaw["AppointmentTypeNum"].ToString();
					row["billingType"]=Defs.GetName(DefCat.BillingTypes,PIn.Long(rowRaw["patBillingType"].ToString()));
					row["Birthdate"]=birthdate.ToShortDateString();
					row["chartNumber"]=rowRaw["patChartNumber"].ToString();
					row["chartNumAndName"]="";
					if(rowRaw["apptIsNewPatient"].ToString()=="1") {
						row["chartNumAndName"]="NP-";
					}
					row["chartNumAndName"]+=rowRaw["patChartNumber"].ToString()+" "
						+PatientLogic.GetNameLF(rowRaw["patLName"].ToString(),rowRaw["patFName"].ToString(),
						rowRaw["patPreferred"].ToString(),rowRaw["patMiddleI"].ToString());
					row["ClinicNum"]=rowRaw["ClinicNum"].ToString();
					row["ColorOverride"]=rowRaw["apptColorOverride"].ToString();
					row["confirmed"]=Defs.GetName(DefCat.ApptConfirmed,PIn.Long(rowRaw["apptConfirmed"].ToString()));
					row["Confirmed"]=rowRaw["apptConfirmed"].ToString();
					row["contactMethods"]="";
					if(rowRaw["patPreferConfirmMethod"].ToString()!="0") {
						row["contactMethods"]+=Lans.g("Appointments","Confirm Method: ")
							+((ContactMethod)PIn.Long(rowRaw["patPreferConfirmMethod"].ToString())).ToString();
					}
					if(rowRaw["patPreferContactMethod"].ToString()!="0") {
						if(row["contactMethods"].ToString()!="") {
							row["contactMethods"]+="\r\n";
						}
						row["contactMethods"]+=Lans.g("Appointments","Contact Method: ")
							+((ContactMethod)PIn.Long(rowRaw["patPreferContactMethod"].ToString())).ToString();
					}
					if(rowRaw["patPreferRecallMethod"].ToString()!="0") {
						if(row["contactMethods"].ToString()!="") {
							row["contactMethods"]+="\r\n";
						}
						row["contactMethods"]+=Lans.g("Appointments","Recall Method: ")
							+((ContactMethod)PIn.Long(rowRaw["patPreferRecallMethod"].ToString())).ToString();
					}
					bool InsToSend=false;
					if(tableRawInsProc!=null) {
						//figure out if pt's family has ins claims that need to be created
						for(int j = 0;j<tableRawInsProc.Rows.Count;j++) {
							if(rowRaw["hasIns"].ToString()!="0") {
								if(rowRaw["patGuarantor"].ToString()==tableRawInsProc.Rows[j]["Guarantor"].ToString()
									||rowRaw["patGuarantor"].ToString()==tableRawInsProc.Rows[j]["PatNum"].ToString()) {
									InsToSend=true;
								}
							}
						}
					}
					row["CreditType"]=rowRaw["patCreditType"].ToString();
					row["discountPlan"]="";
					long discountPlanNum=PIn.Long(rowRaw["DiscountPlan"].ToString());
					DiscountPlan discountPlan=null;
					if(discountPlanNum>0 && dictDiscountPlans.TryGetValue(discountPlanNum,out discountPlan)) {
						row["discountPlan"]+=Lans.g("Appointments","DiscountPlan")+": "+dictDiscountPlans[discountPlanNum].Description;
					}
					row["Email"]=rowRaw["patEmail"].ToString();
					row["famFinUrgNote"]="";
					if(rowRaw["guarFamFinUrgNote"].ToString()!="") {
						row["famFinUrgNote"]=Lans.g("Appointments","FamFinUrgNote: ")+rowRaw["guarFamFinUrgNote"].ToString();
					}
					row["guardians"]="";
					GuardianRelationship guardRelat;
					for(int g = 0;g<tableRawGuardians.Rows.Count;g++) {
						if(rowRaw["apptPatNum"].ToString()==tableRawGuardians.Rows[g]["PatNumChild"].ToString()) {
							if(row["guardians"].ToString()!="") {
								row["guardians"]+=",";
							}
							guardRelat=(GuardianRelationship)PIn.Int(tableRawGuardians.Rows[g]["Relationship"].ToString());
							row["guardians"]+=Patients.GetNameFirstOrPreferred(tableRawGuardians.Rows[g]["FName"].ToString(),tableRawGuardians.Rows[g]["Preferred"].ToString())
								+Guardians.GetGuardianRelationshipStr(guardRelat);
						}
					}
					row["hasDiscount[D]"]="";
					if(rowRaw["hasDiscount"].ToString()!="0") {
						row["hasDiscount[D]"]+="D";
					}
					row["hasIns[I]"]="";
					if(rowRaw["hasIns"].ToString()!="0") {
						row["hasIns[I]"]+="I";
					}
					row["hmPhone"]=Lans.g("Appointments","Hm: ")+rowRaw["patHmPhone"].ToString();
					row["ImageFolder"]=rowRaw["patImageFolder"].ToString();
					row["insurance"]="";
					long planNum1=PIn.Long(rowRaw["InsPlan1"].ToString());
					long planNum2=PIn.Long(rowRaw["InsPlan2"].ToString());
					if(planNum1>0 && dictCarriers.ContainsKey(planNum1)) {
						row["insurance"]+=Lans.g("Appointments","Ins1")+": "+dictCarriers[planNum1];
						row["insColor1"]=dictCarrierColors[planNum1];
					}
					if(planNum2>0 && dictCarriers.ContainsKey(planNum2)) {
						if(row["insurance"].ToString()!="") {
							row["insurance"]+="\r\n";
						}
						row["insurance"]+=Lans.g("Appointments","Ins2")+": "+dictCarriers[planNum2];
						row["insColor2"]=dictCarrierColors[planNum2];
					}
					if(rowRaw["hasIns"].ToString()!="0"&&row["insurance"].ToString()=="") {
						row["insurance"]=Lans.g("Appointments","Insured");
					}
					row["insToSend[!]"]="";
					if(InsToSend) {
						row["insToSend[!]"]="!";
					}
					row["Priority"]=rowRaw["Priority"].ToString();
					row["IsHygiene"]=rowRaw["apptIsHygiene"].ToString();
					row["lab"]="";
					if(rowRaw["labcaseLabCaseNum"].ToString()!="") {
						labDate=PIn.DateT(rowRaw["labcaseDateTimeChecked"].ToString());
						if(labDate.Year>1880) {
							row["lab"]=Lans.g("Appointments","Lab Quality Checked");
						}
						else {
							labDate=PIn.DateT(rowRaw["labcaseDateTimeRecd"].ToString());
							if(labDate.Year>1880) {
								row["lab"]=Lans.g("Appointments","Lab Received");
							}
							else {
								labDate=PIn.DateT(rowRaw["labcaseDateTimeSent"].ToString());
								if(labDate.Year>1880) {
									row["lab"]=Lans.g("Appointments","Lab Sent");//sent but not received
								}
								else {
									row["lab"]=Lans.g("Appointments","Lab Not Sent");
								}
								labDueDate=PIn.DateT(rowRaw["labcaseDateTimeDue"].ToString());
								if(labDueDate.Year>1880) {
									row["lab"]+=", "+Lans.g("Appointments","Due: ")//+dateDue.ToString("ddd")+" "
										+labDueDate.ToShortDateString();//+" "+dateDue.ToShortTimeString();
								}
							}
						}
					}
					CultureInfo culture=CodeBase.MiscUtils.GetCultureFromThreeLetter(rowRaw["patLanguage"].ToString());
					if(culture==null) {//custom language
						row["language"]=rowRaw["patLanguage"].ToString();
					}
					else {
						row["language"]=culture.DisplayName;
					}
					row["medOrPremed[+]"]="";
					long apptPatNum=PIn.Long(rowRaw["apptPatNum"].ToString());
					if(rowRaw["patMedUrgNote"].ToString()!=""||rowRaw["patPremed"].ToString()=="1"
						||listPatsWithDisease.Contains(apptPatNum)||listPatsWithAllergy.Contains(apptPatNum)) {
						row["medOrPremed[+]"]="+";
					}
					row["MedUrgNote"]=rowRaw["patMedUrgNote"].ToString();
					row["Note"]=rowRaw["apptNote"].ToString();
					row["Op"]=rowRaw["apptOp"].ToString();
					if(rowRaw["apptIsNewPatient"].ToString()=="1") {
						row["patientName"]="NP-";
					}
					row["patientName"]+=PatientLogic.GetNameLF(rowRaw["patLName"].ToString(),rowRaw["patFName"].ToString(),
						rowRaw["patPreferred"].ToString(),rowRaw["patMiddleI"].ToString());
					if(rowRaw["apptIsNewPatient"].ToString()=="1") {
						row["patientNameF"]="NP-";
					}
					row["patientNameF"]+=rowRaw["patFName"].ToString();
					row["patientNamePref"]+=rowRaw["patPreferred"].ToString();
					row["PatNum"]=rowRaw["apptPatNum"].ToString();
					row["patNum"]="PatNum: "+rowRaw["apptPatNum"].ToString();
					row["GuarNum"]=rowRaw["patGuarantor"].ToString();
					row["patNumAndName"]="";
					if(rowRaw["apptIsNewPatient"].ToString()=="1") {
						row["patNumAndName"]="NP-";
					}
					row["patNumAndName"]+=rowRaw["apptPatNum"].ToString()+" "
						+PatientLogic.GetNameLF(rowRaw["patLName"].ToString(),rowRaw["patFName"].ToString(),
						rowRaw["patPreferred"].ToString(),rowRaw["patMiddleI"].ToString());
					row["Pattern"]=rowRaw["apptPattern"].ToString();
					row["PatternSecondary"]=rowRaw["apptPatternSecondary"].ToString();
					row["preMedFlag"]="";
					if(rowRaw["patPremed"].ToString()=="1") {
						row["preMedFlag"]=Lans.g("Appointments","Premedicate");
					}
					row["procs"]=rowRaw["apptProcDescript"].ToString();
					row["procsColored"]+=rowRaw["apptProcsColored"].ToString();
					production=0;
					writeoffPPO=0;
					insAmt=0;
					decimal adjAmtForAppt=0;
					decimal discountPlanDiscount=0;
					decimal procDiscount=0;
					if(tableRawProc!=null) {
						for(int p = 0;p<tableRawProc.Rows.Count;p++) {
							ProcStat procStat=PIn.Enum<ProcStat>(PIn.Int(tableRawProc.Rows[p]["ProcStatus"].ToString()));
							if(isPlanned && rowRaw["apptAptNum"].ToString()!=tableRawProc.Rows[p]["PlannedAptNum"].ToString()) {
								continue;
							}
							else if(!isPlanned && rowRaw["apptAptNum"].ToString()!=tableRawProc.Rows[p]["AptNum"].ToString()) {
								continue;
							}
							//We only want to include C, TP, and TPi procedures in the production calculation.
							if(!ListTools.In(procStat,ProcStat.C,ProcStat.TP,ProcStat.TPi)) {
								continue;
							}
							decimal procFee=PIn.Decimal(tableRawProc.Rows[p]["ProcFee"].ToString());
							production+=procFee;
							production-=PIn.Decimal(tableRawProc.Rows[p]["writeoffCap"].ToString());
							//WriteOffEst -1 and WriteOffEstOverride -1 already excluded
							//production-=
							writeoffPPO+=PIn.Decimal(tableRawProc.Rows[p]["writeoffPPO"].ToString());//frequently zero
							insAmt+=PIn.Decimal(tableRawProc.Rows[p]["insAmt"].ToString());
							adjAmtForAppt+=listAdjustments.Where(x => x.ProcNum==PIn.Long(tableRawProc.Rows[p]["ProcNum"].ToString())).Sum(x => (decimal)x.AdjAmt);
							if(rawProcLab!=null) { //Will be null if not Canada.
								for(int a = 0;a<rawProcLab.Rows.Count;a++) {
									if(rawProcLab.Rows[a]["ProcNumLab"].ToString()==tableRawProc.Rows[p]["ProcNum"].ToString()) {
										production+=PIn.Decimal(rawProcLab.Rows[a]["ProcFee"].ToString());
										production-=PIn.Decimal(rawProcLab.Rows[a]["writeoffCap"].ToString());
										writeoffPPO+=PIn.Decimal(rawProcLab.Rows[a]["writeoffPPO"].ToString());//frequently zero
										insAmt+=PIn.Decimal(tableRawProc.Rows[p]["insAmt"].ToString());
										adjAmtForAppt+=listAdjustments.Where(x => x.ProcNum==PIn.Long(rawProcLab.Rows[a]["ProcNum"].ToString())).Sum(x => (decimal)x.AdjAmt);
									}
								}
							}
							if(procStat!=ProcStat.C) {
								procDiscount+=PIn.Decimal(tableRawProc.Rows[p]["Discount"].ToString());//procedurelog.Discount
								decimal procDiscountPlanAmount=PIn.Decimal(tableRawProc.Rows[p]["DiscountPlanAmt"].ToString());//procedurelog.DiscountPlanAmt
								if(discountPlan!=null && CompareDecimal.IsGreaterThanOrEqualToZero(procDiscountPlanAmount)) {
									discountPlanDiscount+=procDiscountPlanAmount;
								}
							}
						}
					}
					row["prophy/PerioPastDue[P]"]=(Recalls.IsPatientPastDue(PIn.Long(rowRaw["apptPatNum"].ToString()),aptDate,true,listRecallsPastDue)? "P":"");
					row["production"]=production.ToString("c");//PIn.Double(rowRaw["Production"].ToString()).ToString("c");
					row["productionVal"]=production.ToString();//rowRaw["Production"].ToString();
					decimal netProduction=(production-writeoffPPO-discountPlanDiscount);
					if(PrefC.GetBool(PrefName.ApptModuleAdjustmentsInProd)) {
						netProduction+=adjAmtForAppt-procDiscount;
					}
					row["netProduction"]=netProduction.ToString("c");
					row["netProductionVal"]=netProduction.ToString();
					decimal patientPortion=Math.Max(production-writeoffPPO+adjAmtForAppt-insAmt-discountPlanDiscount-procDiscount,0);
					row["estPatientPortion"]=patientPortion.ToString("c");
					row["estPatientPortionRaw"]=patientPortion;
					row["adjustmentTotal"]=adjustmentAmt.ToString();
					row["ProvBarText"]=rowRaw["ProvBarText"].ToString();
					long apptProvNum=PIn.Long(rowRaw["apptProvNum"].ToString());
					long apptProvHyg=PIn.Long(rowRaw["apptProvHyg"].ToString());
					if(rowRaw["apptIsHygiene"].ToString()=="1") {
						row["provider"]=Providers.GetAbbr(apptProvHyg);
						if(apptProvNum!=0) {
							row["provider"]+=" ("+Providers.GetAbbr(apptProvNum)+")";
						}
					}
					else {
						row["provider"]=Providers.GetAbbr(apptProvNum);
						if(apptProvHyg!=0) {
							row["provider"]+=" ("+Providers.GetAbbr(apptProvHyg)+")";
						}
					}
					row["ProvNum"]=rowRaw["apptProvNum"].ToString();
					row["ProvHyg"]=rowRaw["apptProvHyg"].ToString();
					row["recallPastDue[R]"]=(Recalls.IsPatientPastDue(PIn.Long(rowRaw["apptPatNum"].ToString()),aptDate,false,listRecallsPastDue) ? "R" : "");
					string referralFrom="";
					string referralFromWithPhone="";
					string referralTo="";
					string referralToWithPhone="";
					if(dictPatientRefInfos.ContainsKey(apptPatNum)) {//Add this patient's referrals
						List<ApptBubbleReferralInfo> listFromRefInfos=dictPatientRefInfos[apptPatNum].FindAll(x => x.RefType==ReferralType.RefFrom);
						if(!listFromRefInfos.IsNullOrEmpty()) {
							if(listFromRefInfos.Any(x => !string.IsNullOrEmpty(x.Name))) {
								referralFrom=Lans.g("Appointment",$"Referred From:")+"\r\n"
									+string.Join("\r\n",listFromRefInfos.Select(x => x.Name));
								referralFromWithPhone=Lans.g("Appointment",$"Referred From With Phone:")+"\r\n"
									+string.Join("\r\n",listFromRefInfos.Select(x => x.Name+" "+x.PhoneNumber));
							}
						}
						List<ApptBubbleReferralInfo> listToRefInfos=dictPatientRefInfos[apptPatNum].FindAll(x => x.RefType==ReferralType.RefTo);
						if(!listToRefInfos.IsNullOrEmpty()) {
							if(listToRefInfos.Any(x => !string.IsNullOrEmpty(x.Name))) {
								referralTo=Lans.g("Appointment",$"Referred To:")+"\r\n"
									+string.Join("\r\n",listToRefInfos.Select(x => x.Name));
								referralToWithPhone=Lans.g("Appointment",$"Referred To With Phone:")+"\r\n"
									+string.Join("\r\n",listToRefInfos.Select(x => x.Name+" "+x.PhoneNumber));
							}
						}
					}
					row["referralFrom"]=referralFrom;
					row["referralFromWithPhone"]=referralFromWithPhone;
					row["referralTo"]=referralTo;
					row["referralToWithPhone"]=referralToWithPhone;
					row["timeAskedToArrive"]="";
					timeAskedToArrive=PIn.DateT(rowRaw["apptDateTimeAskedToArrive"].ToString());
					if(timeAskedToArrive.Year>1880) {
						row["timeAskedToArrive"]=timeAskedToArrive.ToString("H:mm");
					}
					if(includeVerifyIns){
						DateTime dateTimeLastPlanBenefits=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDays));
						DateTime dateTimeLastPatEligibility=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDays));
						bool excludePatVerify=PrefC.GetBool(PrefName.InsVerifyExcludePatVerify);
						if(tableInsVerify!=null && tableInsVerify.Rows.Count!=0) {
							for(int v=0; v<tableInsVerify.Rows.Count; v++) {
								if(PIn.Long(rowRaw["InsPlan1"].ToString())==PIn.Long(tableInsVerify.Rows[v]["FKey"].ToString())) { 
									if(VerifyTypes.InsuranceBenefit!=PIn.Enum<VerifyTypes>(tableInsVerify.Rows[v]["VerifyType"].ToString())) {
										continue;
									}
									if(PIn.Bool(tableInsVerify.Rows[v]["HideFromVerifyList"].ToString())) {
										continue;//Specifically marked as don't verify, so skip it
									}
									if(PIn.Date(tableInsVerify.Rows[v]["DateLastVerified"].ToString())<dateTimeLastPlanBenefits) {
										row["verifyIns[V]"]="V";
										break;
									}
								}
								if(PIn.Long(rowRaw["InsPlan2"].ToString())==PIn.Long(tableInsVerify.Rows[v]["FKey"].ToString())) {
									if(VerifyTypes.InsuranceBenefit!=PIn.Enum<VerifyTypes>(tableInsVerify.Rows[v]["VerifyType"].ToString())) {
										continue;
									}
									if(PIn.Bool(tableInsVerify.Rows[v]["HideFromVerifyList"].ToString())) {
										continue;
									}
									if(PIn.Date(tableInsVerify.Rows[v]["DateLastVerified"].ToString())<dateTimeLastPlanBenefits) {
										row["verifyIns[V]"]="V";
										break;
									}
								}
								if(VerifyTypes.PatientEnrollment!=PIn.Enum<VerifyTypes>(tableInsVerify.Rows[v]["VerifyType"].ToString())) {
									continue;
								}
								if(apptPatNum!=PIn.Long(tableInsVerify.Rows[v]["PatNum"].ToString())) {
									continue;
								}
								if(excludePatVerify) {
									bool excluded=false;
									//On a PatPlan row, we do not have any info about the plan except the PlanNum.
									//That info is guaranteed to be on another row. If this assumption is wrong, the loop is harmless.
									for(int i=0; i<tableInsVerify.Rows.Count;i++) {
										if(PIn.Long(tableInsVerify.Rows[i]["FKey"].ToString())==PIn.Long(tableInsVerify.Rows[v]["PlanNum"].ToString())) {
											if(PIn.Bool(tableInsVerify.Rows[i]["HideFromVerifyList"].ToString())) {
												excluded=true;
												break;
											}
										}
									}
									if(excluded) {
										break;//Skip any patplan that is apart of a insplan that is marked as don't verify
									}
								}
								if(PIn.Date(tableInsVerify.Rows[v]["DateLastVerified"].ToString())<dateTimeLastPatEligibility) {
									row["verifyIns[V]"]="V";
									break;
								}
							}
						}
					}
					row["wirelessPhone"]=Lans.g("Appointments","Cell: ")+rowRaw["patWirelessPhone"].ToString();
					row["wkPhone"]=Lans.g("Appointments","Wk: ")+rowRaw["patWkPhone"].ToString();
					row["writeoffPPO"]=writeoffPPO.ToString();
					#endregion Make Row
					lock (locker) {
						table.Rows.Add(row);
					}
				}));
			}
			if(actions.Count<=500) {
				//Per https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable?view=netframework-4.8, DataTable is not thread-safe for write 
				//operations.  Even with locking the creation/addition of the DataRow, simply writing to various fields has been shown to occasionally drop
				//data.  In the case of B16298, this resulted in appointments disappearing from view due to AptDateTime field not being set.  Building each 
				//DataRow synchronously avoids the issue with DataTable thread safety.  Additionally, testing has shown that for fewer than 500 rows, it is 
				//faster to run synchronously.  Most ApptViews are expected to result in this DataTable containing fewer than 500 rows, which correlates to 
				//an ApptView with 20 operatories with 25 appointments per operatory.
				actions.ForEach(x => x());
			}
			else {
				//However, there are valid scenarios where a large customer may have a very large DataTable being built here.  Synchronously building each
				//row results in unacceptable slowness in this method, and was the original reason these actions were threaded.  Setting the type on the 
				//AptDateTime and AptDateTimeArrived fields has been shown to dramatically, but not fully, reduce the occurrence of dropped data.  Logic has
				//been added in the individual actions to attempt to validate/reset AptDateTime and AtpDateTimeArrived when these fields are not set 
				//correctly.
				ODThread.RunParallel(actions,TimeSpan.FromMinutes(1));
			}
			return table;
		}


		///<summary>Gets a mini version of the PeriodApptsTable for all appointments on the passed in day for the given provider. This is specifically
		///used by WebSched so that we don't have to get the full PeriodApptsTable when checking for double booked appointments.</summary>
		public static DataTable GetPeriodApptsTableMini(DateTime dateTimeAppointmentStart,long provNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateTimeAppointmentStart,provNum);
			} 
			DataConnection dcon=new DataConnection();
			string command=@"SELECT 
				appt.AptDateTime,
				appt.AptNum,
				appt.AptStatus,
				appt.IsHygiene,
				appt.Pattern,
				appt.ProvHyg,
				appt.ProvNum
				FROM appointment appt
				WHERE (appt.ProvNum="+POut.Long(provNum)+" OR appt.ProvHyg="+POut.Long(provNum)+")"+
				" AND appt.AptDateTime BETWEEN "+POut.DateT(dateTimeAppointmentStart.Date)+" AND "+POut.DateT(dateTimeAppointmentStart.AddDays(1).Date);
			return dcon.GetTable(command);
		}

		///<summary>Returns a DataTable with all the ApptFields for the appointments passed in.</summary>
		public static DataTable GetApptFields(DataTable tableAppts) {
			//No need to check RemotingRole; no call to db.
			List<long> aptNums=new List<long>();
			for(int i=0;i<tableAppts.Rows.Count;i++) {
				aptNums.Add(PIn.Long(tableAppts.Rows[i]["AptNum"].ToString()));
			}
			return GetApptFieldsByApptNums(aptNums);
		}

		/// <summary>Returns a DataTable with all the ApptFields for the AptNums passed in.</summary>
		public static DataTable GetApptFieldsByApptNums(List<long> aptNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),aptNums);
			}
			string command="SELECT AptNum,FieldName,FieldValue "
				+"FROM apptfield "
				+"WHERE AptNum IN (";
			if(aptNums.Count==0) {
				command+="0";
			}
			else for(int i=0;i<aptNums.Count;i++) {
				if(i>0) {
					command+=",";
				}
				command+=POut.Long(aptNums[i]);
			}
			command+=")";
			DataConnection dcon=new DataConnection();
			DataTable table= dcon.GetTable(command);
			table.TableName="ApptFields";
			return table;
		}

		///<summary>Returns a DataTable with all the PatFields for the PatNums passed in.  Columns: PatNum, FieldName, FieldValue.
		///It's in Appointments class because it used to get passed an entire TableAppointments.</summary>
		public static DataTable GetPatFields(List<long> listPatNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums.Count==0) {
				listPatNums.Add(0);
			}
			string command="SELECT PatNum,FieldName,FieldValue "
				+"FROM patfield "
				+"WHERE PatNum IN ("+String.Join(",",listPatNums)+")";
			DataConnection dcon=new DataConnection();
			DataTable table=dcon.GetTable(command);
			table.TableName="PatFields";
			return table;
		}

		///<summary>Returns all the appointment fields that should show in the appointment edit window.</summary>
		public static DataTable GetApptFields(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT appointmentfield.ApptFieldNum,apptfielddef.FieldName,appointmentfield.FieldValue "
				+"FROM apptfielddef "
				+"LEFT JOIN fielddeflink ON apptfielddef.ApptFieldDefNum=fielddeflink.FieldDefNum "
					+"AND fielddeflink.FieldDefType="+POut.Int((int)FieldDefTypes.Appointment)+" "
					+"AND fielddeflink.FieldLocation="+POut.Int((int)FieldLocations.AppointmentEdit)+" "
				+"LEFT JOIN ("
					+"SELECT apptfield.ApptFieldNum,apptfield.FieldName,apptfield.FieldValue "
					+"FROM apptfield "
					+"WHERE AptNum = "+POut.Long(aptNum)+" "
					+"GROUP BY apptfield.FieldName "
				+") appointmentfield ON apptfielddef.FieldName=appointmentfield.FieldName "
				+"WHERE fielddeflink.FieldDefLinkNum IS NULL "
				+"ORDER BY apptfielddef.FieldName";
			DataConnection dcon=new DataConnection();
			DataTable table= dcon.GetTable(command);
			table.TableName="ApptFields";
			return table;
		}

		///<summary>Used to get the waiting room data table. Pass in the client date time to get the correct waiting time. The date time is also passed into middle tier so that it uses the correct client time.</summary>
		public static DataTable GetPeriodWaitingRoomTable(DateTime dateTime) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateTime);
			}
			//DateTime dateStart=PIn.PDate(strDateStart);
			//DateTime dateEnd=PIn.PDate(strDateEnd);
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("WaitingRoom");
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("patName");
			table.Columns.Add("FName");
			table.Columns.Add("LName");
			table.Columns.Add("waitTime");
			table.Columns.Add("OpNum");
			string strDateTime=POut.DateT(dateTime);
			string command="SELECT DateTimeArrived,DateTimeSeated,LName,FName,Preferred,"+strDateTime+" dateTimeNow,Op "
				+"FROM appointment "
				+"JOIN patient ON appointment.PatNum=patient.PatNum "
				+"WHERE "+DbHelper.DtimeToDate("AptDateTime")+" = "+POut.Date(DateTime.Now)+" "
				+"AND DateTimeArrived > "+POut.Date(DateTime.Now)+" "//midnight earlier today
				+"AND DateTimeArrived < "+DbHelper.Now()+" "
				+"AND "+DbHelper.DtimeToDate("DateTimeArrived")+"="+DbHelper.DtimeToDate("AptDateTime")+" ";//prevents people from getting "stuck" in waiting room.
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				command+="AND TO_NUMBER(TO_CHAR(DateTimeSeated,'SSSSS')) = 0 "
					+"AND TO_NUMBER(TO_CHAR(DateTimeDismissed,'SSSSS')) = 0 ";
			}
			else{
				command+="AND TIME(DateTimeSeated) = 0 "
					+"AND TIME(DateTimeDismissed) = 0 ";
			}
			command+="AND AptStatus IN ("+POut.Int((int)ApptStatus.Complete)+","
																	 +POut.Int((int)ApptStatus.Scheduled)+") "//None of the other statuses
				+"ORDER BY AptDateTime";
			DataTable raw=dcon.GetTable(command);
			TimeSpan timeArrived;
			//DateTime timeSeated;
			DateTime waitTime;
			Patient pat;
			DateTime dateTimeNow;
			//int minutes;
			for(int i=0;i<raw.Rows.Count;i++) {
				row=table.NewRow();
				pat=new Patient();
				pat.LName=raw.Rows[i]["LName"].ToString();
				pat.FName=raw.Rows[i]["FName"].ToString();
				pat.Preferred=raw.Rows[i]["Preferred"].ToString();
				row["FName"]=raw.Rows[i]["FName"];
				row["LName"]=raw.Rows[i]["LName"];
				row["patName"]=pat.GetNameLF();
				dateTimeNow=PIn.DateT(raw.Rows[i]["dateTimeNow"].ToString());
				timeArrived=(PIn.DateT(raw.Rows[i]["DateTimeArrived"].ToString())).TimeOfDay;
				waitTime=dateTimeNow-timeArrived;
				row["waitTime"]=waitTime.ToString("H:mm:ss");
				//minutes=waitTime.Minutes;
				//if(waitTime.Hours>0){
				//	row["waitTime"]+=waitTime.Hours.ToString()+"h ";
					//minutes-=60*waitTime.Hours;
				//}
				//row["waitTime"]+=waitTime.Minutes.ToString()+"m";
				row["OpNum"]=raw.Rows[i]["Op"].ToString();
				table.Rows.Add(row);
			}
			return table;
		}

		public static DataTable GetApptTable(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM appointment WHERE AptNum="+aptNum.ToString();
			DataTable table=Db.GetTable(command);
			table.TableName="Appointment";
			return table;
		}

		public static DataTable GetPatTable(string patNum,Appointment appt) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,appt);
			}
			DataTable table=new DataTable("Patient");
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("field");
			table.Columns.Add("value");
			string command="SELECT * FROM patient WHERE PatNum="+patNum;
			DataTable rawPat=Db.GetTable(command);
			DataRow row;
			//Patient Name--------------------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Name");
			row["value"]=PatientLogic.GetNameLF(rawPat.Rows[0]["LName"].ToString(),rawPat.Rows[0]["FName"].ToString(),
				rawPat.Rows[0]["Preferred"].ToString(),rawPat.Rows[0]["MiddleI"].ToString());
			table.Rows.Add(row);
			//Patient First Name--------------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","First Name");
			row["value"]=rawPat.Rows[0]["FName"];
			table.Rows.Add(row);
			//Patient Last name---------------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Last Name");
			row["value"]=rawPat.Rows[0]["LName"];
			table.Rows.Add(row);
			//Patient middle initial----------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Middle Initial");
			row["value"]=rawPat.Rows[0]["MiddleI"];
			table.Rows.Add(row);
			//Patient birthdate----------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Birthdate");
			row["value"]=PIn.Date(rawPat.Rows[0]["Birthdate"].ToString()).ToShortDateString();
			table.Rows.Add(row);
			//Patient home phone--------------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Home Phone");
			row["value"]=rawPat.Rows[0]["HmPhone"];
			table.Rows.Add(row);
			//Patient work phone--------------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Work Phone");
			row["value"]=rawPat.Rows[0]["WkPhone"];
			table.Rows.Add(row);
			//Patient wireless phone----------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Wireless Phone");
			row["value"]=rawPat.Rows[0]["WirelessPhone"];
			table.Rows.Add(row);
			//Patient credit type-------------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Credit Type");
			row["value"]=rawPat.Rows[0]["CreditType"];
			table.Rows.Add(row);
			//Patient billing type------------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Billing Type");
			row["value"]=Defs.GetName(DefCat.BillingTypes,PIn.Long(rawPat.Rows[0]["BillingType"].ToString()));
			table.Rows.Add(row);
			//Patient total balance-----------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Total Balance");
			double totalBalance=PIn.Double(rawPat.Rows[0]["EstBalance"].ToString());
			row["value"]=totalBalance.ToString("F");
			table.Rows.Add(row);
			//Patient address and phone notes-------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Address and Phone Notes");
			row["value"]=rawPat.Rows[0]["AddrNote"];
			table.Rows.Add(row);
			//Patient family balance----------------------------------------------------------------
			command="SELECT BalTotal,InsEst FROM patient WHERE PatNum="+POut.String(rawPat.Rows[0]["Guarantor"].ToString())+"";
			DataTable familyBalance=Db.GetTable(command);
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Family Balance");
			double balance=PIn.Double(familyBalance.Rows[0]["BalTotal"].ToString())
				-PIn.Double(familyBalance.Rows[0]["InsEst"].ToString());
			row["value"]=balance.ToString("F");
			table.Rows.Add(row);
			//Site----------------------------------------------------------------------------------
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				row=table.NewRow();
				row["field"]=Lans.g("FormApptEdit","Site");
				row["value"]=Sites.GetDescription(PIn.Long(rawPat.Rows[0]["SiteNum"].ToString()));
				table.Rows.Add(row);
			}
			//Estimated Patient Portion-------------------------------------------------------------
			row=table.NewRow();
			row["field"]=Lans.g("FormApptEdit","Est. Patient Portion");
			row["value"]=GetEstPatientPortion(appt).ToString("F");
			table.Rows.Add(row);
			return table;
		}
		
		///<summary>Returns the estimated patient portion for the procedures attached to this appointment.</summary>
		public static decimal GetEstPatientPortion(Appointment appt) {
			if(appt.AptNum==0) {//Appt hasn't been inserted into the database yet.
				return 0;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<decimal>(MethodBase.GetCurrentMethod(),appt);
			}
			DateTime dateStart=appt.AptDateTime.Date;//Use the entire day.
			DateTime dateEnd=appt.AptDateTime.Date.AddDays(1).AddMilliseconds(-1);//Use the entire day.
			DataTable table=GetPeriodApptsTable(dateStart,dateEnd,appt.AptNum,appt.AptStatus==ApptStatus.Planned);
			if(table.Rows.Count==0) {
				return 0;
			}
			return PIn.Decimal(table.Rows[0]["estPatientPortionRaw"].ToString());
		}

		public static DataTable GetProcTable(string patNum,string aptNum,string apptStatus,string aptDateTime) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,aptNum,apptStatus,aptDateTime);
			}
			DataTable table=new DataTable("Procedure");
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("AbbrDesc");
			table.Columns.Add("attached");//0 or 1
			table.Columns.Add("CodeNum");
			table.Columns.Add("descript");
			table.Columns.Add("fee");
			table.Columns.Add("priority");
			table.Columns.Add("Priority");
			table.Columns.Add("ProcCode");
			table.Columns.Add("ProcDate");
			table.Columns.Add("ProcNum");
			table.Columns.Add("ProcStatus");
			table.Columns.Add("ProvNum");
			table.Columns.Add("status");
			table.Columns.Add("Surf");
			table.Columns.Add("toothNum");
			table.Columns.Add("ToothNum");
			table.Columns.Add("ToothRange");
			table.Columns.Add("TreatArea");
			//but we won't actually fill this table with rows until the very end.  It's more useful to use a List<> for now.
			List<DataRow> rows=new List<DataRow>();
			string command="SELECT AbbrDesc,procedurecode.ProcCode,AptNum,LaymanTerm,"
				+"PlannedAptNum,Priority,ProcFee,ProcNum,ProcStatus, "
				+"procedurecode.Descript,procedurelog.CodeNum,ProcDate,procedurelog.ProvNum,Surf,ToothNum,ToothRange,TreatArea "
				+"FROM procedurelog LEFT JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				+"WHERE PatNum="+patNum//sort later
			//1. All TP procs
				+" AND (ProcStatus=1 OR ";//tp
			//2. All attached procs
				//+" AND ";
			if(apptStatus=="6"){//planned
				command+="PlannedAptNum="+aptNum;
			}
			else{
				command+="AptNum="+aptNum;//exclude procs attached to other appts.
			}
			//3. All unattached completed procs with same date as appt.
			//but only if one of these types
			if(apptStatus=="1" || apptStatus=="2" || apptStatus=="4" || apptStatus=="5"){//sched,C,ASAP,broken
				DateTime aptDate=PIn.DateT(aptDateTime);
				command+=" OR (AptNum=0 "//unattached
					+"AND ProcStatus=2 "//complete
					+"AND "+DbHelper.DtimeToDate("ProcDate")+"="+POut.Date(aptDate)+")";//same date
			}
			command+=") "
				+"AND ProcStatus<>6 "//Not deleted.
				+"AND IsCanadianLab=0";
			DataTable rawProc=Db.GetTable(command);
			for(int i=0;i<rawProc.Rows.Count;i++) {
				row=table.NewRow();
				row["AbbrDesc"]=rawProc.Rows[i]["AbbrDesc"].ToString();
				if(apptStatus=="6"){//planned
					row["attached"]=(rawProc.Rows[i]["PlannedAptNum"].ToString()==aptNum) ? "1" : "0";
				}
				else{
					row["attached"]=(rawProc.Rows[i]["AptNum"].ToString()==aptNum) ? "1" : "0";
				}
				row["CodeNum"]=rawProc.Rows[i]["CodeNum"].ToString();
				row["descript"]="";
				if(apptStatus=="6") {//planned
					if(rawProc.Rows[i]["PlannedAptNum"].ToString()!="0" && rawProc.Rows[i]["PlannedAptNum"].ToString()!=aptNum) {
						row["descript"]=Lans.g("FormApptEdit","(other appt)");
					}
					else if(rawProc.Rows[i]["AptNum"].ToString()!="0" && rawProc.Rows[i]["AptNum"].ToString()!=aptNum) {
						row["descript"]=Lans.g("FormApptEdit","(scheduled appt)");
					}
				}
				else {
					if(rawProc.Rows[i]["AptNum"].ToString()!="0" && rawProc.Rows[i]["AptNum"].ToString()!=aptNum) {
						row["descript"]=Lans.g("FormApptEdit","(other appt)");
					}
					else if(rawProc.Rows[i]["PlannedAptNum"].ToString()!="0" && rawProc.Rows[i]["PlannedAptNum"].ToString()!=aptNum) {
						row["descript"]=Lans.g("FormApptEdit","(planned appt)");
					}
				}
				if(rawProc.Rows[i]["LaymanTerm"].ToString()==""){
					row["descript"]+=rawProc.Rows[i]["Descript"].ToString();
				}
				else{
					row["descript"]+=rawProc.Rows[i]["LaymanTerm"].ToString();
				}
				if(rawProc.Rows[i]["ToothRange"].ToString()!=""){
					row["descript"]+=" #"+Tooth.FormatRangeForDisplay(rawProc.Rows[i]["ToothRange"].ToString());
				}
				row["fee"]=PIn.Double(rawProc.Rows[i]["ProcFee"].ToString()).ToString("F");
				row["priority"]=Defs.GetName(DefCat.TxPriorities,PIn.Long(rawProc.Rows[i]["Priority"].ToString()));
				row["Priority"]=rawProc.Rows[i]["Priority"].ToString();
				row["ProcCode"]=rawProc.Rows[i]["ProcCode"].ToString();
				row["ProcDate"]=rawProc.Rows[i]["ProcDate"].ToString();//eg 2012-02-19
				row["ProcNum"]=rawProc.Rows[i]["ProcNum"].ToString();
				row["ProcStatus"]=rawProc.Rows[i]["ProcStatus"].ToString();
				row["ProvNum"]=rawProc.Rows[i]["ProvNum"].ToString();
				row["status"]=((ProcStat)PIn.Long(rawProc.Rows[i]["ProcStatus"].ToString())).ToString();
				row["Surf"]=rawProc.Rows[i]["Surf"].ToString();
				row["toothNum"]=Tooth.GetToothLabel(rawProc.Rows[i]["ToothNum"].ToString());
				row["ToothNum"]=rawProc.Rows[i]["ToothNum"].ToString();
				row["ToothRange"]=rawProc.Rows[i]["ToothRange"].ToString();
				row["TreatArea"]=rawProc.Rows[i]["TreatArea"].ToString();
				rows.Add(row);
			}
			//Sorting
			rows.Sort(CompareRows);
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>Used in FormConfirmList.  The assumption is made that showRecall and showNonRecall will not both be false. Pass in a clinicNum
		///less than 0 to return results for all clinics.</summary>
		public static DataTable GetConfirmList(DateTime dateFrom,DateTime dateTo,long provNum,long clinicNum,bool showRecall,bool showNonRecall,bool showHygPresched,long confirmStatusDefNum,
			bool groupFamilies) 
			{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,provNum,clinicNum,showRecall,showNonRecall,showHygPresched,confirmStatusDefNum,groupFamilies);
			}
			DataTable table=new DataTable();
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("AddrNote");
			table.Columns.Add("AptNum");
			table.Columns.Add("age");
			table.Columns.Add("AptDateTime",typeof(DateTime));
			table.Columns.Add("ClinicNum");//patient.ClinicNum
			table.Columns.Add("confirmed");
			table.Columns.Add("contactMethod");
			table.Columns.Add("dateSched");
			table.Columns.Add("DateTimeAskedToArrive",typeof(DateTime));
			table.Columns.Add("email");//could be patient or guarantor email.
			table.Columns.Add("Guarantor");
			table.Columns.Add("guarClinicNum");
			table.Columns.Add("guarEmail");
			table.Columns.Add("guarNameF");
			table.Columns.Add("guarPreferConfirmMethod");
			table.Columns.Add("guarTxtMsgOK");
			table.Columns.Add("guarWirelessPhone");
			table.Columns.Add("medNotes");
			table.Columns.Add("FName");
			table.Columns.Add("LName");
			table.Columns.Add("Note");
			table.Columns.Add("PatNum");
			table.Columns.Add("PreferConfirmMethod");
			table.Columns.Add("Preferred");
			table.Columns.Add("ProcDescript");
			table.Columns.Add("TxtMsgOk");
			table.Columns.Add("WirelessPhone");
			List<DataRow> rows=new List<DataRow>();
			string command="SELECT patient.PatNum,patient.LName,patient.FName,patient.Preferred,patient.LName,patient.Guarantor,"
				+"AptDateTime,patient.Birthdate,patient.ClinicNum,patient.HmPhone,patient.TxtMsgOk,patient.WkPhone,"
				+"patient.WirelessPhone,appointment.ProcDescript,appointment.Confirmed,appointment.Note,patient.AddrNote,appointment.AptNum,patient.MedUrgNote,"
				+"patient.PreferConfirmMethod,guar.Email guarEmail,guar.WirelessPhone guarWirelessPhone,guar.TxtMsgOK guarTxtMsgOK,guar.ClinicNum guarClinicNum,"
				+"guar.PreferConfirmMethod guarPreferConfirmMethod,patient.Email,patient.Premed,appointment.DateTimeAskedToArrive,securitylog.LogDateTime,"
				+"guar.FName AS guarNameF "
				+"FROM patient "
				+"INNER JOIN appointment ON appointment.PatNum=patient.PatNum "
				+"INNER JOIN patient guar ON guar.PatNum=patient.Guarantor "
				+"LEFT JOIN securitylog ON securitylog.PatNum=appointment.PatNum AND securitylog.PermType="+POut.Int((int)Permissions.AppointmentCreate)+" "
					+"AND securitylog.FKey=appointment.AptNum ";
			if(groupFamilies) {
				command+="INNER JOIN ("
					+"SELECT patient.Guarantor,MAX(appointment.AptDateTime) LastAptDateTime "
					+"FROM patient "
					+"INNER JOIN appointment ON patient.PatNum=appointment.PatNum "
					+"WHERE AptDateTime > "+POut.Date(dateFrom)+" "
					+"AND AptDateTime < "+POut.Date(dateTo.AddDays(1))+" "
					+"AND AptStatus IN("+POut.Int((int)ApptStatus.Scheduled)+","+POut.Int((int)ApptStatus.ASAP)+") "
					+"GROUP BY patient.Guarantor"
				+") t ON t.Guarantor=patient.Guarantor ";
			}
			command+="WHERE AptDateTime > "+POut.Date(dateFrom)+" "
				//Example: AptDateTime="2014-11-26 13:00".  Filter is 11-26, giving "2014-11-27 00:00" to compare against.  This captures all times.
				+"AND AptDateTime < "+POut.Date(dateTo.AddDays(1))+" "
				+"AND AptStatus IN("+POut.Int((int)ApptStatus.Scheduled)+","+POut.Int((int)ApptStatus.ASAP)+") ";
			if(confirmStatusDefNum>0) {
				command+=" AND appointment.Confirmed="+POut.Long(confirmStatusDefNum)+" ";
			}
			if(provNum>0){
				command+="AND ((appointment.ProvNum="+POut.Long(provNum)+" AND appointment.IsHygiene=0) "//only include doc if it's not a hyg appt
					+" OR (appointment.ProvHyg="+POut.Long(provNum)+" AND appointment.IsHygiene=1)) ";//only include hygienists if it's a hygiene appt
			}
			if(clinicNum>=0) { //Only include appointments that belong to HQ clinic when clinics are enabled and no ClinicNum is specified.
				command+="AND appointment.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(showRecall && !showNonRecall && !showHygPresched) {//Show recall only (the All option was not selected)
				command+="AND appointment.AptNum IN ("
					+"SELECT DISTINCT procedurelog.AptNum FROM procedurelog "
					+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
					+"AND procedurecode.IsHygiene=1) "//recall appt if there is 1 or more procedure on the appt that is marked IsHygiene
					+"AND patient.PatNum IN ("
					+"SELECT DISTINCT procedurelog.PatNum "
					+"FROM procedurelog "
					+"WHERE procedurelog.ProcStatus=2) ";//and the patient has had a procedure completed in the office (i.e. not the patient's first appt)
			}
			else if(!showRecall && showNonRecall && !showHygPresched) {//Show non-recall only (the All option was not selected)
				command+="AND (appointment.AptNum NOT IN ("
					+"SELECT DISTINCT AptNum FROM procedurelog "
					+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
					+"AND procedurecode.IsHygiene=1) "//include if the appointment does not have a procedure marked IsHygiene
					+"OR patient.PatNum NOT IN ("
					+"SELECT DISTINCT procedurelog.PatNum "
					+"FROM procedurelog "
					+"WHERE procedurelog.ProcStatus=2)) ";//or if the patient has never had a completed procedure (new patient appts)
			}
			else if(!showRecall && !showNonRecall && showHygPresched) {//Show hygiene prescheduled only (the All option was not selected)
				//Example: LogDateTime="2014-11-26 13:00".  Filter is 11-26, giving "2014-11-27 00:00" to compare against.  This captures all times for 11-26.
				string aptDateSql="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					aptDateSql="DATE(appointment.AptDateTime-INTERVAL 2 MONTH)";
				}
				else {
					aptDateSql="ADD_MONTHS(TO_CHAR(appointment.AptDateTime,'MM/DD/YYYY %HH24:%MI:%SS'),-2)";
				}
				//Hygiene Prescheduled will consider both the IsHygiene flag on the appointment OR on any associated procedure codes.
				command+="AND (securitylog.PatNum IS NULL OR securitylog.LogDateTime < "+aptDateSql+") "
					+"AND (appointment.IsHygiene=1 "
					+"OR appointment.AptNum IN ("
						+"SELECT DISTINCT procedurelog.AptNum FROM procedurelog "
						+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
						+"AND procedurecode.IsHygiene=1)) ";
			}
			command+=groupFamilies?"ORDER BY LastAptDateTime, Guarantor":"ORDER BY AptDateTime";
			DataTable rawtable=Db.GetTable(command);
			ContactMethod contmeth;
			for(int i=0;i<rawtable.Rows.Count;i++) {
				row=table.NewRow();
				row["AddrNote"]=rawtable.Rows[i]["AddrNote"].ToString();
				row["AptNum"]=rawtable.Rows[i]["AptNum"].ToString();
				row["age"]=Patients.DateToAge(PIn.Date(rawtable.Rows[i]["Birthdate"].ToString())).ToString();//we don't care about m/y.
				row["AptDateTime"]=PIn.DateT(rawtable.Rows[i]["AptDateTime"].ToString());
				row["DateTimeAskedToArrive"]=PIn.DateT(rawtable.Rows[i]["DateTimeAskedToArrive"].ToString());
				row["ClinicNum"]=rawtable.Rows[i]["ClinicNum"].ToString();
				row["confirmed"]=Defs.GetName(DefCat.ApptConfirmed,PIn.Long(rawtable.Rows[i]["Confirmed"].ToString()));
				contmeth=(ContactMethod)PIn.Int(rawtable.Rows[i]["PreferConfirmMethod"].ToString());
				if(contmeth==ContactMethod.None || contmeth==ContactMethod.HmPhone) {
					row["contactMethod"]=Lans.g("FormConfirmList","Hm:")+rawtable.Rows[i]["HmPhone"].ToString();
				}
				if(contmeth==ContactMethod.WkPhone) {
					row["contactMethod"]=Lans.g("FormConfirmList","Wk:")+rawtable.Rows[i]["WkPhone"].ToString();
				}
				if(contmeth==ContactMethod.WirelessPh) {
					row["contactMethod"]=Lans.g("FormConfirmList","Cell:")+rawtable.Rows[i]["WirelessPhone"].ToString();
				}
				if(contmeth==ContactMethod.TextMessage) {
					row["contactMethod"]=Lans.g("FormConfirmList","Text:")+rawtable.Rows[i]["WirelessPhone"].ToString();
				}
				if(contmeth==ContactMethod.Email) {
					row["contactMethod"]=rawtable.Rows[i]["Email"].ToString();
				}
				if(contmeth==ContactMethod.DoNotCall || contmeth==ContactMethod.SeeNotes) {
					row["contactMethod"]=Lans.g("enumContactMethod",contmeth.ToString());
				}
				if(contmeth==ContactMethod.Mail) {
					row["contactMethod"]=Lans.g("FormConfirmList","Mail");
				}
				if(groupFamilies) {//Grouped families need to display guarantor information for specfic contact methods instead
					contmeth=(ContactMethod)PIn.Int(rawtable.Rows[i]["guarPreferConfirmMethod"].ToString());
					if(contmeth==ContactMethod.WirelessPh) {
						row["contactMethod"]=Lans.g("FormConfirmList","Cell:")+rawtable.Rows[i]["guarWirelessPhone"].ToString();
					}
					if(contmeth==ContactMethod.TextMessage) {
						row["contactMethod"]=Lans.g("FormConfirmList","Text:")+rawtable.Rows[i]["guarWirelessPhone"].ToString();
					}
					if(contmeth==ContactMethod.Email) {
						row["contactMethod"]=rawtable.Rows[i]["guarEmail"].ToString();
					}
				}
				row["dateSched"]="Unknown";
				if(rawtable.Rows[i]["LogDateTime"].ToString().Length>0) {
					row["dateSched"]=rawtable.Rows[i]["LogDateTime"].ToString();
				}
				if(rawtable.Rows[i]["Email"].ToString()=="" && rawtable.Rows[i]["guarEmail"].ToString()!="") {
					row["email"]=rawtable.Rows[i]["guarEmail"].ToString();
				}
				else {
					row["email"]=rawtable.Rows[i]["Email"].ToString();
				}
				row["Guarantor"]=rawtable.Rows[i]["Guarantor"].ToString();
				row["guarClinicNum"]=rawtable.Rows[i]["guarClinicNum"].ToString();
				row["guarEmail"]=rawtable.Rows[i]["guarEmail"].ToString();
				row["guarNameF"]=rawtable.Rows[i]["guarNameF"].ToString();
				row["guarPreferConfirmMethod"]=rawtable.Rows[i]["guarPreferConfirmMethod"].ToString();
				row["guarTxtMsgOK"]=rawtable.Rows[i]["guarTxtMsgOK"].ToString();
				row["guarWirelessPhone"]=rawtable.Rows[i]["guarWirelessPhone"].ToString();
				row["medNotes"]="";
				if(rawtable.Rows[i]["Premed"].ToString()=="1"){
					row["medNotes"]=Lans.g("FormConfirmList","Premedicate");
				}
				if(rawtable.Rows[i]["MedUrgNote"].ToString()!=""){
					if(row["medNotes"].ToString()!="") {
						row["medNotes"]+="\r\n";
					}
					row["medNotes"]+=rawtable.Rows[i]["MedUrgNote"].ToString();
				}
				row["FName"]=rawtable.Rows[i]["FName"].ToString();
				row["LName"]=rawtable.Rows[i]["LName"].ToString();
				row["Note"]=rawtable.Rows[i]["Note"].ToString();
				row["PatNum"]=rawtable.Rows[i]["PatNum"].ToString();
				row["PreferConfirmMethod"]=rawtable.Rows[i]["PreferConfirmMethod"].ToString();
				row["Preferred"]=rawtable.Rows[i]["Preferred"].ToString();
				row["ProcDescript"]=rawtable.Rows[i]["ProcDescript"].ToString();
				row["TxtMsgOk"]=rawtable.Rows[i]["TxtMsgOk"].ToString();
				row["WirelessPhone"]=rawtable.Rows[i]["WirelessPhone"].ToString();
				rows.Add(row);
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		public static DataTable GetAddrTableStructure() {
			DataTable table=new DataTable();
			table.Columns.Add("Address");//Can be guar.
			table.Columns.Add("Address2");//Can be guar.
			table.Columns.Add("AptDateTime");
			table.Columns.Add("City");//Can be guar.
			table.Columns.Add("clinicNum");//will be the guar clinicNum if grouped.
			table.Columns.Add("DateTimeAskedToArrive");
			table.Columns.Add("email");//Will be guar if grouped by family
			table.Columns.Add("famList");
			table.Columns.Add("guarLName");
			table.Columns.Add("FName");
			table.Columns.Add("LName");
			table.Columns.Add("MiddleI");
			table.Columns.Add("patNums");//Comma delimited.  Used in email.
			table.Columns.Add("PatNum");
			table.Columns.Add("Preferred");
			table.Columns.Add("State");//Can be guar.
			table.Columns.Add("Zip");//Can be guar.
			return table;
		}

		///<summary>Used in Confirm list to just get addresses.</summary>
		public static DataTable GetAddrTable(List<long> aptNums, bool groupByFamily) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),aptNums,groupByFamily);
			}
			DataTable table=GetAddrTableStructure();
			if(aptNums.IsNullOrEmpty()) {
				return table;
			}
			string command = "SELECT patient.LName,patient.FName,patient.MiddleI,patient.Preferred, patient.Address,patient.Address2,patient.City,"
				+"patient.State,patient.Zip,patient.Guarantor,patient.Email,appointment.AptDateTime,appointment.ClinicNum,patient.PatNum,"
				+"appointment.DateTimeAskedToArrive,guar.Address AS guarAddress,guar.Address2 AS guarAddress2,"
				+"guar.ClinicNum AS guarClinicNum,guar.FName AS guarFName,guar.LName AS guarLName,guar.State AS guarState,"
				+"guar.Zip AS guarZip, guar.City AS guarCity, guar.Email as guarEmail "
				+"FROM appointment "
				+"INNER JOIN patient ON patient.PatNum=appointment.PatNum "
				+"INNER JOIN patient guar ON patient.Guarantor=guar.PatNum "
				+"WHERE appointment.AptNum IN ("+string.Join(",",aptNums.Select(x => POut.Long(x)))+") "
				+"ORDER BY "+(groupByFamily?"Guarantor,":"")+"appointment.AptDateTime";
			string familyAptList="";
			string patNumStr="";
			DataTable rawTable=Db.GetTable(command);
			DataRow row;
			List<DataRow> rows=new List<DataRow>();
			for(int i=0;i<rawTable.Rows.Count;i++) {
				Patient pat=new Patient {
					PatNum=PIn.Long(rawTable.Rows[i]["PatNum"].ToString()),
					FName=PIn.String(rawTable.Rows[i]["FName"].ToString()),
					Preferred=PIn.String(rawTable.Rows[i]["Preferred"].ToString()),
					MiddleI=PIn.String(rawTable.Rows[i]["MiddleI"].ToString()),
					LName=PIn.String(rawTable.Rows[i]["LName"].ToString()),
				};
				DateTime aptDateTime=PIn.Date(rawTable.Rows[i]["AptDateTime"].ToString());
				DateTime dateTimeAskedToArrive=PIn.Date(rawTable.Rows[i]["DateTimeAskedToArrive"].ToString());
				if(!groupByFamily) {
					row=table.NewRow();
					row["Address"]=rawTable.Rows[i]["Address"].ToString();
					if(rawTable.Rows[i]["Address2"].ToString()!="") {
						row["Address2"]=rawTable.Rows[i]["Address2"].ToString();
					}
					row["City"]=rawTable.Rows[i]["City"].ToString();
					row["clinicNum"]=rawTable.Rows[i]["ClinicNum"].ToString();
					row["AptDateTime"]=aptDateTime;
					row["DateTimeAskedToArrive"]=dateTimeAskedToArrive;
					//since not grouping by family, this is always just the patient email
					row["email"]=rawTable.Rows[i]["Email"].ToString();
					row["famList"]="";
					row["guarLName"]=rawTable.Rows[i]["guarLName"].ToString();
					row["FName"]=pat.FName;
					row["LName"]=pat.LName;
					row["MiddleI"]=pat.MiddleI;
					row["patNums"]=pat.PatNum;
					row["PatNum"]=pat.PatNum;
					row["Preferred"]=pat.Preferred;
					row["State"]=rawTable.Rows[i]["State"].ToString();
					row["Zip"]=rawTable.Rows[i]["Zip"].ToString();
					table.Rows.Add(row);
					continue;
				}
				//groupByFamily----------------------------------------------------------------------
				if(familyAptList=="") {//if this is the first patient in the family
					if(i==rawTable.Rows.Count-1//if this is the last row
						|| rawTable.Rows[i]["Guarantor"].ToString()!=rawTable.Rows[i+1]["Guarantor"].ToString())//or if the guarantor on next line is different
					{
						//then this is a single patient, and there are no other family members in the list.
						row=table.NewRow();
						row["Address"]=rawTable.Rows[i]["Address"].ToString();
						if(rawTable.Rows[i]["Address2"].ToString()!="") {
							row["Address2"]=rawTable.Rows[i]["Address2"].ToString();
						}
						row["City"]=rawTable.Rows[i]["City"].ToString();
						row["State"]=rawTable.Rows[i]["State"].ToString();
						row["Zip"]=rawTable.Rows[i]["Zip"].ToString();
						row["clinicNum"]=rawTable.Rows[i]["ClinicNum"].ToString();
						row["AptDateTime"]=aptDateTime;
						row["DateTimeAskedToArrive"]=dateTimeAskedToArrive;
						//this will always be the guarantor email
						row["email"]=rawTable.Rows[i]["guarEmail"].ToString();
						row["famList"]="";
						row["guarLName"]=rawTable.Rows[i]["guarLName"].ToString();
						row["FName"]=pat.FName;
						row["LName"]=pat.LName;
						row["MiddleI"]=pat.MiddleI;
						row["patNums"]=pat.PatNum;
						row["PatNum"]=pat.PatNum;
						row["Preferred"]=pat.Preferred;
						table.Rows.Add(row);
						continue;
					}
					else{//this is the first patient of a family with multiple family members
						familyAptList=PatComm.BuildAppointmentMessage(pat,dateTimeAskedToArrive,aptDateTime);
						patNumStr=rawTable.Rows[i]["PatNum"].ToString();
						continue;
					}
				}
				else{//not the first patient
					familyAptList+="\r\n"+PatComm.BuildAppointmentMessage(pat,dateTimeAskedToArrive,aptDateTime);
					patNumStr+=","+rawTable.Rows[i]["PatNum"].ToString();
				}
				if(i==rawTable.Rows.Count-1//if this is the last row
					|| rawTable.Rows[i]["Guarantor"].ToString()!=rawTable.Rows[i+1]["Guarantor"].ToString())//or if the guarantor on next line is different
				{
					//This part only happens for the last family member of a grouped family
					row=table.NewRow();
					row["Address"]=rawTable.Rows[i]["guarAddress"].ToString();
					if(rawTable.Rows[i]["guarAddress2"].ToString()!="") {
						row["Address2"]+=rawTable.Rows[i]["guarAddress2"].ToString();
					}
					row["City"]=rawTable.Rows[i]["guarCity"].ToString();
					row["State"]=rawTable.Rows[i]["guarState"].ToString();
					row["Zip"]=rawTable.Rows[i]["guarZip"].ToString();
					row["clinicNum"]=rawTable.Rows[i]["guarClinicNum"].ToString();
					row["AptDateTime"]=aptDateTime;
					row["DateTimeAskedToArrive"]=dateTimeAskedToArrive;
					row["email"]=rawTable.Rows[i]["guarEmail"].ToString();
					row["famList"]=familyAptList;
					row["guarLName"]=rawTable.Rows[i]["guarLName"].ToString();
					row["FName"]=pat.FName;
					row["LName"]=pat.LName;
					row["MiddleI"]=pat.MiddleI;
					row["patNums"]=patNumStr;
					row["PatNum"]=pat.PatNum;
					row["Preferred"]=pat.Preferred;
					table.Rows.Add(row);
					familyAptList="";
				}	
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>Manipulates ProcDescript and ProcsColored on the appointment passed in.  Does not update the database.
		///Pass in the list of procs attached to the appointment to avoid a db call to the procedurelog table.</summary>
		public static void SetProcDescript(Appointment apt,List<Procedure> listProcs = null) {
			apt.ProcDescript="";
			apt.ProcsColored="";
			if(listProcs == null) {
				listProcs=Procedures.GetProcsForSingle(apt.AptNum,apt.AptStatus == ApptStatus.Planned);
			}
			List<ProcedureCode> listProcCodes = ProcedureCodes.GetCodesForCodeNums(listProcs.Select(x => x.CodeNum).ToList());
			foreach(Procedure proc in listProcs) {
				if(apt.AptStatus==ApptStatus.Planned && apt.AptNum != proc.PlannedAptNum) {
					continue;
				}
				if(apt.AptStatus != ApptStatus.Planned && apt.AptNum != proc.AptNum) {
					continue;
				}
				string procDescOne = "";
				ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				if(!string.IsNullOrEmpty(apt.ProcDescript)) {
					apt.ProcDescript+=", ";
				}
				string displaySurf=Tooth.SurfTidyFromDbToDisplay(proc.Surf,proc.ToothNum);//Fixes surface display for Canadian users
				switch(procCode.TreatArea) {
					case TreatmentArea.Surf:
						procDescOne+="#"+Tooth.GetToothLabel(proc.ToothNum)+"-"+displaySurf+"-";//""#12-MOD-"
						break;
					case TreatmentArea.Tooth:
						procDescOne+="#"+Tooth.GetToothLabel(proc.ToothNum)+"-";//"#12-"
						break;
					default://area 3 or 0 (mouth)
						break;
					case TreatmentArea.Quad:
						procDescOne+=displaySurf+"-";//"UL-"
						break;
					case TreatmentArea.Sextant:
						procDescOne+="S"+displaySurf+"-";//"S2-"
						break;
					case TreatmentArea.Arch:
						procDescOne+=displaySurf+"-";//"U-"
						break;
					case TreatmentArea.ToothRange:
						//strLine+=table.Rows[j][13].ToString()+" ";//don't show range
						break;
				}
				procDescOne+=procCode.AbbrDesc;
				apt.ProcDescript+=procDescOne;
				//Color and previous date are determined by ProcApptColor object
				ProcApptColor pac = ProcApptColors.GetMatch(procCode.ProcCode);
				System.Drawing.Color pColor = System.Drawing.Color.Black;
				string prevDateString = "";
				if(pac!=null) {
					pColor=pac.ColorText;
					if(pac.ShowPreviousDate) {
						prevDateString=Procedures.GetRecentProcDateString(apt.PatNum,apt.AptDateTime,pac.CodeRange);
						if(prevDateString!="") {
							prevDateString=" ("+prevDateString+")";
						}
					}
				}
				apt.ProcsColored+="<span color=\""+pColor.ToArgb().ToString()+"\">"+procDescOne+prevDateString+"</span>";
			}
		}

		///<summary>Returns a list of special appointment objects that only contain information necessary to fill the OtherAppts window.
		///The main purpose of this method is to significantly cut back on the amount of data sent back to the client.</summary>
		public static List<ApptOther> GetApptOthersForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Remoting role check in order to limit the amount of data sent back to the client.
				return Meth.GetObject<List<ApptOther>>(MethodBase.GetCurrentMethod(),patNum);
			}
			Appointment[] listAppts=GetForPat(patNum);
			return listAppts.Select(x => new ApptOther(x)).ToList();
		}

		///<summary>Returns all appointments for the given patient, ordered from earliest to latest.
		///Used in statements, appt cards, OtherAppts window, etc.</summary>
		public static Appointment[] GetForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Appointment[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM appointment "
				+"WHERE PatNum = '"+POut.Long(patNum)+"' "
				+"AND NOT (AptDateTime < "+POut.Date(new DateTime(1880,1,1))+" AND AptStatus="+POut.Int((int)ApptStatus.UnschedList)+") "//AND NOT (on the pinboard)
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command).ToArray();
		}

		///<summary>Gets all appointments for a single patient ordered by AptDateTime.</summary>
		public static List<Appointment> GetListForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM appointment "
				+"WHERE patnum = '"+POut.Long(patNum)+"' "
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Gets one appointment from db.  Returns null if not found.</summary>
		public static Appointment GetOneApt(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Appointment>(MethodBase.GetCurrentMethod(),aptNum);
			}
			if(aptNum==0) {
				return null;
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptNum = '"+POut.Long(aptNum)+"'";
			return Crud.AppointmentCrud.SelectOne(command);
		}

		///<summary>Gets one AppointForApi from db. Returns null if not found.</summary>
		public static AppointmentForApi GetOneAptForApi(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<AppointmentForApi>(MethodBase.GetCurrentMethod(),aptNum);
			}
			if(aptNum==0) {
				return null;
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptNum = '"+POut.Long(aptNum)+"'";
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before appts for rigorous inclusion of appts
			AppointmentForApi appointmentForApi=new AppointmentForApi();
			appointmentForApi.AppointmentCur=Crud.AppointmentCrud.SelectOne(command);
			appointmentForApi.DateTimeServer=dateTimeServer;
			return appointmentForApi;
		}

		///<summary>Gets an appointment (of any status) from the db with this NextAptNum (FK to the AptNum of a planned appt).</summary>
		public static Appointment GetScheduledPlannedApt(long nextAptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Appointment>(MethodBase.GetCurrentMethod(),nextAptNum);
			}
			if(nextAptNum==0) {
				return null;
			}
			string command="SELECT * FROM appointment "
				+"WHERE NextAptNum = '"+POut.Long(nextAptNum)+"'";
			return Crud.AppointmentCrud.SelectOne(command);
		}

		///<summary>Gets a list of all future appointments which are scheduled.  Ordered by dateTime</summary>
		public static List<Appointment> GetFutureSchedApts(long patNum) {
			return GetFutureSchedApts(new List<long> { patNum });
		}

		///<summary>Gets a list of all future appointments which are scheduled.  Ordered by dateTime</summary>
		public static List<Appointment> GetFutureSchedApts(List<long> listPatNums) {
			if(listPatNums.Count==0) {
				return new List<Appointment>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			string command="SELECT * FROM appointment "
				+"WHERE PatNum IN ("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+") "
				+"AND AptDateTime > "+DbHelper.Now()+" "
				+"AND AptStatus = "+(int)ApptStatus.Scheduled+" "
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Gets a list of all future appointments which are either sched or ASAP for all patients.  Ordered by dateTime</summary>
		public static List<Appointment> GetFutureSchedApts() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime > "+DbHelper.Now()+" "
				+"AND AptStatus = "+(int)ApptStatus.Scheduled+" "
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		public static List<Appointment> GetChangedSince(DateTime changedSince,DateTime excludeOlderThan) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),changedSince,excludeOlderThan);
			}
			string command="SELECT * FROM appointment WHERE DateTStamp > "+POut.DateT(changedSince)
				+" AND AptDateTime > "+POut.DateT(excludeOlderThan);
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Used if the number of records are very large, in which case using GetChangedSince is not the preffered route due to memory problems caused by large recordsets. </summary>
		public static List<long> GetChangedSinceAptNums(DateTime changedSince,DateTime excludeOlderThan) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince,excludeOlderThan);
			}
			string command="SELECT AptNum FROM appointment WHERE DateTStamp > "+POut.DateT(changedSince)
				+" AND AptDateTime > "+POut.DateT(excludeOlderThan);
			DataTable dt=Db.GetTable(command);
			List<long> aptnums = new List<long>(dt.Rows.Count);
			for(int i=0;i<dt.Rows.Count;i++) {
				aptnums.Add(PIn.Long(dt.Rows[i]["AptNum"].ToString()));
			}
			return aptnums;
		}

		///<summary>Used along with GetChangedSinceAptNums</summary>
		public static List<Appointment> GetMultApts(List<long> aptNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),aptNums);
			}
			if(aptNums.Count < 1) {
				return new List<Appointment>();
			}
			string command="";
			command="SELECT * FROM appointment WHERE AptNum IN ("+string.Join(",",aptNums)+")";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Gets AptNums and AptDateTimes to use for task sorting with the TaskUseApptDate pref.</summary>
		public static DataTable GetAptDateTimeForAptNums(List<long> listAptNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listAptNums);
			}
			if(listAptNums.Count==0) {
				return new DataTable();
			}
			string command="SELECT AptNum, AptDateTime FROM appointment WHERE AptNum IN ("+string.Join(",",listAptNums)+")";
			return Db.GetTable(command);
		}

		///<summary>Gets a list of appointments for a period of time in the schedule, whether hidden or not.</summary>
		public static Appointment[] GetForPeriod(DateTime startDate,DateTime endDate){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Appointment[]>(MethodBase.GetCurrentMethod(),startDate,endDate);
			}
			//DateSelected = thisDay;
			string command=
				"SELECT * from appointment "
				+"WHERE AptDateTime BETWEEN "+POut.Date(startDate)+" AND "+POut.Date(endDate.AddDays(1))+" "
				+"AND aptstatus != '"+(int)ApptStatus.UnschedList+"' "
				+"AND aptstatus != '"+(int)ApptStatus.Planned+"'";
			return Crud.AppointmentCrud.SelectMany(command).ToArray();
		}

		///<summary>Gets appointments for a period of time, whether hidden or not.  Optionally pass in a clinic to only get appointments associated to that clinic.  clinicNum of 0 will get all appointments.</summary>
		public static List<Appointment> GetForPeriodList(DateTime startDate,DateTime endDate,long clinicNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),startDate,endDate,clinicNum);
			}
			//DateSelected = thisDay;
			string command=
				"SELECT * FROM appointment "
				+"WHERE AptDateTime BETWEEN "+POut.Date(startDate)+" AND "+POut.Date(endDate.AddDays(1))+" "
				+"AND AptStatus != '"+(int)ApptStatus.UnschedList+"' "
				+"AND AptStatus != '"+(int)ApptStatus.Planned+"' "
				+(clinicNum > 0 ? "AND ClinicNum="+POut.Long(clinicNum) : "");
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Used by API.</summary>
		public static List<Appointment> GetForProv(DateTime startDate,DateTime endDate,long provNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),startDate,endDate,provNum);
			}
			string command=
				"SELECT * FROM appointment "
				+"WHERE AptDateTime BETWEEN "+POut.Date(startDate)+" AND "+POut.Date(endDate.AddDays(1))+" "//between is inclusive. Midnight to midnight
				+"AND AptStatus != '"+(int)ApptStatus.UnschedList+"' "
				+"AND AptStatus != '"+(int)ApptStatus.Planned+"' "
				+"AND ProvNum="+POut.Long(provNum);
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Overload. Gets a List of appointments for a period of time in the schedule, whether hidden or not.
		///Optionally pass in a list of clinics to only get appointments associated to those clinics.
		///An empty list of ClinicNums will get all appointments.</summary>
		public static List<Appointment> GetForPeriodList(DateTime startDate,DateTime endDate,List<long> listOpNums,List<long> listClinicNums) {
			if(listOpNums==null || listOpNums.Count < 1) {
				return new List<Appointment>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),startDate,endDate,listOpNums,listClinicNums);
			}
			string command=
				"SELECT * FROM appointment "
				+"WHERE appointment.AptDateTime BETWEEN "+POut.Date(startDate)+" AND "+POut.Date(endDate.AddDays(1))+" "
				+"AND appointment.AptStatus != '"+(int)ApptStatus.UnschedList+"' "
				+"AND appointment.AptStatus != '"+(int)ApptStatus.Planned+"' "
				+"AND appointment.Op IN ("+string.Join(",",listOpNums)+") ";
			if(listClinicNums.Count>0) {
				command+="AND appointment.ClinicNum IN ("+string.Join(",",listClinicNums)+")";
			}
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>A list of strings.  Each string corresponds to one appointment in the supplied list.  Each string is a comma delimited list of codenums of the procedures attached to the appointment.</summary>
		public static List<string> GetUAppointProcs(List<Appointment> appts){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),appts);
			}
			List<string> retVal=new List<string>();
			if(appts.Count==0){
				return retVal;
			}
			string command="SELECT AptNum,CodeNum FROM procedurelog WHERE AptNum IN(";
			for(int i=0;i<appts.Count;i++){
				if(i>0){
					command+=",";
				}
				command+=POut.Long(appts[i].AptNum);
			}
			command+=")";
			DataTable table=Db.GetTable(command);
			string str;
			for(int i=0;i<appts.Count;i++){
				str="";
				for(int p=0;p<table.Rows.Count;p++){
					if(table.Rows[p]["AptNum"].ToString()==appts[i].AptNum.ToString()){
						if(str!=""){
							str+=",";
						}
						str+=table.Rows[p]["CodeNum"].ToString();
					}
				}
				retVal.Add(str);
			}
			return retVal;
		}

		///<summary>Returns list of appointments for today for one patient. This ignores unscheduled/broken appointments.</summary>
		public static List<Appointment> GetTodaysApptsForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			List<Appointment> listAppts=new List<Appointment>();
			string command=$"SELECT * FROM appointment WHERE PatNum={POut.Long(patNum)} " +
				$"AND {DbHelper.BetweenDates("AptDateTime",DateTime.Today,DateTime.Today)} "+
				$"AND AptStatus NOT IN({POut.Int((int)ApptStatus.UnschedList)},{POut.Int((int)ApptStatus.Broken)})";
			listAppts=Crud.AppointmentCrud.SelectMany(command);
			return listAppts;
		}

		///<summary>Gets list of asap AppointmentsForApi. Pass in a clinicNum less than 0 to get ASAP appts for all clinics.</summary>
		public static List<AppointmentForApi> RefreshAsapForApi(long provNum,long siteNum,long clinicNum,List<ApptStatus> listStatuses,string codeRangeStart="",
			string codeRangeEnd="") 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AppointmentForApi>>(MethodBase.GetCurrentMethod(),provNum,siteNum,clinicNum,listStatuses,codeRangeStart,codeRangeEnd);
			}
			List<long> listCodeNums=new List<long>();
			if(!string.IsNullOrEmpty(codeRangeStart)) {
				//Get a list of CodeNums that meet the procedure code range passed in.
				listCodeNums=Db.GetListLong(
					"SELECT CodeNum FROM procedurecode "
					+"WHERE ProcCode BETWEEN '"+POut.String(codeRangeStart)+"' AND '"+POut.String(codeRangeEnd)+"' ");
				if(listCodeNums.Count==0) { //ProcCodes do not exist.
					return new List<AppointmentForApi>();
				}
			}
			string command="SELECT appointment.* FROM appointment ";
			if(siteNum>0) {
				command+="LEFT JOIN patient ON patient.PatNum=appointment.PatNum ";
			}
			command+="WHERE appointment.Priority="+POut.Int((int)ApptPriority.ASAP)+" ";
			if(provNum>0) {
				command+="AND (appointment.ProvNum="+POut.Long(provNum)+" OR appointment.ProvHyg="+POut.Long(provNum)+") ";
			}
			if(siteNum>0) {
				command+="AND patient.SiteNum="+POut.Long(siteNum)+" ";
			}
			if(clinicNum>=0) { //Only include appointments that belong to HQ clinic when clinics are enabled and no ClinicNum is specified.
				command+="AND appointment.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(listStatuses.Count>0) {
				command+="AND appointment.AptStatus IN ("+string.Join(",",listStatuses.Select(x => POut.Int((int)x)))+") ";
			}
			else {
				command+="AND appointment.AptStatus IN("
					+POut.Int((int)ApptStatus.Scheduled)+","
					+POut.Int((int)ApptStatus.UnschedList)+","
					+POut.Int((int)ApptStatus.Broken)+","
					+POut.Int((int)ApptStatus.Planned)+") ";
			}
			//If a planned appointment has been scheduled, don't show that planned appointment in the list.
			command+="AND NOT EXISTS(SELECT * FROM appointment a2 WHERE a2.NextAptNum=appointment.AptNum AND appointment.AptStatus="
				+POut.Int((int)ApptStatus.Planned)+") "
			+"GROUP BY appointment.AptNum "
			+"ORDER BY appointment.AptDateTime";
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before appts for rigorous inclusion of appts
			List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
			if(listCodeNums.Count>0 && listAppointments.Count>0) {
				//Get every procedure's CodeNum and it's corresponding AptNum for all appointments in listAppts.
				command="SELECT procedurelog.AptNum,procedurelog.CodeNum "
					+"FROM procedurelog "
					+"WHERE procedurelog.AptNum IN("+string.Join(",",listAppointments.Select(x => POut.Long(x.AptNum)))+") "
					+"GROUP BY procedurelog.AptNum,procedurelog.CodeNum";
				//Sam and Saul tried to speed this up many different ways. This was the best way to make sure we always use indexes on procedurelog.
				var listFilteredAptNum=Db.GetTable(command).AsEnumerable()
					.Select(x => new {
						AptNum=PIn.Long(x["AptNum"].ToString()),
						CodeNum=PIn.Long(x["CodeNum"].ToString()),
					})
					//We only care about code nums that were included in the range provided.
					.Where(x => listCodeNums.Any(y => y==x.CodeNum))
					.Select(x => x.AptNum).ToList();
				//Remove the appointments that are not in the list of filtered AptNums.
				listAppointments.RemoveAll(x => !listFilteredAptNum.Contains(x.AptNum));
			}
			List<AppointmentForApi> listAppointmentForApis=new List<AppointmentForApi>();
			for(int i=0;i<listAppointments.Count;i++) {//list can be empty
				AppointmentForApi appointmentForApi=new AppointmentForApi();
				appointmentForApi.AppointmentCur=listAppointments[i];
				appointmentForApi.DateTimeServer=dateTimeServer;
				listAppointmentForApis.Add(appointmentForApi);
			}
			return listAppointmentForApis;
		}

		///<summary>Gets list of asap Appointments. Pass in a clinicNum less than 0 to get ASAP appts for all clinics.</summary>
		public static List<Appointment> RefreshASAP(long provNum,long siteNum,long clinicNum,List<ApptStatus> listStatuses,string codeRangeStart="",
			string codeRangeEnd="") 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),provNum,siteNum,clinicNum,listStatuses,codeRangeStart,codeRangeEnd);
			}
			List<long> listCodeNums=new List<long>();
			if(!string.IsNullOrEmpty(codeRangeStart)) {
				//Get a list of CodeNums that meet the procedure code range passed in.
				listCodeNums=Db.GetListLong(
					"SELECT CodeNum FROM procedurecode "
					+"WHERE ProcCode BETWEEN '"+POut.String(codeRangeStart)+"' AND '"+POut.String(codeRangeEnd)+"' ");
				if(listCodeNums.Count==0) { //ProcCodes do not exist.
					return new List<Appointment>();
				}
			}
			string command="SELECT appointment.* FROM appointment ";
			if(siteNum>0) {
				command+="LEFT JOIN patient ON patient.PatNum=appointment.PatNum ";
			}
			command+="WHERE appointment.Priority="+POut.Int((int)ApptPriority.ASAP)+" ";
			if(provNum>0) {
				command+="AND (appointment.ProvNum="+POut.Long(provNum)+" OR appointment.ProvHyg="+POut.Long(provNum)+") ";
			}
			if(siteNum>0) {
				command+="AND patient.SiteNum="+POut.Long(siteNum)+" ";
			}
			if(clinicNum>=0) { //Only include appointments that belong to HQ clinic when clinics are enabled and no ClinicNum is specified.
				command+="AND appointment.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(listStatuses.Count>0) {
				command+="AND appointment.AptStatus IN ("+string.Join(",",listStatuses.Select(x => POut.Int((int)x)))+") ";
			}
			else {
				command+="AND appointment.AptStatus IN("
					+POut.Int((int)ApptStatus.Scheduled)+","
					+POut.Int((int)ApptStatus.UnschedList)+","
					+POut.Int((int)ApptStatus.Broken)+","
					+POut.Int((int)ApptStatus.Planned)+") ";
			}
			//If a planned appointment has been scheduled, don't show that planned appointment in the list.
			command+="AND NOT EXISTS(SELECT * FROM appointment a2 WHERE a2.NextAptNum=appointment.AptNum AND appointment.AptStatus="
				+POut.Int((int)ApptStatus.Planned)+") "
			+"GROUP BY appointment.AptNum "
			+"ORDER BY appointment.AptDateTime";
			List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
			if(listCodeNums.Count>0 && listAppointments.Count>0) {
				//Get every procedure's CodeNum and it's corresponding AptNum for all appointments in listAppts.
				command="SELECT procedurelog.AptNum,procedurelog.CodeNum "
					+"FROM procedurelog "
					+"WHERE procedurelog.AptNum IN("+string.Join(",",listAppointments.Select(x => POut.Long(x.AptNum)))+") "
					+"GROUP BY procedurelog.AptNum,procedurelog.CodeNum";
				//Sam and Saul tried to speed this up many different ways. This was the best way to make sure we always use indexes on procedurelog.
				var listFilteredAptNum=Db.GetTable(command).AsEnumerable()
					.Select(x => new {
						AptNum=PIn.Long(x["AptNum"].ToString()),
						CodeNum=PIn.Long(x["CodeNum"].ToString()),
					})
					//We only care about code nums that were included in the range provided.
					.Where(x => listCodeNums.Any(y => y==x.CodeNum))
					.Select(x => x.AptNum).ToList();
				//Remove the appointments that are not in the list of filtered AptNums.
				listAppointments.RemoveAll(x => !listFilteredAptNum.Contains(x.AptNum));
			}
			return listAppointments;
		}

		///<summary>Set clinicNum to 0 to return 'all' clinics.  Otherwise, filters the data set on the clinic num passed in.  
		///Currently only filters GetPeriodEmployeeSchedTable()
		///Any ApptNums within listPinApptNums will get forcefully added to the DataSet.
		///If listOpNums and listProvNums are null then we do not filter the tableAppt based on visible ops and provs for the appt view.
		///No longer being used by ContrAppt. Might be used in eServices?</summary>
		public static DataSet RefreshPeriod(DateTime dateStart,DateTime dateEnd,long clinicNum,List<long> listPinApptNums=null,List<long> listOpNums=null,List<long> listProvNums=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),dateStart,dateEnd,clinicNum,listPinApptNums,listOpNums,listProvNums);
			} 
			DataSet retVal=new DataSet();
			DataTable tableAppt=GetPeriodApptsTable(dateStart,dateEnd,0,false,listPinApptNums,listOpNums,listProvNums);
			retVal.Tables.Add(tableAppt);
			retVal.Tables.Add(Schedules.GetPeriodEmployeeSchedTable(dateStart,dateEnd,clinicNum));
			retVal.Tables.Add(Schedules.GetPeriodProviderSchedTable(dateStart,dateEnd,clinicNum));
			//retVal.Tables.Add(GetPeriodWaitingRoomTable(clinicNum));
			retVal.Tables.Add(GetPeriodWaitingRoomTable(DateTime.Now));
			retVal.Tables.Add(Schedules.GetPeriodSchedule(dateStart,dateEnd,listOpNums));
			retVal.Tables.Add(GetApptFields(tableAppt));
			retVal.Tables.Add(GetPatFields(tableAppt.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList()));
			return retVal;
		}

		///<summary>Allowed orderby: status, alph, date. Pass in a clinicNum less than 0 to get all appointments regardless of clinic.</summary>
		public static List<Appointment> RefreshPlannedTracker(string orderby,long provNum,long siteNum,long clinicNum,string codeStart,string codeEnd,DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),orderby,provNum,siteNum,clinicNum,codeStart,codeEnd,dateStart,dateEnd);
			}
			//We create a in-memory temporary table by joining the appointment and patient
			//tables to get a list of planned appointments for active paients, then we
			//perform a left join on that temporary table against the appointment table
			//to exclude any appointments in the temporary table which are already refereced
			//by the NextAptNum column by any other appointment within the appointment table.
			//Using an in-memory temporary table reduces the number of row comparisons performed for
			//this query overall as compared to left joining the appointment table onto itself,
			//because the in-memory temporary table has many fewer rows than the appointment table
			//on average.
			string command="SELECT tplanned.* "
				+"FROM (SELECT a.* FROM appointment a "
				+"INNER JOIN patient p ON p.PatNum=a.PatNum ";
			if(!string.IsNullOrEmpty(codeStart)) {
				command+="INNER JOIN ( "
						+"SELECT procedurelog.PlannedAptNum "
						+"FROM procedurelog "
						+"INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
						+"AND procedurecode.ProcCode >= '"+POut.String(codeStart)+"' "
						+"AND procedurecode.ProcCode <= '"+POut.String(codeEnd)+"' "
						+"WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP)+" "
						+"AND procedurelog.PlannedAptNum!=0 "
						+"AND procedurelog.ProcDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)
						+"GROUP BY procedurelog.PlannedAptNum "
					+")ProcCheck ON ProcCheck.PlannedAptNum=a.AptNum ";
			}
			command+="WHERE a.AptStatus="+POut.Long((int)ApptStatus.Planned)
				+" AND p.PatStatus="+POut.Long((int)PatientStatus.Patient)+" ";
			if(provNum>0) {
				command+="AND (a.ProvNum="+POut.Long(provNum)+" OR a.ProvHyg="+POut.Long(provNum)+") ";
			}
			if(siteNum>0) {
				command+="AND p.SiteNum="+POut.Long(siteNum)+" ";
			}
			if(clinicNum>=0) { //Only include appointments that belong to HQ clinic when clinics are enabled and no ClinicNum is specified.
				command+="AND a.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			command+="AND "+DbHelper.DtimeToDate("a.AptDateTime")+" BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd);
			if(orderby=="status") {
				command+="ORDER BY a.UnschedStatus,a.AptDateTime";
			} 
			else if(orderby=="alph") {
				command+="ORDER BY p.LName,p.FName";
			} 
			else { //if(orderby=="date"){
				command+="ORDER BY a.AptDateTime";
			}
			command+=") tplanned "
				+"LEFT JOIN appointment tregular ON tplanned.AptNum=tregular.NextAptNum "
				+"WHERE tregular.NextAptNum IS NULL";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Gets list of unscheduled appointments.  Allowed orderby: status, alph, date.
		///Pass in a negative clinicNum to show all.</summary>
		public static List<Appointment> RefreshUnsched(string orderby,long provNum,long siteNum,bool includeBrokenAppts,long clinicNum,string codeStart,
			string codeEnd,DateTime dateStart,DateTime dateEnd)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),orderby,provNum,siteNum,includeBrokenAppts,clinicNum,codeStart,codeEnd,
					dateStart,dateEnd);
			}
			string command="SELECT * FROM appointment "
				+"LEFT JOIN patient ON patient.PatNum=appointment.PatNum ";
			if(!string.IsNullOrEmpty(codeStart)) {
				command+="INNER JOIN ( "
						+"SELECT procedurelog.AptNum "
						+"FROM procedurelog "
						+"INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
						+"AND procedurecode.ProcCode >= '"+POut.String(codeStart)+"' "
						+"AND procedurecode.ProcCode <= '"+POut.String(codeEnd)+"' "
						+"WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP)+" "
						+"AND procedurelog.AptNum!=0 "
						+"AND procedurelog.ProcDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd.Year<1880 ? DateTime.MaxValue : dateEnd)
						+"GROUP BY procedurelog.AptNum "
					+")ProcCheck ON ProcCheck.AptNum=appointment.AptNum ";
			}
				command+="WHERE ";
			if(includeBrokenAppts) {
				command+="(AptStatus = "+POut.Long((int)ApptStatus.UnschedList)+" OR AptStatus = "+POut.Long((int)ApptStatus.Broken)+") ";
			}
			else {
				command+="AptStatus = "+POut.Long((int)ApptStatus.UnschedList)+" ";
			}
			if(provNum>0) {
				command+="AND (appointment.ProvNum="+POut.Long(provNum)+" OR appointment.ProvHyg="+POut.Long(provNum)+") ";
			}
			if(siteNum>0) {
				command+="AND patient.SiteNum="+POut.Long(siteNum)+" ";
			}
			if(clinicNum>=0) { //Only include appointments that belong to HQ clinic when clinics are enabled and no ClinicNum is specified.
				command+="AND appointment.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(dateEnd.Year<1880) {
				command+=$"AND appointment.AptDateTime>={POut.Date(dateStart)} ";
			}
			else {
				command+=$"AND {DbHelper.BetweenDates("appointment.AptDateTime",dateStart,dateEnd)} ";
			}
			command+="AND patient.PatStatus IN("+POut.Long((int)PatientStatus.Patient)+","+POut.Long((int)PatientStatus.Prospective)+") ";	
			if(orderby=="status") {
				command+="ORDER BY UnschedStatus,AptDateTime";
			}
			else if(orderby=="alph") {
				command+="ORDER BY LName,FName";
			}
			else { //if(orderby=="date"){
				command+="ORDER BY AptDateTime";
			}
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Returns the first DefNum of type DefCat.ApptConfirmed.</summary>
		public static long GetUnconfirmedStatus() {
			//No need to check RemotingRole; no call to db.
			return Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;
		}

		///<summary>Returns all DefNums of confirmation statuses that can be considered "confirmed".</summary>
		public static List<long> GetConfirmedStatuses() {
			//No need to check RemotingRole; no call to db.
			return new List<long> {
				PrefC.GetLong(PrefName.ApptEConfirmStatusAccepted),
				PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger),
				PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger),
				PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger),
			};
		}

		///<summary>Returns all of the appointments for a given patient that have a status of unscheduled.</summary>
		public static List<Appointment> GetUnschedApptsForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$"SELECT * FROM appointment WHERE AptStatus={POut.Int((int)ApptStatus.UnschedList)} AND PatNum={POut.Long(patNum)} ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Gets the number of procedures on each appointment. Returns a dictionary of aptNum and its procedure count.
		///Only considers unscheduled appointments.  This could change in the future which this method should be enhanced to handle any new statuses.</summary>
		public static SerializableDictionary<long,int> GetProcCountForUnscheduledApts(List<long> listAptNums) {
			if(listAptNums==null || listAptNums.Count < 1) {
				return new SerializableDictionary<long, int>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,int>(MethodBase.GetCurrentMethod(),listAptNums);
			}
			string command=$@"SELECT appointment.AptNum, COUNT(procedurelog.AptNum) ProcCount
											  FROM appointment 
											  INNER JOIN procedurelog ON appointment.AptNum=procedurelog.AptNum
											  WHERE appointment.AptNum IN({String.Join(",",listAptNums)}) 
											  AND AptStatus={POut.Int((int)ApptStatus.UnschedList)}
											  GROUP BY appointment.AptNum";
			return Db.GetTable(command).Select().ToSerializableDictionary(x => PIn.Long(x["AptNum"].ToString()),x => PIn.Int(x["ProcCount"].ToString()));
		}

		///<summary>Tests to see if this appointment will create a double booking. Returns arrayList with no items in it if no double bookings for 
		///this appt.  But if double booking, then it returns an arrayList of codes which would be double booked.  You must supply the appointment being 
		///scheduled as well as a list of all appointments for that day.  The list can include the appointment being tested if user is moving it to a 
		///different time on the same day.  The ProcsForOne list of procedures needs to contain the procedures for the apt becauese procsMultApts won't 
		///necessarily, especially if it's a planned appt on the pinboard.</summary>
		public static ArrayList GetDoubleBookedCodes(Appointment apt,DataTable dayTable,List<Procedure> procsMultApts,Procedure[] procsForOne) {
			//No need to check RemotingRole; no call to db.
			ArrayList retVal=new ArrayList();//codes
			//figure out which provider we are testing for
			long provNum;
			if(apt.IsHygiene){
				provNum=apt.ProvHyg;
			}
			else{
				provNum=apt.ProvNum;
			}
			//compute the starting row of this appt
			int convertToY=(int)(((double)apt.AptDateTime.Hour*(double)60
				/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)
				+(double)apt.AptDateTime.Minute
				/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)));
			int startIndex=convertToY;
			string pattern=Appointments.ConvertPatternFrom5(apt.Pattern);
			//keep track of which rows in the entire day would be occupied by provider time for this appt
			ArrayList aptProvTime=new ArrayList();
			for(int k=0;k<pattern.Length;k++){
				if(pattern.Substring(k,1)=="X"){
					aptProvTime.Add(startIndex+k);//even if it extends past midnight, we don't care
				}
			}
			//Now, loop through all the other appointments for the day, and see if any would overlap this one
			bool overlaps;
			Procedure[] procs;
			bool doubleBooked=false;//applies to all appts, not just one at a time.
			DateTime aptDateTime;
			for(int i=0;i<dayTable.Rows.Count;i++){
				if(dayTable.Rows[i]["AptNum"].ToString()==apt.AptNum.ToString()){//ignore current apt in its old location
					continue;
				}
				//ignore other providers
				if(dayTable.Rows[i]["IsHygiene"].ToString()=="1" && dayTable.Rows[i]["ProvHyg"].ToString()!=provNum.ToString()){
					continue;
				}
				if(dayTable.Rows[i]["IsHygiene"].ToString()=="0" && dayTable.Rows[i]["ProvNum"].ToString()!=provNum.ToString()){
					continue;
				}
				if(dayTable.Rows[i]["AptStatus"].ToString()==((int)ApptStatus.Broken).ToString()){//ignore broken appts
					continue;
				}
				aptDateTime=PIn.DateT(dayTable.Rows[i]["AptDateTime"].ToString());
				if(aptDateTime.Date!=apt.AptDateTime.Date) {//These appointments are on different days.
					continue;
				}
				//calculate starting row
				//this math is copied from another section of the program, so it's sloppy. Safer than trying to rewrite it:
				convertToY=(int)(((double)aptDateTime.Hour*(double)60
					/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)
					+(double)aptDateTime.Minute
					/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)));
				startIndex=convertToY;
				pattern=Appointments.ConvertPatternFrom5(dayTable.Rows[i]["Pattern"].ToString());
				//now compare it to apt
				overlaps=false;
				for(int k=0;k<pattern.Length;k++){
					if(pattern.Substring(k,1)=="X"){
						if(aptProvTime.Contains(startIndex+k)){
							overlaps=true;
							doubleBooked=true;
						}
					}
				}
				if(overlaps){
					//we need to add all codes for this appt to retVal
					procs=Procedures.GetProcsOneApt(PIn.Long(dayTable.Rows[i]["AptNum"].ToString()),procsMultApts);
					for(int j=0;j<procs.Length;j++){
						retVal.Add(ProcedureCodes.GetStringProcCode(procs[j].CodeNum));
					}
				}
			}
			//now, retVal contains all double booked procs except for this appt
			//need to all procs for this appt.
			if(doubleBooked){
				for(int j=0;j<procsForOne.Length;j++) {
					retVal.Add(ProcedureCodes.GetStringProcCode(procsForOne[j].CodeNum));
				}
			}
			return retVal;
		}

		public static DataTable GetDataBatchCC(DateTime dateFrom,DateTime dateTo,List<long> listPatNumsToExclude) {
			//No need to check RemotingRole; no call to db.
			List<Appointment> listApptsInPeriod=GetAppointmentsBatchCCInDateRange(dateFrom,dateTo,listPatNumsToExclude);
			List<long> listAptNumsInPeriod=listApptsInPeriod.Select(x => x.AptNum).ToList();
			//Setting DateFrom and DateTo to DateTime.MinValue on purpose. We only want to get data for appointments in listAptNumsInPeriod.
			//We are using the GetPeriodApptsTable because it calculates and returns estimated fees for every appointment that we want. 
			//We could use the GetEstPatientPortion(appt) method in a loop, however, it passes through to the GetPeriodApptsTable method, which seems worse. 
			return GetPeriodApptsTable(DateTime.MinValue,DateTime.MinValue,aptNum:0,isPlanned:false,listPinApptNums:listAptNumsInPeriod);
		}

		public static List<Appointment> GetAppointmentsBatchCCInDateRange(DateTime dateFrom,DateTime dateTo,List<long> listPatNumsToExclude) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listPatNumsToExclude);
			}
			string command="SELECT * FROM appointment "
				+$"WHERE {DbHelper.BetweenDates("AptDateTime",dateFrom,dateTo)} "
				+$"AND AptStatus IN({string.Join(",",ListScheduledApptStatuses.Select(x => POut.Int((int)x)).ToList())}) ";
			if(listPatNumsToExclude.Count>0) {
				command+=$"AND PatNum NOT IN({string.Join(",",listPatNumsToExclude.Select(x => POut.Int((int)x)))}) ";
			}
			return Crud.AppointmentCrud.SelectMany(command);
		}
		#endregion

		#region Insert Methods
		///<summary>Creates and inserts a "new patient" appointment using the information passed in.  Validation must be done prior to calling this.
		///Also, does not flag the patient as prospective.  That must be done outside this method as well. Defaults to marking appt as new patient.
		///Used by multiple applications so be very careful when changing this method.  E.g. Open Dental and Web Sched.</summary>
		public static Appointment CreateNewAppointment(Patient patCur,Operatory operatory,DateTime dateTimeStart,DateTime dateTimeAskedToArrive
			,string pattern,List<Schedule> listSchedPeriod,string apptNote="",long apptConfirmed=0,AppointmentType appointmentType=null,bool isNewPatAppt=true) 
		{
			//No need to check RemotingRole; no call to db.
			Appointment appointment=new Appointment();
			appointment.PatNum=patCur.PatNum;
			appointment.IsNewPatient=isNewPatAppt;
			//if the pattern passed in is blank, set the time using the default pattern for an appointment with no attached procedures.
			appointment.Pattern=String.IsNullOrWhiteSpace(pattern) ? GetApptTimePatternForNoProcs() : pattern;
			if(patCur.PriProv==0) {
				appointment.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			else {
				appointment.ProvNum=patCur.PriProv;
			}
			appointment.ProvHyg=patCur.SecProv;
			appointment.AptStatus=ApptStatus.Scheduled;
			appointment.AptDateTime=dateTimeStart;
			appointment.DateTimeAskedToArrive=dateTimeAskedToArrive;
			appointment.Op=operatory.OperatoryNum;
			if(apptConfirmed==0) {
				appointment.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;
			}
			else {
				appointment.Confirmed=apptConfirmed;
			}
			//if(operatory.ProvDentist!=0) {//if no dentist is assigned to op, then keep the original dentist.  All appts must have prov.
			//  apt.ProvNum=operatory.ProvDentist;
			//}
			//apt.ProvHyg=operatory.ProvHygienist;
			long assignedDent=Schedules.GetAssignedProvNumForSpot(listSchedPeriod,operatory,false,appointment.AptDateTime);
			long assignedHyg=Schedules.GetAssignedProvNumForSpot(listSchedPeriod,operatory,true,appointment.AptDateTime);
			if(assignedDent!=0) {//if no dentist is assigned to op, then keep the original dentist.  All appts must have prov.
				appointment.ProvNum=assignedDent;
			}
			appointment.ProvHyg=assignedHyg;
			appointment.IsHygiene=operatory.IsHygiene;
			appointment.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			if(operatory.ClinicNum==0) {
				appointment.ClinicNum=patCur.ClinicNum;
			}
			else {
				appointment.ClinicNum=operatory.ClinicNum;
			}
			appointment.Note=apptNote;
			if(appointmentType!=null) {
				//Set the appointment's AppointmentTypeNum and ColorOverride to the corresponding values of the appointment type passed in.
				appointment.AppointmentTypeNum=appointmentType.AppointmentTypeNum;
				appointment.ColorOverride=appointmentType.AppointmentTypeColor;
			}
			Appointments.Insert(appointment);//Handles inserting signal
			return appointment;
		}

		///<summary>Fills an appointment passed in with all appropriate procedures for the recall passed in. 
		///Set listRecalls to a list of all potential recalls for this patient that MIGHT need to be automatically scheduled for this current appointment.
		///It's up to the calling class to then place the appointment on the pinboard or schedule.  
		///The appointment will be inserted into the database in this method so it's important to delete it if the appointment doesn't get scheduled.  
		///Returns the list of procedures that were created for the appointment so that they can be displayed to Orion users.</summary>
		public static List<Procedure> FillAppointmentForRecall(Appointment aptCur,Recall recallCur,List<Recall> listRecalls,Patient patCur
			,List<string> listProcStrs,List<InsPlan> listPlans,List<InsSub> listSubs) 
		{
			//No need to check RemotingRole; no call to db.
			aptCur.PatNum=patCur.PatNum;
			aptCur.AptStatus=ApptStatus.UnschedList;//In all places where this is used, the unsched status with no aptDateTime will cause the appt to be deleted when the pinboard is cleared.
			if(patCur.PriProv==0) {
				aptCur.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			else {
				aptCur.ProvNum=patCur.PriProv;
			}
			aptCur.ProvHyg=patCur.SecProv;
			if(aptCur.ProvHyg!=0) {
				aptCur.IsHygiene=true;
			}
			aptCur.ClinicNum=patCur.ClinicNum;
			string recallPattern=Recalls.GetRecallTimePattern(recallCur,listRecalls,patCur,listProcStrs);
			aptCur.Pattern=RecallTypes.ConvertTimePattern(recallPattern);
			aptCur.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			List<PatPlan> listPatPlans=PatPlans.Refresh(patCur.PatNum);
			List<Benefit> listBenifits=Benefits.Refresh(listPatPlans,listSubs);
			InsSub sub1=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listPlans,listSubs)),listSubs);
			InsSub sub2=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listPlans,listSubs)),listSubs);
			aptCur.InsPlan1=sub1.PlanNum;
			aptCur.InsPlan2=sub2.PlanNum;
			Appointments.Insert(aptCur);
			Procedure procCur;
			List<Procedure> listProcs=new List<Procedure>();
			for(int i=0;i<listProcStrs.Count;i++) {
				procCur=new Procedure();//this will be an insert
				//procnum
				procCur.PatNum=patCur.PatNum;
				procCur.AptNum=aptCur.AptNum;
				ProcedureCode procCodeCur=ProcedureCodes.GetProcCode(listProcStrs[i]);
				procCur.CodeNum=procCodeCur.CodeNum;
				procCur.ProcDate=(aptCur.AptDateTime.Year>1800 ? aptCur.AptDateTime : DateTime.Now);
				procCur.DateTP=DateTime.Now;
				procCur.ProvNum=patCur.PriProv;
				//Procedures.Cur.Dx=
				procCur.ClinicNum=patCur.ClinicNum;
				procCur.MedicalCode=procCodeCur.MedicalCode;
				procCur.ProcFee=Procedures.GetProcFee(patCur,listPatPlans,listSubs,listPlans,procCur.CodeNum,procCur.ProvNum,procCur.ClinicNum,
					procCur.MedicalCode);
				//surf
				//toothnum
				//Procedures.Cur.ToothRange="";
				//ProcCur.NoBillIns=ProcedureCodes.GetProcCode(ProcCur.CodeNum).NoBillIns;
				//priority
				procCur.ProcStatus=ProcStat.TP;
				procCur.Note=ProcCodeNotes.GetNote(procCur.ProvNum,procCur.CodeNum,procCur.ProcStatus); //get the TP note.
				//Procedures.Cur.PriEstim=
				//Procedures.Cur.SecEstim=
				//claimnum
				//nextaptnum
				procCur.BaseUnits=procCodeCur.BaseUnits;
				procCur.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
				procCur.PlaceService=(PlaceOfService)PrefC.GetInt(PrefName.DefaultProcedurePlaceService);//Default Proc Place of Service for the Practice is used.
				if(Userods.IsUserCpoe(Security.CurUser)) {
					//This procedure is considered CPOE because the provider is the one that has added it.
					procCur.IsCpoe=true;
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					procCur.SiteNum=patCur.SiteNum;
				}
				Procedures.Insert(procCur);//no recall synch required
				Procedures.ComputeEstimates(procCur,patCur.PatNum,new List<ClaimProc>(),false,listPlans,listPatPlans,listBenifits,patCur.Age,listSubs);
				listProcs.Add(procCur);
			}
			UpdateProcDescriptForAppts(new List<Appointment>() { aptCur });
			return listProcs;
		}

		///<summary>Insert an appointment, and an invalid appointment signalod. Only pass in secUserNum if InsertIncludeAptNum would otherwise overwrite it. </summary>
		public static void Insert(Appointment appt,long secUserNum=0) {
			//No need to check RemotingRole; no call to db.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				InsertIncludeAptNum(appt,false,secUserNum);
			}
			else {//Oracle must always have a valid PK.
				appt.AptNum=DbHelper.GetNextOracleKey("appointment","AptNum");
				InsertIncludeAptNum(appt,true,secUserNum);
			}
			Signalods.SetInvalidAppt(appt);
		}

		///<summary>Set includeAptNum to true only in rare situations.  Like when we are inserting for eCW. Inserts an invalid appointment signalod.
		///Only include the secUserNum if a user is not available via Security.CurUser (e.g. MobileWeb calls). </summary>
		public static long InsertIncludeAptNum(Appointment appt,bool useExistingPK,long secUserNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				appt.AptNum=Meth.GetLong(MethodBase.GetCurrentMethod(),appt,useExistingPK,secUserNum);
				return appt.AptNum;
			}
			if(secUserNum!=0) {
				appt.SecUserNumEntry=secUserNum;
			}
			else {//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				appt.SecUserNumEntry=Security.CurUser.UserNum;
			}
			//make sure all fields are properly filled:
			if(appt.Confirmed==0){
				appt.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;
			}
			if(appt.ProvNum==0){
				appt.ProvNum=Providers.GetFirst(true).ProvNum;
			}
			double dayInterval=PrefC.GetDouble(PrefName.ApptReminderDayInterval);
			double hourInterval=PrefC.GetDouble(PrefName.ApptReminderHourInterval);
			DateTime automationBeginPref=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeStart);
			DateTime automationEndPref=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeEnd);
			//ApptComms.InsertForAppt(appt,dayInterval,hourInterval,automationBeginPref,automationEndPref);
			long retVal=Crud.AppointmentCrud.Insert(appt,useExistingPK);
			HistAppointments.CreateHistoryEntry(appt.AptNum,HistAppointmentAction.Created);
			Signalods.SetInvalidAppt(appt);
			return retVal;
		}

		#endregion

		#region Update Methods
		///<summary>Use to send to unscheduled list, to set broken, etc.  Do not use to set complete.  Inserts an invalid appointment signalod.</summary>
		public static void SetAptStatus(Appointment appt,ApptStatus newStatus,bool suppressHistory=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appt,newStatus,suppressHistory);
				return;
			}
			string command="UPDATE appointment SET AptStatus="+POut.Long((int)newStatus);
			if(newStatus==ApptStatus.UnschedList) {
				command+=",Op=0";//We do this so that this appointment does not stop an operatory from being hidden.
			}
			command+=" WHERE AptNum="+POut.Long(appt.AptNum);
			Db.NonQ(command);
			if(newStatus!=ApptStatus.Scheduled) {
				AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { appt.AptNum });
			}
			Signalods.SetInvalidAppt(appt);
			if(newStatus!=ApptStatus.Scheduled) {
				//ApptComms.DeleteForAppt(aptNum);//Delete the automated reminder if it was unscheduled.
			}
			if(suppressHistory) {//Breaking and charting Missed or Canceled proc codes from an apt causes its own history log creation.
				return;
			}
			HistAppointments.CreateHistoryEntry(appt.AptNum,HistAppointmentAction.Changed);
		}

		///<summary>The plan nums that are passed in are simply saved in columns in the appt.  Those two fields are used by approximately one office right now.
		///Inserts an invalid appointment signalod.</summary>
		public static void SetAptStatusComplete(Appointment appt,long planNum1,long planNum2) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appt,planNum1,planNum2);
				return;
			}
			string command="UPDATE appointment SET "
				+"AptStatus="+POut.Long((int)ApptStatus.Complete)+", "
				+"InsPlan1="+POut.Long(planNum1)+", "
				+"InsPlan2="+POut.Long(planNum2)+" "
				+"WHERE AptNum="+POut.Long(appt.AptNum);
			Db.NonQ(command);
			AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { appt.AptNum });
			Signalods.SetInvalidAppt(appt);
			HistAppointments.CreateHistoryEntry(appt.AptNum,HistAppointmentAction.Changed);
		}

		///<summary>Set the priority of the appointment.  Inserts an invalid appointment signalod.</summary>
		public static void SetPriority(Appointment appt,ApptPriority priority) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appt,priority);
				return;
			}
			string command="UPDATE appointment SET Priority="+POut.Int((int)priority)
				+" WHERE AptNum="+POut.Long(appt.AptNum);
			Db.NonQ(command);
			Signalods.SetInvalidAppt(appt);
			HistAppointments.CreateHistoryEntry(appt.AptNum,HistAppointmentAction.Changed);
		}

		public static void SetAptTimeLocked() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="UPDATE appointment SET TimeLocked="+POut.Bool(true);
			Signalods.SetInvalid(InvalidType.Appointment);
			Db.NonQ(command);
		}

		///<summary>The confirmStatus will be a DefNum or 0.  Inserts an invalid appointment signalod.</summary>
		public static void SetConfirmed(Appointment appt,long confirmStatus,bool createSheetsForCheckin=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appt,confirmStatus,createSheetsForCheckin);
				return;
			}
			string command="UPDATE appointment SET Confirmed="+POut.Long(confirmStatus);
			if(PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)==confirmStatus) {
				command+=",DateTimeArrived="+POut.DateT(DateTime.Now);
				if(createSheetsForCheckin) {
					Sheets.CreateSheetsForCheckIn(appt);
				}
			}
			else if(PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger)==confirmStatus){
				command+=",DateTimeSeated="+POut.DateT(DateTime.Now);
			}
			else if(PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger)==confirmStatus){
				command+=",DateTimeDismissed="+POut.DateT(DateTime.Now);
			}
			command+=" WHERE AptNum="+POut.Long(appt.AptNum);
			Db.NonQ(command);
			if(confirmStatus!=PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined)) {//now the status is not 'Callback'
				AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { appt.AptNum });
			}
			Signalods.SetInvalidAppt(appt);
			Plugins.HookAddCode(null, "Appointments.SetConfirmed_end", appt.AptNum, confirmStatus); 
			HistAppointments.CreateHistoryEntry(appt.AptNum,HistAppointmentAction.Changed);
		}

		///<summary>Sets the new pattern for an appointment.  This is how resizing is done.  Must contain only / and X, with each char representing 5 minutes.
		///Inserts an invalid appointment signalod.</summary>
		public static void SetPattern(Appointment appt,string newPattern) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appt,newPattern);
				return;
			}
			string command="UPDATE appointment SET Pattern='"+POut.String(newPattern)+"' WHERE AptNum="+POut.Long(appt.AptNum);
			Db.NonQ(command);
			Signalods.SetInvalidAppt(appt);
			HistAppointments.CreateHistoryEntry(appt.AptNum,HistAppointmentAction.Changed);
		}

		///<summary>Updates only the changed columns and returns the number of rows affected.  Supply an oldApt for comparison.  Inserts an invalid appointment signalod.</summary>
		public static bool Update(Appointment appointment,Appointment oldAppointment,bool suppressHistory=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),appointment,oldAppointment,suppressHistory);
			}
			bool retval=false;
			//ApptComms.UpdateForAppt(appointment);
			retval=Crud.AppointmentCrud.Update(appointment,oldAppointment);
			if(retval) {
				Signalods.SetInvalidAppt(appointment,oldAppointment);
			}
			if(appointment.AptStatus==ApptStatus.UnschedList && appointment.AptStatus!=oldAppointment.AptStatus) {
				appointment.Op=0;
				SetAptStatus(appointment,appointment.AptStatus);
			}
			if(appointment.Confirmed!=oldAppointment.Confirmed && appointment.Confirmed==PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)) {
				Sheets.CreateSheetsForCheckIn(appointment);
			}
			if(retval && !suppressHistory) {//Something actually changed.
				HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Changed);
			}
			if((oldAppointment.Confirmed==PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined) //If the status was 'Callback'
				&& appointment.Confirmed!=PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined))  //and now the status is not 'Callback'.
				|| appointment.AptStatus!=ApptStatus.Scheduled)						 //Or the appointment is no longer scheduled.
			{
				AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { appointment.AptNum });
			}
			return retval;
		}

		///<summary>Updates all appointments for the patient passed with the patients Primary and Secondary dental insurance plans.</summary>
		public static void UpdateInsPlansForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			Family fam=Patients.GetFamily(patNum);
			List<PatPlan> listPatPlans=PatPlans.Refresh(patNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(fam);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			InsSub sub1=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
			InsSub sub2=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
			Appointments.UpdateInsPlansForPatHelper(patNum,sub1.PlanNum,sub2.PlanNum);
		}

		///<summary>Updates InsPlan1 and InsPlan2 for every appointment that isn't completed, broken, or a patient note for the patient passed in. Do not
		///call this method directly; use UpdateInsPlansForPat.</summary>
		private static void UpdateInsPlansForPatHelper(long patNum,long planNum1,long planNum2) {
			//No remoting role check; private method
			string command;
			int days=PrefC.GetInt(PrefName.ApptAutoRefreshRange);
			bool addSignal=true;
			string where="WHERE appointment.AptStatus NOT IN ("+POut.Int((int)ApptStatus.Complete)
					+","+POut.Int((int)ApptStatus.Broken)
					+","+POut.Int((int)ApptStatus.PtNote)
					+","+POut.Int((int)ApptStatus.PtNoteCompleted)+")"
				+" AND appointment.PatNum="+patNum;
			if(days>-1) {
				command="SELECT COUNT(AptNum) FROM appointment "+where
					+" AND "+DbHelper.BetweenDates("AptDateTime",DateTime.Today,DateTime.Today.AddDays(days));
				//Will be true if they have any appointments that will be updated between the Appt Refresh Range.
				addSignal=Db.GetCount(command)!="0";
			}
			command="UPDATE appointment SET appointment.InsPlan1="+planNum1+",appointment.InsPlan2="+planNum2+" "+where;
			Db.NonQ(command);
			if(addSignal) {
				Signalods.SetInvalid(InvalidType.Appointment);
			}
		}

		public static void UpdateProcDescriptionForAppt(Procedure procNew,Procedure procOld) {
			if(procNew.AptNum==0 && procNew.PlannedAptNum==0 && procOld.AptNum==0 && procOld.PlannedAptNum==0) {
				return;//Nothing to update.
			}
			long aptNum=procNew.AptNum>0 ? procNew.AptNum : procNew.PlannedAptNum;
			Appointment apt;
			if(procNew.AptNum==0 && procOld.AptNum>0) {
				aptNum=procOld.AptNum;
			}
			else if(procNew.PlannedAptNum==0 && procOld.PlannedAptNum>0) {
				aptNum=procOld.PlannedAptNum;
			}
			apt=GetOneApt(aptNum);
			if(apt==null) {
				return;//Apt not found in db, most likely deleted.
			}
			UpdateProcDescriptForAppts(new List<Appointment>() { apt });
		}

		///<summary>Updates the ProcDesript and ProcsColored to be current for every appointment passed in.
		///Inserts an invalid appointment signalod.</summary>
		public static void UpdateProcDescriptForAppts(List<Appointment> listAppointments) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAppointments);
				return;
			}
			foreach(Appointment apt in listAppointments) {
				//In the event a null object makes its way in to the list
				if(apt==null) {
					continue;
				}
				Appointment aptOld = apt.Copy();
				SetProcDescript(apt);
				if(Appointments.Update(apt,aptOld)) {
					Signalods.SetInvalidAppt(apt,aptOld);
				}
			}
		}

		///<summary>Inserts or updates the appointment and makes any other related updates to the database.</summary>
		///<exception cref="ApplicationException" />
		public static ApptSaveHelperResult ApptSaveHelper(Appointment aptCur,Appointment aptOld,bool isInsertRequired,List<Procedure> listProcsForAppt,
			List<Appointment> listAppointments,List<int> listSelectedIndices,List<long> listProcNumsAttachedStart,bool isPlanned,
			List<InsPlan> listInsPlans,List<InsSub> listInsSubs,long selectedProvNum,long selectedProvHygNum,List<Procedure> listProcsSelected,bool isNew,
			Patient pat,Family fam,bool doUpdateProcFees,bool doRemoveCompleteProcs,bool doCreateSecLog,bool doInsertHL7) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ApptSaveHelperResult>(MethodBase.GetCurrentMethod(),aptCur,aptOld,isInsertRequired,listProcsForAppt,listAppointments,
					listSelectedIndices,listProcNumsAttachedStart,isPlanned,listInsPlans,listInsSubs,selectedProvNum,selectedProvHygNum,listProcsSelected,isNew,
					pat,fam,doUpdateProcFees,doRemoveCompleteProcs,doCreateSecLog,doInsertHL7);
			}
			ApptSaveHelperResult retVal=new ApptSaveHelperResult();
			DateTime datePrevious=aptCur.DateTStamp;
			if(isInsertRequired) {
				Appointments.Insert(aptCur);
				//If we are on middle tier, the reference to aptCur will not be in the listAppointments anymore.
				//If not on middle tier, the reference to aptCur will be the same as what's in the list, so we don't need to add again.
				if(listAppointments.RemoveAll(x => x.AptNum==0) > 0) {
					listAppointments.Add(aptCur);
				}
			}
			else {
				Appointments.Update(aptCur,aptOld);
			}
			Procedures.ProcsAptNumHelper(listProcsForAppt,aptCur,listAppointments,listSelectedIndices,listProcNumsAttachedStart,isPlanned);			
			retVal.DoRunAutomation=Procedures.UpdateProcsInApptHelper(listProcsForAppt,pat,aptCur,aptOld,listInsPlans,listInsSubs,listSelectedIndices,
				doRemoveCompleteProcs,doUpdateProcFees);
			if(!isNew && aptCur.Confirmed!=aptOld.Confirmed) {
				//Log confirmation status changes.
				SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,pat.PatNum,"Appointment confirmation status changed from"+" "
					+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,aptCur.Confirmed)
					+" from the appointment edit window.",aptCur.AptNum,datePrevious);
			}
			bool isCreateAppt=false;
			bool isAptCreatedOrEdited=false;
			if(isNew) {
				if(aptCur.AptStatus==ApptStatus.UnschedList && aptCur.AptDateTime==DateTime.MinValue) { //If new appt is being added directly to pinboard
					//Do nothing.  Log will be created when appointment is dragged off the pinboard.
				}
				else {
					if(doCreateSecLog) {
						SecurityLogs.MakeLogEntry(Permissions.AppointmentCreate,pat.PatNum,
							aptCur.AptDateTime.ToString()+", "+aptCur.ProcDescript,
							aptCur.AptNum,datePrevious);
					}
					isAptCreatedOrEdited=true;
					isCreateAppt=true;
				}
			}
			else {
				string logEntryMessage="";
				if(aptCur.AptStatus==ApptStatus.Complete) {
					string newCarrierName1=InsPlans.GetCarrierName(aptCur.InsPlan1,listInsPlans);
					string newCarrierName2=InsPlans.GetCarrierName(aptCur.InsPlan2,listInsPlans);
					string oldCarrierName1=InsPlans.GetCarrierName(aptOld.InsPlan1,listInsPlans);
					string oldCarrierName2=InsPlans.GetCarrierName(aptOld.InsPlan2,listInsPlans);
					if(aptOld.InsPlan1!=aptCur.InsPlan1) {
						if(aptCur.InsPlan1==0) {
							logEntryMessage+="\r\nRemoved "+oldCarrierName1+" for InsPlan1";
						}
						else if(aptOld.InsPlan1==0) {
							logEntryMessage+="\r\nAdded "+newCarrierName1+" for InsPlan1";
						}
						else {
							logEntryMessage+="\r\nChanged "+oldCarrierName1+" to "+newCarrierName1+" for InsPlan1";
						}
					}
					if(aptOld.InsPlan2!=aptCur.InsPlan2) {
						if(aptCur.InsPlan2==0) {
							logEntryMessage+="\r\nRemoved "+oldCarrierName2+" for InsPlan2";
						}
						else if(aptOld.InsPlan2==0) {
							logEntryMessage+="\r\nAdded "+newCarrierName2+" for InsPlan2";
						}
						else {
							logEntryMessage+="\r\nChanged "+oldCarrierName2+" to "+newCarrierName2+" for InsPlan2";
						}
					}
				}
				if(doCreateSecLog) {
					if(aptOld.AptStatus==ApptStatus.Complete) {//seperate log entry for completed appointments
						SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,pat.PatNum,
							aptCur.AptDateTime.ToShortDateString()+", "+aptCur.ProcDescript+logEntryMessage,aptCur.AptNum,datePrevious);
					}
					else {
						string logText=aptCur.AptDateTime.ToShortDateString()+", "+aptCur.ProcDescript;
						if(aptCur.AptStatus==ApptStatus.Complete) {
							logText+=", Set Complete";//Podium expects this exact text in the security log when the appointment was set complete.
						}
						logText+=logEntryMessage;
						SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,pat.PatNum,logText,aptCur.AptNum,datePrevious);
					}
				}
				isAptCreatedOrEdited=true;
			}
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(isAptCreatedOrEdited && doInsertHL7 && HL7Defs.IsExistingHL7Enabled()) {
				//S14 - Appt Modification event, S12 - New Appt Booking event
				MessageHL7 messageHL7=null;
				if(isCreateAppt) {
					messageHL7=MessageConstructor.GenerateSIU(pat,fam.GetPatient(pat.Guarantor),EventTypeHL7.S12,aptCur);
				}
				else {
					messageHL7=MessageConstructor.GenerateSIU(pat,fam.GetPatient(pat.Guarantor),EventTypeHL7.S14,aptCur);
				}
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=aptCur.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=pat.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						System.Windows.Forms.MessageBox.Show(messageHL7.ToString());
					}
				}
			}
			if(isAptCreatedOrEdited && doInsertHL7 && HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(pat.PatNum));
			}
			retVal.ListProcsForAppt=listProcsForAppt;
			retVal.AptCur=aptCur;
			retVal.ListAppts=listAppointments;
			return retVal;
		}

		///<summary>The fields that are returned from AptSaveHelper.</summary>
		[Serializable]
		public class ApptSaveHelperResult {
			public List<Procedure> ListProcsForAppt;
			public Appointment AptCur;
			public List<Appointment> ListAppts;
			public bool DoRunAutomation;
		}
		#endregion

		#region Delete Methods
		///<summary>Deletes the apt and cleans up objects pointing to this apt.  If the patient is new, sets DateFirstVisit.
		///Updates procedurelog.ProcDate to today for procedures attached to the appointment if the ProcDate is invalid.
		///Updates procedurelog.PlannedAptNum (for planned apts) or procedurelog.AptNum (for all other AptStatuses); sets to 0.
		///Updates labcase.PlannedAptNum (for planned apts) or labcase.AptNum (for all other AptStatuses); sets to 0.
		///Deletes any rows in the plannedappt table with this AptNum.
		///Updates appointment.NextAptNum (for planned apts) of any apt pointing to this planned apt; sets to 0;
		///Deletes any rows in the apptfield table with this AptNum.
		///Makes an entry in the HistAppointment table.
		///Deletes ApptComm entries that were created for this appointment.
		///Inserts an invalid appointment signalod if hasSignal.  The hasSignal defaults to false because this function is referenced from CRUD.</summary>
		public static void Delete(long aptNum,bool hasSignal=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aptNum,hasSignal);
				return;
			}
			string command;
			command="SELECT PatNum,IsNewPatient,AptStatus FROM appointment WHERE AptNum="+POut.Long(aptNum);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count<1){
				return;//Already deleted or did not exist.
			}
			if(table.Rows[0]["IsNewPatient"].ToString()=="1") {
				Patient pat=Patients.GetPat(PIn.Long(table.Rows[0]["PatNum"].ToString()));
				Procedures.SetDateFirstVisit(DateTime.MinValue,3,pat);
			}
			//procs
			command="UPDATE procedurelog SET ProcDate="+DbHelper.Curdate()
				+" WHERE ProcDate<"+POut.Date(new DateTime(1880,1,1))
				+" AND PlannedAptNum="+POut.Long(aptNum)
				+" AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP);//Only change procdate for TP procedures
			Db.NonQ(command);
			command="UPDATE procedurelog SET ProcDate="+DbHelper.Curdate()
				+" WHERE ProcDate<"+POut.Date(new DateTime(1880,1,1))
				+" AND AptNum="+POut.Long(aptNum)
				+" AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP);//Only change procdate for TP procedures
			Db.NonQ(command);
			if(table.Rows[0]["AptStatus"].ToString()=="6") {//planned
				command="UPDATE procedurelog SET PlannedAptNum =0 WHERE PlannedAptNum = "+POut.Long(aptNum);
			}
			else {
				command="UPDATE procedurelog SET AptNum =0 WHERE AptNum = "+POut.Long(aptNum);
			}
			Db.NonQ(command);
			//labcases
			if(table.Rows[0]["AptStatus"].ToString()=="6") {//planned
				command="UPDATE labcase SET PlannedAptNum =0 WHERE PlannedAptNum = "+POut.Long(aptNum);
			}
			else {
				command="UPDATE labcase SET AptNum =0 WHERE AptNum = "+POut.Long(aptNum);
			}
			Db.NonQ(command);
			//plannedappt
			command="DELETE FROM plannedappt WHERE AptNum="+POut.Long(aptNum);
			Db.NonQ(command);
			//if deleting a planned appt, make sure there are no appts with NextAptNum (which should be named PlannedAptNum) pointing to this appt
			if(table.Rows[0]["AptStatus"].ToString()=="6") {//planned
				command="UPDATE appointment SET NextAptNum=0 WHERE NextAptNum="+POut.Long(aptNum);
				Db.NonQ(command);
			}
			//apptfield
			command="DELETE FROM apptfield WHERE AptNum = "+POut.Long(aptNum);
			Db.NonQ(command);
			command="SELECT * FROM appointment WHERE AptNum = "+POut.Long(aptNum);
			Appointment apt=Crud.AppointmentCrud.SelectOne(command);
			HistAppointments.CreateHistoryEntry(apt,HistAppointmentAction.Deleted);
			AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { aptNum });
			Appointments.ClearFkey(aptNum);//Zero securitylog FKey column for row to be deleted.
			//we will not reset item orders here
			command="DELETE FROM appointment WHERE AptNum = "+POut.Long(aptNum);
			//ApptComms.DeleteForAppt(aptNum);
			Db.NonQ(command);
			if(hasSignal) {
				Signalods.SetInvalidAppt(null,apt);//pass in the old appointment that we are deleting
			}
		}

		///<summary>Deletes the apts and cleans up objects pointing to these apts.  If the patient is new, sets DateFirstVisit.
		///Updates procedurelog.ProcDate to today for procedures attached to the appointment if the ProcDate is invalid.
		///Updates procedurelog.PlannedAptNum (for planned apts) or procedurelog.AptNum (for all other AptStatuses); sets to 0.
		///Updates labcase.PlannedAptNum (for planned apts) or labcase.AptNum (for all other AptStatuses); sets to 0.
		///Deletes any rows in the plannedappt table with this AptNum.
		///Updates appointment.NextAptNum (for planned apts) of any apt pointing to this planned apt; sets to 0;
		///Deletes any rows in the apptfield table with this AptNum.
		///Makes an entry in the HistAppointment table.
		///Deletes ApptComm entries that were created for this appointment.</summary>
		public static void Delete(List<long> aptNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aptNums);
				return;
			}
			if(aptNums==null || aptNums.Count<1) {
				return;
			}
			string command="SELECT PatNum,IsNewPatient,AptStatus,AptNum FROM appointment WHERE AptNum IN("+String.Join(",",aptNums)+")";
			DataTable table = Db.GetTable(command);
			if(table.Rows.Count<1) {
				return;//All entries were already deleted or did not exist.
			}
			List<long> listPlannedAptNums = new List<long>();  //List of AptNums for planned appointments only
			List<long> listNotPlannedAptNums = new List<long>();  //List of AptNums for all appointments that are not planned
			List<long> listAllAptNums = new List<long>();  //List of AptNums for all appointments
			foreach(DataRow row in table.Rows) {
				if(row["IsNewPatient"].ToString()=="1") {
					//Potentially improve this to not run one at a time
					Patient pat = Patients.GetPat(PIn.Long(row["PatNum"].ToString()));
					Procedures.SetDateFirstVisit(DateTime.MinValue,3,pat);
				}
				if(row["AptStatus"].ToString()=="6") {//planned
					listPlannedAptNums.Add(PIn.Long(row["AptNum"].ToString()));
				}
				else {//Everything else
					listNotPlannedAptNums.Add(PIn.Long(row["AptNum"].ToString()));
				}
				listAllAptNums.Add(PIn.Long(row["AptNum"].ToString()));
			}
			//procs
			command="UPDATE procedurelog SET ProcDate="+DbHelper.Curdate()
				+" WHERE ProcDate<"+POut.Date(new DateTime(1880,1,1))
				+" AND (AptNum IN("+String.Join(",",listAllAptNums)+") OR PlannedAptNum IN("+String.Join(",",listAllAptNums)+"))"
				+" AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP);//Only change procdate for TP procedures
			Db.NonQ(command);
			if(listPlannedAptNums.Count!=0) {
				command="UPDATE procedurelog SET PlannedAptNum=0 WHERE PlannedAptNum IN("+String.Join(",",listPlannedAptNums)+")";
				Db.NonQ(command);
			}
			if(listNotPlannedAptNums.Count!=0) {
				command="UPDATE procedurelog SET AptNum=0 WHERE AptNum IN("+String.Join(",",listNotPlannedAptNums)+")";
				Db.NonQ(command);
			}
			//labcases
			if(listPlannedAptNums.Count!=0) {
				command="UPDATE labcase SET PlannedAptNum=0 WHERE PlannedAptNum IN("+String.Join(",",listPlannedAptNums)+")";
				Db.NonQ(command);
			}
			if(listNotPlannedAptNums.Count!=0) {
				command="UPDATE labcase SET AptNum=0 WHERE AptNum IN("+String.Join(",",listNotPlannedAptNums)+")";
				Db.NonQ(command);
			}
			//plannedappt
			command="DELETE FROM plannedappt WHERE AptNum IN("+String.Join(",",listAllAptNums)+")";
			Db.NonQ(command);
			//if deleting a planned appt, make sure there are no appts with NextAptNum (which should be named PlannedAptNum) pointing to this appt
			if(listPlannedAptNums.Count!=0) {
				command="UPDATE appointment SET NextAptNum=0 WHERE NextAptNum IN("+String.Join(",",listNotPlannedAptNums)+")";
				Db.NonQ(command);
			}
			//apptfield
			command="DELETE FROM apptfield WHERE AptNum IN("+String.Join(",",listAllAptNums)+")";
			Db.NonQ(command);
			Appointments.ClearFkey(listAllAptNums);//Zero securitylog FKey column for row to be deleted.
			//we will not reset item orders here
			//ApptComms.DeleteForAppts(listAllAptNums);
			command="SELECT * FROM appointment WHERE AptNum IN("+String.Join(",",listAllAptNums)+")";
			List<Appointment> listApts=Crud.AppointmentCrud.SelectMany(command);
			listApts.ForEach(x => HistAppointments.CreateHistoryEntry(x,HistAppointmentAction.Deleted));
			AlertItems.DeleteFor(AlertType.CallbackRequested,listApts.Select(x => x.AptNum).ToList());
			command="DELETE FROM appointment WHERE AptNum IN("+String.Join(",",listAllAptNums)+")";
			Db.NonQ(command);
			Signalods.SetInvalid(InvalidType.Appointment);
		}
		#endregion

		#region Sync Methods
		///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
		///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new Apptcomm items.
		///Returns true if a change was made, otherwise false.</summary>
		public static bool Sync(List<Appointment> listNew,List<Appointment> listOld,long patNum,long userNum=0,bool isOpMerge=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld,patNum,userNum,isOpMerge);
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			userNum=Security.CurUser.UserNum;
			bool isChanged=Crud.AppointmentCrud.Sync(listNew,listOld,userNum);
			if(isChanged) {
				if(isOpMerge) { //If this is operatory merge the list could be very long.  Just send a generalized, invalid appt signal, this shouldn't happen often anyway.
					Signalods.SetInvalid(InvalidType.Appointment);
				}
				else {
					foreach(Appointment appt in listNew.Union(listOld).DistinctBy(x => x.AptNum)) { //insert a new signal for each unique appt
						Signalods.SetInvalidAppt(appt);
					}
				}
			}
			return isChanged;
		}
		#endregion Sync Methods

		#region Misc Methods
		///<summary>Returns the time pattern after combining all codes together for the providers passed in.
		///If make5minute is false, then result will be in 10 or 15 minute blocks and will need a later conversion step before going to db.</summary>
		public static string CalculatePattern(long provDent,long provHyg,List<long> codeNums,bool make5minute) {
			//No need to check RemotingRole; no call to db.
			List<ProcedureCode> listProcedureCodes=ProcedureCodes.GetListDeep();
			List<string> listProcPatterns=new List<string>();
			foreach(long codeNum in codeNums) {
				if(ProcedureCodes.GetProcCode(codeNum,listProcedureCodes).IsHygiene) {
					listProcPatterns.Add(ProcCodeNotes.GetTimePattern(provHyg,codeNum));
				}
				else {//dentist proc
					listProcPatterns.Add(ProcCodeNotes.GetTimePattern(provDent,codeNum));
				}
			}
			//Tack all time portions together to make an end result.
			string pattern=GetApptTimePatternFromProcPatterns(listProcPatterns);
			//Creating a new StringBuilder to preserve old hook parameter object Types.
			Plugins.HookAddCode(null,"Appointments.CalculatePattern_end",new StringBuilder(pattern),provDent,provHyg,codeNums);
			if(make5minute) {
				return ConvertPatternTo5(pattern);
			}
			return pattern;
		}

		///<summary>Returns an appointment pattern in a 5 minute increment format.
		///If listProcs is empty or null will return default appt length based on PrefName.AppointmentWithoutProcsDefaultLength or '/' (5 mins) if not defined.</summary>
		public static string CalculatePattern(Patient pat,long aptProvNum,long aptProvHygNum,List<Procedure> listProcs,bool isTimeLocked=false,bool ignoreTimeLocked=false) {
			if(!ignoreTimeLocked && isTimeLocked) {
				return null;
			}
			//We are using the providers selected for the appt rather than the providers for the procs.
			//Providers for the procs get reset when closing this form.
			long provDent=Patients.GetProvNum(pat);
			long provHyg=Patients.GetProvNum(pat);
			if(aptProvNum!=0){
				provDent=aptProvNum;
				provHyg=aptProvNum;
			}
			if(aptProvHygNum!=0) {
				provHyg=aptProvHygNum;
			}
			List<long> codeNums=new List<long>();
			foreach(Procedure proc in listProcs) {
				codeNums.Add(proc.CodeNum);
			}
			return CalculatePattern(provDent,provHyg,codeNums,listProcs.Count>0);
			//Plugins.HookAddCode(this,"FormApptEdit.CalculateTime_end",strBTime,provDent,provHyg,codeNums);//set strBTime, but without using the 'new' keyword.--Hook removed.
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching aptNum as FKey and are related to Appointment.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Appointment table type.</summary>
		public static void ClearFkey(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aptNum);
				return;
			}
			Crud.AppointmentCrud.ClearFkey(aptNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching aptNums as FKey and are related to Appointment.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Appointment table type.</summary>
		public static void ClearFkey(List<long> listAptNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAptNums);
				return;
			}
			Crud.AppointmentCrud.ClearFkey(listAptNums);
		}

		///<summary>Returns true if no appointments are scheduled in the time slot for that operatory unless the appointment scheduled is the 
		///appointment under consideration.</summary>
		public static bool IsSlotAvailable(DateTime slotStart,DateTime slotEnd,long operatory,Appointment appt) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),slotStart,slotEnd,operatory,appt);
			}
			string command="SELECT COUNT(appointment.AptNum) "
				+"FROM appointment "
				+"WHERE appointment.Op="+POut.Long(operatory)+" "
				+"AND appointment.AptDateTime < "+POut.DateT(slotEnd)+" "
				+"AND appointment.AptDateTime+INTERVAL LENGTH(appointment.Pattern)*5 MINUTE > "+POut.DateT(slotStart)+" "
				+"AND appointment.AptStatus IN("+string.Join(",",Appointments.ListScheduledApptStatuses.Select(x => POut.Int((int)x)))+") ";
			if(appt.AptNum != 0) {//If we are checking for an already existing appointment, then we don't count this appointment as filling the slot.
				command+="AND appointment.AptNum!="+POut.Long(appt.AptNum);
			}
			if(Db.GetCount(command)!="0") {
				return false;
			}
			return !Appointments.CheckForBlockoutOverlap(appt);
		}

		///<summary>The supplied DataRows must include the following columns: attached,Priority,ToothRange,ToothNum,ProcCode. This sorts all objects in Chart module based on their dates, times, priority, and toothnum.  For time comparisons, procs are not included.  But if other types such as comm have a time component in ProcDate, then they will be sorted by time as well.</summary>
		public static int CompareRows(DataRow x,DataRow y) {
			//No need to check RemotingRole; no call to db.
			/*if(x["attached"].ToString()!=y["attached"].ToString()){//if one is attached and the other is not
				if(x["attached"].ToString()=="1"){
					return -1;
				}
				else{
					return 1;
				}
			}*/
			return ProcedureLogic.CompareProcedures(x,y);//sort by priority, toothnum, procCode
		}

		///<summary>Modifies apt.Op with closest OpNum which has an opening at the specified apt.AptDateTime. 
		///First tries apt.OpNum, then tries remaining ops from left-to-right. Then tries remaining ops from right-to-left.
		///Returns true if overlap is found. Returns false if no overlap is found.</summary>
		public static bool TryAdjustAppointment(Appointment apt,List<Operatory> visOps,bool canChangeOp,bool canShortenPattern,bool hasEndTimeCheck
			,bool hasBlockoutCheck,out bool isPatternChanged) 
		{
			isPatternChanged=false;
			//First check the Op we are in for overlap.
			if(!TryAdjustAppointment_HasOverlap(apt,hasEndTimeCheck,hasBlockoutCheck,apt.Op)) { //No overlap on our original op so we are good.
				return false;
			}
			if(canShortenPattern) { //If we are allowed to shorten the pattern then we will not change ops, we will shorten the pattern down to no less than 1 and return it.
				isPatternChanged=true;
				do {
					if(apt.Pattern.Length==1) { //Pattern has been reduced to smallest allowed size.
						return TryAdjustAppointment_HasOverlap(apt,hasEndTimeCheck,hasBlockoutCheck,apt.Op);
					}
					//Reduce the pattern by 1.
					apt.Pattern=apt.Pattern.Substring(0,apt.Pattern.Length-1);
				} while(TryAdjustAppointment_HasOverlap(apt,hasEndTimeCheck,hasBlockoutCheck,apt.Op));
				//If canShortenPattern==true then caller is always expecting false.
				return false;
			}
			if(!canChangeOp) { //We tried our op and we are not allowed to change the op so there is an overlap.
				return true;
			}
			//Our op has an overlap but we are allowed to change so let's try all other ops.
			int startingOp=visOps.Select(x => x.OperatoryNum).ToList().IndexOf(apt.Op);
			//Left-to-right start at op directly to the right of this one.
			for(int i=startingOp+1;i<visOps.Count;i++) {
				long opNum=visOps[i].OperatoryNum;
				if(!TryAdjustAppointment_HasOverlap(apt,hasEndTimeCheck,hasBlockoutCheck,apt.Op)) { //We found an open op. Set it and return.
					apt.Op=opNum;
					return false;
				}
			}
			//Right-to-left starting at op directly to left of this one.
			for(int i=startingOp;i>=0;i--) {
				long opNum=visOps[i].OperatoryNum;
				if(!TryAdjustAppointment_HasOverlap(apt,hasEndTimeCheck,hasBlockoutCheck,apt.Op)) { //We found an open op. Set it and return.
					apt.Op=opNum;
					return false;
				}
			}
			//We could not find an open op so this AptDateTime overlaps all.
			return true;
		}

		private static bool TryAdjustAppointment_HasOverlap(Appointment apt,bool hasEndTimeCheck,bool hasBlockoutCheck,long opNum) {
			//Key=OpNum, value=list of appointments in that op
			Dictionary<long,List<Appointment>> dictLocalCache=new Dictionary<long, List<Appointment>>();
			//We may be coming back here several times for the same opNum so store our query results for each opNum for re-use.
			List<Appointment> listAppts;
			if(!dictLocalCache.TryGetValue(opNum,out listAppts)) {
				listAppts=Appointments.GetAppointmentsForOpsByPeriod(new List<long>() { opNum },apt.AptDateTime,apt.AptDateTime).FindAll(x => x.AptNum!=apt.AptNum);
				dictLocalCache[opNum]=listAppts;
			}
			//Start and end time of the apt we are validating.
			DateTime movedAptStartTime=apt.AptDateTime;
			DateTime movedAptEndTime=apt.AptDateTime.Add(TimeSpan.FromMinutes(apt.Pattern.Length*5));
			//Check for collisions with blockouts if specified, return true if there is one.
			if(hasBlockoutCheck) {
				List<Schedule> listOverlappingBlockouts=GetOverlappingBlockouts(apt);
				if(CheckForBlockoutOverlap(apt,listOverlappingBlockouts)) {
					return true;
				}
			}
			if(PrefC.GetYN(PrefName.ApptsAllowOverlap)) {//Only check for another appointment overlap when the preference is turned off
				return false;
			}
			foreach(Appointment curApt in listAppts) {
				//Start and end time of another apt in this op.
				DateTime curAptScheduledStartTime=curApt.AptDateTime;
				DateTime curAptScheduledEndTime=curApt.AptDateTime.Add(TimeSpan.FromMinutes(curApt.Pattern.Length*5));
				//Check start time.
				if(movedAptStartTime>=curAptScheduledStartTime && movedAptStartTime<curAptScheduledEndTime){//Starts during curApt's blockout time.
					return true;
				}
				if(!hasEndTimeCheck) { //We only care about start time so move on to the next apt in this op.
					continue;
				}
				if(movedAptEndTime<=curAptScheduledStartTime) { //moved appt ends before the current one starts
					continue;
				}
				if(movedAptStartTime>=curAptScheduledEndTime) { //moved appt starts after the current one ends
					continue;
				}
				//The moved appointment completely engulfs the scheduled appointment, return true for a collision.
				return true;
			}
			//No overlap found.
			return false;
		}

		//private delegate bool HasOverlap(long opNum,out bool doesOverlapBlockout);

		///<summary>Checks if the appointment passed in overlaps any appointment in the same operatory.
		///Returns true if overlap found.
		///Will modify the appointment.Pattern if canShortenPattern is true.</summary>
		public static bool TryAdjustAppointmentInCurrentOp(Appointment apt,bool canShortenPattern,bool hasEndTimeCheck,out bool isPatternChanged) 
		{
			return TryAdjustAppointment(apt,null,false,canShortenPattern,hasEndTimeCheck,true,out isPatternChanged);
		}

		///<summary>Returns true if there is an overlap with a blockout with the the flag "NS".</summary>
		public static bool CheckForBlockoutOverlap(Appointment apt,List<Schedule> listBlockouts=null) {
			return GetOverlappingBlockouts(apt,listBlockouts).Count > 0;
		}

		///<summary>Returns true if there is an overlap with a blockout with the the flag "NS" based on a given time point.
		///If operatoryNum=0 then all operatories will be checked.</summary>
		public static bool CheckTimeForBlockoutOverlap(DateTime aptTime, long operatoryNum) {
			return GetOverlappingBlockouts(new Appointment {
				AptDateTime=aptTime,
				Op=operatoryNum,
				Pattern="/",//Pretend this appointment is 5 minutes long since that's the minimum it could be.
			}).Count > 0;
		}

		///<summary>Gets all groups of overlapping appointments.</summary>
		public static List<List<long>> GetOverlappingAppts(DataTable dtAppointments) {
			//No need to check RemotingRole; no call to db.
			if(dtAppointments==null || dtAppointments.Rows.Count==0) {
				return new List<List<long>>();
			}
			//Group appointments by operatory
			var groupsApptsPerOp=dtAppointments.Select().Select(x => new Appointment {
				AptNum=PIn.Long(x["aptNum"].ToString()),
				Op=PIn.Long(x["Op"].ToString()),
				AptDateTime=PIn.DateT(x["AptDateTime"].ToString()),
				Pattern=(x["Pattern"].ToString())
			}).GroupBy(x => x.Op);
			//Set the type of the list
			List<Appointment> listOverlapAppts=new List<Appointment>();
			foreach(var opGroup in groupsApptsPerOp) {
				//order grouped appointments by start time. This increases efficiency as we do not need to search through the entire list, only
				//until we find an appoinment outside of the date time.
				List<Appointment> listAppts=opGroup.OrderBy(x => x.AptDateTime).ToList();
				for(int i=0;i<listAppts.Count;i++) {
					int j=i+1;
					Appointment curAppt=listAppts[i];
					for(j=i+1;j<listAppts.Count;j++) {
						Appointment nextAppt =listAppts[j];
						if(curAppt.EndTime<=nextAppt.AptDateTime) {
							break;
						}
						if(MiscUtils.DoSlotsOverlap(curAppt.AptDateTime,curAppt.EndTime,nextAppt.AptDateTime,nextAppt.EndTime)) {
							listOverlapAppts.Add(curAppt);
							listOverlapAppts.Add(nextAppt);
						}
					}
				}
			}
			//Unsorted AptNums
			listOverlapAppts=listOverlapAppts.Distinct().OrderBy(x => x.AptDateTime).ToList();
			//Break the AptNums into groups. These should be appointments that are overlapping and chained together. 
			//e.g. 10am-11am, 10:30am-11:30am, and 11am-12pm should all be in one group even though the first and second don't directly overlap
			List<List<long>> listApptOverlapGroups=new List<List<long>>();
			//Set Type
			while(listOverlapAppts.Count > 0) {
				List<Appointment> listApptsInGroup=new List<Appointment>{ listOverlapAppts[0] };
				for(int i=1;i<listOverlapAppts.Count;i++) {
					//start at 1 as we already have the first one
					//appointments are still in order by datetime so none will be missed
					if(listApptsInGroup.Any(x => MiscUtils.DoSlotsOverlap(x.AptDateTime,x.EndTime,
						listOverlapAppts[i].AptDateTime,listOverlapAppts[i].EndTime)
						&& x.Op==listOverlapAppts[i].Op)) 
					{
						listApptsInGroup.Add(listOverlapAppts[i]);
					}
				}
				listOverlapAppts=listOverlapAppts.Except(listApptsInGroup).ToList();//remove the ones found
				listApptOverlapGroups.Add(listApptsInGroup.Select(x => x.AptNum).ToList());
			}
			return listApptOverlapGroups;
		}
		
		///<summary>Returns overlapping blockout with the the flag "NS".</summary>
		public static List<Schedule> GetOverlappingBlockouts(Appointment apt,List<Schedule> listBlockouts=null) {
			//No need to check RemotingRole; no call to db.
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.BlockoutTypes);
			return (listBlockouts??Schedules.GetAllForDateAndType(apt.AptDateTime,ScheduleType.Blockout,listOpNums:new List<long> { apt.Op }))
				.Where(x => MiscUtils.DoSlotsOverlap(x.SchedDate.Add(x.StartTime),x.SchedDate.Add(x.StopTime),
					apt.AptDateTime,apt.AptDateTime.AddMinutes(apt.Length)))
				.Where(x => Defs.GetDef(DefCat.BlockoutTypes,x.BlockoutType,listDefs).ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()))
				.ToList();
		}

		//private static FilterBlockouts(Appointment )
		/// <summary>Called to move existing appointments from the web.
		/// Will only attempt to move to given operatory.</summary>
		public static void TryMoveApptWebHelper(Appointment appt,DateTime apptDateTimeNew,long opNumNew,LogSources secLogSource=LogSources.MobileWeb) {
			Appointment apptOld=GetOneApt(appt.AptNum);//Will always exist since you can not move a non inserted appointment.
			Patient pat=Patients.GetPat(appt.PatNum);
			Operatory op=Operatories.GetOperatory(opNumNew);
			List<Schedule> listSchedules=Schedules.ConvertTableToList(Schedules.GetPeriodSchedule(apptDateTimeNew.Date,apptDateTimeNew.Date));
			List<Operatory> listOps=new List<Operatory> { op };//List of ops to attempt to move appt to, only consider 1.
			bool provChangedNotUsed=false;//Since we are not skipping validation, let function identify prov change itself.
			bool hygChangedNotUsed=false;//Since we are not skipping validation, let function identify hyg change itself.
			TryMoveAppointment(appt,apptOld,pat,op,listSchedules,listOps,apptDateTimeNew,
				doValidation:true,doSetArriveEarly:true,doProvChange:true,doUpdatePattern:false,doAllowFreqConflicts:true,doResetConfirmationStatus:true,doUpdatePatStatus:true,
				provChanged:provChangedNotUsed,hygChanged:hygChangedNotUsed,timeWasMoved:(appt.AptDateTime!=apptDateTimeNew),isOpChanged:(appt.Op!=opNumNew),secLogSource:secLogSource);
		}
		
		///<summary>Throws exception. Called when moving an appt in the appt module on mouse up after validation and user input.</summary>
		public static void MoveValidatedAppointment(Appointment apt,Appointment aptOld,Patient patCur,Operatory curOp,
			List<Schedule> schedListPeriod,List<Operatory> listOps,bool provChanged,bool hygChanged,bool timeWasMoved,bool isOpChanged,bool isOpUpdate=false)
		{
			//Skipping validation so all bools that mimic prompt inputs are set to false since they have already ran in ContrApt.MoveAppointments(...)s validation.
			TryMoveAppointment(apt,aptOld,patCur,curOp,schedListPeriod,listOps,DateTime.MinValue,
				doValidation:false,doSetArriveEarly:false,doProvChange:false,doUpdatePattern:false,doAllowFreqConflicts:false,doResetConfirmationStatus:false,doUpdatePatStatus:false,
				provChanged:provChanged,hygChanged:hygChanged,timeWasMoved:timeWasMoved,isOpChanged:isOpChanged,isOpUpdate:isOpUpdate);
		}

		///<summary>Throws exception. When doSkipValidation is false all 'do' bools need to be set and considered.</summary>
		public static void TryMoveAppointment(Appointment apt,Appointment aptOld,Patient patCur,Operatory curOp,List<Schedule> schedListPeriod,
			List<Operatory> listOps,DateTime newAptDateTime,bool doValidation,bool doSetArriveEarly,bool doProvChange,bool doUpdatePattern,
			bool doAllowFreqConflicts,bool doResetConfirmationStatus,bool doUpdatePatStatus,bool provChanged,bool hygChanged,bool timeWasMoved,bool isOpChanged,bool isOpUpdate=false,LogSources secLogSource=LogSources.None) 
		{
			if(newAptDateTime!=DateTime.MinValue) {
				apt.AptDateTime=newAptDateTime;//The time we are attempting to move the appt to.
				timeWasMoved=(apt.AptDateTime!=aptOld.AptDateTime);
			}
			List<Procedure> procsForSingleApt=null;
			if(doValidation) {//ContrAppt.MoveAppointments(...) has identical validation but allows for YesNo input, mimiced here as booleans.
				#region Appointment validation and modifications
				apt.Op=curOp.OperatoryNum;
				provChanged=false;
				hygChanged=false;
				long assignedDent=Schedules.GetAssignedProvNumForSpot(schedListPeriod,curOp,false,apt.AptDateTime);
				long assignedHyg=Schedules.GetAssignedProvNumForSpot(schedListPeriod,curOp,true,apt.AptDateTime);
				if(apt.AptStatus!=ApptStatus.PtNote && apt.AptStatus!=ApptStatus.PtNoteCompleted) {
					if(timeWasMoved) {
						#region Update Appt's DateTimeAskedToArrive
						if(patCur.AskToArriveEarly>0) {
							apt.DateTimeAskedToArrive=apt.AptDateTime.AddMinutes(-patCur.AskToArriveEarly);
						}
						else {
							if(apt.DateTimeAskedToArrive.Year>1880 && (aptOld.AptDateTime-aptOld.DateTimeAskedToArrive).TotalMinutes>0) {
								apt.DateTimeAskedToArrive=apt.AptDateTime-(aptOld.AptDateTime-aptOld.DateTimeAskedToArrive);
								if(!doSetArriveEarly) {
									apt.DateTimeAskedToArrive=aptOld.DateTimeAskedToArrive;
								}
							}
							else {
								apt.DateTimeAskedToArrive=DateTime.MinValue;
							}
						}
						#endregion Update Appt's DateTimeAskedToArrive
					}
					#region Update Appt's Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
					//if no dentist/hygenist is assigned to spot, then keep the original dentist/hygenist without prompt.  All appts must have prov.
					if((assignedDent!=0 && assignedDent!=apt.ProvNum) || (assignedHyg!=0 && assignedHyg!=apt.ProvHyg)) {
						if(isOpUpdate || doProvChange) {//Short circuit logic.  If we're updating op through right click, never ask.
							if(assignedDent!=0) {//the dentist will only be changed if the spot has a dentist.
								apt.ProvNum=assignedDent;
								provChanged=true;
							}
							if(assignedHyg!=0 || PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly)) {//the hygienist will only be changed if the spot has a hygienist.
								apt.ProvHyg=assignedHyg;
								hygChanged=true;
							}
							if(curOp.IsHygiene) {
								apt.IsHygiene=true;
							}
							else {//op not marked as hygiene op
								if(assignedDent==0) {//no dentist assigned
									if(assignedHyg!=0) {//hyg is assigned (we don't really have to test for this)
										apt.IsHygiene=true;
									}
								}
								else {//dentist is assigned
									if(assignedHyg==0) {//hyg is not assigned
										apt.IsHygiene=false;
									}
									//if both dentist and hyg are assigned, it's tricky
									//only explicitly set it if user has a dentist assigned to the op
									if(curOp.ProvDentist!=0) {
										apt.IsHygiene=false;
									}
								}
							}
							procsForSingleApt=Procedures.GetProcsForSingle(apt.AptNum,false);
							List<long> codeNums=new List<long>();
							for(int p = 0;p<procsForSingleApt.Count;p++) {
								codeNums.Add(procsForSingleApt[p].CodeNum);
							}
							if(!isOpUpdate && doUpdatePattern) {
								string calcPattern=Appointments.CalculatePattern(apt.ProvNum,apt.ProvHyg,codeNums,true);
								if(apt.Pattern!=calcPattern
									&& !PrefC.GetBool(PrefName.AppointmentTimeIsLocked))//Updating op provs will not change apt lengths.
								{
									apt.Pattern=calcPattern;
								}
							}
						}
					}
					#endregion Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
				}
				#region Prevent overlap
				//JS Overlap is no longer prevented when moving from ContrAppt, and this code won't be hit because doValidation is false.
				//It is still prevented with TryMoveApptWebHelper, but only because I'm not overhauling that part of the code right now.
				if(!isOpUpdate && !TryAdjustAppointmentOp(apt,listOps)) {
					throw new ODException(Lans.g("MoveAppointment","Appointment overlaps existing appointment or blockout."));
				}
				#endregion Prevent overlap
				#region Detect Frequency Conflicts
				//Detect frequency conflicts with procedures in the appointment
				if(!isOpUpdate && PrefC.GetBool(PrefName.InsChecksFrequency)) {
					procsForSingleApt=Procedures.GetProcsForSingle(apt.AptNum,false);
					string frequencyConflicts="";
					try {
						frequencyConflicts=Procedures.CheckFrequency(procsForSingleApt,apt.PatNum,apt.AptDateTime);
					}
					catch(Exception e) {
						throw new Exception(Lans.g("MoveAppointment","There was an error checking frequencies."
								+"  Disable the Insurance Frequency Checking feature or try to fix the following error:")
								+"\r\n"+e.Message);
					}
					if(frequencyConflicts!="" && !doAllowFreqConflicts) {
						return;
					}
				}
				#endregion Detect Frequency Conflicts
				#region Patient status
				if(!isOpUpdate && doUpdatePatStatus) {
					Operatory opCur=Operatories.GetOperatory(apt.Op);
					Operatory opOld=Operatories.GetOperatory(aptOld.Op);
					if(opOld==null || opCur.SetProspective!=opOld.SetProspective) {
						Patient patOld=patCur.Copy();
						if(opCur.SetProspective && patCur.PatStatus!=PatientStatus.Prospective) { //Don't need to prompt if patient is already prospective.
							patCur.PatStatus=PatientStatus.Prospective;
						}
						else if(!opCur.SetProspective && patCur.PatStatus==PatientStatus.Prospective) {
							//Do we need to warn about changing FROM prospective? Assume so for now.
							patCur.PatStatus=PatientStatus.Patient;
						}
						if(patCur.PatStatus!=patOld.PatStatus) {
							SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patCur.PatNum,"Patient's status changed from "
								+patOld.PatStatus.GetDescription()+" to "+patCur.PatStatus.GetDescription()+".");
						}
						Patients.Update(patCur,patOld);
					}
				}
				#endregion Patient status
				#region Update Appt's AptStatus, ClinicNum, Confirmed
				if(apt.AptStatus==ApptStatus.Broken && (timeWasMoved||isOpChanged)) {
					apt.AptStatus=ApptStatus.Scheduled;
				}
				//original location of provider code
				if(curOp.ClinicNum==0) {
					apt.ClinicNum=patCur.ClinicNum;
				}
				else {
					apt.ClinicNum=curOp.ClinicNum;
				}
				if(apt.AptDateTime!=aptOld.AptDateTime
					&& apt.Confirmed!=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum
					&& apt.AptDateTime.Date!=DateTime.Today
					&& doResetConfirmationStatus)
				{
					apt.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;//Causes the confirmation status to be reset.
				}
				#endregion Update Appt's AptStatus, ClinicNum, Confirmed
				#endregion
				//All validation above is also in ContrAppt.MoveAppointments(...)
			}
			#region Update Appt in db
			try {
				Appointments.Update(apt,aptOld);
			}
			catch(ApplicationException ex) {
				ex.DoNothing();
				throw;
			}
			#endregion Update Appt in db
			#region apt.Confirmed securitylog
			if(apt.Confirmed!=aptOld.Confirmed) {
				//Log confirmation status changes.
				SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,apt.PatNum,
					Lans.g("MoveAppointment","Appointment confirmation status changed from")+" "
					+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed)+" "+Lans.g("MoveAppointment","to")+" "+Defs.GetName(DefCat.ApptConfirmed,apt.Confirmed)
					+Lans.g("MoveAppointment","from the appointment module")+".",apt.AptNum,secLogSource,aptOld.DateTStamp);
			}
			#endregion
			#region Set prov in apt
			if(apt.AptStatus!=ApptStatus.Complete) {
				if(procsForSingleApt==null) {
					procsForSingleApt=Procedures.GetProcsForSingle(apt.AptNum,false);
				}
				ProcFeeHelper procFeeHelper=new ProcFeeHelper(apt.PatNum);
				bool isUpdatingFees=false;
				List<Procedure> listProcsNew=procsForSingleApt.Select(x => Procedures.ChangeProcInAppointment(apt,x.Copy())).ToList();
				if(procsForSingleApt.Exists(x => x.ProvNum!=listProcsNew.FirstOrDefault(y => y.ProcNum==x.ProcNum).ProvNum)) {//Either the primary or hygienist changed.
					string promptText="";
					isUpdatingFees=Procedures.ShouldFeesChange(listProcsNew,procsForSingleApt,ref promptText,procFeeHelper);
					if(isUpdatingFees) {//Made it pass the pref check.
						if(promptText!=""){
								isUpdatingFees=false;
						}
					}
				}
				Procedures.SetProvidersInAppointment(apt,procsForSingleApt,isUpdatingFees,procFeeHelper);
			}
			#endregion
			#region SecurityLog
			if(isOpUpdate) {
				string logtext="";
				if(provChanged) {
					logtext=" "+Lans.g("MoveAppointment","provider changed");
				}
				if(hygChanged) {
					if(logtext!="") {
						logtext+=" "+Lans.g("MoveAppointment","and");
					}
					logtext+=" "+Lans.g("MoveAppointment","hygienist changed");
				}
				if(logtext!="") {
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,apt.PatNum,
						Lans.g("MoveAppointment","Appointment on")+" "+apt.AptDateTime.ToString()+logtext,apt.AptNum,secLogSource,apt.DateTStamp);
				}
			}
			else { 
				if(apt.AptStatus==ApptStatus.Complete) { //separate log entry for editing completed appointments
					SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,apt.PatNum,
						Lans.g("MoveAppointment","moved")+" "+apt.ProcDescript+" "+Lans.g("MoveAppointment","from")+" "+aptOld.AptDateTime.ToString()+", "+Lans.g("MoveAppointment","to")+" "+apt.AptDateTime.ToString(),
						apt.AptNum,secLogSource,aptOld.DateTStamp);
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.AppointmentMove,apt.PatNum,
						apt.ProcDescript+" "+Lans.g("MoveAppointment","from")+" "+aptOld.AptDateTime.ToString()+", "+Lans.g("MoveAppointment","to")+" "+apt.AptDateTime.ToString(),
						apt.AptNum,secLogSource,aptOld.DateTStamp);
				}
			}
			#endregion SecurityLog
			#region HL7
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				//S13 - Appt Rescheduling
				MessageHL7 messageHL7=MessageConstructor.GenerateSIU(patCur,Patients.GetPat(patCur.Guarantor),EventTypeHL7.S13,apt);
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=apt.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=patCur.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						throw new Exception(messageHL7.ToString());
					}
				}
			}
			#endregion HL7
			#region HieQueue
			if(HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(patCur.PatNum));
			}
			#endregion
		}

		///<summary>Creates a scheduled appointment from planned appointment passed in. listApptFields can be null. The fields will just be fetched from the database.
		///Returns the newly scheduled appointment and a boolean indicating whether at least one attached procedure was already attached to another appointment.</summary>
		public static ODTuple<Appointment,bool> SchedulePlannedApt(Appointment apptPlanned,Patient pat,List<ApptField> listApptFields,DateTime aptDateTime,long opNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ODTuple<Appointment,bool>>(MethodBase.GetCurrentMethod(),apptPlanned,pat,listApptFields,aptDateTime,opNum);
			}
			Appointment apptNew=apptPlanned.Copy();
			apptNew.NextAptNum=apptPlanned.AptNum;
			apptNew.AptStatus=ApptStatus.Scheduled;
			apptNew.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			apptNew.AptDateTime=aptDateTime;
			apptNew.Op=opNum;
			Appointments.Insert(apptNew);//now, aptnum is different.
			listApptFields=listApptFields??ApptFields.GetForAppt(apptPlanned.AptNum);
			foreach(ApptField apptField in listApptFields) {
				apptField.AptNum=apptNew.AptNum;
				ApptFields.Insert(apptField);
			}
			#region HL7
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				//S12 - New Appt Booking event
				MessageHL7 messageHL7=MessageConstructor.GenerateSIU(pat,Patients.GetPat(pat.Guarantor),EventTypeHL7.S12,apptNew);
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=apptNew.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=pat.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						Console.WriteLine(messageHL7.ToString());
					}
				}
			}
			#endregion HL7
			#region HieQueue
			if(HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(pat.PatNum));
			}
			#endregion
			List<Procedure> procsForPat=Procedures.GetForPlanned(apptPlanned.PatNum,apptPlanned.AptNum);
			bool isProcAlreadyAttached=false;
			for(int i=0;i<procsForPat.Count;i++) {
				if(procsForPat[i].AptNum > 0) {//already attached to another appt
					isProcAlreadyAttached=true;
				}
				else {//only update procedures not already attached to another apt
					Procedures.UpdateAptNum(procsForPat[i].ProcNum,apptNew.AptNum);
				}
			}
			LabCase lab=LabCases.GetForPlanned(apptPlanned.AptNum);
			if(lab!=null) {
				LabCases.AttachToAppt(lab.LabCaseNum,apptNew.AptNum);
			}
			return new ODTuple<Appointment,bool>(apptNew,isProcAlreadyAttached);
		}

		///<summary>Modifies apt.Op with closest OpNum which has an opening at the specified apt.AptDateTime. 
		///First tries apt.OpNum, then tries remaining ops from left-to-right. Then tries remaining ops from right-to-left.
		///Calling RefreshPeriod() is not necessary before calling this method. It goes to the db only as much as is necessary.
		///Returns true if adjustment was successful or no adjustment was necessary. Returns false if all potential adjustments still caused overlap.</summary>
		public static bool TryAdjustAppointmentOp(Appointment apt,List<Operatory> listOps) {
			bool notUsed;
			return !Appointments.TryAdjustAppointment(apt,listOps,true,false,true,true,out notUsed);
		}

		///<summary>Creates a new appointment for the given patient.
		///Accepts null for the patient.  If the patient is null, no patient specific defaults will be set.
		///Set useApptDrawingSettings to true if the user double clicked on the appointment schedule in order to make a new appointment.
		///It will utilize the global static properties to help set required fields for "Scheduled" appointments.
		///Otherwise, simply sets the corresponding PatNum and then the status to "Unscheduled".</summary>
		public static Appointment MakeNewAppointment(Patient PatCur,DateTime apptDateTime,long opNum,bool useApptDrawingSettings) {
			Appointment AptCur=new Appointment();
			if(PatCur!=null) {
				//Anything referencing PatCur must be in here.
				AptCur.PatNum=PatCur.PatNum;
				if(PatCur.DateFirstVisit.Year<1880
					&& !Procedures.AreAnyComplete(PatCur.PatNum))//this only runs if firstVisit blank
				{
					AptCur.IsNewPatient=true;
				}
				if(PatCur.PriProv==0) {
					AptCur.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
				}
				else {
					AptCur.ProvNum=PatCur.PriProv;
				}
				AptCur.ProvHyg=PatCur.SecProv;
				AptCur.ClinicNum=PatCur.ClinicNum;
			}
			if(useApptDrawingSettings) {//initially double clicked on appt module
				AptCur.AptDateTime=apptDateTime;
				if(PatCur!=null && PatCur.AskToArriveEarly>0) {
					AptCur.DateTimeAskedToArrive=AptCur.AptDateTime.AddMinutes(-PatCur.AskToArriveEarly);
				}
				AptCur.Op=opNum;
				AptCur=AssignFieldsForOperatory(AptCur);
				AptCur.AptStatus=ApptStatus.Scheduled;
			}
			else {
				//new appt will be placed on pinboard instead of specific time
				AptCur.AptStatus=ApptStatus.UnschedList;//This is so that if it's on the pinboard when use shuts down OD, no db inconsistency.
			}
			AptCur.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			AptCur.ColorOverride=System.Drawing.Color.FromArgb(0);
			AptCur.Pattern="/X/";
			return AptCur;
		}

		///<summary>Converts the pattern passed in to be 5 minute increments based on the AppointmentTimeIncrement preference.
		///Returns "/" if the pattern passed in is empty.</summary>
		public static string ConvertPatternTo5(string pattern) {
			//No need to check RemotingRole; no call to db.
			StringBuilder savePattern=new StringBuilder();
			for(int i=0;i<pattern.Length;i++) {
				savePattern.Append(pattern.Substring(i,1));
				if(PrefC.GetLong(PrefName.AppointmentTimeIncrement)==10) {
					savePattern.Append(pattern.Substring(i,1));
				}
				if(PrefC.GetLong(PrefName.AppointmentTimeIncrement)==15) {
					savePattern.Append(pattern.Substring(i,1));
					savePattern.Append(pattern.Substring(i,1));
				}
			}
			if(savePattern.Length==0) {
				savePattern=new StringBuilder("/");
			}
			return savePattern.ToString();
		}
		
		/// <summary>Converts time pattern from 5 to current increment preference.</summary>
		public static string ConvertPatternFrom5(string timepattern) {
			//convert time pattern from 5 to current increment.
			StringBuilder strBTime=new StringBuilder();
			for(int i=0;i<timepattern.Length;i++) {
				strBTime.Append(timepattern.Substring(i,1));
				if(PrefC.GetLong(PrefName.AppointmentTimeIncrement)==10) {
					i++;
				}
				if(PrefC.GetLong(PrefName.AppointmentTimeIncrement)==15) {
					i++;
					i++;
				}
			}
			return strBTime.ToString();
		}

		///<summary>Returns a new Appointment with various field values copied into it from the supplied Appointment.</summary>
		public static Appointment CopyStructure(Appointment appt) {
			Appointment apptNew=new Appointment();
			apptNew.PatNum=appt.PatNum;
			apptNew.ProvNum=appt.ProvNum;
			apptNew.Pattern=appt.Pattern;//Cannot copy length directly.
			apptNew.Note=appt.Note;
			apptNew.AptStatus=ApptStatus.UnschedList;//Set to unscheduled. Dragging and dropping this appointment from the pinboard to the operatory will change the status to 'Scheduled'
			apptNew.AppointmentTypeNum=appt.AppointmentTypeNum;
			return apptNew;
		}

		///<summary>Takes all time patterns passed in and fuses them into one final time pattern that should be used on appointments.
		///Returns "/" if a null or empty list of patterns is passed in (preserves old behavior).</summary>
		public static string GetApptTimePatternFromProcPatterns(List<string> listProcPatterns) {
			//No need to check RemotingRole; no call to db.
			//In v16.3 it was deemed a bug to convert procedure time patterns the way we were doing it.
			//The main problem in the old logic was the assumption that hyg time was always necessary at the beginning and ending of each appointment.
			//DESIRED NEW LOGIC-----------------------------------------------------------------------------------------------------------------------------
			//It is now acceptable to have no hyg time at the beginning or at the ending of the appointment.  E.g. X + XX = XXX
			//Also, all provider time (X's) will be preserved and only the max hyg time (/'s) at the beginning and the end will be preserved.
			//E.g. /X/ + /X/ + /XX/ + /XX/ = /XXXXXX/
			//E.g. //XXX/ + /X/ = //XXXX/
			if(listProcPatterns==null || listProcPatterns.Count < 1) {
				return GetApptTimePatternForNoProcs();//Returns 5 min interval based pattern.
			}
			string provTimeTotal="";
			string hygTimeStart="";
			string hygTimeEnd="";
			//listProcPatterns pattern formats were based on the PrefName.AppointmentTimeIncrement at the time that the proc code pattern was set.
			//This means that proc A could be in 5 min increments while proc B could be in 15 min increments.
			//We might want to eventually fix this so that the proc codes always save in 5 minute increments like appointment.Pattern.
			foreach(string procPatternRaw in listProcPatterns) {
				if(string.IsNullOrEmpty(procPatternRaw)) {
					continue;//No proc pattern to add to total time pattern.
				}
				string procPattern=procPatternRaw.ToUpper();
				string hygTimeStartCur=procPattern.Substring(0,procPattern.Length-procPattern.TrimStart('/').Length);
				//Keep track of the max leading hyg time (/'s)
				if(hygTimeStartCur.Length > hygTimeStart.Length) {
					hygTimeStart=hygTimeStartCur;
				}
				//Trim away the hyg start time and then trim off any /'s on the end and this will be the provider time.
				//Always retain the middle of the procedure time.  E.g. "/XXX///XX///" should retain "XXX///XX" for the provider time portion.
				provTimeTotal+=procPattern.Trim('/');
				//Keep track of the max ending hyg time (/'s) as long as there is at least one prov time (X's) present.
				if(procPattern.Contains('X')) {
					string hygTimeEndCur=procPattern.Substring(procPattern.TrimEnd('/').Length);
					if(hygTimeEndCur.Length > hygTimeEnd.Length) {
						hygTimeEnd=hygTimeEndCur;
					}
				}
			}
			//Make sure the time pattern is not longer than 39 characters (preserve old behavior).
			string timePatternFinal=hygTimeStart+provTimeTotal+hygTimeEnd;
			if(timePatternFinal.Length > 39) {
				timePatternFinal=timePatternFinal.Remove(39);
			}
			return timePatternFinal;
		}

		///<summary>Return the default time pattern for an appointment with no procedures attached using the AppointmentWithoutProcsDefaultLength pref.
		///Returns "/" if the defaultLength is set to 0. (preserves old behavior). Returned pattern is always in 5 minute increments.</summary>
		public static string GetApptTimePatternForNoProcs() {
			//No need to check RemotingRole; no call to db.
			int defaultLength=PrefC.GetInt(PrefName.AppointmentWithoutProcsDefaultLength);
			if(defaultLength > 0) {
				return new String('/',(defaultLength/5));
			}
			return "/";//Preserves old behavior
		}

		///<summary>Returns true if the patient has any broken appointments, future appointments, unscheduled appointments, or unsched planned appointments.  
		///This adds intelligence when user attempts to schedule an appointment by only showing the appointments for the patient when needed rather than always.
		///Setting exludePlannedAppts to true will remove them from the search.</summary>
		public static bool HasOutstandingAppts(long patNum,bool excludePlannedAppts=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum,excludePlannedAppts);
			}
			string command="SELECT COUNT(*) FROM appointment "
				+"WHERE PatNum='"+POut.Long(patNum)+"' "
				+"AND (AptStatus='"+POut.Long((int)ApptStatus.Broken)+"' "
				+"OR AptStatus='"+POut.Long((int)ApptStatus.UnschedList)+"' "
				+"OR (AptStatus='"+POut.Long((int)ApptStatus.Scheduled)+"' AND AptDateTime > "+DbHelper.Curdate()+" ) ";//future scheduled
				//planned appts that are already scheduled will also show because they are caught on the line above rather then on the next line
				if(!excludePlannedAppts) {
					command+="OR (AptStatus='"+POut.Long((int)ApptStatus.Planned)+"' "//planned, not sched
						+"AND NOT EXISTS(SELECT * FROM appointment a2 WHERE a2.PatNum='"+POut.Long(patNum)+"' AND a2.NextAptNum=appointment.AptNum)) ";
				}
				command+=")";
			if(Db.GetScalar(command)=="0") {
				return false;
			}
			return true;
		}

		///<summary>Returns true if appt has at least 1 proc attached.</summary>
		public static bool HasProcsAttached(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT COUNT(*) FROM procedurelog WHERE AptNum="+POut.Long(aptNum);
			if(PIn.Int(Db.GetScalar(command))>0) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if appt has at least 1 completed proc attached.</summary>
		public static bool HasCompletedProcsAttached(long aptNum,List<Procedure> listProcsAttachToApt=null) {
			if(listProcsAttachToApt!=null) { 
				return listProcsAttachToApt.Any(x => x.AptNum==aptNum && x.ProcStatus==ProcStat.C);
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),aptNum,listProcsAttachToApt);
			}
			string command=$"SELECT COUNT(*) FROM procedurelog " +
					$"WHERE AptNum={POut.Long(aptNum)} AND ProcStatus={POut.Int((int)ProcStat.C)}";
			return Db.GetScalar(command)!="0";
		}

		/// <summary>Checks if the specialty exists in the list of specialties for the clinic.</summary>
		public static bool ClinicHasSpecialty(Def defPatSpecialty,long clinicNum) {
			if(defPatSpecialty==null) {
				return true;//Patient does not have a specialty, so any clinic is fair game.
			}
			return DefLinks.GetDefLinksByType(DefLinkType.Clinic,defPatSpecialty.DefNum).Any(x => x.FKey==clinicNum);
		}

		/// <summary>Throws Exception. Determines if the patient for this appointment has a specialty which is included in the list of specialties 
		/// associated to the clinic for this appointment. UI independent.</summary>
		public static void HasSpecialtyConflict(long patNum,long clinicNum) {
			if(!PrefC.HasClinicsEnabled 
				|| (ApptSchedEnforceSpecialty)PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty)==ApptSchedEnforceSpecialty.DontEnforce) 
			{
				return;//Clinics off OR enforce preference off
			}
			Def def=Patients.GetPatientSpecialtyDef(patNum);
			if(!Appointments.ClinicHasSpecialty(def,clinicNum)) {
				string msgText="";
				switch((ApptSchedEnforceSpecialty)PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty)) {
					case ApptSchedEnforceSpecialty.Warn:
						//From MobileWeb, we will handle both Warn and Block with an error message.  The Warn option will direct the user to the desktop
						//application if they truly want to schedule this specialty mismatch appointment.
						msgText="The patient's specialty is not found in the operatory's/clinic's listed specialties.";
						break;
					case ApptSchedEnforceSpecialty.Block:
						msgText="Not allowed to schedule appointment. The patient's specialty is not found in the operatory's/clinic's listed specialties.";
						break;
					default:
						break;
				}
				throw new ODException(msgText,PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty));
			}
		}

		///<summary>Only called from the mobile server, not from any workstation.  Pass in an apptViewNum of 0 for now.  We might use that parameter later.</summary>
		public static string GetMobileBitmap(DateTime date,long apptViewNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),date,apptViewNum);
			}
			//For testing pass a resource image.
			return POut.Bitmap(Properties.Resources.ApptBackTest,ImageFormat.Gif);
		}

		public static DataTable GetPeriodEmployeeSchedTable(DateTime dateStart,DateTime dateEnd) {
			//No need to check RemotingRole; no call to db.
			return Schedules.GetPeriodEmployeeSchedTable(dateStart,dateEnd,0);
		}		

		///<summary>Used in Chart module to test whether a procedure is attached to an appointment with today's date. The procedure might have a different date if still TP status.  ApptList should include all appointments for this patient. Does not make a call to db.</summary>
		public static bool ProcIsToday(Appointment[] apptList,Procedure proc){
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<apptList.Length;i++){
				if(apptList[i].AptDateTime.Date==DateTime.Today
					&& apptList[i].AptNum==proc.AptNum
					&& (apptList[i].AptStatus==ApptStatus.Scheduled
					|| apptList[i].AptStatus==ApptStatus.Broken
					|| apptList[i].AptStatus==ApptStatus.Complete))
				{
					return true;
				}
			}
			return false;
		}

		public static Appointment TableToObject(DataTable table) {
			//No need to check RemotingRole; no call to db.
			if(table.Rows.Count==0) {
				return null;
			}
			return Crud.AppointmentCrud.TableToList(table)[0];
		}

		///<summary>Used by web to insert or update a given appt.
		///Dynamically charts procs based on appt.AppointmentTypeNum when created or changed.
		///This logic is attempting to mimic FormApptEdit when interacting with a new or existing appointment.</summary>
		public static void UpsertApptFromWeb(Appointment appt,bool canUpdateApptPattern=false,LogSources secLogSource=LogSources.MobileWeb){
			Patient pat=Patients.GetPat(appt.PatNum);
			List<Procedure> listProcsForApptEdit=Procedures.GetProcsForApptEdit(appt);//List of all procedures that would show in FormApptEdit.cs
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPat(pat.PatNum);
			List<InsSub> listInsSubs=new List<InsSub>();
			List<InsPlan> listInsPlans=new List<InsPlan>();
			if(listPatPlans.Count>0) {
				listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
				listInsPlans=InsPlans.GetByInsSubs(listInsSubs.Select(x => x.InsSubNum).ToList());
			}
			AppointmentType apptTypeCur=AppointmentTypes.GetOne(appt.AppointmentTypeNum);//When AppointmentTypeNum=0 this will be null.
			Appointment apptOld=GetOneApt(appt.AptNum);//When inserting a new appt this will be null.
			appt.IsNew=(apptOld==null);
			long apptTypeNumOld=(apptOld==null ? 0:apptOld.AppointmentTypeNum);
			List<Procedure> listProcsOnAppt;//Subset of listProcsForApptEdit. All procs associated to the given appt. Some aptNums may not be set yet.
			if(apptTypeCur!=null && apptTypeCur.AppointmentTypeNum!=apptTypeNumOld) {//Appointment type set and changed.
				//Dynamically added procs will exist in listProcsForApptEdit.
				listProcsOnAppt=ApptTypeMissingProcHelper(appt,apptTypeCur,listProcsForApptEdit,pat,canUpdateApptPattern,listPatPlans,listInsSubs,listInsPlans);
				appt.ColorOverride=apptTypeCur.AppointmentTypeColor;
			}
			else {
				listProcsOnAppt=listProcsForApptEdit.FindAll(x => x.AptNum!=0 && x.AptNum==appt.AptNum).Select(x => x.Copy()).ToList();
			}
			ProcedureLogic.SortProcedures(ref listProcsOnAppt);//Mimic FormApptEdit
			if(apptOld!=null && appt.Confirmed!=apptOld.Confirmed) {
				if(PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)==appt.Confirmed) {
					appt.DateTimeArrived=DateTime.Now;
				}
				else if(PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger)==appt.Confirmed) {
					appt.DateTimeSeated=DateTime.Now;
				}
				else if(PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger)==appt.Confirmed) {
					appt.DateTimeDismissed=DateTime.Now;
				}
			}
			#region Appointment insert or update
			#region Appt.ProcDescript
			//Mimics FormApptEdit.UpdateListAndDB(...)
			List<Procedure> listProcsForDescript=listProcsOnAppt.Select(x => x.Copy()).ToList();
			foreach(Procedure proc in listProcsForDescript) {
				//This allows Appointments.SetProcDescript(...) to associate all the passed in procs into AptCur.ProcDescript
				proc.AptNum=appt.AptNum;
				proc.PlannedAptNum=appt.AptNum;
			}
			Appointments.SetProcDescript(appt,listProcsForDescript);
			#endregion
			if(appt.IsNew) {
				#region Set Appt fields
				appt.InsPlan1=(listInsPlans.Count>=1 ? listInsPlans[0].PlanNum:0);
				appt.InsPlan2=(listInsPlans.Count>=2 ? listInsPlans[1].PlanNum:0);
				appt.DateTimeArrived=appt.AptDateTime.Date;
				appt.DateTimeSeated=appt.AptDateTime.Date;
				appt.DateTimeDismissed=appt.AptDateTime.Date;
				#endregion
				Insert(appt,appt.SecUserNumEntry);//Inserts the invalid signal
				SecurityLogs.MakeLogEntry(new SecurityLog()
				{
					PermType=Permissions.AppointmentCreate,
					UserNum=appt.SecUserNumEntry,
					LogDateTime=DateTime.Now,
					LogText=$"New appointment created from {secLogSource.GetDescription()} by "+Userods.GetUser(appt.SecUserNumEntry)?.UserName,
					PatNum=appt.PatNum,
					FKey=appt.AptNum,
					LogSource=secLogSource,
					DateTPrevious=appt.SecDateTEntry,
					CompName=Security.CurComputerName
				});
			}
			else {
				Update(appt,apptOld);//Inserts the invalid signal
				SecurityLogs.MakeLogEntry(new SecurityLog()
				{
					PermType=Permissions.AppointmentEdit,
					UserNum=appt.SecUserNumEntry,
					LogDateTime=DateTime.Now,
					LogText="Appointment updated from MobileWeb by "+Userods.GetUser(appt.SecUserNumEntry)?.UserName,
					PatNum=appt.PatNum,
					FKey=appt.AptNum,
					LogSource=LogSources.MobileWeb,
					DateTPrevious=appt.SecDateTEntry,
					CompName=Security.CurComputerName,
				});
			}
			#endregion
			#region Mimic FormApptEdit proc selection logic
			//At this point all pertinent procs have been charted or existed as a TPed proc already.
			//The below logic is attempting to mimic how FormApptEdit would make proc selections.
			List<int> listProcSelectedIndices=new List<int>();//Equivalent to current proc selections in FormApptEdit.
			List<long> listProcNumsAttachedStart=new List<long>();//Equivalent to OnLoad proc selections in FormApptEdit.
			foreach(Procedure proc in listProcsOnAppt){
				//All procs in listProcsOnAppt are treated like the user selected them in FormApptEdit.
				listProcSelectedIndices.Add(listProcsForApptEdit.FindIndex(x => x.ProcNum==proc.ProcNum));
				if(!appt.IsNew && proc.AptNum==appt.AptNum) {
					//When updating an existing appt some procs might have already been associated to the given appt.
					//Procs that have an AptNum=appt.AptNum were already set prior to this function.
					//This is equivalent to FormApptEdit loading and some procs being pre selected, used to identify attaching and detaching logic in below method calls.
					listProcNumsAttachedStart.Add(proc.ProcNum);
				}
			}
			#endregion
			List<Appointment> listAppointments=Appointments.GetForPat(appt.PatNum).ToList();
			Procedures.ProcsAptNumHelper(listProcsForApptEdit,appt,listAppointments,listProcSelectedIndices,listProcNumsAttachedStart,(appt.AptStatus==ApptStatus.Planned),secLogSource);
			Procedures.UpdateProcsInApptHelper(listProcsForApptEdit,pat,appt,apptOld,listInsPlans,listInsSubs,listProcSelectedIndices,true,false,secLogSource);
			//No need to create an invalid appt signal, the call to either Insert or Update will have already done so.
		}

		///<summary>If this method doesn't throw, then the appointment is considered valid.</summary>
		public static void ValidateApptForWeb(Appointment appt,bool allowDoubleBooking=false) {
			bool isPatternChanged;
			if(Patients.GetPat(appt.PatNum)==null) {
				//I don't suggest selecting a patient here because you can only change a patient for new appointments.
				throw new ODException("Patient selected is not valid.");
			}
			if(Providers.GetProv(appt.ProvNum)==null) {
				throw new ODException("Provider selected is not valid.  Please select a new provider.");
			}
			if(Providers.GetProv(appt.ProvNum).IsHidden) {
				throw new ODException("Provider selected is marked hidden.  Please select a new provider.");
			}
			if(!allowDoubleBooking && Appointments.TryAdjustAppointmentInCurrentOp(appt,false,true,out isPatternChanged)) {
				throw new ODException("Appointment overlaps existing appointment or blockout. Please change the appointment length.");
			}
			//Throws Exception if Warn/Block, returns false if DontEnforce or no conflict.
			try {
				Appointments.HasSpecialtyConflict(appt.PatNum,appt.ClinicNum);//"Don't Enforce" does nothing.
			}
			catch(ODException odex) {//Warn
				switch((ApptSchedEnforceSpecialty)odex.ErrorCode) {
					case ApptSchedEnforceSpecialty.Warn:
						throw new ODException(odex.Message+"\r\nTo schedule appointment anyway, please use the desktop application of OpenDental.");
					case ApptSchedEnforceSpecialty.Block:
						throw new ODException(odex.Message);
				}
			}
		}

		///<summary>Charts missing procedures for given appt and apptType.
		///Added procedures are reflected in listProcsForAppt, also returns subset procedures that will need to be associated to the given appt.
		///Dynamically charted procs do not have their aptNum set.</summary>
		public static List<Procedure> ApptTypeMissingProcHelper(Appointment appt,AppointmentType apptType,List<Procedure> listProcsForAppt,Patient pat=null,bool canUpdateApptPattern=true,
			List<PatPlan> listPatPlans=null,List<InsSub> listInsSubs=null,List<InsPlan> listInsPlans=null,List<Benefit>listBenefits=null)
		{
			List<Procedure> retList=new List<Procedure>();
			if(ListTools.In(appt.AptStatus,ApptStatus.PtNote,ApptStatus.PtNoteCompleted)) {
				return retList;//Patient notes can't have procedures associated to them.
			}
			List<ProcedureCode> listAptTypeProcs=ProcedureCodes.GetFromCommaDelimitedList(apptType.CodeStr);
			if(listAptTypeProcs.Count>0) {//AppointmentType is associated to procs.
				if(pat==null) {
					pat=Patients.GetPat(appt.PatNum);
				}
				if(listPatPlans==null) {
					listPatPlans=PatPlans.GetPatPlansForPat(pat.PatNum);
				}
				if(listInsSubs==null) {
					listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
				}
				if(listInsPlans==null) {
					listInsPlans=InsPlans.GetByInsSubs(listInsSubs.Select(x => x.InsSubNum).ToList());
				}
				if(listBenefits==null) {
					listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
				}
				List<SubstitutionLink> listSubstLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);
				long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(pat.PatNum,appt.AptDateTime); //Use the appointments date
				List<Fee> listFees=Fees.GetListFromObjects(listAptTypeProcs,null,null,//no existing procs to pull medCodes and provNums out of
					pat.PriProv,pat.SecProv,pat.FeeSched,listInsPlans,new List<long>(){appt.ClinicNum },new List<Appointment>(){appt},listSubstLinks,discountPlanNum);
				//possible (unlikely) issue: if a proc.ProvNumDefault is used, provider might be from different clinic, and a clinic fee override might, therefore, be missing. 
				bool isApptPlanned=(appt.AptStatus==ApptStatus.Planned);
				List<Procedure> listNewlyAddedProcs=new List<Procedure>();
				foreach(ProcedureCode procCodeCur in listAptTypeProcs) {
					bool existsInAppt=false;
					foreach(Procedure proc in listProcsForAppt) {
						if(proc.CodeNum==procCodeCur.CodeNum
							//The procedure has not already been added to the return list. 
							&& !retList.Any(x => x.ProcNum==proc.ProcNum)) 
						{
							//appt.AptNum can be 0.
							if((isApptPlanned && (proc.PlannedAptNum==0 || proc.PlannedAptNum == appt.AptNum))
								|| (!isApptPlanned && (proc.AptNum==0 || proc.AptNum == appt.AptNum)))
							{
								retList.Add(proc.Copy());
								existsInAppt=true;
								break;
							}
						}
					}
					if(!existsInAppt) { //if the procedure doesn't already exist in the appointment
						Procedure proc=Procedures.ConstructProcedureForAppt(procCodeCur.CodeNum,appt,pat,listPatPlans,listInsPlans,listInsSubs,listFees);
						Procedures.Insert(proc);
						List<ClaimProc> listClaimProcs=new List<ClaimProc>();
						Procedures.ComputeEstimates(proc,pat.PatNum,ref listClaimProcs,true,listInsPlans,listPatPlans,listBenefits,
							null,null,true,
							pat.Age,listInsSubs,null,false,false,listSubstLinks,false,listFees);
						retList.Add(proc.Copy());
						listNewlyAddedProcs.Add(proc);
					}
				}
				listProcsForAppt.AddRange(listNewlyAddedProcs);
				if(!isApptPlanned && listAptTypeProcs.Count > 0) {
					Procedures.SetDateFirstVisit(appt.AptDateTime.Date,1,pat);
				}
			}
			if(canUpdateApptPattern && apptType.Pattern!=null && apptType.Pattern!="") {
				appt.Pattern=apptType.Pattern;
			}
			return retList;
		}

		///<summary>Sends verification texts and e-mails after a patient confirms any type of appointment via WebSched.</summary>
		public static void SendWebSchedNotify(Appointment appt,PrefName typePref,PrefName textPref,PrefName emailSubjPref,PrefName emailBodyPref
			,PrefName emailType,bool logErrors=true) 
		{
			try {
				Patient pat=Patients.GetPat(appt.PatNum);
				Clinic clinic=Clinics.GetClinic(appt.ClinicNum);
				WebSchedVerifyType verificationType=(WebSchedVerifyType)PIn.Int(ClinicPrefs.GetPrefValue(typePref,appt.ClinicNum));
				if(verificationType==WebSchedVerifyType.None) {
					return;
				}
				CommOptOut commOptOut=CommOptOuts.Refresh(pat.PatNum);
				//Load in the templates and insert replacement fields
				string textTemplate=ClinicPrefs.GetPrefValue(textPref,appt.ClinicNum);
				textTemplate=Patients.ReplacePatient(textTemplate,pat);
				textTemplate=Appointments.ReplaceAppointment(textTemplate,appt);
				textTemplate=Clinics.ReplaceOffice(textTemplate,clinic);
				string emailSubj=OpenDentBusiness.EmailMessages.SubjectTidy(ClinicPrefs.GetPrefValue(emailSubjPref,appt.ClinicNum));
				emailSubj=Patients.ReplacePatient(emailSubj,pat);
				emailSubj=Appointments.ReplaceAppointment(emailSubj,appt);
				emailSubj=Clinics.ReplaceOffice(emailSubj,clinic);
				string emailBody=OpenDentBusiness.EmailMessages.BodyTidy(ClinicPrefs.GetPrefValue(emailBodyPref,appt.ClinicNum));
				emailBody=Patients.ReplacePatient(emailBody,pat,isHtmlEmail:true);
				emailBody=Appointments.ReplaceAppointment(emailBody,appt,isHtmlEmail:true);
				emailBody=Clinics.ReplaceOffice(emailBody,clinic,isHtmlEmail:true,doReplaceDisclaimer:true);
				//send text
				if(verificationType==WebSchedVerifyType.Text || verificationType==WebSchedVerifyType.TextAndEmail) {					
					try {
						if(commOptOut.IsOptedOut(CommOptOutMode.Text,CommOptOutType.Verify)) {
							throw new ODException("Patient has opted out of text automated messaging.");
						}
						else {
							SmsToMobiles.SendSmsSingle(pat.PatNum,pat.WirelessPhone,textTemplate,appt.ClinicNum,SmsMessageSource.Verify,true,canCheckBal:false);
						}
					}
					catch(ODException odex) {
						if(verificationType==WebSchedVerifyType.TextAndEmail && logErrors) {
							//SMS failed, so log, but continue so that we also try to send the email.
							Logger.WriteException(odex,"SendFollowUpErrors");
						}
						else if(verificationType==WebSchedVerifyType.Text) {
							throw odex;
						}
					}
				}
				//send e-mail
				if(verificationType==WebSchedVerifyType.Email || verificationType==WebSchedVerifyType.TextAndEmail) {
					EmailAddress addr=EmailAddresses.GetByClinic(appt.ClinicNum,true);
					if(addr==null) { //If clinic is not setup for email then don't bother trying to send.
						return;
					}
					if(commOptOut.IsOptedOut(CommOptOutMode.Email,CommOptOutType.Verify)) {
						throw new ODException("Patient has opted out of email automated messaging.");
					}
					EmailMessage msg=new EmailMessage()
					{
						PatNum=pat.PatNum,
						ToAddress=pat.Email,
						FromAddress=addr.GetFrom(),
						Subject=emailSubj,
						BodyText=emailBody,
						HtmlType=PIn.Enum<EmailType>(ClinicPrefs.GetPrefValue(emailType,appt.ClinicNum)),
						MsgDateTime=DateTime_.Now,
						SentOrReceived=EmailSentOrReceived.Sent,
						MsgType=EmailMessageSource.Verification
					};
					EmailMessages.PrepHtmlEmail(msg);
					EmailMessages.SendEmail(msg,addr);
				}
			}
			catch(Exception e) {
				if(logErrors) {
					Logger.WriteException(e,"SendFollowUpErrors");
				}
			}
		}

		///<summary>Replaces all appointment fields in the given message with the given appointment's information.  Returns the resulting string.
		///If apt is null, replaces fields with blanks.
		///Replaces: [ApptDate], [ApptTime], [ApptDayOfWeek], [ApptProcList], [date], [time]. </summary>
		///<summary>Replaces all appointment fields in the given message with the given appointment's information.  Returns the resulting string.
		///If apt is null, replaces fields with blanks.
		///Replaces: [ApptDate], [ApptTime], [ApptDayOfWeek], [ApptProcList], [date], [time]. </summary>
		public static string ReplaceAppointment(string message,Appointment apt,bool isHtmlEmail=false) {
			StringBuilder template=new StringBuilder(message);
			if(apt==null) {
				ReplaceTags.ReplaceOneTag(template,"[ApptDate]","",isHtmlEmail);
				ReplaceTags.ReplaceOneTag(template,"[date]","",isHtmlEmail);
				ReplaceTags.ReplaceOneTag(template,"[ApptTime]","",isHtmlEmail);
				ReplaceTags.ReplaceOneTag(template,"[time]","",isHtmlEmail);
				ReplaceTags.ReplaceOneTag(template,"[ApptDayOfWeek]","",isHtmlEmail);
				ReplaceTags.ReplaceOneTag(template,"[ApptProcsList]","",isHtmlEmail);
				ReplaceTags.ReplaceOneTag(template,"[ProvName]","",isHtmlEmail);
				ReplaceTags.ReplaceOneTag(template,"[ProvAbbr]","",isHtmlEmail);
				ReplaceTags.ReplaceOneTag(template,"[ApptTimeAskedArrive]","",isHtmlEmail);
				return template.ToString();
			}
			ReplaceTags.ReplaceOneTag(template,"[ApptDate]",apt.AptDateTime.ToString(PrefC.PatientCommunicationDateFormat),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[date]",apt.AptDateTime.ToString(PrefC.PatientCommunicationDateFormat),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ApptTime]",apt.AptDateTime.ToShortTimeString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[time]",apt.AptDateTime.ToShortTimeString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ApptDayOfWeek]",apt.AptDateTime.DayOfWeek.ToString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ProvName]",Providers.GetFormalName(apt.ProvNum),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ProvAbbr]",Providers.GetAbbr(apt.ProvNum),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ApptTimeAskedArrive]",
				apt.DateTimeAskedToArrive.Year>1880?apt.DateTimeAskedToArrive.ToShortTimeString():apt.AptDateTime.ToShortTimeString(),
				isHtmlEmail);
			if(message.Contains("[ApptProcsList]")) {
				bool isPlanned=false;
				if(apt.AptStatus==ApptStatus.Planned) {
					isPlanned=true;
				}
				List<Procedure> listProcs=Procedures.GetProcsForSingle(apt.AptNum,isPlanned);
				List<ProcedureCode> listProcCodes=new List<ProcedureCode>();
				ProcedureCode procCode=new ProcedureCode();
				StringBuilder strProcs=new StringBuilder();
				string procDescript="";
				for(int i=0;i<listProcs.Count;i++) {
					procCode=ProcedureCodes.GetProcCode(listProcs[i].CodeNum);
					if(procCode.LaymanTerm=="") {
						procDescript=procCode.Descript;
					}
					else {
						procDescript=procCode.LaymanTerm;
					}
					if(i>0) {
						strProcs.Append("\n");
					}
					strProcs.Append(listProcs[i].ProcDate.ToShortDateString()+" "+procCode.ProcCode+" "+procDescript);
				}
				ReplaceTags.ReplaceOneTag(template,"[ApptProcsList]",strProcs.ToString(),isHtmlEmail);
			}
			return template.ToString();
		}

		///<summary>Assigns the ProvNum, ProvHyg, IsHygiene, and ClinicNum to the appointment based on the appointment's operatory.
		///Returns the updated appointment.</summary>
		public static Appointment AssignFieldsForOperatory(Appointment aptCur) {
			Appointment apt=aptCur.Copy();
			Operatory curOp=Operatories.GetOperatory(apt.Op);
			List<Schedule> schedListPeriod=Schedules.RefreshDayEdit(apt.AptDateTime);
			long assignedDent=Schedules.GetAssignedProvNumForSpot(schedListPeriod,curOp,false,apt.AptDateTime);
			long assignedHyg=Schedules.GetAssignedProvNumForSpot(schedListPeriod,curOp,true,apt.AptDateTime);
			//the section below regarding providers is overly wordy because it's copied from ContrAppt.pinBoard_MouseUp to make maint easier.
			if(assignedDent!=0) {
				apt.ProvNum=assignedDent;
			}
			if(assignedHyg!=0 || PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly)) {
				apt.ProvHyg=assignedHyg;
			}
			if(curOp.IsHygiene) {
				apt.IsHygiene=true;
			}
			else {//op not marked as hygiene op
				if(assignedDent==0) {//no dentist assigned
					if(assignedHyg!=0) {//hyg is assigned (we don't really have to test for this)
						apt.IsHygiene=true;
					}
				}
				else {//dentist is assigned
					if(assignedHyg==0) {//hyg is not assigned
						apt.IsHygiene=false;
					}
					//if both dentist and hyg are assigned, it's tricky
					//only explicitly set it if user has a dentist assigned to the op
					if(curOp.ProvDentist!=0) {
						apt.IsHygiene=false;
					}
				}
			}
			if(curOp.ClinicNum!=0) {
				apt.ClinicNum=curOp.ClinicNum;
			}
			return apt;
		}

		///<summary>Used to set an appointment complete when it is right-clicked set complete.  Insert an invalid appointment signalod.</summary>
		public static ODTuple<Appointment,List<Procedure>> CompleteClick(Appointment apt,List<Procedure> listProcsForAppt,bool removeCompletedProcs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ODTuple<Appointment,List<Procedure>>>(MethodBase.GetCurrentMethod(),apt,listProcsForAppt,removeCompletedProcs);
			}
			Family fam=Patients.GetFamily(apt.PatNum);
			Patient pat=fam.GetPatient(apt.PatNum);
			List<InsSub> SubList=InsSubs.RefreshForFam(fam);
			List<InsPlan> PlanList=InsPlans.RefreshForSubList(SubList);
			List<PatPlan> PatPlanList=PatPlans.Refresh(apt.PatNum);
			DateTime datePrevious=apt.DateTStamp;
			if(apt.AptStatus==ApptStatus.PtNote) {
				Appointments.SetAptStatus(apt,ApptStatus.PtNoteCompleted);//Sets the invalid signal
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,apt.PatNum,
					apt.AptDateTime.ToString()+", Patient NOTE Set Complete",
					apt.AptNum,datePrevious);//shouldn't ever happen, but don't allow procedures to be completed from notes
			}
			else {
				InsSub sub1=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,PatPlanList,PlanList,SubList)),SubList);
				InsSub sub2=InsSubs.GetSub(PatPlans.GetInsSubNum(PatPlanList,PatPlans.GetOrdinal(PriSecMed.Secondary,PatPlanList,PlanList,SubList)),SubList);
				Appointments.SetAptStatusComplete(apt,sub1.PlanNum,sub2.PlanNum);//Sets the invalid signal
				Procedures.SetCompleteInAppt(apt,PlanList,PatPlanList,pat,SubList,removeCompletedProcs);//loops through each proc
				if(apt.AptStatus==ApptStatus.Complete) { // seperate log entry for editing completed appointments.
					SecurityLogs.MakeLogEntry(Permissions.AppointmentCompleteEdit,apt.PatNum,
						apt.ProcDescript+", "+ apt.AptDateTime.ToString()+", Set Complete",
						apt.AptNum,datePrevious);//Log showing the appt. is set complete
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,apt.PatNum,
						apt.ProcDescript+", "+ apt.AptDateTime.ToString()+", Set Complete",
						apt.AptNum,datePrevious);//Log showing the appt. is set complete
				}
				//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					//S14 - Appt Modification event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(pat,fam.GetPatient(pat.Guarantor),EventTypeHL7.S14,apt);
					//Will be null if there is no outbound SIU message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=apt.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=pat.PatNum;
						HL7Msgs.Insert(hl7Msg);
						if(ODBuild.IsDebug()) {
							Console.WriteLine(messageHL7.ToString());
						}
					}
				}
				if(HieClinics.IsEnabled()) {
					HieQueues.Insert(new HieQueue(pat.PatNum));
				}
			}
			Recalls.SynchScheduledApptFull(apt.PatNum);
			//No need to enter an invalid signal here, the SetAptStatus and SetAptStatusComplete calls will have already done so
			return new ODTuple<Appointment,List<Procedure>>(apt,listProcsForAppt);
		}
				
		///<summary>Determines if a specified Appointment is a recall Appointment.  
		///Pass in listApptProcs if the Procedures on the Appointment may not match the database.
		///A recall appointment is defined as follows: 
		///1) Patient has a recall table entry.
		///2) Recall is not disabled
		///3) Recall is of a RecallType in the list specified by preference RecallTypesShowingInList.
		///4) Appointment.AptDateTime matches the DateScheduled on a Recall for the corresponding Patient and Appointment has at least one attached 
		///Procedure that matches that Recall's RecallType's corresponding Procedure Code triggers.
		///</summary>
		public static bool IsRecallAppointment(Appointment appt,List<Procedure> listApptProcs=null) {
			if(appt==null) {
				return false;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),appt,listApptProcs);
			}
			if(listApptProcs==null) {
				listApptProcs=Procedures.GetProcsForSingle(appt.AptNum,appt.AptStatus==ApptStatus.Planned);//Get this appt's selected procs if not specified.
			}
			if(listApptProcs.Count==0) {
				return false;
			}
			List<Recall> listRecalls=Recalls.GetList(appt.PatNum);//Get the recalls for this patient only.
			string strRecallTypesShowingInList=PrefC.GetString(PrefName.RecallTypesShowingInList);
			if(!string.IsNullOrEmpty(strRecallTypesShowingInList)) {//Limit RecallTypes to check against if RecallTypesShowingInList preference is set.
				List<long> listRecallTypeNums=strRecallTypesShowingInList.Split(new char[] {',' },StringSplitOptions.RemoveEmptyEntries)
					.Select(x => PIn.Long(x)).ToList();
				listRecalls=listRecalls.FindAll(x => ListTools.In(x.RecallTypeNum,listRecallTypeNums));
			}
			foreach(Recall recall in listRecalls) {
				if(recall.IsDisabled) {
					continue;//Skip disabled recalls.
				}
				List<long> listTriggerCodeNums=RecallTriggers.GetForType(recall.RecallTypeNum).Select(x => x.CodeNum).ToList();
				if(recall.DateScheduled.Date==appt.AptDateTime.Date && listApptProcs.Exists(x => ListTools.In(x.CodeNum,listTriggerCodeNums))) {
					return true;//Appt is scheduled on the day of the recall, and appt includes a corresponding recall trigger proc.
				}
			}
			return false;
		}

		#endregion
		
		///<summary>All appointment statuses that are real appointments and show up on the apppointment book.</summary>
		public static List<ApptStatus> ListScheduledApptStatuses {
			get {
				return new List<ApptStatus> {
					ApptStatus.Scheduled,
					ApptStatus.Complete,
					ApptStatus.Broken
				};
			}
		}

		///<summary>Used to help organize and display referral information for the appointment bubble.</summary>
		public class ApptBubbleReferralInfo {
			public ReferralType RefType;
			public string Name;
			public string PhoneNumber;

			public ApptBubbleReferralInfo(ReferralType refType,string name,string phoneNumber) {
				RefType=refType;
				Name=name;
				PhoneNumber=phoneNumber;
			}
		}
	}
	
	///<summary>Holds information about a provider's Schedule. Not actual database table.</summary>
	[Serializable]
	public class ApptSearchProviderSchedule {
		///<summary>FK to Provider</summary>
		public long ProviderNum;
		///<summary>Date of the ProviderSchedule.</summary>
		public DateTime SchedDate;
		///<summary>This contains a bool for each 5 minute block throughout the day. True means provider is scheduled to work, False means provider is not scheduled to work.</summary>
		public bool[] IsProvScheduled;
		///<summary>This contains a bool for each 5 minute block throughout the day. True means available, False means something is scheduled there or the provider is not scheduled to work.</summary>
		public bool[] IsProvAvailable;

		///<summary>Constructor.</summary>
		public ApptSearchProviderSchedule() {
			IsProvScheduled=new bool[288];
			IsProvAvailable=new bool[288];
		}
	}

	///<summary>A lite version of the Appointment object designed for populating the OtherAppts window.</summary>
	[Serializable]
	public class ApptOther {
		public long AptNum;
		public ApptStatus AptStatus;
		public long NextAptNum;
		public long ProvNum;
		public long ClinicNum;
		public DateTime AptDateTime;
		public string Pattern;
		public string ProcDescript;
		public string Note;
		public long Op;

		///<summary>Required for serialization purposes.</summary>
		public ApptOther() {
		}

		///<summary>Only preserves information from the appointment passed in that is necessary to fill the OtherAppts window.</summary>
		public ApptOther(Appointment appt) {
			AptNum=appt.AptNum;
			AptStatus=appt.AptStatus;
			NextAptNum=appt.NextAptNum;
			ProvNum=appt.ProvNum;
			ClinicNum=appt.ClinicNum;
			AptDateTime=appt.AptDateTime;
			Pattern=appt.Pattern;
			ProcDescript=appt.ProcDescript;
			Note=appt.Note;
			Op=appt.Op;
		}
	}

	///<summary>Holds information about broken appt logic. Not actual database table.</summary>
	public enum BrokenApptProcedure {
		///<summary>0 - Do not chart a procedure.</summary>
		None,
		///<summary>1 - Chart D9986.</summary>
		Missed,
		///<summary>2 - Chart D9987.</summary>
		Cancelled,
		///<summary> - Chart D9986 and D9987.</summary>
		Both
	}

	public class AppointmentForApi {
		public Appointment AppointmentCur;
		public DateTime DateTimeServer;
	}
}
