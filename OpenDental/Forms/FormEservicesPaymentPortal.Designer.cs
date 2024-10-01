namespace OpenDental{
	partial class FormEServicesPaymentPortal {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesPaymentPortal));
			this.groupBoxPaymentPortal = new OpenDental.UI.GroupBox();
			this.labelPortalExplanation = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textPatientFacingPaymentUrl = new System.Windows.Forms.TextBox();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.labelTags = new System.Windows.Forms.Label();
			this.comboClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.butSave = new OpenDental.UI.Button();
			this.groupBoxMessageToPay = new OpenDental.UI.GroupBox();
			this.groupBoxUserControlFill = new OpenDental.UI.GroupBox();
			this.groupBoxPaymentPortal.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBoxMessageToPay.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxPaymentPortal
			// 
			this.groupBoxPaymentPortal.Controls.Add(this.labelPortalExplanation);
			this.groupBoxPaymentPortal.Controls.Add(this.label8);
			this.groupBoxPaymentPortal.Controls.Add(this.textPatientFacingPaymentUrl);
			this.groupBoxPaymentPortal.Location = new System.Drawing.Point(12, 40);
			this.groupBoxPaymentPortal.Name = "groupBoxPaymentPortal";
			this.groupBoxPaymentPortal.Size = new System.Drawing.Size(538, 82);
			this.groupBoxPaymentPortal.TabIndex = 0;
			this.groupBoxPaymentPortal.Text = "Payment Portal";
			// 
			// labelPortalExplanation
			// 
			this.labelPortalExplanation.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelPortalExplanation.Location = new System.Drawing.Point(15, 24);
			this.labelPortalExplanation.Name = "labelPortalExplanation";
			this.labelPortalExplanation.Size = new System.Drawing.Size(504, 18);
			this.labelPortalExplanation.TabIndex = 0;
			this.labelPortalExplanation.Text = "This is the link that patients will use to reach your office\'s Payment Portal.";
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label8.Location = new System.Drawing.Point(-143, 129);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(129, 17);
			this.label8.TabIndex = 52;
			this.label8.Text = "Patient Facing URL";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientFacingPaymentUrl
			// 
			this.textPatientFacingPaymentUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPatientFacingPaymentUrl.Location = new System.Drawing.Point(19, 45);
			this.textPatientFacingPaymentUrl.Name = "textPatientFacingPaymentUrl";
			this.textPatientFacingPaymentUrl.ReadOnly = true;
			this.textPatientFacingPaymentUrl.Size = new System.Drawing.Size(498, 20);
			this.textPatientFacingPaymentUrl.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.labelTags);
			this.groupBox2.Location = new System.Drawing.Point(18, 342);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(501, 77);
			this.groupBox2.TabIndex = 116;
			this.groupBox2.Text = "Template Replacement Tags";
			// 
			// labelTags
			// 
			this.labelTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTags.Location = new System.Drawing.Point(3, 16);
			this.labelTags.Name = "labelTags";
			this.labelTags.Size = new System.Drawing.Size(495, 60);
			this.labelTags.TabIndex = 110;
			this.labelTags.Text = "Use template tags to create dynamic messages.";
			// 
			// comboClinicPicker
			// 
			this.comboClinicPicker.HqDescription = "None";
			this.comboClinicPicker.IncludeUnassigned = true;
			this.comboClinicPicker.Location = new System.Drawing.Point(349, 13);
			this.comboClinicPicker.Name = "comboClinicPicker";
			this.comboClinicPicker.Size = new System.Drawing.Size(200, 21);
			this.comboClinicPicker.TabIndex = 1;
			this.comboClinicPicker.SelectedIndexChanged += new System.EventHandler(this.comboClinicPicker_SelectedIndexChanged);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(475, 572);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 2;
			this.butSave.Text = "Save";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// groupBoxMessageToPay
			// 
			this.groupBoxMessageToPay.Controls.Add(this.groupBoxUserControlFill);
			this.groupBoxMessageToPay.Controls.Add(this.groupBox2);
			this.groupBoxMessageToPay.Location = new System.Drawing.Point(12, 128);
			this.groupBoxMessageToPay.Name = "groupBoxMessageToPay";
			this.groupBoxMessageToPay.Size = new System.Drawing.Size(538, 430);
			this.groupBoxMessageToPay.TabIndex = 53;
			this.groupBoxMessageToPay.Text = "Message-to-Pay Template (Same for all clinics)";
			// 
			// groupBoxUserControlFill
			// 
			this.groupBoxUserControlFill.Location = new System.Drawing.Point(18, 21);
			this.groupBoxUserControlFill.Name = "groupBoxUserControlFill";
			this.groupBoxUserControlFill.Size = new System.Drawing.Size(498, 315);
			this.groupBoxUserControlFill.TabIndex = 117;
			this.groupBoxUserControlFill.Text = "";
			// 
			// FormEServicesPaymentPortal
			// 
			this.ClientSize = new System.Drawing.Size(562, 608);
			this.Controls.Add(this.groupBoxMessageToPay);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.comboClinicPicker);
			this.Controls.Add(this.groupBoxPaymentPortal);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesPaymentPortal";
			this.Text = "eService Payment Portal";
			this.Load += new System.EventHandler(this.FormEServicesPaymentPortal_Load);
			this.groupBoxPaymentPortal.ResumeLayout(false);
			this.groupBoxPaymentPortal.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBoxMessageToPay.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GroupBox groupBoxPaymentPortal;
		private System.Windows.Forms.Label labelPortalExplanation;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textPatientFacingPaymentUrl;
		private UI.ComboBoxClinicPicker comboClinicPicker;
		private UI.GroupBox groupBox2;
		private System.Windows.Forms.Label labelTags;
		private UI.Button butSave;
		private UI.GroupBox groupBoxMessageToPay;
		private UI.GroupBox groupBoxUserControlFill;
	}
}