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
			this.groupSendTime = new System.Windows.Forms.GroupBox();
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
			this.groupSendOrder = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.labelRuleType = new System.Windows.Forms.Label();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.butAdvanced = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.labelShortCodes = new System.Windows.Forms.Label();
			this.groupShortCodes = new System.Windows.Forms.GroupBox();
			this.butLanguage = new OpenDental.UI.Button();
			this.tabPageDefault = new System.Windows.Forms.TabPage();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.checkEConfirmationAutoReplies = new System.Windows.Forms.CheckBox();
			this.butRemove = new OpenDental.UI.Button();
			this.groupSendTime.SuspendLayout();
			this.groupSendOrder.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupShortCodes.SuspendLayout();
			this.tabControl.SuspendLayout();
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
			this.groupSendTime.TabStop = false;
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
			this.labelDoNotSendWithin.Size = new System.Drawing.Size(221, 28);
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
			this.groupSendOrder.Controls.Add(this.gridPriorities);
			this.groupSendOrder.Controls.Add(this.checkSendAll);
			this.groupSendOrder.Controls.Add(this.butDown);
			this.groupSendOrder.Controls.Add(this.butUp);
			this.groupSendOrder.Location = new System.Drawing.Point(18, 105);
			this.groupSendOrder.Name = "groupSendOrder";
			this.groupSendOrder.Size = new System.Drawing.Size(415, 140);
			this.groupSendOrder.TabIndex = 114;
			this.groupSendOrder.TabStop = false;
			this.groupSendOrder.Text = "Send Order";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.Controls.Add(this.labelTags);
			this.groupBox2.Location = new System.Drawing.Point(18, 599);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(713, 56);
			this.groupBox2.TabIndex = 115;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Template Replacement Tags";
			// 
			// labelRuleType
			// 
			this.labelRuleType.Location = new System.Drawing.Point(297, 0);
			this.labelRuleType.Name = "labelRuleType";
			this.labelRuleType.Size = new System.Drawing.Size(282, 21);
			this.labelRuleType.TabIndex = 16;
			this.labelRuleType.Text = "labelRuleType";
			this.labelRuleType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(163, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 21);
			this.label2.TabIndex = 126;
			this.label2.Text = "Reminder Rule Type:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelShortCodes
			// 
			this.labelShortCodes.Location = new System.Drawing.Point(6, 18);
			this.labelShortCodes.Name = "labelShortCodes";
			this.labelShortCodes.Size = new System.Drawing.Size(280, 118);
			this.labelShortCodes.TabIndex = 127;
			// 
			// groupShortCodes
			// 
			this.groupShortCodes.Controls.Add(this.labelShortCodes);
			this.groupShortCodes.Location = new System.Drawing.Point(439, 106);
			this.groupShortCodes.Name = "groupShortCodes";
			this.groupShortCodes.Size = new System.Drawing.Size(292, 139);
			this.groupShortCodes.TabIndex = 128;
			this.groupShortCodes.TabStop = false;
			this.groupShortCodes.Text = "Short Codes";
			this.groupShortCodes.Visible = false;
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
			this.tabPageDefault.Size = new System.Drawing.Size(710, 303);
			this.tabPageDefault.TabIndex = 0;
			this.tabPageDefault.Text = "Default";
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
			this.tabControl.Size = new System.Drawing.Size(718, 329);
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
			// FormApptReminderRuleEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(743, 696);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.checkEConfirmationAutoReplies);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.butLanguage);
			this.Controls.Add(this.groupShortCodes);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelRuleType);
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
			this.groupShortCodes.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
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
		private System.Windows.Forms.GroupBox groupSendTime;
		private System.Windows.Forms.GroupBox groupSendOrder;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label labelRuleType;
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
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelShortCodes;
		private System.Windows.Forms.GroupBox groupShortCodes;
		private UI.Button butLanguage;
		private System.Windows.Forms.TabPage tabPageDefault;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.CheckBox checkEConfirmationAutoReplies;
		private UI.Button butRemove;
	}
}