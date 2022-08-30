namespace OpenDental{
	partial class FormCreditRecurringDateChoose {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCreditRecurringDateChoose));
			this.butCancel = new OpenDental.UI.Button();
			this.butLastMonth = new OpenDental.UI.Button();
			this.butThisMonth = new OpenDental.UI.Button();
			this.labelMessage = new System.Windows.Forms.Label();
			this.labelLastMonth = new System.Windows.Forms.Label();
			this.labelThisMonth = new System.Windows.Forms.Label();
			this.labelMonthSelect = new System.Windows.Forms.Label();
			this.comboBoxMonthSelect = new OpenDental.UI.ComboBoxOD();
			this.butOK = new OpenDental.UI.Button();
			this.labelNoDates = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(301, 159);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butLastMonth
			// 
			this.butLastMonth.Location = new System.Drawing.Point(17, 62);
			this.butLastMonth.Name = "butLastMonth";
			this.butLastMonth.Size = new System.Drawing.Size(96, 24);
			this.butLastMonth.TabIndex = 4;
			this.butLastMonth.Text = "Last Month";
			this.butLastMonth.Click += new System.EventHandler(this.butLastMonth_Click);
			// 
			// butThisMonth
			// 
			this.butThisMonth.Location = new System.Drawing.Point(17, 103);
			this.butThisMonth.Name = "butThisMonth";
			this.butThisMonth.Size = new System.Drawing.Size(96, 24);
			this.butThisMonth.TabIndex = 5;
			this.butThisMonth.Text = "This Month";
			this.butThisMonth.Click += new System.EventHandler(this.butThisMonth_Click);
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(14, 27);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(362, 23);
			this.labelMessage.TabIndex = 8;
			this.labelMessage.Text = "Which month should this payment be applied to?";
			// 
			// labelLastMonth
			// 
			this.labelLastMonth.Location = new System.Drawing.Point(119, 62);
			this.labelLastMonth.Name = "labelLastMonth";
			this.labelLastMonth.Size = new System.Drawing.Size(267, 24);
			this.labelLastMonth.TabIndex = 9;
			this.labelLastMonth.Text = "Payment date will be:";
			this.labelLastMonth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelThisMonth
			// 
			this.labelThisMonth.Location = new System.Drawing.Point(119, 103);
			this.labelThisMonth.Name = "labelThisMonth";
			this.labelThisMonth.Size = new System.Drawing.Size(267, 24);
			this.labelThisMonth.TabIndex = 10;
			this.labelThisMonth.Text = "Payment date will be:";
			this.labelThisMonth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelMonthSelect
			// 
			this.labelMonthSelect.Location = new System.Drawing.Point(138, 56);
			this.labelMonthSelect.Name = "labelMonthSelect";
			this.labelMonthSelect.Size = new System.Drawing.Size(238, 23);
			this.labelMonthSelect.TabIndex = 16;
			this.labelMonthSelect.Text = "Previous Charge Dates";
			this.labelMonthSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelMonthSelect.Visible = false;
			// 
			// comboBoxMonthSelect
			// 
			this.comboBoxMonthSelect.Location = new System.Drawing.Point(17, 56);
			this.comboBoxMonthSelect.Name = "comboBoxMonthSelect";
			this.comboBoxMonthSelect.Size = new System.Drawing.Size(121, 21);
			this.comboBoxMonthSelect.TabIndex = 15;
			this.comboBoxMonthSelect.Visible = false;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(220, 159);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 17;
			this.butOK.Text = "&OK";
			this.butOK.Visible = false;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelNoDates
			// 
			this.labelNoDates.Location = new System.Drawing.Point(45, 80);
			this.labelNoDates.Name = "labelNoDates";
			this.labelNoDates.Size = new System.Drawing.Size(318, 13);
			this.labelNoDates.TabIndex = 18;
			this.labelNoDates.Text = "No available dates after credit card start date.";
			this.labelNoDates.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelNoDates.Visible = false;
			// 
			// FormCreditRecurringDateChoose
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(388, 195);
			this.Controls.Add(this.labelNoDates);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelMonthSelect);
			this.Controls.Add(this.comboBoxMonthSelect);
			this.Controls.Add(this.labelThisMonth);
			this.Controls.Add(this.labelLastMonth);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.butThisMonth);
			this.Controls.Add(this.butLastMonth);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCreditRecurringDateChoose";
			this.Text = "Recurring Charge Month";
			this.Load += new System.EventHandler(this.FormCreditRecurringDateChoose_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private UI.Button butLastMonth;
		private UI.Button butThisMonth;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.Label labelLastMonth;
		private System.Windows.Forms.Label labelThisMonth;
		private System.Windows.Forms.Label labelMonthSelect;
		private UI.ComboBoxOD comboBoxMonthSelect;
		private UI.Button butOK;
		private System.Windows.Forms.Label labelNoDates;
	}
}