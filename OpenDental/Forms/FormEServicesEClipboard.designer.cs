namespace OpenDental{
	partial class FormEServicesEClipboard{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesEClipboard));
			this.label37 = new System.Windows.Forms.Label();
			this.menuWebSchedVerifyTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.insertReplacementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.butSave = new OpenDental.UI.Button();
			this.checkEClipboardUseDefaults = new OpenDental.UI.CheckBox();
			this.groupEClipboardRules = new OpenDental.UI.GroupBox();
			this.checkEClipboardAllowPaymentCheckIn = new OpenDental.UI.CheckBox();
			this.checkDisplayIndividually = new OpenDental.UI.CheckBox();
			this.checkRequire2FA = new OpenDental.UI.CheckBox();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.butConfStatuses = new OpenDental.UI.Button();
			this.textByodSmsTemplate = new System.Windows.Forms.TextBox();
			this.checkAppendByodToArrivalResponseSms = new OpenDental.UI.CheckBox();
			this.checkEnableByodSms = new OpenDental.UI.CheckBox();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.checkEClipboardCreateMissingForms = new OpenDental.UI.CheckBox();
			this.labelEClipboardMessage = new System.Windows.Forms.Label();
			this.textEClipboardMessage = new System.Windows.Forms.TextBox();
			this.checkEClipboardAllowSheets = new OpenDental.UI.CheckBox();
			this.checkEClipboardAllowCheckIn = new OpenDental.UI.CheckBox();
			this.checkEClipboardPopupKiosk = new OpenDental.UI.CheckBox();
			this.butImageAdd = new OpenDental.UI.Button();
			this.clinicPickerEClipboard = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelEClipboardNotSignedUp = new System.Windows.Forms.Label();
			this.butBrandingProfile = new OpenDental.UI.Button();
			this.butEFormAdd = new OpenDental.UI.Button();
			this.butSheetAdd = new OpenDental.UI.Button();
			this.butEClipboardUp = new OpenDental.UI.Button();
			this.butEClipboardDown = new OpenDental.UI.Button();
			this.gridForms = new OpenDental.UI.GridOD();
			this.gridImages = new OpenDental.UI.GridOD();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			this.groupEClipboardRules.SuspendLayout();
			this.groupBox2.SuspendLayout();
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
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(1105, 662);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 500;
			this.butSave.Text = "&Save";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkEClipboardUseDefaults
			// 
			this.checkEClipboardUseDefaults.Location = new System.Drawing.Point(11, 33);
			this.checkEClipboardUseDefaults.Name = "checkEClipboardUseDefaults";
			this.checkEClipboardUseDefaults.Size = new System.Drawing.Size(298, 17);
			this.checkEClipboardUseDefaults.TabIndex = 269;
			this.checkEClipboardUseDefaults.Text = "Use Defaults for this clinic";
			this.checkEClipboardUseDefaults.Click += new System.EventHandler(this.CheckEClipboardUseDefaults_Click);
			// 
			// groupEClipboardRules
			// 
			this.groupEClipboardRules.Controls.Add(this.checkEClipboardAllowPaymentCheckIn);
			this.groupEClipboardRules.Controls.Add(this.checkDisplayIndividually);
			this.groupEClipboardRules.Controls.Add(this.checkRequire2FA);
			this.groupEClipboardRules.Controls.Add(this.groupBox2);
			this.groupEClipboardRules.Controls.Add(this.groupBox1);
			this.groupEClipboardRules.Controls.Add(this.checkEClipboardCreateMissingForms);
			this.groupEClipboardRules.Controls.Add(this.labelEClipboardMessage);
			this.groupEClipboardRules.Controls.Add(this.textEClipboardMessage);
			this.groupEClipboardRules.Controls.Add(this.checkEClipboardAllowSheets);
			this.groupEClipboardRules.Controls.Add(this.checkEClipboardAllowCheckIn);
			this.groupEClipboardRules.Controls.Add(this.checkEClipboardPopupKiosk);
			this.groupEClipboardRules.Location = new System.Drawing.Point(11, 53);
			this.groupEClipboardRules.Name = "groupEClipboardRules";
			this.groupEClipboardRules.Size = new System.Drawing.Size(1123, 207);
			this.groupEClipboardRules.TabIndex = 265;
			this.groupEClipboardRules.Text = "Behavior Rules";
			// 
			// checkEClipboardAllowPaymentCheckIn
			// 
			this.checkEClipboardAllowPaymentCheckIn.Location = new System.Drawing.Point(19, 50);
			this.checkEClipboardAllowPaymentCheckIn.Name = "checkEClipboardAllowPaymentCheckIn";
			this.checkEClipboardAllowPaymentCheckIn.Size = new System.Drawing.Size(458, 17);
			this.checkEClipboardAllowPaymentCheckIn.TabIndex = 504;
			this.checkEClipboardAllowPaymentCheckIn.Text = "Allow payment when patient is checking in";
			this.checkEClipboardAllowPaymentCheckIn.Click += new System.EventHandler(this.CheckAllowPaymentCheckIn_Click);
			// 
			// checkDisplayIndividually
			// 
			this.checkDisplayIndividually.Location = new System.Drawing.Point(19, 122);
			this.checkDisplayIndividually.Name = "checkDisplayIndividually";
			this.checkDisplayIndividually.Size = new System.Drawing.Size(458, 17);
			this.checkDisplayIndividually.TabIndex = 503;
			this.checkDisplayIndividually.Text = "Display check-in questions individually";
			this.checkDisplayIndividually.Click += new System.EventHandler(this.checkDisplayIndividually_Click);
			// 
			// checkRequire2FA
			// 
			this.checkRequire2FA.Location = new System.Drawing.Point(19, 104);
			this.checkRequire2FA.Name = "checkRequire2FA";
			this.checkRequire2FA.Size = new System.Drawing.Size(458, 17);
			this.checkRequire2FA.TabIndex = 502;
			this.checkRequire2FA.Text = "Require patients to complete authentication before showing sheets";
			this.checkRequire2FA.Click += new System.EventHandler(this.checkRequire2FA_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.butConfStatuses);
			this.groupBox2.Controls.Add(this.textByodSmsTemplate);
			this.groupBox2.Controls.Add(this.checkAppendByodToArrivalResponseSms);
			this.groupBox2.Controls.Add(this.checkEnableByodSms);
			this.groupBox2.Location = new System.Drawing.Point(541, 15);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(461, 90);
			this.groupBox2.TabIndex = 272;
			this.groupBox2.Text = "Send eClipboard BYOD to patient phones";
			// 
			// butConfStatuses
			// 
			this.butConfStatuses.Location = new System.Drawing.Point(337, 3);
			this.butConfStatuses.Name = "butConfStatuses";
			this.butConfStatuses.Size = new System.Drawing.Size(121, 24);
			this.butConfStatuses.TabIndex = 272;
			this.butConfStatuses.Text = "Confirmation Statuses";
			this.butConfStatuses.UseVisualStyleBackColor = true;
			this.butConfStatuses.Click += new System.EventHandler(this.butConfStatuses_Click);
			// 
			// textByodSmsTemplate
			// 
			this.textByodSmsTemplate.Location = new System.Drawing.Point(6, 60);
			this.textByodSmsTemplate.Name = "textByodSmsTemplate";
			this.textByodSmsTemplate.Size = new System.Drawing.Size(452, 20);
			this.textByodSmsTemplate.TabIndex = 271;
			this.textByodSmsTemplate.TextChanged += new System.EventHandler(this.TextByodSmsTemplate_TextChanged);
			this.textByodSmsTemplate.Leave += new System.EventHandler(this.textByodSmsTemplate_Leave);
			// 
			// checkAppendByodToArrivalResponseSms
			// 
			this.checkAppendByodToArrivalResponseSms.Location = new System.Drawing.Point(6, 40);
			this.checkAppendByodToArrivalResponseSms.Name = "checkAppendByodToArrivalResponseSms";
			this.checkAppendByodToArrivalResponseSms.Size = new System.Drawing.Size(452, 17);
			this.checkAppendByodToArrivalResponseSms.TabIndex = 270;
			this.checkAppendByodToArrivalResponseSms.Text = "Append eClipboard BYOD to Automated Arrival text messages";
			this.checkAppendByodToArrivalResponseSms.Click += new System.EventHandler(this.checkAppendByodToArrivalResponseSms_Click);
			// 
			// checkEnableByodSms
			// 
			this.checkEnableByodSms.Location = new System.Drawing.Point(6, 21);
			this.checkEnableByodSms.Name = "checkEnableByodSms";
			this.checkEnableByodSms.Size = new System.Drawing.Size(452, 17);
			this.checkEnableByodSms.TabIndex = 269;
			this.checkEnableByodSms.Text = "Allow eClipboard BYOD via text messages";
			this.checkEnableByodSms.Click += new System.EventHandler(this.checkEnableByodSms_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(-34, -99);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 100);
			this.groupBox1.TabIndex = 271;
			this.groupBox1.Text = "groupBox1";
			// 
			// checkEClipboardCreateMissingForms
			// 
			this.checkEClipboardCreateMissingForms.Location = new System.Drawing.Point(19, 68);
			this.checkEClipboardCreateMissingForms.Name = "checkEClipboardCreateMissingForms";
			this.checkEClipboardCreateMissingForms.Size = new System.Drawing.Size(458, 17);
			this.checkEClipboardCreateMissingForms.TabIndex = 267;
			this.checkEClipboardCreateMissingForms.Text = "Add specified forms upon patient arrival";
			this.checkEClipboardCreateMissingForms.Click += new System.EventHandler(this.CheckEClipboardCreateMissingForms_Click);
			// 
			// labelEClipboardMessage
			// 
			this.labelEClipboardMessage.Location = new System.Drawing.Point(538, 110);
			this.labelEClipboardMessage.Name = "labelEClipboardMessage";
			this.labelEClipboardMessage.Size = new System.Drawing.Size(461, 18);
			this.labelEClipboardMessage.TabIndex = 266;
			this.labelEClipboardMessage.Text = "Message to show patients after successful check-in";
			this.labelEClipboardMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textEClipboardMessage
			// 
			this.textEClipboardMessage.Location = new System.Drawing.Point(541, 131);
			this.textEClipboardMessage.Multiline = true;
			this.textEClipboardMessage.Name = "textEClipboardMessage";
			this.textEClipboardMessage.Size = new System.Drawing.Size(458, 66);
			this.textEClipboardMessage.TabIndex = 3;
			this.textEClipboardMessage.TextChanged += new System.EventHandler(this.TextEClipboardMessage_TextChanged);
			// 
			// checkEClipboardAllowSheets
			// 
			this.checkEClipboardAllowSheets.Location = new System.Drawing.Point(19, 32);
			this.checkEClipboardAllowSheets.Name = "checkEClipboardAllowSheets";
			this.checkEClipboardAllowSheets.Size = new System.Drawing.Size(458, 17);
			this.checkEClipboardAllowSheets.TabIndex = 2;
			this.checkEClipboardAllowSheets.Text = "Allow patients to fill out forms in mobile app";
			this.checkEClipboardAllowSheets.Click += new System.EventHandler(this.CheckEClipboardAllowSheets_Click);
			// 
			// checkEClipboardAllowCheckIn
			// 
			this.checkEClipboardAllowCheckIn.Location = new System.Drawing.Point(19, 14);
			this.checkEClipboardAllowCheckIn.Name = "checkEClipboardAllowCheckIn";
			this.checkEClipboardAllowCheckIn.Size = new System.Drawing.Size(458, 17);
			this.checkEClipboardAllowCheckIn.TabIndex = 1;
			this.checkEClipboardAllowCheckIn.Text = "Allow self check-in";
			this.checkEClipboardAllowCheckIn.Click += new System.EventHandler(this.CheckEClipboardAllowCheckIn_Click);
			// 
			// checkEClipboardPopupKiosk
			// 
			this.checkEClipboardPopupKiosk.Location = new System.Drawing.Point(19, 86);
			this.checkEClipboardPopupKiosk.Name = "checkEClipboardPopupKiosk";
			this.checkEClipboardPopupKiosk.Size = new System.Drawing.Size(458, 17);
			this.checkEClipboardPopupKiosk.TabIndex = 0;
			this.checkEClipboardPopupKiosk.Text = "Show kiosk manager when staff changes patient status to arrived";
			this.checkEClipboardPopupKiosk.Click += new System.EventHandler(this.CheckEClipboardPopupKiosk_Click);
			// 
			// butImageAdd
			// 
			this.butImageAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butImageAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butImageAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butImageAdd.Location = new System.Drawing.Point(11, 556);
			this.butImageAdd.Name = "butImageAdd";
			this.butImageAdd.Size = new System.Drawing.Size(82, 24);
			this.butImageAdd.TabIndex = 504;
			this.butImageAdd.Text = "Add";
			this.butImageAdd.UseVisualStyleBackColor = true;
			this.butImageAdd.Click += new System.EventHandler(this.butImageAdd_Click);
			// 
			// clinicPickerEClipboard
			// 
			this.clinicPickerEClipboard.HqDescription = "Default";
			this.clinicPickerEClipboard.IncludeUnassigned = true;
			this.clinicPickerEClipboard.Location = new System.Drawing.Point(449, 17);
			this.clinicPickerEClipboard.Name = "clinicPickerEClipboard";
			this.clinicPickerEClipboard.Size = new System.Drawing.Size(200, 21);
			this.clinicPickerEClipboard.TabIndex = 272;
			// 
			// labelEClipboardNotSignedUp
			// 
			this.labelEClipboardNotSignedUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelEClipboardNotSignedUp.Location = new System.Drawing.Point(8, 11);
			this.labelEClipboardNotSignedUp.Name = "labelEClipboardNotSignedUp";
			this.labelEClipboardNotSignedUp.Size = new System.Drawing.Size(321, 19);
			this.labelEClipboardNotSignedUp.TabIndex = 273;
			this.labelEClipboardNotSignedUp.Text = "Go to the Signup Portal to enable this feature.";
			this.labelEClipboardNotSignedUp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butBrandingProfile
			// 
			this.butBrandingProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butBrandingProfile.Location = new System.Drawing.Point(11, 661);
			this.butBrandingProfile.Name = "butBrandingProfile";
			this.butBrandingProfile.Size = new System.Drawing.Size(102, 24);
			this.butBrandingProfile.TabIndex = 503;
			this.butBrandingProfile.Text = "Branding Profile";
			this.butBrandingProfile.UseVisualStyleBackColor = true;
			this.butBrandingProfile.Click += new System.EventHandler(this.butBrandingProfile_Click);
			// 
			// butEFormAdd
			// 
			this.butEFormAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEFormAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butEFormAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEFormAdd.Location = new System.Drawing.Point(598, 619);
			this.butEFormAdd.Name = "butEFormAdd";
			this.butEFormAdd.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.butEFormAdd.Size = new System.Drawing.Size(92, 24);
			this.butEFormAdd.TabIndex = 511;
			this.butEFormAdd.Text = "Add eForm";
			this.butEFormAdd.UseVisualStyleBackColor = true;
			this.butEFormAdd.Click += new System.EventHandler(this.butEFormAdd_Click);
			// 
			// butSheetAdd
			// 
			this.butSheetAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSheetAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butSheetAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSheetAdd.Location = new System.Drawing.Point(496, 619);
			this.butSheetAdd.Name = "butSheetAdd";
			this.butSheetAdd.Size = new System.Drawing.Size(92, 24);
			this.butSheetAdd.TabIndex = 510;
			this.butSheetAdd.Text = "Add Sheet";
			this.butSheetAdd.UseVisualStyleBackColor = true;
			this.butSheetAdd.Click += new System.EventHandler(this.butSheetAdd_Click);
			// 
			// butEClipboardUp
			// 
			this.butEClipboardUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butEClipboardUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEClipboardUp.Image = global::OpenDental.Properties.Resources.up;
			this.butEClipboardUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEClipboardUp.Location = new System.Drawing.Point(700, 619);
			this.butEClipboardUp.Name = "butEClipboardUp";
			this.butEClipboardUp.Size = new System.Drawing.Size(69, 24);
			this.butEClipboardUp.TabIndex = 508;
			this.butEClipboardUp.Text = "Up";
			this.butEClipboardUp.Click += new System.EventHandler(this.ButEClipboardUp_Click);
			// 
			// butEClipboardDown
			// 
			this.butEClipboardDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEClipboardDown.Image = global::OpenDental.Properties.Resources.down;
			this.butEClipboardDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEClipboardDown.Location = new System.Drawing.Point(779, 619);
			this.butEClipboardDown.Name = "butEClipboardDown";
			this.butEClipboardDown.Size = new System.Drawing.Size(69, 24);
			this.butEClipboardDown.TabIndex = 509;
			this.butEClipboardDown.Text = "Down";
			this.butEClipboardDown.Click += new System.EventHandler(this.ButEClipboardDown_Click);
			// 
			// gridForms
			// 
			this.gridForms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridForms.HasAutoWrappedHeaders = true;
			this.gridForms.HasMultilineHeaders = true;
			this.gridForms.Location = new System.Drawing.Point(496, 266);
			this.gridForms.Name = "gridForms";
			this.gridForms.Size = new System.Drawing.Size(638, 347);
			this.gridForms.TabIndex = 507;
			this.gridForms.Title = "Forms";
			this.gridForms.TranslationName = "TableAvailableRows";
			this.gridForms.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridForms_CellDoubleClick);
			// 
			// gridImages
			// 
			this.gridImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridImages.HasAutoWrappedHeaders = true;
			this.gridImages.HasMultilineHeaders = true;
			this.gridImages.Location = new System.Drawing.Point(11, 266);
			this.gridImages.Name = "gridImages";
			this.gridImages.Size = new System.Drawing.Size(445, 284);
			this.gridImages.TabIndex = 512;
			this.gridImages.Title = "Images";
			this.gridImages.TranslationName = "TableAvailableRows";
			this.gridImages.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridImages_CellDoubleClick);
			// 
			// FormEServicesEClipboard
			// 
			this.ClientSize = new System.Drawing.Size(1192, 696);
			this.Controls.Add(this.gridImages);
			this.Controls.Add(this.butImageAdd);
			this.Controls.Add(this.butEFormAdd);
			this.Controls.Add(this.butSheetAdd);
			this.Controls.Add(this.butEClipboardUp);
			this.Controls.Add(this.butEClipboardDown);
			this.Controls.Add(this.gridForms);
			this.Controls.Add(this.butBrandingProfile);
			this.Controls.Add(this.labelEClipboardNotSignedUp);
			this.Controls.Add(this.clinicPickerEClipboard);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.groupEClipboardRules);
			this.Controls.Add(this.checkEClipboardUseDefaults);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesEClipboard";
			this.Text = "eServices eClipboard";
			this.Load += new System.EventHandler(this.FormEServicesEClipboard_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.groupEClipboardRules.ResumeLayout(false);
			this.groupEClipboardRules.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butSave;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.ContextMenuStrip menuWebSchedVerifyTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem insertReplacementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private OpenDental.UI.CheckBox checkEClipboardUseDefaults;
		private OpenDental.UI.GroupBox groupEClipboardRules;
		private OpenDental.UI.CheckBox checkEClipboardCreateMissingForms;
		private System.Windows.Forms.Label labelEClipboardMessage;
		private System.Windows.Forms.TextBox textEClipboardMessage;
		private OpenDental.UI.CheckBox checkEClipboardAllowSheets;
		private OpenDental.UI.CheckBox checkEClipboardAllowCheckIn;
		private OpenDental.UI.CheckBox checkEClipboardPopupKiosk;
		private UI.ComboBoxClinicPicker clinicPickerEClipboard;
		private System.Windows.Forms.Label labelEClipboardNotSignedUp;
		private OpenDental.UI.CheckBox checkEnableByodSms;
		private OpenDental.UI.CheckBox checkAppendByodToArrivalResponseSms;
		private OpenDental.UI.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textByodSmsTemplate;
		private OpenDental.UI.GroupBox groupBox1;
		private UI.Button butConfStatuses;
		private OpenDental.UI.CheckBox checkRequire2FA;
		private OpenDental.UI.CheckBox checkDisplayIndividually;
		private OpenDental.UI.CheckBox checkEClipboardAllowPaymentCheckIn;
		private UI.Button butBrandingProfile;
		private UI.Button butImageAdd;
		private UI.Button butEFormAdd;
		private UI.Button butSheetAdd;
		private UI.Button butEClipboardUp;
		private UI.Button butEClipboardDown;
		private UI.GridOD gridForms;
		private UI.GridOD gridImages;
	}
}