namespace OpenDental{
	partial class FormEServicesEConnector {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesEConnector));
			this.label37 = new System.Windows.Forms.Label();
			this.menuWebSchedVerifyTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.insertReplacementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.butOk = new OpenDental.UI.Button();
			this.label25 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textLogCleanupInterval = new OpenDental.ValidNum();
			this.labelIntervalDays = new System.Windows.Forms.Label();
			this.checkEmailsWithDiffProcess = new System.Windows.Forms.CheckBox();
			this.labelInstallWarning = new System.Windows.Forms.Label();
			this.butInstallEConnector = new OpenDental.UI.Button();
			this.labelListenerServiceAck = new System.Windows.Forms.Label();
			this.butListenerServiceAck = new OpenDental.UI.Button();
			this.label27 = new System.Windows.Forms.Label();
			this.butListenerServiceHistoryRefresh = new OpenDental.UI.Button();
			this.label26 = new System.Windows.Forms.Label();
			this.gridListenerServiceStatusHistory = new OpenDental.UI.GridOD();
			this.butStartListenerService = new OpenDental.UI.Button();
			this.label24 = new System.Windows.Forms.Label();
			this.labelListenerStatus = new System.Windows.Forms.Label();
			this.butListenerAlertsOff = new OpenDental.UI.Button();
			this.textListenerServiceStatus = new System.Windows.Forms.TextBox();
			this.butCancel = new OpenDental.UI.Button();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(0, 0);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(100, 23);
			this.label37.TabIndex = 0;
			// 
			// menuWebSchedVerifyTextTemplate
			// 
			this.menuWebSchedVerifyTextTemplate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertReplacementsToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.selectAllToolStripMenuItem});
			this.menuWebSchedVerifyTextTemplate.Name = "menuASAPEmailBody";
			this.menuWebSchedVerifyTextTemplate.Size = new System.Drawing.Size(137, 136);
			this.menuWebSchedVerifyTextTemplate.Text = "Insert Replacements";
			// 
			// insertReplacementsToolStripMenuItem
			// 
			this.insertReplacementsToolStripMenuItem.Name = "insertReplacementsToolStripMenuItem";
			this.insertReplacementsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.insertReplacementsToolStripMenuItem.Text = "Insert Fields";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.selectAllToolStripMenuItem.Text = "Select All";
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(834, 532);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 23);
			this.butOk.TabIndex = 500;
			this.butOk.Text = "OK";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(12, 9);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(932, 68);
			this.label25.TabIndex = 251;
			this.label25.Text = resources.GetString("label25.Text");
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.groupBox1);
			this.groupBox3.Controls.Add(this.checkEmailsWithDiffProcess);
			this.groupBox3.Controls.Add(this.labelInstallWarning);
			this.groupBox3.Controls.Add(this.butInstallEConnector);
			this.groupBox3.Controls.Add(this.labelListenerServiceAck);
			this.groupBox3.Controls.Add(this.butListenerServiceAck);
			this.groupBox3.Controls.Add(this.label27);
			this.groupBox3.Controls.Add(this.butListenerServiceHistoryRefresh);
			this.groupBox3.Controls.Add(this.label26);
			this.groupBox3.Controls.Add(this.gridListenerServiceStatusHistory);
			this.groupBox3.Controls.Add(this.butStartListenerService);
			this.groupBox3.Controls.Add(this.label24);
			this.groupBox3.Controls.Add(this.labelListenerStatus);
			this.groupBox3.Controls.Add(this.butListenerAlertsOff);
			this.groupBox3.Controls.Add(this.textListenerServiceStatus);
			this.groupBox3.Location = new System.Drawing.Point(15, 80);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(975, 447);
			this.groupBox3.TabIndex = 249;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "eConnector Service Monitor";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.textLogCleanupInterval);
			this.groupBox1.Controls.Add(this.labelIntervalDays);
			this.groupBox1.Location = new System.Drawing.Point(9, 360);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(286, 45);
			this.groupBox1.TabIndex = 502;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Delete logs older than";
			// 
			// textLogCleanupInterval
			// 
			this.textLogCleanupInterval.AcceptsTab = true;
			this.textLogCleanupInterval.BackColor = System.Drawing.SystemColors.Window;
			this.textLogCleanupInterval.Location = new System.Drawing.Point(10, 16);
			this.textLogCleanupInterval.MaxVal = 36500;
			this.textLogCleanupInterval.MinVal = 0;
			this.textLogCleanupInterval.Name = "textLogCleanupInterval";
			this.textLogCleanupInterval.Size = new System.Drawing.Size(42, 20);
			this.textLogCleanupInterval.TabIndex = 258;
			this.textLogCleanupInterval.ShowZero = false;
			// 
			// labelIntervalDays
			// 
			this.labelIntervalDays.Location = new System.Drawing.Point(55, 16);
			this.labelIntervalDays.Name = "labelIntervalDays";
			this.labelIntervalDays.Size = new System.Drawing.Size(200, 17);
			this.labelIntervalDays.TabIndex = 259;
			this.labelIntervalDays.Text = " days (0 or blank to disable)";
			this.labelIntervalDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkEmailsWithDiffProcess
			// 
			this.checkEmailsWithDiffProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkEmailsWithDiffProcess.AutoSize = true;
			this.checkEmailsWithDiffProcess.Location = new System.Drawing.Point(9, 339);
			this.checkEmailsWithDiffProcess.Name = "checkEmailsWithDiffProcess";
			this.checkEmailsWithDiffProcess.Size = new System.Drawing.Size(286, 17);
			this.checkEmailsWithDiffProcess.TabIndex = 257;
			this.checkEmailsWithDiffProcess.Text = "Send emails with a different process on the eConnector";
			this.checkEmailsWithDiffProcess.UseVisualStyleBackColor = true;
			// 
			// labelInstallWarning
			// 
			this.labelInstallWarning.Location = new System.Drawing.Point(524, 72);
			this.labelInstallWarning.Name = "labelInstallWarning";
			this.labelInstallWarning.Size = new System.Drawing.Size(399, 13);
			this.labelInstallWarning.TabIndex = 256;
			this.labelInstallWarning.Text = " By clicking \'Install\' you consent to running the eConnector as a service";
			// 
			// butInstallEConnector
			// 
			this.butInstallEConnector.Location = new System.Drawing.Point(594, 45);
			this.butInstallEConnector.Name = "butInstallEConnector";
			this.butInstallEConnector.Size = new System.Drawing.Size(61, 24);
			this.butInstallEConnector.TabIndex = 255;
			this.butInstallEConnector.Text = "Install";
			this.butInstallEConnector.Click += new System.EventHandler(this.butInstallEConnector_Click);
			// 
			// labelListenerServiceAck
			// 
			this.labelListenerServiceAck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelListenerServiceAck.Location = new System.Drawing.Point(324, 340);
			this.labelListenerServiceAck.Name = "labelListenerServiceAck";
			this.labelListenerServiceAck.Size = new System.Drawing.Size(578, 13);
			this.labelListenerServiceAck.TabIndex = 254;
			this.labelListenerServiceAck.Text = "Acknowledge all errors.  This will stop the eServices menu from showing yellow.";
			this.labelListenerServiceAck.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butListenerServiceAck
			// 
			this.butListenerServiceAck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butListenerServiceAck.Location = new System.Drawing.Point(908, 334);
			this.butListenerServiceAck.Name = "butListenerServiceAck";
			this.butListenerServiceAck.Size = new System.Drawing.Size(61, 24);
			this.butListenerServiceAck.TabIndex = 253;
			this.butListenerServiceAck.Text = "Ack";
			this.butListenerServiceAck.Click += new System.EventHandler(this.butListenerServiceAck_Click);
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(7, 18);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(916, 19);
			this.label27.TabIndex = 252;
			this.label27.Text = "Open Dental monitors the status of the eConnector Service and alerts all workstat" +
    "ions when status is critical.";
			// 
			// butListenerServiceHistoryRefresh
			// 
			this.butListenerServiceHistoryRefresh.Location = new System.Drawing.Point(862, 111);
			this.butListenerServiceHistoryRefresh.Name = "butListenerServiceHistoryRefresh";
			this.butListenerServiceHistoryRefresh.Size = new System.Drawing.Size(61, 24);
			this.butListenerServiceHistoryRefresh.TabIndex = 251;
			this.butListenerServiceHistoryRefresh.Text = "Refresh";
			this.butListenerServiceHistoryRefresh.Click += new System.EventHandler(this.butListenerServiceHistoryRefresh_Click);
			// 
			// label26
			// 
			this.label26.Location = new System.Drawing.Point(3, 94);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(853, 37);
			this.label26.TabIndex = 250;
			this.label26.Text = resources.GetString("label26.Text");
			this.label26.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridListenerServiceStatusHistory
			// 
			this.gridListenerServiceStatusHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridListenerServiceStatusHistory.Location = new System.Drawing.Point(6, 141);
			this.gridListenerServiceStatusHistory.Name = "gridListenerServiceStatusHistory";
			this.gridListenerServiceStatusHistory.Size = new System.Drawing.Size(963, 185);
			this.gridListenerServiceStatusHistory.TabIndex = 249;
			this.gridListenerServiceStatusHistory.Title = "eConnector History";
			this.gridListenerServiceStatusHistory.TranslationName = "FormEServicesSetup";
			this.gridListenerServiceStatusHistory.WrapText = false;
			// 
			// butStartListenerService
			// 
			this.butStartListenerService.Enabled = false;
			this.butStartListenerService.Location = new System.Drawing.Point(527, 45);
			this.butStartListenerService.Name = "butStartListenerService";
			this.butStartListenerService.Size = new System.Drawing.Size(61, 24);
			this.butStartListenerService.TabIndex = 245;
			this.butStartListenerService.Text = "Start";
			this.butStartListenerService.Click += new System.EventHandler(this.butStartListenerService_Click);
			// 
			// label24
			// 
			this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label24.Location = new System.Drawing.Point(115, 412);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(578, 29);
			this.label24.TabIndex = 248;
			this.label24.Text = "Before you stop monitoring, first uninstall the eConnector Service.\r\nMonitoring w" +
    "ill automatically resume when an active eConnector Service has been detected.";
			// 
			// labelListenerStatus
			// 
			this.labelListenerStatus.Location = new System.Drawing.Point(177, 48);
			this.labelListenerStatus.Name = "labelListenerStatus";
			this.labelListenerStatus.Size = new System.Drawing.Size(238, 17);
			this.labelListenerStatus.TabIndex = 244;
			this.labelListenerStatus.Text = "Current eConnector Service Status";
			this.labelListenerStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butListenerAlertsOff
			// 
			this.butListenerAlertsOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butListenerAlertsOff.Location = new System.Drawing.Point(9, 413);
			this.butListenerAlertsOff.Name = "butListenerAlertsOff";
			this.butListenerAlertsOff.Size = new System.Drawing.Size(100, 24);
			this.butListenerAlertsOff.TabIndex = 247;
			this.butListenerAlertsOff.Text = "Stop Monitoring";
			this.butListenerAlertsOff.Click += new System.EventHandler(this.butListenerAlertsOff_Click);
			// 
			// textListenerServiceStatus
			// 
			this.textListenerServiceStatus.Location = new System.Drawing.Point(421, 47);
			this.textListenerServiceStatus.Name = "textListenerServiceStatus";
			this.textListenerServiceStatus.ReadOnly = true;
			this.textListenerServiceStatus.Size = new System.Drawing.Size(100, 20);
			this.textListenerServiceStatus.TabIndex = 246;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(915, 531);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 501;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEServicesEConnector
			// 
			this.ClientSize = new System.Drawing.Size(1002, 563);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label25);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.butOk);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesEConnector";
			this.Text = "eServices EConnector";
			this.Load += new System.EventHandler(this.FormEServicesEConnector_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butOk;
		private System.Windows.Forms.GroupBox groupBox3;
		private UI.Button butStartListenerService;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label labelListenerStatus;
		private UI.Button butListenerAlertsOff;
		private System.Windows.Forms.TextBox textListenerServiceStatus;
		private UI.GridOD gridListenerServiceStatusHistory;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private UI.Button butListenerServiceHistoryRefresh;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label labelListenerServiceAck;
		private UI.Button butListenerServiceAck;
		private System.Windows.Forms.Label label37;
		private UI.Button butInstallEConnector;
		private System.Windows.Forms.Label labelInstallWarning;
		private System.Windows.Forms.ContextMenuStrip menuWebSchedVerifyTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem insertReplacementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.CheckBox checkEmailsWithDiffProcess;
		private UI.Button butCancel;
		private System.Windows.Forms.Label labelIntervalDays;
		private ValidNum textLogCleanupInterval;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}