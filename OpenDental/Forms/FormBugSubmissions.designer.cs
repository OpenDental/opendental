namespace OpenDental{
	partial class FormBugSubmissions {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBugSubmissions));
			this.butAddJob = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridSubs = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textMsgText = new OpenDental.ODtextBox();
			this.textStackFilter = new OpenDental.ODtextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupFilters = new System.Windows.Forms.GroupBox();
			this.butRefreshMobile = new OpenDental.UI.Button();
			this.listVersionsFilter = new OpenDental.UI.ListBoxOD();
			this.textCategoryFilters = new OpenDental.ODtextBox();
			this.listShowHideOptions = new OpenDental.UI.ListBoxOD();
			this.textDevNoteFilter = new OpenDental.ODtextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textPatNums = new OpenDental.ODtextBox();
			this.comboRegKeys = new OpenDental.UI.ComboBoxOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.comboSortBy = new System.Windows.Forms.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.comboGrouping = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.bugSubmissionControl = new OpenDental.UI.BugSubmissionControl();
			this.labelDateTime = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.labelHashNum = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.groupFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// butAddJob
			// 
			this.butAddJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddJob.Location = new System.Drawing.Point(1043, 723);
			this.butAddJob.Name = "butAddJob";
			this.butAddJob.Size = new System.Drawing.Size(85, 24);
			this.butAddJob.TabIndex = 3;
			this.butAddJob.Text = "&Add Job";
			this.butAddJob.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1134, 723);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(85, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridSubs
			// 
			this.gridSubs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridSubs.Location = new System.Drawing.Point(12, 127);
			this.gridSubs.Name = "gridSubs";
			this.gridSubs.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridSubs.Size = new System.Drawing.Size(532, 619);
			this.gridSubs.TabIndex = 4;
			this.gridSubs.Title = "Submissions";
			this.gridSubs.TranslationName = "TableSubmissions";
			this.gridSubs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSubs_CellDoubleClick);
			this.gridSubs.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSubs_CellClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(444, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 14);
			this.label1.TabIndex = 7;
			this.label1.Text = "Categories (CSV)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(178, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 14);
			this.label2.TabIndex = 8;
			this.label2.Text = "Message Text";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(441, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(97, 14);
			this.label3.TabIndex = 9;
			this.label3.Text = "Stack Trace (CSV)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMsgText
			// 
			this.textMsgText.AcceptsTab = true;
			this.textMsgText.BackColor = System.Drawing.SystemColors.Window;
			this.textMsgText.DetectLinksEnabled = false;
			this.textMsgText.DetectUrls = false;
			this.textMsgText.Location = new System.Drawing.Point(261, 13);
			this.textMsgText.Multiline = false;
			this.textMsgText.Name = "textMsgText";
			this.textMsgText.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textMsgText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMsgText.Size = new System.Drawing.Size(174, 21);
			this.textMsgText.TabIndex = 10;
			this.textMsgText.Text = "";
			// 
			// textStackFilter
			// 
			this.textStackFilter.AcceptsTab = true;
			this.textStackFilter.BackColor = System.Drawing.SystemColors.Window;
			this.textStackFilter.DetectLinksEnabled = false;
			this.textStackFilter.DetectUrls = false;
			this.textStackFilter.Location = new System.Drawing.Point(544, 48);
			this.textStackFilter.Multiline = false;
			this.textStackFilter.Name = "textStackFilter";
			this.textStackFilter.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textStackFilter.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textStackFilter.Size = new System.Drawing.Size(177, 21);
			this.textStackFilter.TabIndex = 11;
			this.textStackFilter.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(181, 51);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(74, 14);
			this.label4.TabIndex = 12;
			this.label4.Text = "Submitters";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupFilters
			// 
			this.groupFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupFilters.Controls.Add(this.butRefreshMobile);
			this.groupFilters.Controls.Add(this.listVersionsFilter);
			this.groupFilters.Controls.Add(this.textCategoryFilters);
			this.groupFilters.Controls.Add(this.listShowHideOptions);
			this.groupFilters.Controls.Add(this.textDevNoteFilter);
			this.groupFilters.Controls.Add(this.label15);
			this.groupFilters.Controls.Add(this.textPatNums);
			this.groupFilters.Controls.Add(this.comboRegKeys);
			this.groupFilters.Controls.Add(this.butRefresh);
			this.groupFilters.Controls.Add(this.label3);
			this.groupFilters.Controls.Add(this.label4);
			this.groupFilters.Controls.Add(this.label1);
			this.groupFilters.Controls.Add(this.textStackFilter);
			this.groupFilters.Controls.Add(this.label2);
			this.groupFilters.Controls.Add(this.textMsgText);
			this.groupFilters.Controls.Add(this.label7);
			this.groupFilters.Controls.Add(this.dateRangePicker);
			this.groupFilters.Location = new System.Drawing.Point(12, 27);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(1208, 77);
			this.groupFilters.TabIndex = 14;
			this.groupFilters.TabStop = false;
			this.groupFilters.Text = "Filters";
			// 
			// butRefreshMobile
			// 
			this.butRefreshMobile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefreshMobile.Location = new System.Drawing.Point(1109, 19);
			this.butRefreshMobile.Name = "butRefreshMobile";
			this.butRefreshMobile.Size = new System.Drawing.Size(93, 24);
			this.butRefreshMobile.TabIndex = 46;
			this.butRefreshMobile.Text = "&Refresh Mobile";
			this.butRefreshMobile.Click += new System.EventHandler(this.butRefreshMobile_Click);
			// 
			// listVersionsFilter
			// 
			this.listVersionsFilter.Location = new System.Drawing.Point(899, 12);
			this.listVersionsFilter.Name = "listVersionsFilter";
			this.listVersionsFilter.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listVersionsFilter.Size = new System.Drawing.Size(106, 56);
			this.listVersionsFilter.TabIndex = 45;
			// 
			// textCategoryFilters
			// 
			this.textCategoryFilters.AcceptsTab = true;
			this.textCategoryFilters.BackColor = System.Drawing.SystemColors.Window;
			this.textCategoryFilters.DetectLinksEnabled = false;
			this.textCategoryFilters.DetectUrls = false;
			this.textCategoryFilters.Location = new System.Drawing.Point(544, 13);
			this.textCategoryFilters.Multiline = false;
			this.textCategoryFilters.Name = "textCategoryFilters";
			this.textCategoryFilters.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textCategoryFilters.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textCategoryFilters.Size = new System.Drawing.Size(177, 21);
			this.textCategoryFilters.TabIndex = 44;
			this.textCategoryFilters.Text = "";
			// 
			// listShowHideOptions
			// 
			this.listShowHideOptions.ItemStrings = new string[] {
        "Show HQ",
        "Show Attached",
        "Show Hidden",
        "Min Count"};
			this.listShowHideOptions.Location = new System.Drawing.Point(1011, 12);
			this.listShowHideOptions.Name = "listShowHideOptions";
			this.listShowHideOptions.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listShowHideOptions.Size = new System.Drawing.Size(92, 56);
			this.listShowHideOptions.TabIndex = 43;
			this.listShowHideOptions.SelectedIndexChanged += new System.EventHandler(this.ListShowHideOptions_SelectedIndexChanged);
			// 
			// textDevNoteFilter
			// 
			this.textDevNoteFilter.AcceptsTab = true;
			this.textDevNoteFilter.BackColor = System.Drawing.SystemColors.Window;
			this.textDevNoteFilter.DetectLinksEnabled = false;
			this.textDevNoteFilter.DetectUrls = false;
			this.textDevNoteFilter.Location = new System.Drawing.Point(781, 47);
			this.textDevNoteFilter.Multiline = false;
			this.textDevNoteFilter.Name = "textDevNoteFilter";
			this.textDevNoteFilter.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textDevNoteFilter.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDevNoteFilter.Size = new System.Drawing.Size(102, 22);
			this.textDevNoteFilter.TabIndex = 31;
			this.textDevNoteFilter.Text = "";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(727, 52);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(50, 13);
			this.label15.TabIndex = 32;
			this.label15.Text = "DevNote";
			// 
			// textPatNums
			// 
			this.textPatNums.AcceptsTab = true;
			this.textPatNums.BackColor = System.Drawing.SystemColors.Window;
			this.textPatNums.DetectLinksEnabled = false;
			this.textPatNums.DetectUrls = false;
			this.textPatNums.Location = new System.Drawing.Point(781, 12);
			this.textPatNums.Multiline = false;
			this.textPatNums.Name = "textPatNums";
			this.textPatNums.QuickPasteType = OpenDentBusiness.QuickPasteType.JobManager;
			this.textPatNums.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPatNums.Size = new System.Drawing.Size(102, 22);
			this.textPatNums.TabIndex = 29;
			this.textPatNums.Text = "";
			// 
			// comboRegKeys
			// 
			this.comboRegKeys.BackColor = System.Drawing.SystemColors.Window;
			this.comboRegKeys.Location = new System.Drawing.Point(261, 48);
			this.comboRegKeys.Name = "comboRegKeys";
			this.comboRegKeys.SelectionModeMulti = true;
			this.comboRegKeys.IncludeAll = true;
			this.comboRegKeys.Size = new System.Drawing.Size(174, 21);
			this.comboRegKeys.TabIndex = 27;
			this.comboRegKeys.SelectionChangeCommitted += new System.EventHandler(this.comboVersions_SelectionChangeCommitted);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(1109, 48);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(93, 24);
			this.butRefresh.TabIndex = 26;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(727, 17);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(56, 13);
			this.label7.TabIndex = 30;
			this.label7.Text = "PatNum(s)";
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.SystemColors.Control;
			this.dateRangePicker.EnableWeekButtons = false;
			this.dateRangePicker.IsVertical = true;
			this.dateRangePicker.Location = new System.Drawing.Point(5, 13);
			this.dateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(165, 46);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(165, 46);
			this.dateRangePicker.TabIndex = 14;
			this.dateRangePicker.CalendarClosed += new OpenDental.UI.CalendarClosedHandler(this.dateRangePicker_CalendarClosed);
			// 
			// comboSortBy
			// 
			this.comboSortBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSortBy.FormattingEnabled = true;
			this.comboSortBy.Location = new System.Drawing.Point(219, 104);
			this.comboSortBy.Name = "comboSortBy";
			this.comboSortBy.Size = new System.Drawing.Size(102, 21);
			this.comboSortBy.TabIndex = 38;
			this.comboSortBy.SelectionChangeCommitted += new System.EventHandler(this.comboVersions_SelectionChangeCommitted);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(175, 109);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(44, 13);
			this.label14.TabIndex = 37;
			this.label14.Text = "Sort By:";
			// 
			// comboGrouping
			// 
			this.comboGrouping.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboGrouping.FormattingEnabled = true;
			this.comboGrouping.Location = new System.Drawing.Point(65, 104);
			this.comboGrouping.Name = "comboGrouping";
			this.comboGrouping.Size = new System.Drawing.Size(102, 21);
			this.comboGrouping.TabIndex = 36;
			this.comboGrouping.SelectionChangeCommitted += new System.EventHandler(this.comboVersions_SelectionChangeCommitted);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(12, 109);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(54, 13);
			this.label13.TabIndex = 35;
			this.label13.Text = "Group By:";
			// 
			// bugSubmissionControl
			// 
			this.bugSubmissionControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.bugSubmissionControl.ControlMode = BugSubmissionControlMode.General;
			this.bugSubmissionControl.Location = new System.Drawing.Point(550, 124);
			this.bugSubmissionControl.MinimumSize = new System.Drawing.Size(594, 521);
			this.bugSubmissionControl.Name = "bugSubmissionControl";
			this.bugSubmissionControl.Size = new System.Drawing.Size(673, 624);
			this.bugSubmissionControl.TabIndex = 40;
			// 
			// labelDateTime
			// 
			this.labelDateTime.Location = new System.Drawing.Point(616, 112);
			this.labelDateTime.Name = "labelDateTime";
			this.labelDateTime.Size = new System.Drawing.Size(173, 13);
			this.labelDateTime.TabIndex = 42;
			this.labelDateTime.Text = "XXXXX";
			this.labelDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(553, 112);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(65, 13);
			this.label5.TabIndex = 41;
			this.label5.Text = "DateTime:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelHashNum
			// 
			this.labelHashNum.Location = new System.Drawing.Point(853, 112);
			this.labelHashNum.Name = "labelHashNum";
			this.labelHashNum.Size = new System.Drawing.Size(215, 13);
			this.labelHashNum.TabIndex = 44;
			this.labelHashNum.Text = "XXXXX";
			this.labelHashNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(790, 112);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(65, 13);
			this.label6.TabIndex = 43;
			this.label6.Text = "HashNum:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(1232, 24);
			this.menuMain.TabIndex = 45;
			// 
			// FormBugSubmissions
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1232, 760);
			this.Controls.Add(this.labelHashNum);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.labelDateTime);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboSortBy);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.comboGrouping);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.gridSubs);
			this.Controls.Add(this.butAddJob);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.bugSubmissionControl);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBugSubmissions";
			this.Text = "Bug Submissions";
			this.Load += new System.EventHandler(this.FormBugSubmissions_Load);
			this.groupFilters.ResumeLayout(false);
			this.groupFilters.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butAddJob;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridSubs;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private ODtextBox textMsgText;
		private ODtextBox textStackFilter;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupFilters;
		private UI.ODDateRangePicker dateRangePicker;
		private UI.Button butRefresh;
		private UI.ComboBoxOD comboRegKeys;
		private System.Windows.Forms.Label label7;
		private ODtextBox textPatNums;
		private System.Windows.Forms.ComboBox comboSortBy;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.ComboBox comboGrouping;
		private System.Windows.Forms.Label label13;
		private ODtextBox textDevNoteFilter;
		private System.Windows.Forms.Label label15;
		private UI.BugSubmissionControl bugSubmissionControl;
		private System.Windows.Forms.Label labelDateTime;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.ListBoxOD listShowHideOptions;
		private ODtextBox textCategoryFilters;
		private OpenDental.UI.ListBoxOD listVersionsFilter;
		private UI.Button butRefreshMobile;
		private System.Windows.Forms.Label labelHashNum;
		private System.Windows.Forms.Label label6;
		private UI.MenuOD menuMain;
	}
}