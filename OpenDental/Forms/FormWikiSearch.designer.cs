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
			this.label1 = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.checkIgnoreContent = new System.Windows.Forms.CheckBox();
			this.checkArchivedOnly = new System.Windows.Forms.CheckBox();
			this.webBrowserWiki = new System.Windows.Forms.WebBrowser();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butRestore = new OpenDental.UI.Button();
			this.checkBoxMatchWholeWord = new System.Windows.Forms.CheckBox();
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
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(82, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 18);
			this.label1.TabIndex = 13;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(160, 12);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(100, 20);
			this.textSearch.TabIndex = 0;
			// 
			// checkIgnoreContent
			// 
			this.checkIgnoreContent.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIgnoreContent.Location = new System.Drawing.Point(266, 9);
			this.checkIgnoreContent.Name = "checkIgnoreContent";
			this.checkIgnoreContent.Size = new System.Drawing.Size(188, 22);
			this.checkIgnoreContent.TabIndex = 14;
			this.checkIgnoreContent.Text = "Ignore Content";
			this.checkIgnoreContent.CheckedChanged += new System.EventHandler(this.checkIgnoreContent_CheckedChanged);
			// 
			// checkArchivedOnly
			// 
			this.checkArchivedOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkArchivedOnly.Location = new System.Drawing.Point(460, 9);
			this.checkArchivedOnly.Name = "checkArchivedOnly";
			this.checkArchivedOnly.Size = new System.Drawing.Size(188, 22);
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
			this.butRestore.Location = new System.Drawing.Point(1097, 12);
			this.butRestore.Name = "butRestore";
			this.butRestore.Size = new System.Drawing.Size(75, 24);
			this.butRestore.TabIndex = 16;
			this.butRestore.Text = "Restore";
			this.butRestore.Click += new System.EventHandler(this.butRestore_Click);
			// 
			// checkBoxMatchWholeWord
			// 
			this.checkBoxMatchWholeWord.Location = new System.Drawing.Point(654, 12);
			this.checkBoxMatchWholeWord.Name = "checkBoxMatchWholeWord";
			this.checkBoxMatchWholeWord.Size = new System.Drawing.Size(188, 17);
			this.checkBoxMatchWholeWord.TabIndex = 18;
			this.checkBoxMatchWholeWord.Text = "Exact Match";
			this.checkBoxMatchWholeWord.UseVisualStyleBackColor = true;
			this.checkBoxMatchWholeWord.CheckedChanged += new System.EventHandler(this.checkBoxMatchWholeWord_CheckedChanged);
			// 
			// FormWikiSearch
			// 
			this.ClientSize = new System.Drawing.Size(1184, 662);
			this.Controls.Add(this.checkBoxMatchWholeWord);
			this.Controls.Add(this.butRestore);
			this.Controls.Add(this.checkArchivedOnly);
			this.Controls.Add(this.checkIgnoreContent);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.webBrowserWiki);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiSearch";
			this.Text = "Wiki Search";
			this.Load += new System.EventHandler(this.FormWikiSearch_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.WebBrowser webBrowserWiki;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSearch;
		private System.Windows.Forms.CheckBox checkIgnoreContent;
		private System.Windows.Forms.CheckBox checkArchivedOnly;
		private UI.Button butRestore;
		private System.Windows.Forms.CheckBox checkBoxMatchWholeWord;
	}
}