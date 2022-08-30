namespace OpenDental{
	partial class FormLoginFailed {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLoginFailed));
			this.labelErrMsg = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.textUser = new System.Windows.Forms.TextBox();
			this.butLogin = new OpenDental.UI.Button();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.butExit = new OpenDental.UI.Button();
			this.labelPassword = new System.Windows.Forms.Label();
			this.labelUser = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelErrMsg
			// 
			this.labelErrMsg.Location = new System.Drawing.Point(12, 12);
			this.labelErrMsg.Name = "labelErrMsg";
			this.labelErrMsg.Size = new System.Drawing.Size(376, 39);
			this.labelErrMsg.TabIndex = 0;
			this.labelErrMsg.Text = "Error Message";
			this.labelErrMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.textUser);
			this.panel1.Controls.Add(this.butLogin);
			this.panel1.Controls.Add(this.textPassword);
			this.panel1.Controls.Add(this.butExit);
			this.panel1.Controls.Add(this.labelPassword);
			this.panel1.Controls.Add(this.labelUser);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(400, 150);
			this.panel1.TabIndex = 0;
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(12, 76);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(155, 20);
			this.textUser.TabIndex = 1;
			// 
			// butLogin
			// 
			this.butLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butLogin.Location = new System.Drawing.Point(232, 114);
			this.butLogin.Name = "butLogin";
			this.butLogin.Size = new System.Drawing.Size(75, 24);
			this.butLogin.TabIndex = 3;
			this.butLogin.Text = "Login";
			this.butLogin.Click += new System.EventHandler(this.butLogin_Click);
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(173, 76);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(215, 20);
			this.textPassword.TabIndex = 2;
			// 
			// butExit
			// 
			this.butExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butExit.Location = new System.Drawing.Point(313, 114);
			this.butExit.Name = "butExit";
			this.butExit.Size = new System.Drawing.Size(75, 24);
			this.butExit.TabIndex = 4;
			this.butExit.Text = "Exit Program";
			this.butExit.Click += new System.EventHandler(this.butExit_Click);
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(173, 57);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(215, 18);
			this.labelPassword.TabIndex = 0;
			this.labelPassword.Text = "Password";
			this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelUser
			// 
			this.labelUser.Location = new System.Drawing.Point(12, 57);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(155, 18);
			this.labelUser.TabIndex = 0;
			this.labelUser.Text = "User";
			this.labelUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormLoginFailed
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(400, 150);
			this.Controls.Add(this.labelErrMsg);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLoginFailed";
			this.ShowInTaskbar = false;
			this.Text = "Login Failed";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FormLoginFailed_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butLogin;
		private OpenDental.UI.Button butExit;
		private System.Windows.Forms.Label labelErrMsg;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Label labelUser;
	}
}