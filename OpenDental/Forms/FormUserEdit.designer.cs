using System.Windows.Forms;

namespace OpenDental {
	public partial class FormUserEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUserEdit));
			this.tabControl1 = new OpenDental.UI.TabControl();
			this.tabUser = new OpenDental.UI.TabPage();
			this.labelBadgeInstruction = new System.Windows.Forms.Label();
			this.textBadgeId = new System.Windows.Forms.TextBox();
			this.labelBadgeId = new System.Windows.Forms.Label();
			this.textLogOffAfterMinutes = new System.Windows.Forms.TextBox();
			this.labelAutoLogoff = new System.Windows.Forms.Label();
			this.securityTreeUser = new OpenDental.UserControlSecurityTree();
			this.butDoseSpotAdditional = new OpenDental.UI.Button();
			this.textDoseSpotUserID = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.checkRequireReset = new OpenDental.UI.CheckBox();
			this.butPickDomainUser = new OpenDental.UI.Button();
			this.textDomainUser = new System.Windows.Forms.TextBox();
			this.labelDomainUser = new System.Windows.Forms.Label();
			this.textUserNum = new System.Windows.Forms.TextBox();
			this.label27 = new System.Windows.Forms.Label();
			this.checkIsHidden = new OpenDental.UI.CheckBox();
			this.listProv = new OpenDental.UI.ListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.listEmployee = new OpenDental.UI.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.listUserGroup = new OpenDental.UI.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabClinics = new OpenDental.UI.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.listClinicMulti = new OpenDental.UI.ListBox();
			this.listClinic = new OpenDental.UI.ListBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.tabAlertSubs = new OpenDental.UI.TabPage();
			this.labelAlertClinic = new System.Windows.Forms.Label();
			this.listAlertSubsClinicsMulti = new OpenDental.UI.ListBox();
			this.listAlertSubMulti = new OpenDental.UI.ListBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butJobRoles = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.butPassword = new OpenDental.UI.Button();
			this.butUnlock = new OpenDental.UI.Button();
			this.tabControl1.SuspendLayout();
			this.tabUser.SuspendLayout();
			this.tabClinics.SuspendLayout();
			this.tabAlertSubs.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabUser);
			this.tabControl1.Controls.Add(this.tabClinics);
			this.tabControl1.Controls.Add(this.tabAlertSubs);
			this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControl1.Location = new System.Drawing.Point(12, 13);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.Size = new System.Drawing.Size(979, 641);
			this.tabControl1.TabIndex = 0;
			// 
			// tabUser
			// 
			this.tabUser.BackColor = System.Drawing.SystemColors.Control;
			this.tabUser.Controls.Add(this.labelBadgeInstruction);
			this.tabUser.Controls.Add(this.textBadgeId);
			this.tabUser.Controls.Add(this.labelBadgeId);
			this.tabUser.Controls.Add(this.textLogOffAfterMinutes);
			this.tabUser.Controls.Add(this.labelAutoLogoff);
			this.tabUser.Controls.Add(this.securityTreeUser);
			this.tabUser.Controls.Add(this.butDoseSpotAdditional);
			this.tabUser.Controls.Add(this.textDoseSpotUserID);
			this.tabUser.Controls.Add(this.label8);
			this.tabUser.Controls.Add(this.checkRequireReset);
			this.tabUser.Controls.Add(this.butPickDomainUser);
			this.tabUser.Controls.Add(this.textDomainUser);
			this.tabUser.Controls.Add(this.labelDomainUser);
			this.tabUser.Controls.Add(this.textUserNum);
			this.tabUser.Controls.Add(this.label27);
			this.tabUser.Controls.Add(this.checkIsHidden);
			this.tabUser.Controls.Add(this.listProv);
			this.tabUser.Controls.Add(this.label5);
			this.tabUser.Controls.Add(this.label4);
			this.tabUser.Controls.Add(this.listEmployee);
			this.tabUser.Controls.Add(this.label2);
			this.tabUser.Controls.Add(this.textUserName);
			this.tabUser.Controls.Add(this.listUserGroup);
			this.tabUser.Controls.Add(this.label3);
			this.tabUser.Controls.Add(this.label1);
			this.tabUser.Location = new System.Drawing.Point(2, 21);
			this.tabUser.Name = "tabUser";
			this.tabUser.Padding = new System.Windows.Forms.Padding(3);
			this.tabUser.Size = new System.Drawing.Size(975, 618);
			this.tabUser.TabIndex = 0;
			this.tabUser.Text = "User";
			// 
			// labelBadgeInstruction
			// 
			this.labelBadgeInstruction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelBadgeInstruction.Location = new System.Drawing.Point(116, 50);
			this.labelBadgeInstruction.Name = "labelBadgeInstruction";
			this.labelBadgeInstruction.Size = new System.Drawing.Size(91, 20);
			this.labelBadgeInstruction.TabIndex = 270;
			this.labelBadgeInstruction.Text = "8 digits or less";
			this.labelBadgeInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBadgeId
			// 
			this.textBadgeId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBadgeId.Location = new System.Drawing.Point(63, 50);
			this.textBadgeId.Name = "textBadgeId";
			this.textBadgeId.Size = new System.Drawing.Size(58, 20);
			this.textBadgeId.TabIndex = 269;
			// 
			// labelBadgeId
			// 
			this.labelBadgeId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelBadgeId.Location = new System.Drawing.Point(0, 50);
			this.labelBadgeId.Name = "labelBadgeId";
			this.labelBadgeId.Size = new System.Drawing.Size(63, 20);
			this.labelBadgeId.TabIndex = 268;
			this.labelBadgeId.Text = "Badge ID";
			this.labelBadgeId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLogOffAfterMinutes
			// 
			this.textLogOffAfterMinutes.Location = new System.Drawing.Point(568, 53);
			this.textLogOffAfterMinutes.Name = "textLogOffAfterMinutes";
			this.textLogOffAfterMinutes.Size = new System.Drawing.Size(29, 20);
			this.textLogOffAfterMinutes.TabIndex = 267;
			this.textLogOffAfterMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// labelAutoLogoff
			// 
			this.labelAutoLogoff.Location = new System.Drawing.Point(425, 49);
			this.labelAutoLogoff.Name = "labelAutoLogoff";
			this.labelAutoLogoff.Size = new System.Drawing.Size(143, 47);
			this.labelAutoLogoff.TabIndex = 266;
			this.labelAutoLogoff.Text = "Automatic logoff time in minutes (0 is disabled, blank is global value)";
			this.labelAutoLogoff.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// securityTreeUser
			// 
			this.securityTreeUser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.securityTreeUser.BackColor = System.Drawing.Color.Transparent;
			this.securityTreeUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.securityTreeUser.Location = new System.Drawing.Point(603, 11);
			this.securityTreeUser.Name = "securityTreeUser";
			this.securityTreeUser.ReadOnly = true;
			this.securityTreeUser.Size = new System.Drawing.Size(358, 561);
			this.securityTreeUser.TabIndex = 174;
			// 
			// butDoseSpotAdditional
			// 
			this.butDoseSpotAdditional.Location = new System.Drawing.Point(375, 25);
			this.butDoseSpotAdditional.Name = "butDoseSpotAdditional";
			this.butDoseSpotAdditional.Size = new System.Drawing.Size(23, 21);
			this.butDoseSpotAdditional.TabIndex = 173;
			this.butDoseSpotAdditional.Text = "...";
			this.butDoseSpotAdditional.Click += new System.EventHandler(this.butDoseSpotAdditional_Click);
			// 
			// textDoseSpotUserID
			// 
			this.textDoseSpotUserID.Location = new System.Drawing.Point(274, 26);
			this.textDoseSpotUserID.Name = "textDoseSpotUserID";
			this.textDoseSpotUserID.Size = new System.Drawing.Size(96, 20);
			this.textDoseSpotUserID.TabIndex = 172;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(159, 26);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(117, 20);
			this.label8.TabIndex = 171;
			this.label8.Text = "DoseSpot User ID";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRequireReset
			// 
			this.checkRequireReset.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRequireReset.Location = new System.Drawing.Point(429, 11);
			this.checkRequireReset.Name = "checkRequireReset";
			this.checkRequireReset.Size = new System.Drawing.Size(168, 20);
			this.checkRequireReset.TabIndex = 170;
			this.checkRequireReset.Text = "Require Password Reset";
			// 
			// butPickDomainUser
			// 
			this.butPickDomainUser.Location = new System.Drawing.Point(376, 2);
			this.butPickDomainUser.Name = "butPickDomainUser";
			this.butPickDomainUser.Size = new System.Drawing.Size(23, 21);
			this.butPickDomainUser.TabIndex = 169;
			this.butPickDomainUser.Text = "...";
			this.butPickDomainUser.Click += new System.EventHandler(this.butPickDomainUser_Click);
			// 
			// textDomainUser
			// 
			this.textDomainUser.Location = new System.Drawing.Point(274, 3);
			this.textDomainUser.Name = "textDomainUser";
			this.textDomainUser.ReadOnly = true;
			this.textDomainUser.Size = new System.Drawing.Size(96, 20);
			this.textDomainUser.TabIndex = 168;
			// 
			// labelDomainUser
			// 
			this.labelDomainUser.Location = new System.Drawing.Point(194, 2);
			this.labelDomainUser.Name = "labelDomainUser";
			this.labelDomainUser.Size = new System.Drawing.Size(81, 20);
			this.labelDomainUser.TabIndex = 167;
			this.labelDomainUser.Text = "Domain User";
			this.labelDomainUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUserNum
			// 
			this.textUserNum.BackColor = System.Drawing.SystemColors.Control;
			this.textUserNum.Location = new System.Drawing.Point(63, 2);
			this.textUserNum.Name = "textUserNum";
			this.textUserNum.ReadOnly = true;
			this.textUserNum.Size = new System.Drawing.Size(72, 20);
			this.textUserNum.TabIndex = 165;
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(1, 4);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(62, 17);
			this.label27.TabIndex = 166;
			this.label27.Text = "UserNum";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.Location = new System.Drawing.Point(493, 31);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(104, 16);
			this.checkIsHidden.TabIndex = 163;
			this.checkIsHidden.Text = "Is Hidden";
			// 
			// listProv
			// 
			this.listProv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listProv.IntegralHeight = false;
			this.listProv.Location = new System.Drawing.Point(217, 110);
			this.listProv.Name = "listProv";
			this.listProv.Size = new System.Drawing.Size(185, 462);
			this.listProv.TabIndex = 160;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(217, 91);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(124, 20);
			this.label5.TabIndex = 159;
			this.label5.Text = "Provider";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(24, 585);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(378, 23);
			this.label4.TabIndex = 158;
			this.label4.Text = "Setting employee or provider is entirely optional";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listEmployee
			// 
			this.listEmployee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listEmployee.IntegralHeight = false;
			this.listEmployee.Location = new System.Drawing.Point(24, 110);
			this.listEmployee.Name = "listEmployee";
			this.listEmployee.Size = new System.Drawing.Size(185, 462);
			this.listEmployee.TabIndex = 157;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 91);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(165, 20);
			this.label2.TabIndex = 156;
			this.label2.Text = "Employee (for timecards)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textUserName
			// 
			this.textUserName.Location = new System.Drawing.Point(63, 26);
			this.textUserName.Name = "textUserName";
			this.textUserName.Size = new System.Drawing.Size(90, 20);
			this.textUserName.TabIndex = 152;
			// 
			// listUserGroup
			// 
			this.listUserGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listUserGroup.IntegralHeight = false;
			this.listUserGroup.Location = new System.Drawing.Point(410, 110);
			this.listUserGroup.Name = "listUserGroup";
			this.listUserGroup.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listUserGroup.Size = new System.Drawing.Size(185, 462);
			this.listUserGroup.TabIndex = 154;
			this.listUserGroup.SelectedIndexChanged += new System.EventHandler(this.listUserGroup_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(410, 91);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(185, 20);
			this.label3.TabIndex = 153;
			this.label3.Text = "User Group(s)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-2, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 20);
			this.label1.TabIndex = 151;
			this.label1.Text = "Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabClinics
			// 
			this.tabClinics.BackColor = System.Drawing.SystemColors.Control;
			this.tabClinics.Controls.Add(this.label6);
			this.tabClinics.Controls.Add(this.listClinicMulti);
			this.tabClinics.Controls.Add(this.listClinic);
			this.tabClinics.Controls.Add(this.labelClinic);
			this.tabClinics.Location = new System.Drawing.Point(2, 21);
			this.tabClinics.Name = "tabClinics";
			this.tabClinics.Padding = new System.Windows.Forms.Padding(3);
			this.tabClinics.Size = new System.Drawing.Size(975, 618);
			this.tabClinics.TabIndex = 1;
			this.tabClinics.Text = "Clinics";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(329, 43);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(150, 20);
			this.label6.TabIndex = 169;
			this.label6.Text = "User Restricted Clinics";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listClinicMulti
			// 
			this.listClinicMulti.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listClinicMulti.Location = new System.Drawing.Point(329, 66);
			this.listClinicMulti.Name = "listClinicMulti";
			this.listClinicMulti.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClinicMulti.Size = new System.Drawing.Size(250, 498);
			this.listClinicMulti.TabIndex = 168;
			// 
			// listClinic
			// 
			this.listClinic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listClinic.Location = new System.Drawing.Point(28, 66);
			this.listClinic.Name = "listClinic";
			this.listClinic.Size = new System.Drawing.Size(250, 498);
			this.listClinic.TabIndex = 166;
			this.listClinic.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listClinic_MouseClick);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(28, 43);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(150, 20);
			this.labelClinic.TabIndex = 165;
			this.labelClinic.Text = "User Default Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// tabAlertSubs
			// 
			this.tabAlertSubs.BackColor = System.Drawing.SystemColors.Control;
			this.tabAlertSubs.Controls.Add(this.labelAlertClinic);
			this.tabAlertSubs.Controls.Add(this.listAlertSubsClinicsMulti);
			this.tabAlertSubs.Controls.Add(this.listAlertSubMulti);
			this.tabAlertSubs.Controls.Add(this.label7);
			this.tabAlertSubs.Location = new System.Drawing.Point(2, 21);
			this.tabAlertSubs.Name = "tabAlertSubs";
			this.tabAlertSubs.Size = new System.Drawing.Size(975, 618);
			this.tabAlertSubs.TabIndex = 2;
			this.tabAlertSubs.Text = "Alert Subs";
			// 
			// labelAlertClinic
			// 
			this.labelAlertClinic.Location = new System.Drawing.Point(329, 43);
			this.labelAlertClinic.Name = "labelAlertClinic";
			this.labelAlertClinic.Size = new System.Drawing.Size(150, 20);
			this.labelAlertClinic.TabIndex = 171;
			this.labelAlertClinic.Text = "Clinics Subscribed";
			this.labelAlertClinic.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listAlertSubsClinicsMulti
			// 
			this.listAlertSubsClinicsMulti.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listAlertSubsClinicsMulti.Location = new System.Drawing.Point(329, 66);
			this.listAlertSubsClinicsMulti.Name = "listAlertSubsClinicsMulti";
			this.listAlertSubsClinicsMulti.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listAlertSubsClinicsMulti.Size = new System.Drawing.Size(250, 498);
			this.listAlertSubsClinicsMulti.TabIndex = 170;
			// 
			// listAlertSubMulti
			// 
			this.listAlertSubMulti.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listAlertSubMulti.Location = new System.Drawing.Point(28, 66);
			this.listAlertSubMulti.Name = "listAlertSubMulti";
			this.listAlertSubMulti.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listAlertSubMulti.Size = new System.Drawing.Size(250, 498);
			this.listAlertSubMulti.TabIndex = 168;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(28, 43);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(167, 20);
			this.label7.TabIndex = 167;
			this.label7.Text = "User Alert Subscriptions";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butJobRoles
			// 
			this.butJobRoles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butJobRoles.Location = new System.Drawing.Point(268, 663);
			this.butJobRoles.Name = "butJobRoles";
			this.butJobRoles.Size = new System.Drawing.Size(103, 26);
			this.butJobRoles.TabIndex = 167;
			this.butJobRoles.Text = "Set Job Roles";
			this.butJobRoles.Click += new System.EventHandler(this.butJobRoles_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(916, 663);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 26);
			this.butSave.TabIndex = 150;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butPassword
			// 
			this.butPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPassword.Location = new System.Drawing.Point(16, 663);
			this.butPassword.Name = "butPassword";
			this.butPassword.Size = new System.Drawing.Size(103, 26);
			this.butPassword.TabIndex = 155;
			this.butPassword.Text = "Change Password";
			this.butPassword.Click += new System.EventHandler(this.butPassword_Click);
			// 
			// butUnlock
			// 
			this.butUnlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUnlock.Location = new System.Drawing.Point(142, 663);
			this.butUnlock.Name = "butUnlock";
			this.butUnlock.Size = new System.Drawing.Size(103, 26);
			this.butUnlock.TabIndex = 168;
			this.butUnlock.Text = "Unlock Account";
			this.butUnlock.Click += new System.EventHandler(this.butUnlock_Click);
			// 
			// FormUserEdit
			// 
			this.ClientSize = new System.Drawing.Size(1003, 694);
			this.Controls.Add(this.butUnlock);
			this.Controls.Add(this.butJobRoles);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.butPassword);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormUserEdit";
			this.ShowInTaskbar = false;
			this.Text = "User Edit";
			this.Load += new System.EventHandler(this.FormUserEdit_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabUser.ResumeLayout(false);
			this.tabUser.PerformLayout();
			this.tabClinics.ResumeLayout(false);
			this.tabAlertSubs.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.TabControl tabControl1;
		private OpenDental.UI.TabPage tabUser;
		private OpenDental.UI.TabPage tabClinics;
		private UI.Button butSave;
		private UI.Button butPassword;
		private UI.Button butJobRoles;
		private Label labelClinic;
		private OpenDental.UI.ListBox listClinic;
		private Label label1;
		private Label label3;
		private OpenDental.UI.ListBox listUserGroup;
		private TextBox textUserName;
		private Label label2;
		private OpenDental.UI.ListBox listEmployee;
		private Label label4;
		private Label label5;
		private OpenDental.UI.ListBox listProv;
		private OpenDental.UI.CheckBox checkIsHidden;
		private Label label27;
		private TextBox textUserNum;
		private OpenDental.UI.ListBox listClinicMulti;
		private Label label6;
		private OpenDental.UI.TabPage tabAlertSubs;
		private OpenDental.UI.ListBox listAlertSubMulti;
		private Label label7;
		private Label labelAlertClinic;
		private OpenDental.UI.ListBox listAlertSubsClinicsMulti;
		private UI.Button butUnlock;
		private TextBox textDomainUser;
		private Label labelDomainUser;
		private UI.Button butPickDomainUser;
		private OpenDental.UI.CheckBox checkRequireReset;
		private TextBox textDoseSpotUserID;
		private Label label8;
		private UI.Button butDoseSpotAdditional;
		private UserControlSecurityTree securityTreeUser;
		private Label labelAutoLogoff;
		private TextBox textLogOffAfterMinutes;
		private TextBox textBadgeId;
		private Label labelBadgeId;
		private Label labelBadgeInstruction;
	}
}
