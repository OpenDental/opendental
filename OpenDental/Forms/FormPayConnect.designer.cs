namespace OpenDental{
	partial class FormPayConnect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayConnect));
			this.groupProcessMethod = new System.Windows.Forms.GroupBox();
			this.radioWebService = new System.Windows.Forms.RadioButton();
			this.radioTerminal = new System.Windows.Forms.RadioButton();
			this.sigBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.checkForceDuplicate = new System.Windows.Forms.CheckBox();
			this.radioForce = new System.Windows.Forms.RadioButton();
			this.labelStoreCCNumWarning = new System.Windows.Forms.Label();
			this.checkSaveToken = new System.Windows.Forms.CheckBox();
			this.labelRefNumber = new System.Windows.Forms.Label();
			this.textRefNumber = new System.Windows.Forms.TextBox();
			this.groupTransType = new System.Windows.Forms.GroupBox();
			this.radioSale = new System.Windows.Forms.RadioButton();
			this.radioReturn = new System.Windows.Forms.RadioButton();
			this.radioAuthorization = new System.Windows.Forms.RadioButton();
			this.radioVoid = new System.Windows.Forms.RadioButton();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
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
			this.timerParseCardSwipe = new System.Windows.Forms.Timer(this.components);
			this.groupProcessMethod.SuspendLayout();
			this.groupTransType.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupProcessMethod
			// 
			this.groupProcessMethod.Controls.Add(this.radioWebService);
			this.groupProcessMethod.Controls.Add(this.radioTerminal);
			this.groupProcessMethod.Location = new System.Drawing.Point(12, 68);
			this.groupProcessMethod.Name = "groupProcessMethod";
			this.groupProcessMethod.Size = new System.Drawing.Size(228, 50);
			this.groupProcessMethod.TabIndex = 1;
			this.groupProcessMethod.TabStop = false;
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
			// sigBoxWrapper
			// 
			this.sigBoxWrapper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.sigBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.sigBoxWrapper.Location = new System.Drawing.Point(12, 317);
			this.sigBoxWrapper.Name = "sigBoxWrapper";
			this.sigBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.sigBoxWrapper.Size = new System.Drawing.Size(369, 81);
			this.sigBoxWrapper.TabIndex = 19;
			this.sigBoxWrapper.TabStop = false;
			this.sigBoxWrapper.UserSig = null;
			// 
			// checkForceDuplicate
			// 
			this.checkForceDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkForceDuplicate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkForceDuplicate.Location = new System.Drawing.Point(163, 252);
			this.checkForceDuplicate.Name = "checkForceDuplicate";
			this.checkForceDuplicate.Size = new System.Drawing.Size(217, 17);
			this.checkForceDuplicate.TabIndex = 19;
			this.checkForceDuplicate.Text = "Force Duplicate";
			// 
			// radioForce
			// 
			this.radioForce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioForce.AutoSize = true;
			this.radioForce.Location = new System.Drawing.Point(262, 60);
			this.radioForce.Name = "radioForce";
			this.radioForce.Size = new System.Drawing.Size(77, 17);
			this.radioForce.TabIndex = 1;
			this.radioForce.Text = "Force Auth";
			this.radioForce.UseVisualStyleBackColor = true;
			this.radioForce.Visible = false;
			this.radioForce.Click += new System.EventHandler(this.radioForce_Click);
			// 
			// labelStoreCCNumWarning
			// 
			this.labelStoreCCNumWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelStoreCCNumWarning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.labelStoreCCNumWarning.Location = new System.Drawing.Point(54, 275);
			this.labelStoreCCNumWarning.Name = "labelStoreCCNumWarning";
			this.labelStoreCCNumWarning.Size = new System.Drawing.Size(300, 28);
			this.labelStoreCCNumWarning.TabIndex = 0;
			this.labelStoreCCNumWarning.Text = "You should turn off the option in Module Setup for \"allow storing credit card num" +
    "bers\" in order to start using tokens.";
			this.labelStoreCCNumWarning.Visible = false;
			// 
			// checkSaveToken
			// 
			this.checkSaveToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkSaveToken.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSaveToken.Location = new System.Drawing.Point(12, 252);
			this.checkSaveToken.Name = "checkSaveToken";
			this.checkSaveToken.Size = new System.Drawing.Size(150, 17);
			this.checkSaveToken.TabIndex = 8;
			this.checkSaveToken.Text = "Save Token";
			// 
			// labelRefNumber
			// 
			this.labelRefNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelRefNumber.Location = new System.Drawing.Point(263, 80);
			this.labelRefNumber.Name = "labelRefNumber";
			this.labelRefNumber.Size = new System.Drawing.Size(117, 16);
			this.labelRefNumber.TabIndex = 0;
			this.labelRefNumber.Text = "Ref Number";
			this.labelRefNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelRefNumber.Visible = false;
			// 
			// textRefNumber
			// 
			this.textRefNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textRefNumber.Location = new System.Drawing.Point(263, 97);
			this.textRefNumber.Name = "textRefNumber";
			this.textRefNumber.Size = new System.Drawing.Size(118, 20);
			this.textRefNumber.TabIndex = 7;
			this.textRefNumber.Visible = false;
			// 
			// groupTransType
			// 
			this.groupTransType.Controls.Add(this.radioSale);
			this.groupTransType.Controls.Add(this.radioReturn);
			this.groupTransType.Controls.Add(this.radioAuthorization);
			this.groupTransType.Controls.Add(this.radioVoid);
			this.groupTransType.Location = new System.Drawing.Point(12, 12);
			this.groupTransType.Name = "groupTransType";
			this.groupTransType.Size = new System.Drawing.Size(228, 50);
			this.groupTransType.TabIndex = 0;
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
			this.radioReturn.CheckedChanged += new System.EventHandler(this.radioReturn_Changed);
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
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(209, 413);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 17;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(306, 413);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 18;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textZipCode
			// 
			this.textZipCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textZipCode.Location = new System.Drawing.Point(263, 183);
			this.textZipCode.Name = "textZipCode";
			this.textZipCode.Size = new System.Drawing.Size(118, 20);
			this.textZipCode.TabIndex = 5;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(263, 166);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(117, 16);
			this.label7.TabIndex = 0;
			this.label7.Text = "Zip Code";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSecurityCode
			// 
			this.textSecurityCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textSecurityCode.Location = new System.Drawing.Point(263, 140);
			this.textSecurityCode.Name = "textSecurityCode";
			this.textSecurityCode.Size = new System.Drawing.Size(118, 20);
			this.textSecurityCode.TabIndex = 4;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(263, 123);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(117, 16);
			this.label6.TabIndex = 0;
			this.label6.Text = "Security Code";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textNameOnCard
			// 
			this.textNameOnCard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textNameOnCard.Location = new System.Drawing.Point(12, 226);
			this.textNameOnCard.Name = "textNameOnCard";
			this.textNameOnCard.Size = new System.Drawing.Size(228, 20);
			this.textNameOnCard.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(12, 209);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(150, 16);
			this.label5.TabIndex = 0;
			this.label5.Text = "Name On Card";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textExpDate
			// 
			this.textExpDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textExpDate.Location = new System.Drawing.Point(12, 183);
			this.textExpDate.Name = "textExpDate";
			this.textExpDate.Size = new System.Drawing.Size(150, 20);
			this.textExpDate.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(12, 166);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(150, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "Expiration (MMYY)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textCardNumber
			// 
			this.textCardNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textCardNumber.Location = new System.Drawing.Point(12, 140);
			this.textCardNumber.Name = "textCardNumber";
			this.textCardNumber.Size = new System.Drawing.Size(228, 20);
			this.textCardNumber.TabIndex = 1;
			this.textCardNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCardNumber_KeyPress);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(12, 123);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(150, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Card Number";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textAmount
			// 
			this.textAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textAmount.Location = new System.Drawing.Point(263, 226);
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(118, 20);
			this.textAmount.TabIndex = 6;
			// 
			// labelAmount
			// 
			this.labelAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelAmount.Location = new System.Drawing.Point(263, 209);
			this.labelAmount.Name = "labelAmount";
			this.labelAmount.Size = new System.Drawing.Size(117, 16);
			this.labelAmount.TabIndex = 0;
			this.labelAmount.Text = "Amount";
			this.labelAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// timerParseCardSwipe
			// 
			this.timerParseCardSwipe.Interval = 500;
			this.timerParseCardSwipe.Tick += new System.EventHandler(this.timerParseCardSwipe_Tick);
			// 
			// FormPayConnect
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(392, 449);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupProcessMethod);
			this.Controls.Add(this.sigBoxWrapper);
			this.Controls.Add(this.checkForceDuplicate);
			this.Controls.Add(this.radioForce);
			this.Controls.Add(this.labelStoreCCNumWarning);
			this.Controls.Add(this.checkSaveToken);
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
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayConnect";
			this.Text = "Pay Connect Payment Information";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPayConnect_FormClosing);
			this.Load += new System.EventHandler(this.FormPayConnect_Load);
			this.groupProcessMethod.ResumeLayout(false);
			this.groupProcessMethod.PerformLayout();
			this.groupTransType.ResumeLayout(false);
			this.groupTransType.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelAmount;
		private System.Windows.Forms.TextBox textAmount;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textCardNumber;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textExpDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textNameOnCard;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textSecurityCode;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textZipCode;
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.RadioButton radioSale;
		private System.Windows.Forms.RadioButton radioAuthorization;
		private System.Windows.Forms.RadioButton radioVoid;
		private System.Windows.Forms.RadioButton radioReturn;
		private System.Windows.Forms.GroupBox groupTransType;
		private System.Windows.Forms.TextBox textRefNumber;
		private System.Windows.Forms.Label labelRefNumber;
		private System.Windows.Forms.CheckBox checkSaveToken;
		private System.Windows.Forms.Label labelStoreCCNumWarning;
		private System.Windows.Forms.RadioButton radioForce;
		private UI.SignatureBoxWrapper sigBoxWrapper;
		private System.Windows.Forms.CheckBox checkForceDuplicate;
		private System.Windows.Forms.GroupBox groupProcessMethod;
		private System.Windows.Forms.RadioButton radioWebService;
		private System.Windows.Forms.RadioButton radioTerminal;
		private System.Windows.Forms.Timer timerParseCardSwipe;
	}
}