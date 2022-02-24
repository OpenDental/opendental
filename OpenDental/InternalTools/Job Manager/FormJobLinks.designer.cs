namespace OpenDental{
	partial class FormJobLinks {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobLinks));
			this.butRemove = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butLinkTask = new OpenDental.UI.Button();
			this.butLinkFeatReq = new OpenDental.UI.Button();
			this.butLinkBug = new OpenDental.UI.Button();
			this.groupAddLink = new System.Windows.Forms.GroupBox();
			this.butLinkQuote = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupAddLink.SuspendLayout();
			this.SuspendLayout();
			// 
			// butRemove
			// 
			this.butRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemove.Location = new System.Drawing.Point(365, 226);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(75, 24);
			this.butRemove.TabIndex = 17;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(365, 318);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 16;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butLinkTask
			// 
			this.butLinkTask.Location = new System.Drawing.Point(6, 19);
			this.butLinkTask.Name = "butLinkTask";
			this.butLinkTask.Size = new System.Drawing.Size(77, 22);
			this.butLinkTask.TabIndex = 0;
			this.butLinkTask.Text = "Task";
			this.butLinkTask.Click += new System.EventHandler(this.butLinkTask_Click);
			// 
			// butLinkFeatReq
			// 
			this.butLinkFeatReq.Location = new System.Drawing.Point(6, 47);
			this.butLinkFeatReq.Name = "butLinkFeatReq";
			this.butLinkFeatReq.Size = new System.Drawing.Size(77, 22);
			this.butLinkFeatReq.TabIndex = 1;
			this.butLinkFeatReq.Text = "Feat. Req.";
			this.butLinkFeatReq.Click += new System.EventHandler(this.butLinkFeatReq_Click);
			// 
			// butLinkBug
			// 
			this.butLinkBug.Enabled = false;
			this.butLinkBug.Location = new System.Drawing.Point(6, 75);
			this.butLinkBug.Name = "butLinkBug";
			this.butLinkBug.Size = new System.Drawing.Size(77, 22);
			this.butLinkBug.TabIndex = 2;
			this.butLinkBug.Text = "Bug";
			this.butLinkBug.Click += new System.EventHandler(this.butLinkBug_Click);
			// 
			// groupAddLink
			// 
			this.groupAddLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupAddLink.Controls.Add(this.butLinkQuote);
			this.groupAddLink.Controls.Add(this.butLinkTask);
			this.groupAddLink.Controls.Add(this.butLinkFeatReq);
			this.groupAddLink.Controls.Add(this.butLinkBug);
			this.groupAddLink.Location = new System.Drawing.Point(359, 12);
			this.groupAddLink.Name = "groupAddLink";
			this.groupAddLink.Size = new System.Drawing.Size(88, 133);
			this.groupAddLink.TabIndex = 13;
			this.groupAddLink.TabStop = false;
			this.groupAddLink.Text = "Add Link";
			// 
			// butLinkQuote
			// 
			this.butLinkQuote.Location = new System.Drawing.Point(6, 103);
			this.butLinkQuote.Name = "butLinkQuote";
			this.butLinkQuote.Size = new System.Drawing.Size(77, 22);
			this.butLinkQuote.TabIndex = 3;
			this.butLinkQuote.Text = "Quote";
			this.butLinkQuote.Click += new System.EventHandler(this.butLinkQuote_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(341, 330);
			this.gridMain.TabIndex = 18;
			this.gridMain.TabStop = false;
			this.gridMain.Title = "Links";
			this.gridMain.TranslationName = "TableLinks";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormJobLinks
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(456, 354);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.groupAddLink);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobLinks";
			this.Text = "Job Links";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormJobLinks_FormClosing);
			this.Load += new System.EventHandler(this.FormJobLinks_Load);
			this.groupAddLink.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.Button butRemove;
		private UI.Button butLinkTask;
		private UI.Button butLinkFeatReq;
		private UI.Button butLinkBug;
		private System.Windows.Forms.GroupBox groupAddLink;
		private UI.GridOD gridMain;
		private UI.Button butLinkQuote;
	}
}