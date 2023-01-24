
namespace OpenDental {
	partial class UserControlAccountInsurance {
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
			this.checkCanadianPpoLabEst = new OpenDental.UI.CheckBox();
			this.textInsWriteoffDescript = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.labelClaimIdPrefixDetails = new System.Windows.Forms.Label();
			this.labelEclaimsSeparateTreatProvDetails = new System.Windows.Forms.Label();
			this.groupBoxClaimsMedical = new OpenDental.UI.GroupBox();
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical = new OpenDental.UI.CheckBox();
			this.checkEclaimsMedicalProvTreatmentAsOrdering = new OpenDental.UI.CheckBox();
			this.groupBoxClaimsPayments = new OpenDental.UI.GroupBox();
			this.checkClaimPaymentPickStatementType = new OpenDental.UI.CheckBox();
			this.checkNoInitialPrimaryInsMoreThanProc = new OpenDental.UI.CheckBox();
			this.checkAllowProcAdjFromClaim = new OpenDental.UI.CheckBox();
			this.checkInsPayNoWriteoffMoreThanProc = new OpenDental.UI.CheckBox();
			this.comboClaimCredit = new OpenDental.UI.ComboBox();
			this.labelClaimCredit = new System.Windows.Forms.Label();
			this.groupBoxOD2 = new OpenDental.UI.GroupBox();
			this.checkInsAutoReceiveNoAssign = new OpenDental.UI.CheckBox();
			this.checkShowClaimPatResp = new OpenDental.UI.CheckBox();
			this.checkPromptForSecondaryClaim = new OpenDental.UI.CheckBox();
			this.checkClaimPrimaryRecievedForceSecondaryStatus = new OpenDental.UI.CheckBox();
			this.checkProviderIncomeShows = new OpenDental.UI.CheckBox();
			this.checkShowClaimPayTracking = new OpenDental.UI.CheckBox();
			this.checkInsEstRecalcReceived = new OpenDental.UI.CheckBox();
			this.checkClaimTrackingExcludeNone = new OpenDental.UI.CheckBox();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.checkPpoUseUcr = new OpenDental.UI.CheckBox();
			this.checkClaimFormTreatDentSaysSigOnFile = new OpenDental.UI.CheckBox();
			this.textClaimAttachPath = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.checkEclaimsSeparateTreatProv = new OpenDental.UI.CheckBox();
			this.comboZeroDollarProcClaimBehavior = new OpenDental.UI.ComboBox();
			this.label55 = new System.Windows.Forms.Label();
			this.checkPriClaimAllowSetToHoldUntilPriReceived = new OpenDental.UI.CheckBox();
			this.groupBoxClaimIdPrefix = new OpenDental.UI.GroupBox();
			this.butReplacements = new OpenDental.UI.Button();
			this.textClaimIdentifier = new System.Windows.Forms.TextBox();
			this.checkClaimsValidateACN = new OpenDental.UI.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxClaimsMedical.SuspendLayout();
			this.groupBoxClaimsPayments.SuspendLayout();
			this.groupBoxOD2.SuspendLayout();
			this.groupBoxOD1.SuspendLayout();
			this.groupBoxClaimIdPrefix.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkCanadianPpoLabEst
			// 
			this.checkCanadianPpoLabEst.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCanadianPpoLabEst.Location = new System.Drawing.Point(93, 579);
			this.checkCanadianPpoLabEst.Name = "checkCanadianPpoLabEst";
			this.checkCanadianPpoLabEst.Size = new System.Drawing.Size(367, 17);
			this.checkCanadianPpoLabEst.TabIndex = 5;
			this.checkCanadianPpoLabEst.Text = "Canadian PPO insurance plans create lab estimates";
			// 
			// textInsWriteoffDescript
			// 
			this.textInsWriteoffDescript.Location = new System.Drawing.Point(330, 557);
			this.textInsWriteoffDescript.Name = "textInsWriteoffDescript";
			this.textInsWriteoffDescript.Size = new System.Drawing.Size(130, 20);
			this.textInsWriteoffDescript.TabIndex = 4;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(73, 560);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(254, 17);
			this.label17.TabIndex = 284;
			this.label17.Text = "PPO write-off description (blank for \"Write-off\")";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClaimIdPrefixDetails
			// 
			this.labelClaimIdPrefixDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelClaimIdPrefixDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelClaimIdPrefixDetails.Location = new System.Drawing.Point(476, 160);
			this.labelClaimIdPrefixDetails.Name = "labelClaimIdPrefixDetails";
			this.labelClaimIdPrefixDetails.Size = new System.Drawing.Size(498, 29);
			this.labelClaimIdPrefixDetails.TabIndex = 324;
			this.labelClaimIdPrefixDetails.Text = "Default is [PatNum]/. This prefix is followed by an auto-generated claim number. " +
    "Useful for internal tracking of claims.\r\n";
			this.labelClaimIdPrefixDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelEclaimsSeparateTreatProvDetails
			// 
			this.labelEclaimsSeparateTreatProvDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelEclaimsSeparateTreatProvDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelEclaimsSeparateTreatProvDetails.Location = new System.Drawing.Point(476, 60);
			this.labelEclaimsSeparateTreatProvDetails.Name = "labelEclaimsSeparateTreatProvDetails";
			this.labelEclaimsSeparateTreatProvDetails.Size = new System.Drawing.Size(498, 17);
			this.labelEclaimsSeparateTreatProvDetails.TabIndex = 325;
			this.labelEclaimsSeparateTreatProvDetails.Text = "recommend checked";
			this.labelEclaimsSeparateTreatProvDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBoxClaimsMedical
			// 
			this.groupBoxClaimsMedical.Controls.Add(this.checkClaimMedTypeIsInstWhenInsPlanIsMedical);
			this.groupBoxClaimsMedical.Controls.Add(this.checkEclaimsMedicalProvTreatmentAsOrdering);
			this.groupBoxClaimsMedical.Location = new System.Drawing.Point(20, 503);
			this.groupBoxClaimsMedical.Name = "groupBoxClaimsMedical";
			this.groupBoxClaimsMedical.Size = new System.Drawing.Size(450, 51);
			this.groupBoxClaimsMedical.TabIndex = 3;
			this.groupBoxClaimsMedical.Text = "Claims Medical";
			// 
			// checkClaimMedTypeIsInstWhenInsPlanIsMedical
			// 
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Location = new System.Drawing.Point(85, 10);
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Name = "checkClaimMedTypeIsInstWhenInsPlanIsMedical";
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Size = new System.Drawing.Size(355, 17);
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.TabIndex = 0;
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Text = "Set medical claims to institutional when using medical insurance";
			// 
			// checkEclaimsMedicalProvTreatmentAsOrdering
			// 
			this.checkEclaimsMedicalProvTreatmentAsOrdering.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Location = new System.Drawing.Point(15, 29);
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Name = "checkEclaimsMedicalProvTreatmentAsOrdering";
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Size = new System.Drawing.Size(425, 17);
			this.checkEclaimsMedicalProvTreatmentAsOrdering.TabIndex = 1;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Text = "On medical e-claims, send treating provider as ordering provider by default";
			// 
			// groupBoxClaimsPayments
			// 
			this.groupBoxClaimsPayments.Controls.Add(this.checkClaimPaymentPickStatementType);
			this.groupBoxClaimsPayments.Controls.Add(this.checkNoInitialPrimaryInsMoreThanProc);
			this.groupBoxClaimsPayments.Controls.Add(this.checkAllowProcAdjFromClaim);
			this.groupBoxClaimsPayments.Controls.Add(this.checkInsPayNoWriteoffMoreThanProc);
			this.groupBoxClaimsPayments.Controls.Add(this.comboClaimCredit);
			this.groupBoxClaimsPayments.Controls.Add(this.labelClaimCredit);
			this.groupBoxClaimsPayments.Location = new System.Drawing.Point(20, 389);
			this.groupBoxClaimsPayments.Name = "groupBoxClaimsPayments";
			this.groupBoxClaimsPayments.Size = new System.Drawing.Size(450, 112);
			this.groupBoxClaimsPayments.TabIndex = 2;
			this.groupBoxClaimsPayments.Text = "Claims Payments";
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
			// comboClaimCredit
			// 
			this.comboClaimCredit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClaimCredit.Location = new System.Drawing.Point(272, 29);
			this.comboClaimCredit.Name = "comboClaimCredit";
			this.comboClaimCredit.Size = new System.Drawing.Size(168, 21);
			this.comboClaimCredit.TabIndex = 1;
			// 
			// labelClaimCredit
			// 
			this.labelClaimCredit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimCredit.Location = new System.Drawing.Point(30, 32);
			this.labelClaimCredit.Name = "labelClaimCredit";
			this.labelClaimCredit.Size = new System.Drawing.Size(239, 17);
			this.labelClaimCredit.TabIndex = 290;
			this.labelClaimCredit.Text = "Payment exceeds procedure balance";
			this.labelClaimCredit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxOD2
			// 
			this.groupBoxOD2.Controls.Add(this.checkInsAutoReceiveNoAssign);
			this.groupBoxOD2.Controls.Add(this.checkShowClaimPatResp);
			this.groupBoxOD2.Controls.Add(this.checkPromptForSecondaryClaim);
			this.groupBoxOD2.Controls.Add(this.checkClaimPrimaryRecievedForceSecondaryStatus);
			this.groupBoxOD2.Controls.Add(this.checkProviderIncomeShows);
			this.groupBoxOD2.Controls.Add(this.checkShowClaimPayTracking);
			this.groupBoxOD2.Controls.Add(this.checkInsEstRecalcReceived);
			this.groupBoxOD2.Controls.Add(this.checkClaimTrackingExcludeNone);
			this.groupBoxOD2.Location = new System.Drawing.Point(20, 222);
			this.groupBoxOD2.Name = "groupBoxOD2";
			this.groupBoxOD2.Size = new System.Drawing.Size(450, 165);
			this.groupBoxOD2.TabIndex = 1;
			this.groupBoxOD2.Text = "Claims Receive";
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
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.checkPpoUseUcr);
			this.groupBoxOD1.Controls.Add(this.checkClaimFormTreatDentSaysSigOnFile);
			this.groupBoxOD1.Controls.Add(this.textClaimAttachPath);
			this.groupBoxOD1.Controls.Add(this.label20);
			this.groupBoxOD1.Controls.Add(this.checkEclaimsSeparateTreatProv);
			this.groupBoxOD1.Controls.Add(this.comboZeroDollarProcClaimBehavior);
			this.groupBoxOD1.Controls.Add(this.label55);
			this.groupBoxOD1.Controls.Add(this.checkPriClaimAllowSetToHoldUntilPriReceived);
			this.groupBoxOD1.Controls.Add(this.groupBoxClaimIdPrefix);
			this.groupBoxOD1.Controls.Add(this.checkClaimsValidateACN);
			this.groupBoxOD1.Location = new System.Drawing.Point(20, 10);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(450, 210);
			this.groupBoxOD1.TabIndex = 0;
			this.groupBoxOD1.Text = "Claims Send";
			// 
			// checkPpoUseUcr
			// 
			this.checkPpoUseUcr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPpoUseUcr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPpoUseUcr.Location = new System.Drawing.Point(89, 10);
			this.checkPpoUseUcr.Name = "checkPpoUseUcr";
			this.checkPpoUseUcr.Size = new System.Drawing.Size(351, 17);
			this.checkPpoUseUcr.TabIndex = 0;
			this.checkPpoUseUcr.Text = "Use UCR fee for billed fee even if PPO fee is higher";
			// 
			// checkClaimFormTreatDentSaysSigOnFile
			// 
			this.checkClaimFormTreatDentSaysSigOnFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimFormTreatDentSaysSigOnFile.Location = new System.Drawing.Point(40, 93);
			this.checkClaimFormTreatDentSaysSigOnFile.Name = "checkClaimFormTreatDentSaysSigOnFile";
			this.checkClaimFormTreatDentSaysSigOnFile.Size = new System.Drawing.Size(400, 17);
			this.checkClaimFormTreatDentSaysSigOnFile.TabIndex = 4;
			this.checkClaimFormTreatDentSaysSigOnFile.Text = "Claim form treating provider shows \'Signature On File\' rather than name";
			// 
			// textClaimAttachPath
			// 
			this.textClaimAttachPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimAttachPath.Location = new System.Drawing.Point(243, 29);
			this.textClaimAttachPath.Name = "textClaimAttachPath";
			this.textClaimAttachPath.Size = new System.Drawing.Size(197, 20);
			this.textClaimAttachPath.TabIndex = 1;
			// 
			// label20
			// 
			this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label20.Location = new System.Drawing.Point(56, 32);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(184, 17);
			this.label20.TabIndex = 279;
			this.label20.Text = "Claim attachment export path";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEclaimsSeparateTreatProv
			// 
			this.checkEclaimsSeparateTreatProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEclaimsSeparateTreatProv.Location = new System.Drawing.Point(50, 51);
			this.checkEclaimsSeparateTreatProv.Name = "checkEclaimsSeparateTreatProv";
			this.checkEclaimsSeparateTreatProv.Size = new System.Drawing.Size(390, 17);
			this.checkEclaimsSeparateTreatProv.TabIndex = 2;
			this.checkEclaimsSeparateTreatProv.Text = "On e-claims, send treating provider info for each separate procedure";
			// 
			// comboZeroDollarProcClaimBehavior
			// 
			this.comboZeroDollarProcClaimBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboZeroDollarProcClaimBehavior.Location = new System.Drawing.Point(272, 70);
			this.comboZeroDollarProcClaimBehavior.Name = "comboZeroDollarProcClaimBehavior";
			this.comboZeroDollarProcClaimBehavior.Size = new System.Drawing.Size(168, 21);
			this.comboZeroDollarProcClaimBehavior.TabIndex = 3;
			// 
			// label55
			// 
			this.label55.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label55.Location = new System.Drawing.Point(56, 73);
			this.label55.Name = "label55";
			this.label55.Size = new System.Drawing.Size(213, 17);
			this.label55.TabIndex = 292;
			this.label55.Text = "Creating claims with $0 procedures";
			this.label55.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPriClaimAllowSetToHoldUntilPriReceived
			// 
			this.checkPriClaimAllowSetToHoldUntilPriReceived.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Location = new System.Drawing.Point(73, 131);
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Name = "checkPriClaimAllowSetToHoldUntilPriReceived";
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Size = new System.Drawing.Size(367, 17);
			this.checkPriClaimAllowSetToHoldUntilPriReceived.TabIndex = 6;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Text = "Allow primary claim status to be \'Hold Until Pri Received\'";
			// 
			// groupBoxClaimIdPrefix
			// 
			this.groupBoxClaimIdPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxClaimIdPrefix.BackColor = System.Drawing.Color.White;
			this.groupBoxClaimIdPrefix.Controls.Add(this.butReplacements);
			this.groupBoxClaimIdPrefix.Controls.Add(this.textClaimIdentifier);
			this.groupBoxClaimIdPrefix.Location = new System.Drawing.Point(40, 150);
			this.groupBoxClaimIdPrefix.Name = "groupBoxClaimIdPrefix";
			this.groupBoxClaimIdPrefix.Size = new System.Drawing.Size(407, 55);
			this.groupBoxClaimIdPrefix.TabIndex = 7;
			this.groupBoxClaimIdPrefix.Text = "Claim Identification Prefix";
			// 
			// butReplacements
			// 
			this.butReplacements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butReplacements.Location = new System.Drawing.Point(309, 27);
			this.butReplacements.Name = "butReplacements";
			this.butReplacements.Size = new System.Drawing.Size(91, 23);
			this.butReplacements.TabIndex = 1;
			this.butReplacements.Text = "Replacements";
			this.butReplacements.UseVisualStyleBackColor = true;
			this.butReplacements.Click += new System.EventHandler(this.butReplacements_Click);
			// 
			// textClaimIdentifier
			// 
			this.textClaimIdentifier.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimIdentifier.Location = new System.Drawing.Point(229, 5);
			this.textClaimIdentifier.Name = "textClaimIdentifier";
			this.textClaimIdentifier.Size = new System.Drawing.Size(171, 20);
			this.textClaimIdentifier.TabIndex = 0;
			// 
			// checkClaimsValidateACN
			// 
			this.checkClaimsValidateACN.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimsValidateACN.Location = new System.Drawing.Point(73, 112);
			this.checkClaimsValidateACN.Name = "checkClaimsValidateACN";
			this.checkClaimsValidateACN.Size = new System.Drawing.Size(367, 17);
			this.checkClaimsValidateACN.TabIndex = 5;
			this.checkClaimsValidateACN.Text = "Require ACN# in remarks on claims with ADDP group name";
			// 
			// label1
			// 
			this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
			this.label1.Location = new System.Drawing.Point(476, 365);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(455, 17);
			this.label1.TabIndex = 326;
			this.label1.Text = "Only applies to claims with no payments entered and no received claim procedures." +
    "";
			// 
			// UserControlAccountInsurance
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelEclaimsSeparateTreatProvDetails);
			this.Controls.Add(this.labelClaimIdPrefixDetails);
			this.Controls.Add(this.groupBoxClaimsMedical);
			this.Controls.Add(this.groupBoxClaimsPayments);
			this.Controls.Add(this.groupBoxOD2);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.textInsWriteoffDescript);
			this.Controls.Add(this.checkCanadianPpoLabEst);
			this.Name = "UserControlAccountInsurance";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupBoxClaimsMedical.ResumeLayout(false);
			this.groupBoxClaimsPayments.ResumeLayout(false);
			this.groupBoxOD2.ResumeLayout(false);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD1.PerformLayout();
			this.groupBoxClaimIdPrefix.ResumeLayout(false);
			this.groupBoxClaimIdPrefix.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.CheckBox checkClaimPrimaryRecievedForceSecondaryStatus;
		private OpenDental.UI.CheckBox checkShowClaimPayTracking;
		private OpenDental.UI.CheckBox checkShowClaimPatResp;
		private OpenDental.UI.CheckBox checkPriClaimAllowSetToHoldUntilPriReceived;
		private OpenDental.UI.CheckBox checkCanadianPpoLabEst;
		private OpenDental.UI.CheckBox checkInsEstRecalcReceived;
		private OpenDental.UI.CheckBox checkPromptForSecondaryClaim;
		private OpenDental.UI.CheckBox checkInsPayNoWriteoffMoreThanProc;
		private OpenDental.UI.CheckBox checkClaimTrackingExcludeNone;
		private System.Windows.Forms.Label label55;
		private UI.ComboBox comboZeroDollarProcClaimBehavior;
		private System.Windows.Forms.Label labelClaimCredit;
		private UI.ComboBox comboClaimCredit;
		private UI.GroupBox groupBoxClaimIdPrefix;
		private UI.Button butReplacements;
		private System.Windows.Forms.TextBox textClaimIdentifier;
		private OpenDental.UI.CheckBox checkAllowProcAdjFromClaim;
		private OpenDental.UI.CheckBox checkProviderIncomeShows;
		private OpenDental.UI.CheckBox checkClaimFormTreatDentSaysSigOnFile;
		private OpenDental.UI.CheckBox checkClaimMedTypeIsInstWhenInsPlanIsMedical;
		private OpenDental.UI.CheckBox checkEclaimsMedicalProvTreatmentAsOrdering;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox textClaimAttachPath;
		private OpenDental.UI.CheckBox checkClaimsValidateACN;
		private System.Windows.Forms.TextBox textInsWriteoffDescript;
		private System.Windows.Forms.Label label17;
		private UI.GroupBox groupBoxOD1;
		private UI.GroupBox groupBoxOD2;
		private OpenDental.UI.CheckBox checkPpoUseUcr;
		private UI.GroupBox groupBoxClaimsMedical;
		private UI.GroupBox groupBoxClaimsPayments;
		private System.Windows.Forms.Label labelClaimIdPrefixDetails;
		private OpenDental.UI.CheckBox checkNoInitialPrimaryInsMoreThanProc;
		private System.Windows.Forms.Label labelEclaimsSeparateTreatProvDetails;
		private OpenDental.UI.CheckBox checkEclaimsSeparateTreatProv;
		private OpenDental.UI.CheckBox checkInsAutoReceiveNoAssign;
		private OpenDental.UI.CheckBox checkClaimPaymentPickStatementType;
		private System.Windows.Forms.Label label1;
	}
        
}

