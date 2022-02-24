namespace OpenDental {
	partial class FormReferralMerge {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReferralMerge));
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkIsDoctorFrom = new System.Windows.Forms.CheckBox();
			this.butChangeReferralFrom = new OpenDental.UI.Button();
			this.checkIsPersonFrom = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textReferralNameFrom = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textTitleFrom = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkIsDoctorInto = new System.Windows.Forms.CheckBox();
			this.checkIsPersonInto = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textTitleInto = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butChangeReferralInto = new OpenDental.UI.Button();
			this.textReferralNameInto = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butMerge = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkIsDoctorFrom);
			this.groupBox2.Controls.Add(this.butChangeReferralFrom);
			this.groupBox2.Controls.Add(this.checkIsPersonFrom);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.textReferralNameFrom);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.textTitleFrom);
			this.groupBox2.Location = new System.Drawing.Point(12, 110);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(638, 88);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Referral to merge from. This referral will be merged into the referral above.";
			// 
			// checkIsDoctorFrom
			// 
			this.checkIsDoctorFrom.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkIsDoctorFrom.Enabled = false;
			this.checkIsDoctorFrom.Location = new System.Drawing.Point(487, 38);
			this.checkIsDoctorFrom.Name = "checkIsDoctorFrom";
			this.checkIsDoctorFrom.Size = new System.Drawing.Size(17, 18);
			this.checkIsDoctorFrom.TabIndex = 138;
			this.checkIsDoctorFrom.UseVisualStyleBackColor = true;
			// 
			// butChangeReferralFrom
			// 
			this.butChangeReferralFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeReferralFrom.Location = new System.Drawing.Point(550, 32);
			this.butChangeReferralFrom.Name = "butChangeReferralFrom";
			this.butChangeReferralFrom.Size = new System.Drawing.Size(75, 24);
			this.butChangeReferralFrom.TabIndex = 9;
			this.butChangeReferralFrom.Text = "Change";
			this.butChangeReferralFrom.Click += new System.EventHandler(this.butChangeReferralFrom_Click);
			// 
			// checkIsPersonFrom
			// 
			this.checkIsPersonFrom.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkIsPersonFrom.Enabled = false;
			this.checkIsPersonFrom.Location = new System.Drawing.Point(401, 38);
			this.checkIsPersonFrom.Name = "checkIsPersonFrom";
			this.checkIsPersonFrom.Size = new System.Drawing.Size(17, 18);
			this.checkIsPersonFrom.TabIndex = 137;
			this.checkIsPersonFrom.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(4, 21);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(114, 13);
			this.label8.TabIndex = 131;
			this.label8.Text = "Referral Name";
			this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(454, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(86, 13);
			this.label3.TabIndex = 136;
			this.label3.Text = "IsDoctor";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textReferralNameFrom
			// 
			this.textReferralNameFrom.Location = new System.Drawing.Point(7, 37);
			this.textReferralNameFrom.Name = "textReferralNameFrom";
			this.textReferralNameFrom.ReadOnly = true;
			this.textReferralNameFrom.Size = new System.Drawing.Size(237, 20);
			this.textReferralNameFrom.TabIndex = 132;
			this.textReferralNameFrom.TabStop = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(364, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(91, 13);
			this.label4.TabIndex = 135;
			this.label4.Text = "Is Person";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(247, 21);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(85, 13);
			this.label6.TabIndex = 133;
			this.label6.Text = "Title";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textTitleFrom
			// 
			this.textTitleFrom.Location = new System.Drawing.Point(250, 37);
			this.textTitleFrom.Name = "textTitleFrom";
			this.textTitleFrom.ReadOnly = true;
			this.textTitleFrom.Size = new System.Drawing.Size(100, 20);
			this.textTitleFrom.TabIndex = 134;
			this.textTitleFrom.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkIsDoctorInto);
			this.groupBox1.Controls.Add(this.checkIsPersonInto);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textTitleInto);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.butChangeReferralInto);
			this.groupBox1.Controls.Add(this.textReferralNameInto);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(638, 88);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Referral to merge into. The referral below will be merged into this referral.";
			// 
			// checkIsDoctorInto
			// 
			this.checkIsDoctorInto.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkIsDoctorInto.Enabled = false;
			this.checkIsDoctorInto.Location = new System.Drawing.Point(487, 38);
			this.checkIsDoctorInto.Name = "checkIsDoctorInto";
			this.checkIsDoctorInto.Size = new System.Drawing.Size(17, 18);
			this.checkIsDoctorInto.TabIndex = 130;
			this.checkIsDoctorInto.UseVisualStyleBackColor = true;
			// 
			// checkIsPersonInto
			// 
			this.checkIsPersonInto.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkIsPersonInto.Enabled = false;
			this.checkIsPersonInto.Location = new System.Drawing.Point(401, 38);
			this.checkIsPersonInto.Name = "checkIsPersonInto";
			this.checkIsPersonInto.Size = new System.Drawing.Size(17, 18);
			this.checkIsPersonInto.TabIndex = 129;
			this.checkIsPersonInto.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(454, 21);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(86, 13);
			this.label7.TabIndex = 8;
			this.label7.Text = "Is Doctor";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(364, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Is Person";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textTitleInto
			// 
			this.textTitleInto.Location = new System.Drawing.Point(250, 37);
			this.textTitleInto.Name = "textTitleInto";
			this.textTitleInto.ReadOnly = true;
			this.textTitleInto.Size = new System.Drawing.Size(100, 20);
			this.textTitleInto.TabIndex = 6;
			this.textTitleInto.TabStop = false;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(247, 21);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Title";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butChangeReferralInto
			// 
			this.butChangeReferralInto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeReferralInto.Location = new System.Drawing.Point(550, 32);
			this.butChangeReferralInto.Name = "butChangeReferralInto";
			this.butChangeReferralInto.Size = new System.Drawing.Size(75, 24);
			this.butChangeReferralInto.TabIndex = 4;
			this.butChangeReferralInto.Text = "Change";
			this.butChangeReferralInto.Click += new System.EventHandler(this.butChangeReferralInto_Click);
			// 
			// textReferralNameInto
			// 
			this.textReferralNameInto.Location = new System.Drawing.Point(7, 37);
			this.textReferralNameInto.Name = "textReferralNameInto";
			this.textReferralNameInto.ReadOnly = true;
			this.textReferralNameInto.Size = new System.Drawing.Size(237, 20);
			this.textReferralNameInto.TabIndex = 3;
			this.textReferralNameInto.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(114, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Referral Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butMerge
			// 
			this.butMerge.Enabled = false;
			this.butMerge.Location = new System.Drawing.Point(478, 211);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(75, 24);
			this.butMerge.TabIndex = 7;
			this.butMerge.Text = "Merge";
			this.butMerge.Click += new System.EventHandler(this.butMerge_Click);
			// 
			// butClose
			// 
			this.butClose.Location = new System.Drawing.Point(562, 211);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 6;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormReferralMerge
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(666, 249);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butMerge);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReferralMerge";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Merge Referrals";
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private UI.Button butChangeReferralFrom;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textTitleInto;
		private System.Windows.Forms.Label label5;
		private UI.Button butChangeReferralInto;
		private System.Windows.Forms.TextBox textReferralNameInto;
		private System.Windows.Forms.Label label2;
		private UI.Button butMerge;
		private UI.Button butClose;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkIsDoctorInto;
		private System.Windows.Forms.CheckBox checkIsPersonInto;
		private System.Windows.Forms.CheckBox checkIsDoctorFrom;
		private System.Windows.Forms.CheckBox checkIsPersonFrom;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textReferralNameFrom;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textTitleFrom;

	}
}