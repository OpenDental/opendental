namespace OpenDental {
	partial class FormPhoneList {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneList));
			this.labelMsg = new System.Windows.Forms.Label();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.radioByExt = new System.Windows.Forms.RadioButton();
			this.radioByName = new System.Windows.Forms.RadioButton();
			this.butSettings = new OpenDental.UI.Button();
			this.butConfRooms = new OpenDental.UI.Button();
			this.checkHideClockedOut = new OpenDental.UI.CheckBox();
			this.timerFlash = new System.Windows.Forms.Timer(this.components);
			this.checkHideOnBreak = new OpenDental.UI.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textNameOrExt = new System.Windows.Forms.TextBox();
			this.labelSearch = new System.Windows.Forms.Label();
			this.checkNeedsHelpTop = new OpenDental.UI.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxStatus = new OpenDental.UI.ListBox();
			this.panelGrid2 = new System.Windows.Forms.Panel();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.butEditDefaults = new OpenDental.UI.Button();
			this.butGotoPatient = new OpenDental.UI.Button();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butPhoneManage = new OpenDental.UI.Button();
			this.butPhoneAttach = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.listBoxClockOut = new OpenDental.UI.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxQueues = new OpenDental.UI.ListBox();
			this.label7 = new System.Windows.Forms.Label();
			this.listBoxQueueFilter = new OpenDental.UI.ListBox();
			this.checkHideFloaters = new OpenDental.UI.CheckBox();
			this.groupBox1.SuspendLayout();
			this.panelGrid2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelMsg
			// 
			this.labelMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMsg.ForeColor = System.Drawing.Color.Firebrick;
			this.labelMsg.Location = new System.Drawing.Point(70, 1);
			this.labelMsg.Name = "labelMsg";
			this.labelMsg.Size = new System.Drawing.Size(128, 20);
			this.labelMsg.TabIndex = 27;
			this.labelMsg.Text = "Voice Mails: 0";
			this.labelMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioByExt);
			this.groupBox1.Controls.Add(this.radioByName);
			this.groupBox1.Location = new System.Drawing.Point(24, 180);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(126, 58);
			this.groupBox1.TabIndex = 29;
			this.groupBox1.Text = "Sort By:";
			// 
			// radioByExt
			// 
			this.radioByExt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioByExt.Location = new System.Drawing.Point(17, 37);
			this.radioByExt.Name = "radioByExt";
			this.radioByExt.Size = new System.Drawing.Size(85, 17);
			this.radioByExt.TabIndex = 1;
			this.radioByExt.Text = "Extension";
			this.radioByExt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioByExt.UseVisualStyleBackColor = true;
			// 
			// radioByName
			// 
			this.radioByName.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioByName.Checked = true;
			this.radioByName.Location = new System.Drawing.Point(30, 17);
			this.radioByName.Name = "radioByName";
			this.radioByName.Size = new System.Drawing.Size(72, 17);
			this.radioByName.TabIndex = 0;
			this.radioByName.TabStop = true;
			this.radioByName.Text = "Name";
			this.radioByName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioByName.UseVisualStyleBackColor = true;
			// 
			// butSettings
			// 
			this.butSettings.Location = new System.Drawing.Point(0, 0);
			this.butSettings.Name = "butSettings";
			this.butSettings.Size = new System.Drawing.Size(65, 24);
			this.butSettings.TabIndex = 26;
			this.butSettings.Text = "Settings";
			this.butSettings.Click += new System.EventHandler(this.butSettings_Click);
			// 
			// butConfRooms
			// 
			this.butConfRooms.Location = new System.Drawing.Point(78, 32);
			this.butConfRooms.Name = "butConfRooms";
			this.butConfRooms.Size = new System.Drawing.Size(89, 24);
			this.butConfRooms.TabIndex = 30;
			this.butConfRooms.Text = "Conf Rooms";
			this.butConfRooms.Click += new System.EventHandler(this.butConfRooms_Click);
			// 
			// checkHideClockedOut
			// 
			this.checkHideClockedOut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideClockedOut.Location = new System.Drawing.Point(8, 248);
			this.checkHideClockedOut.Name = "checkHideClockedOut";
			this.checkHideClockedOut.Size = new System.Drawing.Size(118, 16);
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
			this.checkHideOnBreak.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideOnBreak.Location = new System.Drawing.Point(20, 271);
			this.checkHideOnBreak.Name = "checkHideOnBreak";
			this.checkHideOnBreak.Size = new System.Drawing.Size(106, 16);
			this.checkHideOnBreak.TabIndex = 32;
			this.checkHideOnBreak.Text = "Hide on break";
			this.checkHideOnBreak.CheckedChanged += new System.EventHandler(this.checkHideOnBreak_CheckedChanged);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasAlternateRowsColored = true;
			this.gridMain.Location = new System.Drawing.Point(200, 5);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(938, 687);
			this.gridMain.TabIndex = 33;
			this.gridMain.TranslationName = "TableBigPhones";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// textNameOrExt
			// 
			this.textNameOrExt.Location = new System.Drawing.Point(113, 62);
			this.textNameOrExt.Name = "textNameOrExt";
			this.textNameOrExt.Size = new System.Drawing.Size(81, 20);
			this.textNameOrExt.TabIndex = 0;
			// 
			// labelSearch
			// 
			this.labelSearch.Location = new System.Drawing.Point(22, 61);
			this.labelSearch.Name = "labelSearch";
			this.labelSearch.Size = new System.Drawing.Size(87, 20);
			this.labelSearch.TabIndex = 35;
			this.labelSearch.Text = "Name or Ext";
			this.labelSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkNeedsHelpTop
			// 
			this.checkNeedsHelpTop.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNeedsHelpTop.Checked = true;
			this.checkNeedsHelpTop.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkNeedsHelpTop.Location = new System.Drawing.Point(2, 158);
			this.checkNeedsHelpTop.Name = "checkNeedsHelpTop";
			this.checkNeedsHelpTop.Size = new System.Drawing.Size(124, 16);
			this.checkNeedsHelpTop.TabIndex = 36;
			this.checkNeedsHelpTop.Text = "Needs Help at Top";
			this.checkNeedsHelpTop.Click += new System.EventHandler(this.checkNeedsHelpTop_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-2, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(93, 18);
			this.label1.TabIndex = 38;
			this.label1.Text = "Change Status";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
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
			this.listBoxStatus.Location = new System.Drawing.Point(1, 28);
			this.listBoxStatus.Name = "listBoxStatus";
			this.listBoxStatus.Size = new System.Drawing.Size(85, 121);
			this.listBoxStatus.TabIndex = 37;
			this.listBoxStatus.Text = "listBox1";
			this.listBoxStatus.SelectionChangeCommitted += new System.EventHandler(this.listBoxStatus_SelectionChangeCommitted);
			// 
			// panelGrid2
			// 
			this.panelGrid2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panelGrid2.Controls.Add(this.listBoxStatus);
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
			this.panelGrid2.Location = new System.Drawing.Point(1141, 13);
			this.panelGrid2.Name = "panelGrid2";
			this.panelGrid2.Size = new System.Drawing.Size(88, 563);
			this.panelGrid2.TabIndex = 38;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(1, 433);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(89, 30);
			this.label6.TabIndex = 47;
			this.label6.Text = "(or double click\r\nCustomer col)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(1, 496);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(88, 42);
			this.label5.TabIndex = 46;
			this.label5.Text = "for Employee\r\n(or double click\r\nany other col)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butEditDefaults
			// 
			this.butEditDefaults.Location = new System.Drawing.Point(3, 470);
			this.butEditDefaults.Name = "butEditDefaults";
			this.butEditDefaults.Size = new System.Drawing.Size(82, 24);
			this.butEditDefaults.TabIndex = 46;
			this.butEditDefaults.Text = "Edit Defaults";
			this.butEditDefaults.Click += new System.EventHandler(this.butEditDefaults_Click);
			// 
			// butGotoPatient
			// 
			this.butGotoPatient.Location = new System.Drawing.Point(3, 406);
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
			this.groupBox2.Location = new System.Drawing.Point(0, 293);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(88, 104);
			this.groupBox2.TabIndex = 44;
			this.groupBox2.Text = "Phone Numbers";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(13, 74);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 28);
			this.label4.TabIndex = 45;
			this.label4.Text = "to Current\r\nPatient";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butPhoneManage
			// 
			this.butPhoneManage.Location = new System.Drawing.Point(8, 18);
			this.butPhoneManage.Name = "butPhoneManage";
			this.butPhoneManage.Size = new System.Drawing.Size(72, 24);
			this.butPhoneManage.TabIndex = 39;
			this.butPhoneManage.Text = "Manage";
			this.butPhoneManage.Click += new System.EventHandler(this.butPhoneManage_Click);
			// 
			// butPhoneAttach
			// 
			this.butPhoneAttach.Location = new System.Drawing.Point(8, 47);
			this.butPhoneAttach.Name = "butPhoneAttach";
			this.butPhoneAttach.Size = new System.Drawing.Size(72, 24);
			this.butPhoneAttach.TabIndex = 43;
			this.butPhoneAttach.Text = "Attach";
			this.butPhoneAttach.Click += new System.EventHandler(this.butPhoneAttach_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(-2, 224);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 18);
			this.label3.TabIndex = 42;
			this.label3.Text = "Clock out for";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxClockOut
			// 
			this.listBoxClockOut.ItemStrings = new string[] {
        "Home",
        "Lunch",
        "Break"};
			this.listBoxClockOut.Location = new System.Drawing.Point(1, 245);
			this.listBoxClockOut.Name = "listBoxClockOut";
			this.listBoxClockOut.Size = new System.Drawing.Size(85, 43);
			this.listBoxClockOut.TabIndex = 41;
			this.listBoxClockOut.Text = "listBox1";
			this.listBoxClockOut.SelectionChangeCommitted += new System.EventHandler(this.listBoxClockOut_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-2, 147);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 18);
			this.label2.TabIndex = 40;
			this.label2.Text = "Queues";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxQueues
			// 
			this.listBoxQueues.ItemStrings = new string[] {
        "Tech",
        "None",
        "Default",
        "Backup"};
			this.listBoxQueues.Location = new System.Drawing.Point(1, 168);
			this.listBoxQueues.Name = "listBoxQueues";
			this.listBoxQueues.Size = new System.Drawing.Size(85, 56);
			this.listBoxQueues.TabIndex = 39;
			this.listBoxQueues.Text = "listBox1";
			this.listBoxQueues.SelectionChangeCommitted += new System.EventHandler(this.listBoxQueues_SelectionChangeCommitted);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(46, 86);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(63, 18);
			this.label7.TabIndex = 49;
			this.label7.Text = "Queues";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listBoxQueueFilter
			// 
			this.listBoxQueueFilter.ItemStrings = new string[] {
        "All",
        "Tech",
        "None",
        "Default",
        "Backup"};
			this.listBoxQueueFilter.Location = new System.Drawing.Point(113, 86);
			this.listBoxQueueFilter.Name = "listBoxQueueFilter";
			this.listBoxQueueFilter.Size = new System.Drawing.Size(81, 69);
			this.listBoxQueueFilter.TabIndex = 48;
			this.listBoxQueueFilter.Text = "listBox1";
			// 
			// checkHideFloaters
			// 
			this.checkHideFloaters.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideFloaters.Checked = true;
			this.checkHideFloaters.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkHideFloaters.Location = new System.Drawing.Point(20, 294);
			this.checkHideFloaters.Name = "checkHideFloaters";
			this.checkHideFloaters.Size = new System.Drawing.Size(106, 16);
			this.checkHideFloaters.TabIndex = 50;
			this.checkHideFloaters.Text = "Hide floaters";
			this.checkHideFloaters.CheckedChanged += new System.EventHandler(this.checkHideFloaters_CheckedChanged);
			// 
			// FormPhoneList
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.checkHideFloaters);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.listBoxQueueFilter);
			this.Controls.Add(this.textNameOrExt);
			this.Controls.Add(this.panelGrid2);
			this.Controls.Add(this.labelSearch);
			this.Controls.Add(this.checkNeedsHelpTop);
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
			this.Name = "FormPhoneList";
			this.Text = "Phone List";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormPhoneList_Load);
			this.Shown += new System.EventHandler(this.FormPhoneTiles_Shown);
			this.groupBox1.ResumeLayout(false);
			this.panelGrid2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelMsg;
		private OpenDental.UI.Button butSettings;
		private OpenDental.UI.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioByExt;
		private System.Windows.Forms.RadioButton radioByName;
		private UI.Button butConfRooms;
		private OpenDental.UI.CheckBox checkHideClockedOut;
		private System.Windows.Forms.Timer timerFlash;
		private OpenDental.UI.CheckBox checkHideOnBreak;
		private UI.GridOD gridMain;
		private System.Windows.Forms.TextBox textNameOrExt;
		private System.Windows.Forms.Label labelSearch;
		private UI.CheckBox checkNeedsHelpTop;
		private System.Windows.Forms.Label label1;
		private UI.ListBox listBoxStatus;
		private System.Windows.Forms.Panel panelGrid2;
		private System.Windows.Forms.Label label2;
		private UI.ListBox listBoxQueues;
		private System.Windows.Forms.Label label3;
		private UI.ListBox listBoxClockOut;
		private UI.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private UI.Button butPhoneManage;
		private UI.Button butPhoneAttach;
		private UI.Button butGotoPatient;
		private System.Windows.Forms.Label label5;
		private UI.Button butEditDefaults;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private UI.ListBox listBoxQueueFilter;
		private UI.CheckBox checkHideFloaters;
	}
}