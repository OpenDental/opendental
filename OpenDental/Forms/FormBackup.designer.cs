namespace OpenDental {
	partial class FormBackup : FormODBase {
				///<summary></summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBackup));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageBackup = new System.Windows.Forms.TabPage();
			this.groupManagedBackups = new System.Windows.Forms.GroupBox();
			this.pictureCDS = new OpenDental.UI.ODPictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkExcludeImages = new System.Windows.Forms.CheckBox();
			this.butSave = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.butBrowseRestoreAtoZTo = new OpenDental.UI.Button();
			this.textBackupRestoreAtoZToPath = new System.Windows.Forms.TextBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.butBrowseRestoreTo = new OpenDental.UI.Button();
			this.textBackupRestoreToPath = new System.Windows.Forms.TextBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.butBrowseRestoreFrom = new OpenDental.UI.Button();
			this.textBackupRestoreFromPath = new System.Windows.Forms.TextBox();
			this.butRestore = new OpenDental.UI.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butBrowseFrom = new OpenDental.UI.Button();
			this.butBackup = new OpenDental.UI.Button();
			this.textBackupFromPath = new System.Windows.Forms.TextBox();
			this.textBackupToPath = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.butBrowseTo = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tabPageArchive = new System.Windows.Forms.TabPage();
			this.checkArchiveDoBackupFirst = new System.Windows.Forms.CheckBox();
			this.labelWarning = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.butSaveArchiveData = new OpenDental.UI.Button();
			this.groupBoxBackupConnection = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textArchiveServerName = new System.Windows.Forms.TextBox();
			this.textArchivePass = new System.Windows.Forms.TextBox();
			this.textArchiveUser = new System.Windows.Forms.TextBox();
			this.butArchive = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.dateTimeArchive = new System.Windows.Forms.DateTimePicker();
			this.tabPageSupplementalBackups = new System.Windows.Forms.TabPage();
			this.butSupplementalSaveDefaults = new OpenDental.UI.Button();
			this.butSupplementalBrowse = new OpenDental.UI.Button();
			this.labelSupplementalBackupCopyNetworkPath = new System.Windows.Forms.Label();
			this.textSupplementalBackupCopyNetworkPath = new OpenDental.ODtextBox();
			this.labelLastSupplementalBackupDateTime = new System.Windows.Forms.Label();
			this.textSupplementalBackupDateLastComplete = new OpenDental.ODtextBox();
			this.checkSupplementalBackupEnabled = new System.Windows.Forms.CheckBox();
			this.labelExplanation = new System.Windows.Forms.Label();
			this.folderBrowserSupplementalCopyNetworkPath = new OpenDental.FolderBrowserDialog();
			this.tabControl1.SuspendLayout();
			this.tabPageBackup.SuspendLayout();
			this.groupManagedBackups.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPageArchive.SuspendLayout();
			this.groupBoxBackupConnection.SuspendLayout();
			this.tabPageSupplementalBackups.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageBackup);
			this.tabControl1.Controls.Add(this.tabPageArchive);
			this.tabControl1.Controls.Add(this.tabPageSupplementalBackups);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(780, 579);
			this.tabControl1.TabIndex = 1;
			this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabControl1Tab_Selected);
			// 
			// tabPageBackup
			// 
			this.tabPageBackup.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageBackup.Controls.Add(this.groupManagedBackups);
			this.tabPageBackup.Controls.Add(this.label1);
			this.tabPageBackup.Controls.Add(this.checkExcludeImages);
			this.tabPageBackup.Controls.Add(this.butSave);
			this.tabPageBackup.Controls.Add(this.groupBox1);
			this.tabPageBackup.Controls.Add(this.textBox2);
			this.tabPageBackup.Controls.Add(this.butCancel);
			this.tabPageBackup.Controls.Add(this.butBrowseFrom);
			this.tabPageBackup.Controls.Add(this.butBackup);
			this.tabPageBackup.Controls.Add(this.textBackupFromPath);
			this.tabPageBackup.Controls.Add(this.textBackupToPath);
			this.tabPageBackup.Controls.Add(this.textBox1);
			this.tabPageBackup.Controls.Add(this.butBrowseTo);
			this.tabPageBackup.Controls.Add(this.groupBox2);
			this.tabPageBackup.Location = new System.Drawing.Point(4, 22);
			this.tabPageBackup.Name = "tabPageBackup";
			this.tabPageBackup.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageBackup.Size = new System.Drawing.Size(772, 553);
			this.tabPageBackup.TabIndex = 0;
			this.tabPageBackup.Text = "Backup";
			// 
			// groupManagedBackups
			// 
			this.groupManagedBackups.Controls.Add(this.pictureCDS);
			this.groupManagedBackups.Location = new System.Drawing.Point(319, 488);
			this.groupManagedBackups.Name = "groupManagedBackups";
			this.groupManagedBackups.Size = new System.Drawing.Size(114, 57);
			this.groupManagedBackups.TabIndex = 17;
			this.groupManagedBackups.TabStop = false;
			this.groupManagedBackups.Text = "Managed Backups";
			// 
			// pictureCDS
			// 
			this.pictureCDS.HasBorder = false;
			this.pictureCDS.Image = global::OpenDental.Properties.Resources.CDS_Button_green;
			this.pictureCDS.Location = new System.Drawing.Point(10, 22);
			this.pictureCDS.Name = "pictureCDS";
			this.pictureCDS.Size = new System.Drawing.Size(83, 24);
			this.pictureCDS.TabIndex = 16;
			this.pictureCDS.TextNullImage = null;
			this.pictureCDS.Click += new System.EventHandler(this.pictureCDS_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(19, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(713, 28);
			this.label1.TabIndex = 2;
			this.label1.Text = "BACKUPS ARE USELESS UNLESS YOU REGULARLY VERIFY THEIR QUALITY BY RESTORING THEM O" +
    "N A SEPARATE SECURE DEVICE. We suggest an encrypted USB flash drive for this pur" +
    "pose.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkExcludeImages
			// 
			this.checkExcludeImages.AutoSize = true;
			this.checkExcludeImages.Location = new System.Drawing.Point(13, 49);
			this.checkExcludeImages.Name = "checkExcludeImages";
			this.checkExcludeImages.Size = new System.Drawing.Size(221, 17);
			this.checkExcludeImages.TabIndex = 15;
			this.checkExcludeImages.Text = "Exclude image folder in backup or restore";
			this.checkExcludeImages.UseVisualStyleBackColor = true;
			this.checkExcludeImages.Click += new System.EventHandler(this.checkExcludeImages_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(15, 491);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(86, 26);
			this.butSave.TabIndex = 13;
			this.butSave.Text = "Save Defaults";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBox5);
			this.groupBox1.Controls.Add(this.butBrowseRestoreAtoZTo);
			this.groupBox1.Controls.Add(this.textBackupRestoreAtoZToPath);
			this.groupBox1.Controls.Add(this.textBox3);
			this.groupBox1.Controls.Add(this.butBrowseRestoreTo);
			this.groupBox1.Controls.Add(this.textBackupRestoreToPath);
			this.groupBox1.Controls.Add(this.textBox4);
			this.groupBox1.Controls.Add(this.butBrowseRestoreFrom);
			this.groupBox1.Controls.Add(this.textBackupRestoreFromPath);
			this.groupBox1.Controls.Add(this.butRestore);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(13, 262);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(747, 213);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Restore";
			// 
			// textBox5
			// 
			this.textBox5.BackColor = System.Drawing.SystemColors.Control;
			this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox5.Location = new System.Drawing.Point(7, 142);
			this.textBox5.Multiline = true;
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(396, 27);
			this.textBox5.TabIndex = 21;
			this.textBox5.Text = "Restore A-Z images to this folder: (example:)\r\nC:\\OpenDentImages\\";
			// 
			// butBrowseRestoreAtoZTo
			// 
			this.butBrowseRestoreAtoZTo.Location = new System.Drawing.Point(500, 170);
			this.butBrowseRestoreAtoZTo.Name = "butBrowseRestoreAtoZTo";
			this.butBrowseRestoreAtoZTo.Size = new System.Drawing.Size(86, 26);
			this.butBrowseRestoreAtoZTo.TabIndex = 20;
			this.butBrowseRestoreAtoZTo.Text = "Browse";
			this.butBrowseRestoreAtoZTo.Click += new System.EventHandler(this.butBrowseRestoreAtoZTo_Click);
			// 
			// textBackupRestoreAtoZToPath
			// 
			this.textBackupRestoreAtoZToPath.Location = new System.Drawing.Point(6, 173);
			this.textBackupRestoreAtoZToPath.Name = "textBackupRestoreAtoZToPath";
			this.textBackupRestoreAtoZToPath.Size = new System.Drawing.Size(481, 20);
			this.textBackupRestoreAtoZToPath.TabIndex = 19;
			// 
			// textBox3
			// 
			this.textBox3.BackColor = System.Drawing.SystemColors.Control;
			this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox3.Location = new System.Drawing.Point(7, 81);
			this.textBox3.Multiline = true;
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(247, 27);
			this.textBox3.TabIndex = 18;
			this.textBox3.Text = "Restore database TO this folder: (example:)\r\nC:\\mysql\\data\\";
			// 
			// butBrowseRestoreTo
			// 
			this.butBrowseRestoreTo.Location = new System.Drawing.Point(500, 109);
			this.butBrowseRestoreTo.Name = "butBrowseRestoreTo";
			this.butBrowseRestoreTo.Size = new System.Drawing.Size(86, 26);
			this.butBrowseRestoreTo.TabIndex = 17;
			this.butBrowseRestoreTo.Text = "Browse";
			this.butBrowseRestoreTo.Click += new System.EventHandler(this.butBrowseRestoreTo_Click);
			// 
			// textBackupRestoreToPath
			// 
			this.textBackupRestoreToPath.Location = new System.Drawing.Point(6, 112);
			this.textBackupRestoreToPath.Name = "textBackupRestoreToPath";
			this.textBackupRestoreToPath.Size = new System.Drawing.Size(481, 20);
			this.textBackupRestoreToPath.TabIndex = 16;
			// 
			// textBox4
			// 
			this.textBox4.BackColor = System.Drawing.SystemColors.Control;
			this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox4.Location = new System.Drawing.Point(7, 20);
			this.textBox4.Multiline = true;
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(280, 29);
			this.textBox4.TabIndex = 15;
			this.textBox4.Text = "Restore FROM this folder: (example:)\r\nD:\\";
			// 
			// butBrowseRestoreFrom
			// 
			this.butBrowseRestoreFrom.Location = new System.Drawing.Point(500, 47);
			this.butBrowseRestoreFrom.Name = "butBrowseRestoreFrom";
			this.butBrowseRestoreFrom.Size = new System.Drawing.Size(86, 26);
			this.butBrowseRestoreFrom.TabIndex = 14;
			this.butBrowseRestoreFrom.Text = "Browse";
			this.butBrowseRestoreFrom.Click += new System.EventHandler(this.butBrowseRestoreFrom_Click);
			// 
			// textBackupRestoreFromPath
			// 
			this.textBackupRestoreFromPath.Location = new System.Drawing.Point(6, 50);
			this.textBackupRestoreFromPath.Name = "textBackupRestoreFromPath";
			this.textBackupRestoreFromPath.Size = new System.Drawing.Size(481, 20);
			this.textBackupRestoreFromPath.TabIndex = 13;
			// 
			// butRestore
			// 
			this.butRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRestore.Location = new System.Drawing.Point(648, 170);
			this.butRestore.Name = "butRestore";
			this.butRestore.Size = new System.Drawing.Size(86, 26);
			this.butRestore.TabIndex = 6;
			this.butRestore.Text = "Restore";
			this.butRestore.Click += new System.EventHandler(this.butRestore_Click);
			// 
			// textBox2
			// 
			this.textBox2.BackColor = System.Drawing.SystemColors.Control;
			this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox2.Location = new System.Drawing.Point(20, 88);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(240, 43);
			this.textBox2.TabIndex = 12;
			this.textBox2.Text = "Backup database FROM this folder: (examples:)\r\nC:\\mysql\\data\\\r\n\\\\server\\mysql\\dat" +
    "a\\";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(663, 491);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(86, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butBrowseFrom
			// 
			this.butBrowseFrom.Location = new System.Drawing.Point(513, 130);
			this.butBrowseFrom.Name = "butBrowseFrom";
			this.butBrowseFrom.Size = new System.Drawing.Size(86, 26);
			this.butBrowseFrom.TabIndex = 11;
			this.butBrowseFrom.Text = "Browse";
			this.butBrowseFrom.Click += new System.EventHandler(this.butBrowseFrom_Click);
			// 
			// butBackup
			// 
			this.butBackup.Location = new System.Drawing.Point(666, 216);
			this.butBackup.Name = "butBackup";
			this.butBackup.Size = new System.Drawing.Size(86, 26);
			this.butBackup.TabIndex = 1;
			this.butBackup.Text = "Backup";
			this.butBackup.Click += new System.EventHandler(this.butBackup_Click);
			// 
			// textBackupFromPath
			// 
			this.textBackupFromPath.Location = new System.Drawing.Point(19, 133);
			this.textBackupFromPath.Name = "textBackupFromPath";
			this.textBackupFromPath.Size = new System.Drawing.Size(481, 20);
			this.textBackupFromPath.TabIndex = 10;
			// 
			// textBackupToPath
			// 
			this.textBackupToPath.Location = new System.Drawing.Point(19, 219);
			this.textBackupToPath.Name = "textBackupToPath";
			this.textBackupToPath.Size = new System.Drawing.Size(481, 20);
			this.textBackupToPath.TabIndex = 4;
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(20, 162);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(279, 55);
			this.textBox1.TabIndex = 9;
			this.textBox1.Text = "Backup TO this folder: (examples:)\r\nD:\\\r\nD:\\Backups\\\r\n\\\\frontdesk\\backups\\";
			// 
			// butBrowseTo
			// 
			this.butBrowseTo.Location = new System.Drawing.Point(513, 216);
			this.butBrowseTo.Name = "butBrowseTo";
			this.butBrowseTo.Size = new System.Drawing.Size(86, 26);
			this.butBrowseTo.TabIndex = 5;
			this.butBrowseTo.Text = "Browse";
			this.butBrowseTo.Click += new System.EventHandler(this.butBrowseTo_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Location = new System.Drawing.Point(13, 72);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(747, 184);
			this.groupBox2.TabIndex = 14;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Backup";
			// 
			// tabPageArchive
			// 
			this.tabPageArchive.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageArchive.Controls.Add(this.checkArchiveDoBackupFirst);
			this.tabPageArchive.Controls.Add(this.labelWarning);
			this.tabPageArchive.Controls.Add(this.label7);
			this.tabPageArchive.Controls.Add(this.butSaveArchiveData);
			this.tabPageArchive.Controls.Add(this.groupBoxBackupConnection);
			this.tabPageArchive.Controls.Add(this.butArchive);
			this.tabPageArchive.Controls.Add(this.label2);
			this.tabPageArchive.Controls.Add(this.dateTimeArchive);
			this.tabPageArchive.Location = new System.Drawing.Point(4, 22);
			this.tabPageArchive.Name = "tabPageArchive";
			this.tabPageArchive.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageArchive.Size = new System.Drawing.Size(772, 553);
			this.tabPageArchive.TabIndex = 1;
			this.tabPageArchive.Text = "Remove Old Data";
			// 
			// checkArchiveDoBackupFirst
			// 
			this.checkArchiveDoBackupFirst.Location = new System.Drawing.Point(140, 128);
			this.checkArchiveDoBackupFirst.Name = "checkArchiveDoBackupFirst";
			this.checkArchiveDoBackupFirst.Size = new System.Drawing.Size(166, 17);
			this.checkArchiveDoBackupFirst.TabIndex = 14;
			this.checkArchiveDoBackupFirst.Text = "Backup before removing data";
			this.checkArchiveDoBackupFirst.UseVisualStyleBackColor = true;
			this.checkArchiveDoBackupFirst.CheckedChanged += new System.EventHandler(this.checkMakeBackup_CheckedChanged);
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWarning.ForeColor = System.Drawing.Color.Red;
			this.labelWarning.Location = new System.Drawing.Point(246, 350);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(509, 55);
			this.labelWarning.TabIndex = 13;
			this.labelWarning.Text = "Not available when using a Middle Tier connection. You may only remove old data w" +
    "hen directly connected to the server.";
			this.labelWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelWarning.Visible = false;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(19, 12);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(734, 41);
			this.label7.TabIndex = 12;
			this.label7.Text = resources.GetString("label7.Text");
			// 
			// butSaveArchiveData
			// 
			this.butSaveArchiveData.Location = new System.Drawing.Point(141, 333);
			this.butSaveArchiveData.Name = "butSaveArchiveData";
			this.butSaveArchiveData.Size = new System.Drawing.Size(86, 26);
			this.butSaveArchiveData.TabIndex = 4;
			this.butSaveArchiveData.Text = "Save Defaults";
			this.butSaveArchiveData.UseVisualStyleBackColor = true;
			this.butSaveArchiveData.Click += new System.EventHandler(this.butSaveArchive_Click);
			// 
			// groupBoxBackupConnection
			// 
			this.groupBoxBackupConnection.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBoxBackupConnection.Controls.Add(this.label5);
			this.groupBoxBackupConnection.Controls.Add(this.label8);
			this.groupBoxBackupConnection.Controls.Add(this.label3);
			this.groupBoxBackupConnection.Controls.Add(this.textArchiveServerName);
			this.groupBoxBackupConnection.Controls.Add(this.textArchivePass);
			this.groupBoxBackupConnection.Controls.Add(this.textArchiveUser);
			this.groupBoxBackupConnection.Location = new System.Drawing.Point(140, 151);
			this.groupBoxBackupConnection.Name = "groupBoxBackupConnection";
			this.groupBoxBackupConnection.Size = new System.Drawing.Size(493, 176);
			this.groupBoxBackupConnection.TabIndex = 0;
			this.groupBoxBackupConnection.TabStop = false;
			this.groupBoxBackupConnection.Text = "Backup Connection Settings";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(7, 122);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(480, 18);
			this.label5.TabIndex = 37;
			this.label5.Text = "Password: For new installations, the password will be blank.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(7, 78);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(480, 18);
			this.label8.TabIndex = 36;
			this.label8.Text = "User: When MySQL is first installed, the user is root.";
			this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(481, 36);
			this.label3.TabIndex = 35;
			this.label3.Text = "Server Name: The name of the computer where the backup server and database are lo" +
    "cated.\r\nIf running on a single computer only, Server Name may be localhost.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textArchiveServerName
			// 
			this.textArchiveServerName.Location = new System.Drawing.Point(9, 55);
			this.textArchiveServerName.Name = "textArchiveServerName";
			this.textArchiveServerName.Size = new System.Drawing.Size(283, 20);
			this.textArchiveServerName.TabIndex = 0;
			// 
			// textArchivePass
			// 
			this.textArchivePass.Location = new System.Drawing.Point(10, 143);
			this.textArchivePass.Name = "textArchivePass";
			this.textArchivePass.PasswordChar = '*';
			this.textArchivePass.Size = new System.Drawing.Size(283, 20);
			this.textArchivePass.TabIndex = 3;
			// 
			// textArchiveUser
			// 
			this.textArchiveUser.Location = new System.Drawing.Point(10, 99);
			this.textArchiveUser.Name = "textArchiveUser";
			this.textArchiveUser.Size = new System.Drawing.Size(283, 20);
			this.textArchiveUser.TabIndex = 2;
			// 
			// butArchive
			// 
			this.butArchive.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butArchive.Location = new System.Drawing.Point(141, 365);
			this.butArchive.Name = "butArchive";
			this.butArchive.Size = new System.Drawing.Size(99, 26);
			this.butArchive.TabIndex = 2;
			this.butArchive.Text = "Remove Old Data";
			this.butArchive.UseVisualStyleBackColor = true;
			this.butArchive.Click += new System.EventHandler(this.butArchive_Click);
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label2.Location = new System.Drawing.Point(149, 76);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(272, 17);
			this.label2.TabIndex = 1;
			this.label2.Text = "Remove old data entries on or before:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// dateTimeArchive
			// 
			this.dateTimeArchive.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.dateTimeArchive.Location = new System.Drawing.Point(150, 96);
			this.dateTimeArchive.Name = "dateTimeArchive";
			this.dateTimeArchive.Size = new System.Drawing.Size(237, 20);
			this.dateTimeArchive.TabIndex = 1;
			// 
			// tabPageSupplementalBackups
			// 
			this.tabPageSupplementalBackups.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageSupplementalBackups.Controls.Add(this.butSupplementalSaveDefaults);
			this.tabPageSupplementalBackups.Controls.Add(this.butSupplementalBrowse);
			this.tabPageSupplementalBackups.Controls.Add(this.labelSupplementalBackupCopyNetworkPath);
			this.tabPageSupplementalBackups.Controls.Add(this.textSupplementalBackupCopyNetworkPath);
			this.tabPageSupplementalBackups.Controls.Add(this.labelLastSupplementalBackupDateTime);
			this.tabPageSupplementalBackups.Controls.Add(this.textSupplementalBackupDateLastComplete);
			this.tabPageSupplementalBackups.Controls.Add(this.checkSupplementalBackupEnabled);
			this.tabPageSupplementalBackups.Controls.Add(this.labelExplanation);
			this.tabPageSupplementalBackups.Location = new System.Drawing.Point(4, 22);
			this.tabPageSupplementalBackups.Name = "tabPageSupplementalBackups";
			this.tabPageSupplementalBackups.Size = new System.Drawing.Size(772, 553);
			this.tabPageSupplementalBackups.TabIndex = 2;
			this.tabPageSupplementalBackups.Text = "Supplemental Backups";
			// 
			// butSupplementalSaveDefaults
			// 
			this.butSupplementalSaveDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSupplementalSaveDefaults.Location = new System.Drawing.Point(188, 135);
			this.butSupplementalSaveDefaults.Name = "butSupplementalSaveDefaults";
			this.butSupplementalSaveDefaults.Size = new System.Drawing.Size(86, 26);
			this.butSupplementalSaveDefaults.TabIndex = 20;
			this.butSupplementalSaveDefaults.Text = "Save Defaults";
			this.butSupplementalSaveDefaults.Click += new System.EventHandler(this.ButSupplementalSaveDefaults_Click);
			// 
			// butSupplementalBrowse
			// 
			this.butSupplementalBrowse.Location = new System.Drawing.Point(688, 109);
			this.butSupplementalBrowse.Name = "butSupplementalBrowse";
			this.butSupplementalBrowse.Size = new System.Drawing.Size(30, 20);
			this.butSupplementalBrowse.TabIndex = 19;
			this.butSupplementalBrowse.Text = "...";
			this.butSupplementalBrowse.Click += new System.EventHandler(this.ButSupplementalBrowse_Click);
			// 
			// labelSupplementalBackupCopyNetworkPath
			// 
			this.labelSupplementalBackupCopyNetworkPath.Location = new System.Drawing.Point(8, 109);
			this.labelSupplementalBackupCopyNetworkPath.Name = "labelSupplementalBackupCopyNetworkPath";
			this.labelSupplementalBackupCopyNetworkPath.Size = new System.Drawing.Size(180, 20);
			this.labelSupplementalBackupCopyNetworkPath.TabIndex = 18;
			this.labelSupplementalBackupCopyNetworkPath.Text = "Backup Copy Network Path";
			this.labelSupplementalBackupCopyNetworkPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSupplementalBackupCopyNetworkPath
			// 
			this.textSupplementalBackupCopyNetworkPath.AcceptsTab = true;
			this.textSupplementalBackupCopyNetworkPath.BackColor = System.Drawing.SystemColors.Window;
			this.textSupplementalBackupCopyNetworkPath.DetectLinksEnabled = false;
			this.textSupplementalBackupCopyNetworkPath.DetectUrls = false;
			this.textSupplementalBackupCopyNetworkPath.Location = new System.Drawing.Point(188, 109);
			this.textSupplementalBackupCopyNetworkPath.Multiline = false;
			this.textSupplementalBackupCopyNetworkPath.Name = "textSupplementalBackupCopyNetworkPath";
			this.textSupplementalBackupCopyNetworkPath.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textSupplementalBackupCopyNetworkPath.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSupplementalBackupCopyNetworkPath.Size = new System.Drawing.Size(494, 20);
			this.textSupplementalBackupCopyNetworkPath.TabIndex = 17;
			this.textSupplementalBackupCopyNetworkPath.Text = "";
			// 
			// labelLastSupplementalBackupDateTime
			// 
			this.labelLastSupplementalBackupDateTime.Location = new System.Drawing.Point(8, 83);
			this.labelLastSupplementalBackupDateTime.Name = "labelLastSupplementalBackupDateTime";
			this.labelLastSupplementalBackupDateTime.Size = new System.Drawing.Size(180, 20);
			this.labelLastSupplementalBackupDateTime.TabIndex = 16;
			this.labelLastSupplementalBackupDateTime.Text = "Last Backup Date Time";
			this.labelLastSupplementalBackupDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSupplementalBackupDateLastComplete
			// 
			this.textSupplementalBackupDateLastComplete.AcceptsTab = true;
			this.textSupplementalBackupDateLastComplete.BackColor = System.Drawing.SystemColors.Control;
			this.textSupplementalBackupDateLastComplete.DetectLinksEnabled = false;
			this.textSupplementalBackupDateLastComplete.DetectUrls = false;
			this.textSupplementalBackupDateLastComplete.Location = new System.Drawing.Point(188, 83);
			this.textSupplementalBackupDateLastComplete.Multiline = false;
			this.textSupplementalBackupDateLastComplete.Name = "textSupplementalBackupDateLastComplete";
			this.textSupplementalBackupDateLastComplete.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textSupplementalBackupDateLastComplete.ReadOnly = true;
			this.textSupplementalBackupDateLastComplete.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSupplementalBackupDateLastComplete.Size = new System.Drawing.Size(150, 20);
			this.textSupplementalBackupDateLastComplete.TabIndex = 15;
			this.textSupplementalBackupDateLastComplete.Text = "";
			// 
			// checkSupplementalBackupEnabled
			// 
			this.checkSupplementalBackupEnabled.Location = new System.Drawing.Point(8, 60);
			this.checkSupplementalBackupEnabled.Name = "checkSupplementalBackupEnabled";
			this.checkSupplementalBackupEnabled.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkSupplementalBackupEnabled.Size = new System.Drawing.Size(193, 20);
			this.checkSupplementalBackupEnabled.TabIndex = 14;
			this.checkSupplementalBackupEnabled.Text = "Supplemental Backups Enabled";
			this.checkSupplementalBackupEnabled.UseVisualStyleBackColor = true;
			// 
			// labelExplanation
			// 
			this.labelExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelExplanation.Location = new System.Drawing.Point(8, 8);
			this.labelExplanation.Name = "labelExplanation";
			this.labelExplanation.Size = new System.Drawing.Size(755, 39);
			this.labelExplanation.TabIndex = 13;
			this.labelExplanation.Text = resources.GetString("labelExplanation.Text");
			this.labelExplanation.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormBackup
			// 
			this.ClientSize = new System.Drawing.Size(777, 582);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormBackup";
			this.ShowInTaskbar = false;
			this.Text = "Backup";
			this.Load += new System.EventHandler(this.FormBackup_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPageBackup.ResumeLayout(false);
			this.tabPageBackup.PerformLayout();
			this.groupManagedBackups.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPageArchive.ResumeLayout(false);
			this.groupBoxBackupConnection.ResumeLayout(false);
			this.groupBoxBackupConnection.PerformLayout();
			this.tabPageSupplementalBackups.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.CheckBox checkArchiveDoBackupFirst;
		private System.Windows.Forms.TabPage tabPageSupplementalBackups;
		private UI.Button butSupplementalBrowse;
		private System.Windows.Forms.Label labelSupplementalBackupCopyNetworkPath;
		private ODtextBox textSupplementalBackupCopyNetworkPath;
		private System.Windows.Forms.Label labelLastSupplementalBackupDateTime;
		private ODtextBox textSupplementalBackupDateLastComplete;
		private System.Windows.Forms.CheckBox checkSupplementalBackupEnabled;
		private System.Windows.Forms.Label labelExplanation;
		private OpenDental.FolderBrowserDialog folderBrowserSupplementalCopyNetworkPath;
		private UI.Button butSupplementalSaveDefaults;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butRestore;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox4;
		private OpenDental.UI.Button butBackup;
		private OpenDental.UI.Button butBrowseTo;
		private OpenDental.UI.Button butBrowseFrom;
		private OpenDental.UI.Button butBrowseRestoreFrom;
		private System.Windows.Forms.TextBox textBox3;
		private OpenDental.UI.Button butBrowseRestoreTo;
		private System.Windows.Forms.TextBox textBackupToPath;
		private System.Windows.Forms.TextBox textBackupFromPath;
		private System.Windows.Forms.TextBox textBackupRestoreFromPath;
		private System.Windows.Forms.TextBox textBackupRestoreToPath;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.TextBox textBackupRestoreAtoZToPath;
		private OpenDental.UI.Button butBrowseRestoreAtoZTo;
		private OpenDental.UI.Button butSave;
		//Required designer variable.
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkExcludeImages;
		private UI.ODPictureBox pictureCDS;
		private System.Windows.Forms.GroupBox groupManagedBackups;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageArchive;
		private System.Windows.Forms.TabPage tabPageBackup;
		private System.Windows.Forms.DateTimePicker dateTimeArchive;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textArchiveServerName;
		private System.Windows.Forms.TextBox textArchivePass;
		private System.Windows.Forms.TextBox textArchiveUser;
		private UI.Button butArchive;
		private System.Windows.Forms.GroupBox groupBoxBackupConnection;
		private System.Windows.Forms.Label label7;
		private UI.Button butSaveArchiveData;
		private System.Windows.Forms.Label labelWarning;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label5;
	}
}
