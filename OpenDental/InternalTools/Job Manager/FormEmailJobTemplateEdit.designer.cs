namespace OpenDental {
	partial class FormEmailJobTemplateEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailJobTemplateEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textPledgeTemplate = new OpenDental.ODtextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBodyTextTemplate = new OpenDental.ODtextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(705, 258);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(786, 258);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textPledgeTemplate
			// 
			this.textPledgeTemplate.AcceptsTab = true;
			this.textPledgeTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPledgeTemplate.BackColor = System.Drawing.SystemColors.Window;
			this.textPledgeTemplate.DetectUrls = false;
			this.textPledgeTemplate.Location = new System.Drawing.Point(152, 206);
			this.textPledgeTemplate.Name = "textPledgeTemplate";
			this.textPledgeTemplate.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textPledgeTemplate.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPledgeTemplate.Size = new System.Drawing.Size(709, 46);
			this.textPledgeTemplate.TabIndex = 236;
			this.textPledgeTemplate.Text = "";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(12, 208);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(134, 14);
			this.label4.TabIndex = 237;
			this.label4.Text = "Pledge Paragraph:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBodyTextTemplate
			// 
			this.textBodyTextTemplate.AcceptsTab = true;
			this.textBodyTextTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBodyTextTemplate.BackColor = System.Drawing.SystemColors.Window;
			this.textBodyTextTemplate.DetectUrls = false;
			this.textBodyTextTemplate.Location = new System.Drawing.Point(152, 12);
			this.textBodyTextTemplate.Name = "textBodyTextTemplate";
			this.textBodyTextTemplate.QuickPasteType = OpenDentBusiness.QuickPasteType.Email;
			this.textBodyTextTemplate.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textBodyTextTemplate.Size = new System.Drawing.Size(709, 188);
			this.textBodyTextTemplate.TabIndex = 235;
			this.textBodyTextTemplate.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(134, 14);
			this.label2.TabIndex = 234;
			this.label2.Text = "Subject:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEmailJobTemplateEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(873, 294);
			this.Controls.Add(this.textPledgeTemplate);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBodyTextTemplate);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailJobTemplateEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Job Email Templates";
			this.Load += new System.EventHandler(this.FormEmailJobTemplateEdit_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ODtextBox textPledgeTemplate;
		private System.Windows.Forms.Label label4;
		private ODtextBox textBodyTextTemplate;
		private System.Windows.Forms.Label label2;
	}
}