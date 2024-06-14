
namespace OpenDental {
	partial class UserControlEnterpriseReports {
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
			this.checkReportingServerCompNameOrURI = new OpenDental.UI.CheckBox();
			this.radioReportServerMiddleTier = new System.Windows.Forms.RadioButton();
			this.radioReportServerDirect = new System.Windows.Forms.RadioButton();
			this.groupConnectionSettings = new OpenDental.UI.GroupBox();
			this.textReportingServerCompName = new System.Windows.Forms.TextBox();
			this.textMysqlPass = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textReportingServerMySqlUser = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboReportingServerDbName = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupMiddleTier = new OpenDental.UI.GroupBox();
			this.textReportingServerURI = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.groupConnectionSettings.SuspendLayout();
			this.groupMiddleTier.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkReportingServerCompNameOrURI
			// 
			this.checkReportingServerCompNameOrURI.Location = new System.Drawing.Point(11, 24);
			this.checkReportingServerCompNameOrURI.Name = "checkReportingServerCompNameOrURI";
			this.checkReportingServerCompNameOrURI.Size = new System.Drawing.Size(356, 18);
			this.checkReportingServerCompNameOrURI.TabIndex = 238;
			this.checkReportingServerCompNameOrURI.Text = "Use separate reporting server";
			this.checkReportingServerCompNameOrURI.CheckStateChanged += new System.EventHandler(this.checkUseReportServer_CheckedChanged);
			// 
			// radioReportServerMiddleTier
			// 
			this.radioReportServerMiddleTier.Location = new System.Drawing.Point(11, 274);
			this.radioReportServerMiddleTier.Name = "radioReportServerMiddleTier";
			this.radioReportServerMiddleTier.Size = new System.Drawing.Size(356, 18);
			this.radioReportServerMiddleTier.TabIndex = 237;
			this.radioReportServerMiddleTier.Text = "Middle Tier";
			this.radioReportServerMiddleTier.UseVisualStyleBackColor = true;
			// 
			// radioReportServerDirect
			// 
			this.radioReportServerDirect.Checked = true;
			this.radioReportServerDirect.Location = new System.Drawing.Point(11, 48);
			this.radioReportServerDirect.Name = "radioReportServerDirect";
			this.radioReportServerDirect.Size = new System.Drawing.Size(356, 18);
			this.radioReportServerDirect.TabIndex = 236;
			this.radioReportServerDirect.TabStop = true;
			this.radioReportServerDirect.Text = "Direct Connection";
			this.radioReportServerDirect.UseVisualStyleBackColor = true;
			this.radioReportServerDirect.CheckedChanged += new System.EventHandler(this.RadioReportServerDirect_CheckedChanged);
			// 
			// groupConnectionSettings
			// 
			this.groupConnectionSettings.Controls.Add(this.textReportingServerCompName);
			this.groupConnectionSettings.Controls.Add(this.textMysqlPass);
			this.groupConnectionSettings.Controls.Add(this.label6);
			this.groupConnectionSettings.Controls.Add(this.textReportingServerMySqlUser);
			this.groupConnectionSettings.Controls.Add(this.label1);
			this.groupConnectionSettings.Controls.Add(this.comboReportingServerDbName);
			this.groupConnectionSettings.Controls.Add(this.label5);
			this.groupConnectionSettings.Controls.Add(this.label4);
			this.groupConnectionSettings.Location = new System.Drawing.Point(29, 68);
			this.groupConnectionSettings.Name = "groupConnectionSettings";
			this.groupConnectionSettings.Size = new System.Drawing.Size(455, 200);
			this.groupConnectionSettings.TabIndex = 235;
			this.groupConnectionSettings.Text = "Connection Settings";
			// 
			// textReportingServerCompName
			// 
			this.textReportingServerCompName.Location = new System.Drawing.Point(6, 38);
			this.textReportingServerCompName.Name = "textReportingServerCompName";
			this.textReportingServerCompName.Size = new System.Drawing.Size(443, 20);
			this.textReportingServerCompName.TabIndex = 15;
			this.textReportingServerCompName.Validating += new System.ComponentModel.CancelEventHandler(this.textReportingServerCompName_Validating);
			// 
			// textMysqlPass
			// 
			this.textMysqlPass.Location = new System.Drawing.Point(6, 174);
			this.textMysqlPass.Name = "textMysqlPass";
			this.textMysqlPass.PasswordChar = '*';
			this.textMysqlPass.Size = new System.Drawing.Size(443, 20);
			this.textMysqlPass.TabIndex = 223;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 155);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(443, 18);
			this.label6.TabIndex = 221;
			this.label6.Text = "MySQL Password:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textReportingServerMySqlUser
			// 
			this.textReportingServerMySqlUser.Location = new System.Drawing.Point(6, 129);
			this.textReportingServerMySqlUser.Name = "textReportingServerMySqlUser";
			this.textReportingServerMySqlUser.Size = new System.Drawing.Size(443, 20);
			this.textReportingServerMySqlUser.TabIndex = 222;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(443, 18);
			this.label1.TabIndex = 215;
			this.label1.Text = "Server Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboReportingServerDbName
			// 
			this.comboReportingServerDbName.FormattingEnabled = true;
			this.comboReportingServerDbName.Location = new System.Drawing.Point(6, 83);
			this.comboReportingServerDbName.Name = "comboReportingServerDbName";
			this.comboReportingServerDbName.Size = new System.Drawing.Size(443, 21);
			this.comboReportingServerDbName.TabIndex = 216;
			this.comboReportingServerDbName.Click += new System.EventHandler(this.ComboDatabase_DropDown);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 110);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(443, 18);
			this.label5.TabIndex = 219;
			this.label5.Text = "MySQL User:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(443, 18);
			this.label4.TabIndex = 217;
			this.label4.Text = "Database:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupMiddleTier
			// 
			this.groupMiddleTier.Controls.Add(this.textReportingServerURI);
			this.groupMiddleTier.Controls.Add(this.label11);
			this.groupMiddleTier.Controls.Add(this.label9);
			this.groupMiddleTier.Location = new System.Drawing.Point(29, 294);
			this.groupMiddleTier.Name = "groupMiddleTier";
			this.groupMiddleTier.Size = new System.Drawing.Size(455, 92);
			this.groupMiddleTier.TabIndex = 234;
			this.groupMiddleTier.Text = "Middle Tier";
			// 
			// textReportingServerURI
			// 
			this.textReportingServerURI.Location = new System.Drawing.Point(6, 38);
			this.textReportingServerURI.Name = "textReportingServerURI";
			this.textReportingServerURI.Size = new System.Drawing.Size(443, 20);
			this.textReportingServerURI.TabIndex = 7;
			this.textReportingServerURI.Validating += new System.ComponentModel.CancelEventHandler(this.textReportingServerURI_Validating);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(6, 60);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(443, 26);
			this.label11.TabIndex = 14;
			this.label11.Text = "The currently logged in user\'s credentials will be used to when accessing the Mid" +
    "dle Tier database.";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 19);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(443, 18);
			this.label9.TabIndex = 9;
			this.label9.Text = "URI";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UserControlEnterpriseReports
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkReportingServerCompNameOrURI);
			this.Controls.Add(this.radioReportServerMiddleTier);
			this.Controls.Add(this.radioReportServerDirect);
			this.Controls.Add(this.groupConnectionSettings);
			this.Controls.Add(this.groupMiddleTier);
			this.Name = "UserControlEnterpriseReports";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupConnectionSettings.ResumeLayout(false);
			this.groupConnectionSettings.PerformLayout();
			this.groupMiddleTier.ResumeLayout(false);
			this.groupMiddleTier.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.CheckBox checkReportingServerCompNameOrURI;
		private System.Windows.Forms.RadioButton radioReportServerMiddleTier;
		private System.Windows.Forms.RadioButton radioReportServerDirect;
		private UI.GroupBox groupConnectionSettings;
		private System.Windows.Forms.TextBox textReportingServerCompName;
		private System.Windows.Forms.TextBox textMysqlPass;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textReportingServerMySqlUser;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboReportingServerDbName;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private UI.GroupBox groupMiddleTier;
		private System.Windows.Forms.TextBox textReportingServerURI;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label9;
	}
}
