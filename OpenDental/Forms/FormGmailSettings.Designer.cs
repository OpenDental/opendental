
namespace OpenDental {
	partial class FormGmailSettings {
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
			this.checkDownloadInbox = new System.Windows.Forms.CheckBox();
			this.labelParams = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textParams = new System.Windows.Forms.TextBox();
			this.checkUnread = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkDownloadInbox
			// 
			this.checkDownloadInbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkDownloadInbox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDownloadInbox.Location = new System.Drawing.Point(18, 147);
			this.checkDownloadInbox.Name = "checkDownloadInbox";
			this.checkDownloadInbox.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkDownloadInbox.Size = new System.Drawing.Size(293, 20);
			this.checkDownloadInbox.TabIndex = 18;
			this.checkDownloadInbox.Text = "Download incoming emails";
			this.checkDownloadInbox.UseVisualStyleBackColor = true;
			this.checkDownloadInbox.CheckedChanged += new System.EventHandler(this.checkDownloadInbox_CheckedChanged);
			// 
			// labelParams
			// 
			this.labelParams.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelParams.Location = new System.Drawing.Point(12, 9);
			this.labelParams.Name = "labelParams";
			this.labelParams.Size = new System.Drawing.Size(304, 20);
			this.labelParams.TabIndex = 20;
			this.labelParams.Text = "Inbox Search Operators for Filtering";
			this.labelParams.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(160, 177);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(241, 177);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textParams
			// 
			this.textParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textParams.Location = new System.Drawing.Point(12, 32);
			this.textParams.Multiline = true;
			this.textParams.Name = "textParams";
			this.textParams.Size = new System.Drawing.Size(304, 87);
			this.textParams.TabIndex = 0;
			this.textParams.Text = "in:inbox";
			// 
			// checkUnread
			// 
			this.checkUnread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkUnread.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUnread.Location = new System.Drawing.Point(18, 125);
			this.checkUnread.Name = "checkUnread";
			this.checkUnread.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkUnread.Size = new System.Drawing.Size(293, 20);
			this.checkUnread.TabIndex = 25;
			this.checkUnread.Text = "Get unread mail only";
			this.checkUnread.UseVisualStyleBackColor = true;
			this.checkUnread.CheckedChanged += new System.EventHandler(this.checkUnread_CheckedChanged);
			// 
			// FormGmailSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(328, 213);
			this.Controls.Add(this.checkUnread);
			this.Controls.Add(this.textParams);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelParams);
			this.Controls.Add(this.checkDownloadInbox);
			this.Name = "FormGmailSettings";
			this.Text = "Gmail Settings";
			this.Load += new System.EventHandler(this.FormGmailSettings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkDownloadInbox;
		private System.Windows.Forms.Label labelParams;
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.TextBox textParams;
		private System.Windows.Forms.CheckBox checkUnread;
	}
}