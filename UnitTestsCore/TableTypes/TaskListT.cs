using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class TaskListT {
		///<summary>Creates a TaskList.  If parent is not 0, then a parentDesc must also be provided.</summary>
		public static TaskList CreateTaskList(string descript="",long parent=0,bool isRepeating=false,TaskDateType dateType=TaskDateType.None,long fromNum=0,TaskObjectType objectType=TaskObjectType.None,string parentDesc="",GlobalTaskFilterType globalTaskFilterType=GlobalTaskFilterType.None)
		{
			TaskList taskList=new TaskList
			{
				Descript=descript,
				Parent=parent,
				DateTL=DateTime.MinValue,
				IsRepeating=isRepeating,
				DateType=dateType,
				FromNum=fromNum,
				ObjectType=objectType,
				DateTimeEntry=DateTime.Now,
				ParentDesc=parentDesc,
				NewTaskCount=0,
				GlobalTaskFilterType=globalTaskFilterType,
			};
			TaskLists.Insert(taskList);
			return taskList;
		}

		///<summary>Deletes everything from the TaskSubscription table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearTaskListTable() {
			string command="DELETE FROM tasklist";
			DataCore.NonQ(command);
		}
	}
}
