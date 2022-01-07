using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TimeCardRules{
		///<summary>One list of tuples per week, each list of tuples contains up to one entry per clinic, if employee clocked any time at that clinic.
		///<para>Tuple is &lt;ClinicNum,TimeSpan></para></summary>
		private static List<List<Tuple<long,TimeSpan>>> _listEvents;

		#region CachePattern

		private class TimeCardRuleCache : CacheListAbs<TimeCardRule> {
			protected override List<TimeCardRule> GetCacheFromDb() {
				string command="SELECT * FROM timecardrule";
				return Crud.TimeCardRuleCrud.SelectMany(command);
			}
			protected override List<TimeCardRule> TableToList(DataTable table) {
				return Crud.TimeCardRuleCrud.TableToList(table);
			}
			protected override TimeCardRule Copy(TimeCardRule timeCardRule) {
				return timeCardRule.Clone();
			}
			protected override DataTable ListToTable(List<TimeCardRule> listTimeCardRules) {
				return Crud.TimeCardRuleCrud.ListToTable(listTimeCardRules,"TimeCardRule");
			}
			protected override void FillCacheIfNeeded() {
				TimeCardRules.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static TimeCardRuleCache _timeCardRuleCache=new TimeCardRuleCache();

		public static List<TimeCardRule> GetDeepCopy(bool isShort=false) {
			return _timeCardRuleCache.GetDeepCopy(isShort);
		}

		public static List<TimeCardRule> GetWhere(Predicate<TimeCardRule> match,bool isShort=false) {
			return _timeCardRuleCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_timeCardRuleCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_timeCardRuleCache.FillCacheFromTable(table);
				return table;
			}
			return _timeCardRuleCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<TimeCardRule> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TimeCardRule>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM timecardrule WHERE PatNum = "+POut.Long(patNum);
			return Crud.TimeCardRuleCrud.SelectMany(command);
		}

		///<summary>Gets one TimeCardRule from the db.</summary>
		public static TimeCardRule GetOne(long timeCardRuleNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<TimeCardRule>(MethodBase.GetCurrentMethod(),timeCardRuleNum);
			}
			return Crud.TimeCardRuleCrud.SelectOne(timeCardRuleNum);
		}*/

		///<summary></summary>
		public static long Insert(TimeCardRule timeCardRule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				timeCardRule.TimeCardRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),timeCardRule);
				return timeCardRule.TimeCardRuleNum;
			}
			return Crud.TimeCardRuleCrud.Insert(timeCardRule);
		}
		
		///<summary></summary>
		public static void InsertMany(List<TimeCardRule> listTimeCardRules){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTimeCardRules);
				return;
			}
			Crud.TimeCardRuleCrud.InsertMany(listTimeCardRules);
		}

		///<summary></summary>
		public static void Update(TimeCardRule timeCardRule){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),timeCardRule);
				return;
			}
			Crud.TimeCardRuleCrud.Update(timeCardRule);
		}

		///<summary></summary>
		public static void Delete(long timeCardRuleNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),timeCardRuleNum);
				return;
			}
			string command= "DELETE FROM timecardrule WHERE TimeCardRuleNum = "+POut.Long(timeCardRuleNum);
			Db.NonQ(command);
		}

		
		///<summary></summary>
		public static void DeleteMany(List<long> listTimeCardRuleNums) {
			if(listTimeCardRuleNums==null || listTimeCardRuleNums.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTimeCardRuleNums);
				return;
			}
			string command="DELETE FROM timecardrule WHERE TimeCardRuleNum IN ("+string.Join(",",listTimeCardRuleNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Validates pay period before making any adjustments.
		///If today falls before the stopDate passed in, stopDate will be set to today's date.</summary>
		public static string ValidatePayPeriod(Employee employeeCur,DateTime startDate,DateTime stopDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Middle tier check here just to save trips.  No call to the db.
				return Meth.GetString(MethodBase.GetCurrentMethod(),employeeCur,startDate,stopDate);
			}
			//If calculating breaks before the end date of the pay period, only calculate breaks and validate clock in and out events for days
			//before today.  Use the server time just because we are dealing with time cards.
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			ClockEvent lastClockEvent=ClockEvents.GetLastEvent(employeeCur.EmployeeNum);
			bool isStillWorking=(lastClockEvent!=null && (lastClockEvent.ClockStatus==TimeClockStatus.Break || lastClockEvent.TimeDisplayed2.Year<1880));
			if(dateTimeNow.Date < stopDate.Date && isStillWorking) {
				stopDate=dateTimeNow.Date.AddDays(-1);
			}
			List<ClockEvent> breakList=ClockEvents.Refresh(employeeCur.EmployeeNum,startDate,stopDate,true);
			List<ClockEvent> clockEventList=ClockEvents.Refresh(employeeCur.EmployeeNum,startDate,stopDate,false);
			bool errorFound=false;
			string retVal="Time card errors for employee : "+Employees.GetNameFL(employeeCur)+"\r\n";
			//Validate clock events
			foreach(ClockEvent cCur in clockEventList) {
				if(cCur.TimeDisplayed2.Year<1880) {
					retVal+="  "+cCur.TimeDisplayed1.ToShortDateString()+" : Employee not clocked out.\r\n";
					errorFound=true;
				}
				else if(cCur.TimeDisplayed1.Date!=cCur.TimeDisplayed2.Date) {
					retVal+="  "+cCur.TimeDisplayed1.ToShortDateString()+" : Clock entry spans multiple days.\r\n";
					errorFound=true;
				}
			}
			//Validate Breaks
			foreach(ClockEvent bCur in breakList) {
				if(bCur.TimeDisplayed2.Year<1880) {
					retVal+="  "+bCur.TimeDisplayed1.ToShortDateString()+" : Employee not clocked in from break.\r\n";
					errorFound=true;
				}
				if(bCur.TimeDisplayed1.Date!=bCur.TimeDisplayed2.Date) {
					retVal+="  "+bCur.TimeDisplayed1.ToShortDateString()+" : One break spans multiple days.\r\n";
					errorFound=true;
				}
				for(int c=clockEventList.Count-1;c>=0;c--) {
					if(clockEventList[c].TimeDisplayed1.Date==bCur.TimeDisplayed1.Date) {
						break;
					}
					if(c==0) { //we never found a match
						retVal+="  "+bCur.TimeDisplayed1.ToShortDateString()+" : Break found during non-working day.\r\n";
						errorFound=true;
					}
				}
			}
			return (errorFound?retVal:"");
		}

		///<summary>Cannot have both AM/PM rules and OverHours rules defined. 
		/// We no longer block having multiple rules defined. With a better interface we can improve some of this functionality. Per NS 09/15/2015.</summary>
		public static string ValidateOvertimeRules(List<long> listEmployeeNums=null) {
			StringBuilder sb=new StringBuilder();
			TimeCardRules.RefreshCache();
			List<TimeCardRule> listTimeCardRules=TimeCardRules.GetDeepCopy();
			if(listEmployeeNums!=null && listEmployeeNums.Count>0) {
				listTimeCardRules=listTimeCardRules.FindAll(x => x.EmployeeNum==0 || listEmployeeNums.Contains(x.EmployeeNum));
			}
			//Generate error messages for "All Employees" timecard rules.
			List<TimeCardRule> listTimeCardRulesAll=listTimeCardRules.FindAll(x => x.EmployeeNum==0);
			if(listTimeCardRulesAll.Any(x => x.AfterTimeOfDay>TimeSpan.Zero || x.BeforeTimeOfDay>TimeSpan.Zero) //There exists an AM or PM rule
				   && listTimeCardRulesAll.Any(x => x.OverHoursPerDay>TimeSpan.Zero)) //There also exists an Over hours rule.
			{
				sb.AppendLine("Time card errors found for \"All Employees\":");
				sb.AppendLine("  Both a time of day rule and an over hours per day rule found. Only one or the other is allowed.");
				return sb.ToString();
			}
			listEmployeeNums=listTimeCardRules.Where(x=>x.EmployeeNum>0).Select(x=>x.EmployeeNum).Distinct().ToList();
			//Generate Employee specific errors
			for(int i=0;i<listEmployeeNums.Count;i++) {
				long empNum=listEmployeeNums[i];
				List<TimeCardRule> listTimeCardRulesEmp=listTimeCardRules.FindAll(x => x.EmployeeNum==0 || x.EmployeeNum==empNum);
				if(listTimeCardRulesEmp.Any(x => x.AfterTimeOfDay>TimeSpan.Zero || x.BeforeTimeOfDay>TimeSpan.Zero) //There exists an AM or PM rule
				   && listTimeCardRulesEmp.Any(x => x.OverHoursPerDay>TimeSpan.Zero)) //There also exists an Over hours rule.
				{
					string empName=Employees.GetNameFL(Employees.GetEmp(empNum));
					sb.AppendLine("Time card errors found for "+empName+":");
					sb.AppendLine("  Both a time of day rule and an over hours per day rule found. Only one or the other is allowed.\r\n");
				}
			}
			return sb.ToString();
		}

		///<summary>Clears automatic adjustment/adjustOT values and deletes automatic TimeAdjusts for period.</summary>
		public static void ClearAuto(long employeeNum,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employeeNum,dateStart,dateStop);
				return;
			}
			List<ClockEvent> ListCE=ClockEvents.GetSimpleList(employeeNum,dateStart,dateStop);
			for(int i=0;i<ListCE.Count;i++) {
				ListCE[i].AdjustAuto=TimeSpan.Zero;
				ListCE[i].OTimeAuto=TimeSpan.Zero;
				ListCE[i].Rate2Auto=TimeSpan.Zero;
				ClockEvents.Update(ListCE[i]);
			}
			List<TimeAdjust> ListTA=TimeAdjusts.GetSimpleListAuto(employeeNum,dateStart,dateStop);
			for(int i=0;i<ListTA.Count;i++) {
				TimeAdjusts.Delete(ListTA[i]);
				SecurityLogs.MakeLogEntry(Permissions.TimeAdjustEdit,0,
					$"Automatic Time Card Adjustments were cleared. Adjustment deleted for Employee: {Employees.GetNameFL(ListTA[i].EmployeeNum)}.");
			}
		}

		///<summary>Clears all manual adjustments/Adjust OT values from clock events. Does not alter adjustments to clockevent.TimeDisplayed1/2 nor does it delete or alter any TimeAdjusts.</summary>
		public static void ClearManual(long employeeNum,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employeeNum,dateStart,dateStop);
				return;
			}
			List<ClockEvent> ListCE=ClockEvents.GetSimpleList(employeeNum,dateStart,dateStop);
			for(int i=0;i<ListCE.Count;i++) {
				ListCE[i].Adjust=TimeSpan.Zero;
				ListCE[i].AdjustIsOverridden=false;
				ListCE[i].OTimeHours=TimeSpan.FromHours(-1);
				ListCE[i].Rate2Hours=TimeSpan.FromHours(-1);
				ClockEvents.Update(ListCE[i]);
			}
		}

		///<summary>Validates list and throws exceptions. Always returns a value. Creates a timecard rule based on all applicable timecard rules for a given employee.</summary>
		public static TimeCardRule GetTimeCardRule(Employee employeeCur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<TimeCardRule>(MethodBase.GetCurrentMethod(),employeeCur);
			}
			//Validate Rules---------------------------------------------------------------------------------------------------------------
			string errors=TimeCardRules.ValidateOvertimeRules(new List<long> {employeeCur.EmployeeNum});
			if(errors.Length>0) {
				throw new Exception(errors);
			}
			//Build return value ----------------------------------------------------------------------------------------------------------
			List<TimeCardRule> listTimeCardRulesEmp=GetWhere(x => x.EmployeeNum==0 || x.EmployeeNum==employeeCur.EmployeeNum);
			TimeCardRule amRule=listTimeCardRulesEmp.Where(x => x.BeforeTimeOfDay>TimeSpan.Zero).OrderByDescending(x => x.BeforeTimeOfDay).FirstOrDefault();
			TimeCardRule pmRule=listTimeCardRulesEmp.Where(x => x.AfterTimeOfDay>TimeSpan.Zero).OrderBy(x => x.AfterTimeOfDay).FirstOrDefault();
			TimeCardRule hoursRule=listTimeCardRulesEmp.Where(x => x.OverHoursPerDay>TimeSpan.Zero).OrderBy(x => x.OverHoursPerDay).FirstOrDefault();
			TimeCardRule isOvertimeExempt=listTimeCardRulesEmp.Where(x => x.IsOvertimeExempt).FirstOrDefault();
			TimeCardRule retVal=new TimeCardRule();
			if(amRule!=null) {
				retVal.BeforeTimeOfDay=amRule.BeforeTimeOfDay;
			}
			if(pmRule!=null) {
				retVal.AfterTimeOfDay=pmRule.AfterTimeOfDay;
			}
			if(hoursRule!=null) {
				retVal.OverHoursPerDay=hoursRule.OverHoursPerDay;
			}
			if(isOvertimeExempt!=null) {
				retVal.IsOvertimeExempt=isOvertimeExempt.IsOvertimeExempt;
			}
			return retVal;
		}

		///<summary>Calculates daily overtime. Throws exceptions when encountering errors, though all errors SHOULD have been caught already by using the ValidatePayPeriod() function and generating a msgbox.</summary>
		public static void CalculateDailyOvertime_Old(Employee EmployeeCur,DateTime StartDate,DateTime StopDate) {
			DateTime previousDate;
			List<ClockEvent> ClockEventList=ClockEvents.Refresh(EmployeeCur.EmployeeNum,StartDate,StopDate,false);//PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),IsBreaks);
			//Over breaks-------------------------------------------------------------------------------------------------
			if(PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks)) {
				//set adj auto to zero for all.
				for(int i=0;i<ClockEventList.Count;i++) {
					ClockEventList[i].AdjustAuto=TimeSpan.Zero;
					ClockEvents.Update(ClockEventList[i]);
				}
				List<ClockEvent> breakList=ClockEvents.Refresh(EmployeeCur.EmployeeNum,StartDate,StopDate,true);//PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),true);
				TimeSpan totalToday=TimeSpan.Zero;
				TimeSpan totalOne=TimeSpan.Zero;
				previousDate=DateTime.MinValue;
				for(int b=0;b<breakList.Count;b++) {
					if(breakList[b].TimeDisplayed2.Year<1880) {
						throw new Exception("Error. Employee break malformed.");
					}
					if(breakList[b].TimeDisplayed1.Date != breakList[b].TimeDisplayed2.Date) {
						throw new Exception("Error. One break spans multiple dates.");
					}
					//calc time for the one break
					totalOne=breakList[b].TimeDisplayed2-breakList[b].TimeDisplayed1;
					//calc daily total
					if(previousDate.Date != breakList[b].TimeDisplayed1.Date) {//if date changed, this is the first pair of the day
						totalToday=TimeSpan.Zero;//new day
						previousDate=breakList[b].TimeDisplayed1.Date;//for the next loop
					}
					totalToday+=totalOne;
					//decide if breaks for the day went over 30 min.
					if(totalToday > TimeSpan.FromMinutes(31)) {//31 to prevent silly fractions less than 1.
						//loop through all ClockEvents in this grid to find one to adjust.
						//Go backwards to find the last entry for a given date.
						for(int c=ClockEventList.Count-1;c>=0;c--) {
							if(ClockEventList[c].TimeDisplayed1.Date==breakList[b].TimeDisplayed1.Date) {
								ClockEventList[c].AdjustAuto-=(totalToday-TimeSpan.FromMinutes(30));
								ClockEvents.Update(ClockEventList[c]);
								totalToday=TimeSpan.FromMinutes(30);//reset to 30.  Therefore, any additional breaks will be wholly adjustments.
								break;
							}
							if(c==0) {//we never found a match
								throw new Exception("Error. Over breaks, but could not adjust because not regular time entered for date:"
								  +breakList[b].TimeDisplayed1.Date.ToShortDateString());
							}
						}
					}
				}
			}
			//OT-------------------------------------------------------------------------------------------------------------
			TimeCardRule afterTimeRule=null;
			TimeCardRule beforeTimeRule=null;
			TimeCardRule overHoursRule=null;
			//loop through timecardrules to find one rule of each kind.
			List<TimeCardRule> listTimeCardRules=GetDeepCopy();
			for(int i=0;i<listTimeCardRules.Count;i++) {
				if(listTimeCardRules[i].EmployeeNum!=0 && listTimeCardRules[i].EmployeeNum!=EmployeeCur.EmployeeNum) {
					continue;
				}
				if(listTimeCardRules[i].AfterTimeOfDay > TimeSpan.Zero) {
					if(afterTimeRule != null) {//already found a match, and this is a second match
						throw new Exception("Error.  Multiple matches of AfterTimeOfDay found for this employee.  Only one allowed.");
						//return;
					}
					afterTimeRule=listTimeCardRules[i];
				}
				else if(listTimeCardRules[i].OverHoursPerDay > TimeSpan.Zero) {
					if(overHoursRule != null) {//already found a match, and this is a second match
						throw new Exception("Error.  Multiple matches of OverHoursPerDay found for this employee.  Only one allowed.");
						//return;
					}
					overHoursRule=listTimeCardRules[i];
				}
				if(afterTimeRule!= null && overHoursRule != null) {
					throw new Exception("Error.  Both an OverHoursPerDay and an AfterTimeOfDay found for this employee.  Only one or the other is allowed.");
					//return;
				}
				if(beforeTimeRule != null && overHoursRule != null) {
					throw new Exception("Error.  Both an OverHoursPerDay and an BeforeTimeOfDay found for this employee.  Only one or the other is allowed.");
					//return;
				}
				if(listTimeCardRules[i].BeforeTimeOfDay > TimeSpan.Zero) {
					if(beforeTimeRule != null) {//already found a match, and this is a second match
						throw new Exception("Error.  Multiple matches of BeforeTimeOfDay found for this employee.  Only one allowed.");
						//return;
					}
					beforeTimeRule=listTimeCardRules[i];
				}
			}
			//loop through all ClockEvents in this grid.
			TimeSpan dailyTotal=TimeSpan.Zero;
			TimeSpan pairTotal=TimeSpan.Zero;
			previousDate=DateTime.MinValue;
			for(int i=0;i<ClockEventList.Count;i++) {
				if(ClockEventList[i].TimeDisplayed2.Year<1880) {
					throw new Exception("Error. Employee not clocked out.");
					//return;
				}
				if(ClockEventList[i].TimeDisplayed1.Date != ClockEventList[i].TimeDisplayed2.Date) {
					throw new Exception("Error. One clock pair spans multiple dates.");
					//return;
				}
				pairTotal=ClockEventList[i].TimeDisplayed2-ClockEventList[i].TimeDisplayed1;
				//add any adjustments, manual or overrides.
				if(ClockEventList[i].AdjustIsOverridden) {
					pairTotal+=ClockEventList[i].Adjust;
				}
				else {
					pairTotal+=ClockEventList[i].AdjustAuto;
				}
				//calc daily total
				if(previousDate.Date != ClockEventList[i].TimeDisplayed1.Date) { //if date changed
					dailyTotal=TimeSpan.Zero;//new day
					previousDate=ClockEventList[i].TimeDisplayed1.Date;//for the next loop
				}
				dailyTotal+=pairTotal;
				//handle OT
				ClockEventList[i].OTimeAuto=TimeSpan.Zero;//set auto OT to zero.
				if(ClockEventList[i].OTimeHours != TimeSpan.FromHours(-1)) {//if OT is overridden
					//don't try to calc a time.
					ClockEvents.Update(ClockEventList[i]);//just to possibly clear autoOT, even though it doesn't count.
					//but still need to subtract OT from dailyTotal
					dailyTotal-=ClockEventList[i].OTimeHours;
					continue;
				}
				if(afterTimeRule != null) {
					//test to see if this span is after specified time
					TimeSpan afterTime=TimeSpan.Zero;
					if(ClockEventList[i].TimeDisplayed1.TimeOfDay > afterTimeRule.AfterTimeOfDay) {//the start time is after time, so the whole pairTotal is OT
						afterTime=pairTotal;
					}
					else if(ClockEventList[i].TimeDisplayed2.TimeOfDay > afterTimeRule.AfterTimeOfDay) {//only the second time is after time
						afterTime=ClockEventList[i].TimeDisplayed2.TimeOfDay-afterTimeRule.AfterTimeOfDay;//only a portion of the pairTotal is OT
					}
					ClockEventList[i].OTimeAuto=afterTime;
				}
				if(beforeTimeRule != null) {
					//test to see if this span is after specified time
					TimeSpan beforeTime=TimeSpan.Zero;
					if(ClockEventList[i].TimeDisplayed2.TimeOfDay < beforeTimeRule.BeforeTimeOfDay) {//the end time is before time, so the whole pairTotal is OT
						beforeTime+=pairTotal;
					}
					else if(ClockEventList[i].TimeDisplayed1.TimeOfDay < beforeTimeRule.BeforeTimeOfDay) {//only the first time is before time
						beforeTime+=beforeTimeRule.BeforeTimeOfDay-ClockEventList[i].TimeDisplayed1.TimeOfDay;//only a portion of the pairTotal is OT
					} 
					ClockEventList[i].OTimeAuto+=beforeTime;
				}
				if(overHoursRule != null) {
					//test dailyTotal
					TimeSpan overHours=TimeSpan.Zero;
					if(dailyTotal > overHoursRule.OverHoursPerDay) {
						overHours=dailyTotal-overHoursRule.OverHoursPerDay;
						dailyTotal=overHoursRule.OverHoursPerDay;//e.g. reset to 8.  Any further pairs on this date will be wholly OT
						ClockEventList[i].OTimeAuto+=overHours;
					}
				}
				ClockEvents.Update(ClockEventList[i]);
			}
			AdjustBreaksHelper(EmployeeCur,StartDate,StopDate);
		}

		///<summary>Calculates daily overtime.  Daily overtime does not take into account any time adjust events.
		///All manually entered time adjust events are assumed to be entered correctly and should not be used in calculating automatic totals.
		///Throws exceptions when encountering errors.</summary>
		public static void CalculateDailyOvertime(Employee employee,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Middle tier check here just to save trips.  No call to the db.
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employee,dateStart,dateStop);
				return;
			}
			#region Fill Lists, validate data sets, generate error messages.
			List<ClockEvent> listClockEvent=new List<ClockEvent>();
			List<ClockEvent> listClockEventBreak=new List<ClockEvent>();
			TimeCardRule timeCardRule=new TimeCardRule();
			string errors="";
			string clockErrors="";
			string breakErrors="";
			string ruleErrors="";
			//If calculating breaks before the end date of the pay period, only calculate breaks and validate clock in and out events for days
			//before today.  Use the server time just because we are dealing with time cards.
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			ClockEvent lastClockEvent=ClockEvents.GetLastEvent(employee.EmployeeNum);
			bool isStillWorking=(lastClockEvent!=null && (lastClockEvent.ClockStatus==TimeClockStatus.Break || lastClockEvent.TimeDisplayed2.Year<1880));
			if(dateTimeNow.Date < dateStop.Date && isStillWorking) {
				dateStop=dateTimeNow.Date.AddDays(-1);
			}
			//Fill lists and catch validation error messages------------------------------------------------------------------------------------------------------------
			try {
				listClockEvent=ClockEvents.GetValidList(employee.EmployeeNum,dateStart,dateStop,false);
			}
			catch(Exception ex) {
				clockErrors+=ex.Message;
			}
			try {
				listClockEventBreak=ClockEvents.GetValidList(employee.EmployeeNum,dateStart,dateStop,true);
			}
			catch(Exception ex) {
				breakErrors+=ex.Message;
			}
			try {
				timeCardRule=TimeCardRules.GetTimeCardRule(employee);
			}
			catch(Exception ex) {
				ruleErrors+=ex.Message;
			}
			//Validation between two or more lists above----------------------------------------------------------------------------------------------------------------
			for(int b=0;b<listClockEventBreak.Count;b++) {
				bool isValidBreak=false;
				for(int c=0;c<listClockEvent.Count;c++) {
					if(timeClockEventsOverlapHelper(listClockEventBreak[b],listClockEvent[c])) {
						if(listClockEventBreak[b].TimeDisplayed1>listClockEvent[c].TimeDisplayed1//break started after work did
							&& listClockEventBreak[b].TimeDisplayed2<listClockEvent[c].TimeDisplayed2)//break ended before working hours
						{
							//valid break.
							isValidBreak=true;
							break;
						}
						//invalid break.
						isValidBreak=false;//redundant, but harmless. Makes code more readable.
						break;
					}
				}
				if(isValidBreak) {
					continue;
				}
				breakErrors+="  "+listClockEventBreak[b].TimeDisplayed1.ToString()+" : break found during non-working hours.\r\n";//ToString() instead of ToShortDateString() to show time.
			}
			//Report Errors---------------------------------------------------------------------------------------------------------------------------------------------
			errors=ruleErrors+clockErrors+breakErrors;
			if(errors!="") {
				throw new Exception(Employees.GetNameFL(employee)+" has the following errors:\r\n"+errors);
				//throw new Exception(errors);
			}
			#endregion
			#region Fill time card rules
			//Begin calculations=========================================================================================================================================
			TimeSpan tsHoursWorkedTotal  =new TimeSpan()      ;
			TimeSpan tsOvertimeHoursRule =new TimeSpan(24,0,0);//Example 10:00 for overtime rule after 10 hours per day.
			TimeSpan tsDifferentialAMRule=new TimeSpan()      ;//Example 06:00 for differential rule before 6am.
			TimeSpan tsDifferentialPMRule=new TimeSpan(24,0,0);//Example 17:00 for differential rule after  5pm.
			//Fill over hours rule from list-------------------------------------------------------------------------------------
			if(timeCardRule.OverHoursPerDay!=TimeSpan.Zero) {//OverHours Rule
				tsOvertimeHoursRule=timeCardRule.OverHoursPerDay;//at most, one non-zero OverHours rule available at this point.
			}
			if(timeCardRule.BeforeTimeOfDay!=TimeSpan.Zero) {//AM Rule
				tsDifferentialAMRule=timeCardRule.BeforeTimeOfDay;//at most, one non-zero AM rule available at this point.
			}
			if(timeCardRule.AfterTimeOfDay!=TimeSpan.Zero) {//PM Rule
				tsDifferentialPMRule=timeCardRule.AfterTimeOfDay;//at most, one non-zero PM rule available at this point.
			}
			#endregion
			//Calculations: Regular Time, Overtime, Rate2 time---------------------------------------------------------------------------------------------------
			TimeSpan tsDailyBreaksAdjustTotal=new TimeSpan();//used to adjust the clock event
			TimeSpan tsDailyBreaksTotal=new TimeSpan();//used in calculating breaks over 30 minutes per day.
			TimeSpan tsDailyDifferentialTotal=new TimeSpan();//hours before and after AM/PM diff rules. Adjusted for overbreaks.
			//Note: If TimeCardsMakesAdjustmentsForOverBreaks is true, only the first 30 minutes of break per day are paid. 
			//All breaktime thereafter will be calculated as if the employee was clocked out at that time.
			for(int i=0;i<listClockEvent.Count;i++) {
				#region  Differential pay (including overbreak adjustments)--------------------------------------------------------------
				if(i==0 || listClockEvent[i].TimeDisplayed1.Date!=listClockEvent[i-1].TimeDisplayed1.Date) {
					tsDailyDifferentialTotal=TimeSpan.Zero;
				}
				//AM-----------------------------------
				if(listClockEvent[i].TimeDisplayed1.TimeOfDay<tsDifferentialAMRule) {//clocked in before AM differential rule
					tsDailyDifferentialTotal+=tsDifferentialAMRule-listClockEvent[i].TimeDisplayed1.TimeOfDay;
					if(listClockEvent[i].TimeDisplayed2.TimeOfDay<tsDifferentialAMRule) {//clocked out before AM differential rule also
						tsDailyDifferentialTotal+=listClockEvent[i].TimeDisplayed1.TimeOfDay-tsDifferentialAMRule;//add a negative timespan
					}
					//Adjust AM differential by overbreaks-----
					TimeSpan tsAMBreakTimeCounter=new TimeSpan();//tracks all break time for use in calculating overages.
					TimeSpan tsAMBreakDuringDiff=new TimeSpan();//tracks only the portion of breaks that occurred during differential hours.
					for(int b=0;b<listClockEventBreak.Count;b++) {
						if(tsAMBreakTimeCounter>TimeSpan.FromMinutes(30)) {
							tsAMBreakTimeCounter=TimeSpan.FromMinutes(30);//reset overages for next calculation.
						}
						if(listClockEventBreak[b].TimeDisplayed1.Date!=listClockEvent[i].TimeDisplayed1.Date) {
							continue;//skip breaks for other days.
						}
						tsAMBreakTimeCounter+=listClockEventBreak[b].TimeDisplayed2-listClockEventBreak[b].TimeDisplayed1;
						tsAMBreakDuringDiff+=calcDifferentialPortion(tsDifferentialAMRule,TimeSpan.FromHours(24),listClockEventBreak[b]);
						if(tsAMBreakTimeCounter<TimeSpan.FromMinutes(30)) {
							continue;//not over thirty minutes yet.
						}
						if(timeClockEventsOverlapHelper(listClockEvent[i],listClockEventBreak[b])) {
							continue;//There must be multiple clock events for this day, and we have gone over breaks during a later clock event period
						}
						if(listClockEventBreak[b].TimeDisplayed1.TimeOfDay>tsDifferentialAMRule) {
							continue;//this break started after the AM differential so there is nothing left to do in this loop. break out of the entire loop.
						}
						if(listClockEventBreak[b].TimeDisplayed2.TimeOfDay-(tsAMBreakTimeCounter-TimeSpan.FromMinutes(30))>tsDifferentialAMRule) {
							continue;//entirety of break overage occurred after AM differential time.
						}
						//Make adjustments because: 30+ minutes of break, break occurred during clockEvent, break started before the AM rule.
						TimeSpan tsAMAdjustAmount=System.TimeSpan.FromMinutes(30)-tsAMBreakTimeCounter;
						if(tsAMAdjustAmount<-tsAMBreakDuringDiff) {
							tsAMAdjustAmount=-tsAMBreakDuringDiff;//cannot adjust off more break overage time than we have had breaks during this time.
						}
						tsDailyDifferentialTotal+=tsAMAdjustAmount;//adjust down
						tsAMBreakDuringDiff+=tsAMAdjustAmount;//adjust down
					}
				}
				//PM-------------------------------------
				if(listClockEvent[i].TimeDisplayed2.TimeOfDay>tsDifferentialPMRule) {//clocked out after PM differential rule
					tsDailyDifferentialTotal+=listClockEvent[i].TimeDisplayed2.TimeOfDay-tsDifferentialPMRule;
					if(listClockEvent[i].TimeDisplayed1.TimeOfDay>tsDifferentialPMRule) {//clocked in after PM differential rule also
						tsDailyDifferentialTotal+=tsDifferentialPMRule-listClockEvent[i].TimeDisplayed1.TimeOfDay;//add a negative timespan
					}
					//Adjust PM differential by overbreaks-----
					TimeSpan tsPMBreakTimeCounter=new TimeSpan();//tracks all break time for use in calculating overages.
					TimeSpan tsPMBreakDuringDiff=new TimeSpan();//tracks only the portion of breaks that occurred during differential hours.
					for(int b=0;b<listClockEventBreak.Count;b++) {
						if(tsPMBreakTimeCounter>TimeSpan.FromMinutes(30)) {
							tsPMBreakTimeCounter=TimeSpan.FromMinutes(30);//reset overages for next calculation.
						}
						if(listClockEventBreak[b].TimeDisplayed1.Date!=listClockEvent[i].TimeDisplayed1.Date) {
							continue;//skip breaks for other days.
						}
						tsPMBreakTimeCounter+=listClockEventBreak[b].TimeDisplayed2-listClockEventBreak[b].TimeDisplayed1;
						tsPMBreakDuringDiff+=calcDifferentialPortion(TimeSpan.Zero,tsDifferentialPMRule,listClockEventBreak[b]);
						if(tsPMBreakTimeCounter<TimeSpan.FromMinutes(30)) {
							continue;//not over thirty minutes yet.
						}
						if(!timeClockEventsOverlapHelper(listClockEvent[i],listClockEventBreak[b])) {
							continue;//There must be multiple clock events for this day, and we have gone over breaks during a different clock event period
						}
						if(listClockEventBreak[b].TimeDisplayed2.TimeOfDay<tsDifferentialPMRule) {
							continue;//entirety of break overage occurred before PM differential time.
						}
						//Make adjustments because: 30+ minutes of break, break occurred during clockEvent, break ended after the PM rule.
						TimeSpan tsPMAdjustAmount=System.TimeSpan.FromMinutes(30)-tsPMBreakTimeCounter;//tsPMBreakTimeCounter is always > 30 at this point in time
						if(tsPMAdjustAmount<-tsPMBreakDuringDiff) {
							tsPMAdjustAmount=-tsPMBreakDuringDiff;//cannot adjust off more break overage time than we have had breaks during this time.
						}
						tsDailyDifferentialTotal+=tsPMAdjustAmount;//adjust down
						tsPMBreakDuringDiff+=tsPMAdjustAmount;//adjust down
					}
				}
				//Apply differential to clock event-----------------------------------------------------------------------------------
				if(tsDailyDifferentialTotal<TimeSpan.Zero) {
					//this should never happen. If it ever does, we need to know about it, because that means some math has been miscalculated.
					throw new Exception(" - "+listClockEvent[i].TimeDisplayed1.Date.ToShortDateString()+", "+employee.FName+" "+employee.LName+" : calculated differential hours was negative.");
				}
				listClockEvent[i].Rate2Auto=tsDailyDifferentialTotal;//should be zero or greater.
				#endregion
				#region Regular hours and OT hours calulations (including overbreak adjustments)----------------------------------------
				listClockEvent[i].OTimeAuto	=TimeSpan.Zero;
				listClockEvent[i].AdjustAuto=TimeSpan.Zero;
				if(i==0 || listClockEvent[i].TimeDisplayed1.Date!=listClockEvent[i-1].TimeDisplayed1.Date) {
					tsHoursWorkedTotal=TimeSpan.Zero;
					tsDailyBreaksAdjustTotal=TimeSpan.Zero;
					tsDailyBreaksTotal=TimeSpan.Zero;
					tsDailyDifferentialTotal=TimeSpan.Zero;
				}
				tsHoursWorkedTotal+=listClockEvent[i].TimeDisplayed2-listClockEvent[i].TimeDisplayed1;//Hours worked
				if(tsHoursWorkedTotal>tsOvertimeHoursRule) {//if OverHoursPerDay then make AutoOTAdjustments.
					listClockEvent[i].OTimeAuto	+=tsHoursWorkedTotal-tsOvertimeHoursRule;//++OTimeAuto
					//listClockEvent[i].AdjustAuto-=tsHoursWorkedTotal-tsOvertimeHoursRule;//--AdjustAuto
					tsHoursWorkedTotal=tsOvertimeHoursRule;//subsequent clock events should be counted as overtime.
				}
				if(i==listClockEvent.Count-1 || listClockEvent[i].TimeDisplayed1.Date!=listClockEvent[i+1].TimeDisplayed1.Date) {
					//Either the last clock event in the list or last clock event for the day.
					//OVERBREAKS--------------------------------------------------------------------------------------------------------
					if(PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks)) {//Apply overbreaks to this clockEvent.
						tsDailyBreaksAdjustTotal=new TimeSpan();//used to adjust the clock event
						tsDailyBreaksTotal=new TimeSpan();//used in calculating breaks over 30 minutes per day.
						for(int b=0;b<listClockEventBreak.Count;b++) {//check all breaks for current day.
							if(listClockEventBreak[b].TimeDisplayed1.Date!=listClockEvent[i].TimeDisplayed1.Date) {
								continue;//skip breaks for other dates than current ClockEvent
							}
							tsDailyBreaksTotal+=(listClockEventBreak[b].TimeDisplayed2.TimeOfDay-listClockEventBreak[b].TimeDisplayed1.TimeOfDay);
							if(tsDailyBreaksTotal>TimeSpan.FromMinutes(31)) {//over 31 to avoid adjustments less than 1 minutes.
								listClockEventBreak[b].AdjustAuto=TimeSpan.FromMinutes(30)-tsDailyBreaksTotal;
								ClockEvents.Update(listClockEventBreak[b]);//save adjustments to breaks.
								tsDailyBreaksAdjustTotal+=listClockEventBreak[b].AdjustAuto;
								tsDailyBreaksTotal=TimeSpan.FromMinutes(30);//reset daily breaks to 30 minutes so the next break is all adjustment.
							}//end overBreaks>31 minutes
							else {
								//If the adjustment is 30 minutes or less, the adjustment amount should be set to 0 
								listClockEventBreak[b].AdjustAuto=TimeSpan.Zero;
								ClockEvents.Update(listClockEventBreak[b]);
							}
						}//end checking all breaks for current day
						//OverBreaks applies to overtime and then to RegularTime
						listClockEvent[i].OTimeAuto+=tsDailyBreaksAdjustTotal;//tsDailyBreaksTotal<=TimeSpan.Zero
						listClockEvent[i].AdjustAuto+=tsDailyBreaksAdjustTotal;//tsDailyBreaksTotal is less than or equal to zero
						if(listClockEvent[i].OTimeAuto<TimeSpan.Zero) {//we have adjusted OT too far
							//listClockEvent[i].AdjustAuto+=listClockEvent[i].OTimeAuto;
							listClockEvent[i].OTimeAuto=TimeSpan.Zero;
						}
						tsDailyBreaksTotal=TimeSpan.Zero;//zero out for the next day.
						tsHoursWorkedTotal=TimeSpan.Zero;//zero out for next day.
					}//end overbreaks
				}
				#endregion
				ClockEvents.Update(listClockEvent[i]);
			}//end clockEvent loop.
		}

		private static TimeSpan calcDifferentialPortion(TimeSpan tsDifferentialAMRule,TimeSpan tsDifferentialPMRule,ClockEvent clockEventBreak) {
			TimeSpan retVal=new TimeSpan();
			//AM overlap==========================================================
			//Visual representation
			//AM Rule      :           X
			//Entire Break :o-------o  |             Stop-Start == Entire Break
			//Partial Break:      o----|---o         Rule-Start == Partial Break
			//No Break     :           |  o------o   Rule-Rule  == No break (won't actually happen in this block)
			retVal+=TimeSpan.FromTicks(
				Math.Min(clockEventBreak.TimeDisplayed2.TimeOfDay.Ticks,tsDifferentialAMRule.Ticks)//min of stop or rule
				-Math.Min(clockEventBreak.TimeDisplayed1.TimeOfDay.Ticks,tsDifferentialAMRule.Ticks)//min of start or rule
				);//equals the entire break, part of the break, or non of the break.
			//PM overlap==========================================================
			//Visual representation
			//PM Rule      :           X
			//Entire Break :o-------o  |             Rule-Rule   == No Break
			//Partial Break:      o----|---o         Stop-Rule   == Partial Break
			//No Break     :           |  o------o   Stop-Start  == Entire break
			retVal+=TimeSpan.FromTicks(
				Math.Max(clockEventBreak.TimeDisplayed2.TimeOfDay.Ticks,tsDifferentialPMRule.Ticks)//max of stop or rule
				-Math.Max(clockEventBreak.TimeDisplayed1.TimeOfDay.Ticks,tsDifferentialPMRule.Ticks)//max of start or rule
				);//equals the entire break, part of the break, or non of the break.
			return retVal;
		}

		///<summary>Returns true if two clock events overlap. Useful for determining if a break applies to a given clock event.  
		///Does not matter which order clock events are provided.</summary>
		private static bool timeClockEventsOverlapHelper(ClockEvent clockEvent1,ClockEvent clockEvent2) {
			//Visual representation
			//ClockEvent1:            o----------------o
			//ClockEvent2:o---------------o   or  o-------------------o
			if(clockEvent2.TimeDisplayed2>clockEvent1.TimeDisplayed1 
				&& clockEvent2.TimeDisplayed1<clockEvent1.TimeDisplayed2) {
					return true;
			}
			return false;
		}

		///<summary>Updates OTimeAuto, AdjustAuto (calculated and set above., and Rate2Auto based on the rules passed in, and calculated break time overages.</summary>
		private static void AdjustAutoClockEventEntriesHelper(List<ClockEvent> listClockEvent,List<ClockEvent> listClockEventBreak,TimeSpan tsDifferentialAMRule,TimeSpan tsDifferentialPMRule,TimeSpan tsOvertimeHoursRule) {
			for(int i=0;i<listClockEvent.Count;i++) {
				//listClockEvent[i].OTimeAuto	=TimeSpan.Zero;
				listClockEvent[i].AdjustAuto	=TimeSpan.Zero;
				listClockEvent[i].Rate2Auto		=TimeSpan.Zero;
				//OTimeAuto and AdjustAuto---------------------------------------------------------------------------------
				//if((listClockEvent[i].TimeDisplayed2.TimeOfDay-listClockEvent[i].TimeDisplayed1.TimeOfDay)>tsOvertimeHoursRule) {
				//	listClockEvent[i].OTimeAuto+=listClockEvent[i].TimeDisplayed2.TimeOfDay-listClockEvent[i].TimeDisplayed1.TimeOfDay-tsOvertimeHoursRule;
				//listClockEvent[i].AdjustAuto+=-listClockEvent[i].OTimeAuto;
				//}
				//AdjustAuto due to break overages-------------------------------------------------------------------------
				if(PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks)) {
					if(i==listClockEvent.Count-1 || listClockEvent[i].TimeDisplayed1.Date!=listClockEvent[i+1].TimeDisplayed1.Date) {//last item or last item for a given day.
						TimeSpan tsTotalBreaksToday=TimeSpan.Zero;
						for(int j=0;j<listClockEventBreak.Count;j++) {
							if(listClockEventBreak[j].TimeDisplayed1.Date!=listClockEvent[i].TimeDisplayed1.Date) {//skip breaks that occurred on different days.
								continue;
							}
							tsTotalBreaksToday+=listClockEventBreak[j].TimeDisplayed2.TimeOfDay-listClockEventBreak[j].TimeDisplayed1.TimeOfDay;
						}
						if(tsTotalBreaksToday>TimeSpan.FromMinutes(31)) {
							listClockEvent[i].AdjustAuto+=TimeSpan.FromMinutes(30)-tsTotalBreaksToday;//should add a negative time span.
							listClockEvent[i].OTimeAuto+=TimeSpan.FromMinutes(30)-tsTotalBreaksToday;//should add a negative time span.
							if(listClockEvent[i].OTimeAuto<TimeSpan.Zero) {//if we removed too much overbreak from otAuto, remove it from adjust auto instead and set otauto to zero
								listClockEvent[i].AdjustAuto+=listClockEvent[i].OTimeAuto;
								listClockEvent[i].OTimeAuto=TimeSpan.Zero;
							}
							tsTotalBreaksToday=TimeSpan.FromMinutes(30);//reset break today to 30 minutes, so next break is entirely overBreak.
						}
					}
				}
				//Rate2Auto-------------------------------------------------------------------------------------------------
				if(listClockEvent[i].TimeDisplayed1.TimeOfDay<tsDifferentialAMRule) {//AM, example rule before 8am, work from 5am to 7am
					listClockEvent[i].Rate2Auto+=tsDifferentialAMRule-listClockEvent[i].TimeDisplayed1.TimeOfDay;//8am-5am=3hrs
					if(listClockEvent[i].TimeDisplayed2.TimeOfDay<tsDifferentialAMRule) {
						listClockEvent[i].Rate2Auto+=listClockEvent[i].TimeDisplayed2.TimeOfDay-tsDifferentialAMRule;//8am-7am=-1hr =>2hrs total
					}
				}
				if(listClockEvent[i].TimeDisplayed2.TimeOfDay>tsDifferentialPMRule) {//PM, example diffRule after 8pm, work from 9 to 11pm. 
					listClockEvent[i].Rate2Auto+=listClockEvent[i].TimeDisplayed2.TimeOfDay-tsDifferentialPMRule;//11pm-8pm = 3hrs 
					if(listClockEvent[i].TimeDisplayed1.TimeOfDay>tsDifferentialPMRule) {
						listClockEvent[i].Rate2Auto+=tsDifferentialPMRule-listClockEvent[i].TimeDisplayed1.TimeOfDay;//8pm-9pm = -1hr =>2hrs total
					}
				}
				ClockEvents.Update(listClockEvent[i]);
			}//end ClockEvent list
		}

		///<summary>Deprecated.  This function is aesthetic and has no bearing on actual OT calculations. It adds adjustments to breaks so that when viewing them you can see if they went over 30 minutes.</summary>
		private static void AdjustBreaksHelper(Employee EmployeeCur,DateTime StartDate,DateTime StopDate) {
			if(!PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks)){
				//Only adjust breaks if preference is set.
				return;
			}
			List<ClockEvent> breakList=ClockEvents.Refresh(EmployeeCur.EmployeeNum,StartDate,StopDate,true);//PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),true);
			TimeSpan totalToday=TimeSpan.Zero;
			TimeSpan totalOne=TimeSpan.Zero;
			DateTime previousDate=DateTime.MinValue;
			for(int b=0;b<breakList.Count;b++) {
				if(breakList[b].TimeDisplayed2.Year<1880) {
					return;
				}
				if(breakList[b].TimeDisplayed1.Date != breakList[b].TimeDisplayed2.Date) {
					//MsgBox.Show(this,"Error. One break spans multiple dates.");
					return;
				}
				//calc time for the one break
				totalOne=breakList[b].TimeDisplayed2-breakList[b].TimeDisplayed1;
				//calc daily total
				if(previousDate.Date != breakList[b].TimeDisplayed1.Date) {//if date changed, this is the first pair of the day
					totalToday=TimeSpan.Zero;//new day
					previousDate=breakList[b].TimeDisplayed1.Date;//for the next loop
				}
				totalToday+=totalOne;
				//decide if breaks for the day went over 30 min.
				if(totalToday > TimeSpan.FromMinutes(31)) {//31 to prevent silly fractions less than 1.
					breakList[b].AdjustAuto=-(totalToday-TimeSpan.FromMinutes(30));
					ClockEvents.Update(breakList[b]);
					totalToday=TimeSpan.FromMinutes(30);//reset to 30.  Therefore, any additional breaks will be wholly adjustments.
				}
			}//end breaklist
		}

		///<summary>Calculates weekly overtime and inserts TimeAdjustments accordingly.</summary>
		public static void CalculateWeeklyOvertime_Old(Employee EmployeeCur,DateTime StartDate,DateTime StopDate) {
			List<TimeAdjust> TimeAdjustList=TimeAdjusts.Refresh(EmployeeCur.EmployeeNum,StartDate,StopDate);
			List<ClockEvent> ClockEventList=ClockEvents.Refresh(EmployeeCur.EmployeeNum,StartDate,StopDate,false);
			//first, delete all existing overtime entries
			for(int i=0;i<TimeAdjustList.Count;i++) {
				if(TimeAdjustList[i].OTimeHours==TimeSpan.Zero) {
					continue;
				}
				if(!TimeAdjustList[i].IsAuto) {
					continue;
				}
				TimeAdjusts.Delete(TimeAdjustList[i]);
				SecurityLogs.MakeLogEntry(Permissions.TimeAdjustEdit,0,
					$"Weekly overtime was calculated. Time Card Adjustment deleted for Employee: {Employees.GetNameFL(EmployeeCur)}.");
			}
			//refresh list after it has been cleaned up.
			TimeAdjustList=TimeAdjusts.Refresh(EmployeeCur.EmployeeNum,StartDate,StopDate);
			ArrayList mergedAL = new ArrayList();
			foreach(ClockEvent clockEvent in ClockEventList) {
				mergedAL.Add(clockEvent);
			}
			foreach(TimeAdjust timeAdjust in TimeAdjustList) {
				mergedAL.Add(timeAdjust);
			}
			//then, fill grid
			Calendar cal=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule rule=CalendarWeekRule.FirstFullWeek;//CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			//rule=CalendarWeekRule.FirstFullWeek;//CalendarWeekRule is an Enum. For these calculations, we want to use FirstFullWeek, not FirstDay;
			List<TimeSpan> WeeklyTotals = new List<TimeSpan>();
			WeeklyTotals = FillWeeklyTotalsHelper(true,EmployeeCur,mergedAL);
			//loop through all rows
			for(int i=0;i<mergedAL.Count;i++) {
				//ignore rows that aren't weekly totals
				if(i<mergedAL.Count-1//if not the last row
					//if the next row has the same week as this row
					&& cal.GetWeekOfYear(GetDateForRowHelper(mergedAL[i+1]),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//Default is 0-Sunday
					== cal.GetWeekOfYear(GetDateForRowHelper(mergedAL[i]),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))) {
					continue;
				}
				if(WeeklyTotals[i]<=TimeSpan.FromHours(40)) {
					continue;
				}
				//found a weekly total over 40 hours
				TimeAdjust adjust=new TimeAdjust();
				adjust.IsAuto=true;
				adjust.EmployeeNum=EmployeeCur.EmployeeNum;
				adjust.TimeEntry=GetDateForRowHelper(mergedAL[i]).AddHours(20);//puts it at 8pm on the same day.
				adjust.OTimeHours=WeeklyTotals[i]-TimeSpan.FromHours(40);
				adjust.RegHours=-adjust.OTimeHours;
				adjust.IsUnpaidProtectedLeave=false;
				adjust.SecuUserNumEntry=Security.CurUser.UserNum;
				TimeAdjusts.Insert(adjust);
				SecurityLogs.MakeLogEntry(Permissions.TimeAdjustEdit,0,
					$"Weekly overtime was calculated. Time Card Adjustment created for Employee: {Employees.GetNameFL(EmployeeCur)}.");
			}

		}

		///<summary>Calculates weekly overtime and inserts TimeAdjustments accordingly.</summary>
		public static void CalculateWeeklyOvertime(Employee EmployeeCur,DateTime StartDate,DateTime StopDate) {
			TimeCardRule timeCardRule=GetTimeCardRule(EmployeeCur);
			if(timeCardRule!=null && timeCardRule.IsOvertimeExempt) {
				return;
			}
			List<ClockEvent> listClockEvent=new List<ClockEvent>();
			List<TimeAdjust> listTimeAdjust=new List<TimeAdjust>();
			string errors="";
			string clockErrors="";
			string timeAdjustErrors="";
			//Fill lists and catch validation error messages------------------------------------------------------------------------------------------------------------
			try{listClockEvent=ClockEvents.GetValidList(EmployeeCur.EmployeeNum,StartDate,StopDate,false)	;}catch(Exception ex) {clockErrors+=ex.Message;}
			try{listTimeAdjust=TimeAdjusts.GetValidList(EmployeeCur.EmployeeNum,StartDate,StopDate)				;}catch(Exception ex) {timeAdjustErrors+=ex.Message;}
			//Report Errors---------------------------------------------------------------------------------------------------------------------------------------------
			errors=clockErrors+timeAdjustErrors;
			if(errors!="") {
				throw new Exception(Employees.GetNameFL(EmployeeCur)+" has the following errors:\r\n"+errors);
			}
			//first, delete all existing non manual overtime entries
			for(int i=0;i<listTimeAdjust.Count;i++) {
				if(listTimeAdjust[i].IsAuto) {
					TimeAdjusts.Delete(listTimeAdjust[i]);
					SecurityLogs.MakeLogEntry(Permissions.TimeAdjustEdit,0,
						$"Weekly overtime was calculated. Time Card Adjustment deleted for Employee: {Employees.GetNameFL(EmployeeCur)}.");
				}
			}
			//refresh list after it has been cleaned up.
			listTimeAdjust=TimeAdjusts.Refresh(EmployeeCur.EmployeeNum,StartDate,StopDate);
			ArrayList mergedAL=MergeClockEventAndTimeAdjust(listClockEvent,listTimeAdjust);
			//then, fill grid
			Calendar cal=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule rule=CalendarWeekRule.FirstFullWeek;//CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			//rule=CalendarWeekRule.FirstFullWeek;//CalendarWeekRule is an Enum. For these calculations, we want to use FirstFullWeek, not FirstDay;
			List<TimeSpan> WeeklyTotals = new List<TimeSpan>();
			WeeklyTotals = FillWeeklyTotalsHelper(true,EmployeeCur,mergedAL);
			//loop through all rows
			int weekIdx=0;//first week index==0
			for(int i=0;i<mergedAL.Count;i++) {
				//ignore rows that aren't weekly totals
				if(i<mergedAL.Count-1//if not the last row
					//if the next row has the same week as this row
					&& cal.GetWeekOfYear(GetDateForRowHelper(mergedAL[i+1]),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//Default is 0-Sunday
					== cal.GetWeekOfYear(GetDateForRowHelper(mergedAL[i]),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))) {
					continue;//continue within a single week
				}
				if(WeeklyTotals[i]<=TimeSpan.FromHours(40)) {
					weekIdx++;//Going to the next week, go to the next week's entry in the list of tuples
					continue;
				}
				//======CALUCLATE WEEKLY OVERTIME ADJUSTMENTS IF NEEDED======
				//found a weekly total over 40 hours
				List<Tuple<long,TimeSpan>> listEvents=_listEvents[weekIdx];//stores all worked hours per clinic in the order they were worked, sans dates, for a week.
				//validate the list of clock events.
				if(listEvents.GroupBy(x=>x.Item1).Any(x=>x.Select(y=>y.Item2.TotalHours).Sum()<0)) {
					//should never happen.
					throw new ApplicationException("Clock events for employee total a negative number of hours for a clinic.");
				}
				TimeSpan weeklyHours=TimeSpan.Zero;
				//this tracks each OT entry as it occurs, in the order it occurred, so that we can properly account for negative adjustments later.
				List<Tuple<long,TimeSpan>> listOTEntries=new List<Tuple<long, TimeSpan>>();
				Dictionary <long,TimeSpan> dictOvertime=new Dictionary<long, TimeSpan>();
				foreach(Tuple<long,TimeSpan> tupleCur in listEvents) {
					TimeSpan prevTotal=TimeSpan.FromHours(weeklyHours.TotalHours);//deep copy of timeSpan.
					weeklyHours=weeklyHours.Add(tupleCur.Item2);//add new timespan to running total.
					if(tupleCur.Item2<TimeSpan.Zero) { //negative span
						TimeSpan negTime=-tupleCur.Item2;//store as positive balance to simplify comparisons below.
						//First try to compensate "using up" Ot from this clinic
						for(int j=listOTEntries.Count-1;j>=0;j--) {
							if(listOTEntries[j].Item1!=tupleCur.Item1) {//only events for this clinic to start with.
								continue;
							}
							if(listOTEntries[j].Item2>negTime) {
								listOTEntries[j]=new Tuple<long,TimeSpan>(listOTEntries[j].Item1,listOTEntries[j].Item2.Subtract(negTime));
								negTime=TimeSpan.Zero;
								break;//we zeroed out the adjustment using only overtime from this clinic.
							}
							else {
								negTime=negTime.Subtract(listOTEntries[j].Item2);
								listOTEntries[j]=new Tuple<long,TimeSpan>(listOTEntries[j].Item1,listOTEntries[j].Item2.Subtract(listOTEntries[j].Item2));//zero it out.
							}
						}
						//houskeeping
						listOTEntries.RemoveAll(x=>x.Item2==TimeSpan.Zero);
						//possibly adjust off OT using time from other clinics in the order the time was accrued.
						for(int j=listOTEntries.Count-1;j>=0;j--) {
							if(negTime<=TimeSpan.Zero) {
								break;
							}
							if(listOTEntries[j].Item2>negTime) {
								listOTEntries[j]=new Tuple<long,TimeSpan>(listOTEntries[j].Item1,listOTEntries[j].Item2.Subtract(negTime));
								negTime=TimeSpan.Zero;
								break;//we zeroed out the adjustment using only overtime from this clinic.
							}
							else {
								negTime=negTime.Subtract(listOTEntries[j].Item2);
								listOTEntries[j]=new Tuple<long,TimeSpan>(listOTEntries[j].Item1,listOTEntries[j].Item2.Subtract(listOTEntries[j].Item2));//zero it out.
							}
						}
						//houskeeping
						listOTEntries.RemoveAll(x=>x.Item2==TimeSpan.Zero);
					}
					if(weeklyHours.TotalHours>40) {//this clock event put us into overtime.
						Tuple<long,TimeSpan> otEntry=new Tuple<long, TimeSpan>(tupleCur.Item1,TimeSpan.FromHours(weeklyHours.TotalHours-Math.Max(40,prevTotal.TotalHours)));
						if(otEntry.Item2>TimeSpan.Zero) {
							listOTEntries.Add(otEntry);
						}
					}
				}
				//Build dictOvertime by aggregating all entries in list above. Dict contains one entry per clinic with a timespan for OT time worked.
				dictOvertime=listOTEntries.GroupBy(x=>x.Item1).ToDictionary(x=>x.Key,x=>TimeSpan.FromHours(x.Select(y=>y.Item2.TotalHours).Sum()));
				//======ADD OT ADJUSTMENTS FOR ONE WEEK AFTER ALL CALCULATIONS FOR THAT WEEK HAVE BEEN COMPLETED======
				foreach(KeyValuePair<long,TimeSpan> kvp in dictOvertime) {
					if(kvp.Value<=TimeSpan.Zero) {
						continue;
					}
					TimeAdjust adjust=new TimeAdjust();
					adjust.IsAuto=true;
					adjust.EmployeeNum=EmployeeCur.EmployeeNum;
					adjust.TimeEntry=GetDateForRowHelper(mergedAL[i]).AddHours(20);
					adjust.OTimeHours=kvp.Value;
					adjust.RegHours=-adjust.OTimeHours;
					adjust.ClinicNum=kvp.Key;
					adjust.IsUnpaidProtectedLeave=false;
					adjust.SecuUserNumEntry=Security.CurUser.UserNum;
					TimeAdjusts.Insert(adjust);
					SecurityLogs.MakeLogEntry(Permissions.TimeAdjustEdit,0,
						$"Weekly overtime was calculated. Time Card Adjustment created for Employee: {Employees.GetNameFL(EmployeeCur)}.");
				}
				weekIdx++;
			}
		}

		///<summary>Merges a list of ClockEvent and a list of TimeAdjust, sorted by ClockEvent.TimeDisplayed1 and TimeAdjust.TimeEntry</summary>
		private static ArrayList MergeClockEventAndTimeAdjust(List<ClockEvent> listClockEvents,List<TimeAdjust> listTimeAdjusts) {
			List<ClockEvent> listOrderedClockEvents=listClockEvents.OrderBy(x => x.TimeDisplayed1).ToList();//Oldest entries first
			List<TimeAdjust> listOrderedTimeAdjusts=listTimeAdjusts.OrderBy(x => x.TimeEntry).ToList();
			ArrayList mergedAL=new ArrayList();
			int idxCE=0;
			int idxTA=0;
			while(idxCE<listOrderedClockEvents.Count || idxTA<listOrderedTimeAdjusts.Count) {//Merge listClockEvent and listTimeAdjust, sort by TimeDisplayed1/TimeEntry
				if(idxCE>listOrderedClockEvents.Count || idxTA>listOrderedTimeAdjusts.Count) {
						break;//Shouldn't happen.
				}
				if(idxCE==listOrderedClockEvents.Count) {//All ClockEvents added, so remaining TimeAdjusts will all be added.
					mergedAL.Add(listOrderedTimeAdjusts[idxTA]);
					idxTA++;
				}
				else if(idxTA==listOrderedTimeAdjusts.Count) {//All TimeAdjusts added, so remaining ClockEvents will all be added.
					mergedAL.Add(listOrderedClockEvents[idxCE]);//So add next ClockEvent
					idxCE++;
				}
				else if(listOrderedClockEvents[idxCE].TimeDisplayed1<=listOrderedTimeAdjusts[idxTA].TimeEntry) {//ClockEvent is next
					mergedAL.Add(listOrderedClockEvents[idxCE]);
					idxCE++;
				}
				else {//TimeAdjust is next
					mergedAL.Add(listOrderedTimeAdjusts[idxTA]);
					idxTA++;
				}
			}
			return mergedAL;
		}

		/// <summary>This was originally analogous to the FormTimeCard.FillGrid(), before this logic was moved to the business layer.</summary>
		private static List<TimeSpan> FillWeeklyTotalsHelper(bool fromDB,Employee EmployeeCur,ArrayList mergedAL) {
			List<Tuple<long,TimeSpan>> listWeek=new List<Tuple<long,TimeSpan>>();
			_listEvents=new List<List<Tuple<long,TimeSpan>>>();
			List<TimeSpan> retVal = new List<TimeSpan>();
			TimeSpan[] WeeklyTotals=new TimeSpan[mergedAL.Count];
			TimeSpan alteredSpan=TimeSpan.Zero;//used to display altered times
			TimeSpan oneSpan=TimeSpan.Zero;//used to sum one pair of clock-in/clock-out
			TimeSpan oneAdj;
			TimeSpan oneOT;
			TimeSpan daySpan=TimeSpan.Zero;//used for daily totals.
			TimeSpan weekSpan=TimeSpan.Zero;//used for weekly totals.
			if(mergedAL.Count>0) {
				weekSpan=ClockEvents.GetWeekTotal(EmployeeCur.EmployeeNum,GetDateForRowHelper(mergedAL[0]));
			}
			bool prevHours=weekSpan!=TimeSpan.Zero;
			TimeSpan periodSpan=TimeSpan.Zero;//used to add up totals for entire page. (Not used. Left over from when this code existed in the UI.)
			TimeSpan otspan=TimeSpan.Zero;//overtime for the entire period
			Calendar cal=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule rule=CalendarWeekRule.FirstFullWeek;// CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			DateTime curDate=DateTime.MinValue;
			DateTime previousDate=DateTime.MinValue;
			Type type;
			ClockEvent clock;
			TimeAdjust adjust;
			for(int i=0;i<mergedAL.Count;i++) {
				type=mergedAL[i].GetType();
				previousDate=curDate;
				//clock event row---------------------------------------------------------------------------------------------
				if(type==typeof(ClockEvent)) {
					clock=(ClockEvent)mergedAL[i];
					if(prevHours) {//Add in previous pay period's hours for this week if the week started in the middle of last payperiod.  Only need to do it once.
						listWeek.Add(Tuple.Create(clock.ClinicNum,weekSpan));
						prevHours=false;
					}
					curDate=clock.TimeDisplayed1.Date;
					if(clock.TimeDisplayed2.Year<1880) {
						//ignore clock event where user has not clocked out yet.
					}
					else {
						oneSpan=clock.TimeDisplayed2-clock.TimeDisplayed1;
						daySpan+=oneSpan;
						weekSpan+=oneSpan;
						periodSpan+=oneSpan;
						listWeek.Add(Tuple.Create(clock.ClinicNum,oneSpan));
					}
					//Adjust---------------------------------
					oneAdj=TimeSpan.Zero;
					if(clock.AdjustIsOverridden) {
						oneAdj+=clock.Adjust;
					}
					else {
						oneAdj+=clock.AdjustAuto;//typically zero
					}
					daySpan+=oneAdj;
					weekSpan+=oneAdj;
					periodSpan+=oneAdj;
					if(oneAdj!=TimeSpan.Zero) {//take adjustments from breaks away from the OT values in the dictionary
						listWeek.Add(Tuple.Create(clock.ClinicNum,oneAdj));
					}
					//Overtime------------------------------
					oneOT=TimeSpan.Zero;
					if(clock.OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
						oneOT=clock.OTimeHours;
					}
					else {
						oneOT=clock.OTimeAuto;//typically zero
					}
					otspan+=oneOT;
					daySpan-=oneOT;
					weekSpan-=oneOT;
					periodSpan-=oneOT;
					if(oneOT>TimeSpan.Zero) {
						listWeek.Add(Tuple.Create(clock.ClinicNum,-oneOT));
					}					
					//Daily-----------------------------------
					//if this is the last entry for a given date
					if(i==mergedAL.Count-1//if this is the last row
		        || GetDateForRowHelper(mergedAL[i+1]) != curDate)//or the next row is a different date
		      {
						daySpan=TimeSpan.Zero;
					}
					else {//not the last entry for the day
					}
					//Weekly-------------------------------------
					WeeklyTotals[i]=weekSpan;
					//if this is the last entry for a given week
					if(i==mergedAL.Count-1//if this is the last row 
		        || cal.GetWeekOfYear(GetDateForRowHelper(mergedAL[i+1]),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
		        != cal.GetWeekOfYear(clock.TimeDisplayed1.Date,rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
		      {
						_listEvents.Add(listWeek);
						listWeek=new List<Tuple<long,TimeSpan>>();//start over for the next week.
						weekSpan=TimeSpan.Zero;
					}
				}
				//adjustment row--------------------------------------------------------------------------------------
				else if(type==typeof(TimeAdjust)) {
					adjust=(TimeAdjust)mergedAL[i];
					curDate=adjust.TimeEntry.Date;
					if(!adjust.IsUnpaidProtectedLeave) {
						//Adjust------------------------------
						daySpan+=adjust.RegHours;//might be negative
						weekSpan+=adjust.RegHours;
						periodSpan+=adjust.RegHours;
						oneAdj=adjust.RegHours;
						if(oneAdj!=TimeSpan.Zero) {
							listWeek.Add(Tuple.Create(adjust.ClinicNum,oneAdj));
						}
						//Overtime------------------------------
						otspan+=adjust.OTimeHours;
						oneOT=adjust.OTimeHours;
						if(oneOT!=TimeSpan.Zero) {
							listWeek.Add(Tuple.Create(adjust.ClinicNum,oneOT));
						}
					}
					//Daily-----------------------------------
					//if this is the last entry for a given date
					if(i==mergedAL.Count-1//if this is the last row
		        || GetDateForRowHelper(mergedAL[i+1]) != curDate)//or the next row is a different date
		      {
						daySpan=new TimeSpan(0);
					}
					//Weekly-------------------------------------
					WeeklyTotals[i]=weekSpan;
					//if this is the last entry for a given week
					if(i==mergedAL.Count-1//if this is the last row 
		        || cal.GetWeekOfYear(GetDateForRowHelper(mergedAL[i+1]),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
		        != cal.GetWeekOfYear(adjust.TimeEntry.Date,rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
		      {
						_listEvents.Add(listWeek);
						listWeek=new List<Tuple<long,TimeSpan>>();//start over for the next week.
						weekSpan=new TimeSpan(0);
					}
				}
			}
			foreach(TimeSpan week in WeeklyTotals) {
				retVal.Add(week);
			}
			return retVal;
		}

		private static DateTime GetDateForRowHelper(object timeEvent) {
			if(timeEvent.GetType()==typeof(ClockEvent)) {
				return ((ClockEvent)timeEvent).TimeDisplayed1.Date;
			}
			else if(timeEvent.GetType()==typeof(TimeAdjust)) {
				return ((TimeAdjust)timeEvent).TimeEntry.Date;
			}
			return DateTime.MinValue;
		}
	}
}
