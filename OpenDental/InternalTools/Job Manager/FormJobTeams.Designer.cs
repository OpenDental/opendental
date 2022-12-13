namespace OpenDental {
	partial class FormJobTeams {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobTeams));
			this.gridTeams = new OpenDental.UI.GridOD();
			this.gridTeamsRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteTeamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.listBoxTeamMembers = new OpenDental.UI.ListBoxOD();
			this.butAddTeamMember = new OpenDental.UI.Button();
			this.butDeleteTeamMember = new OpenDental.UI.Button();
			this.butChangeTeamLead = new OpenDental.UI.Button();
			this.gridTeamsRightClickMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridTeams
			// 
			this.gridTeams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridTeams.ContextMenuStrip = this.gridTeamsRightClickMenu;
			this.gridTeams.HasAddButton = true;
			this.gridTeams.Location = new System.Drawing.Point(12, 12);
			this.gridTeams.Name = "gridTeams";
			this.gridTeams.Size = new System.Drawing.Size(395, 440);
			this.gridTeams.TabIndex = 0;
			this.gridTeams.Text = "Teams";
			this.gridTeams.Title = "Teams";
			this.gridTeams.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTeams_CellDoubleClick);
			this.gridTeams.SelectionCommitted += new System.EventHandler(this.gridTeams_SelectionCommitted);
			this.gridTeams.TitleAddClick += new System.EventHandler(this.gridTeams_TitleAddClick);
			// 
			// gridTeamsRightClickMenu
			// 
			this.gridTeamsRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteTeamToolStripMenuItem});
			this.gridTeamsRightClickMenu.Name = "gridTeamsRightClickMenu";
			this.gridTeamsRightClickMenu.Size = new System.Drawing.Size(139, 26);
			// 
			// deleteTeamToolStripMenuItem
			// 
			this.deleteTeamToolStripMenuItem.Name = "deleteTeamToolStripMenuItem";
			this.deleteTeamToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.deleteTeamToolStripMenuItem.Text = "Delete Team";
			this.deleteTeamToolStripMenuItem.Click += new System.EventHandler(this.butDeleteTeamSelected_Click);
			// 
			// listBoxTeamMembers
			// 
			this.listBoxTeamMembers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxTeamMembers.Location = new System.Drawing.Point(410, 97);
			this.listBoxTeamMembers.Name = "listBoxTeamMembers";
			this.listBoxTeamMembers.Size = new System.Drawing.Size(146, 355);
			this.listBoxTeamMembers.TabIndex = 4;
			this.listBoxTeamMembers.Text = "Team Members";
			this.listBoxTeamMembers.SelectionChangeCommitted += new System.EventHandler(this.listBoxTeamMembers_SelectionChangeCommitted);
			// 
			// butAddTeamMember
			// 
			this.butAddTeamMember.Location = new System.Drawing.Point(427, 43);
			this.butAddTeamMember.Name = "butAddTeamMember";
			this.butAddTeamMember.Size = new System.Drawing.Size(114, 24);
			this.butAddTeamMember.TabIndex = 2;
			this.butAddTeamMember.Text = "Add Member";
			this.butAddTeamMember.UseVisualStyleBackColor = true;
			this.butAddTeamMember.Click += new System.EventHandler(this.butAddTeamMember_Click);
			// 
			// butDeleteTeamMember
			// 
			this.butDeleteTeamMember.Location = new System.Drawing.Point(427, 70);
			this.butDeleteTeamMember.Name = "butDeleteTeamMember";
			this.butDeleteTeamMember.Size = new System.Drawing.Size(114, 24);
			this.butDeleteTeamMember.TabIndex = 3;
			this.butDeleteTeamMember.Text = "Delete Member";
			this.butDeleteTeamMember.UseVisualStyleBackColor = true;
			this.butDeleteTeamMember.Click += new System.EventHandler(this.butDeleteTeamMember_Click);
			// 
			// butChangeTeamLead
			// 
			this.butChangeTeamLead.Location = new System.Drawing.Point(427, 16);
			this.butChangeTeamLead.Name = "butChangeTeamLead";
			this.butChangeTeamLead.Size = new System.Drawing.Size(114, 24);
			this.butChangeTeamLead.TabIndex = 1;
			this.butChangeTeamLead.Text = "Change Team Lead";
			this.butChangeTeamLead.UseVisualStyleBackColor = true;
			this.butChangeTeamLead.Click += new System.EventHandler(this.butChangeTeamLead_Click);
			// 
			// FormJobTeams
			// 
			this.ClientSize = new System.Drawing.Size(568, 465);
			this.Controls.Add(this.butChangeTeamLead);
			this.Controls.Add(this.butDeleteTeamMember);
			this.Controls.Add(this.butAddTeamMember);
			this.Controls.Add(this.listBoxTeamMembers);
			this.Controls.Add(this.gridTeams);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobTeams";
			this.Text = "Teams";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormJobTeams_FormClosing);
			this.Load += new System.EventHandler(this.FormJobTeams_Load);
			this.gridTeamsRightClickMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridTeams;
		private System.Windows.Forms.ContextMenuStrip gridTeamsRightClickMenu;
		private System.Windows.Forms.ToolStripMenuItem deleteTeamToolStripMenuItem;
		private UI.ListBoxOD listBoxTeamMembers;
		private UI.Button butAddTeamMember;
		private UI.Button butDeleteTeamMember;
		private UI.Button butChangeTeamLead;
	}
}

