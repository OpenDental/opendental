namespace OpenDental {
	partial class FormCareCredit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCareCredit));
			this.butClose = new OpenDental.UI.Button();
			this.comboProviders = new OpenDental.UI.ComboBox();
			this.labelProviders = new System.Windows.Forms.Label();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.butApply = new OpenDental.UI.Button();
			this.butLookup = new OpenDental.UI.Button();
			this.butTransactions = new OpenDental.UI.Button();
			this.butReport = new OpenDental.UI.Button();
			this.butPromotions = new OpenDental.UI.Button();
			this.labelMerchantClosedDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(346, 261);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 8;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboProviders
			// 
			this.comboProviders.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.comboProviders.Location = new System.Drawing.Point(100, 57);
			this.comboProviders.Name = "comboProviders";
			this.comboProviders.Size = new System.Drawing.Size(214, 21);
			this.comboProviders.TabIndex = 1;
			this.comboProviders.Text = "Providers";
			this.comboProviders.SelectionChangeCommitted += new System.EventHandler(this.comboProviders_SelectionChangedCommitted);
			// 
			// labelProviders
			// 
			this.labelProviders.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelProviders.Location = new System.Drawing.Point(5, 58);
			this.labelProviders.Name = "labelProviders";
			this.labelProviders.Size = new System.Drawing.Size(93, 16);
			this.labelProviders.TabIndex = 273;
			this.labelProviders.Text = "Provider";
			this.labelProviders.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinics
			// 
			this.comboClinics.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.comboClinics.HqDescription = "Headquarters";
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(63, 29);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(251, 21);
			this.comboClinics.TabIndex = 0;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.comboClinics_SelectionChangeCommitted);
			// 
			// butApply
			// 
			this.butApply.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butApply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butApply.Location = new System.Drawing.Point(100, 101);
			this.butApply.Name = "butApply";
			this.butApply.Size = new System.Drawing.Size(75, 24);
			this.butApply.TabIndex = 2;
			this.butApply.Text = "Apply";
			this.butApply.Click += new System.EventHandler(this.butApply_Click);
			// 
			// butLookup
			// 
			this.butLookup.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butLookup.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butLookup.Location = new System.Drawing.Point(100, 142);
			this.butLookup.Name = "butLookup";
			this.butLookup.Size = new System.Drawing.Size(75, 24);
			this.butLookup.TabIndex = 3;
			this.butLookup.Text = "Lookup";
			this.butLookup.Click += new System.EventHandler(this.butLookup_Click);
			// 
			// butTransactions
			// 
			this.butTransactions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butTransactions.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butTransactions.Location = new System.Drawing.Point(27, 261);
			this.butTransactions.Name = "butTransactions";
			this.butTransactions.Size = new System.Drawing.Size(90, 24);
			this.butTransactions.TabIndex = 7;
			this.butTransactions.Text = "Transactions";
			this.butTransactions.Click += new System.EventHandler(this.butTransactions_Click);
			// 
			// butReport
			// 
			this.butReport.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butReport.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butReport.Location = new System.Drawing.Point(239, 101);
			this.butReport.Name = "butReport";
			this.butReport.Size = new System.Drawing.Size(75, 24);
			this.butReport.TabIndex = 5;
			this.butReport.Text = "Reports";
			this.butReport.Click += new System.EventHandler(this.butReport_Click);
			// 
			// butPromotions
			// 
			this.butPromotions.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butPromotions.Location = new System.Drawing.Point(239, 142);
			this.butPromotions.Name = "butPromotions";
			this.butPromotions.Size = new System.Drawing.Size(75, 24);
			this.butPromotions.TabIndex = 6;
			this.butPromotions.Text = "Manage";
			this.butPromotions.Click += new System.EventHandler(this.butPromotions_Click);
			// 
			// labelMerchantClosedDescription
			// 
			this.labelMerchantClosedDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelMerchantClosedDescription.ForeColor = System.Drawing.Color.Red;
			this.labelMerchantClosedDescription.Location = new System.Drawing.Point(69, 193);
			this.labelMerchantClosedDescription.Name = "labelMerchantClosedDescription";
			this.labelMerchantClosedDescription.Size = new System.Drawing.Size(352, 39);
			this.labelMerchantClosedDescription.TabIndex = 274;
			this.labelMerchantClosedDescription.Text = "The Merchant Number you are using has been closed.\r\nPlease refer to the manual on" +
    " how to change your settings.";
			this.labelMerchantClosedDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormCareCredit
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(433, 297);
			this.Controls.Add(this.labelMerchantClosedDescription);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butPromotions);
			this.Controls.Add(this.butLookup);
			this.Controls.Add(this.butApply);
			this.Controls.Add(this.butReport);
			this.Controls.Add(this.comboProviders);
			this.Controls.Add(this.butTransactions);
			this.Controls.Add(this.labelProviders);
			this.Controls.Add(this.comboClinics);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCareCredit";
			this.Text = "CareCredit";
			this.Load += new System.EventHandler(this.FormCareCredit_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.ComboBox comboProviders;
		private System.Windows.Forms.Label labelProviders;
		private UI.ComboBoxClinicPicker comboClinics;
		private UI.Button butApply;
		private UI.Button butLookup;
		private UI.Button butTransactions;
		private UI.Button butReport;
		private UI.Button butPromotions;
		private System.Windows.Forms.Label labelMerchantClosedDescription;
	}
}