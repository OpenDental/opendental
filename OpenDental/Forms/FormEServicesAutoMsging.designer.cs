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
			this.butActivateConfirm = new OpenDental.UI.Button();
			this.butActivateReminder = new OpenDental.UI.Button();
			this.butAddThankYouVerify = new OpenDental.UI.Button();
			this.butActivateThanks = new OpenDental.UI.Button();
			this.gridRemindersMain = new OpenDental.UI.GridOD();
			this.textStatusConfirmations = new System.Windows.Forms.TextBox();
			this.textStatusReminders = new System.Windows.Forms.TextBox();
			this.textStatusThankYous = new System.Windows.Forms.TextBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butAddArrival = new OpenDental.UI.Button();
			this.textStatusArrivals = new System.Windows.Forms.TextBox();
			this.butActivateArrivals = new OpenDental.UI.Button();
			this.textStatusNotifications = new System.Windows.Forms.TextBox();
			this.butActivateInvites = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.butAddPatientPortalInvite = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkUseDefaultsInviteBefore = new System.Windows.Forms.CheckBox();
			this.butAddPatientPortalInviteBefore = new OpenDental.UI.Button();
			this.checkUseDefaultThanks = new System.Windows.Forms.CheckBox();
			this.checkUseDefaultsConfirmation = new System.Windows.Forms.CheckBox();
			this.checkUseDefaultsReminder = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkUseDefaultsArrival = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.checkUseDefaultsGeneralMessage = new System.Windows.Forms.CheckBox();
			this.butAddGeneralMessage = new OpenDental.UI.Button();
			this.checkUseDefaultsInvite = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.textStatusGeneralMessage = new System.Windows.Forms.TextBox();
			this.butActivateGeneralMessages = new OpenDental.UI.Button();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
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
			this.butOK.Click += new System.EventHandler(this.ButOK_Click);
			// 
			// butAddReminder
			// 
			this.butAddReminder.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddReminder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddReminder.Location = new System.Drawing.Point(156, 17);
			this.butAddReminder.Name = "butAddReminder";
			this.butAddReminder.Size = new System.Drawing.Size(185, 24);
			this.butAddReminder.TabIndex = 92;
			this.butAddReminder.Text = "Add eReminder";
			this.butAddReminder.UseVisualStyleBackColor = true;
			this.butAddReminder.Click += new System.EventHandler(this.ButAddReminder_Click);
			// 
			// butAddConfirmation
			// 
			this.butAddConfirmation.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddConfirmation.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddConfirmation.Location = new System.Drawing.Point(156, 46);
			this.butAddConfirmation.Name = "butAddConfirmation";
			this.butAddConfirmation.Size = new System.Drawing.Size(185, 24);
			this.butAddConfirmation.TabIndex = 93;
			this.butAddConfirmation.Text = "Add eConfirmation";
			this.butAddConfirmation.UseVisualStyleBackColor = true;
			this.butAddConfirmation.Click += new System.EventHandler(this.ButAddConfirmation_Click);
			// 
			// label54
			// 
			this.label54.Location = new System.Drawing.Point(304, 33);
			this.label54.Name = "label54";
			this.label54.Size = new System.Drawing.Size(57, 16);
			this.label54.TabIndex = 165;
			this.label54.Text = "Clinic";
			this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinicEConfirm
			// 
			this.comboClinicEConfirm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinicEConfirm.Location = new System.Drawing.Point(365, 32);
			this.comboClinicEConfirm.MaxDropDownItems = 30;
			this.comboClinicEConfirm.Name = "comboClinicEConfirm";
			this.comboClinicEConfirm.Size = new System.Drawing.Size(194, 21);
			this.comboClinicEConfirm.TabIndex = 164;
			this.comboClinicEConfirm.SelectionChangeCommitted += new System.EventHandler(this.comboClinicEConfirm_SelectionChangeCommitted);
			// 
			// checkIsConfirmEnabled
			// 
			this.checkIsConfirmEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsConfirmEnabled.Location = new System.Drawing.Point(568, 33);
			this.checkIsConfirmEnabled.Name = "checkIsConfirmEnabled";
			this.checkIsConfirmEnabled.Size = new System.Drawing.Size(198, 19);
			this.checkIsConfirmEnabled.TabIndex = 167;
			this.checkIsConfirmEnabled.Text = "Enable Automation for Clinic";
			this.checkIsConfirmEnabled.Click += new System.EventHandler(this.checkIsConfirmEnabled_Click);
			// 
			// butActivateConfirm
			// 
			this.butActivateConfirm.Location = new System.Drawing.Point(164, 46);
			this.butActivateConfirm.Name = "butActivateConfirm";
			this.butActivateConfirm.Size = new System.Drawing.Size(177, 23);
			this.butActivateConfirm.TabIndex = 257;
			this.butActivateConfirm.Text = "Activate eConfirmations";
			this.butActivateConfirm.UseVisualStyleBackColor = true;
			this.butActivateConfirm.Click += new System.EventHandler(this.ButActivateConfirm_Click);
			// 
			// butActivateReminder
			// 
			this.butActivateReminder.Location = new System.Drawing.Point(164, 17);
			this.butActivateReminder.Name = "butActivateReminder";
			this.butActivateReminder.Size = new System.Drawing.Size(177, 23);
			this.butActivateReminder.TabIndex = 261;
			this.butActivateReminder.Text = "Activate eReminders";
			this.butActivateReminder.UseVisualStyleBackColor = true;
			this.butActivateReminder.Click += new System.EventHandler(this.ButActivateReminder_Click);
			// 
			// butAddThankYouVerify
			// 
			this.butAddThankYouVerify.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddThankYouVerify.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddThankYouVerify.Location = new System.Drawing.Point(156, 75);
			this.butAddThankYouVerify.Name = "butAddThankYouVerify";
			this.butAddThankYouVerify.Size = new System.Drawing.Size(185, 24);
			this.butAddThankYouVerify.TabIndex = 271;
			this.butAddThankYouVerify.Text = "Add Auto Thank-You";
			this.butAddThankYouVerify.UseVisualStyleBackColor = true;
			this.butAddThankYouVerify.Click += new System.EventHandler(this.ButAddThankYou_Click);
			// 
			// butActivateThanks
			// 
			this.butActivateThanks.Location = new System.Drawing.Point(164, 75);
			this.butActivateThanks.Name = "butActivateThanks";
			this.butActivateThanks.Size = new System.Drawing.Size(177, 23);
			this.butActivateThanks.TabIndex = 272;
			this.butActivateThanks.Text = "Activate Auto Thank-You";
			this.butActivateThanks.UseVisualStyleBackColor = true;
			this.butActivateThanks.Click += new System.EventHandler(this.ButActivateThankYou_Click);
			// 
			// gridRemindersMain
			// 
			this.gridRemindersMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRemindersMain.HasMultilineHeaders = true;
			this.gridRemindersMain.Location = new System.Drawing.Point(365, 59);
			this.gridRemindersMain.Name = "gridRemindersMain";
			this.gridRemindersMain.Size = new System.Drawing.Size(790, 553);
			this.gridRemindersMain.TabIndex = 68;
			this.gridRemindersMain.Title = "Automated Messaging Rules";
			this.gridRemindersMain.TranslationName = "TableRules";
			this.gridRemindersMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.GridRemindersMain_CellDoubleClick);
			// 
			// textStatusConfirmations
			// 
			this.textStatusConfirmations.Location = new System.Drawing.Point(6, 48);
			this.textStatusConfirmations.Name = "textStatusConfirmations";
			this.textStatusConfirmations.ReadOnly = true;
			this.textStatusConfirmations.Size = new System.Drawing.Size(152, 20);
			this.textStatusConfirmations.TabIndex = 260;
			this.textStatusConfirmations.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textStatusReminders
			// 
			this.textStatusReminders.Location = new System.Drawing.Point(6, 19);
			this.textStatusReminders.Name = "textStatusReminders";
			this.textStatusReminders.ReadOnly = true;
			this.textStatusReminders.Size = new System.Drawing.Size(152, 20);
			this.textStatusReminders.TabIndex = 262;
			this.textStatusReminders.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// textStatusThankYous
			// 
			this.textStatusThankYous.Location = new System.Drawing.Point(6, 77);
			this.textStatusThankYous.Name = "textStatusThankYous";
			this.textStatusThankYous.ReadOnly = true;
			this.textStatusThankYous.Size = new System.Drawing.Size(152, 20);
			this.textStatusThankYous.TabIndex = 273;
			this.textStatusThankYous.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
			this.butCancel.Click += new System.EventHandler(this.ButCancel_Click);
			// 
			// butAddArrival
			// 
			this.butAddArrival.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddArrival.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddArrival.Location = new System.Drawing.Point(156, 17);
			this.butAddArrival.Name = "butAddArrival";
			this.butAddArrival.Size = new System.Drawing.Size(185, 24);
			this.butAddArrival.TabIndex = 502;
			this.butAddArrival.Text = "Add Arrival";
			this.butAddArrival.UseVisualStyleBackColor = true;
			this.butAddArrival.Click += new System.EventHandler(this.ButAddArrival_Click);
			// 
			// textStatusArrivals
			// 
			this.textStatusArrivals.Location = new System.Drawing.Point(6, 106);
			this.textStatusArrivals.Name = "textStatusArrivals";
			this.textStatusArrivals.ReadOnly = true;
			this.textStatusArrivals.Size = new System.Drawing.Size(152, 20);
			this.textStatusArrivals.TabIndex = 504;
			this.textStatusArrivals.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// butActivateArrivals
			// 
			this.butActivateArrivals.Location = new System.Drawing.Point(164, 104);
			this.butActivateArrivals.Name = "butActivateArrivals";
			this.butActivateArrivals.Size = new System.Drawing.Size(177, 23);
			this.butActivateArrivals.TabIndex = 503;
			this.butActivateArrivals.Text = "Activate Arrivals";
			this.butActivateArrivals.UseVisualStyleBackColor = true;
			this.butActivateArrivals.Click += new System.EventHandler(this.ButActivateArrivals_Click);
			// 
			// textStatusNotifications
			// 
			this.textStatusNotifications.Location = new System.Drawing.Point(6, 135);
			this.textStatusNotifications.Name = "textStatusNotifications";
			this.textStatusNotifications.ReadOnly = true;
			this.textStatusNotifications.Size = new System.Drawing.Size(152, 20);
			this.textStatusNotifications.TabIndex = 507;
			this.textStatusNotifications.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// butActivateInvites
			// 
			this.butActivateInvites.Location = new System.Drawing.Point(164, 133);
			this.butActivateInvites.Name = "butActivateInvites";
			this.butActivateInvites.Size = new System.Drawing.Size(177, 23);
			this.butActivateInvites.TabIndex = 506;
			this.butActivateInvites.Text = "Activate Patient Portal Invites";
			this.butActivateInvites.UseVisualStyleBackColor = true;
			this.butActivateInvites.Click += new System.EventHandler(this.butActivateInvites_Click);
			// 
			// menuMain
			// 
			this.menuMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(1167, 24);
			this.menuMain.TabIndex = 509;
			this.menuMain.Text = "menuOD1";
			// 
			// butAddPatientPortalInvite
			// 
			this.butAddPatientPortalInvite.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddPatientPortalInvite.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddPatientPortalInvite.Location = new System.Drawing.Point(156, 17);
			this.butAddPatientPortalInvite.Name = "butAddPatientPortalInvite";
			this.butAddPatientPortalInvite.Size = new System.Drawing.Size(185, 24);
			this.butAddPatientPortalInvite.TabIndex = 510;
			this.butAddPatientPortalInvite.Text = "Add Patient Portal Invite";
			this.butAddPatientPortalInvite.UseVisualStyleBackColor = true;
			this.butAddPatientPortalInvite.Click += new System.EventHandler(this.butAddPatientPortalInvite_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkUseDefaultsInviteBefore);
			this.groupBox1.Controls.Add(this.butAddPatientPortalInviteBefore);
			this.groupBox1.Controls.Add(this.checkUseDefaultThanks);
			this.groupBox1.Controls.Add(this.checkUseDefaultsConfirmation);
			this.groupBox1.Controls.Add(this.checkUseDefaultsReminder);
			this.groupBox1.Controls.Add(this.butAddThankYouVerify);
			this.groupBox1.Controls.Add(this.butAddReminder);
			this.groupBox1.Controls.Add(this.butAddConfirmation);
			this.groupBox1.Location = new System.Drawing.Point(12, 279);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(347, 136);
			this.groupBox1.TabIndex = 512;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Before Appointment";
			// 
			// checkUseDefaultsInviteBefore
			// 
			this.checkUseDefaultsInviteBefore.Location = new System.Drawing.Point(6, 109);
			this.checkUseDefaultsInviteBefore.Name = "checkUseDefaultsInviteBefore";
			this.checkUseDefaultsInviteBefore.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsInviteBefore.TabIndex = 276;
			this.checkUseDefaultsInviteBefore.Text = "Use Defaults";
			this.checkUseDefaultsInviteBefore.UseVisualStyleBackColor = true;
			this.checkUseDefaultsInviteBefore.Click += new System.EventHandler(this.checkUseDefaultsInviteBefore_Click);
			// 
			// butAddPatientPortalInviteBefore
			// 
			this.butAddPatientPortalInviteBefore.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddPatientPortalInviteBefore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddPatientPortalInviteBefore.Location = new System.Drawing.Point(156, 105);
			this.butAddPatientPortalInviteBefore.Name = "butAddPatientPortalInviteBefore";
			this.butAddPatientPortalInviteBefore.Size = new System.Drawing.Size(185, 24);
			this.butAddPatientPortalInviteBefore.TabIndex = 275;
			this.butAddPatientPortalInviteBefore.Text = "Add Patient Portal Invite";
			this.butAddPatientPortalInviteBefore.UseVisualStyleBackColor = true;
			this.butAddPatientPortalInviteBefore.Click += new System.EventHandler(this.butAddPatientPortalInviteBefore_Click);
			// 
			// checkUseDefaultThanks
			// 
			this.checkUseDefaultThanks.Location = new System.Drawing.Point(6, 79);
			this.checkUseDefaultThanks.Name = "checkUseDefaultThanks";
			this.checkUseDefaultThanks.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultThanks.TabIndex = 274;
			this.checkUseDefaultThanks.Text = "Use Defaults";
			this.checkUseDefaultThanks.UseVisualStyleBackColor = true;
			this.checkUseDefaultThanks.Click += new System.EventHandler(this.checkUseDefaultThanks_Click);
			// 
			// checkUseDefaultsConfirmation
			// 
			this.checkUseDefaultsConfirmation.Location = new System.Drawing.Point(6, 50);
			this.checkUseDefaultsConfirmation.Name = "checkUseDefaultsConfirmation";
			this.checkUseDefaultsConfirmation.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsConfirmation.TabIndex = 273;
			this.checkUseDefaultsConfirmation.Text = "Use Defaults";
			this.checkUseDefaultsConfirmation.UseVisualStyleBackColor = true;
			this.checkUseDefaultsConfirmation.Click += new System.EventHandler(this.checkUseDefaultsConfirmation_Click);
			// 
			// checkUseDefaultsReminder
			// 
			this.checkUseDefaultsReminder.Location = new System.Drawing.Point(6, 21);
			this.checkUseDefaultsReminder.Name = "checkUseDefaultsReminder";
			this.checkUseDefaultsReminder.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsReminder.TabIndex = 272;
			this.checkUseDefaultsReminder.Text = "Use Defaults";
			this.checkUseDefaultsReminder.UseVisualStyleBackColor = true;
			this.checkUseDefaultsReminder.Click += new System.EventHandler(this.checkUseDefaultsReminder_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkUseDefaultsArrival);
			this.groupBox2.Controls.Add(this.butAddArrival);
			this.groupBox2.Location = new System.Drawing.Point(12, 421);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(347, 48);
			this.groupBox2.TabIndex = 513;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "At Appointment";
			// 
			// checkUseDefaultsArrival
			// 
			this.checkUseDefaultsArrival.Location = new System.Drawing.Point(6, 21);
			this.checkUseDefaultsArrival.Name = "checkUseDefaultsArrival";
			this.checkUseDefaultsArrival.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsArrival.TabIndex = 503;
			this.checkUseDefaultsArrival.Text = "Use Defaults";
			this.checkUseDefaultsArrival.UseVisualStyleBackColor = true;
			this.checkUseDefaultsArrival.Click += new System.EventHandler(this.checkUseDefaultsArrival_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.checkUseDefaultsGeneralMessage);
			this.groupBox3.Controls.Add(this.butAddGeneralMessage);
			this.groupBox3.Controls.Add(this.checkUseDefaultsInvite);
			this.groupBox3.Controls.Add(this.butAddPatientPortalInvite);
			this.groupBox3.Location = new System.Drawing.Point(12, 475);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(347, 77);
			this.groupBox3.TabIndex = 514;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "After Appointment";
			// 
			// checkUseDefaultsGeneralMessage
			// 
			this.checkUseDefaultsGeneralMessage.Location = new System.Drawing.Point(6, 50);
			this.checkUseDefaultsGeneralMessage.Name = "checkUseDefaultsGeneralMessage";
			this.checkUseDefaultsGeneralMessage.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsGeneralMessage.TabIndex = 511;
			this.checkUseDefaultsGeneralMessage.Text = "Use Defaults";
			this.checkUseDefaultsGeneralMessage.UseVisualStyleBackColor = true;
			this.checkUseDefaultsGeneralMessage.Click += new System.EventHandler(this.checkUseDefaultsGeneralMessage_Click);
			// 
			// butAddGeneralMessage
			// 
			this.butAddGeneralMessage.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddGeneralMessage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddGeneralMessage.Location = new System.Drawing.Point(156, 46);
			this.butAddGeneralMessage.Name = "butAddGeneralMessage";
			this.butAddGeneralMessage.Size = new System.Drawing.Size(185, 24);
			this.butAddGeneralMessage.TabIndex = 512;
			this.butAddGeneralMessage.Text = "Add General Message";
			this.butAddGeneralMessage.UseVisualStyleBackColor = true;
			this.butAddGeneralMessage.Click += new System.EventHandler(this.butAddGeneralMessage_Click);
			// 
			// checkUseDefaultsInvite
			// 
			this.checkUseDefaultsInvite.Location = new System.Drawing.Point(6, 21);
			this.checkUseDefaultsInvite.Name = "checkUseDefaultsInvite";
			this.checkUseDefaultsInvite.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsInvite.TabIndex = 504;
			this.checkUseDefaultsInvite.Text = "Use Defaults";
			this.checkUseDefaultsInvite.UseVisualStyleBackColor = true;
			this.checkUseDefaultsInvite.Click += new System.EventHandler(this.checkUseDefaultsInvite_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.textStatusGeneralMessage);
			this.groupBox4.Controls.Add(this.butActivateGeneralMessages);
			this.groupBox4.Controls.Add(this.butActivateReminder);
			this.groupBox4.Controls.Add(this.butActivateConfirm);
			this.groupBox4.Controls.Add(this.butActivateThanks);
			this.groupBox4.Controls.Add(this.textStatusConfirmations);
			this.groupBox4.Controls.Add(this.textStatusReminders);
			this.groupBox4.Controls.Add(this.textStatusThankYous);
			this.groupBox4.Controls.Add(this.textStatusNotifications);
			this.groupBox4.Controls.Add(this.butActivateArrivals);
			this.groupBox4.Controls.Add(this.butActivateInvites);
			this.groupBox4.Controls.Add(this.textStatusArrivals);
			this.groupBox4.Location = new System.Drawing.Point(12, 59);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(347, 193);
			this.groupBox4.TabIndex = 515;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Activate for Practice";
			// 
			// textStatusGeneralMessage
			// 
			this.textStatusGeneralMessage.Location = new System.Drawing.Point(6, 164);
			this.textStatusGeneralMessage.Name = "textStatusGeneralMessage";
			this.textStatusGeneralMessage.ReadOnly = true;
			this.textStatusGeneralMessage.Size = new System.Drawing.Size(152, 20);
			this.textStatusGeneralMessage.TabIndex = 509;
			this.textStatusGeneralMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// butActivateGeneralMessages
			// 
			this.butActivateGeneralMessages.Location = new System.Drawing.Point(164, 162);
			this.butActivateGeneralMessages.Name = "butActivateGeneralMessages";
			this.butActivateGeneralMessages.Size = new System.Drawing.Size(177, 23);
			this.butActivateGeneralMessages.TabIndex = 508;
			this.butActivateGeneralMessages.Text = "Activate General Messages";
			this.butActivateGeneralMessages.UseVisualStyleBackColor = true;
			this.butActivateGeneralMessages.Click += new System.EventHandler(this.butActivateGeneralMessages_Click);
			// 
			// FormEServicesAutoMsging
			// 
			this.ClientSize = new System.Drawing.Size(1167, 679);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.menuMain);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridRemindersMain);
			this.Controls.Add(this.label54);
			this.Controls.Add(this.comboClinicEConfirm);
			this.Controls.Add(this.checkIsConfirmEnabled);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesAutoMsging";
			this.Text = "eServices Automated Messaging";
			this.Load += new System.EventHandler(this.FormEServicesAutoMsging_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

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
		private UI.Button butActivateConfirm;
		private UI.Button butActivateReminder;
		private UI.Button butAddThankYouVerify;
		private UI.Button butActivateThanks;
		private UI.GridOD gridRemindersMain;
		private System.Windows.Forms.TextBox textStatusConfirmations;
		private System.Windows.Forms.TextBox textStatusReminders;
		private System.Windows.Forms.TextBox textStatusThankYous;
		private UI.Button butCancel;
		private UI.Button butAddArrival;
		private System.Windows.Forms.TextBox textStatusArrivals;
		private UI.Button butActivateArrivals;
		private System.Windows.Forms.TextBox textStatusNotifications;
		private UI.Button butActivateInvites;
		private UI.MenuOD menuMain;
		private UI.Button butAddPatientPortalInvite;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkUseDefaultThanks;
		private System.Windows.Forms.CheckBox checkUseDefaultsConfirmation;
		private System.Windows.Forms.CheckBox checkUseDefaultsReminder;
		private System.Windows.Forms.CheckBox checkUseDefaultsArrival;
		private System.Windows.Forms.CheckBox checkUseDefaultsInvite;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox textStatusGeneralMessage;
		private UI.Button butActivateGeneralMessages;
		private System.Windows.Forms.CheckBox checkUseDefaultsInviteBefore;
		private UI.Button butAddPatientPortalInviteBefore;
		private System.Windows.Forms.CheckBox checkUseDefaultsGeneralMessage;
		private UI.Button butAddGeneralMessage;
	}
}