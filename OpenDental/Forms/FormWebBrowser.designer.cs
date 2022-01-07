namespace OpenDental{
	partial class FormWebBrowser {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebBrowser));
			this.browser = new System.Windows.Forms.WebBrowser();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.SuspendLayout();
			// 
			// browser
			// 
			this.browser.AllowWebBrowserDrop = false;
			this.browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.browser.IsWebBrowserContextMenuEnabled = false;
			this.browser.Location = new System.Drawing.Point(0, 26);
			this.browser.MinimumSize = new System.Drawing.Size(300, 300);
			this.browser.Name = "browser";
			this.browser.ScriptErrorsSuppressed = true;
			this.browser.Size = new System.Drawing.Size(974, 669);
			this.browser.TabIndex = 1;
			this.browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.browser_DocumentCompleted);
			this.browser.NewWindow += new System.ComponentModel.CancelEventHandler(this.browser_NewWindow);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "arrowLeft22.gif");
			this.imageList1.Images.SetKeyName(1, "arrowRight22.gif");
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageList1;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(974, 25);
			this.ToolBarMain.TabIndex = 32;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// FormErx
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(974, 696);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.browser);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebBrowser";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Web Browser";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormWebBrowser_Load);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.WebBrowser browser;
		private UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.ImageList imageList1;
	}
}