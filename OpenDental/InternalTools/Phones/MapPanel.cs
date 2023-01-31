using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDentBusiness.Crud;

namespace OpenDental.InternalTools.Phones{
	///<summary>This is the panel where all the cubicles are drawn.  For now, it is contained within a scrollable panel, but that might change once pan and zoom are added.</summary>
	public partial class MapPanel:UserControl{
		#region Fields - public
		public bool IsEditMode;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		public List<MapArea> ListMapAreas=new List<MapArea>();
		///<summary>Always same length as ListMapAreas. 1:1 relationship.</summary>
		public List<MapAreaMore> ListMapAreaMores=new List<MapAreaMore>();
		public bool SnapToFeet=true;
		#endregion Fields - public

		#region Fields - private
		///<summary>18x18. Same image used for webchat and chat.</summary>
		private Bitmap _bitmapChat;
		///<summary>16x16</summary>
		private Bitmap _bitmapHouse;
		///<summary>21x17</summary>
		private Bitmap _bitmapPhone;
		///<summary>15x17</summary>
		private Bitmap _bitmapProxFig;
		///<summary>21x17, small red circle, mostly wasted extra pixels.</summary>
		private Bitmap _bitmapProxAway;
		///<summary>18x18</summary>
		private Bitmap _bitmapRemote;
		///<summary>This prevents accidental drags when clicking.</summary>
		private DateTime _dateTimeMouseDown;
		///<summary>As _timerFlash ticks, it toggles this back and forth. If false, normal colors paint. If true, the opposite colors paint.</summary>
		private bool _isFlashOn;
		private bool _isMouseDown;
		private List<int> _listSelected=new List<int>();
		private MapAreaContainer _mapAreaContainer;
		private Phone _phoneClicked;
		///<summary>Coordinates of the mouse when it was initially clicked, in control coords. Can't use Map coords because of circular logic.</summary>
		private Point _pointMouseDown;
		///<summary>Current coordinates of the mouse in control coords. Can't use Map coords because of circular logic.</summary>
		private Point _pointMouseNow;
		///<summary>In map coords, when user drags.</summary>
		private PointF _pointFTranslation;
		private PointF _pointFTranslationOld;
		private Timer _timerFlash;
		///<summary>This is the difference between server time and local computer time.  Used to ensure that times displayed are accurate to the second.  This value is usally just a few seconds, but possibly a few minutes.</summary>
		private TimeSpan _timeSpanDelta;
		private bool _wasDoubleClick;
		///<summary>The zoom level that lets the room fit perfectly in the canvas. This number is typically in the thousands because each cubicle is only "6" wide.</summary>
		private int _zoomFit;
		///<summary>Example: 1759. This must be a float for better accuracy. This number is typically in the thousands because each cubicle is only "6" wide.</summary>
		private float _zoomValue=1;
		#endregion Fields - private

		#region Constructor
		public MapPanel(){
			InitializeComponent();
			DoubleBuffered=true;
			_bitmapChat=Properties.Resources.WebChatIcon;
			_bitmapHouse=PhoneTile.GetHouse16();
			_bitmapPhone=Properties.Resources.phoneInUse;
			_bitmapProxFig=Properties.Resources.Figure;
			_bitmapProxAway=Properties.Resources.NoFigure;//Red circle
			_bitmapRemote=Properties.Resources.remoteSupportIcon;
			_timerFlash=new Timer();
			_timerFlash.Tick+=_timerFlash_Tick;
			_timerFlash.Interval=300;
			_timerFlash.Enabled=true;//always ticking, even if no cubicles flashing
		}
		#endregion Constructor

		#region Events
		///<summary>The eventHandler contains the patNum.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<long> GoToPatient;

		///<summary>In Edit Mode, this notifies parent that any changes were made so that it can send signal to other computers for refresh.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler IsChanged;

		///<summary>Certain detail gets passed up that we need to show in the detail panel.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<CubicleClickedDetail> CubicleClicked;
		#endregion Events

		#region Methods - public
		///<summary>Full refresh from db. Only used in edit mode.</summary>
		public void RefreshEditMode(MapAreaContainer mapAreaContainer){
			_mapAreaContainer=mapAreaContainer;
			ListMapAreas=MapAreas.Refresh(_mapAreaContainer.MapAreaContainerNum);
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			ListMapAreaMores=new List<MapAreaMore>();
			for(int i=0;i<ListMapAreas.Count;i++){
				MapAreaMore mapAreaMore=new MapAreaMore();
				if(ListMapAreas[i].ItemType==MapItemType.Cubicle){
					mapAreaMore.ColorBorder=ColorOD.Gray(128);//if selected, this gets overridden to red in paint.
					mapAreaMore.ColorBack=ColorOD.Gray(245);
					mapAreaMore.ColorFont=Color.Black;
				}
				else{//label
					mapAreaMore.ColorBack=Color.White;
					mapAreaMore.ColorFont=Color.Black;
				}
				mapAreaMore.RectangleFBounds=new RectangleF((float)ListMapAreas[i].XPos,(float)ListMapAreas[i].YPos,(float)ListMapAreas[i].Width,(float)ListMapAreas[i].Height);
				if(ListMapAreas[i].ItemType!=MapItemType.Cubicle){
					ListMapAreaMores.Add(mapAreaMore);
					continue;//done with labels
				}
				if(ListMapAreas[i].Extension==0) { //This cubicle has not been given an extension yet.
					mapAreaMore.IsEmpty=true;
					mapAreaMore.ColorFont=ColorOD.Gray(192);
					mapAreaMore.ColorBorder=ColorOD.Gray(192);
					mapAreaMore.ColorBack=ColorOD.Gray(245);
					ListMapAreaMores.Add(mapAreaMore);
					continue;
				}
				PhoneEmpDefault phoneEmpDefault=listPhoneEmpDefaults.Find(x=>x.PhoneExt==ListMapAreas[i].Extension);
				if(phoneEmpDefault==null) {//We have a cubicle with no corresponding phone emp default entry.
					mapAreaMore.IsEmpty=true;
					mapAreaMore.ColorFont=ColorOD.Gray(192);
					mapAreaMore.ColorBorder=ColorOD.Gray(192);
					mapAreaMore.ColorBack=ColorOD.Gray(245);
					ListMapAreaMores.Add(mapAreaMore);
					continue;
				}
				//we got this far so we found a corresponding phoneEmpDefault for this cubicle
				mapAreaMore.EmployeeName=phoneEmpDefault.EmpName;
				ListMapAreaMores.Add(mapAreaMore);
			}
			Invalidate();
		}

