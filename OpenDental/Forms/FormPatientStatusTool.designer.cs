namespace OpenDental {
	partial class FormPatientStatusTool {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatientStatusTool));
			this.butCancel = new OpenDental.UI.Button();
			this.groupCriteria = new System.Windows.Forms.GroupBox();
			this.comboPatientStatusCur = new OpenDental.UI.ComboBoxOD();
			this.labelPatientCurrent = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.listOptions = new OpenDental.UI.ListBoxOD();
			this.odDatePickerSince = new OpenDental.UI.ODDatePicker();
			this.butSelectAll = new OpenDental.UI.Button();
			this.butDeselectAll = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butCreateList = new OpenDental.UI.Button();
			this.butRun = new OpenDental.UI.Button();
			this.labelChangeTo = new System.Windows.Forms.Label();
			this.comboChangePatientStatusTo = new OpenDental.UI.ComboBoxOD();
			this.groupCriteria.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(932, 603);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupCriteria
			// 
			this.groupCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupCriteria.Controls.Add(this.comboPatientStatusCur);
			this.groupCriteria.Controls.Add(this.labelPatientCurrent);
			this.groupCriteria.Controls.Add(this.label1);
			this.groupCriteria.Controls.Add(this.comboClinic);
			this.groupCriteria.Controls.Add(this.listOptions);
			this.groupCriteria.Controls.Add(this.odDatePickerSince);
			this.groupCriteria.Location = new System.Drawing.Point(19, 12);
			this.groupCriteria.Name = "groupCriteria";
			this.groupCriteria.Size = new System.Drawing.Size(900, 82);
			this.groupCriteria.TabIndex = 83;
			this.groupCriteria.TabStop = false;
			this.groupCriteria.Text = "Criteria";
			// 
			// comboPatientStatusCur
			// 
			this.comboPatientStatusCur.Location = new System.Drawing.Point(131, 36);
			this.comboPatientStatusCur.Name = "comboPatientStatusCur";
			this.comboPatientStatusCur.Size = new System.Drawing.Size(149, 21);
			this.comboPatientStatusCur.TabIndex = 213;
			this.comboPatientStatusCur.Text = "comboBoxOD1";
			this.comboPatientStatusCur.SelectionChangeCommitted += new System.EventHandler(this.comboPatientStatusCur_SelectionChangeCommitted);
			// 
			// labelPatientCurrent
			// 
			this.labelPatientCurrent.Location = new System.Drawing.Point(10, 36);
			this.labelPatientCurrent.Name = "labelPatientCurrent";
			this.labelPatientCurrent.Size = new System.Drawing.Size(111, 16);
			this.labelPatientCurrent.TabIndex = 213;
			this.labelPatientCurrent.Text = "Patients with Status";
			this.labelPatientCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(478, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 16);
			this.label1.TabIndex = 86;
			this.label1.Text = "Since";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(686, 36);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(197, 21);
			this.comboClinic.TabIndex = 83;
			// 
			// listOptions
			// 
			this.listOptions.Location = new System.Drawing.Point(322, 19);
			this.listOptions.Name = "listOptions";
			this.listOptions.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listOptions.Size = new System.Drawing.Size(120, 56);
			this.listOptions.TabIndex = 77;
			// 
			// odDatePickerSince
			// 
			this.odDatePickerSince.BackColor = System.Drawing.Color.Transparent;
			this.odDatePickerSince.Location = new System.Drawing.Point(464, 36);
			this.odDatePickerSince.MaximumSize = new System.Drawing.Size(0, 184);
			this.odDatePickerSince.MinimumSize = new System.Drawing.Size(227, 23);
			this.odDatePickerSince.Name = "odDatePickerSince";
			this.odDatePickerSince.Size = new System.Drawing.Size(227, 23);
			this.odDatePickerSince.TabIndex = 85;
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSelectAll.Location = new System.Drawing.Point(19, 601);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(83, 26);
			this.butSelectAll.TabIndex = 76;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.UseVisualStyleBackColor = true;
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// butDeselectAll
			// 
			this.butDeselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeselectAll.Location = new System.Drawing.Point(108, 601);
			this.butDeselectAll.Name = "butDeselectAll";
			this.butDeselectAll.Size = new System.Drawing.Size(82, 26);
			this.butDeselectAll.TabIndex = 75;
			this.butDeselectAll.Text = "Deselect All";
			this.butDeselectAll.UseVisualStyleBackColor = true;
			this.butDeselectAll.Click += new System.EventHandler(this.butDeselectAll_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(19, 100);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(988, 477);
			this.gridMain.TabIndex = 6;
			this.gridMain.Title = "Patients to Convert";
			this.gridMain.TranslationName = "Patients to Convert";
			// 
			// butCreateList
			// 
			this.butCreateList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butCreateList.Location = new System.Drawing.Point(932, 68);
			this.butCreateList.Name = "butCreateList";
			this.butCreateList.Size = new System.Drawing.Size(75, 26);
			this.butCreateList.TabIndex = 74;
			this.butCreateList.Text = "Create List";
			this.butCreateList.UseVisualStyleBackColor = true;
			this.butCreateList.Click += new System.EventHandler(this.butCreateList_Click);
			// 
			// butRun
			// 
			this.butRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRun.Location = new System.Drawing.Point(844, 603);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(75, 24);
			this.butRun.TabIndex = 3;
			this.butRun.Text = "&Run";
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// labelChangeTo
			// 
			this.labelChangeTo.Location = new System.Drawing.Point(358, 598);
			this.labelChangeTo.Name = "labelChangeTo";
			this.labelChangeTo.Size = new System.Drawing.Size(147, 21);
			this.labelChangeTo.TabIndex = 211;
			this.labelChangeTo.Text = "Change Patient Status To";
			this.labelChangeTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboChangePatientStatusTo
			// 
			this.comboChangePatientStatusTo.Location = new System.Drawing.Point(511, 598);
			this.comboChangePatientStatusTo.Name = "comboChangePatientStatusTo";
			this.comboChangePatientStatusTo.Size = new System.Drawing.Size(149, 21);
			this.comboChangePatientStatusTo.TabIndex = 214;
			this.comboChangePatientStatusTo.Text = "comboBoxOD1";
			// 
			// FormPatientStatusTool
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1019, 639);
			this.Controls.Add(this.labelChangeTo);
			this.Controls.Add(this.groupCriteria);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.butDeselectAll);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCreateList);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.comboChangePatientStatusTo);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPatientStatusTool";
			this.Text = "Patient Status Setter";
			this.Load += new System.EventHandler(this.FormPatientStatusTool_Load);
			this.groupCriteria.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butRun;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private UI.Button butCreateList;
		private UI.Button butDeselectAll;
		private UI.Button butSelectAll;
		private OpenDental.UI.ListBoxOD listOptions;
		private System.Windows.Forms.GroupBox groupCriteria;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.Label label1;
		private UI.ODDatePicker odDatePickerSince;
		private System.Windows.Forms.Label labelChangeTo;
		private System.Windows.Forms.Label labelPatientCurrent;
		private UI.ComboBoxOD comboPatientStatusCur;
		private UI.ComboBoxOD comboChangePatientStatusTo;
	}
}