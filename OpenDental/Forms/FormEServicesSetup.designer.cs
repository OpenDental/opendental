namespace OpenDental{
	partial class FormEServicesSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesSetup));
			this.butCancel = new OpenDental.UI.Button();
			this.butTextingServices = new OpenDental.UI.Button();
			this.butMassEmail = new OpenDental.UI.Button();
			this.butPatPortal = new OpenDental.UI.Button();
			this.butMobileWeb = new OpenDental.UI.Button();
			this.butMisc = new OpenDental.UI.Button();
			this.butEConnector = new OpenDental.UI.Button();
			this.butEClipboard = new OpenDental.UI.Button();
			this.butECR = new OpenDental.UI.Button();
			this.butSignup = new OpenDental.UI.Button();
			this.label23 = new System.Windows.Forms.Label();
			this.labelECR = new System.Windows.Forms.Label();
			this.labelSignup = new System.Windows.Forms.Label();
			this.groupboxWebSched = new System.Windows.Forms.GroupBox();
			this.butExistingPatient = new OpenDental.UI.Button();
			this.butAdvanced = new OpenDental.UI.Button();
			this.butNewPatient = new OpenDental.UI.Button();
			this.butRecall = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butMobileSettings = new OpenDental.UI.Button();
			this.butSMSTemplateSetup = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.groupboxWebSched.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(407, 523);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 12;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butTextingServices
			// 
			this.butTextingServices.Location = new System.Drawing.Point(26, 213);
			this.butTextingServices.Name = "butTextingServices";
			this.butTextingServices.Size = new System.Drawing.Size(120, 24);
			this.butTextingServices.TabIndex = 6;
			this.butTextingServices.Text = "Texting Services";
			this.butTextingServices.UseVisualStyleBackColor = true;
			this.butTextingServices.Click += new System.EventHandler(this.butTextingServices_Click);
			// 
			// butMassEmail
			// 
			this.butMassEmail.Location = new System.Drawing.Point(26, 337);
			this.butMassEmail.Name = "butMassEmail";
			this.butMassEmail.Size = new System.Drawing.Size(120, 24);
			this.butMassEmail.TabIndex = 10;
			this.butMassEmail.Text = "Hosted Email";
			this.butMassEmail.UseVisualStyleBackColor = true;
			this.butMassEmail.Click += new System.EventHandler(this.butMassEmail_Click);
			// 
			// butPatPortal
			// 
			this.butPatPortal.Location = new System.Drawing.Point(26, 153);
			this.butPatPortal.Name = "butPatPortal";
			this.butPatPortal.Size = new System.Drawing.Size(120, 24);
			this.butPatPortal.TabIndex = 4;
			this.butPatPortal.Text = "Patient Portal";
			this.butPatPortal.UseVisualStyleBackColor = true;
			this.butPatPortal.Click += new System.EventHandler(this.butPatPortal_Click);
			// 
			// butMobileWeb
			// 
			this.butMobileWeb.Location = new System.Drawing.Point(26, 122);
			this.butMobileWeb.Name = "butMobileWeb";
			this.butMobileWeb.Size = new System.Drawing.Size(120, 24);
			this.butMobileWeb.TabIndex = 3;
			this.butMobileWeb.Text = "Mobile Web";
			this.butMobileWeb.UseVisualStyleBackColor = true;
			this.butMobileWeb.Click += new System.EventHandler(this.butMobileWeb_Click);
			// 
			// butMisc
			// 
			this.butMisc.Location = new System.Drawing.Point(26, 306);
			this.butMisc.Name = "butMisc";
			this.butMisc.Size = new System.Drawing.Size(120, 24);
			this.butMisc.TabIndex = 9;
			this.butMisc.Text = "Miscellaneous";
			this.butMisc.UseVisualStyleBackColor = true;
			this.butMisc.Click += new System.EventHandler(this.butMisc_Click);
			// 
			// butEConnector
			// 
			this.butEConnector.Location = new System.Drawing.Point(26, 91);
			this.butEConnector.Name = "butEConnector";
			this.butEConnector.Size = new System.Drawing.Size(120, 24);
			this.butEConnector.TabIndex = 2;
			this.butEConnector.Text = "eConnector Service";
			this.butEConnector.UseVisualStyleBackColor = true;
			this.butEConnector.Click += new System.EventHandler(this.butEConnector_Click);
			// 
			// butEClipboard
			// 
			this.butEClipboard.Location = new System.Drawing.Point(26, 275);
			this.butEClipboard.Name = "butEClipboard";
			this.butEClipboard.Size = new System.Drawing.Size(120, 24);
			this.butEClipboard.TabIndex = 8;
			this.butEClipboard.Text = "eClipboard";
			this.butEClipboard.UseVisualStyleBackColor = true;
			this.butEClipboard.Click += new System.EventHandler(this.butEClipboard_Click);
			// 
			// butECR
			// 
			this.butECR.Location = new System.Drawing.Point(26, 244);
			this.butECR.Name = "butECR";
			this.butECR.Size = new System.Drawing.Size(120, 24);
			this.butECR.TabIndex = 7;
			this.butECR.Text = "Automated Messaging";
			this.butECR.UseVisualStyleBackColor = true;
			this.butECR.Click += new System.EventHandler(this.butECR_Click);
			// 
			// butSignup
			// 
			this.butSignup.Location = new System.Drawing.Point(26, 60);
			this.butSignup.Name = "butSignup";
			this.butSignup.Size = new System.Drawing.Size(120, 24);
			this.butSignup.TabIndex = 1;
			this.butSignup.Text = "Signup";
			this.butSignup.UseVisualStyleBackColor = true;
			this.butSignup.Click += new System.EventHandler(this.butSignup_Click);
			// 
			// label23
			// 
			this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label23.Location = new System.Drawing.Point(18, 15);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(414, 26);
			this.label23.TabIndex = 245;
			this.label23.Text = "eServices are Open Dental features that can be delivered electronically via the i" +
    "nternet.  \r\nAll eServices hosted by Open Dental use the eConnector Service.";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelECR
			// 
			this.labelECR.Location = new System.Drawing.Point(150, 248);
			this.labelECR.Name = "labelECR";
			this.labelECR.Size = new System.Drawing.Size(339, 16);
			this.labelECR.TabIndex = 247;
			this.labelECR.Text = "Automated eReminders, eConfirmations, Thank-Yous, && Arrivals";
			this.labelECR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelSignup
			// 
			this.labelSignup.Location = new System.Drawing.Point(150, 64);
			this.labelSignup.Name = "labelSignup";
			this.labelSignup.Size = new System.Drawing.Size(87, 16);
			this.labelSignup.TabIndex = 246;
			this.labelSignup.Text = "Get Started Here";
			this.labelSignup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupboxWebSched
			// 
			this.groupboxWebSched.Controls.Add(this.butExistingPatient);
			this.groupboxWebSched.Controls.Add(this.butAdvanced);
			this.groupboxWebSched.Controls.Add(this.butNewPatient);
			this.groupboxWebSched.Controls.Add(this.butRecall);
			this.groupboxWebSched.Location = new System.Drawing.Point(19, 400);
			this.groupboxWebSched.Name = "groupboxWebSched";
			this.groupboxWebSched.Size = new System.Drawing.Size(134, 147);
			this.groupboxWebSched.TabIndex = 248;
			this.groupboxWebSched.TabStop = false;
			this.groupboxWebSched.Text = "Web Scheduling";
			// 
			// butExistingPatient
			// 
			this.butExistingPatient.Location = new System.Drawing.Point(7, 80);
			this.butExistingPatient.Name = "butExistingPatient";
			this.butExistingPatient.Size = new System.Drawing.Size(120, 24);
			this.butExistingPatient.TabIndex = 251;
			this.butExistingPatient.Text = "Existing Patient";
			this.butExistingPatient.UseVisualStyleBackColor = true;
			this.butExistingPatient.Click += new System.EventHandler(this.butExistingPatient_Click);
			// 
			// butAdvanced
			// 
			this.butAdvanced.Location = new System.Drawing.Point(7, 111);
			this.butAdvanced.Name = "butAdvanced";
			this.butAdvanced.Size = new System.Drawing.Size(121, 24);
			this.butAdvanced.TabIndex = 250;
			this.butAdvanced.Text = "Advanced";
			this.butAdvanced.UseVisualStyleBackColor = true;
			this.butAdvanced.Click += new System.EventHandler(this.butAdvanced_Click);
			// 
			// butNewPatient
			// 
			this.butNewPatient.Location = new System.Drawing.Point(7, 49);
			this.butNewPatient.Name = "butNewPatient";
			this.butNewPatient.Size = new System.Drawing.Size(120, 24);
			this.butNewPatient.TabIndex = 1;
			this.butNewPatient.Text = "New Patient";
			this.butNewPatient.UseVisualStyleBackColor = true;
			this.butNewPatient.Click += new System.EventHandler(this.butNewPatient_Click);
			// 
			// butRecall
			// 
			this.butRecall.Location = new System.Drawing.Point(7, 18);
			this.butRecall.Name = "butRecall";
			this.butRecall.Size = new System.Drawing.Size(120, 24);
			this.butRecall.TabIndex = 0;
			this.butRecall.Text = "Recall";
			this.butRecall.UseVisualStyleBackColor = true;
			this.butRecall.Click += new System.EventHandler(this.butRecall_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(152, 341);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(339, 16);
			this.label1.TabIndex = 249;
			this.label1.Text = "Includes Mass Email";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butMobileSettings
			// 
			this.butMobileSettings.Location = new System.Drawing.Point(26, 367);
			this.butMobileSettings.Name = "butMobileSettings";
			this.butMobileSettings.Size = new System.Drawing.Size(120, 24);
			this.butMobileSettings.TabIndex = 250;
			this.butMobileSettings.Text = "Mobile Settings";
			this.butMobileSettings.UseVisualStyleBackColor = true;
			this.butMobileSettings.Click += new System.EventHandler(this.butMobileSettings_Click);
			// 
			// butSMSTemplateSetup
			// 
			this.butSMSTemplateSetup.Location = new System.Drawing.Point(26, 183);
			this.butSMSTemplateSetup.Name = "butSMSTemplateSetup";
			this.butSMSTemplateSetup.Size = new System.Drawing.Size(120, 24);
			this.butSMSTemplateSetup.TabIndex = 251;
			this.butSMSTemplateSetup.Text = "SMS Template Setup";
			this.butSMSTemplateSetup.UseVisualStyleBackColor = true;
			this.butSMSTemplateSetup.Visible = false;
			this.butSMSTemplateSetup.Click += new System.EventHandler(this.butSMSTemplateSetup_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(152, 187);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(339, 16);
			this.label2.TabIndex = 252;
			this.label2.Text = "Edit templates for sending statement links through text.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label2.Visible = false;
			// 
			// FormEServicesSetup
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(494, 559);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butSMSTemplateSetup);
			this.Controls.Add(this.butMobileSettings);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupboxWebSched);
			this.Controls.Add(this.labelECR);
			this.Controls.Add(this.labelSignup);
			this.Controls.Add(this.butSignup);
			this.Controls.Add(this.label23);
			this.Controls.Add(this.butTextingServices);
			this.Controls.Add(this.butMassEmail);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butPatPortal);
			this.Controls.Add(this.butEConnector);
			this.Controls.Add(this.butMobileWeb);
			this.Controls.Add(this.butECR);
			this.Controls.Add(this.butMisc);
			this.Controls.Add(this.butEClipboard);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesSetup";
			this.Text = "eServices Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEServicesSetup2_FormClosing);
			this.Load += new System.EventHandler(this.FormEServicesSetup_Load);
			this.groupboxWebSched.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.Button butTextingServices;
		private UI.Button butPatPortal;
		private UI.Button butMobileWeb;
		private UI.Button butEConnector;
		private UI.Button butSignup;
		private UI.Button butMassEmail;
		private UI.Button butMisc;
		private UI.Button butEClipboard;
		private UI.Button butECR;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label labelECR;
		private System.Windows.Forms.Label labelSignup;
		private System.Windows.Forms.GroupBox groupboxWebSched;
		private UI.Button butNewPatient;
		private UI.Button butRecall;
		private UI.Button butAdvanced;
		private UI.Button butExistingPatient;
		private System.Windows.Forms.Label label1;
		private UI.Button butMobileSettings;
		private UI.Button butSMSTemplateSetup;
		private System.Windows.Forms.Label label2;
	}
}