using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Linq;

namespace OpenDental {
	public partial class FormSecurity:FormODBase {

		public FormSecurity() {
			InitializeComponent();
			InitializeLayoutManager();
			Font=LayoutManagerForms.FontInitial;
			Lan.F(this);
		}
		
		private void FormSecurityEdit_Load(object sender,EventArgs e) {
			LayoutMenu();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Global Security Settings",globalSecuritySettingsToolStripMenuItem_Click));
			if(Userods.HasUsersForCEMTNoCache()) {
				menuMain.Add(new MenuItemOD("CEMT Users",CEMTUsersToolStripMenuItem_Click));
			}
			menuMain.EndUpdate();
		}

		private void globalSecuritySettingsToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormGlobalSecurity formGlobalSecurity = new FormGlobalSecurity();
			formGlobalSecurity.ShowDialog();//no refresh needed; settings changed in FormGlobalSecurity have no bearing on what displays in this form.
		}

		private void CEMTUsersToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormSecurityCentralUserEdit formSecurityCentralUserEdit = new FormSecurityCentralUserEdit();
			formSecurityCentralUserEdit.ShowDialog();//no refresh needed; settings changed in FormGlobalSecurity have no bearing on what displays in this form.
		}

		private void userControlSecurityTabs_AddUserClick(object sender,SecurityEventArgs e) {
			Userod userod = new Userod();
			using FormUserEdit formUserEdit = new FormUserEdit(userod);
			formUserEdit.IsNew=true;
			formUserEdit.ShowDialog();
			if(formUserEdit.DialogResult == DialogResult.OK) {//update to reflect changes that were made in FormUserEdit.
				userControlSecurityUserGroup.FillGridUsers();//New user is not in grid yet, add them.
				userControlSecurityUserGroup.SelectedUser=formUserEdit.UserodCur;//Selects the user that was just added in the grid.
				userControlSecurityUserGroup.RefreshUserTabGroups();//Previously selected users User Groups are still selected, refresh for UserCur.
			}
		}		

		private void UserControlSecurityTabs_CopyUserClick(object sender,SecurityEventArgs e) {
			//validation is handled here in GetUniqueUsername(...) 
			Userod userod=e.User;
			if (userod is null) {
				MsgBox.Show(Lan.g(this,"Please select a user."));
				return;
			}		
			if(!Userods.TryGetUniqueUsername(userod.UserName+"(Copy)",0,false,false,out string newUserName)){//This should really never fail.
				MsgBox.Show(this,"Could not generate a unique username.");
				return;
			}
			//New username format; user.UserName(copy)(X)
			using FormUserPassword formUserPassword=new FormUserPassword(false,newUserName,isCopiedUser:true);
			formUserPassword.IsInSecurityWindow=true;//Do not show or validate current password UI since this is a new user.
			if(formUserPassword.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Userod userodNew=Userods.CopyUser(userod,formUserPassword.PasswordContainer_,formUserPassword.IsPasswordStrong,newUserName);
			DataValid.SetInvalid(InvalidType.Security,InvalidType.UserClinics);//Must be called after Userods.CopyUser(...)
			userControlSecurityUserGroup.FillGridUsers();
			userControlSecurityUserGroup.SelectedUser=userodNew;
			userControlSecurityUserGroup.RefreshUserTabGroups();
		}

		private void userControlSecurityTabs_EditUserClick(object sender,SecurityEventArgs e) {
			using FormUserEdit formUserEdit = new FormUserEdit(e.User);
			formUserEdit.ShowDialog();
			if(formUserEdit.DialogResult == DialogResult.OK) {//update to reflect changes that were made in FormUserEdit.
				userControlSecurityUserGroup.FillGridUsers();
				userControlSecurityUserGroup.RefreshUserTabGroups();
			}
		}

		private void userControlSecurityTabs_AddUserGroupClick(object sender,SecurityEventArgs e) {
			UserGroup userGroup = new UserGroup();
			FrmUserGroupEdit frmUserGroupEdit = new FrmUserGroupEdit(userGroup);
			frmUserGroupEdit.IsNew=true;
			frmUserGroupEdit.ShowDialog();
			if(frmUserGroupEdit.IsDialogOK) {
				userControlSecurityUserGroup.FillListUserGroupTabUserGroups();//update to reflect changes that were made in FormUserGroupEdit.
				userControlSecurityUserGroup.SelectedUserGroup=userGroup;
			}
		}

		private void userControlSecurityTabs_EditUserGroupClick(object sender,SecurityEventArgs e) {
			FrmUserGroupEdit frmUserGroupEdit = new FrmUserGroupEdit(e.Group);
			frmUserGroupEdit.ShowDialog();
			if(frmUserGroupEdit.IsDialogOK) {
				userControlSecurityUserGroup.FillListUserGroupTabUserGroups();
			}
		}

		private DialogResult userControlSecurityTabs_ReportPermissionChecked(object sender,SecurityEventArgs e) {
			GroupPermission groupPermission = e.Perm;
			using FormReportSetup formReportSetup = new FormReportSetup(groupPermission.UserGroupNum,true);
			formReportSetup.ShowDialog();//FormReportSetup will handle all add/deleting report permissions, including FKey of 0.
			return formReportSetup.DialogResult;
		}

		private DialogResult userControlSecurityTabs_GroupPermissionChecked(object sender,SecurityEventArgs e) {
			using FormGroupPermEdit formGroupPermEdit = new FormGroupPermEdit(e.Perm);
			formGroupPermEdit.IsNew=true;
			formGroupPermEdit.ShowDialog();
			return formGroupPermEdit.DialogResult;
		}

		private DialogResult userControlSecurityTabs_AdjustmentTypeDenyPermissionChecked(object sender,SecurityEventArgs e) {
			List<GroupPermission> listGroupPermissionsOld=GroupPermissions.GetAdjustmentTypeDenyPermsForUserGroup(e.Perm.UserGroupNum);
			List<Def> listDefsAll=Defs.GetDefsForCategory(DefCat.AdjTypes);
			List<Def> listDefs=Defs.GetDefs(DefCat.AdjTypes,listGroupPermissionsOld.Select(x => x.FKey).ToList());
			if(listGroupPermissionsOld.Any(x => x.FKey==0)) {//All individual permissions.
				listDefs=listDefsAll.Select(x => x.Copy()).ToList();
			}
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.AdjTypes,listDefs);
			formDefinitionPicker.IsMultiSelectionMode=true;
			formDefinitionPicker.HasShowHiddenOption=true;
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult!=DialogResult.OK) {
				return DialogResult.Cancel;
			}
			List<GroupPermission> listGroupPermissionsNew=new List<GroupPermission>();
			GroupPermission groupPermission=null;
			List<Def> listDefsSelected=formDefinitionPicker.ListDefsSelected;
			if(listDefsSelected.Count==listDefsAll.Count) {//Selected all individual permissions.
				groupPermission=new GroupPermission();
				groupPermission.UserGroupNum=e.Perm.UserGroupNum;
				groupPermission.PermType=EnumPermType.AdjustmentTypeDeny;
				groupPermission.FKey=0;
				listGroupPermissionsNew.Add(groupPermission);
				GroupPermissions.Sync(listGroupPermissionsNew,listGroupPermissionsOld);
				return DialogResult.OK;
			}
			//Selected none or some individual permissions.
			for(int i=0;i<listDefsSelected.Count;i++) {
				groupPermission=listGroupPermissionsOld.Find(x => x.FKey==listDefsSelected[i].DefNum);
				if(groupPermission==null) { 
					groupPermission=new GroupPermission();
					groupPermission.UserGroupNum=e.Perm.UserGroupNum;
					groupPermission.PermType=EnumPermType.AdjustmentTypeDeny;
					groupPermission.FKey=listDefsSelected[i].DefNum;
				}
				listGroupPermissionsNew.Add(groupPermission);
			}
			GroupPermissions.Sync(listGroupPermissionsNew,listGroupPermissionsOld);
			return DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormSecurityEdit_FormClosing(object sender,FormClosingEventArgs e) {
			DataValid.SetInvalid(InvalidType.Security);
		}
	}
}