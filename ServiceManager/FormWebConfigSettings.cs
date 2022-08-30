using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using CodeBase;
using DataConnectionBase;

namespace ServiceManager {
	public partial class FormWebConfigSettings:Form {
		private FileInfo _serviceFile;

		///<summary>Pass in the file information for the service file that is being installed.  We will use the file path to determine where to put the config file.</summary>
		public FormWebConfigSettings(FileInfo serviceFile) {
			InitializeComponent();
			_serviceFile=serviceFile;
			if(_serviceFile.Name!="OpenDentalReplicationService.exe") {
				//The replication service controls are shwoing by default. 
				//Don't show the replciation service controls.
				Height=325;
				checkIsOneWayReplication.Visible=false;
				groupReplicationMaster.Visible=false;
			}
			Text=$"{GetFileName()} Settings";
		}

		private void FormWebConfigSettings_Load(object sender,EventArgs e) {
			string xmlPath=Path.Combine(Application.StartupPath,"FreeDentalConfig.xml");
			XmlDocument document=new XmlDocument();
			try {//Try FreeDentalConfig.xml first
				document.Load(xmlPath);
				XPathNavigator Navigator=document.CreateNavigator();
				XPathNavigator nav;
				nav=Navigator.SelectSingleNode("//DatabaseConnection");
				if(nav==null) {
					throw new Exception("DatabaseConnection element missing from FreeDentalConfig.xml, which is required.");
				}
				textServer.Text=nav.SelectSingleNode("ComputerName").Value;
				textDatabase.Text=nav.SelectSingleNode("Database").Value;
				textUser.Text=nav.SelectSingleNode("User").Value;
				textPassword.Text=nav.SelectSingleNode("Password").Value;
				XPathNavigator encryptedPwdNode=nav.SelectSingleNode("MySQLPassHash");
				string decryptedPwd;
				if(textPassword.Text==""
					&& encryptedPwdNode!=null
					&& encryptedPwdNode.Value!=""
					&& CDT.Class1.Decrypt(encryptedPwdNode.Value,out decryptedPwd))
				{
					textPassword.Text=decryptedPwd;
				}
				textPassword.PasswordChar=textPassword.Text==""?default(char):'*';//mask password
				textUserLow.Text="";
				textPasswordLow.Text="";
				comboLogLevel.Items.AddRange(Enum.GetNames(typeof(LogLevel)));//Isn't included in FreeDentalConfig, but is needed for the web service.
				comboLogLevel.SelectedItem=comboLogLevel.Items[0];
			}
			catch(Exception ex) {//FreeDentalConfig didn't load correctly
				ex.DoNothing();
				textServer.Text="localhost";
				textDatabase.Text="opendental";
				textUser.Text="root";
				textPassword.Text="";
				textUserLow.Text="";
				textPasswordLow.Text="";
				comboLogLevel.Items.AddRange(Enum.GetNames(typeof(LogLevel)));
				comboLogLevel.SelectedItem=comboLogLevel.Items[0];
			}
			comboDatabaseType.Items.AddRange(Enum.GetNames(typeof(DatabaseType)));
			comboDatabaseType.SelectedIndex=0;//MySQL
			groupReplicationMaster.Enabled=checkIsOneWayReplication.Checked;
			comboReplicationMasterDbType.Items.AddRange(Enum.GetNames(typeof(DatabaseType)));
			comboReplicationMasterDbType.SelectedIndex=0;//MySQL
		}

		private void checkIsOneWayReplication_CheckedChanged(object sender,EventArgs e) {
			groupReplicationMaster.Enabled=checkIsOneWayReplication.Checked;
		}

		private void textPassword_TextChanged(object sender,EventArgs e) {
			if(textPassword.Text=="") {
				textPassword.PasswordChar=default(char);//if text is cleared, turn off password char mask
			}
		}

		private void textPassword_Leave(object sender,EventArgs e) {
			textPassword.PasswordChar=textPassword.Text==""?default(char):'*';//mask password on leave
		}

