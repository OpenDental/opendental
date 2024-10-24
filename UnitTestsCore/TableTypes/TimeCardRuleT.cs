using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class TimeCardRuleT {

		public static void CreateTimeCardRule(long employeeNum=0,TimeSpan beforeTimeOfDay=default,TimeSpan afterTimeOfDay=default,TimeSpan overHoursPerDay=default,bool isOvertimeExempt=false,bool hasWeekendRate3=false) {
			TimeCardRule timeCardRule=new TimeCardRule();
			timeCardRule.EmployeeNum=employeeNum;
			timeCardRule.BeforeTimeOfDay=beforeTimeOfDay;
			timeCardRule.AfterTimeOfDay=afterTimeOfDay;
			timeCardRule.OverHoursPerDay=overHoursPerDay;
			timeCardRule.IsOvertimeExempt=isOvertimeExempt;
			timeCardRule.HasWeekendRate3=hasWeekendRate3;
			TimeCardRules.Insert(timeCardRule);
		}

		public static void CreateAMTimeRule(long employeeNum,TimeSpan beforeTimeOfDay) {
			CreateTimeCardRule(employeeNum:employeeNum,beforeTimeOfDay:beforeTimeOfDay);
		}

		public static void CreatePMTimeRule(long employeeNum,TimeSpan afterTimeOfDay) {
			CreateTimeCardRule(employeeNum:employeeNum,afterTimeOfDay:afterTimeOfDay);
		}

		public static void CreateHoursTimeRule(long employeeNum,TimeSpan overHoursPerDay) {
			CreateTimeCardRule(employeeNum:employeeNum,overHoursPerDay:overHoursPerDay);
		}

		///<summary>Deletes everything from the timecardrule table. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearTimeCardRuleTable() {
			string command="DELETE FROM timecardrule";
			DataCore.NonQ(command);
		}

	}
}
