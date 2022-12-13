namespace OpenDental{
	partial class FormWikiSearch {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiSearch));
			this.gridMain = new OpenDental.UI.GridOD();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.checkIgnoreContent = new OpenDental.UI.CheckBox();
			this.checkArchivedOnly = new OpenDental.UI.CheckBox();
			this.webBrowserWiki = new System.Windows.Forms.WebBrowser();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butRestore = new OpenDental.UI.Button();
			this.checkBoxMatchWholeWord = new OpenDental.UI.CheckBox();
			this.checkBoxShowMainPages = new OpenDental.UI.CheckBox();
			this.butSearch = new OpenDental.UI.Button();
			this.checkReportServer = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 38);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(248, 612);
			this.gridMain.TabIndex = 10;
			this.gridMain.Title = "Wiki Pages";
			this.gridMain.TranslationName = "TableWikiSearchPages";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(12, 12);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(248, 20);
			this.textSearch.TabIndex = 0;
			this.textSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textSearch_KeyDown);
			// 
			// checkIgnoreContent
			// 
			this.checkIgnoreContent.Location = new System.Drawing.Point(414, 11);
			this.checkIgnoreContent.Name = "checkIgnoreContent";
			this.checkIgnoreContent.Size = new System.Drawing.Size(110, 22);
			this.checkIgnoreContent.TabIndex = 14;
			this.checkIgnoreContent.Text = "Ignore Content";
			this.checkIgnoreContent.Click += new System.EventHandler(this.checkIgnoreContent_Click);
			// 
			// checkArchivedOnly
			// 
			this.checkArchivedOnly.Location = new System.Drawing.Point(536, 11);
			this.checkArchivedOnly.Name = "checkArchivedOnly";
			this.checkArchivedOnly.Size = new System.Drawing.Size(108, 22);
			this.checkArchivedOnly.TabIndex = 15;
			this.checkArchivedOnly.Text = "Archived Only";
			this.checkArchivedOnly.CheckedChanged += new System.EventHandler(this.checkArchivedOnly_CheckedChanged);
			// 
			// webBrowserWiki
			// 
			this.webBrowserWiki.AllowNavigation = false;
			this.webBrowserWiki.AllowWebBrowserDrop = false;
			this.webBrowserWiki.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserWiki.IsWebBrowserContextMenuEnabled = false;
			this.webBrowserWiki.Location = new System.Drawing.Point(266, 38);
			this.webBrowserWiki.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserWiki.Name = "webBrowserWiki";
			this.webBrowserWiki.Size = new System.Drawing.Size(825, 612);
			this.webBrowserWiki.TabIndex = 11;
			this.webBrowserWiki.WebBrowserShortcutsEnabled = false;
			this.webBrowserWiki.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserWiki_Navigated);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1097, 596);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1097, 626);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butRestore
			// 
			this.butRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRestore.Enabled = false;
			this.butRestore.Location = new System.Drawing.Point(1097, 10);
			this.butRestore.Name = "butRestore";
			this.butRestore.Size = new System.Drawing.Size(75, 24);
			this.butRestore.TabIndex = 16;
			this.butRestore.Text = "Restore";
			this.butRestore.Click += new System.EventHandler(this.butRestore_Click);
			// 
			// checkBoxMatchWholeWord
			// 
			this.checkBoxMatchWholeWord.Location = new System.Drawing.Point(656, 14);
			this.checkBoxMatchWholeWord.Name = "checkBoxMatchWholeWord";
			this.checkBoxMatchWholeWord.Size = new System.Drawing.Size(102, 17);
			this.checkBoxMatchWholeWord.TabIndex = 18;
			this.checkBoxMatchWholeWord.Text = "Exact Match";
			this.checkBoxMatchWholeWord.CheckedChanged += new System.EventHandler(this.checkBoxMatchWholeWord_CheckedChanged);
			// 
			// checkBoxShowMainPages
			// 
			this.checkBoxShowMainPages.Checked = true;
			this.checkBoxShowMainPages.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxShowMainPages.Location = new System.Drawing.Point(770, 11);
			this.checkBoxShowMainPages.Name = "checkBoxShowMainPages";
			this.checkBoxShowMainPages.Size = new System.Drawing.Size(135, 22);
			this.checkBoxShowMainPages.TabIndex = 19;
			this.checkBoxShowMainPages.Text = "Show Main Pages";
			this.checkBoxShowMainPages.CheckedChanged += new System.EventHandler(this.checkBoxShowMainPages_CheckedChanged);
			// 
			// butSearch
			// 
			this.butSearch.Location = new System.Drawing.Point(266, 10);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(75, 24);
			this.butSearch.TabIndex = 20;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// checkReportServer
			// 
			this.checkReportServer.Location = new System.Drawing.Point(911, 12);
			this.checkReportServer.Name = "checkReportServer";
			this.checkReportServer.Size = new System.Drawing.Size(145, 22);
			this.checkReportServer.TabIndex = 21;
			this.checkReportServer.Text = "Run on report server";
			// 
			// FormWikiSearch
			// 
			this.ClientSize = new System.Drawing.Size(1184, 662);
			this.Controls.Add(this.checkReportServer);
			this.Controls.Add(this.butSearch);
			this.Controls.Add(this.checkBoxShowMainPages);
			this.Controls.Add(this.checkBoxMatchWholeWord);
			this.Controls.Add(this.butRestore);
			this.Controls.Add(this.checkArchivedOnly);
			this.Controls.Add(this.checkIgnoreContent);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.webBrowserWiki);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiSearch";
			this.Text = "Wiki Search";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWikiSearch_FormClosing);
			this.Load += new System.EventHandler(this.FormWikiSearch_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.WebBrowser webBrowserWiki;
		private UI.GridOD gridMain;
		private System.Windows.Forms.TextBox textSearch;
		private OpenDental.UI.CheckBox checkIgnoreContent;
		private OpenDental.UI.CheckBox checkArchivedOnly;
		private UI.Button butRestore;
		private OpenDental.UI.CheckBox checkBoxMatchWholeWord;
		private OpenDental.UI.CheckBox checkBoxShowMainPages;
		private UI.Button butSearch;
		private OpenDental.UI.CheckBox checkReportServer;
	}
}