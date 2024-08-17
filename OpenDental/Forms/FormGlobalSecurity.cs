using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.DirectoryServices;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormGlobalSecurity:FormODBase {
		///<summary>The ObjectGuid for the domain path entered.</summary>
		private string _domainObjectGuid;

		public FormGlobalSecurity() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormGlobalSecurity_Load(object sender,EventArgs e) {
			textLogOffAfterMinutes.Text=PrefC.GetInt(PrefName.SecurityLogOffAfterMinutes).ToString();
			checkAllowLogoffOverride.Checked=PrefC.GetBool(PrefName.SecurityLogOffAllowUserOverride);
			checkPasswordsMustBeStrong.Checked=PrefC.GetBool(PrefName.PasswordsMustBeStrong);
			checkPasswordsStrongIncludeSpecial.Checked=PrefC.GetBool(PrefName.PasswordsStrongIncludeSpecial);
			checkPasswordForceWeakToStrong.Checked=PrefC.GetBool(PrefName.PasswordsWeakChangeToStrong);
			checkTimecardSecurityEnabled.Checked=PrefC.GetBool(PrefName.TimecardSecurityEnabled);
			checkCannotEditPastPayPeriods.Checked=PrefC.GetBool(PrefName.TimecardUsersCantEditPastPayPeriods);
			checkCannotEditOwn.Checked=PrefC.GetBool(PrefName.TimecardUsersDontEditOwnCard);
			checkCannotEditOwn.Enabled=checkTimecardSecurityEnabled.Checked;
			checkCannotEditPastPayPeriods.Enabled=checkTimecardSecurityEnabled.Checked;
			checkDomainLoginEnabled.Checked=PrefC.GetBool(PrefName.DomainLoginEnabled);
			textDomainLoginPath.ReadOnly=!checkDomainLoginEnabled.Checked;
			textDomainLoginPath.Text=PrefC.GetString(PrefName.DomainLoginPath);
			checkLogOffWindows.Checked=PrefC.GetBool(PrefName.SecurityLogOffWithWindows);
			checkUserNameManualEntry.Checked=PrefC.GetBool(PrefName.UserNameManualEntry);
			checkMaintainPatient.Checked=PrefC.GetBool(PrefName.PatientMaintainedOnUserChange);
			if(!PrefC.HasClinicsEnabled) {
				//This pref only matters when clinics are turned on. When clinics are off it behaves the same as if the pref were on. 
				checkMaintainPatient.Visible=false;
			}
			if(PrefC.GetDate(PrefName.BackupReminderLastDateRun).ToShortDateString()==DateTime.MaxValue.AddMonths(-1).ToShortDateString()) {
				checkDisableBackupReminder.Checked=true;
			}
			if(PrefC.GetInt(PrefName.SecurityLockDays)>0) {
				textDaysLock.Text=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			}
			if(PrefC.GetDate(PrefName.SecurityLockDate).Year>1880) {
				textDateLock.Text=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			}
			if(PrefC.GetBool(PrefName.CentralManagerSecurityLock)) {
				butChange.Enabled=false;
				labelGlobalDateLockDisabled.Visible=true;
			}
			List<UserGroup> listGroupsNotAdmin=UserGroups.GetList().FindAll(x => !GroupPermissions.HasPermission(x.UserGroupNum,Permissions.SecurityAdmin,0));
			for(int i=0;i<listGroupsNotAdmin.Count;i++){
				comboGroups.Items.Add(listGroupsNotAdmin[i].Description,listGroupsNotAdmin[i]);
				if(PrefC.GetLong(PrefName.DefaultUserGroup)==listGroupsNotAdmin[i].UserGroupNum) {
					comboGroups.SelectedIndex=i;
				}
			}
		}

		private void checkTimecardSecurityEnabled_Click(object sender,EventArgs e) {
			if(!checkTimecardSecurityEnabled.Checked) {
				checkCannotEditOwn.Checked=false;
				checkCannotEditPastPayPeriods.Checked=false;
			}
			checkCannotEditOwn.Enabled=checkTimecardSecurityEnabled.Checked;//can't edit timecards at all
			checkCannotEditPastPayPeriods.Enabled=checkTimecardSecurityEnabled.Checked;//can only edit own timecard for current pay period
		}

		private void checkCanEditOwnCur_Click(object sender,EventArgs e) {
			if(checkCannotEditPastPayPeriods.Checked) {//one or other can be checked but not both, both can be unchecked
				checkCannotEditOwn.Checked=false;
			}
		}

		private void checkCannotEditOwn_Click(object sender,EventArgs e) {
			if(checkCannotEditOwn.Checked) {//one or other can be checked but not both, both can be unchecked
				checkCannotEditPastPayPeriods.Checked=false;
			}
		}

		private void checkDomainLoginEnabled_CheckedChanged(object sender,EventArgs e) {
			textDomainLoginPath.ReadOnly=!checkDomainLoginEnabled.Checked;
		}

		private void checkDomainLoginEnabled_MouseUp(object sender,MouseEventArgs e) {
			if(!checkDomainLoginEnabled.Checked || !string.IsNullOrWhiteSpace(textDomainLoginPath.Text)) {
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to use your current domain as the domain login path?")) {
				DirectoryEntry directoryEntryRoot = new DirectoryEntry("LDAP://RootDSE");
				if(directoryEntryRoot.Properties["defaultNamingContext"].Value==null) {
					MsgBox.Show(this,"Unable to bind to the current domain.");
					return;
				}
				string defaultNamingContext=directoryEntryRoot.Properties["defaultNamingContext"].Value.ToString();
				textDomainLoginPath.Text="LDAP://"+defaultNamingContext;
				DirectoryEntry directoryEntryTest=new DirectoryEntry(textDomainLoginPath.Text);
				_domainObjectGuid=directoryEntryTest.Guid.ToString();
			}
		}

		///<summary>Validation for the domain login path provided. 
		///Accepted formats are those listed here: https://msdn.microsoft.com/en-us/library/aa746384(v=vs.85).aspx, excluding plain "LDAP:"
		///Does not check if there are users on the domain object, only that the domain object exists and can be searched.</summary>
		private void textDomainLoginPath_Leave(object sender,EventArgs e) {
			if(!checkDomainLoginEnabled.Checked) {
				return;
			}
			if(string.IsNullOrWhiteSpace(textDomainLoginPath.Text)) {
				MsgBox.Show(this,"Warning. Domain Login is enabled, but no path has been entered. If you do not provide a domain path,"
					+"you will not be able to assign domain logins to users.");
				_domainObjectGuid="";
			}
			else {
				DirectoryEntry directoryEntryTest=directoryEntryTest = new DirectoryEntry(textDomainLoginPath.Text);
				DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntryTest);
				try {
					SearchResultCollection searchResultsCollectionTestResults = directorySearcher.FindAll(); //Just do a generic search to verify the object might have users on it
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"An error occurred while attempting to access the provided Domain Login Path."),ex);
					return;
				}
				_domainObjectGuid=directoryEntryTest.Guid.ToString();
			}
		}

		private void checkPasswordsMustBeStrong_Click(object sender,EventArgs e) {
			if(checkPasswordsMustBeStrong.Checked) {//ensuring box is unchecked
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning.  If this box is unchecked, the strong password flag on all users will be reset.  "
				+"If strong passwords are again turned on later, then each user will have to edit their password in order to cause the strong password flag to be set again.")) 
			{
				checkPasswordsMustBeStrong.Checked=true;//recheck it.
				return;
			}
		}

		private void checkDisableBackupReminder_Click(object sender,EventArgs e) {
			using InputBox inputbox = new InputBox("Please enter password");
			inputbox.setTitle("Change Backup Reminder Settings");
			inputbox.ShowDialog();
			if(inputbox.DialogResult!=DialogResult.OK) {
				checkDisableBackupReminder.Checked=!checkDisableBackupReminder.Checked;
				return;
			}
			if(inputbox.textResult.Text!="abracadabra") {
				checkDisableBackupReminder.Checked=!checkDisableBackupReminder.Checked;
				MsgBox.Show(this,"Wrong password");
				return;
			}
		}

		private void butChange_Click(object sender,EventArgs e) {
			using FormSecurityLock formSecurityLock = new FormSecurityLock();
			formSecurityLock.ShowDialog();//prefs are set invalid within that form if needed.
			if(PrefC.GetInt(PrefName.SecurityLockDays)>0) {
				textDaysLock.Text=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			}
			else {
				textDaysLock.Text="";
			}
			if(PrefC.GetDate(PrefName.SecurityLockDate).Year>1880) {
				textDateLock.Text=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			}
			else {
				textDateLock.Text="";
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			int logOffMinutes;
			bool isLogOffMinutesParseValid = Int32.TryParse(textLogOffAfterMinutes.Text, out logOffMinutes);
			if(!isLogOffMinutesParseValid) {
				MsgBox.Show(this,"Please enter a numeric minute value.");
				return;
			}
			if(logOffMinutes<0) {//Automatic log off must be a positive numerical value.
				MsgBox.Show(this,"Log off after minutes is invalid.");
				return;
			}
			DataValid.SetInvalid(InvalidType.Security);
			bool doInvalidatePrefs=false;
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.TimecardSecurityEnabled,checkTimecardSecurityEnabled.Checked);
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.TimecardUsersCantEditPastPayPeriods,checkCannotEditPastPayPeriods.Checked);
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.TimecardUsersDontEditOwnCard,checkCannotEditOwn.Checked);
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.SecurityLogOffWithWindows,checkLogOffWindows.Checked);
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.UserNameManualEntry,checkUserNameManualEntry.Checked);
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.PasswordsStrongIncludeSpecial,checkPasswordsStrongIncludeSpecial.Checked);
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.PasswordsWeakChangeToStrong,checkPasswordForceWeakToStrong.Checked);
			doInvalidatePrefs |=Prefs.UpdateInt(PrefName.SecurityLogOffAfterMinutes,PIn.Int(textLogOffAfterMinutes.Text));
			doInvalidatePrefs |=Prefs.UpdateString(PrefName.DomainLoginPath,PIn.String(textDomainLoginPath.Text));
			doInvalidatePrefs |=Prefs.UpdateString(PrefName.DomainLoginPath,textDomainLoginPath.Text);
			doInvalidatePrefs |=Prefs.UpdateString(PrefName.DomainLoginPath,textDomainLoginPath.Text);
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.DomainLoginEnabled,checkDomainLoginEnabled.Checked);
			doInvalidatePrefs |=(_domainObjectGuid!=null && Prefs.UpdateString(PrefName.DomainObjectGuid,_domainObjectGuid));
			if(comboGroups.SelectedIndex==-1) {
				doInvalidatePrefs |=Prefs.UpdateLong(PrefName.DefaultUserGroup,newValue:0);
			}
			else {
				doInvalidatePrefs |=Prefs.UpdateLong(PrefName.DefaultUserGroup,comboGroups.GetSelected<UserGroup>().UserGroupNum);
			}
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.SecurityLogOffAllowUserOverride,checkAllowLogoffOverride.Checked);
			doInvalidatePrefs |=Prefs.UpdateBool(PrefName.PatientMaintainedOnUserChange,checkMaintainPatient.Checked);
			//if PasswordsMustBeStrong was unchecked, then reset the strong password flags.
			if(Prefs.UpdateBool(PrefName.PasswordsMustBeStrong,checkPasswordsMustBeStrong.Checked) && !checkPasswordsMustBeStrong.Checked) {
				doInvalidatePrefs=true;
				Userods.ResetStrongPasswordFlags();
			}
			if(checkDisableBackupReminder.Checked) {
				doInvalidatePrefs|=Prefs.UpdateDateT(PrefName.BackupReminderLastDateRun,DateTime.MaxValue.AddMonths(-1)); //if MaxValue, gives error on startup.
			}
			else {
				doInvalidatePrefs|=Prefs.UpdateDateT(PrefName.BackupReminderLastDateRun,DateTime.Today);
			}
			if(doInvalidatePrefs) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		
	}
}