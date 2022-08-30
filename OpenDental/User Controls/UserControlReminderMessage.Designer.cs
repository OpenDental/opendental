namespace OpenDental {
	partial class UserControlReminderMessage {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.textTemplateSms = new System.Windows.Forms.RichTextBox();
			this.textTemplateSubject = new System.Windows.Forms.RichTextBox();
			this.butEditEmail = new OpenDental.UI.Button();
			this.labelEmail = new System.Windows.Forms.Label();
			this.browserEmailBody = new System.Windows.Forms.WebBrowser();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textTemplateSms
			// 
			this.textTemplateSms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTemplateSms.Location = new System.Drawing.Point(1, 21);
			this.textTemplateSms.Name = "textTemplateSms";
			this.textTemplateSms.Size = new System.Drawing.Size(701, 59);
			this.textTemplateSms.TabIndex = 107;
			this.textTemplateSms.Text = "";
			// 
			// textTemplateSubject
			// 
			this.textTemplateSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTemplateSubject.Location = new System.Drawing.Point(0, 100);
			this.textTemplateSubject.Name = "textTemplateSubject";
			this.textTemplateSubject.Size = new System.Drawing.Size(701, 22);
			this.textTemplateSubject.TabIndex = 113;
			this.textTemplateSubject.Text = "";
			// 
			// butEditEmail
			// 
			this.butEditEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditEmail.Location = new System.Drawing.Point(626, 274);
			this.butEditEmail.Name = "butEditEmail";
			this.butEditEmail.Size = new System.Drawing.Size(75, 26);
			this.butEditEmail.TabIndex = 126;
			this.butEditEmail.Text = "&Edit";
			this.butEditEmail.UseVisualStyleBackColor = true;
			this.butEditEmail.Click += new System.EventHandler(this.butEditEmail_Click);
			// 
			// labelEmail
			// 
			this.labelEmail.Location = new System.Drawing.Point(0, 79);
			this.labelEmail.Name = "labelEmail";
			this.labelEmail.Size = new System.Drawing.Size(163, 18);
			this.labelEmail.TabIndex = 127;
			this.labelEmail.Text = "Email Subject and Body";
			this.labelEmail.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// browserEmailBody
			// 
			this.browserEmailBody.AllowWebBrowserDrop = false;
			this.browserEmailBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.browserEmailBody.Location = new System.Drawing.Point(2, 124);
			this.browserEmailBody.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserEmailBody.Name = "browserEmailBody";
			this.browserEmailBody.Size = new System.Drawing.Size(696, 144);
			this.browserEmailBody.TabIndex = 128;
			this.browserEmailBody.WebBrowserShortcutsEnabled = false;
			this.browserEmailBody.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.browserEmailBody_Navigating);
			this.browserEmailBody.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.browserEmailBody_Navigated);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(163, 18);
			this.label2.TabIndex = 129;
			this.label2.Text = "Text Message";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// UserControlReminderMessage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textTemplateSms);
			this.Controls.Add(this.browserEmailBody);
			this.Controls.Add(this.labelEmail);
			this.Controls.Add(this.textTemplateSubject);
			this.Controls.Add(this.butEditEmail);
			this.MinimumSize = new System.Drawing.Size(702, 303);
			this.Name = "UserControlReminderMessage";
			this.Size = new System.Drawing.Size(705, 303);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.RichTextBox textTemplateSms;
		private UI.Button butEditEmail;
		private System.Windows.Forms.RichTextBox textTemplateSubject;
		private System.Windows.Forms.Label labelEmail;
		private System.Windows.Forms.WebBrowser browserEmailBody;
		private System.Windows.Forms.Label label2;
	}
}
