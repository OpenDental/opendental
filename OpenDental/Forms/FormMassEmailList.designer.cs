namespace OpenDental{
	partial class FormMassEmailList {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmailList));
			this.butClose = new OpenDental.UI.Button();
			this.labelPatsSelected = new System.Windows.Forms.Label();
			this.labelNumberPats = new System.Windows.Forms.Label();
			this.butSendEmails = new OpenDental.UI.Button();
			this.butSetupTemplates = new OpenDental.UI.Button();
			this.butSelectAll = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkHideSeenSince = new System.Windows.Forms.CheckBox();
			this.checkHideNotSeenSince = new System.Windows.Forms.CheckBox();
			this.labelRefreshNeeded = new System.Windows.Forms.Label();
			this.butRefreshPatientFilters = new OpenDental.UI.Button();
			this.checkHiddenFutureAppt = new System.Windows.Forms.CheckBox();
			this.datePickerNotSeenSince = new OpenDental.UI.ODDatePicker();
			this.datePickerSeenSince = new OpenDental.UI.ODDatePicker();
			this.textAgeTo = new OpenDental.ValidNum();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textAgeFrom = new OpenDental.ValidNum();
			this.comboClinicPatient = new OpenDental.UI.ComboBoxClinicPicker();
			this.label1 = new System.Windows.Forms.Label();
			this.lableDays = new System.Windows.Forms.Label();
			this.checkExcludeWithin = new System.Windows.Forms.CheckBox();
			this.labelPatBillingType = new System.Windows.Forms.Label();
			this.listBoxPatBillingType = new OpenDental.UI.ListBoxOD();
			this.labelPatStatus = new System.Windows.Forms.Label();
			this.labelContact = new System.Windows.Forms.Label();
			this.groupBoxRecipients = new System.Windows.Forms.GroupBox();
			this.textNumDays = new OpenDental.ValidNum();
			this.listBoxPatStatus = new OpenDental.UI.ListBoxOD();
			this.listBoxContactMethod = new OpenDental.UI.ListBoxOD();
			this.groupBoxFilters = new System.Windows.Forms.GroupBox();
			this.groupBoxUserQuery = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textUserQuery = new OpenDental.ODtextBox();
			this.comboEmailHostingTemplate = new OpenDental.UI.ComboBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.textEmailPreview = new OpenDental.ODtextBox();
			this.checkUserQuery = new System.Windows.Forms.CheckBox();
			this.butSetSelected = new OpenDental.UI.Button();
			this.butClearSelected = new OpenDental.UI.Button();
			this.butClearAll = new OpenDental.UI.Button();
			this.butPreviewTemplate = new OpenDental.UI.Button();
			this.labelNotEnabled = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBoxRecipients.SuspendLayout();
			this.groupBoxFilters.SuspendLayout();
			this.groupBoxUserQuery.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1102, 671);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(70, 24);
			this.butClose.TabIndex = 10;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelPatsSelected
			// 
			this.labelPatsSelected.Location = new System.Drawing.Point(306, 9);
			this.labelPatsSelected.Name = "labelPatsSelected";
			this.labelPatsSelected.Size = new System.Drawing.Size(150, 16);
			this.labelPatsSelected.TabIndex = 92;
			this.labelPatsSelected.Text = "# of Selected Patients:";
			this.labelPatsSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelNumberPats
			// 
			this.labelNumberPats.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNumberPats.ForeColor = System.Drawing.Color.LimeGreen;
			this.labelNumberPats.Location = new System.Drawing.Point(462, 7);
			this.labelNumberPats.Name = "labelNumberPats";
			this.labelNumberPats.Size = new System.Drawing.Size(59, 17);
			this.labelNumberPats.TabIndex = 128;
			this.labelNumberPats.Text = "0";
			this.labelNumberPats.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butSendEmails
			// 
			this.butSendEmails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSendEmails.Location = new System.Drawing.Point(998, 671);
			this.butSendEmails.Name = "butSendEmails";
			this.butSendEmails.Size = new System.Drawing.Size(98, 24);
			this.butSendEmails.TabIndex = 130;
			this.butSendEmails.Text = "Prepare to Send";
			this.butSendEmails.Click += new System.EventHandler(this.butSendEmails_Click);
			// 
			// butSetupTemplates
			// 
			this.butSetupTemplates.Location = new System.Drawing.Point(3, 4);
			this.butSetupTemplates.Name = "butSetupTemplates";
			this.butSetupTemplates.Size = new System.Drawing.Size(95, 24);
			this.butSetupTemplates.TabIndex = 131;
			this.butSetupTemplates.Text = "Setup Templates";
			this.butSetupTemplates.UseVisualStyleBackColor = true;
			this.butSetupTemplates.Click += new System.EventHandler(this.butSetupTemplates_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSelectAll.Location = new System.Drawing.Point(527, 671);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(70, 24);
			this.butSelectAll.TabIndex = 217;
			this.butSelectAll.Text = "Set All";
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasAutoWrappedHeaders = true;
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(527, 4);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(645, 661);
			this.gridMain.TabIndex = 213;
			this.gridMain.Title = "Available Patients";
			this.gridMain.TranslationName = "TableAvailablePatients";
			this.gridMain.WrapText = false;
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// checkHideSeenSince
			// 
			this.checkHideSeenSince.Location = new System.Drawing.Point(13, 239);
			this.checkHideSeenSince.Name = "checkHideSeenSince";
			this.checkHideSeenSince.Size = new System.Drawing.Size(160, 18);
			this.checkHideSeenSince.TabIndex = 219;
			this.checkHideSeenSince.Text = "Exclude patients seen since";
			this.checkHideSeenSince.UseVisualStyleBackColor = true;
			this.checkHideSeenSince.Click += new System.EventHandler(this.checkBoxHideSeenSince_Click);
			// 
			// checkHideNotSeenSince
			// 
			this.checkHideNotSeenSince.Location = new System.Drawing.Point(13, 215);
			this.checkHideNotSeenSince.Name = "checkHideNotSeenSince";
			this.checkHideNotSeenSince.Size = new System.Drawing.Size(195, 18);
			this.checkHideNotSeenSince.TabIndex = 218;
			this.checkHideNotSeenSince.Text = "Exclude patients not seen since";
			this.checkHideNotSeenSince.UseVisualStyleBackColor = true;
			this.checkHideNotSeenSince.Click += new System.EventHandler(this.checkHideNotSeenSince_Click);
			// 
			// labelRefreshNeeded
			// 
			this.labelRefreshNeeded.ForeColor = System.Drawing.Color.Firebrick;
			this.labelRefreshNeeded.Location = new System.Drawing.Point(228, 348);
			this.labelRefreshNeeded.Name = "labelRefreshNeeded";
			this.labelRefreshNeeded.Size = new System.Drawing.Size(194, 19);
			this.labelRefreshNeeded.TabIndex = 215;
			this.labelRefreshNeeded.Text = "Filters changed, refresh needed";
			this.labelRefreshNeeded.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelRefreshNeeded.Visible = false;
			// 
			// butRefreshPatientFilters
			// 
			this.butRefreshPatientFilters.Location = new System.Drawing.Point(427, 345);
			this.butRefreshPatientFilters.Name = "butRefreshPatientFilters";
			this.butRefreshPatientFilters.Size = new System.Drawing.Size(85, 24);
			this.butRefreshPatientFilters.TabIndex = 214;
			this.butRefreshPatientFilters.Text = "Refresh";
			this.butRefreshPatientFilters.Click += new System.EventHandler(this.butRefreshPatientFilters_Click);
			// 
			// checkHiddenFutureAppt
			// 
			this.checkHiddenFutureAppt.Location = new System.Drawing.Point(13, 191);
			this.checkHiddenFutureAppt.Name = "checkHiddenFutureAppt";
			this.checkHiddenFutureAppt.Size = new System.Drawing.Size(288, 18);
			this.checkHiddenFutureAppt.TabIndex = 213;
			this.checkHiddenFutureAppt.Text = "Hide patients with future appointments";
			this.checkHiddenFutureAppt.UseVisualStyleBackColor = true;
			this.checkHiddenFutureAppt.Click += new System.EventHandler(this.checkHiddenFutureAppt_Click);
			// 
			// datePickerNotSeenSince
			// 
			this.datePickerNotSeenSince.BackColor = System.Drawing.Color.Transparent;
			this.datePickerNotSeenSince.Location = new System.Drawing.Point(150, 209);
			this.datePickerNotSeenSince.MaximumSize = new System.Drawing.Size(0, 184);
			this.datePickerNotSeenSince.MinimumSize = new System.Drawing.Size(227, 23);
			this.datePickerNotSeenSince.Name = "datePickerNotSeenSince";
			this.datePickerNotSeenSince.Size = new System.Drawing.Size(227, 23);
			this.datePickerNotSeenSince.TabIndex = 222;
			// 
			// datePickerSeenSince
			// 
			this.datePickerSeenSince.BackColor = System.Drawing.Color.Transparent;
			this.datePickerSeenSince.Location = new System.Drawing.Point(150, 235);
			this.datePickerSeenSince.MaximumSize = new System.Drawing.Size(0, 184);
			this.datePickerSeenSince.MinimumSize = new System.Drawing.Size(227, 23);
			this.datePickerSeenSince.Name = "datePickerSeenSince";
			this.datePickerSeenSince.Size = new System.Drawing.Size(227, 23);
			this.datePickerSeenSince.TabIndex = 223;
			// 
			// textAgeTo
			// 
			this.textAgeTo.Location = new System.Drawing.Point(124, 26);
			this.textAgeTo.Name = "textAgeTo";
			this.textAgeTo.Size = new System.Drawing.Size(39, 20);
			this.textAgeTo.TabIndex = 209;
			this.textAgeTo.TextChanged += new System.EventHandler(this.textAgeTo_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textAgeTo);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textAgeFrom);
			this.groupBox1.Location = new System.Drawing.Point(13, 123);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(176, 62);
			this.groupBox1.TabIndex = 217;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Patient Age";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 17);
			this.label3.TabIndex = 208;
			this.label3.Text = "From";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(101, 26);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(23, 17);
			this.label4.TabIndex = 207;
			this.label4.Text = "To";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAgeFrom
			// 
			this.textAgeFrom.Location = new System.Drawing.Point(51, 26);
			this.textAgeFrom.Name = "textAgeFrom";
			this.textAgeFrom.Size = new System.Drawing.Size(39, 20);
			this.textAgeFrom.TabIndex = 130;
			this.textAgeFrom.TextChanged += new System.EventHandler(this.textAgeFrom_TextChanged);
			// 
			// comboClinicPatient
			// 
			this.comboClinicPatient.HqDescription = "Headquarters";
			this.comboClinicPatient.IncludeAll = true;
			this.comboClinicPatient.IncludeUnassigned = true;
			this.comboClinicPatient.Location = new System.Drawing.Point(6, 315);
			this.comboClinicPatient.Name = "comboClinicPatient";
			this.comboClinicPatient.Size = new System.Drawing.Size(200, 22);
			this.comboClinicPatient.TabIndex = 216;
			this.comboClinicPatient.SelectionChangeCommitted += new System.EventHandler(this.comboClinicPatient_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(31, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 18);
			this.label1.TabIndex = 208;
			this.label1.Text = "In the last";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lableDays
			// 
			this.lableDays.Location = new System.Drawing.Point(135, 39);
			this.lableDays.Name = "lableDays";
			this.lableDays.Size = new System.Drawing.Size(61, 18);
			this.lableDays.TabIndex = 207;
			this.lableDays.Text = "days";
			this.lableDays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkExcludeWithin
			// 
			this.checkExcludeWithin.Location = new System.Drawing.Point(6, 19);
			this.checkExcludeWithin.Name = "checkExcludeWithin";
			this.checkExcludeWithin.Size = new System.Drawing.Size(266, 18);
			this.checkExcludeWithin.TabIndex = 129;
			this.checkExcludeWithin.Text = "Exclude patients who received a mass email\r\n";
			this.checkExcludeWithin.UseVisualStyleBackColor = true;
			this.checkExcludeWithin.Click += new System.EventHandler(this.checkExcludeWithin_Click);
			// 
			// labelPatBillingType
			// 
			this.labelPatBillingType.Location = new System.Drawing.Point(342, 16);
			this.labelPatBillingType.Name = "labelPatBillingType";
			this.labelPatBillingType.Size = new System.Drawing.Size(154, 16);
			this.labelPatBillingType.TabIndex = 221;
			this.labelPatBillingType.Text = "Patient Billing Type";
			this.labelPatBillingType.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxPatBillingType
			// 
			this.listBoxPatBillingType.Location = new System.Drawing.Point(342, 35);
			this.listBoxPatBillingType.Name = "listBoxPatBillingType";
			this.listBoxPatBillingType.Size = new System.Drawing.Size(154, 82);
			this.listBoxPatBillingType.TabIndex = 220;
			this.listBoxPatBillingType.SelectedIndexChanged += new System.EventHandler(this.listBoxPatBillingType_SelectedIndexChanged);
			// 
			// labelPatStatus
			// 
			this.labelPatStatus.Location = new System.Drawing.Point(181, 16);
			this.labelPatStatus.Name = "labelPatStatus";
			this.labelPatStatus.Size = new System.Drawing.Size(154, 16);
			this.labelPatStatus.TabIndex = 212;
			this.labelPatStatus.Text = "Patient Status";
			this.labelPatStatus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelContact
			// 
			this.labelContact.Location = new System.Drawing.Point(13, 16);
			this.labelContact.Name = "labelContact";
			this.labelContact.Size = new System.Drawing.Size(154, 16);
			this.labelContact.TabIndex = 211;
			this.labelContact.Text = "Preferred Contact Method";
			this.labelContact.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBoxRecipients
			// 
			this.groupBoxRecipients.Controls.Add(this.label1);
			this.groupBoxRecipients.Controls.Add(this.lableDays);
			this.groupBoxRecipients.Controls.Add(this.textNumDays);
			this.groupBoxRecipients.Controls.Add(this.checkExcludeWithin);
			this.groupBoxRecipients.Location = new System.Drawing.Point(195, 123);
			this.groupBoxRecipients.Name = "groupBoxRecipients";
			this.groupBoxRecipients.Size = new System.Drawing.Size(275, 62);
			this.groupBoxRecipients.TabIndex = 210;
			this.groupBoxRecipients.TabStop = false;
			this.groupBoxRecipients.Text = "Mass Email Recipients";
			// 
			// textNumDays
			// 
			this.textNumDays.Location = new System.Drawing.Point(93, 38);
			this.textNumDays.Name = "textNumDays";
			this.textNumDays.Size = new System.Drawing.Size(39, 20);
			this.textNumDays.TabIndex = 130;
			this.textNumDays.TextChanged += new System.EventHandler(this.textNumDays_TextChanged);
			// 
			// listBoxPatStatus
			// 
			this.listBoxPatStatus.Location = new System.Drawing.Point(181, 35);
			this.listBoxPatStatus.Name = "listBoxPatStatus";
			this.listBoxPatStatus.Size = new System.Drawing.Size(154, 82);
			this.listBoxPatStatus.TabIndex = 209;
			this.listBoxPatStatus.SelectedIndexChanged += new System.EventHandler(this.listBoxPatStatus_SelectedIndexChanged);
			// 
			// listBoxContactMethod
			// 
			this.listBoxContactMethod.Location = new System.Drawing.Point(13, 35);
			this.listBoxContactMethod.Name = "listBoxContactMethod";
			this.listBoxContactMethod.Size = new System.Drawing.Size(161, 82);
			this.listBoxContactMethod.TabIndex = 208;
			this.listBoxContactMethod.SelectedIndexChanged += new System.EventHandler(this.listBoxContactMethod_SelectedIndexChanged);
			// 
			// groupBoxFilters
			// 
			this.groupBoxFilters.Controls.Add(this.labelPatBillingType);
			this.groupBoxFilters.Controls.Add(this.listBoxPatBillingType);
			this.groupBoxFilters.Controls.Add(this.checkHideSeenSince);
			this.groupBoxFilters.Controls.Add(this.labelPatStatus);
			this.groupBoxFilters.Controls.Add(this.labelContact);
			this.groupBoxFilters.Controls.Add(this.groupBox1);
			this.groupBoxFilters.Controls.Add(this.listBoxPatStatus);
			this.groupBoxFilters.Controls.Add(this.listBoxContactMethod);
			this.groupBoxFilters.Controls.Add(this.checkHideNotSeenSince);
			this.groupBoxFilters.Controls.Add(this.groupBoxRecipients);
			this.groupBoxFilters.Controls.Add(this.checkHiddenFutureAppt);
			this.groupBoxFilters.Controls.Add(this.datePickerNotSeenSince);
			this.groupBoxFilters.Controls.Add(this.datePickerSeenSince);
			this.groupBoxFilters.Location = new System.Drawing.Point(3, 34);
			this.groupBoxFilters.Name = "groupBoxFilters";
			this.groupBoxFilters.Size = new System.Drawing.Size(509, 275);
			this.groupBoxFilters.TabIndex = 219;
			this.groupBoxFilters.TabStop = false;
			this.groupBoxFilters.Text = "Filters";
			// 
			// groupBoxUserQuery
			// 
			this.groupBoxUserQuery.Controls.Add(this.label5);
			this.groupBoxUserQuery.Controls.Add(this.textUserQuery);
			this.groupBoxUserQuery.Location = new System.Drawing.Point(3, 34);
			this.groupBoxUserQuery.Name = "groupBoxUserQuery";
			this.groupBoxUserQuery.Size = new System.Drawing.Size(509, 275);
			this.groupBoxUserQuery.TabIndex = 225;
			this.groupBoxUserQuery.TabStop = false;
			this.groupBoxUserQuery.Text = "User Query";
			this.groupBoxUserQuery.Visible = false;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 18);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(473, 19);
			this.label5.TabIndex = 225;
			this.label5.Text = "You would not normally use a user query.  If you do, you must include a PatNum co" +
    "lumn.";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textUserQuery
			// 
			this.textUserQuery.AcceptsTab = true;
			this.textUserQuery.BackColor = System.Drawing.SystemColors.Window;
			this.textUserQuery.DetectLinksEnabled = false;
			this.textUserQuery.DetectUrls = false;
			this.textUserQuery.Location = new System.Drawing.Point(6, 39);
			this.textUserQuery.Name = "textUserQuery";
			this.textUserQuery.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textUserQuery.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textUserQuery.Size = new System.Drawing.Size(497, 226);
			this.textUserQuery.TabIndex = 224;
			this.textUserQuery.Text = "";
			// 
			// comboEmailHostingTemplate
			// 
			this.comboEmailHostingTemplate.Location = new System.Drawing.Point(95, 377);
			this.comboEmailHostingTemplate.Name = "comboEmailHostingTemplate";
			this.comboEmailHostingTemplate.Size = new System.Drawing.Size(243, 21);
			this.comboEmailHostingTemplate.TabIndex = 220;
			this.comboEmailHostingTemplate.Text = "combo";
			this.comboEmailHostingTemplate.SelectionChangeCommitted += new System.EventHandler(this.comboEmailHostingTemplate_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 379);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 18);
			this.label2.TabIndex = 221;
			this.label2.Text = "Email Template";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmailPreview
			// 
			this.textEmailPreview.AcceptsTab = true;
			this.textEmailPreview.BackColor = System.Drawing.SystemColors.Control;
			this.textEmailPreview.DetectLinksEnabled = false;
			this.textEmailPreview.DetectUrls = false;
			this.textEmailPreview.Location = new System.Drawing.Point(9, 404);
			this.textEmailPreview.Name = "textEmailPreview";
			this.textEmailPreview.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textEmailPreview.ReadOnly = true;
			this.textEmailPreview.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textEmailPreview.Size = new System.Drawing.Size(497, 261);
			this.textEmailPreview.TabIndex = 222;
			this.textEmailPreview.Text = "";
			// 
			// checkUserQuery
			// 
			this.checkUserQuery.Location = new System.Drawing.Point(9, 345);
			this.checkUserQuery.Name = "checkUserQuery";
			this.checkUserQuery.Size = new System.Drawing.Size(131, 17);
			this.checkUserQuery.TabIndex = 226;
			this.checkUserQuery.Text = "Use User Query";
			this.checkUserQuery.UseVisualStyleBackColor = true;
			this.checkUserQuery.Click += new System.EventHandler(this.checkUserQuery_Click);
			// 
			// butSetSelected
			// 
			this.butSetSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSetSelected.Location = new System.Drawing.Point(603, 671);
			this.butSetSelected.Name = "butSetSelected";
			this.butSetSelected.Size = new System.Drawing.Size(75, 24);
			this.butSetSelected.TabIndex = 227;
			this.butSetSelected.Text = "Set Selected";
			this.butSetSelected.UseVisualStyleBackColor = true;
			this.butSetSelected.Click += new System.EventHandler(this.butSetSelected_Click);
			// 
			// butClearSelected
			// 
			this.butClearSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butClearSelected.Location = new System.Drawing.Point(684, 671);
			this.butClearSelected.Name = "butClearSelected";
			this.butClearSelected.Size = new System.Drawing.Size(86, 24);
			this.butClearSelected.TabIndex = 228;
			this.butClearSelected.Text = "Clear Selected";
			this.butClearSelected.UseVisualStyleBackColor = true;
			this.butClearSelected.Click += new System.EventHandler(this.butClearSelected_Click);
			// 
			// butClearAll
			// 
			this.butClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butClearAll.Location = new System.Drawing.Point(776, 671);
			this.butClearAll.Name = "butClearAll";
			this.butClearAll.Size = new System.Drawing.Size(75, 24);
			this.butClearAll.TabIndex = 229;
			this.butClearAll.Text = "Clear All";
			this.butClearAll.UseVisualStyleBackColor = true;
			this.butClearAll.Click += new System.EventHandler(this.butClearAll_Click);
			// 
			// butPreviewTemplate
			// 
			this.butPreviewTemplate.Location = new System.Drawing.Point(412, 376);
			this.butPreviewTemplate.Name = "butPreviewTemplate";
			this.butPreviewTemplate.Size = new System.Drawing.Size(100, 24);
			this.butPreviewTemplate.TabIndex = 230;
			this.butPreviewTemplate.Text = "Preview Template";
			this.butPreviewTemplate.Click += new System.EventHandler(this.butPreviewTemplate_Click);
			// 
			// labelNotEnabled
			// 
			this.labelNotEnabled.ForeColor = System.Drawing.Color.Red;
			this.labelNotEnabled.Location = new System.Drawing.Point(217, 318);
			this.labelNotEnabled.Name = "labelNotEnabled";
			this.labelNotEnabled.Size = new System.Drawing.Size(295, 17);
			this.labelNotEnabled.TabIndex = 231;
			this.labelNotEnabled.Text = "* Clinic is not enabled";
			this.labelNotEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormMassEmailList
			// 
			this.ClientSize = new System.Drawing.Size(1184, 707);
			this.Controls.Add(this.labelNotEnabled);
			this.Controls.Add(this.butPreviewTemplate);
			this.Controls.Add(this.butClearAll);
			this.Controls.Add(this.butClearSelected);
			this.Controls.Add(this.butSetSelected);
			this.Controls.Add(this.comboClinicPatient);
			this.Controls.Add(this.checkUserQuery);
			this.Controls.Add(this.textEmailPreview);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboEmailHostingTemplate);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelRefreshNeeded);
			this.Controls.Add(this.butSetupTemplates);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butRefreshPatientFilters);
			this.Controls.Add(this.butSendEmails);
			this.Controls.Add(this.labelNumberPats);
			this.Controls.Add(this.labelPatsSelected);
			this.Controls.Add(this.groupBoxFilters);
			this.Controls.Add(this.groupBoxUserQuery);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmailList";
			this.Text = "Mass Email List";
			this.Load += new System.EventHandler(this.FormMassEmail_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBoxRecipients.ResumeLayout(false);
			this.groupBoxRecipients.PerformLayout();
			this.groupBoxFilters.ResumeLayout(false);
			this.groupBoxUserQuery.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
    private UI.Button butClose;
		private System.Windows.Forms.Label labelPatsSelected;
		private System.Windows.Forms.Label labelNumberPats;
		private UI.Button butSendEmails;
		private UI.Button butSetupTemplates;
		private UI.Button butSelectAll;
		private UI.GridOD gridMain;
		private System.Windows.Forms.CheckBox checkHideSeenSince;
		private System.Windows.Forms.CheckBox checkHideNotSeenSince;
		private System.Windows.Forms.Label labelRefreshNeeded;
		private UI.Button butRefreshPatientFilters;
		private System.Windows.Forms.CheckBox checkHiddenFutureAppt;
		private UI.ODDatePicker datePickerNotSeenSince;
		private UI.ODDatePicker datePickerSeenSince;
		private ValidNum textAgeTo;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private ValidNum textAgeFrom;
		private UI.ComboBoxClinicPicker comboClinicPatient;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lableDays;
		private System.Windows.Forms.CheckBox checkExcludeWithin;
		private System.Windows.Forms.Label labelPatBillingType;
		private UI.ListBoxOD listBoxPatBillingType;
		private System.Windows.Forms.Label labelPatStatus;
		private System.Windows.Forms.Label labelContact;
		private System.Windows.Forms.GroupBox groupBoxRecipients;
		private ValidNum textNumDays;
		private UI.ListBoxOD listBoxPatStatus;
		private UI.ListBoxOD listBoxContactMethod;
		private System.Windows.Forms.GroupBox groupBoxFilters;
		private UI.ComboBoxOD comboEmailHostingTemplate;
		private System.Windows.Forms.Label label2;
		private ODtextBox textEmailPreview;
		private System.Windows.Forms.GroupBox groupBoxUserQuery;
		private ODtextBox textUserQuery;
		private System.Windows.Forms.CheckBox checkUserQuery;
		private UI.Button butSetSelected;
		private UI.Button butClearSelected;
		private UI.Button butClearAll;
		private System.Windows.Forms.Label label5;
		private UI.Button butPreviewTemplate;
		private System.Windows.Forms.Label labelNotEnabled;
	}
}