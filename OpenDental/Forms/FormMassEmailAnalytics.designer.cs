namespace OpenDental{
	partial class FormMassEmailAnalytics {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmailAnalytics));
			this.comboClinicAnalytics = new OpenDental.UI.ComboBoxClinicPicker();
			this.butRefreshAnalytics = new OpenDental.UI.Button();
			this.gridAnalytics = new OpenDental.UI.GridOD();
			this.dateRangeAnalytics = new OpenDental.UI.ODDateRangePicker();
			this.labelNotActivated = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboClinicAnalytics
			// 
			this.comboClinicAnalytics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinicAnalytics.HqDescription = "Headquarters";
			this.comboClinicAnalytics.IncludeUnassigned = true;
			this.comboClinicAnalytics.Location = new System.Drawing.Point(808, 18);
			this.comboClinicAnalytics.Name = "comboClinicAnalytics";
			this.comboClinicAnalytics.Size = new System.Drawing.Size(200, 21);
			this.comboClinicAnalytics.TabIndex = 7;
			this.comboClinicAnalytics.SelectionChangeCommitted += new System.EventHandler(this.comboClinicAnalytics_SelectionChangeCommitted);
			// 
			// butRefreshAnalytics
			// 
			this.butRefreshAnalytics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefreshAnalytics.Location = new System.Drawing.Point(1065, 15);
			this.butRefreshAnalytics.Name = "butRefreshAnalytics";
			this.butRefreshAnalytics.Size = new System.Drawing.Size(87, 24);
			this.butRefreshAnalytics.TabIndex = 6;
			this.butRefreshAnalytics.Text = "Refresh";
			this.butRefreshAnalytics.UseVisualStyleBackColor = true;
			this.butRefreshAnalytics.Click += new System.EventHandler(this.butRefreshAnalytics_Click);
			// 
			// gridAnalytics
			// 
			this.gridAnalytics.AllowSelection = false;
			this.gridAnalytics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAnalytics.Location = new System.Drawing.Point(13, 50);
			this.gridAnalytics.Name = "gridAnalytics";
			this.gridAnalytics.Size = new System.Drawing.Size(1139, 594);
			this.gridAnalytics.TabIndex = 5;
			this.gridAnalytics.Title = "Analytics";
			this.gridAnalytics.TranslationName = "GridAnalytics";
			// 
			// dateRangeAnalytics
			// 
			this.dateRangeAnalytics.BackColor = System.Drawing.Color.Transparent;
			this.dateRangeAnalytics.Location = new System.Drawing.Point(13, 15);
			this.dateRangeAnalytics.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangeAnalytics.Name = "dateRangeAnalytics";
			this.dateRangeAnalytics.Size = new System.Drawing.Size(453, 24);
			this.dateRangeAnalytics.TabIndex = 4;
			// 
			// labelNotActivated
			// 
			this.labelNotActivated.ForeColor = System.Drawing.Color.Red;
			this.labelNotActivated.Location = new System.Drawing.Point(639, 19);
			this.labelNotActivated.Name = "labelNotActivated";
			this.labelNotActivated.Size = new System.Drawing.Size(163, 17);
			this.labelNotActivated.TabIndex = 325;
			this.labelNotActivated.Text = "* Clinic is not signed up";
			this.labelNotActivated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormMassEmailAnalytics
			// 
			this.ClientSize = new System.Drawing.Size(1164, 656);
			this.Controls.Add(this.labelNotActivated);
			this.Controls.Add(this.comboClinicAnalytics);
			this.Controls.Add(this.butRefreshAnalytics);
			this.Controls.Add(this.gridAnalytics);
			this.Controls.Add(this.dateRangeAnalytics);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmailAnalytics";
			this.Text = "Mass Email Analytics";
			this.Load += new System.EventHandler(this.FormMassEmailAnalytics_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.ComboBoxClinicPicker comboClinicAnalytics;
		private UI.Button butRefreshAnalytics;
		private UI.GridOD gridAnalytics;
		private UI.ODDateRangePicker dateRangeAnalytics;
		private System.Windows.Forms.Label labelNotActivated;
	}
}