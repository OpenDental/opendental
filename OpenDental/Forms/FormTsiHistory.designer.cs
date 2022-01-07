using System;

namespace OpenDental{
	partial class FormTsiHistory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTsiHistory));
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemGoTo = new System.Windows.Forms.ToolStripMenuItem();
			this.butClose = new OpenDental.UI.Button();
			this.checkShowPatNums = new System.Windows.Forms.CheckBox();
			this.groupFilters = new System.Windows.Forms.GroupBox();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.datePicker = new OpenDental.UI.ODDateRangePicker();
			this.comboClientIDs = new OpenDental.UI.ComboBoxOD();
			this.labelClientIDs = new System.Windows.Forms.Label();
			this.comboAcctStatuses = new OpenDental.UI.ComboBoxOD();
			this.labelAccountStatuses = new System.Windows.Forms.Label();
			this.comboTransTypes = new OpenDental.UI.ComboBoxOD();
			this.labelTransTypes = new System.Windows.Forms.Label();
			this.butCurrent = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.butFind = new OpenDental.UI.Button();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.labelPatient = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupRawMsg = new System.Windows.Forms.GroupBox();
			this.labelSelectedFieldDetails = new System.Windows.Forms.Label();
			this.textSelectedFieldDetails = new System.Windows.Forms.TextBox();
			this.labelSelectedFieldName = new System.Windows.Forms.Label();
			this.textSelectedFieldName = new System.Windows.Forms.TextBox();
			this.textRawMsg = new System.Windows.Forms.RichTextBox();
			this.contextMenu.SuspendLayout();
			this.groupFilters.SuspendLayout();
			this.groupRawMsg.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemGoTo});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(106, 26);
			// 
			// menuItemGoTo
			// 
			this.menuItemGoTo.Name = "menuItemGoTo";
			this.menuItemGoTo.Size = new System.Drawing.Size(105, 22);
			this.menuItemGoTo.Text = "Go To";
			this.menuItemGoTo.Click += new System.EventHandler(this.menuItemGoTo_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1143, 662);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 5;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// checkShowPatNums
			// 
			this.checkShowPatNums.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowPatNums.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowPatNums.Location = new System.Drawing.Point(12, 184);
			this.checkShowPatNums.Name = "checkShowPatNums";
			this.checkShowPatNums.Size = new System.Drawing.Size(110, 16);
			this.checkShowPatNums.TabIndex = 2;
			this.checkShowPatNums.Text = "Show PatNums";
			this.checkShowPatNums.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupFilters
			// 
			this.groupFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupFilters.Controls.Add(this.comboClinics);
			this.groupFilters.Controls.Add(this.datePicker);
			this.groupFilters.Controls.Add(this.comboClientIDs);
			this.groupFilters.Controls.Add(this.labelClientIDs);
			this.groupFilters.Controls.Add(this.comboAcctStatuses);
			this.groupFilters.Controls.Add(this.labelAccountStatuses);
			this.groupFilters.Controls.Add(this.comboTransTypes);
			this.groupFilters.Controls.Add(this.labelTransTypes);
			this.groupFilters.Controls.Add(this.butCurrent);
			this.groupFilters.Controls.Add(this.butAll);
			this.groupFilters.Controls.Add(this.butFind);
			this.groupFilters.Controls.Add(this.textPatient);
			this.groupFilters.Controls.Add(this.labelPatient);
			this.groupFilters.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupFilters.Location = new System.Drawing.Point(12, 12);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(1206, 71);
			this.groupFilters.TabIndex = 0;
			this.groupFilters.TabStop = false;
			this.groupFilters.Text = "Message Filters";
			// 
			// comboClinics
			// 
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(605, 14);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.SelectionModeMulti = true;
			this.comboClinics.Size = new System.Drawing.Size(258, 21);
			this.comboClinics.TabIndex = 16;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.ComboClinics_SelectionChangeCommitted);
			// 
			// datePicker
			// 
			this.datePicker.BackColor = System.Drawing.Color.Transparent;
			this.datePicker.EnableWeekButtons = false;
			this.datePicker.IsVertical = true;
			this.datePicker.Location = new System.Drawing.Point(6, 14);
			this.datePicker.MinimumSize = new System.Drawing.Size(165, 46);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(173, 46);
			this.datePicker.TabIndex = 6;
			// 
			// comboClientIDs
			// 
			this.comboClientIDs.BackColor = System.Drawing.SystemColors.Window;
			this.comboClientIDs.Location = new System.Drawing.Point(642, 39);
			this.comboClientIDs.Name = "comboClientIDs";
			this.comboClientIDs.SelectionModeMulti = true;
			this.comboClientIDs.Size = new System.Drawing.Size(221, 21);
			this.comboClientIDs.TabIndex = 7;
			this.comboClientIDs.SelectionChangeCommitted += new System.EventHandler(this.comboBoxMultiClientIDs_SelectionChangeCommitted);
			// 
			// labelClientIDs
			// 
			this.labelClientIDs.Location = new System.Drawing.Point(556, 41);
			this.labelClientIDs.Name = "labelClientIDs";
			this.labelClientIDs.Size = new System.Drawing.Size(85, 17);
			this.labelClientIDs.TabIndex = 15;
			this.labelClientIDs.Text = "Client IDs";
			this.labelClientIDs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboAcctStatuses
			// 
			this.comboAcctStatuses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboAcctStatuses.BackColor = System.Drawing.SystemColors.Window;
			this.comboAcctStatuses.Location = new System.Drawing.Point(985, 14);
			this.comboAcctStatuses.Name = "comboAcctStatuses";
			this.comboAcctStatuses.SelectionModeMulti = true;
			this.comboAcctStatuses.Size = new System.Drawing.Size(215, 21);
			this.comboAcctStatuses.TabIndex = 8;
			this.comboAcctStatuses.SelectionChangeCommitted += new System.EventHandler(this.comboBoxMultiAcctStatuses_SelectionChangeCommitted);
			// 
			// labelAccountStatuses
			// 
			this.labelAccountStatuses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAccountStatuses.Location = new System.Drawing.Point(869, 16);
			this.labelAccountStatuses.Name = "labelAccountStatuses";
			this.labelAccountStatuses.Size = new System.Drawing.Size(115, 17);
			this.labelAccountStatuses.TabIndex = 13;
			this.labelAccountStatuses.Text = "Account Statuses";
			this.labelAccountStatuses.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTransTypes
			// 
			this.comboTransTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboTransTypes.BackColor = System.Drawing.SystemColors.Window;
			this.comboTransTypes.Location = new System.Drawing.Point(985, 39);
			this.comboTransTypes.Name = "comboTransTypes";
			this.comboTransTypes.SelectionModeMulti = true;
			this.comboTransTypes.Size = new System.Drawing.Size(215, 21);
			this.comboTransTypes.TabIndex = 9;
			this.comboTransTypes.SelectionChangeCommitted += new System.EventHandler(this.comboBoxMultiTransTypes_SelectionChangeCommitted);
			// 
			// labelTransTypes
			// 
			this.labelTransTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTransTypes.Location = new System.Drawing.Point(869, 41);
			this.labelTransTypes.Name = "labelTransTypes";
			this.labelTransTypes.Size = new System.Drawing.Size(115, 17);
			this.labelTransTypes.TabIndex = 11;
			this.labelTransTypes.Text = "Trans Types";
			this.labelTransTypes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCurrent
			// 
			this.butCurrent.Location = new System.Drawing.Point(251, 37);
			this.butCurrent.Name = "butCurrent";
			this.butCurrent.Size = new System.Drawing.Size(64, 24);
			this.butCurrent.TabIndex = 3;
			this.butCurrent.Text = "Current";
			this.butCurrent.Click += new System.EventHandler(this.butCurrent_Click);
			// 
			// butAll
			// 
			this.butAll.Location = new System.Drawing.Point(391, 37);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(64, 24);
			this.butAll.TabIndex = 5;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butFind
			// 
			this.butFind.Location = new System.Drawing.Point(321, 37);
			this.butFind.Name = "butFind";
			this.butFind.Size = new System.Drawing.Size(64, 24);
			this.butFind.TabIndex = 4;
			this.butFind.Text = "Find";
			this.butFind.Click += new System.EventHandler(this.butFind_Click);
			// 
			// textPatient
			// 
			this.textPatient.BackColor = System.Drawing.SystemColors.Window;
			this.textPatient.Location = new System.Drawing.Point(250, 14);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(300, 20);
			this.textPatient.TabIndex = 2;
			this.textPatient.TabStop = false;
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(185, 16);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(65, 17);
			this.labelPatient.TabIndex = 8;
			this.labelPatient.Text = "Patient";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRefresh.Location = new System.Drawing.Point(12, 662);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 4;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.contextMenu;
			this.gridMain.Location = new System.Drawing.Point(12, 202);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1206, 454);
			this.gridMain.TabIndex = 3;
			this.gridMain.Title = "Messages";
			this.gridMain.TranslationName = "TableTsiHistory";
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			this.gridMain.ColumnSorted += new System.EventHandler(this.gridMain_ColumnSorted);
			// 
			// groupRawMsg
			// 
			this.groupRawMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupRawMsg.Controls.Add(this.labelSelectedFieldDetails);
			this.groupRawMsg.Controls.Add(this.textSelectedFieldDetails);
			this.groupRawMsg.Controls.Add(this.labelSelectedFieldName);
			this.groupRawMsg.Controls.Add(this.textSelectedFieldName);
			this.groupRawMsg.Controls.Add(this.textRawMsg);
			this.groupRawMsg.Location = new System.Drawing.Point(12, 89);
			this.groupRawMsg.Name = "groupRawMsg";
			this.groupRawMsg.Size = new System.Drawing.Size(1206, 89);
			this.groupRawMsg.TabIndex = 1;
			this.groupRawMsg.TabStop = false;
			this.groupRawMsg.Text = "Raw Message";
			// 
			// labelSelectedFieldDetails
			// 
			this.labelSelectedFieldDetails.Location = new System.Drawing.Point(511, 14);
			this.labelSelectedFieldDetails.Name = "labelSelectedFieldDetails";
			this.labelSelectedFieldDetails.Size = new System.Drawing.Size(117, 17);
			this.labelSelectedFieldDetails.TabIndex = 11;
			this.labelSelectedFieldDetails.Text = "Selected Field Details";
			this.labelSelectedFieldDetails.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSelectedFieldDetails
			// 
			this.textSelectedFieldDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSelectedFieldDetails.BackColor = System.Drawing.SystemColors.Window;
			this.textSelectedFieldDetails.Location = new System.Drawing.Point(629, 12);
			this.textSelectedFieldDetails.Name = "textSelectedFieldDetails";
			this.textSelectedFieldDetails.ReadOnly = true;
			this.textSelectedFieldDetails.Size = new System.Drawing.Size(400, 20);
			this.textSelectedFieldDetails.TabIndex = 12;
			this.textSelectedFieldDetails.TabStop = false;
			// 
			// labelSelectedFieldName
			// 
			this.labelSelectedFieldName.Location = new System.Drawing.Point(178, 14);
			this.labelSelectedFieldName.Name = "labelSelectedFieldName";
			this.labelSelectedFieldName.Size = new System.Drawing.Size(116, 17);
			this.labelSelectedFieldName.TabIndex = 0;
			this.labelSelectedFieldName.Text = "Selected Field Name";
			this.labelSelectedFieldName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSelectedFieldName
			// 
			this.textSelectedFieldName.BackColor = System.Drawing.SystemColors.Window;
			this.textSelectedFieldName.Location = new System.Drawing.Point(295, 12);
			this.textSelectedFieldName.Name = "textSelectedFieldName";
			this.textSelectedFieldName.ReadOnly = true;
			this.textSelectedFieldName.Size = new System.Drawing.Size(210, 20);
			this.textSelectedFieldName.TabIndex = 4;
			this.textSelectedFieldName.TabStop = false;
			// 
			// textRawMsg
			// 
			this.textRawMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textRawMsg.BackColor = System.Drawing.SystemColors.Window;
			this.textRawMsg.Location = new System.Drawing.Point(6, 38);
			this.textRawMsg.Name = "textRawMsg";
			this.textRawMsg.ReadOnly = true;
			this.textRawMsg.Size = new System.Drawing.Size(1194, 45);
			this.textRawMsg.TabIndex = 0;
			this.textRawMsg.Text = "";
			this.textRawMsg.SelectionChanged += new System.EventHandler(this.textRawMsg_SelectionChanged);
			// 
			// FormTsiHistory
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.checkShowPatNums);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupRawMsg);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTsiHistory";
			this.Text = "TSI Message History";
			this.Load += new System.EventHandler(this.FormTsiHistory_Load);
			this.contextMenu.ResumeLayout(false);
			this.groupFilters.ResumeLayout(false);
			this.groupFilters.PerformLayout();
			this.groupRawMsg.ResumeLayout(false);
			this.groupRawMsg.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupFilters;
		private UI.Button butCurrent;
		private UI.Button butAll;
		private UI.Button butFind;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label labelPatient;
		private UI.Button butRefresh;
		private UI.ComboBoxOD comboTransTypes;
		private System.Windows.Forms.Label labelTransTypes;
		private UI.ComboBoxOD comboAcctStatuses;
		private System.Windows.Forms.Label labelAccountStatuses;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemGoTo;
		private System.Windows.Forms.RichTextBox textRawMsg;
		private System.Windows.Forms.TextBox textSelectedFieldName;
		private System.Windows.Forms.Label labelSelectedFieldName;
		private System.Windows.Forms.GroupBox groupRawMsg;
		private System.Windows.Forms.CheckBox checkShowPatNums;
		private System.Windows.Forms.Label labelSelectedFieldDetails;
		private System.Windows.Forms.TextBox textSelectedFieldDetails;
		private UI.ComboBoxOD comboClientIDs;
		private System.Windows.Forms.Label labelClientIDs;
		private UI.ODDateRangePicker datePicker;
		private UI.ComboBoxClinicPicker comboClinics;
	}
}