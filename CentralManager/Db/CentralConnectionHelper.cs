using System;
using OpenDentBusiness;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DataConnectionBase;
using System.Diagnostics;

namespace CentralManager {
	public class CentralConnectionHelper {

		///<summary>Returns command-line arguments for launching Open Dental based off of the settings for the connection passed in.</summary>
		private static string GetArgsFromConnection(CentralConnection centralConnection,bool useDynamicMode) {
			string args="";
			if(centralConnection.DatabaseName!="") {
				args+="ServerName="+Scrub(centralConnection.ServerName)+" ";
				args+="DatabaseName="+Scrub(centralConnection.DatabaseName)+" ";
				args+="MySqlUser="+Scrub(centralConnection.MySqlUser)+" ";
				if(centralConnection.MySqlPassword!="") {
					string mySqlPass=CentralConnections.Decrypt(centralConnection.MySqlPassword,FormCentralManager.EncryptionKey);
					CDT.Class1.Encrypt(mySqlPass,out string mySqlPassObfuscated);
					args+="MySqlPassObfuscated="+Scrub(mySqlPassObfuscated)+" ";
				}
			}
			else if(centralConnection.ServiceURI!="") {
				args+="WebServiceUri="+Scrub(centralConnection.ServiceURI)+" ";
				if(centralConnection.WebServiceIsEcw) {
					args+="WebServiceIsEcw=True ";
				}
			}
			args+="DynamicMode="+Scrub(useDynamicMode.ToString())+" ";
			return args;
		}

		///<summary>Launches OD.  Sets hWnd and ProcessID.  If this fails to launch, a textbox will appear.</summary>
		public static void LaunchOpenDental(CentralConnection centralConnection,bool useDynamicMode,bool isAutoLogin,bool isDomainLogin,long patNum,ref WindowInfo windowInfo) {
			string args=GetArgsFromConnection(centralConnection,useDynamicMode);
			if(isAutoLogin) {
				args+="UserName="+Scrub(Security.CurUser.UserName)+" ";
				CDT.Class1.Encrypt(Security.PasswordTyped,out string odPassObfuscated);
				args+="OdPassObfuscated="+Scrub(odPassObfuscated)+" ";
			}
			if(isDomainLogin) {
				args+="DomainUser="+Scrub(Security.CurUser.DomainUser)+" ";
			}
			if(patNum!=0){
				args+="PatNum="+Scrub(patNum.ToString(),encapsulate:false)+" ";
			}
			try {
				Process process=Process.Start("OpenDental.exe",args);
				windowInfo.HWnd=IntPtr.Zero;//process.MainWindowHandle;//but this hWnd seems to be wrong
				windowInfo.ProcessId=process.Id;
			}
			catch {
				OpenDental.MessageBox.Show("Unable to start the process OpenDental.exe.");
				//return IntPtr.Zero;
			}
		}

		///<summary>Sets the current data connection settings of the central manager to the connection settings passed in.  Automatically refreshes the local cache to reflect the cache of the connection passed in.  There is an overload for this function if you dont want to refresh the entire cache.</summary>
		public static bool SetCentralConnection(CentralConnection centralConnection) {
			return SetCentralConnection(centralConnection,true);
		}

		///<summary>Sets the current data connection settings of the central manager to the connection settings passed in.  Setting refreshCache to true will cause the entire local cache to get updated with the cache from the connection passed in if the new connection settings are successful.</summary>
		public static bool SetCentralConnection(CentralConnection centralConnection,bool refreshCache) {
			UTF8Encoding enc=new UTF8Encoding();
			byte[] EncryptionKey=enc.GetBytes("mQlEGebnokhGFEFV");//Gotten from FormCentralManager constructor. Only place that does anything like this.
			string computerName="";
			string database="";
			string user="";
			string password="";
			if(centralConnection.ServerName!="") {//Direct connection
				computerName=centralConnection.ServerName;
				database=centralConnection.DatabaseName;
				user=centralConnection.MySqlUser;
				if(centralConnection.MySqlPassword!="") {
					password=CentralConnections.Decrypt(centralConnection.MySqlPassword,EncryptionKey);
				}
				try {
					DataConnection.DBtype=DatabaseType.MySql;
					DataConnection dcon=new DataConnection();
					dcon.SetDbT(computerName,database,user,password,"","",DataConnection.DBtype);
					RemotingClient.SetRemotingRoleT(MiddleTierRole.ClientDirect);
					if(refreshCache) {
						Cache.Refresh(InvalidType.AllLocal);
					}
				}
				catch {
					return false;
				}
			}
			else if(centralConnection.ServiceURI!="") {//Middle tier connection
				RemotingClient.SetServerURIT(centralConnection.ServiceURI);
				RemotingClient.SetRemotingRoleT(MiddleTierRole.ClientMT);
			}
			else {
				MessageBox.Show("Either a database or a Middle Tier URI must be specified in the connection.");
				return false;
			}
			return true;
		}

		///<summary>Correctly formats command line arguments that end with backslashes and need to be surrounded with double quotes. Use encapsulate to surround argValue with double quotes.</summary>
		//Per MSDN, "If a double quotation mark follows two or an even number of backslashes, each proceeding backslash pair is replaced with one backslash and the double quotation mark is removed.
		//If a double quotation mark follows an odd number of backslashes, including just one, each preceding pair is replaced with one backslash and the remaining backslash is removed; however, in this case the double quotation mark is not removed."
		//https://learn.microsoft.com/en-us/dotnet/api/system.environment.getcommandlineargs?view=netframework-4.8#System_Environment_GetCommandLineArgs
		private static string Scrub(string argValue,bool encapsulate=true) {
			if(!encapsulate) {
				return argValue;//If the argValue does not need to be encapsulated, no scrubbing is required.
			}
			if(!argValue.EndsWith("\\")) {
				return "\""+argValue+"\"";
			}
			string scrubbedStr=argValue;
			//Get the number of trailing backslashes and add that many more to the end of the string.
			int backslashCount=0;
			for(int i=argValue.Length-1;i>=0 && argValue[i]=='\\';i--) {
				backslashCount++;
			}
			scrubbedStr=argValue+new string('\\',backslashCount);
			scrubbedStr="\""+scrubbedStr+"\"";
			return scrubbedStr;
		}
	}
}
