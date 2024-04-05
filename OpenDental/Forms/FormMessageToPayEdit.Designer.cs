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
			this.butSend = new OpenDental.UI.Button();
			this.textMessage = new System.Windows.Forms.RichTextBox();
			this.radioText = new System.Windows.Forms.RadioButton();
			this.groupCommType = new OpenDental.UI.GroupBox();
			this.labelTextingSignup = new System.Windows.Forms.Label();
			this.radioEmail = new System.Windows.Forms.RadioButton();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.labelMessage = new System.Windows.Forms.Label();
			this.textSubject = new System.Windows.Forms.RichTextBox();
			this.labelSubject = new System.Windows.Forms.Label();
			this.browserEmail = new System.Windows.Forms.WebBrowser();
			this.butEdit = new OpenDental.UI.Button();
			this.groupCommType.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(463, 468);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 3;
			this.butSend.Text = "&Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// textMessage
			// 
			this.textMessage.AcceptsTab = true;
			this.textMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMessage.BackColor = System.Drawing.SystemColors.Window;
			this.textMessage.Location = new System.Drawing.Point(12, 192);
			this.textMessage.Name = "textMessage";
			this.textMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMessage.Size = new System.Drawing.Size(529, 248);
			this.textMessage.TabIndex = 4;
			this.textMessage.Text = "";
			// 
			// radioText
			// 
			this.radioText.Location = new System.Drawing.Point(15, 22);
			this.radioText.Name = "radioText";
			this.radioText.Size = new System.Drawing.Size(74, 17);
			this.radioText.TabIndex = 5;
			this.radioText.TabStop = true;
			this.radioText.Text = "Text";
			this.radioText.UseVisualStyleBackColor = true;
			this.radioText.CheckedChanged += new System.EventHandler(this.radioMessageType_CheckedChanged);
			// 
			// groupCommType
			// 
			this.groupCommType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupCommType.Controls.Add(this.labelTextingSignup);
			this.groupCommType.Controls.Add(this.radioEmail);
			this.groupCommType.Controls.Add(this.radioText);
			this.groupCommType.Location = new System.Drawing.Point(12, 38);
			this.groupCommType.Name = "groupCommType";
			this.groupCommType.Size = new System.Drawing.Size(529, 73);
			this.groupCommType.TabIndex = 6;
			this.groupCommType.Text = "Message Type";
			// 
			// labelTextingSignup
			// 
			this.labelTextingSignup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTextingSignup.Location = new System.Drawing.Point(138, 22);
			this.labelTextingSignup.Name = "labelTextingSignup";
			this.labelTextingSignup.Size = new System.Drawing.Size(388, 17);
			this.labelTextingSignup.TabIndex = 11;
			this.labelTextingSignup.Text = "Sign up for Integrated Texting to send this message as a text message\r\n";
			this.labelTextingSignup.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// radioEmail
			// 
			this.radioEmail.Location = new System.Drawing.Point(15, 45);
			this.radioEmail.Name = "radioEmail";
			this.radioEmail.Size = new System.Drawing.Size(85, 17);
			this.radioEmail.TabIndex = 6;
			this.radioEmail.TabStop = true;
			this.radioEmail.Text = "Email";
			this.radioEmail.UseVisualStyleBackColor = true;
			this.radioEmail.CheckedChanged += new System.EventHandler(this.radioMessageType_CheckedChanged);
			// 
			// textPatient
			// 
			this.textPatient.AcceptsTab = true;
			this.textPatient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPatient.BackColor = System.Drawing.SystemColors.Control;
			this.textPatient.Location = new System.Drawing.Point(12, 12);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textPatient.Size = new System.Drawing.Size(529, 20);
			this.textPatient.TabIndex = 7;
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(12, 176);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(121, 13);
			this.labelMessage.TabIndex = 8;
			this.labelMessage.Text = "Message Text";
			// 
			// textSubject
			// 
			this.textSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSubject.Location = new System.Drawing.Point(12, 145);
			this.textSubject.Name = "textSubject";
			this.textSubject.Size = new System.Drawing.Size(529, 20);
			this.textSubject.TabIndex = 120;
			this.textSubject.Text = "";
			// 
			// labelSubject
			// 
			this.labelSubject.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelSubject.Location = new System.Drawing.Point(12, 124);
			this.labelSubject.Name = "labelSubject";
			this.labelSubject.Size = new System.Drawing.Size(203, 18);
			this.labelSubject.TabIndex = 119;
			this.labelSubject.Text = "Email Subject";
			this.labelSubject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// browserEmail
			// 
			this.browserEmail.Location = new System.Drawing.Point(12, 192);
			this.browserEmail.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserEmail.Name = "browserEmail";
			this.browserEmail.Size = new System.Drawing.Size(526, 261);
			this.browserEmail.TabIndex = 121;
			this.browserEmail.Visible = false;
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEdit.Location = new System.Drawing.Point(12, 468);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(132, 24);
			this.butEdit.TabIndex = 122;
			this.butEdit.Text = "&Edit Email Message";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// FormMessageToPayEdit
			// 
			this.ClientSize = new System.Drawing.Size(553, 504);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.browserEmail);
			this.Controls.Add(this.textSubject);
			this.Controls.Add(this.labelSubject);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.groupCommType);
			this.Controls.Add(this.textMessage);
			this.Controls.Add(this.butSend);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMessageToPayEdit";
			this.Text = "Message-to-Pay Edit";
			this.Load += new System.EventHandler(this.FormMessageToPayEdit_Load);
			this.groupCommType.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSend;
		private System.Windows.Forms.RichTextBox textMessage;
		private System.Windows.Forms.RadioButton radioText;
		private UI.GroupBox groupCommType;
		private System.Windows.Forms.RadioButton radioEmail;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.RichTextBox textSubject;
		private System.Windows.Forms.Label labelSubject;
		private System.Windows.Forms.WebBrowser browserEmail;
		private UI.Button butEdit;
		private System.Windows.Forms.Label labelTextingSignup;
	}
}