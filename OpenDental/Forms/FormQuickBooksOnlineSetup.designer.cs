namespace OpenDental{
	partial class FormQuickBooksOnlineSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQuickBooksOnlineSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.butAuthenticate = new OpenDental.UI.Button();
			this.labelClassesInOD = new System.Windows.Forms.Label();
			this.labelIncomeAccountsInOD = new System.Windows.Forms.Label();
			this.labelDepositAccountsInOD = new System.Windows.Forms.Label();
			this.groupEnable = new System.Windows.Forms.GroupBox();
			this.textAccessToken = new System.Windows.Forms.TextBox();
			this.labelAccessToken = new System.Windows.Forms.Label();
			this.textRefreshToken = new System.Windows.Forms.TextBox();
			this.labelRefreshToken = new System.Windows.Forms.Label();
			this.labelAuthenticate = new System.Windows.Forms.Label();
			this.labelDeposits = new System.Windows.Forms.Label();
			this.butAddDepositAccount = new OpenDental.UI.Button();
			this.butRemoveDepositAccount = new OpenDental.UI.Button();
			this.labelDepositAccountsAvailable = new System.Windows.Forms.Label();
			this.labelIncomeAccountsAvailable = new System.Windows.Forms.Label();
			this.labelClassesAvailable = new System.Windows.Forms.Label();
			this.butRemoveIncomeAccount = new OpenDental.UI.Button();
			this.butAddIncomeAccount = new OpenDental.UI.Button();
			this.butRemoveClass = new OpenDental.UI.Button();
			this.butAddClass = new OpenDental.UI.Button();
			this.listBoxDepositAccountsInOD = new OpenDental.UI.ListBoxOD();
			this.listBoxDepositAccountsAvailable = new OpenDental.UI.ListBoxOD();
			this.listBoxIncomeAccountsInOD = new OpenDental.UI.ListBoxOD();
			this.listBoxIncomeAccountsAvailable = new OpenDental.UI.ListBoxOD();
			this.listBoxClassRefsInOD = new OpenDental.UI.ListBoxOD();
			this.listBoxClassRefsAvailable = new OpenDental.UI.ListBoxOD();
			this.groupEnable.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(631, 573);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 14;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(712, 573);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 15;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnabled.Location = new System.Drawing.Point(268, 16);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(82, 18);
			this.checkEnabled.TabIndex = 0;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Click += new System.EventHandler(this.checkEnabled_Click);
			// 
			// butAuthenticate
			// 
			this.butAuthenticate.Location = new System.Drawing.Point(265, 44);
			this.butAuthenticate.Name = "butAuthenticate";
			this.butAuthenticate.Size = new System.Drawing.Size(85, 26);
			this.butAuthenticate.TabIndex = 0;
			this.butAuthenticate.Text = "Authenticate";
			this.butAuthenticate.Click += new System.EventHandler(this.butAuthenticate_Click);
			// 
			// labelClassesInOD
			// 
			this.labelClassesInOD.Location = new System.Drawing.Point(10, 449);
			this.labelClassesInOD.Name = "labelClassesInOD";
			this.labelClassesInOD.Size = new System.Drawing.Size(141, 71);
			this.labelClassesInOD.TabIndex = 85;
			this.labelClassesInOD.Text = "User will get to pick from this list of classes.";
			this.labelClassesInOD.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelIncomeAccountsInOD
			// 
			this.labelIncomeAccountsInOD.Location = new System.Drawing.Point(7, 325);
			this.labelIncomeAccountsInOD.Name = "labelIncomeAccountsInOD";
			this.labelIncomeAccountsInOD.Size = new System.Drawing.Size(144, 68);
			this.labelIncomeAccountsInOD.TabIndex = 83;
			this.labelIncomeAccountsInOD.Text = "User will get to pick from this list of income accounts.";
			this.labelIncomeAccountsInOD.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelDepositAccountsInOD
			// 
			this.labelDepositAccountsInOD.Location = new System.Drawing.Point(4, 201);
			this.labelDepositAccountsInOD.Name = "labelDepositAccountsInOD";
			this.labelDepositAccountsInOD.Size = new System.Drawing.Size(147, 70);
			this.labelDepositAccountsInOD.TabIndex = 80;
			this.labelDepositAccountsInOD.Text = "User will get to pick from this list of accounts to deposit to.";
			this.labelDepositAccountsInOD.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupEnable
			// 
			this.groupEnable.Controls.Add(this.textAccessToken);
			this.groupEnable.Controls.Add(this.labelAccessToken);
			this.groupEnable.Controls.Add(this.textRefreshToken);
			this.groupEnable.Controls.Add(this.labelRefreshToken);
			this.groupEnable.Controls.Add(this.labelAuthenticate);
			this.groupEnable.Controls.Add(this.butAuthenticate);
			this.groupEnable.Controls.Add(this.checkEnabled);
			this.groupEnable.Location = new System.Drawing.Point(222, 6);
			this.groupEnable.Name = "groupEnable";
			this.groupEnable.Size = new System.Drawing.Size(370, 143);
			this.groupEnable.TabIndex = 1;
			this.groupEnable.TabStop = false;
			this.groupEnable.Text = "Enable";
			// 
			// textAccessToken
			// 
			this.textAccessToken.Location = new System.Drawing.Point(97, 86);
			this.textAccessToken.Name = "textAccessToken";
			this.textAccessToken.ReadOnly = true;
			this.textAccessToken.Size = new System.Drawing.Size(253, 20);
			this.textAccessToken.TabIndex = 77;
			// 
			// labelAccessToken
			// 
			this.labelAccessToken.Location = new System.Drawing.Point(7, 85);
			this.labelAccessToken.Name = "labelAccessToken";
			this.labelAccessToken.Size = new System.Drawing.Size(89, 20);
			this.labelAccessToken.TabIndex = 79;
			this.labelAccessToken.Text = "Access Token";
			this.labelAccessToken.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRefreshToken
			// 
			this.textRefreshToken.Location = new System.Drawing.Point(97, 112);
			this.textRefreshToken.Name = "textRefreshToken";
			this.textRefreshToken.ReadOnly = true;
			this.textRefreshToken.Size = new System.Drawing.Size(253, 20);
			this.textRefreshToken.TabIndex = 78;
			// 
			// labelRefreshToken
			// 
			this.labelRefreshToken.Location = new System.Drawing.Point(7, 111);
			this.labelRefreshToken.Name = "labelRefreshToken";
			this.labelRefreshToken.Size = new System.Drawing.Size(89, 20);
			this.labelRefreshToken.TabIndex = 80;
			this.labelRefreshToken.Text = "Refresh Token";
			this.labelRefreshToken.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAuthenticate
			// 
			this.labelAuthenticate.Location = new System.Drawing.Point(4, 43);
			this.labelAuthenticate.Name = "labelAuthenticate";
			this.labelAuthenticate.Size = new System.Drawing.Size(258, 40);
			this.labelAuthenticate.TabIndex = 76;
			this.labelAuthenticate.Text = "Click to log in to your QuickBooks Online account and receive an Authorization Co" +
    "de and Realm ID.";
			this.labelAuthenticate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelDeposits
			// 
			this.labelDeposits.Location = new System.Drawing.Point(24, 157);
			this.labelDeposits.Name = "labelDeposits";
			this.labelDeposits.Size = new System.Drawing.Size(767, 41);
			this.labelDeposits.TabIndex = 92;
			this.labelDeposits.Text = resources.GetString("labelDeposits.Text");
			// 
			// butAddDepositAccount
			// 
			this.butAddDepositAccount.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butAddDepositAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddDepositAccount.Image = global::OpenDental.Properties.Resources.Left;
			this.butAddDepositAccount.Location = new System.Drawing.Point(390, 223);
			this.butAddDepositAccount.Name = "butAddDepositAccount";
			this.butAddDepositAccount.Size = new System.Drawing.Size(35, 24);
			this.butAddDepositAccount.TabIndex = 3;
			this.butAddDepositAccount.Click += new System.EventHandler(this.butAddDepositAccount_Click);
			// 
			// butRemoveDepositAccount
			// 
			this.butRemoveDepositAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemoveDepositAccount.Image = global::OpenDental.Properties.Resources.Right;
			this.butRemoveDepositAccount.Location = new System.Drawing.Point(390, 263);
			this.butRemoveDepositAccount.Name = "butRemoveDepositAccount";
			this.butRemoveDepositAccount.Size = new System.Drawing.Size(35, 24);
			this.butRemoveDepositAccount.TabIndex = 4;
			this.butRemoveDepositAccount.Click += new System.EventHandler(this.butRemoveDepositAccount_Click);
			// 
			// labelDepositAccountsAvailable
			// 
			this.labelDepositAccountsAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDepositAccountsAvailable.Location = new System.Drawing.Point(664, 201);
			this.labelDepositAccountsAvailable.Name = "labelDepositAccountsAvailable";
			this.labelDepositAccountsAvailable.Size = new System.Drawing.Size(136, 70);
			this.labelDepositAccountsAvailable.TabIndex = 98;
			this.labelDepositAccountsAvailable.Text = "Deposit accounts available in QuickBooks Online.";
			// 
			// labelIncomeAccountsAvailable
			// 
			this.labelIncomeAccountsAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelIncomeAccountsAvailable.Location = new System.Drawing.Point(664, 325);
			this.labelIncomeAccountsAvailable.Name = "labelIncomeAccountsAvailable";
			this.labelIncomeAccountsAvailable.Size = new System.Drawing.Size(136, 68);
			this.labelIncomeAccountsAvailable.TabIndex = 99;
			this.labelIncomeAccountsAvailable.Text = "Income accounts available in QuickBooks Online.";
			// 
			// labelClassesAvailable
			// 
			this.labelClassesAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClassesAvailable.Location = new System.Drawing.Point(664, 449);
			this.labelClassesAvailable.Name = "labelClassesAvailable";
			this.labelClassesAvailable.Size = new System.Drawing.Size(136, 71);
			this.labelClassesAvailable.TabIndex = 100;
			this.labelClassesAvailable.Text = "Classes available in QuickBooks Online.";
			// 
			// butRemoveIncomeAccount
			// 
			this.butRemoveIncomeAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemoveIncomeAccount.Image = global::OpenDental.Properties.Resources.Right;
			this.butRemoveIncomeAccount.Location = new System.Drawing.Point(390, 387);
			this.butRemoveIncomeAccount.Name = "butRemoveIncomeAccount";
			this.butRemoveIncomeAccount.Size = new System.Drawing.Size(35, 24);
			this.butRemoveIncomeAccount.TabIndex = 8;
			this.butRemoveIncomeAccount.Click += new System.EventHandler(this.butRemoveIncomeAccount_Click);
			// 
			// butAddIncomeAccount
			// 
			this.butAddIncomeAccount.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butAddIncomeAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddIncomeAccount.Image = global::OpenDental.Properties.Resources.Left;
			this.butAddIncomeAccount.Location = new System.Drawing.Point(390, 347);
			this.butAddIncomeAccount.Name = "butAddIncomeAccount";
			this.butAddIncomeAccount.Size = new System.Drawing.Size(35, 24);
			this.butAddIncomeAccount.TabIndex = 7;
			this.butAddIncomeAccount.Click += new System.EventHandler(this.butAddIncomeAccount_Click);
			// 
			// butRemoveClass
			// 
			this.butRemoveClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemoveClass.Image = global::OpenDental.Properties.Resources.Right;
			this.butRemoveClass.Location = new System.Drawing.Point(390, 511);
			this.butRemoveClass.Name = "butRemoveClass";
			this.butRemoveClass.Size = new System.Drawing.Size(35, 24);
			this.butRemoveClass.TabIndex = 12;
			this.butRemoveClass.Click += new System.EventHandler(this.butRemoveClass_Click);
			// 
			// butAddClass
			// 
			this.butAddClass.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butAddClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddClass.Image = global::OpenDental.Properties.Resources.Left;
			this.butAddClass.Location = new System.Drawing.Point(390, 471);
			this.butAddClass.Name = "butAddClass";
			this.butAddClass.Size = new System.Drawing.Size(35, 24);
			this.butAddClass.TabIndex = 11;
			this.butAddClass.Click += new System.EventHandler(this.butAddClass_Click);
			// 
			// listBoxDepositAccountsInOD
			// 
			this.listBoxDepositAccountsInOD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxDepositAccountsInOD.Location = new System.Drawing.Point(152, 201);
			this.listBoxDepositAccountsInOD.Name = "listBoxDepositAccountsInOD";
			this.listBoxDepositAccountsInOD.Size = new System.Drawing.Size(230, 108);
			this.listBoxDepositAccountsInOD.TabIndex = 2;
			// 
			// listBoxDepositAccountsAvailable
			// 
			this.listBoxDepositAccountsAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxDepositAccountsAvailable.Location = new System.Drawing.Point(433, 201);
			this.listBoxDepositAccountsAvailable.Name = "listBoxDepositAccountsAvailable";
			this.listBoxDepositAccountsAvailable.Size = new System.Drawing.Size(230, 108);
			this.listBoxDepositAccountsAvailable.TabIndex = 5;
			// 
			// listBoxIncomeAccountsInOD
			// 
			this.listBoxIncomeAccountsInOD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxIncomeAccountsInOD.Location = new System.Drawing.Point(152, 325);
			this.listBoxIncomeAccountsInOD.Name = "listBoxIncomeAccountsInOD";
			this.listBoxIncomeAccountsInOD.Size = new System.Drawing.Size(230, 108);
			this.listBoxIncomeAccountsInOD.TabIndex = 6;
			// 
			// listBoxIncomeAccountsAvailable
			// 
			this.listBoxIncomeAccountsAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxIncomeAccountsAvailable.Location = new System.Drawing.Point(433, 325);
			this.listBoxIncomeAccountsAvailable.Name = "listBoxIncomeAccountsAvailable";
			this.listBoxIncomeAccountsAvailable.Size = new System.Drawing.Size(230, 108);
			this.listBoxIncomeAccountsAvailable.TabIndex = 9;
			// 
			// listBoxClassRefsInOD
			// 
			this.listBoxClassRefsInOD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxClassRefsInOD.Location = new System.Drawing.Point(152, 449);
			this.listBoxClassRefsInOD.Name = "listBoxClassRefsInOD";
			this.listBoxClassRefsInOD.Size = new System.Drawing.Size(230, 108);
			this.listBoxClassRefsInOD.TabIndex = 10;
			// 
			// listBoxClassRefsAvailable
			// 
			this.listBoxClassRefsAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxClassRefsAvailable.Location = new System.Drawing.Point(433, 449);
			this.listBoxClassRefsAvailable.Name = "listBoxClassRefsAvailable";
			this.listBoxClassRefsAvailable.Size = new System.Drawing.Size(230, 108);
			this.listBoxClassRefsAvailable.TabIndex = 13;
			// 
			// FormQuickBooksOnlineSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(814, 609);
			this.Controls.Add(this.listBoxClassRefsAvailable);
			this.Controls.Add(this.listBoxClassRefsInOD);
			this.Controls.Add(this.listBoxIncomeAccountsAvailable);
			this.Controls.Add(this.listBoxIncomeAccountsInOD);
			this.Controls.Add(this.listBoxDepositAccountsAvailable);
			this.Controls.Add(this.listBoxDepositAccountsInOD);
			this.Controls.Add(this.butRemoveClass);
			this.Controls.Add(this.butAddClass);
			this.Controls.Add(this.butRemoveIncomeAccount);
			this.Controls.Add(this.butAddIncomeAccount);
			this.Controls.Add(this.labelClassesAvailable);
			this.Controls.Add(this.labelIncomeAccountsAvailable);
			this.Controls.Add(this.labelDepositAccountsAvailable);
			this.Controls.Add(this.butRemoveDepositAccount);
			this.Controls.Add(this.butAddDepositAccount);
			this.Controls.Add(this.labelDeposits);
			this.Controls.Add(this.groupEnable);
			this.Controls.Add(this.labelClassesInOD);
			this.Controls.Add(this.labelIncomeAccountsInOD);
			this.Controls.Add(this.labelDepositAccountsInOD);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormQuickBooksOnlineSetup";
			this.Text = "QuickBooks Online Setup";
			this.Load += new System.EventHandler(this.FormQuickBooksOnlineSetup_Load);
			this.groupEnable.ResumeLayout(false);
			this.groupEnable.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkEnabled;
		private UI.Button butAuthenticate;
		private System.Windows.Forms.Label labelClassesInOD;
		private System.Windows.Forms.Label labelIncomeAccountsInOD;
		private System.Windows.Forms.Label labelDepositAccountsInOD;
		private System.Windows.Forms.GroupBox groupEnable;
		private System.Windows.Forms.Label labelAuthenticate;
		private System.Windows.Forms.Label labelDeposits;
		private System.Windows.Forms.TextBox textAccessToken;
		private System.Windows.Forms.Label labelAccessToken;
		private System.Windows.Forms.TextBox textRefreshToken;
		private System.Windows.Forms.Label labelRefreshToken;
		private UI.Button butAddDepositAccount;
		private UI.Button butRemoveDepositAccount;
		private System.Windows.Forms.Label labelDepositAccountsAvailable;
		private System.Windows.Forms.Label labelIncomeAccountsAvailable;
		private System.Windows.Forms.Label labelClassesAvailable;
		private UI.Button butRemoveIncomeAccount;
		private UI.Button butAddIncomeAccount;
		private UI.Button butRemoveClass;
		private UI.Button butAddClass;
		private UI.ListBoxOD listBoxDepositAccountsInOD;
		private UI.ListBoxOD listBoxDepositAccountsAvailable;
		private UI.ListBoxOD listBoxIncomeAccountsInOD;
		private UI.ListBoxOD listBoxIncomeAccountsAvailable;
		private UI.ListBoxOD listBoxClassRefsInOD;
		private UI.ListBoxOD listBoxClassRefsAvailable;
	}
}