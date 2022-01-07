using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Windows.Forms;
using CodeBase;
using OpenDental;
using OpenDentBusiness;

namespace CentralManager {
	public partial class FormCentralSecurity:Form {
		public List<CentralConnection> ListConns;
		private string _domainObjectGuid;

		public FormCentralSecurity() {
			ListConns=new List<CentralConnection>();
			InitializeComponent();
		}

		private void FormCentralSecurity_Load(object sender,EventArgs e) {
			#region Load Global Settings
			textSyncCode.Text=PrefC.GetString(PrefName.CentralManagerSyncCode);
			checkEnable.Checked=PrefC.GetBool(PrefName.CentralManagerSecurityLock);
			checkAdmin.Checked=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
			checkDomainLoginEnabled.Checked=PrefC.GetBool(PrefName.DomainLoginEnabled);
			textDomainLoginPath.ReadOnly=!checkDomainLoginEnabled.Checked;
			textDomainLoginPath.Text=PrefC.GetString(PrefName.DomainLoginPath);
			if(PrefC.GetDate(PrefName.SecurityLockDate).Year>1880) {
				textDate.Text=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			}
			if(PrefC.GetInt(PrefName.SecurityLockDays)>0) {
				textDays.Text=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			}
			#endregion
		}

		#region Global Variable Methods
		private void checkDomainLoginEnabled_CheckedChanged(object sender,EventArgs e) {
			textDomainLoginPath.ReadOnly=!checkDomainLoginEnabled.Checked;
		}

