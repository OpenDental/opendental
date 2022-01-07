using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.SecurityLogs_Tests {
	[TestClass]
	public class SecurityLogsTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void SecurityLogs_MakeLogEntry_DuplicateEntry() {
			Patient patient=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			//There are lots of bug submissions with exception text like "Duplicate entry 'XXXXX' for key 'PRIMARY'".
			//OpenDentBusiness.SecurityLogs.MakeLogEntry() seems to be the common theme for most of the submissions.
			//Loop as fast as we can and insert 200 security logs trying to get a duplicate entry exception.
			for(int i=0;i<200;i++) {
				try {
					SecurityLogs.MakeLogEntry(Permissions.Accounting,patient.PatNum,"",0,DateTime.Now.AddDays(-7));
				}
				catch(Exception ex) {
					Assert.Fail(ex.Message);
					break;
				}
			}
		}

		[TestMethod]
		public void SecurityLogs_MakeLogEntry_DuplicateEntryRandomKeys() {
			Patient patient=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			//First, turn on random primary keys
			PrefT.UpdateBool(PrefName.RandomPrimaryKeys,true);
			//There are lots of bug submissions with exception text like "Duplicate entry 'XXXXX' for key 'PRIMARY'".
			//OpenDentBusiness.SecurityLogs.MakeLogEntry() seems to be the common theme for most of the submissions.
			//Loop as fast as we can and insert 200 security logs trying to get a duplicate entry exception.
			for(int i=0;i<200;i++) {
				try {
					SecurityLogs.MakeLogEntry(Permissions.Accounting,patient.PatNum,"",0,DateTime.Now.AddDays(-7));
				}
				catch(Exception ex) {
					PrefT.UpdateBool(PrefName.RandomPrimaryKeys,false);
					Assert.Fail(ex.Message);
					break;
				}
			}
			PrefT.UpdateBool(PrefName.RandomPrimaryKeys,false);
		}

		[TestMethod]
		public void SecurityLogs_MakeLogEntry_DuplicateEntryParallel() {
			Patient patient=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			//There are lots of bug submissions with exception text like "Duplicate entry 'XXXXX' for key 'PRIMARY'".
			//OpenDentBusiness.SecurityLogs.MakeLogEntry() seems to be the common theme for most of the submissions.
			//Spawn parallel threads to insert 200 security logs trying to get a duplicate entry exception.
			List<Action> listActions=new List<Action>();
			for(int i=0;i<200;i++) {
				listActions.Add(() => SecurityLogs.MakeLogEntry(Permissions.Accounting,patient.PatNum,"",0,DateTime.Now.AddDays(-7)));
			}
			//Parallel threads do not support Middle Tier mode when unit testing due to how we have to fake being both the client and the server.
			RemotingRole remotingRoleOld=RemotingClient.RemotingRole;
			if(RemotingClient.RemotingRole!=RemotingRole.ClientDirect) {
				RemotingClient.RemotingRole=RemotingRole.ClientDirect;
			}
			ODThread.RunParallel(listActions,onException:(ex) => {
				RemotingClient.RemotingRole=remotingRoleOld;
				Assert.Fail(ex.Message);
			});
			RemotingClient.RemotingRole=remotingRoleOld;
		}

		[TestMethod]
		public void SecurityLogs_MakeLogEntry_DuplicateEntryParallelRandomKeys() {
			Patient patient=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			//First, turn on random primary keys
			PrefT.UpdateBool(PrefName.RandomPrimaryKeys,true);
			//There are lots of bug submissions with exception text like "Duplicate entry 'XXXXX' for key 'PRIMARY'".
			//OpenDentBusiness.SecurityLogs.MakeLogEntry() seems to be the common theme for most of the submissions.
			//Spawn parallel threads to insert 200 security logs trying to get a duplicate entry exception.
			List<Action> listActions=new List<Action>();
			for(int i=0;i<200;i++) {
				listActions.Add(() => SecurityLogs.MakeLogEntry(Permissions.Accounting,patient.PatNum,"",0,DateTime.Now.AddDays(-7)));
			}
			//Parallel threads do not support Middle Tier mode when unit testing due to how we have to fake being both the client and the server.
			RemotingRole remotingRoleOld=RemotingClient.RemotingRole;
			if(RemotingClient.RemotingRole!=RemotingRole.ClientDirect) {
				RemotingClient.RemotingRole=RemotingRole.ClientDirect;
			}
			ODThread.RunParallel(listActions,onException: (ex) => {
				PrefT.UpdateBool(PrefName.RandomPrimaryKeys,false);
				RemotingClient.RemotingRole=remotingRoleOld;
				Assert.Fail(ex.Message);
			});
			PrefT.UpdateBool(PrefName.RandomPrimaryKeys,false);
			RemotingClient.RemotingRole=remotingRoleOld;
		}

	}
}
