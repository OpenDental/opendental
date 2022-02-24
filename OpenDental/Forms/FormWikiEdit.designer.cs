namespace OpenDental{
	partial class FormWikiEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiEdit));
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemCut = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolBar2 = new OpenDental.UI.ToolBarOD();
			this.webBrowserWiki = new System.Windows.Forms.WebBrowser();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.textContent = new OpenDental.ODcodeBox();
			this.contextMenuMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "refresh.gif");
			this.imageListMain.Images.SetKeyName(1, "save.gif");
			this.imageListMain.Images.SetKeyName(2, "cancel.gif");
			this.imageListMain.Images.SetKeyName(3, "cut.gif");
			this.imageListMain.Images.SetKeyName(4, "copy.gif");
			this.imageListMain.Images.SetKeyName(5, "paste.gif");
			this.imageListMain.Images.SetKeyName(6, "undo.gif");
			this.imageListMain.Images.SetKeyName(7, "link.gif");
			this.imageListMain.Images.SetKeyName(8, "linkExternal.gif");
			this.imageListMain.Images.SetKeyName(9, "h1.gif");
			this.imageListMain.Images.SetKeyName(10, "h2.gif");
			this.imageListMain.Images.SetKeyName(11, "h3.gif");
			this.imageListMain.Images.SetKeyName(12, "bold.gif");
			this.imageListMain.Images.SetKeyName(13, "italic.gif");
			this.imageListMain.Images.SetKeyName(14, "color.gif");
			this.imageListMain.Images.SetKeyName(15, "table.gif");
			this.imageListMain.Images.SetKeyName(16, "image.gif");
			this.imageListMain.Images.SetKeyName(17, "fontbutton.gif");
			this.imageListMain.Images.SetKeyName(18, "SaveDraft_2.png");
			this.imageListMain.Images.SetKeyName(19, "padlock_open.png");
			this.imageListMain.Images.SetKeyName(20, "padlock_closed.png");
			// 
			// contextMenuMain
			// 
			this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemCut,
            this.menuItemCopy,
            this.menuItemPaste,
            this.toolStripMenuItem2,
            this.menuItemUndo});
			this.contextMenuMain.Name = "contextMenuMain";
			this.contextMenuMain.Size = new System.Drawing.Size(104, 98);
			// 
			// menuItemCut
			// 
			this.menuItemCut.Name = "menuItemCut";
			this.menuItemCut.Size = new System.Drawing.Size(103, 22);
			this.menuItemCut.Text = "Cut";
			this.menuItemCut.Click += new System.EventHandler(this.menuItemCut_Click);
			// 
			// menuItemCopy
			// 
			this.menuItemCopy.Name = "menuItemCopy";
			this.menuItemCopy.Size = new System.Drawing.Size(103, 22);
			this.menuItemCopy.Text = "Copy";
			this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
			// 
			// menuItemPaste
			// 
			this.menuItemPaste.Name = "menuItemPaste";
			this.menuItemPaste.Size = new System.Drawing.Size(103, 22);
			this.menuItemPaste.Text = "Paste";
			this.menuItemPaste.Click += new System.EventHandler(this.menuItemPaste_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(100, 6);
			// 
			// menuItemUndo
			// 
			this.menuItemUndo.Name = "menuItemUndo";
			this.menuItemUndo.Size = new System.Drawing.Size(103, 22);
			this.menuItemUndo.Text = "Undo";
			this.menuItemUndo.Click += new System.EventHandler(this.menuItemUndo_Click);
			// 
			// toolBar2
			// 
			this.toolBar2.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolBar2.ImageList = this.imageListMain;
			this.toolBar2.Location = new System.Drawing.Point(0, 25);
			this.toolBar2.Name = "toolBar2";
			this.toolBar2.Size = new System.Drawing.Size(944, 25);
			this.toolBar2.TabIndex = 82;
			this.toolBar2.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBar2_ButtonClick);
			// 
			// webBrowserWiki
			// 
			this.webBrowserWiki.AllowWebBrowserDrop = false;
			this.webBrowserWiki.Location = new System.Drawing.Point(474, 58);
			this.webBrowserWiki.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserWiki.Name = "webBrowserWiki";
			this.webBrowserWiki.Size = new System.Drawing.Size(470, 514);
			this.webBrowserWiki.TabIndex = 78;
			this.webBrowserWiki.WebBrowserShortcutsEnabled = false;
			this.webBrowserWiki.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserWiki_DocumentCompleted);
			this.webBrowserWiki.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserWiki_Navigated);
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(944, 25);
			this.ToolBarMain.TabIndex = 3;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// textContent
			// 
			this.textContent.AcceptsTab = true;
			this.textContent.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textContent.Location = new System.Drawing.Point(0, 57);
			this.textContent.Name = "textContent";
			this.textContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textContent.Size = new System.Drawing.Size(472, 515);
			this.textContent.TabIndex = 83;
			this.textContent.Text = "";
			this.textContent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textContent_KeyPress);
			this.textContent.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textContent_MouseDoubleClick);
			// 
			// FormWikiEdit
			// 
			this.ClientSize = new System.Drawing.Size(944, 573);
			this.Controls.Add(this.textContent);
			this.Controls.Add(this.webBrowserWiki);
			this.Controls.Add(this.toolBar2);
			this.Controls.Add(this.ToolBarMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiEdit";
			this.Text = "Wiki Edit";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWikiEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormWikiEdit_Load);
			this.SizeChanged += new System.EventHandler(this.FormWikiEdit_SizeChanged);
			this.contextMenuMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.WebBrowser webBrowserWiki;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.ContextMenuStrip contextMenuMain;
		private System.Windows.Forms.ToolStripMenuItem menuItemCut;
		private System.Windows.Forms.ToolStripMenuItem menuItemCopy;
		private System.Windows.Forms.ToolStripMenuItem menuItemPaste;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem menuItemUndo;
		private UI.ToolBarOD toolBar2;
		private OpenDental.ODcodeBox textContent;
	}
}