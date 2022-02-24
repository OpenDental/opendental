namespace OpenDental{
	partial class FormCloneAdd {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCloneAdd));
			this.butClone = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelSpecialty = new System.Windows.Forms.Label();
			this.labelClinic = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxOD();
			this.comboSpecialty = new OpenDental.UI.ComboBoxOD();
			this.textPreferred = new System.Windows.Forms.TextBox();
			this.textMiddleI = new System.Windows.Forms.TextBox();
			this.textFName = new System.Windows.Forms.TextBox();
			this.textLName = new System.Windows.Forms.TextBox();
			this.labelPreferredAndMiddleI = new System.Windows.Forms.Label();
			this.labelFName = new System.Windows.Forms.Label();
			this.labelLName = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textAge = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBirthDate = new System.Windows.Forms.TextBox();
			this.labelPriProv = new System.Windows.Forms.Label();
			this.butPickPrimary = new OpenDental.UI.Button();
			this.comboPriProv = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClone
			// 
			this.butClone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClone.Location = new System.Drawing.Point(353, 212);
			this.butClone.Name = "butClone";
			this.butClone.Size = new System.Drawing.Size(75, 24);
			this.butClone.TabIndex = 3;
			this.butClone.Text = "Clone";
			this.butClone.Click += new System.EventHandler(this.butClone_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(434, 212);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelSpecialty
			// 
			this.labelSpecialty.Location = new System.Drawing.Point(83, 151);
			this.labelSpecialty.Name = "labelSpecialty";
			this.labelSpecialty.Size = new System.Drawing.Size(110, 16);
			this.labelSpecialty.TabIndex = 66;
			this.labelSpecialty.Text = "Specialty";
			this.labelSpecialty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelSpecialty.Visible = false;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(83, 176);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(110, 16);
			this.labelClinic.TabIndex = 67;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelClinic.Visible = false;
			// 
			// comboClinic
			// 
			this.comboClinic.Location = new System.Drawing.Point(196, 174);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(232, 21);
			this.comboClinic.TabIndex = 68;
			this.comboClinic.Visible = false;
			// 
			// comboSpecialty
			// 
			this.comboSpecialty.Location = new System.Drawing.Point(196, 149);
			this.comboSpecialty.Name = "comboSpecialty";
			this.comboSpecialty.Size = new System.Drawing.Size(232, 21);
			this.comboSpecialty.TabIndex = 69;
			this.comboSpecialty.Visible = false;
			this.comboSpecialty.SelectionChangeCommitted += new System.EventHandler(this.comboSpecialty_SelectionChangeCommitted);
			// 
			// textPreferred
			// 
			this.textPreferred.Location = new System.Drawing.Point(184, 54);
			this.textPreferred.MaxLength = 100;
			this.textPreferred.Name = "textPreferred";
			this.textPreferred.ReadOnly = true;
			this.textPreferred.Size = new System.Drawing.Size(145, 20);
			this.textPreferred.TabIndex = 75;
			// 
			// textMiddleI
			// 
			this.textMiddleI.Location = new System.Drawing.Point(330, 54);
			this.textMiddleI.MaxLength = 100;
			this.textMiddleI.Name = "textMiddleI";
			this.textMiddleI.ReadOnly = true;
			this.textMiddleI.Size = new System.Drawing.Size(86, 20);
			this.textMiddleI.TabIndex = 76;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(184, 34);
			this.textFName.MaxLength = 100;
			this.textFName.Name = "textFName";
			this.textFName.ReadOnly = true;
			this.textFName.Size = new System.Drawing.Size(232, 20);
			this.textFName.TabIndex = 74;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(184, 14);
			this.textLName.MaxLength = 100;
			this.textLName.Name = "textLName";
			this.textLName.ReadOnly = true;
			this.textLName.Size = new System.Drawing.Size(232, 20);
			this.textLName.TabIndex = 70;
			// 
			// labelPreferredAndMiddleI
			// 
			this.labelPreferredAndMiddleI.Location = new System.Drawing.Point(6, 57);
			this.labelPreferredAndMiddleI.Name = "labelPreferredAndMiddleI";
			this.labelPreferredAndMiddleI.Size = new System.Drawing.Size(177, 14);
			this.labelPreferredAndMiddleI.TabIndex = 71;
			this.labelPreferredAndMiddleI.Text = "Preferred Name, Middle Initial";
			this.labelPreferredAndMiddleI.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFName
			// 
			this.labelFName.Location = new System.Drawing.Point(29, 37);
			this.labelFName.Name = "labelFName";
			this.labelFName.Size = new System.Drawing.Size(154, 14);
			this.labelFName.TabIndex = 72;
			this.labelFName.Text = "First Name";
			this.labelFName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelLName
			// 
			this.labelLName.Location = new System.Drawing.Point(29, 17);
			this.labelLName.Name = "labelLName";
			this.labelLName.Size = new System.Drawing.Size(154, 14);
			this.labelLName.TabIndex = 73;
			this.labelLName.Text = "Last Name";
			this.labelLName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textAge);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textBirthDate);
			this.groupBox1.Controls.Add(this.labelLName);
			this.groupBox1.Controls.Add(this.labelFName);
			this.groupBox1.Controls.Add(this.labelPreferredAndMiddleI);
			this.groupBox1.Controls.Add(this.textLName);
			this.groupBox1.Controls.Add(this.textPreferred);
			this.groupBox1.Controls.Add(this.textFName);
			this.groupBox1.Controls.Add(this.textMiddleI);
			this.groupBox1.Location = new System.Drawing.Point(12, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(497, 104);
			this.groupBox1.TabIndex = 80;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Patient to Clone";
			// 
			// textAge
			// 
			this.textAge.Location = new System.Drawing.Point(330, 74);
			this.textAge.MaxLength = 100;
			this.textAge.Name = "textAge";
			this.textAge.ReadOnly = true;
			this.textAge.Size = new System.Drawing.Size(86, 20);
			this.textAge.TabIndex = 83;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(29, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(154, 14);
			this.label2.TabIndex = 82;
			this.label2.Text = "Birthdate, Age";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBirthDate
			// 
			this.textBirthDate.Location = new System.Drawing.Point(184, 74);
			this.textBirthDate.MaxLength = 100;
			this.textBirthDate.Name = "textBirthDate";
			this.textBirthDate.ReadOnly = true;
			this.textBirthDate.Size = new System.Drawing.Size(145, 20);
			this.textBirthDate.TabIndex = 81;
			// 
			// labelPriProv
			// 
			this.labelPriProv.Location = new System.Drawing.Point(43, 126);
			this.labelPriProv.Name = "labelPriProv";
			this.labelPriProv.Size = new System.Drawing.Size(152, 14);
			this.labelPriProv.TabIndex = 81;
			this.labelPriProv.Text = "Primary Provider";
			this.labelPriProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPickPrimary
			// 
			this.butPickPrimary.Location = new System.Drawing.Point(434, 124);
			this.butPickPrimary.Name = "butPickPrimary";
			this.butPickPrimary.Size = new System.Drawing.Size(23, 21);
			this.butPickPrimary.TabIndex = 83;
			this.butPickPrimary.Text = "...";
			this.butPickPrimary.Click += new System.EventHandler(this.butPickPrimary_Click);
			// 
			// comboPriProv
			// 
			this.comboPriProv.Location = new System.Drawing.Point(196, 124);
			this.comboPriProv.MaxDropDownItems = 30;
			this.comboPriProv.Name = "comboPriProv";
			this.comboPriProv.Size = new System.Drawing.Size(232, 21);
			this.comboPriProv.TabIndex = 84;
			this.comboPriProv.SelectionChangeCommitted += new System.EventHandler(this.comboPriProv_SelectionChangeCommitted);
			// 
			// FormCloneAdd
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(521, 248);
			this.Controls.Add(this.comboPriProv);
			this.Controls.Add(this.butPickPrimary);
			this.Controls.Add(this.labelPriProv);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.comboSpecialty);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.labelSpecialty);
			this.Controls.Add(this.butClone);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCloneAdd";
			this.Text = "Clone Add";
			this.Load += new System.EventHandler(this.FormCloneAdd_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClone;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelSpecialty;
		private System.Windows.Forms.Label labelClinic;
		private OpenDental.UI.ComboBoxOD comboClinic;
		private OpenDental.UI.ComboBoxOD comboSpecialty;
		private System.Windows.Forms.TextBox textPreferred;
		private System.Windows.Forms.TextBox textMiddleI;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.Label labelPreferredAndMiddleI;
		private System.Windows.Forms.Label labelFName;
		private System.Windows.Forms.Label labelLName;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBirthDate;
		private System.Windows.Forms.TextBox textAge;
		private System.Windows.Forms.Label labelPriProv;
		private UI.Button butPickPrimary;
		private System.Windows.Forms.ComboBox comboPriProv;
	}
}