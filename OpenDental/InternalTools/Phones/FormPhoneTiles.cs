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
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPhoneTiles : FormODBase {

		#region Fields
		private Bitmap _bitmapHouse=PhoneTile.GetHouse16();
		///<summary>A timer is constantly flipping this back and forth.  Indicates currently pink, regardless of whether any lights really need to be flashing.</summary>
		private bool _isFlashingPink;
		private List<ChatUser> _listChatUsers;
		///<summary>This thread fills labelMsg</summary>
		private List<Phone> _listPhones;
		private List<WebChatSession> _listWebChatSessions;
		private List<PeerInfo> _listRemoteSupportSessions;
		//private int _msgCount;
		///<summary>This gives us something to paint on.</summary>
		private UI.PanelOD panelMain;
		private Phone _phoneSelected;
		private Phones.PhoneComparer.SortBy _sortBy=Phones.PhoneComparer.SortBy.name;
		private int _tileHeight=35;
		private int _tileWidth=142;
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
		protected void OnGoToPatient() {
			GoToPatient?.Invoke(this,new EventArgs());
		}
		[Category("Property Changed"), Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler GoToPatient=null;
		#endregion Events - Raise

		#region Methods - Event Handlers Standard
		private void FormPhoneTiles_Load(object sender,EventArgs e) {
			panelMain=new UI.PanelOD();
			panelMain.Size=PanelClient.Size;
			panelMain.Anchor=AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			panelMain.MouseClick+=panelMain_MouseClick;
			panelMain.DoubleClick+=panelMain_DoubleClick;
			LayoutManager.Add(panelMain,PanelClient);
			panelMain.Paint += panelMain_Paint;
			if(!ODBuild.IsDebug() && Environment.MachineName.ToLower()!="jordans"
				&& Environment.MachineName.ToLower()!="nathan") 
			{
				checkBoxAll.Visible=false;//so this will also be visible in debug
			}
			_tileStart=butSettings.Bottom+8;
			_isFlashingPink=false;
			_timeSpanDelta=MiscData.GetNowDateTime()-DateTime.Now;
			//Do not call FillTiles() yet. Need to create PhoneTile controls first.
			SetPhoneList(Phones.GetPhoneList(),ChatUsers.GetAll(),WebChatSessions.GetActiveSessions(),PeerInfos.GetActiveSessions(false,true),
				isFillTiles:false);
			if(_listPhones.Count>=1) {
				int columns=(int)Math.Ceiling((double)_listPhones.Count/_tilesPerColumn);
				int widthForm=1780;//This width and height is changed to accomadate Jordan's taskbar and sidebar setup.
				int heightForm=1026;
				this.Size=new Size(widthForm,heightForm);
			}
			radioByExt.CheckedChanged+=radioSort_CheckedChanged;
			radioByName.CheckedChanged+=radioSort_CheckedChanged;
		}

		private void FormPhoneTiles_Shown(object sender,EventArgs e) {      
			DateTime now=DateTime.Now;
			while(now.AddSeconds(1)>DateTime.Now) {
				Application.DoEvents();
			}
		}

		private void checkHideClockedOut_CheckedChanged(object sender,EventArgs e) {
			FilterPhoneList();
			FillTiles(false);
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
			FillTiles(true);
		}

		private void phoneTile_GoToChanged(Phone phoneCur) {
			if(phoneCur==null) {
				return;
			}
			if(phoneCur.PatNum==0) {
				return;
			}
			PatNumGoTo=phoneCur.PatNum;
			OnGoToPatient();
		}

		private void timerFlash_Tick(object sender,EventArgs e) {
			_isFlashingPink=!_isFlashingPink;//toggles flash
			panelMain.Invalidate();
		}
		#endregion Methods - Event Handlers Standard

		#region Method - OnPaint
		
		private void panelMain_Paint(object sender, PaintEventArgs e){
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
			int customerX=60;
			int customerY=18;
			int extensionWidth=100;
			int statusWidth=62;
			int statusX=0;
			int statusY=20;
			int imageLocationX=99;
			int proxLocationX=126;
			int timeBoxLocationX=3;
			int timeBoxLocationY=17;
			int timeBoxWidth=56;
			int timeLocationX=11;
			int yTop=_tileStart;//the offset of the phone tile taking the other controls of the form into account
			int x=0;//x axis location
			int y=0;//y axis location
			int numColumns=(int)Math.Ceiling((double)_listPhones.Count/_tilesPerColumn);
			int webChatHeightWidth=17;//Used for the rectangle that houses the web chat icon to prevent it from breaking the box boundary.
			//i starts at 1, because it determines the number of columns with modulo division on line if(i%_tilesPerColumn==0)
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			for(int i=1;i<_listPhones.Count+1;i++) {
				int numCur=i-1;
				employee=Employees.GetEmp(_listPhones[numCur].EmployeeNum);
				bool isWorkingHome=false;
				if(employee!=null) {
					isWorkingHome=employee.IsWorkingHome;
				}
				ChatUser chatUser=_listChatUsers.Where(chat => chat.Extension==_listPhones[numCur].Extension).FirstOrDefault();
				WebChatSession webChatSession=_listWebChatSessions.FirstOrDefault(session => session.TechName==_listPhones[numCur].EmployeeName);
				PeerInfo remoteSupportSession=_listRemoteSupportSessions.FirstOrDefault(x => x.EmployeeNum==_listPhones[numCur].EmployeeNum);
				Color outerColor;
				Color innerColor;
				Color fontColor;
				bool isTriageOperatorOnTheClock=false;
				//set the phone color 
				Phones.GetPhoneColor(_listPhones[numCur],listPhoneEmpDefaults.Find(phone => phone.EmployeeNum==_listPhones[numCur].EmployeeNum),false,
					out outerColor,out innerColor,out fontColor,out isTriageOperatorOnTheClock);
				//get the color scheme
				Phones.PhoneColorScheme phoneColorScheme=new Phones.PhoneColorScheme(true);
				string extensionAndName=$"{_listPhones[numCur].Extension}-{_listPhones[numCur].EmployeeName}";
				//determine if the extension has a user associated with it
				if(_listPhones[numCur].EmployeeName=="") {
					extensionAndName+="Vacant";
				}
				//determine the status or note if the user is gone, otherwise just leave it
				string statusAndNote="";
				if(_listPhones[numCur].ClockStatus==ClockStatusEnum.Home
					|| _listPhones[numCur].ClockStatus==ClockStatusEnum.None
					|| _listPhones[numCur].ClockStatus==ClockStatusEnum.Off) {
					statusAndNote="Clock In";
				}
				if(_listPhones[numCur].ClockStatus==ClockStatusEnum.Unavailable) {
					statusAndNote="Unavailable";
				}
				//get the customer number
				string customer=_listPhones[numCur].CustomerNumber;
				//get the time that has passed on a call
				string time="";
				if(_listPhones[numCur].DateTimeNeedsHelpStart.Date==DateTime.Today) {
					time=(DateTime.Now-_listPhones[numCur].DateTimeNeedsHelpStart+_timeSpanDelta).ToStringHmmss();
				}
				else if(_listPhones[numCur].DateTimeStart.Date==DateTime.Today) {
					time=(DateTime.Now-_listPhones[numCur].DateTimeStart+_timeSpanDelta).ToStringHmmss();
				}
				//draw the time box with its determined color
				Color colorBar=outerColor;
				//don't draw anything if they are clocked out or unavailable
				if(_listPhones[numCur].ClockStatus!=ClockStatusEnum.Home
					&&_listPhones[numCur].ClockStatus!=ClockStatusEnum.None
					&&_listPhones[numCur].ClockStatus!=ClockStatusEnum.Off) {
					//determine if they need help and flash pink if they do 
					if(_listPhones[numCur].ClockStatus==ClockStatusEnum.NeedsHelp) {
						if(_isFlashingPink) {
							colorBar=phoneColorScheme.ColorOuterNeedsHelp;
						}
						else {
							colorBar=Color.Transparent;
						}
					}
					//set the color of the inside of the time box
					if(_listPhones[numCur].ClockStatus==ClockStatusEnum.HelpOnTheWay) {
						colorBar=phoneColorScheme.ColorOuterNeedsHelp;
					}
					//draw the inside of the time box if the user is not on break
					if(_listPhones[numCur].ClockStatus!=ClockStatusEnum.Break && _listPhones[numCur].ClockStatus!=ClockStatusEnum.Unavailable) {
						using(SolidBrush brush=new SolidBrush(colorBar)) {
							g.FillRectangle(brush,(x*_tileWidth)+timeBoxLocationX,(y*_tileHeight)+yTop+timeBoxLocationY,timeBoxWidth,controlHeight);
						}
					}
					//draw the outline of the time box
					if(_listPhones[numCur].ClockStatus!=ClockStatusEnum.Unavailable) {
						g.DrawRectangle(pen,(x*_tileWidth)+timeBoxLocationX,(y*_tileHeight)+yTop+timeBoxLocationY,timeBoxWidth,controlHeight);
					}
					//draw either the figure or circle depending on if they are proxmial 
					if(_listPhones[numCur].IsProxVisible && !isWorkingHome && _listPhones[numCur].ClockStatus!=ClockStatusEnum.Unavailable) {
						g.DrawImage(Properties.Resources.Figure,(x*_tileWidth)+proxLocationX,(y*_tileHeight)+yTop+controlMargin);
					}
					else if(_listPhones[numCur].DateTProximal.AddHours(8)>DateTime.Now && !isWorkingHome && _listPhones[numCur].ClockStatus!=ClockStatusEnum.Unavailable) {
						g.DrawImage(Properties.Resources.NoFigure,(x*_tileWidth)+proxLocationX,(y*_tileHeight)+yTop+controlMargin);
					}
					//draw the phone image if it is in use
					if(_listPhones[numCur].Description!="") {
						g.DrawImage(Properties.Resources.phoneInUse,(x*_tileWidth)+imageLocationX,(y*_tileHeight)+yTop+controlMargin);
					}
					else if(webChatSession!=null) {
						g.DrawImage(Properties.Resources.WebChatIcon,
							new Rectangle((x*_tileWidth)+imageLocationX,(y*_tileHeight)+yTop+controlMargin,webChatHeightWidth,webChatHeightWidth));
						time=(DateTime.Now-webChatSession.DateTcreated).ToStringHmmss();
					}
					else if(chatUser!=null && chatUser.CurrentSessions!=0) {
						g.DrawImage(Properties.Resources.gtaicon3,(x*_tileWidth)+imageLocationX,(y*_tileHeight)+yTop+controlMargin);
						time=TimeSpan.FromMilliseconds(chatUser.SessionTime).ToStringHmmss();
					}
					else if(remoteSupportSession!=null) {
						g.DrawImage(Properties.Resources.remoteSupportIcon,(x*_tileWidth)+imageLocationX,(y*_tileHeight)+yTop+controlMargin);
						time=remoteSupportSession.SessionTime.ToStringHmmss();
					}
					//draw the time on call or what ever activity they are doing
					if(time!="" && _listPhones[numCur].ClockStatus!=ClockStatusEnum.Unavailable) {
						g.DrawString(time,fontDraw,solidBrush,(x*_tileWidth)+timeLocationX,(y*_tileHeight)+yTop+timeBoxLocationY+2);
					}
				}
				if(isWorkingHome) {//draw the home icon regardless if they are clocked in or out
					try {
						g.DrawImage(_bitmapHouse,
							new Rectangle((x*_tileWidth)+proxLocationX-2,(y*_tileHeight)+yTop+controlMargin+2,16,16));
					}
					catch {
						//Do nothing. It will be redrawn again after 300ms.
					}
				}
				//draw the things that need to be shown at all times such as the employee name, customer, and emp status
				g.DrawString(extensionAndName,fontBold,solidBrush,new RectangleF((x*_tileWidth),(y*_tileHeight)+yTop+controlMargin,
					extensionWidth,controlHeight),new StringFormat() { FormatFlags=StringFormatFlags.NoWrap });
				if(statusAndNote!="") { //the status only shows if it is Clock In or Unavailable
					g.DrawString(statusAndNote,fontDraw,solidBrush,new RectangleF((x*_tileWidth)+statusX,(y*_tileHeight)+yTop+statusY,
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
				if(i%_tilesPerColumn==0 && i!=_listPhones.Count) {
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

		#region Methods - Event Handlers Click
		private void butSettings_Click(object sender,EventArgs e) {
			using FormPhoneEmpDefaults formPED=new FormPhoneEmpDefaults();
			formPED.ShowDialog();
		}

		private void butConfRooms_Click(object sender,EventArgs e) {
			using FormPhoneConfs FormPC=new FormPhoneConfs();
			FormPC.ShowDialog();//ShowDialog because we do not this window to be floating open for long periods of time.
		}

		private void checkBoxAll_Click(object sender,EventArgs e) {
			Phones.ClearImages();
			FillTiles(false);
		}

		private void menuItemManage_Click(object sender,EventArgs e) {
			PhoneUI.Manage(_phoneSelected);
		}

		private void menuItemAdd_Click(object sender,EventArgs e) {
			PhoneUI.Add(_phoneSelected);
		}

		//Timecards-------------------------------------------------------------------------------------

		private void menuItemAvailable_Click(object sender,EventArgs e) {
			PhoneUI.Available(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemTraining_Click(object sender,EventArgs e) {
			PhoneUI.Training(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemTeamAssist_Click(object sender,EventArgs e) {
			PhoneUI.TeamAssist(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemNeedsHelp_Click(object sender,EventArgs e) {
			PhoneUI.NeedsHelp(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemWrapUp_Click(object sender,EventArgs e) {
			PhoneUI.WrapUp(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemOfflineAssist_Click(object sender,EventArgs e) {
			PhoneUI.OfflineAssist(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemUnavailable_Click(object sender,EventArgs e) {
			PhoneUI.Unavailable(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemBackup_Click(object sender,EventArgs e) {
			PhoneUI.Backup(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemTCResponder_Click(object sender,EventArgs e) {
			PhoneUI.TCResponder(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemEmployeeSettings_Click(object sender,EventArgs e) {
			PhoneUI.EmployeeSettings(_phoneSelected);
			FillTiles(true);
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
			FillTiles(true);
		}

		private void menuItemHome_Click(object sender,EventArgs e) {
			PhoneUI.Home(_phoneSelected);
			FillTiles(true);
		}

		private void menuItemBreak_Click(object sender,EventArgs e) {
			PhoneUI.Break(_phoneSelected);
			FillTiles(true);
		}

		private Point FindPhoneAndLocation(EventArgs e,out int x, out int y) {
			MouseEventArgs mouseEventArgs=(MouseEventArgs)e;
			Point location=mouseEventArgs.Location; //location of click
			int column=(int)Math.Floor(location.X/(double)_tileWidth); //determine how many columns over the click was
			int row=(int)Math.Floor((location.Y-butSettings.Bottom-8)/(double)_tileHeight); //determine what row the click was in
			_phoneSelected=GetPhone(location,column,row); //phone tile that was clicked
			//subtract the rest of the columns and rows width to center the location back to (0,0). This will make it 
			//easy to determine which "control" is being clicked on in the drawn phonetile
			//x and y are coordinates within the individual cell
			x=location.X-(column*_tileWidth); 
			y=(location.Y-butSettings.Bottom-8)-(row*_tileHeight);
			//when we call Menu.Show later, it is expecting the absolute point on the screen, not relative to the current form.
			return this.PointToScreen(location);
		}

		private void panelMain_MouseClick(object sender,MouseEventArgs e) {
			int statusStartX=3;
			int statusFinalX=59;
			int timeStartY=17;
			int startY=20;
			int finalY=34;
			int customerStartX=60;
			int customerFinalX=140;
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
			}
			else {
				if(x<customerFinalX && x>customerStartX && y<finalY && y>startY) { //left click on customer
					phoneTile_GoToChanged(_phoneSelected);
				}
			}
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
		#endregion Methods - Event Handlers Click

		#region Methods - Public 
		public void SetPhoneList(List<Phone> listPhones,List<ChatUser> listChatUsers,List<WebChatSession> listWebChatSession,
			List<PeerInfo> listRemoteSupportSessions,bool isFillTiles=true)
		{
			//create a new list so our sorting doesn't affect this list elsewhere
			_listPhones=new List<Phone>(listPhones);
			_listChatUsers=listChatUsers;
			_listWebChatSessions=listWebChatSession;
			_listRemoteSupportSessions=listRemoteSupportSessions;
			if(isFillTiles) {
				FilterPhoneList();
				FillTiles(false);//this is dangerous.  If it were true, it would be a loop.
			}
			else {
				_listPhones.Sort(new Phones.PhoneComparer(_sortBy));
			}
			panelMain.Invalidate();
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
			using FormPhoneEmpDefaultEdit FormPEDE=new FormPhoneEmpDefaultEdit();
			FormPEDE.PedCur=phoneEmpDefault;
			FormPEDE.ShowDialog();
		}

		///<summary>Could return null</summary>
		private Phone GetPhone(Point location,int column,int row) {
			int index=(column)*_tilesPerColumn+row;
			Phone phoneClicked=null;
			if(index<_listPhones.Count 
				//Make sure the click location is not above or below the grid.
				&& location.Y>groupBox1.Bottom+1 && location.Y<((_tilesPerColumn*_tileHeight)+_tileStart)) 
			{
				phoneClicked=_listPhones[index];
			}
			return phoneClicked;
		}

		private void FillTiles(bool doRefreshList) {
			if(doRefreshList) { //Refresh the phone list. This will cause a database refresh for our list and call this function again with the new list.
				DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
				SetPhoneList(Phones.GetPhoneList(),ChatUsers.GetAll(),WebChatSessions.GetActiveSessions(),PeerInfos.GetActiveSessions(false,true));
				return;
			}
			panelMain.Invalidate();
		}

		///<summary>Filters phone list. Checks the checkHideClockedOut and will remove phones if employee is not clocked in.</summary>
		private void FilterPhoneList() {
			List<Phone> listFiltered=new List<Phone>();
			foreach(Phone phone in _listPhones) {
				if(checkHideClockedOut.Checked && ListTools.In(phone.ClockStatus,ClockStatusEnum.None,ClockStatusEnum.Home,ClockStatusEnum.Off)) {
					//Show only clocked in employees. Don't add to filtered list.
				}
				else {
					listFiltered.Add(phone);
				}
			}
			_listPhones=listFiltered;
			_listPhones.Sort(new Phones.PhoneComparer(_sortBy));
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
