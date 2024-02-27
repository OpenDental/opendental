namespace OpenDental.InternalTools.Job_Manager {
	partial class UserControlTeamReport {
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
			this.groupBoxFilters = new OpenDental.UI.GroupBox();
			this.checkNoLogsThirtyDays = new OpenDental.UI.CheckBox();
			this.butExport = new OpenDental.UI.Button();
			this.butSummary = new OpenDental.UI.Button();
			this.odDateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.comboTeamFilter = new OpenDental.UI.ComboBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.labelTeamFilter = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBoxFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxFilters
			// 
			this.groupBoxFilters.Controls.Add(this.checkNoLogsThirtyDays);
			this.groupBoxFilters.Controls.Add(this.butExport);
			this.groupBoxFilters.Controls.Add(this.butSummary);
			this.groupBoxFilters.Controls.Add(this.odDateRangePicker);
			this.groupBoxFilters.Controls.Add(this.comboTeamFilter);
			this.groupBoxFilters.Controls.Add(this.butRefresh);
			this.groupBoxFilters.Controls.Add(this.labelTeamFilter);
			this.groupBoxFilters.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBoxFilters.Location = new System.Drawing.Point(0, 0);
			this.groupBoxFilters.Name = "groupBoxFilters";
			this.groupBoxFilters.Size = new System.Drawing.Size(1314, 62);
			this.groupBoxFilters.TabIndex = 64;
			this.groupBoxFilters.Text = "Filters";
			// 
			// checkNoLogsThirtyDays
			// 
			this.checkNoLogsThirtyDays.Location = new System.Drawing.Point(783, 23);
			this.checkNoLogsThirtyDays.Name = "checkNoLogsThirtyDays";
			this.checkNoLogsThirtyDays.Size = new System.Drawing.Size(206, 18);
			this.checkNoLogsThirtyDays.TabIndex = 69;
			this.checkNoLogsThirtyDays.Text = "Include jobs with no logs in 30+ days";
			// 
			// butExport
			// 
			this.butExport.Location = new System.Drawing.Point(1216, 19);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(75, 26);
			this.butExport.TabIndex = 68;
			this.butExport.Text = "Export";
			this.butExport.UseVisualStyleBackColor = true;
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butSummary
			// 
			this.butSummary.Location = new System.Drawing.Point(1111, 19);
			this.butSummary.Name = "butSummary";
			this.butSummary.Size = new System.Drawing.Size(75, 26);
			this.butSummary.TabIndex = 67;
			this.butSummary.Text = "Summary";
			this.butSummary.UseVisualStyleBackColor = true;
			this.butSummary.Click += new System.EventHandler(this.butSummary_Click);
			// 
			// odDateRangePicker
			// 
			this.odDateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.odDateRangePicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.odDateRangePicker.Location = new System.Drawing.Point(13, 20);
			this.odDateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.odDateRangePicker.MinimumSize = new System.Drawing.Size(450, 24);
			this.odDateRangePicker.Name = "odDateRangePicker";
			this.odDateRangePicker.Size = new System.Drawing.Size(450, 24);
			this.odDateRangePicker.TabIndex = 66;
			// 
			// comboTeamFilter
			// 
			this.comboTeamFilter.Location = new System.Drawing.Point(529, 22);
			this.comboTeamFilter.Name = "comboTeamFilter";
			this.comboTeamFilter.Size = new System.Drawing.Size(229, 21);
			this.comboTeamFilter.TabIndex = 65;
			this.comboTeamFilter.Text = "comboBox1";
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(1006, 19);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 26);
			this.butRefresh.TabIndex = 60;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// labelTeamFilter
			// 
			this.labelTeamFilter.Location = new System.Drawing.Point(467, 25);
			this.labelTeamFilter.Name = "labelTeamFilter";
			this.labelTeamFilter.Size = new System.Drawing.Size(60, 14);
			this.labelTeamFilter.TabIndex = 64;
			this.labelTeamFilter.Text = "Team Filter";
			this.labelTeamFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridMain.Location = new System.Drawing.Point(0, 62);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(1314, 459);
			this.gridMain.TabIndex = 65;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// UserControlJobTeamReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBoxFilters);
			this.Name = "UserControlJobTeamReport";
			this.Size = new System.Drawing.Size(1314, 521);
			this.groupBoxFilters.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GroupBox groupBoxFilters;
		private UI.ODDateRangePicker odDateRangePicker;
		private UI.Button butRefresh;
		private System.Windows.Forms.Label labelTeamFilter;
		private UI.ComboBox comboTeamFilter;
		private UI.GridOD gridMain;
		private UI.Button butSummary;
		private UI.Button butExport;
		private UI.CheckBox checkNoLogsThirtyDays;
	}
}
