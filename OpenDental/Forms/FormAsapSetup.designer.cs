namespace OpenDental{
	partial class FormAsapSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAsapSetup));
			this.butClose = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkUseDefaults = new System.Windows.Forms.CheckBox();
			this.textWebSchedPerDay = new OpenDental.ValidNum();
			this.labelMaxWebSched = new System.Windows.Forms.Label();
			this.labelLeaveBlank = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(597, 401);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Default";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(112, 32);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(233, 21);
			this.comboClinic.TabIndex = 167;
			this.comboClinic.SelectedIndexChanged += new System.EventHandler(this.comboClinic_SelectedIndexChanged);
			// 
			// gridMain
			// 
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(12, 63);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(660, 292);
			this.gridMain.TabIndex = 166;
			this.gridMain.Title = "Templates";
			this.gridMain.TranslationName = "TableTemplates";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// checkUseDefaults
			// 
			this.checkUseDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseDefaults.Location = new System.Drawing.Point(353, 33);
			this.checkUseDefaults.Name = "checkUseDefaults";
			this.checkUseDefaults.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaults.TabIndex = 264;
			this.checkUseDefaults.Text = "Use Defaults";
			this.checkUseDefaults.Click += new System.EventHandler(this.checkUseDefaults_Click);
			// 
			// textWebSchedPerDay
			// 
			this.textWebSchedPerDay.Location = new System.Drawing.Point(226, 372);
			this.textWebSchedPerDay.MaxVal = 10000000;
			this.textWebSchedPerDay.MinVal = 0;
			this.textWebSchedPerDay.Name = "textWebSchedPerDay";
			this.textWebSchedPerDay.Size = new System.Drawing.Size(39, 20);
			this.textWebSchedPerDay.TabIndex = 265;
			this.textWebSchedPerDay.Leave += new System.EventHandler(this.textWebSchedPerDay_Leave);
			this.textWebSchedPerDay.ShowZero = false;
			// 
			// labelMaxWebSched
			// 
			this.labelMaxWebSched.Location = new System.Drawing.Point(15, 365);
			this.labelMaxWebSched.Name = "labelMaxWebSched";
			this.labelMaxWebSched.Size = new System.Drawing.Size(205, 33);
			this.labelMaxWebSched.TabIndex = 266;
			this.labelMaxWebSched.Text = "Maximum number of texts to send to a patient in a day via Web Sched";
			this.labelMaxWebSched.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelLeaveBlank
			// 
			this.labelLeaveBlank.Location = new System.Drawing.Point(271, 374);
			this.labelLeaveBlank.Name = "labelLeaveBlank";
			this.labelLeaveBlank.Size = new System.Drawing.Size(146, 16);
			this.labelLeaveBlank.TabIndex = 267;
			this.labelLeaveBlank.Text = "(leave blank for no limit)";
			this.labelLeaveBlank.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormAsapSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(684, 437);
			this.Controls.Add(this.labelLeaveBlank);
			this.Controls.Add(this.labelMaxWebSched);
			this.Controls.Add(this.textWebSchedPerDay);
			this.Controls.Add(this.checkUseDefaults);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAsapSetup";
			this.Text = "ASAP List Setup";
			this.Load += new System.EventHandler(this.FormAsapSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private UI.GridOD gridMain;
		private System.Windows.Forms.CheckBox checkUseDefaults;
		private ValidNum textWebSchedPerDay;
		private System.Windows.Forms.Label labelMaxWebSched;
		private System.Windows.Forms.Label labelLeaveBlank;
	}
}