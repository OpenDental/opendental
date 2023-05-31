namespace OpenDental {
	partial class FormEServicesAutoMsgingPreferences {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesAutoMsgingPreferences));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkUseDefaultPrefs = new System.Windows.Forms.CheckBox();
			this.groupNewPat = new System.Windows.Forms.GroupBox();
			this.listBoxWebForms = new OpenDental.UI.ListBox();
			this.labelWebForm = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkSendToGuarantorForMinors = new System.Windows.Forms.CheckBox();
			this.groupNewPat.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(199, 310);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(280, 310);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkUseDefaultPrefs
			// 
			this.checkUseDefaultPrefs.Location = new System.Drawing.Point(95, 52);
			this.checkUseDefaultPrefs.Name = "checkUseDefaultPrefs";
			this.checkUseDefaultPrefs.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultPrefs.TabIndex = 273;
			this.checkUseDefaultPrefs.Text = "Use Defaults";
			this.checkUseDefaultPrefs.UseVisualStyleBackColor = true;
			this.checkUseDefaultPrefs.Click += new System.EventHandler(this.checkUseDefaultPrefs_Click);
			// 
			// groupNewPat
			// 
			this.groupNewPat.Controls.Add(this.listBoxWebForms);
			this.groupNewPat.Controls.Add(this.labelWebForm);
			this.groupNewPat.Location = new System.Drawing.Point(12, 77);
			this.groupNewPat.Name = "groupNewPat";
			this.groupNewPat.Size = new System.Drawing.Size(306, 197);
			this.groupNewPat.TabIndex = 276;
			this.groupNewPat.TabStop = false;
			this.groupNewPat.Text = "New Patient Thank-Yous";
			// 
			// listBoxWebForms
			// 
			this.listBoxWebForms.Location = new System.Drawing.Point(83, 23);
			this.listBoxWebForms.Name = "listBoxWebForms";
			this.listBoxWebForms.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxWebForms.Size = new System.Drawing.Size(201, 160);
			this.listBoxWebForms.TabIndex = 278;
			this.listBoxWebForms.Text = "listBoxWebForms";
			this.listBoxWebForms.SelectionChangeCommitted += new System.EventHandler(this.listBoxWebForm_SelectionChangeCommitted);
			// 
			// labelWebForm
			// 
			this.labelWebForm.Location = new System.Drawing.Point(10, 23);
			this.labelWebForm.Name = "labelWebForm";
			this.labelWebForm.Size = new System.Drawing.Size(67, 16);
			this.labelWebForm.TabIndex = 277;
			this.labelWebForm.Text = "Web Forms";
			this.labelWebForm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Default";
			this.comboClinic.IncludeAll = true;
			this.comboClinic.Location = new System.Drawing.Point(59, 25);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(237, 21);
			this.comboClinic.TabIndex = 278;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// checkSendToGuarantorForMinors
			// 
			this.checkSendToGuarantorForMinors.Location = new System.Drawing.Point(95, 280);
			this.checkSendToGuarantorForMinors.Name = "checkSendToGuarantorForMinors";
			this.checkSendToGuarantorForMinors.Size = new System.Drawing.Size(260, 19);
			this.checkSendToGuarantorForMinors.TabIndex = 517;
			this.checkSendToGuarantorForMinors.Text = "Patients under age 18 - Send to Guarantor";
			this.checkSendToGuarantorForMinors.UseVisualStyleBackColor = true;
			this.checkSendToGuarantorForMinors.Click += new System.EventHandler(this.checkSendToGuarantorForMinors_Click);
			// 
			// FormEServicesAutoMsgingPreferences
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(367, 346);
			this.Controls.Add(this.checkSendToGuarantorForMinors);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.groupNewPat);
			this.Controls.Add(this.checkUseDefaultPrefs);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesAutoMsgingPreferences";
			this.Text = "Automated Messaging Preferences";
			this.Load += new System.EventHandler(this.FormEServicesAutoMsgingPreferences_Load);
			this.groupNewPat.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkUseDefaultPrefs;
		private System.Windows.Forms.GroupBox groupNewPat;
		private System.Windows.Forms.Label labelWebForm;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.ListBox listBoxWebForms;
		private System.Windows.Forms.CheckBox checkSendToGuarantorForMinors;
	}
}