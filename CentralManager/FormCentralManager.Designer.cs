namespace CentralManager {
	partial class FormCentralManager {
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
			this.textConnSearch = new System.Windows.Forms.TextBox();
			this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.menuItemLogoff = new System.Windows.Forms.MenuItem();
			this.menuItemFile = new System.Windows.Forms.MenuItem();
			this.menuItemPassword = new System.Windows.Forms.MenuItem();
			this.menuItemUserSettings = new System.Windows.Forms.MenuItem();
			this.menuItemSetup = new System.Windows.Forms.MenuItem();
			this.menuItemConnections = new System.Windows.Forms.MenuItem();
			this.menuItemDisplayFields = new System.Windows.Forms.MenuItem();
			this.menuItemGroups = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemSecurity = new System.Windows.Forms.MenuItem();
			this.menuItemReports = new System.Windows.Forms.MenuItem();
			this.menuItemAnnualPI = new System.Windows.Forms.MenuItem();
			this.menuTransfer = new System.Windows.Forms.MenuItem();
			this.menuTransferPatient = new System.Windows.Forms.MenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.comboConnectionGroups = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textProviderSearch = new System.Windows.Forms.TextBox();
			this.textClinicSearch = new System.Windows.Forms.TextBox();
			this.gridMainRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.labelVersion = new System.Windows.Forms.Label();
			this.gridConns = new OpenDental.UI.GridOD();
			this.butFilter = new OpenDental.UI.Button();
			this.butRefreshStatuses = new OpenDental.UI.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.labelFetch = new System.Windows.Forms.Label();
			this.checkLimit = new System.Windows.Forms.CheckBox();
			this.butSearchPats = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textClinicPatSearch = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.checkGuarantors = new System.Windows.Forms.CheckBox();
			this.textConnPatSearch = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textCountry = new System.Windows.Forms.TextBox();
			this.labelCountry = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.labelEmail = new System.Windows.Forms.Label();
			this.textSubscriberID = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textBirthdate = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkHideArchived = new System.Windows.Forms.CheckBox();
			this.textChartNumber = new System.Windows.Forms.TextBox();
			this.textSSN = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textState = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.checkHideInactive = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textPhone = new OpenDental.ValidPhone();
			this.label15 = new System.Windows.Forms.Label();
			this.textFName = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textLName = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.gridPats = new OpenDental.UI.GridOD();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.gridMainRightClickMenu.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textConnSearch
			// 
			this.textConnSearch.Location = new System.Drawing.Point(71, 15);
			this.textConnSearch.Name = "textConnSearch";
			this.textConnSearch.Size = new System.Drawing.Size(157, 20);
			this.textConnSearch.TabIndex = 211;
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemLogoff,
            this.menuItemFile,
            this.menuItemSetup,
            this.menuItemReports,
            this.menuTransfer});
			// 
			// menuItemLogoff
			// 
			this.menuItemLogoff.Index = 0;
			this.menuItemLogoff.Text = "Logoff";
			this.menuItemLogoff.Click += new System.EventHandler(this.menuItemLogoff_Click);
			// 
			// menuItemFile
			// 
			this.menuItemFile.Index = 1;
			this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemPassword,
            this.menuItemUserSettings});
			this.menuItemFile.Text = "File";
			// 
			// menuItemPassword
			// 
			this.menuItemPassword.Index = 0;
			this.menuItemPassword.Text = "Change Password";
			this.menuItemPassword.Click += new System.EventHandler(this.menuItemPassword_Click);
			// 
			// menuItemUserSettings
			// 
			this.menuItemUserSettings.Index = 1;
			this.menuItemUserSettings.Text = "User Settings";
			this.menuItemUserSettings.Visible = false;
			this.menuItemUserSettings.Click += new System.EventHandler(this.menuItemUserSettings_Click);
			// 
			// menuItemSetup
			// 
			this.menuItemSetup.Index = 2;
			this.menuItemSetup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemConnections,
            this.menuItemDisplayFields,
            this.menuItemGroups,
            this.menuItem1,
            this.menuItemSecurity});
			this.menuItemSetup.Text = "Setup";
			// 
			// menuItemConnections
			// 
			this.menuItemConnections.Index = 0;
			this.menuItemConnections.Text = "Connections";
			this.menuItemConnections.Click += new System.EventHandler(this.menuConnSetup_Click);
			// 
			// menuItemDisplayFields
			// 
			this.menuItemDisplayFields.Index = 1;
			this.menuItemDisplayFields.Text = "Display Fields";
			this.menuItemDisplayFields.Click += new System.EventHandler(this.menuItemDisplayFields_Click);
			// 
			// menuItemGroups
			// 
			this.menuItemGroups.Index = 2;
			this.menuItemGroups.Text = "Groups";
			this.menuItemGroups.Click += new System.EventHandler(this.menuGroups_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 3;
			this.menuItem1.Text = "Report Permissions";
			this.menuItem1.Click += new System.EventHandler(this.menuItemReportSetup_Click);
			// 
			// menuItemSecurity
			// 
			this.menuItemSecurity.Index = 4;
			this.menuItemSecurity.Text = "Security";
			this.menuItemSecurity.Click += new System.EventHandler(this.menuItemSecurity_Click);
			// 
			// menuItemReports
			// 
			this.menuItemReports.Index = 3;
			this.menuItemReports.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAnnualPI});
			this.menuItemReports.Text = "Reports";
			// 
			// menuItemAnnualPI
			// 
			this.menuItemAnnualPI.Index = 0;
			this.menuItemAnnualPI.Text = "Production and Income";
			this.menuItemAnnualPI.Click += new System.EventHandler(this.menuProdInc_Click);
			// 
			// menuTransfer
			// 
			this.menuTransfer.Index = 4;
			this.menuTransfer.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuTransferPatient});
			this.menuTransfer.Text = "Transfer";
			// 
			// menuTransferPatient
			// 
			this.menuTransferPatient.Index = 0;
			this.menuTransferPatient.Text = "Patient";
			this.menuTransferPatient.Click += new System.EventHandler(this.menuTransferPatient_Click);
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(5, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 15);
			this.label1.TabIndex = 213;
			this.label1.Text = "Conn Groups";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// comboConnectionGroups
			// 
			this.comboConnectionGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboConnectionGroups.FormattingEnabled = true;
			this.comboConnectionGroups.Location = new System.Drawing.Point(91, 3);
			this.comboConnectionGroups.MaxDropDownItems = 20;
			this.comboConnectionGroups.Name = "comboConnectionGroups";
			this.comboConnectionGroups.Size = new System.Drawing.Size(169, 21);
			this.comboConnectionGroups.TabIndex = 214;
			this.comboConnectionGroups.SelectionChangeCommitted += new System.EventHandler(this.comboConnectionGroups_SelectionChangeCommitted);
			// 
			// label6
			// 
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label6.Location = new System.Drawing.Point(6, 63);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 15);
			this.label6.TabIndex = 226;
			this.label6.Text = "Provider";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label4
			// 
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label4.Location = new System.Drawing.Point(6, 41);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 15);
			this.label4.TabIndex = 224;
			this.label4.Text = "Clinic";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label5
			// 
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label5.Location = new System.Drawing.Point(6, 19);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 15);
			this.label5.TabIndex = 225;
			this.label5.Text = "Conn Name";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textProviderSearch
			// 
			this.textProviderSearch.Location = new System.Drawing.Point(71, 59);
			this.textProviderSearch.Name = "textProviderSearch";
			this.textProviderSearch.Size = new System.Drawing.Size(157, 20);
			this.textProviderSearch.TabIndex = 213;
			// 
			// textClinicSearch
			// 
			this.textClinicSearch.Location = new System.Drawing.Point(71, 37);
			this.textClinicSearch.Name = "textClinicSearch";
			this.textClinicSearch.Size = new System.Drawing.Size(157, 20);
			this.textClinicSearch.TabIndex = 212;
			// 
			// gridMainRightClickMenu
			// 
			this.gridMainRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
			this.gridMainRightClickMenu.Name = "gridMainRightClickMenu";
			this.gridMainRightClickMenu.Size = new System.Drawing.Size(114, 26);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(113, 22);
			this.toolStripMenuItem1.Text = "Refresh";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.butRefreshStatuses_Click);
			// 
			// labelVersion
			// 
			this.labelVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelVersion.Location = new System.Drawing.Point(3, 71);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(110, 12);
			this.labelVersion.TabIndex = 227;
			this.labelVersion.Text = "Version";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridConns
			// 
			this.gridConns.AllowSortingByColumn = true;
			this.gridConns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridConns.ContextMenuStrip = this.gridMainRightClickMenu;
			this.gridConns.Location = new System.Drawing.Point(6, 88);
			this.gridConns.Name = "gridConns";
			this.gridConns.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridConns.Size = new System.Drawing.Size(703, 837);
			this.gridConns.TabIndex = 5;
			this.gridConns.Title = "Connections - Double-click to Launch";
			this.gridConns.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridConns_CellDoubleClick);
			// 
			// butFilter
			// 
			this.butFilter.Location = new System.Drawing.Point(234, 55);
			this.butFilter.Name = "butFilter";
			this.butFilter.Size = new System.Drawing.Size(56, 24);
			this.butFilter.TabIndex = 216;
			this.butFilter.Text = "Filter";
			this.butFilter.UseVisualStyleBackColor = true;
			this.butFilter.Click += new System.EventHandler(this.butFilter_Click);
			// 
			// butRefreshStatuses
			// 
			this.butRefreshStatuses.Location = new System.Drawing.Point(155, 35);
			this.butRefreshStatuses.Name = "butRefreshStatuses";
			this.butRefreshStatuses.Size = new System.Drawing.Size(105, 24);
			this.butRefreshStatuses.TabIndex = 227;
			this.butRefreshStatuses.Text = "Refresh Statuses";
			this.butRefreshStatuses.UseVisualStyleBackColor = true;
			this.butRefreshStatuses.Click += new System.EventHandler(this.butRefreshStatuses_Click);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 500;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// labelFetch
			// 
			this.labelFetch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFetch.BackColor = System.Drawing.Color.Transparent;
			this.labelFetch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFetch.ForeColor = System.Drawing.Color.Red;
			this.labelFetch.Location = new System.Drawing.Point(1589, 530);
			this.labelFetch.Name = "labelFetch";
			this.labelFetch.Size = new System.Drawing.Size(148, 17);
			this.labelFetch.TabIndex = 235;
			this.labelFetch.Text = "Fetching Results...";
			this.labelFetch.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.labelFetch.Visible = false;
			// 
			// checkLimit
			// 
			this.checkLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkLimit.Checked = true;
			this.checkLimit.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkLimit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLimit.Location = new System.Drawing.Point(1605, 502);
			this.checkLimit.Name = "checkLimit";
			this.checkLimit.Size = new System.Drawing.Size(217, 16);
			this.checkLimit.TabIndex = 234;
			this.checkLimit.Text = "Limit 30 patients per connection";
			// 
			// butSearchPats
			// 
			this.butSearchPats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSearchPats.Location = new System.Drawing.Point(1738, 529);
			this.butSearchPats.Name = "butSearchPats";
			this.butSearchPats.Size = new System.Drawing.Size(84, 23);
			this.butSearchPats.TabIndex = 233;
			this.butSearchPats.Text = "Search";
			this.butSearchPats.Click += new System.EventHandler(this.butSearchPats_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.textClinicPatSearch);
			this.groupBox2.Controls.Add(this.label18);
			this.groupBox2.Controls.Add(this.checkGuarantors);
			this.groupBox2.Controls.Add(this.textConnPatSearch);
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.textCountry);
			this.groupBox2.Controls.Add(this.labelCountry);
			this.groupBox2.Controls.Add(this.textEmail);
			this.groupBox2.Controls.Add(this.labelEmail);
			this.groupBox2.Controls.Add(this.textSubscriberID);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.textBirthdate);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.checkHideArchived);
			this.groupBox2.Controls.Add(this.textChartNumber);
			this.groupBox2.Controls.Add(this.textSSN);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.textPatNum);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.textState);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.textCity);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.checkHideInactive);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.textAddress);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.textPhone);
			this.groupBox2.Controls.Add(this.label15);
			this.groupBox2.Controls.Add(this.textFName);
			this.groupBox2.Controls.Add(this.label16);
			this.groupBox2.Controls.Add(this.textLName);
			this.groupBox2.Controls.Add(this.label17);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(1593, 100);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(229, 388);
			this.groupBox2.TabIndex = 232;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Search by:";
			// 
			// textClinicPatSearch
			// 
			this.textClinicPatSearch.Location = new System.Drawing.Point(123, 312);
			this.textClinicPatSearch.Name = "textClinicPatSearch";
			this.textClinicPatSearch.Size = new System.Drawing.Size(90, 20);
			this.textClinicPatSearch.TabIndex = 50;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(11, 313);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(113, 17);
			this.label18.TabIndex = 51;
			this.label18.Text = "Clinic";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkGuarantors
			// 
			this.checkGuarantors.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGuarantors.Location = new System.Drawing.Point(11, 339);
			this.checkGuarantors.Name = "checkGuarantors";
			this.checkGuarantors.Size = new System.Drawing.Size(202, 16);
			this.checkGuarantors.TabIndex = 49;
			this.checkGuarantors.Text = "Show Guarantors Only";
			// 
			// textConnPatSearch
			// 
			this.textConnPatSearch.Location = new System.Drawing.Point(123, 292);
			this.textConnPatSearch.Name = "textConnPatSearch";
			this.textConnPatSearch.Size = new System.Drawing.Size(90, 20);
			this.textConnPatSearch.TabIndex = 47;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(11, 293);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(113, 17);
			this.label14.TabIndex = 48;
			this.label14.Text = "Connection";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCountry
			// 
			this.textCountry.Location = new System.Drawing.Point(123, 272);
			this.textCountry.Name = "textCountry";
			this.textCountry.Size = new System.Drawing.Size(90, 20);
			this.textCountry.TabIndex = 12;
			// 
			// labelCountry
			// 
			this.labelCountry.Location = new System.Drawing.Point(11, 273);
			this.labelCountry.Name = "labelCountry";
			this.labelCountry.Size = new System.Drawing.Size(113, 17);
			this.labelCountry.TabIndex = 46;
			this.labelCountry.Text = "Country";
			this.labelCountry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(123, 252);
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(90, 20);
			this.textEmail.TabIndex = 11;
			// 
			// labelEmail
			// 
			this.labelEmail.Location = new System.Drawing.Point(11, 256);
			this.labelEmail.Name = "labelEmail";
			this.labelEmail.Size = new System.Drawing.Size(113, 12);
			this.labelEmail.TabIndex = 43;
			this.labelEmail.Text = "E-mail";
			this.labelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubscriberID
			// 
			this.textSubscriberID.Location = new System.Drawing.Point(123, 232);
			this.textSubscriberID.Name = "textSubscriberID";
			this.textSubscriberID.Size = new System.Drawing.Size(90, 20);
			this.textSubscriberID.TabIndex = 10;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(11, 236);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(113, 12);
			this.label13.TabIndex = 41;
			this.label13.Text = "Subscriber ID";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBirthdate
			// 
			this.textBirthdate.Location = new System.Drawing.Point(123, 212);
			this.textBirthdate.Name = "textBirthdate";
			this.textBirthdate.Size = new System.Drawing.Size(90, 20);
			this.textBirthdate.TabIndex = 9;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 216);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(113, 12);
			this.label2.TabIndex = 27;
			this.label2.Text = "Birthdate";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkHideArchived
			// 
			this.checkHideArchived.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideArchived.Location = new System.Drawing.Point(11, 370);
			this.checkHideArchived.Name = "checkHideArchived";
			this.checkHideArchived.Size = new System.Drawing.Size(203, 16);
			this.checkHideArchived.TabIndex = 25;
			this.checkHideArchived.Text = "Hide Archived/Deceased";
			// 
			// textChartNumber
			// 
			this.textChartNumber.Location = new System.Drawing.Point(123, 192);
			this.textChartNumber.Name = "textChartNumber";
			this.textChartNumber.Size = new System.Drawing.Size(90, 20);
			this.textChartNumber.TabIndex = 8;
			// 
			// textSSN
			// 
			this.textSSN.Location = new System.Drawing.Point(123, 152);
			this.textSSN.Name = "textSSN";
			this.textSSN.Size = new System.Drawing.Size(90, 20);
			this.textSSN.TabIndex = 6;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(11, 156);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(112, 12);
			this.label12.TabIndex = 24;
			this.label12.Text = "SSN";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(11, 196);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(113, 12);
			this.label10.TabIndex = 20;
			this.label10.Text = "Chart Number";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(123, 172);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(90, 20);
			this.textPatNum.TabIndex = 7;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(11, 176);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(113, 12);
			this.label9.TabIndex = 18;
			this.label9.Text = "Patient Number";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(123, 132);
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(90, 20);
			this.textState.TabIndex = 5;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(11, 136);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(111, 12);
			this.label8.TabIndex = 16;
			this.label8.Text = "State";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(123, 112);
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(90, 20);
			this.textCity.TabIndex = 4;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(11, 114);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(109, 14);
			this.label7.TabIndex = 14;
			this.label7.Text = "City";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkHideInactive
			// 
			this.checkHideInactive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideInactive.Location = new System.Drawing.Point(11, 354);
			this.checkHideInactive.Name = "checkHideInactive";
			this.checkHideInactive.Size = new System.Drawing.Size(202, 16);
			this.checkHideInactive.TabIndex = 44;
			this.checkHideInactive.Text = "Hide Inactive Patients";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(11, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(202, 14);
			this.label3.TabIndex = 10;
			this.label3.Text = "Hint: enter values in multiple boxes.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(123, 92);
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(90, 20);
			this.textAddress.TabIndex = 3;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(11, 95);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(111, 12);
			this.label11.TabIndex = 9;
			this.label11.Text = "Address";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(123, 72);
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(90, 20);
			this.textPhone.TabIndex = 2;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(11, 74);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(112, 16);
			this.label15.TabIndex = 7;
			this.label15.Text = "Phone (any)";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(123, 52);
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(90, 20);
			this.textFName.TabIndex = 1;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(11, 56);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(111, 12);
			this.label16.TabIndex = 5;
			this.label16.Text = "First Name";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(123, 32);
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(90, 20);
			this.textLName.TabIndex = 0;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(11, 35);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(111, 12);
			this.label17.TabIndex = 3;
			this.label17.Text = "Last Name";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridPats
			// 
			this.gridPats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPats.HScrollVisible = true;
			this.gridPats.Location = new System.Drawing.Point(715, 88);
			this.gridPats.Name = "gridPats";
			this.gridPats.Size = new System.Drawing.Size(868, 837);
			this.gridPats.TabIndex = 231;
			this.gridPats.Title = "Patients - Double-click to Launch Connection";
			this.gridPats.TranslationName = "FormPatientSelect";
			this.gridPats.WrapText = false;
			this.gridPats.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPats_CellDoubleClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textConnSearch);
			this.groupBox1.Controls.Add(this.textClinicSearch);
			this.groupBox1.Controls.Add(this.textProviderSearch);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.butFilter);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Location = new System.Drawing.Point(285, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(297, 85);
			this.groupBox1.TabIndex = 236;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filter Connections";
			// 
			// FormCentralManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1834, 929);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelFetch);
			this.Controls.Add(this.checkLimit);
			this.Controls.Add(this.butSearchPats);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.gridPats);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.butRefreshStatuses);
			this.Controls.Add(this.comboConnectionGroups);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridConns);
			this.Menu = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(1006, 572);
			this.Name = "FormCentralManager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Central Enterprise Management Tool (CEMT)";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCentralManager_FormClosing);
			this.Load += new System.EventHandler(this.FormCentralManager_Load);
			this.gridMainRightClickMenu.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridConns;
		private System.Windows.Forms.TextBox textConnSearch;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItemSetup;
		private System.Windows.Forms.MenuItem menuItemReports;
		private System.Windows.Forms.MenuItem menuItemConnections;
		private System.Windows.Forms.MenuItem menuItemSecurity;
		private System.Windows.Forms.MenuItem menuItemAnnualPI;
		private System.Windows.Forms.MenuItem menuItemGroups;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboConnectionGroups;
		private System.Windows.Forms.MenuItem menuItemLogoff;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemPassword;
		private System.Windows.Forms.MenuItem menuItemUserSettings;
		private OpenDental.UI.Button butFilter;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textProviderSearch;
		private System.Windows.Forms.TextBox textClinicSearch;
		private OpenDental.UI.Button butRefreshStatuses;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemDisplayFields;
		private System.Windows.Forms.ContextMenuStrip gridMainRightClickMenu;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.MenuItem menuTransfer;
		private System.Windows.Forms.MenuItem menuTransferPatient;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label labelFetch;
		private System.Windows.Forms.CheckBox checkLimit;
		private OpenDental.UI.Button butSearchPats;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkGuarantors;
		private System.Windows.Forms.TextBox textConnPatSearch;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textCountry;
		private System.Windows.Forms.Label labelCountry;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.Label labelEmail;
		private System.Windows.Forms.TextBox textSubscriberID;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textBirthdate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkHideArchived;
		private System.Windows.Forms.TextBox textChartNumber;
		private System.Windows.Forms.TextBox textSSN;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textState;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkHideInactive;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textAddress;
		private System.Windows.Forms.Label label11;
		private OpenDental.ValidPhone textPhone;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.Label label17;
		private OpenDental.UI.GridOD gridPats;
		private System.Windows.Forms.TextBox textClinicPatSearch;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}

