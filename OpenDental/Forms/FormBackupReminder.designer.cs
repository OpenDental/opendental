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
			this.checkBackupMethodOnline = new System.Windows.Forms.CheckBox();
			this.labelBackupMethod = new System.Windows.Forms.Label();
			this.checkBackupMethodRemovable = new System.Windows.Forms.CheckBox();
			this.checkBackupMethodOther = new System.Windows.Forms.CheckBox();
			this.checkBackupMethodNetwork = new System.Windows.Forms.CheckBox();
			this.checkRestoreServer = new System.Windows.Forms.CheckBox();
			this.checkRestoreHome = new System.Windows.Forms.CheckBox();
			this.labelProof = new System.Windows.Forms.Label();
			this.checkNoProof = new System.Windows.Forms.CheckBox();
			this.checkNoStrategy = new System.Windows.Forms.CheckBox();
			this.checkSecondaryMethodHardCopy = new System.Windows.Forms.CheckBox();
			this.checkSecondaryMethodArchive = new System.Windows.Forms.CheckBox();
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
			// checkBackupMethodOnline
			// 
			this.checkBackupMethodOnline.Location = new System.Drawing.Point(45, 111);
			this.checkBackupMethodOnline.Name = "checkBackupMethodOnline";
			this.checkBackupMethodOnline.Size = new System.Drawing.Size(200, 20);
			this.checkBackupMethodOnline.TabIndex = 8;
			this.checkBackupMethodOnline.Text = "Online";
			this.checkBackupMethodOnline.UseVisualStyleBackColor = true;
			// 
			// labelBackupMethod
			// 
			this.labelBackupMethod.Location = new System.Drawing.Point(42, 95);
			this.labelBackupMethod.Name = "labelBackupMethod";
			this.labelBackupMethod.Size = new System.Drawing.Size(523, 18);
			this.labelBackupMethod.TabIndex = 7;
			this.labelBackupMethod.Text = "Do you make backups every single day?  Backup method:";
			// 
			// checkBackupMethodRemovable
			// 
			this.checkBackupMethodRemovable.Location = new System.Drawing.Point(45, 131);
			this.checkBackupMethodRemovable.Name = "checkBackupMethodRemovable";
			this.checkBackupMethodRemovable.Size = new System.Drawing.Size(530, 20);
			this.checkBackupMethodRemovable.TabIndex = 9;
			this.checkBackupMethodRemovable.Text = "Removable (external HD, USB drive, etc)";
			this.checkBackupMethodRemovable.UseVisualStyleBackColor = true;
			// 
			// checkBackupMethodOther
			// 
			this.checkBackupMethodOther.Location = new System.Drawing.Point(45, 171);
			this.checkBackupMethodOther.Name = "checkBackupMethodOther";
			this.checkBackupMethodOther.Size = new System.Drawing.Size(151, 20);
			this.checkBackupMethodOther.TabIndex = 11;
			this.checkBackupMethodOther.Text = "Other backup method";
			this.checkBackupMethodOther.UseVisualStyleBackColor = true;
			// 
			// checkBackupMethodNetwork
			// 
			this.checkBackupMethodNetwork.Location = new System.Drawing.Point(45, 151);
			this.checkBackupMethodNetwork.Name = "checkBackupMethodNetwork";
			this.checkBackupMethodNetwork.Size = new System.Drawing.Size(302, 20);
			this.checkBackupMethodNetwork.TabIndex = 10;
			this.checkBackupMethodNetwork.Text = "Network (to another computer in your office)";
			this.checkBackupMethodNetwork.UseVisualStyleBackColor = true;
			// 
			// checkRestoreServer
			// 
			this.checkRestoreServer.Location = new System.Drawing.Point(45, 261);
			this.checkRestoreServer.Name = "checkRestoreServer";
			this.checkRestoreServer.Size = new System.Drawing.Size(250, 20);
			this.checkRestoreServer.TabIndex = 14;
			this.checkRestoreServer.Text = "Run backup from a second server";
			this.checkRestoreServer.UseVisualStyleBackColor = true;
			// 
			// checkRestoreHome
			// 
			this.checkRestoreHome.Location = new System.Drawing.Point(45, 241);
			this.checkRestoreHome.Name = "checkRestoreHome";
			this.checkRestoreHome.Size = new System.Drawing.Size(352, 20);
			this.checkRestoreHome.TabIndex = 13;
			this.checkRestoreHome.Text = "Restore to home computer at least once a week";
			this.checkRestoreHome.UseVisualStyleBackColor = true;
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
			// checkSecondaryMethodHardCopy
			// 
			this.checkSecondaryMethodHardCopy.Location = new System.Drawing.Point(45, 349);
			this.checkSecondaryMethodHardCopy.Name = "checkSecondaryMethodHardCopy";
			this.checkSecondaryMethodHardCopy.Size = new System.Drawing.Size(312, 20);
			this.checkSecondaryMethodHardCopy.TabIndex = 18;
			this.checkSecondaryMethodHardCopy.Text = "Saved hardcopy paper reports";
			this.checkSecondaryMethodHardCopy.UseVisualStyleBackColor = true;
			// 
			// checkSecondaryMethodArchive
			// 
			this.checkSecondaryMethodArchive.Location = new System.Drawing.Point(45, 329);
			this.checkSecondaryMethodArchive.Name = "checkSecondaryMethodArchive";
			this.checkSecondaryMethodArchive.Size = new System.Drawing.Size(352, 20);
			this.checkSecondaryMethodArchive.TabIndex = 17;
			this.checkSecondaryMethodArchive.Text = "Completely separate archives stored offsite (DVD, HD, etc)";
			this.checkSecondaryMethodArchive.UseVisualStyleBackColor = true;
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
			this.labelSupplementalBackupDisabled.BackColor = System.Drawing.SystemColors.Control;
			this.labelSupplementalBackupDisabled.ForeColor = System.Drawing.Color.Red;
			this.labelSupplementalBackupDisabled.Location = new System.Drawing.Point(42, 392);
			this.labelSupplementalBackupDisabled.Name = "labelSupplementalBackupDisabled";
			this.labelSupplementalBackupDisabled.Size = new System.Drawing.Size(463, 26);
			this.labelSupplementalBackupDisabled.TabIndex = 21;
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
			this.labelSupplementalBackupPath.TabIndex = 22;
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
			this.Controls.Add(this.checkSecondaryMethodHardCopy);
			this.Controls.Add(this.checkSecondaryMethodArchive);
			this.Controls.Add(this.labelStrategy);
			this.Controls.Add(this.checkNoProof);
			this.Controls.Add(this.checkRestoreServer);
			this.Controls.Add(this.checkRestoreHome);
			this.Controls.Add(this.labelProof);
			this.Controls.Add(this.checkBackupMethodOther);
			this.Controls.Add(this.checkBackupMethodNetwork);
			this.Controls.Add(this.checkBackupMethodRemovable);
			this.Controls.Add(this.checkBackupMethodOnline);
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
		private System.Windows.Forms.CheckBox checkBackupMethodOnline;
		private System.Windows.Forms.Label labelBackupMethod;
		private System.Windows.Forms.CheckBox checkBackupMethodRemovable;
		private System.Windows.Forms.CheckBox checkBackupMethodOther;
		private System.Windows.Forms.CheckBox checkBackupMethodNetwork;
		private System.Windows.Forms.CheckBox checkRestoreServer;
		private System.Windows.Forms.CheckBox checkRestoreHome;
		private System.Windows.Forms.Label labelProof;
		private System.Windows.Forms.CheckBox checkNoProof;
		private System.Windows.Forms.CheckBox checkNoStrategy;
		private System.Windows.Forms.CheckBox checkSecondaryMethodHardCopy;
		private System.Windows.Forms.CheckBox checkSecondaryMethodArchive;
		private System.Windows.Forms.Label labelStrategy;
		private System.Windows.Forms.Label labelSupplementalBackupDisabled;
		private System.Windows.Forms.Label labelSupplementalBackupPath;
	}
}