namespace OpenDental{
	partial class FormWebBrowserPrefs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebBrowserPrefs));
			this.browser = new System.Windows.Forms.WebBrowser();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// browser
			// 
			this.browser.AllowWebBrowserDrop = false;
			this.browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.browser.IsWebBrowserContextMenuEnabled = false;
			this.browser.Location = new System.Drawing.Point(12, 1);
			this.browser.Name = "browser";
			this.browser.ScriptErrorsSuppressed = true;
			this.browser.Size = new System.Drawing.Size(360, 168);
			this.browser.TabIndex = 1;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "arrowLeft22.gif");
			this.imageList1.Images.SetKeyName(1, "arrowRight22.gif");
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(297, 175);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormWebBrowserPrefs
			// 
			this.ClientSize = new System.Drawing.Size(384, 211);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.browser);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebBrowserPrefs";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "";
			this.Load += new System.EventHandler(this.FormWebBrowserPrefs_Load);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.WebBrowser browser;
		private System.Windows.Forms.ImageList imageList1;
		private UI.Button butClose;
	}
}