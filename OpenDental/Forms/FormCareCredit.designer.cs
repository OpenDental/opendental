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
			this.comboProviders = new OpenDental.UI.ComboBoxOD();
			this.labelProviders = new System.Windows.Forms.Label();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.butApply = new OpenDental.UI.Button();
			this.butLookup = new OpenDental.UI.Button();
			this.butTransactions = new OpenDental.UI.Button();
			this.butReport = new OpenDental.UI.Button();
			this.butPromotions = new OpenDental.UI.Button();
			this.butQuickScreen = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(346, 242);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 8;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboProviders
			// 
			this.comboProviders.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.comboProviders.Location = new System.Drawing.Point(100, 48);
			this.comboProviders.Name = "comboProviders";
			this.comboProviders.Size = new System.Drawing.Size(214, 21);
			this.comboProviders.TabIndex = 1;
			this.comboProviders.Text = "Providers";
			// 
			// labelProviders
			// 
			this.labelProviders.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelProviders.Location = new System.Drawing.Point(5, 49);
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
			this.comboClinics.Location = new System.Drawing.Point(63, 20);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(251, 21);
			this.comboClinics.TabIndex = 0;
			// 
			// butApply
			// 
			this.butApply.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butApply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butApply.Location = new System.Drawing.Point(100, 92);
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
			this.butLookup.Location = new System.Drawing.Point(100, 133);
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
			this.butTransactions.Location = new System.Drawing.Point(27, 242);
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
			this.butReport.Location = new System.Drawing.Point(239, 92);
			this.butReport.Name = "butReport";
			this.butReport.Size = new System.Drawing.Size(75, 24);
			this.butReport.TabIndex = 5;
			this.butReport.Text = "Reports";
			this.butReport.Click += new System.EventHandler(this.butReport_Click);
			// 
			// butPromotions
			// 
			this.butPromotions.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butPromotions.Location = new System.Drawing.Point(239, 133);
			this.butPromotions.Name = "butPromotions";
			this.butPromotions.Size = new System.Drawing.Size(75, 24);
			this.butPromotions.TabIndex = 6;
			this.butPromotions.Text = "Manage";
			this.butPromotions.Click += new System.EventHandler(this.butPromotions_Click);
			// 
			// butQuickScreen
			// 
			this.butQuickScreen.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butQuickScreen.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butQuickScreen.Location = new System.Drawing.Point(100, 173);
			this.butQuickScreen.Name = "butQuickScreen";
			this.butQuickScreen.Size = new System.Drawing.Size(75, 24);
			this.butQuickScreen.TabIndex = 4;
			this.butQuickScreen.Text = "Quickscreen";
			this.butQuickScreen.Click += new System.EventHandler(this.butQuickScreen_Click);
			// 
			// FormCareCredit
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(433, 278);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butQuickScreen);
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
		private UI.ComboBoxOD comboProviders;
		private System.Windows.Forms.Label labelProviders;
		private UI.ComboBoxClinicPicker comboClinics;
		private UI.Button butApply;
		private UI.Button butLookup;
		private UI.Button butTransactions;
		private UI.Button butReport;
		private UI.Button butPromotions;
		private UI.Button butQuickScreen;
	}
}