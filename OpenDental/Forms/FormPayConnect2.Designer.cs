namespace OpenDental{
	partial class FormPayConnect2 {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayConnect2));
			this.butOK = new OpenDental.UI.Button();
			this.sigBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.checkSaveToken = new OpenDental.UI.CheckBox();
			this.labelRefNumber = new System.Windows.Forms.Label();
			this.textRefNumber = new System.Windows.Forms.TextBox();
			this.groupTransType = new OpenDental.UI.GroupBox();
			this.radioSale = new System.Windows.Forms.RadioButton();
			this.radioRefund = new System.Windows.Forms.RadioButton();
			this.radioAuthorization = new System.Windows.Forms.RadioButton();
			this.radioVoid = new System.Windows.Forms.RadioButton();
			this.labelAmount = new System.Windows.Forms.Label();
			this.groupProcessMethod = new OpenDental.UI.GroupBox();
			this.radioWebService = new System.Windows.Forms.RadioButton();
			this.radioTerminal = new System.Windows.Forms.RadioButton();
			this.comboTerminal = new OpenDental.UI.ComboBox();
			this.labelTerminal = new System.Windows.Forms.Label();
			this.textAmount = new OpenDental.ValidDouble();
			this.groupTransType.SuspendLayout();
			this.groupProcessMethod.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(333, 359);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 39;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// sigBoxWrapper
			// 
			this.sigBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.sigBoxWrapper.Location = new System.Drawing.Point(21, 266);
			this.sigBoxWrapper.Name = "sigBoxWrapper";
			this.sigBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.sigBoxWrapper.Size = new System.Drawing.Size(369, 81);
			this.sigBoxWrapper.TabIndex = 42;
			this.sigBoxWrapper.TabStop = false;
			this.sigBoxWrapper.UserSig = null;
			// 
			// checkSaveToken
			// 
			this.checkSaveToken.Location = new System.Drawing.Point(23, 236);
			this.checkSaveToken.Name = "checkSaveToken";
			this.checkSaveToken.Size = new System.Drawing.Size(150, 17);
			this.checkSaveToken.TabIndex = 38;
			this.checkSaveToken.Text = "Save Card";
			// 
			// labelRefNumber
			// 
			this.labelRefNumber.Location = new System.Drawing.Point(23, 193);
			this.labelRefNumber.Name = "labelRefNumber";
			this.labelRefNumber.Size = new System.Drawing.Size(117, 16);
			this.labelRefNumber.TabIndex = 24;
			this.labelRefNumber.Text = "Reference Number";
			this.labelRefNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textRefNumber
			// 
			this.textRefNumber.Location = new System.Drawing.Point(23, 210);
			this.textRefNumber.Name = "textRefNumber";
			this.textRefNumber.Size = new System.Drawing.Size(118, 20);
			this.textRefNumber.TabIndex = 37;
			// 
			// groupTransType
			// 
			this.groupTransType.Controls.Add(this.radioSale);
			this.groupTransType.Controls.Add(this.radioRefund);
			this.groupTransType.Controls.Add(this.radioAuthorization);
			this.groupTransType.Controls.Add(this.radioVoid);
			this.groupTransType.Location = new System.Drawing.Point(21, 12);
			this.groupTransType.Name = "groupTransType";
			this.groupTransType.Size = new System.Drawing.Size(228, 50);
			this.groupTransType.TabIndex = 26;
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
			// 
			// radioRefund
			// 
			this.radioRefund.AutoSize = true;
			this.radioRefund.Location = new System.Drawing.Point(156, 19);
			this.radioRefund.Name = "radioRefund";
			this.radioRefund.Size = new System.Drawing.Size(60, 17);
			this.radioRefund.TabIndex = 0;
			this.radioRefund.Text = "Refund";
			this.radioRefund.UseVisualStyleBackColor = true;
			this.radioRefund.CheckedChanged += new System.EventHandler(this.transactionTypeCheckedChanged);
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
			this.radioVoid.CheckedChanged += new System.EventHandler(this.transactionTypeCheckedChanged);
			// 
			// labelAmount
			// 
			this.labelAmount.Location = new System.Drawing.Point(23, 153);
			this.labelAmount.Name = "labelAmount";
			this.labelAmount.Size = new System.Drawing.Size(117, 16);
			this.labelAmount.TabIndex = 20;
			this.labelAmount.Text = "Amount";
			this.labelAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupProcessMethod
			// 
			this.groupProcessMethod.Controls.Add(this.radioWebService);
			this.groupProcessMethod.Controls.Add(this.radioTerminal);
			this.groupProcessMethod.Location = new System.Drawing.Point(21, 68);
			this.groupProcessMethod.Name = "groupProcessMethod";
			this.groupProcessMethod.Size = new System.Drawing.Size(228, 50);
			this.groupProcessMethod.TabIndex = 43;
			this.groupProcessMethod.Text = "Processing Method";
			// 
			// radioWebService
			// 
			this.radioWebService.AutoSize = true;
			this.radioWebService.Checked = true;
			this.radioWebService.Location = new System.Drawing.Point(5, 19);
			this.radioWebService.Name = "radioWebService";
			this.radioWebService.Size = new System.Drawing.Size(87, 17);
			this.radioWebService.TabIndex = 0;
			this.radioWebService.TabStop = true;
			this.radioWebService.Text = "Web Service";
			this.radioWebService.UseVisualStyleBackColor = true;
			this.radioWebService.CheckedChanged += new System.EventHandler(this.radioWebService_CheckedChanged);
			// 
			// radioTerminal
			// 
			this.radioTerminal.Location = new System.Drawing.Point(106, 19);
			this.radioTerminal.Name = "radioTerminal";
			this.radioTerminal.Size = new System.Drawing.Size(70, 17);
			this.radioTerminal.TabIndex = 0;
			this.radioTerminal.Text = "Terminal";
			this.radioTerminal.UseVisualStyleBackColor = true;
			this.radioTerminal.CheckedChanged += new System.EventHandler(this.radioTerminal_CheckedChanged);
			// 
			// comboTerminal
			// 
			this.comboTerminal.Location = new System.Drawing.Point(255, 97);
			this.comboTerminal.Name = "comboTerminal";
			this.comboTerminal.Size = new System.Drawing.Size(142, 21);
			this.comboTerminal.TabIndex = 44;
			this.comboTerminal.Text = "comboBox1";
			// 
			// labelTerminal
			// 
			this.labelTerminal.Location = new System.Drawing.Point(256, 68);
			this.labelTerminal.Name = "labelTerminal";
			this.labelTerminal.Size = new System.Drawing.Size(151, 26);
			this.labelTerminal.TabIndex = 45;
			this.labelTerminal.Text = "Terminal for transaction";
			this.labelTerminal.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(21, 170);
			this.textAmount.MaxVal = 100000000D;
			this.textAmount.MinVal = 0D;
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(118, 20);
			this.textAmount.TabIndex = 37;
			// 
			// FormPayConnect2
			// 
			this.ClientSize = new System.Drawing.Size(420, 395);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.labelTerminal);
			this.Controls.Add(this.comboTerminal);
			this.Controls.Add(this.groupProcessMethod);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.sigBoxWrapper);
			this.Controls.Add(this.checkSaveToken);
			this.Controls.Add(this.labelRefNumber);
			this.Controls.Add(this.textRefNumber);
			this.Controls.Add(this.groupTransType);
			this.Controls.Add(this.labelAmount);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayConnect2";
			this.Text = "PayConnect Payment Information";
			this.Load += new System.EventHandler(this.FormPayConnect2_Load);
			this.groupTransType.ResumeLayout(false);
			this.groupTransType.PerformLayout();
			this.groupProcessMethod.ResumeLayout(false);
			this.groupProcessMethod.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butOK;
		private UI.SignatureBoxWrapper sigBoxWrapper;
		private UI.CheckBox checkSaveToken;
		private System.Windows.Forms.Label labelRefNumber;
		private System.Windows.Forms.TextBox textRefNumber;
		private UI.GroupBox groupTransType;
		private System.Windows.Forms.RadioButton radioSale;
		private System.Windows.Forms.RadioButton radioRefund;
		private System.Windows.Forms.RadioButton radioAuthorization;
		private System.Windows.Forms.RadioButton radioVoid;
		private System.Windows.Forms.Label labelAmount;
		private UI.GroupBox groupProcessMethod;
		private System.Windows.Forms.RadioButton radioWebService;
		private System.Windows.Forms.RadioButton radioTerminal;
		private UI.ComboBox comboTerminal;
		private System.Windows.Forms.Label labelTerminal;
		private ValidDouble textAmount;
	}
}