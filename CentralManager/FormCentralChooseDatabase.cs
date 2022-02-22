using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using CentralManager;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public class FormCentralChooseDatabase:System.Windows.Forms.Form {
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.ComponentModel.Container components = null;
		private GroupBox groupServer;
		private Label label9;
		private Label label6;
		private TextBox textUser2;
		private TextBox textPassword2;
		private Label label10;
		private Label label11;
		private CheckBox checkAutoLogin;
		private string _username;
		private string _password;
		///<summary>When silently running GetConfig() without showing UI, this gets set to true if either NoShowOnStartup or UsingEcw is found in config file.</summary>
		public YN NoShow;
		private TextBox textURI;
		public string OdUser;
		///<summary>Only used by Ecw.</summary>
		public string OdPassHash;
		///<summary>This is used when selecting File>Choose Database.  It will behave slightly differently.</summary>
		public bool IsAccessedFromMainMenu;
		public bool UsingAutoLogin=false;
		public YN WebServiceIsEcw;
		public string OdPassword;
		public string ServerName;
		public string DatabaseName;
		public string MySqlUser;
		public string MySqlPassword;

		///<summary></summary>
		public FormCentralChooseDatabase(string webServiceUri) {
			InitializeComponent();
			textURI.Text=webServiceUri;
			Lan.F(this);
		}

		///<summary></summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralChooseDatabase));
			this.groupServer = new System.Windows.Forms.GroupBox();
			this.textURI = new System.Windows.Forms.TextBox();
			this.textUser2 = new System.Windows.Forms.TextBox();
			this.textPassword2 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkAutoLogin = new System.Windows.Forms.CheckBox();
			this.groupServer.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupServer
			// 
			this.groupServer.Controls.Add(this.textURI);
			this.groupServer.Controls.Add(this.textUser2);
			this.groupServer.Controls.Add(this.textPassword2);
			this.groupServer.Controls.Add(this.label10);
			this.groupServer.Controls.Add(this.label11);
			this.groupServer.Controls.Add(this.label9);
			this.groupServer.Controls.Add(this.label6);
			this.groupServer.Location = new System.Drawing.Point(12,12);
			this.groupServer.Name = "groupServer";
			this.groupServer.Size = new System.Drawing.Size(336,182);
			this.groupServer.TabIndex = 2;
			this.groupServer.TabStop = false;
			this.groupServer.Text = "Connect to Middle Tier";
			// 
			// textURI
			// 
			this.textURI.Location = new System.Drawing.Point(13,65);
			this.textURI.Name = "textURI";
			this.textURI.Size = new System.Drawing.Size(309,20);
			this.textURI.TabIndex = 7;
			// 
			// textUser2
			// 
			this.textUser2.Location = new System.Drawing.Point(13,108);
			this.textUser2.Name = "textUser2";
			this.textUser2.Size = new System.Drawing.Size(309,20);
			this.textUser2.TabIndex = 8;
			// 
			// textPassword2
			// 
			this.textPassword2.Location = new System.Drawing.Point(13,149);
			this.textPassword2.Name = "textPassword2";
			this.textPassword2.PasswordChar = '*';
			this.textPassword2.Size = new System.Drawing.Size(309,20);
			this.textPassword2.TabIndex = 9;
			this.textPassword2.UseSystemPasswordChar = true;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(11,130);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(281,18);
			this.label10.TabIndex = 11;
			this.label10.Text = "Password";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(11,89);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(295,18);
			this.label11.TabIndex = 14;
			this.label11.Text = "Open Dental User (not MySQL user)";
			this.label11.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(10,44);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(283,18);
			this.label9.TabIndex = 9;
			this.label9.Text = "URI";
			this.label9.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(9,25);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(297,18);
			this.label6.TabIndex = 0;
			this.label6.Text = "Read the manual to learn how to install the middle tier.";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(273,201);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(192,201);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkAutoLogin
			// 
			this.checkAutoLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkAutoLogin.AutoSize = true;
			this.checkAutoLogin.Location = new System.Drawing.Point(24,206);
			this.checkAutoLogin.Name = "checkAutoLogin";
			this.checkAutoLogin.Size = new System.Drawing.Size(136,17);
			this.checkAutoLogin.TabIndex = 14;
			this.checkAutoLogin.Text = "Log me in automatically";
			this.checkAutoLogin.UseVisualStyleBackColor = true;
			// 
			// FormCentralChooseDatabase
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(360,238);
			this.Controls.Add(this.checkAutoLogin);
			this.Controls.Add(this.groupServer);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormCentralChooseDatabase";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Choose Database";
			this.groupServer.ResumeLayout(false);
			this.groupServer.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		///<summary>Attempts to connect to the Middle Tier Server with passed in credentials.  </summary>
		public bool CanConnectToMTServer(string username,string password) {
			string originalURI=RemotingClient.ServerURI;
			RemotingClient.ServerURI=textURI.Text;
			try {
				Userod user=Security.LogInWeb(username,password,"",Application.ProductVersion,false);
				Security.CurUser=user;
				Security.PasswordTyped=password;
				RemotingClient.RemotingRole=RemotingRole.ClientWeb;
			}
			catch(Exception ex) {
				RemotingClient.ServerURI=originalURI;
				MessageBox.Show(ex.Message);
				return false;
			}
			return true;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			_username=textUser2.Text;
			_password=textPassword2.Text;
			if(!CanConnectToMTServer(_username,_password)) {
				return;
			}
			if(checkAutoLogin.Checked) {
				try {
					CentralConfigHelper.EnableAutoLogin(textURI.Text,_username,_password);
					UsingAutoLogin=true;
				}
				catch(Exception) {
					OpenDental.MessageBox.Show("Unable to enable automatic log in.");
				}
			}
			DialogResult=DialogResult.OK;
		}
		
		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
