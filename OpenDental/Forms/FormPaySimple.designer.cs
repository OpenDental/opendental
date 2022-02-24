namespace OpenDental{
	partial class FormPaySimple {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPaySimple));
			this.butCancel = new OpenDental.UI.Button();
			this.checkOneTimePayment = new System.Windows.Forms.CheckBox();
			this.labelRefNumber = new System.Windows.Forms.Label();
			this.textRefNumber = new System.Windows.Forms.TextBox();
			this.groupTransType = new System.Windows.Forms.GroupBox();
			this.radioSale = new System.Windows.Forms.RadioButton();
			this.radioReturn = new System.Windows.Forms.RadioButton();
			this.radioAuthorization = new System.Windows.Forms.RadioButton();
			this.radioVoid = new System.Windows.Forms.RadioButton();
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
			this.butOK = new OpenDental.UI.Button();
			this.pd2 = new System.Drawing.Printing.PrintDocument();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabCredit = new System.Windows.Forms.TabPage();
			this.tabACH = new System.Windows.Forms.TabPage();
			this.labelAmountACH = new System.Windows.Forms.Label();
			this.textAmountACH = new System.Windows.Forms.TextBox();
			this.checkOneTimePaymentACH = new System.Windows.Forms.CheckBox();
			this.groupBankAccountType = new System.Windows.Forms.GroupBox();
			this.radioCheckings = new System.Windows.Forms.RadioButton();
			this.radioSavings = new System.Windows.Forms.RadioButton();
			this.label8 = new System.Windows.Forms.Label();
			this.textRoutingNumber = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textCheckSaveNumber = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBankName = new System.Windows.Forms.TextBox();
			this.timerParseCardSwipe = new System.Windows.Forms.Timer(this.components);
			this.groupTransType.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabCredit.SuspendLayout();
			this.tabACH.SuspendLayout();
			this.groupBankAccountType.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(352, 343);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkOneTimePayment
			// 
			this.checkOneTimePayment.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOneTimePayment.Location = new System.Drawing.Point(17, 205);
			this.checkOneTimePayment.Name = "checkOneTimePayment";
			this.checkOneTimePayment.Size = new System.Drawing.Size(150, 17);
			this.checkOneTimePayment.TabIndex = 38;
			this.checkOneTimePayment.Text = "One-Time Payment";
			// 
			// labelRefNumber
			// 
			this.labelRefNumber.Location = new System.Drawing.Point(268, 79);
			this.labelRefNumber.Name = "labelRefNumber";
			this.labelRefNumber.Size = new System.Drawing.Size(117, 16);
			this.labelRefNumber.TabIndex = 28;
			this.labelRefNumber.Text = "Ref Number";
			this.labelRefNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelRefNumber.Visible = false;
			// 
			// textRefNumber
			// 
			this.textRefNumber.Location = new System.Drawing.Point(268, 96);
			this.textRefNumber.Name = "textRefNumber";
			this.textRefNumber.Size = new System.Drawing.Size(118, 20);
			this.textRefNumber.TabIndex = 37;
			this.textRefNumber.Visible = false;
			// 
			// groupTransType
			// 
			this.groupTransType.Controls.Add(this.radioSale);
			this.groupTransType.Controls.Add(this.radioReturn);
			this.groupTransType.Controls.Add(this.radioAuthorization);
			this.groupTransType.Controls.Add(this.radioVoid);
			this.groupTransType.Location = new System.Drawing.Point(17, 11);
			this.groupTransType.Name = "groupTransType";
			this.groupTransType.Size = new System.Drawing.Size(228, 50);
			this.groupTransType.TabIndex = 25;
			this.groupTransType.TabStop = false;
			this.groupTransType.Text = "Transaction Type";
			// 
			// radioSale
			// 
			this.radioSale.AutoSize = true;
			this.radioSale.Checked = true;
			this.radioSale.Location = new System.Drawing.Point(5, 19);
			this.radioSale.Name = "radioSale";
			this.radioSale.Size = new System.Drawing.Size(46, 17);
			this.radioSale.TabIndex = 0;
			this.radioSale.TabStop = true;
			this.radioSale.Text = "Sale";
			this.radioSale.UseVisualStyleBackColor = true;
			this.radioSale.Click += new System.EventHandler(this.radioSale_Click);
			// 
			// radioReturn
			// 
			this.radioReturn.AutoSize = true;
			this.radioReturn.Location = new System.Drawing.Point(156, 19);
			this.radioReturn.Name = "radioReturn";
			this.radioReturn.Size = new System.Drawing.Size(57, 17);
			this.radioReturn.TabIndex = 0;
			this.radioReturn.Text = "Return";
			this.radioReturn.UseVisualStyleBackColor = true;
			this.radioReturn.Click += new System.EventHandler(this.radioReturn_Click);
			// 
			// radioAuthorization
			// 
			this.radioAuthorization.AutoSize = true;
			this.radioAuthorization.Location = new System.Drawing.Point(55, 19);
			this.radioAuthorization.Name = "radioAuthorization";
			this.radioAuthorization.Size = new System.Drawing.Size(47, 17);
			this.radioAuthorization.TabIndex = 0;
			this.radioAuthorization.Text = "Auth";
			this.radioAuthorization.UseVisualStyleBackColor = true;
			this.radioAuthorization.Click += new System.EventHandler(this.radioAuthorization_Click);
			// 
			// radioVoid
			// 
			this.radioVoid.AutoSize = true;
			this.radioVoid.Location = new System.Drawing.Point(106, 19);
			this.radioVoid.Name = "radioVoid";
			this.radioVoid.Size = new System.Drawing.Size(46, 17);
			this.radioVoid.TabIndex = 0;
			this.radioVoid.Text = "Void";
			this.radioVoid.UseVisualStyleBackColor = true;
			this.radioVoid.Click += new System.EventHandler(this.radioVoid_Click);
			// 
			// textZipCode
			// 
			this.textZipCode.Location = new System.Drawing.Point(268, 182);
			this.textZipCode.Name = "textZipCode";
			this.textZipCode.Size = new System.Drawing.Size(118, 20);
			this.textZipCode.TabIndex = 35;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(268, 165);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(117, 16);
			this.label7.TabIndex = 20;
			this.label7.Text = "Zip Code";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSecurityCode
			// 
			this.textSecurityCode.Location = new System.Drawing.Point(268, 139);
			this.textSecurityCode.Name = "textSecurityCode";
			this.textSecurityCode.Size = new System.Drawing.Size(118, 20);
			this.textSecurityCode.TabIndex = 34;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(268, 122);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(117, 16);
			this.label6.TabIndex = 24;
			this.label6.Text = "Security Code";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textNameOnCard
			// 
			this.textNameOnCard.Location = new System.Drawing.Point(17, 182);
			this.textNameOnCard.Name = "textNameOnCard";
			this.textNameOnCard.Size = new System.Drawing.Size(228, 20);
			this.textNameOnCard.TabIndex = 33;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(17, 165);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(150, 16);
			this.label5.TabIndex = 23;
			this.label5.Text = "Name On Card";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textExpDate
			// 
			this.textExpDate.Location = new System.Drawing.Point(17, 139);
			this.textExpDate.Name = "textExpDate";
			this.textExpDate.Size = new System.Drawing.Size(150, 20);
			this.textExpDate.TabIndex = 32;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(17, 122);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(150, 16);
			this.label4.TabIndex = 22;
			this.label4.Text = "Expiration (MMYY)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textCardNumber
			// 
			this.textCardNumber.Location = new System.Drawing.Point(17, 96);
			this.textCardNumber.Name = "textCardNumber";
			this.textCardNumber.Size = new System.Drawing.Size(228, 20);
			this.textCardNumber.TabIndex = 30;
			this.textCardNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCardNumber_KeyPress);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(17, 79);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(150, 16);
			this.label3.TabIndex = 21;
			this.label3.Text = "Card Number";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(268, 225);
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(118, 20);
			this.textAmount.TabIndex = 36;
			// 
			// labelAmount
			// 
			this.labelAmount.Location = new System.Drawing.Point(268, 208);
			this.labelAmount.Name = "labelAmount";
			this.labelAmount.Size = new System.Drawing.Size(117, 16);
			this.labelAmount.TabIndex = 26;
			this.labelAmount.Text = "Amount";
			this.labelAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(261, 343);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabCredit);
			this.tabControl.Controls.Add(this.tabACH);
			this.tabControl.Location = new System.Drawing.Point(20, 12);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(420, 315);
			this.tabControl.TabIndex = 39;
			// 
			// tabCredit
			// 
			this.tabCredit.BackColor = System.Drawing.SystemColors.Control;
			this.tabCredit.Controls.Add(this.groupTransType);
			this.tabCredit.Controls.Add(this.checkOneTimePayment);
			this.tabCredit.Controls.Add(this.labelAmount);
			this.tabCredit.Controls.Add(this.labelRefNumber);
			this.tabCredit.Controls.Add(this.textAmount);
			this.tabCredit.Controls.Add(this.textRefNumber);
			this.tabCredit.Controls.Add(this.label3);
			this.tabCredit.Controls.Add(this.textCardNumber);
			this.tabCredit.Controls.Add(this.textZipCode);
			this.tabCredit.Controls.Add(this.label4);
			this.tabCredit.Controls.Add(this.label7);
			this.tabCredit.Controls.Add(this.textExpDate);
			this.tabCredit.Controls.Add(this.textSecurityCode);
			this.tabCredit.Controls.Add(this.label5);
			this.tabCredit.Controls.Add(this.label6);
			this.tabCredit.Controls.Add(this.textNameOnCard);
			this.tabCredit.Location = new System.Drawing.Point(4, 22);
			this.tabCredit.Name = "tabCredit";
			this.tabCredit.Padding = new System.Windows.Forms.Padding(3);
			this.tabCredit.Size = new System.Drawing.Size(412, 289);
			this.tabCredit.TabIndex = 0;
			this.tabCredit.Text = "Credit/Debit";
			// 
			// tabACH
			// 
			this.tabACH.BackColor = System.Drawing.SystemColors.Control;
			this.tabACH.Controls.Add(this.labelAmountACH);
			this.tabACH.Controls.Add(this.textAmountACH);
			this.tabACH.Controls.Add(this.checkOneTimePaymentACH);
			this.tabACH.Controls.Add(this.groupBankAccountType);
			this.tabACH.Controls.Add(this.label8);
			this.tabACH.Controls.Add(this.textRoutingNumber);
			this.tabACH.Controls.Add(this.label2);
			this.tabACH.Controls.Add(this.textCheckSaveNumber);
			this.tabACH.Controls.Add(this.label1);
			this.tabACH.Controls.Add(this.textBankName);
			this.tabACH.Location = new System.Drawing.Point(4, 22);
			this.tabACH.Name = "tabACH";
			this.tabACH.Padding = new System.Windows.Forms.Padding(3);
			this.tabACH.Size = new System.Drawing.Size(412, 289);
			this.tabACH.TabIndex = 1;
			this.tabACH.Text = "ACH";
			// 
			// labelAmountACH
			// 
			this.labelAmountACH.Location = new System.Drawing.Point(92, 209);
			this.labelAmountACH.Name = "labelAmountACH";
			this.labelAmountACH.Size = new System.Drawing.Size(117, 16);
			this.labelAmountACH.TabIndex = 40;
			this.labelAmountACH.Text = "Amount";
			this.labelAmountACH.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textAmountACH
			// 
			this.textAmountACH.Location = new System.Drawing.Point(92, 226);
			this.textAmountACH.Name = "textAmountACH";
			this.textAmountACH.Size = new System.Drawing.Size(118, 20);
			this.textAmountACH.TabIndex = 25;
			// 
			// checkOneTimePaymentACH
			// 
			this.checkOneTimePaymentACH.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOneTimePaymentACH.Location = new System.Drawing.Point(92, 255);
			this.checkOneTimePaymentACH.Name = "checkOneTimePaymentACH";
			this.checkOneTimePaymentACH.Size = new System.Drawing.Size(150, 17);
			this.checkOneTimePaymentACH.TabIndex = 30;
			this.checkOneTimePaymentACH.Text = "One-Time Payment";
			// 
			// groupBankAccountType
			// 
			this.groupBankAccountType.Controls.Add(this.radioCheckings);
			this.groupBankAccountType.Controls.Add(this.radioSavings);
			this.groupBankAccountType.Location = new System.Drawing.Point(92, 15);
			this.groupBankAccountType.Name = "groupBankAccountType";
			this.groupBankAccountType.Size = new System.Drawing.Size(228, 50);
			this.groupBankAccountType.TabIndex = 5;
			this.groupBankAccountType.TabStop = false;
			this.groupBankAccountType.Text = "Account Type";
			// 
			// radioCheckings
			// 
			this.radioCheckings.Checked = true;
			this.radioCheckings.Location = new System.Drawing.Point(5, 19);
			this.radioCheckings.Name = "radioCheckings";
			this.radioCheckings.Size = new System.Drawing.Size(108, 17);
			this.radioCheckings.TabIndex = 0;
			this.radioCheckings.TabStop = true;
			this.radioCheckings.Text = "Checking";
			this.radioCheckings.UseVisualStyleBackColor = true;
			// 
			// radioSavings
			// 
			this.radioSavings.Location = new System.Drawing.Point(119, 19);
			this.radioSavings.Name = "radioSavings";
			this.radioSavings.Size = new System.Drawing.Size(84, 17);
			this.radioSavings.TabIndex = 0;
			this.radioSavings.Text = "Savings";
			this.radioSavings.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(92, 72);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(150, 16);
			this.label8.TabIndex = 35;
			this.label8.Text = "Routing Number";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textRoutingNumber
			// 
			this.textRoutingNumber.Location = new System.Drawing.Point(92, 89);
			this.textRoutingNumber.Name = "textRoutingNumber";
			this.textRoutingNumber.Size = new System.Drawing.Size(228, 20);
			this.textRoutingNumber.TabIndex = 10;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(92, 119);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(150, 16);
			this.label2.TabIndex = 33;
			this.label2.Text = "Account Number";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textCheckSaveNumber
			// 
			this.textCheckSaveNumber.Location = new System.Drawing.Point(92, 135);
			this.textCheckSaveNumber.Name = "textCheckSaveNumber";
			this.textCheckSaveNumber.Size = new System.Drawing.Size(228, 20);
			this.textCheckSaveNumber.TabIndex = 15;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(92, 163);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(150, 16);
			this.label1.TabIndex = 31;
			this.label1.Text = "Bank Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBankName
			// 
			this.textBankName.Location = new System.Drawing.Point(92, 180);
			this.textBankName.Name = "textBankName";
			this.textBankName.Size = new System.Drawing.Size(228, 20);
			this.textBankName.TabIndex = 20;
			// 
			// timerParseCardSwipe
			// 
			this.timerParseCardSwipe.Interval = 500;
			this.timerParseCardSwipe.Tick += new System.EventHandler(this.timerParseCardSwipe_Tick);
			// 
			// FormPaySimple
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(456, 379);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPaySimple";
			this.Text = "PaySimple Payment Information";
			this.Load += new System.EventHandler(this.FormPaySimple_Load);
			this.groupTransType.ResumeLayout(false);
			this.groupTransType.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.tabCredit.ResumeLayout(false);
			this.tabCredit.PerformLayout();
			this.tabACH.ResumeLayout(false);
			this.tabACH.PerformLayout();
			this.groupBankAccountType.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkOneTimePayment;
		private System.Windows.Forms.Label labelRefNumber;
		private System.Windows.Forms.TextBox textRefNumber;
		private System.Windows.Forms.GroupBox groupTransType;
		private System.Windows.Forms.RadioButton radioSale;
		private System.Windows.Forms.RadioButton radioReturn;
		private System.Windows.Forms.RadioButton radioVoid;
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
		private System.Windows.Forms.RadioButton radioAuthorization;
		private System.Drawing.Printing.PrintDocument pd2;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabCredit;
		private System.Windows.Forms.TabPage tabACH;
		private System.Windows.Forms.CheckBox checkOneTimePaymentACH;
		private System.Windows.Forms.GroupBox groupBankAccountType;
		private System.Windows.Forms.RadioButton radioCheckings;
		private System.Windows.Forms.RadioButton radioSavings;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textRoutingNumber;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textCheckSaveNumber;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBankName;
		private System.Windows.Forms.Label labelAmountACH;
		private System.Windows.Forms.TextBox textAmountACH;
		private System.Windows.Forms.Timer timerParseCardSwipe;
	}
}