namespace OpenDental{
	partial class FormEServicesPatientPortal {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesPatientPortal));
			this.label37 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.comboPPClinicUrl = new OpenDental.UI.ComboBoxClinicPicker();
			this.label39 = new System.Windows.Forms.Label();
			this.radioPatientPortalPayment = new System.Windows.Forms.RadioButton();
			this.radioPatientPortalLogin = new System.Windows.Forms.RadioButton();
			this.butCopyToClipboard = new OpenDental.UI.Button();
			this.textHostedUrlPortal = new System.Windows.Forms.TextBox();
			this.butNavigateTo = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBoxNotification = new System.Windows.Forms.GroupBox();
			this.butEditWebMailNotificationBody = new OpenDental.UI.Button();
			this.browserWebMailNotificatonBody = new System.Windows.Forms.WebBrowser();
			this.label9 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textBoxNotificationSubject = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butCustomUrl = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textPatientFacingUrlPortal = new System.Windows.Forms.TextBox();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox4.SuspendLayout();
			this.groupBoxNotification.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(0, 0);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(100, 23);
			this.label37.TabIndex = 0;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(392, 625);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 500;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12,591);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(536,32);
			this.label5.TabIndex = 502;
			this.label5.Text = "All setup for Patient Portal Invites is now done in eServices Automated Messaging" +
    ".";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.comboPPClinicUrl);
			this.groupBox4.Controls.Add(this.label39);
			this.groupBox4.Controls.Add(this.radioPatientPortalPayment);
			this.groupBox4.Controls.Add(this.radioPatientPortalLogin);
			this.groupBox4.Controls.Add(this.butCopyToClipboard);
			this.groupBox4.Controls.Add(this.textHostedUrlPortal);
			this.groupBox4.Controls.Add(this.butNavigateTo);
			this.groupBox4.Controls.Add(this.label2);
			this.groupBox4.Controls.Add(this.label3);
			this.groupBox4.Location = new System.Drawing.Point(12, 127);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(536, 203);
			this.groupBox4.TabIndex = 51;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Construct Hosted URL";
			// 
			// comboPPClinicUrl
			// 
			this.comboPPClinicUrl.HqDescription = "None";
			this.comboPPClinicUrl.IncludeUnassigned = true;
			this.comboPPClinicUrl.Location = new System.Drawing.Point(88, 168);
			this.comboPPClinicUrl.Name = "comboPPClinicUrl";
			this.comboPPClinicUrl.Size = new System.Drawing.Size(194, 21);
			this.comboPPClinicUrl.TabIndex = 269;
			this.comboPPClinicUrl.SelectedIndexChanged += new System.EventHandler(this.comboPPClinicUrl_SelectedIndexChanged);
			// 
			// label39
			// 
			this.label39.Location = new System.Drawing.Point(15, 143);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(104, 17);
			this.label39.TabIndex = 277;
			this.label39.Text = "Destination";
			this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioPatientPortalPayment
			// 
			this.radioPatientPortalPayment.BackColor = System.Drawing.Color.Transparent;
			this.radioPatientPortalPayment.Location = new System.Drawing.Point(259, 143);
			this.radioPatientPortalPayment.Name = "radioPatientPortalPayment";
			this.radioPatientPortalPayment.Size = new System.Drawing.Size(163, 17);
			this.radioPatientPortalPayment.TabIndex = 275;
			this.radioPatientPortalPayment.Text = "Make Payment";
			this.radioPatientPortalPayment.UseVisualStyleBackColor = false;
			this.radioPatientPortalPayment.CheckedChanged += new System.EventHandler(this.radioPatientPortal_CheckedChanged);
			// 
			// radioPatientPortalLogin
			// 
			this.radioPatientPortalLogin.Checked = true;
			this.radioPatientPortalLogin.Location = new System.Drawing.Point(125, 143);
			this.radioPatientPortalLogin.Name = "radioPatientPortalLogin";
			this.radioPatientPortalLogin.Size = new System.Drawing.Size(128, 17);
			this.radioPatientPortalLogin.TabIndex = 274;
			this.radioPatientPortalLogin.TabStop = true;
			this.radioPatientPortalLogin.Text = "Login Page";
			this.radioPatientPortalLogin.UseVisualStyleBackColor = true;
			this.radioPatientPortalLogin.CheckedChanged += new System.EventHandler(this.radioPatientPortal_CheckedChanged);
			// 
			// butCopyToClipboard
			// 
			this.butCopyToClipboard.Location = new System.Drawing.Point(245, 107);
			this.butCopyToClipboard.Name = "butCopyToClipboard";
			this.butCopyToClipboard.Size = new System.Drawing.Size(115, 24);
			this.butCopyToClipboard.TabIndex = 269;
			this.butCopyToClipboard.Text = "Copy to Clipboard";
			this.butCopyToClipboard.Click += new System.EventHandler(this.butCopyToClipboard_Click);
			// 
			// textHostedUrlPortal
			// 
			this.textHostedUrlPortal.Location = new System.Drawing.Point(122, 81);
			this.textHostedUrlPortal.Name = "textHostedUrlPortal";
			this.textHostedUrlPortal.ReadOnly = true;
			this.textHostedUrlPortal.Size = new System.Drawing.Size(397, 20);
			this.textHostedUrlPortal.TabIndex = 46;
			// 
			// butNavigateTo
			// 
			this.butNavigateTo.Location = new System.Drawing.Point(122, 107);
			this.butNavigateTo.Name = "butNavigateTo";
			this.butNavigateTo.Size = new System.Drawing.Size(117, 24);
			this.butNavigateTo.TabIndex = 270;
			this.butNavigateTo.Text = "Navigate to URL";
			this.butNavigateTo.Click += new System.EventHandler(this.butNavigateTo_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 82);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(110, 17);
			this.label2.TabIndex = 44;
			this.label2.Text = "Hosted URL";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(15, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(504, 51);
			this.label3.TabIndex = 45;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// groupBoxNotification
			// 
			this.groupBoxNotification.Controls.Add(this.butEditWebMailNotificationBody);
			this.groupBoxNotification.Controls.Add(this.browserWebMailNotificatonBody);
			this.groupBoxNotification.Controls.Add(this.label9);
			this.groupBoxNotification.Controls.Add(this.label7);
			this.groupBoxNotification.Controls.Add(this.textBoxNotificationSubject);
			this.groupBoxNotification.Controls.Add(this.label6);
			this.groupBoxNotification.Controls.Add(this.label4);
			this.groupBoxNotification.Location = new System.Drawing.Point(12, 336);
			this.groupBoxNotification.Name = "groupBoxNotification";
			this.groupBoxNotification.Size = new System.Drawing.Size(536, 252);
			this.groupBoxNotification.TabIndex = 48;
			this.groupBoxNotification.TabStop = false;
			this.groupBoxNotification.Text = "Notification Email";
			// 
			// butEditWebMailNotificationBody
			// 
			this.butEditWebMailNotificationBody.Location = new System.Drawing.Point(449, 220);
			this.butEditWebMailNotificationBody.Name = "butEditWebMailNotificationBody";
			this.butEditWebMailNotificationBody.Size = new System.Drawing.Size(70, 20);
			this.butEditWebMailNotificationBody.TabIndex = 320;
			this.butEditWebMailNotificationBody.Text = "Edit";
			this.butEditWebMailNotificationBody.UseVisualStyleBackColor = true;
			this.butEditWebMailNotificationBody.Click += new System.EventHandler(this.butEditWebMailNotificationBody_Click);
			// 
			// browserWebMailNotificatonBody
			// 
			this.browserWebMailNotificatonBody.AllowWebBrowserDrop = false;
			this.browserWebMailNotificatonBody.Location = new System.Drawing.Point(93, 120);
			this.browserWebMailNotificatonBody.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserWebMailNotificatonBody.Name = "browserWebMailNotificatonBody";
			this.browserWebMailNotificatonBody.Size = new System.Drawing.Size(426, 94);
			this.browserWebMailNotificatonBody.TabIndex = 319;
			this.browserWebMailNotificatonBody.WebBrowserShortcutsEnabled = false;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.Location = new System.Drawing.Point(15, 16);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(504, 56);
			this.label9.TabIndex = 52;
			this.label9.Text = resources.GetString("label9.Text");
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(90, 100);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(440, 15);
			this.label7.TabIndex = 48;
			this.label7.Text = "[URL] will be replaced with the value of \'Patient Facing URL\' as entered above.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxNotificationSubject
			// 
			this.textBoxNotificationSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxNotificationSubject.Location = new System.Drawing.Point(93, 75);
			this.textBoxNotificationSubject.Name = "textBoxNotificationSubject";
			this.textBoxNotificationSubject.Size = new System.Drawing.Size(426, 20);
			this.textBoxNotificationSubject.TabIndex = 45;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 120);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(75, 17);
			this.label6.TabIndex = 47;
			this.label6.Text = "Body";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(9, 76);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 17);
			this.label4.TabIndex = 44;
			this.label4.Text = "Subject";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butCustomUrl);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.textPatientFacingUrlPortal);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(536, 109);
			this.groupBox1.TabIndex = 49;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Patient Facing URL";
			// 
			// butCustomUrl
			// 
			this.butCustomUrl.Location = new System.Drawing.Point(18, 79);
			this.butCustomUrl.Name = "butCustomUrl";
			this.butCustomUrl.Size = new System.Drawing.Size(88, 24);
			this.butCustomUrl.TabIndex = 54;
			this.butCustomUrl.Text = "Customize URL";
			this.butCustomUrl.UseVisualStyleBackColor = true;
			this.butCustomUrl.Click += new System.EventHandler(this.butCustomUrl_Click);
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label1.Location = new System.Drawing.Point(15, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(504, 30);
			this.label1.TabIndex = 51;
			this.label1.Text = "This will be the link that patients will use to reach your office\'s patient porta" +
    "l. This is also the URL that will be on the printout given to patients.";
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label8.Location = new System.Drawing.Point(-143, 129);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(129, 17);
			this.label8.TabIndex = 52;
			this.label8.Text = "Patient Facing URL";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientFacingUrlPortal
			// 
			this.textPatientFacingUrlPortal.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textPatientFacingUrlPortal.Enabled = false;
			this.textPatientFacingUrlPortal.Location = new System.Drawing.Point(18, 56);
			this.textPatientFacingUrlPortal.Name = "textPatientFacingUrlPortal";
			this.textPatientFacingUrlPortal.Size = new System.Drawing.Size(501, 20);
			this.textPatientFacingUrlPortal.TabIndex = 50;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(473,625);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 501;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEServicesPatientPortal
			// 
			this.ClientSize = new System.Drawing.Size(562, 663);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBoxNotification);
			this.Controls.Add(this.groupBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesPatientPortal";
			this.Text = "eServices Patient Portal";
			this.Load += new System.EventHandler(this.FormEServicesPatientPortal_Load);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBoxNotification.ResumeLayout(false);
			this.groupBoxNotification.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TextBox textBoxNotificationSubject;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBoxNotification;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textPatientFacingUrlPortal;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label9;
		private UI.Button butOK;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox textHostedUrlPortal;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private UI.Button butCopyToClipboard;
		private UI.Button butNavigateTo;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.RadioButton radioPatientPortalPayment;
		private System.Windows.Forms.RadioButton radioPatientPortalLogin;
		private UI.ComboBoxClinicPicker comboPPClinicUrl;
		private UI.Button butEditWebMailNotificationBody;
		private System.Windows.Forms.WebBrowser browserWebMailNotificatonBody;
		private UI.Button butCancel;
		private UI.Button butCustomUrl;
		private System.Windows.Forms.Label label5;
	}
}