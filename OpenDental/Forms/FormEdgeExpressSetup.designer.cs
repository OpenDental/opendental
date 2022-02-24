namespace OpenDental{
	partial class FormEdgeExpressSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEdgeExpressSetup));
			this.butCancel = new OpenDental.UI.Button();
			this.checkPreventSavingNewCC = new System.Windows.Forms.CheckBox();
			this.checkForceDuplicate = new System.Windows.Forms.CheckBox();
			this.groupPaySettings = new System.Windows.Forms.GroupBox();
			this.checkWebPayEnabled = new System.Windows.Forms.CheckBox();
			this.checkPrintReceipt = new System.Windows.Forms.CheckBox();
			this.checkPromptSig = new System.Windows.Forms.CheckBox();
			this.groupCredentials = new System.Windows.Forms.GroupBox();
			this.textTerminalID = new System.Windows.Forms.TextBox();
			this.labelTerminalID = new System.Windows.Forms.Label();
			this.textAuthKey = new System.Windows.Forms.TextBox();
			this.labelAuthKey = new System.Windows.Forms.Label();
			this.textXWebID = new System.Windows.Forms.TextBox();
			this.labelXWebID = new System.Windows.Forms.Label();
			this.comboPaymentType = new System.Windows.Forms.ComboBox();
			this.labelPaymentType = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.butOK = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.labelClinicEnable = new System.Windows.Forms.Label();
			this.groupPaySettings.SuspendLayout();
			this.groupCredentials.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(341, 387);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 19;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkPreventSavingNewCC
			// 
			this.checkPreventSavingNewCC.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreventSavingNewCC.Location = new System.Drawing.Point(97, 102);
			this.checkPreventSavingNewCC.Name = "checkPreventSavingNewCC";
			this.checkPreventSavingNewCC.Size = new System.Drawing.Size(273, 17);
			this.checkPreventSavingNewCC.TabIndex = 8;
			this.checkPreventSavingNewCC.Text = "Prevent saving new cards";
			this.checkPreventSavingNewCC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreventSavingNewCC.UseVisualStyleBackColor = true;
			// 
			// checkForceDuplicate
			// 
			this.checkForceDuplicate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkForceDuplicate.Location = new System.Drawing.Point(97, 82);
			this.checkForceDuplicate.Name = "checkForceDuplicate";
			this.checkForceDuplicate.Size = new System.Drawing.Size(273, 17);
			this.checkForceDuplicate.TabIndex = 7;
			this.checkForceDuplicate.Text = "Recurring charges force duplicates by default";
			this.checkForceDuplicate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkForceDuplicate.UseVisualStyleBackColor = true;
			// 
			// groupPaySettings
			// 
			this.groupPaySettings.Controls.Add(this.checkWebPayEnabled);
			this.groupPaySettings.Controls.Add(this.checkPreventSavingNewCC);
			this.groupPaySettings.Controls.Add(this.checkForceDuplicate);
			this.groupPaySettings.Controls.Add(this.checkPrintReceipt);
			this.groupPaySettings.Controls.Add(this.checkPromptSig);
			this.groupPaySettings.Controls.Add(this.groupCredentials);
			this.groupPaySettings.Controls.Add(this.comboPaymentType);
			this.groupPaySettings.Controls.Add(this.labelPaymentType);
			this.groupPaySettings.Location = new System.Drawing.Point(24, 129);
			this.groupPaySettings.Name = "groupPaySettings";
			this.groupPaySettings.Size = new System.Drawing.Size(392, 245);
			this.groupPaySettings.TabIndex = 17;
			this.groupPaySettings.TabStop = false;
			this.groupPaySettings.Text = "Clinic Payment Settings";
			// 
			// checkWebPayEnabled
			// 
			this.checkWebPayEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWebPayEnabled.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWebPayEnabled.Location = new System.Drawing.Point(97, 122);
			this.checkWebPayEnabled.Name = "checkWebPayEnabled";
			this.checkWebPayEnabled.Size = new System.Drawing.Size(273, 17);
			this.checkWebPayEnabled.TabIndex = 8;
			this.checkWebPayEnabled.Text = "Enable payments for patient portal and eClipboard";
			this.checkWebPayEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWebPayEnabled.Click += new System.EventHandler(this.checkWebPayEnabled_Click);
			// 
			// checkPrintReceipt
			// 
			this.checkPrintReceipt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrintReceipt.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrintReceipt.Location = new System.Drawing.Point(97, 63);
			this.checkPrintReceipt.Name = "checkPrintReceipt";
			this.checkPrintReceipt.Size = new System.Drawing.Size(273, 17);
			this.checkPrintReceipt.TabIndex = 5;
			this.checkPrintReceipt.Text = "Print receipts by default";
			this.checkPrintReceipt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPromptSig
			// 
			this.checkPromptSig.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPromptSig.Location = new System.Drawing.Point(97, 43);
			this.checkPromptSig.Name = "checkPromptSig";
			this.checkPromptSig.Size = new System.Drawing.Size(273, 17);
			this.checkPromptSig.TabIndex = 4;
			this.checkPromptSig.Text = "Prompt for signature on CC trans by default";
			this.checkPromptSig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupCredentials
			// 
			this.groupCredentials.Controls.Add(this.textTerminalID);
			this.groupCredentials.Controls.Add(this.labelTerminalID);
			this.groupCredentials.Controls.Add(this.textAuthKey);
			this.groupCredentials.Controls.Add(this.labelAuthKey);
			this.groupCredentials.Controls.Add(this.textXWebID);
			this.groupCredentials.Controls.Add(this.labelXWebID);
			this.groupCredentials.Location = new System.Drawing.Point(6, 142);
			this.groupCredentials.Name = "groupCredentials";
			this.groupCredentials.Size = new System.Drawing.Size(377, 97);
			this.groupCredentials.TabIndex = 6;
			this.groupCredentials.TabStop = false;
			this.groupCredentials.Text = "X-Web Credentials";
			// 
			// textTerminalID
			// 
			this.textTerminalID.Location = new System.Drawing.Point(90, 66);
			this.textTerminalID.Name = "textTerminalID";
			this.textTerminalID.Size = new System.Drawing.Size(273, 20);
			this.textTerminalID.TabIndex = 3;
			// 
			// labelTerminalID
			// 
			this.labelTerminalID.Location = new System.Drawing.Point(3, 68);
			this.labelTerminalID.Name = "labelTerminalID";
			this.labelTerminalID.Size = new System.Drawing.Size(86, 16);
			this.labelTerminalID.TabIndex = 0;
			this.labelTerminalID.Text = "Terminal ID";
			this.labelTerminalID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAuthKey
			// 
			this.textAuthKey.Location = new System.Drawing.Point(90, 43);
			this.textAuthKey.Name = "textAuthKey";
			this.textAuthKey.Size = new System.Drawing.Size(273, 20);
			this.textAuthKey.TabIndex = 2;
			// 
			// labelAuthKey
			// 
			this.labelAuthKey.Location = new System.Drawing.Point(3, 45);
			this.labelAuthKey.Name = "labelAuthKey";
			this.labelAuthKey.Size = new System.Drawing.Size(86, 16);
			this.labelAuthKey.TabIndex = 0;
			this.labelAuthKey.Text = "Auth Key";
			this.labelAuthKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textXWebID
			// 
			this.textXWebID.Location = new System.Drawing.Point(90, 20);
			this.textXWebID.Name = "textXWebID";
			this.textXWebID.Size = new System.Drawing.Size(273, 20);
			this.textXWebID.TabIndex = 1;
			// 
			// labelXWebID
			// 
			this.labelXWebID.Location = new System.Drawing.Point(3, 22);
			this.labelXWebID.Name = "labelXWebID";
			this.labelXWebID.Size = new System.Drawing.Size(86, 16);
			this.labelXWebID.TabIndex = 0;
			this.labelXWebID.Text = "X-Web ID";
			this.labelXWebID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaymentType
			// 
			this.comboPaymentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPaymentType.FormattingEnabled = true;
			this.comboPaymentType.Location = new System.Drawing.Point(177, 19);
			this.comboPaymentType.MaxDropDownItems = 25;
			this.comboPaymentType.Name = "comboPaymentType";
			this.comboPaymentType.Size = new System.Drawing.Size(192, 21);
			this.comboPaymentType.TabIndex = 3;
			// 
			// labelPaymentType
			// 
			this.labelPaymentType.Location = new System.Drawing.Point(12, 21);
			this.labelPaymentType.Name = "labelPaymentType";
			this.labelPaymentType.Size = new System.Drawing.Size(162, 16);
			this.labelPaymentType.TabIndex = 0;
			this.labelPaymentType.Text = "Payment Type";
			this.labelPaymentType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(163, 105);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(229, 21);
			this.comboClinic.TabIndex = 16;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(255, 387);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 18;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Location = new System.Drawing.Point(120, 60);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(273, 17);
			this.checkEnabled.TabIndex = 13;
			this.checkEnabled.Text = "Enabled (affects all clinics)";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// linkLabel1
			// 
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(30, 41);
			this.linkLabel1.Location = new System.Drawing.Point(22, 32);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(410, 16);
			this.linkLabel1.TabIndex = 12;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "The EdgeExpress website is at https://www.globalpaymentsintegrated.com/";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// labelClinicEnable
			// 
			this.labelClinicEnable.Location = new System.Drawing.Point(22, 80);
			this.labelClinicEnable.Name = "labelClinicEnable";
			this.labelClinicEnable.Size = new System.Drawing.Size(410, 16);
			this.labelClinicEnable.TabIndex = 20;
			this.labelClinicEnable.Text = "To enable EdgeExpress for a clinic, enter the X-Web credentials for that clinic.";
			this.labelClinicEnable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormEdgeExpressSetup
			// 
			this.ClientSize = new System.Drawing.Size(435, 429);
			this.Controls.Add(this.labelClinicEnable);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupPaySettings);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.linkLabel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEdgeExpressSetup";
			this.Text = "EdgeExpress Setup";
			this.Load += new System.EventHandler(this.FormEdgeExpressSetup_Load);
			this.groupPaySettings.ResumeLayout(false);
			this.groupCredentials.ResumeLayout(false);
			this.groupCredentials.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkPreventSavingNewCC;
		private System.Windows.Forms.CheckBox checkForceDuplicate;
		private System.Windows.Forms.GroupBox groupPaySettings;
		private System.Windows.Forms.CheckBox checkPrintReceipt;
		private System.Windows.Forms.CheckBox checkPromptSig;
		private System.Windows.Forms.GroupBox groupCredentials;
		private System.Windows.Forms.TextBox textTerminalID;
		private System.Windows.Forms.Label labelTerminalID;
		private System.Windows.Forms.TextBox textAuthKey;
		private System.Windows.Forms.Label labelAuthKey;
		private System.Windows.Forms.TextBox textXWebID;
		private System.Windows.Forms.Label labelXWebID;
		private System.Windows.Forms.ComboBox comboPaymentType;
		private System.Windows.Forms.Label labelPaymentType;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.Button butOK;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label labelClinicEnable;
		private System.Windows.Forms.CheckBox checkWebPayEnabled;
	}
}