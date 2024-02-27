namespace OpenDental{
	partial class FormCareCreditTransactions {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCareCreditTransactions));
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemGoTo = new System.Windows.Forms.ToolStripMenuItem();
			this.openPaymentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.processRefundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.acknowledgeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewErrorMessageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.tabControl = new OpenDental.UI.TabControl();
			this.tabTrans = new OpenDental.UI.TabPage();
			this.tabQSBatchTrans = new OpenDental.UI.TabPage();
			this.butNoneQSBatch = new OpenDental.UI.Button();
			this.butButAllQSTrans = new OpenDental.UI.Button();
			this.labelProcessQSBatches = new System.Windows.Forms.Label();
			this.butProcessBatchQS = new OpenDental.UI.Button();
			this.gridQSBatchTrans = new OpenDental.UI.GridOD();
			this.tabErrors = new OpenDental.UI.TabPage();
			this.butNone = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.butAck = new OpenDental.UI.Button();
			this.gridError = new OpenDental.UI.GridOD();
			this.checkIncludeAck = new OpenDental.UI.CheckBox();
			this.labelStatus = new System.Windows.Forms.Label();
			this.comboStatuses = new OpenDental.UI.ComboBox();
			this.contextMenu.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabTrans.SuspendLayout();
			this.tabQSBatchTrans.SuspendLayout();
			this.tabErrors.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemGoTo,
            this.openPaymentToolStripMenuItem,
            this.processRefundToolStripMenuItem,
            this.acknowledgeToolStripMenuItem,
            this.viewErrorMessageMenuItem});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(177, 114);
			this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.Name = "menuItemGoTo";
			this.menuItemGoTo.Size = new System.Drawing.Size(176, 22);
			this.menuItemGoTo.Text = "Go To Account";
			this.menuItemGoTo.Click += new System.EventHandler(this.menuItemGoTo_Click);
			// 
			// openPaymentToolStripMenuItem
			// 
			this.openPaymentToolStripMenuItem.Name = "openPaymentToolStripMenuItem";
			this.openPaymentToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.openPaymentToolStripMenuItem.Text = "Open Payment";
			this.openPaymentToolStripMenuItem.Click += new System.EventHandler(this.openPaymentToolStripMenuItem_Click);
			// 
			// processRefundToolStripMenuItem
			// 
			this.processRefundToolStripMenuItem.Name = "processRefundToolStripMenuItem";
			this.processRefundToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.processRefundToolStripMenuItem.Text = "Process Refund";
			this.processRefundToolStripMenuItem.Click += new System.EventHandler(this.processReturnToolStripMenuItem_Click);
			// 
			// acknowledgeToolStripMenuItem
			// 
			this.acknowledgeToolStripMenuItem.Name = "acknowledgeToolStripMenuItem";
			this.acknowledgeToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.acknowledgeToolStripMenuItem.Text = "Acknowledge";
			this.acknowledgeToolStripMenuItem.Click += new System.EventHandler(this.acknowledgeToolStripMenuItem_Click);
			// 
			// viewErrorMessageMenuItem
			// 
			this.viewErrorMessageMenuItem.Name = "viewErrorMessageMenuItem";
			this.viewErrorMessageMenuItem.Size = new System.Drawing.Size(176, 22);
			this.viewErrorMessageMenuItem.Text = "View Error Message";
			this.viewErrorMessageMenuItem.Click += new System.EventHandler(this.viewErrorMessage_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.contextMenu;
			this.gridMain.Location = new System.Drawing.Point(1, 1);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(946, 492);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "CareCredit Transactions";
			this.gridMain.TranslationName = "TableCareCreditTrans";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCur_CellDoubleClick);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(882, 10);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(82, 24);
			this.butRefresh.TabIndex = 3;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(657, 12);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(200, 21);
			this.comboClinic.TabIndex = 1;
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.dateRangePicker.Location = new System.Drawing.Point(4, 9);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 24);
			this.dateRangePicker.TabIndex = 0;
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabTrans);
			this.tabControl.Controls.Add(this.tabQSBatchTrans);
			this.tabControl.Controls.Add(this.tabErrors);
			this.tabControl.Location = new System.Drawing.Point(12, 56);
			this.tabControl.Name = "tabControl";
			this.tabControl.Size = new System.Drawing.Size(952, 516);
			this.tabControl.TabIndex = 4;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tabTrans
			// 
			this.tabTrans.Controls.Add(this.gridMain);
			this.tabTrans.Location = new System.Drawing.Point(2, 21);
			this.tabTrans.Name = "tabTrans";
			this.tabTrans.Padding = new System.Windows.Forms.Padding(3);
			this.tabTrans.Size = new System.Drawing.Size(948, 493);
			this.tabTrans.TabIndex = 0;
			this.tabTrans.Text = "Transactions";
			// 
			// tabQSBatchTrans
			// 
			this.tabQSBatchTrans.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.tabQSBatchTrans.Controls.Add(this.butNoneQSBatch);
			this.tabQSBatchTrans.Controls.Add(this.butButAllQSTrans);
			this.tabQSBatchTrans.Controls.Add(this.labelProcessQSBatches);
			this.tabQSBatchTrans.Controls.Add(this.butProcessBatchQS);
			this.tabQSBatchTrans.Controls.Add(this.gridQSBatchTrans);
			this.tabQSBatchTrans.Location = new System.Drawing.Point(2, 21);
			this.tabQSBatchTrans.Name = "tabQSBatchTrans";
			this.tabQSBatchTrans.Padding = new System.Windows.Forms.Padding(3);
			this.tabQSBatchTrans.Size = new System.Drawing.Size(948, 493);
			this.tabQSBatchTrans.TabIndex = 2;
			this.tabQSBatchTrans.Text = "Batch Transactions";
			// 
			// butNoneQSBatch
			// 
			this.butNoneQSBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNoneQSBatch.Location = new System.Drawing.Point(104, 461);
			this.butNoneQSBatch.Name = "butNoneQSBatch";
			this.butNoneQSBatch.Size = new System.Drawing.Size(82, 24);
			this.butNoneQSBatch.TabIndex = 151;
			this.butNoneQSBatch.Text = "None";
			this.butNoneQSBatch.Click += new System.EventHandler(this.butNoneQSBatch_Click);
			// 
			// butButAllQSTrans
			// 
			this.butButAllQSTrans.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butButAllQSTrans.Location = new System.Drawing.Point(6, 461);
			this.butButAllQSTrans.Name = "butButAllQSTrans";
			this.butButAllQSTrans.Size = new System.Drawing.Size(82, 24);
			this.butButAllQSTrans.TabIndex = 150;
			this.butButAllQSTrans.Text = "All";
			this.butButAllQSTrans.Click += new System.EventHandler(this.butButAllQSTrans_Click);
			// 
			// labelProcessQSBatches
			// 
			this.labelProcessQSBatches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelProcessQSBatches.Location = new System.Drawing.Point(462, 461);
			this.labelProcessQSBatches.Name = "labelProcessQSBatches";
			this.labelProcessQSBatches.Size = new System.Drawing.Size(367, 24);
			this.labelProcessQSBatches.TabIndex = 149;
			this.labelProcessQSBatches.Text = "Only reprocesses expired batches";
			this.labelProcessQSBatches.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butProcessBatchQS
			// 
			this.butProcessBatchQS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butProcessBatchQS.Location = new System.Drawing.Point(367, 461);
			this.butProcessBatchQS.Name = "butProcessBatchQS";
			this.butProcessBatchQS.Size = new System.Drawing.Size(91, 24);
			this.butProcessBatchQS.TabIndex = 148;
			this.butProcessBatchQS.Text = "Reprocess";
			this.butProcessBatchQS.Click += new System.EventHandler(this.butProcessBatchQS_Click);
			// 
			// gridQSBatchTrans
			// 
			this.gridQSBatchTrans.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridQSBatchTrans.ContextMenuStrip = this.contextMenu;
			this.gridQSBatchTrans.Location = new System.Drawing.Point(1, 1);
			this.gridQSBatchTrans.Name = "gridQSBatchTrans";
			this.gridQSBatchTrans.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridQSBatchTrans.Size = new System.Drawing.Size(945, 450);
			this.gridQSBatchTrans.TabIndex = 142;
			this.gridQSBatchTrans.Title = "CareCredit Batch Transactions";
			this.gridQSBatchTrans.TranslationName = "TableCareCreditBatch";
			// 
			// tabErrors
			// 
			this.tabErrors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.tabErrors.Controls.Add(this.butNone);
			this.tabErrors.Controls.Add(this.butAll);
			this.tabErrors.Controls.Add(this.butAck);
			this.tabErrors.Controls.Add(this.gridError);
			this.tabErrors.Location = new System.Drawing.Point(2, 21);
			this.tabErrors.Name = "tabErrors";
			this.tabErrors.Padding = new System.Windows.Forms.Padding(3);
			this.tabErrors.Size = new System.Drawing.Size(948, 493);
			this.tabErrors.TabIndex = 1;
			this.tabErrors.Text = "Errors(0)";
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNone.Location = new System.Drawing.Point(104, 461);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(82, 24);
			this.butNone.TabIndex = 148;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// butAll
			// 
			this.butAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAll.Location = new System.Drawing.Point(6, 461);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(82, 24);
			this.butAll.TabIndex = 147;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butAck
			// 
			this.butAck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAck.Location = new System.Drawing.Point(367, 461);
			this.butAck.Name = "butAck";
			this.butAck.Size = new System.Drawing.Size(91, 24);
			this.butAck.TabIndex = 146;
			this.butAck.Text = "Acknowledge";
			this.butAck.Click += new System.EventHandler(this.butAck_Click);
			// 
			// gridError
			// 
			this.gridError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridError.ContextMenuStrip = this.contextMenu;
			this.gridError.Location = new System.Drawing.Point(1, 1);
			this.gridError.Name = "gridError";
			this.gridError.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridError.Size = new System.Drawing.Size(945, 450);
			this.gridError.TabIndex = 141;
			this.gridError.Title = "CareCredit Batch Errors";
			this.gridError.TranslationName = "TableCareCreditErrors";
			this.gridError.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCur_CellDoubleClick);
			// 
			// checkIncludeAck
			// 
			this.checkIncludeAck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeAck.Location = new System.Drawing.Point(722, 39);
			this.checkIncludeAck.Name = "checkIncludeAck";
			this.checkIncludeAck.Size = new System.Drawing.Size(135, 16);
			this.checkIncludeAck.TabIndex = 2;
			this.checkIncludeAck.Text = "Include Acknowledged";
			// 
			// labelStatus
			// 
			this.labelStatus.Location = new System.Drawing.Point(426, 14);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(51, 16);
			this.labelStatus.TabIndex = 41;
			this.labelStatus.Text = "Status";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatuses
			// 
			this.comboStatuses.BackColor = System.Drawing.SystemColors.Window;
			this.comboStatuses.Location = new System.Drawing.Point(479, 13);
			this.comboStatuses.Name = "comboStatuses";
			this.comboStatuses.SelectionModeMulti = true;
			this.comboStatuses.Size = new System.Drawing.Size(160, 21);
			this.comboStatuses.TabIndex = 40;
			// 
			// FormCareCreditTransactions
			// 
			this.ClientSize = new System.Drawing.Size(990, 584);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.comboStatuses);
			this.Controls.Add(this.checkIncludeAck);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.dateRangePicker);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butRefresh);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCareCreditTransactions";
			this.Text = "CareCredit Transactions";
			this.Load += new System.EventHandler(this.FormCareCreditTransactions_Load);
			this.contextMenu.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabTrans.ResumeLayout(false);
			this.tabQSBatchTrans.ResumeLayout(false);
			this.tabErrors.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
		private UI.Button butRefresh;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemGoTo;
		private System.Windows.Forms.ToolStripMenuItem openPaymentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem processRefundToolStripMenuItem;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.ODDateRangePicker dateRangePicker;
		private OpenDental.UI.TabControl tabControl;
		private OpenDental.UI.TabPage tabTrans;
		private OpenDental.UI.TabPage tabErrors;
		private UI.GridOD gridError;
		private UI.Button butAck;
		private UI.Button butAll;
		private UI.Button butNone;
		private System.Windows.Forms.ToolStripMenuItem acknowledgeToolStripMenuItem;
		private OpenDental.UI.CheckBox checkIncludeAck;
		private OpenDental.UI.TabPage tabQSBatchTrans;
		private UI.GridOD gridQSBatchTrans;
		private UI.Button butProcessBatchQS;
		private System.Windows.Forms.Label labelStatus;
		private UI.ComboBox comboStatuses;
		private System.Windows.Forms.Label labelProcessQSBatches;
		private System.Windows.Forms.ToolStripMenuItem viewErrorMessageMenuItem;
		private UI.Button butNoneQSBatch;
		private UI.Button butButAllQSTrans;
	}
}