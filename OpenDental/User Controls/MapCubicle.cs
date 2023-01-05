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
		private RectangleF _rectangleFName;
		/// <summary>Area of the cubicle where the phone icon is drawn.</summary>
		private RectangleF _rectangleFPhone;

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
		public TimeSpan TimeSpanElapsed { get; set; }

		[Category("OD")]
		[Description("Current Employee Status")]
		public string Status { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current Phone Status")]
		public Image ImagePhone { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current Phone Status")]
		public Image ImageChat { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current WebChat Status")]
		public Image ImageWebChat { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current Proximity Status")]
		public Image ImageProx { get; set; }

		[Category("OD")]
		[Description("Image Indicating Employee's Current Remote Support Status")]
		public Image ImageRemoteSupport { get; set; }

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
		private Color _colorInnerRestore=Color.FromArgb(128,Color.Red);
		private Color colorDefaultOuter=Color.Red;
		[Category("OD")]
		[Description("Exterior Border Color")]
		public Color ColorOuter {
			get {
				return colorDefaultOuter;
			}
			set {
				colorDefaultOuter=value;
				Invalidate();
			}
		}

		///<summary>Set when flashing starts so we know what outer color to go back to.</summary>
		private Color _colorOuterRestore=Color.Red;
		private Color _colorDefaultInner=Color.FromArgb(128,Color.Red);
		[Category("OD")]
		[Description("Interior Fill Color")]
		public Color ColorInner {
			get {
				return _colorDefaultInner;
			}
			set {
				_colorDefaultInner=value;
				Invalidate();
			}
		}

		private bool _isEmpty=false;
		[Category("OD")]
		[Description("No Extension Assigned")]
		public bool IsEmpty {
			get {
				return _isEmpty;
			}
			set {
				_isEmpty=value;
				Invalidate();
			}
		}

		private bool _isEditAllowed=false;
		[Category("OD")]
		[Description("Double-click will open editor")]
		public bool IsEditAllowed {
			get {
				return _isEditAllowed;
			}
			set {
				_isEditAllowed=value;
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
			_colorOuterRestore=ColorOuter;
			_colorInnerRestore=ColorInner;
			timerFlash.Start();
		}

		public void StopFlashing() {
			if(!IsFlashing()) { //already off
				return;
			}
			timerFlash.Stop();
			ColorOuter=_colorOuterRestore;
			ColorInner=_colorInnerRestore;
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

		public void SetColors(Color colorFore,Color colorOuter,Color colorInner) {
			ForeColor=colorFore;
			ColorOuter=colorOuter;
			ColorInner=colorInner;
		}

		private void timerFlash_Tick(object sender,EventArgs e) {
			//flip inner and outer colors
			if(ColorOuter==_colorOuterRestore) {
				ColorOuter=_colorInnerRestore;
				ColorInner=_colorOuterRestore;
			}
			else {
				ColorOuter=_colorOuterRestore;
				ColorInner=_colorInnerRestore;
			}
		}

		private void mapCubicle_Paint(object sender,PaintEventArgs e) {
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
			Color color=ColorInner;
			if(IsEmpty) {
				color=Color.FromArgb(20,Color.Gray);
			}
			using Brush brushInner=new SolidBrush(color);
			color=ForeColor;
			if(IsEmpty) {
				color=Color.FromArgb(128,Color.Gray);
			}
			using Brush brushText=new SolidBrush(color);
			color=ColorOuter;
			if(IsEmpty) {
				color=Color.FromArgb(128,Color.Gray);
			}
			using Pen penOuter=new Pen(color,BorderThickness);
			_rectangleFPhone=RectangleF.Empty;
			_rectangleFName=RectangleF.Empty;
			string timeElapsed=TimeSpanToStringHelper(TimeSpanElapsed);
			RectangleF rectangleFOuter=this.ClientRectangle;
			//clear control canvas
			g.Clear(this.BackColor);
			float halfPenThickness=BorderThickness/(float)2;
			//deflate for border
			rectangleFOuter.Inflate(-halfPenThickness,-halfPenThickness);
			//draw border
			g.DrawRectangle(penOuter,rectangleFOuter.X,rectangleFOuter.Y,rectangleFOuter.Width,rectangleFOuter.Height);
			//deflate to drawable region
			rectangleFOuter.Inflate(-halfPenThickness,-halfPenThickness);
			//fill interior
			g.FillRectangle(brushInner,rectangleFOuter);
			StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.Alignment=StringAlignment.Center;
			stringFormat.LineAlignment=StringAlignment.Center;
			if(this.IsEmpty) { //empty room so gray out and return
				g.DrawString("EMPTY",Font,brushText,rectangleFOuter,stringFormat);
				return;
			}
			else if(Text!="") { //using as a label so just draw the string					
				FitText(Text,Font,brushText,new RectangleF(rectangleFOuter.Left,rectangleFOuter.Top+2,rectangleFOuter.Width,rectangleFOuter.Height),stringFormat,g);
				return;
			}
			//3 rows of data
			int rowsLowestCommonDenominator=6;
			float typicalRowHeight=rectangleFOuter.Height/(float)rowsLowestCommonDenominator;
			//==================== row 1 - EMPLOYEE NAME ====================
			float rowHeight=typicalRowHeight*2; //row 1 is 2/6 tall
			_rectangleFName=new RectangleF(rectangleFOuter.X,rectangleFOuter.Y-2,rectangleFOuter.Width,rowHeight);
			FitText(EmployeeName,FontHeader,brushText,_rectangleFName,stringFormat,g);
			float yPosBottom=rowHeight;
			//g.DrawRectangle(Pens.LimeGreen,rcOuter.X,rcOuter.Y,rcOuter.Width,rowHeight);
			//==================== row 2 - ELAPSED TIME ====================
			rowHeight=typicalRowHeight*2; //row 2 is 2/6 tall
			FitText(timeElapsed,Font,brushText,new RectangleF(rectangleFOuter.X,rectangleFOuter.Y+yPosBottom-14,rectangleFOuter.Width,rowHeight),stringFormat,g);
			//g.DrawRectangle(Pens.Red,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
			yPosBottom+=rowHeight;
			//==================== row 3 (Middle) - EMPLOYEE EXTENSION ====================
			//Display employee extension if they are present at their desk
			if(IsAtDesk) {
				FitText("x"+Extension,Font,brushText,new RectangleF(rectangleFOuter.X,rectangleFOuter.Y+yPosBottom-30,rectangleFOuter.Width,rowHeight),stringFormat,g);
			}
			//==================== row 4 (Bottom) - EMPLOYEE STATUS ====================
			//left-most 3/4 of row 3 is the status text
			FitText(Status,Font,brushText,new RectangleF(rectangleFOuter.X+(rectangleFOuter.Width/6)-5,rectangleFOuter.Y+yPosBottom-14,((rectangleFOuter.Width/6)*4)+4,rowHeight),stringFormat,g);
			//FitText(Status,Font,brushText,new RectangleF(rcOuter.X+(rcOuter.Width/6)-2,rcOuter.Y+yPosBottom+1,((rcOuter.Width/6)*4)+4,rowHeight),stringFormat,g);
			//==================== row 5 (Left) - PROXIMITY STATUS ====================
			int iconShiftCenter = 8;
			rowHeight =typicalRowHeight*2; //row 3 is 2/6 tall
			if(ImageProx!=null) {
				//right-most 1/4 of row 3 is the phone icon
				RectangleF rectangleF = new RectangleF(rectangleFOuter.X-2+iconShiftCenter,rectangleFOuter.Y+yPosBottom+4,ImageProx.Width,rowHeight);
				//Scale the image.
				if(ImageProx.Height<rectangleF.Height) {
					rectangleF.Y+=(rectangleF.Height-ImageProx.Height)/2;
					rectangleF.Height=ImageProx.Height;
				}
				if(ImageProx.Width<rectangleF.Width) {
					rectangleF.X-=(rectangleF.Width-ImageProx.Width)/2;
					rectangleF.Width=ImageProx.Width;
				}
				g.DrawImage(
					ImageProx,
					rectangleF,
					new RectangleF(0,0,ImageProx.Width,ImageProx.Height),
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
			if(ImagePhone!=null) {
				//right-most 1/4 of row 3 is the phone icon
				RectangleF rect=new RectangleF((rectangleFOuter.X+(rectangleFOuter.Width/6)*5)-BorderThickness-2-iconShiftCenter,rectangleFOuter.Y+yPosBottom+4,ImagePhone.Width,rowHeight);
				//Scale the image.
				if(ImagePhone.Height<rect.Height) {
					rect.Y+=(rect.Height-ImagePhone.Height)/2;
					rect.Height=ImagePhone.Height;
				}
				if(ImagePhone.Width<rect.Width) {
					rect.X-=(rect.Width-ImagePhone.Width)/2;
					rect.Width=ImagePhone.Width;
				}
				_rectangleFPhone=rect;
				g.DrawImage(
					ImagePhone,
					_rectangleFPhone,
					new RectangleF(0,0,ImagePhone.Width,ImagePhone.Height),
					GraphicsUnit.Pixel);
					//g.DrawRectangle(Pens.Orange,rectImage.X,rectImage.Y,rectImage.Width,rectImage.Height);
			}
			//Show a chat icon when the employee is not on the phone.
			//==================== row 5 (middle) - WEB CHAT ICON ====================
			else if(ImageWebChat!=null) {
				DisplayChatImage(ImageWebChat,e,rectangleFOuter,yPosBottom,rowHeight);
			}
			//==================== row 5 (middle) - CHAT ICON ====================
			else if(ImageChat!=null) {
				DisplayChatImage(ImageChat,e,rectangleFOuter,yPosBottom,rowHeight);
			}
			//==================== row 5 (middle) - REMOTE SUPPORT ICON ====================
			else if(ImageRemoteSupport!=null) {
				DisplayChatImage(ImageRemoteSupport,e,rectangleFOuter,yPosBottom,rowHeight);
			}
			//g.DrawRectangle(Pens.Blue,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
			yPosBottom+=rowHeight;
		}

		///<summary>Draws a modified cube to account for less workable area.</summary>
		private void PaintSmall(PaintEventArgs e) {
			Graphics g=e.Graphics;//alias
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			Color color=ColorInner;
			if(IsEmpty) {
				color=Color.FromArgb(20,Color.Gray);
			}
			using Brush brushInner=new SolidBrush(color);
			color=ForeColor;
			if(IsEmpty) {
				color=Color.FromArgb(128,Color.Gray);
			}
			using Brush brushText=new SolidBrush(color);
			float halfPenThickness=BorderThickness/(float)2;
			color=ColorOuter;
			if(IsEmpty) {
				color=Color.FromArgb(128,Color.Gray);
			}
			using Pen penOuter=new Pen(color,halfPenThickness);
			_rectangleFPhone=RectangleF.Empty;
			_rectangleFName=RectangleF.Empty;
			string timeElapsed=TimeSpanToStringHelper(TimeSpanElapsed,true);
			if(EmployeeName!=null && EmployeeName.Length>8){
				EmployeeName=EmployeeName.Substring(0,8);
			}
			RectangleF rectangleFOuter=this.ClientRectangle;
			//clear control canvas
			g.Clear(this.BackColor);
			//deflate for border
			rectangleFOuter.Inflate(-halfPenThickness,-halfPenThickness);
			//draw border
			g.DrawRectangle(penOuter,rectangleFOuter.X,rectangleFOuter.Y,rectangleFOuter.Width,rectangleFOuter.Height);
			//deflate to drawable region
			rectangleFOuter.Inflate(-halfPenThickness,-halfPenThickness);
			//fill interior
			g.FillRectangle(brushInner,rectangleFOuter);
			StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.Alignment=StringAlignment.Center;
			stringFormat.LineAlignment=StringAlignment.Center;
			if(this.IsEmpty) { //empty room so gray out and return
				g.DrawString("EMPTY",Font,brushText,rectangleFOuter,stringFormat);
				return;
			}
			else if(this.Text!="") { //using as a label so just draw the string
				FitText(this.Text,Font,brushText,new RectangleF(rectangleFOuter.Left,rectangleFOuter.Top+2,rectangleFOuter.Width,rectangleFOuter.Height),stringFormat,g);
				return;
			}
			//3 rows of data
			int rowsLowestCommonDenominator=6;
			float typicalRowHeight=rectangleFOuter.Height/(float)rowsLowestCommonDenominator;
			//==================== row 1 - EMPLOYEE NAME ====================
			float rowHeight=typicalRowHeight*2; //row 1 is 2/6 tall
			_rectangleFName=new RectangleF(rectangleFOuter.X,rectangleFOuter.Y,rectangleFOuter.Width,rowHeight);
			FitText(EmployeeName,FontHeader,brushText,_rectangleFName,stringFormat,g);
			float yPosBottom=rowHeight;
			//g.DrawRectangle(Pens.LimeGreen,rcOuter.X,rcOuter.Y,rcOuter.Width,rowHeight);
			//==================== row 2 - ELAPSED TIME ====================
			rowHeight=typicalRowHeight*2;//row 2 is 2/6 tall
			FitText(timeElapsed,Font,brushText,new RectangleF(rectangleFOuter.X,rectangleFOuter.Y+rowHeight,rectangleFOuter.Width,rowHeight),stringFormat,g);
			//g.DrawRectangle(Pens.Red,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
			//==================== row 3 - Prox ====================
			yPosBottom+=rowHeight;
			int smallMargin=1;
			rowHeight=typicalRowHeight*2;//row 3 is 2/6 tall
			if(ImageProx!=null) {
				//right-most 1/4 of row 3 is the phone icon
				RectangleF rectangleFImage=new RectangleF(rectangleFOuter.X,rectangleFOuter.Y+rowHeight*2-smallMargin,ImageProx.Width,rowHeight);
				//Scale the image.
				if(ImageProx.Height<rectangleFImage.Height || ImageProx.Width<rectangleFImage.Width) {
					rectangleFImage.Y+=(rectangleFImage.Height-ImageProx.Height)/2;
					rectangleFImage.X+=(rectangleFImage.Width-ImageProx.Width)/2;
					rectangleFImage.Height=ImageProx.Height;
					rectangleFImage.Width=ImageProx.Width;
				}
				g.DrawImage(
					ImageProx,
					rectangleFImage,
					new RectangleF(0,0,ImageProx.Width,ImageProx.Height),
					GraphicsUnit.Pixel);
				//g.DrawRectangle(Pens.Orange,rectImage.X,rectImage.Y,rectImage.Width,rectImage.Height);
			}
			//Only show the Phone icon when employee is on the phone. Do not show a chat icon along with the phone icon.
			//==================== row 3 (right) - PHONE ICON ====================
			if(ImagePhone!=null) {
				Size sizeNew=new Size((int)(ImagePhone.Width/1.2),(int)(ImagePhone.Height/1.2));
				RectangleF rectangleFImage=new RectangleF(((rectangleFOuter.Width+BorderThickness)-sizeNew.Width-smallMargin-2),rectangleFOuter.Y+yPosBottom-smallMargin,sizeNew.Width+3,rowHeight);
				//Scale the image.
				if(sizeNew.Height<rectangleFImage.Height || sizeNew.Width<=rectangleFImage.Width) {
					rectangleFImage.Y+=(rectangleFImage.Height-sizeNew.Height)/2;
					rectangleFImage.X+=(rectangleFImage.Width-sizeNew.Width)/2;
					rectangleFImage.Height=sizeNew.Height;
					rectangleFImage.Width=sizeNew.Width;
				}
				_rectangleFPhone=rectangleFImage;
				g.DrawImage(
					ImagePhone,
					_rectangleFPhone,
					new RectangleF(0,0,sizeNew.Width+3,sizeNew.Height),
					GraphicsUnit.Pixel);
				if(ODBuild.IsDebug()) {
					//Uncomment if you need to see the boundaries of the rectangles being drawn.
					//g.DrawRectangle(Pens.Orange,rectImage.X,rectImage.Y,rectImage.Width,rectImage.Height);
				}
			}
			//Show a chat icon when the employee is not on the phone.
			//==================== row 3 (middle) - WEB CHAT ICON ====================
			else if(ImageWebChat!=null) {
				DisplayChatImage(ImageWebChat,e,rectangleFOuter,yPosBottom-BorderThickness,rowHeight);
			}
			//==================== row 3 (middle) - CHAT ICON ====================
			else if(ImageChat!=null) {
				DisplayChatImage(ImageChat,e,rectangleFOuter,yPosBottom-BorderThickness,rowHeight);
			}
			//==================== row 3 (middle) - REMOTE SUPPORT ICON ====================
			else if(ImageRemoteSupport!=null) {
				DisplayChatImage(ImageRemoteSupport,e,rectangleFOuter,yPosBottom-BorderThickness,rowHeight);
			}
			if(ODBuild.IsDebug()) {
				//Uncomment if you need to see the boundaries of the rectangles being drawn.
				//g.DrawRectangle(Pens.Blue,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
			}
			yPosBottom+=rowHeight;
		}

		///<summary>Converts a TimeSpan to a string depending on the size of the room and how much time has elapsed.</summary>
		private static string TimeSpanToStringHelper(TimeSpan span,bool isSmallCube=false) {
			//Build the smaller time string and if it's over an hour, just display minutes.
			if(isSmallCube && span.Hours>0) { 
				return (span.Hours+"hr "+span.Minutes).ToString();
			}
			return span.ToStringHmmss();
		}

		private static void DisplayChatImage(Image img,PaintEventArgs e,RectangleF rcOuter,float yPosBottom,float rowHeight) {
			//right-most 1/4 of row 3 is the phone icon
			RectangleF rectangleFImage=new RectangleF((rcOuter.X+(rcOuter.Width/2))-8,rcOuter.Y+yPosBottom+4,img.Width,rowHeight);
			//Scale the image.
			if(img.Height<rectangleFImage.Height || img.Width<rectangleFImage.Width) {
				rectangleFImage.Y+=(rectangleFImage.Height-img.Height)/2;
				rectangleFImage.Height=img.Height;
				rectangleFImage.X-=(rectangleFImage.Width-img.Width)/2;
				rectangleFImage.Width=img.Width;
			}
			e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
			e.Graphics.DrawImage(
				img,
				rectangleFImage,
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
			if(!IsEditAllowed) {
				return;
			}
			//edit this room
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaItem=this.MapAreaCur;
			if(formMapAreaEdit.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			if(MapCubicleEdited!=null) { //let anyone interested know that this cubicle was edited
				MapCubicleEdited(this,new EventArgs());
			}
		}

		private void MapCubicle_Click(object sender,EventArgs e) {
			if(IsEditAllowed) {
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
					if(_rectangleFName.Contains(e.Location)) {
						PhoneUI.ShowEmployeeSettings(PhoneCur);
						return;
					}
					if(_rectangleFPhone.Contains(e.Location) && PhoneCur.PatNum!=0) {
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
			ToolStripItem[] toolStripItemsArrayFound=menuStatus.Items.Find(groupName,false);
			if(toolStripItemsArrayFound.IsNullOrEmpty()) {
				return;
			}
			toolStripItemsArrayFound[0].Text=itemText;
		}

		private void SetToolstripItemText(string toolStripItemName,bool isClockedIn) {
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
	}
}
