namespace OpenDental{
	partial class FormAvaTax {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAvaTax));
			this.butCancel = new OpenDental.UI.Button();
			this.groupProcCodes = new System.Windows.Forms.GroupBox();
			this.textDiscountCodes = new System.Windows.Forms.TextBox();
			this.labelDiscountCodes = new System.Windows.Forms.Label();
			this.labelPrePayCodes = new System.Windows.Forms.Label();
			this.textPrePayCodes = new System.Windows.Forms.TextBox();
			this.labelStatesTaxed = new System.Windows.Forms.Label();
			this.listBoxTaxedStates = new OpenDental.UI.ListBoxOD();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.labelStatesNotTaxed = new System.Windows.Forms.Label();
			this.listBoxNonTaxedStates = new OpenDental.UI.ListBoxOD();
			this.groupOffice = new System.Windows.Forms.GroupBox();
			this.validTaxLockDate = new OpenDental.ValidDate();
			this.labelSalesTaxLockDate = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textReturnAdjType = new System.Windows.Forms.TextBox();
			this.butChooseRetAdjType = new OpenDental.UI.Button();
			this.labelTaxExempt = new System.Windows.Forms.Label();
			this.textTaxExempt = new System.Windows.Forms.TextBox();
			this.butChooseTaxExempt = new OpenDental.UI.Button();
			this.labelST = new System.Windows.Forms.Label();
			this.textAdjType = new System.Windows.Forms.TextBox();
			this.labelCompanyCode = new System.Windows.Forms.Label();
			this.textCompanyCode = new System.Windows.Forms.TextBox();
			this.butChooseTaxAdjType = new OpenDental.UI.Button();
			this.groupApiSettings = new System.Windows.Forms.GroupBox();
			this.listBoxLogLevel = new OpenDental.UI.ListBoxOD();
			this.labelLogLevel = new System.Windows.Forms.Label();
			this.labelPass = new System.Windows.Forms.Label();
			this.butPing = new OpenDental.UI.Button();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.labelUser = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.radioProdEnv = new System.Windows.Forms.RadioButton();
			this.radioTestEnv = new System.Windows.Forms.RadioButton();
			this.butOK = new OpenDental.UI.Button();
			this.groupOverrides = new System.Windows.Forms.GroupBox();
			this.textOverrides = new System.Windows.Forms.TextBox();
			this.labelOverrides = new System.Windows.Forms.Label();
			this.groupProcCodes.SuspendLayout();
			this.groupOffice.SuspendLayout();
			this.groupApiSettings.SuspendLayout();
			this.groupOverrides.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(518, 645);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupProcCodes
			// 
			this.groupProcCodes.Controls.Add(this.textDiscountCodes);
			this.groupProcCodes.Controls.Add(this.labelDiscountCodes);
			this.groupProcCodes.Controls.Add(this.labelPrePayCodes);
			this.groupProcCodes.Controls.Add(this.textPrePayCodes);
			this.groupProcCodes.Location = new System.Drawing.Point(12, 354);
			this.groupProcCodes.Name = "groupProcCodes";
			this.groupProcCodes.Size = new System.Drawing.Size(317, 170);
			this.groupProcCodes.TabIndex = 4;
			this.groupProcCodes.TabStop = false;
			this.groupProcCodes.Text = "Pre-Payable Procedure Codes";
			// 
			// textDiscountCodes
			// 
			this.textDiscountCodes.Location = new System.Drawing.Point(15, 100);
			this.textDiscountCodes.Multiline = true;
			this.textDiscountCodes.Name = "textDiscountCodes";
			this.textDiscountCodes.Size = new System.Drawing.Size(277, 18);
			this.textDiscountCodes.TabIndex = 3;
			// 
			// labelDiscountCodes
			// 
			this.labelDiscountCodes.Location = new System.Drawing.Point(15, 71);
			this.labelDiscountCodes.Name = "labelDiscountCodes";
			this.labelDiscountCodes.Size = new System.Drawing.Size(296, 26);
			this.labelDiscountCodes.TabIndex = 2;
			this.labelDiscountCodes.Text = "Enter the procedure codes that will be eligible for a discount in the pre-payment" +
    " tool as a comma-separated list.";
			this.labelDiscountCodes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPrePayCodes
			// 
			this.labelPrePayCodes.Location = new System.Drawing.Point(15, 16);
			this.labelPrePayCodes.Name = "labelPrePayCodes";
			this.labelPrePayCodes.Size = new System.Drawing.Size(296, 26);
			this.labelPrePayCodes.TabIndex = 0;
			this.labelPrePayCodes.Text = "Enter the procedure codes that will appear in the pre-payment tool as a comma-sep" +
    "arated list.";
			this.labelPrePayCodes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPrePayCodes
			// 
			this.textPrePayCodes.Location = new System.Drawing.Point(15, 45);
			this.textPrePayCodes.Multiline = true;
			this.textPrePayCodes.Name = "textPrePayCodes";
			this.textPrePayCodes.Size = new System.Drawing.Size(277, 18);
			this.textPrePayCodes.TabIndex = 1;
			// 
			// labelStatesTaxed
			// 
			this.labelStatesTaxed.Location = new System.Drawing.Point(194, 26);
			this.labelStatesTaxed.Name = "labelStatesTaxed";
			this.labelStatesTaxed.Size = new System.Drawing.Size(135, 16);
			this.labelStatesTaxed.TabIndex = 57;
			this.labelStatesTaxed.Text = "Taxed States";
			this.labelStatesTaxed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBoxTaxedStates
			// 
			this.listBoxTaxedStates.Location = new System.Drawing.Point(194, 45);
			this.listBoxTaxedStates.Name = "listBoxTaxedStates";
			this.listBoxTaxedStates.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listBoxTaxedStates.Size = new System.Drawing.Size(135, 303);
			this.listBoxTaxedStates.TabIndex = 56;
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(153, 157);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 26);
			this.butRight.TabIndex = 55;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(153, 191);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 26);
			this.butLeft.TabIndex = 54;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.Location = new System.Drawing.Point(15, 8);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(104, 17);
			this.checkEnabled.TabIndex = 8;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// labelStatesNotTaxed
			// 
			this.labelStatesNotTaxed.Location = new System.Drawing.Point(12, 28);
			this.labelStatesNotTaxed.Name = "labelStatesNotTaxed";
			this.labelStatesNotTaxed.Size = new System.Drawing.Size(135, 16);
			this.labelStatesNotTaxed.TabIndex = 0;
			this.labelStatesNotTaxed.Text = "Non-Taxed States";
			this.labelStatesNotTaxed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBoxNonTaxedStates
			// 
			this.listBoxNonTaxedStates.Location = new System.Drawing.Point(12, 45);
			this.listBoxNonTaxedStates.Name = "listBoxNonTaxedStates";
			this.listBoxNonTaxedStates.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listBoxNonTaxedStates.Size = new System.Drawing.Size(135, 303);
			this.listBoxNonTaxedStates.TabIndex = 1;
			// 
			// groupOffice
			// 
			this.groupOffice.Controls.Add(this.validTaxLockDate);
			this.groupOffice.Controls.Add(this.labelSalesTaxLockDate);
			this.groupOffice.Controls.Add(this.label1);
			this.groupOffice.Controls.Add(this.textReturnAdjType);
			this.groupOffice.Controls.Add(this.butChooseRetAdjType);
			this.groupOffice.Controls.Add(this.labelTaxExempt);
			this.groupOffice.Controls.Add(this.textTaxExempt);
			this.groupOffice.Controls.Add(this.butChooseTaxExempt);
			this.groupOffice.Controls.Add(this.labelST);
			this.groupOffice.Controls.Add(this.textAdjType);
			this.groupOffice.Controls.Add(this.labelCompanyCode);
			this.groupOffice.Controls.Add(this.textCompanyCode);
			this.groupOffice.Controls.Add(this.butChooseTaxAdjType);
			this.groupOffice.Location = new System.Drawing.Point(339, 297);
			this.groupOffice.Name = "groupOffice";
			this.groupOffice.Size = new System.Drawing.Size(254, 227);
			this.groupOffice.TabIndex = 3;
			this.groupOffice.TabStop = false;
			this.groupOffice.Text = "Office Information";
			// 
			// validTaxLockDate
			// 
			this.validTaxLockDate.Location = new System.Drawing.Point(15, 190);
			this.validTaxLockDate.Name = "validTaxLockDate";
			this.validTaxLockDate.Size = new System.Drawing.Size(164, 20);
			this.validTaxLockDate.TabIndex = 10;
			// 
			// labelSalesTaxLockDate
			// 
			this.labelSalesTaxLockDate.Location = new System.Drawing.Point(15, 174);
			this.labelSalesTaxLockDate.Name = "labelSalesTaxLockDate";
			this.labelSalesTaxLockDate.Size = new System.Drawing.Size(196, 16);
			this.labelSalesTaxLockDate.TabIndex = 9;
			this.labelSalesTaxLockDate.Text = "Tax Lock Date (e.g. 3/31/19)";
			this.labelSalesTaxLockDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 100);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(164, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Sales Tax Return Adjustment Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textReturnAdjType
			// 
			this.textReturnAdjType.Location = new System.Drawing.Point(15, 114);
			this.textReturnAdjType.Name = "textReturnAdjType";
			this.textReturnAdjType.ReadOnly = true;
			this.textReturnAdjType.Size = new System.Drawing.Size(164, 20);
			this.textReturnAdjType.TabIndex = 7;
			this.textReturnAdjType.TabStop = false;
			// 
			// butChooseRetAdjType
			// 
			this.butChooseRetAdjType.Location = new System.Drawing.Point(185, 112);
			this.butChooseRetAdjType.Name = "butChooseRetAdjType";
			this.butChooseRetAdjType.Size = new System.Drawing.Size(26, 22);
			this.butChooseRetAdjType.TabIndex = 8;
			this.butChooseRetAdjType.Text = "...";
			this.butChooseRetAdjType.UseVisualStyleBackColor = true;
			this.butChooseRetAdjType.Click += new System.EventHandler(this.butChooseRetAdjType_Click);
			// 
			// labelTaxExempt
			// 
			this.labelTaxExempt.Location = new System.Drawing.Point(15, 137);
			this.labelTaxExempt.Name = "labelTaxExempt";
			this.labelTaxExempt.Size = new System.Drawing.Size(164, 13);
			this.labelTaxExempt.TabIndex = 3;
			this.labelTaxExempt.Text = "Tax Exempt Pat Field Def";
			this.labelTaxExempt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textTaxExempt
			// 
			this.textTaxExempt.Location = new System.Drawing.Point(15, 151);
			this.textTaxExempt.Name = "textTaxExempt";
			this.textTaxExempt.ReadOnly = true;
			this.textTaxExempt.Size = new System.Drawing.Size(164, 20);
			this.textTaxExempt.TabIndex = 4;
			this.textTaxExempt.TabStop = false;
			// 
			// butChooseTaxExempt
			// 
			this.butChooseTaxExempt.Location = new System.Drawing.Point(185, 149);
			this.butChooseTaxExempt.Name = "butChooseTaxExempt";
			this.butChooseTaxExempt.Size = new System.Drawing.Size(26, 22);
			this.butChooseTaxExempt.TabIndex = 5;
			this.butChooseTaxExempt.Text = "...";
			this.butChooseTaxExempt.UseVisualStyleBackColor = true;
			this.butChooseTaxExempt.Click += new System.EventHandler(this.butChooseTaxExempt_Click);
			// 
			// labelST
			// 
			this.labelST.Location = new System.Drawing.Point(15, 63);
			this.labelST.Name = "labelST";
			this.labelST.Size = new System.Drawing.Size(164, 13);
			this.labelST.TabIndex = 0;
			this.labelST.Text = "Sales Tax Adjustment Type";
			this.labelST.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textAdjType
			// 
			this.textAdjType.Location = new System.Drawing.Point(15, 77);
			this.textAdjType.Name = "textAdjType";
			this.textAdjType.ReadOnly = true;
			this.textAdjType.Size = new System.Drawing.Size(164, 20);
			this.textAdjType.TabIndex = 0;
			this.textAdjType.TabStop = false;
			// 
			// labelCompanyCode
			// 
			this.labelCompanyCode.Location = new System.Drawing.Point(15, 23);
			this.labelCompanyCode.Name = "labelCompanyCode";
			this.labelCompanyCode.Size = new System.Drawing.Size(135, 16);
			this.labelCompanyCode.TabIndex = 0;
			this.labelCompanyCode.Text = "Company Code";
			this.labelCompanyCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textCompanyCode
			// 
			this.textCompanyCode.Location = new System.Drawing.Point(15, 40);
			this.textCompanyCode.Name = "textCompanyCode";
			this.textCompanyCode.Size = new System.Drawing.Size(164, 20);
			this.textCompanyCode.TabIndex = 1;
			// 
			// butChooseTaxAdjType
			// 
			this.butChooseTaxAdjType.Location = new System.Drawing.Point(185, 75);
			this.butChooseTaxAdjType.Name = "butChooseTaxAdjType";
			this.butChooseTaxAdjType.Size = new System.Drawing.Size(26, 22);
			this.butChooseTaxAdjType.TabIndex = 2;
			this.butChooseTaxAdjType.Text = "...";
			this.butChooseTaxAdjType.UseVisualStyleBackColor = true;
			this.butChooseTaxAdjType.Click += new System.EventHandler(this.butChooseTaxAdjType_Click);
			// 
			// groupApiSettings
			// 
			this.groupApiSettings.Controls.Add(this.listBoxLogLevel);
			this.groupApiSettings.Controls.Add(this.labelLogLevel);
			this.groupApiSettings.Controls.Add(this.labelPass);
			this.groupApiSettings.Controls.Add(this.butPing);
			this.groupApiSettings.Controls.Add(this.textPassword);
			this.groupApiSettings.Controls.Add(this.labelUser);
			this.groupApiSettings.Controls.Add(this.textUsername);
			this.groupApiSettings.Controls.Add(this.radioProdEnv);
			this.groupApiSettings.Controls.Add(this.radioTestEnv);
			this.groupApiSettings.Location = new System.Drawing.Point(339, 8);
			this.groupApiSettings.Name = "groupApiSettings";
			this.groupApiSettings.Size = new System.Drawing.Size(254, 283);
			this.groupApiSettings.TabIndex = 2;
			this.groupApiSettings.TabStop = false;
			this.groupApiSettings.Text = "API Settings";
			// 
			// listBoxLogLevel
			// 
			this.listBoxLogLevel.Location = new System.Drawing.Point(15, 201);
			this.listBoxLogLevel.Name = "listBoxLogLevel";
			this.listBoxLogLevel.Size = new System.Drawing.Size(120, 69);
			this.listBoxLogLevel.TabIndex = 6;
			// 
			// labelLogLevel
			// 
			this.labelLogLevel.Location = new System.Drawing.Point(15, 182);
			this.labelLogLevel.Name = "labelLogLevel";
			this.labelLogLevel.Size = new System.Drawing.Size(135, 16);
			this.labelLogLevel.TabIndex = 0;
			this.labelLogLevel.Text = "Log Level";
			this.labelLogLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPass
			// 
			this.labelPass.Location = new System.Drawing.Point(15, 100);
			this.labelPass.Name = "labelPass";
			this.labelPass.Size = new System.Drawing.Size(135, 13);
			this.labelPass.TabIndex = 0;
			this.labelPass.Text = "Password";
			this.labelPass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butPing
			// 
			this.butPing.Location = new System.Drawing.Point(15, 149);
			this.butPing.Name = "butPing";
			this.butPing.Size = new System.Drawing.Size(97, 23);
			this.butPing.TabIndex = 5;
			this.butPing.Text = "Test Connection";
			this.butPing.UseVisualStyleBackColor = true;
			this.butPing.Click += new System.EventHandler(this.butPing_Click);
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(15, 114);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(164, 20);
			this.textPassword.TabIndex = 4;
			this.textPassword.UseSystemPasswordChar = true;
			// 
			// labelUser
			// 
			this.labelUser.Location = new System.Drawing.Point(15, 51);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(135, 16);
			this.labelUser.TabIndex = 0;
			this.labelUser.Text = "Username";
			this.labelUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(15, 68);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(164, 20);
			this.textUsername.TabIndex = 3;
			// 
			// radioProdEnv
			// 
			this.radioProdEnv.Location = new System.Drawing.Point(139, 29);
			this.radioProdEnv.Name = "radioProdEnv";
			this.radioProdEnv.Size = new System.Drawing.Size(103, 17);
			this.radioProdEnv.TabIndex = 2;
			this.radioProdEnv.TabStop = true;
			this.radioProdEnv.Text = "Production";
			this.radioProdEnv.UseVisualStyleBackColor = true;
			// 
			// radioTestEnv
			// 
			this.radioTestEnv.Location = new System.Drawing.Point(15, 29);
			this.radioTestEnv.Name = "radioTestEnv";
			this.radioTestEnv.Size = new System.Drawing.Size(104, 17);
			this.radioTestEnv.TabIndex = 1;
			this.radioTestEnv.TabStop = true;
			this.radioTestEnv.Text = "Test";
			this.radioTestEnv.UseVisualStyleBackColor = true;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(437, 645);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupOverrides
			// 
			this.groupOverrides.Controls.Add(this.textOverrides);
			this.groupOverrides.Controls.Add(this.labelOverrides);
			this.groupOverrides.Location = new System.Drawing.Point(12, 530);
			this.groupOverrides.Name = "groupOverrides";
			this.groupOverrides.Size = new System.Drawing.Size(581, 108);
			this.groupOverrides.TabIndex = 58;
			this.groupOverrides.TabStop = false;
			this.groupOverrides.Text = "Tax Overrides";
			// 
			// textOverrides
			// 
			this.textOverrides.Location = new System.Drawing.Point(8, 57);
			this.textOverrides.Multiline = true;
			this.textOverrides.Name = "textOverrides";
			this.textOverrides.Size = new System.Drawing.Size(566, 43);
			this.textOverrides.TabIndex = 1;
			// 
			// labelOverrides
			// 
			this.labelOverrides.Location = new System.Drawing.Point(6, 15);
			this.labelOverrides.Name = "labelOverrides";
			this.labelOverrides.Size = new System.Drawing.Size(552, 41);
			this.labelOverrides.TabIndex = 0;
			this.labelOverrides.Text = resources.GetString("labelOverrides.Text");
			// 
			// FormAvaTax
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(605, 681);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupOverrides);
			this.Controls.Add(this.groupProcCodes);
			this.Controls.Add(this.labelStatesTaxed);
			this.Controls.Add(this.listBoxTaxedStates);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.labelStatesNotTaxed);
			this.Controls.Add(this.listBoxNonTaxedStates);
			this.Controls.Add(this.groupOffice);
			this.Controls.Add(this.groupApiSettings);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAvaTax";
			this.Text = "Ava Tax Settings";
			this.Load += new System.EventHandler(this.FormAvaTax_Load);
			this.groupProcCodes.ResumeLayout(false);
			this.groupProcCodes.PerformLayout();
			this.groupOffice.ResumeLayout(false);
			this.groupOffice.PerformLayout();
			this.groupApiSettings.ResumeLayout(false);
			this.groupApiSettings.PerformLayout();
			this.groupOverrides.ResumeLayout(false);
			this.groupOverrides.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupOffice;
		private UI.Button butChooseTaxAdjType;
		private UI.ListBoxOD listBoxNonTaxedStates;
		private System.Windows.Forms.Label labelStatesNotTaxed;
		private System.Windows.Forms.RadioButton radioTestEnv;
		private System.Windows.Forms.RadioButton radioProdEnv;
		private System.Windows.Forms.TextBox textUsername;
		private System.Windows.Forms.Label labelUser;
		private System.Windows.Forms.GroupBox groupApiSettings;
		private System.Windows.Forms.Label labelPass;
		private UI.Button butPing;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label labelST;
		private System.Windows.Forms.TextBox textAdjType;
		private System.Windows.Forms.Label labelCompanyCode;
		private System.Windows.Forms.TextBox textCompanyCode;
		private System.Windows.Forms.Label labelLogLevel;
		private OpenDental.UI.ListBoxOD listBoxLogLevel;
		private System.Windows.Forms.CheckBox checkEnabled;
		private UI.Button butRight;
		private UI.Button butLeft;
		private UI.ListBoxOD listBoxTaxedStates;
		private System.Windows.Forms.Label labelStatesTaxed;
		private System.Windows.Forms.GroupBox groupProcCodes;
		private System.Windows.Forms.Label labelPrePayCodes;
		private System.Windows.Forms.TextBox textPrePayCodes;
		private System.Windows.Forms.Label labelTaxExempt;
		private System.Windows.Forms.TextBox textTaxExempt;
		private UI.Button butChooseTaxExempt;
		private System.Windows.Forms.TextBox textDiscountCodes;
		private System.Windows.Forms.Label labelDiscountCodes;
		private System.Windows.Forms.GroupBox groupOverrides;
		private System.Windows.Forms.TextBox textOverrides;
		private System.Windows.Forms.Label labelOverrides;
		private System.Windows.Forms.Label labelSalesTaxLockDate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textReturnAdjType;
		private UI.Button butChooseRetAdjType;
		private ValidDate validTaxLockDate;
	}
}