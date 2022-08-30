namespace OpenDental{
	partial class FormPaymentPlanOptions {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPaymentPlanOptions));
			this.butClose = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.radioMonthly = new System.Windows.Forms.RadioButton();
			this.radioQuarterly = new System.Windows.Forms.RadioButton();
			this.radioWeekly = new System.Windows.Forms.RadioButton();
			this.radioEveryOtherWeek = new System.Windows.Forms.RadioButton();
			this.radioOrdinalWeekday = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(400, 274);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(39, 10);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(426, 62);
			this.label8.TabIndex = 20;
			this.label8.Text = resources.GetString("label8.Text");
			// 
			// radioMonthly
			// 
			this.radioMonthly.Checked = true;
			this.radioMonthly.Location = new System.Drawing.Point(42, 140);
			this.radioMonthly.Name = "radioMonthly";
			this.radioMonthly.Size = new System.Drawing.Size(104, 18);
			this.radioMonthly.TabIndex = 21;
			this.radioMonthly.TabStop = true;
			this.radioMonthly.Text = "Monthly";
			this.radioMonthly.UseVisualStyleBackColor = true;
			// 
			// radioQuarterly
			// 
			this.radioQuarterly.Location = new System.Drawing.Point(42, 160);
			this.radioQuarterly.Name = "radioQuarterly";
			this.radioQuarterly.Size = new System.Drawing.Size(104, 18);
			this.radioQuarterly.TabIndex = 22;
			this.radioQuarterly.TabStop = true;
			this.radioQuarterly.Text = "Quarterly";
			this.radioQuarterly.UseVisualStyleBackColor = true;
			// 
			// radioWeekly
			// 
			this.radioWeekly.Location = new System.Drawing.Point(42, 78);
			this.radioWeekly.Name = "radioWeekly";
			this.radioWeekly.Size = new System.Drawing.Size(104, 18);
			this.radioWeekly.TabIndex = 23;
			this.radioWeekly.TabStop = true;
			this.radioWeekly.Text = "Weekly";
			this.radioWeekly.UseVisualStyleBackColor = true;
			// 
			// radioEveryOtherWeek
			// 
			this.radioEveryOtherWeek.Location = new System.Drawing.Point(42, 99);
			this.radioEveryOtherWeek.Name = "radioEveryOtherWeek";
			this.radioEveryOtherWeek.Size = new System.Drawing.Size(156, 18);
			this.radioEveryOtherWeek.TabIndex = 24;
			this.radioEveryOtherWeek.TabStop = true;
			this.radioEveryOtherWeek.Text = "Every other week";
			this.radioEveryOtherWeek.UseVisualStyleBackColor = true;
			// 
			// radioOrdinalWeekday
			// 
			this.radioOrdinalWeekday.Location = new System.Drawing.Point(42, 119);
			this.radioOrdinalWeekday.Name = "radioOrdinalWeekday";
			this.radioOrdinalWeekday.Size = new System.Drawing.Size(264, 18);
			this.radioOrdinalWeekday.TabIndex = 25;
			this.radioOrdinalWeekday.TabStop = true;
			this.radioOrdinalWeekday.Text = "First/second/etc Mon/Tue/etc of month";
			this.radioOrdinalWeekday.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(39, 192);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(426, 66);
			this.label1.TabIndex = 26;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// FormPaymentPlanOptions
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(490, 312);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.radioOrdinalWeekday);
			this.Controls.Add(this.radioEveryOtherWeek);
			this.Controls.Add(this.radioWeekly);
			this.Controls.Add(this.radioQuarterly);
			this.Controls.Add(this.radioMonthly);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPaymentPlanOptions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Payment Plan Options";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label label8;
		public System.Windows.Forms.RadioButton radioMonthly;
		public System.Windows.Forms.RadioButton radioQuarterly;
		public System.Windows.Forms.RadioButton radioWeekly;
		public System.Windows.Forms.RadioButton radioEveryOtherWeek;
		public System.Windows.Forms.RadioButton radioOrdinalWeekday;
		private System.Windows.Forms.Label label1;
	}
}