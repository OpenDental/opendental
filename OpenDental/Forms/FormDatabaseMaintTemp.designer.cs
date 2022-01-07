namespace OpenDental{
	partial class FormDatabaseMaintTemp {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDatabaseMaintTemp));
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.comboDbs = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textResults = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butFix3 = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.butFix2 = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.butFix1 = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.butBackup = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.butPrint = new OpenDental.UI.Button();
			this.butRun = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(97, 43);
			this.linkLabel1.Location = new System.Drawing.Point(12, 7);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(671, 51);
			this.linkLabel1.TabIndex = 5;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "This tool checks the database to make sure no damage was done by a specific bug a" +
    "s described at\r\nhttp://www.opendental.com/manual/bugcp.html\r\n";
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 59);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(300, 18);
			this.label1.TabIndex = 6;
			this.label1.Text = "Step 1.  Choose the most recent backup of this database.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboDbs
			// 
			this.comboDbs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDbs.FormattingEnabled = true;
			this.comboDbs.Location = new System.Drawing.Point(307, 60);
			this.comboDbs.MaxDropDownItems = 100;
			this.comboDbs.Name = "comboDbs";
			this.comboDbs.Size = new System.Drawing.Size(317, 21);
			this.comboDbs.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 84);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 18);
			this.label2.TabIndex = 8;
			this.label2.Text = "Step 2.  Run";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 109);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(138, 18);
			this.label3.TabIndex = 9;
			this.label3.Text = "Step 3.  View results";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(12, 561);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(138, 18);
			this.label4.TabIndex = 10;
			this.label4.Text = "Step 4.  Print results";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textResults
			// 
			this.textResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textResults.Location = new System.Drawing.Point(15, 130);
			this.textResults.Multiline = true;
			this.textResults.Name = "textResults";
			this.textResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textResults.Size = new System.Drawing.Size(706, 422);
			this.textResults.TabIndex = 11;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.butFix3);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.butFix2);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.butFix1);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.butBackup);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(15, 589);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(609, 93);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Fix (only if needed)";
			// 
			// butFix3
			// 
			this.butFix3.Enabled = false;
			this.butFix3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butFix3.Location = new System.Drawing.Point(527, 62);
			this.butFix3.Name = "butFix3";
			this.butFix3.Size = new System.Drawing.Size(66, 24);
			this.butFix3.TabIndex = 20;
			this.butFix3.Text = "Fix";
			this.butFix3.Click += new System.EventHandler(this.butFix3_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(214, 65);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(309, 18);
			this.label8.TabIndex = 19;
			this.label8.Text = "Fix missing claim payments (by pulling from backup db)";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butFix2
			// 
			this.butFix2.Enabled = false;
			this.butFix2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butFix2.Location = new System.Drawing.Point(527, 37);
			this.butFix2.Name = "butFix2";
			this.butFix2.Size = new System.Drawing.Size(66, 24);
			this.butFix2.TabIndex = 18;
			this.butFix2.Text = "Fix";
			this.butFix2.Click += new System.EventHandler(this.butFix2_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(318, 40);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(205, 18);
			this.label7.TabIndex = 17;
			this.label7.Text = "Fix duplicate supplemental payments";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butFix1
			// 
			this.butFix1.Enabled = false;
			this.butFix1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butFix1.Location = new System.Drawing.Point(527, 12);
			this.butFix1.Name = "butFix1";
			this.butFix1.Size = new System.Drawing.Size(66, 24);
			this.butFix1.TabIndex = 16;
			this.butFix1.Text = "Fix";
			this.butFix1.Click += new System.EventHandler(this.butFix1_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(318, 15);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(205, 18);
			this.label6.TabIndex = 15;
			this.label6.Text = "Fix duplicate claim payments";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butBackup
			// 
			this.butBackup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBackup.Location = new System.Drawing.Point(138, 14);
			this.butBackup.Name = "butBackup";
			this.butBackup.Size = new System.Drawing.Size(75, 24);
			this.butBackup.TabIndex = 14;
			this.butBackup.Text = "Backup";
			this.butBackup.Click += new System.EventHandler(this.butBackup_Click);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(9, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(131, 74);
			this.label5.TabIndex = 13;
			this.label5.Text = "Step 5.  Make a backup.  A backup must be made before any of the buttons at the r" +
    "ight will be available.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(129, 559);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 12;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butRun
			// 
			this.butRun.Location = new System.Drawing.Point(87, 83);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(75, 24);
			this.butRun.TabIndex = 3;
			this.butRun.Text = "Run";
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(646, 658);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormDatabaseMaintTemp
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(733, 694);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.textResults);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboDbs);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.linkLabel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDatabaseMaintTemp";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Temporary Database Integrity Check";
			this.Load += new System.EventHandler(this.FormDatabaseMaintTemp_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butRun;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboDbs;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textResults;
		private OpenDental.UI.Button butPrint;
		private System.Windows.Forms.GroupBox groupBox1;
		private OpenDental.UI.Button butBackup;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.Button butFix1;
		private System.Windows.Forms.Label label6;
		private OpenDental.UI.Button butFix3;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.Button butFix2;
		private System.Windows.Forms.Label label7;
	}
}