using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPayConnectSetup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayConnectSetup));
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.checkEnabled = new OpenDental.UI.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboPaymentType = new OpenDental.UI.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.comboClinic = new OpenDental.UI.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.groupPaySettings = new OpenDental.UI.GroupBox();
			this.butMerchantInfo = new OpenDental.UI.Button();
			this.groupBoxVersion = new OpenDental.UI.GroupBox();
			this.radioVersion2 = new System.Windows.Forms.RadioButton();
			this.radioVersion1 = new System.Windows.Forms.RadioButton();
			this.checkSurcharge = new OpenDental.UI.CheckBox();
			this.textAPISecret = new System.Windows.Forms.TextBox();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.butGenerateToken = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textToken = new System.Windows.Forms.TextBox();
			this.checkPatientPortalPayEnabled = new OpenDental.UI.CheckBox();
			this.checkPreventSavingNewCC = new OpenDental.UI.CheckBox();
			this.comboDefaultProcessing = new OpenDental.UI.ComboBox();
			this.labelDefaultProcMethod = new System.Windows.Forms.Label();
			this.checkForceRecurring = new OpenDental.UI.CheckBox();
			this.checkTerminal = new OpenDental.UI.CheckBox();
			this.butDownloadDriver = new OpenDental.UI.Button();
			this.labelClinicEnable = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBoxTerminals = new OpenDental.UI.GroupBox();
			this.butAddTerminal = new OpenDental.UI.Button();
			this.gridODTerminals = new OpenDental.UI.GridOD();
			this.groupPaySettings.SuspendLayout();
			this.groupBoxVersion.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBoxTerminals.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(29, 28);
			this.linkLabel1.Location = new System.Drawing.Point(12, 9);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(312, 16);
			this.linkLabel1.TabIndex = 0;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "The PayConnect website is at www.payconnect.com";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Location = new System.Drawing.Point(97, 34);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(226, 18);
			this.checkEnabled.TabIndex = 1;
			this.checkEnabled.Text = "Enabled (affects all clinics)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(432, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(124, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Payment Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaymentType
			// 
			this.comboPaymentType.Location = new System.Drawing.Point(557, 19);
			this.comboPaymentType.Name = "comboPaymentType";
			this.comboPaymentType.Size = new System.Drawing.Size(175, 21);
			this.comboPaymentType.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(129, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Username";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(218, 21);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(175, 20);
			this.textUsername.TabIndex = 1;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(132, 50);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(85, 16);
			this.labelPassword.TabIndex = 0;
			this.labelPassword.Text = "Password";
			this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(218, 48);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(175, 20);
			this.textPassword.TabIndex = 2;
			this.textPassword.UseSystemPasswordChar = true;
			// 
			// comboClinic
			// 
			this.comboClinic.Location = new System.Drawing.Point(148, 89);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(175, 21);
			this.comboClinic.TabIndex = 3;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(69, 89);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(73, 16);
			this.labelClinic.TabIndex = 0;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPaySettings
			// 
			this.groupPaySettings.Controls.Add(this.butMerchantInfo);
			this.groupPaySettings.Controls.Add(this.groupBoxVersion);
			this.groupPaySettings.Controls.Add(this.checkSurcharge);
			this.groupPaySettings.Controls.Add(this.textAPISecret);
			this.groupPaySettings.Controls.Add(this.groupBox1);
			this.groupPaySettings.Controls.Add(this.checkPreventSavingNewCC);
			this.groupPaySettings.Controls.Add(this.comboDefaultProcessing);
			this.groupPaySettings.Controls.Add(this.labelDefaultProcMethod);
			this.groupPaySettings.Controls.Add(this.checkForceRecurring);
			this.groupPaySettings.Controls.Add(this.checkTerminal);
			this.groupPaySettings.Controls.Add(this.textPassword);
			this.groupPaySettings.Controls.Add(this.labelPassword);
			this.groupPaySettings.Controls.Add(this.textUsername);
			this.groupPaySettings.Controls.Add(this.label2);
			this.groupPaySettings.Controls.Add(this.comboPaymentType);
			this.groupPaySettings.Controls.Add(this.label1);
			this.groupPaySettings.Location = new System.Drawing.Point(9, 122);
			this.groupPaySettings.Name = "groupPaySettings";
			this.groupPaySettings.Size = new System.Drawing.Size(754, 173);
			this.groupPaySettings.TabIndex = 4;
			this.groupPaySettings.Text = "Clinic Payment Settings";
			// 
			// butMerchantInfo
			// 
			this.butMerchantInfo.Location = new System.Drawing.Point(21, 97);
			this.butMerchantInfo.Name = "butMerchantInfo";
			this.butMerchantInfo.Size = new System.Drawing.Size(106, 24);
			this.butMerchantInfo.TabIndex = 13;
			this.butMerchantInfo.Text = "Merchant Info";
			this.butMerchantInfo.UseVisualStyleBackColor = true;
			this.butMerchantInfo.Visible = false;
			this.butMerchantInfo.Click += new System.EventHandler(this.butMerchantInfo_Click);
			// 
			// groupBoxVersion
			// 
			this.groupBoxVersion.Controls.Add(this.radioVersion2);
			this.groupBoxVersion.Controls.Add(this.radioVersion1);
			this.groupBoxVersion.Location = new System.Drawing.Point(21, 22);
			this.groupBoxVersion.Name = "groupBoxVersion";
			this.groupBoxVersion.Size = new System.Drawing.Size(106, 68);
			this.groupBoxVersion.TabIndex = 9;
			this.groupBoxVersion.Text = "Program Version";
			// 
			// radioVersion2
			// 
			this.radioVersion2.Location = new System.Drawing.Point(19, 45);
			this.radioVersion2.Name = "radioVersion2";
			this.radioVersion2.Size = new System.Drawing.Size(56, 17);
			this.radioVersion2.TabIndex = 1;
			this.radioVersion2.TabStop = true;
			this.radioVersion2.Text = "2.0";
			this.radioVersion2.UseVisualStyleBackColor = true;
			this.radioVersion2.CheckedChanged += new System.EventHandler(this.radioVersion2_CheckedChanged);
			// 
			// radioVersion1
			// 
			this.radioVersion1.Location = new System.Drawing.Point(19, 18);
			this.radioVersion1.Name = "radioVersion1";
			this.radioVersion1.Size = new System.Drawing.Size(75, 21);
			this.radioVersion1.TabIndex = 0;
			this.radioVersion1.TabStop = true;
			this.radioVersion1.Text = "1.0";
			this.radioVersion1.UseVisualStyleBackColor = true;
			this.radioVersion1.CheckedChanged += new System.EventHandler(this.radioVersion1_CheckedChanged);
			// 
			// checkSurcharge
			// 
			this.checkSurcharge.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSurcharge.Location = new System.Drawing.Point(243, 141);
			this.checkSurcharge.Name = "checkSurcharge";
			this.checkSurcharge.Size = new System.Drawing.Size(150, 18);
			this.checkSurcharge.TabIndex = 12;
			this.checkSurcharge.Text = "Surcharge account";
			// 
			// textAPISecret
			// 
			this.textAPISecret.Location = new System.Drawing.Point(218, 21);
			this.textAPISecret.Name = "textAPISecret";
			this.textAPISecret.Size = new System.Drawing.Size(175, 20);
			this.textAPISecret.TabIndex = 11;
			this.textAPISecret.UseSystemPasswordChar = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butGenerateToken);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textToken);
			this.groupBox1.Controls.Add(this.checkPatientPortalPayEnabled);
			this.groupBox1.Location = new System.Drawing.Point(435, 82);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(307, 74);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.Text = "Payment Portal Payments";
			// 
			// butGenerateToken
			// 
			this.butGenerateToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butGenerateToken.Location = new System.Drawing.Point(212, 37);
			this.butGenerateToken.Name = "butGenerateToken";
			this.butGenerateToken.Size = new System.Drawing.Size(75, 26);
			this.butGenerateToken.TabIndex = 12;
			this.butGenerateToken.Text = "Generate";
			this.butGenerateToken.Click += new System.EventHandler(this.butGenerateToken_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(35, 41);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 16);
			this.label4.TabIndex = 11;
			this.label4.Text = "Token";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textToken
			// 
			this.textToken.Location = new System.Drawing.Point(106, 40);
			this.textToken.Name = "textToken";
			this.textToken.ReadOnly = true;
			this.textToken.Size = new System.Drawing.Size(103, 20);
			this.textToken.TabIndex = 10;
			// 
			// checkPatientPortalPayEnabled
			// 
			this.checkPatientPortalPayEnabled.Location = new System.Drawing.Point(106, 19);
			this.checkPatientPortalPayEnabled.Name = "checkPatientPortalPayEnabled";
			this.checkPatientPortalPayEnabled.Size = new System.Drawing.Size(127, 17);
			this.checkPatientPortalPayEnabled.TabIndex = 9;
			this.checkPatientPortalPayEnabled.Text = "Enabled";
			this.checkPatientPortalPayEnabled.Click += new System.EventHandler(this.checkPatientPortalPayEnabled_Click);
			// 
			// checkPreventSavingNewCC
			// 
			this.checkPreventSavingNewCC.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreventSavingNewCC.Location = new System.Drawing.Point(175, 119);
			this.checkPreventSavingNewCC.Name = "checkPreventSavingNewCC";
			this.checkPreventSavingNewCC.Size = new System.Drawing.Size(218, 17);
			this.checkPreventSavingNewCC.TabIndex = 8;
			this.checkPreventSavingNewCC.Text = "Prevent saving new cards";
			// 
			// comboDefaultProcessing
			// 
			this.comboDefaultProcessing.Location = new System.Drawing.Point(557, 46);
			this.comboDefaultProcessing.Name = "comboDefaultProcessing";
			this.comboDefaultProcessing.Size = new System.Drawing.Size(175, 21);
			this.comboDefaultProcessing.TabIndex = 6;
			// 
			// labelDefaultProcMethod
			// 
			this.labelDefaultProcMethod.Location = new System.Drawing.Point(432, 43);
			this.labelDefaultProcMethod.Name = "labelDefaultProcMethod";
			this.labelDefaultProcMethod.Size = new System.Drawing.Size(124, 26);
			this.labelDefaultProcMethod.TabIndex = 7;
			this.labelDefaultProcMethod.Text = "Default Processing Method";
			this.labelDefaultProcMethod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkForceRecurring
			// 
			this.checkForceRecurring.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkForceRecurring.Location = new System.Drawing.Point(175, 89);
			this.checkForceRecurring.Name = "checkForceRecurring";
			this.checkForceRecurring.Size = new System.Drawing.Size(218, 30);
			this.checkForceRecurring.TabIndex = 5;
			this.checkForceRecurring.Text = "Recurring charges force duplicates by default";
			// 
			// checkTerminal
			// 
			this.checkTerminal.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTerminal.Location = new System.Drawing.Point(175, 74);
			this.checkTerminal.Name = "checkTerminal";
			this.checkTerminal.Size = new System.Drawing.Size(218, 17);
			this.checkTerminal.TabIndex = 3;
			this.checkTerminal.Text = "Enable terminal processing";
			this.checkTerminal.CheckedChanged += new System.EventHandler(this.checkTerminal_CheckedChanged);
			// 
			// butDownloadDriver
			// 
			this.butDownloadDriver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDownloadDriver.Location = new System.Drawing.Point(10, 670);
			this.butDownloadDriver.Name = "butDownloadDriver";
			this.butDownloadDriver.Size = new System.Drawing.Size(93, 26);
			this.butDownloadDriver.TabIndex = 5;
			this.butDownloadDriver.Text = "Download Driver";
			this.butDownloadDriver.Visible = false;
			this.butDownloadDriver.Click += new System.EventHandler(this.butDownloadDriver_Click);
			// 
			// labelClinicEnable
			// 
			this.labelClinicEnable.Location = new System.Drawing.Point(77, 55);
			this.labelClinicEnable.Name = "labelClinicEnable";
			this.labelClinicEnable.Size = new System.Drawing.Size(246, 28);
			this.labelClinicEnable.TabIndex = 2;
			this.labelClinicEnable.Text = "To enable PayConnect for a clinic, set the credentials for that clinic.";
			this.labelClinicEnable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(607, 670);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(693, 670);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupBoxTerminals
			// 
			this.groupBoxTerminals.Controls.Add(this.butAddTerminal);
			this.groupBoxTerminals.Controls.Add(this.gridODTerminals);
			this.groupBoxTerminals.Location = new System.Drawing.Point(9, 302);
			this.groupBoxTerminals.Name = "groupBoxTerminals";
			this.groupBoxTerminals.Size = new System.Drawing.Size(754, 355);
			this.groupBoxTerminals.TabIndex = 8;
			this.groupBoxTerminals.Text = "Terminals";
			// 
			// butAddTerminal
			// 
			this.butAddTerminal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddTerminal.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddTerminal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddTerminal.Location = new System.Drawing.Point(632, 318);
			this.butAddTerminal.Name = "butAddTerminal";
			this.butAddTerminal.Size = new System.Drawing.Size(110, 24);
			this.butAddTerminal.TabIndex = 152;
			this.butAddTerminal.Text = "Add Terminal";
			this.butAddTerminal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.butAddTerminal.Click += new System.EventHandler(this.butAddTerminal_Click);
			// 
			// gridODTerminals
			// 
			this.gridODTerminals.Location = new System.Drawing.Point(9, 21);
			this.gridODTerminals.Name = "gridODTerminals";
			this.gridODTerminals.Size = new System.Drawing.Size(733, 291);
			this.gridODTerminals.TabIndex = 0;
			this.gridODTerminals.Title = "Terminals";
			this.gridODTerminals.TranslationName = "table devices";
			this.gridODTerminals.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridODTerminals_CellDoubleClick);
			// 
			// FormPayConnectSetup
			// 
			this.ClientSize = new System.Drawing.Size(776, 708);
			this.Controls.Add(this.groupBoxTerminals);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butDownloadDriver);
			this.Controls.Add(this.labelClinicEnable);
			this.Controls.Add(this.groupPaySettings);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.linkLabel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPayConnectSetup";
			this.ShowInTaskbar = false;
			this.Text = "PayConnect Setup";
			this.Load += new System.EventHandler(this.FormPayConnectSetup_Load);
			this.groupPaySettings.ResumeLayout(false);
			this.groupPaySettings.PerformLayout();
			this.groupBoxVersion.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBoxTerminals.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private LinkLabel linkLabel1;
		private OpenDental.UI.CheckBox checkEnabled;
		private Label label1;
		private OpenDental.UI.ComboBox comboPaymentType;
		private Label label2;
		private TextBox textUsername;
		private Label labelPassword;
		private TextBox textPassword;
		private OpenDental.UI.ComboBox comboClinic;
		private Label labelClinic;
		private OpenDental.UI.GroupBox groupPaySettings;
		private Label labelClinicEnable;
		private OpenDental.UI.CheckBox checkTerminal;
		private UI.Button butDownloadDriver;
		private OpenDental.UI.CheckBox checkForceRecurring;
		private OpenDental.UI.ComboBox comboDefaultProcessing;
		private Label labelDefaultProcMethod;
		private OpenDental.UI.CheckBox checkPreventSavingNewCC;
		private OpenDental.UI.GroupBox groupBox1;
		private OpenDental.UI.CheckBox checkPatientPortalPayEnabled;
		private Label label4;
		private TextBox textToken;
		private UI.Button butGenerateToken;
		private UI.GroupBox groupBoxTerminals;
		private UI.GridOD gridODTerminals;
		private UI.GroupBox groupBoxVersion;
		private RadioButton radioVersion2;
		private RadioButton radioVersion1;
		private TextBox textAPISecret;
		private UI.CheckBox checkSurcharge;
		private UI.Button butAddTerminal;
		private UI.Button butMerchantInfo;
	}
}
