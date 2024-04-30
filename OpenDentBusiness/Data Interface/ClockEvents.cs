using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class ClockEvents {
		#region Get Methods
		///<summary>Gets clockevents between a date range for a given employee. Datatable return type to allow easy binding in WPF.</summary>
		public static DataTable GetEmployeeClockEventsForDateRange(long employeeNum,DateTime dateTimeStart,DateTime dateTimeStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),employeeNum,dateTimeStart,dateTimeStop);
			}
			string command="SELECT (CASE WHEN ClockStatus=0 THEN 'Home' WHEN ClockStatus=1 THEN 'Lunch' ELSE 'Break' END) AS 'Status',"
				+" TimeEntered1 AS 'In',TimeEntered2 AS 'Out'"
				+" FROM clockevent WHERE EmployeeNum="+POut.Long(employeeNum)+" AND TimeEntered1 BETWEEN "+POut.Date(dateTimeStart)+" AND "+POut.Date(dateTimeStop)
				+" ORDER BY TimeDisplayed1";
			return Db.GetTable(command);
		}

		///<summary>Gets the time a given employee clocked in for a given date. Datatable return type to allow easy binding in WPF.</summary>
		public static DataTable GetTimeClockedInOnDate(long employeeNum,DateTime dateTime){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),employeeNum,dateTime);
			}
			string command="SELECT TimeEntered1, TimeEntered2 FROM clockevent"
				+" WHERE EmployeeNum="+POut.Long(employeeNum)+" AND TimeEntered1 BETWEEN "+POut.Date(dateTime)+" AND "+POut.Date(dateTime.AddDays(1))
				+" GROUP BY EmployeeNum ORDER BY TimeEntered1 ASC";
			return Db.GetTable(command);
		}

		///<summary>Gets a list of ClockEvents that meets a set of criteria for the API.</summary>
		public static List<ClockEvent> GetClockEventsForApi(int limit,int offset,long employeeNum,DateTime dateTStart,DateTime dateTEnd,int clockStatus,long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClockEvent>>(MethodBase.GetCurrentMethod(),limit,offset,employeeNum,dateTStart,dateTEnd,clockStatus,clinicNum);
			}
			string command="SELECT * FROM clockevent "
				+"WHERE TimeDisplayed1 >= "+POut.DateT(dateTStart)+" "
				+"AND TimeDisplayed1 < "+POut.DateT(dateTEnd)+" ";
			if(employeeNum>0) {
				command+="AND EmployeeNum="+POut.Long(employeeNum)+" ";
			}
			if(clockStatus>-1) {
				command+="AND ClockStatus="+POut.Long(clockStatus)+" ";
			}
			if(clinicNum>-1){
				command+="AND ClinicNum="+POut.Long(clinicNum)+" ";
			}
			command+="ORDER BY ClockEventNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.ClockEventCrud.SelectMany(command);
		}
		#endregion

		///<summary></summary>
		public static List<ClockEvent> Refresh(long employeeNum,DateTime dateTimeFrom,DateTime dateTimeTo,bool isBreaks){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClockEvent>>(MethodBase.GetCurrentMethod(),employeeNum,dateTimeFrom,dateTimeTo,isBreaks);
			}
			string command=
				"SELECT * FROM clockevent WHERE"
				+" EmployeeNum = '"+POut.Long(employeeNum)+"'"
				+" AND TimeDisplayed1 >= "+POut.Date(dateTimeFrom)
				//adding a day takes it to midnight of the specified toDate
				+" AND TimeDisplayed1 < "+POut.Date(dateTimeTo.AddDays(1));
			if(isBreaks){
				command+=" AND ClockStatus = '2'";
			}
			else{
				command+=" AND (ClockStatus = '0' OR ClockStatus = '1')";
			}
			command+=" ORDER BY TimeDisplayed1";
			return Crud.ClockEventCrud.SelectMany(command);
		}

		///<summary>Validates list and throws exceptions.  Returns a list of clock events within the date range for employee.</summary>
		///<param name="employeeNum">The primary key of the employee.</param>
		///<param name="dateTimeFrom">The start date of the clock events we are validating for an employee.</param>
		///<param name="dateTimeTo">The end date of the clock events we are validating for an employee.</param>
		///<param name="isBreaks">Indicates whether we are validating break events as opposed to clock in and out events.</param>
		public static List<ClockEvent> GetValidList(long employeeNum,DateTime dateTimeFrom,DateTime dateTimeTo,bool isBreaks) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClockEvent>>(MethodBase.GetCurrentMethod(),employeeNum,dateTimeFrom,dateTimeTo,isBreaks);
			}
			List<ClockEvent> listClockEvents=new List<ClockEvent>();
			string errors="";
			//Fill list-----------------------------------------------------------------------------------------------------------------------------
			string command=
				"SELECT * FROM clockevent WHERE"
				+" EmployeeNum = '"+POut.Long(employeeNum)+"'"
				+" AND TimeDisplayed1 >= "+POut.Date(dateTimeFrom)
				//adding a day takes it to midnight of the specified toDate
				+" AND TimeDisplayed1 < "+POut.Date(dateTimeTo.AddDays(1));
			if(isBreaks) {
				command+=" AND ClockStatus = '2'";
			}
			else {
				command+=" AND (ClockStatus = '0' OR ClockStatus = '1')";
			}
			command+=" ORDER BY TimeDisplayed1";
			listClockEvents=Crud.ClockEventCrud.SelectMany(command);
			//Validate Pay Period------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<listClockEvents.Count;i++) {
				if(listClockEvents[i].TimeDisplayed2.Year < 1880) {
					errors+="  "+listClockEvents[i].TimeDisplayed1.ToShortDateString()+" : ";
					if(isBreaks) {
						errors+="the employee did not clock in from break.\r\n";
					}
					errors+="the employee did not clock out.\r\n";
				}
				else if(listClockEvents[i].TimeDisplayed1.Date!=listClockEvents[i].TimeDisplayed2.Date) {
					errors+="  "+listClockEvents[i].TimeDisplayed1.ToShortDateString()+" : ";
					if(isBreaks) {
						errors+="break spans multiple days.\r\n";
					}
					errors+="entry spans multiple days.\r\n";
				}
			}
			if(errors=="") {
				return listClockEvents;
			}
			if(isBreaks) {
				throw new Exception("Break event errors :\r\n"+errors);
			}
			throw new Exception("Clock event errors :\r\n"+errors);
		}

		///<summary>Returns all clock events (Breaks and Non-Breaks) for all employees across all clinics. Currently only used internally for
		///payroll benefits report.</summary>
		public static List<ClockEvent> GetAllForPeriod(DateTime dateTimeFrom,DateTime dateTimeTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClockEvent>>(MethodBase.GetCurrentMethod(),dateTimeFrom,dateTimeTo);
			}
			string command = "SELECT * FROM clockevent WHERE TimeDisplayed1 >= "+POut.Date(dateTimeFrom)+" AND TimeDisplayed1 < "+POut.Date(dateTimeTo.AddDays(1));
			return Crud.ClockEventCrud.SelectMany(command);
		}

		///<summary>Validates list and throws exceptions.  Returns a list of clock events (not breaks) within the date range for employee.
		///No option for breaks because this is just used in summing for time card report; use GetTimeCardRule instead.</summary>
		public static List<ClockEvent> GetListForTimeCardManage(long employeeNum,long clinicNum,DateTime dateTimeFrom,DateTime dateTimeTo,bool isAll) {
			//No need to check MiddleTierRole; no call to db.
			List<ClockEvent> listClockEvents=GetListForTimeCardManage(new List<long>() { employeeNum },clinicNum,dateTimeFrom,dateTimeTo,isAll);
			string errors=ValidatePayPeriod(listClockEvents);
			if(errors!="") {
				throw new Exception(errors);
			}
			return listClockEvents;
		}

		///<summary>Returns a list of clock events (not breaks) within the date range for the employees.
		///No option for breaks because this is just used in summing for time card report; use GetTimeCardRule instead.</summary>
		public static List<ClockEvent> GetListForTimeCardManage(List<long> listEmployeeNums,long clinicNum,DateTime dateTimeFrom,DateTime dateTimeTo,bool isAll) {
			if(listEmployeeNums.IsNullOrEmpty()) {
				return new List<ClockEvent>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClockEvent>>(MethodBase.GetCurrentMethod(),listEmployeeNums,clinicNum,dateTimeFrom,dateTimeTo,isAll);
			}
			string command="SELECT * FROM clockevent WHERE"
				+" EmployeeNum IN ("+string.Join(",",listEmployeeNums.Select(x => POut.Long(x)))+")"
				+" AND TimeDisplayed1 >= "+POut.Date(dateTimeFrom)
				+" AND TimeDisplayed1 < "+POut.Date(dateTimeTo.AddDays(1));//adding a day takes it to midnight of the specified toDate
				if(!isAll) {
					command+=" AND ClinicNum = '"+POut.Long(clinicNum)+"'";
				}
				command+=" AND (ClockStatus = 0 OR ClockStatus = 1)"
				+" ORDER BY TimeDisplayed1";
			return Crud.ClockEventCrud.SelectMany(command);
		}

		///<summary>Returns an error message containing a description of what is wrong with the clock events passed in if any problems are detected.
		///Otherwise; returns an empty string if no problems are detected.</summary>
		public static string ValidatePayPeriod(List<ClockEvent> listClockEvents) {
			//No need to check MiddleTierRole; no call to db.
			if(listClockEvents.IsNullOrEmpty()) {
				return "";
			}
			StringBuilder stringBuilderErrors=new StringBuilder();
			for(int i=0;i<listClockEvents.Count;i++) { 
				if(listClockEvents[i].TimeDisplayed2.Year<1880) {
					stringBuilderErrors.AppendLine("  "+listClockEvents[i].TimeDisplayed1.ToShortDateString()+" : the employee did not clock out.");
				}
				else if(listClockEvents[i].TimeDisplayed1.Date!=listClockEvents[i].TimeDisplayed2.Date) {
					stringBuilderErrors.AppendLine("  "+listClockEvents[i].TimeDisplayed1.ToShortDateString()+" : entry spans multiple days.");
				}
			}
			if(!string.IsNullOrEmpty(stringBuilderErrors.ToString())) {
				return "Clock event errors :\r\n"+stringBuilderErrors.ToString();
			}
			return "";
		}

		///<summary>Gets one ClockEvent from the db.</summary>
		public static ClockEvent GetOne(long clockEventNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ClockEvent>(MethodBase.GetCurrentMethod(),clockEventNum);
			}
			return Crud.ClockEventCrud.SelectOne(clockEventNum);
		}

		///<summary></summary>
		public static long Insert(ClockEvent clockEvent){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				clockEvent.ClockEventNum=Meth.GetLong(MethodBase.GetCurrentMethod(),clockEvent);
				return clockEvent.ClockEventNum;
			}
			long clockEventNum=0;
			clockEventNum=Crud.ClockEventCrud.Insert(clockEvent);
			if(PrefC.GetBool(PrefName.LocalTimeOverridesServerTime)) {
				//Cannot call update since we manually have to update the TimeEntered1 because it is a DateEntry column
				string command="UPDATE clockevent SET TimeEntered1="+POut.DateT(DateTime.Now)+", TimeDisplayed1="+POut.DateT(DateTime.Now)+" WHERE clockEventNum="+POut.Long(clockEventNum);
				Db.NonQ(command);
			}
			return clockEventNum;
		}

		///<summary></summary>
		public static void Update(ClockEvent clockEvent){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clockEvent);
				return;
			}
			Crud.ClockEventCrud.Update(clockEvent);
		}

		///<summary></summary>
		public static void Delete(long clockEventNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clockEventNum);
				return;
			}
			string command= "DELETE FROM clockevent WHERE ClockEventNum = "+POut.Long(clockEventNum);
			Db.NonQ(command);
		}

		///<summary>Gets directly from the database.  If the last event is a completed break, then it instead grabs the half-finished clock in.
		///Other possibilities include half-finished clock in which truly was the last event, a finished clock in/out,
		///a half-finished clock out for break, or null for a new employee.
		///Returns null if employeeNum of 0 passed in or no clockevent was found for the corresponding employee.</summary>
		public static ClockEvent GetLastEvent(long employeeNum) {
			if(employeeNum==0) {//Every clockevent should be associated to an employee.  Do not waste time looking through the entire table.
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ClockEvent>(MethodBase.GetCurrentMethod(),employeeNum);
			}
			string command="SELECT * FROM clockevent WHERE EmployeeNum="+POut.Long(employeeNum)
				+" ORDER BY TimeDisplayed1 DESC";
			command=DbHelper.LimitOrderBy(command,1);
			ClockEvent clockEvent=Crud.ClockEventCrud.SelectOne(command);
			if(clockEvent==null || clockEvent.ClockStatus!=TimeClockStatus.Break || clockEvent.TimeDisplayed2.Year<=1880) {
				return clockEvent;
			}
			command="SELECT * FROM clockevent WHERE EmployeeNum="+POut.Long(employeeNum)+" "
				+"AND ClockStatus != 2 "//not a break
				+"ORDER BY TimeDisplayed1 DESC";
			command=DbHelper.LimitOrderBy(command,1);
			clockEvent=Crud.ClockEventCrud.SelectOne(command);
			return clockEvent;
		}

		///<summary>From db</summary>
		public static bool IsClockedIn(long employeeNum) {
			//No remoting role check; no call to db
			ClockEvent clockEvent=ClockEvents.GetLastEvent(employeeNum);
			if(clockEvent==null) {//new employee
				return false;
			}
			else if(clockEvent.ClockStatus==TimeClockStatus.Break) {//only incomplete breaks will have been returned.
				//so currently on break
				return false;
			}
			//normal clock in/out row found
			if(clockEvent.TimeDisplayed2.Year<1880) {//already clocked in
				return true;
			}
			//clocked out for home or lunch.
			return false;
		}

		///<summary>Will throw an exception if already clocked in.</summary>
		public static void ClockIn(long employeeNum,bool isAtHome) {
			//No remoting role check; no call to db
			TimeSpan timeSpanMinClockIn=TimeCardRules.GetWhere(x => x.EmployeeNum.In(0,employeeNum) && x.MinClockInTime!=TimeSpan.Zero)
				.OrderBy(x => x.MinClockInTime).FirstOrDefault()?.MinClockInTime??TimeSpan.Zero;
			if(DateTime.Now.TimeOfDay<timeSpanMinClockIn) {
				throw new Exception(Lans.g("ClockEvents","Error. Cannot clock in until")+": "+timeSpanMinClockIn.ToStringHmm());
			}
			//we'll get this again, because it may have been a while and may be out of date
			ClockEvent clockEvent=ClockEvents.GetLastEvent(employeeNum);
			if(clockEvent==null) {//new employee clocking in
				clockEvent=new ClockEvent();
				clockEvent.EmployeeNum=employeeNum;
				clockEvent.ClockStatus=TimeClockStatus.Home;
				clockEvent.ClinicNum=Clinics.ClinicNum;
				clockEvent.IsWorkingHome=isAtHome;
				ClockEvents.Insert(clockEvent);//times handled
			}
			else if(clockEvent.ClockStatus==TimeClockStatus.Break) {//only incomplete breaks will have been returned.
				//clocking back in from break
				if(PrefC.GetBool(PrefName.LocalTimeOverridesServerTime)) {
					clockEvent.TimeEntered2=DateTime.Now;
				}
				else {
					clockEvent.TimeEntered2=MiscData.GetNowDateTime();
				}
				clockEvent.TimeDisplayed2=clockEvent.TimeEntered2;
				ClockEvents.Update(clockEvent);
				if(clockEvent.IsWorkingHome!=isAtHome) { //If coming back from break, and switching locations between home / office
					//This Sleep will ensure that the end of the break is before the end of the clockevent, otherwise it would break calc daily functionality.
					System.Threading.Thread.Sleep(1000); 
					ClockOut(employeeNum,TimeClockStatus.Home); //Home means not lunch or break is being counted.
					//This Sleep will ensure that New clockevent starts a second after the first clockevent ends, otherwise it causes incorrect employee status.
					System.Threading.Thread.Sleep(1000); 
					ClockIn(employeeNum,isAtHome);//Clock in to start new clockEvent from new location.
					return; //ensure we dont make 2 security logs for clocking in by hitting the end of this method twice.
				}
			}
			else {//normal clock in/out
				if(clockEvent.TimeDisplayed2.Year<1880) {//already clocked in
					throw new Exception(Lans.g("ClockEvents","Error.  Already clocked in."));
				}
				//clocked out for home or lunch. Need to clock back in by starting a new row.
				TimeClockStatus timeClockStatus=clockEvent.ClockStatus;
				clockEvent=new ClockEvent();
				clockEvent.EmployeeNum=employeeNum;
				clockEvent.ClockStatus=timeClockStatus;
				clockEvent.ClinicNum=Clinics.ClinicNum;
				clockEvent.IsWorkingHome=isAtHome;
				ClockEvents.Insert(clockEvent);//times handled
			}
			Employee employee=Employees.GetEmp(employeeNum);
			SecurityLogs.MakeLogEntry(EnumPermType.UserLogOnOff,0,employee.FName+" "+employee.LName+" "+"clocked in from "+clockEvent.ClockStatus.ToString()+".");
		}

		///<summary>Will throw an exception if already clocked out.</summary>
		public static void ClockOut(long employeeNum,TimeClockStatus timeClockStatus) {
			//No remoting role check; no call to db
			ClockEvent clockEvent=ClockEvents.GetLastEvent(employeeNum);
			if(clockEvent==null) {//new employee never clocked in
				throw new Exception(Lans.g("ClockEvents","Error.  New employee never clocked in."));
			}
			else if(clockEvent.ClockStatus==TimeClockStatus.Break) {//only incomplete breaks will have been returned.
				throw new Exception(Lans.g("ClockEvents","Error.  Already clocked out for break."));
			}
			//normal clock in/out
			if(clockEvent.TimeDisplayed2.Year>1880) {//clocked out for home or lunch. 
				throw new Exception(Lans.g("ClockEvents","Error.  Already clocked out."));
			}
			//clocked in.
			if(timeClockStatus==TimeClockStatus.Break) {//clocking out on break
				//leave the half-finished event alone and start a new one
				long clinicNum=clockEvent.ClinicNum;
				bool isWorkingHome=clockEvent.IsWorkingHome;
				clockEvent=new ClockEvent();
				clockEvent.EmployeeNum=employeeNum;
				clockEvent.ClockStatus=TimeClockStatus.Break;
				clockEvent.ClinicNum=clinicNum;
				clockEvent.IsWorkingHome=isWorkingHome;
				ClockEvents.Insert(clockEvent);//times handled
			}
			else {//finish the existing event
				if(PrefC.GetBool(PrefName.LocalTimeOverridesServerTime)) {
					clockEvent.TimeEntered2=DateTime.Now;
				}
				else {
					clockEvent.TimeEntered2=MiscData.GetNowDateTime();
				}
				clockEvent.TimeDisplayed2=clockEvent.TimeEntered2;
				clockEvent.ClockStatus=timeClockStatus;//whatever the user selected
				ClockEvents.Update(clockEvent);
				if(PrefC.GetBool(PrefName.DockPhonePanelShow) && clockEvent.ClockStatus==TimeClockStatus.Home) { //only applies to HQ
					ClockOutForHQ(employeeNum);
				}
			}
			Employee employee=Employees.GetEmp(employeeNum);
			SecurityLogs.MakeLogEntry(EnumPermType.UserLogOnOff,0,employee.FName+" "+employee.LName+" "+"clocked out for "+clockEvent.ClockStatus.ToString()+".");
		}

		///<summary>Special logic needs to be run for the phone system when users clock out.</summary>
		private static void ClockOutForHQ(long employeeNum) {
			//No remoting role check; no call to db
			//The name showing for this extension might change to a different user.  
			//It would only need to change if the employee clocking out is not assigned to the current extension. (assigned ext set in the employee table)
			//Get the information corresponding to the employee clocking out.
			PhoneEmpDefault phoneEmpDefaultClockingOut=PhoneEmpDefaults.GetOne(employeeNum);
			if(phoneEmpDefaultClockingOut==null) {
				return;//This should never happen.
			}
			//Get the employee that is normally assigned to this extension (assigned ext set in the employee table).
			long permanentLinkageEmployeeNum=Employees.GetEmpNumAtExtension(phoneEmpDefaultClockingOut.PhoneExt);
			if(permanentLinkageEmployeeNum>=1) { //Extension is nomrally assigned to an employee.
				if(employeeNum!=permanentLinkageEmployeeNum) {//This is not the normally linked employee so let's revert back to the proper employee.
					PhoneEmpDefault phoneEmpDefaultRevertTo=PhoneEmpDefaults.GetOne(permanentLinkageEmployeeNum);
					//Make sure the employee we are about to revert is not logged in at yet a different workstation. This would be rare but it's worth checking.
					if(phoneEmpDefaultRevertTo!=null && !ClockEvents.IsClockedIn(phoneEmpDefaultRevertTo.EmployeeNum)) {
						//Revert to the permanent extension for this PhoneEmpDefault.
						phoneEmpDefaultRevertTo.PhoneExt=phoneEmpDefaultClockingOut.PhoneExt;
						PhoneEmpDefaults.Update(phoneEmpDefaultRevertTo);
						//Update phone table to match this change.
						Phones.SetPhoneStatus(ClockStatusEnum.Home,phoneEmpDefaultRevertTo.PhoneExt,phoneEmpDefaultRevertTo.EmployeeNum);
					}
				}
			}
			//Now let's switch this employee back to his normal extension.
			Employee employeeClockingOut=Employees.GetEmp(employeeNum);
			if(employeeClockingOut==null) {//should not get here
				return;
			}
			//Now check if the assigned extension is associated to a valid phone tile.
			Phone phoneAssignedToEmp=Phones.GetPhoneForExtensionDB(employeeClockingOut.PhoneExt);
			//If the employee is assigned to a valid phone tile (extension) and they are assigned to a different phone tile than the one they just clocked out of.
			if(phoneAssignedToEmp==null || employeeClockingOut.PhoneExt==phoneEmpDefaultClockingOut.PhoneExt) {
				//Update phone table to match this change.
				Phones.SetPhoneStatus(ClockStatusEnum.Home,phoneEmpDefaultClockingOut.PhoneExt,employeeClockingOut.EmployeeNum);
				return;
			}
			//Revert PhoneEmpDefault and Phone to the normally assigned extension for this employee.	
			//Start by setting this employee back to their normally assigned extension.
			phoneEmpDefaultClockingOut.PhoneExt=employeeClockingOut.PhoneExt;
			//Check if someone is currently using their assigned extension
			if(ClockEvents.IsClockedIn(phoneAssignedToEmp.EmployeeNum)) {
				//The third employee is clocked in so set our employee extension to 0.
				//The currently clocked in employee will retain the extension for now.
				//Our employee will retain the proper extension next time they clock in.
				phoneEmpDefaultClockingOut.PhoneExt=0;
				//Update the phone table accordingly.
				Phones.UpdatePhoneToEmpty(phoneEmpDefaultClockingOut.EmployeeNum,-1);
			}
			PhoneEmpDefaults.Update(phoneEmpDefaultClockingOut);
			//Update phone table to match this change.
			Phones.SetPhoneStatus(ClockStatusEnum.Home,phoneEmpDefaultClockingOut.PhoneExt,employeeClockingOut.EmployeeNum);
		}

		///<summary>Used in the timecard to track hours worked per week when the week started in a previous time period.  This gets all the hours of the first week before the date listed.  Also adds in any adjustments for that week.</summary>
		public static TimeSpan GetWeekTotal(long employeeNum,DateTime date) {
			//No need to check MiddleTierRole; no call to db.
			TimeSpan timeSpan=new TimeSpan(0);
			//If the first day of the pay period is the starting date for the overtime, then there is no need to retrieve any times from the previous pay period.
			if(date.DayOfWeek==(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)) {
				return timeSpan;
			}
			//We only want to go back to the most recent "FirstDayOfWeek" week day.
			//The old code would simply go back in time 6 days which would cause problems.
			//The main problem was that hours from previous weeks were being counted towards other pay periods (sometimes).
			//E.g. pay period A = 01/28/2014 - 02/10/2014  pay period B = 02/11/2014 - 02/24/2014
			//The preference TimeCardOvertimeFirstDayOfWeek is set to Tuesday.
			//Employee worked 8 hours on Monday 02/10/2014 (falls within pay period A)
			//The first weekly total in pay period B will include hours worked from Monday 02/10/2014 (pay period A) because Tuesday > Monday (old logic).
			//However, the weekly total should NOT include Monday 02/10/2014 in pay period B's first weekly total because it is in not part of the current "week" based on TimeCardOvertimeFirstDayOfWeek. 
			//Therefore, we need to find out the date of the most recent TimeCardOvertimeFirstDayOfWeek.
			DateTime dateMostRecentFirstDayOfWeek=date;//Start with the current date.
			//Loop backward through the week days until the TimeCardOvertimeFirstDayOfWeek is hit.
			for(int i=1;i<7;i++) {//1 based because we already know that TimeCardOvertimeFirstDayOfWeek is not set to today so no need to check it.
				if(dateMostRecentFirstDayOfWeek.AddDays(-i).DayOfWeek==(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)) {
					dateMostRecentFirstDayOfWeek=dateMostRecentFirstDayOfWeek.AddDays(-i);
					break;
				}
			}
			//mostRecentFirstDayOfWeekDate=date.AddDays(-6);
			List<ClockEvent> listClockEvents=Refresh(employeeNum,dateMostRecentFirstDayOfWeek,date.AddDays(-1),false);
			//eg, if this is Thursday, then we are getting last Friday through this Wed.
			for(int i=0;i<listClockEvents.Count;i++) {
				//This is the old logic of trying to figure out if "events" fall within the most recent week.
				//The new way is correctly getting only the "events" for the most recent week which negates the need for any filtering.
				//if(events[i].TimeDisplayed1.DayOfWeek > date.DayOfWeek){//eg, Friday > Thursday, so ignore
				//	continue;
				//}
				//This scenario happens if the user clocks in and their system clock changes to previous date/time then they clock out.
				//This specific scenario has happened to PatNum 31287
				//This same PatNum also had an employee with multiple clock in events in the same minute (10+) with only one of the events having a clock out.
				//This check would fix that issue as well. 
				//If someone intentionally backdates a clock out event to get negative time, they can use an adjustment instead.
				if(listClockEvents[i].TimeDisplayed2<listClockEvents[i].TimeDisplayed1) {
					continue;
				}
				timeSpan+=listClockEvents[i].TimeDisplayed2-listClockEvents[i].TimeDisplayed1;
				if(listClockEvents[i].AdjustIsOverridden) {
					timeSpan+=listClockEvents[i].Adjust;
				}
				else {
					timeSpan+=listClockEvents[i].AdjustAuto;//typically zero
				}
				//ot
				if(listClockEvents[i].OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
					timeSpan-=listClockEvents[i].OTimeHours;
				}
				else {
					timeSpan-=listClockEvents[i].OTimeAuto;//typically zero
				}
			}
			//now, adjustments
			List<TimeAdjust> listTimeAdjusts=TimeAdjusts.Refresh(employeeNum,dateMostRecentFirstDayOfWeek,date.AddDays(-1));
			for(int i=0;i<listTimeAdjusts.Count;i++) {
				//This is the old logic of trying to figure out if adjustments fall within the most recent week.  
				//The new way is correctly getting only the "TimeAdjustList" for the most recent week which negates the need for any filtering.
				//if(TimeAdjustList[i].TimeEntry.DayOfWeek > date.DayOfWeek) {//eg, Friday > Thursday, so ignore
				//	continue;
				//}
				if(listTimeAdjusts[i].IsUnpaidProtectedLeave) {
					continue;
				}
				timeSpan+=listTimeAdjusts[i].RegHours;
			}
			return timeSpan;
		}

		///<summary>-hh:mm or -hh.mm.ss or -hh.mm, depending on the pref.TimeCardsUseDecimalInsteadOfColon and pref.TimeCardShowSeconds.  Blank if zero.</summary>
		public static string Format(TimeSpan timeSpan) {
			//No remoting role check; no call to db
			if(PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon)){
				if(timeSpan==TimeSpan.Zero){
					return "";
				}
				return timeSpan.TotalHours.ToString("n");
			}
			else if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {//Colon format with seconds
				return timeSpan.ToStringHmmss();
			}
			//Colon format without seconds
			return timeSpan.ToStringHmm();//blank if zero
		}

		///<summary>Avoids some funky behavior from TimeSpan.Parse(). Surround in try/catch.
		///Valid formats: 
		///			hh:mm
		///			hh:mm:ss
		///			hh:mm:ss.fff 
		///TimeSpan.Parse("23:00:00") returns 23 hours.
		///TimeSpan.Parse("25:00:00") returns 25 days.
		///In this method, '25:00:00' is treated as 25 hours.
		///Throws exceptions</summary>
		public static TimeSpan ParseHours(string timeString) {
			//No remoting role check; no call to db
			List<string> listParts=timeString.TrimStart('-').Split(new[] { ':' }).ToList();
			if(listParts.Count>3) {
				//User input more than hours i.e. 00:00:00:00 this only accepts hours.
				throw new Exception("Invalid format");
			}
			if(listParts.Any(x => string.IsNullOrEmpty(x))) {
				//Blank or contains a blank segment
				throw new Exception("Invalid format");
			}
			bool isNegative=timeString.StartsWith("-");
			TimeSpan timeSpan=TimeSpan.Zero;
			//Hours
			if(listParts.Count>=1){
				double hours=0;
				try {
					hours=double.Parse(listParts[0]);
				}
				catch {
					throw new Exception("Invalid format");
				}
				timeSpan+=TimeSpan.FromHours(hours);
			}
			//Minutes
			if(listParts.Count>=2) {
				int minutes=0;
				if(minutes>=60 || minutes<0) {
					throw new Exception("Invalid format");
				}
				try {
					minutes=int.Parse(listParts[1]);
				}
				catch {
					throw new Exception("Invalid format");
				}
				timeSpan+=TimeSpan.FromMinutes(minutes);
			}
			//Seconds
			if(listParts.Count==3) {
				double seconds=0;
				if(seconds>=60 && seconds<0){
					throw new Exception("Invalid format");
				}
				try{
					seconds=double.Parse(listParts[2]);
				}
				catch{
					throw new Exception("Invalid format");
				}
				timeSpan+=TimeSpan.FromSeconds(seconds);
			}
			if(isNegative) {
				timeSpan=-timeSpan;
			}
			return timeSpan;
		}

		///<summary>Returns clockevent information for all non-hidden employees.  Used only in the time card manage window.
		///Set isAll to true to return all employee time cards (used for clinics).</summary>
		public static DataTable GetTimeCardManage(DateTime dateTimeStart,DateTime dateTimeStop,long clinicNum,bool isAll) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateTimeStart,dateTimeStop,clinicNum,isAll);
			}
			//Construct empty table------------------------------------------------------------------------------------------------------------------------
			DataTable table=new DataTable("TimeCardManage");
			table.Columns.Add("PayrollID");
			table.Columns.Add("EmployeeNum");
			table.Columns.Add("firstName");
			table.Columns.Add("lastName");
			table.Columns.Add("totalHours");//should be sum of RegularHours and OvertimeHours (excluding RegularHours from Unpaid Protected Leave Adjustments)
			table.Columns.Add("rate1Hours");
			table.Columns.Add("rate1OTHours");
			table.Columns.Add("rate2Hours");
			table.Columns.Add("rate2OTHours");
			table.Columns.Add("rate3Hours");
			table.Columns.Add("rate3OTHours");
			table.Columns.Add("PTOHours");
			table.Columns.Add("protectedLeaveHours");
			table.Columns.Add("Note");
			//Loop through employees.  Each employee adds one row to table --------------------------------------------------------------------------------
			List<Employee> listEmployees=Employees.GetForTimeCard();//Gets all non-hidden employees
			List<Employee> listEmployeesForClinic=Employees.GetEmpsForClinic(clinicNum);
			List<long> listEmployeeNums=listEmployees.Select(x => x.EmployeeNum).ToList();
			List<ClockEvent> listClockEvents=GetListForTimeCardManage(listEmployeeNums,clinicNum,dateTimeStart,dateTimeStop,isAll);
			List<TimeAdjust> listTimeAdjusts=TimeAdjusts.GetListForTimeCardManage(listEmployeeNums,clinicNum,dateTimeStart,dateTimeStop,isAll);
			//get all pay period notes for all employees for this pay period. 
			List<TimeAdjust> listTimeAdjustsPayPeriodNote=TimeAdjusts.GetNotesForPayPeriod(dateTimeStart);
			for(int i=0;i<listEmployees.Count;i++) {
				string note="";
				DataRow dataRow=table.NewRow();
				dataRow.ItemArray.Initialize();//changes all nulls to blanks and zeros.
				//PayrollID-------------------------------------------------------------------------------------------------------------------------------------
				dataRow["PayrollID"]=listEmployees[i].PayrollID;
				//EmployeeNum and Name----------------------------------------------------------------------------------------------------------------------------------
				dataRow["EmployeeNum"]=listEmployees[i].EmployeeNum;
				dataRow["firstName"]=listEmployees[i].FName;
				dataRow["lastName"]=listEmployees[i].LName;
				//Begin calculations------------------------------------------------------------------------------------------------------------------------------------
				//each list below will contain one entry per week.
				List<TimeSpan> listTimeSpansRegularHoursWeekly=new List<TimeSpan>();//Total non-overtime hours.  Used for calculation, not displayed or part of dataTable.
				List<TimeSpan> listTimeSpansOTHoursWeekly=new List<TimeSpan>();//Total overtime hours.  Used for calculation, not displayed or part of dataTable.
				List<TimeSpan> listTimeSpansRate2HoursWeekly=new List<TimeSpan>();//Not included in total hours worked.  tsRate2Hours is different than r2Hours and r2OTHours
				List<TimeSpan> listTimeSpansRate3HoursWeekly=new List<TimeSpan>();//Not included in total hours worked.  tsRate3Hours is differant than r3Hours and r3OTHours
				List<ClockEvent> listClockEventsEmp=listClockEvents.FindAll(x => x.EmployeeNum==listEmployees[i].EmployeeNum);
				List<TimeAdjust> listTimeAdjustsEmp=listTimeAdjusts.FindAll(x => x.EmployeeNum==listEmployees[i].EmployeeNum);
				//If there are no clock events, nor time adjusts, and the current employee isn't "assigned" to the clinic passed in, skip.
				if(listClockEventsEmp.Count==0 //employee has no clock events for this clinic.
					&& listTimeAdjustsEmp.Count==0 //employee has no time adjusts for this clinic.
					&& (!isAll && listEmployeesForClinic.Count(x => x.EmployeeNum==listEmployees[i].EmployeeNum)==0)) //employee not explicitly assigned to clinic
				{
					continue;
				}
				//report errors in note column and move to next employee.----------------------------------------------------------------
				string employeeErrors=ValidatePayPeriod(listClockEventsEmp);
				if(employeeErrors!="") {
					dataRow["Note"]=employeeErrors.Trim();
					table.Rows.Add(dataRow);
					continue;//display employee errors in note field for employee. All columns will be blank for just this employee.
				}
				//sum values for each week----------------------------------------------------------------------------------------------------
				List<DateTime> listDateTimesWeekStart=WeekStartHelper(dateTimeStart,dateTimeStop);
				for(int j=0;j<listDateTimesWeekStart.Count;j++) {
					listTimeSpansRegularHoursWeekly.Add(TimeSpan.Zero);			
					listTimeSpansOTHoursWeekly.Add(TimeSpan.Zero);
					listTimeSpansRate2HoursWeekly.Add(TimeSpan.Zero);
					listTimeSpansRate3HoursWeekly.Add(TimeSpan.Zero);
				}
				int week=0;
				for(int j=0;j<listClockEventsEmp.Count;j++) {
					//set current week for clock event
					for(int k=0;k<listDateTimesWeekStart.Count;k++) {
						if(listClockEventsEmp[j].TimeDisplayed1<listDateTimesWeekStart[k].AddDays(7)) {
							week=k;//clock event occurs during the week "j"
							break;
						}
					}
					if(j==0) {//we only want the comment from the first clock event entry.
						note=listClockEventsEmp[j].Note;
					}
					//TimeDisplayed-----
					listTimeSpansRegularHoursWeekly[week]+=listClockEventsEmp[j].TimeDisplayed2-listClockEventsEmp[j].TimeDisplayed1;
					//Adjusts-----
					if(listClockEventsEmp[j].AdjustIsOverridden) {
						listTimeSpansRegularHoursWeekly[week]+=listClockEventsEmp[j].Adjust;
					}
					else {
						listTimeSpansRegularHoursWeekly[week]+=listClockEventsEmp[j].AdjustAuto;
					}
					//OverTime-----
					if(listClockEventsEmp[j].OTimeHours!=TimeSpan.FromHours(-1)) {//Manual override
						listTimeSpansOTHoursWeekly[week]+=listClockEventsEmp[j].OTimeHours;
						listTimeSpansRegularHoursWeekly[week]+=-listClockEventsEmp[j].OTimeHours;
					}
					else {
						listTimeSpansOTHoursWeekly[week]+=listClockEventsEmp[j].OTimeAuto;
						listTimeSpansRegularHoursWeekly[week]+=-listClockEventsEmp[j].OTimeAuto;
					}
					//Rate2
					if(listClockEventsEmp[j].Rate2Hours!=TimeSpan.FromHours(-1)) {//Manual override
						listTimeSpansRate2HoursWeekly[week]+=listClockEventsEmp[j].Rate2Hours;
					}
					else {
						listTimeSpansRate2HoursWeekly[week]+=listClockEventsEmp[j].Rate2Auto;
					}
					//Rate3
					if(listClockEventsEmp[j].Rate3Hours!=TimeSpan.FromHours(-1)) {//Manual override
						listTimeSpansRate3HoursWeekly[week]+=listClockEventsEmp[j].Rate3Hours;
					}
					else {
						listTimeSpansRate3HoursWeekly[week]+=listClockEventsEmp[j].Rate3Auto;
					}
				}
				//reset current week to itterate through time adjusts
				week=0;
				for(int j=0;j<listTimeAdjustsEmp.Count;j++) {//list of timeAdjusts have already been filtered.
					if(listTimeAdjustsEmp[j].IsUnpaidProtectedLeave) {
						continue;
					}
					//set current week for time adjusts-----
					for(int k=0;k<listDateTimesWeekStart.Count;k++) {
						if(listTimeAdjustsEmp[j].TimeEntry<listDateTimesWeekStart[k].AddDays(7)) {
							week=k;//clock event occurs during the week "j"
							break;
						}
					}
					listTimeSpansRegularHoursWeekly[week]+=listTimeAdjustsEmp[j].RegHours;
					listTimeSpansOTHoursWeekly[week]+=listTimeAdjustsEmp[j].OTimeHours;
				}
				//Overtime should have already been calculated by CalcWeekly(); No calculations needed, just sum values.------------------------------------------------------
				double totalHoursWorked=0;
				double totalRegularHoursWorked=0;
				double totalOTHoursWorked=0;
				double totalRate2HoursWorked=0;
				double totalRate3HoursWorked=0;
				//sum weekly totals.
				for(int j=0;j<listDateTimesWeekStart.Count;j++){
					totalHoursWorked+=listTimeSpansRegularHoursWeekly[j].TotalHours;
					totalHoursWorked+=listTimeSpansOTHoursWeekly[j].TotalHours;
					totalRegularHoursWorked+=listTimeSpansRegularHoursWeekly[j].TotalHours;
					totalOTHoursWorked+=listTimeSpansOTHoursWeekly[j].TotalHours;
					totalRate2HoursWorked+=listTimeSpansRate2HoursWeekly[j].TotalHours;
					totalRate3HoursWorked+=listTimeSpansRate3HoursWeekly[j].TotalHours;
				}
				double rate1ratio=0;
				double rate2ratio=0;
				double rate3ratio=0;
				//Overtime hours for each rate is based on the ratio of hours of each rate against the total OT hours worked.
				//Example for 40 hour week + 10 OT hours = 50 total hours
				//
				//					Hours Worked	HoursWorked/TotalHours		BaseHoursAtRate			OTHoursAtRate
				//	Rate1				30						30 / 50	= 0.60				0.60 * 40 = 24			0.60 * 10 =  6
				//	Rate2				 5						 5 / 50 = 0.10				0.10 * 40 =  4			0.10 * 10 =  1
				//	Rate3				15						15 / 50 = 0.30				0.30 * 40 = 12			0.30 * 10 =  3
				//																											Total = 40			    Total = 10
				if(totalHoursWorked!=0) {
					rate2ratio=totalRate2HoursWorked/totalHoursWorked;
					rate3ratio=totalRate3HoursWorked/totalHoursWorked;
					rate1ratio=1-rate2ratio-rate3ratio;//"self correcting math" guaranteed to total correctly.
				}
				dataRow["totalHours"]=TimeSpan.FromHours(totalHoursWorked).ToString();
				//Regular time at Rate1, Rate2, and Rate3
				double r1Hours=rate1ratio*totalRegularHoursWorked;
				double r2Hours=rate2ratio*totalRegularHoursWorked;
				double r3Hours=totalRegularHoursWorked-r1Hours-r2Hours; //"self correcting math" guaranteed to total correctly.
				//OT hours
				double r1OTHours=rate1ratio*totalOTHoursWorked;
				double r2OTHours=rate2ratio*totalOTHoursWorked;
				double r3OTHours=totalHoursWorked-r1Hours-r2Hours-r3Hours-r1OTHours-r2OTHours;//"self correcting math" guaranteed to total correctly.
				double unpaidProtectedLeaveHours=listTimeAdjustsEmp.Where(x => x.IsUnpaidProtectedLeave).Sum(x => x.RegHours.TotalHours);
				double ptoHours=listTimeAdjustsEmp.Where(x=>x.PtoDefNum!=0).Sum(x=>x.PtoHours.TotalHours);
				dataRow["rate1Hours"]  =TimeSpan.FromHours(r1Hours).ToString();
				dataRow["rate2Hours"]  =TimeSpan.FromHours(r2Hours).ToString();
				dataRow["rate3Hours"]  =TimeSpan.FromHours(r3Hours).ToString();
				dataRow["rate1OTHours"]=TimeSpan.FromHours(r1OTHours).ToString();
				dataRow["rate2OTHours"]=TimeSpan.FromHours(r2OTHours).ToString();
				dataRow["rate3OTHours"]=TimeSpan.FromHours(r3OTHours).ToString();
				dataRow["PTOHours"]    =TimeSpan.FromHours(ptoHours).ToString();
				dataRow["protectedLeaveHours"]=TimeSpan.FromHours(unpaidProtectedLeaveHours).ToString();
				string payPeriodNote=listTimeAdjustsPayPeriodNote.Find(x => x.EmployeeNum==listEmployees[i].EmployeeNum)?.Note;
				if(string.IsNullOrEmpty(payPeriodNote)){
					dataRow["Note"]=note;
				}
				else {
					dataRow["Note"]=payPeriodNote;
				}
				table.Rows.Add(dataRow);
			}
			return table;
		}

		/// <summary>Used to sum a partial weeks worth of regular hours from clock events and time spans.</summary>
		private static TimeSpan prevWeekRegHoursHelper(long employeeNum,DateTime dateTimeStart,DateTime dateTimeEnd) {
			//No remoting role check; no call to db
			string errors="";
			List<ClockEvent> listClockEvents=new List<ClockEvent>();
			List<TimeAdjust> listTimeAdjusts=new List<TimeAdjust>();
			try { 
				listClockEvents=ClockEvents.GetListForTimeCardManage(employeeNum,0,dateTimeStart,dateTimeEnd,true); 
			}
			catch(Exception ex) {
				errors+=ex.Message;
			}
			try { 
				listTimeAdjusts=TimeAdjusts.GetListForTimeCardManage(employeeNum,0,dateTimeStart,dateTimeEnd,true);
			}
			catch(Exception ex) {
				errors+=ex.Message; 
			}
			TimeSpan timeSpan=TimeSpan.Zero;
			for(int i=0;i<listClockEvents.Count;i++) {
				timeSpan+=listClockEvents[i].TimeDisplayed2-listClockEvents[i].TimeDisplayed1;
				if(listClockEvents[i].AdjustIsOverridden) {//Manual override
					timeSpan+=listClockEvents[i].Adjust;
				}
				else {
					timeSpan+=listClockEvents[i].AdjustAuto;
				}
			}
			for(int i=0;i<listTimeAdjusts.Count;i++) {
				if(listTimeAdjusts[i].IsUnpaidProtectedLeave) {
					continue;
				}
				timeSpan+=listTimeAdjusts[i].RegHours;
			}
			return timeSpan;
		}

		/// <summary>Used to sum a partial weeks worth of OT hours from clock events and time spans.</summary>
		private static TimeSpan prevWeekOTHoursHelper(long employeeNum,DateTime dateTimeStart,DateTime dateTimeEnd) {
			//No remoting role check; no call to db
			List<ClockEvent> listClockEvents=ClockEvents.GetListForTimeCardManage(employeeNum,0,dateTimeStart,dateTimeEnd,true);
			List<TimeAdjust> listTimeAdjusts=TimeAdjusts.GetListForTimeCardManage(employeeNum,0,dateTimeStart,dateTimeEnd,true);
			TimeSpan timeSpan=TimeSpan.Zero;
			for(int i=0;i<listClockEvents.Count;i++) {
				if(listClockEvents[i].OTimeHours!=TimeSpan.FromHours(-1)) {//Manual override
					timeSpan+=listClockEvents[i].OTimeHours;
				}
				else {
					timeSpan+=listClockEvents[i].OTimeAuto;
				}
			}
			for(int i=0;i<listTimeAdjusts.Count;i++) {
				timeSpan+=listTimeAdjusts[i].OTimeHours;
			}
			return timeSpan;
		}

		/// <summary>Used to sum a partial weeks worth of rate2 hours from clock events.</summary>
		private static TimeSpan prevWeekDiffHoursHelper(long employeeNum,DateTime dateTimeStart,DateTime dateTimeEnd) {
			//No remoting role check; no call to db
			List<ClockEvent> listClockEvents=ClockEvents.GetListForTimeCardManage(employeeNum,0,dateTimeStart,dateTimeEnd,true);
			TimeSpan timeSpan=TimeSpan.Zero;
			for(int i=0;i<listClockEvents.Count;i++) {
				if(listClockEvents[i].Rate2Hours!=TimeSpan.FromHours(-1)) {//Manual override
					timeSpan+=listClockEvents[i].Rate2Hours;
				}
				else {
					timeSpan+=listClockEvents[i].Rate2Auto;
				}
			}
			return timeSpan;
		}

		///<summary>Returns number of work weeks spanned by dates.  Example: "11-01-2013"(Friday), to "11-14-2013"(Thursday) spans 3 weeks, if the workweek starts on Sunday it would
		///return a list containing "10-27-2013"(Sunday),"11-03-2013"(Sunday),and"11-10-2013"(Sunday).  Used to determine which week time adjustments and clock events belong to when totalling timespans.</summary>
		public static List<DateTime> WeekStartHelper(DateTime dateTimeStart,DateTime dateTimeStop) {
			//No remoting role check; no call to db
			List<DateTime> listDateTimes=new List<DateTime>();
			DayOfWeek dayOfWeekFirst=(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek);
			for(int i=0;i<7;i++) {//start date of first week.
				if(dateTimeStart.AddDays(-i).DayOfWeek==dayOfWeekFirst) {
					listDateTimes.Add(dateTimeStart.AddDays(-i));//found and added start date of first week.
					break;
				}
			}
			while(listDateTimes[listDateTimes.Count-1].AddDays(7)<dateTimeStop) {//add start of each workweek until we are past the dateStop
				listDateTimes.Add(listDateTimes[listDateTimes.Count-1].AddDays(7));
			}
			return listDateTimes;
		}
		
		///<summary>Returns all clock events, of all statuses, for a given employee between the date range (inclusive).</summary>
		public static List<ClockEvent> GetSimpleList(long employeeNum,DateTime dateTimeStart,DateTime dateTimeStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClockEvent>>(MethodBase.GetCurrentMethod(),employeeNum,dateTimeStart,dateTimeStop);
			}
			//Fill list-----------------------------------------------------------------------------------------------------------------------------
			string command=
				"SELECT * FROM clockevent WHERE"
				+" EmployeeNum = '"+POut.Long(employeeNum)+"'"
				+" AND TimeDisplayed1 >= "+POut.Date(dateTimeStart)
				+" AND TimeDisplayed1 < "+POut.Date(dateTimeStop.AddDays(1))//adding a day takes it to midnight of the specified toDate
				+" ORDER BY TimeDisplayed1";
			return Crud.ClockEventCrud.SelectMany(command);
		}
	}

	
}




