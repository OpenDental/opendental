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
		private static bool _hasSet_group_concat_max_len;

		#region Get Methods
		public static List<Schedule> GetSchedListForDateRange(long employeeNum, DateTime dateStart, DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),employeeNum,dateStart,dateEnd);
			}
			string command="SELECT * FROM schedule where schedule.EmployeeNum="+employeeNum
				+" AND schedule.SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)
				+" ORDER BY schedule.SchedDate ASC";
			return RefreshAndFill(command,true);
		}

		///<summary>Gets a list of Schedule items for one date, ordered by start time.</summary>
		public static List<Schedule> GetDayList(DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		public static List<long> GetOverlappingSchedProvNums(List<long> listProvNums,Schedule schedule,List<Schedule> listSchedulesProvOnly
			,List<long> listSelectedOpNums)
		{
			//No need to check MiddleTierRole; no call to db.
			List<long> listProvsOverlap=new List<long>();
			if(listProvNums==null || schedule==null || listSchedulesProvOnly==null || listSelectedOpNums==null) {
				return listProvsOverlap;
			}
			for(int i=0;i<listProvNums.Count;i++) {//Potentially check each provider for overlap.
				Schedule scheduleTemp=schedule.Copy();
				scheduleTemp.ProvNum=listProvNums[i];
				if(scheduleTemp.IsNew) {
					listSchedulesProvOnly.Add(scheduleTemp);//new scheds will be added to check if overlap.
				}
				//====================SIMPLE OVERLAP, No-Ops==========================
				bool isOverlapping=false;
				if(scheduleTemp.Ops.Count==0) {//only look at schedules without operatories
					isOverlapping=(listSchedulesProvOnly.Where(x => x.ProvNum==scheduleTemp.ProvNum //Only consider current provider for overlaps w/o Ops
							&& x.Ops.Count==0 //Also doesn't have an operatory
							&& scheduleTemp.StartTime<x.StopTime //Overlapping Time
							&& scheduleTemp.StopTime>=x.StartTime)//Overlapping Time
						.Count() > 1);
				}
				//====================COMPLEX OVERLAP, Ops and All====================
				else if(!isOverlapping && scheduleTemp.Ops.Count>0) {//If we did not find a simple overlap, attempt to find a "complicated" overlap
					isOverlapping=(listSchedulesProvOnly.Where(x => x.Ops.Count>0 //Can only overlap if Ops are involved
							&& scheduleTemp.StartTime<x.StopTime //Overlapping Time
							&& scheduleTemp.StopTime>=x.StartTime //Overlapping Time
							&& x.Ops.Any(y => listSelectedOpNums.Contains(y)))//Schedule contains any operatory in question.
						.Count() > 1);
				}
				if(isOverlapping) {
					listProvsOverlap.Add(listProvNums[i]);
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
			//No need to check MiddleTierRole; no call to db.
			List<long> listProvsOverlap=new List<long>();
			if(listSchedules==null || listSchedules.Count < 1) {
				return listProvsOverlap;//Nothing to check overlapping against.  Return empty list.
			}
			//Get the currently scheduled provider schedules for the date range.
			List<Schedule> listSchedulesDbProv=GetAllForDateRangeAndType(dateStart,dateEnd,ScheduleType.Provider);
			//Filter out any schedules from the database that are related to the providers that should be ignored (user is going to replace existing).
			if(listIgnoreProvNums!=null) {
				listSchedulesDbProv.RemoveAll(x => listIgnoreProvNums.Contains(x.ProvNum));
			}
			if(dateStart.Date==dateEnd.Date) {//Daily mode.
				//Daily mode will take the entire list of schedules passed in and compare each schedule to the schedules in the database.
				for(int i=0;i<listSchedules.Count;i++) {
					List<long> listProvNums=new List<long>();
					listProvNums.Add(listSchedules[i].ProvNum);
					listProvsOverlap=listProvsOverlap.Union(GetOverlappingSchedProvNums(
							listProvNums,
							listSchedules[i],
							listSchedulesDbProv,
							listSchedules[i].Ops))
						.ToList();
				}
			}
			else {//Weekly mode.
				//Break up the list of provider schedules from the database into a dictionary grouped by schedDate.
				Dictionary<DateTime,List<Schedule>> dictDbProvSchedsByDate=listSchedulesDbProv.GroupBy(x => x.SchedDate).ToDictionary(x => x.Key,x => x.ToList());
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
					for(int i=0;i<listSchedsForDayOfWeek.Count;i++) {
						List<long> listProvNums=new List<long>();
						listProvNums.Add(listSchedsForDayOfWeek[i].ProvNum);
						listProvsOverlap=listProvsOverlap.Union(GetOverlappingSchedProvNums(
								listProvNums,
								listSchedsForDayOfWeek[i],
								dictDbProvSchedsByDate[dateSched],
								listSchedsForDayOfWeek[i].Ops))
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
			//No need to check MiddleTierRole; no call to db. Better to do this locally as it may take some time and we do not want Middle Tier to timeout.
			//It is allowed to paste back over the same day or week.
			List<long> listOpNums=ApptViewItems.GetOpsForView(apptViewNum);
			List<Schedule> listSchedulesToCopy=RefreshPeriodBlockouts(dateCopyStart,dateCopyEnd,listOpNums);
			//Build a list of blockouts that can't be Cut/Copy/Pasted
			List<Def> listDefsUserBlockout=Defs.GetDefsForCategory(DefCat.BlockoutTypes, true)
				.FindAll(x => x.ItemValue.Contains(BlockoutType.DontCopy.GetDescription()));
			//No SchedList only contains blockouts that are NOT marked "Do not Cut/Copy/Paste"
			listSchedulesToCopy.RemoveAll(x => listDefsUserBlockout.Any(y => y.DefNum==x.BlockoutType));
			int weekDelta=0;
			if(isWeek) {
				TimeSpan timeSpan=dateSelectedStart-dateCopyStart;
				weekDelta=timeSpan.Days/7;//usually a positive # representing a future paste, but can be negative
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
				ClearBlockouts(dateSelectedStart,dateEnd,listOpNums,includeWeekend);
			}
			else {
				//If we are not replacing, pull all schedules in the entire date range so we do not need to access the database every time.
				listSchedulesPossibleOverlap=GetAllForDateRangeAndType(dateSelectedStart,dateEnd,ScheduleType.Blockout,
					listOpNums:listOpNums);
			}
			//Stores all of the blockouts to be inserted.
			List<Schedule> listSchedulesNew=new List<Schedule>();
			int dayDelta=0;//this is needed when repeat pasting days in order to calculate skipping weekends.
			//dayDelta will start out zero and increment separately from r.
			for(int r=0;r<numRepeat;r++) {
				for(int i=0;i<listSchedulesToCopy.Count;i++) {
					Schedule schedule=listSchedulesToCopy[i].Copy();
					schedule.ScheduleNum=0;//So that overlap logic works.
					if(isWeek) {
						schedule.SchedDate=schedule.SchedDate.AddDays((weekDelta+r)*7);
					}
					else {
						schedule.SchedDate=dateSelectedStart.AddDays(dayDelta);
					}
					if(!doReplace && Overlaps(schedule,listSchedulesPossibleOverlap)) {
						Insert(validate:false,hasSignal:true,listSchedules:listSchedulesNew);
						string error=Lans.g("Schedule","A blockout overlaps with an existing blockout. Could not paste the blockout on")
							+" "+schedule.SchedDate.ToShortDateString()+" "+schedule.StartTime.ToShortTimeString();
						return error;
					}
					listSchedulesNew.Add(schedule);
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
			Insert(validate:false,hasSignal:true,listSchedules:listSchedulesNew);
			string logText=Lans.g("Schedule","Blockouts for operatories")+" ";
			for(int i=0;i<listOpNums.Count;i++) {
				if(i>0) {
					logText+=", ";
				}
				logText+=Operatories.GetOpName(listOpNums[i]);
			}
			logText+=" "+Lans.g("Schedule","copied from")+" "+dateCopyStart.ToShortDateString()+" ";
			if(dateCopyStart!=dateCopyEnd){
				logText+=Lans.g("Schedule","through")+" "+dateCopyEnd.ToShortDateString()+" ";
			}
			if(numRepeat>1) {
				logText+=Lans.g("Schedule","and pasted from")+" "+dateSelectedStart.ToShortDateString()+" "+Lans.g("Schedule","until");
			} 
			else {
				logText+=Lans.g("Schedule","and pasted to");
			}
			logText+=" "+dateEnd.ToShortDateString();
			SecurityLogs.MakeLogEntry(EnumPermType.Blockouts,0,logText);
			return "";
		}

		///<summary>Creates a securitylog that is constructed with definitions, operatories, dates, translation, etc. already done.
		///If several blockouts are cleared, specify the dateTime for the blockouts cleared and BlockoutAction.Clear.
		///If several blockouts are cleared for a specific operatory, specify the dateTime, operatory, and BlockoutAction.Clear
		///If several blockouts are cleared for specific operatory and clinic, specify the dateTime, operatory, clinic, and BlockoutAction.Clear
		///Otherwise, supply blockout and action taken.</summary>
		public static void BlockoutLogHelper(BlockoutAction blockoutAction,Schedule scheduleBlockout=null,DateTime dateTime=new DateTime(),long opNum=0,long clinicNum=-1) {
			string logText="";
			if(scheduleBlockout==null) {//Day cleared
				logText+=Lans.g("Schedule","Blockouts")+" ";
			}
			else if(scheduleBlockout.SchedType==ScheduleType.WebSchedASAP) {
				logText+=Lans.g("Schedule","Blockout of type Web Schedule ASAP Blockout ");
			}
			else {
				logText+=Lans.g("Schedule","Blockout of type")+" "+Defs.GetName(DefCat.BlockoutTypes,scheduleBlockout.BlockoutType)+" ";
			}
			switch(blockoutAction) {
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
			if(scheduleBlockout==null) {//Clear action taken for specific date
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
				if(scheduleBlockout.Ops.Count>1) {
					logText+=Lans.g("Schedule","operatories")+" ";
				}
				else {
					logText+=Lans.g("Schedule","operatory")+" ";
				}
				for(int i=0;i<scheduleBlockout.Ops.Count;i++) {
					if(i>0) {
						logText+=", ";
					}
					logText+=Operatories.GetOpName(scheduleBlockout.Ops[i]);
				}
				logText+=" "+Lans.g("Schedule","on")+" "+scheduleBlockout.SchedDate.ToShortDateString()+" "
					+Lans.g("Schedule","for")+" "+scheduleBlockout.StartTime.ToShortTimeString()+" - "+scheduleBlockout.StopTime.ToShortTimeString();
			}
			SecurityLogs.MakeLogEntry(EnumPermType.Blockouts,0,logText);
		}

		#endregion		

		///<summary>Used in the Schedules edit window to get a filtered list of schedule items in preparation for paste or repeat.</summary>
		public static List<Schedule> RefreshPeriod(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listEmpNums,bool includePNotes,
			bool includeCNotes,long clinicNum)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listEmpNums,includePNotes,includeCNotes,clinicNum);
			}
			if(listProvNums.Count==0 && listEmpNums.Count==0 && !includeCNotes && !includePNotes) {
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
			if(listProvNums.Count>0) {
				listOrClauses.Add("schedule.ProvNum IN ("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+")");
			}
			if(listEmpNums.Count>0) {
				listOrClauses.Add("schedule.EmployeeNum IN ("+string.Join(",",listEmpNums.Select(x => POut.Long(x)))+")");
			}
			string command="SELECT * FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND ("+string.Join(" OR ",listOrClauses)+")";
			return RefreshAndFill(command);
		}

		///<summary></summary>
		public static List<Schedule> RefreshPeriodBlockouts(DateTime dateStart,DateTime dateEnd,List<long> listOpNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listOpNums);
			}
			if(listOpNums.Count==0){
				return new List<Schedule>();
			}
			string command="SELECT * "
				+"FROM schedule "
				+"WHERE SchedType="+POut.Int((int)ScheduleType.Blockout)+" "
				+"AND SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND ScheduleNum IN (SELECT ScheduleNum FROM scheduleop WHERE OperatoryNum IN("+string.Join(",",listOpNums.Select(x => POut.Long(x)))+"))";
			return RefreshAndFill(command);
		}

		///<summary></summary>
		public static List<Schedule> RefreshDayEdit(DateTime dateSched){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSched);
			}
			string command="SELECT schedule.* "
				+"FROM schedule "
				+"WHERE SchedDate="+POut.Date(dateSched)+" "
				+"AND SchedType IN (0,1,3)";//Practice or Provider or Employee
			return RefreshAndFill(command);
		}

		///<summary>Gets a list of Schedule items for one date filtered by providers and employees.  Also option to include practice and clinic holidays and practice notes.</summary>
		public static List<Schedule> RefreshDayEditForPracticeProvsEmps(DateTime dateSched,List<long> listProvNums,List<long> listEmployeeNums,long clinicNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSched,listProvNums,listEmployeeNums,clinicNum);
			}
			List<string> listOrClauses=new List<string>();
			if(listProvNums.Count>0) {
				listOrClauses.Add("(SchedType="+POut.Int((int)ScheduleType.Provider)+" "
					+"AND ProvNum IN ("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+"))");
			}
			if(listEmployeeNums.Count>0) {
				listOrClauses.Add("(SchedType="+POut.Int((int)ScheduleType.Employee)+" "
					+"AND EmployeeNum IN ("+string.Join(",",listEmployeeNums.Select(x => POut.Long(x)))+"))");
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

		public static List<Schedule> RefreshPeriodForEmps(DateTime dateStart,DateTime dateEnd,List<long> listEmployeeNums) {
			if(listEmployeeNums.IsNullOrEmpty()) {
				return new List<Schedule>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listEmployeeNums);
			}
			string command="SELECT schedule.* "
				+"FROM schedule "
				+"WHERE SchedType="+POut.Int((int)ScheduleType.Employee)+" "
				+"AND EmployeeNum IN ("+string.Join(",",listEmployeeNums.Select(x => POut.Long(x)))+") "
				+"AND SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"ORDER BY SchedDate";
			return RefreshAndFill(command);
		}

		///<summary>Returns a list of schedules with at least one conflict using the given provider and clinics.  Returns empty list if no conflicts found.</summary>
		public static List<Schedule> GetClinicOverlapsForProv(DateTime dateFrom,DateTime dateTo,long provNum,List<long> listClinicNums) {
			DataTable tableSchedsForProvider=GetPeriodSchedsForProvsAndClinics(dateFrom,dateTo,new List<long>() {provNum },listClinicNums);
			//The datatable contains a row for each schedule and its clinic for this provider.  Now we go through it and determine if there are any overlaps.
			//Compare schedules and find ones that overlap.  If they do overlap, compare clinics.
			List<Schedule> listSchedulesConflict=new List<Schedule>(); 
			for(int i=0;i<tableSchedsForProvider.Rows.Count;i++) {
				DateTime date1=PIn.DateT(tableSchedsForProvider.Rows[i]["SchedDate"].ToString());
				TimeSpan timeSpanStart1=PIn.Time(tableSchedsForProvider.Rows[i]["StartTime"].ToString());
				TimeSpan timeSpanStop1=PIn.Time(tableSchedsForProvider.Rows[i]["StopTime"].ToString());
				long clinicNum1=PIn.Long(tableSchedsForProvider.Rows[i]["OpClinicNum"].ToString());
				long schedNum=PIn.Long(tableSchedsForProvider.Rows[i]["ScheduleNum"].ToString());
				if(listSchedulesConflict.Exists(x => x.SchedDate==date1 && x.StartTime==timeSpanStart1 && x.StopTime==timeSpanStop1)) {
					continue;
				}
				for(int j=0;j<tableSchedsForProvider.Rows.Count;j++) {
					DateTime date2=PIn.DateT(tableSchedsForProvider.Rows[j]["SchedDate"].ToString());
					TimeSpan timeSpanStart2=PIn.Time(tableSchedsForProvider.Rows[j]["StartTime"].ToString());
					TimeSpan timeSpanStop2=PIn.Time(tableSchedsForProvider.Rows[j]["StopTime"].ToString());
					long clinicNum2=PIn.Long(tableSchedsForProvider.Rows[j]["OpClinicNum"].ToString());
					if(date1!=date2 || clinicNum1==clinicNum2) {
						continue;
					}
					//Their clinics don't match and are on the same day, let's see if they overlap.
					if((timeSpanStart1<=timeSpanStart2 && timeSpanStop1>=timeSpanStart2)//conflict due to the start time being between start and stop.
						|| (timeSpanStop1>=timeSpanStop2 && timeSpanStart1<=timeSpanStop2)) //conflict due to the end time being between start and stop
					{
						Schedule schedule=new Schedule();
						schedule.ClinicNum=clinicNum1;
						schedule.SchedDate=date1;
						schedule.StartTime=timeSpanStart1;
						schedule.StopTime=timeSpanStop1;
						schedule.ProvNum=provNum;
						listSchedulesConflict.Add(schedule);
						break;
					}
				}
			}
			return listSchedulesConflict;
		}

		///<summary>Returns a table of schedules with their clinic (gotten from operatory if a link exists) for the providers specified, for the date range specified, and for the clinics specified.</summary>
		public static DataTable GetPeriodSchedsForProvsAndClinics(DateTime dateFrom,DateTime dateTo,List<long> listProvNums,List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		public static List<Schedule> GetTwoYearPeriod(DateTime dateStart) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart);
			}
			string command="SELECT schedule.* "
				+"FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateStart.AddYears(2))+" "
				+"AND SchedType IN (0,1,3)";//Practice or Provider or Employee
			return RefreshAndFill(command);
		}

		///<summary></summary>
		public static List<Schedule> GetSchedulesForAppointmentSearch(DateTime dateStart,DateTime dateEnd,List<long> listClinicNums,
			List<long> listOpNums,List<long> listProvNums,List<long> listBlockoutTypes, bool isForMakeRecall=false)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listClinicNums,listOpNums,listProvNums,listBlockoutTypes,isForMakeRecall);
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
			listBlockoutTypeDefNums.AddRange(listBlockoutTypes);
			List<int> listSchedTypes=new List<int>();
			listSchedTypes.Add((int)ScheduleType.Provider);
			listSchedTypes.Add((int)ScheduleType.Blockout);
			listSchedTypes.Add((int)ScheduleType.Practice);
			listSchedTypes.Add((int)ScheduleType.Employee);
			return GetSchedulesHelper(dateStart,dateEnd,listClinicNums,listOpNums,listProvNums,listBlockoutTypeDefNums,listSchedTypes,isForMakeRecall: isForMakeRecall);
		}

		///<summary>Used in the check database integrity tool.  Does NOT fill the list of operatories per schedule.</summary>
		public static Schedule[] RefreshAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Schedule[]>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM schedule";
			return RefreshAndFill(command,true).ToArray();
		}

		public static List<Schedule> GetChangedSince(DateTime changedSince) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),changedSince);
			}
			string command="SELECT * schedule WHERE DateTStamp>"+POut.DateT(changedSince)+" AND SchedType="+POut.Int((int)ScheduleType.Provider);
			return RefreshAndFill(command);
		}

		///<summary>All ~19 places where this is called must already handle passing off to middle tier before this is called.  Retrieves all schedules from the db using command, then retrieves all scheduleops for the schedules and fills the schedule.Ops list with OperatoryNums for the schedule.</summary>
		private static List<Schedule> RefreshAndFill(string command,bool skipSchedOps=false) {
			//No need to check MiddleTierRole; private method.
			//Not a typical refreshandfill, as it contains a query.
			List<Schedule> listSchedules=Crud.ScheduleCrud.SelectMany(command);
			if(listSchedules.Count==0 || skipSchedOps) {
				return listSchedules;
			}
			//Set group_concat_max_len for this session so that the comma-separated 'Ops' string may not be truncated.
			//The default length of 1024 could easily get hit with random primary keys.
			//Group_concat_max_len is constrained by max_allowed_packet, so set group_concat_max_len to the global value of max_allowed_packet.
			//If that's not big enough, they will have problems in other places before here.
			if(!_hasSet_group_concat_max_len){
				int maxAllowedPacket=MiscData.GetMaxAllowedPacket();
				command="SET SESSION group_concat_max_len = " + POut.Int(maxAllowedPacket);
				Db.NonQ(command);
				_hasSet_group_concat_max_len=true;
			}
			command="SELECT ScheduleNum,"
				+"GROUP_CONCAT(DISTINCT OperatoryNum SEPARATOR ',' ) 'Ops' "
				+"FROM scheduleop WHERE ScheduleNum IN("+string.Join(",",listSchedules.Select(x => x.ScheduleNum))+") "
				+"GROUP BY ScheduleNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return listSchedules;
			}
			for(int i=0;i<table.Rows.Count;i++) {//looping through schedules
				List<long> listOpNumsDb=table.Rows[i]["Ops"].ToString().Split(',').Select(x => PIn.Long(x)).ToList();
				//Jordan I wanted to get rid of the FindAll, but couldn't prove that there were no duplicate rows with same ScheduleNum
				//Looked at all 19 spots, and one spot had some inner joins which could create that problem.
				listSchedules.FindAll(x => x.ScheduleNum==PIn.Long(table.Rows[i]["ScheduleNum"].ToString()))//find schedules that have 1+ scheduleops.
					.ForEach(x => x.Ops=listOpNumsDb);
			}
			return listSchedules;
		}

		public static List<Schedule> ConvertTableToList(DataTable table){
			//No need to check MiddleTierRole; no call to db.
			List<Schedule> listSchedules=Crud.ScheduleCrud.TableToList(table);
			if(!table.Columns.Contains("ops")) {
				return listSchedules;
			}
			for(int i = 0;i<listSchedules.Count;i++) {
				listSchedules[i].Ops=table.Rows[i]["ops"].ToString().Split(new[] { "," },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
			}
			return listSchedules;
		}

		///<summary>Update a schedule.  Insert an invalid schedule signalod.</summary>
		public static void Update(Schedule schedule){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),schedule);
				return;
			}
			Validate(schedule);
			Crud.ScheduleCrud.Update(schedule);
			string command="DELETE FROM scheduleop WHERE ScheduleNum="+POut.Long(schedule.ScheduleNum);
			Db.NonQ(command);
			Signalods.SetInvalidSched(schedule);
			for(int i=0;i<schedule.Ops.Count;i++) {
				ScheduleOp scheduleOp=new ScheduleOp();
				scheduleOp.ScheduleNum=schedule.ScheduleNum;
				scheduleOp.OperatoryNum=schedule.Ops[i];
				ScheduleOps.Insert(scheduleOp);
			}
		}

		///<summary>Similar to Crud.ScheduleCrud.Update except this also handles ScheduleOps.  Insert an invalid schedule signalod when hasSignal=true.</summary>
		public static void Update(Schedule scheduleNew,Schedule scheduleOld,bool validate,bool hasSignal=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),scheduleNew,scheduleOld,validate,hasSignal);
				return;
			}
			if(validate) {
				Validate(scheduleNew);
			}
			Crud.ScheduleCrud.Update(scheduleNew,scheduleOld); //may not cause an update, but we still need to check for updates to ScheduleOps below.
			if(hasSignal) {
				Signalods.SetInvalidSched(scheduleNew);
			}
			//Sort Ops for SequenceEqual call below.
			scheduleNew.Ops.Sort();
			scheduleOld.Ops.Sort();
			if(scheduleNew.Ops.SequenceEqual(scheduleOld.Ops)) {  //If both lists contain exactly the same ops.
				return;//no updates to ScheduleOps needed
			}
			string command="DELETE FROM scheduleop WHERE ScheduleNum="+POut.Long(scheduleNew.ScheduleNum);
			Db.NonQ(command);
			//re-insert ScheduleOps based on the list of opnums in schedNew.Ops
			for(int i=0;i<scheduleNew.Ops.Count;i++) {
				ScheduleOp scheduleOp=new ScheduleOp();
				scheduleOp.ScheduleNum=scheduleNew.ScheduleNum;
				scheduleOp.OperatoryNum=scheduleNew.Ops[i];
				ScheduleOps.Insert(scheduleOp);
			}
		}

		///<summary>Set validate to true to throw an exception if start and stop times need to be validated.  If validate is set to false, then the calling code is responsible for the validation.  Also inserts necessary scheduleop enteries.  Insert an invalid schedule signalod when hasSignal=true.</summary>
		public static long Insert(Schedule schedule,bool validate,bool hasSignal=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//Remoting role check so that the PK can be returned to the client.
				schedule.ScheduleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),schedule,validate,hasSignal);
				return schedule.ScheduleNum;
			}
			Insert(validate,hasSignal,new List<Schedule>(){ schedule});
			return schedule.ScheduleNum;
		}

		///<summary>Set validate to true to throw an exception if start and stop times need to be validated.
		///If validate is set to false, then the calling code is responsible for the validation.  Also inserts necessary scheduleop enteries.
		///Inserts an invalid schedule signalod for each schedule passed in when hasSignal=true.</summary>
		public static void Insert(bool validate,bool hasSignal,List<Schedule> listSchedules=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				//Unusual middle tier check. Break up into trips of 100.
				for(int i=0;i<listSchedules.Count;i+=100) {
					List<Schedule> listSchedulesToSend=listSchedules.GetRange(i,Math.Min(100,listSchedules.Count-i));
					Meth.GetVoid(MethodBase.GetCurrentMethod(),validate,hasSignal,listSchedulesToSend.ToArray());
				}
				return;
			}
			if(validate) {
				for(int i=0;i<listSchedules.Count();i++) {
					Validate(listSchedules[i]);
				}
			}
			List<Schedule> listSchedulesBulkInsert=listSchedules.FindAll(x => x.Ops.Count==0);
			if(listSchedulesBulkInsert.Count > 1) {
				Crud.ScheduleCrud.InsertMany(listSchedulesBulkInsert);
			}
			else if(listSchedulesBulkInsert.Count==1) {
				Crud.ScheduleCrud.Insert(listSchedulesBulkInsert[0]);//If this is an individual schedule, it might be expecting its PK to be set.
			}
			//For schedules that have operatories, we have to insert each schedule one at a time in order to correctly set the PK for any subsequent FK 
			//relationships (e.g. scheduleops).
			List<Schedule> listSchedulesWithOps=listSchedules.FindAll(x => x.Ops.Count > 0);
			for(int i=0;i<listSchedulesWithOps.Count();i++) {
				Crud.ScheduleCrud.Insert(listSchedulesWithOps[i]);
			}
			if(hasSignal) {
				Signalods.SetInvalidSched(listSchedules.ToArray());
			}
			//Create a new ScheduleOp object for every single OperatoryNum within each schedule's Ops variable.
			List<ScheduleOp> listScheduleOps=listSchedules.SelectMany(
					x => x.Ops.Select(y => new ScheduleOp { ScheduleNum=x.ScheduleNum,OperatoryNum=y })).ToList();
			//Bulk insert all of the schedule ops we just created.
			Crud.ScheduleOpCrud.InsertMany(listScheduleOps);
		}

		///<summary></summary>
		private static void Validate(Schedule schedule){
			if(schedule.StopTime>TimeSpan.FromDays(1)) {//if pasting to late afternoon, the stop time might be calculated as early the next morning.
				throw new Exception(Lans.g("Schedule","Stop time must be later than start time."));
			}
			if(schedule.StartTime>schedule.StopTime) {
				throw new Exception(Lans.g("Schedule","Stop time must be later than start time."));
			}
			if(schedule.StartTime+TimeSpan.FromMinutes(5)>schedule.StopTime	&& schedule.Status==SchedStatus.Open) {
				throw new Exception(Lans.g("Schedule","Stop time cannot be the same as the start time."));
			}
		}

		///<summary>Goes to the db to look for overlaps if listSchedulesPossiblyOverlapping is null. Implemented for blockouts, 
		///but should work for other types, too. If listSchedulesPossiblyOverlapping is set, will not go to the database.</summary>
		public static bool Overlaps(Schedule schedule,List<Schedule> listSchedulesPossiblyOverlapping=null){
			//No need to check MiddleTierRole; no call to db.
			List<Schedule> listSchedulesOverlap;
			return Overlaps(schedule,out listSchedulesOverlap,listSchedulesPossiblyOverlapping);
		}

		///<summary>Goes to the db to look for overlaps if listSchedulesPossiblyOverlapping is null. Implemented for blockouts, 
		///but should work for other types, too. If listSchedulesPossiblyOverlapping is set, will not go to the database and use the list to find
		///overlapping schedules.</summary>
		public static bool Overlaps(Schedule schedule,out List<Schedule> listSchedulesOverlap,List<Schedule> listSchedulesPossiblyOverlapping=null) {
			//No need to check MiddleTierRole; no call to db.
			if(listSchedulesPossiblyOverlapping==null) {
				listSchedulesPossiblyOverlapping=Schedules.GetDayList(schedule.SchedDate);
			}
			//No need to check MiddleTierRole; no call to db.
			listSchedulesOverlap=Schedules.GetListForType(listSchedulesPossiblyOverlapping,schedule.SchedType,schedule.ProvNum)
				.FindAll(x => x.ScheduleNum!=schedule.ScheduleNum //not the same schedule
					&& (x.SchedType!=ScheduleType.Blockout || schedule.Ops.Any(y => x.Ops.Contains(y)))//blockout that shares at least one op
					&& (schedule.SchedDate.Date==x.SchedDate.Date) //May not be on the same day if using passed in list.
					&& (schedule.StopTime>x.StartTime && schedule.StartTime<x.StopTime));//time overlap
			return listSchedulesOverlap.Count>0;
		}

		public static bool IsAppointmentBlocking(long defNum) {
			//No need to check MiddleTierRole; no call to db.
			List<Def> listDefsBlockout=Defs.GetDefsForCategory(DefCat.BlockoutTypes,true)
				.FindAll(x => x.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription()));
			bool isBlocking=listDefsBlockout.Exists(x=>x.DefNum==defNum);
			return isBlocking;
		}

		///<summary>Delete an invalid schedule.  Insert an invalid schedule signalod when hasSignal=true.</summary>
		public static void Delete(Schedule schedule,bool hasSignal=false){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),schedule,hasSignal);
				return;
			}
			string command= "DELETE from schedule WHERE schedulenum='"+POut.Long(schedule.ScheduleNum)+"'";
 			Db.NonQ(command);
			command="DELETE FROM scheduleop WHERE ScheduleNum="+POut.Long(schedule.ScheduleNum);
			Db.NonQ(command);
			if(schedule.SchedType==ScheduleType.Provider){
				DeletedObjects.SetDeleted(DeletedObjectType.ScheduleProv,schedule.ScheduleNum);
			}
			if(hasSignal) {
				Signalods.SetInvalidSched(schedule);
			}
		}

		///<summary>Delete the schedules and their associated scheduleops.  Inserts an invalid schedule signalod.</summary>
		public static void DeleteMany(List<long> listScheduleNums) {
			if(listScheduleNums.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listScheduleNums);
				return;
			}
			//make deleted entries for synch purposes
			DeletedObjects.SetDeleted(DeletedObjectType.ScheduleProv,listScheduleNums);
			//We need to query the database to get the list of schedules being deleted so we can run our logic to determine which signal refreshes need to be sent.
			//We use RefreshAndFill() because we need both the Schedule and ScheduleOp information to perform our signal logic.
			List <Schedule> listSchedulesDelete=RefreshAndFill("SELECT * FROM schedule WHERE ScheduleNum IN ("+string.Join(",",listScheduleNums)+")");
			string command="DELETE FROM schedule WHERE ScheduleNum IN("+string.Join(",",listScheduleNums)+")";
			Db.NonQ(command);
			command="DELETE FROM scheduleop WHERE ScheduleNum IN("+string.Join(",",listScheduleNums)+")";
			Db.NonQ(command);
			Signalods.SetInvalidSched(listSchedulesDelete.ToArray());
		}

		///<summary>Supply a list of all Schedules for one day. Then, this filters out for one type.</summary>
		public static List<Schedule> GetListForType(List<Schedule> listSchedules,ScheduleType scheduleType,long provNum){
			//No need to check MiddleTierRole; no call to db.
			return listSchedules.FindAll(x => x.SchedType==scheduleType && x.ProvNum==provNum);
		}

		///<summary>Supply a list of Schedule . Then, this filters out for an employee.</summary>
		public static List<Schedule> GetForEmployee(List<Schedule> listSchedules,long employeeNum) {
			//No need to check MiddleTierRole; no call to db.
			return listSchedules.FindAll(x => x.SchedType==ScheduleType.Employee && x.EmployeeNum==employeeNum);
		}
		
		public static List<Schedule> GetSchedsForOp(Operatory operatory,List<DateTime> listDatesAppt) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),operatory,listDatesAppt);
			}
			if(listDatesAppt.Count==0) {//Should never happen.  If it does, the query will throw a UE for invalid syntax.
				listDatesAppt.Add(DateTime.Today);
			}
			string command="SELECT schedule.* FROM schedule INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum "
				+"WHERE scheduleop.OperatoryNum="+POut.Long(operatory.OperatoryNum)+" AND schedule.SchedDate IN("+string.Join(",",listDatesAppt.Select(x => POut.Date(x)))+")";
			List<Schedule> listSchedules=Crud.ScheduleCrud.SelectMany(command);
			for(int i=0;i<listSchedules.Count;i++) {
				listSchedules[i].Ops.Add(operatory.OperatoryNum);//we know this schedule has op's operatorynum.  Add it here for later use.
			}
			return listSchedules;
		}

		///<summary>Returns schedules with SchedType.Provider for a specific op.  This overload is for when the listSchedules includes multiple days.</summary>
		public static List<Schedule> GetProvSchedsForOp(List<Schedule> listSchedules,DayOfWeek dayOfWeek,Operatory operatory){
			//No need to check MiddleTierRole; no call to db.
			List<Schedule> listSchedulesPeriod=listSchedules.FindAll(x => x.SchedDate.DayOfWeek==dayOfWeek).Select(x => x.Copy()).ToList();
			return GetProvSchedsForOp(listSchedulesPeriod,operatory);
		}

		///<summary>Returns schedules with SchedType.Provider for a specific op.  This overload is for when the listForPeriod includes only one day.</summary>
		public static List<Schedule> GetProvSchedsForOp(List<Schedule> listSchedulesPeriod,Operatory operatory){
			//No need to check MiddleTierRole; no call to db.
			List<Schedule> listSchedules=new List<Schedule>();
			List<Schedule> listSchedulesProv=listSchedulesPeriod.FindAll(x => x.SchedType==ScheduleType.Provider);
			for(int i=0;i<listSchedulesProv.Count;i++) {//only schedules for provs
				if(listSchedulesProv[i].Ops.Count(x => x!=0)>0) {//leaving count only non 0's, but 0's are no longer added in ConvertTableToList with remove empty entries code
					if(listSchedulesProv[i].Ops.Contains(operatory.OperatoryNum)) {//the schedule is for specific op(s), add if it is for this op
						listSchedules.Add(listSchedulesProv[i].Copy());
					}
					continue;
				}
				//the schedule is not for specific op(s), check op settings to see whether to add it
				if(operatory.ProvDentist>0 && !operatory.IsHygiene) {//op uses dentist
					if(listSchedulesProv[i].ProvNum==operatory.ProvDentist) {
						listSchedules.Add(listSchedulesProv[i].Copy());
					}
				}
				else if(operatory.ProvHygienist>0 && operatory.IsHygiene) {//op uses hygienist
					if(listSchedulesProv[i].ProvNum==operatory.ProvHygienist) {
						listSchedules.Add(listSchedulesProv[i].Copy());
					}
				}
				else {//op is either a hygiene op with no hygienist set or not a hygiene op with no provider set
					if(listSchedulesProv[i].ProvNum==PrefC.GetLong(PrefName.ScheduleProvUnassigned)) {//use the provider for unassigned ops
						listSchedules.Add(listSchedulesProv[i].Copy());
					}
				}
			}
			return listSchedules;
		}

		///<summary>If no provider is found for spot then the operatory provider is returned.</summary>
		///<param name="preferredProvNum">If there are multiple provider schedules that match this time and preferredProvNum is not zero, then
		///preferredProvNum if it is in listSchedulesPeriod. If it is not in the list, the first provider that matches will be returned.</param>
		public static long GetAssignedProvNumForSpot(List<Schedule> listSchedulesPeriod,Operatory operatory,bool isSecondary,DateTime dateTime,
			long preferredProvNum=0) 
		{
			//No need to check MiddleTierRole; no call to db.
			//first, look for a sched assigned specifically to that spot
			long matchingNonPreferredProvNum=0;
			for(int i=0;i<listSchedulesPeriod.Count;i++){
				if(listSchedulesPeriod[i].SchedType!=ScheduleType.Provider){
					continue;
				}
				if(dateTime.Date!=listSchedulesPeriod[i].SchedDate){
					continue;
				}
				if(!listSchedulesPeriod[i].Ops.Contains(operatory.OperatoryNum)){
					continue;
				}
				if(isSecondary && !Providers.GetIsSec(listSchedulesPeriod[i].ProvNum)){
					continue;
				}
				if(!isSecondary && Providers.GetIsSec(listSchedulesPeriod[i].ProvNum)){
					continue;
				}
				//for the time, if the sched starts later than the apt starts
				if(listSchedulesPeriod[i].StartTime > dateTime.TimeOfDay){
					continue;
				}
				//or if the sched ends (before or at same time) as the apt starts
				if(listSchedulesPeriod[i].StopTime <= dateTime.TimeOfDay){
					continue;
				}
				//matching sched found
				if(preferredProvNum!=0 && preferredProvNum!=listSchedulesPeriod[i].ProvNum) {
					if(matchingNonPreferredProvNum==0) {
						matchingNonPreferredProvNum=listSchedulesPeriod[i].ProvNum;
					}
					continue;//keep looking to see if we can find the preferred ProvNum
				}
				Plugins.HookAddCode(null,"Schedules.GetAssignedProvNumForSpot_Found",isSecondary);
				return listSchedulesPeriod[i].ProvNum;
			}
			if(matchingNonPreferredProvNum!=0) {
				//We couldn't match the preferred ProvNum, but we'll return one that did match.
				Plugins.HookAddCode(null,"Schedules.GetAssignedProvNumForSpot_Found",isSecondary);
				return matchingNonPreferredProvNum;
			}
			//if no matching sched found, then use the operatory
			Plugins.HookAddCode(null,"Schedules.GetAssignedProvNumForSpot_None",isSecondary);
			if(isSecondary){
				return operatory.ProvHygienist;
			}
			return operatory.ProvDentist;
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
			DateTime dateStart,DateTime dateEnd) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,double>(MethodBase.GetCurrentMethod(),listProvNums,listOpNums,dateStart,dateEnd);
			}
			string command="SELECT * FROM schedule ";
			if(listOpNums!=null && listOpNums.Count!=0) {
				command+="INNER JOIN scheduleop ON scheduleop.ScheduleNum=schedule.ScheduleNum AND OperatoryNum IN("+string.Join(",",listOpNums)+") ";
			}
			command+="WHERE SchedType="+POut.Int((int)ScheduleType.Provider)+" "
				+"AND Status="+POut.Int((int)SchedStatus.Open)+" "
				+"AND "+DbHelper.BetweenDates("SchedDate",dateStart,dateEnd)+" ";
			if(listProvNums.Count!=0) {
				command+="AND ProvNum IN ("+string.Join(",",listProvNums)+") ";
			}
			command+="ORDER BY SchedDate,ProvNum,StartTime";
			List<Schedule> listSchedules=Crud.ScheduleCrud.SelectMany(command);
			SerializableDictionary<long,double> retVal=new SerializableDictionary<long, double>();
			Dictionary<long,List<Schedule>> dictProvScheds=listSchedules.GroupBy(x => x.ProvNum).ToList()
				.ToDictionary(x => x.Key,x => listSchedules.Where(y => y.ProvNum==x.Key).ToList());
			//Get a list of "Schedules" that are the distinct times without overlaps to which the provider has worked.
			List<Schedule> listSchedulesProv=GetProvSchedsForProductionGoals(dictProvScheds);
			for(int i=0;i<listSchedulesProv.Count;i++) { 
				TimeSpan timeSpan;
				if(!retVal.ContainsKey(listSchedulesProv[i].ProvNum)) {
					retVal.Add(listSchedulesProv[i].ProvNum,0);
				}
				timeSpan=listSchedulesProv[i].StopTime.Subtract(listSchedulesProv[i].StartTime);
				retVal[listSchedulesProv[i].ProvNum]=retVal[listSchedulesProv[i].ProvNum]+timeSpan.TotalHours;
			}
			return retVal;
		}

		///<summary>Returns a list of schedules for all of the providers passed in.The method considers overlapping  and gaps in the schedules passed in. 
		///I.e. a provider that is scheduled on one day from 8-12, 9-3, and 4-5 will return schedules of 8-3 and 4-5</summary>
		public static List<Schedule> GetProvSchedsForProductionGoals(Dictionary<long,List<Schedule>> dictionaryProvScheds) {
			List<Schedule> listSchedules=new List<Schedule>();
			foreach(KeyValuePair<long,List<Schedule>> kvp in dictionaryProvScheds) {
				//Order by sched date so that we don't miss out on any scheduled times when finding the distinct schedule spans below.
				kvp.Value.OrderBy(x => x.SchedDate)
					.ThenBy(x => x.StartTime)
					.ToList();
				//This is used to keep track of schedules that we have already considered, since we only have a reference to their StopTimes.
				DateTime dateTime=DateTime.MinValue;
				for(int i=0;i<kvp.Value.Count;i++) {
					//Ignore schedules that we have already passed.
					if(kvp.Value[i].SchedDate==dateTime.Date && kvp.Value[i].StopTime<=dateTime.TimeOfDay) {
						continue;
					}
					//Get the calculated end time for the current schedule.  This can span multiple schedules, which we will skip later.
					//This ensures that we do not hold duplicate scheduled times for this method, which would inflate production goal amounts.
					//Ex.  Sched1: 8am-3pm, Sched2: 1pm-5pm.  Without this method we would get 11hrs, when in reality it should be 9hrs.
					dateTime=new DateTime(kvp.Value[i].SchedDate.Ticks);
					dateTime=dateTime.AddTicks(GetEndTimeForProvSchedStartTime(kvp.Value[i],kvp.Value).Ticks);
					Schedule schedule=new Schedule();
					schedule.ProvNum=kvp.Key;
					schedule.SchedDate=kvp.Value[i].SchedDate;
					schedule.StartTime=kvp.Value[i].StartTime;
					schedule.StopTime=dateTime.TimeOfDay;
					listSchedules.Add(schedule);
				}
			}
			return listSchedules;
		}

		///<summary>Gets a calculated StopTime based on all schedules that are passed in.  This is to ensure that we don't get duplicate schedule times.
		///Ex.  Sched1: 8am-3pm, Sched2: 1pm-5pm.  This will return 5pm because the actual schedule runs 8am-5pm even though its split out into multiple schedule rows.</summary>
		private static DateTime GetEndTimeForProvSchedStartTime(Schedule schedule,List<Schedule> listSchedulesForProv) {
			List<Schedule> listSchedulesOrdered = listSchedulesForProv.FindAll(x => x.SchedDate==schedule.SchedDate)
				.OrderBy(x => x.StartTime).ToList(); //Order the list, just in case we didn't prior to coming in here
			TimeSpan timeSpan = schedule.StopTime;
			for(int i = 0;i<listSchedulesOrdered.Count;i++) {
				if(listSchedulesOrdered[i].StartTime>timeSpan) {
					//Time is no longer contiguous, break out
					break;
				}
				if(listSchedulesOrdered[i].StopTime>timeSpan) {
					//Set the stop time, since it is greater than what is currently set
					timeSpan=listSchedulesOrdered[i].StopTime;
				}
			}
			return new DateTime(timeSpan.Ticks);
		}

		///<summary>Clears all blockouts for day.</summary>
		public static void ClearBlockoutsForDay(DateTime date){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),date);
				return;
			}
			//Get ScheduleNums that are to be deleted so we can delete scheduleops
			string command="SELECT ScheduleNum FROM schedule WHERE SchedDate="+POut.Date(date)+" AND SchedType="+POut.Int((int)ScheduleType.Blockout);
			List<long> listScheduleNums=Db.GetListLong(command);
			if(listScheduleNums.Count==0) {
				return;//nothing to delete
			}
			string schedNumStr=string.Join(",",listScheduleNums.Select(x => POut.Long(x)));
			//first delete schedules
			command="DELETE FROM schedule WHERE ScheduleNum IN("+schedNumStr+")";
			Db.NonQ(command);
			//then delete scheduleops for the deleted schedules.
			command="DELETE FROM scheduleop WHERE ScheduleNum IN("+schedNumStr+")";
			Db.NonQ(command);
			Signalods.SetInvalidSched(date);
		}

		public static void ClearBlockoutsForOp(long opNum, DateTime dateClear) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),opNum,dateClear);
				return;
			}
			//A schedule may be attached to more than one operatory.
			List<Schedule> listSchedules=GetForDate(dateClear);
			listSchedules.RemoveAll(x => x.SchedType!=ScheduleType.Blockout);
			//Find the sched ops that we want to delete.
			List<ScheduleOp> listScheduleOps = ScheduleOps.GetForSchedList(listSchedules);
			listScheduleOps.RemoveAll(x => x.OperatoryNum!=opNum);
			ScheduleOps.DeleteBatch(listScheduleOps.Select(x => x.ScheduleOpNum).ToList());
			//If deleting the sched op above caused the schedule to be orphaned it should be deleted.
			DeleteOrphanedBlockouts(listScheduleOps.Select(x => x.ScheduleNum).ToList());
			List<Schedule> listSchedulesSetInvalid=new List<Schedule>();
			Schedule schedule=new Schedule();
			schedule.SchedDate=dateClear;
			schedule.Ops=new List<long>() { opNum };
			listSchedulesSetInvalid.Add(schedule);
			Signalods.SetInvalidSchedForOps(listSchedulesSetInvalid);
		}

		public static void ClearBlockoutsForClinic(long clinicNum,DateTime dateClear) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicNum,dateClear);
				return;
			}
			//A schedule may be attached to more than one operatory.
			List<Schedule> listSchedules = GetForDate(dateClear);
			listSchedules.RemoveAll(x => x.SchedType!=ScheduleType.Blockout);
			//Find the sched ops that we want to delete.
			List<long> listOpNums = Operatories.GetOpsForClinic(clinicNum).Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOp> listScheduleOps = ScheduleOps.GetForSchedList(listSchedules);
			listScheduleOps.RemoveAll(x => !listOpNums.Contains(x.OperatoryNum));
			ScheduleOps.DeleteBatch(listScheduleOps.Select(x => x.ScheduleOpNum).ToList());
			//If deleting the sched op above caused the schedule to be orphaned it should be deleted.
			DeleteOrphanedBlockouts(listScheduleOps.Select(x => x.ScheduleNum).ToList());
			List<Schedule> listSchedulesSetInvalid=new List<Schedule>();
			Schedule schedule=new Schedule();
			schedule.SchedDate=dateClear;
			schedule.Ops=listScheduleOps.Select(x => x.OperatoryNum).ToList();;
			listSchedulesSetInvalid.Add(schedule);
			Signalods.SetInvalidSchedForOps(listSchedulesSetInvalid);
		}

		///<summary>Will only check for orphaned blockouts for those schedulenums passed in. Inserts an invalid schedule signalod.</summary>
		private static void DeleteOrphanedBlockouts(List<long> listScheduleNums) {
			//No need to check MiddleTierRole; private static.
			if(listScheduleNums.Count==0) {
				return;//nothing to delete
			}
			string command=$@"SELECT ScheduleNum FROM scheduleop WHERE ScheduleNum IN ({ string.Join(",",listScheduleNums) })";
			List<long> listScheduleNumsDoNotDelete=Db.GetListLong(command);
			List<string> listScheduleNumsForDelete=listScheduleNums.Where(x => !listScheduleNumsDoNotDelete.Contains(x)).Select(x => POut.Long(x))
				.ToList();
			if(listScheduleNumsForDelete.Count==0) {
				return;//nothing to delete
			}
			command="DELETE FROM schedule WHERE ScheduleNum IN ("+string.Join(",",listScheduleNumsForDelete)+")";
			Db.NonQ(command);
		}

		///<summary>Similar to GetDayList but uses Crud pattern and classes.  No need to call RefreshAndFill since this is only used for the ScheduleNums</summary>
		private static List<Schedule> GetForDate(DateTime dateClear) {
			//No need to check MiddleTierRole; private static.
			string command="SELECT * FROM schedule Where SchedDate="+POut.Date(dateClear);
			return Crud.ScheduleCrud.SelectMany(command);
		}
		
		public static bool DateIsHoliday(DateTime date){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		public static List<Schedule> GetAllForDateAndType(DateTime date,ScheduleType scheduleType,bool skipSchedOps=false,List<long> listOpNums=null) {
			//No need to check MiddleTierRole; no call to db.
			return GetAllForDateRangeAndType(date,date,scheduleType,skipSchedOps,listOpNums);
		}

		///<summary>Gets all schedules for the given date and schedule type. Can optionally skip including ops in the schedule objects. If a list of 
		///op nums are passed in, only schedules in these operatories will be gotten.</summary>
		public static List<Schedule> GetAllForDateRangeAndType(DateTime dateSelectedStart,DateTime dateSelectedEnd,ScheduleType scheduleType
			,bool skipSchedOps=false,List<long> listOpNums=null)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSelectedStart,dateSelectedEnd,scheduleType,skipSchedOps,
					listOpNums);
			}
			string command="SELECT schedule.* FROM schedule ";
			if(!listOpNums.IsNullOrEmpty()) {
				command+="INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum AND scheduleop.OperatoryNum IN ("
					+string.Join(",",listOpNums.Select(x => POut.Long(x)))+") ";
			}
			command+="WHERE SchedDate BETWEEN "+POut.Date(dateSelectedStart)+" AND "+POut.Date(dateSelectedEnd)+" "
				+"AND SchedType="+POut.Int((int)scheduleType)+" "
				+"GROUP BY schedule.ScheduleNum";
			return RefreshAndFill(command,skipSchedOps);
		}

		///<summary>Gets all scheduled holidays for the given date range based on the ClinicNum.</summary>
		public static List<Schedule> GetAllHolidaysForDateRange(DateTime dateSelectedStart,DateTime dateSelectedEnd,List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSelectedStart,dateSelectedEnd,listClinicNums);
			}
			string command="SELECT schedule.* FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateSelectedStart)+" AND "+POut.Date(dateSelectedEnd)+" "
				+"AND Status="+POut.Int((int)SchedStatus.Holiday)+" ";
			if(listClinicNums.Count>0) {
				command+="AND schedule.ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			return Crud.ScheduleCrud.SelectMany(command);
		}

		///<summary>Used by API.</summary>
		public static List<Schedule> GetForProv(DateTime dateSelectedStart,DateTime dateSelectedEnd,long provNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateSelectedStart,dateSelectedEnd,provNum);
			}
			string command="SELECT * FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateSelectedStart)+" AND "+POut.Date(dateSelectedEnd)+" "//not a datetime. Between is inclusive.
				+"AND SchedType="+POut.Int((int)ScheduleType.Provider)+" "
				+"AND ProvNum="+POut.Long(provNum);
			return Crud.ScheduleCrud.SelectMany(command);
		}

		///<summary>Returns a 7 column data table in a calendar layout so all you have to do is draw it on the screen.</summary>
		public static DataTable GetPeriod(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listEmployeeNums,bool includePNotes,
			bool includeCNotes,long clinicNum,bool showClinicSchedule,bool includeEmpNotes)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listEmployeeNums,includePNotes,includeCNotes,clinicNum,showClinicSchedule,includeEmpNotes);
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
			while(true){
				if(dateSched>dateEnd) {
					break;
				}
				table.Rows[GetRowCal(dateStart,dateSched)][(int)dateSched.DayOfWeek]=
					dateSched.ToString("MMM d, yyyy");
				dateSched=dateSched.AddDays(1);
			}
			//no schedules to show, just return the table with weeks and days in date range.
			if(listProvNums.Count==0 && listEmployeeNums.Count==0 && !includeCNotes && !includePNotes) {
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
				string filter="AND (SchedType="+POut.Int((int)ScheduleType.Practice)+" AND schedule.ClinicNum";
				if(clinicNum==0) {
					filter+=">0";
				}
				else {
					filter+="="+POut.Long(clinicNum);
				}
				filter+=")";
				listFilters.Add(filter);
			}
			if(listProvNums.Count>0) {
				listFilters.Add("AND schedule.ProvNum IN("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+")");
			}
			if(listEmployeeNums.Count>0) {
				listFilters.Add("AND schedule.EmployeeNum IN("+string.Join(",",listEmployeeNums.Select(x => POut.Long(x)))+")");
			}
			#endregion
			string command="";
			//Make a standard and dynamic schedule query that is UNION'd together for each filter in the list.
			for(int i=0;i<listFilters.Count;i++) {
				//Purposefully use a UNION instead of UNION ALL because we want to remove all duplicate rows.
				if(!string.IsNullOrEmpty(command)) {
					command+=" UNION ";
				}
				command+=commandScheduleCore+listFilters[i]+" UNION "+commandDynamicScheduleCore+listFilters[i];
			}
			//If the for loop below changes to compare values in a row and the previous row, this query must be ordered by the additional comparison column
			command+=" ORDER BY SchedDate,FName,ItemOrder,StartTime,ClinicNum,Status";
			DataTable tableRaw=Db.GetTable(command);
			DateTime dateTimeStart;
			DateTime dateTimeStop;
			int rowI;
			for(int i=0;i<tableRaw.Rows.Count;i++){
				dateSched=PIn.Date(tableRaw.Rows[i]["SchedDate"].ToString());
				dateTimeStart=PIn.DateT(tableRaw.Rows[i]["StartTime"].ToString());
				dateTimeStop=PIn.DateT(tableRaw.Rows[i]["StopTime"].ToString());
				rowI=GetRowCal(dateStart,dateSched);
				if(i!=0//not first row
					&& tableRaw.Rows[i-1]["Abbr"].ToString()==tableRaw.Rows[i]["Abbr"].ToString()//same provider as previous row
					&& tableRaw.Rows[i-1]["FName"].ToString()==tableRaw.Rows[i]["FName"].ToString()//same employee as previous row
					&& tableRaw.Rows[i-1]["SchedDate"].ToString()==tableRaw.Rows[i]["SchedDate"].ToString())//and same date as previous row
				{
					#region Not First Row and Same Prov/Emp/Date as Previous Row
					if(dateTimeStart.TimeOfDay==PIn.DateT("12 AM").TimeOfDay && dateTimeStop.TimeOfDay==PIn.DateT("12 AM").TimeOfDay) {
						#region Note or Holiday
						if((PrefC.HasClinicsEnabled && tableRaw.Rows[i-1]["ClinicNum"].ToString()!=tableRaw.Rows[i]["ClinicNum"].ToString())//different clinic than previous line
							|| tableRaw.Rows[i-1]["Status"].ToString()!=tableRaw.Rows[i]["Status"].ToString())//start notes and holidays on different lines
						{
							table.Rows[rowI][(int)dateSched.DayOfWeek]+="\r\n";
							if(tableRaw.Rows[i]["Status"].ToString()=="2") {//if holiday
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=Lans.g("Schedules","Holiday");
							}
							else {
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=Lans.g("Schedules","Note");
							}
							if(PrefC.HasClinicsEnabled && tableRaw.Rows[i]["SchedType"].ToString()=="0") {//a practice sched type, prov/emp notes do not have a clinic associated
								string clinicAbbr=Clinics.GetAbbr(PIn.Long(tableRaw.Rows[i]["ClinicNum"].ToString()));
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
						table.Rows[rowI][(int)dateSched.DayOfWeek]+=dateTimeStart.ToString("h:mm")+"-"+dateTimeStop.ToString("h:mm");
					}
					#endregion Not First Row and Same Prov/Emp/Date as Previous Row
				}
				else {
					#region First Row or Different Prov/Emp/Date as Previous Row
					table.Rows[rowI][(int)dateSched.DayOfWeek]+="\r\n";
					if(dateTimeStart.TimeOfDay==PIn.DateT("12 AM").TimeOfDay && dateTimeStop.TimeOfDay==PIn.DateT("12 AM").TimeOfDay) {
						#region Note or Holiday
						if(tableRaw.Rows[i]["Status"].ToString()=="2"){//if holiday
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=Lans.g("Schedules","Holiday");
						}
						else {//note
							if(tableRaw.Rows[i]["Abbr"].ToString()!=""){
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=tableRaw.Rows[i]["Abbr"].ToString()+" ";
							}
							if(tableRaw.Rows[i]["FName"].ToString()!="") {
								table.Rows[rowI][(int)dateSched.DayOfWeek]+=tableRaw.Rows[i]["FName"].ToString()+" ";
							}
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=Lans.g("Schedules","Note");
						}
						if(PrefC.HasClinicsEnabled && tableRaw.Rows[i]["SchedType"].ToString()=="0") {//a practice sched type, prov/emp notes do not have a clinic associated
							string clinicAbbr=Clinics.GetAbbr(PIn.Long(tableRaw.Rows[i]["ClinicNum"].ToString()));
							if(string.IsNullOrEmpty(clinicAbbr)) {
								clinicAbbr="Headquarters";
							}
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=" ("+clinicAbbr+")";
						}
						table.Rows[rowI][(int)dateSched.DayOfWeek]+=":";
						#endregion Note or Holiday
					}
					else {
						if(tableRaw.Rows[i]["Abbr"].ToString()!="") {
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=tableRaw.Rows[i]["Abbr"].ToString()+" ";
						}
						if(tableRaw.Rows[i]["FName"].ToString()!="") {
							table.Rows[rowI][(int)dateSched.DayOfWeek]+=tableRaw.Rows[i]["FName"].ToString()+" ";
						}
						table.Rows[rowI][(int)dateSched.DayOfWeek]+=dateTimeStart.ToString("h:mm")+"-"+dateTimeStop.ToString("h:mm");
					}
					#endregion First Row or Different Prov/Emp/Date as Previous Row
				}
				if(includeEmpNotes && tableRaw.Rows[i]["Note"].ToString()!="") {
					table.Rows[rowI][(int)dateSched.DayOfWeek]+=" "+tableRaw.Rows[i]["Note"].ToString();
				}
			}
			return table;
		}

		///<summary>Gets all schedules and blockouts that meet the Web Sched requirements.  Set isRecall to false to get New Pat Appt ops.
		///Setting clinicNum to 0 will only consider unassigned operatories.</summary>
		public static List<Schedule> GetSchedulesAndBlockoutsForWebSched(List<long> listProvNums,DateTime dateStart,DateTime dateEnd,bool isRecall
			,long clinicNum,Logger.IWriteLine log=null, List<Schedule> listSchedulesBlockouts=null,bool isNewPat=false) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),listProvNums,dateStart,dateEnd,isRecall,clinicNum,log,
					listSchedulesBlockouts,isNewPat);
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
				List<DefLink> listDefLinksRecall=DefLinks.GetDefLinksByType(DefLinkType.RecallType);
				//If the recall type is not associated with any Restricted-To blockout types, then remove every distinct restricted-to blockout type from
				//the list of blockouts that can be scheduled over
				if(listSchedulesBlockouts.IsNullOrEmpty()) {
					listBlockoutTypesToIgnore.RemoveAll(x => listDefLinksRecall.Select(y => y.DefNum).Distinct().Contains(x));
				}
				else { //If the recall type is associated with some Restricted-To blockout types, then remove all blockouts from the list that do not match those blockouts
					listBlockoutTypesToIgnore=listDefLinksRecall.Select(x => x.DefNum).Distinct().ToList();
				}
			}
			else {
				if(isNewPat) {
					listBlockoutTypesToIgnore=PrefC.GetWebSchedNewPatAllowedBlockouts;
				}
				else {
					listBlockoutTypesToIgnore=PrefC.GetWebSchedExistingPatAllowedBlockouts;
				}
				//Get all of the operatory nums for operatories for either WSNP or WSEP
				listOperatories=Operatories.GetOpsForWebSchedNewOrExistingPatAppts(isNewPat:isNewPat);
				if(listOperatories==null || listOperatories.Count < 1) {
					return new List<Schedule>(); //No operatories setup for this WS type.
				}
				List<DefLink> listDefLinksBlockout=DefLinks.GetDefLinksByType(DefLinkType.BlockoutType);
				//If the appointment type is not associated with any Restricted-To blockout types, then remove every distinct restricted-to blockout type from
				//the list of blockouts that can be scheduled over
				if(listSchedulesBlockouts.IsNullOrEmpty()) {
					listBlockoutTypesToIgnore.RemoveAll(x => listDefLinksBlockout.Select(y => y.FKey).Distinct().Contains(x));
				}
				else { //If the appointment type is associated with some Restricted-To blockout types, then remove all blockouts from the list that do not match those blockouts
					listBlockoutTypesToIgnore=listDefLinksBlockout.Select(x => x.FKey).Distinct().ToList();
				}
			}
			//Get all blockout types that are not ignored in order to tell GetSchedulesHelper() which blockouts we need to know about.
			listBlockoutTypeDefNums=Defs.GetDefsForCategory(DefCat.BlockoutTypes)
					.FindAll(x => !listBlockoutTypesToIgnore.Contains(x.DefNum))//listBlockoutTypesToIgnore contains a list of blockouts that can be scheduled on.
					.Select(x => x.DefNum).ToList();
			if(!listBlockoutTypeDefNums.Contains(0)) {
				listBlockoutTypeDefNums.Add(0);//Non-blockouts must always be considered.
			}
			listOperatoryNums.AddRange(listOperatories.Select(x => x.OperatoryNum));
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				listClinicNums.Add(clinicNum);
			}
			List<int> listSchedTypes=new List<int>();
			listSchedTypes.Add((int)ScheduleType.Provider);
			listSchedTypes.Add((int)ScheduleType.Blockout);
			return GetSchedulesHelper(dateStart,dateEnd,listClinicNums,listOperatoryNums,listProvNumsWithZero,listBlockoutTypeDefNums,listSchedTypes,log);
		}

		///<summary>Gets a list of schedules for different methods.  Explicitly specify blockout types that need to be considered. Must be public for unit test.</summary>
		public static List<Schedule> GetSchedulesHelper(DateTime dateStart,DateTime dateEnd,List<long> listClinicNums,List<long> listOpNums
			,List<long> listProvNums,List<long> listDefNumsBlockout,List<int> listSchedTypes,Logger.IWriteLine log=null, bool isForMakeRecall=false)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listClinicNums,listOpNums,listProvNums,
					listDefNumsBlockout,listSchedTypes,log,isForMakeRecall);
			}
			//It is very important not to format these filters using DbHelper.DtimeToDate(). This would remove the index but yield the exact same results. 
			//It is already a Date column (no time) so no need to truncate the filter.
			if(listOpNums==null || listOpNums.Count < 1) {
				return new List<Schedule>();
			}
			if(listClinicNums.IsNullOrEmpty()) {
				listClinicNums.Add(0); //For customers without clinics. Necessary for filtering listOpNums. 
			}
			if(listDefNumsBlockout==null) {
				listDefNumsBlockout=new List<long>();
				listDefNumsBlockout.Add(0);
			}
			List<long> listOpNumsFiltered=listOpNums;
			if(PrefC.HasClinicsEnabled) {
				listOpNumsFiltered=Operatories.GetOpNumsForClinics(listClinicNums).Where(x => listOpNums.Contains(x)).ToList();
				if(listOpNumsFiltered.Count==0) {
					throw new Exception("No operatories for these clinics.");
				}
			}
			List<long> listProvNumsFiltered=ListTools.DeepCopy<long,long>(listProvNums); //Initial state
			List<long> listProvNumsDent;
			List<long> listProvNumsHyg;
			List<long> listProvNumsFromAllowedClinics=new List<long>();
			//create list of providers the current user is permitted to access.
			listProvNumsFromAllowedClinics=Providers.GetProvsForClinicList(listClinicNums).Select(x => x.ProvNum).Distinct().ToList();
			listProvNumsFiltered.RemoveAll(x => !listProvNumsFromAllowedClinics.Contains(x)); //filter passed list of providers to remove those the user cannot access
			if(listProvNums.Contains(0)) {
				listProvNumsFiltered.Add(0);  //will correctly display no results if filtered listProvidersNums is empty
			}
			listProvNumsDent=Operatories.GetOperatories(listOpNumsFiltered)
				.Where(x => listProvNumsFiltered.Contains(x.ProvDentist))
				.Select(x => x.ProvDentist)
				.Distinct().ToList();
			if(listProvNumsDent.IsNullOrEmpty() && listProvNums.Contains(0)) {
				listProvNumsDent.Add(0);
			}
			listProvNumsHyg=Operatories.GetOperatories(listOpNumsFiltered)
				.Where(x => listProvNumsFiltered.Contains(x.ProvHygienist))
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
						AND operatory.OperatoryNum IN ({string.Join(",",listOpNumsFiltered)}) 
					WHERE ";
				if(!listProvNumsFiltered.IsNullOrEmpty()) {
					command+=$" schedule.ProvNum IN({ string.Join(",",listProvNumsFiltered)}) AND";
				}
				command+=$@" schedule.BlockoutType IN ({string.Join(",",listDefNumsBlockout.Select(x => POut.Long(x)))})
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
					WHERE operatory.OperatoryNum IN ({string.Join(",",listOpNumsFiltered)}) ";
				if(!listProvNumsFiltered.IsNullOrEmpty()) {
					command+=$" AND schedule.ProvNum IN({ string.Join(",",listProvNumsFiltered)})";
				}
				command+=$@" AND schedule.BlockoutType IN ({string.Join(",",listDefNumsBlockout.Select(x => POut.Long(x)))})
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
			List<string> listColumnNames=new List<string>();
			listColumnNames.Add("ProvDentist");
			listColumnNames.Add("ProvHygienist");
			for(int i=0;i<listColumnNames.Count;i++) {
				//-- Using UNION instead of UNION ALL because we want duplicate entries to be removed.
				//-- Next, get all schedules that are not associated to any operatories
				//-- Blockouts should be ignored because they HAVE to be assigned to an operatory.
				//-- Only consider schedules that are NOT assigned to any operatories (dynamic schedules)
				command+=$@"
					UNION 
					(SELECT schedule.* FROM schedule
						INNER JOIN provider ON schedule.ProvNum=provider.ProvNum
						INNER JOIN operatory ON schedule.ProvNum=operatory.{listColumnNames[i]}
						LEFT JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
						WHERE provider.IsHidden!=1 ";
				if(listColumnNames[i]=="ProvDentist" && !listProvNumsDent.IsNullOrEmpty()) { //includes relevant provider parameter if possible
					command+=$"AND provider.ProvNum IN ({String.Join(",",listProvNumsDent)}) ";
				}
				else if(listColumnNames[i]=="ProvHygienist" && !listProvNumsHyg.IsNullOrEmpty()) {
					command+=$"AND provider.ProvNum IN ({String.Join(",",listProvNumsHyg)}) ";
				}
				command+=$@"
						AND operatory.OperatoryNum IN ({string.Join(",",listOpNumsFiltered)})
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
		public static int GetRowCal(DateTime dateStart,DateTime dateEnd){
			//No need to check MiddleTierRole; no call to db.
			TimeSpan timeSpan=dateEnd-dateStart;
			int dayInterval=timeSpan.Days;
			int daysFirstWeek=7-(int)dateStart.DayOfWeek;//eg Monday=7-1=6.  or Sat=7-6=1.
			dayInterval=dayInterval-daysFirstWeek;
			if(dayInterval<0){
				return 0;
			}
			return (int)Math.Ceiling((dayInterval+1)/7d);
		}

		///<summary>When click on a calendar grid, this is used to calculate the date clicked on.  StartDate is the first date in the Calendar, which does
		///not have to be Sun.</summary>
		public static DateTime GetDateCal(DateTime dateStart,int row,int col){
			//No need to check MiddleTierRole; no call to db.
			DateTime dateFirstRow;//the first date of row 0. Typically a few days before startDate. Always a Sun.
			dateFirstRow=dateStart.AddDays(-(int)dateStart.DayOfWeek);//example: (Tues,May 9).AddDays(-2)=Sun,May 7.
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
		public static void SetForDay(List<Schedule> listSchedules,List<Schedule> listSchedulesOld) {
			if(listSchedules.Any(x=>x.StartTime>x.StopTime)) {
				throw new Exception(Lans.g("Schedule","Stop time must be later than start time."));
			}
			Sync(listSchedules,listSchedulesOld);
		}

		///<summary>Inserts, updates, or deletes the passed in listNew against the stale listOld.  Returns true if db changes were made.
		///This does not call the normal crud.Sync due to the special cases of DeletedObject and ScheduleOps.
		///This sends less data across middle teir for update logic, which is why remoting role occurs after we have filtered both lists.
		///Inserts an invalid schedule signal for the date of the first item in listNew (this is only called by SetForDay).</summary>
		public static bool Sync(List<Schedule> listSchedulesNew,List<Schedule> listSchedulesOld) {
			//No call to DB yet, remoting role to be checked later.
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<Schedule> listSchedulesIns = new List<Schedule>();
			List<Schedule> listSchedulesUpdNew = new List<Schedule>();
			List<Schedule> listSchedulesUpdDB = new List<Schedule>();
			List<Schedule> listSchedulesDel = new List<Schedule>();
			listSchedulesNew.Sort((Schedule x,Schedule y) => { return x.ScheduleNum.CompareTo(y.ScheduleNum); });//Anonymous function, sorts by compairing PK. 
			listSchedulesOld.Sort((Schedule x,Schedule y) => { return x.ScheduleNum.CompareTo(y.ScheduleNum); });//Anonymous function, sorts by compairing PK. 
			int idxNew = 0;
			int idxDB = 0;
			Schedule scheduleNew;
			Schedule scheduleDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(true) {
				if(idxNew>=listSchedulesNew.Count && idxDB>=listSchedulesOld.Count) {
					break;
				}
				scheduleNew=null;
				if(idxNew<listSchedulesNew.Count) {
					scheduleNew=listSchedulesNew[idxNew];
				}
				scheduleDB=null;
				if(idxDB<listSchedulesOld.Count) {
					scheduleDB=listSchedulesOld[idxDB];
				}
				//begin compare
				if(scheduleNew!=null && scheduleDB==null) {//listNew has more items, listDB does not.
					listSchedulesIns.Add(scheduleNew);
					idxNew++;
					continue;
				}
				else if(scheduleNew==null && scheduleDB!=null) {//listDB has more items, listNew does not.
					listSchedulesDel.Add(scheduleDB);
					idxDB++;
					continue;
				}
				else if(scheduleNew.ScheduleNum<scheduleDB.ScheduleNum) {//newPK less than dbPK, newItem is 'next'
					listSchedulesIns.Add(scheduleNew);
					idxNew++;
					continue;
				}
				else if(scheduleNew.ScheduleNum>scheduleDB.ScheduleNum) {//dbPK less than newPK, dbItem is 'next'
					listSchedulesDel.Add(scheduleDB);
					idxDB++;
					continue;
				}
				//This filters out schedules that do not need to be updated, instead of relying on the update new/old pattern to filter.
				//Everything past this point needs to increment idxNew and idxDB.
				else if(Crud.ScheduleCrud.UpdateComparison(scheduleNew,scheduleDB) || !scheduleNew.Ops.OrderBy(x=>x).SequenceEqual(scheduleDB.Ops.OrderBy(x=>x)))  //if the two lists are not identical
				{
					//Both lists contain the 'next' item, update required
					listSchedulesUpdNew.Add(scheduleNew);
					listSchedulesUpdDB.Add(scheduleDB);
				}
				idxNew++;
				idxDB++;
				//There is nothing to do with this schedule?
			}
			if(listSchedulesIns.Count==0 && listSchedulesUpdNew.Count==0 && listSchedulesUpdDB.Count==0 && listSchedulesDel.Count==0) {
				return false;//No need to go through remoting role check and following code because it will do nothing
			}
			//This sync logic was split up from the typical sync logic in order to restrict payload sizes that are sent over middle tier.
			//If this method starts having issues in the future we will need to serialize the lists into DataTables to further save size.
			bool isSuccess=SyncToDbHelper(listSchedulesIns,listSchedulesUpdNew,listSchedulesUpdDB,listSchedulesDel);
			if(isSuccess) {
				//We supress signal insertion in SyncToDbHelper since we know that this method is only called by SetForDay, we can use the date from the first
				//sched in either the new or old list (since either, but not both, can be empty at this point) and insert a generalized signal for that date.
				Signalods.SetInvalidSched(listSchedulesNew.Concat(listSchedulesOld).First().SchedDate);
			}
			return isSuccess;
		}

		///<summary>Inserts, updates, or deletes database rows sepcified in the supplied lists.  Returns true if db changes were made.
		///This was split from the list building logic to limit the payload that needed to be sent over middle tier.</summary>
		public static bool SyncToDbHelper(List<Schedule> listSchedulesIns,List<Schedule> listSchedulesUpdNew,List<Schedule> listSchedulesUpdDB
			,List<Schedule> listSchedulesDel) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listSchedulesIns,listSchedulesUpdNew,listSchedulesUpdDB,listSchedulesDel);
			}
			//Commit changes to DB 
			//to foreach loops
			for(int i = 0;i<listSchedulesIns.Count;i++) {
				Insert(listSchedulesIns[i],false,false);
			}
			for(int i = 0;i<listSchedulesUpdNew.Count;i++) {
				Update(listSchedulesUpdNew[i],listSchedulesUpdDB[i],false,false);
			}
			for(int i = 0;i<listSchedulesDel.Count;i++) {
				Delete(listSchedulesDel[i],false);
			}
			if(listSchedulesIns.Count>0 || listSchedulesUpdNew.Count>0 || listSchedulesDel.Count>0) {
				//Unlike base Sync pattern, we already know that anything in the listUpdNew should be updated.
				//filtering for update should have already been performed, otherwise the return value may be a false positive.
				return true;
			}
			return false;
		}

		///<summary>Clears all schedule entries for the given date range and the given providers, employees, and practice. 
		///Insert an invalid schedule signalod.</summary>
		public static void Clear(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listEmployeeNums,bool includePNotes,bool includeCNotes,bool excludeHolidays,long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listEmployeeNums,includePNotes,includeCNotes,excludeHolidays,clinicNum);
				return;
			}
			DeleteMany(GetSchedulesToDelete(dateStart,dateEnd,listProvNums,listEmployeeNums,includePNotes,includeCNotes,clinicNum,excludeHolidays).Select(x => x.ScheduleNum).ToList());
		}

		///<summary>Returns all Schedules that match the passed in arguments.</summary>
		public static List<Schedule> GetSchedulesToDelete(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listEmployeeNums,
			bool includePNotes,bool includeCNotes,long clinicNum,bool excludeHolidays=false) 
		{
			if(listProvNums.Count==0 && listEmployeeNums.Count==0 && !includeCNotes && !includePNotes) {
				return new List<Schedule>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listEmployeeNums,includePNotes,includeCNotes,clinicNum,excludeHolidays);
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
			if(listProvNums.Count>0) {
				listOrClauses.Add("schedule.ProvNum IN("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+")");
			}
			if(listEmployeeNums.Count>0) {
				listOrClauses.Add("schedule.EmployeeNum IN("+string.Join(",",listEmployeeNums.Select(x => POut.Long(x)))+")");
			}
			string command="SELECT * FROM schedule "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND ("+string.Join(" OR ",listOrClauses)+") ";
			if(excludeHolidays) {
				command+="AND schedule.Status!="+POut.Int((int)SchedStatus.Holiday);
			}
			return Crud.ScheduleCrud.SelectMany(command);
		}

		///<summary>Clears all Blockout schedule entries for the given date ranges and the given ops.</summary>
		public static void ClearBlockouts(DateTime dateStart,DateTime dateEnd,List<long> listOpNums,bool includeWeekend) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listOpNums,includeWeekend);
				return;
			}
			string command=$@"SELECT schedule.ScheduleNum,scheduleop.ScheduleOpNum,schedule.SchedDate,scheduleop.OperatoryNum
				FROM schedule 
				INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
				WHERE schedule.SchedType={POut.Int((int)ScheduleType.Blockout)}
				AND schedule.SchedDate BETWEEN {POut.Date(dateStart)} AND {POut.Date(dateEnd)}
