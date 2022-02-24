namespace OpenDental {
	partial class FormQBAccountSelect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQBAccountSelect));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelDepositAccount = new System.Windows.Forms.Label();
			this.comboDepositAccount = new System.Windows.Forms.ComboBox();
			this.labelIncomeAccountQB = new System.Windows.Forms.Label();
			this.comboIncomeAccount = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(312, 61);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(312, 91);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelDepositAccount
			// 
			this.labelDepositAccount.Location = new System.Drawing.Point(12, 35);
			this.labelDepositAccount.Name = "labelDepositAccount";
			this.labelDepositAccount.Size = new System.Drawing.Size(289, 16);
			this.labelDepositAccount.TabIndex = 116;
			this.labelDepositAccount.Text = "Deposit into Account";
			this.labelDepositAccount.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboDepositAccount
			// 
			this.comboDepositAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDepositAccount.FormattingEnabled = true;
			this.comboDepositAccount.Location = new System.Drawing.Point(12, 52);
			this.comboDepositAccount.Name = "comboDepositAccount";
			this.comboDepositAccount.Size = new System.Drawing.Size(289, 21);
			this.comboDepositAccount.TabIndex = 0;
			// 
			// labelIncomeAccountQB
			// 
			this.labelIncomeAccountQB.Location = new System.Drawing.Point(12, 78);
			this.labelIncomeAccountQB.Name = "labelIncomeAccountQB";
			this.labelIncomeAccountQB.Size = new System.Drawing.Size(289, 11);
			this.labelIncomeAccountQB.TabIndex = 118;
			this.labelIncomeAccountQB.Text = "Income Account";
			this.labelIncomeAccountQB.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboIncomeAccountQB
			// 
			this.comboIncomeAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboIncomeAccount.FormattingEnabled = true;
			this.comboIncomeAccount.Location = new System.Drawing.Point(12, 93);
			this.comboIncomeAccount.Name = "comboIncomeAccountQB";
			this.comboIncomeAccount.Size = new System.Drawing.Size(289, 21);
			this.comboIncomeAccount.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(410, 16);
			this.label1.TabIndex = 119;
			this.label1.Text = "Select which accounts you want to use for this deposit";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormQBAccountSelect
			// 
			this.ClientSize = new System.Drawing.Size(399, 127);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelDepositAccount);
			this.Controls.Add(this.comboDepositAccount);
			this.Controls.Add(this.labelIncomeAccountQB);
			this.Controls.Add(this.comboIncomeAccount);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormQBAccountSelect";
			this.Text = "Select QuickBooks Accounts";
			this.Load += new System.EventHandler(this.FormQBAccountSelect_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelDepositAccount;
		private System.Windows.Forms.ComboBox comboDepositAccount;
		private System.Windows.Forms.Label labelIncomeAccountQB;
		private System.Windows.Forms.ComboBox comboIncomeAccount;
		private System.Windows.Forms.Label label1;
	}
}