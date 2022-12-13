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

		///<summary>Creates and returns a list of 7 tasks with all fields utilized and varied. Useful for testing a task search for different fields.</summary>
		public static List<OpenDentBusiness.Task> CreateVariedTaskSet() {
			#region Create TaskLists
			TaskListT.ClearTaskListTable();
			TaskList tasklist1=TaskListT.CreateTaskList(descript:"TaskList 1",objectType:OpenDentBusiness.TaskObjectType.None);
			TaskList tasklist2=TaskListT.CreateTaskList(descript:"TaskList 2",objectType:OpenDentBusiness.TaskObjectType.Patient);
			TaskList tasklist3=TaskListT.CreateTaskList(descript:"TaskList 3",objectType:OpenDentBusiness.TaskObjectType.Appointment);
			#endregion
			List<OpenDentBusiness.Task> listTasks=new List<OpenDentBusiness.Task>();
			#region Creating Tasks and Adding to listTasks
			listTasks.Add(new OpenDentBusiness.Task {
				TaskListNum=tasklist1.TaskListNum, 
				KeyNum=0, 
				Descript="Task 1",
				TaskStatus=OpenDentBusiness.TaskStatusEnum.New, 
				IsRepeating=false,
				DateType=OpenDentBusiness.TaskDateType.None,
				FromNum=0,
				ObjectType=OpenDentBusiness.TaskObjectType.None, 
				DateTimeEntry=default,
				UserNum=0,
				IsUnread=false,
				ParentDesc="",
				PatientName="",
				PriorityDefNum=0,
				ReminderGroupId="",
				ReminderType=OpenDentBusiness.TaskReminderType.NoReminder,
				ReminderFrequency=0,
			});
			listTasks.Add(new OpenDentBusiness.Task {
				TaskListNum=tasklist1.TaskListNum, 
				KeyNum=0, 
				Descript="Task 2",
				TaskStatus=OpenDentBusiness.TaskStatusEnum.Viewed, 
				IsRepeating=false,
				DateType=OpenDentBusiness.TaskDateType.Week,
				FromNum=0,
				ObjectType=OpenDentBusiness.TaskObjectType.None, 
				DateTimeEntry=default,
				UserNum=0,
				IsUnread=false,
				ParentDesc="",
				PatientName="",
				PriorityDefNum=0,
				ReminderGroupId="",
				ReminderType=OpenDentBusiness.TaskReminderType.NoReminder,
				ReminderFrequency=0,
			});
			listTasks.Add(new OpenDentBusiness.Task {
				TaskListNum=tasklist1.TaskListNum,
				KeyNum=0, 
				Descript="Task 3",
				TaskStatus=OpenDentBusiness.TaskStatusEnum.Done, 
				IsRepeating=true,
				DateType=OpenDentBusiness.TaskDateType.Month,
				FromNum=0,
				ObjectType=OpenDentBusiness.TaskObjectType.None, 
				DateTimeEntry=default,
				UserNum=0,
				IsUnread=false,
				ParentDesc="",
				PatientName="",
				PriorityDefNum=0,
				ReminderGroupId="",
				ReminderType=OpenDentBusiness.TaskReminderType.NoReminder,
				ReminderFrequency=0,
			});
			OpenDentBusiness.Patient patient=PatientT.CreatePatient("Patient1");
			listTasks.Add(new OpenDentBusiness.Task {
				TaskListNum=tasklist2.TaskListNum, 
				KeyNum=patient.PatNum, 
				Descript="Task 4",
				TaskStatus=OpenDentBusiness.TaskStatusEnum.New, 
				IsRepeating=true,
				DateType=OpenDentBusiness.TaskDateType.None,
				FromNum=0,
				ObjectType=OpenDentBusiness.TaskObjectType.Patient, 
				DateTimeEntry=default,
				UserNum=0,
				IsUnread=true,
				ParentDesc="",
				PatientName="",
				PriorityDefNum=0,
				ReminderGroupId="",
				ReminderType=OpenDentBusiness.TaskReminderType.NoReminder,
				ReminderFrequency=0,
			});
			patient=PatientT.CreatePatient("Patient2");
			listTasks.Add(new OpenDentBusiness.Task {
				TaskListNum=tasklist2.TaskListNum, 
				KeyNum=patient.PatNum, 
				Descript="Task 5",
				TaskStatus=OpenDentBusiness.TaskStatusEnum.Done, 
				IsRepeating=true,
				DateType=OpenDentBusiness.TaskDateType.None,
				FromNum=0,
				ObjectType=OpenDentBusiness.TaskObjectType.Patient, 
				DateTimeEntry=default,
				UserNum=0,
				IsUnread=true,
				ParentDesc="",
				PatientName="",
				PriorityDefNum=0,
				ReminderGroupId="",
				ReminderType=OpenDentBusiness.TaskReminderType.NoReminder,
				ReminderFrequency=0,
			});
			patient=PatientT.CreatePatient("Patient3");
			OpenDentBusiness.Operatory operatory=OperatoryT.CreateOperatory();
			long provNum=ProviderT.CreateProvider("provider");
			OpenDentBusiness.Appointment appointment=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now.AddDays(-10),operatory.OperatoryNum,provNum);
			listTasks.Add(new OpenDentBusiness.Task {
				TaskListNum=tasklist3.TaskListNum, 
				KeyNum=appointment.AptNum, 
				Descript="Task 6",
				TaskStatus=OpenDentBusiness.TaskStatusEnum.New, 
				IsRepeating=true,
				DateType=OpenDentBusiness.TaskDateType.None,
				FromNum=0,
				ObjectType=OpenDentBusiness.TaskObjectType.Appointment, 
				DateTimeEntry=default,
				UserNum=0,
				IsUnread=true,
				ParentDesc="",
				PatientName="",
				PriorityDefNum=0,
				ReminderGroupId="",
				ReminderType=OpenDentBusiness.TaskReminderType.NoReminder,
				ReminderFrequency=0,
			});
			patient=PatientT.CreatePatient("Patient4");
			appointment=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now.AddDays(-15),operatory.OperatoryNum,provNum);
			listTasks.Add(new OpenDentBusiness.Task {
				TaskListNum=tasklist3.TaskListNum, 
				KeyNum=appointment.AptNum, 
				Descript="Task 7",
				TaskStatus=OpenDentBusiness.TaskStatusEnum.Viewed, 
				IsRepeating=true,
				DateType=OpenDentBusiness.TaskDateType.None,
				FromNum=0,
				ObjectType=OpenDentBusiness.TaskObjectType.Appointment, 
				DateTimeEntry=default,
				UserNum=0,
				IsUnread=true,
				ParentDesc="",
				PatientName="",
				PriorityDefNum=0,
				ReminderGroupId="",
				ReminderType=OpenDentBusiness.TaskReminderType.NoReminder,
				ReminderFrequency=0,
			});
			#endregion
			List<OpenDentBusiness.Task> listTasksReturned=new List<OpenDentBusiness.Task>();
			for(int i=0;i<listTasks.Count;i++) {
				OpenDentBusiness.Crud.TaskCrud.Insert(listTasks[i]);
				SetDateTimeOriginal(listTasks[i],DateTime.Now.AddDays(-i)); //Offset each tasks DateTimeOriginal by -1 day for one per day in the week.
				listTasksReturned.Add(OpenDentBusiness.Tasks.GetOne(listTasks[i].TaskNum));
			}
			return listTasksReturned;
		}

		///<summary>Updates the DateTimeOriginal. Unable to set the DateTimeOriginal to anything but DateTime.Now in the CRUD Layer.</summary>
		public static void SetDateTimeOriginal(Task Task, DateTime dateTime) {
			string command="UPDATE task SET DateTimeOriginal = "+POut.Date(dateTime)
				+" WHERE TaskNum = "+POut.Long(Task.TaskNum);
			DataCore.NonQ(command);
    }




	}
}