		///<summary>This gets run once to place the MapAreas on the panel. Then, SetPhoneList is called to change colors, text, etc.</summary>
		public void SetMapAreaContainer(MapAreaContainer mapAreaContainer){
			_mapAreaContainer=mapAreaContainer;
			ListMapAreas=MapAreas.Refresh(_mapAreaContainer.MapAreaContainerNum);
			ListMapAreaMores=new List<MapAreaMore>();
			for(int i=0;i<ListMapAreas.Count;i++){
				MapAreaMore mapAreaMore=new MapAreaMore();
				if(ListMapAreas[i].ItemType==MapItemType.Cubicle){
					mapAreaMore.ColorBack=ColorOD.Gray(245);
					mapAreaMore.ColorBorder=Color.Black;//ColorOD.Gray(100);
					mapAreaMore.ColorFont=Color.Black;
				}
				else{//label
					mapAreaMore.ColorBack=Color.White;
					mapAreaMore.ColorFont=Color.Black;
				}
				mapAreaMore.RectangleFBounds=new RectangleF((float)ListMapAreas[i].XPos,(float)ListMapAreas[i].YPos,(float)ListMapAreas[i].Width,(float)ListMapAreas[i].Height);
				ListMapAreaMores.Add(mapAreaMore);
			}
		}

		///<summary>Refresh the phone panel every X seconds after it has already been setup.  Make sure to call SetListMapAreas before calling this the first time. Normal as opposed to EditMode.</summary>
		public void SetPhoneList(List<Phone> listPhones,List<ChatUser> listChatUsers,List<WebChatSession> listWebChatSessions, List<PeerInfo> listPeerInfosRemoteSupportSessions)
		{
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			for(int i=0;i<ListMapAreas.Count;i++){
				if(ListMapAreas[i].ItemType!=MapItemType.Cubicle){
					continue;//we're only updating cubicles, not labels
				}
				if(ListMapAreas[i].Extension==0) { //This cubicle has not been given an extension yet.
					ListMapAreaMores[i].IsEmpty=true;
					ListMapAreaMores[i].ColorFont=ColorOD.Gray(192);
					ListMapAreaMores[i].ColorBorder=ColorOD.Gray(192);
					ListMapAreaMores[i].ColorBack=ColorOD.Gray(245);
					continue;
				}
				Phone phone=listPhones.Find(x => x.Extension==ListMapAreas[i].Extension);
				if(phone==null) {//We have a cubicle with no corresponding phone entry.
					ListMapAreaMores[i].IsEmpty=true;
					ListMapAreaMores[i].ColorFont=ColorOD.Gray(192);
					ListMapAreaMores[i].ColorBorder=ColorOD.Gray(192);
					ListMapAreaMores[i].ColorBack=ColorOD.Gray(245);
					continue;
				}
				ChatUser chatuser=listChatUsers.Find(x => x.Extension==phone.Extension);
				PhoneEmpDefault phoneEmpDefault=PhoneEmpDefaults.GetEmpDefaultFromList(phone.EmployeeNum,listPhoneEmpDefaults);
				if(phoneEmpDefault==null) {//We have a cubicle with no corresponding phone emp default entry.
					ListMapAreaMores[i].IsEmpty=true;
					ListMapAreaMores[i].ColorFont=ColorOD.Gray(192);
					ListMapAreaMores[i].ColorBorder=ColorOD.Gray(192);
					ListMapAreaMores[i].ColorBack=ColorOD.Gray(245);
					continue;
				}
				//we got this far so we found a corresponding cubicle for this phone entry
				ListMapAreaMores[i].PhoneCur=phone;
				ListMapAreaMores[i].EmployeeNum=phone.EmployeeNum;
				ListMapAreaMores[i].EmployeeName=phone.EmployeeName;
				ListMapAreaMores[i].CustomerNumber=phone.CustomerNumber;
				WebChatSession webChatSession=listWebChatSessions.FirstOrDefault(x => x.TechName==phone.EmployeeName);
				PeerInfo peerInfoRemoteSupportSession=listPeerInfosRemoteSupportSessions.FirstOrDefault(x => x.EmployeeNum==phone.EmployeeNum);
				if(phone.DateTimeNeedsHelpStart.Date==DateTime.Today) { //if they need help, use that time.
					TimeSpan timeSpan=DateTime.Now-phone.DateTimeNeedsHelpStart+_timeSpanDelta;
					ListMapAreaMores[i].TimeSpanElapsed=timeSpan;
				}
				else if(phone.DateTimeStart.Date==DateTime.Today && phone.Description != "") { //else if in a call, use call time.
					TimeSpan timeSpan=DateTime.Now-phone.DateTimeStart+_timeSpanDelta;
					ListMapAreaMores[i].TimeSpanElapsed=timeSpan;
				}
				else if(phone.Description=="" && webChatSession!=null ) {//else if in a web chat session, use web chat session time
					TimeSpan timeSpan=DateTime.Now-webChatSession.DateTcreated+_timeSpanDelta;
					ListMapAreaMores[i].TimeSpanElapsed=timeSpan;	
				}
				else if(phone.Description=="" && chatuser!=null && chatuser.CurrentSessions>0) { //else if in a chat, use chat time.
					TimeSpan timeSpan=TimeSpan.FromMilliseconds(chatuser.SessionTime)+_timeSpanDelta;
					ListMapAreaMores[i].TimeSpanElapsed=timeSpan;
				}
				else if(phone.Description=="" && peerInfoRemoteSupportSession!=null) {
					//Might need to enhance later to get a 'timeDelta' for the Remote Support server.
					ListMapAreaMores[i].TimeSpanElapsed=peerInfoRemoteSupportSession.SessionTime;
				}
				else if(phone.DateTimeStart.Date==DateTime.Today) { //else available, use that time.
					TimeSpan timeSpan = DateTime.Now-phone.DateTimeStart+_timeSpanDelta;
					ListMapAreaMores[i].TimeSpanElapsed=timeSpan;
				}
				else { //else, whatever.
					ListMapAreaMores[i].TimeSpanElapsed=TimeSpan.Zero;
				}
				ListMapAreaMores[i].IsHome=false;
				ListMapAreaMores[i].IsProx=false;
				ListMapAreaMores[i].IsProxAway=false;
				Employee employee=Employees.GetEmp(phone.EmployeeNum);//from cache
				if(employee?.IsWorkingHome??false) {
					ListMapAreaMores[i].IsHome=true;
				}
				else if(phone.IsProxVisible()) {
					ListMapAreaMores[i].IsProx=true;
				}
				else if(phone.DateTProximal.AddHours(8)>DateTime.Now) {
					ListMapAreaMores[i].IsProxAway=true;
				}
				string status=OpenDentBusiness.Phones.ConvertClockStatusToString(phone.ClockStatus);
				if(phone.ClockStatus==ClockStatusEnum.None) {
					status="Home";
				}
				ListMapAreaMores[i].Status=status;
				ListMapAreaMores[i].ClockStatus=phone.ClockStatus;
				ListMapAreaMores[i].PatNumCall=phone.PatNum;
				ListMapAreaMores[i].IsPhone=false;
				ListMapAreaMores[i].IsChat=false;
				ListMapAreaMores[i].IsRemoteSupport=false;
				if(phone.Description!="") {//"In use"
					ListMapAreaMores[i].IsPhone=true;
				}
				else {//phone not in use
					if(webChatSession!=null) {//active web chat session
						ListMapAreaMores[i].IsChat=true;
					}
					else if(chatuser!=null && chatuser.CurrentSessions!=0) {//check for GTA sessions if no web chats
						ListMapAreaMores[i].IsChat=true;
					}
					else if(peerInfoRemoteSupportSession!=null) {
						ListMapAreaMores[i].IsRemoteSupport=true;
					}
				}
				Color colorBorder;
				Color colorBack;
				Color colorFont;
				bool isTriageOperatorOnTheClock;
				//get the cubicle color and triage status
				OpenDentBusiness.Phones.GetPhoneColor(phone,phoneEmpDefault,isForDualColorScheme:true,out colorBorder,out colorBack,out colorFont,out isTriageOperatorOnTheClock);
				ListMapAreaMores[i].ColorBorder=colorBorder;
				ListMapAreaMores[i].ColorBack=colorBack;
				ListMapAreaMores[i].ColorFont=colorFont;
				if(phone.ClockStatus==ClockStatusEnum.NeedsHelp) { //turn on flashing
					ListMapAreaMores[i].IsFlashing=true;
				}
				else { //turn off flashing
					ListMapAreaMores[i].IsFlashing=false;
				}
			}
			Invalidate();
		}

