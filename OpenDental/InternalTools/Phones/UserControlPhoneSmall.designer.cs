namespace OpenDental {
	partial class UserControlPhoneSmall {
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
			this.components = new System.ComponentModel.Container();
			this.menuStatus = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemStatusOnBehalf = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAvailable = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAvailableHome = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTraining = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTeamAssist = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTCResponder = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemNeedsHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemWrapUp = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemOfflineAssist = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemUnavailable = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemBackup = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparatorRingGroups = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemRingGroupOnBehalf = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRinggroupAll = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRinggroupNone = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRinggroupsDefault = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRinggroupsBackup = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparatorClockEvents = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemClockOnBehalf = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemLunch = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemHome = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemBreak = new System.Windows.Forms.ToolStripMenuItem();
			this.menuNumbers = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemManage = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.phoneTile = new OpenDental.PhoneTile();
			this.labelConfRoom = new System.Windows.Forms.Label();
			this.butGetConfRoom = new OpenDental.UI.Button();
			this.timerConfRoom = new System.Windows.Forms.Timer(this.components);
			this.butConfRooms = new OpenDental.UI.Button();
			this.menuStatus.SuspendLayout();
			this.menuNumbers.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStatus
			// 
			this.menuStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemStatusOnBehalf,
            this.menuItemAvailable,
            this.menuItemAvailableHome,
            this.menuItemTraining,
            this.menuItemTeamAssist,
            this.menuItemTCResponder,
            this.menuItemNeedsHelp,
            this.menuItemWrapUp,
            this.menuItemOfflineAssist,
            this.menuItemUnavailable,
            this.menuItemBackup,
            this.toolStripSeparatorRingGroups,
            this.menuItemRingGroupOnBehalf,
            this.menuItemRinggroupAll,
            this.menuItemRinggroupNone,
            this.menuItemRinggroupsDefault,
            this.menuItemRinggroupsBackup,
            this.toolStripSeparatorClockEvents,
            this.menuItemClockOnBehalf,
            this.menuItemLunch,
            this.menuItemHome,
            this.menuItemBreak});
			this.menuStatus.Name = "menuStatus";
			this.menuStatus.Size = new System.Drawing.Size(215, 456);
			// 
			// menuItemStatusOnBehalf
			// 
			this.menuItemStatusOnBehalf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuItemStatusOnBehalf.Name = "menuItemStatusOnBehalf";
			this.menuItemStatusOnBehalf.Size = new System.Drawing.Size(214, 22);
			this.menuItemStatusOnBehalf.Text = "Status On Behalf Of";
			// 
			// menuItemAvailable
			// 
			this.menuItemAvailable.Name = "menuItemAvailable";
			this.menuItemAvailable.Size = new System.Drawing.Size(214, 22);
			this.menuItemAvailable.Text = "Available At Office";
			this.menuItemAvailable.Click += new System.EventHandler(this.menuItemAvailable_Click);
			// 
			// menuItemAvailableHome
			// 
			this.menuItemAvailableHome.Name = "menuItemAvailableHome";
			this.menuItemAvailableHome.Size = new System.Drawing.Size(214, 22);
			this.menuItemAvailableHome.Text = "Available At Home";
			this.menuItemAvailableHome.Click += new System.EventHandler(this.menuItemAvailableHome_Click);
			// 
			// menuItemTraining
			// 
			this.menuItemTraining.Name = "menuItemTraining";
			this.menuItemTraining.Size = new System.Drawing.Size(214, 22);
			this.menuItemTraining.Text = "Training";
			this.menuItemTraining.Click += new System.EventHandler(this.menuItemTraining_Click);
			// 
			// menuItemTeamAssist
			// 
			this.menuItemTeamAssist.Name = "menuItemTeamAssist";
			this.menuItemTeamAssist.Size = new System.Drawing.Size(214, 22);
			this.menuItemTeamAssist.Text = "TeamAssist";
			this.menuItemTeamAssist.Click += new System.EventHandler(this.menuItemTeamAssist_Click);
			// 
			// menuItemTCResponder
			// 
			this.menuItemTCResponder.Name = "menuItemTCResponder";
			this.menuItemTCResponder.Size = new System.Drawing.Size(214, 22);
			this.menuItemTCResponder.Text = "TC/Responder";
			this.menuItemTCResponder.Click += new System.EventHandler(this.menuItemTCResponder_Click);
			// 
			// menuItemNeedsHelp
			// 
			this.menuItemNeedsHelp.Name = "menuItemNeedsHelp";
			this.menuItemNeedsHelp.Size = new System.Drawing.Size(214, 22);
			this.menuItemNeedsHelp.Text = "NeedsHelp";
			this.menuItemNeedsHelp.Click += new System.EventHandler(this.menuItemNeedsHelp_Click);
			// 
			// menuItemWrapUp
			// 
			this.menuItemWrapUp.Name = "menuItemWrapUp";
			this.menuItemWrapUp.Size = new System.Drawing.Size(214, 22);
			this.menuItemWrapUp.Text = "WrapUp";
			this.menuItemWrapUp.Click += new System.EventHandler(this.menuItemWrapUp_Click);
			// 
			// menuItemOfflineAssist
			// 
			this.menuItemOfflineAssist.Name = "menuItemOfflineAssist";
			this.menuItemOfflineAssist.Size = new System.Drawing.Size(214, 22);
			this.menuItemOfflineAssist.Text = "OfflineAssist";
			this.menuItemOfflineAssist.Click += new System.EventHandler(this.menuItemOfflineAssist_Click);
			// 
			// menuItemUnavailable
			// 
			this.menuItemUnavailable.Name = "menuItemUnavailable";
			this.menuItemUnavailable.Size = new System.Drawing.Size(214, 22);
			this.menuItemUnavailable.Text = "Unavailable";
			this.menuItemUnavailable.Click += new System.EventHandler(this.menuItemUnavailable_Click);
			// 
			// menuItemBackup
			// 
			this.menuItemBackup.Name = "menuItemBackup";
			this.menuItemBackup.Size = new System.Drawing.Size(214, 22);
			this.menuItemBackup.Text = "Backup";
			this.menuItemBackup.Click += new System.EventHandler(this.menuItemBackup_Click);
			// 
			// toolStripSeparatorRingGroups
			// 
			this.toolStripSeparatorRingGroups.Name = "toolStripSeparatorRingGroups";
			this.toolStripSeparatorRingGroups.Size = new System.Drawing.Size(211, 6);
			// 
			// menuItemRingGroupOnBehalf
			// 
			this.menuItemRingGroupOnBehalf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuItemRingGroupOnBehalf.Name = "menuItemRingGroupOnBehalf";
			this.menuItemRingGroupOnBehalf.Size = new System.Drawing.Size(214, 22);
			this.menuItemRingGroupOnBehalf.Text = "Queue On Behalf Of";
			// 
			// menuItemRinggroupAll
			// 
			this.menuItemRinggroupAll.Name = "menuItemRinggroupAll";
			this.menuItemRinggroupAll.Size = new System.Drawing.Size(214, 22);
			this.menuItemRinggroupAll.Text = "Queue Tech";
			this.menuItemRinggroupAll.Click += new System.EventHandler(this.menuItemQueueTech_Click);
			// 
			// menuItemRinggroupNone
			// 
			this.menuItemRinggroupNone.Name = "menuItemRinggroupNone";
			this.menuItemRinggroupNone.Size = new System.Drawing.Size(214, 22);
			this.menuItemRinggroupNone.Text = "Queue None";
			this.menuItemRinggroupNone.Click += new System.EventHandler(this.menuItemQueueNone_Click);
			// 
			// menuItemRinggroupsDefault
			// 
			this.menuItemRinggroupsDefault.Name = "menuItemRinggroupsDefault";
			this.menuItemRinggroupsDefault.Size = new System.Drawing.Size(214, 22);
			this.menuItemRinggroupsDefault.Text = "Queue Default";
			this.menuItemRinggroupsDefault.Click += new System.EventHandler(this.menuItemQueueDefault_Click);
			// 
			// menuItemRinggroupsBackup
			// 
			this.menuItemRinggroupsBackup.Name = "menuItemRinggroupsBackup";
			this.menuItemRinggroupsBackup.Size = new System.Drawing.Size(214, 22);
			this.menuItemRinggroupsBackup.Text = "Queue Backup";
			this.menuItemRinggroupsBackup.Click += new System.EventHandler(this.menuItemQueueBackup_Click);
			// 
			// toolStripSeparatorClockEvents
			// 
			this.toolStripSeparatorClockEvents.Name = "toolStripSeparatorClockEvents";
			this.toolStripSeparatorClockEvents.Size = new System.Drawing.Size(211, 6);
			// 
			// menuItemClockOnBehalf
			// 
			this.menuItemClockOnBehalf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuItemClockOnBehalf.Name = "menuItemClockOnBehalf";
			this.menuItemClockOnBehalf.Size = new System.Drawing.Size(214, 22);
			this.menuItemClockOnBehalf.Text = "Clock Event On Behalf Of";
			// 
			// menuItemLunch
			// 
			this.menuItemLunch.Name = "menuItemLunch";
			this.menuItemLunch.Size = new System.Drawing.Size(214, 22);
			this.menuItemLunch.Text = "Lunch";
			this.menuItemLunch.Click += new System.EventHandler(this.menuItemLunch_Click);
			// 
			// menuItemHome
			// 
			this.menuItemHome.Name = "menuItemHome";
			this.menuItemHome.Size = new System.Drawing.Size(214, 22);
			this.menuItemHome.Text = "Clock Out";
			this.menuItemHome.Click += new System.EventHandler(this.menuItemHome_Click);
			// 
			// menuItemBreak
			// 
			this.menuItemBreak.Name = "menuItemBreak";
			this.menuItemBreak.Size = new System.Drawing.Size(214, 22);
			this.menuItemBreak.Text = "Break";
			this.menuItemBreak.Click += new System.EventHandler(this.menuItemBreak_Click);
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
			// phoneTile
			// 
			this.phoneTile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.phoneTile.LayoutHorizontal = false;
			this.phoneTile.Location = new System.Drawing.Point(0, 7);
			this.phoneTile.Name = "phoneTile";
			this.phoneTile.Size = new System.Drawing.Size(213, 81);
			this.phoneTile.TabIndex = 0;
			// 
			// labelConfRoom
			// 
			this.labelConfRoom.Location = new System.Drawing.Point(3, 121);
			this.labelConfRoom.Name = "labelConfRoom";
			this.labelConfRoom.Size = new System.Drawing.Size(207, 37);
			this.labelConfRoom.TabIndex = 3;
			this.labelConfRoom.Text = "Reserved Conf Room:";
			this.labelConfRoom.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.labelConfRoom.Visible = false;
			// 
			// butGetConfRoom
			// 
			this.butGetConfRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butGetConfRoom.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butGetConfRoom.Location = new System.Drawing.Point(6, 94);
			this.butGetConfRoom.Name = "butGetConfRoom";
			this.butGetConfRoom.Size = new System.Drawing.Size(107, 24);
			this.butGetConfRoom.TabIndex = 4;
			this.butGetConfRoom.Text = "Get Conf Room";
			this.butGetConfRoom.Click += new System.EventHandler(this.butGetConfRoom_Click);
			// 
			// timerConfRoom
			// 
			this.timerConfRoom.Interval = 1000;
			this.timerConfRoom.Tick += new System.EventHandler(this.timerConfRoom_Tick);
			// 
			// butConfRooms
			// 
			this.butConfRooms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butConfRooms.Location = new System.Drawing.Point(119, 94);
			this.butConfRooms.Name = "butConfRooms";
			this.butConfRooms.Size = new System.Drawing.Size(89, 24);
			this.butConfRooms.TabIndex = 32;
			this.butConfRooms.Text = "Conf Rooms";
			this.butConfRooms.Click += new System.EventHandler(this.butConfRooms_Click);
			// 
			// UserControlPhoneSmall
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butConfRooms);
			this.Controls.Add(this.butGetConfRoom);
			this.Controls.Add(this.labelConfRoom);
			this.Controls.Add(this.phoneTile);
			this.DoubleBuffered = true;
			this.Name = "UserControlPhoneSmall";
			this.Size = new System.Drawing.Size(213, 250);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.UserControlPhoneSmall_Paint);
			this.menuStatus.ResumeLayout(false);
			this.menuNumbers.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private PhoneTile phoneTile;
		private System.Windows.Forms.ContextMenuStrip menuStatus;
		private System.Windows.Forms.ToolStripMenuItem menuItemAvailable;
		private System.Windows.Forms.ToolStripMenuItem menuItemTraining;
		private System.Windows.Forms.ToolStripMenuItem menuItemTeamAssist;
		private System.Windows.Forms.ToolStripMenuItem menuItemWrapUp;
		private System.Windows.Forms.ToolStripMenuItem menuItemOfflineAssist;
		private System.Windows.Forms.ToolStripMenuItem menuItemUnavailable;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparatorRingGroups;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupAll;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupNone;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupsDefault;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupsBackup;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparatorClockEvents;
		private System.Windows.Forms.ToolStripMenuItem menuItemLunch;
		private System.Windows.Forms.ToolStripMenuItem menuItemHome;
		private System.Windows.Forms.ToolStripMenuItem menuItemBreak;
		private System.Windows.Forms.ContextMenuStrip menuNumbers;
		private System.Windows.Forms.ToolStripMenuItem menuItemManage;
		private System.Windows.Forms.ToolStripMenuItem menuItemAdd;
		private System.Windows.Forms.ToolStripMenuItem menuItemNeedsHelp;
		private System.Windows.Forms.ToolStripMenuItem menuItemStatusOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemRingGroupOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemClockOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemBackup;
		private System.Windows.Forms.Label labelConfRoom;
		private UI.Button butGetConfRoom;
		private System.Windows.Forms.Timer timerConfRoom;
		private System.Windows.Forms.ToolStripMenuItem menuItemTCResponder;
		private UI.Button butConfRooms;
		private System.Windows.Forms.ToolStripMenuItem menuItemAvailableHome;
	}
}
