using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Tasks_Tests {
	[TestClass]
	public class TasksTests:TestBase {
		private static Clinic _clinicN;
		private static Clinic _clinicS;
		private static Clinic _clinicNW;
		private static Patient _patN;
		private static Patient _patS;
		private static Def _defRegionN;
		private static Def _defRegionS;
		///<summary>Contains all tasks added in the setup for this test class.</summary>
		private static List<Task> _listTasks;
		///<summary>Subset of _listTasks, containing all tasks _userA is subscribed to.  Must be properly managed by test cases.</summary>
		private static List<Task> _listTasksForUserA;
		private static List<Task> _listTasksForUserNW;
		private static Userod _userA;
		private static Userod _userNW;
		private static TaskList _taskListMain;
		private static TaskList _taskListUserA;
		private static TaskList _taskListUserNW;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			_defRegionN=DefT.CreateDefinition(DefCat.Regions,"RegionN","RegionN");
			_defRegionS=DefT.CreateDefinition(DefCat.Regions,"RegionS","RegionS");
			_clinicN=ClinicT.CreateClinic("ClinicN",regionDef:_defRegionN);
			_clinicNW=ClinicT.CreateClinic("ClinicNW",regionDef:_defRegionN);
			_clinicS=ClinicT.CreateClinic("ClinicS",regionDef:_defRegionS);
			_patN=PatientT.CreatePatient("Tasks",clinicNum:_clinicN.ClinicNum,lName:_clinicN.Description,fName:"Patient");
			_patS=PatientT.CreatePatient("Tasks",clinicNum:_clinicS.ClinicNum,lName:_clinicS.Description,fName:"Patient");
			_userA=UserodT.CreateUser(userName:"TaskUserA",clinicNum:_clinicN.ClinicNum,isClinicIsRestricted:false);
			_userNW=UserodT.CreateUser(userName:"TaskUserNW",clinicNum:_clinicNW.ClinicNum,isClinicIsRestricted:true);
			List<UserClinic> listUserClinics=new List<UserClinic>() { new UserClinic(_clinicNW.ClinicNum,_userNW.UserNum) };
			if(UserClinics.Sync(listUserClinics,_userNW.UserNum)) {//Either syncs new list, or clears old list if no longer restricted.
				UserClinics.RefreshCache();
			}
		}

		[TestInitialize]
		public void SetupTest() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);
			PrefT.UpdateInt(PrefName.TasksGlobalFilterType,(int)GlobalTaskFilterType.None);//Default is disabled.  Turn on, but set to None.
			TaskListT.ClearTaskListTable();
			TaskT.ClearTaskTable();
			TaskSubscriptionT.ClearTaskSubscriptionTable();
			_listTasks=new List<Task>();
			_listTasksForUserA=new List<Task>();
			_listTasksForUserNW=new List<Task>();
			_taskListMain=TaskListT.CreateTaskList("Main");
			_taskListUserA=TaskListT.CreateTaskList(_userA.UserName);
			_taskListUserNW=TaskListT.CreateTaskList(_userNW.UserName);
			TaskSubscriptions.TrySubscList(_taskListUserA.TaskListNum,_userA.UserNum);
			TaskSubscriptions.TrySubscList(_taskListUserNW.TaskListNum,_userNW.UserNum);
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
		
		[TestMethod]
		public void Tasks_RefreshMainTrunk_ClinicFilterTasksFromOtherClinicDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshMainTrunk(false,DateTime.Now,_userA.UserNum,TaskType.All,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshMainTrunkTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshMainTrunk_ClinicFilterTasksFromThisClinicDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshMainTrunk(false,DateTime.Now,_userA.UserNum,TaskType.All,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshMainTrunkTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshMainTrunk_ClinicFilterTasksFromThisClinicDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix+"2",_patS,_userA);
			//Filter by clinicN and clinicS
			List<Task> listTasksActual=Tasks.RefreshMainTrunk(false,DateTime.Now,_userA.UserNum,TaskType.All,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum,_clinicS.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshMainTrunkTask(x) && IsTaskInClinic(x,_clinicN,_clinicS));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		#region RefreshMainTrunk
		[TestMethod]
		public void Tasks_RefreshMainTrunk_NoFilterAllTasksShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Hashtag No filter
			List<Task> listTasksActual=Tasks.RefreshMainTrunk(false,DateTime.Now,_userA.UserNum,TaskType.All,GlobalTaskFilterType.None);
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshMainTrunkTask(x));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshMainTrunk_RegionFilterTasksFromOtherRegionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshMainTrunk(false,DateTime.Now,_userA.UserNum,TaskType.All,GlobalTaskFilterType.Region,
				new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshMainTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshMainTrunk_RegionFilterTasksFromThisRegionClinicRestrictionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userNW);
			//Filter by clinicN.Region.  _userNW is restricted to _clinicNW, which is in RegionN, _patN is in _clinicN also in RegionN, and even though
			//_userNW is in RegionN as well, _userNW is restricted from _clinicN, so the Task should not show.
			List<Task> listTasksActual=Tasks.RefreshMainTrunk(false,DateTime.Now,_userNW.UserNum,TaskType.All,GlobalTaskFilterType.Region,
				new List<long> { _clinicNW.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshMainTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userNW,_clinicNW.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshMainTrunk_RegionFilterTasksFromThisRegionDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshMainTrunk(false,DateTime.Now,_userA.UserNum,TaskType.All,GlobalTaskFilterType.Region,
				new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshMainTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshMainTrunk_RegionFilterTasksFromThisRegionDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			CreateTasks(suffix+"2",_patS,_userA);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshMainTrunk(false,DateTime.Now,_userA.UserNum,TaskType.All,GlobalTaskFilterType.Region,
				new List<long> { _clinicN.Region,_clinicS.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshMainTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region,
				_clinicS.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}
		#endregion

		#region RefreshUserNew
		[TestMethod]
		public void Tasks_RefreshUserNew_ClinicFilterTasksFromOtherClinicDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListUserA.TaskListNum);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshUserNew(_userA.UserNum,GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=GetNewTasksForUser(_userA).FindAll(x => IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshUserNew_ClinicFilterTasksFromThisClinicDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListUserA.TaskListNum);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshUserNew(_userA.UserNum,GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=GetNewTasksForUser(_userA).FindAll(x => IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshUserNew_ClinicFilterTasksFromThisClinicDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListUserA.TaskListNum);
			CreateTasks(suffix,_patS,_userA,_taskListUserA.TaskListNum);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshUserNew(_userA.UserNum,GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum,
				_clinicS.ClinicNum });
			List<Task> listTasksExpected=GetNewTasksForUser(_userA).FindAll(x => IsTaskInClinic(x,_clinicN,_clinicS));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshUserNew_NoFilterAllTasksShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListUserA.TaskListNum);
			//Hashtag No Filter
			List<Task> listTasksActual=Tasks.RefreshUserNew(_userA.UserNum,GlobalTaskFilterType.None);
			List<Task> listTasksExpected=GetNewTasksForUser(_userA);
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshUserNew_RegionFilterTasksFromOtherRegionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,_taskListUserA.TaskListNum);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshUserNew(_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=GetNewTasksForUser(_userA).FindAll(x => IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region ));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshUserNew_RegionFilterTasksFromThisRegionClinicRestrictionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userNW,_taskListUserNW.TaskListNum);
			//Filter by clinicN.Region.  _userNW is restricted to _clinicNW, which is in RegionN, _patN is in _clinicN also in RegionN, and even though
			//_userNW is in RegionN as well, _userNW is restricted from _clinicN, so the Task should not show.
			List<Task> listTasksActual=Tasks.RefreshUserNew(_userNW.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicNW.Region });
			List<Task> listTasksExpected=GetNewTasksForUser(_userNW).FindAll(x => IsTaskInRegionAndUnrestricted(x,_userNW,_clinicNW.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshUserNew_RegionFilterTasksFromThisRegionDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListUserA.TaskListNum);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshUserNew(_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=GetNewTasksForUser(_userA).FindAll(x => IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshUserNew_RegionFilterTasksFromThisRegionDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,_taskListUserA.TaskListNum);
			CreateTasks(suffix,_patS,_userA,_taskListUserA.TaskListNum);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshUserNew(_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region,_clinicS.Region });
			List<Task> listTasksExpected=GetNewTasksForUser(_userA).FindAll(x => IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region,_clinicS.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}
		#endregion

		#region RefreshOpenTickets
		[TestMethod]
		public void Tasks_RefreshOpenTickets_ClinicFilterTasksFromOtherClinicDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshOpenTickets(_userA.UserNum,GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshOpenTicketsTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshOpenTickets_ClinicFilterTasksFromThisClinicDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshOpenTickets(_userA.UserNum,GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshOpenTicketsTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshOpenTickets_ClinicFilterTasksFromThisClinicDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshOpenTickets(_userA.UserNum,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum,_clinicS.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshOpenTicketsTask(x) && IsTaskInClinic(x,_clinicN,_clinicS));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshOpenTickets_NoFilterAllTasksShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Hashtag No filter
			List<Task> listTasksActual=Tasks.RefreshOpenTickets(_userA.UserNum,GlobalTaskFilterType.None);
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshOpenTicketsTask(x));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshOpenTickets_RegionFilterTasksFromOtherRegionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshOpenTickets(_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshOpenTicketsTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshOpenTickets_RegionFilterTasksFromThisRegionClinicRestrictionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userNW);
			//Filter by clinicN.Region.  _userNW is restricted to _clinicNW, which is in RegionN, _patN is in _clinicN also in RegionN, and even though
			//_userNW is in RegionN as well, _userNW is restricted from _clinicN, so the Task should not show.
			List<Task> listTasksActual=Tasks.RefreshOpenTickets(_userNW.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicNW.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshOpenTicketsTask(x) && IsTaskInRegionAndUnrestricted(x,_userNW,_clinicNW.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshOpenTickets_RegionFilterTasksFromThisRegionDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshOpenTickets(_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshOpenTicketsTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshOpenTickets_RegionFilterTasksFromThisRegionDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN.Region and clinicS.Region
			List<Task> listTasksActual=Tasks.RefreshOpenTickets(_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region,_clinicS.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshOpenTicketsTask(x) 
				&& IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region,_clinicS.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}
		#endregion

		#region RefreshPatientTickets
		[TestMethod]
		public void Tasks_RefreshPatientTickets_ClinicFilterTasksFromOtherClinicDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);//
			//Filter by clinicN, but look for _patB.  Shouldn't show because in a different clinic.
			List<Task> listTasksActual=Tasks.RefreshPatientTickets(_patS.PatNum,_userA.UserNum,GlobalTaskFilterType.Clinic,
				new List<long> {_clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshPatientTicketsTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshPatientTickets_ClinicFilterTasksFromThisClinicDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshPatientTickets(_patN.PatNum,_userA.UserNum,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshPatientTicketsTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshPatientTickets_ClinicFilterTasksFromThisClinicDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN and clinicS
			List<Task> listTasksActual=Tasks.RefreshPatientTickets(_patN.PatNum,_userA.UserNum,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum,_clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshPatientTicketsTask(x) && IsTaskInClinic(x,_clinicN,_clinicS));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshPatientTickets_NoFilterAllTasksShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Hashtag No filter
			List<Task> listTasksActual=Tasks.RefreshPatientTickets(_patN.PatNum,_userA.UserNum,GlobalTaskFilterType.None);
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshPatientTicketsTask(x));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshPatientTickets_RegionFilterTasksFromOtherRegionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN.Region, but look for _patB.  Shouldn't show because in a different region.
			List<Task> listTasksActual=Tasks.RefreshPatientTickets(_patS.PatNum,_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshPatientTicketsTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshPatientTickets_RegionFilterTasksFromThisRegionClinicRestrictionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userNW);
			//Filter by clinicN.Region.  _userNW is restricted to _clinicNW, which is in RegionN, _patN is in _clinicN also in RegionN, and even though
			//_userNW is in RegionN as well, _userNW is restricted from _clinicN, so the Task should not show.
			List<Task> listTasksActual=Tasks.RefreshPatientTickets(_patN.PatNum,_userNW.UserNum,GlobalTaskFilterType.Region,
				new List<long> { _clinicNW.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshPatientTicketsTask(x) && IsTaskInRegionAndUnrestricted(x,_userNW,_clinicNW.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshPatientTickets_RegionFilterTasksFromThisRegionDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshPatientTickets(_patN.PatNum,_userA.UserNum,GlobalTaskFilterType.Region,new List<long>{ _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshPatientTicketsTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshPatientTickets_RegionFilterTasksFromMultiRegionsDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN.Region and clincS.Region
			List<Task> listTasksActual=Tasks.RefreshPatientTickets(_patN.PatNum,_userA.UserNum,GlobalTaskFilterType.Region,
				new List<long> { _clinicN.Region,_clinicS.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshPatientTicketsTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,
				_clinicN.Region,_clinicS.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}
		#endregion

		#region RefreshRepeatingTrunk
		[TestMethod]
		public void Tasks_RefreshRepeatingTrunk_ClinicFilterTasksFromOtherClinicDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating:true);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshRepeatingTrunk(_userA.UserNum,GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTrunkTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeatingTrunk_ClinicFilterTasksFromThisClinicDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,isRepeating:true);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshRepeatingTrunk(_userA.UserNum,GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTrunkTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeatingTrunk_ClinicFilterTasksFromThisClinicDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,isRepeating: true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating: true);
			//Filter by clinicN and clinicS
			List<Task> listTasksActual=Tasks.RefreshRepeatingTrunk(_userA.UserNum,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum,_clinicS.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTrunkTask(x) && IsTaskInClinic(x,_clinicN,_clinicS));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeatingTrunk_NoFilterAllTasksShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating:true);
			//Hashtag No filter
			List<Task> listTasksActual=Tasks.RefreshRepeatingTrunk(_userA.UserNum,GlobalTaskFilterType.None);
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTrunkTask(x));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeatingTrunk_RegionFilterTasksFromOtherRegionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating:true);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshRepeatingTrunk(_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeatingTrunk_RegionFilterTasksFromThisRegionClinicRestrictionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userNW,isRepeating:true);
			//Filter by clinicN.Region.  _userNW is restricted to _clinicNW, which is in RegionN, _patN is in _clinicN also in RegionN, and even though
			//_userNW is in RegionN as well, _userNW is restricted from _clinicN, so the Task should not show.
			List<Task> listTasksActual=Tasks.RefreshRepeatingTrunk(_userNW.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicNW.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userNW,_clinicNW.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeatingTrunk_RegionFilterTasksFromThisRegionDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,isRepeating:true);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshRepeatingTrunk(_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeatingTrunk_RegionFilterTasksFromThisRegionDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,isRepeating: true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating: true);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshRepeatingTrunk(_userA.UserNum,GlobalTaskFilterType.Region,
				new List<long> { _clinicN.Region,_clinicS.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTrunkTask(x) 
				&& IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region,_clinicS.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}
		#endregion

		#region RefreshChildren
		[TestMethod]
		public void Tasks_RefreshChildren_ClinicFilterTasksFromOtherClinicDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long taskListNum=_taskListMain.TaskListNum;
			long userInbox=TaskLists.GetMailboxUserNum(taskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListNum);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshChildren(taskListNum,false,DateTime.Today,_userA.UserNum,userInbox,TaskType.All,false,
				GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshChildrenTask(x,taskListNum) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshChildren_ClinicFilterTasksFromThisClinicDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long taskListNum=_taskListMain.TaskListNum;
			long userInbox=TaskLists.GetMailboxUserNum(taskListNum);
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListNum);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshChildren(taskListNum,false,DateTime.Today,_userA.UserNum,userInbox,TaskType.All,false,
				GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshChildrenTask(x,taskListNum) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshChildren_ClinicFilterTasksFromThisClinicDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long taskListNum=_taskListMain.TaskListNum;
			long userInbox=TaskLists.GetMailboxUserNum(taskListNum);
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListNum);
			//Filter by clinicN and clinicS
			List<Task> listTasksActual=Tasks.RefreshChildren(taskListNum,false,DateTime.Today,_userA.UserNum,userInbox,TaskType.All,false,
				GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum,_clinicS.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshChildrenTask(x,taskListNum) && IsTaskInClinic(x,_clinicN,_clinicS));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshChildren_NoFilterAllTasksShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long taskListNum=_taskListMain.TaskListNum;
			long userInbox=TaskLists.GetMailboxUserNum(taskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListNum);
			//Hashtag No filter
			List<Task> listTasksActual=Tasks.RefreshChildren(taskListNum,false,DateTime.Today,_userA.UserNum,userInbox,TaskType.All,false,GlobalTaskFilterType.None);
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshChildrenTask(x,taskListNum));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshChildren_RegionFilterTasksFromOtherRegionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long taskListNum=_taskListMain.TaskListNum;
			long userInbox=TaskLists.GetMailboxUserNum(taskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,taskListNum);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshChildren(taskListNum,false,DateTime.Today,_userA.UserNum,userInbox,TaskType.All,false,
				GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshChildrenTask(x,taskListNum) && IsTaskInRegionAndUnrestricted(x,_userA, _clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshChildren_RegionFilterTasksFromThisRegionClinicRestrictionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long taskListNum=_taskListMain.TaskListNum;
			long userInbox=TaskLists.GetMailboxUserNum(taskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userNW,taskListNum);
			//Filter by clinicN.Region.  _userNW is restricted to _clinicNW, which is in RegionN, _patN is in _clinicN also in RegionN, and even though
			//_userNW is in RegionN as well, _userNW is restricted from _clinicN, so the Task should not show.
			List<Task> listTasksActual=Tasks.RefreshChildren(taskListNum,false,DateTime.Today,_userNW.UserNum,userInbox,TaskType.All,false,
				GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshChildrenTask(x,taskListNum) && IsTaskInRegionAndUnrestricted(x,_userNW,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshChildren_RegionFilterTasksFromThisRegionDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long taskListNum=_taskListMain.TaskListNum;
			long userInbox=TaskLists.GetMailboxUserNum(taskListNum);
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListNum);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshChildren(taskListNum,false,DateTime.Today,_userA.UserNum,userInbox,TaskType.All,false,
				GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshChildrenTask(x,taskListNum) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshChildren_RegionFilterTasksFromThisRegionDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long taskListNum=_taskListMain.TaskListNum;
			long userInbox=TaskLists.GetMailboxUserNum(taskListNum);
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListNum);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,taskListNum);
			//Filter by clinicN.Region and clinicS.Region
			List<Task> listTasksActual=Tasks.RefreshChildren(taskListNum,false,DateTime.Today,_userA.UserNum,userInbox,TaskType.All,false,
				GlobalTaskFilterType.Region,new List<long> { _clinicN.Region,_clinicS.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshChildrenTask(x,taskListNum) && IsTaskInRegionAndUnrestricted(x,_userA,
				_clinicN.Region,_clinicS.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}
		#endregion

		#region RefreshRepeating
		[TestMethod]
		public void Tasks_RefreshRepeating_ClinicFilterTasksFromOtherClinicDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating:true);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshRepeating(TaskDateType.None,_userA.UserNum,GlobalTaskFilterType.Clinic,
				new List<long> {_clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeating_ClinicFilterTasksFromThisClinicDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,isRepeating:true);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshRepeating(TaskDateType.None,_userA.UserNum,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeating_ClinicFilterTasksFromThisClinicDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,isRepeating: true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating: true);
			//Filter by clinicN and clinicS
			List<Task> listTasksActual=Tasks.RefreshRepeating(TaskDateType.None,_userA.UserNum,GlobalTaskFilterType.Clinic,
				new List<long> { _clinicN.ClinicNum,_clinicS.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTask(x) && IsTaskInClinic(x,_clinicN,_clinicS));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeating_NoFilterAllTasksShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating:true);
			//Hashtag No filter
			List<Task> listTasksActual=Tasks.RefreshRepeating(TaskDateType.None,_userA.UserNum,GlobalTaskFilterType.None);
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTask(x));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeating_RegionFilterTasksFromOtherRegionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating:true);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshRepeating(TaskDateType.None,_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeating_RegionFilterTasksFromThisRegionClinicRestrictionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userNW,isRepeating:true);
			//Filter by clinicN.Region.  _userNW is restricted to _clinicNW, which is in RegionN, _patN is in _clinicN also in RegionN, and even though
			//_userNW is in RegionN as well, _userNW is restricted from _clinicN, so the Task should not show.
			List<Task> listTasksActual=Tasks.RefreshRepeating(TaskDateType.None,_userNW.UserNum,GlobalTaskFilterType.Region,
				new List<long> { _clinicNW.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTask(x) && IsTaskInRegionAndUnrestricted(x,_userNW));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeating_RegionFilterTasksFromThisRegionDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,isRepeating:true);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshRepeating(TaskDateType.None,_userA.UserNum,GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshRepeating_RegionFilterTasksFromThisRegionDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA,isRepeating:true);
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA,isRepeating:true);
			//Filter by clinicN.Region and clinicS.Region
			List<Task> listTasksActual=Tasks.RefreshRepeating(TaskDateType.None,_userA.UserNum,GlobalTaskFilterType.Region,
				new List<long> { _clinicN.Region,_clinicS.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshRepeatingTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,
				_clinicN.Region,_clinicS.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}
		#endregion

		#region RefreshDatedTrunk
		[TestMethod]
		public void Tasks_RefreshDatedTrunk_ClinicFilterTasksFromOtherClinicDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,false,DateTime.Today,_userA.UserNum,
				GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshDatedTrunkTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshDatedTrunk_ClinicFilterTasksFromThisClinicDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Filter by clinicN
			List<Task> listTasksActual=Tasks.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,false,DateTime.Today,_userA.UserNum,
				GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshDatedTrunkTask(x) && IsTaskInClinic(x,_clinicN));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshDatedTrunk_ClinicFilterTasksFromThisClinicDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN and clinicS
			List<Task> listTasksActual=Tasks.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,false,DateTime.Today,_userA.UserNum,
				GlobalTaskFilterType.Clinic,new List<long> { _clinicN.ClinicNum,_clinicS.ClinicNum });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshDatedTrunkTask(x) && IsTaskInClinic(x,_clinicN,_clinicS));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshDatedTrunk_NoFilterAllTasksShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Hashtag No filter
			List<Task> listTasksActual=Tasks.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,false,DateTime.Today,_userA.UserNum,GlobalTaskFilterType.None);
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshDatedTrunkTask(x));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshDatedTrunk_RegionFilterTasksFromOtherRegionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicS appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,false,DateTime.Today,_userA.UserNum,
				GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshDatedTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshDatedTrunk_RegionFilterTasksFromThisRegionClinicRestrictionDoNotShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userNW);
			//Filter by clinicN.Region.  _userNW is restricted to _clinicNW, which is in RegionN, _patN is in _clinicN also in RegionN, and even though
			//_userNW is in RegionN as well, _userNW is restricted from _clinicN, so the Task should not show.
			List<Task> listTasksActual=Tasks.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,false,DateTime.Today,_userNW.UserNum,
				GlobalTaskFilterType.Region,new List<long> { _clinicNW.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshDatedTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userNW,_clinicNW.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshDatedTrunk_RegionFilterTasksFromThisRegionDoShow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Filter by clinicN.Region
			List<Task> listTasksActual=Tasks.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,false,DateTime.Today,_userA.UserNum,
				GlobalTaskFilterType.Region,new List<long> { _clinicN.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshDatedTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,_clinicN.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}

		[TestMethod]
		public void Tasks_RefreshDatedTrunk_RegionFilterTasksFromThisRegionDoShowMulti() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patN,_userA);
			//Add clinicN appointment and patient Tasks.
			CreateTasks(suffix,_patS,_userA);
			//Filter by clinicN.Region and clinicS.Region
			List<Task> listTasksActual=Tasks.RefreshDatedTrunk(DateTime.Today,TaskDateType.None,false,DateTime.Today,_userA.UserNum,
				GlobalTaskFilterType.Region,new List<long> { _clinicN.Region,_clinicS.Region });
			List<Task> listTasksExpected=_listTasks.FindAll(x => IsRefreshDatedTrunkTask(x) && IsTaskInRegionAndUnrestricted(x,_userA,
				_clinicN.Region,_clinicS.Region));
			Assert.IsTrue(ContainsSameTaskNums(listTasksExpected,listTasksActual));
		}
		#endregion

		#region HelperMethods
		private bool IsRefreshMainTrunkTask(Task task) {
			return (task.TaskListNum==0 && task.TaskStatus!=TaskStatusEnum.Done);
		}

		private List<Task> GetNewTasksForUser(Userod user) {
			List<Task> listTasksNew=new List<Task>();
			if(user.UserNum==_userA.UserNum) {
				listTasksNew=_listTasksForUserA.FindAll(x => x.TaskStatus!=TaskStatusEnum.Done);
			}
			else if(user.UserNum==_userNW.UserNum) {
				listTasksNew=_listTasksForUserNW.FindAll(x => x.TaskStatus!=TaskStatusEnum.Done);
			}
			return listTasksNew;
		}

		private bool IsRefreshOpenTicketsTask(Task task) {
			return (task.UserNum==_userA.UserNum && task.ObjectType==TaskObjectType.Patient && task.TaskStatus!=TaskStatusEnum.Done);
		}

		private bool IsRefreshPatientTicketsTask(Task task) {
			return (task.TaskStatus!=TaskStatusEnum.Done && task.ObjectType==TaskObjectType.Patient && task.KeyNum==_patN.PatNum);
		}

		private bool IsRefreshRepeatingTrunkTask(Task task) {
			return (task.IsRepeating && string.IsNullOrWhiteSpace(task.ReminderGroupId) && task.DateType==TaskDateType.None);
		}

		private bool IsRefreshChildrenTask(Task task,long taskListNum) {
			return (task.TaskStatus!=TaskStatusEnum.Done && task.TaskListNum==taskListNum);
		}

		private bool IsRefreshRepeatingTask(Task task) {
			return (task.IsRepeating && string.IsNullOrWhiteSpace(task.ReminderGroupId) && task.DateType==TaskDateType.None);
		}

		private bool IsRefreshDatedTrunkTask(Task task) {
			return (task.DateType==TaskDateType.None && task.TaskStatus!=TaskStatusEnum.Done);
		}

		///<summary>Creates an Appointment Task and a Patient Task using pat and clinic, and adds them to _listTasks.</summary>
		private void CreateTasks(string suffix,Patient pat,Userod user,long taskListNum=0,bool isRepeating=false) {
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,0,0,clinicNum:pat.ClinicNum);
			Task taskAppt=TaskT.CreateTask(taskListNum,appt.AptNum,suffix,isRepeating:isRepeating,dateType:TaskDateType.None,objectType:TaskObjectType.Appointment,userNum:user.UserNum);
			_listTasks.Add(taskAppt);
			Task taskPat=TaskT.CreateTask(taskListNum,pat.PatNum,suffix,isRepeating:isRepeating,dateType:TaskDateType.None,objectType:TaskObjectType.Patient,userNum:user.UserNum);
			_listTasks.Add(taskPat);
			Task taskNone=TaskT.CreateTask(taskListNum,0,suffix,isRepeating:isRepeating,dateType:TaskDateType.None,objectType:TaskObjectType.None,userNum:user.UserNum);
			_listTasks.Add(taskNone);
			//Manage test lists of "subscribed" tasks so we don't have to do database logic to determine if a user is subscribed to a task.
			if(OpenDental.UserControlTasks.GetSubscribedTaskLists(_userA.UserNum).Exists(x => x.TaskListNum==taskListNum)) {
				TaskUnreads.SetUnread(_userA.UserNum,taskAppt);
				TaskUnreads.SetUnread(_userA.UserNum,taskPat);
				TaskUnreads.SetUnread(_userA.UserNum,taskNone);
				_listTasksForUserA.Add(taskAppt);
				_listTasksForUserA.Add(taskPat);
				_listTasksForUserA.Add(taskNone);
			}
			if(OpenDental.UserControlTasks.GetSubscribedTaskLists(_userNW.UserNum).Exists(x => x.TaskListNum==taskListNum)) {
				TaskUnreads.SetUnread(_userNW.UserNum,taskAppt);
				TaskUnreads.SetUnread(_userNW.UserNum,taskPat);
				TaskUnreads.SetUnread(_userNW.UserNum,taskNone);
				_listTasksForUserNW.Add(taskAppt);
				_listTasksForUserNW.Add(taskPat);
				_listTasksForUserNW.Add(taskNone);
			}
		}

		///<summary>Determines if the Task is in the Clinic.</summary>
		private bool IsTaskInClinic(Task task,params Clinic[] listClinics) {
			bool retVal=false;
			switch(task.ObjectType) {
				case TaskObjectType.Appointment:
					Appointment appt=Appointments.GetOneApt(task.KeyNum);
					//Is Appointment's clinic in the same clinic as passed in clinic?
					//0 clinic also allowed.
					if(appt.ClinicNum==0 || listClinics.Any(x=>x.ClinicNum==appt.ClinicNum)) {
						retVal=true;
					}
					break;
				case TaskObjectType.Patient:
					Patient pat=Patients.GetPat(task.KeyNum);
					if(pat.ClinicNum==0 || listClinics.Any(x => x.ClinicNum==pat.ClinicNum)) {
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
		private bool IsTaskInRegionAndUnrestricted(Task task,Userod user,params long[] listRegions) {
			bool retVal=false;
			List<Clinic> listUnrestrictedClinics=Clinics.GetAllForUserod(user).FindAll(x=> ListTools.In(x.Region,listRegions));
			switch(task.ObjectType) {
				case TaskObjectType.Appointment:
					Appointment appt=Appointments.GetOneApt(task.KeyNum);
					Clinic clinicAppt=Clinics.GetClinic(appt.ClinicNum);
					//Is Appointment's clinic in the same region as passed in clinic, and user has permission to view the clinicAppt?
					//0 Region also allowed.
					if(clinicAppt.Region==0 || listUnrestrictedClinics.Any(x => x.ClinicNum==clinicAppt.ClinicNum)) {
						retVal=true;
					}
					break;
				case TaskObjectType.Patient:
					Patient pat=Patients.GetPat(task.KeyNum);
					Clinic clinicPat=Clinics.GetClinic(pat.ClinicNum);
					//Is Appointment's clinic in the same region as passed in clinic, and user has permission to view the clinicPat?
					//0 Region also allowed.
					if(clinicPat.Region==0 || listUnrestrictedClinics.Any(x =>  x.ClinicNum==clinicPat.ClinicNum)) {
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
			List<long> listActualNotExpected=listActual.Select(x => x.TaskNum).Except(listExpected.Select(x => x.TaskNum)).ToList();
			List<long> listExpectedNotActual=listExpected.Select(x => x.TaskNum).Except(listActual.Select(x => x.TaskNum)).ToList();
			return !listActualNotExpected.Any() && !listExpectedNotActual.Any();
		}
		#endregion
	}
}
