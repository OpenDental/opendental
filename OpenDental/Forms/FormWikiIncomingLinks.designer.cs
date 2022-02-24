namespace OpenDental{
	partial class FormWikiIncomingLinks {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiIncomingLinks));
			this.gridMain = new OpenDental.UI.GridOD();
			this.webBrowserWiki = new System.Windows.Forms.WebBrowser();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(248, 621);
			this.gridMain.TabIndex = 6;
			this.gridMain.Title = "Incoming Links";
			this.gridMain.TranslationName = "TableIncomingLinks";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.Click += new System.EventHandler(this.gridMain_Click);
			// 
			// webBrowserWiki
			// 
			this.webBrowserWiki.AllowNavigation = false;
			this.webBrowserWiki.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserWiki.IsWebBrowserContextMenuEnabled = false;
			this.webBrowserWiki.Location = new System.Drawing.Point(266, 12);
			this.webBrowserWiki.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserWiki.Name = "webBrowserWiki";
			this.webBrowserWiki.Size = new System.Drawing.Size(592, 621);
			this.webBrowserWiki.TabIndex = 0;
			this.webBrowserWiki.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserWiki_Navigated);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(864, 609);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormWikiIncomingLinks
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(951, 645);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.webBrowserWiki);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiIncomingLinks";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Incoming Links";
			this.Load += new System.EventHandler(this.FormWiki_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser webBrowserWiki;
		private UI.GridOD gridMain;
		private UI.Button butClose;

	}
}