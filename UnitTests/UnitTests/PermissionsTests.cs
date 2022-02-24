using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Permissions_Tests {
	[TestClass]
	public class PermissionsTests:TestBase {
		private static UserGroup _userGroup;
		private static Userod _user;
		public static void CreateAndAttachUsers() {
			_userGroup=new UserGroup();
			UserGroups.Insert(_userGroup);
			UserGroups.RefreshCache();
			_user=UserodT.CreateUser();
			UserGroupAttaches.AddForUser(_user,_userGroup.UserGroupNum);
			UserGroupAttaches.RefreshCache();
		}
		[ClassInitialize]
		public static void ClassSetUp(TestContext testContext) {
			CreateAndAttachUsers();
		}

		[ClassCleanup]
		public static void ClassTearDown() {
			List<UserGroupAttach> listAttaches=UserGroupAttaches.GetForUserGroup(_userGroup.UserGroupNum);
			foreach(UserGroupAttach attach in listAttaches) {
				UserGroupAttaches.Delete(attach);
			}
			List<UserGroup> listGroups=UserGroups.GetList();
			foreach(UserGroup userGroup in listGroups) {
				UserGroups.Delete(userGroup);
			}
		}

		[TestMethod]
		public void Permissions_Procedures_HasProcDeletePermission() {
			GroupPermission gp=new GroupPermission();
			gp.PermType=Permissions.ProcDelete;
			gp.UserGroupNum=_userGroup.UserGroupNum;
			gp.NewerDate=new DateTime(2020,11,11);
			GroupPermissions.Insert(gp);
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.TP,"",0,procDate:new DateTime(2019,1,1));
			proc.DateEntryC=new DateTime(2020,11,12);
			DateTime dateForPerm=Procedures.GetDateForPermCheck(proc);
			Assert.IsTrue(Security.IsAuthorized(Permissions.ProcDelete,dateForPerm,true,true,_user,proc.CodeNum,proc.ProcFee,0,0));
		}
	}
}
