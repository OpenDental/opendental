using CodeBase;
using DataConnectionBase;
using Microsoft.VisualBasic.ApplicationServices;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormQueryMonitor:FormODBase {
		///<summary>Queries will be coming in at an alarming rate and this queue will get filled via DbMonitor events that are fired.
		///There will be a timer running on the UI thread that will be pulling queries out of this queue and storing them into a dictionary.</summary>
		private ConcurrentQueue<DbQueryObj> _concurrentQueueDbQueryObjs=new ConcurrentQueue<DbQueryObj>();
		///<summary>Stores all query objects that this instance of the query monitor has captured. 2022-10-05-js-dictionary ok</summary>
		private Dictionary<Guid,DbQueryObj> _dictionaryQueries=new Dictionary<Guid,DbQueryObj>();
		///<summary>Shallow copy of the currently selected query object.  Do not trust the grid because it gets truncated as the feed grows.</summary>
		private DbQueryObj _dbQueryObj=null;
		/// <summary> True if monitoring Middle Tier Payloads, false if monitoring queries on a direct DB connection </summary>
		public bool IsPayloadMonitor=RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT;

		public FormQueryMonitor() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormQueryMonitor_Load(object sender,EventArgs e) {
			FillGridColumns();
			//Add "View DOB" right click option, MenuItemPopup() will show and hide it as needed.
			if(gridFeed.ContextMenu==null) {
				gridFeed.ContextMenu=new ContextMenu();//ODGrid will automatically attach the default Popups
			}
			ContextMenu contextMenu=gridFeed.ContextMenu;
			System.Windows.Forms.MenuItem menuItemStackTrace=new System.Windows.Forms.MenuItem();
			menuItemStackTrace.Name="showStackTrace";
			menuItemStackTrace.Text=Lan.g(this,"View Stack Trace");
			menuItemStackTrace.Click+= new EventHandler(this.MenuItemShowStackTrace_Click);
			contextMenu.MenuItems.Add(menuItemStackTrace);
			contextMenu.Popup+=MenuItemPopupShowStackTrace;
			if(!IsPayloadMonitor){
				return;
			}
			Text=Lan.g(this,"Payload Monitor");
			gridFeed.Title=Lan.g(this,"Payload Feed");
			
		}

		private void MenuItemShowStackTrace_Click(object sender,EventArgs e) {
			int index=gridFeed.GetSelectedIndex();
			if(index!=-1) {
				DbQueryObj dbQueryObj=gridFeed.ListGridRows[index].Tag as DbQueryObj;
				MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(dbQueryObj.StackTrace.ToString());
				msgBoxCopyPaste.Show();
			}
		}

		private void MenuItemPopupShowStackTrace(Object sender, EventArgs e) {
			System.Windows.Forms.MenuItem menuItem=gridFeed.ContextMenu.MenuItems
				.OfType<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Name=="showStackTrace");
			if(menuItem==null) { return; }//Should not happen
			System.Windows.Forms.MenuItem menuItemSeperator=gridFeed.ContextMenu.MenuItems
				.OfType<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Text=="-");
			if(menuItemSeperator==null) { return; }//Should not happen
			int indexRowClick=gridFeed.GetSelectedIndex();
			if(indexRowClick!=-1) {
				DbQueryObj dbQueryObj=gridFeed.ListGridRows[indexRowClick].Tag as DbQueryObj;
				if(dbQueryObj.StackTrace is null) {
					menuItem.Enabled=false;
					menuItem.Visible=false;
					menuItemSeperator.Visible=false;
					menuItemSeperator.Enabled=false;
					return;
				} 
				menuItem.Enabled=true;
				menuItem.Visible=true;
				menuItemSeperator.Visible=true;
				menuItemSeperator.Enabled=true;
				return;
			}
			menuItem.Visible=false;
			menuItem.Enabled=false;
			menuItemSeperator.Visible=false;
			menuItemSeperator.Enabled=false;
		}

		private bool UserHasStackTracePref() {
			UserOdPref userOdPref=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum, UserOdFkeyType.QueryMonitorHasStackTraces);
			if(userOdPref.IsNew) {
				userOdPref.ValueString="0";
				UserOdPrefs.Insert(userOdPref);
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
				return false;
			}
			if(userOdPref.ValueString=="1") {
				return true;
			}
			return false;
			
		}

		private void ToggleUserStackTracePref() {
			UserOdPref userOdPref=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum, UserOdFkeyType.QueryMonitorHasStackTraces);
			UserOdPref userOdPrefOld=userOdPref.Clone();
			if(userOdPref.ValueString=="0") {
				userOdPref.ValueString="1";
				if(UserOdPrefs.Update(userOdPref,userOdPrefOld)) {
					//Only need to signal cache refresh on change.
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
				return;
			}
			userOdPref.ValueString="0";
			if(UserOdPrefs.Update(userOdPref,userOdPrefOld)) {
				//Only need to signal cache refresh on change.
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
		}

		private void TimerProcessQueue_Tick(object sender,EventArgs e) {
			if(_concurrentQueueDbQueryObjs.Count==0) {
				return;
			}
			List<DbQueryObj> listDbQueryObjs=new List<DbQueryObj>();
			while(_concurrentQueueDbQueryObjs.Count!=0) {
				if(!_concurrentQueueDbQueryObjs.TryDequeue(out DbQueryObj query)) {
					break;
				}
				if((IsPayloadMonitor && query.IsPayload) || (!IsPayloadMonitor && !query.IsPayload)){ //Payload monitor and query is payload, OR query monitor and query is query
					listDbQueryObjs.Add(query);
				}
			}
			AddQueriesToGrid(listDbQueryObjs.ToArray());
		}

		private void FillGridColumns() {
			gridFeed.BeginUpdate();
			gridFeed.Columns.Clear();
			gridFeed.Columns.Add(new GridColumn(Lan.g(gridFeed.TranslationName,"Server"),100));
			gridFeed.Columns.Add(new GridColumn(Lan.g(gridFeed.TranslationName,"Command"),100){ IsWidthDynamic=true });
			gridFeed.Columns.Add(new GridColumn(
				Lan.g(gridFeed.TranslationName,"DateTimeStart"),125,HorizontalAlignment.Center,GridSortingStrategy.DateParse)
			);
			gridFeed.Columns.Add(new GridColumn(Lan.g(gridFeed.TranslationName,"Elapsed"),100,HorizontalAlignment.Center));
			gridFeed.EndUpdate();
		}

		private void GridFeed_CellClick(object sender,ODGridClickEventArgs e) {
			textDateTimeStart.Clear();
			textDateTimeStop.Clear();
			textElapsed.Clear();
			textCommand.Clear();
			DbQueryObj dbQueryObj=gridFeed.ListGridRows[e.Row].Tag as DbQueryObj;
			if(dbQueryObj==null) {
				return;
			}
			ODException.SwallowAnyException(() => {
				_dbQueryObj=dbQueryObj;
				textDateTimeStart.Text=dbQueryObj.DateTimeStart.ToString();
				textDateTimeStop.Text=dbQueryObj.DateTimeStart.ToString();
				textElapsed.Text=dbQueryObj.Elapsed.ToString("G");
				textCommand.Text=dbQueryObj.Command;
			});
		}

		private void AddQueriesToGrid(params DbQueryObj[] dbQueryObjArray) {
			if(dbQueryObjArray.IsNullOrEmpty()) {
				return;
			}
			for(int i=0;i< dbQueryObjArray.Count();i++) {
				_dictionaryQueries[dbQueryObjArray[i].GUID]=dbQueryObjArray[i];//Refresh the object within the dictionary so that any new information(e.g.Elapsed) gets updated.
			}
			//Arbitrarily limit the number of rows showing in the grid.
			//The top of the grid is the oldest query so take the last X items.
			//Use TakeLast(X) instead of a loop because a bunch of queries could have been queued (e.g. pasting schedules can queue thousands).
			List<DbQueryObj> listDbQueryObjs=UIHelper.TakeLast(_dictionaryQueries.Values,500).ToList();
			gridFeed.BeginUpdate();
			gridFeed.ListGridRows.Clear();
			for(int i=0; i<listDbQueryObjs.Count;i++) {
				string command;
				string server;
				if(IsPayloadMonitor) {
					DataTransferObject dataTransferObjectParsedCmd=DataTransferObject.Deserialize(listDbQueryObjs[i].Command);
					command=$"{dataTransferObjectParsedCmd.GetType().Name} - {dataTransferObjectParsedCmd.MethodName}";
					listDbQueryObjs[i].MethodName=dataTransferObjectParsedCmd.MethodName;
					server="";
				}
				else {
					command=listDbQueryObjs[i].Command.Trim();
					server=listDbQueryObjs[i].ServerName;
				}
				GridRow row;
				if(listDbQueryObjs[i].Elapsed==TimeSpan.MinValue){
					row=new GridRow(server,command,listDbQueryObjs[i].DateTimeInit.ToString(),"");
				}
				else{
					row=new GridRow(server,command,listDbQueryObjs[i].DateTimeInit.ToString(),listDbQueryObjs[i].Elapsed.ToString("G"));
				}
				row.Tag=listDbQueryObjs[i];
				gridFeed.ListGridRows.Add(row);
			}
			gridFeed.EndUpdate();
			gridFeed.ScrollToIndex(gridFeed.ListGridRows.Count-1);
		}

		private void ButStart_Click(object sender,EventArgs e) {
			if(butStart.Text=="Stop") {//currently monitoring queries.
				timerProcessQueue.Stop();
				QueryMonitorEvent.Fired-=DbMonitorEvent_Fired;
				QueryMonitor.Monitor.IsMonitoring=false;
				butStart.Text="Start";
				return;
			}
			timerProcessQueue.Start();
			QueryMonitorEvent.Fired+=DbMonitorEvent_Fired;
			QueryMonitor.Monitor.IsMonitoring=true;
			QueryMonitor.Monitor.HasStackTrace=UserHasStackTracePref();
			butStart.Text="Stop";
			
		}

		private void DbMonitorEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.QueryMonitor || !(e.Tag is DbQueryObj)) {
				return;
			}
			_concurrentQueueDbQueryObjs.Enqueue(e.Tag as DbQueryObj);
		}

		private void ButLog_Click(object sender,EventArgs e) {
			if(butStart.Text=="Stop") {//currently monitoring queries.
				MsgBox.Show(this,"Stop monitoring queries before creating a log.");
				return;
			}
			if(_dictionaryQueries.Count==0) {
				MsgBox.Show(this,"No queries in the Query Feed to log.");
				return;
			}
			if(ODEnvironment.IsCloudServer) {
				MsgBox.Show(this,"Logging not supported while using Open Dental Cloud.");
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.YesNo,Lan.g(this,"Log all queries to a file?  Total query count")+$": {_dictionaryQueries.Count.ToString("N0")}")) {
				return;
			}
			string logFolderPath="QueryMonitorLogs";
			string logFileName="";
			if(!OpenDentBusiness.FileIO.FileAtoZ.DirectoryExistsRelative(logFolderPath)) {
				try {
					//Create the query monitor log folder in the AtoZ image path.
					OpenDentBusiness.FileIO.FileAtoZ.CreateDirectoryRelative(logFolderPath);
				}
				catch(ODException ode) {
					MsgBox.Show(ode.Message);
					return;
				}
			}
			//Get a unique file name within the log folder.
			logFileName=$"QueryMonitorLog_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt";
			try{
				while(OpenDentBusiness.FileIO.FileAtoZ.ExistsRelative(logFolderPath,logFileName)) {
					logFileName=$"QueryMonitorLog_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt";
					Thread.Sleep(100);
				}
			}
			catch(ODException ode) {
				MsgBox.Show(ode.Message);
				return;
			}
			//Dump the entire query history into the log file.
			try{
			OpenDentBusiness.FileIO.FileAtoZ.WriteAllTextRelative(logFolderPath,logFileName,
				$"Query Monitor Log - {DateTime.Now.ToString()}, OD User: {Security.CurUser.UserName}, Computer: {ODEnvironment.MachineName}\r\n"+
				$"{string.Join("\r\n",_dictionaryQueries.Values.Select(x => x.ToString()))}");
			}
			catch(Exception ex) {
				MsgBox.Show(Lan.g(this,"Error creating log file")+$":\r\n{ex.Message}");
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Log file created.  Would you like to open the file?")) {
				try {
					FileAtoZ.StartProcessRelative(logFolderPath,logFileName);
				}
				catch(Exception ex) {
					MsgBox.Show(Lan.g(this,"Could not open log file")+$":\r\n{ex.Message}");
				}
			}
		}
		
		private void butClear_Click(object sender,EventArgs e) {
			if(gridFeed.ListGridRows.IsNullOrEmpty()) {
				MsgBox.Show(this,"There are no items in the feed to clear.");
				return;
			}
			//Clear out the query feed dictionary and grid control.
			_dictionaryQueries.Clear();
			gridFeed.BeginUpdate();
			gridFeed.ListGridRows.Clear();
			gridFeed.EndUpdate();
		}

		private void ButCopy_Click(object sender,EventArgs e) {
			if(_dbQueryObj==null) {
				MsgBox.Show(this,"Select a row from the Query Feed.");
				return;
			}
			try {
				ODClipboard.SetClipboard(_dbQueryObj.ToString());
				MsgBox.Show(this,"Copied");
			}
			catch(Exception ex) {
				ex.DoNothing();
				MsgBox.Show(this,"Could not copy contents to clipboard.  Please try again.");
			}
		}

		private void FormQueryMonitor_FormClosing(object sender,FormClosingEventArgs e) {
			ODException.SwallowAnyException(() => { QueryMonitorEvent.Fired-=DbMonitorEvent_Fired; });
		}

	}
}