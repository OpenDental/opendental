using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using OpenDental;
using CodeBase;
using DataConnectionBase;
using System.Globalization;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;

namespace CentralManager {
	public partial class FormCentralManager:Form {
		#region Fields - Public
		public static byte[] EncryptionKey;
		#endregion Fields - Public

		#region Fields - Private
		/// <summary>Dataset containing tables of patients for each connection.</summary>
		private DataSet _dataSetPats=new DataSet();
		private bool _hasFatalError;
		private bool _usingMiddleTier;
		private bool _usingAutoLogin=false;
		private string _invalidConnsLog="";
		private string _middleTierAddr;
		private List<ConnectionGroup> _listConnectionGroups;
		///<summary>A full list of connections.</summary>
		private List<CentralConnection> _listConnsAll=new List<CentralConnection>();
		///<summary>List of connections with a status of OK.</summary>
		private List<CentralConnection> _listConnsOK;
		private List<DisplayReport> _listDisplayReports_ProdInc;
		private List<GroupPermission> _listGroupPermissions_Reports;
		///<summary>This is a list of info about OD windows that are open and that we are controlling.  They are "refreshed" separately, including with a subsecond timer.  This list is kept very current, and then used as read-only during fillGrid.</summary>
		private List<WindowInfo> _listWindowInfos=new List<WindowInfo>();
		private object _lockObj=new object();
		private string _progVersion;
		#endregion Fields - Private

		#region Constructor
		public FormCentralManager() {
			InitializeComponent();
			UTF8Encoding enc=new UTF8Encoding();
			EncryptionKey=enc.GetBytes("mQlEGebnokhGFEFV");
			this.Menu=mainMenu;
		}
		#endregion Constructor

		#region Events
		private void butFilter_Click(object sender,EventArgs e) {
			if(textProviderSearch.Text=="" && textClinicSearch.Text=="") {
				FillGridConns();
				return;
			}
			for(int i=0;i<gridConns.ListGridRows.Count;i++) {
				CentralConnection conn=(CentralConnection)gridConns.ListGridRows[i].Tag;
				if(conn.ConnectionStatus!="OK") {
					continue;
				}
				ODThread odThread=new ODThread(ConnectAndFilter,new object[] { conn });
				odThread.Name="SearchThread"+i;
				odThread.GroupName="Search";
				odThread.Start(false);
			}
			ODThread.JoinThreadsByGroupName(Timeout.Infinite,"Search");
			List<ODThread> listThreads=ODThread.GetThreadsByGroupName("Search");
			List<long> listConnNums=new List<long>();
			for(int i=0;i<listThreads.Count;i++) {
				object[] obj=(object[])listThreads[i].Tag;
				CentralConnection conn=(CentralConnection)obj[0];
				bool result=(bool)obj[1];
				if(result) {
					listConnNums.Add(conn.CentralConnectionNum);
				}
				listThreads[i].QuitAsync();
			}
			FillGridConns(listConnNums);
		}

		private void butRefreshStatuses_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			if(gridConns.SelectedIndices.Length==0) {
				gridConns.SetAll(true);
			}
			for(int i=0;i<gridConns.SelectedIndices.Length;i++) {
				CentralConnection conn=(CentralConnection)gridConns.ListGridRows[gridConns.SelectedIndices[i]].Tag;
				if(conn.DatabaseName=="" && conn.ServerName=="" && conn.ServiceURI=="") {
					continue;
				}
				ODThread odThread=new ODThread(ConnectAndVerify,new object[] { conn,_progVersion });
				odThread.Name="VerifyThread"+i;
				odThread.GroupName="Verify";
				odThread.Start(false);
			}
			ODThread.JoinThreadsByGroupName(Timeout.Infinite,"Verify");
			List<ODThread> listComplThreads=ODThread.GetThreadsByGroupName("Verify");
			for(int i=0;i<listComplThreads.Count;i++) {
				object[] obj=(object[])listComplThreads[i].Tag;
				CentralConnection conn=((CentralConnection)obj[0]);
				string status=((string)obj[1]);
				CentralConnection connection=_listConnsAll.Find(x => x.CentralConnectionNum==conn.CentralConnectionNum);
				connection.ConnectionStatus=status;
				eServiceSignalSeverity severity=((eServiceSignalSeverity)obj[2]);
				connection.EServicesSignalSeverity=severity;
			}
			ODThread.QuitSyncThreadsByGroupName(100,"Verify");
			Cursor=Cursors.Default;
			FillGridConns();
		}

		private string GetConnectionName(CentralConnection connection) {
			string connName;
			if(connection.DatabaseName=="") {//uri
				connName=connection.ServiceURI;
			}
			else {
				connName=connection.ServerName+", "+connection.DatabaseName;
			}
			return connName;
		}

		private bool ConnectionStatusErrorGenerated(CentralConnection connection) {
			string connName=GetConnectionName(connection);
			string connectionPrefix="\r\n"+connName+": ";
			//Check if the connection status is neither OK nor a version number (indicating a different version than the CEMT one)
			if(connection.ConnectionStatus!="OK" && !Regex.IsMatch(connection.ConnectionStatus,@"^[0-9]+[.][0-9]+[.][0-9]+[.][0-9]+$")) {
				lock(_lockObj) {
					_invalidConnsLog+=connectionPrefix+Lan.g(this,"Skipped due to connection status.")+"\r\n";
				}
				return true;
			}
			return false;
		}

