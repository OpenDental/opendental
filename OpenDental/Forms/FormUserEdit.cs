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
		public Userod UserCur;
		private List<AlertSub> _listUserAlertTypesOld;
		private List<UserGroup> _listUserGroups;
		private List<Clinic> _listClinics;
		///<summary>The password that was entered in FormUserPassword.</summary>
		private string _passwordTyped;
		///<summary>The alert categories that are available to be selected. Some alert types will not be displayed if this is not OD HQ.</summary>
		private List<AlertCategory> _listAlertCategories;
		///<summary>The UserOdPref for DoseSpot User ID.</summary>
		private UserOdPref _doseSpotUserPrefDefault;
		private List<Employee> _listEmployees;
		private List<Provider> _listProviders;
		private bool _isFromAddUser;
		private bool _isFromCentralUserEdit;
		private List<UserOdPref> _listDoseSpotUserPrefOld;
		private List<UserOdPref> _listDoseSpotUserPrefNew;
		private bool _isFillingList;
		private UserOdPref _logOffAfterMinutes;
		private string _logOffAfterMinutesInitialValue;

		///<summary></summary>
		public FormUserEdit(Userod userCur,bool isFromAddUser=false,bool isFromCentralUserEdit=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			UserCur=userCur.Copy();
			_isFromAddUser=isFromAddUser;
			_isFromCentralUserEdit=isFromCentralUserEdit;
		}

		private void FormUserEdit_Load(object sender, System.EventArgs e) {
			_logOffAfterMinutes=UserOdPrefs.GetByUserAndFkeyType(UserCur.UserNum,UserOdFkeyType.LogOffTimerOverride).FirstOrDefault();
			_logOffAfterMinutesInitialValue=(_logOffAfterMinutes==null) ? "" : _logOffAfterMinutes.ValueString;
			textLogOffAfterMinutes.Text=_logOffAfterMinutesInitialValue;
			checkIsHidden.Checked=UserCur.IsHidden;
			if(UserCur.UserNum!=0) {
				textUserNum.Text=UserCur.UserNum.ToString();
			}
			textUserName.Text=UserCur.UserName;
			if(!string.IsNullOrEmpty(UserCur.DomainUser) && UserCur.DomainUser.Split('\\').Length>1) {
				textDomainUser.Text=UserCur.DomainUser.Split('\\')[1];
			}
			if(!PrefC.GetBool(PrefName.DomainLoginEnabled)) {
				labelDomainUser.Visible=false;
				textDomainUser.Visible=false;
				butPickDomainUser.Visible=false;
			}
			checkRequireReset.Checked=UserCur.IsPasswordResetRequired;
			_listUserGroups=UserGroups.GetList(_isFromCentralUserEdit);
			_isFillingList=true;
			for(int i=0;i<_listUserGroups.Count;i++){
				listUserGroup.Items.Add(_listUserGroups[i].Description,_listUserGroups[i]);
				if(!_isFromAddUser && UserCur.IsInUserGroup(_listUserGroups[i].UserGroupNum)) {
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
				if(UserCur.EmployeeNum==_listEmployees[i].EmployeeNum) {
					listEmployee.SelectedIndex=i+1;
				}
			}
			listProv.Items.Clear();
			listProv.Items.Add(Lan.g(this,"none"));
			listProv.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				listProv.Items.Add(_listProviders[i].GetLongDesc());
				if(UserCur.ProvNum==_listProviders[i].ProvNum) {
					listProv.SelectedIndex=i+1;
				}
			}
			_listClinics=Clinics.GetDeepCopy(true);
			_listUserAlertTypesOld=AlertSubs.GetAllForUser(UserCur.UserNum);
			List<long> listSubscribedClinics;
			bool isAllClinicsSubscribed=false;
			if(_listUserAlertTypesOld.Select(x => x.ClinicNum).Contains(-1)) {//User subscribed to all clinics
				isAllClinicsSubscribed=true;
				listSubscribedClinics=_listClinics.Select(x => x.ClinicNum).Distinct().ToList();
			}
			else {
				listSubscribedClinics=_listUserAlertTypesOld.Select(x => x.ClinicNum).Distinct().ToList();
			}
			List<long> listAlertCatNums=_listUserAlertTypesOld.Select(x => x.AlertCategoryNum).Distinct().ToList();
			listAlertSubMulti.Items.Clear();
			_listAlertCategories=AlertCategories.GetDeepCopy();
			List<long> listUserAlertCatNums=_listUserAlertTypesOld.Select(x => x.AlertCategoryNum).ToList();
			for(int i=0;i<_listAlertCategories.Count;i++) {
				listAlertSubMulti.Items.Add(Lan.g(this,_listAlertCategories[i].Description));
				listAlertSubMulti.SetSelected(i,listUserAlertCatNums.Contains(_listAlertCategories[i].AlertCategoryNum));
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
				if(UserCur.ClinicNum==0) {//Unrestricted
					listClinic.SetSelected(0);
					checkClinicIsRestricted.Enabled=false;//We don't really need this checkbox any more but it's probably better for users to keep it....
				}
				if(isAllClinicsSubscribed) {//They are subscribed to all clinics
					listAlertSubsClinicsMulti.SetSelected(0);
				}
				else if(listSubscribedClinics.Contains(0)) {//They are subscribed to Headquarters
					listAlertSubsClinicsMulti.SetSelected(1);
				}
				List<UserClinic> listUserClinics=UserClinics.GetForUser(UserCur.UserNum);
				for(int i=0;i<_listClinics.Count;i++) {
					listClinic.Items.Add(_listClinics[i].Abbr);
					listClinicMulti.Items.Add(_listClinics[i].Abbr);
					listAlertSubsClinicsMulti.Items.Add(_listClinics[i].Abbr);
					if(UserCur.ClinicNum==_listClinics[i].ClinicNum) {
						listClinic.SetSelected(i+1);
					}
					if(UserCur.ClinicNum!=0 && listUserClinics.Exists(x => x.ClinicNum==_listClinics[i].ClinicNum)) {
						listClinicMulti.SetSelected(i);//No "All" option, don't select i+1
					}
					if(!isAllClinicsSubscribed && _listUserAlertTypesOld.Exists(x => x.ClinicNum==_listClinics[i].ClinicNum)) {
						listAlertSubsClinicsMulti.SetSelected(i+2);//All+HQ
					}
				}
				checkClinicIsRestricted.Checked=UserCur.ClinicIsRestricted;
			}
			if(string.IsNullOrEmpty(UserCur.PasswordHash)){
				butPassword.Text=Lan.g(this,"Create Password");
			}
			if(!PrefC.IsODHQ) {
				butJobRoles.Visible=false;
			}
			if(IsNew) {
				butUnlock.Visible=false;
			}
			_listDoseSpotUserPrefOld=UserOdPrefs.GetByUserAndFkeyAndFkeyType(UserCur.UserNum,
				Programs.GetCur(ProgramName.eRx).ProgramNum,UserOdFkeyType.Program,
				Clinics.GetForUserod(Security.CurUser,true).Select(x => x.ClinicNum)
				.Union(new List<long>() { 0 })//Always include 0 clinic, this is the default, NOT a headquarters only value.
				.Distinct()
				.ToList());
			_listDoseSpotUserPrefNew=_listDoseSpotUserPrefOld.Select(x => x.Clone()).ToList();
			_doseSpotUserPrefDefault=_listDoseSpotUserPrefNew.Find(x => x.ClinicNum==0);
			if(_doseSpotUserPrefDefault==null) {
				_doseSpotUserPrefDefault=DoseSpot.GetDoseSpotUserIdFromPref(UserCur.UserNum,0);
				_listDoseSpotUserPrefNew.Add(_doseSpotUserPrefDefault);
			}
			textDoseSpotUserID.Text=_doseSpotUserPrefDefault.ValueString;
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
				LayoutManager.Remove(tabClinics);
				LayoutManager.Remove(tabAlertSubs);
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
			using FormDomainUserPick FormDU=new FormDomainUserPick();
			FormDU.ShowDialog();
			if(FormDU.DialogResult==DialogResult.OK && FormDU.SelectedDomainName!=null) { //only check for null, as empty string should clear the field
				UserCur.DomainUser=$@"{PrefC.GetString(PrefName.DomainObjectGuid)}\{FormDU.SelectedDomainName}";
				textDomainUser.Text=FormDU.SelectedDomainName;
			}
		}

		private void listClinic_MouseClick(object sender,MouseEventArgs e) {
			int idx=listClinic.IndexFromPoint(e.Location);
			if(idx==-1){
				return;
			}
			if(idx==0){//all
				checkClinicIsRestricted.Checked=false;
				checkClinicIsRestricted.Enabled=false;
			}
			else{
				checkClinicIsRestricted.Enabled=true;
			}
		}

		private void butPassword_Click(object sender, System.EventArgs e) {
			bool isCreate=string.IsNullOrEmpty(UserCur.PasswordHash);
			using FormUserPassword FormU=new FormUserPassword(isCreate,UserCur.UserName);
			FormU.IsInSecurityWindow=true;
			FormU.ShowDialog();
			if(FormU.DialogResult==DialogResult.Cancel){
				return;
			}
			UserCur.LoginDetails=FormU.LoginDetails;
			UserCur.PasswordIsStrong=FormU.PasswordIsStrong;
			_passwordTyped=FormU.PasswordTyped;
			if(string.IsNullOrEmpty(UserCur.PasswordHash)) {
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
			UserCur.DateTFail=DateTime.MinValue;
			UserCur.FailedAttempts=0;
			try {
				Userods.Update(UserCur);
				MsgBox.Show(this,"User has been unlocked.");
			}
			catch(Exception) {
				MsgBox.Show(this,"There was a problem unlocking this user.  Please call support or wait the allotted lock time.");
			}
		}

		private void butJobRoles_Click(object sender,EventArgs e) {
			using FormJobPermissions FormJR=new FormJobPermissions(UserCur.UserNum);
			FormJR.ShowDialog();
		}

		private void butDoseSpotAdditional_Click(object sender,EventArgs e) {
			_doseSpotUserPrefDefault.ValueString=textDoseSpotUserID.Text;
			using FormUserPrefAdditional FormUP=new FormUserPrefAdditional(_listDoseSpotUserPrefNew,UserCur);
			FormUP.ShowDialog();
			if(FormUP.DialogResult==DialogResult.OK) {
				_listDoseSpotUserPrefNew=FormUP.ListUserPrefOut;
				_doseSpotUserPrefDefault=_listDoseSpotUserPrefNew.Find(x => x.ClinicNum==0);
				textDoseSpotUserID.Text=_doseSpotUserPrefDefault.ValueString;
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

		private void SaveLogOffPreferences() {
			if(textLogOffAfterMinutes.Text.IsNullOrEmpty() && !_logOffAfterMinutesInitialValue.IsNullOrEmpty()) {
				UserOdPrefs.Delete(_logOffAfterMinutes.UserOdPrefNum);
			}
			else if(textLogOffAfterMinutes.Text!=_logOffAfterMinutesInitialValue) { //Only do this if the value has changed
				if(_logOffAfterMinutes==null) {
					_logOffAfterMinutes=new UserOdPref() { Fkey=0, FkeyType=UserOdFkeyType.LogOffTimerOverride, UserNum=UserCur.UserNum };
				}
				_logOffAfterMinutes.ValueString=textLogOffAfterMinutes.Text;
				UserOdPrefs.Upsert(_logOffAfterMinutes);
				if(!PrefC.GetBool(PrefName.SecurityLogOffAllowUserOverride)) {
					MsgBox.Show(this,"User logoff overrides will not take effect until the Global Security setting \"Allow user override for automatic logoff\" is checked");
				}
			}
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
			if(PrefC.HasClinicsEnabled && checkClinicIsRestricted.Checked) {//They want to restrict the user to certain clinics or clinics are enabled.  
				for(int i=0;i<listClinicMulti.SelectedIndices.Count;i++) {
					listUserClinics.Add(new UserClinic(_listClinics[listClinicMulti.SelectedIndices[i]].ClinicNum,UserCur.UserNum));
				}
				//If they set the user up with a default clinic and it's not in the restricted list, return.
				if(!listUserClinics.Exists(x => x.ClinicNum==_listClinics[listClinic.SelectedIndex-1].ClinicNum)) {
					MsgBox.Show(this,"User cannot have a default clinic that they are not restricted to.");
					return;
				}
			}
			if(!PrefC.HasClinicsEnabled || listClinic.SelectedIndex==0) {
				UserCur.ClinicNum=0;
			}
			else {
				UserCur.ClinicNum=_listClinics[listClinic.SelectedIndex-1].ClinicNum;
			}
			UserCur.ClinicIsRestricted=checkClinicIsRestricted.Checked;//This is kept in sync with their choice of "All".
			UserCur.IsHidden=checkIsHidden.Checked;
			UserCur.IsPasswordResetRequired=checkRequireReset.Checked;
			UserCur.UserName=textUserName.Text;
			if(listEmployee.SelectedIndex==0){
				UserCur.EmployeeNum=0;
			}
			else{
				UserCur.EmployeeNum=_listEmployees[listEmployee.SelectedIndex-1].EmployeeNum;
			}
			if(listProv.SelectedIndex==0) {
				Provider prov=Providers.GetProv(UserCur.ProvNum);
				if(prov!=null) {
					prov.IsInstructor=false;//If there are more than 1 users associated to this provider, they will no longer be an instructor.
					Providers.Update(prov);	
				}
				UserCur.ProvNum=0;
			}
			else {
				Provider prov=Providers.GetProv(UserCur.ProvNum);
				if(prov!=null) {
					if(prov.ProvNum!=_listProviders[listProv.SelectedIndex-1].ProvNum) {
						prov.IsInstructor=false;//If there are more than 1 users associated to this provider, they will no longer be an instructor.
					}
					Providers.Update(prov);
				}
				UserCur.ProvNum=_listProviders[listProv.SelectedIndex-1].ProvNum;
			}
			try{
				if(IsNew){
					Userods.Insert(UserCur,listUserGroup.GetListSelected<UserGroup>().Select(x => x.UserGroupNum).ToList());
					//Set the userodprefs to the new user's UserNum that was just retreived from the database.
					_listDoseSpotUserPrefNew.ForEach(x => x.UserNum=UserCur.UserNum);
					listUserClinics.ForEach(x => x.UserNum=UserCur.UserNum);//Set the user clinic's UserNum to the one we just inserted.
					SecurityLogs.MakeLogEntry(Permissions.AddNewUser,0,"New user '"+UserCur.UserName+"' added");
				}
				else{
					List<UserGroup> listNewUserGroups=listUserGroup.GetListSelected<UserGroup>();
					List<UserGroup> listOldUserGroups=UserCur.GetGroups();
					Userods.Update(UserCur,listNewUserGroups.Select(x => x.UserGroupNum).ToList());
					//if this is the current user, update the user, credentials, etc.
					if(UserCur.UserNum==Security.CurUser.UserNum) {
						Security.CurUser=UserCur.Copy();
						if(_passwordTyped!=null) {
							Security.PasswordTyped=_passwordTyped; //update the password typed for middle tier refresh
						}
					}
					//Log changes to the User's UserGroups.
					Func<List<UserGroup>,List<UserGroup>,List<UserGroup>> funcGetMissing=(listCur,listCompare) => {
						List<UserGroup> retVal=new List<UserGroup>();
						foreach(UserGroup group in listCur) {
							if(listCompare.Exists(x => x.UserGroupNum==group.UserGroupNum)) {
								continue;
							}
							retVal.Add(group);
						}
						return retVal;
					};
					List<UserGroup> listRemovedGroups=funcGetMissing(listOldUserGroups,listNewUserGroups);
					List<UserGroup> listAddedGroups=funcGetMissing(listNewUserGroups,listOldUserGroups);
					if(listRemovedGroups.Count>0) {//Only log if there are items in the list
						SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"User "+UserCur.UserName+
							" removed from User group(s): "+string.Join(", ",listRemovedGroups.Select(x => x.Description).ToArray())+" by: "+Security.CurUser.UserName);
					}
					if(listAddedGroups.Count>0) {//Only log if there are items in the list.
						SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"User "+UserCur.UserName+
							" added to User group(s): "+string.Join(", ",listAddedGroups.Select(x => x.Description).ToArray())+" by: "+Security.CurUser.UserName);
					}
				}
				if(UserClinics.Sync(listUserClinics,UserCur.UserNum)) {//Either syncs new list, or clears old list if no longer restricted.
					DataValid.SetInvalid(InvalidType.UserClinics);
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			//DoseSpot User ID Insert/Update/Delete
			if(_doseSpotUserPrefDefault.ValueString!=textDoseSpotUserID.Text) {
				if(string.IsNullOrWhiteSpace(textDoseSpotUserID.Text)) {
					UserOdPrefs.DeleteMany(_doseSpotUserPrefDefault.UserNum,_doseSpotUserPrefDefault.Fkey,UserOdFkeyType.Program);
				}
				else {
					_doseSpotUserPrefDefault.ValueString=textDoseSpotUserID.Text.Trim();
					UserOdPrefs.Upsert(_doseSpotUserPrefDefault);
				}
			}
			DataValid.SetInvalid(InvalidType.Security);
			//List of AlertTypes that are selected.
			List<long> listUserAlertCats=new List<long>();
			for(int i=0;i<listAlertSubMulti.SelectedIndices.Count;i++) {
				listUserAlertCats.Add(_listAlertCategories[listAlertSubMulti.SelectedIndices[i]].AlertCategoryNum);
			}
			List<long> listClinics=new List<long>();
			for(int i=0;i<listAlertSubsClinicsMulti.SelectedIndices.Count;i++) {
				if(listAlertSubsClinicsMulti.SelectedIndices[i]==0) {//All
					listClinics.Add(-1);//Add All
					break;
				}
				if(listAlertSubsClinicsMulti.SelectedIndices[i]==1) {//HQ
					listClinics.Add(0);
					continue;
				}
				Clinic clinic=_listClinics[listAlertSubsClinicsMulti.SelectedIndices[i]-2];//Subtract 2 for 'All' and 'HQ'
				listClinics.Add(clinic.ClinicNum);
			}
			List<AlertSub> _listUserAlertTypesNew=_listUserAlertTypesOld.Select(x => x.Copy()).ToList();
			//Remove AlertTypes that have been deselected through either deslecting the type or clinic.
			_listUserAlertTypesNew.RemoveAll(x => !listUserAlertCats.Contains(x.AlertCategoryNum));
			if(PrefC.HasClinicsEnabled) {
				_listUserAlertTypesNew.RemoveAll(x => !listClinics.Contains(x.ClinicNum));
			}
			foreach(long alertCatNum in listUserAlertCats) {
				if(!PrefC.HasClinicsEnabled) {
					if(!_listUserAlertTypesOld.Exists(x => x.AlertCategoryNum==alertCatNum)) {//Was not subscribed to type.
						_listUserAlertTypesNew.Add(new AlertSub(UserCur.UserNum,0,alertCatNum));
					}
				}
				else {//Clinics enabled.
					foreach(long clinicNumCur in listClinics) {
						if(!_listUserAlertTypesOld.Exists(x => x.ClinicNum==clinicNumCur && x.AlertCategoryNum==alertCatNum)) {//Was not subscribed to type.
							_listUserAlertTypesNew.Add(new AlertSub(UserCur.UserNum,clinicNumCur,alertCatNum));
							continue;
						}
					}
				}
			}
			SaveLogOffPreferences();
			AlertSubs.Sync(_listUserAlertTypesNew,_listUserAlertTypesOld);
			UserOdPrefs.Sync(_listDoseSpotUserPrefNew,_listDoseSpotUserPrefOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}