namespace OpenDental{
	partial class FormAdvertisingPatientList {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdvertisingPatientList));
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkUserQuery = new OpenDental.UI.CheckBox();
			this.butRefreshPatientFilters = new OpenDental.UI.Button();
			this.labelNumberPatientsSelected = new System.Windows.Forms.Label();
			this.labelPatsSelected = new System.Windows.Forms.Label();
			this.panelFilterControls = new OpenDental.UI.PanelOD();
			this.groupAppt = new OpenDental.UI.GroupBox();
			this.checkHidePatientsNotSeenSince = new OpenDental.UI.CheckBox();
			this.checkHidePatientsSeenSince = new OpenDental.UI.CheckBox();
			this.datePickerPatientsSeenSince = new OpenDental.UI.ODDatePicker();
			this.datePickerPatientsNotSeenSince = new OpenDental.UI.ODDatePicker();
			this.checkHiddenFutureAppt = new OpenDental.UI.CheckBox();
			this.labelPatBillingType = new System.Windows.Forms.Label();
			this.listBoxPatientBillingType = new OpenDental.UI.ListBox();
			this.labelPatStatus = new System.Windows.Forms.Label();
			this.labelContact = new System.Windows.Forms.Label();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.textPatientAgeTo = new OpenDental.ValidNum();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textPatientAgeFrom = new OpenDental.ValidNum();
			this.listBoxPatientStatuses = new OpenDental.UI.ListBox();
			this.listBoxContactMethods = new OpenDental.UI.ListBox();
			this.panelUserQuery = new OpenDental.UI.PanelOD();
			this.butFavorite = new OpenDental.UI.Button();
			this.labelUserQuery = new System.Windows.Forms.Label();
			this.textUserQuery = new System.Windows.Forms.TextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClearAll = new OpenDental.UI.Button();
			this.butClearSelected = new OpenDental.UI.Button();
			this.butSetSelected = new OpenDental.UI.Button();
			this.butSelectAll = new OpenDental.UI.Button();
			this.butCommitList = new OpenDental.UI.Button();
			this.panelFilterControls.SuspendLayout();
			this.groupAppt.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panelUserQuery.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboClinics
			// 
			this.comboClinics.HqDescription = "Headquarters";
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(6, 7);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.comboClinics.Size = new System.Drawing.Size(195, 22);
			this.comboClinics.TabIndex = 237;
			// 
			// checkUserQuery
			// 
			this.checkUserQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkUserQuery.Location = new System.Drawing.Point(7, 2);
			this.checkUserQuery.Name = "checkUserQuery";
			this.checkUserQuery.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkUserQuery.Size = new System.Drawing.Size(86, 17);
			this.checkUserQuery.TabIndex = 240;
			this.checkUserQuery.Text = "User Query";
			this.checkUserQuery.Click += new System.EventHandler(this.checkUserQuery_Click);
			// 
			// butRefreshPatientFilters
			// 
			this.butRefreshPatientFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefreshPatientFilters.Location = new System.Drawing.Point(504, 212);
			this.butRefreshPatientFilters.Name = "butRefreshPatientFilters";
			this.butRefreshPatientFilters.Size = new System.Drawing.Size(85, 24);
			this.butRefreshPatientFilters.TabIndex = 235;
			this.butRefreshPatientFilters.Text = "Refresh";
			this.butRefreshPatientFilters.Click += new System.EventHandler(this.butRefreshPatientFilters_Click);
			// 
			// labelNumberPatientsSelected
			// 
			this.labelNumberPatientsSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNumberPatientsSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNumberPatientsSelected.ForeColor = System.Drawing.Color.LimeGreen;
			this.labelNumberPatientsSelected.Location = new System.Drawing.Point(916, 688);
			this.labelNumberPatientsSelected.Name = "labelNumberPatientsSelected";
			this.labelNumberPatientsSelected.Size = new System.Drawing.Size(59, 17);
			this.labelNumberPatientsSelected.TabIndex = 233;
			this.labelNumberPatientsSelected.Text = "0";
			this.labelNumberPatientsSelected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPatsSelected
			// 
			this.labelPatsSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPatsSelected.Location = new System.Drawing.Point(916, 673);
			this.labelPatsSelected.Name = "labelPatsSelected";
			this.labelPatsSelected.Size = new System.Drawing.Size(150, 16);
			this.labelPatsSelected.TabIndex = 232;
			this.labelPatsSelected.Text = "# of Selected Patients:";
			this.labelPatsSelected.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelFilterControls
			// 
			this.panelFilterControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelFilterControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.panelFilterControls.Controls.Add(this.groupAppt);
			this.panelFilterControls.Controls.Add(this.labelPatBillingType);
			this.panelFilterControls.Controls.Add(this.listBoxPatientBillingType);
			this.panelFilterControls.Controls.Add(this.labelPatStatus);
			this.panelFilterControls.Controls.Add(this.labelContact);
			this.panelFilterControls.Controls.Add(this.groupBox1);
			this.panelFilterControls.Controls.Add(this.comboClinics);
			this.panelFilterControls.Controls.Add(this.listBoxPatientStatuses);
			this.panelFilterControls.Controls.Add(this.listBoxContactMethods);
			this.panelFilterControls.Location = new System.Drawing.Point(0, 18);
			this.panelFilterControls.Name = "panelFilterControls";
			this.panelFilterControls.Size = new System.Drawing.Size(498, 218);
			this.panelFilterControls.TabIndex = 238;
			this.panelFilterControls.Text = "Filters";
			// 
			// groupAppt
			// 
			this.groupAppt.Controls.Add(this.checkHidePatientsNotSeenSince);
			this.groupAppt.Controls.Add(this.checkHidePatientsSeenSince);
			this.groupAppt.Controls.Add(this.datePickerPatientsSeenSince);
			this.groupAppt.Controls.Add(this.datePickerPatientsNotSeenSince);
			this.groupAppt.Controls.Add(this.checkHiddenFutureAppt);
			this.groupAppt.Location = new System.Drawing.Point(110, 31);
			this.groupAppt.Name = "groupAppt";
			this.groupAppt.Size = new System.Drawing.Size(379, 82);
			this.groupAppt.TabIndex = 241;
			this.groupAppt.Text = "Appointments";
			// 
			// checkHidePatientsNotSeenSince
			// 
			this.checkHidePatientsNotSeenSince.Location = new System.Drawing.Point(3, 37);
			this.checkHidePatientsNotSeenSince.Name = "checkHidePatientsNotSeenSince";
			this.checkHidePatientsNotSeenSince.Size = new System.Drawing.Size(195, 18);
			this.checkHidePatientsNotSeenSince.TabIndex = 218;
			this.checkHidePatientsNotSeenSince.Text = "Exclude patients not seen since";
			// 
			// checkHidePatientsSeenSince
			// 
			this.checkHidePatientsSeenSince.Location = new System.Drawing.Point(3, 59);
			this.checkHidePatientsSeenSince.Name = "checkHidePatientsSeenSince";
			this.checkHidePatientsSeenSince.Size = new System.Drawing.Size(160, 18);
			this.checkHidePatientsSeenSince.TabIndex = 219;
			this.checkHidePatientsSeenSince.Text = "Exclude patients seen since";
			// 
			// datePickerPatientsSeenSince
			// 
			this.datePickerPatientsSeenSince.BackColor = System.Drawing.Color.Transparent;
			this.datePickerPatientsSeenSince.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.datePickerPatientsSeenSince.Location = new System.Drawing.Point(140, 55);
			this.datePickerPatientsSeenSince.MaximumSize = new System.Drawing.Size(0, 184);
			this.datePickerPatientsSeenSince.MinimumSize = new System.Drawing.Size(227, 23);
			this.datePickerPatientsSeenSince.Name = "datePickerPatientsSeenSince";
			this.datePickerPatientsSeenSince.Size = new System.Drawing.Size(227, 23);
			this.datePickerPatientsSeenSince.TabIndex = 223;
			// 
			// datePickerPatientsNotSeenSince
			// 
			this.datePickerPatientsNotSeenSince.BackColor = System.Drawing.Color.Transparent;
			this.datePickerPatientsNotSeenSince.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.datePickerPatientsNotSeenSince.Location = new System.Drawing.Point(140, 31);
			this.datePickerPatientsNotSeenSince.MaximumSize = new System.Drawing.Size(0, 184);
			this.datePickerPatientsNotSeenSince.MinimumSize = new System.Drawing.Size(227, 23);
			this.datePickerPatientsNotSeenSince.Name = "datePickerPatientsNotSeenSince";
			this.datePickerPatientsNotSeenSince.Size = new System.Drawing.Size(227, 23);
			this.datePickerPatientsNotSeenSince.TabIndex = 222;
			// 
			// checkHiddenFutureAppt
			// 
			this.checkHiddenFutureAppt.Location = new System.Drawing.Point(3, 16);
			this.checkHiddenFutureAppt.Name = "checkHiddenFutureAppt";
			this.checkHiddenFutureAppt.Size = new System.Drawing.Size(288, 18);
			this.checkHiddenFutureAppt.TabIndex = 213;
			this.checkHiddenFutureAppt.Text = "Hide patients with future appointments";
			// 
			// labelPatBillingType
			// 
			this.labelPatBillingType.Location = new System.Drawing.Point(335, 117);
			this.labelPatBillingType.Name = "labelPatBillingType";
			this.labelPatBillingType.Size = new System.Drawing.Size(154, 16);
			this.labelPatBillingType.TabIndex = 221;
			this.labelPatBillingType.Text = "Patient Billing Type";
			this.labelPatBillingType.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxPatientBillingType
			// 
			this.listBoxPatientBillingType.Location = new System.Drawing.Point(335, 136);
			this.listBoxPatientBillingType.Name = "listBoxPatientBillingType";
			this.listBoxPatientBillingType.Size = new System.Drawing.Size(154, 82);
			this.listBoxPatientBillingType.TabIndex = 220;
			// 
			// labelPatStatus
			// 
			this.labelPatStatus.Location = new System.Drawing.Point(175, 117);
			this.labelPatStatus.Name = "labelPatStatus";
			this.labelPatStatus.Size = new System.Drawing.Size(154, 16);
			this.labelPatStatus.TabIndex = 212;
			this.labelPatStatus.Text = "Patient Status";
			this.labelPatStatus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelContact
			// 
			this.labelContact.Location = new System.Drawing.Point(8, 117);
			this.labelContact.Name = "labelContact";
			this.labelContact.Size = new System.Drawing.Size(154, 16);
			this.labelContact.TabIndex = 211;
			this.labelContact.Text = "Preferred Contact Method";
			this.labelContact.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.groupBox1.Controls.Add(this.textPatientAgeTo);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textPatientAgeFrom);
			this.groupBox1.Location = new System.Drawing.Point(7, 31);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(97, 82);
			this.groupBox1.TabIndex = 217;
			this.groupBox1.Text = "Patient Age";
			// 
			// textPatientAgeTo
			// 
			this.textPatientAgeTo.Location = new System.Drawing.Point(51, 47);
			this.textPatientAgeTo.Name = "textPatientAgeTo";
			this.textPatientAgeTo.Size = new System.Drawing.Size(39, 20);
			this.textPatientAgeTo.TabIndex = 209;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 17);
			this.label3.TabIndex = 208;
			this.label3.Text = "From";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(28, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(23, 17);
			this.label4.TabIndex = 207;
			this.label4.Text = "To";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientAgeFrom
			// 
			this.textPatientAgeFrom.Location = new System.Drawing.Point(51, 24);
			this.textPatientAgeFrom.Name = "textPatientAgeFrom";
			this.textPatientAgeFrom.Size = new System.Drawing.Size(39, 20);
			this.textPatientAgeFrom.TabIndex = 130;
			// 
			// listBoxPatientStatuses
			// 
			this.listBoxPatientStatuses.Location = new System.Drawing.Point(175, 136);
			this.listBoxPatientStatuses.Name = "listBoxPatientStatuses";
			this.listBoxPatientStatuses.Size = new System.Drawing.Size(154, 82);
			this.listBoxPatientStatuses.TabIndex = 209;
			// 
			// listBoxContactMethods
			// 
			this.listBoxContactMethods.Location = new System.Drawing.Point(8, 136);
			this.listBoxContactMethods.Name = "listBoxContactMethods";
			this.listBoxContactMethods.Size = new System.Drawing.Size(161, 82);
			this.listBoxContactMethods.TabIndex = 208;
			// 
			// panelUserQuery
			// 
			this.panelUserQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.panelUserQuery.Controls.Add(this.butFavorite);
			this.panelUserQuery.Controls.Add(this.labelUserQuery);
			this.panelUserQuery.Controls.Add(this.textUserQuery);
			this.panelUserQuery.Location = new System.Drawing.Point(0, 18);
			this.panelUserQuery.Name = "panelUserQuery";
			this.panelUserQuery.Size = new System.Drawing.Size(498, 218);
			this.panelUserQuery.TabIndex = 239;
			this.panelUserQuery.Text = "User Query";
			this.panelUserQuery.Visible = false;
			// 
			// butFavorite
			// 
			this.butFavorite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butFavorite.Location = new System.Drawing.Point(405, 3);
			this.butFavorite.Name = "butFavorite";
			this.butFavorite.Size = new System.Drawing.Size(90, 23);
			this.butFavorite.TabIndex = 248;
			this.butFavorite.Text = "Favorites";
			this.butFavorite.Click += new System.EventHandler(this.butFavorite_Click);
			// 
			// labelUserQuery
			// 
			this.labelUserQuery.Location = new System.Drawing.Point(0, 0);
			this.labelUserQuery.Name = "labelUserQuery";
			this.labelUserQuery.Size = new System.Drawing.Size(329, 19);
			this.labelUserQuery.TabIndex = 225;
			this.labelUserQuery.Text = "Query must output a PatNum column.";
			this.labelUserQuery.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textUserQuery
			// 
			this.textUserQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textUserQuery.BackColor = System.Drawing.SystemColors.Window;
			this.textUserQuery.Location = new System.Drawing.Point(0, 31);
			this.textUserQuery.Multiline = true;
			this.textUserQuery.Name = "textUserQuery";
			this.textUserQuery.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textUserQuery.Size = new System.Drawing.Size(498, 187);
			this.textUserQuery.TabIndex = 224;
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasAutoWrappedHeaders = true;
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(3, 241);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1224, 429);
			this.gridMain.TabIndex = 242;
			this.gridMain.Title = "Available Patients";
			this.gridMain.TranslationName = "TableAvailablePatients";
			this.gridMain.WrapText = false;
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butClearAll
			// 
			this.butClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butClearAll.Location = new System.Drawing.Point(252, 676);
			this.butClearAll.Name = "butClearAll";
			this.butClearAll.Size = new System.Drawing.Size(75, 24);
			this.butClearAll.TabIndex = 246;
			this.butClearAll.Text = "Clear All";
			this.butClearAll.UseVisualStyleBackColor = true;
			this.butClearAll.Click += new System.EventHandler(this.butClearAll_Click);
			// 
			// butClearSelected
			// 
			this.butClearSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butClearSelected.Location = new System.Drawing.Point(160, 676);
			this.butClearSelected.Name = "butClearSelected";
			this.butClearSelected.Size = new System.Drawing.Size(86, 24);
			this.butClearSelected.TabIndex = 245;
			this.butClearSelected.Text = "Clear Selected";
			this.butClearSelected.UseVisualStyleBackColor = true;
			this.butClearSelected.Click += new System.EventHandler(this.butClearSelected_Click);
			// 
			// butSetSelected
			// 
			this.butSetSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSetSelected.Location = new System.Drawing.Point(79, 676);
			this.butSetSelected.Name = "butSetSelected";
			this.butSetSelected.Size = new System.Drawing.Size(75, 24);
			this.butSetSelected.TabIndex = 244;
			this.butSetSelected.Text = "Set Selected";
			this.butSetSelected.UseVisualStyleBackColor = true;
			this.butSetSelected.Click += new System.EventHandler(this.butSetSelected_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSelectAll.Location = new System.Drawing.Point(3, 676);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(70, 24);
			this.butSelectAll.TabIndex = 243;
			this.butSelectAll.Text = "Set All";
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// butCommitList
			// 
			this.butCommitList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCommitList.Location = new System.Drawing.Point(1152, 676);
			this.butCommitList.Name = "butCommitList";
			this.butCommitList.Size = new System.Drawing.Size(75, 24);
			this.butCommitList.TabIndex = 247;
			this.butCommitList.Text = "&OK";
			this.butCommitList.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormAdvertisingPatientList
			// 
			this.ClientSize = new System.Drawing.Size(1230, 707);
			this.Controls.Add(this.butRefreshPatientFilters);
			this.Controls.Add(this.butCommitList);
			this.Controls.Add(this.checkUserQuery);
			this.Controls.Add(this.butClearAll);
			this.Controls.Add(this.butClearSelected);
			this.Controls.Add(this.butSetSelected);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelNumberPatientsSelected);
			this.Controls.Add(this.labelPatsSelected);
			this.Controls.Add(this.panelFilterControls);
			this.Controls.Add(this.panelUserQuery);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAdvertisingPatientList";
			this.Text = "Advertising - Select Patients";
			this.Load += new System.EventHandler(this.FormMassPostcardList_Load);
			this.panelFilterControls.ResumeLayout(false);
			this.groupAppt.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panelUserQuery.ResumeLayout(false);
			this.panelUserQuery.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.ComboBoxClinicPicker comboClinics;
		private OpenDental.UI.CheckBox checkUserQuery;
		private UI.Button butRefreshPatientFilters;
		private System.Windows.Forms.Label labelNumberPatientsSelected;
		private System.Windows.Forms.Label labelPatsSelected;
		private UI.PanelOD panelFilterControls;
		private System.Windows.Forms.Label labelPatBillingType;
		private UI.ListBox listBoxPatientBillingType;
		private OpenDental.UI.CheckBox checkHidePatientsSeenSince;
		private System.Windows.Forms.Label labelPatStatus;
		private System.Windows.Forms.Label labelContact;
		private UI.GroupBox groupBox1;
		private ValidNum textPatientAgeTo;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private ValidNum textPatientAgeFrom;
		private UI.ListBox listBoxPatientStatuses;
		private UI.ListBox listBoxContactMethods;
		private OpenDental.UI.CheckBox checkHidePatientsNotSeenSince;
		private OpenDental.UI.CheckBox checkHiddenFutureAppt;
		private UI.ODDatePicker datePickerPatientsNotSeenSince;
		private UI.ODDatePicker datePickerPatientsSeenSince;
		private UI.PanelOD panelUserQuery;
		private System.Windows.Forms.Label labelUserQuery;
		private System.Windows.Forms.TextBox textUserQuery;
		private UI.GridOD gridMain;
		private UI.Button butClearAll;
		private UI.Button butClearSelected;
		private UI.Button butSetSelected;
		private UI.Button butSelectAll;
		private UI.Button butCommitList;
		private UI.GroupBox groupAppt;
		private UI.Button butFavorite;
	}
}