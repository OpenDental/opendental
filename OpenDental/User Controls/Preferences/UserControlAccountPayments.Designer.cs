
namespace OpenDental {
	partial class UserControlAccountPayments {
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
			this.labelPayPlansUseSheetsDetails = new System.Windows.Forms.Label();
			this.labelDppUnearnedTypeDetails = new System.Windows.Forms.Label();
			this.labelStoreCCTokensDetails = new System.Windows.Forms.Label();
			this.groupBoxUnearnedIncome = new OpenDental.UI.GroupBoxOD();
			this.checkShowAllocateUnearnedPaymentPrompt = new System.Windows.Forms.CheckBox();
			this.checkAllowPrepayProvider = new System.Windows.Forms.CheckBox();
			this.comboUnallocatedSplits = new OpenDental.UI.ComboBoxOD();
			this.label28 = new System.Windows.Forms.Label();
			this.groupBoxPayPlans = new OpenDental.UI.GroupBoxOD();
			this.checkPayPlansExcludePastActivity = new System.Windows.Forms.CheckBox();
			this.labelTermsAndConditions = new System.Windows.Forms.Label();
			this.butPayPlanTermsAndConditions = new OpenDental.UI.Button();
			this.checkPayPlansUseSheets = new System.Windows.Forms.CheckBox();
			this.checkPayPlanSaveSignedPdf = new System.Windows.Forms.CheckBox();
			this.label39 = new System.Windows.Forms.Label();
			this.comboPayPlansVersion = new OpenDental.UI.ComboBoxOD();
			this.comboDppUnearnedType = new OpenDental.UI.ComboBoxOD();
			this.checkHideDueNow = new System.Windows.Forms.CheckBox();
			this.label59 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.textDynamicPayPlan = new OpenDental.ValidTime();
			this.groupBoxPayments = new OpenDental.UI.GroupBoxOD();
			this.checkOnlinePaymentsMarkAsProcessed = new System.Windows.Forms.CheckBox();
			this.checkStoreCCTokens = new System.Windows.Forms.CheckBox();
			this.checkPaymentCompletedDisableMerchantButtons = new System.Windows.Forms.CheckBox();
			this.comboPaymentClinicSetting = new OpenDental.UI.ComboBoxOD();
			this.checkIncTxfrTreatNegProdAsIncome = new System.Windows.Forms.CheckBox();
			this.label38 = new System.Windows.Forms.Label();
			this.checkAllowEmailCCReceipt = new System.Windows.Forms.CheckBox();
			this.checkPaymentsPromptForPayType = new System.Windows.Forms.CheckBox();
			this.butIncTxfrTreatNegProdAsIncomeDetails = new OpenDental.UI.Button();
			this.butPayPlansVersionDetails = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxUnearnedIncome.SuspendLayout();
			this.groupBoxPayPlans.SuspendLayout();
			this.groupBoxPayments.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelPayPlansUseSheetsDetails
			// 
			this.labelPayPlansUseSheetsDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPayPlansUseSheetsDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelPayPlansUseSheetsDetails.Location = new System.Drawing.Point(476, 237);
			this.labelPayPlansUseSheetsDetails.Name = "labelPayPlansUseSheetsDetails";
			this.labelPayPlansUseSheetsDetails.Size = new System.Drawing.Size(498, 17);
			this.labelPayPlansUseSheetsDetails.TabIndex = 330;
			this.labelPayPlansUseSheetsDetails.Text = "See Setup, Sheets";
			this.labelPayPlansUseSheetsDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDppUnearnedTypeDetails
			// 
			this.labelDppUnearnedTypeDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDppUnearnedTypeDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelDppUnearnedTypeDetails.Location = new System.Drawing.Point(476, 356);
			this.labelDppUnearnedTypeDetails.Name = "labelDppUnearnedTypeDetails";
			this.labelDppUnearnedTypeDetails.Size = new System.Drawing.Size(498, 30);
			this.labelDppUnearnedTypeDetails.TabIndex = 333;
			this.labelDppUnearnedTypeDetails.Text = "Hides prepayments from income transfer system. Only PaySplit unearned types marke" +
    "d as Do Not Show on Account are listed here.";
			this.labelDppUnearnedTypeDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelStoreCCTokensDetails
			// 
			this.labelStoreCCTokensDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStoreCCTokensDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelStoreCCTokensDetails.Location = new System.Drawing.Point(476, 29);
			this.labelStoreCCTokensDetails.Name = "labelStoreCCTokensDetails";
			this.labelStoreCCTokensDetails.Size = new System.Drawing.Size(498, 17);
			this.labelStoreCCTokensDetails.TabIndex = 334;
			this.labelStoreCCTokensDetails.Text = "for use with XCharge and PayConnect";
			this.labelStoreCCTokensDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBoxUnearnedIncome
			// 
			this.groupBoxUnearnedIncome.Controls.Add(this.checkShowAllocateUnearnedPaymentPrompt);
			this.groupBoxUnearnedIncome.Controls.Add(this.checkAllowPrepayProvider);
			this.groupBoxUnearnedIncome.Controls.Add(this.comboUnallocatedSplits);
			this.groupBoxUnearnedIncome.Controls.Add(this.label28);
			this.groupBoxUnearnedIncome.Location = new System.Drawing.Point(20, 425);
			this.groupBoxUnearnedIncome.Name = "groupBoxUnearnedIncome";
			this.groupBoxUnearnedIncome.Size = new System.Drawing.Size(450, 87);
			this.groupBoxUnearnedIncome.TabIndex = 323;
			this.groupBoxUnearnedIncome.Text = "Unearned Income";
			// 
			// checkShowAllocateUnearnedPaymentPrompt
			// 
			this.checkShowAllocateUnearnedPaymentPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowAllocateUnearnedPaymentPrompt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAllocateUnearnedPaymentPrompt.Location = new System.Drawing.Point(80, 60);
			this.checkShowAllocateUnearnedPaymentPrompt.Name = "checkShowAllocateUnearnedPaymentPrompt";
			this.checkShowAllocateUnearnedPaymentPrompt.Size = new System.Drawing.Size(360, 17);
			this.checkShowAllocateUnearnedPaymentPrompt.TabIndex = 307;
			this.checkShowAllocateUnearnedPaymentPrompt.Text = "Prompt user to allocate unearned income after creating a claim";
			this.checkShowAllocateUnearnedPaymentPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowPrepayProvider
			// 
			this.checkAllowPrepayProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowPrepayProvider.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowPrepayProvider.Location = new System.Drawing.Point(135, 10);
			this.checkAllowPrepayProvider.Name = "checkAllowPrepayProvider";
			this.checkAllowPrepayProvider.Size = new System.Drawing.Size(305, 17);
			this.checkAllowPrepayProvider.TabIndex = 303;
			this.checkAllowPrepayProvider.Text = "Allow assigning unearned income to providers";
			this.checkAllowPrepayProvider.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowPrepayProvider.UseVisualStyleBackColor = true;
			// 
			// comboUnallocatedSplits
			// 
			this.comboUnallocatedSplits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboUnallocatedSplits.Location = new System.Drawing.Point(315, 33);
			this.comboUnallocatedSplits.Name = "comboUnallocatedSplits";
			this.comboUnallocatedSplits.Size = new System.Drawing.Size(125, 21);
			this.comboUnallocatedSplits.TabIndex = 281;
			// 
			// label28
			// 
			this.label28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label28.Location = new System.Drawing.Point(51, 36);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(261, 17);
			this.label28.TabIndex = 282;
			this.label28.Text = "Default unearned type for unallocated paysplits";
			this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxPayPlans
			// 
			this.groupBoxPayPlans.Controls.Add(this.checkPayPlansExcludePastActivity);
			this.groupBoxPayPlans.Controls.Add(this.labelTermsAndConditions);
			this.groupBoxPayPlans.Controls.Add(this.butPayPlanTermsAndConditions);
			this.groupBoxPayPlans.Controls.Add(this.checkPayPlansUseSheets);
			this.groupBoxPayPlans.Controls.Add(this.checkPayPlanSaveSignedPdf);
			this.groupBoxPayPlans.Controls.Add(this.label39);
			this.groupBoxPayPlans.Controls.Add(this.comboPayPlansVersion);
			this.groupBoxPayPlans.Controls.Add(this.comboDppUnearnedType);
			this.groupBoxPayPlans.Controls.Add(this.checkHideDueNow);
			this.groupBoxPayPlans.Controls.Add(this.label59);
			this.groupBoxPayPlans.Controls.Add(this.label27);
			this.groupBoxPayPlans.Controls.Add(this.textDynamicPayPlan);
			this.groupBoxPayPlans.Location = new System.Drawing.Point(20, 205);
			this.groupBoxPayPlans.Name = "groupBoxPayPlans";
			this.groupBoxPayPlans.Size = new System.Drawing.Size(450, 214);
			this.groupBoxPayPlans.TabIndex = 322;
			this.groupBoxPayPlans.Text = "Pay Plans";
			// 
			// checkPayPlansExcludePastActivity
			// 
			this.checkPayPlansExcludePastActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPayPlansExcludePastActivity.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPayPlansExcludePastActivity.Location = new System.Drawing.Point(122, 10);
			this.checkPayPlansExcludePastActivity.Name = "checkPayPlansExcludePastActivity";
			this.checkPayPlansExcludePastActivity.Size = new System.Drawing.Size(318, 17);
			this.checkPayPlansExcludePastActivity.TabIndex = 310;
			this.checkPayPlansExcludePastActivity.Text = "Payment Plans exclude past activity by default";
			this.checkPayPlansExcludePastActivity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTermsAndConditions
			// 
			this.labelTermsAndConditions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTermsAndConditions.Location = new System.Drawing.Point(146, 185);
			this.labelTermsAndConditions.Name = "labelTermsAndConditions";
			this.labelTermsAndConditions.Size = new System.Drawing.Size(223, 17);
			this.labelTermsAndConditions.TabIndex = 321;
			this.labelTermsAndConditions.Text = "Payment Plan Terms and Conditions";
			this.labelTermsAndConditions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPayPlanTermsAndConditions
			// 
			this.butPayPlanTermsAndConditions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPayPlanTermsAndConditions.Location = new System.Drawing.Point(372, 182);
			this.butPayPlanTermsAndConditions.Name = "butPayPlanTermsAndConditions";
			this.butPayPlanTermsAndConditions.Size = new System.Drawing.Size(68, 22);
			this.butPayPlanTermsAndConditions.TabIndex = 317;
			this.butPayPlanTermsAndConditions.Text = "Edit";
			this.butPayPlanTermsAndConditions.Click += new System.EventHandler(this.butPayPlanTermsAndConditions_Click);
			// 
			// checkPayPlansUseSheets
			// 
			this.checkPayPlansUseSheets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPayPlansUseSheets.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPayPlansUseSheets.Location = new System.Drawing.Point(122, 33);
			this.checkPayPlansUseSheets.Name = "checkPayPlansUseSheets";
			this.checkPayPlansUseSheets.Size = new System.Drawing.Size(318, 17);
			this.checkPayPlansUseSheets.TabIndex = 311;
			this.checkPayPlansUseSheets.Text = "Pay Plans use Sheets for printing";
			this.checkPayPlansUseSheets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPayPlanSaveSignedPdf
			// 
			this.checkPayPlanSaveSignedPdf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPayPlanSaveSignedPdf.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPayPlanSaveSignedPdf.Location = new System.Drawing.Point(122, 56);
			this.checkPayPlanSaveSignedPdf.Name = "checkPayPlanSaveSignedPdf";
			this.checkPayPlanSaveSignedPdf.Size = new System.Drawing.Size(318, 17);
			this.checkPayPlanSaveSignedPdf.TabIndex = 312;
			this.checkPayPlanSaveSignedPdf.Text = "In eClipboard, save signed Payment Plans as PDF";
			this.checkPayPlanSaveSignedPdf.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPayPlanSaveSignedPdf.UseVisualStyleBackColor = true;
			// 
			// label39
			// 
			this.label39.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label39.Location = new System.Drawing.Point(28, 158);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(215, 17);
			this.label39.TabIndex = 320;
			this.label39.Text = "Dynamic payment plan unearned type";
			this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPayPlansVersion
			// 
			this.comboPayPlansVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPayPlansVersion.Location = new System.Drawing.Point(246, 79);
			this.comboPayPlansVersion.Name = "comboPayPlansVersion";
			this.comboPayPlansVersion.Size = new System.Drawing.Size(194, 21);
			this.comboPayPlansVersion.TabIndex = 313;
			this.comboPayPlansVersion.SelectionChangeCommitted += new System.EventHandler(this.comboPayPlansVersion_SelectionChangeCommitted);
			// 
			// comboDppUnearnedType
			// 
			this.comboDppUnearnedType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboDppUnearnedType.Location = new System.Drawing.Point(246, 155);
			this.comboDppUnearnedType.Name = "comboDppUnearnedType";
			this.comboDppUnearnedType.Size = new System.Drawing.Size(194, 21);
			this.comboDppUnearnedType.TabIndex = 316;
			this.comboDppUnearnedType.Text = "comboBoxOD1";
			// 
			// checkHideDueNow
			// 
			this.checkHideDueNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkHideDueNow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideDueNow.Location = new System.Drawing.Point(122, 106);
			this.checkHideDueNow.Name = "checkHideDueNow";
			this.checkHideDueNow.Size = new System.Drawing.Size(318, 17);
			this.checkHideDueNow.TabIndex = 314;
			this.checkHideDueNow.Text = "Hide \"Due Now\" in Payment Plans Grid";
			this.checkHideDueNow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label59
			// 
			this.label59.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label59.Location = new System.Drawing.Point(199, 132);
			this.label59.Name = "label59";
			this.label59.Size = new System.Drawing.Size(170, 17);
			this.label59.TabIndex = 319;
			this.label59.Text = "Dynamic Pay Plan run time";
			this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label27
			// 
			this.label27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label27.Location = new System.Drawing.Point(93, 82);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(150, 17);
			this.label27.TabIndex = 318;
			this.label27.Text = "Pay Plan charge logic";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDynamicPayPlan
			// 
			this.textDynamicPayPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDynamicPayPlan.Location = new System.Drawing.Point(372, 129);
			this.textDynamicPayPlan.Name = "textDynamicPayPlan";
			this.textDynamicPayPlan.Size = new System.Drawing.Size(68, 20);
			this.textDynamicPayPlan.TabIndex = 315;
			// 
			// groupBoxPayments
			// 
			this.groupBoxPayments.Controls.Add(this.checkOnlinePaymentsMarkAsProcessed);
			this.groupBoxPayments.Controls.Add(this.checkStoreCCTokens);
			this.groupBoxPayments.Controls.Add(this.checkPaymentCompletedDisableMerchantButtons);
			this.groupBoxPayments.Controls.Add(this.comboPaymentClinicSetting);
			this.groupBoxPayments.Controls.Add(this.checkIncTxfrTreatNegProdAsIncome);
			this.groupBoxPayments.Controls.Add(this.label38);
			this.groupBoxPayments.Controls.Add(this.checkAllowEmailCCReceipt);
			this.groupBoxPayments.Controls.Add(this.checkPaymentsPromptForPayType);
			this.groupBoxPayments.Location = new System.Drawing.Point(20, 20);
			this.groupBoxPayments.Name = "groupBoxPayments";
			this.groupBoxPayments.Size = new System.Drawing.Size(450, 179);
			this.groupBoxPayments.TabIndex = 307;
			this.groupBoxPayments.Text = "Payments";
			// 
			// checkOnlinePaymentsMarkAsProcessed
			// 
			this.checkOnlinePaymentsMarkAsProcessed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkOnlinePaymentsMarkAsProcessed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOnlinePaymentsMarkAsProcessed.Location = new System.Drawing.Point(80, 152);
			this.checkOnlinePaymentsMarkAsProcessed.Name = "checkOnlinePaymentsMarkAsProcessed";
			this.checkOnlinePaymentsMarkAsProcessed.Size = new System.Drawing.Size(360, 17);
			this.checkOnlinePaymentsMarkAsProcessed.TabIndex = 308;
			this.checkOnlinePaymentsMarkAsProcessed.Text = "Mark online payments as processed";
			this.checkOnlinePaymentsMarkAsProcessed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkStoreCCTokens
			// 
			this.checkStoreCCTokens.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkStoreCCTokens.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStoreCCTokens.Location = new System.Drawing.Point(210, 10);
			this.checkStoreCCTokens.Name = "checkStoreCCTokens";
			this.checkStoreCCTokens.Size = new System.Drawing.Size(230, 17);
			this.checkStoreCCTokens.TabIndex = 280;
			this.checkStoreCCTokens.Text = "Automatically store credit card tokens";
			this.checkStoreCCTokens.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStoreCCTokens.UseVisualStyleBackColor = true;
			// 
			// checkPaymentCompletedDisableMerchantButtons
			// 
			this.checkPaymentCompletedDisableMerchantButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPaymentCompletedDisableMerchantButtons.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPaymentCompletedDisableMerchantButtons.Location = new System.Drawing.Point(96, 129);
			this.checkPaymentCompletedDisableMerchantButtons.Name = "checkPaymentCompletedDisableMerchantButtons";
			this.checkPaymentCompletedDisableMerchantButtons.Size = new System.Drawing.Size(344, 17);
			this.checkPaymentCompletedDisableMerchantButtons.TabIndex = 306;
			this.checkPaymentCompletedDisableMerchantButtons.Text = "Disable merchant buttons for completed payments";
			this.checkPaymentCompletedDisableMerchantButtons.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPaymentCompletedDisableMerchantButtons.UseVisualStyleBackColor = true;
			// 
			// comboPaymentClinicSetting
			// 
			this.comboPaymentClinicSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPaymentClinicSetting.Location = new System.Drawing.Point(277, 33);
			this.comboPaymentClinicSetting.Name = "comboPaymentClinicSetting";
			this.comboPaymentClinicSetting.Size = new System.Drawing.Size(163, 21);
			this.comboPaymentClinicSetting.TabIndex = 290;
			// 
			// checkIncTxfrTreatNegProdAsIncome
			// 
			this.checkIncTxfrTreatNegProdAsIncome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncTxfrTreatNegProdAsIncome.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncTxfrTreatNegProdAsIncome.Location = new System.Drawing.Point(13, 106);
			this.checkIncTxfrTreatNegProdAsIncome.Name = "checkIncTxfrTreatNegProdAsIncome";
			this.checkIncTxfrTreatNegProdAsIncome.Size = new System.Drawing.Size(427, 17);
			this.checkIncTxfrTreatNegProdAsIncome.TabIndex = 305;
			this.checkIncTxfrTreatNegProdAsIncome.Text = "Income transfers treat negative production as income";
			this.checkIncTxfrTreatNegProdAsIncome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncTxfrTreatNegProdAsIncome.UseVisualStyleBackColor = true;
			// 
			// label38
			// 
			this.label38.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label38.Location = new System.Drawing.Point(73, 36);
			this.label38.Margin = new System.Windows.Forms.Padding(0);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(201, 17);
			this.label38.TabIndex = 291;
			this.label38.Text = "Default Clinic for patient payments";
			this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowEmailCCReceipt
			// 
			this.checkAllowEmailCCReceipt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowEmailCCReceipt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowEmailCCReceipt.Location = new System.Drawing.Point(230, 83);
			this.checkAllowEmailCCReceipt.Name = "checkAllowEmailCCReceipt";
			this.checkAllowEmailCCReceipt.Size = new System.Drawing.Size(210, 17);
			this.checkAllowEmailCCReceipt.TabIndex = 292;
			this.checkAllowEmailCCReceipt.Text = "Allow emailing credit card receipts";
			this.checkAllowEmailCCReceipt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowEmailCCReceipt.UseVisualStyleBackColor = true;
			// 
			// checkPaymentsPromptForPayType
			// 
			this.checkPaymentsPromptForPayType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPaymentsPromptForPayType.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPaymentsPromptForPayType.Location = new System.Drawing.Point(170, 60);
			this.checkPaymentsPromptForPayType.Name = "checkPaymentsPromptForPayType";
			this.checkPaymentsPromptForPayType.Size = new System.Drawing.Size(270, 17);
			this.checkPaymentsPromptForPayType.TabIndex = 284;
			this.checkPaymentsPromptForPayType.Text = "Payments prompt for Payment Type";
			this.checkPaymentsPromptForPayType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butIncTxfrTreatNegProdAsIncomeDetails
			// 
			this.butIncTxfrTreatNegProdAsIncomeDetails.ForeColor = System.Drawing.Color.Black;
			this.butIncTxfrTreatNegProdAsIncomeDetails.Location = new System.Drawing.Point(479, 123);
			this.butIncTxfrTreatNegProdAsIncomeDetails.Name = "butIncTxfrTreatNegProdAsIncomeDetails";
			this.butIncTxfrTreatNegProdAsIncomeDetails.Size = new System.Drawing.Size(64, 21);
			this.butIncTxfrTreatNegProdAsIncomeDetails.TabIndex = 371;
			this.butIncTxfrTreatNegProdAsIncomeDetails.Text = "Details";
			this.butIncTxfrTreatNegProdAsIncomeDetails.Click += new System.EventHandler(this.butIncTxfrTreatNegProdAsIncomeDetails_Click);
			// 
			// butPayPlansVersionDetails
			// 
			this.butPayPlansVersionDetails.ForeColor = System.Drawing.Color.Black;
			this.butPayPlansVersionDetails.Location = new System.Drawing.Point(479, 285);
			this.butPayPlansVersionDetails.Name = "butPayPlansVersionDetails";
			this.butPayPlansVersionDetails.Size = new System.Drawing.Size(64, 21);
			this.butPayPlansVersionDetails.TabIndex = 372;
			this.butPayPlansVersionDetails.Text = "Details";
			this.butPayPlansVersionDetails.Click += new System.EventHandler(this.butPayPlansVersionDetails_Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.MidnightBlue;
			this.label1.Location = new System.Drawing.Point(476, 171);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(498, 17);
			this.label1.TabIndex = 373;
			this.label1.Text = "otherwise review manually";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UserControlAccountPayments
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butPayPlansVersionDetails);
			this.Controls.Add(this.butIncTxfrTreatNegProdAsIncomeDetails);
			this.Controls.Add(this.labelStoreCCTokensDetails);
			this.Controls.Add(this.labelDppUnearnedTypeDetails);
			this.Controls.Add(this.labelPayPlansUseSheetsDetails);
			this.Controls.Add(this.groupBoxUnearnedIncome);
			this.Controls.Add(this.groupBoxPayPlans);
			this.Controls.Add(this.groupBoxPayments);
			this.Name = "UserControlAccountPayments";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupBoxUnearnedIncome.ResumeLayout(false);
			this.groupBoxPayPlans.ResumeLayout(false);
			this.groupBoxPayPlans.PerformLayout();
			this.groupBoxPayments.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.CheckBox checkPaymentCompletedDisableMerchantButtons;
		private System.Windows.Forms.CheckBox checkIncTxfrTreatNegProdAsIncome;
		private System.Windows.Forms.CheckBox checkStoreCCTokens;
		private UI.ComboBoxOD comboPaymentClinicSetting;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.CheckBox checkPaymentsPromptForPayType;
		private System.Windows.Forms.CheckBox checkAllowPrepayProvider;
		private UI.ComboBoxOD comboUnallocatedSplits;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.CheckBox checkAllowEmailCCReceipt;
		private UI.GroupBoxOD groupBoxPayments;
		private UI.Button butPayPlanTermsAndConditions;
		private System.Windows.Forms.Label labelTermsAndConditions;
		private System.Windows.Forms.CheckBox checkPayPlansExcludePastActivity;
		private System.Windows.Forms.CheckBox checkPayPlanSaveSignedPdf;
		private System.Windows.Forms.CheckBox checkPayPlansUseSheets;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.CheckBox checkHideDueNow;
		private UI.ComboBoxOD comboDppUnearnedType;
		private UI.ComboBoxOD comboPayPlansVersion;
		private System.Windows.Forms.Label label59;
		private System.Windows.Forms.Label label27;
		private ValidTime textDynamicPayPlan;
		private UI.GroupBoxOD groupBoxPayPlans;
		private System.Windows.Forms.CheckBox checkShowAllocateUnearnedPaymentPrompt;
		private UI.GroupBoxOD groupBoxUnearnedIncome;
		private System.Windows.Forms.Label labelPayPlansUseSheetsDetails;
		private System.Windows.Forms.Label labelDppUnearnedTypeDetails;
		private System.Windows.Forms.Label labelStoreCCTokensDetails;
		private UI.Button butIncTxfrTreatNegProdAsIncomeDetails;
		private UI.Button butPayPlansVersionDetails;
		private System.Windows.Forms.CheckBox checkOnlinePaymentsMarkAsProcessed;
		private System.Windows.Forms.Label label1;
	}
}
