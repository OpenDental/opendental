namespace OpenDental {
	partial class MapAreaRoomControl {
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
				ProxImage?.Dispose();
				ChatImage?.Dispose();
				WebChatImage?.Dispose();
				RemoteSupportImage?.Dispose();
				PhoneImage?.Dispose();
				BackgroundImage?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.timerFlash = new System.Windows.Forms.Timer(this.components);
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
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemCustomer = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemGoTo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemEmployee = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemEmployeeSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStatus.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerFlash
			// 
			this.timerFlash.Interval = 300;
			this.timerFlash.Tick += new System.EventHandler(this.timerFlash_Tick);
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
            this.menuItemBreak,
            this.toolStripMenuItem3,
            this.menuItemCustomer,
            this.menuItemGoTo,
            this.toolStripMenuItem4,
            this.menuItemEmployee,
            this.menuItemEmployeeSettings});
			this.menuStatus.Name = "menuStatus";
			this.menuStatus.Size = new System.Drawing.Size(246, 556);
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
			this.menuItemRinggroupAll.Click += new System.EventHandler(this.menuItemRinggroupAll_Click);
			// 
			// menuItemRinggroupNone
			// 
			this.menuItemRinggroupNone.Name = "menuItemRinggroupNone";
			this.menuItemRinggroupNone.Size = new System.Drawing.Size(245, 22);
			this.menuItemRinggroupNone.Text = "Queues None";
			this.menuItemRinggroupNone.Click += new System.EventHandler(this.menuItemRinggroupNone_Click);
			// 
			// menuItemRinggroupsDefault
			// 
			this.menuItemRinggroupsDefault.Name = "menuItemRinggroupsDefault";
			this.menuItemRinggroupsDefault.Size = new System.Drawing.Size(245, 22);
			this.menuItemRinggroupsDefault.Text = "Queues Default";
			this.menuItemRinggroupsDefault.Click += new System.EventHandler(this.menuItemRinggroupsDefault_Click);
			// 
			// menuItemRinggroupBackup
			// 
			this.menuItemRinggroupBackup.Name = "menuItemRinggroupBackup";
			this.menuItemRinggroupBackup.Size = new System.Drawing.Size(245, 22);
			this.menuItemRinggroupBackup.Text = "Queues Backup";
			this.menuItemRinggroupBackup.Click += new System.EventHandler(this.menuItemRinggroupBackup_Click);
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
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(242, 6);
			// 
			// menuItemCustomer
			// 
			this.menuItemCustomer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuItemCustomer.Name = "menuItemCustomer";
			this.menuItemCustomer.Size = new System.Drawing.Size(245, 22);
			this.menuItemCustomer.Text = "menuItemCustomer";
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.Name = "menuItemGoTo";
			this.menuItemGoTo.Size = new System.Drawing.Size(245, 22);
			this.menuItemGoTo.Text = "Go To Customer";
			this.menuItemGoTo.Click += new System.EventHandler(this.menuItemGoTo_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(242, 6);
			// 
			// menuItemEmployee
			// 
			this.menuItemEmployee.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuItemEmployee.Name = "menuItemEmployee";
			this.menuItemEmployee.Size = new System.Drawing.Size(245, 22);
			this.menuItemEmployee.Text = "menuItemEmployee";
			// 
			// menuItemEmployeeSettings
			// 
			this.menuItemEmployeeSettings.Name = "menuItemEmployeeSettings";
			this.menuItemEmployeeSettings.Size = new System.Drawing.Size(245, 22);
			this.menuItemEmployeeSettings.Text = "Employee Settings";
			this.menuItemEmployeeSettings.Click += new System.EventHandler(this.menuItemEmployeeSettings_Click);
			// 
			// MapAreaRoomControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "MapAreaRoomControl";
			this.Size = new System.Drawing.Size(180, 163);
			this.Click += new System.EventHandler(this.MapAreaRoomControl_Click);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MapAreaRoomControl_Paint);
			this.DoubleClick += new System.EventHandler(this.MapAreaRoomControl_DoubleClick);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapAreaRoomControl_MouseDown);
			this.menuStatus.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timerFlash;
		private System.Windows.Forms.ContextMenuStrip menuStatus;
		private System.Windows.Forms.ToolStripMenuItem menuItemStatusOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemAvailable;
		private System.Windows.Forms.ToolStripMenuItem menuItemTraining;
		private System.Windows.Forms.ToolStripMenuItem menuItemTeamAssist;
		private System.Windows.Forms.ToolStripMenuItem menuItemTCResponder;
		private System.Windows.Forms.ToolStripMenuItem menuItemNeedsHelp;
		private System.Windows.Forms.ToolStripMenuItem menuItemWrapUp;
		private System.Windows.Forms.ToolStripMenuItem menuItemOfflineAssist;
		private System.Windows.Forms.ToolStripMenuItem menuItemUnavailable;
		private System.Windows.Forms.ToolStripMenuItem menuItemBackup;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem menuItemRingGroupOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupAll;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupNone;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupsDefault;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupBackup;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem menuItemClockOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemLunch;
		private System.Windows.Forms.ToolStripMenuItem menuItemHome;
		private System.Windows.Forms.ToolStripMenuItem menuItemBreak;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem menuItemCustomer;
		private System.Windows.Forms.ToolStripMenuItem menuItemGoTo;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem menuItemEmployee;
		private System.Windows.Forms.ToolStripMenuItem menuItemEmployeeSettings;
	}
}
