namespace OpenDental{
	partial class FormOrthoCase {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoCase));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupVisit = new System.Windows.Forms.GroupBox();
			this.textVisitPercent = new OpenDental.ValidDouble();
			this.labelVisitsCompleted = new System.Windows.Forms.Label();
			this.TextVisitCountComplete = new OpenDental.ValidNum();
			this.labelVisitAmount = new System.Windows.Forms.Label();
			this.labelVisitPercent = new System.Windows.Forms.Label();
			this.textVisitAmount = new OpenDental.ValidDouble();
			this.labelVisitsPlanned = new System.Windows.Forms.Label();
			this.textVisitCountPlanned = new OpenDental.ValidNum();
			this.textAllVisitsPercent = new OpenDental.ValidDouble();
			this.textDebondPercent = new OpenDental.ValidDouble();
			this.textBandingPercent = new OpenDental.ValidDouble();
			this.labelFeesAmount = new System.Windows.Forms.Label();
			this.textAllVisitsAmount = new OpenDental.ValidDouble();
			this.textDebondAmount = new OpenDental.ValidDouble();
			this.textBandingAmount = new OpenDental.ValidDouble();
			this.labelFeesPercent = new System.Windows.Forms.Label();
			this.labelVisitsFees = new System.Windows.Forms.Label();
			this.labelDebondFee = new System.Windows.Forms.Label();
			this.labelBandingFee = new System.Windows.Forms.Label();
			this.labelTreatmentLength = new System.Windows.Forms.Label();
			this.textTreatmentLength = new OpenDental.ValidNum();
			this.textPatAR = new OpenDental.ValidDouble();
			this.textPrimaryInsAR = new OpenDental.ValidDouble();
			this.textTotalAR = new OpenDental.ValidDouble();
			this.labelAR = new System.Windows.Forms.Label();
			this.textBandingProc = new System.Windows.Forms.TextBox();
			this.butAddBandingProcedure = new OpenDental.UI.Button();
			this.checkIsTransfer = new System.Windows.Forms.CheckBox();
			this.labelRemaining = new System.Windows.Forms.Label();
			this.labelCompleted = new System.Windows.Forms.Label();
			this.labelTotal = new System.Windows.Forms.Label();
			this.textPatRemaining = new OpenDental.ValidDouble();
			this.labelPatientFee = new System.Windows.Forms.Label();
			this.textPatCompleted = new OpenDental.ValidDouble();
			this.textPatientFee = new OpenDental.ValidDouble();
			this.textPrimaryInsRemaining = new OpenDental.ValidDouble();
			this.labelPrimaryInsuranceFee = new System.Windows.Forms.Label();
			this.textPrimaryInsCompleted = new OpenDental.ValidDouble();
			this.textPrimaryInsuranceFee = new OpenDental.ValidDouble();
			this.textTotalRemaining = new OpenDental.ValidDouble();
			this.textExpectedDebondDate = new OpenDental.ValidDate();
			this.textTotalCompleted = new OpenDental.ValidDouble();
			this.labelExpectedDebondDate = new System.Windows.Forms.Label();
			this.textBandingDate = new OpenDental.ValidDate();
			this.labelBandingDate = new System.Windows.Forms.Label();
			this.labelTotalFee = new System.Windows.Forms.Label();
			this.textTotalFee = new OpenDental.ValidDouble();
			this.gridOrthoSchedule = new OpenDental.UI.GridOD();
			this.butCloseOrthoCase = new OpenDental.UI.Button();
			this.butDeleteOrthoCase = new OpenDental.UI.Button();
			this.butDetachProcs = new OpenDental.UI.Button();
			this.groupFee = new System.Windows.Forms.GroupBox();
			this.textSecondaryInsuranceFee = new OpenDental.ValidDouble();
			this.textSecondaryInsCompleted = new OpenDental.ValidDouble();
			this.textSecondaryInsAR = new OpenDental.ValidDouble();
			this.labelSecondaryInsuranceFee = new System.Windows.Forms.Label();
			this.textSecondaryInsRemaining = new OpenDental.ValidDouble();
			this.groupDates = new System.Windows.Forms.GroupBox();
			this.groupProcBreakdown = new System.Windows.Forms.GroupBox();
			this.labelBandingProc = new System.Windows.Forms.Label();
			this.butPatPayPlan = new OpenDental.UI.Button();
			this.groupVisit.SuspendLayout();
			this.groupFee.SuspendLayout();
			this.groupDates.SuspendLayout();
			this.groupProcBreakdown.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(707, 384);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 9;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.ButOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(788, 384);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.ButCancel_Click);
			// 
			// groupVisit
			// 
			this.groupVisit.Controls.Add(this.textVisitPercent);
			this.groupVisit.Controls.Add(this.labelVisitsCompleted);
			this.groupVisit.Controls.Add(this.TextVisitCountComplete);
			this.groupVisit.Controls.Add(this.labelVisitAmount);
			this.groupVisit.Controls.Add(this.labelVisitPercent);
			this.groupVisit.Controls.Add(this.textVisitAmount);
			this.groupVisit.Controls.Add(this.labelVisitsPlanned);
			this.groupVisit.Controls.Add(this.textVisitCountPlanned);
			this.groupVisit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupVisit.Location = new System.Drawing.Point(304, 178);
			this.groupVisit.Name = "groupVisit";
			this.groupVisit.Size = new System.Drawing.Size(194, 105);
			this.groupVisit.TabIndex = 4;
			this.groupVisit.TabStop = false;
			this.groupVisit.Text = "Visit Details";
			// 
			// textVisitPercent
			// 
			this.textVisitPercent.Enabled = false;
			this.textVisitPercent.Location = new System.Drawing.Point(127, 14);
			this.textVisitPercent.MaxVal = 100D;
			this.textVisitPercent.MinVal = 0D;
			this.textVisitPercent.Name = "textVisitPercent";
			this.textVisitPercent.Size = new System.Drawing.Size(62, 20);
			this.textVisitPercent.TabIndex = 0;
			this.textVisitPercent.TextChanged += new System.EventHandler(this.TextVisitPercent_TextChanged);
			this.textVisitPercent.Leave += new System.EventHandler(this.TextVisitPercent_Leave);
			// 
			// labelVisitsCompleted
			// 
			this.labelVisitsCompleted.Location = new System.Drawing.Point(18, 82);
			this.labelVisitsCompleted.Name = "labelVisitsCompleted";
			this.labelVisitsCompleted.Size = new System.Drawing.Size(108, 17);
			this.labelVisitsCompleted.TabIndex = 242;
			this.labelVisitsCompleted.Text = "Visits Completed";
			this.labelVisitsCompleted.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// TextVisitCountComplete
			// 
			this.TextVisitCountComplete.Enabled = false;
			this.TextVisitCountComplete.Location = new System.Drawing.Point(127, 80);
			this.TextVisitCountComplete.MaxLength = 4;
			this.TextVisitCountComplete.MaxVal = 9999;
			this.TextVisitCountComplete.MinVal = 0;
			this.TextVisitCountComplete.Name = "TextVisitCountComplete";
			this.TextVisitCountComplete.Size = new System.Drawing.Size(62, 20);
			this.TextVisitCountComplete.TabIndex = 241;
			this.TextVisitCountComplete.ShowZero = false;
			// 
			// labelVisitAmount
			// 
			this.labelVisitAmount.Location = new System.Drawing.Point(18, 38);
			this.labelVisitAmount.Name = "labelVisitAmount";
			this.labelVisitAmount.Size = new System.Drawing.Size(108, 17);
			this.labelVisitAmount.TabIndex = 234;
			this.labelVisitAmount.Text = "Amount per Visit";
			this.labelVisitAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelVisitPercent
			// 
			this.labelVisitPercent.Location = new System.Drawing.Point(18, 16);
			this.labelVisitPercent.Name = "labelVisitPercent";
			this.labelVisitPercent.Size = new System.Drawing.Size(108, 17);
			this.labelVisitPercent.TabIndex = 233;
			this.labelVisitPercent.Text = "Percent per Visit";
			this.labelVisitPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVisitAmount
			// 
			this.textVisitAmount.Enabled = false;
			this.textVisitAmount.Location = new System.Drawing.Point(127, 36);
			this.textVisitAmount.MaxVal = 100000000D;
			this.textVisitAmount.MinVal = 0D;
			this.textVisitAmount.Name = "textVisitAmount";
			this.textVisitAmount.Size = new System.Drawing.Size(62, 20);
			this.textVisitAmount.TabIndex = 1;
			this.textVisitAmount.TextChanged += new System.EventHandler(this.TextVisitAmount_TextChanged);
			this.textVisitAmount.Leave += new System.EventHandler(this.TextVisitAmount_Leave);
			// 
			// labelVisitsPlanned
			// 
			this.labelVisitsPlanned.Location = new System.Drawing.Point(18, 60);
			this.labelVisitsPlanned.Name = "labelVisitsPlanned";
			this.labelVisitsPlanned.Size = new System.Drawing.Size(108, 17);
			this.labelVisitsPlanned.TabIndex = 232;
			this.labelVisitsPlanned.Text = "Visits Planned";
			this.labelVisitsPlanned.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVisitCountPlanned
			// 
			this.textVisitCountPlanned.Enabled = false;
			this.textVisitCountPlanned.Location = new System.Drawing.Point(127, 58);
			this.textVisitCountPlanned.MaxLength = 4;
			this.textVisitCountPlanned.MaxVal = 9999;
			this.textVisitCountPlanned.MinVal = 0;
			this.textVisitCountPlanned.Name = "textVisitCountPlanned";
			this.textVisitCountPlanned.Size = new System.Drawing.Size(62, 20);
			this.textVisitCountPlanned.TabIndex = 2;
			this.textVisitCountPlanned.TextChanged += new System.EventHandler(this.TextVisitCountPlanned_TextChanged);
			this.textVisitCountPlanned.Leave += new System.EventHandler(this.TextVisitCountPlanned_Leave);
			this.textVisitCountPlanned.ShowZero = false;
			// 
			// textAllVisitsPercent
			// 
			this.textAllVisitsPercent.Enabled = false;
			this.textAllVisitsPercent.Location = new System.Drawing.Point(138, 70);
			this.textAllVisitsPercent.MaxVal = 100D;
			this.textAllVisitsPercent.MinVal = 0D;
			this.textAllVisitsPercent.Name = "textAllVisitsPercent";
			this.textAllVisitsPercent.Size = new System.Drawing.Size(62, 20);
			this.textAllVisitsPercent.TabIndex = 2;
			this.textAllVisitsPercent.TextChanged += new System.EventHandler(this.TextAllVisitsPercent_TextChanged);
			this.textAllVisitsPercent.Leave += new System.EventHandler(this.TextAllVisitsPercent_Leave);
			// 
			// textDebondPercent
			// 
			this.textDebondPercent.Enabled = false;
			this.textDebondPercent.Location = new System.Drawing.Point(138, 48);
			this.textDebondPercent.MaxVal = 100D;
			this.textDebondPercent.MinVal = 0D;
			this.textDebondPercent.Name = "textDebondPercent";
			this.textDebondPercent.Size = new System.Drawing.Size(62, 20);
			this.textDebondPercent.TabIndex = 1;
			this.textDebondPercent.TextChanged += new System.EventHandler(this.TextDebondPercent_TextChanged);
			this.textDebondPercent.Leave += new System.EventHandler(this.TextDebondPercent_Leave);
			// 
			// textBandingPercent
			// 
			this.textBandingPercent.Enabled = false;
			this.textBandingPercent.Location = new System.Drawing.Point(138, 26);
			this.textBandingPercent.MaxVal = 100D;
			this.textBandingPercent.MinVal = 0D;
			this.textBandingPercent.Name = "textBandingPercent";
			this.textBandingPercent.Size = new System.Drawing.Size(62, 20);
			this.textBandingPercent.TabIndex = 0;
			this.textBandingPercent.TextChanged += new System.EventHandler(this.TextBandingPercent_TextChanged);
			this.textBandingPercent.Leave += new System.EventHandler(this.TextBandingPercent_Leave);
			// 
			// labelFeesAmount
			// 
			this.labelFeesAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFeesAmount.Location = new System.Drawing.Point(202, 10);
			this.labelFeesAmount.Name = "labelFeesAmount";
			this.labelFeesAmount.Size = new System.Drawing.Size(85, 15);
			this.labelFeesAmount.TabIndex = 207;
			this.labelFeesAmount.Text = "$";
			this.labelFeesAmount.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textAllVisitsAmount
			// 
			this.textAllVisitsAmount.Enabled = false;
			this.textAllVisitsAmount.Location = new System.Drawing.Point(202, 70);
			this.textAllVisitsAmount.MaxVal = 100000000D;
			this.textAllVisitsAmount.MinVal = 0D;
			this.textAllVisitsAmount.Name = "textAllVisitsAmount";
			this.textAllVisitsAmount.Size = new System.Drawing.Size(85, 20);
			this.textAllVisitsAmount.TabIndex = 5;
			this.textAllVisitsAmount.TextChanged += new System.EventHandler(this.TextAllVisitsAmount_TextChanged);
			this.textAllVisitsAmount.Leave += new System.EventHandler(this.TextAllVisitsAmount_Leave);
			// 
			// textDebondAmount
			// 
			this.textDebondAmount.Enabled = false;
			this.textDebondAmount.Location = new System.Drawing.Point(202, 48);
			this.textDebondAmount.MaxVal = 100000000D;
			this.textDebondAmount.MinVal = 0D;
			this.textDebondAmount.Name = "textDebondAmount";
			this.textDebondAmount.Size = new System.Drawing.Size(85, 20);
			this.textDebondAmount.TabIndex = 4;
			this.textDebondAmount.TextChanged += new System.EventHandler(this.TextDebondAmount_TextChanged);
			this.textDebondAmount.Leave += new System.EventHandler(this.TextDebondAmount_Leave);
			// 
			// textBandingAmount
			// 
			this.textBandingAmount.Enabled = false;
			this.textBandingAmount.Location = new System.Drawing.Point(202, 26);
			this.textBandingAmount.MaxVal = 100000000D;
			this.textBandingAmount.MinVal = 0D;
			this.textBandingAmount.Name = "textBandingAmount";
			this.textBandingAmount.Size = new System.Drawing.Size(85, 20);
			this.textBandingAmount.TabIndex = 3;
			this.textBandingAmount.TextChanged += new System.EventHandler(this.TextBandingAmount_TextChanged);
			this.textBandingAmount.Leave += new System.EventHandler(this.TextBandingAmount_Leave);
			// 
			// labelFeesPercent
			// 
			this.labelFeesPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFeesPercent.Location = new System.Drawing.Point(138, 10);
			this.labelFeesPercent.Name = "labelFeesPercent";
			this.labelFeesPercent.Size = new System.Drawing.Size(62, 15);
			this.labelFeesPercent.TabIndex = 203;
			this.labelFeesPercent.Text = "%";
			this.labelFeesPercent.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// labelVisitsFees
			// 
			this.labelVisitsFees.Location = new System.Drawing.Point(25, 71);
			this.labelVisitsFees.Name = "labelVisitsFees";
			this.labelVisitsFees.Size = new System.Drawing.Size(112, 17);
			this.labelVisitsFees.TabIndex = 31;
			this.labelVisitsFees.Text = "All Visits Amount";
			this.labelVisitsFees.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDebondFee
			// 
			this.labelDebondFee.Location = new System.Drawing.Point(22, 49);
			this.labelDebondFee.Name = "labelDebondFee";
			this.labelDebondFee.Size = new System.Drawing.Size(115, 17);
			this.labelDebondFee.TabIndex = 7;
			this.labelDebondFee.Text = "Debond Amount";
			this.labelDebondFee.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelBandingFee
			// 
			this.labelBandingFee.Location = new System.Drawing.Point(19, 27);
			this.labelBandingFee.Name = "labelBandingFee";
			this.labelBandingFee.Size = new System.Drawing.Size(118, 17);
			this.labelBandingFee.TabIndex = 5;
			this.labelBandingFee.Text = "Banding Amount";
			this.labelBandingFee.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTreatmentLength
			// 
			this.labelTreatmentLength.Location = new System.Drawing.Point(13, 56);
			this.labelTreatmentLength.Name = "labelTreatmentLength";
			this.labelTreatmentLength.Size = new System.Drawing.Size(188, 17);
			this.labelTreatmentLength.TabIndex = 243;
			this.labelTreatmentLength.Text = "Expected days of treatment";
			this.labelTreatmentLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTreatmentLength
			// 
			this.textTreatmentLength.Enabled = false;
			this.textTreatmentLength.Location = new System.Drawing.Point(202, 55);
			this.textTreatmentLength.MaxLength = 4;
			this.textTreatmentLength.MaxVal = 9999;
			this.textTreatmentLength.MinVal = 0;
			this.textTreatmentLength.Name = "textTreatmentLength";
			this.textTreatmentLength.Size = new System.Drawing.Size(85, 20);
			this.textTreatmentLength.TabIndex = 17;
			this.textTreatmentLength.ShowZero = false;
			// 
			// textPatAR
			// 
			this.textPatAR.Enabled = false;
			this.textPatAR.Location = new System.Drawing.Point(399, 48);
			this.textPatAR.MaxVal = 100000000D;
			this.textPatAR.MinVal = 0D;
			this.textPatAR.Name = "textPatAR";
			this.textPatAR.Size = new System.Drawing.Size(85, 20);
			this.textPatAR.TabIndex = 238;
			// 
			// textPrimaryInsAR
			// 
			this.textPrimaryInsAR.Enabled = false;
			this.textPrimaryInsAR.Location = new System.Drawing.Point(399, 70);
			this.textPrimaryInsAR.MaxVal = 100000000D;
			this.textPrimaryInsAR.MinVal = 0D;
			this.textPrimaryInsAR.Name = "textPrimaryInsAR";
			this.textPrimaryInsAR.Size = new System.Drawing.Size(85, 20);
			this.textPrimaryInsAR.TabIndex = 237;
			// 
			// textTotalAR
			// 
			this.textTotalAR.Enabled = false;
			this.textTotalAR.Location = new System.Drawing.Point(399, 26);
			this.textTotalAR.MaxVal = 100000000D;
			this.textTotalAR.MinVal = 0D;
			this.textTotalAR.Name = "textTotalAR";
			this.textTotalAR.Size = new System.Drawing.Size(85, 20);
			this.textTotalAR.TabIndex = 236;
			// 
			// labelAR
			// 
			this.labelAR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAR.Location = new System.Drawing.Point(399, 10);
			this.labelAR.Name = "labelAR";
			this.labelAR.Size = new System.Drawing.Size(85, 15);
			this.labelAR.TabIndex = 235;
			this.labelAR.Text = "AR";
			this.labelAR.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textBandingProc
			// 
			this.textBandingProc.BackColor = System.Drawing.SystemColors.Window;
			this.textBandingProc.Enabled = false;
			this.textBandingProc.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textBandingProc.Location = new System.Drawing.Point(233, 14);
			this.textBandingProc.Name = "textBandingProc";
			this.textBandingProc.ReadOnly = true;
			this.textBandingProc.Size = new System.Drawing.Size(265, 20);
			this.textBandingProc.TabIndex = 0;
			// 
			// butAddBandingProcedure
			// 
			this.butAddBandingProcedure.Location = new System.Drawing.Point(504, 14);
			this.butAddBandingProcedure.Name = "butAddBandingProcedure";
			this.butAddBandingProcedure.Size = new System.Drawing.Size(23, 21);
			this.butAddBandingProcedure.TabIndex = 0;
			this.butAddBandingProcedure.Text = "...";
			this.butAddBandingProcedure.Click += new System.EventHandler(this.ButAddBandingProcedure_Click);
			// 
			// checkIsTransfer
			// 
			this.checkIsTransfer.Location = new System.Drawing.Point(557, 14);
			this.checkIsTransfer.Name = "checkIsTransfer";
			this.checkIsTransfer.Size = new System.Drawing.Size(133, 20);
			this.checkIsTransfer.TabIndex = 1;
			this.checkIsTransfer.Text = "Is Transfer";
			this.checkIsTransfer.UseVisualStyleBackColor = true;
			this.checkIsTransfer.CheckedChanged += new System.EventHandler(this.CheckIsTransfer_CheckedChanged);
			// 
			// labelRemaining
			// 
			this.labelRemaining.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRemaining.Location = new System.Drawing.Point(312, 10);
			this.labelRemaining.Name = "labelRemaining";
			this.labelRemaining.Size = new System.Drawing.Size(85, 15);
			this.labelRemaining.TabIndex = 225;
			this.labelRemaining.Text = "Remaining";
			this.labelRemaining.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// labelCompleted
			// 
			this.labelCompleted.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCompleted.Location = new System.Drawing.Point(225, 10);
			this.labelCompleted.Name = "labelCompleted";
			this.labelCompleted.Size = new System.Drawing.Size(85, 15);
			this.labelCompleted.TabIndex = 224;
			this.labelCompleted.Text = "Completed";
			this.labelCompleted.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// labelTotal
			// 
			this.labelTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTotal.Location = new System.Drawing.Point(138, 10);
			this.labelTotal.Name = "labelTotal";
			this.labelTotal.Size = new System.Drawing.Size(85, 15);
			this.labelTotal.TabIndex = 223;
			this.labelTotal.Text = "Total";
			this.labelTotal.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textPatRemaining
			// 
			this.textPatRemaining.Enabled = false;
			this.textPatRemaining.Location = new System.Drawing.Point(312, 48);
			this.textPatRemaining.MaxVal = 100000000D;
			this.textPatRemaining.MinVal = 0D;
			this.textPatRemaining.Name = "textPatRemaining";
			this.textPatRemaining.Size = new System.Drawing.Size(85, 20);
			this.textPatRemaining.TabIndex = 222;
			// 
			// labelPatientFee
			// 
			this.labelPatientFee.Location = new System.Drawing.Point(13, 50);
			this.labelPatientFee.Name = "labelPatientFee";
			this.labelPatientFee.Size = new System.Drawing.Size(124, 17);
			this.labelPatientFee.TabIndex = 211;
			this.labelPatientFee.Text = "Patient Portion";
			this.labelPatientFee.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatCompleted
			// 
			this.textPatCompleted.Enabled = false;
			this.textPatCompleted.Location = new System.Drawing.Point(225, 48);
			this.textPatCompleted.MaxVal = 100000000D;
			this.textPatCompleted.MinVal = 0D;
			this.textPatCompleted.Name = "textPatCompleted";
			this.textPatCompleted.Size = new System.Drawing.Size(85, 20);
			this.textPatCompleted.TabIndex = 221;
			// 
			// textPatientFee
			// 
			this.textPatientFee.Enabled = false;
			this.textPatientFee.Location = new System.Drawing.Point(138, 48);
			this.textPatientFee.MaxVal = 100000000D;
			this.textPatientFee.MinVal = 0D;
			this.textPatientFee.Name = "textPatientFee";
			this.textPatientFee.Size = new System.Drawing.Size(85, 20);
			this.textPatientFee.TabIndex = 1;
			this.textPatientFee.TextChanged += new System.EventHandler(this.TextPatientFee_TextChanged);
			this.textPatientFee.Leave += new System.EventHandler(this.TextPatientFee_Leave);
			// 
			// textPrimaryInsRemaining
			// 
			this.textPrimaryInsRemaining.Enabled = false;
			this.textPrimaryInsRemaining.Location = new System.Drawing.Point(312, 70);
			this.textPrimaryInsRemaining.MaxVal = 100000000D;
			this.textPrimaryInsRemaining.MinVal = 0D;
			this.textPrimaryInsRemaining.Name = "textPrimaryInsRemaining";
			this.textPrimaryInsRemaining.Size = new System.Drawing.Size(85, 20);
			this.textPrimaryInsRemaining.TabIndex = 219;
			// 
			// labelPrimaryInsuranceFee
			// 
			this.labelPrimaryInsuranceFee.Location = new System.Drawing.Point(13, 72);
			this.labelPrimaryInsuranceFee.Name = "labelPrimaryInsuranceFee";
			this.labelPrimaryInsuranceFee.Size = new System.Drawing.Size(124, 17);
			this.labelPrimaryInsuranceFee.TabIndex = 209;
			this.labelPrimaryInsuranceFee.Text = "Primary Insurance";
			this.labelPrimaryInsuranceFee.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPrimaryInsCompleted
			// 
			this.textPrimaryInsCompleted.Enabled = false;
			this.textPrimaryInsCompleted.Location = new System.Drawing.Point(225, 70);
			this.textPrimaryInsCompleted.MaxVal = 100000000D;
			this.textPrimaryInsCompleted.MinVal = 0D;
			this.textPrimaryInsCompleted.Name = "textPrimaryInsCompleted";
			this.textPrimaryInsCompleted.Size = new System.Drawing.Size(85, 20);
			this.textPrimaryInsCompleted.TabIndex = 218;
			// 
			// textPrimaryInsuranceFee
			// 
			this.textPrimaryInsuranceFee.Enabled = false;
			this.textPrimaryInsuranceFee.Location = new System.Drawing.Point(138, 70);
			this.textPrimaryInsuranceFee.MaxVal = 100000000D;
			this.textPrimaryInsuranceFee.MinVal = 0D;
			this.textPrimaryInsuranceFee.Name = "textPrimaryInsuranceFee";
			this.textPrimaryInsuranceFee.Size = new System.Drawing.Size(85, 20);
			this.textPrimaryInsuranceFee.TabIndex = 2;
			this.textPrimaryInsuranceFee.TextChanged += new System.EventHandler(this.TextPrimaryInsuranceFee_TextChanged);
			this.textPrimaryInsuranceFee.Leave += new System.EventHandler(this.TextPrimaryInsuranceFee_Leave);
			// 
			// textTotalRemaining
			// 
			this.textTotalRemaining.Enabled = false;
			this.textTotalRemaining.Location = new System.Drawing.Point(312, 26);
			this.textTotalRemaining.MaxVal = 100000000D;
			this.textTotalRemaining.MinVal = 0D;
			this.textTotalRemaining.Name = "textTotalRemaining";
			this.textTotalRemaining.Size = new System.Drawing.Size(85, 20);
			this.textTotalRemaining.TabIndex = 216;
			// 
			// textExpectedDebondDate
			// 
			this.textExpectedDebondDate.Location = new System.Drawing.Point(202, 33);
			this.textExpectedDebondDate.Name = "textExpectedDebondDate";
			this.textExpectedDebondDate.Size = new System.Drawing.Size(85, 20);
			this.textExpectedDebondDate.TabIndex = 1;
			this.textExpectedDebondDate.TextChanged += new System.EventHandler(this.TextExpectedDebondDate_TextChanged);
			// 
			// textTotalCompleted
			// 
			this.textTotalCompleted.Enabled = false;
			this.textTotalCompleted.Location = new System.Drawing.Point(225, 26);
			this.textTotalCompleted.MaxVal = 100000000D;
			this.textTotalCompleted.MinVal = 0D;
			this.textTotalCompleted.Name = "textTotalCompleted";
			this.textTotalCompleted.Size = new System.Drawing.Size(85, 20);
			this.textTotalCompleted.TabIndex = 214;
			// 
			// labelExpectedDebondDate
			// 
			this.labelExpectedDebondDate.Location = new System.Drawing.Point(25, 34);
			this.labelExpectedDebondDate.Name = "labelExpectedDebondDate";
			this.labelExpectedDebondDate.Size = new System.Drawing.Size(176, 17);
			this.labelExpectedDebondDate.TabIndex = 0;
			this.labelExpectedDebondDate.Text = "Expected Debond Date";
			this.labelExpectedDebondDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBandingDate
			// 
			this.textBandingDate.Location = new System.Drawing.Point(202, 11);
			this.textBandingDate.Name = "textBandingDate";
			this.textBandingDate.Size = new System.Drawing.Size(85, 20);
			this.textBandingDate.TabIndex = 0;
			this.textBandingDate.TextChanged += new System.EventHandler(this.TextBandingDate_TextChanged);
			// 
			// labelBandingDate
			// 
			this.labelBandingDate.Location = new System.Drawing.Point(28, 11);
			this.labelBandingDate.Name = "labelBandingDate";
			this.labelBandingDate.Size = new System.Drawing.Size(173, 17);
			this.labelBandingDate.TabIndex = 0;
			this.labelBandingDate.Text = "Banding Date";
			this.labelBandingDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTotalFee
			// 
			this.labelTotalFee.Location = new System.Drawing.Point(13, 27);
			this.labelTotalFee.Name = "labelTotalFee";
			this.labelTotalFee.Size = new System.Drawing.Size(124, 17);
			this.labelTotalFee.TabIndex = 0;
			this.labelTotalFee.Text = "Fee";
			this.labelTotalFee.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTotalFee
			// 
			this.textTotalFee.Enabled = false;
			this.textTotalFee.Location = new System.Drawing.Point(138, 26);
			this.textTotalFee.MaxVal = 100000000D;
			this.textTotalFee.MinVal = 0.01D;
			this.textTotalFee.Name = "textTotalFee";
			this.textTotalFee.Size = new System.Drawing.Size(85, 20);
			this.textTotalFee.TabIndex = 0;
			this.textTotalFee.TextChanged += new System.EventHandler(this.TextTotalFee_TextChanged);
			this.textTotalFee.Leave += new System.EventHandler(this.TextTotalFee_Leave);
			// 
			// gridOrthoSchedule
			// 
			this.gridOrthoSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridOrthoSchedule.Location = new System.Drawing.Point(505, 58);
			this.gridOrthoSchedule.Name = "gridOrthoSchedule";
			this.gridOrthoSchedule.Size = new System.Drawing.Size(358, 320);
			this.gridOrthoSchedule.TabIndex = 7;
			this.gridOrthoSchedule.Title = "Ortho Schedule";
			this.gridOrthoSchedule.TranslationName = "TableOrthoSchedule";
			this.gridOrthoSchedule.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridOrthoSchedule_CellDoubleClick);
			this.gridOrthoSchedule.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.GridOrthoSchedule_CellClick);
			// 
			// butCloseOrthoCase
			// 
			this.butCloseOrthoCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCloseOrthoCase.Image = global::OpenDental.Properties.Resources.close_door;
			this.butCloseOrthoCase.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCloseOrthoCase.Location = new System.Drawing.Point(101, 384);
			this.butCloseOrthoCase.Name = "butCloseOrthoCase";
			this.butCloseOrthoCase.Size = new System.Drawing.Size(98, 24);
			this.butCloseOrthoCase.TabIndex = 7;
			this.butCloseOrthoCase.Text = "Close Case";
			this.butCloseOrthoCase.Click += new System.EventHandler(this.ButCloseOrthoCase_Click);
			// 
			// butDeleteOrthoCase
			// 
			this.butDeleteOrthoCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeleteOrthoCase.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteOrthoCase.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteOrthoCase.Location = new System.Drawing.Point(8, 384);
			this.butDeleteOrthoCase.Name = "butDeleteOrthoCase";
			this.butDeleteOrthoCase.Size = new System.Drawing.Size(84, 24);
			this.butDeleteOrthoCase.TabIndex = 6;
			this.butDeleteOrthoCase.Text = "&Delete";
			this.butDeleteOrthoCase.Click += new System.EventHandler(this.ButDeleteOrthoCase_Click);
			// 
			// butDetachProcs
			// 
			this.butDetachProcs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDetachProcs.Location = new System.Drawing.Point(505, 384);
			this.butDetachProcs.Name = "butDetachProcs";
			this.butDetachProcs.Size = new System.Drawing.Size(114, 24);
			this.butDetachProcs.TabIndex = 8;
			this.butDetachProcs.Text = "Detach Selected";
			this.butDetachProcs.Click += new System.EventHandler(this.ButDetachProc_Click);
			// 
			// groupFee
			// 
			this.groupFee.Controls.Add(this.textSecondaryInsuranceFee);
			this.groupFee.Controls.Add(this.textSecondaryInsCompleted);
			this.groupFee.Controls.Add(this.textSecondaryInsAR);
			this.groupFee.Controls.Add(this.labelSecondaryInsuranceFee);
			this.groupFee.Controls.Add(this.textSecondaryInsRemaining);
			this.groupFee.Controls.Add(this.textTotalFee);
			this.groupFee.Controls.Add(this.labelTotalFee);
			this.groupFee.Controls.Add(this.textTotalCompleted);
			this.groupFee.Controls.Add(this.textTotalRemaining);
			this.groupFee.Controls.Add(this.textPrimaryInsuranceFee);
			this.groupFee.Controls.Add(this.textPatAR);
			this.groupFee.Controls.Add(this.textPrimaryInsCompleted);
			this.groupFee.Controls.Add(this.textPrimaryInsAR);
			this.groupFee.Controls.Add(this.labelPrimaryInsuranceFee);
			this.groupFee.Controls.Add(this.textTotalAR);
			this.groupFee.Controls.Add(this.textPrimaryInsRemaining);
			this.groupFee.Controls.Add(this.labelAR);
			this.groupFee.Controls.Add(this.textPatientFee);
			this.groupFee.Controls.Add(this.textPatCompleted);
			this.groupFee.Controls.Add(this.labelPatientFee);
			this.groupFee.Controls.Add(this.textPatRemaining);
			this.groupFee.Controls.Add(this.labelTotal);
			this.groupFee.Controls.Add(this.labelCompleted);
			this.groupFee.Controls.Add(this.labelRemaining);
			this.groupFee.Location = new System.Drawing.Point(9, 58);
			this.groupFee.Name = "groupFee";
			this.groupFee.Size = new System.Drawing.Size(489, 117);
			this.groupFee.TabIndex = 2;
			this.groupFee.TabStop = false;
			this.groupFee.Text = "Fee Details";
			// 
			// textSecondaryInsuranceFee
			// 
			this.textSecondaryInsuranceFee.Enabled = false;
			this.textSecondaryInsuranceFee.Location = new System.Drawing.Point(138, 92);
			this.textSecondaryInsuranceFee.MaxVal = 100000000D;
			this.textSecondaryInsuranceFee.MinVal = 0D;
			this.textSecondaryInsuranceFee.Name = "textSecondaryInsuranceFee";
			this.textSecondaryInsuranceFee.Size = new System.Drawing.Size(85, 20);
			this.textSecondaryInsuranceFee.TabIndex = 3;
			this.textSecondaryInsuranceFee.TextChanged += new System.EventHandler(this.TextSecondaryInsuranceFee_TextChanged);
			this.textSecondaryInsuranceFee.Leave += new System.EventHandler(this.TextSecondaryInsuranceFee_Leave);
			// 
			// textSecondaryInsCompleted
			// 
			this.textSecondaryInsCompleted.Enabled = false;
			this.textSecondaryInsCompleted.Location = new System.Drawing.Point(225, 92);
			this.textSecondaryInsCompleted.MaxVal = 100000000D;
			this.textSecondaryInsCompleted.MinVal = 0D;
			this.textSecondaryInsCompleted.Name = "textSecondaryInsCompleted";
			this.textSecondaryInsCompleted.Size = new System.Drawing.Size(85, 20);
			this.textSecondaryInsCompleted.TabIndex = 241;
			// 
			// textSecondaryInsAR
			// 
			this.textSecondaryInsAR.Enabled = false;
			this.textSecondaryInsAR.Location = new System.Drawing.Point(399, 92);
			this.textSecondaryInsAR.MaxVal = 100000000D;
			this.textSecondaryInsAR.MinVal = 0D;
			this.textSecondaryInsAR.Name = "textSecondaryInsAR";
			this.textSecondaryInsAR.Size = new System.Drawing.Size(85, 20);
			this.textSecondaryInsAR.TabIndex = 243;
			// 
			// labelSecondaryInsuranceFee
			// 
			this.labelSecondaryInsuranceFee.Location = new System.Drawing.Point(1, 94);
			this.labelSecondaryInsuranceFee.Name = "labelSecondaryInsuranceFee";
			this.labelSecondaryInsuranceFee.Size = new System.Drawing.Size(136, 17);
			this.labelSecondaryInsuranceFee.TabIndex = 240;
			this.labelSecondaryInsuranceFee.Text = "Secondary Insurance";
			this.labelSecondaryInsuranceFee.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSecondaryInsRemaining
			// 
			this.textSecondaryInsRemaining.Enabled = false;
			this.textSecondaryInsRemaining.Location = new System.Drawing.Point(312, 92);
			this.textSecondaryInsRemaining.MaxVal = 100000000D;
			this.textSecondaryInsRemaining.MinVal = 0D;
			this.textSecondaryInsRemaining.Name = "textSecondaryInsRemaining";
			this.textSecondaryInsRemaining.Size = new System.Drawing.Size(85, 20);
			this.textSecondaryInsRemaining.TabIndex = 242;
			// 
			// groupDates
			// 
			this.groupDates.Controls.Add(this.labelTreatmentLength);
			this.groupDates.Controls.Add(this.textBandingDate);
			this.groupDates.Controls.Add(this.labelExpectedDebondDate);
			this.groupDates.Controls.Add(this.textTreatmentLength);
			this.groupDates.Controls.Add(this.textExpectedDebondDate);
			this.groupDates.Controls.Add(this.labelBandingDate);
			this.groupDates.Location = new System.Drawing.Point(9, 286);
			this.groupDates.Name = "groupDates";
			this.groupDates.Size = new System.Drawing.Size(292, 81);
			this.groupDates.TabIndex = 5;
			this.groupDates.TabStop = false;
			this.groupDates.Text = "Dates";
			// 
			// groupProcBreakdown
			// 
			this.groupProcBreakdown.Controls.Add(this.textDebondAmount);
			this.groupProcBreakdown.Controls.Add(this.labelBandingFee);
			this.groupProcBreakdown.Controls.Add(this.labelDebondFee);
			this.groupProcBreakdown.Controls.Add(this.labelVisitsFees);
			this.groupProcBreakdown.Controls.Add(this.textAllVisitsPercent);
			this.groupProcBreakdown.Controls.Add(this.labelFeesPercent);
			this.groupProcBreakdown.Controls.Add(this.textBandingAmount);
			this.groupProcBreakdown.Controls.Add(this.textDebondPercent);
			this.groupProcBreakdown.Controls.Add(this.textAllVisitsAmount);
			this.groupProcBreakdown.Controls.Add(this.labelFeesAmount);
			this.groupProcBreakdown.Controls.Add(this.textBandingPercent);
			this.groupProcBreakdown.Location = new System.Drawing.Point(9, 178);
			this.groupProcBreakdown.Name = "groupProcBreakdown";
			this.groupProcBreakdown.Size = new System.Drawing.Size(292, 105);
			this.groupProcBreakdown.TabIndex = 3;
			this.groupProcBreakdown.TabStop = false;
			this.groupProcBreakdown.Text = "Procedure Breakdown";
			// 
			// labelBandingProc
			// 
			this.labelBandingProc.Location = new System.Drawing.Point(107, 15);
			this.labelBandingProc.Name = "labelBandingProc";
			this.labelBandingProc.Size = new System.Drawing.Size(125, 18);
			this.labelBandingProc.TabIndex = 239;
			this.labelBandingProc.Text = "Banding Procedure";
			this.labelBandingProc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPatPayPlan
			// 
			this.butPatPayPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPatPayPlan.Location = new System.Drawing.Point(716, 9);
			this.butPatPayPlan.Name = "butPatPayPlan";
			this.butPatPayPlan.Size = new System.Drawing.Size(147, 24);
			this.butPatPayPlan.TabIndex = 240;
			this.butPatPayPlan.Text = "Dynamic Payment Plan";
			this.butPatPayPlan.Click += new System.EventHandler(this.ButPatPayPlan_Click);
			// 
			// FormOrthoCase
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(874, 418);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butPatPayPlan);
			this.Controls.Add(this.labelBandingProc);
			this.Controls.Add(this.groupProcBreakdown);
			this.Controls.Add(this.groupDates);
			this.Controls.Add(this.groupFee);
			this.Controls.Add(this.butDetachProcs);
			this.Controls.Add(this.butDeleteOrthoCase);
			this.Controls.Add(this.butCloseOrthoCase);
			this.Controls.Add(this.gridOrthoSchedule);
			this.Controls.Add(this.groupVisit);
			this.Controls.Add(this.butAddBandingProcedure);
			this.Controls.Add(this.textBandingProc);
			this.Controls.Add(this.checkIsTransfer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoCase";
			this.Text = "Ortho Case";
			this.Load += new System.EventHandler(this.FormOrthoCase_Load);
			this.groupVisit.ResumeLayout(false);
			this.groupVisit.PerformLayout();
			this.groupFee.ResumeLayout(false);
			this.groupFee.PerformLayout();
			this.groupDates.ResumeLayout(false);
			this.groupDates.PerformLayout();
			this.groupProcBreakdown.ResumeLayout(false);
			this.groupProcBreakdown.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupVisit;
		private System.Windows.Forms.Label labelExpectedDebondDate;
		private ValidDate textBandingDate;
		private System.Windows.Forms.Label labelBandingDate;
		private System.Windows.Forms.Label labelTotalFee;
		private ValidDouble textTotalFee;
		private System.Windows.Forms.Label labelBandingFee;
		private System.Windows.Forms.Label labelDebondFee;
		private System.Windows.Forms.Label labelVisitsFees;
		private System.Windows.Forms.Label labelFeesAmount;
		private ValidDouble textAllVisitsAmount;
		private ValidDouble textDebondAmount;
		private ValidDouble textBandingAmount;
		private System.Windows.Forms.Label labelFeesPercent;
		private System.Windows.Forms.Label labelPatientFee;
		private ValidDouble textPatientFee;
		private System.Windows.Forms.Label labelPrimaryInsuranceFee;
		private ValidDouble textPrimaryInsuranceFee;
		private ValidDate textExpectedDebondDate;
		private ValidDouble textTotalCompleted;
		private ValidDouble textTotalRemaining;
		private ValidDouble textPrimaryInsRemaining;
		private ValidDouble textPrimaryInsCompleted;
		private ValidDouble textPatRemaining;
		private ValidDouble textPatCompleted;
		private System.Windows.Forms.Label labelRemaining;
		private System.Windows.Forms.Label labelCompleted;
		private System.Windows.Forms.Label labelTotal;
		private System.Windows.Forms.CheckBox checkIsTransfer;
		private ValidDouble textAllVisitsPercent;
		private ValidDouble textDebondPercent;
		private ValidDouble textBandingPercent;
		private System.Windows.Forms.Label labelVisitsPlanned;
		private ValidNum textVisitCountPlanned;
		private ValidDouble textVisitAmount;
		private System.Windows.Forms.Label labelVisitPercent;
		private System.Windows.Forms.Label labelVisitAmount;
		private UI.GridOD gridOrthoSchedule;
		private UI.Button butAddBandingProcedure;
		private System.Windows.Forms.TextBox textBandingProc;
		private UI.Button butCloseOrthoCase;
		private UI.Button butDeleteOrthoCase;
		private System.Windows.Forms.Label labelAR;
		private ValidDouble textPatAR;
		private ValidDouble textPrimaryInsAR;
		private ValidDouble textTotalAR;
		private ValidNum TextVisitCountComplete;
		private System.Windows.Forms.Label labelTreatmentLength;
		private ValidNum textTreatmentLength;
		private UI.Button butDetachProcs;
		private System.Windows.Forms.GroupBox groupFee;
		private System.Windows.Forms.GroupBox groupDates;
		private System.Windows.Forms.GroupBox groupProcBreakdown;
		private System.Windows.Forms.Label labelVisitsCompleted;
		private System.Windows.Forms.Label labelBandingProc;
		private ValidDouble textSecondaryInsuranceFee;
		private ValidDouble textSecondaryInsCompleted;
		private ValidDouble textSecondaryInsAR;
		private System.Windows.Forms.Label labelSecondaryInsuranceFee;
		private ValidDouble textSecondaryInsRemaining;
		private ValidDouble textVisitPercent;
		private UI.Button butPatPayPlan;
	}
}