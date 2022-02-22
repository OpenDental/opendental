using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CentralManager;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.CentralManager_Tests {
	[TestClass]
	public class CentralManagerTests:TestBase {
		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			CentralManagerT.ClearGroupPermissionTable();
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			CentralManagerT.ClearGroupPermissionTable();
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void CentralSyncHelper_SyncDisplayReportFKeysCEMT_OnlySyncKnownInternalReports() {
			#region Add DisplayReports to listDisplayReportsRemote
			//Act like the remote database doesn't know about any reports at all.
			List<DisplayReport> listDisplayReportsRemote=new List<DisplayReport>();
			#endregion
			#region Add DisplayReports to listDisplayReportsCEMT
			//Act like the CEMT database has a non-internal report to synchronize with the remote database.
			List<DisplayReport> listDisplayReportsCEMT=new List<DisplayReport>() {
				CentralManagerT.CreateDisplayReport(displayReportNum:1)
			};
			#endregion
			#region Add GroupPermissions to listGroupPermissionsCEMT
			//Make a group permission for the non-internal report and a random non-reports permission.
			List<GroupPermission> listGroupPermissionsCEMT=new List<GroupPermission>() {
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.Reports,userGroupNum:1),//Non-internal report
				CentralManagerT.CreateGroupPermission(fKey:2,permType:Permissions.AdjustmentTypeDeny,userGroupNum:1),//Unknown report
			};
			#endregion
			listGroupPermissionsCEMT=CentralSyncHelper.SyncDisplayReportFKeysCEMT(listGroupPermissionsCEMT,listDisplayReportsRemote,listDisplayReportsCEMT);
			//Assert that the non-internal report was removed so that it will not be synchronized with the remote database.
			//This is because the remote database may not know about the non-internal report and we cannot trust the FKey value.
			Assert.AreEqual(1,listGroupPermissionsCEMT.Count);
			Assert.IsNotNull(listGroupPermissionsCEMT.FirstOrDefault(x => x.PermType==Permissions.AdjustmentTypeDeny));
		}

		[TestMethod]
		public void CentralSyncHelper_SyncDisplayReportFKeysCEMT_RemoveAllReportPermsIfFKeyOfZeroLocated() {
			#region Add DisplayReports to listDisplayReportsRemote
			//Act like the remote database doesn't know about any reports at all.
			List<DisplayReport> listDisplayReportsRemote=new List<DisplayReport>();
			#endregion
			#region Add DisplayReports to listDisplayReportsCEMT
			//Act like the CEMT database has a non-internal report to synchronize with the remote database.
			List<DisplayReport> listDisplayReportsCEMT=new List<DisplayReport>() {
				CentralManagerT.CreateDisplayReport(displayReportNum:1)
			};
			#endregion
			#region Add GroupPermissions to listGroupPermissionsCEMT
			//Make several report group permissions and make sure at least one FKey has a value of 0.
			List<GroupPermission> listGroupPermissionsCEMT=new List<GroupPermission>() {
				CentralManagerT.CreateGroupPermission(fKey:0,permType:Permissions.Reports,userGroupNum:1),//All report permissions granted
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.Reports,userGroupNum:1),//Non-internal report
				CentralManagerT.CreateGroupPermission(fKey:2,permType:Permissions.Reports,userGroupNum:1),//Unknown report
			};
			#endregion
			listGroupPermissionsCEMT=CentralSyncHelper.SyncDisplayReportFKeysCEMT(listGroupPermissionsCEMT,listDisplayReportsRemote,listDisplayReportsCEMT);
			//Assert that the zero FKey report is honored since it grants the user group permission to all reports.
			Assert.AreEqual(1,listGroupPermissionsCEMT.Count);
			Assert.AreEqual(0,listGroupPermissionsCEMT.FirstOrDefault(x => x.PermType==Permissions.Reports).FKey);
		}

		[TestMethod]
		///<summary>"Concerning" report permissions are permissions that associate to an unknown or non-internal CEMT DisplayReport, or an internal CEMT DisplayReport that does not match any internal remote DisplayReport.</summary>
		public void CentralSyncHelper_SyncDisplayReportFKeysCEMT_RemoveConcerningReportPerms() {
			#region Add DisplayReports to listDisplayReportsRemote
			//Act like the remote database knows about the "foot" internal report.
			List<DisplayReport> listDisplayReportsRemote=new List<DisplayReport>() {
				CentralManagerT.CreateDisplayReport(displayReportNum:1,internalName:"foot")
			};
			#endregion
			#region Add DisplayReports to listDisplayReportsCEMT
			//Act like the CEMT database has a non-internal report and one internal report to synchronize with the remote database but the internal name doesn't match.
			List<DisplayReport> listDisplayReportsCEMT=new List<DisplayReport>() {
				CentralManagerT.CreateDisplayReport(displayReportNum:1),
				CentralManagerT.CreateDisplayReport(displayReportNum:2,internalName:"foo")
			};
			#endregion
			#region Add GroupPermissions to listGroupPermissionsCEMT
			//Make several report group permissions and make sure at least one permission exists for both display reports in the CEMT database.
			List<GroupPermission> listGroupPermissionsCEMT=new List<GroupPermission>() {
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.Reports,userGroupNum:1),//Non-internal report
				CentralManagerT.CreateGroupPermission(fKey:2,permType:Permissions.Reports,userGroupNum:1),//Valid report that is not linked to a remote report.
				CentralManagerT.CreateGroupPermission(fKey:3,permType:Permissions.Reports,userGroupNum:1) //Unknown report
			};
			#endregion
			listGroupPermissionsCEMT=CentralSyncHelper.SyncDisplayReportFKeysCEMT(listGroupPermissionsCEMT,listDisplayReportsRemote,listDisplayReportsCEMT);
			//Assert that no reports are synchronized because no matching internal named reports were found.
			Assert.AreEqual(0,listGroupPermissionsCEMT.Count);
		}

		[TestMethod]
		public void CentralSyncHelper_SyncDisplayReportFKeysCEMT_CorrectReportPermFKeys() {
			#region Add DisplayReports to listDisplayReportsRemote
			//Act like the remote database knows about the "foo" internal report but set a strange PK.
			List<DisplayReport> listDisplayReportsRemote=new List<DisplayReport>() {
				CentralManagerT.CreateDisplayReport(displayReportNum:15,internalName:"foo")
			};
			#endregion
			#region Add DisplayReports to listDisplayReportsCEMT
			//Act like the CEMT database also knows about the "foo" internal report but has a different PK than the remote database.
			List<DisplayReport> listDisplayReportsCEMT=new List<DisplayReport>() {
				CentralManagerT.CreateDisplayReport(displayReportNum:1,internalName:"foo")
			};
			#endregion
			#region Add GroupPermissions to listGroupPermissionsCEMT
			//Make a report group permission for the internal display report.
			List<GroupPermission> listGroupPermissionsCEMT=new List<GroupPermission>() {
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.Reports,userGroupNum:1),//Internal report
			};
			#endregion
			listGroupPermissionsCEMT=CentralSyncHelper.SyncDisplayReportFKeysCEMT(listGroupPermissionsCEMT,listDisplayReportsRemote,listDisplayReportsCEMT);
			//Assert that the permission was preserved and that the FKey was synchronized with the remote database.
			Assert.AreEqual(1,listGroupPermissionsCEMT.Count);
			Assert.AreEqual(listDisplayReportsRemote.First().DisplayReportNum,listGroupPermissionsCEMT.FirstOrDefault(x => x.PermType==Permissions.Reports).FKey);
		}

		[TestMethod]
		public void CentralSyncHelper_RemoveFKeysForPermissions_CorrectIfAll() {
			#region Add GroupPermissions to listGroupPermissionsCEMT
			List<GroupPermission> listGroupPermissionsCEMT=new List<GroupPermission>() {
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.AdjustmentTypeDeny,userGroupNum:1),
				CentralManagerT.CreateGroupPermission(fKey:2,permType:Permissions.AdjustmentTypeDeny,userGroupNum:1),
				CentralManagerT.CreateGroupPermission(fKey:0,permType:Permissions.AdjustmentTypeDeny,userGroupNum:1),
			};
			#endregion
			listGroupPermissionsCEMT=CentralSyncHelper.RemoveFKeysForPermissions(listGroupPermissionsCEMT,Permissions.AdjustmentTypeDeny);
			Assert.AreEqual(1,listGroupPermissionsCEMT.Count(x => x.PermType==Permissions.AdjustmentTypeDeny));
			Assert.IsNotNull(listGroupPermissionsCEMT.FirstOrDefault(x => x.PermType==Permissions.AdjustmentTypeDeny && x.FKey==0));
		}

		[TestMethod]
		public void CentralSyncHelper_RemoveFKeysForPermissions_EmptyForPermTypeIfNoFKeyOfZero() {
			#region Add GroupPermissions to listGroupPermissionsCEMT
			List<GroupPermission> listGroupPermissionsCEMT=new List<GroupPermission>() {
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.AdjustmentTypeDeny,userGroupNum:1),
				CentralManagerT.CreateGroupPermission(fKey:2,permType:Permissions.AdjustmentTypeDeny,userGroupNum:1),
			};
			#endregion
			listGroupPermissionsCEMT=CentralSyncHelper.RemoveFKeysForPermissions(listGroupPermissionsCEMT,Permissions.AdjustmentTypeDeny);
			Assert.AreEqual(0,listGroupPermissionsCEMT.Count);
		}

		[TestMethod]
		public void CentralSyncHelper_RemoveFKeysForPermissions_DoesNotRemoveForUnspecifiedPermType() {
			#region Add GroupPermissions to listGroupPermissionsCEMT
			List<GroupPermission> listGroupPermissionsCEMT=new List<GroupPermission>() {
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.AdjustmentTypeDeny,userGroupNum:1),
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.Reports,userGroupNum:1),
			};
			#endregion
			listGroupPermissionsCEMT=CentralSyncHelper.RemoveFKeysForPermissions(listGroupPermissionsCEMT,Permissions.AdjustmentTypeDeny);
			Assert.IsTrue(listGroupPermissionsCEMT.Count(x => x.PermType==Permissions.Reports)==1);//Did not remove anything from the permission we didn't specify.
			Assert.IsTrue(listGroupPermissionsCEMT.Count(x => x.PermType==Permissions.AdjustmentTypeDeny)==0);
		}

		[TestMethod]
		public void CentralGroupPermissions_Sync_ReplaceAllGroupPermissions() {
			//First, setup the test scenario.
			long userGroupNum1=UserGroupT.CreateUserGroup("UserGroup1");
			long userGroupNum2=UserGroupT.CreateUserGroup("UserGroup2");
			#region Add to listGroupPermissionsCEMT
			//Make group permissions for two non-internal reports in the CEMT database.
			List<GroupPermission> listGroupPermissionsCEMT=new List<GroupPermission>() {
				CentralManagerT.CreateGroupPermission(fKey:1,permType:Permissions.Reports,userGroupNum:userGroupNum1),
				CentralManagerT.CreateGroupPermission(fKey:3,permType:Permissions.Reports,userGroupNum:userGroupNum1),
			};
			#endregion
			#region Add to listGroupPermissionsRemote and insert into DB
			//Make group permissions for three non-internal reports spread between two user groups along with a random group permission in the remote database.
			//Actually insert these group permissions into the unit test database so that we can treat the unit test database as the remote database.
			List<GroupPermission> listGroupPermissionsRemote=new List<GroupPermission>() {
				CentralManagerT.InsertGroupPermissionNoCache(FKey:2,Permissions.Reports,userGroupNum:userGroupNum1),
				CentralManagerT.InsertGroupPermissionNoCache(FKey:3,Permissions.Reports,userGroupNum:userGroupNum1),
				CentralManagerT.InsertGroupPermissionNoCache(FKey:3,Permissions.AdjustmentTypeDeny,userGroupNum:userGroupNum1),
				CentralManagerT.InsertGroupPermissionNoCache(FKey:1,Permissions.Reports,userGroupNum:userGroupNum2),
			};
			#endregion
			#region Assert remote GroupPermissions prior to syncing
			//Assert that the unit test database looks exactly as we expect.
			List<GroupPermission> listGroupPermissionsUserGroupsRemote=GroupPermissions.GetPermsNoCache(userGroupNum1);
			listGroupPermissionsUserGroupsRemote.AddRange(GroupPermissions.GetPermsNoCache(userGroupNum2));
			Assert.AreEqual(listGroupPermissionsRemote.Count,listGroupPermissionsUserGroupsRemote.Count);
			foreach(GroupPermission gp in listGroupPermissionsUserGroupsRemote) {
				Assert.IsTrue(listGroupPermissionsRemote.Any(x => x.FKey==gp.FKey && x.PermType==gp.PermType && x.UserGroupNum==gp.UserGroupNum));
			}
			#endregion
			//Assert that the special sync method trusts both lists as if they have been vetted already.
			//The sync is special because it will ignore the differences between the PK values. 
			CentralGroupPermissions.Sync(listGroupPermissionsCEMT,listGroupPermissionsRemote);			
			#region Assert UserGroup1
			List<GroupPermission> listGroupPermissionsUserGroup1=GroupPermissions.GetPermsNoCache(userGroupNum1);
			Assert.AreEqual(listGroupPermissionsUserGroup1.Count,2);
			Assert.AreEqual(listGroupPermissionsUserGroup1.Count(x => x.FKey==1
			 && x.PermType==Permissions.Reports
			 && x.UserGroupNum==userGroupNum1),1);
			Assert.AreEqual(listGroupPermissionsUserGroup1.Count(x => x.FKey==3
			 && x.PermType==Permissions.Reports
			 && x.UserGroupNum==userGroupNum1),1);
			#endregion
			#region Assert UserGroup2
			List<GroupPermission> listGroupPermissionsUserGroup2=GroupPermissions.GetPermsNoCache(userGroupNum2);
			Assert.AreEqual(listGroupPermissionsUserGroup2.Count,0);//Did not keep groupPermissions from remote list.
			#endregion
		}
	}
}
