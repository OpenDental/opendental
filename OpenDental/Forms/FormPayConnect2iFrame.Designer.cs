namespace OpenDental{
	partial class FormPayConnect2iFrame {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayConnect2iFrame));
			this.butClose = new OpenDental.UI.Button();
			this.webBrowserMain = new System.Windows.Forms.WebBrowser();
			this.webViewMain = new CodeBase.Controls.ODWebView2();
			((System.ComponentModel.ISupportInitialize)(this.webViewMain)).BeginInit();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(437, 479);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// webBrowserMain
			// 
			this.webBrowserMain.Location = new System.Drawing.Point(12, 13);
			this.webBrowserMain.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserMain.Name = "webBrowserMain";
			this.webBrowserMain.Size = new System.Drawing.Size(500, 400);
			this.webBrowserMain.TabIndex = 5;
			this.webBrowserMain.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserMain_DocumentCompleted);
			// 
			// webViewMain
			// 
			this.webViewMain.CreationProperties = null;
			this.webViewMain.DefaultBackgroundColor = System.Drawing.Color.White;
			this.webViewMain.Location = new System.Drawing.Point(12, 13);
			this.webViewMain.Name = "webViewMain";
			this.webViewMain.Size = new System.Drawing.Size(500, 400);
			this.webViewMain.TabIndex = 7;
			this.webViewMain.ZoomFactor = 1D;
			// 
			// FormPayConnect2iFrame
			// 
			this.ClientSize = new System.Drawing.Size(524, 515);
			this.Controls.Add(this.webViewMain);
			this.Controls.Add(this.webBrowserMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayConnect2iFrame";
			this.Text = "PayConnect";
			this.Load += new System.EventHandler(this.FormPayConnect2iFrame_Load);
			((System.ComponentModel.ISupportInitialize)(this.webViewMain)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.WebBrowser webBrowserMain;
		private CodeBase.Controls.ODWebView2 webViewMain;
	}
}