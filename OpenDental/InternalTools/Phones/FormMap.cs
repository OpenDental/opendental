using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDental.InternalTools.Phones {
	public partial class FormMap:FormODBase {
		#region Fields - Public
		///<summary>Used when launching this form and we want to start with a specific map.</summary>
		public string MapDescription;
		#endregion Fields - Public

		#region Fields - Private
		///<summary>Tracks when chat count box needs to be set red.</summary>
		private int _chatRedCount=2;
		///<summary>Tracks when chat time box needs to be set red.</summary>
		private int _chatRedTimeMin=1;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<ChatUser> _listChatUsers;
		private List<MapAreaContainer> _listMapAreaContainers;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<PeerInfo> _listPeerInfosRemoteSupportSessions;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<WebChatSession> _listWebChatSessions;
		///<summary>This is the one that's showing.</summary>
		private MapAreaContainer _mapAreaContainer;
		///<summary>Used for the main menu and updates when the user is or is not in full screen mode.</summary>
		private UI.MenuItemOD _menuItemSettings;
		///<summary>The site that is associated with the first three octets of the computer that has launched this map.</summary>
		private Site _siteThisComputer;
		private int _triageCalls;
		private int _triageCallsWarning;
		private int _triageRedCalls;
		private int _triageRedTime;
		private int _triageTime;
		private int _triageTimeWarning;
		private int _voicemailCalls;
		private int _voicemailTime;
		#endregion Fields - Private

		#region Constructor
		public FormMap() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			escalationView.FadeAlphaIncrement=20;
			mapPanel.GoToPatient+=(sender,patNum)=>GoToPatient?.Invoke(sender,patNum);
			mapPanel.CubicleClicked+=(sender,cubicleClickedDetail)=>MapPanel_CubicleClicked(cubicleClickedDetail);
		}
		#endregion Constructor

		#region Events
		///<summary>When user to open a second map.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler ExtraMapClicked;

		///<summary>The eventHandler contains the patNum.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<long> GoToPatient;
		#endregion Events

		#region Methods - Public
		///<summary></summary>
		public void SetChatCount() {
			_listWebChatSessions??=WebChatSessions.GetActiveSessions();
			//get # of WebChats that still need to be claimed
			List<WebChatSession> listWebChatSessionsUnclaimed=_listWebChatSessions.FindAll(x => string.IsNullOrEmpty(x.TechName));
			DateTime dateTimeOldestWebChat=DateTime.MinValue;
			TimeSpan timeSpanChatBehind=TimeSpan.Zero;
			mapNumberChatCount.Text=listWebChatSessionsUnclaimed.Count.ToString();
			if(listWebChatSessionsUnclaimed.Count>0) {
				dateTimeOldestWebChat=listWebChatSessionsUnclaimed.Min(x => x.DateTcreated);
				timeSpanChatBehind=DateTime.Now-dateTimeOldestWebChat;
				mapNumberChatTime.Text=timeSpanChatBehind.ToStringmmss();
			}
			else {
				mapNumberChatTime.Text="00:00";
			}
			if(listWebChatSessionsUnclaimed.Count>=_chatRedCount) {
				mapNumberChatCount.SetAlertColors();
			}
			else {
				mapNumberChatCount.SetNormalColors();
			}
			if(timeSpanChatBehind>=TimeSpan.FromMinutes(_chatRedTimeMin)) {
				mapNumberChatTime.SetAlertColors();
			}
			else {
				mapNumberChatTime.SetNormalColors();
			}
		}
		
		public void SetEServiceMetrics(EServiceMetrics eServiceMetricsToday) {
			if(ODBuild.IsDebug()) {
				if(Environment.MachineName.ToLower()=="jordanhome"){
					return;
				}
			}
			eServiceMetricsControl.SetAccountBalance(eServiceMetricsToday.AccountBalanceEuro);
			if(eServiceMetricsToday.Severity==eServiceSignalSeverity.Critical) {
				eServiceMetricsControl.StartFlashing(eServiceMetricsToday.CriticalStatus);
			}
			else {
				eServiceMetricsControl.StopFlashing();
			}
			switch(eServiceMetricsToday.Severity) {
				case eServiceSignalSeverity.Working:
					eServiceMetricsControl.SetColorAlert(Color.LimeGreen);
					break;
				case eServiceSignalSeverity.Warning:
					eServiceMetricsControl.SetColorAlert(Color.Yellow);
					break;
				case eServiceSignalSeverity.Error:
					eServiceMetricsControl.SetColorAlert(Color.Orange);
					break;
				case eServiceSignalSeverity.Critical:
					eServiceMetricsControl.SetColorAlert(Color.Red);
					break;
			}
		}

		///<summary>Gets called every second or two from a thread.</summary>
		public void SetPhoneList(List<Phone> listPhones,List<PhoneEmpSubGroup> listPhoneEmpSubGroups,List<ChatUser> listChatUsers,
			List<WebChatSession> listWebChatSessions,List<PeerInfo> listPeerInfosRemoteSupportSessions)
		{
			_listWebChatSessions=listWebChatSessions;
			_listPeerInfosRemoteSupportSessions=listPeerInfosRemoteSupportSessions;
			_listChatUsers=listChatUsers;
			string title="Call Center Map - Triage Coord. - ";
			SiteLink siteLink=SiteLinks.GetFirstOrDefault(x => x.SiteNum==_mapAreaContainer.SiteNum);
			if(siteLink==null){
				title+="Not Set";
			}
			else{
				Employee employee=Employees.GetEmp(siteLink.EmployeeNum);
				if(employee is null){
					title+="Not Set";
				}
				else{
					title+=Employees.GetNameFL(employee);
				}
			}
			labelTriageCoordinator.Text=title;
			labelCurrentTime.Text=DateTime.Now.ToShortTimeString();
			#region Triage Counts
			//The triage count used to only count up the triage operators within the currently selected room.
			//Now we want to count all operators at the selected site (local) and then all operators across all sites (total).
			int triageStaffCountLocal=0;
			int triageStaffCountTotal=0;
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			List<PhoneEmpDefault> listPhoneEmpDefaultsTriage=listPhoneEmpDefaults.FindAll(x => x.IsTriageOperator && x.HasColor);
			for(int i=0;i<listPhoneEmpDefaultsTriage.Count;i++){
				Phone phone=listPhones.FirstOrDefault(x => x.Extension==listPhoneEmpDefaultsTriage[i].PhoneExt);
				if(phone==null) {
					continue;
				}
				if(phone.ClockStatus.In(ClockStatusEnum.None,ClockStatusEnum.Home,ClockStatusEnum.Lunch,ClockStatusEnum.Break,ClockStatusEnum.Off
					,ClockStatusEnum.Unavailable,ClockStatusEnum.NeedsHelp,ClockStatusEnum.HelpOnTheWay))
				{
					continue;
				}
				//This is a triage operator who is currently here and on the clock.
				if(listPhoneEmpDefaultsTriage[i].SiteNum==_mapAreaContainer.SiteNum) {
					triageStaffCountLocal++;
				}
				triageStaffCountTotal++;
			}
			mapNumberTriageOpsLocal.Text=triageStaffCountLocal.ToString();
			mapNumberTriageOpsTotal.Text=triageStaffCountTotal.ToString();
			#endregion
			mapPanel.SetPhoneList(listPhones, listChatUsers,listWebChatSessions,listPeerInfosRemoteSupportSessions);
			RefreshCurrentTabHelper(listPhoneEmpDefaults,listPhones,listPhoneEmpSubGroups);
		}

		///<summary></summary>
		public void SetOfficesDownList(List<Task> listTasksOfficeDown) {
			escalationViewOfficesDown.BeginUpdate();
			if(listTasksOfficeDown.IsNullOrEmpty()) {
				escalationViewOfficesDown.ListEscalationItems.Clear();
				mapNumberCustDownCount.Text="0";
				mapNumberCustDownTime.Text="0";
				escalationViewOfficesDown.EndUpdate();
				return;
			}
			//Sort list by oldest.
			listTasksOfficeDown=listTasksOfficeDown.OrderBy(x=>x.DateTimeEntry).ToList();
			List<EscalationItem> listEscalationItems=new List<EscalationItem>();
			for(int i=0;i<listTasksOfficeDown.Count;i++) {
				Task task=listTasksOfficeDown[i];
				if(task.TaskStatus==TaskStatusEnum.Done) { //Filter out old tasks. Should not be any but just in case.
					continue;
				}
				TimeSpan timeSpanActive=DateTime.Now.Subtract(task.DateTimeEntry);
				//We got this far so the office is down.
				EscalationItem escalationItem=new EscalationItem();
				escalationItem.EmpName=timeSpanActive.ToStringHmmss()+" - "+task.KeyNum.ToString();
				listEscalationItems.Add(escalationItem);
			}
			if(listEscalationItems.Count==0) {
				escalationViewOfficesDown.ListEscalationItems.Clear();
				mapNumberCustDownCount.Text="0";
				mapNumberCustDownTime.Text="0";
				escalationViewOfficesDown.EndUpdate();
				return;
			}
			escalationViewOfficesDown.ListEscalationItems=listEscalationItems;
			mapNumberCustDownCount.Text=listTasksOfficeDown.Count.ToString();
			//Get the time of the oldest task
			TimeSpan timeSpanOldest=DateTime.Now.Subtract(listTasksOfficeDown[0].DateTimeEntry);
			mapNumberCustDownTime.Text=((int)timeSpanOldest.TotalMinutes).ToString();
			escalationViewOfficesDown.EndUpdate();
		}

		///<summary></summary>
		public void SetTriageMain(int callsWithNotes,int callsWithNoNotes,TimeSpan timeSpanBehind,int triageRed) {
			if(timeSpanBehind==TimeSpan.Zero) { //format the string special for this case
				mapNumberTriageTime.Text="0";
			}
			else {
				mapNumberTriageTime.Text=((int)timeSpanBehind.TotalMinutes).ToString();
			}
			if(callsWithNoNotes>0 || triageRed>0) { //we have calls which don't have notes or a red triage task so display that number
				mapNumberTriageCalls.Text=(callsWithNoNotes+triageRed).ToString();
			}
			else { //we don't have any calls with no notes nor any red triage tasks so display count of total tasks
				mapNumberTriageCalls.Text="("+callsWithNotes.ToString()+")";
			}
			if(callsWithNoNotes+triageRed>=_triageCalls) { //we are behind
				mapNumberTriageCalls.SetAlertColors();
			}
			else if(callsWithNoNotes+triageRed>=_triageCallsWarning) { //we are approaching being behind
				mapNumberTriageCalls.SetWarnColors();
			}
			else { //we are ok
				mapNumberTriageCalls.SetNormalColors();
			}
			if(timeSpanBehind>=TimeSpan.FromMinutes(_triageTime)) { //we are behind
				mapNumberTriageTime.SetAlertColors();
			}
			else if(timeSpanBehind>=TimeSpan.FromMinutes(_triageTimeWarning)) { //we are approaching being behind
				mapNumberTriageTime.SetWarnColors();
			}
			else { //we are ok
				mapNumberTriageTime.SetNormalColors();
			}
		}
		
		///<summary></summary>
		public void SetTriageRed(int calls,TimeSpan timeSpanBehind) {
			mapNumberTriageRedCalls.Text=calls.ToString();
			if(timeSpanBehind==TimeSpan.Zero) { //format the string special for this case
				mapNumberTriageRedTime.Text="00:00";
			}
			else {
				this.mapNumberTriageRedTime.Text=timeSpanBehind.ToStringmmss();
			}
			if(calls>=_triageRedCalls) { //we are behind
				mapNumberTriageRedCalls.SetAlertColors();
			}
			else { //we are ok
				mapNumberTriageRedCalls.SetNormalColors();
			}
			if(timeSpanBehind>=TimeSpan.FromMinutes(_triageRedTime)) { //we are behind
				mapNumberTriageRedTime.SetAlertColors();
			}
			else { //we are ok
				mapNumberTriageRedTime.SetNormalColors();
			}
		}

		public void SetVoicemail(int calls,TimeSpan timeSpanBehind) {
			mapNumberVoicemailCalls.Text=calls.ToString();
			if(timeSpanBehind==TimeSpan.Zero) { //format the string special for this case
				mapNumberVoicemailTime.Text="00:00";
			}
			else {
				mapNumberVoicemailTime.Text=timeSpanBehind.ToStringmmss();
			}
			if(calls>=_voicemailCalls) { //we are behind
				mapNumberVoicemailCalls.SetAlertColors();
			}
			else { //we are ok
				mapNumberVoicemailCalls.SetNormalColors();
			}
			if(timeSpanBehind>=TimeSpan.FromMinutes(_voicemailTime)) { //we are behind
				mapNumberVoicemailTime.SetAlertColors();
			}
			else { //we are ok
				mapNumberVoicemailTime.SetNormalColors();
			}
		}
		#endregion Methods - Public

		#region Methods - Private, Event Handlers
		private void comboRoom_SelectionChangeCommitted(object sender,EventArgs e) {
			_mapAreaContainer=_listMapAreaContainers[comboRoom.SelectedIndex];
			this.Text="Call Center Status Map - "+_listMapAreaContainers[comboRoom.SelectedIndex].Description;
			FillMapPanel();
			FillTriageLabelColors();
			//refresh immediately instead of waiting a second for a new signal.
			ODThread.WakeUpThreadsByGroupName(FormOpenDental.FormODThreadNames.HqMetrics.GetDescription());
		}

		private void FormMapHQ2_FormClosed(object sender,FormClosedEventArgs e) {
			FormOpenDental.RemoveMapFromList2(this);
		}

		private void FormMap_Load(object sender,EventArgs e) {
			_siteThisComputer=SiteLinks.GetSiteByGateway();
			if(_siteThisComputer==null) {
				MessageBox.Show("Error.  No sites found in the cache.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			labelCurrentTime.Text=DateTime.Now.ToShortTimeString();
			LayoutMenu();
			FillMaps();
			FillTabs();
			FillCombo();
			FillMapPanel();
			FillTriagePreferences();
			FillTriageLabelColors();
			FormOpenDental.AddMapToList2(this);
			//refresh immediately instead of waiting a second for a new signal.
			ODThread.WakeUpThreadsByGroupName(FormOpenDental.FormODThreadNames.HqMetrics.GetDescription());
			//ProcessHqMetricsPhones();
		}

		private void tabMain_SelectedIndexChanged(object sender,EventArgs e) {
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			List<Phone> listPhones=OpenDentBusiness.Phones.GetPhoneList();
			List<PhoneEmpSubGroup> listPhoneEmpSubGroups=PhoneEmpSubGroups.GetAll();
			RefreshCurrentTabHelper(listPhoneEmpDefaults,listPhones,listPhoneEmpSubGroups);
		}
		#endregion Methods - Private, Event Handlers

		#region Methods - private Menu

		private void menuItemCallCenterThresh_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMapHQPrefs formMapHqPrefsTriage=new FormMapHQPrefs();
			formMapHqPrefsTriage.ShowDialog();
			if(formMapHqPrefsTriage.DialogResult!=DialogResult.OK) {
				return;
			}
			FillTriagePreferences();
		}

		private void menuItemConfRooms_Click(object sender,EventArgs e) {
			using FormPhoneConfs formPhoneConfs=new FormPhoneConfs();
			formPhoneConfs.ShowDialog();//ShowDialog because we do not want this window to be floating open for long periods of time.
		}

		private void menuItemEscalation_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormPhoneEmpDefaultEscalationEdit formPhoneEmpDefaultEscalationEdit=new FormPhoneEmpDefaultEscalationEdit();
			formPhoneEmpDefaultEscalationEdit.ShowDialog();
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			RefreshCurrentTabHelper(listPhoneEmpDefaults,OpenDentBusiness.Phones.GetPhoneList(),PhoneEmpSubGroups.GetAll());
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Escalation team changed");
		}

		private void menuItemEditMap_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMapAreaContainers formMapAreaContainers=new FormMapAreaContainers();
			formMapAreaContainers.ShowDialog();
			_listMapAreaContainers=MapAreaContainers.GetAll(_siteThisComputer.SiteNum);
			_mapAreaContainer=_listMapAreaContainers[comboRoom.SelectedIndex];
			FillCombo();
			FillMapPanel();
			ODThread.WakeUpThreadsByGroupName(FormOpenDental.FormODThreadNames.HqMetrics.GetDescription());
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"MapHQ layout changed");
		}

		private void menuItemOpenNewMap_Click(object sender,EventArgs e) {
			//Open the map from FormOpenDental so that it can register for the click events.
			ExtraMapClicked?.Invoke(this,e);
			//Form gets added to the list of all HQMaps in Load method.
		}

		private void menuItemPhoneEmpDefaults_Click(object sender,EventArgs e) {
			using FormPhoneEmpDefaults formPhoneEmpDefaults=new FormPhoneEmpDefaults();
			formPhoneEmpDefaults.ShowDialog();
		}

		private void menuItemToggleTriage_Click(object sender,EventArgs e) {
			Point pointLoc=LayoutManager.ScalePoint(new Point(0,78));
			if(panelContainer.Left==0) { 
				pointLoc=LayoutManager.ScalePoint(new Point(365,78));
			}
			Rectangle rectangle=new Rectangle(pointLoc.X,pointLoc.Y,ClientSize.Width-pointLoc.X-2,ClientSize.Height-pointLoc.Y-2);
			LayoutManager.Move(panelContainer,rectangle);
		}
		#endregion Methods - private Menu

		#region Methods - Private
		///<summary>Returns true if the employee for the PhoneEmpDefault should be added to the selected escalation view.</summary>
		private bool DoAddToEscalationView(PhoneEmpDefault phoneEmpDefault,List<Phone> listPhones) {
			if(phoneEmpDefault.EscalationOrder<=0) { //Filter out employees that do not have an escalation order set.
				return false;
			}
			if(phoneEmpDefault.IsTriageOperator) {
				return false;
			}
			Phone phone=OpenDentBusiness.Phones.GetPhoneForEmployeeNum(listPhones,phoneEmpDefault.EmployeeNum);
			if(phone==null || phone.Description!="") { //Filter out invalid employees or employees that are already on the phone.
				return false;
			}
			PhoneEmpSubGroupType phoneEmpSubGroupTypeSelected=(PhoneEmpSubGroupType)(tabMain.SelectedTab?.Tag??PhoneEmpSubGroupType.Escal);
			if(phoneEmpSubGroupTypeSelected==PhoneEmpSubGroupType.Avail) {//Special rules for the Avail escalation view
				if(!phone.IsProxVisible() && !Employees.GetEmp(phoneEmpDefault.EmployeeNum).IsWorkingHome) {
					return false;
				}
				if(!phone.ClockStatus.In(ClockStatusEnum.Available,ClockStatusEnum.Training,ClockStatusEnum.Backup)) {
					return false;
				}
				return true;
			}
			//All other escalation views besides Avail
			if(!phone.ClockStatus.In(ClockStatusEnum.Available,ClockStatusEnum.Backup)) {
				return false;
			}
			return true;
		}

		public void MapPanel_CubicleClicked(CubicleClickedDetail cubicleClickedDetail) {
			FillDetails(cubicleClickedDetail);
		}

		///<summary>Preserves _mapAreaContainer. Does not fill mapPanel.</summary>
		private void FillCombo() {
			comboRoom.Items.Clear();
			for (int i=0;i<_listMapAreaContainers.Count;i++) {
				comboRoom.Items.Add(_listMapAreaContainers[i].Description);
			}
			int selectedIndex=0;
			if(_mapAreaContainer!=null) {
				selectedIndex=_listMapAreaContainers.FindIndex(x => x.MapAreaContainerNum==_mapAreaContainer.MapAreaContainerNum);
			}
			if(selectedIndex==-1){
				selectedIndex=0;
			}
			comboRoom.SelectedIndex=selectedIndex;
			this.Text="Call Center Status Map - "+_listMapAreaContainers[comboRoom.SelectedIndex].Description;
		}

		///<summary>Sets the detail control to the clicked on cubicle as long as there is an associated phone.</summary>
		private void FillDetails(CubicleClickedDetail cubicleClickedDetail) {
			//set the text before the image, because the image can take time to load
			userControlMapDetails1.SetEmployee2(cubicleClickedDetail);
			userControlMapDetails1.Visible=true;
			Application.DoEvents();//process the events first so the text on the control will be set
			FillDetailImage(cubicleClickedDetail.EmployeeNum);
		}

		private void FillDetailImage(long employeeNum){
			//First, look for webcam image
			Bitmap bitmap=GetWebCamImage(employeeNum);
			if(bitmap!=null) {
				userControlMapDetails1.SetBitmap(bitmap,mapImageDisplayStatus:OpenDental.InternalTools.Phones.EnumMapImageDisplayStatus.WebCam,employeeNum);
				return;
			}
			//Then, look for a stock image
			if(userControlMapDetails1.MapImageDisplayStatus==OpenDental.InternalTools.Phones.EnumMapImageDisplayStatus.Stock//already showing stock
				&& userControlMapDetails1.EmployeeNumImage==employeeNum)//for this employee
			{
				return;
			}
			bitmap=GetEmployeePicture(employeeNum);
			if(bitmap!=null){
				userControlMapDetails1.SetBitmap(bitmap,mapImageDisplayStatus:OpenDental.InternalTools.Phones.EnumMapImageDisplayStatus.Stock,employeeNum);
				return;
			}
			userControlMapDetails1.SetBitmap(null,mapImageDisplayStatus:OpenDental.InternalTools.Phones.EnumMapImageDisplayStatus.Empty,employeeNum);
		}

		///<summary>This timer is always running.  4 times per second.</summary>
		private void timerWebCam_Tick(object sender,EventArgs e) {
			if(userControlMapDetails1.EmployeeNumCur<0){
				return;
			}
			FillDetailImage(userControlMapDetails1.EmployeeNumCur);
		}

		///<summary>Returns null if not found</summary>
		private Bitmap GetWebCamImage(long employeeNum) {
			Employee employee=Employees.GetEmp(employeeNum);
			if(employee==null) {
				return null;
			}
			//employee.FName should look like FirstL
			string filePathWebCam=@"\\10.10.11.126\HQVideo\"+employee.FName+@"\webcam.jpg";
			if(ODBuild.IsDebug()) {
				if(Environment.MachineName.ToLower()=="jordanhome"){
					filePathWebCam=@"C:\HQVideo\"+employee.FName+@"\webcam.jpg";
				}
			}
			if(!File.Exists(filePathWebCam)){
				return null;
			}
			Bitmap bitmap=null;
			try{
				bitmap=(Bitmap)Image.FromFile(filePathWebCam);
			}
			catch{ }
			Bitmap bitmapResized=null;
			if(bitmap!=null){
				bitmapResized=new Bitmap(bitmap);
			}
			bitmap?.Dispose();//to release file lock
			return bitmapResized;
		}

		private Bitmap GetEmployeePicture(long employeeNum){
			Employee employee=Employees.GetEmp(employeeNum);
			if(employee==null) {
				return null;
			}
			//Only grab the first part of the FName if there are multiple uppercased parts (ie. StevenS should be Steven).
			string fname=Regex.Split(employee.FName,@"(?<!^)(?=[A-Z])")[0];
			string employeeName=fname+" "+employee.LName;
			string dir=@"\\opendental.od\SERVERFILES\Storage\OPEN DENTAL\Staff\Staff Photos";
			if(ODBuild.IsDebug()) {
				if(Environment.MachineName.ToLower()=="jordanhome"){
					dir=@"E:\Documents\Open Dental\Photos Staff";
				}
			}
			List<string> listFiles=Directory.GetFiles(dir).ToList().FindAll(x => x.ToLower().EndsWith(employeeName.ToLower()+".jpg"));
			if(listFiles.Count==0){
				return null;
			}
			using Bitmap bitmapOrig=(Bitmap)Image.FromFile(listFiles[0]);
			//to release file lock as well as resize
			Bitmap bitmap2=new Bitmap(bitmapOrig,new System.Drawing.Size(bitmapOrig.Width/8,bitmapOrig.Height/8));
			return bitmap2;
		}

		///<summary>Setup the map panel with the cubicles and labels before filling with real-time data. Call this on load or anytime the cubicle layout has changed.</summary>
		private void FillMapPanel() {
			if(comboRoom.SelectedIndex==-1){
				return;
			}
			MapAreaContainer mapAreaContainer=_listMapAreaContainers[comboRoom.SelectedIndex];
			LayoutManager.MoveSize(mapPanel,new Size(mapAreaContainer.FloorWidthFeet*17,mapAreaContainer.FloorHeightFeet*17));
			List<MapArea> listMapAreas=MapAreas.Refresh(mapAreaContainer.MapAreaContainerNum);
			mapPanel.SetListMapAreas(listMapAreas);
			mapPanel.Invalidate();
			/*
			mapAreaPanel.PixelsPerFoot=LayoutManager.Scale(17);
			mapAreaPanel.Clear(false);
			mapAreaPanel.HeightFloorFeet=Math.Max(_mapAreaContainer.FloorHeightFeet,55);//Should at least fill the space set in the designer.
			mapAreaPanel.WidthFloorFeet=Math.Max(_mapAreaContainer.FloorWidthFeet,89);//Should at least fill the space set in the designer.
																			  //fill the panel
			_bitmapHouse?.Dispose();//We do not know if the bitmap was disposed of by MapCubicle.Dispose() method, so manually dispose to be safe
			_bitmapHouse=PhoneTile.GetHouse16();//Since we disposed above, we need to recreate the house bitmap
			List<MapArea> listMapAreas=MapAreas.Refresh(_listMapAreaContainers[comboRoom.SelectedIndex].MapAreaContainerNum);
			listMapAreas=listMapAreas.OrderByDescending(x => (int)(x.ItemType)).ToList();
			for(int i=0;i<listMapAreas.Count;i++) {
				if(listMapAreas[i].MapAreaContainerNum!=_listMapAreaContainers[comboRoom.SelectedIndex].MapAreaContainerNum) {
					continue;
				}
				if(listMapAreas[i].ItemType==MapItemType.Cubicle) {
					mapAreaPanel.AddCubicle(listMapAreas[i]);
				}
				else if(listMapAreas[i].ItemType==MapItemType.Label) {
					mapAreaPanel.AddDisplayLabel(listMapAreas[i]);
				}
			}*/
		}

		private void FillMaps() {
			_listMapAreaContainers=MapAreaContainers.GetAll(_siteThisComputer.SiteNum);
			if(MapDescription.IsNullOrEmpty()) {
				_mapAreaContainer=_listMapAreaContainers[0];
				return;
			}
			//Or find the desired Maps passed in from MapDescription.
			_mapAreaContainer=_listMapAreaContainers.FirstOrDefault(x => x.Description.ToLower()==MapDescription.ToLower());
			if(_mapAreaContainer==null) {
				//Default if MapDescription not found.
				_mapAreaContainer=_listMapAreaContainers[0];
			}
		}

		private void FillTabs() {
			tabMain.TabPages.Clear();
			UI.TabPage tabPage=new UI.TabPage(PhoneEmpSubGroupType.Avail.ToString());
			tabPage.Name=PhoneEmpSubGroupType.Avail.ToString();
			tabPage.Tag=PhoneEmpSubGroupType.Avail;
			LayoutManager.Add(tabPage,tabMain);
			List<PhoneEmpSubGroupType> listPhoneEmpSubGroupTypes=Enum.GetValues(typeof(PhoneEmpSubGroupType)).Cast<PhoneEmpSubGroupType>().ToList();
			for(int i=0;i<listPhoneEmpSubGroupTypes.Count;i++){
				if(listPhoneEmpSubGroupTypes[i]==PhoneEmpSubGroupType.Avail) {
					continue;//Already added above
				}
				tabPage=new UI.TabPage(listPhoneEmpSubGroupTypes[i].ToString());
				tabPage.Name=listPhoneEmpSubGroupTypes[i].ToString();
				tabPage.Tag=listPhoneEmpSubGroupTypes[i];
				LayoutManager.Add(tabPage,tabMain);
			}
		}

		private void FillTriageLabelColors() {
			mapNumberTriageOpsLocal.SetTriageColors(_mapAreaContainer.SiteNum);
			mapNumberTriageOpsTotal.SetTriageColors();
		}

		/// <summary>Gets the preferences from the database that determine when the map alert colors change.</summary>
		private void FillTriagePreferences() {
			_triageRedCalls=PrefC.GetInt(PrefName.TriageRedCalls);
			_triageCalls=PrefC.GetInt(PrefName.TriageCalls);
			_triageCallsWarning=PrefC.GetInt(PrefName.TriageCallsWarning);
			_triageRedTime=PrefC.GetInt(PrefName.TriageRedTime);
			_triageTime=PrefC.GetInt(PrefName.TriageTime);
			_triageTimeWarning=PrefC.GetInt(PrefName.TriageTimeWarning);
			_voicemailCalls=PrefC.GetInt(PrefName.VoicemailCalls);
			_voicemailTime=PrefC.GetInt(PrefName.VoicemailTime);
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			//Settings-----------------------------------------------------------------------------------------------------------
			_menuItemSettings=new UI.MenuItemOD("Setup");
			menuMain.Add(_menuItemSettings);
			_menuItemSettings.Add("Call Center Thresholds",menuItemCallCenterThresh_Click);
			_menuItemSettings.Add("Escalation",menuItemEscalation_Click);
			_menuItemSettings.Add("Edit Maps",menuItemEditMap_Click);
			_menuItemSettings.Add("Phone Emp Defaults",menuItemPhoneEmpDefaults_Click);
			//Toggle Triage View--------------------------------------------------------------------------------------------------
			menuMain.Add(new UI.MenuItemOD("Toggle Triage View",menuItemToggleTriage_Click));
			//Open New Map--------------------------------------------------------------------------------------------------
			menuMain.Add(new UI.MenuItemOD("Open New Map",menuItemOpenNewMap_Click));
			//Conf Rooms--------------------------------------------------------------------------------------------------
			menuMain.Add(new UI.MenuItemOD("Conf Rooms",menuItemConfRooms_Click));
			menuMain.EndUpdate();
		}

		///<summary>If the phone map has been changed, update all phone maps.</summary>
		public override void ProcessSignalODs(List<Signalod> listSignalods) {
			if(listSignalods.Exists(x => x.IType==InvalidType.PhoneMap)) {
				FillCombo();
				FillMapPanel();
				ODThread.WakeUpThreadsByGroupName(FormOpenDental.FormODThreadNames.HqMetrics.GetDescription());
			}
		}

		private void RefreshCurrentTabHelper(List<PhoneEmpDefault> listPhoneEmpDefaultsIn,List<Phone> listPhones,List<PhoneEmpSubGroup> listPhoneEmpSubGroupsIn) {
			PhoneEmpSubGroupType phoneEmpSubGroupTypeSelected=(PhoneEmpSubGroupType)(tabMain.SelectedTab?.Tag??PhoneEmpSubGroupType.Escal);
			List<PhoneEmpSubGroup> listPhoneEmpSubGroups=listPhoneEmpSubGroupsIn.FindAll(x => x.SubGroupType==phoneEmpSubGroupTypeSelected);
			//List of EmployeeNums to show for current tab.
			List<long> listEmployeeNums=listPhoneEmpSubGroups.Select(y => y.EmployeeNum).ToList();
			List<PhoneEmpDefault> listPhoneEmpDefaultsInGroup=listPhoneEmpDefaultsIn.FindAll(x => listEmployeeNums.Contains(x.EmployeeNum));//Employees who belong to this sub group.
			for (int i=0;i<listPhoneEmpDefaultsInGroup.Count;i++) {
				listPhoneEmpDefaultsInGroup[i].EscalationOrder=listPhoneEmpSubGroups.First(x => x.EmployeeNum==listPhoneEmpDefaultsInGroup[i].EmployeeNum).EscalationOrder;
			}
			//Set Escalation List------------------------------------------------------------------------------
			List<EscalationItem> listEscalationItems=new List<EscalationItem>();
			escalationView.BeginUpdate();
			if(escalationView.Tag==null || (((int)escalationView.Tag)!=tabMain.SelectedIndex)) {
				escalationView.IsNewItems=true;
				escalationView.Tag=tabMain.SelectedIndex;
			}
			List<PhoneEmpDefault> listPhoneEmpDefaultsEscalation=listPhoneEmpDefaultsInGroup.FindAll(x => DoAddToEscalationView(x,listPhones));
			List<PhoneEmpDefault> listPhoneEmpDefaults=SortForEscalationView(listPhoneEmpDefaultsEscalation,listPhones);
			if(_listChatUsers==null) {
				_listChatUsers=ChatUsers.GetAll();
			}
			if(_listWebChatSessions==null) {
				_listWebChatSessions=WebChatSessions.GetActiveSessions();
			}
			if(_listPeerInfosRemoteSupportSessions==null) {
				_listPeerInfosRemoteSupportSessions=PeerInfos.GetActiveSessions(false,true);
			}
			for(int i=0;i<listPhoneEmpDefaults.Count;i++) {
				Phone phone=listPhones.Find(x=>x.EmployeeNum==listPhoneEmpDefaults[i].EmployeeNum);
				if(phone is null){
					phone=new Phone();
				}
				EscalationItem escalationItem=new EscalationItem();
				escalationItem.EmpName=listPhoneEmpDefaults[i].EmpName;
				//Only show the proximity icon if the phone.IsProxVisible AND the employee is at the same site as our currently selected room.
				escalationItem.IsProximity=_mapAreaContainer.SiteNum==listPhoneEmpDefaults[i].SiteNum && phone.IsProxVisible();
				//PeerInfo peerInfoRemoteSupportSession=_listPeerInfosRemoteSupportSessions.Find(x => x.EmployeeNum==phone.EmployeeNum);//not used?
				escalationItem.IsWebChat=_listWebChatSessions.Any(x => x.TechName==phone.EmployeeName);
				escalationItem.IsGTAChat=_listChatUsers.Any(x => x.Extension==listPhoneEmpDefaults[i].PhoneExt && x.CurrentSessions>0);
				escalationItem.IsRemoteSupport=_listPeerInfosRemoteSupportSessions.Any(x => x.EmployeeNum==phone.EmployeeNum);
				//Extensions will always show if they are working from home or for both locations unless the employee is not proximal. 
				if(Employees.GetEmp(listPhoneEmpDefaults[i].EmployeeNum).IsWorkingHome) {
					escalationItem.ShowExtension=true;
				} 
				else {
					escalationItem.ShowExtension=phone.IsProxVisible();
				}
				escalationItem.Extension=listPhoneEmpDefaults[i].PhoneExt;
				listEscalationItems.Add(escalationItem);
			}
			escalationView.ListEscalationItems=listEscalationItems;
			escalationView.EndUpdate();
		}

		///<summary>Sorts the list of PhoneEmpDefaults in the appropriate way for the selected escalation view.</summary>
		private List<PhoneEmpDefault> SortForEscalationView(List<PhoneEmpDefault> listPhoneEmpDefaults,List<Phone> listPhones) {
			PhoneEmpSubGroupType phoneEmpSubGroupTypeSelected=(PhoneEmpSubGroupType)(tabMain.SelectedTab?.Tag??PhoneEmpSubGroupType.Escal);
			if(phoneEmpSubGroupTypeSelected==PhoneEmpSubGroupType.Avail) {
				Func<PhoneEmpDefault,Phone> funcGetPhone=new Func<PhoneEmpDefault,Phone>((phoneEmpDef) => {
					Phone phone=listPhones.Find(x=>x.EmployeeNum==phoneEmpDef.EmployeeNum);
					if(phone is null){
						return new Phone();
					}
					return phone;
				});
				return listPhoneEmpDefaults.OrderBy(x => funcGetPhone(x).ClockStatus!=ClockStatusEnum.Available)//Show Available first
					.ThenBy(x => funcGetPhone(x).ClockStatus!=ClockStatusEnum.Training)//Training next
					.ThenBy(x => funcGetPhone(x).ClockStatus!=ClockStatusEnum.Backup)//Backup next
					.ThenBy(x => funcGetPhone(x).DateTimeStart.Year < 1880)//Show people who have an actual DateTimeStart first
					.ThenBy(x => funcGetPhone(x).DateTimeStart)//Show those first who have been in this status longest
					.ToList();
			}
			//All other escalation views beside Avail
			return listPhoneEmpDefaults.OrderBy(x => x.EscalationOrder)//Show people at the selected location first
				.ThenBy(x => x.EmpName).ToList();
		}

		#endregion Methods - Private

		
	}
}

/*
Todo:
Spacing on the small cubicles. Might already be fixed.
Change date/time slightly on local machine and see if it still displays properly.

Corruption Fixed:
FormPhoneTiles _listMapAreaContainers line 850
SELECT maparea.* 
FROM maparea
LEFT JOIN mapareacontainer
ON maparea.MapAreaContainerNum=mapareacontainer.MapAreaContainerNum
WHERE mapareacontainer.MapAreaContainerNum IS NULL;
DELETE FROM maparea WHERE MapAreaContainerNum<5;

*/