		public bool TestConnection(bool isOneWayRepMaster=false) {
			DataConnection con=new DataConnection();
			try {
				string server=(isOneWayRepMaster ? textReplicationMasterServer.Text:textServer.Text);
				string database=(isOneWayRepMaster ? textReplicationMasterDatabase.Text:textDatabase.Text);
				string user=(isOneWayRepMaster ? textReplicationMasterUser.Text:textUser.Text);
				string password=(isOneWayRepMaster ? textReplicationMasterPass.Text:textPassword.Text);
				string userLow;
				string passwordLow;
				if(isOneWayRepMaster) {//The replication service uses a direct connection. The low user is never used.
					userLow="";
					passwordLow="";
				}
				else {
					userLow=textUserLow.Text;
					passwordLow=textPasswordLow.Text;
				}
				DatabaseType dbType=(isOneWayRepMaster ? (DatabaseType)comboReplicationMasterDbType.SelectedIndex:(DatabaseType)comboDatabaseType.SelectedIndex);
				con.SetDb(server,database,user,password,userLow,passwordLow,dbType);
				return true;
			}
			catch(Exception ex) {
				MessageBox.Show("Error connecting to database: "+ex.Message);
				return false;
			}
		}

		private bool IsOneWayReplicationMasterValid() {
			if(!checkIsOneWayReplication.Checked) {
				return true;
			}
			if(string.IsNullOrWhiteSpace(textReplicationMasterServer.Text)) {
				MessageBox.Show("Cannot leave master server field blank.");
				return false;
			}
			if(string.IsNullOrWhiteSpace(textReplicationMasterDatabase.Text)) {
				MessageBox.Show("Cannot leave master database field blank.");
				return false;
			}
			if(string.IsNullOrWhiteSpace(textReplicationMasterServer.Text)) {
				MessageBox.Show("Cannot leave master server field blank.");
				return false;
			}
			if(!TestConnection(isOneWayRepMaster:true)) {
				return false;
			}
			return true;
		}

		private XmlNode GetDataConnectionNode(XmlDocument document,bool isOneWayRepMaster=false) {
			string dbConnection=(isOneWayRepMaster ?"DatabaseConnectionMaster":"DatabaseConnection");
			string strServer=(isOneWayRepMaster ? textReplicationMasterServer.Text:textServer.Text);
			string strDatabase=(isOneWayRepMaster ? textReplicationMasterDatabase.Text:textDatabase.Text);
			string strUser=(isOneWayRepMaster ? textReplicationMasterUser.Text:textUser.Text);
			string strPassword=(isOneWayRepMaster ? textReplicationMasterPass.Text:textPassword.Text);
			string strUserLow;
			string strPasswordLow;
			if(isOneWayRepMaster) {//The replication service uses a direct connection. The low user is never used.
				strUserLow="";
				strPasswordLow="";
			}
			else {
				strUserLow=textUserLow.Text;
				strPasswordLow=textPasswordLow.Text;
			}
			//dbType - Not going to support Oracle until someone complains
			XmlNode databaseConnection=document.CreateNode(XmlNodeType.Element,dbConnection,"");
			XmlNode compName=document.CreateNode(XmlNodeType.Element,"ComputerName","");
			compName.InnerText=strServer;
			XmlNode database=document.CreateNode(XmlNodeType.Element,"Database","");
			database.InnerText=strDatabase;
			XmlNode user=document.CreateNode(XmlNodeType.Element,"User","");
			user.InnerText=strUser;
			string encryptedPwd;
			CDT.Class1.Encrypt(strPassword,out encryptedPwd);
			XmlNode password=document.CreateNode(XmlNodeType.Element,"Password","");
			password.InnerText=string.IsNullOrEmpty(encryptedPwd) ? strPassword : "";//only write the mysql password in plain text if encryption fails
			XmlNode mysqlPassHash=document.CreateNode(XmlNodeType.Element,"MySQLPassHash","");
			mysqlPassHash.InnerText=encryptedPwd??"";//if encryptedPwd is null write empty string
			XmlNode userLow=document.CreateNode(XmlNodeType.Element,"UserLow","");
			userLow.InnerText=strUserLow;
			XmlNode passwordLow=document.CreateNode(XmlNodeType.Element,"PasswordLow","");
			passwordLow.InnerText=strPasswordLow;
			XmlNode dbType=document.CreateNode(XmlNodeType.Element,"DatabaseType","");
			dbType.InnerText="MySql";//Not going to support Oracle until someone complains.
			//Assigning Structure
			databaseConnection.AppendChild(compName);
			databaseConnection.AppendChild(database);
			databaseConnection.AppendChild(user);
			databaseConnection.AppendChild(password);
			databaseConnection.AppendChild(mysqlPassHash);
			databaseConnection.AppendChild(userLow);
			databaseConnection.AppendChild(passwordLow);
			databaseConnection.AppendChild(dbType);
			return databaseConnection;
		}

