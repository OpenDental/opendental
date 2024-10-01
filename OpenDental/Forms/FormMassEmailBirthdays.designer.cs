namespace OpenDental{
	partial class FormMassEmailBirthdays {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmailBirthdays));
			this.butSave = new OpenDental.UI.Button();
			this.checkBoxSendGuarantorBirthdayForMinor = new OpenDental.UI.CheckBox();
			this.groupSendTime = new OpenDental.UI.GroupBox();
			this.radioOnBirthday = new System.Windows.Forms.RadioButton();
			this.radioAfterBirthday = new System.Windows.Forms.RadioButton();
			this.radioBeforeBirthday = new System.Windows.Forms.RadioButton();
			this.textDays = new OpenDental.ValidNum();
			this.label32 = new System.Windows.Forms.Label();
			this.checkUseDefaultsBirthday = new OpenDental.UI.CheckBox();
			this.checkIsEnabled = new OpenDental.UI.CheckBox();
			this.butEditTemplate = new OpenDental.UI.Button();
			this.tabControlLanguages = new OpenDental.UI.TabControl();
			this.tabPageDefault = new OpenDental.UI.TabPage();
			this.userControlEmailTemplateDefault = new OpenDental.UserControlEmailTemplate();
			this.butLangaugeAdd = new OpenDental.UI.Button();
			this.butLanguageRemove = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboMinorAge = new OpenDental.UI.ComboBox();
			this.labelNotActivated = new System.Windows.Forms.Label();
			this.labelDefaults = new System.Windows.Forms.Label();
			this.groupSendTime.SuspendLayout();
			this.tabControlLanguages.SuspendLayout();
			this.tabPageDefault.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(846, 660);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkBoxSendGuarantorBirthdayForMinor
			// 
			this.checkBoxSendGuarantorBirthdayForMinor.Location = new System.Drawing.Point(278, 17);
			this.checkBoxSendGuarantorBirthdayForMinor.Name = "checkBoxSendGuarantorBirthdayForMinor";
			this.checkBoxSendGuarantorBirthdayForMinor.Size = new System.Drawing.Size(254, 38);
			this.checkBoxSendGuarantorBirthdayForMinor.TabIndex = 284;
			this.checkBoxSendGuarantorBirthdayForMinor.Text = "Do not send birthday greetings to patients under ";
			this.checkBoxSendGuarantorBirthdayForMinor.CheckedChanged += new System.EventHandler(this.checkBoxSendGuarantorBirthdayForMinor_CheckedChanged);
			// 
			// groupSendTime
			// 
			this.groupSendTime.Controls.Add(this.radioOnBirthday);
			this.groupSendTime.Controls.Add(this.radioAfterBirthday);
			this.groupSendTime.Controls.Add(this.radioBeforeBirthday);
			this.groupSendTime.Controls.Add(this.textDays);
			this.groupSendTime.Controls.Add(this.label32);
			this.groupSendTime.Location = new System.Drawing.Point(12, 12);
			this.groupSendTime.Name = "groupSendTime";
			this.groupSendTime.Size = new System.Drawing.Size(260, 77);
			this.groupSendTime.TabIndex = 283;
			this.groupSendTime.Text = "Send Time";
			// 
			// radioOnBirthday
			// 
			this.radioOnBirthday.Location = new System.Drawing.Point(120, 51);
			this.radioOnBirthday.Name = "radioOnBirthday";
			this.radioOnBirthday.Size = new System.Drawing.Size(110, 20);
			this.radioOnBirthday.TabIndex = 21;
			this.radioOnBirthday.TabStop = true;
			this.radioOnBirthday.Text = "On birthday";
			this.radioOnBirthday.Click += new System.EventHandler(this.radioOnBirthday_Click);
			// 
			// radioAfterBirthday
			// 
			this.radioAfterBirthday.Location = new System.Drawing.Point(120, 30);
			this.radioAfterBirthday.Name = "radioAfterBirthday";
			this.radioAfterBirthday.Size = new System.Drawing.Size(127, 20);
			this.radioAfterBirthday.TabIndex = 20;
			this.radioAfterBirthday.TabStop = true;
			this.radioAfterBirthday.Text = "After birthday";
			this.radioAfterBirthday.Click += new System.EventHandler(this.radioAfterBirthday_Click);
			// 
			// radioBeforeBirthday
			// 
			this.radioBeforeBirthday.Location = new System.Drawing.Point(120, 8);
			this.radioBeforeBirthday.Name = "radioBeforeBirthday";
			this.radioBeforeBirthday.Size = new System.Drawing.Size(127, 20);
			this.radioBeforeBirthday.TabIndex = 19;
			this.radioBeforeBirthday.TabStop = true;
			this.radioBeforeBirthday.Text = "Before birthday";
			this.radioBeforeBirthday.Click += new System.EventHandler(this.radioBeforeBirthday_Click);
			// 
			// textDays
			// 
			this.textDays.Location = new System.Drawing.Point(18, 23);
			this.textDays.MaxVal = 364;
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(51, 20);
			this.textDays.TabIndex = 18;
			// 
			// label32
			// 
			this.label32.Location = new System.Drawing.Point(71, 22);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(43, 21);
			this.label32.TabIndex = 17;
			this.label32.Text = "Days";
			this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkUseDefaultsBirthday
			// 
			this.checkUseDefaultsBirthday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkUseDefaultsBirthday.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseDefaultsBirthday.Location = new System.Drawing.Point(812, 36);
			this.checkUseDefaultsBirthday.Name = "checkUseDefaultsBirthday";
			this.checkUseDefaultsBirthday.Size = new System.Drawing.Size(105, 19);
			this.checkUseDefaultsBirthday.TabIndex = 280;
			this.checkUseDefaultsBirthday.Text = "Use Defaults";
			this.checkUseDefaultsBirthday.Click += new System.EventHandler(this.checkUseDefaultsBirthday_Click);
			// 
			// checkIsEnabled
			// 
			this.checkIsEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIsEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsEnabled.Location = new System.Drawing.Point(812, 57);
			this.checkIsEnabled.Name = "checkIsEnabled";
			this.checkIsEnabled.Size = new System.Drawing.Size(105, 19);
			this.checkIsEnabled.TabIndex = 294;
			this.checkIsEnabled.Text = "Enabled";
			this.checkIsEnabled.Click += new System.EventHandler(this.checkIsEnabled_Click);
			// 
			// butEditTemplate
			// 
			this.butEditTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEditTemplate.Location = new System.Drawing.Point(16, 660);
			this.butEditTemplate.Name = "butEditTemplate";
			this.butEditTemplate.Size = new System.Drawing.Size(101, 24);
			this.butEditTemplate.TabIndex = 296;
			this.butEditTemplate.Text = "Edit Template";
			this.butEditTemplate.Click += new System.EventHandler(this.butEditTemplate_Click);
			// 
			// tabControlLanguages
			// 
			this.tabControlLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlLanguages.Controls.Add(this.tabPageDefault);
			this.tabControlLanguages.Location = new System.Drawing.Point(12, 113);
			this.tabControlLanguages.Name = "tabControlLanguages";
			this.tabControlLanguages.Size = new System.Drawing.Size(909, 541);
			this.tabControlLanguages.TabIndex = 297;
			// 
			// tabPageDefault
			// 
			this.tabPageDefault.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageDefault.Controls.Add(this.userControlEmailTemplateDefault);
			this.tabPageDefault.Location = new System.Drawing.Point(2, 21);
			this.tabPageDefault.Name = "tabPageDefault";
			this.tabPageDefault.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDefault.Size = new System.Drawing.Size(905, 518);
			this.tabPageDefault.TabIndex = 0;
			this.tabPageDefault.Text = "Default Language";
			// 
			// userControlEmailTemplateDefault
			// 
			this.userControlEmailTemplateDefault.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.userControlEmailTemplateDefault.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userControlEmailTemplateDefault.Location = new System.Drawing.Point(3, 3);
			this.userControlEmailTemplateDefault.Name = "userControlEmailTemplateDefault";
			this.userControlEmailTemplateDefault.Size = new System.Drawing.Size(899, 512);
			this.userControlEmailTemplateDefault.TabIndex = 296;
			// 
			// butLangaugeAdd
			// 
			this.butLangaugeAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLangaugeAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLangaugeAdd.Location = new System.Drawing.Point(123, 660);
			this.butLangaugeAdd.Name = "butLangaugeAdd";
			this.butLangaugeAdd.Size = new System.Drawing.Size(101, 24);
			this.butLangaugeAdd.TabIndex = 298;
			this.butLangaugeAdd.Text = "Add Language";
			this.butLangaugeAdd.Click += new System.EventHandler(this.butLangaugeAdd_Click);
			// 
			// butLanguageRemove
			// 
			this.butLanguageRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLanguageRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLanguageRemove.Location = new System.Drawing.Point(230, 660);
			this.butLanguageRemove.Name = "butLanguageRemove";
			this.butLanguageRemove.Size = new System.Drawing.Size(108, 24);
			this.butLanguageRemove.TabIndex = 299;
			this.butLanguageRemove.Text = "Remove Language";
			this.butLanguageRemove.Click += new System.EventHandler(this.butLanguageRemove_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(717, 12);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(200, 21);
			this.comboClinic.TabIndex = 300;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// comboMinorAge
			// 
			this.comboMinorAge.Location = new System.Drawing.Point(538, 14);
			this.comboMinorAge.Name = "comboMinorAge";
			this.comboMinorAge.Size = new System.Drawing.Size(70, 21);
			this.comboMinorAge.TabIndex = 301;
			// 
			// labelNotActivated
			// 
			this.labelNotActivated.ForeColor = System.Drawing.Color.Red;
			this.labelNotActivated.Location = new System.Drawing.Point(758, 79);
			this.labelNotActivated.Name = "labelNotActivated";
			this.labelNotActivated.Size = new System.Drawing.Size(163, 17);
			this.labelNotActivated.TabIndex = 326;
			this.labelNotActivated.Text = "* Clinic is not signed up";
			this.labelNotActivated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDefaults
			// 
			this.labelDefaults.ForeColor = System.Drawing.Color.Black;
			this.labelDefaults.Location = new System.Drawing.Point(739, 37);
			this.labelDefaults.Name = "labelDefaults";
			this.labelDefaults.Size = new System.Drawing.Size(163, 17);
			this.labelDefaults.TabIndex = 327;
			this.labelDefaults.Text = "(Defaults)";
			this.labelDefaults.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormMassEmailBirthdays
			// 
			this.ClientSize = new System.Drawing.Size(933, 696);
			this.Controls.Add(this.labelDefaults);
			this.Controls.Add(this.labelNotActivated);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.comboMinorAge);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butLanguageRemove);
			this.Controls.Add(this.butLangaugeAdd);
			this.Controls.Add(this.butEditTemplate);
			this.Controls.Add(this.tabControlLanguages);
			this.Controls.Add(this.checkIsEnabled);
			this.Controls.Add(this.checkBoxSendGuarantorBirthdayForMinor);
			this.Controls.Add(this.groupSendTime);
			this.Controls.Add(this.checkUseDefaultsBirthday);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmailBirthdays";
			this.Text = "Mass Email Birthdays";
			this.Load += new System.EventHandler(this.FormMassEmailBirthdays_Load);
			this.groupSendTime.ResumeLayout(false);
			this.groupSendTime.PerformLayout();
			this.tabControlLanguages.ResumeLayout(false);
			this.tabPageDefault.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.CheckBox checkBoxSendGuarantorBirthdayForMinor;
		private OpenDental.UI.GroupBox groupSendTime;
		private System.Windows.Forms.RadioButton radioAfterBirthday;
		private System.Windows.Forms.RadioButton radioBeforeBirthday;
		private ValidNum textDays;
		private System.Windows.Forms.Label label32;
		private OpenDental.UI.CheckBox checkUseDefaultsBirthday;
		private OpenDental.UI.CheckBox checkIsEnabled;
		private UI.Button butEditTemplate;
		private OpenDental.UI.TabControl tabControlLanguages;
		private OpenDental.UI.TabPage tabPageDefault;
		private UserControlEmailTemplate userControlEmailTemplateDefault;
		private UI.Button butLangaugeAdd;
		private UI.Button butLanguageRemove;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.RadioButton radioOnBirthday;
		private OpenDental.UI.ComboBox comboMinorAge;
		private System.Windows.Forms.Label labelNotActivated;
		private System.Windows.Forms.Label labelDefaults;
	}
}