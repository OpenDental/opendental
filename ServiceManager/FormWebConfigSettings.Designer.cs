namespace ServiceManager {
	partial class FormWebConfigSettings {
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
			this.butCancel = new System.Windows.Forms.Button();
			this.butOk = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboLogLevel = new System.Windows.Forms.ComboBox();
			this.textPasswordLow = new System.Windows.Forms.TextBox();
			this.textUserLow = new System.Windows.Forms.TextBox();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.textUser = new System.Windows.Forms.TextBox();
			this.textDatabase = new System.Windows.Forms.TextBox();
			this.textServer = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.comboDatabaseType = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(292, 263);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(211, 263);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 23);
			this.butOk.TabIndex = 1;
			this.butOk.Text = "OK";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.comboDatabaseType);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.comboLogLevel);
			this.groupBox1.Controls.Add(this.textPasswordLow);
			this.groupBox1.Controls.Add(this.textUserLow);
			this.groupBox1.Controls.Add(this.textPassword);
			this.groupBox1.Controls.Add(this.textUser);
			this.groupBox1.Controls.Add(this.textDatabase);
			this.groupBox1.Controls.Add(this.textServer);
			this.groupBox1.Location = new System.Drawing.Point(27, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(324, 235);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Connection Settings";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(9, 202);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(119, 18);
			this.label7.TabIndex = 13;
			this.label7.Text = "Log Level";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 149);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(122, 18);
			this.label6.TabIndex = 12;
			this.label6.Text = "PasswordLow";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(9, 123);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(119, 18);
			this.label5.TabIndex = 11;
			this.label5.Text = "UserLow";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 97);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(122, 18);
			this.label4.TabIndex = 10;
			this.label4.Text = "Password";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 71);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(122, 18);
			this.label3.TabIndex = 9;
			this.label3.Text = "User";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(116, 18);
			this.label2.TabIndex = 8;
			this.label2.Text = "Database";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 18);
			this.label1.TabIndex = 7;
			this.label1.Text = "Server";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboLogLevel
			// 
			this.comboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLogLevel.FormattingEnabled = true;
			this.comboLogLevel.Location = new System.Drawing.Point(129, 202);
			this.comboLogLevel.Name = "comboLogLevel";
			this.comboLogLevel.Size = new System.Drawing.Size(189, 21);
			this.comboLogLevel.TabIndex = 6;
			// 
			// textPasswordLow
			// 
			this.textPasswordLow.Location = new System.Drawing.Point(129, 149);
			this.textPasswordLow.Name = "textPasswordLow";
			this.textPasswordLow.Size = new System.Drawing.Size(189, 20);
			this.textPasswordLow.TabIndex = 5;
			// 
			// textUserLow
			// 
			this.textUserLow.Location = new System.Drawing.Point(129, 123);
			this.textUserLow.Name = "textUserLow";
			this.textUserLow.Size = new System.Drawing.Size(189, 20);
			this.textUserLow.TabIndex = 4;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(129, 97);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(189, 20);
			this.textPassword.TabIndex = 3;
			this.textPassword.TextChanged += new System.EventHandler(this.textPassword_TextChanged);
			this.textPassword.Leave += new System.EventHandler(this.textPassword_Leave);
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(129, 71);
			this.textUser.Name = "textUser";
			this.textUser.Size = new System.Drawing.Size(189, 20);
			this.textUser.TabIndex = 2;
			// 
			// textDatabase
			// 
			this.textDatabase.Location = new System.Drawing.Point(129, 45);
			this.textDatabase.Name = "textDatabase";
			this.textDatabase.Size = new System.Drawing.Size(189, 20);
			this.textDatabase.TabIndex = 1;
			// 
			// textServer
			// 
			this.textServer.Location = new System.Drawing.Point(129, 19);
			this.textServer.Name = "textServer";
			this.textServer.Size = new System.Drawing.Size(189, 20);
			this.textServer.TabIndex = 0;
			this.textServer.Text = "localhost";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(9, 175);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(119, 18);
			this.label8.TabIndex = 15;
			this.label8.Text = "Database Type";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDatabaseType
			// 
			this.comboDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDatabaseType.FormattingEnabled = true;
			this.comboDatabaseType.Location = new System.Drawing.Point(129, 175);
			this.comboDatabaseType.Name = "comboDatabaseType";
			this.comboDatabaseType.Size = new System.Drawing.Size(189, 21);
			this.comboDatabaseType.TabIndex = 14;
			// 
			// FormWebConfigSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(379, 298);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Name = "FormWebConfigSettings";
			this.Text = "OpenDentalWebConfig.xml Settings";
			this.Load += new System.EventHandler(this.FormWebConfigSettings_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOk;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboLogLevel;
		private System.Windows.Forms.TextBox textPasswordLow;
		private System.Windows.Forms.TextBox textUserLow;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.TextBox textDatabase;
		private System.Windows.Forms.TextBox textServer;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboDatabaseType;
	}
}