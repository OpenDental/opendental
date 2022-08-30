namespace OpenDental{
	partial class FormRepeatChargeEditMulti {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRepeatChargeEditMulti));
			this.butClose = new OpenDental.UI.Button();
			this.textProcCode = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textDateStart = new OpenDental.ValidDate();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.butRun = new OpenDental.UI.Button();
			this.textChargeAmount = new OpenDental.ValidDouble();
			this.textChargeAmountNew = new OpenDental.ValidDouble();
			this.comboIsEnabled = new OpenDental.UI.ComboBoxOD();
			this.label12 = new System.Windows.Forms.Label();
			this.textPatNumSuperFamilyHead = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(592, 269);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// textProcCode
			// 
			this.textProcCode.Location = new System.Drawing.Point(184, 23);
			this.textProcCode.MaxLength = 15;
			this.textProcCode.Name = "textProcCode";
			this.textProcCode.Size = new System.Drawing.Size(100, 20);
			this.textProcCode.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(2, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(174, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Procedure Code";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(2, 65);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(174, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Current Charge Amount";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(2, 151);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(174, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Super Family Head";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(299, 150);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(333, 31);
			this.label4.TabIndex = 10;
			this.label4.Text = "Enter patnum of super family head to only update that super family. Leave blank t" +
    "o update entire database.";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(299, 26);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(333, 18);
			this.label5.TabIndex = 11;
			this.label5.Text = "Update repeating charges that have this procedure code.";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(299, 67);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(333, 20);
			this.label6.TabIndex = 12;
			this.label6.Text = "Update repeating charges that currently have this charge amount.";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(299, 190);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(333, 33);
			this.label7.TabIndex = 14;
			this.label7.Text = "Update repeating charges that are currently enabled or disabled. Select both to u" +
    "pdate enabled and disabled.";
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(184, 226);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 5;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(2, 227);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(174, 16);
			this.label8.TabIndex = 16;
			this.label8.Text = "Start Date";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(299, 226);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(333, 33);
			this.label9.TabIndex = 17;
			this.label9.Text = "Update repeating charges that have a start date on or after this date. Leave blan" +
    "k to update regardless of start date.";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(299, 109);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(333, 20);
			this.label10.TabIndex = 20;
			this.label10.Text = "Update repeating charges to this charge amount.";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(2, 107);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(174, 16);
			this.label11.TabIndex = 19;
			this.label11.Text = "New Charge Amount";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRun
			// 
			this.butRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRun.Location = new System.Drawing.Point(511, 269);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(75, 24);
			this.butRun.TabIndex = 6;
			this.butRun.Text = "&Run";
			this.butRun.UseVisualStyleBackColor = false;
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// textChargeAmount
			// 
			this.textChargeAmount.Location = new System.Drawing.Point(184, 64);
			this.textChargeAmount.MaxVal = 100000000D;
			this.textChargeAmount.MinVal = -100000000D;
			this.textChargeAmount.Name = "textChargeAmount";
			this.textChargeAmount.Size = new System.Drawing.Size(100, 20);
			this.textChargeAmount.TabIndex = 1;
			// 
			// textChargeAmountNew
			// 
			this.textChargeAmountNew.Location = new System.Drawing.Point(184, 106);
			this.textChargeAmountNew.MaxVal = 100000000D;
			this.textChargeAmountNew.MinVal = -100000000D;
			this.textChargeAmountNew.Name = "textChargeAmountNew";
			this.textChargeAmountNew.Size = new System.Drawing.Size(100, 20);
			this.textChargeAmountNew.TabIndex = 2;
			// 
			// comboIsEnabled
			// 
			this.comboIsEnabled.Location = new System.Drawing.Point(184, 190);
			this.comboIsEnabled.Name = "comboIsEnabled";
			this.comboIsEnabled.Size = new System.Drawing.Size(100, 21);
			this.comboIsEnabled.TabIndex = 4;
			this.comboIsEnabled.Text = "comboBoxOD1";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(2, 190);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(174, 16);
			this.label12.TabIndex = 22;
			this.label12.Text = "Repeat Charge Status";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatNumSuperFamilyHead
			// 
			this.textPatNumSuperFamilyHead.Location = new System.Drawing.Point(184, 150);
			this.textPatNumSuperFamilyHead.Name = "textPatNumSuperFamilyHead";
			this.textPatNumSuperFamilyHead.Size = new System.Drawing.Size(100, 20);
			this.textPatNumSuperFamilyHead.TabIndex = 3;
			// 
			// FormRepeatChargeEditMulti
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(679, 305);
			this.Controls.Add(this.textPatNumSuperFamilyHead);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.comboIsEnabled);
			this.Controls.Add(this.textChargeAmountNew);
			this.Controls.Add(this.textChargeAmount);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textProcCode);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRepeatChargeEditMulti";
			this.Text = "Multi Repeat Charge Edit";
			this.Load += new System.EventHandler(this.FormRepeatChargeEditMulti_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.TextBox textProcCode;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private UI.Button butRun;
		private ValidDouble textChargeAmount;
		private ValidDouble textChargeAmountNew;
		private UI.ComboBoxOD comboIsEnabled;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textPatNumSuperFamilyHead;
	}
}