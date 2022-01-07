namespace OpenDental{
	partial class FormWikiHistory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiHistory));
			this.labelNotAuthorized = new System.Windows.Forms.Label();
			this.butRevert = new OpenDental.UI.Button();
			this.textContent = new OpenDental.ODcodeBox();
			this.webBrowserWiki = new System.Windows.Forms.WebBrowser();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelNotAuthorized
			// 
			this.labelNotAuthorized.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNotAuthorized.Location = new System.Drawing.Point(1067, 48);
			this.labelNotAuthorized.Name = "labelNotAuthorized";
			this.labelNotAuthorized.Size = new System.Drawing.Size(81, 110);
			this.labelNotAuthorized.TabIndex = 85;
			this.labelNotAuthorized.Text = "This wiki page is locked and cannot be edited without the WikiAdmin permission.";
			// 
			// butRevert
			// 
			this.butRevert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRevert.Location = new System.Drawing.Point(1067, 12);
			this.butRevert.Name = "butRevert";
			this.butRevert.Size = new System.Drawing.Size(75, 24);
			this.butRevert.TabIndex = 84;
			this.butRevert.Text = "Revert";
			this.butRevert.Click += new System.EventHandler(this.butRevert_Click);
			// 
			// textContent
			// 
			this.textContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textContent.Location = new System.Drawing.Point(266, 12);
			this.textContent.Name = "textContent";
			this.textContent.ReadOnly = true;
			this.textContent.Size = new System.Drawing.Size(375, 614);
			this.textContent.TabIndex = 82;
			this.textContent.Text = "";
			// 
			// webBrowserWiki
			// 
			this.webBrowserWiki.AllowWebBrowserDrop = false;
			this.webBrowserWiki.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserWiki.IsWebBrowserContextMenuEnabled = false;
			this.webBrowserWiki.Location = new System.Drawing.Point(647, 12);
			this.webBrowserWiki.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserWiki.Name = "webBrowserWiki";
			this.webBrowserWiki.Size = new System.Drawing.Size(414, 614);
			this.webBrowserWiki.TabIndex = 6;
			this.webBrowserWiki.WebBrowserShortcutsEnabled = false;
			this.webBrowserWiki.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserWiki_Navigated);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(248, 614);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Page History";
			this.gridMain.TranslationName = "TableWikiHistory";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.Click += new System.EventHandler(this.gridMain_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1067, 602);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormWikiHistory
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1154, 638);
			this.Controls.Add(this.labelNotAuthorized);
			this.Controls.Add(this.butRevert);
			this.Controls.Add(this.textContent);
			this.Controls.Add(this.webBrowserWiki);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiHistory";
			this.Text = "Wiki History";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormWikiHistory_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private System.Windows.Forms.WebBrowser webBrowserWiki;
		private ODcodeBox textContent;
		private UI.Button butRevert;
		private System.Windows.Forms.Label labelNotAuthorized;
	}
}