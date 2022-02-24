namespace OpenDental{
	partial class FormEServicesTexting {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesTexting));
			this.label37 = new System.Windows.Forms.Label();
			this.menuWebSchedVerifyTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.insertReplacementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupShortCode = new System.Windows.Forms.GroupBox();
			this.labelUnsavedShortCodeChanges = new System.Windows.Forms.Label();
			this.butSaveShortCodes = new OpenDental.UI.Button();
			this.labelShortCodeOptInClinicTitle = new System.Windows.Forms.Label();
			this.textShortCodeOptInClinicTitle = new System.Windows.Forms.TextBox();
			this.checkOptInPrompt = new System.Windows.Forms.CheckBox();
			this.comboShortCodeClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.butDefaultClinicClear = new OpenDental.UI.Button();
			this.butDefaultClinic = new OpenDental.UI.Button();
			this.butBackMonth = new OpenDental.UI.Button();
			this.dateTimePickerSms = new System.Windows.Forms.DateTimePicker();
			this.gridSmsSummary = new OpenDental.UI.GridOD();
			this.butFwdMonth = new OpenDental.UI.Button();
			this.butThisMonth = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			this.groupShortCode.SuspendLayout();
			this.SuspendLayout();
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(0, 0);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(100, 23);
			this.label37.TabIndex = 0;
			// 
			// menuWebSchedVerifyTextTemplate
			// 
			this.menuWebSchedVerifyTextTemplate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertReplacementsToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.selectAllToolStripMenuItem});
			this.menuWebSchedVerifyTextTemplate.Name = "menuASAPEmailBody";
			this.menuWebSchedVerifyTextTemplate.Size = new System.Drawing.Size(137, 136);
			this.menuWebSchedVerifyTextTemplate.Text = "Insert Replacements";
			// 
			// insertReplacementsToolStripMenuItem
			// 
			this.insertReplacementsToolStripMenuItem.Name = "insertReplacementsToolStripMenuItem";
			this.insertReplacementsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.insertReplacementsToolStripMenuItem.Text = "Insert Fields";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.selectAllToolStripMenuItem.Text = "Select All";
			// 
			// groupShortCode
			// 
			this.groupShortCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupShortCode.Controls.Add(this.labelUnsavedShortCodeChanges);
			this.groupShortCode.Controls.Add(this.butSaveShortCodes);
			this.groupShortCode.Controls.Add(this.labelShortCodeOptInClinicTitle);
			this.groupShortCode.Controls.Add(this.textShortCodeOptInClinicTitle);
			this.groupShortCode.Controls.Add(this.checkOptInPrompt);
			this.groupShortCode.Controls.Add(this.comboShortCodeClinic);
			this.groupShortCode.Location = new System.Drawing.Point(15, 512);
			this.groupShortCode.Name = "groupShortCode";
			this.groupShortCode.Size = new System.Drawing.Size(375, 85);
			this.groupShortCode.TabIndex = 270;
			this.groupShortCode.TabStop = false;
			this.groupShortCode.Text = "OptIn Preferences";
			// 
			// labelUnsavedShortCodeChanges
			// 
			this.labelUnsavedShortCodeChanges.ForeColor = System.Drawing.Color.Red;
			this.labelUnsavedShortCodeChanges.Location = new System.Drawing.Point(188, 59);
			this.labelUnsavedShortCodeChanges.Name = "labelUnsavedShortCodeChanges";
			this.labelUnsavedShortCodeChanges.Size = new System.Drawing.Size(100, 19);
			this.labelUnsavedShortCodeChanges.TabIndex = 272;
			this.labelUnsavedShortCodeChanges.Text = "unsaved changes*";
			this.labelUnsavedShortCodeChanges.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butSaveShortCodes
			// 
			this.butSaveShortCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSaveShortCodes.Location = new System.Drawing.Point(291, 58);
			this.butSaveShortCodes.Name = "butSaveShortCodes";
			this.butSaveShortCodes.Size = new System.Drawing.Size(81, 23);
			this.butSaveShortCodes.TabIndex = 271;
			this.butSaveShortCodes.Text = "Save";
			this.butSaveShortCodes.UseVisualStyleBackColor = true;
			this.butSaveShortCodes.Click += new System.EventHandler(this.butSaveShortCodes_Click);
			// 
			// labelShortCodeOptInClinicTitle
			// 
			this.labelShortCodeOptInClinicTitle.Location = new System.Drawing.Point(74, 37);
			this.labelShortCodeOptInClinicTitle.Name = "labelShortCodeOptInClinicTitle";
			this.labelShortCodeOptInClinicTitle.Size = new System.Drawing.Size(100, 18);
			this.labelShortCodeOptInClinicTitle.TabIndex = 3;
			this.labelShortCodeOptInClinicTitle.Text = "OptIn Office Title";
			this.labelShortCodeOptInClinicTitle.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textShortCodeOptInClinicTitle
			// 
			this.textShortCodeOptInClinicTitle.Location = new System.Drawing.Point(174, 35);
			this.textShortCodeOptInClinicTitle.Name = "textShortCodeOptInClinicTitle";
			this.textShortCodeOptInClinicTitle.Size = new System.Drawing.Size(198, 20);
			this.textShortCodeOptInClinicTitle.TabIndex = 2;
			this.textShortCodeOptInClinicTitle.TextChanged += new System.EventHandler(this.textShortCodeOptInClinicTitle_TextChanged);
			// 
			// checkOptInPrompt
			// 
			this.checkOptInPrompt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOptInPrompt.Location = new System.Drawing.Point(1, 55);
			this.checkOptInPrompt.Name = "checkOptInPrompt";
			this.checkOptInPrompt.Size = new System.Drawing.Size(187, 24);
			this.checkOptInPrompt.TabIndex = 1;
			this.checkOptInPrompt.Text = "Prompt for OptIn";
			this.checkOptInPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOptInPrompt.UseVisualStyleBackColor = true;
			this.checkOptInPrompt.Click += new System.EventHandler(this.checkOptInPrompt_Click);
			// 
			// comboShortCodeClinic
			// 
			this.comboShortCodeClinic.HqDescription = "Default";
			this.comboShortCodeClinic.IncludeUnassigned = true;
			this.comboShortCodeClinic.Location = new System.Drawing.Point(137, 12);
			this.comboShortCodeClinic.Name = "comboShortCodeClinic";
			this.comboShortCodeClinic.SelectionModeMulti = true;
			this.comboShortCodeClinic.Size = new System.Drawing.Size(235, 21);
			this.comboShortCodeClinic.TabIndex = 0;
			this.comboShortCodeClinic.SelectionChangeCommitted += new System.EventHandler(this.comboShortCodeClinic_SelectionChangeCommitted);
			// 
			// butDefaultClinicClear
			// 
			this.butDefaultClinicClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDefaultClinicClear.Location = new System.Drawing.Point(992, 518);
			this.butDefaultClinicClear.Name = "butDefaultClinicClear";
			this.butDefaultClinicClear.Size = new System.Drawing.Size(81, 23);
			this.butDefaultClinicClear.TabIndex = 269;
			this.butDefaultClinicClear.Text = "Clear Default";
			this.butDefaultClinicClear.UseVisualStyleBackColor = true;
			this.butDefaultClinicClear.Click += new System.EventHandler(this.butDefaultClinicClear_Click);
			// 
			// butDefaultClinic
			// 
			this.butDefaultClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDefaultClinic.Location = new System.Drawing.Point(1079, 518);
			this.butDefaultClinic.Name = "butDefaultClinic";
			this.butDefaultClinic.Size = new System.Drawing.Size(81, 23);
			this.butDefaultClinic.TabIndex = 262;
			this.butDefaultClinic.Text = "Set Default";
			this.butDefaultClinic.UseVisualStyleBackColor = true;
			this.butDefaultClinic.Click += new System.EventHandler(this.butDefaultClinic_Click);
			// 
			// butBackMonth
			// 
			this.butBackMonth.AdjustImageLocation = new System.Drawing.Point(-3, -1);
			this.butBackMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butBackMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butBackMonth.Image = ((System.Drawing.Image)(resources.GetObject("butBackMonth.Image")));
			this.butBackMonth.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBackMonth.Location = new System.Drawing.Point(502, 521);
			this.butBackMonth.Name = "butBackMonth";
			this.butBackMonth.Size = new System.Drawing.Size(32, 22);
			this.butBackMonth.TabIndex = 268;
			this.butBackMonth.Text = "M";
			this.butBackMonth.Click += new System.EventHandler(this.butBackMonth_Click);
			// 
			// dateTimePickerSms
			// 
			this.dateTimePickerSms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.dateTimePickerSms.CustomFormat = "MMM yyyy";
			this.dateTimePickerSms.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePickerSms.Location = new System.Drawing.Point(534, 521);
			this.dateTimePickerSms.Name = "dateTimePickerSms";
			this.dateTimePickerSms.Size = new System.Drawing.Size(113, 20);
			this.dateTimePickerSms.TabIndex = 258;
			this.dateTimePickerSms.ValueChanged += new System.EventHandler(this.dateTimePickerSms_ValueChanged);
			// 
			// gridSmsSummary
			// 
			this.gridSmsSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridSmsSummary.HasMultilineHeaders = true;
			this.gridSmsSummary.Location = new System.Drawing.Point(12, 12);
			this.gridSmsSummary.Name = "gridSmsSummary";
			this.gridSmsSummary.Size = new System.Drawing.Size(1150, 500);
			this.gridSmsSummary.TabIndex = 252;
			this.gridSmsSummary.Title = "Text Messaging Phone Number and Usage Summary";
			this.gridSmsSummary.TranslationName = "FormEServicesSetup";
			this.gridSmsSummary.WrapText = false;
			// 
			// butFwdMonth
			// 
			this.butFwdMonth.AdjustImageLocation = new System.Drawing.Point(5, -1);
			this.butFwdMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butFwdMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butFwdMonth.Image = ((System.Drawing.Image)(resources.GetObject("butFwdMonth.Image")));
			this.butFwdMonth.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butFwdMonth.Location = new System.Drawing.Point(647, 521);
			this.butFwdMonth.Name = "butFwdMonth";
			this.butFwdMonth.Size = new System.Drawing.Size(29, 22);
			this.butFwdMonth.TabIndex = 267;
			this.butFwdMonth.Text = "M";
			this.butFwdMonth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butFwdMonth.Click += new System.EventHandler(this.butFwdMonth_Click);
			// 
			// butThisMonth
			// 
			this.butThisMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butThisMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butThisMonth.Location = new System.Drawing.Point(553, 546);
			this.butThisMonth.Name = "butThisMonth";
			this.butThisMonth.Size = new System.Drawing.Size(75, 22);
			this.butThisMonth.TabIndex = 262;
			this.butThisMonth.Text = "This Month";
			this.butThisMonth.Click += new System.EventHandler(this.butThisMonth_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1085, 602);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 501;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormEServicesTexting
			// 
			this.ClientSize = new System.Drawing.Size(1176, 638);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupShortCode);
			this.Controls.Add(this.butDefaultClinicClear);
			this.Controls.Add(this.butDefaultClinic);
			this.Controls.Add(this.butBackMonth);
			this.Controls.Add(this.dateTimePickerSms);
			this.Controls.Add(this.gridSmsSummary);
			this.Controls.Add(this.butThisMonth);
			this.Controls.Add(this.butFwdMonth);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesTexting";
			this.Text = "eServices Texting";
			this.Load += new System.EventHandler(this.FormEServicesTexting_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.groupShortCode.ResumeLayout(false);
			this.groupShortCode.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridSmsSummary;
		private System.Windows.Forms.DateTimePicker dateTimePickerSms;
		private UI.Button butBackMonth;
		private UI.Button butFwdMonth;
		private UI.Button butThisMonth;
		private System.Windows.Forms.Label label37;
		private UI.Button butDefaultClinic;
		private UI.Button butDefaultClinicClear;
		private System.Windows.Forms.ContextMenuStrip menuWebSchedVerifyTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem insertReplacementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupShortCode;
		private System.Windows.Forms.CheckBox checkOptInPrompt;
		private UI.ComboBoxClinicPicker comboShortCodeClinic;
		private System.Windows.Forms.Label labelShortCodeOptInClinicTitle;
		private System.Windows.Forms.TextBox textShortCodeOptInClinicTitle;
		private UI.Button butSaveShortCodes;
		private System.Windows.Forms.Label labelUnsavedShortCodeChanges;
		private UI.Button butClose;
	}
}