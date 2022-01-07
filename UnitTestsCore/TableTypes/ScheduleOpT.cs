using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ScheduleOpT {

		///<summary></summary>
		public static ScheduleOp CreateScheduleOp(long operatoryNum,long scheduleNum) {
			ScheduleOp schedOp=new ScheduleOp();
			schedOp.OperatoryNum=operatoryNum;
			schedOp.ScheduleNum=scheduleNum;
			ScheduleOps.Insert(schedOp);
			return schedOp;
		}

		///<summary>Deletes everything from the scheduleop table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearScheduleOpTable() {
			string command="DELETE FROM scheduleop WHERE ScheduleOpNum > 0";
			DataCore.NonQ(command);
		}
	}
}