		private void checkDomainLoginEnabled_MouseUp(object sender,MouseEventArgs e) {
			if(checkDomainLoginEnabled.Checked && string.IsNullOrWhiteSpace(textDomainLoginPath.Text)) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to use your current domain as the domain login path?")) {
					try {
						DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
						string defaultNamingContext = rootDSE.Properties["defaultNamingContext"].Value.ToString();
						textDomainLoginPath.Text="LDAP://"+defaultNamingContext;
						DirectoryEntry testEntry=new DirectoryEntry(textDomainLoginPath.Text);
						_domainObjectGuid=testEntry.Guid.ToString();
					}
					catch(Exception ex) {
						FriendlyException.Show(Lan.g(this,"Unable to bind to the current domain."),ex);
					}
				}
			}
		}

		private void textDomainLoginPath_Leave(object sender,EventArgs e) {
			if(checkDomainLoginEnabled.Checked) {
				if(string.IsNullOrWhiteSpace(textDomainLoginPath.Text)) {
					MsgBox.Show(this,"Warning. Domain Login is enabled, but no path has been entered. If you do not provide a domain path,"
						+"you will not be able to assign domain logins to users.");
					_domainObjectGuid="";
				}
				else {
					try {
						DirectoryEntry testEntry = new DirectoryEntry(textDomainLoginPath.Text);
						DirectorySearcher search = new DirectorySearcher(testEntry);
						SearchResultCollection testResults = search.FindAll(); //Just do a generic search to verify the object might have users on it
						_domainObjectGuid=testEntry.Guid.ToString();
					}
					catch(Exception ex) {
						FriendlyException.Show(Lan.g(this,"An error occurred while attempting to access the provided Domain Login Path."),ex);
					}
				}
			}
		}

		private void textDate_KeyDown(object sender,KeyEventArgs e) {
			textDays.Text="";
		}

		private void textDays_KeyDown(object sender,KeyEventArgs e) {
			textDate.Text="";
			textDate.Validate();
		}

		///<summary>Updates the local Lock preferences with form details. </summary>
		private bool UpdateLockPreferences() {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix error first.");
				return false;
			}
			int days=PIn.Int(textDays.Text);
			DateTime date=PIn.Date(textDate.Text);
			Prefs.UpdateString(PrefName.SecurityLockDate,POut.Date(date,false));
			Prefs.UpdateInt(PrefName.SecurityLockDays,days);
			Prefs.UpdateBool(PrefName.SecurityLockIncludesAdmin,checkAdmin.Checked);
			Prefs.UpdateBool(PrefName.CentralManagerSecurityLock,checkEnable.Checked);
			return true;
		}
		#endregion

		#region Sync Methods
		private void butPushBoth_Click(object sender,EventArgs e) {
			if(!UpdateLockPreferences()) {
				return;
			}
			using FormCentralConnections FormCC=new FormCentralConnections();
			FormCC.IsSelectionMode=true;
			FormCC.ShowDialog();
			if(FormCC.DialogResult!=DialogResult.OK){
				return;
			}
			using CodeBase.MsgBoxCopyPaste MsgBoxCopyPaste=new CodeBase.MsgBoxCopyPaste(CentralSyncHelper.PushBoth(FormCC.ListConnsSelected));
			MsgBoxCopyPaste.ShowDialog();
		}

		private void butPushUsers_Click(object sender,EventArgs e) {
			using FormCentralConnections FormCC=new FormCentralConnections();
			FormCC.IsSelectionMode=true;
			FormCC.ShowDialog();
			if(FormCC.DialogResult!=DialogResult.OK){
				return;
			}
			using CodeBase.MsgBoxCopyPaste MsgBoxCopyPaste=new CodeBase.MsgBoxCopyPaste(CentralSyncHelper.PushUsers(FormCC.ListConnsSelected));
			MsgBoxCopyPaste.ShowDialog();			
		}

		private void butPushLocks_Click(object sender,EventArgs e) {
			if(!UpdateLockPreferences()) {
				return;
			}
			using FormCentralConnections FormCC=new FormCentralConnections();
			FormCC.IsSelectionMode=true;
			FormCC.ShowDialog();
			if(FormCC.DialogResult!=DialogResult.OK){
				return;
			}
			using CodeBase.MsgBoxCopyPaste MsgBoxCopyPaste=new CodeBase.MsgBoxCopyPaste(CentralSyncHelper.PushLocks(FormCC.ListConnsSelected));
			MsgBoxCopyPaste.ShowDialog();
		}
		#endregion

		///<summary>Add user button.</summary>
		private void userControlSecurityTabs_AddUserClick(object sender,SecurityEventArgs e) {
			Userod user = new Userod();
			user.IsNew=true;
			using FormCentralUserEdit FormCU = new FormCentralUserEdit(user);
			FormCU.ShowDialog();
			if(FormCU.DialogResult == DialogResult.OK) {//update to reflect changes that were made in FormUserEdit.
				userControlSecurityTabs.FillGridUsers();//New user is not in grid yet, add them.
				userControlSecurityTabs.SelectedUser=FormCU.UserCur;//Selects the user that was just added in the grid.
				userControlSecurityTabs.RefreshUserTabGroups();//Previously selected users User Groups are still selected, refresh for UserCur.
			}
		}

		///<summary>Edit user button.</summary>
		private void userControlSecurityTabs_EditUserClick(object sender,SecurityEventArgs e) {
			using FormCentralUserEdit FormCUE = new FormCentralUserEdit(e.User);
			FormCUE.ShowDialog();
			if(FormCUE.DialogResult == DialogResult.OK) {
				userControlSecurityTabs.FillGridUsers();
				userControlSecurityTabs.RefreshUserTabGroups();
			}
		}

		///<summary>Add user group button.</summary>
		private void userControlSecurityTabs_AddUserGroupClick(object sender,SecurityEventArgs e) {
			UserGroup group = new UserGroup();
			group.IsNew=true;
			using FormCentralUserGroupEdit FormU = new FormCentralUserGroupEdit(group);
			FormU.ShowDialog();
			if(FormU.DialogResult == DialogResult.OK) {
				userControlSecurityTabs.FillListUserGroupTabUserGroups();
				userControlSecurityTabs.SelectedUserGroup=group;
			}
		}

		///<summary>Edit user group button.</summary>
		private void userControlSecurityTabs_EditUserGroupClick(object sender,SecurityEventArgs e) {
			using FormCentralUserGroupEdit FormU = new FormCentralUserGroupEdit(e.Group);
			FormU.ShowDialog();
			if(FormU.DialogResult==DialogResult.OK) {
				userControlSecurityTabs.FillListUserGroupTabUserGroups();
			}
		}

		private DialogResult userControlSecurityTabs_ReportPermissionChecked(object sender,SecurityEventArgs e) {
			GroupPermission perm = e.Perm;
			using FormCentralReportSetup FormCRS = new FormCentralReportSetup(perm.UserGroupNum,true);
			FormCRS.ShowDialog();
			if(FormCRS.DialogResult==DialogResult.Cancel) {
				return FormCRS.DialogResult;
			}
			if(!FormCRS.HasReportPerms) {//Only insert base Reports permission if the user actually has any reports allowed
				return FormCRS.DialogResult;
			}
			try {
				GroupPermissions.Insert(perm);
			}
			catch(Exception ex) {
				OpenDental.MessageBox.Show(ex.Message);
				return DialogResult.Cancel;
			}
			return FormCRS.DialogResult;
		}

		private DialogResult userControlSecurityTabs_GroupPermissionChecked(object sender,SecurityEventArgs e) {
			using FormCentralGroupPermEdit FormCG = new FormCentralGroupPermEdit(e.Perm);
			FormCG.ShowDialog();
			return FormCG.DialogResult;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!UpdateLockPreferences()) {
				return;
			}
			Prefs.UpdateString(PrefName.DomainLoginPath,PIn.String(textDomainLoginPath.Text));
			Prefs.UpdateBool(PrefName.DomainLoginEnabled,checkDomainLoginEnabled.Checked);
			if(_domainObjectGuid!=null) {
				Prefs.UpdateString(PrefName.DomainObjectGuid,_domainObjectGuid);
			}
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
