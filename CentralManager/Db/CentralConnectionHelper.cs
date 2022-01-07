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
				args+="ServerName=\""+centralConnection.ServerName+"\" "
					+"DatabaseName=\""+centralConnection.DatabaseName+"\" "
					+"MySqlUser=\""+centralConnection.MySqlUser+"\" ";
				if(centralConnection.MySqlPassword!="") {
					args+="MySqlPassword=\""+CentralConnections.Decrypt(centralConnection.MySqlPassword,FormCentralManager.EncryptionKey)+"\" ";
				}
			}
			else if(centralConnection.ServiceURI!="") {
				args+="WebServiceUri=\""+centralConnection.ServiceURI+"\" ";
				if(centralConnection.WebServiceIsEcw) {
					args+="WebServiceIsEcw=True ";
				}
			}
			args+="DynamicMode=\""+(useDynamicMode.ToString())+"\" ";
			return args;
		}

		///<summary>Launches OD.  Sets hWnd and ProcessID.  If this fails to launch, a textbox will appear.</summary>
		public static void LaunchOpenDental(CentralConnection centralConnection,bool useDynamicMode,bool isAutoLogin,bool isDomainLogin,long patNum,ref WindowInfo windowInfo) {
			string args=GetArgsFromConnection(centralConnection,useDynamicMode);
			if(isAutoLogin) {
				args+="UserName=\""+Security.CurUser.UserName+"\" ";
				args+="OdPassword=\""+Security.PasswordTyped+"\" ";
			}
			if(isDomainLogin) {
				args+="DomainUser=\""+Security.CurUser.DomainUser+"\" ";
			}
			if(patNum!=0){
				args+="PatNum="+patNum.ToString();
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
					RemotingClient.SetRemotingRoleT(RemotingRole.ClientDirect);
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
				RemotingClient.SetRemotingRoleT(RemotingRole.ClientWeb);
			}
			else {
				MessageBox.Show("Either a database or a Middle Tier URI must be specified in the connection.");
				return false;
			}
			return true;
		}
	}
}
