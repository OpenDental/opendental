
namespace OpenDental {
	partial class UserControlAccountClaimReceive {
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
			this.groupBoxClaimsMedical = new OpenDental.UI.GroupBox();
			this.checkClaimMedReceivedPromptForPrimaryClaim = new OpenDental.UI.CheckBox();
			this.checkClaimMedReceivedForcePrimaryStatus = new OpenDental.UI.CheckBox();
			this.groupBoxClaimsPayments = new OpenDental.UI.GroupBox();
			this.comboClaimCredit = new OpenDental.UI.ComboBox();
			this.checkClaimPaymentPickStatementType = new OpenDental.UI.CheckBox();
			this.checkNoInitialPrimaryInsMoreThanProc = new OpenDental.UI.CheckBox();
			this.checkAllowProcAdjFromClaim = new OpenDental.UI.CheckBox();
			this.checkInsPayNoWriteoffMoreThanProc = new OpenDental.UI.CheckBox();
			this.labelClaimCredit = new System.Windows.Forms.Label();
			this.groupBoxOD2 = new OpenDental.UI.GroupBox();
			this.checkClaimPrimaryReceivedRecalcSecondary = new OpenDental.UI.CheckBox();
			this.checkClaimFinalizeWarning = new OpenDental.UI.CheckBox();
			this.checkInsAutoReceiveNoAssign = new OpenDental.UI.CheckBox();
			this.checkShowClaimPatResp = new OpenDental.UI.CheckBox();
			this.checkPromptForSecondaryClaim = new OpenDental.UI.CheckBox();
			this.checkClaimPrimaryRecievedForceSecondaryStatus = new OpenDental.UI.CheckBox();
			this.checkProviderIncomeShows = new OpenDental.UI.CheckBox();
			this.checkShowClaimPayTracking = new OpenDental.UI.CheckBox();
			this.checkInsEstRecalcReceived = new OpenDental.UI.CheckBox();
			this.checkClaimTrackingExcludeNone = new OpenDental.UI.CheckBox();
			this.groupBoxClaimsMedical.SuspendLayout();
			this.groupBoxClaimsPayments.SuspendLayout();
			this.groupBoxOD2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxClaimsMedical
			// 
			this.groupBoxClaimsMedical.Controls.Add(this.checkClaimMedReceivedPromptForPrimaryClaim);
			this.groupBoxClaimsMedical.Controls.Add(this.checkClaimMedReceivedForcePrimaryStatus);
			this.groupBoxClaimsMedical.Location = new System.Drawing.Point(20, 339);
			this.groupBoxClaimsMedical.Name = "groupBoxClaimsMedical";
			this.groupBoxClaimsMedical.Size = new System.Drawing.Size(450, 51);
			this.groupBoxClaimsMedical.TabIndex = 4;
			this.groupBoxClaimsMedical.Text = "Claims Medical";
			// 
			// checkClaimMedReceivedPromptForPrimaryClaim
			// 
			this.checkClaimMedReceivedPromptForPrimaryClaim.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimMedReceivedPromptForPrimaryClaim.Location = new System.Drawing.Point(85, 10);
			this.checkClaimMedReceivedPromptForPrimaryClaim.Name = "checkClaimMedReceivedPromptForPrimaryClaim";
			this.checkClaimMedReceivedPromptForPrimaryClaim.Size = new System.Drawing.Size(355, 17);
			this.checkClaimMedReceivedPromptForPrimaryClaim.TabIndex = 0;
			this.checkClaimMedReceivedPromptForPrimaryClaim.Text = "Prompt for primary claims";
			// 
			// checkClaimMedReceivedForcePrimaryStatus
			// 
			this.checkClaimMedReceivedForcePrimaryStatus.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimMedReceivedForcePrimaryStatus.Location = new System.Drawing.Point(15, 29);
			this.checkClaimMedReceivedForcePrimaryStatus.Name = "checkClaimMedReceivedForcePrimaryStatus";
			this.checkClaimMedReceivedForcePrimaryStatus.Size = new System.Drawing.Size(425, 17);
			this.checkClaimMedReceivedForcePrimaryStatus.TabIndex = 1;
			this.checkClaimMedReceivedForcePrimaryStatus.Text = "Remove \'Do Nothing\' for primary claims with \'Hold Until Pri Received\' or \'Unsent\'" +
    "";
			// 
			// groupBoxClaimsPayments
			// 
			this.groupBoxClaimsPayments.Controls.Add(this.comboClaimCredit);
			this.groupBoxClaimsPayments.Controls.Add(this.checkClaimPaymentPickStatementType);
			this.groupBoxClaimsPayments.Controls.Add(this.checkNoInitialPrimaryInsMoreThanProc);
			this.groupBoxClaimsPayments.Controls.Add(this.checkAllowProcAdjFromClaim);
			this.groupBoxClaimsPayments.Controls.Add(this.checkInsPayNoWriteoffMoreThanProc);
			this.groupBoxClaimsPayments.Controls.Add(this.labelClaimCredit);
			this.groupBoxClaimsPayments.Location = new System.Drawing.Point(20, 220);
			this.groupBoxClaimsPayments.Name = "groupBoxClaimsPayments";
			this.groupBoxClaimsPayments.Size = new System.Drawing.Size(450, 112);
			this.groupBoxClaimsPayments.TabIndex = 2;
			this.groupBoxClaimsPayments.Text = "Claims Payments";
			// 
			// comboClaimCredit
			// 
			this.comboClaimCredit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClaimCredit.Location = new System.Drawing.Point(272, 29);
			this.comboClaimCredit.Name = "comboClaimCredit";
			this.comboClaimCredit.Size = new System.Drawing.Size(168, 21);
			this.comboClaimCredit.TabIndex = 1;
			// 
			// checkClaimPaymentPickStatementType
			// 
			this.checkClaimPaymentPickStatementType.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimPaymentPickStatementType.Location = new System.Drawing.Point(3, 90);
			this.checkClaimPaymentPickStatementType.Name = "checkClaimPaymentPickStatementType";
			this.checkClaimPaymentPickStatementType.Size = new System.Drawing.Size(437, 17);
			this.checkClaimPaymentPickStatementType.TabIndex = 291;
			this.checkClaimPaymentPickStatementType.Text = "Claim payments prompt for Payment Type";
			// 
			// checkNoInitialPrimaryInsMoreThanProc
			// 
			this.checkNoInitialPrimaryInsMoreThanProc.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNoInitialPrimaryInsMoreThanProc.Location = new System.Drawing.Point(3, 71);
			this.checkNoInitialPrimaryInsMoreThanProc.Name = "checkNoInitialPrimaryInsMoreThanProc";
			this.checkNoInitialPrimaryInsMoreThanProc.Size = new System.Drawing.Size(437, 17);
			this.checkNoInitialPrimaryInsMoreThanProc.TabIndex = 3;
			this.checkNoInitialPrimaryInsMoreThanProc.Text = "Initial primary insurance payment and write-off cannot exceed adjusted proc fee";
			// 
			// checkAllowProcAdjFromClaim
			// 
			this.checkAllowProcAdjFromClaim.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowProcAdjFromClaim.Location = new System.Drawing.Point(115, 10);
			this.checkAllowProcAdjFromClaim.Name = "checkAllowProcAdjFromClaim";
			this.checkAllowProcAdjFromClaim.Size = new System.Drawing.Size(325, 17);
			this.checkAllowProcAdjFromClaim.TabIndex = 0;
			this.checkAllowProcAdjFromClaim.Text = "Allow procedure adjustments from Edit Claim window";
			// 
			// checkInsPayNoWriteoffMoreThanProc
			// 
			this.checkInsPayNoWriteoffMoreThanProc.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPayNoWriteoffMoreThanProc.Location = new System.Drawing.Point(73, 52);
			this.checkInsPayNoWriteoffMoreThanProc.Name = "checkInsPayNoWriteoffMoreThanProc";
			this.checkInsPayNoWriteoffMoreThanProc.Size = new System.Drawing.Size(367, 17);
			this.checkInsPayNoWriteoffMoreThanProc.TabIndex = 2;
			this.checkInsPayNoWriteoffMoreThanProc.Text = "Disallow write-offs greater than the adjusted procedure fee";
			// 
			// labelClaimCredit
			// 
			this.labelClaimCredit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimCredit.Location = new System.Drawing.Point(32, 32);
			this.labelClaimCredit.Name = "labelClaimCredit";
			this.labelClaimCredit.Size = new System.Drawing.Size(239, 17);
			this.labelClaimCredit.TabIndex = 290;
			this.labelClaimCredit.Text = "Payment exceeds procedure balance";
			this.labelClaimCredit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxOD2
			// 
			this.groupBoxOD2.Controls.Add(this.checkClaimPrimaryReceivedRecalcSecondary);
			this.groupBoxOD2.Controls.Add(this.checkClaimFinalizeWarning);
			this.groupBoxOD2.Controls.Add(this.checkInsAutoReceiveNoAssign);
			this.groupBoxOD2.Controls.Add(this.checkShowClaimPatResp);
			this.groupBoxOD2.Controls.Add(this.checkPromptForSecondaryClaim);
			this.groupBoxOD2.Controls.Add(this.checkClaimPrimaryRecievedForceSecondaryStatus);
			this.groupBoxOD2.Controls.Add(this.checkProviderIncomeShows);
			this.groupBoxOD2.Controls.Add(this.checkShowClaimPayTracking);
			this.groupBoxOD2.Controls.Add(this.checkInsEstRecalcReceived);
			this.groupBoxOD2.Controls.Add(this.checkClaimTrackingExcludeNone);
			this.groupBoxOD2.Location = new System.Drawing.Point(20, 10);
			this.groupBoxOD2.Name = "groupBoxOD2";
			this.groupBoxOD2.Size = new System.Drawing.Size(450, 203);
			this.groupBoxOD2.TabIndex = 1;
			this.groupBoxOD2.Text = "Claims Receive";
			// 
			// checkClaimPrimaryReceivedRecalcSecondary
			// 
			this.checkClaimPrimaryReceivedRecalcSecondary.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimPrimaryReceivedRecalcSecondary.Location = new System.Drawing.Point(30, 181);
			this.checkClaimPrimaryReceivedRecalcSecondary.Name = "checkClaimPrimaryReceivedRecalcSecondary";
			this.checkClaimPrimaryReceivedRecalcSecondary.Size = new System.Drawing.Size(410, 17);
			this.checkClaimPrimaryReceivedRecalcSecondary.TabIndex = 9;
			this.checkClaimPrimaryReceivedRecalcSecondary.Text = "Auto update secondary claim estimates when primary is received";
			// 
			// checkClaimFinalizeWarning
			// 
			this.checkClaimFinalizeWarning.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimFinalizeWarning.Location = new System.Drawing.Point(30, 162);
			this.checkClaimFinalizeWarning.Name = "checkClaimFinalizeWarning";
			this.checkClaimFinalizeWarning.Size = new System.Drawing.Size(410, 17);
			this.checkClaimFinalizeWarning.TabIndex = 8;
			this.checkClaimFinalizeWarning.Text = "Warn users to finalize payments for received claims";
			// 
			// checkInsAutoReceiveNoAssign
			// 
			this.checkInsAutoReceiveNoAssign.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsAutoReceiveNoAssign.Location = new System.Drawing.Point(30, 143);
			this.checkInsAutoReceiveNoAssign.Name = "checkInsAutoReceiveNoAssign";
			this.checkInsAutoReceiveNoAssign.Size = new System.Drawing.Size(410, 17);
			this.checkInsAutoReceiveNoAssign.TabIndex = 7;
			this.checkInsAutoReceiveNoAssign.Text = "Auto receive claims with no assignment of benefits";
			// 
			// checkShowClaimPatResp
			// 
			this.checkShowClaimPatResp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowClaimPatResp.Location = new System.Drawing.Point(30, 105);
			this.checkShowClaimPatResp.Name = "checkShowClaimPatResp";
			this.checkShowClaimPatResp.Size = new System.Drawing.Size(410, 17);
			this.checkShowClaimPatResp.TabIndex = 5;
			this.checkShowClaimPatResp.Text = "Show Patient Responsibility column in the Edit Claim/Payment windows";
			// 
			// checkPromptForSecondaryClaim
			// 
			this.checkPromptForSecondaryClaim.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPromptForSecondaryClaim.Location = new System.Drawing.Point(73, 48);
			this.checkPromptForSecondaryClaim.Name = "checkPromptForSecondaryClaim";
			this.checkPromptForSecondaryClaim.Size = new System.Drawing.Size(367, 17);
			this.checkPromptForSecondaryClaim.TabIndex = 2;
			this.checkPromptForSecondaryClaim.Text = "Prompt for secondary claims";
			// 
			// checkClaimPrimaryRecievedForceSecondaryStatus
			// 
			this.checkClaimPrimaryRecievedForceSecondaryStatus.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimPrimaryRecievedForceSecondaryStatus.Location = new System.Drawing.Point(30, 124);
			this.checkClaimPrimaryRecievedForceSecondaryStatus.Name = "checkClaimPrimaryRecievedForceSecondaryStatus";
			this.checkClaimPrimaryRecievedForceSecondaryStatus.Size = new System.Drawing.Size(410, 17);
			this.checkClaimPrimaryRecievedForceSecondaryStatus.TabIndex = 6;
			this.checkClaimPrimaryRecievedForceSecondaryStatus.Text = "Remove \'Do Nothing\' for secondary claims with \'Hold until Pri received\'";
			// 
			// checkProviderIncomeShows
			// 
			this.checkProviderIncomeShows.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProviderIncomeShows.Location = new System.Drawing.Point(30, 29);
			this.checkProviderIncomeShows.Name = "checkProviderIncomeShows";
			this.checkProviderIncomeShows.Size = new System.Drawing.Size(410, 17);
			this.checkProviderIncomeShows.TabIndex = 1;
			this.checkProviderIncomeShows.Text = "Show provider income transfer window after entering insurance payment";
			// 
			// checkShowClaimPayTracking
			// 
			this.checkShowClaimPayTracking.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowClaimPayTracking.Location = new System.Drawing.Point(30, 86);
			this.checkShowClaimPayTracking.Name = "checkShowClaimPayTracking";
			this.checkShowClaimPayTracking.Size = new System.Drawing.Size(410, 17);
			this.checkShowClaimPayTracking.TabIndex = 4;
			this.checkShowClaimPayTracking.Text = "Show Payment Tracking column in the Edit Claim/Payment windows";
			// 
			// checkInsEstRecalcReceived
			// 
			this.checkInsEstRecalcReceived.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsEstRecalcReceived.Location = new System.Drawing.Point(73, 67);
			this.checkInsEstRecalcReceived.Name = "checkInsEstRecalcReceived";
			this.checkInsEstRecalcReceived.Size = new System.Drawing.Size(367, 17);
			this.checkInsEstRecalcReceived.TabIndex = 3;
			this.checkInsEstRecalcReceived.Text = "Recalculate estimates for received claim procedures";
			// 
			// checkClaimTrackingExcludeNone
			// 
			this.checkClaimTrackingExcludeNone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimTrackingExcludeNone.Location = new System.Drawing.Point(100, 10);
			this.checkClaimTrackingExcludeNone.Name = "checkClaimTrackingExcludeNone";
			this.checkClaimTrackingExcludeNone.Size = new System.Drawing.Size(340, 17);
			this.checkClaimTrackingExcludeNone.TabIndex = 0;
			this.checkClaimTrackingExcludeNone.Text = "Exclude \'None\' as an option on Custom Tracking Status";
			// 
			// UserControlAccountClaimReceive
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.groupBoxClaimsMedical);
			this.Controls.Add(this.groupBoxClaimsPayments);
			this.Controls.Add(this.groupBoxOD2);
			this.Name = "UserControlAccountClaimReceive";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupBoxClaimsMedical.ResumeLayout(false);
			this.groupBoxClaimsPayments.ResumeLayout(false);
			this.groupBoxOD2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.CheckBox checkClaimPrimaryRecievedForceSecondaryStatus;
		private OpenDental.UI.CheckBox checkShowClaimPayTracking;
		private OpenDental.UI.CheckBox checkShowClaimPatResp;
		private OpenDental.UI.CheckBox checkInsEstRecalcReceived;
		private OpenDental.UI.CheckBox checkPromptForSecondaryClaim;
		private OpenDental.UI.CheckBox checkInsPayNoWriteoffMoreThanProc;
		private OpenDental.UI.CheckBox checkClaimTrackingExcludeNone;
		private System.Windows.Forms.Label labelClaimCredit;
		private UI.ComboBox comboClaimCredit;
		private OpenDental.UI.CheckBox checkAllowProcAdjFromClaim;
		private OpenDental.UI.CheckBox checkProviderIncomeShows;
		private UI.GroupBox groupBoxOD2;
		private UI.GroupBox groupBoxClaimsPayments;
		private OpenDental.UI.CheckBox checkNoInitialPrimaryInsMoreThanProc;
		private OpenDental.UI.CheckBox checkInsAutoReceiveNoAssign;
		private OpenDental.UI.CheckBox checkClaimPaymentPickStatementType;
		private UI.CheckBox checkClaimFinalizeWarning;
		private UI.CheckBox checkClaimPrimaryReceivedRecalcSecondary;
		private UI.GroupBox groupBoxClaimsMedical;
		private UI.CheckBox checkClaimMedReceivedPromptForPrimaryClaim;
		private UI.CheckBox checkClaimMedReceivedForcePrimaryStatus;
	}

}

