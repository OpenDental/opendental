namespace OpenDental {
	partial class FormEnterpriseSetup {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEnterpriseSetup));
			this.butCancel = new OpenDental.UI.Button();
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabAccount = new System.Windows.Forms.TabPage();
			this.label13 = new System.Windows.Forms.Label();
			this.checkBillingShowProgress = new System.Windows.Forms.CheckBox();
			this.label27 = new System.Windows.Forms.Label();
			this.checkAgingShowPayplanPayments = new System.Windows.Forms.CheckBox();
			this.textBillingElectBatchMax = new OpenDental.ValidNum();
			this.comboPayPlansVersion = new System.Windows.Forms.ComboBox();
			this.checkReceiveReportsService = new System.Windows.Forms.CheckBox();
			this.comboRigorousAdjustments = new System.Windows.Forms.ComboBox();
			this.checkHidePaysplits = new System.Windows.Forms.CheckBox();
			this.checkAgingEnterprise = new System.Windows.Forms.CheckBox();
			this.textReportCheckInterval = new System.Windows.Forms.TextBox();
			this.label41 = new System.Windows.Forms.Label();
			this.labelAgingServiceTimeDue = new System.Windows.Forms.Label();
			this.comboRigorousAccounting = new System.Windows.Forms.ComboBox();
			this.label39 = new System.Windows.Forms.Label();
			this.checkBillShowTransSinceZero = new System.Windows.Forms.CheckBox();
			this.labelReportheckUnits = new System.Windows.Forms.Label();
			this.validDateAgingServiceTimeDue = new OpenDental.ValidDate();
			this.radioInterval = new System.Windows.Forms.RadioButton();
			this.checkAgingMonthly = new System.Windows.Forms.CheckBox();
			this.comboPaymentClinicSetting = new System.Windows.Forms.ComboBox();
			this.radioTime = new System.Windows.Forms.RadioButton();
			this.checkPaymentsPromptForPayType = new System.Windows.Forms.CheckBox();
			this.textReportCheckTime = new OpenDental.ValidTime();
			this.label38 = new System.Windows.Forms.Label();
			this.groupBoxClaimIdPrefix = new System.Windows.Forms.GroupBox();
			this.butReplacements = new OpenDental.UI.Button();
			this.textClaimIdentifier = new System.Windows.Forms.TextBox();
			this.tabAdvanced = new System.Windows.Forms.TabPage();
			this.checkEnableEmailAddressAutoComplete = new System.Windows.Forms.CheckBox();
			this.checkUpdateAlterLargeTablesDirectly = new System.Windows.Forms.CheckBox();
			this.groupPatientSelect = new System.Windows.Forms.GroupBox();
			this.checkEnterpriseAllowRefresh = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textPhoneNumDigits = new OpenDental.ValidNum();
			this.checkMatchExactPhoneNum = new System.Windows.Forms.CheckBox();
			this.checkPatientSelectFilterRestrictedClinics = new System.Windows.Forms.CheckBox();
			this.butSyncPhNums = new OpenDental.UI.Button();
			this.checkUsePhoneNumTable = new System.Windows.Forms.CheckBox();
			this.checkPatSearchEmptyParams = new System.Windows.Forms.CheckBox();
			this.labelPatSelectPauseMs = new System.Windows.Forms.Label();
			this.textPatSelectPauseMs = new OpenDental.ValidNum();
			this.labelPatSelectMinChars = new System.Windows.Forms.Label();
			this.textPatSelectMinChars = new OpenDental.ValidNum();
			this.checkUserNameManualEntry = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkUpdateStreamlinePassword = new System.Windows.Forms.CheckBox();
			this.textSigInterval = new OpenDental.ValidNum();
			this.textLogOffAfterMinutes = new System.Windows.Forms.TextBox();
			this.checkDBMSkipCheckTable = new System.Windows.Forms.CheckBox();
			this.checkDBMDisableOptimize = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.checkPasswordsMustBeStrong = new System.Windows.Forms.CheckBox();
			this.textInactiveSignal = new OpenDental.ValidNum();
			this.label12 = new System.Windows.Forms.Label();
			this.checkLockIncludesAdmin = new System.Windows.Forms.CheckBox();
			this.checkPasswordForceWeakToStrong = new System.Windows.Forms.CheckBox();
			this.checkPasswordsStrongIncludeSpecial = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textDateLock = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butChange = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.textDaysLock = new System.Windows.Forms.TextBox();
			this.labelGlobalDateLockDisabled = new System.Windows.Forms.Label();
			this.checkEnableClinics = new System.Windows.Forms.CheckBox();
			this.tabAppts = new System.Windows.Forms.TabPage();
			this.checkEnableNoneView = new System.Windows.Forms.CheckBox();
			this.checkEnterpriseApptList = new System.Windows.Forms.CheckBox();
			this.checkUseOpHygProv = new System.Windows.Forms.CheckBox();
			this.checkApptsRequireProcs = new System.Windows.Forms.CheckBox();
			this.tabFamily = new System.Windows.Forms.TabPage();
			this.checkShowFeeSchedGroups = new System.Windows.Forms.CheckBox();
			this.groupClaimSnapshot = new System.Windows.Forms.GroupBox();
			this.textClaimSnapshotRunTime = new System.Windows.Forms.TextBox();
			this.comboClaimSnapshotTrigger = new System.Windows.Forms.ComboBox();
			this.labelClaimSnapshotTrigger = new System.Windows.Forms.Label();
			this.labelClaimSnapshotRunTime = new System.Windows.Forms.Label();
			this.checkPatClone = new System.Windows.Forms.CheckBox();
			this.checkSuperFam = new System.Windows.Forms.CheckBox();
			this.checkClaimSnapshotEnabled = new System.Windows.Forms.CheckBox();
			this.checkSuperFamCloneCreate = new System.Windows.Forms.CheckBox();
			this.tabReport = new System.Windows.Forms.TabPage();
			this.checkUseReportServer = new System.Windows.Forms.CheckBox();
			this.radioReportServerMiddleTier = new System.Windows.Forms.RadioButton();
			this.radioReportServerDirect = new System.Windows.Forms.RadioButton();
			this.groupConnectionSettings = new System.Windows.Forms.GroupBox();
			this.textServerName = new System.Windows.Forms.TextBox();
			this.textMysqlPass = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textMysqlUser = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboDatabase = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupMiddleTier = new System.Windows.Forms.GroupBox();
			this.textMiddleTierURI = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.tabManage = new System.Windows.Forms.TabPage();
			this.checkEra835sStrictClaimMatching = new System.Windows.Forms.CheckBox();
			this.checkEra835sRefreshOnLoad = new System.Windows.Forms.CheckBox();
			this.checkEra835sShowStatusAndClinic = new System.Windows.Forms.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.checkCaclAgingBatchClaims = new System.Windows.Forms.CheckBox();
			this.tabControlMain.SuspendLayout();
			this.tabAccount.SuspendLayout();
			this.groupBoxClaimIdPrefix.SuspendLayout();
			this.tabAdvanced.SuspendLayout();
			this.groupPatientSelect.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabAppts.SuspendLayout();
			this.tabFamily.SuspendLayout();
			this.groupClaimSnapshot.SuspendLayout();
			this.tabReport.SuspendLayout();
			this.groupConnectionSettings.SuspendLayout();
			this.groupMiddleTier.SuspendLayout();
			this.tabManage.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(430, 631);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabAccount);
			this.tabControlMain.Controls.Add(this.tabAdvanced);
			this.tabControlMain.Controls.Add(this.tabAppts);
			this.tabControlMain.Controls.Add(this.tabFamily);
			this.tabControlMain.Controls.Add(this.tabReport);
			this.tabControlMain.Controls.Add(this.tabManage);
			this.tabControlMain.Location = new System.Drawing.Point(6, 6);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(502, 619);
			this.tabControlMain.TabIndex = 276;
			// 
			// tabAccount
			// 
			this.tabAccount.BackColor = System.Drawing.SystemColors.Window;
			this.tabAccount.Controls.Add(this.checkCaclAgingBatchClaims);
			this.tabAccount.Controls.Add(this.label13);
			this.tabAccount.Controls.Add(this.checkBillingShowProgress);
			this.tabAccount.Controls.Add(this.label27);
			this.tabAccount.Controls.Add(this.checkAgingShowPayplanPayments);
			this.tabAccount.Controls.Add(this.textBillingElectBatchMax);
			this.tabAccount.Controls.Add(this.comboPayPlansVersion);
			this.tabAccount.Controls.Add(this.checkReceiveReportsService);
			this.tabAccount.Controls.Add(this.comboRigorousAdjustments);
			this.tabAccount.Controls.Add(this.checkHidePaysplits);
			this.tabAccount.Controls.Add(this.checkAgingEnterprise);
			this.tabAccount.Controls.Add(this.textReportCheckInterval);
			this.tabAccount.Controls.Add(this.label41);
			this.tabAccount.Controls.Add(this.labelAgingServiceTimeDue);
			this.tabAccount.Controls.Add(this.comboRigorousAccounting);
			this.tabAccount.Controls.Add(this.label39);
			this.tabAccount.Controls.Add(this.checkBillShowTransSinceZero);
			this.tabAccount.Controls.Add(this.labelReportheckUnits);
			this.tabAccount.Controls.Add(this.validDateAgingServiceTimeDue);
			this.tabAccount.Controls.Add(this.radioInterval);
			this.tabAccount.Controls.Add(this.checkAgingMonthly);
			this.tabAccount.Controls.Add(this.comboPaymentClinicSetting);
			this.tabAccount.Controls.Add(this.radioTime);
			this.tabAccount.Controls.Add(this.checkPaymentsPromptForPayType);
			this.tabAccount.Controls.Add(this.textReportCheckTime);
			this.tabAccount.Controls.Add(this.label38);
			this.tabAccount.Controls.Add(this.groupBoxClaimIdPrefix);
			this.tabAccount.Location = new System.Drawing.Point(4, 22);
			this.tabAccount.Name = "tabAccount";
			this.tabAccount.Padding = new System.Windows.Forms.Padding(3);
			this.tabAccount.Size = new System.Drawing.Size(494, 593);
			this.tabAccount.TabIndex = 2;
			this.tabAccount.Text = "Account";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(96, 415);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(316, 18);
			this.label13.TabIndex = 277;
			this.label13.Text = "Max number of statements per batch (0 for no limit)";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBillingShowProgress
			// 
			this.checkBillingShowProgress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBillingShowProgress.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBillingShowProgress.Location = new System.Drawing.Point(177, 437);
			this.checkBillingShowProgress.Name = "checkBillingShowProgress";
			this.checkBillingShowProgress.Size = new System.Drawing.Size(296, 18);
			this.checkBillingShowProgress.TabIndex = 250;
			this.checkBillingShowProgress.Text = "Show progress when sending statements";
			this.checkBillingShowProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(31, 388);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(238, 18);
			this.label27.TabIndex = 257;
			this.label27.Text = "Pay Plan Charge Logic";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAgingShowPayplanPayments
			// 
			this.checkAgingShowPayplanPayments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingShowPayplanPayments.Enabled = false;
			this.checkAgingShowPayplanPayments.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgingShowPayplanPayments.Location = new System.Drawing.Point(177, 62);
			this.checkAgingShowPayplanPayments.Name = "checkAgingShowPayplanPayments";
			this.checkAgingShowPayplanPayments.Size = new System.Drawing.Size(296, 18);
			this.checkAgingShowPayplanPayments.TabIndex = 60;
			this.checkAgingShowPayplanPayments.Text = "Aging Report Show Age Pat Payplan Payments";
			this.checkAgingShowPayplanPayments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBillingElectBatchMax
			// 
			this.textBillingElectBatchMax.Location = new System.Drawing.Point(413, 412);
			this.textBillingElectBatchMax.Name = "textBillingElectBatchMax";
			this.textBillingElectBatchMax.Size = new System.Drawing.Size(60, 20);
			this.textBillingElectBatchMax.TabIndex = 248;
			this.textBillingElectBatchMax.Text = "0";
			this.textBillingElectBatchMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// comboPayPlansVersion
			// 
			this.comboPayPlansVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPayPlansVersion.FormattingEnabled = true;
			this.comboPayPlansVersion.Location = new System.Drawing.Point(270, 388);
			this.comboPayPlansVersion.Name = "comboPayPlansVersion";
			this.comboPayPlansVersion.Size = new System.Drawing.Size(205, 21);
			this.comboPayPlansVersion.TabIndex = 256;
			// 
			// checkReceiveReportsService
			// 
			this.checkReceiveReportsService.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReceiveReportsService.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReceiveReportsService.Location = new System.Drawing.Point(177, 247);
			this.checkReceiveReportsService.Name = "checkReceiveReportsService";
			this.checkReceiveReportsService.Size = new System.Drawing.Size(296, 18);
			this.checkReceiveReportsService.TabIndex = 269;
			this.checkReceiveReportsService.TabStop = false;
			this.checkReceiveReportsService.Text = "Receive Reports by Service";
			this.checkReceiveReportsService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboRigorousAdjustments
			// 
			this.comboRigorousAdjustments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRigorousAdjustments.Location = new System.Drawing.Point(310, 343);
			this.comboRigorousAdjustments.MaxDropDownItems = 30;
			this.comboRigorousAdjustments.Name = "comboRigorousAdjustments";
			this.comboRigorousAdjustments.Size = new System.Drawing.Size(163, 21);
			this.comboRigorousAdjustments.TabIndex = 252;
			// 
			// checkHidePaysplits
			// 
			this.checkHidePaysplits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidePaysplits.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHidePaysplits.Location = new System.Drawing.Point(177, 367);
			this.checkHidePaysplits.Name = "checkHidePaysplits";
			this.checkHidePaysplits.Size = new System.Drawing.Size(296, 18);
			this.checkHidePaysplits.TabIndex = 248;
			this.checkHidePaysplits.Text = "Hide paysplits from payment window by default";
			this.checkHidePaysplits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidePaysplits.UseVisualStyleBackColor = true;
			// 
			// checkAgingEnterprise
			// 
			this.checkAgingEnterprise.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingEnterprise.Enabled = false;
			this.checkAgingEnterprise.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgingEnterprise.Location = new System.Drawing.Point(123, 44);
			this.checkAgingEnterprise.Name = "checkAgingEnterprise";
			this.checkAgingEnterprise.Size = new System.Drawing.Size(350, 18);
			this.checkAgingEnterprise.TabIndex = 59;
			this.checkAgingEnterprise.Text = "Aging for enterprise Galera cluster environments using FamAging Table";
			this.checkAgingEnterprise.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReportCheckInterval
			// 
			this.textReportCheckInterval.Location = new System.Drawing.Point(444, 265);
			this.textReportCheckInterval.MaxLength = 2147483647;
			this.textReportCheckInterval.Name = "textReportCheckInterval";
			this.textReportCheckInterval.Size = new System.Drawing.Size(29, 20);
			this.textReportCheckInterval.TabIndex = 267;
			// 
			// label41
			// 
			this.label41.Location = new System.Drawing.Point(75, 343);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(234, 18);
			this.label41.TabIndex = 251;
			this.label41.Text = "Enforce Valid Adjustments";
			this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAgingServiceTimeDue
			// 
			this.labelAgingServiceTimeDue.Location = new System.Drawing.Point(75, 83);
			this.labelAgingServiceTimeDue.Name = "labelAgingServiceTimeDue";
			this.labelAgingServiceTimeDue.Size = new System.Drawing.Size(234, 18);
			this.labelAgingServiceTimeDue.TabIndex = 259;
			this.labelAgingServiceTimeDue.Text = "Aging Service Time Due";
			this.labelAgingServiceTimeDue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboRigorousAccounting
			// 
			this.comboRigorousAccounting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRigorousAccounting.FormattingEnabled = true;
			this.comboRigorousAccounting.Location = new System.Drawing.Point(310, 317);
			this.comboRigorousAccounting.MaxDropDownItems = 30;
			this.comboRigorousAccounting.Name = "comboRigorousAccounting";
			this.comboRigorousAccounting.Size = new System.Drawing.Size(163, 21);
			this.comboRigorousAccounting.TabIndex = 250;
			// 
			// label39
			// 
			this.label39.Location = new System.Drawing.Point(75, 317);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(234, 18);
			this.label39.TabIndex = 249;
			this.label39.Text = "Enforce Valid Paysplits";
			this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBillShowTransSinceZero
			// 
			this.checkBillShowTransSinceZero.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBillShowTransSinceZero.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBillShowTransSinceZero.Location = new System.Drawing.Point(177, 148);
			this.checkBillShowTransSinceZero.Name = "checkBillShowTransSinceZero";
			this.checkBillShowTransSinceZero.Size = new System.Drawing.Size(296, 18);
			this.checkBillShowTransSinceZero.TabIndex = 255;
			this.checkBillShowTransSinceZero.Text = "Show all transactions since zero balance";
			this.checkBillShowTransSinceZero.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelReportheckUnits
			// 
			this.labelReportheckUnits.Location = new System.Drawing.Point(343, 268);
			this.labelReportheckUnits.Name = "labelReportheckUnits";
			this.labelReportheckUnits.Size = new System.Drawing.Size(100, 18);
			this.labelReportheckUnits.TabIndex = 268;
			this.labelReportheckUnits.Text = "minutes (5 to 60)";
			this.labelReportheckUnits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// validDateAgingServiceTimeDue
			// 
			this.validDateAgingServiceTimeDue.Location = new System.Drawing.Point(310, 80);
			this.validDateAgingServiceTimeDue.Name = "validDateAgingServiceTimeDue";
			this.validDateAgingServiceTimeDue.ReadOnly = true;
			this.validDateAgingServiceTimeDue.Size = new System.Drawing.Size(163, 20);
			this.validDateAgingServiceTimeDue.TabIndex = 260;
			// 
			// radioInterval
			// 
			this.radioInterval.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioInterval.Checked = true;
			this.radioInterval.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioInterval.Location = new System.Drawing.Point(156, 267);
			this.radioInterval.Name = "radioInterval";
			this.radioInterval.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioInterval.Size = new System.Drawing.Size(182, 18);
			this.radioInterval.TabIndex = 265;
			this.radioInterval.TabStop = true;
			this.radioInterval.Text = "Receive at an interval";
			this.radioInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioInterval.UseVisualStyleBackColor = true;
			this.radioInterval.CheckedChanged += new System.EventHandler(this.radioInterval_CheckedChanged);
			// 
			// checkAgingMonthly
			// 
			this.checkAgingMonthly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingMonthly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgingMonthly.Location = new System.Drawing.Point(177, 12);
			this.checkAgingMonthly.Name = "checkAgingMonthly";
			this.checkAgingMonthly.Size = new System.Drawing.Size(296, 18);
			this.checkAgingMonthly.TabIndex = 59;
			this.checkAgingMonthly.Text = "Aging calculated monthly instead of daily";
			this.checkAgingMonthly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaymentClinicSetting
			// 
			this.comboPaymentClinicSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPaymentClinicSetting.FormattingEnabled = true;
			this.comboPaymentClinicSetting.Location = new System.Drawing.Point(310, 104);
			this.comboPaymentClinicSetting.MaxDropDownItems = 30;
			this.comboPaymentClinicSetting.Name = "comboPaymentClinicSetting";
			this.comboPaymentClinicSetting.Size = new System.Drawing.Size(163, 21);
			this.comboPaymentClinicSetting.TabIndex = 239;
			// 
			// radioTime
			// 
			this.radioTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioTime.Location = new System.Drawing.Point(156, 292);
			this.radioTime.Name = "radioTime";
			this.radioTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioTime.Size = new System.Drawing.Size(182, 18);
			this.radioTime.TabIndex = 266;
			this.radioTime.Text = "Receive at a set time";
			this.radioTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioTime.UseVisualStyleBackColor = true;
			// 
			// checkPaymentsPromptForPayType
			// 
			this.checkPaymentsPromptForPayType.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPaymentsPromptForPayType.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPaymentsPromptForPayType.Location = new System.Drawing.Point(177, 130);
			this.checkPaymentsPromptForPayType.Name = "checkPaymentsPromptForPayType";
			this.checkPaymentsPromptForPayType.Size = new System.Drawing.Size(296, 18);
			this.checkPaymentsPromptForPayType.TabIndex = 230;
			this.checkPaymentsPromptForPayType.Text = "Payments prompt for Payment Type";
			this.checkPaymentsPromptForPayType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReportCheckTime
			// 
			this.textReportCheckTime.Enabled = false;
			this.textReportCheckTime.Location = new System.Drawing.Point(346, 292);
			this.textReportCheckTime.Name = "textReportCheckTime";
			this.textReportCheckTime.Size = new System.Drawing.Size(127, 20);
			this.textReportCheckTime.TabIndex = 264;
			// 
			// label38
			// 
			this.label38.Location = new System.Drawing.Point(75, 107);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(234, 18);
			this.label38.TabIndex = 240;
			this.label38.Text = "Patient Payments Use";
			this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxClaimIdPrefix
			// 
			this.groupBoxClaimIdPrefix.Controls.Add(this.butReplacements);
			this.groupBoxClaimIdPrefix.Controls.Add(this.textClaimIdentifier);
			this.groupBoxClaimIdPrefix.Location = new System.Drawing.Point(291, 172);
			this.groupBoxClaimIdPrefix.Name = "groupBoxClaimIdPrefix";
			this.groupBoxClaimIdPrefix.Size = new System.Drawing.Size(197, 71);
			this.groupBoxClaimIdPrefix.TabIndex = 256;
			this.groupBoxClaimIdPrefix.TabStop = false;
			this.groupBoxClaimIdPrefix.Text = "Claim Identification Prefix";
			// 
			// butReplacements
			// 
			this.butReplacements.Location = new System.Drawing.Point(97, 42);
			this.butReplacements.Name = "butReplacements";
			this.butReplacements.Size = new System.Drawing.Size(85, 23);
			this.butReplacements.TabIndex = 240;
			this.butReplacements.Text = "Replacements";
			this.butReplacements.UseVisualStyleBackColor = true;
			this.butReplacements.Click += new System.EventHandler(this.butReplacements_Click);
			// 
			// textClaimIdentifier
			// 
			this.textClaimIdentifier.Location = new System.Drawing.Point(15, 19);
			this.textClaimIdentifier.Name = "textClaimIdentifier";
			this.textClaimIdentifier.Size = new System.Drawing.Size(167, 20);
			this.textClaimIdentifier.TabIndex = 238;
			// 
			// tabAdvanced
			// 
			this.tabAdvanced.BackColor = System.Drawing.SystemColors.Window;
			this.tabAdvanced.Controls.Add(this.checkEnableEmailAddressAutoComplete);
			this.tabAdvanced.Controls.Add(this.checkUpdateAlterLargeTablesDirectly);
			this.tabAdvanced.Controls.Add(this.groupPatientSelect);
			this.tabAdvanced.Controls.Add(this.checkUserNameManualEntry);
			this.tabAdvanced.Controls.Add(this.label3);
			this.tabAdvanced.Controls.Add(this.checkUpdateStreamlinePassword);
			this.tabAdvanced.Controls.Add(this.textSigInterval);
			this.tabAdvanced.Controls.Add(this.textLogOffAfterMinutes);
			this.tabAdvanced.Controls.Add(this.checkDBMSkipCheckTable);
			this.tabAdvanced.Controls.Add(this.checkDBMDisableOptimize);
			this.tabAdvanced.Controls.Add(this.label10);
			this.tabAdvanced.Controls.Add(this.checkPasswordsMustBeStrong);
			this.tabAdvanced.Controls.Add(this.textInactiveSignal);
			this.tabAdvanced.Controls.Add(this.label12);
			this.tabAdvanced.Controls.Add(this.checkLockIncludesAdmin);
			this.tabAdvanced.Controls.Add(this.checkPasswordForceWeakToStrong);
			this.tabAdvanced.Controls.Add(this.checkPasswordsStrongIncludeSpecial);
			this.tabAdvanced.Controls.Add(this.groupBox2);
			this.tabAdvanced.Controls.Add(this.checkEnableClinics);
			this.tabAdvanced.Location = new System.Drawing.Point(4, 22);
			this.tabAdvanced.Name = "tabAdvanced";
			this.tabAdvanced.Padding = new System.Windows.Forms.Padding(3);
			this.tabAdvanced.Size = new System.Drawing.Size(494, 593);
			this.tabAdvanced.TabIndex = 4;
			this.tabAdvanced.Text = "Advanced";
			// 
			// checkEnableEmailAddressAutoComplete
			// 
			this.checkEnableEmailAddressAutoComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableEmailAddressAutoComplete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnableEmailAddressAutoComplete.Location = new System.Drawing.Point(12, 567);
			this.checkEnableEmailAddressAutoComplete.Name = "checkEnableEmailAddressAutoComplete";
			this.checkEnableEmailAddressAutoComplete.Size = new System.Drawing.Size(461, 18);
			this.checkEnableEmailAddressAutoComplete.TabIndex = 291;
			this.checkEnableEmailAddressAutoComplete.Text = "Enable email address auto-complete";
			this.checkEnableEmailAddressAutoComplete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUpdateAlterLargeTablesDirectly
			// 
			this.checkUpdateAlterLargeTablesDirectly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUpdateAlterLargeTablesDirectly.Enabled = false;
			this.checkUpdateAlterLargeTablesDirectly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUpdateAlterLargeTablesDirectly.Location = new System.Drawing.Point(252, 292);
			this.checkUpdateAlterLargeTablesDirectly.Name = "checkUpdateAlterLargeTablesDirectly";
			this.checkUpdateAlterLargeTablesDirectly.Size = new System.Drawing.Size(221, 18);
			this.checkUpdateAlterLargeTablesDirectly.TabIndex = 290;
			this.checkUpdateAlterLargeTablesDirectly.Text = "Update alter large tables directly";
			this.checkUpdateAlterLargeTablesDirectly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPatientSelect
			// 
			this.groupPatientSelect.Controls.Add(this.checkEnterpriseAllowRefresh);
			this.groupPatientSelect.Controls.Add(this.label2);
			this.groupPatientSelect.Controls.Add(this.textPhoneNumDigits);
			this.groupPatientSelect.Controls.Add(this.checkMatchExactPhoneNum);
			this.groupPatientSelect.Controls.Add(this.checkPatientSelectFilterRestrictedClinics);
			this.groupPatientSelect.Controls.Add(this.butSyncPhNums);
			this.groupPatientSelect.Controls.Add(this.checkUsePhoneNumTable);
			this.groupPatientSelect.Controls.Add(this.checkPatSearchEmptyParams);
			this.groupPatientSelect.Controls.Add(this.labelPatSelectPauseMs);
			this.groupPatientSelect.Controls.Add(this.textPatSelectPauseMs);
			this.groupPatientSelect.Controls.Add(this.labelPatSelectMinChars);
			this.groupPatientSelect.Controls.Add(this.textPatSelectMinChars);
			this.groupPatientSelect.Location = new System.Drawing.Point(6, 370);
			this.groupPatientSelect.Name = "groupPatientSelect";
			this.groupPatientSelect.Size = new System.Drawing.Size(482, 191);
			this.groupPatientSelect.TabIndex = 289;
			this.groupPatientSelect.TabStop = false;
			this.groupPatientSelect.Text = "Patient Select";
			// 
			// checkEnterpriseAllowRefresh
			// 
			this.checkEnterpriseAllowRefresh.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnterpriseAllowRefresh.Location = new System.Drawing.Point(6, 166);
			this.checkEnterpriseAllowRefresh.Name = "checkEnterpriseAllowRefresh";
			this.checkEnterpriseAllowRefresh.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkEnterpriseAllowRefresh.Size = new System.Drawing.Size(461, 18);
			this.checkEnterpriseAllowRefresh.TabIndex = 292;
			this.checkEnterpriseAllowRefresh.Text = "Allow Refresh While Typing in Select Patient Window";
			this.checkEnterpriseAllowRefresh.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(364, 142);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 18);
			this.label2.TabIndex = 294;
			this.label2.Text = "# of digits";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhoneNumDigits
			// 
			this.textPhoneNumDigits.Enabled = false;
			this.textPhoneNumDigits.Location = new System.Drawing.Point(426, 141);
			this.textPhoneNumDigits.MaxVal = 25;
			this.textPhoneNumDigits.MinVal = 1;
			this.textPhoneNumDigits.Name = "textPhoneNumDigits";
			this.textPhoneNumDigits.Size = new System.Drawing.Size(40, 20);
			this.textPhoneNumDigits.TabIndex = 293;
			this.textPhoneNumDigits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkMatchExactPhoneNum
			// 
			this.checkMatchExactPhoneNum.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMatchExactPhoneNum.Location = new System.Drawing.Point(6, 142);
			this.checkMatchExactPhoneNum.Name = "checkMatchExactPhoneNum";
			this.checkMatchExactPhoneNum.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkMatchExactPhoneNum.Size = new System.Drawing.Size(353, 18);
			this.checkMatchExactPhoneNum.TabIndex = 292;
			this.checkMatchExactPhoneNum.Text = "Only match patients by exact phone number";
			this.checkMatchExactPhoneNum.UseVisualStyleBackColor = true;
			this.checkMatchExactPhoneNum.CheckedChanged += new System.EventHandler(this.checkExactMatchPhoneNum_CheckedChanged);
			// 
			// checkPatientSelectFilterRestrictedClinics
			// 
			this.checkPatientSelectFilterRestrictedClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSelectFilterRestrictedClinics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientSelectFilterRestrictedClinics.Location = new System.Drawing.Point(6, 121);
			this.checkPatientSelectFilterRestrictedClinics.Name = "checkPatientSelectFilterRestrictedClinics";
			this.checkPatientSelectFilterRestrictedClinics.Size = new System.Drawing.Size(461, 18);
			this.checkPatientSelectFilterRestrictedClinics.TabIndex = 290;
			this.checkPatientSelectFilterRestrictedClinics.Text = "Hide patients from restricted clinics when viewing \"All\" clinics";
			this.checkPatientSelectFilterRestrictedClinics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSyncPhNums
			// 
			this.butSyncPhNums.Location = new System.Drawing.Point(418, 95);
			this.butSyncPhNums.Name = "butSyncPhNums";
			this.butSyncPhNums.Size = new System.Drawing.Size(49, 21);
			this.butSyncPhNums.TabIndex = 292;
			this.butSyncPhNums.Text = "Sync";
			this.butSyncPhNums.Click += new System.EventHandler(this.butSyncPhNums_Click);
			// 
			// checkUsePhoneNumTable
			// 
			this.checkUsePhoneNumTable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUsePhoneNumTable.Checked = true;
			this.checkUsePhoneNumTable.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.checkUsePhoneNumTable.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUsePhoneNumTable.Location = new System.Drawing.Point(6, 97);
			this.checkUsePhoneNumTable.Name = "checkUsePhoneNumTable";
			this.checkUsePhoneNumTable.Size = new System.Drawing.Size(406, 17);
			this.checkUsePhoneNumTable.TabIndex = 291;
			this.checkUsePhoneNumTable.Text = "Store patient phone numbers in a separate table for patient search";
			this.checkUsePhoneNumTable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPatSearchEmptyParams
			// 
			this.checkPatSearchEmptyParams.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatSearchEmptyParams.Checked = true;
			this.checkPatSearchEmptyParams.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.checkPatSearchEmptyParams.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatSearchEmptyParams.Location = new System.Drawing.Point(6, 74);
			this.checkPatSearchEmptyParams.Name = "checkPatSearchEmptyParams";
			this.checkPatSearchEmptyParams.Size = new System.Drawing.Size(461, 17);
			this.checkPatSearchEmptyParams.TabIndex = 290;
			this.checkPatSearchEmptyParams.Text = "Search and fill grid with all empty search fields";
			this.checkPatSearchEmptyParams.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPatSelectPauseMs
			// 
			this.labelPatSelectPauseMs.Location = new System.Drawing.Point(6, 44);
			this.labelPatSelectPauseMs.Name = "labelPatSelectPauseMs";
			this.labelPatSelectPauseMs.Size = new System.Drawing.Size(420, 26);
			this.labelPatSelectPauseMs.TabIndex = 281;
			this.labelPatSelectPauseMs.Text = "The number of milliseconds to wait after a character is entered before filling th" +
    "e grid\r\n1 to 10000 milliseconds, try starting with 1500";
			this.labelPatSelectPauseMs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatSelectPauseMs
			// 
			this.textPatSelectPauseMs.Location = new System.Drawing.Point(427, 47);
			this.textPatSelectPauseMs.MaxVal = 10000;
			this.textPatSelectPauseMs.MinVal = 1;
			this.textPatSelectPauseMs.Name = "textPatSelectPauseMs";
			this.textPatSelectPauseMs.Size = new System.Drawing.Size(40, 20);
			this.textPatSelectPauseMs.TabIndex = 280;
			this.textPatSelectPauseMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPatSelectMinChars
			// 
			this.labelPatSelectMinChars.Location = new System.Drawing.Point(6, 14);
			this.labelPatSelectMinChars.Name = "labelPatSelectMinChars";
			this.labelPatSelectMinChars.Size = new System.Drawing.Size(420, 26);
			this.labelPatSelectMinChars.TabIndex = 279;
			this.labelPatSelectMinChars.Text = "The number of characters entered into the search fields before filling the grid\r\n" +
    "1 to 10 characters, try starting with 3";
			this.labelPatSelectMinChars.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatSelectMinChars
			// 
			this.textPatSelectMinChars.Location = new System.Drawing.Point(427, 17);
			this.textPatSelectMinChars.MaxVal = 10;
			this.textPatSelectMinChars.MinVal = 1;
			this.textPatSelectMinChars.Name = "textPatSelectMinChars";
			this.textPatSelectMinChars.Size = new System.Drawing.Size(40, 20);
			this.textPatSelectMinChars.TabIndex = 278;
			this.textPatSelectMinChars.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkUserNameManualEntry
			// 
			this.checkUserNameManualEntry.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUserNameManualEntry.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUserNameManualEntry.Location = new System.Drawing.Point(198, 109);
			this.checkUserNameManualEntry.Name = "checkUserNameManualEntry";
			this.checkUserNameManualEntry.Size = new System.Drawing.Size(275, 18);
			this.checkUserNameManualEntry.TabIndex = 288;
			this.checkUserNameManualEntry.Text = "Manually enter log on credentials";
			this.checkUserNameManualEntry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 341);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(392, 25);
			this.label3.TabIndex = 282;
			this.label3.Text = "Process signal interval in seconds. Usually every 6 to 20 seconds\r\nLeave blank to" +
    " disable autorefresh";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUpdateStreamlinePassword
			// 
			this.checkUpdateStreamlinePassword.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUpdateStreamlinePassword.Enabled = false;
			this.checkUpdateStreamlinePassword.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUpdateStreamlinePassword.Location = new System.Drawing.Point(252, 274);
			this.checkUpdateStreamlinePassword.Name = "checkUpdateStreamlinePassword";
			this.checkUpdateStreamlinePassword.Size = new System.Drawing.Size(221, 18);
			this.checkUpdateStreamlinePassword.TabIndex = 287;
			this.checkUpdateStreamlinePassword.Text = "Update Streamline Password";
			this.checkUpdateStreamlinePassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSigInterval
			// 
			this.textSigInterval.Location = new System.Drawing.Point(399, 343);
			this.textSigInterval.MaxVal = 1000000;
			this.textSigInterval.MinVal = 1;
			this.textSigInterval.Name = "textSigInterval";
			this.textSigInterval.ShowZero = false;
			this.textSigInterval.Size = new System.Drawing.Size(74, 20);
			this.textSigInterval.TabIndex = 283;
			this.textSigInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textLogOffAfterMinutes
			// 
			this.textLogOffAfterMinutes.Location = new System.Drawing.Point(444, 86);
			this.textLogOffAfterMinutes.Name = "textLogOffAfterMinutes";
			this.textLogOffAfterMinutes.Size = new System.Drawing.Size(29, 20);
			this.textLogOffAfterMinutes.TabIndex = 286;
			this.textLogOffAfterMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkDBMSkipCheckTable
			// 
			this.checkDBMSkipCheckTable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDBMSkipCheckTable.Enabled = false;
			this.checkDBMSkipCheckTable.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDBMSkipCheckTable.Location = new System.Drawing.Point(252, 238);
			this.checkDBMSkipCheckTable.Name = "checkDBMSkipCheckTable";
			this.checkDBMSkipCheckTable.Size = new System.Drawing.Size(221, 18);
			this.checkDBMSkipCheckTable.TabIndex = 271;
			this.checkDBMSkipCheckTable.Text = "DBM Skip Check Table";
			this.checkDBMSkipCheckTable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDBMSkipCheckTable.UseVisualStyleBackColor = true;
			// 
			// checkDBMDisableOptimize
			// 
			this.checkDBMDisableOptimize.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDBMDisableOptimize.Enabled = false;
			this.checkDBMDisableOptimize.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDBMDisableOptimize.Location = new System.Drawing.Point(252, 220);
			this.checkDBMDisableOptimize.Name = "checkDBMDisableOptimize";
			this.checkDBMDisableOptimize.Size = new System.Drawing.Size(221, 18);
			this.checkDBMDisableOptimize.TabIndex = 270;
			this.checkDBMDisableOptimize.Text = "DBM Disable Optimize";
			this.checkDBMDisableOptimize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDBMDisableOptimize.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(198, 87);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(245, 18);
			this.label10.TabIndex = 287;
			this.label10.Text = "Automatic logoff time in minutes (0 to disable)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPasswordsMustBeStrong
			// 
			this.checkPasswordsMustBeStrong.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordsMustBeStrong.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPasswordsMustBeStrong.Location = new System.Drawing.Point(198, 12);
			this.checkPasswordsMustBeStrong.Name = "checkPasswordsMustBeStrong";
			this.checkPasswordsMustBeStrong.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordsMustBeStrong.TabIndex = 281;
			this.checkPasswordsMustBeStrong.Text = "Passwords must be strong";
			this.checkPasswordsMustBeStrong.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInactiveSignal
			// 
			this.textInactiveSignal.Location = new System.Drawing.Point(399, 314);
			this.textInactiveSignal.MaxVal = 1000000;
			this.textInactiveSignal.MinVal = 1;
			this.textInactiveSignal.Name = "textInactiveSignal";
			this.textInactiveSignal.ShowZero = false;
			this.textInactiveSignal.Size = new System.Drawing.Size(74, 20);
			this.textInactiveSignal.TabIndex = 285;
			this.textInactiveSignal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(6, 312);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(392, 25);
			this.label12.TabIndex = 284;
			this.label12.Text = "Disable signal interval after this many minutes of user inactivity\r\nLeave blank t" +
    "o disable";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkLockIncludesAdmin
			// 
			this.checkLockIncludesAdmin.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLockIncludesAdmin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLockIncludesAdmin.Location = new System.Drawing.Point(249, 66);
			this.checkLockIncludesAdmin.Name = "checkLockIncludesAdmin";
			this.checkLockIncludesAdmin.Size = new System.Drawing.Size(224, 18);
			this.checkLockIncludesAdmin.TabIndex = 285;
			this.checkLockIncludesAdmin.Text = "Lock includes administrators";
			this.checkLockIncludesAdmin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPasswordForceWeakToStrong
			// 
			this.checkPasswordForceWeakToStrong.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordForceWeakToStrong.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPasswordForceWeakToStrong.Location = new System.Drawing.Point(198, 48);
			this.checkPasswordForceWeakToStrong.Name = "checkPasswordForceWeakToStrong";
			this.checkPasswordForceWeakToStrong.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordForceWeakToStrong.TabIndex = 283;
			this.checkPasswordForceWeakToStrong.Text = "Force password change if not strong";
			this.checkPasswordForceWeakToStrong.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPasswordsStrongIncludeSpecial
			// 
			this.checkPasswordsStrongIncludeSpecial.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordsStrongIncludeSpecial.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPasswordsStrongIncludeSpecial.Location = new System.Drawing.Point(198, 30);
			this.checkPasswordsStrongIncludeSpecial.Name = "checkPasswordsStrongIncludeSpecial";
			this.checkPasswordsStrongIncludeSpecial.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordsStrongIncludeSpecial.TabIndex = 282;
			this.checkPasswordsStrongIncludeSpecial.Text = "Strong passwords require special character";
			this.checkPasswordsStrongIncludeSpecial.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textDateLock);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.butChange);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.textDaysLock);
			this.groupBox2.Controls.Add(this.labelGlobalDateLockDisabled);
			this.groupBox2.Location = new System.Drawing.Point(215, 131);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(273, 85);
			this.groupBox2.TabIndex = 284;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Global Lock";
			// 
			// textDateLock
			// 
			this.textDateLock.Location = new System.Drawing.Point(100, 17);
			this.textDateLock.Name = "textDateLock";
			this.textDateLock.ReadOnly = true;
			this.textDateLock.Size = new System.Drawing.Size(82, 20);
			this.textDateLock.TabIndex = 260;
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
			// textDaysLock
			// 
			this.textDaysLock.Location = new System.Drawing.Point(100, 38);
			this.textDaysLock.Name = "textDaysLock";
			this.textDaysLock.ReadOnly = true;
			this.textDaysLock.Size = new System.Drawing.Size(82, 20);
			this.textDaysLock.TabIndex = 262;
			this.textDaysLock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
			// checkEnableClinics
			// 
			this.checkEnableClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableClinics.Enabled = false;
			this.checkEnableClinics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnableClinics.Location = new System.Drawing.Point(252, 256);
			this.checkEnableClinics.Name = "checkEnableClinics";
			this.checkEnableClinics.Size = new System.Drawing.Size(221, 18);
			this.checkEnableClinics.TabIndex = 272;
			this.checkEnableClinics.Text = "Clinics (multiple office locations)";
			this.checkEnableClinics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabAppts
			// 
			this.tabAppts.BackColor = System.Drawing.SystemColors.Window;
			this.tabAppts.Controls.Add(this.checkEnableNoneView);
			this.tabAppts.Controls.Add(this.checkEnterpriseApptList);
			this.tabAppts.Controls.Add(this.checkUseOpHygProv);
			this.tabAppts.Controls.Add(this.checkApptsRequireProcs);
			this.tabAppts.Location = new System.Drawing.Point(4, 22);
			this.tabAppts.Name = "tabAppts";
			this.tabAppts.Padding = new System.Windows.Forms.Padding(3);
			this.tabAppts.Size = new System.Drawing.Size(494, 593);
			this.tabAppts.TabIndex = 0;
			this.tabAppts.Text = "Appts";
			// 
			// checkEnableNoneView
			// 
			this.checkEnableNoneView.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableNoneView.Location = new System.Drawing.Point(6, 64);
			this.checkEnableNoneView.Name = "checkEnableNoneView";
			this.checkEnableNoneView.Size = new System.Drawing.Size(468, 24);
			this.checkEnableNoneView.TabIndex = 287;
			this.checkEnableNoneView.Text = "Do not include \'None\' Appointment View when other views are available";
			this.checkEnableNoneView.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableNoneView.UseVisualStyleBackColor = true;
			// 
			// checkEnterpriseApptList
			// 
			this.checkEnterpriseApptList.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseApptList.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnterpriseApptList.Location = new System.Drawing.Point(153, 48);
			this.checkEnterpriseApptList.Name = "checkEnterpriseApptList";
			this.checkEnterpriseApptList.Size = new System.Drawing.Size(320, 18);
			this.checkEnterpriseApptList.TabIndex = 285;
			this.checkEnterpriseApptList.Text = "Enterprise Appointment Lists";
			this.checkEnterpriseApptList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUseOpHygProv
			// 
			this.checkUseOpHygProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseOpHygProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseOpHygProv.Location = new System.Drawing.Point(153, 30);
			this.checkUseOpHygProv.Name = "checkUseOpHygProv";
			this.checkUseOpHygProv.Size = new System.Drawing.Size(320, 18);
			this.checkUseOpHygProv.TabIndex = 284;
			this.checkUseOpHygProv.Text = "Force op\'s hygiene provider as secondary provider";
			this.checkUseOpHygProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkApptsRequireProcs
			// 
			this.checkApptsRequireProcs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptsRequireProcs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkApptsRequireProcs.Location = new System.Drawing.Point(153, 12);
			this.checkApptsRequireProcs.Name = "checkApptsRequireProcs";
			this.checkApptsRequireProcs.Size = new System.Drawing.Size(320, 18);
			this.checkApptsRequireProcs.TabIndex = 283;
			this.checkApptsRequireProcs.Text = "Appointments require procedures";
			this.checkApptsRequireProcs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabFamily
			// 
			this.tabFamily.BackColor = System.Drawing.SystemColors.Window;
			this.tabFamily.Controls.Add(this.checkShowFeeSchedGroups);
			this.tabFamily.Controls.Add(this.groupClaimSnapshot);
			this.tabFamily.Controls.Add(this.checkPatClone);
			this.tabFamily.Controls.Add(this.checkSuperFam);
			this.tabFamily.Controls.Add(this.checkClaimSnapshotEnabled);
			this.tabFamily.Controls.Add(this.checkSuperFamCloneCreate);
			this.tabFamily.Location = new System.Drawing.Point(4, 22);
			this.tabFamily.Name = "tabFamily";
			this.tabFamily.Padding = new System.Windows.Forms.Padding(3);
			this.tabFamily.Size = new System.Drawing.Size(494, 593);
			this.tabFamily.TabIndex = 1;
			this.tabFamily.Text = "Family";
			// 
			// checkShowFeeSchedGroups
			// 
			this.checkShowFeeSchedGroups.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowFeeSchedGroups.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowFeeSchedGroups.Location = new System.Drawing.Point(154, 84);
			this.checkShowFeeSchedGroups.Name = "checkShowFeeSchedGroups";
			this.checkShowFeeSchedGroups.Size = new System.Drawing.Size(319, 18);
			this.checkShowFeeSchedGroups.TabIndex = 281;
			this.checkShowFeeSchedGroups.Text = "Show Fee Schedule Groups";
			this.checkShowFeeSchedGroups.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupClaimSnapshot
			// 
			this.groupClaimSnapshot.Controls.Add(this.textClaimSnapshotRunTime);
			this.groupClaimSnapshot.Controls.Add(this.comboClaimSnapshotTrigger);
			this.groupClaimSnapshot.Controls.Add(this.labelClaimSnapshotTrigger);
			this.groupClaimSnapshot.Controls.Add(this.labelClaimSnapshotRunTime);
			this.groupClaimSnapshot.Location = new System.Drawing.Point(177, 108);
			this.groupClaimSnapshot.Name = "groupClaimSnapshot";
			this.groupClaimSnapshot.Size = new System.Drawing.Size(296, 73);
			this.groupClaimSnapshot.TabIndex = 280;
			this.groupClaimSnapshot.TabStop = false;
			this.groupClaimSnapshot.Text = "Claim Snapshot";
			// 
			// textClaimSnapshotRunTime
			// 
			this.textClaimSnapshotRunTime.Location = new System.Drawing.Point(194, 42);
			this.textClaimSnapshotRunTime.Name = "textClaimSnapshotRunTime";
			this.textClaimSnapshotRunTime.Size = new System.Drawing.Size(96, 20);
			this.textClaimSnapshotRunTime.TabIndex = 270;
			// 
			// comboClaimSnapshotTrigger
			// 
			this.comboClaimSnapshotTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClaimSnapshotTrigger.FormattingEnabled = true;
			this.comboClaimSnapshotTrigger.Location = new System.Drawing.Point(118, 17);
			this.comboClaimSnapshotTrigger.MaxDropDownItems = 30;
			this.comboClaimSnapshotTrigger.Name = "comboClaimSnapshotTrigger";
			this.comboClaimSnapshotTrigger.Size = new System.Drawing.Size(172, 21);
			this.comboClaimSnapshotTrigger.TabIndex = 269;
			// 
			// labelClaimSnapshotTrigger
			// 
			this.labelClaimSnapshotTrigger.Location = new System.Drawing.Point(17, 18);
			this.labelClaimSnapshotTrigger.Name = "labelClaimSnapshotTrigger";
			this.labelClaimSnapshotTrigger.Size = new System.Drawing.Size(100, 18);
			this.labelClaimSnapshotTrigger.TabIndex = 272;
			this.labelClaimSnapshotTrigger.Text = "Snapshot Trigger";
			this.labelClaimSnapshotTrigger.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClaimSnapshotRunTime
			// 
			this.labelClaimSnapshotRunTime.Location = new System.Drawing.Point(93, 43);
			this.labelClaimSnapshotRunTime.Name = "labelClaimSnapshotRunTime";
			this.labelClaimSnapshotRunTime.Size = new System.Drawing.Size(100, 18);
			this.labelClaimSnapshotRunTime.TabIndex = 271;
			this.labelClaimSnapshotRunTime.Text = "Service Run Time";
			this.labelClaimSnapshotRunTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPatClone
			// 
			this.checkPatClone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatClone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatClone.Location = new System.Drawing.Point(154, 30);
			this.checkPatClone.Name = "checkPatClone";
			this.checkPatClone.Size = new System.Drawing.Size(319, 18);
			this.checkPatClone.TabIndex = 279;
			this.checkPatClone.Text = "Patient Clone";
			this.checkPatClone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSuperFam
			// 
			this.checkSuperFam.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFam.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSuperFam.Location = new System.Drawing.Point(154, 12);
			this.checkSuperFam.Name = "checkSuperFam";
			this.checkSuperFam.Size = new System.Drawing.Size(319, 18);
			this.checkSuperFam.TabIndex = 271;
			this.checkSuperFam.Text = "Super Families";
			this.checkSuperFam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimSnapshotEnabled
			// 
			this.checkClaimSnapshotEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimSnapshotEnabled.Enabled = false;
			this.checkClaimSnapshotEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimSnapshotEnabled.Location = new System.Drawing.Point(154, 66);
			this.checkClaimSnapshotEnabled.Name = "checkClaimSnapshotEnabled";
			this.checkClaimSnapshotEnabled.Size = new System.Drawing.Size(319, 18);
			this.checkClaimSnapshotEnabled.TabIndex = 264;
			this.checkClaimSnapshotEnabled.Text = "Claim Snapshot Enabled";
			this.checkClaimSnapshotEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimSnapshotEnabled.UseVisualStyleBackColor = true;
			// 
			// checkSuperFamCloneCreate
			// 
			this.checkSuperFamCloneCreate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFamCloneCreate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSuperFamCloneCreate.Location = new System.Drawing.Point(154, 48);
			this.checkSuperFamCloneCreate.Name = "checkSuperFamCloneCreate";
			this.checkSuperFamCloneCreate.Size = new System.Drawing.Size(319, 18);
			this.checkSuperFamCloneCreate.TabIndex = 270;
			this.checkSuperFamCloneCreate.Text = "New patient clones use super family instead of regular family";
			this.checkSuperFamCloneCreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabReport
			// 
			this.tabReport.Controls.Add(this.checkUseReportServer);
			this.tabReport.Controls.Add(this.radioReportServerMiddleTier);
			this.tabReport.Controls.Add(this.radioReportServerDirect);
			this.tabReport.Controls.Add(this.groupConnectionSettings);
			this.tabReport.Controls.Add(this.groupMiddleTier);
			this.tabReport.Location = new System.Drawing.Point(4, 22);
			this.tabReport.Name = "tabReport";
			this.tabReport.Padding = new System.Windows.Forms.Padding(3);
			this.tabReport.Size = new System.Drawing.Size(494, 593);
			this.tabReport.TabIndex = 7;
			this.tabReport.Text = "Reports";
			this.tabReport.UseVisualStyleBackColor = true;
			// 
			// checkUseReportServer
			// 
			this.checkUseReportServer.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseReportServer.Location = new System.Drawing.Point(6, 12);
			this.checkUseReportServer.Name = "checkUseReportServer";
			this.checkUseReportServer.Size = new System.Drawing.Size(356, 18);
			this.checkUseReportServer.TabIndex = 233;
			this.checkUseReportServer.Text = "Use separate reporting server";
			this.checkUseReportServer.UseVisualStyleBackColor = true;
			this.checkUseReportServer.CheckedChanged += new System.EventHandler(this.checkUseReportServer_CheckedChanged);
			// 
			// radioReportServerMiddleTier
			// 
			this.radioReportServerMiddleTier.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioReportServerMiddleTier.Location = new System.Drawing.Point(6, 262);
			this.radioReportServerMiddleTier.Name = "radioReportServerMiddleTier";
			this.radioReportServerMiddleTier.Size = new System.Drawing.Size(356, 18);
			this.radioReportServerMiddleTier.TabIndex = 232;
			this.radioReportServerMiddleTier.Text = "Middle Tier";
			this.radioReportServerMiddleTier.UseVisualStyleBackColor = true;
			// 
			// radioReportServerDirect
			// 
			this.radioReportServerDirect.Checked = true;
			this.radioReportServerDirect.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioReportServerDirect.Location = new System.Drawing.Point(6, 36);
			this.radioReportServerDirect.Name = "radioReportServerDirect";
			this.radioReportServerDirect.Size = new System.Drawing.Size(356, 18);
			this.radioReportServerDirect.TabIndex = 231;
			this.radioReportServerDirect.TabStop = true;
			this.radioReportServerDirect.Text = "Direct Connection";
			this.radioReportServerDirect.UseVisualStyleBackColor = true;
			this.radioReportServerDirect.CheckedChanged += new System.EventHandler(this.RadioReportServerDirect_CheckedChanged);
			// 
			// groupConnectionSettings
			// 
			this.groupConnectionSettings.Controls.Add(this.textServerName);
			this.groupConnectionSettings.Controls.Add(this.textMysqlPass);
			this.groupConnectionSettings.Controls.Add(this.label6);
			this.groupConnectionSettings.Controls.Add(this.textMysqlUser);
			this.groupConnectionSettings.Controls.Add(this.label1);
			this.groupConnectionSettings.Controls.Add(this.comboDatabase);
			this.groupConnectionSettings.Controls.Add(this.label5);
			this.groupConnectionSettings.Controls.Add(this.label4);
			this.groupConnectionSettings.Location = new System.Drawing.Point(24, 56);
			this.groupConnectionSettings.Name = "groupConnectionSettings";
			this.groupConnectionSettings.Size = new System.Drawing.Size(455, 200);
			this.groupConnectionSettings.TabIndex = 230;
			this.groupConnectionSettings.TabStop = false;
			this.groupConnectionSettings.Text = "Connection Settings";
			// 
			// textServerName
			// 
			this.textServerName.Location = new System.Drawing.Point(6, 38);
			this.textServerName.Name = "textServerName";
			this.textServerName.Size = new System.Drawing.Size(443, 20);
			this.textServerName.TabIndex = 15;
			// 
			// textMysqlPass
			// 
			this.textMysqlPass.Location = new System.Drawing.Point(6, 174);
			this.textMysqlPass.Name = "textMysqlPass";
			this.textMysqlPass.PasswordChar = '*';
			this.textMysqlPass.Size = new System.Drawing.Size(443, 20);
			this.textMysqlPass.TabIndex = 223;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 155);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(443, 18);
			this.label6.TabIndex = 221;
			this.label6.Text = "MySQL Password:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textMysqlUser
			// 
			this.textMysqlUser.Location = new System.Drawing.Point(6, 129);
			this.textMysqlUser.Name = "textMysqlUser";
			this.textMysqlUser.Size = new System.Drawing.Size(443, 20);
			this.textMysqlUser.TabIndex = 222;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(443, 18);
			this.label1.TabIndex = 215;
			this.label1.Text = "Server Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboDatabase
			// 
			this.comboDatabase.FormattingEnabled = true;
			this.comboDatabase.Location = new System.Drawing.Point(6, 83);
			this.comboDatabase.Name = "comboDatabase";
			this.comboDatabase.Size = new System.Drawing.Size(443, 21);
			this.comboDatabase.TabIndex = 216;
			this.comboDatabase.DropDown += new System.EventHandler(this.ComboDatabase_DropDown);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 110);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(443, 18);
			this.label5.TabIndex = 219;
			this.label5.Text = "MySQL User:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(443, 18);
			this.label4.TabIndex = 217;
			this.label4.Text = "Database:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupMiddleTier
			// 
			this.groupMiddleTier.Controls.Add(this.textMiddleTierURI);
			this.groupMiddleTier.Controls.Add(this.label11);
			this.groupMiddleTier.Controls.Add(this.label9);
			this.groupMiddleTier.Location = new System.Drawing.Point(24, 282);
			this.groupMiddleTier.Name = "groupMiddleTier";
			this.groupMiddleTier.Size = new System.Drawing.Size(455, 92);
			this.groupMiddleTier.TabIndex = 229;
			this.groupMiddleTier.TabStop = false;
			this.groupMiddleTier.Text = "Middle Tier";
			// 
			// textMiddleTierURI
			// 
			this.textMiddleTierURI.Location = new System.Drawing.Point(6, 38);
			this.textMiddleTierURI.Name = "textMiddleTierURI";
			this.textMiddleTierURI.Size = new System.Drawing.Size(443, 20);
			this.textMiddleTierURI.TabIndex = 7;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(6, 60);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(443, 26);
			this.label11.TabIndex = 14;
			this.label11.Text = "The currently logged in user\'s credentials will be used to when accessing the Mid" +
    "dle Tier database.";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 19);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(443, 18);
			this.label9.TabIndex = 9;
			this.label9.Text = "URI";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tabManage
			// 
			this.tabManage.Controls.Add(this.checkEra835sStrictClaimMatching);
			this.tabManage.Controls.Add(this.checkEra835sRefreshOnLoad);
			this.tabManage.Controls.Add(this.checkEra835sShowStatusAndClinic);
			this.tabManage.Location = new System.Drawing.Point(4, 22);
			this.tabManage.Name = "tabManage";
			this.tabManage.Size = new System.Drawing.Size(494, 593);
			this.tabManage.TabIndex = 8;
			this.tabManage.Text = "Manage";
			this.tabManage.UseVisualStyleBackColor = true;
			// 
			// checkEra835sStrictClaimMatching
			// 
			this.checkEra835sStrictClaimMatching.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEra835sStrictClaimMatching.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEra835sStrictClaimMatching.Location = new System.Drawing.Point(154, 30);
			this.checkEra835sStrictClaimMatching.Name = "checkEra835sStrictClaimMatching";
			this.checkEra835sStrictClaimMatching.Size = new System.Drawing.Size(319, 18);
			this.checkEra835sStrictClaimMatching.TabIndex = 273;
			this.checkEra835sStrictClaimMatching.Text = "ERA 835s use strict claim date matching";
			this.checkEra835sStrictClaimMatching.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEra835sRefreshOnLoad
			// 
			this.checkEra835sRefreshOnLoad.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEra835sRefreshOnLoad.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEra835sRefreshOnLoad.Location = new System.Drawing.Point(154, 12);
			this.checkEra835sRefreshOnLoad.Name = "checkEra835sRefreshOnLoad";
			this.checkEra835sRefreshOnLoad.Size = new System.Drawing.Size(319, 18);
			this.checkEra835sRefreshOnLoad.TabIndex = 273;
			this.checkEra835sRefreshOnLoad.Text = "ERA 835s window refresh data on load";
			this.checkEra835sRefreshOnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEra835sShowStatusAndClinic
			// 
			this.checkEra835sShowStatusAndClinic.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEra835sShowStatusAndClinic.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEra835sShowStatusAndClinic.Location = new System.Drawing.Point(154, 48);
			this.checkEra835sShowStatusAndClinic.Name = "checkEra835sShowStatusAndClinic";
			this.checkEra835sShowStatusAndClinic.Size = new System.Drawing.Size(319, 18);
			this.checkEra835sShowStatusAndClinic.TabIndex = 274;
			this.checkEra835sShowStatusAndClinic.Text = "ERA 835s window show status and clinic information";
			this.checkEra835sShowStatusAndClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(349, 631);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkCaclAgingBatchClaims
			// 
			this.checkCaclAgingBatchClaims.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCaclAgingBatchClaims.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCaclAgingBatchClaims.Location = new System.Drawing.Point(177, 28);
			this.checkCaclAgingBatchClaims.Name = "checkCaclAgingBatchClaims";
			this.checkCaclAgingBatchClaims.Size = new System.Drawing.Size(296, 18);
			this.checkCaclAgingBatchClaims.TabIndex = 278;
			this.checkCaclAgingBatchClaims.Text = "Aging calculated on receipt of batch claim payments";
			this.checkCaclAgingBatchClaims.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEnterpriseSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(514, 667);
			this.Controls.Add(this.tabControlMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEnterpriseSetup";
			this.Text = "Enterprise Setup";
			this.Load += new System.EventHandler(this.FormEnterpriseSetup_Load);
			this.tabControlMain.ResumeLayout(false);
			this.tabAccount.ResumeLayout(false);
			this.tabAccount.PerformLayout();
			this.groupBoxClaimIdPrefix.ResumeLayout(false);
			this.groupBoxClaimIdPrefix.PerformLayout();
			this.tabAdvanced.ResumeLayout(false);
			this.tabAdvanced.PerformLayout();
			this.groupPatientSelect.ResumeLayout(false);
			this.groupPatientSelect.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabAppts.ResumeLayout(false);
			this.tabFamily.ResumeLayout(false);
			this.groupClaimSnapshot.ResumeLayout(false);
			this.groupClaimSnapshot.PerformLayout();
			this.tabReport.ResumeLayout(false);
			this.groupConnectionSettings.ResumeLayout(false);
			this.groupConnectionSettings.PerformLayout();
			this.groupMiddleTier.ResumeLayout(false);
			this.groupMiddleTier.PerformLayout();
			this.tabManage.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkAgingEnterprise;
		private System.Windows.Forms.CheckBox checkAgingShowPayplanPayments;
		private System.Windows.Forms.Label labelAgingServiceTimeDue;
		private ValidDate validDateAgingServiceTimeDue;
		private System.Windows.Forms.CheckBox checkClaimSnapshotEnabled;
		private System.Windows.Forms.CheckBox checkDBMDisableOptimize;
		private System.Windows.Forms.CheckBox checkDBMSkipCheckTable;
		private System.Windows.Forms.CheckBox checkEnableClinics;
		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabAppts;
		private System.Windows.Forms.TabPage tabFamily;
		private System.Windows.Forms.TabPage tabAccount;
		private System.Windows.Forms.TabPage tabAdvanced;
		private System.Windows.Forms.CheckBox checkPaymentsPromptForPayType;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.ComboBox comboPaymentClinicSetting;
		private System.Windows.Forms.CheckBox checkAgingMonthly;
		private System.Windows.Forms.GroupBox groupBoxClaimIdPrefix;
		private UI.Button butReplacements;
		private System.Windows.Forms.TextBox textClaimIdentifier;
		private System.Windows.Forms.RadioButton radioInterval;
		private System.Windows.Forms.RadioButton radioTime;
		private ValidTime textReportCheckTime;
		private System.Windows.Forms.CheckBox checkBillShowTransSinceZero;
		private System.Windows.Forms.Label labelClaimSnapshotTrigger;
		private System.Windows.Forms.Label labelClaimSnapshotRunTime;
		private System.Windows.Forms.ComboBox comboClaimSnapshotTrigger;
		private System.Windows.Forms.TextBox textClaimSnapshotRunTime;
		private System.Windows.Forms.TextBox textReportCheckInterval;
		private System.Windows.Forms.Label labelReportheckUnits;
		private System.Windows.Forms.CheckBox checkSuperFamCloneCreate;
		private System.Windows.Forms.CheckBox checkReceiveReportsService;
		private System.Windows.Forms.CheckBox checkHidePaysplits;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.ComboBox comboPayPlansVersion;
		private System.Windows.Forms.TabPage tabReport;
		private System.Windows.Forms.RadioButton radioReportServerMiddleTier;
		private System.Windows.Forms.RadioButton radioReportServerDirect;
		private System.Windows.Forms.GroupBox groupConnectionSettings;
		private System.Windows.Forms.TextBox textMysqlPass;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textMysqlUser;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboDatabase;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupMiddleTier;
		private System.Windows.Forms.TextBox textMiddleTierURI;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox comboRigorousAccounting;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.ComboBox comboRigorousAdjustments;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.CheckBox checkPatClone;
		private System.Windows.Forms.CheckBox checkSuperFam;
		private System.Windows.Forms.CheckBox checkUserNameManualEntry;
		private System.Windows.Forms.TextBox textLogOffAfterMinutes;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkLockIncludesAdmin;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textDateLock;
		private System.Windows.Forms.Label label7;
		private UI.Button butChange;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textDaysLock;
		private System.Windows.Forms.Label labelGlobalDateLockDisabled;
		private System.Windows.Forms.CheckBox checkPasswordsStrongIncludeSpecial;
		private System.Windows.Forms.CheckBox checkPasswordForceWeakToStrong;
		private System.Windows.Forms.CheckBox checkPasswordsMustBeStrong;
		private ValidNum textInactiveSignal;
		private System.Windows.Forms.Label label12;
		private ValidNum textSigInterval;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBillingShowProgress;
		private ValidNum textBillingElectBatchMax;
		private System.Windows.Forms.CheckBox checkEnterpriseApptList;
		private System.Windows.Forms.CheckBox checkUseOpHygProv;
		private System.Windows.Forms.CheckBox checkApptsRequireProcs;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox checkUpdateStreamlinePassword;
		private System.Windows.Forms.CheckBox checkUseReportServer;
		private System.Windows.Forms.TextBox textServerName;
		private System.Windows.Forms.GroupBox groupPatientSelect;
		private System.Windows.Forms.Label labelPatSelectPauseMs;
		private ValidNum textPatSelectPauseMs;
		private System.Windows.Forms.Label labelPatSelectMinChars;
		private ValidNum textPatSelectMinChars;
		private System.Windows.Forms.CheckBox checkPatSearchEmptyParams;
		private System.Windows.Forms.CheckBox checkEnableNoneView;
		private System.Windows.Forms.CheckBox checkUsePhoneNumTable;
		private UI.Button butSyncPhNums;
		private System.Windows.Forms.GroupBox groupClaimSnapshot;
		private System.Windows.Forms.CheckBox checkPatientSelectFilterRestrictedClinics;
		private System.Windows.Forms.CheckBox checkShowFeeSchedGroups;
		private System.Windows.Forms.CheckBox checkUpdateAlterLargeTablesDirectly;
		private System.Windows.Forms.CheckBox checkEnableEmailAddressAutoComplete;
		private System.Windows.Forms.CheckBox checkMatchExactPhoneNum;
		private ValidNum textPhoneNumDigits;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkEnterpriseAllowRefresh;
		private System.Windows.Forms.TabPage tabManage;
		private System.Windows.Forms.CheckBox checkEra835sRefreshOnLoad;
        private System.Windows.Forms.CheckBox checkEra835sStrictClaimMatching;
        private System.Windows.Forms.CheckBox checkEra835sShowStatusAndClinic;
		private System.Windows.Forms.CheckBox checkCaclAgingBatchClaims;
	}
}