﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormMapHQ:FormODBase {
		#region Events
		public event EventHandler RoomControlClicked;
		public event EventHandler ExtraMapClicked;
		[Category("OD")]
		[Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler GoToChanged=null;
		#endregion
		#region Public Members
		public string MapDescription=null;
		#endregion
		#region Private Members
		///<summary>This is the difference between server time and local computer time.  Used to ensure that times displayed are accurate to the second.  This value is usally just a few seconds, but possibly a few minutes.</summary>
		private TimeSpan _timeSpanDelta;
		private List<MapAreaContainer> _listMapAreaContainers;
		private MapAreaContainer _mapAreaContainer;
		///<summary>The site that is associated to the first three octets of the computer that has launched this map.</summary>
		private Site _site;
		//preferences for setting triage alert colors
		private int _triageRedCalls,_triageRedTime,_voicemailCalls,_voicemailTime,_triageCalls,_triageTime,_triageTimeWarning,_triageCallsWarning;
		///<summary>Tracks when chat boxes for map need to be set red.</summary>
		private int _chatRedCount=2;
		///<summary>Tracks when chat boxes for map need to be set red.</summary>
		private int _chatRedTimeMin=1;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<WebChatSession> _listWebChatSessions;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<PeerInfo> _listPeerInfosRemoteSupportSessions;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<ChatUser> _listChatUsers;
		///<summary>Used for the main menu and updates when the user is or is not in full screen mode.</summary>
		private MenuItemOD _menuItemSettings;
		private MapCubicle _mapCubicleSelected;
		///<summary>Used to store the house icon that is displayed on cubicles if the employee is working from home.</summary>
		private Bitmap _bitmapHouse;

		private PhoneEmpSubGroupType GetCurSubGroupType() {
			if(!tabMain.TabPages.ContainsKey(PhoneEmpSubGroupType.Escal.ToString())) {//Control has not been initialized.
				return PhoneEmpSubGroupType.Escal;
			}
			return (PhoneEmpSubGroupType)(tabMain.SelectedTab.Tag??PhoneEmpSubGroupType.Escal);
		}
		#endregion Private Members

		#region Initialize

		public FormMapHQ() {
			InitializeComponent();
			//This form needs to dynamically add, remove, and dispose controls which remove is not currently supported when using custom borders.
			//isLayoutMS handles most of the problems.
			//is96dpi is turned on to allow proper drawing in the map area when zoom is used.
			//It's a bit hacky, and really just needs an overhaul
			InitializeLayoutManager(isLayoutMS:true,is96dpi:true);
			//Do not do anything to do with database or control init here. We will be using this ctor later in order to create a temporary object so we can figure out what size the form should be when the user comes back from full screen mode. Wait until FormMapHQ_Load to do anything meaningful.
			//_isFullScreen=false;
			_timeSpanDelta=MiscData.GetNowDateTime()-DateTime.Now;
			//Add the mousewheel event to allow mousewheel scrolling to repaint the grid as well.
			mapAreaPanel.MouseWheel+=new MouseEventHandler(mapAreaPanelHQ_Scroll);
			mapAreaPanel.MapCubicleClicked+=MapAreaPanel_CubicleClicked;
			mapAreaPanel.ClickedGoTo+=(sender,e)=>GoToChanged?.Invoke(sender,new EventArgs());//just bubble up
		}

		public void MapAreaPanel_CubicleClicked(object sender,EventArgs e) {
			MapCubicle mapCubicle=(MapCubicle)sender;
			if(mapCubicle==null) {
				return;
			}
			FillDetails(mapCubicle);
			RoomControlClicked?.Invoke(sender,e);
		}

		private void FormMapHQ_Load(object sender,EventArgs e) {
			LayoutControls();
			_site=SiteLinks.GetSiteByGateway();
			if(_site==null) {
				MessageBox.Show("Error.  No sites found in the cache.");
				DialogResult=DialogResult.Abort;
				Close();
				return;
			}
			FormOpenDental.AddMapToList(this);
			LayoutMenu();
			FillMaps();
			FillTabs();
			FillCombo();
			FillMapAreaPanel();
			FillTriagePreferences();
			FillTriageLabelColors();
		}

		private void FillMaps() {
			//Get the list of maps from our JSON preference.
			_listMapAreaContainers=PhoneMapJSON.GetFromDb();
			//Add a custom order to this map list which will prefer maps that are associated to the local computer's site.
			_listMapAreaContainers=_listMapAreaContainers.OrderBy(x => x.SiteNum!=_site.SiteNum)
				.ThenBy(x => x.Description).ToList();
			//Select the first map in our list that matches the site associated to the current computer.
			if(MapDescription.IsNullOrEmpty()) {
				_mapAreaContainer=_listMapAreaContainers[0];
			}
			else {
				//Or find the desired Maps passed in from MapDescription.
				_mapAreaContainer=_listMapAreaContainers.FirstOrDefault(x => x.Description.ToLower()==MapDescription.ToLower());
				if(_mapAreaContainer==null) {
					//Default if MapDescription not found.
					_mapAreaContainer=_listMapAreaContainers[0];
				}
			}
		}

		private void FillTriageLabelColors() {
			labelTriageOpsCountLocal.SetTriageColors(_mapAreaContainer.SiteNum);
			labelTriageOpsCountTotal.SetTriageColors();
		}

		private void FillTabs() {
			//jordan: this should be failing. Not allowed. Revisit.
			foreach(TabPage tab in tabMain.TabPages) {
				LayoutManager.Remove(tab);
				//tabMain.TabPages.Remove(tab);
				tab.Dispose();
			}
			TabPage tabPage=new TabPage(PhoneEmpSubGroupType.Avail.ToString());
			tabPage.Name=PhoneEmpSubGroupType.Avail.ToString();
			tabPage.Tag=PhoneEmpSubGroupType.Avail;
			LayoutManager.Add(tabPage,tabMain);
			//tabMain.TabPages.Add(tabPage);
			List<PhoneEmpSubGroupType> listPhoneempSubGroupTypes=Enum.GetValues(typeof(PhoneEmpSubGroupType)).Cast<PhoneEmpSubGroupType>().ToList();
			foreach(PhoneEmpSubGroupType phoneEmpSubGroupType in listPhoneempSubGroupTypes) {
				if(phoneEmpSubGroupType==PhoneEmpSubGroupType.Avail) {
					continue;//Already added above
				}
				tabPage=new TabPage(phoneEmpSubGroupType.ToString());
				tabPage.Name=phoneEmpSubGroupType.ToString();
				tabPage.Tag=phoneEmpSubGroupType;
				LayoutManager.Add(tabPage,tabMain);
				//tabMain.TabPages.Add(tabPage);
			}
		}

		private void FillCombo() {
			comboRoom.Items.Clear();
			foreach(MapAreaContainer mapCur in _listMapAreaContainers) {
				comboRoom.Items.Add(mapCur.Description);
			}
			int selectedIndex=0;
			if(_mapAreaContainer!=null) {
				selectedIndex=_listMapAreaContainers.FindIndex(x => x.MapAreaContainerNum==_mapAreaContainer.MapAreaContainerNum);
			}
			comboRoom.SelectedIndex=(selectedIndex==-1 ? 0 : selectedIndex);
			this.Text="Call Center Status Map - "+_listMapAreaContainers[comboRoom.SelectedIndex].Description;
		}

		///<summary>Setup the map panel with the cubicles and labels before filling with real-time data. Call this on load or anytime the cubicle layout has changed.</summary>
		private void FillMapAreaPanel() {
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
			}
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

		#endregion

		#region Set label text and colors

		public void SetEServiceMetrics(EServiceMetrics metricsToday) {
			if(ODBuild.IsDebug()) {
				if(Environment.MachineName.ToLower()=="jordanhome"){
					return;
				}
			}
			eServiceMetricsControl.AccountBalance=metricsToday.AccountBalanceEuro;
			if(metricsToday.Severity==eServiceSignalSeverity.Critical) {
				eServiceMetricsControl.StartFlashing(metricsToday.CriticalStatus);
			}
			else {
				eServiceMetricsControl.StopFlashing();
			}
			switch(metricsToday.Severity) {
				case eServiceSignalSeverity.Working:
					eServiceMetricsControl.AlertColor=Color.LimeGreen;
					break;
				case eServiceSignalSeverity.Warning:
					eServiceMetricsControl.AlertColor=Color.Yellow;
					break;
				case eServiceSignalSeverity.Error:
					eServiceMetricsControl.AlertColor=Color.Orange;
					break;
				case eServiceSignalSeverity.Critical:
					eServiceMetricsControl.AlertColor=Color.Red;
					break;
			}
		}

		///<summary>Refresh the phone panel every X seconds after it has already been setup.  Make sure to call FillMapAreaPanel before calling this the first time.</summary>
		public void SetPhoneList(List<Phone> phones,List<PhoneEmpSubGroup> listSubGroups,List<ChatUser> listChatUsers,
			List<WebChatSession> listWebChatSessions,List<PeerInfo> listRemoteSupportSessions)
		{
			//refresh our lists to minimize trips to the database.
			_listWebChatSessions=listWebChatSessions;
			_listPeerInfosRemoteSupportSessions=listRemoteSupportSessions;
			_listChatUsers=listChatUsers;
			string title="Call Center Map - Triage Coord. - ";
			try { //get the triage coord label but don't fail just because we can't find it
				SiteLink siteLink=SiteLinks.GetFirstOrDefault(x => x.SiteNum==_mapAreaContainer.SiteNum);
				title+=Employees.GetNameFL(Employees.GetEmp(siteLink.EmployeeNum));
			}
			catch {
				title+="Not Set";
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
				Phone phone=phones.FirstOrDefault(x => x.Extension==listPhoneEmpDefaultsTriage[i].PhoneExt);
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
			labelTriageOpsCountLocal.Text=triageStaffCountLocal.ToString();
			labelTriageOpsCountTotal.Text=triageStaffCountTotal.ToString();
			#endregion
			List<Control> listAreaPanelControls = new List<Control>();
			listAreaPanelControls.AddRange(this.mapAreaPanel.Controls.OfType<Control>());
			//listMapAreaRoomControls.Add(_mapCubicleSelected);
			for(int i=0;i<listAreaPanelControls.Count;i++) { //loop through all of our cubicles and labels and find the matches
				if(!(listAreaPanelControls[i] is MapCubicle)) {
					continue;
				}
				MapCubicle mapCubicle=(MapCubicle)listAreaPanelControls[i];
				if(mapCubicle.MapAreaCur.Extension==0) { //This cubicle has not been given an extension yet.
					mapCubicle.Empty=true;
					continue;
				}
				Phone phone=Phones.GetPhoneForExtension(phones,mapCubicle.MapAreaCur.Extension);
				if(phone==null) {//We have a cubicle with no corresponding phone entry.
					mapCubicle.Empty=true;
					continue;
				}
				mapCubicle.PhoneCur=phone;//Refresh PhoneCur so that it has up to date customer information.
				ChatUser chatuser=listChatUsers.Where(x => x.Extension == phone.Extension).FirstOrDefault();
				PhoneEmpDefault phoneEmpDefault=PhoneEmpDefaults.GetEmpDefaultFromList(phone.EmployeeNum,listPhoneEmpDefaults);
				if(phoneEmpDefault==null) {//We have a cubicle with no corresponding phone emp default entry.
					mapCubicle.Empty=true;
					continue;
				}
				//we got this far so we found a corresponding cubicle for this phone entry
				mapCubicle.EmployeeNum=phone.EmployeeNum;
				mapCubicle.EmployeeName=phone.EmployeeName;
				WebChatSession webChatSession=listWebChatSessions.FirstOrDefault(x => x.TechName==phone.EmployeeName);
				PeerInfo remoteSupportSession=listRemoteSupportSessions.FirstOrDefault(x => x.EmployeeNum==phone.EmployeeNum);
				if(phone.DateTimeNeedsHelpStart.Date==DateTime.Today) { //if they need help, use that time.
					TimeSpan span=DateTime.Now-phone.DateTimeNeedsHelpStart+_timeSpanDelta;
					DateTime timeOfDay=DateTime.Today+span;
					mapCubicle.Elapsed=span;
				}
				else if(phone.DateTimeStart.Date==DateTime.Today && phone.Description != "") { //else if in a call, use call time.
					TimeSpan span=DateTime.Now-phone.DateTimeStart+_timeSpanDelta;
					DateTime timeOfDay=DateTime.Today+span;
					mapCubicle.Elapsed=span;
				}
				else if(phone.Description=="" && webChatSession!=null ) {//else if in a web chat session, use web chat session time
					TimeSpan span=DateTime.Now-webChatSession.DateTcreated+_timeSpanDelta;
					mapCubicle.Elapsed=span;	
				}
				else if(phone.Description=="" && chatuser!=null && chatuser.CurrentSessions>0) { //else if in a chat, use chat time.
					TimeSpan span=TimeSpan.FromMilliseconds(chatuser.SessionTime)+_timeSpanDelta;
					mapCubicle.Elapsed=span;
				}
				else if(phone.Description=="" && remoteSupportSession!=null) {
					//Might need to enhance later to get a 'timeDelta' for the Remote Support server.
					mapCubicle.Elapsed=remoteSupportSession.SessionTime;
				}
				else if(phone.DateTimeStart.Date==DateTime.Today) { //else available, use that time.
					TimeSpan span = DateTime.Now-phone.DateTimeStart+_timeSpanDelta;
					DateTime timeOfDay = DateTime.Today+span;
					mapCubicle.Elapsed=span;
				}
				else { //else, whatever.
					mapCubicle.Elapsed=TimeSpan.Zero;
				}
				if(phone.IsProxVisible) {
					mapCubicle.ProxImage=Properties.Resources.Figure;
				}
				else if(phone.DateTProximal.AddHours(8)>DateTime.Now) {
					mapCubicle.ProxImage=Properties.Resources.NoFigure;//TODO: replace image with one from Nathan
				}
				else {
					mapCubicle.ProxImage=null;
				}
				mapCubicle.IsAtDesk=phone.IsProxVisible;
				string status=Phones.ConvertClockStatusToString(phone.ClockStatus);
				//Check if the user is logged in.
				if(phone.ClockStatus==ClockStatusEnum.None
					||phone.ClockStatus==ClockStatusEnum.Home) {
					status="Home";
				}
				mapCubicle.Status=status;
				if(phone.Description=="") {
					mapCubicle.PhoneImage=null;
					mapCubicle.WebChatImage=null;
					mapCubicle.ChatImage=null;
					mapCubicle.RemoteSupportImage=null;
					if(webChatSession!=null) {//active web chat session
						mapCubicle.WebChatImage=Properties.Resources.WebChatIcon;
					}
					else if(chatuser!=null && chatuser.CurrentSessions!=0) {//check for GTA sessions if no web chats
						mapCubicle.ChatImage=Properties.Resources.WebChatIcon;
					}
					else if(remoteSupportSession!=null) {
						mapCubicle.RemoteSupportImage=Properties.Resources.remoteSupportIcon;
					}
				}
				else {
					mapCubicle.PhoneImage=Properties.Resources.phoneInUse;
				}
				Color outerColor;
				Color innerColor;
				Color fontColor;
				bool isTriageOperatorOnTheClock;
				//get the cubicle color and triage status
				Phones.GetPhoneColor(phone,phoneEmpDefault,true,out outerColor,out innerColor,out fontColor,out isTriageOperatorOnTheClock);
				if(!mapCubicle.IsFlashing()) { //if the control is already flashing then don't overwrite the colors. this would cause a "spastic" flash effect.
					mapCubicle.OuterColor=outerColor;
					mapCubicle.InnerColor=innerColor;
				}
				mapCubicle.ForeColor=fontColor;
				if(phone.ClockStatus==ClockStatusEnum.NeedsHelp) { //turn on flashing
					mapCubicle.StartFlashing();
				}
				else { //turn off flashing
					mapCubicle.StopFlashing();
				}
				if(phone.EmployeeNum>0 && phone.EmployeeNum==userControlMapDetails1.EmployeeNumCur) {
					userControlMapDetails1.UpdateControl(mapCubicle);
				}
				Employee employee=Employees.GetEmp(phone.EmployeeNum);//from cache
				if(employee!=null){
					if(employee.IsWorkingHome) {
						mapCubicle.ProxImage=_bitmapHouse;
					}
					//plan to use this to show extra employee fields, like cellphone
				}
				mapCubicle.Invalidate(true);
			}
			refreshCurrentTabHelper(listPhoneEmpDefaults,phones,listSubGroups);
		}

		///<summary>Sets the detail control to the clicked on cubicle as long as there is an associated phone.</summary>
		private void FillDetails(MapCubicle cubeClicked=null) {
			if(cubeClicked is null){

			}
			if(cubeClicked.PhoneCur is null) {
				return;
			}
			_mapCubicleSelected=cubeClicked;
			//set the text before the image, because it was taking too long to load
			userControlMapDetails1.SetEmployee(cubeClicked);
			userControlMapDetails1.Visible=true;
			Application.DoEvents();//process the events first so the text on the control will be set
			FillDetailImage(cubeClicked.PhoneCur.EmployeeNum);
		}

		private void FillDetailImage(long employeeNum){
			//First, look for webcam image
			Bitmap bitmap=GetWebCamImage(employeeNum);
			if(bitmap!=null) {
				userControlMapDetails1.SetBitmap(bitmap,mapImageDisplayStatus:InternalTools.Phones.EnumMapImageDisplayStatus.WebCam,employeeNum);
				return;
			}
			//Then, look for a stock image
			if(userControlMapDetails1.MapImageDisplayStatus==InternalTools.Phones.EnumMapImageDisplayStatus.Stock//already showing stock
				&& userControlMapDetails1.EmployeeNumImage==employeeNum)//for this employee
			{
				return;
			}
			bitmap=GetEmployeePicture(employeeNum);
			if(bitmap!=null){
				userControlMapDetails1.SetBitmap(bitmap,mapImageDisplayStatus:InternalTools.Phones.EnumMapImageDisplayStatus.Stock,employeeNum);
				return;
			}
			userControlMapDetails1.SetBitmap(null,mapImageDisplayStatus:InternalTools.Phones.EnumMapImageDisplayStatus.Empty,employeeNum);
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

		private void tabMain_SelectedIndexChanged(object sender,EventArgs e) {
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			refreshCurrentTabHelper(listPhoneEmpDefaults,Phones.GetPhoneList(),PhoneEmpSubGroups.GetAll());
		}

		private void refreshCurrentTabHelper(List<PhoneEmpDefault> phoneEmpDefaultsIn,List<Phone> phonesIn,List<PhoneEmpSubGroup> subGroupsIn) {
			List<PhoneEmpSubGroup> subGroups=subGroupsIn.FindAll(x => x.SubGroupType==GetCurSubGroupType());
			//List of EmployeeNums to show for current tab.
			List<long> empNums=subGroups.Select(y => y.EmployeeNum).ToList();
			List<PhoneEmpDefault> phoneEmpDefaults=phoneEmpDefaultsIn.FindAll(x => empNums.Contains(x.EmployeeNum));//Employees who belong to this sub group.
			foreach(PhoneEmpDefault phoneEmp in phoneEmpDefaults) {
				phoneEmp.EscalationOrder=subGroups.First(x => x.EmployeeNum==phoneEmp.EmployeeNum).EscalationOrder;
			}
			SetEscalationList(phoneEmpDefaults,phonesIn);
		}

		private void SetEscalationList(List<PhoneEmpDefault> peds,List<Phone> phones) {
			try {
				escalationView.BeginUpdate();
				escalationView.Items.Clear();
				escalationView.DictProximity.Clear();
				escalationView.DictShowExtension.Clear();
				escalationView.DictExtensions.Clear();
				escalationView.DictWebChat.Clear();
				escalationView.DictGTAChat.Clear();
				escalationView.DictRemoteSupport.Clear();
				if(escalationView.Tag==null || (((int)escalationView.Tag)!=tabMain.SelectedIndex)) {
					escalationView.IsNewItems=true;
					escalationView.Tag=tabMain.SelectedIndex;
				}
				List<PhoneEmpDefault> listFiltered=peds.FindAll(x => DoAddToEscalationView(x,phones));
				List<PhoneEmpDefault> listSorted=SortForEscalationView(listFiltered,phones);
				if(_listChatUsers==null) {
					_listChatUsers=ChatUsers.GetAll();
				}
				if(_listWebChatSessions==null) {
					_listWebChatSessions=WebChatSessions.GetActiveSessions();
				}
				if(_listPeerInfosRemoteSupportSessions==null) {
					_listPeerInfosRemoteSupportSessions=PeerInfos.GetActiveSessions(false,true);
				}
				for(int i=0;i<listSorted.Count;i++) {
					PhoneEmpDefault ped=listSorted[i];
					Phone phone=ODMethodsT.Coalesce(Phones.GetPhoneForEmployeeNum(phones,ped.EmployeeNum));
					escalationView.Items.Add(ped.EmpName);
					//Only show the proximity icon if the phone.IsProxVisible AND the employee is at the same site as our currently selected room.
					escalationView.DictProximity.Add(ped.EmpName,(_mapAreaContainer.SiteNum==ped.SiteNum && phone.IsProxVisible));
					PeerInfo remoteSupportSession=_listPeerInfosRemoteSupportSessions.FirstOrDefault(x => x.EmployeeNum==phone.EmployeeNum);
					escalationView.DictWebChat.Add(ped.EmpName,_listWebChatSessions.Any(x => x.TechName==phone.EmployeeName));
					escalationView.DictGTAChat.Add(ped.EmpName,_listChatUsers.Any(x => x.Extension==ped.PhoneExt && x.CurrentSessions>0));
					escalationView.DictRemoteSupport.Add(ped.EmpName,_listPeerInfosRemoteSupportSessions.Any(x => x.EmployeeNum==phone.EmployeeNum));
					//Extensions will always show if they are working from home or for both locations unless the employee is not proximal. 
					if(Employees.GetEmp(ped.EmployeeNum).IsWorkingHome) {
						escalationView.DictShowExtension.Add(ped.EmpName,true);
					} 
					else {
						escalationView.DictShowExtension.Add(ped.EmpName,phone.IsProxVisible);
					}
					escalationView.DictExtensions.Add(ped.EmpName,ped.PhoneExt);
				}
			}
			catch {
			}
			finally {
				escalationView.EndUpdate();
			}
		}

		///<summary>Sorts the list of PhoneEmpDefaults in the appropriate way for the selected escalation view.</summary>
		private List<PhoneEmpDefault> SortForEscalationView(List<PhoneEmpDefault> peds,List<Phone> phones) {
			if(GetCurSubGroupType()==PhoneEmpSubGroupType.Avail) {
				Func<PhoneEmpDefault,Phone> getPhone=new Func<PhoneEmpDefault,Phone>((phoneEmpDef) => {
					return ODMethodsT.Coalesce(Phones.GetPhoneForEmployeeNum(phones,phoneEmpDef.EmployeeNum));
				});
				return peds.OrderBy(x => getPhone(x).ClockStatus!=ClockStatusEnum.Available)//Show Available first
					.ThenBy(x => getPhone(x).ClockStatus!=ClockStatusEnum.Training)//Training next
					.ThenBy(x => getPhone(x).ClockStatus!=ClockStatusEnum.Backup)//Backup next
					.ThenBy(x => getPhone(x).DateTimeStart.Year < 1880)//Show people who have an actual DateTimeStart first
					.ThenBy(x => getPhone(x).DateTimeStart)//Show those first who have been in this status longest
					.ToList();
			}
			//All other escalation views beside Avail
			return peds.OrderBy(x => x.EscalationOrder)//Show people at the selected location first
				.ThenBy(x => x.EmpName).ToList();
		}

		///<summary>Returns true if the employee for the PhoneEmpDefault should be added to the selected escalation view.</summary>
		private bool DoAddToEscalationView(PhoneEmpDefault ped,List<Phone> phones) {
			if(ped.EscalationOrder<=0) { //Filter out employees that do not have an escalation order set.
				return false;
			}
			if(ped.IsTriageOperator) {
				return false;
			}
			Phone phone=Phones.GetPhoneForEmployeeNum(phones,ped.EmployeeNum);
			if(phone==null || phone.Description!="") { //Filter out invalid employees or employees that are already on the phone.
				return false;
			}
			if(GetCurSubGroupType()==PhoneEmpSubGroupType.Avail) {//Special rules for the Avail escalation view
				if(!phone.IsProxVisible && !Employees.GetEmp(ped.EmployeeNum).IsWorkingHome) {
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

		///<summary>Returns true if the employee for the PhoneEmpDefault is at the current location. An employee is considered to be at the same location
		///as a room if the employee's site is the same for the current room.</summary>
		private bool IsAtCurrentLocation(PhoneEmpDefault ped) {
			return ped.SiteNum==_mapAreaContainer.SiteNum;
		}

		public void SetOfficesDownList(List<Task> listOfficesDown) {
			try {
				officesDownView.BeginUpdate();
				officesDownView.Items.Clear();
				//Sort list by oldest.
				listOfficesDown.Sort(delegate(Task t1,Task t2) {
					return Comparer<DateTime>.Default.Compare(t1.DateTimeEntry,t2.DateTimeEntry);
				});
				for(int i=0;i<listOfficesDown.Count;i++) {
					Task task=listOfficesDown[i];
					if(task.TaskStatus==TaskStatusEnum.Done) { //Filter out old tasks. Should not be any but just in case.
						continue;
					}
					TimeSpan timeActive=DateTime.Now.Subtract(task.DateTimeEntry);
					//We got this far so the office is down.
					officesDownView.Items.Add(timeActive.ToStringHmmss()+" - "+task.KeyNum.ToString());
				}
				labelCustDownCount.Text=listOfficesDown.Count.ToString();
				if(listOfficesDown.Count>0) {
					//Get the time of the oldest task
					TimeSpan timeActive=DateTime.Now.Subtract(listOfficesDown[0].DateTimeEntry);
					labelCustDownTime.Text=((int)timeActive.TotalMinutes).ToString();
				}
				else {
					labelCustDownTime.Text="0";
				}
			}
			catch {
			}
			finally {
				officesDownView.EndUpdate();
			}		
		}

		public void SetTriageUrgent(int calls,TimeSpan timeBehind) {
			this.labelTriageRedCalls.Text=calls.ToString();
			if(timeBehind==TimeSpan.Zero) { //format the string special for this case
				this.labelTriageRedTimeSpan.Text="00:00";
			}
			else {
				this.labelTriageRedTimeSpan.Text=timeBehind.ToStringmmss();
			}
			if(calls>=_triageRedCalls) { //we are behind
				labelTriageRedCalls.SetAlertColors();
			}
			else { //we are ok
				labelTriageRedCalls.SetNormalColors();
			}
			if(timeBehind>=TimeSpan.FromMinutes(_triageRedTime)) { //we are behind
				labelTriageRedTimeSpan.SetAlertColors();
			}
			else { //we are ok
				labelTriageRedTimeSpan.SetNormalColors();
			}
		}

		public void SetVoicemailRed(int calls,TimeSpan timeBehind) {
			this.labelVoicemailCalls.Text=calls.ToString();
			if(timeBehind==TimeSpan.Zero) { //format the string special for this case
				this.labelVoicemailTimeSpan.Text="00:00";
			}
			else {
				this.labelVoicemailTimeSpan.Text=timeBehind.ToStringmmss();
			}
			if(calls>=_voicemailCalls) { //we are behind
				labelVoicemailCalls.SetAlertColors();
			}
			else { //we are ok
				labelVoicemailCalls.SetNormalColors();
			}
			if(timeBehind>=TimeSpan.FromMinutes(_voicemailTime)) { //we are behind
				labelVoicemailTimeSpan.SetAlertColors();
			}
			else { //we are ok
				labelVoicemailTimeSpan.SetNormalColors();
			}
		}
		
		///<summary>Sets the time for current triage tasks and colors it according to how far behind we are.</summary>
		public void SetTriageNormal(int callsWithNotes,int callsWithNoNotes,TimeSpan timeBehind,int triageRed) {
			if(timeBehind==TimeSpan.Zero) { //format the string special for this case
				labelTriageTimeSpan.Text="0";
			}
			else {
				labelTriageTimeSpan.Text=((int)timeBehind.TotalMinutes).ToString();			
			}
			if(callsWithNoNotes>0 || triageRed>0) { //we have calls which don't have notes or a red triage task so display that number
				labelTriageCalls.Text=(callsWithNoNotes+triageRed).ToString();
			}
			else { //we don't have any calls with no notes nor any red triage tasks so display count of total tasks
				labelTriageCalls.Text="("+callsWithNotes.ToString()+")";
			}
			if(callsWithNoNotes+triageRed>=_triageCalls) { //we are behind
				labelTriageCalls.SetAlertColors();
			}
			else if(callsWithNoNotes+triageRed>=_triageCallsWarning) { //we are approaching being behind
				labelTriageCalls.SetWarnColors();
			}
			else { //we are ok
				labelTriageCalls.SetNormalColors();
			}
			if(timeBehind>=TimeSpan.FromMinutes(_triageTime)) { //we are behind
				labelTriageTimeSpan.SetAlertColors();
			}
			else if(timeBehind>=TimeSpan.FromMinutes(_triageTimeWarning)) { //we are approaching being behind
				labelTriageTimeSpan.SetWarnColors();
			}
			else { //we are ok
				labelTriageTimeSpan.SetNormalColors();
			}
		}

		public void SetChatCount() {
			if(_listWebChatSessions==null) {
				_listWebChatSessions=WebChatSessions.GetActiveSessions();
			}
			#region set label value
			//get # of WebChats that still need to be claimed
			List<WebChatSession> listUnclaimedSessions=_listWebChatSessions.FindAll(x => string.IsNullOrEmpty(x.TechName));
			DateTime dateTimeOldestWebChat=DateTime.MinValue;
			TimeSpan chatBehind=TimeSpan.Zero;
			labelChatCount.Text=listUnclaimedSessions.Count.ToString();
			if(listUnclaimedSessions.Count>0) {
				dateTimeOldestWebChat=listUnclaimedSessions.Min(x => x.DateTcreated);
				chatBehind=DateTime.Now-dateTimeOldestWebChat;
				labelChatTimeSpan.Text=chatBehind.ToStringmmss();
			}
			else {
				labelChatTimeSpan.Text="00:00";
			}
			#endregion
			#region set label colors
			if(listUnclaimedSessions.Count>=_chatRedCount) {
				labelChatCount.SetAlertColors();
			}
			else {
				labelChatCount.SetNormalColors();
			}
			if(chatBehind>=TimeSpan.FromMinutes(_chatRedTimeMin)) {
				labelChatTimeSpan.SetAlertColors();
			}
			else {
				labelChatTimeSpan.SetNormalColors();
			}
			#endregion
		}

		#endregion Set label text and colors

		private void comboRoom_SelectionChangeCommitted(object sender,EventArgs e) {
			_mapAreaContainer=_listMapAreaContainers[comboRoom.SelectedIndex];
			this.Text="Call Center Status Map - "+_listMapAreaContainers[comboRoom.SelectedIndex].Description;
			FillMapAreaPanel();
			FillTriageLabelColors();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			//Settings-----------------------------------------------------------------------------------------------------------
			_menuItemSettings=new MenuItemOD("Settings");
			menuMain.Add(_menuItemSettings);
			_menuItemSettings.Add("Call Center Thresholds",callCenterThreshToolStripMenuItem_Click);
			_menuItemSettings.Add("Escalation",escalationToolStripMenuItem_Click);
			_menuItemSettings.Add("Map",mapToolStripMenuItem_Click);
			_menuItemSettings.Add("Phones",phonesToolStripMenuItem_Click);
			//Toggle Triage View--------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Toggle Triage View",toggleTriageToolStripMenuItem_Click));
			//Open New Map--------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Open New Map",openNewMapToolStripMenuItem_Click));
			//Conf Rooms--------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Conf Rooms",confRoomsToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		private void mapAreaPanel_Click(object sender,EventArgs e) {

		}

		private void mapToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMapSetup FormMS=new FormMapSetup();
			FormMS.ShowDialog();
			if(FormMS.DialogResult!=DialogResult.OK) {
				return;
			}
			_listMapAreaContainers=FormMS.ListMapAreaContainers;
			_mapAreaContainer=_listMapAreaContainers[comboRoom.SelectedIndex];
			FillCombo();
			FillMapAreaPanel();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"MapHQ layout changed");
		}

		private void callCenterThreshToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMapHQPrefs TriagePref=new FormMapHQPrefs();
			TriagePref.ShowDialog();
			if(TriagePref.DialogResult!=DialogResult.OK) {
				return;
			}
			FillTriagePreferences();
		}

		private void escalationToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormPhoneEmpDefaultEscalationEdit FormE=new FormPhoneEmpDefaultEscalationEdit();
			FormE.ShowDialog();
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			refreshCurrentTabHelper(listPhoneEmpDefaults,Phones.GetPhoneList(),PhoneEmpSubGroups.GetAll());
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Escalation team changed");
		}
		
		///<summary>Starts the timer that will refresh the panel so that ugly lines don't stay on the panel when scrolling.
		///The timer prevents lag due to repainting every time the scroll bar moves.</summary>
		private void mapAreaPanelHQ_Scroll(object sender,EventArgs e) {
			timer1.Stop();
			timer1.Start();
		}

		private void timer1_Tick(object sender,EventArgs e) {
			mapAreaPanel.Refresh();
			timer1.Stop();
		}
		
		private void tabMain_DrawItem(object sender,DrawItemEventArgs e) {
			Graphics g=e.Graphics;
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			Brush brush=new SolidBrush(Color.Black);
			TabPage tabPage=tabMain.TabPages[e.Index];
			Rectangle rectangle=tabMain.GetTabRect(e.Index);
			if(e.State==DrawItemState.Selected) {
				g.FillRectangle(Brushes.White,e.Bounds);
			}
			else {
				g.FillRectangle(Brushes.LightGray,e.Bounds);
			}
			StringFormat stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Near;
			stringFormat.LineAlignment=StringAlignment.Center;
			g.DrawString(tabPage.Text,tabMain.Font,brush,rectangle,stringFormat);
		}

		private void FormMapHQ_ResizeEnd(object sender,EventArgs e) {
			LayoutControls();
		}

		private void LayoutControls(){
			tabMain.ItemSize=new Size(LayoutManager.Scale(28),LayoutManager.Scale(150));
			mapAreaPanel.PixelsPerFoot=LayoutManager.Scale(17);
		}

		private void toggleTriageToolStripMenuItem_Click(object sender,EventArgs e) {
			if(mapAreaPanel.Left==0){
				Point point=LayoutManager.ScalePoint(new Point(365,78));
				mapAreaPanel.Bounds=new Rectangle(point.X,point.Y,ClientSize.Width-point.X,ClientSize.Height-point.Y);
			}
			else{
				//Expand mapAreaPanel to the left, covering up the other controls.
				Point point=LayoutManager.ScalePoint(new Point(0,78));
				mapAreaPanel.Bounds=new Rectangle(point.X,point.Y,ClientSize.Width-point.X,ClientSize.Height-point.Y);
			}
		}

		private void openNewMapToolStripMenuItem_Click(object sender,EventArgs e) {
			//Open the map from FormOpenDental so that it can register for the click events.
			ExtraMapClicked?.Invoke(this,e);
			//Form gets added to the list of all HQMaps in Load method.
		}

		private void FormMapHQ_FormClosed(object sender,FormClosedEventArgs e) {
			_bitmapHouse?.Dispose();//We do not know if the bitmap was disposed of by MapCubicle.Dispose() method, so manually dispose to be safe
			FormOpenDental.RemoveMapFromList(this);
		}

		private void confRoomsToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormPhoneConfs FormPC=new FormPhoneConfs();
			FormPC.ShowDialog();//ShowDialog because we do not want this window to be floating open for long periods of time.
		}

		private void phonesToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormPhoneEmpDefaults formPED=new FormPhoneEmpDefaults();
			formPED.ShowDialog();
		}

		///<summary>If the phone map has been changed, update all phone maps.</summary>
		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(listSignals.Exists(x => x.IType==InvalidType.PhoneMap)) {
				FillMapAreaPanel();
			}
		}
	}
}
