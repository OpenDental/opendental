namespace OpenDentalGraph {
	partial class FormDashboardEditTab {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDashboardEditTab));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.butSaveChanges = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.labelHelp = new System.Windows.Forms.Label();
			this.listItems = new System.Windows.Forms.ListBox();
			this.dashboardTabControl = new OpenDentalGraph.DashboardTabCtrl();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemDefaultGraphs = new System.Windows.Forms.ToolStripMenuItem();
			this.addPracticeDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addClinicDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addProviderDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemResetAR = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 27);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.butSaveChanges);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.labelHelp);
			this.splitContainer1.Panel1.Controls.Add(this.listItems);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.dashboardTabControl);
			this.splitContainer1.Size = new System.Drawing.Size(1061, 495);
			this.splitContainer1.SplitterDistance = 140;
			this.splitContainer1.TabIndex = 1;
			// 
			// butSaveChanges
			// 
			this.butSaveChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSaveChanges.Location = new System.Drawing.Point(24, 469);
			this.butSaveChanges.Name = "butSaveChanges";
			this.butSaveChanges.Size = new System.Drawing.Size(96, 23);
			this.butSaveChanges.TabIndex = 3;
			this.butSaveChanges.Text = "Save Changes";
			this.butSaveChanges.UseVisualStyleBackColor = true;
			this.butSaveChanges.Click += new System.EventHandler(this.butSaveChanges_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 330);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(134, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Graph Types:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelHelp
			// 
			this.labelHelp.Location = new System.Drawing.Point(4, 4);
			this.labelHelp.Name = "labelHelp";
			this.labelHelp.Size = new System.Drawing.Size(138, 326);
			this.labelHelp.TabIndex = 1;
			// 
			// listItems
			// 
			this.listItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listItems.FormattingEnabled = true;
			this.listItems.Location = new System.Drawing.Point(3, 346);
			this.listItems.MinimumSize = new System.Drawing.Size(4, 100);
			this.listItems.Name = "listItems";
			this.listItems.Size = new System.Drawing.Size(139, 121);
			this.listItems.TabIndex = 0;
			this.listItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listItems_MouseDown);
			// 
			// dashboardTabControl
			// 
			this.dashboardTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dashboardTabControl.HasUnsavedChanges = false;
			this.dashboardTabControl.IsEditMode = false;
			this.dashboardTabControl.Location = new System.Drawing.Point(2, 2);
			this.dashboardTabControl.Name = "dashboardTabControl";
			this.dashboardTabControl.Size = new System.Drawing.Size(914, 491);
			this.dashboardTabControl.TabIndex = 3;
			// 
			// menuStrip
			// 
			this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.refreshDataToolStripMenuItem,
            this.menuItemDefaultGraphs,
            this.menuItemResetAR});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(458, 24);
			this.menuStrip.TabIndex = 25;
			this.menuStrip.Text = "menuStrip1";
			// 
			// setupToolStripMenuItem
			// 
			this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
			this.setupToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
			this.setupToolStripMenuItem.Text = "Setup";
			this.setupToolStripMenuItem.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
			// 
			// refreshDataToolStripMenuItem
			// 
			this.refreshDataToolStripMenuItem.Name = "refreshDataToolStripMenuItem";
			this.refreshDataToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
			this.refreshDataToolStripMenuItem.Text = "Refresh Data";
			this.refreshDataToolStripMenuItem.Click += new System.EventHandler(this.refreshDataToolStripMenuItem_Click);
			// 
			// menuItemDefaultGraphs
			// 
			this.menuItemDefaultGraphs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPracticeDefaultToolStripMenuItem,
            this.addClinicDefaultToolStripMenuItem,
            this.addProviderDefaultToolStripMenuItem});
			this.menuItemDefaultGraphs.Name = "menuItemDefaultGraphs";
			this.menuItemDefaultGraphs.Size = new System.Drawing.Size(97, 20);
			this.menuItemDefaultGraphs.Text = "Default Graphs";
			this.menuItemDefaultGraphs.Visible = false;
			// 
			// addPracticeDefaultToolStripMenuItem
			// 
			this.addPracticeDefaultToolStripMenuItem.Name = "addPracticeDefaultToolStripMenuItem";
			this.addPracticeDefaultToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
			this.addPracticeDefaultToolStripMenuItem.Text = "Add Practice Default";
			this.addPracticeDefaultToolStripMenuItem.Click += new System.EventHandler(this.addPracticeDefaultToolStripMenuItem_Click);
			// 
			// addClinicDefaultToolStripMenuItem
			// 
			this.addClinicDefaultToolStripMenuItem.Name = "addClinicDefaultToolStripMenuItem";
			this.addClinicDefaultToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
			this.addClinicDefaultToolStripMenuItem.Text = "Add Clinic Default";
			this.addClinicDefaultToolStripMenuItem.Click += new System.EventHandler(this.addClinicDefaultToolStripMenuItem_Click);
			// 
			// addProviderDefaultToolStripMenuItem
			// 
			this.addProviderDefaultToolStripMenuItem.Name = "addProviderDefaultToolStripMenuItem";
			this.addProviderDefaultToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
			this.addProviderDefaultToolStripMenuItem.Text = "Add Provider Default";
			this.addProviderDefaultToolStripMenuItem.Click += new System.EventHandler(this.addProviderDefaultToolStripMenuItem_Click);
			// 
			// menuItemResetAR
			// 
			this.menuItemResetAR.Name = "menuItemResetAR";
			this.menuItemResetAR.Size = new System.Drawing.Size(127, 20);
			this.menuItemResetAR.Text = "Reset AR Graph Data";
			this.menuItemResetAR.Visible = false;
			this.menuItemResetAR.Click += new System.EventHandler(this.menuItemResetAR_Click);
			// 
			// FormDashboardEditTab
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1061, 520);
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDashboardEditTab";
			this.Text = "Dashboard";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDashboardEditTab_FormClosing);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox listItems;
		private System.Windows.Forms.Label labelHelp;
		private System.Windows.Forms.Label label2;
		private DashboardTabCtrl dashboardTabControl;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
		private System.Windows.Forms.Button butSaveChanges;
		private System.Windows.Forms.ToolStripMenuItem refreshDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuItemDefaultGraphs;
		private System.Windows.Forms.ToolStripMenuItem addPracticeDefaultToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addProviderDefaultToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addClinicDefaultToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuItemResetAR;
	}
}