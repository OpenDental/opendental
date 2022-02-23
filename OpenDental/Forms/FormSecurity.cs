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
			using FormGlobalSecurity FormGS = new FormGlobalSecurity();
			FormGS.ShowDialog();//no refresh needed; settings changed in FormGlobalSecurity have no bearing on what displays in this form.
		}

		private void CEMTUsersToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormSecurityCentralUserEdit FormCEMTUserEdit = new FormSecurityCentralUserEdit();
			FormCEMTUserEdit.ShowDialog();//no refresh needed; settings changed in FormGlobalSecurity have no bearing on what displays in this form.
		}

		private void userControlSecurityTabs_AddUserClick(object sender,SecurityEventArgs e) {
			Userod user = new Userod();
			using FormUserEdit FormU = new FormUserEdit(user);
			FormU.IsNew=true;
			FormU.ShowDialog();
			if(FormU.DialogResult == DialogResult.OK) {//update to reflect changes that were made in FormUserEdit.
				userControlSecurityUserGroup.FillGridUsers();//New user is not in grid yet, add them.
				userControlSecurityUserGroup.SelectedUser=FormU.UserCur;//Selects the user that was just added in the grid.
				userControlSecurityUserGroup.RefreshUserTabGroups();//Previously selected users User Groups are still selected, refresh for UserCur.
			}
		}		

		private void UserControlSecurityTabs_CopyUserClick(object sender,SecurityEventArgs e) {
			//validation is handled here in GetUniqueUsername(...) 
			Userod user=e.User;
			if (user is null) {
				MsgBox.Show(Lan.g(this,"Please select a user."));
				return;
			}		
			if(!Userods.TryGetUniqueUsername(user.UserName+"(Copy)",0,false,false,out string newUserName)){//This should really never fail.
				MsgBox.Show(this,"Could not generate a unique username.");
				return;
			}
			//New username format; user.UserName(copy)(X)
			using FormUserPassword formPassword=new FormUserPassword(false,newUserName,isCopiedUser:true);
			formPassword.IsInSecurityWindow=true;//Do not show or validate current password UI since this is a new user.
			if(formPassword.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Userod newUser=Userods.CopyUser(user,formPassword.LoginDetails,formPassword.PasswordIsStrong,newUserName);
			DataValid.SetInvalid(InvalidType.Security,InvalidType.UserClinics);//Must be called after Userods.CopyUser(...)
			userControlSecurityUserGroup.FillGridUsers();
			userControlSecurityUserGroup.SelectedUser=newUser;
			userControlSecurityUserGroup.RefreshUserTabGroups();
		}

		private void userControlSecurityTabs_EditUserClick(object sender,SecurityEventArgs e) {
			using FormUserEdit FormUE = new FormUserEdit(e.User);
			FormUE.ShowDialog();
			if(FormUE.DialogResult == DialogResult.OK) {//update to reflect changes that were made in FormUserEdit.
				userControlSecurityUserGroup.FillGridUsers();
				userControlSecurityUserGroup.RefreshUserTabGroups();
			}
		}

		private void userControlSecurityTabs_AddUserGroupClick(object sender,SecurityEventArgs e) {
			UserGroup group = new UserGroup();
			using FormUserGroupEdit FormU = new FormUserGroupEdit(group);
			FormU.IsNew=true;
			FormU.ShowDialog();
			if(FormU.DialogResult == DialogResult.OK) {
				userControlSecurityUserGroup.FillListUserGroupTabUserGroups();//update to reflect changes that were made in FormUserGroupEdit.
				userControlSecurityUserGroup.SelectedUserGroup=group;
			}
		}

		private void userControlSecurityTabs_EditUserGroupClick(object sender,SecurityEventArgs e) {
			using FormUserGroupEdit FormU = new FormUserGroupEdit(e.Group);
			FormU.ShowDialog();
			if(FormU.DialogResult == DialogResult.OK) {
				userControlSecurityUserGroup.FillListUserGroupTabUserGroups();
			}
		}

		private DialogResult userControlSecurityTabs_ReportPermissionChecked(object sender,SecurityEventArgs e) {
			GroupPermission perm = e.Perm;
			using FormReportSetup FormRS = new FormReportSetup(perm.UserGroupNum,true);
			FormRS.ShowDialog();//FormReportSetup will handle all add/deleting report permissions, including FKey of 0.
			return FormRS.DialogResult;
		}

		private DialogResult userControlSecurityTabs_GroupPermissionChecked(object sender,SecurityEventArgs e) {
			using FormGroupPermEdit FormG = new FormGroupPermEdit(e.Perm);
			FormG.IsNew=true;
			FormG.ShowDialog();
			return FormG.DialogResult;
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
				groupPermission.PermType=Permissions.AdjustmentTypeDeny;
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
					groupPermission.PermType=Permissions.AdjustmentTypeDeny;
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