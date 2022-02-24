namespace OpenDental{
	partial class FormEServicesMobileWeb{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesMobileWeb));
			this.label37 = new System.Windows.Forms.Label();
			this.menuWebSchedVerifyTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.insertReplacementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.butSetupMobileWebUsers = new OpenDental.UI.Button();
			this.label29 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textHostedUrlMobileWeb = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.butClose = new OpenDental.UI.Button();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(0, 0);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(100, 23);
			this.label37.TabIndex = 0;
			// 
			// menuWebSchedVerifyTextTemplate
			// 
			this.menuWebSchedVerifyTextTemplate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertReplacementsToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.selectAllToolStripMenuItem});
			this.menuWebSchedVerifyTextTemplate.Name = "menuASAPEmailBody";
			this.menuWebSchedVerifyTextTemplate.Size = new System.Drawing.Size(137, 136);
			this.menuWebSchedVerifyTextTemplate.Text = "Insert Replacements";
			// 
			// insertReplacementsToolStripMenuItem
			// 
			this.insertReplacementsToolStripMenuItem.Name = "insertReplacementsToolStripMenuItem";
			this.insertReplacementsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.insertReplacementsToolStripMenuItem.Text = "Insert Fields";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.selectAllToolStripMenuItem.Text = "Select All";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.butSetupMobileWebUsers);
			this.groupBox5.Controls.Add(this.label29);
			this.groupBox5.Location = new System.Drawing.Point(12, 107);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(691, 97);
			this.groupBox5.TabIndex = 79;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Setup Users";
			// 
			// butSetupMobileWebUsers
			// 
			this.butSetupMobileWebUsers.Location = new System.Drawing.Point(9, 67);
			this.butSetupMobileWebUsers.Name = "butSetupMobileWebUsers";
			this.butSetupMobileWebUsers.Size = new System.Drawing.Size(220, 24);
			this.butSetupMobileWebUsers.TabIndex = 250;
			this.butSetupMobileWebUsers.Text = "Setup Mobile Web Users";
			this.butSetupMobileWebUsers.Click += new System.EventHandler(this.butSetupMobileWebUsers_Click);
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(6, 16);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(664, 42);
			this.label29.TabIndex = 72;
			this.label29.Text = resources.GetString("label29.Text");
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textHostedUrlMobileWeb);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(691, 89);
			this.groupBox2.TabIndex = 78;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Mobile Web";
			// 
			// textHostedUrlMobileWeb
			// 
			this.textHostedUrlMobileWeb.Location = new System.Drawing.Point(144, 61);
			this.textHostedUrlMobileWeb.Name = "textHostedUrlMobileWeb";
			this.textHostedUrlMobileWeb.ReadOnly = true;
			this.textHostedUrlMobileWeb.Size = new System.Drawing.Size(349, 20);
			this.textHostedUrlMobileWeb.TabIndex = 74;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(12, 63);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(126, 17);
			this.label12.TabIndex = 73;
			this.label12.Text = "Hosted URL";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(664, 42);
			this.label5.TabIndex = 72;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(629, 216);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 501;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormEServicesMobileWeb
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(716, 252);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(0, 0);
			this.Name = "FormEServicesMobileWeb";
			this.Text = "eServices Mobile Web";
			this.Load += new System.EventHandler(this.FormEServicesMobileWeb_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textHostedUrlMobileWeb;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox5;
		private UI.Button butSetupMobileWebUsers;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.ContextMenuStrip menuWebSchedVerifyTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem insertReplacementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private UI.Button butClose;
	}
}