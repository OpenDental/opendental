namespace OpenDental{
	partial class FormEmailSetupEHR {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailSetupEHR));
			this.label6 = new System.Windows.Forms.Label();
			this.textPort = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textPOPserver = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(203, 196);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(156, 20);
			this.label6.TabIndex = 38;
			this.label6.Text = "Usually 110";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPort
			// 
			this.textPort.Location = new System.Drawing.Point(141, 196);
			this.textPort.Name = "textPort";
			this.textPort.Size = new System.Drawing.Size(56, 20);
			this.textPort.TabIndex = 36;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(28, 199);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(111, 20);
			this.label5.TabIndex = 37;
			this.label5.Text = "Port";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(141, 170);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(218, 20);
			this.textPassword.TabIndex = 34;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(25, 173);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(114, 20);
			this.label4.TabIndex = 35;
			this.label4.Text = "Password";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(141, 144);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(218, 20);
			this.textUsername.TabIndex = 32;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(22, 147);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(117, 20);
			this.label3.TabIndex = 33;
			this.label3.Text = "Username";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(143, 64);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(198, 74);
			this.textBox1.TabIndex = 31;
			this.textBox1.Text = "pop.secureserver.net\r\npop.gmail.com\r\npop3.live.com\r\nor similar...";
			// 
			// textPOPserver
			// 
			this.textPOPserver.Location = new System.Drawing.Point(141, 41);
			this.textPOPserver.Name = "textPOPserver";
			this.textPOPserver.Size = new System.Drawing.Size(218, 20);
			this.textPOPserver.TabIndex = 26;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(31, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(110, 16);
			this.label1.TabIndex = 28;
			this.label1.Text = "POP3 Server";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(372, 260);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(372, 301);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEmailSetupEHR
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(472, 352);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textPort);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textUsername);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.textPOPserver);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailSetupEHR";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "EHR Inbound Email Setup ";
			this.Load += new System.EventHandler(this.FormEmailSetupEHR_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textPort;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textUsername;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textPOPserver;
		private System.Windows.Forms.Label label1;
	}
}