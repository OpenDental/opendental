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
	public partial class FormPhoneTiles : FormODBase {

		#region Fields
		private Bitmap _bitmapHouse=PhoneTile.GetHouse16();
		///<summary>A timer is constantly flipping this back and forth.  Indicates currently pink, regardless of whether any lights really need to be flashing.</summary>
		private bool _isFlashingPink;
		private List<ChatUser> _listChatUsers;
		private List<MapArea> _listMapAreas;
		private List<MapAreaContainer> _listMapAreasContainers;
		private List<Phone> _listPhonesAll;
		///<summary>Filtered and sorted to exactly match grid.</summary>
		private List<Phone> _listPhonesShowing;
		private List<WebChatSession> _listWebChatSessions;
		///<summary>Remote Support Sessions.</summary>
		private List<PeerInfo> _listPeerInfos;
		///<summary>This gives us something to paint on.</summary>
		private UI.PanelOD panelMain;
		private Phone _phoneSelected;
		private Phones.PhoneComparer.SortBy _sortBy=Phones.PhoneComparer.SortBy.name;
		private int _tileHeight=35;
		private int _tileWidth=130;
		/// <summary>This is where the tiles need to start (under the existing controls) in the y axis in order to be drawn correctly.</summary>
		private int _tileStart;
		///<summary>How many phone tiles should show up in each column before creating a new column.</summary>
		private int _tilesPerColumn=27;
		///<summary>This is the difference between server time and local computer time.  Used to ensure that times displayed are accurate to the second.  This value is usally just a few seconds, but possibly a few minutes.</summary>
		private TimeSpan _timeSpanDelta;
		//Fields - Public---------------------------------------------------------------------------------------------------
		///<summary>When the GoToChanged event fires, this tells us which patnum.</summary>
		public long PatNumGoTo;
		#endregion Fields

		#region Constructor
		public FormPhoneTiles() {
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
		}
		#endregion Constructor

		#region Events - Raise
		///<summary></summary>
		[Category("Property Changed"), Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler GoToPatient=null;
		#endregion Events - Raise

		#region Methods - Event Handlers
		private void FormPhoneTiles_Load(object sender,EventArgs e) {
			panelMain=new UI.PanelOD();
			panelMain.Visible=false;
			panelMain.Size=PanelClient.Size;
			panelMain.Anchor=AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			panelMain.MouseClick+=panelMain_MouseClick;
			panelMain.DoubleClick+=panelMain_DoubleClick;
			LayoutManager.Add(panelMain,PanelClient);
			panelMain.Paint += panelMain_Paint;
			_tileStart=butSettings.Bottom+8;
			_isFlashingPink=false;
			_timeSpanDelta=MiscData.GetNowDateTime()-DateTime.Now;
			_listPhonesAll=Phones.GetPhoneList();
			_listChatUsers=ChatUsers.GetAll();
			_listWebChatSessions=WebChatSessions.GetActiveSessions();
			_listPeerInfos=PeerInfos.GetActiveSessions(false,true);
			_listPeerInfos=new List<PeerInfo>();
			_listMapAreas=MapAreas.Refresh();
			_listMapAreasContainers=MapAreaContainers.Refresh();
			FilterAndSortPhoneList();
			panelMain.Invalidate();
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
			panelMain.Invalidate();
			FillGrid();
		}

		private void checkHideOnBreak_CheckedChanged(object sender,EventArgs e) {
			FilterAndSortPhoneList();
			panelMain.Invalidate();
			FillGrid();
		}

		private void checkNeedsHelpTop_Click(object sender,EventArgs e){
			FilterAndSortPhoneList();
			panelMain.Invalidate();
			FillGrid();
		}

		private void checkShowOldInterface_Click(object sender,EventArgs e) {
			if(checkShowOldInterface.Checked){
				panelMain.Visible=true;
				gridMain.Visible=false;
				panelGrid.Visible=false;
				panelGrid2.Visible=false;
				panelMain.Invalidate();
			}
			else{//new interface
				panelMain.Visible=false;
				gridMain.Visible=true;
				panelGrid.Visible=true;
				panelGrid2.Visible=true;
				FillGrid();
			}
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
				return;
			}
			Phone phone=_listPhonesShowing[gridMain.GetSelectedIndex()];
			int idx=listBoxQueues.SelectedIndex;
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

		private void panelMain_DoubleClick(object sender,EventArgs e) {
			int extensionStart=0;
			int extensionFinalX=98;
			int extensionFinalY=17;
			Point location=FindPhoneAndLocation(e,out int x,out int y);
			if(_phoneSelected==null) {
				return;
			}
			if(x<extensionFinalX && x>extensionStart && y<extensionFinalY && y>extensionStart) { //double click with left or right on name
				labelExtensionName_DoubleClicked(_phoneSelected);
			}
		}

		private void panelMain_MouseClick(object sender,MouseEventArgs e) {
			int statusStartX=3;
			int statusFinalX=44;
			int timeStartY=17;
			int startY=20;
			int finalY=34;
			int customerStartX=44;
			int customerFinalX=128;
			Point location=FindPhoneAndLocation(e,out int x,out int y);
			if(_phoneSelected==null) {
				return;
			}
			if(e.Button==MouseButtons.Right) {
				if(x<statusFinalX && x>statusStartX && y<finalY && y>timeStartY) { //right click on time 
					ShowMenuStatus(location,_phoneSelected);
				}
				else if(x<customerFinalX && x>customerStartX && y<finalY && y>startY) { //right click on customer
					menuNumbers.Show(location);
				}
				return;
			}
			//left click
			if(x<customerFinalX && x>customerStartX && y<finalY && y>startY) { //left click on customer
				if(_phoneSelected==null) {
					return;
				}
				if(_phoneSelected.PatNum==0) {
					return;
				}
				PatNumGoTo=_phoneSelected.PatNum;
				GoToPatient?.Invoke(this,new EventArgs());
			}
		}

		private void radioSort_CheckedChanged(object sender,EventArgs e) {
			if(sender==null
				|| !(sender is RadioButton)
				|| ((RadioButton)sender).Checked==false) {
				return;
			}
			if(radioByExt.Checked) {
				_sortBy=Phones.PhoneComparer.SortBy.ext;
			}
			else {
				_sortBy=Phones.PhoneComparer.SortBy.name;
			}
			//Get the phone tiles anew. This will force a resort according the preference we just set.
			RefreshList();
		}
		
		private void butSettings_Click(object sender,EventArgs e) {
			using FormPhoneEmpDefaults formPED=new FormPhoneEmpDefaults();
			formPED.ShowDialog();
		}

		private void timerFlash_Tick(object sender,EventArgs e) {
			//every 300 ms
			_isFlashingPink=!_isFlashingPink;//toggles flash
			panelMain.Invalidate();
			FillGrid();
		}
		#endregion Methods - Event Handlers

		#region Method - Paint
		
		private void panelMain_Paint(object sender, PaintEventArgs e){
			if(!checkShowOldInterface.Checked){
				return;
			}
			base.OnPaint(e);
			Graphics g=e.Graphics;
			Pen pen=new Pen(Color.Black);
			SolidBrush solidBrush=new SolidBrush(Color.Black);
			Font fontDraw=new Font("Microsoft Sans Serif",8.25f);
			Font fontBold=new Font("Microsoft Sans Serif",8.25f,FontStyle.Bold);
			Employee employee;
			int controlHeight=16;
			int controlMargin=3;
			int customerWidth=80;
			int customerY=18;
			int customerX=44;
			int extensionWidth=100;
			int statusWidth=62;
			int statusX=0;
			int statusY=20;
			int imageLocationX=97;//phone or chat icon
			int proxLocationX=116;//either person fig, small circle, or home
			int timeBoxLocationX=3;
			int timeBoxLocationY=17;
			int timeBoxWidth=39;
			int timeLocationX=3;
			int yTop=_tileStart;//the offset of the phone tile taking the other controls of the form into account
			int x=0;//x axis location
			int y=0;//y axis location
			int numColumns=(int)Math.Ceiling((double)_listPhonesAll.Count/_tilesPerColumn);
			int webChatHeightWidth=14;//Used for the rectangle that houses the web chat icon to prevent it from breaking the box boundary.
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			for(int i=0;i<_listPhonesAll.Count;i++) {
				employee=Employees.GetEmp(_listPhonesAll[i].EmployeeNum);
				bool isWorkingHome=false;
				if(employee!=null) {
					isWorkingHome=employee.IsWorkingHome;
				}
				ChatUser chatUser=_listChatUsers.Where(chat => chat.Extension==_listPhonesAll[i].Extension).FirstOrDefault();
				WebChatSession webChatSession=_listWebChatSessions.FirstOrDefault(session => session.TechName==_listPhonesAll[i].EmployeeName);
				PeerInfo remoteSupportSession=_listPeerInfos.FirstOrDefault(x => x.EmployeeNum==_listPhonesAll[i].EmployeeNum);
				Color colorOuter;
				Color colorInner;
				Color colorFont;
				bool isTriageOperatorOnTheClock=false;
				//set the phone color 
				Phones.GetPhoneColor(_listPhonesAll[i],listPhoneEmpDefaults.Find(phone => phone.EmployeeNum==_listPhonesAll[i].EmployeeNum),false,
					out colorOuter,out colorInner,out colorFont,out isTriageOperatorOnTheClock);
				//get the color scheme
				Phones.PhoneColorScheme phoneColorScheme=new Phones.PhoneColorScheme(true);
				string extensionAndName=$"{_listPhonesAll[i].Extension}-{_listPhonesAll[i].EmployeeName}";
				//determine if the extension has a user associated with it
				if(_listPhonesAll[i].EmployeeName=="") {
					extensionAndName+="Vacant";
				}
				//determine the status or note if the user is gone, otherwise just leave it
				string clockStatus="";
				if(_listPhonesAll[i].ClockStatus==ClockStatusEnum.Home
					|| _listPhonesAll[i].ClockStatus==ClockStatusEnum.None
					|| _listPhonesAll[i].ClockStatus==ClockStatusEnum.Off) {
					clockStatus="Clock In";
				}
				if(_listPhonesAll[i].ClockStatus==ClockStatusEnum.Unavailable) {
					clockStatus="Unavbl.";
				}
				//get the customer number
				string customer=_listPhonesAll[i].CustomerNumber;
				//get the time that has passed on a call
				string time="";
				if(_listPhonesAll[i].DateTimeNeedsHelpStart.Date==DateTime.Today) {
					time=(DateTime.Now-_listPhonesAll[i].DateTimeNeedsHelpStart+_timeSpanDelta).ToStringHmmss();
				}
				else if(_listPhonesAll[i].DateTimeStart.Date==DateTime.Today) {
					time=(DateTime.Now-_listPhonesAll[i].DateTimeStart+_timeSpanDelta).ToStringHmmss();
				}
				//draw the time box with its determined color
				Color colorBar=colorOuter;
				//don't draw anything if they are clocked out
				if(_listPhonesAll[i].ClockStatus!=ClockStatusEnum.Home
					&&_listPhonesAll[i].ClockStatus!=ClockStatusEnum.None
					&&_listPhonesAll[i].ClockStatus!=ClockStatusEnum.Off) {
					//determine if they need help and flash pink if they do 
					if(_listPhonesAll[i].ClockStatus==ClockStatusEnum.NeedsHelp) {
						if(_isFlashingPink) {
							colorBar=phoneColorScheme.ColorOuterNeedsHelp;
						}
						else {
							colorBar=Color.Transparent;
						}
					}
					//set the color of the inside of the time box
					if(_listPhonesAll[i].ClockStatus==ClockStatusEnum.HelpOnTheWay) {
						colorBar=phoneColorScheme.ColorOuterNeedsHelp;
					}
					//draw the inside of the time box if the user is not on break
					if(_listPhonesAll[i].ClockStatus!=ClockStatusEnum.Break && _listPhonesAll[i].ClockStatus!=ClockStatusEnum.Unavailable) {
						using(SolidBrush brush=new SolidBrush(colorBar)) {
							g.FillRectangle(brush,(x*_tileWidth)+timeBoxLocationX,(y*_tileHeight)+yTop+timeBoxLocationY,timeBoxWidth,controlHeight);
						}
					}
					//draw the outline of the time box
					if(_listPhonesAll[i].ClockStatus!=ClockStatusEnum.Unavailable) {
						g.DrawRectangle(pen,(x*_tileWidth)+timeBoxLocationX,(y*_tileHeight)+yTop+timeBoxLocationY,timeBoxWidth,controlHeight);
					}
					//draw either the figure or circle depending on if they are proxmial 
					if(_listPhonesAll[i].IsProxVisible && !isWorkingHome && _listPhonesAll[i].ClockStatus!=ClockStatusEnum.Unavailable) {
						g.DrawImage(Properties.Resources.Figure,(x*_tileWidth)+proxLocationX,(y*_tileHeight)+yTop+controlMargin,12,14);
					}
					else if(_listPhonesAll[i].DateTProximal.AddHours(8)>DateTime.Now && !isWorkingHome && _listPhonesAll[i].ClockStatus!=ClockStatusEnum.Unavailable) {
						g.DrawImage(Properties.Resources.NoFigure,(x*_tileWidth)+proxLocationX,(y*_tileHeight)+yTop+controlMargin);
					}
					//draw the phone image if it is in use
					if(_listPhonesAll[i].Description!="") {
						g.DrawImage(Properties.Resources.phoneInUse,(x*_tileWidth)+imageLocationX,(y*_tileHeight)+yTop+controlMargin+1,17,13);
					}
					else if(webChatSession!=null) {
						g.DrawImage(Properties.Resources.WebChatIcon,
							new Rectangle((x*_tileWidth)+imageLocationX,(y*_tileHeight)+yTop+controlMargin+1,webChatHeightWidth,webChatHeightWidth));
						time=(DateTime.Now-webChatSession.DateTcreated).ToStringHmmss();
					}
					else if(chatUser!=null && chatUser.CurrentSessions!=0) {
						g.DrawImage(Properties.Resources.gtaicon3,(x*_tileWidth)+imageLocationX,(y*_tileHeight)+yTop+controlMargin+1,14,14);
						time=TimeSpan.FromMilliseconds(chatUser.SessionTime).ToStringHmmss();
					}
					else if(remoteSupportSession!=null) {
						g.DrawImage(Properties.Resources.remoteSupportIcon,(x*_tileWidth)+imageLocationX,(y*_tileHeight)+yTop+controlMargin+1,14,14);
						time=remoteSupportSession.SessionTime.ToStringHmmss();
					}
					//draw the time on call or what ever activity they are doing
					if(time!="" && _listPhonesAll[i].ClockStatus!=ClockStatusEnum.Unavailable) {
						g.DrawString(time,fontDraw,solidBrush,(x*_tileWidth)+timeLocationX,(y*_tileHeight)+yTop+timeBoxLocationY+2);
					}
				}
				if(isWorkingHome) {//draw the home icon regardless if they are clocked in or out
					try {
						g.DrawImage(_bitmapHouse,
							new Rectangle((x*_tileWidth)+proxLocationX-2,(y*_tileHeight)+yTop+controlMargin,16,16));
					}
					catch {
						//Do nothing. It will be redrawn again after 300ms.
					}
				}
				//draw the things that need to be shown at all times such as the employee name, customer, and emp status
				g.DrawString(extensionAndName,fontBold,solidBrush,new RectangleF((x*_tileWidth),(y*_tileHeight)+yTop+controlMargin,
					extensionWidth,controlHeight),new StringFormat() { FormatFlags=StringFormatFlags.NoWrap });
				if(clockStatus!="") { //the status only shows if it is Clock In or Unavailable
					g.DrawString(clockStatus,fontDraw,solidBrush,new RectangleF((x*_tileWidth)+statusX,(y*_tileHeight)+yTop+statusY,
						statusWidth,controlHeight),new StringFormat() { Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap });
				}
				g.DrawString(customer,fontDraw,solidBrush,new RectangleF((x*_tileWidth)+customerX,(y*_tileHeight)+yTop+customerY+controlMargin,
					customerWidth,controlHeight),new StringFormat() { Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.NoWrap });
				//draw the grid lines
				if(i==1) { //this line needs to be drawn differently than the rest
					g.DrawLine(pen,0,yTop,numColumns*_tileWidth,yTop);
				}
				else {
					g.DrawLine(pen,0,(_tileHeight*y)+yTop,numColumns*_tileWidth,(_tileHeight*y)+yTop);
				}
				y++;
				if((i+1)%_tilesPerColumn==0 && i!=_listPhonesAll.Count) {
					x++;
					//draw the right grid line of the entire column
					g.DrawLine(pen,_tileWidth*x,yTop,_tileWidth*x,(_tileHeight*_tilesPerColumn)+yTop);
					y=0;
				}
			}//for listPhones
			g.DrawLine(pen,0,(_tileHeight*_tilesPerColumn)+yTop,numColumns*_tileWidth,(_tileHeight*_tilesPerColumn)+yTop); //final bottom line
			g.DrawLine(pen,_tileWidth*(x+1),yTop,_tileWidth*(x+1),(_tileHeight*_tilesPerColumn)+yTop); //final right line
			//dispose of the objects we no longer need
			pen.Dispose();
			solidBrush.Dispose();
			fontDraw.Dispose();
			fontBold.Dispose();
		}
		#endregion Method - OnPaint

		#region Methods - Event Handlers, Menu
		private void menuItemManage_Click(object sender,EventArgs e) {
			PhoneUI.Manage(_phoneSelected);
		}

		private void menuItemAdd_Click(object sender,EventArgs e) {
			PhoneUI.Add(_phoneSelected);
		}

		//Timecards-------------------------------------------------------------------------------------

		private void menuItemAvailable_Click(object sender,EventArgs e) {
			PhoneUI.Available(_phoneSelected);
			RefreshList();
		}

		private void menuItemTraining_Click(object sender,EventArgs e) {
			PhoneUI.Training(_phoneSelected);
			RefreshList();
		}

		private void menuItemTeamAssist_Click(object sender,EventArgs e) {
			PhoneUI.TeamAssist(_phoneSelected);
			RefreshList();
		}

		private void menuItemNeedsHelp_Click(object sender,EventArgs e) {
			PhoneUI.NeedsHelp(_phoneSelected);
			RefreshList();
		}

		private void menuItemWrapUp_Click(object sender,EventArgs e) {
			PhoneUI.WrapUp(_phoneSelected);
			RefreshList();
		}

		private void menuItemOfflineAssist_Click(object sender,EventArgs e) {
			PhoneUI.OfflineAssist(_phoneSelected);
			RefreshList();
		}

		private void menuItemUnavailable_Click(object sender,EventArgs e) {
			PhoneUI.Unavailable(_phoneSelected);
			RefreshList();
		}

		private void menuItemBackup_Click(object sender,EventArgs e) {
			PhoneUI.Backup(_phoneSelected);
			RefreshList();
		}

		private void menuItemTCResponder_Click(object sender,EventArgs e) {
			PhoneUI.TCResponder(_phoneSelected);
			RefreshList();
		}

		private void menuItemEmployeeSettings_Click(object sender,EventArgs e) {
			PhoneUI.ShowEmployeeSettings(_phoneSelected);
			RefreshList();
		}

		//RingGroups---------------------------------------------------

		private void menuItemQueueTech_Click(object sender,EventArgs e) {
			PhoneUI.QueueTech(_phoneSelected);
		}

		private void menuItemQueueNone_Click(object sender,EventArgs e) {
			PhoneUI.QueueNone(_phoneSelected);
		}

		private void menuItemQueueDefault_Click(object sender,EventArgs e) {
			PhoneUI.QueueDefault(_phoneSelected);
		}

		private void menuItemQueueBackup_Click(object sender,EventArgs e) {
			PhoneUI.QueueBackup(_phoneSelected);
		}

		//Timecard---------------------------------------------------

		private void menuItemLunch_Click(object sender,EventArgs e) {
			PhoneUI.Lunch(_phoneSelected);
			RefreshList();
		}

		private void menuItemHome_Click(object sender,EventArgs e) {
			PhoneUI.Home(_phoneSelected);
			RefreshList();
		}

		private void menuItemBreak_Click(object sender,EventArgs e) {
			PhoneUI.Break(_phoneSelected);
			RefreshList();
		}
		#endregion Methods - Event Handlers, Menu

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
			panelMain.Invalidate();
			FillGrid();
		}

		public void SetVoicemailCount(int voiceMailCount) {
			if(voiceMailCount==0) {
				labelMsg.Font=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Regular);
				labelMsg.ForeColor=Color.Black;
			}
			else {
				labelMsg.Font=new Font(FontFamily.GenericSansSerif,10f,FontStyle.Bold);
				labelMsg.ForeColor=Color.Firebrick;
			}
			labelMsg.Text="Voice Mails: "+voiceMailCount.ToString();
		}
		#endregion Methods - Public

		#region Methods - Private
		private void labelExtensionName_DoubleClicked(Phone phoneCur) {
			if(phoneCur==null || phoneCur.EmployeeNum < 1) {
				return;
			}
			PhoneEmpDefault phoneEmpDefault=PhoneEmpDefaults.GetOne(phoneCur.EmployeeNum);
			if(phoneEmpDefault==null) {
				MessageBox.Show("No 'phoneempdefault' row found for EmployeeNum "+phoneCur.EmployeeNum
					+".\r\nGo to Phone Settings window and add a row for this employee.");
				return;
			}
			using FormPhoneEmpDefaultEdit formPhoneEmpDefaultEdit=new FormPhoneEmpDefaultEdit();
			formPhoneEmpDefaultEdit.PedCur=phoneEmpDefault;
			formPhoneEmpDefaultEdit.ShowDialog();
		}

		private void FillGrid(){
			if(checkShowOldInterface.Checked){
				return;
			}
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
					clockStatus="";
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
				Employee employee=Employees.GetEmp(_listPhonesShowing[i].EmployeeNum);
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
				else if(_listPhonesShowing[i].IsProxVisible) {
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
					&& x.Description!="" 
					&& _listMapAreasContainers.Exists(y=>y.MapAreaContainerNum==x.MapAreaContainerNum));
				if(mapArea==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(mapArea.Description);
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
			_listPhonesShowing=_listPhonesShowing.FindAll(x=>x.EmployeeName.ToLower().Contains(textSearch.Text.ToLower()));
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

		private Point FindPhoneAndLocation(EventArgs e,out int x, out int y) {
			MouseEventArgs mouseEventArgs=(MouseEventArgs)e;
			Point location=mouseEventArgs.Location; //location of click
			int column=(int)Math.Floor(location.X/(double)_tileWidth); //determine how many columns over the click was
			int row=(int)Math.Floor((location.Y-butSettings.Bottom-8)/(double)_tileHeight); //determine what row the click was in
			_phoneSelected=GetPhoneClicked(location,column,row); //phone tile that was clicked
			//subtract the rest of the columns and rows width to center the location back to (0,0). This will make it 
			//easy to determine which "control" is being clicked on in the drawn phonetile
			//x and y are coordinates within the individual cell
			x=location.X-(column*_tileWidth); 
			y=(location.Y-butSettings.Bottom-8)-(row*_tileHeight);
			//when we call Menu.Show later, it is expecting the absolute point on the screen, not relative to the current form.
			return this.PointToScreen(location);
		}

		///<summary>Could return null</summary>
		private Phone GetPhoneClicked(Point location,int column,int row) {
			int index=(column)*_tilesPerColumn+row;
			Phone phoneClicked=null;
			if(index<_listPhonesAll.Count 
				//Make sure the click location is not above or below the grid.
				&& location.Y>groupBox1.Bottom+1 && location.Y<((_tilesPerColumn*_tileHeight)+_tileStart)) 
			{
				phoneClicked=_listPhonesAll[index];
			}
			return phoneClicked;
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
			panelMain.Invalidate();
			FillGrid();
		}

		private void ShowMenuStatus(Point location,Phone phoneCur) {
			if(phoneCur==null) {
				return;
			}
			PhoneUI.BuildMenuStatus(menuStatus,phoneCur);
			menuStatus.Show(location);		
		}







		#endregion Methods - Private

		
	}
}
