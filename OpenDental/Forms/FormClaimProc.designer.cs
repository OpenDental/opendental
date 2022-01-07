namespace OpenDental {
	partial class FormClaimProc {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimProc));
			this.labelInsPayAmt = new System.Windows.Forms.Label();
			this.labelRemarks = new System.Windows.Forms.Label();
			this.textRemarks = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.labelWriteOff = new System.Windows.Forms.Label();
			this.labelInsPayEst = new System.Windows.Forms.Label();
			this.labelNotInClaim = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textInsPlan = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textPercentage = new System.Windows.Forms.TextBox();
			this.labelCopayAmt = new System.Windows.Forms.Label();
			this.checkNoBillIns = new System.Windows.Forms.CheckBox();
			this.labelFee = new System.Windows.Forms.Label();
			this.textFee = new System.Windows.Forms.TextBox();
			this.label28 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.labelCodeSent = new System.Windows.Forms.Label();
			this.textCodeSent = new System.Windows.Forms.TextBox();
			this.labelFeeBilled = new System.Windows.Forms.Label();
			this.labelDedApplied = new System.Windows.Forms.Label();
			this.labelPaidOtherIns = new System.Windows.Forms.Label();
			this.groupClaim = new System.Windows.Forms.GroupBox();
			this.labelAttachedToCheck = new System.Windows.Forms.Label();
			this.radioClaim = new System.Windows.Forms.RadioButton();
			this.radioEstimate = new System.Windows.Forms.RadioButton();
			this.panelClaimExtras = new System.Windows.Forms.Panel();
			this.labelClaimAdjReasonCodes = new System.Windows.Forms.Label();
			this.textClaimAdjReasonCodes = new System.Windows.Forms.TextBox();
			this.textFeeBilled = new OpenDental.ValidDouble();
			this.panelEstimateInfo = new System.Windows.Forms.Panel();
			this.butBlueBookLog = new OpenDental.UI.Button();
			this.textWriteOffEstOverride = new OpenDental.ValidDouble();
			this.textWriteOffEst = new OpenDental.ValidDouble();
			this.labelWriteOffEst = new System.Windows.Forms.Label();
			this.textEstimateNote = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBaseEst = new OpenDental.ValidDouble();
			this.label3 = new System.Windows.Forms.Label();
			this.textPatPortion1 = new OpenDental.ValidDouble();
			this.labelPatPortion1 = new System.Windows.Forms.Label();
			this.textPaidOtherInsOverride = new OpenDental.ValidDouble();
			this.textInsEstTotalOverride = new OpenDental.ValidDouble();
			this.textInsEstTotal = new OpenDental.ValidDouble();
			this.label17 = new System.Windows.Forms.Label();
			this.groupAllowed = new System.Windows.Forms.GroupBox();
			this.textAllowedOverride = new OpenDental.ValidDouble();
			this.label10 = new System.Windows.Forms.Label();
			this.textAllowedFeeSched = new System.Windows.Forms.TextBox();
			this.textSubstCode = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.butUpdateAllowed = new OpenDental.UI.Button();
			this.labelCarrierAllowed = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textCarrierAllowed = new System.Windows.Forms.TextBox();
			this.textPPOFeeSched = new System.Windows.Forms.TextBox();
			this.textDedEst = new OpenDental.ValidDouble();
			this.textPaidOtherIns = new OpenDental.ValidDouble();
			this.textFeeSched = new System.Windows.Forms.TextBox();
			this.labelFeeSched = new System.Windows.Forms.Label();
			this.textCopayOverride = new OpenDental.ValidDouble();
			this.label11 = new System.Windows.Forms.Label();
			this.textCopayAmt = new OpenDental.ValidDouble();
			this.textDedEstOverride = new OpenDental.ValidDouble();
			this.textPercentOverride = new OpenDental.ValidNum();
			this.groupClaimInfo = new System.Windows.Forms.GroupBox();
			this.textPatPortion2 = new OpenDental.ValidDouble();
			this.labelPatPortion2 = new System.Windows.Forms.Label();
			this.textWriteOff = new OpenDental.ValidDouble();
			this.textInsPayEst = new OpenDental.ValidDouble();
			this.textInsPayAmt = new OpenDental.ValidDouble();
			this.textDedApplied = new OpenDental.ValidDouble();
			this.labelProcDate = new System.Windows.Forms.Label();
			this.labelDateEntry = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.textClinic = new System.Windows.Forms.TextBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.comboProvider = new OpenDental.UI.ComboBoxOD();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.checkPayPlan = new System.Windows.Forms.CheckBox();
			this.comboPayTracker = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.butPickProv = new OpenDental.UI.Button();
			this.textDateEntry = new OpenDental.ValidDate();
			this.textProcDate = new OpenDental.ValidDate();
			this.textDateCP = new OpenDental.ValidDate();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupClaim.SuspendLayout();
			this.panelClaimExtras.SuspendLayout();
			this.panelEstimateInfo.SuspendLayout();
			this.groupAllowed.SuspendLayout();
			this.groupClaimInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelInsPayAmt
			// 
			this.labelInsPayAmt.Location = new System.Drawing.Point(31, 55);
			this.labelInsPayAmt.Name = "labelInsPayAmt";
			this.labelInsPayAmt.Size = new System.Drawing.Size(129, 17);
			this.labelInsPayAmt.TabIndex = 0;
			this.labelInsPayAmt.Text = "Insurance Paid";
			this.labelInsPayAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelRemarks
			// 
			this.labelRemarks.Location = new System.Drawing.Point(24, 73);
			this.labelRemarks.Name = "labelRemarks";
			this.labelRemarks.Size = new System.Drawing.Size(113, 37);
			this.labelRemarks.TabIndex = 0;
			this.labelRemarks.Text = "Remarks from EOB";
			this.labelRemarks.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textRemarks
			// 
			this.textRemarks.Location = new System.Drawing.Point(139, 74);
			this.textRemarks.MaxLength = 255;
			this.textRemarks.Multiline = true;
			this.textRemarks.Name = "textRemarks";
			this.textRemarks.Size = new System.Drawing.Size(290, 129);
			this.textRemarks.TabIndex = 3;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(75, 29);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(80, 17);
			this.label9.TabIndex = 0;
			this.label9.Text = "Status";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelWriteOff
			// 
			this.labelWriteOff.Location = new System.Drawing.Point(31, 75);
			this.labelWriteOff.Name = "labelWriteOff";
			this.labelWriteOff.Size = new System.Drawing.Size(129, 17);
			this.labelWriteOff.TabIndex = 0;
			this.labelWriteOff.Text = "Write Off";
			this.labelWriteOff.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInsPayEst
			// 
			this.labelInsPayEst.Location = new System.Drawing.Point(31, 36);
			this.labelInsPayEst.Name = "labelInsPayEst";
			this.labelInsPayEst.Size = new System.Drawing.Size(129, 17);
			this.labelInsPayEst.TabIndex = 0;
			this.labelInsPayEst.Text = "Insurance Estimate";
			this.labelInsPayEst.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelNotInClaim
			// 
			this.labelNotInClaim.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNotInClaim.Location = new System.Drawing.Point(118, 267);
			this.labelNotInClaim.Name = "labelNotInClaim";
			this.labelNotInClaim.Size = new System.Drawing.Size(331, 17);
			this.labelNotInClaim.TabIndex = 0;
			this.labelNotInClaim.Text = "Changes can only be made from within the claim.";
			this.labelNotInClaim.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(33, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(121, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Ins Plan";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsPlan
			// 
			this.textInsPlan.Location = new System.Drawing.Point(157, 4);
			this.textInsPlan.Name = "textInsPlan";
			this.textInsPlan.ReadOnly = true;
			this.textInsPlan.Size = new System.Drawing.Size(341, 20);
			this.textInsPlan.TabIndex = 0;
			this.textInsPlan.TabStop = false;
			this.textInsPlan.Text = "An insurance plan";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(28, 219);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(138, 17);
			this.label4.TabIndex = 0;
			this.label4.Text = "Percentage %";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPercentage
			// 
			this.textPercentage.Location = new System.Drawing.Point(168, 218);
			this.textPercentage.Name = "textPercentage";
			this.textPercentage.ReadOnly = true;
			this.textPercentage.Size = new System.Drawing.Size(70, 20);
			this.textPercentage.TabIndex = 0;
			this.textPercentage.TabStop = false;
			this.textPercentage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelCopayAmt
			// 
			this.labelCopayAmt.Location = new System.Drawing.Point(28, 179);
			this.labelCopayAmt.Name = "labelCopayAmt";
			this.labelCopayAmt.Size = new System.Drawing.Size(138, 17);
			this.labelCopayAmt.TabIndex = 0;
			this.labelCopayAmt.Text = "Patient Copay";
			this.labelCopayAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkNoBillIns
			// 
			this.checkNoBillIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNoBillIns.Location = new System.Drawing.Point(616, 3);
			this.checkNoBillIns.Name = "checkNoBillIns";
			this.checkNoBillIns.Size = new System.Drawing.Size(270, 22);
			this.checkNoBillIns.TabIndex = 8;
			this.checkNoBillIns.Text = "Do Not Bill to This Insurance";
			this.checkNoBillIns.Click += new System.EventHandler(this.checkNoBillIns_Click);
			// 
			// labelFee
			// 
			this.labelFee.Location = new System.Drawing.Point(59, 7);
			this.labelFee.Name = "labelFee";
			this.labelFee.Size = new System.Drawing.Size(107, 14);
			this.labelFee.TabIndex = 0;
			this.labelFee.Text = "Fee";
			this.labelFee.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textFee
			// 
			this.textFee.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textFee.Location = new System.Drawing.Point(169, 8);
			this.textFee.Name = "textFee";
			this.textFee.ReadOnly = true;
			this.textFee.Size = new System.Drawing.Size(58, 13);
			this.textFee.TabIndex = 0;
			this.textFee.TabStop = false;
			this.textFee.Text = "520.00";
			this.textFee.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label28
			// 
			this.label28.Location = new System.Drawing.Point(30, 144);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(125, 17);
			this.label28.TabIndex = 0;
			this.label28.Text = "Payment Date";
			this.label28.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(33, 188);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(121, 17);
			this.label29.TabIndex = 67;
			this.label29.Text = "Description";
			this.label29.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(157, 184);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(203, 20);
			this.textDescription.TabIndex = 0;
			this.textDescription.TabStop = false;
			// 
			// label30
			// 
			this.label30.Location = new System.Drawing.Point(82, 77);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(73, 17);
			this.label30.TabIndex = 0;
			this.label30.Text = "Provider";
			this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCodeSent
			// 
			this.labelCodeSent.Location = new System.Drawing.Point(18, 11);
			this.labelCodeSent.Name = "labelCodeSent";
			this.labelCodeSent.Size = new System.Drawing.Size(121, 14);
			this.labelCodeSent.TabIndex = 0;
			this.labelCodeSent.Text = "Code Sent to Ins";
			this.labelCodeSent.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCodeSent
			// 
			this.textCodeSent.Location = new System.Drawing.Point(139, 8);
			this.textCodeSent.Name = "textCodeSent";
			this.textCodeSent.Size = new System.Drawing.Size(77, 20);
			this.textCodeSent.TabIndex = 1;
			// 
			// labelFeeBilled
			// 
			this.labelFeeBilled.Location = new System.Drawing.Point(17, 32);
			this.labelFeeBilled.Name = "labelFeeBilled";
			this.labelFeeBilled.Size = new System.Drawing.Size(121, 17);
			this.labelFeeBilled.TabIndex = 0;
			this.labelFeeBilled.Text = "Fee Billed to Ins";
			this.labelFeeBilled.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelDedApplied
			// 
			this.labelDedApplied.Location = new System.Drawing.Point(31, 16);
			this.labelDedApplied.Name = "labelDedApplied";
			this.labelDedApplied.Size = new System.Drawing.Size(129, 17);
			this.labelDedApplied.TabIndex = 0;
			this.labelDedApplied.Text = "Deductible";
			this.labelDedApplied.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPaidOtherIns
			// 
			this.labelPaidOtherIns.Location = new System.Drawing.Point(28, 240);
			this.labelPaidOtherIns.Name = "labelPaidOtherIns";
			this.labelPaidOtherIns.Size = new System.Drawing.Size(138, 17);
			this.labelPaidOtherIns.TabIndex = 0;
			this.labelPaidOtherIns.Text = "Paid By Other Ins";
			this.labelPaidOtherIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupClaim
			// 
			this.groupClaim.Controls.Add(this.labelAttachedToCheck);
			this.groupClaim.Controls.Add(this.labelNotInClaim);
			this.groupClaim.Controls.Add(this.radioClaim);
			this.groupClaim.Controls.Add(this.radioEstimate);
			this.groupClaim.Controls.Add(this.panelClaimExtras);
			this.groupClaim.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupClaim.Location = new System.Drawing.Point(14, 212);
			this.groupClaim.Name = "groupClaim";
			this.groupClaim.Size = new System.Drawing.Size(460, 326);
			this.groupClaim.TabIndex = 7;
			this.groupClaim.TabStop = false;
			this.groupClaim.Text = "Claim";
			// 
			// labelAttachedToCheck
			// 
			this.labelAttachedToCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAttachedToCheck.Location = new System.Drawing.Point(118, 291);
			this.labelAttachedToCheck.Name = "labelAttachedToCheck";
			this.labelAttachedToCheck.Size = new System.Drawing.Size(333, 29);
			this.labelAttachedToCheck.TabIndex = 0;
			this.labelAttachedToCheck.Text = "This is attached to an insurance check, so certain changes are not allowed.";
			// 
			// radioClaim
			// 
			this.radioClaim.AutoCheck = false;
			this.radioClaim.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioClaim.Location = new System.Drawing.Point(100, 33);
			this.radioClaim.Name = "radioClaim";
			this.radioClaim.Size = new System.Drawing.Size(353, 18);
			this.radioClaim.TabIndex = 2;
			this.radioClaim.TabStop = true;
			this.radioClaim.Text = "This is part of a claim.";
			this.radioClaim.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			// 
			// radioEstimate
			// 
			this.radioEstimate.AutoCheck = false;
			this.radioEstimate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioEstimate.Location = new System.Drawing.Point(100, 10);
			this.radioEstimate.Name = "radioEstimate";
			this.radioEstimate.Size = new System.Drawing.Size(352, 22);
			this.radioEstimate.TabIndex = 1;
			this.radioEstimate.TabStop = true;
			this.radioEstimate.Text = "This is an estimate only. It has not been attached to a claim.";
			// 
			// panelClaimExtras
			// 
			this.panelClaimExtras.Controls.Add(this.labelClaimAdjReasonCodes);
			this.panelClaimExtras.Controls.Add(this.textClaimAdjReasonCodes);
			this.panelClaimExtras.Controls.Add(this.labelRemarks);
			this.panelClaimExtras.Controls.Add(this.textRemarks);
			this.panelClaimExtras.Controls.Add(this.labelCodeSent);
			this.panelClaimExtras.Controls.Add(this.textCodeSent);
			this.panelClaimExtras.Controls.Add(this.labelFeeBilled);
			this.panelClaimExtras.Controls.Add(this.textFeeBilled);
			this.panelClaimExtras.Location = new System.Drawing.Point(4, 54);
			this.panelClaimExtras.Name = "panelClaimExtras";
			this.panelClaimExtras.Size = new System.Drawing.Size(452, 212);
			this.panelClaimExtras.TabIndex = 3;
			// 
			// labelClaimAdjReasonCodes
			// 
			this.labelClaimAdjReasonCodes.Location = new System.Drawing.Point(2, 53);
			this.labelClaimAdjReasonCodes.Name = "labelClaimAdjReasonCodes";
			this.labelClaimAdjReasonCodes.Size = new System.Drawing.Size(136, 17);
			this.labelClaimAdjReasonCodes.TabIndex = 69;
			this.labelClaimAdjReasonCodes.Text = "Claim Adj Reason Code";
			this.labelClaimAdjReasonCodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClaimAdjReasonCodes
			// 
			this.textClaimAdjReasonCodes.Location = new System.Drawing.Point(139, 52);
			this.textClaimAdjReasonCodes.Name = "textClaimAdjReasonCodes";
			this.textClaimAdjReasonCodes.ReadOnly = true;
			this.textClaimAdjReasonCodes.Size = new System.Drawing.Size(290, 20);
			this.textClaimAdjReasonCodes.TabIndex = 68;
			this.textClaimAdjReasonCodes.TabStop = false;
			// 
			// textFeeBilled
			// 
			this.textFeeBilled.Location = new System.Drawing.Point(139, 30);
			this.textFeeBilled.MaxVal = 100000000D;
			this.textFeeBilled.MinVal = -100000000D;
			this.textFeeBilled.Name = "textFeeBilled";
			this.textFeeBilled.Size = new System.Drawing.Size(77, 20);
			this.textFeeBilled.TabIndex = 2;
			// 
			// panelEstimateInfo
			// 
			this.panelEstimateInfo.Controls.Add(this.butBlueBookLog);
			this.panelEstimateInfo.Controls.Add(this.textWriteOffEstOverride);
			this.panelEstimateInfo.Controls.Add(this.textWriteOffEst);
			this.panelEstimateInfo.Controls.Add(this.labelWriteOffEst);
			this.panelEstimateInfo.Controls.Add(this.textEstimateNote);
			this.panelEstimateInfo.Controls.Add(this.label5);
			this.panelEstimateInfo.Controls.Add(this.textBaseEst);
			this.panelEstimateInfo.Controls.Add(this.label3);
			this.panelEstimateInfo.Controls.Add(this.textPatPortion1);
			this.panelEstimateInfo.Controls.Add(this.labelPatPortion1);
			this.panelEstimateInfo.Controls.Add(this.textPaidOtherInsOverride);
			this.panelEstimateInfo.Controls.Add(this.textInsEstTotalOverride);
			this.panelEstimateInfo.Controls.Add(this.textInsEstTotal);
			this.panelEstimateInfo.Controls.Add(this.label17);
			this.panelEstimateInfo.Controls.Add(this.groupAllowed);
			this.panelEstimateInfo.Controls.Add(this.textDedEst);
			this.panelEstimateInfo.Controls.Add(this.textPaidOtherIns);
			this.panelEstimateInfo.Controls.Add(this.textFeeSched);
			this.panelEstimateInfo.Controls.Add(this.labelFeeSched);
			this.panelEstimateInfo.Controls.Add(this.labelPaidOtherIns);
			this.panelEstimateInfo.Controls.Add(this.textCopayOverride);
			this.panelEstimateInfo.Controls.Add(this.label11);
			this.panelEstimateInfo.Controls.Add(this.labelFee);
			this.panelEstimateInfo.Controls.Add(this.label4);
			this.panelEstimateInfo.Controls.Add(this.textPercentage);
			this.panelEstimateInfo.Controls.Add(this.textCopayAmt);
			this.panelEstimateInfo.Controls.Add(this.labelCopayAmt);
			this.panelEstimateInfo.Controls.Add(this.textDedEstOverride);
			this.panelEstimateInfo.Controls.Add(this.textFee);
			this.panelEstimateInfo.Controls.Add(this.textPercentOverride);
			this.panelEstimateInfo.Location = new System.Drawing.Point(512, 25);
			this.panelEstimateInfo.Name = "panelEstimateInfo";
			this.panelEstimateInfo.Size = new System.Drawing.Size(411, 403);
			this.panelEstimateInfo.TabIndex = 9;
			// 
			// butBlueBookLog
			// 
			this.butBlueBookLog.Location = new System.Drawing.Point(316, 278);
			this.butBlueBookLog.Name = "butBlueBookLog";
			this.butBlueBookLog.Size = new System.Drawing.Size(85, 22);
			this.butBlueBookLog.TabIndex = 3;
			this.butBlueBookLog.Text = "Blue Book Log";
			this.toolTip1.SetToolTip(this.butBlueBookLog, "Edit the fee schedule that holds the fee showing in the Carrier Allowed Amt box.");
			this.butBlueBookLog.Click += new System.EventHandler(this.ButBlueBookLog_Click);
			// 
			// textWriteOffEstOverride
			// 
			this.textWriteOffEstOverride.Location = new System.Drawing.Point(240, 298);
			this.textWriteOffEstOverride.MaxVal = 100000000D;
			this.textWriteOffEstOverride.MinVal = 0D;
			this.textWriteOffEstOverride.Name = "textWriteOffEstOverride";
			this.textWriteOffEstOverride.Size = new System.Drawing.Size(70, 20);
			this.textWriteOffEstOverride.TabIndex = 7;
			this.textWriteOffEstOverride.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textWriteOffEstOverride.Enter += new System.EventHandler(this.textWriteOffEstOverride_Enter);
			this.textWriteOffEstOverride.Leave += new System.EventHandler(this.textWriteOffEstOverride_Leave);
			// 
			// textWriteOffEst
			// 
			this.textWriteOffEst.Location = new System.Drawing.Point(168, 298);
			this.textWriteOffEst.MaxVal = 100000000D;
			this.textWriteOffEst.MinVal = -100000000D;
			this.textWriteOffEst.Name = "textWriteOffEst";
			this.textWriteOffEst.ReadOnly = true;
			this.textWriteOffEst.Size = new System.Drawing.Size(70, 20);
			this.textWriteOffEst.TabIndex = 0;
			this.textWriteOffEst.TabStop = false;
			this.textWriteOffEst.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelWriteOffEst
			// 
			this.labelWriteOffEst.Location = new System.Drawing.Point(28, 301);
			this.labelWriteOffEst.Name = "labelWriteOffEst";
			this.labelWriteOffEst.Size = new System.Drawing.Size(138, 17);
			this.labelWriteOffEst.TabIndex = 0;
			this.labelWriteOffEst.Text = "Write Off Estimate";
			this.labelWriteOffEst.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textEstimateNote
			// 
			this.textEstimateNote.Location = new System.Drawing.Point(168, 338);
			this.textEstimateNote.MaxLength = 255;
			this.textEstimateNote.Multiline = true;
			this.textEstimateNote.Name = "textEstimateNote";
			this.textEstimateNote.ReadOnly = true;
			this.textEstimateNote.Size = new System.Drawing.Size(239, 58);
			this.textEstimateNote.TabIndex = 0;
			this.textEstimateNote.TabStop = false;
			this.textEstimateNote.Text = "Over annual max\r\nExclusions\r\nLimitations";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(27, 341);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(138, 17);
			this.label5.TabIndex = 0;
			this.label5.Text = "Estimate Note";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBaseEst
			// 
			this.textBaseEst.Location = new System.Drawing.Point(168, 258);
			this.textBaseEst.MaxVal = 100000000D;
			this.textBaseEst.MinVal = -100000000D;
			this.textBaseEst.Name = "textBaseEst";
			this.textBaseEst.ReadOnly = true;
			this.textBaseEst.Size = new System.Drawing.Size(70, 20);
			this.textBaseEst.TabIndex = 0;
			this.textBaseEst.TabStop = false;
			this.textBaseEst.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 260);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(161, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "BaseEst (no max or deduct)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPatPortion1
			// 
			this.textPatPortion1.Location = new System.Drawing.Point(168, 318);
			this.textPatPortion1.MaxVal = 100000000D;
			this.textPatPortion1.MinVal = -100000000D;
			this.textPatPortion1.Name = "textPatPortion1";
			this.textPatPortion1.ReadOnly = true;
			this.textPatPortion1.Size = new System.Drawing.Size(70, 20);
			this.textPatPortion1.TabIndex = 0;
			this.textPatPortion1.TabStop = false;
			this.textPatPortion1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPatPortion1
			// 
			this.labelPatPortion1.Location = new System.Drawing.Point(28, 320);
			this.labelPatPortion1.Name = "labelPatPortion1";
			this.labelPatPortion1.Size = new System.Drawing.Size(138, 17);
			this.labelPatPortion1.TabIndex = 0;
			this.labelPatPortion1.Text = "Estimated Patient Portion";
			this.labelPatPortion1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPaidOtherInsOverride
			// 
			this.textPaidOtherInsOverride.Location = new System.Drawing.Point(240, 238);
			this.textPaidOtherInsOverride.MaxVal = 255D;
			this.textPaidOtherInsOverride.MinVal = 0D;
			this.textPaidOtherInsOverride.Name = "textPaidOtherInsOverride";
			this.textPaidOtherInsOverride.Size = new System.Drawing.Size(70, 20);
			this.textPaidOtherInsOverride.TabIndex = 5;
			this.textPaidOtherInsOverride.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textPaidOtherInsOverride.Leave += new System.EventHandler(this.textPaidOtherInsOverride_Leave);
			// 
			// textInsEstTotalOverride
			// 
			this.textInsEstTotalOverride.Location = new System.Drawing.Point(240, 278);
			this.textInsEstTotalOverride.MaxVal = 100000000D;
			this.textInsEstTotalOverride.MinVal = -100000000D;
			this.textInsEstTotalOverride.Name = "textInsEstTotalOverride";
			this.textInsEstTotalOverride.Size = new System.Drawing.Size(70, 20);
			this.textInsEstTotalOverride.TabIndex = 6;
			this.textInsEstTotalOverride.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textInsEstTotalOverride.Leave += new System.EventHandler(this.textInsEstTotalOverride_Leave);
			// 
			// textInsEstTotal
			// 
			this.textInsEstTotal.Location = new System.Drawing.Point(168, 278);
			this.textInsEstTotal.MaxVal = 100000000D;
			this.textInsEstTotal.MinVal = -100000000D;
			this.textInsEstTotal.Name = "textInsEstTotal";
			this.textInsEstTotal.ReadOnly = true;
			this.textInsEstTotal.Size = new System.Drawing.Size(70, 20);
			this.textInsEstTotal.TabIndex = 0;
			this.textInsEstTotal.TabStop = false;
			this.textInsEstTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(28, 281);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(138, 17);
			this.label17.TabIndex = 0;
			this.label17.Text = "Insurance Estimate";
			this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupAllowed
			// 
			this.groupAllowed.Controls.Add(this.textAllowedOverride);
			this.groupAllowed.Controls.Add(this.label10);
			this.groupAllowed.Controls.Add(this.textAllowedFeeSched);
			this.groupAllowed.Controls.Add(this.textSubstCode);
			this.groupAllowed.Controls.Add(this.label7);
			this.groupAllowed.Controls.Add(this.label8);
			this.groupAllowed.Controls.Add(this.butUpdateAllowed);
			this.groupAllowed.Controls.Add(this.labelCarrierAllowed);
			this.groupAllowed.Controls.Add(this.label2);
			this.groupAllowed.Controls.Add(this.textCarrierAllowed);
			this.groupAllowed.Controls.Add(this.textPPOFeeSched);
			this.groupAllowed.Location = new System.Drawing.Point(5, 43);
			this.groupAllowed.Name = "groupAllowed";
			this.groupAllowed.Size = new System.Drawing.Size(388, 132);
			this.groupAllowed.TabIndex = 1;
			this.groupAllowed.TabStop = false;
			this.groupAllowed.Text = "Carrier Allowed Amount";
			// 
			// textAllowedOverride
			// 
			this.textAllowedOverride.Location = new System.Drawing.Point(235, 107);
			this.textAllowedOverride.MaxVal = 100000000D;
			this.textAllowedOverride.MinVal = -100000000D;
			this.textAllowedOverride.Name = "textAllowedOverride";
			this.textAllowedOverride.Size = new System.Drawing.Size(70, 20);
			this.textAllowedOverride.TabIndex = 2;
			this.textAllowedOverride.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textAllowedOverride.Leave += new System.EventHandler(this.textAllowedOverride_Leave);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(123, 85);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(254, 16);
			this.label10.TabIndex = 0;
			this.label10.Text = "Edit the allowed fee schedule for this code.";
			// 
			// textAllowedFeeSched
			// 
			this.textAllowedFeeSched.Location = new System.Drawing.Point(163, 58);
			this.textAllowedFeeSched.Name = "textAllowedFeeSched";
			this.textAllowedFeeSched.ReadOnly = true;
			this.textAllowedFeeSched.Size = new System.Drawing.Size(219, 20);
			this.textAllowedFeeSched.TabIndex = 0;
			this.textAllowedFeeSched.TabStop = false;
			// 
			// textSubstCode
			// 
			this.textSubstCode.Location = new System.Drawing.Point(163, 14);
			this.textSubstCode.Name = "textSubstCode";
			this.textSubstCode.ReadOnly = true;
			this.textSubstCode.Size = new System.Drawing.Size(78, 20);
			this.textSubstCode.TabIndex = 0;
			this.textSubstCode.TabStop = false;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(34, 40);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(127, 14);
			this.label7.TabIndex = 0;
			this.label7.Text = "PPO Fee Schedule";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(34, 62);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(127, 14);
			this.label8.TabIndex = 0;
			this.label8.Text = "Allowed Fee Schedule";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butUpdateAllowed
			// 
			this.butUpdateAllowed.Location = new System.Drawing.Point(13, 81);
			this.butUpdateAllowed.Name = "butUpdateAllowed";
			this.butUpdateAllowed.Size = new System.Drawing.Size(101, 22);
			this.butUpdateAllowed.TabIndex = 1;
			this.butUpdateAllowed.Text = "Edit Allowed Amt";
			this.toolTip1.SetToolTip(this.butUpdateAllowed, "Edit the fee schedule that holds the fee showing in the Carrier Allowed Amt box.");
			this.butUpdateAllowed.Click += new System.EventHandler(this.butUpdateAllowed_Click);
			// 
			// labelCarrierAllowed
			// 
			this.labelCarrierAllowed.Location = new System.Drawing.Point(34, 110);
			this.labelCarrierAllowed.Name = "labelCarrierAllowed";
			this.labelCarrierAllowed.Size = new System.Drawing.Size(127, 14);
			this.labelCarrierAllowed.TabIndex = 0;
			this.labelCarrierAllowed.Text = "Allowed Amt";
			this.labelCarrierAllowed.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(34, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(127, 14);
			this.label2.TabIndex = 0;
			this.label2.Text = "Substitution Code";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCarrierAllowed
			// 
			this.textCarrierAllowed.Location = new System.Drawing.Point(163, 107);
			this.textCarrierAllowed.Name = "textCarrierAllowed";
			this.textCarrierAllowed.ReadOnly = true;
			this.textCarrierAllowed.Size = new System.Drawing.Size(70, 20);
			this.textCarrierAllowed.TabIndex = 0;
			this.textCarrierAllowed.TabStop = false;
			this.textCarrierAllowed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textPPOFeeSched
			// 
			this.textPPOFeeSched.Location = new System.Drawing.Point(163, 36);
			this.textPPOFeeSched.Name = "textPPOFeeSched";
			this.textPPOFeeSched.ReadOnly = true;
			this.textPPOFeeSched.Size = new System.Drawing.Size(219, 20);
			this.textPPOFeeSched.TabIndex = 0;
			this.textPPOFeeSched.TabStop = false;
			// 
			// textDedEst
			// 
			this.textDedEst.Location = new System.Drawing.Point(168, 198);
			this.textDedEst.MaxVal = 100000000D;
			this.textDedEst.MinVal = -100000000D;
			this.textDedEst.Name = "textDedEst";
			this.textDedEst.ReadOnly = true;
			this.textDedEst.Size = new System.Drawing.Size(70, 20);
			this.textDedEst.TabIndex = 0;
			this.textDedEst.TabStop = false;
			this.textDedEst.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textPaidOtherIns
			// 
			this.textPaidOtherIns.Location = new System.Drawing.Point(168, 238);
			this.textPaidOtherIns.MaxVal = 100000000D;
			this.textPaidOtherIns.MinVal = -100000000D;
			this.textPaidOtherIns.Name = "textPaidOtherIns";
			this.textPaidOtherIns.ReadOnly = true;
			this.textPaidOtherIns.Size = new System.Drawing.Size(70, 20);
			this.textPaidOtherIns.TabIndex = 0;
			this.textPaidOtherIns.TabStop = false;
			this.textPaidOtherIns.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textFeeSched
			// 
			this.textFeeSched.Location = new System.Drawing.Point(168, 23);
			this.textFeeSched.Name = "textFeeSched";
			this.textFeeSched.ReadOnly = true;
			this.textFeeSched.Size = new System.Drawing.Size(219, 20);
			this.textFeeSched.TabIndex = 0;
			this.textFeeSched.TabStop = false;
			// 
			// labelFeeSched
			// 
			this.labelFeeSched.Location = new System.Drawing.Point(39, 26);
			this.labelFeeSched.Name = "labelFeeSched";
			this.labelFeeSched.Size = new System.Drawing.Size(127, 14);
			this.labelFeeSched.TabIndex = 0;
			this.labelFeeSched.Text = "Fee Schedule";
			this.labelFeeSched.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCopayOverride
			// 
			this.textCopayOverride.Location = new System.Drawing.Point(240, 178);
			this.textCopayOverride.MaxVal = 100000000D;
			this.textCopayOverride.MinVal = -100000000D;
			this.textCopayOverride.Name = "textCopayOverride";
			this.textCopayOverride.Size = new System.Drawing.Size(70, 20);
			this.textCopayOverride.TabIndex = 2;
			this.textCopayOverride.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textCopayOverride.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textCopayOverride_KeyUp);
			this.textCopayOverride.Leave += new System.EventHandler(this.textCopayOverride_Leave);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(28, 199);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(138, 17);
			this.label11.TabIndex = 0;
			this.label11.Text = "Deductible";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCopayAmt
			// 
			this.textCopayAmt.Location = new System.Drawing.Point(168, 178);
			this.textCopayAmt.MaxVal = 100000000D;
			this.textCopayAmt.MinVal = -100000000D;
			this.textCopayAmt.Name = "textCopayAmt";
			this.textCopayAmt.ReadOnly = true;
			this.textCopayAmt.Size = new System.Drawing.Size(70, 20);
			this.textCopayAmt.TabIndex = 0;
			this.textCopayAmt.TabStop = false;
			this.textCopayAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textDedEstOverride
			// 
			this.textDedEstOverride.Location = new System.Drawing.Point(240, 198);
			this.textDedEstOverride.MaxVal = 100000000D;
			this.textDedEstOverride.MinVal = -100000000D;
			this.textDedEstOverride.Name = "textDedEstOverride";
			this.textDedEstOverride.Size = new System.Drawing.Size(70, 20);
			this.textDedEstOverride.TabIndex = 3;
			this.textDedEstOverride.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textDedEstOverride.Leave += new System.EventHandler(this.textDedEstOverride_Leave);
			// 
			// textPercentOverride
			// 
			this.textPercentOverride.Location = new System.Drawing.Point(240, 218);
			this.textPercentOverride.Name = "textPercentOverride";
			this.textPercentOverride.ShowZero = false;
			this.textPercentOverride.Size = new System.Drawing.Size(70, 20);
			this.textPercentOverride.TabIndex = 4;
			this.textPercentOverride.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textPercentOverride.Leave += new System.EventHandler(this.textPercentOverride_Leave);
			// 
			// groupClaimInfo
			// 
			this.groupClaimInfo.Controls.Add(this.textPatPortion2);
			this.groupClaimInfo.Controls.Add(this.labelPatPortion2);
			this.groupClaimInfo.Controls.Add(this.textWriteOff);
			this.groupClaimInfo.Controls.Add(this.textInsPayEst);
			this.groupClaimInfo.Controls.Add(this.labelInsPayEst);
			this.groupClaimInfo.Controls.Add(this.labelInsPayAmt);
			this.groupClaimInfo.Controls.Add(this.textInsPayAmt);
			this.groupClaimInfo.Controls.Add(this.textDedApplied);
			this.groupClaimInfo.Controls.Add(this.labelDedApplied);
			this.groupClaimInfo.Controls.Add(this.labelWriteOff);
			this.groupClaimInfo.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupClaimInfo.Location = new System.Drawing.Point(517, 473);
			this.groupClaimInfo.Name = "groupClaimInfo";
			this.groupClaimInfo.Size = new System.Drawing.Size(388, 120);
			this.groupClaimInfo.TabIndex = 10;
			this.groupClaimInfo.TabStop = false;
			this.groupClaimInfo.Text = "Claim Info";
			// 
			// textPatPortion2
			// 
			this.textPatPortion2.Location = new System.Drawing.Point(163, 93);
			this.textPatPortion2.MaxVal = 100000000D;
			this.textPatPortion2.MinVal = -100000000D;
			this.textPatPortion2.Name = "textPatPortion2";
			this.textPatPortion2.ReadOnly = true;
			this.textPatPortion2.Size = new System.Drawing.Size(70, 20);
			this.textPatPortion2.TabIndex = 0;
			this.textPatPortion2.TabStop = false;
			this.textPatPortion2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPatPortion2
			// 
			this.labelPatPortion2.Location = new System.Drawing.Point(23, 95);
			this.labelPatPortion2.Name = "labelPatPortion2";
			this.labelPatPortion2.Size = new System.Drawing.Size(138, 17);
			this.labelPatPortion2.TabIndex = 0;
			this.labelPatPortion2.Text = "Estimated Patient Portion";
			this.labelPatPortion2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textWriteOff
			// 
			this.textWriteOff.Location = new System.Drawing.Point(163, 73);
			this.textWriteOff.MaxVal = 100000000D;
			this.textWriteOff.MinVal = -100000000D;
			this.textWriteOff.Name = "textWriteOff";
			this.textWriteOff.Size = new System.Drawing.Size(70, 20);
			this.textWriteOff.TabIndex = 4;
			this.textWriteOff.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textWriteOff.Enter += new System.EventHandler(this.textWriteOff_Enter);
			this.textWriteOff.Leave += new System.EventHandler(this.textWriteOff_Leave);
			// 
			// textInsPayEst
			// 
			this.textInsPayEst.Location = new System.Drawing.Point(163, 33);
			this.textInsPayEst.MaxVal = 100000000D;
			this.textInsPayEst.MinVal = -100000000D;
			this.textInsPayEst.Name = "textInsPayEst";
			this.textInsPayEst.Size = new System.Drawing.Size(70, 20);
			this.textInsPayEst.TabIndex = 2;
			this.textInsPayEst.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textInsPayEst.Leave += new System.EventHandler(this.textInsPayEst_Leave);
			// 
			// textInsPayAmt
			// 
			this.textInsPayAmt.Location = new System.Drawing.Point(163, 53);
			this.textInsPayAmt.MaxVal = 100000000D;
			this.textInsPayAmt.MinVal = -100000000D;
			this.textInsPayAmt.Name = "textInsPayAmt";
			this.textInsPayAmt.Size = new System.Drawing.Size(70, 20);
			this.textInsPayAmt.TabIndex = 3;
			this.textInsPayAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textInsPayAmt.Enter += new System.EventHandler(this.textInsPayAmt_Enter);
			this.textInsPayAmt.Leave += new System.EventHandler(this.textInsPayAmt_Leave);
			// 
			// textDedApplied
			// 
			this.textDedApplied.Location = new System.Drawing.Point(163, 13);
			this.textDedApplied.MaxVal = 100000000D;
			this.textDedApplied.MinVal = -100000000D;
			this.textDedApplied.Name = "textDedApplied";
			this.textDedApplied.Size = new System.Drawing.Size(70, 20);
			this.textDedApplied.TabIndex = 1;
			this.textDedApplied.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textDedApplied.Leave += new System.EventHandler(this.textDedApplied_Leave);
			// 
			// labelProcDate
			// 
			this.labelProcDate.Location = new System.Drawing.Point(30, 166);
			this.labelProcDate.Name = "labelProcDate";
			this.labelProcDate.Size = new System.Drawing.Size(126, 17);
			this.labelProcDate.TabIndex = 0;
			this.labelProcDate.Text = "Procedure Date";
			this.labelProcDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelDateEntry
			// 
			this.labelDateEntry.Location = new System.Drawing.Point(30, 123);
			this.labelDateEntry.Name = "labelDateEntry";
			this.labelDateEntry.Size = new System.Drawing.Size(125, 17);
			this.labelDateEntry.TabIndex = 0;
			this.labelDateEntry.Text = "Pay Entry Date";
			this.labelDateEntry.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textClinic
			// 
			this.textClinic.Location = new System.Drawing.Point(157, 97);
			this.textClinic.Name = "textClinic";
			this.textClinic.ReadOnly = true;
			this.textClinic.Size = new System.Drawing.Size(148, 20);
			this.textClinic.TabIndex = 0;
			this.textClinic.TabStop = false;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(33, 99);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(121, 14);
			this.labelClinic.TabIndex = 0;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProvider
			// 
			this.comboProvider.Location = new System.Drawing.Point(157, 74);
			this.comboProvider.Name = "comboProvider";
			this.comboProvider.Size = new System.Drawing.Size(145, 21);
			this.comboProvider.TabIndex = 3;
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.FormattingEnabled = true;
			this.comboStatus.Location = new System.Drawing.Point(157, 26);
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(145, 21);
			this.comboStatus.TabIndex = 1;
			this.comboStatus.SelectionChangeCommitted += new System.EventHandler(this.comboStatus_SelectionChangeCommitted);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(517, 435);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(388, 17);
			this.label6.TabIndex = 0;
			this.label6.Text = "Values above change based on current insurance information.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// checkPayPlan
			// 
			this.checkPayPlan.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPayPlan.Location = new System.Drawing.Point(375, 600);
			this.checkPayPlan.Name = "checkPayPlan";
			this.checkPayPlan.Size = new System.Drawing.Size(346, 18);
			this.checkPayPlan.TabIndex = 12;
			this.checkPayPlan.Text = "Attached to Insurance Payment Plan";
			this.checkPayPlan.Click += new System.EventHandler(this.checkPayPlan_Click);
			// 
			// comboPayTracker
			// 
			this.comboPayTracker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPayTracker.FormattingEnabled = true;
			this.comboPayTracker.Location = new System.Drawing.Point(157, 50);
			this.comboPayTracker.Name = "comboPayTracker";
			this.comboPayTracker.Size = new System.Drawing.Size(145, 21);
			this.comboPayTracker.TabIndex = 2;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(36, 53);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(119, 17);
			this.label12.TabIndex = 0;
			this.label12.Text = "Payment Tracking";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPickProv
			// 
			this.butPickProv.Location = new System.Drawing.Point(304, 74);
			this.butPickProv.Name = "butPickProv";
			this.butPickProv.Size = new System.Drawing.Size(18, 20);
			this.butPickProv.TabIndex = 4;
			this.butPickProv.Text = "...";
			this.butPickProv.Click += new System.EventHandler(this.butPickProv_Click);
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(157, 119);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(83, 20);
			this.textDateEntry.TabIndex = 0;
			this.textDateEntry.TabStop = false;
			// 
			// textProcDate
			// 
			this.textProcDate.Location = new System.Drawing.Point(157, 162);
			this.textProcDate.Name = "textProcDate";
			this.textProcDate.Size = new System.Drawing.Size(83, 20);
			this.textProcDate.TabIndex = 6;
			// 
			// textDateCP
			// 
			this.textDateCP.Location = new System.Drawing.Point(157, 140);
			this.textDateCP.Name = "textDateCP";
			this.textDateCP.Size = new System.Drawing.Size(83, 20);
			this.textDateCP.TabIndex = 5;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(18, 625);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 11;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(894, 625);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(806, 625);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormClaimProc
			// 
			this.ClientSize = new System.Drawing.Size(981, 658);
			this.Controls.Add(this.comboPayTracker);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.checkPayPlan);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboStatus);
			this.Controls.Add(this.butPickProv);
			this.Controls.Add(this.comboProvider);
			this.Controls.Add(this.textClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.labelDateEntry);
			this.Controls.Add(this.textProcDate);
			this.Controls.Add(this.labelProcDate);
			this.Controls.Add(this.groupClaim);
			this.Controls.Add(this.groupClaimInfo);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.textDateCP);
			this.Controls.Add(this.textInsPlan);
			this.Controls.Add(this.label30);
			this.Controls.Add(this.label29);
			this.Controls.Add(this.label28);
			this.Controls.Add(this.checkNoBillIns);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.panelEstimateInfo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClaimProc";
			this.ShowInTaskbar = false;
			this.Text = "Edit Claim Procedure";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormClaimProc_Closing);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClaimProc_FormClosing);
			this.Load += new System.EventHandler(this.FormClaimProcEdit_Load);
			this.groupClaim.ResumeLayout(false);
			this.panelClaimExtras.ResumeLayout(false);
			this.panelClaimExtras.PerformLayout();
			this.panelEstimateInfo.ResumeLayout(false);
			this.panelEstimateInfo.PerformLayout();
			this.groupAllowed.ResumeLayout(false);
			this.groupAllowed.PerformLayout();
			this.groupClaimInfo.ResumeLayout(false);
			this.groupClaimInfo.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label9;
		private OpenDental.ValidDouble textInsPayAmt;
		private System.Windows.Forms.TextBox textRemarks;
		private OpenDental.ValidDouble textWriteOff;
		private OpenDental.ValidDouble textInsPayEst;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textInsPlan;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox checkNoBillIns;
		private System.Windows.Forms.TextBox textPercentage;
		private OpenDental.ValidDouble textCopayAmt;
		private OpenDental.ValidNum textPercentOverride;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.TextBox textCodeSent;
		private OpenDental.ValidDouble textFeeBilled;
		private OpenDental.ValidDouble textDedApplied;
		private System.Windows.Forms.RadioButton radioEstimate;
		private System.Windows.Forms.RadioButton radioClaim;
		private OpenDental.ValidDate textDateCP;
		private OpenDental.ValidDouble textAllowedOverride;
		private OpenDental.ValidDouble textPaidOtherIns;
		private System.Windows.Forms.TextBox textFee;
		private System.Windows.Forms.Label labelDedApplied;
		private System.Windows.Forms.Panel panelEstimateInfo;
		private System.Windows.Forms.Label labelNotInClaim;
		private System.Windows.Forms.Label labelAttachedToCheck;
		private System.Windows.Forms.GroupBox groupClaimInfo;
		private System.Windows.Forms.Label labelInsPayAmt;
		private System.Windows.Forms.Label labelInsPayEst;
		private System.Windows.Forms.Label labelPaidOtherIns;
		private System.Windows.Forms.Label labelCopayAmt;
		private System.Windows.Forms.Label labelWriteOff;
		private System.Windows.Forms.Label labelFee;
		private System.Windows.Forms.Label labelCodeSent;
		private System.Windows.Forms.Label labelFeeBilled;
		private System.Windows.Forms.Label labelRemarks;
		private OpenDental.ValidDouble textCopayOverride;
		private System.Windows.Forms.Panel panelClaimExtras;
		private System.Windows.Forms.GroupBox groupClaim;
		private OpenDental.ValidDate textProcDate;
		private System.Windows.Forms.Label labelProcDate;
		private System.Windows.Forms.Label labelCarrierAllowed;
		private System.Windows.Forms.TextBox textCarrierAllowed;
		private OpenDental.UI.Button butUpdateAllowed;
		private OpenDental.ValidDate textDateEntry;
		private System.Windows.Forms.Label labelDateEntry;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label labelFeeSched;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPPOFeeSched;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textSubstCode;
		private System.Windows.Forms.TextBox textFeeSched;
		private System.Windows.Forms.TextBox textAllowedFeeSched;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupAllowed;
		private System.Windows.Forms.Label label10;
		private ValidDouble textDedEstOverride;
		private ValidDouble textDedEst;
		private System.Windows.Forms.Label label11;
		private ValidDouble textInsEstTotal;
		private System.Windows.Forms.Label label17;
		private ValidDouble textInsEstTotalOverride;
		private ValidDouble textPaidOtherInsOverride;
		private ValidDouble textPatPortion1;
		private System.Windows.Forms.Label labelPatPortion1;
		private ValidDouble textPatPortion2;
		private System.Windows.Forms.Label labelPatPortion2;
		private ValidDouble textBaseEst;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textEstimateNote;
		private ValidDouble textWriteOffEstOverride;
		private ValidDouble textWriteOffEst;
		private System.Windows.Forms.Label labelWriteOffEst;
		private System.Windows.Forms.TextBox textClinic;
		private System.Windows.Forms.Label labelClinic;
		private OpenDental.UI.Button butPickProv;
		private OpenDental.UI.ComboBoxOD comboProvider;
		private System.Windows.Forms.ComboBox comboStatus;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkPayPlan;
		private System.Windows.Forms.ComboBox comboPayTracker;
		private System.Windows.Forms.Label label12;
		private UI.Button butBlueBookLog;
		private System.Windows.Forms.Label labelClaimAdjReasonCodes;
		private System.Windows.Forms.TextBox textClaimAdjReasonCodes;
	}
}
