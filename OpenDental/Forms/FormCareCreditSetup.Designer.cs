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
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.groupClinicSettings = new System.Windows.Forms.GroupBox();
			this.groupQSBatch = new System.Windows.Forms.GroupBox();
			this.comboBoxDaysOut = new OpenDental.UI.ComboBoxOD();
			this.labelBatchDays = new System.Windows.Forms.Label();
			this.comboPaymentType = new OpenDental.UI.ComboBoxOD();
			this.labelPaymentType = new System.Windows.Forms.Label();
			this.groupPromotions = new System.Windows.Forms.GroupBox();
			this.butPromotions = new OpenDental.UI.Button();
			this.checkQSBatch = new System.Windows.Forms.CheckBox();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboPatFieldDef = new OpenDental.UI.ComboBoxOD();
			this.labelPatFieldType = new System.Windows.Forms.Label();
			this.textMerchantNumberPractice = new System.Windows.Forms.TextBox();
			this.labelMerchantNumberPractice = new System.Windows.Forms.Label();
			this.checkMerchantNumByProv = new System.Windows.Forms.CheckBox();
			this.checkHideAdvertising = new System.Windows.Forms.CheckBox();
			this.labelMerchantNumberClinic = new System.Windows.Forms.Label();
			this.textMerchantNumberClinic = new System.Windows.Forms.TextBox();
			this.groupClinicSettings.SuspendLayout();
			this.groupQSBatch.SuspendLayout();
			this.groupPromotions.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(430, 437);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(515, 437);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Location = new System.Drawing.Point(72, 20);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(118, 18);
			this.checkEnabled.TabIndex = 0;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// groupClinicSettings
			// 
			this.groupClinicSettings.Controls.Add(this.labelMerchantNumberClinic);
			this.groupClinicSettings.Controls.Add(this.textMerchantNumberClinic);
			this.groupClinicSettings.Controls.Add(this.groupQSBatch);
			this.groupClinicSettings.Controls.Add(this.comboPaymentType);
			this.groupClinicSettings.Controls.Add(this.labelPaymentType);
			this.groupClinicSettings.Controls.Add(this.groupPromotions);
			this.groupClinicSettings.Controls.Add(this.checkQSBatch);
			this.groupClinicSettings.Location = new System.Drawing.Point(24, 185);
			this.groupClinicSettings.Name = "groupClinicSettings";
			this.groupClinicSettings.Size = new System.Drawing.Size(566, 238);
			this.groupClinicSettings.TabIndex = 6;
			this.groupClinicSettings.TabStop = false;
			this.groupClinicSettings.Text = "Clinic Settings";
			// 
			// groupQSBatch
			// 
			this.groupQSBatch.Controls.Add(this.comboBoxDaysOut);
			this.groupQSBatch.Controls.Add(this.labelBatchDays);
			this.groupQSBatch.Location = new System.Drawing.Point(12, 158);
			this.groupQSBatch.Name = "groupQSBatch";
			this.groupQSBatch.Size = new System.Drawing.Size(538, 64);
			this.groupQSBatch.TabIndex = 6;
			this.groupQSBatch.TabStop = false;
			this.groupQSBatch.Text = "Batch Quickscreen Settings";
			// 
			// comboBoxDaysOut
			// 
			this.comboBoxDaysOut.Location = new System.Drawing.Point(8, 24);
			this.comboBoxDaysOut.Name = "comboBoxDaysOut";
			this.comboBoxDaysOut.Size = new System.Drawing.Size(121, 21);
			this.comboBoxDaysOut.TabIndex = 0;
			this.comboBoxDaysOut.Text = "comboBoxPlus1";
			// 
			// labelBatchDays
			// 
			this.labelBatchDays.Location = new System.Drawing.Point(133, 26);
			this.labelBatchDays.Name = "labelBatchDays";
			this.labelBatchDays.Size = new System.Drawing.Size(400, 17);
			this.labelBatchDays.TabIndex = 208;
			this.labelBatchDays.Text = "Number of days out to check for pre-approvals for future appointments.";
			this.labelBatchDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.groupPromotions.TabStop = false;
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
			// checkQSBatch
			// 
			this.checkQSBatch.Location = new System.Drawing.Point(6, 136);
			this.checkQSBatch.Name = "checkQSBatch";
			this.checkQSBatch.Size = new System.Drawing.Size(296, 18);
			this.checkQSBatch.TabIndex = 5;
			this.checkQSBatch.Text = "Enable Batch Quickscreen processing";
			this.checkQSBatch.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.checkQSBatch.UseVisualStyleBackColor = true;
			this.checkQSBatch.CheckedChanged += new System.EventHandler(this.checkQSBatch_CheckedChanged);
			// 
			// comboClinics
			// 
			this.comboClinics.HqDescription = "Headquarters";
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(139, 156);
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
			this.checkMerchantNumByProv.Location = new System.Drawing.Point(177, 130);
			this.checkMerchantNumByProv.Name = "checkMerchantNumByProv";
			this.checkMerchantNumByProv.Size = new System.Drawing.Size(213, 18);
			this.checkMerchantNumByProv.TabIndex = 4;
			this.checkMerchantNumByProv.Text = "Merchant number by provider";
			this.checkMerchantNumByProv.UseVisualStyleBackColor = true;
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
			this.checkHideAdvertising.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideAdvertising.UseVisualStyleBackColor = true;
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
			// 
			// FormCareCreditSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(613, 480);
			this.Controls.Add(this.checkHideAdvertising);
			this.Controls.Add(this.checkMerchantNumByProv);
			this.Controls.Add(this.labelMerchantNumberPractice);
			this.Controls.Add(this.textMerchantNumberPractice);
			this.Controls.Add(this.comboPatFieldDef);
			this.Controls.Add(this.labelPatFieldType);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.groupClinicSettings);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
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

		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.GroupBox groupClinicSettings;
		private System.Windows.Forms.CheckBox checkQSBatch;
		private UI.ComboBoxClinicPicker comboClinics;
		private System.Windows.Forms.Label labelPaymentType;
		private UI.ComboBoxOD comboPaymentType;
		private System.Windows.Forms.GroupBox groupPromotions;
		private UI.Button butPromotions;
		private System.Windows.Forms.GroupBox groupQSBatch;
		private System.Windows.Forms.Label labelBatchDays;
		private UI.ComboBoxOD comboBoxDaysOut;
		private UI.ComboBoxOD comboPatFieldDef;
		private System.Windows.Forms.Label labelPatFieldType;
		private System.Windows.Forms.TextBox textMerchantNumberPractice;
		private System.Windows.Forms.Label labelMerchantNumberPractice;
		private System.Windows.Forms.CheckBox checkMerchantNumByProv;
		private System.Windows.Forms.CheckBox checkHideAdvertising;
		private System.Windows.Forms.Label labelMerchantNumberClinic;
		private System.Windows.Forms.TextBox textMerchantNumberClinic;
	}
}