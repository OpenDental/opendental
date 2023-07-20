using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using System.Collections.Generic;
using System.DirectoryServices;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormUserEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		public Userod UserodCur;
		private List<AlertSub> _listAlertSubsUserTypesOld;
		private List<UserGroup> _listUserGroups;
		private List<Clinic> _listClinics;
		///<summary>The password that was entered in FormUserPassword.</summary>
		private string _passwordTyped;
		///<summary>The alert categories that are available to be selected. Some alert types will not be displayed if this is not OD HQ.</summary>
		private List<AlertCategory> _listAlertCategories;
		///<summary>The UserOdPref for DoseSpot User ID.</summary>
		private UserOdPref _userOdPrefDoseSpotDefault;
		private List<Employee> _listEmployees;
		private List<Provider> _listProviders;
		private bool _isFromAddUser;
		private bool _isFromCentralUserEdit;
		private List<UserOdPref> _listUserOdPrefsDoseSpotOld;
		private List<UserOdPref> _listUserOdPrefsDoseSpotNew;
		private bool _isFillingList;
		private UserOdPref _userOdPrefLogOffAfterMinutes;
		private string _logOffAfterMinutesInitialValue;

		///<summary></summary>
		public FormUserEdit(Userod userod,bool isFromAddUser=false,bool isFromCentralUserEdit=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			UserodCur=userod.Copy();
			_isFromAddUser=isFromAddUser;
			_isFromCentralUserEdit=isFromCentralUserEdit;
		}

		private void FormUserEdit_Load(object sender, System.EventArgs e) {
			_userOdPrefLogOffAfterMinutes=UserOdPrefs.GetByUserAndFkeyType(UserodCur.UserNum,UserOdFkeyType.LogOffTimerOverride).FirstOrDefault();
			_logOffAfterMinutesInitialValue=_userOdPrefLogOffAfterMinutes?.ValueString??"";
			textLogOffAfterMinutes.Text=_logOffAfterMinutesInitialValue;
			checkIsHidden.Checked=UserodCur.IsHidden;
			if(UserodCur.UserNum!=0) {
				textUserNum.Text=UserodCur.UserNum.ToString();
			}
			textUserName.Text=UserodCur.UserName;
			if(!string.IsNullOrEmpty(UserodCur.DomainUser) && UserodCur.DomainUser.Split('\\').Length>1) {
				textDomainUser.Text=UserodCur.DomainUser.Split('\\')[1];
			}
			if(!PrefC.GetBool(PrefName.DomainLoginEnabled)) {
				labelDomainUser.Visible=false;
				textDomainUser.Visible=false;
				butPickDomainUser.Visible=false;
			}
			checkRequireReset.Checked=UserodCur.IsPasswordResetRequired;
			_listUserGroups=UserGroups.GetList(_isFromCentralUserEdit);
			_isFillingList=true;
			for(int i=0;i<_listUserGroups.Count;i++){
				listUserGroup.Items.Add(_listUserGroups[i].Description,_listUserGroups[i]);
				if(!_isFromAddUser && UserodCur.IsInUserGroup(_listUserGroups[i].UserGroupNum)) {
					listUserGroup.SetSelected(i);
				}
				if(_isFromAddUser && _listUserGroups[i].UserGroupNum==PrefC.GetLong(PrefName.DefaultUserGroup)) {
					listUserGroup.SetSelected(i);
				}
			}
			if(listUserGroup.SelectedIndices.Count==0){//never allowed to delete last group, so this won't fail
				listUserGroup.SelectedIndex=0;
			}
			_isFillingList=false;
			securityTreeUser.FillTreePermissionsInitial();
			RefreshUserTree();
			listEmployee.Items.Clear();
			listEmployee.Items.Add(Lan.g(this,"none"));
			listEmployee.SelectedIndex=0;
			_listEmployees=Employees.GetDeepCopy(true);
			for(int i=0;i<_listEmployees.Count;i++){
				listEmployee.Items.Add(Employees.GetNameFL(_listEmployees[i]));
				if(UserodCur.EmployeeNum==_listEmployees[i].EmployeeNum) {
					listEmployee.SelectedIndex=i+1;
				}
			}
			listProv.Items.Clear();
			listProv.Items.Add(Lan.g(this,"none"));
			listProv.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				listProv.Items.Add(_listProviders[i].GetLongDesc());
				if(UserodCur.ProvNum==_listProviders[i].ProvNum) {
					listProv.SelectedIndex=i+1;
				}
			}
			_listClinics=Clinics.GetDeepCopy(true);
			_listAlertSubsUserTypesOld=AlertSubs.GetAllForUser(UserodCur.UserNum);
			List<long> listClinicNumsSubscribed;
			bool isAllClinicsSubscribed=false;
			if(_listAlertSubsUserTypesOld.Select(x => x.ClinicNum).Contains(-1)) {//User subscribed to all clinics
				isAllClinicsSubscribed=true;
				listClinicNumsSubscribed=_listClinics.Select(x => x.ClinicNum).Distinct().ToList();
			}
			else {
				listClinicNumsSubscribed=_listAlertSubsUserTypesOld.Select(x => x.ClinicNum).Distinct().ToList();
			}
			List<long> listAlertCategoryNums=_listAlertSubsUserTypesOld.Select(x => x.AlertCategoryNum).Distinct().ToList();
			listAlertSubMulti.Items.Clear();
			_listAlertCategories=AlertCategories.GetDeepCopy();
			List<long> listAlertCategoryNumsUser=_listAlertSubsUserTypesOld.Select(x => x.AlertCategoryNum).ToList();
			for(int i=0;i<_listAlertCategories.Count;i++) {
				listAlertSubMulti.Items.Add(Lan.g(this,_listAlertCategories[i].Description));
				listAlertSubMulti.SetSelected(i,listAlertCategoryNumsUser.Contains(_listAlertCategories[i].AlertCategoryNum));
			}
			if(!PrefC.HasClinicsEnabled) {
				tabClinics.Enabled=false;//Disables all controls in the clinics tab.  Tab is still selectable.
				listAlertSubsClinicsMulti.Visible=false;
				labelAlertClinic.Visible=false;
			}
			else {
				listClinic.Items.Clear();
				listClinic.Items.Add(Lan.g(this,"All"));
				listAlertSubsClinicsMulti.Items.Add(Lan.g(this,"All"));
				listAlertSubsClinicsMulti.Items.Add(Lan.g(this,"Headquarters"));
				if(UserodCur.ClinicNum==0) {//Unrestricted
					listClinic.SetSelected(0);
					listClinicMulti.Enabled=false;
				}
				if(isAllClinicsSubscribed) {//They are subscribed to all clinics
					listAlertSubsClinicsMulti.SetSelected(0);
				}
				else if(listClinicNumsSubscribed.Contains(0)) {//They are subscribed to Headquarters
					listAlertSubsClinicsMulti.SetSelected(1);
				}
				List<UserClinic> listUserClinics=UserClinics.GetForUser(UserodCur.UserNum);
				for(int i=0;i<_listClinics.Count;i++) {
					listClinic.Items.Add(_listClinics[i].Abbr);
					listClinicMulti.Items.Add(_listClinics[i].Abbr);
					listAlertSubsClinicsMulti.Items.Add(_listClinics[i].Abbr);
					if(UserodCur.ClinicNum==_listClinics[i].ClinicNum) {
						listClinic.SetSelected(i+1);
					}
					if(UserodCur.ClinicNum!=0 && listUserClinics.Exists(x => x.ClinicNum==_listClinics[i].ClinicNum)) {
						listClinicMulti.SetSelected(i);//No "All" option, don't select i+1
					}
					if(!isAllClinicsSubscribed && _listAlertSubsUserTypesOld.Exists(x => x.ClinicNum==_listClinics[i].ClinicNum)) {
						listAlertSubsClinicsMulti.SetSelected(i+2);//All+HQ
					}
				}
			}
			if(string.IsNullOrEmpty(UserodCur.PasswordHash)){
				butPassword.Text=Lan.g(this,"Create Password");
			}
			if(!PrefC.IsODHQ) {
				butJobRoles.Visible=false;
			}
			if(IsNew) {
				butUnlock.Visible=false;
			}
			_listUserOdPrefsDoseSpotOld=UserOdPrefs.GetByUserAndFkeyAndFkeyType(UserodCur.UserNum,
				Programs.GetCur(ProgramName.eRx).ProgramNum,UserOdFkeyType.Program,
				Clinics.GetForUserod(Security.CurUser,true).Select(x => x.ClinicNum)
				.Union(new List<long>() { 0 })//Always include 0 clinic, this is the default, NOT a headquarters only value.
				.Distinct()
				.ToList());
			_listUserOdPrefsDoseSpotNew=_listUserOdPrefsDoseSpotOld.Select(x => x.Clone()).ToList();
			_userOdPrefDoseSpotDefault=_listUserOdPrefsDoseSpotNew.Find(x => x.ClinicNum==0);
			if(_userOdPrefDoseSpotDefault==null) {
				_userOdPrefDoseSpotDefault=DoseSpot.GetDoseSpotUserIdFromPref(UserodCur.UserNum,0);
				_listUserOdPrefsDoseSpotNew.Add(_userOdPrefDoseSpotDefault);
			}
			textDoseSpotUserID.Text=_userOdPrefDoseSpotDefault.ValueString;
			if(_isFromAddUser && !Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				butPassword.Visible=false;
				checkRequireReset.Checked=true;
				checkRequireReset.Enabled=false;
				butUnlock.Visible=false;
				butJobRoles.Visible=false;
			}
			if(!PrefC.HasClinicsEnabled) {
				butDoseSpotAdditional.Visible=false;
			}
			if(_isFromCentralUserEdit) {
				tabControl1.TabPages.Remove(tabClinics);
				tabControl1.TabPages.Remove(tabAlertSubs);
				textDomainUser.Enabled=false;
				textUserName.Enabled=false;
				checkRequireReset.Enabled=false;
				checkIsHidden.Enabled=false;
				labelAutoLogoff.Enabled=false;
				textLogOffAfterMinutes.Enabled=false;
				listUserGroup.Enabled=false;
				butPassword.Enabled=false;
				butUnlock.Enabled=false;
				butJobRoles.Enabled=false;
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
				MessageBox.Show(Lan.g(this,"An error occurred while attempting to access the provided DomainLoginPath:")+" "+ex.Message);
				return;
			}
			using FormDomainUserPick formDomainUserPick=new FormDomainUserPick();
			formDomainUserPick.ShowDialog();
			if(formDomainUserPick.DialogResult==DialogResult.OK && formDomainUserPick.SelectedDomainName!=null) { //only check for null, as empty string should clear the field
				UserodCur.DomainUser=$@"{PrefC.GetString(PrefName.DomainObjectGuid)}\{formDomainUserPick.SelectedDomainName}";
				textDomainUser.Text=formDomainUserPick.SelectedDomainName;
			}
		}

		private void listClinic_MouseClick(object sender,MouseEventArgs e) {
			int idx=listClinic.IndexFromPoint(e.Location);
			if(idx==-1){
				return;
			}
			if(idx==0){//all
				listClinicMulti.Enabled=false;
				listClinicMulti.SetAll(false);
			}
			else{
				listClinicMulti.Enabled=true;
			}
		}

		private void butPassword_Click(object sender, System.EventArgs e) {
			bool isCreate=string.IsNullOrEmpty(UserodCur.PasswordHash);
			using FormUserPassword formUserPassword=new FormUserPassword(isCreate,UserodCur.UserName);
			formUserPassword.IsInSecurityWindow=true;
			formUserPassword.ShowDialog();
			if(formUserPassword.DialogResult==DialogResult.Cancel){
				return;
			}
			UserodCur.LoginDetails=formUserPassword.PasswordContainer_;
			UserodCur.PasswordIsStrong=formUserPassword.IsPasswordStrong;
			_passwordTyped=formUserPassword.PasswordTyped;
			if(string.IsNullOrEmpty(UserodCur.PasswordHash)) {
				butPassword.Text=Lan.g(this,"Create Password");
			}
			else{
				butPassword.Text=Lan.g(this,"Change Password");
			}
		}

		private void butUnlock_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Users can become locked when invalid credentials have been entered several times in a row.\r\n"
				+"Unlock this user so that more log in attempts can be made?"))
			{
				return;
			}
			UserodCur.DateTFail=DateTime.MinValue;
			UserodCur.FailedAttempts=0;
			try {
				Userods.Update(UserodCur);
				MsgBox.Show(this,"User has been unlocked.");
			}
			catch(Exception) {
				MsgBox.Show(this,"There was a problem unlocking this user.  Please call support or wait the allotted lock time.");
			}
		}

		private void butJobRoles_Click(object sender,EventArgs e) {
			using FormJobPermissions formJobPermissions=new FormJobPermissions(UserodCur.UserNum);
			formJobPermissions.ShowDialog();
		}

		private void butDoseSpotAdditional_Click(object sender,EventArgs e) {
			_userOdPrefDoseSpotDefault.ValueString=textDoseSpotUserID.Text;
			using FormUserPrefAdditional formUserPrefAdditional=new FormUserPrefAdditional(_listUserOdPrefsDoseSpotNew,UserodCur);
			formUserPrefAdditional.ShowDialog();
			if(formUserPrefAdditional.DialogResult==DialogResult.OK) {
				_listUserOdPrefsDoseSpotNew=formUserPrefAdditional.ListUserOdPrefsOut;
				_userOdPrefDoseSpotDefault=_listUserOdPrefsDoseSpotNew.Find(x => x.ClinicNum==0);
				textDoseSpotUserID.Text=_userOdPrefDoseSpotDefault.ValueString;
			}
		}

		private bool IsValidLogOffMinutes() {
			if(!(textLogOffAfterMinutes.Text=="") && (!int.TryParse(textLogOffAfterMinutes.Text,out int minutes) || minutes<0)) {
				MsgBox.Show(this,"Invalid 'Automatic logoff time in minutes'.\r\n" +
					"Must be blank, 0, or a positive integer.");
				return false;
			}
			return true;
		}

		private bool SaveLogOffPreferences() {
			bool isCacheInvalid=false;
			if(textLogOffAfterMinutes.Text.IsNullOrEmpty() && !_logOffAfterMinutesInitialValue.IsNullOrEmpty()) {
				UserOdPrefs.Delete(_userOdPrefLogOffAfterMinutes.UserOdPrefNum);
				isCacheInvalid=true;
			}
			else if(textLogOffAfterMinutes.Text!=_logOffAfterMinutesInitialValue) { //Only do this if the value has changed
				if(_userOdPrefLogOffAfterMinutes==null) {
					_userOdPrefLogOffAfterMinutes=new UserOdPref() { Fkey=0, FkeyType=UserOdFkeyType.LogOffTimerOverride, UserNum=UserodCur.UserNum };
				}
				_userOdPrefLogOffAfterMinutes.ValueString=textLogOffAfterMinutes.Text;
				UserOdPrefs.Upsert(_userOdPrefLogOffAfterMinutes);
				isCacheInvalid=true;
				if(!PrefC.GetBool(PrefName.SecurityLogOffAllowUserOverride)) {
					MsgBox.Show(this,"User logoff overrides will not take effect until the Global Security setting \"Allow user override for automatic logoff\" is checked");
				}
			}
			return isCacheInvalid;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textUserName.Text==""){
				MsgBox.Show(this,"Please enter a username.");
				return;
			}
			if(!_isFromAddUser && IsNew && PrefC.GetBool(PrefName.PasswordsMustBeStrong) && string.IsNullOrWhiteSpace(_passwordTyped)) {
				MsgBox.Show(this,"Password may not be blank when the strong password feature is turned on.");
				return;
			}
			if(PrefC.HasClinicsEnabled && listClinic.SelectedIndex==-1) {
				MsgBox.Show(this,"This user does not have a User Default Clinic set.  Please choose one to continue.");
				return;
			}
			if(listUserGroup.SelectedIndices.Count == 0) {
				MsgBox.Show(this,"Users must have at least one user group associated. Please select a user group to continue.");
				return;
			}
			List<UserOdPref> listUserOdPrefs=UserOdPrefs.GetByFkeyAndFkeyType(Programs.GetCur(ProgramName.eRx).ProgramNum,UserOdFkeyType.Program);
			List<UserOdPref> listUserOdPrefsNotEmpty=listUserOdPrefs.FindAll(x => !x.ValueString.IsNullOrEmpty()
				&& x.UserNum!=UserodCur.UserNum);//Allow user to reuse their ID at the different clinics
			List<string> listValueStrings=listUserOdPrefsNotEmpty.Select(x => x.ValueString).ToList();
			List<UserOdPref> listUserOdPrefsDuplicateIDs=_listUserOdPrefsDoseSpotNew.FindAll(x => listValueStrings.Contains(x.ValueString));
			if((textDoseSpotUserID.Text!=_userOdPrefDoseSpotDefault.ValueString || _userOdPrefDoseSpotDefault.IsNew)
				&& listValueStrings.Contains(textDoseSpotUserID.Text)
				|| !listUserOdPrefsDuplicateIDs.IsNullOrEmpty())
			{
				string msg="One or more of your DoseSpot User IDs is already in use.\r\n" +
					"The following list shows the users and the DoseSpot User ID aleady in use.\r\n\n";
				msg +="DoseSpot User ID\tUser\r\n";;
				listUserOdPrefsNotEmpty=listUserOdPrefsNotEmpty.DistinctBy(x => x.UserNum).ToList();
				for(int i=0;i<listUserOdPrefsNotEmpty.Count;i++) {
					//Add the DoseSpotIDs and corresponding user name to the msg to be displayed in the msgBox.
					//\t\t used for layout. \r\n used to create new line.
					msg+=String.Format("{0}\t\t{1}\r\n",listUserOdPrefsNotEmpty[i].ValueString,Userods.GetName(listUserOdPrefsNotEmpty[i].UserNum));
				}
				MessageBox.Show(this,msg);
				//reset the DoseSpotIDs back to what the values before a duplicate was entered
				for(int i=0;i<_listUserOdPrefsDoseSpotNew.Count;i++) {
					//new user set the DoseSpotIDs back to blank
					if(_listUserOdPrefsDoseSpotNew[i].IsNew) {
						_listUserOdPrefsDoseSpotNew[i].ValueString="";
					}
					//only changes the duplicate DoseSpotIDs that were entered back, but keeps the entered DoseSpotIDs that are not duplicates
					if(listValueStrings.Contains(_listUserOdPrefsDoseSpotNew[i].ValueString) && !_listUserOdPrefsDoseSpotNew[i].IsNew) {
						_listUserOdPrefsDoseSpotNew[i].ValueString=_listUserOdPrefsDoseSpotOld[i].ValueString;//change _listUserOdPrefsDoseSpotNew back to what it was previously
					}
				}
				//if the default DoseSpotID is changed not from the FormUserPrefAdditional, check that textDoseSpotUserID.Text is not a duplicate
				//and change the _userOdPrefDoseSpotDefault.ValueString
				if(textDoseSpotUserID.Text!=_userOdPrefDoseSpotDefault.ValueString && !listValueStrings.Contains(textDoseSpotUserID.Text)) {
					_userOdPrefDoseSpotDefault.ValueString=textDoseSpotUserID.Text;
				}
				textDoseSpotUserID.Text=_userOdPrefDoseSpotDefault.ValueString;
				return;
			}
			if(_isFromAddUser && !Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				if(listUserGroup.SelectedIndices.Count!=1
					|| !listUserGroup.GetListSelected<UserGroup>().Select(x => x.UserGroupNum).Contains(PrefC.GetLong(PrefName.DefaultUserGroup))) 
				{
					MsgBox.Show(this,"This user must be assigned to the default user group.");
					for(int i=0;i<listUserGroup.Items.Count;i++) {
						if(((UserGroup)listUserGroup.Items.GetObjectAt(i)).UserGroupNum==PrefC.GetLong(PrefName.DefaultUserGroup)) {
							listUserGroup.SetSelected(i);
						}
						else {
							listUserGroup.SetSelected(i,false);
						}
					}
					return;
				}
			}
			if(!IsValidLogOffMinutes()) {
				return;
			}
			List<UserClinic> listUserClinics=new List<UserClinic>();
			if(PrefC.HasClinicsEnabled) {//Check to see if users have restricted clinics set. 
				for(int i=0;i<listClinicMulti.SelectedIndices.Count;i++) {
					listUserClinics.Add(new UserClinic(_listClinics[listClinicMulti.SelectedIndices[i]].ClinicNum,UserodCur.UserNum));
				}
				//If they set the user up with a default clinic and it's not in the restricted list, return.
				if(listUserClinics.Count>0 && !listUserClinics.Exists(x => x.ClinicNum==_listClinics[listClinic.SelectedIndex-1].ClinicNum)) {
					MsgBox.Show(this,"User cannot have a default clinic that they are not restricted to.");
					return;
				}
			}
			if(!PrefC.HasClinicsEnabled || listClinic.SelectedIndex==0) {
				UserodCur.ClinicNum=0;
			}
			else {
				UserodCur.ClinicNum=_listClinics[listClinic.SelectedIndex-1].ClinicNum;
			}
			UserodCur.ClinicIsRestricted=false;//This is kept in sync with their choice of "All".
			if(listClinicMulti.SelectedIndices.Count>0) {
				UserodCur.ClinicIsRestricted=true;
			}
			UserodCur.IsHidden=checkIsHidden.Checked;
			UserodCur.IsPasswordResetRequired=checkRequireReset.Checked;
			UserodCur.UserName=textUserName.Text;
			if(listEmployee.SelectedIndex==0){
				UserodCur.EmployeeNum=0;
			}
			else{
				UserodCur.EmployeeNum=_listEmployees[listEmployee.SelectedIndex-1].EmployeeNum;
			}
			if(listProv.SelectedIndex==0) {
				Provider provider=Providers.GetProv(UserodCur.ProvNum);
				if(provider!=null) {
					provider.IsInstructor=false;//If there are more than 1 users associated to this provider, they will no longer be an instructor.
					Providers.Update(provider);	
				}
				UserodCur.ProvNum=0;
			}
			else {
				Provider provider=Providers.GetProv(UserodCur.ProvNum);
				if(provider!=null) {
					if(provider.ProvNum!=_listProviders[listProv.SelectedIndex-1].ProvNum) {
						provider.IsInstructor=false;//If there are more than 1 users associated to this provider, they will no longer be an instructor.
					}
					Providers.Update(provider);
				}
				UserodCur.ProvNum=_listProviders[listProv.SelectedIndex-1].ProvNum;
			}
			if(IsNew) {
				try {
					Userods.Insert(UserodCur,listUserGroup.GetListSelected<UserGroup>().Select(x => x.UserGroupNum).ToList());
				}
				catch (Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				for(int i = 0;i<_listUserOdPrefsDoseSpotNew.Count;i++) {
					//Set the userodprefs to the new user's UserNum that was just retreived from the database.
					_listUserOdPrefsDoseSpotNew[i].UserNum=UserodCur.UserNum;
				}
				for(int i = 0;i<listUserClinics.Count;i++) {
					//Set the user clinic's UserNum to the one we just inserted.
					listUserClinics[i].UserNum=UserodCur.UserNum;
				}
				SecurityLogs.MakeLogEntry(Permissions.AddNewUser,0,"New user '"+UserodCur.UserName+"' added");
			}
			else{
				List<UserGroup> listUserGroupsNew=listUserGroup.GetListSelected<UserGroup>();
				List<UserGroup> listUserGroupsOld=UserodCur.GetGroups();
				try {
					Userods.Update(UserodCur,listUserGroupsNew.Select(x => x.UserGroupNum).ToList());
				}
				catch (Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				//if this is the current user, update the user, credentials, etc.
				if(UserodCur.UserNum==Security.CurUser.UserNum) {
					Security.CurUser=UserodCur.Copy();
					if(_passwordTyped!=null) {
						Security.PasswordTyped=_passwordTyped; //update the password typed for middle tier refresh
					}
				}
				//Log changes to the User's UserGroups.
				Func<List<UserGroup>,List<UserGroup>,List<UserGroup>> funcGetMissing=(listUserGroups1,listUserGroups2) => {
					List<UserGroup> listUserGroupsRet=new List<UserGroup>();
					for(int i = 0;i<listUserGroups1.Count;i++) {
						if(listUserGroups2.Exists(x => x.UserGroupNum==listUserGroups1[i].UserGroupNum)) {
							continue;
						}
						listUserGroupsRet.Add(listUserGroups1[i]);
					}
					return listUserGroupsRet;
				};
				List<UserGroup> listUserGroupsRemoved=funcGetMissing(listUserGroupsOld,listUserGroupsNew);
				List<UserGroup> listUserGroupsAdded=funcGetMissing(listUserGroupsNew,listUserGroupsOld);
				if(listUserGroupsRemoved.Count>0) {//Only log if there are items in the list
					SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"User "+UserodCur.UserName+
						" removed from User group(s): "+string.Join(", ",listUserGroupsRemoved.Select(x => x.Description).ToArray())+" by: "+Security.CurUser.UserName);
				}
				if(listUserGroupsAdded.Count>0) {//Only log if there are items in the list.
					SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"User "+UserodCur.UserName+
						" added to User group(s): "+string.Join(", ",listUserGroupsAdded.Select(x => x.Description).ToArray())+" by: "+Security.CurUser.UserName);
				}
			}
			if(UserClinics.Sync(listUserClinics,UserodCur.UserNum)) {//Either syncs new list, or clears old list if no longer restricted.
				DataValid.SetInvalid(InvalidType.UserClinics);
			}
			bool isUserOdPrefCacheInvalid=false;
			//DoseSpot User ID Insert/Update/Delete
			if(_userOdPrefDoseSpotDefault.ValueString!=textDoseSpotUserID.Text) {
				if(string.IsNullOrWhiteSpace(textDoseSpotUserID.Text)) {
					UserOdPrefs.DeleteMany(_userOdPrefDoseSpotDefault.UserNum,_userOdPrefDoseSpotDefault.Fkey,UserOdFkeyType.Program);
				}
				else {
					_userOdPrefDoseSpotDefault.ValueString=textDoseSpotUserID.Text.Trim();
				}
				isUserOdPrefCacheInvalid=true;
			}
			DataValid.SetInvalid(InvalidType.Security);
			//List of AlertTypes that are selected.
			List<long> listAlertCatagoryNumsUser=new List<long>();
			for(int i=0;i<listAlertSubMulti.SelectedIndices.Count;i++) {
				listAlertCatagoryNumsUser.Add(_listAlertCategories[listAlertSubMulti.SelectedIndices[i]].AlertCategoryNum);
			}
			List<long> listClinicNums=new List<long>();
			for(int i=0;i<listAlertSubsClinicsMulti.SelectedIndices.Count;i++) {
				if(listAlertSubsClinicsMulti.SelectedIndices[i]==0) {//All
					listClinicNums.Add(-1);//Add All
					break;
				}
				if(listAlertSubsClinicsMulti.SelectedIndices[i]==1) {//HQ
					listClinicNums.Add(0);
					continue;
				}
				Clinic clinic=_listClinics[listAlertSubsClinicsMulti.SelectedIndices[i]-2];//Subtract 2 for 'All' and 'HQ'
				listClinicNums.Add(clinic.ClinicNum);
			}
			List<AlertSub> _listAlertSubsUserTypesNew=_listAlertSubsUserTypesOld.Select(x => x.Copy()).ToList();
			//Remove AlertTypes that have been deselected through either deslecting the type or clinic.
			_listAlertSubsUserTypesNew.RemoveAll(x => !listAlertCatagoryNumsUser.Contains(x.AlertCategoryNum));
			if(PrefC.HasClinicsEnabled) {
				_listAlertSubsUserTypesNew.RemoveAll(x => !listClinicNums.Contains(x.ClinicNum));
			}
			for(int i = 0;i<listAlertCatagoryNumsUser.Count;i++) {
				if(!PrefC.HasClinicsEnabled) {
					if(!_listAlertSubsUserTypesOld.Exists(x => x.AlertCategoryNum==listAlertCatagoryNumsUser[i])) {//Was not subscribed to type.
						_listAlertSubsUserTypesNew.Add(new AlertSub(UserodCur.UserNum,0,listAlertCatagoryNumsUser[i]));
					}
					continue;
				}
				//Clinics enabled.
				for(int j = 0;j<listClinicNums.Count;j++) {
					if(!_listAlertSubsUserTypesOld.Exists(x => x.ClinicNum==listClinicNums[j] && x.AlertCategoryNum==listAlertCatagoryNumsUser[i])) {//Was not subscribed to type.
						_listAlertSubsUserTypesNew.Add(new AlertSub(UserodCur.UserNum,listClinicNums[j],listAlertCatagoryNumsUser[i]));
						continue;
					}
				}
			}
			isUserOdPrefCacheInvalid|=SaveLogOffPreferences();
			AlertSubs.Sync(_listAlertSubsUserTypesNew,_listAlertSubsUserTypesOld);
			isUserOdPrefCacheInvalid|=UserOdPrefs.Sync(_listUserOdPrefsDoseSpotNew,_listUserOdPrefsDoseSpotOld);
			if(isUserOdPrefCacheInvalid) {
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}