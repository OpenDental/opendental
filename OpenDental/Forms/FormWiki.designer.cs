namespace OpenDental{
	partial class FormWiki {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWiki));
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.labelStatus = new System.Windows.Forms.Label();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.webBrowserWiki = new System.Windows.Forms.WebBrowser();
			this.menuHomeDropDown = new System.Windows.Forms.ContextMenu();
			this.menuItemHomePageSave = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "Left.gif");
			this.imageListMain.Images.SetKeyName(1, "Right.gif");
			this.imageListMain.Images.SetKeyName(2, "Manage22.gif");
			this.imageListMain.Images.SetKeyName(3, "home.gif");
			this.imageListMain.Images.SetKeyName(4, "editPencil.gif");
			this.imageListMain.Images.SetKeyName(5, "print.gif");
			this.imageListMain.Images.SetKeyName(6, "rename.gif");
			this.imageListMain.Images.SetKeyName(7, "deleteX.gif");
			this.imageListMain.Images.SetKeyName(8, "history.gif");
			this.imageListMain.Images.SetKeyName(9, "incoming.gif");
			this.imageListMain.Images.SetKeyName(10, "Add.gif");
			this.imageListMain.Images.SetKeyName(11, "allpages.gif");
			this.imageListMain.Images.SetKeyName(12, "search.gif");
			this.imageListMain.Images.SetKeyName(13, "WikiLists.png");
			this.imageListMain.Images.SetKeyName(14, "Drafts_2.png");
			// 
			// labelStatus
			// 
			this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelStatus.Location = new System.Drawing.Point(-3, 686);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(947, 18);
			this.labelStatus.TabIndex = 73;
			this.labelStatus.Text = "Status Bar";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(944, 25);
			this.ToolBarMain.TabIndex = 72;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// webBrowserWiki
			// 
			this.webBrowserWiki.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserWiki.Location = new System.Drawing.Point(0, 28);
			this.webBrowserWiki.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserWiki.Name = "webBrowserWiki";
			this.webBrowserWiki.Size = new System.Drawing.Size(944, 657);
			this.webBrowserWiki.TabIndex = 0;
			this.webBrowserWiki.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserWiki_DocumentCompleted);
			this.webBrowserWiki.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserWiki_Navigating);
			// 
			// menuHomeDropDown
			// 
			this.menuHomeDropDown.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemHomePageSave});
			// 
			// menuItemHomePageSave
			// 
			this.menuItemHomePageSave.Index = 0;
			this.menuItemHomePageSave.Text = "Save As Home Page";
			this.menuItemHomePageSave.Click += new System.EventHandler(this.menuItemHomePageSave_Click);
			// 
			// FormWiki
			// 
			this.ClientSize = new System.Drawing.Size(944, 704);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.webBrowserWiki);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWiki";
			this.Text = "Wiki";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWiki_FormClosing);
			this.Load += new System.EventHandler(this.FormWiki_Load);
			this.ResizeEnd += new System.EventHandler(this.FormWiki_ResizeEnd);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser webBrowserWiki;
		private UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.ContextMenu menuHomeDropDown;
		private System.Windows.Forms.MenuItem menuItemHomePageSave;
	}
}