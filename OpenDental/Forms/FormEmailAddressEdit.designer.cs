using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEmailAddressEdit {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailAddressEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textSMTPserver = new System.Windows.Forms.TextBox();
			this.textSender = new System.Windows.Forms.TextBox();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textPort = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.checkSSL = new OpenDental.UI.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.groupOutgoing = new OpenDental.UI.GroupBoxOD();
			this.label13 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.groupIncoming = new OpenDental.UI.GroupBoxOD();
			this.label14 = new System.Windows.Forms.Label();
			this.textSMTPserverIncoming = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textPortIncoming = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.butRegisterCertificate = new OpenDental.UI.Button();
			this.groupUserod = new OpenDental.UI.GroupBoxOD();
			this.butPickUserod = new OpenDental.UI.Button();
			this.labelUserod = new System.Windows.Forms.Label();
			this.textUserod = new System.Windows.Forms.TextBox();
			this.textAccessToken = new System.Windows.Forms.TextBox();
			this.textRefreshToken = new System.Windows.Forms.TextBox();
			this.labelAccess = new System.Windows.Forms.Label();
			this.labelRefresh = new System.Windows.Forms.Label();
			this.groupAuth = new OpenDental.UI.GroupBoxOD();
			this.butGmailSettings = new OpenDental.UI.Button();
			this.butClearTokens = new OpenDental.UI.Button();
			this.butAuthGoogle = new System.Windows.Forms.Label();
			this.groupAuthentication = new OpenDental.UI.GroupBoxOD();
			this.butAuthMicrosoft = new System.Windows.Forms.Button();
			this.label15 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.webServiceService1 = new OpenDental.com.dentalxchange.webservices.WebServiceService();
			this.groupOutgoing.SuspendLayout();
			this.groupIncoming.SuspendLayout();
			this.groupUserod.SuspendLayout();
			this.groupAuth.SuspendLayout();
			this.groupAuthentication.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(658, 590);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(577, 590);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(87, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(173, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "Outgoing SMTP Server";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(81, 143);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(177, 20);
			this.label2.TabIndex = 3;
			this.label2.Text = "E-mail address of sender";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSMTPserver
			// 
			this.textSMTPserver.Location = new System.Drawing.Point(260, 20);
			this.textSMTPserver.Name = "textSMTPserver";
			this.textSMTPserver.Size = new System.Drawing.Size(187, 20);
			this.textSMTPserver.TabIndex = 1;
			// 
			// textSender
			// 
			this.textSender.Location = new System.Drawing.Point(260, 143);
			this.textSender.Name = "textSender";
			this.textSender.Size = new System.Drawing.Size(187, 20);
			this.textSender.TabIndex = 3;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(273, 12);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(188, 20);
			this.textUsername.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(116, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(151, 20);
			this.label3.TabIndex = 0;
			this.label3.Text = "Username or email address";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(110, 18);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(188, 20);
			this.textPassword.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(9, 18);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(95, 20);
			this.label4.TabIndex = 0;
			this.label4.Text = "Password";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPort
			// 
			this.textPort.Location = new System.Drawing.Point(260, 115);
			this.textPort.Name = "textPort";
			this.textPort.Size = new System.Drawing.Size(56, 20);
			this.textPort.TabIndex = 2;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(84, 115);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(174, 20);
			this.label5.TabIndex = 22;
			this.label5.Text = "Outgoing Port";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(322, 115);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(251, 20);
			this.label6.TabIndex = 0;
			this.label6.Text = "Usually 587.  Sometimes 25 or 465.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkSSL
			// 
			this.checkSSL.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSSL.Location = new System.Drawing.Point(165, 35);
			this.checkSSL.Name = "checkSSL";
			this.checkSSL.Size = new System.Drawing.Size(122, 17);
			this.checkSSL.TabIndex = 3;
			this.checkSSL.Text = "Use SSL";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(14, 590);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 8;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// groupOutgoing
			// 
			this.groupOutgoing.Controls.Add(this.label13);
			this.groupOutgoing.Controls.Add(this.label9);
			this.groupOutgoing.Controls.Add(this.textSMTPserver);
			this.groupOutgoing.Controls.Add(this.label1);
			this.groupOutgoing.Controls.Add(this.label2);
			this.groupOutgoing.Controls.Add(this.label6);
			this.groupOutgoing.Controls.Add(this.textSender);
			this.groupOutgoing.Controls.Add(this.textPort);
			this.groupOutgoing.Controls.Add(this.label5);
			this.groupOutgoing.Location = new System.Drawing.Point(14, 234);
			this.groupOutgoing.Name = "groupOutgoing";
			this.groupOutgoing.Size = new System.Drawing.Size(719, 180);
			this.groupOutgoing.TabIndex = 4;
			this.groupOutgoing.Text = "Outgoing Email Settings";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(262, 43);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(198, 69);
			this.label13.TabIndex = 0;
			this.label13.Text = "smtp.comcast.net\r\nmailhost.mycompany.com \r\nmail.mycompany.com\r\nsmtp.gmail.com\r\nor" +
    " similar...";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(451, 142);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(159, 20);
			this.label9.TabIndex = 0;
			this.label9.Text = "(not used in encrypted email)";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupIncoming
			// 
			this.groupIncoming.Controls.Add(this.label14);
			this.groupIncoming.Controls.Add(this.textSMTPserverIncoming);
			this.groupIncoming.Controls.Add(this.label8);
			this.groupIncoming.Controls.Add(this.label10);
			this.groupIncoming.Controls.Add(this.textPortIncoming);
			this.groupIncoming.Controls.Add(this.label11);
			this.groupIncoming.Location = new System.Drawing.Point(14, 420);
			this.groupIncoming.Name = "groupIncoming";
			this.groupIncoming.Size = new System.Drawing.Size(719, 116);
			this.groupIncoming.TabIndex = 5;
			this.groupIncoming.Text = "Incoming Email Settings";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(262, 38);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(198, 43);
			this.label14.TabIndex = 0;
			this.label14.Text = "pop.secureserver.net\r\npop.gmail.com\r\nor similar...";
			// 
			// textSMTPserverIncoming
			// 
			this.textSMTPserverIncoming.Location = new System.Drawing.Point(260, 15);
			this.textSMTPserverIncoming.Name = "textSMTPserverIncoming";
			this.textSMTPserverIncoming.Size = new System.Drawing.Size(187, 20);
			this.textSMTPserverIncoming.TabIndex = 1;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(87, 15);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(173, 20);
			this.label8.TabIndex = 0;
			this.label8.Text = "Incoming POP3 Server";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(322, 84);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(251, 20);
			this.label10.TabIndex = 0;
			this.label10.Text = "Usually 110.  Sometimes 995.";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPortIncoming
			// 
			this.textPortIncoming.Location = new System.Drawing.Point(260, 84);
			this.textPortIncoming.Name = "textPortIncoming";
			this.textPortIncoming.Size = new System.Drawing.Size(56, 20);
			this.textPortIncoming.TabIndex = 2;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(84, 84);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(174, 20);
			this.label11.TabIndex = 0;
			this.label11.Text = "Incoming Port";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRegisterCertificate
			// 
			this.butRegisterCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRegisterCertificate.Location = new System.Drawing.Point(346, 590);
			this.butRegisterCertificate.Name = "butRegisterCertificate";
			this.butRegisterCertificate.Size = new System.Drawing.Size(122, 24);
			this.butRegisterCertificate.TabIndex = 9;
			this.butRegisterCertificate.Text = "Certificate";
			this.butRegisterCertificate.Click += new System.EventHandler(this.butRegisterCertificate_Click);
			// 
			// groupUserod
			// 
			this.groupUserod.Controls.Add(this.butPickUserod);
			this.groupUserod.Controls.Add(this.labelUserod);
			this.groupUserod.Controls.Add(this.textUserod);
			this.groupUserod.Location = new System.Drawing.Point(15, 542);
			this.groupUserod.Name = "groupUserod";
			this.groupUserod.Size = new System.Drawing.Size(718, 42);
			this.groupUserod.TabIndex = 10;
			this.groupUserod.Text = "User";
			// 
			// butPickUserod
			// 
			this.butPickUserod.Location = new System.Drawing.Point(483, 15);
			this.butPickUserod.Name = "butPickUserod";
			this.butPickUserod.Size = new System.Drawing.Size(26, 23);
			this.butPickUserod.TabIndex = 4;
			this.butPickUserod.Text = "...";
			this.butPickUserod.UseVisualStyleBackColor = true;
			this.butPickUserod.Visible = false;
			this.butPickUserod.Click += new System.EventHandler(this.butPickUserod_Click);
			// 
			// labelUserod
			// 
			this.labelUserod.Location = new System.Drawing.Point(85, 16);
			this.labelUserod.Name = "labelUserod";
			this.labelUserod.Size = new System.Drawing.Size(173, 20);
			this.labelUserod.TabIndex = 3;
			this.labelUserod.Text = "User";
			this.labelUserod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUserod
			// 
			this.textUserod.Location = new System.Drawing.Point(258, 16);
			this.textUserod.Name = "textUserod";
			this.textUserod.ReadOnly = true;
			this.textUserod.Size = new System.Drawing.Size(218, 20);
			this.textUserod.TabIndex = 0;
			// 
			// textAccessToken
			// 
			this.textAccessToken.Location = new System.Drawing.Point(129, 32);
			this.textAccessToken.Name = "textAccessToken";
			this.textAccessToken.ReadOnly = true;
			this.textAccessToken.Size = new System.Drawing.Size(115, 20);
			this.textAccessToken.TabIndex = 11;
			// 
			// textRefreshToken
			// 
			this.textRefreshToken.Location = new System.Drawing.Point(129, 60);
			this.textRefreshToken.Name = "textRefreshToken";
			this.textRefreshToken.ReadOnly = true;
			this.textRefreshToken.Size = new System.Drawing.Size(115, 20);
			this.textRefreshToken.TabIndex = 12;
			// 
			// labelAccess
			// 
			this.labelAccess.Location = new System.Drawing.Point(10, 32);
			this.labelAccess.Name = "labelAccess";
			this.labelAccess.Size = new System.Drawing.Size(116, 20);
			this.labelAccess.TabIndex = 14;
			this.labelAccess.Text = "Access Token";
			this.labelAccess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelRefresh
			// 
			this.labelRefresh.Location = new System.Drawing.Point(11, 60);
			this.labelRefresh.Name = "labelRefresh";
			this.labelRefresh.Size = new System.Drawing.Size(115, 20);
			this.labelRefresh.TabIndex = 15;
			this.labelRefresh.Text = "Refresh Token";
			this.labelRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupAuth
			// 
			this.groupAuth.Controls.Add(this.butGmailSettings);
			this.groupAuth.Controls.Add(this.textAccessToken);
			this.groupAuth.Controls.Add(this.butClearTokens);
			this.groupAuth.Controls.Add(this.labelAccess);
			this.groupAuth.Controls.Add(this.textRefreshToken);
			this.groupAuth.Controls.Add(this.labelRefresh);
			this.groupAuth.Location = new System.Drawing.Point(381, 54);
			this.groupAuth.Name = "groupAuth";
			this.groupAuth.Size = new System.Drawing.Size(352, 174);
			this.groupAuth.TabIndex = 16;
			this.groupAuth.Text = "Gmail Authorization";
			// 
			// butGmailSettings
			// 
			this.butGmailSettings.Location = new System.Drawing.Point(129, 136);
			this.butGmailSettings.Name = "butGmailSettings";
			this.butGmailSettings.Size = new System.Drawing.Size(115, 23);
			this.butGmailSettings.TabIndex = 18;
			this.butGmailSettings.Text = "Gmail Settings";
			this.butGmailSettings.UseVisualStyleBackColor = true;
			this.butGmailSettings.Click += new System.EventHandler(this.butGmailSettings_Click);
			// 
			// butClearTokens
			// 
			this.butClearTokens.Location = new System.Drawing.Point(129, 89);
			this.butClearTokens.Name = "butClearTokens";
			this.butClearTokens.Size = new System.Drawing.Size(115, 23);
			this.butClearTokens.TabIndex = 16;
			this.butClearTokens.Text = "Clear Tokens";
			this.butClearTokens.UseVisualStyleBackColor = true;
			this.butClearTokens.Click += new System.EventHandler(this.butClearTokens_Click);
			// 
			// butAuthGoogle
			// 
			this.butAuthGoogle.Image = global::OpenDental.Properties.Resources.google_signin_normal;
			this.butAuthGoogle.Location = new System.Drawing.Point(107, 76);
			this.butAuthGoogle.Name = "butAuthGoogle";
			this.butAuthGoogle.Size = new System.Drawing.Size(191, 46);
			this.butAuthGoogle.TabIndex = 17;
			this.butAuthGoogle.Click += new System.EventHandler(this.butAuthGoogle_Click);
			this.butAuthGoogle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butAuthGoogle_MouseDown);
			this.butAuthGoogle.MouseEnter += new System.EventHandler(this.butAuthGoogle_MouseEnter);
			this.butAuthGoogle.MouseLeave += new System.EventHandler(this.butAuthGoogle_MouseLeave);
			this.butAuthGoogle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butAuthGoogle_MouseUp);
			// 
			// groupAuthentication
			// 
			this.groupAuthentication.Controls.Add(this.butAuthMicrosoft);
			this.groupAuthentication.Controls.Add(this.label15);
			this.groupAuthentication.Controls.Add(this.label12);
			this.groupAuthentication.Controls.Add(this.textPassword);
			this.groupAuthentication.Controls.Add(this.label4);
			this.groupAuthentication.Controls.Add(this.butAuthGoogle);
			this.groupAuthentication.Controls.Add(this.label7);
			this.groupAuthentication.Location = new System.Drawing.Point(14, 54);
			this.groupAuthentication.Name = "groupAuthentication";
			this.groupAuthentication.Size = new System.Drawing.Size(345, 174);
			this.groupAuthentication.TabIndex = 19;
			this.groupAuthentication.Text = "Email Authentication";
			// 
			// butAuthMicrosoft
			// 
			this.butAuthMicrosoft.FlatAppearance.BorderSize = 0;
			this.butAuthMicrosoft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.butAuthMicrosoft.Image = ((System.Drawing.Image)(resources.GetObject("butAuthMicrosoft.Image")));
			this.butAuthMicrosoft.Location = new System.Drawing.Point(107, 125);
			this.butAuthMicrosoft.Name = "butAuthMicrosoft";
			this.butAuthMicrosoft.Size = new System.Drawing.Size(190, 45);
			this.butAuthMicrosoft.TabIndex = 22;
			this.butAuthMicrosoft.UseVisualStyleBackColor = true;
			this.butAuthMicrosoft.Click += new System.EventHandler(this.butAuthMicrosoft_Click);
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(9, 125);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(95, 43);
			this.label15.TabIndex = 21;
			this.label15.Text = "Required for Microsoft addresses";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(9, 79);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(95, 43);
			this.label12.TabIndex = 19;
			this.label12.Text = "Required for Gmail addresses";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(110, 41);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(188, 31);
			this.label7.TabIndex = 18;
			this.label7.Text = "or";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// webServiceService1
			// 
			this.webServiceService1.Credentials = null;
			this.webServiceService1.Url = "https://webservices.dentalxchange.com/dws/services/dciservice.svl";
			this.webServiceService1.UseDefaultCredentials = false;
			// 
			// FormEmailAddressEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(749, 630);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textUsername);
			this.Controls.Add(this.groupAuthentication);
			this.Controls.Add(this.groupAuth);
			this.Controls.Add(this.groupUserod);
			this.Controls.Add(this.butRegisterCertificate);
			this.Controls.Add(this.groupIncoming);
			this.Controls.Add(this.groupOutgoing);
			this.Controls.Add(this.checkSSL);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEmailAddressEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Email Address";
			this.Load += new System.EventHandler(this.FormEmailAddress_Load);
			this.groupOutgoing.ResumeLayout(false);
			this.groupOutgoing.PerformLayout();
			this.groupIncoming.ResumeLayout(false);
			this.groupIncoming.PerformLayout();
			this.groupUserod.ResumeLayout(false);
			this.groupUserod.PerformLayout();
			this.groupAuth.ResumeLayout(false);
			this.groupAuth.PerformLayout();
			this.groupAuthentication.ResumeLayout(false);
			this.groupAuthentication.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textSMTPserver;
		private System.Windows.Forms.TextBox textSender;
		private TextBox textUsername;
		private Label label3;
		private TextBox textPassword;
		private Label label4;
		private TextBox textPort;
		private Label label5;
		private Label label6;
		private OpenDental.UI.CheckBox checkSSL;
		private UI.Button butDelete;
		private OpenDental.UI.GroupBoxOD groupOutgoing;
		private OpenDental.UI.GroupBoxOD groupIncoming;
		private TextBox textSMTPserverIncoming;
		private Label label8;
		private Label label10;
		private TextBox textPortIncoming;
		private Label label11;
		private Label label9;
		private UI.Button butRegisterCertificate;
		private Label label13;
		private Label label14;
		private OpenDental.UI.GroupBoxOD groupUserod;
		private UI.Button butPickUserod;
		private Label labelUserod;
		private TextBox textUserod;
		private TextBox textAccessToken;
		private TextBox textRefreshToken;
		private Label labelAccess;
		private Label labelRefresh;
		private OpenDental.UI.GroupBoxOD groupAuth;
		private UI.Button butClearTokens;
		private Label butAuthGoogle;
		private OpenDental.UI.GroupBoxOD groupAuthentication;
		private Label label7;
		private Label label12;
		private UI.Button butGmailSettings;
		private com.dentalxchange.webservices.WebServiceService webServiceService1;
		private Label label15;
		private Button butAuthMicrosoft;
	}
}
