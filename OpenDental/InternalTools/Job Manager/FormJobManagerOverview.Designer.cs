namespace OpenDental{
	partial class FormJobManagerOverview {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobManagerOverview));
			this.panelSprintManager = new System.Windows.Forms.Panel();
			this.tablePanelSprintManager = new System.Windows.Forms.TableLayoutPanel();
			this.gridSprints = new OpenDental.UI.GridOD();
			this.panel3 = new System.Windows.Forms.Panel();
			this.tabSprintManager = new System.Windows.Forms.TabControl();
			this.tabSprintManage = new System.Windows.Forms.TabPage();
			this.userControlSprintManager = new OpenDental.UserControlSprintManager();
			this.tabEngineers = new System.Windows.Forms.TabPage();
			this.gridEngineers = new OpenDental.UI.GridOD();
			this.panelJobsReport = new System.Windows.Forms.Panel();
			this.tablePanelJobsReport = new System.Windows.Forms.TableLayoutPanel();
			this.gridJobs = new OpenDental.UI.GridOD();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.butExport = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textCompletionPercent = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textTotalFeatures = new System.Windows.Forms.TextBox();
			this.textTotalBugs = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textTotalQuote = new System.Windows.Forms.TextBox();
			this.textTotalHrsSpent = new System.Windows.Forms.TextBox();
			this.textTotalHrsEst = new System.Windows.Forms.TextBox();
			this.textTotalJobs = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboProposedVersion = new OpenDental.UI.ComboBoxOD();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.dateRangeJobCompleted = new OpenDental.UI.ODDateRangePicker();
			this.checkRemoveApprovalJobs = new System.Windows.Forms.CheckBox();
			this.checkRemoveNoQuote = new System.Windows.Forms.CheckBox();
			this.butPatSelect = new OpenDental.UI.Button();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboEngineers = new OpenDental.UI.ComboBoxOD();
			this.label6 = new System.Windows.Forms.Label();
			this.listPriorities = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.listPhases = new OpenDental.UI.ListBoxOD();
			this.label5 = new System.Windows.Forms.Label();
			this.listCategories = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioCustom = new System.Windows.Forms.RadioButton();
			this.radioStaleJobs = new System.Windows.Forms.RadioButton();
			this.radioNewVersion = new System.Windows.Forms.RadioButton();
			this.radioCurrentVersion = new System.Windows.Forms.RadioButton();
			this.butRefresh = new OpenDental.UI.Button();
			this.toolMainLeft = new System.Windows.Forms.ToolStrip();
			this.butSprintManager = new System.Windows.Forms.ToolStripButton();
			this.butJobsReport = new System.Windows.Forms.ToolStripButton();
			this.toolStripReporting = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.butHomePage = new System.Windows.Forms.ToolStripButton();
			this.butForum = new System.Windows.Forms.ToolStripButton();
			this.butBugsManager = new System.Windows.Forms.ToolStripButton();
			this.butDatabaseSchema = new System.Windows.Forms.ToolStripButton();
			this.butPreviousVersions = new System.Windows.Forms.ToolStripButton();
			this.butUserGroup = new System.Windows.Forms.ToolStripButton();
			this.butSchedules = new System.Windows.Forms.ToolStripButton();
			this.panelSprintManager.SuspendLayout();
			this.tablePanelSprintManager.SuspendLayout();
			this.panel3.SuspendLayout();
			this.tabSprintManager.SuspendLayout();
			this.tabSprintManage.SuspendLayout();
			this.tabEngineers.SuspendLayout();
			this.panelJobsReport.SuspendLayout();
			this.tablePanelJobsReport.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.toolMainLeft.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelSprintManager
			// 
			this.panelSprintManager.Controls.Add(this.tablePanelSprintManager);
			this.panelSprintManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelSprintManager.Location = new System.Drawing.Point(105, 0);
			this.panelSprintManager.Name = "panelSprintManager";
			this.panelSprintManager.Size = new System.Drawing.Size(1403, 778);
			this.panelSprintManager.TabIndex = 16;
			// 
			// tablePanelSprintManager
			// 
			this.tablePanelSprintManager.ColumnCount = 2;
			this.tablePanelSprintManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 259F));
			this.tablePanelSprintManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tablePanelSprintManager.Controls.Add(this.gridSprints, 0, 0);
			this.tablePanelSprintManager.Controls.Add(this.panel3, 1, 0);
			this.tablePanelSprintManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tablePanelSprintManager.Location = new System.Drawing.Point(0, 0);
			this.tablePanelSprintManager.Name = "tablePanelSprintManager";
			this.tablePanelSprintManager.RowCount = 1;
			this.tablePanelSprintManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tablePanelSprintManager.Size = new System.Drawing.Size(1403, 778);
			this.tablePanelSprintManager.TabIndex = 2;
			// 
			// gridSprints
			// 
			this.gridSprints.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridSprints.HasAddButton = true;
			this.gridSprints.Location = new System.Drawing.Point(3, 3);
			this.gridSprints.Name = "gridSprints";
			this.gridSprints.ShowContextMenu = false;
			this.gridSprints.Size = new System.Drawing.Size(253, 772);
			this.gridSprints.TabIndex = 0;
			this.gridSprints.Title = "Sprints";
			this.gridSprints.TranslationName = "Jobs";
			this.gridSprints.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSprints_CellClick);
			this.gridSprints.TitleAddClick += new System.EventHandler(this.gridSprints_TitleAddClick);
			this.gridSprints.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridSprints_MouseClick);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.tabSprintManager);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(262, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(1138, 772);
			this.panel3.TabIndex = 1;
			// 
			// tabSprintManager
			// 
			this.tabSprintManager.Controls.Add(this.tabSprintManage);
			this.tabSprintManager.Controls.Add(this.tabEngineers);
			this.tabSprintManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabSprintManager.Location = new System.Drawing.Point(0, 0);
			this.tabSprintManager.Name = "tabSprintManager";
			this.tabSprintManager.SelectedIndex = 0;
			this.tabSprintManager.Size = new System.Drawing.Size(1138, 772);
			this.tabSprintManager.TabIndex = 1;
			// 
			// tabSprintManage
			// 
			this.tabSprintManage.Controls.Add(this.userControlSprintManager);
			this.tabSprintManage.Location = new System.Drawing.Point(4, 22);
			this.tabSprintManage.Name = "tabSprintManage";
			this.tabSprintManage.Padding = new System.Windows.Forms.Padding(3);
			this.tabSprintManage.Size = new System.Drawing.Size(1130, 746);
			this.tabSprintManage.TabIndex = 0;
			this.tabSprintManage.Text = "Manage";
			this.tabSprintManage.UseVisualStyleBackColor = true;
			// 
			// userControlSprintManager
			// 
			this.userControlSprintManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userControlSprintManager.Enabled = false;
			this.userControlSprintManager.Location = new System.Drawing.Point(3, 3);
			this.userControlSprintManager.Name = "userControlSprintManager";
			this.userControlSprintManager.Size = new System.Drawing.Size(1124, 740);
			this.userControlSprintManager.TabIndex = 0;
			this.userControlSprintManager.SaveClick += new System.EventHandler(this.userControlSprintManager_SaveClick);
			// 
			// tabEngineers
			// 
			this.tabEngineers.Controls.Add(this.gridEngineers);
			this.tabEngineers.Location = new System.Drawing.Point(4, 22);
			this.tabEngineers.Name = "tabEngineers";
			this.tabEngineers.Padding = new System.Windows.Forms.Padding(3);
			this.tabEngineers.Size = new System.Drawing.Size(1130, 746);
			this.tabEngineers.TabIndex = 1;
			this.tabEngineers.Text = "Engineers";
			this.tabEngineers.UseVisualStyleBackColor = true;
			// 
			// gridEngineers
			// 
			this.gridEngineers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridEngineers.Location = new System.Drawing.Point(3, 3);
			this.gridEngineers.Name = "gridEngineers";
			this.gridEngineers.Size = new System.Drawing.Size(1124, 740);
			this.gridEngineers.TabIndex = 0;
			this.gridEngineers.Title = "Engineers";
			this.gridEngineers.TranslationName = "JobManager";
			// 
			// panelJobsReport
			// 
			this.panelJobsReport.Controls.Add(this.tablePanelJobsReport);
			this.panelJobsReport.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelJobsReport.Location = new System.Drawing.Point(105, 0);
			this.panelJobsReport.Name = "panelJobsReport";
			this.panelJobsReport.Size = new System.Drawing.Size(1403, 778);
			this.panelJobsReport.TabIndex = 15;
			// 
			// tablePanelJobsReport
			// 
			this.tablePanelJobsReport.ColumnCount = 1;
			this.tablePanelJobsReport.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tablePanelJobsReport.Controls.Add(this.gridJobs, 0, 1);
			this.tablePanelJobsReport.Controls.Add(this.panel1, 0, 2);
			this.tablePanelJobsReport.Controls.Add(this.panel2, 0, 0);
			this.tablePanelJobsReport.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tablePanelJobsReport.Location = new System.Drawing.Point(0, 0);
			this.tablePanelJobsReport.Name = "tablePanelJobsReport";
			this.tablePanelJobsReport.RowCount = 3;
			this.tablePanelJobsReport.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tablePanelJobsReport.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tablePanelJobsReport.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tablePanelJobsReport.Size = new System.Drawing.Size(1403, 778);
			this.tablePanelJobsReport.TabIndex = 14;
			// 
			// gridJobs
			// 
			this.gridJobs.AllowSortingByColumn = true;
			this.gridJobs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridJobs.Location = new System.Drawing.Point(3, 123);
			this.gridJobs.Name = "gridJobs";
			this.gridJobs.Size = new System.Drawing.Size(1397, 530);
			this.gridJobs.TabIndex = 12;
			this.gridJobs.Title = "Jobs";
			this.gridJobs.TranslationName = "Jobs";
			this.gridJobs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridJobs_CellDoubleClick);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBox6);
			this.panel1.Controls.Add(this.groupBox4);
			this.panel1.Controls.Add(this.groupBox3);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 659);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1397, 116);
			this.panel1.TabIndex = 13;
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.butExport);
			this.groupBox6.Controls.Add(this.butPrint);
			this.groupBox6.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBox6.Location = new System.Drawing.Point(1259, 0);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(138, 116);
			this.groupBox6.TabIndex = 0;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Print Options";
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExport.Image = global::OpenDental.Properties.Resources.butExport;
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(31, 37);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(79, 24);
			this.butExport.TabIndex = 59;
			this.butExport.Text = "&Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Enabled = false;
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(31, 67);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(79, 24);
			this.butPrint.TabIndex = 58;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox4.Location = new System.Drawing.Point(470, 0);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(927, 116);
			this.groupBox4.TabIndex = 61;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Job  Information ";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(254, 48);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(29, 13);
			this.label11.TabIndex = 0;
			this.label11.Text = "TBD";
			// 
			// groupBox3
			// 
			this.groupBox3.AutoSize = true;
			this.groupBox3.Controls.Add(this.label16);
			this.groupBox3.Controls.Add(this.textCompletionPercent);
			this.groupBox3.Controls.Add(this.label15);
			this.groupBox3.Controls.Add(this.textTotalFeatures);
			this.groupBox3.Controls.Add(this.textTotalBugs);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Controls.Add(this.textTotalQuote);
			this.groupBox3.Controls.Add(this.textTotalHrsSpent);
			this.groupBox3.Controls.Add(this.textTotalHrsEst);
			this.groupBox3.Controls.Add(this.textTotalJobs);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBox3.Location = new System.Drawing.Point(0, 0);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(470, 116);
			this.groupBox3.TabIndex = 60;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Report Totals";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(306, 17);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(119, 13);
			this.label16.TabIndex = 16;
			this.label16.Text = "Estimated Completion %";
			// 
			// textCompletionPercent
			// 
			this.textCompletionPercent.Location = new System.Drawing.Point(428, 13);
			this.textCompletionPercent.Name = "textCompletionPercent";
			this.textCompletionPercent.ReadOnly = true;
			this.textCompletionPercent.Size = new System.Drawing.Size(36, 20);
			this.textCompletionPercent.TabIndex = 15;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(158, 43);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(75, 13);
			this.label15.TabIndex = 14;
			this.label15.Text = "Total Features";
			// 
			// textTotalFeatures
			// 
			this.textTotalFeatures.Location = new System.Drawing.Point(236, 39);
			this.textTotalFeatures.Name = "textTotalFeatures";
			this.textTotalFeatures.ReadOnly = true;
			this.textTotalFeatures.Size = new System.Drawing.Size(59, 20);
			this.textTotalFeatures.TabIndex = 13;
			// 
			// textTotalBugs
			// 
			this.textTotalBugs.Location = new System.Drawing.Point(428, 39);
			this.textTotalBugs.Name = "textTotalBugs";
			this.textTotalBugs.ReadOnly = true;
			this.textTotalBugs.Size = new System.Drawing.Size(36, 20);
			this.textTotalBugs.TabIndex = 12;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(296, 42);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(129, 13);
			this.label14.TabIndex = 11;
			this.label14.Text = "# of Bugs/Enhancements";
			// 
			// textTotalQuote
			// 
			this.textTotalQuote.Location = new System.Drawing.Point(236, 13);
			this.textTotalQuote.Name = "textTotalQuote";
			this.textTotalQuote.ReadOnly = true;
			this.textTotalQuote.Size = new System.Drawing.Size(59, 20);
			this.textTotalQuote.TabIndex = 10;
			// 
			// textTotalHrsSpent
			// 
			this.textTotalHrsSpent.Location = new System.Drawing.Point(109, 63);
			this.textTotalHrsSpent.Name = "textTotalHrsSpent";
			this.textTotalHrsSpent.ReadOnly = true;
			this.textTotalHrsSpent.Size = new System.Drawing.Size(40, 20);
			this.textTotalHrsSpent.TabIndex = 9;
			// 
			// textTotalHrsEst
			// 
			this.textTotalHrsEst.Location = new System.Drawing.Point(109, 38);
			this.textTotalHrsEst.Name = "textTotalHrsEst";
			this.textTotalHrsEst.ReadOnly = true;
			this.textTotalHrsEst.Size = new System.Drawing.Size(40, 20);
			this.textTotalHrsEst.TabIndex = 8;
			// 
			// textTotalJobs
			// 
			this.textTotalJobs.Location = new System.Drawing.Point(109, 13);
			this.textTotalJobs.Name = "textTotalJobs";
			this.textTotalJobs.ReadOnly = true;
			this.textTotalJobs.Size = new System.Drawing.Size(40, 20);
			this.textTotalJobs.TabIndex = 7;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(155, 17);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(78, 13);
			this.label10.TabIndex = 3;
			this.label10.Text = "Total Quoted $";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(13, 67);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(95, 13);
			this.label9.TabIndex = 2;
			this.label9.Text = "Total Hours Actual";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(23, 42);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(83, 13);
			this.label8.TabIndex = 1;
			this.label8.Text = "Total Hours Est.";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(28, 17);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(78, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "Total # of Jobs";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.groupBox2);
			this.panel2.Controls.Add(this.groupBox1);
			this.panel2.Controls.Add(this.butRefresh);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1397, 114);
			this.panel2.TabIndex = 14;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboProposedVersion);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.groupBox5);
			this.groupBox2.Controls.Add(this.checkRemoveApprovalJobs);
			this.groupBox2.Controls.Add(this.checkRemoveNoQuote);
			this.groupBox2.Controls.Add(this.butPatSelect);
			this.groupBox2.Controls.Add(this.textPatNum);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.comboEngineers);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.listPriorities);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.listPhases);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.listCategories);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBox2.Location = new System.Drawing.Point(130, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(1101, 114);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Filters";
			// 
			// comboProposedVersion
			// 
			this.comboProposedVersion.Location = new System.Drawing.Point(465, 31);
			this.comboProposedVersion.Name = "comboProposedVersion";
			this.comboProposedVersion.Size = new System.Drawing.Size(111, 21);
			this.comboProposedVersion.TabIndex = 249;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(466, 13);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(63, 13);
			this.label12.TabIndex = 248;
			this.label12.Text = "Est. Version";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.dateRangeJobCompleted);
			this.groupBox5.Location = new System.Drawing.Point(247, 39);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(212, 72);
			this.groupBox5.TabIndex = 247;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "If Completed, Completed Date Between";
			this.groupBox5.Visible = false;
			// 
			// dateRangeJobCompleted
			// 
			this.dateRangeJobCompleted.BackColor = System.Drawing.Color.Transparent;
			this.dateRangeJobCompleted.IsVertical = true;
			this.dateRangeJobCompleted.Location = new System.Drawing.Point(6, 19);
			this.dateRangeJobCompleted.MinimumSize = new System.Drawing.Size(165, 46);
			this.dateRangeJobCompleted.Name = "dateRangeJobCompleted";
			this.dateRangeJobCompleted.Size = new System.Drawing.Size(200, 46);
			this.dateRangeJobCompleted.TabIndex = 246;
			// 
			// checkRemoveApprovalJobs
			// 
			this.checkRemoveApprovalJobs.AutoSize = true;
			this.checkRemoveApprovalJobs.Location = new System.Drawing.Point(12, 84);
			this.checkRemoveApprovalJobs.Name = "checkRemoveApprovalJobs";
			this.checkRemoveApprovalJobs.Size = new System.Drawing.Size(136, 17);
			this.checkRemoveApprovalJobs.TabIndex = 245;
			this.checkRemoveApprovalJobs.Text = "Remove Approval Jobs";
			this.checkRemoveApprovalJobs.UseVisualStyleBackColor = true;
			// 
			// checkRemoveNoQuote
			// 
			this.checkRemoveNoQuote.AutoSize = true;
			this.checkRemoveNoQuote.Location = new System.Drawing.Point(12, 61);
			this.checkRemoveNoQuote.Name = "checkRemoveNoQuote";
			this.checkRemoveNoQuote.Size = new System.Drawing.Size(121, 17);
			this.checkRemoveNoQuote.TabIndex = 244;
			this.checkRemoveNoQuote.Text = "Remove Un-Quoted";
			this.checkRemoveNoQuote.UseVisualStyleBackColor = true;
			// 
			// butPatSelect
			// 
			this.butPatSelect.Location = new System.Drawing.Point(146, 35);
			this.butPatSelect.Name = "butPatSelect";
			this.butPatSelect.Size = new System.Drawing.Size(19, 20);
			this.butPatSelect.TabIndex = 243;
			this.butPatSelect.Text = "...";
			this.butPatSelect.Click += new System.EventHandler(this.butPatSelect_Click);
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(66, 35);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(80, 20);
			this.textPatNum.TabIndex = 242;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(51, 13);
			this.label2.TabIndex = 241;
			this.label2.Text = "Customer";
			// 
			// comboEngineers
			// 
			this.comboEngineers.Location = new System.Drawing.Point(66, 13);
			this.comboEngineers.Name = "comboEngineers";
			this.comboEngineers.Size = new System.Drawing.Size(184, 21);
			this.comboEngineers.TabIndex = 237;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(931, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(46, 13);
			this.label6.TabIndex = 24;
			this.label6.Text = "Priorities";
			// 
			// listPriorities
			// 
			this.listPriorities.Location = new System.Drawing.Point(934, 24);
			this.listPriorities.Name = "listPriorities";
			this.listPriorities.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listPriorities.Size = new System.Drawing.Size(162, 82);
			this.listPriorities.TabIndex = 23;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 13);
			this.label3.TabIndex = 20;
			this.label3.Text = "Engineers";
			// 
			// listPhases
			// 
			this.listPhases.Location = new System.Drawing.Point(582, 24);
			this.listPhases.Name = "listPhases";
			this.listPhases.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listPhases.Size = new System.Drawing.Size(170, 82);
			this.listPhases.TabIndex = 17;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(755, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(57, 13);
			this.label5.TabIndex = 22;
			this.label5.Text = "Categories";
			// 
			// listCategories
			// 
			this.listCategories.Location = new System.Drawing.Point(758, 24);
			this.listCategories.Name = "listCategories";
			this.listCategories.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listCategories.Size = new System.Drawing.Size(170, 82);
			this.listCategories.TabIndex = 18;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(579, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 13);
			this.label4.TabIndex = 21;
			this.label4.Text = "Phases";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioCustom);
			this.groupBox1.Controls.Add(this.radioStaleJobs);
			this.groupBox1.Controls.Add(this.radioNewVersion);
			this.groupBox1.Controls.Add(this.radioCurrentVersion);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(130, 114);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Preset Filters";
			// 
			// radioCustom
			// 
			this.radioCustom.AutoSize = true;
			this.radioCustom.Location = new System.Drawing.Point(7, 82);
			this.radioCustom.Name = "radioCustom";
			this.radioCustom.Size = new System.Drawing.Size(60, 17);
			this.radioCustom.TabIndex = 3;
			this.radioCustom.Text = "Custom";
			this.radioCustom.UseVisualStyleBackColor = true;
			this.radioCustom.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
			// 
			// radioStaleJobs
			// 
			this.radioStaleJobs.AutoSize = true;
			this.radioStaleJobs.Location = new System.Drawing.Point(7, 60);
			this.radioStaleJobs.Name = "radioStaleJobs";
			this.radioStaleJobs.Size = new System.Drawing.Size(74, 17);
			this.radioStaleJobs.TabIndex = 2;
			this.radioStaleJobs.Text = "Stale Jobs";
			this.radioStaleJobs.UseVisualStyleBackColor = true;
			this.radioStaleJobs.CheckedChanged += new System.EventHandler(this.radioStaleJobs_CheckedChanged);
			// 
			// radioNewVersion
			// 
			this.radioNewVersion.AutoSize = true;
			this.radioNewVersion.Location = new System.Drawing.Point(7, 38);
			this.radioNewVersion.Name = "radioNewVersion";
			this.radioNewVersion.Size = new System.Drawing.Size(85, 17);
			this.radioNewVersion.TabIndex = 1;
			this.radioNewVersion.Text = "Next Version";
			this.radioNewVersion.UseVisualStyleBackColor = true;
			this.radioNewVersion.CheckedChanged += new System.EventHandler(this.radioNewVersion_CheckedChanged);
			// 
			// radioCurrentVersion
			// 
			this.radioCurrentVersion.AutoSize = true;
			this.radioCurrentVersion.Checked = true;
			this.radioCurrentVersion.Location = new System.Drawing.Point(7, 16);
			this.radioCurrentVersion.Name = "radioCurrentVersion";
			this.radioCurrentVersion.Size = new System.Drawing.Size(125, 17);
			this.radioCurrentVersion.TabIndex = 0;
			this.radioCurrentVersion.TabStop = true;
			this.radioCurrentVersion.Text = "Current Development";
			this.radioCurrentVersion.UseVisualStyleBackColor = true;
			this.radioCurrentVersion.CheckedChanged += new System.EventHandler(this.radioCurrentVersion_CheckedChanged);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(1237, 90);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(82, 24);
			this.butRefresh.TabIndex = 240;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// toolMainLeft
			// 
			this.toolMainLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.toolMainLeft.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.butSprintManager,
            this.butJobsReport,
            this.toolStripReporting,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.butHomePage,
            this.butForum,
            this.butBugsManager,
            this.butDatabaseSchema,
            this.butPreviousVersions,
            this.butUserGroup,
            this.butSchedules});
			this.toolMainLeft.Location = new System.Drawing.Point(0, 0);
			this.toolMainLeft.Name = "toolMainLeft";
			this.toolMainLeft.Size = new System.Drawing.Size(105, 778);
			this.toolMainLeft.TabIndex = 3;
			this.toolMainLeft.Text = "toolStrip1";
			// 
			// butSprintManager
			// 
			this.butSprintManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butSprintManager.Image = ((System.Drawing.Image)(resources.GetObject("butSprintManager.Image")));
			this.butSprintManager.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butSprintManager.Name = "butSprintManager";
			this.butSprintManager.Size = new System.Drawing.Size(102, 19);
			this.butSprintManager.Text = "Sprint Manager";
			this.butSprintManager.Click += new System.EventHandler(this.butSprintManager_Click);
			// 
			// butJobsReport
			// 
			this.butJobsReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butJobsReport.Image = ((System.Drawing.Image)(resources.GetObject("butJobsReport.Image")));
			this.butJobsReport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butJobsReport.Name = "butJobsReport";
			this.butJobsReport.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.butJobsReport.Size = new System.Drawing.Size(102, 19);
			this.butJobsReport.Text = "Jobs Report";
			this.butJobsReport.Click += new System.EventHandler(this.butJobsReport_Click);
			// 
			// toolStripReporting
			// 
			this.toolStripReporting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripReporting.Enabled = false;
			this.toolStripReporting.Image = ((System.Drawing.Image)(resources.GetObject("toolStripReporting.Image")));
			this.toolStripReporting.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripReporting.Name = "toolStripReporting";
			this.toolStripReporting.Size = new System.Drawing.Size(102, 19);
			this.toolStripReporting.Text = "Reporting";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(102, 6);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(102, 21);
			this.toolStripLabel1.Text = "Shortcuts";
			// 
			// butHomePage
			// 
			this.butHomePage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butHomePage.Image = ((System.Drawing.Image)(resources.GetObject("butHomePage.Image")));
			this.butHomePage.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butHomePage.Name = "butHomePage";
			this.butHomePage.Size = new System.Drawing.Size(102, 19);
			this.butHomePage.Text = "Home Page";
			this.butHomePage.Click += new System.EventHandler(this.butHomePage_Click);
			// 
			// butForum
			// 
			this.butForum.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butForum.Image = ((System.Drawing.Image)(resources.GetObject("butForum.Image")));
			this.butForum.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butForum.Name = "butForum";
			this.butForum.Size = new System.Drawing.Size(102, 19);
			this.butForum.Text = "Forum";
			this.butForum.Click += new System.EventHandler(this.butForum_Click);
			// 
			// butBugsManager
			// 
			this.butBugsManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butBugsManager.Image = ((System.Drawing.Image)(resources.GetObject("butBugsManager.Image")));
			this.butBugsManager.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butBugsManager.Name = "butBugsManager";
			this.butBugsManager.Size = new System.Drawing.Size(102, 19);
			this.butBugsManager.Text = "Bugs Manager";
			this.butBugsManager.Click += new System.EventHandler(this.butBugManager_Click);
			// 
			// butDatabaseSchema
			// 
			this.butDatabaseSchema.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butDatabaseSchema.Image = ((System.Drawing.Image)(resources.GetObject("butDatabaseSchema.Image")));
			this.butDatabaseSchema.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butDatabaseSchema.Name = "butDatabaseSchema";
			this.butDatabaseSchema.Size = new System.Drawing.Size(102, 19);
			this.butDatabaseSchema.Text = "Database Schema";
			this.butDatabaseSchema.Click += new System.EventHandler(this.butSchema_Click);
			// 
			// butPreviousVersions
			// 
			this.butPreviousVersions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butPreviousVersions.Image = ((System.Drawing.Image)(resources.GetObject("butPreviousVersions.Image")));
			this.butPreviousVersions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butPreviousVersions.Name = "butPreviousVersions";
			this.butPreviousVersions.Size = new System.Drawing.Size(102, 19);
			this.butPreviousVersions.Text = "Prev. Versions";
			this.butPreviousVersions.Click += new System.EventHandler(this.butBugsList_Click);
			// 
			// butUserGroup
			// 
			this.butUserGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butUserGroup.Image = ((System.Drawing.Image)(resources.GetObject("butUserGroup.Image")));
			this.butUserGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butUserGroup.Name = "butUserGroup";
			this.butUserGroup.Size = new System.Drawing.Size(102, 19);
			this.butUserGroup.Text = "User Group";
			this.butUserGroup.Click += new System.EventHandler(this.butUserGroup_Click);
			// 
			// butSchedules
			// 
			this.butSchedules.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.butSchedules.Image = ((System.Drawing.Image)(resources.GetObject("butSchedules.Image")));
			this.butSchedules.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.butSchedules.Name = "butSchedules";
			this.butSchedules.Size = new System.Drawing.Size(102, 19);
			this.butSchedules.Text = "Schedules";
			this.butSchedules.Click += new System.EventHandler(this.butSchedules_Click);
			// 
			// FormJobManagerOverview
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1508, 778);
			this.Controls.Add(this.panelSprintManager);
			this.Controls.Add(this.panelJobsReport);
			this.Controls.Add(this.toolMainLeft);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobManagerOverview";
			this.Text = "Job Manager Overview";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormJobManagerOverview_Load);
			this.panelSprintManager.ResumeLayout(false);
			this.tablePanelSprintManager.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.tabSprintManager.ResumeLayout(false);
			this.tabSprintManage.ResumeLayout(false);
			this.tabEngineers.ResumeLayout(false);
			this.panelJobsReport.ResumeLayout(false);
			this.tablePanelJobsReport.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.toolMainLeft.ResumeLayout(false);
			this.toolMainLeft.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel tablePanelJobsReport;
		private UI.GridOD gridJobs;
		private System.Windows.Forms.Panel panel1;
		private UI.Button butExport;
		private UI.Button butPrint;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioCustom;
		private System.Windows.Forms.RadioButton radioStaleJobs;
		private System.Windows.Forms.RadioButton radioNewVersion;
		private System.Windows.Forms.RadioButton radioCurrentVersion;
		private System.Windows.Forms.Label label3;
		private UI.ListBoxOD listPhases;
		private System.Windows.Forms.Label label5;
		private UI.ListBoxOD listCategories;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private UI.ListBoxOD listPriorities;
		private UI.ComboBoxOD comboEngineers;
		private UI.Button butRefresh;
		private System.Windows.Forms.CheckBox checkRemoveNoQuote;
		private UI.Button butPatSelect;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkRemoveApprovalJobs;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox textTotalBugs;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textTotalQuote;
		private System.Windows.Forms.TextBox textTotalHrsSpent;
		private System.Windows.Forms.TextBox textTotalHrsEst;
		private System.Windows.Forms.TextBox textTotalJobs;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox5;
		private UI.ODDateRangePicker dateRangeJobCompleted;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox textCompletionPercent;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textTotalFeatures;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Panel panelJobsReport;
		private System.Windows.Forms.Panel panelSprintManager;
		private System.Windows.Forms.ToolStripButton butSprintManager;
		private System.Windows.Forms.ToolStripButton butJobsReport;
		private System.Windows.Forms.ToolStripButton toolStripReporting;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripButton butHomePage;
		private System.Windows.Forms.ToolStripButton butForum;
		private System.Windows.Forms.ToolStripButton butBugsManager;
		private System.Windows.Forms.ToolStripButton butDatabaseSchema;
		private System.Windows.Forms.ToolStripButton butPreviousVersions;
		private System.Windows.Forms.ToolStripButton butUserGroup;
		private System.Windows.Forms.ToolStripButton butSchedules;
		private System.Windows.Forms.ToolStrip toolMainLeft;
		private System.Windows.Forms.TableLayoutPanel tablePanelSprintManager;
		private UI.GridOD gridSprints;
		private System.Windows.Forms.Panel panel3;
		private UserControlSprintManager userControlSprintManager;
		private System.Windows.Forms.TabControl tabSprintManager;
		private System.Windows.Forms.TabPage tabSprintManage;
		private System.Windows.Forms.TabPage tabEngineers;
		private UI.GridOD gridEngineers;
		private UI.ComboBoxOD comboProposedVersion;
		private System.Windows.Forms.Label label12;
	}
}