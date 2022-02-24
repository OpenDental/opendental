namespace CentralManager {
	partial class FormCentralSecurity {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralSecurity));
			this.imageListPerm = new System.Windows.Forms.ImageList(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.checkAdmin = new System.Windows.Forms.CheckBox();
			this.checkEnable = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textDays = new System.Windows.Forms.TextBox();
			this.textDate = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.textSyncCode = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butPushBoth = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.butPushLocks = new OpenDental.UI.Button();
			this.butPushUsers = new OpenDental.UI.Button();
			this.userControlSecurityTabs = new OpenDental.UserControlSecurityUserGroup();
			this.butOK = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.textDomainLoginPath = new System.Windows.Forms.TextBox();
			this.labelDomainPath = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.checkDomainLoginEnabled = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListPerm
			// 
			this.imageListPerm.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPerm.ImageStream")));
			this.imageListPerm.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListPerm.Images.SetKeyName(0, "grayBox.gif");
			this.imageListPerm.Images.SetKeyName(1, "checkBoxUnchecked.gif");
			this.imageListPerm.Images.SetKeyName(2, "checkBoxChecked.gif");
			this.imageListPerm.Images.SetKeyName(3, "checkBoxGreen.gif");
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.checkAdmin);
			this.groupBox1.Controls.Add(this.checkEnable);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textDays);
			this.groupBox1.Controls.Add(this.textDate);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 472);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(395, 197);
			this.groupBox1.TabIndex = 106;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Lock Date";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(383, 66);
			this.label1.TabIndex = 100;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(196, 144);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(163, 39);
			this.label6.TabIndex = 105;
			this.label6.Text = "(these settings are only editable from Central Manager)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkAdmin
			// 
			this.checkAdmin.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAdmin.Location = new System.Drawing.Point(5, 144);
			this.checkAdmin.Name = "checkAdmin";
			this.checkAdmin.Size = new System.Drawing.Size(185, 20);
			this.checkAdmin.TabIndex = 3;
			this.checkAdmin.Text = "Lock Includes Admins";
			this.checkAdmin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAdmin.UseVisualStyleBackColor = true;
			// 
			// checkEnable
			// 
			this.checkEnable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnable.Location = new System.Drawing.Point(5, 167);
			this.checkEnable.Name = "checkEnable";
			this.checkEnable.Size = new System.Drawing.Size(185, 20);
			this.checkEnable.TabIndex = 104;
			this.checkEnable.Text = "Central Manager Security Lock";
			this.checkEnable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnable.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 16);
			this.label4.TabIndex = 66;
			this.label4.Text = "Days";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(28, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 18);
			this.label3.TabIndex = 65;
			this.label3.Text = "Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDays
			// 
			this.textDays.Location = new System.Drawing.Point(95, 114);
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(46, 20);
			this.textDays.TabIndex = 2;
			this.textDays.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDays_KeyDown);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(95, 88);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 1;
			this.textDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDate_KeyDown);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(146, 116);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(123, 16);
			this.label2.TabIndex = 68;
			this.label2.Text = "1 means only today";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSyncCode
			// 
			this.textSyncCode.Location = new System.Drawing.Point(18, 33);
			this.textSyncCode.Name = "textSyncCode";
			this.textSyncCode.ReadOnly = true;
			this.textSyncCode.Size = new System.Drawing.Size(122, 20);
			this.textSyncCode.TabIndex = 110;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(15, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(125, 16);
			this.label5.TabIndex = 111;
			this.label5.Text = "Sync Code";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.Controls.Add(this.butPushBoth);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.butPushLocks);
			this.groupBox2.Controls.Add(this.textSyncCode);
			this.groupBox2.Controls.Add(this.butPushUsers);
			this.groupBox2.Location = new System.Drawing.Point(413, 472);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(267, 197);
			this.groupBox2.TabIndex = 107;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sync Options";
			// 
			// butPushBoth
			// 
			this.butPushBoth.Location = new System.Drawing.Point(18, 161);
			this.butPushBoth.Name = "butPushBoth";
			this.butPushBoth.Size = new System.Drawing.Size(75, 24);
			this.butPushBoth.TabIndex = 114;
			this.butPushBoth.Text = "Push Both";
			this.butPushBoth.Click += new System.EventHandler(this.butPushBoth_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(99, 133);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(167, 31);
			this.label8.TabIndex = 113;
			this.label8.Text = "Window will come up to allow selecting databases to push to";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(15, 54);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(245, 31);
			this.label7.TabIndex = 112;
			this.label7.Text = "All databases that can sync with this one must have this code in their Misc Setup" +
    " window.";
			// 
			// butPushLocks
			// 
			this.butPushLocks.Location = new System.Drawing.Point(18, 133);
			this.butPushLocks.Name = "butPushLocks";
			this.butPushLocks.Size = new System.Drawing.Size(75, 24);
			this.butPushLocks.TabIndex = 109;
			this.butPushLocks.Text = "Push Locks";
			this.butPushLocks.Click += new System.EventHandler(this.butPushLocks_Click);
			// 
			// butPushUsers
			// 
			this.butPushUsers.Location = new System.Drawing.Point(18, 105);
			this.butPushUsers.Name = "butPushUsers";
			this.butPushUsers.Size = new System.Drawing.Size(75, 24);
			this.butPushUsers.TabIndex = 108;
			this.butPushUsers.Text = "Push Users";
			this.butPushUsers.Click += new System.EventHandler(this.butPushUsers_Click);
			// 
			// userControlSecurityTabs
			// 
			this.userControlSecurityTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.userControlSecurityTabs.IsForCEMT = true;
			this.userControlSecurityTabs.Location = new System.Drawing.Point(2, 2);
			this.userControlSecurityTabs.MinimumSize = new System.Drawing.Size(914, 217);
			this.userControlSecurityTabs.Name = "userControlSecurityTabs";
			this.userControlSecurityTabs.SelectedUser = null;
			this.userControlSecurityTabs.SelectedUserGroup = null;
			this.userControlSecurityTabs.Size = new System.Drawing.Size(969, 464);
			this.userControlSecurityTabs.TabIndex = 253;
			this.userControlSecurityTabs.AddUserClick += new OpenDental.UserControlSecurityUserGroup.SecurityTabsEventHandler(this.userControlSecurityTabs_AddUserClick);
			this.userControlSecurityTabs.EditUserClick += new OpenDental.UserControlSecurityUserGroup.SecurityTabsEventHandler(this.userControlSecurityTabs_EditUserClick);
			this.userControlSecurityTabs.AddUserGroupClick += new OpenDental.UserControlSecurityUserGroup.SecurityTabsEventHandler(this.userControlSecurityTabs_AddUserGroupClick);
			this.userControlSecurityTabs.EditUserGroupClick += new OpenDental.UserControlSecurityUserGroup.SecurityTabsEventHandler(this.userControlSecurityTabs_EditUserGroupClick);
			this.userControlSecurityTabs.ReportPermissionChecked += new OpenDental.UserControlSecurityUserGroup.SecurityTreeEventHandler(this.userControlSecurityTabs_ReportPermissionChecked);
			this.userControlSecurityTabs.GroupPermissionChecked += new OpenDental.UserControlSecurityUserGroup.SecurityTreeEventHandler(this.userControlSecurityTabs_GroupPermissionChecked);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(897, 610);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 107;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(897, 640);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 65;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// textDomainLoginPath
			// 
			this.textDomainLoginPath.Location = new System.Drawing.Point(95, 37);
			this.textDomainLoginPath.Name = "textDomainLoginPath";
			this.textDomainLoginPath.Size = new System.Drawing.Size(163, 20);
			this.textDomainLoginPath.TabIndex = 271;
			this.textDomainLoginPath.Leave += new System.EventHandler(this.textDomainLoginPath_Leave);
			// 
			// labelDomainPath
			// 
			this.labelDomainPath.Location = new System.Drawing.Point(6, 38);
			this.labelDomainPath.Name = "labelDomainPath";
			this.labelDomainPath.Size = new System.Drawing.Size(89, 18);
			this.labelDomainPath.TabIndex = 272;
			this.labelDomainPath.Text = "Domain Path";
			this.labelDomainPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.checkDomainLoginEnabled);
			this.groupBox3.Controls.Add(this.textDomainLoginPath);
			this.groupBox3.Controls.Add(this.labelDomainPath);
			this.groupBox3.Location = new System.Drawing.Point(707, 472);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(264, 71);
			this.groupBox3.TabIndex = 274;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Domain Login";
			// 
			// checkDomainLoginEnabled
			// 
			this.checkDomainLoginEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDomainLoginEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDomainLoginEnabled.Location = new System.Drawing.Point(34, 15);
			this.checkDomainLoginEnabled.Name = "checkDomainLoginEnabled";
			this.checkDomainLoginEnabled.Size = new System.Drawing.Size(224, 16);
			this.checkDomainLoginEnabled.TabIndex = 270;
			this.checkDomainLoginEnabled.Text = "Domain Login Enabled";
			this.checkDomainLoginEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDomainLoginEnabled.CheckedChanged += new System.EventHandler(this.checkDomainLoginEnabled_CheckedChanged);
			this.checkDomainLoginEnabled.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkDomainLoginEnabled_MouseUp);
			// 
			// FormCentralSecurity
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(984, 676);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.userControlSecurityTabs);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butClose);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(1000, 714);
			this.Name = "FormCentralSecurity";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Central Manager User Setup";
			this.Load += new System.EventHandler(this.FormCentralSecurity_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.ImageList imageListPerm;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkAdmin;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textDays;
		private OpenDental.ValidDate textDate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkEnable;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butPushUsers;
		private OpenDental.UI.Button butPushLocks;
		private System.Windows.Forms.TextBox textSyncCode;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label7;
		private OpenDental.UserControlSecurityUserGroup userControlSecurityTabs;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.Button butPushBoth;
		private System.Windows.Forms.TextBox textDomainLoginPath;
		private System.Windows.Forms.Label labelDomainPath;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkDomainLoginEnabled;
	}
}