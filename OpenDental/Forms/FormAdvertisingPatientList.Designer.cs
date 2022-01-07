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
			this.butCancel = new OpenDental.UI.Button();
			this.comboClinicPatient = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkUserQuery = new System.Windows.Forms.CheckBox();
			this.butRefreshPatientFilters = new OpenDental.UI.Button();
			this.labelNumberPats = new System.Windows.Forms.Label();
			this.labelPatsSelected = new System.Windows.Forms.Label();
			this.panelFilters = new OpenDental.UI.PanelOD();
			this.groupAppt = new OpenDental.UI.GroupBoxOD();
			this.checkHideNotSeenSince = new System.Windows.Forms.CheckBox();
			this.checkHideSeenSince = new System.Windows.Forms.CheckBox();
			this.datePickerSeenSince = new OpenDental.UI.ODDatePicker();
			this.datePickerNotSeenSince = new OpenDental.UI.ODDatePicker();
			this.checkHiddenFutureAppt = new System.Windows.Forms.CheckBox();
			this.labelPatBillingType = new System.Windows.Forms.Label();
			this.listBoxPatBillingType = new OpenDental.UI.ListBoxOD();
			this.labelPatStatus = new System.Windows.Forms.Label();
			this.labelContact = new System.Windows.Forms.Label();
			this.groupBox1 = new OpenDental.UI.GroupBoxOD();
			this.textAgeTo = new OpenDental.ValidNum();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textAgeFrom = new OpenDental.ValidNum();
			this.listBoxPatStatus = new OpenDental.UI.ListBoxOD();
			this.listBoxContactMethod = new OpenDental.UI.ListBoxOD();
			this.panelRefresh = new OpenDental.UI.PanelOD();
			this.panelAdditionalFilters = new OpenDental.UI.PanelOD();
			this.panelUserQuery = new OpenDental.UI.PanelOD();
			this.labelUserQuery = new System.Windows.Forms.Label();
			this.textUserQuery = new OpenDental.ODtextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClearAll = new OpenDental.UI.Button();
			this.butClearSelected = new OpenDental.UI.Button();
			this.butSetSelected = new OpenDental.UI.Button();
			this.butSelectAll = new OpenDental.UI.Button();
			this.butCommitList = new OpenDental.UI.Button();
			this.panelFilters.SuspendLayout();
			this.groupAppt.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panelRefresh.SuspendLayout();
			this.panelUserQuery.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1152, 676);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboClinicPatient
			// 
			this.comboClinicPatient.HqDescription = "Headquarters";
			this.comboClinicPatient.IncludeAll = true;
			this.comboClinicPatient.IncludeUnassigned = true;
			this.comboClinicPatient.Location = new System.Drawing.Point(6, 7);
			this.comboClinicPatient.Name = "comboClinicPatient";
			this.comboClinicPatient.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.comboClinicPatient.Size = new System.Drawing.Size(195, 22);
			this.comboClinicPatient.TabIndex = 237;
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
			this.checkUserQuery.UseVisualStyleBackColor = true;
			this.checkUserQuery.Click += new System.EventHandler(this.checkUserQuery_Click);
			// 
			// butRefreshPatientFilters
			// 
			this.butRefreshPatientFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefreshPatientFilters.Location = new System.Drawing.Point(639, 191);
			this.butRefreshPatientFilters.Name = "butRefreshPatientFilters";
			this.butRefreshPatientFilters.Size = new System.Drawing.Size(85, 24);
			this.butRefreshPatientFilters.TabIndex = 235;
			this.butRefreshPatientFilters.Text = "Refresh";
			this.butRefreshPatientFilters.Click += new System.EventHandler(this.butRefreshPatientFilters_Click);
			// 
			// labelNumberPats
			// 
			this.labelNumberPats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNumberPats.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNumberPats.ForeColor = System.Drawing.Color.LimeGreen;
			this.labelNumberPats.Location = new System.Drawing.Point(916, 688);
			this.labelNumberPats.Name = "labelNumberPats";
			this.labelNumberPats.Size = new System.Drawing.Size(59, 17);
			this.labelNumberPats.TabIndex = 233;
			this.labelNumberPats.Text = "0";
			this.labelNumberPats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			// panelFilters
			// 
			this.panelFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelFilters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.panelFilters.Controls.Add(this.groupAppt);
			this.panelFilters.Controls.Add(this.labelPatBillingType);
			this.panelFilters.Controls.Add(this.listBoxPatBillingType);
			this.panelFilters.Controls.Add(this.labelPatStatus);
			this.panelFilters.Controls.Add(this.labelContact);
			this.panelFilters.Controls.Add(this.groupBox1);
			this.panelFilters.Controls.Add(this.comboClinicPatient);
			this.panelFilters.Controls.Add(this.listBoxPatStatus);
			this.panelFilters.Controls.Add(this.listBoxContactMethod);
			this.panelFilters.Location = new System.Drawing.Point(0, 18);
			this.panelFilters.Name = "panelFilters";
			this.panelFilters.Size = new System.Drawing.Size(498, 218);
			this.panelFilters.TabIndex = 238;
			this.panelFilters.Text = "Filters";
			// 
			// groupAppt
			// 
			this.groupAppt.Controls.Add(this.checkHideNotSeenSince);
			this.groupAppt.Controls.Add(this.checkHideSeenSince);
			this.groupAppt.Controls.Add(this.datePickerSeenSince);
			this.groupAppt.Controls.Add(this.datePickerNotSeenSince);
			this.groupAppt.Controls.Add(this.checkHiddenFutureAppt);
			this.groupAppt.Location = new System.Drawing.Point(110, 31);
			this.groupAppt.Name = "groupAppt";
			this.groupAppt.Size = new System.Drawing.Size(379, 82);
			this.groupAppt.TabIndex = 241;
			this.groupAppt.Text = "Appointments";
			// 
			// checkHideNotSeenSince
			// 
			this.checkHideNotSeenSince.Location = new System.Drawing.Point(3, 37);
			this.checkHideNotSeenSince.Name = "checkHideNotSeenSince";
			this.checkHideNotSeenSince.Size = new System.Drawing.Size(195, 18);
			this.checkHideNotSeenSince.TabIndex = 218;
			this.checkHideNotSeenSince.Text = "Exclude patients not seen since";
			this.checkHideNotSeenSince.UseVisualStyleBackColor = true;
			// 
			// checkHideSeenSince
			// 
			this.checkHideSeenSince.Location = new System.Drawing.Point(3, 59);
			this.checkHideSeenSince.Name = "checkHideSeenSince";
			this.checkHideSeenSince.Size = new System.Drawing.Size(160, 18);
			this.checkHideSeenSince.TabIndex = 219;
			this.checkHideSeenSince.Text = "Exclude patients seen since";
			this.checkHideSeenSince.UseVisualStyleBackColor = true;
			// 
			// datePickerSeenSince
			// 
			this.datePickerSeenSince.BackColor = System.Drawing.Color.Transparent;
			this.datePickerSeenSince.Location = new System.Drawing.Point(140, 55);
			this.datePickerSeenSince.MaximumSize = new System.Drawing.Size(0, 184);
			this.datePickerSeenSince.MinimumSize = new System.Drawing.Size(227, 23);
			this.datePickerSeenSince.Name = "datePickerSeenSince";
			this.datePickerSeenSince.Size = new System.Drawing.Size(227, 23);
			this.datePickerSeenSince.TabIndex = 223;
			// 
			// datePickerNotSeenSince
			// 
			this.datePickerNotSeenSince.BackColor = System.Drawing.Color.Transparent;
			this.datePickerNotSeenSince.Location = new System.Drawing.Point(140, 31);
			this.datePickerNotSeenSince.MaximumSize = new System.Drawing.Size(0, 184);
			this.datePickerNotSeenSince.MinimumSize = new System.Drawing.Size(227, 23);
			this.datePickerNotSeenSince.Name = "datePickerNotSeenSince";
			this.datePickerNotSeenSince.Size = new System.Drawing.Size(227, 23);
			this.datePickerNotSeenSince.TabIndex = 222;
			// 
			// checkHiddenFutureAppt
			// 
			this.checkHiddenFutureAppt.Location = new System.Drawing.Point(3, 16);
			this.checkHiddenFutureAppt.Name = "checkHiddenFutureAppt";
			this.checkHiddenFutureAppt.Size = new System.Drawing.Size(288, 18);
			this.checkHiddenFutureAppt.TabIndex = 213;
			this.checkHiddenFutureAppt.Text = "Hide patients with future appointments";
			this.checkHiddenFutureAppt.UseVisualStyleBackColor = true;
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
			// listBoxPatBillingType
			// 
			this.listBoxPatBillingType.Location = new System.Drawing.Point(335, 136);
			this.listBoxPatBillingType.Name = "listBoxPatBillingType";
			this.listBoxPatBillingType.Size = new System.Drawing.Size(154, 82);
			this.listBoxPatBillingType.TabIndex = 220;
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
			this.groupBox1.Controls.Add(this.textAgeTo);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textAgeFrom);
			this.groupBox1.Location = new System.Drawing.Point(7, 31);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(97, 82);
			this.groupBox1.TabIndex = 217;
			this.groupBox1.Text = "Patient Age";
			// 
			// textAgeTo
			// 
			this.textAgeTo.Location = new System.Drawing.Point(51, 47);
			this.textAgeTo.Name = "textAgeTo";
			this.textAgeTo.Size = new System.Drawing.Size(39, 20);
			this.textAgeTo.TabIndex = 209;
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
			// textAgeFrom
			// 
			this.textAgeFrom.Location = new System.Drawing.Point(51, 24);
			this.textAgeFrom.Name = "textAgeFrom";
			this.textAgeFrom.Size = new System.Drawing.Size(39, 20);
			this.textAgeFrom.TabIndex = 130;
			// 
			// listBoxPatStatus
			// 
			this.listBoxPatStatus.Location = new System.Drawing.Point(175, 136);
			this.listBoxPatStatus.Name = "listBoxPatStatus";
			this.listBoxPatStatus.Size = new System.Drawing.Size(154, 82);
			this.listBoxPatStatus.TabIndex = 209;
			// 
			// listBoxContactMethod
			// 
			this.listBoxContactMethod.Location = new System.Drawing.Point(8, 136);
			this.listBoxContactMethod.Name = "listBoxContactMethod";
			this.listBoxContactMethod.Size = new System.Drawing.Size(161, 82);
			this.listBoxContactMethod.TabIndex = 208;
			// 
			// panelRefresh
			// 
			this.panelRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panelRefresh.Controls.Add(this.panelAdditionalFilters);
			this.panelRefresh.Controls.Add(this.butRefreshPatientFilters);
			this.panelRefresh.Location = new System.Drawing.Point(500, 18);
			this.panelRefresh.Name = "panelRefresh";
			this.panelRefresh.Size = new System.Drawing.Size(727, 218);
			this.panelRefresh.TabIndex = 242;
			// 
			// panelAdditionalFilters
			// 
			this.panelAdditionalFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelAdditionalFilters.Location = new System.Drawing.Point(0, 0);
			this.panelAdditionalFilters.Name = "panelAdditionalFilters";
			this.panelAdditionalFilters.Size = new System.Drawing.Size(638, 218);
			this.panelAdditionalFilters.TabIndex = 243;
			// 
			// panelUserQuery
			// 
			this.panelUserQuery.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.panelUserQuery.Controls.Add(this.labelUserQuery);
			this.panelUserQuery.Controls.Add(this.textUserQuery);
			this.panelUserQuery.Location = new System.Drawing.Point(0, 18);
			this.panelUserQuery.Name = "panelUserQuery";
			this.panelUserQuery.Size = new System.Drawing.Size(498, 218);
			this.panelUserQuery.TabIndex = 239;
			this.panelUserQuery.Text = "User Query";
			this.panelUserQuery.Visible = false;
			// 
			// labelUserQuery
			// 
			this.labelUserQuery.Location = new System.Drawing.Point(0, 0);
			this.labelUserQuery.Name = "labelUserQuery";
			this.labelUserQuery.Size = new System.Drawing.Size(473, 19);
			this.labelUserQuery.TabIndex = 225;
			this.labelUserQuery.Text = "Query must output a PatNum column.";
			this.labelUserQuery.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textUserQuery
			// 
			this.textUserQuery.AcceptsTab = true;
			this.textUserQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textUserQuery.BackColor = System.Drawing.SystemColors.Window;
			this.textUserQuery.DetectLinksEnabled = false;
			this.textUserQuery.DetectUrls = false;
			this.textUserQuery.Location = new System.Drawing.Point(0, 20);
			this.textUserQuery.Name = "textUserQuery";
			this.textUserQuery.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textUserQuery.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textUserQuery.Size = new System.Drawing.Size(498, 198);
			this.textUserQuery.TabIndex = 224;
			this.textUserQuery.Text = "";
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
			this.butCommitList.Location = new System.Drawing.Point(1072, 676);
			this.butCommitList.Name = "butCommitList";
			this.butCommitList.Size = new System.Drawing.Size(75, 24);
			this.butCommitList.TabIndex = 247;
			this.butCommitList.Text = "&OK";
			this.butCommitList.Click += new System.EventHandler(this.butCommitList_Click);
			// 
			// FormAdvertisingPatientList
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1230, 707);
			this.Controls.Add(this.panelRefresh);
			this.Controls.Add(this.butCommitList);
			this.Controls.Add(this.checkUserQuery);
			this.Controls.Add(this.butClearAll);
			this.Controls.Add(this.butClearSelected);
			this.Controls.Add(this.butSetSelected);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelNumberPats);
			this.Controls.Add(this.labelPatsSelected);
			this.Controls.Add(this.panelFilters);
			this.Controls.Add(this.panelUserQuery);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAdvertisingPatientList";
			this.Text = "Advertising - Select Patients";
			this.Load += new System.EventHandler(this.FormMassPostcardList_Load);
			this.panelFilters.ResumeLayout(false);
			this.groupAppt.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panelRefresh.ResumeLayout(false);
			this.panelUserQuery.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.ComboBoxClinicPicker comboClinicPatient;
		private System.Windows.Forms.CheckBox checkUserQuery;
		private UI.Button butRefreshPatientFilters;
		private System.Windows.Forms.Label labelNumberPats;
		private System.Windows.Forms.Label labelPatsSelected;
		private UI.PanelOD panelFilters;
		private System.Windows.Forms.Label labelPatBillingType;
		private UI.ListBoxOD listBoxPatBillingType;
		private System.Windows.Forms.CheckBox checkHideSeenSince;
		private System.Windows.Forms.Label labelPatStatus;
		private System.Windows.Forms.Label labelContact;
		private UI.GroupBoxOD groupBox1;
		private ValidNum textAgeTo;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private ValidNum textAgeFrom;
		private UI.ListBoxOD listBoxPatStatus;
		private UI.ListBoxOD listBoxContactMethod;
		private System.Windows.Forms.CheckBox checkHideNotSeenSince;
		private System.Windows.Forms.CheckBox checkHiddenFutureAppt;
		private UI.ODDatePicker datePickerNotSeenSince;
		private UI.ODDatePicker datePickerSeenSince;
		private UI.PanelOD panelUserQuery;
		private System.Windows.Forms.Label labelUserQuery;
		private ODtextBox textUserQuery;
		private UI.GridOD gridMain;
		private UI.Button butClearAll;
		private UI.Button butClearSelected;
		private UI.Button butSetSelected;
		private UI.Button butSelectAll;
		private UI.Button butCommitList;
		private UI.GroupBoxOD groupAppt;
		private UI.PanelOD panelRefresh;
		private UI.PanelOD panelAdditionalFilters;
	}
}