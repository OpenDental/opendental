namespace OpenDental{
	partial class FormEServicesWebSchedRecall {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesWebSchedRecall));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupWebSchedProvRule = new System.Windows.Forms.GroupBox();
			this.butProvRulePickClinic = new OpenDental.UI.Button();
			this.checkUseDefaultProvRule = new System.Windows.Forms.CheckBox();
			this.comboClinicProvRule = new OpenDental.UI.ComboBoxClinicPicker();
			this.listBoxWebSchedProviderPref = new OpenDental.UI.ListBoxOD();
			this.label21 = new System.Windows.Forms.Label();
			this.checkWSRDoubleBooking = new System.Windows.Forms.CheckBox();
			this.comboWSRConfirmStatus = new OpenDental.UI.ComboBoxOD();
			this.label36 = new System.Windows.Forms.Label();
			this.label43 = new System.Windows.Forms.Label();
			this.label44 = new System.Windows.Forms.Label();
			this.textWebSchedRecallApptSearchDays = new OpenDental.ValidNum();
			this.checkRecallAllowProvSelection = new System.Windows.Forms.CheckBox();
			this.groupWebSchedText = new System.Windows.Forms.GroupBox();
			this.labelWebSchedPerBatch = new System.Windows.Forms.Label();
			this.textWebSchedPerBatch = new OpenDental.ValidNum();
			this.radioDoNotSendText = new System.Windows.Forms.RadioButton();
			this.radioSendText = new System.Windows.Forms.RadioButton();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.listboxWebSchedRecallIgnoreBlockoutTypes = new OpenDental.UI.ListBoxOD();
			this.butWebSchedRecallBlockouts = new OpenDental.UI.Button();
			this.groupBoxWebSchedAutomation = new System.Windows.Forms.GroupBox();
			this.radioSendToEmailNoPreferred = new System.Windows.Forms.RadioButton();
			this.radioDoNotSend = new System.Windows.Forms.RadioButton();
			this.radioSendToEmailOnlyPreferred = new System.Windows.Forms.RadioButton();
			this.radioSendToEmail = new System.Windows.Forms.RadioButton();
			this.groupWebSchedPreview = new System.Windows.Forms.GroupBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.butWebSchedPickClinic = new OpenDental.UI.Button();
			this.butWebSchedPickProv = new OpenDental.UI.Button();
			this.label22 = new System.Windows.Forms.Label();
			this.comboWebSchedProviders = new System.Windows.Forms.ComboBox();
			this.butWebSchedToday = new OpenDental.UI.Button();
			this.gridWebSchedTimeSlots = new OpenDental.UI.GridOD();
			this.textWebSchedDateStart = new OpenDental.ValidDate();
			this.labelWebSchedClinic = new System.Windows.Forms.Label();
			this.labelWebSchedRecallTypes = new System.Windows.Forms.Label();
			this.comboWebSchedClinic = new System.Windows.Forms.ComboBox();
			this.comboWebSchedRecallTypes = new System.Windows.Forms.ComboBox();
			this.gridWebSchedOperatories = new OpenDental.UI.GridOD();
			this.butRecallSchedSetup = new OpenDental.UI.Button();
			this.gridWebSchedRecallTypes = new OpenDental.UI.GridOD();
			this.label20 = new System.Windows.Forms.Label();
			this.butWebSchedRecallNotify = new OpenDental.UI.Button();
			this.butEditOperatories = new OpenDental.UI.Button();
			this.butEditRecallTypes = new OpenDental.UI.Button();
			this.groupWebSchedProvRule.SuspendLayout();
			this.groupWebSchedText.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBoxWebSchedAutomation.SuspendLayout();
			this.groupWebSchedPreview.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(818, 660);
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
			this.butCancel.Location = new System.Drawing.Point(899, 660);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupWebSchedProvRule
			// 
			this.groupWebSchedProvRule.Controls.Add(this.butProvRulePickClinic);
			this.groupWebSchedProvRule.Controls.Add(this.checkUseDefaultProvRule);
			this.groupWebSchedProvRule.Controls.Add(this.comboClinicProvRule);
			this.groupWebSchedProvRule.Controls.Add(this.listBoxWebSchedProviderPref);
			this.groupWebSchedProvRule.Controls.Add(this.label21);
			this.groupWebSchedProvRule.Location = new System.Drawing.Point(27, 495);
			this.groupWebSchedProvRule.Name = "groupWebSchedProvRule";
			this.groupWebSchedProvRule.Size = new System.Drawing.Size(439, 113);
			this.groupWebSchedProvRule.TabIndex = 405;
			this.groupWebSchedProvRule.TabStop = false;
			this.groupWebSchedProvRule.Text = "Provider Rule";
			// 
			// butProvRulePickClinic
			// 
			this.butProvRulePickClinic.Location = new System.Drawing.Point(414, 12);
			this.butProvRulePickClinic.Name = "butProvRulePickClinic";
			this.butProvRulePickClinic.Size = new System.Drawing.Size(18, 21);
			this.butProvRulePickClinic.TabIndex = 314;
			this.butProvRulePickClinic.Text = "...";
			this.butProvRulePickClinic.Click += new System.EventHandler(this.butProvRulePickClinic_Click);
			// 
			// checkUseDefaultProvRule
			// 
			this.checkUseDefaultProvRule.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseDefaultProvRule.Location = new System.Drawing.Point(92, 12);
			this.checkUseDefaultProvRule.Name = "checkUseDefaultProvRule";
			this.checkUseDefaultProvRule.Size = new System.Drawing.Size(104, 24);
			this.checkUseDefaultProvRule.TabIndex = 312;
			this.checkUseDefaultProvRule.Text = "Use Defaults";
			this.checkUseDefaultProvRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseDefaultProvRule.UseVisualStyleBackColor = true;
			this.checkUseDefaultProvRule.Visible = false;
			this.checkUseDefaultProvRule.Click += new System.EventHandler(this.checkUseDefaultProvRule_Click);
			// 
			// comboClinicProvRule
			// 
			this.comboClinicProvRule.HqDescription = "Defaults";
			this.comboClinicProvRule.IncludeUnassigned = true;
			this.comboClinicProvRule.Location = new System.Drawing.Point(209, 12);
			this.comboClinicProvRule.Name = "comboClinicProvRule";
			this.comboClinicProvRule.Size = new System.Drawing.Size(200, 21);
			this.comboClinicProvRule.TabIndex = 311;
			this.comboClinicProvRule.SelectionChangeCommitted += new System.EventHandler(this.comboClinicProvRule_SelectionChangeCommitted);
			// 
			// listBoxWebSchedProviderPref
			// 
			this.listBoxWebSchedProviderPref.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.listBoxWebSchedProviderPref.ItemStrings = new string[] {
        "First Available",
        "Primary Provider",
        "Secondary Provider",
        "Last Seen Hygienist"};
			this.listBoxWebSchedProviderPref.Location = new System.Drawing.Point(313, 43);
			this.listBoxWebSchedProviderPref.Name = "listBoxWebSchedProviderPref";
			this.listBoxWebSchedProviderPref.Size = new System.Drawing.Size(120, 56);
			this.listBoxWebSchedProviderPref.TabIndex = 309;
			this.listBoxWebSchedProviderPref.SelectedIndexChanged += new System.EventHandler(this.listBoxWebSchedProviderPref_SelectedIndexChanged);
			// 
			// label21
			// 
			this.label21.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label21.Location = new System.Drawing.Point(11, 45);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(301, 54);
			this.label21.TabIndex = 310;
			this.label21.Text = resources.GetString("label21.Text");
			this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkWSRDoubleBooking
			// 
			this.checkWSRDoubleBooking.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWSRDoubleBooking.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkWSRDoubleBooking.Location = new System.Drawing.Point(761, 351);
			this.checkWSRDoubleBooking.Name = "checkWSRDoubleBooking";
			this.checkWSRDoubleBooking.Size = new System.Drawing.Size(204, 18);
			this.checkWSRDoubleBooking.TabIndex = 401;
			this.checkWSRDoubleBooking.Text = "Prevent double booking";
			this.checkWSRDoubleBooking.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboWSRConfirmStatus
			// 
			this.comboWSRConfirmStatus.Location = new System.Drawing.Point(268, 612);
			this.comboWSRConfirmStatus.Name = "comboWSRConfirmStatus";
			this.comboWSRConfirmStatus.Size = new System.Drawing.Size(191, 21);
			this.comboWSRConfirmStatus.TabIndex = 400;
			// 
			// label36
			// 
			this.label36.Location = new System.Drawing.Point(41, 614);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(221, 17);
			this.label36.TabIndex = 399;
			this.label36.Text = "Web Sched Recall Confirm Status";
			this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label43
			// 
			this.label43.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label43.Location = new System.Drawing.Point(689, 614);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(252, 17);
			this.label43.TabIndex = 404;
			this.label43.Text = "days.  Empty includes all possible openings.";
			this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label44
			// 
			this.label44.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label44.Location = new System.Drawing.Point(505, 614);
			this.label44.Name = "label44";
			this.label44.Size = new System.Drawing.Size(137, 17);
			this.label44.TabIndex = 402;
			this.label44.Text = "Search for openings after";
			this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWebSchedRecallApptSearchDays
			// 
			this.textWebSchedRecallApptSearchDays.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textWebSchedRecallApptSearchDays.Location = new System.Drawing.Point(648, 613);
			this.textWebSchedRecallApptSearchDays.MaxVal = 365;
			this.textWebSchedRecallApptSearchDays.MinVal = 0;
			this.textWebSchedRecallApptSearchDays.Name = "textWebSchedRecallApptSearchDays";
			this.textWebSchedRecallApptSearchDays.ShowZero = false;
			this.textWebSchedRecallApptSearchDays.Size = new System.Drawing.Size(38, 20);
			this.textWebSchedRecallApptSearchDays.TabIndex = 403;
			this.textWebSchedRecallApptSearchDays.Validated += new System.EventHandler(this.textWebSchedRecallApptSearchDays_Validated);
			// 
			// checkRecallAllowProvSelection
			// 
			this.checkRecallAllowProvSelection.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecallAllowProvSelection.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecallAllowProvSelection.Location = new System.Drawing.Point(761, 327);
			this.checkRecallAllowProvSelection.Name = "checkRecallAllowProvSelection";
			this.checkRecallAllowProvSelection.Size = new System.Drawing.Size(204, 18);
			this.checkRecallAllowProvSelection.TabIndex = 398;
			this.checkRecallAllowProvSelection.Text = "Allow patients to select provider";
			this.checkRecallAllowProvSelection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupWebSchedText
			// 
			this.groupWebSchedText.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupWebSchedText.Controls.Add(this.labelWebSchedPerBatch);
			this.groupWebSchedText.Controls.Add(this.textWebSchedPerBatch);
			this.groupWebSchedText.Controls.Add(this.radioDoNotSendText);
			this.groupWebSchedText.Controls.Add(this.radioSendText);
			this.groupWebSchedText.Location = new System.Drawing.Point(496, 529);
			this.groupWebSchedText.Name = "groupWebSchedText";
			this.groupWebSchedText.Size = new System.Drawing.Size(484, 75);
			this.groupWebSchedText.TabIndex = 397;
			this.groupWebSchedText.TabStop = false;
			this.groupWebSchedText.Text = "Send Text Messages Automatically To";
			// 
			// labelWebSchedPerBatch
			// 
			this.labelWebSchedPerBatch.Location = new System.Drawing.Point(288, 19);
			this.labelWebSchedPerBatch.Name = "labelWebSchedPerBatch";
			this.labelWebSchedPerBatch.Size = new System.Drawing.Size(136, 32);
			this.labelWebSchedPerBatch.TabIndex = 314;
			this.labelWebSchedPerBatch.Text = "Max number of texts sent every 10 minutes per clinic";
			this.labelWebSchedPerBatch.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textWebSchedPerBatch
			// 
			this.textWebSchedPerBatch.Location = new System.Drawing.Point(426, 31);
			this.textWebSchedPerBatch.MaxVal = 100000000;
			this.textWebSchedPerBatch.MinVal = 1;
			this.textWebSchedPerBatch.Name = "textWebSchedPerBatch";
			this.textWebSchedPerBatch.ShowZero = true;
			this.textWebSchedPerBatch.Size = new System.Drawing.Size(39, 20);
			this.textWebSchedPerBatch.TabIndex = 242;
			// 
			// radioDoNotSendText
			// 
			this.radioDoNotSendText.Location = new System.Drawing.Point(7, 20);
			this.radioDoNotSendText.Name = "radioDoNotSendText";
			this.radioDoNotSendText.Size = new System.Drawing.Size(229, 16);
			this.radioDoNotSendText.TabIndex = 77;
			this.radioDoNotSendText.Text = "Do Not Send";
			this.radioDoNotSendText.UseVisualStyleBackColor = true;
			this.radioDoNotSendText.CheckedChanged += new System.EventHandler(this.WebSchedRecallAutoSendRadioButtons_CheckedChanged);
			// 
			// radioSendText
			// 
			this.radioSendText.Location = new System.Drawing.Point(7, 38);
			this.radioSendText.Name = "radioSendText";
			this.radioSendText.Size = new System.Drawing.Size(278, 18);
			this.radioSendText.TabIndex = 0;
			this.radioSendText.Text = "Patients with wireless phone (unless \'Text OK\' = No)";
			this.radioSendText.UseVisualStyleBackColor = true;
			this.radioSendText.CheckedChanged += new System.EventHandler(this.WebSchedRecallAutoSendRadioButtons_CheckedChanged);
			// 
			// groupBox6
			// 
			this.groupBox6.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBox6.Controls.Add(this.listboxWebSchedRecallIgnoreBlockoutTypes);
			this.groupBox6.Controls.Add(this.butWebSchedRecallBlockouts);
			this.groupBox6.Location = new System.Drawing.Point(496, 312);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(259, 103);
			this.groupBox6.TabIndex = 396;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Allowed Blockout Types";
			// 
			// listboxWebSchedRecallIgnoreBlockoutTypes
			// 
			this.listboxWebSchedRecallIgnoreBlockoutTypes.Location = new System.Drawing.Point(125, 15);
			this.listboxWebSchedRecallIgnoreBlockoutTypes.Name = "listboxWebSchedRecallIgnoreBlockoutTypes";
			this.listboxWebSchedRecallIgnoreBlockoutTypes.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listboxWebSchedRecallIgnoreBlockoutTypes.Size = new System.Drawing.Size(120, 82);
			this.listboxWebSchedRecallIgnoreBlockoutTypes.TabIndex = 197;
			// 
			// butWebSchedRecallBlockouts
			// 
			this.butWebSchedRecallBlockouts.Location = new System.Drawing.Point(51, 73);
			this.butWebSchedRecallBlockouts.Name = "butWebSchedRecallBlockouts";
			this.butWebSchedRecallBlockouts.Size = new System.Drawing.Size(68, 24);
			this.butWebSchedRecallBlockouts.TabIndex = 197;
			this.butWebSchedRecallBlockouts.Text = "Edit";
			this.butWebSchedRecallBlockouts.Click += new System.EventHandler(this.butWebSchedRecallBlockouts_Click);
			// 
			// groupBoxWebSchedAutomation
			// 
			this.groupBoxWebSchedAutomation.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupBoxWebSchedAutomation.Controls.Add(this.radioSendToEmailNoPreferred);
			this.groupBoxWebSchedAutomation.Controls.Add(this.radioDoNotSend);
			this.groupBoxWebSchedAutomation.Controls.Add(this.radioSendToEmailOnlyPreferred);
			this.groupBoxWebSchedAutomation.Controls.Add(this.radioSendToEmail);
			this.groupBoxWebSchedAutomation.Location = new System.Drawing.Point(496, 426);
			this.groupBoxWebSchedAutomation.Name = "groupBoxWebSchedAutomation";
			this.groupBoxWebSchedAutomation.Size = new System.Drawing.Size(484, 97);
			this.groupBoxWebSchedAutomation.TabIndex = 388;
			this.groupBoxWebSchedAutomation.TabStop = false;
			this.groupBoxWebSchedAutomation.Text = "Send Email Messages Automatically To";
			// 
			// radioSendToEmailNoPreferred
			// 
			this.radioSendToEmailNoPreferred.Location = new System.Drawing.Point(7, 51);
			this.radioSendToEmailNoPreferred.Name = "radioSendToEmailNoPreferred";
			this.radioSendToEmailNoPreferred.Size = new System.Drawing.Size(438, 18);
			this.radioSendToEmailNoPreferred.TabIndex = 1;
			this.radioSendToEmailNoPreferred.Text = "Patients with email address and no other preferred recall method is selected.";
			this.radioSendToEmailNoPreferred.UseVisualStyleBackColor = true;
			this.radioSendToEmailNoPreferred.CheckedChanged += new System.EventHandler(this.WebSchedRecallAutoSendRadioButtons_CheckedChanged);
			// 
			// radioDoNotSend
			// 
			this.radioDoNotSend.Location = new System.Drawing.Point(7, 16);
			this.radioDoNotSend.Name = "radioDoNotSend";
			this.radioDoNotSend.Size = new System.Drawing.Size(438, 16);
			this.radioDoNotSend.TabIndex = 77;
			this.radioDoNotSend.Text = "Do Not Send";
			this.radioDoNotSend.UseVisualStyleBackColor = true;
			// 
			// radioSendToEmailOnlyPreferred
			// 
			this.radioSendToEmailOnlyPreferred.Location = new System.Drawing.Point(7, 69);
			this.radioSendToEmailOnlyPreferred.Name = "radioSendToEmailOnlyPreferred";
			this.radioSendToEmailOnlyPreferred.Size = new System.Drawing.Size(438, 18);
			this.radioSendToEmailOnlyPreferred.TabIndex = 74;
			this.radioSendToEmailOnlyPreferred.Text = "Patients with email address and email is selected as their preferred recall metho" +
    "d.";
			this.radioSendToEmailOnlyPreferred.UseVisualStyleBackColor = true;
			this.radioSendToEmailOnlyPreferred.CheckedChanged += new System.EventHandler(this.WebSchedRecallAutoSendRadioButtons_CheckedChanged);
			// 
			// radioSendToEmail
			// 
			this.radioSendToEmail.Location = new System.Drawing.Point(7, 34);
			this.radioSendToEmail.Name = "radioSendToEmail";
			this.radioSendToEmail.Size = new System.Drawing.Size(438, 16);
			this.radioSendToEmail.TabIndex = 0;
			this.radioSendToEmail.Text = "Patients with email address";
			this.radioSendToEmail.UseVisualStyleBackColor = true;
			this.radioSendToEmail.CheckedChanged += new System.EventHandler(this.WebSchedRecallAutoSendRadioButtons_CheckedChanged);
			// 
			// groupWebSchedPreview
			// 
			this.groupWebSchedPreview.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.groupWebSchedPreview.Controls.Add(this.butRefresh);
			this.groupWebSchedPreview.Controls.Add(this.butWebSchedPickClinic);
			this.groupWebSchedPreview.Controls.Add(this.butWebSchedPickProv);
			this.groupWebSchedPreview.Controls.Add(this.label22);
			this.groupWebSchedPreview.Controls.Add(this.comboWebSchedProviders);
			this.groupWebSchedPreview.Controls.Add(this.butWebSchedToday);
			this.groupWebSchedPreview.Controls.Add(this.gridWebSchedTimeSlots);
			this.groupWebSchedPreview.Controls.Add(this.textWebSchedDateStart);
			this.groupWebSchedPreview.Controls.Add(this.labelWebSchedClinic);
			this.groupWebSchedPreview.Controls.Add(this.labelWebSchedRecallTypes);
			this.groupWebSchedPreview.Controls.Add(this.comboWebSchedClinic);
			this.groupWebSchedPreview.Controls.Add(this.comboWebSchedRecallTypes);
			this.groupWebSchedPreview.Location = new System.Drawing.Point(27, 276);
			this.groupWebSchedPreview.Name = "groupWebSchedPreview";
			this.groupWebSchedPreview.Size = new System.Drawing.Size(439, 210);
			this.groupWebSchedPreview.TabIndex = 390;
			this.groupWebSchedPreview.TabStop = false;
			this.groupWebSchedPreview.Text = "Available Times For Patients";
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(134, 176);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(66, 24);
			this.butRefresh.TabIndex = 407;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(butRefresh_Click);
			// 
			// butWebSchedPickClinic
			// 
			this.butWebSchedPickClinic.Location = new System.Drawing.Point(182, 142);
			this.butWebSchedPickClinic.Name = "butWebSchedPickClinic";
			this.butWebSchedPickClinic.Size = new System.Drawing.Size(18, 21);
			this.butWebSchedPickClinic.TabIndex = 313;
			this.butWebSchedPickClinic.Text = "...";
			this.butWebSchedPickClinic.Click += new System.EventHandler(this.butWebSchedPickClinic_Click);
			// 
			// butWebSchedPickProv
			// 
			this.butWebSchedPickProv.Location = new System.Drawing.Point(182, 101);
			this.butWebSchedPickProv.Name = "butWebSchedPickProv";
			this.butWebSchedPickProv.Size = new System.Drawing.Size(18, 21);
			this.butWebSchedPickProv.TabIndex = 312;
			this.butWebSchedPickProv.Text = "...";
			this.butWebSchedPickProv.Click += new System.EventHandler(this.butWebSchedPickProv_Click);
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(11, 84);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(182, 14);
			this.label22.TabIndex = 310;
			this.label22.Text = "Provider";
			this.label22.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboWebSchedProviders
			// 
			this.comboWebSchedProviders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboWebSchedProviders.Location = new System.Drawing.Point(11, 101);
			this.comboWebSchedProviders.MaxDropDownItems = 30;
			this.comboWebSchedProviders.Name = "comboWebSchedProviders";
			this.comboWebSchedProviders.Size = new System.Drawing.Size(165, 21);
			this.comboWebSchedProviders.TabIndex = 311;
			this.comboWebSchedProviders.SelectionChangeCommitted += new System.EventHandler(this.comboWebSchedProviders_SelectionChangeCommitted);
			// 
			// butWebSchedToday
			// 
			this.butWebSchedToday.Location = new System.Drawing.Point(110, 16);
			this.butWebSchedToday.Name = "butWebSchedToday";
			this.butWebSchedToday.Size = new System.Drawing.Size(66, 24);
			this.butWebSchedToday.TabIndex = 309;
			this.butWebSchedToday.Text = "Today";
			this.butWebSchedToday.Click += new System.EventHandler(this.butWebSchedToday_Click);
			// 
			// gridWebSchedTimeSlots
			// 
			this.gridWebSchedTimeSlots.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridWebSchedTimeSlots.Location = new System.Drawing.Point(247, 19);
			this.gridWebSchedTimeSlots.Name = "gridWebSchedTimeSlots";
			this.gridWebSchedTimeSlots.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridWebSchedTimeSlots.Size = new System.Drawing.Size(185, 186);
			this.gridWebSchedTimeSlots.TabIndex = 302;
			this.gridWebSchedTimeSlots.Title = "Time Slots";
			this.gridWebSchedTimeSlots.TranslationName = "FormEServicesSetup";
			this.gridWebSchedTimeSlots.WrapText = false;
			// 
			// textWebSchedDateStart
			// 
			this.textWebSchedDateStart.Location = new System.Drawing.Point(14, 19);
			this.textWebSchedDateStart.Name = "textWebSchedDateStart";
			this.textWebSchedDateStart.Size = new System.Drawing.Size(90, 20);
			this.textWebSchedDateStart.TabIndex = 303;
			this.textWebSchedDateStart.Text = "07/08/2015";
			// 
			// labelWebSchedClinic
			// 
			this.labelWebSchedClinic.Location = new System.Drawing.Point(11, 125);
			this.labelWebSchedClinic.Name = "labelWebSchedClinic";
			this.labelWebSchedClinic.Size = new System.Drawing.Size(182, 14);
			this.labelWebSchedClinic.TabIndex = 264;
			this.labelWebSchedClinic.Text = "Clinic";
			this.labelWebSchedClinic.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelWebSchedRecallTypes
			// 
			this.labelWebSchedRecallTypes.Location = new System.Drawing.Point(11, 43);
			this.labelWebSchedRecallTypes.Name = "labelWebSchedRecallTypes";
			this.labelWebSchedRecallTypes.Size = new System.Drawing.Size(182, 14);
			this.labelWebSchedRecallTypes.TabIndex = 254;
			this.labelWebSchedRecallTypes.Text = "Recall Type";
			this.labelWebSchedRecallTypes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboWebSchedClinic
			// 
			this.comboWebSchedClinic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboWebSchedClinic.Location = new System.Drawing.Point(11, 142);
			this.comboWebSchedClinic.MaxDropDownItems = 30;
			this.comboWebSchedClinic.Name = "comboWebSchedClinic";
			this.comboWebSchedClinic.Size = new System.Drawing.Size(165, 21);
			this.comboWebSchedClinic.TabIndex = 305;
			this.comboWebSchedClinic.SelectionChangeCommitted += new System.EventHandler(this.comboWebSchedClinic_SelectionChangeCommitted);
			// 
			// comboWebSchedRecallTypes
			// 
			this.comboWebSchedRecallTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboWebSchedRecallTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboWebSchedRecallTypes.Location = new System.Drawing.Point(11, 60);
			this.comboWebSchedRecallTypes.MaxDropDownItems = 30;
			this.comboWebSchedRecallTypes.Name = "comboWebSchedRecallTypes";
			this.comboWebSchedRecallTypes.Size = new System.Drawing.Size(165, 21);
			this.comboWebSchedRecallTypes.TabIndex = 304;
			// 
			// gridWebSchedOperatories
			// 
			this.gridWebSchedOperatories.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.gridWebSchedOperatories.Location = new System.Drawing.Point(27, 34);
			this.gridWebSchedOperatories.Name = "gridWebSchedOperatories";
			this.gridWebSchedOperatories.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridWebSchedOperatories.Size = new System.Drawing.Size(532, 225);
			this.gridWebSchedOperatories.TabIndex = 393;
			this.gridWebSchedOperatories.Title = "Operatories Considered";
			this.gridWebSchedOperatories.TranslationName = "FormEServicesSetup";
			this.gridWebSchedOperatories.WrapText = false;
			// 
			// butRecallSchedSetup
			// 
			this.butRecallSchedSetup.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butRecallSchedSetup.Location = new System.Drawing.Point(787, 278);
			this.butRecallSchedSetup.Name = "butRecallSchedSetup";
			this.butRecallSchedSetup.Size = new System.Drawing.Size(103, 24);
			this.butRecallSchedSetup.TabIndex = 395;
			this.butRecallSchedSetup.Text = "Recall Setup";
			this.butRecallSchedSetup.Click += new System.EventHandler(this.butWebSchedSetup_Click);
			// 
			// gridWebSchedRecallTypes
			// 
			this.gridWebSchedRecallTypes.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.gridWebSchedRecallTypes.Location = new System.Drawing.Point(579, 34);
			this.gridWebSchedRecallTypes.Name = "gridWebSchedRecallTypes";
			this.gridWebSchedRecallTypes.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridWebSchedRecallTypes.Size = new System.Drawing.Size(395, 225);
			this.gridWebSchedRecallTypes.TabIndex = 394;
			this.gridWebSchedRecallTypes.Title = "Recall Types";
			this.gridWebSchedRecallTypes.TranslationName = "FormEServicesSetup";
			this.gridWebSchedRecallTypes.WrapText = false;
			// 
			// label20
			// 
			this.label20.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label20.Location = new System.Drawing.Point(548, 276);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(233, 28);
			this.label20.TabIndex = 389;
			this.label20.Text = "Customize the notification message that will be sent to the patient.";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butWebSchedRecallNotify
			// 
			this.butWebSchedRecallNotify.Location = new System.Drawing.Point(27, 639);
			this.butWebSchedRecallNotify.Name = "butWebSchedRecallNotify";
			this.butWebSchedRecallNotify.Size = new System.Drawing.Size(109, 24);
			this.butWebSchedRecallNotify.TabIndex = 406;
			this.butWebSchedRecallNotify.Text = "Notification Settings";
			this.butWebSchedRecallNotify.UseVisualStyleBackColor = true;
			this.butWebSchedRecallNotify.Click += new System.EventHandler(this.butWebSchedRecallNotify_Click);
			// 
			// butEditOperatories
			// 
			this.butEditOperatories.Location = new System.Drawing.Point(491, 4);
			this.butEditOperatories.Name = "butEditOperatories";
			this.butEditOperatories.Size = new System.Drawing.Size(68, 24);
			this.butEditOperatories.TabIndex = 407;
			this.butEditOperatories.Text = "Edit";
			this.butEditOperatories.Click += new System.EventHandler(this.butEditOperatories_Click);
			// 
			// butEditRecallTypes
			// 
			this.butEditRecallTypes.Location = new System.Drawing.Point(906, 4);
			this.butEditRecallTypes.Name = "butEditRecallTypes";
			this.butEditRecallTypes.Size = new System.Drawing.Size(68, 24);
			this.butEditRecallTypes.TabIndex = 408;
			this.butEditRecallTypes.Text = "Edit";
			this.butEditRecallTypes.Click += new System.EventHandler(this.butEditRecallTypes_Click);
			// 
			// FormEServicesWebSchedRecall
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(998, 696);
			this.Controls.Add(this.butEditRecallTypes);
			this.Controls.Add(this.butEditOperatories);
			this.Controls.Add(this.butWebSchedRecallNotify);
			this.Controls.Add(this.groupWebSchedProvRule);
			this.Controls.Add(this.checkWSRDoubleBooking);
			this.Controls.Add(this.comboWSRConfirmStatus);
			this.Controls.Add(this.label36);
			this.Controls.Add(this.label43);
			this.Controls.Add(this.label44);
			this.Controls.Add(this.textWebSchedRecallApptSearchDays);
			this.Controls.Add(this.checkRecallAllowProvSelection);
			this.Controls.Add(this.groupWebSchedText);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBoxWebSchedAutomation);
			this.Controls.Add(this.groupWebSchedPreview);
			this.Controls.Add(this.gridWebSchedOperatories);
			this.Controls.Add(this.butRecallSchedSetup);
			this.Controls.Add(this.gridWebSchedRecallTypes);
			this.Controls.Add(this.label20);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesWebSchedRecall";
			this.Text = "Recall";
			this.Load += new System.EventHandler(this.FormEServicesWebSchedRecall_Load);
			this.groupWebSchedProvRule.ResumeLayout(false);
			this.groupWebSchedText.ResumeLayout(false);
			this.groupWebSchedText.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBoxWebSchedAutomation.ResumeLayout(false);
			this.groupWebSchedPreview.ResumeLayout(false);
			this.groupWebSchedPreview.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.GroupBox groupWebSchedProvRule;
		private UI.Button butProvRulePickClinic;
		private System.Windows.Forms.CheckBox checkUseDefaultProvRule;
		private UI.ComboBoxClinicPicker comboClinicProvRule;
		private OpenDental.UI.ListBoxOD listBoxWebSchedProviderPref;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.CheckBox checkWSRDoubleBooking;
		private UI.ComboBoxOD comboWSRConfirmStatus;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.Label label44;
		private ValidNum textWebSchedRecallApptSearchDays;
		private System.Windows.Forms.CheckBox checkRecallAllowProvSelection;
		private System.Windows.Forms.GroupBox groupWebSchedText;
		private System.Windows.Forms.Label labelWebSchedPerBatch;
		private ValidNum textWebSchedPerBatch;
		private System.Windows.Forms.RadioButton radioDoNotSendText;
		private System.Windows.Forms.RadioButton radioSendText;
		private System.Windows.Forms.GroupBox groupBox6;
		private OpenDental.UI.ListBoxOD listboxWebSchedRecallIgnoreBlockoutTypes;
		private UI.Button butWebSchedRecallBlockouts;
		private System.Windows.Forms.GroupBox groupBoxWebSchedAutomation;
		private System.Windows.Forms.RadioButton radioSendToEmailNoPreferred;
		private System.Windows.Forms.RadioButton radioDoNotSend;
		private System.Windows.Forms.RadioButton radioSendToEmailOnlyPreferred;
		private System.Windows.Forms.RadioButton radioSendToEmail;
		private System.Windows.Forms.GroupBox groupWebSchedPreview;
		private UI.Button butWebSchedPickClinic;
		private UI.Button butWebSchedPickProv;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.ComboBox comboWebSchedProviders;
		private UI.Button butWebSchedToday;
		private UI.GridOD gridWebSchedTimeSlots;
		private ValidDate textWebSchedDateStart;
		private System.Windows.Forms.Label labelWebSchedClinic;
		private System.Windows.Forms.Label labelWebSchedRecallTypes;
		private System.Windows.Forms.ComboBox comboWebSchedClinic;
		private System.Windows.Forms.ComboBox comboWebSchedRecallTypes;
		private UI.GridOD gridWebSchedOperatories;
		private UI.Button butRecallSchedSetup;
		private UI.GridOD gridWebSchedRecallTypes;
		private System.Windows.Forms.Label label20;
		private UI.Button butWebSchedRecallNotify;
		private UI.Button butRefresh;
		private UI.Button butEditOperatories;
		private UI.Button butEditRecallTypes;
	}
}