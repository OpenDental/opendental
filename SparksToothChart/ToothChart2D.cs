using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace SparksToothChart {
	public partial class ToothChart2D:UserControl {
		///<summary>This is a reference to the TcData object that's at the wrapper level.</summary>
		public ToothChartData TcData;
		private bool MouseIsDown;
		///<summary>Mouse move causes this variable to be updated with the current tooth that the mouse is hovering over.</summary>
		private string hotTooth;
		///<summary>The previous hotTooth.  If this is different than hotTooth, then mouse has just now moved to a new tooth.  Can be 0 to represent no previous.</summary>
		private string hotToothOld;
		///<summary></summary>
		[Category("Action"),Description("Occurs when the mouse goes up ending a drawing segment.")]
		public event ToothChartDrawEventHandler SegmentDrawn=null;
		///<summary></summary>
		[Category("Action"),Description("Occurs when the mouse goes up committing tooth selection.")]
		public event ToothChartSelectionEventHandler ToothSelectionsChanged=null;
		///<summary>GDI+ handle to this control. Used for line drawing and font measurement.</summary>
		private Graphics g=null;
		private List<string> _listSelectedTeethOld=new List<string>();

		public ToothChart2D() {
			InitializeComponent();
		}

		public void InitializeGraphics() {
			g=this.CreateGraphics();
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(DesignMode) {
				e.Graphics.DrawImage(pictBox.Image,new Rectangle(0,0,this.Width,this.Height));
				return;
			}
			//our strategy here will be to draw on a new bitmap.
			Bitmap bitmap=new Bitmap(Width,Height);
			Graphics g=Graphics.FromImage(bitmap);
			g.Clear(TcData.ColorBackground);
			//draw a copy of the tooth chart background
			g.DrawImage(pictBox.Image,TcData.RectTarget);
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
			for(int t=0;t<TcData.ListToothGraphics.Count;t++) {//loop through each tooth
				if(TcData.ListToothGraphics[t].ToothID=="implant") {//this is not an actual tooth.
					continue;
				}
				DrawFacialView(TcData.ListToothGraphics[t],g);
				DrawOcclusalView(TcData.ListToothGraphics[t],g);
			}
			DrawWatches(g);
			DrawNumbers(g);
			DrawDrawingSegments(g);
			e.Graphics.DrawImage(bitmap,0,0);
			g.Dispose();
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
			//base.OnPaintBackground(e);//don't draw background
		}

		///<summary></summary>
		private void DrawFacialView(ToothGraphic toothGraphic,Graphics g) {
			/*
			if(toothGraphic.Visible
				|| (toothGraphic.IsCrown && toothGraphic.IsImplant)
				|| toothGraphic.IsPontic) {
				//DrawTooth(toothGraphic,g);
			}
			float w=0;
			if(!Tooth.IsPrimary(toothGraphic.ToothID)) {
				w=ToothGraphic.GetWidth(toothGraphic.ToothID)/TcData.ScaleMmToPix;
					// /TcData.WidthProjection*(float)Width;
			}
			if(!Tooth.IsPrimary(toothGraphic.ToothID) && (!toothGraphic.Visible || toothGraphic.IsPontic)) {
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
					//g.FillRectangle(new SolidBrush(colorBackSimple),x-w/2f,0,w,Height/2f-20);
				}
				else {
					//g.FillRectangle(new SolidBrush(colorBackSimple),x-w/2f,Height/2f+20,w,Height/2f-20);
				}
			}*/
			if(toothGraphic.DrawBigX) {
				float x=TcData.GetTransXpix(toothGraphic.ToothID);
				float y=TcData.GetTransYfacialPix(toothGraphic.ToothID);
				float halfw=6f*TcData.PixelScaleRatio;
				float halfh=29f*TcData.PixelScaleRatio;//58;
				//float offsety=73;
				//toothGraphic.colorX
				//if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
					//g.DrawLine(new Pen(toothGraphic.colorX),x-halfxwidth,Height/2f-offsetofx-xheight,x+halfxwidth,Height/2f-offsetofx);
					//g.DrawLine(new Pen(toothGraphic.colorX),x+halfxwidth,Height/2f-offsetofx-xheight,x-halfxwidth,Height/2f-offsetofx);
					g.DrawLine(new Pen(toothGraphic.colorX,2f*TcData.PixelScaleRatio),x-halfw,y-halfh,x+halfw,y+halfh);
					g.DrawLine(new Pen(toothGraphic.colorX,2f*TcData.PixelScaleRatio),x+halfw,y-halfh,x-halfw,y+halfh);
				//}
				//else {//Mandible
					//g.DrawLine(new Pen(toothGraphic.colorX),x-halfxwidth,Height/2f+offsetofx+xheight,x+halfxwidth,Height/2f+offsetofx);
					//g.DrawLine(new Pen(toothGraphic.colorX),x+halfxwidth,Height/2f+offsetofx+xheight,x-halfxwidth,Height/2f+offsetofx);
				//	g.DrawLine(new Pen(toothGraphic.colorX),x-halfw,y+halfh,x+halfw,y);
				//	g.DrawLine(new Pen(toothGraphic.colorX),x+halfw,y+halfh,x-halfw,y);
				//}
			}
			//if(toothGraphic.Visible && toothGraphic.IsRCT) {//draw RCT
				//x=,y= etc
				//toothGraphic.colorRCT
				//?
			//}
			//if(toothGraphic.Visible && toothGraphic.IsBU) {//BU or Post
				//?
			//}
			//if(toothGraphic.IsImplant) {
				//?
			//}
		}

		private void DrawOcclusalView(ToothGraphic toothGraphic,Graphics g) {
			//now the occlusal surface. Absolute pixels instead of mm relative to center.
			float x,y;
			x=TcData.GetTransXpix(toothGraphic.ToothID);
			y=TcData.GetTransYocclusalPix(toothGraphic.ToothID);
			if(toothGraphic.Visible//might not be visible if an implant
				|| (toothGraphic.IsCrown && toothGraphic.IsImplant)//a crown on an implant will paint
			//pontics won't paint, because tooth is invisible
				//but, unlike the regular toothchart, we do want pontics to paint here
				|| toothGraphic.IsPontic) {
				DrawToothOcclusal(toothGraphic,g);
			}
			if(toothGraphic.Visible && 
				toothGraphic.IsSealant) {//draw sealant
				//?
			}
		}

		///<summary></summary>
		private void DrawToothOcclusal(ToothGraphic toothGraphic,Graphics g) {
			ToothGroup group;
			float x,y;
			Pen outline=new Pen(Color.Gray);
			for(int i=0;i<toothGraphic.Groups.Count;i++) {
				group=(ToothGroup)toothGraphic.Groups[i];
				if(!group.Visible) {
					continue;
				}
				x=TcData.GetTransXpix(toothGraphic.ToothID);
				y=TcData.GetTransYocclusalPix(toothGraphic.ToothID);
				float sqB=4 * TcData.PixelScaleRatio;//half the size of the central square. B for Big.
				float cirB=9.5f * TcData.PixelScaleRatio;//radius of outer circle
				float sqS=3 * TcData.PixelScaleRatio;//S for small
				float cirS=8f * TcData.PixelScaleRatio;
				GraphicsPath path;
				SolidBrush brush=new SolidBrush(group.PaintColor);
				string dir;
				switch(group.GroupType) {
					case ToothGroupType.O:
						g.FillRectangle(brush,x-sqB,y-sqB,2f*sqB,2f*sqB);
						g.DrawRectangle(outline,x-sqB,y-sqB,2f*sqB,2f*sqB);
						break;
					case ToothGroupType.I:
						g.FillRectangle(brush,x-sqS,y-sqS,2f*sqS,2f*sqS);
						g.DrawRectangle(outline,x-sqS,y-sqS,2f*sqS,2f*sqS);
						break;
					case ToothGroupType.B:
						if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
							path=GetPath("U",x,y,sqB,cirB);
						}
						else {
							path=GetPath("D",x,y,sqB,cirB);
						}
						g.FillPath(brush,path);
						g.DrawPath(outline,path);
						break;
					case ToothGroupType.F:
						if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
							path=GetPath("U",x,y,sqS,cirS);
						}
						else {
							path=GetPath("D",x,y,sqS,cirS);
						}
						g.FillPath(brush,path);
						g.DrawPath(outline,path);
						break;
					case ToothGroupType.L:
						if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)) {
							dir="D";
						}
						else {
							dir="U";
						}
						if(ToothGraphic.IsAnterior(toothGraphic.ToothID)) {
							path=GetPath(dir,x,y,sqS,cirS);
						}
						else {
							path=GetPath(dir,x,y,sqB,cirB);
						}
						g.FillPath(brush,path);
						g.DrawPath(outline,path);
						break;
					case ToothGroupType.M:
						if(ToothGraphic.IsRight(toothGraphic.ToothID)) {
							dir="R";
						}
						else {
							dir="L";
						}
						if(ToothGraphic.IsAnterior(toothGraphic.ToothID)) {
							path=GetPath(dir,x,y,sqS,cirS);
						}
						else {
							path=GetPath(dir,x,y,sqB,cirB);
						}
						g.FillPath(brush,path);
						g.DrawPath(outline,path);
						break;
					case ToothGroupType.D:
						if(ToothGraphic.IsRight(toothGraphic.ToothID)) {
							dir="L";
						}
						else {
							dir="R";
						}
						if(ToothGraphic.IsAnterior(toothGraphic.ToothID)) {
							path=GetPath(dir,x,y,sqS,cirS);
						}
						else {
							path=GetPath(dir,x,y,sqB,cirB);
						}
						g.FillPath(brush,path);
						g.DrawPath(outline,path);
						break;
				}
				//group.PaintColor
				//Gl.glCallList(displayListOffset+toothGraphic.GetIndexForDisplayList(group));
			}
		}

		///<summary>Gets a path for the pie shape that represents a tooth surface.  sq and cir refer to the radius of those two elements.</summary>
		private GraphicsPath GetPath(string UDLR,float x,float y,float sq,float cir) {
			GraphicsPath path=new GraphicsPath();
			float pt=cir*0.7071f;//the x or y dist to the point where the circle is at 45 degrees.
			switch(UDLR) {
				case "U":
					path.AddLine(x-sq,y-sq,x+sq,y-sq);
					path.AddLine(x+sq,y-sq,x+pt,y-pt);
					path.AddArc(x-cir,y-cir,cir*2f,cir*2f,360-45,-90);
					path.AddLine(x-pt,y-pt,x-sq,y-sq);
					break;
				case "D":
					path.AddLine(x+sq,y+sq,x-sq,y+sq);
					path.AddLine(x-sq,y+sq,x-pt,y+pt);
					path.AddArc(x-cir,y-cir,cir*2f,cir*2f,90+45,-90);
					path.AddLine(x+pt,y+pt,x+sq,y+sq);
					break;
				case "L":
					path.AddLine(x-sq,y+sq,x-sq,y-sq);
					path.AddLine(x-sq,y-sq,x-pt,y-pt);
					path.AddArc(x-cir,y-cir,cir*2f,cir*2f,180+45,-90);
					path.AddLine(x-pt,y+pt,x-sq,y+sq);
					break;
				case "R":
					path.AddLine(x+sq,y-sq,x+sq,y+sq);
					path.AddLine(x+sq,y+sq,x+pt,y+pt);
					path.AddArc(x-cir,y-cir,cir*2f,cir*2f,45,-90);
					path.AddLine(x+pt,y-pt,x+sq,y-sq);
					break;
			}
			return path;
		}

		private void DrawWatches(Graphics g){
			Hashtable watchTeeth=new Hashtable(TcData.ListToothGraphics.Count);
			for(int t=0;t<TcData.ListToothGraphics.Count;t++) {//loop through each adult tooth
			  ToothGraphic toothGraphic=TcData.ListToothGraphics[t];
				//If a tooth is marked to be watched then it is always visible, even if the tooth is missing/hidden.
				if(toothGraphic.ToothID=="implant" || !toothGraphic.Watch || Tooth.IsPrimary(toothGraphic.ToothID)) {
					continue;
				}
				watchTeeth[toothGraphic.ToothID]=toothGraphic;
			}
			for(int t=0;t<TcData.ListToothGraphics.Count;t++) {//loop through each primary tooth
			  ToothGraphic toothGraphic=TcData.ListToothGraphics[t];
				//If a tooth is marked to be watched then it is always visible, even if the tooth is missing/hidden.
				if(toothGraphic.ToothID=="implant"|| !toothGraphic.Watch || !Tooth.IsPrimary(toothGraphic.ToothID) || !toothGraphic.Visible) {
					continue;
				}
				watchTeeth[Tooth.PriToPerm(toothGraphic.ToothID)]=toothGraphic;
			}
			foreach(DictionaryEntry toothGraphic in watchTeeth){
				RenderToothWatch(g,(ToothGraphic)toothGraphic.Value);
			}
		}

		private void RenderToothWatch(Graphics g,ToothGraphic toothGraphic){
			float toMm=1f/TcData.ScaleMmToPix;
			SolidBrush brush=new SolidBrush(toothGraphic.colorWatch);
			//Drawing a white silhouette around the colored watch W doesn't make sense here because unerupted teeth do not change color in this chart.
			if(ToothGraphic.IsRight(toothGraphic.ToothID)){
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)){
					g.DrawString("W",Font,brush,new PointF(TcData.GetTransXpix(toothGraphic.ToothID)+toothGraphic.ShiftM-6f,0));
				}
				else{
					g.DrawString("W",Font,brush,new PointF(TcData.GetTransXpix(toothGraphic.ToothID)+toothGraphic.ShiftM-7f,Height-Font.Size-8f));
				}
			}
			else{
				if(ToothGraphic.IsMaxillary(toothGraphic.ToothID)){
					g.DrawString("W",Font,brush,new PointF(TcData.GetTransXpix(toothGraphic.ToothID)-toothGraphic.ShiftM-6f,0));
				}
				else{
					g.DrawString("W",Font,brush,new PointF(TcData.GetTransXpix(toothGraphic.ToothID)-toothGraphic.ShiftM-7f,Height-Font.Size-8f));
				}
			}
			brush.Dispose();
		}

		private void DrawNumbers(Graphics g) {
			if(DesignMode) {
				return;
			}
			string tooth_id;
			for(int i=1;i<=52;i++) {
				tooth_id=Tooth.FromOrdinal(i);
				if(TcData.SelectedTeeth.Contains(tooth_id)) {
					DrawNumber(tooth_id,true,true,g);
				}
				else {
					DrawNumber(tooth_id,false,true,g);
				}
			}
		}

		///<summary>Draws the number and the rectangle behind it.  Draws in the appropriate color</summary>
		private void DrawNumber(string tooth_id,bool isSelected,bool isFullRedraw,Graphics g) {
			if(DesignMode) {
				return;
			}
			if(TcData==null) {
				return;//trying to fix a designtime bug.
			}
			if(!Tooth.IsValidDB(tooth_id)) {
				return;
			}
			if(TcData.ListToothGraphics[tooth_id]==null) {
				//throw new Exception(tooth_id+" null");
				return;//for some reason, it's still getting to here in DesignMode
			}
			if(isFullRedraw) {//if redrawing all numbers
				if(TcData.ListToothGraphics[tooth_id].HideNumber) {//and this is a "hidden" number
					return;//skip
				}
				if(Tooth.IsPrimary(tooth_id)
					&& !TcData.ListToothGraphics[Tooth.PriToPerm(tooth_id)].ShowPrimaryLetter)//but not set to show primary letters
				{
					return;
				}
			}
			string displayNum=Tooth.GetToothLabelGraphic(tooth_id,TcData.ToothNumberingNomenclature);
			float toMm=1f/TcData.ScaleMmToPix;
			float labelWidthMm=g.MeasureString(displayNum,Font).Width/TcData.ScaleMmToPix;
			float labelHeightMm=((float)Font.Height-.5f)/TcData.ScaleMmToPix;
			SizeF labelSizeF=new SizeF(labelWidthMm,(float)Font.Height/TcData.ScaleMmToPix);
			Rectangle rec=TcData.GetNumberRecPix(tooth_id,labelSizeF);
			//Rectangle recPix=TcData.ConvertRecToPix(recMm);
			if(isSelected) {
				g.FillRectangle(new SolidBrush(TcData.ColorBackHighlight),rec);
			}
			else {
				g.FillRectangle(new SolidBrush(TcData.ColorBackground),rec);
			}
			if(TcData.ListToothGraphics[tooth_id].HideNumber) {//If number is hidden.
				//do not print string
			}
			else if(Tooth.IsPrimary(tooth_id)
				&& !TcData.ListToothGraphics[Tooth.PriToPerm(tooth_id)].ShowPrimaryLetter) {
				//do not print string
			}
			else if(isSelected) {
				g.DrawString(displayNum,Font,new SolidBrush(TcData.ColorTextHighlight),rec.X,rec.Y);
			}
			else {
				g.DrawString(displayNum,Font,new SolidBrush(TcData.ColorText),rec.X,rec.Y);
			}
		}

		private void DrawDrawingSegments(Graphics g) {
			string[] pointStr;
			List<PointF> points;
			PointF pointf;
			string[] xy;
			float x;
			float y;
			Pen pen;
			for(int s=0;s<TcData.DrawingSegmentList.Count;s++) {
				pen=new Pen(TcData.DrawingSegmentList[s].ColorDraw,2.2f*TcData.PixelScaleRatio);
				pointStr=TcData.DrawingSegmentList[s].DrawingSegment.Split(';');
				points=new List<PointF>();
				for(int p=0;p<pointStr.Length;p++) {
					xy=pointStr[p].Split(',');
					if(IsValidCoordinate(xy,out x,out y)) {
						x=TcData.RectTarget.X+x*TcData.PixelScaleRatio;
						y=TcData.RectTarget.Y+y*TcData.PixelScaleRatio;
						pointf=new PointF(x,y);
						points.Add(pointf);
					}
				}
				if(points.Count<2) {//Can't draw a line with less than 2 points.
					continue;//See ToothChartDirectX.DrawDrawingSegments() and ToothChartDirectX.DrawExtendedLineStrip().
				}
				g.DrawLines(pen,points.ToArray());
			}
		}

		private bool IsValidCoordinate(string[] coordinate,out float x,out float y) {
			x=0;
			y=0;
			return coordinate.Length==2 && float.TryParse(coordinate[0],out x) && float.TryParse(coordinate[1],out y);
		}

		///<summary>Returns a bitmap of what is showing in the control.  Used for printing.</summary>
		public Bitmap GetBitmap() {
			Bitmap bmp=new Bitmap(this.Width,this.Height);
			Graphics gfx=Graphics.FromImage(bmp);
			PaintEventArgs e=new PaintEventArgs(gfx,new Rectangle(0,0,bmp.Width,bmp.Height));
			OnPaint(e);
			return bmp;
		}

		#region Mouse And Selections

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			MouseIsDown=true;
			if(TcData.ListToothGraphics.Count==0){//still starting up?
				return;
			}
			if(TcData.CursorTool==CursorTool.Pointer){
				_listSelectedTeethOld=TcData.SelectedTeeth.FindAll(x => x!=null);//Make a copy of the list.  No elements should ever be null (copy all).
				string toothClicked=TcData.GetToothAtPoint(e.Location);
				if(TcData.SelectedTeeth.Contains(toothClicked)) {
					SetSelected(toothClicked,false);
				}
				else {
					SetSelected(toothClicked,true);
				}
			}
			else if(TcData.CursorTool==CursorTool.Pen) {
				TcData.PointList.Add(new PointF(e.X,e.Y));
			}
			else if(TcData.CursorTool==CursorTool.Eraser) {
				//do nothing
			}
			else if(TcData.CursorTool==CursorTool.ColorChanger) {
				//look for any lines near the "wand".
				//since the line segments are so short, it's sufficient to check end points.
				string[] xy;
				string[] pointStr;
				float x;
				float y;
				float dist;//the distance between the point being tested and the center of the eraser circle.
				float radius=2f;//by trial and error to achieve best feel.
				PointF pointMouseScaled=TcData.GetPointMouseScaled(e.X,e.Y,Size);
				//PointF eraserPt=new PointF(e.X+8.49f,e.Y+8.49f);
				for(int i=0;i<TcData.DrawingSegmentList.Count;i++) {
					pointStr=TcData.DrawingSegmentList[i].DrawingSegment.Split(';');
					for(int p=0;p<pointStr.Length;p++){
						xy=pointStr[p].Split(',');
						if(IsValidCoordinate(xy,out x,out y)) {
							dist=(float)Math.Sqrt(Math.Pow(Math.Abs(x-pointMouseScaled.X),2)+Math.Pow(Math.Abs(y-pointMouseScaled.Y),2));
							if(dist<=radius){//testing circle intersection here
								OnSegmentDrawn(TcData.DrawingSegmentList[i].DrawingSegment);
								TcData.DrawingSegmentList[i].ColorDraw=TcData.ColorDrawing;
								Invalidate();
								return;
							}
						}
					}
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(TcData.ListToothGraphics.Count==0) {
				return;
			}
			if(TcData.CursorTool==CursorTool.Pointer) {
				//if(drawMode==DrawingMode.Simple2D) {
				hotTooth=TcData.GetToothAtPoint(e.Location);
				if(hotTooth==hotToothOld) {//mouse has not moved to another tooth
					return;
				}
				hotToothOld=hotTooth;
				if(MouseIsDown) {//drag action
					if(TcData.SelectedTeeth.Contains(hotTooth)) {
						SetSelected(hotTooth,false);
					}
					else {
						SetSelected(hotTooth,true);
					}
				}
				//}
			}
			else if(TcData.CursorTool==CursorTool.Pen) {
				if(!MouseIsDown){
					return;
				}
				TcData.PointList.Add(new PointF(e.X,e.Y));
				//just add the last line segment instead of redrawing the whole thing.
				//Graphics g=this.CreateGraphics();
				g.SmoothingMode=SmoothingMode.HighQuality;
				Pen pen=new Pen(TcData.ColorDrawing,2.2f*TcData.PixelScaleRatio);
				int i=TcData.PointList.Count-1;
				g.DrawLine(pen,TcData.PointList[i-1].X,TcData.PointList[i-1].Y,TcData.PointList[i].X,TcData.PointList[i].Y);
				//g.Dispose();
				//Invalidate();
			}
			else if(TcData.CursorTool==CursorTool.Eraser) {
				if(!MouseIsDown){
					return;
				}
				//look for any lines that intersect the "eraser".
				//since the line segments are so short, it's sufficient to check end points.
				string[] xy;
				string[] pointStr;
				float x;
				float y;
				float dist;//the distance between the point being tested and the center of the eraser circle.
				float radius=8f;//by trial and error to achieve best feel.
				PointF eraserPt=TcData.GetPointMouseScaled(e.X+8.49f,e.Y+8.49f,Size);
				for(int i=0;i<TcData.DrawingSegmentList.Count;i++) {
					pointStr=TcData.DrawingSegmentList[i].DrawingSegment.Split(';');
					for(int p=0;p<pointStr.Length;p++){
						xy=pointStr[p].Split(',');
						if(IsValidCoordinate(xy,out x,out y)){
							dist=(float)Math.Sqrt(Math.Pow(Math.Abs(x-eraserPt.X),2)+Math.Pow(Math.Abs(y-eraserPt.Y),2));
							if(dist<=radius){//testing circle intersection here
								OnSegmentDrawn(TcData.DrawingSegmentList[i].DrawingSegment);//triggers a deletion from db.
								TcData.DrawingSegmentList.RemoveAt(i);
								Invalidate();
								return;
							}
						}
					}
				}
			}
			else if(TcData.CursorTool==CursorTool.ColorChanger) {
				//do nothing
			}
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			MouseIsDown=false;
			if(TcData.CursorTool==CursorTool.Pointer) {
				if(TcData.HasSelectedTeethChanged(_listSelectedTeethOld)) {
					OnToothSelectionsChanged();
				}
			}
			else if(TcData.CursorTool==CursorTool.Pen) {
				string drawingSegment="";
				for(int i=0;i<TcData.PointList.Count;i++) {
					if(i>0) {
						drawingSegment+=";";
					}
					PointF pointMouseScaled=TcData.GetPointMouseScaled(TcData.PointList[i].X,TcData.PointList[i].Y,Size);
					//I could compensate to center point here:
					drawingSegment+=pointMouseScaled.X+","+pointMouseScaled.Y;
				}
				OnSegmentDrawn(drawingSegment);
				TcData.PointList=new List<PointF>();
				//Invalidate();//?
			}
			else if(TcData.CursorTool==CursorTool.Eraser) {
				//do nothing
			}
			else if(TcData.CursorTool==CursorTool.ColorChanger) {
				//do nothing
			}
		}

		///<summary></summary>
		protected void OnSegmentDrawn(string drawingSegment) {
			ToothChartDrawEventArgs tArgs=new ToothChartDrawEventArgs(drawingSegment);
			if(SegmentDrawn!=null) {
				SegmentDrawn(this,tArgs);
			}
		}

		///<summary></summary>
		protected void OnToothSelectionsChanged() {
			if(ToothSelectionsChanged!=null) {
				ToothSelectionsChanged(this);
			}
		}

		///<summary>Used by mousedown and mouse move to set teeth selected or unselected.  A similar method is used externally in the wrapper to set teeth selected.  This private method might be faster since it only draws the changes.</summary>
		private void SetSelected(string tooth_id,bool setValue) {
			//Graphics g=this.CreateGraphics();
			if(setValue) {
				TcData.SelectedTeeth.Add(tooth_id);
				DrawNumber(tooth_id,true,false,g);
			}
			else {
				TcData.SelectedTeeth.Remove(tooth_id);
				DrawNumber(tooth_id,false,false,g);
			}
			//string displayNum=Tooth.GetToothLabelGraphic(tooth_id,TcData.ToothNumberingNomenclature);
			//float labelWidthMm=g.MeasureString(displayNum,Font).Width/TcData.ScaleMmToPix;
			//float labelHeightMm=(float)Font.Height/TcData.ScaleMmToPix;
			//SizeF labelSizeF=new SizeF(labelWidthMm,labelHeightMm);
			//RectangleF recF=TcData.GetNumberRecPix(tooth_id,labelSizeF);
			//Rectangle rec=new Rectangle((int)recF.X,(int)recF.Y,(int)recF.Width,(int)recF.Height);
			Invalidate();//rec);
			Application.DoEvents();
			/*
			if(TcData.ALSelectedTeeth.Count==0) {
				selectedTeeth=new string[0];
			}
			else {
				selectedTeeth=new string[ALSelectedTeeth.Count];
				for(int i=0;i<ALSelectedTeeth.Count;i++) {
					if(ToothGraphic.IsValidToothID(ToothGraphic.PermToPri(ALSelectedTeeth[i].ToString()))//pri is valid
					&& ListToothGraphics[ALSelectedTeeth[i].ToString()].ShowPrimary)//and set to show pri
				{
						selectedTeeth[i]=ToothGraphic.PermToPri(ALSelectedTeeth[i].ToString());
					}
					else {
						selectedTeeth[i]=((int)ALSelectedTeeth[i]).ToString();
					}
				}
			}*/
			//g.Dispose();
		}

		#endregion Mouse And Selections




	}
}
