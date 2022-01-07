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
			this.label8 = new System.Windows.Forms.Label();
			this.comboDatabaseType = new System.Windows.Forms.ComboBox();
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
			this.groupReplicationMaster = new System.Windows.Forms.GroupBox();
			this.labelReplicationMasterDbType = new System.Windows.Forms.Label();
			this.comboReplicationMasterDbType = new System.Windows.Forms.ComboBox();
			this.labelReplicationMasterPass = new System.Windows.Forms.Label();
			this.labelReplicationMasterUser = new System.Windows.Forms.Label();
			this.labelReplicationMasterDatabase = new System.Windows.Forms.Label();
			this.labelReplicationMasterServer = new System.Windows.Forms.Label();
			this.textReplicationMasterPass = new System.Windows.Forms.TextBox();
			this.textReplicationMasterUser = new System.Windows.Forms.TextBox();
			this.textReplicationMasterDatabase = new System.Windows.Forms.TextBox();
			this.textReplicationMasterServer = new System.Windows.Forms.TextBox();
			this.checkIsOneWayReplication = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupReplicationMaster.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(292, 450);
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
			this.butOk.Location = new System.Drawing.Point(211, 450);
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
			// groupReplicationMaster
			// 
			this.groupReplicationMaster.Controls.Add(this.labelReplicationMasterDbType);
			this.groupReplicationMaster.Controls.Add(this.comboReplicationMasterDbType);
			this.groupReplicationMaster.Controls.Add(this.labelReplicationMasterPass);
			this.groupReplicationMaster.Controls.Add(this.labelReplicationMasterUser);
			this.groupReplicationMaster.Controls.Add(this.labelReplicationMasterDatabase);
			this.groupReplicationMaster.Controls.Add(this.labelReplicationMasterServer);
			this.groupReplicationMaster.Controls.Add(this.textReplicationMasterPass);
			this.groupReplicationMaster.Controls.Add(this.textReplicationMasterUser);
			this.groupReplicationMaster.Controls.Add(this.textReplicationMasterDatabase);
			this.groupReplicationMaster.Controls.Add(this.textReplicationMasterServer);
			this.groupReplicationMaster.Location = new System.Drawing.Point(27, 282);
			this.groupReplicationMaster.Name = "groupReplicationMaster";
			this.groupReplicationMaster.Size = new System.Drawing.Size(324, 160);
			this.groupReplicationMaster.TabIndex = 3;
			this.groupReplicationMaster.TabStop = false;
			this.groupReplicationMaster.Text = "Master Connection Settings";
			// 
			// labelReplicationMasterDbType
			// 
			this.labelReplicationMasterDbType.Location = new System.Drawing.Point(9, 123);
			this.labelReplicationMasterDbType.Name = "labelReplicationMasterDbType";
			this.labelReplicationMasterDbType.Size = new System.Drawing.Size(119, 18);
			this.labelReplicationMasterDbType.TabIndex = 15;
			this.labelReplicationMasterDbType.Text = "Database Type";
			this.labelReplicationMasterDbType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboReplicationMasterDbType
			// 
			this.comboReplicationMasterDbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReplicationMasterDbType.FormattingEnabled = true;
			this.comboReplicationMasterDbType.Location = new System.Drawing.Point(129, 123);
			this.comboReplicationMasterDbType.Name = "comboReplicationMasterDbType";
			this.comboReplicationMasterDbType.Size = new System.Drawing.Size(189, 21);
			this.comboReplicationMasterDbType.TabIndex = 14;
			// 
			// labelReplicationMasterPass
			// 
			this.labelReplicationMasterPass.Location = new System.Drawing.Point(6, 97);
			this.labelReplicationMasterPass.Name = "labelReplicationMasterPass";
			this.labelReplicationMasterPass.Size = new System.Drawing.Size(122, 18);
			this.labelReplicationMasterPass.TabIndex = 10;
			this.labelReplicationMasterPass.Text = "Password";
			this.labelReplicationMasterPass.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelReplicationMasterUser
			// 
			this.labelReplicationMasterUser.Location = new System.Drawing.Point(6, 71);
			this.labelReplicationMasterUser.Name = "labelReplicationMasterUser";
			this.labelReplicationMasterUser.Size = new System.Drawing.Size(122, 18);
			this.labelReplicationMasterUser.TabIndex = 9;
			this.labelReplicationMasterUser.Text = "User";
			this.labelReplicationMasterUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelReplicationMasterDatabase
			// 
			this.labelReplicationMasterDatabase.Location = new System.Drawing.Point(12, 45);
			this.labelReplicationMasterDatabase.Name = "labelReplicationMasterDatabase";
			this.labelReplicationMasterDatabase.Size = new System.Drawing.Size(116, 18);
			this.labelReplicationMasterDatabase.TabIndex = 8;
			this.labelReplicationMasterDatabase.Text = "Database";
			this.labelReplicationMasterDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelReplicationMasterServer
			// 
			this.labelReplicationMasterServer.Location = new System.Drawing.Point(6, 19);
			this.labelReplicationMasterServer.Name = "labelReplicationMasterServer";
			this.labelReplicationMasterServer.Size = new System.Drawing.Size(122, 18);
			this.labelReplicationMasterServer.TabIndex = 7;
			this.labelReplicationMasterServer.Text = "Server";
			this.labelReplicationMasterServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReplicationMasterPass
			// 
			this.textReplicationMasterPass.Location = new System.Drawing.Point(129, 97);
			this.textReplicationMasterPass.Name = "textReplicationMasterPass";
			this.textReplicationMasterPass.Size = new System.Drawing.Size(189, 20);
			this.textReplicationMasterPass.TabIndex = 3;
			// 
			// textReplicationMasterUser
			// 
			this.textReplicationMasterUser.Location = new System.Drawing.Point(129, 71);
			this.textReplicationMasterUser.Name = "textReplicationMasterUser";
			this.textReplicationMasterUser.Size = new System.Drawing.Size(189, 20);
			this.textReplicationMasterUser.TabIndex = 2;
			// 
			// textReplicationMasterDatabase
			// 
			this.textReplicationMasterDatabase.Location = new System.Drawing.Point(129, 45);
			this.textReplicationMasterDatabase.Name = "textReplicationMasterDatabase";
			this.textReplicationMasterDatabase.Size = new System.Drawing.Size(189, 20);
			this.textReplicationMasterDatabase.TabIndex = 1;
			// 
			// textReplicationMasterServer
			// 
			this.textReplicationMasterServer.Location = new System.Drawing.Point(129, 19);
			this.textReplicationMasterServer.Name = "textReplicationMasterServer";
			this.textReplicationMasterServer.Size = new System.Drawing.Size(189, 20);
			this.textReplicationMasterServer.TabIndex = 0;
			// 
			// checkIsOneWayReplication
			// 
			this.checkIsOneWayReplication.Location = new System.Drawing.Point(27, 253);
			this.checkIsOneWayReplication.Name = "checkIsOneWayReplication";
			this.checkIsOneWayReplication.Size = new System.Drawing.Size(324, 24);
			this.checkIsOneWayReplication.TabIndex = 4;
			this.checkIsOneWayReplication.Text = "Is one way replication";
			this.checkIsOneWayReplication.UseVisualStyleBackColor = true;
			this.checkIsOneWayReplication.CheckedChanged += new System.EventHandler(this.checkIsOneWayReplication_CheckedChanged);
			// 
			// FormWebConfigSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(379, 485);
			this.Controls.Add(this.checkIsOneWayReplication);
			this.Controls.Add(this.groupReplicationMaster);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Name = "FormWebConfigSettings";
			this.Text = "OpenDentalWebConfig.xml Settings";
			this.Load += new System.EventHandler(this.FormWebConfigSettings_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupReplicationMaster.ResumeLayout(false);
			this.groupReplicationMaster.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupReplicationMaster;
		private System.Windows.Forms.Label labelReplicationMasterDbType;
		private System.Windows.Forms.ComboBox comboReplicationMasterDbType;
		private System.Windows.Forms.Label labelReplicationMasterPass;
		private System.Windows.Forms.Label labelReplicationMasterUser;
		private System.Windows.Forms.Label labelReplicationMasterDatabase;
		private System.Windows.Forms.Label labelReplicationMasterServer;
		private System.Windows.Forms.TextBox textReplicationMasterPass;
		private System.Windows.Forms.TextBox textReplicationMasterUser;
		private System.Windows.Forms.TextBox textReplicationMasterDatabase;
		private System.Windows.Forms.TextBox textReplicationMasterServer;
		private System.Windows.Forms.CheckBox checkIsOneWayReplication;
	}
}