		public void SetZoomInitialFit(Size sizeCanvas,Size sizeRoom){
			_zoomFit=(int)(UI.ImageTools.CalcScaleFit(sizeCanvas,sizeRoom,0)*100);
			_zoomValue=_zoomFit;
			//unlike the zoomSlider, we don't care about a maximum
			_pointFTranslation=new PointF();
			Invalidate();
		}
		#endregion Methods - public

		#region Method - private OnPaint
		protected override void OnPaint(PaintEventArgs pe){
			//base.OnPaint(pe);
			Graphics g=pe.Graphics;
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			g.SmoothingMode=SmoothingMode.HighQuality;
			//This was an attempt to make the little man not have a flattop. Didn't work. I think just not enough pixels.
			g.CompositingQuality=CompositingQuality.HighQuality;
			g.Clear(Color.White);
			//if(DesignMode){
			g.DrawRectangle(Pens.Black,0,0,Width-1,Height-1);
			//}
			//Center screen:
			g.TranslateTransform(Width/2f,Height/2f);
			//because of order, scaling is center of panel instead of center of image.
			float scaleFactor=_zoomValue/100f;//example 1759/100=17.59
			g.ScaleTransform(scaleFactor,scaleFactor);
			//and the user translation must be in image coords rather than panel coords
			g.TranslateTransform(_pointFTranslation.X,_pointFTranslation.Y);
			//Back to the UL of this room
			if(_mapAreaContainer is null){
				return;
			}
			g.TranslateTransform(-_mapAreaContainer.FloorWidthFeet/2f,-_mapAreaContainer.FloorHeightFeet/2f);
			//outline of container because it will always be either too wide or too tall to fit perfectly
			using Pen penOutline=new Pen(Color.Black,1f/scaleFactor);
			g.DrawRectangle(penOutline,0,0,_mapAreaContainer.FloorWidthFeet-1f/scaleFactor,_mapAreaContainer.FloorHeightFeet-1f/scaleFactor);
			//first, cubicles
			for(int i=0;i<ListMapAreas.Count;i++){
				if(ListMapAreas[i].ItemType!=MapItemType.Cubicle){
					continue;
				}
				if(IsEditMode && _listSelected.Contains(i) && _isMouseDown){
					//dragging, so draw it further down.
					continue;
				}
				if(ListMapAreas[i].Width<6) {
					this.DrawCubicleSmall(g,i);
				}
				else {
					DrawCubicleLarge(g,i);
				}
			}
			//then, labels so they're on top
			for(int i=0;i<ListMapAreas.Count;i++){
				if(ListMapAreas[i].ItemType!=MapItemType.Label){
					continue;
				}
				if(IsEditMode && _listSelected.Contains(i) && _isMouseDown){
					//dragging, so draw it further down.
					continue;
				}
				DrawLabel(g,i);
			}
			//then, anything that we're dragging, so that it's on top
			if(IsEditMode && _isMouseDown){
				for(int i=0;i<ListMapAreas.Count;i++){
					if(!_listSelected.Contains(i)){
						continue;
					}
					GraphicsState graphicsState = g.Save();
					//pointMouseNow is in control coords, so it's too big. Needs to be smaller.
					float scaleTrans=(float)100/_zoomValue;//example, 2000%, 100/2000=.05, indicating .05 map units for each screen pixel
					g.TranslateTransform((_pointMouseNow.X-_pointMouseDown.X)*scaleTrans,(_pointMouseNow.Y-_pointMouseDown.Y)*scaleTrans);
					if(ListMapAreas[i].ItemType==MapItemType.Label){
						DrawLabel(g,i);
					}
					else if(ListMapAreas[i].Width<6) {
						this.DrawCubicleSmall(g,i);
					}
					else {
						DrawCubicleLarge(g,i);
					}
					g.Restore(graphicsState);
				}
			}
		}
		#endregion Method - private OnPaint

		#region Methods - Menu
		private void menuItemAvailable_Click(object sender,EventArgs e) {
			PhoneUI.Available(_phoneClicked);
		}

		private void menuItemTraining_Click(object sender,EventArgs e) {
			PhoneUI.Training(_phoneClicked);
		}

		private void menuItemTeamAssist_Click(object sender,EventArgs e) {
			PhoneUI.TeamAssist(_phoneClicked);
		}

		private void menuItemTCResponder_Click(object sender,EventArgs e) {
			PhoneUI.TCResponder(_phoneClicked);
		}

		private void menuItemNeedsHelp_Click(object sender,EventArgs e) {
			PhoneUI.NeedsHelp(_phoneClicked);
		}

		private void menuItemWrapUp_Click(object sender,EventArgs e) {
			PhoneUI.WrapUp(_phoneClicked);
		}

		private void menuItemOfflineAssist_Click(object sender,EventArgs e) {
			PhoneUI.OfflineAssist(_phoneClicked);
		}

		private void menuItemUnavailable_Click(object sender,EventArgs e) {
			PhoneUI.Unavailable(_phoneClicked);
		}

		private void menuItemBackup_Click(object sender,EventArgs e) {
			PhoneUI.Backup(_phoneClicked);
		}

		private void menuItemRinggroupTech_Click(object sender,EventArgs e) {
			PhoneUI.QueueTech(_phoneClicked);
		}

		private void menuItemRinggroupNone_Click(object sender,EventArgs e) {
			PhoneUI.QueueNone(_phoneClicked);
		}

