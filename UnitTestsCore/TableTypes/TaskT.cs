using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class TaskT {
		///<summary>Creates a TaskSubscription, dateTimeEntry will be DateTime.Now if not specified.</summary>
		public static Task CreateTask(long taskListNum=0,long keyNum=0,string descript="",
			TaskStatusEnum taskStatus=TaskStatusEnum.New,bool isRepeating=false,TaskDateType dateType=TaskDateType.None,long fromNum=0,
			TaskObjectType objectType=TaskObjectType.None,DateTime dateTimeEntry=new DateTime(),long userNum=0,bool isUnread=false,string parentDesc="",string patientName="",
			long priorityDefNum=0,string reminderGroupId="",TaskReminderType reminderType=TaskReminderType.NoReminder,int reminderFrequency=0)
		{
			if(dateTimeEntry==DateTime.MinValue) {
				dateTimeEntry=DateTime.Now;
			}
			Task task=new Task
			{
				TaskListNum=taskListNum,
				DateTask=DateTime.MinValue,
				KeyNum=keyNum,
				Descript=descript,
				TaskStatus=taskStatus,
				IsRepeating=isRepeating,
				DateType=dateType,
				FromNum=fromNum,
				ObjectType=objectType,
				DateTimeEntry=dateTimeEntry,
				UserNum=userNum,
				DateTimeFinished=DateTime.MinValue,
				IsUnread=isUnread,
				ParentDesc=parentDesc,
				PatientName=patientName,
				PriorityDefNum=priorityDefNum,
				ReminderGroupId=reminderGroupId,
				ReminderType=reminderType,
				ReminderFrequency=reminderFrequency,
				DateTimeOriginal=DateTime.Now
			};
			Tasks.Insert(task);
			task=Tasks.GetOne(task.TaskNum);//Make sure task matches Db. Avoids problems with DateTime columns.
			return task;
		}

		///<summary>Deletes everything from the TaskSubscription table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearTaskTable() {
			string command="DELETE FROM task";
			DataCore.NonQ(command);
		}
	}
}
