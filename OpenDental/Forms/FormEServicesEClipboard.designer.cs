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
			this.butOK = new OpenDental.UI.Button();
			this.checkEClipboardUseDefaults = new OpenDental.UI.CheckBox();
			this.groupEClipboardRules = new OpenDental.UI.GroupBox();
			this.checkEClipboardAllowPaymentCheckIn = new OpenDental.UI.CheckBox();
			this.groupBoxImage = new OpenDental.UI.GroupBox();
			this.butImageOptions = new OpenDental.UI.Button();
			this.textEclipboardImageDefs = new System.Windows.Forms.TextBox();
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
			this.groupEClipboardSheets = new OpenDental.UI.GroupBox();
			this.butEClipboardAddSheets = new OpenDental.UI.Button();
			this.butEClipboardUp = new OpenDental.UI.Button();
			this.butEClipboardDown = new OpenDental.UI.Button();
			this.butEClipboardRight = new OpenDental.UI.Button();
			this.butEClipboardLeft = new OpenDental.UI.Button();
			this.gridEClipboardSheetsInUse = new OpenDental.UI.GridOD();
			this.listEClipboardSheetsAvailable = new OpenDental.UI.ListBox();
			this.labelOps = new System.Windows.Forms.Label();
			this.clinicPickerEClipboard = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelEClipboardNotSignedUp = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butBrandingProfile = new OpenDental.UI.Button();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			this.groupEClipboardRules.SuspendLayout();
			this.groupBoxImage.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupEClipboardSheets.SuspendLayout();
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
			this.butOK.Location = new System.Drawing.Point(1024, 572);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 500;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
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
			this.groupEClipboardRules.Controls.Add(this.groupBoxImage);
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
			this.groupEClipboardRules.Size = new System.Drawing.Size(1169, 234);
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
			// groupBoxImage
			// 
			this.groupBoxImage.Controls.Add(this.butImageOptions);
			this.groupBoxImage.Controls.Add(this.textEclipboardImageDefs);
			this.groupBoxImage.Location = new System.Drawing.Point(541, 111);
			this.groupBoxImage.Name = "groupBoxImage";
			this.groupBoxImage.Size = new System.Drawing.Size(461, 73);
			this.groupBoxImage.TabIndex = 503;
			this.groupBoxImage.Text = "Prompt for Image Capture";
			// 
			// butImageOptions
			// 
			this.butImageOptions.Location = new System.Drawing.Point(398, 26);
			this.butImageOptions.Name = "butImageOptions";
			this.butImageOptions.Size = new System.Drawing.Size(53, 24);
			this.butImageOptions.TabIndex = 1;
			this.butImageOptions.Text = "Edit";
			this.butImageOptions.UseVisualStyleBackColor = true;
			this.butImageOptions.Click += new System.EventHandler(this.butImageOptions_Click);
			// 
			// textEclipboardImageDefs
			// 
			this.textEclipboardImageDefs.Location = new System.Drawing.Point(6, 26);
			this.textEclipboardImageDefs.Multiline = true;
			this.textEclipboardImageDefs.Name = "textEclipboardImageDefs";
			this.textEclipboardImageDefs.ReadOnly = true;
			this.textEclipboardImageDefs.Size = new System.Drawing.Size(386, 34);
			this.textEclipboardImageDefs.TabIndex = 0;
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
			this.labelEClipboardMessage.Location = new System.Drawing.Point(16, 142);
			this.labelEClipboardMessage.Name = "labelEClipboardMessage";
			this.labelEClipboardMessage.Size = new System.Drawing.Size(461, 18);
			this.labelEClipboardMessage.TabIndex = 266;
			this.labelEClipboardMessage.Text = "Message to show patients after successful check-in";
			this.labelEClipboardMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textEClipboardMessage
			// 
			this.textEClipboardMessage.Location = new System.Drawing.Point(19, 163);
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
			// groupEClipboardSheets
			// 
			this.groupEClipboardSheets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupEClipboardSheets.Controls.Add(this.butEClipboardAddSheets);
			this.groupEClipboardSheets.Controls.Add(this.butEClipboardUp);
			this.groupEClipboardSheets.Controls.Add(this.butEClipboardDown);
			this.groupEClipboardSheets.Controls.Add(this.butEClipboardRight);
			this.groupEClipboardSheets.Controls.Add(this.butEClipboardLeft);
			this.groupEClipboardSheets.Controls.Add(this.gridEClipboardSheetsInUse);
			this.groupEClipboardSheets.Controls.Add(this.listEClipboardSheetsAvailable);
			this.groupEClipboardSheets.Controls.Add(this.labelOps);
			this.groupEClipboardSheets.Location = new System.Drawing.Point(11, 293);
			this.groupEClipboardSheets.Name = "groupEClipboardSheets";
			this.groupEClipboardSheets.Size = new System.Drawing.Size(1169, 273);
			this.groupEClipboardSheets.TabIndex = 266;
			this.groupEClipboardSheets.Text = "Specify which forms are added upon patient arrival";
			// 
			// butEClipboardAddSheets
			// 
			this.butEClipboardAddSheets.Location = new System.Drawing.Point(12, 239);
			this.butEClipboardAddSheets.Name = "butEClipboardAddSheets";
			this.butEClipboardAddSheets.Size = new System.Drawing.Size(117, 23);
			this.butEClipboardAddSheets.TabIndex = 503;
			this.butEClipboardAddSheets.Text = "Add Custom Sheets";
			this.butEClipboardAddSheets.UseVisualStyleBackColor = true;
			this.butEClipboardAddSheets.Click += new System.EventHandler(this.ButEClipboardAddSheets_Click);
			// 
			// butEClipboardUp
			// 
			this.butEClipboardUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butEClipboardUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEClipboardUp.Image = global::OpenDental.Properties.Resources.up;
			this.butEClipboardUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEClipboardUp.Location = new System.Drawing.Point(1130, 177);
			this.butEClipboardUp.Name = "butEClipboardUp";
			this.butEClipboardUp.Size = new System.Drawing.Size(33, 26);
			this.butEClipboardUp.TabIndex = 267;
			this.butEClipboardUp.Click += new System.EventHandler(this.ButEClipboardUp_Click);
			// 
			// butEClipboardDown
			// 
			this.butEClipboardDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEClipboardDown.Image = global::OpenDental.Properties.Resources.down;
			this.butEClipboardDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEClipboardDown.Location = new System.Drawing.Point(1130, 209);
			this.butEClipboardDown.Name = "butEClipboardDown";
			this.butEClipboardDown.Size = new System.Drawing.Size(33, 26);
			this.butEClipboardDown.TabIndex = 268;
			this.butEClipboardDown.Click += new System.EventHandler(this.ButEClipboardDown_Click);
			// 
			// butEClipboardRight
			// 
			this.butEClipboardRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butEClipboardRight.Location = new System.Drawing.Point(490, 111);
			this.butEClipboardRight.Name = "butEClipboardRight";
			this.butEClipboardRight.Size = new System.Drawing.Size(35, 26);
			this.butEClipboardRight.TabIndex = 64;
			this.butEClipboardRight.Click += new System.EventHandler(this.ButEClipboardRight_Click);
			// 
			// butEClipboardLeft
			// 
			this.butEClipboardLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butEClipboardLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butEClipboardLeft.Location = new System.Drawing.Point(490, 143);
			this.butEClipboardLeft.Name = "butEClipboardLeft";
			this.butEClipboardLeft.Size = new System.Drawing.Size(35, 26);
			this.butEClipboardLeft.TabIndex = 63;
			this.butEClipboardLeft.Click += new System.EventHandler(this.ButEClipboardLeft_Click);
			// 
			// gridEClipboardSheetsInUse
			// 
			this.gridEClipboardSheetsInUse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEClipboardSheetsInUse.HasAutoWrappedHeaders = true;
			this.gridEClipboardSheetsInUse.HasMultilineHeaders = true;
			this.gridEClipboardSheetsInUse.Location = new System.Drawing.Point(540, 36);
			this.gridEClipboardSheetsInUse.Name = "gridEClipboardSheetsInUse";
			this.gridEClipboardSheetsInUse.Size = new System.Drawing.Size(582, 199);
			this.gridEClipboardSheetsInUse.TabIndex = 62;
			this.gridEClipboardSheetsInUse.Title = "Sheets In Use";
			this.gridEClipboardSheetsInUse.TranslationName = "TableAvailableRows";
			this.gridEClipboardSheetsInUse.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.GridEClipboardSheetsInUse_CellDoubleClick);
			// 
			// listEClipboardSheetsAvailable
			// 
			this.listEClipboardSheetsAvailable.Location = new System.Drawing.Point(8, 36);
			this.listEClipboardSheetsAvailable.Name = "listEClipboardSheetsAvailable";
			this.listEClipboardSheetsAvailable.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listEClipboardSheetsAvailable.Size = new System.Drawing.Size(466, 199);
			this.listEClipboardSheetsAvailable.TabIndex = 42;
			// 
			// labelOps
			// 
			this.labelOps.Location = new System.Drawing.Point(5, 14);
			this.labelOps.Name = "labelOps";
			this.labelOps.Size = new System.Drawing.Size(214, 18);
			this.labelOps.TabIndex = 41;
			this.labelOps.Text = "Available Forms (Custom Sheets Only)";
			this.labelOps.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
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
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1105, 571);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 501;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butBrandingProfile
			// 
			this.butBrandingProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butBrandingProfile.Location = new System.Drawing.Point(11, 572);
			this.butBrandingProfile.Name = "butBrandingProfile";
			this.butBrandingProfile.Size = new System.Drawing.Size(102, 24);
			this.butBrandingProfile.TabIndex = 503;
			this.butBrandingProfile.Text = "Branding Profile";
			this.butBrandingProfile.UseVisualStyleBackColor = true;
			this.butBrandingProfile.Click += new System.EventHandler(this.butBrandingProfile_Click);
			// 
			// FormEServicesEClipboard
			// 
			this.ClientSize = new System.Drawing.Size(1192, 607);
			this.Controls.Add(this.butBrandingProfile);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelEClipboardNotSignedUp);
			this.Controls.Add(this.clinicPickerEClipboard);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupEClipboardSheets);
			this.Controls.Add(this.groupEClipboardRules);
			this.Controls.Add(this.checkEClipboardUseDefaults);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesEClipboard";
			this.Text = "eServices eClipboard";
			this.Load += new System.EventHandler(this.FormEServicesEClipboard_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.groupEClipboardRules.ResumeLayout(false);
			this.groupEClipboardRules.PerformLayout();
			this.groupBoxImage.ResumeLayout(false);
			this.groupBoxImage.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupEClipboardSheets.ResumeLayout(false);
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
		private OpenDental.UI.CheckBox checkEClipboardUseDefaults;
		private OpenDental.UI.GroupBox groupEClipboardRules;
		private OpenDental.UI.CheckBox checkEClipboardCreateMissingForms;
		private System.Windows.Forms.Label labelEClipboardMessage;
		private System.Windows.Forms.TextBox textEClipboardMessage;
		private OpenDental.UI.CheckBox checkEClipboardAllowSheets;
		private OpenDental.UI.CheckBox checkEClipboardAllowCheckIn;
		private OpenDental.UI.CheckBox checkEClipboardPopupKiosk;
		private OpenDental.UI.GroupBox groupEClipboardSheets;
		private UI.Button butEClipboardAddSheets;
		private UI.Button butEClipboardUp;
		private UI.Button butEClipboardDown;
		private UI.Button butEClipboardRight;
		private UI.Button butEClipboardLeft;
		private UI.GridOD gridEClipboardSheetsInUse;
		private UI.ListBox listEClipboardSheetsAvailable;
		private System.Windows.Forms.Label labelOps;
		private UI.ComboBoxClinicPicker clinicPickerEClipboard;
		private System.Windows.Forms.Label labelEClipboardNotSignedUp;
		private UI.Button butCancel;
		private OpenDental.UI.CheckBox checkEnableByodSms;
		private OpenDental.UI.CheckBox checkAppendByodToArrivalResponseSms;
		private OpenDental.UI.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textByodSmsTemplate;
		private OpenDental.UI.GroupBox groupBox1;
		private UI.Button butConfStatuses;
		private OpenDental.UI.CheckBox checkRequire2FA;
		private OpenDental.UI.GroupBox groupBoxImage;
		private UI.Button butImageOptions;
		private System.Windows.Forms.TextBox textEclipboardImageDefs;
		private OpenDental.UI.CheckBox checkDisplayIndividually;
		private OpenDental.UI.CheckBox checkEClipboardAllowPaymentCheckIn;
		private UI.Button butBrandingProfile;
	}
}