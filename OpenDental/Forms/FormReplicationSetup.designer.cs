namespace OpenDental{
	partial class FormReplicationSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReplicationSetup));
			this.checkRandomPrimaryKeys = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butSynch = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBoxReplicationFailure = new System.Windows.Forms.GroupBox();
			this.butClearReplicationFailureAtServer_id = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.textReplicaitonFailureAtServer_id = new System.Windows.Forms.TextBox();
			this.butTest = new OpenDental.UI.Button();
			this.butSetRanges = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBoxReplicationFailure.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkRandomPrimaryKeys
			// 
			this.checkRandomPrimaryKeys.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRandomPrimaryKeys.Location = new System.Drawing.Point(17, 15);
			this.checkRandomPrimaryKeys.Name = "checkRandomPrimaryKeys";
			this.checkRandomPrimaryKeys.Size = new System.Drawing.Size(346, 17);
			this.checkRandomPrimaryKeys.TabIndex = 56;
			this.checkRandomPrimaryKeys.Text = "Use Random Primary Keys (BE VERY CAREFUL.  IRREVERSIBLE)";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(113, 475);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(550, 35);
			this.label1.TabIndex = 61;
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(113, 517);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(431, 35);
			this.label2.TabIndex = 63;
			this.label2.Text = "Use the Test button to generate and display some sample key values for this compu" +
    "ter.  The displayed values should fall within the range set above.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(103, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(431, 45);
			this.label3.TabIndex = 65;
			this.label3.Text = resources.GetString("label3.Text");
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.textPassword);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textUsername);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.butSynch);
			this.groupBox1.Location = new System.Drawing.Point(10, 556);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(771, 66);
			this.groupBox1.TabIndex = 66;
			this.groupBox1.Text = "Force Synch";
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(641, 40);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(125, 20);
			this.textPassword.TabIndex = 69;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(530, 40);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(111, 18);
			this.label5.TabIndex = 68;
			this.label5.Text = "Password";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(641, 18);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(125, 20);
			this.textUsername.TabIndex = 67;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(530, 18);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(111, 18);
			this.label4.TabIndex = 66;
			this.label4.Text = "MySQL Username";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSynch
			// 
			this.butSynch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSynch.Location = new System.Drawing.Point(7, 26);
			this.butSynch.Name = "butSynch";
			this.butSynch.Size = new System.Drawing.Size(90, 24);
			this.butSynch.TabIndex = 64;
			this.butSynch.Text = "Synch";
			this.butSynch.Click += new System.EventHandler(this.butSynch_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(17, 42);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(764, 417);
			this.gridMain.TabIndex = 57;
			this.gridMain.Title = "Servers";
			this.gridMain.TranslationName = "FormReplicationSetup";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// groupBoxReplicationFailure
			// 
			this.groupBoxReplicationFailure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBoxReplicationFailure.Controls.Add(this.butClearReplicationFailureAtServer_id);
			this.groupBoxReplicationFailure.Controls.Add(this.label6);
			this.groupBoxReplicationFailure.Controls.Add(this.textReplicaitonFailureAtServer_id);
			this.groupBoxReplicationFailure.ForeColor = System.Drawing.Color.Red;
			this.groupBoxReplicationFailure.Location = new System.Drawing.Point(550, 502);
			this.groupBoxReplicationFailure.Name = "groupBoxReplicationFailure";
			this.groupBoxReplicationFailure.Size = new System.Drawing.Size(323, 50);
			this.groupBoxReplicationFailure.TabIndex = 67;
			this.groupBoxReplicationFailure.Text = "Replication Failure Detected";
			this.groupBoxReplicationFailure.Visible = false;
			// 
			// butClearReplicationFailureAtServer_id
			// 
			this.butClearReplicationFailureAtServer_id.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClearReplicationFailureAtServer_id.ForeColor = System.Drawing.SystemColors.ControlText;
			this.butClearReplicationFailureAtServer_id.Location = new System.Drawing.Point(248, 19);
			this.butClearReplicationFailureAtServer_id.Name = "butClearReplicationFailureAtServer_id";
			this.butClearReplicationFailureAtServer_id.Size = new System.Drawing.Size(69, 24);
			this.butClearReplicationFailureAtServer_id.TabIndex = 2;
			this.butClearReplicationFailureAtServer_id.Text = "Clear";
			this.butClearReplicationFailureAtServer_id.Click += new System.EventHandler(this.butResetReplicationFailureAtServer_id_Click);
			// 
			// label6
			// 
			this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label6.Location = new System.Drawing.Point(6, 22);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(155, 18);
			this.label6.TabIndex = 66;
			this.label6.Text = "Replication Failure at Server_id";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReplicaitonFailureAtServer_id
			// 
			this.textReplicaitonFailureAtServer_id.Location = new System.Drawing.Point(167, 22);
			this.textReplicaitonFailureAtServer_id.Name = "textReplicaitonFailureAtServer_id";
			this.textReplicaitonFailureAtServer_id.ReadOnly = true;
			this.textReplicaitonFailureAtServer_id.Size = new System.Drawing.Size(75, 20);
			this.textReplicaitonFailureAtServer_id.TabIndex = 67;
			// 
			// butTest
			// 
			this.butTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butTest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butTest.Location = new System.Drawing.Point(17, 522);
			this.butTest.Name = "butTest";
			this.butTest.Size = new System.Drawing.Size(90, 24);
			this.butTest.TabIndex = 62;
			this.butTest.Text = "Test";
			this.butTest.Click += new System.EventHandler(this.butTest_Click);
			// 
			// butSetRanges
			// 
			this.butSetRanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSetRanges.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSetRanges.Location = new System.Drawing.Point(17, 481);
			this.butSetRanges.Name = "butSetRanges";
			this.butSetRanges.Size = new System.Drawing.Size(90, 24);
			this.butSetRanges.TabIndex = 59;
			this.butSetRanges.Text = "Set Ranges";
			this.butSetRanges.Click += new System.EventHandler(this.butSetRanges_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(798, 435);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 58;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(798, 598);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormReplicationSetup
			// 
			this.ClientSize = new System.Drawing.Size(885, 636);
			this.Controls.Add(this.groupBoxReplicationFailure);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butTest);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSetRanges);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.checkRandomPrimaryKeys);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReplicationSetup";
			this.Text = "Replication Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormReplicationSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormReplicationSetup_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBoxReplicationFailure.ResumeLayout(false);
			this.groupBoxReplicationFailure.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.CheckBox checkRandomPrimaryKeys;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butSetRanges;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butTest;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butSynch;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textUsername;
		private System.Windows.Forms.GroupBox groupBoxReplicationFailure;
		private UI.Button butClearReplicationFailureAtServer_id;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textReplicaitonFailureAtServer_id;
	}
}