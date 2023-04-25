
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlFamilyInsurance));
			this.labelPPOpercentageDetails = new System.Windows.Forms.Label();
			this.labelPatInitBillingTypeFromPriInsPlanDetails = new System.Windows.Forms.Label();
			this.labelInsPlanExclusionsMarkDoNotBillDetails = new System.Windows.Forms.Label();
			this.labelInsPPOsecWriteoffsDetails = new System.Windows.Forms.Label();
			this.labelClaimUseOverrideProcDescriptDetails = new System.Windows.Forms.Label();
			this.linkLabelZeroOutWriteoffOnAgeOrFreq = new System.Windows.Forms.LinkLabel();
			this.linkLabelZeroOutWriteoffOnAnnualMax = new System.Windows.Forms.LinkLabel();
			this.linkLabelCobRuleDetails = new System.Windows.Forms.LinkLabel();
			this.labelCobSendPaidByInsAtDetails = new System.Windows.Forms.Label();
			this.labelPerVisit = new System.Windows.Forms.Label();
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
			this.butInsPlanExclusionsUseUCR = new OpenDental.UI.Button();
			this.butFixedBenefitBlankLikeZero = new OpenDental.UI.Button();
			this.butCoPayFeeScheduleBlankLikeZero = new OpenDental.UI.Button();
			this.groupBoxClaims = new OpenDental.UI.GroupBox();
			this.checkClaimUseOverrideProcDescript = new OpenDental.UI.CheckBox();
			this.checkClaimTrackingRequireError = new OpenDental.UI.CheckBox();
			this.groupBoxOtherInsInfo = new OpenDental.UI.GroupBox();
			this.checkInsDefaultShowUCRonClaims = new OpenDental.UI.CheckBox();
			this.checkInsPlanExclusionsUseUCR = new OpenDental.UI.CheckBox();
			this.checkEnableZeroWriteoffOnLimitations = new OpenDental.UI.CheckBox();
			this.checkEnableZeroWriteoffOnAnnualMax = new OpenDental.UI.CheckBox();
			this.checkInsPlanExclusionsMarkDoNotBill = new OpenDental.UI.CheckBox();
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
			// labelPPOpercentageDetails
			// 
			this.labelPPOpercentageDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPPOpercentageDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelPPOpercentageDetails.Location = new System.Drawing.Point(476, 47);
			this.labelPPOpercentageDetails.Name = "labelPPOpercentageDetails";
			this.labelPPOpercentageDetails.Size = new System.Drawing.Size(498, 17);
			this.labelPPOpercentageDetails.TabIndex = 358;
			this.labelPPOpercentageDetails.Text = "otherwise, default to \'Category Percentage\'";
			this.labelPPOpercentageDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPatInitBillingTypeFromPriInsPlanDetails
			// 
			this.labelPatInitBillingTypeFromPriInsPlanDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPatInitBillingTypeFromPriInsPlanDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelPatInitBillingTypeFromPriInsPlanDetails.Location = new System.Drawing.Point(476, 161);
			this.labelPatInitBillingTypeFromPriInsPlanDetails.Name = "labelPatInitBillingTypeFromPriInsPlanDetails";
			this.labelPatInitBillingTypeFromPriInsPlanDetails.Size = new System.Drawing.Size(498, 18);
			this.labelPatInitBillingTypeFromPriInsPlanDetails.TabIndex = 361;
			this.labelPatInitBillingTypeFromPriInsPlanDetails.Text = "but changing an existing insurance plan\'s billing type will not change the patien" +
    "t\'s billing type";
			this.labelPatInitBillingTypeFromPriInsPlanDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelInsPlanExclusionsMarkDoNotBillDetails
			// 
			this.labelInsPlanExclusionsMarkDoNotBillDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInsPlanExclusionsMarkDoNotBillDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelInsPlanExclusionsMarkDoNotBillDetails.Location = new System.Drawing.Point(476, 256);
			this.labelInsPlanExclusionsMarkDoNotBillDetails.Name = "labelInsPlanExclusionsMarkDoNotBillDetails";
			this.labelInsPlanExclusionsMarkDoNotBillDetails.Size = new System.Drawing.Size(498, 17);
			this.labelInsPlanExclusionsMarkDoNotBillDetails.TabIndex = 362;
			this.labelInsPlanExclusionsMarkDoNotBillDetails.Text = "only used when \"exclusions use UCR fee\" above is checked";
			this.labelInsPlanExclusionsMarkDoNotBillDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelInsPPOsecWriteoffsDetails
			// 
			this.labelInsPPOsecWriteoffsDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInsPPOsecWriteoffsDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelInsPPOsecWriteoffsDetails.Location = new System.Drawing.Point(476, 138);
			this.labelInsPPOsecWriteoffsDetails.Name = "labelInsPPOsecWriteoffsDetails";
			this.labelInsPPOsecWriteoffsDetails.Size = new System.Drawing.Size(498, 18);
			this.labelInsPPOsecWriteoffsDetails.TabIndex = 367;
			this.labelInsPPOsecWriteoffsDetails.Text = "not recommended";
			this.labelInsPPOsecWriteoffsDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelClaimUseOverrideProcDescriptDetails
			// 
			this.labelClaimUseOverrideProcDescriptDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelClaimUseOverrideProcDescriptDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelClaimUseOverrideProcDescriptDetails.Location = new System.Drawing.Point(476, 352);
			this.labelClaimUseOverrideProcDescriptDetails.Name = "labelClaimUseOverrideProcDescriptDetails";
			this.labelClaimUseOverrideProcDescriptDetails.Size = new System.Drawing.Size(498, 18);
			this.labelClaimUseOverrideProcDescriptDetails.TabIndex = 368;
			this.labelClaimUseOverrideProcDescriptDetails.Text = "useful when charting custom procedures, otherwise uses base procedure code descri" +
    "ption";
			this.labelClaimUseOverrideProcDescriptDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// linkLabelZeroOutWriteoffOnAgeOrFreq
			// 
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.ForeColor = System.Drawing.Color.MidnightBlue;
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.LinkArea = new System.Windows.Forms.LinkArea(36, 10);
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.LinkColor = System.Drawing.Color.MidnightBlue;
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.Location = new System.Drawing.Point(476, 301);
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.Name = "linkLabelZeroOutWriteoffOnAgeOrFreq";
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.Size = new System.Drawing.Size(498, 17);
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.TabIndex = 375;
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.TabStop = true;
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.Text = "can be overridden by plan; also see Unit Tests 118-123";
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.UseCompatibleTextRendering = true;
			this.linkLabelZeroOutWriteoffOnAgeOrFreq.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelZeroOutWriteoffOnAgeOrFreq_LinkClicked);
			// 
			// linkLabelZeroOutWriteoffOnAnnualMax
			// 
			this.linkLabelZeroOutWriteoffOnAnnualMax.ForeColor = System.Drawing.Color.MidnightBlue;
			this.linkLabelZeroOutWriteoffOnAnnualMax.LinkArea = new System.Windows.Forms.LinkArea(36, 10);
			this.linkLabelZeroOutWriteoffOnAnnualMax.LinkColor = System.Drawing.Color.MidnightBlue;
			this.linkLabelZeroOutWriteoffOnAnnualMax.Location = new System.Drawing.Point(476, 278);
			this.linkLabelZeroOutWriteoffOnAnnualMax.Name = "linkLabelZeroOutWriteoffOnAnnualMax";
			this.linkLabelZeroOutWriteoffOnAnnualMax.Size = new System.Drawing.Size(498, 17);
			this.linkLabelZeroOutWriteoffOnAnnualMax.TabIndex = 374;
			this.linkLabelZeroOutWriteoffOnAnnualMax.TabStop = true;
			this.linkLabelZeroOutWriteoffOnAnnualMax.Text = "can be overridden by plan; also see Unit Tests 115-117";
			this.linkLabelZeroOutWriteoffOnAnnualMax.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linkLabelZeroOutWriteoffOnAnnualMax.UseCompatibleTextRendering = true;
			this.linkLabelZeroOutWriteoffOnAnnualMax.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelZeroOutWriteoffOnAnnualMax_LinkClicked);
			// 
			// linkLabelCobRuleDetails
			// 
			this.linkLabelCobRuleDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.linkLabelCobRuleDetails.LinkArea = new System.Windows.Forms.LinkArea(4, 24);
			this.linkLabelCobRuleDetails.LinkColor = System.Drawing.Color.MidnightBlue;
			this.linkLabelCobRuleDetails.Location = new System.Drawing.Point(476, 428);
			this.linkLabelCobRuleDetails.Name = "linkLabelCobRuleDetails";
			this.linkLabelCobRuleDetails.Size = new System.Drawing.Size(498, 17);
			this.linkLabelCobRuleDetails.TabIndex = 377;
			this.linkLabelCobRuleDetails.TabStop = true;
			this.linkLabelCobRuleDetails.Text = "see Coordination of Benefits";
			this.linkLabelCobRuleDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linkLabelCobRuleDetails.UseCompatibleTextRendering = true;
			this.linkLabelCobRuleDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCobRuleDetails_LinkClicked);
			// 
			// labelCobSendPaidByInsAtDetails
			// 
			this.labelCobSendPaidByInsAtDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCobSendPaidByInsAtDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelCobSendPaidByInsAtDetails.Location = new System.Drawing.Point(476, 455);
			this.labelCobSendPaidByInsAtDetails.Name = "labelCobSendPaidByInsAtDetails";
			this.labelCobSendPaidByInsAtDetails.Size = new System.Drawing.Size(498, 17);
			this.labelCobSendPaidByInsAtDetails.TabIndex = 378;
			this.labelCobSendPaidByInsAtDetails.Text = "Claim Level means just the total for the claim, Procedure Level means the amount " +
    "for each procedure";
			this.labelCobSendPaidByInsAtDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPerVisit
			// 
			this.labelPerVisit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPerVisit.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelPerVisit.Location = new System.Drawing.Point(473, 509);
			this.labelPerVisit.Name = "labelPerVisit";
			this.labelPerVisit.Size = new System.Drawing.Size(498, 46);
			this.labelPerVisit.TabIndex = 380;
			this.labelPerVisit.Text = resources.GetString("labelPerVisit.Text");
			this.labelPerVisit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBoxPerVisitProcedureCodes
			// 
			this.groupBoxPerVisitProcedureCodes.BackColor = System.Drawing.Color.White;
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.butPickPerVisitInsAmountProcCode);
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.butPickPerVisitPatAmountProcCode);
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.textPerVisitInsAmountProcCode);
			this.groupBoxPerVisitProcedureCodes.Controls.Add(this.textPerVisitPatAmountProcCode);
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
			this.labelPerVisitInsAmount.Location = new System.Drawing.Point(67, 45);
			this.labelPerVisitInsAmount.Name = "labelPerVisitInsAmount";
			this.labelPerVisitInsAmount.Size = new System.Drawing.Size(242, 17);
			this.labelPerVisitInsAmount.TabIndex = 286;
			this.labelPerVisitInsAmount.Text = "Insurance procedure code";
			this.labelPerVisitInsAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPerVisitPatCopay
			// 
			this.labelPerVisitPatCopay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPerVisitPatCopay.Location = new System.Drawing.Point(67, 18);
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
			this.groupBoxCOB.Controls.Add(this.labelCobSendPaidByOtherInsAt);
			this.groupBoxCOB.Controls.Add(this.comboCobRule);
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
			this.labelCobSendPaidByOtherInsAt.Location = new System.Drawing.Point(67, 45);
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
			this.labelCobRule.Location = new System.Drawing.Point(67, 18);
			this.labelCobRule.Name = "labelCobRule";
			this.labelCobRule.Size = new System.Drawing.Size(242, 17);
			this.labelCobRule.TabIndex = 264;
			this.labelCobRule.Text = "Coordination of Benefits (COB) rule";
			this.labelCobRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butInsPlanExclusionsUseUCR
			// 
			this.butInsPlanExclusionsUseUCR.ForeColor = System.Drawing.Color.Black;
			this.butInsPlanExclusionsUseUCR.Location = new System.Drawing.Point(479, 231);
			this.butInsPlanExclusionsUseUCR.Name = "butInsPlanExclusionsUseUCR";
			this.butInsPlanExclusionsUseUCR.Size = new System.Drawing.Size(64, 21);
			this.butInsPlanExclusionsUseUCR.TabIndex = 366;
			this.butInsPlanExclusionsUseUCR.Text = "Details";
			this.butInsPlanExclusionsUseUCR.Click += new System.EventHandler(this.butInsPlanExclusionsUseUCR_Click);
			// 
			// butFixedBenefitBlankLikeZero
			// 
			this.butFixedBenefitBlankLikeZero.ForeColor = System.Drawing.Color.Black;
			this.butFixedBenefitBlankLikeZero.Location = new System.Drawing.Point(479, 91);
			this.butFixedBenefitBlankLikeZero.Name = "butFixedBenefitBlankLikeZero";
			this.butFixedBenefitBlankLikeZero.Size = new System.Drawing.Size(64, 21);
			this.butFixedBenefitBlankLikeZero.TabIndex = 365;
			this.butFixedBenefitBlankLikeZero.Text = "Examples";
			this.butFixedBenefitBlankLikeZero.Click += new System.EventHandler(this.butFixedBenefitBlankLikeZero_Click);
			// 
			// butCoPayFeeScheduleBlankLikeZero
			// 
			this.butCoPayFeeScheduleBlankLikeZero.ForeColor = System.Drawing.Color.Black;
			this.butCoPayFeeScheduleBlankLikeZero.Location = new System.Drawing.Point(479, 68);
			this.butCoPayFeeScheduleBlankLikeZero.Name = "butCoPayFeeScheduleBlankLikeZero";
			this.butCoPayFeeScheduleBlankLikeZero.Size = new System.Drawing.Size(64, 21);
			this.butCoPayFeeScheduleBlankLikeZero.TabIndex = 364;
			this.butCoPayFeeScheduleBlankLikeZero.Text = "Examples";
			this.butCoPayFeeScheduleBlankLikeZero.Click += new System.EventHandler(this.butCoPayFeeScheduleBlankLikeZero_Click);
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
			this.groupBoxOtherInsInfo.Controls.Add(this.checkInsDefaultShowUCRonClaims);
			this.groupBoxOtherInsInfo.Controls.Add(this.checkInsPlanExclusionsUseUCR);
			this.groupBoxOtherInsInfo.Controls.Add(this.checkEnableZeroWriteoffOnLimitations);
			this.groupBoxOtherInsInfo.Controls.Add(this.checkEnableZeroWriteoffOnAnnualMax);
			this.groupBoxOtherInsInfo.Controls.Add(this.checkInsPlanExclusionsMarkDoNotBill);
			this.groupBoxOtherInsInfo.Location = new System.Drawing.Point(20, 196);
			this.groupBoxOtherInsInfo.Name = "groupBoxOtherInsInfo";
			this.groupBoxOtherInsInfo.Size = new System.Drawing.Size(450, 137);
			this.groupBoxOtherInsInfo.TabIndex = 279;
			this.groupBoxOtherInsInfo.Text = "Other Insurance Info";
			// 
			// checkInsDefaultShowUCRonClaims
			// 
			this.checkInsDefaultShowUCRonClaims.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsDefaultShowUCRonClaims.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsDefaultShowUCRonClaims.Location = new System.Drawing.Point(40, 15);
			this.checkInsDefaultShowUCRonClaims.Name = "checkInsDefaultShowUCRonClaims";
			this.checkInsDefaultShowUCRonClaims.Size = new System.Drawing.Size(400, 17);
			this.checkInsDefaultShowUCRonClaims.TabIndex = 261;
			this.checkInsDefaultShowUCRonClaims.Text = "Insurance plans default to show UCR fee on claims";
			this.checkInsDefaultShowUCRonClaims.Click += new System.EventHandler(this.checkInsDefaultShowUCRonClaims_Click);
			// 
			// checkInsPlanExclusionsUseUCR
			// 
			this.checkInsPlanExclusionsUseUCR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsPlanExclusionsUseUCR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPlanExclusionsUseUCR.Location = new System.Drawing.Point(18, 38);
			this.checkInsPlanExclusionsUseUCR.Name = "checkInsPlanExclusionsUseUCR";
			this.checkInsPlanExclusionsUseUCR.Size = new System.Drawing.Size(422, 17);
			this.checkInsPlanExclusionsUseUCR.TabIndex = 277;
			this.checkInsPlanExclusionsUseUCR.Text = "Ins plans with exclusions use UCR fee";
			// 
			// checkEnableZeroWriteoffOnLimitations
			// 
			this.checkEnableZeroWriteoffOnLimitations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkEnableZeroWriteoffOnLimitations.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableZeroWriteoffOnLimitations.Location = new System.Drawing.Point(18, 106);
			this.checkEnableZeroWriteoffOnLimitations.Name = "checkEnableZeroWriteoffOnLimitations";
			this.checkEnableZeroWriteoffOnLimitations.Size = new System.Drawing.Size(422, 17);
			this.checkEnableZeroWriteoffOnLimitations.TabIndex = 373;
			this.checkEnableZeroWriteoffOnLimitations.Text = "Ins plans zero out write-offs when frequencies or age limits are met";
			// 
			// checkEnableZeroWriteoffOnAnnualMax
			// 
			this.checkEnableZeroWriteoffOnAnnualMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkEnableZeroWriteoffOnAnnualMax.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableZeroWriteoffOnAnnualMax.Location = new System.Drawing.Point(18, 83);
			this.checkEnableZeroWriteoffOnAnnualMax.Name = "checkEnableZeroWriteoffOnAnnualMax";
			this.checkEnableZeroWriteoffOnAnnualMax.Size = new System.Drawing.Size(422, 17);
			this.checkEnableZeroWriteoffOnAnnualMax.TabIndex = 372;
			this.checkEnableZeroWriteoffOnAnnualMax.Text = "Ins plans zero out write-offs when annual max is met";
			// 
			// checkInsPlanExclusionsMarkDoNotBill
			// 
			this.checkInsPlanExclusionsMarkDoNotBill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsPlanExclusionsMarkDoNotBill.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPlanExclusionsMarkDoNotBill.Location = new System.Drawing.Point(18, 61);
			this.checkInsPlanExclusionsMarkDoNotBill.Name = "checkInsPlanExclusionsMarkDoNotBill";
			this.checkInsPlanExclusionsMarkDoNotBill.Size = new System.Drawing.Size(422, 17);
			this.checkInsPlanExclusionsMarkDoNotBill.TabIndex = 276;
			this.checkInsPlanExclusionsMarkDoNotBill.Text = "Ins plans with exclusions mark as Do Not Bill Ins";
			// 
			// groupBoxInsuranceGeneral
			// 
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkPatInitBillingTypeFromPriInsPlan);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkInsurancePlansShared);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkFixedBenefitBlankLikeZero);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkCoPayFeeScheduleBlankLikeZero);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkInsPPOsecWriteoffs);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkPPOpercentage);
			this.groupBoxInsuranceGeneral.Controls.Add(this.checkInsDefaultAssignmentOfBenefits);
			this.groupBoxInsuranceGeneral.Location = new System.Drawing.Point(20, 10);
			this.groupBoxInsuranceGeneral.Name = "groupBoxInsuranceGeneral";
			this.groupBoxInsuranceGeneral.Size = new System.Drawing.Size(450, 180);
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
			this.Controls.Add(this.labelPerVisit);
			this.Controls.Add(this.groupBoxPerVisitProcedureCodes);
			this.Controls.Add(this.labelCobSendPaidByInsAtDetails);
			this.Controls.Add(this.linkLabelCobRuleDetails);
			this.Controls.Add(this.groupBoxCOB);
			this.Controls.Add(this.linkLabelZeroOutWriteoffOnAgeOrFreq);
			this.Controls.Add(this.linkLabelZeroOutWriteoffOnAnnualMax);
			this.Controls.Add(this.labelClaimUseOverrideProcDescriptDetails);
			this.Controls.Add(this.labelInsPPOsecWriteoffsDetails);
			this.Controls.Add(this.butInsPlanExclusionsUseUCR);
			this.Controls.Add(this.butFixedBenefitBlankLikeZero);
			this.Controls.Add(this.butCoPayFeeScheduleBlankLikeZero);
			this.Controls.Add(this.labelInsPlanExclusionsMarkDoNotBillDetails);
			this.Controls.Add(this.labelPatInitBillingTypeFromPriInsPlanDetails);
			this.Controls.Add(this.labelPPOpercentageDetails);
			this.Controls.Add(this.groupBoxClaims);
			this.Controls.Add(this.groupBoxOtherInsInfo);
			this.Controls.Add(this.groupBoxInsuranceGeneral);
			this.Name = "UserControlFamilyInsurance";
			this.Size = new System.Drawing.Size(974, 624);
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
		private System.Windows.Forms.Label labelPPOpercentageDetails;
		private System.Windows.Forms.Label labelPatInitBillingTypeFromPriInsPlanDetails;
		private System.Windows.Forms.Label labelInsPlanExclusionsMarkDoNotBillDetails;
		private UI.Button butCoPayFeeScheduleBlankLikeZero;
		private UI.Button butFixedBenefitBlankLikeZero;
		private UI.Button butInsPlanExclusionsUseUCR;
		private System.Windows.Forms.Label labelInsPPOsecWriteoffsDetails;
		private System.Windows.Forms.Label labelClaimUseOverrideProcDescriptDetails;
		private OpenDental.UI.CheckBox checkEnableZeroWriteoffOnLimitations;
		private OpenDental.UI.CheckBox checkEnableZeroWriteoffOnAnnualMax;
		private System.Windows.Forms.LinkLabel linkLabelZeroOutWriteoffOnAgeOrFreq;
		private System.Windows.Forms.LinkLabel linkLabelZeroOutWriteoffOnAnnualMax;
		private UI.GroupBox groupBoxCOB;
		private UI.ComboBox comboCobSendPaidByInsAt;
		private System.Windows.Forms.Label labelCobSendPaidByOtherInsAt;
		private UI.ComboBox comboCobRule;
		private System.Windows.Forms.Label labelCobRule;
		private System.Windows.Forms.LinkLabel linkLabelCobRuleDetails;
		private System.Windows.Forms.Label labelCobSendPaidByInsAtDetails;
		private OpenDental.UI.GroupBox groupBoxPerVisitProcedureCodes;
		private System.Windows.Forms.Label labelPerVisitInsAmount;
		private System.Windows.Forms.Label labelPerVisitPatCopay;
		private System.Windows.Forms.TextBox textPerVisitInsAmountProcCode;
		private System.Windows.Forms.TextBox textPerVisitPatAmountProcCode;
		private UI.Button butPickPerVisitInsAmountProcCode;
		private UI.Button butPickPerVisitPatAmountProcCode;
		private System.Windows.Forms.Label labelPerVisit;
	}
}
