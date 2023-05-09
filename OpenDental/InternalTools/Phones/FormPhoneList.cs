using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPhoneList : FormODBase {

		#region Fields
		private Bitmap _bitmapHouse=PhoneTile.GetHouse16();
		///<summary>A timer is constantly flipping this back and forth.  Indicates currently pink, regardless of whether any lights really need to be flashing.</summary>
		private bool _isFlashingPink;
		private List<ChatUser> _listChatUsers;
		private List<MapArea> _listMapAreas;
		private List<Phone> _listPhonesAll;
		///<summary>Filtered and sorted to exactly match grid.</summary>
		private List<Phone> _listPhonesShowing;
		private List<WebChatSession> _listWebChatSessions;
		///<summary>Remote Support Sessions.</summary>
		private List<PeerInfo> _listPeerInfos;
		private Phone _phoneSelected;
		private int _tileHeight=35;
		private int _tileWidth=130;
		///<summary>How many phone tiles should show up in each column before creating a new column.</summary>
		private int _tilesPerColumn=27;
		///<summary>This is the difference between server time and local computer time.  Used to ensure that times displayed are accurate to the second.  This value is usally just a few seconds, but possibly a few minutes.</summary>
		private TimeSpan _timeSpanDelta;
		//Fields - Public---------------------------------------------------------------------------------------------------
		///<summary>When the GoToChanged event fires, this tells us which patnum.</summary>
		public long PatNumGoTo;
		#endregion Fields

		#region Constructor
		public FormPhoneList() {
			InitializeComponent();
			InitializeLayoutManager();
			//This window is bigger than our "allowed" size.
			//It's maximized on startup to prevent it from resizing if zoomed.
		}
		#endregion Constructor

		#region Events - Raise
		///<summary></summary>
		[Category("Property Changed"), Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler GoToPatient=null;
		#endregion Events - Raise

		#region Methods - Event Handlers
		private void FormPhoneList_Load(object sender,EventArgs e) {
			_isFlashingPink=false;
			_timeSpanDelta=MiscData.GetNowDateTime()-DateTime.Now;
			_listPhonesAll=Phones.GetPhoneList();
			_listChatUsers=ChatUsers.GetAll();
			_listWebChatSessions=WebChatSessions.GetActiveSessions();
			_listPeerInfos=PeerInfos.GetActiveSessions(false,true);
			_listPeerInfos=new List<PeerInfo>();
			_listMapAreas=MapAreas.Refresh();
			listBoxQueueFilter.SelectedIndex=0;//all
			Employees.RefreshCache();
			FilterAndSortPhoneList();
			FillGrid();
			radioByExt.CheckedChanged+=radioSort_CheckedChanged;
			radioByName.CheckedChanged+=radioSort_CheckedChanged;
			listBoxStatus.Items.Clear();
			listBoxStatus.Items.Add("Available",ClockStatusEnum.Available);
			listBoxStatus.Items.Add("Training",ClockStatusEnum.Training);
			listBoxStatus.Items.Add("TeamAssist",ClockStatusEnum.TeamAssist);
			listBoxStatus.Items.Add("TC/Responder",ClockStatusEnum.TCResponder);
			listBoxStatus.Items.Add("NeedsHelp",ClockStatusEnum.NeedsHelp);
			listBoxStatus.Items.Add("WrapUp",ClockStatusEnum.WrapUp);
			listBoxStatus.Items.Add("OfflineAssist",ClockStatusEnum.OfflineAssist);
			listBoxStatus.Items.Add("Unavailable",ClockStatusEnum.Unavailable);
			listBoxStatus.Items.Add("Backup",ClockStatusEnum.Backup);
			timerFlash.Enabled=true;
			textNameOrExt.Select();//Tab order might not have been good enough
			//There is no event for textNameOrExt changing value because the grid gets refreshed every second or two.
		}

		private void FormPhoneTiles_Shown(object sender,EventArgs e) {      
			//DateTime now=DateTime.Now;
			//while(now.AddSeconds(1)>DateTime.Now) {
			//	Application.DoEvents();
			//}
		}

		private void butConfRooms_Click(object sender,EventArgs e) {
			using FormPhoneConfs FormPC=new FormPhoneConfs();
			FormPC.ShowDialog();//ShowDialog because we do not want this window to be floating open for long periods of time.
		}

		private void butEditDefaults_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Please select a row in the grid first.");
				return;
			}
			Phone phone = _listPhonesShowing[gridMain.GetSelectedIndex()];
			if(phone.EmployeeNum < 1) {
				MsgBox.Show("This row does not have an employee.");
				return;
			}
			PhoneEmpDefault phoneEmpDefault=PhoneEmpDefaults.GetOne(phone.EmployeeNum);
			if(phoneEmpDefault==null) {
				MessageBox.Show("No 'phoneempdefault' row found for EmployeeNum "+phone.EmployeeNum
					+".\r\nGo to Phone Settings window and add a row for this employee.");
				return;
			}
			using FormPhoneEmpDefaultEdit formPhoneEmpDefaultEdit=new FormPhoneEmpDefaultEdit();
			formPhoneEmpDefaultEdit.PedCur=phoneEmpDefault;
			formPhoneEmpDefaultEdit.ShowDialog();
			RefreshList();
		}

		private void butGotoPatient_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Please select a row in the grid first.");
				return;
			}
			Phone phone = _listPhonesShowing[gridMain.GetSelectedIndex()];
			if(phone.PatNum==0) {
				MsgBox.Show("This row has no patient attached.");
				return;
			}
			PatNumGoTo=phone.PatNum;
			GoToPatient?.Invoke(this,new EventArgs());
		}

		private void butPhoneManage_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Please select a row in the grid first.");
				return;
			}
			Phone phone = _listPhonesShowing[gridMain.GetSelectedIndex()];
			PhoneUI.Manage(phone);
		}

		private void butPhoneAttach_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Please select a row in the grid first.");
				return;
			}
			Phone phone = _listPhonesShowing[gridMain.GetSelectedIndex()];
			PhoneUI.Add(phone);
		}

		private void checkHideClockedOut_CheckedChanged(object sender,EventArgs e) {
			FilterAndSortPhoneList();
			FillGrid();
		}

		private void checkHideOnBreak_CheckedChanged(object sender,EventArgs e) {
			FilterAndSortPhoneList();
			FillGrid();
		}

		private void checkNeedsHelpTop_Click(object sender,EventArgs e){
			FilterAndSortPhoneList();
			FillGrid();
		}

		private void checkShowOldInterface_Click(object sender,EventArgs e) {
			
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e){
			if(e.Col==7){//Customer
				butGotoPatient_Click(this,new EventArgs());
				return;
			}
			//any other col:
			butEditDefaults_Click(this,new EventArgs());
		}

		private void listBoxClockOut_SelectionChangeCommitted(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show("Please select a row in the grid first.");
				listBoxClockOut.SelectedIndex=-1;
				return;
			}
			Phone phone=_listPhonesShowing[gridMain.GetSelectedIndex()];
			int idx=listBoxClockOut.SelectedIndex;
			switch(idx){
				case 0:
					PhoneUI.Home(phone);//these will show msgbox if user does not have permission
					break;
				case 1:
					PhoneUI.Lunch(phone);
					break;
				case 2:
					PhoneUI.Break(phone);
					break;
			}
			listBoxClockOut.SelectedIndex=-1;
			RefreshList();
		}

		private void listBoxQueues_SelectionChangeCommitted(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show("Please select a row in the grid first.");
				listBoxQueues.SelectedIndex=-1;
				return;
			}
			Phone phone=_listPhonesShowing[gridMain.GetSelectedIndex()];
			int idx=listBoxQueues.SelectedIndex;
			switch(idx){
				case 0:
					PhoneUI.QueueTech(phone);//these will show msgbox if user does not have permission
					break;
				case 1:
					PhoneUI.QueueNone(phone);
					break;
				case 2:
					PhoneUI.QueueDefault(phone);
					break;
				case 3:
					PhoneUI.QueueBackup(phone);
					break;
			}
			listBoxQueues.SelectedIndex=-1;
		}

		private void listBoxStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show("Please select a row in the grid first.");
				listBoxStatus.SelectedIndex=-1;
				return;
			}
			Phone phone=_listPhonesShowing[gridMain.GetSelectedIndex()];
			bool allowStatusEdit=ClockEvents.IsClockedIn(phone.EmployeeNum);
			if(phone.EmployeeNum==Security.CurUser.EmployeeNum) { //Always allow status edit for yourself
				allowStatusEdit=true;
			}
			if(phone.ClockStatus==ClockStatusEnum.NeedsHelp) { //Always allow any employee to change any other employee from NeedsAssistance to Available
				allowStatusEdit=true;
			}
			string statusOnBehalfOf=phone.EmployeeName;
			bool allowSetSelfAvailable=false;
			if(!ClockEvents.IsClockedIn(phone.EmployeeNum) //No one is clocked in at this extension.
				&& !ClockEvents.IsClockedIn(Security.CurUser.EmployeeNum)) //This user is not clocked in either.
			{ 
				//Vacant extension and this user is not clocked in so allow this user to clock in at this extension.
				statusOnBehalfOf=Security.CurUser.UserName;
				allowSetSelfAvailable=true;
			}
			ClockStatusEnum clockStatus=listBoxStatus.GetSelected<ClockStatusEnum>();
			if(clockStatus==ClockStatusEnum.Available){
				if(!allowStatusEdit && !allowSetSelfAvailable){
					MsgBox.Show("Not clocked in.");
					return;
				}
			}
			else{
				if(!allowStatusEdit){
					MsgBox.Show("Not clocked in.");
					return;
				}
			}
			if(clockStatus==ClockStatusEnum.Available){
				PhoneUI.Available(phone);
			}
			else if(clockStatus==ClockStatusEnum.Training){
				PhoneUI.Training(phone);
			}
			else if(clockStatus==ClockStatusEnum.TeamAssist){
				PhoneUI.TeamAssist(phone);
			}
			else if(clockStatus==ClockStatusEnum.TCResponder){
				PhoneUI.TCResponder(phone);
			}
			else if(clockStatus==ClockStatusEnum.NeedsHelp){
				PhoneUI.NeedsHelp(phone);
			}
			else if(clockStatus==ClockStatusEnum.WrapUp){
				PhoneUI.WrapUp(phone);
			}
			else if(clockStatus==ClockStatusEnum.OfflineAssist){
				PhoneUI.OfflineAssist(phone);
			}
			else if(clockStatus==ClockStatusEnum.Unavailable){
				PhoneUI.Unavailable(phone);
			}
			else if(clockStatus==ClockStatusEnum.Backup){
				PhoneUI.Backup(phone);
			}
			listBoxStatus.SelectedIndex=-1;
			RefreshList();
		}

		private void radioSort_CheckedChanged(object sender,EventArgs e) {
			RefreshList();
		}
		
		private void butSettings_Click(object sender,EventArgs e) {
			using FormPhoneEmpDefaults formPED=new FormPhoneEmpDefaults();
			formPED.ShowDialog();
		}

		private void timerFlash_Tick(object sender,EventArgs e) {
			//every 300 ms
			_isFlashingPink=!_isFlashingPink;//toggles flash
			FillGrid();
		}
		#endregion Methods - Event Handlers

		#region Methods - Public 
		///<summary>Called from FormOpenDental when signals come in.</summary>
		public void SetPhoneList(List<Phone> listPhones,List<ChatUser> listChatUsers,List<WebChatSession> listWebChatSession,
			List<PeerInfo> listPeerInfos)
		{
			_listPhonesAll=new List<Phone>(listPhones);//a copy so that we don't alter the original
			_listChatUsers=listChatUsers;
			_listWebChatSessions=listWebChatSession;
			_listPeerInfos=listPeerInfos;
			FilterAndSortPhoneList();
			FillGrid();
		}

		public void SetVoicemailCount(int voiceMailCount) {
			if(voiceMailCount==0) {
				labelMsg.Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleFontODZoom(8.5f),FontStyle.Regular);
				labelMsg.ForeColor=Color.Black;
			}
			else {
				labelMsg.Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleFontODZoom(10f),FontStyle.Bold);
				labelMsg.ForeColor=Color.Firebrick;
			}
			labelMsg.Text="Voice Mails: "+voiceMailCount.ToString();
		}
		#endregion Methods - Public

		#region Methods - Private
		private void FillGrid(){
			//this grid is on a timer to get refreshed every 300ms, which is pretty cool.  It doesn't even flicker.
			int idxSelected=gridMain.GetSelectedIndex();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Ext",40);
			gridMain.Columns.Add(col);
			col=new GridColumn("Name",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Status",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Phone",60);
			gridMain.Columns.Add(col);
			col=new GridColumn("Prox",50);
			gridMain.Columns.Add(col);
			col=new GridColumn("Time",50);
			gridMain.Columns.Add(col);
			col=new GridColumn("Cubicle",50);
			gridMain.Columns.Add(col);
			col=new GridColumn("Queue",50);
			gridMain.Columns.Add(col);
			col=new GridColumn("Customer",150);
			gridMain.Columns.Add(col);
			col=new GridColumn("Employee Notes",200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			for(int i=0;i<_listPhonesShowing.Count;i++){
				GridRow row=new GridRow();
				//Ext--------------------------------------------------------------------------------------------------------------
				row.Cells.Add(_listPhonesShowing[i].Extension.ToString());
				//Name--------------------------------------------------------------------------------------------------------------
				row.Cells.Add(_listPhonesShowing[i].EmployeeName);
				//Status------------------------------------------------------------------------------------------------------------
				//the old interface only showed Unavbl, Clock In, or blank. I think that's terrible,
				//so I'm showing all clockStatuses except Home, None, and Off.
				string clockStatus=_listPhonesShowing[i].ClockStatus.ToString();
				if(_listPhonesShowing[i].ClockStatus==ClockStatusEnum.Unavailable){
					clockStatus="Unavail";
				}
				if(_listPhonesShowing[i].ClockStatus.In(ClockStatusEnum.Home,ClockStatusEnum.None,ClockStatusEnum.Off)){
					clockStatus="ClockedOut";
				}
				row.Cells.Add(clockStatus);
				//Phone-------------------------------------------------------------------------------------------------------------
				string time="";
				if(_listPhonesShowing[i].DateTimeNeedsHelpStart.Date==DateTime.Today) {
					time=(DateTime.Now-_listPhonesShowing[i].DateTimeNeedsHelpStart+_timeSpanDelta).ToStringHmmss();
				}
				else if(_listPhonesShowing[i].DateTimeStart.Date==DateTime.Today) {
					time=(DateTime.Now-_listPhonesShowing[i].DateTimeStart+_timeSpanDelta).ToStringHmmss();
				}
				ChatUser chatUser=_listChatUsers.Find(x => x.Extension==_listPhonesShowing[i].Extension);
				WebChatSession webChatSession=_listWebChatSessions.Find(session => session.TechName==_listPhonesShowing[i].EmployeeName);
				PeerInfo peerInfo=_listPeerInfos.Find(x => x.EmployeeNum==_listPhonesShowing[i].EmployeeNum);
				if(_listPhonesShowing[i].ClockStatus.In(ClockStatusEnum.Home,ClockStatusEnum.None,ClockStatusEnum.Off)){
					row.Cells.Add("");
				}
				else if(_listPhonesShowing[i].Description!="") {//InUse
					row.Cells.Add("Phone");//Later, replace with phone icon.
				}
				else if(webChatSession!=null) {
					row.Cells.Add("WebChat");//Later, replace with WebChatIcon
					time=(DateTime.Now-webChatSession.DateTcreated).ToStringHmmss();
				}
				else if(chatUser!=null && chatUser.CurrentSessions!=0) {
					row.Cells.Add("Chat");//Later, replace with gtaicon3
					time=TimeSpan.FromMilliseconds(chatUser.SessionTime).ToStringHmmss();
				}
				else if(peerInfo!=null) {
					row.Cells.Add("RemoteSupp");//Later, replace with remoteSupportIcon
					time=peerInfo.SessionTime.ToStringHmmss();
				}
				else{
					row.Cells.Add("");
				}
				//Prox--------------------------------------------------------------------------------------------------------------
				Employee employee;
				try {
					employee=Employees.GetEmp(_listPhonesShowing[i].EmployeeNum);
				}
				catch {
					employee=null;
				}
				bool isWorkingHome=false;
				if(employee!=null) {
					isWorkingHome=employee.IsWorkingHome;
				}
				if(isWorkingHome) {//draw the home icon regardless of whether they are clocked in or out
					row.Cells.Add("AtHome");//todo: show _bitmapHouse
				}
				else if(_listPhonesShowing[i].ClockStatus.In(ClockStatusEnum.Unavailable,ClockStatusEnum.Home,ClockStatusEnum.None,ClockStatusEnum.Off)){
					row.Cells.Add("");
				}
				else if(_listPhonesShowing[i].IsProxVisible()) {
					row.Cells.Add("AtDesk");//Todo: show human figure icon
				}
				else if(_listPhonesShowing[i].DateTProximal.AddHours(8)>DateTime.Now ) {
					row.Cells.Add("Away");//Todo: show small circle icon
				}
				else{
					row.Cells.Add("");
				}
				//Time--------------------------------------------------------------------------------------------------------------
				Color colorOuter;
				Color colorInner;//not used
				Color colorFont;//not used
				bool isTriageOperatorOnTheClock=false;
				PhoneEmpDefault phoneEmpDefault=listPhoneEmpDefaults.Find(phone => phone.EmployeeNum==_listPhonesShowing[i].EmployeeNum);
				//phoneEmpDefault of null is ok in this method:
				Phones.GetPhoneColor(_listPhonesShowing[i],phoneEmpDefault,false,
					out colorOuter,out colorInner,out colorFont,out isTriageOperatorOnTheClock);
				//todo: get rid of the outs by splitting GetPhoneColor into multiple methods.
				Phones.PhoneColorScheme phoneColorScheme=new Phones.PhoneColorScheme(true);
				Color colorBar=colorOuter;
				if(_listPhonesShowing[i].ClockStatus==ClockStatusEnum.NeedsHelp) {
					if(_isFlashingPink) {
						colorBar=phoneColorScheme.ColorOuterNeedsHelp;
					}
					else {
						colorBar=Color.Empty;
					}
				}
				if(_listPhonesShowing[i].ClockStatus==ClockStatusEnum.HelpOnTheWay) {
					colorBar=phoneColorScheme.ColorOuterNeedsHelp;
				}
				if(_listPhonesShowing[i].ClockStatus.In(ClockStatusEnum.Home,ClockStatusEnum.None,ClockStatusEnum.Off)){
					row.Cells.Add("");//no time or color
				}
				else{
					GridCell gridCell=new GridCell(time);
					gridCell.ColorBackG=colorBar;
					row.Cells.Add(gridCell);
				}
				//Cubicle------------------------------------------------------------------------------------------------------------
				MapArea mapArea=_listMapAreas.Find(x => 
					x.Extension==_listPhonesShowing[i].Extension 
					&& x.Description!="");
				if(mapArea==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(mapArea.Description);
				}
				//Queue---------------------------------------------------------------------------------------------------------------
				if(_listPhonesShowing[i].RingGroups==AsteriskQueues.None){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listPhonesShowing[i].RingGroups.ToString());
				}
				//Customer------------------------------------------------------------------------------------------------------------
				row.Cells.Add(_listPhonesShowing[i].CustomerNumber);
				//Employee Notes--------------------------------------------------------------------------------------------------------
				if(phoneEmpDefault is null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(phoneEmpDefault.Notes);
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(idxSelected);
		}
		
		///<summary></summary>
		private void FilterAndSortPhoneList() {
			_listPhonesShowing=new List<Phone>();
			for(int i=0;i<_listPhonesAll.Count;i++) {
				if(checkHideClockedOut.Checked && _listPhonesAll[i].ClockStatus.In(ClockStatusEnum.None,ClockStatusEnum.Home,ClockStatusEnum.Off)) {
					//Show only clocked in employees. Don't add to filtered list.
				}
				else if(checkHideOnBreak.Checked && _listPhonesAll[i].ClockStatus.In(ClockStatusEnum.Break,ClockStatusEnum.Lunch)) {
					//Do not show phone tiles for phones that are on break or out to lunch when 'Hide on break' is checked.
				}
				else {
					_listPhonesShowing.Add(_listPhonesAll[i]);
				}
			}
			_listPhonesShowing=_listPhonesShowing.FindAll(x=>x.EmployeeName.ToLower().Contains(textNameOrExt.Text.ToLower()) || x.Extension.ToString().Contains(textNameOrExt.Text.ToLower()));
			if(listBoxQueueFilter.SelectedIndex>0){
				AsteriskQueues asteriskQueues=(AsteriskQueues)(listBoxQueueFilter.SelectedIndex-1);
				_listPhonesShowing=_listPhonesShowing.FindAll(x=>x.RingGroups==asteriskQueues);
			}
			if(checkNeedsHelpTop.Checked){
				if(radioByName.Checked){
					_listPhonesShowing=_listPhonesShowing.OrderBy(x=>x.ClockStatus!=ClockStatusEnum.NeedsHelp)
						.ThenBy(x=>x.EmployeeName=="")//empty names at bottom
						.ThenBy(x=>x.EmployeeName).ToList();
				}
				else{
					_listPhonesShowing=_listPhonesShowing.OrderBy(x=>x.ClockStatus!=ClockStatusEnum.NeedsHelp).ThenBy(x=>x.Extension).ToList();
				}
			}
			else{
				if(radioByName.Checked){
					_listPhonesShowing=_listPhonesShowing.OrderBy(x=>x.EmployeeName=="")//empty names at bottom
						.ThenBy(x=>x.EmployeeName).ToList();
				}
				else{
					_listPhonesShowing=_listPhonesShowing.OrderBy(x=>x.Extension).ToList();
				}
			}
		}

		///<summary>This also does a FillGrid.</summary>
		private void RefreshList() {
			PhoneEmpDefaults.RefreshCache();
			_listPhonesAll=Phones.GetPhoneList();
			_listChatUsers=ChatUsers.GetAll();
			_listWebChatSessions=WebChatSessions.GetActiveSessions();
			_listPeerInfos=PeerInfos.GetActiveSessions(false,true);
			_listMapAreas=MapAreas.Refresh();
			FilterAndSortPhoneList();
			FillGrid();
		}

		#endregion Methods - Private

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}
