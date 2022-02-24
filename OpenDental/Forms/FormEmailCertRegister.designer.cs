namespace OpenDental{
	partial class FormEmailCertRegister {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailCertRegister));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupStep1 = new System.Windows.Forms.GroupBox();
			this.textEmailAddress = new System.Windows.Forms.TextBox();
			this.labelEmailAddress = new System.Windows.Forms.Label();
			this.butSendCode = new OpenDental.UI.Button();
			this.groupStep2 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelVerificationCode = new System.Windows.Forms.Label();
			this.textVerificationCode = new System.Windows.Forms.TextBox();
			this.groupStep3 = new System.Windows.Forms.GroupBox();
			this.butBrowse = new OpenDental.UI.Button();
			this.labelCertFilePath = new System.Windows.Forms.Label();
			this.textCertFilePath = new System.Windows.Forms.TextBox();
			this.openFileDialogCert = new System.Windows.Forms.OpenFileDialog();
			this.groupStep1.SuspendLayout();
			this.groupStep2.SuspendLayout();
			this.groupStep3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(416, 265);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(497, 265);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupStep1
			// 
			this.groupStep1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupStep1.Controls.Add(this.butSendCode);
			this.groupStep1.Controls.Add(this.labelEmailAddress);
			this.groupStep1.Controls.Add(this.textEmailAddress);
			this.groupStep1.Location = new System.Drawing.Point(18, 64);
			this.groupStep1.Name = "groupStep1";
			this.groupStep1.Size = new System.Drawing.Size(557, 44);
			this.groupStep1.TabIndex = 4;
			this.groupStep1.TabStop = false;
			this.groupStep1.Text = "Step 1 - Send Verification Code";
			// 
			// textEmailAddress
			// 
			this.textEmailAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEmailAddress.Location = new System.Drawing.Point(138, 16);
			this.textEmailAddress.Name = "textEmailAddress";
			this.textEmailAddress.Size = new System.Drawing.Size(332, 20);
			this.textEmailAddress.TabIndex = 0;
			// 
			// labelEmailAddress
			// 
			this.labelEmailAddress.Location = new System.Drawing.Point(9, 16);
			this.labelEmailAddress.Name = "labelEmailAddress";
			this.labelEmailAddress.Size = new System.Drawing.Size(128, 20);
			this.labelEmailAddress.TabIndex = 6;
			this.labelEmailAddress.Text = "Email Address";
			this.labelEmailAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSendCode
			// 
			this.butSendCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSendCode.Location = new System.Drawing.Point(476, 14);
			this.butSendCode.Name = "butSendCode";
			this.butSendCode.Size = new System.Drawing.Size(75, 24);
			this.butSendCode.TabIndex = 5;
			this.butSendCode.Text = "Send Code";
			this.butSendCode.Click += new System.EventHandler(this.butSendCode_Click);
			// 
			// groupStep2
			// 
			this.groupStep2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupStep2.Controls.Add(this.labelVerificationCode);
			this.groupStep2.Controls.Add(this.textVerificationCode);
			this.groupStep2.Location = new System.Drawing.Point(18, 124);
			this.groupStep2.Name = "groupStep2";
			this.groupStep2.Size = new System.Drawing.Size(557, 44);
			this.groupStep2.TabIndex = 5;
			this.groupStep2.TabStop = false;
			this.groupStep2.Text = "Step 2 - Enter Verification Code from Received Email";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(15, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(557, 45);
			this.label2.TabIndex = 7;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// labelVerificationCode
			// 
			this.labelVerificationCode.Location = new System.Drawing.Point(6, 19);
			this.labelVerificationCode.Name = "labelVerificationCode";
			this.labelVerificationCode.Size = new System.Drawing.Size(131, 20);
			this.labelVerificationCode.TabIndex = 8;
			this.labelVerificationCode.Text = "Verification Code";
			this.labelVerificationCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVerificationCode
			// 
			this.textVerificationCode.Location = new System.Drawing.Point(138, 19);
			this.textVerificationCode.Name = "textVerificationCode";
			this.textVerificationCode.Size = new System.Drawing.Size(100, 20);
			this.textVerificationCode.TabIndex = 7;
			// 
			// groupStep3
			// 
			this.groupStep3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupStep3.Controls.Add(this.butBrowse);
			this.groupStep3.Controls.Add(this.labelCertFilePath);
			this.groupStep3.Controls.Add(this.textCertFilePath);
			this.groupStep3.Location = new System.Drawing.Point(18, 184);
			this.groupStep3.Name = "groupStep3";
			this.groupStep3.Size = new System.Drawing.Size(557, 44);
			this.groupStep3.TabIndex = 8;
			this.groupStep3.TabStop = false;
			this.groupStep3.Text = "Step 3 - Pick Certificate File";
			// 
			// butBrowse
			// 
			this.butBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBrowse.Location = new System.Drawing.Point(530, 16);
			this.butBrowse.Name = "butBrowse";
			this.butBrowse.Size = new System.Drawing.Size(21, 20);
			this.butBrowse.TabIndex = 8;
			this.butBrowse.Text = "...";
			this.butBrowse.Click += new System.EventHandler(this.butBrowse_Click);
			// 
			// labelCertFilePath
			// 
			this.labelCertFilePath.Location = new System.Drawing.Point(9, 16);
			this.labelCertFilePath.Name = "labelCertFilePath";
			this.labelCertFilePath.Size = new System.Drawing.Size(128, 20);
			this.labelCertFilePath.TabIndex = 9;
			this.labelCertFilePath.Text = "Certificate File Path";
			this.labelCertFilePath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCertFilePath
			// 
			this.textCertFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textCertFilePath.Location = new System.Drawing.Point(138, 16);
			this.textCertFilePath.Name = "textCertFilePath";
			this.textCertFilePath.Size = new System.Drawing.Size(386, 20);
			this.textCertFilePath.TabIndex = 7;
			// 
			// openFileDialogCert
			// 
			this.openFileDialogCert.DefaultExt = "cer";
			this.openFileDialogCert.Filter = "Public Email Certificate|*.cer";
			// 
			// FormEmailCertRegister
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(584, 301);
			this.Controls.Add(this.groupStep3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupStep2);
			this.Controls.Add(this.groupStep1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailCertRegister";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Email Certificate Registration";
			this.groupStep1.ResumeLayout(false);
			this.groupStep1.PerformLayout();
			this.groupStep2.ResumeLayout(false);
			this.groupStep2.PerformLayout();
			this.groupStep3.ResumeLayout(false);
			this.groupStep3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupStep1;
		private System.Windows.Forms.Label labelEmailAddress;
		private System.Windows.Forms.TextBox textEmailAddress;
		private UI.Button butSendCode;
		private System.Windows.Forms.GroupBox groupStep2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelVerificationCode;
		private System.Windows.Forms.TextBox textVerificationCode;
		private System.Windows.Forms.GroupBox groupStep3;
		private UI.Button butBrowse;
		private System.Windows.Forms.Label labelCertFilePath;
		private System.Windows.Forms.TextBox textCertFilePath;
		private System.Windows.Forms.OpenFileDialog openFileDialogCert;
	}
}