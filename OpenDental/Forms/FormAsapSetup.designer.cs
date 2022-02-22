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
			this.checkAsapPromptEnabled = new System.Windows.Forms.CheckBox();
			this.groupBoxASAPPrompt = new OpenDental.UI.GroupBoxOD();
			this.labelNoteASAP = new System.Windows.Forms.Label();
			this.groupBoxASAPPrompt.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(597, 463);
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
			this.textWebSchedPerDay.Location = new System.Drawing.Point(203, 365);
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
			this.labelMaxWebSched.Location = new System.Drawing.Point(9, 358);
			this.labelMaxWebSched.Name = "labelMaxWebSched";
			this.labelMaxWebSched.Size = new System.Drawing.Size(188, 33);
			this.labelMaxWebSched.TabIndex = 266;
			this.labelMaxWebSched.Text = "Maximum number of texts to send to a patient in a day via Web Sched";
			this.labelMaxWebSched.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelLeaveBlank
			// 
			this.labelLeaveBlank.Location = new System.Drawing.Point(248, 366);
			this.labelLeaveBlank.Name = "labelLeaveBlank";
			this.labelLeaveBlank.Size = new System.Drawing.Size(146, 16);
			this.labelLeaveBlank.TabIndex = 267;
			this.labelLeaveBlank.Text = "(leave blank for no limit)";
			this.labelLeaveBlank.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkAsapPromptEnabled
			// 
			this.checkAsapPromptEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAsapPromptEnabled.Location = new System.Drawing.Point(7, 19);
			this.checkAsapPromptEnabled.Name = "checkAsapPromptEnabled";
			this.checkAsapPromptEnabled.Size = new System.Drawing.Size(509, 30);
			this.checkAsapPromptEnabled.TabIndex = 268;
			this.checkAsapPromptEnabled.Text = "Prompt user to send Web Sched messages to patients on ASAP list when an appointme" +
    "nt is broken";
			this.checkAsapPromptEnabled.UseVisualStyleBackColor = false;
			this.checkAsapPromptEnabled.Click += new System.EventHandler(this.checkAsapPromptEnabled_Click);
			// 
			// groupBoxASAPPrompt
			// 
			this.groupBoxASAPPrompt.Controls.Add(this.labelNoteASAP);
			this.groupBoxASAPPrompt.Controls.Add(this.checkAsapPromptEnabled);
			this.groupBoxASAPPrompt.Location = new System.Drawing.Point(12, 398);
			this.groupBoxASAPPrompt.Name = "groupBoxASAPPrompt";
			this.groupBoxASAPPrompt.Size = new System.Drawing.Size(579, 89);
			this.groupBoxASAPPrompt.TabIndex = 269;
			this.groupBoxASAPPrompt.Text = "ASAP Web Sched Prompt";
			// 
			// labelNoteASAP
			// 
			this.labelNoteASAP.Location = new System.Drawing.Point(3, 53);
			this.labelNoteASAP.Name = "labelNoteASAP";
			this.labelNoteASAP.Size = new System.Drawing.Size(573, 33);
			this.labelNoteASAP.TabIndex = 268;
			this.labelNoteASAP.Text = "(Note: No prompt will show if the appointment starts within the next 20 minutes o" +
    "r if the appointment has already started.)";
			// 
			// FormAsapSetup
			// 
			this.ClientSize = new System.Drawing.Size(684, 499);
			this.Controls.Add(this.groupBoxASAPPrompt);
			this.Controls.Add(this.checkUseDefaults);
			this.Controls.Add(this.labelMaxWebSched);
			this.Controls.Add(this.textWebSchedPerDay);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelLeaveBlank);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAsapSetup";
			this.Text = "ASAP List Setup";
			this.Load += new System.EventHandler(this.FormAsapSetup_Load);
			this.groupBoxASAPPrompt.ResumeLayout(false);
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
		private System.Windows.Forms.CheckBox checkAsapPromptEnabled;
		private UI.GroupBoxOD groupBoxASAPPrompt;
		private System.Windows.Forms.Label labelNoteASAP;
	}
}