		private void menuItemRinggroupsDefault_Click(object sender,EventArgs e) {
			PhoneUI.QueueDefault(_phoneClicked);
		}

		private void menuItemRinggroupBackup_Click(object sender,EventArgs e) {
			PhoneUI.QueueBackup(_phoneClicked);
		}

		private void menuItemLunch_Click(object sender,EventArgs e) {
			PhoneUI.Lunch(_phoneClicked);
		}

		private void menuItemHome_Click(object sender,EventArgs e) {
			PhoneUI.Home(_phoneClicked);
		}

		private void menuItemBreak_Click(object sender,EventArgs e) {
			PhoneUI.Break(_phoneClicked);
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			GoToPatient?.Invoke(this,_phoneClicked.PatNum);
		}

		private void menuItemEmployeeSettings_Click(object sender,EventArgs e) {
			PhoneUI.ShowEmployeeSettings(_phoneClicked);
		}

		private void SetToolStripGroupText(string groupName,string itemText) {
			ToolStripItem[] toolStripItemsArrayFound=menuStatus.Items.Find(groupName,false);
			if(toolStripItemsArrayFound.IsNullOrEmpty()) {
				return;
			}
			toolStripItemsArrayFound[0].Text=itemText;
		}

		private void SetToolStripItemText(string toolStripItemName,bool isClockedIn) {
			ToolStripItem[] toolStripItemsArrayFound=menuStatus.Items.Find(toolStripItemName,false);
			if(toolStripItemsArrayFound.IsNullOrEmpty()) {
				return;
			}
			//set back to default
			toolStripItemsArrayFound[0].Text=toolStripItemsArrayFound[0].Text.Replace(" (Not Clocked In)","");
			if(isClockedIn) {
				toolStripItemsArrayFound[0].Enabled=true;
				return;
			}
			toolStripItemsArrayFound[0].Enabled=false;
			toolStripItemsArrayFound[0].Text=toolStripItemsArrayFound[0].Text+" (Not Clocked In)";
		}
		#endregion Methods - Menu

