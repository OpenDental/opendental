using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class TimeAdjustT {

		public static TimeAdjust CreateTimeAdjust(long employeeNum,DateTime timeEntry,TimeSpan oTimeHours=default,TimeSpan ptoHours=default,TimeSpan regHours=default) {
			TimeAdjust timeAdjust=new TimeAdjust() {
				ClinicNum=0,
				EmployeeNum=employeeNum,
				IsAuto=false,
				IsUnpaidProtectedLeave=false,
				Note="",
				OTimeHours=oTimeHours,
				PtoDefNum=0,
				PtoHours=ptoHours,
				RegHours=regHours,
				TimeEntry=timeEntry,
			};
			TimeAdjusts.Insert(timeAdjust);
			return timeAdjust;
		}

	}
}
