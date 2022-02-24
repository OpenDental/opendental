namespace OpenDental{
	partial class FormEdgeExpressTrans {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEdgeExpressTrans));
			this.butOK = new OpenDental.UI.Button();
			this.labelCashBackAmt = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textCashBackAmt = new OpenDental.ValidDouble();
			this.listTransType = new OpenDental.UI.ListBoxOD();
			this.checkSaveToken = new System.Windows.Forms.CheckBox();
			this.checkSignature = new System.Windows.Forms.CheckBox();
			this.checkPrintReceipt = new System.Windows.Forms.CheckBox();
			this.textTransactionId = new System.Windows.Forms.TextBox();
			this.labelTransactionID = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(219, 283);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelCashBackAmt
			// 
			this.labelCashBackAmt.Location = new System.Drawing.Point(50, 195);
			this.labelCashBackAmt.Name = "labelCashBackAmt";
			this.labelCashBackAmt.Size = new System.Drawing.Size(154, 16);
			this.labelCashBackAmt.TabIndex = 60;
			this.labelCashBackAmt.Text = "Cash Back Amount";
			this.labelCashBackAmt.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelCashBackAmt.Visible = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(50, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(154, 16);
			this.label1.TabIndex = 58;
			this.label1.Text = "Transaction Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textCashBackAmt
			// 
			this.textCashBackAmt.Location = new System.Drawing.Point(52, 214);
			this.textCashBackAmt.MaxVal = 100000000D;
			this.textCashBackAmt.MinVal = -100000000D;
			this.textCashBackAmt.Name = "textCashBackAmt";
			this.textCashBackAmt.Size = new System.Drawing.Size(100, 20);
			this.textCashBackAmt.TabIndex = 61;
			this.textCashBackAmt.Visible = false;
			// 
			// listTransType
			// 
			this.listTransType.Location = new System.Drawing.Point(53, 47);
			this.listTransType.Name = "listTransType";
			this.listTransType.Size = new System.Drawing.Size(202, 108);
			this.listTransType.TabIndex = 62;
			this.listTransType.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listTransType_MouseClick);
			// 
			// checkSaveToken
			// 
			this.checkSaveToken.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSaveToken.Location = new System.Drawing.Point(52, 240);
			this.checkSaveToken.Name = "checkSaveToken";
			this.checkSaveToken.Size = new System.Drawing.Size(161, 17);
			this.checkSaveToken.TabIndex = 63;
			this.checkSaveToken.Text = "Save Token";
			// 
			// checkSignature
			// 
			this.checkSignature.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSignature.Location = new System.Drawing.Point(52, 257);
			this.checkSignature.Name = "checkSignature";
			this.checkSignature.Size = new System.Drawing.Size(161, 17);
			this.checkSignature.TabIndex = 64;
			this.checkSignature.Text = "Prompt for Signature";
			// 
			// checkPrintReceipt
			// 
			this.checkPrintReceipt.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPrintReceipt.Location = new System.Drawing.Point(52, 274);
			this.checkPrintReceipt.Name = "checkPrintReceipt";
			this.checkPrintReceipt.Size = new System.Drawing.Size(161, 17);
			this.checkPrintReceipt.TabIndex = 65;
			this.checkPrintReceipt.Text = "Print Receipt";
			// 
			// textTransactionId
			// 
			this.textTransactionId.Location = new System.Drawing.Point(52, 178);
			this.textTransactionId.Name = "textTransactionId";
			this.textTransactionId.Size = new System.Drawing.Size(100, 20);
			this.textTransactionId.TabIndex = 66;
			this.textTransactionId.Visible = false;
			// 
			// labelTransactionID
			// 
			this.labelTransactionID.Location = new System.Drawing.Point(49, 159);
			this.labelTransactionID.Name = "labelTransactionID";
			this.labelTransactionID.Size = new System.Drawing.Size(154, 16);
			this.labelTransactionID.TabIndex = 67;
			this.labelTransactionID.Text = "Transaction ID";
			this.labelTransactionID.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelTransactionID.Visible = false;
			// 
			// FormEdgeExpressTrans
			// 
			this.ClientSize = new System.Drawing.Size(321, 323);
			this.Controls.Add(this.labelTransactionID);
			this.Controls.Add(this.textTransactionId);
			this.Controls.Add(this.checkPrintReceipt);
			this.Controls.Add(this.checkSignature);
			this.Controls.Add(this.checkSaveToken);
			this.Controls.Add(this.listTransType);
			this.Controls.Add(this.textCashBackAmt);
			this.Controls.Add(this.labelCashBackAmt);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEdgeExpressTrans";
			this.Text = "EdgeExpress Transaction Types";
			this.Load += new System.EventHandler(this.FormEdgeExpressTrans_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelCashBackAmt;
		private System.Windows.Forms.Label label1;
		private OpenDental.ValidDouble textCashBackAmt;
		private OpenDental.UI.ListBoxOD listTransType;
		private System.Windows.Forms.CheckBox checkSaveToken;
		private System.Windows.Forms.CheckBox checkSignature;
		private System.Windows.Forms.CheckBox checkPrintReceipt;
		private System.Windows.Forms.TextBox textTransactionId;
		private System.Windows.Forms.Label labelTransactionID;
	}
}