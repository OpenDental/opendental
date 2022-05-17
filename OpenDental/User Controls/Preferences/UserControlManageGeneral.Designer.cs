
namespace OpenDental {
	partial class UserControlManageGeneral {
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
			this.checkScheduleProvEmpSelectAll = new System.Windows.Forms.CheckBox();
			this.checkClaimsSendWindowValidateOnLoad = new System.Windows.Forms.CheckBox();
			this.checkTimeCardADP = new System.Windows.Forms.CheckBox();
			this.comboTimeCardOvertimeFirstDayOfWeek = new OpenDental.UI.ComboBoxOD();
			this.label16 = new System.Windows.Forms.Label();
			this.checkRxSendNewToQueue = new System.Windows.Forms.CheckBox();
			this.checkRxHideProvsWithoutDEA = new System.Windows.Forms.CheckBox();
			this.comboEraAutomation = new OpenDental.UI.ComboBoxOD();
			this.labelEraAutomation = new System.Windows.Forms.Label();
			this.comboDepositSoftware = new OpenDental.UI.ComboBoxOD();
			this.labelDepositSoftware = new System.Windows.Forms.Label();
			this.checkEraAllowTotalPayment = new System.Windows.Forms.CheckBox();
			this.checkIncludeEraWOPercCoPay = new System.Windows.Forms.CheckBox();
			this.checkClockEventAllowBreak = new System.Windows.Forms.CheckBox();
			this.textClaimsReceivedDays = new OpenDental.ValidNum();
			this.checkShowAutoDeposit = new System.Windows.Forms.CheckBox();
			this.checkEraOneClaimPerPage = new System.Windows.Forms.CheckBox();
			this.checkClaimPaymentBatchOnly = new System.Windows.Forms.CheckBox();
			this.labelClaimsReceivedDays = new System.Windows.Forms.Label();
			this.groupBoxERA = new OpenDental.UI.GroupBoxOD();
			this.groupBoxClaims = new OpenDental.UI.GroupBoxOD();
			this.groupBoxPrescriptions = new OpenDental.UI.GroupBoxOD();
			this.groupBoxDeposits = new OpenDental.UI.GroupBoxOD();
			this.groupBoxTimeCards = new OpenDental.UI.GroupBoxOD();
			this.checkAccountingInvoiceAttachmentsSaveInDatabase = new System.Windows.Forms.CheckBox();
			this.groupBoxERA.SuspendLayout();
			this.groupBoxClaims.SuspendLayout();
			this.groupBoxPrescriptions.SuspendLayout();
			this.groupBoxDeposits.SuspendLayout();
			this.groupBoxTimeCards.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkScheduleProvEmpSelectAll
			// 
			this.checkScheduleProvEmpSelectAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkScheduleProvEmpSelectAll.Location = new System.Drawing.Point(39, 447);
			this.checkScheduleProvEmpSelectAll.Name = "checkScheduleProvEmpSelectAll";
			this.checkScheduleProvEmpSelectAll.Size = new System.Drawing.Size(421, 17);
			this.checkScheduleProvEmpSelectAll.TabIndex = 201;
			this.checkScheduleProvEmpSelectAll.Text = "Select all provider/employees when loading schedules";
			this.checkScheduleProvEmpSelectAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimsSendWindowValidateOnLoad
			// 
			this.checkClaimsSendWindowValidateOnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimsSendWindowValidateOnLoad.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimsSendWindowValidateOnLoad.Location = new System.Drawing.Point(60, 10);
			this.checkClaimsSendWindowValidateOnLoad.Name = "checkClaimsSendWindowValidateOnLoad";
			this.checkClaimsSendWindowValidateOnLoad.Size = new System.Drawing.Size(380, 17);
			this.checkClaimsSendWindowValidateOnLoad.TabIndex = 200;
			this.checkClaimsSendWindowValidateOnLoad.Text = "Claims Send window validate on load (can cause slowness)";
			this.checkClaimsSendWindowValidateOnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkTimeCardADP
			// 
			this.checkTimeCardADP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTimeCardADP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimeCardADP.Location = new System.Drawing.Point(81, 37);
			this.checkTimeCardADP.Name = "checkTimeCardADP";
			this.checkTimeCardADP.Size = new System.Drawing.Size(359, 17);
			this.checkTimeCardADP.TabIndex = 199;
			this.checkTimeCardADP.Text = "ADP export includes employee name";
			this.checkTimeCardADP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTimeCardOvertimeFirstDayOfWeek
			// 
			this.comboTimeCardOvertimeFirstDayOfWeek.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboTimeCardOvertimeFirstDayOfWeek.Location = new System.Drawing.Point(270, 10);
			this.comboTimeCardOvertimeFirstDayOfWeek.Name = "comboTimeCardOvertimeFirstDayOfWeek";
			this.comboTimeCardOvertimeFirstDayOfWeek.Size = new System.Drawing.Size(170, 21);
			this.comboTimeCardOvertimeFirstDayOfWeek.TabIndex = 198;
			// 
			// label16
			// 
			this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label16.Location = new System.Drawing.Point(49, 13);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(218, 17);
			this.label16.TabIndex = 202;
			this.label16.Text = "Time Card first day of week for overtime";
			this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkRxSendNewToQueue
			// 
			this.checkRxSendNewToQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRxSendNewToQueue.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRxSendNewToQueue.Location = new System.Drawing.Point(90, 10);
			this.checkRxSendNewToQueue.Name = "checkRxSendNewToQueue";
			this.checkRxSendNewToQueue.Size = new System.Drawing.Size(350, 17);
			this.checkRxSendNewToQueue.TabIndex = 197;
			this.checkRxSendNewToQueue.Text = "Send all new prescriptions to electronic queue";
			this.checkRxSendNewToQueue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRxHideProvsWithoutDEA
			// 
			this.checkRxHideProvsWithoutDEA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRxHideProvsWithoutDEA.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRxHideProvsWithoutDEA.Location = new System.Drawing.Point(20, 33);
			this.checkRxHideProvsWithoutDEA.Name = "checkRxHideProvsWithoutDEA";
			this.checkRxHideProvsWithoutDEA.Size = new System.Drawing.Size(420, 17);
			this.checkRxHideProvsWithoutDEA.TabIndex = 302;
			this.checkRxHideProvsWithoutDEA.Text = "Hide providers without DEA number from making (non electronic)  prescriptions";
			this.checkRxHideProvsWithoutDEA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboEraAutomation
			// 
			this.comboEraAutomation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboEraAutomation.Location = new System.Drawing.Point(310, 79);
			this.comboEraAutomation.Name = "comboEraAutomation";
			this.comboEraAutomation.Size = new System.Drawing.Size(130, 21);
			this.comboEraAutomation.TabIndex = 297;
			// 
			// labelEraAutomation
			// 
			this.labelEraAutomation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelEraAutomation.Location = new System.Drawing.Point(118, 82);
			this.labelEraAutomation.Name = "labelEraAutomation";
			this.labelEraAutomation.Size = new System.Drawing.Size(189, 17);
			this.labelEraAutomation.TabIndex = 301;
			this.labelEraAutomation.Text = "ERA Automation";
			this.labelEraAutomation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDepositSoftware
			// 
			this.comboDepositSoftware.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboDepositSoftware.Location = new System.Drawing.Point(310, 33);
			this.comboDepositSoftware.Name = "comboDepositSoftware";
			this.comboDepositSoftware.Size = new System.Drawing.Size(130, 21);
			this.comboDepositSoftware.TabIndex = 298;
			this.comboDepositSoftware.SelectionChangeCommitted += new System.EventHandler(this.comboDepositSoftware_SelectionChangeCommitted);
			// 
			// labelDepositSoftware
			// 
			this.labelDepositSoftware.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDepositSoftware.Location = new System.Drawing.Point(115, 36);
			this.labelDepositSoftware.Name = "labelDepositSoftware";
			this.labelDepositSoftware.Size = new System.Drawing.Size(192, 17);
			this.labelDepositSoftware.TabIndex = 300;
			this.labelDepositSoftware.Text = "Deposit Software";
			this.labelDepositSoftware.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEraAllowTotalPayment
			// 
			this.checkEraAllowTotalPayment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkEraAllowTotalPayment.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraAllowTotalPayment.Location = new System.Drawing.Point(19, 56);
			this.checkEraAllowTotalPayment.Name = "checkEraAllowTotalPayment";
			this.checkEraAllowTotalPayment.Size = new System.Drawing.Size(421, 17);
			this.checkEraAllowTotalPayment.TabIndex = 296;
			this.checkEraAllowTotalPayment.Text = "ERA allow total payments";
			this.checkEraAllowTotalPayment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIncludeEraWOPercCoPay
			// 
			this.checkIncludeEraWOPercCoPay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeEraWOPercCoPay.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeEraWOPercCoPay.Location = new System.Drawing.Point(40, 33);
			this.checkIncludeEraWOPercCoPay.Name = "checkIncludeEraWOPercCoPay";
			this.checkIncludeEraWOPercCoPay.Size = new System.Drawing.Size(400, 17);
			this.checkIncludeEraWOPercCoPay.TabIndex = 293;
			this.checkIncludeEraWOPercCoPay.Text = "ERA posts write-offs for Category Percentage and Medicaid/Flat Copay";
			this.checkIncludeEraWOPercCoPay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClockEventAllowBreak
			// 
			this.checkClockEventAllowBreak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClockEventAllowBreak.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClockEventAllowBreak.Location = new System.Drawing.Point(19, 60);
			this.checkClockEventAllowBreak.Name = "checkClockEventAllowBreak";
			this.checkClockEventAllowBreak.Size = new System.Drawing.Size(421, 17);
			this.checkClockEventAllowBreak.TabIndex = 295;
			this.checkClockEventAllowBreak.Text = "Allow paid 30 minute breaks";
			this.checkClockEventAllowBreak.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClaimsReceivedDays
			// 
			this.textClaimsReceivedDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimsReceivedDays.Location = new System.Drawing.Point(380, 33);
			this.textClaimsReceivedDays.MaxVal = 999999;
			this.textClaimsReceivedDays.MinVal = 1;
			this.textClaimsReceivedDays.Name = "textClaimsReceivedDays";
			this.textClaimsReceivedDays.ShowZero = false;
			this.textClaimsReceivedDays.Size = new System.Drawing.Size(60, 20);
			this.textClaimsReceivedDays.TabIndex = 290;
			this.textClaimsReceivedDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkShowAutoDeposit
			// 
			this.checkShowAutoDeposit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowAutoDeposit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAutoDeposit.Location = new System.Drawing.Point(90, 10);
			this.checkShowAutoDeposit.Name = "checkShowAutoDeposit";
			this.checkShowAutoDeposit.Size = new System.Drawing.Size(350, 17);
			this.checkShowAutoDeposit.TabIndex = 294;
			this.checkShowAutoDeposit.Text = "Insurance payments show auto deposit";
			this.checkShowAutoDeposit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEraOneClaimPerPage
			// 
			this.checkEraOneClaimPerPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkEraOneClaimPerPage.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraOneClaimPerPage.Location = new System.Drawing.Point(40, 10);
			this.checkEraOneClaimPerPage.Name = "checkEraOneClaimPerPage";
			this.checkEraOneClaimPerPage.Size = new System.Drawing.Size(400, 17);
			this.checkEraOneClaimPerPage.TabIndex = 292;
			this.checkEraOneClaimPerPage.Text = "ERA prints one page per claim";
			this.checkEraOneClaimPerPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimPaymentBatchOnly
			// 
			this.checkClaimPaymentBatchOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimPaymentBatchOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimPaymentBatchOnly.Location = new System.Drawing.Point(19, 59);
			this.checkClaimPaymentBatchOnly.Name = "checkClaimPaymentBatchOnly";
			this.checkClaimPaymentBatchOnly.Size = new System.Drawing.Size(421, 17);
			this.checkClaimPaymentBatchOnly.TabIndex = 291;
			this.checkClaimPaymentBatchOnly.Text = "Finalize claim payments in Batch Insurance window only";
			this.checkClaimPaymentBatchOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClaimsReceivedDays
			// 
			this.labelClaimsReceivedDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimsReceivedDays.Location = new System.Drawing.Point(16, 36);
			this.labelClaimsReceivedDays.MaximumSize = new System.Drawing.Size(1000, 300);
			this.labelClaimsReceivedDays.Name = "labelClaimsReceivedDays";
			this.labelClaimsReceivedDays.Size = new System.Drawing.Size(361, 17);
			this.labelClaimsReceivedDays.TabIndex = 299;
			this.labelClaimsReceivedDays.Text = "Show claims received after days (blank to disable)";
			this.labelClaimsReceivedDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxERA
			// 
			this.groupBoxERA.Controls.Add(this.checkEraOneClaimPerPage);
			this.groupBoxERA.Controls.Add(this.checkIncludeEraWOPercCoPay);
			this.groupBoxERA.Controls.Add(this.labelEraAutomation);
			this.groupBoxERA.Controls.Add(this.comboEraAutomation);
			this.groupBoxERA.Controls.Add(this.checkEraAllowTotalPayment);
			this.groupBoxERA.Location = new System.Drawing.Point(20, 10);
			this.groupBoxERA.Name = "groupBoxERA";
			this.groupBoxERA.Size = new System.Drawing.Size(450, 110);
			this.groupBoxERA.TabIndex = 303;
			this.groupBoxERA.Text = "ERA";
			// 
			// groupBoxClaims
			// 
			this.groupBoxClaims.Controls.Add(this.checkClaimsSendWindowValidateOnLoad);
			this.groupBoxClaims.Controls.Add(this.textClaimsReceivedDays);
			this.groupBoxClaims.Controls.Add(this.labelClaimsReceivedDays);
			this.groupBoxClaims.Controls.Add(this.checkClaimPaymentBatchOnly);
			this.groupBoxClaims.Location = new System.Drawing.Point(20, 126);
			this.groupBoxClaims.Name = "groupBoxClaims";
			this.groupBoxClaims.Size = new System.Drawing.Size(450, 86);
			this.groupBoxClaims.TabIndex = 304;
			this.groupBoxClaims.Text = "Claims";
			// 
			// groupBoxPrescriptions
			// 
			this.groupBoxPrescriptions.Controls.Add(this.checkRxSendNewToQueue);
			this.groupBoxPrescriptions.Controls.Add(this.checkRxHideProvsWithoutDEA);
			this.groupBoxPrescriptions.Location = new System.Drawing.Point(20, 218);
			this.groupBoxPrescriptions.Name = "groupBoxPrescriptions";
			this.groupBoxPrescriptions.Size = new System.Drawing.Size(450, 60);
			this.groupBoxPrescriptions.TabIndex = 305;
			this.groupBoxPrescriptions.Text = "Prescriptions";
			// 
			// groupBoxDeposits
			// 
			this.groupBoxDeposits.Controls.Add(this.checkShowAutoDeposit);
			this.groupBoxDeposits.Controls.Add(this.comboDepositSoftware);
			this.groupBoxDeposits.Controls.Add(this.labelDepositSoftware);
			this.groupBoxDeposits.Location = new System.Drawing.Point(20, 284);
			this.groupBoxDeposits.Name = "groupBoxDeposits";
			this.groupBoxDeposits.Size = new System.Drawing.Size(450, 64);
			this.groupBoxDeposits.TabIndex = 306;
			this.groupBoxDeposits.Text = "Deposits";
			// 
			// groupBoxTimeCards
			// 
			this.groupBoxTimeCards.Controls.Add(this.comboTimeCardOvertimeFirstDayOfWeek);
			this.groupBoxTimeCards.Controls.Add(this.label16);
			this.groupBoxTimeCards.Controls.Add(this.checkTimeCardADP);
			this.groupBoxTimeCards.Controls.Add(this.checkClockEventAllowBreak);
			this.groupBoxTimeCards.Location = new System.Drawing.Point(20, 354);
			this.groupBoxTimeCards.Name = "groupBoxTimeCards";
			this.groupBoxTimeCards.Size = new System.Drawing.Size(450, 87);
			this.groupBoxTimeCards.TabIndex = 307;
			this.groupBoxTimeCards.Text = "Time Cards";
			// 
			// checkAccountingInvoiceAttachmentsSaveInDatabase
			// 
			this.checkAccountingInvoiceAttachmentsSaveInDatabase.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAccountingInvoiceAttachmentsSaveInDatabase.Location = new System.Drawing.Point(25, 470);
			this.checkAccountingInvoiceAttachmentsSaveInDatabase.Name = "checkAccountingInvoiceAttachmentsSaveInDatabase";
			this.checkAccountingInvoiceAttachmentsSaveInDatabase.Size = new System.Drawing.Size(435, 17);
			this.checkAccountingInvoiceAttachmentsSaveInDatabase.TabIndex = 308;
			this.checkAccountingInvoiceAttachmentsSaveInDatabase.Text = "Save accounting invoice attachments in database";
			this.checkAccountingInvoiceAttachmentsSaveInDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// UserControlManageGeneral
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkAccountingInvoiceAttachmentsSaveInDatabase);
			this.Controls.Add(this.groupBoxTimeCards);
			this.Controls.Add(this.groupBoxDeposits);
			this.Controls.Add(this.groupBoxPrescriptions);
			this.Controls.Add(this.groupBoxClaims);
			this.Controls.Add(this.groupBoxERA);
			this.Controls.Add(this.checkScheduleProvEmpSelectAll);
			this.Name = "UserControlManageGeneral";
			this.Size = new System.Drawing.Size(974, 641);
			this.groupBoxERA.ResumeLayout(false);
			this.groupBoxClaims.ResumeLayout(false);
			this.groupBoxClaims.PerformLayout();
			this.groupBoxPrescriptions.ResumeLayout(false);
			this.groupBoxDeposits.ResumeLayout(false);
			this.groupBoxTimeCards.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox checkScheduleProvEmpSelectAll;
		private System.Windows.Forms.CheckBox checkClaimsSendWindowValidateOnLoad;
		private System.Windows.Forms.CheckBox checkTimeCardADP;
		private UI.ComboBoxOD comboTimeCardOvertimeFirstDayOfWeek;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.CheckBox checkRxSendNewToQueue;
		private System.Windows.Forms.CheckBox checkRxHideProvsWithoutDEA;
		private UI.ComboBoxOD comboEraAutomation;
		private System.Windows.Forms.Label labelEraAutomation;
		private UI.ComboBoxOD comboDepositSoftware;
		private System.Windows.Forms.Label labelDepositSoftware;
		private System.Windows.Forms.CheckBox checkEraAllowTotalPayment;
		private System.Windows.Forms.CheckBox checkIncludeEraWOPercCoPay;
		private System.Windows.Forms.CheckBox checkClockEventAllowBreak;
		private ValidNum textClaimsReceivedDays;
		private System.Windows.Forms.CheckBox checkShowAutoDeposit;
		private System.Windows.Forms.CheckBox checkEraOneClaimPerPage;
		private System.Windows.Forms.CheckBox checkClaimPaymentBatchOnly;
		private System.Windows.Forms.Label labelClaimsReceivedDays;
		private UI.GroupBoxOD groupBoxERA;
		private UI.GroupBoxOD groupBoxClaims;
		private UI.GroupBoxOD groupBoxPrescriptions;
		private UI.GroupBoxOD groupBoxDeposits;
		private UI.GroupBoxOD groupBoxTimeCards;
		private System.Windows.Forms.CheckBox checkAccountingInvoiceAttachmentsSaveInDatabase;
	}
}
