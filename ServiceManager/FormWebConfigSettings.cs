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
		}

		private void textPassword_TextChanged(object sender,EventArgs e) {
			if(textPassword.Text=="") {
				textPassword.PasswordChar=default(char);//if text is cleared, turn off password char mask
			}
		}

		private void textPassword_Leave(object sender,EventArgs e) {
			textPassword.PasswordChar=textPassword.Text==""?default(char):'*';//mask password on leave
		}

		public bool TestConnection() {
			DataConnection con=new DataConnection();
			try {
				con.SetDb(textServer.Text,textDatabase.Text,textUser.Text,textPassword.Text,textUserLow.Text,textPasswordLow.Text,
					(DatabaseType)comboDatabaseType.SelectedIndex);
				return true;
			}
			catch(Exception ex) {
				MessageBox.Show("Error connecting to database: "+ex.Message);
				return false;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			this.Close();
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
			//Creating Nodes
			XmlNode connSettings=document.CreateNode(XmlNodeType.Element,"ConnectionSettings","");
			XmlNode databaseConnection=document.CreateNode(XmlNodeType.Element,"DatabaseConnection","");
			XmlNode compName=document.CreateNode(XmlNodeType.Element,"ComputerName","");
			compName.InnerText=textServer.Text;
			XmlNode database=document.CreateNode(XmlNodeType.Element,"Database","");
			database.InnerText=textDatabase.Text;
			XmlNode user=document.CreateNode(XmlNodeType.Element,"User","");
			user.InnerText=textUser.Text;
			string encryptedPwd;
			CDT.Class1.Encrypt(textPassword.Text,out encryptedPwd);
			XmlNode password=document.CreateNode(XmlNodeType.Element,"Password","");
			password.InnerText=string.IsNullOrEmpty(encryptedPwd)?textPassword.Text:"";//only write the mysql password in plain text if encryption fails
			XmlNode mysqlPassHash=document.CreateNode(XmlNodeType.Element,"MySQLPassHash","");
			mysqlPassHash.InnerText=encryptedPwd??"";//if encryptedPwd is null write empty string
			XmlNode userLow=document.CreateNode(XmlNodeType.Element,"UserLow","");
			userLow.InnerText=textUserLow.Text;
			XmlNode passwordLow=document.CreateNode(XmlNodeType.Element,"PasswordLow","");
			passwordLow.InnerText=textPasswordLow.Text;
			XmlNode dbType=document.CreateNode(XmlNodeType.Element,"DatabaseType","");
			dbType.InnerText="MySql";//Not going to support Oracle until someone complains.
			XmlNode logLevelOfApp=document.CreateNode(XmlNodeType.Element,"LogLevelOfApplication","");
			logLevelOfApp.InnerText=comboLogLevel.Items[comboLogLevel.SelectedIndex].ToString();
			//Assigning Structure
			databaseConnection.AppendChild(compName);
			databaseConnection.AppendChild(database);
			databaseConnection.AppendChild(user);
			databaseConnection.AppendChild(password);
			databaseConnection.AppendChild(mysqlPassHash);
			databaseConnection.AppendChild(userLow);
			databaseConnection.AppendChild(passwordLow);
			databaseConnection.AppendChild(dbType);
			connSettings.AppendChild(databaseConnection);
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
			string fileName="OpenDentalWebConfig.xml";
			if(_serviceFile.Name=="OpenDentalService.exe") {
				fileName="OpenDentalServiceConfig.xml";
			}
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
