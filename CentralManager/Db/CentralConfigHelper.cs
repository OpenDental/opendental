using CodeBase;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace CentralManager {
	public class CentralConfigHelper {

		///<summary>Adds two nodes to CentralManagerConfig.xml, "UsingAutoLogin" and "AutoLoginUser". These are used to skip FormCentralChooseDatabase for subsequent sign ins. Nodes removed by checking box in FormCentralManager>File>User Settings.  Throws exceptions.</summary>
		public static void EnableAutoLogin(string uri,string username,string password) {
			string xmlPath=ODFileUtils.CombinePaths(Application.StartupPath,"CentralManagerConfig.xml");
			XmlDocument document=new XmlDocument();
			document.Load(xmlPath); //shouldn't throw, has already been accessed at this point
			XmlNode nodeDBConnection=document.SelectSingleNode("//DatabaseConnection");
			//Remove auto login nodes if present. User was unable to log on successfully with them
			XmlNode nodeUsingAutoLogin=nodeDBConnection.SelectSingleNode("UsingAutoLogin");
			XmlNode nodeAutoLoginUser=nodeDBConnection.SelectSingleNode("AutoLoginUser");
			if(nodeUsingAutoLogin!=null) {
				nodeDBConnection.RemoveChild(nodeUsingAutoLogin);
			}
			if(nodeAutoLoginUser!=null) {
				nodeDBConnection.RemoveChild(nodeAutoLoginUser);
			}
			nodeUsingAutoLogin=document.CreateNode(XmlNodeType.Element,"UsingAutoLogin","");
			nodeAutoLoginUser=document.CreateNode(XmlNodeType.Element,"AutoLoginUser","");
			nodeUsingAutoLogin.InnerText="True";
			nodeAutoLoginUser.InnerText=username;
			nodeDBConnection.AppendChild(nodeUsingAutoLogin);
			nodeDBConnection.AppendChild(nodeAutoLoginUser);
			//Outputting completed XML document
			StringBuilder strb=new StringBuilder();
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars="   ";
			using(XmlWriter xmlWriter = XmlWriter.Create(strb,settings)) {
				document.WriteTo(xmlWriter);
				xmlWriter.Flush();
				File.WriteAllText(xmlPath,strb.ToString());
				xmlWriter.Close();
				strb.Clear();
			}
			PasswordVaultWrapper.WritePassword(uri,username,password); //password already used successfully, do not need to validate
		}

		///<summary>Disables logging in automatically through Middle Tier for the current user. Removes their credentials from the PasswordVault and deletes the relevent login nodes in CentralManagerConfig.xml. Throws exceptions.  </summary>
		public static void DisableAutoLogin(string uri,string username) {
			string password="";
			password=PasswordVaultWrapper.RetrievePassword(uri,username);   //hold password to restore if needed
			PasswordVaultWrapper.ClearCredentials(uri);
			//remove auto login nodes from file
			string xmlPath=ODFileUtils.CombinePaths(Application.StartupPath,"CentralManagerConfig.xml");
			XmlDocument document=new XmlDocument();
			try {
				document.Load(xmlPath);
				XmlNode nodeDBConnection=document.SelectSingleNode("//DatabaseConnection");
				//remove auto login nodes if present. User was unable to log on successfully with them
				XmlNode nodeUsingAutoLogin=nodeDBConnection.SelectSingleNode("UsingAutoLogin");
				XmlNode nodeAutoLoginUser=nodeDBConnection.SelectSingleNode("AutoLoginUser");
				if(nodeUsingAutoLogin!=null) {
					nodeDBConnection.RemoveChild(nodeUsingAutoLogin);
				}
				if(nodeAutoLoginUser!=null) {
					nodeDBConnection.RemoveChild(nodeAutoLoginUser);
				}
				//Outputting completed XML document
				StringBuilder strb=new StringBuilder();
				XmlWriterSettings settings=new XmlWriterSettings();
				settings.Indent=true;
				settings.IndentChars="   ";
				using(XmlWriter xmlWriter = XmlWriter.Create(strb,settings)) {
					document.WriteTo(xmlWriter);
					xmlWriter.Flush();
					File.WriteAllText(xmlPath,strb.ToString());
					xmlWriter.Close();
					strb.Clear();
				}
			}
			catch(Exception) { //Unable to remove automatic login nodes, put back deleted username and password
				PasswordVaultWrapper.WritePassword(uri,username,password);
				throw;
			}
		}

	}
}
