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
			GroupPermissionT.ClearGroupPermissionTable();
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
			GroupPermissionT.ClearGroupPermissionTable();
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

		///<summary>Verifies that a usergroup with an AdjustmentTypeDeny permission with an FKey of 0, does not have access to any adjtype.</summary>
		[TestMethod]
		public void Permissions_Adjustment_HasZeroFKeyAdjustmentTypeDeny() {
			GroupPermissionT.ClearGroupPermissionTable();
			GroupPermission gp=new GroupPermission();
			gp.PermType=Permissions.AdjustmentTypeDeny;
			gp.UserGroupNum=_userGroup.UserGroupNum;
			gp.FKey=0;
			GroupPermissions.Insert(gp);
			GroupPermissions.RefreshCache();
			Security.CurUser=_user;
			DefT.DeleteAllForCategory(DefCat.AdjTypes);
			DefT.CreateDefinition(DefCat.AdjTypes,"Family Discount","-");
			DefT.CreateDefinition(DefCat.AdjTypes,"Sales Tax","+");
			DefT.CreateDefinition(DefCat.AdjTypes,"Late Charge","+");
			DefT.CreateDefinition(DefCat.AdjTypes,"New Customer Discount","-");
			Assert.AreEqual(0,Defs.GetPositiveAdjTypes(considerPermission:true).Count);
			Assert.AreEqual(0,Defs.GetNegativeAdjTypes(considerPermission:true).Count);
		}

		///<summary>Verifies that if the usergroup does not have an AdjustmentTypeDeny perm with an Fkey of 0, then the usergroup is only denied whichever adjtypes they do have
		///an AdjustmentTypeDeny perm for.</summary>
		[TestMethod]
		public void Permissions_Adjustments_NoZeroFKeyAdjustmentTypeDeny() {
			GroupPermissionT.ClearGroupPermissionTable();
			DefT.DeleteAllForCategory(DefCat.AdjTypes);
			Def defAdjSalesTax=DefT.CreateDefinition(DefCat.AdjTypes,"Sales Tax","+");
			Def defAdjDiscount=DefT.CreateDefinition(DefCat.AdjTypes,"Discount","-");
			GroupPermission gp=new GroupPermission();
			gp.PermType=Permissions.AdjustmentTypeDeny;
			gp.UserGroupNum=_userGroup.UserGroupNum;
			gp.FKey=defAdjSalesTax.DefNum;
			GroupPermissions.Insert(gp);
			GroupPermissions.RefreshCache();
			Security.CurUser=_user;
			Assert.IsFalse(GroupPermissions.HasPermissionForAdjType(defAdjSalesTax));
			Assert.IsTrue(GroupPermissions.HasPermissionForAdjType(defAdjDiscount));
		}

		///<summary>Verifies that clicking 'Set All' in the security tree deletes all AdjustmentTypeDeny perms for the usergroup, resulting in the usergroup having access to every adjtype.</summary>
		[TestMethod]
		public void Permissions_Adjustments_SecurityTreeSetAllAdjustmentTypeDeny() {
			GroupPermissionT.ClearGroupPermissionTable();
			GroupPermissions.RefreshCache();
			OpenDental.UserControlSecurityTree userControlSecurityTree=new OpenDental.UserControlSecurityTree();
			userControlSecurityTree.FillForUserGroup(_userGroup.UserGroupNum);
			DefT.DeleteAllForCategory(DefCat.AdjTypes);
			DefT.CreateDefinition(DefCat.AdjTypes,"Family Discount","-");
			DefT.CreateDefinition(DefCat.AdjTypes,"Sales Tax","+");
			DefT.CreateDefinition(DefCat.AdjTypes,"Late Charge","+");
			DefT.CreateDefinition(DefCat.AdjTypes,"New Customer Discount","-");
			Security.CurUser=_user;
			userControlSecurityTree.SetAll();
			Assert.AreEqual(0,GroupPermissions.GetAdjustmentTypeDenyPermsForUserGroup(_userGroup.UserGroupNum).Count);
			Assert.AreEqual(2,Defs.GetPositiveAdjTypes(considerPermission:true).Count);
			Assert.AreEqual(2,Defs.GetNegativeAdjTypes(considerPermission:true).Count);
		}

		///<summary>Verifies that clicking 'Set None' in the security tree inserts an AdjustmentTypeDeny perm for the usergroup with an FKey of 0, resulting in the usergroup not having 
		///access to any adjtype.</summary>
		[TestMethod]
		public void Permissions_Adjustments_SecurityTreeSetNoneAdjustmentTypeDeny() {
			GroupPermissionT.ClearGroupPermissionTable();
			GroupPermissions.RefreshCache();
			OpenDental.UserControlSecurityTree userControlSecurityTree=new OpenDental.UserControlSecurityTree();
			userControlSecurityTree.FillForUserGroup(_userGroup.UserGroupNum);
			DefT.DeleteAllForCategory(DefCat.AdjTypes);
			DefT.CreateDefinition(DefCat.AdjTypes,"Family Discount","-");
			DefT.CreateDefinition(DefCat.AdjTypes,"Sales Tax","+");
			DefT.CreateDefinition(DefCat.AdjTypes,"Late Charge","+");
			DefT.CreateDefinition(DefCat.AdjTypes,"New Customer Discount","-");
			Security.CurUser=_user;
			userControlSecurityTree.SetNone();
			List<GroupPermission> listGroupPermissions=GroupPermissions.GetAdjustmentTypeDenyPermsForUserGroup(_userGroup.UserGroupNum);
			Assert.AreEqual(1,listGroupPermissions.Count);
			Assert.AreEqual(0,listGroupPermissions[0].FKey);
			Assert.AreEqual(0,Defs.GetPositiveAdjTypes(considerPermission:true).Count);
			Assert.AreEqual(0,Defs.GetNegativeAdjTypes(considerPermission:true).Count);
		}
	}
}
