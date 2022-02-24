namespace OpenDental{
	partial class FormGlobalSecurity {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGlobalSecurity));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelDomainPath = new System.Windows.Forms.Label();
			this.textDomainLoginPath = new System.Windows.Forms.TextBox();
			this.checkDomainLoginEnabled = new System.Windows.Forms.CheckBox();
			this.checkPasswordForceWeakToStrong = new System.Windows.Forms.CheckBox();
			this.checkPasswordsStrongIncludeSpecial = new System.Windows.Forms.CheckBox();
			this.checkDisableBackupReminder = new System.Windows.Forms.CheckBox();
			this.labelGlobalDateLockDisabled = new System.Windows.Forms.Label();
			this.checkUserNameManualEntry = new System.Windows.Forms.CheckBox();
			this.textLogOffAfterMinutes = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkLogOffWindows = new System.Windows.Forms.CheckBox();
			this.textDaysLock = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkPasswordsMustBeStrong = new System.Windows.Forms.CheckBox();
			this.butChange = new OpenDental.UI.Button();
			this.textDateLock = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkCannotEditOwn = new System.Windows.Forms.CheckBox();
			this.checkTimecardSecurityEnabled = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboGroups = new OpenDental.UI.ComboBoxOD();
			this.labelPermission = new System.Windows.Forms.Label();
			this.checkAllowLogoffOverride = new System.Windows.Forms.CheckBox();
			this.checkCannotEditPastPayPeriods = new System.Windows.Forms.CheckBox();
			this.checkMaintainPatient = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(200, 498);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(281, 498);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
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
			// textDomainLoginPath
			// 
			this.textDomainLoginPath.Location = new System.Drawing.Point(95, 37);
			this.textDomainLoginPath.Name = "textDomainLoginPath";
			this.textDomainLoginPath.Size = new System.Drawing.Size(163, 20);
			this.textDomainLoginPath.TabIndex = 271;
			this.textDomainLoginPath.Leave += new System.EventHandler(this.textDomainLoginPath_Leave);
			// 
			// checkDomainLoginEnabled
			// 
			this.checkDomainLoginEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDomainLoginEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDomainLoginEnabled.Location = new System.Drawing.Point(34, 19);
			this.checkDomainLoginEnabled.Name = "checkDomainLoginEnabled";
			this.checkDomainLoginEnabled.Size = new System.Drawing.Size(224, 16);
			this.checkDomainLoginEnabled.TabIndex = 270;
			this.checkDomainLoginEnabled.Text = "Domain Login Enabled";
			this.checkDomainLoginEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDomainLoginEnabled.CheckedChanged += new System.EventHandler(this.checkDomainLoginEnabled_CheckedChanged);
			this.checkDomainLoginEnabled.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkDomainLoginEnabled_MouseUp);
			// 
			// checkPasswordForceWeakToStrong
			// 
			this.checkPasswordForceWeakToStrong.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordForceWeakToStrong.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPasswordForceWeakToStrong.Location = new System.Drawing.Point(74, 166);
			this.checkPasswordForceWeakToStrong.Name = "checkPasswordForceWeakToStrong";
			this.checkPasswordForceWeakToStrong.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordForceWeakToStrong.TabIndex = 269;
			this.checkPasswordForceWeakToStrong.Text = "Force password change if not strong";
			this.checkPasswordForceWeakToStrong.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPasswordsStrongIncludeSpecial
			// 
			this.checkPasswordsStrongIncludeSpecial.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordsStrongIncludeSpecial.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPasswordsStrongIncludeSpecial.Location = new System.Drawing.Point(74, 145);
			this.checkPasswordsStrongIncludeSpecial.Name = "checkPasswordsStrongIncludeSpecial";
			this.checkPasswordsStrongIncludeSpecial.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordsStrongIncludeSpecial.TabIndex = 268;
			this.checkPasswordsStrongIncludeSpecial.Text = "Strong passwords require special character";
			this.checkPasswordsStrongIncludeSpecial.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkDisableBackupReminder
			// 
			this.checkDisableBackupReminder.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDisableBackupReminder.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDisableBackupReminder.Location = new System.Drawing.Point(74, 83);
			this.checkDisableBackupReminder.Name = "checkDisableBackupReminder";
			this.checkDisableBackupReminder.Size = new System.Drawing.Size(275, 17);
			this.checkDisableBackupReminder.TabIndex = 267;
			this.checkDisableBackupReminder.Text = "Disable monthly backup reminder";
			this.checkDisableBackupReminder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDisableBackupReminder.Click += new System.EventHandler(this.checkDisableBackupReminder_Click);
			// 
			// labelGlobalDateLockDisabled
			// 
			this.labelGlobalDateLockDisabled.Location = new System.Drawing.Point(38, 63);
			this.labelGlobalDateLockDisabled.Name = "labelGlobalDateLockDisabled";
			this.labelGlobalDateLockDisabled.Size = new System.Drawing.Size(203, 18);
			this.labelGlobalDateLockDisabled.TabIndex = 266;
			this.labelGlobalDateLockDisabled.Text = "(Disabled from Central Management Tool)";
			this.labelGlobalDateLockDisabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelGlobalDateLockDisabled.Visible = false;
			// 
			// checkUserNameManualEntry
			// 
			this.checkUserNameManualEntry.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUserNameManualEntry.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUserNameManualEntry.Location = new System.Drawing.Point(74, 187);
			this.checkUserNameManualEntry.Name = "checkUserNameManualEntry";
			this.checkUserNameManualEntry.Size = new System.Drawing.Size(275, 18);
			this.checkUserNameManualEntry.TabIndex = 265;
			this.checkUserNameManualEntry.Text = "Manually enter log on credentials";
			this.checkUserNameManualEntry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLogOffAfterMinutes
			// 
			this.textLogOffAfterMinutes.Location = new System.Drawing.Point(321, 268);
			this.textLogOffAfterMinutes.Name = "textLogOffAfterMinutes";
			this.textLogOffAfterMinutes.Size = new System.Drawing.Size(29, 20);
			this.textLogOffAfterMinutes.TabIndex = 253;
			this.textLogOffAfterMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(76, 269);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(245, 18);
			this.label3.TabIndex = 263;
			this.label3.Text = "Automatic logoff time in minutes (0 to disable):";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkLogOffWindows
			// 
			this.checkLogOffWindows.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLogOffWindows.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLogOffWindows.Location = new System.Drawing.Point(74, 103);
			this.checkLogOffWindows.Name = "checkLogOffWindows";
			this.checkLogOffWindows.Size = new System.Drawing.Size(275, 18);
			this.checkLogOffWindows.TabIndex = 255;
			this.checkLogOffWindows.Text = "Log off user on Windows lock";
			this.checkLogOffWindows.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDaysLock
			// 
			this.textDaysLock.Location = new System.Drawing.Point(83, 40);
			this.textDaysLock.Name = "textDaysLock";
			this.textDaysLock.ReadOnly = true;
			this.textDaysLock.Size = new System.Drawing.Size(82, 20);
			this.textDaysLock.TabIndex = 262;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(2, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 18);
			this.label2.TabIndex = 261;
			this.label2.Text = "Lock Days";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPasswordsMustBeStrong
			// 
			this.checkPasswordsMustBeStrong.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordsMustBeStrong.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPasswordsMustBeStrong.Location = new System.Drawing.Point(74, 124);
			this.checkPasswordsMustBeStrong.Name = "checkPasswordsMustBeStrong";
			this.checkPasswordsMustBeStrong.Size = new System.Drawing.Size(275, 18);
			this.checkPasswordsMustBeStrong.TabIndex = 257;
			this.checkPasswordsMustBeStrong.Text = "Passwords must be strong";
			this.checkPasswordsMustBeStrong.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPasswordsMustBeStrong.Click += new System.EventHandler(this.checkPasswordsMustBeStrong_Click);
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(171, 26);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(70, 24);
			this.butChange.TabIndex = 258;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// textDateLock
			// 
			this.textDateLock.Location = new System.Drawing.Point(83, 19);
			this.textDateLock.Name = "textDateLock";
			this.textDateLock.ReadOnly = true;
			this.textDateLock.Size = new System.Drawing.Size(82, 20);
			this.textDateLock.TabIndex = 260;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(2, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 18);
			this.label1.TabIndex = 259;
			this.label1.Text = "Lock Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkCannotEditOwn
			// 
			this.checkCannotEditOwn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCannotEditOwn.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCannotEditOwn.Location = new System.Drawing.Point(74, 35);
			this.checkCannotEditOwn.Name = "checkCannotEditOwn";
			this.checkCannotEditOwn.Size = new System.Drawing.Size(275, 18);
			this.checkCannotEditOwn.TabIndex = 256;
			this.checkCannotEditOwn.Text = "Users cannot edit their own time card";
			this.checkCannotEditOwn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCannotEditOwn.Click += new System.EventHandler(this.checkCannotEditOwn_Click);
			// 
			// checkTimecardSecurityEnabled
			// 
			this.checkTimecardSecurityEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimecardSecurityEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTimecardSecurityEnabled.Location = new System.Drawing.Point(74, 14);
			this.checkTimecardSecurityEnabled.Name = "checkTimecardSecurityEnabled";
			this.checkTimecardSecurityEnabled.Size = new System.Drawing.Size(275, 18);
			this.checkTimecardSecurityEnabled.TabIndex = 254;
			this.checkTimecardSecurityEnabled.Text = "Time card security enabled";
			this.checkTimecardSecurityEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimecardSecurityEnabled.Click += new System.EventHandler(this.checkTimecardSecurityEnabled_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkDomainLoginEnabled);
			this.groupBox1.Controls.Add(this.textDomainLoginPath);
			this.groupBox1.Controls.Add(this.labelDomainPath);
			this.groupBox1.Location = new System.Drawing.Point(90, 317);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(264, 71);
			this.groupBox1.TabIndex = 273;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Domain Login";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textDateLock);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.butChange);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.textDaysLock);
			this.groupBox2.Controls.Add(this.labelGlobalDateLockDisabled);
			this.groupBox2.Location = new System.Drawing.Point(90, 396);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(264, 93);
			this.groupBox2.TabIndex = 274;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Global Lock";
			// 
			// comboGroups
			// 
			this.comboGroups.Location = new System.Drawing.Point(199, 241);
			this.comboGroups.Name = "comboGroups";
			this.comboGroups.Size = new System.Drawing.Size(151, 21);
			this.comboGroups.TabIndex = 275;
			// 
			// labelPermission
			// 
			this.labelPermission.Location = new System.Drawing.Point(38, 242);
			this.labelPermission.Name = "labelPermission";
			this.labelPermission.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelPermission.Size = new System.Drawing.Size(159, 19);
			this.labelPermission.TabIndex = 276;
			this.labelPermission.Text = "Default User Group";
			this.labelPermission.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkAllowLogoffOverride
			// 
			this.checkAllowLogoffOverride.Location = new System.Drawing.Point(95, 295);
			this.checkAllowLogoffOverride.Name = "checkAllowLogoffOverride";
			this.checkAllowLogoffOverride.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkAllowLogoffOverride.Size = new System.Drawing.Size(255, 17);
			this.checkAllowLogoffOverride.TabIndex = 277;
			this.checkAllowLogoffOverride.Text = "Allow user override for automatic logoff";
			this.checkAllowLogoffOverride.UseVisualStyleBackColor = true;
			// 
			// checkCannotEditPastPayPeriods
			// 
			this.checkCannotEditPastPayPeriods.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCannotEditPastPayPeriods.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCannotEditPastPayPeriods.Location = new System.Drawing.Point(1, 56);
			this.checkCannotEditPastPayPeriods.Name = "checkCannotEditPastPayPeriods";
			this.checkCannotEditPastPayPeriods.Size = new System.Drawing.Size(348, 26);
			this.checkCannotEditPastPayPeriods.TabIndex = 278;
			this.checkCannotEditPastPayPeriods.Text = "Users cannot edit their own time card except current pay period\r\n";
			this.checkCannotEditPastPayPeriods.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCannotEditPastPayPeriods.Click += new System.EventHandler(this.checkCanEditOwnCur_Click);
			// 
			// checkMaintainPatient
			// 
			this.checkMaintainPatient.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMaintainPatient.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMaintainPatient.Location = new System.Drawing.Point(1, 208);
			this.checkMaintainPatient.Name = "checkMaintainPatient";
			this.checkMaintainPatient.Size = new System.Drawing.Size(348, 17);
			this.checkMaintainPatient.TabIndex = 279;
			this.checkMaintainPatient.Text = "Maintain selected patient when changing users";
			this.checkMaintainPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormGlobalSecurity
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(370, 534);
			this.Controls.Add(this.checkMaintainPatient);
			this.Controls.Add(this.checkCannotEditPastPayPeriods);
			this.Controls.Add(this.checkAllowLogoffOverride);
			this.Controls.Add(this.labelPermission);
			this.Controls.Add(this.comboGroups);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkPasswordForceWeakToStrong);
			this.Controls.Add(this.checkPasswordsStrongIncludeSpecial);
			this.Controls.Add(this.checkDisableBackupReminder);
			this.Controls.Add(this.checkUserNameManualEntry);
			this.Controls.Add(this.textLogOffAfterMinutes);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkLogOffWindows);
			this.Controls.Add(this.checkPasswordsMustBeStrong);
			this.Controls.Add(this.checkCannotEditOwn);
			this.Controls.Add(this.checkTimecardSecurityEnabled);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormGlobalSecurity";
			this.Text = "Global Security Settings";
			this.Load += new System.EventHandler(this.FormGlobalSecurity_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelDomainPath;
		private System.Windows.Forms.TextBox textDomainLoginPath;
		private System.Windows.Forms.CheckBox checkDomainLoginEnabled;
		private System.Windows.Forms.CheckBox checkPasswordForceWeakToStrong;
		private System.Windows.Forms.CheckBox checkPasswordsStrongIncludeSpecial;
		private System.Windows.Forms.CheckBox checkDisableBackupReminder;
		private System.Windows.Forms.Label labelGlobalDateLockDisabled;
		private System.Windows.Forms.CheckBox checkUserNameManualEntry;
		private System.Windows.Forms.TextBox textLogOffAfterMinutes;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkLogOffWindows;
		private System.Windows.Forms.TextBox textDaysLock;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkPasswordsMustBeStrong;
		private UI.Button butChange;
		private System.Windows.Forms.TextBox textDateLock;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkCannotEditOwn;
		private System.Windows.Forms.CheckBox checkTimecardSecurityEnabled;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private UI.ComboBoxOD comboGroups;
		private System.Windows.Forms.Label labelPermission;
		private System.Windows.Forms.CheckBox checkAllowLogoffOverride;
		private System.Windows.Forms.CheckBox checkCannotEditPastPayPeriods;
		private System.Windows.Forms.CheckBox checkMaintainPatient;
	}
}