";
			if(!includeWeekend) {
				command+="AND DAYOFWEEK(schedule.SchedDate) BETWEEN 2 AND 6 \r\n";//1 is Sunday and 7 is Saturday in MySQL
			}
			command+=$"AND scheduleop.OperatoryNum IN({string.Join(",",listOpNums.Select(x => POut.Long(x)))})";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return;
			}
			command=$@"DELETE FROM scheduleop
				WHERE ScheduleOpNum IN ({string.Join(",",table.Select().Select(x => x["ScheduleOpNum"].ToString()))})";
			Db.NonQ(command);
			//If deleting the sched op above caused the schedule to be orphaned, it should be deleted.
			DeleteOrphanedBlockouts(table.Select().Select(x => PIn.Long(x["ScheduleNum"].ToString())).Distinct().ToList());
			List<Schedule> listSchedulesSetInvalid=new List<Schedule>();
			for(int i=0;i<table.Rows.Count;i++) {
				Schedule schedule=new Schedule();
				schedule.SchedDate=PIn.DateT(table.Rows[i]["SchedDate"].ToString());
				List<long> listOpNumsSchedule=new List<long>();
				listOpNumsSchedule.Add(PIn.Long(table.Rows[i]["OperatoryNum"].ToString()));
				schedule.Ops=listOpNumsSchedule;
				listSchedulesSetInvalid.Add(schedule);
			}
			Signalods.SetInvalidSchedForOps(listSchedulesSetInvalid);
		}

		public static int GetDuplicateBlockoutCount() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="SELECT * FROM schedule WHERE SchedType="+POut.Int((int)ScheduleType.Blockout);
			List<long> listScheduleNumsDup=RefreshAndFill(command)
				.GroupBy(x => new { x.SchedDate,x.StartTime,x.StopTime,ops=string.Join(",",x.Ops.OrderBy(y=>y)) })//group by duplicates
				.Select(x => x.Skip(1)) //each group represents a set of duplicates. First time in group is the "non-duplicate"
				.SelectMany(x=>x.Select(y=>y.ScheduleNum)).ToList(); //get those with dupes
			//We need to query the database to get the list of schedules being deleted so we can run our logic to determine which signal refreshes need to be sent.
			//We use RefreshAndFill() because we need both the Schedule and ScheduleOp information to perform our signal logic.
			List <Schedule> listSchedulesDelete=RefreshAndFill("SELECT * FROM schedule WHERE ScheduleNum IN ("+string.Join(",",listScheduleNumsDup)+")");
			command="DELETE FROM schedule WHERE ScheduleNum IN("+string.Join(",",listScheduleNumsDup)+")";
			Db.NonQ(command);
			command="DELETE FROM scheduleop WHERE ScheduleNum IN("+string.Join(",",listScheduleNumsDup)+")";
			Db.NonQ(command);
			Signalods.SetInvalidSched(listSchedulesDelete.ToArray());
		}

		///<summary>Set clinicNum to 0 to return 'unassigned' clinics.  Otherwise, filters the data set on the clinic num passed in.
		///Added to the DataSet in Appointments.RefreshPeriod.</summary>
		public static DataTable GetPeriodEmployeeSchedTable(DateTime dateStart,DateTime dateEnd,long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
				List<Employee> listEmployees=Employees.GetEmpsForClinic(clinicNum);
				if(listEmployees.Count==0) {
					return table;
				}
				command+="AND employee.EmployeeNum IN ("+string.Join(",",listEmployees.Select(x => x.EmployeeNum))+") ";
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY schedule.ScheduleNum ";
			}
			else {
				command+="GROUP BY employee.EmployeeNum,StartTime,StopTime,FName,Note,schedule.ScheduleNum,LName ";
			}
			//Sort by Emp num so that sort is deterministic
			command+="ORDER BY FName,LName,employee.EmployeeNum,StartTime";//order by FName for display, LName and EmployeeNum for emps with same FName
			DataTable tableRaw=Db.GetTable(command);
			DataRow row;
			DateTime dateTimeStart;
			DateTime dateTimeStop;
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				row=table.NewRow();
				row["EmployeeNum"]=tableRaw.Rows[i]["EmployeeNum"].ToString();
				if(table.Rows.Count==0 || tableRaw.Rows[i]["EmployeeNum"].ToString()!=table.Rows[table.Rows.Count-1]["EmployeeNum"].ToString()) {
					row["empName"]=tableRaw.Rows[i]["FName"].ToString();
				}
				dateTimeStart=PIn.DateT(tableRaw.Rows[i]["StartTime"].ToString());
				dateTimeStop=PIn.DateT(tableRaw.Rows[i]["StopTime"].ToString());
				row["schedule"]=dateTimeStart.ToString("h:mm")+"-"+dateTimeStop.ToString("h:mm");
				row["Note"]=tableRaw.Rows[i]["Note"].ToString();
				table.Rows.Add(row);
			}
			table.Columns.Remove("EmployeeNum");//Not necessary to drop this column, but it was not part of the original table when this code was refactored
			return table;
		}
	
		///<summary>Set clinicNum to 0 to return 'unassigned' clinics.  Otherwise, filters the data set on the clinic num passed in.
		///Added to the DataSet in Appointments.RefreshPeriod.</summary>
		public static DataTable GetPeriodProviderSchedTable(DateTime dateStart,DateTime dateEnd,long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			List<Schedule> listSchedules=ListSchedulesForDate.FindAll(x => listProvNums.Contains(x.ProvNum) && (!isODHQ || (isODHQ && x.StartTime!=x.StopTime)));
			listSchedules=listSchedules.OrderBy(x => listProvNums.IndexOf(x.ProvNum)).ToList();//Make list alphabetical.
			Schedule schedule;
			DataRow row;
			DateTime dateTimeStart;
			DateTime dateTimeStop;
			for(int i=0; i<listSchedules.Count; i++){
				schedule=listSchedules[i];
				row=table.NewRow();
				row["ProvAbbr"]=Providers.GetAbbr(schedule.ProvNum);
				dateTimeStart=PIn.DateT(schedule.StartTime.ToString());
				dateTimeStop=PIn.DateT(schedule.StopTime.ToString());
				row["schedule"]=dateTimeStart.ToString("h:mm")+"-"+dateTimeStop.ToString("h:mm");
				row["Note"]=schedule.Note;
				table.Rows.Add(row);
			}
			return table;
		}

		/// <summary></summary>
		/// <param name="doRunQueryOnNoOps">Set to false if an empty DataTable should be returned when listOpNums is null or empty.</param>
		/// <returns></returns>
		public static DataTable GetPeriodSchedule(DateTime dateStart,DateTime dateEnd,List<long> listOpNums=null,bool doRunQueryOnNoOps=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			//Set group_concat_max_len for this session so that the comma-separated 'Ops' string may not be truncated.
			//The default length of 1024 could easily get hit with random primary keys.
			//Group_concat_max_len is constrained by max_allowed_packet, so set group_concat_max_len to the global value of max_allowed_packet.
			//If that's not big enough, they will have problems in other places before here.
			string command;
			if(!_hasSet_group_concat_max_len){
				int maxAllowedPacket=MiscData.GetMaxAllowedPacket();
				command="SET SESSION group_concat_max_len = " + POut.Int(maxAllowedPacket);
				Db.NonQ(command);
				_hasSet_group_concat_max_len=true;
			}
			//Go get every schedule for the date range passed in.
			//Left join on the scheduleop table as to get the necessary information needed to fill the custom "ops" column (above).
			command="SELECT schedule.ScheduleNum,SchedDate,StartTime,StopTime,SchedType,ProvNum,BlockoutType,Note,"
				+"Status,EmployeeNum,DateTStamp,schedule.ClinicNum,"
				+"GROUP_CONCAT(DISTINCT scheduleop.OperatoryNum SEPARATOR ',') 'Ops' "
				+"FROM schedule "
				+"LEFT JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum "
				+"WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
			if(listOpNums!=null && listOpNums.Count > 0) {
				List<string> listStrOps=listOpNums.Select(x => POut.Long(x)).ToList();
				command+="AND (scheduleop.OperatoryNum IN ("+string.Join(",",listStrOps)+") OR scheduleop.OperatoryNum IS NULL) ";
			}
			command+="GROUP BY ScheduleNum ";
			command+="ORDER BY StartTime ";
			DataTable tableRaw=Db.GetTable(command);
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				DataRow dataRow=table.NewRow();
				dataRow["ScheduleNum"]=tableRaw.Rows[i]["ScheduleNum"].ToString();
				dataRow["SchedDate"]=tableRaw.Rows[i]["SchedDate"].ToString();
				dataRow["StartTime"]=tableRaw.Rows[i]["StartTime"].ToString();
				dataRow["StopTime"]=tableRaw.Rows[i]["StopTime"].ToString();
				dataRow["SchedType"]=tableRaw.Rows[i]["SchedType"].ToString();
				dataRow["ProvNum"]=tableRaw.Rows[i]["ProvNum"].ToString();
				dataRow["BlockoutType"]=tableRaw.Rows[i]["BlockoutType"].ToString();
				dataRow["Note"]=tableRaw.Rows[i]["Note"].ToString();
				dataRow["Status"]=tableRaw.Rows[i]["Status"].ToString();
				dataRow["ops"]=tableRaw.Rows[i]["Ops"].ToString();
				dataRow["EmployeeNum"]=tableRaw.Rows[i]["EmployeeNum"].ToString();
				dataRow["DateTStamp"]=tableRaw.Rows[i]["DateTStamp"].ToString();
				dataRow["ClinicNum"]=tableRaw.Rows[i]["ClinicNum"].ToString();
				table.Rows.Add(dataRow);
			}
			return table;
		}

		///<summary>Gets schedule info that's filtered to match the criteria of any passed in arguments.</summary>
		public static DataTable GetPeriodScheduleForApi(DateTime dateStart,DateTime dateEnd,long schedType,long blockoutDefNum,long provNum,long employeeNum,
			long scheduleNum,int limit,int offset,List<long> listOpNums=null,string dateFormatString="yyyy-MM-dd")
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,schedType,blockoutDefNum,provNum,employeeNum,scheduleNum,limit,offset,listOpNums,dateFormatString);
			}
			DataTable tableReturn=new DataTable("Schedule");
			tableReturn.Columns.Add("ScheduleNum");
			tableReturn.Columns.Add("SchedDate");
			tableReturn.Columns.Add("StartTime");
			tableReturn.Columns.Add("StopTime");
			tableReturn.Columns.Add("SchedType");
			tableReturn.Columns.Add("ProvNum");
			tableReturn.Columns.Add("BlockoutType");
			tableReturn.Columns.Add("blockoutType");
			tableReturn.Columns.Add("Note");
			tableReturn.Columns.Add("operatories");
			tableReturn.Columns.Add("EmployeeNum");
			//Go get every schedule for the date range passed in.
			//Left join on the scheduleop table as to get the necessary information needed to fill the custom "ops" column (above).
			string command="SELECT schedule.ScheduleNum,SchedDate,StartTime,StopTime,SchedType,ProvNum,BlockoutType,Note,"
				+"Status,EmployeeNum,schedule.ClinicNum,scheduleop.OperatoryNum "
				+"FROM schedule "
				+"LEFT JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum ";
			if(scheduleNum>0) {
				command+="WHERE schedule.ScheduleNum="+POut.Long(scheduleNum)+" "; //Getting one
			}
			else {
				command+="WHERE SchedDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "; //Getting many
			}
			if(schedType > -1) { //0 is included when looking for a change in schedType, since it's an Enum that starts at 0.
				command+="AND SchedType="+POut.Long(schedType)+" ";
			}
			if(blockoutDefNum > 0) {
				command+="AND BlockoutType="+POut.Long(blockoutDefNum)+" ";
			}
			if(provNum > 0) {
				command+="AND ProvNum="+POut.Long(provNum)+" ";
			}
			if(employeeNum > 0) {
				command+="AND EmployeeNum="+POut.Long(employeeNum)+" ";
			}
			if(listOpNums!=null && listOpNums.Count > 0) {
				command+="AND (scheduleop.OperatoryNum IN ("+string.Join(",",listOpNums.Select(x => POut.Long(x)))+") OR scheduleop.OperatoryNum IS NULL) ";
			}
			command+="ORDER BY StartTime";
			DataTable tableRaw=Db.GetTable(command);
			List<DataRow> listRowsRaw=tableRaw.Rows.OfType<DataRow>().ToList(); //Each row that exists in the raw table returned by the query. Some rows may share the same ScheduleNum.
			List<DataRow> listRowsDistinct=listRowsRaw.DistinctBy(x => x["ScheduleNum"]).ToList(); //In the table we return, we don't want duplicate ScheduleNum results.
			for(int i=offset;i<listRowsDistinct.Count;i++) {
				if(i>=offset+limit) { //Paging.
					break;
				}
				DataRow rowCur=listRowsDistinct[i];
				List<DataRow> listRowsSameScheduleNum=listRowsRaw.FindAll(x => x["ScheduleNum"].ToString()==rowCur["ScheduleNum"].ToString()); //Get each row with the current's ScheduleNum.
				List<string> listOps=new List<string>(); //List of OperatoryNum strings.
				for(int j=0;j<listRowsSameScheduleNum.Count;j++) { //If a schedule entry is in multiple operatories, scheduleop will have 1 row for each operatory a ScheduleNum is in.
					listOps.Add(listRowsSameScheduleNum[j]["OperatoryNum"].ToString()); //Grab the OperatoryNum from each row with the same ScheduleNum and add to listOps.
				}
				DataRow row=tableReturn.NewRow();
				row["ScheduleNum"]=rowCur["ScheduleNum"].ToString();
				row["SchedDate"]=PIn.Date(rowCur["SchedDate"].ToString()).ToString(dateFormatString);
				row["StartTime"]=rowCur["StartTime"].ToString();
				row["StopTime"]=rowCur["StopTime"].ToString();
				row["SchedType"]=Enum.GetName(typeof(ScheduleType),PIn.Long(rowCur["SchedType"].ToString()));
				row["ProvNum"]=rowCur["ProvNum"].ToString();
				row["BlockoutType"]=rowCur["BlockoutType"].ToString();
				row["blockoutType"]=Defs.GetName(DefCat.BlockoutTypes,PIn.Long(rowCur["BlockoutType"].ToString()));
				row["Note"]=rowCur["Note"].ToString();
				row["operatories"]=string.Join(",",listOps);
				row["EmployeeNum"]=rowCur["EmployeeNum"].ToString();
				tableReturn.Rows.Add(row);
			}
			return tableReturn;
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
			List<long> listOpNums,List<DefLink> listDefLinksBlockouts=null) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),defNumReason,dateStart,dateStop,listOpNums,listDefLinksBlockouts);
			}
			if(listOpNums==null || listOpNums.Count<1) {
				return new List<Schedule>();
			}
			if(listDefLinksBlockouts==null) {
				listDefLinksBlockouts=DefLinks.GetDefLinksByType(DefLinkType.BlockoutType,defNumReason);
			}
			if(listDefLinksBlockouts==null || listDefLinksBlockouts.Count<1) {
				return new List<Schedule>();
			}
			//See comments on schedule.ClinicNum for why it is not included in this query.  Schedules are typically linked to clinics via operatories via scheduleops.
			string command=$@"SELECT schedule.*
				FROM schedule
				INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
				WHERE schedule.SchedDate>={POut.Date(dateStart)} 
				AND schedule.SchedDate<={POut.Date(dateStop)}
				AND scheduleop.OperatoryNum IN ({string.Join(",",listOpNums)})
				AND schedule.BlockoutType IN ({ string.Join(",",listDefLinksBlockouts.Select(x => POut.Long(x.FKey)))}) 
				AND schedule.SchedType={POut.Int((int)ScheduleType.Blockout)}";
			return RefreshAndFill(command);
		}

		///<summary>Using the provided RecallTypeNum, gets associated blockouts and returns the schedules for those blockouts within a date range.</summary>
		public static List<Schedule> GetRestrictedToBlockoutsByRecallType(long recallTypeNum,DateTime dateStart,DateTime dateStop,
			List<long> listOpNums,List<DefLink> listDefLinksBlockouts=null) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Schedule>>(MethodBase.GetCurrentMethod(),recallTypeNum,dateStart,dateStop,listOpNums,listDefLinksBlockouts);
			}
			if(listOpNums==null || listOpNums.Count<1) {
				return new List<Schedule>();
			}
			if(listDefLinksBlockouts==null) {
				listDefLinksBlockouts=DefLinks.GetListByFKey(recallTypeNum,DefLinkType.RecallType);
			}
			if(listDefLinksBlockouts==null || listDefLinksBlockouts.Count<1) {
				return new List<Schedule>();
			}
			string command=$@"SELECT schedule.*
				FROM schedule
				INNER JOIN scheduleop ON schedule.ScheduleNum=scheduleop.ScheduleNum
				WHERE schedule.SchedDate>={POut.Date(dateStart)} 
				AND schedule.SchedDate<={POut.Date(dateStop)}
				AND scheduleop.OperatoryNum IN ({string.Join(",",listOpNums)})
				AND schedule.BlockoutType IN ({ string.Join(",",listDefLinksBlockouts.Select(x => POut.Long(x.DefNum)))}) 
				AND schedule.SchedType={POut.Int((int)ScheduleType.Blockout)}";
			return RefreshAndFill(command);
		}
	}
}