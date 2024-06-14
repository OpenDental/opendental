
namespace OpenDental {
	partial class UserControlEnterpriseAccount {
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
			this.checkAgingCalculateOnBatchClaimReceipt = new OpenDental.UI.CheckBox();
			this.label13 = new System.Windows.Forms.Label();
			this.checkBillingShowSendProgress = new OpenDental.UI.CheckBox();
			this.label27 = new System.Windows.Forms.Label();
			this.checkAgingReportShowAgePatPayplanPayments = new OpenDental.UI.CheckBox();
			this.textBillingElectBatchMax = new OpenDental.ValidNum();
			this.comboPayPlansVersion = new OpenDental.UI.ComboBox();
			this.checkClaimReportReceivedByService = new OpenDental.UI.CheckBox();
			this.comboRigorousAdjustments = new OpenDental.UI.ComboBox();
			this.checkPaymentWindowDefaultHideSplits = new OpenDental.UI.CheckBox();
			this.textClaimReportReceiveInterval = new System.Windows.Forms.TextBox();
			this.label41 = new System.Windows.Forms.Label();
			this.labelAgingServiceTimeDue = new System.Windows.Forms.Label();
			this.comboRigorousAccounting = new OpenDental.UI.ComboBox();
			this.label39 = new System.Windows.Forms.Label();
			this.checkBillingShowTransSinceBalZero = new OpenDental.UI.CheckBox();
			this.labelReportheckUnits = new System.Windows.Forms.Label();
			this.textAgingServiceTimeDue = new OpenDental.ValidDate();
			this.radioReceiveAtAnInterval = new System.Windows.Forms.RadioButton();
			this.comboPaymentClinicSetting = new OpenDental.UI.ComboBox();
			this.radioReceiveAtASetTime = new System.Windows.Forms.RadioButton();
			this.checkPaymentsPromptForPayType = new OpenDental.UI.CheckBox();
			this.textClaimReportReceiveTime = new OpenDental.ValidTime();
			this.label38 = new System.Windows.Forms.Label();
			this.groupBoxClaimIdPrefix = new OpenDental.UI.GroupBox();
			this.butReplacements = new OpenDental.UI.Button();
			this.textClaimIdPrefix = new System.Windows.Forms.TextBox();
			this.groupBoxClaimIdPrefix.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkAgingCalculateOnBatchClaimReceipt
			// 
			this.checkAgingCalculateOnBatchClaimReceipt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingCalculateOnBatchClaimReceipt.Location = new System.Drawing.Point(157, 24);
			this.checkAgingCalculateOnBatchClaimReceipt.Name = "checkAgingCalculateOnBatchClaimReceipt";
			this.checkAgingCalculateOnBatchClaimReceipt.Size = new System.Drawing.Size(296, 18);
			this.checkAgingCalculateOnBatchClaimReceipt.TabIndex = 303;
			this.checkAgingCalculateOnBatchClaimReceipt.Text = "Aging calculated on receipt of batch claim payments";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(76, 396);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(316, 18);
			this.label13.TabIndex = 302;
			this.label13.Text = "Max number of statements per batch (0 for no limit)";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBillingShowSendProgress
			// 
			this.checkBillingShowSendProgress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBillingShowSendProgress.Location = new System.Drawing.Point(157, 418);
			this.checkBillingShowSendProgress.Name = "checkBillingShowSendProgress";
			this.checkBillingShowSendProgress.Size = new System.Drawing.Size(296, 18);
			this.checkBillingShowSendProgress.TabIndex = 286;
			this.checkBillingShowSendProgress.Text = "Show progress when sending statements";
			this.checkBillingShowSendProgress.Click += new System.EventHandler(this.checkBillingShowSendProgress_Click);
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(11, 369);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(238, 18);
			this.label27.TabIndex = 293;
			this.label27.Text = "Pay Plan charge logic";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAgingReportShowAgePatPayplanPayments
			// 
			this.checkAgingReportShowAgePatPayplanPayments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingReportShowAgePatPayplanPayments.Enabled = false;
			this.checkAgingReportShowAgePatPayplanPayments.Location = new System.Drawing.Point(157, 43);
			this.checkAgingReportShowAgePatPayplanPayments.Name = "checkAgingReportShowAgePatPayplanPayments";
			this.checkAgingReportShowAgePatPayplanPayments.Size = new System.Drawing.Size(296, 18);
			this.checkAgingReportShowAgePatPayplanPayments.TabIndex = 279;
			this.checkAgingReportShowAgePatPayplanPayments.Text = "Aging Report Show Age Pat Payplan Payments";
			// 
			// textBillingElectBatchMax
			// 
			this.textBillingElectBatchMax.Location = new System.Drawing.Point(393, 395);
			this.textBillingElectBatchMax.Name = "textBillingElectBatchMax";
			this.textBillingElectBatchMax.Size = new System.Drawing.Size(60, 20);
			this.textBillingElectBatchMax.TabIndex = 283;
			this.textBillingElectBatchMax.Text = "0";
			this.textBillingElectBatchMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBillingElectBatchMax.Validating += new System.ComponentModel.CancelEventHandler(this.textBillingElectBatchMax_Validating);
			// 
			// comboPayPlansVersion
			// 
			this.comboPayPlansVersion.Location = new System.Drawing.Point(250, 369);
			this.comboPayPlansVersion.Name = "comboPayPlansVersion";
			this.comboPayPlansVersion.Size = new System.Drawing.Size(205, 21);
			this.comboPayPlansVersion.TabIndex = 292;
			this.comboPayPlansVersion.SelectionChangeCommitted += new System.EventHandler(this.comboPayPlansVersion_ChangeCommitted);
			// 
			// checkClaimReportReceivedByService
			// 
			this.checkClaimReportReceivedByService.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimReportReceivedByService.Location = new System.Drawing.Point(157, 228);
			this.checkClaimReportReceivedByService.Name = "checkClaimReportReceivedByService";
			this.checkClaimReportReceivedByService.Size = new System.Drawing.Size(296, 18);
			this.checkClaimReportReceivedByService.TabIndex = 301;
			this.checkClaimReportReceivedByService.TabStop = false;
			this.checkClaimReportReceivedByService.Text = "Receive Reports by Service";
			// 
			// comboRigorousAdjustments
			// 
			this.comboRigorousAdjustments.Location = new System.Drawing.Point(290, 324);
			this.comboRigorousAdjustments.Name = "comboRigorousAdjustments";
			this.comboRigorousAdjustments.Size = new System.Drawing.Size(163, 21);
			this.comboRigorousAdjustments.TabIndex = 289;
			// 
			// checkPaymentWindowDefaultHideSplits
			// 
			this.checkPaymentWindowDefaultHideSplits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPaymentWindowDefaultHideSplits.Location = new System.Drawing.Point(157, 348);
			this.checkPaymentWindowDefaultHideSplits.Name = "checkPaymentWindowDefaultHideSplits";
			this.checkPaymentWindowDefaultHideSplits.Size = new System.Drawing.Size(296, 18);
			this.checkPaymentWindowDefaultHideSplits.TabIndex = 284;
			this.checkPaymentWindowDefaultHideSplits.Text = "Hide paysplits from payment window by default";
			// 
			// textClaimReportReceiveInterval
			// 
			this.textClaimReportReceiveInterval.Location = new System.Drawing.Point(424, 248);
			this.textClaimReportReceiveInterval.MaxLength = 2147483647;
			this.textClaimReportReceiveInterval.Name = "textClaimReportReceiveInterval";
			this.textClaimReportReceiveInterval.Size = new System.Drawing.Size(29, 20);
			this.textClaimReportReceiveInterval.TabIndex = 299;
			// 
			// label41
			// 
			this.label41.Location = new System.Drawing.Point(55, 324);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(234, 18);
			this.label41.TabIndex = 288;
			this.label41.Text = "Adjustments Allocation";
			this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAgingServiceTimeDue
			// 
			this.labelAgingServiceTimeDue.Location = new System.Drawing.Point(55, 64);
			this.labelAgingServiceTimeDue.Name = "labelAgingServiceTimeDue";
			this.labelAgingServiceTimeDue.Size = new System.Drawing.Size(234, 18);
			this.labelAgingServiceTimeDue.TabIndex = 294;
			this.labelAgingServiceTimeDue.Text = "Aging Service Time Due";
			this.labelAgingServiceTimeDue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboRigorousAccounting
			// 
			this.comboRigorousAccounting.Location = new System.Drawing.Point(290, 298);
			this.comboRigorousAccounting.Name = "comboRigorousAccounting";
			this.comboRigorousAccounting.Size = new System.Drawing.Size(163, 21);
			this.comboRigorousAccounting.TabIndex = 287;
			// 
			// label39
			// 
			this.label39.Location = new System.Drawing.Point(55, 298);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(234, 18);
			this.label39.TabIndex = 285;
			this.label39.Text = "Paysplits Allocation";
			this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBillingShowTransSinceBalZero
			// 
			this.checkBillingShowTransSinceBalZero.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBillingShowTransSinceBalZero.Location = new System.Drawing.Point(157, 129);
			this.checkBillingShowTransSinceBalZero.Name = "checkBillingShowTransSinceBalZero";
			this.checkBillingShowTransSinceBalZero.Size = new System.Drawing.Size(296, 18);
			this.checkBillingShowTransSinceBalZero.TabIndex = 290;
			this.checkBillingShowTransSinceBalZero.Text = "Show all transactions since zero balance";
			// 
			// labelReportheckUnits
			// 
			this.labelReportheckUnits.Location = new System.Drawing.Point(323, 249);
			this.labelReportheckUnits.Name = "labelReportheckUnits";
			this.labelReportheckUnits.Size = new System.Drawing.Size(100, 18);
			this.labelReportheckUnits.TabIndex = 300;
			this.labelReportheckUnits.Text = "minutes (5 to 60)";
			this.labelReportheckUnits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAgingServiceTimeDue
			// 
			this.textAgingServiceTimeDue.Location = new System.Drawing.Point(290, 63);
			this.textAgingServiceTimeDue.Name = "textAgingServiceTimeDue";
			this.textAgingServiceTimeDue.ReadOnly = true;
			this.textAgingServiceTimeDue.Size = new System.Drawing.Size(163, 20);
			this.textAgingServiceTimeDue.TabIndex = 295;
			// 
			// radioReceiveAtAnInterval
			// 
			this.radioReceiveAtAnInterval.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioReceiveAtAnInterval.Checked = true;
			this.radioReceiveAtAnInterval.Location = new System.Drawing.Point(136, 248);
			this.radioReceiveAtAnInterval.Name = "radioReceiveAtAnInterval";
			this.radioReceiveAtAnInterval.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioReceiveAtAnInterval.Size = new System.Drawing.Size(182, 18);
			this.radioReceiveAtAnInterval.TabIndex = 297;
			this.radioReceiveAtAnInterval.TabStop = true;
			this.radioReceiveAtAnInterval.Text = "Receive at an interval";
			this.radioReceiveAtAnInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioReceiveAtAnInterval.UseVisualStyleBackColor = true;
			this.radioReceiveAtAnInterval.CheckedChanged += new System.EventHandler(this.radioInterval_CheckedChanged);
			// 
			// comboPaymentClinicSetting
			// 
			this.comboPaymentClinicSetting.Location = new System.Drawing.Point(290, 87);
			this.comboPaymentClinicSetting.Name = "comboPaymentClinicSetting";
			this.comboPaymentClinicSetting.Size = new System.Drawing.Size(163, 21);
			this.comboPaymentClinicSetting.TabIndex = 281;
			this.comboPaymentClinicSetting.SelectionChangeCommitted += new System.EventHandler(this.comboPaymentClinicSetting_ChangeCommitted);
			// 
			// radioReceiveAtASetTime
			// 
			this.radioReceiveAtASetTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioReceiveAtASetTime.Location = new System.Drawing.Point(136, 273);
			this.radioReceiveAtASetTime.Name = "radioReceiveAtASetTime";
			this.radioReceiveAtASetTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioReceiveAtASetTime.Size = new System.Drawing.Size(182, 18);
			this.radioReceiveAtASetTime.TabIndex = 298;
			this.radioReceiveAtASetTime.Text = "Receive at a set time";
			this.radioReceiveAtASetTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioReceiveAtASetTime.UseVisualStyleBackColor = true;
			// 
			// checkPaymentsPromptForPayType
			// 
			this.checkPaymentsPromptForPayType.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPaymentsPromptForPayType.Location = new System.Drawing.Point(157, 111);
			this.checkPaymentsPromptForPayType.Name = "checkPaymentsPromptForPayType";
			this.checkPaymentsPromptForPayType.Size = new System.Drawing.Size(296, 18);
			this.checkPaymentsPromptForPayType.TabIndex = 280;
			this.checkPaymentsPromptForPayType.Text = "Payments prompt for Payment Type";
			this.checkPaymentsPromptForPayType.Click += new System.EventHandler(this.checkPaymentsPromptForPayType_Click);
			// 
			// textClaimReportReceiveTime
			// 
			this.textClaimReportReceiveTime.Enabled = false;
			this.textClaimReportReceiveTime.Location = new System.Drawing.Point(326, 273);
			this.textClaimReportReceiveTime.Name = "textClaimReportReceiveTime";
			this.textClaimReportReceiveTime.Size = new System.Drawing.Size(127, 20);
			this.textClaimReportReceiveTime.TabIndex = 296;
			// 
			// label38
			// 
			this.label38.Location = new System.Drawing.Point(55, 88);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(234, 18);
			this.label38.TabIndex = 282;
			this.label38.Text = "Patient Payments Use";
			this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxClaimIdPrefix
			// 
			this.groupBoxClaimIdPrefix.Controls.Add(this.butReplacements);
			this.groupBoxClaimIdPrefix.Controls.Add(this.textClaimIdPrefix);
			this.groupBoxClaimIdPrefix.Location = new System.Drawing.Point(271, 153);
			this.groupBoxClaimIdPrefix.Name = "groupBoxClaimIdPrefix";
			this.groupBoxClaimIdPrefix.Size = new System.Drawing.Size(197, 71);
			this.groupBoxClaimIdPrefix.TabIndex = 291;
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
			// textClaimIdPrefix
			// 
			this.textClaimIdPrefix.Location = new System.Drawing.Point(15, 19);
			this.textClaimIdPrefix.Name = "textClaimIdPrefix";
			this.textClaimIdPrefix.Size = new System.Drawing.Size(167, 20);
			this.textClaimIdPrefix.TabIndex = 238;
			this.textClaimIdPrefix.Validating += new System.ComponentModel.CancelEventHandler(this.textClaimIdPrefix_Validating);
			// 
			// UserControlEnterpriseAccount
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkAgingCalculateOnBatchClaimReceipt);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.checkBillingShowSendProgress);
			this.Controls.Add(this.label27);
			this.Controls.Add(this.checkAgingReportShowAgePatPayplanPayments);
			this.Controls.Add(this.textBillingElectBatchMax);
			this.Controls.Add(this.comboPayPlansVersion);
			this.Controls.Add(this.checkClaimReportReceivedByService);
			this.Controls.Add(this.comboRigorousAdjustments);
			this.Controls.Add(this.checkPaymentWindowDefaultHideSplits);
			this.Controls.Add(this.textClaimReportReceiveInterval);
			this.Controls.Add(this.label41);
			this.Controls.Add(this.labelAgingServiceTimeDue);
			this.Controls.Add(this.comboRigorousAccounting);
			this.Controls.Add(this.label39);
			this.Controls.Add(this.checkBillingShowTransSinceBalZero);
			this.Controls.Add(this.labelReportheckUnits);
			this.Controls.Add(this.textAgingServiceTimeDue);
			this.Controls.Add(this.radioReceiveAtAnInterval);
			this.Controls.Add(this.comboPaymentClinicSetting);
			this.Controls.Add(this.radioReceiveAtASetTime);
			this.Controls.Add(this.checkPaymentsPromptForPayType);
			this.Controls.Add(this.textClaimReportReceiveTime);
			this.Controls.Add(this.label38);
			this.Controls.Add(this.groupBoxClaimIdPrefix);
			this.Name = "UserControlEnterpriseAccount";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupBoxClaimIdPrefix.ResumeLayout(false);
			this.groupBoxClaimIdPrefix.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.CheckBox checkAgingCalculateOnBatchClaimReceipt;
		private System.Windows.Forms.Label label13;
		private UI.CheckBox checkBillingShowSendProgress;
		private System.Windows.Forms.Label label27;
		private UI.CheckBox checkAgingReportShowAgePatPayplanPayments;
		private ValidNum textBillingElectBatchMax;
		private UI.ComboBox comboPayPlansVersion;
		private UI.CheckBox checkClaimReportReceivedByService;
		private UI.ComboBox comboRigorousAdjustments;
		private UI.CheckBox checkPaymentWindowDefaultHideSplits;
		private System.Windows.Forms.TextBox textClaimReportReceiveInterval;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.Label labelAgingServiceTimeDue;
		private UI.ComboBox comboRigorousAccounting;
		private System.Windows.Forms.Label label39;
		private UI.CheckBox checkBillingShowTransSinceBalZero;
		private System.Windows.Forms.Label labelReportheckUnits;
		private ValidDate textAgingServiceTimeDue;
		private System.Windows.Forms.RadioButton radioReceiveAtAnInterval;
		private UI.ComboBox comboPaymentClinicSetting;
		private System.Windows.Forms.RadioButton radioReceiveAtASetTime;
		private UI.CheckBox checkPaymentsPromptForPayType;
		private ValidTime textClaimReportReceiveTime;
		private System.Windows.Forms.Label label38;
		private UI.GroupBox groupBoxClaimIdPrefix;
		private UI.Button butReplacements;
		private System.Windows.Forms.TextBox textClaimIdPrefix;
	}
}