		private void butSearchPats_Click(object sender,EventArgs e) {
			_listConnsOK=new List<CentralConnection>();
			lock(_lockObj) {
				_invalidConnsLog="";
			}
			if(gridConns.SelectedIndices.Length==0){
				for(int i=0;i<gridConns.ListGridRows.Count;i++) {
					CentralConnection connection=(CentralConnection)gridConns.ListGridRows[i].Tag;
					if(ConnectionStatusErrorGenerated(connection)) {
						continue;
					}
					_listConnsOK.Add((CentralConnection)gridConns.ListGridRows[i].Tag);
				}
			}
			else{
				for(int i=0;i<gridConns.SelectedIndices.Length;i++) {
					CentralConnection connection=(CentralConnection)gridConns.ListGridRows[gridConns.SelectedIndices[i]].Tag;
					if(ConnectionStatusErrorGenerated(connection)) {
						continue;
					}
					_listConnsOK.Add((CentralConnection)gridConns.ListGridRows[gridConns.SelectedIndices[i]].Tag);
				}
			}
			ODThread.JoinThreadsByGroupName(1,"FetchPats");//Stop fetching immediately
			if(butSearchPats.Text!=Lans.g(this,"Search")) {//in middle of previous search
				butSearchPats.Text=Lans.g(this,"Search");
				labelFetch.Visible=false;
				return;
			}
			Cursor=Cursors.WaitCursor;
			//_dataSetPats.Clear();
			butSearchPats.Text=Lans.g(this,"Stop Search");
			labelFetch.Visible=true;
			//Loops through all connections passed in and spawns a thread for each to go fetch patient data from each db using the given filters.
			//StartThreadsForConns();
			_dataSetPats.Tables.Clear();
			for(int i=0;i<_listConnsOK.Count;i++) {
				//Filter the threads by their connection name
				string connName=GetConnectionName(_listConnsOK[i]);
				if(!connName.Contains(textConnPatSearch.Text)) {
					//Do NOT spawn a thread to go fetch data for this connection because the user has filtered it out.
					//Increment the completed thread count and continue.
					continue;
				}
				//At this point we know the connection has not been filtered out, so fire up a thread to go get the patient data table for the search.
				ODThread odThread=new ODThread(GetDataTablePatForConn,new object[]{_listConnsOK[i]});
				odThread.GroupName="FetchPats";
				odThread.Start();
			}
			ODThread.JoinThreadsByGroupName(Timeout.Infinite,"FetchPats");
			FillGridPats();
			butSearchPats.Text=Lans.g(this,"Search");
			labelFetch.Visible=false;

			Cursor=Cursors.Default;
			if(_invalidConnsLog!="") {
				using OpenDental.MsgBoxCopyPaste msgBoxCopyPaste=new OpenDental.MsgBoxCopyPaste(Lan.g(this,"The following connections had errors occur")
					+":"+_invalidConnsLog);
				msgBoxCopyPaste.ShowDialog();
			}
		}

		private void comboConnectionGroups_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridConns();
		}

		private void FormCentralManager_FormClosing(object sender,FormClosingEventArgs e) {
			if(_hasFatalError) {
				return;//Don't do any of the below
			}
			ODThread.QuitSyncAllOdThreads();
			foreach(CentralConnection conn in _listConnsAll) {//Reflect connection status changes in the database.
				CentralConnections.UpdateStatus(conn);
			}
		}

		private void FormCentralManager_Load(object sender,EventArgs e) {
			if(!GetConfigAndConnect()) {
				return;
			}
			string syncCodePref=PrefC.GetString(PrefName.CentralManagerSyncCode);
			if(syncCodePref=="") {
				//Generate new sync code of 10 alphanumeric characters.
				Random rand=new Random();
				string allowedChars="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
				for(int i=0;i<10;i++) {
					syncCodePref+=allowedChars[rand.Next(allowedChars.Length)];
				}
				Prefs.UpdateString(PrefName.CentralManagerSyncCode,syncCodePref);
			}
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				CultureInfo cInfo=(CultureInfo)CultureInfo.CurrentCulture.Clone();
				cInfo.DateTimeFormat.ShortDatePattern="MM/dd/yyyy";
				Application.CurrentCulture=cInfo;
			}
			if(_usingMiddleTier) {
				menuItemUserSettings.Visible = true;
			}