		private void butOk_Click(object sender,EventArgs e) {
			XmlDocument document=new XmlDocument();
			if(textServer.Text=="") {
				MessageBox.Show("Cannot leave server field blank.");
				return;
			}
			if(textDatabase.Text=="") {
				MessageBox.Show("Cannot leave database field blank.");
				return;
			}
			if(textUser.Text=="") {
				MessageBox.Show("Cannot leave user field blank.");
				return;
			}
			if(!TestConnection()) {
				return;
			}
			if(!IsOneWayReplicationMasterValid()) {
				return;
			}
			//Creating Nodes
			XmlNode connSettings=document.CreateNode(XmlNodeType.Element,"ConnectionSettings","");
			XmlNode databaseConnection=GetDataConnectionNode(document);
			connSettings.AppendChild(databaseConnection);
			if(checkIsOneWayReplication.Checked) {
				XmlNode dataConnectionMaster=GetDataConnectionNode(document,isOneWayRepMaster:true);
				connSettings.AppendChild(dataConnectionMaster);
			}
			XmlNode logLevelOfApp=document.CreateNode(XmlNodeType.Element,"LogLevelOfApplication","");
			logLevelOfApp.InnerText=comboLogLevel.Items[comboLogLevel.SelectedIndex].ToString();
			connSettings.AppendChild(logLevelOfApp);
			document.AppendChild(connSettings);
			//Outputting completed XML document
			StringBuilder strb=new StringBuilder();
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars="   ";
			settings.NewLineChars="\r\n";
			settings.OmitXmlDeclaration=true;
			XmlWriter xmlWriter=XmlWriter.Create(strb,settings);
			document.WriteTo(xmlWriter);
			xmlWriter.Flush();
			string fileName=GetFileName();
			try {
				File.WriteAllText(Path.Combine(_serviceFile.DirectoryName,fileName),strb.ToString());
			}
			catch {
				MessageBox.Show("There was a problem writing a file to the system. Please see manual for more information.");
				return;
			}
			xmlWriter.Close();
			strb.Clear();
			DialogResult=DialogResult.OK;
		}

		private string GetFileName() {
			string fileName="OpenDentalWebConfig.xml";
			if(_serviceFile.Name=="OpenDentalService.exe") {
				fileName="OpenDentalServiceConfig.xml";
			}
			else if(_serviceFile.Name=="OpenDentalReplicationService.exe") {
				fileName="OpenDentalReplicationServiceConfig.xml";
			}
			return fileName;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			this.Close();
		}
	}

	///<summary>0=Error, 1=Information, 2=Verbose</summary>
	public enum LogLevel {
		///<summary>0 Logs only errors.</summary>
		Error=0,
		///<summary>1 Logs information plus errors.</summary>
		Information=1,
		///<summary>2 Most verbose form of logging (use sparingly for very specific troubleshooting). Logs all entries all the time.</summary>
		Verbose=2
	}
}
