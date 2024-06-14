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
			this.butSave = new OpenDental.UI.Button();
			this.checkUseDefaultPrefs = new System.Windows.Forms.CheckBox();
			this.groupNewPat = new System.Windows.Forms.GroupBox();
			this.listBoxWebForms = new OpenDental.UI.ListBox();
			this.labelWebForm = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkSendToGuarantorForMinors = new System.Windows.Forms.CheckBox();
			this.groupNewPat.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(244, 310);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
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
			this.checkSendToGuarantorForMinors.Location = new System.Drawing.Point(59, 280);
			this.checkSendToGuarantorForMinors.Name = "checkSendToGuarantorForMinors";
			this.checkSendToGuarantorForMinors.Size = new System.Drawing.Size(260, 19);
			this.checkSendToGuarantorForMinors.TabIndex = 517;
			this.checkSendToGuarantorForMinors.Text = "Patients under age 18 - Send to Guarantor";
			this.checkSendToGuarantorForMinors.UseVisualStyleBackColor = true;
			this.checkSendToGuarantorForMinors.Click += new System.EventHandler(this.checkSendToGuarantorForMinors_Click);
			// 
			// FormEServicesAutoMsgingPreferences
			// 
			this.ClientSize = new System.Drawing.Size(328, 346);
			this.Controls.Add(this.checkSendToGuarantorForMinors);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.groupNewPat);
			this.Controls.Add(this.checkUseDefaultPrefs);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesAutoMsgingPreferences";
			this.Text = "Automated Messaging Preferences";
			this.Load += new System.EventHandler(this.FormEServicesAutoMsgingPreferences_Load);
			this.groupNewPat.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.CheckBox checkUseDefaultPrefs;
		private System.Windows.Forms.GroupBox groupNewPat;
		private System.Windows.Forms.Label labelWebForm;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.ListBox listBoxWebForms;
		private System.Windows.Forms.CheckBox checkSendToGuarantorForMinors;
	}
}