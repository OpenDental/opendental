namespace CentralManager {
	partial class FormCentralProdInc {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralProdInc));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.radioWriteoffProc = new System.Windows.Forms.RadioButton();
			this.radioWriteoffPay = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butRight = new OpenDental.UI.Button();
			this.butThis = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textDateFrom = new OpenDental.ValidDate();
			this.textDateTo = new OpenDental.ValidDate();
			this.label3 = new System.Windows.Forms.Label();
			this.butLeft = new OpenDental.UI.Button();
			this.textToday = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioProvider = new System.Windows.Forms.RadioButton();
			this.radioAnnual = new System.Windows.Forms.RadioButton();
			this.radioDaily = new System.Windows.Forms.RadioButton();
			this.radioMonthly = new System.Windows.Forms.RadioButton();
			this.checkShowUnearned = new System.Windows.Forms.CheckBox();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(313, 305);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(394, 305);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.radioWriteoffProc);
			this.groupBox3.Controls.Add(this.radioWriteoffPay);
			this.groupBox3.Location = new System.Drawing.Point(164, 188);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(281, 79);
			this.groupBox3.TabIndex = 51;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Show Insurance Writeoffs";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 59);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(269, 17);
			this.label5.TabIndex = 2;
			this.label5.Text = "(this is discussed in the PPO section of the manual)";
			// 
			// radioWriteoffProc
			// 
			this.radioWriteoffProc.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProc.Checked = true;
			this.radioWriteoffProc.Location = new System.Drawing.Point(9, 36);
			this.radioWriteoffProc.Name = "radioWriteoffProc";
			this.radioWriteoffProc.Size = new System.Drawing.Size(244, 17);
			this.radioWriteoffProc.TabIndex = 1;
			this.radioWriteoffProc.TabStop = true;
			this.radioWriteoffProc.Text = "Using procedure date.";
			this.radioWriteoffProc.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProc.UseVisualStyleBackColor = true;
			// 
			// radioWriteoffPay
			// 
			this.radioWriteoffPay.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffPay.Location = new System.Drawing.Point(9, 17);
			this.radioWriteoffPay.Name = "radioWriteoffPay";
			this.radioWriteoffPay.Size = new System.Drawing.Size(244, 17);
			this.radioWriteoffPay.TabIndex = 0;
			this.radioWriteoffPay.Text = "Using insurance payment date.";
			this.radioWriteoffPay.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffPay.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.butRight);
			this.groupBox2.Controls.Add(this.butThis);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.textDateFrom);
			this.groupBox2.Controls.Add(this.textDateTo);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.butLeft);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(85, 38);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(281, 144);
			this.groupBox2.TabIndex = 50;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Date Range";
			// 
			// butRight
			// 
			this.butRight.Image = ((System.Drawing.Image)(resources.GetObject("butRight.Image")));
			this.butRight.Location = new System.Drawing.Point(205, 30);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(45, 24);
			this.butRight.TabIndex = 46;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butThis
			// 
			this.butThis.Location = new System.Drawing.Point(95, 30);
			this.butThis.Name = "butThis";
			this.butThis.Size = new System.Drawing.Size(101, 24);
			this.butThis.TabIndex = 45;
			this.butThis.Text = "This";
			this.butThis.Click += new System.EventHandler(this.butThis_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 18);
			this.label2.TabIndex = 37;
			this.label2.Text = "From";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(95, 77);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(100, 20);
			this.textDateFrom.TabIndex = 43;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(95, 104);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(100, 20);
			this.textDateTo.TabIndex = 44;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 105);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 18);
			this.label3.TabIndex = 39;
			this.label3.Text = "To";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butLeft
			// 
			this.butLeft.Image = ((System.Drawing.Image)(resources.GetObject("butLeft.Image")));
			this.butLeft.Location = new System.Drawing.Point(41, 30);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(45, 24);
			this.butLeft.TabIndex = 44;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// textToday
			// 
			this.textToday.Location = new System.Drawing.Point(180, 12);
			this.textToday.Name = "textToday";
			this.textToday.ReadOnly = true;
			this.textToday.Size = new System.Drawing.Size(100, 20);
			this.textToday.TabIndex = 49;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(51, 14);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(127, 20);
			this.label4.TabIndex = 48;
			this.label4.Text = "Today\'s Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioProvider);
			this.groupBox1.Controls.Add(this.radioAnnual);
			this.groupBox1.Controls.Add(this.radioDaily);
			this.groupBox1.Controls.Add(this.radioMonthly);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(35, 188);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(123, 104);
			this.groupBox1.TabIndex = 47;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Report Type";
			// 
			// radioProvider
			// 
			this.radioProvider.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioProvider.Location = new System.Drawing.Point(14, 73);
			this.radioProvider.Name = "radioProvider";
			this.radioProvider.Size = new System.Drawing.Size(104, 17);
			this.radioProvider.TabIndex = 36;
			this.radioProvider.Text = "Provider";
			this.radioProvider.Click += new System.EventHandler(this.radioProvider_Click);
			// 
			// radioAnnual
			// 
			this.radioAnnual.Checked = true;
			this.radioAnnual.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAnnual.Location = new System.Drawing.Point(14, 55);
			this.radioAnnual.Name = "radioAnnual";
			this.radioAnnual.Size = new System.Drawing.Size(104, 17);
			this.radioAnnual.TabIndex = 35;
			this.radioAnnual.TabStop = true;
			this.radioAnnual.Text = "Annual";
			this.radioAnnual.Click += new System.EventHandler(this.radioAnnual_Click);
			// 
			// radioDaily
			// 
			this.radioDaily.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioDaily.Location = new System.Drawing.Point(14, 17);
			this.radioDaily.Name = "radioDaily";
			this.radioDaily.Size = new System.Drawing.Size(104, 17);
			this.radioDaily.TabIndex = 34;
			this.radioDaily.Text = "Daily";
			this.radioDaily.Click += new System.EventHandler(this.radioDaily_Click);
			// 
			// radioMonthly
			// 
			this.radioMonthly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioMonthly.Location = new System.Drawing.Point(14, 36);
			this.radioMonthly.Name = "radioMonthly";
			this.radioMonthly.Size = new System.Drawing.Size(104, 17);
			this.radioMonthly.TabIndex = 33;
			this.radioMonthly.Text = "Monthly";
			this.radioMonthly.Click += new System.EventHandler(this.radioMonthly_Click);
			// 
			// checkShowUnearned
			// 
			this.checkShowUnearned.Checked = true;
			this.checkShowUnearned.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowUnearned.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowUnearned.Location = new System.Drawing.Point(173, 273);
			this.checkShowUnearned.Name = "checkShowUnearned";
			this.checkShowUnearned.Size = new System.Drawing.Size(160, 16);
			this.checkShowUnearned.TabIndex = 53;
			this.checkShowUnearned.Text = "Include Unearned";
			// 
			// FormCentralProdInc
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(481, 341);
			this.Controls.Add(this.checkShowUnearned);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.textToday);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "FormCentralProdInc";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Production and Income Report";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCentralProdInc_FormClosing);
			this.Load += new System.EventHandler(this.FormCentralProdInc_Load);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.RadioButton radioWriteoffProc;
		private System.Windows.Forms.RadioButton radioWriteoffPay;
		private System.Windows.Forms.GroupBox groupBox2;
		private OpenDental.UI.Button butRight;
		private OpenDental.UI.Button butThis;
		private System.Windows.Forms.Label label2;
		private OpenDental.ValidDate textDateFrom;
		private OpenDental.ValidDate textDateTo;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butLeft;
		private System.Windows.Forms.TextBox textToday;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioAnnual;
		private System.Windows.Forms.RadioButton radioDaily;
		private System.Windows.Forms.RadioButton radioMonthly;
		private System.Windows.Forms.RadioButton radioProvider;
		private System.Windows.Forms.CheckBox checkShowUnearned;
	}
}