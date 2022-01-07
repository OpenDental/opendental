namespace OpenDental{
	partial class FormCanadaOutstandingTransactions {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCanadaOutstandingTransactions));
			this.listCarriers = new OpenDental.UI.ListBoxOD();
			this.groupCarrier = new System.Windows.Forms.GroupBox();
			this.radioVersion2 = new System.Windows.Forms.RadioButton();
			this.radioVersion4Itrans = new System.Windows.Forms.RadioButton();
			this.radioVersion4ToCarrier = new System.Windows.Forms.RadioButton();
			this.groupOfficeNumber = new System.Windows.Forms.GroupBox();
			this.listOfficeNumbers = new OpenDental.UI.ListBoxOD();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupCarrier.SuspendLayout();
			this.groupOfficeNumber.SuspendLayout();
			this.SuspendLayout();
			// 
			// listCarriers
			// 
			this.listCarriers.Location = new System.Drawing.Point(6, 19);
			this.listCarriers.Name = "listCarriers";
			this.listCarriers.Size = new System.Drawing.Size(307, 95);
			this.listCarriers.TabIndex = 107;
			// 
			// groupCarrier
			// 
			this.groupCarrier.Controls.Add(this.listCarriers);
			this.groupCarrier.Enabled = false;
			this.groupCarrier.Location = new System.Drawing.Point(29, 117);
			this.groupCarrier.Name = "groupCarrier";
			this.groupCarrier.Size = new System.Drawing.Size(319, 120);
			this.groupCarrier.TabIndex = 109;
			this.groupCarrier.TabStop = false;
			this.groupCarrier.Text = "Carrier";
			// 
			// radioVersion2
			// 
			this.radioVersion2.AutoSize = true;
			this.radioVersion2.Location = new System.Drawing.Point(12, 48);
			this.radioVersion2.Name = "radioVersion2";
			this.radioVersion2.Size = new System.Drawing.Size(69, 17);
			this.radioVersion2.TabIndex = 110;
			this.radioVersion2.Text = "Version 2";
			this.radioVersion2.UseVisualStyleBackColor = true;
			this.radioVersion2.Click += new System.EventHandler(this.radioVersion2_Click);
			// 
			// radioVersion4Itrans
			// 
			this.radioVersion4Itrans.AutoSize = true;
			this.radioVersion4Itrans.Checked = true;
			this.radioVersion4Itrans.Location = new System.Drawing.Point(12, 71);
			this.radioVersion4Itrans.Name = "radioVersion4Itrans";
			this.radioVersion4Itrans.Size = new System.Drawing.Size(69, 17);
			this.radioVersion4Itrans.TabIndex = 111;
			this.radioVersion4Itrans.TabStop = true;
			this.radioVersion4Itrans.Text = "Version 4";
			this.radioVersion4Itrans.UseVisualStyleBackColor = true;
			this.radioVersion4Itrans.Click += new System.EventHandler(this.radioVersion4Itrans_Click);
			// 
			// radioVersion4ToCarrier
			// 
			this.radioVersion4ToCarrier.Location = new System.Drawing.Point(12, 94);
			this.radioVersion4ToCarrier.Name = "radioVersion4ToCarrier";
			this.radioVersion4ToCarrier.Size = new System.Drawing.Size(270, 17);
			this.radioVersion4ToCarrier.TabIndex = 112;
			this.radioVersion4ToCarrier.Text = "Version 4 To Specific Carrier (not commonly used)";
			this.radioVersion4ToCarrier.UseVisualStyleBackColor = true;
			this.radioVersion4ToCarrier.Click += new System.EventHandler(this.radioVersion4ToCarrier_Click);
			// 
			// groupOfficeNumber
			// 
			this.groupOfficeNumber.Controls.Add(this.listOfficeNumbers);
			this.groupOfficeNumber.Location = new System.Drawing.Point(29, 237);
			this.groupOfficeNumber.Name = "groupOfficeNumber";
			this.groupOfficeNumber.Size = new System.Drawing.Size(319, 70);
			this.groupOfficeNumber.TabIndex = 113;
			this.groupOfficeNumber.TabStop = false;
			this.groupOfficeNumber.Text = "Office Number";
			// 
			// listOfficeNumbers
			// 
			this.listOfficeNumbers.Location = new System.Drawing.Point(6, 19);
			this.listOfficeNumbers.Name = "listOfficeNumbers";
			this.listOfficeNumbers.Size = new System.Drawing.Size(307, 43);
			this.listOfficeNumbers.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox1.Location = new System.Drawing.Point(12, 12);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(336, 32);
			this.textBox1.TabIndex = 114;
			this.textBox1.Text = "The outstanding transactions request should be run for both version 2 and version" +
    " 4.";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(192, 312);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(273, 312);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormCanadaOutstandingTransactions
			// 
			this.ClientSize = new System.Drawing.Size(360, 352);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.groupOfficeNumber);
			this.Controls.Add(this.radioVersion4ToCarrier);
			this.Controls.Add(this.radioVersion4Itrans);
			this.Controls.Add(this.radioVersion2);
			this.Controls.Add(this.groupCarrier);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCanadaOutstandingTransactions";
			this.Text = "Outstanding Transactions Request (ROT)";
			this.Load += new System.EventHandler(this.FormCanadaOutstandingTransactions_Load);
			this.groupCarrier.ResumeLayout(false);
			this.groupOfficeNumber.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listCarriers;
		private System.Windows.Forms.GroupBox groupCarrier;
		private System.Windows.Forms.RadioButton radioVersion2;
		private System.Windows.Forms.RadioButton radioVersion4Itrans;
		private System.Windows.Forms.RadioButton radioVersion4ToCarrier;
		private System.Windows.Forms.GroupBox groupOfficeNumber;
		private OpenDental.UI.ListBoxOD listOfficeNumbers;
		private System.Windows.Forms.TextBox textBox1;
	}
}