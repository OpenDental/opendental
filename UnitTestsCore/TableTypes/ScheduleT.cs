using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ScheduleT {

		///<summary></summary>
		public static Schedule CreateSchedule(DateTime schedDate,TimeSpan startTime,TimeSpan stopTime,ScheduleType schedType=ScheduleType.Practice
			,SchedStatus status=SchedStatus.Open,long blockoutType=0,long clinicNum=0,long employeeNum=0,long provNum=0,List<long> listOpNums=null)
		{
			Schedule schedule=new Schedule();
			schedule.BlockoutType=blockoutType;
			schedule.ClinicNum=clinicNum;
			schedule.EmployeeNum=employeeNum;
			schedule.ProvNum=provNum;
			schedule.SchedDate=schedDate;
			schedule.SchedType=schedType;
			schedule.StartTime=startTime;
			schedule.Status=status;
			schedule.StopTime=stopTime;
			schedule.Ops=listOpNums??new List<long>();
			schedule.ScheduleNum=Schedules.Insert(schedule,false);
			return schedule;
		}

		///<summary>Creates a schedule entry for each ScheduleType. Each schedule is for today. provNum, blockoutDefNum and employeeNum are passed in to their respective schedules.
		///Schedules are returned as follows:
		///[SchedType=Practice 8-9, SchedType=Provider 9-10, SchedType=Blockout 10-11, SchedType=Employee 11-12, SchedType=WebSchedASAP 12-13]</summary>
		public static List<long> CreateSchedulesEachSchedType(long provNum=0,long blockoutDefNum=0,long employeeNum=0) {
			List<long> listScheduleNums=new List<long>();
			listScheduleNums.Add(CreateSchedule(DateTime.Today,TimeSpan.FromHours(8),TimeSpan.FromHours(9),ScheduleType.Practice).ScheduleNum);
			listScheduleNums.Add(CreateSchedule(DateTime.Today,TimeSpan.FromHours(9),TimeSpan.FromHours(10),ScheduleType.Provider,provNum:provNum).ScheduleNum);
			listScheduleNums.Add(CreateSchedule(DateTime.Today,TimeSpan.FromHours(10),TimeSpan.FromHours(11),ScheduleType.Blockout,blockoutType:blockoutDefNum).ScheduleNum);
			listScheduleNums.Add(CreateSchedule(DateTime.Today,TimeSpan.FromHours(11),TimeSpan.FromHours(12),ScheduleType.Employee,employeeNum:employeeNum).ScheduleNum);
			listScheduleNums.Add(CreateSchedule(DateTime.Today,TimeSpan.FromHours(12),TimeSpan.FromHours(13),ScheduleType.WebSchedASAP).ScheduleNum);
			return listScheduleNums;
		}

		///<summary>Creates a schedule entry for yesterday (7AM-8AM), one for today (9AM-10AM), and one for tomorrow (11AM-12PM), returned in that order. </summary>
		public static List<long> CreateSchedulesYesterdayThroughTomorrow() {
			List<long> listScheduleNums=new List<long>();
			listScheduleNums.Add(CreateSchedule(DateTime.Today.AddDays(-1),TimeSpan.FromHours(7),TimeSpan.FromHours(8),ScheduleType.Practice).ScheduleNum);
			listScheduleNums.Add(CreateSchedule(DateTime.Today,TimeSpan.FromHours(9),TimeSpan.FromHours(10),ScheduleType.Practice).ScheduleNum);
			listScheduleNums.Add(CreateSchedule(DateTime.Today.AddDays(1),TimeSpan.FromHours(11),TimeSpan.FromHours(12),ScheduleType.Practice).ScheduleNum);
			return listScheduleNums;
		}

		///<summary>Deletes everything from the schedule table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearScheduleTable() {
			string command="DELETE FROM schedule WHERE ScheduleNum > 0";
			DataCore.NonQ(command);
		}

		public static void DeleteSchedule(long scheduleNum) {
			string command="DELETE FROM schedule WHERE ScheduleNum="+scheduleNum;
			DataCore.NonQ(command);
		}
	}
}
