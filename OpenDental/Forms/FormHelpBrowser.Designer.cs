namespace OpenDental {
	partial class FormHelpBrowser {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHelpBrowser));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.faqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.manageFAQsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.forwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.webBrowserManual = new System.Windows.Forms.WebBrowser();
			this.webBrowserFAQ = new System.Windows.Forms.WebBrowser();
			this.versionReleaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripFaqQuickAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.faqToolStripMenuItem,
            this.versionReleaseToolStripMenuItem,
            this.backToolStripMenuItem,
            this.forwardToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1096, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// faqToolStripMenuItem
			// 
			this.faqToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageFAQsToolStripMenuItem,
            this.toolStripFaqQuickAdd});
			this.faqToolStripMenuItem.Name = "faqToolStripMenuItem";
			this.faqToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.faqToolStripMenuItem.Text = "FAQs";
			// 
			// manageFAQsToolStripMenuItem
			// 
			this.manageFAQsToolStripMenuItem.Name = "manageFAQsToolStripMenuItem";
			this.manageFAQsToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.manageFAQsToolStripMenuItem.Text = "Manage FAQ\'s";
			this.manageFAQsToolStripMenuItem.Click += new System.EventHandler(this.ManageToolStripMenuItem_Click);
			// 
			// backToolStripMenuItem
			// 
			this.backToolStripMenuItem.Image = global::OpenDental.Properties.Resources.Left;
			this.backToolStripMenuItem.Name = "backToolStripMenuItem";
			this.backToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
			this.backToolStripMenuItem.Text = "Back";
			this.backToolStripMenuItem.Click += new System.EventHandler(this.BackToolStripMenuItem_Click);
			// 
			// forwardToolStripMenuItem
			// 
			this.forwardToolStripMenuItem.Image = global::OpenDental.Properties.Resources.Right;
			this.forwardToolStripMenuItem.Name = "forwardToolStripMenuItem";
			this.forwardToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
			this.forwardToolStripMenuItem.Text = "Forward";
			this.forwardToolStripMenuItem.Click += new System.EventHandler(this.ForwardToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.webBrowserManual);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.webBrowserFAQ);
			this.splitContainer1.Size = new System.Drawing.Size(1096, 827);
			this.splitContainer1.SplitterDistance = 531;
			this.splitContainer1.SplitterWidth = 6;
			this.splitContainer1.TabIndex = 1;
			// 
			// webBrowserManual
			// 
			this.webBrowserManual.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowserManual.Location = new System.Drawing.Point(0, 0);
			this.webBrowserManual.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserManual.Name = "webBrowserManual";
			this.webBrowserManual.Size = new System.Drawing.Size(1096, 531);
			this.webBrowserManual.TabIndex = 0;
			this.webBrowserManual.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowserManual_Navigated);
			// 
			// webBrowserFAQ
			// 
			this.webBrowserFAQ.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowserFAQ.Location = new System.Drawing.Point(0, 0);
			this.webBrowserFAQ.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserFAQ.Name = "webBrowserFAQ";
			this.webBrowserFAQ.Size = new System.Drawing.Size(1096, 290);
			this.webBrowserFAQ.TabIndex = 1;
			this.webBrowserFAQ.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowserFAQ_Navigated);
			// 
			// versionReleaseToolStripMenuItem
			// 
			this.versionReleaseToolStripMenuItem.Name = "versionReleaseToolStripMenuItem";
			this.versionReleaseToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
			this.versionReleaseToolStripMenuItem.Text = "Version Release";
			this.versionReleaseToolStripMenuItem.Click += new System.EventHandler(this.VersionReleaseToolStripMenuItem_Click);
			// 
			// toolStripFaqQuickAdd
			// 
			this.toolStripFaqQuickAdd.Name = "toolStripFaqQuickAdd";
			this.toolStripFaqQuickAdd.Size = new System.Drawing.Size(211, 22);
			this.toolStripFaqQuickAdd.Text = "Add FAQ for Current Page";
			this.toolStripFaqQuickAdd.Click += new System.EventHandler(this.toolStripFaqQuickAdd_Click);
			// 
			// FormHelpBrowser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1096, 851);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormHelpBrowser";
			this.Text = "Help";
			this.CloseXClicked += new System.ComponentModel.CancelEventHandler(this.FormHelpBrowser_CloseXClicked);
			this.Load += new System.EventHandler(this.FormHelpBrowser_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem faqToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem manageFAQsToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.WebBrowser webBrowserManual;
		private System.Windows.Forms.WebBrowser webBrowserFAQ;
		private System.Windows.Forms.ToolStripMenuItem backToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem forwardToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem versionReleaseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripFaqQuickAdd;
	}
}