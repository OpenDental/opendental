namespace OpenDental {
	partial class FormRpProcNotBilledIns {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpProcNotBilledIns));
			this.butClose = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.checkMedical = new OpenDental.UI.CheckBox();
			this.imageListCalendar = new System.Windows.Forms.ImageList(this.components);
			this.gridMain = new OpenDental.UI.GridOD();
			this.butNewClaims = new OpenDental.UI.Button();
			this.contextMenuGrid = new System.Windows.Forms.ContextMenu();
			this.menuItemGoToAccount = new System.Windows.Forms.MenuItem();
			this.checkAutoGroupProcs = new OpenDental.UI.CheckBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.butSelectAll = new OpenDental.UI.Button();
			this.checkShowProcsNoIns = new OpenDental.UI.CheckBox();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.checkExcludeProcCodes = new OpenDental.UI.CheckBox();
			this.checkOnlyProcCodes = new OpenDental.UI.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textProcedureCodes = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkShowProcsInProcess = new OpenDental.UI.CheckBox();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(936, 665);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(25, 665);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 26);
			this.butPrint.TabIndex = 3;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// checkMedical
			// 
			this.checkMedical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedical.Location = new System.Drawing.Point(734, 12);
			this.checkMedical.Name = "checkMedical";
			this.checkMedical.Size = new System.Drawing.Size(211, 17);
			this.checkMedical.TabIndex = 11;
			this.checkMedical.Text = "Include Medical Procedures";
			this.checkMedical.Visible = false;
			// 
			// imageListCalendar
			// 
			this.imageListCalendar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListCalendar.ImageStream")));
			this.imageListCalendar.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListCalendar.Images.SetKeyName(0, "arrowDownTriangle.gif");
			this.imageListCalendar.Images.SetKeyName(1, "arrowUpTriangle.gif");
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(25, 119);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(967, 540);
			this.gridMain.TabIndex = 69;
			this.gridMain.Title = "Procedures Not Billed";
			this.gridMain.TranslationName = "TableProcedures";
			// 
			// butNewClaims
			// 
			this.butNewClaims.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butNewClaims.Location = new System.Drawing.Point(513, 666);
			this.butNewClaims.Name = "butNewClaims";
			this.butNewClaims.Size = new System.Drawing.Size(94, 24);
			this.butNewClaims.TabIndex = 71;
			this.butNewClaims.Text = "Create Claims";
			this.butNewClaims.UseVisualStyleBackColor = true;
			this.butNewClaims.Click += new System.EventHandler(this.butNewClaims_Click);
			// 
			// contextMenuGrid
			// 
			this.contextMenuGrid.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemGoToAccount});
			// 
			// menuItemGoToAccount
			// 
			this.menuItemGoToAccount.Index = 0;
			this.menuItemGoToAccount.Text = "Go To Account";
			this.menuItemGoToAccount.Click += new System.EventHandler(this.menuItemGridGoToAccount_Click);
			// 
			// checkAutoGroupProcs
			// 
			this.checkAutoGroupProcs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoGroupProcs.Location = new System.Drawing.Point(734, 33);
			this.checkAutoGroupProcs.Name = "checkAutoGroupProcs";
			this.checkAutoGroupProcs.Size = new System.Drawing.Size(211, 17);
			this.checkAutoGroupProcs.TabIndex = 72;
			this.checkAutoGroupProcs.Text = "Automatically Group Procedures";
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(871, 76);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 73;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butSelectAll.Location = new System.Drawing.Point(432, 666);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(75, 24);
			this.butSelectAll.TabIndex = 74;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.UseVisualStyleBackColor = true;
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// checkShowProcsNoIns
			// 
			this.checkShowProcsNoIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowProcsNoIns.Location = new System.Drawing.Point(33, 12);
			this.checkShowProcsNoIns.Name = "checkShowProcsNoIns";
			this.checkShowProcsNoIns.Size = new System.Drawing.Size(371, 17);
			this.checkShowProcsNoIns.TabIndex = 75;
			this.checkShowProcsNoIns.Text = "Show Procedures Completed Before Insurance Added";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkExcludeProcCodes);
			this.groupBox2.Controls.Add(this.checkOnlyProcCodes);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.butRefresh);
			this.groupBox2.Controls.Add(this.textProcedureCodes);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.comboClinics);
			this.groupBox2.Controls.Add(this.checkShowProcsInProcess);
			this.groupBox2.Controls.Add(this.checkShowProcsNoIns);
			this.groupBox2.Controls.Add(this.dateRangePicker);
			this.groupBox2.Controls.Add(this.checkMedical);
			this.groupBox2.Controls.Add(this.checkAutoGroupProcs);
			this.groupBox2.Location = new System.Drawing.Point(25, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(967, 109);
			this.groupBox2.TabIndex = 77;
			this.groupBox2.Text = "Filters";
			// 
			// checkExcludeProcCodes
			// 
			this.checkExcludeProcCodes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeProcCodes.Location = new System.Drawing.Point(472, 76);
			this.checkExcludeProcCodes.Name = "checkExcludeProcCodes";
			this.checkExcludeProcCodes.Size = new System.Drawing.Size(108, 17);
			this.checkExcludeProcCodes.TabIndex = 79;
			this.checkExcludeProcCodes.Text = "Exclude";
			this.checkExcludeProcCodes.Click += new System.EventHandler(this.checkExclude_Click);
			// 
			// checkOnlyProcCodes
			// 
			this.checkOnlyProcCodes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOnlyProcCodes.Location = new System.Drawing.Point(472, 57);
			this.checkOnlyProcCodes.Name = "checkOnlyProcCodes";
			this.checkOnlyProcCodes.Size = new System.Drawing.Size(108, 17);
			this.checkOnlyProcCodes.TabIndex = 78;
			this.checkOnlyProcCodes.Text = "Only";
			this.checkOnlyProcCodes.Click += new System.EventHandler(this.checkOnly_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(602, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 34);
			this.label2.TabIndex = 82;
			this.label2.Text = "separate multiple by commas";
			// 
			// textProcedureCodes
			// 
			this.textProcedureCodes.Location = new System.Drawing.Point(569, 35);
			this.textProcedureCodes.Name = "textProcedureCodes";
			this.textProcedureCodes.Size = new System.Drawing.Size(163, 20);
			this.textProcedureCodes.TabIndex = 81;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(469, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 80;
			this.label1.Text = "Procedure Codes";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinics
			// 
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeHiddenInAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(532, 12);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.SelectionModeMulti = true;
			this.comboClinics.Size = new System.Drawing.Size(200, 21);
			this.comboClinics.TabIndex = 77;
			// 
			// checkShowProcsInProcess
			// 
			this.checkShowProcsInProcess.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowProcsInProcess.Location = new System.Drawing.Point(748, 54);
			this.checkShowProcsInProcess.Name = "checkShowProcsInProcess";
			this.checkShowProcsInProcess.Size = new System.Drawing.Size(197, 17);
			this.checkShowProcsInProcess.TabIndex = 76;
			this.checkShowProcsInProcess.Text = "Show Procedures In Process";
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.dateRangePicker.EnableWeekButtons = false;
			this.dateRangePicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.dateRangePicker.Location = new System.Drawing.Point(1, 34);
			this.dateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 22);
			this.dateRangePicker.TabIndex = 0;
			// 
			// FormRpProcNotBilledIns
			// 
			this.AcceptButton = this.butPrint;
			this.ClientSize = new System.Drawing.Size(1019, 696);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.butNewClaims);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpProcNotBilledIns";
			this.Text = "Procedures Not Billed to Insurance";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRpProcNotBilledIns_FormClosing);
			this.Load += new System.EventHandler(this.FormProcNotAttach_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butPrint;
		private UI.Button butNewClaims;
		private OpenDental.UI.CheckBox checkMedical;
		private System.Windows.Forms.ContextMenu contextMenuGrid;
		private System.Windows.Forms.MenuItem menuItemGoToAccount;
		private UI.GridOD gridMain;
		private OpenDental.UI.CheckBox checkAutoGroupProcs;
		private UI.Button butRefresh;
		private System.Windows.Forms.ImageList imageListCalendar;
		private UI.Button butSelectAll;
		private OpenDental.UI.CheckBox checkShowProcsNoIns;
		private OpenDental.UI.GroupBox groupBox2;
		private UI.ODDateRangePicker dateRangePicker;
		private OpenDental.UI.CheckBox checkShowProcsInProcess;
		private UI.ComboBoxClinicPicker comboClinics;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textProcedureCodes;
		private System.Windows.Forms.Label label1;
		private UI.CheckBox checkExcludeProcCodes;
		private UI.CheckBox checkOnlyProcCodes;
	}
}
