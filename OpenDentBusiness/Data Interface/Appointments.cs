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
		#region Prompt constant strings
		public const string PROMPT_ListAptsToDelete = "One or more procedures are attached to another appointment.\r\n"
					+ "All selected procedures will be detached from the other appointment which will result in its deletion.\r\n"
					+ "Continue?";
		public const string PROMPT_NotPlannedProcsConcurrent = "One or more procedures are attached to another appointment.\r\n"
					+ "All selected procedures will be detached from the other appointment.\r\n"
					+ "Continue?";
		public const string PROMPT_CompletedProceduresBeingMoved = "Cannot attach procedures to this appointment that were set complete by another user. Please try again.";
		#endregion Prompt constant strings

		#region Get Methods
		public static DataTable GetCommTable(string patNum,long aptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,aptNum);
			}
			DataTable table=new DataTable("Comm");
			DataRow dataRow;
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
			DataTable tableRawComm=Db.GetTable(command);
			for(int i=0;i<tableRawComm.Rows.Count;i++) {
				dataRow=table.NewRow();
				dataRow["commDateTime"]=PIn.DateT(tableRawComm.Rows[i]["commDateTime"].ToString()).ToShortDateString();
				dataRow["CommlogNum"]=tableRawComm.Rows[i]["CommlogNum"].ToString();
				dataRow["CommSource"]=tableRawComm.Rows[i]["CommSource"].ToString();
				dataRow["CommType"]=tableRawComm.Rows[i]["CommType"].ToString();
				dataRow["Note"]=tableRawComm.Rows[i]["Note"].ToString();
				dataRow["EmailMessageNum"]=0;
				dataRow["EmailMessageHideIn"]=0;
				table.Rows.Add(dataRow);
			}
			if(aptNum==0) {
				table.DefaultView.Sort="commDateTime";
				return table.DefaultView.ToTable();
			}
			command=@"SELECT emailmessage.MsgDateTime,emailmessage.Subject,emailmessage.EmailMessageNum,emailmessage.HideIn
				FROM emailmessage
				WHERE emailmessage.PatNum="+POut.String(patNum)+@"
				AND emailmessage.AptNum="+POut.Long(aptNum);
			tableRawComm.Clear();
			tableRawComm=Db.GetTable(command);
			for(int i=0;i<tableRawComm.Rows.Count;i++) {
				dataRow=table.NewRow();
				dataRow["commDateTime"]=PIn.DateT(tableRawComm.Rows[i]["MsgDateTime"].ToString()).ToShortDateString();
				dataRow["EmailMessageNum"]=tableRawComm.Rows[i]["EmailMessageNum"].ToString();
				dataRow["CommlogNum"]=0;
				dataRow["CommSource"]="";
				dataRow["Subject"]=tableRawComm.Rows[i]["Subject"].ToString();
				dataRow["EmailMessageHideIn"]=tableRawComm.Rows[i]["HideIn"].ToString();
				table.Rows.Add(dataRow);
			}
			table.DefaultView.Sort="commDateTime";
			return table.DefaultView.ToTable();
		}

		public static DataTable GetMiscTable(string aptNum,bool isPlanned) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),aptNum,isPlanned);
			}
			DataTable table=new DataTable("Misc");
			DataRow dataRow;
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
			DataTable tableRaw=Db.GetTable(command);
			DateTime dateTimeForTracking;
			DateTime dateTimeDue;
			//for(int i=0;i<raw.Rows.Count;i++) {//always return one row:
			dataRow=table.NewRow();
			dataRow["LabCaseNum"]="0";
			dataRow["labDescript"]="";
			if(tableRaw.Rows.Count>0){
				dataRow["LabCaseNum"]=tableRaw.Rows[0]["LabCaseNum"].ToString();
				dataRow["labDescript"]=tableRaw.Rows[0]["Description"].ToString();
				dateTimeForTracking=PIn.DateT(tableRaw.Rows[0]["DateTimeChecked"].ToString());
				if(dateTimeForTracking.Year>1880){
					dataRow["labDescript"]+=", "+Lans.g("FormApptEdit","Quality Checked");
				}
				else{
					dateTimeForTracking=PIn.DateT(tableRaw.Rows[0]["DateTimeRecd"].ToString());
					if(dateTimeForTracking.Year>1880){
						dataRow["labDescript"]+=", "+Lans.g("FormApptEdit","Received");
					}
					else{
						dateTimeForTracking=PIn.DateT(tableRaw.Rows[0]["DateTimeSent"].ToString());
						if(dateTimeForTracking.Year>1880){
							dataRow["labDescript"]+=", "+Lans.g("FormApptEdit","Sent");//sent but not received
						}
						else{
							dataRow["labDescript"]+=", "+Lans.g("FormApptEdit","Not Sent");
						}
						dateTimeDue=PIn.DateT(tableRaw.Rows[0]["DateTimeDue"].ToString());
						if(dateTimeDue.Year>1880) {
							dataRow["labDescript"]+=", "+Lans.g("FormAppEdit","Due: ")+dateTimeDue.ToString("ddd")+" "
								+dateTimeDue.ToShortDateString()+" "+dateTimeDue.ToShortTimeString();
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
			tableRaw=Db.GetTable(command);
			dataRow["requirements"]="";
			for(int i=0;i<tableRaw.Rows.Count;i++){
				if(i!=0){
					dataRow["requirements"]+="\r\n";
				}
				dataRow["requirements"]+=tableRaw.Rows[i]["LName"].ToString()+", "+tableRaw.Rows[i]["FName"].ToString()
					+": "+tableRaw.Rows[i]["Descript"].ToString();
			}
			table.Rows.Add(dataRow);
			return table;
		}

		///<summary></summary>
		public static List<AppointmentWithServerDT> GetAppointmentsForApi(int limit,int offset,DateTime dateTStart,DateTime dateTEnd,DateTime dateTStamp,long clinicNum,long patNum,int aptStatus,long operatoryNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AppointmentWithServerDT>>(MethodBase.GetCurrentMethod(),limit,offset,dateTStart,dateTEnd,dateTStamp,clinicNum,patNum,aptStatus,operatoryNum);
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime >= "+POut.DateT(dateTStart)+" "
				+"AND AptDateTime < "+POut.DateT(dateTEnd)+" "
				+"AND DateTStamp >= "+POut.DateT(dateTStamp)+" ";
			if(clinicNum>-1){
				command+="AND ClinicNum="+POut.Long(clinicNum)+" ";
			}
			if(patNum>0) {
				command+="AND PatNum="+POut.Long(patNum)+" ";
			}
			if(aptStatus>-1) {
				command+="AND AptStatus="+POut.Int(aptStatus)+" ";
			}
			if(operatoryNum>-1) {
				command+="AND Op="+POut.Long(operatoryNum)+" ";
			}
			command+="ORDER BY AptDateTime,AptNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before appts for rigorous inclusion of appts
			List<Appointment> listAppointments=Crud.AppointmentCrud.TableToList(Db.GetTable(command));
			List<AppointmentWithServerDT> listAppointmentsWithServerDTs=new List<AppointmentWithServerDT>();//For APIs
			for(int i=0;i<listAppointments.Count;i++) {//list can be empty
				AppointmentWithServerDT appointmentWithServerDT=new AppointmentWithServerDT();
				appointmentWithServerDT.AppointmentCur=listAppointments[i];
				appointmentWithServerDT.DateTimeServer=dateTimeServer;
				listAppointmentsWithServerDTs.Add(appointmentWithServerDT);
			}
			return listAppointmentsWithServerDTs;
		}

		///<summary>Gets appointments made through WebSched for the API. The type of appointment is indicated by the eServiceType which can be NewPat, ExistingPat, Recall, or ASAP. </summary>
		public static DataTable GetWebSchedAppointmentsForApi(int limit,int offset,DateTime dateTStart,DateTime dateTEnd,DateTime dateTStamp,long clinicNum,string dateTimeFormatString) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<DataTable>(MethodBase.GetCurrentMethod(),limit,offset,dateTStart,dateTEnd,dateTStamp,clinicNum,dateTimeFormatString);
			}
			DataTable tableReturn=new DataTable("AppointmentsListForApi");
			DataTable tableAppointments;
			DataRow dataRow;
			//Define columns for tableReturn
			tableReturn.Columns.Add("AptNum",typeof(long));
			tableReturn.Columns.Add("PatNum",typeof(long));
			tableReturn.Columns.Add("AptStatus");
			tableReturn.Columns.Add("Pattern");
			tableReturn.Columns.Add("Confirmed",typeof(long));
			tableReturn.Columns.Add("confirmed");
			tableReturn.Columns.Add("Op",typeof(long));
			tableReturn.Columns.Add("Note");
			tableReturn.Columns.Add("ProvNum",typeof(long));
			tableReturn.Columns.Add("provAbbr");
			tableReturn.Columns.Add("ProvHyg",typeof(long));
			tableReturn.Columns.Add("AptDateTime");
			tableReturn.Columns.Add("NextAptNum",typeof(long));
			tableReturn.Columns.Add("UnschedStatus",typeof(long));
			tableReturn.Columns.Add("unschedStatus");
			tableReturn.Columns.Add("ProcDescript");
			tableReturn.Columns.Add("ClinicNum",typeof(long));
			tableReturn.Columns.Add("IsHygiene");
			tableReturn.Columns.Add("DateTStamp");
			tableReturn.Columns.Add("DateTimeArrived");
			tableReturn.Columns.Add("DateTimeSeated");
			tableReturn.Columns.Add("DateTimeDismissed");
			tableReturn.Columns.Add("AppointmentTypeNum",typeof(long));
			tableReturn.Columns.Add("eServiceLogType");
			tableReturn.Columns.Add("serverDateTime");
			//Run Query
			List<eServiceType> listEserviceTypes=new List<eServiceType>{eServiceType.WSRecall,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSAsap};
			string command="SELECT appointment.*, eservicelog.EServiceType FROM appointment ";
			command+="LEFT JOIN eservicelog ON appointment.AptNum=eservicelog.FKey "
				+"AND eservicelog.EserviceAction="+POut.Int((int)eServiceAction.WSAppointmentScheduledFromServer)+" "
				+"AND eservicelog.EServiceType IN ("+string.Join(",",listEserviceTypes.Select(x => (int)x))+") ";
			command+="WHERE appointment.AptDateTime >= "+POut.DateT(dateTStart)+" "
				+"AND appointment.AptDateTime < "+POut.DateT(dateTEnd)+" "
				+"AND appointment.DateTStamp >= "+POut.DateT(dateTStamp)+" ";
			if(clinicNum>-1) {
				command+="AND appointment.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			command+="ORDER BY appointment.AptDateTime,appointment.AptNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before appts for rigorous inclusion of appts
			tableAppointments=Db.GetTable(command);
			//Organize results
			for(int i=0;i<tableAppointments.Rows.Count;i++) {//Paging handled in query.
				dataRow=tableReturn.NewRow();
				dataRow["AptNum"]=PIn.Long(tableAppointments.Rows[i]["AptNum"].ToString());
				dataRow["PatNum"]=PIn.Long(tableAppointments.Rows[i]["PatNum"].ToString());
				dataRow["AptStatus"]=(ApptStatus)PIn.Int(tableAppointments.Rows[i]["AptStatus"].ToString());
				dataRow["Pattern"]=tableAppointments.Rows[i]["Pattern"].ToString();
				dataRow["Confirmed"]=PIn.Long(tableAppointments.Rows[i]["Confirmed"].ToString());
				dataRow["confirmed"]=OpenDentBusiness.Defs.GetName(OpenDentBusiness.DefCat.ApptConfirmed,PIn.Long(tableAppointments.Rows[i]["Confirmed"].ToString()));
				dataRow["Op"]=PIn.Long(tableAppointments.Rows[i]["Op"].ToString());
				dataRow["Note"]=tableAppointments.Rows[i]["Note"].ToString();
				dataRow["ProvNum"]=PIn.Long(tableAppointments.Rows[i]["ProvNum"].ToString());
				dataRow["provAbbr"]=Providers.GetAbbr(PIn.Long(tableAppointments.Rows[i]["ProvNum"].ToString()));
				dataRow["ProvHyg"]=tableAppointments.Rows[i]["ProvHyg"].ToString();
				dataRow["AptDateTime"]=PIn.DateT(tableAppointments.Rows[i]["AptDateTime"].ToString()).ToString(dateTimeFormatString);
				dataRow["NextAptNum"]=PIn.Long(tableAppointments.Rows[i]["NextAptNum"].ToString());
				dataRow["UnschedStatus"]=PIn.Long(tableAppointments.Rows[i]["UnschedStatus"].ToString());
				dataRow["unschedStatus"]=OpenDentBusiness.Defs.GetName(OpenDentBusiness.DefCat.RecallUnschedStatus,PIn.Long(tableAppointments.Rows[i]["UnschedStatus"].ToString()));
				dataRow["ProcDescript"]=tableAppointments.Rows[i]["ProcDescript"].ToString();
				dataRow["ClinicNum"]=PIn.Long(tableAppointments.Rows[i]["ClinicNum"].ToString());
				dataRow["IsHygiene"]=PIn.Bool(tableAppointments.Rows[i]["IsHygiene"].ToString()).ToString();
				dataRow["DateTStamp"]=PIn.DateT(tableAppointments.Rows[i]["DateTStamp"].ToString()).ToString(dateTimeFormatString);
				dataRow["DateTimeArrived"]=PIn.DateT(tableAppointments.Rows[i]["DateTimeArrived"].ToString()).ToString(dateTimeFormatString);
				dataRow["DateTimeSeated"]=PIn.DateT(tableAppointments.Rows[i]["DateTimeSeated"].ToString()).ToString(dateTimeFormatString);
				dataRow["DateTimeDismissed"]=PIn.DateT(tableAppointments.Rows[i]["DateTimeDismissed"].ToString()).ToString(dateTimeFormatString);
				dataRow["AppointmentTypeNum"]=PIn.Long(tableAppointments.Rows[i]["AppointmentTypeNum"].ToString());
				dataRow["eServiceLogType"]=(ApiEServiceLogType)PIn.Int(tableAppointments.Rows[i]["EServiceType"].ToString());
				dataRow["serverDateTime"]=dateTimeServer.ToString(dateTimeFormatString);
				tableReturn.Rows.Add(dataRow);
			}
			return tableReturn;
		}

		///<summary>Returns a list of appointments that are scheduled between start date and end datetime. 
		///The end of the appointment must also be in the period.</summary>
		public static List<Appointment> GetAppointmentsForPeriod(DateTime dateTStart,DateTime dateTEnd,params ApptStatus[] apptStatusIgnoreArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateTStart,dateTEnd,apptStatusIgnoreArray);
			}
			//jsalmon - leaving start.Date even though this doesn't make much sense.
			List<Appointment> listAppointments=GetAppointmentsStartingWithinPeriod(dateTStart.Date,dateTEnd,apptStatusIgnoreArray);
			//Now that we have all appointments that start within our period, make sure that the entire appointment fits within.
			for(int i=listAppointments.Count-1;i>=0;i--) {
				if(listAppointments[i].AptDateTime.AddMinutes(listAppointments[i].Pattern.Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement))>dateTEnd) {
					listAppointments.RemoveAt(i);
				}
			}
			return listAppointments;
		}

		///<summary>Returns a list of appointments that are scheduled between start date and end date.
		///This method only considers the AptDateTime and does not check to see if the appointment </summary>
		public static List<Appointment> GetAppointmentsStartingWithinPeriod(DateTime dateTStart,DateTime dateTEnd,params ApptStatus[] apptStatusIgnoreArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateTStart,dateTEnd,apptStatusIgnoreArray);
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime >= "+POut.DateT(dateTStart)+" "
				+"AND AptDateTime <= "+POut.DateT(dateTEnd);
				if(apptStatusIgnoreArray.Length > 0) {
					command+="AND AptStatus NOT IN (";
					for(int i=0;i<apptStatusIgnoreArray.Length;i++) {
						if(i > 0) {
							command+=",";
						}
						command+=POut.Int((int)apptStatusIgnoreArray[i]);
					}
					command+=") ";
				}
			List<Appointment> listAppointments=Crud.AppointmentCrud.TableToList(Db.GetTable(command));
			return listAppointments;
		}

		///<summary>Gets all appointments scheduled in the operatories passed in that fall within the start and end dates.
		///Does not currently consider the time portion of the DateTimes passed in.</summary>
		public static List<Appointment> GetAppointmentsForOpsByPeriod(List<long> listOpNums,DateTime dateTStart,DateTime dateTEnd=new DateTime(),
			Logger.IWriteLine log=null,List<long> listProvNums=null) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),listOpNums,dateTStart,dateTEnd,log,listProvNums);
			}
			string command="SELECT * FROM appointment WHERE Op > 0 ";
			if(listOpNums!=null && listOpNums.Count > 0) {
				command+="AND Op IN("+String.Join(",",listOpNums)+") ";
			}
			//It is very important to format these filters as DateT. That will allow the index to be used. 
			//Truncate dateStart/dateEnd down to .Date in order to mimic the behavior of DbHelper.DtimeToDate().
			command+="AND AptStatus!="+POut.Int((int)ApptStatus.UnschedList)+" "
				+"AND AptDateTime>="+POut.DateT(dateTStart.Date)+" ";
			if(dateTEnd.Year > 1880) {
				command+="AND AptDateTime<="+POut.DateT(dateTEnd.Date.AddDays(1))+" ";
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
		public static List<Appointment> GetApptsForDatesOps(List<AsapComms.DateTOpNum> listDateTOpNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),listDateTOpNums);
			}
			if(listDateTOpNums.Count==0) {
				return new List<Appointment>();
			}
			string command="SELECT * FROM appointment WHERE AptStatus NOT IN("+POut.Int((int)ApptStatus.UnschedList)+","
				+POut.Int((int)ApptStatus.Planned)+") AND (";
			for(int i=0;i<listDateTOpNums.Count();i++){
				if(i>0){
					command+=" OR";
				}
				command+=" ("+DbHelper.BetweenDates("AptDateTime",listDateTOpNums[i].DateTAppt,listDateTOpNums[i].DateTAppt)
					+" AND Op=" + POut.Long(listDateTOpNums[i].OpNum)+")";
			}
			command+=")";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Returns a list containing every appointment associated to the provided patnum.</summary>
		public static List<Appointment> GetAppointmentsForPat(params long[] patNumArray) {
			if(patNumArray.IsNullOrEmpty()) {
				return new List<Appointment>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),patNumArray);
			}
			string command="SELECT * FROM appointment WHERE PatNum IN("+string.Join(",",patNumArray.Select(x => POut.Long(x)))+") ORDER BY AptDateTime";
			return Crud.AppointmentCrud.TableToList(Db.GetTable(command));
		}

		///<summary>Returns a dictionary containing the last completed appointment date of each patient.</summary>
		public static SerializableDictionary<long,DateTime> GetDateLastVisit() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,DateTime>(MethodBase.GetCurrentMethod());
			}
			//==Jordan dictionaries are not allowed, especially ones that get ALL pats.
			//But since this is only used by DemandForce bridge we won't bother fixing it right now.
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,List<Appointment>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			//==Jordan dictionaries are not allowed, especially ones that get ALL pats.
			//But since this is only used by DemandForce bridge we won't bother fixing it right now.
			SerializableDictionary<long,List<Appointment>> retVal=new SerializableDictionary<long,List<Appointment>>();
			string command="SELECT * "
					+"FROM appointment "
					+"WHERE AptStatus IN ("+POut.Int((int)ApptStatus.Scheduled)+","+POut.Int((int)ApptStatus.Complete)+") "
					+"AND "+DbHelper.DtimeToDate("AptDateTime")+">="+POut.Date(dateFrom)+" AND "+DbHelper.DtimeToDate("AptDateTime")+"<="+POut.Date(dateTo);
			List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
			for(int i=0;i<listAppointments.Count;i++) {
				if(retVal.ContainsKey(listAppointments[i].PatNum)) {
					retVal[listAppointments[i].PatNum].Add(listAppointments[i]);//Add the current appointment to the list of appointments for the patient.
				}
				else {
					retVal.Add(listAppointments[i].PatNum,new List<Appointment> { listAppointments[i] });//Initialize the list of appointments for the current patient and include the current appoinment.
				}
			}
			return retVal;
		}

		/// <summary>Get a dictionary of all procedure codes for all scheduled, ASAP, and completed appointments</summary>
		public static SerializableDictionary<long,List<long>> GetCodeNumsAllApts() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,List<long>>(MethodBase.GetCurrentMethod());
			}
			//==Jordan dictionaries are not allowed, especially ones that get ALL appointments.
			//But since this is only used by DemandForce bridge we won't bother fixing it right now.
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
		public static List<Appointment> GetAppointmentsForProcs(List<Procedure> listProcedures) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),listProcedures);
			}
			if(listProcedures.Count < 1) {
				return new List<Appointment>();
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptNum IN("+string.Join(",",listProcedures.Select(x => x.AptNum).Distinct().ToList())+") "
					+"OR AptNum IN("+string.Join(",",listProcedures.Select(x => x.PlannedAptNum).Distinct().ToList())+") "
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Returns a list of appointments that are scheduled (have scheduled or ASAP status) to start between start date and end date.</summary>
		public static List<Appointment> GetSchedApptsForPeriod(DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime >= "+POut.DateT(dateStart)+" "
				+"AND AptDateTime <= "+POut.DateT(dateEnd)+" "
				+"AND AptStatus="+POut.Int((int)ApptStatus.Scheduled);
			List<Appointment> listAppointments=Crud.AppointmentCrud.TableToList(Db.GetTable(command));
			return listAppointments;
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
			//No need to check MiddleTierRole; no call to db.
			//==Jordan Dicts are not allowed. I do want to fix this one, but it has a lot of tentacles and will require test environment.
			//Strategy for refactor is to return a flat list of ApptSearchProviderSchedules, and use linq at the last minute to get a group by date.
			Dictionary<DateTime,List<ApptSearchProviderSchedule>> dictProviderSchedulesByDate=new Dictionary<DateTime,List<ApptSearchProviderSchedule>>();
			List<ApptSearchProviderSchedule> listApptSearchProviderSchedules=new List<ApptSearchProviderSchedule>();
			if(dateScheduleStart.Date>=dateScheduleStop.Date) {
				listApptSearchProviderSchedules=GetProviderScheduleForProvidersAndDate(listProvNums,dateScheduleStart.Date,listSchedules,listAppointments);
				dictProviderSchedulesByDate.Add(dateScheduleStart.Date,listApptSearchProviderSchedules);
				return dictProviderSchedulesByDate;
			}
			//Loop through all the days between the start and stop date and return the ApptSearchProviderSchedule's for all days.
			for(int i=0;i<=(dateScheduleStop.Date-dateScheduleStart.Date).Days;i++) {
				listApptSearchProviderSchedules=GetProviderScheduleForProvidersAndDate(listProvNums,dateScheduleStart.Date.AddDays(i),listSchedules,listAppointments);
				if(dictProviderSchedulesByDate.ContainsKey(dateScheduleStart.Date.AddDays(i))) {//Just in case.
					dictProviderSchedulesByDate[dateScheduleStart.Date.AddDays(i)]=listApptSearchProviderSchedules;
				}
				else {
					dictProviderSchedulesByDate.Add(dateScheduleStart.Date.AddDays(i),listApptSearchProviderSchedules);
				}
			}
			return dictProviderSchedulesByDate;
		}

		///<summary>Uses the input parameters to construct a List&lt;ApptSearchProviderSchedule&gt;.Written to reduce the number of queries to the database.</summary>
		/// <param name="listProvNums">PrimaryKeys to Provider.</param>
		/// <param name="dateForSchedule">The date to construct the schedule for.</param>
		/// <param name="listSchedules">A List of Schedules containing all of the schedules for the given day, or possibly more. 
		/// Intended to be all schedules between search start date and search start date plus 2 years. This is to reduce queries to DB.</param>
		/// <param name="listAppointments">A List of Appointments containing all of the schedules for the given day, or possibly more. 
		/// Intended to be all Appointments between search start date and search start date plus 2 years. This is to reduce queries to DB.</param>
		//js 2/2021, I disagree that 2 years of appointments is efficient, but will research later. Their assumption seems to be that there is not a tighter search range.
		public static List<ApptSearchProviderSchedule> GetProviderScheduleForProvidersAndDate(List<long> listProvNums
			,DateTime dateForSchedule,List<Schedule> listSchedules,List<Appointment> listAppointments) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<ApptSearchProviderSchedule> listApptSearchProviderSchedules=new List<ApptSearchProviderSchedule>();
			List<Def> listDefsBlockouts=Defs.GetDefsForCategory(DefCat.BlockoutTypes,true);
			List<long> listDefNumsBlockoutDoNotSchedule=new List<long>();
			for(int i=0;i<listDefsBlockouts.Count();i++){
				if(listDefsBlockouts[i].ItemValue.Contains(BlockoutType.NoSchedule.GetDescription())) {
					listDefNumsBlockoutDoNotSchedule.Add(listDefsBlockouts[i].DefNum);//do not return results for blockouts set to 'Do Not Schedule'
					continue;
				}
			}
			//Make a shallow copy of the list of schedules passed in so that we can order them in a unique fashion without affecting calling methods.
			List<Schedule> listSchedulesOrdered=listSchedules.OrderByDescending(x => listDefNumsBlockoutDoNotSchedule.Contains(x.BlockoutType))
				.ThenBy(x => x.BlockoutType > 0).ToList();
			for(int i=0;i<listProvNums.Count;i++) {
				listApptSearchProviderSchedules.Add(new ApptSearchProviderSchedule());
				listApptSearchProviderSchedules[i].ProviderNum=listProvNums[i];
				listApptSearchProviderSchedules[i].SchedDate=dateForSchedule;
			}
			for(int s=0;s<listSchedulesOrdered.Count();s++){
				if(listSchedulesOrdered[s].SchedDate.Date!=dateForSchedule) {//ignore schedules for different dates.
					continue;
				}
				if(listProvNums.Contains(listSchedulesOrdered[s].ProvNum)) {//schedule applies to one of the selected providers
					int indexOfProvider = listProvNums.IndexOf(listSchedulesOrdered[s].ProvNum);//cache the provider index
					int scheduleStartBlock = (int)listSchedulesOrdered[s].StartTime.TotalMinutes/5;//cache the start time of the schedule
					int scheduleLengthInBlocks = (int)(listSchedulesOrdered[s].StopTime-listSchedulesOrdered[s].StartTime).TotalMinutes/5;//cache the length of the schedule
					for(int i=0;i<scheduleLengthInBlocks;i++) {
						if(listSchedulesOrdered[s].BlockoutType > 0 && listDefNumsBlockoutDoNotSchedule.Contains(listSchedulesOrdered[s].BlockoutType)) {
							listApptSearchProviderSchedules[indexOfProvider].IsProvAvailable[scheduleStartBlock+i]=false;
							listApptSearchProviderSchedules[indexOfProvider].IsProvScheduled[scheduleStartBlock+i]=false;
						}
						else {
							listApptSearchProviderSchedules[indexOfProvider].IsProvAvailable[scheduleStartBlock+i]=true;//provider may have an appointment here
							listApptSearchProviderSchedules[indexOfProvider].IsProvScheduled[scheduleStartBlock+i]=true;//provider is scheduled today
						}
					}
				}
			}
			int numBlocksInDay=60*24/5;//Number of five minute increments in a day. Matches the length of the IsProvAvailableArray
			for(int a=0;a<listAppointments.Count();a++){
				if(listAppointments[a].AptDateTime.Date!=dateForSchedule) {
					continue;
				}
				if(!listAppointments[a].IsHygiene && listProvNums.Contains(listAppointments[a].ProvNum)) {//Not hygiene Modify provider bar based on ProvNum
					int indexOfProvider = listProvNums.IndexOf(listAppointments[a].ProvNum);
					int appointmentStartBlock = (int)listAppointments[a].AptDateTime.TimeOfDay.TotalMinutes/5;
					for(int i=0;i<listAppointments[a].Pattern.Length;i++) {
						if(listAppointments[a].Pattern[i]=='X') {
							if(appointmentStartBlock+i>=numBlocksInDay) {//if the appointment is scheduled over a day, prevents the search from breaking
								break;
							}
							listApptSearchProviderSchedules[indexOfProvider].IsProvAvailable[appointmentStartBlock+i]=false;
						}
					}
				}
				else if(listAppointments[a].IsHygiene && listProvNums.Contains(listAppointments[a].ProvHyg)) {//Modify provider bar based on ProvHyg
					int indexOfProvider = listProvNums.IndexOf(listAppointments[a].ProvHyg);
					int appointmentStartBlock = (int)listAppointments[a].AptDateTime.TimeOfDay.TotalMinutes/5;
					for(int i=0;i<listAppointments[a].Pattern.Length;i++) {
						if(listAppointments[a].Pattern[i]=='X') {
							if(appointmentStartBlock+i>=numBlocksInDay) {//if the appointment is scheduled over a day, prevents the search from breaking
								break;
							}
							listApptSearchProviderSchedules[indexOfProvider].IsProvAvailable[appointmentStartBlock+i]=false;
						}
					}
				}
			}
			return listApptSearchProviderSchedules;
		}

		///<summary>Gets the ProvNum for the last completed or scheduled appointment for a patient. If none, returns 0.</summary>
		public static long GetProvNumFromLastApptForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT Confirmed FROM appointment WHERE AptNum="+POut.Long(aptNum);
			return PIn.Long(Db.GetScalar(command));
		}
		
		///<summary></summary>
		public static DataSet GetApptEdit(long aptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),aptNum);
			}
			DataSet dataSet=new DataSet();
			dataSet.Tables.Add(GetApptTable(aptNum));
			dataSet.Tables.Add(GetPatTable(dataSet.Tables["Appointment"].Rows[0]["PatNum"].ToString()
				,Appointments.GetOneApt(PIn.Long(dataSet.Tables["Appointment"].Rows[0]["AptNum"].ToString()))));
			dataSet.Tables.Add(GetProcTable(dataSet.Tables["Appointment"].Rows[0]["PatNum"].ToString(),aptNum.ToString(),
				dataSet.Tables["Appointment"].Rows[0]["AptStatus"].ToString(),
				dataSet.Tables["Appointment"].Rows[0]["AptDateTime"].ToString()
				));
			dataSet.Tables.Add(GetCommTable(dataSet.Tables["Appointment"].Rows[0]["PatNum"].ToString(),aptNum));
			bool isPlanned=false;
			if(dataSet.Tables["Appointment"].Rows[0]["AptStatus"].ToString()=="6"){
				isPlanned=true;
			}
			dataSet.Tables.Add(GetMiscTable(aptNum.ToString(),isPlanned));
			dataSet.Tables.Add(GetApptFields(aptNum));
			return dataSet;
		}

		///<summary></summary>
		public static DataSet RefreshOneApt(long aptNum,bool isPlanned,List<long> listOpNums=null,List<long> listProvNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),aptNum,isPlanned,listOpNums,listProvNums);
			} 
			DataSet dataSet=new DataSet();
			dataSet.Tables.Add(GetPeriodApptsTable(DateTime.MinValue,DateTime.MinValue,aptNum,isPlanned,listOpNums,listProvNums));
			return dataSet;
		}

		///<summary>If aptnum is specified, then the dates are ignored in the Appointment table queries; they are still used in the Adjustment table queries.  If getting data for one planned appt, then pass isPlanned=1.  The times of the dateStart and dateEnd are ignored.  This changes which procedures are retrieved.  Any ApptNums within listPinApptNums will get forcefully added to the DataTable.  Since dateStart and dateEnd are used to calcualte adjustment totals, when in 'Day' view mode, dateStart should be set to the beginning of the day and dateEnd set to the end of the day.  Set "allowRunQueryOnNoOps" to false if an empty DataTable should be returned when listOpNums is null or empty.</summary>
		public static DataTable GetPeriodApptsTable(DateTime dateTStart,DateTime dateTEnd,long aptNum,bool isPlanned,List<long> listAptNumsPin=null,
			List<long> listOpNums=null,List<long> listProvNums=null,bool allowRunQueryOnNoOps=true,bool includeVerifyIns=false) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateTStart,dateTEnd,aptNum,isPlanned,listAptNumsPin,listOpNums,listProvNums,allowRunQueryOnNoOps,includeVerifyIns);
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
			table.Columns.Add("hasFamilyBalance");
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
			if(!allowRunQueryOnNoOps && (listOpNums==null || listOpNums.Count==0)) {
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
				+"patient.CreditType patCreditType,appointment.DateTimeAskedToArrive apptDateTimeAskedToArrive,"
				+"patient.Email patEmail,guar.FamFinUrgNote guarFamFinUrgNote,patient.FName patFName,patient.Guarantor patGuarantor,"
				+"patient.HmPhone patHmPhone,patient.ImageFolder patImageFolder,appointment.IsHygiene apptIsHygiene,appointment.IsNewPatient apptIsNewPatient,"
				+"patient.Language patLanguage,patient.LName patLName,patient.MedUrgNote patMedUrgNote,"
				+"patient.MiddleI patMiddleI,appointment.Note apptNote,appointment.Op apptOp,appointment.PatNum apptPatNum,"
				+"appointment.Pattern apptPattern, appointment.PatternSecondary apptPatternSecondary," 
				+"(CASE WHEN patplan.InsSubNum IS NULL THEN 0 ELSE 1 END) hasIns,patient.PreferConfirmMethod patPreferConfirmMethod,"
				+"patient.PreferContactMethod patPreferContactMethod,patient.Preferred patPreferred,"
				+"patient.PreferRecallMethod patPreferRecallMethod,patient.Premed patPremed,"
				+"appointment.ProcDescript apptProcDescript,appointment.ProcsColored apptProcsColored,appointment.ProvBarText,"
				+"appointment.ProvHyg apptProvHyg,appointment.ProvNum apptProvNum,"
				+"patient.State patState,patient.WirelessPhone patWirelessPhone,patient.WkPhone patWkPhone,patient.Zip patZip,"
				+"(CASE WHEN discountplansub.DiscountPlanNum IS NULL THEN 0 ELSE discountplansub.DiscountPlanNum END) discountPlan,"
				+"(CASE WHEN discountplansub.DiscountPlanNum IS NULL THEN 0 ELSE 1 END) hasDiscount, "
				+"IF(guar.BalTotal > 0,'$','') hasFamilyBalance "
				+"FROM appointment "
				+"LEFT JOIN patient ON patient.PatNum=appointment.PatNum ";
			command+="LEFT JOIN patient guar ON guar.PatNum=patient.Guarantor "
				+"LEFT JOIN patplan ON patplan.PatNum=patient.PatNum AND patplan.Ordinal=1 "
				+"LEFT JOIN discountplansub ON discountplansub.PatNum=patient.PatNum ";
			if(aptNum > 0) {//Only get information regarding this one appointment passed in.
				command+="WHERE appointment.AptNum="+POut.Long(aptNum);
			}
			else {//Get all information for the appointments for the date range and any appointments on the pinboard.
				command+="WHERE ((AptDateTime >= "+POut.Date(dateTStart)+" "
					+"AND AptDateTime < "+POut.Date(dateTEnd.AddDays(1))+" "
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
				if(listAptNumsPin!=null && listAptNumsPin.Count>0) {
					command+="OR appointment.AptNum IN ("+String.Join(",",listAptNumsPin)+")";
				}
				command+=")";
			}
			DataTable tableRaw=dcon.GetTable(command);
			//rawProc table was historically used for other purposes.  It is currently only used for production--------------------------
			//rawProcLab table is only used for Canada and goes hand in hand with the rawProc table, also only used for production.
			DataTable tableRawProc;
			DataTable tableRawProcLab=null;
			if(tableRaw.Rows.Count==0){
				tableRawProc=new DataTable();
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					tableRawProcLab=new DataTable();
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
					tableRawProcLab=dcon.GetTable(command);
				}
			}
			List<long> listPatNums=new List<long>();
			List<long> listPlanNums=new List<long>();
			List<long> listGuarantorsWithIns=new List<long>();
			for(int i=0;i<tableRaw.Rows.Count;i++){
				listPatNums.Add(PIn.Long(tableRaw.Rows[i]["apptPatNum"].ToString()));
				listPlanNums.Add(PIn.Long(tableRaw.Rows[i]["InsPlan1"].ToString()));
				listPlanNums.Add(PIn.Long(tableRaw.Rows[i]["InsPlan2"].ToString()));
				if(tableRaw.Rows[i]["hasIns"].ToString()!="0") {
					listGuarantorsWithIns.Add(PIn.Long(tableRaw.Rows[i]["patGuarantor"].ToString()));
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
			//QUERY 5: listInsPlans: insplan (Carriers is cached)=====================================================================================================
			List<InsPlan> listInsPlans=InsPlans.GetPlans(listPlanNums);
			//QUERY 6: listDiscountPlans: discountplan=================================================================================================================
			//DiscountPlan is a small table not linked to patient, but it's still better to only get the ones we need.
			List<long> listDiscountPlanNums=new List<long>();
			for(int i=0;i<tableRaw.Rows.Count;i++){
				if(PIn.Long(tableRaw.Rows[i]["DiscountPlan"].ToString())>0){
					listDiscountPlanNums.Add(PIn.Long(tableRaw.Rows[i]["DiscountPlan"].ToString()));
				}
			}
			List<DiscountPlan> listDiscountPlans=DiscountPlans.GetDiscountPlansByPlanNum(listDiscountPlanNums);
			//QUERY 7: listPatsWithDisease: disease=================================================================================================================
			List<long> listPatNumsWithDisease=Diseases.GetPatientsWithDisease(listPatNums);
			//QUERY 8: listPatsWithAllergy: allergy=================================================================================================================
			List<long> listPatNumsWithAllergy=Allergies.GetPatientsWithAllergy(listPatNums);
			//QUERY 9: listRefAttaches: refattach=================================================================================================================
			List<long> listRefNums=new List<long>();
			List<RefAttach> listRefAttaches=RefAttaches.GetRefAttaches(listPatNums);
			for(int i=0;i<listRefAttaches.Count;i++) {
				if(!listRefNums.Contains(listRefAttaches[i].ReferralNum)) {
					listRefNums.Add(listRefAttaches[i].ReferralNum);
				}
			}
			List<Referral> listReferrals=Referrals.GetReferrals(listRefNums);//cached
			//QUERY 10: listRecallsPastDue: recall=================================================================================================================
			List<Recall> listRecallsPastDue=Recalls.GetPastDueForPats(dateTStart,listPatNums);
			//QUERY 11: listAdjustments: adjustment, appointment union adjustment=====================================================================================
			command=Adjustments.GetQueryAdjustmentsForAppointments(dateTStart,dateTEnd,listOpNums,doGetSum:false);
			if(tableRawProc!=null && tableRawProc.Rows.Count > 0) {
				string procNums=string.Join(",",tableRawProc.AsEnumerable().Select(x => x["ProcNum"].ToString()));
				if(tableRawProcLab!=null && tableRawProcLab.Rows.Count>0) {
					procNums+=","+string.Join(",",tableRawProcLab.AsEnumerable().Select(x => x["ProcNum"].ToString()));
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
				command="SELECT FKey,MAX(DateLastVerified) AS DateLastVerified,VerifyType,patplan.PatNum,NULL HideFromVerifyList,inssub.PlanNum "
					+"FROM insverify "
					+"LEFT JOIN patplan ON patplan.PatPlanNum=insverify.FKey AND insverify.VerifyType="+POut.Enum<VerifyTypes>(VerifyTypes.PatientEnrollment)+" "
					+"LEFT JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum "
					+"WHERE patplan.PatNum IN("+patNums+") "
					+"AND insverify.VerifyType="+POut.Enum<VerifyTypes>(VerifyTypes.PatientEnrollment)+" "
					+"GROUP BY patplan.PatPlanNum "//Grouping to ensure we get latest DateLastVerified if there are multiple rows for one patnum (JobNum:50382)
					+"UNION ALL "
					+"SELECT FKey,MAX(DateLastVerified) AS DateLastVerified,VerifyType,NULL PatNum,insplan.HideFromVerifyList,NULL PlanNum "
					+"FROM insverify "
					+"LEFT JOIN insplan ON insplan.PlanNum=insverify.FKey AND insverify.VerifyType="+POut.Enum<VerifyTypes>(VerifyTypes.InsuranceBenefit)+" "
					+"WHERE insverify.FKey IN("+insPlanNums+") "
					+"AND insverify.VerifyType="+POut.Enum<VerifyTypes>(VerifyTypes.InsuranceBenefit)+" "
					+"GROUP BY insverify.FKey";//Grouping here as well per JobNum:50382
				if(insPlanNums!="") {//if no insPlans, then there can't be any patPlans.
					tableInsVerify=dcon.GetTable(command);
				}
			}
			//Query 13: labcases associated to appointments================================================================================================
			List<LabCase> listLabCases=new List<LabCase>();
			if(aptNum>0 || tableRaw.Rows.Count>0) {
				command="SELECT * FROM labcase WHERE ";
				if(isPlanned) {
					command+="PlannedAptNum!=0 AND PlannedAptNum ";
				} 
				else {
					command+="AptNum!=0 AND AptNum ";
				}
				command+="IN(";//this was far too slow:SELECT a.AptNum FROM appointment a WHERE ";
				if(aptNum==0) {
					for(int i=0;i<tableRaw.Rows.Count;i++){
						if(i>0){
							command+=",";
						}
						command+=tableRaw.Rows[i]["apptAptNum"].ToString();
					}
				}
				else {
					command+=POut.Long(aptNum);
				}
				command+=")";
				listLabCases=Crud.LabCaseCrud.SelectMany(command);
			}
			List<Laboratory> listLaboratories=Laboratories.GetMany(listLabCases.Select(x => x.LaboratoryNum).ToList());
			List<LabCase> listLabCasesAptNum;//This will get used in the for loop below.
			for(int d=0;d<tableRaw.Rows.Count;d++){
				DataRow dataRow=table.NewRow();
				DateTime dateTApt;
				TimeSpan timeSpan;
				int hours;
				int minutes;
				DateTime dateTLab;
				DateTime dateTLabDue;
				DateTime birthdate;
				DateTime timeAskedToArrive;
				decimal productionAmt;
				decimal writeoffPPOAmt;
				decimal insAmt;
				#region Make Row
				dataRow["address"]=Patients.GetAddressFull(tableRaw.Rows[d]["patAddress1"].ToString(),tableRaw.Rows[d]["patAddress2"].ToString(),
				tableRaw.Rows[d]["patCity"].ToString(),tableRaw.Rows[d]["patState"].ToString(),tableRaw.Rows[d]["patZip"].ToString());
				dataRow["addrNote"]="";
				if(tableRaw.Rows[d]["patAddrNote"].ToString()!="") {
					dataRow["addrNote"]=Lans.g("Appointments","AddrNote: ")+tableRaw.Rows[d]["patAddrNote"].ToString();
				}
				dateTApt=PIn.DateT(tableRaw.Rows[d]["apptAptDateTime"].ToString());
				dataRow["AptDateTime"]=dateTApt;
				dataRow["AptDateTimeArrived"]=PIn.DateT(tableRaw.Rows[d]["apptAptDateTimeArrived"].ToString());
				birthdate=PIn.Date(tableRaw.Rows[d]["patBirthdate"].ToString());
				DateTime dateTimeDeceased=PIn.Date(tableRaw.Rows[d]["patDateTimeDeceased"].ToString());
				DateTime dateTimeTo=DateTime.Now;
				if(dateTimeDeceased.Year>1880) {
					dateTimeTo=dateTimeDeceased;
				}
				dataRow["age"]="";
				if(birthdate.AddYears(18)<dateTimeTo) {
					dataRow["age"]=Lans.g("Appointments","Age: ");//only show if older than 18
				}
				if(birthdate.Year>1880) {
					dataRow["age"]+=PatientLogic.DateToAgeString(birthdate,dateTimeTo);
				}
				else {
					dataRow["age"]+="?";
				}
				dataRow["apptModNote"]="";
				if(tableRaw.Rows[d]["patApptModNote"].ToString()!="") {
					dataRow["apptModNote"]=Lans.g("Appointments","ApptModNote: ")+tableRaw.Rows[d]["patApptModNote"].ToString();
				}
				dataRow["aptDate"]=dateTApt.ToShortDateString();
				dataRow["aptDay"]=dateTApt.ToString("dddd");
				timeSpan=TimeSpan.FromMinutes(tableRaw.Rows[d]["apptPattern"].ToString().Length*5);
				hours=timeSpan.Hours;
				minutes=timeSpan.Minutes;
				if(hours==0) {
					dataRow["aptLength"]=minutes.ToString()+Lans.g("Appointments"," Min");
				}
				else if(hours==1) {
					dataRow["aptLength"]=hours.ToString()+Lans.g("Appointments"," Hr, ")
						+minutes.ToString()+Lans.g("Appointments"," Min");
				}
				else {
					dataRow["aptLength"]=hours.ToString()+Lans.g("Appointments"," Hrs, ")
						+minutes.ToString()+Lans.g("Appointments"," Min");
				}
				dataRow["aptTime"]=dateTApt.ToShortTimeString();
				dataRow["AptNum"]=tableRaw.Rows[d]["apptAptNum"].ToString();
				dataRow["AptStatus"]=tableRaw.Rows[d]["apptAptStatus"].ToString();
				dataRow["Assistant"]=tableRaw.Rows[d]["apptAssistant"].ToString();
				dataRow["assistantAbbr"]="";
				if(dataRow["Assistant"].ToString()!="0") {
					dataRow["assistantAbbr"]=Employees.GetAbbr(PIn.Long(tableRaw.Rows[d]["apptAssistant"].ToString()));
				}
				dataRow["AppointmentTypeNum"]=tableRaw.Rows[d]["AppointmentTypeNum"].ToString();
				dataRow["billingType"]=Defs.GetName(DefCat.BillingTypes,PIn.Long(tableRaw.Rows[d]["patBillingType"].ToString()));
				dataRow["Birthdate"]=birthdate.ToShortDateString();
				dataRow["chartNumber"]=tableRaw.Rows[d]["patChartNumber"].ToString();
				dataRow["chartNumAndName"]="";
				if(tableRaw.Rows[d]["apptIsNewPatient"].ToString()=="1") {
					dataRow["chartNumAndName"]="NP-";
				}
				dataRow["chartNumAndName"]+=tableRaw.Rows[d]["patChartNumber"].ToString()+" "
					+PatientLogic.GetNameLF(tableRaw.Rows[d]["patLName"].ToString(),tableRaw.Rows[d]["patFName"].ToString(),
					tableRaw.Rows[d]["patPreferred"].ToString(),tableRaw.Rows[d]["patMiddleI"].ToString());
				dataRow["ClinicNum"]=tableRaw.Rows[d]["ClinicNum"].ToString();
				dataRow["ColorOverride"]=tableRaw.Rows[d]["apptColorOverride"].ToString();
				dataRow["confirmed"]=Defs.GetName(DefCat.ApptConfirmed,PIn.Long(tableRaw.Rows[d]["apptConfirmed"].ToString()));
				dataRow["Confirmed"]=tableRaw.Rows[d]["apptConfirmed"].ToString();
				dataRow["contactMethods"]="";
				if(tableRaw.Rows[d]["patPreferConfirmMethod"].ToString()!="0") {
					dataRow["contactMethods"]+=Lans.g("Appointments","Confirm Method: ")
						+((ContactMethod)PIn.Long(tableRaw.Rows[d]["patPreferConfirmMethod"].ToString())).ToString();
				}
				if(tableRaw.Rows[d]["patPreferContactMethod"].ToString()!="0") {
					if(dataRow["contactMethods"].ToString()!="") {
						dataRow["contactMethods"]+="\r\n";
					}
					dataRow["contactMethods"]+=Lans.g("Appointments","Contact Method: ")
						+((ContactMethod)PIn.Long(tableRaw.Rows[d]["patPreferContactMethod"].ToString())).ToString();
				}
				if(tableRaw.Rows[d]["patPreferRecallMethod"].ToString()!="0") {
					if(dataRow["contactMethods"].ToString()!="") {
						dataRow["contactMethods"]+="\r\n";
					}
					dataRow["contactMethods"]+=Lans.g("Appointments","Recall Method: ")
						+((ContactMethod)PIn.Long(tableRaw.Rows[d]["patPreferRecallMethod"].ToString())).ToString();
				}
				bool hasInsToSend=false;
				if(tableRawInsProc!=null) {
					//figure out if pt's family has ins claims that need to be created
					for(int j = 0;j<tableRawInsProc.Rows.Count;j++) {
						if(tableRaw.Rows[d]["hasIns"].ToString()!="0") {
							if(tableRaw.Rows[d]["patGuarantor"].ToString()==tableRawInsProc.Rows[j]["Guarantor"].ToString()
								||tableRaw.Rows[d]["patGuarantor"].ToString()==tableRawInsProc.Rows[j]["PatNum"].ToString()) {
								hasInsToSend=true;
							}
						}
					}
				}
				dataRow["CreditType"]=tableRaw.Rows[d]["patCreditType"].ToString();
				dataRow["discountPlan"]="";
				long discountPlanNum=PIn.Long(tableRaw.Rows[d]["DiscountPlan"].ToString());
				DiscountPlan discountPlan=null;
				if(discountPlanNum>0) {
					discountPlan=listDiscountPlans.Find(x => x.DiscountPlanNum==discountPlanNum);
					if(discountPlan!=null){
						dataRow["discountPlan"]+=Lans.g("Appointments","DiscountPlan")+": "+discountPlan.Description;
					}
				}
				dataRow["Email"]=tableRaw.Rows[d]["patEmail"].ToString();
				dataRow["famFinUrgNote"]="";
				if(tableRaw.Rows[d]["guarFamFinUrgNote"].ToString()!="") {
					dataRow["famFinUrgNote"]=Lans.g("Appointments","FamFinUrgNote: ")+tableRaw.Rows[d]["guarFamFinUrgNote"].ToString();
				}
				dataRow["guardians"]="";
				GuardianRelationship guardianRelationship;
				for(int g = 0;g<tableRawGuardians.Rows.Count;g++) {
					if(tableRaw.Rows[d]["apptPatNum"].ToString()==tableRawGuardians.Rows[g]["PatNumChild"].ToString()) {
						if(dataRow["guardians"].ToString()!="") {
							dataRow["guardians"]+=",";
						}
						guardianRelationship=(GuardianRelationship)PIn.Int(tableRawGuardians.Rows[g]["Relationship"].ToString());
						dataRow["guardians"]+=Patients.GetNameFirstOrPreferred(tableRawGuardians.Rows[g]["FName"].ToString(),tableRawGuardians.Rows[g]["Preferred"].ToString())
							+Guardians.GetGuardianRelationshipStr(guardianRelationship);
					}
				}
				dataRow["hasDiscount[D]"]="";
				if(tableRaw.Rows[d]["hasDiscount"].ToString()!="0") {
					dataRow["hasDiscount[D]"]+="D";
				}
				dataRow["hasFamilyBalance"]=tableRaw.Rows[d]["hasFamilyBalance"].ToString();
				dataRow["hasIns[I]"]="";
				if(tableRaw.Rows[d]["hasIns"].ToString()!="0") {
					dataRow["hasIns[I]"]+="I";
				}
				dataRow["hmPhone"]=Lans.g("Appointments","Hm: ")+tableRaw.Rows[d]["patHmPhone"].ToString();
				dataRow["ImageFolder"]=tableRaw.Rows[d]["patImageFolder"].ToString();
				dataRow["insurance"]="";
				long planNum1=PIn.Long(tableRaw.Rows[d]["InsPlan1"].ToString());
				long planNum2=PIn.Long(tableRaw.Rows[d]["InsPlan2"].ToString());
				InsPlan insPlan1=listInsPlans.Find(x=>x.PlanNum==planNum1);
				if(planNum1>0 && insPlan1!=null) {
					Carrier carrier=Carriers.GetCarrier(insPlan1.CarrierNum);
					dataRow["insurance"]+=Lans.g("Appointments","Ins1")+": "+carrier.CarrierName;
					dataRow["insColor1"]=carrier.ApptTextBackColor.ToArgb();
				}
				InsPlan insPlan2=listInsPlans.Find(x=>x.PlanNum==planNum2);
				if(planNum2>0 && insPlan2!=null) {
					Carrier carrier=Carriers.GetCarrier(insPlan2.CarrierNum);
					if(dataRow["insurance"].ToString()!="") {
						dataRow["insurance"]+="\r\n";
					}
					dataRow["insurance"]+=Lans.g("Appointments","Ins2")+": "+carrier.CarrierName;
					dataRow["insColor2"]=carrier.ApptTextBackColor.ToArgb();
				}
				if(tableRaw.Rows[d]["hasIns"].ToString()!="0"&&dataRow["insurance"].ToString()=="") {
					dataRow["insurance"]=Lans.g("Appointments","Insured");
				}
				dataRow["insToSend[!]"]="";
				if(hasInsToSend) {
					dataRow["insToSend[!]"]="!";
				}
				dataRow["Priority"]=tableRaw.Rows[d]["Priority"].ToString();
				dataRow["IsHygiene"]=tableRaw.Rows[d]["apptIsHygiene"].ToString();
				dataRow["lab"]="";
				long apptAptNum=PIn.Long(tableRaw.Rows[d]["apptAptNum"].ToString());
				if(isPlanned) {
					listLabCasesAptNum=listLabCases.FindAll(x => x.PlannedAptNum==apptAptNum);
				}
				else {
					listLabCasesAptNum=listLabCases.FindAll(x => x.AptNum==apptAptNum);
				}
				if(!listLabCasesAptNum.IsNullOrEmpty()) {
					string labText="";
					foreach(LabCase labCaseCur in listLabCasesAptNum) {
						Laboratory laboratoryCur=listLaboratories.Find(x => x.LaboratoryNum==labCaseCur.LaboratoryNum);
						if(!string.IsNullOrEmpty(labText)) {
							labText+=Environment.NewLine;
						}
						if(laboratoryCur!=null && listLabCasesAptNum.Count>1) {
							//Only add the laboratory description if more than 1 labcase per appointment.
							labText+=laboratoryCur.Description+": ";
						}
						dateTLab=labCaseCur.DateTimeChecked;
						if(dateTLab.Year>1880) {
							labText+=Lans.g("Appointments","Lab Quality Checked");
						}
						else {
							dateTLab=labCaseCur.DateTimeRecd;
							if(dateTLab.Year>1880) {
								labText+=Lans.g("Appointments","Lab Received");
							}
							else {
								dateTLab=labCaseCur.DateTimeSent;
								if(dateTLab.Year>1880) {
									labText+=Lans.g("Appointments","Lab Sent");//sent but not received
								}
								else {
									labText+=Lans.g("Appointments","Lab Not Sent");
								}
								dateTLabDue=labCaseCur.DateTimeDue;
								if(dateTLabDue.Year>1880) {
									labText+=", "+Lans.g("Appointments","Due: ")//+dateDue.ToString("ddd")+" "
										+dateTLabDue.ToShortDateString();//+" "+dateDue.ToShortTimeString();
								}
							}
						}
					}
					dataRow["lab"]=labText;
				}
				CultureInfo cultureInfo=CodeBase.MiscUtils.GetCultureFromThreeLetter(tableRaw.Rows[d]["patLanguage"].ToString());
				if(cultureInfo==null) {//custom language
					dataRow["language"]=tableRaw.Rows[d]["patLanguage"].ToString();
				}
				else {
					dataRow["language"]=cultureInfo.DisplayName;
				}
				dataRow["medOrPremed[+]"]="";
				long apptPatNum=PIn.Long(tableRaw.Rows[d]["apptPatNum"].ToString());
				if(tableRaw.Rows[d]["patMedUrgNote"].ToString()!=""||tableRaw.Rows[d]["patPremed"].ToString()=="1"
					||listPatNumsWithDisease.Contains(apptPatNum)||listPatNumsWithAllergy.Contains(apptPatNum)) {
					dataRow["medOrPremed[+]"]="+";
				}
				dataRow["MedUrgNote"]=tableRaw.Rows[d]["patMedUrgNote"].ToString();
				dataRow["Note"]=tableRaw.Rows[d]["apptNote"].ToString();
				dataRow["Op"]=tableRaw.Rows[d]["apptOp"].ToString();
				if(tableRaw.Rows[d]["apptIsNewPatient"].ToString()=="1") {
					dataRow["patientName"]="NP-";
				}
				dataRow["patientName"]+=PatientLogic.GetNameLF(tableRaw.Rows[d]["patLName"].ToString(),tableRaw.Rows[d]["patFName"].ToString(),
					tableRaw.Rows[d]["patPreferred"].ToString(),tableRaw.Rows[d]["patMiddleI"].ToString());
				if(tableRaw.Rows[d]["apptIsNewPatient"].ToString()=="1") {
					dataRow["patientNameF"]="NP-";
				}
				dataRow["patientNameF"]+=tableRaw.Rows[d]["patFName"].ToString();
				dataRow["patientNamePref"]+=tableRaw.Rows[d]["patPreferred"].ToString();
				dataRow["PatNum"]=tableRaw.Rows[d]["apptPatNum"].ToString();
				dataRow["patNum"]="PatNum: "+tableRaw.Rows[d]["apptPatNum"].ToString();
				dataRow["GuarNum"]=tableRaw.Rows[d]["patGuarantor"].ToString();
				dataRow["patNumAndName"]="";
				if(tableRaw.Rows[d]["apptIsNewPatient"].ToString()=="1") {
					dataRow["patNumAndName"]="NP-";
				}
				dataRow["patNumAndName"]+=tableRaw.Rows[d]["apptPatNum"].ToString()+" "
					+PatientLogic.GetNameLF(tableRaw.Rows[d]["patLName"].ToString(),tableRaw.Rows[d]["patFName"].ToString(),
					tableRaw.Rows[d]["patPreferred"].ToString(),tableRaw.Rows[d]["patMiddleI"].ToString());
				dataRow["Pattern"]=tableRaw.Rows[d]["apptPattern"].ToString();
				dataRow["PatternSecondary"]=tableRaw.Rows[d]["apptPatternSecondary"].ToString();
				dataRow["preMedFlag"]="";
				if(tableRaw.Rows[d]["patPremed"].ToString()=="1") {
					dataRow["preMedFlag"]=Lans.g("Appointments","Premedicate");
				}
				dataRow["procs"]=tableRaw.Rows[d]["apptProcDescript"].ToString();
				dataRow["procsColored"]+=tableRaw.Rows[d]["apptProcsColored"].ToString();
				productionAmt=0;
				writeoffPPOAmt=0;
				insAmt=0;
				decimal adjAmtForAppt=0;
				decimal discountPlanAmt=0;
				decimal procDiscountAmt=0;
				if(tableRawProc!=null) {
					for(int p = 0;p<tableRawProc.Rows.Count;p++) {
						ProcStat procStat=PIn.Enum<ProcStat>(PIn.Int(tableRawProc.Rows[p]["ProcStatus"].ToString()));
						if(isPlanned && tableRaw.Rows[d]["apptAptNum"].ToString()!=tableRawProc.Rows[p]["PlannedAptNum"].ToString()) {
							continue;
						}
						else if(!isPlanned && tableRaw.Rows[d]["apptAptNum"].ToString()!=tableRawProc.Rows[p]["AptNum"].ToString()) {
							continue;
						}
						//We only want to include C, TP, and TPi procedures in the production calculation.
						if(!procStat.In(ProcStat.C,ProcStat.TP,ProcStat.TPi)) {
							continue;
						}
						decimal procFee=PIn.Decimal(tableRawProc.Rows[p]["ProcFee"].ToString());
						productionAmt+=procFee;
						productionAmt-=PIn.Decimal(tableRawProc.Rows[p]["writeoffCap"].ToString());
						//WriteOffEst -1 and WriteOffEstOverride -1 already excluded
						//production-=
						writeoffPPOAmt+=PIn.Decimal(tableRawProc.Rows[p]["writeoffPPO"].ToString());//frequently zero
						insAmt+=PIn.Decimal(tableRawProc.Rows[p]["insAmt"].ToString());
						adjAmtForAppt+=listAdjustments.Where(x => x.ProcNum==PIn.Long(tableRawProc.Rows[p]["ProcNum"].ToString())).Sum(x => (decimal)x.AdjAmt);
						if(tableRawProcLab!=null) { //Will be null if not Canada.
							for(int a = 0;a<tableRawProcLab.Rows.Count;a++) {
								if(tableRawProcLab.Rows[a]["ProcNumLab"].ToString()==tableRawProc.Rows[p]["ProcNum"].ToString()) {
									productionAmt+=PIn.Decimal(tableRawProcLab.Rows[a]["ProcFee"].ToString());
									productionAmt-=PIn.Decimal(tableRawProcLab.Rows[a]["writeoffCap"].ToString());
									writeoffPPOAmt+=PIn.Decimal(tableRawProcLab.Rows[a]["writeoffPPO"].ToString());//frequently zero
									insAmt+=PIn.Decimal(tableRawProc.Rows[p]["insAmt"].ToString());
									adjAmtForAppt+=listAdjustments.Where(x => x.ProcNum==PIn.Long(tableRawProcLab.Rows[a]["ProcNum"].ToString())).Sum(x => (decimal)x.AdjAmt);
								}
							}
						}
						if(procStat!=ProcStat.C) {
							procDiscountAmt+=PIn.Decimal(tableRawProc.Rows[p]["Discount"].ToString());//procedurelog.Discount
							decimal procDiscountPlanAmount=PIn.Decimal(tableRawProc.Rows[p]["DiscountPlanAmt"].ToString());//procedurelog.DiscountPlanAmt
							if(discountPlan!=null && CompareDecimal.IsGreaterThanOrEqualToZero(procDiscountPlanAmount)) {
								discountPlanAmt+=procDiscountPlanAmount;
							}
						}
					}
				}
				dataRow["prophy/PerioPastDue[P]"]=(Recalls.IsPatientPastDue(PIn.Long(tableRaw.Rows[d]["apptPatNum"].ToString()),dateTApt,true,listRecallsPastDue)? "P":"");
				dataRow["production"]=productionAmt.ToString("c");//PIn.Double(tableRaw.Rows[r]["Production"].ToString()).ToString("c");
				dataRow["productionVal"]=productionAmt.ToString();//tableRaw.Rows[r]["Production"].ToString();
				decimal netProductionAmt=(productionAmt-writeoffPPOAmt-discountPlanAmt);
				if(PrefC.GetBool(PrefName.ApptModuleAdjustmentsInProd)) {
					netProductionAmt+=adjAmtForAppt-procDiscountAmt;
				}
				dataRow["netProduction"]=netProductionAmt.ToString("c");
				dataRow["netProductionVal"]=netProductionAmt.ToString();
				decimal feePatientPortion=Math.Max(productionAmt-writeoffPPOAmt+adjAmtForAppt-insAmt-discountPlanAmt-procDiscountAmt,0);
				dataRow["estPatientPortion"]=feePatientPortion.ToString("c");
				dataRow["estPatientPortionRaw"]=feePatientPortion;
				dataRow["adjustmentTotal"]=adjustmentAmt.ToString();
				dataRow["ProvBarText"]=tableRaw.Rows[d]["ProvBarText"].ToString();
				long apptProvNum=PIn.Long(tableRaw.Rows[d]["apptProvNum"].ToString());
				long apptProvHyg=PIn.Long(tableRaw.Rows[d]["apptProvHyg"].ToString());
				if(tableRaw.Rows[d]["apptIsHygiene"].ToString()=="1") {
					dataRow["provider"]=Providers.GetAbbr(apptProvHyg);
					if(apptProvNum!=0) {
						dataRow["provider"]+=" ("+Providers.GetAbbr(apptProvNum)+")";
					}
				}
				else {
					dataRow["provider"]=Providers.GetAbbr(apptProvNum);
					if(apptProvHyg!=0) {
						dataRow["provider"]+=" ("+Providers.GetAbbr(apptProvHyg)+")";
					}
				}
				dataRow["ProvNum"]=tableRaw.Rows[d]["apptProvNum"].ToString();
				dataRow["ProvHyg"]=tableRaw.Rows[d]["apptProvHyg"].ToString();
				dataRow["recallPastDue[R]"]=(Recalls.IsPatientPastDue(PIn.Long(tableRaw.Rows[d]["apptPatNum"].ToString()),dateTApt,false,listRecallsPastDue) ? "R" : "");
				string referralFrom="";
				string referralFromWithPhone="";
				string referralTo="";
				string referralToWithPhone="";
				List<RefAttach> listRefAttachesFrom=listRefAttaches.FindAll(x=>x.RefType==ReferralType.RefFrom && x.PatNum==apptPatNum);
				if(listRefAttachesFrom.Count>0) {
					List<long> listReferralNums=listRefAttachesFrom.Select(x=>x.ReferralNum).ToList();
					List<Referral> listReferralsFrom=listReferrals.FindAll(x=>listReferralNums.Contains(x.ReferralNum));
					referralFrom=Lans.g("Appointment",$"Referred From:")+"\r\n"
						+string.Join("\r\n",listReferralsFrom.Select(x=>x.LName+", "+x.FName));
					referralFromWithPhone=Lans.g("Appointment",$"Referred From With Phone:")+"\r\n";
					for(int f=0;f<listReferralsFrom.Count;f++) {
						if(f>0){
							referralFromWithPhone+="\r\n";
						}
						referralFromWithPhone+=listReferralsFrom[f].LName+", "+listReferralsFrom[f].FName+" ";
						string phoneNum=TelephoneNumbers.AutoFormat(listReferralsFrom[f].Telephone);
						if(phoneNum=="") {
							phoneNum=Lans.g("Appointments","(No Phone Number)");
						}
						referralFromWithPhone+=phoneNum;
					}
				}
				List<RefAttach> listRefAttachesTo=listRefAttaches.FindAll(x=>x.RefType==ReferralType.RefTo && x.PatNum==apptPatNum);
				if(listRefAttachesTo.Count>0) {
					List<long> listReferralNums=listRefAttachesTo.Select(x=>x.ReferralNum).ToList();
					List<Referral> listReferralsTo=listReferrals.FindAll(x=>listReferralNums.Contains(x.ReferralNum));
					referralTo=Lans.g("Appointment",$"Referred To:")+"\r\n"
						+string.Join("\r\n",listReferralsTo.Select(x=>x.LName+", "+x.FName));
					referralToWithPhone=Lans.g("Appointment",$"Referred To With Phone:")+"\r\n";
					for(int t = 0;t<listReferralsTo.Count;t++) {
						if(t>0) {
							referralToWithPhone+="\r\n";
						}
						referralToWithPhone+=listReferralsTo[t].LName+", "+listReferralsTo[t].FName+" ";
						string phoneNum = TelephoneNumbers.AutoFormat(listReferralsTo[t].Telephone);
						if(phoneNum=="") {
							phoneNum=Lans.g("Appointments","(No Phone Number)");
						}
						referralToWithPhone+=phoneNum;
					}
				}
				dataRow["referralFrom"]=referralFrom;
				dataRow["referralFromWithPhone"]=referralFromWithPhone;
				dataRow["referralTo"]=referralTo;
				dataRow["referralToWithPhone"]=referralToWithPhone;
				dataRow["timeAskedToArrive"]="";
				timeAskedToArrive=PIn.DateT(tableRaw.Rows[d]["apptDateTimeAskedToArrive"].ToString());
				if(timeAskedToArrive.Year>1880) {
					dataRow["timeAskedToArrive"]=timeAskedToArrive.ToString("H:mm");
				}
				if(includeVerifyIns){
					DateTime dateTimeLastPlanBenefitsStandard=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDays));
					DateTime dateTimeLastPatEligibilityStandard=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDays));
					DateTime dateTimeLastPlanBenefitsMedicaid=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDaysMedicaid));
					DateTime dateTimeLastPatEligibilityMedicaid=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDaysMedicaid));
					bool excludePatVerify=PrefC.GetBool(PrefName.InsVerifyExcludePatVerify);
					List<string> listInsVerifyMedicaidFilingCodes=PrefC.GetString(PrefName.InsVerifyMedicaidFilingCodes).Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
					//Convert InsVerifyMedicaidFilingCodes pref into a list of longs
					List<long> listInsVerifyMedicaidFilingCodeNums=listInsVerifyMedicaidFilingCodes.Select(x => PIn.Long(x,hasExceptions:false)).ToList();
					if(tableInsVerify!=null && tableInsVerify.Rows.Count!=0) {
						for(int v=0; v<tableInsVerify.Rows.Count; v++) {
							//Here we're dealing with InsPlans, so the PlanNum is the FKey.
							long insVerifyRowPlanNum=PIn.Long(tableInsVerify.Rows[v]["FKey"].ToString());
							InsPlan insPlan=listInsPlans.Find(x=>x.PlanNum==insVerifyRowPlanNum);
							if(PIn.Long(tableRaw.Rows[d]["InsPlan1"].ToString())==insVerifyRowPlanNum) {
								if(VerifyTypes.InsuranceBenefit!=PIn.Enum<VerifyTypes>(tableInsVerify.Rows[v]["VerifyType"].ToString())) {
									continue;
								}
								if(PIn.Bool(tableInsVerify.Rows[v]["HideFromVerifyList"].ToString())) {
									continue;//Specifically marked as don't verify, so skip it
								}
								DateTime dateLastPlanBenefits=dateTimeLastPlanBenefitsStandard;
								if(insPlan!=null && listInsVerifyMedicaidFilingCodeNums.Contains(insPlan.FilingCode)){ //If this filing code is for Medicaid, use Medicaid timing.
									dateLastPlanBenefits=dateTimeLastPlanBenefitsMedicaid;
								}
								if(PIn.Date(tableInsVerify.Rows[v]["DateLastVerified"].ToString())<dateLastPlanBenefits) {
									dataRow["verifyIns[V]"]="V";
									break;
								}
							}
							if(PIn.Long(tableRaw.Rows[d]["InsPlan2"].ToString())==insVerifyRowPlanNum) {
								if(VerifyTypes.InsuranceBenefit!=PIn.Enum<VerifyTypes>(tableInsVerify.Rows[v]["VerifyType"].ToString())) {
									continue;
								}
								if(PIn.Bool(tableInsVerify.Rows[v]["HideFromVerifyList"].ToString())) {
									continue;
								}
								DateTime dateLastPlanBenefits=dateTimeLastPlanBenefitsStandard;
								if(insPlan!=null && listInsVerifyMedicaidFilingCodeNums.Contains(insPlan.FilingCode)){ //If this filing code is for Medicaid, use Medicaid timing.
									dateLastPlanBenefits=dateTimeLastPlanBenefitsMedicaid;
								}
								if(PIn.Date(tableInsVerify.Rows[v]["DateLastVerified"].ToString())<dateLastPlanBenefits) {
									dataRow["verifyIns[V]"]="V";
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
								bool isExcluded=false;
								//On a PatPlan row, we do not have any info about the plan except the PlanNum.
								//That info is guaranteed to be on another row. If this assumption is wrong, the loop is harmless.
								for(int i=0; i<tableInsVerify.Rows.Count;i++) {
									if(PIn.Long(tableInsVerify.Rows[i]["FKey"].ToString())==PIn.Long(tableInsVerify.Rows[v]["PlanNum"].ToString())) {
										if(PIn.Bool(tableInsVerify.Rows[i]["HideFromVerifyList"].ToString())) {
											isExcluded=true;
											break;
										}
									}
								}
								if(isExcluded) {
									continue; //Skip any patplan that is a part of an insplan that is marked as "Don't Verify"
								}
							}
							//At this point, we're dealing with a PatPlan, so the FKey is the PatPlanNum. We need the PlanNum instead.
							insPlan=listInsPlans.Find(x=>x.PlanNum==PIn.Long(tableInsVerify.Rows[v]["PlanNum"].ToString()));
							DateTime dateLastPatEligibility=dateTimeLastPatEligibilityStandard;
							if(insPlan!=null && listInsVerifyMedicaidFilingCodeNums.Contains(insPlan.FilingCode)){ //If this filing code is for Medicaid, use Medicaid timing.
								dateLastPatEligibility=dateTimeLastPatEligibilityMedicaid;
							}
							if(PIn.Date(tableInsVerify.Rows[v]["DateLastVerified"].ToString())<dateLastPatEligibility) {
								dataRow["verifyIns[V]"]="V";
								break;
							}
						}
					}
				}
				dataRow["wirelessPhone"]=Lans.g("Appointments","Cell: ")+tableRaw.Rows[d]["patWirelessPhone"].ToString();
				dataRow["wkPhone"]=Lans.g("Appointments","Wk: ")+tableRaw.Rows[d]["patWkPhone"].ToString();
				dataRow["writeoffPPO"]=writeoffPPOAmt.ToString();
				#endregion Make Row
				table.Rows.Add(dataRow);
			}
			return table;
		}


		///<summary>Gets a mini version of the PeriodApptsTable for all appointments on the passed in day for the given provider. This is specifically
		///used by WebSched so that we don't have to get the full PeriodApptsTable when checking for double booked appointments.</summary>
		public static DataTable GetPeriodApptsTableMini(DateTime dateTimeAppointmentStart,long provNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			//No need to check MiddleTierRole; no call to db.
			List<long> listAptNums=new List<long>();
			for(int i=0;i<tableAppts.Rows.Count;i++) {
				listAptNums.Add(PIn.Long(tableAppts.Rows[i]["AptNum"].ToString()));
			}
			return GetApptFieldsByApptNums(listAptNums);
		}

		/// <summary>Returns a DataTable with all the ApptFields for the AptNums passed in.</summary>
		public static DataTable GetApptFieldsByApptNums(List<long> listAptNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listAptNums);
			}
			string command="SELECT AptNum,FieldName,FieldValue "
				+"FROM apptfield "
				+"WHERE AptNum IN (";
			if(listAptNums.Count==0) {
				command+="0";
			}
			else for(int i=0;i<listAptNums.Count;i++) {
				if(i>0) {
					command+=",";
				}
				command+=POut.Long(listAptNums[i]);
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
				+"ORDER BY apptfielddef.ItemOrder";
			DataConnection dcon=new DataConnection();
			DataTable table= dcon.GetTable(command);
			table.TableName="ApptFields";
			return table;
		}

		///<summary>Used to get the waiting room data table. Pass in the client date time to get the correct waiting time. The date time is also passed into middle tier so that it uses the correct client time.</summary>
		public static DataTable GetPeriodWaitingRoomTable(DateTime dateTime) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateTime);
			}
			//DateTime dateStart=PIn.PDate(strDateStart);
			//DateTime dateEnd=PIn.PDate(strDateEnd);
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("WaitingRoom");
			DataRow dataRow;
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
			DataTable tableRaw=dcon.GetTable(command);
			TimeSpan timeSpanArrived;
			//DateTime timeSeated;
			DateTime dateTimeWait;
			Patient patient;
			DateTime dateTimeNow;
			//int minutes;
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				dataRow=table.NewRow();
				patient=new Patient();
				patient.LName=tableRaw.Rows[i]["LName"].ToString();
				patient.FName=tableRaw.Rows[i]["FName"].ToString();
				patient.Preferred=tableRaw.Rows[i]["Preferred"].ToString();
				dataRow["FName"]=tableRaw.Rows[i]["FName"];
				dataRow["LName"]=tableRaw.Rows[i]["LName"];
				dataRow["patName"]=patient.GetNameLF();
				dateTimeNow=PIn.DateT(tableRaw.Rows[i]["dateTimeNow"].ToString());
				timeSpanArrived=(PIn.DateT(tableRaw.Rows[i]["DateTimeArrived"].ToString())).TimeOfDay;
				dateTimeWait=dateTimeNow-timeSpanArrived;
				dataRow["waitTime"]=dateTimeWait.ToString("H:mm:ss");
				//minutes=waitTime.Minutes;
				//if(waitTime.Hours>0){
				//	row["waitTime"]+=waitTime.Hours.ToString()+"h ";
					//minutes-=60*waitTime.Hours;
				//}
				//row["waitTime"]+=waitTime.Minutes.ToString()+"m";
				dataRow["OpNum"]=tableRaw.Rows[i]["Op"].ToString();
				table.Rows.Add(dataRow);
			}
			return table;
		}

		public static DataTable GetApptTable(long aptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM appointment WHERE AptNum="+aptNum.ToString();
			DataTable table=Db.GetTable(command);
			table.TableName="Appointment";
			return table;
		}

		public static DataTable GetPatTable(string patNum,Appointment appt) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,appt);
			}
			DataTable table=new DataTable("Patient");
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("field");
			table.Columns.Add("value");
			string command="SELECT * FROM patient WHERE PatNum="+patNum;
			DataTable tablePatRaw=Db.GetTable(command);
			DataRow dataRow;
			//Patient Name--------------------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Name");
			dataRow["value"]=PatientLogic.GetNameLF(tablePatRaw.Rows[0]["LName"].ToString(),tablePatRaw.Rows[0]["FName"].ToString(),
				tablePatRaw.Rows[0]["Preferred"].ToString(),tablePatRaw.Rows[0]["MiddleI"].ToString());
			table.Rows.Add(dataRow);
			//Patient First Name--------------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","First Name");
			dataRow["value"]=tablePatRaw.Rows[0]["FName"];
			table.Rows.Add(dataRow);
			//Patient Last name---------------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Last Name");
			dataRow["value"]=tablePatRaw.Rows[0]["LName"];
			table.Rows.Add(dataRow);
			//Patient middle initial----------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Middle Initial");
			dataRow["value"]=tablePatRaw.Rows[0]["MiddleI"];
			table.Rows.Add(dataRow);
			//Patient birthdate----------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Birthdate");
			dataRow["value"]=PIn.Date(tablePatRaw.Rows[0]["Birthdate"].ToString()).ToShortDateString();
			table.Rows.Add(dataRow);
			//Patient home phone--------------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Home Phone");
			dataRow["value"]=tablePatRaw.Rows[0]["HmPhone"];
			table.Rows.Add(dataRow);
			//Patient work phone--------------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Work Phone");
			dataRow["value"]=tablePatRaw.Rows[0]["WkPhone"];
			table.Rows.Add(dataRow);
			//Patient wireless phone----------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Wireless Phone");
			dataRow["value"]=tablePatRaw.Rows[0]["WirelessPhone"];
			table.Rows.Add(dataRow);
			//Patient credit type-------------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Credit Type");
			dataRow["value"]=tablePatRaw.Rows[0]["CreditType"];
			table.Rows.Add(dataRow);
			//Patient billing type------------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Billing Type");
			dataRow["value"]=Defs.GetName(DefCat.BillingTypes,PIn.Long(tablePatRaw.Rows[0]["BillingType"].ToString()));
			table.Rows.Add(dataRow);
			//Patient total balance-----------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Total Balance");
			double totalBalance=PIn.Double(tablePatRaw.Rows[0]["EstBalance"].ToString());
			dataRow["value"]=totalBalance.ToString("F");
			table.Rows.Add(dataRow);
			//Patient address and phone notes-------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Address and Phone Notes");
			dataRow["value"]=tablePatRaw.Rows[0]["AddrNote"];
			table.Rows.Add(dataRow);
			//Patient family balance----------------------------------------------------------------
			command="SELECT BalTotal,InsEst FROM patient WHERE PatNum="+POut.String(tablePatRaw.Rows[0]["Guarantor"].ToString())+"";
			DataTable familyBalance=Db.GetTable(command);
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Family Balance");
			double balance=PIn.Double(familyBalance.Rows[0]["BalTotal"].ToString())
				-PIn.Double(familyBalance.Rows[0]["InsEst"].ToString());
			dataRow["value"]=balance.ToString("F");
			table.Rows.Add(dataRow);
			//Site----------------------------------------------------------------------------------
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				dataRow=table.NewRow();
				dataRow["field"]=Lans.g("FormApptEdit","Site");
				dataRow["value"]=Sites.GetDescription(PIn.Long(tablePatRaw.Rows[0]["SiteNum"].ToString()));
				table.Rows.Add(dataRow);
			}
			//Estimated Patient Portion-------------------------------------------------------------
			dataRow=table.NewRow();
			dataRow["field"]=Lans.g("FormApptEdit","Est. Patient Portion");
			dataRow["value"]=GetEstPatientPortion(appt).ToString("F");
			table.Rows.Add(dataRow);
			return table;
		}
		
		///<summary>Returns the estimated patient portion for the procedures attached to this appointment.</summary>
		public static decimal GetEstPatientPortion(Appointment appointment) {
			if(appointment.AptNum==0) {//Appt hasn't been inserted into the database yet.
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<decimal>(MethodBase.GetCurrentMethod(),appointment);
			}
			DateTime dateStart=appointment.AptDateTime.Date;//Use the entire day.
			DateTime dateEnd=appointment.AptDateTime.Date.AddDays(1).AddMilliseconds(-1);//Use the entire day.
			DataTable table=GetPeriodApptsTable(dateStart,dateEnd,appointment.AptNum,appointment.AptStatus==ApptStatus.Planned);
			if(table.Rows.Count==0) {
				return 0;
			}
			return PIn.Decimal(table.Rows[0]["estPatientPortionRaw"].ToString());
		}

		public static DataTable GetProcTable(string strPatNum,string strAptNum,string strApptStatus,string strAptDateTime) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),strPatNum,strAptNum,strApptStatus,strAptDateTime);
			}
			DataTable table=new DataTable("Procedure");
			DataRow dataRow;
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
			List<DataRow> listDataRows=new List<DataRow>();
			string command="SELECT AbbrDesc,procedurecode.ProcCode,AptNum,LaymanTerm,"
				+"PlannedAptNum,Priority,ProcFee,ProcNum,ProcStatus, "
				+"procedurecode.Descript,procedurelog.CodeNum,ProcDate,procedurelog.ProvNum,Surf,ToothNum,ToothRange,TreatArea "
				+"FROM procedurelog LEFT JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				+"WHERE PatNum="+strPatNum//sort later
			//1. All TP procs
				+" AND (ProcStatus=1 OR ";//tp
			//2. All attached procs
				//+" AND ";
			if(strApptStatus=="6"){//planned
				command+="PlannedAptNum="+strAptNum;
			}
			else{
				command+="AptNum="+strAptNum;//exclude procs attached to other appts.
			}
			//3. All unattached completed procs with same date as appt.
			//but only if one of these types
			if(strApptStatus=="1" || strApptStatus=="2" || strApptStatus=="4" || strApptStatus=="5"){//sched,C,ASAP,broken
				DateTime dateApt=PIn.DateT(strAptDateTime);
				command+=" OR (AptNum=0 "//unattached
					+"AND ProcStatus=2 "//complete
					+"AND "+DbHelper.DtimeToDate("ProcDate")+"="+POut.Date(dateApt)+")";//same date
			}
			command+=") "
				+"AND ProcStatus<>6 "//Not deleted.
				+"AND IsCanadianLab=0";
			DataTable tableProcRaw=Db.GetTable(command);
			for(int i=0;i<tableProcRaw.Rows.Count;i++) {
				dataRow=table.NewRow();
				dataRow["AbbrDesc"]=tableProcRaw.Rows[i]["AbbrDesc"].ToString();
				if(strApptStatus=="6"){//planned
					dataRow["attached"]=(tableProcRaw.Rows[i]["PlannedAptNum"].ToString()==strAptNum) ? "1" : "0";
				}
				else{
					dataRow["attached"]=(tableProcRaw.Rows[i]["AptNum"].ToString()==strAptNum) ? "1" : "0";
				}
				dataRow["CodeNum"]=tableProcRaw.Rows[i]["CodeNum"].ToString();
				dataRow["descript"]="";
				if(strApptStatus=="6") {//planned
					if(tableProcRaw.Rows[i]["PlannedAptNum"].ToString()!="0" && tableProcRaw.Rows[i]["PlannedAptNum"].ToString()!=strAptNum) {
						dataRow["descript"]=Lans.g("FormApptEdit","(other appt)");
					}
					else if(tableProcRaw.Rows[i]["AptNum"].ToString()!="0" && tableProcRaw.Rows[i]["AptNum"].ToString()!=strAptNum) {
						dataRow["descript"]=Lans.g("FormApptEdit","(scheduled appt)");
					}
				}
				else {
					if(tableProcRaw.Rows[i]["AptNum"].ToString()!="0" && tableProcRaw.Rows[i]["AptNum"].ToString()!=strAptNum) {
						dataRow["descript"]=Lans.g("FormApptEdit","(other appt)");
					}
					else if(tableProcRaw.Rows[i]["PlannedAptNum"].ToString()!="0" && tableProcRaw.Rows[i]["PlannedAptNum"].ToString()!=strAptNum) {
						dataRow["descript"]=Lans.g("FormApptEdit","(planned appt)");
					}
				}
				if(tableProcRaw.Rows[i]["LaymanTerm"].ToString()==""){
					dataRow["descript"]+=tableProcRaw.Rows[i]["Descript"].ToString();
				}
				else{
					dataRow["descript"]+=tableProcRaw.Rows[i]["LaymanTerm"].ToString();
				}
				if(tableProcRaw.Rows[i]["ToothRange"].ToString()!=""){
					dataRow["descript"]+=" #"+Tooth.DisplayRange(tableProcRaw.Rows[i]["ToothRange"].ToString());
				}
				dataRow["fee"]=PIn.Double(tableProcRaw.Rows[i]["ProcFee"].ToString()).ToString("F");
				dataRow["priority"]=Defs.GetName(DefCat.TxPriorities,PIn.Long(tableProcRaw.Rows[i]["Priority"].ToString()));
				dataRow["Priority"]=tableProcRaw.Rows[i]["Priority"].ToString();
				dataRow["ProcCode"]=tableProcRaw.Rows[i]["ProcCode"].ToString();
				dataRow["ProcDate"]=tableProcRaw.Rows[i]["ProcDate"].ToString();//eg 2012-02-19
				dataRow["ProcNum"]=tableProcRaw.Rows[i]["ProcNum"].ToString();
				dataRow["ProcStatus"]=tableProcRaw.Rows[i]["ProcStatus"].ToString();
				dataRow["ProvNum"]=tableProcRaw.Rows[i]["ProvNum"].ToString();
				dataRow["status"]=((ProcStat)PIn.Long(tableProcRaw.Rows[i]["ProcStatus"].ToString())).ToString();
				dataRow["Surf"]=tableProcRaw.Rows[i]["Surf"].ToString();
				dataRow["toothNum"]=Tooth.Display(tableProcRaw.Rows[i]["ToothNum"].ToString());
				dataRow["ToothNum"]=tableProcRaw.Rows[i]["ToothNum"].ToString();
				dataRow["ToothRange"]=tableProcRaw.Rows[i]["ToothRange"].ToString();
				dataRow["TreatArea"]=tableProcRaw.Rows[i]["TreatArea"].ToString();
				listDataRows.Add(dataRow);
			}
			//Sorting
			listDataRows.Sort(CompareRows);
			for(int i=0;i<listDataRows.Count;i++) {
				table.Rows.Add(listDataRows[i]);
			}
			return table;
		}

		///<summary>Used in FormConfirmList.  The assumption is made that showRecall and showNonRecall will not both be false. Pass in a clinicNum
		///less than 0 to return results for all clinics.</summary>
		public static DataTable GetConfirmList(DateTime dateFrom,DateTime dateTo,long provNum,long clinicNum,bool showRecall,bool showNonRecall,bool showHygPresched,long defNumConfirmStatus,
			bool groupFamilies) 
			{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,provNum,clinicNum,showRecall,showNonRecall,showHygPresched,defNumConfirmStatus,groupFamilies);
			}
			DataTable table=new DataTable();
			DataRow dataRow;
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
			table.Columns.Add("PriProv");
			table.Columns.Add("ProcDescript");
			table.Columns.Add("TxtMsgOk");
			table.Columns.Add("WirelessPhone");
			List<DataRow> listDataRows=new List<DataRow>();
			string command="SELECT patient.PatNum,patient.LName,patient.FName,patient.Preferred,patient.LName,patient.Guarantor,"
				+"AptDateTime,patient.Birthdate,patient.ClinicNum,patient.HmPhone,patient.TxtMsgOk,patient.WkPhone,patient.PriProv,"
				+"patient.WirelessPhone,appointment.ProcDescript,appointment.Confirmed,appointment.Note,patient.AddrNote,appointment.AptNum,patient.MedUrgNote,"
				+"patient.PreferConfirmMethod,guar.Email guarEmail,guar.WirelessPhone guarWirelessPhone,guar.TxtMsgOK guarTxtMsgOK,guar.ClinicNum guarClinicNum,"
				+"guar.PreferConfirmMethod guarPreferConfirmMethod,patient.Email,patient.Premed,appointment.DateTimeAskedToArrive,securitylog.LogDateTime,"
				+"guar.FName AS guarNameF "
				+"FROM patient "
				+"INNER JOIN appointment ON appointment.PatNum=patient.PatNum "
				+"INNER JOIN patient guar ON guar.PatNum=patient.Guarantor "
				+"LEFT JOIN securitylog ON securitylog.PatNum=appointment.PatNum AND securitylog.PermType="+POut.Int((int)EnumPermType.AppointmentCreate)+" "
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
			if(defNumConfirmStatus>0) {
				command+=" AND appointment.Confirmed="+POut.Long(defNumConfirmStatus)+" ";
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
			DataTable tableRaw=Db.GetTable(command);
			ContactMethod contactMethod;
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				dataRow=table.NewRow();
				dataRow["AddrNote"]=tableRaw.Rows[i]["AddrNote"].ToString();
				dataRow["AptNum"]=tableRaw.Rows[i]["AptNum"].ToString();
				dataRow["age"]=Patients.DateToAge(PIn.Date(tableRaw.Rows[i]["Birthdate"].ToString())).ToString();//we don't care about m/y.
				dataRow["AptDateTime"]=PIn.DateT(tableRaw.Rows[i]["AptDateTime"].ToString());
				dataRow["DateTimeAskedToArrive"]=PIn.DateT(tableRaw.Rows[i]["DateTimeAskedToArrive"].ToString());
				dataRow["ClinicNum"]=tableRaw.Rows[i]["ClinicNum"].ToString();
				dataRow["confirmed"]=Defs.GetName(DefCat.ApptConfirmed,PIn.Long(tableRaw.Rows[i]["Confirmed"].ToString()));
				contactMethod=(ContactMethod)PIn.Int(tableRaw.Rows[i]["PreferConfirmMethod"].ToString());
				if(contactMethod==ContactMethod.None || contactMethod==ContactMethod.HmPhone) {
					dataRow["contactMethod"]=Lans.g("FormConfirmList","Hm:")+tableRaw.Rows[i]["HmPhone"].ToString();
				}
				if(contactMethod==ContactMethod.WkPhone) {
					dataRow["contactMethod"]=Lans.g("FormConfirmList","Wk:")+tableRaw.Rows[i]["WkPhone"].ToString();
				}
				if(contactMethod==ContactMethod.WirelessPh) {
					dataRow["contactMethod"]=Lans.g("FormConfirmList","Cell:")+tableRaw.Rows[i]["WirelessPhone"].ToString();
				}
				if(contactMethod==ContactMethod.TextMessage) {
					dataRow["contactMethod"]=Lans.g("FormConfirmList","Text:")+tableRaw.Rows[i]["WirelessPhone"].ToString();
				}
				if(contactMethod==ContactMethod.Email) {
					dataRow["contactMethod"]=tableRaw.Rows[i]["Email"].ToString();
				}
				if(contactMethod==ContactMethod.DoNotCall || contactMethod==ContactMethod.SeeNotes) {
					dataRow["contactMethod"]=Lans.g("enumContactMethod",contactMethod.ToString());
				}
				if(contactMethod==ContactMethod.Mail) {
					dataRow["contactMethod"]=Lans.g("FormConfirmList","Mail");
				}
				if(groupFamilies) {//Grouped families need to display guarantor information for specfic contact methods instead
					contactMethod=(ContactMethod)PIn.Int(tableRaw.Rows[i]["guarPreferConfirmMethod"].ToString());
					if(contactMethod==ContactMethod.WirelessPh) {
						dataRow["contactMethod"]=Lans.g("FormConfirmList","Cell:")+tableRaw.Rows[i]["guarWirelessPhone"].ToString();
					}
					if(contactMethod==ContactMethod.TextMessage) {
						dataRow["contactMethod"]=Lans.g("FormConfirmList","Text:")+tableRaw.Rows[i]["guarWirelessPhone"].ToString();
					}
					if(contactMethod==ContactMethod.Email) {
						dataRow["contactMethod"]=tableRaw.Rows[i]["guarEmail"].ToString();
					}
				}
				dataRow["dateSched"]="Unknown";
				if(tableRaw.Rows[i]["LogDateTime"].ToString().Length>0) {
					dataRow["dateSched"]=tableRaw.Rows[i]["LogDateTime"].ToString();
				}
				if(tableRaw.Rows[i]["Email"].ToString()=="" && tableRaw.Rows[i]["guarEmail"].ToString()!="") {
					dataRow["email"]=tableRaw.Rows[i]["guarEmail"].ToString();
				}
				else {
					dataRow["email"]=tableRaw.Rows[i]["Email"].ToString();
				}
				dataRow["Guarantor"]=tableRaw.Rows[i]["Guarantor"].ToString();
				dataRow["guarClinicNum"]=tableRaw.Rows[i]["guarClinicNum"].ToString();
				dataRow["guarEmail"]=tableRaw.Rows[i]["guarEmail"].ToString();
				dataRow["guarNameF"]=tableRaw.Rows[i]["guarNameF"].ToString();
				dataRow["guarPreferConfirmMethod"]=tableRaw.Rows[i]["guarPreferConfirmMethod"].ToString();
				dataRow["guarTxtMsgOK"]=tableRaw.Rows[i]["guarTxtMsgOK"].ToString();
				dataRow["guarWirelessPhone"]=tableRaw.Rows[i]["guarWirelessPhone"].ToString();
				dataRow["medNotes"]="";
				if(tableRaw.Rows[i]["Premed"].ToString()=="1"){
					dataRow["medNotes"]=Lans.g("FormConfirmList","Premedicate");
				}
				if(tableRaw.Rows[i]["MedUrgNote"].ToString()!=""){
					if(dataRow["medNotes"].ToString()!="") {
						dataRow["medNotes"]+="\r\n";
					}
					dataRow["medNotes"]+=tableRaw.Rows[i]["MedUrgNote"].ToString();
				}
				dataRow["FName"]=tableRaw.Rows[i]["FName"].ToString();
				dataRow["LName"]=tableRaw.Rows[i]["LName"].ToString();
				dataRow["Note"]=tableRaw.Rows[i]["Note"].ToString();
				dataRow["PatNum"]=tableRaw.Rows[i]["PatNum"].ToString();
				dataRow["PreferConfirmMethod"]=tableRaw.Rows[i]["PreferConfirmMethod"].ToString();
				dataRow["Preferred"]=tableRaw.Rows[i]["Preferred"].ToString();
				dataRow["PriProv"]=tableRaw.Rows[i]["PriProv"].ToString();
				dataRow["ProcDescript"]=tableRaw.Rows[i]["ProcDescript"].ToString();
				dataRow["TxtMsgOk"]=tableRaw.Rows[i]["TxtMsgOk"].ToString();
				dataRow["WirelessPhone"]=tableRaw.Rows[i]["WirelessPhone"].ToString();
				listDataRows.Add(dataRow);
			}
			for(int i=0;i<listDataRows.Count;i++) {
				table.Rows.Add(listDataRows[i]);
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			string strFamilyAptList="";
			string patNumStr="";
			DataTable tableRaw=Db.GetTable(command);
			DataRow dataRow;
			List<DataRow> listDataRows=new List<DataRow>();
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				Patient patient=new Patient {
					PatNum=PIn.Long(tableRaw.Rows[i]["PatNum"].ToString()),
					FName=PIn.String(tableRaw.Rows[i]["FName"].ToString()),
					Preferred=PIn.String(tableRaw.Rows[i]["Preferred"].ToString()),
					MiddleI=PIn.String(tableRaw.Rows[i]["MiddleI"].ToString()),
					LName=PIn.String(tableRaw.Rows[i]["LName"].ToString()),
				};
				DateTime dateTimeApt=PIn.Date(tableRaw.Rows[i]["AptDateTime"].ToString());
				DateTime dateTimeAskedToArrive=PIn.Date(tableRaw.Rows[i]["DateTimeAskedToArrive"].ToString());
				if(!groupByFamily) {
					dataRow=table.NewRow();
					dataRow["Address"]=tableRaw.Rows[i]["Address"].ToString();
					if(tableRaw.Rows[i]["Address2"].ToString()!="") {
						dataRow["Address2"]=tableRaw.Rows[i]["Address2"].ToString();
					}
					dataRow["City"]=tableRaw.Rows[i]["City"].ToString();
					dataRow["clinicNum"]=tableRaw.Rows[i]["ClinicNum"].ToString();
					dataRow["AptDateTime"]=dateTimeApt;
					dataRow["DateTimeAskedToArrive"]=dateTimeAskedToArrive;
					//since not grouping by family, this is always just the patient email
					dataRow["email"]=tableRaw.Rows[i]["Email"].ToString();
					dataRow["famList"]="";
					dataRow["guarLName"]=tableRaw.Rows[i]["guarLName"].ToString();
					dataRow["FName"]=patient.FName;
					dataRow["LName"]=patient.LName;
					dataRow["MiddleI"]=patient.MiddleI;
					dataRow["patNums"]=patient.PatNum;
					dataRow["PatNum"]=patient.PatNum;
					dataRow["Preferred"]=patient.Preferred;
					dataRow["State"]=tableRaw.Rows[i]["State"].ToString();
					dataRow["Zip"]=tableRaw.Rows[i]["Zip"].ToString();
					table.Rows.Add(dataRow);
					continue;
				}
				//groupByFamily----------------------------------------------------------------------
				if(strFamilyAptList=="") {//if this is the first patient in the family
					if(i==tableRaw.Rows.Count-1//if this is the last row
						|| tableRaw.Rows[i]["Guarantor"].ToString()!=tableRaw.Rows[i+1]["Guarantor"].ToString())//or if the guarantor on next line is different
					{
						//then this is a single patient, and there are no other family members in the list.
						dataRow=table.NewRow();
						dataRow["Address"]=tableRaw.Rows[i]["Address"].ToString();
						if(tableRaw.Rows[i]["Address2"].ToString()!="") {
							dataRow["Address2"]=tableRaw.Rows[i]["Address2"].ToString();
						}
						dataRow["City"]=tableRaw.Rows[i]["City"].ToString();
						dataRow["State"]=tableRaw.Rows[i]["State"].ToString();
						dataRow["Zip"]=tableRaw.Rows[i]["Zip"].ToString();
						dataRow["clinicNum"]=tableRaw.Rows[i]["ClinicNum"].ToString();
						dataRow["AptDateTime"]=dateTimeApt;
						dataRow["DateTimeAskedToArrive"]=dateTimeAskedToArrive;
						//this will always be the guarantor email
						dataRow["email"]=tableRaw.Rows[i]["guarEmail"].ToString();
						dataRow["famList"]="";
						dataRow["guarLName"]=tableRaw.Rows[i]["guarLName"].ToString();
						dataRow["FName"]=patient.FName;
						dataRow["LName"]=patient.LName;
						dataRow["MiddleI"]=patient.MiddleI;
						dataRow["patNums"]=patient.PatNum;
						dataRow["PatNum"]=patient.PatNum;
						dataRow["Preferred"]=patient.Preferred;
						table.Rows.Add(dataRow);
						continue;
					}
					else{//this is the first patient of a family with multiple family members
						strFamilyAptList=PatComm.BuildAppointmentMessage(patient,dateTimeAskedToArrive,dateTimeApt);
						patNumStr=tableRaw.Rows[i]["PatNum"].ToString();
						continue;
					}
				}
				else{//not the first patient
					strFamilyAptList+="\r\n"+PatComm.BuildAppointmentMessage(patient,dateTimeAskedToArrive,dateTimeApt);
					patNumStr+=","+tableRaw.Rows[i]["PatNum"].ToString();
				}
				if(i==tableRaw.Rows.Count-1//if this is the last row
					|| tableRaw.Rows[i]["Guarantor"].ToString()!=tableRaw.Rows[i+1]["Guarantor"].ToString())//or if the guarantor on next line is different
				{
					//This part only happens for the last family member of a grouped family
					dataRow=table.NewRow();
					dataRow["Address"]=tableRaw.Rows[i]["guarAddress"].ToString();
					if(tableRaw.Rows[i]["guarAddress2"].ToString()!="") {
						dataRow["Address2"]+=tableRaw.Rows[i]["guarAddress2"].ToString();
					}
					dataRow["City"]=tableRaw.Rows[i]["guarCity"].ToString();
					dataRow["State"]=tableRaw.Rows[i]["guarState"].ToString();
					dataRow["Zip"]=tableRaw.Rows[i]["guarZip"].ToString();
					dataRow["clinicNum"]=tableRaw.Rows[i]["guarClinicNum"].ToString();
					dataRow["AptDateTime"]=dateTimeApt;
					dataRow["DateTimeAskedToArrive"]=dateTimeAskedToArrive;
					dataRow["email"]=tableRaw.Rows[i]["guarEmail"].ToString();
					dataRow["famList"]=strFamilyAptList;
					dataRow["guarLName"]=tableRaw.Rows[i]["guarLName"].ToString();
					dataRow["FName"]=patient.FName;
					dataRow["LName"]=patient.LName;
					dataRow["MiddleI"]=patient.MiddleI;
					dataRow["patNums"]=patNumStr;
					dataRow["PatNum"]=patient.PatNum;
					dataRow["Preferred"]=patient.Preferred;
					table.Rows.Add(dataRow);
					strFamilyAptList="";
				}	
			}
			for(int i=0;i<listDataRows.Count;i++) {
				table.Rows.Add(listDataRows[i]);
			}
			return table;
		}

		///<summary>Manipulates ProcDescript and ProcsColored on the appointment passed in.  Does not update the database.
		///Pass in the list of procs attached to the appointment to avoid a db call to the procedurelog table.</summary>
		public static void SetProcDescript(Appointment appointment,List<Procedure> listProcedures = null) {
			appointment.ProcDescript="";
			appointment.ProcsColored="";
			if(listProcedures == null) {
				listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,appointment.AptStatus == ApptStatus.Planned);
			}
			List<ProcedureCode> listProcCodes = ProcedureCodes.GetCodesForCodeNums(listProcedures.Select(x => x.CodeNum).ToList());
			foreach(Procedure proc in listProcedures) {
				if(appointment.AptStatus==ApptStatus.Planned && appointment.AptNum != proc.PlannedAptNum) {
					continue;
				}
				if(appointment.AptStatus != ApptStatus.Planned && appointment.AptNum != proc.AptNum) {
					continue;
				}
				string procDescOne = "";
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				if(!string.IsNullOrEmpty(appointment.ProcDescript)) {
					appointment.ProcDescript+=", ";
				}
				string displaySurf;
				if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Sextant) {
					displaySurf=Tooth.GetSextant(proc.Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
				}
				else {
					displaySurf=Tooth.SurfTidyFromDbToDisplay(proc.Surf,proc.ToothNum);//Fixes surface display for Canadian users
				}
				switch(procedureCode.TreatArea) {
					case TreatmentArea.Surf:
						procDescOne+="#"+Tooth.Display(proc.ToothNum)+"-"+displaySurf+"-";//""#12-MOD-"
						break;
					case TreatmentArea.Tooth:
						procDescOne+="#"+Tooth.Display(proc.ToothNum)+"-";//"#12-"
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
				procDescOne+=procedureCode.AbbrDesc;
				appointment.ProcDescript+=procDescOne;
				//Color and previous date are determined by ProcApptColor object
				ProcApptColor procApptColor = ProcApptColors.GetMatch(procedureCode.ProcCode);
				System.Drawing.Color colorProc = System.Drawing.Color.Black;
				string prevDateString = "";
				if(procApptColor!=null) {
					colorProc=procApptColor.ColorText;
					if(procApptColor.ShowPreviousDate) {
						prevDateString=Procedures.GetRecentProcDateString(appointment.PatNum,appointment.AptDateTime,procApptColor.CodeRange);
						if(prevDateString!="") {
							prevDateString=" ("+prevDateString+")";
						}
					}
				}
				appointment.ProcsColored+="<span color=\""+colorProc.ToArgb().ToString()+"\">"+procDescOne+prevDateString+"</span>";
			}
		}

		///<summary>Returns a list of special appointment objects that only contain information necessary to fill the OtherAppts window.
		///The main purpose of this method is to significantly cut back on the amount of data sent back to the client.</summary>
		public static List<ApptOther> GetApptOthersForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//Remoting role check in order to limit the amount of data sent back to the client.
				return Meth.GetObject<List<ApptOther>>(MethodBase.GetCurrentMethod(),patNum);
			}
			Appointment[] appointmentArray=GetForPat(patNum);
			return appointmentArray.Select(x => new ApptOther(x)).ToList();
		}

		///<summary>Returns all appointments for the given patient, ordered from earliest to latest. Used in statements, appt cards, OtherAppts window, etc.</summary>
		public static Appointment[] GetForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Appointment[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM appointment "
				+"WHERE PatNum = '"+POut.Long(patNum)+"' "
				+"AND NOT (AptDateTime < "+POut.Date(new DateTime(1880,1,1))+" AND AptStatus="+POut.Int((int)ApptStatus.UnschedList)+") "
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command).ToArray();
		}

		///<summary>Gets all appointments for a single patient ordered by AptDateTime.</summary>
		public static List<Appointment> GetPatientData(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM appointment "
				+"WHERE PatNum = '"+POut.Long(patNum)+"' "
				+"AND NOT (AptDateTime < "+POut.Date(new DateTime(1880,1,1))+" AND AptStatus="+POut.Int((int)ApptStatus.UnschedList)+") "
				//The above line is for a very rare edge case where a new appointment can be on the pinboard.
				//The above line does not exclude Unsched appts.
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Gets one appointment from db.  Returns null if not found.</summary>
		public static Appointment GetOneApt(long aptNum) {
			if(aptNum==0) {
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Appointment>(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptNum = "+POut.Long(aptNum);
			return Crud.AppointmentCrud.SelectOne(command);
		}

		///<summary>Gets one AppointForApi from db. Returns null if not found.</summary>
		public static AppointmentWithServerDT GetOneAptForApi(long aptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<AppointmentWithServerDT>(MethodBase.GetCurrentMethod(),aptNum);
			}
			if(aptNum==0) {
				return null;
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptNum = '"+POut.Long(aptNum)+"'";
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before appts for rigorous inclusion of appts
			AppointmentWithServerDT appointmentWithServerDT=new AppointmentWithServerDT();
			appointmentWithServerDT.AppointmentCur=Crud.AppointmentCrud.SelectOne(command);
			appointmentWithServerDT.DateTimeServer=dateTimeServer;
			return appointmentWithServerDT;
		}

		///<summary>Gets an appointment (of any status) from the db with this NextAptNum (FK to the AptNum of a planned appt).</summary>
		public static Appointment GetScheduledPlannedApt(long nextAptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime > "+DbHelper.Now()+" "
				+"AND AptStatus = "+(int)ApptStatus.Scheduled+" "
				+"ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		public static List<Appointment> GetChangedSince(DateTime dateTimeChangedSince,DateTime dateTimeExcludeOlderThan) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateTimeChangedSince,dateTimeExcludeOlderThan);
			}
			string command="SELECT * FROM appointment WHERE DateTStamp > "+POut.DateT(dateTimeChangedSince)
				+" AND AptDateTime > "+POut.DateT(dateTimeExcludeOlderThan);
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Used if the number of records are very large, in which case using GetChangedSince is not the preffered route due to memory problems caused by large recordsets. </summary>
		public static List<long> GetChangedSinceAptNums(DateTime dateTimeChangedSince,DateTime dateTimeExcludeOlderThan) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateTimeChangedSince,dateTimeExcludeOlderThan);
			}
			string command="SELECT AptNum FROM appointment WHERE DateTStamp > "+POut.DateT(dateTimeChangedSince)
				+" AND AptDateTime > "+POut.DateT(dateTimeExcludeOlderThan);
			DataTable table=Db.GetTable(command);
			List<long> aptnums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				aptnums.Add(PIn.Long(table.Rows[i]["AptNum"].ToString()));
			}
			return aptnums;
		}

		///<summary>Gets a list of appointments for the AptNums passed in.</summary>
		public static List<Appointment> GetMultApts(List<long> listAptNums) {
			if(listAptNums.IsNullOrEmpty()) {
				return new List<Appointment>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),listAptNums);
			}
			string command="SELECT * FROM appointment WHERE AptNum IN ("+string.Join(",",listAptNums)+")";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Gets AptNums and AptDateTimes to use for task sorting with the TaskUseApptDate pref.</summary>
		public static DataTable GetAptDateTimeForAptNums(List<long> listAptNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listAptNums);
			}
			if(listAptNums.Count==0) {
				return new DataTable();
			}
			string command="SELECT AptNum,AptDateTime,AptStatus FROM appointment WHERE AptNum IN ("+string.Join(",",listAptNums)+")";
			return Db.GetTable(command);
		}

		///<summary>Gets a list of appointments for a period of time in the schedule, whether hidden or not.</summary>
		public static Appointment[] GetForPeriod(DateTime dateTStart,DateTime dateTEnd){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Appointment[]>(MethodBase.GetCurrentMethod(),dateTStart,dateTEnd);
			}
			//DateSelected = thisDay;
			string command=
				"SELECT * from appointment "
				+"WHERE AptDateTime BETWEEN "+POut.Date(dateTStart)+" AND "+POut.Date(dateTEnd.AddDays(1))+" "
				+"AND aptstatus != '"+(int)ApptStatus.UnschedList+"' "
				+"AND aptstatus != '"+(int)ApptStatus.Planned+"'";
			return Crud.AppointmentCrud.SelectMany(command).ToArray();
		}

		///<summary>Gets appointments for a period of time, whether hidden or not.  Optionally pass in a clinic to only get appointments associated to that clinic.  clinicNum of 0 will get all appointments.</summary>
		public static List<Appointment> GetForPeriodList(DateTime dateTStart,DateTime dateTEnd,long clinicNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateTStart,dateTEnd,clinicNum);
			}
			//DateSelected = thisDay;
			string command=
				"SELECT * FROM appointment "
				+"WHERE AptDateTime BETWEEN "+POut.Date(dateTStart)+" AND "+POut.Date(dateTEnd.AddDays(1))+" "
				+"AND AptStatus != '"+(int)ApptStatus.UnschedList+"' "
				+"AND AptStatus != '"+(int)ApptStatus.Planned+"' "
				+(clinicNum > 0 ? "AND ClinicNum="+POut.Long(clinicNum) : "");
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Used by API. Gets appointments in a date range based on ProvNum or ProvHyg.</summary>
		public static List<Appointment> GetForProv(DateTime dateTStart,DateTime dateTEnd,long provNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateTStart,dateTEnd,provNum);
			}
			string command=
				"SELECT * FROM appointment "
				+"WHERE AptDateTime BETWEEN "+POut.Date(dateTStart)+" AND "+POut.Date(dateTEnd.AddDays(1))+" "//Between is inclusive. Midnight to midnight.
				+"AND AptStatus != '"+(int)ApptStatus.UnschedList+"' "
				+"AND AptStatus != '"+(int)ApptStatus.Planned+"' "
				+"AND (ProvNum="+POut.Long(provNum)+" OR ProvHyg="+POut.Long(provNum)+")";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Overload. Gets a List of appointments for a period of time in the schedule, whether hidden or not.
		///Optionally pass in a list of clinics to only get appointments associated to those clinics.
		///An empty list of ClinicNums will get all appointments.</summary>
		public static List<Appointment> GetForPeriodList(DateTime dateTStart,DateTime datetTEnd,List<long> listOpNums,List<long> listClinicNums) {
			if(listOpNums==null || listOpNums.Count < 1) {
				return new List<Appointment>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateTStart,datetTEnd,listOpNums,listClinicNums);
			}
			string command=
				"SELECT * FROM appointment "
				+"WHERE appointment.AptDateTime BETWEEN "+POut.Date(dateTStart)+" AND "+POut.Date(datetTEnd.AddDays(1))+" "
				+"AND appointment.AptStatus != '"+(int)ApptStatus.UnschedList+"' "
				+"AND appointment.AptStatus != '"+(int)ApptStatus.Broken+"' "
				+"AND appointment.AptStatus != '"+(int)ApptStatus.Planned+"' "
				+"AND appointment.Op IN ("+string.Join(",",listOpNums)+") ";
			if(listClinicNums.Count>0) {
				command+="AND appointment.ClinicNum IN ("+string.Join(",",listClinicNums)+")";
			}
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>A list of strings.  Each string corresponds to one appointment in the supplied list.  Each string is a comma delimited list of codenums of the procedures attached to the appointment.</summary>
		public static List<string> GetUAppointProcs(List<Appointment> listAppointments){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),listAppointments);
			}
			List<string> listStrAppointmentProcs=new List<string>();
			if(listAppointments.Count==0){
				return listStrAppointmentProcs;
			}
			string command="SELECT AptNum,CodeNum FROM procedurelog WHERE AptNum IN(";
			for(int i=0;i<listAppointments.Count;i++){
				if(i>0){
					command+=",";
				}
				command+=POut.Long(listAppointments[i].AptNum);
			}
			command+=")";
			DataTable table=Db.GetTable(command);
			string str;
			for(int i=0;i<listAppointments.Count;i++){
				str="";
				for(int p=0;p<table.Rows.Count;p++){
					if(table.Rows[p]["AptNum"].ToString()==listAppointments[i].AptNum.ToString()){
						if(str!=""){
							str+=",";
						}
						str+=table.Rows[p]["CodeNum"].ToString();
					}
				}
				listStrAppointmentProcs.Add(str);
			}
			return listStrAppointmentProcs;
		}

		///<summary>Returns list of appointments for today for one patient. This ignores unscheduled/broken appointments.</summary>
		public static List<Appointment> GetTodaysApptsForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			List<Appointment> listAppointments=new List<Appointment>();
			string command=$"SELECT * FROM appointment WHERE PatNum={POut.Long(patNum)} " +
				$"AND {DbHelper.BetweenDates("AptDateTime",DateTime.Today,DateTime.Today)} "+
				$"AND AptStatus NOT IN({POut.Int((int)ApptStatus.UnschedList)},{POut.Int((int)ApptStatus.Broken)})";
			listAppointments=Crud.AppointmentCrud.SelectMany(command);
			return listAppointments;
		}

		///<summary>Gets list of asap AppointmentsForApi. Pass in a clinicNum less than 0 to get ASAP appts for all clinics.</summary>
		public static List<AppointmentWithServerDT> RefreshAsapForApi(int limit,int offset,long provNum,long siteNum,long clinicNum,
			List<ApptStatus> listApptStatuses,string codeRangeStart="",string codeRangeEnd="") 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AppointmentWithServerDT>>(MethodBase.GetCurrentMethod(),limit,offset,provNum,siteNum,clinicNum,listApptStatuses,codeRangeStart,codeRangeEnd);
			}
			List<long> listCodeNums=new List<long>();
			if(!string.IsNullOrEmpty(codeRangeStart)) {
				//Get a list of CodeNums that meet the procedure code range passed in.
				listCodeNums=Db.GetListLong(
					"SELECT CodeNum FROM procedurecode "
					+"WHERE ProcCode BETWEEN '"+POut.String(codeRangeStart)+"' AND '"+POut.String(codeRangeEnd)+"' ");
				if(listCodeNums.Count==0) { //ProcCodes do not exist.
					return new List<AppointmentWithServerDT>();
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
			if(listApptStatuses.Count>0) {
				command+="AND appointment.AptStatus IN ("+string.Join(",",listApptStatuses.Select(x => POut.Int((int)x)))+") ";
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
			+"ORDER BY appointment.AptDateTime "
			+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before appts for rigorous inclusion of appts
			List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
			if(listCodeNums.Count>0 && listAppointments.Count>0) {
				//Get every procedure's CodeNum and it's corresponding AptNum/PlannedAptNum for all appointments in listAppts.
				command="SELECT procedurelog.AptNum,procedurelog.PlannedAptNum,procedurelog.CodeNum "
					+"FROM procedurelog "
					+"WHERE procedurelog.AptNum IN("+string.Join(",",listAppointments.Select(x => POut.Long(x.AptNum)))+") "
					+"OR procedurelog.PlannedAptNum IN("+string.Join(",",listAppointments.Select(x => POut.Long(x.AptNum)))+") "
					+"GROUP BY procedurelog.AptNum,procedurelog.PlannedAptNum,procedurelog.CodeNum";
				//Sam and Saul tried to speed this up many different ways. This was the best way to make sure we always use indexes on procedurelog.
				var listFilteredAptNum=Db.GetTable(command).AsEnumerable()
					.Select(x => new {
						AptNum=PIn.Long(x["AptNum"].ToString()),
						PlannedApptNum=PIn.Long(x["PlannedAptNum"].ToString()),
						CodeNum=PIn.Long(x["CodeNum"].ToString()),
					})
					//We only care about code nums that were included in the range provided.
					.Where(x => listCodeNums.Any(y => y==x.CodeNum))
					//If the appointment is Unscheduled and Planned, the AptNum will be 0.
					.Select(x => x.AptNum>0?x.AptNum:x.PlannedApptNum).ToList();
				//Remove the appointments that are not in the list of filtered AptNums.
				listAppointments.RemoveAll(x => !listFilteredAptNum.Contains(x.AptNum));
			}
			List<AppointmentWithServerDT> listAppointmentWithServerDTs=new List<AppointmentWithServerDT>();
			for(int i=0;i<listAppointments.Count;i++) {//list can be empty
				AppointmentWithServerDT appointmentWithServerDT=new AppointmentWithServerDT();
				appointmentWithServerDT.AppointmentCur=listAppointments[i];
				appointmentWithServerDT.DateTimeServer=dateTimeServer;
				listAppointmentWithServerDTs.Add(appointmentWithServerDT);
			}
			return listAppointmentWithServerDTs;
		}

		///<summary>Gets list of asap Appointments. Pass in a clinicNum less than 0 to get ASAP appts for all clinics.</summary>
		public static List<Appointment> RefreshASAP(long provNum,long siteNum,long clinicNum,List<ApptStatus> listApptStatuses,string codeRangeStart="",
			string codeRangeEnd="") 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),provNum,siteNum,clinicNum,listApptStatuses,codeRangeStart,codeRangeEnd);
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
			if(listApptStatuses.Count>0) {
				command+="AND appointment.AptStatus IN ("+string.Join(",",listApptStatuses.Select(x => POut.Int((int)x)))+") ";
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
				//Get every procedure's CodeNum and it's corresponding AptNum/PlannedAptNum for all appointments in listAppts.
				command="SELECT procedurelog.AptNum,procedurelog.PlannedAptNum,procedurelog.CodeNum "
					+"FROM procedurelog "
					+"WHERE procedurelog.AptNum IN("+string.Join(",",listAppointments.Select(x => POut.Long(x.AptNum)))+") "
					+"OR procedurelog.PlannedAptNum IN("+string.Join(",",listAppointments.Select(x => POut.Long(x.AptNum)))+") "
					+"GROUP BY procedurelog.AptNum,procedurelog.PlannedAptNum,procedurelog.CodeNum";
				//Sam and Saul tried to speed this up many different ways. This was the best way to make sure we always use indexes on procedurelog.
				var listFilteredAptNum=Db.GetTable(command).AsEnumerable()
					.Select(x => new {
						AptNum=PIn.Long(x["AptNum"].ToString()),
						PlannedApptNum=PIn.Long(x["PlannedAptNum"].ToString()),
						CodeNum=PIn.Long(x["CodeNum"].ToString()),
					})
					//We only care about code nums that were included in the range provided.
					.Where(x => listCodeNums.Any(y => y==x.CodeNum))
					//If the appointment is Unscheduled and Planned, the AptNum will be 0.
					.Select(x => x.AptNum>0?x.AptNum:x.PlannedApptNum).ToList();
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
		public static DataSet RefreshPeriod(DateTime dateStart,DateTime dateEnd,long clinicNum,List<long> listAptNumsPin=null,List<long> listOpNums=null,List<long> listProvNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),dateStart,dateEnd,clinicNum,listAptNumsPin,listOpNums,listProvNums);
			} 
			DataSet dataSet=new DataSet();
			DataTable tableAppt=GetPeriodApptsTable(dateStart,dateEnd,0,false,listAptNumsPin,listOpNums,listProvNums);
			dataSet.Tables.Add(tableAppt);
			dataSet.Tables.Add(Schedules.GetPeriodEmployeeSchedTable(dateStart,dateEnd,clinicNum));
			dataSet.Tables.Add(Schedules.GetPeriodProviderSchedTable(dateStart,dateEnd,clinicNum));
			//retVal.Tables.Add(GetPeriodWaitingRoomTable(clinicNum));
			dataSet.Tables.Add(GetPeriodWaitingRoomTable(DateTime.Now));
			dataSet.Tables.Add(Schedules.GetPeriodSchedule(dateStart,dateEnd,listOpNums));
			dataSet.Tables.Add(GetApptFields(tableAppt));
			dataSet.Tables.Add(GetPatFields(tableAppt.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList()));
			return dataSet;
		}

		///<summary>Allowed orderby: status, alph, date. Pass in a clinicNum less than 0 to get all appointments regardless of clinic.</summary>
		public static List<Appointment> RefreshPlannedTracker(string orderBy,long provNum,long siteNum,long clinicNum,string codeStart,string codeEnd,DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),orderBy,provNum,siteNum,clinicNum,codeStart,codeEnd,dateStart,dateEnd);
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
			string command="SELECT a.* FROM appointment a "
				+"INNER JOIN patient p ON p.PatNum=a.PatNum "
			  +"LEFT JOIN appointment tregular ON a.AptNum=tregular.NextAptNum ";
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
			if(orderBy=="status") {
				command+="LEFT JOIN definition d ON d.DefNum=a.UnschedStatus ";
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
			command+="AND "+DbHelper.DtimeToDate("a.AptDateTime")+" BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND tregular.NextAptNum IS NULL ";
			if(orderBy=="status") {
				command+="ORDER BY d.ItemName,a.AptDateTime";
			} 
			else if(orderBy=="alph") {
				command+="ORDER BY p.LName,p.FName";
			} 
			else { //if(orderby=="date"){
				command+="ORDER BY a.AptDateTime";
			}
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Gets list of unscheduled appointments.  Allowed orderby: status, alph, date.
		///Pass in a negative clinicNum to show all.</summary>
		public static List<Appointment> RefreshUnsched(string orderBy,long provNum,long siteNum,bool includeBrokenAppts,long clinicNum,string codeStart,
			string codeEnd,DateTime dateStart,DateTime dateEnd)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),orderBy,provNum,siteNum,includeBrokenAppts,clinicNum,codeStart,codeEnd,
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
			if(orderBy=="status") {
				command+="ORDER BY UnschedStatus,AptDateTime";
			}
			else if(orderBy=="alph") {
				command+="ORDER BY LName,FName";
			}
			else { //if(orderby=="date"){
				command+="ORDER BY AptDateTime";
			}
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Returns the first DefNum of type DefCat.ApptConfirmed.</summary>
		public static long GetUnconfirmedStatus() {
			//No need to check MiddleTierRole; no call to db.
			return Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;
		}

		///<summary>Returns all DefNums of confirmation statuses that can be considered "confirmed".</summary>
		public static List<long> GetConfirmedStatuses() {
			//No need to check MiddleTierRole; no call to db.
			return new List<long> {
				PrefC.GetLong(PrefName.ApptEConfirmStatusAccepted),
				PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger),
				PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger),
				PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger),
			};
		}

		///<summary>Returns all of the appointments for a given patient that have a status of unscheduled.</summary>
		public static List<Appointment> GetUnschedApptsForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$"SELECT * FROM appointment WHERE AptStatus={POut.Int((int)ApptStatus.UnschedList)} AND PatNum={POut.Long(patNum)} ORDER BY AptDateTime";
			return Crud.AppointmentCrud.SelectMany(command);
		}

		///<summary>Tests to see if this appointment will create a double booking. Returns arrayList with no items in it if no double bookings for 
		///this appt.  But if double booking, then it returns an arrayList of codes which would be double booked.  You must supply the appointment being 
		///scheduled as well as a list of all appointments for that day.  The list can include the appointment being tested if user is moving it to a 
		///different time on the same day.  The ProcsForOne list of procedures needs to contain the procedures for the apt becauese procsMultApts won't 
		///necessarily, especially if it's a planned appt on the pinboard.</summary>
		public static List<string> GetDoubleBookedCodes(Appointment appointment,DataTable tableDay,List<Procedure> listProceduresMultApts,Procedure[] procedureArrayForOne) {
			//No need to check MiddleTierRole; no call to db.
			List<string> listProcCodes=new List<string>();//codes
			//figure out which provider we are testing for
			long provNum;
			if(appointment.IsHygiene){
				provNum=appointment.ProvHyg;
			}
			else{
				provNum=appointment.ProvNum;
			}
			//compute the starting row of this appt
			int convertToY=(int)(((double)appointment.AptDateTime.Hour*(double)60
				/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)
				+(double)appointment.AptDateTime.Minute
				/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)));
			int startIndex=convertToY;
			string pattern=Appointments.ConvertPatternFrom5(appointment.Pattern);
			//keep track of which rows in the entire day would be occupied by provider time for this appt
			ArrayList aptProvTime=new ArrayList();
			for(int k=0;k<pattern.Length;k++){
				if(pattern.Substring(k,1)=="X"){
					aptProvTime.Add(startIndex+k);//even if it extends past midnight, we don't care
				}
			}
			//Now, loop through all the other appointments for the day, and see if any would overlap this one
			bool overlaps;
			List<Procedure> listProcedures;
			bool isDoubleBooked=false;//applies to all appts, not just one at a time.
			DateTime dateTimeApt;
			for(int i=0;i<tableDay.Rows.Count;i++){
				if(tableDay.Rows[i]["AptNum"].ToString()==appointment.AptNum.ToString()){//ignore current apt in its old location
					continue;
				}
				//ignore other providers
				if(tableDay.Rows[i]["IsHygiene"].ToString()=="1" && tableDay.Rows[i]["ProvHyg"].ToString()!=provNum.ToString()){
					continue;
				}
				if(tableDay.Rows[i]["IsHygiene"].ToString()=="0" && tableDay.Rows[i]["ProvNum"].ToString()!=provNum.ToString()){
					continue;
				}
				if(tableDay.Rows[i]["AptStatus"].ToString()==((int)ApptStatus.Broken).ToString()){//ignore broken appts
					continue;
				}
				dateTimeApt=PIn.DateT(tableDay.Rows[i]["AptDateTime"].ToString());
				if(dateTimeApt.Date!=appointment.AptDateTime.Date) {//These appointments are on different days.
					continue;
				}
				//calculate starting row
				//this math is copied from another section of the program, so it's sloppy. Safer than trying to rewrite it:
				convertToY=(int)(((double)dateTimeApt.Hour*(double)60
					/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)
					+(double)dateTimeApt.Minute
					/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)));
				startIndex=convertToY;
				pattern=Appointments.ConvertPatternFrom5(tableDay.Rows[i]["Pattern"].ToString());
				//now compare it to apt
				overlaps=false;
				for(int k=0;k<pattern.Length;k++){
					if(pattern.Substring(k,1)=="X"){
						if(aptProvTime.Contains(startIndex+k)){
							overlaps=true;
							isDoubleBooked=true;
						}
					}
				}
				if(overlaps){
					//we need to add all codes for this appt to retVal
					listProcedures=Procedures.GetProcsOneApt(PIn.Long(tableDay.Rows[i]["AptNum"].ToString()),listProceduresMultApts);
					for(int j=0;j<listProcedures.Count;j++){
						listProcCodes.Add(ProcedureCodes.GetStringProcCode(listProcedures[j].CodeNum));
					}
				}
			}
			//now, retVal contains all double booked procs except for this appt
			//need to all procs for this appt.
			if(isDoubleBooked){
				for(int j=0;j<procedureArrayForOne.Length;j++) {
					listProcCodes.Add(ProcedureCodes.GetStringProcCode(procedureArrayForOne[j].CodeNum));
				}
			}
			return listProcCodes;
		}

		public static DataTable GetDataBatchCC(DateTime dateFrom,DateTime dateTo,List<long> listPatNumsToExclude) {
			//No need to check MiddleTierRole; no call to db.
			List<Appointment> listAppointmentsInPeriod=GetAppointmentsBatchCCInDateRange(dateFrom,dateTo,listPatNumsToExclude);
			List<long> listAptNumsInPeriod=listAppointmentsInPeriod.Select(x => x.AptNum).ToList();
			//Setting DateFrom and DateTo to DateTime.MinValue on purpose. We only want to get data for appointments in listAptNumsInPeriod.
			//We are using the GetPeriodApptsTable because it calculates and returns estimated fees for every appointment that we want. 
			//We could use the GetEstPatientPortion(appt) method in a loop, however, it passes through to the GetPeriodApptsTable method, which seems worse. 
			return GetPeriodApptsTable(DateTime.MinValue,DateTime.MinValue,aptNum:0,isPlanned:false,listAptNumsPin:listAptNumsInPeriod);
		}

		public static List<Appointment> GetAppointmentsBatchCCInDateRange(DateTime dateFrom,DateTime dateTo,List<long> listPatNumsToExclude) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		public static Appointment CreateNewAppointment(Patient patient,Operatory operatory,DateTime dateTimeStart,DateTime dateTimeAskedToArrive
			,string pattern,List<Schedule> listSchedulesPeriod,string apptNote="",long defNumApptConfirmed=0,AppointmentType appointmentType=null,bool isNewPatAppt=true) 
		{
			//No need to check MiddleTierRole; no call to db.
			Appointment appointment=new Appointment();
			appointment.PatNum=patient.PatNum;
			appointment.IsNewPatient=isNewPatAppt;
			//if the pattern passed in is blank, set the time using the default pattern for an appointment with no attached procedures.
			appointment.Pattern=String.IsNullOrWhiteSpace(pattern) ? GetApptTimePatternForNoProcs() : pattern;
			if(patient.PriProv==0) {
				appointment.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			else {
				appointment.ProvNum=patient.PriProv;
			}
			appointment.ProvHyg=patient.SecProv;
			appointment.AptStatus=ApptStatus.Scheduled;
			appointment.AptDateTime=dateTimeStart;
			appointment.DateTimeAskedToArrive=dateTimeAskedToArrive;
			appointment.Op=operatory.OperatoryNum;
			if(defNumApptConfirmed==0) {
				appointment.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;
			}
			else {
				appointment.Confirmed=defNumApptConfirmed;
			}
			//if(operatory.ProvDentist!=0) {//if no dentist is assigned to op, then keep the original dentist.  All appts must have prov.
			//  apt.ProvNum=operatory.ProvDentist;
			//}
			//apt.ProvHyg=operatory.ProvHygienist;
			long provNumAssigned=Schedules.GetAssignedProvNumForSpot(listSchedulesPeriod,operatory,false,appointment.AptDateTime);
			long provNumHygAssigned=Schedules.GetAssignedProvNumForSpot(listSchedulesPeriod,operatory,true,appointment.AptDateTime);
			if(provNumAssigned!=0) {//if no dentist is assigned to op, then keep the original dentist.  All appts must have prov.
				appointment.ProvNum=provNumAssigned;
			}
			appointment.ProvHyg=provNumHygAssigned;
			appointment.IsHygiene=operatory.IsHygiene;
			appointment.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			if(operatory.ClinicNum==0) {
				appointment.ClinicNum=patient.ClinicNum;
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
			appointment.SecurityHash=HashFields(appointment);
			Appointments.Insert(appointment);//Handles inserting signal
			return appointment;
		}

		///<summary>Fills an appointment passed in with all appropriate procedures for the recall passed in. 
		///Set listRecalls to a list of all potential recalls for this patient that MIGHT need to be automatically scheduled for this current appointment.
		///It's up to the calling class to then place the appointment on the pinboard or schedule.  
		///The appointment will be inserted into the database in this method so it's important to delete it if the appointment doesn't get scheduled.  
		///Returns the list of procedures that were created for the appointment so that they can be displayed to Orion users.</summary>
		public static List<Procedure> FillAppointmentForRecall(Appointment appointment,Recall recall,List<Recall> listRecalls,Patient patient
			,List<string> listProcStrs,List<InsPlan> listInsPlans,List<InsSub> listInsSubs) 
		{
			//No need to check MiddleTierRole; no call to db.
			appointment.PatNum=patient.PatNum;
			appointment.AptStatus=ApptStatus.UnschedList;//In all places where this is used, the unsched status with no aptDateTime will cause the appt to be deleted when the pinboard is cleared.
			if(patient.PriProv==0) {
				appointment.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			}
			else {
				appointment.ProvNum=patient.PriProv;
			}
			appointment.ProvHyg=patient.SecProv;
			if(appointment.ProvHyg!=0) {
				appointment.IsHygiene=true;
			}
			appointment.ClinicNum=patient.ClinicNum;
			string recallPattern=Recalls.GetRecallTimePattern(recall,listRecalls,patient,listProcStrs);
			appointment.Pattern=RecallTypes.ConvertTimePattern(recallPattern);
			appointment.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			List<PatPlan> listPatPlans=PatPlans.Refresh(patient.PatNum);
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			InsSub insSub1=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
			InsSub insSub2=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
			appointment.InsPlan1=insSub1.PlanNum;
			appointment.InsPlan2=insSub2.PlanNum;
			appointment.SecurityHash=HashFields(appointment);
			Appointments.Insert(appointment);
			Procedure procedure;
			List<Procedure> listProcedures=new List<Procedure>();
			for(int i=0;i<listProcStrs.Count;i++) {
				procedure=new Procedure();//this will be an insert
				//procnum
				procedure.PatNum=patient.PatNum;
				procedure.AptNum=appointment.AptNum;
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProcStrs[i]);
				procedure.CodeNum=procedureCode.CodeNum;
				procedure.ProcDate=(appointment.AptDateTime.Year>1800 ? appointment.AptDateTime : DateTime.Now);
				procedure.DateTP=DateTime.Now;
				procedure.ProvNum=patient.PriProv;
				//Procedures.Cur.Dx=
				procedure.ClinicNum=patient.ClinicNum;
				procedure.MedicalCode=procedureCode.MedicalCode;
				procedure.ProcFee=Procedures.GetProcFee(patient,listPatPlans,listInsSubs,listInsPlans,procedure.CodeNum,procedure.ProvNum,procedure.ClinicNum,
					procedure.MedicalCode);
				//surf
				//toothnum
				//Procedures.Cur.ToothRange="";
				//ProcCur.NoBillIns=ProcedureCodes.GetProcCode(ProcCur.CodeNum).NoBillIns;
				//priority
				procedure.ProcStatus=ProcStat.TP;
				procedure.Note=ProcCodeNotes.GetNote(procedure.ProvNum,procedure.CodeNum,procedure.ProcStatus); //get the TP note.
				//Procedures.Cur.PriEstim=
				//Procedures.Cur.SecEstim=
				//claimnum
				//nextaptnum
				procedure.BaseUnits=procedureCode.BaseUnits;
				Procedures.SetDiagnosticCodesToDefault(procedure,procedureCode);
				procedure.PlaceService=Clinics.GetPlaceService(procedure.ClinicNum);
				if(Userods.IsUserCpoe(Security.CurUser)) {
					//This procedure is considered CPOE because the provider is the one that has added it.
					procedure.IsCpoe=true;
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					procedure.SiteNum=patient.SiteNum;
				}
				Procedures.Insert(procedure);//no recall synch required
				Procedures.ComputeEstimates(procedure,patient.PatNum,new List<ClaimProc>(),false,listInsPlans,listPatPlans,listBenefits,patient.Age,listInsSubs);
				listProcedures.Add(procedure);
			}
			UpdateProcDescriptForAppts(new List<Appointment>() { appointment });
			return listProcedures;
		}

		///<summary>Insert an appointment, and an invalid appointment signalod. Only pass in secUserNum if InsertIncludeAptNum would otherwise overwrite it. </summary>
		public static void Insert(Appointment appointment,long secUserNum=0) {
			//No need to check MiddleTierRole; no call to db.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				InsertIncludeAptNum(appointment,false,secUserNum);
			}
			else {//Oracle must always have a valid PK.
				appointment.AptNum=DbHelper.GetNextOracleKey("appointment","AptNum");
				InsertIncludeAptNum(appointment,true,secUserNum);
			}
			Signalods.SetInvalidAppt(appointment);
		}

		///<summary>Set includeAptNum to true only in rare situations.  Like when we are inserting for eCW. Inserts an invalid appointment signalod.
		///Only include the secUserNumEntry if a user is not available via Security.CurUser (e.g. MobileWeb calls). </summary>
		public static long InsertIncludeAptNum(Appointment appointment,bool useExistingPK,long secUserNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				appointment.AptNum=Meth.GetLong(MethodBase.GetCurrentMethod(),appointment,useExistingPK,secUserNum);
				return appointment.AptNum;
			}
			if(secUserNum!=0) {
				appointment.SecUserNumEntry=secUserNum;
			}
			else {//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				appointment.SecUserNumEntry=Security.CurUser.UserNum;
			}
			//make sure all fields are properly filled:
			if(appointment.Confirmed==0){
				appointment.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;
			}
			if(appointment.ProvNum==0){
				appointment.ProvNum=Providers.GetFirst(true).ProvNum;
			}
			appointment.SecurityHash=HashFields(appointment);
			double dayInterval=PrefC.GetDouble(PrefName.ApptReminderDayInterval);
			double hourInterval=PrefC.GetDouble(PrefName.ApptReminderHourInterval);
			DateTime dateTAutomationBeginPref=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeStart);
			DateTime dateTAutomationEndPref=PrefC.GetDateT(PrefName.AutomaticCommunicationTimeEnd);
			//ApptComms.InsertForAppt(appt,dayInterval,hourInterval,automationBeginPref,automationEndPref);
			long aptNum=Crud.AppointmentCrud.Insert(appointment,useExistingPK);
			HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Created);
			Signalods.SetInvalidAppt(appointment);
			return aptNum;
		}

		#endregion

		#region Update Methods
		///<summary>Use to send to unscheduled list, to set broken, etc.  Do not use to set complete.  Inserts an invalid appointment signalod.</summary>
		public static void SetAptStatus(Appointment appointment,ApptStatus apptStatusNew,bool suppressHistory=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointment,apptStatusNew,suppressHistory);
				return;
			}
			appointment.AptStatus=apptStatusNew;
			appointment.SecurityHash=HashFields(appointment);			
			string command="UPDATE appointment SET AptStatus="+POut.Long((int)apptStatusNew);
			command+=",SecurityHash='"+POut.String(appointment.SecurityHash)+"'";
			if(apptStatusNew==ApptStatus.UnschedList) {
				command+=",Op=0";//We do this so that this appointment does not stop an operatory from being hidden.
			}
			command+=" WHERE AptNum="+POut.Long(appointment.AptNum);
			Db.NonQ(command);
			if(apptStatusNew!=ApptStatus.Scheduled) {
				AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { appointment.AptNum });
			}
			Signalods.SetInvalidAppt(appointment);
			if(apptStatusNew!=ApptStatus.Scheduled) {
				//ApptComms.DeleteForAppt(aptNum);//Delete the automated reminder if it was unscheduled.
			}
			if(suppressHistory) {//Breaking and charting Missed or Canceled proc codes from an apt causes its own history log creation.
				return;
			}
			HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Changed);
		}

		///<summary>The plan nums that are passed in are simply saved in columns in the appt.  Those two fields are used by approximately one office right now.
		///Inserts an invalid appointment signalod.</summary>
		public static void SetAptStatusComplete(Appointment appointment,long planNum1,long planNum2) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointment,planNum1,planNum2);
				return;
			}
			appointment.AptStatus=ApptStatus.Complete;
			appointment.SecurityHash=HashFields(appointment);
			string command="UPDATE appointment SET "
				+"AptStatus="+POut.Long((int)ApptStatus.Complete)+", "
				+"InsPlan1="+POut.Long(planNum1)+", "
				+"InsPlan2="+POut.Long(planNum2)+", "
				+"SecurityHash='"+POut.String(appointment.SecurityHash)+"' "
				+"WHERE AptNum="+POut.Long(appointment.AptNum);
			Db.NonQ(command);
			AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { appointment.AptNum });
			Signalods.SetInvalidAppt(appointment);
			HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Changed);
		}

		///<summary>Set the priority of the appointment.  Inserts an invalid appointment signalod.</summary>
		public static void SetPriority(Appointment appointment,ApptPriority apptPriority) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointment,apptPriority);
				return;
			}
			string command="UPDATE appointment SET Priority="+POut.Int((int)apptPriority)
				+" WHERE AptNum="+POut.Long(appointment.AptNum);
			Db.NonQ(command);
			Signalods.SetInvalidAppt(appointment);
			HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Changed);
		}

		public static void SetAptTimeLocked() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="UPDATE appointment SET TimeLocked="+POut.Bool(true);
			Signalods.SetInvalid(InvalidType.Appointment);
			Db.NonQ(command);
		}

		///<summary>The defNumApptConfirmed will be a DefNum or 0.  Inserts an invalid appointment signalod.</summary>
		public static void SetConfirmed(Appointment appointment,long defNumApptConfirmed,bool createSheetsForCheckin=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointment,defNumApptConfirmed,createSheetsForCheckin);
				return;
			}
			appointment.Confirmed=defNumApptConfirmed;
			appointment.SecurityHash=HashFields(appointment);
			string command="UPDATE appointment SET Confirmed="+POut.Long(defNumApptConfirmed);
			command+=",SecurityHash='"+POut.String(appointment.SecurityHash)+"'";
			if(PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)==defNumApptConfirmed) {
				command+=",DateTimeArrived="+POut.DateT(DateTime.Now);
				if(createSheetsForCheckin) {
				   Sheets.CreateSheetsForCheckIn(appointment);
				}
			}
			else if(PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger)==defNumApptConfirmed){
				command+=",DateTimeSeated="+POut.DateT(DateTime.Now);
			}
			else if(PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger)==defNumApptConfirmed){
				command+=",DateTimeDismissed="+POut.DateT(DateTime.Now);
			}
			command+=" WHERE AptNum="+POut.Long(appointment.AptNum);
			Db.NonQ(command);
			if(defNumApptConfirmed!=PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined)) {//now the status is not 'Callback'
				AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { appointment.AptNum });
			}
			Signalods.SetInvalidAppt(appointment);
			Plugins.HookAddCode(null,"Appointments.SetConfirmed_end", appointment.AptNum, defNumApptConfirmed); 
			HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Changed);
		}

		///<summary>Sets the new pattern for an appointment.  This is how resizing is done.  Must contain only / and X, with each char representing 5 minutes.
		///Inserts an invalid appointment signalod.</summary>
		public static void SetPattern(Appointment appointment,string newPattern) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointment,newPattern);
				return;
			}
			string command="UPDATE appointment SET Pattern='"+POut.String(newPattern)+"' WHERE AptNum="+POut.Long(appointment.AptNum);
			Db.NonQ(command);
			Signalods.SetInvalidAppt(appointment);
			HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Changed);
		}

		///<summary>Updates only the changed columns and returns the number of rows affected.  Supply an oldApt for comparison.  Inserts an invalid appointment signalod.</summary>
		public static bool Update(Appointment appointment,Appointment appointmentOld,bool suppressHistory=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),appointment,appointmentOld,suppressHistory);
			}
			bool isSuccess=false;
			//ApptComms.UpdateForAppt(appointment);
			if(IsAppointmentHashValid(appointmentOld)) { //Only rehash appointments that are already valid
				appointment.SecurityHash=HashFields(appointment);
			}
			isSuccess=Crud.AppointmentCrud.Update(appointment,appointmentOld);
			if(isSuccess) {
				Signalods.SetInvalidAppt(appointment,appointmentOld);
			}
			if(appointment.AptStatus==ApptStatus.UnschedList && appointment.AptStatus!=appointmentOld.AptStatus) {
				appointment.Op=0;
				SetAptStatus(appointment,appointment.AptStatus);
			}
			if(appointment.Confirmed!=appointmentOld.Confirmed && appointment.Confirmed==PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)) {
				Sheets.CreateSheetsForCheckIn(appointment);
			}
			if(isSuccess && !suppressHistory) {//Something actually changed.
				HistAppointments.CreateHistoryEntry(appointment.AptNum,HistAppointmentAction.Changed);
			}
			if((appointmentOld.Confirmed==PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined) //If the status was 'Callback'
				&& appointment.Confirmed!=PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined))  //and now the status is not 'Callback'.
				|| appointment.AptStatus!=ApptStatus.Scheduled)						 //Or the appointment is no longer scheduled.
			{
				AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { appointment.AptNum });
			}
			return isSuccess;
		}

		///<summary>Updates all appointments for the patient passed with the patients Primary and Secondary dental insurance plans.</summary>
		public static void UpdateInsPlansForPat(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			Family family=Patients.GetFamily(patNum);
			List<PatPlan> listPatPlans=PatPlans.Refresh(patNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			InsSub insSub1=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
			InsSub insSub2=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
			Appointments.UpdateInsPlansForPatHelper(patNum,insSub1.PlanNum,insSub2.PlanNum);
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

		public static void UpdateProcDescriptionForAppt(Procedure procedureNew,Procedure procedureOld) {
			if(procedureNew.AptNum==0 && procedureNew.PlannedAptNum==0 && procedureOld.AptNum==0 && procedureOld.PlannedAptNum==0) {
				return;//Nothing to update.
			}
			long aptNum=procedureNew.AptNum>0 ? procedureNew.AptNum : procedureNew.PlannedAptNum;
			Appointment appointment;
			if(procedureNew.AptNum==0 && procedureOld.AptNum>0) {
				aptNum=procedureOld.AptNum;
			}
			else if(procedureNew.PlannedAptNum==0 && procedureOld.PlannedAptNum>0) {
				aptNum=procedureOld.PlannedAptNum;
			}
			appointment=GetOneApt(aptNum);
			if(appointment==null) {
				return;//Apt not found in db, most likely deleted.
			}
			UpdateProcDescriptForAppts(new List<Appointment>() { appointment });
		}

		///<summary>Updates the ProcDesript and ProcsColored to be current for every appointment passed in.
		///Inserts an invalid appointment signalod.</summary>
		public static void UpdateProcDescriptForAppts(List<Appointment> listAppointments) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAppointments);
				return;
			}
			foreach(Appointment apt in listAppointments) {
				//In the event a null object makes its way in to the list
				if(apt==null) {
					continue;
				}
				Appointment appointmentOld = apt.Copy();
				SetProcDescript(apt);
				if(Appointments.Update(apt,appointmentOld)) {
					Signalods.SetInvalidAppt(apt,appointmentOld);
				}
			}
		}

		///<summary>Updates the ColorOverride for all future appointments of the specified AppointmentType (including all made for today). </summary>
		public static void UpdateFutureApptColorForApptType(AppointmentType appointmentType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointmentType);
				return;
			}
			string command="SELECT * FROM appointment "
				+"WHERE AptDateTime >= "+POut.Date(DateTime.Today)
				+"AND AppointmentTypeNum = "+POut.Long(appointmentType.AppointmentTypeNum);
			List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
			for(int i = 0;i<listAppointments.Count;i++) {
				Appointment appointmentNew=listAppointments[i].Copy();
				appointmentNew.ColorOverride=appointmentType.AppointmentTypeColor;
				Update(appointmentNew,listAppointments[i]);
			}
		}

		///<summary>Inserts or updates the appointment and makes any other related updates to the database.</summary>
		///<exception cref="ApplicationException" />
		public static ApptSaveHelperResult ApptSaveHelper(Appointment appointment,Appointment appointmentOld,bool isInsertRequired,List<Procedure> listProceduresForAppt,
			List<Appointment> listAppointments,List<int> listSelectedIndices,List<long> listProcNumsAttachedStart,bool isPlanned,
			List<InsPlan> listInsPlans,List<InsSub> listInsSubs,long selectedProvNum,long selectedProvHygNum,List<Procedure> listProceduresSelected,bool isNew,
			Patient patient,Family family,bool updateProcFees,bool removeCompleteProcs,bool createSecLog,bool insertHL7) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ApptSaveHelperResult>(MethodBase.GetCurrentMethod(),appointment,appointmentOld,isInsertRequired,listProceduresForAppt,listAppointments,
					listSelectedIndices,listProcNumsAttachedStart,isPlanned,listInsPlans,listInsSubs,selectedProvNum,selectedProvHygNum,listProceduresSelected,isNew,
					patient,family,updateProcFees,removeCompleteProcs,createSecLog,insertHL7);
			}
			ApptSaveHelperResult apptSaveHelperResult=new ApptSaveHelperResult();
			DateTime dateTPrevious=appointment.DateTStamp;
			if(isInsertRequired) {
				Appointments.Insert(appointment);
				//If we are on middle tier, the reference to appointment will not be in the listAppointments anymore.
				//If not on middle tier, the reference to appointment will be the same as what's in the list, so we don't need to add again.
				if(listAppointments.RemoveAll(x => x.AptNum==0) > 0) {
					listAppointments.Add(appointment);
				}
			}
			else {
				Appointments.Update(appointment,appointmentOld);
				//If we are on middle tier, the reference to appointment will not be in the listAppointments anymore.
				//If not on middle tier, the reference to appointment will be the same as what's in the list, so we don't need to add again.
				if(listAppointments.RemoveAll(x => x.AptNum==appointment.AptNum)>0) {
					listAppointments.Add(appointment);
				}
			}
			if(appointment.AptStatus==ApptStatus.Planned) {
				List<Appointment> listAppointmentsChecks=listAppointments;
				for(int i = 0; i<listAppointments.Count;i++) {
					if(listAppointments[i].AptNum==appointment.AptNum || listAppointments[i].AptStatus==ApptStatus.Planned) {
						continue;
					}
					//We must go to db to get these procedures because AptNum is wrong.
					if(!listProceduresSelected.Any(x=>Procedures.GetOneProc(x.ProcNum,false).AptNum==listAppointments[i].AptNum)) {
						continue;
					}
					//We now know that the scheduled appt in this loop has one of our procs attached.
					//Mark this appointment as being derived from the planned appointment we are saving.
					//It's harmless if we set this field on multiple scheduled appts.
					//In that case, when putting a planned appt on the pinboard, it will just use the first one it finds.
					Appointment appointmentScheduled=listAppointments[i].Copy();
					appointmentScheduled.NextAptNum=appointment.AptNum;//planned AptNum
					Appointments.Update(appointmentScheduled,listAppointments[i]);
				}
			}
			Procedures.ProcsAptNumHelper(listProceduresForAppt,appointment,listAppointments,listSelectedIndices,listProcNumsAttachedStart,isPlanned);			
			apptSaveHelperResult.DoRunAutomation=Procedures.UpdateProcsInApptHelper(listProceduresForAppt,patient,appointment,appointmentOld,listInsPlans,listInsSubs,listSelectedIndices,
				removeCompleteProcs,updateProcFees);
			if(isInsertRequired) {
				listProceduresForAppt.AddRange(TryAddPerVisitProcCodesToAppt(appointment,ApptStatus.None));
			}
			else {
				listProceduresForAppt.AddRange(TryAddPerVisitProcCodesToAppt(appointment,appointmentOld.AptStatus));
			}
			if(!isNew && appointment.Confirmed!=appointmentOld.Confirmed) {
				//Log confirmation status changes.
				SecurityLogs.MakeLogEntry(EnumPermType.ApptConfirmStatusEdit,patient.PatNum,"Appointment confirmation status changed from"+" "
					+Defs.GetName(DefCat.ApptConfirmed,appointmentOld.Confirmed)+" to "+Defs.GetName(DefCat.ApptConfirmed,appointment.Confirmed)
					+" from the appointment edit window.",appointment.AptNum,dateTPrevious);
			}
			bool isCreateAppt=false;
			bool isAptCreatedOrEdited=false;
			if(isNew) {
				if(appointment.AptStatus==ApptStatus.UnschedList && appointment.AptDateTime==DateTime.MinValue) { //If new appt is being added directly to pinboard
					//Do nothing.  Log will be created when appointment is dragged off the pinboard.
				}
				else {
					if(createSecLog) {
						SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCreate,patient.PatNum,
							appointment.AptDateTime.ToString()+", "+appointment.ProcDescript,
							appointment.AptNum,dateTPrevious);
					}
					isAptCreatedOrEdited=true;
					isCreateAppt=true;
				}
			}
			else {
				string logEntryMessage="";
				if(appointment.AptStatus==ApptStatus.Complete) {
					string newCarrierName1=InsPlans.GetCarrierName(appointment.InsPlan1,listInsPlans);
					string newCarrierName2=InsPlans.GetCarrierName(appointment.InsPlan2,listInsPlans);
					string oldCarrierName1=InsPlans.GetCarrierName(appointmentOld.InsPlan1,listInsPlans);
					string oldCarrierName2=InsPlans.GetCarrierName(appointmentOld.InsPlan2,listInsPlans);
					if(appointmentOld.InsPlan1!=appointment.InsPlan1) {
						if(appointment.InsPlan1==0) {
							logEntryMessage+="\r\nRemoved "+oldCarrierName1+" for InsPlan1";
						}
						else if(appointmentOld.InsPlan1==0) {
							logEntryMessage+="\r\nAdded "+newCarrierName1+" for InsPlan1";
						}
						else {
							logEntryMessage+="\r\nChanged "+oldCarrierName1+" to "+newCarrierName1+" for InsPlan1";
						}
					}
					if(appointmentOld.InsPlan2!=appointment.InsPlan2) {
						if(appointment.InsPlan2==0) {
							logEntryMessage+="\r\nRemoved "+oldCarrierName2+" for InsPlan2";
						}
						else if(appointmentOld.InsPlan2==0) {
							logEntryMessage+="\r\nAdded "+newCarrierName2+" for InsPlan2";
						}
						else {
							logEntryMessage+="\r\nChanged "+oldCarrierName2+" to "+newCarrierName2+" for InsPlan2";
						}
					}
				}
				if(createSecLog) {
					if(appointmentOld.AptStatus==ApptStatus.Complete) {//seperate log entry for completed appointments
						SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCompleteEdit,patient.PatNum,
							appointment.AptDateTime.ToShortDateString()+", "+appointment.ProcDescript+logEntryMessage,appointment.AptNum,dateTPrevious);
					}
					else {
						string logText=appointment.AptDateTime.ToShortDateString()+", "+appointment.ProcDescript;
						if(appointment.AptStatus==ApptStatus.Complete) {
							logText+=", Set Complete";//Podium expects this exact text in the security log when the appointment was set complete.
						}
						logText+=logEntryMessage;
						SecurityLogs.MakeLogEntry(EnumPermType.AppointmentEdit,patient.PatNum,logText,appointment.AptNum,dateTPrevious);
					}
				}
				isAptCreatedOrEdited=true;
			}
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(isAptCreatedOrEdited && insertHL7 && HL7Defs.IsExistingHL7Enabled()) {
				//S14 - Appt Modification event, S12 - New Appt Booking event
				MessageHL7 messageHL7=null;
				if(isCreateAppt) {
					messageHL7=MessageConstructor.GenerateSIU(patient,family.GetPatient(patient.Guarantor),EventTypeHL7.S12,appointment);
				}
				else {
					messageHL7=MessageConstructor.GenerateSIU(patient,family.GetPatient(patient.Guarantor),EventTypeHL7.S14,appointment);
				}
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=appointment.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=patient.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						System.Windows.Forms.MessageBox.Show(messageHL7.ToString());
					}
				}
			}
			if(isAptCreatedOrEdited && insertHL7 && HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(patient.PatNum));
			}
			apptSaveHelperResult.ListProcsForAppt=listProceduresForAppt;
			apptSaveHelperResult.AptCur=appointment;
			apptSaveHelperResult.ListAppts=listAppointments;
			return apptSaveHelperResult;
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
				Patient patient=Patients.GetPat(PIn.Long(table.Rows[0]["PatNum"].ToString()));
				Procedures.SetDateFirstVisit(DateTime.MinValue,3,patient);
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
			Appointment appointment=Crud.AppointmentCrud.SelectOne(command);
			HistAppointments.CreateHistoryEntry(appointment,HistAppointmentAction.Deleted);
			AlertItems.DeleteFor(AlertType.CallbackRequested,new List<long> { aptNum });
			Appointments.ClearFkey(aptNum);//Zero securitylog FKey column for row to be deleted.
			//we will not reset item orders here
			command="DELETE FROM appointment WHERE AptNum = "+POut.Long(aptNum);
			//ApptComms.DeleteForAppt(aptNum);
			Db.NonQ(command);
			if(hasSignal) {
				Signalods.SetInvalidAppt(null,appointment);//pass in the old appointment that we are deleting
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			List<long> listAptNumsPlanned = new List<long>();  //List of AptNums for planned appointments only
			List<long> listAptNumsNotPlanned = new List<long>();  //List of AptNums for all appointments that are not planned
			List<long> listAptNumsAll = new List<long>();  //List of AptNums for all appointments
			foreach(DataRow row in table.Rows) {
				if(row["IsNewPatient"].ToString()=="1") {
					//Potentially improve this to not run one at a time
					Patient patient = Patients.GetPat(PIn.Long(row["PatNum"].ToString()));
					Procedures.SetDateFirstVisit(DateTime.MinValue,3,patient);
				}
				if(row["AptStatus"].ToString()=="6") {//planned
					listAptNumsPlanned.Add(PIn.Long(row["AptNum"].ToString()));
				}
				else {//Everything else
					listAptNumsNotPlanned.Add(PIn.Long(row["AptNum"].ToString()));
				}
				listAptNumsAll.Add(PIn.Long(row["AptNum"].ToString()));
			}
			//procs
			command="UPDATE procedurelog SET ProcDate="+DbHelper.Curdate()
				+" WHERE ProcDate<"+POut.Date(new DateTime(1880,1,1))
				+" AND (AptNum IN("+String.Join(",",listAptNumsAll)+") OR PlannedAptNum IN("+String.Join(",",listAptNumsAll)+"))"
				+" AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP);//Only change procdate for TP procedures
			Db.NonQ(command);
			if(listAptNumsPlanned.Count!=0) {
				command="UPDATE procedurelog SET PlannedAptNum=0 WHERE PlannedAptNum IN("+String.Join(",",listAptNumsPlanned)+")";
				Db.NonQ(command);
			}
			if(listAptNumsNotPlanned.Count!=0) {
				command="UPDATE procedurelog SET AptNum=0 WHERE AptNum IN("+String.Join(",",listAptNumsNotPlanned)+")";
				Db.NonQ(command);
			}
			//labcases
			if(listAptNumsPlanned.Count!=0) {
				command="UPDATE labcase SET PlannedAptNum=0 WHERE PlannedAptNum IN("+String.Join(",",listAptNumsPlanned)+")";
				Db.NonQ(command);
			}
			if(listAptNumsNotPlanned.Count!=0) {
				command="UPDATE labcase SET AptNum=0 WHERE AptNum IN("+String.Join(",",listAptNumsNotPlanned)+")";
				Db.NonQ(command);
			}
			//plannedappt
			command="DELETE FROM plannedappt WHERE AptNum IN("+String.Join(",",listAptNumsAll)+")";
			Db.NonQ(command);
			//if deleting a planned appt, make sure there are no appts with NextAptNum (which should be named PlannedAptNum) pointing to this appt
			if(listAptNumsPlanned.Count!=0 && listAptNumsNotPlanned.Count!=0) {
				command="UPDATE appointment SET NextAptNum=0 WHERE NextAptNum IN("+String.Join(",",listAptNumsNotPlanned)+")";
				Db.NonQ(command);
			}
			//apptfield
			command="DELETE FROM apptfield WHERE AptNum IN("+String.Join(",",listAptNumsAll)+")";
			Db.NonQ(command);
			Appointments.ClearFkey(listAptNumsAll);//Zero securitylog FKey column for row to be deleted.
			//we will not reset item orders here
			//ApptComms.DeleteForAppts(listAllAptNums);
			command="SELECT * FROM appointment WHERE AptNum IN("+String.Join(",",listAptNumsAll)+")";
			List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
			listAppointments.ForEach(x => HistAppointments.CreateHistoryEntry(x,HistAppointmentAction.Deleted));
			AlertItems.DeleteFor(AlertType.CallbackRequested,listAppointments.Select(x => x.AptNum).ToList());
			command="DELETE FROM appointment WHERE AptNum IN("+String.Join(",",listAptNumsAll)+")";
			Db.NonQ(command);
			Signalods.SetInvalid(InvalidType.Appointment);
		}
		#endregion

		#region Sync Methods
		///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
		///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new Apptcomm items.
		///Returns true if a change was made, otherwise false.</summary>
		public static bool Sync(List<Appointment> listAppointmentsNew,List<Appointment> listAppointmentsOld,long patNum,long userNum=0,bool isOpMerge=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listAppointmentsNew,listAppointmentsOld,patNum,userNum,isOpMerge);
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			userNum=Security.CurUser.UserNum;
			bool isChanged=Crud.AppointmentCrud.Sync(listAppointmentsNew,listAppointmentsOld,userNum);
			bool isHashNeeded=true;
			for(int i = 0;i<listAppointmentsNew.Count;i++) {
				isHashNeeded=true;
				//Only rehash existing splits that are already valid
				Appointment appointmentOld=listAppointmentsOld.FirstOrDefault(x => listAppointmentsNew[i].AptNum==x.AptNum);
				if(appointmentOld!=null) {
					isHashNeeded=IsAppointmentHashValid(appointmentOld);
				}
				//Hash appointments that are either new, or rehash existing appointments that are valid
				if(isHashNeeded) {
					listAppointmentsNew[i].SecurityHash=HashFields(listAppointmentsNew[i]);
				}
			}
			if(isChanged) {
				if(isOpMerge) { //If this is operatory merge the list could be very long.  Just send a generalized, invalid appt signal, this shouldn't happen often anyway.
					Signalods.SetInvalid(InvalidType.Appointment);
				}
				else {
					foreach(Appointment appt in listAppointmentsNew.Union(listAppointmentsOld).DistinctBy(x => x.AptNum)) { //insert a new signal for each unique appt
						Signalods.SetInvalidAppt(appt);
					}
				}
			}
			return isChanged;
		}

		///<summary>Returns the salted hash for the appointment. Will return an empty string if the calling program is unable to use CDT.dll. </summary>
		private static string HashFields(Appointment appointment) {
			//No need to check MiddleTierRole; no call to db and private method.
			string unhashedText=(int)appointment.AptStatus+appointment.Confirmed.ToString()+appointment.AptDateTime.ToString("yyyy-MM-dd HH:mm:ss");
			try {
				return CDT.Class1.CreateSaltedHash(unhashedText);
			}
			catch(Exception ex)  {
				ex.DoNothing();
				return ex.GetType().Name;
			}
		}

		///<summary>Validates the hash string in appointment.SecurityHash. Returns true if it matches the expected hash, otherwise false. </summary>
		public static bool IsAppointmentHashValid(Appointment appointment) {
			//No need to check MiddleTierRole; no call to db.
			if(appointment==null) {
				return true;
			}
			DateTime dateHashStart=Misc.SecurityHash.GetHashingDate();
			if(appointment.AptDateTime < dateHashStart) { //old
				//See notes in CDT.Class1 on how to pick which column to use for comparison
				return true;
			}
			if(appointment.SecurityHash==HashFields(appointment)) {
				return true;
			}
			return false; 
		}
		#endregion Sync Methods

		#region Misc Methods
		///<summary>Returns the time pattern after combining all codes together for the providers passed in.
		///If make5minute is false, then result will be in 10 or 15 minute blocks and will need a later conversion step before going to db.</summary>
		public static string CalculatePattern(long provNum,long provHyg,List<long> codeNums,bool make5minute) {
			//No need to check MiddleTierRole; no call to db.
			List<ProcedureCode> listProcedureCodes=ProcedureCodes.GetListDeep();
			List<string> listProcPatterns=new List<string>();
			foreach(long codeNum in codeNums) {
				if(ProcedureCodes.GetProcCode(codeNum,listProcedureCodes).IsHygiene) {
					listProcPatterns.Add(ProcCodeNotes.GetTimePattern(provHyg,codeNum));
				}
				else {//dentist proc
					listProcPatterns.Add(ProcCodeNotes.GetTimePattern(provNum,codeNum));
				}
			}
			//Tack all time portions together to make an end result.
			string pattern=GetApptTimePatternFromProcPatterns(listProcPatterns);
			//Creating a new StringBuilder to preserve old hook parameter object Types.
			Plugins.HookAddCode(null,"Appointments.CalculatePattern_end",new StringBuilder(pattern),provNum,provHyg,codeNums);
			if(make5minute) {
				return ConvertPatternTo5(pattern);
			}
			return pattern;
		}

		///<summary>Returns an appointment pattern in a 5 minute increment format.
		///If listProcs is empty or null will return default appt length based on PrefName.AppointmentWithoutProcsDefaultLength or '/' (5 mins) if not defined.</summary>
		public static string CalculatePattern(Patient patient,long provNumApt,long provHygApt,List<Procedure> listProcedures,bool isTimeLocked=false,bool ignoreTimeLocked=false) {
			if(!ignoreTimeLocked && isTimeLocked) {
				return null;
			}
			//We are using the providers selected for the appt rather than the providers for the procs.
			//Providers for the procs get reset when closing this form.
			long provNum=Patients.GetProvNum(patient);
			long provHyg=Patients.GetProvNum(patient);
			if(provNumApt!=0){
				provNum=provNumApt;
				provHyg=provNumApt;
			}
			if(provHygApt!=0) {
				provHyg=provHygApt;
			}
			List<long> listCodeNums=new List<long>();
			foreach(Procedure proc in listProcedures) {
				listCodeNums.Add(proc.CodeNum);
			}
			return CalculatePattern(provNum,provHyg,listCodeNums,listProcedures.Count>0);
			//Plugins.HookAddCode(this,"FormApptEdit.CalculateTime_end",strBTime,provDent,provHyg,codeNums);//set strBTime, but without using the 'new' keyword.--Hook removed.
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching aptNum as FKey and are related to Appointment.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Appointment table type.</summary>
		public static void ClearFkey(long aptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aptNum);
				return;
			}
			Crud.AppointmentCrud.ClearFkey(aptNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching aptNums as FKey and are related to Appointment.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Appointment table type.</summary>
		public static void ClearFkey(List<long> listAptNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAptNums);
				return;
			}
			Crud.AppointmentCrud.ClearFkey(listAptNums);
		}

		///<summary>Returns true if no appointments are scheduled in the time slot for that operatory unless the appointment scheduled is the 
		///appointment under consideration.</summary>
		public static bool IsSlotAvailable(DateTime dateTimeSlotStart,DateTime dateTimeSlotEnd,long operatory,Appointment appointment) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),dateTimeSlotStart,dateTimeSlotEnd,operatory,appointment);
			}
			string command="SELECT COUNT(appointment.AptNum) "
				+"FROM appointment "
				+"WHERE appointment.Op="+POut.Long(operatory)+" "
				+"AND appointment.AptDateTime < "+POut.DateT(dateTimeSlotEnd)+" "
				+"AND appointment.AptDateTime+INTERVAL LENGTH(appointment.Pattern)*5 MINUTE > "+POut.DateT(dateTimeSlotStart)+" "
				+"AND appointment.AptStatus IN("+string.Join(",",Appointments.ListScheduledApptStatuses.Select(x => POut.Int((int)x)))+") ";
			if(appointment.AptNum != 0) {//If we are checking for an already existing appointment, then we don't count this appointment as filling the slot.
				command+="AND appointment.AptNum!="+POut.Long(appointment.AptNum);
			}
			if(Db.GetCount(command)!="0") {
				return false;
			}
			return !Appointments.CheckForBlockoutOverlap(appointment);
		}

		///<summary>The supplied DataRows must include the following columns: attached,Priority,ToothRange,ToothNum,ProcCode. This sorts all objects in Chart module based on their dates, times, priority, and toothnum.  For time comparisons, procs are not included.  But if other types such as comm have a time component in ProcDate, then they will be sorted by time as well.</summary>
		public static int CompareRows(DataRow x,DataRow y) {
			//No need to check MiddleTierRole; no call to db.
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
		public static bool TryAdjustAppointment(Appointment appointment,List<Operatory> listOperatoriesVis,bool canChangeOp,bool canShortenPattern,
			bool hasEndTimeCheck,bool hasBlockoutCheck,out bool isPatternChanged) 
		{
			isPatternChanged=false;
			//First check the Op we are in for overlap.
			if(!TryAdjustAppointment_HasOverlap(appointment,hasEndTimeCheck,hasBlockoutCheck,appointment.Op)) { //No overlap on our original op so we are good.
				return false;
			}
			if(canShortenPattern) { //If we are allowed to shorten the pattern then we will not change ops, we will shorten the pattern down to no less than 1 and return it.
				isPatternChanged=true;
				do {
					if(appointment.Pattern.Length==1) { //Pattern has been reduced to smallest allowed size.
						return TryAdjustAppointment_HasOverlap(appointment,hasEndTimeCheck,hasBlockoutCheck,appointment.Op);
					}
					//Reduce the pattern by 1.
					appointment.Pattern=appointment.Pattern.Substring(0,appointment.Pattern.Length-1);
				} while(TryAdjustAppointment_HasOverlap(appointment,hasEndTimeCheck,hasBlockoutCheck,appointment.Op));
				//If canShortenPattern==true then caller is always expecting false.
				return false;
			}
			if(!canChangeOp) { //We tried our op and we are not allowed to change the op so there is an overlap.
				return true;
			}
			//Our op has an overlap but we are allowed to change so let's try all other ops.
			int startingOp=listOperatoriesVis.Select(x => x.OperatoryNum).ToList().IndexOf(appointment.Op);
			//Left-to-right start at op directly to the right of this one.
			for(int i=startingOp+1;i<listOperatoriesVis.Count;i++) {
				long opNum=listOperatoriesVis[i].OperatoryNum;
				if(!TryAdjustAppointment_HasOverlap(appointment,hasEndTimeCheck,hasBlockoutCheck,appointment.Op)) { //We found an open op. Set it and return.
					appointment.Op=opNum;
					return false;
				}
			}
			//Right-to-left starting at op directly to left of this one.
			for(int i=startingOp;i>=0;i--) {
				long opNum=listOperatoriesVis[i].OperatoryNum;
				if(!TryAdjustAppointment_HasOverlap(appointment,hasEndTimeCheck,hasBlockoutCheck,appointment.Op)) { //We found an open op. Set it and return.
					appointment.Op=opNum;
					return false;
				}
			}
			//We could not find an open op so this AptDateTime overlaps all.
			return true;
		}

		private static bool TryAdjustAppointment_HasOverlap(Appointment appointment,bool hasEndTimeCheck,bool hasBlockoutCheck,long opNum) {
			//Key=OpNum, value=list of appointments in that op
			Dictionary<long,List<Appointment>> dictLocalCache=new Dictionary<long, List<Appointment>>();
			//We may be coming back here several times for the same opNum so store our query results for each opNum for re-use.
			List<Appointment> listAppointments;
			if(!dictLocalCache.TryGetValue(opNum,out listAppointments)) {
				listAppointments=Appointments.GetAppointmentsForOpsByPeriod(new List<long>() { opNum },appointment.AptDateTime,appointment.AptDateTime).FindAll(x => x.AptNum!=appointment.AptNum);
				dictLocalCache[opNum]=listAppointments;
			}
			//Start and end time of the apt we are validating.
			DateTime dateTimeMovedAptStart=appointment.AptDateTime;
			DateTime dateTimeMovedAptEnd=appointment.AptDateTime.Add(TimeSpan.FromMinutes(appointment.Pattern.Length*5));
			//Check for collisions with blockouts if specified, return true if there is one.
			if(hasBlockoutCheck) {
				List<Schedule> listSchedulesBlockoutsOverlapping=GetBlockoutsOverlappingNoSchedule(appointment);
				if(CheckForBlockoutOverlap(appointment,listSchedulesBlockoutsOverlapping)) {
					return true;
				}
			}
			if(PrefC.GetBool(PrefName.ApptsAllowOverlap)) {//Only check for another appointment overlap when the preference is turned off
				return false;
			}
			foreach(Appointment curApt in listAppointments) {
				//Start and end time of another apt in this op.
				DateTime dateTimeAptScheduledStart=curApt.AptDateTime;
				DateTime dateTimeAptScheduledEnd=curApt.AptDateTime.Add(TimeSpan.FromMinutes(curApt.Pattern.Length*5));
				//Check start time.
				if(dateTimeMovedAptStart>=dateTimeAptScheduledStart && dateTimeMovedAptStart<dateTimeAptScheduledEnd){//Starts during curApt's blockout time.
					return true;
				}
				if(!hasEndTimeCheck) { //We only care about start time so move on to the next apt in this op.
					continue;
				}
				if(dateTimeMovedAptEnd<=dateTimeAptScheduledStart) { //moved appt ends before the current one starts
					continue;
				}
				if(dateTimeMovedAptStart>=dateTimeAptScheduledEnd) { //moved appt starts after the current one ends
					continue;
				}
				//The moved appointment completely engulfs the scheduled appointment, return true for a collision.
				return true;
			}
			//No overlap found.
			return false;
		}

		///<summary>Tries to add per visit patient amount and insurance amount procedures to an appointment. Adds procedures if appointment was set complete or scheduled. If an appointment is set complete the added per visit procedures and any procedures associated to the appointment will be set complete. apptStatusOld should be set to ApptStatus.None if appointment is new. Returns empty list of procedures if no procedure was added to appointment. Returns list of per visit Procedures added to appointment.</summary>
		public static List<Procedure> TryAddPerVisitProcCodesToAppt(Appointment appointment,ApptStatus apptStatusOld) {
			//No need to check MiddleTierRole; no call to db.
			List<Procedure> listProcedures=new List<Procedure> ();
			if(appointment==null|| appointment.AptNum==0 || appointment.PatNum==0) {
				return listProcedures;
			}
			if(appointment.AptStatus!=ApptStatus.Scheduled && appointment.AptStatus!=ApptStatus.Complete) {//AptStatus needs to have ApptStatus.Scheduled or ApptStatus.Complete.
				return listProcedures;
			}
			if((apptStatusOld==ApptStatus.Complete && appointment.AptStatus==ApptStatus.Complete)
				|| (apptStatusOld==ApptStatus.Scheduled && appointment.AptStatus==ApptStatus.Scheduled)) {//Appointment was not Scheduled or Set Complete.
				return listProcedures;
			}
			string perVisitPatAmountProcCode=PrefC.GetString(PrefName.PerVisitPatAmountProcCode);
			string perVisitInsAmountProcCode=PrefC.GetString(PrefName.PerVisitInsAmountProcCode);
			if(perVisitPatAmountProcCode.IsNullOrEmpty() && perVisitInsAmountProcCode.IsNullOrEmpty()) {
				return listProcedures;
			}
			Family family=Patients.GetFamily(appointment.PatNum);
			List<PatPlan> listPatPlans=PatPlans.Refresh(appointment.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			InsSub insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
			InsPlan insPlan=InsPlans.GetPlan(insSub.PlanNum,listInsPlans);
			if(insPlan==null) {//Returns empty list if there is no Primary insurance plan.
				return listProcedures;
			}
			if(insPlan.PerVisitInsAmount==0 && insPlan.PerVisitPatAmount==0) {//Returns empty list if there are no per visit amounts set.
				return listProcedures;
			}
			Patient patient=family.GetPatient(appointment.PatNum);
			Procedure procedure=null;
			long codeNumPat=0;
			List<Procedure> listProceduresAppt=Procedures.GetProcsForSingle(appointment.AptNum,false);
			if(insPlan.PerVisitPatAmount>0 && !perVisitPatAmountProcCode.IsNullOrEmpty()) {
				codeNumPat=ProcedureCodes.GetCodeNum(perVisitPatAmountProcCode);
				if(!listProceduresAppt.Any(x => x.CodeNum==codeNumPat && x.AptNum==appointment.AptNum)) {//Check if procedure is already attached to appointment.
					procedure=ConstructPerVisitProcForAppt(codeNumPat,appointment,patient,insPlan.PerVisitPatAmount);//Construct Per visit patient procedure.
					Procedures.Insert(procedure);
					Procedures.ComputeEstimates(procedure,appointment.PatNum,new List<ClaimProc>(),true,listInsPlans,listPatPlans,listBenefits,patient.Age,listInsSubs);
					SecurityLogs.MakeLogEntry(EnumPermType.ProcEdit,procedure.PatNum,perVisitPatAmountProcCode+" "+Lans.g("Appointments","treatment planned via per visit automation."));
					listProcedures.Add(procedure);
				}
			}
			long codeNumIns=0;
			if(insPlan.PerVisitInsAmount>0 && !perVisitInsAmountProcCode.IsNullOrEmpty()) {
				codeNumIns=ProcedureCodes.GetCodeNum(perVisitInsAmountProcCode);
				if(!listProceduresAppt.Any(x => x.CodeNum==codeNumIns && x.AptNum==appointment.AptNum)) {//Check if procedure is already attached to appointment.
					procedure=ConstructPerVisitProcForAppt(codeNumIns,appointment,patient,insPlan.PerVisitInsAmount);//Construct Per visit insurance procedure.
					Procedures.Insert(procedure);
					Procedures.ComputeEstimates(procedure,appointment.PatNum,new List<ClaimProc>(),true,listInsPlans,listPatPlans,listBenefits,patient.Age,listInsSubs);
					SecurityLogs.MakeLogEntry(EnumPermType.ProcEdit,procedure.PatNum,perVisitInsAmountProcCode+" "+Lans.g("Appointments","treatment planned via per visit automation."));
					listProcedures.Add(procedure);
				}
			}
			if(listProcedures.Count==0) {//Returns empty list if no Procedure was created.
				return listProcedures;
			}
			if(appointment.AptStatus==ApptStatus.Complete) {
				//Complete any procedures that are associated with the appointment and do not have the same completed status.
				List<Procedure> listProceduresSetComplete=Procedures.SetCompleteInAppt(appointment,listInsPlans,listPatPlans,patient,listInsSubs,true);
				Procedures.AfterProcsSetComplete(listProceduresSetComplete);
				listProcedures=listProceduresSetComplete.Where(x => (x.CodeNum==codeNumPat || x.CodeNum==codeNumIns) && listProcedures.Any(y => y.CodeNum==x.CodeNum)).ToList();
			}
			List<Appointment> listAppointments=new List<Appointment>();
			listAppointments.Add(appointment);
			UpdateProcDescriptForAppts(listAppointments);
			return listProcedures;
		}

		///<summary>Constructs a per visit procedure from a passed-in codenum and procFee. Does not prompt you to fill in info like toothNum, etc. 
		///Does NOT insert the procedure into the DB, just returns it.</summary>
		public static Procedure ConstructPerVisitProcForAppt(long codeNum,Appointment appointment,Patient patient,Double procFee) {
			//No need to check MiddleTierRole; no call to db.
			Procedure procedure=new Procedure();
			procedure.PatNum=appointment.PatNum;
			procedure.AptNum=appointment.AptNum;
			procedure.CodeNum=codeNum;
			procedure.ProcDate=DateTime.Today;
			procedure.DateTP=DateTime.Today;
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
			procedure.ProcFee=procFee;
			procedure.Priority=0;
			procedure.ProcStatus=ProcStat.TP;
			procedure.ProvNum=appointment.ProvNum;
			if(procedureCode.ProvNumDefault!=0) {//Override provider for procedures with a default provider
				//This provider might be restricted to a different clinic than this user.
				procedure.ProvNum=procedureCode.ProvNumDefault;
			}
			else if(procedureCode.IsHygiene && appointment.ProvHyg!=0) {
				procedure.ProvNum=appointment.ProvHyg;
			}
			procedure.ClinicNum=appointment.ClinicNum;
			procedure.PlaceService=Clinics.GetPlaceService(procedure.ClinicNum);
			procedure.MedicalCode=procedureCode.MedicalCode;
			Procedures.SetDiagnosticCodesToDefault(procedure,procedureCode);
			procedure.RevCode=procedureCode.RevenueCodeDefault;
			procedure.BaseUnits=procedureCode.BaseUnits;
			procedure.SiteNum=patient.SiteNum;
			if(Security.CurUser!=null) {
				procedure.SecUserNumEntry=Security.CurUser.UserNum;
			}
			procedure.Note=ProcCodeNotes.GetNote(procedure.ProvNum,procedure.CodeNum,procedure.ProcStatus);
			if(Userods.IsUserCpoe(Security.CurUser)) {
				//This procedure is considered CPOE because the provider is the one that has added it.
				procedure.IsCpoe=true;
			}
			return procedure;
		}

		//private delegate bool HasOverlap(long opNum,out bool doesOverlapBlockout);

		///<summary>Checks if the appointment passed in overlaps any appointment in the same operatory.
		///Returns true if overlap found.
		///Will modify the appointment.Pattern if canShortenPattern is true.</summary>
		public static bool TryAdjustAppointmentInCurrentOp(Appointment appointment,bool canShortenPattern,bool hasEndTimeCheck,out bool isPatternChanged) 
		{
			return TryAdjustAppointment(appointment,null,false,canShortenPattern,hasEndTimeCheck,true,out isPatternChanged);
		}

		///<summary>Returns true if there is an overlap with a blockout with the the flag "NS", or if apts AppointmentType is associated to different blockouts.</summary>
		public static bool CheckForBlockoutOverlap(Appointment appointment,List<Schedule> listSchedulesBlockouts=null) {
			return GetBlockoutsOverlappingNoSchedule(appointment,listSchedulesBlockouts).Count > 0;
		}

		///<summary>Returns true if there is an overlap with a blockout with the the flag "NS" based on a given time point.
		///If operatoryNum=0 then all operatories will be checked.</summary>
		public static bool CheckTimeForBlockoutOverlap(DateTime aptDateTime, long operatoryNum) {
			return GetBlockoutsOverlappingNoSchedule(new Appointment {
				AptDateTime=aptDateTime,
				Op=operatoryNum,
				Pattern="/",//Pretend this appointment is 5 minutes long since that's the minimum it could be.
			}).Count > 0;
		}

		///<summary>Gets all groups of overlapping appointments.</summary>
		public static List<List<long>> GetOverlappingAppts(DataTable tableAppointments) {
			//No need to check MiddleTierRole; no call to db.
			if(tableAppointments==null || tableAppointments.Rows.Count==0) {
				return new List<List<long>>();
			}
			//Group appointments by operatory
			var groupsApptsPerOp=tableAppointments.Select().Select(x => new Appointment {
				AptNum=PIn.Long(x["aptNum"].ToString()),
				Op=PIn.Long(x["Op"].ToString()),
				AptDateTime=PIn.DateT(x["AptDateTime"].ToString()),
				Pattern=(x["Pattern"].ToString())
			}).GroupBy(x => x.Op);
			//Set the type of the list
			List<Appointment> listAppointmentsOverlap=new List<Appointment>();
			foreach(var opGroup in groupsApptsPerOp) {
				//order grouped appointments by start time. This increases efficiency as we do not need to search through the entire list, only
				//until we find an appoinment outside of the date time.
				List<Appointment> listAppointments=opGroup.OrderBy(x => x.AptDateTime).ToList();
				for(int i=0;i<listAppointments.Count;i++) {
					int j=i+1;
					Appointment appointment=listAppointments[i];
					for(j=i+1;j<listAppointments.Count;j++) {
						Appointment appointmentNext =listAppointments[j];
						if(appointment.EndTime<=appointmentNext.AptDateTime) {
							break;
						}
						if(MiscUtils.DoSlotsOverlap(appointment.AptDateTime,appointment.EndTime,appointmentNext.AptDateTime,appointmentNext.EndTime)) {
							listAppointmentsOverlap.Add(appointment);
							listAppointmentsOverlap.Add(appointmentNext);
						}
					}
				}
			}
			//Unsorted AptNums
			listAppointmentsOverlap=listAppointmentsOverlap.Distinct().OrderBy(x => x.AptDateTime).ToList();
			//Break the AptNums into groups. These should be appointments that are overlapping and chained together. 
			//e.g. 10am-11am, 10:30am-11:30am, and 11am-12pm should all be in one group even though the first and second don't directly overlap
			List<List<long>> listListsAptNumsOverlapGroups=new List<List<long>>();
			//Set Type
			while(listAppointmentsOverlap.Count > 0) {
				List<Appointment> listAppointmentsInGroup=new List<Appointment>{ listAppointmentsOverlap[0] };
				for(int i=1;i<listAppointmentsOverlap.Count;i++) {
					//start at 1 as we already have the first one
					//appointments are still in order by datetime so none will be missed
					if(listAppointmentsInGroup.Any(x => MiscUtils.DoSlotsOverlap(x.AptDateTime,x.EndTime,
						listAppointmentsOverlap[i].AptDateTime,listAppointmentsOverlap[i].EndTime)
						&& x.Op==listAppointmentsOverlap[i].Op)) 
					{
						listAppointmentsInGroup.Add(listAppointmentsOverlap[i]);
					}
				}
				listAppointmentsOverlap=listAppointmentsOverlap.Except(listAppointmentsInGroup).ToList();//remove the ones found
				listListsAptNumsOverlapGroups.Add(listAppointmentsInGroup.Select(x => x.AptNum).ToList());
			}
			return listListsAptNumsOverlapGroups;
		}
		
		///<summary>Returns overlapping blockouts with the the flag "NS", or with a BlockoutType not present in the current apt's AppointmentType.BlockoutTypes.</summary>
		public static List<Schedule> GetBlockoutsOverlappingNoSchedule(Appointment appointment,List<Schedule> listSchedulesBlockouts=null) {
			//No need to check MiddleTierRole; no call to db.
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.BlockoutTypes);
			if(listSchedulesBlockouts is null) { //Get all of today's blockouts that exist in the same operatory as the appointment
				listSchedulesBlockouts=Schedules.GetAllForDateAndType(appointment.AptDateTime,ScheduleType.Blockout,listOpNums:new List<long> { appointment.Op });
			}
			//Get all of today's blockouts that overlap the appointment, and that are of type "NoSchedule"
			List<Schedule> listSchedulesOverlappingBlockouts=listSchedulesBlockouts
				.FindAll(x => MiscUtils.DoSlotsOverlap(x.SchedDate.Add(x.StartTime),x.SchedDate.Add(x.StopTime),appointment.AptDateTime,appointment.AptDateTime.AddMinutes(appointment.Length)));
			List<Schedule> listSchedulesOverlappingBlockoutsNoSchedule=listSchedulesOverlappingBlockouts
				.FindAll(x => Defs.GetDef(DefCat.BlockoutTypes,x.BlockoutType,listDefs).ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()));
			//See if apt has an AppointmentType associated to it. Also see if the AppointmentType is associated to any blockout types in the first place.
			AppointmentType appointmentType=AppointmentTypes.GetOne(appointment.AppointmentTypeNum);
			if(appointmentType!=null && !appointmentType.BlockoutTypes.IsNullOrEmpty()) {
				//Get all BlockoutTypes associated to this AppointmentType
				List<string> listBlockoutTypes=appointmentType.BlockoutTypes.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
				//Convert the BlockoutTypes into a list of longs
				List<long> listDefNumsBlockoutTypeForApptType=listBlockoutTypes.Select(x => PIn.Long(x,hasExceptions:false)).ToList();
				//Add any non-associated BlockoutTypes to our return list.
				listSchedulesOverlappingBlockoutsNoSchedule.AddRange(listSchedulesOverlappingBlockouts
					.FindAll(x => !listDefNumsBlockoutTypeForApptType.Contains(x.BlockoutType)));
			}
			return listSchedulesOverlappingBlockoutsNoSchedule;
		}

		//private static FilterBlockouts(Appointment )
		/// <summary>Called to move existing appointments from the web.
		/// Will only attempt to move to given operatory.</summary>
		public static void TryMoveApptWebHelper(Appointment appointment,DateTime dateTimeApptNew,long opNumNew,LogSources secLogSource=LogSources.MobileWeb, long userNum=0) {
			Appointment appointmentOld=GetOneApt(appointment.AptNum);//Will always exist since you can not move a non inserted appointment.
			Patient patient=Patients.GetPat(appointment.PatNum);
			Operatory operatory=Operatories.GetOperatory(opNumNew);
			List<Schedule> listSchedules=Schedules.ConvertTableToList(Schedules.GetPeriodSchedule(dateTimeApptNew.Date,dateTimeApptNew.Date));
			List<Operatory> listOperatories=new List<Operatory> { operatory };//List of ops to attempt to move appt to, only consider 1.
			bool skipProvChanged=false;//Since we are not skipping validation, let function identify prov change itself.
			bool skipHygChanged=false;//Since we are not skipping validation, let function identify hyg change itself.
			TryMoveAppointment(appointment,appointmentOld,patient,operatory,listSchedules,listOperatories,dateTimeApptNew,
				doValidation:true,setArriveEarly:true,changeProv:true,updatePattern:false,allowFreqConflicts:true,resetConfirmationStatus:true,updatePatStatus:true,
				provChanged:skipProvChanged,hygChanged:skipHygChanged,timeWasMoved:(appointment.AptDateTime!=dateTimeApptNew),isOpChanged:(appointment.Op!=opNumNew),secLogSource:secLogSource, userNum:userNum);
		}
		
		///<summary>Throws exception. Called when moving an appt in the appt module on mouse up after validation and user input.</summary>
		public static void MoveValidatedAppointment(Appointment appointment,Appointment appointmentOld,Patient patient,Operatory operatory,
			List<Schedule> listSchedulesPeriod,List<Operatory> listOperatories,bool isProvChanged,bool isHygChanged,bool isTimeMoved,bool isOpChanged,bool isOpUpdate=false)
		{
			//Skipping validation so all bools that mimic prompt inputs are set to false since they have already ran in ContrApt.MoveAppointments(...)s validation.
			TryMoveAppointment(appointment,appointmentOld,patient,operatory,listSchedulesPeriod,listOperatories,DateTime.MinValue,
				doValidation:false,setArriveEarly:false,changeProv:false,updatePattern:false,allowFreqConflicts:false,resetConfirmationStatus:false,updatePatStatus:false,
				provChanged:isProvChanged,hygChanged:isHygChanged,timeWasMoved:isTimeMoved,isOpChanged:isOpChanged,isOpUpdate:isOpUpdate);
		}

		///<summary>Throws exception. When doSkipValidation is false all bools need to be set and considered.</summary>
		public static void TryMoveAppointment(Appointment appointment,Appointment appointmentOld,Patient patient,Operatory operatory,List<Schedule> listSchedulesPeriod,
			List<Operatory> listOperatories,DateTime aptDateTimeNew,bool doValidation,bool setArriveEarly,bool changeProv,bool updatePattern,
			bool allowFreqConflicts,bool resetConfirmationStatus,bool updatePatStatus,bool provChanged,bool hygChanged,bool timeWasMoved,bool isOpChanged,bool isOpUpdate=false,LogSources secLogSource=LogSources.None, long userNum=0) 
		{
			if(aptDateTimeNew!=DateTime.MinValue) {
				appointment.AptDateTime=aptDateTimeNew;//The time we are attempting to move the appt to.
				timeWasMoved=(appointment.AptDateTime!=appointmentOld.AptDateTime);
			}
			List<Procedure> listProceduresSingleAppt=null;
			if(doValidation) {//ContrAppt.MoveAppointments(...) has identical validation but allows for YesNo input, mimicked here as booleans.
				#region Appointment validation and modifications
				appointment.Op=operatory.OperatoryNum;
				provChanged=false;
				hygChanged=false;
				long provNumDentAssigned=Schedules.GetAssignedProvNumForSpot(listSchedulesPeriod,operatory,false,appointment.AptDateTime);
				long provNumHygAssigned=Schedules.GetAssignedProvNumForSpot(listSchedulesPeriod,operatory,true,appointment.AptDateTime);
				if(appointment.AptStatus!=ApptStatus.PtNote && appointment.AptStatus!=ApptStatus.PtNoteCompleted) {
					if(timeWasMoved) {
						#region Update Appt's DateTimeAskedToArrive
						if(patient.AskToArriveEarly>0) {
							appointment.DateTimeAskedToArrive=appointment.AptDateTime.AddMinutes(-patient.AskToArriveEarly);
						}
						else {
							if(appointment.DateTimeAskedToArrive.Year>1880 && (appointmentOld.AptDateTime-appointmentOld.DateTimeAskedToArrive).TotalMinutes>0) {
								appointment.DateTimeAskedToArrive=appointment.AptDateTime-(appointmentOld.AptDateTime-appointmentOld.DateTimeAskedToArrive);
								if(!setArriveEarly) {
									appointment.DateTimeAskedToArrive=appointmentOld.DateTimeAskedToArrive;
								}
							}
							else {
								appointment.DateTimeAskedToArrive=DateTime.MinValue;
							}
						}
						#endregion Update Appt's DateTimeAskedToArrive
					}
					#region Update Appt's Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
					//if no dentist/hygenist is assigned to spot, then keep the original dentist/hygenist without prompt.  All appts must have prov.
					if((provNumDentAssigned!=0 && provNumDentAssigned!=appointment.ProvNum) || (provNumHygAssigned!=0 && provNumHygAssigned!=appointment.ProvHyg)) {
						if(isOpUpdate || changeProv) {//Short circuit logic.  If we're updating op through right click, never ask.
							if(provNumDentAssigned!=0) {//the dentist will only be changed if the spot has a dentist.
								appointment.ProvNum=provNumDentAssigned;
								provChanged=true;
							}
							if(provNumHygAssigned!=0 || PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly)) {//the hygienist will only be changed if the spot has a hygienist.
								appointment.ProvHyg=provNumHygAssigned;
								hygChanged=true;
							}
							if(operatory.IsHygiene) {
								appointment.IsHygiene=true;
							}
							else {//op not marked as hygiene op
								if(provNumDentAssigned==0) {//no dentist assigned
									if(provNumHygAssigned!=0) {//hyg is assigned (we don't really have to test for this)
										appointment.IsHygiene=true;
									}
								}
								else {//dentist is assigned
									if(provNumHygAssigned==0) {//hyg is not assigned
										appointment.IsHygiene=false;
									}
									//if both dentist and hyg are assigned, it's tricky
									//only explicitly set it if user has a dentist assigned to the op
									if(operatory.ProvDentist!=0) {
										appointment.IsHygiene=false;
									}
								}
							}
							listProceduresSingleAppt=Procedures.GetProcsForSingle(appointment.AptNum,false);
							List<long> listCodeNums=new List<long>();
							for(int p = 0;p<listProceduresSingleAppt.Count;p++) {
								listCodeNums.Add(listProceduresSingleAppt[p].CodeNum);
							}
							if(!isOpUpdate && updatePattern) {
								string calcPattern=Appointments.CalculatePattern(appointment.ProvNum,appointment.ProvHyg,listCodeNums,true);
								if(appointment.Pattern!=calcPattern
									&& Security.IsAuthorized(EnumPermType.AppointmentResize,suppressMessage:true)
									&& !PrefC.GetBool(PrefName.AppointmentTimeIsLocked))//Updating op provs will not change apt lengths.
								{
									appointment.Pattern=calcPattern;
								}
							}
						}
					}
					#endregion Update Appt's ProvNum, ProvHyg, IsHygiene, Pattern
				}
				#region Prevent overlap
				//JS Overlap is no longer prevented when moving from ContrAppt, and this code won't be hit because doValidation is false.
				//It is still prevented with TryMoveApptWebHelper, but only because I'm not overhauling that part of the code right now.
				if(!isOpUpdate && !TryAdjustAppointmentOp(appointment,listOperatories)) {
					throw new ODException(Lans.g("MoveAppointment","Appointment overlaps existing appointment or blockout."));
				}
				#endregion Prevent overlap
				#region Detect Frequency Conflicts
				//Detect frequency conflicts with procedures in the appointment
				if(!isOpUpdate && PrefC.GetBool(PrefName.InsChecksFrequency)) {
					listProceduresSingleAppt=Procedures.GetProcsForSingle(appointment.AptNum,false);
					string frequencyConflicts="";
					try {
						frequencyConflicts=Procedures.CheckFrequency(listProceduresSingleAppt,appointment.PatNum,appointment.AptDateTime);
					}
					catch(Exception e) {
						throw new Exception(Lans.g("MoveAppointment","There was an error checking frequencies."
								+"  Disable the Insurance Frequency Checking feature or try to fix the following error:")
								+"\r\n"+e.Message);
					}
					if(frequencyConflicts!="" && !allowFreqConflicts) {
						return;
					}
				}
				#endregion Detect Frequency Conflicts
				#region Patient status
				if(!isOpUpdate && updatePatStatus) {
					Operatory operatoryNow=Operatories.GetOperatory(appointment.Op);
					Operatory operatoryOld=Operatories.GetOperatory(appointmentOld.Op);
					if(operatoryOld==null || operatoryNow.SetProspective!=operatoryOld.SetProspective) {
						Patient patientOld=patient.Copy();
						if(operatoryNow.SetProspective && patient.PatStatus!=PatientStatus.Prospective) { //Don't need to prompt if patient is already prospective.
							patient.PatStatus=PatientStatus.Prospective;
						}
						else if(!operatoryNow.SetProspective && patient.PatStatus==PatientStatus.Prospective) {
							//Do we need to warn about changing FROM prospective? Assume so for now.
							patient.PatStatus=PatientStatus.Patient;
						}
						if(patient.PatStatus!=patientOld.PatStatus) {
							SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,patient.PatNum,"Patient's status changed from "
								+patientOld.PatStatus.GetDescription()+" to "+patient.PatStatus.GetDescription()+".");
						}
						Patients.Update(patient,patientOld);
					}
				}
				#endregion Patient status
				#region Update Appt's AptStatus, ClinicNum, Confirmed
				if(appointment.AptStatus==ApptStatus.Broken && (timeWasMoved||isOpChanged)) {
					appointment.AptStatus=ApptStatus.Scheduled;
				}
				//original location of provider code
				if(operatory.ClinicNum==0) {
					appointment.ClinicNum=patient.ClinicNum;
				}
				else {
					appointment.ClinicNum=operatory.ClinicNum;
				}
				if(appointment.AptDateTime!=appointmentOld.AptDateTime
					&& appointment.Confirmed!=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum
					&& appointment.AptDateTime.Date!=DateTime.Today
					&& resetConfirmationStatus)
				{
					appointment.Confirmed=Defs.GetFirstForCategory(DefCat.ApptConfirmed,true).DefNum;//Causes the confirmation status to be reset.
				}
				#endregion Update Appt's AptStatus, ClinicNum, Confirmed
				#endregion
				//All validation above is also in ContrAppt.MoveAppointments(...)
			}
			#region Update Appt in db
			try {
				Appointments.Update(appointment,appointmentOld);
			}
			catch(ApplicationException ex) {
				ex.DoNothing();
				throw;
			}
			TryAddPerVisitProcCodesToAppt(appointment,appointmentOld.AptStatus);
			#endregion Update Appt in db
			#region apt.Confirmed securitylog
			if(appointment.Confirmed!=appointmentOld.Confirmed) {
				//Log confirmation status changes.
				SecurityLogs.MakeLogEntry(EnumPermType.ApptConfirmStatusEdit,appointment.PatNum,
					Lans.g("MoveAppointment","Appointment confirmation status changed from")+" "
					+Defs.GetName(DefCat.ApptConfirmed,appointmentOld.Confirmed)+" "+Lans.g("MoveAppointment","to")+" "+Defs.GetName(DefCat.ApptConfirmed,appointment.Confirmed)
					+Lans.g("MoveAppointment","from the appointment module")+".",appointment.AptNum,secLogSource,appointmentOld.DateTStamp);
			}
			#endregion
			#region Set prov in apt
			if(appointment.AptStatus!=ApptStatus.Complete) {
				if(listProceduresSingleAppt==null) {
					listProceduresSingleAppt=Procedures.GetProcsForSingle(appointment.AptNum,false);
				}
				ProcFeeHelper procFeeHelper=new ProcFeeHelper(appointment.PatNum);
				bool isUpdatingFees=false;
				List<Procedure> listProceduresNew=listProceduresSingleAppt.Select(x => Procedures.ChangeProcInAppointment(appointment,x.Copy())).ToList();
				if(listProceduresSingleAppt.Exists(x => x.ProvNum!=listProceduresNew.FirstOrDefault(y => y.ProcNum==x.ProcNum).ProvNum)) {//Either the primary or hygienist changed.
					string promptText="";
					isUpdatingFees=Procedures.ShouldFeesChange(listProceduresNew,listProceduresSingleAppt,ref promptText,procFeeHelper);
					if(isUpdatingFees) {//Made it pass the pref check.
						if(promptText!=""){
								isUpdatingFees=false;
						}
					}
				}
				Procedures.SetProvidersInAppointment(appointment,listProceduresSingleAppt,isUpdatingFees,procFeeHelper);
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
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentEdit,appointment.PatNum,
						Lans.g("MoveAppointment","Appointment on")+" "+appointment.AptDateTime.ToString()+logtext,appointment.AptNum,secLogSource,appointment.DateTStamp, userNum);
				}
			}
			else { 
				if(appointment.AptStatus==ApptStatus.Complete) { //separate log entry for editing completed appointments
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCompleteEdit,appointment.PatNum,
						Lans.g("MoveAppointment","moved")+" "+appointment.ProcDescript+" "+Lans.g("MoveAppointment","from")+" "+appointmentOld.AptDateTime.ToString()+", "+Lans.g("MoveAppointment","to")+" "+appointment.AptDateTime.ToString(),
						appointment.AptNum,secLogSource,appointmentOld.DateTStamp, userNum);
				}
				else {
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentMove,appointment.PatNum,
						appointment.ProcDescript+" "+Lans.g("MoveAppointment","from")+" "+appointmentOld.AptDateTime.ToString()+", "+Lans.g("MoveAppointment","to")+" "+appointment.AptDateTime.ToString(),
						appointment.AptNum,secLogSource,appointmentOld.DateTStamp,userNum);
				}
			}
			#endregion SecurityLog
			#region HL7
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				//S13 - Appt Rescheduling
				MessageHL7 messageHL7=MessageConstructor.GenerateSIU(patient,Patients.GetPat(patient.Guarantor),EventTypeHL7.S13,appointment);
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=appointment.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=patient.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						throw new Exception(messageHL7.ToString());
					}
				}
			}
			#endregion HL7
			#region HieQueue
			if(HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(patient.PatNum));
			}
			#endregion
		}

		///<summary>Creates a scheduled appointment from planned appointment passed in. listApptFields can be null. The fields will just be fetched from the database.
		///Returns the newly scheduled appointment and a boolean indicating whether at least one attached procedure was already attached to another appointment.</summary>
		public static Appointment SchedulePlannedApt(Appointment appointmentPlanned,Patient patient,List<ApptField> listApptFields,DateTime dateTimeAppt,long opNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Appointment>(MethodBase.GetCurrentMethod(),appointmentPlanned,patient,listApptFields,dateTimeAppt,opNum);
			}
			Appointment appointmentNew=appointmentPlanned.Copy();
			appointmentNew.NextAptNum=appointmentPlanned.AptNum;
			appointmentNew.AptStatus=ApptStatus.Scheduled;
			appointmentNew.TimeLocked=appointmentPlanned.TimeLocked;
			if(PrefC.GetBool(PrefName.AppointmentTimeIsLocked)) {
				appointmentNew.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			}
			appointmentNew.AptDateTime=dateTimeAppt;
			appointmentNew.Op=opNum;
			Appointments.Insert(appointmentNew);//now, aptnum is different.
			listApptFields=listApptFields??ApptFields.GetForAppt(appointmentPlanned.AptNum);
			foreach(ApptField apptField in listApptFields) {
				apptField.AptNum=appointmentNew.AptNum;
				ApptFields.Insert(apptField);
			}
			#region HL7
			//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
			if(HL7Defs.IsExistingHL7Enabled()) {
				//S12 - New Appt Booking event
				MessageHL7 messageHL7=MessageConstructor.GenerateSIU(patient,Patients.GetPat(patient.Guarantor),EventTypeHL7.S12,appointmentNew);
				//Will be null if there is no outbound SIU message defined, so do nothing
				if(messageHL7!=null) {
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=appointmentNew.AptNum;
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=patient.PatNum;
					HL7Msgs.Insert(hl7Msg);
					if(ODBuild.IsDebug()) {
						Console.WriteLine(messageHL7.ToString());
					}
				}
			}
			#endregion HL7
			#region HieQueue
			if(HieClinics.IsEnabled()) {
				HieQueues.Insert(new HieQueue(patient.PatNum));
			}
			#endregion
			List<Procedure> listProcedures=Procedures.GetForPlanned(appointmentPlanned.PatNum,appointmentPlanned.AptNum);
			for(int i=0;i<listProcedures.Count;i++) {
				if(listProcedures[i].AptNum == 0) {//not attached to another appt
					Procedures.UpdateAptNum(listProcedures[i].ProcNum,appointmentNew.AptNum);
				}
			}
			TryAddPerVisitProcCodesToAppt(appointmentNew,appointmentPlanned.AptStatus);
			List<LabCase> listLabCases=LabCases.GetForPlanned(appointmentPlanned.AptNum);
			if(!listLabCases.IsNullOrEmpty()) {
				LabCases.AttachToAppt(listLabCases.Select(x => x.LabCaseNum).ToList(),appointmentNew.AptNum);
			}
			return appointmentNew;
		}

		public static bool IsProcAlreadyAttached(Appointment appointmentPlanned){
			List<Procedure> listProcedures=Procedures.GetForPlanned(appointmentPlanned.PatNum,appointmentPlanned.AptNum);
			for(int i=0;i<listProcedures.Count;i++) {
				if(listProcedures[i].AptNum > 0) {//already attached to another appt
					return true;
				}
			}
			return false;
		}

		///<summary>Modifies apt.Op with closest OpNum which has an opening at the specified apt.AptDateTime. 
		///First tries apt.OpNum, then tries remaining ops from left-to-right. Then tries remaining ops from right-to-left.
		///Calling RefreshPeriod() is not necessary before calling this method. It goes to the db only as much as is necessary.
		///Returns true if adjustment was successful or no adjustment was necessary. Returns false if all potential adjustments still caused overlap.</summary>
		public static bool TryAdjustAppointmentOp(Appointment appointment,List<Operatory> listOperatories) {
			bool isNotUsed;
			return !TryAdjustAppointment(appointment,listOperatories,true,false,true,true,out isNotUsed);
		}

		///<summary>Creates a new appointment for the given patient.
		///Accepts null for the patient.  If the patient is null, no patient specific defaults will be set.
		///Set useApptDrawingSettings to true if the user double clicked on the appointment schedule in order to make a new appointment.
		///It will utilize the global static properties to help set required fields for "Scheduled" appointments.
		///Otherwise, simply sets the corresponding PatNum and then the status to "Unscheduled".</summary>
		public static Appointment MakeNewAppointment(Patient patient,DateTime aptDateTime,long opNum,bool useApptDrawingSettings) {
			Appointment appointment=new Appointment();
			if(patient!=null) {
				//Anything referencing PatCur must be in here.
				appointment.PatNum=patient.PatNum;
				if(patient.DateFirstVisit.Year<1880
					&& !Procedures.AreAnyComplete(patient.PatNum))//this only runs if firstVisit blank
				{
					appointment.IsNewPatient=true;
				}
				if(patient.PriProv==0) {
					appointment.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
				}
				else {
					appointment.ProvNum=patient.PriProv;
				}
				appointment.ProvHyg=patient.SecProv;
				appointment.ClinicNum=patient.ClinicNum;
			}
			if(useApptDrawingSettings) {//initially double clicked on appt module
				appointment.AptDateTime=aptDateTime;
				if(patient!=null && patient.AskToArriveEarly>0) {
					appointment.DateTimeAskedToArrive=appointment.AptDateTime.AddMinutes(-patient.AskToArriveEarly);
				}
				appointment.Op=opNum;
				appointment=AssignFieldsForOperatory(appointment);
				appointment.AptStatus=ApptStatus.Scheduled;
			}
			else {
				//new appt will be placed on pinboard instead of specific time
				appointment.AptStatus=ApptStatus.UnschedList;//This is so that if it's on the pinboard when use shuts down OD, no db inconsistency.
			}
			appointment.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			appointment.ColorOverride=System.Drawing.Color.FromArgb(0);
			appointment.Pattern="/X/";
			appointment.SecurityHash=HashFields(appointment);
			return appointment;
		}

		///<summary>Converts the pattern passed in to be 5 minute increments based on the AppointmentTimeIncrement preference.
		///Returns "/" if the pattern passed in is empty.</summary>
		public static string ConvertPatternTo5(string pattern) {
			//No need to check MiddleTierRole; no call to db.
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
		public static Appointment CopyStructure(Appointment appointment) {
			Appointment appointmentNew=new Appointment();
			appointmentNew.PatNum=appointment.PatNum;
			appointmentNew.ProvNum=appointment.ProvNum;
			appointmentNew.Pattern=appointment.Pattern;//Cannot copy length directly.
			appointmentNew.Note=appointment.Note;
			appointmentNew.AptStatus=ApptStatus.UnschedList;//Set to unscheduled. Dragging and dropping this appointment from the pinboard to the operatory will change the status to 'Scheduled'
			appointmentNew.AppointmentTypeNum=appointment.AppointmentTypeNum;
			return appointmentNew;
		}

		///<summary>Takes all time patterns passed in and fuses them into one final time pattern that should be used on appointments.
		///Returns "/" if a null or empty list of patterns is passed in (preserves old behavior).</summary>
		public static string GetApptTimePatternFromProcPatterns(List<string> listProcPatterns) {
			//No need to check MiddleTierRole; no call to db.
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
				string hygTimeStartNow=procPattern.Substring(0,procPattern.Length-procPattern.TrimStart('/').Length);
				//Keep track of the max leading hyg time (/'s)
				if(hygTimeStartNow.Length > hygTimeStart.Length) {
					hygTimeStart=hygTimeStartNow;
				}
				//Trim away the hyg start time and then trim off any /'s on the end and this will be the provider time.
				//Always retain the middle of the procedure time.  E.g. "/XXX///XX///" should retain "XXX///XX" for the provider time portion.
				provTimeTotal+=procPattern.Trim('/');
				//Keep track of the max ending hyg time (/'s) as long as there is at least one prov time (X's) present.
				if(procPattern.Contains('X')) {
					string hygTimeEndNow=procPattern.Substring(procPattern.TrimEnd('/').Length);
					if(hygTimeEndNow.Length > hygTimeEnd.Length) {
						hygTimeEnd=hygTimeEndNow;
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
			//No need to check MiddleTierRole; no call to db.
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT COUNT(*) FROM procedurelog WHERE AptNum="+POut.Long(aptNum);
			if(PIn.Int(Db.GetScalar(command))>0) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if appt has at least 1 completed proc attached.</summary>
		public static bool HasCompletedProcsAttached(long aptNum,List<Procedure> listProceduresAttachToApt=null) {
			if(listProceduresAttachToApt!=null) { 
				return listProceduresAttachToApt.Any(x => x.AptNum==aptNum && x.ProcStatus==ProcStat.C);
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),aptNum,listProceduresAttachToApt);
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
			return DefLinks.GetDefLinksByType(DefLinkType.ClinicSpecialty,defPatSpecialty.DefNum).Any(x => x.FKey==clinicNum);
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),date,apptViewNum);
			}
			//For testing pass a resource image.
			return POut.Bitmap(Properties.Resources.ApptBackTest,ImageFormat.Gif);
		}

		public static DataTable GetPeriodEmployeeSchedTable(DateTime dateStart,DateTime dateEnd) {
			//No need to check MiddleTierRole; no call to db.
			return Schedules.GetPeriodEmployeeSchedTable(dateStart,dateEnd,0);
		}		

		///<summary>Used in Chart module to test whether a procedure is attached to an appointment with today's date. The procedure might have a different date if still TP status.  ApptList should include all appointments for this patient. Does not make a call to db.</summary>
		public static bool ProcIsToday(Appointment[] appointmentArray,Procedure procedure){
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<appointmentArray.Length;i++){
				if(appointmentArray[i].AptDateTime.Date==DateTime.Today
					&& appointmentArray[i].AptNum==procedure.AptNum
					&& (appointmentArray[i].AptStatus==ApptStatus.Scheduled
					|| appointmentArray[i].AptStatus==ApptStatus.Broken
					|| appointmentArray[i].AptStatus==ApptStatus.Complete))
				{
					return true;
				}
			}
			return false;
		}

		public static Appointment TableToObject(DataTable table) {
			//No need to check MiddleTierRole; no call to db.
			if(table.Rows.Count==0) {
				return null;
			}
			return Crud.AppointmentCrud.TableToList(table)[0];
		}

		///<summary>Used by web to insert or update a given appt.
		///Dynamically charts procs based on appt.AppointmentTypeNum when created or changed.
		///This logic is attempting to mimic FormApptEdit when interacting with a new or existing appointment.
		///Set listProcsForApptEditOverride to override Procedures.GetProcsForApptEdit(...) . This should only be done if you already retrieved the list.
		///When listAttachedProcNums is set and not empty, we use this list as the predetermined list of proc nums that should be associated to given appt.</summary>
		public static void UpsertApptFromWeb(Appointment appointment,bool canUpdateApptPattern=false,LogSources secLogSource=LogSources.MobileWeb,
			List<Procedure> listProceduresForApptEditOverride=null, List<long> listProcNumsAttached=null)
		{
			Patient patient=Patients.GetPat(appointment.PatNum);
			List<Procedure> listProceduresForApptEdit=listProceduresForApptEditOverride ?? Procedures.GetProcsForApptEdit(appointment);//List of all procedures that would show in FormApptEdit.cs
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPat(patient.PatNum);
			List<InsSub> listInsSubs=new List<InsSub>();
			List<InsPlan> listInsPlans=new List<InsPlan>();
			if(listPatPlans.Count>0) {
				listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
				listInsPlans=InsPlans.GetByInsSubs(listInsSubs.Select(x => x.InsSubNum).ToList());
			}
			AppointmentType appointmentType=AppointmentTypes.GetOne(appointment.AppointmentTypeNum);//When AppointmentTypeNum=0 this will be null.
			Appointment appointmentOld=GetOneApt(appointment.AptNum);//When inserting a new appt this will be null.
			appointment.IsNew=(appointmentOld==null);
			long appointmentTypeNumOld=(appointmentOld==null ? 0:appointmentOld.AppointmentTypeNum);
			List<Procedure> listProceduresOnAppt;//Subset of listProcsForApptEdit. All procs associated to the given appt. Some aptNums may not be set yet.
			if(appointmentType!=null && appointmentType.AppointmentTypeNum!=appointmentTypeNumOld) {//Appointment type set and changed.
				//Dynamically added procs will exist in listProcsForApptEdit.
				listProceduresOnAppt=ApptTypeMissingProcHelper(appointment,appointmentType,listProceduresForApptEdit,patient,canUpdateApptPattern,listPatPlans,listInsSubs,listInsPlans);
				appointment.ColorOverride=appointmentType.AppointmentTypeColor;
			}
			else {
				listProceduresOnAppt=listProceduresForApptEdit.FindAll(x => x.AptNum!=0 && x.AptNum==appointment.AptNum).Select(x => x.Copy()).ToList();
			}
			if(!listProcNumsAttached.IsNullOrEmpty()) {
				//When listAttachedProcNums is set then we must verify that only the given procnums are going to be associated to this appt.
				List<long> listProcNumsOnAppt = listProceduresOnAppt.Select(x => x.ProcNum).ToList();
				if(listProcNumsAttached.Any(procNum => !listProcNumsOnAppt.Contains(procNum))) {
					//There are procnums that are suppose to be associated to this apt, but are not present in the list of procs for this apt.
					listProceduresForApptEdit.Where(x => listProcNumsAttached.Contains(x.ProcNum) && !listProceduresOnAppt.Any(y => y.ProcNum==x.ProcNum)).ForEach(x => listProceduresOnAppt.Add(x));
				}
				//Remove procs that are not suppose to be attached to appt.
				listProceduresOnAppt.RemoveAll(proc => !listProcNumsAttached.Contains(proc.ProcNum));
			}
			ProcedureLogic.SortProcedures(listProceduresOnAppt);//Mimic FormApptEdit
			if(appointmentOld!=null && appointment.Confirmed!=appointmentOld.Confirmed) {
				if(PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger)==appointment.Confirmed) {
					appointment.DateTimeArrived=DateTime.Now;
				}
				else if(PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger)==appointment.Confirmed) {
					appointment.DateTimeSeated=DateTime.Now;
				}
				else if(PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger)==appointment.Confirmed) {
					appointment.DateTimeDismissed=DateTime.Now;
				}
			}
			#region Appointment insert or update
			#region Appt.ProcDescript
			//Mimics FormApptEdit.UpdateListAndDB(...)
			List<Procedure> listProceduresForDescript=listProceduresOnAppt.Select(x => x.Copy()).ToList();
			foreach(Procedure proc in listProceduresForDescript) {
				//This allows Appointments.SetProcDescript(...) to associate all the passed in procs into AptCur.ProcDescript
				proc.AptNum=appointment.AptNum;
				proc.PlannedAptNum=appointment.AptNum;
			}
			Appointments.SetProcDescript(appointment,listProceduresForDescript);
			#endregion
			List<Procedure> listProcedures=new List<Procedure>();
			if(appointment.IsNew) {
				#region Set Appt fields
				appointment.InsPlan1=(listInsPlans.Count>=1 ? listInsPlans[0].PlanNum:0);
				appointment.InsPlan2=(listInsPlans.Count>=2 ? listInsPlans[1].PlanNum:0);
				appointment.DateTimeArrived=appointment.AptDateTime.Date;
				appointment.DateTimeSeated=appointment.AptDateTime.Date;
				appointment.DateTimeDismissed=appointment.AptDateTime.Date;
				#endregion
				Insert(appointment,appointment.SecUserNumEntry);//Inserts the invalid signal
				listProcedures.AddRange(TryAddPerVisitProcCodesToAppt(appointment,ApptStatus.None));
				listProceduresForApptEdit.AddRange(listProcedures);
				listProceduresOnAppt.AddRange(listProcedures);
				SecurityLogs.MakeLogEntry(new SecurityLog()
				{
					PermType=EnumPermType.AppointmentCreate,
					UserNum=appointment.SecUserNumEntry,
					LogDateTime=DateTime.Now,
					LogText=$"New appointment created from {secLogSource.GetDescription()} by "+Userods.GetUser(appointment.SecUserNumEntry)?.UserName,
					PatNum=appointment.PatNum,
					FKey=appointment.AptNum,
					LogSource=secLogSource,
					DateTPrevious=appointment.SecDateTEntry,
					CompName=Security.GetComplexComputerName()
				});
			}
			else {
				Update(appointment,appointmentOld);//Inserts the invalid signal
				listProcedures.AddRange(TryAddPerVisitProcCodesToAppt(appointment,appointmentOld.AptStatus));
				listProceduresForApptEdit.AddRange(listProcedures);
				listProceduresOnAppt.AddRange(listProcedures);
				SecurityLogs.MakeLogEntry(new SecurityLog()
				{
					PermType=EnumPermType.AppointmentEdit,
					UserNum=appointment.SecUserNumEntry,
					LogDateTime=DateTime.Now,
					LogText=$"Appointment updated from {secLogSource.GetDescription()} by "+Userods.GetUser(appointment.SecUserNumEntry)?.UserName,
					PatNum=appointment.PatNum,
					FKey=appointment.AptNum,
					LogSource= secLogSource,
					DateTPrevious=appointment.SecDateTEntry,
					CompName=Security.GetComplexComputerName(),
				});
			}
			Appointments.SetProcDescript(appointment,listProceduresForDescript);
			#endregion
			#region Mimic FormApptEdit proc selection logic
			//At this point all pertinent procs have been charted or existed as a TPed proc already.
			//The below logic is attempting to mimic how FormApptEdit would make proc selections.
			List<int> listIndicesSelectedProcs=new List<int>();//Equivalent to current proc selections in FormApptEdit.
			List<long> listProcNumsAttachedStart=new List<long>();//Equivalent to OnLoad proc selections in FormApptEdit.
			foreach(Procedure proc in listProceduresOnAppt){
				//All procs in listProcsOnAppt are treated like the user selected them in FormApptEdit.
				listIndicesSelectedProcs.Add(listProceduresForApptEdit.FindIndex(x => x.ProcNum==proc.ProcNum));
				if(!appointment.IsNew && proc.AptNum==appointment.AptNum) {
					//When updating an existing appt some procs might have already been associated to the given appt.
					//Procs that have an AptNum=appt.AptNum were already set prior to this function.
					//This is equivalent to FormApptEdit loading and some procs being pre selected, used to identify attaching and detaching logic in below method calls.
					listProcNumsAttachedStart.Add(proc.ProcNum);
				}
			}
			#endregion
			List<Appointment> listAppointments=Appointments.GetForPat(appointment.PatNum).ToList();
			Procedures.ProcsAptNumHelper(listProceduresForApptEdit,appointment,listAppointments,listIndicesSelectedProcs,listProcNumsAttachedStart,(appointment.AptStatus==ApptStatus.Planned),secLogSource);
			Procedures.UpdateProcsInApptHelper(listProceduresForApptEdit,patient,appointment,appointmentOld,listInsPlans,listInsSubs,listIndicesSelectedProcs,true,false,secLogSource);
			//No need to create an invalid appt signal, the call to either Insert or Update will have already done so.
		}

		///<summary>If this method doesn't throw, then the appointment is considered valid.</summary>
		public static void ValidateApptForWeb(Appointment appointment) {
			bool isPatternChanged;
			bool allowDoubleBooking=PrefC.GetBool(PrefName.ApptsAllowOverlap);
			if(Patients.GetPat(appointment.PatNum)==null) {
				//I don't suggest selecting a patient here because you can only change a patient for new appointments.
				throw new ODException("Patient selected is not valid.");
			}
			if(Providers.GetProv(appointment.ProvNum)==null) {
				throw new ODException("Provider selected is not valid.  Please select a new provider.");
			}
			if(Providers.GetProv(appointment.ProvNum).IsHidden) {
				throw new ODException("Provider selected is marked hidden.  Please select a new provider.");
			}
			if(Appointments.CheckForBlockoutOverlap(appointment) || (!allowDoubleBooking && Appointments.TryAdjustAppointmentInCurrentOp(appointment,false,true,out isPatternChanged))) {
				throw new ODException("Appointment overlaps existing appointment or blockout. Please change the appointment length.");
			}
			//Throws Exception if Warn/Block, returns false if DontEnforce or no conflict.
			try {
				Appointments.HasSpecialtyConflict(appointment.PatNum,appointment.ClinicNum);//"Don't Enforce" does nothing.
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
		public static List<Procedure> ApptTypeMissingProcHelper(Appointment appointment,AppointmentType appointmentType,List<Procedure> listProceduresForAppt,Patient patient=null,bool canUpdateApptPattern=true,
			List<PatPlan> listPatPlans=null,List<InsSub> listInsSubs=null,List<InsPlan> listInsPlans=null,List<Benefit>listBenefits=null)
		{
			List<Procedure> listProcedures=new List<Procedure>();
			if(appointment.AptStatus.In(ApptStatus.PtNote,ApptStatus.PtNoteCompleted)) {
				return listProcedures;//Patient notes can't have procedures associated to them.
			}
			List<ProcedureCode> listProcedureCodesAptType=ProcedureCodes.GetFromCommaDelimitedList(appointmentType.CodeStr);
			if(listProcedureCodesAptType.Count>0) {//AppointmentType is associated to procs.
				if(patient==null) {
					patient=Patients.GetPat(appointment.PatNum);
				}
				if(listPatPlans==null) {
					listPatPlans=PatPlans.GetPatPlansForPat(patient.PatNum);
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
				List<SubstitutionLink> listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);
				long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(patient.PatNum,appointment.AptDateTime); //Use the appointments date
				List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodesAptType,null,null,//no existing procs to pull medCodes and provNums out of
					patient.PriProv,patient.SecProv,patient.FeeSched,listInsPlans,new List<long>(){appointment.ClinicNum },new List<Appointment>(){appointment},listSubstitutionLinks,discountPlanNum);
				//possible (unlikely) issue: if a proc.ProvNumDefault is used, provider might be from different clinic, and a clinic fee override might, therefore, be missing. 
				bool isApptPlanned=(appointment.AptStatus==ApptStatus.Planned);
				List<Procedure> listProceduresNewlyAdded=new List<Procedure>();
				foreach(ProcedureCode procCodeCur in listProcedureCodesAptType) {
					bool existsInAppt=false;
					foreach(Procedure proc in listProceduresForAppt) {
						if(proc.CodeNum==procCodeCur.CodeNum
							//The procedure has not already been added to the return list. 
							&& !listProcedures.Any(x => x.ProcNum==proc.ProcNum)) 
						{
							//appt.AptNum can be 0.
							if(proc.PlannedAptNum!=0 && proc.PlannedAptNum!=appointment.AptNum) {//procedure is attached to planned appointment
								continue;
							}
							if((isApptPlanned && proc.AptNum==0 && (proc.PlannedAptNum==0 || proc.PlannedAptNum == appointment.AptNum))
								|| (!isApptPlanned && (proc.AptNum==0 || proc.AptNum == appointment.AptNum)))
							{
								listProcedures.Add(proc.Copy());
								existsInAppt=true;
								break;
							}
						}
					}
					if(!existsInAppt) { //if the procedure doesn't already exist in the appointment
						Procedure procedure=Procedures.ConstructProcedureForAppt(procCodeCur.CodeNum,appointment,patient,listPatPlans,listInsPlans,listInsSubs,listFees);
						Procedures.Insert(procedure);
						List<ClaimProc> listClaimProcs=new List<ClaimProc>();
						Procedures.ComputeEstimates(procedure,patient.PatNum,ref listClaimProcs,true,listInsPlans,listPatPlans,listBenefits,
							null,null,true,
							patient.Age,listInsSubs,null,false,false,listSubstitutionLinks,false,listFees);
						listProcedures.Add(procedure.Copy());
						listProceduresNewlyAdded.Add(procedure);
					}
				}
				listProceduresForAppt.AddRange(listProceduresNewlyAdded);
				if(!isApptPlanned && listProcedureCodesAptType.Count > 0) {
					Procedures.SetDateFirstVisit(appointment.AptDateTime.Date,1,patient);
				}
			}
			if(canUpdateApptPattern && appointmentType.Pattern!=null && appointmentType.Pattern!="") {
				appointment.Pattern=appointmentType.Pattern;
			}
			return listProcedures;
		}

		///<summary>Sends verification texts and e-mails after a patient confirms any type of appointment via WebSched.</summary>
		public static void SendWebSchedNotify(Appointment appointment,PrefName prefNameType,PrefName prefNameText,PrefName prefNameEmailSubj,PrefName prefNameEmailBody
			,PrefName prefNameEmailType,bool logErrors=true) 
		{
			try {
				Patient patient=Patients.GetPat(appointment.PatNum);
				Clinic clinic=Clinics.GetClinic(appointment.ClinicNum);
				WebSchedVerifyType webSchedVerifyType=(WebSchedVerifyType)PIn.Int(ClinicPrefs.GetPrefValue(prefNameType,appointment.ClinicNum));
				if(webSchedVerifyType==WebSchedVerifyType.None) {
					return;
				}
				CommOptOut commOptOut=CommOptOuts.Refresh(patient.PatNum);
				//Load in the templates and insert replacement fields
				string textTemplate=ClinicPrefs.GetPrefValue(prefNameText,appointment.ClinicNum);
				textTemplate=Patients.ReplacePatient(textTemplate,patient);
				textTemplate=Appointments.ReplaceAppointment(textTemplate,appointment);
				textTemplate=Clinics.ReplaceOffice(textTemplate,clinic);
				string emailSubj=OpenDentBusiness.EmailMessages.SubjectTidy(ClinicPrefs.GetPrefValue(prefNameEmailSubj,appointment.ClinicNum));
				emailSubj=Patients.ReplacePatient(emailSubj,patient);
				emailSubj=Appointments.ReplaceAppointment(emailSubj,appointment);
				emailSubj=Clinics.ReplaceOffice(emailSubj,clinic);
				string emailBody=OpenDentBusiness.EmailMessages.BodyTidy(ClinicPrefs.GetPrefValue(prefNameEmailBody,appointment.ClinicNum));
				emailBody=Patients.ReplacePatient(emailBody,patient,isHtmlEmail:true);
				emailBody=Appointments.ReplaceAppointment(emailBody,appointment,isHtmlEmail:true);
				emailBody=Clinics.ReplaceOffice(emailBody,clinic,isHtmlEmail:true,doReplaceDisclaimer:true);
				//send text
				if(webSchedVerifyType==WebSchedVerifyType.Text || webSchedVerifyType==WebSchedVerifyType.TextAndEmail) {					
					try {
						if(commOptOut.IsOptedOut(CommOptOutMode.Text,CommOptOutType.Verify)) {
							throw new ODException("Patient has opted out of text automated messaging.");
						}
						else {
							SmsToMobiles.SendSmsSingle(patient.PatNum,patient.WirelessPhone,textTemplate,appointment.ClinicNum,SmsMessageSource.Verify,true,canCheckBal:false);
						}
					}
					catch(ODException odex) {
						if(webSchedVerifyType==WebSchedVerifyType.TextAndEmail && logErrors) {
							//SMS failed, so log, but continue so that we also try to send the email.
							Logger.WriteException(odex,"SendFollowUpErrors");
						}
						else if(webSchedVerifyType==WebSchedVerifyType.Text) {
							throw odex;
						}
					}
				}
				//send e-mail
				if(webSchedVerifyType==WebSchedVerifyType.Email || webSchedVerifyType==WebSchedVerifyType.TextAndEmail) {
					EmailAddress emailAddress=EmailAddresses.GetByClinic(appointment.ClinicNum,true);
					if(emailAddress==null) { //If clinic is not setup for email then don't bother trying to send.
						return;
					}
					if(commOptOut.IsOptedOut(CommOptOutMode.Email,CommOptOutType.Verify)) {
						throw new ODException("Patient has opted out of email automated messaging.");
					}
					emailAddress=EmailAddresses.OverrideSenderAddressClinical(emailAddress,patient.ClinicNum); //Use clinic's Email Sender Address Override, if present
					EmailMessage emailMessage=new EmailMessage()
					{
						PatNum=patient.PatNum,
						ToAddress=patient.Email,
						FromAddress=emailAddress.GetFrom(),
						Subject=emailSubj,
						BodyText=emailBody,
						HtmlType=PIn.Enum<EmailType>(ClinicPrefs.GetPrefValue(prefNameEmailType,appointment.ClinicNum)),
						MsgDateTime=DateTime_.Now,
						SentOrReceived=EmailSentOrReceived.Sent,
						MsgType=EmailMessageSource.Verification
					};
					EmailMessages.PrepHtmlEmail(emailMessage);
					EmailMessages.SendEmail(emailMessage,emailAddress);
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
		public static string ReplaceAppointment(string message,Appointment appointment,bool isHtmlEmail=false) {
			StringBuilder template=new StringBuilder(message);
			if(appointment==null) {
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
			ReplaceTags.ReplaceOneTag(template,"[ApptDate]",appointment.AptDateTime.ToString(PrefC.PatientCommunicationDateFormat),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[date]",appointment.AptDateTime.ToString(PrefC.PatientCommunicationDateFormat),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ApptTime]",appointment.AptDateTime.ToShortTimeString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[time]",appointment.AptDateTime.ToShortTimeString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ApptDayOfWeek]",appointment.AptDateTime.DayOfWeek.ToString(),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ProvName]",Providers.GetFormalName(appointment.ProvNum),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ProvAbbr]",Providers.GetAbbr(appointment.ProvNum),isHtmlEmail);
			ReplaceTags.ReplaceOneTag(template,"[ApptTimeAskedArrive]",
				appointment.DateTimeAskedToArrive.Year>1880?appointment.DateTimeAskedToArrive.ToShortTimeString():appointment.AptDateTime.ToShortTimeString(),
				isHtmlEmail);
			if(message.Contains("[ApptProcsList]")) {
				bool isPlanned=false;
				if(appointment.AptStatus==ApptStatus.Planned) {
					isPlanned=true;
				}
				List<Procedure> listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,isPlanned);
				List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
				ProcedureCode procedureCode=new ProcedureCode();
				StringBuilder strProcs=new StringBuilder();
				string procDescript="";
				for(int i=0;i<listProcedures.Count;i++) {
					procedureCode=ProcedureCodes.GetProcCode(listProcedures[i].CodeNum);
					if(procedureCode.LaymanTerm=="") {
						procDescript=procedureCode.Descript;
					}
					else {
						procDescript=procedureCode.LaymanTerm;
					}
					if(i>0) {
						strProcs.Append("\n");
					}
					strProcs.Append(listProcedures[i].ProcDate.ToShortDateString()+" "+procedureCode.ProcCode+" "+procDescript);
				}
				ReplaceTags.ReplaceOneTag(template,"[ApptProcsList]",strProcs.ToString(),isHtmlEmail);
			}
			return template.ToString();
		}

		///<summary>Assigns the ProvNum, ProvHyg, IsHygiene, and ClinicNum to the appointment based on the appointment's operatory.
		///Returns the updated appointment.</summary>
		public static Appointment AssignFieldsForOperatory(Appointment appointmentNow) {
			Appointment appointment=appointmentNow.Copy();
			Operatory operatory=Operatories.GetOperatory(appointment.Op);
			List<Schedule> listSchedulesPeriod=Schedules.RefreshDayEdit(appointment.AptDateTime);
			long provNumDentAssigned=Schedules.GetAssignedProvNumForSpot(listSchedulesPeriod,operatory,false,appointment.AptDateTime);
			long provNumHygAssigned=Schedules.GetAssignedProvNumForSpot(listSchedulesPeriod,operatory,true,appointment.AptDateTime);
			//the section below regarding providers is overly wordy because it's copied from ContrAppt.pinBoard_MouseUp to make maint easier.
			if(provNumDentAssigned!=0) {
				appointment.ProvNum=provNumDentAssigned;
			}
			if(provNumHygAssigned!=0 || PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly)) {
				appointment.ProvHyg=provNumHygAssigned;
			}
			if(operatory.IsHygiene) {
				appointment.IsHygiene=true;
			}
			else {//op not marked as hygiene op
				if(provNumDentAssigned==0) {//no dentist assigned
					if(provNumHygAssigned!=0) {//hyg is assigned (we don't really have to test for this)
						appointment.IsHygiene=true;
					}
				}
				else {//dentist is assigned
					if(provNumHygAssigned==0) {//hyg is not assigned
						appointment.IsHygiene=false;
					}
					//if both dentist and hyg are assigned, it's tricky
					//only explicitly set it if user has a dentist assigned to the op
					if(operatory.ProvDentist!=0) {
						appointment.IsHygiene=false;
					}
				}
			}
			if(operatory.ClinicNum!=0) {
				appointment.ClinicNum=operatory.ClinicNum;
			}
			return appointment;
		}

		///<summary>Used to set an appointment complete when it is right-clicked set complete.  Insert an invalid appointment signalod.</summary>
		public static ODTuple<Appointment,List<Procedure>> CompleteClick(Appointment appointment,List<Procedure> listProceduresForAppt,bool removeCompletedProcs) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ODTuple<Appointment,List<Procedure>>>(MethodBase.GetCurrentMethod(),appointment,listProceduresForAppt,removeCompletedProcs);
			}
			Family family=Patients.GetFamily(appointment.PatNum);
			Patient patient=family.GetPatient(appointment.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(appointment.PatNum);
			DateTime datePrevious=appointment.DateTStamp;
			if(appointment.AptStatus==ApptStatus.PtNote) {
				Appointments.SetAptStatus(appointment,ApptStatus.PtNoteCompleted);//Sets the invalid signal
				SecurityLogs.MakeLogEntry(EnumPermType.AppointmentEdit,appointment.PatNum,
					appointment.AptDateTime.ToString()+", Patient NOTE Set Complete",
					appointment.AptNum,datePrevious);//shouldn't ever happen, but don't allow procedures to be completed from notes
			}
			else {
				InsSub insSub1=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
				InsSub insSub2=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs)),listInsSubs);
				ApptStatus apptStatusOld=appointment.AptStatus;
				Appointments.SetAptStatusComplete(appointment,insSub1.PlanNum,insSub2.PlanNum);//Sets the invalid signal
				TryAddPerVisitProcCodesToAppt(appointment,apptStatusOld);
				Procedures.SetCompleteInAppt(appointment,listInsPlans,listPatPlans,patient,listInsSubs,removeCompletedProcs);//loops through each proc
				if(apptStatusOld==ApptStatus.Complete) { // seperate log entry for editing completed appointments.
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentCompleteEdit,appointment.PatNum,
						appointment.ProcDescript+", "+ appointment.AptDateTime.ToString()+", Set Complete",
						appointment.AptNum,datePrevious);//Log showing the appt. is set complete
				}
				else {
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentEdit,appointment.PatNum,
						appointment.ProcDescript+", "+ appointment.AptDateTime.ToString()+", Set Complete",
						appointment.AptNum,datePrevious);//Log showing the appt. is set complete
				}
				//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					//S14 - Appt Modification event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(patient,family.GetPatient(patient.Guarantor),EventTypeHL7.S14,appointment);
					//Will be null if there is no outbound SIU message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=appointment.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=patient.PatNum;
						HL7Msgs.Insert(hl7Msg);
						if(ODBuild.IsDebug()) {
							Console.WriteLine(messageHL7.ToString());
						}
					}
				}
				if(HieClinics.IsEnabled()) {
					HieQueues.Insert(new HieQueue(patient.PatNum));
				}
			}
			Recalls.SynchScheduledApptFull(appointment.PatNum);
			//No need to enter an invalid signal here, the SetAptStatus and SetAptStatusComplete calls will have already done so
			return new ODTuple<Appointment,List<Procedure>>(appointment,listProceduresForAppt);
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
		public static bool IsRecallAppointment(Appointment appointment,List<Procedure> listProceduresAppt=null) {
			if(appointment==null) {
				return false;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),appointment,listProceduresAppt);
			}
			if(listProceduresAppt==null) {
				listProceduresAppt=Procedures.GetProcsForSingle(appointment.AptNum,appointment.AptStatus==ApptStatus.Planned);//Get this appt's selected procs if not specified.
			}
			if(listProceduresAppt.Count==0) {
				return false;
			}
			List<Recall> listRecalls=Recalls.GetList(appointment.PatNum);//Get the recalls for this patient only.
			string strRecallTypesShowingInList=PrefC.GetString(PrefName.RecallTypesShowingInList);
			if(!string.IsNullOrEmpty(strRecallTypesShowingInList)) {//Limit RecallTypes to check against if RecallTypesShowingInList preference is set.
				List<long> listRecallTypeNums=strRecallTypesShowingInList.Split(new char[] {',' },StringSplitOptions.RemoveEmptyEntries)
					.Select(x => PIn.Long(x)).ToList();
				listRecalls=listRecalls.FindAll(x => listRecallTypeNums.Contains(x.RecallTypeNum));
			}
			foreach(Recall recall in listRecalls) {
				if(recall.IsDisabled) {
					continue;//Skip disabled recalls.
				}
				List<long> listCodeNumsRecallTrigger=RecallTriggers.GetForType(recall.RecallTypeNum).Select(x => x.CodeNum).ToList();
				if(recall.DateScheduled.Date==appointment.AptDateTime.Date && listProceduresAppt.Exists(x => listCodeNumsRecallTrigger.Contains(x.CodeNum))) {
					return true;//Appt is scheduled on the day of the recall, and appt includes a corresponding recall trigger proc.
				}
			}
			return false;
		}

		///<summary>Returns list of appointments going to be empty if the procedures passed in were deleted. Pass in a list of AptNums to explicitly consider. Leave the list of AptNums null to check all appointments associated with the procedures passed in.</summary>
		public static List<Appointment> GetApptsGoingToBeEmpty(List<Procedure> listProcedures,List<long> listApptNums=null,PatientData pd=null,bool isForPlanned=false) {
			//No need to check MiddleTierRole; no call to db
			if(listApptNums.IsNullOrEmpty()) {
				listApptNums=new List<long>();
				listApptNums.AddRange(listProcedures.Select(x => x.PlannedAptNum).ToList().FindAll(x => x>0).Distinct());
				listApptNums.AddRange(listProcedures.Select(x => x.AptNum).ToList().FindAll(x => x>0).Distinct());
			}
			List<Appointment> listAppointments=new List<Appointment>();
			List<Procedure> listProcsAllForAppts;
			if(pd==null) {
				listProcsAllForAppts=Procedures.GetProcsMultApts(listApptNums);
			}
			else {
				pd.FillIfNeeded(EnumPdTable.Procedure);
				listProcsAllForAppts=pd.ListProcedures.FindAll(x => listApptNums.Contains(x.AptNum) || listApptNums.Contains(x.PlannedAptNum));
			}
			for(int i=0;i<listApptNums.Count;i++) {
				int countProcsForAppt=Procedures.GetProcsOneApt(listApptNums[i],listProcedures,isForPlanned).Count;
				int countProcsAllForAppt=Procedures.GetProcsOneApt(listApptNums[i],listProcsAllForAppts,isForPlanned).Count;
				if(countProcsForAppt>=countProcsAllForAppt&&countProcsForAppt!=0) {
					Appointment appointment;
					if(pd==null) {
						appointment=Appointments.GetOneApt(listApptNums[i]);
					}
					else {
						pd.FillIfNeeded(EnumPdTable.Appointment);
						appointment=pd.ListAppointments.Find(x => x.AptNum==listApptNums[i]);
					}
					listAppointments.Add(appointment);
				}
			}
			return listAppointments;
		}

		///<summary>Verifies various appointment procedure states. Calls given funcs as validation from the user is needed.</summary>
		public static bool ProcsAttachedToOtherAptsHelper(List<Procedure> listProceduresInGrid,Appointment appointment, 
			List<long> listProcNumsCurrentlySelected,List<long> listProcNumsOriginallyAttached,Func<List<long>,bool> funcListAptsToDelete,
			Func<bool> funcProcsConcurrentAndNotPlanned,List<Procedure> listProceduresAll,Action actionCompletedProceduresBeingMoved)
		{
			bool isPlanned=appointment.AptStatus == ApptStatus.Planned;
			List<long> listAptNumsToDelete=new List<long>();
			List<Procedure> listProceduresBeingMoved=new List<Procedure>();
			bool hasProcsConcurrent=false;
			for(int i=0;i<listProceduresInGrid.Count;i++) {
				bool isAttaching=listProcNumsCurrentlySelected.Contains(listProceduresInGrid[i].ProcNum);
				bool isAttachedStart=listProcNumsOriginallyAttached.Contains(listProceduresInGrid[i].ProcNum);
				if(!isAttachedStart && isAttaching && isPlanned) {//Attaching to this planned appointment.
					if(listProceduresInGrid[i].PlannedAptNum != 0 && listProceduresInGrid[i].PlannedAptNum != appointment.AptNum) {//However, the procedure is attached to another planned appointment.
						hasProcsConcurrent=true;
						listProceduresBeingMoved.Add(listProceduresInGrid[i]);
					}
				}
				else if(!isAttachedStart && isAttaching && !isPlanned) {//Attaching to this appointment.
					if(listProceduresInGrid[i].AptNum != 0 && listProceduresInGrid[i].AptNum != appointment.AptNum) {//However, the procedure is attached to another appointment.
						hasProcsConcurrent=true;
						listProceduresBeingMoved.Add(listProceduresInGrid[i]);
					}
				}
			}
			if(PrefC.GetBool(PrefName.ApptsRequireProc) && listProceduresBeingMoved.Count>0) {//Only check if we are actually moving procedures.
				//Check to see if the number of procedures we are stealing from the original appointment is the same
				//as the total number of procedures on the appointment. If this is the case the appointment must be deleted.
				//Per the job for this feature we will only delete unscheduled appointments that become empty.
				for(int i=0;i<listProceduresBeingMoved.Count;i++) {
					int countProceduresBeingMoved=listProceduresBeingMoved.Count(x=>x.AptNum==listProceduresBeingMoved[i].AptNum);
					int countProceduresTotal=listProceduresAll.Count(x=>x.AptNum==listProceduresBeingMoved[i].AptNum);
					if(isPlanned) {
						countProceduresBeingMoved=listProceduresBeingMoved.Count(x => x.PlannedAptNum==listProceduresBeingMoved[i].PlannedAptNum);
						countProceduresTotal=listProceduresAll.Count(x => x.PlannedAptNum==listProceduresBeingMoved[i].PlannedAptNum);
					}
					//All the procedures are being moved off appointment, so mark old AptNum for deletion
					if(countProceduresBeingMoved>0 && countProceduresTotal>0 && countProceduresBeingMoved==countProceduresTotal) {
						if(isPlanned) {
							listAptNumsToDelete.Add(listProceduresBeingMoved[i].PlannedAptNum);
						}
						else {
							listAptNumsToDelete.Add(listProceduresBeingMoved[i].AptNum);
						}
					}
				}
			}
			if(listAptNumsToDelete.Count>0) {
				if(!funcListAptsToDelete(listAptNumsToDelete)) {
					return false;
				}
			}
			else if(hasProcsConcurrent && !isPlanned) {
				if(!funcProcsConcurrentAndNotPlanned()) {
					return false;
				}
			}
			bool areCompletedProceduresBeingMoved=false;
			//Refreshing here in case msgboxes above had been up for a while
			List<Appointment> listAppointmentsAll=Appointments.GetAppointmentsForPat(appointment.PatNum);//Refresh AptStatuses
			if(listProceduresBeingMoved!=null) {
				for(int i=0;i<listProceduresBeingMoved.Count;i++) {
					Appointment appointmentFrom=listAppointmentsAll.Find(x=>x.AptNum==listProceduresBeingMoved[i].AptNum);
					if(appointmentFrom!=null && appointmentFrom.AptStatus==ApptStatus.Complete){
						areCompletedProceduresBeingMoved=true;
						break;
					}
				}
			}
			if(areCompletedProceduresBeingMoved) {
				actionCompletedProceduresBeingMoved();
				return false;
			}
			return true;
		}

		///<summary>Helper method that deletes appointments and inserts a security log.
		///This method is called when PrefName.ApptsRequireProc is enabled and a user moves all procs off of appt(s) to another appt.
		///This method does not verify the preference state or the appointment types.</summary>
		public static void DeleteEmptyAppts(List<long> listAptNumsToDelete,long patNum) {
			//We have finished saving this appointment. We can now safely delete the unscheduled appointments marked for deletion.
			if(listAptNumsToDelete.Count > 0) {
				List<Appointment> listAppointmentsToDelete=Appointments.GetMultApts(listAptNumsToDelete); //Get appointments to use procedure and date info.
				//Nathan asked for a specific log entry message explaining why each apt was deleted.
				List<long> listAptNumsToDeleteDistinct=listAptNumsToDelete.Distinct().ToList();
				for(int i=0;i<listAptNumsToDeleteDistinct.Count;i++) {
					Appointment appointment=listAppointmentsToDelete.FirstOrDefault(x=>x.AptNum==listAptNumsToDeleteDistinct[i]);
					if(appointment==null){
						continue;
					}
					string message="All procedures (" + appointment.ProcDescript + ") were moved off of";
					if(appointment.AptStatus==ApptStatus.Planned){
						message+=" a planned appointment";
					}
					else{
						string strAptDate=appointment.AptDateTime.ToShortDateString();
						message+=" the appointment on " + strAptDate;
					}
					message+=", resulting in its deletion.";
					//"All procedures ([proc abbreviations]) were moved off of [a planned appointment|the appointment on [appt date]], resulting in its deletion."
					SecurityLogs.MakeLogEntry(EnumPermType.AppointmentEdit, patNum, message, listAptNumsToDeleteDistinct[i], DateTime.MinValue);
				}
				Delete(listAptNumsToDelete);
			}
		}

		///<summary>Checks if the procedures being deleted are required for the appointment type of an attached appointment. Returns an error message; Otherwise, an empty string.
		///Moved from AppointmentL.cs so that it can be used on the WebApps side.</summary>
		public static string CheckRequiredProcForApptType(List<Procedure> listProceduresToDelete,PatientData pd=null) {
			List<Appointment> listAppointments;//Create a list of appointments to iterate through.
			List<Procedure> listProceduresMultAppt;//Will be needed for Procedures.GetProcsOneApt(...)
			List<AppointmentType> listAppointmentTypes=AppointmentTypes.GetDeepCopy();
			List<long> listApptNums=listProceduresToDelete.Select(x => x.AptNum).Distinct().ToList();
			listApptNums.AddRange(listProceduresToDelete.Select(x => x.PlannedAptNum).Distinct().ToList());
			listApptNums.RemoveAll(x => x<=0);
			if(pd==null) {
				listAppointments=GetMultApts(listApptNums);
				listProceduresMultAppt=Procedures.GetProcsMultApts(listApptNums);
			}
			else {
				pd.FillIfNeeded(EnumPdTable.Appointment,EnumPdTable.Procedure);
				listAppointments=pd.ListAppointments.FindAll(x => listApptNums.Contains(x.AptNum));
				listProceduresMultAppt=pd.ListProcedures.FindAll(x => listApptNums.Contains(x.AptNum) || listApptNums.Contains(x.PlannedAptNum));
			}
			List<string> listAppointmentTypeNames=new List<string>();
			List<string> listRequiredProcs=new List<string>();
			string allOrSome=""; //Will be used for the warning message.
			for(int a=0;a<listAppointments.Count();a++){
				AppointmentType appointmentType=listAppointmentTypes.Find(x => x.AppointmentTypeNum==listAppointments[a].AppointmentTypeNum);
				if(appointmentType==null || appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.None){//If the appt does not have an appttype, or appttype does not need any of the required proc codes to be attached.
					continue;	
				}
				List<long> listProcNumsToDelete=listProceduresToDelete.FindAll(x => x.AptNum==listAppointments[a].AptNum || x.PlannedAptNum==listAppointments[a].AptNum).Select(y => y.ProcNum).ToList();
				List<Procedure> listProceduresForAppt=Procedures.GetProcsOneApt(listAppointments[a].AptNum, listProceduresMultAppt);
				listProceduresForAppt.RemoveAll(x => listProcNumsToDelete.Contains(x.ProcNum));//Remove the procs we intend to delete from the Procedures on Appointment grid to simulate if the procs are successfully deleted.
				List<long> listCodeNumsRemaining=listProceduresForAppt.Select(x => x.CodeNum).ToList();
				List<string> listStrProcCodesRemaining = new List<string>();
				for(int p=0;p<listCodeNumsRemaining.Count();p++){
					listStrProcCodesRemaining.Add(ProcedureCodes.GetProcCode(listCodeNumsRemaining[p]).ProcCode);
				}
				//All procs in grid, minus for other appts, minus selected is simulated result. Verify simulated result meets appointment type requirements and if not then show message.
				bool isMissingRequiredProcs=false;
				int requiredCodesAttached=0;
				List<string> listProcCodesRequiredForApptType=appointmentType.CodeStrRequired.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();//Includes duplicates.
				for(int t=0;t<listProcCodesRequiredForApptType.Count;t++) {
					if(listStrProcCodesRemaining.Contains(listProcCodesRequiredForApptType[t])) {
						requiredCodesAttached++;
						listStrProcCodesRemaining.Remove(listProcCodesRequiredForApptType[t]);
					}
				}
				if(appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.All &&  requiredCodesAttached!=listProcCodesRequiredForApptType.Count) {
					isMissingRequiredProcs=true;
				}
				if(appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.AtLeastOne && requiredCodesAttached==0) {
					isMissingRequiredProcs=true;
				}
				//Gather Appointment Type Name(s) and Required Procedure(s) to use on message.
				if(isMissingRequiredProcs && !listAppointmentTypeNames.Any(x => x==appointmentType.AppointmentTypeName)){
					listAppointmentTypeNames.Add(appointmentType.AppointmentTypeName);
					listRequiredProcs.AddRange(listProcCodesRequiredForApptType);
					//Update allOrSome with the string that corresponds with its EnumRequiredProcCodesNeeded. It will be used on the warning message if there is just one appointment in the list.
					allOrSome=(appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.All)?"all":"at least one";
				}
			}
			if(listAppointmentTypeNames.Count==0){
				return "";
			}
			//The Appointment Type(s) [name(s)] require(s) [at least one | all] of the following procedures to be attached: proc1, proc2.
			if(listAppointmentTypeNames.Count>1){
				allOrSome="all or some"; //Default to this general string if there is more than one appointment in the list as there could be a mix of appointment types.
			}
			string errorMessage=("Appointment Type(s)"+" \""+String.Join("\", \"",listAppointmentTypeNames)+"\" "+"requires "+allOrSome+" of the following procedures:"
				+"\r\n"+String.Join(", ",listRequiredProcs)
				+"\r\n\n"+"To delete these procedures change the Appointment Type to None.");
			return errorMessage;
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

			public ApptBubbleReferralInfo(ReferralType referralType,string name,string phoneNumber) {
				RefType=referralType;
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
		public ApptOther(Appointment appointment) {
			AptNum=appointment.AptNum;
			AptStatus=appointment.AptStatus;
			NextAptNum=appointment.NextAptNum;
			ProvNum=appointment.ProvNum;
			ClinicNum=appointment.ClinicNum;
			AptDateTime=appointment.AptDateTime;
			Pattern=appointment.Pattern;
			ProcDescript=appointment.ProcDescript;
			Note=appointment.Note;
			Op=appointment.Op;
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

	///<summary>For API</summary>
	public class AppointmentWithServerDT {
		public Appointment AppointmentCur;
		public DateTime DateTimeServer;
	}
}
