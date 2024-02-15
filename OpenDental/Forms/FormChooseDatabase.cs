using OpenDentBusiness;
using System;
using System.Windows.Forms;
using DataConnectionBase;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental {
	public partial class FormChooseDatabase : FormODBase {
		///<summary></summary>
		public ChooseDatabaseInfo ChooseDatabaseInfo_;

		public FormChooseDatabase(ChooseDatabaseInfo chooseDatabaseInfo) {
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ChooseDatabaseInfo_=chooseDatabaseInfo;
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}

		private void FormChooseDatabase_Load(object sender,EventArgs e) {
			Logger.LogToPath("Load",LogPath.Startup,LogPhase.Start);
			FillForm();
			if(ODBuild.IsThinfinity()) {
				//Don't let the user choose another office's database (this window should never show anyway because NoShowOnStartup should be true)
				DisableAllExcept(butOK);
			}
			Logger.LogToPath("Load",LogPath.Startup,LogPhase.End);
			this.ActiveControl=comboComputerName;
		}

		private void FillForm() {
			Logger.LogToPath("FillForm",LogPath.Startup,LogPhase.Start);
			if(ChooseDatabaseInfo_.IsAccessedFromMainMenu) {
				if(ODEnvironment.IsCloudServer) {
					textUser.UseSystemPasswordChar=true;
				}
				comboComputerName.Enabled=false;
				comboComputerName.Text=DataConnection.GetServerName();
				comboDatabase.Enabled=false;
				comboDatabase.Text=DataConnection.GetDatabaseName();
				checkConnectServer.Enabled=false;
				textURI.ReadOnly=true;
			}
			listType.Items.Add("MySql");
			listType.Items.Add("Oracle");
			listType.SelectedIndex=0;
			checkConnectServer.Checked=false;
			groupDirect.Enabled=true;
			groupServer.Enabled=false;
			comboComputerName.Text=ChooseDatabaseInfo_.CentralConnectionCur.ServerName;
			comboDatabase.Text=ChooseDatabaseInfo_.CentralConnectionCur.DatabaseName;
			textUser.Text=ChooseDatabaseInfo_.CentralConnectionCur.MySqlUser;
			textPassword.Text=ChooseDatabaseInfo_.CentralConnectionCur.MySqlPassword;
			textPassword.PasswordChar=(textPassword.Text=="" ? default(char) : '*');
			textUser2.Text=ChooseDatabaseInfo_.CentralConnectionCur.OdUser;
			textPassword2.Text=ChooseDatabaseInfo_.CentralConnectionCur.OdPassword;
			textPEM.Text=ChooseDatabaseInfo_.CentralConnectionCur.SslCA??"";
			if(listType.Items.Count > 0 && listType.Items.Count >= 2) {
				listType.SelectedIndex=(int)ChooseDatabaseInfo_.DatabaseType;
			}
			textConnectionString.Text=ChooseDatabaseInfo_.ConnectionString;
			checkNoShow.Checked=(ChooseDatabaseInfo_.NoShow==YN.Yes);
			if(ChooseDatabaseInfo_.AllowAutoLogin) {
				checkBoxAutomaticLogin.Checked=ChooseDatabaseInfo_.CentralConnectionCur.IsAutomaticLogin;
			}
			else {
				checkBoxAutomaticLogin.Visible=false;
			}
			if(!string.IsNullOrEmpty(ChooseDatabaseInfo_.CentralConnectionCur.ServiceURI)) {
				checkConnectServer.Checked=true;
				groupDirect.Enabled=false;
				groupServer.Enabled=true;
				textURI.Text=ChooseDatabaseInfo_.CentralConnectionCur.ServiceURI;
				checkUsingEcw.Checked=ChooseDatabaseInfo_.CentralConnectionCur.WebServiceIsEcw;
				checkDynamicMode.Checked=false;
				checkDynamicMode.Enabled=false;
				textUser2.Select();
				return;
			}
			FillComboComputerNames();
			FillComboDatabases();
			if(textUser2.Text!="") {
				textPassword2.Select();
			}
			checkDynamicMode.Checked=ChooseDatabaseInfo_.UseDynamicMode;
			Logger.LogToPath("FillForm",LogPath.Startup,LogPhase.End);
		}

		private void SyncInfoWithUI() {
			ChooseDatabaseInfo_.CentralConnectionCur.ServerName=comboComputerName.Text;
			ChooseDatabaseInfo_.CentralConnectionCur.DatabaseName=comboDatabase.Text;
			ChooseDatabaseInfo_.CentralConnectionCur.MySqlUser=textUser.Text;
			ChooseDatabaseInfo_.CentralConnectionCur.MySqlPassword=textPassword.Text;
			ChooseDatabaseInfo_.NoShow=(checkNoShow.Checked ? YN.Yes : YN.No);
			ChooseDatabaseInfo_.CentralConnectionCur.ServiceURI=(checkConnectServer.Checked ? textURI.Text : "");
			ChooseDatabaseInfo_.CentralConnectionCur.OdUser=textUser2.Text;
			ChooseDatabaseInfo_.CentralConnectionCur.OdPassword=textPassword2.Text;
			ChooseDatabaseInfo_.CentralConnectionCur.WebServiceIsEcw=checkUsingEcw.Checked;
			ChooseDatabaseInfo_.DatabaseType=(listType.SelectedIndex==1 ? DatabaseType.Oracle : DatabaseType.MySql);
			ChooseDatabaseInfo_.CentralConnectionCur.SslCA=textPEM.Text;
			ChooseDatabaseInfo_.ConnectionString=textConnectionString.Text;
			//Only save AutoLogin if connecting to MT and AutoLogin box is checked.
			ChooseDatabaseInfo_.CentralConnectionCur.IsAutomaticLogin=(checkBoxAutomaticLogin.Checked && checkConnectServer.Checked);
			ChooseDatabaseInfo_.UseDynamicMode=checkDynamicMode.Checked;
		}

		private void FillComboComputerNames() {
			Logger.LogToPath("FillComboComputerNames",LogPath.Startup,LogPhase.Start);
			comboComputerName.Items.Clear();
			List<string> listComputerNames = CentralConnections.GetComputerNames();
			comboComputerName.Items.AddRange(listComputerNames.ToArray());
			Logger.LogToPath("FillComboComputerNames",LogPath.Startup,LogPhase.End);
		}

		private void FillComboDatabases() {
			Logger.LogToPath("FillComboDatabases",LogPath.Startup,LogPhase.Start);
			comboDatabase.Items.Clear();
			List<string> listNames=CentralConnections.GetDatabases(ChooseDatabaseInfo_.CentralConnectionCur,ChooseDatabaseInfo_.DatabaseType);
			comboDatabase.Items.AddRange(listNames.ToArray());
			Logger.LogToPath("FillComboDatabases",LogPath.Startup,LogPhase.End);
		}

		private void comboDatabase_DropDown(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			SyncInfoWithUI();
			FillComboDatabases();
			Cursor=Cursors.Default;
		}

		private void checkConnectServer_Click(object sender,EventArgs e) {
			if(checkConnectServer.Checked) {
				groupServer.Enabled=true;
				groupDirect.Enabled=false;
				checkDynamicMode.Checked=false;
				checkDynamicMode.Enabled=false;
				textPEM.Enabled=false;
			}
			else {
				groupServer.Enabled=false;
				groupDirect.Enabled=true;
				checkDynamicMode.Enabled=true;
				textPEM.Enabled=true;
			}
		}

		private void textPassword_TextChanged(object sender,EventArgs e) {
			if(textPassword.Text=="") {
				textPassword.PasswordChar=default(char);//if text is cleared, turn off password char mask
			}
		}

		private void textPassword_Leave(object sender,EventArgs e) {
			textPassword.PasswordChar=(textPassword.Text=="" ? default(char) : '*');//mask password if loaded from the config file
		}

		private void checkDynamicMode_CheckedChanged(object sender,EventArgs e) {
			if(checkDynamicMode.Checked) {
				checkNoShow.Checked=false;
				checkBoxAutomaticLogin.Checked=false;
			}
			checkNoShow.Enabled=!checkDynamicMode.Checked;
			checkBoxAutomaticLogin.Enabled=!checkDynamicMode.Checked;
		}

		///<summary>Attempts to connect to the database (via Middle Tier if necessary).
		///Returns true if connection settings are valid. Otherwise, false.</summary>
		private bool IsValidConnection() {
			SyncInfoWithUI();
			try {
				CentralConnections.TryToConnect(
					ChooseDatabaseInfo_.CentralConnectionCur,
					ChooseDatabaseInfo_.DatabaseType,
					ChooseDatabaseInfo_.ConnectionString,
					noShowOnStartup:(ChooseDatabaseInfo_.NoShow==YN.Yes),
					listAdminCompNames:ChooseDatabaseInfo_.ListAdminCompNames,
					useDynamicMode:ChooseDatabaseInfo_.UseDynamicMode,
					allowAutoLogin:ChooseDatabaseInfo_.AllowAutoLogin
					);
			}
			catch(ApplicationException aex) {
				//This error was thrown by Open Dental an has already been formatted to be shown directly to the user. E.g. invalid credentials were entered.
				MessageBox.Show(aex.Message);
				return false;
			}
			catch(Exception ex) {
				//No translation because there was a failure connecting to the database where translations are stored.
				FriendlyException.Show("Unable to connect to database.",ex);
				return false;
			}
			//A successful connection was made using the settings within the current choose database info.
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!IsValidConnection()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

	}

}