//todo: this shouldn't be necessary, and I'm wondering if there are other caches that need to be refreshed.
			DisplayFields.RefreshCache();
			this.Text+=" - "+Security.CurUser.UserName;
			FillComboGroups(PrefC.GetLong(PrefName.ConnGroupCEMT));
			_listConnsAll=CentralConnections.GetConnections();
			_progVersion=PrefC.GetString(PrefName.ProgramVersion);
			labelVersion.Text="Version: "+_progVersion;
			FillGridConns();
			_listGroupPermissions_Reports=GroupPermissions.GetPermsForReports().Where(x => Security.CurUser.IsInUserGroup(x.UserGroupNum)).ToList();
			_listDisplayReports_ProdInc=DisplayReports.GetForCategory(DisplayReportCategory.ProdInc,false);
		}

		private void gridConns_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			CentralConnection conn=(CentralConnection)gridConns.ListGridRows[e.Row].Tag;
			LaunchConnection(conn);
		}

		private void gridPats_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DataRow dataRow=(DataRow)gridPats.ListGridRows[e.Row].Tag;
			CentralConnection conn=_listConnsOK.Find(x => (x.ServerName+", "+x.DatabaseName)==dataRow["Conn"].ToString() || x.ServiceURI==dataRow["Conn"].ToString());
			long patNum=PIn.Long(dataRow["PatNum"].ToString());
			LaunchConnection(conn,patNum);
		}

		private void timer1_Tick(object sender, EventArgs e){
			//every 500ms
			//TimeSpan timeStart=DateTime.Now.TimeOfDay;
			bool changed=false;
			IntPtr hWnd=IntPtr.Zero;
			ShowState showState=ShowState.SW_SHOW;
			for(int i=_listWindowInfos.Count-1;i>=0;i--){//loop backward so we can remove bad ones
				//we try to fix zero here
				if(_listWindowInfos[i].HWnd==IntPtr.Zero){
					List<IntPtr> listVisibleWindows=NativeHelpers.GetAllVisibleWindows();//approx 50
					hWnd=GetHwndForProcess(_listWindowInfos[i].ProcessId,listVisibleWindows);
					if(hWnd==IntPtr.Zero){
						continue;
					}
					else{
						_listWindowInfos[i].HWnd=hWnd;
						changed|=true;
					}
				}
				try{
					showState=GetWindowState(_listWindowInfos[i].HWnd);
					if(showState==ShowState.SW_HIDE){//window is gone
						_listWindowInfos.Remove(_listWindowInfos[i]);
						changed|=true;
						continue;
					}
				}
				catch{
					_listWindowInfos.Remove(_listWindowInfos[i]);
					changed|=true;
					continue;
				}
				if(NativeHelpers.IsMinimized(showState)){
					if(!_listWindowInfos[i].IsMinimized){
						_listWindowInfos[i].IsMinimized=true;
						changed|=true;
					}
				}
				else if(NativeHelpers.IsMaximized(showState)){
					if(!_listWindowInfos[i].WasMaximized){
						_listWindowInfos[i].WasMaximized=true;
						changed|=true;
					}
					if(_listWindowInfos[i].IsMinimized){
						_listWindowInfos[i].IsMinimized=false;
						changed|=true;
					}
				}
				else{//normal state
					if(_listWindowInfos[i].WasMaximized){
						_listWindowInfos[i].WasMaximized=false;
						changed|=true;
					}
					if(_listWindowInfos[i].IsMinimized){
						_listWindowInfos[i].IsMinimized=false;
						changed|=true;
					}
				}
			}
			//TimeSpan timeDelta=DateTime.Now.TimeOfDay-timeStart;
			//Debug.WriteLine(timeDelta.TotalMilliseconds.ToString());
			//Metrics: this method usually takes 0ms.  10% of time: .25ms. 1% of time: 1ms.
			//So there is essentially no cost, and running it every 500 ms is totally harmless.
			if(changed){
				FillGridConns();
			}
		}

		#endregion Events

		#region Events - Menu
		private void menuItemLogoff_Click(object sender,EventArgs e) {
			using FormCentralLogOn FormCLO=new FormCentralLogOn();
			if(FormCLO.ShowDialog()!=DialogResult.OK) {
				Application.Exit();
				return;
			}
			this.Text="Central Manager - "+Security.CurUser.UserName;
		}
		
		private void menuItemPassword_Click(object sender,EventArgs e) {
			using FormCentralUserPasswordEdit FormCPE=new FormCentralUserPasswordEdit(false,Security.CurUser.UserName);
			FormCPE.IsInSecurityWindow=false;
			if(FormCPE.ShowDialog()==DialogResult.Cancel){
				return;
			}
			try {
				Security.CurUser.IsPasswordResetRequired=false;
				Userods.Update(Security.CurUser);
				Userods.UpdatePassword(Security.CurUser,FormCPE.LoginDetails,false,includeCEMT:true); ;
				Security.PasswordTyped=FormCPE.PasswordTyped;//Update the last typed in for middle tier refresh
				Security.CurUser=Userods.GetUserNoCache(Security.CurUser.UserNum);//UpdatePassword() changes multiple fields.  Refresh from db.
			}
			catch(Exception ex) {
				OpenDental.MessageBox.Show(ex.Message);
			}
			Cache.Refresh(InvalidType.Security);
		}

		private void menuItemUserSettings_Click(object sender,EventArgs e) {
			using FormCentralUserSettings FormCUserSettings=new FormCentralUserSettings(_middleTierAddr,_usingAutoLogin);
			if(FormCUserSettings.ShowDialog()==DialogResult.Cancel) {
				return;
			}
			_usingAutoLogin=FormCUserSettings.UsingAutoLogin;
		}

		private void menuTransferPatient_Click(object sender,EventArgs e) {
			if(gridConns.SelectedIndices.Count()!=1) {
				MsgBox.Show(this,"Please select one and only one connection to start transfer.");
				return;
			}
			//long selectedCentralConnNum=gridConns.SelectedTag<CentralConnection>().CentralConnectionNum;
			//RefreshConnections(true);
			//CentralConnection connSelected=gridConns.GetTags<CentralConnection>().FirstOrDefault(x => x.CentralConnectionNum==selectedCentralConnNum);
			CentralConnection connSelected=gridConns.SelectedTag<CentralConnection>();
			if(!connSelected.IsConnectionValid()) {
				MsgBox.Show(this,"Server Offline.  Fix connection and check status again to connect.");
				return;
			}
			using FormCentralPatientTransfer FormCentralConnectionPatientTransfer=new FormCentralPatientTransfer(connSelected,
				_listConnsAll.Where(x=>x.IsConnectionValid()).ToList());
			FormCentralConnectionPatientTransfer.ShowDialog();
		}
		#endregion Events - Menu

		#region Events - Menu Setup
		private void menuConnSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormCentralConnections FormCC=new FormCentralConnections();
			FormCC.ShowDialog();
			//if(FormCC.DialogResult!=DialogResult.OK) {//result is always Cancel because there is no OK button
			_listConnsAll=CentralConnections.GetConnections();
			FillGridConns();
		}

		private void menuGroups_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			ConnectionGroup connGroupCur=null;
			if(comboConnectionGroups.SelectedIndex>0) {
				connGroupCur=_listConnectionGroups[comboConnectionGroups.SelectedIndex-1];
			}
			using FormCentralConnectionGroups FormCCG=new FormCentralConnectionGroups();
			FormCCG.ShowDialog();
			FillComboGroups(connGroupCur==null ? 0 : connGroupCur.ConnectionGroupNum);//Reselect the connection group that the user had before.
			FillGridConns();
		}

		private void menuItemReportSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormCentralReportSetup FormCRS=new FormCentralReportSetup(Security.CurUser.UserNum,true);
			FormCRS.ShowDialog();
		}

		private void menuItemSecurity_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			using FormCentralSecurity FormCUS=new FormCentralSecurity();
			foreach(CentralConnection conn in _listConnsAll) {
				FormCUS.ListConns.Add(conn.Copy());
			}
			FormCUS.ShowDialog();
			_listConnsAll=CentralConnections.GetConnections();//List may have changed when syncing security settings.
			FillGridConns();
		}
				
		private void menuItemDisplayFields_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDisplayFieldCategories FormD=new FormDisplayFieldCategories(true);
			FormD.ShowDialog();
			DisplayFields.RefreshCache();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Display Fields");
		}
		#endregion Events - Menu Setup

		#region Events - Menu Reports
		private void menuProdInc_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Reports)) {
				return;
			}
			if(!_listGroupPermissions_Reports.Exists(x => x.FKey==_listDisplayReports_ProdInc.FirstOrDefault(y => 
				y.InternalName==DisplayReports.ReportNames.ODMoreOptions)?.DisplayReportNum)) 
			{
				MsgBox.Show(this,"You do not have the 'More Options' report permission.");
				return;
			}
			if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				MsgBox.Show(this,"The current user needs to have the 'All Providers' permission for this report");
				return;
			}
			List<CentralConnection> listSelectedConn=new List<CentralConnection>();
			for(int i=0;i<gridConns.SelectedIndices.Length;i++) {
				listSelectedConn.Add((CentralConnection)gridConns.ListGridRows[gridConns.SelectedIndices[i]].Tag);//The tag of this grid is the CentralConnection object
			}
			if(listSelectedConn.Count==0) {
				MsgBox.Show(this,"Please select at least one connection to run this report against.");
				return;
			}
			foreach(CentralConnection conn in listSelectedConn) {
				if(conn.ConnectionStatus.Contains("OFFLINE")) {
					MsgBox.Show(this,"One or more connections are offline. Please remove the offline connection to run this report.");
					return;
				}
			}
			using FormCentralProdInc FormCPI=new FormCentralProdInc();
			FormCPI.ConnList=listSelectedConn;
			FormCPI.EncryptionKey=EncryptionKey;
			FormCPI.ShowDialog();
		}

		#endregion Events - Menu Reports

		#region Methods
		private void ConnectAndFilter(ODThread odThread) {
			CentralConnection connection=(CentralConnection)odThread.Parameters[0];
			if(!CentralConnectionHelper.SetCentralConnection(connection,false)) {//No updating the cache since we're going to be connecting to multiple remote servers at the same time.
				odThread.Tag=new object[] { connection,false };//Can't connect, just return false to not include the connection.
				connection.ConnectionStatus="OFFLINE";
				return;
			}
			List<Provider> listProvs=Providers.GetProvsNoCache();
			//If clinic and provider are both entered is it good enough to find one match among the two?  I'm going to assume yes for now, and maybe later
			//if we decide that if both boxes have something entered that both entries need to be in the db we're searching I can change it.
			bool provMatch=false;
			for(int i=0;i<listProvs.Count;i++) {
				if(textProviderSearch.Text=="") {
					provMatch=true;
					break;
				}
				if(listProvs[i].Abbr.ToLower().Contains(textProviderSearch.Text.ToLower())
					|| listProvs[i].LName.ToLower().Contains(textProviderSearch.Text.ToLower())
					|| listProvs[i].FName.ToLower().Contains(textProviderSearch.Text.ToLower())) 
				{
					provMatch=true;
					break;
				}
			}
			List<Clinic> listClinics=Clinics.GetClinicsNoCache();
			bool clinMatch=false;
			for(int i=0;i<listClinics.Count;i++) {
				if(textClinicSearch.Text=="") {
					clinMatch=true;
					break;
				}
				if(listClinics[i].Description.ToLower().Contains(textClinicSearch.Text.ToLower())) {
					clinMatch=true;
					break;
				}
			}
			if(clinMatch && provMatch) {
				odThread.Tag=new object[] { connection,true };//Match found
			}
			else {
				odThread.Tag=new object[] { connection,false };//No match found
			}
		}

		private void ConnectAndVerify(ODThread odThread) {
			CentralConnection connection=(CentralConnection)odThread.Parameters[0];
			try {
				string progVersion=(string)odThread.Parameters[1];
				if(!CentralConnectionHelper.SetCentralConnection(connection,false)) {
					odThread.Tag=new object[] { connection,"OFFLINE",eServiceSignalSeverity.None };//Can't connect
					return;
				}
				if(!string.IsNullOrEmpty(connection.ServiceURI)) {
					//Middle tier connection. Make sure the current user and password are valid.
					Security.LogInWeb(Security.CurUser.UserName,Security.PasswordTyped,"","",connection.WebServiceIsEcw);
				}
				string progVersionRemote=PrefC.GetStringNoCache(PrefName.ProgramVersion);
				string err="";
				if(progVersionRemote!=progVersion) {
					err=progVersionRemote;
				}
				else {
					err="OK";
				}
				eServiceSignalSeverity severity=EServiceSignals.GetListenerServiceStatus();
				odThread.Tag=new object[] { connection,err,severity };
			}
			catch(Exception ex) {
				ex.DoNothing();
				odThread.Tag=new object[] { connection,"OFFLINE: "+StringTools.Truncate(ex.Message,100),eServiceSignalSeverity.None };//Can't connect
			}
		}

		///<summary>Refreshes _listConnectionGroups with the current cache (could have been updated) and then selects the conn group passed in.</summary>
		private void FillComboGroups(long connGroupNum) {
			_listConnectionGroups=ConnectionGroups.GetAll();
			comboConnectionGroups.Items.Clear();
			comboConnectionGroups.Items.Add("All");
			comboConnectionGroups.SelectedIndex=0;
			for(int i=0;i<_listConnectionGroups.Count;i++) {
				comboConnectionGroups.Items.Add(_listConnectionGroups[i].Description);
				if(_listConnectionGroups[i].ConnectionGroupNum==connGroupNum) {
					comboConnectionGroups.SelectedIndex=i+1;//0 is "All"
				}
			}
		}

		///<summary>listConnNums is only used when pushing the Filter button.  The filter goes away on the next refresh, which is slightly odd.</summary>
		private void FillGridConns(List<long> listConnNums=null) {
			List<CentralConnection> listConnsFiltered=null;
			if(comboConnectionGroups.SelectedIndex>0) {
				listConnsFiltered=CentralConnections.FilterConnections(_listConnsAll,textConnSearch.Text,_listConnectionGroups[comboConnectionGroups.SelectedIndex-1]);
			}
			else {
				listConnsFiltered=CentralConnections.FilterConnections(_listConnsAll,textConnSearch.Text,null);
			}
			gridConns.BeginUpdate();
			gridConns.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("#",20);
			col.SortingStrategy=GridSortingStrategy.AmountParse;
			gridConns.ListGridColumns.Add(col);
			col=new GridColumn("Status",70,HorizontalAlignment.Center);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridConns.ListGridColumns.Add(col);
			col=new GridColumn("eConnector",80,HorizontalAlignment.Center);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridConns.ListGridColumns.Add(col);
			col=new GridColumn("Window",70,HorizontalAlignment.Center);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridConns.ListGridColumns.Add(col);
			col=new GridColumn("Database",210);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridConns.ListGridColumns.Add(col);
			col=new GridColumn("Note",210);
			col.SortingStrategy=GridSortingStrategy.StringCompare;
			gridConns.ListGridColumns.Add(col);
			gridConns.ListGridRows.Clear();
			GridRow row;
			GridCell cell;
			for(int i=0;i<listConnsFiltered.Count;i++) {
				if(listConnNums!=null && !listConnNums.Contains(listConnsFiltered[i].CentralConnectionNum)) {
					continue; //We only want certain connections showing up in the grid.
				}
				string status=listConnsFiltered[i].ConnectionStatus;
				row=new GridRow();
				row.Cells.Add(listConnsFiltered[i].ItemOrder.ToString());
				cell=new GridCell();
				if(status=="") {
					cell.Text="Not checked";
					cell.ColorText=Color.DarkGoldenrod;
				}
				else if(status=="OK") {
					cell.Text=status;
					cell.ColorText=Color.Green;
					cell.Bold=YN.Yes;
				}
				else if(status.StartsWith("OFFLINE")) {
					cell.Text=status;
					row.Bold=true;
					row.ColorText=Color.Red;
				}
				else {
					cell.Text=status;
					cell.ColorText=Color.Red;
				}
				row.Cells.Add(cell);
				cell=new GridCell();
				cell.Text=listConnsFiltered[i].EServicesSignalSeverity.ToString();
				switch(listConnsFiltered[i].EServicesSignalSeverity) {
					case eServiceSignalSeverity.Info:
					case eServiceSignalSeverity.Working:
						cell.ColorText=Color.Green;
						break;
					case eServiceSignalSeverity.Warning:
						cell.ColorText=Color.DarkGoldenrod;
						break;
					case eServiceSignalSeverity.Error:
						cell.ColorText=Color.Orange;
						break;
					case eServiceSignalSeverity.Critical:
						cell.ColorText=Color.Red;
						break;
					default: //Covers None and NotEnabled
						break;
				}
				row.Cells.Add(cell);
				//not refreshed during FillGrid.  
				//Todo: FillGrid will be triggered if any windowInfo changes.
				WindowInfo windowInfo=_listWindowInfos.FirstOrDefault(x=>x.CentralConnectionNum==listConnsFiltered[i].CentralConnectionNum);
				if(windowInfo==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(windowInfo.GetStringState());
				}
				if(listConnsFiltered[i].DatabaseName=="") {//uri
					row.Cells.Add(listConnsFiltered[i].ServiceURI);
				}
				else {
					row.Cells.Add(listConnsFiltered[i].ServerName+", "+listConnsFiltered[i].DatabaseName);
				}
				row.Cells.Add(listConnsFiltered[i].Note);
				
				row.Tag=listConnsFiltered[i];
				gridConns.ListGridRows.Add(row);
			}
			gridConns.EndUpdate();
		}

		private void FillGridPats() {
			if(_dataSetPats.Tables.Count==0){
				gridPats.BeginUpdate();
				gridPats.ListGridColumns.Clear();
				gridPats.ListGridRows.Clear();
				gridPats.EndUpdate();
				return;
			}
			//create a single table so that we can sort.
			//An alternative would be a list of DataRows.
			//List<DataRow> listRows=new List<DataRow>();
			DataTable table=_dataSetPats.Tables[0].Clone();//just structure, no data
			table.Columns.Add("Conn");
			for(int i=0;i<_dataSetPats.Tables.Count;i++) {
				_dataSetPats.Tables[i].Columns.Add("Conn");
				for(int j=0;j<_dataSetPats.Tables[i].Rows.Count;j++) {
					_dataSetPats.Tables[i].Rows[j]["Conn"]=_dataSetPats.Tables[i].TableName;
				}
				table.Merge(_dataSetPats.Tables[i]);
			}
			DataView dataView=table.DefaultView;
			dataView.Sort="LName,FName";
			table=dataView.ToTable();
			gridPats.BeginUpdate();
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.CEMTSearchPatients);
			gridPats.ListGridColumns.Clear();
			foreach(DisplayField field in fields) {
				string heading=field.InternalName;
				if(!string.IsNullOrEmpty(field.Description)) {
					heading=field.Description;
				}
				gridPats.ListGridColumns.Add(new GridColumn(heading,field.ColumnWidth));
			}
			gridPats.ListGridRows.Clear();
			GridRow gridRow;
			for(int i=0;i<table.Rows.Count;i++) {
				gridRow=new GridRow();
				foreach(DisplayField field in fields) {
					switch(field.InternalName) {
						#region Row Cell Filling
						case "Conn":
							gridRow.Cells.Add(table.Rows[i]["Conn"].ToString());
							break;
						case "PatNum":
							gridRow.Cells.Add(table.Rows[i]["PatNum"].ToString());
							break;
						case "LName":
							gridRow.Cells.Add(table.Rows[i]["LName"].ToString());
							break;
						case "FName":
							gridRow.Cells.Add(table.Rows[i]["FName"].ToString());
							break;
						case "SSN":
							gridRow.Cells.Add(table.Rows[i]["SSN"].ToString());
							break;
						case "PatStatus":
							gridRow.Cells.Add(table.Rows[i]["PatStatus"].ToString());
							break;
						case "Age":
							gridRow.Cells.Add(table.Rows[i]["age"].ToString());
							break;
						case "City":
							gridRow.Cells.Add(table.Rows[i]["City"].ToString());
							break;
						case "State":
							gridRow.Cells.Add(table.Rows[i]["State"].ToString());
							break;
						case "Address":
							gridRow.Cells.Add(table.Rows[i]["Address"].ToString());
							break;
						case "Wk Phone":
							gridRow.Cells.Add(table.Rows[i]["WkPhone"].ToString());
							break;
						case "Email":
							gridRow.Cells.Add(table.Rows[i]["Email"].ToString());
							break;
						case "ChartNum":
							gridRow.Cells.Add(table.Rows[i]["ChartNumber"].ToString());
							break;
						case "MI":
							gridRow.Cells.Add(table.Rows[i]["MiddleI"].ToString());
							break;
						case "Pref Name":
							gridRow.Cells.Add(table.Rows[i]["Preferred"].ToString());
							break;
						case "Hm Phone":
							gridRow.Cells.Add(table.Rows[i]["HmPhone"].ToString());
							break;
						case "Bill Type":
							gridRow.Cells.Add(table.Rows[i]["BillingType"].ToString());
							break;
						case "Pri Prov":
							gridRow.Cells.Add(table.Rows[i]["PriProv"].ToString());
							break;
						case "Birthdate":
							gridRow.Cells.Add(table.Rows[i]["Birthdate"].ToString());
							break;
						case "Site":
							gridRow.Cells.Add(table.Rows[i]["site"].ToString());
							break;
						case "Clinic":
							gridRow.Cells.Add(table.Rows[i]["clinic"].ToString());
							break;
						case "Wireless Ph":
							gridRow.Cells.Add(table.Rows[i]["WirelessPhone"].ToString());
							break;
						case "Sec Prov":
							gridRow.Cells.Add(table.Rows[i]["SecProv"].ToString());
							break;
						case "LastVisit":
							gridRow.Cells.Add(table.Rows[i]["lastVisit"].ToString());
							break;
						case "NextVisit":
							gridRow.Cells.Add(table.Rows[i]["nextVisit"].ToString());
							break;
						case "Country":
							gridRow.Cells.Add(table.Rows[i]["Country"].ToString());
							break;
						#endregion
					}
				}
				gridRow.Tag=table.Rows[i];
				gridPats.ListGridRows.Add(gridRow);
			}
			gridPats.EndUpdate();
		}

		///<summary>Gets the settings from the config file and attempts to connect.</summary>
		private bool GetConfigAndConnect() {
			string xmlPath=Path.Combine(Application.StartupPath,"CentralManagerConfig.xml");
			if(!File.Exists(xmlPath)) {
				OpenDental.MessageBox.Show("Please create CentralManagerConfig.xml according to the manual before using this tool.");
				_hasFatalError=true;
				Application.Exit();
				return false;
			}
			XmlDocument document=new XmlDocument();
			string computerName="";
			string database="";
			string mySqlUser="";
			string autoLoginUser="";
			string mySqlPassword="";
			try {
				document.Load(xmlPath);
				XPathNavigator Navigator=document.CreateNavigator();
				XPathNavigator nav;
				DataConnection.DBtype=DatabaseType.MySql;	
				//See if there's a DatabaseConnection
				nav=Navigator.SelectSingleNode("//DatabaseConnection");
				if(nav==null) {
					OpenDental.MessageBox.Show("DatabaseConnection element missing from CentralManagerConfig.xml.");
					Application.Exit();
					return false;
				}
				computerName=nav.SelectSingleNode("ComputerName").Value;
				database=nav.SelectSingleNode("Database").Value;
				XPathNavigator nodeMT=nav.SelectSingleNode("MiddleTierAddr");
				if(nodeMT==null) {//Using direct database connection
					mySqlUser=nav.SelectSingleNode("User").Value;
					mySqlPassword=nav.SelectSingleNode("Password").Value;
					XPathNavigator passHashNode=nav.SelectSingleNode("MySQLPassHash");
					string decryptedPwd;
					if(mySqlPassword=="" && passHashNode!=null && passHashNode.Value!="" && CDT.Class1.Decrypt(passHashNode.Value,out decryptedPwd)) {
						mySqlPassword=decryptedPwd;
					}
				}
				else { //Using middle tier connection
					_middleTierAddr=nodeMT.Value;
					_usingMiddleTier=true;	
					if(nav.SelectSingleNode("UsingAutoLogin")!=null) {  //Using "Log me in automatically" via Middle Tier
						_usingAutoLogin=true;
						autoLoginUser=nav.SelectSingleNode("AutoLoginUser").Value;
					}
				}
			}
			catch(Exception ex) {
				//Common error: root element is missing
				OpenDental.MessageBox.Show(ex.Message);
				Application.Exit();
				return false;
			}
			DataConnection.DBtype=DatabaseType.MySql;
			DataConnection dcon=new DataConnection();
			//Try to connect to the database directly
			if(_usingMiddleTier) {
				bool doShowWindow=true;
				using FormCentralChooseDatabase FormCCD=new FormCentralChooseDatabase(_middleTierAddr); 
				if(_usingAutoLogin) {
					try {
						string password=PasswordVaultWrapper.RetrievePassword(_middleTierAddr,autoLoginUser);
						if(FormCCD.CanConnectToMTServer(autoLoginUser,password)) {
							doShowWindow=false;
						}
					}
					catch(Exception ex) {
						ex.DoNothing();
					}
				}
				if(doShowWindow) {
					if(FormCCD.ShowDialog()==DialogResult.Cancel) {
						Application.Exit();
						return false;
					}
					_usingAutoLogin=FormCCD.UsingAutoLogin;   //still necessary to ensure checkbox on FormCentralUserSettings is checked when first enabling auto login.
				}
			}
			else { //not using Middle Tier
				try {
					dcon.SetDb(computerName,database,mySqlUser,mySqlPassword,"","",DataConnection.DBtype);
					Version storedVersion=new Version(PrefC.GetString(PrefName.ProgramVersion));
					Version currentVersion=Assembly.GetAssembly(typeof(Db)).GetName().Version;
					//if(!ODBuild.IsDebug() && storedVersion.CompareTo(currentVersion)!=0) {
					if(storedVersion.CompareTo(currentVersion)!=0) {
						OpenDental.MessageBox.Show(Lan.g(this,"Program version")+": "+currentVersion.ToString()+"\r\n"
							+Lan.g(this,"Database version")+": "+storedVersion.ToString()+"\r\n"
							+Lan.g(this,"Versions must match.  Please manually connect to the database through the main program in order to update the version."));
						_hasFatalError=true;
						Application.Exit();
						return false;
					}
					RemotingClient.RemotingRole=RemotingRole.ClientDirect;
					SerializableDictionary<long,string> dictDomainUsers=Userods.GetDomainUsers();
					if(dictDomainUsers.Count==1) {
						//one user returned
						Security.CurUser=Userods.GetUserNoCache(dictDomainUsers.Keys.First());
						return true;
					}
					else if(dictDomainUsers.Count>1) {
						//multiple users returned
						using(InputBox box=new InputBox(Lan.g(this,"Select an Open Dental user to log in with:"),dictDomainUsers.Values.ToList())){
							if(box.ShowDialog()==DialogResult.OK) {
								Security.CurUser=Userods.GetUserNoCache(dictDomainUsers.Keys.ElementAt(box.SelectedIndex));
								return true;
							}
						}
					}
					//If no users match the domainUser show the normal log in screen
					using FormCentralLogOn FormCLO=new FormCentralLogOn();
					if(FormCLO.ShowDialog()!=DialogResult.OK) {
						_hasFatalError=true;
						Application.Exit();
						return false;
					}
					return true;
				}
				catch(Exception ex) {
					OpenDental.MessageBox.Show(ex.Message);
					_hasFatalError=true;
					Application.Exit();
					return false;
				}
			}
			return true;
		}

		private void GetDataTablePatForConn(ODThread odThread) {
			CentralConnection connection=(CentralConnection)odThread.Parameters[0];
			//Filter the threads by their connection name
			string connName=GetConnectionName(connection);
			if(!CentralConnectionHelper.SetCentralConnection(connection,false)) {
				lock(_lockObj) {
					_invalidConnsLog+="\r\n"+connName+": "+Lan.g(this,"could not connect")+"\r\n";
				}
				connection.ConnectionStatus="OFFLINE";
				//BeginInvoke((Action)FillGridPats);
				return;
			}
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.CEMTSearchPatients);
			bool hasNextLastVisit=fields.Any(x => ListTools.In(x.InternalName,"NextVisit","LastVisit"));
			DataTable table=new DataTable();
			try {
				PtTableSearchParams ptTableSearchParams=new PtTableSearchParams(checkLimit.Checked,textLName.Text,textFName.Text,textPhone.Text,
					textAddress.Text,checkHideInactive.Checked,textCity.Text,textState.Text,textSSN.Text,textPatNum.Text,textChartNumber.Text,0,
					checkGuarantors.Checked,!checkHideArchived.Checked,//checkHideArchived is opposite label for what this function expects, but hideArchived makes more sense
					SIn.DateT(textBirthdate.Text),0,textSubscriberID.Text,textEmail.Text,textCountry.Text,"","",textClinicPatSearch.Text,"",hasNextLastVisit:hasNextLastVisit);
				table=Patients.GetPtDataTable(ptTableSearchParams);
			}
			catch(ThreadAbortException tae) {
				throw tae;//ODThread needs to clean up after an abort exception is thrown.
			}
			catch(Exception e) {
				//This can happen if the connection to the server was severed somehow during the execution of the query.
				lock(_lockObj) {
					_invalidConnsLog+="\r\n"+connName+": "+Lan.g(this,"An error was encountered while searching for patients.")
													+"\r\n  "+Lan.g(this,"This connection's database may be an incompatible version.")+"\r\n  "
													+Lan.g(this,"The error was")+":\r\n  "+e.Message+"\r\n"+e.StackTrace+"\r\n";
				}
				//BeginInvoke((Action)FillGridPats);//Pops up a message box if this was the last thread to finish.
				return;
			}
			table.TableName=connName;
			odThread.Tag=table;
			lock(_lockObj) {
				_dataSetPats.Tables.Add((DataTable)odThread.Tag);
			}
			//BeginInvoke((Action)FillGridPats);
		}

		///<summary>Pass in a list of about 50 visible windows.  This finds the one that matches the given process.  If not found, returns 0.</summary>
		private IntPtr GetHwndForProcess(int processID,List<IntPtr> listVisibleWindows){
			//IntPtr hWndDesktop=NativeMethods.GetDesktopWindow();
			//int GW_OWNER=4;
			//int GA_PARENT=1;
			//int GA_ROOTOWNER=3;
			List<IntPtr> listHwndProcess=new List<IntPtr>();//this will be filtered for the single process
			for(int i=0;i<listVisibleWindows.Count;i++) {
				int pId=NativeHelpers.GetProcessID(listVisibleWindows[i]);
				if(pId!=processID){
					continue;
				}
				listHwndProcess.Add(listVisibleWindows[i]);
			}
			//Now, we're typically down to 2, but it's hard to pick between them.
			for(int i=0;i<listHwndProcess.Count;i++) {
				IntPtr hWndThisLoop=listHwndProcess[i];
				//IntPtr hWndOwner=NativeMethods.GetWindow(hWndThisLoop,GW_OWNER);//0 for both
				//IntPtr hWndParent=NativeMethods.GetParent(hWndThisLoop);//0 for both
				//IntPtr hWndAncParent=NativeMethods.GetAncestor(hWndThisLoop,GA_PARENT);// desktop for both
				//IntPtr hWndAncRootOwn=NativeMethods.GetAncestor(hWndThisLoop,GA_ROOTOWNER);//self for both
				//IntPtr hWndLastActive=NativeMethods.GetLastActivePopup(hWndThisLoop);//self for both
				StringBuilder strb=new StringBuilder();
				NativeMethods.GetWindowText(hWndThisLoop,strb,256);//empty for one
				if(strb.Length==0){
					continue;
				}
				return hWndThisLoop;
			}
			return IntPtr.Zero;
		}

		public static ShowState GetWindowState(IntPtr hwnd) {//, out System.Drawing.Rectangle rect) {This rect is useless to us.  It's some sort of "restore" rect.
			WINDOWPLACEMENT windowPlacement = new WINDOWPLACEMENT();
			windowPlacement.length = Marshal.SizeOf(windowPlacement);
			bool success=NativeMethods.GetWindowPlacement(hwnd,ref windowPlacement);
			if(!success){

			}
			return windowPlacement.showCmd;
		}

		private void LaunchConnection(CentralConnection conn,long patNum=0){
			if(conn.ConnectionStatus.StartsWith("OFFLINE")) {
				MsgBox.Show(this,"Server Offline.  Fix connection and check status again to connect.");
				return;
			}
			//Only if they are not using dynamic mode is a version mismatch a problem.
			if(conn.ConnectionStatus!="OK" && !PrefC.GetBool(PrefName.CentralManagerUseDynamicMode) ){
				MsgBox.Show(this,"Version mismatch.  Either update your program or update the remote server's program and check status again to connect.");
				return;
			}
			if(string.IsNullOrEmpty(conn.DatabaseName) && string.IsNullOrEmpty(conn.ServiceURI)) {
				OpenDental.MessageBox.Show("Either a database or a Middle Tier URI must be specified in the connection.");
				return;
			}
			ShowState showState=ShowState.SW_SHOW;
			//loop to minimize any other windows 
			for(int i=_listWindowInfos.Count-1;i>=0;i--){//loop backward so we can remove bad ones
				if(_listWindowInfos[i].HWnd==IntPtr.Zero){
					List<IntPtr> listVisibleWindows=NativeHelpers.GetAllVisibleWindows();//approx 50
					_listWindowInfos[i].HWnd=GetHwndForProcess(_listWindowInfos[i].ProcessId,listVisibleWindows);
				}
				try{
					showState=GetWindowState(_listWindowInfos[i].HWnd);
					if(showState==ShowState.SW_HIDE){//window is gone
						_listWindowInfos.Remove(_listWindowInfos[i]);
						continue;
					}
				}
				catch{
					_listWindowInfos.Remove(_listWindowInfos[i]);
					continue;
				}
				if(NativeHelpers.IsMinimized(showState)){
					continue;
				}
				if(NativeHelpers.IsMaximized(showState)){
					_listWindowInfos[i].WasMaximized=true;
				}
				else{
					_listWindowInfos[i].WasMaximized=false;
				}
				NativeMethods.ShowWindow(_listWindowInfos[i].HWnd,(int)ShowState.SW_MINIMIZE);//.SW_SHOWMINNOACTIVE);//didn't work
				_listWindowInfos[i].IsMinimized=true;
			}
			WindowInfo windowInfo=_listWindowInfos.FirstOrDefault(x=>x.CentralConnectionNum==conn.CentralConnectionNum);
			if(windowInfo==null){//new, so double clicked on a connection with no previous window.
				windowInfo=new WindowInfo();
				windowInfo.CentralConnectionNum=conn.CentralConnectionNum;
				CentralConnectionHelper.LaunchOpenDental(conn,PrefC.GetBool(PrefName.CentralManagerUseDynamicMode),PrefC.GetBool(PrefName.CentralManagerIsAutoLogon),
					PrefC.GetBool(PrefName.DomainLoginEnabled), patNum,ref windowInfo);
				//List<IntPtr> listVisibleWindows=NativeHelpers.GetAllVisibleWindows();
				//can't get hWnd here because we can't test recent popup yet:
				//windowInfo.HWnd=GetHwndForProcess(windowInfo.ProcessId,listVisibleWindows);		
				_listWindowInfos.Add(windowInfo);
				FillGridConns();
				return;
			}
			//from here down, we are attempting to open an existing window
			if(windowInfo.HWnd==IntPtr.Zero){
				List<IntPtr> listVisibleWindows=NativeHelpers.GetAllVisibleWindows();//approx 50
				windowInfo.HWnd=GetHwndForProcess(windowInfo.ProcessId,listVisibleWindows);
			}
			try{
				showState=GetWindowState(windowInfo.HWnd);
			}
			catch{
//todo: this might be a bad test for a window being gone
				//window is gone
				_listWindowInfos.Remove(windowInfo);
				FillGridConns();
				return;
			}
			if(NativeHelpers.IsMinimized(showState)){
				if(windowInfo.WasMaximized){
					NativeMethods.ShowWindow(windowInfo.HWnd,(int)ShowState.SW_MAXIMIZE);
				}
				else{
					NativeMethods.ShowWindow(windowInfo.HWnd,(int)ShowState.SW_SHOWNORMAL);
				}
				windowInfo.IsMinimized=false;
			}
			NativeMethods.BringWindowToTop(windowInfo.HWnd);
			FillGridConns();
		}
		#endregion Methods

		
	}
}
