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
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBoxPaymentPortal = new OpenDental.UI.GroupBox();
			this.labelPortalExplanation = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textPatientFacingPaymentUrl = new System.Windows.Forms.TextBox();
			this.comboClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupBoxPaymentPortal.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(394, 140);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 100;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(475, 140);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 99;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupBoxPaymentPortal
			// 
			this.groupBoxPaymentPortal.Controls.Add(this.labelPortalExplanation);
			this.groupBoxPaymentPortal.Controls.Add(this.label8);
			this.groupBoxPaymentPortal.Controls.Add(this.textPatientFacingPaymentUrl);
			this.groupBoxPaymentPortal.Location = new System.Drawing.Point(12, 40);
			this.groupBoxPaymentPortal.Name = "groupBoxPaymentPortal";
			this.groupBoxPaymentPortal.Size = new System.Drawing.Size(538, 85);
			this.groupBoxPaymentPortal.TabIndex = 0;
			this.groupBoxPaymentPortal.Text = "Payment Portal";
			// 
			// labelPortalExplanation
			// 
			this.labelPortalExplanation.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelPortalExplanation.Location = new System.Drawing.Point(16, 24);
			this.labelPortalExplanation.Name = "labelPortalExplanation";
			this.labelPortalExplanation.Size = new System.Drawing.Size(504, 18);
			this.labelPortalExplanation.TabIndex = 0;
			this.labelPortalExplanation.Text = "This is the link that patients will use to reach your office\'s Payment Portal.";
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label8.Location = new System.Drawing.Point(-142, 129);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(129, 17);
			this.label8.TabIndex = 52;
			this.label8.Text = "Patient Facing URL";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientFacingPaymentUrl
			// 
			this.textPatientFacingPaymentUrl.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textPatientFacingPaymentUrl.Location = new System.Drawing.Point(19, 45);
			this.textPatientFacingPaymentUrl.Name = "textPatientFacingPaymentUrl";
			this.textPatientFacingPaymentUrl.ReadOnly = true;
			this.textPatientFacingPaymentUrl.Size = new System.Drawing.Size(501, 20);
			this.textPatientFacingPaymentUrl.TabIndex = 1;
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
			// FormEServicesPaymentPortal
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(562, 176);
			this.Controls.Add(this.comboClinicPicker);
			this.Controls.Add(this.groupBoxPaymentPortal);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesPaymentPortal";
			this.Text = "eService Payment Portal";
			this.groupBoxPaymentPortal.ResumeLayout(false);
			this.groupBoxPaymentPortal.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GroupBox groupBoxPaymentPortal;
		private System.Windows.Forms.Label labelPortalExplanation;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textPatientFacingPaymentUrl;
		private UI.ComboBoxClinicPicker comboClinicPicker;
	}
}