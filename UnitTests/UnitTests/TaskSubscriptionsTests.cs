using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.TaskSubscriptions_Tests {
	[TestClass]
	public class TaskSubscriptionsTests:TestBase {
		private long _userNum=1;
		private TaskList _taskListParent;
		private TaskList _taskListChild;
		private TaskList _taskListGrandchild;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			TaskListT.ClearTaskListTable();
			TaskT.ClearTaskTable();
			TaskSubscriptionT.ClearTaskSubscriptionTable();
			_taskListParent=TaskListT.CreateTaskList(descript:"TaskList1");
			_taskListChild=TaskListT.CreateTaskList(descript:"TaskList1Child",parent:_taskListParent.TaskListNum,parentDesc:_taskListParent.Descript);
			_taskListGrandchild=TaskListT.CreateTaskList(descript:"TaskList1Grandchild",parent:_taskListChild.TaskListNum,
				parentDesc:_taskListChild.Descript);
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		[TestMethod]
		public void TaskSubscriptions_GetSubscribedTaskLists_SubscribedToParent() {
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListParent.TaskListNum);
			List<TaskSubscription> listTaskSubs=TaskSubscriptions.GetTaskSubscriptionsForUser(_userNum);
			Assert.AreEqual(1,listTaskSubs.Count);
			Assert.IsTrue(listTaskSubs.Any(x => x.TaskListNum==_taskListParent.TaskListNum));//parent
		}

		[TestMethod]
		public void TaskSubscriptions_GetSubscribedTaskLists_SubscribedToParentAndChild() {
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListParent.TaskListNum);
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListChild.TaskListNum);
			List<TaskSubscription> listTaskSubs=TaskSubscriptions.GetTaskSubscriptionsForUser(_userNum);
			Assert.AreEqual(2,listTaskSubs.Count);
			Assert.IsTrue(listTaskSubs.Any(x => x.TaskListNum==_taskListParent.TaskListNum));//parent
			Assert.IsTrue(listTaskSubs.Any(x => x.TaskListNum==_taskListChild.TaskListNum));//child
		}

		[TestMethod]
		public void TaskSubscriptions_GetSubscribedTaskLists_SubscribedToGrandchildOnlyl() {
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListGrandchild.TaskListNum);
			List<TaskSubscription> listTaskSubs=TaskSubscriptions.GetTaskSubscriptionsForUser(_userNum);
			Assert.AreEqual(1,listTaskSubs.Count);
			Assert.IsFalse(listTaskSubs.Any(x => x.TaskListNum==_taskListParent.TaskListNum));//not parent
			Assert.IsFalse(listTaskSubs.Any(x => x.TaskListNum==_taskListChild.TaskListNum));//not child
			Assert.IsTrue(listTaskSubs.Any(x => x.TaskListNum==_taskListGrandchild.TaskListNum));//grandchild
		}

		[TestMethod]
		public void TaskSubscriptions_TrySubscList_MarkOldRemindersRead() {
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListChild.TaskListNum);//Subscribed to TaskListChild (and by extension, TaskListGrandchild).
			#region Create Unread Past Due Reminders
			OpenDentBusiness.Task taskParent=TaskT.CreateTask(_taskListParent.TaskListNum,descript:"ParentReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddSeconds(-1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskChild=TaskT.CreateTask(_taskListChild.TaskListNum,descript:"ChildReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddSeconds(-1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskGrandchild=TaskT.CreateTask(_taskListGrandchild.TaskListNum,descript:"GrandchildReminder",isUnread:true
				,dateTimeEntry:DateTime.Now.AddSeconds(-1),reminderGroupId:"1",reminderType:TaskReminderType.Once);
			TaskAncestors.SynchAll();
			TaskUnreads.SetUnread(_userNum,taskParent);
			TaskUnreads.SetUnread(_userNum,taskChild);
			TaskUnreads.SetUnread(_userNum,taskGrandchild);
			#endregion 
			bool isSuccess=TaskSubscriptions.TrySubscList(_taskListParent.TaskListNum,_userNum);
			OpenDentBusiness.Task taskParentDb=Tasks.GetOne(taskParent.TaskNum);
			OpenDentBusiness.Task taskChildDb=Tasks.GetOne(taskChild.TaskNum);
			OpenDentBusiness.Task taskGrandchildDb=Tasks.GetOne(taskGrandchild.TaskNum);
			Assert.IsTrue(isSuccess);
			Assert.IsFalse(TaskUnreads.IsUnread(_userNum,taskParentDb));//Only the task in taskListParent should be Read.
			Assert.IsTrue(TaskUnreads.IsUnread(_userNum,taskChildDb));//The task in taskListChild should still be Unread.
			Assert.IsTrue(TaskUnreads.IsUnread(_userNum,taskGrandchildDb));//The task in taskListGrandchild should still be Unread.
		}

		///<summary>Method to validate that subscribing to a TaskList will set future newly subscribed reminders to unread if they were previously not 
		///set to unread.</summary>
		[TestMethod]
		public void TaskSubscriptions_TrySubscList_SubToParentMarksDescendentsFutureRemindersAsUnread() {
			#region Create Read Future Due Reminders
			OpenDentBusiness.Task taskParent=TaskT.CreateTask(_taskListParent.TaskListNum,descript:"ParentReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskChild=TaskT.CreateTask(_taskListChild.TaskListNum,descript:"ChildReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskGrandchild=TaskT.CreateTask(_taskListGrandchild.TaskListNum,descript:"GrandchildReminder",isUnread:true
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderGroupId:"1",reminderType:TaskReminderType.Once);
			TaskAncestors.SynchAll();
			TaskUnreads.SetRead(_userNum,taskParent,taskChild,taskGrandchild);
			#endregion 
			bool isSuccess=TaskSubscriptions.TrySubscList(_taskListParent.TaskListNum,_userNum);
			Assert.IsTrue(isSuccess);
			foreach(OpenDentBusiness.Task task in new List<OpenDentBusiness.Task> { taskParent,taskChild,taskGrandchild }) {
				Assert.IsTrue(TaskUnreads.IsUnread(_userNum,task));
			}
		}

		///<summary>Method to validate that subscribing to a TaskList will set future newly subscribed reminders to unread if they were previously not 
		///set to unread.  The user will already have been subscribed to a child tasklist and have marked the future reminders as read; these reminders
		///should not change to unread.</summary>
		[TestMethod]
		public void TaskSubscriptions_TrySubscList_SubToParentMarksParentTaskUnread() {
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListChild.TaskListNum);//Subscribed to TaskListChild 
			#region Create Read Future Due Reminders
			OpenDentBusiness.Task taskParent=TaskT.CreateTask(_taskListParent.TaskListNum,descript:"ParentReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskChild=TaskT.CreateTask(_taskListChild.TaskListNum,descript:"ChildReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskGrandchild=TaskT.CreateTask(_taskListGrandchild.TaskListNum,descript:"GrandchildReminder",isUnread:true
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderGroupId:"1",reminderType:TaskReminderType.Once);
			TaskAncestors.SynchAll();
			TaskUnreads.SetRead(_userNum,taskParent,taskChild,taskGrandchild);
			#endregion 
			bool isSuccess=TaskSubscriptions.TrySubscList(_taskListParent.TaskListNum,_userNum);
			Assert.IsTrue(isSuccess);
			Assert.IsTrue(TaskUnreads.IsUnread(_userNum,taskParent));
			foreach(OpenDentBusiness.Task task in new List<OpenDentBusiness.Task> { taskChild,taskGrandchild }) {
				//User was already subscribed these these TaskLists, so the tasks should still be read.
				Assert.IsFalse(TaskUnreads.IsUnread(_userNum,task));
			}
		}

		///<summary>Method to validate that unsubscribing from a TaskList will set future reminders to read if they were previously set to unread.</summary>
		[TestMethod]
		public void TaskSubscriptions_UnsubscList_UnSubToParentMarksDescendentsFutureRemindersAsRead() {
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListParent.TaskListNum);//Subscribed to TaskListParent 
			#region Create Unread Past Due Reminders
			OpenDentBusiness.Task taskParent=TaskT.CreateTask(_taskListParent.TaskListNum,descript:"ParentReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskChild=TaskT.CreateTask(_taskListChild.TaskListNum,descript:"ChildReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskGrandchild=TaskT.CreateTask(_taskListGrandchild.TaskListNum,descript:"GrandchildReminder",isUnread:true
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderGroupId:"1",reminderType:TaskReminderType.Once);
			TaskAncestors.SynchAll();
			TaskUnreads.SetUnread(_userNum,taskParent);
			TaskUnreads.SetUnread(_userNum,taskChild);
			TaskUnreads.SetUnread(_userNum,taskGrandchild);
			#endregion 
			TaskSubscriptions.UnsubscList(_taskListParent.TaskListNum,_userNum);
			foreach(OpenDentBusiness.Task task in new List<OpenDentBusiness.Task> { taskParent,taskChild,taskGrandchild }) {
				Assert.IsFalse(TaskUnreads.IsUnread(_userNum,task));
			}
		}

		///<summary>Method to validate that unsubscribing from a TaskList will set future reminders to read if they were previously set to unread.
		///User is still subscribed to TaskListChild, so its Reminders should not be set to Read.</summary>
		[TestMethod]
		public void TaskSubscriptions_UnsubscList_UnSubToParentMarksParentTaskRead() {
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListParent.TaskListNum);//Subscribed to TaskListParent 
			TaskSubscriptionT.CreateTaskSubscription(_userNum,_taskListChild.TaskListNum);//Subscribed to TaskListChild
			#region Create Unread Past Due Reminders
			OpenDentBusiness.Task taskParent=TaskT.CreateTask(_taskListParent.TaskListNum,descript:"ParentReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskChild=TaskT.CreateTask(_taskListChild.TaskListNum,descript:"ChildReminder",isUnread:true,reminderGroupId:"1"
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderType:TaskReminderType.Once);
			OpenDentBusiness.Task taskGrandchild=TaskT.CreateTask(_taskListGrandchild.TaskListNum,descript:"GrandchildReminder",isUnread:true
				,dateTimeEntry:DateTime.Now.AddDays(1),reminderGroupId:"1",reminderType:TaskReminderType.Once);
			TaskAncestors.SynchAll();
			TaskUnreads.SetUnread(_userNum,taskParent);
			TaskUnreads.SetUnread(_userNum,taskChild);
			TaskUnreads.SetUnread(_userNum,taskGrandchild);
			#endregion 
			TaskSubscriptions.UnsubscList(_taskListParent.TaskListNum,_userNum);
			Assert.IsFalse(TaskUnreads.IsUnread(_userNum,taskParent));
			foreach(OpenDentBusiness.Task task in new List<OpenDentBusiness.Task> { taskChild,taskGrandchild }) {
				Assert.IsTrue(TaskUnreads.IsUnread(_userNum,task));
			}
		}
	}
}
