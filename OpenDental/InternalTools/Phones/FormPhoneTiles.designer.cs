namespace OpenDental {
	partial class FormPhoneTiles {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
				_bitmapHouse?.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneTiles));
			this.labelMsg = new System.Windows.Forms.Label();
			this.menuNumbers = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemManage = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStatus = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemStatusOnBehalf = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAvailable = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTraining = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTeamAssist = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTCResponder = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemNeedsHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemWrapUp = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemOfflineAssist = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemUnavailable = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemBackup = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemEmployeeSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemRingGroupOnBehalf = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRinggroupAll = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRinggroupNone = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRinggroupsDefault = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRinggroupBackup = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemClockOnBehalf = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemLunch = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemHome = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemBreak = new System.Windows.Forms.ToolStripMenuItem();
			this.checkBoxAll = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioByExt = new System.Windows.Forms.RadioButton();
			this.radioByName = new System.Windows.Forms.RadioButton();
			this.butSettings = new OpenDental.UI.Button();
			this.butConfRooms = new OpenDental.UI.Button();
			this.checkHideClockedOut = new System.Windows.Forms.CheckBox();
			this.timerFlash = new System.Windows.Forms.Timer(this.components);
			this.menuNumbers.SuspendLayout();
			this.menuStatus.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelMsg
			// 
			this.labelMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMsg.ForeColor = System.Drawing.Color.Firebrick;
			this.labelMsg.Location = new System.Drawing.Point(102, 10);
			this.labelMsg.Name = "labelMsg";
			this.labelMsg.Size = new System.Drawing.Size(198, 20);
			this.labelMsg.TabIndex = 27;
			this.labelMsg.Text = "Voice Mails: 0";
			this.labelMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// menuNumbers
			// 
			this.menuNumbers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemManage,
            this.menuItemAdd});
			this.menuNumbers.Name = "contextMenuStrip1";
			this.menuNumbers.Size = new System.Drawing.Size(291, 48);
			// 
			// menuItemManage
			// 
			this.menuItemManage.Name = "menuItemManage";
			this.menuItemManage.Size = new System.Drawing.Size(290, 22);
			this.menuItemManage.Text = "Manage Phone Numbers";
			this.menuItemManage.Click += new System.EventHandler(this.menuItemManage_Click);
			// 
			// menuItemAdd
			// 
			this.menuItemAdd.Name = "menuItemAdd";
			this.menuItemAdd.Size = new System.Drawing.Size(290, 22);
			this.menuItemAdd.Text = "Attach Phone Number to Current Patient";
			this.menuItemAdd.Click += new System.EventHandler(this.menuItemAdd_Click);
			// 
			// menuStatus
			// 
			this.menuStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemStatusOnBehalf,
            this.menuItemAvailable,
            this.menuItemTraining,
            this.menuItemTeamAssist,
            this.menuItemTCResponder,
            this.menuItemNeedsHelp,
            this.menuItemWrapUp,
            this.menuItemOfflineAssist,
            this.menuItemUnavailable,
            this.menuItemBackup,
            this.menuItemEmployeeSettings,
            this.toolStripMenuItem2,
            this.menuItemRingGroupOnBehalf,
            this.menuItemRinggroupAll,
            this.menuItemRinggroupNone,
            this.menuItemRinggroupsDefault,
            this.menuItemRinggroupBackup,
            this.toolStripMenuItem1,
            this.menuItemClockOnBehalf,
            this.menuItemLunch,
            this.menuItemHome,
            this.menuItemBreak});
			this.menuStatus.Name = "menuStatus";
			this.menuStatus.Size = new System.Drawing.Size(246, 456);
			// 
			// menuItemStatusOnBehalf
			// 
			this.menuItemStatusOnBehalf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuItemStatusOnBehalf.Name = "menuItemStatusOnBehalf";
			this.menuItemStatusOnBehalf.Size = new System.Drawing.Size(245, 22);
			this.menuItemStatusOnBehalf.Text = "menuItemStatusOnBehalf";
			// 
			// menuItemAvailable
			// 
			this.menuItemAvailable.Name = "menuItemAvailable";
			this.menuItemAvailable.Size = new System.Drawing.Size(245, 22);
			this.menuItemAvailable.Text = "Available";
			this.menuItemAvailable.Click += new System.EventHandler(this.menuItemAvailable_Click);
			// 
			// menuItemTraining
			// 
			this.menuItemTraining.Name = "menuItemTraining";
			this.menuItemTraining.Size = new System.Drawing.Size(245, 22);
			this.menuItemTraining.Text = "Training";
			this.menuItemTraining.Click += new System.EventHandler(this.menuItemTraining_Click);
			// 
			// menuItemTeamAssist
			// 
			this.menuItemTeamAssist.Name = "menuItemTeamAssist";
			this.menuItemTeamAssist.Size = new System.Drawing.Size(245, 22);
			this.menuItemTeamAssist.Text = "TeamAssist";
			this.menuItemTeamAssist.Click += new System.EventHandler(this.menuItemTeamAssist_Click);
			// 
			// menuItemTCResponder
			// 
			this.menuItemTCResponder.Name = "menuItemTCResponder";
			this.menuItemTCResponder.Size = new System.Drawing.Size(245, 22);
			this.menuItemTCResponder.Text = "TC/Responder";
			this.menuItemTCResponder.Click += new System.EventHandler(this.menuItemTCResponder_Click);
			// 
			// menuItemNeedsHelp
			// 
			this.menuItemNeedsHelp.Name = "menuItemNeedsHelp";
			this.menuItemNeedsHelp.Size = new System.Drawing.Size(245, 22);
			this.menuItemNeedsHelp.Text = "NeedsHelp";
			this.menuItemNeedsHelp.Click += new System.EventHandler(this.menuItemNeedsHelp_Click);
			// 
			// menuItemWrapUp
			// 
			this.menuItemWrapUp.Name = "menuItemWrapUp";
			this.menuItemWrapUp.Size = new System.Drawing.Size(245, 22);
			this.menuItemWrapUp.Text = "WrapUp";
			this.menuItemWrapUp.Click += new System.EventHandler(this.menuItemWrapUp_Click);
			// 
			// menuItemOfflineAssist
			// 
			this.menuItemOfflineAssist.Name = "menuItemOfflineAssist";
			this.menuItemOfflineAssist.Size = new System.Drawing.Size(245, 22);
			this.menuItemOfflineAssist.Text = "OfflineAssist";
			this.menuItemOfflineAssist.Click += new System.EventHandler(this.menuItemOfflineAssist_Click);
			// 
			// menuItemUnavailable
			// 
			this.menuItemUnavailable.Name = "menuItemUnavailable";
			this.menuItemUnavailable.Size = new System.Drawing.Size(245, 22);
			this.menuItemUnavailable.Text = "Unavailable";
			this.menuItemUnavailable.Click += new System.EventHandler(this.menuItemUnavailable_Click);
			// 
			// menuItemBackup
			// 
			this.menuItemBackup.Name = "menuItemBackup";
			this.menuItemBackup.Size = new System.Drawing.Size(245, 22);
			this.menuItemBackup.Text = "Backup";
			this.menuItemBackup.Click += new System.EventHandler(this.menuItemBackup_Click);
			// 
			// menuItemEmployeeSettings
			// 
			this.menuItemEmployeeSettings.Name = "menuItemEmployeeSettings";
			this.menuItemEmployeeSettings.Size = new System.Drawing.Size(245, 22);
			this.menuItemEmployeeSettings.Text = "Employee Settings";
			this.menuItemEmployeeSettings.Click += new System.EventHandler(this.menuItemEmployeeSettings_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(242, 6);
			// 
			// menuItemRingGroupOnBehalf
			// 
			this.menuItemRingGroupOnBehalf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuItemRingGroupOnBehalf.Name = "menuItemRingGroupOnBehalf";
			this.menuItemRingGroupOnBehalf.Size = new System.Drawing.Size(245, 22);
			this.menuItemRingGroupOnBehalf.Text = "menuItemRingGroupOnBehalf";
			// 
			// menuItemRinggroupAll
			// 
			this.menuItemRinggroupAll.Name = "menuItemRinggroupAll";
			this.menuItemRinggroupAll.Size = new System.Drawing.Size(245, 22);
			this.menuItemRinggroupAll.Text = "Queues Tech";
			this.menuItemRinggroupAll.Click += new System.EventHandler(this.menuItemQueueTech_Click);
			// 
			// menuItemRinggroupNone
			// 
			this.menuItemRinggroupNone.Name = "menuItemRinggroupNone";
			this.menuItemRinggroupNone.Size = new System.Drawing.Size(245, 22);
			this.menuItemRinggroupNone.Text = "Queues None";
			this.menuItemRinggroupNone.Click += new System.EventHandler(this.menuItemQueueNone_Click);
			// 
			// menuItemRinggroupsDefault
			// 
			this.menuItemRinggroupsDefault.Name = "menuItemRinggroupsDefault";
			this.menuItemRinggroupsDefault.Size = new System.Drawing.Size(245, 22);
			this.menuItemRinggroupsDefault.Text = "Queues Default";
			this.menuItemRinggroupsDefault.Click += new System.EventHandler(this.menuItemQueueDefault_Click);
			// 
			// menuItemRinggroupBackup
			// 
			this.menuItemRinggroupBackup.Name = "menuItemRinggroupBackup";
			this.menuItemRinggroupBackup.Size = new System.Drawing.Size(245, 22);
			this.menuItemRinggroupBackup.Text = "Queues Backup";
			this.menuItemRinggroupBackup.Click += new System.EventHandler(this.menuItemQueueBackup_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(242, 6);
			// 
			// menuItemClockOnBehalf
			// 
			this.menuItemClockOnBehalf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuItemClockOnBehalf.Name = "menuItemClockOnBehalf";
			this.menuItemClockOnBehalf.Size = new System.Drawing.Size(245, 22);
			this.menuItemClockOnBehalf.Text = "menuItemClockOnBehalf";
			// 
			// menuItemLunch
			// 
			this.menuItemLunch.Name = "menuItemLunch";
			this.menuItemLunch.Size = new System.Drawing.Size(245, 22);
			this.menuItemLunch.Text = "Lunch";
			this.menuItemLunch.Click += new System.EventHandler(this.menuItemLunch_Click);
			// 
			// menuItemHome
			// 
			this.menuItemHome.Name = "menuItemHome";
			this.menuItemHome.Size = new System.Drawing.Size(245, 22);
			this.menuItemHome.Text = "Home";
			this.menuItemHome.Click += new System.EventHandler(this.menuItemHome_Click);
			// 
			// menuItemBreak
			// 
			this.menuItemBreak.Name = "menuItemBreak";
			this.menuItemBreak.Size = new System.Drawing.Size(245, 22);
			this.menuItemBreak.Text = "Break";
			this.menuItemBreak.Click += new System.EventHandler(this.menuItemBreak_Click);
			// 
			// checkBoxAll
			// 
			this.checkBoxAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxAll.Location = new System.Drawing.Point(1624, 5);
			this.checkBoxAll.Name = "checkBoxAll";
			this.checkBoxAll.Size = new System.Drawing.Size(128, 16);
			this.checkBoxAll.TabIndex = 28;
			this.checkBoxAll.Text = "Show All";
			this.checkBoxAll.UseVisualStyleBackColor = true;
			this.checkBoxAll.Click += new System.EventHandler(this.checkBoxAll_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioByExt);
			this.groupBox1.Controls.Add(this.radioByName);
			this.groupBox1.Location = new System.Drawing.Point(325, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(198, 40);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sort By:";
			// 
			// radioByExt
			// 
			this.radioByExt.AutoSize = true;
			this.radioByExt.Location = new System.Drawing.Point(107, 17);
			this.radioByExt.Name = "radioByExt";
			this.radioByExt.Size = new System.Drawing.Size(71, 17);
			this.radioByExt.TabIndex = 1;
			this.radioByExt.Text = "Extension";
			this.radioByExt.UseVisualStyleBackColor = true;
			// 
			// radioByName
			// 
			this.radioByName.AutoSize = true;
			this.radioByName.Checked = true;
			this.radioByName.Location = new System.Drawing.Point(24, 17);
			this.radioByName.Name = "radioByName";
			this.radioByName.Size = new System.Drawing.Size(53, 17);
			this.radioByName.TabIndex = 0;
			this.radioByName.TabStop = true;
			this.radioByName.Text = "Name";
			this.radioByName.UseVisualStyleBackColor = true;
			// 
			// butSettings
			// 
			this.butSettings.Location = new System.Drawing.Point(0, 8);
			this.butSettings.Name = "butSettings";
			this.butSettings.Size = new System.Drawing.Size(75, 24);
			this.butSettings.TabIndex = 26;
			this.butSettings.Text = "Settings";
			this.butSettings.Click += new System.EventHandler(this.butSettings_Click);
			// 
			// butConfRooms
			// 
			this.butConfRooms.Location = new System.Drawing.Point(672, 8);
			this.butConfRooms.Name = "butConfRooms";
			this.butConfRooms.Size = new System.Drawing.Size(89, 24);
			this.butConfRooms.TabIndex = 30;
			this.butConfRooms.Text = "Conf Rooms";
			this.butConfRooms.Click += new System.EventHandler(this.butConfRooms_Click);
			// 
			// checkHideClockedOut
			// 
			this.checkHideClockedOut.Location = new System.Drawing.Point(542, 10);
			this.checkHideClockedOut.Name = "checkHideClockedOut";
			this.checkHideClockedOut.Size = new System.Drawing.Size(128, 16);
			this.checkHideClockedOut.TabIndex = 31;
			this.checkHideClockedOut.Text = "Hide clocked out";
			this.checkHideClockedOut.UseVisualStyleBackColor = true;
			this.checkHideClockedOut.CheckedChanged += new System.EventHandler(this.checkHideClockedOut_CheckedChanged);
			// 
			// timerFlash
			// 
			this.timerFlash.Enabled = true;
			this.timerFlash.Interval = 300;
			this.timerFlash.Tick += new System.EventHandler(this.timerFlash_Tick);
			// 
			// FormPhoneTiles
			// 
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1764, 987);
			this.Controls.Add(this.checkHideClockedOut);
			this.Controls.Add(this.butConfRooms);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkBoxAll);
			this.Controls.Add(this.labelMsg);
			this.Controls.Add(this.butSettings);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(10, 10);
			this.Name = "FormPhoneTiles";
			this.Text = "Big Phones";
			this.Load += new System.EventHandler(this.FormPhoneTiles_Load);
			this.Shown += new System.EventHandler(this.FormPhoneTiles_Shown);
			this.menuNumbers.ResumeLayout(false);
			this.menuStatus.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelMsg;
		private OpenDental.UI.Button butSettings;
		private System.Windows.Forms.ContextMenuStrip menuNumbers;
		private System.Windows.Forms.ToolStripMenuItem menuItemManage;
		private System.Windows.Forms.ToolStripMenuItem menuItemAdd;
		private System.Windows.Forms.ContextMenuStrip menuStatus;
		private System.Windows.Forms.ToolStripMenuItem menuItemAvailable;
		private System.Windows.Forms.ToolStripMenuItem menuItemTraining;
		private System.Windows.Forms.ToolStripMenuItem menuItemTeamAssist;
		private System.Windows.Forms.ToolStripMenuItem menuItemWrapUp;
		private System.Windows.Forms.ToolStripMenuItem menuItemOfflineAssist;
		private System.Windows.Forms.ToolStripMenuItem menuItemUnavailable;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupAll;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupNone;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupsDefault;
		private System.Windows.Forms.ToolStripMenuItem menuItemBackup;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem menuItemLunch;
		private System.Windows.Forms.ToolStripMenuItem menuItemHome;
		private System.Windows.Forms.ToolStripMenuItem menuItemBreak;
		private System.Windows.Forms.CheckBox checkBoxAll;
		private System.Windows.Forms.ToolStripMenuItem menuItemNeedsHelp;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioByExt;
		private System.Windows.Forms.RadioButton radioByName;
		private System.Windows.Forms.ToolStripMenuItem menuItemStatusOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemRingGroupOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemClockOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupBackup;
		private UI.Button butConfRooms;
		private System.Windows.Forms.ToolStripMenuItem menuItemTCResponder;
		private System.Windows.Forms.ToolStripMenuItem menuItemEmployeeSettings;
		private System.Windows.Forms.CheckBox checkHideClockedOut;
		private System.Windows.Forms.Timer timerFlash;
	}
}