namespace OpenDental {
	partial class UserControlSecurityUserGroup {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.contextMenuUsers = new System.Windows.Forms.ContextMenu();
			this.tabPageUserGroups = new System.Windows.Forms.TabPage();
			this.butExpandAll = new OpenDental.UI.Button();
			this.butCollapseAll = new OpenDental.UI.Button();
			this.butEditGroup = new OpenDental.UI.Button();
			this.butSetAll = new OpenDental.UI.Button();
			this.butAddGroup = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.userControlSecurityTree = new OpenDental.UserControlSecurityTree();
			this.listAssociatedUsers = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.listUserGroupTabUserGroups = new OpenDental.UI.ListBoxOD();
			this.tabPageUsers = new System.Windows.Forms.TabPage();
			this.panelUserGroups = new OpenDental.UI.PanelOD();
			this.butCopyUser = new OpenDental.UI.Button();
			this.labelUserCurr = new System.Windows.Forms.Label();
			this.listUserTabUserGroups = new OpenDental.UI.ListBoxOD();
			this.butAddUser = new OpenDental.UI.Button();
			this.labelUserTabUserGroups = new System.Windows.Forms.Label();
			this.gridUsers = new OpenDental.UI.GridOD();
			this.securityTreeUser = new OpenDental.UserControlSecurityTree();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.labelFilterType = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboShowOnly = new OpenDental.UI.ComboBoxOD();
			this.comboClinic = new OpenDental.UI.ComboBoxOD();
			this.textPowerSearch = new System.Windows.Forms.TextBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.comboGroups = new OpenDental.UI.ComboBoxOD();
			this.labelPermission = new System.Windows.Forms.Label();
			this.comboSchoolClass = new OpenDental.UI.ComboBoxOD();
			this.labelSchoolClass = new System.Windows.Forms.Label();
			this.labelPerm = new System.Windows.Forms.Label();
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageUserGroups.SuspendLayout();
			this.tabPageUsers.SuspendLayout();
			this.panelUserGroups.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabControlMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPageUserGroups
			// 
			this.tabPageUserGroups.Controls.Add(this.butExpandAll);
			this.tabPageUserGroups.Controls.Add(this.butCollapseAll);
			this.tabPageUserGroups.Controls.Add(this.butEditGroup);
			this.tabPageUserGroups.Controls.Add(this.butSetAll);
			this.tabPageUserGroups.Controls.Add(this.butAddGroup);
			this.tabPageUserGroups.Controls.Add(this.label3);
			this.tabPageUserGroups.Controls.Add(this.userControlSecurityTree);
			this.tabPageUserGroups.Controls.Add(this.listAssociatedUsers);
			this.tabPageUserGroups.Controls.Add(this.label4);
			this.tabPageUserGroups.Controls.Add(this.label6);
			this.tabPageUserGroups.Controls.Add(this.listUserGroupTabUserGroups);
			this.tabPageUserGroups.Location = new System.Drawing.Point(4, 22);
			this.tabPageUserGroups.Name = "tabPageUserGroups";
			this.tabPageUserGroups.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageUserGroups.Size = new System.Drawing.Size(961, 641);
			this.tabPageUserGroups.TabIndex = 0;
			this.tabPageUserGroups.Text = "User Groups";
			this.tabPageUserGroups.UseVisualStyleBackColor = true;
			// 
			// butExpandAll
			// 
			this.butExpandAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExpandAll.Location = new System.Drawing.Point(516, 612);
			this.butExpandAll.Name = "butExpandAll";
			this.butExpandAll.Size = new System.Drawing.Size(79, 24);
			this.butExpandAll.TabIndex = 27;
			this.butExpandAll.Text = "Expand All";
			this.butExpandAll.Click += new System.EventHandler(this.butExpandAll_Click);
			// 
			// butCollapseAll
			// 
			this.butCollapseAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCollapseAll.Location = new System.Drawing.Point(431, 612);
			this.butCollapseAll.Name = "butCollapseAll";
			this.butCollapseAll.Size = new System.Drawing.Size(79, 24);
			this.butCollapseAll.TabIndex = 26;
			this.butCollapseAll.Text = "Collapse All";
			this.butCollapseAll.Click += new System.EventHandler(this.butCollapseAll_Click);
			// 
			// butEditGroup
			// 
			this.butEditGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditGroup.Image = global::OpenDental.Properties.Resources.editPencil;
			this.butEditGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditGroup.Location = new System.Drawing.Point(189, 612);
			this.butEditGroup.Name = "butEditGroup";
			this.butEditGroup.Size = new System.Drawing.Size(87, 24);
			this.butEditGroup.TabIndex = 25;
			this.butEditGroup.Text = "Edit Group";
			this.butEditGroup.Click += new System.EventHandler(this.butEditGroup_Click);
			// 
			// butSetAll
			// 
			this.butSetAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSetAll.Location = new System.Drawing.Point(282, 612);
			this.butSetAll.Name = "butSetAll";
			this.butSetAll.Size = new System.Drawing.Size(79, 24);
			this.butSetAll.TabIndex = 20;
			this.butSetAll.Text = "Set All";
			this.butSetAll.Click += new System.EventHandler(this.butSetAll_Click);
			// 
			// butAddGroup
			// 
			this.butAddGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddGroup.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddGroup.Location = new System.Drawing.Point(6, 612);
			this.butAddGroup.Name = "butAddGroup";
			this.butAddGroup.Size = new System.Drawing.Size(87, 24);
			this.butAddGroup.TabIndex = 18;
			this.butAddGroup.Text = "Add Group";
			this.butAddGroup.Click += new System.EventHandler(this.butAddGroup_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 1);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(225, 15);
			this.label3.TabIndex = 24;
			this.label3.Text = "User Group:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// userControlSecurityTree
			// 
			this.userControlSecurityTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.userControlSecurityTree.Location = new System.Drawing.Point(282, 19);
			this.userControlSecurityTree.Name = "userControlSecurityTree";
			this.userControlSecurityTree.ReadOnly = false;
			this.userControlSecurityTree.Size = new System.Drawing.Size(426, 589);
			this.userControlSecurityTree.TabIndex = 23;
			this.userControlSecurityTree.ReportPermissionChecked += new OpenDental.UserControlSecurityTree.SecurityTreeEventHandler(this.securityTreeUserGroup_ReportPermissionChecked);
			this.userControlSecurityTree.GroupPermissionChecked += new OpenDental.UserControlSecurityTree.SecurityTreeEventHandler(this.securityTreeUserGroup_GroupPermissionChecked);
			// 
			// listAssociatedUsers
			// 
			this.listAssociatedUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listAssociatedUsers.IntegralHeight = false;
			this.listAssociatedUsers.Location = new System.Drawing.Point(714, 19);
			this.listAssociatedUsers.Name = "listAssociatedUsers";
			this.listAssociatedUsers.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listAssociatedUsers.Size = new System.Drawing.Size(242, 589);
			this.listAssociatedUsers.TabIndex = 22;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(711, 1);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(210, 15);
			this.label4.TabIndex = 21;
			this.label4.Text = "Users currently associated:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(279, 1);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(258, 15);
			this.label6.TabIndex = 19;
			this.label6.Text = "Permissions for group:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listUserGroupTabUserGroups
			// 
			this.listUserGroupTabUserGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listUserGroupTabUserGroups.ImeMode = System.Windows.Forms.ImeMode.On;
			this.listUserGroupTabUserGroups.IntegralHeight = false;
			this.listUserGroupTabUserGroups.Location = new System.Drawing.Point(6, 19);
			this.listUserGroupTabUserGroups.Name = "listUserGroupTabUserGroups";
			this.listUserGroupTabUserGroups.Size = new System.Drawing.Size(270, 589);
			this.listUserGroupTabUserGroups.TabIndex = 17;
			this.listUserGroupTabUserGroups.SelectedIndexChanged += new System.EventHandler(this.listUserGroupTabUserGroups_SelectedIndexChanged);
			this.listUserGroupTabUserGroups.DoubleClick += new System.EventHandler(this.listUserGroupTabUserGroups_DoubleClick);
			// 
			// tabPageUsers
			// 
			this.tabPageUsers.Controls.Add(this.panelUserGroups);
			this.tabPageUsers.Controls.Add(this.gridUsers);
			this.tabPageUsers.Controls.Add(this.securityTreeUser);
			this.tabPageUsers.Controls.Add(this.groupBox2);
			this.tabPageUsers.Controls.Add(this.labelPerm);
			this.tabPageUsers.Location = new System.Drawing.Point(4, 22);
			this.tabPageUsers.Name = "tabPageUsers";
			this.tabPageUsers.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageUsers.Size = new System.Drawing.Size(961, 641);
			this.tabPageUsers.TabIndex = 1;
			this.tabPageUsers.Text = "Users";
			this.tabPageUsers.UseVisualStyleBackColor = true;
			this.tabPageUsers.SizeChanged += new System.EventHandler(this.tabPageUsers_SizeChanged);
			// 
			// panelUserGroups
			// 
			this.panelUserGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.panelUserGroups.Controls.Add(this.butCopyUser);
			this.panelUserGroups.Controls.Add(this.labelUserCurr);
			this.panelUserGroups.Controls.Add(this.listUserTabUserGroups);
			this.panelUserGroups.Controls.Add(this.butAddUser);
			this.panelUserGroups.Controls.Add(this.labelUserTabUserGroups);
			this.panelUserGroups.Location = new System.Drawing.Point(454, 103);
			this.panelUserGroups.Name = "panelUserGroups";
			this.panelUserGroups.Size = new System.Drawing.Size(150, 513);
			this.panelUserGroups.TabIndex = 257;
			// 
			// butCopyUser
			// 
			this.butCopyUser.Location = new System.Drawing.Point(2, 26);
			this.butCopyUser.Name = "butCopyUser";
			this.butCopyUser.Size = new System.Drawing.Size(79, 24);
			this.butCopyUser.TabIndex = 257;
			this.butCopyUser.TabStop = false;
			this.butCopyUser.Text = "Copy User";
			this.butCopyUser.UseVisualStyleBackColor = true;
			this.butCopyUser.Visible = false;
			this.butCopyUser.Click += new System.EventHandler(this.butCopyUser_Click);
			// 
			// labelUserCurr
			// 
			this.labelUserCurr.Location = new System.Drawing.Point(3, 69);
			this.labelUserCurr.Name = "labelUserCurr";
			this.labelUserCurr.Size = new System.Drawing.Size(145, 15);
			this.labelUserCurr.TabIndex = 256;
			this.labelUserCurr.Text = "None";
			this.labelUserCurr.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listUserTabUserGroups
			// 
			this.listUserTabUserGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listUserTabUserGroups.IntegralHeight = false;
			this.listUserTabUserGroups.Location = new System.Drawing.Point(2, 87);
			this.listUserTabUserGroups.Name = "listUserTabUserGroups";
			this.listUserTabUserGroups.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listUserTabUserGroups.Size = new System.Drawing.Size(146, 426);
			this.listUserTabUserGroups.TabIndex = 5;
			this.listUserTabUserGroups.SelectedIndexChanged += new System.EventHandler(this.listUserTabUserGroups_SelectedIndexChanged);
			// 
			// butAddUser
			// 
			this.butAddUser.Location = new System.Drawing.Point(2, 0);
			this.butAddUser.Name = "butAddUser";
			this.butAddUser.Size = new System.Drawing.Size(79, 24);
			this.butAddUser.TabIndex = 255;
			this.butAddUser.Text = "Add User";
			this.butAddUser.Click += new System.EventHandler(this.butAddUser_Click);
			// 
			// labelUserTabUserGroups
			// 
			this.labelUserTabUserGroups.Location = new System.Drawing.Point(1, 54);
			this.labelUserTabUserGroups.Name = "labelUserTabUserGroups";
			this.labelUserTabUserGroups.Size = new System.Drawing.Size(145, 15);
			this.labelUserTabUserGroups.TabIndex = 12;
			this.labelUserTabUserGroups.Text = "User Groups for:";
			this.labelUserTabUserGroups.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridUsers
			// 
			this.gridUsers.AllowSortingByColumn = true;
			this.gridUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridUsers.HasMultilineHeaders = true;
			this.gridUsers.HScrollVisible = true;
			this.gridUsers.Location = new System.Drawing.Point(0, 103);
			this.gridUsers.Name = "gridUsers";
			this.gridUsers.Size = new System.Drawing.Size(454, 513);
			this.gridUsers.TabIndex = 254;
			this.gridUsers.Title = "Users";
			this.gridUsers.TranslationName = "TableUsers";
			this.gridUsers.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridUsers_CellDoubleClick);
			this.gridUsers.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridUsers_CellClick);
			// 
			// securityTreeUser
			// 
			this.securityTreeUser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.securityTreeUser.Location = new System.Drawing.Point(605, 23);
			this.securityTreeUser.Name = "securityTreeUser";
			this.securityTreeUser.ReadOnly = true;
			this.securityTreeUser.Size = new System.Drawing.Size(354, 593);
			this.securityTreeUser.TabIndex = 11;
			this.securityTreeUser.LocationChanged += new System.EventHandler(this.securityTreeUser_LocationChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkShowHidden);
			this.groupBox2.Controls.Add(this.labelFilterType);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.comboShowOnly);
			this.groupBox2.Controls.Add(this.comboClinic);
			this.groupBox2.Controls.Add(this.textPowerSearch);
			this.groupBox2.Controls.Add(this.labelClinic);
			this.groupBox2.Controls.Add(this.comboGroups);
			this.groupBox2.Controls.Add(this.labelPermission);
			this.groupBox2.Controls.Add(this.comboSchoolClass);
			this.groupBox2.Controls.Add(this.labelSchoolClass);
			this.groupBox2.Location = new System.Drawing.Point(6, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(410, 94);
			this.groupBox2.TabIndex = 248;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "User Filters";
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkShowHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowHidden.Location = new System.Drawing.Point(280, 56);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(118, 32);
			this.checkShowHidden.TabIndex = 248;
			this.checkShowHidden.Text = "Show hidden users";
			this.checkShowHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.Click += new System.EventHandler(this.Filter_Changed);
			// 
			// labelFilterType
			// 
			this.labelFilterType.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelFilterType.Location = new System.Drawing.Point(6, 52);
			this.labelFilterType.Name = "labelFilterType";
			this.labelFilterType.Size = new System.Drawing.Size(115, 15);
			this.labelFilterType.TabIndex = 248;
			this.labelFilterType.Text = "Username";
			this.labelFilterType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 13);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(115, 15);
			this.label5.TabIndex = 247;
			this.label5.Text = "Show Only";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboShowOnly
			// 
			this.comboShowOnly.Location = new System.Drawing.Point(6, 29);
			this.comboShowOnly.Name = "comboShowOnly";
			this.comboShowOnly.Size = new System.Drawing.Size(118, 21);
			this.comboShowOnly.TabIndex = 1;
			this.comboShowOnly.SelectedIndexChanged += new System.EventHandler(this.comboShowOnly_SelectionIndexChanged);
			// 
			// comboClinic
			// 
			this.comboClinic.Location = new System.Drawing.Point(280, 29);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(118, 21);
			this.comboClinic.TabIndex = 245;
			this.comboClinic.Visible = false;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.Filter_Changed);
			// 
			// textPowerSearch
			// 
			this.textPowerSearch.Location = new System.Drawing.Point(6, 68);
			this.textPowerSearch.Name = "textPowerSearch";
			this.textPowerSearch.Size = new System.Drawing.Size(118, 20);
			this.textPowerSearch.TabIndex = 243;
			this.textPowerSearch.TextChanged += new System.EventHandler(this.Filter_Changed);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(277, 13);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(115, 15);
			this.labelClinic.TabIndex = 246;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelClinic.Visible = false;
			// 
			// comboGroups
			// 
			this.comboGroups.Location = new System.Drawing.Point(143, 29);
			this.comboGroups.Name = "comboGroups";
			this.comboGroups.Size = new System.Drawing.Size(118, 21);
			this.comboGroups.TabIndex = 245;
			this.comboGroups.SelectionChangeCommitted += new System.EventHandler(this.Filter_Changed);
			// 
			// labelPermission
			// 
			this.labelPermission.Location = new System.Drawing.Point(140, 13);
			this.labelPermission.Name = "labelPermission";
			this.labelPermission.Size = new System.Drawing.Size(115, 15);
			this.labelPermission.TabIndex = 246;
			this.labelPermission.Text = "Group";
			this.labelPermission.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboSchoolClass
			// 
			this.comboSchoolClass.Location = new System.Drawing.Point(143, 67);
			this.comboSchoolClass.Name = "comboSchoolClass";
			this.comboSchoolClass.Size = new System.Drawing.Size(118, 21);
			this.comboSchoolClass.TabIndex = 2;
			this.comboSchoolClass.Visible = false;
			this.comboSchoolClass.SelectionChangeCommitted += new System.EventHandler(this.Filter_Changed);
			// 
			// labelSchoolClass
			// 
			this.labelSchoolClass.Location = new System.Drawing.Point(140, 51);
			this.labelSchoolClass.Name = "labelSchoolClass";
			this.labelSchoolClass.Size = new System.Drawing.Size(115, 15);
			this.labelSchoolClass.TabIndex = 91;
			this.labelSchoolClass.Text = "Class";
			this.labelSchoolClass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelSchoolClass.Visible = false;
			// 
			// labelPerm
			// 
			this.labelPerm.Location = new System.Drawing.Point(602, 5);
			this.labelPerm.Name = "labelPerm";
			this.labelPerm.Size = new System.Drawing.Size(354, 15);
			this.labelPerm.TabIndex = 10;
			this.labelPerm.Text = "Effective permissions for user: (Edit from the User Groups tab)";
			this.labelPerm.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageUsers);
			this.tabControlMain.Controls.Add(this.tabPageUserGroups);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(0, 0);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(969, 667);
			this.tabControlMain.TabIndex = 251;
			this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
			// 
			// UserControlSecurityUserGroup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControlMain);
			this.MinimumSize = new System.Drawing.Size(914, 217);
			this.Name = "UserControlSecurityUserGroup";
			this.Size = new System.Drawing.Size(969, 667);
			this.Load += new System.EventHandler(this.UserControlUserGroupSecurity_Load);
			this.tabPageUserGroups.ResumeLayout(false);
			this.tabPageUsers.ResumeLayout(false);
			this.panelUserGroups.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabControlMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabPage tabPageUserGroups;
		private UI.Button butEditGroup;
		private UI.Button butSetAll;
		private UI.Button butAddGroup;
		private System.Windows.Forms.Label label3;
		private UserControlSecurityTree userControlSecurityTree;
		private UI.ListBoxOD listAssociatedUsers;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private UI.ListBoxOD listUserGroupTabUserGroups;
		private System.Windows.Forms.TabPage tabPageUsers;
		private UI.GridOD gridUsers;
		private UserControlSecurityTree securityTreeUser;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkShowHidden;
		private System.Windows.Forms.Label labelFilterType;
		private System.Windows.Forms.Label label5;
		private UI.ComboBoxOD comboShowOnly;
		private UI.ComboBoxOD comboClinic;
		private System.Windows.Forms.TextBox textPowerSearch;
		private System.Windows.Forms.Label labelClinic;
		private UI.ComboBoxOD comboGroups;
		private System.Windows.Forms.Label labelPermission;
		private UI.ComboBoxOD comboSchoolClass;
		private System.Windows.Forms.Label labelSchoolClass;
		private UI.ListBoxOD listUserTabUserGroups;
		private System.Windows.Forms.Label labelPerm;
		private System.Windows.Forms.Label labelUserTabUserGroups;
		private System.Windows.Forms.TabControl tabControlMain;
		private UI.Button butAddUser;
		private UI.PanelOD panelUserGroups;
		private System.Windows.Forms.Label labelUserCurr;
		private UI.Button butCopyUser;
		private UI.Button butExpandAll;
		private UI.Button butCollapseAll;
	}
}
