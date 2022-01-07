using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormXchargeSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormXchargeSetup));
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.textPath = new System.Windows.Forms.TextBox();
			this.labelPath = new System.Windows.Forms.Label();
			this.labelPaymentType = new System.Windows.Forms.Label();
			this.comboPaymentType = new System.Windows.Forms.ComboBox();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.labelUsername = new System.Windows.Forms.Label();
			this.textOverride = new System.Windows.Forms.TextBox();
			this.labelOverride = new System.Windows.Forms.Label();
			this.groupXWeb = new System.Windows.Forms.GroupBox();
			this.checkWebPayEnabled = new System.Windows.Forms.CheckBox();
			this.textTerminalID = new System.Windows.Forms.TextBox();
			this.labelTerminalID = new System.Windows.Forms.Label();
			this.labelXwebDesc = new System.Windows.Forms.Label();
			this.textAuthKey = new System.Windows.Forms.TextBox();
			this.labelAuthKey = new System.Windows.Forms.Label();
			this.textXWebID = new System.Windows.Forms.TextBox();
			this.labelXWebID = new System.Windows.Forms.Label();
			this.checkPrintReceipt = new System.Windows.Forms.CheckBox();
			this.checkPromptSig = new System.Windows.Forms.CheckBox();
			this.labelClinicEnable = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupPaySettings = new System.Windows.Forms.GroupBox();
			this.checkPreventSavingNewCC = new System.Windows.Forms.CheckBox();
			this.checkForceDuplicate = new System.Windows.Forms.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupXWeb.SuspendLayout();
			this.groupPaySettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(27, 64);
			this.linkLabel1.Location = new System.Drawing.Point(1, 13);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(482, 29);
			this.linkLabel1.TabIndex = 1;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "The X-Charge website is at https://opendental.com/resources/redirects/redirectope" +
    "nedge.html";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// checkEnabled
			// 
			this.checkEnabled.Location = new System.Drawing.Point(187, 45);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(273, 17);
			this.checkEnabled.TabIndex = 2;
			this.checkEnabled.Text = "Enabled (affects all clinics)";
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// textPath
			// 
			this.textPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPath.Location = new System.Drawing.Point(187, 65);
			this.textPath.Name = "textPath";
			this.textPath.Size = new System.Drawing.Size(273, 20);
			this.textPath.TabIndex = 3;
			// 
			// labelPath
			// 
			this.labelPath.Location = new System.Drawing.Point(24, 67);
			this.labelPath.Name = "labelPath";
			this.labelPath.Size = new System.Drawing.Size(162, 16);
			this.labelPath.TabIndex = 0;
			this.labelPath.Text = "Program Path";
			this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPaymentType
			// 
			this.labelPaymentType.Location = new System.Drawing.Point(12, 67);
			this.labelPaymentType.Name = "labelPaymentType";
			this.labelPaymentType.Size = new System.Drawing.Size(162, 16);
			this.labelPaymentType.TabIndex = 0;
			this.labelPaymentType.Text = "Payment Type";
			this.labelPaymentType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaymentType
			// 
			this.comboPaymentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPaymentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPaymentType.FormattingEnabled = true;
			this.comboPaymentType.Location = new System.Drawing.Point(175, 65);
			this.comboPaymentType.MaxDropDownItems = 25;
			this.comboPaymentType.Name = "comboPaymentType";
			this.comboPaymentType.Size = new System.Drawing.Size(192, 21);
			this.comboPaymentType.TabIndex = 3;
			// 
			// textPassword
			// 
			this.textPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPassword.Location = new System.Drawing.Point(175, 42);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(273, 20);
			this.textPassword.TabIndex = 2;
			this.textPassword.UseSystemPasswordChar = true;
			this.textPassword.TextChanged += new System.EventHandler(this.textPassword_TextChanged);
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(12, 44);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(162, 16);
			this.labelPassword.TabIndex = 0;
			this.labelPassword.Text = "Password";
			this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUsername
			// 
			this.textUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textUsername.Location = new System.Drawing.Point(175, 19);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(273, 20);
			this.textUsername.TabIndex = 1;
			// 
			// labelUsername
			// 
			this.labelUsername.Location = new System.Drawing.Point(12, 21);
			this.labelUsername.Name = "labelUsername";
			this.labelUsername.Size = new System.Drawing.Size(162, 16);
			this.labelUsername.TabIndex = 0;
			this.labelUsername.Text = "Username";
			this.labelUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOverride
			// 
			this.textOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textOverride.Location = new System.Drawing.Point(187, 88);
			this.textOverride.Name = "textOverride";
			this.textOverride.Size = new System.Drawing.Size(273, 20);
			this.textOverride.TabIndex = 4;
			// 
			// labelOverride
			// 
			this.labelOverride.Location = new System.Drawing.Point(6, 90);
			this.labelOverride.Name = "labelOverride";
			this.labelOverride.Size = new System.Drawing.Size(180, 16);
			this.labelOverride.TabIndex = 0;
			this.labelOverride.Text = "Local Path Override (usually blank)";
			this.labelOverride.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupXWeb
			// 
			this.groupXWeb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupXWeb.Controls.Add(this.checkWebPayEnabled);
			this.groupXWeb.Controls.Add(this.textTerminalID);
			this.groupXWeb.Controls.Add(this.labelTerminalID);
			this.groupXWeb.Controls.Add(this.labelXwebDesc);
			this.groupXWeb.Controls.Add(this.textAuthKey);
			this.groupXWeb.Controls.Add(this.labelAuthKey);
			this.groupXWeb.Controls.Add(this.textXWebID);
			this.groupXWeb.Controls.Add(this.labelXWebID);
			this.groupXWeb.Location = new System.Drawing.Point(6, 165);
			this.groupXWeb.Name = "groupXWeb";
			this.groupXWeb.Size = new System.Drawing.Size(448, 145);
			this.groupXWeb.TabIndex = 6;
			this.groupXWeb.TabStop = false;
			this.groupXWeb.Text = "X-Web";
			// 
			// checkWebPayEnabled
			// 
			this.checkWebPayEnabled.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWebPayEnabled.Location = new System.Drawing.Point(169, 122);
			this.checkWebPayEnabled.Name = "checkWebPayEnabled";
			this.checkWebPayEnabled.Size = new System.Drawing.Size(273, 17);
			this.checkWebPayEnabled.TabIndex = 7;
			this.checkWebPayEnabled.Text = "Enable X-Web for patient portal payments";
			this.checkWebPayEnabled.Click += new System.EventHandler(this.checkWebPayEnabled_Click);
			// 
			// textTerminalID
			// 
			this.textTerminalID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTerminalID.Location = new System.Drawing.Point(169, 99);
			this.textTerminalID.Name = "textTerminalID";
			this.textTerminalID.Size = new System.Drawing.Size(273, 20);
			this.textTerminalID.TabIndex = 3;
			// 
			// labelTerminalID
			// 
			this.labelTerminalID.Location = new System.Drawing.Point(6, 101);
			this.labelTerminalID.Name = "labelTerminalID";
			this.labelTerminalID.Size = new System.Drawing.Size(162, 16);
			this.labelTerminalID.TabIndex = 0;
			this.labelTerminalID.Text = "Terminal ID";
			this.labelTerminalID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelXwebDesc
			// 
			this.labelXwebDesc.Location = new System.Drawing.Point(6, 19);
			this.labelXwebDesc.Name = "labelXwebDesc";
			this.labelXwebDesc.Size = new System.Drawing.Size(436, 31);
			this.labelXwebDesc.TabIndex = 0;
			this.labelXwebDesc.Text = "The following settings are required to enable receiving online payments via the P" +
    "atient Portal.  These settings are provided by X-Charge when you sign up for X-W" +
    "eb.";
			this.labelXwebDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textAuthKey
			// 
			this.textAuthKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textAuthKey.Location = new System.Drawing.Point(169, 76);
			this.textAuthKey.Name = "textAuthKey";
			this.textAuthKey.Size = new System.Drawing.Size(273, 20);
			this.textAuthKey.TabIndex = 2;
			this.textAuthKey.TextChanged += new System.EventHandler(this.textAuthKey_TextChanged);
			// 
			// labelAuthKey
			// 
			this.labelAuthKey.Location = new System.Drawing.Point(6, 78);
			this.labelAuthKey.Name = "labelAuthKey";
			this.labelAuthKey.Size = new System.Drawing.Size(162, 16);
			this.labelAuthKey.TabIndex = 0;
			this.labelAuthKey.Text = "Auth Key";
			this.labelAuthKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textXWebID
			// 
			this.textXWebID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textXWebID.Location = new System.Drawing.Point(169, 53);
			this.textXWebID.Name = "textXWebID";
			this.textXWebID.Size = new System.Drawing.Size(273, 20);
			this.textXWebID.TabIndex = 1;
			// 
			// labelXWebID
			// 
			this.labelXWebID.Location = new System.Drawing.Point(6, 55);
			this.labelXWebID.Name = "labelXWebID";
			this.labelXWebID.Size = new System.Drawing.Size(162, 16);
			this.labelXWebID.TabIndex = 0;
			this.labelXWebID.Text = "XWeb ID";
			this.labelXWebID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPrintReceipt
			// 
			this.checkPrintReceipt.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrintReceipt.Location = new System.Drawing.Point(175, 109);
			this.checkPrintReceipt.Name = "checkPrintReceipt";
			this.checkPrintReceipt.Size = new System.Drawing.Size(273, 17);
			this.checkPrintReceipt.TabIndex = 5;
			this.checkPrintReceipt.Text = "Print receipts by default";
			// 
			// checkPromptSig
			// 
			this.checkPromptSig.Location = new System.Drawing.Point(175, 89);
			this.checkPromptSig.Name = "checkPromptSig";
			this.checkPromptSig.Size = new System.Drawing.Size(273, 17);
			this.checkPromptSig.TabIndex = 4;
			this.checkPromptSig.Text = "Prompt for signature on CC trans by default";
			// 
			// labelClinicEnable
			// 
			this.labelClinicEnable.Location = new System.Drawing.Point(24, 118);
			this.labelClinicEnable.Name = "labelClinicEnable";
			this.labelClinicEnable.Size = new System.Drawing.Size(436, 16);
			this.labelClinicEnable.TabIndex = 0;
			this.labelClinicEnable.Text = "To enable X-Charge for a clinic, set the User ID and Password for that clinic.";
			this.labelClinicEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(150, 144);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(229, 21);
			this.comboClinic.TabIndex = 5;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// groupPaySettings
			// 
			this.groupPaySettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupPaySettings.Controls.Add(this.checkPreventSavingNewCC);
			this.groupPaySettings.Controls.Add(this.checkForceDuplicate);
			this.groupPaySettings.Controls.Add(this.checkPrintReceipt);
			this.groupPaySettings.Controls.Add(this.checkPromptSig);
			this.groupPaySettings.Controls.Add(this.groupXWeb);
			this.groupPaySettings.Controls.Add(this.textUsername);
			this.groupPaySettings.Controls.Add(this.textPassword);
			this.groupPaySettings.Controls.Add(this.comboPaymentType);
			this.groupPaySettings.Controls.Add(this.labelUsername);
			this.groupPaySettings.Controls.Add(this.labelPassword);
			this.groupPaySettings.Controls.Add(this.labelPaymentType);
			this.groupPaySettings.Location = new System.Drawing.Point(12, 171);
			this.groupPaySettings.Name = "groupPaySettings";
			this.groupPaySettings.Size = new System.Drawing.Size(460, 318);
			this.groupPaySettings.TabIndex = 6;
			this.groupPaySettings.TabStop = false;
			this.groupPaySettings.Text = "Clinic Payment Settings";
			// 
			// checkPreventSavingNewCC
			// 
			this.checkPreventSavingNewCC.Location = new System.Drawing.Point(175, 148);
			this.checkPreventSavingNewCC.Name = "checkPreventSavingNewCC";
			this.checkPreventSavingNewCC.Size = new System.Drawing.Size(273, 17);
			this.checkPreventSavingNewCC.TabIndex = 8;
			this.checkPreventSavingNewCC.Text = "Prevent saving new cards";
			this.checkPreventSavingNewCC.UseVisualStyleBackColor = true;
			// 
			// checkForceDuplicate
			// 
			this.checkForceDuplicate.Location = new System.Drawing.Point(175, 128);
			this.checkForceDuplicate.Name = "checkForceDuplicate";
			this.checkForceDuplicate.Size = new System.Drawing.Size(273, 17);
			this.checkForceDuplicate.TabIndex = 7;
			this.checkForceDuplicate.Text = "Recurring charges force duplicates by default";
			this.checkForceDuplicate.UseVisualStyleBackColor = true;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(311, 495);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(397, 495);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormXchargeSetup
			// 
			this.ClientSize = new System.Drawing.Size(484, 533);
			this.Controls.Add(this.groupPaySettings);
			this.Controls.Add(this.labelClinicEnable);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.textOverride);
			this.Controls.Add(this.labelOverride);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.labelPath);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormXchargeSetup";
			this.ShowInTaskbar = false;
			this.Text = "X-Charge Setup";
			this.Load += new System.EventHandler(this.FormXchargeSetup_Load);
			this.groupXWeb.ResumeLayout(false);
			this.groupXWeb.PerformLayout();
			this.groupPaySettings.ResumeLayout(false);
			this.groupPaySettings.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private LinkLabel linkLabel1;
		private CheckBox checkEnabled;
		private TextBox textPath;
		private Label labelPath;
		private Label labelPaymentType;
		private ComboBox comboPaymentType;
		private TextBox textPassword;
		private Label labelPassword;
		private TextBox textUsername;
		private TextBox textOverride;
		private Label labelOverride;
		private Label labelUsername;
		private GroupBox groupXWeb;
		private Label labelXwebDesc;
		private TextBox textAuthKey;
		private Label labelAuthKey;
		private TextBox textXWebID;
		private Label labelXWebID;
		private TextBox textTerminalID;
		private Label labelTerminalID;
		private CheckBox checkPrintReceipt;
		private CheckBox checkPromptSig;
		private Label labelClinicEnable;
		private UI.ComboBoxClinicPicker comboClinic;
		private GroupBox groupPaySettings;
		private CheckBox checkWebPayEnabled;
		private CheckBox checkForceDuplicate;
		private CheckBox checkPreventSavingNewCC;
	}
}
