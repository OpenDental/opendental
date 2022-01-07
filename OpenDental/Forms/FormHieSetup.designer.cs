namespace OpenDental{
	partial class FormHieSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHieSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.checkMedicaidOnly = new System.Windows.Forms.CheckBox();
			this.textExportPath = new System.Windows.Forms.TextBox();
			this.labelExportPath = new System.Windows.Forms.Label();
			this.labelExportTimeOfDay = new System.Windows.Forms.Label();
			this.dateTimeOfExportCCD = new System.Windows.Forms.DateTimePicker();
			this.butBrowse = new OpenDental.UI.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(517, 212);
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
			this.butCancel.Location = new System.Drawing.Point(517, 242);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(127, 56);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(237, 21);
			this.comboClinic.TabIndex = 4;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Location = new System.Drawing.Point(74, 83);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(104, 20);
			this.checkEnabled.TabIndex = 15;
			this.checkEnabled.Text = "Is Enabled";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// checkMedicaidOnly
			// 
			this.checkMedicaidOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicaidOnly.Location = new System.Drawing.Point(74, 110);
			this.checkMedicaidOnly.Name = "checkMedicaidOnly";
			this.checkMedicaidOnly.Size = new System.Drawing.Size(104, 20);
			this.checkMedicaidOnly.TabIndex = 16;
			this.checkMedicaidOnly.Text = "Is Medicaid Only";
			this.checkMedicaidOnly.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.checkMedicaidOnly.UseVisualStyleBackColor = true;
			// 
			// textExportPath
			// 
			this.textExportPath.Location = new System.Drawing.Point(164, 137);
			this.textExportPath.MaxLength = 255;
			this.textExportPath.Name = "textExportPath";
			this.textExportPath.Size = new System.Drawing.Size(317, 20);
			this.textExportPath.TabIndex = 18;
			// 
			// labelExportPath
			// 
			this.labelExportPath.Location = new System.Drawing.Point(29, 137);
			this.labelExportPath.Name = "labelExportPath";
			this.labelExportPath.Size = new System.Drawing.Size(134, 20);
			this.labelExportPath.TabIndex = 17;
			this.labelExportPath.Text = "Export CCD Path";
			this.labelExportPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelExportTimeOfDay
			// 
			this.labelExportTimeOfDay.Location = new System.Drawing.Point(13, 164);
			this.labelExportTimeOfDay.Name = "labelExportTimeOfDay";
			this.labelExportTimeOfDay.Size = new System.Drawing.Size(150, 20);
			this.labelExportTimeOfDay.TabIndex = 19;
			this.labelExportTimeOfDay.Text = "Export CCD Time of Day";
			this.labelExportTimeOfDay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dateTimeOfExportCCD
			// 
			this.dateTimeOfExportCCD.CustomFormat = " ";
			this.dateTimeOfExportCCD.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.dateTimeOfExportCCD.Location = new System.Drawing.Point(164, 164);
			this.dateTimeOfExportCCD.Name = "dateTimeOfExportCCD";
			this.dateTimeOfExportCCD.ShowUpDown = true;
			this.dateTimeOfExportCCD.Size = new System.Drawing.Size(113, 20);
			this.dateTimeOfExportCCD.TabIndex = 20;
			this.dateTimeOfExportCCD.Value = new System.DateTime(2021, 4, 1, 21, 0, 0, 0);
			// 
			// butBrowse
			// 
			this.butBrowse.Location = new System.Drawing.Point(484, 137);
			this.butBrowse.Name = "butBrowse";
			this.butBrowse.Size = new System.Drawing.Size(30, 20);
			this.butBrowse.TabIndex = 21;
			this.butBrowse.Text = "...";
			this.butBrowse.UseVisualStyleBackColor = true;
			this.butBrowse.Click += new System.EventHandler(this.butBrowse_Click);
			// 
			// FormHieSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(604, 278);
			this.Controls.Add(this.butBrowse);
			this.Controls.Add(this.dateTimeOfExportCCD);
			this.Controls.Add(this.labelExportTimeOfDay);
			this.Controls.Add(this.textExportPath);
			this.Controls.Add(this.labelExportPath);
			this.Controls.Add(this.checkMedicaidOnly);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormHieSetup";
			this.Text = "HIE Setup";
			this.Load += new System.EventHandler(this.FormHieSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.CheckBox checkMedicaidOnly;
		private System.Windows.Forms.TextBox textExportPath;
		private System.Windows.Forms.Label labelExportPath;
		private System.Windows.Forms.Label labelExportTimeOfDay;
		private System.Windows.Forms.DateTimePicker dateTimeOfExportCCD;
		private UI.Button butBrowse;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}