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
			this.gridMain = new OpenDental.UI.GridOD();
			this.butCompare = new OpenDental.UI.Button();
			this.webBrowserWiki = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// labelNotAuthorized
			// 
			this.labelNotAuthorized.Location = new System.Drawing.Point(174, 12);
			this.labelNotAuthorized.Name = "labelNotAuthorized";
			this.labelNotAuthorized.Size = new System.Drawing.Size(887, 27);
			this.labelNotAuthorized.TabIndex = 85;
			this.labelNotAuthorized.Text = "This wiki page is locked and cannot be edited without the WikiAdmin permission.";
			// 
			// butRevert
			// 
			this.butRevert.Location = new System.Drawing.Point(93, 12);
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
			this.textContent.Location = new System.Drawing.Point(266, 42);
			this.textContent.Name = "textContent";
			this.textContent.ReadOnly = true;
			this.textContent.Size = new System.Drawing.Size(435, 584);
			this.textContent.TabIndex = 7;
			this.textContent.Text = "";
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 42);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(248, 584);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Page History";
			this.gridMain.TranslationName = "TableWikiHistory";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.Click += new System.EventHandler(this.gridMain_Click);
			// 
			// butCompare
			// 
			this.butCompare.Location = new System.Drawing.Point(12, 12);
			this.butCompare.Name = "butCompare";
			this.butCompare.Size = new System.Drawing.Size(75, 24);
			this.butCompare.TabIndex = 86;
			this.butCompare.Text = "Compare";
			this.butCompare.UseVisualStyleBackColor = false;
			this.butCompare.Click += new System.EventHandler(this.butCompare_Click);
			// 
			// webBrowserWiki
			// 
			this.webBrowserWiki.AllowWebBrowserDrop = false;
			this.webBrowserWiki.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserWiki.IsWebBrowserContextMenuEnabled = false;
			this.webBrowserWiki.Location = new System.Drawing.Point(707, 42);
			this.webBrowserWiki.Name = "webBrowserWiki";
			this.webBrowserWiki.Size = new System.Drawing.Size(435, 584);
			this.webBrowserWiki.TabIndex = 87;
			this.webBrowserWiki.WebBrowserShortcutsEnabled = false;
			// 
			// FormWikiHistory
			// 
			this.ClientSize = new System.Drawing.Size(1154, 638);
			this.Controls.Add(this.webBrowserWiki);
			this.Controls.Add(this.butCompare);
			this.Controls.Add(this.labelNotAuthorized);
			this.Controls.Add(this.butRevert);
			this.Controls.Add(this.textContent);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiHistory";
			this.Text = "Wiki History";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormWikiHistory_Load);
			this.SizeChanged += new System.EventHandler(this.FormWikiHistory_ResizeChanged);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
		private ODcodeBox textContent;
		private UI.Button butRevert;
		private System.Windows.Forms.Label labelNotAuthorized;
		private UI.Button butCompare;
		private System.Windows.Forms.WebBrowser webBrowserWiki;
	}
}