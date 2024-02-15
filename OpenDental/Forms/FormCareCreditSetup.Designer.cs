namespace OpenDental {
	partial class FormCareCreditSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCareCreditSetup));
			this.butSave = new OpenDental.UI.Button();
			this.checkEnabled = new OpenDental.UI.CheckBox();
			this.groupClinicSettings = new OpenDental.UI.GroupBox();
			this.labelQSBatch = new System.Windows.Forms.Label();
			this.listBoxQSOptions = new OpenDental.UI.ListBox();
			this.labelMerchantNumberClinic = new System.Windows.Forms.Label();
			this.textMerchantNumberClinic = new System.Windows.Forms.TextBox();
			this.groupQSBatch = new OpenDental.UI.GroupBox();
			this.comboBoxDaysOut = new OpenDental.UI.ComboBox();
			this.labelQSDays = new System.Windows.Forms.Label();
			this.comboPaymentType = new OpenDental.UI.ComboBox();
			this.labelPaymentType = new System.Windows.Forms.Label();
			this.groupPromotions = new OpenDental.UI.GroupBox();
			this.butPromotions = new OpenDental.UI.Button();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboPatFieldDef = new OpenDental.UI.ComboBox();
			this.labelPatFieldType = new System.Windows.Forms.Label();
			this.textMerchantNumberPractice = new System.Windows.Forms.TextBox();
			this.labelMerchantNumberPractice = new System.Windows.Forms.Label();
			this.checkMerchantNumByProv = new OpenDental.UI.CheckBox();
			this.checkHideAdvertising = new OpenDental.UI.CheckBox();
			this.labelMerchantClosedDescription = new System.Windows.Forms.Label();
			this.comboPatFieldPreApprvAmt = new OpenDental.UI.ComboBox();
			this.labelPreApprvAmt = new System.Windows.Forms.Label();
			this.comboPatFieldAvailableCredit = new OpenDental.UI.ComboBox();
			this.labelAvailableCredit = new System.Windows.Forms.Label();
			this.groupClinicSettings.SuspendLayout();
			this.groupQSBatch.SuspendLayout();
			this.groupPromotions.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(515, 495);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 7;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Location = new System.Drawing.Point(72, 20);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(118, 18);
			this.checkEnabled.TabIndex = 0;
			this.checkEnabled.Text = "Enabled";
			// 
			// groupClinicSettings
			// 
			this.groupClinicSettings.Controls.Add(this.labelQSBatch);
			this.groupClinicSettings.Controls.Add(this.listBoxQSOptions);
			this.groupClinicSettings.Controls.Add(this.labelMerchantNumberClinic);
			this.groupClinicSettings.Controls.Add(this.textMerchantNumberClinic);
			this.groupClinicSettings.Controls.Add(this.groupQSBatch);
			this.groupClinicSettings.Controls.Add(this.comboPaymentType);
			this.groupClinicSettings.Controls.Add(this.labelPaymentType);
			this.groupClinicSettings.Controls.Add(this.groupPromotions);
			this.groupClinicSettings.Location = new System.Drawing.Point(24, 240);
			this.groupClinicSettings.Name = "groupClinicSettings";
			this.groupClinicSettings.Size = new System.Drawing.Size(566, 238);
			this.groupClinicSettings.TabIndex = 6;
			this.groupClinicSettings.Text = "Clinic Settings";
			// 
			// labelQSBatch
			// 
			this.labelQSBatch.Location = new System.Drawing.Point(6, 139);
			this.labelQSBatch.Name = "labelQSBatch";
			this.labelQSBatch.Size = new System.Drawing.Size(160, 16);
			this.labelQSBatch.TabIndex = 276;
			this.labelQSBatch.Text = "Quickscreen Feature";
			this.labelQSBatch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBoxQSBatch
			// 
			this.listBoxQSOptions.Location = new System.Drawing.Point(9, 158);
			this.listBoxQSOptions.Name = "listBoxQSBatch";
			this.listBoxQSOptions.Size = new System.Drawing.Size(92, 64);
			this.listBoxQSOptions.TabIndex = 277;
			this.listBoxQSOptions.Text = "listQSBatch";
			this.listBoxQSOptions.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listQSBatch_SelectionChanged);
			// 
			// labelMerchantNumberClinic
			// 
			this.labelMerchantNumberClinic.Location = new System.Drawing.Point(6, 106);
			this.labelMerchantNumberClinic.Name = "labelMerchantNumberClinic";
			this.labelMerchantNumberClinic.Size = new System.Drawing.Size(138, 16);
			this.labelMerchantNumberClinic.TabIndex = 276;
			this.labelMerchantNumberClinic.Text = "Merchant Number";
			this.labelMerchantNumberClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMerchantNumberClinic
			// 
			this.textMerchantNumberClinic.Location = new System.Drawing.Point(146, 105);
			this.textMerchantNumberClinic.Name = "textMerchantNumberClinic";
			this.textMerchantNumberClinic.Size = new System.Drawing.Size(220, 20);
			this.textMerchantNumberClinic.TabIndex = 275;
			this.textMerchantNumberClinic.Leave += new System.EventHandler(this.textMerchantNumberClinic_Leave);
			// 
			// groupQSBatch
			// 
			this.groupQSBatch.Controls.Add(this.comboBoxDaysOut);
			this.groupQSBatch.Controls.Add(this.labelQSDays);
			this.groupQSBatch.Location = new System.Drawing.Point(104, 158);
			this.groupQSBatch.Name = "groupQSBatch";
			this.groupQSBatch.Size = new System.Drawing.Size(459, 64);
			this.groupQSBatch.TabIndex = 6;
			this.groupQSBatch.Text = "Quickscreen Settings";
			// 
			// comboBoxDaysOut
			// 
			this.comboBoxDaysOut.Location = new System.Drawing.Point(8, 24);
			this.comboBoxDaysOut.Name = "comboBoxDaysOut";
			this.comboBoxDaysOut.Size = new System.Drawing.Size(38, 21);
			this.comboBoxDaysOut.TabIndex = 0;
			this.comboBoxDaysOut.Text = "comboBoxPlus1";
			// 
			// labelBatchDays
			// 
			this.labelQSDays.Location = new System.Drawing.Point(52, 26);
			this.labelQSDays.Name = "labelQSDays";
			this.labelQSDays.Size = new System.Drawing.Size(404, 19);
			this.labelQSDays.TabIndex = 208;
			this.labelQSDays.Text = "Number of days out to check for pre-approvals for future appointments.";
			this.labelQSDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboPaymentType
			// 
			this.comboPaymentType.Location = new System.Drawing.Point(146, 17);
			this.comboPaymentType.Name = "comboPaymentType";
			this.comboPaymentType.Size = new System.Drawing.Size(220, 21);
			this.comboPaymentType.TabIndex = 0;
			this.comboPaymentType.Text = "comboPaymentType";
			// 
			// labelPaymentType
			// 
			this.labelPaymentType.Location = new System.Drawing.Point(53, 18);
			this.labelPaymentType.Name = "labelPaymentType";
			this.labelPaymentType.Size = new System.Drawing.Size(90, 16);
			this.labelPaymentType.TabIndex = 18;
			this.labelPaymentType.Text = "Payment Type";
			this.labelPaymentType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPromotions
			// 
			this.groupPromotions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupPromotions.Controls.Add(this.butPromotions);
			this.groupPromotions.Location = new System.Drawing.Point(146, 44);
			this.groupPromotions.Name = "groupPromotions";
			this.groupPromotions.Size = new System.Drawing.Size(220, 54);
			this.groupPromotions.TabIndex = 4;
			this.groupPromotions.Text = "Promotions Setup";
			// 
			// butPromotions
			// 
			this.butPromotions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPromotions.Location = new System.Drawing.Point(73, 19);
			this.butPromotions.Name = "butPromotions";
			this.butPromotions.Size = new System.Drawing.Size(75, 24);
			this.butPromotions.TabIndex = 0;
			this.butPromotions.Text = "Manage";
			this.butPromotions.Click += new System.EventHandler(this.butPromotions_Click);
			// 
			// comboClinics
			// 
			this.comboClinics.HqDescription = "Headquarters";
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(139, 211);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(251, 21);
			this.comboClinics.TabIndex = 2;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.comboClinics_SelectionChangeCommitted);
			// 
			// comboPatFieldDef
			// 
			this.comboPatFieldDef.Location = new System.Drawing.Point(177, 101);
			this.comboPatFieldDef.Name = "comboPatFieldDef";
			this.comboPatFieldDef.Size = new System.Drawing.Size(213, 21);
			this.comboPatFieldDef.TabIndex = 3;
			this.comboPatFieldDef.Text = "comboBoxPlus1";
			// 
			// labelPatFieldType
			// 
			this.labelPatFieldType.Location = new System.Drawing.Point(5, 102);
			this.labelPatFieldType.Name = "labelPatFieldType";
			this.labelPatFieldType.Size = new System.Drawing.Size(169, 16);
			this.labelPatFieldType.TabIndex = 270;
			this.labelPatFieldType.Text = "Pre-Approval Status Patient Field";
			this.labelPatFieldType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMerchantNumberPractice
			// 
			this.textMerchantNumberPractice.Location = new System.Drawing.Point(177, 73);
			this.textMerchantNumberPractice.Name = "textMerchantNumberPractice";
			this.textMerchantNumberPractice.Size = new System.Drawing.Size(213, 20);
			this.textMerchantNumberPractice.TabIndex = 5;
			this.textMerchantNumberPractice.Leave += new System.EventHandler(this.textMerchantNumberPractice_Leave);
			// 
			// labelMerchantNumberPractice
			// 
			this.labelMerchantNumberPractice.Location = new System.Drawing.Point(36, 74);
			this.labelMerchantNumberPractice.Name = "labelMerchantNumberPractice";
			this.labelMerchantNumberPractice.Size = new System.Drawing.Size(138, 16);
			this.labelMerchantNumberPractice.TabIndex = 274;
			this.labelMerchantNumberPractice.Text = "Merchant Number";
			this.labelMerchantNumberPractice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkMerchantNumByProv
			// 
			this.checkMerchantNumByProv.Location = new System.Drawing.Point(177, 185);
			this.checkMerchantNumByProv.Name = "checkMerchantNumByProv";
			this.checkMerchantNumByProv.Size = new System.Drawing.Size(213, 18);
			this.checkMerchantNumByProv.TabIndex = 4;
			this.checkMerchantNumByProv.Text = "Merchant number by provider";
			this.checkMerchantNumByProv.CheckedChanged += new System.EventHandler(this.checkMerchantNumByProv_CheckedChanged);
			// 
			// checkHideAdvertising
			// 
			this.checkHideAdvertising.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideAdvertising.Location = new System.Drawing.Point(72, 44);
			this.checkHideAdvertising.Name = "checkHideAdvertising";
			this.checkHideAdvertising.Size = new System.Drawing.Size(118, 18);
			this.checkHideAdvertising.TabIndex = 1;
			this.checkHideAdvertising.Text = "Hide advertising";
			// 
			// labelMerchantClosedDescription
			// 
			this.labelMerchantClosedDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelMerchantClosedDescription.ForeColor = System.Drawing.Color.Red;
			this.labelMerchantClosedDescription.Location = new System.Drawing.Point(21, 485);
			this.labelMerchantClosedDescription.Name = "labelMerchantClosedDescription";
			this.labelMerchantClosedDescription.Size = new System.Drawing.Size(392, 31);
			this.labelMerchantClosedDescription.TabIndex = 275;
			this.labelMerchantClosedDescription.Text = "The selected Merchant Number has been closed.\r\nPlease enter a new one or refer to" +
    " the manual for more information.";
			this.labelMerchantClosedDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboPatFieldPreApprvAmt
			// 
			this.comboPatFieldPreApprvAmt.Location = new System.Drawing.Point(177, 129);
			this.comboPatFieldPreApprvAmt.Name = "comboPatFieldPreApprvAmt";
			this.comboPatFieldPreApprvAmt.Size = new System.Drawing.Size(213, 21);
			this.comboPatFieldPreApprvAmt.TabIndex = 276;
			this.comboPatFieldPreApprvAmt.Text = "comboBoxPlus1";
			// 
			// labelPreApprvAmt
			// 
			this.labelPreApprvAmt.Location = new System.Drawing.Point(5, 130);
			this.labelPreApprvAmt.Name = "labelPreApprvAmt";
			this.labelPreApprvAmt.Size = new System.Drawing.Size(169, 16);
			this.labelPreApprvAmt.TabIndex = 277;
			this.labelPreApprvAmt.Text = "Pre-Approval Amount Patient Field";
			this.labelPreApprvAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPatFieldAvailableCredit
			// 
			this.comboPatFieldAvailableCredit.Location = new System.Drawing.Point(177, 157);
			this.comboPatFieldAvailableCredit.Name = "comboPatFieldAvailableCredit";
			this.comboPatFieldAvailableCredit.Size = new System.Drawing.Size(213, 21);
			this.comboPatFieldAvailableCredit.TabIndex = 278;
			this.comboPatFieldAvailableCredit.Text = "comboBoxPlus1";
			// 
			// labelAvailableCredit
			// 
			this.labelAvailableCredit.Location = new System.Drawing.Point(5, 158);
			this.labelAvailableCredit.Name = "labelAvailableCredit";
			this.labelAvailableCredit.Size = new System.Drawing.Size(169, 16);
			this.labelAvailableCredit.TabIndex = 279;
			this.labelAvailableCredit.Text = "Available Credit Patient Field";
			this.labelAvailableCredit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormCareCreditSetup
			// 
			this.ClientSize = new System.Drawing.Size(613, 531);
			this.Controls.Add(this.comboPatFieldAvailableCredit);
			this.Controls.Add(this.labelAvailableCredit);
			this.Controls.Add(this.comboPatFieldPreApprvAmt);
			this.Controls.Add(this.labelPreApprvAmt);
			this.Controls.Add(this.labelMerchantClosedDescription);
			this.Controls.Add(this.checkHideAdvertising);
			this.Controls.Add(this.checkMerchantNumByProv);
			this.Controls.Add(this.labelMerchantNumberPractice);
			this.Controls.Add(this.textMerchantNumberPractice);
			this.Controls.Add(this.comboPatFieldDef);
			this.Controls.Add(this.labelPatFieldType);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.groupClinicSettings);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCareCreditSetup";
			this.Text = "CareCredit Setup";
			this.Load += new System.EventHandler(this.FormCareCreditSetup_Load);
			this.groupClinicSettings.ResumeLayout(false);
			this.groupClinicSettings.PerformLayout();
			this.groupQSBatch.ResumeLayout(false);
			this.groupPromotions.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butSave;
		private OpenDental.UI.CheckBox checkEnabled;
		private OpenDental.UI.GroupBox groupClinicSettings;
		private UI.ComboBoxClinicPicker comboClinics;
		private System.Windows.Forms.Label labelPaymentType;
		private UI.ComboBox comboPaymentType;
		private OpenDental.UI.GroupBox groupPromotions;
		private UI.Button butPromotions;
		private OpenDental.UI.GroupBox groupQSBatch;
		private System.Windows.Forms.Label labelQSDays;
		private UI.ComboBox comboBoxDaysOut;
		private UI.ComboBox comboPatFieldDef;
		private System.Windows.Forms.Label labelPatFieldType;
		private System.Windows.Forms.TextBox textMerchantNumberPractice;
		private System.Windows.Forms.Label labelMerchantNumberPractice;
		private OpenDental.UI.CheckBox checkMerchantNumByProv;
		private OpenDental.UI.CheckBox checkHideAdvertising;
		private System.Windows.Forms.Label labelMerchantNumberClinic;
		private System.Windows.Forms.TextBox textMerchantNumberClinic;
		private System.Windows.Forms.Label labelMerchantClosedDescription;
		private UI.ComboBox comboPatFieldPreApprvAmt;
		private System.Windows.Forms.Label labelPreApprvAmt;
		private UI.ComboBox comboPatFieldAvailableCredit;
		private System.Windows.Forms.Label labelAvailableCredit;
		private UI.ListBox listBoxQSOptions;
		private System.Windows.Forms.Label labelQSBatch;
	}
}