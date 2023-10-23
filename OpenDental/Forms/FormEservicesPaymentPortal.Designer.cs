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
			this.comboClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupBoxPaymentPortal.SuspendLayout();
			this.SuspendLayout();
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
			this.ClientSize = new System.Drawing.Size(562, 133);
			this.Controls.Add(this.comboClinicPicker);
			this.Controls.Add(this.groupBoxPaymentPortal);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesPaymentPortal";
			this.Text = "eService Payment Portal";
			this.groupBoxPaymentPortal.ResumeLayout(false);
			this.groupBoxPaymentPortal.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GroupBox groupBoxPaymentPortal;
		private System.Windows.Forms.Label labelPortalExplanation;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textPatientFacingPaymentUrl;
		private UI.ComboBoxClinicPicker comboClinicPicker;
	}
}