namespace OpenDental{
	partial class FormCanadaPaymentReconciliation {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCanadaPaymentReconciliation));
			this.label1 = new System.Windows.Forms.Label();
			this.listCarriers = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.listBillingProvider = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.listTreatingProvider = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.textDateReconciliation = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textBillingOfficeNumber = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textTreatingOfficeNumber = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(278, 17);
			this.label1.TabIndex = 106;
			this.label1.Text = "Carrier";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listCarriers
			// 
			this.listCarriers.Location = new System.Drawing.Point(15, 26);
			this.listCarriers.Name = "listCarriers";
			this.listCarriers.Size = new System.Drawing.Size(275, 43);
			this.listCarriers.TabIndex = 107;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(278, 17);
			this.label2.TabIndex = 109;
			this.label2.Text = "Billing Provider";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBillingProvider
			// 
			this.listBillingProvider.Location = new System.Drawing.Point(15, 92);
			this.listBillingProvider.Name = "listBillingProvider";
			this.listBillingProvider.Size = new System.Drawing.Size(275, 43);
			this.listBillingProvider.TabIndex = 110;
			this.listBillingProvider.Click += new System.EventHandler(this.listBillingProvider_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 182);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(278, 17);
			this.label3.TabIndex = 111;
			this.label3.Text = "Treating Provider";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listTreatingProvider
			// 
			this.listTreatingProvider.Location = new System.Drawing.Point(15, 202);
			this.listTreatingProvider.Name = "listTreatingProvider";
			this.listTreatingProvider.Size = new System.Drawing.Size(276, 43);
			this.listTreatingProvider.TabIndex = 112;
			this.listTreatingProvider.Click += new System.EventHandler(this.listTreatingProvider_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(187, 248);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(103, 17);
			this.label4.TabIndex = 113;
			this.label4.Text = "Reconciliation Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textDateReconciliation
			// 
			this.textDateReconciliation.Location = new System.Drawing.Point(190, 268);
			this.textDateReconciliation.Name = "textDateReconciliation";
			this.textDateReconciliation.Size = new System.Drawing.Size(100, 20);
			this.textDateReconciliation.TabIndex = 114;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(134, 300);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(215, 300);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textBillingOfficeNumber
			// 
			this.textBillingOfficeNumber.Location = new System.Drawing.Point(15, 158);
			this.textBillingOfficeNumber.Name = "textBillingOfficeNumber";
			this.textBillingOfficeNumber.ReadOnly = true;
			this.textBillingOfficeNumber.Size = new System.Drawing.Size(102, 20);
			this.textBillingOfficeNumber.TabIndex = 114;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 138);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(278, 17);
			this.label5.TabIndex = 115;
			this.label5.Text = "Billing Office Number";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 248);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(151, 17);
			this.label6.TabIndex = 117;
			this.label6.Text = "Treating Office Number";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textTreatingOfficeNumber
			// 
			this.textTreatingOfficeNumber.Location = new System.Drawing.Point(15, 268);
			this.textTreatingOfficeNumber.Name = "textTreatingOfficeNumber";
			this.textTreatingOfficeNumber.ReadOnly = true;
			this.textTreatingOfficeNumber.Size = new System.Drawing.Size(102, 20);
			this.textTreatingOfficeNumber.TabIndex = 116;
			// 
			// FormCanadaPaymentReconciliation
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(306, 336);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textTreatingOfficeNumber);
			this.Controls.Add(this.listCarriers);
			this.Controls.Add(this.textDateReconciliation);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textBillingOfficeNumber);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.listTreatingProvider);
			this.Controls.Add(this.listBillingProvider);
			this.Controls.Add(this.label3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCanadaPaymentReconciliation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Payment Reconciliation Request";
			this.Load += new System.EventHandler(this.FormCanadaPaymentReconciliation_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listCarriers;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listBillingProvider;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ListBoxOD listTreatingProvider;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textDateReconciliation;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBillingOfficeNumber;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textTreatingOfficeNumber;
	}
}