		#region Methods - Mouse
		private void MapPanel_MouseDoubleClick(object sender,MouseEventArgs e){
			if(!IsEditMode){
				return;
			}
			_wasDoubleClick=true;
			int idx=HitTest(e.Location);
			if(idx==-1){
				return;
			}
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaCur=ListMapAreas[idx];
			formMapAreaEdit.ShowDialog(this);
			if(formMapAreaEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			IsChanged?.Invoke(this, new EventArgs());
			RefreshEditMode(_mapAreaContainer);
		}

		private void MapPanel_MouseDown(object sender,MouseEventArgs e) {
			if(IsEditMode){
				MapPanel_MouseDown_EditMode(e);
			}
			else{
				MapPanel_MouseDown_Normal(e);
			}
		}

		private void MapPanel_MouseDown_EditMode(MouseEventArgs e){
			_wasDoubleClick=false;
			int idx=HitTest(e.Location);
			_listSelected=new List<int>();
			if(idx==-1){
				Invalidate();
				return;
			}
			_isMouseDown=true;
			_pointMouseDown=e.Location;
			_pointMouseNow=e.Location;
			_dateTimeMouseDown=DateTime.Now;
			_listSelected.Add(idx);
			Invalidate();
		}

		private void MapPanel_MouseDown_Normal(MouseEventArgs e){
			int idx=HitTestCubicle(e.Location);
			_pointMouseDown=e.Location;
			_isMouseDown=true;
			_pointFTranslationOld=_pointFTranslation;
			CubicleClickedDetail cubicleClickedDetail=new CubicleClickedDetail();
			if(idx==-1){
				CubicleClicked?.Invoke(this,cubicleClickedDetail);
				return;
			}
			cubicleClickedDetail.EmployeeName=ListMapAreaMores[idx].EmployeeName;
			cubicleClickedDetail.EmployeeNum=ListMapAreaMores[idx].EmployeeNum;
			cubicleClickedDetail.Extension=ListMapAreas[idx].Extension;
			cubicleClickedDetail.Status=ListMapAreaMores[idx].Status;
			cubicleClickedDetail.TimeSpanElapsed=ListMapAreaMores[idx].TimeSpanElapsed;
			cubicleClickedDetail.Description=ListMapAreas[idx].Description;
			cubicleClickedDetail.CustomerNumber=ListMapAreaMores[idx].CustomerNumber;
			if(e.Button==MouseButtons.Left) {
				if(ListMapAreaMores[idx].ClockStatus==ClockStatusEnum.NeedsHelp){
					//User is clicking to change status from NeedsHelp to HelpOnTheWay
					OpenDentBusiness.Phones.SetPhoneStatus(ClockStatusEnum.HelpOnTheWay,ListMapAreas[idx].Extension);
					ODThread.WakeUpThreadsByGroupName(FormOpenDental.FormODThreadNames.HqMetrics.GetDescription());
					cubicleClickedDetail.Status=OpenDentBusiness.Phones.ConvertClockStatusToString(ClockStatusEnum.HelpOnTheWay);
					CubicleClicked?.Invoke(this,cubicleClickedDetail);
					return;
				}
				if(ListMapAreaMores[idx].ClockStatus==ClockStatusEnum.HelpOnTheWay){
					//User is clicking to change status from HelpOnTheWay to Available
					OpenDentBusiness.Phones.SetPhoneStatus(ClockStatusEnum.Available,ListMapAreas[idx].Extension);
					ODThread.WakeUpThreadsByGroupName(FormOpenDental.FormODThreadNames.HqMetrics.GetDescription());
					cubicleClickedDetail.Status=OpenDentBusiness.Phones.ConvertClockStatusToString(ClockStatusEnum.Available);
					CubicleClicked?.Invoke(this,cubicleClickedDetail);
					return;
				}
				CubicleClicked?.Invoke(this,cubicleClickedDetail);
				if(HitTestName(idx,e.Location)){//left clicked on name
					//this will all happen before mouse up, so one way to handle it would be:
					//_isMouseDown=false;
					//But many other windows could come up, so we should handle them all in the mouse move to prevent things getting stuck on the mouse.
					List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
					PhoneEmpDefault phoneEmpDefault=listPhoneEmpDefaults.Find
						(x=>x.PhoneExt==ListMapAreas[idx].Extension && x.EmployeeNum==ListMapAreaMores[idx].EmployeeNum);
					if(phoneEmpDefault==null) {
						MsgBox.Show("Could not find the selected EmployeeNum/Extension pair in database. Please verify that the correct extension is listed for this user in map setup.");
						return;
					}
					using FormPhoneEmpDefaultEdit formPhoneEmpDefaultEdit=new FormPhoneEmpDefaultEdit();
					formPhoneEmpDefaultEdit.PedCur=phoneEmpDefault;
					formPhoneEmpDefaultEdit.ShowDialog();
					return;
				}
				if(HitTestPhone(idx,e.Location) && ListMapAreaMores[idx].PatNumCall!=0){//left clicked on phone icon
					GoToPatient?.Invoke(this,ListMapAreaMores[idx].PatNumCall);
				}	
				return;
			}
			if(e.Button!=MouseButtons.Right) {
				return;
			}
			//Right click
			CubicleClicked?.Invoke(this,cubicleClickedDetail);
			_phoneClicked=ListMapAreaMores[idx].PhoneCur;
			bool allowStatusEdit=ClockEvents.IsClockedIn(ListMapAreaMores[idx].EmployeeNum);
			if(ListMapAreaMores[idx].EmployeeNum==Security.CurUser.EmployeeNum) {//can always edit yourself
				allowStatusEdit=true;
			}
			if(ListMapAreaMores[idx].ClockStatus==ClockStatusEnum.NeedsHelp) {
				//Always allow any employee to change any other employee from NeedsAssistance to Available
				allowStatusEdit=true;
			}
			string statusOnBehalfOf=ListMapAreaMores[idx].EmployeeName;
			bool allowSetSelfAvailable=false;
			if(!ClockEvents.IsClockedIn(ListMapAreaMores[idx].EmployeeNum) //No one is clocked in at this extension.
				&& !ClockEvents.IsClockedIn(Security.CurUser.EmployeeNum)) //This user is not clocked in either.
			{
				//Vacant extension and this user is not clocked in so allow this user to clock in at this extension.
				statusOnBehalfOf=Security.CurUser.UserName;
				allowSetSelfAvailable=true;
			}
			SetToolStripGroupText("menuItemStatusOnBehalf","Status for: "+statusOnBehalfOf);
			SetToolStripGroupText("menuItemRingGroupOnBehalf","Queues for ext: "+ListMapAreas[idx].Extension.ToString());
			SetToolStripGroupText("menuItemClockOnBehalf","Clock event for: "+ListMapAreaMores[idx].EmployeeName);
			SetToolStripGroupText("menuItemCustomer","Customer: "+ListMapAreaMores[idx].CustomerNumber);
			SetToolStripGroupText("menuItemEmployee","Employee: "+ListMapAreaMores[idx].EmployeeName);
			SetToolStripItemText("menuItemAvailable",allowStatusEdit || allowSetSelfAvailable);
			SetToolStripItemText("menuItemTraining",allowStatusEdit);
			SetToolStripItemText("menuItemTeamAssist",allowStatusEdit);
			SetToolStripItemText("menuItemNeedsHelp",allowStatusEdit);
			SetToolStripItemText("menuItemWrapUp",allowStatusEdit);
			SetToolStripItemText("menuItemOfflineAssist",allowStatusEdit);
			SetToolStripItemText("menuItemUnavailable",allowStatusEdit);
			SetToolStripItemText("menuItemTCResponder",allowStatusEdit);
			SetToolStripItemText("menuItemBackup",allowStatusEdit);
			SetToolStripItemText("menuItemLunch",allowStatusEdit);
			SetToolStripItemText("menuItemHome",allowStatusEdit);
			SetToolStripItemText("menuItemBreak",allowStatusEdit);
			menuItemGoTo.Enabled=true;
			if(ListMapAreaMores[idx].PatNumCall==0) {//disable goto if not a current patient
				menuItemGoTo.Enabled=false;
			}
			menuStatus.Show(PointToScreen(e.Location));
			Application.DoEvents();
		}

		private void MapPanel_MouseLeave(object sender,EventArgs e) {
			if(!IsEditMode){
				return;
			}
			//won't need this unless doing a hover effect.
		}

		private void MapPanel_MouseMove(object sender,MouseEventArgs e) {
			//if a dialog came up, dragged item could get stuck on mouse, so this fixes it:
			if(Control.MouseButtons!=MouseButtons.Left){
				_isMouseDown=false;
			}
			_pointMouseNow=e.Location;//We will use this in edit mode and normal mode
			if(IsEditMode){
				//if(!_isMouseDown){
				//	return;
				//}
				//The drag effect actually happens entirely within OnPaint.
				Invalidate();
				return;
			}
			if(!_isMouseDown){
				return;
			}
			//Not edit mode, and panning
			float scaleTrans=(float)100/_zoomValue;//example, 2000%, 100/2000=.05, indicating .05 map units for each screen pixel
			float xTrans=_pointFTranslationOld.X+(_pointMouseNow.X-_pointMouseDown.X)*scaleTrans;
			float yTrans=_pointFTranslationOld.Y+(_pointMouseNow.Y-_pointMouseDown.Y)*scaleTrans;
			_pointFTranslation=new PointF(xTrans,yTrans);
			Invalidate();
		}

		private void MapPanel_MouseUp(object sender,MouseEventArgs e) {
			_isMouseDown=false;
			if(!IsEditMode){
				return;
			}
			//From here down, Edit mode
			if(_dateTimeMouseDown.AddMilliseconds(250)>DateTime.Now){
				Invalidate();
				return;//ignore a very fast drag because it's actually a click.
			}
			if(_wasDoubleClick){
				return;
			}
			//float deltaX=_pointMouseNow.X-_pointMouseDown.X;
			//PointF pointFMouseNow=ControlPointToMapPoint(_pointMouseNow);
			for(int i=0;i<ListMapAreas.Count;i++){
				if(!_listSelected.Contains(i)){
					continue;
				}
				float scaleTrans=(float)100/_zoomValue;//example, 2000%, 100/2000=.05, indicating .05 map units for each screen pixel
				double x=ListMapAreas[i].XPos+(_pointMouseNow.X-_pointMouseDown.X)*scaleTrans;
				double y=ListMapAreas[i].YPos+(_pointMouseNow.Y-_pointMouseDown.Y)*scaleTrans;
				if(ListMapAreas[i].ItemType==MapItemType.Cubicle && SnapToFeet){
					ListMapAreas[i].XPos=Math.Round(x);
					ListMapAreas[i].YPos=Math.Round(y);
				}
				else{
					ListMapAreas[i].XPos=x;
					ListMapAreas[i].YPos=y;
				}
				MapAreas.Update(ListMapAreas[i]);
			}
			IsChanged?.Invoke(this, new EventArgs());
			RefreshEditMode(_mapAreaContainer);
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if(IsEditMode){
				return;
			}
			float deltaZoom=_zoomValue*(float)e.Delta/SystemInformation.MouseWheelScrollDelta/8f;//For example, -15 
			float zoomValNew=_zoomValue+deltaZoom;
			//Converting to a float causes the int.maxvalue to still wrap around to a negative value, removed 100 from the maximum to avoid this error.
			if(zoomValNew>=int.MaxValue){
				zoomValNew=(float)int.MaxValue-100;
			}
			if(zoomValNew<1){
				zoomValNew=1;
			}
			_zoomValue=zoomValNew;
			Invalidate();
		}
		#endregion Methods - Mouse

		#region Methods - private
		///<summary>This is so that we can have a using statement in front of each StringFormat and it makes the code more readable.</summary>
		private StringFormat CreateStringFormat(StringAlignment stringAlignmentHoriz,StringAlignment stringAlignmentVert){
			StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.Alignment=stringAlignmentHoriz;
			stringFormat.LineAlignment=stringAlignmentVert;
			return stringFormat;
		}

		///<summary>Converts a point on the control, like a mouse click, into a point on the map.</summary>
		private PointF PointControlToMap(Point pointIn){
			//because we're going backward from the rendering, each step is the "opposite".
			Matrix matrix=new Matrix();
			//to center of screen
			matrix.Translate(-Width/2f,-Height/2f,MatrixOrder.Append);
			//scale
			matrix.Scale(1f/_zoomValue*100f,1f/_zoomValue*100f,MatrixOrder.Append);//1 if no scale
			//from center of screen to center of map (because _pointTranslation is in map coords
			matrix.Translate(-_pointFTranslation.X,-_pointFTranslation.Y,MatrixOrder.Append);
			//from center of map to UL corner of map
			matrix.Translate(_mapAreaContainer.FloorWidthFeet/2f,_mapAreaContainer.FloorHeightFeet/2f,MatrixOrder.Append);
			PointF[] pointFArray={ pointIn };
			matrix.TransformPoints(pointFArray);
			return pointFArray[0];
		}

		private void DrawCubicleLarge(Graphics g,int i){
			GraphicsState graphicsState = g.Save();
			g.TranslateTransform((float)ListMapAreas[i].XPos,(float)ListMapAreas[i].YPos);
			Color colorBack=ListMapAreaMores[i].ColorBack;
			Color colorBorder=ListMapAreaMores[i].ColorBorder;
			if(_isFlashOn && ListMapAreaMores[i].IsFlashing){
				colorBack=ListMapAreaMores[i].ColorBorder;
				colorBorder=ListMapAreaMores[i].ColorBack;
			}
			if(IsEditMode && _listSelected.Contains(i)){
				colorBorder=Color.Red;
			}
			using Brush brushBack=new SolidBrush(colorBack);
			//we leave a 1 pixel white border because it prevents cubicles from touching. which looks best.
			//The .5 pixel is because of how pixel alignment works in GDI+
			//The 17 factor is because there are 17 pixels per foot of map
			RectangleF rectangleF=new RectangleF(1.5f/17,1.5f/17,(float)ListMapAreas[i].Width-4f/17,(float)ListMapAreas[i].Height-4f/17);
			g.FillRectangle(brushBack,rectangleF);
			Pen penOutline=new Pen(colorBorder,2f/17);
			g.DrawRectangle(penOutline,rectangleF.X,rectangleF.Y,rectangleF.Width,rectangleF.Height);
			using StringFormat stringFormatCenter=CreateStringFormat(StringAlignment.Center,StringAlignment.Center);
			using Brush brushText=new SolidBrush(ListMapAreaMores[i].ColorFont);
			using Font font=new Font("Calibri",LayoutManager.UnscaleMS(14)/17,FontStyle.Bold);//This makes it so that we draw a smaller font because MS will automatically make it bigger
			if(ListMapAreaMores[i].IsEmpty) { //empty room, so gray out and return
				rectangleF=new RectangleF(0,0,(float)ListMapAreas[i].Width,(float)ListMapAreas[i].Height);
				g.DrawString("EMPTY",font,brushText,rectangleF,stringFormatCenter);
				g.Restore(graphicsState);
				return;
			}
			//5 rows total
			float heightRow=(float)ListMapAreas[i].Height/5f;
			//==================== row 1 - EMPLOYEE NAME ====================
			rectangleF=new RectangleF(0,5f/17,(float)ListMapAreas[i].Width,heightRow);
			using Font fontHeader=new Font("Calibri",LayoutManager.UnscaleMS(18)/17,FontStyle.Bold);
			FitText(ListMapAreaMores[i].EmployeeName,fontHeader,brushText,rectangleF,stringFormatCenter,g);
			float yPos=heightRow;
			//==================== row 2 - ELAPSED TIME ====================
			rectangleF=new RectangleF(0,yPos+4f/17,(float)ListMapAreas[i].Width,heightRow);
			string timeSpanElapsed=ListMapAreaMores[i].TimeSpanElapsed.ToStringHmmss();
			if(IsEditMode){
				timeSpanElapsed=ListMapAreas[i].Description;//show description instead of time
			}
			FitText(timeSpanElapsed,font,brushText,rectangleF,stringFormatCenter,g);
			yPos+=heightRow;
			//==================== row 3 - EMPLOYEE EXTENSION ====================
			//Display employee extension if they are present at their desk
			if(ListMapAreaMores[i].IsProx || IsEditMode) {
				rectangleF=new RectangleF(0,yPos,(float)ListMapAreas[i].Width,heightRow);
				FitText("x"+ListMapAreas[i].Extension,font,brushText,rectangleF,stringFormatCenter,g);
			}
			yPos+=heightRow;
			//==================== row 4 - STATUS TEXT ====================
			rectangleF=new RectangleF(0,yPos-3f/17,(float)ListMapAreas[i].Width,heightRow);
			FitText(ListMapAreaMores[i].Status,font,brushText,rectangleF,stringFormatCenter,g);
			yPos+=heightRow;
			//==================== row 5 (Left) - PROXIMITY ====================
			if(ListMapAreaMores[i].IsHome) {
				DrawImage(g,_bitmapHouse,7f/17,yPos-2f/17,1);
			}
			else if(ListMapAreaMores[i].IsProx){
				DrawImage(g,_bitmapProxFig,7f/17,yPos-2f/17,1);
			}
			else if(ListMapAreaMores[i].IsProxAway) {
				DrawImage(g,_bitmapProxAway,7f/17,yPos-2f/17,1);
			}
			//==================== row 5 (Middle) - CHAT ICONS ====================
			if(ListMapAreaMores[i].IsChat) {//or webchat
				DrawImage(g,_bitmapChat,(float)ListMapAreas[i].Width/2-9f/17,yPos-2f/17,1);
			}
			if(ListMapAreaMores[i].IsRemoteSupport) {
				DrawImage(g,_bitmapRemote,(float)ListMapAreas[i].Width/2-9f/17,yPos-2f/17,1);
			}
			//==================== row 5 (Right) - PHONE ICON ====================
			if(ListMapAreaMores[i].IsPhone) {
				DrawImage(g,_bitmapPhone,(float)ListMapAreas[i].Width-27f/17,yPos-2f/17,1);
			}
			g.Restore(graphicsState);
		}

		private void DrawCubicleSmall(Graphics g,int i){
			GraphicsState graphicsState = g.Save();
			g.TranslateTransform((float)ListMapAreas[i].XPos,(float)ListMapAreas[i].YPos);
			Color colorBack=ListMapAreaMores[i].ColorBack;
			Color colorBorder=ListMapAreaMores[i].ColorBorder;
			if(_isFlashOn && ListMapAreaMores[i].IsFlashing){
				colorBack=ListMapAreaMores[i].ColorBorder;
				colorBorder=ListMapAreaMores[i].ColorBack;
			}
			if(IsEditMode && _listSelected.Contains(i)){
				colorBorder=Color.Red;
			}
			using Brush brushBack=new SolidBrush(colorBack);
			//we leave a 1 pixel white border because it prevents cubicles from touching.
			//The .5 pixel is because of how pixel alignment works in GDI+
			RectangleF rectangleF=new RectangleF(1.5f/17,1.5f/17,(float)ListMapAreas[i].Width-4f/17,(float)ListMapAreas[i].Height-4f/17);
			g.FillRectangle(brushBack,rectangleF);
			Pen penOutline=new Pen(colorBorder,2f/17);
			//penOutline.Alignment=PenAlignment.Inset;
			g.DrawRectangle(penOutline,rectangleF.X,rectangleF.Y,rectangleF.Width,rectangleF.Height);
			using StringFormat stringFormatCenter=CreateStringFormat(StringAlignment.Center,StringAlignment.Center);
			using Brush brushText=new SolidBrush(ListMapAreaMores[i].ColorFont);
			using Font font=new Font("Calibri",LayoutManager.UnscaleMS(14f/17),FontStyle.Bold);
			if(ListMapAreaMores[i].IsEmpty) { //empty room, so gray out and return
				rectangleF=new RectangleF(0,0,(float)ListMapAreas[i].Width,(float)ListMapAreas[i].Height);
				g.DrawString("EMPTY",font,brushText,rectangleF,stringFormatCenter);
				g.Restore(graphicsState);
				return;
			}
			//3 rows total
			float heightRow=(float)ListMapAreas[i].Height/3f;
			//==================== row 1 - EMPLOYEE NAME ====================
			rectangleF=new RectangleF(0,4f/17,(float)ListMapAreas[i].Width,heightRow);
			using Font fontHeader=new Font("Calibri",LayoutManager.UnscaleMS(18f/17),FontStyle.Bold);
			FitText(ListMapAreaMores[i].EmployeeName,fontHeader,brushText,rectangleF,stringFormatCenter,g);
			float yPos=heightRow;
			//==================== row 2 - ELAPSED TIME ====================
			rectangleF=new RectangleF(0,yPos,(float)ListMapAreas[i].Width,heightRow);
			string timeSpanElapsed=ListMapAreaMores[i].TimeSpanElapsed.ToStringHmmss();
			//todo: I think there is room for seconds
			if(ListMapAreaMores[i].TimeSpanElapsed.Hours>0) { //no room for hours and seconds
				timeSpanElapsed=ListMapAreaMores[i].TimeSpanElapsed.Hours+"hr "+ListMapAreaMores[i].TimeSpanElapsed.Minutes;
			}
			if(IsEditMode){
				timeSpanElapsed=ListMapAreas[i].Description;//show description instead of time
			}
			FitText(timeSpanElapsed,font,brushText,rectangleF,stringFormatCenter,g);
			//g.DrawRectangle(Pens.Red,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
			yPos+=heightRow;
			//row 3 edit mode
			if(IsEditMode){
				rectangleF=new RectangleF(0,yPos-4f/17,(float)ListMapAreas[i].Width,heightRow);
				FitText("x"+ListMapAreas[i].Extension,font,brushText,rectangleF,stringFormatCenter,g);
			}
			//==================== row 3 (Left) - PROXIMITY ====================
			if(ListMapAreaMores[i].IsHome) {
				DrawImage(g,_bitmapHouse,5f/17,yPos,1);
			}
			else if(ListMapAreaMores[i].IsProx){
				DrawImage(g,_bitmapProxFig,5f/17,yPos,1);
			}
			else if(ListMapAreaMores[i].IsProxAway) {
				DrawImage(g,_bitmapProxAway,5f/17,yPos,1);
			}
			//==================== row 3 (Middle) - CHAT ICONS ====================
			if(ListMapAreaMores[i].IsChat) {//or webchat
				DrawImage(g,_bitmapChat,(float)ListMapAreas[i].Width/2-9f/17,yPos,1);
			}
			if(ListMapAreaMores[i].IsRemoteSupport) {
				DrawImage(g,_bitmapRemote,(float)ListMapAreas[i].Width/2-9f/17,yPos,1);
			}
			//==================== row 3 (Right) - PHONE ICON ====================
			if(ListMapAreaMores[i].IsPhone) {
				DrawImage(g,_bitmapPhone,(float)ListMapAreas[i].Width-25f/17,yPos,1);
			}
			g.Restore(graphicsState);
		}

		///<summary>Draws bitmap at the x,y coords specified and scaled to fit the specified height.</summary>
		private void DrawImage(Graphics g,Bitmap bitmap,float x,float y,float height){
			//For consistency, we currently always pass in a height of 1 so that the icons
			//all draw 1 foot high and look the same.
			float scale=height/bitmap.Height;//this is less than 1
			g.DrawImage(bitmap,x,y,bitmap.Width*scale,bitmap.Height*scale);
		}

		private void DrawLabel(Graphics g,int i){
			//The label Width and Height are completely ignored.
			//Make the width and height match the measured text size.
			using Font font=new Font("Calibri",LayoutManager.UnscaleMS(14f/17),FontStyle.Bold);
			SizeF sizeF=g.MeasureString(ListMapAreas[i].Description,font);
			using Brush brushBack=new SolidBrush(ListMapAreaMores[i].ColorBack);
			RectangleF rectangleF=new RectangleF((float)ListMapAreas[i].XPos,(float)ListMapAreas[i].YPos,sizeF.Width,font.Height-2);
			g.FillRectangle(brushBack,rectangleF);
			using Brush brushFont=new SolidBrush(ListMapAreaMores[i].ColorFont);
			g.DrawString(ListMapAreas[i].Description,font,brushFont,(float)ListMapAreas[i].XPos,(float)ListMapAreas[i].YPos);
		}

		///<summary>Replaces Graphics.DrawString. If the text is wider than will fit, then this reduces its size.  It does not consider height.</summary>
		private void FitText(string text,Font font,Brush brush,RectangleF rectangleF,StringFormat stringFormat,Graphics g) {
			if(text.IsNullOrEmpty()){
				return;
			}
			float emSize=font.Size;
			SizeF sizeF=g.MeasureString(text,font);//this is too big, so we have to unscale it
			float widthText=sizeF.Width;
			widthText=LayoutManager.UnscaleMS(widthText);
			if(widthText>=rectangleF.Width) {
				emSize=emSize*(rectangleF.Width/widthText);//get the ratio of the room width to font width and multiply that by the font size
			}
			//but because of the quantum nature of fonts, this sometimes fails to draw the last letter.
			//so the rectangle needs to be slightly bigger
			rectangleF.Width=rectangleF.Width*1.05f;
			rectangleF.X=rectangleF.X-rectangleF.Width*0.025f;
			using Font fontNew=new Font(font.FontFamily,emSize,font.Style);
			g.DrawString(text,fontNew,brush,rectangleF,stringFormat);
			//g.DrawString(text,font,brush,rectangleF,stringFormat);
		}

		///<summary>Returns idx of cubicle clicked on, or -1 if no valid cubicle. Ignores labels. Pass in control coords.</summary>
		private int HitTestCubicle(Point point){
			PointF pointF=PointControlToMap(point);
			for(int i=0;i<ListMapAreas.Count;i++){
				if(ListMapAreas[i].ItemType!=MapItemType.Cubicle){
					continue;
				}
				if(ListMapAreaMores[i].RectangleFBounds.Contains(pointF.X,pointF.Y)){
					return i;
				}
			}
			return -1;
		}

		///<summary>Returns idx of cubicle or label clicked on, or -1 if no valid MapArea.</summary>
		private int HitTest(Point point){
			PointF pointF=PointControlToMap(point);
			//So from here down, we work entirely in map coords.  It should look a lot like the drawing code, which is also in map coords.
			Graphics g=this.CreateGraphics();
			//In this MapPanel, non of the lines and rectangles are scaled by MS amount.
			//So we need to unscale all fonts by MS amount prior to measuring or drawing.
			//There is also no zoom component in the MapPanel.
			//But the math below seems wrong to me, both for MeasureString, which should be unscaled,
			//and for font.Height, which shouldn't be. But it looks good.
			//Not sure why.  Maybe it's how we got the Graphics object.
			using Font font=new Font("Calibri",14f/17,FontStyle.Bold);
			//first lables, since they are on top
			for(int i=0;i<ListMapAreas.Count;i++){
				if(ListMapAreas[i].ItemType!=MapItemType.Label){
					continue;
				}
				string desc=ListMapAreas[i].Description;
				float width=g.MeasureString(desc,font).Width;
				float height=font.Height;
				height=LayoutManager.UnscaleMS(height);
				RectangleF rectanglef=new RectangleF((float)ListMapAreas[i].XPos,(float)ListMapAreas[i].YPos,width,height);
				if(rectanglef.Contains(pointF.X,pointF.Y)){
					return i;
				}
			}
			//then cubicles
			for(int i=0;i<ListMapAreas.Count;i++){
				if(ListMapAreas[i].ItemType!=MapItemType.Cubicle){
					continue;
				}
				if(ListMapAreaMores[i].RectangleFBounds.Contains(pointF.X,pointF.Y)){
					return i;
				}
			}
			return -1;
		}

		///<summary>Pass in the idx of the cubicle in question. Tests to see of the click was on the name area.  x and y are for the entire mapPanel.</summary>
		private bool HitTestName(int idx,Point point){
			PointF pointF=PointControlToMap(point);
			if(ListMapAreas[idx].Width<6) {//small cubicle
				float heightRow=(float)ListMapAreas[idx].Height/3f;
				RectangleF rectangleF=new RectangleF((float)ListMapAreas[idx].XPos,(float)ListMapAreas[idx].YPos+4f/17,(float)ListMapAreas[idx].Width,heightRow);
				return rectangleF.Contains(pointF.X,pointF.Y);
			}
			//large cubicle
			float heightRow2=(float)ListMapAreas[idx].Height/5f;
			RectangleF rectangleF2=new RectangleF((float)ListMapAreas[idx].XPos,(float)ListMapAreas[idx].YPos+5f/17,(float)ListMapAreas[idx].Width,heightRow2);
			return rectangleF2.Contains(pointF.X,pointF.Y);
		}

		///<summary>Pass in the idx of the cubicle in question. Tests to see of the click was on the phone icon.  x and y are for the entire mapPanel.</summary>
		private bool HitTestPhone(int idx,Point point){
			PointF pointF=PointControlToMap(point);
			float heightRow;
			RectangleF rectangleF;
			if(ListMapAreas[idx].Width<6) {//small cubicle
				heightRow=(float)ListMapAreas[idx].Height/3f;
				rectangleF=new RectangleF(
					x:(float)ListMapAreas[idx].Width-25f/17,
					y:(float)ListMapAreas[idx].Height-heightRow,
					width:_bitmapPhone.Width/17f,
					height:heightRow);
				return rectangleF.Contains(pointF.X,pointF.Y);
			}
			//large cubicle
			heightRow=(float)ListMapAreas[idx].Height/5f;
			//ListMapAreaMores[i].Width-27,yPos-2,heightRow);
			rectangleF=new RectangleF(
				x:(float)ListMapAreas[idx].Width-27f/17,
				y:(float)ListMapAreas[idx].Height-heightRow-2f/17,
				width:_bitmapPhone.Width/17f,
				height:heightRow);
			return rectangleF.Contains(pointF.X,pointF.Y);
		}

		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(!DesignMode){
				_timeSpanDelta=MiscData.GetNowDateTime()-DateTime.Now;
			}
		}

