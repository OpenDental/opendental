using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental;
using OpenDentBusiness;

namespace CentralManager {
	public partial class FormCentralUserEdit:Form {
		public Userod UserCur;
		private List<AlertSub> _listAlertSubsOld;
		private bool _isFillingList;
		///<summary>The password that was entered in FormCentralUserPasswordEdit.</summary>
		private string _passwordTyped;

		public FormCentralUserEdit(Userod user) {
			InitializeComponent();
			UserCur=user.Copy();
		}		

		private void FormCentralUserEdit_Load(object sender,EventArgs e) {
			checkIsHidden.Checked=UserCur.IsHidden;
			textUserName.Text=UserCur.UserName;
			List<UserGroup> listUserGroups=UserGroups.GetDeepCopy();
			_isFillingList=true;
			for(int i = 0;i < listUserGroups.Count;i++) {
				UserGroup groupCur=listUserGroups[i];
				listUserGroup.Items.Add(groupCur.Description,groupCur);
				if(UserCur.IsInUserGroup(groupCur.UserGroupNum)){
					listUserGroup.SetSelected(i,true);
				}
			}
			if(listUserGroup.SelectedIndices.Count==0){//never allowed to delete last group, so this won't fail
				listUserGroup.SelectedIndex=0;
			}
			_isFillingList=false;
			securityTreeUser.FillTreePermissionsInitial();
			RefreshUserTree();
			if(UserCur.PasswordHash==""){
				butPassword.Text="Create Password";
			}
			_listAlertSubsOld=AlertSubs.GetAllForUser(Security.CurUser.UserNum);
			listAlertSubMulti.Items.Clear();
			string[] arrayAlertTypes=Enum.GetNames(typeof(AlertType));
			for(int i=0;i<arrayAlertTypes.Length;i++){
				listAlertSubMulti.Items.Add(arrayAlertTypes[i]);
				listAlertSubMulti.SetSelected(i,_listAlertSubsOld.Exists(x => x.Type==(AlertType)i));
			}
			if(UserCur.IsNew) {
				butUnlock.Visible=false;
			}
			if(!string.IsNullOrEmpty(UserCur.DomainUser) && UserCur.DomainUser.Split('\\').Length>1) {
				textDomainUser.Text=UserCur.DomainUser.Split('\\')[1];
			}
			if(!PrefC.GetBool(PrefName.DomainLoginEnabled)) {
				labelDomainUser.Visible=false;
				textDomainUser.Visible=false;
				butPickDomainUser.Visible=false;
			}
		}

		///<summary>Refreshes the security tree in the "Users" tab.</summary>
		private void RefreshUserTree() {
			securityTreeUser.FillForUserGroup(listUserGroup.GetListSelected<UserGroup>().Select(x => x.UserGroupNum).ToList());
		}

		private void listUserGroup_SelectedIndexChanged(object sender,EventArgs e) {
			if(_isFillingList) {
				return;
			}
			RefreshUserTree();
		}

		private void butPickDomainUser_Click(object sender,EventArgs e) {
			//DirectoryEntry does recognize an empty string as a valid LDAP entry and will just return all logins from all available domains
			//But all logins should be on the same domain, so this field is required
			if(string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.DomainLoginPath))) {
				MsgBox.Show(this,"DomainLoginPath is missing in security settings. DomainLoginPath is required before assigning domain logins to user accounts.");
				return;
			}
			//Try to access the specified DomainLoginPath
			try {
				DirectoryEntry.Exists(PrefC.GetString(PrefName.DomainLoginPath));
			}
			catch(Exception ex) {
				OpenDental.MessageBox.Show(Lan.g(this,"An error occurred while attempting to access the provided DomainLoginPath:")+" "+ex.Message);
				return;
			}
			using FormDomainUserPick FormDU=new FormDomainUserPick();
			if(FormDU.ShowDialog()==DialogResult.OK && FormDU.SelectedDomainName!=null) { //only check for null, as empty string should clear the field
				UserCur.DomainUser=$@"{PrefC.GetString(PrefName.DomainObjectGuid)}\{FormDU.SelectedDomainName}"; 
				textDomainUser.Text=FormDU.SelectedDomainName;//only display username
			}
		}

		private void butPassword_Click(object sender,EventArgs e) {
			bool isCreate=false;
			if(string.IsNullOrEmpty(UserCur.PasswordHash)) {
				isCreate=true;
			}
			using FormCentralUserPasswordEdit FormCPE=new FormCentralUserPasswordEdit(isCreate,UserCur.UserName);
			FormCPE.IsInSecurityWindow=true;
			FormCPE.ShowDialog();
			if(FormCPE.DialogResult==DialogResult.Cancel){
				return;
			}
			UserCur.LoginDetails=FormCPE.LoginDetails;
			_passwordTyped=FormCPE.PasswordTyped;
			if(UserCur.PasswordHash==""){
				butPassword.Text="Create Password";
			}
			else{
				butPassword.Text="Change Password";
			}
		}

		private void butUnlock_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Users can become locked when invalid credentials have been entered several times in a row.\r\n"
				+"Unlock this user so that more log in attempts can be made?")) 
			{
				return;
			}
			UserCur.DateTFail=DateTime.MinValue;
			UserCur.FailedAttempts=0;
			try {
				Userods.Update(UserCur);//This will also commit other things about the user if they've changed.  Oh well.
				MsgBox.Show(this,"User has been unlocked.");
			}
			catch(Exception) {
				MsgBox.Show(this,"There was a problem unlocking this user.  Please call support or wait the allotted lock time.");
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textUserName.Text=="") {
				OpenDental.MessageBox.Show(this,"Please enter a username.");
				return;
			}
			if(listUserGroup.SelectedIndices.Count==0) {
				OpenDental.MessageBox.Show(this,"Every user must be associated to at least one User Group.");
				return;
			}
			List<AlertSub> listAlertSubsCur=new List<AlertSub>();
			foreach(int index in listAlertSubMulti.SelectedIndices) {
				AlertSub alertSub=new AlertSub();
				alertSub.ClinicNum=0;
				alertSub.UserNum=Security.CurUser.UserNum;
				alertSub.Type=(AlertType)index;
				listAlertSubsCur.Add(alertSub);
			}
			AlertSubs.Sync(listAlertSubsCur,_listAlertSubsOld);
			UserCur.IsHidden=checkIsHidden.Checked;
			UserCur.UserName=textUserName.Text;
			if(UserCur.UserNum==Security.CurUser.UserNum) {
				Security.CurUser.UserName=textUserName.Text;
				//They changed their logged in user's information.  Update for when they sync then attempt to connect to remote DB.
			}
			UserCur.EmployeeNum=0;
			UserCur.ProvNum=0;
			UserCur.ClinicNum=0;
			UserCur.ClinicIsRestricted=false;
			try{
				if(UserCur.IsNew){
					//also updates the user's UserNumCEMT to be the user's usernum.
					long userNum=Userods.Insert(UserCur,listUserGroup.GetListSelected<UserGroup>().Select(x => x.UserGroupNum).ToList(),true);
				}
				else{
					Userods.Update(UserCur,listUserGroup.GetListSelected<UserGroup>().Select(x => x.UserGroupNum).ToList());
					if(UserCur.UserNum==Security.CurUser.UserNum) {
						Security.CurUser=UserCur.Copy();
						if(_passwordTyped!=null) {
							Security.PasswordTyped=_passwordTyped;//update the password typed for middle tier
						}
					}
				}
			}
			catch(Exception ex){
				OpenDental.MessageBox.Show(ex.Message);
				return;
			}
			Cache.Refresh(InvalidType.Security);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
