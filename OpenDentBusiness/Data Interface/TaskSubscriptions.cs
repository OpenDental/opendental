using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TaskSubscriptions {
		#region Get Methods
		///<summary>Returns a list of TaskSubscriptions for the TaskLists userNum is directly subscribed to. Does not include any children/grandchildren 
		///of the TaskLists in TaskSubscription.</summary>
		public static List<TaskSubscription> GetTaskSubscriptionsForUser(long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskSubscription>>(MethodBase.GetCurrentMethod(),userNum);
			}
			string command="SELECT * FROM tasksubscription WHERE UserNum="+POut.Long(userNum);
			return Crud.TaskSubscriptionCrud.SelectMany(command);
		}
		#endregion
	
		///<summary></summary>
		public static long Insert(TaskSubscription taskSubscription){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				taskSubscription.TaskSubscriptionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),taskSubscription);
				return taskSubscription.TaskSubscriptionNum;
			}
			return Crud.TaskSubscriptionCrud.Insert(taskSubscription);
		}

		/*
		///<summary></summary>
		public static void Update(TaskSubscription subsc) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),subsc);
				return;
			}
			Crud.TaskSubscriptionCrud.Update(subsc);
		}*/

		///<summary>Attempts to create a subscription to a TaskList with TaskListNum of subscribeToTaskListNum.
		///The curUserNum must be the currently logged in user.</summary>
		public static bool TrySubscList(long taskListNum,long userNum) {
			Meth.NoCheckMiddleTierRole();
			//Get the list of directly subscribed TaskListNums.  This avoids the concurrency issue of the same user logged in via multiple WS and 
			//subscribing to the same TaskList.  Additionally, this allows the user to directly subscribe to child TaskLists of subscribed parent Tasklists
			//which was old behavior that was inadvertently removed.
			List<long> listTaskSubscriptionNumsExisting=GetTaskSubscriptionsForUser(userNum).Select(x => x.TaskListNum).ToList();
			if(listTaskSubscriptionNumsExisting.Contains(taskListNum)) {
				return false;//Already subscribed.
			}
			//Get all currently subscribed unread Reminder tasks before adding new subscription.
			List<long> listTaskNumsReminderOld=GetUnreadReminderTasks(userNum).Select(x => x.TaskNum).ToList();
			TaskSubscription taskSubscription=new TaskSubscription();
			taskSubscription.IsNew=true;
			taskSubscription.UserNum=userNum;
			taskSubscription.TaskListNum=taskListNum;
			Insert(taskSubscription);
			//Get newly subscribed unread Reminder tasks.
			List<Task> listTasksNewReminder=GetUnreadReminderTasks(userNum).FindAll(x => !listTaskNumsReminderOld.Contains(x.TaskNum));
			//Set any past unread Reminder tasks as read.
			TaskUnreads.SetRead(userNum,listTasksNewReminder.FindAll(x => x.DateTimeEntry<DateTime.Now).ToArray());
			//Get all future reminders in the newly subscribed Tasklist (and sub Tasklists) that the user was not previously subscribed to.
			List<Task> listTasksFutureReminders=GetNewReadReminders(listTaskSubscriptionNumsExisting,taskListNum,userNum)
				.Where(x => x.DateTimeEntry>=DateTime.Now).ToList();
			//We already know these tasks do not have any TaskUnreads (due to GetNewReadReminders->Tasks.RefreshChildren()), safe to insert TaskUnreads.
			TaskUnreads.InsertManyForTasks(listTasksFutureReminders,userNum);
			return true;
		}

		///<summary>Gets all Read Reminders in a TaskList/Task hierarchy that the user was not already subscribed to.</summary>
		private static List<Task> GetNewReadReminders(List<long> listTaskSubscriptionNumsExisting,long taskListNum,long userNum) {
			List<Task> listTasksReminders=new List<Task>();
			if(listTaskSubscriptionNumsExisting.Contains(taskListNum)) {
				//We are only looking for Reminders that we were not already subscribed to.
				return listTasksReminders;
			}
			long userNumInbox=TaskLists.GetMailboxUserNum(taskListNum);//Can be 0, not a user inbox.
			List<TaskList> listTaskLists=TaskLists.RefreshChildren(taskListNum,userNum,userNumInbox,TaskType.Reminder);
			for(int i=0;i<listTaskLists.Count;i++) {
				listTasksReminders.AddRange(GetNewReadReminders(listTaskSubscriptionNumsExisting,listTaskLists[i].TaskListNum,userNum));
			}
			listTasksReminders.AddRange(Tasks.RefreshChildren(taskListNum,false,DateTime.MinValue,userNum,userNumInbox,TaskType.Reminder,false
				).Where(x => !x.IsUnread));//IsUnread field set accurately by Tasks.RefreshChildren(...)
			return listTasksReminders;
		}

		///<summary>Gets all unread Reminder Tasks for curUserNum.  Mimics logic in FormOpenDental.SignalsTick.</summary>
		private static List<Task> GetUnreadReminderTasks(long userNum) {
			Meth.NoCheckMiddleTierRole();
			List<Task> listTasksReminders=new List<Task>();
			if(!PrefC.GetBool(PrefName.TasksUseRepeating)) {//Using Reminders (Reminders not allowed if using repeating tasks)
				List<Task> listTasksRefreshed=Tasks.GetNewTasksThisUser(userNum,Clinics.ClinicNum);//Get all tasks pertaining to current user.
				for(int i=0;i<listTasksRefreshed.Count;i++) {
					if(!String.IsNullOrEmpty(listTasksRefreshed[i].ReminderGroupId) && listTasksRefreshed[i].ReminderType!=TaskReminderType.NoReminder) {
						//Task is a Reminder.
						listTasksReminders.Add(listTasksRefreshed[i]);
					}
				}
			}
			return listTasksReminders;
		}

		/// <summary>Returns a list of userNums for users that are subscribed to the task list a passed in task is currently in./// </summary>
		public static List<long> GetSubscribersForTask(Task task){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),task);
			}
			string command=@"
				SELECT tasksubscription.UserNum
				FROM tasksubscription
				INNER JOIN tasklist ON tasksubscription.TaskListNum=tasklist.TaskListNum 
				INNER JOIN taskancestor ON taskancestor.TaskListNum=tasklist.TaskListNum AND taskancestor.TaskNum='"+POut.Long(task.TaskNum)+"'";
			return Db.GetListLong(command);
		}

		///<summary>Removes a subscription to a list.</summary>
		public static void UnsubscList(long taskListNum,long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskListNum,userNum);
				return;
			}
			//Get all future unread reminders
			List<Task> listTasksFutureUnreadReminders=Tasks.GetNewTasksThisUser(userNum,0)//Use clinicnum=0 to get all tasks, no task clinic filtering.
				.Where(x => Tasks.IsReminderTask(x) && x.DateTimeEntry>=DateTime.Now)
				.ToList();
			string command="DELETE FROM tasksubscription "
				+"WHERE UserNum="+POut.Long(userNum)
				+" AND TaskListNum="+POut.Long(taskListNum);
			Db.NonQ(command);
			List<Task> listTasksStillSubscribed=Tasks.GetNewTasksThisUser(userNum,0)//Use clinicnum=0 to get all tasks, no task clinic filtering.
				.Where(x => Tasks.IsReminderTask(x) && x.DateTimeEntry>=DateTime.Now).ToList();
			List<Task> listTasksUnsubForUser=listTasksFutureUnreadReminders.Where(x => !listTasksStillSubscribed.Select(y => y.TaskNum).Contains(x.TaskNum)).ToList();
			//Set unsubbed reminders in the future to be read so reminders wont show in NewForUser tasklist.
			TaskUnreads.SetRead(userNum,listTasksUnsubForUser.ToArray());
		}

		///<summary>Removes all the subscribers from a given tasklist</summary>
		public static void RemoveAllSubscribers(long taskListNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskListNum);
				return;
			}
			string command="DELETE FROM tasksubscription "
				+"WHERE TaskListNum="+POut.Long(taskListNum);
			Db.NonQ(command);
		}

		///<summary>Moves all subscriptions from taskListOld to taskListNew. Used when cutting and pasting a tasklist. Can also be used when deleting a tasklist to remove all subscriptions from the tasklist by sending in 0 as taskListNumNew.</summary>
		public static void UpdateTaskListSubs(long taskListNumOld,long taskListNumNew) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskListNumOld,taskListNumNew);
				return;
			}
			string command="";
			if(taskListNumNew==0) {
				command="DELETE FROM tasksubscription WHERE TaskListNum="+POut.Long(taskListNumOld);
			}
			else {
				command="UPDATE tasksubscription SET TaskListNum="+POut.Long(taskListNumNew)+" WHERE TaskListNum="+POut.Long(taskListNumOld);
			}
			Db.NonQ(command);
		}
		
		///<summary>Deletes rows for given PK tasksubscription.TaskSubscriptionNums.</summary>
		public static void DeleteMany(List<long> listTaskSubscriptionNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTaskSubscriptionNums);
				return;
			}
			if(listTaskSubscriptionNums.Count==0) {
				return;
			}
			string command="DELETE FROM tasksubscription WHERE TaskSubscriptionNum IN ("+String.Join(",",listTaskSubscriptionNums)+")";
			Db.NonQ(command);
		}

	}

	


	


}









