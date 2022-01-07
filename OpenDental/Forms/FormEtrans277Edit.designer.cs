namespace OpenDental{
	partial class FormEtrans277Edit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEtrans277Edit));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textQuantityAccepted = new System.Windows.Forms.TextBox();
			this.textQuantityRejected = new System.Windows.Forms.TextBox();
			this.textAmountAccepted = new System.Windows.Forms.TextBox();
			this.textAmountRejected = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textReceiptDate = new System.Windows.Forms.TextBox();
			this.textProcessDate = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butRawMessage = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(175,13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106,20);
			this.label1.TabIndex = 128;
			this.label1.Text = "Quantity Accepted";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(175,36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106,20);
			this.label2.TabIndex = 129;
			this.label2.Text = "Quantity Rejected";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(373,13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(110,20);
			this.label3.TabIndex = 130;
			this.label3.Text = "Amount Accepted";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(373,36);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(110,20);
			this.label4.TabIndex = 131;
			this.label4.Text = "Amount Rejected";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textQuantityAccepted
			// 
			this.textQuantityAccepted.Location = new System.Drawing.Point(287,13);
			this.textQuantityAccepted.Name = "textQuantityAccepted";
			this.textQuantityAccepted.ReadOnly = true;
			this.textQuantityAccepted.Size = new System.Drawing.Size(80,20);
			this.textQuantityAccepted.TabIndex = 132;
			// 
			// textQuantityRejected
			// 
			this.textQuantityRejected.Location = new System.Drawing.Point(287,36);
			this.textQuantityRejected.Name = "textQuantityRejected";
			this.textQuantityRejected.ReadOnly = true;
			this.textQuantityRejected.Size = new System.Drawing.Size(80,20);
			this.textQuantityRejected.TabIndex = 133;
			// 
			// textAmountAccepted
			// 
			this.textAmountAccepted.Location = new System.Drawing.Point(489,13);
			this.textAmountAccepted.Name = "textAmountAccepted";
			this.textAmountAccepted.ReadOnly = true;
			this.textAmountAccepted.Size = new System.Drawing.Size(80,20);
			this.textAmountAccepted.TabIndex = 134;
			// 
			// textAmountRejected
			// 
			this.textAmountRejected.Location = new System.Drawing.Point(489,36);
			this.textAmountRejected.Name = "textAmountRejected";
			this.textAmountRejected.ReadOnly = true;
			this.textAmountRejected.Size = new System.Drawing.Size(80,20);
			this.textAmountRejected.TabIndex = 135;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6,13);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(76,20);
			this.label5.TabIndex = 136;
			this.label5.Text = "Receipt Date";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReceiptDate
			// 
			this.textReceiptDate.Location = new System.Drawing.Point(89,13);
			this.textReceiptDate.Name = "textReceiptDate";
			this.textReceiptDate.ReadOnly = true;
			this.textReceiptDate.Size = new System.Drawing.Size(80,20);
			this.textReceiptDate.TabIndex = 137;
			// 
			// textProcessDate
			// 
			this.textProcessDate.Location = new System.Drawing.Point(89,36);
			this.textProcessDate.Name = "textProcessDate";
			this.textProcessDate.ReadOnly = true;
			this.textProcessDate.Size = new System.Drawing.Size(80,20);
			this.textProcessDate.TabIndex = 139;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6,36);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(76,20);
			this.label6.TabIndex = 138;
			this.label6.Text = "Process Date";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(9,58);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(966,613);
			this.gridMain.TabIndex = 114;
			this.gridMain.Title = "Claim Status and Information";
			this.gridMain.TranslationName = "FormEtrans277Edit";
			// 
			// butRawMessage
			// 
			this.butRawMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRawMessage.Location = new System.Drawing.Point(875,32);
			this.butRawMessage.Name = "butRawMessage";
			this.butRawMessage.Size = new System.Drawing.Size(100,24);
			this.butRawMessage.TabIndex = 116;
			this.butRawMessage.Text = "Raw Message";
			this.butRawMessage.Click += new System.EventHandler(this.butRawMessage_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(900,677);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormEtrans277Edit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(984,709);
			this.Controls.Add(this.textProcessDate);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textReceiptDate);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textAmountRejected);
			this.Controls.Add(this.textAmountAccepted);
			this.Controls.Add(this.textQuantityRejected);
			this.Controls.Add(this.textQuantityAccepted);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butRawMessage);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEtrans277Edit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Claim Status Response From ";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormEtrans277Edit_Load);
			this.Resize += new System.EventHandler(this.FormEtrans277Edit_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.GridOD gridMain;
		private UI.Button butRawMessage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textQuantityAccepted;
		private System.Windows.Forms.TextBox textQuantityRejected;
		private System.Windows.Forms.TextBox textAmountAccepted;
		private System.Windows.Forms.TextBox textAmountRejected;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textReceiptDate;
		private System.Windows.Forms.TextBox textProcessDate;
		private System.Windows.Forms.Label label6;
	}
}