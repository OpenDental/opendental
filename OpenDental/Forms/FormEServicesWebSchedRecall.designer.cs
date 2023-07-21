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
			this.groupWebSchedProvRule = new OpenDental.UI.GroupBox();
			this.butProvRulePickClinic = new OpenDental.UI.Button();
			this.checkUseDefaultProvRule = new OpenDental.UI.CheckBox();
			this.comboClinicProvRule = new OpenDental.UI.ComboBoxClinicPicker();
			this.listBoxWebSchedProviderPref = new OpenDental.UI.ListBox();
			this.label21 = new System.Windows.Forms.Label();
			this.checkWSRDoubleBooking = new OpenDental.UI.CheckBox();
			this.checkRecallAllowProvSelection = new OpenDental.UI.CheckBox();
			this.comboWSRConfirmStatus = new OpenDental.UI.ComboBox();
			this.labelRecallConfirmStatus = new System.Windows.Forms.Label();
			this.labelDaysFuture = new System.Windows.Forms.Label();
			this.textWebSchedRecallApptSearchDays = new OpenDental.ValidNum();
			this.groupWebSchedText = new OpenDental.UI.GroupBox();
			this.labelWebSchedPerBatch = new System.Windows.Forms.Label();
			this.textWebSchedPerBatch = new OpenDental.ValidNum();
			this.radioDoNotSendText = new System.Windows.Forms.RadioButton();
			this.radioSendText = new System.Windows.Forms.RadioButton();
			this.groupBox6 = new OpenDental.UI.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butWebSchedRecallRestrictedToBlockoutEdit = new OpenDental.UI.Button();
			this.listboxWebSchedRecallIgnoreBlockoutTypes = new OpenDental.UI.ListBox();
			this.labelRestrictedToBlockouts = new System.Windows.Forms.Label();
			this.butWebSchedRecallBlockouts = new OpenDental.UI.Button();
			this.listboxRestrictedToBlockouts = new OpenDental.UI.ListBox();
			this.groupBoxWebSchedAutomation = new OpenDental.UI.GroupBox();
			this.radioSendToEmailNoPreferred = new System.Windows.Forms.RadioButton();
			this.radioDoNotSend = new System.Windows.Forms.RadioButton();
			this.radioSendToEmailOnlyPreferred = new System.Windows.Forms.RadioButton();
			this.radioSendToEmail = new System.Windows.Forms.RadioButton();
			this.groupWebSchedPreview = new OpenDental.UI.GroupBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.butWebSchedPickClinic = new OpenDental.UI.Button();
			this.butWebSchedPickProv = new OpenDental.UI.Button();
			this.label22 = new System.Windows.Forms.Label();
			this.comboWebSchedProviders = new OpenDental.UI.ComboBox();
			this.butWebSchedToday = new OpenDental.UI.Button();
			this.gridWebSchedTimeSlots = new OpenDental.UI.GridOD();
			this.textWebSchedDateStart = new OpenDental.ValidDate();
			this.labelWebSchedClinic = new System.Windows.Forms.Label();
			this.labelWebSchedRecallTypes = new System.Windows.Forms.Label();
			this.comboWebSchedClinic = new OpenDental.UI.ComboBox();
			this.comboWebSchedRecallTypes = new OpenDental.UI.ComboBox();
			this.gridWebSchedOperatories = new OpenDental.UI.GridOD();
			this.butRecallSchedSetup = new OpenDental.UI.Button();
			this.gridWebSchedRecallTypes = new OpenDental.UI.GridOD();
			this.labelRecallSetup = new System.Windows.Forms.Label();
			this.butWebSchedRecallNotify = new OpenDental.UI.Button();
			this.butEditOperatories = new OpenDental.UI.Button();
			this.butEditRecallTypes = new OpenDental.UI.Button();
			this.groupCustomizedMessages = new OpenDental.UI.GroupBox();
			this.labelNotificationSettings = new System.Windows.Forms.Label();
			this.groupOtherSettings = new OpenDental.UI.GroupBox();
			this.textNumMonthsCheck = new OpenDental.ValidNum();
			this.labelNumMonthsCheck = new System.Windows.Forms.Label();
			this.groupWebSchedProvRule.SuspendLayout();
			this.groupWebSchedText.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBoxWebSchedAutomation.SuspendLayout();
			this.groupWebSchedPreview.SuspendLayout();
			this.groupCustomizedMessages.SuspendLayout();
			this.groupOtherSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(876, 651);
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
			this.butCancel.Location = new System.Drawing.Point(957, 651);
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
			this.groupWebSchedProvRule.Location = new System.Drawing.Point(11, 404);
			this.groupWebSchedProvRule.Name = "groupWebSchedProvRule";
			this.groupWebSchedProvRule.Size = new System.Drawing.Size(439, 111);
			this.groupWebSchedProvRule.TabIndex = 405;
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
			this.listBoxWebSchedProviderPref.Location = new System.Drawing.Point(312, 43);
			this.listBoxWebSchedProviderPref.Name = "listBoxWebSchedProviderPref";
			this.listBoxWebSchedProviderPref.Size = new System.Drawing.Size(120, 56);
			this.listBoxWebSchedProviderPref.TabIndex = 309;
			this.listBoxWebSchedProviderPref.SelectedIndexChanged += new System.EventHandler(this.listBoxWebSchedProviderPref_SelectedIndexChanged);
			// 
			// label21
			// 
			this.label21.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label21.Location = new System.Drawing.Point(9, 45);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(301, 54);
			this.label21.TabIndex = 310;
			this.label21.Text = resources.GetString("label21.Text");
			this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkWSRDoubleBooking
			// 
			this.checkWSRDoubleBooking.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWSRDoubleBooking.Location = new System.Drawing.Point(24, 15);
			this.checkWSRDoubleBooking.Name = "checkWSRDoubleBooking";
			this.checkWSRDoubleBooking.Size = new System.Drawing.Size(196, 18);
			this.checkWSRDoubleBooking.TabIndex = 401;
			this.checkWSRDoubleBooking.Text = "Prevent double booking";
			// 
			// checkRecallAllowProvSelection
			// 
			this.checkRecallAllowProvSelection.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecallAllowProvSelection.Location = new System.Drawing.Point(226, 15);
			this.checkRecallAllowProvSelection.Name = "checkRecallAllowProvSelection";
			this.checkRecallAllowProvSelection.Size = new System.Drawing.Size(191, 18);
			this.checkRecallAllowProvSelection.TabIndex = 398;
			this.checkRecallAllowProvSelection.Text = "Allow patients to select provider";
			// 
			// comboWSRConfirmStatus
			// 
			this.comboWSRConfirmStatus.Location = new System.Drawing.Point(226, 91);
			this.comboWSRConfirmStatus.Name = "comboWSRConfirmStatus";
			this.comboWSRConfirmStatus.Size = new System.Drawing.Size(191, 21);
			this.comboWSRConfirmStatus.TabIndex = 400;
			// 
			// labelRecallConfirmStatus
			// 
			this.labelRecallConfirmStatus.Location = new System.Drawing.Point(14, 93);
			this.labelRecallConfirmStatus.Name = "labelRecallConfirmStatus";
			this.labelRecallConfirmStatus.Size = new System.Drawing.Size(206, 17);
			this.labelRecallConfirmStatus.TabIndex = 399;
			this.labelRecallConfirmStatus.Text = "Web Sched Recall Confirm Status";
			this.labelRecallConfirmStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDaysFuture
			// 
			this.labelDaysFuture.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelDaysFuture.Location = new System.Drawing.Point(62, 38);
			this.labelDaysFuture.Name = "labelDaysFuture";
			this.labelDaysFuture.Size = new System.Drawing.Size(311, 19);
			this.labelDaysFuture.TabIndex = 402;
			this.labelDaysFuture.Text = "Number of days in the future to start searching for openings";
			this.labelDaysFuture.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWebSchedRecallApptSearchDays
			// 
			this.textWebSchedRecallApptSearchDays.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textWebSchedRecallApptSearchDays.Location = new System.Drawing.Point(379, 39);
			this.textWebSchedRecallApptSearchDays.MaxVal = 365;
			this.textWebSchedRecallApptSearchDays.Name = "textWebSchedRecallApptSearchDays";
			this.textWebSchedRecallApptSearchDays.ShowZero = false;
			this.textWebSchedRecallApptSearchDays.Size = new System.Drawing.Size(38, 20);
			this.textWebSchedRecallApptSearchDays.TabIndex = 403;
			// 
			// groupWebSchedText
			// 
			this.groupWebSchedText.Controls.Add(this.labelWebSchedPerBatch);
			this.groupWebSchedText.Controls.Add(this.textWebSchedPerBatch);
			this.groupWebSchedText.Controls.Add(this.radioDoNotSendText);
			this.groupWebSchedText.Controls.Add(this.radioSendText);
			this.groupWebSchedText.Location = new System.Drawing.Point(457, 447);
			this.groupWebSchedText.Name = "groupWebSchedText";
			this.groupWebSchedText.Size = new System.Drawing.Size(575, 68);
			this.groupWebSchedText.TabIndex = 397;
			this.groupWebSchedText.Text = "Send Text Messages Automatically To";
			// 
			// labelWebSchedPerBatch
			// 
			this.labelWebSchedPerBatch.Location = new System.Drawing.Point(357, 24);
			this.labelWebSchedPerBatch.Name = "labelWebSchedPerBatch";
			this.labelWebSchedPerBatch.Size = new System.Drawing.Size(156, 32);
			this.labelWebSchedPerBatch.TabIndex = 314;
			this.labelWebSchedPerBatch.Text = "Max number of texts sent every 10 minutes per clinic";
			this.labelWebSchedPerBatch.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textWebSchedPerBatch
			// 
			this.textWebSchedPerBatch.Location = new System.Drawing.Point(515, 36);
			this.textWebSchedPerBatch.MaxVal = 100000000;
			this.textWebSchedPerBatch.MinVal = 1;
			this.textWebSchedPerBatch.Name = "textWebSchedPerBatch";
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
			this.groupBox6.Controls.Add(this.label1);
			this.groupBox6.Controls.Add(this.butWebSchedRecallRestrictedToBlockoutEdit);
			this.groupBox6.Controls.Add(this.listboxWebSchedRecallIgnoreBlockoutTypes);
			this.groupBox6.Controls.Add(this.labelRestrictedToBlockouts);
			this.groupBox6.Controls.Add(this.butWebSchedRecallBlockouts);
			this.groupBox6.Controls.Add(this.listboxRestrictedToBlockouts);
			this.groupBox6.Location = new System.Drawing.Point(457, 187);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(575, 151);
			this.groupBox6.TabIndex = 396;
			this.groupBox6.Text = "Allowed Blockout Types";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(179, 20);
			this.label1.TabIndex = 412;
			this.label1.Text = "Generally Allowed";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butWebSchedRecallRestrictedToBlockoutEdit
			// 
			this.butWebSchedRecallRestrictedToBlockoutEdit.Location = new System.Drawing.Point(290, 124);
			this.butWebSchedRecallRestrictedToBlockoutEdit.Name = "butWebSchedRecallRestrictedToBlockoutEdit";
			this.butWebSchedRecallRestrictedToBlockoutEdit.Size = new System.Drawing.Size(68, 24);
			this.butWebSchedRecallRestrictedToBlockoutEdit.TabIndex = 411;
			this.butWebSchedRecallRestrictedToBlockoutEdit.Text = "Edit";
			this.butWebSchedRecallRestrictedToBlockoutEdit.UseVisualStyleBackColor = false;
			this.butWebSchedRecallRestrictedToBlockoutEdit.Click += new System.EventHandler(this.butWebSchedRecallRestrictedToBlockoutEdit_Click);
			// 
			// listboxWebSchedRecallIgnoreBlockoutTypes
			// 
			this.listboxWebSchedRecallIgnoreBlockoutTypes.Location = new System.Drawing.Point(7, 39);
			this.listboxWebSchedRecallIgnoreBlockoutTypes.Name = "listboxWebSchedRecallIgnoreBlockoutTypes";
			this.listboxWebSchedRecallIgnoreBlockoutTypes.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listboxWebSchedRecallIgnoreBlockoutTypes.Size = new System.Drawing.Size(275, 82);
			this.listboxWebSchedRecallIgnoreBlockoutTypes.TabIndex = 197;
			// 
			// labelRestrictedToBlockouts
			// 
			this.labelRestrictedToBlockouts.Location = new System.Drawing.Point(287, 16);
			this.labelRestrictedToBlockouts.Name = "labelRestrictedToBlockouts";
			this.labelRestrictedToBlockouts.Size = new System.Drawing.Size(179, 20);
			this.labelRestrictedToBlockouts.TabIndex = 410;
			this.labelRestrictedToBlockouts.Text = "Restricted to Recall Types";
			this.labelRestrictedToBlockouts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butWebSchedRecallBlockouts
			// 
			this.butWebSchedRecallBlockouts.Location = new System.Drawing.Point(7, 124);
			this.butWebSchedRecallBlockouts.Name = "butWebSchedRecallBlockouts";
			this.butWebSchedRecallBlockouts.Size = new System.Drawing.Size(68, 24);
			this.butWebSchedRecallBlockouts.TabIndex = 197;
			this.butWebSchedRecallBlockouts.Text = "Edit";
			this.butWebSchedRecallBlockouts.Click += new System.EventHandler(this.butWebSchedRecallBlockouts_Click);
			// 
			// listboxRestrictedToBlockouts
			// 
			this.listboxRestrictedToBlockouts.Location = new System.Drawing.Point(290, 39);
			this.listboxRestrictedToBlockouts.Name = "listboxRestrictedToBlockouts";
			this.listboxRestrictedToBlockouts.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listboxRestrictedToBlockouts.Size = new System.Drawing.Size(275, 82);
			this.listboxRestrictedToBlockouts.TabIndex = 409;
			// 
			// groupBoxWebSchedAutomation
			// 
			this.groupBoxWebSchedAutomation.Controls.Add(this.radioSendToEmailNoPreferred);
			this.groupBoxWebSchedAutomation.Controls.Add(this.radioDoNotSend);
			this.groupBoxWebSchedAutomation.Controls.Add(this.radioSendToEmailOnlyPreferred);
			this.groupBoxWebSchedAutomation.Controls.Add(this.radioSendToEmail);
			this.groupBoxWebSchedAutomation.Location = new System.Drawing.Point(457, 344);
			this.groupBoxWebSchedAutomation.Name = "groupBoxWebSchedAutomation";
			this.groupBoxWebSchedAutomation.Size = new System.Drawing.Size(575, 97);
			this.groupBoxWebSchedAutomation.TabIndex = 388;
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
			this.groupWebSchedPreview.Location = new System.Drawing.Point(11, 187);
			this.groupWebSchedPreview.Name = "groupWebSchedPreview";
			this.groupWebSchedPreview.Size = new System.Drawing.Size(439, 210);
			this.groupWebSchedPreview.TabIndex = 390;
			this.groupWebSchedPreview.Text = "Available Times For Patients";
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(134, 176);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(66, 24);
			this.butRefresh.TabIndex = 407;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
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
			this.textWebSchedDateStart.Text = "7/8/2015";
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
			this.comboWebSchedClinic.Name = "comboWebSchedClinic";
			this.comboWebSchedClinic.Size = new System.Drawing.Size(165, 21);
			this.comboWebSchedClinic.TabIndex = 305;
			this.comboWebSchedClinic.SelectionChangeCommitted += new System.EventHandler(this.comboWebSchedClinic_SelectionChangeCommitted);
			// 
			// comboWebSchedRecallTypes
			// 
			this.comboWebSchedRecallTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboWebSchedRecallTypes.Location = new System.Drawing.Point(11, 60);
			this.comboWebSchedRecallTypes.Name = "comboWebSchedRecallTypes";
			this.comboWebSchedRecallTypes.Size = new System.Drawing.Size(165, 21);
			this.comboWebSchedRecallTypes.TabIndex = 304;
			// 
			// gridWebSchedOperatories
			// 
			this.gridWebSchedOperatories.Location = new System.Drawing.Point(457, 38);
			this.gridWebSchedOperatories.Name = "gridWebSchedOperatories";
			this.gridWebSchedOperatories.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridWebSchedOperatories.Size = new System.Drawing.Size(575, 142);
			this.gridWebSchedOperatories.TabIndex = 393;
			this.gridWebSchedOperatories.Title = "Operatories Considered";
			this.gridWebSchedOperatories.TranslationName = "FormEServicesSetup";
			this.gridWebSchedOperatories.WrapText = false;
			// 
			// butRecallSchedSetup
			// 
			this.butRecallSchedSetup.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butRecallSchedSetup.Location = new System.Drawing.Point(445, 20);
			this.butRecallSchedSetup.Name = "butRecallSchedSetup";
			this.butRecallSchedSetup.Size = new System.Drawing.Size(109, 24);
			this.butRecallSchedSetup.TabIndex = 395;
			this.butRecallSchedSetup.Text = "Recall Setup";
			this.butRecallSchedSetup.Click += new System.EventHandler(this.butWebSchedSetup_Click);
			// 
			// gridWebSchedRecallTypes
			// 
			this.gridWebSchedRecallTypes.Location = new System.Drawing.Point(11, 38);
			this.gridWebSchedRecallTypes.Name = "gridWebSchedRecallTypes";
			this.gridWebSchedRecallTypes.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridWebSchedRecallTypes.Size = new System.Drawing.Size(439, 142);
			this.gridWebSchedRecallTypes.TabIndex = 394;
			this.gridWebSchedRecallTypes.Title = "Recall Types";
			this.gridWebSchedRecallTypes.TranslationName = "FormEServicesSetup";
			this.gridWebSchedRecallTypes.WrapText = false;
			// 
			// labelRecallSetup
			// 
			this.labelRecallSetup.Location = new System.Drawing.Point(9, 18);
			this.labelRecallSetup.Name = "labelRecallSetup";
			this.labelRecallSetup.Size = new System.Drawing.Size(428, 28);
			this.labelRecallSetup.TabIndex = 389;
			this.labelRecallSetup.Text = "Customize the recall message that will be sent to the patient";
			this.labelRecallSetup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butWebSchedRecallNotify
			// 
			this.butWebSchedRecallNotify.Location = new System.Drawing.Point(445, 54);
			this.butWebSchedRecallNotify.Name = "butWebSchedRecallNotify";
			this.butWebSchedRecallNotify.Size = new System.Drawing.Size(109, 24);
			this.butWebSchedRecallNotify.TabIndex = 406;
			this.butWebSchedRecallNotify.Text = "Notification Settings";
			this.butWebSchedRecallNotify.UseVisualStyleBackColor = false;
			this.butWebSchedRecallNotify.Click += new System.EventHandler(this.butWebSchedRecallNotify_Click);
			// 
			// butEditOperatories
			// 
			this.butEditOperatories.Location = new System.Drawing.Point(964, 8);
			this.butEditOperatories.Name = "butEditOperatories";
			this.butEditOperatories.Size = new System.Drawing.Size(68, 24);
			this.butEditOperatories.TabIndex = 407;
			this.butEditOperatories.Text = "Edit";
			this.butEditOperatories.Click += new System.EventHandler(this.butEditOperatories_Click);
			// 
			// butEditRecallTypes
			// 
			this.butEditRecallTypes.Location = new System.Drawing.Point(382, 8);
			this.butEditRecallTypes.Name = "butEditRecallTypes";
			this.butEditRecallTypes.Size = new System.Drawing.Size(68, 24);
			this.butEditRecallTypes.TabIndex = 408;
			this.butEditRecallTypes.Text = "Edit";
			this.butEditRecallTypes.Click += new System.EventHandler(this.butEditRecallTypes_Click);
			// 
			// groupCustomizedMessages
			// 
			this.groupCustomizedMessages.Controls.Add(this.labelNotificationSettings);
			this.groupCustomizedMessages.Controls.Add(this.butWebSchedRecallNotify);
			this.groupCustomizedMessages.Controls.Add(this.labelRecallSetup);
			this.groupCustomizedMessages.Controls.Add(this.butRecallSchedSetup);
			this.groupCustomizedMessages.Location = new System.Drawing.Point(457, 521);
			this.groupCustomizedMessages.Name = "groupCustomizedMessages";
			this.groupCustomizedMessages.Size = new System.Drawing.Size(575, 117);
			this.groupCustomizedMessages.TabIndex = 409;
			this.groupCustomizedMessages.Text = "Customized Messages";
			// 
			// labelNotificationSettings
			// 
			this.labelNotificationSettings.Location = new System.Drawing.Point(9, 54);
			this.labelNotificationSettings.Name = "labelNotificationSettings";
			this.labelNotificationSettings.Size = new System.Drawing.Size(428, 23);
			this.labelNotificationSettings.TabIndex = 407;
			this.labelNotificationSettings.Text = "Customize the \"appointment booked\" notification";
			this.labelNotificationSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupOtherSettings
			// 
			this.groupOtherSettings.Controls.Add(this.textNumMonthsCheck);
			this.groupOtherSettings.Controls.Add(this.labelNumMonthsCheck);
			this.groupOtherSettings.Controls.Add(this.comboWSRConfirmStatus);
			this.groupOtherSettings.Controls.Add(this.labelRecallConfirmStatus);
			this.groupOtherSettings.Controls.Add(this.textWebSchedRecallApptSearchDays);
			this.groupOtherSettings.Controls.Add(this.labelDaysFuture);
			this.groupOtherSettings.Controls.Add(this.checkWSRDoubleBooking);
			this.groupOtherSettings.Controls.Add(this.checkRecallAllowProvSelection);
			this.groupOtherSettings.Location = new System.Drawing.Point(11, 521);
			this.groupOtherSettings.Name = "groupOtherSettings";
			this.groupOtherSettings.Size = new System.Drawing.Size(439, 117);
			this.groupOtherSettings.TabIndex = 410;
			this.groupOtherSettings.Text = "Other Settings";
			// 
			// textNumMonthsCheck
			// 
			this.textNumMonthsCheck.Location = new System.Drawing.Point(379, 65);
			this.textNumMonthsCheck.MaxVal = 24;
			this.textNumMonthsCheck.MinVal = 1;
			this.textNumMonthsCheck.Name = "textNumMonthsCheck";
			this.textNumMonthsCheck.Size = new System.Drawing.Size(38, 20);
			this.textNumMonthsCheck.TabIndex = 405;
			// 
			// labelNumMonthsCheck
			// 
			this.labelNumMonthsCheck.Location = new System.Drawing.Point(62, 64);
			this.labelNumMonthsCheck.Name = "labelNumMonthsCheck";
			this.labelNumMonthsCheck.Size = new System.Drawing.Size(311, 19);
			this.labelNumMonthsCheck.TabIndex = 404;
			this.labelNumMonthsCheck.Text = "Maximum number of months to search on initial check (1-24)";
			this.labelNumMonthsCheck.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEServicesWebSchedRecall
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1046, 687);
			this.Controls.Add(this.groupOtherSettings);
			this.Controls.Add(this.groupCustomizedMessages);
			this.Controls.Add(this.butEditRecallTypes);
			this.Controls.Add(this.butEditOperatories);
			this.Controls.Add(this.groupWebSchedProvRule);
			this.Controls.Add(this.groupWebSchedText);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBoxWebSchedAutomation);
			this.Controls.Add(this.groupWebSchedPreview);
			this.Controls.Add(this.gridWebSchedOperatories);
			this.Controls.Add(this.gridWebSchedRecallTypes);
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
			this.groupCustomizedMessages.ResumeLayout(false);
			this.groupOtherSettings.ResumeLayout(false);
			this.groupOtherSettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.GroupBox groupWebSchedProvRule;
		private UI.Button butProvRulePickClinic;
		private OpenDental.UI.CheckBox checkUseDefaultProvRule;
		private UI.ComboBoxClinicPicker comboClinicProvRule;
		private OpenDental.UI.ListBox listBoxWebSchedProviderPref;
		private System.Windows.Forms.Label label21;
		private OpenDental.UI.CheckBox checkWSRDoubleBooking;
		private UI.ComboBox comboWSRConfirmStatus;
		private System.Windows.Forms.Label labelRecallConfirmStatus;
		private System.Windows.Forms.Label labelDaysFuture;
		private ValidNum textWebSchedRecallApptSearchDays;
		private OpenDental.UI.CheckBox checkRecallAllowProvSelection;
		private OpenDental.UI.GroupBox groupWebSchedText;
		private System.Windows.Forms.Label labelWebSchedPerBatch;
		private ValidNum textWebSchedPerBatch;
		private System.Windows.Forms.RadioButton radioDoNotSendText;
		private System.Windows.Forms.RadioButton radioSendText;
		private OpenDental.UI.GroupBox groupBox6;
		private OpenDental.UI.ListBox listboxWebSchedRecallIgnoreBlockoutTypes;
		private UI.Button butWebSchedRecallBlockouts;
		private OpenDental.UI.GroupBox groupBoxWebSchedAutomation;
		private System.Windows.Forms.RadioButton radioSendToEmailNoPreferred;
		private System.Windows.Forms.RadioButton radioDoNotSend;
		private System.Windows.Forms.RadioButton radioSendToEmailOnlyPreferred;
		private System.Windows.Forms.RadioButton radioSendToEmail;
		private OpenDental.UI.GroupBox groupWebSchedPreview;
		private UI.Button butWebSchedPickClinic;
		private UI.Button butWebSchedPickProv;
		private System.Windows.Forms.Label label22;
		private OpenDental.UI.ComboBox comboWebSchedProviders;
		private UI.Button butWebSchedToday;
		private UI.GridOD gridWebSchedTimeSlots;
		private ValidDate textWebSchedDateStart;
		private System.Windows.Forms.Label labelWebSchedClinic;
		private System.Windows.Forms.Label labelWebSchedRecallTypes;
		private OpenDental.UI.ComboBox comboWebSchedClinic;
		private OpenDental.UI.ComboBox comboWebSchedRecallTypes;
		private UI.GridOD gridWebSchedOperatories;
		private UI.Button butRecallSchedSetup;
		private UI.GridOD gridWebSchedRecallTypes;
		private System.Windows.Forms.Label labelRecallSetup;
		private UI.Button butWebSchedRecallNotify;
		private UI.Button butRefresh;
		private UI.Button butEditOperatories;
		private UI.Button butEditRecallTypes;
		private System.Windows.Forms.Label label1;
		private UI.Button butWebSchedRecallRestrictedToBlockoutEdit;
		private System.Windows.Forms.Label labelRestrictedToBlockouts;
		private UI.ListBox listboxRestrictedToBlockouts;
		private UI.GroupBox groupCustomizedMessages;
		private UI.GroupBox groupOtherSettings;
		private System.Windows.Forms.Label labelNotificationSettings;
		private ValidNum textNumMonthsCheck;
		private System.Windows.Forms.Label labelNumMonthsCheck;
	}
}