namespace OpenDental{
	partial class FormCanadaSummaryReconciliation {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCanadaSummaryReconciliation));
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.listCarriers = new OpenDental.UI.ListBoxOD();
			this.listNetworks = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.listTreatingProvider = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.textDateReconciliation = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupCarrierOrNetwork = new System.Windows.Forms.GroupBox();
			this.checkGetForAllCarriers = new System.Windows.Forms.CheckBox();
			this.groupCarrierOrNetwork.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 199);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(151, 17);
			this.label5.TabIndex = 105;
			this.label5.Text = "Network";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(278, 17);
			this.label1.TabIndex = 106;
			this.label1.Text = "Carrier";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listCarriers
			// 
			this.listCarriers.Location = new System.Drawing.Point(9, 36);
			this.listCarriers.Name = "listCarriers";
			this.listCarriers.Size = new System.Drawing.Size(275, 160);
			this.listCarriers.TabIndex = 107;
			this.listCarriers.Click += new System.EventHandler(this.listCarriers_Click);
			// 
			// listNetworks
			// 
			this.listNetworks.Location = new System.Drawing.Point(9, 219);
			this.listNetworks.Name = "listNetworks";
			this.listNetworks.Size = new System.Drawing.Size(275, 134);
			this.listNetworks.TabIndex = 108;
			this.listNetworks.Click += new System.EventHandler(this.listNetwork_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(18, 396);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(278, 17);
			this.label3.TabIndex = 111;
			this.label3.Text = "Treating Provider";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listTreatingProvider
			// 
			this.listTreatingProvider.Location = new System.Drawing.Point(21, 416);
			this.listTreatingProvider.Name = "listTreatingProvider";
			this.listTreatingProvider.Size = new System.Drawing.Size(276, 69);
			this.listTreatingProvider.TabIndex = 112;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(88, 492);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(103, 17);
			this.label4.TabIndex = 113;
			this.label4.Text = "Reconciliation Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textDateReconciliation
			// 
			this.textDateReconciliation.Location = new System.Drawing.Point(197, 491);
			this.textDateReconciliation.Name = "textDateReconciliation";
			this.textDateReconciliation.Size = new System.Drawing.Size(100, 20);
			this.textDateReconciliation.TabIndex = 114;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(141, 520);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(222, 520);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupCarrierOrNetwork
			// 
			this.groupCarrierOrNetwork.Controls.Add(this.label1);
			this.groupCarrierOrNetwork.Controls.Add(this.label5);
			this.groupCarrierOrNetwork.Controls.Add(this.listCarriers);
			this.groupCarrierOrNetwork.Controls.Add(this.listNetworks);
			this.groupCarrierOrNetwork.Enabled = false;
			this.groupCarrierOrNetwork.Location = new System.Drawing.Point(12, 32);
			this.groupCarrierOrNetwork.Name = "groupCarrierOrNetwork";
			this.groupCarrierOrNetwork.Size = new System.Drawing.Size(293, 361);
			this.groupCarrierOrNetwork.TabIndex = 115;
			this.groupCarrierOrNetwork.TabStop = false;
			this.groupCarrierOrNetwork.Text = "Carrier or Network";
			// 
			// checkGetForAllCarriers
			// 
			this.checkGetForAllCarriers.Checked = true;
			this.checkGetForAllCarriers.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkGetForAllCarriers.Location = new System.Drawing.Point(12, 9);
			this.checkGetForAllCarriers.Name = "checkGetForAllCarriers";
			this.checkGetForAllCarriers.Size = new System.Drawing.Size(190, 17);
			this.checkGetForAllCarriers.TabIndex = 116;
			this.checkGetForAllCarriers.Text = "Get Transactions For All Carriers";
			this.checkGetForAllCarriers.UseVisualStyleBackColor = true;
			this.checkGetForAllCarriers.Click += new System.EventHandler(this.checkGetForAllCarriers_Click);
			// 
			// FormCanadaSummaryReconciliation
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(318, 556);
			this.Controls.Add(this.checkGetForAllCarriers);
			this.Controls.Add(this.groupCarrierOrNetwork);
			this.Controls.Add(this.textDateReconciliation);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listTreatingProvider);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCanadaSummaryReconciliation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Summary Reconciliation Request";
			this.Load += new System.EventHandler(this.FormCanadaPaymentReconciliation_Load);
			this.groupCarrierOrNetwork.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listCarriers;
		private OpenDental.UI.ListBoxOD listNetworks;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ListBoxOD listTreatingProvider;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textDateReconciliation;
		private System.Windows.Forms.GroupBox groupCarrierOrNetwork;
		private System.Windows.Forms.CheckBox checkGetForAllCarriers;
	}
}