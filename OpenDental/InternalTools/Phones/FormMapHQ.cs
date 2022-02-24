using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Drawing;
using System.Linq;
using System.Text;
using CodeBase;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormMapHQ:FormODBase {
		#region Events
		public event EventHandler RoomControlClicked;
		public event EventHandler ExtraMapClicked;
		[Category("Property Changed"),Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler GoToChanged=null;
		#endregion

		#region Private Members
		//<summary>Keep track of full screen state</summary>
		//private bool _isFullScreen;
		///<summary>This is the difference between server time and local computer time.  Used to ensure that times displayed are accurate to the second.  This value is usally just a few seconds, but possibly a few minutes.</summary>
		private TimeSpan _timeDelta;
		private List<MapAreaContainer> _listMaps;
		private MapAreaContainer _mapCur;
		///<summary>The site that is associated to the first three octets of the computer that has launched this map.</summary>
		private Site _siteCur;
		//preferences for setting triage alert colors
		private int _triageRedCalls,_triageRedTime,_voicemailCalls,_voicemailTime,_triageCalls,_triageTime,_triageTimeWarning,_triageCallsWarning;
		///<summary>Tracks when chat boxes for map need to be set red.</summary>
		private int _chatRedCount=2;
		///<summary>Tracks when chat boxes for map need to be set red.</summary>
		private int _chatRedTimeMin=1;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<WebChatSession> _listWebChatSessions;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<PeerInfo> _listRemoteSupportSessions;
		///<summary>can be null. Will be set and re-set whenever SetPhoneList is called/refreshed.</summary>
		private List<ChatUser> _listChatUsers;
		///<summary>Used for the main menu and updates when the user is or is not in full screen mode.</summary>
		private MenuItemOD _menuItemSettings;
		private MenuItemOD _menuItemFullScreen;

		private PhoneEmpSubGroupType _curSubGroupType {
			get {
					if(!tabMain.TabPages.ContainsKey(PhoneEmpSubGroupType.Escal.ToString())) {//Control has not been initialized.
						return PhoneEmpSubGroupType.Escal;
					}
					return (PhoneEmpSubGroupType)(tabMain.SelectedTab.Tag??PhoneEmpSubGroupType.Escal);
			}
		}
		#endregion Private Members

		#region Initialize

		public FormMapHQ() {
			InitializeComponent();
			//This form needs to dynamically add, remove, and dispose controls which remove is not currently supported when using custom borders.
			InitializeLayoutManager(isLayoutMS:true);
			//Do not do anything to do with database or control init here. We will be using this ctor later in order to create a temporary object so we can figure out what size the form should be when the user comes back from full screen mode. Wait until FormMapHQ_Load to do anything meaningful.
			//_isFullScreen=false;
			_timeDelta=MiscData.GetNowDateTime()-DateTime.Now;
			//Add the mousewheel event to allow mousewheel scrolling to repaint the grid as well.
			mapAreaPanelHQ.MouseWheel+=new MouseEventHandler(mapAreaPanelHQ_Scroll);
			mapAreaPanelHQ.RoomControlClicked+=MapAreaPanelHQ_RoomControlClicked;
			mapAreaPanelHQ.GoToChanged+=new System.EventHandler(this.MapAreaRoomControl_GoToChanged);
		}

		public void MapAreaPanelHQ_RoomControlClicked(object sender,EventArgs e) {
			MapAreaRoomControl clickedPhone=(MapAreaRoomControl)sender;
			if(clickedPhone==null) {
				return;
			}
			FillDetails(clickedPhone);
			RoomControlClicked?.Invoke(sender,e);
		}

		private void FormMapHQ_Load(object sender,EventArgs e) {
			LayoutControls();
			_siteCur=SiteLinks.GetSiteByGateway();
			if(_siteCur==null) {
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
			_listMaps=PhoneMapJSON.GetFromDb();
			//Add a custom order to this map list which will prefer maps that are associated to the local computer's site.
			_listMaps=_listMaps.OrderBy(x => x.SiteNum!=_siteCur.SiteNum)
				.ThenBy(x => x.Description).ToList();
			//Select the first map in our list that matches the site associated to the current computer.
			_mapCur=_listMaps[0];
		}

		private void FillTriageLabelColors() {
			labelTriageOpsCountLocal.SetTriageColors(_mapCur.SiteNum);
			labelTriageOpsCountTotal.SetTriageColors();
		}

		private void FillTabs() {
			TabPage tabPage;
			foreach(TabPage tab in tabMain.TabPages) {
				LayoutManager.Remove(tab);
				//tabMain.TabPages.Remove(tab);
				tab.Dispose();
			}
			tabPage=new TabPage(PhoneEmpSubGroupType.Avail.ToString());
			tabPage.Name=PhoneEmpSubGroupType.Avail.ToString();
			tabPage.Tag=PhoneEmpSubGroupType.Avail;
			LayoutManager.Add(tabPage,tabMain);
			//tabMain.TabPages.Add(tabPage);
			List<PhoneEmpSubGroupType> eVals=Enum.GetValues(typeof(PhoneEmpSubGroupType)).Cast<PhoneEmpSubGroupType>().ToList();
			foreach(PhoneEmpSubGroupType e in eVals) {
				if(e==PhoneEmpSubGroupType.Avail) {
					continue;//Already added above
				}
				tabPage=new TabPage(e.ToString());
				tabPage.Name=e.ToString();
				tabPage.Tag=e;
				LayoutManager.Add(tabPage,tabMain);
				//tabMain.TabPages.Add(tabPage);
			}
		}

		private void FillCombo() {
			comboRoom.Items.Clear();
			foreach(MapAreaContainer mapCur in _listMaps) {
				comboRoom.Items.Add(mapCur.Description);
			}
			int selectedIndex=0;
			if(_mapCur!=null) {
				_listMaps.FindIndex(x => x.MapAreaContainerNum==_mapCur.MapAreaContainerNum);
			}
			comboRoom.SelectedIndex=(selectedIndex==-1 ? 0 : selectedIndex);
		}

		///<summary>Setup the map panel with the cubicles and labels before filling with real-time data. Call this on load or anytime the cubicle layout has changed.</summary>
		private void FillMapAreaPanel() {
			mapAreaPanelHQ.PixelsPerFoot=LayoutManager.Scale(17);
			mapAreaPanelHQ.Clear(false);
			mapAreaPanelHQ.FloorHeightFeet=Math.Max(_mapCur.FloorHeightFeet,55);//Should at least fill the space set in the designer.
			mapAreaPanelHQ.FloorWidthFeet=Math.Max(_mapCur.FloorWidthFeet,89);//Should at least fill the space set in the designer.
																			  //fill the panel
			List<MapArea> clinicMapItems=MapAreas.Refresh(_listMaps[comboRoom.SelectedIndex].MapAreaContainerNum);
			clinicMapItems=clinicMapItems.OrderByDescending(x => (int)(x.ItemType)).ToList();
			for(int i=0;i<clinicMapItems.Count;i++) {
				if(clinicMapItems[i].MapAreaContainerNum!=_listMaps[comboRoom.SelectedIndex].MapAreaContainerNum) {
					continue;
				}
				if(clinicMapItems[i].ItemType==MapItemType.Room) {
					mapAreaPanelHQ.AddCubicle(clinicMapItems[i]);
				}
				else if(clinicMapItems[i].ItemType==MapItemType.DisplayLabel) {
					mapAreaPanelHQ.AddDisplayLabel(clinicMapItems[i]);
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

		///<summary>Refresh the phone panel every X seconds after it has already been setup.  Make sure to call FillMapAreaPanel before calling 
		///this the first time.</summary>
		public void SetPhoneList(List<Phone> phones,List<PhoneEmpSubGroup> listSubGroups,List<ChatUser> listChatUsers,
			List<WebChatSession> listWebChatSessions,List<PeerInfo> listRemoteSupportSessions)
		{
			//refresh our lists to minimize trips to the database.
			_listWebChatSessions=listWebChatSessions;
			_listRemoteSupportSessions=listRemoteSupportSessions;
			_listChatUsers=listChatUsers;
			try {
				string title="Call Center Map - Triage Coord. - ";
				try { //get the triage coord label but don't fail just because we can't find it
					SiteLink siteLink=SiteLinks.GetFirstOrDefault(x => x.SiteNum==_mapCur.SiteNum);
					title+=Employees.GetNameFL(Employees.GetEmp(siteLink.EmployeeNum));
				}
				catch {
					title+="Not Set";
				}
				labelTriageCoordinator.Text=title;
				labelCurrentTime.Text=DateTime.Now.ToShortTimeString();
				Bitmap bitmapHouse=PhoneTile.GetHouse16();
				#region Triage Counts
				//The triage count used to only count up the triage operators within the currently selected room.
				//Now we want to count all operators at the selected site (local) and then all operators across all sites (total).
				int triageStaffCountLocal=0;
				int triageStaffCountTotal=0;
				List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
				List<PhoneEmpDefault> listPhoneEmpDefaultsTriage=listPhoneEmpDefaults.FindAll(x => x.IsTriageOperator && x.HasColor);
				for(int i=0;i<listPhoneEmpDefaultsTriage.Count;i++){
				//foreach(PhoneEmpDefault phoneEmpDefault in peds.FindAll(x => x.IsTriageOperator && x.HasColor)) {
					Phone phone=phones.FirstOrDefault(x => x.Extension==listPhoneEmpDefaultsTriage[i].PhoneExt);
					if(phone==null) {
						continue;
					}
					if(ListTools.In(phone.ClockStatus,ClockStatusEnum.None,ClockStatusEnum.Home,ClockStatusEnum.Lunch,ClockStatusEnum.Break,ClockStatusEnum.Off
						,ClockStatusEnum.Unavailable,ClockStatusEnum.NeedsHelp,ClockStatusEnum.HelpOnTheWay))
					{
						continue;
					}
					//This is a triage operator who is currently here and on the clock.
					if(listPhoneEmpDefaultsTriage[i].SiteNum==_mapCur.SiteNum) {
						triageStaffCountLocal++;
					}
					triageStaffCountTotal++;
				}
				labelTriageOpsCountLocal.Text=triageStaffCountLocal.ToString();
				labelTriageOpsCountTotal.Text=triageStaffCountTotal.ToString();
				#endregion
				for(int i=0;i<this.mapAreaPanelHQ.Controls.Count;i++) { //loop through all of our cubicles and labels and find the matches
					try {
						if(!(this.mapAreaPanelHQ.Controls[i] is MapAreaRoomControl)) {
							continue;
						}
						MapAreaRoomControl mapAreaRoomControl=(MapAreaRoomControl)this.mapAreaPanelHQ.Controls[i];
						if(mapAreaRoomControl.MapAreaItem.Extension==0) { //This cubicle has not been given an extension yet.
							mapAreaRoomControl.Empty=true;
							continue;
						}
						Phone phone=Phones.GetPhoneForExtension(phones,mapAreaRoomControl.MapAreaItem.Extension);
						if(phone==null) {//We have a cubicle with no corresponding phone entry.
							mapAreaRoomControl.Empty=true;
							continue;
						}
						mapAreaRoomControl.PhoneCur=phone;//Refresh PhoneCur so that it has up to date customer information.
						ChatUser chatuser=listChatUsers.Where(x => x.Extension == phone.Extension).FirstOrDefault();
						PhoneEmpDefault phoneEmpDefault=PhoneEmpDefaults.GetEmpDefaultFromList(phone.EmployeeNum,listPhoneEmpDefaults);
						if(phoneEmpDefault==null) {//We have a cubicle with no corresponding phone emp default entry.
							mapAreaRoomControl.Empty=true;
							continue;
						}
						//we got this far so we found a corresponding cubicle for this phone entry
						mapAreaRoomControl.EmployeeNum=phone.EmployeeNum;
						mapAreaRoomControl.EmployeeName=phone.EmployeeName;
						WebChatSession webChatSession=listWebChatSessions.FirstOrDefault(x => x.TechName==phone.EmployeeName);
						PeerInfo remoteSupportSession=listRemoteSupportSessions.FirstOrDefault(x => x.EmployeeNum==phone.EmployeeNum);
						if(phone.DateTimeNeedsHelpStart.Date==DateTime.Today) { //if they need help, use that time.
							TimeSpan span=DateTime.Now-phone.DateTimeNeedsHelpStart+_timeDelta;
							DateTime timeOfDay=DateTime.Today+span;
							mapAreaRoomControl.Elapsed=span;
						}
						else if(phone.DateTimeStart.Date==DateTime.Today && phone.Description != "") { //else if in a call, use call time.
							TimeSpan span=DateTime.Now-phone.DateTimeStart+_timeDelta;
							DateTime timeOfDay=DateTime.Today+span;
							mapAreaRoomControl.Elapsed=span;
						}
						else if(phone.Description=="" && webChatSession!=null ) {//else if in a web chat session, use web chat session time
							TimeSpan span=DateTime.Now-webChatSession.DateTcreated+_timeDelta;
							mapAreaRoomControl.Elapsed=span;	
						}
						else if(phone.Description=="" && chatuser!=null && chatuser.CurrentSessions>0) { //else if in a chat, use chat time.
						  TimeSpan span=TimeSpan.FromMilliseconds(chatuser.SessionTime)+_timeDelta;
						  mapAreaRoomControl.Elapsed=span;
						}
						else if(phone.Description=="" && remoteSupportSession!=null) {
							//Might need to enhance later to get a 'timeDelta' for the Remote Support server.
							mapAreaRoomControl.Elapsed=remoteSupportSession.SessionTime;
						}
						else if(phone.DateTimeStart.Date==DateTime.Today) { //else available, use that time.
						  TimeSpan span = DateTime.Now-phone.DateTimeStart+_timeDelta;
						  DateTime timeOfDay = DateTime.Today+span;
						  mapAreaRoomControl.Elapsed=span;
						}
						else { //else, whatever.
							mapAreaRoomControl.Elapsed=TimeSpan.Zero;
						}
						if(phone.IsProxVisible) {
							mapAreaRoomControl.ProxImage=Properties.Resources.Figure;
						}
						else if(phone.DateTProximal.AddHours(8)>DateTime.Now) {
							mapAreaRoomControl.ProxImage=Properties.Resources.NoFigure;//TODO: replace image with one from Nathan
						}
						else {
							mapAreaRoomControl.ProxImage=null;
						}
						mapAreaRoomControl.IsAtDesk=phone.IsProxVisible;
						string status=Phones.ConvertClockStatusToString(phone.ClockStatus);
						//Check if the user is logged in.
						if(phone.ClockStatus==ClockStatusEnum.None
							||phone.ClockStatus==ClockStatusEnum.Home) {
							status="Home";
						}
						mapAreaRoomControl.Status=status;
						if(phone.Description=="") {
							mapAreaRoomControl.PhoneImage=null;
							mapAreaRoomControl.WebChatImage=null;
							mapAreaRoomControl.ChatImage=null;
							mapAreaRoomControl.RemoteSupportImage=null;
							if(webChatSession!=null) {//active web chat session
								mapAreaRoomControl.WebChatImage=Properties.Resources.WebChatIcon;
							}
							else if(chatuser!=null && chatuser.CurrentSessions!=0) {//check for GTA sessions if no web chats
								mapAreaRoomControl.ChatImage=Properties.Resources.WebChatIcon;
							}
							else if(remoteSupportSession!=null) {
								mapAreaRoomControl.RemoteSupportImage=Properties.Resources.remoteSupportIcon;
							}
						}
						else {
							mapAreaRoomControl.PhoneImage=Properties.Resources.phoneInUse;
						}
						Color outerColor;
						Color innerColor;
						Color fontColor;
						bool isTriageOperatorOnTheClock;
						//get the cubicle color and triage status
						Phones.GetPhoneColor(phone,phoneEmpDefault,true,out outerColor,out innerColor,out fontColor,out isTriageOperatorOnTheClock);
						if(!mapAreaRoomControl.IsFlashing) { //if the control is already flashing then don't overwrite the colors. this would cause a "spastic" flash effect.
							mapAreaRoomControl.OuterColor=outerColor;
							mapAreaRoomControl.InnerColor=innerColor;
						}
						mapAreaRoomControl.ForeColor=fontColor;
						if(phone.ClockStatus==ClockStatusEnum.NeedsHelp) { //turn on flashing
							mapAreaRoomControl.StartFlashing();
						}
						else { //turn off flashing
							mapAreaRoomControl.StopFlashing();
						}
						if(phone.EmployeeNum>0 && phone.EmployeeNum==userControlMapDetails1.EmployeeNumCur) {
							userControlMapDetails1.UpdateControl(mapAreaRoomControl);
						}
						Employee employee=Employees.GetEmp(phone.EmployeeNum);//from cache
						if(employee!=null){
							if(employee.IsWorkingHome) {
								mapAreaRoomControl.ProxImage=bitmapHouse;
							}
							//plan to use this to show extra employee fields, like cellphone
						}
						mapAreaRoomControl.Invalidate(true);
					}
					catch(Exception e) {
						e.DoNothing();
					}
				}
				refreshCurrentTabHelper(listPhoneEmpDefaults,phones,listSubGroups);
			}
			catch {
				//something failed unexpectedly
			}
		}

		///<summary>Sets the detail control to the clicked on cubicle as long as there is an associated phone.</summary>
		private void FillDetails(MapAreaRoomControl cubeClicked=null) {
			if(cubeClicked.PhoneCur!=null) {
				//set the text before the image, because it was taking too long to load
				userControlMapDetails1.SetEmployee(cubeClicked,null);
				userControlMapDetails1.Visible=true;
				Application.DoEvents();//process the events first so the text on the control will be set
				Image empImage=GetEmployeePicture(cubeClicked.PhoneCur);
				if(empImage!=null) {
					userControlMapDetails1.SetEmployee(cubeClicked,empImage);
				}
			}
		}

		///<summary>Attempts to get Employee photos from the server with a hardcoded filepath. Returns null if the desired picture could not be found or accessed.</summary>
		private Image GetEmployeePicture(Phone phoneEmployee) {
			Employee emp=Employees.GetEmp(phoneEmployee.EmployeeNum);
			if(emp==null) {
				return null;
			}
			try {
				//Only grab the first part of the FName if there are multiple uppercased parts (ie. StevenS should be Steven).
				string fname=Regex.Split(emp.FName,@"(?<!^)(?=[A-Z])")[0];
				string employeeName=fname+" "+emp.LName;
				List<string> files=Directory.GetFiles(@"\\opendental.od\SERVERFILES\Storage\OPEN DENTAL\Staff\Staff Photos").ToList().FindAll(x => x.ToLower().EndsWith(employeeName.ToLower()+".jpg"));
				foreach(string fileSource in files) {
					using(Bitmap original=(Bitmap)System.Drawing.Image.FromFile(fileSource)) {
						Bitmap resized=new Bitmap(original,new System.Drawing.Size(original.Width/8,original.Height/8));
							return resized;
					}
				}
			}
			catch(Exception ex) {
				//Don't really care if it fails so swallow everything.
				ex.DoNothing();
			}
			return null;
		}

		private void tabMain_SelectedIndexChanged(object sender,EventArgs e) {
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			refreshCurrentTabHelper(listPhoneEmpDefaults,Phones.GetPhoneList(),PhoneEmpSubGroups.GetAll());
		}

		private void refreshCurrentTabHelper(List<PhoneEmpDefault> phoneEmpDefaultsIn,List<Phone> phonesIn,List<PhoneEmpSubGroup> subGroupsIn) {
			List<PhoneEmpSubGroup> subGroups=subGroupsIn.FindAll(x => x.SubGroupType==_curSubGroupType);
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
				if(_listRemoteSupportSessions==null) {
					_listRemoteSupportSessions=PeerInfos.GetActiveSessions(false,true);
				}
				for(int i=0;i<listSorted.Count;i++) {
					PhoneEmpDefault ped=listSorted[i];
					Phone phone=ODMethodsT.Coalesce(Phones.GetPhoneForEmployeeNum(phones,ped.EmployeeNum));
					escalationView.Items.Add(ped.EmpName);
					//Only show the proximity icon if the phone.IsProxVisible AND the employee is at the same site as our currently selected room.
					escalationView.DictProximity.Add(ped.EmpName,(_mapCur.SiteNum==ped.SiteNum && phone.IsProxVisible));
					PeerInfo remoteSupportSession=_listRemoteSupportSessions.FirstOrDefault(x => x.EmployeeNum==phone.EmployeeNum);
					escalationView.DictWebChat.Add(ped.EmpName,_listWebChatSessions.Any(x => x.TechName==phone.EmployeeName));
					escalationView.DictGTAChat.Add(ped.EmpName,_listChatUsers.Any(x => x.Extension==ped.PhoneExt && x.CurrentSessions>0));
					escalationView.DictRemoteSupport.Add(ped.EmpName,_listRemoteSupportSessions.Any(x => x.EmployeeNum==phone.EmployeeNum));
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
			if(_curSubGroupType==PhoneEmpSubGroupType.Avail) {
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
			if(_curSubGroupType==PhoneEmpSubGroupType.Avail) {//Special rules for the Avail escalation view
				if(!phone.IsProxVisible && !Employees.GetEmp(ped.EmployeeNum).IsWorkingHome) {
					return false;
				}
				if(!ListTools.In(phone.ClockStatus,ClockStatusEnum.Available,ClockStatusEnum.Training,ClockStatusEnum.Backup)) {
					return false;
				}
				return true;
			}
			//All other escalation views besides Avail
			if(!ListTools.In(phone.ClockStatus,ClockStatusEnum.Available,ClockStatusEnum.OfflineAssist,ClockStatusEnum.Backup))	{
				return false;
			}
			return true;
		}

		///<summary>Returns true if the employee for the PhoneEmpDefault is at the current location. An employee is considered to be at the same location
		///as a room if the employee's site is the same for the current room.</summary>
		private bool IsAtCurrentLocation(PhoneEmpDefault ped) {
			return ped.SiteNum==_mapCur.SiteNum;
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
			_mapCur=_listMaps[comboRoom.SelectedIndex];
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

		private void mapToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormMapSetup FormMS=new FormMapSetup();
			FormMS.ShowDialog();
			if(FormMS.DialogResult!=DialogResult.OK) {
				return;
			}
			_listMaps=FormMS.ListMaps;
			_mapCur=_listMaps[comboRoom.SelectedIndex];
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
			mapAreaPanelHQ.Refresh();
			timer1.Stop();
		}
		
		private void tabMain_DrawItem(object sender,DrawItemEventArgs e) {
			Graphics g=e.Graphics;
			Brush _textBrush=new SolidBrush(Color.Black);
			TabPage _tabPage=tabMain.TabPages[e.Index];
			Rectangle _tabBounds=tabMain.GetTabRect(e.Index);
			if(e.State==DrawItemState.Selected) {
				g.FillRectangle(Brushes.White,e.Bounds);
			}
			else {
				g.FillRectangle(Brushes.LightGray,e.Bounds);
			}
			// Draw string. Center the text.
			StringFormat _stringFlags=new StringFormat();
			_stringFlags.Alignment=StringAlignment.Near;
			_stringFlags.LineAlignment=StringAlignment.Near;
			g.DrawString(_tabPage.Text,tabMain.Font,_textBrush,_tabBounds,new StringFormat(_stringFlags));
		}

		private void FormMapHQ_ResizeEnd(object sender,EventArgs e) {
			LayoutControls();
		}

		private void LayoutControls(){
			splitContainer1.SplitterDistance=LayoutManager.Scale(84);
			splitContainer2.SplitterDistance=LayoutManager.Scale(363);
			tabMain.ItemSize=new Size(LayoutManager.Scale(28),LayoutManager.Scale(150));
			mapAreaPanelHQ.PixelsPerFoot=LayoutManager.Scale(17);
		}

		private void toggleTriageToolStripMenuItem_Click(object sender,EventArgs e) {
			splitContainer2.Panel1Collapsed=!splitContainer2.Panel1Collapsed;
		}

		private void openNewMapToolStripMenuItem_Click(object sender,EventArgs e) {
			//Open the map from FormOpenDental so that it can register for the click events.
			ExtraMapClicked?.Invoke(this,e);
			//Form gets added to the list of all HQMaps in Load method.
		}

		private void FormMapHQ_FormClosed(object sender,FormClosedEventArgs e) {
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

		private void MapAreaRoomControl_GoToChanged(object sender,EventArgs e) {
			GoToChanged?.Invoke(sender,new EventArgs());
		}

		///<summary>If the phone map has been changed, update all phone maps.</summary>
		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(listSignals.Exists(x => x.IType==InvalidType.PhoneMap)) {
				FillMapAreaPanel();
			}
		}
	}
}
