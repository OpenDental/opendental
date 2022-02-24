namespace OpenDental{
	partial class FormEServicesMisc {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesMisc));
			this.label37 = new System.Windows.Forms.Label();
			this.menuWebSchedVerifyTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.insertReplacementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.butClose = new OpenDental.UI.Button();
			this.groupDateFormat = new System.Windows.Forms.GroupBox();
			this.label30 = new System.Windows.Forms.Label();
			this.labelDateCustom = new System.Windows.Forms.Label();
			this.textDateCustom = new System.Windows.Forms.TextBox();
			this.label34 = new System.Windows.Forms.Label();
			this.radioDateCustom = new System.Windows.Forms.RadioButton();
			this.radioDateMMMMdyyyy = new System.Windows.Forms.RadioButton();
			this.radioDatem = new System.Windows.Forms.RadioButton();
			this.radioDateLongDate = new System.Windows.Forms.RadioButton();
			this.radioDateShortDate = new System.Windows.Forms.RadioButton();
			this.groupNotUsed = new System.Windows.Forms.GroupBox();
			this.butShowOldMobileSych = new OpenDental.UI.Button();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.dateRunEnd = new System.Windows.Forms.DateTimePicker();
			this.label46 = new System.Windows.Forms.Label();
			this.dateRunStart = new System.Windows.Forms.DateTimePicker();
			this.label47 = new System.Windows.Forms.Label();
			this.label48 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			this.groupDateFormat.SuspendLayout();
			this.groupNotUsed.SuspendLayout();
			this.groupBox8.SuspendLayout();
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
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(708, 336);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 500;
			this.butClose.Text = "OK";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupDateFormat
			// 
			this.groupDateFormat.Controls.Add(this.label30);
			this.groupDateFormat.Controls.Add(this.labelDateCustom);
			this.groupDateFormat.Controls.Add(this.textDateCustom);
			this.groupDateFormat.Controls.Add(this.label34);
			this.groupDateFormat.Controls.Add(this.radioDateCustom);
			this.groupDateFormat.Controls.Add(this.radioDateMMMMdyyyy);
			this.groupDateFormat.Controls.Add(this.radioDatem);
			this.groupDateFormat.Controls.Add(this.radioDateLongDate);
			this.groupDateFormat.Controls.Add(this.radioDateShortDate);
			this.groupDateFormat.Location = new System.Drawing.Point(12, 99);
			this.groupDateFormat.Name = "groupDateFormat";
			this.groupDateFormat.Size = new System.Drawing.Size(851, 160);
			this.groupDateFormat.TabIndex = 251;
			this.groupDateFormat.TabStop = false;
			this.groupDateFormat.Text = "Date Format";
			// 
			// label30
			// 
			this.label30.Location = new System.Drawing.Point(6, 16);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(838, 13);
			this.label30.TabIndex = 315;
			this.label30.Text = "This date format will be applied to eReminders, eConfirmations, manual confirmati" +
    "ons, ASAP List texts, and other forms of patient communication.";
			// 
			// labelDateCustom
			// 
			this.labelDateCustom.Location = new System.Drawing.Point(217, 114);
			this.labelDateCustom.Name = "labelDateCustom";
			this.labelDateCustom.Size = new System.Drawing.Size(225, 20);
			this.labelDateCustom.TabIndex = 314;
			this.labelDateCustom.Text = "labelDateCustom";
			this.labelDateCustom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textDateCustom
			// 
			this.textDateCustom.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textDateCustom.Location = new System.Drawing.Point(86, 114);
			this.textDateCustom.Multiline = true;
			this.textDateCustom.Name = "textDateCustom";
			this.textDateCustom.Size = new System.Drawing.Size(123, 20);
			this.textDateCustom.TabIndex = 313;
			this.textDateCustom.TextChanged += new System.EventHandler(this.textDateCustom_TextChanged);
			// 
			// label34
			// 
			this.label34.Location = new System.Drawing.Point(89, 138);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(755, 20);
			this.label34.TabIndex = 74;
			this.label34.Text = "E.g. of custom date format is MMMM d, yyyy";
			// 
			// radioDateCustom
			// 
			this.radioDateCustom.Location = new System.Drawing.Point(15, 114);
			this.radioDateCustom.Name = "radioDateCustom";
			this.radioDateCustom.Size = new System.Drawing.Size(70, 20);
			this.radioDateCustom.TabIndex = 82;
			this.radioDateCustom.Text = "Custom:";
			this.radioDateCustom.UseVisualStyleBackColor = true;
			// 
			// radioDateMMMMdyyyy
			// 
			this.radioDateMMMMdyyyy.Location = new System.Drawing.Point(15, 74);
			this.radioDateMMMMdyyyy.Name = "radioDateMMMMdyyyy";
			this.radioDateMMMMdyyyy.Size = new System.Drawing.Size(438, 20);
			this.radioDateMMMMdyyyy.TabIndex = 81;
			this.radioDateMMMMdyyyy.Text = "March 15, 2018";
			this.radioDateMMMMdyyyy.UseVisualStyleBackColor = true;
			// 
			// radioDatem
			// 
			this.radioDatem.Location = new System.Drawing.Point(15, 94);
			this.radioDatem.Name = "radioDatem";
			this.radioDatem.Size = new System.Drawing.Size(438, 20);
			this.radioDatem.TabIndex = 80;
			this.radioDatem.Text = "March 15";
			this.radioDatem.UseVisualStyleBackColor = true;
			// 
			// radioDateLongDate
			// 
			this.radioDateLongDate.Location = new System.Drawing.Point(15, 54);
			this.radioDateLongDate.Name = "radioDateLongDate";
			this.radioDateLongDate.Size = new System.Drawing.Size(438, 20);
			this.radioDateLongDate.TabIndex = 79;
			this.radioDateLongDate.Text = "Thursday March 15, 2018";
			this.radioDateLongDate.UseVisualStyleBackColor = true;
			this.radioDateLongDate.Click += new System.EventHandler(this.radioDateFormat_Click);
			// 
			// radioDateShortDate
			// 
			this.radioDateShortDate.Location = new System.Drawing.Point(15, 34);
			this.radioDateShortDate.Name = "radioDateShortDate";
			this.radioDateShortDate.Size = new System.Drawing.Size(438, 20);
			this.radioDateShortDate.TabIndex = 78;
			this.radioDateShortDate.Text = "3/15/2018";
			this.radioDateShortDate.UseVisualStyleBackColor = true;
			this.radioDateShortDate.Click += new System.EventHandler(this.radioDateFormat_Click);
			// 
			// groupNotUsed
			// 
			this.groupNotUsed.Controls.Add(this.butShowOldMobileSych);
			this.groupNotUsed.Location = new System.Drawing.Point(12, 265);
			this.groupNotUsed.Name = "groupNotUsed";
			this.groupNotUsed.Size = new System.Drawing.Size(851, 58);
			this.groupNotUsed.TabIndex = 250;
			this.groupNotUsed.TabStop = false;
			this.groupNotUsed.Text = "No Longer Used";
			this.groupNotUsed.Visible = false;
			// 
			// butShowOldMobileSych
			// 
			this.butShowOldMobileSych.Location = new System.Drawing.Point(9, 21);
			this.butShowOldMobileSych.Name = "butShowOldMobileSych";
			this.butShowOldMobileSych.Size = new System.Drawing.Size(225, 24);
			this.butShowOldMobileSych.TabIndex = 249;
			this.butShowOldMobileSych.Text = "Show Mobile Synch (old-style)";
			this.butShowOldMobileSych.Visible = false;
			this.butShowOldMobileSych.Click += new System.EventHandler(this.butShowOldMobileSych_Click);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.dateRunEnd);
			this.groupBox8.Controls.Add(this.label46);
			this.groupBox8.Controls.Add(this.dateRunStart);
			this.groupBox8.Controls.Add(this.label47);
			this.groupBox8.Controls.Add(this.label48);
			this.groupBox8.Location = new System.Drawing.Point(12, 12);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(851, 81);
			this.groupBox8.TabIndex = 76;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Automated eServices Schedule";
			// 
			// dateRunEnd
			// 
			this.dateRunEnd.CustomFormat = " ";
			this.dateRunEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dateRunEnd.Location = new System.Drawing.Point(144, 56);
			this.dateRunEnd.Name = "dateRunEnd";
			this.dateRunEnd.ShowUpDown = true;
			this.dateRunEnd.Size = new System.Drawing.Size(90, 20);
			this.dateRunEnd.TabIndex = 7;
			this.dateRunEnd.Value = new System.DateTime(2015, 11, 3, 22, 0, 0, 0);
			// 
			// label46
			// 
			this.label46.Location = new System.Drawing.Point(6, 16);
			this.label46.Name = "label46";
			this.label46.Size = new System.Drawing.Size(838, 13);
			this.label46.TabIndex = 72;
			this.label46.Text = "This applies to eConfirmations, eReminders, and WebSched recall notifications. It" +
    " dictates the time interval that the service will automatically notify patients." +
    "";
			// 
			// dateRunStart
			// 
			this.dateRunStart.CustomFormat = " ";
			this.dateRunStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dateRunStart.Location = new System.Drawing.Point(144, 36);
			this.dateRunStart.Name = "dateRunStart";
			this.dateRunStart.ShowUpDown = true;
			this.dateRunStart.Size = new System.Drawing.Size(90, 20);
			this.dateRunStart.TabIndex = 6;
			this.dateRunStart.Value = new System.DateTime(2015, 11, 3, 7, 0, 0, 0);
			// 
			// label47
			// 
			this.label47.Location = new System.Drawing.Point(12, 58);
			this.label47.Name = "label47";
			this.label47.Size = new System.Drawing.Size(126, 17);
			this.label47.TabIndex = 5;
			this.label47.Text = "End Time";
			this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label48
			// 
			this.label48.Location = new System.Drawing.Point(12, 38);
			this.label48.Name = "label48";
			this.label48.Size = new System.Drawing.Size(126, 17);
			this.label48.TabIndex = 4;
			this.label48.Text = "Start Time";
			this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(789, 335);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 501;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEServicesMisc
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(876, 371);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupDateFormat);
			this.Controls.Add(this.groupNotUsed);
			this.Controls.Add(this.groupBox8);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(0, 0);
			this.Name = "FormEServicesMisc";
			this.Text = "eServices Misc";
			this.Load += new System.EventHandler(this.FormEServicesMisc_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.groupDateFormat.ResumeLayout(false);
			this.groupDateFormat.PerformLayout();
			this.groupNotUsed.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butClose;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.Label label46;
		private System.Windows.Forms.DateTimePicker dateRunEnd;
		private System.Windows.Forms.DateTimePicker dateRunStart;
		private System.Windows.Forms.Label label47;
		private System.Windows.Forms.Label label48;
		private System.Windows.Forms.GroupBox groupNotUsed;
		private UI.Button butShowOldMobileSych;
		private System.Windows.Forms.ContextMenuStrip menuWebSchedVerifyTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem insertReplacementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.GroupBox groupDateFormat;
		private System.Windows.Forms.RadioButton radioDateCustom;
		private System.Windows.Forms.RadioButton radioDateMMMMdyyyy;
		private System.Windows.Forms.RadioButton radioDatem;
		private System.Windows.Forms.RadioButton radioDateLongDate;
		private System.Windows.Forms.RadioButton radioDateShortDate;
		private System.Windows.Forms.TextBox textDateCustom;
		private System.Windows.Forms.Label labelDateCustom;
		private System.Windows.Forms.Label label30;
		private UI.Button butCancel;
	}
}