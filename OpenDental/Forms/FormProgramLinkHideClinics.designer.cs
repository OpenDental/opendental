namespace OpenDental {
	partial class FormProgramLinkHideClinics {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProgramLinkHideClinics));
			this.labelHidden = new System.Windows.Forms.Label();
			this.listboxHiddenClinics = new OpenDental.UI.ListBoxOD();
			this.listboxVisibleClinics = new OpenDental.UI.ListBoxOD();
			this.labelVisible = new System.Windows.Forms.Label();
			this.butLeft = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelClinicStateWarning = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.checkOrderAlphabetical = new System.Windows.Forms.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelHidden
			// 
			this.labelHidden.Location = new System.Drawing.Point(12, 74);
			this.labelHidden.Name = "labelHidden";
			this.labelHidden.Size = new System.Drawing.Size(180, 18);
			this.labelHidden.TabIndex = 37;
			this.labelHidden.Text = "Hidden";
			this.labelHidden.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listboxHiddenClinics
			// 
			this.listboxHiddenClinics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listboxHiddenClinics.Location = new System.Drawing.Point(12, 95);
			this.listboxHiddenClinics.Name = "listboxHiddenClinics";
			this.listboxHiddenClinics.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listboxHiddenClinics.Size = new System.Drawing.Size(180, 368);
			this.listboxHiddenClinics.TabIndex = 38;
			this.listboxHiddenClinics.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListboxHiddenClinics_MouseClick);
			// 
			// listboxVisibleClinics
			// 
			this.listboxVisibleClinics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listboxVisibleClinics.Location = new System.Drawing.Point(260, 95);
			this.listboxVisibleClinics.Name = "listboxVisibleClinics";
			this.listboxVisibleClinics.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listboxVisibleClinics.Size = new System.Drawing.Size(180, 368);
			this.listboxVisibleClinics.TabIndex = 42;
			this.listboxVisibleClinics.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListboxVisibleClinics_MouseClick);
			// 
			// labelVisible
			// 
			this.labelVisible.Location = new System.Drawing.Point(260, 74);
			this.labelVisible.Name = "labelVisible";
			this.labelVisible.Size = new System.Drawing.Size(180, 18);
			this.labelVisible.TabIndex = 41;
			this.labelVisible.Text = "Visible";
			this.labelVisible.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(204, 201);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(44, 24);
			this.butLeft.TabIndex = 40;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(204, 167);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(44, 24);
			this.butRight.TabIndex = 39;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(365, 473);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelClinicStateWarning
			// 
			this.labelClinicStateWarning.ForeColor = System.Drawing.Color.DarkRed;
			this.labelClinicStateWarning.Location = new System.Drawing.Point(12, 56);
			this.labelClinicStateWarning.Name = "labelClinicStateWarning";
			this.labelClinicStateWarning.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelClinicStateWarning.Size = new System.Drawing.Size(310, 17);
			this.labelClinicStateWarning.TabIndex = 77;
			this.labelClinicStateWarning.Text = "Some clinics not shown due to user clinic restriction.";
			// 
			// label3
			// 
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(12, 4);
			this.label3.Name = "label3";
			this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label3.Size = new System.Drawing.Size(436, 29);
			this.label3.TabIndex = 78;
			this.label3.Text = "Program Link button will be hidden for clinics on the left, and visible for clini" +
    "cs on the right.";
			// 
			// checkOrderAlphabetical
			// 
			this.checkOrderAlphabetical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrderAlphabetical.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOrderAlphabetical.Location = new System.Drawing.Point(328, 55);
			this.checkOrderAlphabetical.Name = "checkOrderAlphabetical";
			this.checkOrderAlphabetical.Size = new System.Drawing.Size(111, 18);
			this.checkOrderAlphabetical.TabIndex = 79;
			this.checkOrderAlphabetical.Text = "Order Alphabetical";
			this.checkOrderAlphabetical.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrderAlphabetical.UseVisualStyleBackColor = true;
			this.checkOrderAlphabetical.CheckedChanged += new System.EventHandler(this.CheckOrderAlphabetical_CheckedChanged);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(284, 473);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 80;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormProgramLinkHideClinics
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(451, 506);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkOrderAlphabetical);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelClinicStateWarning);
			this.Controls.Add(this.listboxVisibleClinics);
			this.Controls.Add(this.labelVisible);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.listboxHiddenClinics);
			this.Controls.Add(this.labelHidden);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProgramLinkHideClinics";
			this.Text = "Hide Program Link Button by Clinic";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelHidden;
		private UI.ListBoxOD listboxHiddenClinics;
		private UI.Button butRight;
		private UI.Button butLeft;
		private UI.ListBoxOD listboxVisibleClinics;
		private System.Windows.Forms.Label labelVisible;
		private System.Windows.Forms.Label labelClinicStateWarning;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkOrderAlphabetical;
		private UI.Button butOK;
	}
}