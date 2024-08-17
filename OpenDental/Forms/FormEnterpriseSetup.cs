using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEnterpriseSetup:FormODBase {
		private int _intervalReceiveClaimReports;
		private bool _doUsePhonenumTable=false;

		public FormEnterpriseSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEnterpriseSetup_Load(object sender,EventArgs e) {
			FillStandardPrefs();
			try {
				FillHiddenPrefs();
			}
			catch(Exception ex) {
				ex.DoNothing();//Suppress unhandled exceptions from hidden preferences, since they are read only.
			}
			if(ODBuild.IsWeb()) {
				tabControlMain.TabPages.Remove(tabReport);//Not supported in OD Cloud
			}
		}

		/// <summary>Sets UI for preferences that we know for sure will exist.</summary>
		private void FillStandardPrefs() {
			#region Account Tab
			checkCaclAgingBatchClaims.Checked=PrefC.GetBool(PrefName.AgingCalculateOnBatchClaimReceipt);
			comboPaymentClinicSetting.Items.AddEnums<PayClinicSetting>();
			comboPaymentClinicSetting.SelectedIndex=PrefC.GetInt(PrefName.PaymentClinicSetting);
			checkPaymentsPromptForPayType.Checked=PrefC.GetBool(PrefName.PaymentsPromptForPayType);
			checkBillShowTransSinceZero.Checked=PrefC.GetBool(PrefName.BillingShowTransSinceBalZero);
			textClaimIdentifier.Text=PrefC.GetString(PrefName.ClaimIdPrefix);
			checkReceiveReportsService.Checked=PrefC.GetBool(PrefName.ClaimReportReceivedByService);
			_intervalReceiveClaimReports=PrefC.GetInt(PrefName.ClaimReportReceiveInterval);
			if(_intervalReceiveClaimReports==0) {
				radioTime.Checked=true;
				DateTime dateTReceived=PrefC.GetDateT(PrefName.ClaimReportReceiveTime);
				textReportCheckTime.Text=dateTReceived.ToShortTimeString();
			}
			else {
				textReportCheckInterval.Text=POut.Int(_intervalReceiveClaimReports);
				radioInterval.Checked=true;
			}
			List<RigorousAccounting> listRigorousAccountings=Enum.GetValues(typeof(RigorousAccounting)).OfType<RigorousAccounting>().ToList();
			for(int i=0;i<listRigorousAccountings.Count;i++) {
				comboRigorousAccounting.Items.Add(listRigorousAccountings[i].GetDescription());
			}
			comboRigorousAccounting.SelectedIndex=PrefC.GetInt(PrefName.RigorousAccounting);
			List<RigorousAdjustments> listRigorousAdjustments=Enum.GetValues(typeof(RigorousAdjustments)).OfType<RigorousAdjustments>().ToList();
			for(int i=0;i<listRigorousAdjustments.Count;i++) {
				comboRigorousAdjustments.Items.Add(listRigorousAdjustments[i].GetDescription());
			}
			comboRigorousAdjustments.SelectedIndex=PrefC.GetInt(PrefName.RigorousAdjustments);
			checkHidePaysplits.Checked=PrefC.GetBool(PrefName.PaymentWindowDefaultHideSplits);
			comboPayPlansVersion.Items.AddEnums<PayPlanVersions>();
			comboPayPlansVersion.SetSelectedEnum(PrefC.GetInt(PrefName.PayPlansVersion));
			textBillingElectBatchMax.Text=PrefC.GetInt(PrefName.BillingElectBatchMax).ToString();
			checkBillingShowProgress.Checked=PrefC.GetBool(PrefName.BillingShowSendProgress);
			#endregion Account Tab
			#region Advanced Tab
			checkPasswordsMustBeStrong.Checked=PrefC.GetBool(PrefName.PasswordsMustBeStrong);
			checkPasswordsStrongIncludeSpecial.Checked=PrefC.GetBool(PrefName.PasswordsStrongIncludeSpecial);
			checkPasswordForceWeakToStrong.Checked=PrefC.GetBool(PrefName.PasswordsWeakChangeToStrong);
			checkLockIncludesAdmin.Checked=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
			textLogOffAfterMinutes.Text=PrefC.GetInt(PrefName.SecurityLogOffAfterMinutes).ToString();
			checkUserNameManualEntry.Checked=PrefC.GetBool(PrefName.UserNameManualEntry);
			textDateLock.Text=PrefC.GetDate(PrefName.SecurityLockDate).ToShortDateString();
			textDaysLock.Text=PrefC.GetInt(PrefName.SecurityLockDays).ToString();
			long minutesSignalInactive=PrefC.GetLong(PrefName.SignalInactiveMinutes);
			textInactiveSignal.Text=(minutesSignalInactive==0 ? "" : minutesSignalInactive.ToString());
			long sigIntervalSeconds=PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs);
			textSigInterval.Text=(sigIntervalSeconds==0 ? "" : sigIntervalSeconds.ToString());
			string patSearchMinChars=PrefC.GetString(PrefName.PatientSelectSearchMinChars);
			textPatSelectMinChars.Text=Math.Min(10,Math.Max(1,PIn.Int(patSearchMinChars,false))).ToString();//enforce minimum 1 maximum 10
			string patSearchPauseMs=PrefC.GetString(PrefName.PatientSelectSearchPauseMs);
			textPatSelectPauseMs.Text=Math.Min(10000,Math.Max(1,PIn.Int(patSearchPauseMs,false))).ToString();//enforce minimum 1 maximum 10000
			checkPatientSelectFilterRestrictedClinics.Checked=PrefC.GetBool(PrefName.PatientSelectFilterRestrictedClinics);
			checkMatchExactPhoneNum.Checked=PrefC.GetBool(PrefName.EnterpriseExactMatchPhone);
			textPhoneNumDigits.Text=PrefC.GetInt(PrefName.EnterpriseExactMatchPhoneNumDigits).ToString();
			checkEnterpriseAllowRefresh.Checked=PrefC.GetBool(PrefName.EnterpriseAllowRefreshWhileTyping);
			YN ynSearchEmptyParams=PIn.Enum<YN>(PrefC.GetInt(PrefName.PatientSelectSearchWithEmptyParams));
			if(ynSearchEmptyParams!=YN.Unknown) {
				checkPatSearchEmptyParams.CheckState=CheckState.Unchecked;
				checkPatSearchEmptyParams.Checked=ynSearchEmptyParams==YN.Yes;
			}
			_doUsePhonenumTable=PrefC.GetBool(PrefName.PatientPhoneUsePhonenumberTable);
			checkUsePhoneNumTable.Checked=_doUsePhonenumTable;
			checkEnableEmailAddressAutoComplete.Checked=PrefC.GetBool(PrefName.EnableEmailAddressAutoComplete);
			checkEnterpriseCommlogOmitDefaults.Checked=PrefC.GetBool(PrefName.EnterpriseCommlogOmitDefaults);
			#endregion Advanced Tab
			#region Appts Tab
			checkApptsRequireProcs.Checked=PrefC.GetBool(PrefName.ApptsRequireProc);
			checkUseOpHygProv.Checked=PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly);
			checkEnterpriseApptList.Checked=PrefC.GetBool(PrefName.EnterpriseApptList);
			checkEnableNoneView.Checked=PrefC.GetBool(PrefName.EnterpriseNoneApptViewDefaultDisabled);
			checkHygProcUsePriProvFee.Checked=PrefC.GetBool(PrefName.EnterpriseHygProcUsePriProvFee);
			#endregion Appts Tab
			#region Family Tab
			checkSuperFam.Checked=PrefC.GetBool(PrefName.ShowFeatureSuperfamilies);
			checkPatClone.Checked=PrefC.GetBool(PrefName.ShowFeaturePatientClone);
			checkShowFeeSchedGroups.Checked=PrefC.GetBool(PrefName.ShowFeeSchedGroups);
			checkSuperFamCloneCreate.Checked=PrefC.GetBool(PrefName.CloneCreateSuperFamily);
			//users should only see the snapshot trigger and service runtime if they have it set to something other than ClaimCreate.
			//if a user wants to be able to change claimsnapshot settings, the following MySQL statement should be run:
			//UPDATE preference SET ValueString = 'Service'	 WHERE PrefName = 'ClaimSnapshotTriggerType'
			if(PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),isEnumAsString:true)==ClaimSnapshotTrigger.ClaimCreate) {
				groupClaimSnapshot.Visible=false;
			}
			comboClaimSnapshotTrigger.Items.AddEnums<ClaimSnapshotTrigger>();
			comboClaimSnapshotTrigger.SelectedIndex=(int)PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true);
			textClaimSnapshotRunTime.Text=PrefC.GetDateT(PrefName.ClaimSnapshotRunTime).ToShortTimeString();
			#endregion Family Tab
			#region Reports Tab
			checkUseReportServer.Checked=(PrefC.GetString(PrefName.ReportingServerCompName)!="" || PrefC.GetString(PrefName.ReportingServerURI)!="");
			textServerName.Text=PrefC.GetString(PrefName.ReportingServerCompName);
			comboDatabase.Text=PrefC.GetString(PrefName.ReportingServerDbName);
			textMysqlUser.Text=PrefC.GetString(PrefName.ReportingServerMySqlUser);
			string decryptedPass;
			CDT.Class1.Decrypt(PrefC.GetString(PrefName.ReportingServerMySqlPassHash),out decryptedPass);
			textMysqlPass.Text=decryptedPass;
			textMiddleTierURI.Text=PrefC.GetString(PrefName.ReportingServerURI);
			FillComboDatabases();
			SetReportServerUIEnabled();
			#endregion Reports Tab
			#region Manage Tab
			checkEra835sRefreshOnLoad.Checked=PrefC.GetBool(PrefName.EraRefreshOnLoad);
			checkEra835sStrictClaimMatching.Checked=PrefC.GetBool(PrefName.EraStrictClaimMatching);
			checkEra835sShowStatusAndClinic.Checked=PrefC.GetBool(PrefName.EraShowStatusAndClinic);
			checkRefresh.Checked=PrefC.GetBool(PrefName.EnterpriseManualRefreshMainTaskLists);
			#endregion Manage Tab
		}

		///<summary>Load values from database for hidden preferences if they exist.  If a pref doesn't exist then the corresponding UI is hidden.</summary>
		private void FillHiddenPrefs() {
			FillOptionalPrefBool(checkAgingShowPayplanPayments,PrefName.AgingReportShowAgePatPayplanPayments);
			FillOptionalPrefBool(checkClaimSnapshotEnabled,PrefName.ClaimSnapshotEnabled);
			FillOptionalPrefBool(checkDBMDisableOptimize,PrefName.DatabaseMaintenanceDisableOptimize);
			FillOptionalPrefBool(checkDBMSkipCheckTable,PrefName.DatabaseMaintenanceSkipCheckTable);
			validDateAgingServiceTimeDue.Text=PrefC.GetDateT(PrefName.AgingServiceTimeDue).ToShortTimeString();
			checkEnableClinics.Checked=PrefC.HasClinicsEnabled;
			string updateStreamline=GetHiddenPrefString(PrefName.UpdateStreamLinePassword);
			if(updateStreamline!=null) {
				checkUpdateStreamlinePassword.Checked=(updateStreamline=="abracadabra");
			}
			else {
				checkUpdateStreamlinePassword.Visible=false;
			}
			string updateLargeTables=GetHiddenPrefString(PrefName.UpdateAlterLargeTablesDirectly);
			if(updateLargeTables!=null) {
				checkUpdateAlterLargeTablesDirectly.Checked=updateLargeTables=="1";
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

		#region Report helper functions

		private void SetReportServerUIEnabled() {
			if(checkUseReportServer.Checked) {
				radioReportServerDirect.Enabled=true;
				radioReportServerMiddleTier.Enabled=true;
				if(radioReportServerDirect.Checked) {
					groupConnectionSettings.Enabled=true;
					groupMiddleTier.Enabled=false;
				}
				else {
					groupConnectionSettings.Enabled=false;
					groupMiddleTier.Enabled=true;
				}
			}
			else {
				radioReportServerDirect.Enabled=false;
				radioReportServerMiddleTier.Enabled=false;
				groupConnectionSettings.Enabled=false;
				groupMiddleTier.Enabled=false;
			}
		}

		private void FillComboDatabases() {
			comboDatabase.Items.Clear();
			comboDatabase.Items.AddRange(GetDatabases());
		}

		///<summary>Taken from FormReportSetup.</summary>
		private string[] GetDatabases() {
			if(textServerName.Text=="") {
				return new string[0];
			}
			DataConnection dataConnection;
			DataTable table;
			string command="SHOW DATABASES";
			//use the one table that we know exists
			if(textMysqlUser.Text=="") {
				dataConnection=new DataConnection(textServerName.Text,"mysql","root",textMysqlPass.Text,DatabaseType.MySql);
			}
			else {
				dataConnection=new DataConnection(textServerName.Text,"mysql",textMysqlUser.Text,textMysqlPass.Text,DatabaseType.MySql);
			}
			try {	//if this next step fails, table will simply have 0 rows
				table=dataConnection.GetTable(command,false);
			}
			catch(Exception) {
				return new string[0];
			}
			string[] stringArrayNames=new string[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++) {
				stringArrayNames[i]=table.Rows[i][0].ToString();
			}
			return stringArrayNames;
		}

		#endregion
		#region Update preference helpers

		private void UpdatePreferenceChanges() {
			bool hasChanges=false;
			bool hasChangesViews=false;
			if(Prefs.UpdateBool(PrefName.AgingCalculateOnBatchClaimReceipt,checkCaclAgingBatchClaims.Checked)
				| Prefs.UpdateBool(PrefName.ApptSecondaryProviderConsiderOpOnly,checkUseOpHygProv.Checked)
				| Prefs.UpdateBool(PrefName.ApptsRequireProc,checkApptsRequireProcs.Checked)
				| Prefs.UpdateBool(PrefName.BillingShowSendProgress,checkBillingShowProgress.Checked)
				| Prefs.UpdateBool(PrefName.BillingShowTransSinceBalZero,checkBillShowTransSinceZero.Checked)
				| Prefs.UpdateBool(PrefName.ClaimReportReceivedByService,checkReceiveReportsService.Checked)
				| Prefs.UpdateBool(PrefName.CloneCreateSuperFamily,checkSuperFamCloneCreate.Checked)
				| Prefs.UpdateBool(PrefName.EnterpriseApptList,checkEnterpriseApptList.Checked)
				| Prefs.UpdateBool(PrefName.EnterpriseHygProcUsePriProvFee,checkHygProcUsePriProvFee.Checked)
				| Prefs.UpdateBool(PrefName.PasswordsMustBeStrong,checkPasswordsMustBeStrong.Checked)
				| Prefs.UpdateBool(PrefName.PasswordsStrongIncludeSpecial,checkPasswordsStrongIncludeSpecial.Checked)
				| Prefs.UpdateBool(PrefName.PasswordsWeakChangeToStrong,checkPasswordForceWeakToStrong.Checked)
				| Prefs.UpdateBool(PrefName.PaymentWindowDefaultHideSplits,checkHidePaysplits.Checked)
				| Prefs.UpdateBool(PrefName.PaymentsPromptForPayType,checkPaymentsPromptForPayType.Checked)
				| Prefs.UpdateBool(PrefName.SecurityLockIncludesAdmin,checkLockIncludesAdmin.Checked)
				| Prefs.UpdateBool(PrefName.ShowFeaturePatientClone,checkPatClone.Checked)
				| Prefs.UpdateBool(PrefName.ShowFeatureSuperfamilies,checkSuperFam.Checked)
				| Prefs.UpdateBool(PrefName.ShowFeeSchedGroups,checkShowFeeSchedGroups.Checked)
				| Prefs.UpdateBool(PrefName.UserNameManualEntry,checkUserNameManualEntry.Checked)
				| Prefs.UpdateInt(PrefName.BillingElectBatchMax,PIn.Int(textBillingElectBatchMax.Text))
				| Prefs.UpdateString(PrefName.ClaimIdPrefix,textClaimIdentifier.Text)
				| Prefs.UpdateInt(PrefName.ClaimReportReceiveInterval,PIn.Int(textReportCheckInterval.Text))
				| Prefs.UpdateDateT(PrefName.ClaimReportReceiveTime,PIn.DateT(textReportCheckTime.Text))
				| Prefs.UpdateLong(PrefName.ProcessSigsIntervalInSecs,PIn.Long(textSigInterval.Text))
				//SecurityLockDate and SecurityLockDays are handled in FormSecurityLock
				//| Prefs.UpdateString(PrefName.SecurityLockDate,POut.Date(PIn.Date(textDateLock.Text),false))
				//| Prefs.UpdateInt(PrefName.SecurityLockDays,PIn.Int(textDaysLock.Text))
				| Prefs.UpdateInt(PrefName.SecurityLogOffAfterMinutes,PIn.Int(textLogOffAfterMinutes.Text))
				| Prefs.UpdateLong(PrefName.SignalInactiveMinutes,PIn.Long(textInactiveSignal.Text))
				| Prefs.UpdateInt(PrefName.PayPlansVersion,(int)comboPayPlansVersion.GetSelected<PayPlanVersions>())
				| Prefs.UpdateInt(PrefName.PaymentClinicSetting,comboPaymentClinicSetting.SelectedIndex)
				| Prefs.UpdateInt(PrefName.PatientSelectSearchMinChars,PIn.Int(textPatSelectMinChars.Text))
				| Prefs.UpdateInt(PrefName.PatientSelectSearchPauseMs,PIn.Int(textPatSelectPauseMs.Text))
				| Prefs.UpdateBool(PrefName.PatientSelectFilterRestrictedClinics,checkPatientSelectFilterRestrictedClinics.Checked)
				| Prefs.UpdateBool(PrefName.EnableEmailAddressAutoComplete,checkEnableEmailAddressAutoComplete.Checked)
				| Prefs.UpdateBool(PrefName.EnterpriseExactMatchPhone,checkMatchExactPhoneNum.Checked)
				| Prefs.UpdateInt(PrefName.EnterpriseExactMatchPhoneNumDigits,PIn.Int(textPhoneNumDigits.Text))
				| Prefs.UpdateBool(PrefName.EnterpriseAllowRefreshWhileTyping,checkEnterpriseAllowRefresh.Checked)
				| Prefs.UpdateBool(PrefName.EraRefreshOnLoad,checkEra835sRefreshOnLoad.Checked)
				| Prefs.UpdateBool(PrefName.EraStrictClaimMatching,checkEra835sStrictClaimMatching.Checked)
				| Prefs.UpdateBool(PrefName.EraShowStatusAndClinic,checkEra835sShowStatusAndClinic.Checked)
				| Prefs.UpdateBool(PrefName.EnterpriseCommlogOmitDefaults,checkEnterpriseCommlogOmitDefaults.Checked)
				| Prefs.UpdateBool(PrefName.EnterpriseManualRefreshMainTaskLists,checkRefresh.Checked)
			)
			{
				hasChanges=true;
			}
			if(checkPatSearchEmptyParams.CheckState!=CheckState.Indeterminate) {
				hasChanges|=Prefs.UpdateInt(PrefName.PatientSelectSearchWithEmptyParams,(int)(checkPatSearchEmptyParams.Checked ? YN.Yes : YN.No));
			}
			hasChanges|=Prefs.UpdateBool(PrefName.PatientPhoneUsePhonenumberTable,checkUsePhoneNumTable.Checked);
			if(Prefs.UpdateBool(PrefName.EnterpriseNoneApptViewDefaultDisabled,checkEnableNoneView.Checked)) {
				hasChanges=true;
				hasChangesViews=true;
			}
			int prefRigorousAccounting=PrefC.GetInt(PrefName.RigorousAccounting);
			//Copied logging for RigorousAccounting and RigorousAdjustments from FormModuleSetup.
			if(Prefs.UpdateInt(PrefName.RigorousAccounting,comboRigorousAccounting.SelectedIndex)) {
				hasChanges=true;
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Rigorous accounting changed from "+
					((RigorousAccounting)prefRigorousAccounting).GetDescription()+" to "
					+((RigorousAccounting)comboRigorousAccounting.SelectedIndex).GetDescription()+".");
			}
			int prefRigorousAdjustments=PrefC.GetInt(PrefName.RigorousAdjustments);
			if(Prefs.UpdateInt(PrefName.RigorousAdjustments,comboRigorousAdjustments.SelectedIndex)) {
				hasChanges=true;
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Rigorous adjustments changed from "+
					((RigorousAdjustments)prefRigorousAdjustments).GetDescription()+" to "
					+((RigorousAdjustments)comboRigorousAdjustments.SelectedIndex).GetDescription()+".");
			}
			hasChanges|=UpdateReportingServer();
			hasChanges|=UpdateClaimSnapshotRuntime();
			hasChanges|=UpdateClaimSnapshotTrigger();
			if(hasChanges) {
				DataValid.SetInvalid(InvalidType.Prefs);
				if(hasChangesViews) {
					DataValid.SetInvalid(InvalidType.Views);
				}
			}
		}

		///<summary>Copied from FormReportSetup.</summary>
		private bool UpdateReportingServer() {
			bool changed=false;
			if(!checkUseReportServer.Checked) {
				if(Prefs.UpdateString(PrefName.ReportingServerCompName,"")
					| Prefs.UpdateString(PrefName.ReportingServerDbName,"")
					| Prefs.UpdateString(PrefName.ReportingServerMySqlUser,"")
					| Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,"")
					| Prefs.UpdateString(PrefName.ReportingServerURI,""))
				{
					changed=true;
				}
			}
			else {
				if(radioReportServerDirect.Checked) {
					string encryptedPass;
					CDT.Class1.Encrypt(textMysqlPass.Text,out encryptedPass);
					if(Prefs.UpdateString(PrefName.ReportingServerCompName,textServerName.Text)
						| Prefs.UpdateString(PrefName.ReportingServerDbName,comboDatabase.Text)
						| Prefs.UpdateString(PrefName.ReportingServerMySqlUser,textMysqlUser.Text)
						| Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,encryptedPass)
						| Prefs.UpdateString(PrefName.ReportingServerURI,""))
					{
						changed=true;
					}
				}
				else {
					if(Prefs.UpdateString(PrefName.ReportingServerCompName,"")
						|Prefs.UpdateString(PrefName.ReportingServerDbName,"")
						|Prefs.UpdateString(PrefName.ReportingServerMySqlUser,"")
						|Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,"")
						|Prefs.UpdateString(PrefName.ReportingServerURI,textMiddleTierURI.Text))
					{
						changed=true;
					}
				}
			}
			return changed;
		}

		private bool UpdateClaimSnapshotRuntime() {
			DateTime dateTSnapshotRunTime=DateTime.MinValue;
			DateTime.TryParse(textClaimSnapshotRunTime.Text,out dateTSnapshotRunTime);//This already gets checked in the validate method.
			dateTSnapshotRunTime=new DateTime(1881,01,01,dateTSnapshotRunTime.Hour,dateTSnapshotRunTime.Minute,dateTSnapshotRunTime.Second);
			return Prefs.UpdateDateT(PrefName.ClaimSnapshotRunTime,dateTSnapshotRunTime);
		}

		private bool UpdateClaimSnapshotTrigger() {
			for(int i=0;i<Enum.GetValues(typeof(ClaimSnapshotTrigger)).Length;i++) {
				ClaimSnapshotTrigger claimSnapshotTrigger=(ClaimSnapshotTrigger)i;
				if(claimSnapshotTrigger.GetDescription()==comboClaimSnapshotTrigger.Text) {
					return Prefs.UpdateString(PrefName.ClaimSnapshotTriggerType,claimSnapshotTrigger.ToString());
				}
			}
			return false;
		}

		///<summary>Checks preferences that take user entry for errors, returns true if all entries are valid.</summary>
		private bool ValidateEntries() {
			string errorMsg="";
			//SecurityLogOffAfterMinutes
			if(textLogOffAfterMinutes.Text!="") {
				try {
					int logOffMinutes = Int32.Parse(textLogOffAfterMinutes.Text);
					if(logOffMinutes<0) {//Automatic log off must be a positive numerical value.
						throw new Exception();
					}
				}
				catch(Exception e) {
					e.DoNothing();
					errorMsg+="Log off after minutes is invalid. Must be a positive number.\r\n";
				}
			}
			//ClaimReportReceiveInterval
			int reportCheckIntervalMinuteCount=0;
			reportCheckIntervalMinuteCount=PIn.Int(textReportCheckInterval.Text,false);
			if(textReportCheckInterval.Enabled && (reportCheckIntervalMinuteCount<5 || reportCheckIntervalMinuteCount>60)) {
				errorMsg+="Report check interval must be between 5 and 60 inclusive.\r\n";
			}
			//ClaimReportReceiveTime
			if(radioTime.Checked && (textReportCheckTime.Text=="" || !textReportCheckTime.IsValid())) {
				errorMsg+="Please enter a time to receive reports.";
			}
			//ClaimSnapshotRuntime
			if(!DateTime.TryParse(textClaimSnapshotRunTime.Text,out DateTime claimSnapshotRunTime)) {
				errorMsg+="Service Snapshot Run Time must be a valid time value.\r\n";
			}
			//ProcessSigsIntervalInSecs
			if(!textSigInterval.IsValid()) {
				errorMsg+="Signal interval must be a valid number or blank.\r\n";
			}
			//SignalInactiveMinutes
			if(!textInactiveSignal.IsValid()) {
				errorMsg+="Disable signal interval must be a valid number or blank.\r\n";
			}
			//BillingElectBatchMax
			if(!textBillingElectBatchMax.IsValid()) {
				errorMsg+="The maximum number of statements per batch must be a valid number or blank.\r\n";
			}
			//PatientSelectSearchMinChars
			if(!textPatSelectMinChars.IsValid()) {
				errorMsg+="The patient select number of characters before filling the grid must be a valid number.\r\n";
			}
			//PatientSelectSearchPauseMs
			if(!textPatSelectPauseMs.IsValid()) {
				errorMsg+="The patient select number of milliseconds to wait before filling the grid must be a valid number.\r\n";
			}
			if(!textPhoneNumDigits.IsValid()) {
				errorMsg+="The number of phone digits for exact match searching must be between 1 and 25.\r\n";
			}
			if(errorMsg!="") {
				MsgBox.Show(this,"Please fix the following errors:\r\n"+errorMsg);
				return false;
			}
			if(!_doUsePhonenumTable && checkUsePhoneNumTable.Checked) {
				string msgText="When enabling the use of the phonenumber table a one-time sync of patient phone numbers needs to take place.  This could "+
					"take a couple minutes.  Continue?";
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,msgText)) {
					return false;
				}
				if(!SyncPhoneNums()) {
					return false;
				}
				MsgBox.Show(this,"Done");
			}
			return true;
		}

		private bool SyncPhoneNums() {
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ShowCancelButton=false;
			progressOD.ActionMain=PhoneNumbers.SyncAllPats;
			progressOD.StartingMessage=Lan.g(this,"Syncing all patient phone numbers to the phonenumber table")+"...";
			try{
				progressOD.ShowDialogProgress();
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

		#endregion

		private void checkUseReportServer_CheckedChanged(object sender,EventArgs e) {
			SetReportServerUIEnabled();
		}

		private void checkExactMatchPhoneNum_CheckedChanged(object sender,EventArgs e) {
			textPhoneNumDigits.Enabled=checkMatchExactPhoneNum.Checked;
		}

		private void RadioReportServerDirect_CheckedChanged(object sender,EventArgs e) {
			SetReportServerUIEnabled();
		}

		private void ComboDatabase_DropDown(object sender,EventArgs e) {
			FillComboDatabases();
		}

		private void radioInterval_CheckedChanged(object sender,EventArgs e) {
			//Copied from FormClearingHouses
			if(radioInterval.Checked) {
				labelReportheckUnits.Enabled=true;
				textReportCheckInterval.Enabled=true;
				textReportCheckTime.Text="";
				textReportCheckTime.Enabled=false;
				textReportCheckTime.ClearError();
			}
			else {
				labelReportheckUnits.Enabled=false;
				textReportCheckInterval.Text="";
				textReportCheckInterval.Enabled=false;
				textReportCheckTime.Enabled=true;
			}
		}

		private void butReplacements_Click(object sender,EventArgs e) {
			//Copied from FormModuleSetup.
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			using FormMessageReplacements formMessageReplcements=new FormMessageReplacements(listMessageReplaceTypes);
			formMessageReplcements.IsSelectionMode=true;
			formMessageReplcements.ShowDialog();
			if(formMessageReplcements.DialogResult!=DialogResult.OK) {
				return;
			}
			textClaimIdentifier.Focus();
			int cursorIndex=textClaimIdentifier.SelectionStart;
			textClaimIdentifier.Text=textClaimIdentifier.Text.Insert(cursorIndex,formMessageReplcements.Replacement);
			textClaimIdentifier.SelectionStart=cursorIndex+formMessageReplcements.Replacement.Length;
		}

		private void butChange_Click(object sender,EventArgs e) {
			//Copied from FormGlobalSecurity.
			using FormSecurityLock formSecurityLock=new FormSecurityLock();
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
			checkLockIncludesAdmin.Checked=PrefC.GetBool(PrefName.SecurityLockIncludesAdmin);
		}

		private void butSyncPhNums_Click(object sender,EventArgs e) {
			if(SyncPhoneNums()) {
				MsgBox.Show(this,"Done");
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateEntries()) {
				return;
			}
			UpdatePreferenceChanges();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}