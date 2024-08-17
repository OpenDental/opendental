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
			this.panel2 = new System.Windows.Forms.Panel();
			this.gridEngineerHours = new OpenDental.UI.GridOD();
			this.groupPrintOptions = new OpenDental.UI.GroupBox();
			this.butFixTopParents = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.groupFilters = new OpenDental.UI.GroupBox();
			this.textWithinLastXDays = new System.Windows.Forms.TextBox();
			this.labelWithinLastXDays = new System.Windows.Forms.Label();
			this.comboTeams = new OpenDental.UI.ComboBox();
			this.labelTeams = new System.Windows.Forms.Label();
			this.groupCompletedDateRange = new OpenDental.UI.GroupBox();
			this.dateRangeJobCompleted = new OpenDental.UI.ODDateRangePicker();
			this.checkRemoveApprovalJobs = new OpenDental.UI.CheckBox();
			this.checkRemoveNoQuote = new OpenDental.UI.CheckBox();
			this.butPatSelect = new OpenDental.UI.Button();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboEngineers = new OpenDental.UI.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.listPriorities = new OpenDental.UI.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.listPhases = new OpenDental.UI.ListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.listCategories = new OpenDental.UI.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupPresetFilters = new OpenDental.UI.GroupBox();
			this.radioLastThirtyDays = new System.Windows.Forms.RadioButton();
			this.radioLastSevenDays = new System.Windows.Forms.RadioButton();
			this.radioCurrentVersion = new System.Windows.Forms.RadioButton();
			this.gridJobs = new OpenDental.UI.GridOD();
			this.toolMainLeft = new System.Windows.Forms.ToolStrip();
			this.butJobsReport = new System.Windows.Forms.ToolStripButton();
			this.butSprintManager = new System.Windows.Forms.ToolStripButton();
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
			this.panel2.SuspendLayout();
			this.groupPrintOptions.SuspendLayout();
			this.groupFilters.SuspendLayout();
			this.groupCompletedDateRange.SuspendLayout();
			this.groupPresetFilters.SuspendLayout();
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
			this.tablePanelJobsReport.Controls.Add(this.panel2, 0, 0);
			this.tablePanelJobsReport.Controls.Add(this.gridJobs, 0, 1);
			this.tablePanelJobsReport.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tablePanelJobsReport.Location = new System.Drawing.Point(0, 0);
			this.tablePanelJobsReport.Name = "tablePanelJobsReport";
			this.tablePanelJobsReport.RowCount = 2;
			this.tablePanelJobsReport.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tablePanelJobsReport.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tablePanelJobsReport.Size = new System.Drawing.Size(1403, 778);
			this.tablePanelJobsReport.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.gridEngineerHours);
			this.panel2.Controls.Add(this.groupPrintOptions);
			this.panel2.Controls.Add(this.groupFilters);
			this.panel2.Controls.Add(this.groupPresetFilters);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1397, 221);
			this.panel2.TabIndex = 14;
			// 
			// gridEngineerHours
			// 
			this.gridEngineerHours.AllowSortingByColumn = true;
			this.gridEngineerHours.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridEngineerHours.Location = new System.Drawing.Point(1098, 0);
			this.gridEngineerHours.Name = "gridEngineerHours";
			this.gridEngineerHours.Size = new System.Drawing.Size(299, 221);
			this.gridEngineerHours.TabIndex = 4;
			this.gridEngineerHours.Title = "Engineer Hours";
			this.gridEngineerHours.TranslationName = "Engineer Hours";
			this.gridEngineerHours.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEngineerHours_CellClick);
			// 
			// groupPrintOptions
			// 
			this.groupPrintOptions.Controls.Add(this.butFixTopParents);
			this.groupPrintOptions.Controls.Add(this.butExport);
			this.groupPrintOptions.Controls.Add(this.butPrint);
			this.groupPrintOptions.Controls.Add(this.butRefresh);
			this.groupPrintOptions.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupPrintOptions.Location = new System.Drawing.Point(958, 0);
			this.groupPrintOptions.Name = "groupPrintOptions";
			this.groupPrintOptions.Size = new System.Drawing.Size(140, 221);
			this.groupPrintOptions.TabIndex = 2;
			this.groupPrintOptions.Text = "";
			// 
			// butFixTopParents
			// 
			this.butFixTopParents.Location = new System.Drawing.Point(29, 156);
			this.butFixTopParents.Name = "butFixTopParents";
			this.butFixTopParents.Size = new System.Drawing.Size(82, 24);
			this.butFixTopParents.TabIndex = 4;
			this.butFixTopParents.Text = "Fix Top Parent";
			this.butFixTopParents.Click += new System.EventHandler(this.butFixTopParents_Click);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExport.Image = global::OpenDental.Properties.Resources.butExport;
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(29, 24);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(79, 24);
			this.butExport.TabIndex = 0;
			this.butExport.Text = "&Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Enabled = false;
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(29, 54);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(79, 24);
			this.butPrint.TabIndex = 1;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(29, 186);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(82, 24);
			this.butRefresh.TabIndex = 3;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// groupFilters
			// 
			this.groupFilters.Controls.Add(this.textWithinLastXDays);
			this.groupFilters.Controls.Add(this.labelWithinLastXDays);
			this.groupFilters.Controls.Add(this.comboTeams);
			this.groupFilters.Controls.Add(this.labelTeams);
			this.groupFilters.Controls.Add(this.groupCompletedDateRange);
			this.groupFilters.Controls.Add(this.checkRemoveApprovalJobs);
			this.groupFilters.Controls.Add(this.checkRemoveNoQuote);
			this.groupFilters.Controls.Add(this.butPatSelect);
			this.groupFilters.Controls.Add(this.textPatNum);
			this.groupFilters.Controls.Add(this.label2);
			this.groupFilters.Controls.Add(this.comboEngineers);
			this.groupFilters.Controls.Add(this.label6);
			this.groupFilters.Controls.Add(this.listPriorities);
			this.groupFilters.Controls.Add(this.label3);
			this.groupFilters.Controls.Add(this.listPhases);
			this.groupFilters.Controls.Add(this.label5);
			this.groupFilters.Controls.Add(this.listCategories);
			this.groupFilters.Controls.Add(this.label4);
			this.groupFilters.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupFilters.Location = new System.Drawing.Point(141, 0);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(817, 221);
			this.groupFilters.TabIndex = 1;
			this.groupFilters.Text = "Filters";
			// 
			// textWithinLastXDays
			// 
			this.textWithinLastXDays.Location = new System.Drawing.Point(104, 78);
			this.textWithinLastXDays.Name = "textWithinLastXDays";
			this.textWithinLastXDays.Size = new System.Drawing.Size(80, 20);
			this.textWithinLastXDays.TabIndex = 4;
			this.textWithinLastXDays.Text = "30";
			// 
			// labelWithinLastXDays
			// 
			this.labelWithinLastXDays.Location = new System.Drawing.Point(12, 81);
			this.labelWithinLastXDays.Name = "labelWithinLastXDays";
			this.labelWithinLastXDays.Size = new System.Drawing.Size(91, 13);
			this.labelWithinLastXDays.TabIndex = 252;
			this.labelWithinLastXDays.Text = "Within last X days";
			// 
			// comboTeams
			// 
			this.comboTeams.Location = new System.Drawing.Point(104, 13);
			this.comboTeams.Name = "comboTeams";
			this.comboTeams.Size = new System.Drawing.Size(164, 21);
			this.comboTeams.TabIndex = 0;
			this.comboTeams.SelectionChangeCommitted += new System.EventHandler(this.comboTeams_SelectionChangeCommitted);
			// 
			// labelTeams
			// 
			this.labelTeams.Location = new System.Drawing.Point(64, 17);
			this.labelTeams.Name = "labelTeams";
			this.labelTeams.Size = new System.Drawing.Size(39, 13);
			this.labelTeams.TabIndex = 250;
			this.labelTeams.Text = "Teams";
			// 
			// groupCompletedDateRange
			// 
			this.groupCompletedDateRange.Controls.Add(this.dateRangeJobCompleted);
			this.groupCompletedDateRange.Location = new System.Drawing.Point(11, 103);
			this.groupCompletedDateRange.Name = "groupCompletedDateRange";
			this.groupCompletedDateRange.Size = new System.Drawing.Size(212, 72);
			this.groupCompletedDateRange.TabIndex = 5;
			this.groupCompletedDateRange.Text = "If Completed, Completed Date Between";
			// 
			// dateRangeJobCompleted
			// 
			this.dateRangeJobCompleted.BackColor = System.Drawing.Color.Transparent;
			this.dateRangeJobCompleted.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.dateRangeJobCompleted.IsVertical = true;
			this.dateRangeJobCompleted.Location = new System.Drawing.Point(6, 19);
			this.dateRangeJobCompleted.MinimumSize = new System.Drawing.Size(165, 46);
			this.dateRangeJobCompleted.Name = "dateRangeJobCompleted";
			this.dateRangeJobCompleted.Size = new System.Drawing.Size(200, 46);
			this.dateRangeJobCompleted.TabIndex = 0;
			// 
			// checkRemoveApprovalJobs
			// 
			this.checkRemoveApprovalJobs.Location = new System.Drawing.Point(12, 201);
			this.checkRemoveApprovalJobs.Name = "checkRemoveApprovalJobs";
			this.checkRemoveApprovalJobs.Size = new System.Drawing.Size(136, 17);
			this.checkRemoveApprovalJobs.TabIndex = 7;
			this.checkRemoveApprovalJobs.Text = "Remove Approval Jobs";
			// 
			// checkRemoveNoQuote
			// 
			this.checkRemoveNoQuote.Location = new System.Drawing.Point(12, 178);
			this.checkRemoveNoQuote.Name = "checkRemoveNoQuote";
			this.checkRemoveNoQuote.Size = new System.Drawing.Size(121, 17);
			this.checkRemoveNoQuote.TabIndex = 6;
			this.checkRemoveNoQuote.Text = "Remove Un-Quoted";
			// 
			// butPatSelect
			// 
			this.butPatSelect.Location = new System.Drawing.Point(184, 57);
			this.butPatSelect.Name = "butPatSelect";
			this.butPatSelect.Size = new System.Drawing.Size(19, 20);
			this.butPatSelect.TabIndex = 3;
			this.butPatSelect.Text = "...";
			this.butPatSelect.Click += new System.EventHandler(this.butPatSelect_Click);
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(104, 57);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(80, 20);
			this.textPatNum.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(38, 58);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 17);
			this.label2.TabIndex = 241;
			this.label2.Text = "Customer";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboEngineers
			// 
			this.comboEngineers.Location = new System.Drawing.Point(104, 35);
			this.comboEngineers.Name = "comboEngineers";
			this.comboEngineers.Size = new System.Drawing.Size(164, 21);
			this.comboEngineers.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(638, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(46, 13);
			this.label6.TabIndex = 24;
			this.label6.Text = "Priorities";
			// 
			// listPriorities
			// 
			this.listPriorities.Location = new System.Drawing.Point(641, 24);
			this.listPriorities.Name = "listPriorities";
			this.listPriorities.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPriorities.Size = new System.Drawing.Size(162, 186);
			this.listPriorities.TabIndex = 10;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(49, 39);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 13);
			this.label3.TabIndex = 20;
			this.label3.Text = "Engineers";
			// 
			// listPhases
			// 
			this.listPhases.Location = new System.Drawing.Point(289, 24);
			this.listPhases.Name = "listPhases";
			this.listPhases.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPhases.Size = new System.Drawing.Size(170, 186);
			this.listPhases.TabIndex = 8;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(462, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(57, 13);
			this.label5.TabIndex = 22;
			this.label5.Text = "Categories";
			// 
			// listCategories
			// 
			this.listCategories.Location = new System.Drawing.Point(465, 24);
			this.listCategories.Name = "listCategories";
			this.listCategories.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listCategories.Size = new System.Drawing.Size(170, 186);
			this.listCategories.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(286, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 13);
			this.label4.TabIndex = 21;
			this.label4.Text = "Phases";
			// 
			// groupPresetFilters
			// 
			this.groupPresetFilters.Controls.Add(this.radioLastThirtyDays);
			this.groupPresetFilters.Controls.Add(this.radioLastSevenDays);
			this.groupPresetFilters.Controls.Add(this.radioCurrentVersion);
			this.groupPresetFilters.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupPresetFilters.Location = new System.Drawing.Point(0, 0);
			this.groupPresetFilters.Name = "groupPresetFilters";
			this.groupPresetFilters.Size = new System.Drawing.Size(141, 221);
			this.groupPresetFilters.TabIndex = 0;
			this.groupPresetFilters.Text = "Preset Filters";
			// 
			// radioLastThirtyDays
			// 
			this.radioLastThirtyDays.AutoSize = true;
			this.radioLastThirtyDays.Location = new System.Drawing.Point(7, 41);
			this.radioLastThirtyDays.Name = "radioLastThirtyDays";
			this.radioLastThirtyDays.Size = new System.Drawing.Size(87, 17);
			this.radioLastThirtyDays.TabIndex = 1;
			this.radioLastThirtyDays.Text = "Last 30 Days";
			this.radioLastThirtyDays.UseVisualStyleBackColor = true;
			this.radioLastThirtyDays.Click += new System.EventHandler(this.radioLastThirtyDays_Click);
			// 
			// radioLastSevenDays
			// 
			this.radioLastSevenDays.AutoSize = true;
			this.radioLastSevenDays.Checked = true;
			this.radioLastSevenDays.Location = new System.Drawing.Point(7, 19);
			this.radioLastSevenDays.Name = "radioLastSevenDays";
			this.radioLastSevenDays.Size = new System.Drawing.Size(81, 17);
			this.radioLastSevenDays.TabIndex = 0;
			this.radioLastSevenDays.TabStop = true;
			this.radioLastSevenDays.Text = "Last 7 Days";
			this.radioLastSevenDays.UseVisualStyleBackColor = true;
			this.radioLastSevenDays.Click += new System.EventHandler(this.radioLastSevenDays_Click);
			// 
			// radioCurrentVersion
			// 
			this.radioCurrentVersion.AutoSize = true;
			this.radioCurrentVersion.Location = new System.Drawing.Point(7, 63);
			this.radioCurrentVersion.Name = "radioCurrentVersion";
			this.radioCurrentVersion.Size = new System.Drawing.Size(125, 17);
			this.radioCurrentVersion.TabIndex = 2;
			this.radioCurrentVersion.Text = "Current Development";
			this.radioCurrentVersion.UseVisualStyleBackColor = true;
			this.radioCurrentVersion.Click += new System.EventHandler(this.radioCurrentVersion_Click);
			// 
			// gridJobs
			// 
			this.gridJobs.AllowSortingByColumn = true;
			this.gridJobs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridJobs.Location = new System.Drawing.Point(3, 230);
			this.gridJobs.Name = "gridJobs";
			this.gridJobs.Size = new System.Drawing.Size(1397, 545);
			this.gridJobs.TabIndex = 0;
			this.gridJobs.Title = "Jobs";
			this.gridJobs.TranslationName = "Jobs";
			this.gridJobs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridJobs_CellDoubleClick);
			// 
			// toolMainLeft
			// 
			this.toolMainLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.toolMainLeft.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.butJobsReport,
            this.butSprintManager,
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
			this.toolMainLeft.TabIndex = 0;
			this.toolMainLeft.Text = "toolStrip1";
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
			this.ClientSize = new System.Drawing.Size(1508, 778);
			this.Controls.Add(this.panelJobsReport);
			this.Controls.Add(this.panelSprintManager);
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
			this.panel2.ResumeLayout(false);
			this.groupPrintOptions.ResumeLayout(false);
			this.groupFilters.ResumeLayout(false);
			this.groupFilters.PerformLayout();
			this.groupCompletedDateRange.ResumeLayout(false);
			this.groupPresetFilters.ResumeLayout(false);
			this.groupPresetFilters.PerformLayout();
			this.toolMainLeft.ResumeLayout(false);
			this.toolMainLeft.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel tablePanelJobsReport;
		private UI.GridOD gridJobs;
		private System.Windows.Forms.Panel panel2;
		private OpenDental.UI.GroupBox groupFilters;
		private OpenDental.UI.GroupBox groupPresetFilters;
		private System.Windows.Forms.RadioButton radioCurrentVersion;
		private System.Windows.Forms.Label label3;
		private UI.ListBox listPhases;
		private System.Windows.Forms.Label label5;
		private UI.ListBox listCategories;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private UI.ListBox listPriorities;
		private UI.ComboBox comboEngineers;
		private UI.Button butRefresh;
		private OpenDental.UI.CheckBox checkRemoveNoQuote;
		private UI.Button butPatSelect;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.CheckBox checkRemoveApprovalJobs;
		private OpenDental.UI.GroupBox groupCompletedDateRange;
		private UI.ODDateRangePicker dateRangeJobCompleted;
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
		private UI.ComboBox comboTeams;
		private System.Windows.Forms.Label labelTeams;
		private UI.GroupBox groupPrintOptions;
		private UI.Button butExport;
		private UI.Button butPrint;
		private System.Windows.Forms.RadioButton radioLastThirtyDays;
		private System.Windows.Forms.RadioButton radioLastSevenDays;
		private System.Windows.Forms.TextBox textWithinLastXDays;
		private System.Windows.Forms.Label labelWithinLastXDays;
		private UI.GridOD gridEngineerHours;
		private UI.Button butFixTopParents;
	}
}