using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Schedules {
		#region Get Methods
		public static List<Schedule> GetSchedListForDateRange(long empNum, DateTime startDate, DateTime endDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),empNum,startDate,endDate);
			}
			string command="SELECT * FROM schedule where schedule.EmployeeNum="+empNum
				+" AND schedule.SchedDate BETWEEN "+POut.Date(startDate)+" AND "+POut.Date(endDate)
				+" ORDER BY schedule.SchedDate ASC";
			return RefreshAndFill(command,true);
		}

		///<summary>Gets a list of Schedule items for one date, ordered by start time.</summary>
		public static List<Schedule> GetDayList(DateTime date) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),date);
			}
			string command="SELECT * FROM schedule "
				+"WHERE SchedDate = "+POut.Date(date)+" "
				+"ORDER BY StartTime";
			return RefreshAndFill(command);
		}

		///<summary>Returns a distinct list of ProvNums that have a provider schedule that overlaps with the schedule passed in.
		///Every provider schedule passed in should be for the same day as schedCur.SchedDate otherwise false positive overlaps might be returned.
		///Collisions will be detected for every single ProvNum within listProvNums (mimics schedCur being for each provider provided indicated).
		///Returns an empty list if no collisions were detected or if invalid parameters were passed in.</summary>
		public static List<long> GetOverlappingSchedProvNums(List<long> listProvNums,Schedule schedCur,List<Schedule> listProvSchedsOnly
			,List<long> listSelectedOpNums)
		{
			//No need to check RemotingRole; no call to db.
			List<long> listProvsOverlap=new List<long>();
			if(listProvNums==null || schedCur==null || listProvSchedsOnly==null || listSelectedOpNums==null) {
				return listProvsOverlap;
			}
			foreach(long provNum in listProvNums) {//Potentially check each provider for overlap.
				Schedule schedTemp=schedCur.Copy();
				schedTemp.ProvNum=provNum;
				if(schedTemp.IsNew) {
					listProvSchedsOnly.Add(schedTemp);//new scheds will be added to check if overlap.
				}
				//====================SIMPLE OVERLAP, No-Ops==========================
				bool isOverlapping=false;
				if(schedTemp.Ops.Count==0) {//only look at schedules without operatories
					isOverlapping=(listProvSchedsOnly.Where(x => x.ProvNum==schedTemp.ProvNum //Only consider current provider for overlaps w/o Ops
							&& x.Ops.Count==0 //Also doesn't have an operatory
							&& schedTemp.StartTime<=x.StopTime //Overlapping Time
							&& schedTemp.StopTime>=x.StartTime)//Overlapping Time
						.Count() > 1);
				}
				//====================COMPLEX OVERLAP, Ops and All====================
				else if(!isOverlapping && schedTemp.Ops.Count>0) {//If we did not find a simple overlap, attempt to find a "complicated" overlap
					isOverlapping=(listProvSchedsOnly.Where(x => x.Ops.Count>0 //Can only overlap if Ops are involved
							&& schedTemp.StartTime<=x.StopTime //Overlapping Time
							&& schedTemp.StopTime>=x.StartTime //Overlapping Time
							&& x.Ops.Any(y => ListTools.In(y,listSelectedOpNums)))//Schedule contains any operatory in question.
						.Count() > 1);
				}
				if(isOverlapping) {
					listProvsOverlap.Add(provNum);
				}
			}
			return listProvsOverlap.Distinct().ToList();
		}

		///<summary>Returns a list of ProvNums that have overlapping provider schedule conflicts with the current schedule in the database.
		///Set dateStart and dateEnd to the same date to use the "daily" pasting logic.
		///Setting dateStart and dateEnd to different dates will drastically change the logic of this method putting it into weekly mode.
		///Daily mode will treat all schedules passed in as if they are on the same day.
		///Weekly mode will turn the list of schedules passed in into several lists that are grouped by the day of the week they fall on.
		///This means that weekly mode will assume listSchedules cannot contain schedules that span more than one full week.</summary>
		///<param name="listSchedules">Any schedules that need to be checked for overlapping.</param>
		///<param name="dateStart">Starting date of the overlapping detection.</param>
		///<param name="dateEnd">Ending date of the overlapping detection.</param>
		///<param name="listIgnoreProvNums">Ignores all schedules from the database for the corresponding providers.</param>
		///<returns>Returns a distinct list of ProvNums that have a provider schedule that overlaps with the schedules passed in;
		///Otherwise, empty list if no collisions were detected or if invalid parameters were passed in.</returns>
		public static List<long> GetOverlappingSchedProvNumsForRange(List<Schedule> listSchedules,DateTime dateStart,DateTime dateEnd
			,List<long> listIgnoreProvNums=null)
		{
			//No need to check RemotingRole; no call to db.
			List<long> listProvsOverlap=new List<long>();
			if(listSchedules==null || listSchedules.Count < 1) {
				return listProvsOverlap;//Nothing to check overlapping against.  Return empty list.
			}
			//Get the currently scheduled provider schedules for the date range.
			List<Schedule> listDbProvScheds=Schedules.GetAllForDateRangeAndType(dateStart,dateEnd,ScheduleType.Provider);
			//Filter out any schedules from the database that are related to the providers that should be ignored (user is going to replace existing).
			if(listIgnoreProvNums!=null) {
				listDbProvScheds.RemoveAll(x => ListTools.In(x.ProvNum,listIgnoreProvNums));
			}
			if(dateStart.Date==dateEnd.Date) {//Daily mode.
				//Daily mode will take the entire list of schedules passed in and compare each schedule to the schedules in the database.
				foreach(Schedule schedule in listSchedules) {
					listProvsOverlap=listProvsOverlap.Union(GetOverlappingSchedProvNums(
							new List<long>() { schedule.ProvNum },
							schedule,
							listDbProvScheds,
							schedule.Ops))
						.ToList();
				}
			}
			else {//Weekly mode.
				//Break up the list of provider schedules from the database into a dictionary grouped by schedDate.
				Dictionary<DateTime,List<Schedule>> dictDbProvSchedsByDate=listDbProvScheds.GroupBy(x => x.SchedDate).ToDictionary(x => x.Key,x => x.ToList());
				//Break up the list of schedules passed in by the day of the week because we make the assumption that they cannot copy more than a weeks worth.
				Dictionary<DayOfWeek,List<Schedule>> dictSchedsByDayOfWeek=listSchedules.GroupBy(x => x.SchedDate.DayOfWeek)
					.ToDictionary(x => x.Key,x => x.ToList());
				//Loop through each unique date in the list of schedules from the database.
				foreach(DateTime dateSched in dictDbProvSchedsByDate.Keys) {
					//Get all of the schedules that were passed in that fall on the corresponding day of the week.
					List<Schedule> listSchedsForDayOfWeek;
					if(!dictSchedsByDayOfWeek.TryGetValue(dateSched.DayOfWeek,out listSchedsForDayOfWeek)) {
						listSchedsForDayOfWeek=new List<Schedule>();
					}
					foreach(Schedule scheduleTemp in listSchedsForDayOfWeek) {
						listProvsOverlap=listProvsOverlap.Union(GetOverlappingSchedProvNums(
								new List<long>() { scheduleTemp.ProvNum },
								scheduleTemp,
								dictDbProvSchedsByDate[dateSched],
								scheduleTemp.Ops))
							.ToList();
					}
				}
			}
			return listProvsOverlap.Distinct().ToList();
		}
		#endregion

		#region Misc Methods
		
		///<summary>Copies the blockouts for the passed in appointment view and date range starting at the passed in "selected" range and repeating
		///a certain number of times. The "selected" range will either be 1 day (for repeating a single day) or will be 4 or 6 days for copying weeks 
		///depending on whether weekends are included.  This also creates securitylog entry with the action performed.</summary>
		///<param name="apptViewNum">The appointment view that contains the ops whose blockouts will be copied.</param>
		///<param name="isWeek">Indicates whether the range being copied is a single day or is a week.</param>
		///<param name="includeWeekend">Indicates whether weekends (Saturday and Sunday) will be included when copying blockout schedules.</param>
		///<param name="doReplace">If set to true, it will overwrite all blockouts that exits in the given time frame.</param>
		///<param name="dateCopyStart">The start date of the selected range to be copied.</param>
		///<param name="dateCopyEnd">The end date of the selected range to be copied. Will be the same as dateCopyStart for copying a single day.</param>
		///<param name="dateSelectedStart">The start date of the selected range that will be copied to.</param>
		///<param name="dateSelectedEnd">The end date of the selected range that will be copied to.</param>
		///<param name="numRepeat">The number of times the given blockout range will be copied.</param>
		public static string CopyBlockouts(long apptViewNum,bool isWeek,bool includeWeekend,bool doReplace,DateTime dateCopyStart,
			DateTime dateCopyEnd,DateTime dateSelectedStart,DateTime dateSelectedEnd,int numRepeat) 
		{
			//No need to check RemotingRole; no call to db. Better to do this locally as it may take some time and we do not want Middle Tier to timeout.
			//It is allowed to paste back over the same day or week.
			List<long> listOpNums=ApptViewItems.GetOpsForView(apptViewNum);
			List<Schedule> listSchedulesToCopy=Schedules.RefreshPeriodBlockouts(dateCopyStart,dateCopyEnd,listOpNums);
			//Build a list of blockouts that can't be Cut/Copy/Pasted
			List<Def> listUserBlockoutDefs=Defs.GetDefsForCategory(DefCat.BlockoutTypes, true)
				.FindAll(x => x.ItemValue.Contains(BlockoutType.DontCopy.GetDescription()));
			//No SchedList only contains blockouts that are NOT marked "Do not Cut/Copy/Paste"
			listSchedulesToCopy.RemoveAll(x => listUserBlockoutDefs.Any(y => y.DefNum==x.BlockoutType));
			int weekDelta=0;
			if(isWeek) {
				TimeSpan span=dateSelectedStart-dateCopyStart;
				weekDelta=span.Days/7;//usually a positive # representing a future paste, but can be negative
			}
			DateTime dateEnd;
			if(isWeek) {
				dateEnd=dateSelectedEnd.AddDays((numRepeat-1)*7);
			}
			else {//Copying single days
				if(includeWeekend) {
					dateEnd=dateSelectedEnd.AddDays(numRepeat-1);
				}
				else {
					dateEnd=DateTools.AddWeekDays(dateSelectedEnd,numRepeat-1);
				}
			}
			List<Schedule> listSchedulesPossibleOverlap=null;
			if(doReplace) {
				//If replacing all, do one bulk ClearBlockouts.
				Schedules.ClearBlockouts(dateSelectedStart,dateEnd,listOpNums,includeWeekend);
			}
			else {
				//If we are not replacing, pull all schedules in the entire date range so we do not need to access the database every time.
				listSchedulesPossibleOverlap=Schedules.GetAllForDateRangeAndType(dateSelectedStart,dateEnd,ScheduleType.Blockout,
					listOpNums:listOpNums);
			}
			//Stores all of the blockouts to be inserted.
			List<Schedule> listNewScheds=new List<Schedule>();
			int dayDelta=0;//this is needed when repeat pasting days in order to calculate skipping weekends.
			//dayDelta will start out zero and increment separately from r.
			for(int r=0;r<numRepeat;r++) {
				foreach(Schedule scheduleToCopy in listSchedulesToCopy) {
					Schedule sched=scheduleToCopy.Copy();
					sched.ScheduleNum=0;//So that overlap logic works.
					if(isWeek) {
						sched.SchedDate=sched.SchedDate.AddDays((weekDelta+r)*7);
					}
					else {
						sched.SchedDate=dateSelectedStart.AddDays(dayDelta);
					}
					if(!doReplace && Schedules.Overlaps(sched,listSchedulesPossibleOverlap)) {
						Schedules.Insert(false,true,listNewScheds.ToArray());
						string error=Lans.g("Schedule","A blockout overlaps with an existing blockout. Could not paste the blockout on")
							+" "+sched.SchedDate.ToShortDateString()+" "+sched.StartTime.ToShortTimeString();
						return error;
					}
					listNewScheds.Add(sched);
				}
				//dayDelta is only used for repeating single days, not for repeating weeks, so we don't need to determine whether or not they copied 
				//weekends, we can rely on includeWeekend
				if(!includeWeekend && dateSelectedStart.AddDays(dayDelta).DayOfWeek==DayOfWeek.Friday) {
					dayDelta+=3;
				}
				else {
					dayDelta++;
				}
			}
			Schedules.Insert(false,true,listNewScheds.ToArray());
			string logText=Lans.g("Schedule","Blockouts for operatories")+" ";
			for(int i=0;i<listOpNums.Count;i++) {
				if(i>0) {
					logText+=", ";
				}
				logText+=Operatories.GetOpName(listOpNums[i]);
			}
			logText+=" "+Lans.g("Schedule","copied from")+" "+dateCopyStart.ToShortDateString()+" "
				+(dateCopyStart==dateCopyEnd? "" : Lans.g("Schedule","through")+" "+dateCopyEnd.ToShortDateString()+" ")
				+(numRepeat>1?Lans.g("Schedule","and pasted from")+" "+dateSelectedStart.ToShortDateString()+" "+Lans.g("Schedule","until"):Lans.g("Schedule","and pasted to"))
				+" "+dateEnd.ToShortDateString();
			SecurityLogs.MakeLogEntry(Permissions.Blockouts,0,logText);
			return "";
		}

		///<summary>Creates a securitylog that is constructed with definitions, operatories, dates, translation, etc. already done.
		///If several blockouts are cleared, specify the dateTime for the blockouts cleared and BlockoutAction.Clear.
		///If several blockouts are cleared for a specific operatory, specify the dateTime, operatory, and BlockoutAction.Clear
		///If several blockouts are cleared for specific operatory and clinic, specify the dateTime, operatory, clinic, and BlockoutAction.Clear
		///Otherwise, supply blockout and action taken.</summary>
		public static void BlockoutLogHelper(BlockoutAction action,Schedule blockout=null,DateTime dateTime=new DateTime(),long opNum=0,long clinicNum=-1) {
			string logText="";
			if(blockout==null) {//Day cleared
				logText+=Lans.g("Schedule","Blockouts")+" ";
			}
			else if(blockout.SchedType==ScheduleType.WebSchedASAP) {
				logText+=Lans.g("Schedule","Blockout of type Web Schedule ASAP Blockout ");
			}
			else {
				logText+=Lans.g("Schedule","Blockout of type")+" "+Defs.GetName(DefCat.BlockoutTypes,blockout.BlockoutType)+" ";
			}
			switch(action) {
				case BlockoutAction.Copy:
					logText+=Lans.g("Schedule","copied from")+" ";
					break;
				case BlockoutAction.Create:
					logText+=Lans.g("Schedule","created for")+" ";
					break;
				case BlockoutAction.Edit:
					logText+=Lans.g("Schedule","edited to")+" ";//For edit logs do we want to add where the blockout originated from?  If so, this will have to change some.
					break;
				case BlockoutAction.Cut:
					logText+=Lans.g("Schedule","cut from")+" ";
					break;
				case BlockoutAction.Delete:
					logText+=Lans.g("Schedule","deleted from")+" ";
					break;
				case BlockoutAction.Paste:
					logText+=Lans.g("Schedule","pasted to")+" ";
					break;
				case BlockoutAction.Clear:
					logText+=Lans.g("Schedule","cleared on")+" ";
					break;
			}
			if(blockout==null) {//Clear action taken for specific date
				logText+=dateTime.Date.ToShortDateString()+" ";
				if(opNum!=0) {
					logText+=Lans.g("Schedule","for operatory")+" "+Operatories.GetOpName(opNum);
				}
				if(clinicNum!=-1) {
					if(opNum!=0) {//Not currently an option to clear via op and clinic, but may be in the future.
						logText+=Lans.g("Schedule","and clinic")+" ";
					}
					else {
						logText+=Lans.g("Schedule","for clinic")+" ";
					}
					logText+=(clinicNum==0? Lans.g("Schedule","Headquarters") : Clinics.GetDesc(clinicNum));
				}
			}
			else {
				if(blockout.Ops.Count>1) {
					logText+=Lans.g("Schedule","operatories")+" ";
				}
				else {
					logText+=Lans.g("Schedule","operatory")+" ";
				}
				for(int i=0;i<blockout.Ops.Count;i++) {
					if(i>0) {
						logText+=", ";
					}
					logText+=Operatories.GetOpName(blockout.Ops[i]);
				}
				logText+=" "+Lans.g("Schedule","on")+" "+blockout.SchedDate.ToShortDateString()+" "
					+Lans.g("Schedule","for")+" "+blockout.StartTime.ToShortTimeString()+" - "+blockout.StopTime.ToShortTimeString();
			}
			SecurityLogs.MakeLogEntry(Permissions.Blockouts,0,logText);
		}

		#endregion		

		///<summary>Used in the Schedules edit window to get a filtered list of schedule items in preparation for paste or repeat.</summary>
		public static List<Schedule> RefreshPeriod(DateTime dateStart,DateTime dateEnd,List<long> provNums,List<long> empNums,bool includePNotes,
			bool includeCNotes,long clinicNum)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,provNums,empNums,includePNotes,includeCNotes,clinicNum);
			}
			if(provNums.Count==0 && empNums.Count==0 && !includeCNotes && !includePNotes) {
				return new List<Schedule>();
			}
			List<string> listOrClauses=new List<string>();
			if(includePNotes) {
				listOrClauses.Add("(SchedType="+POut.Int((int)ScheduleType.Practice)+" AND ClinicNum=0)");
			}
			//if the user has the HQ clinic selected and checks the show clinic holidays and notes, this will show holidays and notes for all clinics
			//if any other clinic is selected, this will show those holidays and notes for the selected clinic
			if(includeCNotes) {
				//if HQ, include notes and holidays for all non-HQ clinics, otherwise only include for the selected clinic
				listOrClauses.Add("(SchedType="+POut.Int((int)ScheduleType.Practice)+" AND ClinicNum"+(clinicNum==0?">0":("="+POut.Long(clinicNum)))+")");
			}
			if(provNums.Count>0) {
				listOrClauses.Add("schedule.ProvNum IN ("+string.Join(",",provNums.Select(x => POut.Long(x)))+")");
			}
			if(empNums.Count>0) {
				listOrClauses.Add("schedule.EmployeeNum IN ("+string.Join(",",empNums.Select(x => POut.Long(x)))+")");
			}
			string command="SELECT * FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND ("+string.Join(" OR ",listOrClauses)+")";
			return RefreshAndFill(command);
		}

		///<summary></summary>
		public static List<Schedule> RefreshPeriodBlockouts(DateTime dateStart,DateTime dateEnd,List<long> opNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,opNums);
			}
			if(opNums.Count==0){
				return new List<Schedule>();
			}
			string command="SELECT * "
				+"FROM schedule "
				+"WHERE SchedType="+POut.Int((int)ScheduleType.Blockout)+" "
				+"AND SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND ScheduleNum IN (SELECT ScheduleNum FROM scheduleop WHERE OperatoryNum IN("+string.Join(",",opNums.Select(x => POut.Long(x)))+"))";
			return RefreshAndFill(command);
		}

		///<summary></summary>
		public static List<Schedule> RefreshDayEdit(DateTime dateSched){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSched);
			}
			string command="SELECT schedule.* "
				+"FROM schedule "
				+"WHERE SchedDate="+POut.Date(dateSched)+" "
				+"AND SchedType IN (0,1,3)";//Practice or Provider or Employee
			return RefreshAndFill(command);
		}

		///<summary>Gets a list of Schedule items for one date filtered by providers and employees.  Also option to include practice and clinic holidays and practice notes.</summary>
		public static List<Schedule> RefreshDayEditForPracticeProvsEmps(DateTime dateSched,List<long> listProvNums,List<long> listEmpNums,long clinicNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSched,listProvNums,listEmpNums,clinicNum);
			}
			List<string> listOrClauses=new List<string>();
			if(listProvNums.Count>0) {
				listOrClauses.Add("(SchedType="+POut.Int((int)ScheduleType.Provider)+" "
					+"AND ProvNum IN ("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+"))");
			}
			if(listEmpNums.Count>0) {
				listOrClauses.Add("(SchedType="+POut.Int((int)ScheduleType.Employee)+" "
					+"AND EmployeeNum IN ("+string.Join(",",listEmpNums.Select(x => POut.Long(x)))+"))");
			}
			//always include practice notes, plus any clinic notes for the selected clinic
			string pNoteOr="SchedType="+POut.Int((int)ScheduleType.Practice);
			if(clinicNum>0) {
				pNoteOr="("+pNoteOr+" AND ClinicNum IN (0,"+POut.Long(clinicNum)+"))";//0 for practice notes, clinicNum for clinic notes
			}
			listOrClauses.Add(pNoteOr);
			string command="SELECT schedule.* "
				+"FROM schedule "
				+"WHERE SchedDate="+POut.Date(dateSched)+" "
				+"AND ("+string.Join(" OR ",listOrClauses)+")";
			return RefreshAndFill(command);
		}

		public static List<Schedule> RefreshPeriodForEmps(DateTime dateStart,DateTime dateEnd,List<long> listEmpNums) {
			if(listEmpNums.IsNullOrEmpty()) {
				return new List<Schedule>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listEmpNums);
			}
			string command="SELECT schedule.* "
				+"FROM schedule "
				+"WHERE SchedType="+POut.Int((int)ScheduleType.Employee)+" "
				+"AND EmployeeNum IN ("+string.Join(",",listEmpNums.Select(x => POut.Long(x)))+") "
				+"AND SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"ORDER BY SchedDate";
			return RefreshAndFill(command);
		}

		///<summary>Returns a list of schedules with at least one conflict using the given provider and clinics.  Returns empty list if no conflicts found.</summary>
		public static List<Schedule> GetClinicOverlapsForProv(DateTime dateFrom,DateTime dateTo,long provNum,List<long> listClinicNums) {
			DataTable tableSchedsForProvider=GetPeriodSchedsForProvsAndClinics(dateFrom,dateTo,new List<long>() {provNum },listClinicNums);
			//The datatable contains a row for each schedule and its clinic for this provider.  Now we go through it and determine if there are any overlaps.
			//Compare schedules and find ones that overlap.  If they do overlap, compare clinics.
			List<Schedule> listConflicts=new List<Schedule>(); 
			foreach(DataRow row in tableSchedsForProvider.Rows) {
				DateTime date1=PIn.DateT(row["SchedDate"].ToString());
				TimeSpan startTime1=PIn.Time(row["StartTime"].ToString());
				TimeSpan stopTime1=PIn.Time(row["StopTime"].ToString());
				long clinicNum1=PIn.Long(row["OpClinicNum"].ToString());
				long schedNum=PIn.Long(row["ScheduleNum"].ToString());
				if(listConflicts.Exists(x => x.SchedDate==date1 && x.StartTime==startTime1 && x.StopTime==stopTime1)) {
					continue;
				}
				foreach(DataRow row2 in tableSchedsForProvider.Rows) {
					DateTime date2=PIn.DateT(row2["SchedDate"].ToString());
					TimeSpan startTime2=PIn.Time(row2["StartTime"].ToString());
					TimeSpan stopTime2=PIn.Time(row2["StopTime"].ToString());
					long clinicNum2=PIn.Long(row2["OpClinicNum"].ToString());
					if(date1!=date2 || clinicNum1==clinicNum2) {
						continue;
					}
					//Their clinics don't match and are on the same day, let's see if they overlap.
					if(startTime1<=startTime2 && stopTime1>=startTime2) {
						//conflict due to the start time being between start and stop.
						listConflicts.Add(new Schedule() { ClinicNum=clinicNum1,SchedDate=date1,StartTime=startTime1,StopTime=stopTime1,ProvNum=provNum });
						break;
					}
					if(stopTime1>=stopTime2 && startTime1<=stopTime2) {
						//conflict due to the end time being between start and stop
						listConflicts.Add(new Schedule() { ClinicNum=clinicNum1,SchedDate=date1,StartTime=startTime1,StopTime=stopTime1,ProvNum=provNum });
						break;
					}
				}
			}
			return listConflicts;
		}

		///<summary>Returns a table of schedules with their clinic (gotten from operatory if a link exists) for the providers specified, for the date range specified, and for the clinics specified.</summary>
		public static DataTable GetPeriodSchedsForProvsAndClinics(DateTime dateFrom,DateTime dateTo,List<long> listProvNums,List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvNums,listClinicNums);
			}
			string whereClinic="";
			if(listClinicNums.Count>0) {
				whereClinic=" AND COALESCE(operatory.ClinicNum,0) IN ("+string.Join(",",listClinicNums)+") ";
			}
			string whereProv="";
			if(listProvNums.Count>0) {
					whereProv=" AND schedule.ProvNum IN ("+string.Join(",",listProvNums)+") ";
			}
			string command=@"SELECT schedule.*, COALESCE(operatory.ClinicNum,0) AS OpClinicNum FROM schedule 
				LEFT JOIN scheduleop ON scheduleop.ScheduleNum=schedule.ScheduleNum 
				LEFT JOIN operatory ON scheduleop.OperatoryNum=operatory.OperatoryNum 
				WHERE schedule.SchedType="+POut.Int((int)ScheduleType.Provider)+" "
				+"AND schedule.Status="+POut.Int((int)SchedStatus.Open)+" "
				+"AND schedule."+DbHelper.BetweenDates("SchedDate",dateFrom,dateTo)+" "
				+whereProv
				+whereClinic;
			//The above query is also found in RpProdGoal (it has the addition of provider production goals)
			return Db.GetTable(command);
		}

		public static List<Schedule> GetByScheduleNum(List<long> listScheduleNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),listScheduleNums);
			}
			if(listScheduleNums.Count==0) {
				return new List<Schedule>();
			}
			string command="SELECT schedule.* "
				+"FROM schedule "
				+"WHERE ScheduleNum IN("+string.Join(",",listScheduleNums.Select(x => POut.Long(x)))+") ";
			return RefreshAndFill(command);
		}

		///<summary></summary>
		public static List<Schedule> GetTwoYearPeriod(DateTime startDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),startDate);
			}
			string command="SELECT schedule.* "
				+"FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(startDate)+" AND "+POut.Date(startDate.AddYears(2))+" "
				+"AND SchedType IN (0,1,3)";//Practice or Provider or Employee
			return RefreshAndFill(command);
		}

		///<summary></summary>
		public static List<Schedule> GetSchedulesForAppointmentSearch(DateTime dateStart,DateTime dateEnd,List<long> listClinicNums,
			List<long> listOpNums,List<long> listProvNums,long blockoutType, bool isForMakeRecall=false)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listClinicNums,listOpNums,listProvNums,blockoutType,isForMakeRecall);
			}
			if(!listProvNums.Contains(0)) {
				listProvNums.Add(0); //add 0 so blockouts can be returned.
			}
			List<long> listBlockoutTypeDefNums=new List<long>();
			//get a list of blockout types that are set to Do Not Schedule so we can filter them out later. 
			//(The list of blockouts we don't want to show)
			listBlockoutTypeDefNums=Defs.GetDefsForCategory(DefCat.BlockoutTypes,true)
				.FindAll(x => x.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()))
				.Select(x => x.DefNum).ToList();
			//add the blockoutTypes that we specifically want to search for to the list so they will also be returned, will be 0 if not searching blockouts. 
			//(The list of blockout scheds we want to show)
			listBlockoutTypeDefNums.Add(blockoutType);
			List<int> listSchedTypes=
				new List<int>() { (int)ScheduleType.Provider,(int)ScheduleType.Blockout,(int)ScheduleType.Practice,(int)ScheduleType.Employee };
			return GetSchedulesHelper(dateStart,dateEnd,listClinicNums,listOpNums,listProvNums,listBlockoutTypeDefNums,listSchedTypes,isForMakeRecall: isForMakeRecall);
		}

		///<summary>Used in the check database integrity tool.  Does NOT fill the list of operatories per schedule.</summary>
		public static Schedule[] RefreshAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Schedule[]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM schedule";
			return RefreshAndFill(command,true).ToArray();
		}

		public static List<Schedule> GetChangedSince(DateTime changedSince) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT * schedule WHERE DateTStamp>"+POut.DateT(changedSince)+" AND SchedType="+POut.Int((int)ScheduleType.Provider);
			return RefreshAndFill(command);
		}

		///<summary>This is only allowed because it's PRIVATE.  Retrieves all schedules from the db using command, then retrieves all scheduleops for the schedules and fills the schedule.Ops list with OperatoryNums for the schedule.  Does NOT use GROUP_CONCAT since there is a max length for GROUP_CONCAT and data may be incorrect or truncated, especially with random primary keys.</summary>
		private static List<Schedule> RefreshAndFill(string command,bool skipSchedOps=false) {
			//No need to check RemotingRole; private method.
			//Not a typical refreshandfill, as it contains a query.
			List<Schedule> listScheds=Crud.ScheduleCrud.SelectMany(command);
			if(listScheds.Count==0 || skipSchedOps) {
				return listScheds;
			}
			command="SELECT ScheduleNum,OperatoryNum FROM scheduleop WHERE ScheduleNum IN("+string.Join(",",listScheds.Select(x => x.ScheduleNum))+")";
			DataTable tableSO=Db.GetTable(command);
			if(tableSO.Rows.Count==0) {
				return listScheds;
			}
			Dictionary<long,List<long>> dictSchedOps=tableSO.Rows.OfType<DataRow>()
				.GroupBy(x => PIn.Long(x["ScheduleNum"].ToString()),x => PIn.Long(x["OperatoryNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			listScheds.FindAll(x => dictSchedOps.ContainsKey(x.ScheduleNum))//find schedules that have 1+ scheduleops.
				.ForEach(x => x.Ops=dictSchedOps[x.ScheduleNum]);
			return listScheds;
		}

		public static List<Schedule> ConvertTableToList(DataTable table){
			//No need to check RemotingRole; no call to db.
			List<Schedule> retVal=Crud.ScheduleCrud.TableToList(table);
			if(!table.Columns.Contains("ops")) {
				return retVal;
			}
			for(int i = 0;i<retVal.Count;i++) {
				retVal[i].Ops=table.Rows[i]["ops"].ToString().Split(new[] { "," },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
			}
			return retVal;
		}

		///<summary>Update a schedule.  Insert an invalid schedule signalod.</summary>
		public static void Update(Schedule sched){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sched);
				return;
			}
			Validate(sched);
			Crud.ScheduleCrud.Update(sched);
			string command="DELETE FROM scheduleop WHERE ScheduleNum="+POut.Long(sched.ScheduleNum);
			Db.NonQ(command);
			Signalods.SetInvalidSched(sched);
			sched.Ops.ForEach(x => ScheduleOps.Insert(new ScheduleOp { ScheduleNum=sched.ScheduleNum,OperatoryNum=x }));
		}

		///<summary>Similar to Crud.ScheduleCrud.Update except this also handles ScheduleOps.  Insert an invalid schedule signalod when hasSignal=true.</summary>
		public static void Update(Schedule schedNew,Schedule schedOld,bool validate,bool hasSignal=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),schedNew,schedOld,validate,hasSignal);
				return;
			}
			if(validate) {
				Validate(schedNew);
			}
			Crud.ScheduleCrud.Update(schedNew,schedOld); //may not cause an update, but we still need to check for updates to ScheduleOps below.
			if(hasSignal) {
				Signalods.SetInvalidSched(schedNew);
			}
			//Sort Ops for SequenceEqual call below.
			schedNew.Ops.Sort();
			schedOld.Ops.Sort();
			if(schedNew.Ops.SequenceEqual(schedOld.Ops)) {  //If both lists contain exactly the same ops.
				return;//no updates to ScheduleOps needed
			}
			string command="DELETE FROM scheduleop WHERE ScheduleNum="+POut.Long(schedNew.ScheduleNum);
			Db.NonQ(command);
			//re-insert ScheduleOps based on the list of opnums in schedNew.Ops
			schedNew.Ops.ForEach(x => ScheduleOps.Insert(new ScheduleOp { ScheduleNum=schedNew.ScheduleNum,OperatoryNum=x }));
		}

		///<summary>Set validate to true to throw an exception if start and stop times need to be validated.  If validate is set to false, then the calling code is responsible for the validation.  Also inserts necessary scheduleop enteries.  Insert an invalid schedule signalod when hasSignal=true.</summary>
		public static long Insert(Schedule sched,bool validate,bool hasSignal=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Remoting role check so that the PK can be returned to the client.
				sched.ScheduleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),sched,validate,hasSignal);
				return sched.ScheduleNum;
			}
			Insert(validate,hasSignal,sched);
			return sched.ScheduleNum;
		}

		///<summary>Set validate to true to throw an exception if start and stop times need to be validated.
		///If validate is set to false, then the calling code is responsible for the validation.  Also inserts necessary scheduleop enteries.
		///Inserts an invalid schedule signalod for each schedule passed in when hasSignal=true.</summary>
		public static void Insert(bool validate,bool hasSignal,params Schedule[] arraySchedules) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				List<Schedule> listSchedules=arraySchedules.ToList();
				//Unusual middle tier check. Break up into trips of 100.
				for(int i=0;i<listSchedules.Count;i+=100) {
					List<Schedule> listSchedulesToSend=listSchedules.GetRange(i,Math.Min(100,listSchedules.Count-i));
					Meth.GetVoid(MethodBase.GetCurrentMethod(),validate,hasSignal,listSchedulesToSend.ToArray());
				}
				return;
			}
			if(validate) {
				foreach(Schedule schedule in arraySchedules) {
					Validate(schedule);
				}
			}
			List<Schedule> listBulkInsert=arraySchedules.Where(x => x.Ops.Count==0).ToList();
			if(listBulkInsert.Count > 1) {
				Crud.ScheduleCrud.InsertMany(listBulkInsert);
			}
			else if(listBulkInsert.Count==1) {
				Crud.ScheduleCrud.Insert(listBulkInsert[0]);//If this is an individual schedule, it might be expecting its PK to be set.
			}
			//For schedules that have operatories, we have to insert each schedule one at a time in order to correctly set the PK for any subsequent FK 
			//relationships (e.g. scheduleops).
			foreach(Schedule schedule in arraySchedules.Where(x => x.Ops.Count > 0)) {
				Crud.ScheduleCrud.Insert(schedule);
			}
			if(hasSignal) {
				Signalods.SetInvalidSched(arraySchedules);
			}
			//Create a new ScheduleOp object for every single OperatoryNum within each schedule's Ops variable.
			List<ScheduleOp> listScheduleOps=arraySchedules.SelectMany(
					x => x.Ops.Select(y => new ScheduleOp { ScheduleNum=x.ScheduleNum,OperatoryNum=y })).ToList();
			//Bulk insert all of the schedule ops we just created.
			Crud.ScheduleOpCrud.InsertMany(listScheduleOps);
		}

		///<summary></summary>
		private static void Validate(Schedule sched){
			if(sched.StopTime>TimeSpan.FromDays(1)) {//if pasting to late afternoon, the stop time might be calculated as early the next morning.
				throw new Exception(Lans.g("Schedule","Stop time must be later than start time."));
			}
			if(sched.StartTime>sched.StopTime) {
				throw new Exception(Lans.g("Schedule","Stop time must be later than start time."));
			}
			if(sched.StartTime+TimeSpan.FromMinutes(5)>sched.StopTime	&& sched.Status==SchedStatus.Open) {
				throw new Exception(Lans.g("Schedule","Stop time cannot be the same as the start time."));
			}
		}

		///<summary>Goes to the db to look for overlaps if listSchedulesPossiblyOverlapping is null. Implemented for blockouts, 
		///but should work for other types, too. If listSchedulesPossiblyOverlapping is set, will not go to the database.</summary>
		public static bool Overlaps(Schedule sched,List<Schedule> listSchedulesPossiblyOverlapping=null){
			//No need to check RemotingRole; no call to db.
			List<Schedule> listOverlapSchedules;
			return Overlaps(sched,out listOverlapSchedules,listSchedulesPossiblyOverlapping);
		}

		///<summary>Goes to the db to look for overlaps if listSchedulesPossiblyOverlapping is null. Implemented for blockouts, 
		///but should work for other types, too. If listSchedulesPossiblyOverlapping is set, will not go to the database and use the list to find
		///overlapping schedules.</summary>
		public static bool Overlaps(Schedule sched,out List<Schedule> listOverlapSchedules,List<Schedule> listSchedulesPossiblyOverlapping=null) {
			//No need to check RemotingRole; no call to db.
			if(listSchedulesPossiblyOverlapping==null) {
				listSchedulesPossiblyOverlapping=Schedules.GetDayList(sched.SchedDate);
			}
			//No need to check RemotingRole; no call to db.
			listOverlapSchedules=Schedules.GetForType(listSchedulesPossiblyOverlapping,sched.SchedType,sched.ProvNum).ToList()
				.FindAll(x => x.ScheduleNum!=sched.ScheduleNum //not the same schedule
					&& (x.SchedType!=ScheduleType.Blockout || sched.Ops.Any(y => x.Ops.Contains(y)))//blockout that shares at least one op
					&& (sched.SchedDate.Date==x.SchedDate.Date) //May not be on the same day if using passed in list.
					&& (sched.StopTime>x.StartTime && sched.StartTime<x.StopTime));//time overlap
			return listOverlapSchedules.Count>0;
		}

		public static bool IsAppointmentBlocking(long defNum) {
			//No need to check RemotingRole; no call to db.
			return Defs.GetDefsForCategory(DefCat.BlockoutTypes,true)
				.FindAll(x => x.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()))
				.Select(x => x.DefNum).ToList().Contains(defNum);
		}

		///<summary>Delete an invalid schedule.  Insert an invalid schedule signalod when hasSignal=true.</summary>
		public static void Delete(Schedule sched,bool hasSignal=false){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sched,hasSignal);
				return;
			}
			string command= "DELETE from schedule WHERE schedulenum='"+POut.Long(sched.ScheduleNum)+"'";
 			Db.NonQ(command);
			command="DELETE FROM scheduleop WHERE ScheduleNum="+POut.Long(sched.ScheduleNum);
			Db.NonQ(command);
			if(sched.SchedType==ScheduleType.Provider){
				DeletedObjects.SetDeleted(DeletedObjectType.ScheduleProv,sched.ScheduleNum);
			}
			if(hasSignal) {
				Signalods.SetInvalidSched(sched);
			}
		}

		///<summary>Delete the schedules and their associated scheduleops.  Inserts an invalid schedule signalod.</summary>
		public static void DeleteMany(List<long> listSchedNums) {
			if(listSchedNums.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSchedNums);
				return;
			}
			//make deleted entries for synch purposes
			DeletedObjects.SetDeleted(DeletedObjectType.ScheduleProv,listSchedNums);
			//We need to query the database to get the list of schedules being deleted so we can run our logic to determine which signal refreshes need to be sent.
			//We use RefreshAndFill() because we need both the Schedule and ScheduleOp information to perform our signal logic.
			List <Schedule> listDeleteSchedules=RefreshAndFill("SELECT * FROM schedule WHERE ScheduleNum IN ("+string.Join(",",listSchedNums)+")");
			string command="DELETE FROM schedule WHERE ScheduleNum IN("+string.Join(",",listSchedNums)+")";
			Db.NonQ(command);
			command="DELETE FROM scheduleop WHERE ScheduleNum IN("+string.Join(",",listSchedNums)+")";
			Db.NonQ(command);
			Signalods.SetInvalidSched(listDeleteSchedules.ToArray());
		}
	
		///<summary>Supply a list of all Schedule for one day. Then, this filters out for one type.</summary>
		public static Schedule[] GetForType(List<Schedule> ListDay,ScheduleType schedType,long provNum){
			//No need to check RemotingRole; no call to db.
			return ListDay.FindAll(x => x.SchedType==schedType && x.ProvNum==provNum).ToArray();
		}

		///<summary>Supply a list of all Schedules for one day. Then, this filters out for one type.</summary>
		public static List<Schedule> GetListForType(List<Schedule> ListScheduleDay,ScheduleType schedType,long provNum){
			//No need to check RemotingRole; no call to db.
			return ListScheduleDay.FindAll(x => x.SchedType==schedType && x.ProvNum==provNum);
		}

		///<summary>Supply a list of Schedule . Then, this filters out for an employee.</summary>
		public static List<Schedule> GetForEmployee(List<Schedule> ListDay,long employeeNum) {
			//No need to check RemotingRole; no call to db.
			return ListDay.FindAll(x => x.SchedType==ScheduleType.Employee && x.EmployeeNum==employeeNum);
		}
		
		public static List<Schedule> GetSchedsForOp(Operatory op,List<DateTime> listApptDates) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),op,listApptDates);
			}
			if(listApptDates.Count==0) {//Should never happen.  If it does, the query will throw a UE for invalid syntax.
				listApptDates.Add(DateTime.Today);
			}
			string command="SELECT schedule.* FROM schedule INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum "
				+"WHERE scheduleop.OperatoryNum="+POut.Long(op.OperatoryNum)+" AND schedule.SchedDate IN("+string.Join(",",listApptDates.Select(x => POut.Date(x)))+")";
			List<Schedule> listScheds=Crud.ScheduleCrud.SelectMany(command);
			foreach(Schedule sched in listScheds) {
				sched.Ops.Add(op.OperatoryNum);//we know this schedule has op's operatorynum.  Add it here for later use.
			}
			return listScheds;
		}

		///<summary>Returns schedules with SchedType.Provider for a specific op.  This overload is for when the listForPeriod includes multiple days.</summary>
		public static List<Schedule> GetProvSchedsForOp(List<Schedule> listForPeriod,DayOfWeek dayOfWeek,Operatory op){
			//No need to check RemotingRole; no call to db.
			return GetProvSchedsForOp(listForPeriod.FindAll(x => x.SchedDate.DayOfWeek==dayOfWeek).Select(x => x.Copy()).ToList(),op);
		}

		///<summary>Returns schedules with SchedType.Provider for a specific op.  This overload is for when the listForPeriod includes only one day.</summary>
		public static List<Schedule> GetProvSchedsForOp(List<Schedule> listSchedulesPeriod,Operatory op){
			//No need to check RemotingRole; no call to db.
			List<Schedule> listSchedules=new List<Schedule>();
			foreach(Schedule schedule in listSchedulesPeriod.FindAll(x => x.SchedType==ScheduleType.Provider)) {//only schedules for provs
				if(schedule.Ops.Count(x => x!=0)>0) {//leaving count only non 0's, but 0's are no longer added in ConvertTableToList with remove empty entries code
					if(schedule.Ops.Contains(op.OperatoryNum)) {//the schedule is for specific op(s), add if it is for this op
						listSchedules.Add(schedule.Copy());
					}
					continue;
				}
				//the schedule is not for specific op(s), check op settings to see whether to add it
				if(op.ProvDentist>0 && !op.IsHygiene) {//op uses dentist
					if(schedule.ProvNum==op.ProvDentist) {
						listSchedules.Add(schedule.Copy());
					}
				}
				else if(op.ProvHygienist>0 && op.IsHygiene) {//op uses hygienist
					if(schedule.ProvNum==op.ProvHygienist) {
						listSchedules.Add(schedule.Copy());
					}
				}
				else {//op is either a hygiene op with no hygienist set or not a hygiene op with no provider set
					if(schedule.ProvNum==PrefC.GetLong(PrefName.ScheduleProvUnassigned)) {//use the provider for unassigned ops
						listSchedules.Add(schedule.Copy());
					}
				}
			}
			return listSchedules;
		}

		///<summary>If no provider is found for spot then the operatory provider is returned.</summary>
		///<param name="preferredProvNum">If there are multiple provider schedules that match this time and preferredProvNum is not zero, then
		///preferredProvNum if it is in listForPeriod. If it is not in the list, the first provider that matches will be returned.</param>
		public static long GetAssignedProvNumForSpot(List<Schedule> listForPeriod,Operatory op,bool isSecondary,DateTime aptDateTime,
			long preferredProvNum=0) 
		{
			//No need to check RemotingRole; no call to db.
			//first, look for a sched assigned specifically to that spot
			long matchingNonPreferredProvNum=0;
			for(int i=0;i<listForPeriod.Count;i++){
				if(listForPeriod[i].SchedType!=ScheduleType.Provider){
					continue;
				}
				if(aptDateTime.Date!=listForPeriod[i].SchedDate){
					continue;
				}
				if(!listForPeriod[i].Ops.Contains(op.OperatoryNum)){
					continue;
				}
				if(isSecondary && !Providers.GetIsSec(listForPeriod[i].ProvNum)){
					continue;
				}
				if(!isSecondary && Providers.GetIsSec(listForPeriod[i].ProvNum)){
					continue;
				}
				//for the time, if the sched starts later than the apt starts
				if(listForPeriod[i].StartTime > aptDateTime.TimeOfDay){
					continue;
				}
				//or if the sched ends (before or at same time) as the apt starts
				if(listForPeriod[i].StopTime <= aptDateTime.TimeOfDay){
					continue;
				}
				//matching sched found
				if(preferredProvNum!=0 && preferredProvNum!=listForPeriod[i].ProvNum) {
					if(matchingNonPreferredProvNum==0) {
						matchingNonPreferredProvNum=listForPeriod[i].ProvNum;
					}
					continue;//keep looking to see if we can find the preferred ProvNum
				}
				Plugins.HookAddCode(null,"Schedules.GetAssignedProvNumForSpot_Found",isSecondary);
				return listForPeriod[i].ProvNum;
			}
			if(matchingNonPreferredProvNum!=0) {
				//We couldn't match the preferred ProvNum, but we'll return one that did match.
				Plugins.HookAddCode(null,"Schedules.GetAssignedProvNumForSpot_Found",isSecondary);
				return matchingNonPreferredProvNum;
			}
			//if no matching sched found, then use the operatory
			Plugins.HookAddCode(null,"Schedules.GetAssignedProvNumForSpot_None",isSecondary);
			if(isSecondary){
				return op.ProvHygienist;
			}
			else{
				return op.ProvDentist;
			}
			//return 0;//none
		}

		///<summary>Comma delimits multiple schedules and creates a nice clean sting for screen legibility</summary>
		public static string GetCommaDelimStringForScheds(List<Schedule> listSchedules) {
			string retVal="";
			for(int i=0;i<listSchedules.Count;i++){
				if(i>0){
					retVal+=",";
				}
				retVal+=listSchedules[i].StartTime.ToShortTimeString()+"-"+listSchedules[i].StopTime.ToShortTimeString();
			}
			return retVal;
		}

		///<summary></summary>
		public static SerializableDictionary<long,double> GetHoursSchedForProvsInRange(List<long> listProvNums,List<long> listOpNums,
			DateTime start,DateTime end) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,double>(MethodBase.GetCurrentMethod(),listProvNums,listOpNums,start,end);
			}
			string command="SELECT * FROM schedule ";
			if(listOpNums!=null && listOpNums.Count!=0) {
				command+="INNER JOIN scheduleop ON scheduleop.ScheduleNum=schedule.ScheduleNum AND OperatoryNum IN("+string.Join(",",listOpNums)+") ";
			}
			command+="WHERE SchedType="+POut.Int((int)ScheduleType.Provider)+" "
				+"AND Status="+POut.Int((int)SchedStatus.Open)+" "
				+"AND "+DbHelper.BetweenDates("SchedDate",start,end)+" ";
			if(listProvNums.Count!=0) {
				command+="AND ProvNum IN ("+string.Join(",",listProvNums)+") ";
			}
			command+="ORDER BY SchedDate,ProvNum,StartTime";
			List<Schedule> listScheds=Crud.ScheduleCrud.SelectMany(command);
			SerializableDictionary<long,double> retVal=new SerializableDictionary<long, double>();
			Dictionary<long,List<Schedule>> dictProvScheds=listScheds.GroupBy(x => x.ProvNum).ToList()
				.ToDictionary(x => x.Key,x => listScheds.Where(y => y.ProvNum==x.Key).ToList());
			//Get a list of "Schedules" that are the distinct times without overlaps to which the provider has worked.
			List<Schedule> listProvScheds=GetProvSchedsForProductionGoals(dictProvScheds);
			foreach(Schedule sched in listProvScheds) { 
				TimeSpan timeDiff;
				if(!retVal.ContainsKey(sched.ProvNum)) {
					retVal.Add(sched.ProvNum,0);
				}
				timeDiff=sched.StopTime.Subtract(sched.StartTime);
				retVal[sched.ProvNum]=retVal[sched.ProvNum]+timeDiff.TotalHours;
			}
			return retVal;
		}

		///<summary>Returns a list of schedules for all of the providers passed in.The method considers overlapping  and gaps in the schedules passed in. 
		///I.e. a provider that is scheduled on one day from 8-12, 9-3, and 4-5 will return schedules of 8-3 and 4-5</summary>
		public static List<Schedule> GetProvSchedsForProductionGoals(Dictionary<long,List<Schedule>> dictProvScheds) {
			List<Schedule> retVal=new List<Schedule>();
			foreach(KeyValuePair<long,List<Schedule>> kvp in dictProvScheds) {
				//Order by sched date so that we don't miss out on any scheduled times when finding the distinct schedule spans below.
				kvp.Value.OrderBy(x => x.SchedDate)
					.ThenBy(x => x.StartTime)
					.ToList();
				//This is used to keep track of schedules that we have already considered, since we only have a reference to their StopTimes.
				DateTime lastStopDateTime=DateTime.MinValue;
				foreach(Schedule schedCur in kvp.Value) {
					//Ignore schedules that we have already passed.
					if(schedCur.SchedDate==lastStopDateTime.Date && schedCur.StopTime<=lastStopDateTime.TimeOfDay) {
						continue;
					}
					//Get the calculated end time for the current schedule.  This can span multiple schedules, which we will skip later.
					//This ensures that we do not hold duplicate scheduled times for this method, which would inflate production goal amounts.
					//Ex.  Sched1: 8am-3pm, Sched2: 1pm-5pm.  Without this method we would get 11hrs, when in reality it should be 9hrs.
					lastStopDateTime=new DateTime(schedCur.SchedDate.Ticks);
					lastStopDateTime=lastStopDateTime.AddTicks(GetEndTimeForProvSchedStartTime(schedCur,kvp.Value).Ticks);					
					Schedule schedToAdd=new Schedule() {
						ProvNum=kvp.Key,
						SchedDate=schedCur.SchedDate,
						StartTime=schedCur.StartTime,
						StopTime=lastStopDateTime.TimeOfDay,
					};
					retVal.Add(schedToAdd);
				}
			}
			return retVal;
		}

		///<summary>Gets a calculated StopTime based on all schedules that are passed in.  This is to ensure that we don't get duplicate schedule times.
		///Ex.  Sched1: 8am-3pm, Sched2: 1pm-5pm.  This will return 5pm because the actual schedule runs 8am-5pm even though its split out into multiple schedule rows.</summary>
		private static DateTime GetEndTimeForProvSchedStartTime(Schedule schedToFindEndTimeFor,List<Schedule> listSchedsForProv) {
			//The base case will simply return the StopTime for the passed in schedule.
			DateTime retVal=new DateTime(schedToFindEndTimeFor.StopTime.Ticks);
			List<Schedule> listSchedsOverlapping=listSchedsForProv.FindAll(x => x.SchedDate==schedToFindEndTimeFor.SchedDate 
					&& x.StopTime>schedToFindEndTimeFor.StopTime && x.StartTime<=schedToFindEndTimeFor.StopTime).ToList();
			//Loop through all schedules that end after the passed in schedule, and starts before the passed in schedule has ended.
			foreach(Schedule schedCur in listSchedsOverlapping) {
				//Recursively call this method to see if there are any schedules that overlap the new schedule but end after it.
				retVal=GetEndTimeForProvSchedStartTime(schedCur,listSchedsForProv);
			}
			return retVal;
		}

		///<summary>Clears all blockouts for day.</summary>
		public static void ClearBlockoutsForDay(DateTime date){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),date);
				return;
			}
			//Get ScheduleNums that are to be deleted so we can delete scheduleops
			string command="SELECT ScheduleNum FROM schedule WHERE SchedDate="+POut.Date(date)+" AND SchedType="+POut.Int((int)ScheduleType.Blockout);
			List<long> listSchedNums=Db.GetListLong(command);
			if(listSchedNums.Count==0) {
				return;//nothing to delete
			}
			string schedNumStr=string.Join(",",listSchedNums.Select(x => POut.Long(x)));
			//first delete schedules
			command="DELETE FROM schedule WHERE ScheduleNum IN("+schedNumStr+")";
			Db.NonQ(command);
			//then delete scheduleops for the deleted schedules.
			command="DELETE FROM scheduleop WHERE ScheduleNum IN("+schedNumStr+")";
			Db.NonQ(command);
			Signalods.SetInvalidSched(date);
		}

		public static void ClearBlockoutsForOp(long opNum, DateTime dateClear) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),opNum,dateClear);
				return;
			}
			//A schedule may be attached to more than one operatory.
			List<Schedule> listSchedules = Schedules.GetForDate(dateClear);
			listSchedules.RemoveAll(x => x.SchedType!=ScheduleType.Blockout);
			//Find the sched ops that we want to delete.
			List<ScheduleOp> listSchedOps = ScheduleOps.GetForSchedList(listSchedules);
			listSchedOps.RemoveAll(x => x.OperatoryNum!=opNum);
			ScheduleOps.DeleteBatch(listSchedOps.Select(x => x.ScheduleOpNum).ToList());
			//If deleting the sched op above caused the schedule to be orphaned it should be deleted.
			Schedules.DeleteOrphanedBlockouts(listSchedOps.Select(x => x.ScheduleNum).ToList());
			Dictionary <DateTime,List<long>> dictOpNumsForDates=new Dictionary<DateTime, List<long>>();
			dictOpNumsForDates[dateClear]=new List<long>() { opNum };
			Signalods.SetInvalidSchedForOps(dictOpNumsForDates);
		}

		public static void ClearBlockoutsForClinic(long clinicNum,DateTime dateClear) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicNum,dateClear);
				return;
			}
			//A schedule may be attached to more than one operatory.
			List<Schedule> listSchedules = Schedules.GetForDate(dateClear);
			listSchedules.RemoveAll(x => x.SchedType!=ScheduleType.Blockout);
			//Find the sched ops that we want to delete.
			List<long> listOpNums = Operatories.GetOpsForClinic(clinicNum).Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOp> listSchedOps = ScheduleOps.GetForSchedList(listSchedules);
			listSchedOps.RemoveAll(x => !listOpNums.Contains(x.OperatoryNum));
			ScheduleOps.DeleteBatch(listSchedOps.Select(x => x.ScheduleOpNum).ToList());
			//If deleting the sched op above caused the schedule to be orphaned it should be deleted.
			Schedules.DeleteOrphanedBlockouts(listSchedOps.Select(x => x.ScheduleNum).ToList());
			Dictionary <DateTime,List<long>> dictOpNumsForDates=new Dictionary<DateTime, List<long>>();
			dictOpNumsForDates[dateClear]=listSchedOps.Select(x => x.OperatoryNum).ToList();
			Signalods.SetInvalidSchedForOps(dictOpNumsForDates);
		}

		///<summary>Will only check for orphaned blockouts for those schedulenums passed in. Inserts an invalid schedule signalod.</summary>
		private static void DeleteOrphanedBlockouts(List<long> listScheduleNums) {
			//No need to check RemotingRole; private static.
			if(listScheduleNums.Count==0) {
				return;//nothing to delete
			}
			string command=$@"SELECT ScheduleNum FROM scheduleop WHERE ScheduleNum IN ({ string.Join(",",listScheduleNums) })";
			List<long> listScheduleNumsDoNotDelete=Db.GetListLong(command);
			List<string> listSchedNumsForDelete=listScheduleNums.Where(x => !listScheduleNumsDoNotDelete.Contains(x)).Select(x => POut.Long(x))
				.ToList();
			if(listSchedNumsForDelete.Count==0) {
				return;//nothing to delete
			}
			command="DELETE FROM schedule WHERE ScheduleNum IN ("+string.Join(",",listSchedNumsForDelete)+")";
			Db.NonQ(command);
		}

		///<summary>Similar to GetDayList but uses Crud pattern and classes.  No need to call RefreshAndFill since this is only used for the ScheduleNums</summary>
		private static List<Schedule> GetForDate(DateTime dateClear) {
			//No need to check RemotingRole; private static.
			string command="SELECT * FROM schedule Where SchedDate="+POut.Date(dateClear);
			return Crud.ScheduleCrud.SelectMany(command);
		}
		
		public static bool DateIsHoliday(DateTime date){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),date);
			}
			string command="SELECT COUNT(*) FROM schedule "
				//only count holiday schedules for the entire practice or for the currently selected clinic
				+"WHERE (ClinicNum=0 OR ClinicNum="+POut.Long(Clinics.ClinicNum)+") "
				+"AND Status="+POut.Int((int)SchedStatus.Holiday)+" "
				+"AND SchedType="+POut.Int((int)ScheduleType.Practice)+" "
				+"AND SchedDate="+POut.Date(date);
			string result=Db.GetCount(command);
			return result!="0";
		}

		///<summary>Gets all schedules for the given date and schedule type. Can optionally skip including ops in the schedule objects. If a list of 
		///op nums are passed in, only schedules in these operatories will be gotten.</summary>
		public static List<Schedule> GetAllForDateAndType(DateTime date,ScheduleType schedType,bool skipSchedOps=false,List<long> listOpNums=null) {
			//No need to check RemotingRole; no call to db.
			return GetAllForDateRangeAndType(date,date,schedType,skipSchedOps,listOpNums);
		}

		///<summary>Gets all schedules for the given date and schedule type. Can optionally skip including ops in the schedule objects. If a list of 
		///op nums are passed in, only schedules in these operatories will be gotten.</summary>
		public static List<Schedule> GetAllForDateRangeAndType(DateTime dateSelectedStart,DateTime dateSelectedEnd,ScheduleType schedType
			,bool skipSchedOps=false,List<long> listOpNums=null)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSelectedStart,dateSelectedEnd,schedType,skipSchedOps,
					listOpNums);
			}
			string command="SELECT schedule.* FROM schedule ";
			if(!listOpNums.IsNullOrEmpty()) {
				command+="INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum AND scheduleop.OperatoryNum IN ("
					+string.Join(",",listOpNums.Select(x => POut.Long(x)))+") ";
			}
			command+="WHERE SchedDate BETWEEN "+POut.Date(dateSelectedStart)+" AND "+POut.Date(dateSelectedEnd)+" "
				+"AND SchedType="+POut.Int((int)schedType)+" "
				+"GROUP BY schedule.ScheduleNum";
			return RefreshAndFill(command,skipSchedOps);
		}

		///<summary>Used by API.</summary>
		public static List<Schedule> GetForProv(DateTime dateSelectedStart,DateTime dateSelectedEnd,long provNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSelectedStart,dateSelectedEnd,provNum);
			}
			string command="SELECT * FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateSelectedStart)+" AND "+POut.Date(dateSelectedEnd)+" "//not a datetime. Between is inclusive.
				+"AND SchedType="+POut.Int((int)ScheduleType.Provider)+" "
				+"AND ProvNum="+POut.Long(provNum);
			return Crud.ScheduleCrud.SelectMany(command);
		}

		///<summary>Returns a 7 column data table in a calendar layout so all you have to do is draw it on the screen.</summary>
		public static DataTable GetPeriod(DateTime dateStart,DateTime dateEnd,List<long> provNums,List<long> empNums,bool includePNotes,
			bool includeCNotes,long clinicNum,bool showClinicSchedule,bool includeEmpNotes)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,provNums,empNums,includePNotes,includeCNotes,clinicNum,showClinicSchedule,includeEmpNotes);
			}
			DataTable table=new DataTable();
			table.Columns.Add("sun");
			table.Columns.Add("mon");
			table.Columns.Add("tues");
			table.Columns.Add("wed");
			table.Columns.Add("thurs");
			table.Columns.Add("fri");
			table.Columns.Add("sat");
			DataRow row;
			int rowsInGrid=GetRowCal(dateStart,dateEnd)+1;//because 0-based
			for(int i=0;i<rowsInGrid;i++){
				row=table.NewRow();
				table.Rows.Add(row);
			}
			DateTime dateSched=dateStart;
			while(dateSched<=dateEnd){
				table.Rows[GetRowCal(dateStart,dateSched)][(int)dateSched.DayOfWeek]=
					dateSched.ToString("MMM d, yyyy");
				dateSched=dateSched.AddDays(1);
			}
			//no schedules to show, just return the table with weeks and days in date range.
			if(provNums.Count==0 && empNums.Count==0 && !includeCNotes && !includePNotes) {
				return table;
			}
			//The following queries used to be one big query which ended up being very slow for larger customers (due to having AND (blah OR blah) AND...)
			//Therefore, we added a multi-column index and broke up the "OR clauses" which took the large query from ~17 seconds down to ~0.476 seconds.
			//This section of code will look ugly but is quite efficient with a schedule table of ~4.8 million rows (provider ~2,600 and employee ~1,800).
			#region Schedule Query Core
			string commandScheduleCore="SELECT Abbr,employee.FName,Note,SchedDate,SchedType,Status,StartTime,StopTime,schedule.ClinicNum,provider.ItemOrder "
				+"FROM schedule ";
			if(showClinicSchedule && clinicNum!=0) {
				commandScheduleCore+="INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum "
					+"INNER JOIN operatory ON scheduleop.OperatoryNum=operatory.OperatoryNum ";
			}
			commandScheduleCore+="LEFT JOIN provider ON schedule.ProvNum=provider.ProvNum "
				+"LEFT JOIN employee ON schedule.EmployeeNum=employee.EmployeeNum "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			if(showClinicSchedule && clinicNum!=0) {
				commandScheduleCore+="AND operatory.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			#endregion
			#region Dynamic Schedule Core
			string commandDynamicScheduleCore=
				"SELECT Abbr,employee.FName,Note,SchedDate,SchedType,Status,StartTime,StopTime,schedule.ClinicNum,provider.ItemOrder "
				+"FROM schedule "
				+"LEFT JOIN provider ON schedule.ProvNum=provider.ProvNum "
				+"LEFT JOIN operatory ON operatory.ProvDentist=provider.ProvNum OR operatory.ProvHygienist=provider.ProvNum "
				+"LEFT JOIN employee ON schedule.EmployeeNum=employee.EmployeeNum "
				+"LEFT JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND scheduleop.ScheduleNum IS NULL ";
			if(showClinicSchedule && clinicNum!=0) {
				commandDynamicScheduleCore+="AND operatory.ClinicNum="+POut.Long(clinicNum)+" ";
			}
			#endregion
			#region Schedule Filters
			List<string> listFilters=new List<string>();
			if(includePNotes) {//Only notes with clinicNum==0
				//Add a specific query for practice notes for both regular and dynamic schedule queries that will get UNION'd together later down.
				listFilters.Add("AND (SchedType="+POut.Int((int)ScheduleType.Practice)+" AND schedule.ClinicNum=0)");
			}
			if(includeCNotes) {//Only notes with clinicNum!=0; Treats HQ/ClinicNum==0 as show all non-practice notes.
				listFilters.Add("AND (SchedType="+POut.Int((int)ScheduleType.Practice)
					+" AND schedule.ClinicNum"+(clinicNum==0 ? ">0" : ("="+POut.Long(clinicNum)))+")");
			}
			if(provNums.Count>0) {
				listFilters.Add("AND schedule.ProvNum IN("+string.Join(",",provNums.Select(x => POut.Long(x)))+")");
			}
			if(empNums.Count>0) {
				listFilters.Add("AND schedule.EmployeeNum IN("+string.Join(",",empNums.Select(x => POut.Long(x)))+")");
			}
			#endregion
			string command="";
			//Make a standard and dynamic schedule query that is UNION'd together for each filter in the list.
			foreach(string filter in listFilters) {
				//Purposefully use a UNION instead of UNION ALL because we want to remove all duplicate rows.
				if(!string.IsNullOrEmpty(command)) {
					command+=" UNION ";
				}
				command+=commandScheduleCore+filter+" UNION "+commandDynamicScheduleCore+filter;
			}
			//If the for loop below changes to compare values in a row and the previous row, this query must be ordered by the additional comparison column
			command+=" ORDER BY SchedDate,FName,ItemOrder,StartTime,ClinicNum,Status";
			DataTable raw=Db.GetTable(command);
			DateTime startTime;
			DateTime stopTime;
			int rowI;
			for(int i=0;i<raw.Rows.Count;i++){
				dateSched=PIn.Date(raw.Rows[i]["SchedDate"].ToString());
				startTime=PIn.DateT(raw.Rows[i]["StartTime"].ToString());
				stopTime=PIn.DateT(raw.Rows[i]["StopTime"].ToString());
				rowI=GetRowCal(dateStart,dateSched);
				if(i!=0//not first row
					&& raw.Rows[i-1]["Abbr"].ToString()==raw.Rows[i]["Abbr"].ToString()//same provider as previous row
					&& raw.Rows[i-1]["FName"].ToString()==raw.Rows[i]["FName"].ToString()//same employee as previous row
					&& raw.Rows[i-1]["SchedDate"].ToString()==raw.Rows[i]["SchedDate"].ToString())//and same date as previous row
				{
					#region Not First Row and Same Prov/Emp/Date as Previous Row
					if(startTime.TimeOfDay==PIn.DateT("12 AM").TimeOfDay && stopTime.TimeOfDay==PIn.DateT("12 AM").TimeOfDay) {
						#region Note or Holiday
						if((PrefC.HasClinicsEnabled && raw.Rows[i-1]["ClinicNum"].ToString()!=raw.Rows[i]["ClinicNum"].ToString())//different clinic than previous line
							|| raw.Rows[i-1]["Status"].ToString()!=raw.Rows[i]["Status"].ToString())//start notes and holidays on different lines
						{
							table.Rows[rowI][(int)dateSched.DayOfWeek]+="\r\n";
							if(raw.Rows[i]["Status"].ToString()=="2") {//if holiday
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=Lans.g("Schedules","Holiday");
							}
							else {
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=Lans.g("Schedules","Note");
							}
							if(PrefC.HasClinicsEnabled && raw.Rows[i]["SchedType"].ToString()=="0") {//a practice sched type, prov/emp notes do not have a clinic associated
								string clinicAbbr=Clinics.GetAbbr(PIn.Long(raw.Rows[i]["ClinicNum"].ToString()));
								if(string.IsNullOrEmpty(clinicAbbr)) {
									clinicAbbr="Headquarters";
								}
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=" ("+clinicAbbr+")";
							}
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=":";
						}
						else {
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=",";
						}
						#endregion Note or Holiday
					}
					else {
						table.Rows[rowI][(int)dateSched.DayOfWeek]+=", ";//other than notes and holidays, if same emp or same prov and same date separate by commas
						table.Rows[rowI][(int)dateSched.DayOfWeek]+=startTime.ToString("h:mm")+"-"+stopTime.ToString("h:mm");
					}
					#endregion Not First Row and Same Prov/Emp/Date as Previous Row
				}
				else {
					#region First Row or Different Prov/Emp/Date as Previous Row
					table.Rows[rowI][(int)dateSched.DayOfWeek]+="\r\n";
					if(startTime.TimeOfDay==PIn.DateT("12 AM").TimeOfDay && stopTime.TimeOfDay==PIn.DateT("12 AM").TimeOfDay) {
						#region Note or Holiday
						if(raw.Rows[i]["Status"].ToString()=="2"){//if holiday
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=Lans.g("Schedules","Holiday");
						}
						else {//note
							if(raw.Rows[i]["Abbr"].ToString()!=""){
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=raw.Rows[i]["Abbr"].ToString()+" ";
							}
							if(raw.Rows[i]["FName"].ToString()!="") {
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=raw.Rows[i]["FName"].ToString()+" ";
							}
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=Lans.g("Schedules","Note");
						}
						if(PrefC.HasClinicsEnabled && raw.Rows[i]["SchedType"].ToString()=="0") {//a practice sched type, prov/emp notes do not have a clinic associated
							string clinicAbbr=Clinics.GetAbbr(PIn.Long(raw.Rows[i]["ClinicNum"].ToString()));
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=" ("+(string.IsNullOrEmpty(clinicAbbr)?"Headquarters":clinicAbbr)+")";
						}
						table.Rows[rowI][(int)dateSched.DayOfWeek]+=":";
						#endregion Note or Holiday
					}
					else {
						if(raw.Rows[i]["Abbr"].ToString()!="") {
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=raw.Rows[i]["Abbr"].ToString()+" ";
						}
						if(raw.Rows[i]["FName"].ToString()!="") {
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=raw.Rows[i]["FName"].ToString()+" ";
						}
						table.Rows[rowI][(int)dateSched.DayOfWeek]+=startTime.ToString("h:mm")+"-"+stopTime.ToString("h:mm");
					}
					#endregion First Row or Different Prov/Emp/Date as Previous Row
				}
				if(includeEmpNotes && raw.Rows[i]["Note"].ToString()!="") {
					table.Rows[rowI][(int)dateSched.DayOfWeek]+=" "+raw.Rows[i]["Note"].ToString();
				}
			}
			return table;
		}

		///<summary>Gets all schedules and blockouts that meet the Web Sched requirements.  Set isRecall to false to get New Pat Appt ops.
		///Setting clinicNum to 0 will only consider unassigned operatories.</summary>
		public static List<Schedule> GetSchedulesAndBlockoutsForWebSched(List<long> listProvNums,DateTime dateStart,DateTime dateEnd,bool isRecall
			,long clinicNum,Logger.IWriteLine log=null, List<Schedule> listRestrictedToBlockouts=null,bool isNewPat=false) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),listProvNums,dateStart,dateEnd,isRecall,clinicNum,log,
					listRestrictedToBlockouts,isNewPat);
			}
			List<long> listProvNumsWithZero=new List<long>();
			if(listProvNums!=null) {
				listProvNumsWithZero=listProvNums.Distinct().ToList();
			}
			if(!listProvNumsWithZero.Contains(0)) {
				listProvNumsWithZero.Add(0);//Always add 0 so that blockouts can be returned.
			}
			List<long> listBlockoutTypesToIgnore=new List<long>();
			List<long> listBlockoutTypeDefNums=new List<long>();
			List<long> listOperatoryNums=new List<long>();
			List<Operatory> listOperatories=new List<Operatory>();
			if(isRecall) {
				listBlockoutTypesToIgnore=PrefC.GetWebSchedRecallAllowedBlockouts;
				listOperatories=Operatories.GetOpsForWebSched();
			}
			else {
				listBlockoutTypesToIgnore=(isNewPat) ? PrefC.GetWebSchedNewPatAllowedBlockouts : PrefC.GetWebSchedExistingPatAllowedBlockouts;
				//Get all of the operatory nums for operatories for either WSNP or WSEP
				listOperatories=Operatories.GetOpsForWebSchedNewOrExistingPatAppts(isNewPat:isNewPat);
				if(listOperatories==null || listOperatories.Count < 1) {
					return new List<Schedule>(); //No operatories setup for this WS type.
				}
				List<DefLink> listAllRestrictedToDefLinks=DefLinks.GetDefLinksByType(DefLinkType.BlockoutType);
				//If the appointment type is not associated with any Restricted-To blockout types, then remove every distinct restricted-to blockout type from
				//the list of blockouts that can be scheduled over
				if(listRestrictedToBlockouts.IsNullOrEmpty()) {
					listBlockoutTypesToIgnore.RemoveAll(x => ListTools.In(x,listAllRestrictedToDefLinks.Select(y => y.FKey).Distinct()));
				}
				else { //If the appointment type is associated with some Restricted-To blockout types, then remove all blockouts from the list that do not match those blockouts
					listBlockoutTypesToIgnore=listAllRestrictedToDefLinks.Select(x => x.FKey).Distinct().ToList();
				}
			}
			//Get all blockout types that are not ignored in order to tell GetSchedulesHelper() which blockouts we need to know about.
			listBlockoutTypeDefNums=Defs.GetDefsForCategory(DefCat.BlockoutTypes)
					.FindAll(x => !ListTools.In(x.DefNum,listBlockoutTypesToIgnore))//listBlockoutTypesToIgnore contains a list of blockouts that can be scheduled on.
					.Select(x => x.DefNum).ToList();
			if(!listBlockoutTypeDefNums.Contains(0)) {
				listBlockoutTypeDefNums.Add(0);//Non-blockouts must always be considered.
			}
			listOperatoryNums.AddRange(listOperatories.Select(x => x.OperatoryNum));
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				listClinicNums.Add(clinicNum);
			}
			List<int> listSchedTypes=new List<int>() { (int)ScheduleType.Provider,(int)ScheduleType.Blockout };
			return GetSchedulesHelper(dateStart,dateEnd,listClinicNums,listOperatoryNums,listProvNumsWithZero,listBlockoutTypeDefNums,listSchedTypes,log);
		}

		///<summary>Gets a list of schedules for different methods.  Explicitly specify blockout types that need to be considered. Must be public for unit test.</summary>
		public static List<Schedule> GetSchedulesHelper(DateTime dateStart,DateTime dateEnd,List<long> listClinicNums,List<long> listOperatoryNums
			,List<long> listProvNums,List<long> listBlockoutTypeDefNums,List<int> listSchedTypes,Logger.IWriteLine log=null, bool isForMakeRecall=false)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listClinicNums,listOperatoryNums,listProvNums,
					listBlockoutTypeDefNums,listSchedTypes,log,isForMakeRecall);
			}
			//It is very important not to format these filters using DbHelper.DtimeToDate(). This would remove the index but yield the exact same results. 
			//It is already a Date column (no time) so no need to truncate the filter.
			if(listOperatoryNums==null || listOperatoryNums.Count < 1) {
				return new List<Schedule>();
			}
			if(listClinicNums.IsNullOrEmpty()) {
				listClinicNums.Add(0); //For customers without clinics. Necessary for filtering listOperatoryNums. 
			}
			if(listBlockoutTypeDefNums==null) {
				listBlockoutTypeDefNums=new List<long>();
				listBlockoutTypeDefNums.Add(0);
			}
			List<long> listFilteredOpNums=listOperatoryNums;
      if(PrefC.HasClinicsEnabled) {
				listFilteredOpNums=Operatories.GetOpNumsForClinics(listClinicNums).Where(x => listOperatoryNums.Contains(x)).ToList();
				if(listFilteredOpNums.Count==0) {
					throw new Exception("No operatories for these clinics.");
				}
			}
			List<long> listFilteredProvNums=ListTools.DeepCopy<long,long>(listProvNums); //Initial state
			List<long> listProvNumsDent;
			List<long> listProvNumsHyg;
			List<long> listProvsFromAllowedClinics=new List<long>();
			//create list of providers the current user is permitted to access.
			listProvsFromAllowedClinics=Providers.GetProvsForClinicList(listClinicNums).Select(x => x.ProvNum).Distinct().ToList();
			listFilteredProvNums.RemoveAll(x => !listProvsFromAllowedClinics.Contains(x)); //filter passed list of providers to remove those the user cannot access
			if(listProvNums.Contains(0)) {
				listFilteredProvNums.Add(0);  //will correctly display no results if filtered listProvidersNums is empty
			}
			listProvNumsDent=Operatories.GetOperatories(listFilteredOpNums)
				.Where(x => listFilteredProvNums.Contains(x.ProvDentist))
				.Select(x => x.ProvDentist)
				.Distinct().ToList();
			if(listProvNumsDent.IsNullOrEmpty() && listProvNums.Contains(0)) {
				listProvNumsDent.Add(0);
			}
			listProvNumsHyg=Operatories.GetOperatories(listFilteredOpNums)
				.Where(x => listFilteredProvNums.Contains(x.ProvHygienist))
				.Select(x => x.ProvHygienist)
				.Distinct().ToList();
			if(listProvNumsHyg.IsNullOrEmpty() && listProvNums.Contains(0)) {
				listProvNumsHyg.Add(0);
			}
			//-- First, get all schedules associated to operatories.
			string command;
			if(isForMakeRecall) {
				command=$@"
				(SELECT schedule.* FROM schedule
					LEFT JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
					LEFT JOIN operatory ON operatory.OperatoryNum=scheduleop.OperatoryNum
						AND operatory.OperatoryNum IN ({string.Join(",",listFilteredOpNums)}) 
					WHERE ";
				if(!listFilteredProvNums.IsNullOrEmpty()) {
					command+=$" schedule.ProvNum IN({ string.Join(",",listFilteredProvNums)}) AND";
				}
				command+=$@" schedule.BlockoutType IN ({string.Join(",",listBlockoutTypeDefNums.Select(x => POut.Long(x)))})
					AND schedule.SchedDate>={POut.Date(dateStart)}
					AND schedule.SchedDate<={POut.Date(dateEnd)}
					AND schedule.SchedType IN({string.Join(",",listSchedTypes.Select(x => POut.Int(x)))})
				)";
			}
			else {
				command=$@"
				(SELECT schedule.* FROM schedule
					INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
					INNER JOIN operatory ON operatory.OperatoryNum=scheduleop.OperatoryNum
					WHERE operatory.OperatoryNum IN ({string.Join(",",listFilteredOpNums)}) ";
				if(!listFilteredProvNums.IsNullOrEmpty()) {
					command+=$" AND schedule.ProvNum IN({ string.Join(",",listFilteredProvNums)})";
				}
				command+=$@" AND schedule.BlockoutType IN ({string.Join(",",listBlockoutTypeDefNums.Select(x => POut.Long(x)))})
					AND schedule.SchedDate>={POut.Date(dateStart)}
					AND schedule.SchedDate<={POut.Date(dateEnd)}
					AND schedule.SchedType IN({string.Join(",",listSchedTypes.Select(x => POut.Int(x)))})
				)";
			}
			//Large databases have trouble running the following query when it uses the OR keyword within the INNER JOIN clause.
			//E.g. INNER JOIN operatory ON schedule.ProvNum=operatory.ProvDentist OR schedule.ProvNum=operatory.ProvHygienist
			//Technically the aforementioned join clause runs successfully (with correct results) but takes significantly longer than two identical
			//queries with a simple join clause and then union-ing them together.
			//Therefore, we will loop twice, creating identical queries sans the provider join on the operatory table for speed purposes.
			foreach(string provColumnName in new List<string>() { "ProvDentist","ProvHygienist" }) {
				//-- Using UNION instead of UNION ALL because we want duplicate entries to be removed.
				//-- Next, get all schedules that are not associated to any operatories
				//-- Blockouts should be ignored because they HAVE to be assigned to an operatory.
				//-- Only consider schedules that are NOT assigned to any operatories (dynamic schedules)
				command+=$@"
					UNION 
					(SELECT schedule.* FROM schedule
						INNER JOIN provider ON schedule.ProvNum=provider.ProvNum
						INNER JOIN operatory ON schedule.ProvNum=operatory.{provColumnName}
						LEFT JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
						WHERE provider.IsHidden!=1 ";
				if(provColumnName=="ProvDentist" && !listProvNumsDent.IsNullOrEmpty()) { //includes relevant provider parameter if possible
					command+=$"AND provider.ProvNum IN ({String.Join(",",listProvNumsDent)}) ";
				}
				else if(provColumnName=="ProvHygienist" && !listProvNumsHyg.IsNullOrEmpty()) {
					command+=$"AND provider.ProvNum IN ({String.Join(",",listProvNumsHyg)}) ";
				}
				command+=$@"
						AND operatory.OperatoryNum IN ({string.Join(",",listFilteredOpNums)})
						AND schedule.BlockoutType = 0 
						AND scheduleop.OperatoryNum IS NULL 
						AND schedule.SchedDate>={POut.Date(dateStart)}
						AND schedule.SchedDate<={POut.Date(dateEnd)}
						AND schedule.SchedType IN({string.Join(",",listSchedTypes.Select(x => POut.Int(x)))})
					)";
			}
			command+=" ORDER BY SchedDate";//Order the entire result set by SchedDate.
			log?.WriteLine("command: "+command,LogLevel.Verbose);
			return RefreshAndFill(command);
		}

		///<summary>Returns the 0-based row where endDate will fall in a calendar grid.  It is not necessary to have a function to retrieve the column,
		///because that is simply (int)myDate.DayOfWeek</summary>
		public static int GetRowCal(DateTime startDate,DateTime endDate){
			//No need to check RemotingRole; no call to db.
			TimeSpan span=endDate-startDate;
			int dayInterval=span.Days;
			int daysFirstWeek=7-(int)startDate.DayOfWeek;//eg Monday=7-1=6.  or Sat=7-6=1.
			dayInterval=dayInterval-daysFirstWeek;
			if(dayInterval<0){
				return 0;
			}
			return (int)Math.Ceiling((dayInterval+1)/7d);
		}

		///<summary>When click on a calendar grid, this is used to calculate the date clicked on.  StartDate is the first date in the Calendar, which does
		///not have to be Sun.</summary>
		public static DateTime GetDateCal(DateTime startDate,int row,int col){
			//No need to check RemotingRole; no call to db.
			DateTime dateFirstRow;//the first date of row 0. Typically a few days before startDate. Always a Sun.
			dateFirstRow=startDate.AddDays(-(int)startDate.DayOfWeek);//example: (Tues,May 9).AddDays(-2)=Sun,May 7.
			int days=row*7+col;
			//peculiar bug.  When days=211 (startDate=4/1/10, row=30, col=1
			//and dateFirstRow=3/28/2010 and the current computer date is 4/14/10, and OS is Win7(possibly others),
			//dateFirstRow.AddDays(days)=10/24/10 00:59:58 (off by two seconds)
			//Spent hours trying to duplicate in isolated environment, but it behaves fine outside of this program.
			//Ticks are same, but result is different.
			//Commenting out the CultureInfo changes in FormOpenDental_Load did not help.
			//Not worth further debugging, so:
			DateTime retVal=dateFirstRow.AddDays(days).AddSeconds(5);
			return retVal.Date;
		}

		///<summary>Surround with try/catch.  Uses Sync to update the database with the changes made to listScheds from the stale listSchedsOld.</summary>
		public static void SetForDay(List<Schedule> listScheds,List<Schedule> listSchedsOld) {
			if(listScheds.Any(x=>x.StartTime>x.StopTime)) {
				throw new Exception(Lans.g("Schedule","Stop time must be later than start time."));
			}
			Sync(listScheds,listSchedsOld);
		}

		///<summary>Inserts, updates, or deletes the passed in listNew against the stale listOld.  Returns true if db changes were made.
		///This does not call the normal crud.Sync due to the special cases of DeletedObject and ScheduleOps.
		///This sends less data across middle teir for update logic, which is why remoting role occurs after we have filtered both lists.
		///Inserts an invalid schedule signal for the date of the first item in listNew (this is only called by SetForDay).</summary>
		public static bool Sync(List<Schedule> listNew,List<Schedule> listOld) {
			//No call to DB yet, remoting role to be checked later.
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<Schedule> listIns = new List<Schedule>();
			List<Schedule> listUpdNew = new List<Schedule>();
			List<Schedule> listUpdDB = new List<Schedule>();
			List<Schedule> listDel = new List<Schedule>();
			listNew.Sort((Schedule x,Schedule y) => { return x.ScheduleNum.CompareTo(y.ScheduleNum); });//Anonymous function, sorts by compairing PK. 
			listOld.Sort((Schedule x,Schedule y) => { return x.ScheduleNum.CompareTo(y.ScheduleNum); });//Anonymous function, sorts by compairing PK. 
			int idxNew = 0;
			int idxDB = 0;
			Schedule fieldNew;
			Schedule fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listOld.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listOld.Count) {
					fieldDB=listOld[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.ScheduleNum<fieldDB.ScheduleNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.ScheduleNum>fieldDB.ScheduleNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//This filters out schedules that do not need to be updated, instead of relying on the update new/old pattern to filter.
				//Everything past this point needs to increment idxNew and idxDB.
				else if(Crud.ScheduleCrud.UpdateComparison(fieldNew,fieldDB) || !fieldNew.Ops.OrderBy(x=>x).SequenceEqual(fieldDB.Ops.OrderBy(x=>x)))  //if the two lists are not identical
				{
					//Both lists contain the 'next' item, update required
					listUpdNew.Add(fieldNew);
					listUpdDB.Add(fieldDB);
				}
				idxNew++;
				idxDB++;
				//There is nothing to do with this schedule?
			}
			if(listIns.Count==0 && listUpdNew.Count==0 && listUpdDB.Count==0 && listDel.Count==0) {
				return false;//No need to go through remoting role check and following code because it will do nothing
			}
			//This sync logic was split up from the typical sync logic in order to restrict payload sizes that are sent over middle tier.
			//If this method starts having issues in the future we will need to serialize the lists into DataTables to further save size.
			bool isSuccess=SyncToDbHelper(listIns,listUpdNew,listUpdDB,listDel);
			if(isSuccess) {
				//We supress signal insertion in SyncToDbHelper since we know that this method is only called by SetForDay, we can use the date from the first
				//sched in either the new or old list (since either, but not both, can be empty at this point) and insert a generalized signal for that date.
				Signalods.SetInvalidSched(listNew.Concat(listOld).First().SchedDate);
			}
			return isSuccess;
		}

		///<summary>Inserts, updates, or deletes database rows sepcified in the supplied lists.  Returns true if db changes were made.
		///This was split from the list building logic to limit the payload that needed to be sent over middle tier.</summary>
		public static bool SyncToDbHelper(List<Schedule> listIns,List<Schedule> listUpdNew,List<Schedule> listUpdDB,List<Schedule> listDel) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listIns,listUpdNew,listUpdDB,listDel);
			}
			//Commit changes to DB 
			//to foreach loops
			for(int i = 0;i<listIns.Count;i++) {
				Insert(listIns[i],false,false);
			}
			for(int i = 0;i<listUpdNew.Count;i++) {
				Update(listUpdNew[i],listUpdDB[i],false,false);
			}
			for(int i = 0;i<listDel.Count;i++) {
				Delete(listDel[i],false);
			}
			if(listIns.Count>0 || listUpdNew.Count>0 || listDel.Count>0) {
				//Unlike base Sync pattern, we already know that anything in the listUpdNew should be updated.
				//filtering for update should have already been performed, otherwise the return value may be a false positive.
				return true;
			}
			return false;
		}

		///<summary>Clears all schedule entries for the given date range and the given providers, employees, and practice. 
		///Insert an invalid schedule signalod.</summary>
		public static void Clear(DateTime dateStart,DateTime dateEnd,List<long> provNums,List<long> empNums,bool includePNotes,bool includeCNotes,long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dateStart,dateEnd,provNums,empNums,includePNotes,includeCNotes,clinicNum);
				return;
			}
			DeleteMany(GetSchedulesToDelete(dateStart,dateEnd,provNums,empNums,includePNotes,includeCNotes,clinicNum).Select(x => x.ScheduleNum).ToList());
		}

		///<summary>Returns all Schedules that match the passed in arguments.</summary>
		public static List<Schedule> GetSchedulesToDelete(DateTime dateStart,DateTime dateEnd,List<long> provNums,List<long> empNums,bool includePNotes,
			bool includeCNotes,long clinicNum) 
		{
			if(provNums.Count==0 && empNums.Count==0 && !includeCNotes && !includePNotes) {
				return new List<Schedule>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,provNums,empNums,includePNotes,includeCNotes,clinicNum);
			}
			List<string> listOrClauses=new List<string>();
			//Only notes with clinicNum==0
			if(includePNotes) {
				listOrClauses.Add("(SchedType="+POut.Int((int)ScheduleType.Practice)+" AND ClinicNum=0)");
			}
			//Only notes with clinicNum!=0; Treats HQ/ClinicNum==0 as show all non-practice notes.
			if(includeCNotes) {
				listOrClauses.Add("(SchedType="+POut.Int((int)ScheduleType.Practice)+" AND ClinicNum"+(clinicNum==0?">0":("="+POut.Long(clinicNum)))+")");
			}
			if(provNums.Count>0) {
				listOrClauses.Add("schedule.ProvNum IN("+string.Join(",",provNums.Select(x => POut.Long(x)))+")");
			}
			if(empNums.Count>0) {
				listOrClauses.Add("schedule.EmployeeNum IN("+string.Join(",",empNums.Select(x => POut.Long(x)))+")");
			}
			string command="SELECT * FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND ("+string.Join(" OR ",listOrClauses)+")";
			return Crud.ScheduleCrud.SelectMany(command);
		}

		///<summary>Clears all Blockout schedule entries for the given date ranges and the given ops.</summary>
		public static void ClearBlockouts(DateTime dateStart,DateTime dateEnd,List<long> listOpNums,bool includeWeekend) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listOpNums,includeWeekend);
				return;
			}
			string command=$@"SELECT schedule.ScheduleNum,scheduleop.ScheduleOpNum,schedule.SchedDate,scheduleop.OperatoryNum
				FROM schedule 
				INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
				WHERE schedule.SchedType={POut.Int((int)ScheduleType.Blockout)}
				AND schedule.SchedDate BETWEEN {POut.Date(dateStart)} AND {POut.Date(dateEnd)}
				{(includeWeekend ? "" : "AND DAYOFWEEK(schedule.SchedDate) BETWEEN 2 AND 6")/*1 is Sunday and 7 is Saturday in MySQL*/}
				AND scheduleop.OperatoryNum IN({string.Join(",",listOpNums.Select(x => POut.Long(x)))})";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return;
			}
			command=$@"DELETE FROM scheduleop
				WHERE ScheduleOpNum IN ({string.Join(",",table.Select().Select(x => x["ScheduleOpNum"].ToString()))})";
			Db.NonQ(command);
			//If deleting the sched op above caused the schedule to be orphaned, it should be deleted.
			DeleteOrphanedBlockouts(table.Select().Select(x => PIn.Long(x["ScheduleNum"].ToString())).Distinct().ToList());
			Dictionary <DateTime,List<long>> dictOpNumsForDates=table.Select().GroupBy(x => PIn.Date(x["SchedDate"].ToString()))
				.ToDictionary(x => x.Key,x => x.Select(y => PIn.Long(y["OperatoryNum"].ToString())).ToList());
			Signalods.SetInvalidSchedForOps(dictOpNumsForDates);
		}

		public static int GetDuplicateBlockoutCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM schedule WHERE SchedType="+POut.Int((int)ScheduleType.Blockout);
			int retval=RefreshAndFill(command)
				.GroupBy(x=> new { x.SchedDate,x.StartTime,x.StopTime,ops=string.Join(",",x.Ops.OrderBy(y=>y)) })//group by duplicates
				.Sum(x=>x.Count()-1);//count duplicates, except the one original per group.
			return retval;
		}

		///<summary>Clear duplicate schedule entries.  Insert an invalid schedule signalod.</summary>
		public static void ClearDuplicates() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="SELECT * FROM schedule WHERE SchedType="+POut.Int((int)ScheduleType.Blockout);
			List<long> listDupSchedNums=RefreshAndFill(command)
				.GroupBy(x => new { x.SchedDate,x.StartTime,x.StopTime,ops=string.Join(",",x.Ops.OrderBy(y=>y)) })//group by duplicates
				.Select(x => x.Skip(1)) //each group represents a set of duplicates. First time in group is the "non-duplicate"
				.SelectMany(x=>x.Select(y=>y.ScheduleNum)).ToList(); //get those with dupes
			//We need to query the database to get the list of schedules being deleted so we can run our logic to determine which signal refreshes need to be sent.
			//We use RefreshAndFill() because we need both the Schedule and ScheduleOp information to perform our signal logic.
			List <Schedule> listDeleteSchedules=RefreshAndFill("SELECT * FROM schedule WHERE ScheduleNum IN ("+string.Join(",",listDupSchedNums)+")");
			command="DELETE FROM schedule WHERE ScheduleNum IN("+string.Join(",",listDupSchedNums)+")";
			long schedDel=Db.NonQ(command);
			command="DELETE FROM scheduleop WHERE ScheduleNum IN("+string.Join(",",listDupSchedNums)+")";
			long schedOpsDel=Db.NonQ(command);
			Signalods.SetInvalidSched(listDeleteSchedules.ToArray());
		}

		///<summary>Set clinicNum to 0 to return 'unassigned' clinics.  Otherwise, filters the data set on the clinic num passed in.
		///Added to the DataSet in Appointments.RefreshPeriod.</summary>
		public static DataTable GetPeriodEmployeeSchedTable(DateTime dateStart,DateTime dateEnd,long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,clinicNum);
			}
			DataTable table=new DataTable("EmpSched");
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("EmployeeNum");//only used to compare rows when there are multiple rows for one employee
			table.Columns.Add("empName");
			table.Columns.Add("schedule");
			table.Columns.Add("Note");
			if(dateStart.Date!=dateEnd.Date) {//This check is here to prevent filling the grids in week view.
				table.Columns.Remove("EmployeeNum");
				return table;
			}
			//Consider re-writing to use employee cache
			string command="SELECT employee.EmployeeNum,StartTime,StopTime,FName,Note,schedule.ScheduleNum,LName "
				+"FROM employee "
				+"INNER JOIN schedule ON schedule.EmployeeNum=employee.EmployeeNum "
				+"WHERE SchedType="+POut.Int((int)ScheduleType.Employee)+" "
				+"AND SchedDate="+POut.Date(dateStart)+" "
				+"AND StopTime>'00:00:00' "//We want to ignore invalid schedules, such as Provider/Employee notes.
				+"AND employee.IsHidden=0 ";
			if(PrefC.HasClinicsEnabled) {//Using clinics.
				List<Employee> listEmps=Employees.GetEmpsForClinic(clinicNum);
				if(listEmps.Count==0) {
					return table;
				}
				command+="AND employee.EmployeeNum IN ("+string.Join(",",listEmps.Select(x => x.EmployeeNum))+") ";
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY schedule.ScheduleNum ";
			}
			else {
				command+="GROUP BY employee.EmployeeNum,StartTime,StopTime,FName,Note,schedule.ScheduleNum,LName ";
			}
			//Sort by Emp num so that sort is deterministic
			command+="ORDER BY FName,LName,employee.EmployeeNum,StartTime";//order by FName for display, LName and EmployeeNum for emps with same FName
			DataTable raw=Db.GetTable(command);
			DataRow row;
			DateTime startTime;
			DateTime stopTime;
			foreach(DataRow rawRow in raw.Rows) {
				row=table.NewRow();
				row["EmployeeNum"]=rawRow["EmployeeNum"].ToString();
				if(table.Rows.Count==0 || rawRow["EmployeeNum"].ToString()!=table.Rows[table.Rows.Count-1]["EmployeeNum"].ToString()) {
					row["empName"]=rawRow["FName"].ToString();
				}
				startTime=PIn.DateT(rawRow["StartTime"].ToString());
				stopTime=PIn.DateT(rawRow["StopTime"].ToString());
				row["schedule"]=startTime.ToString("h:mm")+"-"+stopTime.ToString("h:mm");
				row["Note"]=rawRow["Note"].ToString();
				table.Rows.Add(row);
			}
			table.Columns.Remove("EmployeeNum");//Not necessary to drop this column, but it was not part of the original table when this code was refactored
			return table;
		}
	
		///<summary>Set clinicNum to 0 to return 'unassigned' clinics.  Otherwise, filters the data set on the clinic num passed in.
		///Added to the DataSet in Appointments.RefreshPeriod.</summary>
		public static DataTable GetPeriodProviderSchedTable(DateTime dateStart,DateTime dateEnd,long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,clinicNum);
			}
			DataTable table=new DataTable("ProvSched");
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("ProvAbbr");
			table.Columns.Add("schedule");
			table.Columns.Add("Note");
			if(dateStart.Date!=dateEnd.Date) {//This check is here to prevent filling the grids in week view.
				return table;
			}
			List<long> listProvNums;
			if(PrefC.HasClinicsEnabled) {//Using clinics.
				listProvNums=Providers.GetProvsForClinic(clinicNum).Select(x => x.ProvNum).ToList();
				if(listProvNums.Count==0) {
					return table;
				}
			}
			else {
				listProvNums=Providers.GetDeepCopy(true).OrderBy(x => x.ItemOrder).Select(y => y.ProvNum).ToList();
			}
			bool isODHQ=PrefC.IsODHQ;//Saves making deep copy of Pref cache for every schedule in ListSchedulesForDate.
			List<Schedule> ListSchedulesForDate=Schedules.GetAllForDateAndType(dateStart,ScheduleType.Provider);
			List<Schedule> listScheds=ListSchedulesForDate.FindAll(x => listProvNums.Contains(x.ProvNum) && (isODHQ ? (x.StartTime!=x.StopTime) : true));
			listScheds=listScheds.OrderBy(x => listProvNums.IndexOf(x.ProvNum)).ToList();//Make list alphabetical.
			Schedule schedCur;
			DataRow row;
			DateTime startTime;
			DateTime stopTime;
			for(int i=0; i<listScheds.Count; i++){
				schedCur=listScheds[i];
				row=table.NewRow();
				row["ProvAbbr"]=Providers.GetAbbr(schedCur.ProvNum);
				startTime=PIn.DateT(schedCur.StartTime.ToString());
				stopTime=PIn.DateT(schedCur.StopTime.ToString());
				row["schedule"]=startTime.ToString("h:mm")+"-"+stopTime.ToString("h:mm");
				row["Note"]=schedCur.Note;
				table.Rows.Add(row);
			}
			return table;
		}

		/// <summary></summary>
		/// <param name="doRunQueryOnNoOps">Set to false if an empty DataTable should be returned when listOpNums is null or empty.</param>
		/// <returns></returns>
		public static DataTable GetPeriodSchedule(DateTime dateStart,DateTime dateEnd,List<long> listOpNums=null,bool doRunQueryOnNoOps=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listOpNums,doRunQueryOnNoOps);
			}
			DataTable table=new DataTable("Schedule");
			table.Columns.Add("ScheduleNum");
			table.Columns.Add("SchedDate");
			table.Columns.Add("StartTime");
			table.Columns.Add("StopTime");
			table.Columns.Add("SchedType");
			table.Columns.Add("ProvNum");
			table.Columns.Add("BlockoutType");
			table.Columns.Add("Note");
			table.Columns.Add("Status");
			table.Columns.Add("ops");
			table.Columns.Add("EmployeeNum");
			table.Columns.Add("DateTStamp");
			table.Columns.Add("ClinicNum");
			if(!doRunQueryOnNoOps && (listOpNums==null || listOpNums.Count==0)) {
				//If no operatories are defined in an appointment view, the query below will select all schedules in the date range, regardless of clinic,
				//operatory, etc.  This is particularly problematic for large organizations that may have thousands of schedules each day.  Since no ops
				//are defined in this appointment view, it makes sense to return an empty DataTable as no schedules should ever be returned for a view 
				//without ops.
				return table;
			}
			//Go get every schedule for the date range passed in.
			//Left join on the scheduleop table as to get the necessary information needed to fill the custom "ops" column (above).
			//We will use C# to intelligently group and make a dictionary out of the query results later on in this method.
			//Sending back so much duplicated data is a shame for middle tier users but is faster than doing it with a sub-select within the query.
			string command="SELECT schedule.ScheduleNum,SchedDate,StartTime,StopTime,SchedType,ProvNum,BlockoutType,Note,"
				+"Status,EmployeeNum,DateTStamp,schedule.ClinicNum,scheduleop.OperatoryNum "
				+"FROM schedule "
				+"LEFT JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			if(listOpNums!=null && listOpNums.Count > 0) {
				command+="AND (scheduleop.OperatoryNum IN ("+string.Join(",",listOpNums.Select(x => POut.Long(x)))+") OR scheduleop.OperatoryNum IS NULL) ";
			}
			command+="ORDER BY StartTime";
			DataTable raw=Db.GetTable(command);
			//Since we did a left join within the query above there could be duplicate rows for the same schedule (sans the OperatoryNum column).
			//Group by ScheduleNum, using the first DataRow as the dictionary's key.
			//Then pull out all OperatoryNums related to the current schedule as the dictionary's value.
			Dictionary<DataRow,List<long>> dictSchedOps=raw.Rows.OfType<DataRow>()
					.GroupBy(x => PIn.Long(x["ScheduleNum"].ToString()),y => y)
					.ToDictionary(x => x.First(),y => y.Select(x => PIn.Long(x["OperatoryNum"].ToString())).Where(x => x > 0).ToList());
			DataRow row;
			foreach(KeyValuePair<DataRow,List<long>> kvp in dictSchedOps) {
				row=table.NewRow();
				row["ScheduleNum"]=kvp.Key["ScheduleNum"].ToString();
				row["SchedDate"]=kvp.Key["SchedDate"].ToString();
				row["StartTime"]=kvp.Key["StartTime"].ToString();
				row["StopTime"]=kvp.Key["StopTime"].ToString();
				row["SchedType"]=kvp.Key["SchedType"].ToString();
				row["ProvNum"]=kvp.Key["ProvNum"].ToString();
				row["BlockoutType"]=kvp.Key["BlockoutType"].ToString();
				row["Note"]=kvp.Key["Note"].ToString();
				row["Status"]=kvp.Key["Status"].ToString();
				//The dictionary's Key.OperatoryNum should not be used, use the dictionary's Value (list of OperatoryNums) instead.
				row["ops"]=string.Join(",",kvp.Value);
				row["EmployeeNum"]=kvp.Key["EmployeeNum"].ToString();
				row["DateTStamp"]=kvp.Key["DateTStamp"].ToString();
				row["ClinicNum"]=kvp.Key["ClinicNum"].ToString();
				table.Rows.Add(row);
			}
			return table;
		}

		///<summary>True if this blockout is not marked 'Do not schedule'.</summary>
		public static bool CanScheduleInBlockout(long blockoutType,List<Def> listDefs=null) {
			Def defBlockoutType=Defs.GetDef(DefCat.BlockoutTypes,blockoutType,listDefs);
			if(defBlockoutType.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription())) {
				return false;
			}
			return true;
		}

		/// <summary>Using the provided DefLinkNum, gets associated blockouts and returns the schedules for those blockouts within a date range.</summary>
		public static List<Schedule> GetRestrictedToBlockoutsByReason(long defNumReason,DateTime dateStart,DateTime dateStop,
			List<long> listOpNums,List<DefLink> listRestrictedBlockouts=null) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),defNumReason,dateStart,dateStop,listOpNums,listRestrictedBlockouts);
			}
			if(listOpNums==null || listOpNums.Count<1) {
				return new List<Schedule>();
			}
			if(listRestrictedBlockouts==null) {
				listRestrictedBlockouts=DefLinks.GetDefLinksByType(DefLinkType.BlockoutType,defNumReason);
			}
			if(listRestrictedBlockouts==null || listRestrictedBlockouts.Count<1) {
				return new List<Schedule>();
			}
			//See comments on schedule.ClinicNum for why it is not included in this query.  Schedules are typically linked to clinics via operatories via scheduleops.
			string command=$@"SELECT schedule.*
				FROM schedule
				INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
				WHERE schedule.SchedDate>={POut.Date(dateStart)} 
				AND schedule.SchedDate<={POut.Date(dateStop)}
				AND scheduleop.OperatoryNum IN ({string.Join(",",listOpNums)})
				AND schedule.BlockoutType IN ({ string.Join(",",listRestrictedBlockouts.Select(x => POut.Long(x.FKey)))}) 
				AND schedule.SchedType={POut.Int((int)ScheduleType.Blockout)}";
			return RefreshAndFill(command);
		}
	}
}













