using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class AppointmentRuleT {

		///<summary></summary>
		public static AppointmentRule CreateAppointmentRule(string desc,string codeStart,string codeEnd)
		{
			AppointmentRule apptRule=new AppointmentRule()
			{
				RuleDesc=desc,
				CodeStart=codeStart,
				CodeEnd=codeEnd,
				IsEnabled=true
			};
			AppointmentRules.Insert(apptRule);
			AppointmentRules.RefreshCache();
			return apptRule;
		}

		///<summary>Deletes everything from the appointment table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearAppointmentRuleTable() {
			string command="DELETE FROM appointmentrule WHERE AppointmentRuleNum > 0";
			DataCore.NonQ(command);
			AppointmentRules.RefreshCache();
		}
	}
}
