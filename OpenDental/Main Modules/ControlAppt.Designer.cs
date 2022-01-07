namespace OpenDental {
	partial class ControlAppt {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlAppt));
			this.panelCalendar = new System.Windows.Forms.Panel();
			this.panelCalendarLower = new System.Windows.Forms.Panel();
			this.butFwd4 = new OpenDental.UI.Button();
			this.butFwd6 = new OpenDental.UI.Button();
			this.butFwd3 = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.pinBoard = new OpenDental.UI.PinBoard();
			this.listConfirmed = new OpenDental.UI.ListBoxOD();
			this.butAdvSearch = new OpenDental.UI.Button();
			this.butFwdMonth = new OpenDental.UI.Button();
			this.textProdGoal = new System.Windows.Forms.TextBox();
			this.butBackMonth = new OpenDental.UI.Button();
			this.labelProdGoal = new System.Windows.Forms.Label();
			this.butBackWeek = new OpenDental.UI.Button();
			this.radioWeek = new System.Windows.Forms.RadioButton();
			this.butFwdWeek = new OpenDental.UI.Button();
			this.radioDay = new System.Windows.Forms.RadioButton();
			this.butToday = new OpenDental.UI.Button();
			this.butFwd = new OpenDental.UI.Button();
			this.butBack = new OpenDental.UI.Button();
			this.butLab = new OpenDental.UI.Button();
			this.butClearPin = new OpenDental.UI.Button();
			this.butSearch = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textProduction = new System.Windows.Forms.TextBox();
			this.comboView = new OpenDental.UI.ComboBoxOD();
			this.labelProduction = new System.Windows.Forms.Label();
			this.textLab = new System.Windows.Forms.TextBox();
			this.butMonth = new OpenDental.UI.Button();
			this.monthCalendarOD = new OpenDental.UI.MonthCalendarOD();
			this.labelDate2 = new System.Windows.Forms.Label();
			this.labelDate = new System.Windows.Forms.Label();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabWaiting = new System.Windows.Forms.TabPage();
			this.gridWaiting = new OpenDental.UI.GridOD();
			this.tabSched = new System.Windows.Forms.TabPage();
			this.gridEmpSched = new OpenDental.UI.GridOD();
			this.tabProv = new System.Windows.Forms.TabPage();
			this.gridProv = new OpenDental.UI.GridOD();
			this.tabReminders = new System.Windows.Forms.TabPage();
			this.gridReminders = new OpenDental.UI.GridOD();
			this.groupSearch = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butProvHygenist = new OpenDental.UI.Button();
			this.butProvDentist = new OpenDental.UI.Button();
			this.butProvPick = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.listSearchResults = new OpenDental.UI.ListBoxOD();
			this._listBoxProviders = new OpenDental.UI.ListBoxOD();
			this.butSearchClose = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textAfter = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.radioBeforePM = new System.Windows.Forms.RadioButton();
			this.radioBeforeAM = new System.Windows.Forms.RadioButton();
			this.textBefore = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.radioAfterAM = new System.Windows.Forms.RadioButton();
			this.radioAfterPM = new System.Windows.Forms.RadioButton();
			this.dateSearch = new System.Windows.Forms.DateTimePicker();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.butSearchCloseX = new System.Windows.Forms.Button();
			this.butSearchNext = new OpenDental.UI.Button();
			this.labelNoneView = new System.Windows.Forms.Label();
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.menuBlockout = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._menuOp = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuApt = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.timerWaitingRoom = new System.Windows.Forms.Timer(this.components);
			this.menuReminderEdit = new System.Windows.Forms.ContextMenu();
			this.menuItemReminderDone = new System.Windows.Forms.MenuItem();
			this.menuItemReminderGoto = new System.Windows.Forms.MenuItem();
			this.imageListTasks = new System.Windows.Forms.ImageList(this.components);
			this.toolBarMain = new OpenDental.UI.ToolBarOD();
			this.contrApptPanel = new OpenDental.UI.ControlApptPanel();
			this.menuRecall = new System.Windows.Forms.ContextMenu();
			this.panelCalendar.SuspendLayout();
			this.panelCalendarLower.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabWaiting.SuspendLayout();
			this.tabSched.SuspendLayout();
			this.tabProv.SuspendLayout();
			this.tabReminders.SuspendLayout();
			this.groupSearch.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelCalendar
			// 
			this.panelCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panelCalendar.Controls.Add(this.panelCalendarLower);
			this.panelCalendar.Controls.Add(this.butMonth);
			this.panelCalendar.Controls.Add(this.monthCalendarOD);
			this.panelCalendar.Controls.Add(this.labelDate2);
			this.panelCalendar.Controls.Add(this.labelDate);
			this.panelCalendar.Location = new System.Drawing.Point(833, 46);
			this.panelCalendar.Name = "panelCalendar";
			this.panelCalendar.Size = new System.Drawing.Size(219, 509);
			this.panelCalendar.TabIndex = 47;
			// 
			// panelCalendarLower
			// 
			this.panelCalendarLower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelCalendarLower.Controls.Add(this.butFwd4);
			this.panelCalendarLower.Controls.Add(this.butFwd6);
			this.panelCalendarLower.Controls.Add(this.butFwd3);
			this.panelCalendarLower.Controls.Add(this.label1);
			this.panelCalendarLower.Controls.Add(this.pinBoard);
			this.panelCalendarLower.Controls.Add(this.listConfirmed);
			this.panelCalendarLower.Controls.Add(this.butAdvSearch);
			this.panelCalendarLower.Controls.Add(this.butFwdMonth);
			this.panelCalendarLower.Controls.Add(this.textProdGoal);
			this.panelCalendarLower.Controls.Add(this.butBackMonth);
			this.panelCalendarLower.Controls.Add(this.labelProdGoal);
			this.panelCalendarLower.Controls.Add(this.butBackWeek);
			this.panelCalendarLower.Controls.Add(this.radioWeek);
			this.panelCalendarLower.Controls.Add(this.butFwdWeek);
			this.panelCalendarLower.Controls.Add(this.radioDay);
			this.panelCalendarLower.Controls.Add(this.butToday);
			this.panelCalendarLower.Controls.Add(this.butFwd);
			this.panelCalendarLower.Controls.Add(this.butBack);
			this.panelCalendarLower.Controls.Add(this.butLab);
			this.panelCalendarLower.Controls.Add(this.butClearPin);
			this.panelCalendarLower.Controls.Add(this.butSearch);
			this.panelCalendarLower.Controls.Add(this.label2);
			this.panelCalendarLower.Controls.Add(this.textProduction);
			this.panelCalendarLower.Controls.Add(this.comboView);
			this.panelCalendarLower.Controls.Add(this.labelProduction);
			this.panelCalendarLower.Controls.Add(this.textLab);
			this.panelCalendarLower.Location = new System.Drawing.Point(0, 188);
			this.panelCalendarLower.Name = "panelCalendarLower";
			this.panelCalendarLower.Size = new System.Drawing.Size(219, 317);
			this.panelCalendarLower.TabIndex = 88;
			// 
			// butFwd4
			// 
			this.butFwd4.AdjustImageLocation = new System.Drawing.Point(5, -1);
			this.butFwd4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butFwd4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butFwd4.Location = new System.Drawing.Point(178, 0);
			this.butFwd4.Name = "butFwd4";
			this.butFwd4.Size = new System.Drawing.Size(19, 22);
			this.butFwd4.TabIndex = 98;
			this.butFwd4.Text = "4";
			this.butFwd4.Click += new System.EventHandler(this.butFwd4_Click);
			// 
			// butFwd6
			// 
			this.butFwd6.AdjustImageLocation = new System.Drawing.Point(5, -1);
			this.butFwd6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butFwd6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butFwd6.Location = new System.Drawing.Point(197, 0);
			this.butFwd6.Name = "butFwd6";
			this.butFwd6.Size = new System.Drawing.Size(19, 22);
			this.butFwd6.TabIndex = 97;
			this.butFwd6.Text = "6";
			this.butFwd6.Click += new System.EventHandler(this.butFwd6_Click);
			// 
			// butFwd3
			// 
			this.butFwd3.AdjustImageLocation = new System.Drawing.Point(5, -1);
			this.butFwd3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butFwd3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butFwd3.Location = new System.Drawing.Point(159, 0);
			this.butFwd3.Name = "butFwd3";
			this.butFwd3.Size = new System.Drawing.Size(19, 22);
			this.butFwd3.TabIndex = 96;
			this.butFwd3.Text = "3";
			this.butFwd3.Click += new System.EventHandler(this.butFwd3_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 204);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 30);
			this.label1.TabIndex = 95;
			this.label1.Text = "Confirmation\r\nStatus";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pinBoard
			// 
			this.pinBoard.Location = new System.Drawing.Point(118, 24);
			this.pinBoard.Name = "pinBoard";
			this.pinBoard.Size = new System.Drawing.Size(99, 96);
			this.pinBoard.TabIndex = 89;
			this.pinBoard.Text = "pinBoardJ1";
			this.pinBoard.ApptMovedFromPinboard += new System.EventHandler<OpenDental.UI.ApptFromPinboardEventArgs>(this.pinBoard_ApptMovedFromPinboard);
			this.pinBoard.ModuleNeedsRefresh += new System.EventHandler(this.pinBoard_ModuleNeedsRefresh);
			this.pinBoard.PreparingToDragFromPinboard += new System.EventHandler<OpenDental.UI.ApptDataRowEventArgs>(this.pinBoard_PreparingToDragFromPinboard);
			this.pinBoard.SelectedIndexChanged += new System.EventHandler(this.pinBoard_SelectedIndexChanged);
			// 
			// listConfirmed
			// 
			this.listConfirmed.IntegralHeight = false;
			this.listConfirmed.Location = new System.Drawing.Point(84, 202);
			this.listConfirmed.Name = "listConfirmed";
			this.listConfirmed.Size = new System.Drawing.Size(86, 113);
			this.listConfirmed.TabIndex = 75;
			this.listConfirmed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListConfirmed_MouseDown);
			// 
			// butAdvSearch
			// 
			this.butAdvSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdvSearch.Location = new System.Drawing.Point(2, 96);
			this.butAdvSearch.Name = "butAdvSearch";
			this.butAdvSearch.Size = new System.Drawing.Size(63, 24);
			this.butAdvSearch.TabIndex = 90;
			this.butAdvSearch.Text = "Advanced";
			this.butAdvSearch.Click += new System.EventHandler(this.ButAdvSearch_Click);
			// 
			// butFwdMonth
			// 
			this.butFwdMonth.AdjustImageLocation = new System.Drawing.Point(5, -1);
			this.butFwdMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butFwdMonth.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butFwdMonth.Location = new System.Drawing.Point(140, 0);
			this.butFwdMonth.Name = "butFwdMonth";
			this.butFwdMonth.Size = new System.Drawing.Size(19, 22);
			this.butFwdMonth.TabIndex = 56;
			this.butFwdMonth.Text = "M";
			this.butFwdMonth.Click += new System.EventHandler(this.butFwdMonth_Click);
			// 
			// textProdGoal
			// 
			this.textProdGoal.BackColor = System.Drawing.Color.White;
			this.textProdGoal.Location = new System.Drawing.Point(84, 182);
			this.textProdGoal.Name = "textProdGoal";
			this.textProdGoal.ReadOnly = true;
			this.textProdGoal.Size = new System.Drawing.Size(133, 20);
			this.textProdGoal.TabIndex = 93;
			this.textProdGoal.Text = "$100";
			this.textProdGoal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butBackMonth
			// 
			this.butBackMonth.AdjustImageLocation = new System.Drawing.Point(-3, -1);
			this.butBackMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butBackMonth.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBackMonth.Location = new System.Drawing.Point(1, 0);
			this.butBackMonth.Name = "butBackMonth";
			this.butBackMonth.Size = new System.Drawing.Size(19, 22);
			this.butBackMonth.TabIndex = 57;
			this.butBackMonth.Text = "M";
			this.butBackMonth.Click += new System.EventHandler(this.butBackMonth_Click);
			// 
			// labelProdGoal
			// 
			this.labelProdGoal.Location = new System.Drawing.Point(4, 186);
			this.labelProdGoal.Name = "labelProdGoal";
			this.labelProdGoal.Size = new System.Drawing.Size(79, 15);
			this.labelProdGoal.TabIndex = 94;
			this.labelProdGoal.Text = "Daily Goal";
			this.labelProdGoal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butBackWeek
			// 
			this.butBackWeek.AdjustImageLocation = new System.Drawing.Point(-3, -1);
			this.butBackWeek.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butBackWeek.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBackWeek.Location = new System.Drawing.Point(20, 0);
			this.butBackWeek.Name = "butBackWeek";
			this.butBackWeek.Size = new System.Drawing.Size(19, 22);
			this.butBackWeek.TabIndex = 55;
			this.butBackWeek.Text = "W";
			this.butBackWeek.Click += new System.EventHandler(this.butBackWeek_Click);
			// 
			// radioWeek
			// 
			this.radioWeek.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioWeek.Location = new System.Drawing.Point(42, 49);
			this.radioWeek.Name = "radioWeek";
			this.radioWeek.Size = new System.Drawing.Size(68, 16);
			this.radioWeek.TabIndex = 92;
			this.radioWeek.Text = "Week";
			this.radioWeek.Click += new System.EventHandler(this.radioWeek_Click);
			// 
			// butFwdWeek
			// 
			this.butFwdWeek.AdjustImageLocation = new System.Drawing.Point(5, -1);
			this.butFwdWeek.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butFwdWeek.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butFwdWeek.Location = new System.Drawing.Point(121, 0);
			this.butFwdWeek.Name = "butFwdWeek";
			this.butFwdWeek.Size = new System.Drawing.Size(19, 22);
			this.butFwdWeek.TabIndex = 54;
			this.butFwdWeek.Text = "W";
			this.butFwdWeek.Click += new System.EventHandler(this.butFwdWeek_Click);
			// 
			// radioDay
			// 
			this.radioDay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioDay.Location = new System.Drawing.Point(42, 29);
			this.radioDay.Name = "radioDay";
			this.radioDay.Size = new System.Drawing.Size(68, 16);
			this.radioDay.TabIndex = 91;
			this.radioDay.Text = "Day";
			this.radioDay.Click += new System.EventHandler(this.radioDay_Click);
			// 
			// butToday
			// 
			this.butToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butToday.Location = new System.Drawing.Point(58, 0);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(44, 22);
			this.butToday.TabIndex = 29;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// butFwd
			// 
			this.butFwd.AdjustImageLocation = new System.Drawing.Point(5, -1);
			this.butFwd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butFwd.Image = ((System.Drawing.Image)(resources.GetObject("butFwd.Image")));
			this.butFwd.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butFwd.Location = new System.Drawing.Point(102, 0);
			this.butFwd.Name = "butFwd";
			this.butFwd.Size = new System.Drawing.Size(19, 22);
			this.butFwd.TabIndex = 53;
			this.butFwd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butFwd.Click += new System.EventHandler(this.butFwd_Click);
			// 
			// butBack
			// 
			this.butBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butBack.Image = ((System.Drawing.Image)(resources.GetObject("butBack.Image")));
			this.butBack.Location = new System.Drawing.Point(39, 0);
			this.butBack.Name = "butBack";
			this.butBack.Size = new System.Drawing.Size(19, 22);
			this.butBack.TabIndex = 51;
			this.butBack.Click += new System.EventHandler(this.butBack_Click);
			// 
			// butLab
			// 
			this.butLab.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLab.Location = new System.Drawing.Point(2, 144);
			this.butLab.Name = "butLab";
			this.butLab.Size = new System.Drawing.Size(79, 21);
			this.butLab.TabIndex = 77;
			this.butLab.Text = "Lab Cases";
			this.butLab.Click += new System.EventHandler(this.ButLab_Click);
			// 
			// butClearPin
			// 
			this.butClearPin.Image = ((System.Drawing.Image)(resources.GetObject("butClearPin.Image")));
			this.butClearPin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClearPin.Location = new System.Drawing.Point(42, 71);
			this.butClearPin.Name = "butClearPin";
			this.butClearPin.Size = new System.Drawing.Size(75, 24);
			this.butClearPin.TabIndex = 33;
			this.butClearPin.Text = "Clear";
			this.butClearPin.Click += new System.EventHandler(this.butClearPin_Click);
			// 
			// butSearch
			// 
			this.butSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSearch.Location = new System.Drawing.Point(66, 96);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(51, 24);
			this.butSearch.TabIndex = 40;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 125);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 16);
			this.label2.TabIndex = 34;
			this.label2.Text = "View";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProduction
			// 
			this.textProduction.BackColor = System.Drawing.Color.White;
			this.textProduction.Location = new System.Drawing.Point(84, 164);
			this.textProduction.Name = "textProduction";
			this.textProduction.ReadOnly = true;
			this.textProduction.Size = new System.Drawing.Size(133, 20);
			this.textProduction.TabIndex = 38;
			this.textProduction.Text = "$100";
			this.textProduction.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// comboView
			// 
			this.comboView.Location = new System.Drawing.Point(84, 123);
			this.comboView.Name = "comboView";
			this.comboView.Size = new System.Drawing.Size(133, 21);
			this.comboView.TabIndex = 35;
			this.comboView.SelectionChangeCommitted += new System.EventHandler(this.comboView_SelectionChangeCommitted);
			// 
			// labelProduction
			// 
			this.labelProduction.Location = new System.Drawing.Point(1, 168);
			this.labelProduction.Name = "labelProduction";
			this.labelProduction.Size = new System.Drawing.Size(82, 15);
			this.labelProduction.TabIndex = 39;
			this.labelProduction.Text = "Daily Prod";
			this.labelProduction.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLab
			// 
			this.textLab.BackColor = System.Drawing.Color.White;
			this.textLab.Location = new System.Drawing.Point(84, 144);
			this.textLab.Name = "textLab";
			this.textLab.ReadOnly = true;
			this.textLab.Size = new System.Drawing.Size(133, 20);
			this.textLab.TabIndex = 36;
			this.textLab.Text = "All Received";
			this.textLab.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butMonth
			// 
			this.butMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butMonth.Location = new System.Drawing.Point(152, 1);
			this.butMonth.Name = "butMonth";
			this.butMonth.Size = new System.Drawing.Size(65, 22);
			this.butMonth.TabIndex = 79;
			this.butMonth.Text = "Month";
			this.butMonth.Visible = false;
			// 
			// Calendar2
			// 
			this.monthCalendarOD.Location = new System.Drawing.Point(0, 22);
			this.monthCalendarOD.Name = "monthCalendarOD";
			this.monthCalendarOD.TabIndex = 23;
			this.monthCalendarOD.DateChanged += new System.EventHandler(this.Calendar2_DateSelected);
			this.monthCalendarOD.SizeChanged += new System.EventHandler(this.Calendar2_SizeChanged);
			// 
			// labelDate2
			// 
			this.labelDate2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDate2.Location = new System.Drawing.Point(51, 3);
			this.labelDate2.Name = "labelDate2";
			this.labelDate2.Size = new System.Drawing.Size(82, 20);
			this.labelDate2.TabIndex = 25;
			this.labelDate2.Text = "-  Oct 20";
			// 
			// labelDate
			// 
			this.labelDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDate.Location = new System.Drawing.Point(2, 3);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(56, 18);
			this.labelDate.TabIndex = 24;
			this.labelDate.Text = "Wed";
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabWaiting);
			this.tabControl.Controls.Add(this.tabSched);
			this.tabControl.Controls.Add(this.tabProv);
			this.tabControl.Controls.Add(this.tabReminders);
			this.tabControl.Location = new System.Drawing.Point(833, 565);
			this.tabControl.Margin = new System.Windows.Forms.Padding(0);
			this.tabControl.Name = "tabControl";
			this.tabControl.Padding = new System.Drawing.Point(0, 0);
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(219, 166);
			this.tabControl.TabIndex = 84;
			// 
			// tabWaiting
			// 
			this.tabWaiting.Controls.Add(this.gridWaiting);
			this.tabWaiting.Location = new System.Drawing.Point(4, 22);
			this.tabWaiting.Margin = new System.Windows.Forms.Padding(0);
			this.tabWaiting.Name = "tabWaiting";
			this.tabWaiting.Size = new System.Drawing.Size(211, 140);
			this.tabWaiting.TabIndex = 0;
			this.tabWaiting.Text = "Waiting";
			this.tabWaiting.UseVisualStyleBackColor = true;
			// 
			// gridWaiting
			// 
			this.gridWaiting.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridWaiting.Location = new System.Drawing.Point(0, 0);
			this.gridWaiting.Margin = new System.Windows.Forms.Padding(0);
			this.gridWaiting.Name = "gridWaiting";
			this.gridWaiting.Size = new System.Drawing.Size(211, 140);
			this.gridWaiting.TabIndex = 78;
			this.gridWaiting.Title = "Waiting Room";
			this.gridWaiting.TranslationName = "TableApptWaiting";
			// 
			// tabSched
			// 
			this.tabSched.Controls.Add(this.gridEmpSched);
			this.tabSched.Location = new System.Drawing.Point(4, 22);
			this.tabSched.Margin = new System.Windows.Forms.Padding(0);
			this.tabSched.Name = "tabSched";
			this.tabSched.Size = new System.Drawing.Size(211, 140);
			this.tabSched.TabIndex = 1;
			this.tabSched.Text = "Emp";
			this.tabSched.UseVisualStyleBackColor = true;
			// 
			// gridEmpSched
			// 
			this.gridEmpSched.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridEmpSched.HScrollVisible = true;
			this.gridEmpSched.Location = new System.Drawing.Point(0, 0);
			this.gridEmpSched.Margin = new System.Windows.Forms.Padding(0);
			this.gridEmpSched.Name = "gridEmpSched";
			this.gridEmpSched.Size = new System.Drawing.Size(211, 140);
			this.gridEmpSched.TabIndex = 77;
			this.gridEmpSched.Title = "Employee Schedules";
			this.gridEmpSched.TranslationName = "TableApptEmpSched";
			this.gridEmpSched.DoubleClick += new System.EventHandler(this.GridEmpOrProv_DoubleClick);
			// 
			// tabProv
			// 
			this.tabProv.Controls.Add(this.gridProv);
			this.tabProv.Location = new System.Drawing.Point(4, 22);
			this.tabProv.Margin = new System.Windows.Forms.Padding(0);
			this.tabProv.Name = "tabProv";
			this.tabProv.Size = new System.Drawing.Size(211, 140);
			this.tabProv.TabIndex = 2;
			this.tabProv.Text = "Prov";
			this.tabProv.UseVisualStyleBackColor = true;
			// 
			// gridProv
			// 
			this.gridProv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridProv.HScrollVisible = true;
			this.gridProv.Location = new System.Drawing.Point(0, 0);
			this.gridProv.Margin = new System.Windows.Forms.Padding(0);
			this.gridProv.Name = "gridProv";
			this.gridProv.Size = new System.Drawing.Size(211, 140);
			this.gridProv.TabIndex = 79;
			this.gridProv.Title = "Provider Schedules";
			this.gridProv.TranslationName = "TableAppProv";
			this.gridProv.DoubleClick += new System.EventHandler(this.GridEmpOrProv_DoubleClick);
			// 
			// tabReminders
			// 
			this.tabReminders.Controls.Add(this.gridReminders);
			this.tabReminders.Location = new System.Drawing.Point(4, 22);
			this.tabReminders.Margin = new System.Windows.Forms.Padding(0);
			this.tabReminders.Name = "tabReminders";
			this.tabReminders.Size = new System.Drawing.Size(211, 140);
			this.tabReminders.TabIndex = 3;
			this.tabReminders.Text = "Reminders";
			this.tabReminders.UseVisualStyleBackColor = true;
			// 
			// gridReminders
			// 
			this.gridReminders.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridReminders.Location = new System.Drawing.Point(0, 0);
			this.gridReminders.Margin = new System.Windows.Forms.Padding(0);
			this.gridReminders.Name = "gridReminders";
			this.gridReminders.Size = new System.Drawing.Size(211, 140);
			this.gridReminders.TabIndex = 0;
			this.gridReminders.Title = "Reminders";
			this.gridReminders.TranslationName = "TableReminders";
			this.gridReminders.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridReminders_CellDoubleClick);
			this.gridReminders.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridReminders_MouseDown);
			// 
			// groupSearch
			// 
			this.groupSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.groupSearch.Controls.Add(this.groupBox1);
			this.groupSearch.Controls.Add(this.butProvPick);
			this.groupSearch.Controls.Add(this.butRefresh);
			this.groupSearch.Controls.Add(this.listSearchResults);
			this.groupSearch.Controls.Add(this._listBoxProviders);
			this.groupSearch.Controls.Add(this.butSearchClose);
			this.groupSearch.Controls.Add(this.groupBox2);
			this.groupSearch.Controls.Add(this.label8);
			this.groupSearch.Controls.Add(this.butSearchCloseX);
			this.groupSearch.Controls.Add(this.butSearchNext);
			this.groupSearch.Location = new System.Drawing.Point(600, 365);
			this.groupSearch.Name = "groupSearch";
			this.groupSearch.Size = new System.Drawing.Size(219, 367);
			this.groupSearch.TabIndex = 86;
			this.groupSearch.TabStop = false;
			this.groupSearch.Text = "Openings in View";
			this.groupSearch.Visible = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butProvHygenist);
			this.groupBox1.Controls.Add(this.butProvDentist);
			this.groupBox1.Location = new System.Drawing.Point(130, 253);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(85, 63);
			this.groupBox1.TabIndex = 89;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search by";
			// 
			// butProvHygenist
			// 
			this.butProvHygenist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butProvHygenist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butProvHygenist.Location = new System.Drawing.Point(6, 37);
			this.butProvHygenist.Name = "butProvHygenist";
			this.butProvHygenist.Size = new System.Drawing.Size(73, 22);
			this.butProvHygenist.TabIndex = 92;
			this.butProvHygenist.Text = "Hygienists";
			this.butProvHygenist.Click += new System.EventHandler(this.butProvHygenist_Click);
			// 
			// butProvDentist
			// 
			this.butProvDentist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butProvDentist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butProvDentist.Location = new System.Drawing.Point(6, 14);
			this.butProvDentist.Name = "butProvDentist";
			this.butProvDentist.Size = new System.Drawing.Size(73, 22);
			this.butProvDentist.TabIndex = 91;
			this.butProvDentist.Text = "Providers";
			this.butProvDentist.Click += new System.EventHandler(this.butProvDentist_Click);
			// 
			// butProvPick
			// 
			this.butProvPick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butProvPick.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butProvPick.Location = new System.Drawing.Point(6, 340);
			this.butProvPick.Name = "butProvPick";
			this.butProvPick.Size = new System.Drawing.Size(82, 22);
			this.butProvPick.TabIndex = 88;
			this.butProvPick.Text = "Providers...";
			this.butProvPick.Click += new System.EventHandler(this.butProvPick_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRefresh.Location = new System.Drawing.Point(153, 317);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(62, 22);
			this.butRefresh.TabIndex = 88;
			this.butRefresh.Text = "Search";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// listSearchResults
			// 
			this.listSearchResults.IntegralHeight = false;
			this.listSearchResults.Location = new System.Drawing.Point(6, 32);
			this.listSearchResults.Name = "listSearchResults";
			this.listSearchResults.Size = new System.Drawing.Size(193, 134);
			this.listSearchResults.TabIndex = 87;
			this.listSearchResults.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listSearchResults_MouseDown);
			// 
			// _listBoxProviders
			// 
			this._listBoxProviders.Location = new System.Drawing.Point(6, 269);
			this._listBoxProviders.Name = "_listBoxProviders";
			this._listBoxProviders.SelectionMode = OpenDental.UI.SelectionMode.None;
			this._listBoxProviders.Size = new System.Drawing.Size(118, 69);
			this._listBoxProviders.TabIndex = 86;
			// 
			// butSearchClose
			// 
			this.butSearchClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSearchClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSearchClose.Location = new System.Drawing.Point(153, 342);
			this.butSearchClose.Name = "butSearchClose";
			this.butSearchClose.Size = new System.Drawing.Size(62, 22);
			this.butSearchClose.TabIndex = 85;
			this.butSearchClose.Text = "Close";
			this.butSearchClose.Click += new System.EventHandler(this.butSearchClose_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textAfter);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.radioBeforePM);
			this.groupBox2.Controls.Add(this.radioBeforeAM);
			this.groupBox2.Controls.Add(this.textBefore);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.panel1);
			this.groupBox2.Controls.Add(this.dateSearch);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(6, 168);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(193, 84);
			this.groupBox2.TabIndex = 84;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Date/Time Restrictions";
			// 
			// textAfter
			// 
			this.textAfter.Location = new System.Drawing.Point(57, 60);
			this.textAfter.Name = "textAfter";
			this.textAfter.Size = new System.Drawing.Size(44, 20);
			this.textAfter.TabIndex = 88;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(1, 62);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(53, 16);
			this.label11.TabIndex = 87;
			this.label11.Text = "After";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioBeforePM
			// 
			this.radioBeforePM.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioBeforePM.Location = new System.Drawing.Point(151, 41);
			this.radioBeforePM.Name = "radioBeforePM";
			this.radioBeforePM.Size = new System.Drawing.Size(37, 15);
			this.radioBeforePM.TabIndex = 86;
			this.radioBeforePM.Text = "pm";
			// 
			// radioBeforeAM
			// 
			this.radioBeforeAM.Checked = true;
			this.radioBeforeAM.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioBeforeAM.Location = new System.Drawing.Point(108, 41);
			this.radioBeforeAM.Name = "radioBeforeAM";
			this.radioBeforeAM.Size = new System.Drawing.Size(37, 15);
			this.radioBeforeAM.TabIndex = 85;
			this.radioBeforeAM.TabStop = true;
			this.radioBeforeAM.Text = "am";
			// 
			// textBefore
			// 
			this.textBefore.Location = new System.Drawing.Point(57, 38);
			this.textBefore.Name = "textBefore";
			this.textBefore.Size = new System.Drawing.Size(44, 20);
			this.textBefore.TabIndex = 84;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(1, 40);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(53, 16);
			this.label10.TabIndex = 83;
			this.label10.Text = "Before";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioAfterAM);
			this.panel1.Controls.Add(this.radioAfterPM);
			this.panel1.Location = new System.Drawing.Point(105, 60);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(84, 20);
			this.panel1.TabIndex = 86;
			// 
			// radioAfterAM
			// 
			this.radioAfterAM.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAfterAM.Location = new System.Drawing.Point(3, 2);
			this.radioAfterAM.Name = "radioAfterAM";
			this.radioAfterAM.Size = new System.Drawing.Size(37, 15);
			this.radioAfterAM.TabIndex = 89;
			this.radioAfterAM.Text = "am";
			// 
			// radioAfterPM
			// 
			this.radioAfterPM.Checked = true;
			this.radioAfterPM.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAfterPM.Location = new System.Drawing.Point(46, 2);
			this.radioAfterPM.Name = "radioAfterPM";
			this.radioAfterPM.Size = new System.Drawing.Size(36, 15);
			this.radioAfterPM.TabIndex = 90;
			this.radioAfterPM.TabStop = true;
			this.radioAfterPM.Text = "pm";
			// 
			// dateSearch
			// 
			this.dateSearch.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateSearch.Location = new System.Drawing.Point(57, 16);
			this.dateSearch.Name = "dateSearch";
			this.dateSearch.Size = new System.Drawing.Size(130, 20);
			this.dateSearch.TabIndex = 90;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(1, 19);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(54, 16);
			this.label9.TabIndex = 89;
			this.label9.Text = "After";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 251);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(92, 16);
			this.label8.TabIndex = 80;
			this.label8.Text = "Providers";
			this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butSearchCloseX
			// 
			this.butSearchCloseX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.butSearchCloseX.ForeColor = System.Drawing.SystemColors.Control;
			this.butSearchCloseX.Image = ((System.Drawing.Image)(resources.GetObject("butSearchCloseX.Image")));
			this.butSearchCloseX.Location = new System.Drawing.Point(185, 7);
			this.butSearchCloseX.Name = "butSearchCloseX";
			this.butSearchCloseX.Size = new System.Drawing.Size(16, 16);
			this.butSearchCloseX.TabIndex = 0;
			this.butSearchCloseX.Click += new System.EventHandler(this.butSearchCloseX_Click);
			// 
			// butSearchNext
			// 
			this.butSearchNext.Image = ((System.Drawing.Image)(resources.GetObject("butSearchNext.Image")));
			this.butSearchNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butSearchNext.Location = new System.Drawing.Point(111, 9);
			this.butSearchNext.Name = "butSearchNext";
			this.butSearchNext.Size = new System.Drawing.Size(71, 22);
			this.butSearchNext.TabIndex = 77;
			this.butSearchNext.Text = "More";
			this.butSearchNext.Click += new System.EventHandler(this.butSearchMore_Click);
			// 
			// labelNoneView
			// 
			this.labelNoneView.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelNoneView.AutoSize = true;
			this.labelNoneView.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNoneView.Location = new System.Drawing.Point(171, 284);
			this.labelNoneView.Name = "labelNoneView";
			this.labelNoneView.Size = new System.Drawing.Size(324, 66);
			this.labelNoneView.TabIndex = 87;
			this.labelNoneView.Text = "Please select a clinic \r\nor an appointment view.";
			this.labelNoneView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "print.gif");
			this.imageListMain.Images.SetKeyName(1, "apptLists.gif");
			this.imageListMain.Images.SetKeyName(2, "DT Rapid Call.png");
			this.imageListMain.Images.SetKeyName(3, "unschedList.gif");
			this.imageListMain.Images.SetKeyName(4, "breakAptX.gif");
			this.imageListMain.Images.SetKeyName(5, "Check.gif");
			this.imageListMain.Images.SetKeyName(6, "deleteX.gif");
			this.imageListMain.Images.SetKeyName(7, "Patient.gif");
			this.imageListMain.Images.SetKeyName(8, "Add.gif");
			this.imageListMain.Images.SetKeyName(9, "Recall.gif");
			// 
			// _menuOp
			// 
			this._menuOp.Name = "_menuRightClick";
			this._menuOp.Size = new System.Drawing.Size(61, 4);
			// 
			// menuApt
			// 
			this.menuApt.Name = "menuApt";
			this.menuApt.Size = new System.Drawing.Size(61, 4);
			this.menuApt.Opening += new System.ComponentModel.CancelEventHandler(this.menuApt_Opening);
			// 
			// menuBlockout
			// 
			this.menuBlockout.Name = "menuBlockout";
			this.menuBlockout.Size = new System.Drawing.Size(61, 4);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 100;
			this.toolTip1.ReshowDelay = 100;
			// 
			// timerWaitingRoom
			// 
			this.timerWaitingRoom.Enabled = true;
			this.timerWaitingRoom.Interval = 1000;
			this.timerWaitingRoom.Tick += new System.EventHandler(this.timerWaitingRoom_Tick);
			// 
			// menuReminderEdit
			// 
			this.menuReminderEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemReminderDone,
            this.menuItemReminderGoto});
			// 
			// menuItemReminderDone
			// 
			this.menuItemReminderDone.Index = 0;
			this.menuItemReminderDone.Text = "Done (affects all users)";
			this.menuItemReminderDone.Click += new System.EventHandler(this.menuItemReminderDone_Click);
			// 
			// menuItemReminderGoto
			// 
			this.menuItemReminderGoto.Index = 1;
			this.menuItemReminderGoto.Text = "Go To";
			this.menuItemReminderGoto.Click += new System.EventHandler(this.menuItemReminderGoto_Click);
			// 
			// imageListTasks
			// 
			this.imageListTasks.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTasks.ImageStream")));
			this.imageListTasks.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTasks.Images.SetKeyName(0, "TaskList.gif");
			this.imageListTasks.Images.SetKeyName(1, "checkBoxChecked.gif");
			this.imageListTasks.Images.SetKeyName(2, "checkBoxUnchecked.gif");
			this.imageListTasks.Images.SetKeyName(3, "TaskListHighlight.gif");
			this.imageListTasks.Images.SetKeyName(4, "checkBoxNew.gif");
			// 
			// toolBarMain
			// 
			this.toolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolBarMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.toolBarMain.ImageList = this.imageListMain;
			this.toolBarMain.Location = new System.Drawing.Point(0, 0);
			this.toolBarMain.Name = "toolBarMain";
			this.toolBarMain.Size = new System.Drawing.Size(1076, 25);
			this.toolBarMain.TabIndex = 74;
			this.toolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarMain_ButtonClick);
			// 
			// contrApptPanel
			// 
			this.contrApptPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.contrApptPanel.DateSelected = new System.DateTime(((long)(0)));
			this.contrApptPanel.IsWeeklyView = false;
			this.contrApptPanel.Location = new System.Drawing.Point(0, 38);
			this.contrApptPanel.Name = "contrApptPanel";
			this.contrApptPanel.Size = new System.Drawing.Size(594, 703);
			this.contrApptPanel.TabIndex = 0;
			this.contrApptPanel.TableWaitingRoom = null;
			this.contrApptPanel.ApptDoubleClicked += new System.EventHandler<OpenDental.UI.ApptEventArgs>(this.contrApptPanel_ApptDoubleClicked);
			this.contrApptPanel.ApptMainAreaDoubleClicked += new System.EventHandler<OpenDental.UI.ApptMainClickEventArgs>(this.contrApptPanel_ApptMainAreaDoubleClicked);
			this.contrApptPanel.ApptMainAreaRightClicked += new System.EventHandler<OpenDental.UI.ApptMainClickEventArgs>(this.ContrApptPanel_ApptMainAreaRightClicked);
			this.contrApptPanel.ApptMoved += new System.EventHandler<OpenDental.UI.ApptMovedEventArgs>(this.contrApptPanel_ApptMoved);
			this.contrApptPanel.ApptMovedToPinboard += new System.EventHandler<OpenDental.UI.ApptDataRowEventArgs>(this.contrApptPanel_ApptMovedToPinboard);
			this.contrApptPanel.ApptNullFound += new System.EventHandler(this.contrApptPanel_ApptNullFound);
			this.contrApptPanel.ApptResized += new System.EventHandler<OpenDental.UI.ApptEventArgs>(this.contrApptPanel_ApptResized);
			this.contrApptPanel.ApptRightClicked += new System.EventHandler<OpenDental.UI.ApptRightClickEventArgs>(this.ContrApptPanel_ApptRightClicked);
			this.contrApptPanel.DateChanged += new System.EventHandler(this.ContrApptPanel_DateChanged);
			this.contrApptPanel.SelectedApptChanged += new System.EventHandler<OpenDental.UI.ApptSelectedChangedEventArgs>(this.contrApptPanel_SelectedApptChanged);
			// 
			// ControlAppt
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.groupSearch);
			this.Controls.Add(this.toolBarMain);
			this.Controls.Add(this.panelCalendar);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.labelNoneView);
			this.Controls.Add(this.contrApptPanel);
			this.Name = "ControlAppt";
			this.Size = new System.Drawing.Size(1076, 741);
			this.Load += new System.EventHandler(this.ContrAppt_Load);
			this.Resize += new System.EventHandler(this.ContrAppt_Resize);
			this.panelCalendar.ResumeLayout(false);
			this.panelCalendarLower.ResumeLayout(false);
			this.panelCalendarLower.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.tabWaiting.ResumeLayout(false);
			this.tabSched.ResumeLayout(false);
			this.tabProv.ResumeLayout(false);
			this.tabReminders.ResumeLayout(false);
			this.groupSearch.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.ControlApptPanel contrApptPanel;
		private System.Windows.Forms.Panel panelCalendar;
		private UI.Button butAdvSearch;
		private System.Windows.Forms.TextBox textProdGoal;
		private System.Windows.Forms.Label labelProdGoal;
		private System.Windows.Forms.RadioButton radioWeek;
		private UI.Button butBackMonth;
		private UI.Button butFwdMonth;
		private UI.Button butBackWeek;
		private UI.Button butFwdWeek;
		private UI.Button butToday;
		private UI.Button butBack;
		private UI.Button butFwd;
		private System.Windows.Forms.RadioButton radioDay;
		private UI.Button butMonth;
		private UI.Button butLab;
		private UI.Button butSearch;
		private System.Windows.Forms.TextBox textProduction;
		private System.Windows.Forms.Label labelProduction;
		private System.Windows.Forms.TextBox textLab;
		private UI.ComboBoxOD comboView;
		private System.Windows.Forms.Label label2;
		private UI.Button butClearPin;
		private UI.MonthCalendarOD monthCalendarOD;
		private System.Windows.Forms.Label labelDate;
		private System.Windows.Forms.Label labelDate2;
		private UI.ToolBarOD toolBarMain;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabWaiting;
		private UI.GridOD gridWaiting;
		private System.Windows.Forms.TabPage tabSched;
		private UI.GridOD gridEmpSched;
		private System.Windows.Forms.TabPage tabProv;
		private UI.GridOD gridProv;
		private System.Windows.Forms.TabPage tabReminders;
		private UI.GridOD gridReminders;
		private OpenDental.UI.ListBoxOD listConfirmed;
		private System.Windows.Forms.GroupBox groupSearch;
		private System.Windows.Forms.GroupBox groupBox1;
		private UI.Button butProvHygenist;
		private UI.Button butProvDentist;
		private UI.Button butProvPick;
		private UI.Button butRefresh;
		private OpenDental.UI.ListBoxOD listSearchResults;
		private UI.ListBoxOD _listBoxProviders;
		private UI.Button butSearchClose;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textAfter;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.RadioButton radioBeforePM;
		private System.Windows.Forms.RadioButton radioBeforeAM;
		private System.Windows.Forms.TextBox textBefore;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton radioAfterAM;
		private System.Windows.Forms.RadioButton radioAfterPM;
		private System.Windows.Forms.DateTimePicker dateSearch;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button butSearchCloseX;
		private UI.Button butSearchNext;
		private System.Windows.Forms.Label labelNoneView;
		private UI.PinBoard pinBoard;
		private System.Windows.Forms.ContextMenuStrip menuBlockout;
		private System.Windows.Forms.ContextMenuStrip _menuOp;
		private System.Windows.Forms.ContextMenuStrip menuApt;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.Timer timerWaitingRoom;
		private System.Windows.Forms.ContextMenu menuReminderEdit;
		private System.Windows.Forms.MenuItem menuItemReminderDone;
		private System.Windows.Forms.MenuItem menuItemReminderGoto;
		private System.Windows.Forms.ImageList imageListTasks;
		private System.Windows.Forms.Panel panelCalendarLower;
		private System.Windows.Forms.ContextMenu menuRecall;
		private System.Windows.Forms.Label label1;
		private UI.Button butFwd4;
		private UI.Button butFwd6;
		private UI.Button butFwd3;
	}
}
