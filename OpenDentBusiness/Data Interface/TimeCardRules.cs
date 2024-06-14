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
		///<summary>One list of ClinicNumTimeSpans per week, each list of ClinicNumTimeSpans contains up to one entry per clinic, if employee clocked any time at that clinic.
		private static List<List<ClinicNumTimeSpan>> _listEvents;
		//==Jordan A list of lists is a bad pattern. Also, each item in the lists of lists shouldn't be pre-grouped.
		//The correct way to do it is a flat list with one entry per date/clinic.
		//Then, pull out lists at the last minute using LINQ and date ranges.

		private class ClinicNumTimeSpan {
			public long ClinicNum;
			public TimeSpan TimeSpan;
			//public DateTime Date;//Jordan need to add this so that we don't have to do any grouping at all.

			public ClinicNumTimeSpan(){
			}

			public ClinicNumTimeSpan(long clinicNum,TimeSpan timeSpan) {
				ClinicNum=clinicNum;
				TimeSpan=timeSpan;
			}
		}

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
		public static DataTable GetTableFromCache(bool refreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),refreshCache);
				_timeCardRuleCache.FillCacheFromTable(table);
				return table;
			}
			return _timeCardRuleCache.GetTableFromCache(refreshCache);
		}

		public static void ClearCache() {
			_timeCardRuleCache.ClearCache();
		}
		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<TimeCardRule> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TimeCardRule>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM timecardrule WHERE PatNum = "+POut.Long(patNum);
			return Crud.TimeCardRuleCrud.SelectMany(command);
		}

		///<summary>Gets one TimeCardRule from the db.</summary>
		public static TimeCardRule GetOne(long timeCardRuleNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<TimeCardRule>(MethodBase.GetCurrentMethod(),timeCardRuleNum);
			}
			return Crud.TimeCardRuleCrud.SelectOne(timeCardRuleNum);
		}*/

		///<summary></summary>
		public static long Insert(TimeCardRule timeCardRule){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				timeCardRule.TimeCardRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),timeCardRule);
				return timeCardRule.TimeCardRuleNum;
			}
			return Crud.TimeCardRuleCrud.Insert(timeCardRule);
		}
		
		///<summary></summary>
		public static void InsertMany(List<TimeCardRule> listTimeCardRules){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTimeCardRules);
				return;
			}
			Crud.TimeCardRuleCrud.InsertMany(listTimeCardRules);
		}

		///<summary></summary>
		public static void Update(TimeCardRule timeCardRule){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),timeCardRule);
				return;
			}
			Crud.TimeCardRuleCrud.Update(timeCardRule);
		}

		///<summary></summary>
		public static void Delete(long timeCardRuleNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTimeCardRuleNums);
				return;
			}
			string command="DELETE FROM timecardrule WHERE TimeCardRuleNum IN ("+string.Join(",",listTimeCardRuleNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Validates pay period before making any adjustments.
		///If today falls before the stopDate passed in, stopDate will be set to today's date.</summary>
		public static string ValidatePayPeriod(Employee employee,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//Middle tier check here just to save trips.  No call to the db.
				return Meth.GetString(MethodBase.GetCurrentMethod(),employee,dateStart,dateStop);
			}
			//If calculating breaks before the end date of the pay period, only calculate breaks and validate clock in and out events for days
			//before today.  Use the server time just because we are dealing with time cards.
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			ClockEvent clockEventLast=ClockEvents.GetLastEvent(employee.EmployeeNum);
			bool isStillWorking=(clockEventLast!=null && (clockEventLast.ClockStatus==TimeClockStatus.Break || clockEventLast.TimeDisplayed2.Year<1880));
			if(dateTimeNow.Date < dateStop.Date && isStillWorking) {
				dateStop=dateTimeNow.Date.AddDays(-1);
			}
			List<ClockEvent> listClockEventsBreak=ClockEvents.Refresh(employee.EmployeeNum,dateStart,dateStop,true);
			List<ClockEvent> listClockEvents=ClockEvents.Refresh(employee.EmployeeNum,dateStart,dateStop,false);
			bool hasError=false;
			string retVal="Time card errors for employee : "+Employees.GetNameFL(employee)+"\r\n";
			//Validate clock events
			for(int i=0;i<listClockEvents.Count;i++) {
				if(listClockEvents[i].TimeDisplayed2.Year<1880) {
					retVal+="  "+listClockEvents[i].TimeDisplayed1.ToShortDateString()+" : Employee not clocked out.\r\n";
					hasError=true;
				}
				else if(listClockEvents[i].TimeDisplayed1.Date!=listClockEvents[i].TimeDisplayed2.Date) {
					retVal+="  "+listClockEvents[i].TimeDisplayed1.ToShortDateString()+" : Clock entry spans multiple days.\r\n";
					hasError=true;
				}
			}
			//Validate Breaks
			for(int i=0;i<listClockEventsBreak.Count;i++) {
				if(listClockEventsBreak[i].TimeDisplayed2.Year<1880) {
					retVal+="  "+listClockEventsBreak[i].TimeDisplayed1.ToShortDateString()+" : Employee not clocked in from break.\r\n";
					hasError=true;
				}
				if(listClockEventsBreak[i].TimeDisplayed1.Date!=listClockEventsBreak[i].TimeDisplayed2.Date) {
					retVal+="  "+listClockEventsBreak[i].TimeDisplayed1.ToShortDateString()+" : One break spans multiple days.\r\n";
					hasError=true;
				}
				for(int c=listClockEvents.Count-1;c>=0;c--) {
					if(listClockEvents[c].TimeDisplayed1.Date==listClockEventsBreak[i].TimeDisplayed1.Date) {
						break;
					}
					if(c==0) { //we never found a match
						retVal+="  "+listClockEventsBreak[i].TimeDisplayed1.ToShortDateString()+" : Break found during non-working day.\r\n";
						hasError=true;
					}
				}
			}
			if(hasError) {
				return retVal;
			}
			return "";
		}

		///<summary>Cannot have both AM/PM rules and OverHours rules defined. 
		/// We no longer block having multiple rules defined. With a better interface we can improve some of this functionality. Per NS 09/15/2015.</summary>
		public static string ValidateOvertimeRules(List<long> listEmployeeNums=null) {
			StringBuilder stringBuilder=new StringBuilder();
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
				stringBuilder.AppendLine("Time card errors found for \"All Employees\":");
				stringBuilder.AppendLine("  Both a time of day rule and an over hours per day rule found. Only one or the other is allowed.");
				return stringBuilder.ToString();
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
					stringBuilder.AppendLine("Time card errors found for "+empName+":");
					stringBuilder.AppendLine("  Both a time of day rule and an over hours per day rule found. Only one or the other is allowed.\r\n");
				}
			}
			return stringBuilder.ToString();
		}

		///<summary>Clears automatic adjustment/adjustOT values and deletes automatic TimeAdjusts for period.</summary>
		public static void ClearAuto(long employeeNum,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employeeNum,dateStart,dateStop);
				return;
			}
			List<ClockEvent> listClockEvents=ClockEvents.GetSimpleList(employeeNum,dateStart,dateStop);
			for(int i=0;i<listClockEvents.Count;i++) {
				listClockEvents[i].AdjustAuto=TimeSpan.Zero;
				listClockEvents[i].OTimeAuto=TimeSpan.Zero;
				listClockEvents[i].Rate2Auto=TimeSpan.Zero;
				listClockEvents[i].Rate3Auto=TimeSpan.Zero;
				ClockEvents.Update(listClockEvents[i]);
			}
			List<TimeAdjust> listTimeAdjusts=TimeAdjusts.GetSimpleListAuto(employeeNum,dateStart,dateStop);
			for(int i=0;i<listTimeAdjusts.Count;i++) {
				TimeAdjusts.Delete(listTimeAdjusts[i]);
				SecurityLogs.MakeLogEntry(EnumPermType.TimeAdjustEdit,0,
					$"Automatic Time Card Adjustments were cleared. Adjustment deleted for Employee: {Employees.GetNameFL(listTimeAdjusts[i].EmployeeNum)}.");
			}
		}

		///<summary>Clears all manual adjustments/Adjust OT values from clock events. Does not alter adjustments to clockevent.TimeDisplayed1/2 nor does it delete or alter any TimeAdjusts.</summary>
		public static void ClearManual(long employeeNum,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employeeNum,dateStart,dateStop);
				return;
			}
			List<ClockEvent> listClockEvents=ClockEvents.GetSimpleList(employeeNum,dateStart,dateStop);
			for(int i=0;i<listClockEvents.Count;i++) {
				listClockEvents[i].Adjust=TimeSpan.Zero;
				listClockEvents[i].AdjustIsOverridden=false;
				listClockEvents[i].OTimeHours=TimeSpan.FromHours(-1);
				listClockEvents[i].Rate2Hours=TimeSpan.FromHours(-1);
				listClockEvents[i].Rate3Hours=TimeSpan.FromHours(-1);
				ClockEvents.Update(listClockEvents[i]);
			}
		}

		///<summary>Validates list and throws exceptions. Always returns a value. Creates a timecard rule based on all applicable timecard rules for a given employee.</summary>
		public static TimeCardRule GetTimeCardRule(Employee employee) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<TimeCardRule>(MethodBase.GetCurrentMethod(),employee);
			}
			//Validate Rules---------------------------------------------------------------------------------------------------------------
			string errors=TimeCardRules.ValidateOvertimeRules(new List<long> {employee.EmployeeNum});
			if(errors.Length>0) {
				throw new Exception(errors);
			}
			//Build return value ----------------------------------------------------------------------------------------------------------
			List<TimeCardRule> listTimeCardRulesEmp=GetWhere(x => x.EmployeeNum==0 || x.EmployeeNum==employee.EmployeeNum);
			TimeCardRule timeCardRuleAm=listTimeCardRulesEmp.Where(x => x.BeforeTimeOfDay>TimeSpan.Zero).OrderByDescending(x => x.BeforeTimeOfDay).FirstOrDefault();
			TimeCardRule timeCardRulePm=listTimeCardRulesEmp.Where(x => x.AfterTimeOfDay>TimeSpan.Zero).OrderBy(x => x.AfterTimeOfDay).FirstOrDefault();
			TimeCardRule timeCardRuleHours=listTimeCardRulesEmp.Where(x => x.OverHoursPerDay>TimeSpan.Zero).OrderBy(x => x.OverHoursPerDay).FirstOrDefault();
			TimeCardRule timeCardRuleIsOvertimeExempt=listTimeCardRulesEmp.Where(x => x.IsOvertimeExempt).FirstOrDefault();
			TimeCardRule timeCardRuleIsWeekendRate3=listTimeCardRulesEmp.Where(x => x.HasWeekendRate3).FirstOrDefault();
			TimeCardRule timeCardRule=new TimeCardRule();
			if(timeCardRuleAm!=null) {
				timeCardRule.BeforeTimeOfDay=timeCardRuleAm.BeforeTimeOfDay;
			}
			if(timeCardRulePm!=null) {
				timeCardRule.AfterTimeOfDay=timeCardRulePm.AfterTimeOfDay;
			}
			if(timeCardRuleHours!=null) {
				timeCardRule.OverHoursPerDay=timeCardRuleHours.OverHoursPerDay;
			}
			if(timeCardRuleIsOvertimeExempt!=null) {
				timeCardRule.IsOvertimeExempt=timeCardRuleIsOvertimeExempt.IsOvertimeExempt;
			}
			if(timeCardRuleIsWeekendRate3!=null) {
				timeCardRule.HasWeekendRate3=timeCardRuleIsWeekendRate3.HasWeekendRate3;
			}
			return timeCardRule;
		}

		///<summary>Calculates daily overtime. Throws exceptions when encountering errors, though all errors SHOULD have been caught already by using the ValidatePayPeriod() function and generating a msgbox.</summary>
		public static void CalculateDailyOvertime_Old(Employee employee,DateTime dateStart,DateTime dateStop) {
			DateTime datePrevious;
			List<ClockEvent> listClockEvents=ClockEvents.Refresh(employee.EmployeeNum,dateStart,dateStop,false);//PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),IsBreaks);
			//Over breaks-------------------------------------------------------------------------------------------------
			if(PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks)) {
				//set adj auto to zero for all.
				for(int i=0;i<listClockEvents.Count;i++) {
					listClockEvents[i].AdjustAuto=TimeSpan.Zero;
					ClockEvents.Update(listClockEvents[i]);
				}
				List<ClockEvent> listClockEventsBreak=ClockEvents.Refresh(employee.EmployeeNum,dateStart,dateStop,true);//PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),true);
				TimeSpan timeSpanTotalToday=TimeSpan.Zero;
				TimeSpan timeSpanTotalOne=TimeSpan.Zero;
				datePrevious=DateTime.MinValue;
				for(int b=0;b<listClockEventsBreak.Count;b++) {
					if(listClockEventsBreak[b].TimeDisplayed2.Year<1880) {
						throw new Exception("Error. Employee break malformed.");
					}
					if(listClockEventsBreak[b].TimeDisplayed1.Date != listClockEventsBreak[b].TimeDisplayed2.Date) {
						throw new Exception("Error. One break spans multiple dates.");
					}
					//calc time for the one break
					timeSpanTotalOne=listClockEventsBreak[b].TimeDisplayed2-listClockEventsBreak[b].TimeDisplayed1;
					//calc daily total
					if(datePrevious.Date != listClockEventsBreak[b].TimeDisplayed1.Date) {//if date changed, this is the first pair of the day
						timeSpanTotalToday=TimeSpan.Zero;//new day
						datePrevious=listClockEventsBreak[b].TimeDisplayed1.Date;//for the next loop
					}
					timeSpanTotalToday+=timeSpanTotalOne;
					//decide if breaks for the day went over 30 min.
					if(timeSpanTotalToday > TimeSpan.FromMinutes(31)) {//31 to prevent silly fractions less than 1.
						//loop through all ClockEvents in this grid to find one to adjust.
						//Go backwards to find the last entry for a given date.
						for(int c=listClockEvents.Count-1;c>=0;c--) {
							if(listClockEvents[c].TimeDisplayed1.Date==listClockEventsBreak[b].TimeDisplayed1.Date) {
								listClockEvents[c].AdjustAuto-=(timeSpanTotalToday-TimeSpan.FromMinutes(30));
								ClockEvents.Update(listClockEvents[c]);
								timeSpanTotalToday=TimeSpan.FromMinutes(30);//reset to 30.  Therefore, any additional breaks will be wholly adjustments.
								break;
							}
							if(c==0) {//we never found a match
								throw new Exception("Error. Over breaks, but could not adjust because not regular time entered for date:"
									+listClockEventsBreak[b].TimeDisplayed1.Date.ToShortDateString());
							}
						}
					}
				}
			}
			//OT-------------------------------------------------------------------------------------------------------------
			TimeCardRule timeCardRuleAfter=null;
			TimeCardRule timeCardRuleBefore=null;
			TimeCardRule timeCardRuleOverHours=null;
			//loop through timecardrules to find one rule of each kind.
			List<TimeCardRule> listTimeCardRules=GetDeepCopy();
			for(int i=0;i<listTimeCardRules.Count;i++) {
				if(listTimeCardRules[i].EmployeeNum!=0 && listTimeCardRules[i].EmployeeNum!=employee.EmployeeNum) {
					continue;
				}
				if(listTimeCardRules[i].AfterTimeOfDay > TimeSpan.Zero) {
					if(timeCardRuleAfter != null) {//already found a match, and this is a second match
						throw new Exception("Error.  Multiple matches of AfterTimeOfDay found for this employee.  Only one allowed.");
						//return;
					}
					timeCardRuleAfter=listTimeCardRules[i];
				}
				else if(listTimeCardRules[i].OverHoursPerDay > TimeSpan.Zero) {
					if(timeCardRuleOverHours != null) {//already found a match, and this is a second match
						throw new Exception("Error.  Multiple matches of OverHoursPerDay found for this employee.  Only one allowed.");
						//return;
					}
					timeCardRuleOverHours=listTimeCardRules[i];
				}
				if(timeCardRuleAfter!= null && timeCardRuleOverHours != null) {
					throw new Exception("Error.  Both an OverHoursPerDay and an AfterTimeOfDay found for this employee.  Only one or the other is allowed.");
					//return;
				}
				if(timeCardRuleBefore != null && timeCardRuleOverHours != null) {
					throw new Exception("Error.  Both an OverHoursPerDay and an BeforeTimeOfDay found for this employee.  Only one or the other is allowed.");
					//return;
				}
				if(listTimeCardRules[i].BeforeTimeOfDay > TimeSpan.Zero) {
					if(timeCardRuleBefore != null) {//already found a match, and this is a second match
						throw new Exception("Error.  Multiple matches of BeforeTimeOfDay found for this employee.  Only one allowed.");
						//return;
					}
					timeCardRuleBefore=listTimeCardRules[i];
				}
			}
			//loop through all ClockEvents in this grid.
			TimeSpan timeSpanDailyTotal=TimeSpan.Zero;
			TimeSpan timeSpanPairTotal=TimeSpan.Zero;
			datePrevious=DateTime.MinValue;
			for(int i=0;i<listClockEvents.Count;i++) {
				if(listClockEvents[i].TimeDisplayed2.Year<1880) {
					throw new Exception("Error. Employee not clocked out.");
					//return;
				}
				if(listClockEvents[i].TimeDisplayed1.Date != listClockEvents[i].TimeDisplayed2.Date) {
					throw new Exception("Error. One clock pair spans multiple dates.");
					//return;
				}
				timeSpanPairTotal=listClockEvents[i].TimeDisplayed2-listClockEvents[i].TimeDisplayed1;
				//add any adjustments, manual or overrides.
				if(listClockEvents[i].AdjustIsOverridden) {
					timeSpanPairTotal+=listClockEvents[i].Adjust;
				}
				else {
					timeSpanPairTotal+=listClockEvents[i].AdjustAuto;
				}
				//calc daily total
				if(datePrevious.Date != listClockEvents[i].TimeDisplayed1.Date) { //if date changed
					timeSpanDailyTotal=TimeSpan.Zero;//new day
					datePrevious=listClockEvents[i].TimeDisplayed1.Date;//for the next loop
				}
				timeSpanDailyTotal+=timeSpanPairTotal;
				//handle OT
				listClockEvents[i].OTimeAuto=TimeSpan.Zero;//set auto OT to zero.
				if(listClockEvents[i].OTimeHours != TimeSpan.FromHours(-1)) {//if OT is overridden
					//don't try to calc a time.
					ClockEvents.Update(listClockEvents[i]);//just to possibly clear autoOT, even though it doesn't count.
					//but still need to subtract OT from dailyTotal
					timeSpanDailyTotal-=listClockEvents[i].OTimeHours;
					continue;
				}
				if(timeCardRuleAfter != null) {
					//test to see if this span is after specified time
					TimeSpan timeSpanAfter=TimeSpan.Zero;
					if(listClockEvents[i].TimeDisplayed1.TimeOfDay > timeCardRuleAfter.AfterTimeOfDay) {//the start time is after time, so the whole pairTotal is OT
						timeSpanAfter=timeSpanPairTotal;
					}
					else if(listClockEvents[i].TimeDisplayed2.TimeOfDay > timeCardRuleAfter.AfterTimeOfDay) {//only the second time is after time
						timeSpanAfter=listClockEvents[i].TimeDisplayed2.TimeOfDay-timeCardRuleAfter.AfterTimeOfDay;//only a portion of the pairTotal is OT
					}
					listClockEvents[i].OTimeAuto=timeSpanAfter;
				}
				if(timeCardRuleBefore != null) {
					//test to see if this span is after specified time
					TimeSpan timeSpanBefore=TimeSpan.Zero;
					if(listClockEvents[i].TimeDisplayed2.TimeOfDay < timeCardRuleBefore.BeforeTimeOfDay) {//the end time is before time, so the whole pairTotal is OT
						timeSpanBefore+=timeSpanPairTotal;
					}
					else if(listClockEvents[i].TimeDisplayed1.TimeOfDay < timeCardRuleBefore.BeforeTimeOfDay) {//only the first time is before time
						timeSpanBefore+=timeCardRuleBefore.BeforeTimeOfDay-listClockEvents[i].TimeDisplayed1.TimeOfDay;//only a portion of the pairTotal is OT
					} 
					listClockEvents[i].OTimeAuto+=timeSpanBefore;
				}
				if(timeCardRuleOverHours != null) {
					//test dailyTotal
					TimeSpan timeSpanOverHours=TimeSpan.Zero;
					if(timeSpanDailyTotal > timeCardRuleOverHours.OverHoursPerDay) {
						timeSpanOverHours=timeSpanDailyTotal-timeCardRuleOverHours.OverHoursPerDay;
						timeSpanDailyTotal=timeCardRuleOverHours.OverHoursPerDay;//e.g. reset to 8.  Any further pairs on this date will be wholly OT
						listClockEvents[i].OTimeAuto+=timeSpanOverHours;
					}
				}
				ClockEvents.Update(listClockEvents[i]);
			}
			AdjustBreaksHelper(employee,dateStart,dateStop);
		}

		///<summary>Calculates daily overtime.  Daily overtime does not take into account any time adjust events.
		///All manually entered time adjust events are assumed to be entered correctly and should not be used in calculating automatic totals.
		///Throws exceptions when encountering errors.</summary>
		public static void CalculateDailyOvertime(Employee employee,DateTime dateStart,DateTime dateStop) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//Middle tier check here just to save trips.  No call to the db.
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employee,dateStart,dateStop);
				return;
			}
			#region Fill Lists, validate data sets, generate error messages.
			List<ClockEvent> listClockEvents=new List<ClockEvent>();
			List<ClockEvent> listClockEventsBreak=new List<ClockEvent>();
			TimeCardRule timeCardRule=new TimeCardRule();
			string errors="";
			string clockErrors="";
			string breakErrors="";
			string ruleErrors="";
			//If calculating breaks before the end date of the pay period, only calculate breaks and validate clock in and out events for days
			//before today.  Use the server time just because we are dealing with time cards.
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			ClockEvent clockEventLast=ClockEvents.GetLastEvent(employee.EmployeeNum);
			bool isStillWorking=(clockEventLast!=null && (clockEventLast.ClockStatus==TimeClockStatus.Break || clockEventLast.TimeDisplayed2.Year<1880));
			if(dateTimeNow.Date < dateStop.Date && isStillWorking) {
				dateStop=dateTimeNow.Date.AddDays(-1);
			}
			//Fill lists and catch validation error messages------------------------------------------------------------------------------------------------------------
			try {
				listClockEvents=ClockEvents.GetValidList(employee.EmployeeNum,dateStart,dateStop,false);
			}
			catch(Exception ex) {
				clockErrors+=ex.Message;
			}
			try {
				listClockEventsBreak=ClockEvents.GetValidList(employee.EmployeeNum,dateStart,dateStop,true);
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
			for(int b=0;b<listClockEventsBreak.Count;b++) {
				bool isValidBreak=false;
				for(int c=0;c<listClockEvents.Count;c++) {
					if(timeClockEventsOverlapHelper(listClockEventsBreak[b],listClockEvents[c])) {
						if(listClockEventsBreak[b].TimeDisplayed1>listClockEvents[c].TimeDisplayed1//break started after work did
							&& listClockEventsBreak[b].TimeDisplayed2<listClockEvents[c].TimeDisplayed2)//break ended before working hours
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
				breakErrors+="  "+listClockEventsBreak[b].TimeDisplayed1.ToString()+" : break found during non-working hours.\r\n";//ToString() instead of ToShortDateString() to show time.
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
			TimeSpan timeSpanHoursWorkedTotal  =new TimeSpan()      ;
			TimeSpan timeSpanOTHoursRule =new TimeSpan(24,0,0);//Example 10:00 for overtime rule after 10 hours per day.
			TimeSpan timeSpanRate2AMRule=new TimeSpan()      ;//Example 06:00 for Rate2 rule before 6am.
			TimeSpan timeSpanRate2PMRule=new TimeSpan(24,0,0);//Example 17:00 for Rate2 rule after  5pm.
			//Fill over hours rule from list-------------------------------------------------------------------------------------
			if(timeCardRule.OverHoursPerDay!=TimeSpan.Zero) {//OverHours Rule
				timeSpanOTHoursRule=timeCardRule.OverHoursPerDay;//at most, one non-zero OverHours rule available at this point.
			}
			if(timeCardRule.BeforeTimeOfDay!=TimeSpan.Zero) {//AM Rule
				timeSpanRate2AMRule=timeCardRule.BeforeTimeOfDay;//at most, one non-zero AM rule available at this point.
			}
			if(timeCardRule.AfterTimeOfDay!=TimeSpan.Zero) {//PM Rule
				timeSpanRate2PMRule=timeCardRule.AfterTimeOfDay;//at most, one non-zero PM rule available at this point.
			}
			#endregion
			//Calculations: Regular Time, Overtime, Rate2, Rate3 time---------------------------------------------------------------------------------------------------
			TimeSpan timeSpanDailyBreaksAdjustTotal=new TimeSpan();//used to adjust the clock event
			TimeSpan timeSpanDailyBreaksTotal=new TimeSpan();//used in calculating breaks per day.
			TimeSpan timeSpanDailyBreaksCalc=new TimeSpan();//used in calculating breaks per day.
			TimeSpan timeSpanDailyHoursMinusBreaksTotal=new TimeSpan();//used in calculating breaks per day.
			TimeSpan timeSpanDailyRate2Total=new TimeSpan(); //hours before and after AM/PM Rate2 rules. Adjusted for overbreaks.
			TimeSpan timeSpanDailyRate3Total=new TimeSpan(); //hours worked on weekend for Rate3. Adjusted for overbreaks.
			//Note: If TimeCardsMakesAdjustmentsForOverBreaks is true, only the first 30 minutes of break per day are paid. 
			//All breaktime thereafter will be calculated as if the employee was clocked out at that time.
			for(int i=0;i<listClockEvents.Count;i++) {
				#region  Rate3 pay (including overbreak adjustments)--------------------------------------------------------------
				//Determine if using Rate3 rule and hours worked are on the weekend.
				//Rate3 overrides Rate2, meaning any hours worked on the weekend are earned at Rate3 even if working before Rate2 AM rule or after Rate2 PM rule.
				if(timeCardRule.HasWeekendRate3 &&
					(  (listClockEvents[i].TimeDisplayed1.DayOfWeek==DayOfWeek.Saturday && listClockEvents[i].TimeDisplayed2.DayOfWeek==DayOfWeek.Saturday)
					|| (listClockEvents[i].TimeDisplayed1.DayOfWeek==DayOfWeek.Sunday   && listClockEvents[i].TimeDisplayed2.DayOfWeek==DayOfWeek.Sunday  )))
				{
					//All time worked on weekend qualifies for Rate3
					timeSpanDailyRate3Total=listClockEvents[i].TimeDisplayed2-listClockEvents[i].TimeDisplayed1;
					TimeSpan timeSpanRate3BreakTimeCounter=new TimeSpan();
					//Subtract overbreaks
					for(int b=0;b<listClockEventsBreak.Count;b++) {
						//sum breaks that occur during this clock event
						if(timeClockEventsOverlapHelper(listClockEvents[i],listClockEventsBreak[b])) {
							timeSpanRate3BreakTimeCounter+=listClockEventsBreak[b].TimeDisplayed2-listClockEventsBreak[b].TimeDisplayed1;
						}
					}
					TimeSpan timeSpanRate3AdjustAmount=timeSpanRate3BreakTimeCounter-System.TimeSpan.FromMinutes(30); //Overbreak
					if(timeSpanRate3AdjustAmount>TimeSpan.Zero) {
						timeSpanDailyRate3Total-=timeSpanRate3AdjustAmount;
					}
					if(timeSpanDailyRate3Total<TimeSpan.Zero) {
						//this should never happen. If it ever does, we need to know about it, because that means some math has been miscalculated.
						throw new Exception(" - "+listClockEvents[i].TimeDisplayed1.Date.ToShortDateString()+", "+employee.FName+" "+employee.LName+" : calculated Rate3 hours was negative.");
					}
					listClockEvents[i].Rate3Auto=timeSpanDailyRate3Total;//should be zero or greater.
					listClockEvents[i].Rate2Auto=new TimeSpan(0); //No ClockEvent can have both Rate2 and Rate3 hours. Zero out in case modifications were made.
				}
				#endregion
				#region  Rate2 pay (including overbreak adjustments)--------------------------------------------------------------
				else {
					if(i==0||listClockEvents[i].TimeDisplayed1.Date!=listClockEvents[i-1].TimeDisplayed1.Date) {
						timeSpanDailyRate2Total=TimeSpan.Zero;
					}
					//AM-----------------------------------
					if(listClockEvents[i].TimeDisplayed1.TimeOfDay<timeSpanRate2AMRule) {//clocked in before Rate2 AM rule
						timeSpanDailyRate2Total+=timeSpanRate2AMRule-listClockEvents[i].TimeDisplayed1.TimeOfDay;
						if(listClockEvents[i].TimeDisplayed2.TimeOfDay<timeSpanRate2AMRule) {//clocked out before Rate2 AM rule also
							timeSpanDailyRate2Total+=listClockEvents[i].TimeDisplayed2.TimeOfDay-timeSpanRate2AMRule;//add a negative timespan
						}
						//Adjust Rate2 AM by overbreaks-----
						TimeSpan timeSpanAMBreakTimeCounter=new TimeSpan();//tracks all break time for use in calculating overages.
						TimeSpan timeSpanAMBreakDuringRate2=new TimeSpan();//tracks only the portion of breaks that occurred during Rate2 hours.
						for(int b = 0;b<listClockEventsBreak.Count;b++) {
							if(timeSpanAMBreakTimeCounter>TimeSpan.FromMinutes(30)) {
								timeSpanAMBreakTimeCounter=TimeSpan.FromMinutes(30);//reset overages for next calculation.
							}
							if(listClockEventsBreak[b].TimeDisplayed1.Date!=listClockEvents[i].TimeDisplayed1.Date) {
								continue;//skip breaks for other days.
							}
							timeSpanAMBreakTimeCounter+=listClockEventsBreak[b].TimeDisplayed2-listClockEventsBreak[b].TimeDisplayed1;
							timeSpanAMBreakDuringRate2+=calcRate2Portion(timeSpanRate2AMRule,TimeSpan.FromHours(24),listClockEventsBreak[b]);
							if(timeSpanAMBreakTimeCounter<TimeSpan.FromMinutes(30)) {
								continue;//not over thirty minutes yet.
							}
							if(timeClockEventsOverlapHelper(listClockEvents[i],listClockEventsBreak[b])) {
								continue;//There must be multiple clock events for this day, and we have gone over breaks during a later clock event period
							}
							if(listClockEventsBreak[b].TimeDisplayed1.TimeOfDay>timeSpanRate2AMRule) {
								continue;//this break started after the Rate2 AM rule so there is nothing left to do in this loop. break out of the entire loop.
							}
							if(listClockEventsBreak[b].TimeDisplayed2.TimeOfDay-(timeSpanAMBreakTimeCounter-TimeSpan.FromMinutes(30))>timeSpanRate2AMRule) {
								continue;//entirety of break overage occurred after Rate2 AM rule time.
							}
							//Make adjustments because: 30+ minutes of break, break occurred during clockEvent, break started before the AM rule.
							TimeSpan timeSpanAMAdjustAmount=System.TimeSpan.FromMinutes(30)-timeSpanAMBreakTimeCounter;
							if(timeSpanAMAdjustAmount<-timeSpanAMBreakDuringRate2) {
								timeSpanAMAdjustAmount=-timeSpanAMBreakDuringRate2;//cannot adjust off more break overage time than we have had breaks during this time.
							}
							timeSpanDailyRate2Total+=timeSpanAMAdjustAmount;//adjust down
							timeSpanAMBreakDuringRate2+=timeSpanAMAdjustAmount;//adjust down
						}
					}
					//PM-------------------------------------
					if(listClockEvents[i].TimeDisplayed2.TimeOfDay>timeSpanRate2PMRule) {//clocked out after Rate2 PM rule
						timeSpanDailyRate2Total+=listClockEvents[i].TimeDisplayed2.TimeOfDay-timeSpanRate2PMRule;
						if(listClockEvents[i].TimeDisplayed1.TimeOfDay>timeSpanRate2PMRule) {//clocked in after Rate2 PM rule also
							timeSpanDailyRate2Total+=timeSpanRate2PMRule-listClockEvents[i].TimeDisplayed1.TimeOfDay;//add a negative timespan
						}
						//Adjust Rate2 PM by overbreaks-----
						TimeSpan timeSpanPMBreakTimeCounter=new TimeSpan();//tracks all break time for use in calculating overages.
						TimeSpan timeSpanPMBreakDuringRate2=new TimeSpan();//tracks only the portion of breaks that occurred during Rate2 hours.
						for(int b = 0;b<listClockEventsBreak.Count;b++) {
							if(timeSpanPMBreakTimeCounter>TimeSpan.FromMinutes(30)) {
								timeSpanPMBreakTimeCounter=TimeSpan.FromMinutes(30);//reset overages for next calculation.
							}
							if(listClockEventsBreak[b].TimeDisplayed1.Date!=listClockEvents[i].TimeDisplayed1.Date) {
								continue;//skip breaks for other days.
							}
							timeSpanPMBreakTimeCounter+=listClockEventsBreak[b].TimeDisplayed2-listClockEventsBreak[b].TimeDisplayed1;
							timeSpanPMBreakDuringRate2+=calcRate2Portion(TimeSpan.Zero,timeSpanRate2PMRule,listClockEventsBreak[b]);
							if(timeSpanPMBreakTimeCounter<TimeSpan.FromMinutes(30)) {
								continue;//not over thirty minutes yet.
							}
							if(!timeClockEventsOverlapHelper(listClockEvents[i],listClockEventsBreak[b])) {
								continue;//There must be multiple clock events for this day, and we have gone over breaks during a different clock event period
							}
							if(listClockEventsBreak[b].TimeDisplayed2.TimeOfDay<timeSpanRate2PMRule) {
								continue;//entirety of break overage occurred before Rate2 PM time.
							}
							//Make adjustments because: 30+ minutes of break, break occurred during clockEvent, break ended after the PM rule.
							TimeSpan timeSpanPMAdjustAmount=System.TimeSpan.FromMinutes(30)-timeSpanPMBreakTimeCounter;//tsPMBreakTimeCounter is always > 30 at this point in time
							if(timeSpanPMAdjustAmount<-timeSpanPMBreakDuringRate2) {
								timeSpanPMAdjustAmount=-timeSpanPMBreakDuringRate2;//cannot adjust off more break overage time than we have had breaks during this time.
							}
							timeSpanDailyRate2Total+=timeSpanPMAdjustAmount;//adjust down
							timeSpanPMBreakDuringRate2+=timeSpanPMAdjustAmount;//adjust down
						}
					}
					//Apply Rate2 to clock event-----------------------------------------------------------------------------------
					if(timeSpanDailyRate2Total<TimeSpan.Zero) {
						//this should never happen. If it ever does, we need to know about it, because that means some math has been miscalculated.
						throw new Exception(" - "+listClockEvents[i].TimeDisplayed1.Date.ToShortDateString()+", "+employee.FName+" "+employee.LName+" : calculated Rate2 hours was negative.");
					}
					listClockEvents[i].Rate2Auto=timeSpanDailyRate2Total;//should be zero or greater.
					listClockEvents[i].Rate3Auto=new TimeSpan(0); //No ClockEvent can have both Rate2 and Rate3 hours. Zero out in case modifications were made.
				}
				#endregion
				#region Regular hours and OT hours calulations (including overbreak adjustments)----------------------------------------
				listClockEvents[i].OTimeAuto	=TimeSpan.Zero;
				listClockEvents[i].AdjustAuto=TimeSpan.Zero;
				if(i==0 || listClockEvents[i].TimeDisplayed1.Date!=listClockEvents[i-1].TimeDisplayed1.Date) {
					timeSpanHoursWorkedTotal=TimeSpan.Zero;
					timeSpanDailyBreaksAdjustTotal=TimeSpan.Zero;
					timeSpanDailyBreaksCalc=TimeSpan.Zero;
					timeSpanDailyBreaksTotal=TimeSpan.Zero;
					timeSpanDailyRate2Total=TimeSpan.Zero;
					timeSpanDailyRate3Total=TimeSpan.Zero;
				}
				timeSpanHoursWorkedTotal+=listClockEvents[i].TimeDisplayed2-listClockEvents[i].TimeDisplayed1;//Hours worked
				if(timeSpanHoursWorkedTotal>timeSpanOTHoursRule) {//if OverHoursPerDay then make AutoOTAdjustments.
					listClockEvents[i].OTimeAuto	+=timeSpanHoursWorkedTotal-timeSpanOTHoursRule;//++OTimeAuto
					//listClockEvent[i].AdjustAuto-=tsHoursWorkedTotal-tsOvertimeHoursRule;//--AdjustAuto
					timeSpanHoursWorkedTotal=timeSpanOTHoursRule;//subsequent clock events should be counted as overtime.
				}
				if(i==listClockEvents.Count-1 || listClockEvents[i].TimeDisplayed1.Date!=listClockEvents[i+1].TimeDisplayed1.Date) {
					//Either the last clock event in the list or last clock event for the day.
					//OVERBREAKS--------------------------------------------------------------------------------------------------------
					if(PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks)) {//Apply overbreaks to this clockEvent.
						timeSpanDailyBreaksAdjustTotal=new TimeSpan();//used to adjust the clock event
						timeSpanDailyBreaksTotal=new TimeSpan();//used in calculating Daily Hours.
						timeSpanDailyBreaksCalc=new TimeSpan();//used in calculating breaks.
						for(int b=0;b<listClockEventsBreak.Count;b++) {//check all breaks for current day.
							if(listClockEventsBreak[b].TimeDisplayed1.Date!=listClockEvents[i].TimeDisplayed1.Date) {
								continue;//skip breaks for other dates than current ClockEvent
							}
							timeSpanDailyBreaksCalc+=(listClockEventsBreak[b].TimeDisplayed2.TimeOfDay-listClockEventsBreak[b].TimeDisplayed1.TimeOfDay);
							timeSpanDailyBreaksTotal+=(listClockEventsBreak[b].TimeDisplayed2.TimeOfDay-listClockEventsBreak[b].TimeDisplayed1.TimeOfDay);
							timeSpanDailyHoursMinusBreaksTotal=timeSpanHoursWorkedTotal-timeSpanDailyBreaksTotal;
							if(PrefC.IsODHQ) {
								if(timeSpanDailyHoursMinusBreaksTotal>=TimeSpan.FromMinutes(331) && timeSpanDailyBreaksCalc>=TimeSpan.FromMinutes(30)) {//Break is over 30 minutes and total time worked is over 5 hour and 30 minutes.
									listClockEventsBreak[b].AdjustAuto=TimeSpan.FromMinutes(30)-timeSpanDailyBreaksCalc;
									ClockEvents.Update(listClockEventsBreak[b]);//save adjustments to breaks.
									timeSpanDailyBreaksAdjustTotal+=listClockEventsBreak[b].AdjustAuto;
									timeSpanDailyBreaksCalc=TimeSpan.FromMinutes(30);//reset daily breaks to 30 minutes so the next break is all adjustment
								}
								else if(timeSpanDailyHoursMinusBreaksTotal<TimeSpan.FromMinutes(331) && timeSpanDailyHoursMinusBreaksTotal>=TimeSpan.FromMinutes(221) && timeSpanDailyBreaksCalc>=TimeSpan.FromMinutes(20)) {//Break is over 20 minutes and total time worked is over 3 hour and 40 minutes.
									listClockEventsBreak[b].AdjustAuto=TimeSpan.FromMinutes(20)-timeSpanDailyBreaksCalc;
									ClockEvents.Update(listClockEventsBreak[b]);//save adjustments to breaks.
									timeSpanDailyBreaksAdjustTotal+=listClockEventsBreak[b].AdjustAuto;
									timeSpanDailyBreaksCalc=TimeSpan.FromMinutes(20);//reset daily breaks to 20 minutes so the next break is all adjustment
								}
								else if(timeSpanDailyHoursMinusBreaksTotal<TimeSpan.FromMinutes(221) && timeSpanDailyHoursMinusBreaksTotal>=TimeSpan.FromMinutes(111) && timeSpanDailyBreaksCalc>=TimeSpan.FromMinutes(10)) {//Break is over 10 minutes and total time worked is over 1 hour and 50 minutes.
									listClockEventsBreak[b].AdjustAuto=TimeSpan.FromMinutes(10)-timeSpanDailyBreaksCalc;
									ClockEvents.Update(listClockEventsBreak[b]);//save adjustments to breaks.
									timeSpanDailyBreaksAdjustTotal+=listClockEventsBreak[b].AdjustAuto;
									timeSpanDailyBreaksCalc=TimeSpan.FromMinutes(10);//reset daily breaks to 10 minutes so the next break is all adjustment
								}
								else if(timeSpanDailyHoursMinusBreaksTotal<TimeSpan.FromMinutes(111)){//Total time worked is less than 1 hour and 50 minutes.
									listClockEventsBreak[b].AdjustAuto=-timeSpanDailyBreaksCalc;
									ClockEvents.Update(listClockEventsBreak[b]);//save adjustments to breaks.
									timeSpanDailyBreaksAdjustTotal+=listClockEventsBreak[b].AdjustAuto;
									timeSpanDailyBreaksCalc=TimeSpan.FromMinutes(0);//reset daily breaks to 0 minutes so the next break is all adjustment
								}
							}
							else {
								if(timeSpanDailyBreaksCalc>TimeSpan.FromMinutes(31)) {//over 31 to avoid adjustments less than 1 minutes.
									listClockEventsBreak[b].AdjustAuto=TimeSpan.FromMinutes(30)-timeSpanDailyBreaksCalc;
									ClockEvents.Update(listClockEventsBreak[b]);//save adjustments to breaks.
									timeSpanDailyBreaksAdjustTotal+=listClockEventsBreak[b].AdjustAuto;
									timeSpanDailyBreaksCalc=TimeSpan.FromMinutes(30);//reset daily breaks to 30 minutes so the next break is all adjustment.
								}//end overBreaks>31 minutes
								else {
									//If the adjustment is 30 minutes or less, the adjustment amount should be set to 0 
									listClockEventsBreak[b].AdjustAuto=TimeSpan.Zero;
									ClockEvents.Update(listClockEventsBreak[b]);
								}
							}
						}//end checking all breaks for current day
						//OverBreaks applies to overtime and then to RegularTime
						listClockEvents[i].OTimeAuto+=timeSpanDailyBreaksAdjustTotal;//tsDailyBreaksTotal<=TimeSpan.Zero
						listClockEvents[i].AdjustAuto+=timeSpanDailyBreaksAdjustTotal;//tsDailyBreaksTotal is less than or equal to zero
						if(listClockEvents[i].OTimeAuto<TimeSpan.Zero) {//we have adjusted OT too far
							//listClockEvent[i].AdjustAuto+=listClockEvent[i].OTimeAuto;
							listClockEvents[i].OTimeAuto=TimeSpan.Zero;
						}
						timeSpanDailyBreaksTotal=TimeSpan.Zero;//zero out for the next day.
						timeSpanHoursWorkedTotal=TimeSpan.Zero;//zero out for next day.
					}//end overbreaks
				}
				#endregion
				ClockEvents.Update(listClockEvents[i]);
			}//end clockEvent loop.
		}

		private static TimeSpan calcRate2Portion(TimeSpan timeSpanRate2AMRule,TimeSpan timeSpanRate2PMRule,ClockEvent clockEventBreak) {
			TimeSpan timeSpan=new TimeSpan();
			//AM overlap==========================================================
			//Visual representation
			//AM Rule      :           X
			//Entire Break :o-------o  |             Stop-Start == Entire Break
			//Partial Break:      o----|---o         Rule-Start == Partial Break
			//No Break     :           |  o------o   Rule-Rule  == No break (won't actually happen in this block)
			timeSpan+=TimeSpan.FromTicks(
				Math.Min(clockEventBreak.TimeDisplayed2.TimeOfDay.Ticks,timeSpanRate2AMRule.Ticks)//min of stop or rule
				-Math.Min(clockEventBreak.TimeDisplayed1.TimeOfDay.Ticks,timeSpanRate2AMRule.Ticks)//min of start or rule
				);//equals the entire break, part of the break, or non of the break.
			//PM overlap==========================================================
			//Visual representation
			//PM Rule      :           X
			//Entire Break :o-------o  |             Rule-Rule   == No Break
			//Partial Break:      o----|---o         Stop-Rule   == Partial Break
			//No Break     :           |  o------o   Stop-Start  == Entire break
			timeSpan+=TimeSpan.FromTicks(
				Math.Max(clockEventBreak.TimeDisplayed2.TimeOfDay.Ticks,timeSpanRate2PMRule.Ticks)//max of stop or rule
				-Math.Max(clockEventBreak.TimeDisplayed1.TimeOfDay.Ticks,timeSpanRate2PMRule.Ticks)//max of start or rule
				);//equals the entire break, part of the break, or non of the break.
			return timeSpan;
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
		private static void AdjustAutoClockEventEntriesHelper(List<ClockEvent> listClockEvents,List<ClockEvent> listClockEventsBreak,TimeSpan timeSpanDifferentialAMRule,TimeSpan timeSpanDifferentialPMRule,TimeSpan timeSpanOvertimeHoursRule) {
			for(int i=0;i<listClockEvents.Count;i++) {
				//listClockEvent[i].OTimeAuto	=TimeSpan.Zero;
				listClockEvents[i].AdjustAuto	=TimeSpan.Zero;
				listClockEvents[i].Rate2Auto		=TimeSpan.Zero;
				//OTimeAuto and AdjustAuto---------------------------------------------------------------------------------
				//if((listClockEvent[i].TimeDisplayed2.TimeOfDay-listClockEvent[i].TimeDisplayed1.TimeOfDay)>tsOvertimeHoursRule) {
				//	listClockEvent[i].OTimeAuto+=listClockEvent[i].TimeDisplayed2.TimeOfDay-listClockEvent[i].TimeDisplayed1.TimeOfDay-tsOvertimeHoursRule;
				//listClockEvent[i].AdjustAuto+=-listClockEvent[i].OTimeAuto;
				//}
				//AdjustAuto due to break overages-------------------------------------------------------------------------
				if(PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks)) {
					if(i==listClockEvents.Count-1 || listClockEvents[i].TimeDisplayed1.Date!=listClockEvents[i+1].TimeDisplayed1.Date) {//last item or last item for a given day.
						TimeSpan timeSpanTotalBreaksToday=TimeSpan.Zero;
						for(int j=0;j<listClockEventsBreak.Count;j++) {
							if(listClockEventsBreak[j].TimeDisplayed1.Date!=listClockEvents[i].TimeDisplayed1.Date) {//skip breaks that occurred on different days.
								continue;
							}
							timeSpanTotalBreaksToday+=listClockEventsBreak[j].TimeDisplayed2.TimeOfDay-listClockEventsBreak[j].TimeDisplayed1.TimeOfDay;
						}
						if(timeSpanTotalBreaksToday>TimeSpan.FromMinutes(31)) {
							listClockEvents[i].AdjustAuto+=TimeSpan.FromMinutes(30)-timeSpanTotalBreaksToday;//should add a negative time span.
							listClockEvents[i].OTimeAuto+=TimeSpan.FromMinutes(30)-timeSpanTotalBreaksToday;//should add a negative time span.
							if(listClockEvents[i].OTimeAuto<TimeSpan.Zero) {//if we removed too much overbreak from otAuto, remove it from adjust auto instead and set otauto to zero
								listClockEvents[i].AdjustAuto+=listClockEvents[i].OTimeAuto;
								listClockEvents[i].OTimeAuto=TimeSpan.Zero;
							}
							timeSpanTotalBreaksToday=TimeSpan.FromMinutes(30);//reset break today to 30 minutes, so next break is entirely overBreak.
						}
					}
				}
				//Rate2Auto-------------------------------------------------------------------------------------------------
				if(listClockEvents[i].TimeDisplayed1.TimeOfDay<timeSpanDifferentialAMRule) {//AM, example rule before 8am, work from 5am to 7am
					listClockEvents[i].Rate2Auto+=timeSpanDifferentialAMRule-listClockEvents[i].TimeDisplayed1.TimeOfDay;//8am-5am=3hrs
					if(listClockEvents[i].TimeDisplayed2.TimeOfDay<timeSpanDifferentialAMRule) {
						listClockEvents[i].Rate2Auto+=listClockEvents[i].TimeDisplayed2.TimeOfDay-timeSpanDifferentialAMRule;//8am-7am=-1hr =>2hrs total
					}
				}
				if(listClockEvents[i].TimeDisplayed2.TimeOfDay>timeSpanDifferentialPMRule) {//PM, example diffRule after 8pm, work from 9 to 11pm. 
					listClockEvents[i].Rate2Auto+=listClockEvents[i].TimeDisplayed2.TimeOfDay-timeSpanDifferentialPMRule;//11pm-8pm = 3hrs 
					if(listClockEvents[i].TimeDisplayed1.TimeOfDay>timeSpanDifferentialPMRule) {
						listClockEvents[i].Rate2Auto+=timeSpanDifferentialPMRule-listClockEvents[i].TimeDisplayed1.TimeOfDay;//8pm-9pm = -1hr =>2hrs total
					}
				}
				ClockEvents.Update(listClockEvents[i]);
			}//end ClockEvent list
		}

		///<summary>Deprecated.  This function is aesthetic and has no bearing on actual OT calculations. It adds adjustments to breaks so that when viewing them you can see if they went over 30 minutes.</summary>
		private static void AdjustBreaksHelper(Employee employee,DateTime dateStart,DateTime dateStop) {
			if(!PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks)){
				//Only adjust breaks if preference is set.
				return;
			}
			List<ClockEvent> listClockEventsBreak=ClockEvents.Refresh(employee.EmployeeNum,dateStart,dateStop,true);//PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),true);
			TimeSpan timeSpanTotalToday=TimeSpan.Zero;
			TimeSpan timeSpanTotalOne=TimeSpan.Zero;
			DateTime datePrevious=DateTime.MinValue;
			for(int b=0;b<listClockEventsBreak.Count;b++) {
				if(listClockEventsBreak[b].TimeDisplayed2.Year<1880) {
					return;
				}
				if(listClockEventsBreak[b].TimeDisplayed1.Date != listClockEventsBreak[b].TimeDisplayed2.Date) {
					//MsgBox.Show(this,"Error. One break spans multiple dates.");
					return;
				}
				//calc time for the one break
				timeSpanTotalOne=listClockEventsBreak[b].TimeDisplayed2-listClockEventsBreak[b].TimeDisplayed1;
				//calc daily total
				if(datePrevious.Date != listClockEventsBreak[b].TimeDisplayed1.Date) {//if date changed, this is the first pair of the day
					timeSpanTotalToday=TimeSpan.Zero;//new day
					datePrevious=listClockEventsBreak[b].TimeDisplayed1.Date;//for the next loop
				}
				timeSpanTotalToday+=timeSpanTotalOne;
				//decide if breaks for the day went over 30 min.
				if(timeSpanTotalToday > TimeSpan.FromMinutes(31)) {//31 to prevent silly fractions less than 1.
					listClockEventsBreak[b].AdjustAuto=-(timeSpanTotalToday-TimeSpan.FromMinutes(30));
					ClockEvents.Update(listClockEventsBreak[b]);
					timeSpanTotalToday=TimeSpan.FromMinutes(30);//reset to 30.  Therefore, any additional breaks will be wholly adjustments.
				}
			}//end breaklist
		}

		///<summary>Calculates weekly overtime and inserts TimeAdjustments accordingly.</summary>
		public static void CalculateWeeklyOvertime(Employee employee,DateTime dateStart,DateTime dateTime) {
			TimeCardRule timeCardRule=GetTimeCardRule(employee);
			if(timeCardRule!=null && timeCardRule.IsOvertimeExempt) {
				return;
			}
			List<ClockEvent> listClockEvents=new List<ClockEvent>();
			List<TimeAdjust> listTimeAdjusts=new List<TimeAdjust>();
			string errors="";
			string clockErrors="";
			string timeAdjustErrors="";
			//Fill lists and catch validation error messages------------------------------------------------------------------------------------------------------------
			try{
				listClockEvents=ClockEvents.GetValidList(employee.EmployeeNum,dateStart,dateTime,false)	;
			}
			catch(Exception ex) {
				clockErrors+=ex.Message;
			}
			try{
				listTimeAdjusts=TimeAdjusts.GetValidList(employee.EmployeeNum,dateStart,dateTime);
			}
			catch(Exception ex) {
				timeAdjustErrors+=ex.Message;
			}
			//Report Errors---------------------------------------------------------------------------------------------------------------------------------------------
			errors=clockErrors+timeAdjustErrors;
			if(errors!="") {
				throw new Exception(Employees.GetNameFL(employee)+" has the following errors:\r\n"+errors);
			}
			//first, delete all existing non manual overtime entries
			for(int i=0;i<listTimeAdjusts.Count;i++) {
				if(listTimeAdjusts[i].IsAuto) {
					TimeAdjusts.Delete(listTimeAdjusts[i]);
					SecurityLogs.MakeLogEntry(EnumPermType.TimeAdjustEdit,0,
						$"Weekly overtime was calculated. Time Card Adjustment deleted for Employee: {Employees.GetNameFL(employee)}.");
				}
			}
			//refresh list after it has been cleaned up.
			listTimeAdjusts=TimeAdjusts.Refresh(employee.EmployeeNum,dateStart,dateTime);
			ArrayList arrayListMerged=MergeClockEventAndTimeAdjust(listClockEvents,listTimeAdjusts);
			//then, fill grid
			Calendar calendar=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule calendarWeekRule=CalendarWeekRule.FirstFullWeek;//CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			//rule=CalendarWeekRule.FirstFullWeek;//CalendarWeekRule is an Enum. For these calculations, we want to use FirstFullWeek, not FirstDay;
			List<TimeSpan> listTimeSpansWeeklyTotal = new List<TimeSpan>();
			listTimeSpansWeeklyTotal = FillWeeklyTotalsHelper(true,employee,arrayListMerged);
			//loop through all rows
			int weekIdx=0;//first week index==0
			for(int i=0;i<arrayListMerged.Count;i++) {
				//ignore rows that aren't weekly totals
				if(i<arrayListMerged.Count-1//if not the last row
					//if the next row has the same week as this row
					&& calendar.GetWeekOfYear(GetDateForRowHelper(arrayListMerged[i+1]),calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//Default is 0-Sunday
					== calendar.GetWeekOfYear(GetDateForRowHelper(arrayListMerged[i]),calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))) {
					continue;//continue within a single week
				}
				if(listTimeSpansWeeklyTotal[i]<=TimeSpan.FromHours(40)) {
					weekIdx++;//Going to the next week, go to the next week's entry in the list of ClinicNumTimeSpans
					continue;
				}
				//======CALUCLATE WEEKLY OVERTIME ADJUSTMENTS IF NEEDED======
				//found a weekly total over 40 hours
				List<ClinicNumTimeSpan> listClinicNumTimeSpans=_listEvents[weekIdx];//stores all worked hours per clinic in the order they were worked, sans dates, for a week.
				//validate the list of clock events.
				if(listClinicNumTimeSpans.GroupBy(x=>x.ClinicNum).Any(x=>x.Select(y=>y.TimeSpan.TotalHours).Sum()<0)) {
					//should never happen.
					throw new ApplicationException("Clock events for employee total a negative number of hours for a clinic.");
				}
				TimeSpan timeSpanWeeklyHours=TimeSpan.Zero;
				//this tracks each OT entry as it occurs, in the order it occurred, so that we can properly account for negative adjustments later.
				List<ClinicNumTimeSpan> listClinicNumsTimeSpansOTEntries=new List<ClinicNumTimeSpan>();
				for(int k=0;k<listClinicNumTimeSpans.Count;k++) {
					TimeSpan timeSpanPrevTotal=TimeSpan.FromHours(timeSpanWeeklyHours.TotalHours);//deep copy of timeSpan.
					timeSpanWeeklyHours=timeSpanWeeklyHours.Add(listClinicNumTimeSpans[k].TimeSpan);//add new timespan to running total.
					if(listClinicNumTimeSpans[k].TimeSpan<TimeSpan.Zero) { //negative span
						TimeSpan timeSpanNeg=-listClinicNumTimeSpans[k].TimeSpan;//store as positive balance to simplify comparisons below.
						//First try to compensate "using up" Ot from this clinic
						for(int j=listClinicNumsTimeSpansOTEntries.Count-1;j>=0;j--) {
							if(listClinicNumsTimeSpansOTEntries[j].ClinicNum!=listClinicNumTimeSpans[k].ClinicNum) {//only events for this clinic to start with.
								continue;
							}
							if(listClinicNumsTimeSpansOTEntries[j].TimeSpan>timeSpanNeg) {
								listClinicNumsTimeSpansOTEntries[j]=new ClinicNumTimeSpan(listClinicNumsTimeSpansOTEntries[j].ClinicNum,listClinicNumsTimeSpansOTEntries[j].TimeSpan.Subtract(timeSpanNeg));
								timeSpanNeg=TimeSpan.Zero;
								break;//we zeroed out the adjustment using only overtime from this clinic.
							}
							timeSpanNeg=timeSpanNeg.Subtract(listClinicNumsTimeSpansOTEntries[j].TimeSpan);
							listClinicNumsTimeSpansOTEntries[j]=new ClinicNumTimeSpan(listClinicNumsTimeSpansOTEntries[j].ClinicNum,listClinicNumsTimeSpansOTEntries[j].TimeSpan.Subtract(listClinicNumsTimeSpansOTEntries[j].TimeSpan));//zero it out.
						}
						//houskeeping
						listClinicNumsTimeSpansOTEntries.RemoveAll(x=>x.TimeSpan==TimeSpan.Zero);
						//possibly adjust off OT using time from other clinics in the order the time was accrued.
						for(int j=listClinicNumsTimeSpansOTEntries.Count-1;j>=0;j--) {
							if(timeSpanNeg<=TimeSpan.Zero) {
								break;
							}
							if(listClinicNumsTimeSpansOTEntries[j].TimeSpan>timeSpanNeg) {
								listClinicNumsTimeSpansOTEntries[j]=new ClinicNumTimeSpan(listClinicNumsTimeSpansOTEntries[j].ClinicNum,listClinicNumsTimeSpansOTEntries[j].TimeSpan.Subtract(timeSpanNeg));
								timeSpanNeg=TimeSpan.Zero;
								break;//we zeroed out the adjustment using only overtime from this clinic.
							}
							timeSpanNeg=timeSpanNeg.Subtract(listClinicNumsTimeSpansOTEntries[j].TimeSpan);
							listClinicNumsTimeSpansOTEntries[j]=new ClinicNumTimeSpan(listClinicNumsTimeSpansOTEntries[j].ClinicNum,listClinicNumsTimeSpansOTEntries[j].TimeSpan.Subtract(listClinicNumsTimeSpansOTEntries[j].TimeSpan));//zero it out.
						}
						//houskeeping
						listClinicNumsTimeSpansOTEntries.RemoveAll(x=>x.TimeSpan==TimeSpan.Zero);
					}
					if(timeSpanWeeklyHours.TotalHours>40) {//this clock event put us into overtime.
						ClinicNumTimeSpan clinicNumTimeSpan=new ClinicNumTimeSpan();
						clinicNumTimeSpan.ClinicNum=listClinicNumTimeSpans[k].ClinicNum;
						clinicNumTimeSpan.TimeSpan=TimeSpan.FromHours(timeSpanWeeklyHours.TotalHours-Math.Max(40,timeSpanPrevTotal.TotalHours));
						if(clinicNumTimeSpan.TimeSpan>TimeSpan.Zero) {
							listClinicNumsTimeSpansOTEntries.Add(clinicNumTimeSpan);
						}
					}
				}
				//======ADD OT ADJUSTMENTS FOR ONE WEEK AFTER ALL CALCULATIONS FOR THAT WEEK HAVE BEEN COMPLETED======
				List<long> listClinicNums=listClinicNumsTimeSpansOTEntries.Select(x=>x.ClinicNum).Distinct().ToList();
				for(int c=0;c<listClinicNums.Count;c++) {
					TimeSpan timeSpanForClinic=TimeSpan.FromHours(listClinicNumsTimeSpansOTEntries.FindAll(x=>x.ClinicNum==listClinicNums[c]).Sum(x=>x.TimeSpan.TotalHours));
					if(timeSpanForClinic<=TimeSpan.Zero) {
						continue;
					}
					TimeAdjust timeAdjust=new TimeAdjust();
					timeAdjust.IsAuto=true;
					timeAdjust.EmployeeNum=employee.EmployeeNum;
					timeAdjust.TimeEntry=GetDateForRowHelper(arrayListMerged[i]).AddHours(20);
					timeAdjust.OTimeHours=timeSpanForClinic;
					timeAdjust.RegHours=-timeAdjust.OTimeHours;
					timeAdjust.ClinicNum=listClinicNums[c];
					timeAdjust.IsUnpaidProtectedLeave=false;
					timeAdjust.SecuUserNumEntry=Security.CurUser.UserNum;
					TimeAdjusts.Insert(timeAdjust);
					SecurityLogs.MakeLogEntry(EnumPermType.TimeAdjustEdit,0,
						$"Weekly overtime was calculated. Time Card Adjustment created for Employee: {Employees.GetNameFL(employee)}.");
				}
				weekIdx++;
			}
		}

		///<summary>Merges a list of ClockEvent and a list of TimeAdjust, sorted by ClockEvent.TimeDisplayed1 and TimeAdjust.TimeEntry</summary>
		private static ArrayList MergeClockEventAndTimeAdjust(List<ClockEvent> listClockEvents,List<TimeAdjust> listTimeAdjusts) {
			List<ClockEvent> listClockEventsOrdered=listClockEvents.OrderBy(x => x.TimeDisplayed1).ToList();//Oldest entries first
			List<TimeAdjust> listTimeAdjustsOrdered=listTimeAdjusts.OrderBy(x => x.TimeEntry).ToList();
			ArrayList arrayListMerged=new ArrayList();
			int indexClockEvent=0;
			int indexTimeAdjust=0;
			while(true) {//Merge listClockEvent and listTimeAdjust, sort by TimeDisplayed1/TimeEntry
				if(indexClockEvent>=listClockEventsOrdered.Count && indexTimeAdjust>=listTimeAdjustsOrdered.Count) {
					break;
				}
				if(indexClockEvent==listClockEventsOrdered.Count) {//All ClockEvents added, so remaining TimeAdjusts will all be added.
					arrayListMerged.Add(listTimeAdjustsOrdered[indexTimeAdjust]);
					indexTimeAdjust++;
				}
				else if(indexTimeAdjust==listTimeAdjustsOrdered.Count) {//All TimeAdjusts added, so remaining ClockEvents will all be added.
					arrayListMerged.Add(listClockEventsOrdered[indexClockEvent]);//So add next ClockEvent
					indexClockEvent++;
				}
				else if(listClockEventsOrdered[indexClockEvent].TimeDisplayed1<=listTimeAdjustsOrdered[indexTimeAdjust].TimeEntry) {//ClockEvent is next
					arrayListMerged.Add(listClockEventsOrdered[indexClockEvent]);
					indexClockEvent++;
				}
				else {//TimeAdjust is next
					arrayListMerged.Add(listTimeAdjustsOrdered[indexTimeAdjust]);
					indexTimeAdjust++;
				}
			}
			return arrayListMerged;
		}

		/// <summary>This was originally analogous to the FormTimeCard.FillGrid(), before this logic was moved to the business layer.</summary>
		private static List<TimeSpan> FillWeeklyTotalsHelper(bool existsInDB,Employee employee,ArrayList arrayListMerged) {
			List<ClinicNumTimeSpan> listClinicNumTimeSpansWeek=new List<ClinicNumTimeSpan>();
			_listEvents=new List<List<ClinicNumTimeSpan>>();
			List<TimeSpan> listTimeSpans = new List<TimeSpan>();
			TimeSpan[] timeSpanArrayWeeklyTotals=new TimeSpan[arrayListMerged.Count];
			TimeSpan timeSpanAlteredSpan=TimeSpan.Zero;//used to display altered times
			TimeSpan timeSpanOne=TimeSpan.Zero;//used to sum one pair of clock-in/clock-out
			TimeSpan timeSpanOneAdj;
			TimeSpan timeSpanOneOT;
			TimeSpan timeSpanDay=TimeSpan.Zero;//used for daily totals.
			TimeSpan timeSpanWeek=TimeSpan.Zero;//used for weekly totals.
			TimeSpan timeSpanPreviousHours=TimeSpan.Zero;
			if(arrayListMerged.Count>0) {
				timeSpanPreviousHours=ClockEvents.GetWeekTotal(employee.EmployeeNum,GetDateForRowHelper(arrayListMerged[0]));
			}
			TimeSpan timeSpanPeriod=TimeSpan.Zero;//used to add up totals for entire page. (Not used. Left over from when this code existed in the UI.)
			TimeSpan timeSpanOT=TimeSpan.Zero;//overtime for the entire period
			Calendar calendar=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule calendarWeekRule=CalendarWeekRule.FirstFullWeek;// CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			DateTime dateNow=DateTime.MinValue;
			DateTime datePrevious=DateTime.MinValue;
			Type type;
			ClockEvent clockEvent;
			TimeAdjust timeAdjust;
			for(int i=0;i<arrayListMerged.Count;i++) {
				type=arrayListMerged[i].GetType();
				datePrevious=dateNow;
				//clock event row---------------------------------------------------------------------------------------------
				if(type==typeof(ClockEvent)) {
					clockEvent=(ClockEvent)arrayListMerged[i];
					if(timeSpanPreviousHours!=TimeSpan.Zero) {//Add in previous pay period's hours for this week if the week started in the middle of last payperiod.  Only need to do it once.
						listClinicNumTimeSpansWeek.Add(new ClinicNumTimeSpan(clockEvent.ClinicNum,timeSpanPreviousHours));
						timeSpanWeek+=timeSpanPreviousHours;
						timeSpanPreviousHours=TimeSpan.Zero;
					}
					dateNow=clockEvent.TimeDisplayed1.Date;
					if(clockEvent.TimeDisplayed2.Year<1880) {
						//ignore clock event where user has not clocked out yet.
					}
					else {
						timeSpanOne=clockEvent.TimeDisplayed2-clockEvent.TimeDisplayed1;
						timeSpanDay+=timeSpanOne;
						timeSpanWeek+=timeSpanOne;
						timeSpanPeriod+=timeSpanOne;
						listClinicNumTimeSpansWeek.Add(new ClinicNumTimeSpan(clockEvent.ClinicNum,timeSpanOne));
					}
					//Adjust---------------------------------
					timeSpanOneAdj=TimeSpan.Zero;
					if(clockEvent.AdjustIsOverridden) {
						timeSpanOneAdj+=clockEvent.Adjust;
					}
					else {
						timeSpanOneAdj+=clockEvent.AdjustAuto;//typically zero
					}
					timeSpanDay+=timeSpanOneAdj;
					timeSpanWeek+=timeSpanOneAdj;
					timeSpanPeriod+=timeSpanOneAdj;
					if(timeSpanOneAdj!=TimeSpan.Zero) {//take adjustments from breaks away from the OT values in the dictionary
						listClinicNumTimeSpansWeek.Add(new ClinicNumTimeSpan(clockEvent.ClinicNum,timeSpanOneAdj));
					}
					//Overtime------------------------------
					timeSpanOneOT=TimeSpan.Zero;
					if(clockEvent.OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
						timeSpanOneOT=clockEvent.OTimeHours;
					}
					else {
						timeSpanOneOT=clockEvent.OTimeAuto;//typically zero
					}
					timeSpanOT+=timeSpanOneOT;
					timeSpanDay-=timeSpanOneOT;
					timeSpanWeek-=timeSpanOneOT;
					timeSpanPeriod-=timeSpanOneOT;
					if(timeSpanOneOT>TimeSpan.Zero) {
						listClinicNumTimeSpansWeek.Add(new ClinicNumTimeSpan(clockEvent.ClinicNum,-timeSpanOneOT));
					}					
					//Daily-----------------------------------
					//if this is the last entry for a given date
					if(i==arrayListMerged.Count-1//if this is the last row
						|| GetDateForRowHelper(arrayListMerged[i+1]) != dateNow)//or the next row is a different date
					{
						timeSpanDay=TimeSpan.Zero;
					}
					else {//not the last entry for the day
					}
					//Weekly-------------------------------------
					timeSpanArrayWeeklyTotals[i]=timeSpanWeek;
					//if this is the last entry for a given week
					if(i==arrayListMerged.Count-1//if this is the last row 
						|| calendar.GetWeekOfYear(GetDateForRowHelper(arrayListMerged[i+1]),calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= calendar.GetWeekOfYear(clockEvent.TimeDisplayed1.Date,calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						_listEvents.Add(listClinicNumTimeSpansWeek);
						listClinicNumTimeSpansWeek=new List<ClinicNumTimeSpan>();//start over for the next week.
						timeSpanWeek=TimeSpan.Zero;
					}
				}
				//adjustment row--------------------------------------------------------------------------------------
				else if(type==typeof(TimeAdjust)) {
					timeAdjust=(TimeAdjust)arrayListMerged[i];
					if(timeSpanPreviousHours!=TimeSpan.Zero) {//Add in previous pay period's hours for this week if the payperiod starts in the middle of the week.  Only need to do it once.
						listClinicNumTimeSpansWeek.Add(new ClinicNumTimeSpan(timeAdjust.ClinicNum,timeSpanPreviousHours));
						timeSpanWeek+=timeSpanPreviousHours;
						timeSpanPreviousHours=TimeSpan.Zero;
					}
					dateNow=timeAdjust.TimeEntry.Date;
					if(!timeAdjust.IsUnpaidProtectedLeave) {
						//Adjust------------------------------
						timeSpanDay+=timeAdjust.RegHours;//might be negative
						timeSpanWeek+=timeAdjust.RegHours;
						timeSpanPeriod+=timeAdjust.RegHours;
						timeSpanOneAdj=timeAdjust.RegHours;
						if(timeSpanOneAdj!=TimeSpan.Zero) {
							listClinicNumTimeSpansWeek.Add(new ClinicNumTimeSpan(timeAdjust.ClinicNum,timeSpanOneAdj));
						}
						//Overtime------------------------------
						timeSpanOT+=timeAdjust.OTimeHours;
						timeSpanOneOT=timeAdjust.OTimeHours;
						if(timeSpanOneOT!=TimeSpan.Zero) {
							listClinicNumTimeSpansWeek.Add(new ClinicNumTimeSpan(timeAdjust.ClinicNum,timeSpanOneOT));
						}
					}
					//Daily-----------------------------------
					//if this is the last entry for a given date
					if(i==arrayListMerged.Count-1//if this is the last row
						|| GetDateForRowHelper(arrayListMerged[i+1]) != dateNow)//or the next row is a different date
					{
						timeSpanDay=new TimeSpan(0);
					}
					//Weekly-------------------------------------
					timeSpanArrayWeeklyTotals[i]=timeSpanWeek;
					//if this is the last entry for a given week
					if(i==arrayListMerged.Count-1//if this is the last row 
						|| calendar.GetWeekOfYear(GetDateForRowHelper(arrayListMerged[i+1]),calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= calendar.GetWeekOfYear(timeAdjust.TimeEntry.Date,calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						_listEvents.Add(listClinicNumTimeSpansWeek);
						listClinicNumTimeSpansWeek=new List<ClinicNumTimeSpan>();//start over for the next week.
						timeSpanWeek=new TimeSpan(0);
					}
				}
			}
			for(int i=0;i<timeSpanArrayWeeklyTotals.Length;i++) {
				listTimeSpans.Add(timeSpanArrayWeeklyTotals[i]);
			}
			return listTimeSpans;
		}

		private static DateTime GetDateForRowHelper(object objTimeEvent) {
			if(objTimeEvent.GetType()==typeof(ClockEvent)) {
				return ((ClockEvent)objTimeEvent).TimeDisplayed1.Date;
			}
			if(objTimeEvent.GetType()==typeof(TimeAdjust)) {
				return ((TimeAdjust)objTimeEvent).TimeEntry.Date;
			}
			return DateTime.MinValue;
		}
	}
}
