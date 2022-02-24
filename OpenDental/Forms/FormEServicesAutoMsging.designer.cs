namespace OpenDental{
	partial class FormEServicesAutoMsging{
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesAutoMsging));
			this.label37 = new System.Windows.Forms.Label();
			this.menuWebSchedVerifyTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.insertReplacementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.butOK = new OpenDental.UI.Button();
			this.butAddReminder = new OpenDental.UI.Button();
			this.butAddConfirmation = new OpenDental.UI.Button();
			this.label54 = new System.Windows.Forms.Label();
			this.comboClinicEConfirm = new System.Windows.Forms.ComboBox();
			this.checkIsConfirmEnabled = new System.Windows.Forms.CheckBox();
			this.groupAutomationStatuses = new System.Windows.Forms.GroupBox();
			this.radio2ClickConfirm = new System.Windows.Forms.RadioButton();
			this.radio1ClickConfirm = new System.Windows.Forms.RadioButton();
			this.comboStatusEFailed = new OpenDental.UI.ComboBoxOD();
			this.label50 = new System.Windows.Forms.Label();
			this.checkEnableNoClinic = new System.Windows.Forms.CheckBox();
			this.comboStatusEDeclined = new OpenDental.UI.ComboBoxOD();
			this.comboStatusESent = new OpenDental.UI.ComboBoxOD();
			this.comboStatusEAccepted = new OpenDental.UI.ComboBoxOD();
			this.label51 = new System.Windows.Forms.Label();
			this.label52 = new System.Windows.Forms.Label();
			this.label53 = new System.Windows.Forms.Label();
			this.butActivateConfirm = new OpenDental.UI.Button();
			this.butActivateReminder = new OpenDental.UI.Button();
			this.checkUseDefaultsEC = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.butAddThankYouVerify = new OpenDental.UI.Button();
			this.butActivateThanks = new OpenDental.UI.Button();
			this.gridRemindersMain = new OpenDental.UI.GridOD();
			this.textStatusConfirmations = new System.Windows.Forms.TextBox();
			this.textStatusReminders = new System.Windows.Forms.TextBox();
			this.gridConfStatuses = new OpenDental.UI.GridOD();
			this.textStatusThankYous = new System.Windows.Forms.TextBox();
			this.textThankYouTitle = new System.Windows.Forms.TextBox();
			this.labelThankYouTitle = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butAddArrival = new OpenDental.UI.Button();
			this.textStatusArrivals = new System.Windows.Forms.TextBox();
			this.butActivateArrivals = new OpenDental.UI.Button();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			this.groupAutomationStatuses.SuspendLayout();
			this.SuspendLayout();
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(0, 0);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(100, 23);
			this.label37.TabIndex = 0;
			// 
			// menuWebSchedVerifyTextTemplate
			// 
			this.menuWebSchedVerifyTextTemplate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertReplacementsToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.selectAllToolStripMenuItem});
			this.menuWebSchedVerifyTextTemplate.Name = "menuASAPEmailBody";
			this.menuWebSchedVerifyTextTemplate.Size = new System.Drawing.Size(137, 136);
			this.menuWebSchedVerifyTextTemplate.Text = "Insert Replacements";
			// 
			// insertReplacementsToolStripMenuItem
			// 
			this.insertReplacementsToolStripMenuItem.Name = "insertReplacementsToolStripMenuItem";
			this.insertReplacementsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.insertReplacementsToolStripMenuItem.Text = "Insert Fields";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.selectAllToolStripMenuItem.Text = "Select All";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(999, 644);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 500;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAddReminder
			// 
			this.butAddReminder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddReminder.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddReminder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddReminder.Location = new System.Drawing.Point(428, 611);
			this.butAddReminder.Name = "butAddReminder";
			this.butAddReminder.Size = new System.Drawing.Size(119, 24);
			this.butAddReminder.TabIndex = 92;
			this.butAddReminder.Text = "Add  Reminder";
			this.butAddReminder.UseVisualStyleBackColor = true;
			this.butAddReminder.Click += new System.EventHandler(this.butAddReminder_Click);
			// 
			// butAddConfirmation
			// 
			this.butAddConfirmation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddConfirmation.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddConfirmation.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddConfirmation.Location = new System.Drawing.Point(303, 611);
			this.butAddConfirmation.Name = "butAddConfirmation";
			this.butAddConfirmation.Size = new System.Drawing.Size(119, 24);
			this.butAddConfirmation.TabIndex = 93;
			this.butAddConfirmation.Text = "Add Confirmation";
			this.butAddConfirmation.UseVisualStyleBackColor = true;
			this.butAddConfirmation.Click += new System.EventHandler(this.butAddConfirmation_Click);
			// 
			// label54
			// 
			this.label54.Location = new System.Drawing.Point(277, 16);
			this.label54.Name = "label54";
			this.label54.Size = new System.Drawing.Size(57, 16);
			this.label54.TabIndex = 165;
			this.label54.Text = "Clinic";
			this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinicEConfirm
			// 
			this.comboClinicEConfirm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinicEConfirm.Location = new System.Drawing.Point(338, 15);
			this.comboClinicEConfirm.MaxDropDownItems = 30;
			this.comboClinicEConfirm.Name = "comboClinicEConfirm";
			this.comboClinicEConfirm.Size = new System.Drawing.Size(194, 21);
			this.comboClinicEConfirm.TabIndex = 164;
			this.comboClinicEConfirm.SelectedIndexChanged += new System.EventHandler(this.comboClinicEConfirm_SelectedIndexChanged);
			// 
			// checkIsConfirmEnabled
			// 
			this.checkIsConfirmEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsConfirmEnabled.Location = new System.Drawing.Point(541, 16);
			this.checkIsConfirmEnabled.Name = "checkIsConfirmEnabled";
			this.checkIsConfirmEnabled.Size = new System.Drawing.Size(216, 19);
			this.checkIsConfirmEnabled.TabIndex = 167;
			this.checkIsConfirmEnabled.Text = "Enable Automation for Clinic";
			this.checkIsConfirmEnabled.CheckedChanged += new System.EventHandler(this.checkIsConfirmEnabled_CheckedChanged);
			// 
			// groupAutomationStatuses
			// 
			this.groupAutomationStatuses.Controls.Add(this.radio2ClickConfirm);
			this.groupAutomationStatuses.Controls.Add(this.radio1ClickConfirm);
			this.groupAutomationStatuses.Controls.Add(this.comboStatusEFailed);
			this.groupAutomationStatuses.Controls.Add(this.label50);
			this.groupAutomationStatuses.Controls.Add(this.checkEnableNoClinic);
			this.groupAutomationStatuses.Controls.Add(this.comboStatusEDeclined);
			this.groupAutomationStatuses.Controls.Add(this.comboStatusESent);
			this.groupAutomationStatuses.Controls.Add(this.comboStatusEAccepted);
			this.groupAutomationStatuses.Controls.Add(this.label51);
			this.groupAutomationStatuses.Controls.Add(this.label52);
			this.groupAutomationStatuses.Controls.Add(this.label53);
			this.groupAutomationStatuses.Location = new System.Drawing.Point(25, 160);
			this.groupAutomationStatuses.Name = "groupAutomationStatuses";
			this.groupAutomationStatuses.Size = new System.Drawing.Size(272, 200);
			this.groupAutomationStatuses.TabIndex = 169;
			this.groupAutomationStatuses.TabStop = false;
			this.groupAutomationStatuses.Text = "Global eConfirmation Settings";
			// 
			// radio2ClickConfirm
			// 
			this.radio2ClickConfirm.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radio2ClickConfirm.Location = new System.Drawing.Point(3, 164);
			this.radio2ClickConfirm.Name = "radio2ClickConfirm";
			this.radio2ClickConfirm.Size = new System.Drawing.Size(245, 30);
			this.radio2ClickConfirm.TabIndex = 176;
			this.radio2ClickConfirm.TabStop = true;
			this.radio2ClickConfirm.Text = "Confirm in portal after clicking link (2-click confirmation)";
			this.radio2ClickConfirm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radio1ClickConfirm
			// 
			this.radio1ClickConfirm.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radio1ClickConfirm.Location = new System.Drawing.Point(3, 137);
			this.radio1ClickConfirm.Name = "radio1ClickConfirm";
			this.radio1ClickConfirm.Size = new System.Drawing.Size(245, 31);
			this.radio1ClickConfirm.TabIndex = 175;
			this.radio1ClickConfirm.TabStop = true;
			this.radio1ClickConfirm.Text = "Confirm from link in message (1-click confirmation)";
			this.radio1ClickConfirm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatusEFailed
			// 
			this.comboStatusEFailed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusEFailed.Location = new System.Drawing.Point(97, 92);
			this.comboStatusEFailed.Name = "comboStatusEFailed";
			this.comboStatusEFailed.Size = new System.Drawing.Size(151, 21);
			this.comboStatusEFailed.TabIndex = 173;
			// 
			// label50
			// 
			this.label50.Location = new System.Drawing.Point(6, 93);
			this.label50.Name = "label50";
			this.label50.Size = new System.Drawing.Size(89, 16);
			this.label50.TabIndex = 174;
			this.label50.Text = "Failed";
			this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEnableNoClinic
			// 
			this.checkEnableNoClinic.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableNoClinic.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnableNoClinic.Location = new System.Drawing.Point(9, 117);
			this.checkEnableNoClinic.Name = "checkEnableNoClinic";
			this.checkEnableNoClinic.Size = new System.Drawing.Size(239, 17);
			this.checkEnableNoClinic.TabIndex = 172;
			this.checkEnableNoClinic.Text = "Allow eMessages from Appts w/o Clinic";
			this.checkEnableNoClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatusEDeclined
			// 
			this.comboStatusEDeclined.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusEDeclined.Location = new System.Drawing.Point(97, 67);
			this.comboStatusEDeclined.Name = "comboStatusEDeclined";
			this.comboStatusEDeclined.Size = new System.Drawing.Size(151, 21);
			this.comboStatusEDeclined.TabIndex = 170;
			// 
			// comboStatusESent
			// 
			this.comboStatusESent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusESent.Location = new System.Drawing.Point(97, 17);
			this.comboStatusESent.Name = "comboStatusESent";
			this.comboStatusESent.Size = new System.Drawing.Size(151, 21);
			this.comboStatusESent.TabIndex = 166;
			// 
			// comboStatusEAccepted
			// 
			this.comboStatusEAccepted.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusEAccepted.Location = new System.Drawing.Point(97, 42);
			this.comboStatusEAccepted.Name = "comboStatusEAccepted";
			this.comboStatusEAccepted.Size = new System.Drawing.Size(151, 21);
			this.comboStatusEAccepted.TabIndex = 168;
			// 
			// label51
			// 
			this.label51.Location = new System.Drawing.Point(6, 68);
			this.label51.Name = "label51";
			this.label51.Size = new System.Drawing.Size(89, 16);
			this.label51.TabIndex = 171;
			this.label51.Text = "Not Accepted";
			this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label52
			// 
			this.label52.Location = new System.Drawing.Point(6, 18);
			this.label52.Name = "label52";
			this.label52.Size = new System.Drawing.Size(89, 16);
			this.label52.TabIndex = 167;
			this.label52.Text = "Sent";
			this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label53
			// 
			this.label53.Location = new System.Drawing.Point(6, 43);
			this.label53.Name = "label53";
			this.label53.Size = new System.Drawing.Size(89, 16);
			this.label53.TabIndex = 169;
			this.label53.Text = "Accepted";
			this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butActivateConfirm
			// 
			this.butActivateConfirm.Location = new System.Drawing.Point(150, 71);
			this.butActivateConfirm.Name = "butActivateConfirm";
			this.butActivateConfirm.Size = new System.Drawing.Size(147, 23);
			this.butActivateConfirm.TabIndex = 257;
			this.butActivateConfirm.Text = "Activate eConfirmations";
			this.butActivateConfirm.UseVisualStyleBackColor = true;
			this.butActivateConfirm.Click += new System.EventHandler(this.butActivateConfirm_Click);
			// 
			// butActivateReminder
			// 
			this.butActivateReminder.Location = new System.Drawing.Point(150, 42);
			this.butActivateReminder.Name = "butActivateReminder";
			this.butActivateReminder.Size = new System.Drawing.Size(147, 23);
			this.butActivateReminder.TabIndex = 261;
			this.butActivateReminder.Text = "Activate eReminders";
			this.butActivateReminder.UseVisualStyleBackColor = true;
			this.butActivateReminder.Click += new System.EventHandler(this.butActivateReminder_Click);
			// 
			// checkUseDefaultsEC
			// 
			this.checkUseDefaultsEC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkUseDefaultsEC.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseDefaultsEC.Location = new System.Drawing.Point(820, 613);
			this.checkUseDefaultsEC.Name = "checkUseDefaultsEC";
			this.checkUseDefaultsEC.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsEC.TabIndex = 263;
			this.checkUseDefaultsEC.Text = "Use Defaults";
			this.checkUseDefaultsEC.Click += new System.EventHandler(this.checkUseDefaultsEC_CheckedChanged);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(25, 567);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(272, 65);
			this.label11.TabIndex = 175;
			this.label11.Text = "Don\'t Send: If an appointment has this status, do not send a confirmation.\r\nDon\'t" +
    " Change: If an appointment has this status, do not change status when a response" +
    " is received.";
			// 
			// butAddThankYouVerify
			// 
			this.butAddThankYouVerify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddThankYouVerify.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddThankYouVerify.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddThankYouVerify.Location = new System.Drawing.Point(553, 611);
			this.butAddThankYouVerify.Name = "butAddThankYouVerify";
			this.butAddThankYouVerify.Size = new System.Drawing.Size(136, 24);
			this.butAddThankYouVerify.TabIndex = 271;
			this.butAddThankYouVerify.Text = "Add Auto Thank-You";
			this.butAddThankYouVerify.UseVisualStyleBackColor = true;
			this.butAddThankYouVerify.Click += new System.EventHandler(this.butAddThankYou_Click);
			// 
			// butActivateThanks
			// 
			this.butActivateThanks.Location = new System.Drawing.Point(150, 100);
			this.butActivateThanks.Name = "butActivateThanks";
			this.butActivateThanks.Size = new System.Drawing.Size(147, 23);
			this.butActivateThanks.TabIndex = 272;
			this.butActivateThanks.Text = "Activate Auto Thank-You";
			this.butActivateThanks.UseVisualStyleBackColor = true;
			this.butActivateThanks.Click += new System.EventHandler(this.butActivateThankYou_Click);
			// 
			// gridRemindersMain
			// 
			this.gridRemindersMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRemindersMain.HasMultilineHeaders = true;
			this.gridRemindersMain.Location = new System.Drawing.Point(303, 42);
			this.gridRemindersMain.Name = "gridRemindersMain";
			this.gridRemindersMain.Size = new System.Drawing.Size(852, 562);
			this.gridRemindersMain.TabIndex = 68;
			this.gridRemindersMain.Title = "Automated Messaging Rules";
			this.gridRemindersMain.TranslationName = "TableRules";
			this.gridRemindersMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRemindersMain_CellDoubleClick);
			// 
			// textStatusConfirmations
			// 
			this.textStatusConfirmations.Location = new System.Drawing.Point(13, 73);
			this.textStatusConfirmations.Name = "textStatusConfirmations";
			this.textStatusConfirmations.ReadOnly = true;
			this.textStatusConfirmations.Size = new System.Drawing.Size(131, 20);
			this.textStatusConfirmations.TabIndex = 260;
			this.textStatusConfirmations.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textStatusReminders
			// 
			this.textStatusReminders.Location = new System.Drawing.Point(13, 44);
			this.textStatusReminders.Name = "textStatusReminders";
			this.textStatusReminders.ReadOnly = true;
			this.textStatusReminders.Size = new System.Drawing.Size(131, 20);
			this.textStatusReminders.TabIndex = 262;
			this.textStatusReminders.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gridConfStatuses
			// 
			this.gridConfStatuses.HasMultilineHeaders = true;
			this.gridConfStatuses.Location = new System.Drawing.Point(25, 366);
			this.gridConfStatuses.Name = "gridConfStatuses";
			this.gridConfStatuses.Size = new System.Drawing.Size(272, 195);
			this.gridConfStatuses.TabIndex = 265;
			this.gridConfStatuses.Title = "Confirmation Statuses";
			this.gridConfStatuses.TranslationName = "TableStatuses";
			this.gridConfStatuses.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridConfStatuses_CellDoubleClick);
			// 
			// textStatusThankYous
			// 
			this.textStatusThankYous.Location = new System.Drawing.Point(13, 102);
			this.textStatusThankYous.Name = "textStatusThankYous";
			this.textStatusThankYous.ReadOnly = true;
			this.textStatusThankYous.Size = new System.Drawing.Size(131, 20);
			this.textStatusThankYous.TabIndex = 273;
			this.textStatusThankYous.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textThankYouTitle
			// 
			this.textThankYouTitle.AcceptsTab = true;
			this.textThankYouTitle.BackColor = System.Drawing.SystemColors.Window;
			this.textThankYouTitle.Location = new System.Drawing.Point(924, 16);
			this.textThankYouTitle.Name = "textThankYouTitle";
			this.textThankYouTitle.Size = new System.Drawing.Size(231, 20);
			this.textThankYouTitle.TabIndex = 274;
			// 
			// labelThankYouTitle
			// 
			this.labelThankYouTitle.Location = new System.Drawing.Point(710, 17);
			this.labelThankYouTitle.Name = "labelThankYouTitle";
			this.labelThankYouTitle.Size = new System.Drawing.Size(213, 16);
			this.labelThankYouTitle.TabIndex = 275;
			this.labelThankYouTitle.Text = "Auto Thank-You Calendar Event Title";
			this.labelThankYouTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1080, 643);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 501;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAddArrival
			// 
			this.butAddArrival.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddArrival.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddArrival.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddArrival.Location = new System.Drawing.Point(695, 611);
			this.butAddArrival.Name = "butAddArrival";
			this.butAddArrival.Size = new System.Drawing.Size(119, 24);
			this.butAddArrival.TabIndex = 502;
			this.butAddArrival.Text = "Add Arrival";
			this.butAddArrival.UseVisualStyleBackColor = true;
			this.butAddArrival.Click += new System.EventHandler(this.butAddArrival_Click);
			// 
			// textStatusArrivals
			// 
			this.textStatusArrivals.Location = new System.Drawing.Point(13, 131);
			this.textStatusArrivals.Name = "textStatusArrivals";
			this.textStatusArrivals.ReadOnly = true;
			this.textStatusArrivals.Size = new System.Drawing.Size(131, 20);
			this.textStatusArrivals.TabIndex = 504;
			this.textStatusArrivals.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// butActivateArrivals
			// 
			this.butActivateArrivals.Location = new System.Drawing.Point(150, 129);
			this.butActivateArrivals.Name = "butActivateArrivals";
			this.butActivateArrivals.Size = new System.Drawing.Size(147, 23);
			this.butActivateArrivals.TabIndex = 503;
			this.butActivateArrivals.Text = "Activate Arrivals";
			this.butActivateArrivals.UseVisualStyleBackColor = true;
			this.butActivateArrivals.Click += new System.EventHandler(this.butActivateArrivals_Click);
			// 
			// FormEServicesAutoMsging
			// 
			this.ClientSize = new System.Drawing.Size(1167, 679);
			this.Controls.Add(this.textStatusArrivals);
			this.Controls.Add(this.butActivateArrivals);
			this.Controls.Add(this.butAddArrival);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelThankYouTitle);
			this.Controls.Add(this.textThankYouTitle);
			this.Controls.Add(this.textStatusThankYous);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridConfStatuses);
			this.Controls.Add(this.textStatusReminders);
			this.Controls.Add(this.gridRemindersMain);
			this.Controls.Add(this.textStatusConfirmations);
			this.Controls.Add(this.butAddReminder);
			this.Controls.Add(this.butAddConfirmation);
			this.Controls.Add(this.butActivateThanks);
			this.Controls.Add(this.label54);
			this.Controls.Add(this.butAddThankYouVerify);
			this.Controls.Add(this.comboClinicEConfirm);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.checkIsConfirmEnabled);
			this.Controls.Add(this.checkUseDefaultsEC);
			this.Controls.Add(this.groupAutomationStatuses);
			this.Controls.Add(this.butActivateReminder);
			this.Controls.Add(this.butActivateConfirm);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesAutoMsging";
			this.Text = "eServices Automated Messaging";
			this.Load += new System.EventHandler(this.FormEServicesECR_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.groupAutomationStatuses.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butOK;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.ContextMenuStrip menuWebSchedVerifyTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem insertReplacementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private UI.Button butAddReminder;
		private UI.Button butAddConfirmation;
		private System.Windows.Forms.Label label54;
		private System.Windows.Forms.ComboBox comboClinicEConfirm;
		private System.Windows.Forms.CheckBox checkIsConfirmEnabled;
		private System.Windows.Forms.GroupBox groupAutomationStatuses;
		private System.Windows.Forms.RadioButton radio2ClickConfirm;
		private System.Windows.Forms.RadioButton radio1ClickConfirm;
		private UI.ComboBoxOD comboStatusEFailed;
		private System.Windows.Forms.Label label50;
		private System.Windows.Forms.CheckBox checkEnableNoClinic;
		private UI.ComboBoxOD comboStatusEDeclined;
		private UI.ComboBoxOD comboStatusESent;
		private UI.ComboBoxOD comboStatusEAccepted;
		private System.Windows.Forms.Label label51;
		private System.Windows.Forms.Label label52;
		private System.Windows.Forms.Label label53;
		private UI.Button butActivateConfirm;
		private UI.Button butActivateReminder;
		private System.Windows.Forms.CheckBox checkUseDefaultsEC;
		private System.Windows.Forms.Label label11;
		private UI.Button butAddThankYouVerify;
		private UI.Button butActivateThanks;
		private UI.GridOD gridRemindersMain;
		private System.Windows.Forms.TextBox textStatusConfirmations;
		private System.Windows.Forms.TextBox textStatusReminders;
		private UI.GridOD gridConfStatuses;
		private System.Windows.Forms.TextBox textStatusThankYous;
		private System.Windows.Forms.TextBox textThankYouTitle;
		private System.Windows.Forms.Label labelThankYouTitle;
		private UI.Button butCancel;
		private UI.Button butAddArrival;
		private System.Windows.Forms.TextBox textStatusArrivals;
		private UI.Button butActivateArrivals;
	}
}