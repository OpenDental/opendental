namespace OpenDental{
	partial class FormFeeSchedGroups {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeeSchedGroups));
			this.butClose = new OpenDental.UI.Button();
			this.gridGroups = new OpenDental.UI.GridOD();
			this.gridClinics = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.groupFilters = new System.Windows.Forms.GroupBox();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelFeeSchedFilter = new System.Windows.Forms.Label();
			this.textFeeSched = new System.Windows.Forms.TextBox();
			this.groupFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(825, 661);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridGroups
			// 
			this.gridGroups.AllowSortingByColumn = true;
			this.gridGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridGroups.Location = new System.Drawing.Point(12, 71);
			this.gridGroups.Name = "gridGroups";
			this.gridGroups.Size = new System.Drawing.Size(440, 577);
			this.gridGroups.TabIndex = 4;
			this.gridGroups.Title = "Fee Schedule Groups";
			this.gridGroups.TranslationName = "Table Fee Schedule Groups";
			this.gridGroups.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridGroups_CellDoubleClick);
			this.gridGroups.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridGroups_CellClick);
			// 
			// gridClinics
			// 
			this.gridClinics.AllowSortingByColumn = true;
			this.gridClinics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridClinics.Location = new System.Drawing.Point(458, 71);
			this.gridClinics.Name = "gridClinics";
			this.gridClinics.Size = new System.Drawing.Size(442, 577);
			this.gridClinics.TabIndex = 5;
			this.gridClinics.Title = "Clinics in Group";
			this.gridClinics.TranslationName = "Table Clinics";
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Image = ((System.Drawing.Image)(resources.GetObject("butAdd.Image")));
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(10, 661);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(93, 23);
			this.butAdd.TabIndex = 220;
			this.butAdd.Text = "&Add Group";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// groupFilters
			// 
			this.groupFilters.Controls.Add(this.comboClinic);
			this.groupFilters.Controls.Add(this.label1);
			this.groupFilters.Controls.Add(this.labelFeeSchedFilter);
			this.groupFilters.Controls.Add(this.textFeeSched);
			this.groupFilters.Location = new System.Drawing.Point(12, 12);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(440, 53);
			this.groupFilters.TabIndex = 221;
			this.groupFilters.TabStop = false;
			this.groupFilters.Text = "Fee Schedule Group Filters";
			// 
			// comboClinic
			// 
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.FormattingEnabled = true;
			this.comboClinic.Location = new System.Drawing.Point(308, 20);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(126, 21);
			this.comboClinic.TabIndex = 3;
			this.comboClinic.SelectedIndexChanged += new System.EventHandler(this.comboClinic_SelectionChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(250, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 21);
			this.label1.TabIndex = 2;
			this.label1.Text = "Clinic";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFeeSchedFilter
			// 
			this.labelFeeSchedFilter.Location = new System.Drawing.Point(9, 22);
			this.labelFeeSchedFilter.Name = "labelFeeSchedFilter";
			this.labelFeeSchedFilter.Size = new System.Drawing.Size(86, 16);
			this.labelFeeSchedFilter.TabIndex = 1;
			this.labelFeeSchedFilter.Text = "Fee Schedule";
			this.labelFeeSchedFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFeeSched
			// 
			this.textFeeSched.Location = new System.Drawing.Point(101, 21);
			this.textFeeSched.Name = "textFeeSched";
			this.textFeeSched.Size = new System.Drawing.Size(126, 20);
			this.textFeeSched.TabIndex = 0;
			// 
			// FormFeeSchedGroups
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(910, 696);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridClinics);
			this.Controls.Add(this.gridGroups);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFeeSchedGroups";
			this.Text = "Fee Schedule Groups";
			this.Load += new System.EventHandler(this.FormFeeSchedGroups_Load);
			this.groupFilters.ResumeLayout(false);
			this.groupFilters.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridGroups;
		private UI.GridOD gridClinics;
		private UI.Button butAdd;
		private System.Windows.Forms.GroupBox groupFilters;
		private System.Windows.Forms.Label labelFeeSchedFilter;
		private System.Windows.Forms.TextBox textFeeSched;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.Label label1;
	}
}