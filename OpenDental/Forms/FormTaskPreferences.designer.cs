namespace OpenDental{
	partial class FormTaskPreferences {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskPreferences));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkTaskSortApptDateTime = new System.Windows.Forms.CheckBox();
			this.checkShowOpenTickets = new System.Windows.Forms.CheckBox();
			this.checkTasksNewTrackedByUser = new System.Windows.Forms.CheckBox();
			this.groupBoxComputerDefaults = new System.Windows.Forms.GroupBox();
			this.radioRight = new System.Windows.Forms.RadioButton();
			this.radioBottom = new System.Windows.Forms.RadioButton();
			this.validNumY = new OpenDental.ValidNum();
			this.labelY = new System.Windows.Forms.Label();
			this.validNumX = new OpenDental.ValidNum();
			this.labelX = new System.Windows.Forms.Label();
			this.checkBoxTaskKeepListHidden = new System.Windows.Forms.CheckBox();
			this.checkTaskListAlwaysShow = new System.Windows.Forms.CheckBox();
			this.checkShowLegacyRepeatingTasks = new System.Windows.Forms.CheckBox();
			this.groupBoxDatabase = new System.Windows.Forms.GroupBox();
			this.butTaskInboxSetup = new OpenDental.UI.Button();
			this.comboGlobalFilter = new UI.ComboBoxOD();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.labelGlobalFilter = new System.Windows.Forms.Label();
			this.groupBoxComputerDefaults.SuspendLayout();
			this.groupBoxDatabase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(216, 295);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(297, 295);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkTaskSortApptDateTime
			// 
			this.checkTaskSortApptDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTaskSortApptDateTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTaskSortApptDateTime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTaskSortApptDateTime.Location = new System.Drawing.Point(12, 126);
			this.checkTaskSortApptDateTime.Name = "checkTaskSortApptDateTime";
			this.checkTaskSortApptDateTime.Size = new System.Drawing.Size(342, 14);
			this.checkTaskSortApptDateTime.TabIndex = 6;
			this.checkTaskSortApptDateTime.Text = "Default to sorting appointment type task lists by AptDateTime";
			this.checkTaskSortApptDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowOpenTickets
			// 
			this.checkShowOpenTickets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowOpenTickets.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowOpenTickets.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowOpenTickets.Location = new System.Drawing.Point(12, 108);
			this.checkShowOpenTickets.Name = "checkShowOpenTickets";
			this.checkShowOpenTickets.Size = new System.Drawing.Size(342, 14);
			this.checkShowOpenTickets.TabIndex = 5;
			this.checkShowOpenTickets.Text = "Show open tasks for user";
			this.checkShowOpenTickets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkTasksNewTrackedByUser
			// 
			this.checkTasksNewTrackedByUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTasksNewTrackedByUser.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTasksNewTrackedByUser.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTasksNewTrackedByUser.Location = new System.Drawing.Point(54, 90);
			this.checkTasksNewTrackedByUser.Name = "checkTasksNewTrackedByUser";
			this.checkTasksNewTrackedByUser.Size = new System.Drawing.Size(300, 14);
			this.checkTasksNewTrackedByUser.TabIndex = 4;
			this.checkTasksNewTrackedByUser.Text = "New/Viewed status tracked by individual user";
			this.checkTasksNewTrackedByUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxComputerDefaults
			// 
			this.groupBoxComputerDefaults.Controls.Add(this.radioRight);
			this.groupBoxComputerDefaults.Controls.Add(this.radioBottom);
			this.groupBoxComputerDefaults.Controls.Add(this.validNumY);
			this.groupBoxComputerDefaults.Controls.Add(this.labelY);
			this.groupBoxComputerDefaults.Controls.Add(this.validNumX);
			this.groupBoxComputerDefaults.Controls.Add(this.labelX);
			this.groupBoxComputerDefaults.Controls.Add(this.checkBoxTaskKeepListHidden);
			this.groupBoxComputerDefaults.Enabled = false;
			this.groupBoxComputerDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBoxComputerDefaults.Location = new System.Drawing.Point(12, 198);
			this.groupBoxComputerDefaults.Name = "groupBoxComputerDefaults";
			this.groupBoxComputerDefaults.Size = new System.Drawing.Size(360, 86);
			this.groupBoxComputerDefaults.TabIndex = 76;
			this.groupBoxComputerDefaults.TabStop = false;
			this.groupBoxComputerDefaults.Text = "Local Computer Default Settings";
			// 
			// radioRight
			// 
			this.radioRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioRight.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioRight.Location = new System.Drawing.Point(154, 38);
			this.radioRight.Name = "radioRight";
			this.radioRight.Size = new System.Drawing.Size(99, 17);
			this.radioRight.TabIndex = 8;
			this.radioRight.TabStop = true;
			this.radioRight.Text = "Dock Right";
			this.radioRight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioRight.UseVisualStyleBackColor = true;
			// 
			// radioBottom
			// 
			this.radioBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioBottom.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioBottom.Location = new System.Drawing.Point(255, 38);
			this.radioBottom.Name = "radioBottom";
			this.radioBottom.Size = new System.Drawing.Size(96, 17);
			this.radioBottom.TabIndex = 9;
			this.radioBottom.TabStop = true;
			this.radioBottom.Text = "Dock Bottom";
			this.radioBottom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioBottom.UseVisualStyleBackColor = true;
			// 
			// validNumY
			// 
			this.validNumY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.validNumY.Location = new System.Drawing.Point(304, 59);
			this.validNumY.MaxLength = 4;
			this.validNumY.MaxVal = 1200;
			this.validNumY.MinVal = 300;
			this.validNumY.Name = "validNumY";
			this.validNumY.Size = new System.Drawing.Size(47, 20);
			this.validNumY.TabIndex = 11;
			this.validNumY.Text = "542";
			this.validNumY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.validNumY.ShowZero = false;
			// 
			// labelY
			// 
			this.labelY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelY.Location = new System.Drawing.Point(236, 59);
			this.labelY.Name = "labelY";
			this.labelY.Size = new System.Drawing.Size(62, 18);
			this.labelY.TabIndex = 189;
			this.labelY.Text = "Y Default";
			this.labelY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// validNumX
			// 
			this.validNumX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.validNumX.Location = new System.Drawing.Point(184, 59);
			this.validNumX.MaxLength = 4;
			this.validNumX.MaxVal = 2000;
			this.validNumX.MinVal = 300;
			this.validNumX.Name = "validNumX";
			this.validNumX.Size = new System.Drawing.Size(47, 20);
			this.validNumX.TabIndex = 10;
			this.validNumX.Text = "542";
			this.validNumX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.validNumX.ShowZero = false;
			// 
			// labelX
			// 
			this.labelX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelX.Location = new System.Drawing.Point(116, 59);
			this.labelX.Name = "labelX";
			this.labelX.Size = new System.Drawing.Size(62, 18);
			this.labelX.TabIndex = 187;
			this.labelX.Text = "X Default";
			this.labelX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBoxTaskKeepListHidden
			// 
			this.checkBoxTaskKeepListHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxTaskKeepListHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxTaskKeepListHidden.Location = new System.Drawing.Point(133, 19);
			this.checkBoxTaskKeepListHidden.Name = "checkBoxTaskKeepListHidden";
			this.checkBoxTaskKeepListHidden.Size = new System.Drawing.Size(218, 20);
			this.checkBoxTaskKeepListHidden.TabIndex = 7;
			this.checkBoxTaskKeepListHidden.Text = "Don\'t show on this computer";
			this.checkBoxTaskKeepListHidden.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.checkBoxTaskKeepListHidden.UseVisualStyleBackColor = true;
			this.checkBoxTaskKeepListHidden.CheckedChanged += new System.EventHandler(this.checkBoxTaskKeepListHidden_CheckedChanged);
			// 
			// checkTaskListAlwaysShow
			// 
			this.checkTaskListAlwaysShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTaskListAlwaysShow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTaskListAlwaysShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTaskListAlwaysShow.Location = new System.Drawing.Point(164, 72);
			this.checkTaskListAlwaysShow.Name = "checkTaskListAlwaysShow";
			this.checkTaskListAlwaysShow.Size = new System.Drawing.Size(190, 14);
			this.checkTaskListAlwaysShow.TabIndex = 3;
			this.checkTaskListAlwaysShow.Text = "Always show task list";
			this.checkTaskListAlwaysShow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTaskListAlwaysShow.CheckedChanged += new System.EventHandler(this.checkTaskListAlwaysShow_CheckedChanged);
			// 
			// checkShowLegacyRepeatingTasks
			// 
			this.checkShowLegacyRepeatingTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowLegacyRepeatingTasks.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowLegacyRepeatingTasks.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowLegacyRepeatingTasks.Location = new System.Drawing.Point(124, 54);
			this.checkShowLegacyRepeatingTasks.Name = "checkShowLegacyRepeatingTasks";
			this.checkShowLegacyRepeatingTasks.Size = new System.Drawing.Size(230, 14);
			this.checkShowLegacyRepeatingTasks.TabIndex = 2;
			this.checkShowLegacyRepeatingTasks.Text = "Show legacy repeating tasks";
			this.checkShowLegacyRepeatingTasks.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxDatabase
			// 
			this.groupBoxDatabase.Controls.Add(this.labelGlobalFilter);
			this.groupBoxDatabase.Controls.Add(this.butTaskInboxSetup);
			this.groupBoxDatabase.Controls.Add(this.comboGlobalFilter);
			this.groupBoxDatabase.Controls.Add(this.checkShowLegacyRepeatingTasks);
			this.groupBoxDatabase.Controls.Add(this.checkTaskSortApptDateTime);
			this.groupBoxDatabase.Controls.Add(this.checkTaskListAlwaysShow);
			this.groupBoxDatabase.Controls.Add(this.checkShowOpenTickets);
			this.groupBoxDatabase.Controls.Add(this.checkTasksNewTrackedByUser);
			this.groupBoxDatabase.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBoxDatabase.Location = new System.Drawing.Point(12, 12);
			this.groupBoxDatabase.Name = "groupBoxDatabase";
			this.groupBoxDatabase.Size = new System.Drawing.Size(360, 180);
			this.groupBoxDatabase.TabIndex = 80;
			this.groupBoxDatabase.TabStop = false;
			this.groupBoxDatabase.Text = "Global Settings";
			// 
			// butTaskInboxSetup
			// 
			this.butTaskInboxSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butTaskInboxSetup.Location = new System.Drawing.Point(279, 19);
			this.butTaskInboxSetup.Name = "butTaskInboxSetup";
			this.butTaskInboxSetup.Size = new System.Drawing.Size(75, 23);
			this.butTaskInboxSetup.TabIndex = 1;
			this.butTaskInboxSetup.Text = "Inbox Setup";
			this.butTaskInboxSetup.UseVisualStyleBackColor = true;
			this.butTaskInboxSetup.Click += new System.EventHandler(this.butTaskInboxSetup_Click);
			// 
			// comboGlobalFilter
			// 
			this.comboGlobalFilter.Location = new System.Drawing.Point(234, 146);
			this.comboGlobalFilter.Name = "comboGlobalFilter";
			this.comboGlobalFilter.Size = new System.Drawing.Size(120, 21);
			this.comboGlobalFilter.TabIndex = 139;
			this.comboGlobalFilter.Visible = false;
			this.comboGlobalFilter.SelectionChangeCommitted += new System.EventHandler(this.comboGlobalFilter_SelectionChangeCommitted);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// labelGlobalFilter
			// 
			this.labelGlobalFilter.Location = new System.Drawing.Point(54, 146);
			this.labelGlobalFilter.Name = "labelGlobalFilter";
			this.labelGlobalFilter.Size = new System.Drawing.Size(177, 19);
			this.labelGlobalFilter.TabIndex = 140;
			this.labelGlobalFilter.Text = "Global Filter for Task Lists";
			this.labelGlobalFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelGlobalFilter.Visible = false;
			// 
			// FormTaskPreferences
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(384, 331);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBoxComputerDefaults);
			this.Controls.Add(this.groupBoxDatabase);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskPreferences";
			this.Text = "Tasks Preferences";
			this.Load += new System.EventHandler(this.FormTaskPreferences_Load);
			this.groupBoxComputerDefaults.ResumeLayout(false);
			this.groupBoxComputerDefaults.PerformLayout();
			this.groupBoxDatabase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkTaskSortApptDateTime;
		private System.Windows.Forms.CheckBox checkShowOpenTickets;
		private System.Windows.Forms.CheckBox checkTasksNewTrackedByUser;
		private System.Windows.Forms.GroupBox groupBoxComputerDefaults;
		private System.Windows.Forms.RadioButton radioRight;
		private System.Windows.Forms.RadioButton radioBottom;
		private ValidNum validNumY;
		private System.Windows.Forms.Label labelY;
		private ValidNum validNumX;
		private System.Windows.Forms.Label labelX;
		private System.Windows.Forms.CheckBox checkBoxTaskKeepListHidden;
		private System.Windows.Forms.CheckBox checkTaskListAlwaysShow;
		private System.Windows.Forms.CheckBox checkShowLegacyRepeatingTasks;
		private System.Windows.Forms.GroupBox groupBoxDatabase;
		private UI.Button butTaskInboxSetup;
		private UI.ComboBoxOD comboGlobalFilter;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Label labelGlobalFilter;
	}
}