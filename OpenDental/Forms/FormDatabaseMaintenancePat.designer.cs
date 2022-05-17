namespace OpenDental {
	partial class FormDatabaseMaintenancePat {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDatabaseMaintenancePat));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.butPatientSelect = new OpenDental.UI.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.tabControlDBM = new System.Windows.Forms.TabControl();
			this.tabChecks = new System.Windows.Forms.TabPage();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.checkShow = new System.Windows.Forms.CheckBox();
			this.butNone = new OpenDental.UI.Button();
			this.butFix = new OpenDental.UI.Button();
			this.butCheck = new OpenDental.UI.Button();
			this.tabHidden = new System.Windows.Forms.TabPage();
			this.labelOld = new System.Windows.Forms.Label();
			this.gridHidden = new OpenDental.UI.GridOD();
			this.tabOld = new System.Windows.Forms.TabPage();
			this.label1 = new System.Windows.Forms.Label();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.butNoneOld = new OpenDental.UI.Button();
			this.butFixOld = new OpenDental.UI.Button();
			this.butCheckOld = new OpenDental.UI.Button();
			this.gridOld = new OpenDental.UI.GridOD();
			this.tabControlDBM.SuspendLayout();
			this.tabChecks.SuspendLayout();
			this.tabHidden.SuspendLayout();
			this.tabOld.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(6, 16);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(790, 365);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Patient Specific Database Checks";
			this.gridMain.TranslationName = "TableDatabaseMaintPat";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(747, 567);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 8;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(317, 25);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(200, 20);
			this.textPatient.TabIndex = 10;
			// 
			// butPatientSelect
			// 
			this.butPatientSelect.Location = new System.Drawing.Point(523, 25);
			this.butPatientSelect.Name = "butPatientSelect";
			this.butPatientSelect.Size = new System.Drawing.Size(26, 20);
			this.butPatientSelect.TabIndex = 11;
			this.butPatientSelect.Text = "...";
			this.butPatientSelect.Click += new System.EventHandler(this.butPatientSelect_Click);
			// 
			// textBox2
			// 
			this.textBox2.BackColor = System.Drawing.SystemColors.Control;
			this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox2.Location = new System.Drawing.Point(101, 28);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(210, 16);
			this.textBox2.TabIndex = 100;
			this.textBox2.TabStop = false;
			this.textBox2.Text = "Patient\r\n";
			this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// tabControlDBM
			// 
			this.tabControlDBM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlDBM.Controls.Add(this.tabChecks);
			this.tabControlDBM.Controls.Add(this.tabHidden);
			this.tabControlDBM.Controls.Add(this.tabOld);
			this.tabControlDBM.Location = new System.Drawing.Point(12, 51);
			this.tabControlDBM.Name = "tabControlDBM";
			this.tabControlDBM.SelectedIndex = 0;
			this.tabControlDBM.Size = new System.Drawing.Size(810, 494);
			this.tabControlDBM.TabIndex = 104;
			// 
			// tabChecks
			// 
			this.tabChecks.Controls.Add(this.textBox1);
			this.tabChecks.Controls.Add(this.checkShow);
			this.tabChecks.Controls.Add(this.butNone);
			this.tabChecks.Controls.Add(this.butFix);
			this.tabChecks.Controls.Add(this.butCheck);
			this.tabChecks.Controls.Add(this.gridMain);
			this.tabChecks.Location = new System.Drawing.Point(4, 22);
			this.tabChecks.Name = "tabChecks";
			this.tabChecks.Padding = new System.Windows.Forms.Padding(3);
			this.tabChecks.Size = new System.Drawing.Size(802, 468);
			this.tabChecks.TabIndex = 0;
			this.tabChecks.Text = "Checks";
			this.tabChecks.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.BackColor = System.Drawing.SystemColors.Window;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(350, 387);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(365, 26);
			this.textBox1.TabIndex = 108;
			this.textBox1.TabStop = false;
			this.textBox1.Text = "No selections will cause all database checks to run.\r\nOtherwise only selected che" +
    "cks will run.\r\n";
			this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkShow
			// 
			this.checkShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShow.Location = new System.Drawing.Point(6, 387);
			this.checkShow.Name = "checkShow";
			this.checkShow.Size = new System.Drawing.Size(447, 20);
			this.checkShow.TabIndex = 106;
			this.checkShow.Text = "Show me everything in the log  (only for advanced users)";
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNone.Location = new System.Drawing.Point(723, 387);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 26);
			this.butNone.TabIndex = 107;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// butFix
			// 
			this.butFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butFix.Location = new System.Drawing.Point(431, 430);
			this.butFix.Name = "butFix";
			this.butFix.Size = new System.Drawing.Size(75, 26);
			this.butFix.TabIndex = 105;
			this.butFix.Text = "&Fix";
			this.butFix.Click += new System.EventHandler(this.butFix_Click);
			// 
			// butCheck
			// 
			this.butCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCheck.Location = new System.Drawing.Point(306, 430);
			this.butCheck.Name = "butCheck";
			this.butCheck.Size = new System.Drawing.Size(75, 26);
			this.butCheck.TabIndex = 104;
			this.butCheck.Text = "C&heck";
			this.butCheck.Click += new System.EventHandler(this.butCheck_Click);
			// 
			// tabHidden
			// 
			this.tabHidden.Controls.Add(this.labelOld);
			this.tabHidden.Controls.Add(this.gridHidden);
			this.tabHidden.Location = new System.Drawing.Point(4, 22);
			this.tabHidden.Name = "tabHidden";
			this.tabHidden.Padding = new System.Windows.Forms.Padding(3);
			this.tabHidden.Size = new System.Drawing.Size(802, 468);
			this.tabHidden.TabIndex = 1;
			this.tabHidden.Text = "Hidden";
			this.tabHidden.UseVisualStyleBackColor = true;
			// 
			// labelOld
			// 
			this.labelOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelOld.Location = new System.Drawing.Point(7, 388);
			this.labelOld.Name = "labelOld";
			this.labelOld.Size = new System.Drawing.Size(772, 23);
			this.labelOld.TabIndex = 6;
			this.labelOld.Text = "This table shows all of the hidden database maintenance methods. Methods must be " +
    "unhidden from the regular DBM tool.";
			// 
			// gridHidden
			// 
			this.gridHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridHidden.HasMultilineHeaders = true;
			this.gridHidden.Location = new System.Drawing.Point(6, 16);
			this.gridHidden.Name = "gridHidden";
			this.gridHidden.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridHidden.Size = new System.Drawing.Size(790, 365);
			this.gridHidden.TabIndex = 5;
			this.gridHidden.Title = "Patient Specific Database Checks - Hidden";
			this.gridHidden.TranslationName = "TableDatabaseMaintPat";
			// 
			// tabOld
			// 
			this.tabOld.Controls.Add(this.label1);
			this.tabOld.Controls.Add(this.checkShowHidden);
			this.tabOld.Controls.Add(this.textBox4);
			this.tabOld.Controls.Add(this.butNoneOld);
			this.tabOld.Controls.Add(this.butFixOld);
			this.tabOld.Controls.Add(this.butCheckOld);
			this.tabOld.Controls.Add(this.gridOld);
			this.tabOld.Location = new System.Drawing.Point(4, 22);
			this.tabOld.Name = "tabOld";
			this.tabOld.Size = new System.Drawing.Size(802, 468);
			this.tabOld.TabIndex = 2;
			this.tabOld.Text = "Old";
			this.tabOld.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(3, 384);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(449, 43);
			this.label1.TabIndex = 115;
			this.label1.Text = "This table shows database maintenance methods that have been deemed no longer nec" +
    "essary. Should not be run unless directly told to do so.";
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkShowHidden.Location = new System.Drawing.Point(6, 439);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(134, 17);
			this.checkShowHidden.TabIndex = 114;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.UseVisualStyleBackColor = true;
			this.checkShowHidden.CheckedChanged += new System.EventHandler(this.checkShowHidden_CheckedChanged);
			// 
			// textBox4
			// 
			this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox4.BackColor = System.Drawing.SystemColors.Window;
			this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox4.Location = new System.Drawing.Point(350, 387);
			this.textBox4.Multiline = true;
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(365, 26);
			this.textBox4.TabIndex = 113;
			this.textBox4.TabStop = false;
			this.textBox4.Text = "No selections will cause all database checks to run.\r\nOtherwise only selected che" +
    "cks will run.\r\n";
			this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butNoneOld
			// 
			this.butNoneOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNoneOld.Location = new System.Drawing.Point(723, 387);
			this.butNoneOld.Name = "butNoneOld";
			this.butNoneOld.Size = new System.Drawing.Size(75, 26);
			this.butNoneOld.TabIndex = 112;
			this.butNoneOld.Text = "None";
			this.butNoneOld.Click += new System.EventHandler(this.butNoneOld_Click);
			// 
			// butFixOld
			// 
			this.butFixOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butFixOld.Location = new System.Drawing.Point(431, 430);
			this.butFixOld.Name = "butFixOld";
			this.butFixOld.Size = new System.Drawing.Size(75, 26);
			this.butFixOld.TabIndex = 110;
			this.butFixOld.Text = "&Fix";
			this.butFixOld.Click += new System.EventHandler(this.butFixOld_Click);
			// 
			// butCheckOld
			// 
			this.butCheckOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCheckOld.Location = new System.Drawing.Point(306, 430);
			this.butCheckOld.Name = "butCheckOld";
			this.butCheckOld.Size = new System.Drawing.Size(75, 26);
			this.butCheckOld.TabIndex = 109;
			this.butCheckOld.Text = "C&heck";
			this.butCheckOld.Click += new System.EventHandler(this.butCheckOld_Click);
			// 
			// gridOld
			// 
			this.gridOld.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridOld.HasMultilineHeaders = true;
			this.gridOld.Location = new System.Drawing.Point(6, 16);
			this.gridOld.Name = "gridOld";
			this.gridOld.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridOld.Size = new System.Drawing.Size(790, 365);
			this.gridOld.TabIndex = 6;
			this.gridOld.Title = "Patient Specific Database Checks - Old";
			this.gridOld.TranslationName = "TableDatabaseMaintPat";
			this.gridOld.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridOld_CellDoubleClick);
			// 
			// FormDatabaseMaintenancePat
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(834, 605);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.tabControlDBM);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.butPatientSelect);
			this.Controls.Add(this.textPatient);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDatabaseMaintenancePat";
			this.Text = "Database Maintenance for Patient";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDatabaseMaintenancePat_FormClosing);
			this.Load += new System.EventHandler(this.FormDatabaseMaintenancePat_Load);
			this.tabControlDBM.ResumeLayout(false);
			this.tabChecks.ResumeLayout(false);
			this.tabChecks.PerformLayout();
			this.tabHidden.ResumeLayout(false);
			this.tabOld.ResumeLayout(false);
			this.tabOld.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.GridOD gridMain;
		private UI.Button butClose;
		private System.Windows.Forms.TextBox textPatient;
		private UI.Button butPatientSelect;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TabControl tabControlDBM;
		private System.Windows.Forms.TabPage tabChecks;
		private System.Windows.Forms.TabPage tabHidden;
		private System.Windows.Forms.TabPage tabOld;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.CheckBox checkShow;
		private UI.Button butNone;
		private UI.Button butFix;
		private UI.Button butCheck;
		private UI.GridOD gridHidden;
		private System.Windows.Forms.TextBox textBox4;
		private UI.Button butNoneOld;
		private UI.Button butFixOld;
		private UI.Button butCheckOld;
		private UI.GridOD gridOld;
		private System.Windows.Forms.Label labelOld;
		private System.Windows.Forms.CheckBox checkShowHidden;
		private System.Windows.Forms.Label label1;
	}
}