namespace OpenDental {
	partial class FormRecallList {
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

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecallList));
			this.butClose = new OpenDental.UI.Button();
			this.menuRightClick = new System.Windows.Forms.ContextMenu();
			this.menuItemSeeFamily = new System.Windows.Forms.MenuItem();
			this.menuItemSeeAccount = new System.Windows.Forms.MenuItem();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageRecalls = new System.Windows.Forms.TabPage();
			this.groupEmailFromRecalls = new System.Windows.Forms.GroupBox();
			this.comboEmailFromRecalls = new System.Windows.Forms.ComboBox();
			this.panelWebSched = new System.Windows.Forms.Panel();
			this.butUndo = new OpenDental.UI.Button();
			this.butGotoFamily = new OpenDental.UI.Button();
			this.butCommlog = new OpenDental.UI.Button();
			this.butGotoAccount = new OpenDental.UI.Button();
			this.butSchedFam = new OpenDental.UI.Button();
			this.butLabelOne = new OpenDental.UI.Button();
			this.butSchedPat = new OpenDental.UI.Button();
			this.labelPatientCount = new System.Windows.Forms.Label();
			this.butECards = new OpenDental.UI.Button();
			this.butEmail = new OpenDental.UI.Button();
			this.butPostcards = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.gridRecalls = new OpenDental.UI.GridOD();
			this.groupSetStatusRecalls = new System.Windows.Forms.GroupBox();
			this.comboSetStatusRecalls = new OpenDental.UI.ComboBoxOD();
			this.butSetStatusRecalls = new OpenDental.UI.Button();
			this.butLabels = new OpenDental.UI.Button();
			this.butReport = new OpenDental.UI.Button();
			this.groupViewRecalls = new System.Windows.Forms.GroupBox();
			this.comboShowReminders = new OpenDental.UI.ComboBoxOD();
			this.comboProviderRecalls = new OpenDental.UI.ComboBoxOD();
			this.comboRecallTypes = new OpenDental.UI.ComboBoxOD();
			this.labelRecallTypes = new System.Windows.Forms.Label();
			this.checkIncludeReminded = new System.Windows.Forms.CheckBox();
			this.labelSortRecalls = new System.Windows.Forms.Label();
			this.comboSiteRecalls = new OpenDental.UI.ComboBoxOD();
			this.labelSiteRecalls = new System.Windows.Forms.Label();
			this.comboClinicRecalls = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelProviderRecalls = new System.Windows.Forms.Label();
			this.checkShowConflictingTypes = new System.Windows.Forms.CheckBox();
			this.butRefreshRecalls = new OpenDental.UI.Button();
			this.labelShowReminders = new System.Windows.Forms.Label();
			this.checkGroupFamiliesRecalls = new System.Windows.Forms.CheckBox();
			this.datePickerRecalls = new OpenDental.UI.ODDateRangePicker();
			this.comboSortRecalls = new OpenDental.UI.ComboBoxOD();
			this.tabPageReminders = new System.Windows.Forms.TabPage();
			this.gridReminders = new OpenDental.UI.GridOD();
			this.groupViewRemind = new System.Windows.Forms.GroupBox();
			this.butRefreshRemind = new OpenDental.UI.Button();
			this.comboClinicRemind = new OpenDental.UI.ComboBoxClinicPicker();
			this.datePickerRemind = new OpenDental.UI.ODDateRangePicker();
			this.tabPageReactivations = new System.Windows.Forms.TabPage();
			this.groupEmailFromReact = new System.Windows.Forms.GroupBox();
			this.comboEmailFromReact = new System.Windows.Forms.ComboBox();
			this.butReactGoToFam = new OpenDental.UI.Button();
			this.butReactComm = new OpenDental.UI.Button();
			this.butReactGoToAcct = new OpenDental.UI.Button();
			this.butReactSchedFam = new OpenDental.UI.Button();
			this.butReactSingleLabels = new OpenDental.UI.Button();
			this.butReactSchedPat = new OpenDental.UI.Button();
			this.butReactEmail = new OpenDental.UI.Button();
			this.butReactPostcardPreview = new OpenDental.UI.Button();
			this.butReactPrint = new OpenDental.UI.Button();
			this.butReactLabelPreview = new OpenDental.UI.Button();
			this.labelReactPatCount = new System.Windows.Forms.Label();
			this.gridReactivations = new OpenDental.UI.GridOD();
			this.groupSetStatusReact = new System.Windows.Forms.GroupBox();
			this.comboSetStatusReact = new OpenDental.UI.ComboBoxOD();
			this.butSetStatusReact = new OpenDental.UI.Button();
			this.groupViewReact = new System.Windows.Forms.GroupBox();
			this.comboProviderReact = new OpenDental.UI.ComboBoxOD();
			this.datePickerReact = new OpenDental.UI.ODDateRangePicker();
			this.checkExcludeInactive = new System.Windows.Forms.CheckBox();
			this.checkShowDoNotContact = new System.Windows.Forms.CheckBox();
			this.checkGroupFamiliesReact = new System.Windows.Forms.CheckBox();
			this.comboBillingTypes = new OpenDental.UI.ComboBoxOD();
			this.labelBillingTypes = new System.Windows.Forms.Label();
			this.comboShowReactivate = new OpenDental.UI.ComboBoxOD();
			this.labelShowReactivate = new System.Windows.Forms.Label();
			this.comboSortReact = new OpenDental.UI.ComboBoxOD();
			this.labelSortByReact = new System.Windows.Forms.Label();
			this.comboSiteReact = new OpenDental.UI.ComboBoxOD();
			this.labelSiteReact = new System.Windows.Forms.Label();
			this.comboClinicReact = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelProviderReact = new System.Windows.Forms.Label();
			this.butRefreshReact = new OpenDental.UI.Button();
			this.tabControl.SuspendLayout();
			this.tabPageRecalls.SuspendLayout();
			this.groupEmailFromRecalls.SuspendLayout();
			this.groupSetStatusRecalls.SuspendLayout();
			this.groupViewRecalls.SuspendLayout();
			this.tabPageReminders.SuspendLayout();
			this.groupViewRemind.SuspendLayout();
			this.tabPageReactivations.SuspendLayout();
			this.groupEmailFromReact.SuspendLayout();
			this.groupSetStatusReact.SuspendLayout();
			this.groupViewReact.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(906, 733);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// menuRightClick
			// 
			this.menuRightClick.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSeeFamily,
            this.menuItemSeeAccount});
			// 
			// menuItemSeeFamily
			// 
			this.menuItemSeeFamily.Index = 0;
			this.menuItemSeeFamily.Text = "See Family";
			this.menuItemSeeFamily.Click += new System.EventHandler(this.butGotoFamily_Click);
			// 
			// menuItemSeeAccount
			// 
			this.menuItemSeeAccount.Index = 1;
			this.menuItemSeeAccount.Text = "See Account";
			this.menuItemSeeAccount.Click += new System.EventHandler(this.butGotoAccount_Click);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPageRecalls);
			this.tabControl.Controls.Add(this.tabPageReminders);
			this.tabControl.Controls.Add(this.tabPageReactivations);
			this.tabControl.Location = new System.Drawing.Point(2, 2);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(981, 725);
			this.tabControl.TabIndex = 3;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tabPageRecalls
			// 
			this.tabPageRecalls.BackColor = System.Drawing.Color.Transparent;
			this.tabPageRecalls.Controls.Add(this.groupEmailFromRecalls);
			this.tabPageRecalls.Controls.Add(this.panelWebSched);
			this.tabPageRecalls.Controls.Add(this.butUndo);
			this.tabPageRecalls.Controls.Add(this.butGotoFamily);
			this.tabPageRecalls.Controls.Add(this.butCommlog);
			this.tabPageRecalls.Controls.Add(this.butGotoAccount);
			this.tabPageRecalls.Controls.Add(this.butSchedFam);
			this.tabPageRecalls.Controls.Add(this.butLabelOne);
			this.tabPageRecalls.Controls.Add(this.butSchedPat);
			this.tabPageRecalls.Controls.Add(this.labelPatientCount);
			this.tabPageRecalls.Controls.Add(this.butECards);
			this.tabPageRecalls.Controls.Add(this.butEmail);
			this.tabPageRecalls.Controls.Add(this.butPostcards);
			this.tabPageRecalls.Controls.Add(this.butPrint);
			this.tabPageRecalls.Controls.Add(this.gridRecalls);
			this.tabPageRecalls.Controls.Add(this.groupSetStatusRecalls);
			this.tabPageRecalls.Controls.Add(this.butLabels);
			this.tabPageRecalls.Controls.Add(this.butReport);
			this.tabPageRecalls.Controls.Add(this.groupViewRecalls);
			this.tabPageRecalls.Location = new System.Drawing.Point(4, 22);
			this.tabPageRecalls.Name = "tabPageRecalls";
			this.tabPageRecalls.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageRecalls.Size = new System.Drawing.Size(973, 699);
			this.tabPageRecalls.TabIndex = 0;
			this.tabPageRecalls.Tag = this.gridRecalls;
			this.tabPageRecalls.Text = "Recalls";
			// 
			// groupEmailFromRecalls
			// 
			this.groupEmailFromRecalls.Controls.Add(this.comboEmailFromRecalls);
			this.groupEmailFromRecalls.Location = new System.Drawing.Point(739, 62);
			this.groupEmailFromRecalls.Name = "groupEmailFromRecalls";
			this.groupEmailFromRecalls.Size = new System.Drawing.Size(228, 44);
			this.groupEmailFromRecalls.TabIndex = 125;
			this.groupEmailFromRecalls.TabStop = false;
			this.groupEmailFromRecalls.Text = "Email From";
			// 
			// comboEmailFromRecalls
			// 
			this.comboEmailFromRecalls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEmailFromRecalls.Location = new System.Drawing.Point(6, 17);
			this.comboEmailFromRecalls.MaxDropDownItems = 40;
			this.comboEmailFromRecalls.Name = "comboEmailFromRecalls";
			this.comboEmailFromRecalls.Size = new System.Drawing.Size(216, 21);
			this.comboEmailFromRecalls.TabIndex = 65;
			// 
			// panelWebSched
			// 
			this.panelWebSched.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panelWebSched.BackgroundImage = global::OpenDental.Properties.Resources.webSched_PV_Button;
			this.panelWebSched.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.panelWebSched.Location = new System.Drawing.Point(759, 669);
			this.panelWebSched.Name = "panelWebSched";
			this.panelWebSched.Size = new System.Drawing.Size(120, 24);
			this.panelWebSched.TabIndex = 138;
			this.panelWebSched.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelWebSched_MouseClick);
			// 
			// butUndo
			// 
			this.butUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUndo.Location = new System.Drawing.Point(3, 669);
			this.butUndo.Name = "butUndo";
			this.butUndo.Size = new System.Drawing.Size(119, 24);
			this.butUndo.TabIndex = 137;
			this.butUndo.Text = "Undo";
			this.butUndo.Click += new System.EventHandler(this.butUndo_Click);
			// 
			// butGotoFamily
			// 
			this.butGotoFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGotoFamily.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGotoFamily.Location = new System.Drawing.Point(443, 643);
			this.butGotoFamily.Name = "butGotoFamily";
			this.butGotoFamily.Size = new System.Drawing.Size(96, 24);
			this.butGotoFamily.TabIndex = 136;
			this.butGotoFamily.Text = "Go to Family";
			this.butGotoFamily.Click += new System.EventHandler(this.butGotoFamily_Click);
			// 
			// butCommlog
			// 
			this.butCommlog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCommlog.Icon = OpenDental.UI.EnumIcons.CommLog;
			this.butCommlog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCommlog.Location = new System.Drawing.Point(545, 669);
			this.butCommlog.Name = "butCommlog";
			this.butCommlog.Size = new System.Drawing.Size(88, 24);
			this.butCommlog.TabIndex = 135;
			this.butCommlog.Text = "Comm";
			this.butCommlog.Click += new System.EventHandler(this.butCommlog_Click);
			// 
			// butGotoAccount
			// 
			this.butGotoAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGotoAccount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGotoAccount.Location = new System.Drawing.Point(443, 669);
			this.butGotoAccount.Name = "butGotoAccount";
			this.butGotoAccount.Size = new System.Drawing.Size(96, 24);
			this.butGotoAccount.TabIndex = 134;
			this.butGotoAccount.Text = "Go to Account";
			this.butGotoAccount.Click += new System.EventHandler(this.butGotoAccount_Click);
			// 
			// butSchedFam
			// 
			this.butSchedFam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSchedFam.Image = global::OpenDental.Properties.Resources.butPin22;
			this.butSchedFam.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSchedFam.Location = new System.Drawing.Point(639, 669);
			this.butSchedFam.Name = "butSchedFam";
			this.butSchedFam.Size = new System.Drawing.Size(114, 24);
			this.butSchedFam.TabIndex = 129;
			this.butSchedFam.Tag = "SchedFamRecall";
			this.butSchedFam.Text = "Sched Family";
			this.butSchedFam.Click += new System.EventHandler(this.butSched_Click);
			// 
			// butLabelOne
			// 
			this.butLabelOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabelOne.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabelOne.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabelOne.Location = new System.Drawing.Point(128, 643);
			this.butLabelOne.Name = "butLabelOne";
			this.butLabelOne.Size = new System.Drawing.Size(119, 24);
			this.butLabelOne.TabIndex = 133;
			this.butLabelOne.Text = "Single Labels";
			this.butLabelOne.Click += new System.EventHandler(this.butLabelOne_Click);
			// 
			// butSchedPat
			// 
			this.butSchedPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSchedPat.Image = global::OpenDental.Properties.Resources.butPin22;
			this.butSchedPat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSchedPat.Location = new System.Drawing.Point(639, 643);
			this.butSchedPat.Name = "butSchedPat";
			this.butSchedPat.Size = new System.Drawing.Size(114, 24);
			this.butSchedPat.TabIndex = 128;
			this.butSchedPat.Tag = "SchedPatRecall";
			this.butSchedPat.Text = "Sched Patient";
			this.butSchedPat.Click += new System.EventHandler(this.butSched_Click);
			// 
			// labelPatientCount
			// 
			this.labelPatientCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPatientCount.Location = new System.Drawing.Point(856, 641);
			this.labelPatientCount.Name = "labelPatientCount";
			this.labelPatientCount.Size = new System.Drawing.Size(114, 14);
			this.labelPatientCount.TabIndex = 132;
			this.labelPatientCount.Text = "Patient Count:";
			this.labelPatientCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butECards
			// 
			this.butECards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butECards.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butECards.Location = new System.Drawing.Point(253, 643);
			this.butECards.Name = "butECards";
			this.butECards.Size = new System.Drawing.Size(91, 24);
			this.butECards.TabIndex = 130;
			this.butECards.Text = "eCards";
			this.butECards.Visible = false;
			this.butECards.Click += new System.EventHandler(this.butECards_Click);
			// 
			// butEmail
			// 
			this.butEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEmail.Icon = OpenDental.UI.EnumIcons.Email;
			this.butEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEmail.Location = new System.Drawing.Point(253, 669);
			this.butEmail.Name = "butEmail";
			this.butEmail.Size = new System.Drawing.Size(91, 24);
			this.butEmail.TabIndex = 131;
			this.butEmail.Text = "E-Mail";
			this.butEmail.Click += new System.EventHandler(this.butEmail_Click);
			// 
			// butPostcards
			// 
			this.butPostcards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPostcards.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPostcards.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPostcards.Location = new System.Drawing.Point(3, 643);
			this.butPostcards.Name = "butPostcards";
			this.butPostcards.Size = new System.Drawing.Size(119, 24);
			this.butPostcards.TabIndex = 124;
			this.butPostcards.Text = "Postcard Preview";
			this.butPostcards.Click += new System.EventHandler(this.butPostcards_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(350, 669);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 127;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// gridRecalls
			// 
			this.gridRecalls.AllowSortingByColumn = true;
			this.gridRecalls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRecalls.HScrollVisible = true;
			this.gridRecalls.Location = new System.Drawing.Point(3, 112);
			this.gridRecalls.Name = "gridRecalls";
			this.gridRecalls.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridRecalls.Size = new System.Drawing.Size(967, 526);
			this.gridRecalls.TabIndex = 126;
			this.gridRecalls.Title = "Recall List";
			this.gridRecalls.TranslationName = "TableRecallList";
			this.gridRecalls.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRecalls_CellDoubleClick);
			this.gridRecalls.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			// 
			// groupSetStatusRecalls
			// 
			this.groupSetStatusRecalls.Controls.Add(this.comboSetStatusRecalls);
			this.groupSetStatusRecalls.Controls.Add(this.butSetStatusRecalls);
			this.groupSetStatusRecalls.Location = new System.Drawing.Point(739, 6);
			this.groupSetStatusRecalls.Name = "groupSetStatusRecalls";
			this.groupSetStatusRecalls.Size = new System.Drawing.Size(228, 47);
			this.groupSetStatusRecalls.TabIndex = 123;
			this.groupSetStatusRecalls.TabStop = false;
			this.groupSetStatusRecalls.Text = "Set Status";
			// 
			// comboSetStatusRecalls
			// 
			this.comboSetStatusRecalls.Location = new System.Drawing.Point(6, 19);
			this.comboSetStatusRecalls.Name = "comboSetStatusRecalls";
			this.comboSetStatusRecalls.Size = new System.Drawing.Size(160, 21);
			this.comboSetStatusRecalls.TabIndex = 15;
			// 
			// butSetStatusRecalls
			// 
			this.butSetStatusRecalls.Location = new System.Drawing.Point(172, 17);
			this.butSetStatusRecalls.Name = "butSetStatusRecalls";
			this.butSetStatusRecalls.Size = new System.Drawing.Size(50, 24);
			this.butSetStatusRecalls.TabIndex = 14;
			this.butSetStatusRecalls.Text = "Set";
			this.butSetStatusRecalls.Click += new System.EventHandler(this.butSetStatusRecalls_Click);
			// 
			// butLabels
			// 
			this.butLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabels.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabels.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabels.Location = new System.Drawing.Point(128, 669);
			this.butLabels.Name = "butLabels";
			this.butLabels.Size = new System.Drawing.Size(119, 24);
			this.butLabels.TabIndex = 122;
			this.butLabels.Text = "Label Preview";
			this.butLabels.Click += new System.EventHandler(this.butLabels_Click);
			// 
			// butReport
			// 
			this.butReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReport.Location = new System.Drawing.Point(350, 643);
			this.butReport.Name = "butReport";
			this.butReport.Size = new System.Drawing.Size(87, 24);
			this.butReport.TabIndex = 121;
			this.butReport.Text = "R&un Report";
			this.butReport.Click += new System.EventHandler(this.butReport_Click);
			// 
			// groupViewRecalls
			// 
			this.groupViewRecalls.Controls.Add(this.comboShowReminders);
			this.groupViewRecalls.Controls.Add(this.comboProviderRecalls);
			this.groupViewRecalls.Controls.Add(this.comboRecallTypes);
			this.groupViewRecalls.Controls.Add(this.labelRecallTypes);
			this.groupViewRecalls.Controls.Add(this.checkIncludeReminded);
			this.groupViewRecalls.Controls.Add(this.labelSortRecalls);
			this.groupViewRecalls.Controls.Add(this.comboSiteRecalls);
			this.groupViewRecalls.Controls.Add(this.labelSiteRecalls);
			this.groupViewRecalls.Controls.Add(this.comboClinicRecalls);
			this.groupViewRecalls.Controls.Add(this.labelProviderRecalls);
			this.groupViewRecalls.Controls.Add(this.checkShowConflictingTypes);
			this.groupViewRecalls.Controls.Add(this.butRefreshRecalls);
			this.groupViewRecalls.Controls.Add(this.labelShowReminders);
			this.groupViewRecalls.Controls.Add(this.checkGroupFamiliesRecalls);
			this.groupViewRecalls.Controls.Add(this.datePickerRecalls);
			this.groupViewRecalls.Controls.Add(this.comboSortRecalls);
			this.groupViewRecalls.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupViewRecalls.Location = new System.Drawing.Point(6, 6);
			this.groupViewRecalls.Name = "groupViewRecalls";
			this.groupViewRecalls.Size = new System.Drawing.Size(727, 100);
			this.groupViewRecalls.TabIndex = 120;
			this.groupViewRecalls.TabStop = false;
			this.groupViewRecalls.Text = "View";
			// 
			// comboShowReminders
			// 
			this.comboShowReminders.Location = new System.Drawing.Point(244, 14);
			this.comboShowReminders.Name = "comboShowReminders";
			this.comboShowReminders.Size = new System.Drawing.Size(102, 21);
			this.comboShowReminders.TabIndex = 39;
			// 
			// comboProviderRecalls
			// 
			this.comboProviderRecalls.Location = new System.Drawing.Point(420, 14);
			this.comboProviderRecalls.Name = "comboProviderRecalls";
			this.comboProviderRecalls.Size = new System.Drawing.Size(160, 21);
			this.comboProviderRecalls.TabIndex = 41;
			// 
			// comboRecallTypes
			// 
			this.comboRecallTypes.Location = new System.Drawing.Point(244, 37);
			this.comboRecallTypes.Name = "comboRecallTypes";
			this.comboRecallTypes.SelectionModeMulti = true;
			this.comboRecallTypes.Size = new System.Drawing.Size(102, 21);
			this.comboRecallTypes.TabIndex = 44;
			// 
			// labelRecallTypes
			// 
			this.labelRecallTypes.Location = new System.Drawing.Point(151, 39);
			this.labelRecallTypes.Name = "labelRecallTypes";
			this.labelRecallTypes.Size = new System.Drawing.Size(92, 17);
			this.labelRecallTypes.TabIndex = 43;
			this.labelRecallTypes.Text = "Recall Type";
			this.labelRecallTypes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIncludeReminded
			// 
			this.checkIncludeReminded.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeReminded.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeReminded.Location = new System.Drawing.Point(586, 48);
			this.checkIncludeReminded.Name = "checkIncludeReminded";
			this.checkIncludeReminded.Size = new System.Drawing.Size(135, 16);
			this.checkIncludeReminded.TabIndex = 42;
			this.checkIncludeReminded.Text = "Include Reminded";
			this.checkIncludeReminded.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeReminded.UseVisualStyleBackColor = true;
			// 
			// labelSortRecalls
			// 
			this.labelSortRecalls.Location = new System.Drawing.Point(151, 62);
			this.labelSortRecalls.Name = "labelSortRecalls";
			this.labelSortRecalls.Size = new System.Drawing.Size(92, 17);
			this.labelSortRecalls.TabIndex = 36;
			this.labelSortRecalls.Text = "Sort";
			this.labelSortRecalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSiteRecalls
			// 
			this.comboSiteRecalls.Location = new System.Drawing.Point(420, 60);
			this.comboSiteRecalls.Name = "comboSiteRecalls";
			this.comboSiteRecalls.Size = new System.Drawing.Size(160, 21);
			this.comboSiteRecalls.TabIndex = 25;
			// 
			// labelSiteRecalls
			// 
			this.labelSiteRecalls.Location = new System.Drawing.Point(352, 62);
			this.labelSiteRecalls.Name = "labelSiteRecalls";
			this.labelSiteRecalls.Size = new System.Drawing.Size(67, 17);
			this.labelSiteRecalls.TabIndex = 24;
			this.labelSiteRecalls.Text = "Site";
			this.labelSiteRecalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinicRecalls
			// 
			this.comboClinicRecalls.IncludeAll = true;
			this.comboClinicRecalls.IncludeUnassigned = true;
			this.comboClinicRecalls.Location = new System.Drawing.Point(383, 37);
			this.comboClinicRecalls.Name = "comboClinicRecalls";
			this.comboClinicRecalls.Size = new System.Drawing.Size(197, 21);
			this.comboClinicRecalls.TabIndex = 23;
			// 
			// labelProviderRecalls
			// 
			this.labelProviderRecalls.Location = new System.Drawing.Point(352, 16);
			this.labelProviderRecalls.Name = "labelProviderRecalls";
			this.labelProviderRecalls.Size = new System.Drawing.Size(67, 17);
			this.labelProviderRecalls.TabIndex = 20;
			this.labelProviderRecalls.Text = "Provider";
			this.labelProviderRecalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowConflictingTypes
			// 
			this.checkShowConflictingTypes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowConflictingTypes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowConflictingTypes.Location = new System.Drawing.Point(586, 31);
			this.checkShowConflictingTypes.Name = "checkShowConflictingTypes";
			this.checkShowConflictingTypes.Size = new System.Drawing.Size(135, 16);
			this.checkShowConflictingTypes.TabIndex = 40;
			this.checkShowConflictingTypes.Text = "Show Conflicting Types";
			this.checkShowConflictingTypes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowConflictingTypes.UseVisualStyleBackColor = true;
			this.checkShowConflictingTypes.Click += new System.EventHandler(this.checkShowConflictingTypes_Click);
			// 
			// butRefreshRecalls
			// 
			this.butRefreshRecalls.Location = new System.Drawing.Point(641, 70);
			this.butRefreshRecalls.Name = "butRefreshRecalls";
			this.butRefreshRecalls.Size = new System.Drawing.Size(80, 24);
			this.butRefreshRecalls.TabIndex = 2;
			this.butRefreshRecalls.Text = "&Refresh List";
			this.butRefreshRecalls.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// labelShowReminders
			// 
			this.labelShowReminders.Location = new System.Drawing.Point(151, 16);
			this.labelShowReminders.Name = "labelShowReminders";
			this.labelShowReminders.Size = new System.Drawing.Size(92, 17);
			this.labelShowReminders.TabIndex = 38;
			this.labelShowReminders.Text = "Show Reminders";
			this.labelShowReminders.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkGroupFamiliesRecalls
			// 
			this.checkGroupFamiliesRecalls.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamiliesRecalls.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGroupFamiliesRecalls.Location = new System.Drawing.Point(586, 14);
			this.checkGroupFamiliesRecalls.Name = "checkGroupFamiliesRecalls";
			this.checkGroupFamiliesRecalls.Size = new System.Drawing.Size(135, 16);
			this.checkGroupFamiliesRecalls.TabIndex = 19;
			this.checkGroupFamiliesRecalls.Text = "Group Families";
			this.checkGroupFamiliesRecalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamiliesRecalls.UseVisualStyleBackColor = true;
			this.checkGroupFamiliesRecalls.Click += new System.EventHandler(this.checkGroupFamilies_Click);
			// 
			// datePickerRecalls
			// 
			this.datePickerRecalls.BackColor = System.Drawing.Color.Transparent;
			this.datePickerRecalls.EnableWeekButtons = false;
			this.datePickerRecalls.IsVertical = true;
			this.datePickerRecalls.Location = new System.Drawing.Point(-20, 12);
			this.datePickerRecalls.MinimumSize = new System.Drawing.Size(165, 46);
			this.datePickerRecalls.Name = "datePickerRecalls";
			this.datePickerRecalls.Size = new System.Drawing.Size(168, 46);
			this.datePickerRecalls.TabIndex = 4;
			// 
			// comboSortRecalls
			// 
			this.comboSortRecalls.Location = new System.Drawing.Point(244, 60);
			this.comboSortRecalls.Name = "comboSortRecalls";
			this.comboSortRecalls.Size = new System.Drawing.Size(102, 21);
			this.comboSortRecalls.TabIndex = 37;
			// 
			// tabPageReminders
			// 
			this.tabPageReminders.BackColor = System.Drawing.Color.Transparent;
			this.tabPageReminders.Controls.Add(this.gridReminders);
			this.tabPageReminders.Controls.Add(this.groupViewRemind);
			this.tabPageReminders.Location = new System.Drawing.Point(4, 22);
			this.tabPageReminders.Name = "tabPageReminders";
			this.tabPageReminders.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageReminders.Size = new System.Drawing.Size(973, 699);
			this.tabPageReminders.TabIndex = 1;
			this.tabPageReminders.Tag = this.gridReminders;
			this.tabPageReminders.Text = "Reminders";
			// 
			// gridReminders
			// 
			this.gridReminders.AllowSortingByColumn = true;
			this.gridReminders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridReminders.HScrollVisible = true;
			this.gridReminders.Location = new System.Drawing.Point(3, 76);
			this.gridReminders.Name = "gridReminders";
			this.gridReminders.Size = new System.Drawing.Size(967, 617);
			this.gridReminders.TabIndex = 127;
			this.gridReminders.Title = "Reminder List";
			this.gridReminders.TranslationName = "TableReminderList";
			// 
			// groupViewRemind
			// 
			this.groupViewRemind.Controls.Add(this.butRefreshRemind);
			this.groupViewRemind.Controls.Add(this.comboClinicRemind);
			this.groupViewRemind.Controls.Add(this.datePickerRemind);
			this.groupViewRemind.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupViewRemind.Location = new System.Drawing.Point(6, 6);
			this.groupViewRemind.Name = "groupViewRemind";
			this.groupViewRemind.Size = new System.Drawing.Size(464, 64);
			this.groupViewRemind.TabIndex = 121;
			this.groupViewRemind.TabStop = false;
			this.groupViewRemind.Text = "View";
			// 
			// butRefreshRemind
			// 
			this.butRefreshRemind.Location = new System.Drawing.Point(378, 34);
			this.butRefreshRemind.Name = "butRefreshRemind";
			this.butRefreshRemind.Size = new System.Drawing.Size(80, 24);
			this.butRefreshRemind.TabIndex = 2;
			this.butRefreshRemind.Text = "&Refresh List";
			this.butRefreshRemind.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboClinicRemind
			// 
			this.comboClinicRemind.IncludeAll = true;
			this.comboClinicRemind.IncludeUnassigned = true;
			this.comboClinicRemind.Location = new System.Drawing.Point(160, 14);
			this.comboClinicRemind.Name = "comboClinicRemind";
			this.comboClinicRemind.Size = new System.Drawing.Size(198, 21);
			this.comboClinicRemind.TabIndex = 23;
			// 
			// datePickerRemind
			// 
			this.datePickerRemind.BackColor = System.Drawing.Color.Transparent;
			this.datePickerRemind.EnableWeekButtons = false;
			this.datePickerRemind.IsVertical = true;
			this.datePickerRemind.Location = new System.Drawing.Point(-20, 12);
			this.datePickerRemind.MaximumSize = new System.Drawing.Size(0, 185);
			this.datePickerRemind.MinimumSize = new System.Drawing.Size(168, 46);
			this.datePickerRemind.Name = "datePickerRemind";
			this.datePickerRemind.Size = new System.Drawing.Size(168, 46);
			this.datePickerRemind.TabIndex = 24;
			// 
			// tabPageReactivations
			// 
			this.tabPageReactivations.BackColor = System.Drawing.Color.Transparent;
			this.tabPageReactivations.Controls.Add(this.groupEmailFromReact);
			this.tabPageReactivations.Controls.Add(this.butReactGoToFam);
			this.tabPageReactivations.Controls.Add(this.butReactComm);
			this.tabPageReactivations.Controls.Add(this.butReactGoToAcct);
			this.tabPageReactivations.Controls.Add(this.butReactSchedFam);
			this.tabPageReactivations.Controls.Add(this.butReactSingleLabels);
			this.tabPageReactivations.Controls.Add(this.butReactSchedPat);
			this.tabPageReactivations.Controls.Add(this.butReactEmail);
			this.tabPageReactivations.Controls.Add(this.butReactPostcardPreview);
			this.tabPageReactivations.Controls.Add(this.butReactPrint);
			this.tabPageReactivations.Controls.Add(this.butReactLabelPreview);
			this.tabPageReactivations.Controls.Add(this.labelReactPatCount);
			this.tabPageReactivations.Controls.Add(this.gridReactivations);
			this.tabPageReactivations.Controls.Add(this.groupSetStatusReact);
			this.tabPageReactivations.Controls.Add(this.groupViewReact);
			this.tabPageReactivations.Location = new System.Drawing.Point(4, 22);
			this.tabPageReactivations.Name = "tabPageReactivations";
			this.tabPageReactivations.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageReactivations.Size = new System.Drawing.Size(973, 699);
			this.tabPageReactivations.TabIndex = 2;
			this.tabPageReactivations.Tag = this.gridReactivations;
			this.tabPageReactivations.Text = "Reactivations";
			// 
			// groupEmailFromReact
			// 
			this.groupEmailFromReact.Controls.Add(this.comboEmailFromReact);
			this.groupEmailFromReact.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupEmailFromReact.Location = new System.Drawing.Point(739, 62);
			this.groupEmailFromReact.Name = "groupEmailFromReact";
			this.groupEmailFromReact.Size = new System.Drawing.Size(228, 44);
			this.groupEmailFromReact.TabIndex = 163;
			this.groupEmailFromReact.TabStop = false;
			this.groupEmailFromReact.Text = "Email From";
			// 
			// comboEmailFromReact
			// 
			this.comboEmailFromReact.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEmailFromReact.Location = new System.Drawing.Point(6, 17);
			this.comboEmailFromReact.MaxDropDownItems = 40;
			this.comboEmailFromReact.Name = "comboEmailFromReact";
			this.comboEmailFromReact.Size = new System.Drawing.Size(216, 21);
			this.comboEmailFromReact.TabIndex = 65;
			// 
			// butReactGoToFam
			// 
			this.butReactGoToFam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactGoToFam.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactGoToFam.Location = new System.Drawing.Point(443, 643);
			this.butReactGoToFam.Name = "butReactGoToFam";
			this.butReactGoToFam.Size = new System.Drawing.Size(96, 24);
			this.butReactGoToFam.TabIndex = 162;
			this.butReactGoToFam.Text = "Go to Family";
			this.butReactGoToFam.Click += new System.EventHandler(this.butGotoFamily_Click);
			// 
			// butReactComm
			// 
			this.butReactComm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactComm.Icon = OpenDental.UI.EnumIcons.CommLog;
			this.butReactComm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactComm.Location = new System.Drawing.Point(545, 669);
			this.butReactComm.Name = "butReactComm";
			this.butReactComm.Size = new System.Drawing.Size(88, 24);
			this.butReactComm.TabIndex = 161;
			this.butReactComm.Text = "Comm";
			this.butReactComm.Click += new System.EventHandler(this.butCommlog_Click);
			// 
			// butReactGoToAcct
			// 
			this.butReactGoToAcct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactGoToAcct.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactGoToAcct.Location = new System.Drawing.Point(443, 669);
			this.butReactGoToAcct.Name = "butReactGoToAcct";
			this.butReactGoToAcct.Size = new System.Drawing.Size(96, 24);
			this.butReactGoToAcct.TabIndex = 160;
			this.butReactGoToAcct.Text = "Go to Account";
			this.butReactGoToAcct.Click += new System.EventHandler(this.butGotoAccount_Click);
			// 
			// butReactSchedFam
			// 
			this.butReactSchedFam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactSchedFam.Image = global::OpenDental.Properties.Resources.butPin22;
			this.butReactSchedFam.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactSchedFam.Location = new System.Drawing.Point(639, 669);
			this.butReactSchedFam.Name = "butReactSchedFam";
			this.butReactSchedFam.Size = new System.Drawing.Size(114, 24);
			this.butReactSchedFam.TabIndex = 156;
			this.butReactSchedFam.Tag = "SchedFamReact";
			this.butReactSchedFam.Text = "Sched Family";
			this.butReactSchedFam.Click += new System.EventHandler(this.butSched_Click);
			// 
			// butReactSingleLabels
			// 
			this.butReactSingleLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactSingleLabels.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butReactSingleLabels.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactSingleLabels.Location = new System.Drawing.Point(128, 643);
			this.butReactSingleLabels.Name = "butReactSingleLabels";
			this.butReactSingleLabels.Size = new System.Drawing.Size(119, 24);
			this.butReactSingleLabels.TabIndex = 159;
			this.butReactSingleLabels.Text = "Single Labels";
			this.butReactSingleLabels.Click += new System.EventHandler(this.butLabelOne_Click);
			// 
			// butReactSchedPat
			// 
			this.butReactSchedPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactSchedPat.Image = global::OpenDental.Properties.Resources.butPin22;
			this.butReactSchedPat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactSchedPat.Location = new System.Drawing.Point(639, 643);
			this.butReactSchedPat.Name = "butReactSchedPat";
			this.butReactSchedPat.Size = new System.Drawing.Size(114, 24);
			this.butReactSchedPat.TabIndex = 155;
			this.butReactSchedPat.Tag = "SchedPatReact";
			this.butReactSchedPat.Text = "Sched Patient";
			this.butReactSchedPat.Click += new System.EventHandler(this.butSched_Click);
			// 
			// butReactEmail
			// 
			this.butReactEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactEmail.Icon = OpenDental.UI.EnumIcons.Email;
			this.butReactEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactEmail.Location = new System.Drawing.Point(253, 669);
			this.butReactEmail.Name = "butReactEmail";
			this.butReactEmail.Size = new System.Drawing.Size(91, 24);
			this.butReactEmail.TabIndex = 158;
			this.butReactEmail.Text = "E-Mail";
			this.butReactEmail.Click += new System.EventHandler(this.butEmail_Click);
			// 
			// butReactPostcardPreview
			// 
			this.butReactPostcardPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactPostcardPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butReactPostcardPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactPostcardPreview.Location = new System.Drawing.Point(3, 643);
			this.butReactPostcardPreview.Name = "butReactPostcardPreview";
			this.butReactPostcardPreview.Size = new System.Drawing.Size(119, 24);
			this.butReactPostcardPreview.TabIndex = 153;
			this.butReactPostcardPreview.Text = "Postcard Preview";
			this.butReactPostcardPreview.Click += new System.EventHandler(this.butPostcards_Click);
			// 
			// butReactPrint
			// 
			this.butReactPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butReactPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactPrint.Location = new System.Drawing.Point(350, 669);
			this.butReactPrint.Name = "butReactPrint";
			this.butReactPrint.Size = new System.Drawing.Size(87, 24);
			this.butReactPrint.TabIndex = 154;
			this.butReactPrint.Text = "Print List";
			this.butReactPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butReactLabelPreview
			// 
			this.butReactLabelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReactLabelPreview.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butReactLabelPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReactLabelPreview.Location = new System.Drawing.Point(128, 669);
			this.butReactLabelPreview.Name = "butReactLabelPreview";
			this.butReactLabelPreview.Size = new System.Drawing.Size(119, 24);
			this.butReactLabelPreview.TabIndex = 152;
			this.butReactLabelPreview.Text = "Label Preview";
			this.butReactLabelPreview.Click += new System.EventHandler(this.butLabels_Click);
			// 
			// labelReactPatCount
			// 
			this.labelReactPatCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelReactPatCount.Location = new System.Drawing.Point(856, 641);
			this.labelReactPatCount.Name = "labelReactPatCount";
			this.labelReactPatCount.Size = new System.Drawing.Size(114, 14);
			this.labelReactPatCount.TabIndex = 151;
			this.labelReactPatCount.Text = "Patient Count:";
			this.labelReactPatCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gridReactivations
			// 
			this.gridReactivations.AllowSortingByColumn = true;
			this.gridReactivations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridReactivations.HScrollVisible = true;
			this.gridReactivations.Location = new System.Drawing.Point(3, 112);
			this.gridReactivations.Name = "gridReactivations";
			this.gridReactivations.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridReactivations.Size = new System.Drawing.Size(967, 526);
			this.gridReactivations.TabIndex = 145;
			this.gridReactivations.Title = "Reactivation List";
			this.gridReactivations.TranslationName = "TableReactivationList";
			this.gridReactivations.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridReactivations_CellDoubleClick);
			this.gridReactivations.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellClick);
			// 
			// groupSetStatusReact
			// 
			this.groupSetStatusReact.Controls.Add(this.comboSetStatusReact);
			this.groupSetStatusReact.Controls.Add(this.butSetStatusReact);
			this.groupSetStatusReact.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupSetStatusReact.Location = new System.Drawing.Point(739, 6);
			this.groupSetStatusReact.Name = "groupSetStatusReact";
			this.groupSetStatusReact.Size = new System.Drawing.Size(228, 47);
			this.groupSetStatusReact.TabIndex = 142;
			this.groupSetStatusReact.TabStop = false;
			this.groupSetStatusReact.Text = "Set Status";
			// 
			// comboSetStatusReact
			// 
			this.comboSetStatusReact.Location = new System.Drawing.Point(6, 19);
			this.comboSetStatusReact.Name = "comboSetStatusReact";
			this.comboSetStatusReact.Size = new System.Drawing.Size(160, 21);
			this.comboSetStatusReact.TabIndex = 15;
			// 
			// butSetStatusReact
			// 
			this.butSetStatusReact.Location = new System.Drawing.Point(172, 17);
			this.butSetStatusReact.Name = "butSetStatusReact";
			this.butSetStatusReact.Size = new System.Drawing.Size(50, 24);
			this.butSetStatusReact.TabIndex = 14;
			this.butSetStatusReact.Text = "Set";
			this.butSetStatusReact.Click += new System.EventHandler(this.butSetStatusReact_Click);
			// 
			// groupViewReact
			// 
			this.groupViewReact.Controls.Add(this.comboProviderReact);
			this.groupViewReact.Controls.Add(this.datePickerReact);
			this.groupViewReact.Controls.Add(this.checkExcludeInactive);
			this.groupViewReact.Controls.Add(this.checkShowDoNotContact);
			this.groupViewReact.Controls.Add(this.checkGroupFamiliesReact);
			this.groupViewReact.Controls.Add(this.comboBillingTypes);
			this.groupViewReact.Controls.Add(this.labelBillingTypes);
			this.groupViewReact.Controls.Add(this.comboShowReactivate);
			this.groupViewReact.Controls.Add(this.labelShowReactivate);
			this.groupViewReact.Controls.Add(this.comboSortReact);
			this.groupViewReact.Controls.Add(this.labelSortByReact);
			this.groupViewReact.Controls.Add(this.comboSiteReact);
			this.groupViewReact.Controls.Add(this.labelSiteReact);
			this.groupViewReact.Controls.Add(this.comboClinicReact);
			this.groupViewReact.Controls.Add(this.labelProviderReact);
			this.groupViewReact.Controls.Add(this.butRefreshReact);
			this.groupViewReact.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupViewReact.Location = new System.Drawing.Point(6, 6);
			this.groupViewReact.Name = "groupViewReact";
			this.groupViewReact.Size = new System.Drawing.Size(727, 100);
			this.groupViewReact.TabIndex = 139;
			this.groupViewReact.TabStop = false;
			this.groupViewReact.Text = "View";
			// 
			// comboProviderReact
			// 
			this.comboProviderReact.Location = new System.Drawing.Point(420, 14);
			this.comboProviderReact.Name = "comboProviderReact";
			this.comboProviderReact.Size = new System.Drawing.Size(160, 21);
			this.comboProviderReact.TabIndex = 43;
			// 
			// datePickerReact
			// 
			this.datePickerReact.BackColor = System.Drawing.Color.Transparent;
			this.datePickerReact.EnableWeekButtons = false;
			this.datePickerReact.IsVertical = true;
			this.datePickerReact.Location = new System.Drawing.Point(-20, 12);
			this.datePickerReact.MinimumSize = new System.Drawing.Size(165, 46);
			this.datePickerReact.Name = "datePickerReact";
			this.datePickerReact.Size = new System.Drawing.Size(168, 46);
			this.datePickerReact.TabIndex = 48;
			// 
			// checkExcludeInactive
			// 
			this.checkExcludeInactive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeInactive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeInactive.Location = new System.Drawing.Point(586, 48);
			this.checkExcludeInactive.Name = "checkExcludeInactive";
			this.checkExcludeInactive.Size = new System.Drawing.Size(135, 16);
			this.checkExcludeInactive.TabIndex = 47;
			this.checkExcludeInactive.Text = "Exclude Inactive";
			this.checkExcludeInactive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeInactive.UseVisualStyleBackColor = true;
			// 
			// checkShowDoNotContact
			// 
			this.checkShowDoNotContact.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDoNotContact.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowDoNotContact.Location = new System.Drawing.Point(586, 31);
			this.checkShowDoNotContact.Name = "checkShowDoNotContact";
			this.checkShowDoNotContact.Size = new System.Drawing.Size(135, 16);
			this.checkShowDoNotContact.TabIndex = 42;
			this.checkShowDoNotContact.Text = "Show Do Not Contact";
			this.checkShowDoNotContact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDoNotContact.UseVisualStyleBackColor = true;
			// 
			// checkGroupFamiliesReact
			// 
			this.checkGroupFamiliesReact.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamiliesReact.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGroupFamiliesReact.Location = new System.Drawing.Point(586, 14);
			this.checkGroupFamiliesReact.Name = "checkGroupFamiliesReact";
			this.checkGroupFamiliesReact.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkGroupFamiliesReact.Size = new System.Drawing.Size(135, 16);
			this.checkGroupFamiliesReact.TabIndex = 44;
			this.checkGroupFamiliesReact.Text = "Group Families";
			this.checkGroupFamiliesReact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGroupFamiliesReact.UseVisualStyleBackColor = true;
			this.checkGroupFamiliesReact.MouseClick += new System.Windows.Forms.MouseEventHandler(this.checkGroupFamilies_Click);
			// 
			// comboBillingTypes
			// 
			this.comboBillingTypes.Location = new System.Drawing.Point(244, 37);
			this.comboBillingTypes.Name = "comboBillingTypes";
			this.comboBillingTypes.Size = new System.Drawing.Size(102, 21);
			this.comboBillingTypes.TabIndex = 41;
			// 
			// labelBillingTypes
			// 
			this.labelBillingTypes.Location = new System.Drawing.Point(151, 39);
			this.labelBillingTypes.Name = "labelBillingTypes";
			this.labelBillingTypes.Size = new System.Drawing.Size(92, 17);
			this.labelBillingTypes.TabIndex = 40;
			this.labelBillingTypes.Text = "Billing Type";
			this.labelBillingTypes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboShowReactivate
			// 
			this.comboShowReactivate.Location = new System.Drawing.Point(244, 14);
			this.comboShowReactivate.Name = "comboShowReactivate";
			this.comboShowReactivate.Size = new System.Drawing.Size(102, 21);
			this.comboShowReactivate.TabIndex = 39;
			// 
			// labelShowReactivate
			// 
			this.labelShowReactivate.Location = new System.Drawing.Point(151, 16);
			this.labelShowReactivate.Name = "labelShowReactivate";
			this.labelShowReactivate.Size = new System.Drawing.Size(92, 17);
			this.labelShowReactivate.TabIndex = 38;
			this.labelShowReactivate.Text = "Show Reactivate";
			this.labelShowReactivate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSortReact
			// 
			this.comboSortReact.Location = new System.Drawing.Point(244, 60);
			this.comboSortReact.Name = "comboSortReact";
			this.comboSortReact.Size = new System.Drawing.Size(102, 21);
			this.comboSortReact.TabIndex = 37;
			// 
			// labelSortByReact
			// 
			this.labelSortByReact.Location = new System.Drawing.Point(151, 62);
			this.labelSortByReact.Name = "labelSortByReact";
			this.labelSortByReact.Size = new System.Drawing.Size(92, 17);
			this.labelSortByReact.TabIndex = 36;
			this.labelSortByReact.Text = "Sort";
			this.labelSortByReact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSiteReact
			// 
			this.comboSiteReact.Location = new System.Drawing.Point(420, 60);
			this.comboSiteReact.Name = "comboSiteReact";
			this.comboSiteReact.Size = new System.Drawing.Size(160, 21);
			this.comboSiteReact.TabIndex = 25;
			// 
			// labelSiteReact
			// 
			this.labelSiteReact.Location = new System.Drawing.Point(352, 62);
			this.labelSiteReact.Name = "labelSiteReact";
			this.labelSiteReact.Size = new System.Drawing.Size(67, 17);
			this.labelSiteReact.TabIndex = 24;
			this.labelSiteReact.Text = "Site";
			this.labelSiteReact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinicReact
			// 
			this.comboClinicReact.IncludeAll = true;
			this.comboClinicReact.IncludeUnassigned = true;
			this.comboClinicReact.Location = new System.Drawing.Point(383, 37);
			this.comboClinicReact.Name = "comboClinicReact";
			this.comboClinicReact.Size = new System.Drawing.Size(197, 21);
			this.comboClinicReact.TabIndex = 23;
			// 
			// labelProviderReact
			// 
			this.labelProviderReact.Location = new System.Drawing.Point(352, 16);
			this.labelProviderReact.Name = "labelProviderReact";
			this.labelProviderReact.Size = new System.Drawing.Size(67, 17);
			this.labelProviderReact.TabIndex = 20;
			this.labelProviderReact.Text = "Provider";
			this.labelProviderReact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefreshReact
			// 
			this.butRefreshReact.Location = new System.Drawing.Point(641, 70);
			this.butRefreshReact.Name = "butRefreshReact";
			this.butRefreshReact.Size = new System.Drawing.Size(80, 24);
			this.butRefreshReact.TabIndex = 2;
			this.butRefreshReact.Text = "&Refresh List";
			this.butRefreshReact.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// FormRecallList
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(985, 761);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRecallList";
			this.Text = "Recall List";
			this.Load += new System.EventHandler(this.FormRecallList_Load);
			this.Shown += new System.EventHandler(this.FormRecallList_Shown);
			this.tabControl.ResumeLayout(false);
			this.tabPageRecalls.ResumeLayout(false);
			this.groupEmailFromRecalls.ResumeLayout(false);
			this.groupSetStatusRecalls.ResumeLayout(false);
			this.groupViewRecalls.ResumeLayout(false);
			this.tabPageReminders.ResumeLayout(false);
			this.groupViewRemind.ResumeLayout(false);
			this.tabPageReactivations.ResumeLayout(false);
			this.groupEmailFromReact.ResumeLayout(false);
			this.groupSetStatusReact.ResumeLayout(false);
			this.groupViewReact.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region UI Variables
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.ContextMenu menuRightClick;
		private System.Windows.Forms.MenuItem menuItemSeeFamily;
		private System.Windows.Forms.MenuItem menuItemSeeAccount;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageRecalls;
		private System.Windows.Forms.GroupBox groupEmailFromRecalls;
		private System.Windows.Forms.ComboBox comboEmailFromRecalls;
		private System.Windows.Forms.Panel panelWebSched;
		private OpenDental.UI.Button butUndo;
		private OpenDental.UI.Button butGotoFamily;
		private OpenDental.UI.Button butCommlog;
		private OpenDental.UI.Button butGotoAccount;
		private OpenDental.UI.Button butSchedFam;
		private OpenDental.UI.Button butLabelOne;
		private OpenDental.UI.Button butSchedPat;
		private System.Windows.Forms.Label labelPatientCount;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butECards;
		private OpenDental.UI.Button butEmail;
		private OpenDental.UI.Button butPostcards;
		private OpenDental.UI.GridOD gridRecalls;
		private System.Windows.Forms.GroupBox groupSetStatusRecalls;
		private UI.ComboBoxOD comboSetStatusRecalls;
		private OpenDental.UI.Button butSetStatusRecalls;
		private OpenDental.UI.Button butLabels;
		private OpenDental.UI.Button butReport;
		private System.Windows.Forms.GroupBox groupViewRecalls;
		private System.Windows.Forms.CheckBox checkShowConflictingTypes;
		private System.Windows.Forms.Label labelShowReminders;
		private System.Windows.Forms.Label labelSortRecalls;
		private UI.ComboBoxOD comboSiteRecalls;
		private System.Windows.Forms.Label labelSiteRecalls;
		private OpenDental.UI.ComboBoxClinicPicker comboClinicRecalls;
		private System.Windows.Forms.Label labelProviderRecalls;
		private System.Windows.Forms.CheckBox checkGroupFamiliesRecalls;
		private OpenDental.UI.Button butRefreshRecalls;
		private System.Windows.Forms.TabPage tabPageReminders;
		private OpenDental.UI.GridOD gridReminders;
		private System.Windows.Forms.GroupBox groupViewRemind;
		private OpenDental.UI.ComboBoxClinicPicker comboClinicRemind;
		private OpenDental.UI.Button butRefreshRemind;
		private OpenDental.UI.ODDateRangePicker datePickerRemind;
		private System.Windows.Forms.TabPage tabPageReactivations;
		private System.Windows.Forms.Label labelReactPatCount;
		private System.Windows.Forms.GroupBox groupSetStatusReact;
		private OpenDental.UI.Button butSetStatusReact;
		private System.Windows.Forms.GroupBox groupViewReact;
		private UI.ComboBoxOD comboShowReactivate;
		private System.Windows.Forms.Label labelShowReactivate;
		private UI.ComboBoxOD comboSortReact;
		private System.Windows.Forms.Label labelSortByReact;
		private UI.ComboBoxOD comboSiteReact;
		private System.Windows.Forms.Label labelSiteReact;
		private OpenDental.UI.ComboBoxClinicPicker comboClinicReact;
		private System.Windows.Forms.Label labelProviderReact;
		private OpenDental.UI.Button butRefreshReact;
		private UI.ComboBoxOD comboBillingTypes;
		private System.Windows.Forms.Label labelBillingTypes;
		private System.Windows.Forms.CheckBox checkShowDoNotContact;
		private UI.ComboBoxOD comboSetStatusReact;
		private System.Windows.Forms.CheckBox checkGroupFamiliesReact;
		private OpenDental.UI.Button butReactGoToFam;
		private OpenDental.UI.Button butReactComm;
		private OpenDental.UI.Button butReactGoToAcct;
		private OpenDental.UI.Button butReactSchedFam;
		private OpenDental.UI.Button butReactSingleLabels;
		private OpenDental.UI.Button butReactSchedPat;
		private OpenDental.UI.Button butReactEmail;
		private OpenDental.UI.Button butReactPostcardPreview;
		private OpenDental.UI.Button butReactPrint;
		private OpenDental.UI.Button butReactLabelPreview;
		#endregion UI Variables

		private System.Windows.Forms.GroupBox groupEmailFromReact;
		private System.Windows.Forms.ComboBox comboEmailFromReact;
		private UI.GridOD gridReactivations;
		private System.Windows.Forms.CheckBox checkIncludeReminded;
		private System.Windows.Forms.CheckBox checkExcludeInactive;
		private UI.ODDateRangePicker datePickerRecalls;
		private System.Windows.Forms.Label labelRecallTypes;
		private UI.ODDateRangePicker datePickerReact;
		private UI.ComboBoxOD comboSortRecalls;
		private UI.ComboBoxOD comboRecallTypes;
		private UI.ComboBoxOD comboProviderRecalls;
		private UI.ComboBoxOD comboProviderReact;
		private UI.ComboBoxOD comboShowReminders;
	}
}
