using CodeBase;
using DataConnectionBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using PasswordVaultWrapper;

namespace OpenDentBusiness{
	///<summary></summary>
	public class CentralConnections {
		#region Get Methods
		///<summary>Gets all the central connections from the database ordered by ItemOrder.</summary>
		public static List<CentralConnection> GetConnections(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CentralConnection>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM centralconnection ORDER BY ItemOrder";
			return Crud.CentralConnectionCrud.SelectMany(command);
		}

		///<summary>Gets all the central connections from the database for one group, ordered by ItemOrder.</summary>
		public static List<CentralConnection> GetForGroup(long connectionGroupNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CentralConnection>>(MethodBase.GetCurrentMethod(),connectionGroupNum);
			}
			string command="SELECT centralconnection.* "
				+"FROM conngroupattach "
				+"LEFT JOIN centralconnection ON conngroupattach.CentralConnectionNum=centralconnection.CentralConnectionNum "
				+"WHERE conngroupattach.ConnectionGroupNum="+POut.Long(connectionGroupNum)+" "
				+"ORDER BY ItemOrder";
			return Crud.CentralConnectionCrud.SelectMany(command);
		}

		///<summary>Gets all the central connections from the database that are not in the specificed group, ordered by ItemOrder.</summary>
		public static List<CentralConnection> GetNotForGroup(long connectionGroupNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CentralConnection>>(MethodBase.GetCurrentMethod(),connectionGroupNum);
			}
			string command="SELECT centralconnection.* "
				+"FROM centralconnection "
				+"LEFT JOIN conngroupattach ON conngroupattach.CentralConnectionNum=centralconnection.CentralConnectionNum "
				+"AND conngroupattach.ConnectionGroupNum = "+POut.Long(connectionGroupNum)+" "
				+"WHERE conngroupattach.ConnectionGroupNum IS NULL "
				+"ORDER BY ItemOrder";
			return Crud.CentralConnectionCrud.SelectMany(command);
		}
		#endregion

		#region Modification Methods
		///<summary></summary>
		public static long Insert(CentralConnection centralConnection){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				centralConnection.CentralConnectionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),centralConnection);
				return centralConnection.CentralConnectionNum;
			}
			return Crud.CentralConnectionCrud.Insert(centralConnection);
		}

		///<summary></summary>
		public static void Update(CentralConnection centralConnection){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),centralConnection);
				return;
			}
			Crud.CentralConnectionCrud.Update(centralConnection);
		}

		///<summary>Updates Status of the provided CentralConnection</summary>
		public static void UpdateStatus(CentralConnection centralConnection) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),centralConnection);
				return;
			}
			string command="UPDATE centralconnection SET ConnectionStatus='"+POut.String(centralConnection.ConnectionStatus)
				+"' WHERE CentralConnectionNum="+POut.Long(centralConnection.CentralConnectionNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void Delete(long centralConnectionNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),centralConnectionNum);
				return;
			}
			string command= "DELETE FROM centralconnection WHERE CentralConnectionNum = "+POut.Long(centralConnectionNum);
			Db.NonQ(command);
			command= "DELETE FROM conngroupattach WHERE CentralConnectionNum = "+POut.Long(centralConnectionNum);
			Db.NonQ(command);
		}
		#endregion

		#region Choose Database
		///<summary>Gets a list of all computer names on the network (this is not easy)</summary>
		public static List<string> GetComputerNames(){
			if(Environment.OSVersion.Platform==PlatformID.Unix) {
				return new List<string>();
			}
			try {
				Logger.LogToPath("Delete tempCompNames.txt",LogPath.Startup,LogPhase.Start);
				File.Delete(ODFileUtils.CombinePaths(Application.StartupPath,"tempCompNames.txt"));
				Logger.LogToPath("Delete tempCompNames.txt",LogPath.Startup,LogPhase.End);
				List<string> listStrings = new List<string>();
				//string myAdd=Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();//obsolete
				Logger.LogToPath("Dns.GetHostEntry",LogPath.Startup,LogPhase.Start);
				string myAdd=Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
				Logger.LogToPath("Dns.GetHostEntry",LogPath.Startup,LogPhase.End);
				ProcessStartInfo processStartInfo=new ProcessStartInfo();
				processStartInfo.FileName=@"C:\WINDOWS\system32\cmd.exe";//Path for the cmd prompt
				processStartInfo.Arguments="/c net view > tempCompNames.txt";//Arguments for the command prompt
				//"/c" tells it to run the following command which is "net view > tempCompNames.txt"
				//"net view" lists all the computers on the network
				//" > tempCompNames.txt" tells dos to put the results in a file called tempCompNames.txt
				processStartInfo.WindowStyle=ProcessWindowStyle.Hidden;//Hide the window
				Logger.LogToPath("CMD net view",LogPath.Startup,LogPhase.Start);
				Process.Start(processStartInfo);
				StreamReader streamReader=null; //Gets disposed in close() call at end of method
				string filename=ODFileUtils.CombinePaths(Application.StartupPath,"tempCompNames.txt");
				Thread.Sleep(200);//sleep for 1/5 second
				if(!File.Exists(filename)) {
					Logger.LogToPath("CMD net view - FileNotExist",LogPath.Startup,LogPhase.End);
					return new List<string>();
				}
				try {
					streamReader=new StreamReader(filename);
				}
				catch(Exception) {
					Logger.LogToPath("CMD net view - StreamReader Exception",LogPath.Startup,LogPhase.End);
					return new List<string>();
				}
				if(streamReader.Peek()==-1) {//If the file is empty, return.
					Logger.LogToPath("CMD net view - Empty File",LogPath.Startup,LogPhase.End);
					return new List<string>();
				}
				while(!streamReader.ReadLine().StartsWith("--")) {
					//The line just before the data looks like: --------------------------
				}
				string line="";
				listStrings.Add("localhost");
				while(true) {
					line=streamReader.ReadLine();
					if(line.StartsWith("The"))//cycle until we reach,"The command completed successfully."
						break;
					line=line.Split(char.Parse(" "))[0];// Split the line after the first space
					// Normally, in the file it lists it like this
					// \\MyComputer                 My Computer's Description
					// Take off the slashes, "\\MyComputer" to "MyComputer"
					listStrings.Add(line.Substring(2,line.Length-2));
				}
				streamReader.Close();
				Logger.LogToPath("CMD net view",LogPath.Startup,LogPhase.End);
				Logger.LogToPath("Delete tempCompNames.txt cleanup",LogPath.Startup,LogPhase.Start);
				File.Delete(ODFileUtils.CombinePaths(Application.StartupPath,"tempCompNames.txt"));
				Logger.LogToPath("Delete tempCompNames.txt cleanup",LogPath.Startup,LogPhase.End);
				return listStrings;
			}
			catch(Exception) {//it will always fail if not WinXP
				Logger.LogToPath("CMD net view - Generic Exception",LogPath.Startup,LogPhase.End);
				return new List<string>();
			}
		}

		///<summary></summary>
		public static List<string> GetDatabases(CentralConnection centralConnection,DatabaseType databaseType) {
			Logger.LogToPath("GetDatabases",LogPath.Startup,LogPhase.Start);
			if(centralConnection.ServerName=="") {
				Logger.LogToPath("GetDatabases - Blank ServerName",LogPath.Startup,LogPhase.End);
				return new List<string>();
			}
			if(databaseType!=DatabaseType.MySql) {
				Logger.LogToPath("GetDatabases - Not MySQL",LogPath.Startup,LogPhase.End);
				return new List<string>();//because SHOW DATABASES won't work
			}
			try {
				Logger.LogToPath("DataConnection",LogPath.Startup,LogPhase.Start);
				DataConnection dataConnection;
				//use the one table that we know exists
				if(centralConnection.MySqlUser=="") {
					dataConnection=new DataConnection(centralConnection.ServerName,"mysql","root",centralConnection.MySqlPassword,databaseType);
				}
				else {
					dataConnection=new DataConnection(centralConnection.ServerName,"mysql",centralConnection.MySqlUser,centralConnection.MySqlPassword
						,databaseType);
				}
				Logger.LogToPath("DataConnection",LogPath.Startup,LogPhase.End);
				Logger.LogToPath("SHOW DATABASES",LogPath.Startup,LogPhase.Start);
				string command="SHOW DATABASES";
				//if this next step fails, table will simply have 0 rows
				DataTable table=dataConnection.GetTable(command,false);
				Logger.LogToPath("SHOW DATABASES",LogPath.Startup,LogPhase.End);
				List<string> listNames=new List<string>();
				for(int i=0;i<table.Rows.Count;i++){
					listNames.Add(table.Rows[i][0].ToString());
				}
				Logger.LogToPath("GetDatabases",LogPath.Startup,LogPhase.End);
				return listNames;
			}
			catch(Exception) {
				Logger.LogToPath("GetDatabases - Generic Exception",LogPath.Startup,LogPhase.End);
				return new List<string>();
			}
		}

		///<summary>Throws an exception to display to the user if anything goes wrong.</summary>
		public static void TryToConnect(CentralConnection centralConnection,DatabaseType databaseType,string connectionString="",bool noShowOnStartup=false,
			List<string> listAdminCompNames=null,bool isCommandLineArgs=false,bool useDynamicMode=false,bool allowAutoLogin=true) 
		{
			if(!string.IsNullOrEmpty(centralConnection.ServiceURI)) {
				LoadMiddleTierProxySettings();
				string originalURI=RemotingClient.ServerURI;
				RemotingClient.ServerURI=centralConnection.ServiceURI;
				bool useEcwAlgorithm=centralConnection.WebServiceIsEcw;
				MiddleTierRole originalRole=RemotingClient.MiddleTierRole;
				RemotingClient.MiddleTierRole=MiddleTierRole.ClientMT;
				try {
					string password=centralConnection.OdPassword;
					if(useEcwAlgorithm) {
						//It doesn't matter what Security.CurUser is when it is null because we are technically trying to set it for the first time.
						//It cannot be null before invoking HashPassword for Middle Tier for creating credentials for DTOs.
						if(Security.CurUser==null) {
							Security.CurUser=new Userod();
						}
						//Utilize the custom eCW MD5 hashing algorithm if a password was typed into the choose db window or no password hash was passed in via command line arguments.
						if(!string.IsNullOrEmpty(password) || string.IsNullOrEmpty(centralConnection.OdPassHash)) {
							password=Authentication.HashPasswordMD5(password,true);
						}
						else {//eCW gave us the password already hashed via command line arguments, simply use it.
							password=centralConnection.OdPassHash;
						}
					}
					string username=centralConnection.OdUser;
					//ecw requires hash, but non-ecw requires actual password
					Security.CurUser=Security.LogInWeb(username,password,"",Application.ProductVersion,useEcwAlgorithm);
					Security.PasswordTyped=password;//for ecw, this is already encrypted.
				}
				catch(Exception) {
					RemotingClient.ServerURI=originalURI;
					RemotingClient.MiddleTierRole=originalRole;
					throw;
				}
			}
			else {
				Logger.LogToPath("DataConnection.SetDb",LogPath.Startup,LogPhase.Start);
				DataConnection.DBtype=databaseType;
				DataConnection dataConnection=new DataConnection();
				if(connectionString.Length > 0) {
					dataConnection.SetDb(connectionString,"",DataConnection.DBtype);
				}
				else {
					//Password could be plain text password from the Password field of the config file, the decrypted password from the MySQLPassHash field
					//of the config file, or password entered by the user and can be blank (empty string) in all cases
					dataConnection.SetDb(centralConnection.ServerName,centralConnection.DatabaseName,centralConnection.MySqlUser
						,centralConnection.MySqlPassword,"","",DataConnection.DBtype,centralConnection.SslCA);
				}
				Logger.LogToPath("DataConnection.SetDb",LogPath.Startup,LogPhase.End);
				//a direct connection does not utilize lower privileges.
				RemotingClient.MiddleTierRole=MiddleTierRole.ClientDirect;
			}
			//Only gets this far if we have successfully connected, thus, saving connection settings is appropriate.
			TrySaveConnectionSettings(centralConnection,databaseType,connectionString,noShowOnStartup:noShowOnStartup,listAdminCompNames,
				isCommandLineArgs:isCommandLineArgs,useDynamicMode:useDynamicMode,allowAutoLogin:allowAutoLogin);
		}

		///<summary>If MiddleTierProxyConfix.xml is present, this loads the three variables from that file into memory.
		///Throws exceptions if anything goes wrong which will typically be shown to the user.</summary>
		public static void LoadMiddleTierProxySettings() {
			string xmlPath=ODFileUtils.CombinePaths(Application.StartupPath,"MiddleTierProxyConfig.xml");
			if(!File.Exists(xmlPath)) {
				return;
			}
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.Load(xmlPath);
			RemotingClient.MidTierProxyAddress=xmlDocument.SelectSingleNode("//Address").InnerText;
			RemotingClient.MidTierProxyUserName=xmlDocument.SelectSingleNode("//UserName").InnerText;
			RemotingClient.MidTierProxyPassword=xmlDocument.SelectSingleNode("//Password").InnerText;
		}

		///<summary>Returns true if the connection settings were successfully saved to the FreeDentalConfig file and/or Windows PasswordVault.  
		///Otherwise, false.
		///Set isCommandLineArgs to true in order to preserve settings within FreeDentalConfig.xml that are not command line args.
		///E.g. the current value within the FreeDentalConfig.xml for NoShowOnStartup will be preserved instead of the value passed in.</summary>
		public static bool TrySaveConnectionSettings(CentralConnection centralConnection,DatabaseType databaseType,string connectionString=""
			,bool noShowOnStartup=false,List<string> listAdminCompNames=null,bool isCommandLineArgs=false,bool useDynamicMode=false,bool allowAutoLogin=true) 
		{
			try {
				Logger.LogToPath("TrySaveConnectionSettings",LogPath.Startup,LogPhase.Start);
				//The parameters passed in might have misleading information (like noShowOnStartup) if they were comprised from command line arguments.
				//Non-command line settings within the FreeDentalConfig.xml need to be preserved when command line arguments are used.
				if(isCommandLineArgs) {
					//Updating the freedentalconfig.xml file when connecting via command line arguments causes issues for users
					//who prefer to have a desktop icon pointing to their main database and additional icons for other databases (popular amongst CEMT users).
					return false;
					/**** The following code will be reintroduced in the near future.  Leaving as a comment on purpose. ****
					//Override all variables that are NOT valid command line arguments with their current values within the FreeDentalConfig.xml
					//This will preserve the values that should stay the same (e.g. noShowOnStartup is NOT a command line arg and will always be true).
					CentralConnection centralConnectionFile;
					string connectionStringFile;
					YN noShowFile;
					DatabaseType dbTypeFile;
					List<string> listAdminCompNamesFile;
					GetChooseDatabaseConnectionSettings(out centralConnectionFile,out connectionStringFile,out noShowFile,out dbTypeFile
						,out listAdminCompNamesFile,out useDynamicMode);
					//Since command line args are being used, override any variables that are NOT allowed to be passed in via the command line.
					#region FreeDentalConfig Nodes That Do Not Have a Corresponding Command Line Arg
					switch(noShowFile) {//config node: <NoShowOnStartup>
						case YN.Yes:
							noShowOnStartup=true;
							break;
						case YN.No:
						case YN.Unknown:
							//Either NoShowOnStartup was not found or was explicitly set to no.
							noShowOnStartup=false;
							break;
					}
					listAdminCompNames=listAdminCompNamesFile;//config node: <AdminCompNames>
					connectionString=connectionStringFile;//config node: <ConnectionString>
					centralConnection.IsAutomaticLogin=centralConnectionFile.IsAutomaticLogin;//config node: <UsingAutoLogin>
					#endregion
					********************************************************************************************************/
				}
				XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
				xmlWriterSettings.Indent=true;
				xmlWriterSettings.IndentChars=("    ");
				using XmlWriter xmlWriter=XmlWriter.Create(ODFileUtils.CombinePaths(Application.StartupPath,"FreeDentalConfig.xml"),xmlWriterSettings);
				xmlWriter.WriteStartElement("ConnectionSettings");
				if(!allowAutoLogin) {
					//Only add if it was added before.
					xmlWriter.WriteElementString("AllowAutoLogin","False");
				}
				if(connectionString!="") {
					xmlWriter.WriteStartElement("ConnectionString");
					xmlWriter.WriteString(connectionString);
					xmlWriter.WriteEndElement();
				}
				else if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientDirect) {
					xmlWriter.WriteStartElement("DatabaseConnection");
					xmlWriter.WriteStartElement("ComputerName");
					xmlWriter.WriteString(centralConnection.ServerName);
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("Database");
					xmlWriter.WriteString(centralConnection.DatabaseName);
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("User");
					xmlWriter.WriteString(centralConnection.MySqlUser);
					xmlWriter.WriteEndElement();
					string encryptedPwd;
					CDT.Class1.Encrypt(centralConnection.MySqlPassword,out encryptedPwd);//sets encryptedPwd ot value or null
					xmlWriter.WriteStartElement("Password");
					//If encryption fails, write plain text password to xml file; maintains old behavior.
					xmlWriter.WriteString(string.IsNullOrEmpty(encryptedPwd) ? centralConnection.MySqlPassword : "");
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("MySQLPassHash");
					xmlWriter.WriteString(encryptedPwd??"");
					xmlWriter.WriteEndElement();
					xmlWriter.WriteStartElement("NoShowOnStartup");
					if(noShowOnStartup) {
						xmlWriter.WriteString("True");
					}
					else {
						xmlWriter.WriteString("False");
					}
					xmlWriter.WriteEndElement();
					if(!string.IsNullOrEmpty(centralConnection.SslCA)) {
						xmlWriter.WriteStartElement("SslCa");
						xmlWriter.WriteString(centralConnection.SslCA);
						xmlWriter.WriteEndElement();
					}
				}
				else if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
					xmlWriter.WriteStartElement("ServerConnection");
					xmlWriter.WriteStartElement("URI");
					xmlWriter.WriteString(centralConnection.ServiceURI);
					xmlWriter.WriteEndElement();//end URI
					if(centralConnection.IsAutomaticLogin) {
						xmlWriter.WriteStartElement("User");
						xmlWriter.WriteString(centralConnection.OdUser);
						xmlWriter.WriteEndElement();//end Username
						xmlWriter.WriteStartElement("UsingAutoLogin");
						xmlWriter.WriteString("True");
						xmlWriter.WriteEndElement();//end UsingAutoLogin
					}
					xmlWriter.WriteStartElement("UsingEcw");
					if(centralConnection.WebServiceIsEcw) {
						xmlWriter.WriteString("True");
					}
					else {
						xmlWriter.WriteString("False");
					}
					xmlWriter.WriteEndElement();//end UsingEcw
					xmlWriter.WriteEndElement();//end ServerConnection
				}
				xmlWriter.WriteStartElement("DatabaseType");
				if(databaseType==DatabaseType.MySql) {
					xmlWriter.WriteString("MySql");
				}
				else {
					xmlWriter.WriteString("Oracle");
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("UseDynamicMode");
				xmlWriter.WriteString(useDynamicMode.ToString());
				xmlWriter.WriteEndElement();
				if(OpenDentBusiness.WebTypes.Shared.XWeb.XWebs.UseXWebTestGateway) {
					xmlWriter.WriteStartElement("UseXWebTestGateway");
					xmlWriter.WriteString("True");
					xmlWriter.WriteEndElement();
				}
				if(listAdminCompNames!=null && listAdminCompNames.Count>0) {
					xmlWriter.WriteStartElement("AdminCompNames");
					for(int i=0;i<listAdminCompNames.Count;i++){
						xmlWriter.WriteStartElement("CompName");
						xmlWriter.WriteString(listAdminCompNames[i]);
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Close();
				//Input the user's credentials to WCM (Windows Credential Manager) if necessary.
				if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT 
					&& !string.IsNullOrWhiteSpace(centralConnection.ServiceURI) 
					&& centralConnection.IsAutomaticLogin) 
				{
					bool isCredentialSaveRequired=true;//Assume credentials are not saved yet.
					if(WindowsPasswordVaultWrapper.TryRetrieveUserName(centralConnection.ServiceURI,out string userName)) {//Found a saved OD MT UserName
						//Update the credentials if the user or password is different.
						if(centralConnection.OdUser!=userName 
							|| WindowsPasswordVaultWrapper.RetrievePassword(centralConnection.ServiceURI,userName)!=centralConnection.OdPassword) 
						{
							//UserName or Password in the saved credentials does not match current password.  Delete the saved credentials and save the new 
							//credentials.  This will clear all the saved credentials from the PasswordVault for the currently logged in Windows User, which is 
							//desired behavior as each Windows User should only have one set of OpenDental Middle Tier credentials.
							WindowsPasswordVaultWrapper.ClearCredentials(centralConnection.ServiceURI);
						}
						else {
							isCredentialSaveRequired=false;//Username and Password already saved and match current credentials.
						}
					}
					if(isCredentialSaveRequired) { 
						if(!string.IsNullOrWhiteSpace(centralConnection.OdUser) && !string.IsNullOrWhiteSpace(centralConnection.OdPassword)) {
							//Save the new credentials.
							WindowsPasswordVaultWrapper.WritePassword(centralConnection.ServiceURI,centralConnection.OdUser,centralConnection.OdPassword);
						}
						else {
							return false;//Invalid username/password.
						}
					}
				}
			}
			catch(Exception ex) {
				Logger.LogToPath("TrySaveConnectionSettings failed: "+ex.Message,LogPath.Startup,LogPhase.Unspecified);
				return false;
			}
			finally {
				Logger.LogToPath("TrySaveConnectionSettings",LogPath.Startup,LogPhase.End);
			}
			return true;
		}
		#endregion Choose Database

		#region Misc Methods

		///<summary>Gets all of the connection setting information from the FreeDentalConfig.xml. Throws exceptions.</summary>
		public static ChooseDatabaseInfo GetChooseDatabaseConnectionSettings() 
		{
			Meth.NoCheckMiddleTierRole();
			ChooseDatabaseInfo chooseDatabaseInfo=new ChooseDatabaseInfo();
			CentralConnection centralConnection=new CentralConnection();
			string connectionString="";
			YN yNNoShow=YN.Unknown;
			DatabaseType databaseType=DatabaseType.MySql;
			List<string> listAdminCompNames=new List<string>();
			bool useDynamicMode=false;
			bool allowAutoLogin=true;
			string xmlPath=ODFileUtils.CombinePaths(Application.StartupPath,"FreeDentalConfig.xml");
			#region Permission Check
			//Improvement should be made here to avoid requiring admin priv.
			//Search path should be something like this:
			//1. /home/username/.opendental/config.xml (or corresponding user path in Windows)
			//2. /etc/opendental/config.xml (or corresponding machine path in Windows) (should be default for new installs) 
			//3. Application Directory/FreeDentalConfig.xml (read-only if user is not admin)
			if(!File.Exists(xmlPath)) {
				FileStream fileStream;
				try {
					fileStream=File.Create(xmlPath);
				}
				catch(Exception) {
					//No translation right here because we typically do not have a database connection yet.
					throw new ODException("The very first time that the program is run, it must be run as an Admin.  "
						+"If using Vista, right click, run as Admin.");
				}
				fileStream.Close();
			}
			#endregion
			XmlDocument xmlDocument=new XmlDocument();
			try {
				xmlDocument.Load(xmlPath);
				XPathNavigator xPathNavigator=xmlDocument.CreateNavigator();
				XPathNavigator xPathNavigator2;
				#region Nodes with No UI
				//Always look for these settings first in order to always preserve them correctly.
				xPathNavigator2=xPathNavigator.SelectSingleNode("//AdminCompNames");
				if(xPathNavigator2!=null) {
					listAdminCompNames.Clear(); //this method gets called more than once
					XPathNodeIterator xPathNavigatorIterator=xPathNavigator2.SelectChildren(XPathNodeType.All);
					for(int i=0;i<xPathNavigatorIterator.Count;i++) {
						xPathNavigatorIterator.MoveNext();
						listAdminCompNames.Add(xPathNavigatorIterator.Current.Value);//Add this computer name to the list.
					}
				}
				//See if there's a UseXWebTestGateway
				xPathNavigator2=xPathNavigator.SelectSingleNode("//UseXWebTestGateway");
				if(xPathNavigator2!=null) {
					OpenDentBusiness.WebTypes.Shared.XWeb.XWebs.UseXWebTestGateway=xPathNavigator2.Value.ToLower()=="true";
				}
				//See if there's a AllowAutoLogin node
				xPathNavigator2=xPathNavigator.SelectSingleNode("//AllowAutoLogin");
				if(xPathNavigator2!=null && xPathNavigator2.Value.ToLower()=="false") {
					//Node must be specifically set to false to change the allowAutoLogin bool.
					allowAutoLogin=false;
				}
				#endregion
				#region Nodes from Choose Database Window
				#region Nodes with No Group Box
				//Database Type
				xPathNavigator2=xPathNavigator.SelectSingleNode("//DatabaseType");
				databaseType=DatabaseType.MySql;
				if(xPathNavigator2!=null && xPathNavigator2.Value=="Oracle") {
					databaseType=DatabaseType.Oracle;
				}
				//ConnectionString
				xPathNavigator2=xPathNavigator.SelectSingleNode("//ConnectionString");
				if(xPathNavigator2!=null) {
					//If there is a ConnectionString, then use it.
					connectionString=xPathNavigator2.Value;
				}
				//UseDynamicMode
				xPathNavigator2=xPathNavigator.SelectSingleNode("//UseDynamicMode");
				if(xPathNavigator2!=null) {
					//If there is a node, take in its value
					useDynamicMode=PIn.Bool(xPathNavigator2.Value);
				}
				#endregion
				#region Connection Settings Group Box
				//See if there's a DatabaseConnection
				xPathNavigator2=xPathNavigator.SelectSingleNode("//DatabaseConnection");
				if(xPathNavigator2!=null) {
					//If there is a DatabaseConnection, then use it.
					centralConnection.ServerName=xPathNavigator2.SelectSingleNode("ComputerName").Value;
					centralConnection.DatabaseName=xPathNavigator2.SelectSingleNode("Database").Value;
					centralConnection.MySqlUser=xPathNavigator2.SelectSingleNode("User").Value;
					centralConnection.MySqlPassword=xPathNavigator2.SelectSingleNode("Password").Value;
					centralConnection.SslCA=xPathNavigator2.SelectSingleNode("SslCa")?.Value??"";
					XPathNavigator xPathNavigatorEncryptedPwdNode=xPathNavigator2.SelectSingleNode("MySQLPassHash");
					//If the Password node is empty, but there is a value in the MySQLPassHash node, decrypt the node value and use that instead
					string _decryptedPwd;
					if(centralConnection.MySqlPassword==""
						&& xPathNavigatorEncryptedPwdNode!=null
						&& xPathNavigatorEncryptedPwdNode.Value!=""
						&& CDT.Class1.Decrypt(xPathNavigatorEncryptedPwdNode.Value,out _decryptedPwd))
					{
						//decrypted value could be an empty string, which means they don't have a password set, so textPassword will be an empty string
						centralConnection.MySqlPassword=_decryptedPwd;
					}
					XPathNavigator xPathNavigatorNoShow=xPathNavigator2.SelectSingleNode("NoShowOnStartup");
					if(xPathNavigatorNoShow!=null) {
						if(xPathNavigatorNoShow.Value=="True") {
							yNNoShow=YN.Yes;
						}
						else {
							yNNoShow=YN.No;
						}
					}
				}
				#endregion
				#region Connect to Middle Tier Group Box
				xPathNavigator2=xPathNavigator.SelectSingleNode("//ServerConnection");
				/* example:
				<ServerConnection>
					<URI>http://server/OpenDentalServer/ServiceMain.asmx</URI>
					<UsingEcw>True</UsingEcw>
				</ServerConnection>
				*/
				if(xPathNavigator2!=null) {
					//If there is a ServerConnection, then use it.
					centralConnection.ServiceURI=xPathNavigator2.SelectSingleNode("URI").Value;
					XPathNavigator xPathNavigatorEcw=xPathNavigator2.SelectSingleNode("UsingEcw");
					if(xPathNavigatorEcw!=null && xPathNavigatorEcw.Value=="True") {
						yNNoShow=YN.Yes;
						centralConnection.WebServiceIsEcw=true;
					}
					XPathNavigator xPathNavigatorAutoLogin=xPathNavigator2.SelectSingleNode("UsingAutoLogin");
					//Retrieve the user from the Windows password vault for the current ServiceURI that was last to successfully single sign on.
					//If credentials are found then log the user in.  This is safe to do because the password vault is unique per Windows user and workstation.
					//There is code elsewhere that will handle password vault management (only storing the last valid single sign on per ServiceURI).
					//allowAutoLogin defaults to true unless the office specifically set it to false.
					if(xPathNavigatorAutoLogin!=null && xPathNavigatorAutoLogin.Value=="True" && allowAutoLogin) {
						if(!WindowsPasswordVaultWrapper.TryRetrieveUserName(centralConnection.ServiceURI,out centralConnection.OdUser)) {
							centralConnection.OdUser=xPathNavigator2.SelectSingleNode("User").Value;//No username found.  Use the User in FreeDentalConfig (preserve old behavior).
						}
						//Get the user's password from Windows Credential Manager
						try {
							centralConnection.OdPassword=
								WindowsPasswordVaultWrapper.RetrievePassword(centralConnection.ServiceURI,centralConnection.OdUser);
							//Must set this so FormChooseDatabase() does not launch
							yNNoShow=YN.Yes;
							centralConnection.IsAutomaticLogin=true;
						}
						catch(Exception ex) {
							ex.DoNothing();//We still want to display the server URI and the user name if getting the password fails.
						}
					}
				}
				#endregion
				#endregion
			}
			catch(Exception) {
				//Common error: root element is missing
				centralConnection.ServerName="localhost";
				if(ODBuild.IsTrial()) {
					centralConnection.DatabaseName="demo";
				}
				else { 
					centralConnection.DatabaseName="opendental";
				}
				centralConnection.MySqlUser="root";
			}
			chooseDatabaseInfo.AllowAutoLogin=allowAutoLogin;
			chooseDatabaseInfo.CentralConnectionCur=centralConnection;
			chooseDatabaseInfo.ConnectionString=connectionString;
			chooseDatabaseInfo.DatabaseType=databaseType;
			//IsAccessibleFromMainMenu is set in FormOpenDental
			chooseDatabaseInfo.ListAdminCompNames=listAdminCompNames;
			chooseDatabaseInfo.NoShow=yNNoShow;
			chooseDatabaseInfo.UseDynamicMode=useDynamicMode;
			return chooseDatabaseInfo;
		}

		///<summary>Filters _listConns to only include connections that are associated to the selected connection group.</summary>
		public static List<CentralConnection> FilterConnections(List<CentralConnection> listCentralConnections,string filterText,ConnectionGroup connectionGroup) {
			Meth.NoCheckMiddleTierRole();
			//Get all ConnGroupAttaches for selected group.
			List<CentralConnection> listCentralConnectionsRet=listCentralConnections;
			if(connectionGroup!=null) {
				//Find all central connections that are in the group list
				List<ConnGroupAttach> listCentralConnGroupAttaches=ConnGroupAttaches.GetForGroup(connectionGroup.ConnectionGroupNum);
				listCentralConnectionsRet=listCentralConnectionsRet.FindAll(x => listCentralConnGroupAttaches.Exists(y => y.CentralConnectionNum==x.CentralConnectionNum));
			}
			//Find all central connections that meet the filterText criteria
			listCentralConnectionsRet=listCentralConnectionsRet.FindAll(x => x.DatabaseName.ToLower().Contains(filterText.ToLower()) 
																 || x.ServerName.ToLower().Contains(filterText.ToLower()) 
																 || x.ServiceURI.ToLower().Contains(filterText.ToLower()));
			return listCentralConnectionsRet;
		}

		///<summary>Encrypts signature text and returns a base 64 string so that it can go directly into the database.</summary>
		public static string Encrypt(string str,byte[] key){
			Meth.NoCheckMiddleTierRole();
			if(str==""){
				return "";
			}
			byte[] ecryptBytes=Encoding.UTF8.GetBytes(str);
			MemoryStream memoryStream=new MemoryStream();
			CryptoStream cryptoStream=null;
			AesCryptoServiceProvider aesCryptoServiceProvider=new AesCryptoServiceProvider();
			aesCryptoServiceProvider.Key=key;
			aesCryptoServiceProvider.IV=new byte[16];
			ICryptoTransform iCryptoTransformEncryptor=aesCryptoServiceProvider.CreateEncryptor(aesCryptoServiceProvider.Key,aesCryptoServiceProvider.IV);
			cryptoStream=new CryptoStream(memoryStream,iCryptoTransformEncryptor,CryptoStreamMode.Write);
			cryptoStream.Write(ecryptBytes,0,ecryptBytes.Length);
			cryptoStream.FlushFinalBlock();
			byte[] byteArrayEncrypted=new byte[memoryStream.Length];
			memoryStream.Position=0;
			memoryStream.Read(byteArrayEncrypted,0,(int)memoryStream.Length);
			cryptoStream.Dispose();
			memoryStream.Dispose();
			if(aesCryptoServiceProvider!=null) {
				aesCryptoServiceProvider.Clear();
			}
			return Convert.ToBase64String(byteArrayEncrypted);			
		}

		public static string Decrypt(string str,byte[] key) {
			Meth.NoCheckMiddleTierRole();
			if(str==""){
				return "";
			}
			try {
				byte[] byteArrayEncrypted=Convert.FromBase64String(str);
				MemoryStream memoryStream=null;
				CryptoStream cryptoStream=null;
				StreamReader streamReader=null;
				Aes aes=new AesCryptoServiceProvider();
				aes.Key=key;
				aes.IV=new byte[16];
				ICryptoTransform iCryptoTransformDecryptor=aes.CreateDecryptor(aes.Key,aes.IV);
				memoryStream=new MemoryStream(byteArrayEncrypted);
				cryptoStream=new CryptoStream(memoryStream,iCryptoTransformDecryptor,CryptoStreamMode.Read);
				streamReader=new StreamReader(cryptoStream);
				string decrypted=streamReader.ReadToEnd();
				memoryStream.Dispose();
				cryptoStream.Dispose();
				streamReader.Dispose();
				if(aes!=null) {
					aes.Clear();
				}
				return decrypted;
			}
			catch { 
				//MessageBox.Show("Text entered was not valid encrypted text.");
				return"";
			}
		}

		///<summary></summary>
		private static string GenerateHash(string message) {
			Meth.NoCheckMiddleTierRole();
			byte[] byteArrayData=Encoding.ASCII.GetBytes(message);
			HashAlgorithm hashAlgorithm=SHA1.Create();
			byte[] byteArrayHash=hashAlgorithm.ComputeHash(byteArrayData);
			byte byte1;
			byte byte2;
			string str1;
			string str2;
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<byteArrayHash.Length;i++) {
				if(byteArrayHash[i]==0) {
					byte1=0;
					byte2=0;
				}
				else {
					byte1=(byte)Math.Floor((double)byteArrayHash[i]/16d);
					//double remainder=Math.IEEERemainder((double)hashbytes[i],16d);
					byte2=(byte)(byteArrayHash[i]-(byte)(16*byte1));
				}
				str1=ByteToStr(byte1);
				str2=ByteToStr(byte2);
				stringBuilder.Append(str1);
				stringBuilder.Append(str2);
			}
			return stringBuilder.ToString();
		}

		///<summary>Supply a CentralConnection and this method will go through the logic to put together the connection string.</summary>
		public static string GetConnectionString(CentralConnection centralConnection) {
			string connString="";
			if(centralConnection.DatabaseName!="") {
				connString=centralConnection.ServerName;
				connString+=", "+centralConnection.DatabaseName;
			}
			else if(centralConnection.ServiceURI!="") {
				connString=centralConnection.ServiceURI;
			}
			return connString;
		}

		///<summary>The only valid input is a value between 0 and 15.  Text returned will be 1-9 or a-f.</summary>
		private static string ByteToStr(Byte byteVal) {
			Meth.NoCheckMiddleTierRole();
			switch(byteVal) {
				case 10:
					return "a";
				case 11:
					return "b";
				case 12:
					return "c";
				case 13:
					return "d";
				case 14:
					return "e";
				case 15:
					return "f";
				default:
					return byteVal.ToString();
			}
		}


		/*
		
		///<summary>Gets one CentralConnection from the db.</summary>
		public static CentralConnection GetOne(long centralConnectionNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<CentralConnection>(MethodBase.GetCurrentMethod(),centralConnectionNum);
			}
			return Crud.CentralConnectionCrud.SelectOne(centralConnectionNum);
		}

		
		*/
		#endregion Misc Methods
	}

	#region ChooseDatabaseInfo
	///<summary>A storage class that contains database connection information and other information that can show within the Choose Database window and even some information that is only stored within the FreeDentalConfig.xml and has no UI but needs to be preserved.</summary>
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
		public DatabaseType DatabaseType;
		///<summary>Stored so that they don't get deleted when re-writing the FreeDentalConfig file.</summary>
		public List<string> ListAdminCompNames=new List<string>();
		///<summary>Defaults to true. Allows the user to choose whether or not they can select 'Log me in automatically.'</summary>
		public bool AllowAutoLogin=true;

		public ChooseDatabaseInfo() {
		}

		///<summary>Every optional parameter provided should coincide with a command line argument. The values passed in will typically override any settings loaded in from the config file. Passing in a value for webServiceUri or databaseName will cause the config file to not even be considered.</summary>
		public static ChooseDatabaseInfo GetChooseDatabaseInfoFromConfig(string webServiceUri="",YN webServiceIsEcw=YN.Unknown,string odUser=""
			,string serverName="",string databaseName="",string mySqlUser="",string mySqlPassword="",string mySqlPassHash="",YN yNNoShow=YN.Unknown
			,string odPassword="",bool useDynamicMode=false,string odPassHash="") 
		{
			ChooseDatabaseInfo chooseDatabaseInfo=new ChooseDatabaseInfo();
			//Even if we are passed a URI as a command line argument we still need to check the FreeDentalConfig file for middle tier automatic log in.
			//The only time we do not need to do that is if a direct DB has been passed in.
			if(string.IsNullOrEmpty(databaseName)) {
				Logger.LogToPath("GetChooseDatabaseConnectionSettings",LogPath.Startup,LogPhase.Start);
				chooseDatabaseInfo=CentralConnections.GetChooseDatabaseConnectionSettings();
				Logger.LogToPath("GetChooseDatabaseConnectionSettings",LogPath.Startup,LogPhase.End);
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
			if(yNNoShow!=YN.Unknown) {
				chooseDatabaseInfo.NoShow=yNNoShow;
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

		///<summary>Updates the passed in ChooseDatabaseInfo with the current database/middle tier connection. Only updates values that are stored in
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
	#endregion ChooseDatabaseInfo
}