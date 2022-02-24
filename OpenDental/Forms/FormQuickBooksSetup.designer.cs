namespace OpenDental{
	partial class FormQuickBooksSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormQuickBooksSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.buttonRemoveClassRefQB = new OpenDental.UI.Button();
			this.buttonAddClassRefQB = new OpenDental.UI.Button();
			this.listBoxClassRefsQB = new OpenDental.UI.ListBoxOD();
			this.labelClass = new System.Windows.Forms.Label();
			this.butRemoveIncomeQB = new OpenDental.UI.Button();
			this.checkQuickBooksClassRefsEnabled = new System.Windows.Forms.CheckBox();
			this.butAddIncomeQB = new OpenDental.UI.Button();
			this.listBoxIncomeAccountsQB = new OpenDental.UI.ListBoxOD();
			this.labelQuickBooksTitle = new System.Windows.Forms.Label();
			this.labelConnectQB = new System.Windows.Forms.Label();
			this.labelIncomeAccountQB = new System.Windows.Forms.Label();
			this.labelWarning = new System.Windows.Forms.Label();
			this.butConnectQB = new OpenDental.UI.Button();
			this.butBrowseQB = new OpenDental.UI.Button();
			this.labelCompanyFile = new System.Windows.Forms.Label();
			this.textCompanyFileQB = new System.Windows.Forms.TextBox();
			this.listBoxDepositAccountsQB = new OpenDental.UI.ListBoxOD();
			this.butRemoveDepositQB = new OpenDental.UI.Button();
			this.butAddDepositQB = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.labelDepositsQB = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(354, 603);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(435, 603);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// buttonRemoveClassRefQB
			// 
			this.buttonRemoveClassRefQB.Location = new System.Drawing.Point(435, 464);
			this.buttonRemoveClassRefQB.Name = "buttonRemoveClassRefQB";
			this.buttonRemoveClassRefQB.Size = new System.Drawing.Size(75, 24);
			this.buttonRemoveClassRefQB.TabIndex = 12;
			this.buttonRemoveClassRefQB.Text = "Remove";
			this.buttonRemoveClassRefQB.Visible = false;
			this.buttonRemoveClassRefQB.Click += new System.EventHandler(this.buttonRemoveClassRefQB_Click);
			// 
			// buttonAddClassRefQB
			// 
			this.buttonAddClassRefQB.Location = new System.Drawing.Point(435, 434);
			this.buttonAddClassRefQB.Name = "buttonAddClassRefQB";
			this.buttonAddClassRefQB.Size = new System.Drawing.Size(75, 24);
			this.buttonAddClassRefQB.TabIndex = 11;
			this.buttonAddClassRefQB.Text = "Add";
			this.buttonAddClassRefQB.Visible = false;
			this.buttonAddClassRefQB.Click += new System.EventHandler(this.buttonAddClassRefQB_Click);
			// 
			// listBoxClassRefsQB
			// 
			this.listBoxClassRefsQB.Location = new System.Drawing.Point(199, 434);
			this.listBoxClassRefsQB.Name = "listBoxClassRefsQB";
			this.listBoxClassRefsQB.Size = new System.Drawing.Size(230, 108);
			this.listBoxClassRefsQB.TabIndex = 10;
			this.listBoxClassRefsQB.Visible = false;
			// 
			// labelClass
			// 
			this.labelClass.Location = new System.Drawing.Point(20, 435);
			this.labelClass.Name = "labelClass";
			this.labelClass.Size = new System.Drawing.Size(177, 53);
			this.labelClass.TabIndex = 78;
			this.labelClass.Text = "Class List";
			this.labelClass.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelClass.Visible = false;
			// 
			// butRemoveIncomeQB
			// 
			this.butRemoveIncomeQB.Location = new System.Drawing.Point(435, 325);
			this.butRemoveIncomeQB.Name = "butRemoveIncomeQB";
			this.butRemoveIncomeQB.Size = new System.Drawing.Size(75, 24);
			this.butRemoveIncomeQB.TabIndex = 8;
			this.butRemoveIncomeQB.Text = "Remove";
			this.butRemoveIncomeQB.Click += new System.EventHandler(this.butRemoveIncomeQB_Click);
			// 
			// checkQuickBooksClassRefsEnabled
			// 
			this.checkQuickBooksClassRefsEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkQuickBooksClassRefsEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkQuickBooksClassRefsEnabled.Location = new System.Drawing.Point(171, 409);
			this.checkQuickBooksClassRefsEnabled.Name = "checkQuickBooksClassRefsEnabled";
			this.checkQuickBooksClassRefsEnabled.Size = new System.Drawing.Size(258, 19);
			this.checkQuickBooksClassRefsEnabled.TabIndex = 9;
			this.checkQuickBooksClassRefsEnabled.Text = "Enable QuickBooks Class Refs";
			this.checkQuickBooksClassRefsEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkQuickBooksClassRefsEnabled.CheckedChanged += new System.EventHandler(this.checkQuickBooksClassRefsEnabled_CheckedChanged);
			// 
			// butAddIncomeQB
			// 
			this.butAddIncomeQB.Location = new System.Drawing.Point(435, 295);
			this.butAddIncomeQB.Name = "butAddIncomeQB";
			this.butAddIncomeQB.Size = new System.Drawing.Size(75, 24);
			this.butAddIncomeQB.TabIndex = 7;
			this.butAddIncomeQB.Text = "Add";
			this.butAddIncomeQB.Click += new System.EventHandler(this.butAddIncomeQB_Click);
			// 
			// listBoxIncomeAccountsQB
			// 
			this.listBoxIncomeAccountsQB.Location = new System.Drawing.Point(199, 295);
			this.listBoxIncomeAccountsQB.Name = "listBoxIncomeAccountsQB";
			this.listBoxIncomeAccountsQB.Size = new System.Drawing.Size(230, 108);
			this.listBoxIncomeAccountsQB.TabIndex = 6;
			// 
			// labelQuickBooksTitle
			// 
			this.labelQuickBooksTitle.Location = new System.Drawing.Point(32, 9);
			this.labelQuickBooksTitle.Name = "labelQuickBooksTitle";
			this.labelQuickBooksTitle.Size = new System.Drawing.Size(478, 34);
			this.labelQuickBooksTitle.TabIndex = 74;
			this.labelQuickBooksTitle.Text = "QuickBooks and the QuickBooks Foundation Class must be installed on this computer" +
    ". \r\nGo to the QuickBooks page on our website to download the QBFC installer.";
			this.labelQuickBooksTitle.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelConnectQB
			// 
			this.labelConnectQB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelConnectQB.Location = new System.Drawing.Point(32, 71);
			this.labelConnectQB.Name = "labelConnectQB";
			this.labelConnectQB.Size = new System.Drawing.Size(397, 46);
			this.labelConnectQB.TabIndex = 73;
			this.labelConnectQB.Text = "Push the connect button with QuickBooks running in the background if this is your" +
    " first time using QuickBooks with Open Dental. (Only need to do this once)";
			this.labelConnectQB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelIncomeAccountQB
			// 
			this.labelIncomeAccountQB.Location = new System.Drawing.Point(20, 296);
			this.labelIncomeAccountQB.Name = "labelIncomeAccountQB";
			this.labelIncomeAccountQB.Size = new System.Drawing.Size(177, 53);
			this.labelIncomeAccountQB.TabIndex = 72;
			this.labelIncomeAccountQB.Text = "User will get to pick from this list of income accounts.";
			this.labelIncomeAccountQB.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelWarning
			// 
			this.labelWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWarning.Location = new System.Drawing.Point(52, 558);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(448, 27);
			this.labelWarning.TabIndex = 71;
			this.labelWarning.Text = "Open Dental will run faster if your QuickBooks company file is open in the backgr" +
    "ound.";
			this.labelWarning.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butConnectQB
			// 
			this.butConnectQB.Location = new System.Drawing.Point(435, 82);
			this.butConnectQB.Name = "butConnectQB";
			this.butConnectQB.Size = new System.Drawing.Size(75, 24);
			this.butConnectQB.TabIndex = 2;
			this.butConnectQB.Text = "Connect";
			this.butConnectQB.Click += new System.EventHandler(this.butConnectQB_Click);
			// 
			// butBrowseQB
			// 
			this.butBrowseQB.Location = new System.Drawing.Point(435, 46);
			this.butBrowseQB.Name = "butBrowseQB";
			this.butBrowseQB.Size = new System.Drawing.Size(75, 24);
			this.butBrowseQB.TabIndex = 1;
			this.butBrowseQB.Text = "Browse";
			this.butBrowseQB.Click += new System.EventHandler(this.butBrowseQB_Click);
			// 
			// labelCompanyFile
			// 
			this.labelCompanyFile.Location = new System.Drawing.Point(29, 48);
			this.labelCompanyFile.Name = "labelCompanyFile";
			this.labelCompanyFile.Size = new System.Drawing.Size(105, 19);
			this.labelCompanyFile.TabIndex = 67;
			this.labelCompanyFile.Text = "Company File";
			this.labelCompanyFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCompanyFileQB
			// 
			this.textCompanyFileQB.Location = new System.Drawing.Point(140, 48);
			this.textCompanyFileQB.Name = "textCompanyFileQB";
			this.textCompanyFileQB.Size = new System.Drawing.Size(289, 20);
			this.textCompanyFileQB.TabIndex = 0;
			// 
			// listBoxDepositAccountsQB
			// 
			this.listBoxDepositAccountsQB.Location = new System.Drawing.Point(199, 180);
			this.listBoxDepositAccountsQB.Name = "listBoxDepositAccountsQB";
			this.listBoxDepositAccountsQB.Size = new System.Drawing.Size(230, 108);
			this.listBoxDepositAccountsQB.TabIndex = 3;
			// 
			// butRemoveDepositQB
			// 
			this.butRemoveDepositQB.Location = new System.Drawing.Point(435, 209);
			this.butRemoveDepositQB.Name = "butRemoveDepositQB";
			this.butRemoveDepositQB.Size = new System.Drawing.Size(75, 24);
			this.butRemoveDepositQB.TabIndex = 5;
			this.butRemoveDepositQB.Text = "Remove";
			this.butRemoveDepositQB.Click += new System.EventHandler(this.butRemoveDepositQB_Click);
			// 
			// butAddDepositQB
			// 
			this.butAddDepositQB.Location = new System.Drawing.Point(435, 179);
			this.butAddDepositQB.Name = "butAddDepositQB";
			this.butAddDepositQB.Size = new System.Drawing.Size(75, 24);
			this.butAddDepositQB.TabIndex = 4;
			this.butAddDepositQB.Text = "Add";
			this.butAddDepositQB.Click += new System.EventHandler(this.butAddDepositQB_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(20, 180);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(177, 53);
			this.label7.TabIndex = 62;
			this.label7.Text = "User will get to pick from this list of accounts to deposit to.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelDepositsQB
			// 
			this.labelDepositsQB.Location = new System.Drawing.Point(18, 140);
			this.labelDepositsQB.Name = "labelDepositsQB";
			this.labelDepositsQB.Size = new System.Drawing.Size(492, 27);
			this.labelDepositsQB.TabIndex = 61;
			this.labelDepositsQB.Text = "Every time a deposit is created, a deposit will be created within QuickBooks usin" +
    "g these settings.\r\n(Commas must be removed from account names within QuickBooks)" +
    "";
			this.labelDepositsQB.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormQuickBooksSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(528, 639);
			this.Controls.Add(this.buttonRemoveClassRefQB);
			this.Controls.Add(this.buttonAddClassRefQB);
			this.Controls.Add(this.listBoxClassRefsQB);
			this.Controls.Add(this.labelClass);
			this.Controls.Add(this.butRemoveIncomeQB);
			this.Controls.Add(this.checkQuickBooksClassRefsEnabled);
			this.Controls.Add(this.butAddIncomeQB);
			this.Controls.Add(this.listBoxIncomeAccountsQB);
			this.Controls.Add(this.labelQuickBooksTitle);
			this.Controls.Add(this.labelConnectQB);
			this.Controls.Add(this.labelIncomeAccountQB);
			this.Controls.Add(this.labelWarning);
			this.Controls.Add(this.butConnectQB);
			this.Controls.Add(this.butBrowseQB);
			this.Controls.Add(this.labelCompanyFile);
			this.Controls.Add(this.textCompanyFileQB);
			this.Controls.Add(this.listBoxDepositAccountsQB);
			this.Controls.Add(this.butRemoveDepositQB);
			this.Controls.Add(this.butAddDepositQB);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.labelDepositsQB);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormQuickBooksSetup";
			this.Text = "Setup QuickBooks";
			this.Load += new System.EventHandler(this.FormQuickBooksSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button buttonRemoveClassRefQB;
		private UI.Button buttonAddClassRefQB;
		private OpenDental.UI.ListBoxOD listBoxClassRefsQB;
		private System.Windows.Forms.Label labelClass;
		private UI.Button butRemoveIncomeQB;
		private System.Windows.Forms.CheckBox checkQuickBooksClassRefsEnabled;
		private UI.Button butAddIncomeQB;
		private OpenDental.UI.ListBoxOD listBoxIncomeAccountsQB;
		private System.Windows.Forms.Label labelQuickBooksTitle;
		private System.Windows.Forms.Label labelConnectQB;
		private System.Windows.Forms.Label labelIncomeAccountQB;
		private System.Windows.Forms.Label labelWarning;
		private UI.Button butConnectQB;
		private UI.Button butBrowseQB;
		private System.Windows.Forms.Label labelCompanyFile;
		private System.Windows.Forms.TextBox textCompanyFileQB;
		private OpenDental.UI.ListBoxOD listBoxDepositAccountsQB;
		private UI.Button butRemoveDepositQB;
		private UI.Button butAddDepositQB;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelDepositsQB;
	}
}