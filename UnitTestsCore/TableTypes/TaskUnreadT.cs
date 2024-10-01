using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestsCore {
	public class TaskUnreadT {
		///<summary>Gets all taskUnreads for a supplied TaskNum.</summary>
		public static List<TaskUnread> GetAllForTask(long taskNum) {
			if(taskNum==0) {
				return new List<TaskUnread>();
			}
			string command="SELECT * FROM taskunread WHERE TaskNum = "+POut.Long(taskNum);
			return OpenDentBusiness.Crud.TaskUnreadCrud.SelectMany(command);
		}
	}
}
