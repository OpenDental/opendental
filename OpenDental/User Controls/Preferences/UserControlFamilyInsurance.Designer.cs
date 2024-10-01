
namespace OpenDental {
	partial class UserControlFamilyInsurance {
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
			this.groupBoxPerVisitProcedureCodes = new OpenDental.UI.GroupBox();
			this.butPickPerVisitInsAmountProcCode = new OpenDental.UI.Button();
			this.butPickPerVisitPatAmountProcCode = new OpenDental.UI.Button();
			this.textPerVisitInsAmountProcCode = new System.Windows.Forms.TextBox();
			this.textPerVisitPatAmountProcCode = new System.Windows.Forms.TextBox();
			this.labelPerVisitInsAmount = new System.Windows.Forms.Label();
			this.labelPerVisitPatCopay = new System.Windows.Forms.Label();
			this.groupBoxCOB = new OpenDental.UI.GroupBox();
			this.comboCobSendPaidByInsAt = new OpenDental.UI.ComboBox();
			this.labelCobSendPaidByOtherInsAt = new System.Windows.Forms.Label();
			this.comboCobRule = new OpenDental.UI.ComboBox();
			this.labelCobRule = new System.Windows.Forms.Label();
			this.groupBoxClaims = new OpenDental.UI.GroupBox();
			this.checkClaimUseOverrideProcDescript = new OpenDental.UI.CheckBox();
			this.checkClaimTrackingRequireError = new OpenDental.UI.CheckBox();
			this.groupBoxOtherInsInfo = new OpenDental.UI.GroupBox();
			this.checkInsPlanExclusionsUseUCR = new OpenDental.UI.CheckBox();
			this.checkEnableZeroWriteoffOnLimitations = new OpenDental.UI.CheckBox();
			this.checkEnableZeroWriteoffOnAnnualMax = new OpenDental.UI.CheckBox();
			this.checkInsPlanExclusionsMarkDoNotBill = new OpenDental.UI.CheckBox();
			this.checkInsDefaultShowUCRonClaims = new OpenDental.UI.CheckBox();
			this.groupBoxInsuranceGeneral = new OpenDental.UI.GroupBox();
			this.checkPatInitBillingTypeFromPriInsPlan = new OpenDental.UI.CheckBox();
			this.checkInsurancePlansShared = new OpenDental.UI.CheckBox();
			this.checkFixedBenefitBlankLikeZero = new OpenDental.UI.CheckBox();
			this.checkCoPayFeeScheduleBlankLikeZero = new OpenDental.UI.CheckBox();
			this.checkInsPPOsecWriteoffs = new OpenDental.UI.CheckBox();
			this.checkPPOpercentage = new OpenDental.UI.CheckBox();
			this.checkInsDefaultAssignmentOfBenefits = new OpenDental.UI.CheckBox();
			this.groupBoxPerVisitProcedureCodes.SuspendLayout();
			this.groupBoxCOB.SuspendLayout();
			this.groupBoxClaims.SuspendLayout();
			this.groupBoxOtherInsInfo.SuspendLayout();
			this.groupBoxInsuranceGeneral.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxPerVisitProcedureCodes
			// 
			this.groupBoxPerVisitProcedureCodes.BackColor = System.Drawing.Color.White;
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.textPerVisitInsAmountProcCode);
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.textPerVisitPatAmountProcCode);
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.butPickPerVisitInsAmountProcCode);
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.butPickPerVisitPatAmountProcCode);
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.labelPerVisitInsAmount);
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.labelPerVisitPatCopay);
			this.groupBoxPerVisitProcedureCodes.Location = new System.Drawing.Point(20, 494);
			this.groupBoxPerVisitProcedureCodes.Name = "groupBoxPerVisitProcedureCodes";
			this.groupBoxPerVisitProcedureCodes.Size = new System.Drawing.Size(450, 78);
			this.groupBoxPerVisitProcedureCodes.TabIndex = 379;
			this.groupBoxPerVisitProcedureCodes.Text = "Per Visit";
			// 
			// butPickPerVisitInsAmountProcCode
			// 
			this.butPickPerVisitInsAmountProcCode.Location = new System.Drawing.Point(417, 45);
			this.butPickPerVisitInsAmountProcCode.Name = "butPickPerVisitInsAmountProcCode";
			this.butPickPerVisitInsAmountProcCode.Size = new System.Drawing.Size(23, 21);
			this.butPickPerVisitInsAmountProcCode.TabIndex = 291;
			this.butPickPerVisitInsAmountProcCode.Text = "...";
			this.butPickPerVisitInsAmountProcCode.Click += new System.EventHandler(this.butPickPerVisitInsAmountProcCode_Click);
			// 
			// butPickPerVisitPatAmountProcCode
			// 
			this.butPickPerVisitPatAmountProcCode.Location = new System.Drawing.Point(417, 14);
			this.butPickPerVisitPatAmountProcCode.Name = "butPickPerVisitPatAmountProcCode";
			this.butPickPerVisitPatAmountProcCode.Size = new System.Drawing.Size(23, 21);
			this.butPickPerVisitPatAmountProcCode.TabIndex = 290;
			this.butPickPerVisitPatAmountProcCode.Text = "...";
			this.butPickPerVisitPatAmountProcCode.Click += new System.EventHandler(this.butPickPerVisitPatAmountProcCode_Click);
			// 
			// textPerVisitInsAmountProcCode
			// 
			this.textPerVisitInsAmountProcCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textPerVisitInsAmountProcCode.Location = new System.Drawing.Point(312, 45);
			this.textPerVisitInsAmountProcCode.Name = "textPerVisitInsAmountProcCode";
			this.textPerVisitInsAmountProcCode.Size = new System.Drawing.Size(98, 20);
			this.textPerVisitInsAmountProcCode.TabIndex = 289;
			this.textPerVisitInsAmountProcCode.TabStop = false;
			// 
			// textPerVisitPatAmountProcCode
			// 
			this.textPerVisitPatAmountProcCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textPerVisitPatAmountProcCode.Location = new System.Drawing.Point(312, 15);
			this.textPerVisitPatAmountProcCode.Name = "textPerVisitPatAmountProcCode";
			this.textPerVisitPatAmountProcCode.Size = new System.Drawing.Size(98, 20);
			this.textPerVisitPatAmountProcCode.TabIndex = 288;
			this.textPerVisitPatAmountProcCode.TabStop = false;
			// 
			// labelPerVisitInsAmount
			// 
			this.labelPerVisitInsAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPerVisitInsAmount.Location = new System.Drawing.Point(69, 45);
			this.labelPerVisitInsAmount.Name = "labelPerVisitInsAmount";
			this.labelPerVisitInsAmount.Size = new System.Drawing.Size(242, 17);
			this.labelPerVisitInsAmount.TabIndex = 286;
			this.labelPerVisitInsAmount.Text = "Insurance procedure code";
			this.labelPerVisitInsAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPerVisitPatCopay
			// 
			this.labelPerVisitPatCopay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPerVisitPatCopay.Location = new System.Drawing.Point(69, 18);
			this.labelPerVisitPatCopay.Name = "labelPerVisitPatCopay";
			this.labelPerVisitPatCopay.Size = new System.Drawing.Size(242, 17);
			this.labelPerVisitPatCopay.TabIndex = 264;
			this.labelPerVisitPatCopay.Text = "Patient copay procedure code";
			this.labelPerVisitPatCopay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxCOB
			// 
			this.groupBoxCOB.BackColor = System.Drawing.Color.White;
			this.groupBoxCOB.Controls.Add(this.comboCobSendPaidByInsAt);
			this.groupBoxCOB.Controls.Add(this.comboCobRule);
			this.groupBoxCOB.Controls.Add(this.labelCobSendPaidByOtherInsAt);
			this.groupBoxCOB.Controls.Add(this.labelCobRule);
			this.groupBoxCOB.Location = new System.Drawing.Point(20, 410);
			this.groupBoxCOB.Name = "groupBoxCOB";
			this.groupBoxCOB.Size = new System.Drawing.Size(450, 78);
			this.groupBoxCOB.TabIndex = 376;
			this.groupBoxCOB.Text = "Coordination of Benefits (COB)";
			// 
			// comboCobSendPaidByInsAt
			// 
			this.comboCobSendPaidByInsAt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboCobSendPaidByInsAt.Location = new System.Drawing.Point(312, 42);
			this.comboCobSendPaidByInsAt.Name = "comboCobSendPaidByInsAt";
			this.comboCobSendPaidByInsAt.Size = new System.Drawing.Size(128, 21);
			this.comboCobSendPaidByInsAt.TabIndex = 287;
			// 
			// labelCobSendPaidByOtherInsAt
			// 
			this.labelCobSendPaidByOtherInsAt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCobSendPaidByOtherInsAt.Location = new System.Drawing.Point(69, 45);
			this.labelCobSendPaidByOtherInsAt.Name = "labelCobSendPaidByOtherInsAt";
			this.labelCobSendPaidByOtherInsAt.Size = new System.Drawing.Size(242, 17);
			this.labelCobSendPaidByOtherInsAt.TabIndex = 286;
			this.labelCobSendPaidByOtherInsAt.Text = "Send Paid By Other Insurance At";
			this.labelCobSendPaidByOtherInsAt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCobRule
			// 
			this.comboCobRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboCobRule.Location = new System.Drawing.Point(312, 15);
			this.comboCobRule.Name = "comboCobRule";
			this.comboCobRule.Size = new System.Drawing.Size(128, 21);
			this.comboCobRule.TabIndex = 262;
			this.comboCobRule.SelectionChangeCommitted += new System.EventHandler(this.comboCobRule_SelectionChangeCommitted);
			// 
			// labelCobRule
			// 
			this.labelCobRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCobRule.Location = new System.Drawing.Point(69, 18);
			this.labelCobRule.Name = "labelCobRule";
			this.labelCobRule.Size = new System.Drawing.Size(242, 17);
			this.labelCobRule.TabIndex = 264;
			this.labelCobRule.Text = "Coordination of Benefits (COB) rule";
			this.labelCobRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxClaims
			// 
			this.groupBoxClaims.Controls.Add(this.checkClaimUseOverrideProcDescript);
			this.groupBoxClaims.Controls.Add(this.checkClaimTrackingRequireError);
			this.groupBoxClaims.Location = new System.Drawing.Point(20, 339);
			this.groupBoxClaims.Name = "groupBoxClaims";
			this.groupBoxClaims.Size = new System.Drawing.Size(450, 65);
			this.groupBoxClaims.TabIndex = 280;
			this.groupBoxClaims.Text = "Claims";
			// 
			// checkClaimUseOverrideProcDescript
			// 
			this.checkClaimUseOverrideProcDescript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimUseOverrideProcDescript.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimUseOverrideProcDescript.Location = new System.Drawing.Point(40, 15);
			this.checkClaimUseOverrideProcDescript.Name = "checkClaimUseOverrideProcDescript";
			this.checkClaimUseOverrideProcDescript.Size = new System.Drawing.Size(400, 17);
			this.checkClaimUseOverrideProcDescript.TabIndex = 263;
			this.checkClaimUseOverrideProcDescript.Text = "Use the description for the charted procedure code on printed claims";
			// 
			// checkClaimTrackingRequireError
			// 
			this.checkClaimTrackingRequireError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimTrackingRequireError.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimTrackingRequireError.Location = new System.Drawing.Point(15, 38);
			this.checkClaimTrackingRequireError.Name = "checkClaimTrackingRequireError";
			this.checkClaimTrackingRequireError.Size = new System.Drawing.Size(425, 17);
			this.checkClaimTrackingRequireError.TabIndex = 266;
			this.checkClaimTrackingRequireError.Text = "Require error code when adding claim custom tracking status";
			// 
			// groupBoxOtherInsInfo
			// 
			this.groupBoxOtherInsInfo.Controls.Add(this.checkInsPlanExclusionsUseUCR);
			this.groupBoxOtherInsInfo.Controls.Add(this.checkEnableZeroWriteoffOnLimitations);
			this.groupBoxOtherInsInfo.Controls.Add(this.checkEnableZeroWriteoffOnAnnualMax);
			this.groupBoxOtherInsInfo.Controls.Add(this.checkInsPlanExclusionsMarkDoNotBill);
			this.groupBoxOtherInsInfo.Location = new System.Drawing.Point(20, 219);
			this.groupBoxOtherInsInfo.Name = "groupBoxOtherInsInfo";
			this.groupBoxOtherInsInfo.Size = new System.Drawing.Size(450, 114);
			this.groupBoxOtherInsInfo.TabIndex = 279;
			this.groupBoxOtherInsInfo.Text = "Write-Offs for Non-Covered Services";
			// 
			// checkInsPlanExclusionsUseUCR
			// 
			this.checkInsPlanExclusionsUseUCR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsPlanExclusionsUseUCR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPlanExclusionsUseUCR.Location = new System.Drawing.Point(18, 15);
			this.checkInsPlanExclusionsUseUCR.Name = "checkInsPlanExclusionsUseUCR";
			this.checkInsPlanExclusionsUseUCR.Size = new System.Drawing.Size(422, 17);
			this.checkInsPlanExclusionsUseUCR.TabIndex = 277;
			this.checkInsPlanExclusionsUseUCR.Text = "Ins plans with exclusions use UCR fee (zero out write-offs)";
			// 
			// checkEnableZeroWriteoffOnLimitations
			// 
			this.checkEnableZeroWriteoffOnLimitations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkEnableZeroWriteoffOnLimitations.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableZeroWriteoffOnLimitations.Location = new System.Drawing.Point(18, 83);
			this.checkEnableZeroWriteoffOnLimitations.Name = "checkEnableZeroWriteoffOnLimitations";
			this.checkEnableZeroWriteoffOnLimitations.Size = new System.Drawing.Size(422, 17);
			this.checkEnableZeroWriteoffOnLimitations.TabIndex = 373;
			this.checkEnableZeroWriteoffOnLimitations.Text = "Ins plans use UCR fee (zero out write-offs) when frequency or age limits are met";
			// 
			// checkEnableZeroWriteoffOnAnnualMax
			// 
			this.checkEnableZeroWriteoffOnAnnualMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkEnableZeroWriteoffOnAnnualMax.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableZeroWriteoffOnAnnualMax.Location = new System.Drawing.Point(18, 60);
			this.checkEnableZeroWriteoffOnAnnualMax.Name = "checkEnableZeroWriteoffOnAnnualMax";
			this.checkEnableZeroWriteoffOnAnnualMax.Size = new System.Drawing.Size(422, 17);
			this.checkEnableZeroWriteoffOnAnnualMax.TabIndex = 372;
			this.checkEnableZeroWriteoffOnAnnualMax.Text = "Ins plans use UCR fee (zero out write-offs) when annual max is met";
			// 
			// checkInsPlanExclusionsMarkDoNotBill
			// 
			this.checkInsPlanExclusionsMarkDoNotBill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsPlanExclusionsMarkDoNotBill.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPlanExclusionsMarkDoNotBill.Location = new System.Drawing.Point(18, 38);
			this.checkInsPlanExclusionsMarkDoNotBill.Name = "checkInsPlanExclusionsMarkDoNotBill";
			this.checkInsPlanExclusionsMarkDoNotBill.Size = new System.Drawing.Size(422, 17);
			this.checkInsPlanExclusionsMarkDoNotBill.TabIndex = 276;
			this.checkInsPlanExclusionsMarkDoNotBill.Text = "Ins plans with exclusions mark as Do Not Bill Ins";
			// 
			// checkInsDefaultShowUCRonClaims
			// 
			this.checkInsDefaultShowUCRonClaims.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsDefaultShowUCRonClaims.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsDefaultShowUCRonClaims.Location = new System.Drawing.Point(40, 176);
			this.checkInsDefaultShowUCRonClaims.Name = "checkInsDefaultShowUCRonClaims";
			this.checkInsDefaultShowUCRonClaims.Size = new System.Drawing.Size(400, 17);
			this.checkInsDefaultShowUCRonClaims.TabIndex = 261;
			this.checkInsDefaultShowUCRonClaims.Text = "Insurance plans default to show UCR fee on claims";
			this.checkInsDefaultShowUCRonClaims.Click += new System.EventHandler(this.checkInsDefaultShowUCRonClaims_Click);
			// 
			// groupBoxInsuranceGeneral
			// 
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkInsDefaultShowUCRonClaims);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkPatInitBillingTypeFromPriInsPlan);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkInsurancePlansShared);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkFixedBenefitBlankLikeZero);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkCoPayFeeScheduleBlankLikeZero);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkInsPPOsecWriteoffs);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkPPOpercentage);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkInsDefaultAssignmentOfBenefits);
			this.groupBoxInsuranceGeneral.Location = new System.Drawing.Point(20, 10);
			this.groupBoxInsuranceGeneral.Name = "groupBoxInsuranceGeneral";
			this.groupBoxInsuranceGeneral.Size = new System.Drawing.Size(450, 203);
			this.groupBoxInsuranceGeneral.TabIndex = 278;
			this.groupBoxInsuranceGeneral.Text = "Insurance General";
			// 
			// checkPatInitBillingTypeFromPriInsPlan
			// 
			this.checkPatInitBillingTypeFromPriInsPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPatInitBillingTypeFromPriInsPlan.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatInitBillingTypeFromPriInsPlan.Location = new System.Drawing.Point(40, 153);
			this.checkPatInitBillingTypeFromPriInsPlan.Name = "checkPatInitBillingTypeFromPriInsPlan";
			this.checkPatInitBillingTypeFromPriInsPlan.Size = new System.Drawing.Size(400, 17);
			this.checkPatInitBillingTypeFromPriInsPlan.TabIndex = 298;
			this.checkPatInitBillingTypeFromPriInsPlan.Text = "Adding new primary insurance plan to patient sets billing type";
			// 
			// checkInsurancePlansShared
			// 
			this.checkInsurancePlansShared.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsurancePlansShared.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsurancePlansShared.Location = new System.Drawing.Point(40, 15);
			this.checkInsurancePlansShared.Name = "checkInsurancePlansShared";
			this.checkInsurancePlansShared.Size = new System.Drawing.Size(400, 17);
			this.checkInsurancePlansShared.TabIndex = 257;
			this.checkInsurancePlansShared.Text = "InsPlan option at bottom, \'Change Plan for all subscribers\', is default";
			// 
			// checkFixedBenefitBlankLikeZero
			// 
			this.checkFixedBenefitBlankLikeZero.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkFixedBenefitBlankLikeZero.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFixedBenefitBlankLikeZero.Location = new System.Drawing.Point(40, 84);
			this.checkFixedBenefitBlankLikeZero.Name = "checkFixedBenefitBlankLikeZero";
			this.checkFixedBenefitBlankLikeZero.Size = new System.Drawing.Size(400, 17);
			this.checkFixedBenefitBlankLikeZero.TabIndex = 275;
			this.checkFixedBenefitBlankLikeZero.Text = "Fixed benefit fee schedules treat blank entries as zero";
			// 
			// checkCoPayFeeScheduleBlankLikeZero
			// 
			this.checkCoPayFeeScheduleBlankLikeZero.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCoPayFeeScheduleBlankLikeZero.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCoPayFeeScheduleBlankLikeZero.Location = new System.Drawing.Point(40, 61);
			this.checkCoPayFeeScheduleBlankLikeZero.Name = "checkCoPayFeeScheduleBlankLikeZero";
			this.checkCoPayFeeScheduleBlankLikeZero.Size = new System.Drawing.Size(400, 17);
			this.checkCoPayFeeScheduleBlankLikeZero.TabIndex = 260;
			this.checkCoPayFeeScheduleBlankLikeZero.Text = "Copay fee schedules treat blank entries as zero";
			// 
			// checkInsPPOsecWriteoffs
			// 
			this.checkInsPPOsecWriteoffs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsPPOsecWriteoffs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPPOsecWriteoffs.Location = new System.Drawing.Point(15, 130);
			this.checkInsPPOsecWriteoffs.Name = "checkInsPPOsecWriteoffs";
			this.checkInsPPOsecWriteoffs.Size = new System.Drawing.Size(425, 17);
			this.checkInsPPOsecWriteoffs.TabIndex = 270;
			this.checkInsPPOsecWriteoffs.Text = "Calculate secondary insurance PPO write-offs";
			// 
			// checkPPOpercentage
			// 
			this.checkPPOpercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPPOpercentage.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPPOpercentage.Location = new System.Drawing.Point(40, 38);
			this.checkPPOpercentage.Name = "checkPPOpercentage";
			this.checkPPOpercentage.Size = new System.Drawing.Size(400, 17);
			this.checkPPOpercentage.TabIndex = 258;
			this.checkPPOpercentage.Text = "Default new insurance plans to PPO Percentage plan type";
			// 
			// checkInsDefaultAssignmentOfBenefits
			// 
			this.checkInsDefaultAssignmentOfBenefits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsDefaultAssignmentOfBenefits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsDefaultAssignmentOfBenefits.Location = new System.Drawing.Point(40, 107);
			this.checkInsDefaultAssignmentOfBenefits.Name = "checkInsDefaultAssignmentOfBenefits";
			this.checkInsDefaultAssignmentOfBenefits.Size = new System.Drawing.Size(400, 17);
			this.checkInsDefaultAssignmentOfBenefits.TabIndex = 267;
			this.checkInsDefaultAssignmentOfBenefits.Text = "Insurance plans default to assignment of benefits";
			this.checkInsDefaultAssignmentOfBenefits.Click += new System.EventHandler(this.checkInsDefaultAssignmentOfBenefits_Click);
			// 
			// UserControlFamilyInsurance
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.groupBoxPerVisitProcedureCodes);
			this.Controls.Add(this.groupBoxCOB);
			this.Controls.Add(this.groupBoxClaims);
			this.Controls.Add(this.groupBoxOtherInsInfo);
			this.Controls.Add(this.groupBoxInsuranceGeneral);
			this.Name = "UserControlFamilyInsurance";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupBoxPerVisitProcedureCodes.ResumeLayout(false);
			this.groupBoxPerVisitProcedureCodes.PerformLayout();
			this.groupBoxCOB.ResumeLayout(false);
			this.groupBoxClaims.ResumeLayout(false);
			this.groupBoxOtherInsInfo.ResumeLayout(false);
			this.groupBoxInsuranceGeneral.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.CheckBox checkInsurancePlansShared;
		private OpenDental.UI.CheckBox checkCoPayFeeScheduleBlankLikeZero;
		private OpenDental.UI.CheckBox checkInsDefaultShowUCRonClaims;
		private OpenDental.UI.CheckBox checkInsPPOsecWriteoffs;
		private OpenDental.UI.CheckBox checkInsDefaultAssignmentOfBenefits;
		private OpenDental.UI.CheckBox checkClaimUseOverrideProcDescript;
		private OpenDental.UI.CheckBox checkInsPlanExclusionsUseUCR;
		private OpenDental.UI.CheckBox checkPPOpercentage;
		private OpenDental.UI.CheckBox checkInsPlanExclusionsMarkDoNotBill;
		private OpenDental.UI.CheckBox checkClaimTrackingRequireError;
		private OpenDental.UI.CheckBox checkFixedBenefitBlankLikeZero;
		private UI.GroupBox groupBoxInsuranceGeneral;
		private OpenDental.UI.CheckBox checkPatInitBillingTypeFromPriInsPlan;
		private UI.GroupBox groupBoxOtherInsInfo;
		private UI.GroupBox groupBoxClaims;
		private OpenDental.UI.CheckBox checkEnableZeroWriteoffOnLimitations;
		private OpenDental.UI.CheckBox checkEnableZeroWriteoffOnAnnualMax;
		private UI.GroupBox groupBoxCOB;
		private UI.ComboBox comboCobSendPaidByInsAt;
		private System.Windows.Forms.Label labelCobSendPaidByOtherInsAt;
		private UI.ComboBox comboCobRule;
		private System.Windows.Forms.Label labelCobRule;
		private OpenDental.UI.GroupBox groupBoxPerVisitProcedureCodes;
		private System.Windows.Forms.Label labelPerVisitInsAmount;
		private System.Windows.Forms.Label labelPerVisitPatCopay;
		private System.Windows.Forms.TextBox textPerVisitInsAmountProcCode;
		private System.Windows.Forms.TextBox textPerVisitPatAmountProcCode;
		private UI.Button butPickPerVisitInsAmountProcCode;
		private UI.Button butPickPerVisitPatAmountProcCode;
	}
}
