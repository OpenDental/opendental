namespace OpenDental{
	partial class FormExternalLink {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExternalLink));
			this.butOK = new OpenDental.UI.Button();
			this.butEmptyLink = new OpenDental.UI.Button();
			this.labelDisplayText = new System.Windows.Forms.Label();
			this.textDisplay = new System.Windows.Forms.TextBox();
			this.labelUrl = new System.Windows.Forms.Label();
			this.textURL = new System.Windows.Forms.TextBox();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(475, 96);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butEmptyLink
			// 
			this.butEmptyLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEmptyLink.Location = new System.Drawing.Point(556, 96);
			this.butEmptyLink.Name = "butEmptyLink";
			this.butEmptyLink.Size = new System.Drawing.Size(75, 24);
			this.butEmptyLink.TabIndex = 3;
			this.butEmptyLink.Text = "<a href=\"\">";
			this.butEmptyLink.Click += new System.EventHandler(this.butEmptyLink_Click);
			// 
			// labelDisplayText
			// 
			this.labelDisplayText.Location = new System.Drawing.Point(12, 56);
			this.labelDisplayText.Name = "labelDisplayText";
			this.labelDisplayText.Size = new System.Drawing.Size(122, 18);
			this.labelDisplayText.TabIndex = 23;
			this.labelDisplayText.Text = "Display Text";
			this.labelDisplayText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDisplay
			// 
			this.textDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDisplay.Location = new System.Drawing.Point(136, 55);
			this.textDisplay.Name = "textDisplay";
			this.textDisplay.Size = new System.Drawing.Size(514, 20);
			this.textDisplay.TabIndex = 1;
			// 
			// labelUrl
			// 
			this.labelUrl.Location = new System.Drawing.Point(12, 30);
			this.labelUrl.Name = "labelUrl";
			this.labelUrl.Size = new System.Drawing.Size(122, 18);
			this.labelUrl.TabIndex = 25;
			this.labelUrl.Text = "URL";
			this.labelUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textURL
			// 
			this.textURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textURL.Location = new System.Drawing.Point(136, 29);
			this.textURL.Name = "textURL";
			this.textURL.Size = new System.Drawing.Size(514, 20);
			this.textURL.TabIndex = 0;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(637, 96);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormWikiExternalLink
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(724, 132);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelUrl);
			this.Controls.Add(this.textURL);
			this.Controls.Add(this.labelDisplayText);
			this.Controls.Add(this.textDisplay);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butEmptyLink);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiExternalLink";
			this.Text = "Insert Link to External Resource";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butOK;
		private UI.Button butEmptyLink;
		private System.Windows.Forms.Label labelDisplayText;
		private System.Windows.Forms.TextBox textDisplay;
		private System.Windows.Forms.Label labelUrl;
		private System.Windows.Forms.TextBox textURL;
		private UI.Button butCancel;
	}
}