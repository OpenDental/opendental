using CodeBase;
using OpenDental;
using OpenDentBusiness;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CentralManager {
	public partial class FormCentralUserSettings:Form {
		private string _uri;
		public bool UsingAutoLogin;


		public FormCentralUserSettings(string uri,bool usingAutoLogin) {
			InitializeComponent();
			_uri=uri;
			UsingAutoLogin=usingAutoLogin;
		}

		private void FormCentralUserSettings_Load(object sender,EventArgs e) {
			textUserName.Text=Security.CurUser.UserName;
			checkAutoLogin.Checked=UsingAutoLogin;  //Appears as checked if user is logging in automatically
			if(!UsingAutoLogin) {	//otherwise box is disabled
				checkAutoLogin.Enabled=false;
				labelAutoLogin.Visible=true;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!checkAutoLogin.Checked && UsingAutoLogin) { //user wants to stop logging in automatically
				try {
					CentralConfigHelper.DisableAutoLogin(_uri,Security.CurUser.UserName);
					MsgBox.Show("Login credentials removed successfully.");
					checkAutoLogin.Checked=true;
					UsingAutoLogin=false;
				}
				catch(Exception ex) {
					ex.DoNothing();
					MsgBox.Show("Unable to remove login credentials. Try running as Administrator.");
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
