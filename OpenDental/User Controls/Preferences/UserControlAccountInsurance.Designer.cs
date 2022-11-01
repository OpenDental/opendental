
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
			this.checkCanadianPpoLabEst = new System.Windows.Forms.CheckBox();
			this.textInsWriteoffDescript = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.labelClaimIdPrefixDetails = new System.Windows.Forms.Label();
			this.labelEclaimsSeparateTreatProvDetails = new System.Windows.Forms.Label();
			this.groupBoxClaimsMedical = new OpenDental.UI.GroupBoxOD();
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical = new System.Windows.Forms.CheckBox();
			this.checkEclaimsMedicalProvTreatmentAsOrdering = new System.Windows.Forms.CheckBox();
			this.groupBoxClaimsPayments = new OpenDental.UI.GroupBoxOD();
			this.checkNoInitialPrimaryInsMoreThanProc = new System.Windows.Forms.CheckBox();
			this.checkAllowProcAdjFromClaim = new System.Windows.Forms.CheckBox();
			this.checkInsPayNoWriteoffMoreThanProc = new System.Windows.Forms.CheckBox();
			this.comboClaimCredit = new OpenDental.UI.ComboBoxOD();
			this.labelClaimCredit = new System.Windows.Forms.Label();
			this.groupBoxOD2 = new OpenDental.UI.GroupBoxOD();
			this.checkInsAutoReceiveNoAssign = new System.Windows.Forms.CheckBox();
			this.checkShowClaimPatResp = new System.Windows.Forms.CheckBox();
			this.checkPromptForSecondaryClaim = new System.Windows.Forms.CheckBox();
			this.checkClaimPrimaryRecievedForceSecondaryStatus = new System.Windows.Forms.CheckBox();
			this.checkProviderIncomeShows = new System.Windows.Forms.CheckBox();
			this.checkShowClaimPayTracking = new System.Windows.Forms.CheckBox();
			this.checkInsEstRecalcReceived = new System.Windows.Forms.CheckBox();
			this.checkClaimTrackingExcludeNone = new System.Windows.Forms.CheckBox();
			this.groupBoxOD1 = new OpenDental.UI.GroupBoxOD();
			this.checkPpoUseUcr = new System.Windows.Forms.CheckBox();
			this.checkClaimFormTreatDentSaysSigOnFile = new System.Windows.Forms.CheckBox();
			this.textClaimAttachPath = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.checkEclaimsSeparateTreatProv = new System.Windows.Forms.CheckBox();
			this.comboZeroDollarProcClaimBehavior = new OpenDental.UI.ComboBoxOD();
			this.label55 = new System.Windows.Forms.Label();
			this.checkPriClaimAllowSetToHoldUntilPriReceived = new System.Windows.Forms.CheckBox();
			this.groupBoxClaimIdPrefix = new OpenDental.UI.GroupBoxOD();
			this.butReplacements = new OpenDental.UI.Button();
			this.textClaimIdentifier = new System.Windows.Forms.TextBox();
			this.checkClaimsValidateACN = new System.Windows.Forms.CheckBox();
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
			this.checkCanadianPpoLabEst.Location = new System.Drawing.Point(93, 559);
			this.checkCanadianPpoLabEst.Name = "checkCanadianPpoLabEst";
			this.checkCanadianPpoLabEst.Size = new System.Drawing.Size(367, 17);
			this.checkCanadianPpoLabEst.TabIndex = 5;
			this.checkCanadianPpoLabEst.Text = "Canadian PPO insurance plans create lab estimates";
			this.checkCanadianPpoLabEst.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCanadianPpoLabEst.UseVisualStyleBackColor = true;
			// 
			// textInsWriteoffDescript
			// 
			this.textInsWriteoffDescript.Location = new System.Drawing.Point(330, 537);
			this.textInsWriteoffDescript.Name = "textInsWriteoffDescript";
			this.textInsWriteoffDescript.Size = new System.Drawing.Size(130, 20);
			this.textInsWriteoffDescript.TabIndex = 4;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(73, 540);
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
			this.groupBoxClaimsMedical.Location = new System.Drawing.Point(20, 484);
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
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEclaimsMedicalProvTreatmentAsOrdering
			// 
			this.checkEclaimsMedicalProvTreatmentAsOrdering.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Location = new System.Drawing.Point(70, 29);
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Name = "checkEclaimsMedicalProvTreatmentAsOrdering";
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Size = new System.Drawing.Size(370, 17);
			this.checkEclaimsMedicalProvTreatmentAsOrdering.TabIndex = 1;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Text = "On medical e-claims, send treating provider as ordering provider by default";
			this.checkEclaimsMedicalProvTreatmentAsOrdering.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxClaimsPayments
			// 
			this.groupBoxClaimsPayments.Controls.Add(this.checkNoInitialPrimaryInsMoreThanProc);
			this.groupBoxClaimsPayments.Controls.Add(this.checkAllowProcAdjFromClaim);
			this.groupBoxClaimsPayments.Controls.Add(this.checkInsPayNoWriteoffMoreThanProc);
			this.groupBoxClaimsPayments.Controls.Add(this.comboClaimCredit);
			this.groupBoxClaimsPayments.Controls.Add(this.labelClaimCredit);
			this.groupBoxClaimsPayments.Location = new System.Drawing.Point(20, 389);
			this.groupBoxClaimsPayments.Name = "groupBoxClaimsPayments";
			this.groupBoxClaimsPayments.Size = new System.Drawing.Size(450, 93);
			this.groupBoxClaimsPayments.TabIndex = 2;
			this.groupBoxClaimsPayments.Text = "Claims Payments";
			// 
			// checkNoInitialPrimaryInsMoreThanProc
			// 
			this.checkNoInitialPrimaryInsMoreThanProc.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNoInitialPrimaryInsMoreThanProc.Location = new System.Drawing.Point(3, 71);
			this.checkNoInitialPrimaryInsMoreThanProc.Name = "checkNoInitialPrimaryInsMoreThanProc";
			this.checkNoInitialPrimaryInsMoreThanProc.Size = new System.Drawing.Size(437, 17);
			this.checkNoInitialPrimaryInsMoreThanProc.TabIndex = 3;
			this.checkNoInitialPrimaryInsMoreThanProc.Text = "Initial primary insurance payment and write-off cannot exceed adjusted procedure " +
    "fee";
			this.checkNoInitialPrimaryInsMoreThanProc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowProcAdjFromClaim
			// 
			this.checkAllowProcAdjFromClaim.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowProcAdjFromClaim.Location = new System.Drawing.Point(115, 10);
			this.checkAllowProcAdjFromClaim.Name = "checkAllowProcAdjFromClaim";
			this.checkAllowProcAdjFromClaim.Size = new System.Drawing.Size(325, 17);
			this.checkAllowProcAdjFromClaim.TabIndex = 0;
			this.checkAllowProcAdjFromClaim.Text = "Allow procedure adjustments from Edit Claim window";
			this.checkAllowProcAdjFromClaim.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkInsPayNoWriteoffMoreThanProc
			// 
			this.checkInsPayNoWriteoffMoreThanProc.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPayNoWriteoffMoreThanProc.Location = new System.Drawing.Point(73, 52);
			this.checkInsPayNoWriteoffMoreThanProc.Name = "checkInsPayNoWriteoffMoreThanProc";
			this.checkInsPayNoWriteoffMoreThanProc.Size = new System.Drawing.Size(367, 17);
			this.checkInsPayNoWriteoffMoreThanProc.TabIndex = 2;
			this.checkInsPayNoWriteoffMoreThanProc.Text = "Disallow write-offs greater than the adjusted procedure fee";
			this.checkInsPayNoWriteoffMoreThanProc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.checkInsAutoReceiveNoAssign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsAutoReceiveNoAssign.UseVisualStyleBackColor = true;
			// 
			// checkShowClaimPatResp
			// 
			this.checkShowClaimPatResp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowClaimPatResp.Location = new System.Drawing.Point(30, 105);
			this.checkShowClaimPatResp.Name = "checkShowClaimPatResp";
			this.checkShowClaimPatResp.Size = new System.Drawing.Size(410, 17);
			this.checkShowClaimPatResp.TabIndex = 5;
			this.checkShowClaimPatResp.Text = "Show Patient Responsibility column in the Edit Claim/Payment windows";
			this.checkShowClaimPatResp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPromptForSecondaryClaim
			// 
			this.checkPromptForSecondaryClaim.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPromptForSecondaryClaim.Location = new System.Drawing.Point(73, 48);
			this.checkPromptForSecondaryClaim.Name = "checkPromptForSecondaryClaim";
			this.checkPromptForSecondaryClaim.Size = new System.Drawing.Size(367, 17);
			this.checkPromptForSecondaryClaim.TabIndex = 2;
			this.checkPromptForSecondaryClaim.Text = "Prompt for secondary claims";
			this.checkPromptForSecondaryClaim.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimPrimaryRecievedForceSecondaryStatus
			// 
			this.checkClaimPrimaryRecievedForceSecondaryStatus.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimPrimaryRecievedForceSecondaryStatus.Location = new System.Drawing.Point(30, 124);
			this.checkClaimPrimaryRecievedForceSecondaryStatus.Name = "checkClaimPrimaryRecievedForceSecondaryStatus";
			this.checkClaimPrimaryRecievedForceSecondaryStatus.Size = new System.Drawing.Size(410, 17);
			this.checkClaimPrimaryRecievedForceSecondaryStatus.TabIndex = 6;
			this.checkClaimPrimaryRecievedForceSecondaryStatus.Text = "Remove \'Do Nothing\' for secondary claims with \'Hold until Pri received\'";
			this.checkClaimPrimaryRecievedForceSecondaryStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimPrimaryRecievedForceSecondaryStatus.UseVisualStyleBackColor = true;
			// 
			// checkProviderIncomeShows
			// 
			this.checkProviderIncomeShows.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProviderIncomeShows.Location = new System.Drawing.Point(30, 29);
			this.checkProviderIncomeShows.Name = "checkProviderIncomeShows";
			this.checkProviderIncomeShows.Size = new System.Drawing.Size(410, 17);
			this.checkProviderIncomeShows.TabIndex = 1;
			this.checkProviderIncomeShows.Text = "Show provider income transfer window after entering insurance payment";
			this.checkProviderIncomeShows.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowClaimPayTracking
			// 
			this.checkShowClaimPayTracking.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowClaimPayTracking.Location = new System.Drawing.Point(30, 86);
			this.checkShowClaimPayTracking.Name = "checkShowClaimPayTracking";
			this.checkShowClaimPayTracking.Size = new System.Drawing.Size(410, 17);
			this.checkShowClaimPayTracking.TabIndex = 4;
			this.checkShowClaimPayTracking.Text = "Show Payment Tracking column in the Edit Claim/Payment windows";
			this.checkShowClaimPayTracking.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkInsEstRecalcReceived
			// 
			this.checkInsEstRecalcReceived.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsEstRecalcReceived.Location = new System.Drawing.Point(73, 67);
			this.checkInsEstRecalcReceived.Name = "checkInsEstRecalcReceived";
			this.checkInsEstRecalcReceived.Size = new System.Drawing.Size(367, 17);
			this.checkInsEstRecalcReceived.TabIndex = 3;
			this.checkInsEstRecalcReceived.Text = "Recalculate estimates for received claim procedures";
			this.checkInsEstRecalcReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimTrackingExcludeNone
			// 
			this.checkClaimTrackingExcludeNone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimTrackingExcludeNone.Location = new System.Drawing.Point(100, 10);
			this.checkClaimTrackingExcludeNone.Name = "checkClaimTrackingExcludeNone";
			this.checkClaimTrackingExcludeNone.Size = new System.Drawing.Size(340, 17);
			this.checkClaimTrackingExcludeNone.TabIndex = 0;
			this.checkClaimTrackingExcludeNone.Text = "Exclude \'None\' as an option on Custom Tracking Status";
			this.checkClaimTrackingExcludeNone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.checkPpoUseUcr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimFormTreatDentSaysSigOnFile
			// 
			this.checkClaimFormTreatDentSaysSigOnFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimFormTreatDentSaysSigOnFile.Location = new System.Drawing.Point(40, 93);
			this.checkClaimFormTreatDentSaysSigOnFile.Name = "checkClaimFormTreatDentSaysSigOnFile";
			this.checkClaimFormTreatDentSaysSigOnFile.Size = new System.Drawing.Size(400, 17);
			this.checkClaimFormTreatDentSaysSigOnFile.TabIndex = 4;
			this.checkClaimFormTreatDentSaysSigOnFile.Text = "Claim form treating provider shows \'Signature On File\' rather than name";
			this.checkClaimFormTreatDentSaysSigOnFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.checkEclaimsSeparateTreatProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.checkPriClaimAllowSetToHoldUntilPriReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.UseVisualStyleBackColor = true;
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
			this.checkClaimsValidateACN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
			this.label1.Location = new System.Drawing.Point(476, 366);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(455, 17);
			this.label1.TabIndex = 327;
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
		private System.Windows.Forms.CheckBox checkClaimPrimaryRecievedForceSecondaryStatus;
		private System.Windows.Forms.CheckBox checkShowClaimPayTracking;
		private System.Windows.Forms.CheckBox checkShowClaimPatResp;
		private System.Windows.Forms.CheckBox checkPriClaimAllowSetToHoldUntilPriReceived;
		private System.Windows.Forms.CheckBox checkCanadianPpoLabEst;
		private System.Windows.Forms.CheckBox checkInsEstRecalcReceived;
		private System.Windows.Forms.CheckBox checkPromptForSecondaryClaim;
		private System.Windows.Forms.CheckBox checkInsPayNoWriteoffMoreThanProc;
		private System.Windows.Forms.CheckBox checkClaimTrackingExcludeNone;
		private System.Windows.Forms.Label label55;
		private UI.ComboBoxOD comboZeroDollarProcClaimBehavior;
		private System.Windows.Forms.Label labelClaimCredit;
		private UI.ComboBoxOD comboClaimCredit;
		private UI.GroupBoxOD groupBoxClaimIdPrefix;
		private UI.Button butReplacements;
		private System.Windows.Forms.TextBox textClaimIdentifier;
		private System.Windows.Forms.CheckBox checkAllowProcAdjFromClaim;
		private System.Windows.Forms.CheckBox checkProviderIncomeShows;
		private System.Windows.Forms.CheckBox checkClaimFormTreatDentSaysSigOnFile;
		private System.Windows.Forms.CheckBox checkClaimMedTypeIsInstWhenInsPlanIsMedical;
		private System.Windows.Forms.CheckBox checkEclaimsMedicalProvTreatmentAsOrdering;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox textClaimAttachPath;
		private System.Windows.Forms.CheckBox checkClaimsValidateACN;
		private System.Windows.Forms.TextBox textInsWriteoffDescript;
		private System.Windows.Forms.Label label17;
		private UI.GroupBoxOD groupBoxOD1;
		private UI.GroupBoxOD groupBoxOD2;
		private System.Windows.Forms.CheckBox checkPpoUseUcr;
		private UI.GroupBoxOD groupBoxClaimsMedical;
		private UI.GroupBoxOD groupBoxClaimsPayments;
		private System.Windows.Forms.Label labelClaimIdPrefixDetails;
		private System.Windows.Forms.CheckBox checkNoInitialPrimaryInsMoreThanProc;
		private System.Windows.Forms.Label labelEclaimsSeparateTreatProvDetails;
		private System.Windows.Forms.CheckBox checkEclaimsSeparateTreatProv;
		private System.Windows.Forms.CheckBox checkInsAutoReceiveNoAssign;
		private System.Windows.Forms.Label label1;
	}
}
