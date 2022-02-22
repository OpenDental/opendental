namespace OpenDental {
	partial class FormEmailHostingSignatures {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailHostingSignatures));
			this.butOK = new OpenDental.UI.Button();
			this.butEditSignature = new OpenDental.UI.Button();
			this.labelHtml = new System.Windows.Forms.Label();
			this.webBrowserSignature = new System.Windows.Forms.WebBrowser();
			this.labelPlainText = new System.Windows.Forms.Label();
			this.comboClinicMassEmail = new OpenDental.UI.ComboBoxClinicPicker();
			this.label1 = new System.Windows.Forms.Label();
			this.textboxPlainText = new OpenDental.ODtextBox();
			this.labelNotActivated = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butOK.Location = new System.Drawing.Point(809, 304);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&Close";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butEditSignature
			// 
			this.butEditSignature.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditSignature.Image = global::OpenDental.Properties.Resources.editPencil;
			this.butEditSignature.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditSignature.Location = new System.Drawing.Point(748, 39);
			this.butEditSignature.Name = "butEditSignature";
			this.butEditSignature.Size = new System.Drawing.Size(136, 26);
			this.butEditSignature.TabIndex = 321;
			this.butEditSignature.Text = "Edit HTML Signature";
			this.butEditSignature.Click += new System.EventHandler(this.butEditSignature_Click);
			// 
			// labelHtml
			// 
			this.labelHtml.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelHtml.Location = new System.Drawing.Point(456, 47);
			this.labelHtml.Name = "labelHtml";
			this.labelHtml.Size = new System.Drawing.Size(88, 18);
			this.labelHtml.TabIndex = 320;
			this.labelHtml.Text = "HTML Signature";
			this.labelHtml.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// webBrowserSignature
			// 
			this.webBrowserSignature.AllowNavigation = false;
			this.webBrowserSignature.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserSignature.Location = new System.Drawing.Point(459, 72);
			this.webBrowserSignature.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserSignature.Name = "webBrowserSignature";
			this.webBrowserSignature.Size = new System.Drawing.Size(425, 226);
			this.webBrowserSignature.TabIndex = 317;
			// 
			// labelPlainText
			// 
			this.labelPlainText.Location = new System.Drawing.Point(11, 47);
			this.labelPlainText.Name = "labelPlainText";
			this.labelPlainText.Size = new System.Drawing.Size(425, 18);
			this.labelPlainText.TabIndex = 319;
			this.labelPlainText.Text = "Plain Text Signature";
			this.labelPlainText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboClinicMassEmail
			// 
			this.comboClinicMassEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinicMassEmail.HqDescription = "Headquarters";
			this.comboClinicMassEmail.IncludeUnassigned = true;
			this.comboClinicMassEmail.Location = new System.Drawing.Point(670, 12);
			this.comboClinicMassEmail.Name = "comboClinicMassEmail";
			this.comboClinicMassEmail.Size = new System.Drawing.Size(214, 21);
			this.comboClinicMassEmail.TabIndex = 322;
			this.comboClinicMassEmail.SelectionChangeCommitted += new System.EventHandler(this.comboClinicMassEmail_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(444, 23);
			this.label1.TabIndex = 323;
			this.label1.Text = "Signature will be included at the bottom of Mass Email and notifications.";
			// 
			// textboxPlainText
			// 
			this.textboxPlainText.AcceptsTab = true;
			this.textboxPlainText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textboxPlainText.BackColor = System.Drawing.SystemColors.Window;
			this.textboxPlainText.DetectLinksEnabled = false;
			this.textboxPlainText.DetectUrls = false;
			this.textboxPlainText.Location = new System.Drawing.Point(12, 72);
			this.textboxPlainText.Name = "textboxPlainText";
			this.textboxPlainText.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textboxPlainText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textboxPlainText.Size = new System.Drawing.Size(424, 226);
			this.textboxPlainText.TabIndex = 318;
			this.textboxPlainText.Text = "";
			// 
			// labelNotActivated
			// 
			this.labelNotActivated.ForeColor = System.Drawing.Color.Red;
			this.labelNotActivated.Location = new System.Drawing.Point(510, 14);
			this.labelNotActivated.Name = "labelNotActivated";
			this.labelNotActivated.Size = new System.Drawing.Size(163, 17);
			this.labelNotActivated.TabIndex = 324;
			this.labelNotActivated.Text = "* Clinic is not signed up";
			this.labelNotActivated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEmailHostingSignatures
			// 
			this.CancelButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(893, 335);
			this.Controls.Add(this.labelNotActivated);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboClinicMassEmail);
			this.Controls.Add(this.butEditSignature);
			this.Controls.Add(this.labelHtml);
			this.Controls.Add(this.webBrowserSignature);
			this.Controls.Add(this.labelPlainText);
			this.Controls.Add(this.textboxPlainText);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailHostingSignatures";
			this.Text = "Hosted Email Signatures";
			this.Load += new System.EventHandler(this.FormEmailHostingSignatures_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private UI.Button butEditSignature;
		private System.Windows.Forms.Label labelHtml;
		private System.Windows.Forms.WebBrowser webBrowserSignature;
		private System.Windows.Forms.Label labelPlainText;
		private ODtextBox textboxPlainText;
		private UI.ComboBoxClinicPicker comboClinicMassEmail;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelNotActivated;
	}
}