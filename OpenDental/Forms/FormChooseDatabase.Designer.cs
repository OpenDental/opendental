namespace OpenDental{
	partial class FormChooseDatabase {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChooseDatabase));
			this.textConnectionString = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.listType = new OpenDental.UI.ListBoxOD();
			this.label7 = new System.Windows.Forms.Label();
			this.checkConnectServer = new System.Windows.Forms.CheckBox();
			this.groupServer = new System.Windows.Forms.GroupBox();
			this.checkBoxAutomaticLogin = new System.Windows.Forms.CheckBox();
			this.checkUsingEcw = new System.Windows.Forms.CheckBox();
			this.textURI = new System.Windows.Forms.TextBox();
			this.textUser2 = new System.Windows.Forms.TextBox();
			this.textPassword2 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupDirect = new System.Windows.Forms.GroupBox();
			this.textUser = new System.Windows.Forms.TextBox();
			this.comboDatabase = new System.Windows.Forms.ComboBox();
			this.checkNoShow = new System.Windows.Forms.CheckBox();
			this.comboComputerName = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkDynamicMode = new System.Windows.Forms.CheckBox();
			this.groupServer.SuspendLayout();
			this.groupDirect.SuspendLayout();
			this.SuspendLayout();
			// 
			// textConnectionString
			// 
			this.textConnectionString.Location = new System.Drawing.Point(376, 366);
			this.textConnectionString.Multiline = true;
			this.textConnectionString.Name = "textConnectionString";
			this.textConnectionString.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textConnectionString.Size = new System.Drawing.Size(312, 130);
			this.textConnectionString.TabIndex = 34;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(374, 348);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(165, 13);
			this.label8.TabIndex = 39;
			this.label8.Text = "Advanced: Use connection string";
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(376, 310);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(99, 30);
			this.listType.TabIndex = 38;
			this.listType.Visible = false;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(373, 288);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(223, 18);
			this.label7.TabIndex = 37;
			this.label7.Text = "Database Type";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.label7.Visible = false;
			// 
			// checkConnectServer
			// 
			this.checkConnectServer.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkConnectServer.Location = new System.Drawing.Point(28, 276);
			this.checkConnectServer.Name = "checkConnectServer";
			this.checkConnectServer.Size = new System.Drawing.Size(328, 18);
			this.checkConnectServer.TabIndex = 33;
			this.checkConnectServer.Text = "Connect to Middle Tier instead";
			this.checkConnectServer.UseVisualStyleBackColor = true;
			this.checkConnectServer.Click += new System.EventHandler(this.checkConnectServer_Click);
			// 
			// groupServer
			// 
			this.groupServer.Controls.Add(this.checkBoxAutomaticLogin);
			this.groupServer.Controls.Add(this.checkUsingEcw);
			this.groupServer.Controls.Add(this.textURI);
			this.groupServer.Controls.Add(this.textUser2);
			this.groupServer.Controls.Add(this.textPassword2);
			this.groupServer.Controls.Add(this.label10);
			this.groupServer.Controls.Add(this.label11);
			this.groupServer.Controls.Add(this.label9);
			this.groupServer.Controls.Add(this.label6);
			this.groupServer.Location = new System.Drawing.Point(28, 304);
			this.groupServer.Name = "groupServer";
			this.groupServer.Size = new System.Drawing.Size(336, 218);
			this.groupServer.TabIndex = 32;
			this.groupServer.TabStop = false;
			this.groupServer.Text = "Connect to Middle Tier - Only for advanced users";
			// 
			// checkBoxAutomaticLogin
			// 
			this.checkBoxAutomaticLogin.AutoSize = true;
			this.checkBoxAutomaticLogin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBoxAutomaticLogin.Location = new System.Drawing.Point(13, 195);
			this.checkBoxAutomaticLogin.Name = "checkBoxAutomaticLogin";
			this.checkBoxAutomaticLogin.Size = new System.Drawing.Size(145, 18);
			this.checkBoxAutomaticLogin.TabIndex = 40;
			this.checkBoxAutomaticLogin.Text = "Log me in automatically.";
			this.checkBoxAutomaticLogin.UseVisualStyleBackColor = true;
			// 
			// checkUsingEcw
			// 
			this.checkUsingEcw.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUsingEcw.Location = new System.Drawing.Point(13, 176);
			this.checkUsingEcw.Name = "checkUsingEcw";
			this.checkUsingEcw.Size = new System.Drawing.Size(317, 18);
			this.checkUsingEcw.TabIndex = 10;
			this.checkUsingEcw.Text = "Using eClinicalWorks";
			this.checkUsingEcw.UseVisualStyleBackColor = true;
			// 
			// textURI
			// 
			this.textURI.Location = new System.Drawing.Point(13, 63);
			this.textURI.Name = "textURI";
			this.textURI.Size = new System.Drawing.Size(309, 20);
			this.textURI.TabIndex = 7;
			// 
			// textUser2
			// 
			this.textUser2.Location = new System.Drawing.Point(13, 106);
			this.textUser2.Name = "textUser2";
			this.textUser2.Size = new System.Drawing.Size(309, 20);
			this.textUser2.TabIndex = 8;
			// 
			// textPassword2
			// 
			this.textPassword2.Location = new System.Drawing.Point(13, 147);
			this.textPassword2.Name = "textPassword2";
			this.textPassword2.PasswordChar = '*';
			this.textPassword2.Size = new System.Drawing.Size(309, 20);
			this.textPassword2.TabIndex = 9;
			this.textPassword2.UseSystemPasswordChar = true;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(11, 128);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(281, 18);
			this.label10.TabIndex = 11;
			this.label10.Text = "Password";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(11, 87);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(295, 18);
			this.label11.TabIndex = 14;
			this.label11.Text = "Open Dental User (not MySQL user)";
			this.label11.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(10, 42);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(283, 18);
			this.label9.TabIndex = 9;
			this.label9.Text = "URI";
			this.label9.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(9, 25);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(297, 18);
			this.label6.TabIndex = 0;
			this.label6.Text = "Read the manual to learn how to install the middle tier.";
			// 
			// groupDirect
			// 
			this.groupDirect.Controls.Add(this.textUser);
			this.groupDirect.Controls.Add(this.comboDatabase);
			this.groupDirect.Controls.Add(this.checkNoShow);
			this.groupDirect.Controls.Add(this.comboComputerName);
			this.groupDirect.Controls.Add(this.label1);
			this.groupDirect.Controls.Add(this.textPassword);
			this.groupDirect.Controls.Add(this.label2);
			this.groupDirect.Controls.Add(this.label3);
			this.groupDirect.Controls.Add(this.label4);
			this.groupDirect.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupDirect.Location = new System.Drawing.Point(28, 15);
			this.groupDirect.Name = "groupDirect";
			this.groupDirect.Size = new System.Drawing.Size(660, 244);
			this.groupDirect.TabIndex = 31;
			this.groupDirect.TabStop = false;
			this.groupDirect.Text = "Connection Settings - These values will only be used on this computer.  They have" +
    " to be set on each computer";
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(13, 140);
			this.textUser.Name = "textUser";
			this.textUser.Size = new System.Drawing.Size(280, 20);
			this.textUser.TabIndex = 3;
			// 
			// comboDatabase
			// 
			this.comboDatabase.DropDownHeight = 390;
			this.comboDatabase.IntegralHeight = false;
			this.comboDatabase.Location = new System.Drawing.Point(13, 98);
			this.comboDatabase.MaxDropDownItems = 100;
			this.comboDatabase.Name = "comboDatabase";
			this.comboDatabase.Size = new System.Drawing.Size(280, 21);
			this.comboDatabase.TabIndex = 2;
			this.comboDatabase.DropDown += new System.EventHandler(this.comboDatabase_DropDown);
			// 
			// checkNoShow
			// 
			this.checkNoShow.AutoSize = true;
			this.checkNoShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNoShow.Location = new System.Drawing.Point(13, 211);
			this.checkNoShow.Name = "checkNoShow";
			this.checkNoShow.Size = new System.Drawing.Size(294, 18);
			this.checkNoShow.TabIndex = 5;
			this.checkNoShow.Text = "Do not show this window on startup (this computer only)";
			this.checkNoShow.UseVisualStyleBackColor = true;
			// 
			// comboComputerName
			// 
			this.comboComputerName.DropDownHeight = 390;
			this.comboComputerName.IntegralHeight = false;
			this.comboComputerName.Location = new System.Drawing.Point(13, 56);
			this.comboComputerName.MaxDropDownItems = 100;
			this.comboComputerName.Name = "comboComputerName";
			this.comboComputerName.Size = new System.Drawing.Size(280, 21);
			this.comboComputerName.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(582, 38);
			this.label1.TabIndex = 0;
			this.label1.Text = "Server Name: The name of the computer where the MySQL server and database are loc" +
    "ated.  If you are running this program on a single computer only, then the compu" +
    "ter name may be localhost.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(13, 181);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(280, 20);
			this.textPassword.TabIndex = 4;
			this.textPassword.UseSystemPasswordChar = true;
			this.textPassword.TextChanged += new System.EventHandler(this.textPassword_TextChanged);
			this.textPassword.Leave += new System.EventHandler(this.textPassword_Leave);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 162);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(509, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "MySQL Password:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(11, 121);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(480, 18);
			this.label3.TabIndex = 4;
			this.label3.Text = "MySQL User:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(11, 79);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(582, 18);
			this.label4.TabIndex = 6;
			this.label4.Text = "Database: usually opendental unless you changed the name.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(613, 534);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 36;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(520, 534);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 35;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkDynamicMode
			// 
			this.checkDynamicMode.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDynamicMode.Location = new System.Drawing.Point(376, 499);
			this.checkDynamicMode.Name = "checkDynamicMode";
			this.checkDynamicMode.Size = new System.Drawing.Size(312, 29);
			this.checkDynamicMode.TabIndex = 41;
			this.checkDynamicMode.Text = "Dynamic Mode: Automatically downgrades or upgrades to server version.";
			this.checkDynamicMode.UseVisualStyleBackColor = true;
			this.checkDynamicMode.CheckedChanged += new System.EventHandler(this.checkDynamicMode_CheckedChanged);
			// 
			// FormChooseDatabase
			// 
			this.AcceptButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(716, 574);
			this.Controls.Add(this.checkDynamicMode);
			this.Controls.Add(this.textConnectionString);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.listType);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.checkConnectServer);
			this.Controls.Add(this.groupServer);
			this.Controls.Add(this.groupDirect);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormChooseDatabase";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Choose Database";
			this.Load += new System.EventHandler(this.FormChooseDatabase_Load);
			this.groupServer.ResumeLayout(false);
			this.groupServer.PerformLayout();
			this.groupDirect.ResumeLayout(false);
			this.groupDirect.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textConnectionString;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.ListBoxOD listType;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkConnectServer;
		private System.Windows.Forms.GroupBox groupServer;
		private System.Windows.Forms.CheckBox checkUsingEcw;
		private System.Windows.Forms.TextBox textURI;
		private System.Windows.Forms.TextBox textUser2;
		private System.Windows.Forms.TextBox textPassword2;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupDirect;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.ComboBox comboDatabase;
		private System.Windows.Forms.CheckBox checkNoShow;
		private System.Windows.Forms.ComboBox comboComputerName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.CheckBox checkBoxAutomaticLogin;
		private System.Windows.Forms.CheckBox checkDynamicMode;
	}
}