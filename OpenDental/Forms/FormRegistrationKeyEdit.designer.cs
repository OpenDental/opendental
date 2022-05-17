namespace OpenDental {
	partial class FormRegistrationKeyEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRegistrationKeyEdit));
			this.textKey = new System.Windows.Forms.TextBox();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.checkForeign = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.checkFree = new System.Windows.Forms.CheckBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.checkTesting = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textVotesAllotted = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.checkResellerCustomer = new System.Windows.Forms.CheckBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.butMoveTo = new OpenDental.UI.Button();
			this.butNow = new OpenDental.UI.Button();
			this.textDateEnded = new OpenDental.ValidDate();
			this.textDateDisabled = new OpenDental.ValidDate();
			this.textDateStarted = new OpenDental.ValidDate();
			this.butPracticeTitleReset = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label15 = new System.Windows.Forms.Label();
			this.checkEarlyAccess = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// textKey
			// 
			this.textKey.Location = new System.Drawing.Point(134, 10);
			this.textKey.Name = "textKey";
			this.textKey.ReadOnly = true;
			this.textKey.Size = new System.Drawing.Size(269, 20);
			this.textKey.TabIndex = 2;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(134, 343);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(269, 124);
			this.textNote.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(23, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(110, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Registration Key";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(59, 346);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Note";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkForeign
			// 
			this.checkForeign.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkForeign.Location = new System.Drawing.Point(44, 61);
			this.checkForeign.Name = "checkForeign";
			this.checkForeign.Size = new System.Drawing.Size(104, 18);
			this.checkForeign.TabIndex = 7;
			this.checkForeign.Text = "Foreign";
			this.checkForeign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkForeign.UseVisualStyleBackColor = true;
			this.checkForeign.Click += new System.EventHandler(this.checkForeign_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(23, 172);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(110, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "Date Started";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(23, 205);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(110, 16);
			this.label4.TabIndex = 11;
			this.label4.Text = "Date Disabled";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(23, 273);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(110, 16);
			this.label5.TabIndex = 13;
			this.label5.Text = "Date Ended";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(236, 172);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(518, 16);
			this.label6.TabIndex = 14;
			this.label6.Text = "Not accurate before 11/07";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(236, 203);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(518, 67);
			this.label7.TabIndex = 15;
			this.label7.Text = resources.GetString("label7.Text");
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(304, 270);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(450, 32);
			this.label8.TabIndex = 16;
			this.label8.Text = "If a customer drops monthly service, this date should reflect their last date of " +
    "coverage.  They will still be able to download bug fixes.";
			// 
			// checkFree
			// 
			this.checkFree.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFree.Location = new System.Drawing.Point(44, 82);
			this.checkFree.Name = "checkFree";
			this.checkFree.Size = new System.Drawing.Size(104, 18);
			this.checkFree.TabIndex = 18;
			this.checkFree.Text = "Free Version";
			this.checkFree.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFree.UseVisualStyleBackColor = true;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(150, 82);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(604, 16);
			this.label9.TabIndex = 19;
			this.label9.Text = "Only for very poor countries.  Must make absolutely sure that the country meets t" +
    "he requirements.";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(150, 103);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(604, 16);
			this.label10.TabIndex = 21;
			this.label10.Text = "Only for approved developers.  Cannot be used with live patient data.";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkTesting
			// 
			this.checkTesting.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTesting.Location = new System.Drawing.Point(12, 103);
			this.checkTesting.Name = "checkTesting";
			this.checkTesting.Size = new System.Drawing.Size(136, 18);
			this.checkTesting.TabIndex = 20;
			this.checkTesting.Text = "For Testing";
			this.checkTesting.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTesting.UseVisualStyleBackColor = true;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(23, 309);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(110, 16);
			this.label11.TabIndex = 23;
			this.label11.Text = "Votes Allotted";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVotesAllotted
			// 
			this.textVotesAllotted.Location = new System.Drawing.Point(134, 308);
			this.textVotesAllotted.Name = "textVotesAllotted";
			this.textVotesAllotted.Size = new System.Drawing.Size(100, 20);
			this.textVotesAllotted.TabIndex = 24;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(240, 309);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(514, 16);
			this.label12.TabIndex = 25;
			this.label12.Text = "Typically 100, although it can be more for multilocation offices.  Sometimes 0.";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label13
			// 
			this.label13.Enabled = false;
			this.label13.Location = new System.Drawing.Point(150, 124);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(604, 16);
			this.label13.TabIndex = 28;
			this.label13.Text = "Only for customers of resellers.  Helps keep track of customers that we bill via " +
    "passthrough to a reseller.";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkResellerCustomer
			// 
			this.checkResellerCustomer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkResellerCustomer.Enabled = false;
			this.checkResellerCustomer.Location = new System.Drawing.Point(12, 124);
			this.checkResellerCustomer.Name = "checkResellerCustomer";
			this.checkResellerCustomer.Size = new System.Drawing.Size(136, 18);
			this.checkResellerCustomer.TabIndex = 27;
			this.checkResellerCustomer.Text = "Reseller Customer";
			this.checkResellerCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkResellerCustomer.UseVisualStyleBackColor = true;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(23, 37);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(110, 16);
			this.label14.TabIndex = 30;
			this.label14.Text = "PatNum";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(134, 36);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.ReadOnly = true;
			this.textPatNum.Size = new System.Drawing.Size(100, 20);
			this.textPatNum.TabIndex = 29;
			// 
			// butMoveTo
			// 
			this.butMoveTo.Location = new System.Drawing.Point(239, 33);
			this.butMoveTo.Name = "butMoveTo";
			this.butMoveTo.Size = new System.Drawing.Size(75, 24);
			this.butMoveTo.TabIndex = 26;
			this.butMoveTo.Text = "Move To";
			this.butMoveTo.Click += new System.EventHandler(this.butMoveTo_Click);
			// 
			// butNow
			// 
			this.butNow.Location = new System.Drawing.Point(240, 272);
			this.butNow.Name = "butNow";
			this.butNow.Size = new System.Drawing.Size(62, 21);
			this.butNow.TabIndex = 17;
			this.butNow.Text = "Now";
			this.butNow.Click += new System.EventHandler(this.butNow_Click);
			// 
			// textDateEnded
			// 
			this.textDateEnded.Location = new System.Drawing.Point(134, 272);
			this.textDateEnded.Name = "textDateEnded";
			this.textDateEnded.Size = new System.Drawing.Size(100, 20);
			this.textDateEnded.TabIndex = 12;
			// 
			// textDateDisabled
			// 
			this.textDateDisabled.Location = new System.Drawing.Point(134, 204);
			this.textDateDisabled.Name = "textDateDisabled";
			this.textDateDisabled.Size = new System.Drawing.Size(100, 20);
			this.textDateDisabled.TabIndex = 10;
			// 
			// textDateStarted
			// 
			this.textDateStarted.Location = new System.Drawing.Point(134, 171);
			this.textDateStarted.Name = "textDateStarted";
			this.textDateStarted.Size = new System.Drawing.Size(100, 20);
			this.textDateStarted.TabIndex = 8;
			// 
			// butPracticeTitleReset
			// 
			this.butPracticeTitleReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPracticeTitleReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPracticeTitleReset.Location = new System.Drawing.Point(134, 485);
			this.butPracticeTitleReset.Name = "butPracticeTitleReset";
			this.butPracticeTitleReset.Size = new System.Drawing.Size(131, 26);
			this.butPracticeTitleReset.TabIndex = 6;
			this.butPracticeTitleReset.Text = "Reset Practice Title";
			this.butPracticeTitleReset.Visible = false;
			this.butPracticeTitleReset.Click += new System.EventHandler(this.butPracticeTitleReset_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1037, 485);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1118, 485);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(150, 145);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(604, 16);
			this.label15.TabIndex = 32;
			this.label15.Text = "Only for approved registration keys that need early access.  E.g. allows access t" +
    "o Alpha version.";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkEarlyAccess
			// 
			this.checkEarlyAccess.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEarlyAccess.Location = new System.Drawing.Point(12, 145);
			this.checkEarlyAccess.Name = "checkEarlyAccess";
			this.checkEarlyAccess.Size = new System.Drawing.Size(136, 18);
			this.checkEarlyAccess.TabIndex = 31;
			this.checkEarlyAccess.Text = "Early Access";
			this.checkEarlyAccess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEarlyAccess.UseVisualStyleBackColor = true;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(760, 10);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(433, 457);
			this.gridMain.TabIndex = 33;
			// 
			// FormRegistrationKeyEdit
			// 
			this.ClientSize = new System.Drawing.Size(1205, 536);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.checkEarlyAccess);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.textPatNum);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.checkResellerCustomer);
			this.Controls.Add(this.butMoveTo);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.textVotesAllotted);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.checkTesting);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.checkFree);
			this.Controls.Add(this.butNow);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textDateEnded);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textDateDisabled);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDateStarted);
			this.Controls.Add(this.checkForeign);
			this.Controls.Add(this.butPracticeTitleReset);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.textKey);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRegistrationKeyEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Registration Key";
			this.Load += new System.EventHandler(this.FormRegistrationKeyEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textKey;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkForeign;
		private OpenDental.ValidDate textDateStarted;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private OpenDental.ValidDate textDateDisabled;
		private System.Windows.Forms.Label label5;
		private OpenDental.ValidDate textDateEnded;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.Button butNow;
		private System.Windows.Forms.CheckBox checkFree;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkTesting;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textVotesAllotted;
		private System.Windows.Forms.Label label12;
		private OpenDental.UI.Button butMoveTo;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox checkResellerCustomer;
		private OpenDental.UI.Button butPracticeTitleReset;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.CheckBox checkEarlyAccess;
		private UI.GridOD gridMain;
	}
}
