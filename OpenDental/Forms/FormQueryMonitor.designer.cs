namespace OpenDental {
	partial class FormQueryMonitor {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQueryMonitor));
			this.butClose = new OpenDental.UI.Button();
			this.butStart = new OpenDental.UI.Button();
			this.butLog = new OpenDental.UI.Button();
			this.gridFeed = new OpenDental.UI.GridOD();
			this.timerProcessQueue = new System.Windows.Forms.Timer(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textElapsed = new System.Windows.Forms.TextBox();
			this.textDateTimeStop = new System.Windows.Forms.TextBox();
			this.textDateTimeStart = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textCommand = new OpenDental.ODtextBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.butCopy = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(949, 476);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butStart
			// 
			this.butStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butStart.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butStart.Location = new System.Drawing.Point(949, 74);
			this.butStart.Name = "butStart";
			this.butStart.Size = new System.Drawing.Size(75, 24);
			this.butStart.TabIndex = 3;
			this.butStart.Text = "Start";
			this.butStart.Click += new System.EventHandler(this.ButStart_Click);
			// 
			// butLog
			// 
			this.butLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butLog.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butLog.Location = new System.Drawing.Point(949, 136);
			this.butLog.Name = "butLog";
			this.butLog.Size = new System.Drawing.Size(75, 24);
			this.butLog.TabIndex = 4;
			this.butLog.Text = "Log";
			this.butLog.Click += new System.EventHandler(this.ButLog_Click);
			// 
			// gridFeed
			// 
			this.gridFeed.AllowSortingByColumn = true;
			this.gridFeed.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridFeed.Location = new System.Drawing.Point(0, 0);
			this.gridFeed.Name = "gridFeed";
			this.gridFeed.Size = new System.Drawing.Size(931, 200);
			this.gridFeed.TabIndex = 14;
			this.gridFeed.Title = "Query Feed";
			this.gridFeed.TranslationName = "TableQueryFeed";
			this.gridFeed.WrapText = false;
			this.gridFeed.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.GridFeed_CellClick);
			// 
			// timerProcessQueue
			// 
			this.timerProcessQueue.Interval = 5;
			this.timerProcessQueue.Tick += new System.EventHandler(this.TimerProcessQueue_Tick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textElapsed);
			this.groupBox1.Controls.Add(this.textDateTimeStop);
			this.groupBox1.Controls.Add(this.textDateTimeStart);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textCommand);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(931, 284);
			this.groupBox1.TabIndex = 15;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Query Details";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(544, 20);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 17);
			this.label4.TabIndex = 7;
			this.label4.Text = "Elapsed";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(313, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "Stop";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(82, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "Start";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textElapsed
			// 
			this.textElapsed.Location = new System.Drawing.Point(646, 18);
			this.textElapsed.Name = "textElapsed";
			this.textElapsed.Size = new System.Drawing.Size(111, 20);
			this.textElapsed.TabIndex = 4;
			// 
			// textDateTimeStop
			// 
			this.textDateTimeStop.Location = new System.Drawing.Point(415, 18);
			this.textDateTimeStop.Name = "textDateTimeStop";
			this.textDateTimeStop.Size = new System.Drawing.Size(123, 20);
			this.textDateTimeStop.TabIndex = 3;
			// 
			// textDateTimeStart
			// 
			this.textDateTimeStart.Location = new System.Drawing.Point(184, 18);
			this.textDateTimeStart.Name = "textDateTimeStart";
			this.textDateTimeStart.Size = new System.Drawing.Size(123, 20);
			this.textDateTimeStart.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 14);
			this.label1.TabIndex = 1;
			this.label1.Text = "Command";
			// 
			// textCommand
			// 
			this.textCommand.AcceptsTab = true;
			this.textCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textCommand.BackColor = System.Drawing.SystemColors.Window;
			this.textCommand.DetectLinksEnabled = false;
			this.textCommand.DetectUrls = false;
			this.textCommand.Location = new System.Drawing.Point(6, 45);
			this.textCommand.Name = "textCommand";
			this.textCommand.QuickPasteType = OpenDentBusiness.QuickPasteType.Query;
			this.textCommand.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textCommand.Size = new System.Drawing.Size(919, 233);
			this.textCommand.TabIndex = 0;
			this.textCommand.Text = "";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(12, 12);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.gridFeed);
			this.splitContainer1.Panel1MinSize = 200;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2MinSize = 200;
			this.splitContainer1.Size = new System.Drawing.Size(931, 488);
			this.splitContainer1.SplitterDistance = 200;
			this.splitContainer1.TabIndex = 16;
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCopy.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCopy.Location = new System.Drawing.Point(949, 363);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 24);
			this.butCopy.TabIndex = 17;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.ButCopy_Click);
			// 
			// FormQueryMonitor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1036, 512);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.butLog);
			this.Controls.Add(this.butStart);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormQueryMonitor";
			this.Text = "Query Monitor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormQueryMonitor_FormClosing);
			this.Load += new System.EventHandler(this.FormQueryMonitor_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.Button butStart;
		private UI.Button butLog;
		private UI.GridOD gridFeed;
		private System.Windows.Forms.Timer timerProcessQueue;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textElapsed;
		private System.Windows.Forms.TextBox textDateTimeStop;
		private System.Windows.Forms.TextBox textDateTimeStart;
		private System.Windows.Forms.Label label1;
		private ODtextBox textCommand;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private UI.Button butCopy;
	}
}