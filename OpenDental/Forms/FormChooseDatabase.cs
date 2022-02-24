using OpenDentBusiness;
using System;
using System.Windows.Forms;
using DataConnectionBase;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental {
	public partial class FormChooseDatabase : FormODBase {
		///<summary></summary>
		public ChooseDatabaseInfo Model;

		public FormChooseDatabase(ChooseDatabaseInfo model) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			Model=model;
		}

		private void FormChooseDatabase_Load(object sender,EventArgs e) {
			FillForm();
			if(ODBuild.IsWeb()) {
				//Don't let the user choose another office's database (this window should never show anyway because NoShowOnStartup should be true)
				DisableAllExcept(butOK,butCancel);
			}
		}

		private void FillForm() {
			if(Model.IsAccessedFromMainMenu) {
				if(ODBuild.IsWeb()) {
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
			comboComputerName.Text=Model.CentralConnectionCur.ServerName;
			comboDatabase.Text=Model.CentralConnectionCur.DatabaseName;
			textUser.Text=Model.CentralConnectionCur.MySqlUser;
			textPassword.Text=Model.CentralConnectionCur.MySqlPassword;
			textPassword.PasswordChar=(textPassword.Text=="" ? default(char) : '*');
			textUser2.Text=Model.CentralConnectionCur.OdUser;
			textPassword2.Text=Model.CentralConnectionCur.OdPassword;
			if(listType.Items.Count > 0 && listType.Items.Count >= 2) {
				listType.SelectedIndex=(int)Model.DbType;
			}
			textConnectionString.Text=Model.ConnectionString;
			checkNoShow.Checked=(Model.NoShow==YN.Yes);
			if(Model.AllowAutoLogin) {
				checkBoxAutomaticLogin.Checked=Model.CentralConnectionCur.IsAutomaticLogin;
			}
			else {
				checkBoxAutomaticLogin.Visible=false;
			}
			if(!string.IsNullOrEmpty(Model.CentralConnectionCur.ServiceURI)) {
				checkConnectServer.Checked=true;
				groupDirect.Enabled=false;
				groupServer.Enabled=true;
				textURI.Text=Model.CentralConnectionCur.ServiceURI;
				checkUsingEcw.Checked=Model.CentralConnectionCur.WebServiceIsEcw;
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
			checkDynamicMode.Checked=Model.UseDynamicMode;
		}

		private void SyncModelWithUI() {
			Model.CentralConnectionCur.ServerName=comboComputerName.Text;
			Model.CentralConnectionCur.DatabaseName=comboDatabase.Text;
			Model.CentralConnectionCur.MySqlUser=textUser.Text;
			Model.CentralConnectionCur.MySqlPassword=textPassword.Text;
			Model.NoShow=(checkNoShow.Checked ? YN.Yes : YN.No);
			Model.CentralConnectionCur.ServiceURI=(checkConnectServer.Checked ? textURI.Text : "");
			Model.CentralConnectionCur.OdUser=textUser2.Text;
			Model.CentralConnectionCur.OdPassword=textPassword2.Text;
			Model.CentralConnectionCur.WebServiceIsEcw=checkUsingEcw.Checked;
			Model.DbType=(listType.SelectedIndex==1 ? DatabaseType.Oracle : DatabaseType.MySql);
			Model.ConnectionString=textConnectionString.Text;
			//Only save AutoLogin if connecting to MT and AutoLogin box is checked.
			Model.CentralConnectionCur.IsAutomaticLogin=(checkBoxAutomaticLogin.Checked && checkConnectServer.Checked);
			Model.UseDynamicMode=checkDynamicMode.Checked;
		}

		private void FillComboComputerNames() {
			comboComputerName.Items.Clear();
			comboComputerName.Items.AddRange(CentralConnections.GetComputerNames());
		}

		private void FillComboDatabases() {
			comboDatabase.Items.Clear();
			comboDatabase.Items.AddRange(CentralConnections.GetDatabases(Model.CentralConnectionCur,Model.DbType));
		}

		private void comboDatabase_DropDown(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			SyncModelWithUI();
			FillComboDatabases();
			Cursor=Cursors.Default;
		}

		private void checkConnectServer_Click(object sender,EventArgs e) {
			if(checkConnectServer.Checked) {
				groupServer.Enabled=true;
				groupDirect.Enabled=false;
				checkDynamicMode.Checked=false;
				checkDynamicMode.Enabled=false;
			}
			else {
				groupServer.Enabled=false;
				groupDirect.Enabled=true;
				checkDynamicMode.Enabled=true;
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
			SyncModelWithUI();
			try {
				CentralConnections.TryToConnect(Model.CentralConnectionCur,Model.DbType,Model.ConnectionString,noShowOnStartup:(Model.NoShow==YN.Yes),
					Model.ListAdminCompNames,useDynamicMode:Model.UseDynamicMode,allowAutoLogin:Model.AllowAutoLogin);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return false;
			}
			//A successful connection was made using the settings within the current choose database model.
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!IsValidConnection()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}

	///<summary>A helper class that contains database connection information and other information that can show within the Choose Database window and even some information that is only stored within the FreeDentalConfig.xml and has no UI but needs to be preserved.</summary>
	public class ChooseDatabaseInfo {
		///<summmary></summmary>
		public CentralConnection CentralConnectionCur=new CentralConnection();
		///<summary></summary>
		public string ConnectionString="";
		///<summary>Indicates whether the user is using dynamic mode. That is, when connecting to a database of a lower version, they will
		///download the version from the server and run that instead of upgrading/downgrading their own client.</summary>
		public bool UseDynamicMode;
		///<summary>This is used when selecting File>Choose Database.  It will behave slightly differently.</summary>
		public bool IsAccessedFromMainMenu;
		///<summary>When silently running GetConfig() without showing UI, this gets set to true if either NoShowOnStartup or UsingEcw is found in config file.</summary>
		public YN NoShow;
		///<summary></summary>
		public DatabaseType DbType;
		///<summary>Stored so that they don't get deleted when re-writing the FreeDentalConfig file.</summary>
		public List<string> ListAdminCompNames=new List<string>();
		///<summary>Defaults to true. Allows the user to choose whether or not they can select 'Log me in automatically.'</summary>
		public bool AllowAutoLogin=true;

		public ChooseDatabaseInfo() {
		}

		///<summary>Every optional parameter provided should coincide with a command line argument. The values passed in will typically override any settings loaded in from the config file. Passing in a value for webServiceUri or databaseName will cause the config file to not even be considered.</summary>
		public static ChooseDatabaseInfo GetChooseDatabaseInfoFromConfig(string webServiceUri="",YN webServiceIsEcw=YN.Unknown,string odUser=""
			,string serverName="",string databaseName="",string mySqlUser="",string mySqlPassword="",string mySqlPassHash="",YN noShow=YN.Unknown
			,string odPassword="",bool useDynamicMode=false,string odPassHash="") 
		{
			ChooseDatabaseInfo chooseDatabaseInfo=new ChooseDatabaseInfo();
			//Even if we are passed a URI as a command line argument we still need to check the FreeDentalConfig file for middle tier automatic log in.
			//The only time we do not need to do that is if a direct DB has been passed in.
			if(string.IsNullOrEmpty(databaseName)) {
				CentralConnections.GetChooseDatabaseConnectionSettings(out chooseDatabaseInfo.CentralConnectionCur,out chooseDatabaseInfo.ConnectionString
					,out chooseDatabaseInfo.NoShow,out chooseDatabaseInfo.DbType,out chooseDatabaseInfo.ListAdminCompNames,out chooseDatabaseInfo.UseDynamicMode
					,out chooseDatabaseInfo.AllowAutoLogin);
			}
			//Command line args should always trump settings from the config file.
			#region Command Line Arguements
			if(webServiceUri!="") {//if a URI was passed in
				chooseDatabaseInfo.CentralConnectionCur.ServiceURI=webServiceUri;
			}
			if(webServiceIsEcw!=YN.Unknown) {
				chooseDatabaseInfo.CentralConnectionCur.WebServiceIsEcw=(webServiceIsEcw==YN.Yes ? true : false);
			}
			if(odUser!="") {
				chooseDatabaseInfo.CentralConnectionCur.OdUser=odUser;
			}
			if(odPassword!="") {
				chooseDatabaseInfo.CentralConnectionCur.OdPassword=odPassword;
			}
			if(!string.IsNullOrEmpty(odPassHash)) {
				chooseDatabaseInfo.CentralConnectionCur.OdPassHash=odPassHash;
			}
			if(serverName!="") {
				chooseDatabaseInfo.CentralConnectionCur.ServerName=serverName;
			}
			if(databaseName!="") {
				chooseDatabaseInfo.CentralConnectionCur.DatabaseName=databaseName;
			}
			if(mySqlUser!="") {
				chooseDatabaseInfo.CentralConnectionCur.MySqlUser=mySqlUser;
			}
			if(mySqlPassword!="") {
				chooseDatabaseInfo.CentralConnectionCur.MySqlPassword=mySqlPassword;
			}
			if(mySqlPassHash!="") {
				string decryptedPwd;
				CDT.Class1.Decrypt(mySqlPassHash,out decryptedPwd);
				chooseDatabaseInfo.CentralConnectionCur.MySqlPassword=decryptedPwd;
			}
			if(noShow!=YN.Unknown) {
				chooseDatabaseInfo.NoShow=noShow;
			}
			if(odUser!="" && odPassword!="") {
				chooseDatabaseInfo.NoShow=YN.Yes;
			}
			//If they are overridding to say to use dynamic mode.
			if(useDynamicMode) {
				chooseDatabaseInfo.UseDynamicMode=useDynamicMode;
			}
			#endregion
			return chooseDatabaseInfo;
		}

		///<summary>Updates the passed in ChooseDatabaseModel with the current database/middle tier connection. Only updates values that are stored in
		///FreeDentalConfig and are also kept in memory.</summary>
		public static void UpdateChooseDatabaseInfoFromCurrentConnection(ChooseDatabaseInfo chooseDatabaseInfo) {
			if(string.IsNullOrEmpty(RemotingClient.ServerURI)) {//Direct connection
				chooseDatabaseInfo.CentralConnectionCur.ServerName=DataConnection.GetServerName();
				chooseDatabaseInfo.CentralConnectionCur.DatabaseName=DataConnection.GetDatabaseName();
				chooseDatabaseInfo.CentralConnectionCur.MySqlUser=DataConnection.GetMysqlUser();
				chooseDatabaseInfo.CentralConnectionCur.MySqlPassword=DataConnection.GetMysqlPass();
				chooseDatabaseInfo.CentralConnectionCur.ServiceURI="";
			}
			else {//using Middle Tier
				chooseDatabaseInfo.CentralConnectionCur.ServiceURI=RemotingClient.ServerURI;
				chooseDatabaseInfo.CentralConnectionCur.ServerName="";
				chooseDatabaseInfo.CentralConnectionCur.DatabaseName="";
				chooseDatabaseInfo.CentralConnectionCur.MySqlUser="";
				chooseDatabaseInfo.CentralConnectionCur.MySqlPassword="";
			}
		}
	}

}