using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Used as cubicles in the HQ map.</summary>
	public partial class MapCubicle:DraggableControl {

		#region Field not available in designer.
		///<summary>Use this sparingly.  Fonts and scale handled from parent.</summary>
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		public MapArea MapAreaCur=new MapArea();
		///<summary>Holds the phone object for the related Extension.</summary>
		public Phone PhoneCur;
		///<summary>Indicates whether or not the proximity monitor has found the user at their desk.  Gets set at the same time as ProxImage</summary>
		public bool IsAtDesk=false;
		///<summary>Indicates if we came from the map setup window, will disable right click functionality if false.</summary>
		public bool AllowRightClick;
		/// <summary>Area of the cubicle where the employee name is drawn.</summary>
		private RectangleF _rectangleName;
		/// <summary>Area of the cubicle where the phone icon is drawn.</summary>
		private RectangleF _rectanglePhone;

		#endregion

		#region Properties available in designer.

		[Category("OD")]
		[Description("Primary Key From employee Table")]
		public long EmployeeNum { get; set; }

		[Category("OD")]
		[Description("Employee's Name")]
		public string EmployeeName { get; set; }

		[Category("OD")]
		[Description("Employee's Phone Extension #")]
		public string Extension { get; set; }

		[Category("OD")]
		[Description("Elapsed Time Since Last Status Change")]
		public TimeSpan Elapsed { get; set; }

		[Category("OD")]
		[Description("Current Employee Status")]
		public string Status { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current Phone Status")]
		public Image PhoneImage { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current Phone Status")]
		public Image ChatImage { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current WebChat Status")]
		public Image WebChatImage { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current Proximity Status")]
		public Image ProxImage { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current Remote Support Status")]
		public Image RemoteSupportImage { get; set; }

		[Category("OD")]
		[Description("Overrides the drawing of the control and just makes it look like a label with a custom border")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Bindable(true)]
		public override string Text {
			get {
				return base.Text;
			}
			set {
				base.Text = value;
				Invalidate();
			}
		}

		private int _borderThickness=4;
		[Category("OD")]
		[Description("Thickness of the border drawn around the control")]
		public int BorderThickness {
			get {
				return _borderThickness;
			}
			set {
				_borderThickness=value;
				Invalidate();
			}
		}

		///<summary>Set when flashing starts so we know what inner color to go back to.</summary>
		private Color _innerColorRestore=Color.FromArgb(128,Color.Red);
		private Color DefaultOuterColor=Color.Red;
		[Category("OD")]
		[Description("Exterior Border Color")]
		public Color OuterColor {
			get {
				return DefaultOuterColor;
			}
			set {
				DefaultOuterColor=value;
				Invalidate();
			}
		}

		///<summary>Set when flashing starts so we know what outer color to go back to.</summary>
		private Color _outerColorRestore=Color.Red;
		private Color DefaultInnerColor=Color.FromArgb(128,Color.Red);
		[Category("OD")]
		[Description("Interior Fill Color")]
		public Color InnerColor {
			get {
				return DefaultInnerColor;
			}
			set {
				DefaultInnerColor=value;
				Invalidate();
			}
		}

		private bool IsEmpty=false;
		[Category("OD")]
		[Description("No Extension Assigned")]
		public bool Empty {
			get {
				return IsEmpty;
			}
			set {
				IsEmpty=value;
				Invalidate();
			}
		}

		private bool _allowEdit=false;
		[Category("OD")]
		[Description("Double-click will open editor")]
		public bool AllowEdit {
			get {
				return _allowEdit;
			}
			set {
				_allowEdit=value;
			}
		}

		private Font _fontHeader=SystemFonts.DefaultFont;
		[Category("OD")]
		[Description("Font used for the top row. Generally reserved for the name of the MapAreaRoom.")]
		public Font FontHeader {
			get {
				return _fontHeader;
			}
			set {
				_fontHeader=value;
				Invalidate();
			}
		}

		public bool IsFlashing(){
			return timerFlash.Enabled;
		}

		#endregion

		#region Events

		public event EventHandler MapCubicleEdited;
		public event EventHandler RoomControlClicked;
		[Category("OD"),Description("Event raised when user wants to go to a patient or related object.")]
		public event EventHandler ClickedGoTo=null;

		#endregion

		#region Ctor

		///<summary>Default. Must be called by all other ctors as we will call InitializeComponent here.</summary>
		public MapCubicle() {
			InitializeComponent();
		}

		#endregion

		#region Drawing

		public void StartFlashing() {
			if(IsFlashing()) { //already on
				return;
			}
			//save the colors
			_outerColorRestore=OuterColor;
			_innerColorRestore=InnerColor;
			timerFlash.Start();
		}

		public void StopFlashing() {
			if(!IsFlashing()) { //already off
				return;
			}
			timerFlash.Stop();
			OuterColor=_outerColorRestore;
			InnerColor=_innerColorRestore;
		}

		public void SetNormalColors() {
			SetColors(Color.Black,Color.Black,Color.White);
		}

		public void SetWarnColors() {
			SetColors(Color.Black,Color.Black,Color.FromArgb(255,237,102));
		}

		public void SetAlertColors() {
			SetColors(Color.White,Color.Black,Color.Red);
		}

		public void SetTriageColors(long siteNum=0) {
			SetColors(SiteLinks.GetSiteForeColorBySiteNum(siteNum,Color.Black)
				,SiteLinks.GetSiteOuterColorBySiteNum(siteNum,Phones.PhoneColorScheme.COLOR_DUAL_OuterTriage)
				,SiteLinks.GetSiteInnerColorBySiteNum(siteNum,Phones.PhoneColorScheme.COLOR_DUAL_InnerTriageHere));
		}

		public void SetColors(Color foreColor,Color outerColor,Color innerColor) {
			ForeColor=foreColor;
			OuterColor=outerColor;
			InnerColor=innerColor;
		}

		private void timerFlash_Tick(object sender,EventArgs e) {
			//flip inner and outer colors
			if(OuterColor==_outerColorRestore) {
				OuterColor=_innerColorRestore;
				InnerColor=_outerColorRestore;
			}
			else {
				OuterColor=_outerColorRestore;
				InnerColor=_innerColorRestore;
			}
		}

		private void MapCubicle_Paint(object sender,PaintEventArgs e) {
			//use the width of the cubicle to determine which paint method we should use.
			if(this.Size.Width<LayoutManager.Scale(102)) {
				PaintSmall(e);
			}
			else {
				PaintLarge(e);
			}
		}

		///<summary>The original way to paint all cubicles.</summary>
		private void PaintLarge(PaintEventArgs e) {
			Graphics g=e.Graphics;//alias
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			using Brush brushInner=new SolidBrush(Empty?Color.FromArgb(20,Color.Gray):InnerColor);
			using Brush brushText=new SolidBrush(Empty?Color.FromArgb(128,Color.Gray):ForeColor);
			using Pen penOuter=new Pen(Empty?Color.FromArgb(128,Color.Gray):OuterColor,BorderThickness);
			_rectanglePhone=RectangleF.Empty;
			_rectangleName=RectangleF.Empty;
			string timeElapsed=TimeSpanToStringHelper(Elapsed);
			try {
				RectangleF rcOuter=this.ClientRectangle;
				//clear control canvas
				g.Clear(this.BackColor);
				float halfPenThickness=BorderThickness/(float)2;
				//deflate for border
				rcOuter.Inflate(-halfPenThickness,-halfPenThickness);
				//draw border
				g.DrawRectangle(penOuter,rcOuter.X,rcOuter.Y,rcOuter.Width,rcOuter.Height);
				//deflate to drawable region
				rcOuter.Inflate(-halfPenThickness,-halfPenThickness);
				//fill interior
				g.FillRectangle(brushInner,rcOuter);
				StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
				stringFormat.Alignment=StringAlignment.Center;
				stringFormat.LineAlignment=StringAlignment.Center;
				if(this.Empty) { //empty room so gray out and return
					g.DrawString("EMPTY",Font,brushText,rcOuter,stringFormat);
					return;
				}
				else if(Text!="") { //using as a label so just draw the string					
					FitText(Text,Font,brushText,new RectangleF(rcOuter.Left,rcOuter.Top+2,rcOuter.Width,rcOuter.Height),stringFormat,g);
					return;
				}
				//3 rows of data
				int rowsLowestCommonDenominator=6;
				float typicalRowHeight=rcOuter.Height/(float)rowsLowestCommonDenominator;
				//==================== row 1 - EMPLOYEE NAME ====================
				float rowHeight=typicalRowHeight*2; //row 1 is 2/6 tall
				_rectangleName=new RectangleF(rcOuter.X,rcOuter.Y-2,rcOuter.Width,rowHeight);
				FitText(EmployeeName,FontHeader,brushText,_rectangleName,stringFormat,g);
				float yPosBottom=rowHeight;
				//g.DrawRectangle(Pens.LimeGreen,rcOuter.X,rcOuter.Y,rcOuter.Width,rowHeight);
				//==================== row 2 - ELAPSED TIME ====================
				rowHeight=typicalRowHeight*2; //row 2 is 2/6 tall
				FitText(timeElapsed,Font,brushText,new RectangleF(rcOuter.X,rcOuter.Y+yPosBottom-14,rcOuter.Width,rowHeight),stringFormat,g);
				//g.DrawRectangle(Pens.Red,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
				yPosBottom+=rowHeight;
				//==================== row 3 (Middle) - EMPLOYEE EXTENSION ====================
				//Display employee extension if they are present at their desk
				if(IsAtDesk) {
					FitText("x"+Extension,Font,brushText,new RectangleF(rcOuter.X,rcOuter.Y+yPosBottom-30,rcOuter.Width,rowHeight),stringFormat,g);
				}
				//==================== row 4 (Bottom) - EMPLOYEE STATUS ====================
				//left-most 3/4 of row 3 is the status text
				FitText(Status,Font,brushText,new RectangleF(rcOuter.X+(rcOuter.Width/6)-5,rcOuter.Y+yPosBottom-14,((rcOuter.Width/6)*4)+4,rowHeight),stringFormat,g);
				//FitText(Status,Font,brushText,new RectangleF(rcOuter.X+(rcOuter.Width/6)-2,rcOuter.Y+yPosBottom+1,((rcOuter.Width/6)*4)+4,rowHeight),stringFormat,g);
				//==================== row 5 (Left) - PROXIMITY STATUS ====================
				int iconShiftCenter = 8;
				rowHeight =typicalRowHeight*2; //row 3 is 2/6 tall
				if(ProxImage!=null) {
					//right-most 1/4 of row 3 is the phone icon
					RectangleF rect = new RectangleF(rcOuter.X-2+iconShiftCenter,rcOuter.Y+yPosBottom+4,ProxImage.Width,rowHeight);
					//Scale the image.
					if(ProxImage.Height<rect.Height) {
						rect.Y+=(rect.Height-ProxImage.Height)/2;
						rect.Height=ProxImage.Height;
					}
					if(ProxImage.Width<rect.Width) {
						rect.X-=(rect.Width-ProxImage.Width)/2;
						rect.Width=ProxImage.Width;
					}
					g.DrawImage(
						ProxImage,
						rect,
						new RectangleF(0,0,ProxImage.Width,ProxImage.Height),
						GraphicsUnit.Pixel);
					//g.DrawRectangle(Pens.Orange,rectImage.X,rectImage.Y,rectImage.Width,rectImage.Height);
					//using(Font fnt = new Font("Arial",19,FontStyle.Regular)) {
					//	TextRenderer.DrawText(g,"👤",fnt,new Point((int)rcOuter.X-6,(int)rcOuter.Y+(int)yPosBottom+3),Color.FromArgb(96,96,96));
					//	//FitText uses g.DrawString() which does not handle unicode characters well.
					//	//FitText("👤",fnt,Brushes.Gray,new RectangleF(rcOuter.X-2,rcOuter.Y+yPosBottom+1,((rcOuter.Width/6))+1,rowHeight),stringFormat,g,true);
					//}
				}
				//Only show the Phone icon when employee is on the phone. Do not show a chat icon along with the phone icon.
				//==================== row 5 (right) - PHONE ICON ====================
				if(PhoneImage!=null) {
					//right-most 1/4 of row 3 is the phone icon
					RectangleF rect=new RectangleF((rcOuter.X+(rcOuter.Width/6)*5)-BorderThickness-2-iconShiftCenter,rcOuter.Y+yPosBottom+4,PhoneImage.Width,rowHeight);
					//Scale the image.
					if(PhoneImage.Height<rect.Height) {
						rect.Y+=(rect.Height-PhoneImage.Height)/2;
						rect.Height=PhoneImage.Height;
					}
					if(PhoneImage.Width<rect.Width) {
						rect.X-=(rect.Width-PhoneImage.Width)/2;
						rect.Width=PhoneImage.Width;
					}
					_rectanglePhone=rect;
					g.DrawImage(
						PhoneImage,
						_rectanglePhone,
						new RectangleF(0,0,PhoneImage.Width,PhoneImage.Height),
						GraphicsUnit.Pixel);
						//g.DrawRectangle(Pens.Orange,rectImage.X,rectImage.Y,rectImage.Width,rectImage.Height);
				}
				//Show a chat icon when the employee is not on the phone.
				//==================== row 5 (middle) - WEB CHAT ICON ====================
				else if(WebChatImage!=null) {
					DisplayChatImage(WebChatImage,e,rcOuter,yPosBottom,rowHeight);
				}
				//==================== row 5 (middle) - CHAT ICON ====================
				else if(ChatImage!=null) {
					DisplayChatImage(ChatImage,e,rcOuter,yPosBottom,rowHeight);
				}
				//==================== row 5 (middle) - REMOTE SUPPORT ICON ====================
				else if(RemoteSupportImage!=null) {
					DisplayChatImage(RemoteSupportImage,e,rcOuter,yPosBottom,rowHeight);
				}
				//g.DrawRectangle(Pens.Blue,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
				yPosBottom+=rowHeight;
			}
			catch(Exception) { }
		}

		///<summary>Draws a modified cube to account for less workable area.</summary>
		private void PaintSmall(PaintEventArgs e) {
			Graphics g=e.Graphics;//alias
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			using Brush brushInner=new SolidBrush(Empty?Color.FromArgb(20,Color.Gray):InnerColor);
			using Brush brushText=new SolidBrush(Empty?Color.FromArgb(128,Color.Gray):ForeColor);
			float halfPenThickness=BorderThickness/(float)2;
			using Pen penOuter=new Pen(Empty?Color.FromArgb(128,Color.Gray):OuterColor,halfPenThickness);
			_rectanglePhone=RectangleF.Empty;
			_rectangleName=RectangleF.Empty;
			string timeElapsed=TimeSpanToStringHelper(Elapsed,true);
			EmployeeName=ShortenEmployeeNameHelper(EmployeeName);
			try {
				RectangleF rcOuter=this.ClientRectangle;
				//clear control canvas
				g.Clear(this.BackColor);
				//deflate for border
				rcOuter.Inflate(-halfPenThickness,-halfPenThickness);
				//draw border
				g.DrawRectangle(penOuter,rcOuter.X,rcOuter.Y,rcOuter.Width,rcOuter.Height);
				//deflate to drawable region
				rcOuter.Inflate(-halfPenThickness,-halfPenThickness);
				//fill interior
				g.FillRectangle(brushInner,rcOuter);
				StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
				stringFormat.Alignment=StringAlignment.Center;
				stringFormat.LineAlignment=StringAlignment.Center;
				if(this.Empty) { //empty room so gray out and return
					g.DrawString("EMPTY",Font,brushText,rcOuter,stringFormat);
					return;
				}
				else if(this.Text!="") { //using as a label so just draw the string					
					FitText(this.Text,Font,brushText,new RectangleF(rcOuter.Left,rcOuter.Top+2,rcOuter.Width,rcOuter.Height),stringFormat,g);
					return;
				}
				//3 rows of data
				int rowsLowestCommonDenominator=6;
				float typicalRowHeight=rcOuter.Height/(float)rowsLowestCommonDenominator;
				//==================== row 1 - EMPLOYEE NAME ====================
				float rowHeight=typicalRowHeight*2; //row 1 is 2/6 tall
				_rectangleName=new RectangleF(rcOuter.X,rcOuter.Y,rcOuter.Width,rowHeight);
				FitText(EmployeeName,FontHeader,brushText,_rectangleName,stringFormat,g);
				float yPosBottom=rowHeight;
				//g.DrawRectangle(Pens.LimeGreen,rcOuter.X,rcOuter.Y,rcOuter.Width,rowHeight);
				//==================== row 2 - ELAPSED TIME ====================
				rowHeight=typicalRowHeight*2;//row 2 is 2/6 tall
				FitText(timeElapsed,Font,brushText,new RectangleF(rcOuter.X,rcOuter.Y+rowHeight,rcOuter.Width,rowHeight),stringFormat,g);
				//g.DrawRectangle(Pens.Red,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
				yPosBottom+=rowHeight;
				int smallMargin=1;
				rowHeight=typicalRowHeight*2;//row 3 is 2/6 tall
				if(ProxImage!=null) {
					//right-most 1/4 of row 3 is the phone icon
					RectangleF rectImage=new RectangleF(rcOuter.X,rcOuter.Y+rowHeight*2-smallMargin,ProxImage.Width,rowHeight);
					//Scale the image.
					if(ProxImage.Height<rectImage.Height || ProxImage.Width<rectImage.Width) {
						rectImage.Y+=(rectImage.Height-ProxImage.Height)/2;
						rectImage.X+=(rectImage.Width-ProxImage.Width)/2;
						rectImage.Height=ProxImage.Height;
						rectImage.Width=ProxImage.Width;
					}
					g.DrawImage(
						ProxImage,
						rectImage,
						new RectangleF(0,0,ProxImage.Width,ProxImage.Height),
						GraphicsUnit.Pixel);
					//g.DrawRectangle(Pens.Orange,rectImage.X,rectImage.Y,rectImage.Width,rectImage.Height);
				}
				//Only show the Phone icon when employee is on the phone. Do not show a chat icon along with the phone icon.
				//==================== row 3 (right) - PHONE ICON ====================
				if(PhoneImage!=null) {
					Size sizeNew=new Size((int)(PhoneImage.Width/1.2),(int)(PhoneImage.Height/1.2));
					RectangleF rectImage=new RectangleF(((rcOuter.Width+BorderThickness)-sizeNew.Width-smallMargin-2),rcOuter.Y+yPosBottom-smallMargin,sizeNew.Width+3,rowHeight);
					//Scale the image.
					if(sizeNew.Height<rectImage.Height || sizeNew.Width<=rectImage.Width) {
						rectImage.Y+=(rectImage.Height-sizeNew.Height)/2;
						rectImage.X+=(rectImage.Width-sizeNew.Width)/2;
						rectImage.Height=sizeNew.Height;
						rectImage.Width=sizeNew.Width;
					}
					_rectanglePhone=rectImage;
					g.DrawImage(
						PhoneImage,
						_rectanglePhone,
						new RectangleF(0,0,sizeNew.Width+3,sizeNew.Height),
						GraphicsUnit.Pixel);
					if(ODBuild.IsDebug()) {
						//Uncomment if you need to see the boundaries of the rectangles being drawn.
						//g.DrawRectangle(Pens.Orange,rectImage.X,rectImage.Y,rectImage.Width,rectImage.Height);
					}
				}
				//Show a chat icon when the employee is not on the phone.
				//==================== row 3 (middle) - WEB CHAT ICON ====================
				else if(WebChatImage!=null) {
					DisplayChatImage(WebChatImage,e,rcOuter,yPosBottom-BorderThickness,rowHeight);
				}
				//==================== row 3 (middle) - CHAT ICON ====================
				else if(ChatImage!=null) {
					DisplayChatImage(ChatImage,e,rcOuter,yPosBottom-BorderThickness,rowHeight);
				}
				//==================== row 3 (middle) - REMOTE SUPPORT ICON ====================
				else if(RemoteSupportImage!=null) {
					DisplayChatImage(RemoteSupportImage,e,rcOuter,yPosBottom-BorderThickness,rowHeight);
				}
				if(ODBuild.IsDebug()) {
					//Uncomment if you need to see the boundaries of the rectangles being drawn.
					//g.DrawRectangle(Pens.Blue,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
				}
				yPosBottom+=rowHeight;
			}
			catch(Exception) { }
		}

		///<summary>Converts a TimeSpan to a string depending on the size of the room and how much time has elapsed.</summary>
		private static string TimeSpanToStringHelper(TimeSpan span,bool isSmallCube=false) {
			string retVal="";
			//Build the smaller time string
			if(isSmallCube) {
				//Over an hour, just display minutes.
				if(span.Hours>0) {
					retVal=(span.Hours+"hr "+span.Minutes).ToString();
				}
				else {
					retVal=span.ToStringmmss();
				}
			}
			else {
				retVal=span.ToStringHmmss();
			}
			return retVal;
		}

		///<summary>Wrapper for String.Substring that just returns the string if it is too short for the Substring operation.</summary>
		private static string ShortenEmployeeNameHelper(string name) {
			if(name==null){
				return "";
			}
			if(name.Length<=8){
				return name;
			}
			return name.Substring(0,8);
		}

		private static void DisplayChatImage(Image img,PaintEventArgs e,RectangleF rcOuter,float yPosBottom,float rowHeight) {
			//right-most 1/4 of row 3 is the phone icon
			RectangleF rectImage=new RectangleF((rcOuter.X+(rcOuter.Width/2))-8,rcOuter.Y+yPosBottom+4,img.Width,rowHeight);
			//Scale the image.
			if(img.Height<rectImage.Height || img.Width<rectImage.Width) {
				rectImage.Y+=(rectImage.Height-img.Height)/2;
				rectImage.Height=img.Height;
				rectImage.X-=(rectImage.Width-img.Width)/2;
				rectImage.Width=img.Width;
			}
			e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
			e.Graphics.DrawImage(
				img,
				rectImage,
				new RectangleF(0,0,img.Width,img.Height),
				GraphicsUnit.Pixel);
		}

		///<summary>Replaces Graphics.DrawString. Finds a suitable font size to fit the text to the bounding rectangle.</summary>
		public static void FitText(string text,Font font,Brush brush,RectangleF rectF,StringFormat stringFormat,Graphics g) {
			float emSize=font.Size;
			Size size=TextRenderer.MeasureText(text,font);
			if(size.Width>=rectF.Width) {
				emSize=emSize*(rectF.Width/size.Width);//get the ratio of the room width to font width and multiply that by the font size
				if(emSize<2) {//don't let the font be smaller than 2 point font
					emSize=2F;
				}
			}
			using(Font newFont=new Font(font.FontFamily,emSize,font.Style)) {
				g.DrawString(text,newFont,brush,rectF,stringFormat);
			}
		}

		#endregion

		#region Mouse events

		private void MapCubicle_DoubleClick(object sender,EventArgs e) {
			if(!AllowEdit) {
				return;
			}
			//edit this room
			using FormMapAreaEdit FormEP=new FormMapAreaEdit();
			FormEP.MapAreaItem=this.MapAreaCur;
			if(FormEP.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			if(MapCubicleEdited!=null) { //let anyone interested know that this cubicle was edited
				MapCubicleEdited(this,new EventArgs());
			}
		}

		private void MapCubicle_Click(object sender,EventArgs e) {
			if(AllowEdit) {
				return; //they're editing the room, don't change anyone's statuses.
			}
			//they want to change the room's status to 'HelpOnTheWay' if they're needing help.			
			RoomControlClicked?.Invoke(this, e);
		}

		private void MapCubicle_MouseDown(object sender,MouseEventArgs e) {
			if(!AllowRightClick || PhoneCur==null) {
				return;//disable click options in setup window.
			}
			if(e==null || e.Button!=MouseButtons.Right) {
				if(e.Button==MouseButtons.Left && !IsFlashing() && Status!="OnWay") {
					if(_rectangleName.Contains(e.Location)) {
						PhoneUI.ShowEmployeeSettings(PhoneCur);
						return;
					}
					if(_rectanglePhone.Contains(e.Location) && PhoneCur.PatNum!=0) {
						ClickedGoTo?.Invoke(this,new EventArgs());
					}	
				}
				return;
			}
			bool allowStatusEdit=ClockEvents.IsClockedIn(EmployeeNum);
			if(EmployeeNum==Security.CurUser.EmployeeNum) {//can always edit yourself
				allowStatusEdit=true;
			}
			if(Status==Phones.ConvertClockStatusToString(ClockStatusEnum.NeedsHelp)) {
				//Always allow any employee to change any other employee from NeedsAssistance to Available
				allowStatusEdit=true;
			}
			string statusOnBehalfOf=EmployeeName;
			bool allowSetSelfAvailable=false;
			if(!ClockEvents.IsClockedIn(EmployeeNum) //No one is clocked in at this extension.
				&& !ClockEvents.IsClockedIn(Security.CurUser.EmployeeNum)) //This user is not clocked in either.
			{
				//Vacant extension and this user is not clocked in so allow this user to clock in at this extension.
				statusOnBehalfOf=Security.CurUser.UserName;
				allowSetSelfAvailable=true;
			}
			AddToolstripGroup("menuItemStatusOnBehalf","Status for: "+statusOnBehalfOf);
			AddToolstripGroup("menuItemRingGroupOnBehalf","Queues for ext: "+Extension.ToString());
			AddToolstripGroup("menuItemClockOnBehalf","Clock event for: "+EmployeeName);
			AddToolstripGroup("menuItemCustomer","Customer: "+PhoneCur.CustomerNumber);
			AddToolstripGroup("menuItemEmployee","Employee: "+EmployeeName);
			SetToolstripItemText("menuItemAvailable",allowStatusEdit || allowSetSelfAvailable);
			SetToolstripItemText("menuItemTraining",allowStatusEdit);
			SetToolstripItemText("menuItemTeamAssist",allowStatusEdit);
			SetToolstripItemText("menuItemNeedsHelp",allowStatusEdit);
			SetToolstripItemText("menuItemWrapUp",allowStatusEdit);
			SetToolstripItemText("menuItemOfflineAssist",allowStatusEdit);
			SetToolstripItemText("menuItemUnavailable",allowStatusEdit);
			SetToolstripItemText("menuItemTCResponder",allowStatusEdit);
			SetToolstripItemText("menuItemBackup",allowStatusEdit);
			SetToolstripItemText("menuItemLunch",allowStatusEdit);
			SetToolstripItemText("menuItemHome",allowStatusEdit);
			SetToolstripItemText("menuItemBreak",allowStatusEdit);
			menuItemGoTo.Enabled=true;
			if(PhoneCur.PatNum==0) {//disable go to if not a current patient
				menuItemGoTo.Enabled=false;
			}
			Point p=new Point(Location.X+e.Location.X,Location.Y+e.Location.Y);
			menuStatus.Show(Cursor.Position);
			Application.DoEvents();
			RoomControlClicked?.Invoke(this, e);
		}

		private void menuItemAvailable_Click(object sender,EventArgs e) {
			PhoneUI.Available(PhoneCur);
		}

		private void menuItemTraining_Click(object sender,EventArgs e) {
			PhoneUI.Training(PhoneCur);
		}

		private void menuItemTeamAssist_Click(object sender,EventArgs e) {
			PhoneUI.TeamAssist(PhoneCur);
		}

		private void menuItemTCResponder_Click(object sender,EventArgs e) {
			PhoneUI.TCResponder(PhoneCur);
		}

		private void menuItemNeedsHelp_Click(object sender,EventArgs e) {
			PhoneUI.NeedsHelp(PhoneCur);
		}

		private void menuItemWrapUp_Click(object sender,EventArgs e) {
			PhoneUI.WrapUp(PhoneCur);
		}

		private void menuItemOfflineAssist_Click(object sender,EventArgs e) {
			PhoneUI.OfflineAssist(PhoneCur);
		}

		private void menuItemUnavailable_Click(object sender,EventArgs e) {
			PhoneUI.Unavailable(PhoneCur);
		}

		private void menuItemBackup_Click(object sender,EventArgs e) {
			PhoneUI.Backup(PhoneCur);
		}

		private void menuItemRinggroupAll_Click(object sender,EventArgs e) {
			PhoneUI.QueueTech(PhoneCur);
		}

		private void menuItemRinggroupNone_Click(object sender,EventArgs e) {
			PhoneUI.QueueNone(PhoneCur);
		}

		private void menuItemRinggroupsDefault_Click(object sender,EventArgs e) {
			PhoneUI.QueueDefault(PhoneCur);
		}

		private void menuItemRinggroupBackup_Click(object sender,EventArgs e) {
			PhoneUI.QueueBackup(PhoneCur);
		}

		private void menuItemLunch_Click(object sender,EventArgs e) {
			PhoneUI.Lunch(PhoneCur);
		}

		private void menuItemHome_Click(object sender,EventArgs e) {
			PhoneUI.Home(PhoneCur);
		}

		private void menuItemBreak_Click(object sender,EventArgs e) {
			PhoneUI.Break(PhoneCur);
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			ClickedGoTo?.Invoke(this,new EventArgs());
		}

		private void menuItemEmployeeSettings_Click(object sender,EventArgs e) {
			PhoneUI.ShowEmployeeSettings(PhoneCur);
		}
		#endregion

		private void AddToolstripGroup(string groupName,string itemText) {
			ToolStripItem[] tsiFound=menuStatus.Items.Find(groupName,false);
			if(tsiFound==null || tsiFound.Length<=0) {
				return;
			}
			tsiFound[0].Text=itemText;
		}

		private void SetToolstripItemText(string toolStripItemName,bool isClockedIn) {
			ToolStripItem[] tsiFound=menuStatus.Items.Find(toolStripItemName,false);
			if(tsiFound==null || tsiFound.Length<=0) {
				return;
			}
			//set back to default
			tsiFound[0].Text=tsiFound[0].Text.Replace(" (Not Clocked In)","");
			if(isClockedIn) {
				tsiFound[0].Enabled=true;
			}
			else {
				tsiFound[0].Enabled=false;
				tsiFound[0].Text=tsiFound[0].Text+" (Not Clocked In)";
			}
		}


	}
}