		private void _timerFlash_Tick(object sender,EventArgs e){
			_isFlashOn=!_isFlashOn;//simple flip
			if(ListMapAreaMores.Any(x=>x.IsFlashing)){
				Invalidate();
			}
		}


		#endregion Methods - private

		
	}

	///<summary>This class stores additional info about each MapArea, such as colors and timespans.</summary>
	public class MapAreaMore{
		///<summary>Also see Status.</summary>
		public ClockStatusEnum ClockStatus;
		public Color ColorBorder;
		public Color ColorBack;
		public Color ColorFont;
		public string CustomerNumber;
		public string EmployeeName;
		public long EmployeeNum;
		///<summary>Includes both chat and webchat.</summary>
		public bool IsChat;
		public bool IsEmpty;
		public bool IsFlashing;
		public bool IsHome;
		public bool IsPhone;
		public bool IsProx;
		public bool IsProxAway;
		public bool IsRemoteSupport;
		public long PatNumCall;
		///<summary>Use this as rarely as possible.  Same info is usually found in a different field.</summary>
		public Phone PhoneCur;
		///<summary>The bounds of this item. For example, about 6x6 for a cubicle.</summary>
		public RectangleF RectangleFBounds;
		///<summary>A string version of ClockStatusEnum which is shortened specifically for display in a tight space.</summary>
		public string Status;
		public TimeSpan TimeSpanElapsed;

		public override string ToString() {
			return EmployeeName;
		}
	}

	///<summary>When a cubicle is clicked this object gets passed up in an event.</summary>
	public class CubicleClickedDetail{
		public long EmployeeNum;
		public string EmployeeName;
		public int Extension;
		public string Status;
		public TimeSpan TimeSpanElapsed;
		public string Description;
		public string CustomerNumber;
	}
}
