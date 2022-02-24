namespace OpenDental {
	partial class UserControlEmailTemplate {
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
			this.labelPlainText = new System.Windows.Forms.Label();
			this.labelHtml = new System.Windows.Forms.Label();
			this.labelNoHtml = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.webBrowserEmail = new System.Windows.Forms.WebBrowser();
			this.textboxPlainText = new OpenDental.ODtextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelPlainText
			// 
			this.labelPlainText.Location = new System.Drawing.Point(3, 0);
			this.labelPlainText.Name = "labelPlainText";
			this.labelPlainText.Size = new System.Drawing.Size(128, 18);
			this.labelPlainText.TabIndex = 107;
			this.labelPlainText.Text = "Plain Text Body";
			this.labelPlainText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelHtml
			// 
			this.labelHtml.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelHtml.Location = new System.Drawing.Point(433, 0);
			this.labelHtml.Name = "labelHtml";
			this.labelHtml.Size = new System.Drawing.Size(425, 18);
			this.labelHtml.TabIndex = 109;
			this.labelHtml.Text = "HTML ";
			this.labelHtml.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelNoHtml
			// 
			this.labelNoHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNoHtml.Location = new System.Drawing.Point(705, 0);
			this.labelNoHtml.Name = "labelNoHtml";
			this.labelNoHtml.Size = new System.Drawing.Size(162, 18);
			this.labelNoHtml.TabIndex = 112;
			this.labelNoHtml.Text = "No HTML template found";
			this.labelNoHtml.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.webBrowserEmail, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.textboxPlainText, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.labelHtml, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.labelPlainText, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 21);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(861, 245);
			this.tableLayoutPanel1.TabIndex = 114;
			// 
			// webBrowserEmail
			// 
			this.webBrowserEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserEmail.Location = new System.Drawing.Point(433, 28);
			this.webBrowserEmail.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserEmail.Name = "webBrowserEmail";
			this.webBrowserEmail.Size = new System.Drawing.Size(425, 214);
			this.webBrowserEmail.TabIndex = 108;
			// 
			// textboxPlainText
			// 
			this.textboxPlainText.AcceptsTab = true;
			this.textboxPlainText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textboxPlainText.BackColor = System.Drawing.SystemColors.Control;
			this.textboxPlainText.DetectLinksEnabled = false;
			this.textboxPlainText.DetectUrls = false;
			this.textboxPlainText.Location = new System.Drawing.Point(3, 28);
			this.textboxPlainText.Name = "textboxPlainText";
			this.textboxPlainText.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textboxPlainText.ReadOnly = true;
			this.textboxPlainText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textboxPlainText.Size = new System.Drawing.Size(424, 214);
			this.textboxPlainText.TabIndex = 113;
			this.textboxPlainText.Text = "";
			// 
			// UserControlEmailTemplate
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.labelNoHtml);
			this.Name = "UserControlEmailTemplate";
			this.Size = new System.Drawing.Size(870, 269);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelPlainText;
		private System.Windows.Forms.Label labelHtml;
		private System.Windows.Forms.WebBrowser webBrowserEmail;
		private System.Windows.Forms.Label labelNoHtml;
		private ODtextBox textboxPlainText;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
