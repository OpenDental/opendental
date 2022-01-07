using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class TaskSubscriptionT {
		///<summary>Creates a TaskSubscription.</summary>
		public static TaskSubscription CreateTaskSubscription(long userNum=0,long taskListNum=0,long taskNum=0) 
		{
			TaskSubscription taskSub=new TaskSubscription
			{
				UserNum=userNum,
				TaskListNum=taskListNum,
				TaskNum=taskNum
			};
			TaskSubscriptions.Insert(taskSub);
			return taskSub;
		}

		///<summary>Deletes everything from the TaskSubscription table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearTaskSubscriptionTable() {
			string command="DELETE FROM tasksubscription";
			DataCore.NonQ(command);
		}
	}
}
