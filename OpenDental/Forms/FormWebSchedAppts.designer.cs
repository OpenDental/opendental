namespace OpenDental {
	partial class FormWebSchedAppts {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebSchedAppts));
			this.butClose = new OpenDental.UI.Button();
			this.datePicker = new OpenDental.UI.ODDateRangePicker();
			this.comboBoxClinicMulti = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboConfStatus = new OpenDental.UI.ComboBoxOD();
			this.labelConfStatus = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.contextMenuMainGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mainGridMenuItemPatChart = new System.Windows.Forms.ToolStripMenuItem();
			this.mainGridMenuItemApptEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.butRefresh = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkWebSchedExistingPat = new System.Windows.Forms.CheckBox();
			this.checkASAP = new System.Windows.Forms.CheckBox();
			this.checkWebSchedNewPat = new System.Windows.Forms.CheckBox();
			this.checkWebSchedRecall = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.contextMenuMainGrid.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1143, 660);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 6;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// datePicker
			// 
			this.datePicker.BackColor = System.Drawing.SystemColors.Control;
			this.datePicker.EnableWeekButtons = false;
			this.datePicker.Location = new System.Drawing.Point(4, 13);
			this.datePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.datePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(453, 25);
			this.datePicker.TabIndex = 0;
			// 
			// comboBoxClinicMulti
			// 
			this.comboBoxClinicMulti.IncludeAll = true;
			this.comboBoxClinicMulti.IncludeUnassigned = true;
			this.comboBoxClinicMulti.Location = new System.Drawing.Point(45, 69);
			this.comboBoxClinicMulti.Name = "comboBoxClinicMulti";
			this.comboBoxClinicMulti.SelectionModeMulti = true;
			this.comboBoxClinicMulti.Size = new System.Drawing.Size(197, 22);
			this.comboBoxClinicMulti.TabIndex = 1;
			// 
			// comboConfStatus
			// 
			this.comboConfStatus.BackColor = System.Drawing.SystemColors.Window;
			this.comboConfStatus.Location = new System.Drawing.Point(373, 70);
			this.comboConfStatus.Name = "comboConfStatus";
			this.comboConfStatus.SelectionModeMulti = true;
			this.comboConfStatus.Size = new System.Drawing.Size(160, 21);
			this.comboConfStatus.TabIndex = 2;
			// 
			// labelConfStatus
			// 
			this.labelConfStatus.Location = new System.Drawing.Point(248, 71);
			this.labelConfStatus.Name = "labelConfStatus";
			this.labelConfStatus.Size = new System.Drawing.Size(124, 17);
			this.labelConfStatus.TabIndex = 2;
			this.labelConfStatus.Text = "Confirmation Status";
			this.labelConfStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.contextMenuMainGrid;
			this.gridMain.Location = new System.Drawing.Point(13, 97);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(1205, 557);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Appointments";
			this.gridMain.TranslationName = "TableAppointments";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// contextMenuMainGrid
			// 
			this.contextMenuMainGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainGridMenuItemPatChart,
            this.mainGridMenuItemApptEdit});
			this.contextMenuMainGrid.Name = "contextMenuMainGrid";
			this.contextMenuMainGrid.Size = new System.Drawing.Size(176, 48);
			// 
			// mainGridMenuItemPatChart
			// 
			this.mainGridMenuItemPatChart.Name = "mainGridMenuItemPatChart";
			this.mainGridMenuItemPatChart.Size = new System.Drawing.Size(175, 22);
			this.mainGridMenuItemPatChart.Text = "Open Patient Chart";
			this.mainGridMenuItemPatChart.Click += new System.EventHandler(this.mainGridMenuItemPatChart_Click);
			// 
			// mainGridMenuItemApptEdit
			// 
			this.mainGridMenuItemApptEdit.Name = "mainGridMenuItemApptEdit";
			this.mainGridMenuItemApptEdit.Size = new System.Drawing.Size(175, 22);
			this.mainGridMenuItemApptEdit.Text = "Edit Appointment";
			this.mainGridMenuItemApptEdit.Click += new System.EventHandler(this.mainGridMenuItemApptEdit_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(1141, 68);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(77, 23);
			this.butRefresh.TabIndex = 4;
			this.butRefresh.Text = "&Refresh List";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkWebSchedExistingPat);
			this.groupBox2.Controls.Add(this.checkASAP);
			this.groupBox2.Controls.Add(this.checkWebSchedNewPat);
			this.groupBox2.Controls.Add(this.checkWebSchedRecall);
			this.groupBox2.Location = new System.Drawing.Point(672, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(433, 79);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Web Sched Appointment Types";
			// 
			// checkWebSchedExistingPat
			// 
			this.checkWebSchedExistingPat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkWebSchedExistingPat.Location = new System.Drawing.Point(227, 47);
			this.checkWebSchedExistingPat.Name = "checkWebSchedExistingPat";
			this.checkWebSchedExistingPat.Size = new System.Drawing.Size(202, 18);
			this.checkWebSchedExistingPat.TabIndex = 3;
			this.checkWebSchedExistingPat.Text = "Show Existing Patient Appointments";
			this.checkWebSchedExistingPat.UseVisualStyleBackColor = true;
			// 
			// checkASAP
			// 
			this.checkASAP.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkASAP.Location = new System.Drawing.Point(227, 24);
			this.checkASAP.Name = "checkASAP";
			this.checkASAP.Size = new System.Drawing.Size(202, 18);
			this.checkASAP.TabIndex = 2;
			this.checkASAP.Text = "Show ASAP Appointments";
			// 
			// checkWebSchedNewPat
			// 
			this.checkWebSchedNewPat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkWebSchedNewPat.Location = new System.Drawing.Point(19, 47);
			this.checkWebSchedNewPat.Name = "checkWebSchedNewPat";
			this.checkWebSchedNewPat.Size = new System.Drawing.Size(202, 18);
			this.checkWebSchedNewPat.TabIndex = 1;
			this.checkWebSchedNewPat.Text = "Show New Patient Appointments";
			// 
			// checkWebSchedRecall
			// 
			this.checkWebSchedRecall.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkWebSchedRecall.Location = new System.Drawing.Point(19, 24);
			this.checkWebSchedRecall.Name = "checkWebSchedRecall";
			this.checkWebSchedRecall.Size = new System.Drawing.Size(202, 18);
			this.checkWebSchedRecall.TabIndex = 0;
			this.checkWebSchedRecall.Text = "Show Recall Appointments";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.datePicker);
			this.groupBox1.Location = new System.Drawing.Point(13, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(461, 45);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Date Created";
			// 
			// FormWebSchedAppts
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelConfStatus);
			this.Controls.Add(this.comboConfStatus);
			this.Controls.Add(this.comboBoxClinicMulti);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebSchedAppts";
			this.Text = "Web Sched Appointments";
			this.Load += new System.EventHandler(this.FormWebSchedAppts_Load);
			this.contextMenuMainGrid.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.ODDateRangePicker datePicker;
		private UI.ComboBoxClinicPicker comboBoxClinicMulti;
		private UI.ComboBoxOD comboConfStatus;
		private System.Windows.Forms.Label labelConfStatus;
		private UI.GridOD gridMain;
		private UI.Button butRefresh;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkWebSchedNewPat;
		private System.Windows.Forms.CheckBox checkWebSchedRecall;
		private System.Windows.Forms.ContextMenuStrip contextMenuMainGrid;
		private System.Windows.Forms.ToolStripMenuItem mainGridMenuItemPatChart;
		private System.Windows.Forms.ToolStripMenuItem mainGridMenuItemApptEdit;
		private System.Windows.Forms.CheckBox checkASAP;
		private System.Windows.Forms.CheckBox checkWebSchedExistingPat;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}