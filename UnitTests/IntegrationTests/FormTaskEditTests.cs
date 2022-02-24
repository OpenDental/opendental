using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.IntegrationTests.FormTaskEdit_Tests {
	[TestClass]
	public class FormTaskEditTests:TestBase {
		private TaskList _taskListParent;
		private TaskList _taskListChild;
		private TaskList _taskListGrandchild;
		private OpenDentBusiness.Task _task;
		private FormTaskEdit _formTaskEditInstance;
		private PrivateObject _formTaskEditAccessor;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			TaskListT.ClearTaskListTable();
			TaskT.ClearTaskTable();
			TaskSubscriptionT.ClearTaskSubscriptionTable();
			SignalodT.ClearSignalodTable();
			_taskListParent=TaskListT.CreateTaskList(descript:"TaskListParent");
			_taskListChild=TaskListT.CreateTaskList(descript:"TaskListChild",parent:_taskListParent.TaskListNum,parentDesc:_taskListParent.Descript);
			_taskListGrandchild=TaskListT.CreateTaskList(descript:"TaskListGrandchild",parent:_taskListChild.TaskListNum,
				parentDesc:_taskListChild.Descript);
			_task=TaskT.CreateTask(_taskListGrandchild.TaskListNum,descript:"Test Task",fromNum:Security.CurUser.UserNum,priorityDefNum:1);//Starts in _taskListGrandchild
			TaskSubscriptionT.CreateTaskSubscription(Security.CurUser.UserNum,_taskListParent.TaskListNum);//current user subscribes to top level tasklist.
			Security.CurUser.TaskListInBox=_taskListParent.TaskListNum;//Set inbox for current user to _taskListParent.
			try {
				Userods.Update(Security.CurUser);
				Userods.RefreshCache();
			}
			catch {
				Assert.Fail("Failed to update current user task list inbox.");//Error updating user.
			}
			_formTaskEditInstance=new FormTaskEdit(_task);
			_formTaskEditAccessor=new PrivateObject(_formTaskEditInstance);
			_formTaskEditAccessor.Invoke("LoadTask");
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
		///<summary>Correct signals are sent when user completes a TaskNote edit operation after double clicking on an existing TaskNote.</summary>
		public void FormTaskEdit_OnNoteEditComplete_CellDoubleClick() {
			_formTaskEditAccessor.Invoke("OnNoteEditComplete_CellDoubleClick",new object());
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(3,listSignals.Count);
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPopup && x.FKeyType==KeyType.Task && x.FKey==_task.TaskNum));
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));
		}

		[TestMethod]
		///<summary>Correct signals are sent when user completes a TaskNote edit operation after adding a new TaskNote.</summary>
		public void FormTaskEdit_OnNoteEditComplete_Add() {
			_formTaskEditAccessor.Invoke("OnNoteEditComplete_Add",new object());
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(3,listSignals.Count);
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPopup && x.FKeyType==KeyType.Task && x.FKey==_task.TaskNum));
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));
		}

		[TestMethod]
		///<summary>Correct signals are sent when user deletes a Task using "Delete" button, and Task is no longer in database.</summary>
		public void FormTaskEdit_butDelete_Click() {
			_formTaskEditAccessor.SetField("IsNew",true);//Causes butDelete_Click to skip UI interaction.
			_formTaskEditAccessor.Invoke("butDelete_Click",new object(),new EventArgs());
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(2,listSignals.Count);
			OpenDentBusiness.Task task=Tasks.GetOne(_task.TaskNum);
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));
			Assert.IsNull(task);
		}

		[TestMethod]
		///<summary>Correct signals are sent when user clicks "Reply" button, and Task.TaskListNum is correctly updated in database.</summary>
		public void FormTaskEdit_butReply_Click() {
			long oldTaskListNum=_task.TaskListNum;
			_formTaskEditAccessor.SetField("_replyToUserNum",Security.CurUser.UserNum);//User we are replying to.
			_formTaskEditAccessor.SetField("NotesChanged",true);//Causes butReply_Click to skip UI interaction.
			_formTaskEditAccessor.Invoke("butReply_Click",new object(),new EventArgs());
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			OpenDentBusiness.Task task=Tasks.GetOne(_task.TaskNum);
			Assert.AreEqual(4,listSignals.Count);
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==oldTaskListNum));//Old TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));//New TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPopup && x.FKeyType==KeyType.Task && x.FKey==_task.TaskNum));
			Assert.AreEqual(_task.TaskListNum,task.TaskListNum);
		}

		[TestMethod]
		///<summary>Correct signals are sent when user clicks "Reply" button (without having added a TaskNote), and Task.TaskListNum is correctly updated in database.</summary>
		public void FormTaskEdit_OnNoteEditComplete_Reply() {
			long oldTaskListNum=_task.TaskListNum;
			_formTaskEditAccessor.SetField("_replyToUserNum",Security.CurUser.UserNum);//User we are replying to.
			_formTaskEditAccessor.Invoke("OnNoteEditComplete_Reply",new object());
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(4,listSignals.Count);
			OpenDentBusiness.Task task=Tasks.GetOne(_task.TaskNum);
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==oldTaskListNum));//Old TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));//New TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPopup && x.FKeyType==KeyType.Task && x.FKey==_task.TaskNum));
			Assert.AreEqual(_task.TaskListNum,task.TaskListNum);
		}

		[TestMethod]
		///<summary>When sending a Task to multiple TaskLists, copies of the Task are made for the extra TaskLists. Verifies correct signals and Tasks are actually copied.</summary>
		public void FormTaskEdit_SaveCopy_OtherUserInbox() {
			_formTaskEditAccessor.SetField("_taskCur",_task);
			_formTaskEditAccessor.SetField("_listTaskNotes",new List<TaskNote>());
			List<long> listTaskListNums=new List<long>() { _taskListChild.TaskListNum };//Not current user's inbox, but still subscribed.
			_formTaskEditAccessor.Invoke("SaveCopy",listTaskListNums);
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(2,listSignals.Count);//One popup signal, one tasklist signal.
			List<OpenDentBusiness.Task> listAllTasks=Tasks.GetNewTasksThisUser(Security.CurUser.UserNum,Clinics.ClinicNum);
			OpenDentBusiness.Task task=listAllTasks.FirstOrDefault(x => x.TaskListNum==_taskListChild.TaskListNum);//copied task.
			Assert.IsNotNull(task);//Task was copied correctly.
			//popup signal for copied task.
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPopup && x.FKeyType==KeyType.Task && x.FKey==task.TaskNum));
			Assert.AreEqual(_taskListChild.TaskListNum,task.TaskListNum);//Correct tasklist.
		}

		[TestMethod]
		///<summary>If a note was added to a Done task and the user hits "Cancel", the task status is set to Viewed because the note is still there and the task didn't move lists.</summary>
		public void FormTaskEdit_FormTaskEdit_FormClosing_Cancel() {
			_task.TaskStatus=TaskStatusEnum.Done;
			_formTaskEditAccessor.SetField("_taskCur",_task);
			_formTaskEditAccessor.SetField("_taskOld",_task.Copy());
			Tasks.Update(_task);
			_formTaskEditAccessor.SetField("NotesChanged",true);
			_formTaskEditAccessor.Invoke("FormTaskEdit_FormClosing",new object(),new FormClosingEventArgs(CloseReason.UserClosing,false));
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(3,listSignals.Count);
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPopup && x.FKeyType==KeyType.Task && x.FKey==_task.TaskNum));
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));
		}

		[TestMethod]
		///<summary>User clicks "OK" after having changed the Description.</summary>
		public void FormTaskEdit_butOK_Click_DescriptionChanged() {
			_formTaskEditAccessor.SetField("_taskCur",_task);
			_formTaskEditAccessor.SetField("_taskOld",_task.Copy());
			string newDescript="new"+_task.Descript;
			ODtextBox textDescript=(ODtextBox)_formTaskEditAccessor.GetField("textDescript");
			textDescript.Text=newDescript;
			_formTaskEditAccessor.SetField("textDescript",textDescript);
			_formTaskEditAccessor.Invoke("butOK_Click",new object(),new EventArgs());
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(2,listSignals.Count);
			OpenDentBusiness.Task task=Tasks.GetOne(_task.TaskNum);
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));//New TL
			Assert.AreEqual(newDescript,task.Descript);
		}

		[TestMethod]
		///<summary>User clicks "OK" after having not made any changes to the Task.</summary>
		public void FormTaskEdit_butOK_NoChange() {
			_formTaskEditAccessor.SetField("_taskCur",_task);
			_formTaskEditAccessor.SetField("_taskOld",_task.Copy());
			_formTaskEditAccessor.Invoke("butOK_Click",new object(),new EventArgs());
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(0,listSignals.Count);//No signals sent.
		}

		[TestMethod]
		///<summary>Correct signals are sent when changing the TaskList of the Task.</summary>
		public void FormTaskEdit_SendSignalsRefillLocal_ChangeTaskListNum() {
			_task.TaskListNum=_taskListGrandchild.TaskListNum;
			long newTaskListNum=_taskListParent.TaskListNum;
			_formTaskEditAccessor.Invoke("SendSignalsRefillLocal",_task,newTaskListNum,true);
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(3,listSignals.Count);//Only one signal sent.
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==newTaskListNum));//new TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));//old TL
		}

		[TestMethod]
		///<summary>Correct signals are sent when changing the UserNum of the Task.</summary>
		public void FormTaskEdit_SendSignalsRefillLocal_ChangeUser() {
			long oldUserNum= Security.CurUser.UserNum;
			_formTaskEditAccessor.SetField("_userNumFrom",oldUserNum);
			_task.UserNum=Security.CurUser.UserNum+1;
			_formTaskEditAccessor.Invoke("SendSignalsRefillLocal",_task,_task.TaskListNum,true);
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(4,listSignals.Count);//Three signals sent.
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));//current TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskAuthor && x.FKeyType==KeyType.Undefined && x.FKey==oldUserNum));//old User
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskAuthor && x.FKeyType==KeyType.Undefined && x.FKey==_task.UserNum));//new User
		}

		[TestMethod]
		///<summary>Correct signals are sent when changing the Object of the Task from non Patient to a Patient.</summary>
		public void FormTaskEdit_SendSignalsRefillLocal_ChangeFromNoPatientObjectToPatient() {
			_formTaskEditAccessor.SetField("_patientPatNum",0);//No patient object on load.
			long patNum=1;
			_task.KeyNum=patNum;
			_task.ObjectType=TaskObjectType.Patient;
			_formTaskEditAccessor.Invoke("SendSignalsRefillLocal",_task,_task.TaskListNum,true);
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(3,listSignals.Count);//Two signals sent.
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));//current TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPatient && x.FKeyType==KeyType.Undefined && x.FKey==patNum));//new Patient
		}

		[TestMethod]
		///<summary>Correct signals are sent when changing the Object of the Task from a Patient to non Patient.</summary>
		public void FormTaskEdit_SendSignalsRefillLocal_ChangeFromPatientObjectToNoPatientObject() {
			long previousPatNum=55;
			_formTaskEditAccessor.SetField("_patientPatNum",previousPatNum);//Patient object on load, patnum 55.
			long patNum=0;
			_task.KeyNum=patNum;
			_task.ObjectType=TaskObjectType.None;
			_formTaskEditAccessor.Invoke("SendSignalsRefillLocal",_task,_task.TaskListNum,true);
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(3,listSignals.Count);//Two signals sent.
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));//current TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPatient && x.FKeyType==KeyType.Undefined && x.FKey==previousPatNum));//old Patient
		}
		
		[TestMethod]
		///<summary>Correct signals are sent when changing the Object of the Task from PatientA to PatientB.</summary>
		public void FormTaskEdit_SendSignalsRefillLocal_ChangeFromPatientAToPatientB() {
			long patNumA=1;
			long patNumB=2;
			_formTaskEditAccessor.SetField("_patientPatNum",patNumA);//No patient object on load.
			_task.KeyNum=patNumB;
			_task.ObjectType=TaskObjectType.Patient;
			_formTaskEditAccessor.Invoke("SendSignalsRefillLocal",_task,_task.TaskListNum,true);
			List<Signalod> listSignals=SignalodT.GetAllSignalods();
			Assert.AreEqual(4,listSignals.Count);//Three signals sent.
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskList && x.FKeyType==KeyType.Undefined && x.FKey==_task.TaskListNum));//current TL
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPatient && x.FKeyType==KeyType.Undefined && x.FKey==patNumA));//old Patient
			Assert.IsTrue(listSignals.Exists(x => x.IType==InvalidType.TaskPatient && x.FKeyType==KeyType.Undefined && x.FKey==patNumB));//new Patient
		}

	}
}
