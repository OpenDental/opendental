namespace OpenDental{
	partial class FormXWeb {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormXWeb));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelRefNumber = new System.Windows.Forms.Label();
			this.textRefNumber = new System.Windows.Forms.TextBox();
			this.groupTransType = new System.Windows.Forms.GroupBox();
			this.radioReturn = new System.Windows.Forms.RadioButton();
			this.textZipCode = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textSecurityCode = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textNameOnCard = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textExpDate = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textCardNumber = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textAmount = new System.Windows.Forms.TextBox();
			this.labelAmount = new System.Windows.Forms.Label();
			this.textPayNote = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupTransType.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(240, 301);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(321, 301);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelRefNumber
			// 
			this.labelRefNumber.Location = new System.Drawing.Point(263, 25);
			this.labelRefNumber.Name = "labelRefNumber";
			this.labelRefNumber.Size = new System.Drawing.Size(117, 16);
			this.labelRefNumber.TabIndex = 22;
			this.labelRefNumber.Text = "Ref Number";
			this.labelRefNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelRefNumber.Visible = false;
			// 
			// textRefNumber
			// 
			this.textRefNumber.Location = new System.Drawing.Point(263, 42);
			this.textRefNumber.Name = "textRefNumber";
			this.textRefNumber.Size = new System.Drawing.Size(118, 20);
			this.textRefNumber.TabIndex = 35;
			this.textRefNumber.Visible = false;
			// 
			// groupTransType
			// 
			this.groupTransType.Controls.Add(this.radioReturn);
			this.groupTransType.Location = new System.Drawing.Point(12, 12);
			this.groupTransType.Name = "groupTransType";
			this.groupTransType.Size = new System.Drawing.Size(130, 50);
			this.groupTransType.TabIndex = 23;
			this.groupTransType.TabStop = false;
			this.groupTransType.Text = "Transaction Type";
			// 
			// radioReturn
			// 
			this.radioReturn.AutoSize = true;
			this.radioReturn.Location = new System.Drawing.Point(17, 19);
			this.radioReturn.Name = "radioReturn";
			this.radioReturn.Size = new System.Drawing.Size(57, 17);
			this.radioReturn.TabIndex = 0;
			this.radioReturn.Text = "Return";
			this.radioReturn.UseVisualStyleBackColor = true;
			// 
			// textZipCode
			// 
			this.textZipCode.Location = new System.Drawing.Point(263, 128);
			this.textZipCode.Name = "textZipCode";
			this.textZipCode.Size = new System.Drawing.Size(118, 20);
			this.textZipCode.TabIndex = 33;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(263, 111);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(117, 16);
			this.label7.TabIndex = 25;
			this.label7.Text = "Zip Code";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSecurityCode
			// 
			this.textSecurityCode.Location = new System.Drawing.Point(263, 85);
			this.textSecurityCode.Name = "textSecurityCode";
			this.textSecurityCode.Size = new System.Drawing.Size(118, 20);
			this.textSecurityCode.TabIndex = 32;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(263, 68);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(117, 16);
			this.label6.TabIndex = 26;
			this.label6.Text = "Security Code";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textNameOnCard
			// 
			this.textNameOnCard.Location = new System.Drawing.Point(12, 171);
			this.textNameOnCard.Name = "textNameOnCard";
			this.textNameOnCard.Size = new System.Drawing.Size(228, 20);
			this.textNameOnCard.TabIndex = 31;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 154);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(150, 16);
			this.label5.TabIndex = 27;
			this.label5.Text = "Name On Card";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textExpDate
			// 
			this.textExpDate.Location = new System.Drawing.Point(12, 128);
			this.textExpDate.Name = "textExpDate";
			this.textExpDate.Size = new System.Drawing.Size(150, 20);
			this.textExpDate.TabIndex = 30;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 111);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(150, 16);
			this.label4.TabIndex = 24;
			this.label4.Text = "Expiration (MMYY)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textCardNumber
			// 
			this.textCardNumber.Location = new System.Drawing.Point(12, 85);
			this.textCardNumber.Name = "textCardNumber";
			this.textCardNumber.Size = new System.Drawing.Size(228, 20);
			this.textCardNumber.TabIndex = 29;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(150, 16);
			this.label3.TabIndex = 21;
			this.label3.Text = "Card Number";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(263, 171);
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(118, 20);
			this.textAmount.TabIndex = 34;
			// 
			// labelAmount
			// 
			this.labelAmount.Location = new System.Drawing.Point(263, 154);
			this.labelAmount.Name = "labelAmount";
			this.labelAmount.Size = new System.Drawing.Size(117, 16);
			this.labelAmount.TabIndex = 20;
			this.labelAmount.Text = "Amount";
			this.labelAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPayNote
			// 
			this.textPayNote.AcceptsTab = true;
			this.textPayNote.BackColor = System.Drawing.SystemColors.Window;
			this.textPayNote.DetectLinksEnabled = false;
			this.textPayNote.DetectUrls = false;
			this.textPayNote.Location = new System.Drawing.Point(12, 219);
			this.textPayNote.Name = "textPayNote";
			this.textPayNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Payment;
			this.textPayNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPayNote.Size = new System.Drawing.Size(368, 58);
			this.textPayNote.TabIndex = 36;
			this.textPayNote.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 200);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(150, 16);
			this.label1.TabIndex = 37;
			this.label1.Text = "Payment Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormXWeb
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(408, 337);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textPayNote);
			this.Controls.Add(this.labelRefNumber);
			this.Controls.Add(this.textRefNumber);
			this.Controls.Add(this.groupTransType);
			this.Controls.Add(this.textZipCode);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textSecurityCode);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textNameOnCard);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textExpDate);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textCardNumber);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.labelAmount);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormXWeb";
			this.Text = "XWeb";
			this.Load += new System.EventHandler(this.FormXWeb_Load);
			this.groupTransType.ResumeLayout(false);
			this.groupTransType.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelRefNumber;
		private System.Windows.Forms.TextBox textRefNumber;
		private System.Windows.Forms.GroupBox groupTransType;
		private System.Windows.Forms.RadioButton radioReturn;
		private System.Windows.Forms.TextBox textZipCode;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textSecurityCode;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textNameOnCard;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textExpDate;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textCardNumber;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textAmount;
		private System.Windows.Forms.Label labelAmount;
		private ODtextBox textPayNote;
		private System.Windows.Forms.Label label1;
	}
}