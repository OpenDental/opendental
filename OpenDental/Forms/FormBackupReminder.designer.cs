namespace OpenDental{
	partial class FormBackupReminder {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBackupReminder));
			this.butOK = new OpenDental.UI.Button();
			this.labelDescription = new System.Windows.Forms.Label();
			this.checkNoBackups = new System.Windows.Forms.CheckBox();
			this.checkA1 = new System.Windows.Forms.CheckBox();
			this.labelBackupMethod = new System.Windows.Forms.Label();
			this.checkA2 = new System.Windows.Forms.CheckBox();
			this.checkA4 = new System.Windows.Forms.CheckBox();
			this.checkA3 = new System.Windows.Forms.CheckBox();
			this.checkB2 = new System.Windows.Forms.CheckBox();
			this.checkB1 = new System.Windows.Forms.CheckBox();
			this.labelProof = new System.Windows.Forms.Label();
			this.checkNoProof = new System.Windows.Forms.CheckBox();
			this.checkNoStrategy = new System.Windows.Forms.CheckBox();
			this.checkC2 = new System.Windows.Forms.CheckBox();
			this.checkC1 = new System.Windows.Forms.CheckBox();
			this.labelStrategy = new System.Windows.Forms.Label();
			this.labelSupplementalBackupDisabled = new System.Windows.Forms.Label();
			this.labelSupplementalBackupPath = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(525, 451);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(12, 9);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(552, 74);
			this.labelDescription.TabIndex = 4;
			this.labelDescription.Text = resources.GetString("labelDescription.Text");
			// 
			// checkNoBackups
			// 
			this.checkNoBackups.Location = new System.Drawing.Point(45, 191);
			this.checkNoBackups.Name = "checkNoBackups";
			this.checkNoBackups.Size = new System.Drawing.Size(151, 20);
			this.checkNoBackups.TabIndex = 6;
			this.checkNoBackups.Text = "No backups";
			this.checkNoBackups.UseVisualStyleBackColor = true;
			// 
			// checkA1
			// 
			this.checkA1.Location = new System.Drawing.Point(45, 111);
			this.checkA1.Name = "checkA1";
			this.checkA1.Size = new System.Drawing.Size(200, 20);
			this.checkA1.TabIndex = 8;
			this.checkA1.Text = "Online";
			this.checkA1.UseVisualStyleBackColor = true;
			// 
			// labelBackupMethod
			// 
			this.labelBackupMethod.Location = new System.Drawing.Point(42, 95);
			this.labelBackupMethod.Name = "labelBackupMethod";
			this.labelBackupMethod.Size = new System.Drawing.Size(523, 18);
			this.labelBackupMethod.TabIndex = 7;
			this.labelBackupMethod.Text = "Do you make backups every single day?  Backup method:";
			// 
			// checkA2
			// 
			this.checkA2.Location = new System.Drawing.Point(45, 131);
			this.checkA2.Name = "checkA2";
			this.checkA2.Size = new System.Drawing.Size(530, 20);
			this.checkA2.TabIndex = 9;
			this.checkA2.Text = "Removable (external HD, USB drive, etc)";
			this.checkA2.UseVisualStyleBackColor = true;
			// 
			// checkA4
			// 
			this.checkA4.Location = new System.Drawing.Point(45, 171);
			this.checkA4.Name = "checkA4";
			this.checkA4.Size = new System.Drawing.Size(151, 20);
			this.checkA4.TabIndex = 11;
			this.checkA4.Text = "Other backup method";
			this.checkA4.UseVisualStyleBackColor = true;
			// 
			// checkA3
			// 
			this.checkA3.Location = new System.Drawing.Point(45, 151);
			this.checkA3.Name = "checkA3";
			this.checkA3.Size = new System.Drawing.Size(302, 20);
			this.checkA3.TabIndex = 10;
			this.checkA3.Text = "Network (to another computer in your office)";
			this.checkA3.UseVisualStyleBackColor = true;
			// 
			// checkB2
			// 
			this.checkB2.Location = new System.Drawing.Point(45, 261);
			this.checkB2.Name = "checkB2";
			this.checkB2.Size = new System.Drawing.Size(250, 20);
			this.checkB2.TabIndex = 14;
			this.checkB2.Text = "Run backup from a second server";
			this.checkB2.UseVisualStyleBackColor = true;
			// 
			// checkB1
			// 
			this.checkB1.Location = new System.Drawing.Point(45, 241);
			this.checkB1.Name = "checkB1";
			this.checkB1.Size = new System.Drawing.Size(352, 20);
			this.checkB1.TabIndex = 13;
			this.checkB1.Text = "Restore to home computer at least once a week";
			this.checkB1.UseVisualStyleBackColor = true;
			// 
			// labelProof
			// 
			this.labelProof.Location = new System.Drawing.Point(42, 225);
			this.labelProof.Name = "labelProof";
			this.labelProof.Size = new System.Drawing.Size(523, 18);
			this.labelProof.TabIndex = 12;
			this.labelProof.Text = "What proof do you have that your recent backups are good?";
			// 
			// checkNoProof
			// 
			this.checkNoProof.Location = new System.Drawing.Point(45, 281);
			this.checkNoProof.Name = "checkNoProof";
			this.checkNoProof.Size = new System.Drawing.Size(250, 20);
			this.checkNoProof.TabIndex = 15;
			this.checkNoProof.Text = "No proof";
			this.checkNoProof.UseVisualStyleBackColor = true;
			// 
			// checkNoStrategy
			// 
			this.checkNoStrategy.Location = new System.Drawing.Point(45, 369);
			this.checkNoStrategy.Name = "checkNoStrategy";
			this.checkNoStrategy.Size = new System.Drawing.Size(250, 20);
			this.checkNoStrategy.TabIndex = 19;
			this.checkNoStrategy.Text = "No strategy";
			this.checkNoStrategy.UseVisualStyleBackColor = true;
			// 
			// checkC2
			// 
			this.checkC2.Location = new System.Drawing.Point(45, 349);
			this.checkC2.Name = "checkC2";
			this.checkC2.Size = new System.Drawing.Size(312, 20);
			this.checkC2.TabIndex = 18;
			this.checkC2.Text = "Saved hardcopy paper reports";
			this.checkC2.UseVisualStyleBackColor = true;
			// 
			// checkC1
			// 
			this.checkC1.Location = new System.Drawing.Point(45, 329);
			this.checkC1.Name = "checkC1";
			this.checkC1.Size = new System.Drawing.Size(352, 20);
			this.checkC1.TabIndex = 17;
			this.checkC1.Text = "Completely separate archives stored offsite (DVD, HD, etc)";
			this.checkC1.UseVisualStyleBackColor = true;
			// 
			// labelStrategy
			// 
			this.labelStrategy.Location = new System.Drawing.Point(42, 313);
			this.labelStrategy.Name = "labelStrategy";
			this.labelStrategy.Size = new System.Drawing.Size(523, 18);
			this.labelStrategy.TabIndex = 16;
			this.labelStrategy.Text = "What secondary long-term mechanism do you use to ensure minimal data loss?";
			// 
			// labelSupplementalBackupDisabled
			// 
			this.labelSupplementalBackupDisabled.ForeColor = System.Drawing.Color.Red;
			this.labelSupplementalBackupDisabled.Location = new System.Drawing.Point(42, 392);
			this.labelSupplementalBackupDisabled.Name = "labelSupplementalBackupDisabled";
			this.labelSupplementalBackupDisabled.Size = new System.Drawing.Size(463, 26);
			this.labelSupplementalBackupDisabled.TabIndex = 20;
			this.labelSupplementalBackupDisabled.Text = "Warning:  your Supplemental Backups are disabled, using Supplemental Backups in a" +
    "ddition to your own backups is strongly recommended.";
			this.labelSupplementalBackupDisabled.Visible = false;
			// 
			// labelSupplementalBackupPath
			// 
			this.labelSupplementalBackupPath.ForeColor = System.Drawing.Color.Red;
			this.labelSupplementalBackupPath.Location = new System.Drawing.Point(42, 424);
			this.labelSupplementalBackupPath.Name = "labelSupplementalBackupPath";
			this.labelSupplementalBackupPath.Size = new System.Drawing.Size(463, 26);
			this.labelSupplementalBackupPath.TabIndex = 21;
			this.labelSupplementalBackupPath.Text = "Warning:  your Supplemental Backup Network Path is not set. It must be set in ord" +
    "er to store Supplemental Backups locally.";
			this.labelSupplementalBackupPath.Visible = false;
			// 
			// FormBackupReminder
			// 
			this.ClientSize = new System.Drawing.Size(612, 487);
			this.Controls.Add(this.labelSupplementalBackupPath);
			this.Controls.Add(this.labelSupplementalBackupDisabled);
			this.Controls.Add(this.checkNoStrategy);
			this.Controls.Add(this.checkC2);
			this.Controls.Add(this.checkC1);
			this.Controls.Add(this.labelStrategy);
			this.Controls.Add(this.checkNoProof);
			this.Controls.Add(this.checkB2);
			this.Controls.Add(this.checkB1);
			this.Controls.Add(this.labelProof);
			this.Controls.Add(this.checkA4);
			this.Controls.Add(this.checkA3);
			this.Controls.Add(this.checkA2);
			this.Controls.Add(this.checkA1);
			this.Controls.Add(this.labelBackupMethod);
			this.Controls.Add(this.checkNoBackups);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.butOK);
			this.Name = "FormBackupReminder";
			this.Text = "Backup Reminder";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBackupReminder_FormClosing);
			this.Load += new System.EventHandler(this.FormBackupReminder_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.CheckBox checkNoBackups;
		private System.Windows.Forms.CheckBox checkA1;
		private System.Windows.Forms.Label labelBackupMethod;
		private System.Windows.Forms.CheckBox checkA2;
		private System.Windows.Forms.CheckBox checkA4;
		private System.Windows.Forms.CheckBox checkA3;
		private System.Windows.Forms.CheckBox checkB2;
		private System.Windows.Forms.CheckBox checkB1;
		private System.Windows.Forms.Label labelProof;
		private System.Windows.Forms.CheckBox checkNoProof;
		private System.Windows.Forms.CheckBox checkNoStrategy;
		private System.Windows.Forms.CheckBox checkC2;
		private System.Windows.Forms.CheckBox checkC1;
		private System.Windows.Forms.Label labelStrategy;
		private System.Windows.Forms.Label labelSupplementalBackupDisabled;
		private System.Windows.Forms.Label labelSupplementalBackupPath;
	}
}