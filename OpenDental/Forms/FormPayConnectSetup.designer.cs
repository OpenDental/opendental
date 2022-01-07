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
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayConnectSetup));
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboPaymentType = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.groupPaySettings = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butGenerateToken = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textToken = new System.Windows.Forms.TextBox();
			this.checkPatientPortalPayEnabled = new System.Windows.Forms.CheckBox();
			this.checkPreventSavingNewCC = new System.Windows.Forms.CheckBox();
			this.comboDefaultProcessing = new System.Windows.Forms.ComboBox();
			this.labelDefaultProcMethod = new System.Windows.Forms.Label();
			this.checkForceRecurring = new System.Windows.Forms.CheckBox();
			this.checkTerminal = new System.Windows.Forms.CheckBox();
			this.butDownloadDriver = new OpenDental.UI.Button();
			this.labelClinicEnable = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupPaySettings.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(29, 28);
			this.linkLabel1.Location = new System.Drawing.Point(10, 13);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(312, 16);
			this.linkLabel1.TabIndex = 0;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "The PayConnect website is at www.dentalxchange.com";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// checkEnabled
			// 
			this.checkEnabled.Location = new System.Drawing.Point(10, 37);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(226, 18);
			this.checkEnabled.TabIndex = 1;
			this.checkEnabled.Text = "Enabled (affects all clinics)";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(124, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Payment Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaymentType
			// 
			this.comboPaymentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPaymentType.FormattingEnabled = true;
			this.comboPaymentType.Location = new System.Drawing.Point(131, 19);
			this.comboPaymentType.MaxDropDownItems = 25;
			this.comboPaymentType.Name = "comboPaymentType";
			this.comboPaymentType.Size = new System.Drawing.Size(175, 21);
			this.comboPaymentType.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(124, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Username";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(131, 73);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(175, 20);
			this.textUsername.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 98);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(124, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Password";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(131, 96);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(175, 20);
			this.textPassword.TabIndex = 2;
			this.textPassword.UseSystemPasswordChar = true;
			// 
			// comboClinic
			// 
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.Location = new System.Drawing.Point(141, 95);
			this.comboClinic.MaxDropDownItems = 30;
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(175, 21);
			this.comboClinic.TabIndex = 3;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(10, 98);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(130, 16);
			this.labelClinic.TabIndex = 0;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPaySettings
			// 
			this.groupPaySettings.Controls.Add(this.groupBox1);
			this.groupPaySettings.Controls.Add(this.checkPreventSavingNewCC);
			this.groupPaySettings.Controls.Add(this.comboDefaultProcessing);
			this.groupPaySettings.Controls.Add(this.labelDefaultProcMethod);
			this.groupPaySettings.Controls.Add(this.checkForceRecurring);
			this.groupPaySettings.Controls.Add(this.checkTerminal);
			this.groupPaySettings.Controls.Add(this.textPassword);
			this.groupPaySettings.Controls.Add(this.label3);
			this.groupPaySettings.Controls.Add(this.textUsername);
			this.groupPaySettings.Controls.Add(this.label2);
			this.groupPaySettings.Controls.Add(this.comboPaymentType);
			this.groupPaySettings.Controls.Add(this.label1);
			this.groupPaySettings.Location = new System.Drawing.Point(10, 122);
			this.groupPaySettings.Name = "groupPaySettings";
			this.groupPaySettings.Size = new System.Drawing.Size(355, 275);
			this.groupPaySettings.TabIndex = 4;
			this.groupPaySettings.TabStop = false;
			this.groupPaySettings.Text = "Clinic Payment Settings";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butGenerateToken);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textToken);
			this.groupBox1.Controls.Add(this.checkPatientPortalPayEnabled);
			this.groupBox1.Location = new System.Drawing.Point(25, 189);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(307, 74);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Patient Portal Payments";
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
			this.checkPatientPortalPayEnabled.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientPortalPayEnabled.Location = new System.Drawing.Point(106, 19);
			this.checkPatientPortalPayEnabled.Name = "checkPatientPortalPayEnabled";
			this.checkPatientPortalPayEnabled.Size = new System.Drawing.Size(127, 17);
			this.checkPatientPortalPayEnabled.TabIndex = 9;
			this.checkPatientPortalPayEnabled.Text = "Enabled";
			this.checkPatientPortalPayEnabled.Click += new System.EventHandler(this.checkPatientPortalPayEnabled_Click);
			// 
			// checkPreventSavingNewCC
			// 
			this.checkPreventSavingNewCC.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreventSavingNewCC.Location = new System.Drawing.Point(131, 166);
			this.checkPreventSavingNewCC.Name = "checkPreventSavingNewCC";
			this.checkPreventSavingNewCC.Size = new System.Drawing.Size(218, 17);
			this.checkPreventSavingNewCC.TabIndex = 8;
			this.checkPreventSavingNewCC.Text = "Prevent saving new cards";
			// 
			// comboDefaultProcessing
			// 
			this.comboDefaultProcessing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDefaultProcessing.FormattingEnabled = true;
			this.comboDefaultProcessing.Location = new System.Drawing.Point(131, 46);
			this.comboDefaultProcessing.MaxDropDownItems = 25;
			this.comboDefaultProcessing.Name = "comboDefaultProcessing";
			this.comboDefaultProcessing.Size = new System.Drawing.Size(175, 21);
			this.comboDefaultProcessing.TabIndex = 6;
			// 
			// labelDefaultProcMethod
			// 
			this.labelDefaultProcMethod.Location = new System.Drawing.Point(6, 43);
			this.labelDefaultProcMethod.Name = "labelDefaultProcMethod";
			this.labelDefaultProcMethod.Size = new System.Drawing.Size(124, 26);
			this.labelDefaultProcMethod.TabIndex = 7;
			this.labelDefaultProcMethod.Text = "Default Processing Method";
			this.labelDefaultProcMethod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkForceRecurring
			// 
			this.checkForceRecurring.Location = new System.Drawing.Point(131, 136);
			this.checkForceRecurring.Name = "checkForceRecurring";
			this.checkForceRecurring.Size = new System.Drawing.Size(218, 30);
			this.checkForceRecurring.TabIndex = 5;
			this.checkForceRecurring.Text = "Recurring charges force duplicates by default";
			this.checkForceRecurring.UseVisualStyleBackColor = true;
			// 
			// checkTerminal
			// 
			this.checkTerminal.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTerminal.Location = new System.Drawing.Point(131, 121);
			this.checkTerminal.Name = "checkTerminal";
			this.checkTerminal.Size = new System.Drawing.Size(218, 17);
			this.checkTerminal.TabIndex = 3;
			this.checkTerminal.Text = "Enable terminal processing";
			this.checkTerminal.CheckedChanged += new System.EventHandler(this.checkTerminal_CheckedChanged);
			// 
			// butDownloadDriver
			// 
			this.butDownloadDriver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDownloadDriver.Location = new System.Drawing.Point(10, 405);
			this.butDownloadDriver.Name = "butDownloadDriver";
			this.butDownloadDriver.Size = new System.Drawing.Size(93, 26);
			this.butDownloadDriver.TabIndex = 5;
			this.butDownloadDriver.Text = "Download Driver";
			this.butDownloadDriver.Visible = false;
			this.butDownloadDriver.Click += new System.EventHandler(this.butDownloadDriver_Click);
			// 
			// labelClinicEnable
			// 
			this.labelClinicEnable.Location = new System.Drawing.Point(44, 58);
			this.labelClinicEnable.Name = "labelClinicEnable";
			this.labelClinicEnable.Size = new System.Drawing.Size(246, 28);
			this.labelClinicEnable.TabIndex = 2;
			this.labelClinicEnable.Text = "To enable PayConnect for a clinic, set the Username and Password for that clinic." +
    "";
			this.labelClinicEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(204, 405);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(290, 405);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormPayConnectSetup
			// 
			this.ClientSize = new System.Drawing.Size(377, 443);
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
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private LinkLabel linkLabel1;
		private CheckBox checkEnabled;
		private Label label1;
		private ComboBox comboPaymentType;
		private Label label2;
		private TextBox textUsername;
		private Label label3;
		private TextBox textPassword;
		private ComboBox comboClinic;
		private Label labelClinic;
		private GroupBox groupPaySettings;
		private Label labelClinicEnable;
		private CheckBox checkTerminal;
		private UI.Button butDownloadDriver;
		private CheckBox checkForceRecurring;
		private ComboBox comboDefaultProcessing;
		private Label labelDefaultProcMethod;
		private CheckBox checkPreventSavingNewCC;
		private GroupBox groupBox1;
		private CheckBox checkPatientPortalPayEnabled;
		private Label label4;
		private TextBox textToken;
		private UI.Button butGenerateToken;
	}
}
