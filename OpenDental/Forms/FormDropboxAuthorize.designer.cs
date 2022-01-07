namespace OpenDental{
	partial class FormDropboxAuthorize {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDropboxAuthorize));
			this.labelAuthorizeInfo = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.textAccessToken = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// labelAuthorizeInfo
			// 
			this.labelAuthorizeInfo.Location = new System.Drawing.Point(13, 9);
			this.labelAuthorizeInfo.Name = "labelAuthorizeInfo";
			this.labelAuthorizeInfo.Size = new System.Drawing.Size(499, 43);
			this.labelAuthorizeInfo.TabIndex = 80;
			this.labelAuthorizeInfo.Text = "A browser should open to authorize your account with Dropbox.  Please enter the c" +
    "ode below once prompted to by Dropbox.";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(438, 78);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76, 23);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(12, 81);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(133, 19);
			this.label1.TabIndex = 84;
			this.label1.Text = "Enter Access Token";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(357, 78);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76, 23);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textAccessToken
			// 
			this.textAccessToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textAccessToken.Location = new System.Drawing.Point(145, 80);
			this.textAccessToken.Name = "textAccessToken";
			this.textAccessToken.Size = new System.Drawing.Size(199, 20);
			this.textAccessToken.TabIndex = 4;
			// 
			// FormDropboxAuthorize
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(524, 109);
			this.Controls.Add(this.labelAuthorizeInfo);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textAccessToken);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDropboxAuthorize";
			this.Text = "Authorize Dropbox";
			this.Load += new System.EventHandler(this.FormDropboxAuthorize_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelAuthorizeInfo;
		private UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private UI.Button butOK;
		private System.Windows.Forms.TextBox textAccessToken;
	}
}