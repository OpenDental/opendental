namespace CentralManager{
	partial class FormCentralConnectionEdit {
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
			this.butOK = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.textServiceURI = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new System.Windows.Forms.Button();
			this.textServerName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textDatabaseName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textMySqlUser = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textMySqlPassword = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkWebServiceIsEcw = new System.Windows.Forms.CheckBox();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.checkClinicBreakdown = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(520, 362);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(601, 362);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textServiceURI
			// 
			this.textServiceURI.Location = new System.Drawing.Point(120, 19);
			this.textServiceURI.Name = "textServiceURI";
			this.textServiceURI.Size = new System.Drawing.Size(524, 20);
			this.textServiceURI.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(6, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(111, 17);
			this.label2.TabIndex = 200;
			this.label2.Text = "Remote URI";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(13, 362);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 205;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textServerName
			// 
			this.textServerName.Location = new System.Drawing.Point(120, 20);
			this.textServerName.Name = "textServerName";
			this.textServerName.Size = new System.Drawing.Size(190, 20);
			this.textServerName.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label4.Location = new System.Drawing.Point(6, 23);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(111, 17);
			this.label4.TabIndex = 207;
			this.label4.Text = "Server Name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDatabaseName
			// 
			this.textDatabaseName.Location = new System.Drawing.Point(120, 46);
			this.textDatabaseName.Name = "textDatabaseName";
			this.textDatabaseName.Size = new System.Drawing.Size(190, 20);
			this.textDatabaseName.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label5.Location = new System.Drawing.Point(6, 49);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(111, 17);
			this.label5.TabIndex = 209;
			this.label5.Text = "Database";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMySqlUser
			// 
			this.textMySqlUser.Location = new System.Drawing.Point(120, 72);
			this.textMySqlUser.Name = "textMySqlUser";
			this.textMySqlUser.Size = new System.Drawing.Size(190, 20);
			this.textMySqlUser.TabIndex = 2;
			// 
			// label6
			// 
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label6.Location = new System.Drawing.Point(6, 75);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(111, 17);
			this.label6.TabIndex = 211;
			this.label6.Text = "MySql User";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMySqlPassword
			// 
			this.textMySqlPassword.Location = new System.Drawing.Point(120, 98);
			this.textMySqlPassword.Name = "textMySqlPassword";
			this.textMySqlPassword.Size = new System.Drawing.Size(190, 20);
			this.textMySqlPassword.TabIndex = 3;
			this.textMySqlPassword.TextChanged += new System.EventHandler(this.textMySqlPassword_TextChanged);
			this.textMySqlPassword.Leave += new System.EventHandler(this.textMySqlPassword_Leave);
			// 
			// label7
			// 
			this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label7.Location = new System.Drawing.Point(6, 101);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(111, 17);
			this.label7.TabIndex = 213;
			this.label7.Text = "MySql Password";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textMySqlPassword);
			this.groupBox1.Controls.Add(this.textServerName);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textMySqlUser);
			this.groupBox1.Controls.Add(this.textDatabaseName);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Location = new System.Drawing.Point(15, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(341, 134);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Direct Database Connection";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkWebServiceIsEcw);
			this.groupBox2.Controls.Add(this.textServiceURI);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(15, 176);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(661, 65);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Middle Tier Connection";
			// 
			// checkWebServiceIsEcw
			// 
			this.checkWebServiceIsEcw.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWebServiceIsEcw.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkWebServiceIsEcw.Location = new System.Drawing.Point(2, 43);
			this.checkWebServiceIsEcw.Name = "checkWebServiceIsEcw";
			this.checkWebServiceIsEcw.Size = new System.Drawing.Size(131, 18);
			this.checkWebServiceIsEcw.TabIndex = 1;
			this.checkWebServiceIsEcw.Text = "Using eClinicalWorks";
			this.checkWebServiceIsEcw.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWebServiceIsEcw.UseVisualStyleBackColor = true;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(135, 280);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(368, 61);
			this.textNote.TabIndex = 4;
			// 
			// label9
			// 
			this.label9.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label9.Location = new System.Drawing.Point(21, 283);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(111, 17);
			this.label9.TabIndex = 219;
			this.label9.Text = "Note";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label10.Location = new System.Drawing.Point(15, 155);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(111, 17);
			this.label10.TabIndex = 220;
			this.label10.Text = "OR (but not both)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkClinicBreakdown
			// 
			this.checkClinicBreakdown.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClinicBreakdown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClinicBreakdown.Location = new System.Drawing.Point(18, 247);
			this.checkClinicBreakdown.Name = "checkClinicBreakdown";
			this.checkClinicBreakdown.Size = new System.Drawing.Size(130, 30);
			this.checkClinicBreakdown.TabIndex = 221;
			this.checkClinicBreakdown.Text = "Show clinic breakdown on reports";
			this.checkClinicBreakdown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClinicBreakdown.UseVisualStyleBackColor = true;
			// 
			// FormCentralConnectionEdit
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(688, 398);
			this.Controls.Add(this.checkClinicBreakdown);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.MinimumSize = new System.Drawing.Size(704, 408);
			this.Name = "FormCentralConnectionEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Connection";
			this.Load += new System.EventHandler(this.FormCentralConnectionEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.TextBox textServiceURI;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.TextBox textServerName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textDatabaseName;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textMySqlUser;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textMySqlPassword;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkWebServiceIsEcw;
		private System.Windows.Forms.CheckBox checkClinicBreakdown;
	}
}