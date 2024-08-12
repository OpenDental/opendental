
namespace OpenDental {
	partial class UserControlEnterpriseGeneral {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
		if(disposing && (components != null)) {
		components.Dispose();
		}
		base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.checkDatabaseGlobalVariablesDontSet = new OpenDental.UI.CheckBox();
			this.checkEnterpriseCommlogOmitDefaults = new OpenDental.UI.CheckBox();
			this.checkEnableEmailAddressAutoComplete = new OpenDental.UI.CheckBox();
			this.checkUpdateAlterLargeTablesDirectly = new OpenDental.UI.CheckBox();
			this.groupPatientSelect = new OpenDental.UI.GroupBox();
			this.checkEnterpriseAllowRefreshWhileTyping = new OpenDental.UI.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textEnterpriseExactMatchPhoneNumDigits = new OpenDental.ValidNum();
			this.checkEnterpriseExactMatchPhone = new OpenDental.UI.CheckBox();
			this.checkPatientSelectFilterRestrictedClinics = new OpenDental.UI.CheckBox();
			this.butSyncPhNums = new OpenDental.UI.Button();
			this.checkPatientPhoneUsePhonenumberTable = new OpenDental.UI.CheckBox();
			this.checkPatientSelectSearchWithEmptyParams = new OpenDental.UI.CheckBox();
			this.labelPatSelectPauseMs = new System.Windows.Forms.Label();
			this.textPatientSelectSearchPauseMs = new OpenDental.ValidNum();
			this.labelPatSelectMinChars = new System.Windows.Forms.Label();
			this.textPatientSelectSearchMinChars = new OpenDental.ValidNum();
			this.checkUserNameManualEntry = new OpenDental.UI.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkUpdateStreamlinePassword = new OpenDental.UI.CheckBox();
			this.textProcessSigsIntervalInSecs = new OpenDental.ValidNum();
			this.textSecurityLogOffAfterMinutes = new System.Windows.Forms.TextBox();
			this.checkDatabaseMaintenanceSkipCheckTable = new OpenDental.UI.CheckBox();
			this.checkDatabaseMaintenanceDisableOptimize = new OpenDental.UI.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.checkPasswordsMustBeStrong = new OpenDental.UI.CheckBox();
			this.textSignalInactiveMinutes = new OpenDental.ValidNum();
			this.label12 = new System.Windows.Forms.Label();
			this.checkSecurityLockIncludesAdmin = new OpenDental.UI.CheckBox();
			this.checkPasswordsWeakChangeToStrong = new OpenDental.UI.CheckBox();
			this.checkPasswordsStrongIncludeSpecial = new OpenDental.UI.CheckBox();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.textSecurityLockDate = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butChange = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.textSecurityLockDays = new System.Windows.Forms.TextBox();
			this.labelGlobalDateLockDisabled = new System.Windows.Forms.Label();
			this.checkHasClinicsEnabled = new OpenDental.UI.CheckBox();
			this.groupPatientSelect.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkDatabaseGlobalVariablesDontSet
			// 
			this.checkDatabaseGlobalVariablesDontSet.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDatabaseGlobalVariablesDontSet.Location = new System.Drawing.Point(12, 629);
			this.checkDatabaseGlobalVariablesDontSet.Name = "checkDatabaseGlobalVariablesDontSet";
			this.checkDatabaseGlobalVariablesDontSet.Size = new System.Drawing.Size(461, 17);
			this.checkDatabaseGlobalVariablesDontSet.TabIndex = 314;
			this.checkDatabaseGlobalVariablesDontSet.Text = "Disable setting SQL global variables (Used for hosted databases)";
			// 
			// checkEnterpriseCommlogOmitDefaults
			// 
			this.checkEnterpriseCommlogOmitDefaults.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseCommlogOmitDefaults.Location = new System.Drawing.Point(251, 610);
			this.checkEnterpriseCommlogOmitDefaults.Name = "checkEnterpriseCommlogOmitDefaults";
			this.checkEnterpriseCommlogOmitDefaults.Size = new System.Drawing.Size(222, 17);
			this.checkEnterpriseCommlogOmitDefaults.TabIndex = 313;
			this.checkEnterpriseCommlogOmitDefaults.Text = "Commlog fields blank by default";
			// 
			// checkEnableEmailAddressAutoComplete
			// 
			this.checkEnableEmailAddressAutoComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableEmailAddressAutoComplete.Location = new System.Drawing.Point(12, 591);
			this.checkEnableEmailAddressAutoComplete.Name = "checkEnableEmailAddressAutoComplete";
			this.checkEnableEmailAddressAutoComplete.Size = new System.Drawing.Size(461, 18);
			this.checkEnableEmailAddressAutoComplete.TabIndex = 312;
			this.checkEnableEmailAddressAutoComplete.Text = "Enable email address auto-complete";
			// 
			// checkUpdateAlterLargeTablesDirectly
			// 
			this.checkUpdateAlterLargeTablesDirectly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUpdateAlterLargeTablesDirectly.Enabled = false;
			this.checkUpdateAlterLargeTablesDirectly.Location = new System.Drawing.Point(252, 294);
			this.checkUpdateAlterLargeTablesDirectly.Name = "checkUpdateAlterLargeTablesDirectly";
			this.checkUpdateAlterLargeTablesDirectly.Size = new System.Drawing.Size(221, 18);
			this.checkUpdateAlterLargeTablesDirectly.TabIndex = 311;
			this.checkUpdateAlterLargeTablesDirectly.Text = "Update alter large tables directly";
			// 
			// groupPatientSelect
			// 
			this.groupPatientSelect.Controls.Add(this.checkEnterpriseAllowRefreshWhileTyping);
			this.groupPatientSelect.Controls.Add(this.label2);
			this.groupPatientSelect.Controls.Add(this.textEnterpriseExactMatchPhoneNumDigits);
			this.groupPatientSelect.Controls.Add(this.checkEnterpriseExactMatchPhone);
			this.groupPatientSelect.Controls.Add(this.checkPatientSelectFilterRestrictedClinics);
			this.groupPatientSelect.Controls.Add(this.butSyncPhNums);
			this.groupPatientSelect.Controls.Add(this.checkPatientPhoneUsePhonenumberTable);
			this.groupPatientSelect.Controls.Add(this.checkPatientSelectSearchWithEmptyParams);
			this.groupPatientSelect.Controls.Add(this.labelPatSelectPauseMs);
			this.groupPatientSelect.Controls.Add(this.textPatientSelectSearchPauseMs);
			this.groupPatientSelect.Controls.Add(this.labelPatSelectMinChars);
			this.groupPatientSelect.Controls.Add(this.textPatientSelectSearchMinChars);
			this.groupPatientSelect.Location = new System.Drawing.Point(6, 374);
			this.groupPatientSelect.Name = "groupPatientSelect";
			this.groupPatientSelect.Size = new System.Drawing.Size(482, 214);
			this.groupPatientSelect.TabIndex = 310;
			this.groupPatientSelect.Text = "Patient Select";
			// 
			// checkEnterpriseAllowRefreshWhileTyping
			// 
			this.checkEnterpriseAllowRefreshWhileTyping.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseAllowRefreshWhileTyping.Location = new System.Drawing.Point(5, 190);
			this.checkEnterpriseAllowRefreshWhileTyping.Name = "checkEnterpriseAllowRefreshWhileTyping";
			this.checkEnterpriseAllowRefreshWhileTyping.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkEnterpriseAllowRefreshWhileTyping.Size = new System.Drawing.Size(461, 18);
			this.checkEnterpriseAllowRefreshWhileTyping.TabIndex = 292;
			this.checkEnterpriseAllowRefreshWhileTyping.Text = "Allow Refresh While Typing in Select Patient Window";
			this.checkEnterpriseAllowRefreshWhileTyping.Click += new System.EventHandler(this.checkEnterpriseAllowRefreshWhileTyping_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(332, 167);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 18);
			this.label2.TabIndex = 294;
			this.label2.Text = "# of digits";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEnterpriseExactMatchPhoneNumDigits
			// 
			this.textEnterpriseExactMatchPhoneNumDigits.Enabled = false;
			this.textEnterpriseExactMatchPhoneNumDigits.Location = new System.Drawing.Point(427, 167);
			this.textEnterpriseExactMatchPhoneNumDigits.MaxVal = 25;
			this.textEnterpriseExactMatchPhoneNumDigits.MinVal = 1;
			this.textEnterpriseExactMatchPhoneNumDigits.Name = "textEnterpriseExactMatchPhoneNumDigits";
			this.textEnterpriseExactMatchPhoneNumDigits.Size = new System.Drawing.Size(40, 20);
			this.textEnterpriseExactMatchPhoneNumDigits.TabIndex = 293;
			this.textEnterpriseExactMatchPhoneNumDigits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkEnterpriseExactMatchPhone
			// 
			this.checkEnterpriseExactMatchPhone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseExactMatchPhone.Location = new System.Drawing.Point(130, 145);
			this.checkEnterpriseExactMatchPhone.Name = "checkEnterpriseExactMatchPhone";
			this.checkEnterpriseExactMatchPhone.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkEnterpriseExactMatchPhone.Size = new System.Drawing.Size(337, 18);
			this.checkEnterpriseExactMatchPhone.TabIndex = 292;
			this.checkEnterpriseExactMatchPhone.Text = "Only match patients by exact phone number";
			this.checkEnterpriseExactMatchPhone.CheckedChanged += new System.EventHandler(this.checkExactMatchPhoneNum_CheckedChanged);
			// 
			// checkPatientSelectFilterRestrictedClinics
			// 
			this.checkPatientSelectFilterRestrictedClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSelectFilterRestrictedClinics.Location = new System.Drawing.Point(6, 125);
			this.checkPatientSelectFilterRestrictedClinics.Name = "checkPatientSelectFilterRestrictedClinics";
			this.checkPatientSelectFilterRestrictedClinics.Size = new System.Drawing.Size(461, 18);
			this.checkPatientSelectFilterRestrictedClinics.TabIndex = 290;
			this.checkPatientSelectFilterRestrictedClinics.Text = "Hide patients from restricted clinics when viewing \"All\" clinics";
			this.checkPatientSelectFilterRestrictedClinics.Click += new System.EventHandler(this.checkPatientSelectFilterRestrictedClinics_Click);
			// 
			// butSyncPhNums
			// 
			this.butSyncPhNums.Location = new System.Drawing.Point(418, 100);
			this.butSyncPhNums.Name = "butSyncPhNums";
			this.butSyncPhNums.Size = new System.Drawing.Size(49, 21);
			this.butSyncPhNums.TabIndex = 292;
			this.butSyncPhNums.Text = "Sync";
			this.butSyncPhNums.Click += new System.EventHandler(this.butSyncPhNums_Click);
			// 
			// checkPatientPhoneUsePhonenumberTable
			// 
			this.checkPatientPhoneUsePhonenumberTable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientPhoneUsePhonenumberTable.Location = new System.Drawing.Point(6, 102);
			this.checkPatientPhoneUsePhonenumberTable.Name = "checkPatientPhoneUsePhonenumberTable";
			this.checkPatientPhoneUsePhonenumberTable.Size = new System.Drawing.Size(406, 17);
			this.checkPatientPhoneUsePhonenumberTable.TabIndex = 291;
			this.checkPatientPhoneUsePhonenumberTable.Text = "Store patient phone numbers in a separate table for patient search";
			this.checkPatientPhoneUsePhonenumberTable.Click += new System.EventHandler(this.checkPatientPhoneUsePhoneNumberTable_Click);
			// 
			// checkPatientSelectSearchWithEmptyParams
			// 
			this.checkPatientSelectSearchWithEmptyParams.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSelectSearchWithEmptyParams.Checked = true;
			this.checkPatientSelectSearchWithEmptyParams.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.checkPatientSelectSearchWithEmptyParams.Location = new System.Drawing.Point(6, 79);
			this.checkPatientSelectSearchWithEmptyParams.Name = "checkPatientSelectSearchWithEmptyParams";
			this.checkPatientSelectSearchWithEmptyParams.Size = new System.Drawing.Size(461, 17);
			this.checkPatientSelectSearchWithEmptyParams.TabIndex = 290;
			this.checkPatientSelectSearchWithEmptyParams.Text = "Search and fill grid with all empty search fields";
			// 
			// labelPatSelectPauseMs
			// 
			this.labelPatSelectPauseMs.Location = new System.Drawing.Point(6, 52);
			this.labelPatSelectPauseMs.Name = "labelPatSelectPauseMs";
			this.labelPatSelectPauseMs.Size = new System.Drawing.Size(420, 18);
			this.labelPatSelectPauseMs.TabIndex = 281;
			this.labelPatSelectPauseMs.Text = "Wait time in milliseconds after a character is entered before grid is filled";
			this.labelPatSelectPauseMs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientSelectSearchPauseMs
			// 
			this.textPatientSelectSearchPauseMs.Location = new System.Drawing.Point(427, 50);
			this.textPatientSelectSearchPauseMs.MaxVal = 10000;
			this.textPatientSelectSearchPauseMs.MinVal = 1;
			this.textPatientSelectSearchPauseMs.Name = "textPatientSelectSearchPauseMs";
			this.textPatientSelectSearchPauseMs.Size = new System.Drawing.Size(40, 20);
			this.textPatientSelectSearchPauseMs.TabIndex = 280;
			this.textPatientSelectSearchPauseMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPatSelectMinChars
			// 
			this.labelPatSelectMinChars.Location = new System.Drawing.Point(6, 19);
			this.labelPatSelectMinChars.Name = "labelPatSelectMinChars";
			this.labelPatSelectMinChars.Size = new System.Drawing.Size(420, 18);
			this.labelPatSelectMinChars.TabIndex = 279;
			this.labelPatSelectMinChars.Text = "The number of characters entered into the search fields before filling the grid";
			this.labelPatSelectMinChars.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientSelectSearchMinChars
			// 
			this.textPatientSelectSearchMinChars.Location = new System.Drawing.Point(427, 17);
			this.textPatientSelectSearchMinChars.MaxVal = 10;
			this.textPatientSelectSearchMinChars.MinVal = 1;
			this.textPatientSelectSearchMinChars.Name = "textPatientSelectSearchMinChars";
			this.textPatientSelectSearchMinChars.Size = new System.Drawing.Size(40, 20);
			this.textPatientSelectSearchMinChars.TabIndex = 278;
			this.textPatientSelectSearchMinChars.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkUserNameManualEntry
			// 
			this.checkUserNameManualEntry.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUserNameManualEntry.Location = new System.Drawing.Point(198, 111);
			this.checkUserNameManualEntry.Name = "checkUserNameManualEntry";
			this.checkUserNameManualEntry.Size = new System.Drawing.Size(275, 18);
			this.checkUserNameManualEntry.TabIndex = 309;
			this.checkUserNameManualEntry.Text = "Manually enter log on credentials";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 347);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(392, 18);
			this.label3.TabIndex = 298;
			this.label3.Text = "Process signal interval in seconds.  Leave blank to disable autorefresh";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUpdateStreamlinePassword
			// 
			this.checkUpdateStreamlinePassword.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUpdateStreamlinePassword.Enabled = false;
			this.checkUpdateStreamlinePassword.Location = new System.Drawing.Point(252, 276);
			this.checkUpdateStreamlinePassword.Name = "checkUpdateStreamlinePassword";
			this.checkUpdateStreamlinePassword.Size = new System.Drawing.Size(221, 18);
			this.checkUpdateStreamlinePassword.TabIndex = 307;
			this.checkUpdateStreamlinePassword.Text = "Update Streamline Password";
			// 
			// textProcessSigsIntervalInSecs
			// 
			this.textProcessSigsIntervalInSecs.Location = new System.Drawing.Point(399, 345);
			this.textProcessSigsIntervalInSecs.MaxVal = 1000000;
			this.textProcessSigsIntervalInSecs.MinVal = 1;
			this.textProcessSigsIntervalInSecs.Name = "textProcessSigsIntervalInSecs";
			this.textProcessSigsIntervalInSecs.ShowZero = false;
			this.textProcessSigsIntervalInSecs.Size = new System.Drawing.Size(74, 20);
			this.textProcessSigsIntervalInSecs.TabIndex = 300;
			this.textProcessSigsIntervalInSecs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textProcessSigsIntervalInSecs.Validating += new System.ComponentModel.CancelEventHandler(this.textProcessSigsIntervalInSecs_Validating);
			// 
			// textSecurityLogOffAfterMinutes
			// 
			this.textSecurityLogOffAfterMinutes.Location = new System.Drawing.Point(444, 88);
			this.textSecurityLogOffAfterMinutes.Name = "textSecurityLogOffAfterMinutes";
			this.textSecurityLogOffAfterMinutes.Size = new System.Drawing.Size(29, 20);
			this.textSecurityLogOffAfterMinutes.TabIndex = 306;
			this.textSecurityLogOffAfterMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkDatabaseMaintenanceSkipCheckTable
			// 
			this.checkDatabaseMaintenanceSkipCheckTable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDatabaseMaintenanceSkipCheckTable.Enabled = false;
			this.checkDatabaseMaintenanceSkipCheckTable.Location = new System.Drawing.Point(252, 240);
			this.checkDatabaseMaintenanceSkipCheckTable.Name = "checkDatabaseMaintenanceSkipCheckTable";
			this.checkDatabaseMaintenanceSkipCheckTable.Size = new System.Drawing.Size(221, 18);
			this.checkDatabaseMaintenanceSkipCheckTable.TabIndex = 295;
			this.checkDatabaseMaintenanceSkipCheckTable.Text = "DBM Skip Check Table";
			// 
			// checkDatabaseMaintenanceDisableOptimize
			// 
			this.checkDatabaseMaintenanceDisableOptimize.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDatabaseMaintenanceDisableOptimize.Enabled = false;
			this.checkDatabaseMaintenanceDisableOptimize.Location = new System.Drawing.Point(252, 222);
			this.checkDatabaseMaintenanceDisableOptimize.Name = "checkDatabaseMaintenanceDisableOptimize";
			this.checkDatabaseMaintenanceDisableOptimize.Size = new System.Drawing.Size(221, 18);
			this.checkDatabaseMaintenanceDisableOptimize.TabIndex = 294;
			this.checkDatabaseMaintenanceDisableOptimize.Text = "DBM Disable Optimize";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(182, 89);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(261, 18);
			this.label10.TabIndex = 308;
			this.label10.Text = "Automatic logoff time in minutes (0 to disable)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPasswordsMustBeStrong
			// 
			this.checkPasswordsMustBeStrong.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordsMustBeStrong.Location = new System.Drawing.Point(198, 14);
			this.checkPasswordsMustBeStrong.Name = "checkPasswordsMustBeStrong";
			this.checkPasswordsMustBeStrong.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordsMustBeStrong.TabIndex = 297;
			this.checkPasswordsMustBeStrong.Text = "Passwords must be strong";
			// 
			// textSignalInactiveMinutes
			// 
			this.textSignalInactiveMinutes.Location = new System.Drawing.Point(399, 316);
			this.textSignalInactiveMinutes.MaxVal = 1000000;
			this.textSignalInactiveMinutes.MinVal = 1;
			this.textSignalInactiveMinutes.Name = "textSignalInactiveMinutes";
			this.textSignalInactiveMinutes.ShowZero = false;
			this.textSignalInactiveMinutes.Size = new System.Drawing.Size(74, 20);
			this.textSignalInactiveMinutes.TabIndex = 304;
			this.textSignalInactiveMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textSignalInactiveMinutes.Validating += new System.ComponentModel.CancelEventHandler(this.textSignalInactiveMinutes_Validating);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(8, 318);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(392, 18);
			this.label12.TabIndex = 303;
			this.label12.Text = "Disable signal interval after this many minutes of user inactivity";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSecurityLockIncludesAdmin
			// 
			this.checkSecurityLockIncludesAdmin.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSecurityLockIncludesAdmin.Location = new System.Drawing.Point(249, 68);
			this.checkSecurityLockIncludesAdmin.Name = "checkSecurityLockIncludesAdmin";
			this.checkSecurityLockIncludesAdmin.Size = new System.Drawing.Size(224, 18);
			this.checkSecurityLockIncludesAdmin.TabIndex = 305;
			this.checkSecurityLockIncludesAdmin.Text = "Lock includes administrators";
			// 
			// checkPasswordsWeakChangeToStrong
			// 
			this.checkPasswordsWeakChangeToStrong.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordsWeakChangeToStrong.Location = new System.Drawing.Point(198, 50);
			this.checkPasswordsWeakChangeToStrong.Name = "checkPasswordsWeakChangeToStrong";
			this.checkPasswordsWeakChangeToStrong.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordsWeakChangeToStrong.TabIndex = 301;
			this.checkPasswordsWeakChangeToStrong.Text = "Force password change if not strong";
			// 
			// checkPasswordsStrongIncludeSpecial
			// 
			this.checkPasswordsStrongIncludeSpecial.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordsStrongIncludeSpecial.Location = new System.Drawing.Point(198, 32);
			this.checkPasswordsStrongIncludeSpecial.Name = "checkPasswordsStrongIncludeSpecial";
			this.checkPasswordsStrongIncludeSpecial.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordsStrongIncludeSpecial.TabIndex = 299;
			this.checkPasswordsStrongIncludeSpecial.Text = "Strong passwords require special character";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textSecurityLockDate);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.butChange);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.textSecurityLockDays);
			this.groupBox2.Controls.Add(this.labelGlobalDateLockDisabled);
			this.groupBox2.Location = new System.Drawing.Point(215, 133);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(273, 85);
			this.groupBox2.TabIndex = 302;
			this.groupBox2.Text = "Global Lock";
			// 
			// textSecurityLockDate
			// 
			this.textSecurityLockDate.Location = new System.Drawing.Point(100, 17);
			this.textSecurityLockDate.Name = "textSecurityLockDate";
			this.textSecurityLockDate.ReadOnly = true;
			this.textSecurityLockDate.Size = new System.Drawing.Size(82, 20);
			this.textSecurityLockDate.TabIndex = 260;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 17);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(93, 18);
			this.label7.TabIndex = 259;
			this.label7.Text = "Lock Date";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(188, 24);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(70, 24);
			this.butChange.TabIndex = 258;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 38);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(93, 18);
			this.label8.TabIndex = 261;
			this.label8.Text = "Lock Days";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSecurityLockDays
			// 
			this.textSecurityLockDays.Location = new System.Drawing.Point(100, 38);
			this.textSecurityLockDays.Name = "textSecurityLockDays";
			this.textSecurityLockDays.ReadOnly = true;
			this.textSecurityLockDays.Size = new System.Drawing.Size(82, 20);
			this.textSecurityLockDays.TabIndex = 262;
			this.textSecurityLockDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelGlobalDateLockDisabled
			// 
			this.labelGlobalDateLockDisabled.Location = new System.Drawing.Point(6, 61);
			this.labelGlobalDateLockDisabled.Name = "labelGlobalDateLockDisabled";
			this.labelGlobalDateLockDisabled.Size = new System.Drawing.Size(252, 18);
			this.labelGlobalDateLockDisabled.TabIndex = 266;
			this.labelGlobalDateLockDisabled.Text = "(Disabled from Central Management Tool)";
			this.labelGlobalDateLockDisabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelGlobalDateLockDisabled.Visible = false;
			// 
			// checkHasClinicsEnabled
			// 
			this.checkHasClinicsEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHasClinicsEnabled.Enabled = false;
			this.checkHasClinicsEnabled.Location = new System.Drawing.Point(252, 258);
			this.checkHasClinicsEnabled.Name = "checkHasClinicsEnabled";
			this.checkHasClinicsEnabled.Size = new System.Drawing.Size(221, 18);
			this.checkHasClinicsEnabled.TabIndex = 296;
			this.checkHasClinicsEnabled.Text = "Clinics (multiple office locations)";
			// 
			// UserControlEnterpriseGeneral
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkDatabaseGlobalVariablesDontSet);
			this.Controls.Add(this.checkEnterpriseCommlogOmitDefaults);
			this.Controls.Add(this.checkEnableEmailAddressAutoComplete);
			this.Controls.Add(this.checkUpdateAlterLargeTablesDirectly);
			this.Controls.Add(this.groupPatientSelect);
			this.Controls.Add(this.checkUserNameManualEntry);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkUpdateStreamlinePassword);
			this.Controls.Add(this.textProcessSigsIntervalInSecs);
			this.Controls.Add(this.textSecurityLogOffAfterMinutes);
			this.Controls.Add(this.checkDatabaseMaintenanceSkipCheckTable);
			this.Controls.Add(this.checkDatabaseMaintenanceDisableOptimize);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.checkPasswordsMustBeStrong);
			this.Controls.Add(this.textSignalInactiveMinutes);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.checkSecurityLockIncludesAdmin);
			this.Controls.Add(this.checkPasswordsWeakChangeToStrong);
			this.Controls.Add(this.checkPasswordsStrongIncludeSpecial);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.checkHasClinicsEnabled);
			this.Name = "UserControlEnterpriseGeneral";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupPatientSelect.ResumeLayout(false);
			this.groupPatientSelect.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.CheckBox checkDatabaseGlobalVariablesDontSet;
		private UI.CheckBox checkEnterpriseCommlogOmitDefaults;
		private UI.CheckBox checkEnableEmailAddressAutoComplete;
		private UI.CheckBox checkUpdateAlterLargeTablesDirectly;
		private UI.GroupBox groupPatientSelect;
		private UI.CheckBox checkEnterpriseAllowRefreshWhileTyping;
		private System.Windows.Forms.Label label2;
		private ValidNum textEnterpriseExactMatchPhoneNumDigits;
		private UI.CheckBox checkEnterpriseExactMatchPhone;
		private UI.CheckBox checkPatientSelectFilterRestrictedClinics;
		private UI.Button butSyncPhNums;
		private UI.CheckBox checkPatientPhoneUsePhonenumberTable;
		private UI.CheckBox checkPatientSelectSearchWithEmptyParams;
		private System.Windows.Forms.Label labelPatSelectPauseMs;
		private ValidNum textPatientSelectSearchPauseMs;
		private System.Windows.Forms.Label labelPatSelectMinChars;
		private ValidNum textPatientSelectSearchMinChars;
		private UI.CheckBox checkUserNameManualEntry;
		private System.Windows.Forms.Label label3;
		private UI.CheckBox checkUpdateStreamlinePassword;
		private ValidNum textProcessSigsIntervalInSecs;
		private System.Windows.Forms.TextBox textSecurityLogOffAfterMinutes;
		private UI.CheckBox checkDatabaseMaintenanceSkipCheckTable;
		private UI.CheckBox checkDatabaseMaintenanceDisableOptimize;
		private System.Windows.Forms.Label label10;
		private UI.CheckBox checkPasswordsMustBeStrong;
		private ValidNum textSignalInactiveMinutes;
		private System.Windows.Forms.Label label12;
		private UI.CheckBox checkSecurityLockIncludesAdmin;
		private UI.CheckBox checkPasswordsWeakChangeToStrong;
		private UI.CheckBox checkPasswordsStrongIncludeSpecial;
		private UI.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textSecurityLockDate;
		private System.Windows.Forms.Label label7;
		private UI.Button butChange;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textSecurityLockDays;
		private System.Windows.Forms.Label labelGlobalDateLockDisabled;
		private UI.CheckBox checkHasClinicsEnabled;
	}
}
