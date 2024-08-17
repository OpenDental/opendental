﻿namespace OpenDental {
	partial class FormRpDPPOvercharged {
		///<summary>Required designer variable.</summary>
		private System.ComponentModel.IContainer components = null;
		///<summary></summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null)) {
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		///<summary>Required method for Designer support - do not modify the contents of this method with the code editor.</summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpDPPOvercharged));
			this.butClose = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.imageListCalendar = new System.Windows.Forms.ImageList(this.components);
			this.gridMain = new OpenDental.UI.GridOD();
			this.contextMenuGrid = new System.Windows.Forms.ContextMenu();
			this.menuItemGoToAccount = new System.Windows.Forms.MenuItem();
			this.butRefresh = new OpenDental.UI.Button();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.butCurrent = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.butFind = new OpenDental.UI.Button();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.labelPatient = new System.Windows.Forms.Label();
			this.labelProv = new System.Windows.Forms.Label();
			this.comboBoxMultiProv = new OpenDental.UI.ComboBox();
			this.comboBoxMultiClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.butFix = new OpenDental.UI.Button();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1102, 658);
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
			this.butPrint.Location = new System.Drawing.Point(12, 658);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 26);
			this.butPrint.TabIndex = 3;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
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
			this.gridMain.HasAutoWrappedHeaders = true;
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(12, 71);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1165, 581);
			this.gridMain.TabIndex = 69;
			this.gridMain.Title = "Overcharged Production";
			this.gridMain.TranslationName = "TableProcedures";
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
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(1102, 39);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 26);
			this.butRefresh.TabIndex = 73;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.butCurrent);
			this.groupBox2.Controls.Add(this.butAll);
			this.groupBox2.Controls.Add(this.butFind);
			this.groupBox2.Controls.Add(this.textPatient);
			this.groupBox2.Controls.Add(this.labelPatient);
			this.groupBox2.Controls.Add(this.labelProv);
			this.groupBox2.Controls.Add(this.comboBoxMultiProv);
			this.groupBox2.Controls.Add(this.comboBoxMultiClinics);
			this.groupBox2.Controls.Add(this.dateRangePicker);
			this.groupBox2.Location = new System.Drawing.Point(12, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(910, 61);
			this.groupBox2.TabIndex = 77;
			this.groupBox2.Text = "Filters";
			// 
			// butCurrent
			// 
			this.butCurrent.Location = new System.Drawing.Point(698, 31);
			this.butCurrent.Name = "butCurrent";
			this.butCurrent.Size = new System.Drawing.Size(63, 24);
			this.butCurrent.TabIndex = 78;
			this.butCurrent.Text = "Current";
			this.butCurrent.Click += new System.EventHandler(this.butCurrent_Click);
			// 
			// butAll
			// 
			this.butAll.Location = new System.Drawing.Point(841, 31);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(63, 24);
			this.butAll.TabIndex = 77;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butFind
			// 
			this.butFind.Location = new System.Drawing.Point(770, 31);
			this.butFind.Name = "butFind";
			this.butFind.Size = new System.Drawing.Size(63, 24);
			this.butFind.TabIndex = 76;
			this.butFind.Text = "Find";
			this.butFind.Click += new System.EventHandler(this.butFind_Click);
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(698, 11);
			this.textPatient.Name = "textPatient";
			this.textPatient.Size = new System.Drawing.Size(206, 20);
			this.textPatient.TabIndex = 75;
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(631, 15);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(65, 13);
			this.labelPatient.TabIndex = 74;
			this.labelPatient.Text = "Patient";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelProv
			// 
			this.labelProv.Location = new System.Drawing.Point(412, 15);
			this.labelProv.Name = "labelProv";
			this.labelProv.Size = new System.Drawing.Size(55, 16);
			this.labelProv.TabIndex = 73;
			this.labelProv.Text = "Providers";
			this.labelProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxMultiProv
			// 
			this.comboBoxMultiProv.BackColor = System.Drawing.SystemColors.Window;
			this.comboBoxMultiProv.Location = new System.Drawing.Point(469, 12);
			this.comboBoxMultiProv.Name = "comboBoxMultiProv";
			this.comboBoxMultiProv.SelectionModeMulti = true;
			this.comboBoxMultiProv.Size = new System.Drawing.Size(160, 21);
			this.comboBoxMultiProv.TabIndex = 72;
			this.comboBoxMultiProv.SelectionChangeCommitted += new System.EventHandler(this.comboBoxMultiProv_SelectionChangeCommitted);
			// 
			// comboBoxMultiClinics
			// 
			this.comboBoxMultiClinics.IncludeAll = true;
			this.comboBoxMultiClinics.IncludeHiddenInAll = true;
			this.comboBoxMultiClinics.IncludeUnassigned = true;
			this.comboBoxMultiClinics.Location = new System.Drawing.Point(432, 34);
			this.comboBoxMultiClinics.Name = "comboBoxMultiClinics";
			this.comboBoxMultiClinics.SelectionModeMulti = true;
			this.comboBoxMultiClinics.Size = new System.Drawing.Size(197, 21);
			this.comboBoxMultiClinics.TabIndex = 71;
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.dateRangePicker.EnableWeekButtons = false;
			this.dateRangePicker.Location = new System.Drawing.Point(5, 11);
			this.dateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 22);
			this.dateRangePicker.TabIndex = 0;
			// 
			// butFix
			// 
			this.butFix.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butFix.Location = new System.Drawing.Point(548, 658);
			this.butFix.Name = "butFix";
			this.butFix.Size = new System.Drawing.Size(75, 26);
			this.butFix.TabIndex = 78;
			this.butFix.Text = "Fix";
			this.butFix.UseVisualStyleBackColor = true;
			this.butFix.Click += new System.EventHandler(this.butFix_Click);
			// 
			// FormRpDPPOvercharged
			// 
			this.AcceptButton = this.butPrint;
			this.ClientSize = new System.Drawing.Size(1189, 696);
			this.Controls.Add(this.butFix);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpDPPOvercharged";
			this.Text = "Dynamic Pay Plans Overcharged Report";
			this.Load += new System.EventHandler(this.FormRpDPPOvercharged_Load);
			this.Shown += new System.EventHandler(this.FormRpDPPOvercharged_Shown);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.ContextMenu contextMenuGrid;
		private System.Windows.Forms.MenuItem menuItemGoToAccount;
		private System.Windows.Forms.ImageList imageListCalendar;
		private UI.Button butRefresh;
		private UI.Button butClose;
		private UI.Button butPrint;
		private UI.GridOD gridMain;
		private OpenDental.UI.GroupBox groupBox2;
		private UI.ODDateRangePicker dateRangePicker;
		private UI.ComboBoxClinicPicker comboBoxMultiClinics;
		private UI.ComboBox comboBoxMultiProv;
		private System.Windows.Forms.Label labelProv;
		private UI.Button butCurrent;
		private UI.Button butAll;
		private UI.Button butFind;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label labelPatient;
		private UI.Button butFix;
	}
}
