namespace OpenDental{
	partial class FormCertifications {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCertifications));
			this.butCancel = new OpenDental.UI.Button();
			this.listBoxEmployee = new OpenDental.UI.ListBoxOD();
			this.gridMain = new OpenDental.UI.GridOD();
			this.listBoxCategories = new OpenDental.UI.ListBoxOD();
			this.butSetup = new OpenDental.UI.Button();
			this.labelEmployee = new System.Windows.Forms.Label();
			this.labelCategories = new System.Windows.Forms.Label();
			this.groupBoxOrderBy = new OpenDental.UI.GroupBoxOD();
			this.radioCategory = new System.Windows.Forms.RadioButton();
			this.radioCertification = new System.Windows.Forms.RadioButton();
			this.labelCertification = new System.Windows.Forms.Label();
			this.listBoxCertification = new OpenDental.UI.ListBoxOD();
			this.labelEmpSearch = new System.Windows.Forms.Label();
			this.textEmpSearch = new System.Windows.Forms.TextBox();
			this.butPrint = new OpenDental.UI.Button();
			this.checkIncomplete = new System.Windows.Forms.CheckBox();
			this.checkSortDate = new System.Windows.Forms.CheckBox();
			this.labelCategories2 = new System.Windows.Forms.Label();
			this.listBoxCategories2 = new OpenDental.UI.ListBoxOD();
			this.comboSupervisor = new OpenDental.UI.ComboBoxOD();
			this.labelReportsTo = new System.Windows.Forms.Label();
			this.checkSortDateCertComplete = new System.Windows.Forms.CheckBox();
			this.groupBoxOrderBy.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1014, 625);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listBoxEmployee
			// 
			this.listBoxEmployee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxEmployee.IntegralHeight = false;
			this.listBoxEmployee.Location = new System.Drawing.Point(12, 126);
			this.listBoxEmployee.Name = "listBoxEmployee";
			this.listBoxEmployee.Size = new System.Drawing.Size(120, 524);
			this.listBoxEmployee.TabIndex = 3;
			this.listBoxEmployee.Text = "listBoxOD1";
			this.listBoxEmployee.SelectionChangeCommitted += new System.EventHandler(this.listBoxEmployee_SelectionChangeCommitted);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(288, 29);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(708, 620);
			this.gridMain.TabIndex = 5;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// listBoxCategories
			// 
			this.listBoxCategories.Location = new System.Drawing.Point(148, 126);
			this.listBoxCategories.Name = "listBoxCategories";
			this.listBoxCategories.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxCategories.Size = new System.Drawing.Size(120, 316);
			this.listBoxCategories.TabIndex = 4;
			this.listBoxCategories.Text = "listBoxOD1";
			this.listBoxCategories.SelectionChangeCommitted += new System.EventHandler(this.listBoxCategories_SelectionChangeCommitted);
			// 
			// butSetup
			// 
			this.butSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSetup.Location = new System.Drawing.Point(11, 2);
			this.butSetup.Name = "butSetup";
			this.butSetup.Size = new System.Drawing.Size(75, 24);
			this.butSetup.TabIndex = 8;
			this.butSetup.Text = "Setup";
			this.butSetup.Click += new System.EventHandler(this.butSetup_Click);
			// 
			// labelEmployee
			// 
			this.labelEmployee.Location = new System.Drawing.Point(11, 105);
			this.labelEmployee.Name = "labelEmployee";
			this.labelEmployee.Size = new System.Drawing.Size(100, 18);
			this.labelEmployee.TabIndex = 50;
			this.labelEmployee.Text = "Employee";
			this.labelEmployee.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelCategories
			// 
			this.labelCategories.Location = new System.Drawing.Point(147, 105);
			this.labelCategories.Name = "labelCategories";
			this.labelCategories.Size = new System.Drawing.Size(100, 18);
			this.labelCategories.TabIndex = 51;
			this.labelCategories.Text = "Categories";
			this.labelCategories.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBoxOrderBy
			// 
			this.groupBoxOrderBy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOrderBy.Controls.Add(this.radioCategory);
			this.groupBoxOrderBy.Controls.Add(this.radioCertification);
			this.groupBoxOrderBy.Location = new System.Drawing.Point(150, 21);
			this.groupBoxOrderBy.Name = "groupBoxOrderBy";
			this.groupBoxOrderBy.Size = new System.Drawing.Size(120, 70);
			this.groupBoxOrderBy.TabIndex = 5;
			this.groupBoxOrderBy.TabStop = false;
			this.groupBoxOrderBy.Text = "View by";
			// 
			// radioCategory
			// 
			this.radioCategory.Checked = true;
			this.radioCategory.Location = new System.Drawing.Point(6, 22);
			this.radioCategory.Name = "radioCategory";
			this.radioCategory.Size = new System.Drawing.Size(97, 18);
			this.radioCategory.TabIndex = 1;
			this.radioCategory.TabStop = true;
			this.radioCategory.Text = "Category";
			this.radioCategory.UseVisualStyleBackColor = true;
			this.radioCategory.Click += new System.EventHandler(this.radioCategory_Click);
			// 
			// radioCertification
			// 
			this.radioCertification.Location = new System.Drawing.Point(6, 43);
			this.radioCertification.Name = "radioCertification";
			this.radioCertification.Size = new System.Drawing.Size(97, 18);
			this.radioCertification.TabIndex = 2;
			this.radioCertification.Text = "Cert Complete";
			this.radioCertification.UseVisualStyleBackColor = true;
			this.radioCertification.Click += new System.EventHandler(this.radioCertification_Click);
			// 
			// labelCertification
			// 
			this.labelCertification.Location = new System.Drawing.Point(420, 105);
			this.labelCertification.Name = "labelCertification";
			this.labelCertification.Size = new System.Drawing.Size(100, 18);
			this.labelCertification.TabIndex = 53;
			this.labelCertification.Text = "Certifications";
			this.labelCertification.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxCertification
			// 
			this.listBoxCertification.Location = new System.Drawing.Point(421, 126);
			this.listBoxCertification.Name = "listBoxCertification";
			this.listBoxCertification.Size = new System.Drawing.Size(120, 524);
			this.listBoxCertification.TabIndex = 32;
			this.listBoxCertification.Text = "listBoxOD1";
			this.listBoxCertification.SelectionChangeCommitted += new System.EventHandler(this.listBoxCertification_SelectionChangeCommitted);
			// 
			// labelEmpSearch
			// 
			this.labelEmpSearch.Location = new System.Drawing.Point(11, 26);
			this.labelEmpSearch.Name = "labelEmpSearch";
			this.labelEmpSearch.Size = new System.Drawing.Size(100, 18);
			this.labelEmpSearch.TabIndex = 14;
			this.labelEmpSearch.Text = "Employee Search";
			this.labelEmpSearch.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textEmpSearch
			// 
			this.textEmpSearch.Location = new System.Drawing.Point(12, 47);
			this.textEmpSearch.Name = "textEmpSearch";
			this.textEmpSearch.Size = new System.Drawing.Size(120, 20);
			this.textEmpSearch.TabIndex = 1;
			this.textEmpSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEmpSearch_KeyUp);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(1014, 595);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 9;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// checkIncomplete
			// 
			this.checkIncomplete.Location = new System.Drawing.Point(288, 7);
			this.checkIncomplete.Name = "checkIncomplete";
			this.checkIncomplete.Size = new System.Drawing.Size(150, 18);
			this.checkIncomplete.TabIndex = 6;
			this.checkIncomplete.Text = "Show Only Incomplete";
			this.checkIncomplete.UseVisualStyleBackColor = true;
			this.checkIncomplete.Click += new System.EventHandler(this.checkIncomplete_Click);
			// 
			// checkSortDate
			// 
			this.checkSortDate.Location = new System.Drawing.Point(444, 7);
			this.checkSortDate.Name = "checkSortDate";
			this.checkSortDate.Size = new System.Drawing.Size(150, 18);
			this.checkSortDate.TabIndex = 7;
			this.checkSortDate.Text = "Sort By Date Completed";
			this.checkSortDate.UseVisualStyleBackColor = true;
			this.checkSortDate.Click += new System.EventHandler(this.checkSortDate_Click);
			// 
			// labelCategories2
			// 
			this.labelCategories2.Location = new System.Drawing.Point(294, 105);
			this.labelCategories2.Name = "labelCategories2";
			this.labelCategories2.Size = new System.Drawing.Size(100, 18);
			this.labelCategories2.TabIndex = 52;
			this.labelCategories2.Text = "Categories";
			this.labelCategories2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxCategories2
			// 
			this.listBoxCategories2.Location = new System.Drawing.Point(295, 126);
			this.listBoxCategories2.Name = "listBoxCategories2";
			this.listBoxCategories2.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxCategories2.Size = new System.Drawing.Size(120, 316);
			this.listBoxCategories2.TabIndex = 31;
			this.listBoxCategories2.Text = "listBoxOD1";
			this.listBoxCategories2.SelectionChangeCommitted += new System.EventHandler(this.listBoxCategories2_SelectionChangeCommitted);
			// 
			// comboSupervisor
			// 
			this.comboSupervisor.Location = new System.Drawing.Point(12, 86);
			this.comboSupervisor.Name = "comboSupervisor";
			this.comboSupervisor.Size = new System.Drawing.Size(121, 21);
			this.comboSupervisor.TabIndex = 2;
			this.comboSupervisor.Text = "comboBoxOD1";
			this.comboSupervisor.SelectionChangeCommitted += new System.EventHandler(this.comboSupervisor_SelectionChangeCommitted);
			// 
			// labelReportsTo
			// 
			this.labelReportsTo.Location = new System.Drawing.Point(11, 65);
			this.labelReportsTo.Name = "labelReportsTo";
			this.labelReportsTo.Size = new System.Drawing.Size(100, 18);
			this.labelReportsTo.TabIndex = 33;
			this.labelReportsTo.Text = "Reports To";
			this.labelReportsTo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkSortDateCertComplete
			// 
			this.checkSortDateCertComplete.Location = new System.Drawing.Point(586, 6);
			this.checkSortDateCertComplete.Name = "checkSortDateCertComplete";
			this.checkSortDateCertComplete.Size = new System.Drawing.Size(150, 18);
			this.checkSortDateCertComplete.TabIndex = 54;
			this.checkSortDateCertComplete.Text = "Sort By Date Completed";
			this.checkSortDateCertComplete.UseVisualStyleBackColor = true;
			this.checkSortDateCertComplete.Click += new System.EventHandler(this.checkSortDateCertComplete_Click);
			// 
			// FormCertifications
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1095, 663);
			this.Controls.Add(this.checkSortDateCertComplete);
			this.Controls.Add(this.textEmpSearch);
			this.Controls.Add(this.butSetup);
			this.Controls.Add(this.labelReportsTo);
			this.Controls.Add(this.comboSupervisor);
			this.Controls.Add(this.checkSortDate);
			this.Controls.Add(this.checkIncomplete);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.labelEmpSearch);
			this.Controls.Add(this.labelCategories2);
			this.Controls.Add(this.listBoxCategories2);
			this.Controls.Add(this.labelCertification);
			this.Controls.Add(this.listBoxCertification);
			this.Controls.Add(this.groupBoxOrderBy);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelEmployee);
			this.Controls.Add(this.listBoxEmployee);
			this.Controls.Add(this.labelCategories);
			this.Controls.Add(this.listBoxCategories);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCertifications";
			this.Text = "Certifications";
			this.Load += new System.EventHandler(this.FormCertifications_Load);
			this.groupBoxOrderBy.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.ListBoxOD listBoxEmployee;
		private UI.GridOD gridMain;
		private UI.ListBoxOD listBoxCategories;
		private UI.Button butSetup;
		private System.Windows.Forms.Label labelEmployee;
		private System.Windows.Forms.Label labelCategories;
		private UI.GroupBoxOD groupBoxOrderBy;
		private System.Windows.Forms.RadioButton radioCategory;
		private System.Windows.Forms.RadioButton radioCertification;
		private System.Windows.Forms.Label labelCertification;
		private UI.ListBoxOD listBoxCertification;
		private System.Windows.Forms.Label labelEmpSearch;
		private System.Windows.Forms.TextBox textEmpSearch;
		private UI.Button butPrint;
		private System.Windows.Forms.CheckBox checkIncomplete;
		private System.Windows.Forms.CheckBox checkSortDate;
		private System.Windows.Forms.Label labelCategories2;
		private UI.ListBoxOD listBoxCategories2;
		private UI.ComboBoxOD comboSupervisor;
		private System.Windows.Forms.Label labelReportsTo;
		private System.Windows.Forms.CheckBox checkSortDateCertComplete;
	}
}