namespace OpenDental{
	partial class FormMessageToPayEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessageToPayEdit));
			this.groupStatementType = new OpenDental.UI.GroupBox();
			this.radioFamily = new System.Windows.Forms.RadioButton();
			this.radioPatient = new System.Windows.Forms.RadioButton();
			this.butEditEmail = new OpenDental.UI.Button();
			this.browserEmail = new System.Windows.Forms.WebBrowser();
			this.textSubject = new System.Windows.Forms.RichTextBox();
			this.labelSubject = new System.Windows.Forms.Label();
			this.textMessage = new System.Windows.Forms.RichTextBox();
			this.butSend = new OpenDental.UI.Button();
			this.butPreview = new OpenDental.UI.Button();
			this.groupEmail = new OpenDental.UI.GroupBox();
			this.checkEmail = new OpenDental.UI.CheckBox();
			this.checkText = new OpenDental.UI.CheckBox();
			this.groupStatementType.SuspendLayout();
			this.groupEmail.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupStatementType
			// 
			this.groupStatementType.Controls.Add(this.radioFamily);
			this.groupStatementType.Controls.Add(this.radioPatient);
			this.groupStatementType.Location = new System.Drawing.Point(12, 12);
			this.groupStatementType.Name = "groupStatementType";
			this.groupStatementType.Size = new System.Drawing.Size(326, 47);
			this.groupStatementType.TabIndex = 123;
			this.groupStatementType.Text = "Statement Type";
			// 
			// radioFamily
			// 
			this.radioFamily.Location = new System.Drawing.Point(15, 22);
			this.radioFamily.Name = "radioFamily";
			this.radioFamily.Size = new System.Drawing.Size(121, 17);
			this.radioFamily.TabIndex = 14;
			this.radioFamily.TabStop = true;
			this.radioFamily.Text = "Family";
			this.radioFamily.UseVisualStyleBackColor = true;
			// 
			// radioPatient
			// 
			this.radioPatient.Location = new System.Drawing.Point(178, 22);
			this.radioPatient.Name = "radioPatient";
			this.radioPatient.Size = new System.Drawing.Size(121, 17);
			this.radioPatient.TabIndex = 13;
			this.radioPatient.TabStop = true;
			this.radioPatient.Text = "Patient Only";
			this.radioPatient.UseVisualStyleBackColor = true;
			// 
			// butEditEmail
			// 
			this.butEditEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditEmail.Location = new System.Drawing.Point(12, 307);
			this.butEditEmail.Name = "butEditEmail";
			this.butEditEmail.Size = new System.Drawing.Size(116, 24);
			this.butEditEmail.TabIndex = 122;
			this.butEditEmail.Text = "&Edit Email Message";
			this.butEditEmail.Click += new System.EventHandler(this.butEditEmail_Click);
			// 
			// browserEmail
			// 
			this.browserEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.browserEmail.Location = new System.Drawing.Point(9, 23);
			this.browserEmail.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserEmail.Name = "browserEmail";
			this.browserEmail.Size = new System.Drawing.Size(314, 186);
			this.browserEmail.TabIndex = 121;
			// 
			// textSubject
			// 
			this.textSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSubject.Location = new System.Drawing.Point(70, 3);
			this.textSubject.Name = "textSubject";
			this.textSubject.Size = new System.Drawing.Size(253, 20);
			this.textSubject.TabIndex = 120;
			this.textSubject.Text = "";
			// 
			// labelSubject
			// 
			this.labelSubject.Location = new System.Drawing.Point(3, 3);
			this.labelSubject.Name = "labelSubject";
			this.labelSubject.Size = new System.Drawing.Size(61, 20);
			this.labelSubject.TabIndex = 119;
			this.labelSubject.Text = "Subject";
			this.labelSubject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textMessage
			// 
			this.textMessage.AcceptsTab = true;
			this.textMessage.BackColor = System.Drawing.SystemColors.Window;
			this.textMessage.Location = new System.Drawing.Point(344, 89);
			this.textMessage.Name = "textMessage";
			this.textMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMessage.Size = new System.Drawing.Size(191, 212);
			this.textMessage.TabIndex = 4;
			this.textMessage.Text = "";
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(459, 307);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 3;
			this.butSend.Text = "&Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butPreview
			// 
			this.butPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPreview.Location = new System.Drawing.Point(343, 307);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(110, 24);
			this.butPreview.TabIndex = 126;
			this.butPreview.Text = "&Preview Statement";
			this.butPreview.UseVisualStyleBackColor = true;
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// groupEmail
			// 
			this.groupEmail.Controls.Add(this.browserEmail);
			this.groupEmail.Controls.Add(this.labelSubject);
			this.groupEmail.Controls.Add(this.textSubject);
			this.groupEmail.Location = new System.Drawing.Point(12, 89);
			this.groupEmail.Name = "groupEmail";
			this.groupEmail.Size = new System.Drawing.Size(326, 212);
			this.groupEmail.TabIndex = 127;
			this.groupEmail.Text = "";
			// 
			// checkEmail
			// 
			this.checkEmail.Location = new System.Drawing.Point(12, 65);
			this.checkEmail.Name = "checkEmail";
			this.checkEmail.Size = new System.Drawing.Size(120, 18);
			this.checkEmail.TabIndex = 128;
			this.checkEmail.Text = "Send Email";
			this.checkEmail.Click += new System.EventHandler(this.checkEmail_Click);
			// 
			// checkText
			// 
			this.checkText.Location = new System.Drawing.Point(344, 65);
			this.checkText.Name = "checkText";
			this.checkText.Size = new System.Drawing.Size(120, 18);
			this.checkText.TabIndex = 129;
			this.checkText.Text = "Send Text";
			this.checkText.Click += new System.EventHandler(this.checkText_Click);
			// 
			// FormMessageToPayEdit
			// 
			this.ClientSize = new System.Drawing.Size(546, 343);
			this.Controls.Add(this.checkText);
			this.Controls.Add(this.checkEmail);
			this.Controls.Add(this.groupEmail);
			this.Controls.Add(this.butEditEmail);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.groupStatementType);
			this.Controls.Add(this.textMessage);
			this.Controls.Add(this.butSend);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMessageToPayEdit";
			this.Text = "Message-to-Pay";
			this.Load += new System.EventHandler(this.FormMessageToPayEdit_Load);
			this.Shown += new System.EventHandler(this.FormMessageToPayEdit_Shown);
			this.groupStatementType.ResumeLayout(false);
			this.groupEmail.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSend;
		private System.Windows.Forms.RichTextBox textMessage;
		private System.Windows.Forms.RichTextBox textSubject;
		private System.Windows.Forms.Label labelSubject;
		private System.Windows.Forms.WebBrowser browserEmail;
		private UI.Button butEditEmail;
		private UI.GroupBox groupStatementType;
		private System.Windows.Forms.RadioButton radioFamily;
		private System.Windows.Forms.RadioButton radioPatient;
		private UI.Button butPreview;
		private UI.GroupBox groupEmail;
		private UI.CheckBox checkEmail;
		private UI.CheckBox checkText;
	}
}