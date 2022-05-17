namespace OpenDental{
	partial class FormAdvertisingPostcardsSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdvertisingPostcardsSetup));
			this.labelNotEnabled = new System.Windows.Forms.Label();
			this.gridAccounts = new OpenDental.UI.GridOD();
			this.gridClinics = new OpenDental.UI.GridOD();
			this.label2 = new System.Windows.Forms.Label();
			this.butManageAccount = new OpenDental.UI.Button();
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.labelClinicsGridHint = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelNotEnabled
			// 
			this.labelNotEnabled.ForeColor = System.Drawing.Color.Red;
			this.labelNotEnabled.Location = new System.Drawing.Point(125, 179);
			this.labelNotEnabled.Name = "labelNotEnabled";
			this.labelNotEnabled.Size = new System.Drawing.Size(269, 17);
			this.labelNotEnabled.TabIndex = 233;
			this.labelNotEnabled.Text = "* No accounts added, click here to signup";
			this.labelNotEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridAccounts
			// 
			this.gridAccounts.AllowSortingByColumn = true;
			this.gridAccounts.HasAutoWrappedHeaders = true;
			this.gridAccounts.HasMultilineHeaders = true;
			this.gridAccounts.Location = new System.Drawing.Point(12, 12);
			this.gridAccounts.Name = "gridAccounts";
			this.gridAccounts.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridAccounts.Size = new System.Drawing.Size(406, 157);
			this.gridAccounts.TabIndex = 142;
			this.gridAccounts.Title = "Postcards Accounts";
			this.gridAccounts.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAccounts_CellDoubleClick);
			this.gridAccounts.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAccounts_CellClick);
			// 
			// gridClinics
			// 
			this.gridClinics.AllowSortingByColumn = true;
			this.gridClinics.HasAutoWrappedHeaders = true;
			this.gridClinics.HasMultilineHeaders = true;
			this.gridClinics.Location = new System.Drawing.Point(12, 257);
			this.gridClinics.Name = "gridClinics";
			this.gridClinics.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridClinics.Size = new System.Drawing.Size(406, 157);
			this.gridClinics.TabIndex = 141;
			this.gridClinics.Title = "Clinic Accounts";
			this.gridClinics.CellSelectionCommitted += new OpenDental.UI.ODGridClickEventHandler(this.gridClinics_CellSelectionCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 202);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(409, 52);
			this.label2.TabIndex = 235;
			this.label2.Text = "To enable the \'Advertising - Postcards\' feature, create an account, and ensure a " +
    "default account is marked with \'X\' in the Postcards Accounts grid.";
			// 
			// butManageAccount
			// 
			this.butManageAccount.Location = new System.Drawing.Point(12, 175);
			this.butManageAccount.Name = "butManageAccount";
			this.butManageAccount.Size = new System.Drawing.Size(107, 24);
			this.butManageAccount.TabIndex = 8;
			this.butManageAccount.Text = "Add Account";
			this.butManageAccount.Click += new System.EventHandler(this.butManageAccount_Click);
			// 
			// webBrowser
			// 
			this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser.Location = new System.Drawing.Point(424, 6);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(794, 678);
			this.webBrowser.TabIndex = 138;
			// 
			// labelClinicsGridHint
			// 
			this.labelClinicsGridHint.Location = new System.Drawing.Point(9, 417);
			this.labelClinicsGridHint.Name = "labelClinicsGridHint";
			this.labelClinicsGridHint.Size = new System.Drawing.Size(409, 52);
			this.labelClinicsGridHint.TabIndex = 236;
			this.labelClinicsGridHint.Text = "Click into the \"Postcards Account\" column to select the account for a clinic. If " +
    "\"Default\" is selected, the clinic will use the account selected in the Accounts " +
    "grid.";
			// 
			// FormAdvertisingPostcardsSetup
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.labelClinicsGridHint);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelNotEnabled);
			this.Controls.Add(this.butManageAccount);
			this.Controls.Add(this.gridAccounts);
			this.Controls.Add(this.gridClinics);
			this.Controls.Add(this.webBrowser);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAdvertisingPostcardsSetup";
			this.Text = "Advertising - Postcards Setup";
			this.Load += new System.EventHandler(this.FormMassPostcardAdvertising_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridClinics;
		private System.Windows.Forms.WebBrowser webBrowser;
		private UI.Button butManageAccount;
		private UI.GridOD gridAccounts;
		private System.Windows.Forms.Label labelNotEnabled;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelClinicsGridHint;
	}
}