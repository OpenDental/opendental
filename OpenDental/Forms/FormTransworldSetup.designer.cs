namespace OpenDental{
	partial class FormTransworldSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTransworldSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.textSftpPassword = new System.Windows.Forms.TextBox();
			this.textSftpUsername = new System.Windows.Forms.TextBox();
			this.labelSftpPassword = new System.Windows.Forms.Label();
			this.labelSftpUsername = new System.Windows.Forms.Label();
			this.textSftpAddress = new System.Windows.Forms.TextBox();
			this.labelSftpAddress = new System.Windows.Forms.Label();
			this.textClientIdAccelerator = new System.Windows.Forms.TextBox();
			this.labelClientIdAccelerator = new System.Windows.Forms.Label();
			this.labelClinic = new System.Windows.Forms.Label();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.groupClinicSettings = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboNegAdjType = new OpenDental.UI.ComboBoxOD();
			this.comboPosAdjType = new OpenDental.UI.ComboBoxOD();
			this.checkThankYouLetter = new System.Windows.Forms.CheckBox();
			this.groupServicesEnabled = new System.Windows.Forms.GroupBox();
			this.checkAccelService = new System.Windows.Forms.CheckBox();
			this.checkPRService = new System.Windows.Forms.CheckBox();
			this.checkCollService = new System.Windows.Forms.CheckBox();
			this.groupSftpServerInfo = new System.Windows.Forms.GroupBox();
			this.labelSftpPort = new System.Windows.Forms.Label();
			this.textSftpPort = new OpenDental.ValidNum();
			this.groupClientIDs = new System.Windows.Forms.GroupBox();
			this.textClientIdCollection = new System.Windows.Forms.TextBox();
			this.labelClientIdCollection = new System.Windows.Forms.Label();
			this.labelClinicEnable = new System.Windows.Forms.Label();
			this.checkHideButtons = new System.Windows.Forms.CheckBox();
			this.labelSendTimeOfDay = new System.Windows.Forms.Label();
			this.textUpdatesTimeOfDay = new System.Windows.Forms.TextBox();
			this.labelODService = new System.Windows.Forms.Label();
			this.groupSendActivity = new System.Windows.Forms.GroupBox();
			this.labelPaidInFullBillType = new System.Windows.Forms.Label();
			this.comboPaidInFullBillType = new OpenDental.UI.ComboBoxOD();
			this.numericSendFrequency = new System.Windows.Forms.NumericUpDown();
			this.comboSendFrequencyUnits = new System.Windows.Forms.ComboBox();
			this.labelSendFrequency = new System.Windows.Forms.Label();
			this.groupClinicSettings.SuspendLayout();
			this.groupServicesEnabled.SuspendLayout();
			this.groupSftpServerInfo.SuspendLayout();
			this.groupClientIDs.SuspendLayout();
			this.groupSendActivity.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericSendFrequency)).BeginInit();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(255, 502);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(337, 502);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnabled.Location = new System.Drawing.Point(24, 13);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(161, 18);
			this.checkEnabled.TabIndex = 1;
			this.checkEnabled.Text = "Enabled (affects all clinics)";
			this.checkEnabled.Click += new System.EventHandler(this.checkEnabled_Click);
			// 
			// textSftpPassword
			// 
			this.textSftpPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSftpPassword.Location = new System.Drawing.Point(162, 76);
			this.textSftpPassword.Name = "textSftpPassword";
			this.textSftpPassword.PasswordChar = '*';
			this.textSftpPassword.Size = new System.Drawing.Size(220, 20);
			this.textSftpPassword.TabIndex = 5;
			// 
			// textSftpUsername
			// 
			this.textSftpUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSftpUsername.Location = new System.Drawing.Point(162, 56);
			this.textSftpUsername.Name = "textSftpUsername";
			this.textSftpUsername.Size = new System.Drawing.Size(220, 20);
			this.textSftpUsername.TabIndex = 4;
			// 
			// labelSftpPassword
			// 
			this.labelSftpPassword.Location = new System.Drawing.Point(6, 77);
			this.labelSftpPassword.Name = "labelSftpPassword";
			this.labelSftpPassword.Size = new System.Drawing.Size(155, 18);
			this.labelSftpPassword.TabIndex = 0;
			this.labelSftpPassword.Text = "Password";
			this.labelSftpPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSftpUsername
			// 
			this.labelSftpUsername.Location = new System.Drawing.Point(6, 57);
			this.labelSftpUsername.Name = "labelSftpUsername";
			this.labelSftpUsername.Size = new System.Drawing.Size(155, 18);
			this.labelSftpUsername.TabIndex = 0;
			this.labelSftpUsername.Text = "Username";
			this.labelSftpUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSftpAddress
			// 
			this.textSftpAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSftpAddress.Location = new System.Drawing.Point(162, 16);
			this.textSftpAddress.Name = "textSftpAddress";
			this.textSftpAddress.Size = new System.Drawing.Size(220, 20);
			this.textSftpAddress.TabIndex = 2;
			// 
			// labelSftpAddress
			// 
			this.labelSftpAddress.Location = new System.Drawing.Point(6, 17);
			this.labelSftpAddress.Name = "labelSftpAddress";
			this.labelSftpAddress.Size = new System.Drawing.Size(155, 18);
			this.labelSftpAddress.TabIndex = 0;
			this.labelSftpAddress.Text = "Address";
			this.labelSftpAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClientIdAccelerator
			// 
			this.textClientIdAccelerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textClientIdAccelerator.Location = new System.Drawing.Point(162, 16);
			this.textClientIdAccelerator.Name = "textClientIdAccelerator";
			this.textClientIdAccelerator.Size = new System.Drawing.Size(220, 20);
			this.textClientIdAccelerator.TabIndex = 0;
			// 
			// labelClientIdAccelerator
			// 
			this.labelClientIdAccelerator.Location = new System.Drawing.Point(6, 17);
			this.labelClientIdAccelerator.Name = "labelClientIdAccelerator";
			this.labelClientIdAccelerator.Size = new System.Drawing.Size(155, 18);
			this.labelClientIdAccelerator.TabIndex = 0;
			this.labelClientIdAccelerator.Text = "Accelerator";
			this.labelClientIdAccelerator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(12, 77);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(167, 18);
			this.labelClinic.TabIndex = 0;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.FormattingEnabled = true;
			this.comboClinic.Location = new System.Drawing.Point(180, 76);
			this.comboClinic.MaxDropDownItems = 40;
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(220, 21);
			this.comboClinic.TabIndex = 2;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// groupClinicSettings
			// 
			this.groupClinicSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupClinicSettings.Controls.Add(this.label2);
			this.groupClinicSettings.Controls.Add(this.label1);
			this.groupClinicSettings.Controls.Add(this.comboNegAdjType);
			this.groupClinicSettings.Controls.Add(this.comboPosAdjType);
			this.groupClinicSettings.Controls.Add(this.checkThankYouLetter);
			this.groupClinicSettings.Controls.Add(this.groupServicesEnabled);
			this.groupClinicSettings.Controls.Add(this.groupSftpServerInfo);
			this.groupClinicSettings.Controls.Add(this.groupClientIDs);
			this.groupClinicSettings.Location = new System.Drawing.Point(12, 103);
			this.groupClinicSettings.Name = "groupClinicSettings";
			this.groupClinicSettings.Size = new System.Drawing.Size(400, 307);
			this.groupClinicSettings.TabIndex = 3;
			this.groupClinicSettings.TabStop = false;
			this.groupClinicSettings.Text = "Clinic Settings";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 282);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(161, 18);
			this.label2.TabIndex = 241;
			this.label2.Text = "Exclude Negative Adj Type";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 261);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(161, 18);
			this.label1.TabIndex = 240;
			this.label1.Text = "Exclude Positive Adj Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboNegAdjType
			// 
			this.comboNegAdjType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboNegAdjType.Location = new System.Drawing.Point(168, 281);
			this.comboNegAdjType.Name = "comboNegAdjType";
			this.comboNegAdjType.Size = new System.Drawing.Size(220, 21);
			this.comboNegAdjType.TabIndex = 239;
			// 
			// comboPosAdjType
			// 
			this.comboPosAdjType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPosAdjType.Location = new System.Drawing.Point(168, 260);
			this.comboPosAdjType.Name = "comboPosAdjType";
			this.comboPosAdjType.Size = new System.Drawing.Size(220, 21);
			this.comboPosAdjType.TabIndex = 238;
			// 
			// checkThankYouLetter
			// 
			this.checkThankYouLetter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkThankYouLetter.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkThankYouLetter.Location = new System.Drawing.Point(6, 240);
			this.checkThankYouLetter.Name = "checkThankYouLetter";
			this.checkThankYouLetter.Size = new System.Drawing.Size(175, 18);
			this.checkThankYouLetter.TabIndex = 6;
			this.checkThankYouLetter.Text = "\"Paid in Full\" thank you letter";
			this.checkThankYouLetter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupServicesEnabled
			// 
			this.groupServicesEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupServicesEnabled.Controls.Add(this.checkAccelService);
			this.groupServicesEnabled.Controls.Add(this.checkPRService);
			this.groupServicesEnabled.Controls.Add(this.checkCollService);
			this.groupServicesEnabled.Location = new System.Drawing.Point(6, 18);
			this.groupServicesEnabled.Name = "groupServicesEnabled";
			this.groupServicesEnabled.Size = new System.Drawing.Size(388, 40);
			this.groupServicesEnabled.TabIndex = 8;
			this.groupServicesEnabled.TabStop = false;
			this.groupServicesEnabled.Text = "Services Enabled";
			// 
			// checkAccelService
			// 
			this.checkAccelService.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAccelService.Checked = true;
			this.checkAccelService.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAccelService.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAccelService.Location = new System.Drawing.Point(6, 16);
			this.checkAccelService.Name = "checkAccelService";
			this.checkAccelService.Size = new System.Drawing.Size(87, 18);
			this.checkAccelService.TabIndex = 9;
			this.checkAccelService.Text = "Accelerator";
			this.checkAccelService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAccelService.CheckedChanged += new System.EventHandler(this.checkAccelService_CheckedChanged);
			// 
			// checkPRService
			// 
			this.checkPRService.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPRService.Checked = true;
			this.checkPRService.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkPRService.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPRService.Location = new System.Drawing.Point(99, 16);
			this.checkPRService.Name = "checkPRService";
			this.checkPRService.Size = new System.Drawing.Size(111, 18);
			this.checkPRService.TabIndex = 8;
			this.checkPRService.Text = "Profit Recovery";
			this.checkPRService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPRService.CheckedChanged += new System.EventHandler(this.checkPRService_CheckedChanged);
			// 
			// checkCollService
			// 
			this.checkCollService.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCollService.Checked = true;
			this.checkCollService.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkCollService.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCollService.Location = new System.Drawing.Point(216, 16);
			this.checkCollService.Name = "checkCollService";
			this.checkCollService.Size = new System.Drawing.Size(149, 18);
			this.checkCollService.TabIndex = 7;
			this.checkCollService.Text = "Professional Collections";
			this.checkCollService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCollService.CheckedChanged += new System.EventHandler(this.checkCollService_CheckedChanged);
			// 
			// groupSftpServerInfo
			// 
			this.groupSftpServerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupSftpServerInfo.Controls.Add(this.labelSftpPort);
			this.groupSftpServerInfo.Controls.Add(this.textSftpAddress);
			this.groupSftpServerInfo.Controls.Add(this.textSftpPort);
			this.groupSftpServerInfo.Controls.Add(this.labelSftpAddress);
			this.groupSftpServerInfo.Controls.Add(this.textSftpPassword);
			this.groupSftpServerInfo.Controls.Add(this.textSftpUsername);
			this.groupSftpServerInfo.Controls.Add(this.labelSftpPassword);
			this.groupSftpServerInfo.Controls.Add(this.labelSftpUsername);
			this.groupSftpServerInfo.Location = new System.Drawing.Point(6, 64);
			this.groupSftpServerInfo.Name = "groupSftpServerInfo";
			this.groupSftpServerInfo.Size = new System.Drawing.Size(388, 102);
			this.groupSftpServerInfo.TabIndex = 6;
			this.groupSftpServerInfo.TabStop = false;
			this.groupSftpServerInfo.Text = "SFTP Server Details";
			// 
			// labelSftpPort
			// 
			this.labelSftpPort.Location = new System.Drawing.Point(6, 37);
			this.labelSftpPort.Name = "labelSftpPort";
			this.labelSftpPort.Size = new System.Drawing.Size(155, 18);
			this.labelSftpPort.TabIndex = 0;
			this.labelSftpPort.Text = "Port";
			this.labelSftpPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSftpPort
			// 
			this.textSftpPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSftpPort.Location = new System.Drawing.Point(162, 36);
			this.textSftpPort.MaxLength = 5;
			this.textSftpPort.MaxVal = 65535;
			this.textSftpPort.Name = "textSftpPort";
			this.textSftpPort.ShowZero = false;
			this.textSftpPort.Size = new System.Drawing.Size(108, 20);
			this.textSftpPort.TabIndex = 3;
			// 
			// groupClientIDs
			// 
			this.groupClientIDs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupClientIDs.Controls.Add(this.textClientIdAccelerator);
			this.groupClientIDs.Controls.Add(this.textClientIdCollection);
			this.groupClientIDs.Controls.Add(this.labelClientIdAccelerator);
			this.groupClientIDs.Controls.Add(this.labelClientIdCollection);
			this.groupClientIDs.Location = new System.Drawing.Point(6, 172);
			this.groupClientIDs.Name = "groupClientIDs";
			this.groupClientIDs.Size = new System.Drawing.Size(388, 62);
			this.groupClientIDs.TabIndex = 7;
			this.groupClientIDs.TabStop = false;
			this.groupClientIDs.Text = "Client IDs";
			// 
			// textClientIdCollection
			// 
			this.textClientIdCollection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textClientIdCollection.Location = new System.Drawing.Point(162, 36);
			this.textClientIdCollection.Name = "textClientIdCollection";
			this.textClientIdCollection.Size = new System.Drawing.Size(220, 20);
			this.textClientIdCollection.TabIndex = 1;
			// 
			// labelClientIdCollection
			// 
			this.labelClientIdCollection.Location = new System.Drawing.Point(6, 37);
			this.labelClientIdCollection.Name = "labelClientIdCollection";
			this.labelClientIdCollection.Size = new System.Drawing.Size(155, 18);
			this.labelClientIdCollection.TabIndex = 0;
			this.labelClientIdCollection.Text = "Profit Recovery/Collection";
			this.labelClientIdCollection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClinicEnable
			// 
			this.labelClinicEnable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClinicEnable.Location = new System.Drawing.Point(12, 37);
			this.labelClinicEnable.Name = "labelClinicEnable";
			this.labelClinicEnable.Size = new System.Drawing.Size(400, 30);
			this.labelClinicEnable.TabIndex = 0;
			this.labelClinicEnable.Text = "To enable Transworld Systems for a clinic, set the ID(s),\r\nSFTP address and port," +
    " and login credentials for that clinic.";
			this.labelClinicEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// checkHideButtons
			// 
			this.checkHideButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkHideButtons.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideButtons.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideButtons.Location = new System.Drawing.Point(255, 13);
			this.checkHideButtons.Name = "checkHideButtons";
			this.checkHideButtons.Size = new System.Drawing.Size(145, 18);
			this.checkHideButtons.TabIndex = 75;
			this.checkHideButtons.Text = "Hide Unused Button";
			this.checkHideButtons.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSendTimeOfDay
			// 
			this.labelSendTimeOfDay.Location = new System.Drawing.Point(6, 17);
			this.labelSendTimeOfDay.Name = "labelSendTimeOfDay";
			this.labelSendTimeOfDay.Size = new System.Drawing.Size(161, 18);
			this.labelSendTimeOfDay.TabIndex = 233;
			this.labelSendTimeOfDay.Text = "OpenDentalService Send Time";
			this.labelSendTimeOfDay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUpdatesTimeOfDay
			// 
			this.textUpdatesTimeOfDay.Location = new System.Drawing.Point(168, 16);
			this.textUpdatesTimeOfDay.Name = "textUpdatesTimeOfDay";
			this.textUpdatesTimeOfDay.Size = new System.Drawing.Size(58, 20);
			this.textUpdatesTimeOfDay.TabIndex = 232;
			this.textUpdatesTimeOfDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelODService
			// 
			this.labelODService.Location = new System.Drawing.Point(227, 17);
			this.labelODService.Name = "labelODService";
			this.labelODService.Size = new System.Drawing.Size(122, 18);
			this.labelODService.TabIndex = 2;
			this.labelODService.Text = "(by OpenDentalService)";
			this.labelODService.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupSendActivity
			// 
			this.groupSendActivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupSendActivity.Controls.Add(this.labelPaidInFullBillType);
			this.groupSendActivity.Controls.Add(this.comboPaidInFullBillType);
			this.groupSendActivity.Controls.Add(this.numericSendFrequency);
			this.groupSendActivity.Controls.Add(this.comboSendFrequencyUnits);
			this.groupSendActivity.Controls.Add(this.labelSendFrequency);
			this.groupSendActivity.Controls.Add(this.labelSendTimeOfDay);
			this.groupSendActivity.Controls.Add(this.labelODService);
			this.groupSendActivity.Controls.Add(this.textUpdatesTimeOfDay);
			this.groupSendActivity.Location = new System.Drawing.Point(12, 411);
			this.groupSendActivity.Name = "groupSendActivity";
			this.groupSendActivity.Size = new System.Drawing.Size(400, 83);
			this.groupSendActivity.TabIndex = 234;
			this.groupSendActivity.TabStop = false;
			this.groupSendActivity.Text = "Account Activity Updates (affects all clinics)";
			// 
			// labelPaidInFullBillType
			// 
			this.labelPaidInFullBillType.Location = new System.Drawing.Point(6, 57);
			this.labelPaidInFullBillType.Name = "labelPaidInFullBillType";
			this.labelPaidInFullBillType.Size = new System.Drawing.Size(161, 18);
			this.labelPaidInFullBillType.TabIndex = 238;
			this.labelPaidInFullBillType.Text = "Paid in Full Billing Type";
			this.labelPaidInFullBillType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaidInFullBillType
			// 
			this.comboPaidInFullBillType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPaidInFullBillType.Location = new System.Drawing.Point(168, 56);
			this.comboPaidInFullBillType.Name = "comboPaidInFullBillType";
			this.comboPaidInFullBillType.Size = new System.Drawing.Size(220, 21);
			this.comboPaidInFullBillType.TabIndex = 237;
			// 
			// numericSendFrequency
			// 
			this.numericSendFrequency.Location = new System.Drawing.Point(168, 36);
			this.numericSendFrequency.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
			this.numericSendFrequency.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericSendFrequency.Name = "numericSendFrequency";
			this.numericSendFrequency.Size = new System.Drawing.Size(58, 20);
			this.numericSendFrequency.TabIndex = 236;
			this.numericSendFrequency.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// comboSendFrequencyUnits
			// 
			this.comboSendFrequencyUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboSendFrequencyUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSendFrequencyUnits.FormattingEnabled = true;
			this.comboSendFrequencyUnits.Location = new System.Drawing.Point(232, 35);
			this.comboSendFrequencyUnits.Name = "comboSendFrequencyUnits";
			this.comboSendFrequencyUnits.Size = new System.Drawing.Size(156, 21);
			this.comboSendFrequencyUnits.TabIndex = 235;
			this.comboSendFrequencyUnits.SelectedIndexChanged += new System.EventHandler(this.comboSendFrequencyUnits_SelectedIndexChanged);
			// 
			// labelSendFrequency
			// 
			this.labelSendFrequency.Location = new System.Drawing.Point(6, 37);
			this.labelSendFrequency.Name = "labelSendFrequency";
			this.labelSendFrequency.Size = new System.Drawing.Size(161, 18);
			this.labelSendFrequency.TabIndex = 234;
			this.labelSendFrequency.Text = "Repeat Every";
			this.labelSendFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormTransworldSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(424, 538);
			this.Controls.Add(this.groupSendActivity);
			this.Controls.Add(this.checkHideButtons);
			this.Controls.Add(this.labelClinicEnable);
			this.Controls.Add(this.groupClinicSettings);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.checkEnabled);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTransworldSetup";
			this.Text = "Transworld SFTP Setup";
			this.Load += new System.EventHandler(this.FormTransworldSetup_Load);
			this.groupClinicSettings.ResumeLayout(false);
			this.groupServicesEnabled.ResumeLayout(false);
			this.groupSftpServerInfo.ResumeLayout(false);
			this.groupSftpServerInfo.PerformLayout();
			this.groupClientIDs.ResumeLayout(false);
			this.groupClientIDs.PerformLayout();
			this.groupSendActivity.ResumeLayout(false);
			this.groupSendActivity.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericSendFrequency)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.TextBox textSftpPassword;
		private System.Windows.Forms.TextBox textSftpUsername;
		private System.Windows.Forms.Label labelSftpPassword;
		private System.Windows.Forms.Label labelSftpUsername;
		private System.Windows.Forms.TextBox textSftpAddress;
		private System.Windows.Forms.Label labelSftpAddress;
		private System.Windows.Forms.TextBox textClientIdAccelerator;
		private System.Windows.Forms.Label labelClientIdAccelerator;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.GroupBox groupClinicSettings;
		private System.Windows.Forms.TextBox textClientIdCollection;
		private System.Windows.Forms.Label labelClientIdCollection;
		private System.Windows.Forms.CheckBox checkThankYouLetter;
		private ValidNum textSftpPort;
		private System.Windows.Forms.Label labelSftpPort;
		private System.Windows.Forms.Label labelClinicEnable;
		private System.Windows.Forms.GroupBox groupSftpServerInfo;
		private System.Windows.Forms.GroupBox groupClientIDs;
		private System.Windows.Forms.CheckBox checkHideButtons;
		private System.Windows.Forms.Label labelSendTimeOfDay;
		private System.Windows.Forms.TextBox textUpdatesTimeOfDay;
		private System.Windows.Forms.Label labelODService;
		private System.Windows.Forms.GroupBox groupSendActivity;
		private System.Windows.Forms.NumericUpDown numericSendFrequency;
		private System.Windows.Forms.ComboBox comboSendFrequencyUnits;
		private System.Windows.Forms.Label labelSendFrequency;
		private System.Windows.Forms.GroupBox groupServicesEnabled;
		private System.Windows.Forms.CheckBox checkAccelService;
		private System.Windows.Forms.CheckBox checkPRService;
		private System.Windows.Forms.CheckBox checkCollService;
		private System.Windows.Forms.Label labelPaidInFullBillType;
		private UI.ComboBoxOD comboPaidInFullBillType;
		private UI.ComboBoxOD comboNegAdjType;
		private UI.ComboBoxOD comboPosAdjType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}