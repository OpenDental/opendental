using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlServerConnections:UserControl {

		#region Fields - Private

		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public bool DoClearConnectionDictionary;
		#endregion Fields - Public

		#region Constructors
		public UserControlServerConnections() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers

		private void butReadOnlyServerSetupDetails_Click(object sender,EventArgs e) {
			string html=@"Note: Open Dental Cloud users cannot change Read-Only Server Setup.
				<br><br>
				If you would like to run certain database processes (read queries and cache refreshes) on a different server and database, enter settings here.
				<br><br>
				This is useful for larger offices that may experience slowness or that want to avoid the possibility of heavy queries locking up the database.
				<br><br>
				See <a href='https://www.opendental.com/manual/troubleshootingslowness.html' target='_blank' rel='noopener noreferrer'>Troubleshooting Slowness</a> for additional information on processes that utilize Read-Only Server Setup.";
			using FormWebBrowserPrefs formWebBrowserPrefs=new FormWebBrowserPrefs();
			formWebBrowserPrefs.HtmlContent=html;
			formWebBrowserPrefs.SizeWindow=new Size(450,250);
			formWebBrowserPrefs.PointStart=PointToScreen(butReadOnlyServerSetupDetails.Location);
			formWebBrowserPrefs.ShowDialog();
		}

		private void butMiddleTierURIDetails_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"The currently logged in user's credentials will be used when accessing the Middle Tier database.");
		}

		private void checkReadOnlyServer_CheckChanged(object sender,EventArgs e) {
			SetReadOnlyServerUIEnabled();
		}

		private void comboDatabase_DropDown(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			FillComboDatabases();
			Cursor=Cursors.Default;
		}

		private void comboComputers_DropDown(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			FillComboComputers();
			Cursor=Cursors.Default;
		}

		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>Fills comboServerName with a list of all computer names on the network.</summary>
		private void FillComboComputers() {
			comboServerName.Items.Clear();
			comboServerName.Items.AddRange(GetComputerNames());
		}

		///<summary>Fills comboDatabase with a list of all database names on the selected server.</summary>
		private void FillComboDatabases() {
			comboDatabase.Items.Clear();
			comboDatabase.Items.AddRange(GetDatabases());
		}

		private void SetReadOnlyServerUIEnabled() {
			if(!checkUseReadOnlyServer.Checked) {
				radioReadOnlyServerDirect.Enabled=false;
				radioReadOnlyServerMiddleTier.Enabled=false;
				groupConnectionSettings.Enabled=false;
				groupMiddleTier.Enabled=false;
			}
			else {
				radioReadOnlyServerDirect.Enabled=true;
				radioReadOnlyServerMiddleTier.Enabled=true;
				if(radioReadOnlyServerDirect.Checked) {
					groupConnectionSettings.Enabled=true;
					groupMiddleTier.Enabled=false;
				}
				else {
					groupConnectionSettings.Enabled=false;
					groupMiddleTier.Enabled=true;
				}
			}
		}

		///<summary>Gets a list of all computer names on the network (this is not easy)</summary>
		private string[] GetComputerNames() {
			if(Environment.OSVersion.Platform==PlatformID.Unix) {
				return new string[0];
			}
			try {
				File.Delete(ODFileUtils.CombinePaths(Application.StartupPath,"tempCompNames.txt"));
				ArrayList retList = new ArrayList();
				//string myAdd=Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();//obsolete
				string myAdd = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName=@"C:\WINDOWS\system32\cmd.exe";//Path for the cmd prompt
				processStartInfo.Arguments="/c net view > tempCompNames.txt";//Arguments for the command prompt
				//"/c" tells it to run the following command which is "net view > tempCompNames.txt"
				//"net view" lists all the computers on the network
				//" > tempCompNames.txt" tells dos to put the results in a file called tempCompNames.txt
				processStartInfo.WindowStyle=ProcessWindowStyle.Hidden;//Hide the window
				Process.Start(processStartInfo);
				StreamReader streamReader = null;
				string filename = ODFileUtils.CombinePaths(Application.StartupPath,"tempCompNames.txt");
				Thread.Sleep(200);//sleep for 1/5 second
				if(!File.Exists(filename)) {
					return new string[0];
				}
				try {
					streamReader=new StreamReader(filename);
				}
				catch(Exception) {
					if(ODBuild.IsDebug()){
						return new string[0];
					}
				}
				while(!streamReader.ReadLine().StartsWith("--")) {
					//The line just before the data looks like: --------------------------
				}
				string line = "";
				retList.Add("localhost");
				while(true) {
					line=streamReader.ReadLine();
					if(line.StartsWith("The"))//cycle until we reach,"The command completed successfully."
					break;
					line=line.Split(char.Parse(" "))[0];// Split the line after the first space
																	// Normally, in the file it lists it like this
																	// \\MyComputer                 My Computer's Description
																	// Take off the slashes, "\\MyComputer" to "MyComputer"
					retList.Add(line.Substring(2,line.Length-2));
				}
				streamReader.Close();
				File.Delete(ODFileUtils.CombinePaths(Application.StartupPath,"tempCompNames.txt"));
				string[] retArray = new string[retList.Count];
				retList.CopyTo(retArray);
				return retArray;
			}
			catch(Exception) {//it will always fail if not WinXP
				return new string[0];
			}
		}

		///<summary>Get the Databases available based on the server/computer selected from comboServerName. Returns empty array if comboServerName.Text is empty.</summary>
		private string[] GetDatabases() {
			if(comboServerName.Text=="") {
				return new string[0];
			}
			try {
				DataConnection dcon;
				//use the one table that we know exists
				if(textMysqlUser.Text=="") {
					dcon=new DataConnection(comboServerName.Text,"mysql","root",textMysqlPass.Text,DatabaseType.MySql);
				}
				else {
					dcon=new DataConnection(comboServerName.Text,"mysql",textMysqlUser.Text,textMysqlPass.Text,DatabaseType.MySql);
				}
				string command = "SHOW DATABASES";
				//if this next step fails, table will simply have 0 rows
				DataTable table = dcon.GetTable(command,false);
				string[] dbNames = new string[table.Rows.Count];
				for(int i = 0;i<table.Rows.Count;i++) {
					dbNames[i]=table.Rows[i][0].ToString();
				}
				return dbNames;
			}
			catch(Exception) {
				return new string[0];
			}
		}

		#endregion Methods - Private

		#region Methods - Public
		public void FillServerConnections() {
			if(ODBuild.IsWeb()) {//Web users can't change their database settings.
				checkUseReadOnlyServer.Enabled=false;
				groupBoxReadOnlyServerSetup.Enabled=false;
			}
			else {
				checkUseReadOnlyServer.Checked=PrefC.GetString(PrefName.ReadOnlyServerCompName)!="" || PrefC.GetString(PrefName.ReadOnlyServerURI)!="";
				radioReadOnlyServerDirect.Checked=PrefC.GetString(PrefName.ReadOnlyServerURI)=="";
				radioReadOnlyServerMiddleTier.Checked=PrefC.GetString(PrefName.ReadOnlyServerURI)!="";
				comboServerName.Text=PrefC.GetString(PrefName.ReadOnlyServerCompName);
				comboDatabase.Text=PrefC.GetString(PrefName.ReadOnlyServerDbName);
				textMysqlUser.Text=PrefC.GetString(PrefName.ReadOnlyServerMySqlUser);
				string decryptedPass;
				CDT.Class1.Decrypt(PrefC.GetString(PrefName.ReadOnlyServerMySqlPassHash),out decryptedPass);
				textMysqlPass.Text=decryptedPass;
				textMysqlPass.PasswordChar='*';
				textMiddleTierURI.Text=PrefC.GetString(PrefName.ReadOnlyServerURI);
				FillComboComputers();
				FillComboDatabases();
				SetReadOnlyServerUIEnabled();
			}

		}

		public bool SaveServerConnections() {
			if(!checkUseReadOnlyServer.Checked) {
				Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerCompName,"");
				Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerDbName,"");
				Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerMySqlUser,"");
				Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerMySqlPassHash,"");
				Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerURI,"");
			}
			else {
				if(radioReadOnlyServerDirect.Checked) {
					string encryptedPass;
					CDT.Class1.Encrypt(textMysqlPass.Text,out encryptedPass);
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerCompName,comboServerName.Text);
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerDbName,comboDatabase.Text);
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerMySqlUser,textMysqlUser.Text);
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerMySqlPassHash,encryptedPass);
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerURI,"");
				}
				else {
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerCompName,"");
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerDbName,"");
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerMySqlUser,"");
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerMySqlPassHash,"");
					Changed |=Prefs.UpdateString(PrefName.ReadOnlyServerURI,textMiddleTierURI.Text);
				}
			}
			if(Changed) {
				DoClearConnectionDictionary=true;
			}
			return true;
		}
		#endregion Methods - Public

	}
}
