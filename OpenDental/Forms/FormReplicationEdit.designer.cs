namespace OpenDental{
	partial class FormReplicationEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReplicationEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textRangeStart = new System.Windows.Forms.TextBox();
			this.textRangeEnd = new System.Windows.Forms.TextBox();
			this.textAtoZpath = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.checkUpdateBlocked = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.textServerId = new OpenDental.ValidNum();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.checkReportServer = new System.Windows.Forms.CheckBox();
			this.butThisComputerDesc = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(340, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(327, 48);
			this.label1.TabIndex = 60;
			this.label1.Text = "this also needs to be set in the my.ini file on each server.  If the my.ini file " +
    "gets changed, be sure to restart the server and each workstation client.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(238, 22);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(318, 20);
			this.textDescript.TabIndex = 0;
			this.textDescript.WordWrap = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(226, 18);
			this.label2.TabIndex = 62;
			this.label2.Text = "Server Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(226, 18);
			this.label3.TabIndex = 63;
			this.label3.Text = "server_id";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 107);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(226, 18);
			this.label4.TabIndex = 65;
			this.label4.Text = "Range Start";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(10, 137);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(226, 18);
			this.label5.TabIndex = 67;
			this.label5.Text = "Range End";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRangeStart
			// 
			this.textRangeStart.Location = new System.Drawing.Point(238, 107);
			this.textRangeStart.Name = "textRangeStart";
			this.textRangeStart.Size = new System.Drawing.Size(175, 20);
			this.textRangeStart.TabIndex = 3;
			this.textRangeStart.WordWrap = false;
			// 
			// textRangeEnd
			// 
			this.textRangeEnd.Location = new System.Drawing.Point(238, 137);
			this.textRangeEnd.Name = "textRangeEnd";
			this.textRangeEnd.Size = new System.Drawing.Size(175, 20);
			this.textRangeEnd.TabIndex = 4;
			this.textRangeEnd.WordWrap = false;
			// 
			// textAtoZpath
			// 
			this.textAtoZpath.Location = new System.Drawing.Point(238, 168);
			this.textAtoZpath.Name = "textAtoZpath";
			this.textAtoZpath.Size = new System.Drawing.Size(388, 20);
			this.textAtoZpath.TabIndex = 5;
			this.textAtoZpath.WordWrap = false;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 168);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(226, 18);
			this.label6.TabIndex = 71;
			this.label6.Text = "A to Z images folder";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUpdateBlocked
			// 
			this.checkUpdateBlocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUpdateBlocked.Location = new System.Drawing.Point(16, 200);
			this.checkUpdateBlocked.Name = "checkUpdateBlocked";
			this.checkUpdateBlocked.Size = new System.Drawing.Size(236, 18);
			this.checkUpdateBlocked.TabIndex = 6;
			this.checkUpdateBlocked.Text = "Update Blocked";
			this.checkUpdateBlocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUpdateBlocked.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(258, 200);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(368, 48);
			this.label7.TabIndex = 100;
			this.label7.Text = "Use this option carefully. It really will block the ability of the server to upda" +
    "te database versions, and it\'s possible that this could prevent startup of the p" +
    "rogram in certain situations.";
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(418, 141);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(258, 13);
			this.label8.TabIndex = 101;
			this.label8.Text = "Range must be at least 1,000,000 numbers.";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(24, 313);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(86, 24);
			this.butDelete.TabIndex = 12;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textServerId
			// 
			this.textServerId.Location = new System.Drawing.Point(238, 64);
			this.textServerId.MaxVal = 2000000000;
			this.textServerId.Name = "textServerId";
			this.textServerId.Size = new System.Drawing.Size(100, 20);
			this.textServerId.TabIndex = 1;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(488, 313);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 10;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(579, 313);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 11;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(258, 248);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(368, 46);
			this.label11.TabIndex = 104;
			this.label11.Text = "User queries with CREATE TABLE or DROP TABLE syntax can only be run when connecte" +
    "d to this server.  Allowing such queries to run on non-report servers can cause " +
    "replication to crash.";
			// 
			// checkReportServer
			// 
			this.checkReportServer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReportServer.Location = new System.Drawing.Point(16, 248);
			this.checkReportServer.Name = "checkReportServer";
			this.checkReportServer.Size = new System.Drawing.Size(236, 18);
			this.checkReportServer.TabIndex = 7;
			this.checkReportServer.Text = "Report Server";
			this.checkReportServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReportServer.UseVisualStyleBackColor = true;
			// 
			// butThisComputerDesc
			// 
			this.butThisComputerDesc.Location = new System.Drawing.Point(562, 19);
			this.butThisComputerDesc.Name = "butThisComputerDesc";
			this.butThisComputerDesc.Size = new System.Drawing.Size(87, 24);
			this.butThisComputerDesc.TabIndex = 105;
			this.butThisComputerDesc.Text = "This Computer";
			this.butThisComputerDesc.Click += new System.EventHandler(this.butThisComputerDesc_Click);
			// 
			// FormReplicationEdit
			// 
			this.ClientSize = new System.Drawing.Size(678, 353);
			this.Controls.Add(this.butThisComputerDesc);
			this.Controls.Add(this.checkReportServer);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.checkUpdateBlocked);
			this.Controls.Add(this.textAtoZpath);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textRangeEnd);
			this.Controls.Add(this.textRangeStart);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textServerId);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReplicationEdit";
			this.Text = "Edit Replication Server";
			this.Load += new System.EventHandler(this.FormReplicationEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescript;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private ValidNum textServerId;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textRangeStart;
		private System.Windows.Forms.TextBox textRangeEnd;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textAtoZpath;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkUpdateBlocked;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox checkReportServer;
		private UI.Button butThisComputerDesc;
	}
}