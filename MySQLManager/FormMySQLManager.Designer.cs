namespace MySQLManager {
	partial class FormMySQLUserManager {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMySQLUserManager));
			this.groupConnection = new System.Windows.Forms.GroupBox();
			this.butConnect = new System.Windows.Forms.Button();
			this.textPort = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textServer = new System.Windows.Forms.TextBox();
			this.textUser = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butClose = new System.Windows.Forms.Button();
			this.butAdd = new System.Windows.Forms.Button();
			this.butEdit = new System.Windows.Forms.Button();
			this.butDrop = new System.Windows.Forms.Button();
			this.listBoxUsers = new System.Windows.Forms.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupConnection.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupConnection
			// 
			this.groupConnection.Controls.Add(this.butConnect);
			this.groupConnection.Controls.Add(this.textPort);
			this.groupConnection.Controls.Add(this.label5);
			this.groupConnection.Controls.Add(this.textServer);
			this.groupConnection.Controls.Add(this.textUser);
			this.groupConnection.Controls.Add(this.label1);
			this.groupConnection.Controls.Add(this.textPassword);
			this.groupConnection.Controls.Add(this.label2);
			this.groupConnection.Controls.Add(this.label3);
			this.groupConnection.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupConnection.Location = new System.Drawing.Point(12, 12);
			this.groupConnection.Name = "groupConnection";
			this.groupConnection.Size = new System.Drawing.Size(604, 133);
			this.groupConnection.TabIndex = 0;
			this.groupConnection.TabStop = false;
			this.groupConnection.Text = "Connection Settings";
			// 
			// butConnect
			// 
			this.butConnect.Location = new System.Drawing.Point(512, 101);
			this.butConnect.Name = "butConnect";
			this.butConnect.Size = new System.Drawing.Size(75, 23);
			this.butConnect.TabIndex = 3;
			this.butConnect.Text = "C&onnect";
			this.butConnect.UseVisualStyleBackColor = true;
			this.butConnect.Click += new System.EventHandler(this.butConnect_Click);
			// 
			// textPort
			// 
			this.textPort.Location = new System.Drawing.Point(14, 78);
			this.textPort.Name = "textPort";
			this.textPort.Size = new System.Drawing.Size(70, 20);
			this.textPort.TabIndex = 2;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(11, 59);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(282, 16);
			this.label5.TabIndex = 9;
			this.label5.Text = "Port - If different than the default 3306";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textServer
			// 
			this.textServer.Location = new System.Drawing.Point(14, 34);
			this.textServer.Name = "textServer";
			this.textServer.ReadOnly = true;
			this.textServer.Size = new System.Drawing.Size(279, 20);
			this.textServer.TabIndex = 7;
			this.textServer.TabStop = false;
			this.textServer.Text = "localhost";
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(307, 34);
			this.textUser.Name = "textUser";
			this.textUser.Size = new System.Drawing.Size(280, 20);
			this.textUser.TabIndex = 0;
			this.textUser.Text = "root";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(282, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Server Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(307, 75);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(280, 20);
			this.textPassword.TabIndex = 1;
			this.textPassword.UseSystemPasswordChar = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(305, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(282, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "MySQL Password";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(305, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(282, 18);
			this.label3.TabIndex = 4;
			this.label3.Text = "MySQL User";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(524, 466);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "&Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(267, 167);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 23);
			this.butAdd.TabIndex = 1;
			this.butAdd.Text = "&Add";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butEdit
			// 
			this.butEdit.Location = new System.Drawing.Point(267, 202);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(75, 23);
			this.butEdit.TabIndex = 2;
			this.butEdit.Text = "&Edit";
			this.butEdit.UseVisualStyleBackColor = true;
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// butDrop
			// 
			this.butDrop.Location = new System.Drawing.Point(267, 238);
			this.butDrop.Name = "butDrop";
			this.butDrop.Size = new System.Drawing.Size(75, 23);
			this.butDrop.TabIndex = 3;
			this.butDrop.Text = "&Drop";
			this.butDrop.UseVisualStyleBackColor = true;
			this.butDrop.Click += new System.EventHandler(this.butDrop_Click);
			// 
			// listBoxUsers
			// 
			this.listBoxUsers.FormattingEnabled = true;
			this.listBoxUsers.Location = new System.Drawing.Point(26, 167);
			this.listBoxUsers.Name = "listBoxUsers";
			this.listBoxUsers.Size = new System.Drawing.Size(235, 316);
			this.listBoxUsers.TabIndex = 39;
			this.listBoxUsers.DoubleClick += new System.EventHandler(this.listBoxUsers_DoubleClick);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(23, 147);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(102, 18);
			this.label4.TabIndex = 40;
			this.label4.Text = "Users";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormMySQLUserManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 498);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listBoxUsers);
			this.Controls.Add(this.butDrop);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupConnection);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMySQLUserManager";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MySQL User Manager";
			this.Load += new System.EventHandler(this.FormMySQLUserManager_Load);
			this.groupConnection.ResumeLayout(false);
			this.groupConnection.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupConnection;
		private System.Windows.Forms.TextBox textServer;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textPort;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button butConnect;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butAdd;
		private System.Windows.Forms.Button butEdit;
		private System.Windows.Forms.Button butDrop;
		private System.Windows.Forms.ListBox listBoxUsers;
		private System.Windows.Forms.Label label4;
	}
}

