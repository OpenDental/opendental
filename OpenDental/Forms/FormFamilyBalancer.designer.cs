namespace OpenDental{
	partial class FormFamilyBalancer {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFamilyBalancer));
			this.datePickerIncomeTransferDate = new System.Windows.Forms.DateTimePicker();
			this.labelIncomeTransferDate = new System.Windows.Forms.Label();
			this.butRigorous = new OpenDental.UI.Button();
			this.labelIncomeTransferDateDesc = new System.Windows.Forms.Label();
			this.progressBarTransfer = new System.Windows.Forms.ProgressBar();
			this.timerProgress = new System.Windows.Forms.Timer(this.components);
			this.labelProgress = new System.Windows.Forms.Label();
			this.labelPayments = new System.Windows.Forms.Label();
			this.checkDeleteTransfers = new OpenDental.UI.CheckBox();
			this.labelDeleteTransfers = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.butFIFO = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.labelAsOfDateDesc = new System.Windows.Forms.Label();
			this.datePickerAsOfDate = new System.Windows.Forms.DateTimePicker();
			this.labelAsOfDate = new System.Windows.Forms.Label();
			this.groupBoxIncomeTransfers = new OpenDental.UI.GroupBox();
			this.butConfigureAuto = new OpenDental.UI.Button();
			this.checkChangedSinceDate = new OpenDental.UI.CheckBox();
			this.datePickerChangedSinceDate = new System.Windows.Forms.DateTimePicker();
			this.labelChangedSinceDate = new System.Windows.Forms.Label();
			this.groupBoxRecreateSplits = new OpenDental.UI.GroupBox();
			this.labelAsOfDateDescRecreate = new System.Windows.Forms.Label();
			this.butRecreate = new OpenDental.UI.Button();
			this.datePickerAsOfDateRecreate = new System.Windows.Forms.DateTimePicker();
			this.labelAsOfDateRecreate = new System.Windows.Forms.Label();
			this.checkDeleteTransfersRecreate = new OpenDental.UI.CheckBox();
			this.labelDeleteTransfersRecreate = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.textOutput = new System.Windows.Forms.TextBox();
			this.checkOutputVerbose = new OpenDental.UI.CheckBox();
			this.timerOutput = new System.Windows.Forms.Timer(this.components);
			this.butLog = new OpenDental.UI.Button();
			this.groupBoxInsuranceOverpayments = new OpenDental.UI.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label56 = new System.Windows.Forms.Label();
			this.comboOverpayPayType = new OpenDental.UI.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.datePickerOverpayPayDate = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.butOverpay = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxIncomeTransfers.SuspendLayout();
			this.groupBoxRecreateSplits.SuspendLayout();
			this.groupBoxInsuranceOverpayments.SuspendLayout();
			this.SuspendLayout();
			// 
			// datePickerIncomeTransferDate
			// 
			this.datePickerIncomeTransferDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerIncomeTransferDate.Location = new System.Drawing.Point(140, 137);
			this.datePickerIncomeTransferDate.Name = "datePickerIncomeTransferDate";
			this.datePickerIncomeTransferDate.Size = new System.Drawing.Size(132, 20);
			this.datePickerIncomeTransferDate.TabIndex = 0;
			// 
			// labelIncomeTransferDate
			// 
			this.labelIncomeTransferDate.Location = new System.Drawing.Point(14, 140);
			this.labelIncomeTransferDate.Name = "labelIncomeTransferDate";
			this.labelIncomeTransferDate.Size = new System.Drawing.Size(120, 13);
			this.labelIncomeTransferDate.TabIndex = 134;
			this.labelIncomeTransferDate.Text = "Income Transfer Date";
			this.labelIncomeTransferDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRigorous
			// 
			this.butRigorous.Location = new System.Drawing.Point(11, 323);
			this.butRigorous.Name = "butRigorous";
			this.butRigorous.Size = new System.Drawing.Size(93, 26);
			this.butRigorous.TabIndex = 5;
			this.butRigorous.Text = "Start Rigorous";
			this.butRigorous.Click += new System.EventHandler(this.butRigorous_Click);
			// 
			// labelIncomeTransferDateDesc
			// 
			this.labelIncomeTransferDateDesc.Location = new System.Drawing.Point(278, 140);
			this.labelIncomeTransferDateDesc.Name = "labelIncomeTransferDateDesc";
			this.labelIncomeTransferDateDesc.Size = new System.Drawing.Size(304, 13);
			this.labelIncomeTransferDateDesc.TabIndex = 150;
			this.labelIncomeTransferDateDesc.Text = "Date set on all new payments and splits. Usually today\'s date.";
			this.labelIncomeTransferDateDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// progressBarTransfer
			// 
			this.progressBarTransfer.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.progressBarTransfer.Location = new System.Drawing.Point(12, 424);
			this.progressBarTransfer.Name = "progressBarTransfer";
			this.progressBarTransfer.Size = new System.Drawing.Size(1186, 22);
			this.progressBarTransfer.TabIndex = 153;
			// 
			// timerProgress
			// 
			this.timerProgress.Tick += new System.EventHandler(this.timerProgress_Tick);
			// 
			// labelProgress
			// 
			this.labelProgress.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelProgress.Location = new System.Drawing.Point(12, 409);
			this.labelProgress.Name = "labelProgress";
			this.labelProgress.Size = new System.Drawing.Size(551, 13);
			this.labelProgress.TabIndex = 154;
			this.labelProgress.Text = "labelProgress";
			this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPayments
			// 
			this.labelPayments.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelPayments.Location = new System.Drawing.Point(730, 408);
			this.labelPayments.Name = "labelPayments";
			this.labelPayments.Size = new System.Drawing.Size(468, 13);
			this.labelPayments.TabIndex = 155;
			this.labelPayments.Text = "labelPayments";
			this.labelPayments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkDeleteTransfers
			// 
			this.checkDeleteTransfers.Location = new System.Drawing.Point(140, 210);
			this.checkDeleteTransfers.Name = "checkDeleteTransfers";
			this.checkDeleteTransfers.Size = new System.Drawing.Size(442, 24);
			this.checkDeleteTransfers.TabIndex = 2;
			this.checkDeleteTransfers.Text = "Deletes all transfers, regardless of date.  But they must have PayType=0 and Amt=" +
    "0.";
			// 
			// labelDeleteTransfers
			// 
			this.labelDeleteTransfers.Location = new System.Drawing.Point(14, 214);
			this.labelDeleteTransfers.Name = "labelDeleteTransfers";
			this.labelDeleteTransfers.Size = new System.Drawing.Size(120, 13);
			this.labelDeleteTransfers.TabIndex = 157;
			this.labelDeleteTransfers.Text = "Delete All Transfers";
			this.labelDeleteTransfers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 28);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(574, 56);
			this.label4.TabIndex = 158;
			this.label4.Text = resources.GetString("label4.Text");
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(110, 326);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(341, 18);
			this.label5.TabIndex = 159;
			this.label5.Text = "Allocates according to Line-Item Accounting standards.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(110, 358);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(341, 18);
			this.label6.TabIndex = 161;
			this.label6.Text = "Allocates on a FIFO basis without regard for provider or patient.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butFIFO
			// 
			this.butFIFO.Location = new System.Drawing.Point(11, 355);
			this.butFIFO.Name = "butFIFO";
			this.butFIFO.Size = new System.Drawing.Size(93, 26);
			this.butFIFO.TabIndex = 6;
			this.butFIFO.Text = "Start FIFO";
			this.butFIFO.Click += new System.EventHandler(this.butFIFO_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 96);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(574, 38);
			this.label7.TabIndex = 162;
			this.label7.Text = "This window is loosely password protected and is only intended to be run by the c" +
    "onversions department.  A similar thing can be done on a per-family basis in the" +
    " Income Transfer Manager.";
			// 
			// labelAsOfDateDesc
			// 
			this.labelAsOfDateDesc.Location = new System.Drawing.Point(278, 163);
			this.labelAsOfDateDesc.Name = "labelAsOfDateDesc";
			this.labelAsOfDateDesc.Size = new System.Drawing.Size(304, 43);
			this.labelAsOfDateDesc.TabIndex = 166;
			this.labelAsOfDateDesc.Text = "Account entries after this date are ignored during transfers.\r\nFor conversions, s" +
    "et this to the date of the conversion.\r\nFor bi-weekly usage, use today\'s date.";
			this.labelAsOfDateDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// datePickerAsOfDate
			// 
			this.datePickerAsOfDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerAsOfDate.Location = new System.Drawing.Point(140, 174);
			this.datePickerAsOfDate.Name = "datePickerAsOfDate";
			this.datePickerAsOfDate.Size = new System.Drawing.Size(132, 20);
			this.datePickerAsOfDate.TabIndex = 1;
			// 
			// labelAsOfDate
			// 
			this.labelAsOfDate.Location = new System.Drawing.Point(14, 177);
			this.labelAsOfDate.Name = "labelAsOfDate";
			this.labelAsOfDate.Size = new System.Drawing.Size(120, 13);
			this.labelAsOfDate.TabIndex = 165;
			this.labelAsOfDate.Text = "As of Date";
			this.labelAsOfDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxIncomeTransfers
			// 
			this.groupBoxIncomeTransfers.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBoxIncomeTransfers.Controls.Add(this.butConfigureAuto);
			this.groupBoxIncomeTransfers.Controls.Add(this.checkChangedSinceDate);
			this.groupBoxIncomeTransfers.Controls.Add(this.datePickerChangedSinceDate);
			this.groupBoxIncomeTransfers.Controls.Add(this.labelChangedSinceDate);
			this.groupBoxIncomeTransfers.Controls.Add(this.label4);
			this.groupBoxIncomeTransfers.Controls.Add(this.labelAsOfDateDesc);
			this.groupBoxIncomeTransfers.Controls.Add(this.butRigorous);
			this.groupBoxIncomeTransfers.Controls.Add(this.datePickerAsOfDate);
			this.groupBoxIncomeTransfers.Controls.Add(this.labelIncomeTransferDate);
			this.groupBoxIncomeTransfers.Controls.Add(this.labelAsOfDate);
			this.groupBoxIncomeTransfers.Controls.Add(this.datePickerIncomeTransferDate);
			this.groupBoxIncomeTransfers.Controls.Add(this.labelIncomeTransferDateDesc);
			this.groupBoxIncomeTransfers.Controls.Add(this.label7);
			this.groupBoxIncomeTransfers.Controls.Add(this.label6);
			this.groupBoxIncomeTransfers.Controls.Add(this.butFIFO);
			this.groupBoxIncomeTransfers.Controls.Add(this.label5);
			this.groupBoxIncomeTransfers.Controls.Add(this.checkDeleteTransfers);
			this.groupBoxIncomeTransfers.Controls.Add(this.labelDeleteTransfers);
			this.groupBoxIncomeTransfers.Location = new System.Drawing.Point(12, 12);
			this.groupBoxIncomeTransfers.Name = "groupBoxIncomeTransfers";
			this.groupBoxIncomeTransfers.Size = new System.Drawing.Size(590, 393);
			this.groupBoxIncomeTransfers.TabIndex = 0;
			this.groupBoxIncomeTransfers.Text = "Income Transfers";
			// 
			// butConfigureAuto
			// 
			this.butConfigureAuto.Location = new System.Drawing.Point(457, 355);
			this.butConfigureAuto.Name = "butConfigureAuto";
			this.butConfigureAuto.Size = new System.Drawing.Size(125, 26);
			this.butConfigureAuto.TabIndex = 173;
			this.butConfigureAuto.Text = "Configure Automation";
			this.butConfigureAuto.Click += new System.EventHandler(this.butConfigureAuto_Click);
			// 
			// checkChangedSinceDate
			// 
			this.checkChangedSinceDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkChangedSinceDate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkChangedSinceDate.Location = new System.Drawing.Point(140, 236);
			this.checkChangedSinceDate.Name = "checkChangedSinceDate";
			this.checkChangedSinceDate.Size = new System.Drawing.Size(442, 45);
			this.checkChangedSinceDate.TabIndex = 3;
			this.checkChangedSinceDate.Text = resources.GetString("checkChangedSinceDate.Text");
			this.checkChangedSinceDate.CheckedChanged += new System.EventHandler(this.checkChangedSinceDate_CheckedChanged);
			// 
			// datePickerChangedSinceDate
			// 
			this.datePickerChangedSinceDate.Enabled = false;
			this.datePickerChangedSinceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerChangedSinceDate.Location = new System.Drawing.Point(140, 287);
			this.datePickerChangedSinceDate.Name = "datePickerChangedSinceDate";
			this.datePickerChangedSinceDate.Size = new System.Drawing.Size(132, 20);
			this.datePickerChangedSinceDate.TabIndex = 4;
			// 
			// labelChangedSinceDate
			// 
			this.labelChangedSinceDate.Enabled = false;
			this.labelChangedSinceDate.Location = new System.Drawing.Point(14, 290);
			this.labelChangedSinceDate.Name = "labelChangedSinceDate";
			this.labelChangedSinceDate.Size = new System.Drawing.Size(120, 13);
			this.labelChangedSinceDate.TabIndex = 172;
			this.labelChangedSinceDate.Text = "Changed Since Date";
			this.labelChangedSinceDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxRecreateSplits
			// 
			this.groupBoxRecreateSplits.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBoxRecreateSplits.Controls.Add(this.labelAsOfDateDescRecreate);
			this.groupBoxRecreateSplits.Controls.Add(this.butRecreate);
			this.groupBoxRecreateSplits.Controls.Add(this.datePickerAsOfDateRecreate);
			this.groupBoxRecreateSplits.Controls.Add(this.labelAsOfDateRecreate);
			this.groupBoxRecreateSplits.Controls.Add(this.checkDeleteTransfersRecreate);
			this.groupBoxRecreateSplits.Controls.Add(this.labelDeleteTransfersRecreate);
			this.groupBoxRecreateSplits.Controls.Add(this.label11);
			this.groupBoxRecreateSplits.Location = new System.Drawing.Point(608, 12);
			this.groupBoxRecreateSplits.Name = "groupBoxRecreateSplits";
			this.groupBoxRecreateSplits.Size = new System.Drawing.Size(590, 190);
			this.groupBoxRecreateSplits.TabIndex = 1;
			this.groupBoxRecreateSplits.Text = "Recreate Splits";
			// 
			// labelAsOfDateDescRecreate
			// 
			this.labelAsOfDateDescRecreate.Location = new System.Drawing.Point(314, 91);
			this.labelAsOfDateDescRecreate.Name = "labelAsOfDateDescRecreate";
			this.labelAsOfDateDescRecreate.Size = new System.Drawing.Size(214, 13);
			this.labelAsOfDateDescRecreate.TabIndex = 174;
			this.labelAsOfDateDescRecreate.Text = "Account entries after this date are ignored.";
			this.labelAsOfDateDescRecreate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butRecreate
			// 
			this.butRecreate.Location = new System.Drawing.Point(249, 151);
			this.butRecreate.Name = "butRecreate";
			this.butRecreate.Size = new System.Drawing.Size(93, 26);
			this.butRecreate.TabIndex = 2;
			this.butRecreate.Text = "Start Recreate";
			this.butRecreate.Click += new System.EventHandler(this.butRecreate_Click);
			// 
			// datePickerAsOfDateRecreate
			// 
			this.datePickerAsOfDateRecreate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerAsOfDateRecreate.Location = new System.Drawing.Point(176, 88);
			this.datePickerAsOfDateRecreate.Name = "datePickerAsOfDateRecreate";
			this.datePickerAsOfDateRecreate.Size = new System.Drawing.Size(132, 20);
			this.datePickerAsOfDateRecreate.TabIndex = 0;
			// 
			// labelAsOfDateRecreate
			// 
			this.labelAsOfDateRecreate.Location = new System.Drawing.Point(63, 91);
			this.labelAsOfDateRecreate.Name = "labelAsOfDateRecreate";
			this.labelAsOfDateRecreate.Size = new System.Drawing.Size(107, 13);
			this.labelAsOfDateRecreate.TabIndex = 173;
			this.labelAsOfDateRecreate.Text = "As of Date";
			this.labelAsOfDateRecreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkDeleteTransfersRecreate
			// 
			this.checkDeleteTransfersRecreate.Location = new System.Drawing.Point(176, 114);
			this.checkDeleteTransfersRecreate.Name = "checkDeleteTransfersRecreate";
			this.checkDeleteTransfersRecreate.Size = new System.Drawing.Size(352, 24);
			this.checkDeleteTransfersRecreate.TabIndex = 1;
			this.checkDeleteTransfersRecreate.Text = "Deletes all transfers regardless of Payment Date or Payment Type.";
			// 
			// labelDeleteTransfersRecreate
			// 
			this.labelDeleteTransfersRecreate.Location = new System.Drawing.Point(63, 118);
			this.labelDeleteTransfersRecreate.Name = "labelDeleteTransfersRecreate";
			this.labelDeleteTransfersRecreate.Size = new System.Drawing.Size(107, 13);
			this.labelDeleteTransfersRecreate.TabIndex = 169;
			this.labelDeleteTransfersRecreate.Text = "Delete All Transfers";
			this.labelDeleteTransfersRecreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(8, 28);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(574, 56);
			this.label11.TabIndex = 159;
			this.label11.Text = resources.GetString("label11.Text");
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(12, 449);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(551, 13);
			this.label15.TabIndex = 169;
			this.label15.Text = "Output";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textOutput
			// 
			this.textOutput.AcceptsTab = true;
			this.textOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textOutput.BackColor = System.Drawing.SystemColors.Control;
			this.textOutput.Location = new System.Drawing.Point(12, 465);
			this.textOutput.Multiline = true;
			this.textOutput.Name = "textOutput";
			this.textOutput.ReadOnly = true;
			this.textOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textOutput.Size = new System.Drawing.Size(1186, 152);
			this.textOutput.TabIndex = 170;
			// 
			// checkOutputVerbose
			// 
			this.checkOutputVerbose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkOutputVerbose.Location = new System.Drawing.Point(15, 623);
			this.checkOutputVerbose.Name = "checkOutputVerbose";
			this.checkOutputVerbose.Size = new System.Drawing.Size(292, 24);
			this.checkOutputVerbose.TabIndex = 3;
			this.checkOutputVerbose.Text = "Verbose";
			// 
			// timerOutput
			// 
			this.timerOutput.Enabled = true;
			this.timerOutput.Interval = 500;
			this.timerOutput.Tick += new System.EventHandler(this.timerOutput_Tick);
			// 
			// butLog
			// 
			this.butLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butLog.Location = new System.Drawing.Point(567, 623);
			this.butLog.Name = "butLog";
			this.butLog.Size = new System.Drawing.Size(75, 26);
			this.butLog.TabIndex = 4;
			this.butLog.Text = "Log";
			this.butLog.Click += new System.EventHandler(this.butLog_Click);
			// 
			// groupBoxInsuranceOverpayments
			// 
			this.groupBoxInsuranceOverpayments.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBoxInsuranceOverpayments.Controls.Add(this.label8);
			this.groupBoxInsuranceOverpayments.Controls.Add(this.label56);
			this.groupBoxInsuranceOverpayments.Controls.Add(this.comboOverpayPayType);
			this.groupBoxInsuranceOverpayments.Controls.Add(this.label2);
			this.groupBoxInsuranceOverpayments.Controls.Add(this.datePickerOverpayPayDate);
			this.groupBoxInsuranceOverpayments.Controls.Add(this.label3);
			this.groupBoxInsuranceOverpayments.Controls.Add(this.butOverpay);
			this.groupBoxInsuranceOverpayments.Controls.Add(this.label1);
			this.groupBoxInsuranceOverpayments.Location = new System.Drawing.Point(608, 208);
			this.groupBoxInsuranceOverpayments.Name = "groupBoxInsuranceOverpayments";
			this.groupBoxInsuranceOverpayments.Size = new System.Drawing.Size(590, 197);
			this.groupBoxInsuranceOverpayments.TabIndex = 2;
			this.groupBoxInsuranceOverpayments.Text = "Insurance Overpayments";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(314, 119);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(210, 13);
			this.label8.TabIndex = 256;
			this.label8.Text = "Type set on all new payments.";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label56
			// 
			this.label56.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label56.Location = new System.Drawing.Point(53, 119);
			this.label56.Name = "label56";
			this.label56.Size = new System.Drawing.Size(117, 13);
			this.label56.TabIndex = 255;
			this.label56.Text = "Pay Type";
			this.label56.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboOverpayPayType
			// 
			this.comboOverpayPayType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboOverpayPayType.Location = new System.Drawing.Point(176, 115);
			this.comboOverpayPayType.Name = "comboOverpayPayType";
			this.comboOverpayPayType.Size = new System.Drawing.Size(132, 21);
			this.comboOverpayPayType.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(56, 90);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(118, 13);
			this.label2.TabIndex = 163;
			this.label2.Text = "Pay Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// datePickerOverpayPayDate
			// 
			this.datePickerOverpayPayDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerOverpayPayDate.Location = new System.Drawing.Point(176, 87);
			this.datePickerOverpayPayDate.Name = "datePickerOverpayPayDate";
			this.datePickerOverpayPayDate.Size = new System.Drawing.Size(132, 20);
			this.datePickerOverpayPayDate.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(314, 90);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(210, 13);
			this.label3.TabIndex = 164;
			this.label3.Text = "Date set on all new payments and splits.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butOverpay
			// 
			this.butOverpay.Location = new System.Drawing.Point(249, 151);
			this.butOverpay.Name = "butOverpay";
			this.butOverpay.Size = new System.Drawing.Size(93, 26);
			this.butOverpay.TabIndex = 2;
			this.butOverpay.Text = "Start Overpay";
			this.butOverpay.Click += new System.EventHandler(this.butOverpay_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(574, 56);
			this.label1.TabIndex = 160;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// FormFamilyBalancer
			// 
			this.ClientSize = new System.Drawing.Size(1208, 661);
			this.Controls.Add(this.groupBoxInsuranceOverpayments);
			this.Controls.Add(this.butLog);
			this.Controls.Add(this.checkOutputVerbose);
			this.Controls.Add(this.textOutput);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.groupBoxRecreateSplits);
			this.Controls.Add(this.groupBoxIncomeTransfers);
			this.Controls.Add(this.progressBarTransfer);
			this.Controls.Add(this.labelPayments);
			this.Controls.Add(this.labelProgress);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFamilyBalancer";
			this.Text = "Family Balancer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFamilyBalancer_FormClosing);
			this.Load += new System.EventHandler(this.FormFamilyBalancer_Load);
			this.groupBoxIncomeTransfers.ResumeLayout(false);
			this.groupBoxRecreateSplits.ResumeLayout(false);
			this.groupBoxInsuranceOverpayments.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.DateTimePicker datePickerIncomeTransferDate;
		private System.Windows.Forms.Label labelIncomeTransferDate;
		private UI.Button butRigorous;
		private System.Windows.Forms.Label labelIncomeTransferDateDesc;
		private System.Windows.Forms.ProgressBar progressBarTransfer;
		private System.Windows.Forms.Timer timerProgress;
		private System.Windows.Forms.Label labelProgress;
		private System.Windows.Forms.Label labelPayments;
		private OpenDental.UI.CheckBox checkDeleteTransfers;
		private System.Windows.Forms.Label labelDeleteTransfers;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private UI.Button butFIFO;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelAsOfDateDesc;
		private System.Windows.Forms.DateTimePicker datePickerAsOfDate;
		private System.Windows.Forms.Label labelAsOfDate;
		private UI.GroupBox groupBoxIncomeTransfers;
		private UI.GroupBox groupBoxRecreateSplits;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label labelAsOfDateDescRecreate;
		private UI.Button butRecreate;
		private System.Windows.Forms.DateTimePicker datePickerAsOfDateRecreate;
		private System.Windows.Forms.Label labelAsOfDateRecreate;
		private OpenDental.UI.CheckBox checkDeleteTransfersRecreate;
		private System.Windows.Forms.Label labelDeleteTransfersRecreate;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textOutput;
		private OpenDental.UI.CheckBox checkOutputVerbose;
		private System.Windows.Forms.Timer timerOutput;
		private UI.Button butLog;
		private OpenDental.UI.CheckBox checkChangedSinceDate;
		private System.Windows.Forms.DateTimePicker datePickerChangedSinceDate;
		private System.Windows.Forms.Label labelChangedSinceDate;
		private UI.GroupBox groupBoxInsuranceOverpayments;
		private System.Windows.Forms.Label label1;
		private UI.Button butOverpay;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker datePickerOverpayPayDate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label56;
		private UI.ComboBox comboOverpayPayType;
		private System.Windows.Forms.Label label8;
		private UI.Button butConfigureAuto;
	}
}