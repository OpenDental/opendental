namespace UnitTests {
	partial class FormUnitTests {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUnitTests));
			this.label1 = new System.Windows.Forms.Label();
			this.butNewDb = new System.Windows.Forms.Button();
			this.butRun = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textSpecificTest = new System.Windows.Forms.TextBox();
			this.butWebService = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.butCore = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.listType = new System.Windows.Forms.ListBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butSchema = new System.Windows.Forms.Button();
			this.radioSchema1 = new System.Windows.Forms.RadioButton();
			this.radioSchema2 = new System.Windows.Forms.RadioButton();
			this.label8 = new System.Windows.Forms.Label();
			this.butHL7 = new System.Windows.Forms.Button();
			this.textAddr = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textPort = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.groupDatabase = new System.Windows.Forms.GroupBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.textMiddleTierUser = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textMiddleTierURI = new System.Windows.Forms.TextBox();
			this.textMiddleTierPassword = new System.Windows.Forms.TextBox();
			this.textResults = new System.Windows.Forms.RichTextBox();
			this.tabControlConns = new System.Windows.Forms.TabControl();
			this.tabPageLocal = new System.Windows.Forms.TabPage();
			this.tabPageMiddleTier = new System.Windows.Forms.TabPage();
			this.label15 = new System.Windows.Forms.Label();
			this.butWebServiceParamCheck = new System.Windows.Forms.Button();
			this.groupDatabase.SuspendLayout();
			this.tabControlConns.SuspendLayout();
			this.tabPageLocal.SuspendLayout();
			this.tabPageMiddleTier.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(197, 109);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(577, 18);
			this.label1.TabIndex = 3;
			this.label1.Text = "Before running the tests below, make sure \'unittest\' database exists.  Only neede" +
    "d for new versions.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butNewDb
			// 
			this.butNewDb.Location = new System.Drawing.Point(116, 106);
			this.butNewDb.Name = "butNewDb";
			this.butNewDb.Size = new System.Drawing.Size(75, 23);
			this.butNewDb.TabIndex = 0;
			this.butNewDb.Text = "Fresh Db";
			this.butNewDb.UseVisualStyleBackColor = true;
			this.butNewDb.Click += new System.EventHandler(this.butNewDb_Click);
			// 
			// butRun
			// 
			this.butRun.Location = new System.Drawing.Point(100, 86);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(75, 23);
			this.butRun.TabIndex = 7;
			this.butRun.Text = "Run";
			this.butRun.UseVisualStyleBackColor = true;
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(180, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "Specific test #";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSpecificTest
			// 
			this.textSpecificTest.Location = new System.Drawing.Point(265, 88);
			this.textSpecificTest.Name = "textSpecificTest";
			this.textSpecificTest.Size = new System.Drawing.Size(74, 20);
			this.textSpecificTest.TabIndex = 9;
			// 
			// butWebService
			// 
			this.butWebService.Location = new System.Drawing.Point(100, 64);
			this.butWebService.Name = "butWebService";
			this.butWebService.Size = new System.Drawing.Size(75, 23);
			this.butWebService.TabIndex = 10;
			this.butWebService.Text = "Run";
			this.butWebService.UseVisualStyleBackColor = true;
			this.butWebService.Click += new System.EventHandler(this.butWebService_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(98, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(600, 18);
			this.label4.TabIndex = 11;
			this.label4.Text = "Set both this project and OpenDentalServer as startup.  Edit OpenDentalServer.Ope" +
    "nDentalServerConfig.xml to be valid.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butCore
			// 
			this.butCore.Location = new System.Drawing.Point(100, 34);
			this.butCore.Name = "butCore";
			this.butCore.Size = new System.Drawing.Size(75, 23);
			this.butCore.TabIndex = 12;
			this.butCore.Text = "Core Types";
			this.butCore.UseVisualStyleBackColor = true;
			this.butCore.Click += new System.EventHandler(this.butCore_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(181, 37);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(488, 18);
			this.label5.TabIndex = 13;
			this.label5.Text = "Stores and retrieves core data types in database, ensuring that db engine and con" +
    "nector are working.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listType
			// 
			this.listType.FormattingEnabled = true;
			this.listType.Location = new System.Drawing.Point(337, 33);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(99, 30);
			this.listType.TabIndex = 22;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(334, 12);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(99, 18);
			this.label7.TabIndex = 21;
			this.label7.Text = "Database Type";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butSchema
			// 
			this.butSchema.Location = new System.Drawing.Point(100, 8);
			this.butSchema.Name = "butSchema";
			this.butSchema.Size = new System.Drawing.Size(75, 23);
			this.butSchema.TabIndex = 23;
			this.butSchema.Text = "Schema";
			this.butSchema.UseVisualStyleBackColor = true;
			this.butSchema.Click += new System.EventHandler(this.butSchema_Click);
			// 
			// radioSchema1
			// 
			this.radioSchema1.Checked = true;
			this.radioSchema1.Location = new System.Drawing.Point(185, 11);
			this.radioSchema1.Name = "radioSchema1";
			this.radioSchema1.Size = new System.Drawing.Size(133, 18);
			this.radioSchema1.TabIndex = 24;
			this.radioSchema1.TabStop = true;
			this.radioSchema1.Text = "Test proposed crud";
			this.radioSchema1.UseVisualStyleBackColor = true;
			// 
			// radioSchema2
			// 
			this.radioSchema2.Location = new System.Drawing.Point(322, 11);
			this.radioSchema2.Name = "radioSchema2";
			this.radioSchema2.Size = new System.Drawing.Size(189, 18);
			this.radioSchema2.TabIndex = 25;
			this.radioSchema2.Text = "Compare proposed to generated";
			this.radioSchema2.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(180, 62);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(445, 18);
			this.label8.TabIndex = 29;
			this.label8.Text = "This will test the old eCW HL7 message processing as well as the new HL7Defs.";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butHL7
			// 
			this.butHL7.Location = new System.Drawing.Point(100, 60);
			this.butHL7.Name = "butHL7";
			this.butHL7.Size = new System.Drawing.Size(75, 23);
			this.butHL7.TabIndex = 28;
			this.butHL7.Text = "HL7";
			this.butHL7.UseVisualStyleBackColor = true;
			this.butHL7.Click += new System.EventHandler(this.butHL7_Click);
			// 
			// textAddr
			// 
			this.textAddr.Location = new System.Drawing.Point(9, 32);
			this.textAddr.Multiline = true;
			this.textAddr.Name = "textAddr";
			this.textAddr.Size = new System.Drawing.Size(150, 20);
			this.textAddr.TabIndex = 1;
			this.textAddr.Text = "localhost";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(9, 16);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(94, 13);
			this.label9.TabIndex = 31;
			this.label9.Text = "Database Address";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(10, 57);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(66, 13);
			this.label10.TabIndex = 32;
			this.label10.Text = "Port Number";
			// 
			// textPort
			// 
			this.textPort.Location = new System.Drawing.Point(10, 74);
			this.textPort.Name = "textPort";
			this.textPort.Size = new System.Drawing.Size(150, 20);
			this.textPort.TabIndex = 2;
			this.textPort.Text = "3306";
			// 
			// labelPassword
			// 
			this.labelPassword.AutoSize = true;
			this.labelPassword.Location = new System.Drawing.Point(179, 57);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(91, 13);
			this.labelPassword.TabIndex = 35;
			this.labelPassword.Text = "MySQL Password";
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(179, 73);
			this.textPassword.Multiline = true;
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(150, 20);
			this.textPassword.TabIndex = 4;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(179, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(67, 13);
			this.label11.TabIndex = 37;
			this.label11.Text = "MySQL User";
			// 
			// textUserName
			// 
			this.textUserName.Location = new System.Drawing.Point(179, 32);
			this.textUserName.Multiline = true;
			this.textUserName.Name = "textUserName";
			this.textUserName.Size = new System.Drawing.Size(150, 20);
			this.textUserName.TabIndex = 3;
			this.textUserName.Text = "root";
			// 
			// groupDatabase
			// 
			this.groupDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupDatabase.Controls.Add(this.label12);
			this.groupDatabase.Controls.Add(this.label13);
			this.groupDatabase.Controls.Add(this.textMiddleTierUser);
			this.groupDatabase.Controls.Add(this.label14);
			this.groupDatabase.Controls.Add(this.textMiddleTierURI);
			this.groupDatabase.Controls.Add(this.textMiddleTierPassword);
			this.groupDatabase.Controls.Add(this.label9);
			this.groupDatabase.Controls.Add(this.label11);
			this.groupDatabase.Controls.Add(this.label7);
			this.groupDatabase.Controls.Add(this.textUserName);
			this.groupDatabase.Controls.Add(this.listType);
			this.groupDatabase.Controls.Add(this.labelPassword);
			this.groupDatabase.Controls.Add(this.textAddr);
			this.groupDatabase.Controls.Add(this.textPassword);
			this.groupDatabase.Controls.Add(this.label10);
			this.groupDatabase.Controls.Add(this.textPort);
			this.groupDatabase.Location = new System.Drawing.Point(12, 0);
			this.groupDatabase.Name = "groupDatabase";
			this.groupDatabase.Size = new System.Drawing.Size(789, 100);
			this.groupDatabase.TabIndex = 38;
			this.groupDatabase.TabStop = false;
			this.groupDatabase.Text = "Connection Settings";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(442, 16);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(81, 13);
			this.label12.TabIndex = 41;
			this.label12.Text = "Middle Tier URI";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(442, 57);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(92, 13);
			this.label13.TabIndex = 43;
			this.label13.Text = "Open Dental User";
			// 
			// textMiddleTierUser
			// 
			this.textMiddleTierUser.Location = new System.Drawing.Point(442, 73);
			this.textMiddleTierUser.Multiline = true;
			this.textMiddleTierUser.Name = "textMiddleTierUser";
			this.textMiddleTierUser.Size = new System.Drawing.Size(150, 20);
			this.textMiddleTierUser.TabIndex = 39;
			this.textMiddleTierUser.Text = "Admin";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(609, 57);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(116, 13);
			this.label14.TabIndex = 42;
			this.label14.Text = "Open Dental Password";
			// 
			// textMiddleTierURI
			// 
			this.textMiddleTierURI.Location = new System.Drawing.Point(442, 32);
			this.textMiddleTierURI.Multiline = true;
			this.textMiddleTierURI.Name = "textMiddleTierURI";
			this.textMiddleTierURI.Size = new System.Drawing.Size(320, 20);
			this.textMiddleTierURI.TabIndex = 38;
			this.textMiddleTierURI.Text = "http://localhost:49262/ServiceMain.asmx";
			// 
			// textMiddleTierPassword
			// 
			this.textMiddleTierPassword.Location = new System.Drawing.Point(609, 73);
			this.textMiddleTierPassword.Multiline = true;
			this.textMiddleTierPassword.Name = "textMiddleTierPassword";
			this.textMiddleTierPassword.Size = new System.Drawing.Size(150, 20);
			this.textMiddleTierPassword.TabIndex = 40;
			this.textMiddleTierPassword.Text = "pass";
			// 
			// textResults
			// 
			this.textResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textResults.Location = new System.Drawing.Point(12, 288);
			this.textResults.Name = "textResults";
			this.textResults.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textResults.Size = new System.Drawing.Size(789, 507);
			this.textResults.TabIndex = 1;
			this.textResults.Text = "";
			// 
			// tabControlConns
			// 
			this.tabControlConns.Controls.Add(this.tabPageLocal);
			this.tabControlConns.Controls.Add(this.tabPageMiddleTier);
			this.tabControlConns.Location = new System.Drawing.Point(12, 137);
			this.tabControlConns.Name = "tabControlConns";
			this.tabControlConns.SelectedIndex = 0;
			this.tabControlConns.Size = new System.Drawing.Size(789, 145);
			this.tabControlConns.TabIndex = 39;
			// 
			// tabPageLocal
			// 
			this.tabPageLocal.Controls.Add(this.butSchema);
			this.tabPageLocal.Controls.Add(this.butRun);
			this.tabPageLocal.Controls.Add(this.label8);
			this.tabPageLocal.Controls.Add(this.butHL7);
			this.tabPageLocal.Controls.Add(this.textSpecificTest);
			this.tabPageLocal.Controls.Add(this.butCore);
			this.tabPageLocal.Controls.Add(this.label5);
			this.tabPageLocal.Controls.Add(this.radioSchema2);
			this.tabPageLocal.Controls.Add(this.radioSchema1);
			this.tabPageLocal.Controls.Add(this.label3);
			this.tabPageLocal.Location = new System.Drawing.Point(4, 22);
			this.tabPageLocal.Name = "tabPageLocal";
			this.tabPageLocal.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageLocal.Size = new System.Drawing.Size(781, 119);
			this.tabPageLocal.TabIndex = 0;
			this.tabPageLocal.Text = "Local";
			this.tabPageLocal.UseVisualStyleBackColor = true;
			// 
			// tabPageMiddleTier
			// 
			this.tabPageMiddleTier.Controls.Add(this.label15);
			this.tabPageMiddleTier.Controls.Add(this.butWebServiceParamCheck);
			this.tabPageMiddleTier.Controls.Add(this.butWebService);
			this.tabPageMiddleTier.Controls.Add(this.label4);
			this.tabPageMiddleTier.Location = new System.Drawing.Point(4, 22);
			this.tabPageMiddleTier.Name = "tabPageMiddleTier";
			this.tabPageMiddleTier.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMiddleTier.Size = new System.Drawing.Size(781, 119);
			this.tabPageMiddleTier.TabIndex = 1;
			this.tabPageMiddleTier.Text = "Middle Tier";
			this.tabPageMiddleTier.UseVisualStyleBackColor = true;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(178, 40);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(597, 18);
			this.label15.TabIndex = 13;
			this.label15.Text = "Verifies that every public static method within all classes within the directory " +
    "above has correct parameters.";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butWebServiceParamCheck
			// 
			this.butWebServiceParamCheck.Location = new System.Drawing.Point(100, 38);
			this.butWebServiceParamCheck.Name = "butWebServiceParamCheck";
			this.butWebServiceParamCheck.Size = new System.Drawing.Size(75, 23);
			this.butWebServiceParamCheck.TabIndex = 12;
			this.butWebServiceParamCheck.Text = "Params";
			this.butWebServiceParamCheck.UseVisualStyleBackColor = true;
			this.butWebServiceParamCheck.Click += new System.EventHandler(this.butWebServiceParamCheck_Click);
			// 
			// FormUnitTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(813, 807);
			this.Controls.Add(this.tabControlConns);
			this.Controls.Add(this.groupDatabase);
			this.Controls.Add(this.butNewDb);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textResults);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormUnitTests";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormUnitTests";
			this.Load += new System.EventHandler(this.FormUnitTests_Load);
			this.groupDatabase.ResumeLayout(false);
			this.groupDatabase.PerformLayout();
			this.tabControlConns.ResumeLayout(false);
			this.tabPageLocal.ResumeLayout(false);
			this.tabPageLocal.PerformLayout();
			this.tabPageMiddleTier.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button butNewDb;
		private System.Windows.Forms.Button butRun;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textSpecificTest;
		private System.Windows.Forms.Button butWebService;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button butCore;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ListBox listType;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button butSchema;
		private System.Windows.Forms.RadioButton radioSchema1;
		private System.Windows.Forms.RadioButton radioSchema2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button butHL7;
		private System.Windows.Forms.TextBox textAddr;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textPort;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textUserName;
		private System.Windows.Forms.GroupBox groupDatabase;
		private System.Windows.Forms.RichTextBox textResults;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textMiddleTierUser;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textMiddleTierURI;
		private System.Windows.Forms.TextBox textMiddleTierPassword;
		private System.Windows.Forms.TabControl tabControlConns;
		private System.Windows.Forms.TabPage tabPageLocal;
		private System.Windows.Forms.TabPage tabPageMiddleTier;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Button butWebServiceParamCheck;
	}
}

