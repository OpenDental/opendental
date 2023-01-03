
namespace OpenDental {
	partial class UserControlServerConnections {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.checkUseReadOnlyServer = new OpenDental.UI.CheckBox();
			this.groupBoxReadOnlyServerSetup = new OpenDental.UI.GroupBox();
			this.radioReadOnlyServerDirect = new System.Windows.Forms.RadioButton();
			this.groupConnectionSettings = new OpenDental.UI.GroupBox();
			this.comboServerName = new System.Windows.Forms.ComboBox();
			this.textMysqlPass = new System.Windows.Forms.TextBox();
			this.comboDatabase = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textMysqlUser = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.radioReadOnlyServerMiddleTier = new System.Windows.Forms.RadioButton();
			this.groupMiddleTier = new OpenDental.UI.GroupBox();
			this.textMiddleTierURI = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butReadOnlyServerSetupDetails = new OpenDental.UI.Button();
			this.butMiddleTierURIDetails = new OpenDental.UI.Button();
			this.groupBoxReadOnlyServerSetup.SuspendLayout();
			this.groupConnectionSettings.SuspendLayout();
			this.groupMiddleTier.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkUseReadOnlyServer
			// 
			this.checkUseReadOnlyServer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseReadOnlyServer.Location = new System.Drawing.Point(60, 10);
			this.checkUseReadOnlyServer.Name = "checkUseReadOnlyServer";
			this.checkUseReadOnlyServer.Size = new System.Drawing.Size(400, 17);
			this.checkUseReadOnlyServer.TabIndex = 0;
			this.checkUseReadOnlyServer.Text = "Use Separate Read-Only Server\r\n";
			this.checkUseReadOnlyServer.CheckedChanged += new System.EventHandler(this.checkReadOnlyServer_CheckChanged);
			// 
			// groupBoxReadOnlyServerSetup
			// 
			this.groupBoxReadOnlyServerSetup.Controls.Add(this.radioReadOnlyServerDirect);
			this.groupBoxReadOnlyServerSetup.Controls.Add(this.groupConnectionSettings);
			this.groupBoxReadOnlyServerSetup.Controls.Add(this.radioReadOnlyServerMiddleTier);
			this.groupBoxReadOnlyServerSetup.Controls.Add(this.groupMiddleTier);
			this.groupBoxReadOnlyServerSetup.Location = new System.Drawing.Point(20, 30);
			this.groupBoxReadOnlyServerSetup.Name = "groupBoxReadOnlyServerSetup";
			this.groupBoxReadOnlyServerSetup.Size = new System.Drawing.Size(450, 263);
			this.groupBoxReadOnlyServerSetup.TabIndex = 99;
			this.groupBoxReadOnlyServerSetup.Text = "Read-Only Server Setup";
			// 
			// radioReadOnlyServerDirect
			// 
			this.radioReadOnlyServerDirect.Location = new System.Drawing.Point(9, 24);
			this.radioReadOnlyServerDirect.Name = "radioReadOnlyServerDirect";
			this.radioReadOnlyServerDirect.Size = new System.Drawing.Size(157, 15);
			this.radioReadOnlyServerDirect.TabIndex = 3;
			this.radioReadOnlyServerDirect.TabStop = true;
			this.radioReadOnlyServerDirect.Text = "Direct Connection";
			this.radioReadOnlyServerDirect.UseVisualStyleBackColor = true;
			this.radioReadOnlyServerDirect.CheckedChanged += new System.EventHandler(this.checkReadOnlyServer_CheckChanged);
			// 
			// groupConnectionSettings
			// 
			this.groupConnectionSettings.Controls.Add(this.comboServerName);
			this.groupConnectionSettings.Controls.Add(this.textMysqlPass);
			this.groupConnectionSettings.Controls.Add(this.comboDatabase);
			this.groupConnectionSettings.Controls.Add(this.label1);
			this.groupConnectionSettings.Controls.Add(this.textMysqlUser);
			this.groupConnectionSettings.Controls.Add(this.label2);
			this.groupConnectionSettings.Controls.Add(this.label3);
			this.groupConnectionSettings.Controls.Add(this.label4);
			this.groupConnectionSettings.Location = new System.Drawing.Point(9, 45);
			this.groupConnectionSettings.Name = "groupConnectionSettings";
			this.groupConnectionSettings.Size = new System.Drawing.Size(438, 138);
			this.groupConnectionSettings.TabIndex = 100;
			this.groupConnectionSettings.Text = "Connection Settings";
			// 
			// comboServerName
			// 
			this.comboServerName.FormattingEnabled = true;
			this.comboServerName.Location = new System.Drawing.Point(139, 19);
			this.comboServerName.Name = "comboServerName";
			this.comboServerName.Size = new System.Drawing.Size(292, 21);
			this.comboServerName.TabIndex = 4;
			this.comboServerName.DropDown += new System.EventHandler(this.comboComputers_DropDown);
			// 
			// textMysqlPass
			// 
			this.textMysqlPass.Location = new System.Drawing.Point(139, 112);
			this.textMysqlPass.Name = "textMysqlPass";
			this.textMysqlPass.Size = new System.Drawing.Size(292, 20);
			this.textMysqlPass.TabIndex = 7;
			// 
			// comboDatabase
			// 
			this.comboDatabase.FormattingEnabled = true;
			this.comboDatabase.Location = new System.Drawing.Point(139, 50);
			this.comboDatabase.Name = "comboDatabase";
			this.comboDatabase.Size = new System.Drawing.Size(292, 21);
			this.comboDatabase.TabIndex = 5;
			this.comboDatabase.DropDown += new System.EventHandler(this.comboDatabase_DropDown);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 115);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 17);
			this.label1.TabIndex = 104;
			this.label1.Text = "MySQL Password";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMysqlUser
			// 
			this.textMysqlUser.Location = new System.Drawing.Point(139, 81);
			this.textMysqlUser.Name = "textMysqlUser";
			this.textMysqlUser.Size = new System.Drawing.Size(292, 20);
			this.textMysqlUser.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(15, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(122, 17);
			this.label2.TabIndex = 101;
			this.label2.Text = "Server Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(15, 84);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(122, 17);
			this.label3.TabIndex = 103;
			this.label3.Text = "MySQL User";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(15, 53);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(122, 17);
			this.label4.TabIndex = 102;
			this.label4.Text = "Database";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioReadOnlyServerMiddleTier
			// 
			this.radioReadOnlyServerMiddleTier.Location = new System.Drawing.Point(9, 196);
			this.radioReadOnlyServerMiddleTier.Name = "radioReadOnlyServerMiddleTier";
			this.radioReadOnlyServerMiddleTier.Size = new System.Drawing.Size(157, 15);
			this.radioReadOnlyServerMiddleTier.TabIndex = 8;
			this.radioReadOnlyServerMiddleTier.TabStop = true;
			this.radioReadOnlyServerMiddleTier.Text = "Middle Tier";
			this.radioReadOnlyServerMiddleTier.UseVisualStyleBackColor = true;
			this.radioReadOnlyServerMiddleTier.CheckedChanged += new System.EventHandler(this.checkReadOnlyServer_CheckChanged);
			// 
			// groupMiddleTier
			// 
			this.groupMiddleTier.Controls.Add(this.textMiddleTierURI);
			this.groupMiddleTier.Controls.Add(this.label6);
			this.groupMiddleTier.Location = new System.Drawing.Point(9, 215);
			this.groupMiddleTier.Name = "groupMiddleTier";
			this.groupMiddleTier.Size = new System.Drawing.Size(438, 36);
			this.groupMiddleTier.TabIndex = 105;
			this.groupMiddleTier.Text = "";
			// 
			// textMiddleTierURI
			// 
			this.textMiddleTierURI.Location = new System.Drawing.Point(139, 8);
			this.textMiddleTierURI.Name = "textMiddleTierURI";
			this.textMiddleTierURI.Size = new System.Drawing.Size(292, 20);
			this.textMiddleTierURI.TabIndex = 9;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(15, 11);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(122, 17);
			this.label6.TabIndex = 106;
			this.label6.Text = "URI";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butReadOnlyServerSetupDetails
			// 
			this.butReadOnlyServerSetupDetails.ForeColor = System.Drawing.Color.Black;
			this.butReadOnlyServerSetupDetails.Location = new System.Drawing.Point(479, 8);
			this.butReadOnlyServerSetupDetails.Name = "butReadOnlyServerSetupDetails";
			this.butReadOnlyServerSetupDetails.Size = new System.Drawing.Size(64, 21);
			this.butReadOnlyServerSetupDetails.TabIndex = 1;
			this.butReadOnlyServerSetupDetails.Text = "Details";
			this.butReadOnlyServerSetupDetails.Click += new System.EventHandler(this.butReadOnlyServerSetupDetails_Click);
			// 
			// butMiddleTierURIDetails
			// 
			this.butMiddleTierURIDetails.ForeColor = System.Drawing.Color.Black;
			this.butMiddleTierURIDetails.Location = new System.Drawing.Point(479, 253);
			this.butMiddleTierURIDetails.Name = "butMiddleTierURIDetails";
			this.butMiddleTierURIDetails.Size = new System.Drawing.Size(64, 21);
			this.butMiddleTierURIDetails.TabIndex = 10;
			this.butMiddleTierURIDetails.Text = "Details";
			this.butMiddleTierURIDetails.Click += new System.EventHandler(this.butMiddleTierURIDetails_Click);
			// 
			// UserControlServerConnections
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkUseReadOnlyServer);
			this.Controls.Add(this.butReadOnlyServerSetupDetails);
			this.Controls.Add(this.groupBoxReadOnlyServerSetup);
			this.Controls.Add(this.butMiddleTierURIDetails);
			this.Name = "UserControlServerConnections";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupBoxReadOnlyServerSetup.ResumeLayout(false);
			this.groupConnectionSettings.ResumeLayout(false);
			this.groupConnectionSettings.PerformLayout();
			this.groupMiddleTier.ResumeLayout(false);
			this.groupMiddleTier.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TextBox textMysqlPass;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textMysqlUser;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private UI.GroupBox groupConnectionSettings;
		private OpenDental.UI.CheckBox checkUseReadOnlyServer;
		private System.Windows.Forms.RadioButton radioReadOnlyServerMiddleTier;
		private System.Windows.Forms.RadioButton radioReadOnlyServerDirect;
		private System.Windows.Forms.TextBox textMiddleTierURI;
		private System.Windows.Forms.Label label6;
		private UI.GroupBox groupMiddleTier;
		private UI.GroupBox groupBoxReadOnlyServerSetup;
		private System.Windows.Forms.ComboBox comboServerName;
		private System.Windows.Forms.ComboBox comboDatabase;
		private UI.Button butReadOnlyServerSetupDetails;
		private UI.Button butMiddleTierURIDetails;
	}
}
