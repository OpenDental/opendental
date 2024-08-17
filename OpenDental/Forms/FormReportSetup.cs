using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using System.IO;
using CodeBase;
using DataConnectionBase;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Net;

namespace OpenDental {
	public partial class FormReportSetup:FormODBase {
		private bool _hasChanged;
		///<summary>Either the currently logged in user or the user of a group selected in the Security window.</summary>
		private long _userGroupNum;
		private bool _isPermissionMode;

		public FormReportSetup(long userGroupNum,bool isPermissionMode) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_userGroupNum=userGroupNum;
			_isPermissionMode=isPermissionMode;
		}

		private void FormReportSetup_Load(object sender,EventArgs e) {
			if(!PrefC.HasClinicsEnabled) {
				checkReportPIClinic.Visible=false;
				checkReportPIClinicInfo.Visible=false;
			}
			FillComboReportWriteoff();
			comboReportWriteoff.SelectedIndex=PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate);
			checkProviderPayrollAllowToday.Checked=PrefC.GetBool(PrefName.ProviderPayrollAllowToday);
			checkNetProdDetailUseSnapshotToday.Checked=PrefC.GetBool(PrefName.NetProdDetailUseSnapshotToday);
			checkReportsShowPatNum.Checked=PrefC.GetBool(PrefName.ReportsShowPatNum);
			checkReportProdWO.Checked=PrefC.GetBool(PrefName.ReportPandIschedProdSubtractsWO);
			checkReportPIClinicInfo.Checked=PrefC.GetBool(PrefName.ReportPandIhasClinicInfo);
			checkReportPIClinic.Checked=PrefC.GetBool(PrefName.ReportPandIhasClinicBreakdown);
			checkReportPrintWrapColumns.Checked=PrefC.GetBool(PrefName.ReportsWrapColumns);
			checkReportsShowHistory.Checked=PrefC.GetBool(PrefName.ReportsShowHistory);
			checkReportsIncompleteProcsNoNotes.Checked=PrefC.GetBool(PrefName.ReportsIncompleteProcsNoNotes);
			checkReportsIncompleteProcsUnsigned.Checked=PrefC.GetBool(PrefName.ReportsIncompleteProcsUnsigned);
			checkBenefitAssumeGeneral.Checked=PrefC.GetBool(PrefName.TreatFinderProcsAllGeneral);
			checkOutstandingRpDateTab.Checked=PrefC.GetBool(PrefName.OutstandingInsReportDateFilterTab);
			checkReportDisplayUnearnedTP.Checked=PrefC.GetBool(PrefName.ReportsDoShowHiddenTPPrepayments);
			textIncompleteProcsExcludeCodes.Text=PrefC.GetString(PrefName.ReportsIncompleteProcsExcludeCodes);
			if(ODBuild.IsWeb()) {
				tabControl1.TabPages.Remove(tabReportServer);//Web users can't change their database settings.
			}
			else {
				FillReportServer();
			}
			userControlReportSetup.InitializeOnStartup(true,_userGroupNum,_isPermissionMode);
			if(_isPermissionMode) {
				tabControl1.SelectedIndex=1;
			}
		}

		private void FillReportServer() {
			checkUseReportServer.Checked=PrefC.GetString(PrefName.ReportingServerCompName)!="" || PrefC.GetString(PrefName.ReportingServerURI)!="";
			radioReportServerDirect.Checked=PrefC.GetString(PrefName.ReportingServerURI)=="";
			radioReportServerMiddleTier.Checked=PrefC.GetString(PrefName.ReportingServerURI)!="";
			comboServerName.Text=PrefC.GetString(PrefName.ReportingServerCompName);
			comboDatabase.Text=PrefC.GetString(PrefName.ReportingServerDbName);
			textMysqlUser.Text=PrefC.GetString(PrefName.ReportingServerMySqlUser);
			string decryptedPass;
			CDT.Class1.Decrypt(PrefC.GetString(PrefName.ReportingServerMySqlPassHash),out decryptedPass);
			textMysqlPass.Text=decryptedPass;
			textMysqlPass.PasswordChar='*';
			textMiddleTierURI.Text=PrefC.GetString(PrefName.ReportingServerURI);
			FillComboComputers();
			FillComboDatabases();
			SetReportServerUIEnabled();
		}

		private void FillComboComputers() {
			comboServerName.Items.Clear();
			comboServerName.Items.AddRange(GetComputerNames());
		}

		private void FillComboDatabases() {
			comboDatabase.Items.Clear();
			comboDatabase.Items.AddRange(GetDatabases());
		}

		private void FillComboReportWriteoff() {
			comboReportWriteoff.Items.Clear();
			comboReportWriteoff.Items.AddEnums<PPOWriteoffDateCalc>();
		}

		private void SetReportServerUIEnabled() {
			if(!checkUseReportServer.Checked) {
				radioReportServerDirect.Enabled=false;
				radioReportServerMiddleTier.Enabled=false;
				groupConnectionSettings.Enabled=false;
				groupMiddleTier.Enabled=false;
			}
			else {
				radioReportServerDirect.Enabled=true;
				radioReportServerMiddleTier.Enabled=true;
				if(radioReportServerDirect.Checked) {
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
				}
				if(streamReader is null || streamReader.EndOfStream){
					return new string[0];//helps during debugging
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

		///<summary></summary>
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

		private List<string> GetIncompleteProcsExcludeCodes() {
			return textIncompleteProcsExcludeCodes.Text.Split(",",StringSplitOptions.RemoveEmptyEntries)
				.Select(x => x.Trim())
				.ToList();
		}

		private void checkReportingServer_CheckChanged(object sender,EventArgs e) {
			SetReportServerUIEnabled();
		}

		private void comboDatabase_DropDown(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			FillComboDatabases();
			Cursor=Cursors.Default;
		}

		private void tabControl1_SelectedIndexChanged(object sender,EventArgs e) {
			if(tabControl1.SelectedIndex==0) {
				userControlReportSetup.Parent=tabDisplaySettings;
				userControlReportSetup.InitializeOnStartup(false,_userGroupNum,false);//This will change usergroups when they change tabs.  NOT what we want......
			}
			else if(tabControl1.SelectedIndex==1) {
				if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
					tabControl1.SelectedIndex=0;
					return;
				}
				userControlReportSetup.Parent=tabReportPermissions;
				userControlReportSetup.InitializeOnStartup(false,_userGroupNum,true);
			}
		}

		private bool UpdateReportingServer() {
			bool hasChanged=false;
			if(!checkUseReportServer.Checked) {
				hasChanged |=Prefs.UpdateString(PrefName.ReportingServerCompName,"");
				hasChanged |=Prefs.UpdateString(PrefName.ReportingServerDbName,"");
				hasChanged |=Prefs.UpdateString(PrefName.ReportingServerMySqlUser,"");
				hasChanged |=Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,"");
				hasChanged |=Prefs.UpdateString(PrefName.ReportingServerURI,"");
			}
			else {
				if(radioReportServerDirect.Checked) {
					string encryptedPass;
					CDT.Class1.Encrypt(textMysqlPass.Text,out encryptedPass);
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerCompName,comboServerName.Text);
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerDbName,comboDatabase.Text);
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerMySqlUser,textMysqlUser.Text);
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,encryptedPass);
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerURI,"");
				}
				else {
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerCompName,"");
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerDbName,"");
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerMySqlUser,"");
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,"");
					hasChanged |=Prefs.UpdateString(PrefName.ReportingServerURI,textMiddleTierURI.Text);
				}
			}
			return hasChanged;
		}

		private void butCodePicker_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.CanAllowMultipleSelections=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			List<string> listExcludeCodes=GetIncompleteProcsExcludeCodes();
			listExcludeCodes.AddRange(formProcCodes.ListProcedureCodesSelected.Select(x => x.ProcCode));
			textIncompleteProcsExcludeCodes.Text=string.Join(",",listExcludeCodes);
		}

		private void butOK_Click(object sender,EventArgs e) {
			List<string> listAllExcludeCodes=GetIncompleteProcsExcludeCodes();
			_hasChanged |=Prefs.UpdateInt(PrefName.ReportsPPOwriteoffDefaultToProcDate,comboReportWriteoff.SelectedIndex);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportsShowPatNum,checkReportsShowPatNum.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportPandIschedProdSubtractsWO,checkReportProdWO.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportPandIhasClinicInfo,checkReportPIClinicInfo.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportPandIhasClinicBreakdown,checkReportPIClinic.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ProviderPayrollAllowToday,checkProviderPayrollAllowToday.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.NetProdDetailUseSnapshotToday,checkNetProdDetailUseSnapshotToday.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportsWrapColumns,checkReportPrintWrapColumns.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportsIncompleteProcsNoNotes,checkReportsIncompleteProcsNoNotes.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportsIncompleteProcsUnsigned,checkReportsIncompleteProcsUnsigned.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.TreatFinderProcsAllGeneral,checkBenefitAssumeGeneral.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportsShowHistory,checkReportsShowHistory.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.OutstandingInsReportDateFilterTab,checkOutstandingRpDateTab.Checked);
			_hasChanged |=Prefs.UpdateBool(PrefName.ReportsDoShowHiddenTPPrepayments,checkReportDisplayUnearnedTP.Checked);
			_hasChanged |=Prefs.UpdateString(PrefName.ReportsIncompleteProcsExcludeCodes,string.Join(",",listAllExcludeCodes));
			if(UpdateReportingServer()) {
				ConnectionStoreBase.ClearConnectionDictionary();
				_hasChanged=true;
			}
			if(_hasChanged) {
				DataValid.SetInvalid(InvalidType.Prefs);
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Report settings have been changed.");
			}
			if(Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				if(GroupPermissions.Sync(userControlReportSetup.ListGroupPermissionsForReports,userControlReportSetup.ListGroupPermissionsOld)) {
					SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Report permissions have been changed.");
				}
				DataValid.SetInvalid(InvalidType.Security);
			}
			if(DisplayReports.Sync(userControlReportSetup.ListDisplayReportAll)) {
				DataValid.SetInvalid(InvalidType.DisplayReports);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}