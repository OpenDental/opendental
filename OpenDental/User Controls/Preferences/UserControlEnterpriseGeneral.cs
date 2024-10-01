using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlEnterpriseGeneral:UserControl {

		#region Fields - Private
		private bool _doUsePhonenumTable=false;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlEnterpriseGeneral() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void checkExactMatchPhoneNum_CheckedChanged(object sender,EventArgs e) {
			textEnterpriseExactMatchPhoneNumDigits.Enabled=checkEnterpriseExactMatchPhone.Checked;
		}

		private void butChange_Click(object sender,EventArgs e) {
			//Copied from FormGlobalSecurity.
			using FormSecurityLock formSecurityLock=new FormSecurityLock();
			formSecurityLock.ShowDialog();//prefs are set invalid within that form if needed.
			if(PrefC.GetInt(PrefName.SecurityLockDays)>0) {
				textSecurityLockDays.Text=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			}
			else {
				textSecurityLockDays.Text="";
			}
			if(PrefC.GetDate(PrefName.SecurityLockDate).Year>1880) {
				textSecurityLockDate.Text=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			}
			else {
				textSecurityLockDate.Text="";
			}
			checkSecurityLockIncludesAdmin.Checked=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
		}

		private void butSyncPhNums_Click(object sender,EventArgs e) {
			if(SyncPhoneNums()) {
				MsgBox.Show(this,"Done");
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Event Handlers Sync
		private void textSignalInactiveMinutes_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.SignalInactiveMinutes);
			if(!textSignalInactiveMinutes.IsValid()) {
				string errorMsg="Disable signal interval must be a valid number or blank.\r\n";
				MsgBox.Show(this,"Please fix the following errors:\r\n"+errorMsg);
				return;
			}
			prefValSync.PrefVal=POut.Int(textSignalInactiveMinutes.Value);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void textProcessSigsIntervalInSecs_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ProcessSigsIntervalInSecs);
			if(!textProcessSigsIntervalInSecs.IsValid()) {
				string errorMsg="Signal interval must be a valid number or blank.\r\n";
				MsgBox.Show(this,"Please fix the following errors:\r\n"+errorMsg);
				return;
			}
			prefValSync.PrefVal=POut.Int(textProcessSigsIntervalInSecs.Value);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void checkPatientPhoneUsePhoneNumberTable_Click(object sender,EventArgs e) {
			if(!_doUsePhonenumTable && checkPatientPhoneUsePhonenumberTable.Checked) {
				string msgText="When enabling the use of the phonenumber table a one-time sync of patient phone numbers needs to take place.  This could "+
					"take a couple minutes.  Continue?";
				if(MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
					if(SyncPhoneNums()) {
						MsgBox.Show(this,"Done");
					}
				}
			}
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PatientPhoneUsePhonenumberTable);
			prefValSync.PrefVal=POut.Bool(checkPatientPhoneUsePhonenumberTable.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void checkPatientSelectFilterRestrictedClinics_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PatientSelectFilterRestrictedClinics);
			prefValSync.PrefVal=POut.Bool(checkPatientSelectFilterRestrictedClinics.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void checkEnterpriseAllowRefreshWhileTyping_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.EnterpriseAllowRefreshWhileTyping);
			prefValSync.PrefVal=POut.Bool(checkEnterpriseAllowRefreshWhileTyping.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}
		#endregion Methods - Event Handlers Sync

		#region Methods - Private
		///<summary>Checks preferences that take user entry for errors, returns true if all entries are valid.</summary>
		private bool ValidateEntries() {
			string errorMsg="";
			//SecurityLogOffAfterMinutes
			if(textSecurityLogOffAfterMinutes.Text!="") {
				try {
					int logOffMinutes = Int32.Parse(textSecurityLogOffAfterMinutes.Text);
					if(logOffMinutes<0) {//Automatic log off must be a positive numerical value.
						throw new Exception();
					}
				}
				catch(Exception e) {
					e.DoNothing();
					errorMsg+="Log off after minutes is invalid. Must be a positive number.\r\n";
				}
			}
			//ProcessSigsIntervalInSecs
			if(!textProcessSigsIntervalInSecs.IsValid()) {
				errorMsg+="Signal interval must be a valid number or blank.\r\n";
			}
			//SignalInactiveMinutes
			if(!textSignalInactiveMinutes.IsValid()) {
				errorMsg+="Disable signal interval must be a valid number or blank.\r\n";
			}
			//PatientSelectSearchMinChars
			if(!textPatientSelectSearchMinChars.IsValid()) {
				errorMsg+="The patient select number of characters before filling the grid must be a valid number.\r\n";
			}
			//PatientSelectSearchPauseMs
			if(!textPatientSelectSearchPauseMs.IsValid()) {
				errorMsg+="The patient select number of milliseconds to wait before filling the grid must be a valid number.\r\n";
			}
			if(!textEnterpriseExactMatchPhoneNumDigits.IsValid()) {
				errorMsg+="The number of phone digits for exact match searching must be between 1 and 25.\r\n";
			}
			if(errorMsg!="") {
				MsgBox.Show(this,"Please fix the following errors:\r\n"+errorMsg);
				return false;
			}
			//if(!_doUsePhonenumTable && checkPatientPhoneUsePhonenumberTable.Checked) {
			//	string msgText="When enabling the use of the phonenumber table a one-time sync of patient phone numbers needs to take place.  This could "+
			//		"take a couple minutes.  Continue?";
			//	if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
			//		return false;
			//	}
			//	if(!SyncPhoneNums()) {
			//		return false;
			//	}
			//	MsgBox.Show(this,"Done");
			//}
			return true;
		}

		private bool SyncPhoneNums() {
			UI.ProgressWin progressOD=new UI.ProgressWin();
			progressOD.ShowCancelButton=false;
			progressOD.ActionMain=PhoneNumbers.SyncAllPats;
			progressOD.StartingMessage=Lan.g(this,"Syncing all patient phone numbers to the phonenumber table")+"...";
			try{
				progressOD.ShowDialog();
			}
			catch(Exception ex){
				MsgBox.Show(Lan.g(this,"The patient phone number sync failed with the message")+":\r\n"+ex.Message+"\r\n"+Lan.g(this,"Please try again."));
				return false;
			}
			if(progressOD.IsCancelled){
				return false;
			}
			_doUsePhonenumTable=true;//so it won't sync again if you clicked the button
			return true;
		}

		///<summary>Load values from database for hidden preferences if they exist.  If a pref doesn't exist then the corresponding UI is hidden.</summary>
		private void FillHiddenPrefs() {
			FillOptionalPrefBool(checkDatabaseMaintenanceDisableOptimize,PrefName.DatabaseMaintenanceDisableOptimize);
			FillOptionalPrefBool(checkDatabaseMaintenanceSkipCheckTable,PrefName.DatabaseMaintenanceSkipCheckTable);
			checkHasClinicsEnabled.Checked=PrefC.HasClinicsEnabled;
			string updateStreamline=GetHiddenPrefString(PrefName.UpdateStreamLinePassword);
			if(updateStreamline!=null) {
				checkUpdateStreamlinePassword.Checked=(updateStreamline=="abracadabra");
			}
			else {
				checkUpdateStreamlinePassword.Visible=false;
			}
			string updateAlterLargeTablesDirectly=GetHiddenPrefString(PrefName.UpdateAlterLargeTablesDirectly);
			if(updateAlterLargeTablesDirectly!=null) {
				checkUpdateAlterLargeTablesDirectly.Checked=updateAlterLargeTablesDirectly=="1";
			}
			else {
				checkUpdateAlterLargeTablesDirectly.Visible=false;
			}
		}

		///<summary>Returns the ValueString of a pref or null if that pref is not found in the database.</summary>
		private string GetHiddenPrefString(PrefName prefName) {
			Pref prefHidden=null;
			try {
				prefHidden=Prefs.GetOne(prefName);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return null;
			}
			return prefHidden.ValueString;
		}

		///<summary>Helper method for setting UI for boolean preferences.  Some of the preferences calling this may not exist in the database.</summary>
		private void FillOptionalPrefBool(UI.CheckBox checkBox,PrefName prefName) {
			string valueString=GetHiddenPrefString(prefName);
			if(valueString==null) {
				checkBox.Visible=false;
				return;
			}
			checkBox.Checked=PIn.Bool(valueString);
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillEnterpriseGeneral() {
			try {
				FillHiddenPrefs();
			}
			catch(Exception ex) {
				ex.DoNothing();//Suppress unhandled exceptions from hidden preferences, since they are read only.
			}
			checkPasswordsMustBeStrong.Checked=PrefC.GetBool(PrefName.PasswordsMustBeStrong);
			checkPasswordsStrongIncludeSpecial.Checked=PrefC.GetBool(PrefName.PasswordsStrongIncludeSpecial);
			checkPasswordsWeakChangeToStrong.Checked=PrefC.GetBool(PrefName.PasswordsWeakChangeToStrong);
			checkSecurityLockIncludesAdmin.Checked=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
			textSecurityLogOffAfterMinutes.Text=PrefC.GetInt(PrefName.SecurityLogOffAfterMinutes).ToString();
			checkUserNameManualEntry.Checked=PrefC.GetBool(PrefName.UserNameManualEntry);
			textSecurityLockDate.Text=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			textSecurityLockDays.Text=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			//long signalInactiveMinutes=PrefC.GetLong(PrefName.SignalInactiveMinutes);
			//textSignalInactiveMinutes.Text=(signalInactiveMinutes==0 ? "" : signalInactiveMinutes.ToString());
			//long sigIntervalSeconds=PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs);
			//textProcessSigsIntervalInSecs.Text=(sigIntervalSeconds==0 ? "" : sigIntervalSeconds.ToString());
			string patientSelectSearchMinChars=PrefC.GetString(PrefName.PatientSelectSearchMinChars);
			textPatientSelectSearchMinChars.Text=Math.Min(10,Math.Max(1,PIn.Int(patientSelectSearchMinChars,false))).ToString();//enforce minimum 1 maximum 10
			string patientSelectSearchPauseMs=PrefC.GetString(PrefName.PatientSelectSearchPauseMs);
			textPatientSelectSearchPauseMs.Text=Math.Min(10000,Math.Max(1,PIn.Int(patientSelectSearchPauseMs,false))).ToString();//enforce minimum 1 maximum 10000
			YN ynPatientSelectSearchWithEmptyParams=PIn.Enum<YN>(PrefC.GetInt(PrefName.PatientSelectSearchWithEmptyParams));
			if(ynPatientSelectSearchWithEmptyParams!=YN.Unknown) {
				checkPatientSelectSearchWithEmptyParams.CheckState=CheckState.Unchecked;
				checkPatientSelectSearchWithEmptyParams.Checked=ynPatientSelectSearchWithEmptyParams==YN.Yes;
			}
			//_doUsePhonenumTable=PrefC.GetBool(PrefName.PatientPhoneUsePhonenumberTable);
			//checkPatientPhoneUsePhonenumberTable.Checked=_doUsePhonenumTable;
			//checkPatientSelectFilterRestrictedClinics.Checked=PrefC.GetBool(PrefName.PatientSelectFilterRestrictedClinics);
			textEnterpriseExactMatchPhoneNumDigits.Text=PrefC.GetInt(PrefName.EnterpriseExactMatchPhoneNumDigits).ToString();
			checkEnterpriseExactMatchPhone.Checked=PrefC.GetBool(PrefName.EnterpriseExactMatchPhone);
			//checkEnterpriseAllowRefreshWhileTyping.Checked=PrefC.GetBool(PrefName.EnterpriseAllowRefreshWhileTyping);
			checkEnableEmailAddressAutoComplete.Checked=PrefC.GetBool(PrefName.EnableEmailAddressAutoComplete);
			checkEnterpriseCommlogOmitDefaults.Checked=PrefC.GetBool(PrefName.EnterpriseCommlogOmitDefaults);
			checkDatabaseGlobalVariablesDontSet.Checked=PrefC.GetBool(PrefName.DatabaseGlobalVariablesDontSet);
		}

		public bool SaveEnterpriseGeneral() {
			if(!ValidateEntries()) {
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.PasswordsMustBeStrong,checkPasswordsMustBeStrong.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PasswordsStrongIncludeSpecial,checkPasswordsStrongIncludeSpecial.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PasswordsWeakChangeToStrong,checkPasswordsWeakChangeToStrong.Checked);
			Changed|=Prefs.UpdateBool(PrefName.SecurityLockIncludesAdmin,checkSecurityLockIncludesAdmin.Checked);
			Changed|=Prefs.UpdateInt(PrefName.SecurityLogOffAfterMinutes,PIn.Int(textSecurityLogOffAfterMinutes.Text));
			Changed|=Prefs.UpdateBool(PrefName.UserNameManualEntry,checkUserNameManualEntry.Checked);
			//(synced) Changed|=Prefs.UpdateLong(PrefName.SignalInactiveMinutes,PIn.Long(textSignalInactiveMinutes.Text));
			//(synced) Changed|=Prefs.UpdateLong(PrefName.ProcessSigsIntervalInSecs,PIn.Long(textProcessSigsIntervalInSecs.Text));
			Changed|=Prefs.UpdateInt(PrefName.PatientSelectSearchMinChars,PIn.Int(textPatientSelectSearchMinChars.Text));
			Changed|=Prefs.UpdateInt(PrefName.PatientSelectSearchPauseMs,PIn.Int(textPatientSelectSearchPauseMs.Text));
			if(checkPatientSelectSearchWithEmptyParams.CheckState!=CheckState.Indeterminate) {
				Changed|=Prefs.UpdateInt(PrefName.PatientSelectSearchWithEmptyParams,(int)(checkPatientSelectSearchWithEmptyParams.Checked ? YN.Yes : YN.No));
			}
			//Changed|=Prefs.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,checkPatientPhoneUsePhonenumberTable.Checked);
			//Changed|=Prefs.UpdateBool(PrefName.PatientSelectFilterRestrictedClinics,checkPatientSelectFilterRestrictedClinics.Checked);
			Changed|=Prefs.UpdateInt(PrefName.EnterpriseExactMatchPhoneNumDigits,PIn.Int(textEnterpriseExactMatchPhoneNumDigits.Text));
			Changed|=Prefs.UpdateBool(PrefName.EnterpriseExactMatchPhone,checkEnterpriseExactMatchPhone.Checked);
			//Changed|=Prefs.UpdateBool(PrefName.EnterpriseAllowRefreshWhileTyping,checkEnterpriseAllowRefreshWhileTyping.Checked);
			Changed|=Prefs.UpdateBool(PrefName.EnableEmailAddressAutoComplete,checkEnableEmailAddressAutoComplete.Checked);
			Changed|=Prefs.UpdateBool(PrefName.EnterpriseCommlogOmitDefaults,checkEnterpriseCommlogOmitDefaults.Checked);
			Changed|=Prefs.UpdateBool(PrefName.DatabaseGlobalVariablesDontSet,checkDatabaseGlobalVariablesDontSet.Checked);
			return true;
		}

		public void FillSynced(){
			//This will revert invalid Values back to the PrefVal if other prefs are updated before invalid Values are fixed
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.SignalInactiveMinutes);
			textSignalInactiveMinutes.Value=PIn.Int(prefValSync.PrefVal);//0 shows as empty
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ProcessSigsIntervalInSecs);
			textProcessSigsIntervalInSecs.Value=PIn.Int(prefValSync.PrefVal);//0 shows as empty
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PatientPhoneUsePhonenumberTable);
			_doUsePhonenumTable=PIn.Bool(prefValSync.PrefVal);
			checkPatientPhoneUsePhonenumberTable.Checked=_doUsePhonenumTable;
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PatientSelectFilterRestrictedClinics);
			checkPatientSelectFilterRestrictedClinics.Checked=PIn.Bool(prefValSync.PrefVal);
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.EnterpriseAllowRefreshWhileTyping);
			checkEnterpriseAllowRefreshWhileTyping.Checked=PIn.Bool(prefValSync.PrefVal);
		}
		#endregion Methods - Public
	}
}
