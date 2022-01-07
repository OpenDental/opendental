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
			this.butCancel = new OpenDental.UI.Button();
			this.butRigorous = new OpenDental.UI.Button();
			this.labelIncomeTransferDateDesc = new System.Windows.Forms.Label();
			this.progressBarTransfer = new System.Windows.Forms.ProgressBar();
			this.timerProgress = new System.Windows.Forms.Timer(this.components);
			this.labelProgress = new System.Windows.Forms.Label();
			this.labelPayments = new System.Windows.Forms.Label();
			this.checkDeleteTransfers = new System.Windows.Forms.CheckBox();
			this.labelDeleteTransfers = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.butFIFO = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.labelAsOfDateDesc = new System.Windows.Forms.Label();
			this.datePickerAsOfDate = new System.Windows.Forms.DateTimePicker();
			this.labelAsOfDate = new System.Windows.Forms.Label();
			this.datePickerChangedSinceDate = new System.Windows.Forms.DateTimePicker();
			this.labelChangedSinceDate = new System.Windows.Forms.Label();
			this.checkChangedSinceDate = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// datePickerIncomeTransferDate
			// 
			this.datePickerIncomeTransferDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerIncomeTransferDate.Location = new System.Drawing.Point(160, 129);
			this.datePickerIncomeTransferDate.Name = "datePickerIncomeTransferDate";
			this.datePickerIncomeTransferDate.Size = new System.Drawing.Size(132, 20);
			this.datePickerIncomeTransferDate.TabIndex = 7;
			// 
			// labelIncomeTransferDate
			// 
			this.labelIncomeTransferDate.Location = new System.Drawing.Point(14, 132);
			this.labelIncomeTransferDate.Name = "labelIncomeTransferDate";
			this.labelIncomeTransferDate.Size = new System.Drawing.Size(140, 13);
			this.labelIncomeTransferDate.TabIndex = 134;
			this.labelIncomeTransferDate.Text = "Income Transfer Date";
			this.labelIncomeTransferDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(556, 456);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butRigorous
			// 
			this.butRigorous.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRigorous.Location = new System.Drawing.Point(12, 380);
			this.butRigorous.Name = "butRigorous";
			this.butRigorous.Size = new System.Drawing.Size(93, 26);
			this.butRigorous.TabIndex = 149;
			this.butRigorous.Text = "Start Rigorous";
			this.butRigorous.Click += new System.EventHandler(this.butRigorous_Click);
			// 
			// labelIncomeTransferDateDesc
			// 
			this.labelIncomeTransferDateDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelIncomeTransferDateDesc.Location = new System.Drawing.Point(298, 132);
			this.labelIncomeTransferDateDesc.Name = "labelIncomeTransferDateDesc";
			this.labelIncomeTransferDateDesc.Size = new System.Drawing.Size(331, 13);
			this.labelIncomeTransferDateDesc.TabIndex = 150;
			this.labelIncomeTransferDateDesc.Text = "Date set on all new payments and splits. Usually today\'s date.";
			this.labelIncomeTransferDateDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// progressBarTransfer
			// 
			this.progressBarTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarTransfer.Location = new System.Drawing.Point(17, 315);
			this.progressBarTransfer.Name = "progressBarTransfer";
			this.progressBarTransfer.Size = new System.Drawing.Size(612, 23);
			this.progressBarTransfer.TabIndex = 153;
			// 
			// timerProgress
			// 
			this.timerProgress.Tick += new System.EventHandler(this.timerProgress_Tick);
			// 
			// labelProgress
			// 
			this.labelProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelProgress.Location = new System.Drawing.Point(14, 341);
			this.labelProgress.Name = "labelProgress";
			this.labelProgress.Size = new System.Drawing.Size(176, 13);
			this.labelProgress.TabIndex = 154;
			this.labelProgress.Text = "labelProgress";
			this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPayments
			// 
			this.labelPayments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPayments.Location = new System.Drawing.Point(453, 341);
			this.labelPayments.Name = "labelPayments";
			this.labelPayments.Size = new System.Drawing.Size(176, 13);
			this.labelPayments.TabIndex = 155;
			this.labelPayments.Text = "labelPayments";
			this.labelPayments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkDeleteTransfers
			// 
			this.checkDeleteTransfers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkDeleteTransfers.Location = new System.Drawing.Point(160, 201);
			this.checkDeleteTransfers.Name = "checkDeleteTransfers";
			this.checkDeleteTransfers.Size = new System.Drawing.Size(469, 24);
			this.checkDeleteTransfers.TabIndex = 156;
			this.checkDeleteTransfers.Text = "Deletes all transfers, regardless of date.  But they must have PayType=0 and Amt=" +
    "0.";
			this.checkDeleteTransfers.UseVisualStyleBackColor = true;
			// 
			// labelDeleteTransfers
			// 
			this.labelDeleteTransfers.Location = new System.Drawing.Point(14, 205);
			this.labelDeleteTransfers.Name = "labelDeleteTransfers";
			this.labelDeleteTransfers.Size = new System.Drawing.Size(140, 13);
			this.labelDeleteTransfers.TabIndex = 157;
			this.labelDeleteTransfers.Text = "Delete All Transfers";
			this.labelDeleteTransfers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.ForeColor = System.Drawing.Color.Black;
			this.label4.Location = new System.Drawing.Point(10, 20);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(619, 56);
			this.label4.TabIndex = 158;
			this.label4.Text = resources.GetString("label4.Text");
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(111, 383);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(462, 18);
			this.label5.TabIndex = 159;
			this.label5.Text = "Allocates according to Line-Item Accounting standards.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(111, 424);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(462, 18);
			this.label6.TabIndex = 161;
			this.label6.Text = "Allocates on a FIFO basis without regard for provider or patient.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butFIFO
			// 
			this.butFIFO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butFIFO.Location = new System.Drawing.Point(12, 421);
			this.butFIFO.Name = "butFIFO";
			this.butFIFO.Size = new System.Drawing.Size(93, 26);
			this.butFIFO.TabIndex = 160;
			this.butFIFO.Text = "Start FIFO";
			this.butFIFO.Click += new System.EventHandler(this.butFIFO_Click);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.ForeColor = System.Drawing.Color.Black;
			this.label7.Location = new System.Drawing.Point(10, 88);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(621, 38);
			this.label7.TabIndex = 162;
			this.label7.Text = "This window is loosely password protected and is only intended to be run by the c" +
    "onversions department.  A similar thing can be done on a per-family basis in the" +
    " Income Transfer Manager.";
			// 
			// labelAsOfDateDesc
			// 
			this.labelAsOfDateDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAsOfDateDesc.Location = new System.Drawing.Point(298, 154);
			this.labelAsOfDateDesc.Name = "labelAsOfDateDesc";
			this.labelAsOfDateDesc.Size = new System.Drawing.Size(331, 43);
			this.labelAsOfDateDesc.TabIndex = 166;
			this.labelAsOfDateDesc.Text = "Account entries after this date are ignored during transfers.\r\nFor conversions, s" +
    "et this to the date of the conversion.\r\nFor bi-weekly usage, use today\'s date.";
			this.labelAsOfDateDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// datePickerAsOfDate
			// 
			this.datePickerAsOfDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerAsOfDate.Location = new System.Drawing.Point(160, 166);
			this.datePickerAsOfDate.Name = "datePickerAsOfDate";
			this.datePickerAsOfDate.Size = new System.Drawing.Size(132, 20);
			this.datePickerAsOfDate.TabIndex = 164;
			// 
			// labelAsOfDate
			// 
			this.labelAsOfDate.Location = new System.Drawing.Point(14, 169);
			this.labelAsOfDate.Name = "labelAsOfDate";
			this.labelAsOfDate.Size = new System.Drawing.Size(140, 13);
			this.labelAsOfDate.TabIndex = 165;
			this.labelAsOfDate.Text = "As of Date";
			this.labelAsOfDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// datePickerChangedSinceDate
			// 
			this.datePickerChangedSinceDate.Enabled = false;
			this.datePickerChangedSinceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickerChangedSinceDate.Location = new System.Drawing.Point(160, 278);
			this.datePickerChangedSinceDate.Name = "datePickerChangedSinceDate";
			this.datePickerChangedSinceDate.Size = new System.Drawing.Size(132, 20);
			this.datePickerChangedSinceDate.TabIndex = 167;
			// 
			// labelChangedSinceDate
			// 
			this.labelChangedSinceDate.Enabled = false;
			this.labelChangedSinceDate.Location = new System.Drawing.Point(14, 281);
			this.labelChangedSinceDate.Name = "labelChangedSinceDate";
			this.labelChangedSinceDate.Size = new System.Drawing.Size(140, 13);
			this.labelChangedSinceDate.TabIndex = 168;
			this.labelChangedSinceDate.Text = "Changed Since Date";
			this.labelChangedSinceDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkChangedSinceDate
			// 
			this.checkChangedSinceDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkChangedSinceDate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkChangedSinceDate.Location = new System.Drawing.Point(160, 227);
			this.checkChangedSinceDate.Name = "checkChangedSinceDate";
			this.checkChangedSinceDate.Size = new System.Drawing.Size(469, 45);
			this.checkChangedSinceDate.TabIndex = 170;
			this.checkChangedSinceDate.Text = resources.GetString("checkChangedSinceDate.Text");
			this.checkChangedSinceDate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkChangedSinceDate.UseVisualStyleBackColor = true;
			this.checkChangedSinceDate.CheckedChanged += new System.EventHandler(this.checkChangedSinceDate_CheckedChanged);
			// 
			// FormFamilyBalancer
			// 
			this.ClientSize = new System.Drawing.Size(641, 493);
			this.Controls.Add(this.checkChangedSinceDate);
			this.Controls.Add(this.datePickerChangedSinceDate);
			this.Controls.Add(this.labelChangedSinceDate);
			this.Controls.Add(this.labelAsOfDateDesc);
			this.Controls.Add(this.datePickerAsOfDate);
			this.Controls.Add(this.labelAsOfDate);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butFIFO);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelDeleteTransfers);
			this.Controls.Add(this.checkDeleteTransfers);
			this.Controls.Add(this.labelPayments);
			this.Controls.Add(this.labelProgress);
			this.Controls.Add(this.progressBarTransfer);
			this.Controls.Add(this.labelIncomeTransferDateDesc);
			this.Controls.Add(this.datePickerIncomeTransferDate);
			this.Controls.Add(this.labelIncomeTransferDate);
			this.Controls.Add(this.butRigorous);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFamilyBalancer";
			this.Text = "Family Balancer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFamilyBalancer_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butCancel;
		private System.Windows.Forms.DateTimePicker datePickerIncomeTransferDate;
		private System.Windows.Forms.Label labelIncomeTransferDate;
		private UI.Button butRigorous;
		private System.Windows.Forms.Label labelIncomeTransferDateDesc;
		private System.Windows.Forms.ProgressBar progressBarTransfer;
		private System.Windows.Forms.Timer timerProgress;
		private System.Windows.Forms.Label labelProgress;
		private System.Windows.Forms.Label labelPayments;
		private System.Windows.Forms.CheckBox checkDeleteTransfers;
		private System.Windows.Forms.Label labelDeleteTransfers;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private UI.Button butFIFO;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelAsOfDateDesc;
		private System.Windows.Forms.DateTimePicker datePickerAsOfDate;
		private System.Windows.Forms.Label labelAsOfDate;
		private System.Windows.Forms.DateTimePicker datePickerChangedSinceDate;
		private System.Windows.Forms.Label labelChangedSinceDate;
		private System.Windows.Forms.CheckBox checkChangedSinceDate;
	}
}