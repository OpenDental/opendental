namespace OpenDental {
	partial class FormApptReminderRuleEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptReminderRuleEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.labelLeadTime = new System.Windows.Forms.Label();
			this.butOk = new OpenDental.UI.Button();
			this.gridPriorities = new OpenDental.UI.GridOD();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.checkSendAll = new System.Windows.Forms.CheckBox();
			this.labelTags = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.groupSendTime = new OpenDental.UI.GroupBoxOD();
			this.textDaysWithin = new OpenDental.ValidNum();
			this.labelDaysWithin = new System.Windows.Forms.Label();
			this.labelDoNotSendWithin = new System.Windows.Forms.Label();
			this.radioAfterAppt = new System.Windows.Forms.RadioButton();
			this.radioBeforeAppt = new System.Windows.Forms.RadioButton();
			this.textDays = new OpenDental.ValidNum();
			this.textHoursWithin = new OpenDental.ValidNum();
			this.label1 = new System.Windows.Forms.Label();
			this.textHours = new OpenDental.ValidNum();
			this.labelHoursWithin = new System.Windows.Forms.Label();
			this.groupSendOrder = new OpenDental.UI.GroupBoxOD();
			this.groupBox2 = new OpenDental.UI.GroupBoxOD();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.butAdvanced = new OpenDental.UI.Button();
			this.butLanguage = new OpenDental.UI.Button();
			this.tabPageDefault = new System.Windows.Forms.TabPage();
			this.checkSendSecureEmail = new System.Windows.Forms.CheckBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.checkEConfirmationAutoReplies = new System.Windows.Forms.CheckBox();
			this.butRemove = new OpenDental.UI.Button();
			this.groupPatientPortalInvites = new OpenDental.UI.GroupBoxOD();
			this.radioSendPatientPortalInviteNoVisit = new System.Windows.Forms.RadioButton();
			this.textPatientPortalLastVisit = new OpenDental.ValidNum();
			this.radioSendPatientPortalInviteMultiple = new System.Windows.Forms.RadioButton();
			this.radioSendPatientPortalInviteOnce = new System.Windows.Forms.RadioButton();
			this.groupSendTime.SuspendLayout();
			this.groupSendOrder.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.groupPatientPortalInvites.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(656, 661);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelLeadTime
			// 
			this.labelLeadTime.Location = new System.Drawing.Point(7, 48);
			this.labelLeadTime.Name = "labelLeadTime";
			this.labelLeadTime.Size = new System.Drawing.Size(43, 21);
			this.labelLeadTime.TabIndex = 15;
			this.labelLeadTime.Text = "Hours";
			this.labelLeadTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(574, 661);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(76, 26);
			this.butOk.TabIndex = 124;
			this.butOk.Text = "&OK";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// gridPriorities
			// 
			this.gridPriorities.Location = new System.Drawing.Point(42, 19);
			this.gridPriorities.Name = "gridPriorities";
			this.gridPriorities.Size = new System.Drawing.Size(359, 96);
			this.gridPriorities.TabIndex = 106;
			this.gridPriorities.Title = "Contact Methods";
			this.gridPriorities.TranslationName = "TableContactMethods";
			// 
			// butUp
			// 
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.Location = new System.Drawing.Point(6, 19);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(30, 30);
			this.butUp.TabIndex = 103;
			this.butUp.UseVisualStyleBackColor = true;
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.Location = new System.Drawing.Point(6, 55);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(30, 30);
			this.butDown.TabIndex = 104;
			this.butDown.UseVisualStyleBackColor = true;
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// checkSendAll
			// 
			this.checkSendAll.Location = new System.Drawing.Point(42, 119);
			this.checkSendAll.Name = "checkSendAll";
			this.checkSendAll.Size = new System.Drawing.Size(359, 18);
			this.checkSendAll.TabIndex = 105;
			this.checkSendAll.Text = "Send All - If available, send text AND email.";
			this.checkSendAll.UseVisualStyleBackColor = true;
			this.checkSendAll.CheckedChanged += new System.EventHandler(this.checkSendAll_CheckedChanged);
			// 
			// labelTags
			// 
			this.labelTags.Location = new System.Drawing.Point(3, 16);
			this.labelTags.Name = "labelTags";
			this.labelTags.Size = new System.Drawing.Size(698, 37);
			this.labelTags.TabIndex = 110;
			this.labelTags.Text = "Use template tags to create dynamic messages.";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 661);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 26);
			this.butDelete.TabIndex = 111;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// groupSendTime
			// 
			this.groupSendTime.BackColor = System.Drawing.Color.White;
			this.groupSendTime.Controls.Add(this.textDaysWithin);
			this.groupSendTime.Controls.Add(this.labelDaysWithin);
			this.groupSendTime.Controls.Add(this.labelDoNotSendWithin);
			this.groupSendTime.Controls.Add(this.radioAfterAppt);
			this.groupSendTime.Controls.Add(this.radioBeforeAppt);
			this.groupSendTime.Controls.Add(this.textDays);
			this.groupSendTime.Controls.Add(this.textHoursWithin);
			this.groupSendTime.Controls.Add(this.label1);
			this.groupSendTime.Controls.Add(this.textHours);
			this.groupSendTime.Controls.Add(this.labelLeadTime);
			this.groupSendTime.Controls.Add(this.labelHoursWithin);
			this.groupSendTime.Location = new System.Drawing.Point(18, 23);
			this.groupSendTime.Name = "groupSendTime";
			this.groupSendTime.Size = new System.Drawing.Size(496, 77);
			this.groupSendTime.TabIndex = 101;
			this.groupSendTime.Text = "Send Time";
			// 
			// textDaysWithin
			// 
			this.textDaysWithin.Location = new System.Drawing.Point(279, 46);
			this.textDaysWithin.MaxVal = 366;
			this.textDaysWithin.Name = "textDaysWithin";
			this.textDaysWithin.ShowZero = false;
			this.textDaysWithin.Size = new System.Drawing.Size(51, 20);
			this.textDaysWithin.TabIndex = 24;
			this.textDaysWithin.TextChanged += new System.EventHandler(this.textDoNotSendWithin_TextChanged);
			// 
			// labelDaysWithin
			// 
			this.labelDaysWithin.Location = new System.Drawing.Point(332, 45);
			this.labelDaysWithin.Name = "labelDaysWithin";
			this.labelDaysWithin.Size = new System.Drawing.Size(43, 21);
			this.labelDaysWithin.TabIndex = 23;
			this.labelDaysWithin.Text = "Days";
			this.labelDaysWithin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDoNotSendWithin
			// 
			this.labelDoNotSendWithin.Location = new System.Drawing.Point(261, 13);
			this.labelDoNotSendWithin.Name = "labelDoNotSendWithin";
			this.labelDoNotSendWithin.Size = new System.Drawing.Size(218, 28);
			this.labelDoNotSendWithin.TabIndex = 17;
			this.labelDoNotSendWithin.Text = "Do not send within _____________ of appointment";
			// 
			// radioAfterAppt
			// 
			this.radioAfterAppt.Location = new System.Drawing.Point(125, 47);
			this.radioAfterAppt.Name = "radioAfterAppt";
			this.radioAfterAppt.Size = new System.Drawing.Size(127, 20);
			this.radioAfterAppt.TabIndex = 20;
			this.radioAfterAppt.TabStop = true;
			this.radioAfterAppt.Text = "After appointment";
			this.radioAfterAppt.CheckedChanged += new System.EventHandler(this.radioBeforeAfterAppt_CheckedChanged);
			// 
			// radioBeforeAppt
			// 
			this.radioBeforeAppt.Location = new System.Drawing.Point(125, 25);
			this.radioBeforeAppt.Name = "radioBeforeAppt";
			this.radioBeforeAppt.Size = new System.Drawing.Size(127, 20);
			this.radioBeforeAppt.TabIndex = 19;
			this.radioBeforeAppt.TabStop = true;
			this.radioBeforeAppt.Text = "Before appointment";
			this.radioBeforeAppt.CheckedChanged += new System.EventHandler(this.radioBeforeAfterAppt_CheckedChanged);
			// 
			// textDays
			// 
			this.textDays.Location = new System.Drawing.Point(52, 23);
			this.textDays.MaxVal = 366;
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(51, 20);
			this.textDays.TabIndex = 18;
			// 
			// textHoursWithin
			// 
			this.textHoursWithin.Location = new System.Drawing.Point(375, 47);
			this.textHoursWithin.MaxVal = 23;
			this.textHoursWithin.Name = "textHoursWithin";
			this.textHoursWithin.ShowZero = false;
			this.textHoursWithin.Size = new System.Drawing.Size(51, 20);
			this.textHoursWithin.TabIndex = 22;
			this.textHoursWithin.TextChanged += new System.EventHandler(this.textDoNotSendWithin_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 21);
			this.label1.TabIndex = 17;
			this.label1.Text = "Days";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHours
			// 
			this.textHours.Location = new System.Drawing.Point(52, 49);
			this.textHours.MaxVal = 23;
			this.textHours.Name = "textHours";
			this.textHours.Size = new System.Drawing.Size(51, 20);
			this.textHours.TabIndex = 16;
			// 
			// labelHoursWithin
			// 
			this.labelHoursWithin.Location = new System.Drawing.Point(428, 46);
			this.labelHoursWithin.Name = "labelHoursWithin";
			this.labelHoursWithin.Size = new System.Drawing.Size(43, 21);
			this.labelHoursWithin.TabIndex = 21;
			this.labelHoursWithin.Text = "Hours";
			this.labelHoursWithin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupSendOrder
			// 
			this.groupSendOrder.BackColor = System.Drawing.Color.White;
			this.groupSendOrder.Controls.Add(this.gridPriorities);
			this.groupSendOrder.Controls.Add(this.checkSendAll);
			this.groupSendOrder.Controls.Add(this.butDown);
			this.groupSendOrder.Controls.Add(this.butUp);
			this.groupSendOrder.Location = new System.Drawing.Point(18, 105);
			this.groupSendOrder.Name = "groupSendOrder";
			this.groupSendOrder.Size = new System.Drawing.Size(415, 140);
			this.groupSendOrder.TabIndex = 114;
			this.groupSendOrder.Text = "Send Order";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.labelTags);
			this.groupBox2.Location = new System.Drawing.Point(18, 599);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(713, 56);
			this.groupBox2.TabIndex = 115;
			this.groupBox2.Text = "Template Replacement Tags";
			// 
			// checkEnabled
			// 
			this.checkEnabled.Location = new System.Drawing.Point(28, 2);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(90, 18);
			this.checkEnabled.TabIndex = 107;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// butAdvanced
			// 
			this.butAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdvanced.Location = new System.Drawing.Point(234, 661);
			this.butAdvanced.Name = "butAdvanced";
			this.butAdvanced.Size = new System.Drawing.Size(75, 26);
			this.butAdvanced.TabIndex = 125;
			this.butAdvanced.Text = "&Advanced";
			this.butAdvanced.UseVisualStyleBackColor = true;
			this.butAdvanced.Click += new System.EventHandler(this.butAdvanced_Click);
			// 
			// butLanguage
			// 
			this.butLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butLanguage.Location = new System.Drawing.Point(315, 661);
			this.butLanguage.Name = "butLanguage";
			this.butLanguage.Size = new System.Drawing.Size(83, 26);
			this.butLanguage.TabIndex = 129;
			this.butLanguage.Text = "Add Language";
			this.butLanguage.UseVisualStyleBackColor = true;
			this.butLanguage.Click += new System.EventHandler(this.butLanguage_Click);
			// 
			// tabPageDefault
			// 
			this.tabPageDefault.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageDefault.Location = new System.Drawing.Point(4, 22);
			this.tabPageDefault.Name = "tabPageDefault";
			this.tabPageDefault.Size = new System.Drawing.Size(705, 303);
			this.tabPageDefault.TabIndex = 0;
			this.tabPageDefault.Text = "Default";
			// 
			// checkSendSecureEmail
			// 
			this.checkSendSecureEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkSendSecureEmail.Location = new System.Drawing.Point(29, 567);
			this.checkSendSecureEmail.Name = "checkSendSecureEmail";
			this.checkSendSecureEmail.Size = new System.Drawing.Size(179, 18);
			this.checkSendSecureEmail.TabIndex = 134;
			this.checkSendSecureEmail.Text = "Send Secure Email";
			this.checkSendSecureEmail.UseVisualStyleBackColor = true;
			this.checkSendSecureEmail.Click += new System.EventHandler(this.CheckSendSecureEmail_Click);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPageDefault);
			this.tabControl.Location = new System.Drawing.Point(18, 266);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(713, 329);
			this.tabControl.TabIndex = 130;
			// 
			// checkEConfirmationAutoReplies
			// 
			this.checkEConfirmationAutoReplies.Location = new System.Drawing.Point(18, 246);
			this.checkEConfirmationAutoReplies.Name = "checkEConfirmationAutoReplies";
			this.checkEConfirmationAutoReplies.Size = new System.Drawing.Size(713, 18);
			this.checkEConfirmationAutoReplies.TabIndex = 131;
			this.checkEConfirmationAutoReplies.Text = "Send text message auto replies - Template can be edited by clicking \'Advanced\'";
			this.checkEConfirmationAutoReplies.UseVisualStyleBackColor = true;
			// 
			// butRemove
			// 
			this.butRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemove.Location = new System.Drawing.Point(404, 661);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(105, 26);
			this.butRemove.TabIndex = 132;
			this.butRemove.Text = "Remove Language";
			this.butRemove.UseVisualStyleBackColor = true;
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// groupPatientPortalInvites
			// 
			this.groupPatientPortalInvites.BackColor = System.Drawing.Color.White;
			this.groupPatientPortalInvites.Controls.Add(this.radioSendPatientPortalInviteNoVisit);
			this.groupPatientPortalInvites.Controls.Add(this.textPatientPortalLastVisit);
			this.groupPatientPortalInvites.Controls.Add(this.radioSendPatientPortalInviteMultiple);
			this.groupPatientPortalInvites.Controls.Add(this.radioSendPatientPortalInviteOnce);
			this.groupPatientPortalInvites.Location = new System.Drawing.Point(439, 105);
			this.groupPatientPortalInvites.Name = "groupPatientPortalInvites";
			this.groupPatientPortalInvites.Size = new System.Drawing.Size(292, 140);
			this.groupPatientPortalInvites.TabIndex = 133;
			this.groupPatientPortalInvites.Text = "Patient Portal Invites";
			this.groupPatientPortalInvites.Visible = false;
			// 
			// radioSendPatientPortalInviteNoVisit
			// 
			this.radioSendPatientPortalInviteNoVisit.Location = new System.Drawing.Point(10, 81);
			this.radioSendPatientPortalInviteNoVisit.Name = "radioSendPatientPortalInviteNoVisit";
			this.radioSendPatientPortalInviteNoVisit.Size = new System.Drawing.Size(221, 47);
			this.radioSendPatientPortalInviteNoVisit.TabIndex = 24;
			this.radioSendPatientPortalInviteNoVisit.TabStop = true;
			this.radioSendPatientPortalInviteNoVisit.Text = "Invite once per appointment if patient has not visited Portal in _____ days";
			this.radioSendPatientPortalInviteNoVisit.UseVisualStyleBackColor = true;
			this.radioSendPatientPortalInviteNoVisit.Click += new System.EventHandler(this.radioSendPatientPortalInviteNoVisit_Click);
			// 
			// textPatientPortalLastVisit
			// 
			this.textPatientPortalLastVisit.Location = new System.Drawing.Point(234, 96);
			this.textPatientPortalLastVisit.MaxVal = 366;
			this.textPatientPortalLastVisit.Name = "textPatientPortalLastVisit";
			this.textPatientPortalLastVisit.Size = new System.Drawing.Size(51, 20);
			this.textPatientPortalLastVisit.TabIndex = 22;
			// 
			// radioSendPatientPortalInviteMultiple
			// 
			this.radioSendPatientPortalInviteMultiple.Location = new System.Drawing.Point(10, 55);
			this.radioSendPatientPortalInviteMultiple.Name = "radioSendPatientPortalInviteMultiple";
			this.radioSendPatientPortalInviteMultiple.Size = new System.Drawing.Size(206, 20);
			this.radioSendPatientPortalInviteMultiple.TabIndex = 21;
			this.radioSendPatientPortalInviteMultiple.TabStop = true;
			this.radioSendPatientPortalInviteMultiple.Text = "Invite once per appointment";
			this.radioSendPatientPortalInviteMultiple.Click += new System.EventHandler(this.radioSendPatientPortalInviteMultiple_Click);
			// 
			// radioSendPatientPortalInviteOnce
			// 
			this.radioSendPatientPortalInviteOnce.Location = new System.Drawing.Point(10, 22);
			this.radioSendPatientPortalInviteOnce.Name = "radioSendPatientPortalInviteOnce";
			this.radioSendPatientPortalInviteOnce.Size = new System.Drawing.Size(206, 20);
			this.radioSendPatientPortalInviteOnce.TabIndex = 20;
			this.radioSendPatientPortalInviteOnce.TabStop = true;
			this.radioSendPatientPortalInviteOnce.Text = "Invite until patient visits Portal";
			this.radioSendPatientPortalInviteOnce.Click += new System.EventHandler(this.radioSendPatientPortalInviteOnce_Click);
			// 
			// FormApptReminderRuleEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(743, 696);
			this.Controls.Add(this.groupPatientPortalInvites);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.checkEConfirmationAutoReplies);
			this.Controls.Add(this.checkSendSecureEmail);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.butLanguage);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butAdvanced);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupSendOrder);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.groupSendTime);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptReminderRuleEdit";
			this.Text = "Appointment Reminder Rule";
			this.Load += new System.EventHandler(this.FormApptReminderRuleEdit_Load);
			this.groupSendTime.ResumeLayout(false);
			this.groupSendTime.PerformLayout();
			this.groupSendOrder.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.groupPatientPortalInvites.ResumeLayout(false);
			this.groupPatientPortalInvites.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butCancel;
		private System.Windows.Forms.Label labelLeadTime;
		private UI.Button butOk;
		private UI.GridOD gridPriorities;
		private UI.Button butUp;
		private UI.Button butDown;
		private System.Windows.Forms.CheckBox checkSendAll;
		private System.Windows.Forms.Label labelTags;
		private UI.Button butDelete;
		private OpenDental.UI.GroupBoxOD groupSendTime;
		private OpenDental.UI.GroupBoxOD groupSendOrder;
		private OpenDental.UI.GroupBoxOD groupBox2;
		private ValidNum textHours;
		private ValidNum textDays;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkEnabled;
		private UI.Button butAdvanced;
		private System.Windows.Forms.RadioButton radioAfterAppt;
		private System.Windows.Forms.RadioButton radioBeforeAppt;
		private ValidNum textDaysWithin;
		private System.Windows.Forms.Label labelDaysWithin;
		private System.Windows.Forms.Label labelDoNotSendWithin;
		private ValidNum textHoursWithin;
		private System.Windows.Forms.Label labelHoursWithin;
		private UI.Button butLanguage;
		private System.Windows.Forms.TabPage tabPageDefault;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.CheckBox checkEConfirmationAutoReplies;
		private UI.Button butRemove;
		private UI.GroupBoxOD groupPatientPortalInvites;
		private ValidNum textPatientPortalLastVisit;
		private System.Windows.Forms.RadioButton radioSendPatientPortalInviteMultiple;
		private System.Windows.Forms.RadioButton radioSendPatientPortalInviteOnce;
		private System.Windows.Forms.RadioButton radioSendPatientPortalInviteNoVisit;
		public System.Windows.Forms.CheckBox checkSendSecureEmail;
	}
}