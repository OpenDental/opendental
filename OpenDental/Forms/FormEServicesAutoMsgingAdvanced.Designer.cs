
namespace OpenDental {
	partial class FormEServicesAutoMsgingAdvanced {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesAutoMsgingAdvanced));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.groupAutomationStatuses = new OpenDental.UI.GroupBox();
			this.butExclusionDays = new OpenDental.UI.Button();
			this.radio2ClickConfirm = new System.Windows.Forms.RadioButton();
			this.radio1ClickConfirm = new System.Windows.Forms.RadioButton();
			this.comboStatusEFailed = new OpenDental.UI.ComboBox();
			this.label50 = new System.Windows.Forms.Label();
			this.comboStatusEDeclined = new OpenDental.UI.ComboBox();
			this.comboStatusESent = new OpenDental.UI.ComboBox();
			this.comboStatusEAccepted = new OpenDental.UI.ComboBox();
			this.label51 = new System.Windows.Forms.Label();
			this.label52 = new System.Windows.Forms.Label();
			this.label53 = new System.Windows.Forms.Label();
			this.checkEnableNoClinic = new OpenDental.UI.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.labelThankYouTitle = new System.Windows.Forms.Label();
			this.textThankYouTitle = new System.Windows.Forms.TextBox();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.checkUseDefault = new OpenDental.UI.CheckBox();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.textPremedTemplate = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.groupAutomationStatuses.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasAutoWrappedHeaders = true;
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(13, 231);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridMain.Size = new System.Drawing.Size(1066, 374);
			this.gridMain.TabIndex = 0;
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(923, 611);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1004, 611);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Cancel";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupAutomationStatuses
			// 
			this.groupAutomationStatuses.Controls.Add(this.butExclusionDays);
			this.groupAutomationStatuses.Controls.Add(this.radio2ClickConfirm);
			this.groupAutomationStatuses.Controls.Add(this.radio1ClickConfirm);
			this.groupAutomationStatuses.Controls.Add(this.comboStatusEFailed);
			this.groupAutomationStatuses.Controls.Add(this.label50);
			this.groupAutomationStatuses.Controls.Add(this.comboStatusEDeclined);
			this.groupAutomationStatuses.Controls.Add(this.comboStatusESent);
			this.groupAutomationStatuses.Controls.Add(this.comboStatusEAccepted);
			this.groupAutomationStatuses.Controls.Add(this.label51);
			this.groupAutomationStatuses.Controls.Add(this.label52);
			this.groupAutomationStatuses.Controls.Add(this.label53);
			this.groupAutomationStatuses.Location = new System.Drawing.Point(13, 12);
			this.groupAutomationStatuses.Name = "groupAutomationStatuses";
			this.groupAutomationStatuses.Size = new System.Drawing.Size(349, 213);
			this.groupAutomationStatuses.TabIndex = 267;
			this.groupAutomationStatuses.Text = "eConfirmation Settings";
			// 
			// butExclusionDays
			// 
			this.butExclusionDays.Location = new System.Drawing.Point(229, 182);
			this.butExclusionDays.Name = "butExclusionDays";
			this.butExclusionDays.Size = new System.Drawing.Size(96, 24);
			this.butExclusionDays.TabIndex = 517;
			this.butExclusionDays.Text = "Exclusion Days";
			this.butExclusionDays.UseVisualStyleBackColor = true;
			this.butExclusionDays.Click += new System.EventHandler(this.butExclusionDays_Click);
			// 
			// radio2ClickConfirm
			// 
			this.radio2ClickConfirm.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radio2ClickConfirm.Location = new System.Drawing.Point(6, 146);
			this.radio2ClickConfirm.Name = "radio2ClickConfirm";
			this.radio2ClickConfirm.Size = new System.Drawing.Size(319, 30);
			this.radio2ClickConfirm.TabIndex = 176;
			this.radio2ClickConfirm.TabStop = true;
			this.radio2ClickConfirm.Text = "Confirm in portal after clicking link (2-click confirmation)";
			this.radio2ClickConfirm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radio1ClickConfirm
			// 
			this.radio1ClickConfirm.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radio1ClickConfirm.Location = new System.Drawing.Point(6, 119);
			this.radio1ClickConfirm.Name = "radio1ClickConfirm";
			this.radio1ClickConfirm.Size = new System.Drawing.Size(319, 31);
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
			this.comboStatusEFailed.Size = new System.Drawing.Size(228, 21);
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
			// comboStatusEDeclined
			// 
			this.comboStatusEDeclined.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusEDeclined.Location = new System.Drawing.Point(97, 67);
			this.comboStatusEDeclined.Name = "comboStatusEDeclined";
			this.comboStatusEDeclined.Size = new System.Drawing.Size(228, 21);
			this.comboStatusEDeclined.TabIndex = 170;
			// 
			// comboStatusESent
			// 
			this.comboStatusESent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusESent.Location = new System.Drawing.Point(97, 17);
			this.comboStatusESent.Name = "comboStatusESent";
			this.comboStatusESent.Size = new System.Drawing.Size(228, 21);
			this.comboStatusESent.TabIndex = 166;
			// 
			// comboStatusEAccepted
			// 
			this.comboStatusEAccepted.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboStatusEAccepted.Location = new System.Drawing.Point(97, 42);
			this.comboStatusEAccepted.Name = "comboStatusEAccepted";
			this.comboStatusEAccepted.Size = new System.Drawing.Size(228, 21);
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
			// checkEnableNoClinic
			// 
			this.checkEnableNoClinic.Location = new System.Drawing.Point(368, 161);
			this.checkEnableNoClinic.Name = "checkEnableNoClinic";
			this.checkEnableNoClinic.Size = new System.Drawing.Size(355, 17);
			this.checkEnableNoClinic.TabIndex = 172;
			this.checkEnableNoClinic.Text = "Allow Automated Messaging from Appts w/o Clinic";
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label11.Location = new System.Drawing.Point(10, 611);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(647, 46);
			this.label11.TabIndex = 268;
			this.label11.Text = resources.GetString("label11.Text");
			// 
			// labelThankYouTitle
			// 
			this.labelThankYouTitle.Location = new System.Drawing.Point(6, 47);
			this.labelThankYouTitle.Name = "labelThankYouTitle";
			this.labelThankYouTitle.Size = new System.Drawing.Size(213, 16);
			this.labelThankYouTitle.TabIndex = 277;
			this.labelThankYouTitle.Text = "Auto Message Calendar Event Title";
			this.labelThankYouTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textThankYouTitle
			// 
			this.textThankYouTitle.AcceptsTab = true;
			this.textThankYouTitle.BackColor = System.Drawing.SystemColors.Window;
			this.textThankYouTitle.Location = new System.Drawing.Point(225, 46);
			this.textThankYouTitle.Name = "textThankYouTitle";
			this.textThankYouTitle.Size = new System.Drawing.Size(480, 20);
			this.textThankYouTitle.TabIndex = 276;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkUseDefault);
			this.groupBox1.Controls.Add(this.comboClinic);
			this.groupBox1.Controls.Add(this.textThankYouTitle);
			this.groupBox1.Controls.Add(this.labelThankYouTitle);
			this.groupBox1.Location = new System.Drawing.Point(368, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(711, 82);
			this.groupBox1.TabIndex = 278;
			this.groupBox1.Text = "Auto Message Settings";
			// 
			// checkUseDefault
			// 
			this.checkUseDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseDefault.Location = new System.Drawing.Point(73, 20);
			this.checkUseDefault.Name = "checkUseDefault";
			this.checkUseDefault.Size = new System.Drawing.Size(165, 24);
			this.checkUseDefault.TabIndex = 280;
			this.checkUseDefault.Text = "Use Practice Default";
			this.checkUseDefault.Click += new System.EventHandler(this.checkUseDefault_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(505, 19);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(200, 21);
			this.comboClinic.TabIndex = 279;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textPremedTemplate);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(368, 104);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(711, 51);
			this.groupBox2.TabIndex = 279;
			this.groupBox2.Text = "eReminder Settings";
			// 
			// textPremedTemplate
			// 
			this.textPremedTemplate.AcceptsTab = true;
			this.textPremedTemplate.BackColor = System.Drawing.SystemColors.Window;
			this.textPremedTemplate.Location = new System.Drawing.Point(225, 18);
			this.textPremedTemplate.Name = "textPremedTemplate";
			this.textPremedTemplate.Size = new System.Drawing.Size(480, 20);
			this.textPremedTemplate.TabIndex = 278;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(213, 16);
			this.label1.TabIndex = 279;
			this.label1.Text = "Premedicate Template";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// colorDialog
			// 
			this.colorDialog.FullOpen = true;
			// 
			// FormEServicesAutoMsgingAdvanced
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1091, 666);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.groupAutomationStatuses);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.checkEnableNoClinic);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Name = "FormEServicesAutoMsgingAdvanced";
			this.Text = "eServices Automated Messaging Advanced Settings";
			this.Load += new System.EventHandler(this.FormAutomatedConfirmationStatuses_Load);
			this.groupAutomationStatuses.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GridOD gridMain;
		private UI.Button butOK;
		private UI.Button butClose;
		private OpenDental.UI.GroupBox groupAutomationStatuses;
		private System.Windows.Forms.RadioButton radio2ClickConfirm;
		private System.Windows.Forms.RadioButton radio1ClickConfirm;
		private UI.ComboBox comboStatusEFailed;
		private System.Windows.Forms.Label label50;
		private OpenDental.UI.CheckBox checkEnableNoClinic;
		private UI.ComboBox comboStatusEDeclined;
		private UI.ComboBox comboStatusESent;
		private UI.ComboBox comboStatusEAccepted;
		private System.Windows.Forms.Label label51;
		private System.Windows.Forms.Label label52;
		private System.Windows.Forms.Label label53;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label labelThankYouTitle;
		private System.Windows.Forms.TextBox textThankYouTitle;
		private OpenDental.UI.GroupBox groupBox1;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textPremedTemplate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColorDialog colorDialog;
        private OpenDental.UI.CheckBox checkUseDefault;
		private UI.Button butExclusionDays;
	}
}