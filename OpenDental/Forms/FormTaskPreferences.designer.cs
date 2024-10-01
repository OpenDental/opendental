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
			this.butSave = new OpenDental.UI.Button();
			this.checkTaskSortApptDateTime = new OpenDental.UI.CheckBox();
			this.checkShowOpenTickets = new OpenDental.UI.CheckBox();
			this.checkTasksNewTrackedByUser = new OpenDental.UI.CheckBox();
			this.groupBoxComputerDefaults = new OpenDental.UI.GroupBox();
			this.radioRight = new System.Windows.Forms.RadioButton();
			this.radioBottom = new System.Windows.Forms.RadioButton();
			this.validNumY = new OpenDental.ValidNum();
			this.labelY = new System.Windows.Forms.Label();
			this.validNumX = new OpenDental.ValidNum();
			this.labelX = new System.Windows.Forms.Label();
			this.checkBoxTaskKeepListHidden = new OpenDental.UI.CheckBox();
			this.checkTaskListAlwaysShow = new OpenDental.UI.CheckBox();
			this.checkShowLegacyRepeatingTasks = new OpenDental.UI.CheckBox();
			this.groupBoxDatabase = new OpenDental.UI.GroupBox();
			this.checkTaskToApptOneToOne = new OpenDental.UI.CheckBox();
			this.labelImageCategoryFolder = new System.Windows.Forms.Label();
			this.comboImageCategoryFolders = new OpenDental.UI.ComboBox();
			this.labelGlobalFilter = new System.Windows.Forms.Label();
			this.butTaskInboxSetup = new OpenDental.UI.Button();
			this.comboFilterDefault = new OpenDental.UI.ComboBox();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.groupBoxComputerDefaults.SuspendLayout();
			this.groupBoxDatabase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(297, 333);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 12;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkTaskSortApptDateTime
			// 
			this.checkTaskSortApptDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTaskSortApptDateTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTaskSortApptDateTime.Location = new System.Drawing.Point(12, 126);
			this.checkTaskSortApptDateTime.Name = "checkTaskSortApptDateTime";
			this.checkTaskSortApptDateTime.Size = new System.Drawing.Size(342, 14);
			this.checkTaskSortApptDateTime.TabIndex = 6;
			this.checkTaskSortApptDateTime.Text = "Default to sorting appointment type task lists by AptDateTime";
			// 
			// checkShowOpenTickets
			// 
			this.checkShowOpenTickets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowOpenTickets.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowOpenTickets.Location = new System.Drawing.Point(12, 108);
			this.checkShowOpenTickets.Name = "checkShowOpenTickets";
			this.checkShowOpenTickets.Size = new System.Drawing.Size(342, 14);
			this.checkShowOpenTickets.TabIndex = 5;
			this.checkShowOpenTickets.Text = "Show open tasks for user";
			// 
			// checkTasksNewTrackedByUser
			// 
			this.checkTasksNewTrackedByUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTasksNewTrackedByUser.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTasksNewTrackedByUser.Location = new System.Drawing.Point(54, 90);
			this.checkTasksNewTrackedByUser.Name = "checkTasksNewTrackedByUser";
			this.checkTasksNewTrackedByUser.Size = new System.Drawing.Size(300, 14);
			this.checkTasksNewTrackedByUser.TabIndex = 4;
			this.checkTasksNewTrackedByUser.Text = "New/Viewed status tracked by individual user";
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
			this.groupBoxComputerDefaults.Location = new System.Drawing.Point(12, 239);
			this.groupBoxComputerDefaults.Name = "groupBoxComputerDefaults";
			this.groupBoxComputerDefaults.Size = new System.Drawing.Size(360, 86);
			this.groupBoxComputerDefaults.TabIndex = 76;
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
			this.validNumY.ShowZero = false;
			this.validNumY.Size = new System.Drawing.Size(47, 20);
			this.validNumY.TabIndex = 11;
			this.validNumY.Text = "542";
			this.validNumY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
			this.validNumX.ShowZero = false;
			this.validNumX.Size = new System.Drawing.Size(47, 20);
			this.validNumX.TabIndex = 10;
			this.validNumX.Text = "542";
			this.validNumX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
			this.checkBoxTaskKeepListHidden.CheckedChanged += new System.EventHandler(this.checkBoxTaskKeepListHidden_CheckedChanged);
			// 
			// checkTaskListAlwaysShow
			// 
			this.checkTaskListAlwaysShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTaskListAlwaysShow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTaskListAlwaysShow.Location = new System.Drawing.Point(164, 72);
			this.checkTaskListAlwaysShow.Name = "checkTaskListAlwaysShow";
			this.checkTaskListAlwaysShow.Size = new System.Drawing.Size(190, 14);
			this.checkTaskListAlwaysShow.TabIndex = 3;
			this.checkTaskListAlwaysShow.Text = "Always show task list";
			this.checkTaskListAlwaysShow.CheckedChanged += new System.EventHandler(this.checkTaskListAlwaysShow_CheckedChanged);
			// 
			// checkShowLegacyRepeatingTasks
			// 
			this.checkShowLegacyRepeatingTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowLegacyRepeatingTasks.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowLegacyRepeatingTasks.Location = new System.Drawing.Point(124, 54);
			this.checkShowLegacyRepeatingTasks.Name = "checkShowLegacyRepeatingTasks";
			this.checkShowLegacyRepeatingTasks.Size = new System.Drawing.Size(230, 14);
			this.checkShowLegacyRepeatingTasks.TabIndex = 2;
			this.checkShowLegacyRepeatingTasks.Text = "Show legacy repeating tasks";
			// 
			// groupBoxDatabase
			// 
			this.groupBoxDatabase.Controls.Add(this.checkTaskToApptOneToOne);
			this.groupBoxDatabase.Controls.Add(this.labelImageCategoryFolder);
			this.groupBoxDatabase.Controls.Add(this.comboImageCategoryFolders);
			this.groupBoxDatabase.Controls.Add(this.labelGlobalFilter);
			this.groupBoxDatabase.Controls.Add(this.butTaskInboxSetup);
			this.groupBoxDatabase.Controls.Add(this.comboFilterDefault);
			this.groupBoxDatabase.Controls.Add(this.checkShowLegacyRepeatingTasks);
			this.groupBoxDatabase.Controls.Add(this.checkTaskSortApptDateTime);
			this.groupBoxDatabase.Controls.Add(this.checkTaskListAlwaysShow);
			this.groupBoxDatabase.Controls.Add(this.checkShowOpenTickets);
			this.groupBoxDatabase.Controls.Add(this.checkTasksNewTrackedByUser);
			this.groupBoxDatabase.Location = new System.Drawing.Point(12, 12);
			this.groupBoxDatabase.Name = "groupBoxDatabase";
			this.groupBoxDatabase.Size = new System.Drawing.Size(360, 221);
			this.groupBoxDatabase.TabIndex = 80;
			this.groupBoxDatabase.Text = "Global Settings";
			// 
			// checkTaskToApptOneToOne
			// 
			this.checkTaskToApptOneToOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTaskToApptOneToOne.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTaskToApptOneToOne.Location = new System.Drawing.Point(12, 144);
			this.checkTaskToApptOneToOne.Name = "checkTaskToApptOneToOne";
			this.checkTaskToApptOneToOne.Size = new System.Drawing.Size(342, 14);
			this.checkTaskToApptOneToOne.TabIndex = 143;
			this.checkTaskToApptOneToOne.Text = "Allow multiple Tasks attached to one Appointment";
			// 
			// labelImageCategoryFolder
			// 
			this.labelImageCategoryFolder.Location = new System.Drawing.Point(3, 193);
			this.labelImageCategoryFolder.Name = "labelImageCategoryFolder";
			this.labelImageCategoryFolder.Size = new System.Drawing.Size(228, 19);
			this.labelImageCategoryFolder.TabIndex = 142;
			this.labelImageCategoryFolder.Text = "Image category folder for attachments";
			this.labelImageCategoryFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboImageCategoryFolders
			// 
			this.comboImageCategoryFolders.Location = new System.Drawing.Point(234, 193);
			this.comboImageCategoryFolders.Name = "comboImageCategoryFolders";
			this.comboImageCategoryFolders.Size = new System.Drawing.Size(120, 21);
			this.comboImageCategoryFolders.TabIndex = 141;
			// 
			// labelGlobalFilter
			// 
			this.labelGlobalFilter.Location = new System.Drawing.Point(54, 166);
			this.labelGlobalFilter.Name = "labelGlobalFilter";
			this.labelGlobalFilter.Size = new System.Drawing.Size(177, 19);
			this.labelGlobalFilter.TabIndex = 140;
			this.labelGlobalFilter.Text = "Default Filter for Tasks in Lists";
			this.labelGlobalFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelGlobalFilter.Visible = false;
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
			// comboFilterDefault
			// 
			this.comboFilterDefault.Location = new System.Drawing.Point(234, 166);
			this.comboFilterDefault.Name = "comboFilterDefault";
			this.comboFilterDefault.Size = new System.Drawing.Size(120, 21);
			this.comboFilterDefault.TabIndex = 139;
			this.comboFilterDefault.Visible = false;
			this.comboFilterDefault.SelectionChangeCommitted += new System.EventHandler(this.comboGlobalFilter_SelectionChangeCommitted);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// FormTaskPreferences
			// 
			this.ClientSize = new System.Drawing.Size(384, 369);
			this.Controls.Add(this.butSave);
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

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.CheckBox checkTaskSortApptDateTime;
		private OpenDental.UI.CheckBox checkShowOpenTickets;
		private OpenDental.UI.CheckBox checkTasksNewTrackedByUser;
		private OpenDental.UI.GroupBox groupBoxComputerDefaults;
		private System.Windows.Forms.RadioButton radioRight;
		private System.Windows.Forms.RadioButton radioBottom;
		private ValidNum validNumY;
		private System.Windows.Forms.Label labelY;
		private ValidNum validNumX;
		private System.Windows.Forms.Label labelX;
		private OpenDental.UI.CheckBox checkBoxTaskKeepListHidden;
		private OpenDental.UI.CheckBox checkTaskListAlwaysShow;
		private OpenDental.UI.CheckBox checkShowLegacyRepeatingTasks;
		private OpenDental.UI.GroupBox groupBoxDatabase;
		private UI.Button butTaskInboxSetup;
		private UI.ComboBox comboFilterDefault;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Label labelGlobalFilter;
		private System.Windows.Forms.Label labelImageCategoryFolder;
		private UI.ComboBox comboImageCategoryFolders;
		private UI.CheckBox checkTaskToApptOneToOne;
	}
}