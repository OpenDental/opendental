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
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.radioByExt = new System.Windows.Forms.RadioButton();
			this.radioByName = new System.Windows.Forms.RadioButton();
			this.butSettings = new OpenDental.UI.Button();
			this.butConfRooms = new OpenDental.UI.Button();
			this.checkHideClockedOut = new OpenDental.UI.CheckBox();
			this.timerFlash = new System.Windows.Forms.Timer(this.components);
			this.checkHideOnBreak = new OpenDental.UI.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkShowOldInterface = new OpenDental.UI.CheckBox();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.labelSearch = new System.Windows.Forms.Label();
			this.checkNeedsHelpTop = new OpenDental.UI.CheckBox();
			this.panelGrid = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxStatus = new OpenDental.UI.ListBoxOD();
			this.panelGrid2 = new System.Windows.Forms.Panel();
			this.label5 = new System.Windows.Forms.Label();
			this.butEditDefaults = new OpenDental.UI.Button();
			this.butGotoPatient = new OpenDental.UI.Button();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butPhoneManage = new OpenDental.UI.Button();
			this.butPhoneAttach = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.listBoxClockOut = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxQueues = new OpenDental.UI.ListBoxOD();
			this.label6 = new System.Windows.Forms.Label();
			this.menuNumbers.SuspendLayout();
			this.menuStatus.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panelGrid.SuspendLayout();
			this.panelGrid2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelMsg
			// 
			this.labelMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMsg.ForeColor = System.Drawing.Color.Firebrick;
			this.labelMsg.Location = new System.Drawing.Point(81, 10);
			this.labelMsg.Name = "labelMsg";
			this.labelMsg.Size = new System.Drawing.Size(128, 20);
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
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioByExt);
			this.groupBox1.Controls.Add(this.radioByName);
			this.groupBox1.Location = new System.Drawing.Point(215, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(188, 40);
			this.groupBox1.TabIndex = 29;
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
			this.butConfRooms.Location = new System.Drawing.Point(800, 8);
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
			this.checkHideClockedOut.CheckedChanged += new System.EventHandler(this.checkHideClockedOut_CheckedChanged);
			// 
			// timerFlash
			// 
			this.timerFlash.Interval = 300;
			this.timerFlash.Tick += new System.EventHandler(this.timerFlash_Tick);
			// 
			// checkHideOnBreak
			// 
			this.checkHideOnBreak.Location = new System.Drawing.Point(670, 10);
			this.checkHideOnBreak.Name = "checkHideOnBreak";
			this.checkHideOnBreak.Size = new System.Drawing.Size(128, 16);
			this.checkHideOnBreak.TabIndex = 32;
			this.checkHideOnBreak.Text = "Hide on break";
			this.checkHideOnBreak.CheckedChanged += new System.EventHandler(this.checkHideOnBreak_CheckedChanged);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.HasAlternateRowsColored = true;
			this.gridMain.Location = new System.Drawing.Point(280, 46);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(759, 638);
			this.gridMain.TabIndex = 33;
			this.gridMain.TranslationName = "TableBigPhones";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// checkShowOldInterface
			// 
			this.checkShowOldInterface.Location = new System.Drawing.Point(916, 10);
			this.checkShowOldInterface.Name = "checkShowOldInterface";
			this.checkShowOldInterface.Size = new System.Drawing.Size(229, 16);
			this.checkShowOldInterface.TabIndex = 34;
			this.checkShowOldInterface.Text = "Show old interface (soon deprecated)";
			this.checkShowOldInterface.Click += new System.EventHandler(this.checkShowOldInterface_Click);
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(124, 3);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(147, 20);
			this.textSearch.TabIndex = 0;
			// 
			// labelSearch
			// 
			this.labelSearch.Location = new System.Drawing.Point(33, 2);
			this.labelSearch.Name = "labelSearch";
			this.labelSearch.Size = new System.Drawing.Size(87, 20);
			this.labelSearch.TabIndex = 35;
			this.labelSearch.Text = "Search";
			this.labelSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkNeedsHelpTop
			// 
			this.checkNeedsHelpTop.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNeedsHelpTop.Checked = true;
			this.checkNeedsHelpTop.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkNeedsHelpTop.Location = new System.Drawing.Point(9, 29);
			this.checkNeedsHelpTop.Name = "checkNeedsHelpTop";
			this.checkNeedsHelpTop.Size = new System.Drawing.Size(128, 16);
			this.checkNeedsHelpTop.TabIndex = 36;
			this.checkNeedsHelpTop.Text = "Needs Help at Top";
			this.checkNeedsHelpTop.Click += new System.EventHandler(this.checkNeedsHelpTop_Click);
			// 
			// panelGrid
			// 
			this.panelGrid.Controls.Add(this.textSearch);
			this.panelGrid.Controls.Add(this.labelSearch);
			this.panelGrid.Controls.Add(this.checkNeedsHelpTop);
			this.panelGrid.Location = new System.Drawing.Point(0, 43);
			this.panelGrid.Name = "panelGrid";
			this.panelGrid.Size = new System.Drawing.Size(274, 426);
			this.panelGrid.TabIndex = 37;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 18);
			this.label1.TabIndex = 38;
			this.label1.Text = "Change Status";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listBoxStatus
			// 
			this.listBoxStatus.ItemStrings = new string[] {
        "Available",
        "Training",
        "TeamAssist",
        "TC/Responder",
        "NeedsHelp",
        "WrapUp",
        "OfflineAssist",
        "Unavailable",
        "Backup"};
			this.listBoxStatus.Location = new System.Drawing.Point(101, 3);
			this.listBoxStatus.Name = "listBoxStatus";
			this.listBoxStatus.Size = new System.Drawing.Size(120, 121);
			this.listBoxStatus.TabIndex = 37;
			this.listBoxStatus.Text = "listBox1";
			this.listBoxStatus.SelectionChangeCommitted += new System.EventHandler(this.listBoxStatus_SelectionChangeCommitted);
			// 
			// panelGrid2
			// 
			this.panelGrid2.Controls.Add(this.label6);
			this.panelGrid2.Controls.Add(this.label5);
			this.panelGrid2.Controls.Add(this.butEditDefaults);
			this.panelGrid2.Controls.Add(this.butGotoPatient);
			this.panelGrid2.Controls.Add(this.groupBox2);
			this.panelGrid2.Controls.Add(this.label3);
			this.panelGrid2.Controls.Add(this.listBoxClockOut);
			this.panelGrid2.Controls.Add(this.label2);
			this.panelGrid2.Controls.Add(this.listBoxQueues);
			this.panelGrid2.Controls.Add(this.label1);
			this.panelGrid2.Controls.Add(this.listBoxStatus);
			this.panelGrid2.Location = new System.Drawing.Point(1045, 44);
			this.panelGrid2.Name = "panelGrid2";
			this.panelGrid2.Size = new System.Drawing.Size(234, 456);
			this.panelGrid2.TabIndex = 38;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(122, 398);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(108, 42);
			this.label5.TabIndex = 46;
			this.label5.Text = "for Employee\r\n(or double click\r\nany other col)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butEditDefaults
			// 
			this.butEditDefaults.Location = new System.Drawing.Point(36, 408);
			this.butEditDefaults.Name = "butEditDefaults";
			this.butEditDefaults.Size = new System.Drawing.Size(82, 24);
			this.butEditDefaults.TabIndex = 46;
			this.butEditDefaults.Text = "Edit Defaults";
			this.butEditDefaults.Click += new System.EventHandler(this.butEditDefaults_Click);
			// 
			// butGotoPatient
			// 
			this.butGotoPatient.Location = new System.Drawing.Point(36, 361);
			this.butGotoPatient.Name = "butGotoPatient";
			this.butGotoPatient.Size = new System.Drawing.Size(82, 24);
			this.butGotoPatient.TabIndex = 46;
			this.butGotoPatient.Text = "Go to Patient";
			this.butGotoPatient.Click += new System.EventHandler(this.butGotoPatient_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.butPhoneManage);
			this.groupBox2.Controls.Add(this.butPhoneAttach);
			this.groupBox2.Location = new System.Drawing.Point(14, 267);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(207, 86);
			this.groupBox2.TabIndex = 44;
			this.groupBox2.Text = "Phone Numbers";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(100, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(92, 18);
			this.label4.TabIndex = 45;
			this.label4.Text = "to Current Patient";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butPhoneManage
			// 
			this.butPhoneManage.Location = new System.Drawing.Point(22, 24);
			this.butPhoneManage.Name = "butPhoneManage";
			this.butPhoneManage.Size = new System.Drawing.Size(72, 24);
			this.butPhoneManage.TabIndex = 39;
			this.butPhoneManage.Text = "Manage";
			this.butPhoneManage.Click += new System.EventHandler(this.butPhoneManage_Click);
			// 
			// butPhoneAttach
			// 
			this.butPhoneAttach.Location = new System.Drawing.Point(22, 53);
			this.butPhoneAttach.Name = "butPhoneAttach";
			this.butPhoneAttach.Size = new System.Drawing.Size(72, 24);
			this.butPhoneAttach.TabIndex = 43;
			this.butPhoneAttach.Text = "Attach";
			this.butPhoneAttach.Click += new System.EventHandler(this.butPhoneAttach_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 204);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(92, 18);
			this.label3.TabIndex = 42;
			this.label3.Text = "Clock out for";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listBoxClockOut
			// 
			this.listBoxClockOut.ItemStrings = new string[] {
        "Home",
        "Lunch",
        "Break"};
			this.listBoxClockOut.Location = new System.Drawing.Point(101, 203);
			this.listBoxClockOut.Name = "listBoxClockOut";
			this.listBoxClockOut.Size = new System.Drawing.Size(120, 43);
			this.listBoxClockOut.TabIndex = 41;
			this.listBoxClockOut.Text = "listBox1";
			this.listBoxClockOut.SelectionChangeCommitted += new System.EventHandler(this.listBoxClockOut_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 137);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 18);
			this.label2.TabIndex = 40;
			this.label2.Text = "Queues";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listBoxQueues
			// 
			this.listBoxQueues.ItemStrings = new string[] {
        "Tech",
        "None",
        "Default",
        "Backup"};
			this.listBoxQueues.Location = new System.Drawing.Point(101, 136);
			this.listBoxQueues.Name = "listBoxQueues";
			this.listBoxQueues.Size = new System.Drawing.Size(120, 56);
			this.listBoxQueues.TabIndex = 39;
			this.listBoxQueues.Text = "listBox1";
			this.listBoxQueues.SelectionChangeCommitted += new System.EventHandler(this.listBoxQueues_SelectionChangeCommitted);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(121, 357);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(108, 32);
			this.label6.TabIndex = 47;
			this.label6.Text = "(or double click\r\nCustomer col)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormPhoneTiles
			// 
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1284, 696);
			this.Controls.Add(this.panelGrid2);
			this.Controls.Add(this.panelGrid);
			this.Controls.Add(this.checkShowOldInterface);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.checkHideOnBreak);
			this.Controls.Add(this.checkHideClockedOut);
			this.Controls.Add(this.butConfRooms);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelMsg);
			this.Controls.Add(this.butSettings);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(10, 10);
			this.Name = "FormPhoneTiles";
			this.Text = "Big Phones";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormPhoneTiles_Load);
			this.Shown += new System.EventHandler(this.FormPhoneTiles_Shown);
			this.menuNumbers.ResumeLayout(false);
			this.menuStatus.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panelGrid.ResumeLayout(false);
			this.panelGrid.PerformLayout();
			this.panelGrid2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
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
		private System.Windows.Forms.ToolStripMenuItem menuItemNeedsHelp;
		private OpenDental.UI.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioByExt;
		private System.Windows.Forms.RadioButton radioByName;
		private System.Windows.Forms.ToolStripMenuItem menuItemStatusOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemRingGroupOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemClockOnBehalf;
		private System.Windows.Forms.ToolStripMenuItem menuItemRinggroupBackup;
		private UI.Button butConfRooms;
		private System.Windows.Forms.ToolStripMenuItem menuItemTCResponder;
		private System.Windows.Forms.ToolStripMenuItem menuItemEmployeeSettings;
		private OpenDental.UI.CheckBox checkHideClockedOut;
		private System.Windows.Forms.Timer timerFlash;
		private OpenDental.UI.CheckBox checkHideOnBreak;
		private UI.GridOD gridMain;
		private UI.CheckBox checkShowOldInterface;
		private System.Windows.Forms.TextBox textSearch;
		private System.Windows.Forms.Label labelSearch;
		private UI.CheckBox checkNeedsHelpTop;
		private System.Windows.Forms.Panel panelGrid;
		private System.Windows.Forms.Label label1;
		private UI.ListBoxOD listBoxStatus;
		private System.Windows.Forms.Panel panelGrid2;
		private System.Windows.Forms.Label label2;
		private UI.ListBoxOD listBoxQueues;
		private System.Windows.Forms.Label label3;
		private UI.ListBoxOD listBoxClockOut;
		private UI.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private UI.Button butPhoneManage;
		private UI.Button butPhoneAttach;
		private UI.Button butGotoPatient;
		private System.Windows.Forms.Label label5;
		private UI.Button butEditDefaults;
		private System.Windows.Forms.Label label6;
	}
}