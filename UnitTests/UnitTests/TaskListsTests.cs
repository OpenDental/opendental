using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.TaskLists_Tests {
	[TestClass]
	public class TaskListsTests:TestBase {
		private static Clinic _clinicN;
		private static Clinic _clinicS;
		private static Clinic _clinicNW;
		private static Patient _patN;
		private static Patient _patS;
		private static Def _defRegionN;
		private static Def _defRegionS;
		private static Userod _userA;
		private static Userod _userNW;
		private static TaskList _taskListMainNoFilter;
		private static TaskList _taskListClinic;
		private static TaskList _taskListRegion;
		private static TaskList _taskListRepeating;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			_defRegionN=DefT.CreateDefinition(DefCat.Regions,"RegionN","RegionN");
			_defRegionS=DefT.CreateDefinition(DefCat.Regions,"RegionS","RegionS");
			_clinicN=ClinicT.CreateClinic("ClinicN",regionDef: _defRegionN);
			_clinicNW=ClinicT.CreateClinic("ClinicNW",regionDef: _defRegionN);
			_clinicS=ClinicT.CreateClinic("ClinicS",regionDef: _defRegionS);
			_patN=PatientT.CreatePatient("Tasks",clinicNum: _clinicN.ClinicNum,lName: _clinicN.Description,fName: "Patient");
			_patS=PatientT.CreatePatient("Tasks",clinicNum: _clinicS.ClinicNum,lName: _clinicS.Description,fName: "Patient");
			_userA=UserodT.CreateUser(userName: "TaskUserA",clinicNum: _clinicN.ClinicNum,isClinicIsRestricted: false);
			_userNW=UserodT.CreateUser(userName: "TaskUserNW",clinicNum: _clinicNW.ClinicNum,isClinicIsRestricted: true);
			Userods.RefreshCache();
			List<UserClinic> listUserClinics = new List<UserClinic>() { new UserClinic(_clinicNW.ClinicNum,_userNW.UserNum) };
			if(UserClinics.Sync(listUserClinics,_userNW.UserNum)) {//Either syncs new list, or clears old list if no longer restricted.
				UserClinics.RefreshCache();
			}
		}

		[TestInitialize]
		public void SetupTest() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);
			PrefT.UpdateInt(PrefName.TasksGlobalFilterType,(int)GlobalTaskFilterType.None);
			TaskListT.ClearTaskListTable();
			TaskT.ClearTaskTable();
			TaskSubscriptionT.ClearTaskSubscriptionTable();
			_taskListMainNoFilter=TaskListT.CreateTaskList(descript:"No Filter",parent:0,globalTaskFilterType:GlobalTaskFilterType.None);
			_taskListClinic=TaskListT.CreateTaskList(descript:"Clinic Filter",parent:_taskListMainNoFilter.TaskListNum,globalTaskFilterType:GlobalTaskFilterType.Clinic);
			_taskListRegion=TaskListT.CreateTaskList(descript:"Region Filter",parent:_taskListMainNoFilter.TaskListNum,globalTaskFilterType:GlobalTaskFilterType.Region);
			_taskListRepeating=TaskListT.CreateTaskList("Repeating",isRepeating:true);
			TaskSubscriptions.TrySubscList(_taskListMainNoFilter.TaskListNum,_userA.UserNum);
			TaskSubscriptions.TrySubscList(_taskListClinic.TaskListNum,_userA.UserNum);
			TaskSubscriptions.TrySubscList(_taskListRegion.TaskListNum,_userA.UserNum);
			TaskSubscriptions.TrySubscList(_taskListRegion.TaskListNum,_userNW.UserNum);
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			TaskListT.ClearTaskListTable();
			TaskT.ClearTaskTable();
			TaskSubscriptionT.ClearTaskSubscriptionTable();
			ClinicT.ClearClinicTable();
		}

		#region RefreshMainTrunk
		[TestMethod]
		public void TaskLists_RefreshMainTrunk_NoFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListMainNoFilter.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshMainTrunk(_userA.UserNum,TaskType.All,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskListMain=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListMainNoFilter.TaskListNum);
			Assert.IsTrue(taskListMain!=null && taskListMain.NewTaskCount==3);
		}

		[TestMethod]
		public void TaskLists_RefreshMainTrunk_ClinicFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListMainClinicFilter=TaskListT.CreateTaskList(descript:"Main Clinic Filter",parent:0,globalTaskFilterType:GlobalTaskFilterType.Clinic);
			TaskSubscriptions.TrySubscList(taskListMainClinicFilter.TaskListNum,_userA.UserNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListMainClinicFilter.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshMainTrunk(_userA.UserNum,TaskType.All,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskListMain=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListMainClinicFilter.TaskListNum);
			Assert.IsTrue(taskListMain!=null && taskListMain.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshMainTrunk_RegionFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListMainRegionFilter=TaskListT.CreateTaskList(descript:"Main Region Filter",parent:0,globalTaskFilterType:GlobalTaskFilterType.Region);
			TaskSubscriptions.TrySubscList(taskListMainRegionFilter.TaskListNum,_userA.UserNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListMainRegionFilter.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshMainTrunk(_userA.UserNum,TaskType.All,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListMainRegionFilter.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshMainTrunk_RegionFilterClinicRestriction() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListMainRegionFilter=TaskListT.CreateTaskList(descript:"Main Region Filter",parent:0,globalTaskFilterType:GlobalTaskFilterType.Region);
			TaskSubscriptions.TrySubscList(taskListMainRegionFilter.TaskListNum,_userNW.UserNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListMainRegionFilter.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshMainTrunk(_userNW.UserNum,TaskType.All,_clinicNW.ClinicNum,_clinicNW.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListMainRegionFilter.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}
		#endregion

		#region RefreshUserTrunk
		[TestMethod]
		public void TaskLists_RefreshUserTrunk_NoFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListMainNoFilter.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshUserTrunk(_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListMainNoFilter.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==3);
		}

		[TestMethod]
		public void TaskLists_RefreshUserTrunk_ClinicFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListClinic.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshUserTrunk(_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListClinic.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshUserTrunk_RegionFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshUserTrunk(_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshUserTrunk_RegionFilterClinicRestriction() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshUserTrunk(_userNW.UserNum,_clinicNW.ClinicNum,_clinicNW.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}
		#endregion

		#region RefreshRepeatingTrunk
		[TestMethod]
		public void TaskLists_RefreshRepeatingTrunk_NoFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListRepeating.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshRepeatingTrunk(_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListRepeating.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==3);
		}

		[TestMethod]
		public void TaskLists_RefreshRepeatingTrunk_ClinicFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListClinic=TaskListT.CreateTaskList(descript:"Clinic Filter",parent:0,globalTaskFilterType:GlobalTaskFilterType.Clinic
				,isRepeating:true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListClinic.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshRepeatingTrunk(_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListClinic.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshRepeatingTrunk_RegionFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListRegion=TaskListT.CreateTaskList(descript:"Region Filter",parent:0,globalTaskFilterType:GlobalTaskFilterType.Region
				,isRepeating:true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshRepeatingTrunk(_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshRepeatingTrunk_RegionFilterClinicRestriction() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListRegion=TaskListT.CreateTaskList(descript:"Region Filter",parent:0,globalTaskFilterType:GlobalTaskFilterType.Region
				,isRepeating:true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshRepeatingTrunk(_userNW.UserNum,_clinicNW.ClinicNum,_clinicNW.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}
		#endregion

		#region RefreshChildren
		[TestMethod]
		public void TaskLists_RefreshChildren_NoFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListNoFilter=TaskListT.CreateTaskList(descript:"No Filter",parent:_taskListMainNoFilter.TaskListNum,globalTaskFilterType:GlobalTaskFilterType.None);
			long userInbox=TaskLists.GetMailboxUserNum(_taskListMainNoFilter.TaskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListNoFilter.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshChildren(_taskListMainNoFilter.TaskListNum,_userA.UserNum,userInbox,TaskType.All
				,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListNoFilter.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==3);
		}

		[TestMethod]
		public void TaskLists_RefreshChildren_ClinicFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			long userInbox=TaskLists.GetMailboxUserNum(_taskListMainNoFilter.TaskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListClinic.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshChildren(_taskListMainNoFilter.TaskListNum,_userA.UserNum,userInbox,TaskType.All
				,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskListUserA=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListClinic.TaskListNum);
			Assert.IsTrue(taskListUserA!=null && taskListUserA.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshChildren_RegionFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			long userInbox=TaskLists.GetMailboxUserNum(_taskListMainNoFilter.TaskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshChildren(_taskListMainNoFilter.TaskListNum,_userA.UserNum,userInbox,TaskType.All
				,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshChildren_RegionFilterClinicRestriction() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			long userInbox=TaskLists.GetMailboxUserNum(_taskListMainNoFilter.TaskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshChildren(_taskListMainNoFilter.TaskListNum,_userNW.UserNum,userInbox,TaskType.All
				,_clinicNW.ClinicNum,_clinicNW.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}
		#endregion

		#region RefreshRepeating
		[TestMethod]
		public void TaskLists_RefreshRepeating_NoFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListRepeating.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshRepeating(TaskDateType.None,_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListRepeating.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==3);
		}

		[TestMethod]
		public void TaskLists_RefreshRepeating_ClinicFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListClinic=TaskListT.CreateTaskList(descript:"Clinic Filter",parent:_taskListMainNoFilter.TaskListNum,globalTaskFilterType:GlobalTaskFilterType.Clinic
				,isRepeating:true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListClinic.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshRepeating(TaskDateType.None,_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListClinic.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshRepeating_RegionFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListRegion=TaskListT.CreateTaskList(descript:"Region Filter",parent:_taskListMainNoFilter.TaskListNum,globalTaskFilterType:GlobalTaskFilterType.Region
				,isRepeating:true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshRepeating(TaskDateType.None,_userA.UserNum,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshRepeating_RegionFilterClinicRestriction() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListRegion=TaskListT.CreateTaskList(descript:"Region Filter",parent:_taskListMainNoFilter.TaskListNum,globalTaskFilterType:GlobalTaskFilterType.Region
				,isRepeating:true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshRepeating(TaskDateType.None,_userNW.UserNum,_clinicNW.ClinicNum,_clinicNW.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}
		#endregion

		#region RefreshDatedTrunk
		[TestMethod]
		public void TaskLists_RefreshDatedTrunk_NoFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			TaskList taskListNoFilter=TaskListT.CreateTaskList(descript:"No Filter",parent:_taskListMainNoFilter.TaskListNum,globalTaskFilterType:GlobalTaskFilterType.None);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListNoFilter.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,_userA.UserNum
				,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==taskListNoFilter.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==3);
		}

		[TestMethod]
		public void TaskLists_RefreshDatedTrunk_ClinicFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListClinic.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,_userA.UserNum
				,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskListUserA=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListClinic.TaskListNum);
			Assert.IsTrue(taskListUserA!=null && taskListUserA.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshDatedTrunk_RegionFilter() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,_userA.UserNum
				,_clinicN.ClinicNum,_clinicN.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}

		[TestMethod]
		public void TaskLists_RefreshDatedTrunk_RegionFilterClinicRestriction() {
			string suffix = MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListRegion.TaskListNum);
			//Hashtag No filter
			List<TaskList> listTaskListsActual=TaskLists.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,_userNW.UserNum
				,_clinicNW.ClinicNum,_clinicNW.Region);
			TaskList taskList=listTaskListsActual.FirstOrDefault(x => x.TaskListNum==_taskListRegion.TaskListNum);
			Assert.IsTrue(taskList!=null && taskList.NewTaskCount==1);
		}
		#endregion

		#region HelperMethods
		///<summary>Creates an Appointment Task and a Patient Task using pat and clinic, and adds them to _listTasks.</summary>
		private void CreateTasks(string suffix,Patient pat,Userod user,long taskListNum,bool isRepeating = false) {
			Appointment appt = AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,0,0,clinicNum: pat.ClinicNum);
			Task taskAppt = TaskT.CreateTask(taskListNum,appt.AptNum,suffix,isRepeating: isRepeating,dateType: TaskDateType.None,objectType: TaskObjectType.Appointment,userNum: user.UserNum);
			Task taskPat = TaskT.CreateTask(taskListNum,pat.PatNum,suffix,isRepeating: isRepeating,dateType: TaskDateType.None,objectType: TaskObjectType.Patient,userNum: user.UserNum);
			Task taskNone = TaskT.CreateTask(taskListNum,0,suffix,isRepeating: isRepeating,dateType: TaskDateType.None,objectType: TaskObjectType.None,userNum: user.UserNum);
			//Manage test lists of "subscribed" tasks so we don't have to do database logic to determine if a user is subscribed to a task.
			if(OpenDental.UserControlTasks.GetSubscribedTaskLists(_userA.UserNum).Exists(x => x.TaskListNum==taskListNum)) {
				TaskUnreads.SetUnread(_userA.UserNum,taskAppt);
				TaskUnreads.SetUnread(_userA.UserNum,taskPat);
				TaskUnreads.SetUnread(_userA.UserNum,taskNone);
			}
			if(OpenDental.UserControlTasks.GetSubscribedTaskLists(_userNW.UserNum).Exists(x => x.TaskListNum==taskListNum)) {
				TaskUnreads.SetUnread(_userNW.UserNum,taskAppt);
				TaskUnreads.SetUnread(_userNW.UserNum,taskPat);
				TaskUnreads.SetUnread(_userNW.UserNum,taskNone);
			}
		}

		///<summary>Determines if the Task is in the Clinic.</summary>
		private bool IsTaskInClinic(Task task,Clinic clinic) {
			bool retVal = false;
			switch(task.ObjectType) {
				case TaskObjectType.Appointment:
					Appointment appt = Appointments.GetOneApt(task.KeyNum);
					//Is Appointment's clinic in the same clinic as passed in clinic?
					//0 clinic also allowed.
					if(appt.ClinicNum==0 || appt.ClinicNum==clinic.ClinicNum) {
						retVal=true;
					}
					break;
				case TaskObjectType.Patient:
					Patient pat = Patients.GetPat(task.KeyNum);
					if(pat.ClinicNum==0 || pat.ClinicNum==clinic.ClinicNum) {
						retVal=true;
					}
					break;
				case TaskObjectType.None:
					retVal=true;
					break;
			}
			return retVal;
		}

		///<summary>Determines if the Task is in the Clinic's region.</summary>
		private bool IsTaskInRegionAndUnrestricted(Task task,Userod user) {
			bool retVal = false;
			Clinic clinic = Clinics.GetClinic(user.ClinicNum);
			List<long> listUserClinicNums = Clinics.GetAllForUserod(user).Select(x => x.ClinicNum).ToList();
			switch(task.ObjectType) {
				case TaskObjectType.Appointment:
					Appointment appt = Appointments.GetOneApt(task.KeyNum);
					Clinic clinicAppt = Clinics.GetClinic(appt.ClinicNum);
					//Is Appointment's clinic in the same region as passed in clinic, and user has permission to view the clinicAppt?
					//0 Region also allowed.
					if(clinicAppt.Region==0 || (clinicAppt.Region==clinic.Region && listUserClinicNums.Any(x => x==clinicAppt.ClinicNum))) {
						retVal=true;
					}
					break;
				case TaskObjectType.Patient:
					Patient pat = Patients.GetPat(task.KeyNum);
					Clinic clinicPat = Clinics.GetClinic(pat.ClinicNum);
					//Is Appointment's clinic in the same region as passed in clinic, and user has permission to view the clinicPat?
					//0 Region also allowed.
					if(clinicPat.Region==0 || (clinicPat.Region==clinic.Region && listUserClinicNums.Any(x => x==clinicPat.ClinicNum))) {
						retVal=true;
					}
					break;
				case TaskObjectType.None:
					retVal=true;
					break;
			}
			return retVal;
		}

		///<summary>Returns true if the two lists of Tasks contain Tasks of the same TaskNums.</summary>
		private bool ContainsSameTaskNums(List<Task> listExpected,List<Task> listActual) {
			List<long> listActualNotExpected = listActual.Select(x => x.TaskNum).Except(listExpected.Select(x => x.TaskNum)).ToList();
			List<long> listExpectedNotActual = listExpected.Select(x => x.TaskNum).Except(listActual.Select(x => x.TaskNum)).ToList();
			return !listActualNotExpected.Any() && !listExpectedNotActual.Any();
		}
		#endregion
	}
}
