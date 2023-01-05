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
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.InternalTools.Phones{
	///<summary>This is the panel where all the cubicles are drawn.  For now, it is contained within a scrollable panel, but that might change once pan and zoom are added.</summary>
	public partial class MapPanel:Control{
		public List<MapArea> ListMapAreas=new List<MapArea>();
		///<summary>Always same length as ListMapAreas. 1:1 relationship.</summary>
		public List<MapAreaMore> ListMapAreaMores=new List<MapAreaMore>();

		///<summary>Same image used for webchat and chat.</summary>
		private Bitmap _bitmapChat;
		///<summary>15x17</summary>
		private Bitmap _bitmapProxFig;
		///<summary>21x17, red circle, mostly wasted extra pixels.</summary>
		private Bitmap _bitmapProxAway;
		private Bitmap _bitmapRemote;

		public MapPanel(){
			InitializeComponent();
			DoubleBuffered=true;
			_bitmapChat=Properties.Resources.WebChatIcon;
			_bitmapProxFig=Properties.Resources.Figure;
			_bitmapProxAway=Properties.Resources.NoFigure;//Red circle
			_bitmapRemote=Properties.Resources.remoteSupportIcon;
		}

		#region Methods - public
		public void SetListMapAreas(List<MapArea> listMapAreas){
			ListMapAreas=listMapAreas;
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
				mapAreaMore.Height=(float)ListMapAreas[i].Height*17f;
				mapAreaMore.RectangleFBounds=new RectangleF(mapAreaMore.X,mapAreaMore.Y,mapAreaMore.Width,mapAreaMore.Height);
				mapAreaMore.Width=(float)ListMapAreas[i].Width*17f;
				mapAreaMore.X=(float)ListMapAreas[i].XPos*17f;
				mapAreaMore.Y=(float)ListMapAreas[i].YPos*17f;
				ListMapAreaMores.Add(mapAreaMore);
			}
		}

		///<summary>Refresh the phone panel every X seconds after it has already been setup.  Make sure to call SetListMapAreas before calling this the first time.</summary>
		public void SetPhoneList(List<Phone> listPhones,List<ChatUser> listChatUsers,List<WebChatSession> listWebChatSessions, List<PeerInfo> listPeerInfosRemoteSupportSessions)
		{
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			for(int i=0;i<ListMapAreas.Count;i++){
				if(ListMapAreas[i].ItemType!=MapItemType.Cubicle){
					continue;//we're only updating cubicles, not labels
				}
				if(ListMapAreas[i].Extension==0) { //This cubicle has not been given an extension yet.
					ListMapAreaMores[i].IsEmpty=true;
					continue;
				}
				Phone phone=listPhones.Find(x => x.Extension==ListMapAreas[i].Extension);
				if(phone==null) {//We have a cubicle with no corresponding phone entry.
					ListMapAreaMores[i].IsEmpty=true;
					continue;
				}
				//ListMapAreaMores[i].Phone_=phone;//Refresh PhoneCur so that it has up to date customer information.
				ChatUser chatuser=listChatUsers.Find(x => x.Extension==phone.Extension);
				PhoneEmpDefault phoneEmpDefault=PhoneEmpDefaults.GetEmpDefaultFromList(phone.EmployeeNum,listPhoneEmpDefaults);
				if(phoneEmpDefault==null) {//We have a cubicle with no corresponding phone emp default entry.
					ListMapAreaMores[i].IsEmpty=true;
					continue;
				}
				//we got this far so we found a corresponding cubicle for this phone entry
				ListMapAreaMores[i].EmployeeNum=phone.EmployeeNum;
				ListMapAreaMores[i].EmployeeName=phone.EmployeeName;
			}
			Invalidate();
		}
		#endregion Methods - public

		#region Method - private OnPaint
		protected override void OnPaint(PaintEventArgs pe){
			//base.OnPaint(pe);
			Graphics g=pe.Graphics;
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.Clear(Color.White);
			g.DrawRectangle(Pens.Black,0,0,Width-1,Height-1);
			for(int i=0;i<ListMapAreas.Count;i++){
				if(ListMapAreas[i].ItemType==MapItemType.Label){
					DrawLabel(g,i);
					continue;
				}
				if(ListMapAreaMores[i].RectangleFBounds.Width<102) {
					DrawCubicleSmall(g,i);
				}
				else {
					DrawCubicleLarge(g,i);
				}
			}
		}
		#endregion Method - private OnPaint

		#region Methods - private
		///<summary>This is so that we can have a using statement in front of each StringFormat and it makes the code more readable.</summary>
		private StringFormat CreateStringFormat(StringAlignment stringAlignmentHoriz,StringAlignment stringAlignmentVert){
			StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.Alignment=stringAlignmentHoriz;
			stringFormat.LineAlignment=stringAlignmentVert;
			return stringFormat;
		}

		private void DrawCubicleLarge(Graphics g,int i){
			GraphicsState graphicsState = g.Save();
			g.TranslateTransform(ListMapAreaMores[i].X,ListMapAreaMores[i].Y);
			using Brush brushBack=new SolidBrush(ListMapAreaMores[i].ColorBack);
			//we leave a 1 pixel white border because it prevents cubicles from touching. which looks best.
			//The .5 pixel is because of how pixel alignment works in GDI+
			RectangleF rectangleF=new RectangleF(1.5f,1.5f,ListMapAreaMores[i].Width-4,ListMapAreaMores[i].Height-4);
			g.FillRectangle(brushBack,rectangleF);
			Pen penOutline=new Pen(ListMapAreaMores[i].ColorBorder,2);
			g.DrawRectangle(penOutline,rectangleF.X,rectangleF.Y,rectangleF.Width,rectangleF.Height);
			using StringFormat stringFormatCenter=CreateStringFormat(StringAlignment.Center,StringAlignment.Center);
			using Brush brushText=new SolidBrush(ListMapAreaMores[i].ColorFont);
			using Font font=new Font("Calibri",14,FontStyle.Bold);
			if(ListMapAreaMores[i].IsEmpty) { //empty room, so gray out and return
				rectangleF=new RectangleF(0,0,ListMapAreaMores[i].Width,ListMapAreaMores[i].Height);
				g.DrawString("EMPTY",font,brushText,rectangleF,stringFormatCenter);
				g.Restore(graphicsState);
				return;
			}
			//5 rows total
			float heightRow=ListMapAreaMores[i].Height/5f;
			//==================== row 1 - EMPLOYEE NAME ====================
			rectangleF=new RectangleF(0,7,ListMapAreaMores[i].Width,heightRow);
			using Font fontHeader=new Font("Calibri",19,FontStyle.Bold);
			FitText(ListMapAreaMores[i].EmployeeName,fontHeader,brushText,rectangleF,stringFormatCenter,g);
			float yPos=heightRow;
			//==================== row 2 - ELAPSED TIME ====================
			rectangleF=new RectangleF(0,yPos,ListMapAreaMores[i].Width,heightRow);
			FitText(ListMapAreaMores[i].TimeSpanElapsed.ToStringHmmss(),font,brushText,rectangleF,stringFormatCenter,g);
			yPos+=heightRow;
			//==================== row 3 - EMPLOYEE EXTENSION ====================
			//Display employee extension if they are present at their desk
			if(ListMapAreaMores[i].IsAtDesk) {
				rectangleF=new RectangleF(0,yPos,ListMapAreaMores[i].Width,heightRow);
				FitText("x"+ListMapAreas[i].Extension,font,brushText,rectangleF,stringFormatCenter,g);
			}
			yPos+=heightRow;
			//==================== row 4 - STATUS TEXT ====================
			rectangleF=new RectangleF(0,yPos,ListMapAreaMores[i].Width,heightRow);
			FitText(ListMapAreaMores[i].Status,Font,brushText,rectangleF,stringFormatCenter,g);
			yPos+=heightRow;
			//==================== row 5 (Left) - PROXIMITY ====================
			if(ListMapAreaMores[i].IsProx){
				g.DrawImage(_bitmapProxFig,x:5,y:yPos,width:_bitmapProxFig.Width,height:_bitmapProxFig.Height);
			}
			else if(ListMapAreaMores[i].IsProxAway) {
				g.DrawImage(_bitmapProxAway,x:5,y:yPos,width:_bitmapProxAway.Width,height:_bitmapProxAway.Height);
			}
			//==================== row 5 (Middle) - CHAT ICONS ====================
			if(ListMapAreaMores[i].IsChat) {//or webchat
				g.DrawImage(_bitmapChat,x:ListMapAreaMores[i].Width/2-8,y:yPos,width:_bitmapChat.Width,height:_bitmapChat.Height);
			}
			if(ListMapAreaMores[i].IsRemoteSupport) {
				g.DrawImage(_bitmapRemote,x:ListMapAreaMores[i].Width/2-8,y:yPos,width:_bitmapRemote.Width,height:_bitmapRemote.Height);
			}
			//==================== row 5 (Right) - PHONE ICON ====================
			/*
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
			}*/


			g.Restore(graphicsState);
		}

		private void DrawCubicleSmall(Graphics g,int i){
			GraphicsState graphicsState = g.Save();
			g.TranslateTransform(ListMapAreaMores[i].X,ListMapAreaMores[i].Y);
			using Brush brushBack=new SolidBrush(ListMapAreaMores[i].ColorBack);
			//we leave a 1 pixel white border because it prevents cubicles from touching.
			//The .5 pixel is because of how pixel alignment works in GDI+
			RectangleF rectangleF=new RectangleF(1.5f,1.5f,ListMapAreaMores[i].Width-4,ListMapAreaMores[i].Height-4);
			g.FillRectangle(brushBack,rectangleF);
			Pen penOutline=new Pen(ListMapAreaMores[i].ColorBorder,2);
			//penOutline.Alignment=PenAlignment.Inset;
			g.DrawRectangle(penOutline,rectangleF.X,rectangleF.Y,rectangleF.Width,rectangleF.Height);
			using StringFormat stringFormatCenter=CreateStringFormat(StringAlignment.Center,StringAlignment.Center);
			using Brush brushText=new SolidBrush(ListMapAreaMores[i].ColorFont);
			using Font font=new Font("Calibri",14,FontStyle.Bold);
			if(ListMapAreaMores[i].IsEmpty) { //empty room, so gray out and return
				rectangleF=new RectangleF(0,0,ListMapAreaMores[i].Width,ListMapAreaMores[i].Height);
				g.DrawString("EMPTY",font,brushText,rectangleF,stringFormatCenter);
				g.Restore(graphicsState);
				return;
			}
			//3 rows total
			float heightRow=ListMapAreaMores[i].Height/3f;
			//==================== row 1 - EMPLOYEE NAME ====================
			rectangleF=new RectangleF(0,4,ListMapAreaMores[i].Width,heightRow);
			using Font fontHeader=new Font("Calibri",19,FontStyle.Bold);
			FitText(ListMapAreaMores[i].EmployeeName,fontHeader,brushText,rectangleF,stringFormatCenter,g);
			float yPos=heightRow;
			//==================== row 2 - ELAPSED TIME ====================
			rectangleF=new RectangleF(0,yPos,ListMapAreaMores[i].Width,heightRow);
			FitText(ListMapAreaMores[i].TimeSpanElapsed.ToStringHmmss(),font,brushText,rectangleF,stringFormatCenter,g);
			//g.DrawRectangle(Pens.Red,rcOuter.X,rcOuter.Y+yPosBottom,rcOuter.Width,rowHeight);
			yPos+=heightRow;



			g.Restore(graphicsState);
		}

		private void DrawLabel(Graphics g,int i){
			//The label Width and Height are largely ignored.
			//Make the label just tall enough to fit the font and the lesser of the user defined width vs the actual width.
			float widthMax=ListMapAreaMores[i].RectangleFBounds.Width;
			using Font font=new Font("Calibri",14,FontStyle.Bold);
			SizeF sizeF=g.MeasureString(ListMapAreas[i].Description,font,(int)widthMax);
			using Brush brushBack=new SolidBrush(ListMapAreaMores[i].ColorBack);
			RectangleF rectangleF=new RectangleF(ListMapAreaMores[i].X,ListMapAreaMores[i].Y,sizeF.Width,font.Height-2);
			g.FillRectangle(brushBack,rectangleF);
			using Brush brushFont=new SolidBrush(ListMapAreaMores[i].ColorFont);
			g.DrawString(ListMapAreas[i].Description,font,brushFont,ListMapAreaMores[i].X,ListMapAreaMores[i].Y);

		}

		///<summary>Replaces Graphics.DrawString. If the text is wider than will fit, then this reduces its size.  It does not consider height.</summary>
		private static void FitText(string text,Font font,Brush brush,RectangleF rectangleF,StringFormat stringFormat,Graphics g) {
			float emSize=font.Size;
			Size size=TextRenderer.MeasureText(text,font);
			if(size.Width>=rectangleF.Width) {
				emSize=emSize*(rectangleF.Width/size.Width);//get the ratio of the room width to font width and multiply that by the font size
				if(emSize<2) {//don't let the font be smaller than 2 point font
					emSize=2F;
				}
			}
			using Font fontNew=new Font(font.FontFamily,emSize,font.Style);
			g.DrawString(text,fontNew,brush,rectangleF,stringFormat);
		}
		#endregion Methods - private
	}

	///<summary>This class stores additional info about each MapArea, such as colors and timespans.</summary>
	public class MapAreaMore{
		public Color ColorBorder;
		public Color ColorBack;
		public Color ColorFont;
		public string EmployeeName;
		public long EmployeeNum;
		///<summary>Already converted from feet to pixels.</summary>
		public float Height;
		public bool IsAtDesk;
		///<summary>Includes both chat and webchat.</summary>
		public bool IsChat;
		public bool IsEmpty;
		public bool IsProx;
		public bool IsProxAway;
		public bool IsRemoteSupport;
		///<summary>Already converted from feet to pixels.</summary>
		public RectangleF RectangleFBounds;
		public string Status;
		public TimeSpan TimeSpanElapsed;
		///<summary>Already converted from feet to pixels.</summary>
		public float Width;
		///<summary>Already converted from feet to pixels.</summary>
		public float X;
		///<summary>Already converted from feet to pixels.</summary>
		public float Y;

		public override string ToString() {
			return EmployeeName;
		}
	}
}
