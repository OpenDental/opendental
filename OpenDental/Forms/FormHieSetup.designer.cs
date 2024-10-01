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
			this.butSave = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkEnabled = new OpenDental.UI.CheckBox();
			this.checkMedicaidOnly = new OpenDental.UI.CheckBox();
			this.textExportPath = new System.Windows.Forms.TextBox();
			this.labelExportPath = new System.Windows.Forms.Label();
			this.labelExportTimeOfDay = new System.Windows.Forms.Label();
			this.dateTimeOfExportCCD = new System.Windows.Forms.DateTimePicker();
			this.butBrowse = new OpenDental.UI.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(456, 135);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(115, 12);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(237, 21);
			this.comboClinic.TabIndex = 4;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Location = new System.Drawing.Point(62, 39);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(104, 20);
			this.checkEnabled.TabIndex = 15;
			this.checkEnabled.Text = "Is Enabled";
			// 
			// checkMedicaidOnly
			// 
			this.checkMedicaidOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicaidOnly.Location = new System.Drawing.Point(62, 66);
			this.checkMedicaidOnly.Name = "checkMedicaidOnly";
			this.checkMedicaidOnly.Size = new System.Drawing.Size(104, 20);
			this.checkMedicaidOnly.TabIndex = 16;
			this.checkMedicaidOnly.Text = "Is Medicaid Only";
			// 
			// textExportPath
			// 
			this.textExportPath.Location = new System.Drawing.Point(152, 93);
			this.textExportPath.MaxLength = 255;
			this.textExportPath.Name = "textExportPath";
			this.textExportPath.Size = new System.Drawing.Size(317, 20);
			this.textExportPath.TabIndex = 18;
			// 
			// labelExportPath
			// 
			this.labelExportPath.Location = new System.Drawing.Point(17, 93);
			this.labelExportPath.Name = "labelExportPath";
			this.labelExportPath.Size = new System.Drawing.Size(134, 20);
			this.labelExportPath.TabIndex = 17;
			this.labelExportPath.Text = "Export CCD Path";
			this.labelExportPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelExportTimeOfDay
			// 
			this.labelExportTimeOfDay.Location = new System.Drawing.Point(1, 120);
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
			this.dateTimeOfExportCCD.Location = new System.Drawing.Point(152, 120);
			this.dateTimeOfExportCCD.Name = "dateTimeOfExportCCD";
			this.dateTimeOfExportCCD.ShowUpDown = true;
			this.dateTimeOfExportCCD.Size = new System.Drawing.Size(113, 20);
			this.dateTimeOfExportCCD.TabIndex = 20;
			this.dateTimeOfExportCCD.Value = new System.DateTime(2021, 4, 1, 21, 0, 0, 0);
			// 
			// butBrowse
			// 
			this.butBrowse.Location = new System.Drawing.Point(472, 93);
			this.butBrowse.Name = "butBrowse";
			this.butBrowse.Size = new System.Drawing.Size(30, 20);
			this.butBrowse.TabIndex = 21;
			this.butBrowse.Text = "...";
			this.butBrowse.UseVisualStyleBackColor = true;
			this.butBrowse.Click += new System.EventHandler(this.butBrowse_Click);
			// 
			// FormHieSetup
			// 
			this.ClientSize = new System.Drawing.Size(543, 171);
			this.Controls.Add(this.butBrowse);
			this.Controls.Add(this.dateTimeOfExportCCD);
			this.Controls.Add(this.labelExportTimeOfDay);
			this.Controls.Add(this.textExportPath);
			this.Controls.Add(this.labelExportPath);
			this.Controls.Add(this.checkMedicaidOnly);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormHieSetup";
			this.Text = "HIE Setup";
			this.Load += new System.EventHandler(this.FormHieSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private UI.ComboBoxClinicPicker comboClinic;
		private OpenDental.UI.CheckBox checkEnabled;
		private OpenDental.UI.CheckBox checkMedicaidOnly;
		private System.Windows.Forms.TextBox textExportPath;
		private System.Windows.Forms.Label labelExportPath;
		private System.Windows.Forms.Label labelExportTimeOfDay;
		private System.Windows.Forms.DateTimePicker dateTimeOfExportCCD;
		private UI.Button butBrowse;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}