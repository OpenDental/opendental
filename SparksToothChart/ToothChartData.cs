using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenDentBusiness;
using SharpDX.Direct3D9;

namespace SparksToothChart {
	///<summary>This is an object full of data about how to draw the 3D graphical teeth.  It also contains a number of helper functions that need to be shared between the different tooth charts.</summary>
	public class ToothChartData {
		///<summary>A strongly typed collection of ToothGraphics.  This includes all 32 perm and all 20 primary teeth, whether they will be drawn or not.  If a tooth is missing, it gets marked as visible false.  If it's set to primary, then the permanent tooth gets repositioned under the primary, and a primary gets set to visible true.  If a tooth is impacted, it gets repositioned.  Supernumerary graphics are not yet supported, but they might be handled by adding to this list.  "implant" is also stored as another tooth in this collection.  It is just used to store the graphics for any implant.</summary>
		public ToothGraphicCollection ListToothGraphics;
		///<summary>The main color of the background behind the teeth.</summary>
		public Color ColorBackground;
		///<summary>The normal color of the text for tooth numbers and letters.</summary>
		public Color ColorText;
		///<summary>The color of text when a tooth number is highlighted.</summary>
		public Color ColorTextHighlight;
		///<summary>The color of the background highlight rectangle around a selected tooth number.</summary>
		public Color ColorBackHighlight;
		private List<string> selectedTeeth;
		public List<ToothInitial> DrawingSegmentList;
		///<summary>This is the size of the control in screen pixels.</summary>
		private Size sizeControl;
		/// <summary>In tooth mm, exactly how much of the projection to show.</summary>
		public SizeF SizeOriginalProjection=new SizeF(130f,97.34f);
		///<summary>Ratio of pix/mm.  Gets recalculated every time SizeControl changes due to wrapper resize.  Multiply this ratio times a tooth mm measurement to get a pixel equivalent. If starting with a pixel value, then divide it by this ratio to get mm.</summary>
		public float ScaleMmToPix;
		///<summary>Whenever the control is resized, this value is set.  If the control ratio is wider than the 3D chart ratio, then this is true.  There would be extra background space on the sides.  If the ratio is taller than the 3D chart, then extra background on the top and bottom.  Default is (barely) false.</summary>
		public bool IsWide;
		///<summary>This defines a rectangle within the control where the tooth chart is to be drawn.  It will be different than the SizeControl if the control is wider or taller than the projection ratio.  This is set every time the control is resized.  It's in pixels.</summary>
		public Rectangle RectTarget;
		/// <summary>NEVER CHANGE from 410,307. When the drawing feature was originally added, this was the size of the tooth chart. This number must forever be preserved and drawings scaled to account for it.</summary>
		public static Size SizeOriginalDrawing=new Size(410,307);//NEVER CHANGE
		///<summary>An enum that indicates which kind of cursor is currently being used.</summary>
		public CursorTool CursorTool;
		///<summary>The color being used for freehand drawing.</summary>
		public Color ColorDrawing;
		///<summary>This font object is used to measure strings.</summary>
		public System.Drawing.Font Font;
		///<summary>A list of points for a line currently being drawn.  Once the mouse is raised, this list gets cleared.</summary>
		public List<PointF> PointList;
		///<summary>The size of the current drawing in pixels / the size of the original drawing.  This number is used to scale original drawing to the new size.</summary>
		public float PixelScaleRatio;
		public ToothNumberingNomenclature ToothNumberingNomenclature;
		public bool PerioMode;
		///<summary>This very closely mirrors the organization in the db, but we don't include here mobility or skiptooth.</summary>
		public List<PerioMeasure> ListPerioMeasure;
		public Color ColorBleeding;
		public Color ColorSuppuration;
		public Color ColorFurcations;
		public Color ColorFurcationsRed;
		public Color ColorGingivalMargin;
		public Color ColorCAL;
		public Color ColorMGJ;
		public Color ColorProbing;
		public Color ColorProbingRed;
		///<summary>Any probing bar that is deeper than or equal to this number will show in 'red'.  Typical value is 4 or 5.</summary>
		public int RedLimitProbing;
		///<summary>Any furcation grade that is greater than or equal to this number will show in 'red'.  Typical value is 2.</summary>
		public int RedLimitFurcations;

		public ToothChartData() {
			ListToothGraphics=new ToothGraphicCollection();
			ColorBackground=Color.FromArgb(150,145,152);//defaults to gray
			ColorText=Color.White;
			ColorTextHighlight=Color.Red;
			ColorBackHighlight=Color.White;
			selectedTeeth=new List<string>();
			sizeControl=SizeOriginalDrawing;
			DrawingSegmentList=new List<ToothInitial>();
			CursorTool=CursorTool.Pointer;
			ColorDrawing=Color.Black;
			Font=new System.Drawing.Font(FontFamily.GenericSansSerif,8.25f);
			PointList=new List<PointF>();
			ListPerioMeasure=new List<PerioMeasure>();
		}

		///<summary></summary>
		public ToothChartData Copy() {
			ToothChartData tcd=(ToothChartData)MemberwiseClone();
			tcd.ListToothGraphics=this.ListToothGraphics.Copy();
			tcd.selectedTeeth=new List<string>();
			foreach(string tooth in selectedTeeth) {
				tcd.selectedTeeth.Add(tooth);
			}
			tcd.DrawingSegmentList=new List<ToothInitial>();
			foreach(ToothInitial ti in DrawingSegmentList) {
				tcd.DrawingSegmentList.Add(ti.Copy());
			}
			tcd.PointList=new List<PointF>();
			foreach(PointF p in PointList) {
				tcd.PointList.Add(p);
			}
			tcd.ListPerioMeasure=new List<PerioMeasure>();
			foreach(PerioMeasure pm in ListPerioMeasure) {
				tcd.ListPerioMeasure.Add(pm.Copy());
			}
			return tcd;
		}

		///<summary>Unregisters all vertex and index buffers from their current device.</summary>
		public void CleanupDirectX(){
			for(int i=0;i<ListToothGraphics.Count;i++){
				ListToothGraphics[i].CleanupDirectX();
			}
		}

		///<summary>Registers all vertex and index buffers with the specified device.</summary>
		public void PrepareForDirectX(Device device){
			for(int i=0;i<ListToothGraphics.Count;i++) {
				ListToothGraphics[i].PrepareForDirectX(device);
			}
		}

		///<summary>This gets set whenever the wrapper resizes.  It's the size of the control in screen pixels.</summary>
		public Size SizeControl {
			get {
				return sizeControl;
			}
			set {
				sizeControl=value;
				//compute scaleMmToPix
				if(SizeOriginalProjection.Width/(float)sizeControl.Width < SizeOriginalProjection.Height/(float)sizeControl.Height) { 
					//if wide, use control h.  The default settings will just barely make this false.
					IsWide=true;
					ScaleMmToPix=(float)sizeControl.Height/SizeOriginalProjection.Height;
					RectTarget.Height=sizeControl.Height;
					RectTarget.Y=0;
					RectTarget.Width=(int)(((float)SizeOriginalDrawing.Width/SizeOriginalDrawing.Height)*RectTarget.Height);
					RectTarget.X=(sizeControl.Width-RectTarget.Width)/2;
				}
				else {//otherwise, use control w
					IsWide=false;
					ScaleMmToPix=(float)sizeControl.Width/(float)SizeOriginalProjection.Width;
					RectTarget.Width=sizeControl.Width;
					RectTarget.X=0;
					RectTarget.Height=(int)(((float)SizeOriginalDrawing.Height/SizeOriginalDrawing.Width)*RectTarget.Width);
					RectTarget.Y=(sizeControl.Height-RectTarget.Height)/2;
				}
				PixelScaleRatio=(float)RectTarget.Width/(float)SizeOriginalDrawing.Width;
			}
		}

		///<summary>Valid values are 1-32 and A-Z.  To set which teeth are selected, use SetSelected().</summary>
		public List<string> SelectedTeeth {
			get {
				return selectedTeeth;
			}
		}

		public bool HasSelectedTeethChanged(List<string> listSelectedTeethOld) {
			foreach(string tooth in listSelectedTeethOld) {
				if(!selectedTeeth.Contains(tooth)) {
					return true;
				}
			}
			foreach(string tooth in selectedTeeth) {
				if(!listSelectedTeethOld.Contains(tooth)) {
					return true;
				}
			}
			return false;
		}

		///<summary>Gets the rectangle surrounding a tooth number.  Used to draw the box and the number inside it.  Includes any mesial shifts.  Pass in the labelSize in scene mm.  The resulting rectangle has origin at lower left regardless of what quadrant it's in.</summary>
		public RectangleF GetNumberRecMm(string tooth_id,SizeF labelSizeF){
			float xPos=0;
			float yPos=0;
			if(ToothGraphic.IsMaxillary(tooth_id)) {
				if(Tooth.IsPrimary(tooth_id)) {
					yPos+=4.9f;
				}
				else {
					yPos+=.8f;
				}
			}
			else {
				if(Tooth.IsPrimary(tooth_id)) {
					yPos-=labelSizeF.Height+4.9f;
				}
				else {
					yPos-=labelSizeF.Height+.8f;
				}
			}
			xPos+=GetTransX(tooth_id);
			//string displayNum=OpenDentBusiness.Tooth.GetToothLabelGraphic(tooth_id,ToothNumberingNomenclature);
			//float strWidthMm=g.MeasureString(displayNum,Font).Width/ScaleMmToPix;
			//xPos-=strWidthMm/2f;
			xPos-=labelSizeF.Width/2f;
			//only use the ShiftM portion of the user translation
			if(ToothGraphic.IsRight(tooth_id)) {
				xPos+=ListToothGraphics[tooth_id].ShiftM;
			}
			else {
				xPos-=ListToothGraphics[tooth_id].ShiftM;
			}
			//float toMm=(float)WidthProjection/(float)widthControl;//mm/pix
			float toMm=1f/ScaleMmToPix;
			RectangleF recMm=new RectangleF(xPos-0f*toMm,yPos-0f*toMm,labelSizeF.Width-0f*toMm,labelSizeF.Height);//this rec has origin at LL
			return recMm;
		}

		///<summary>Used by 2D tooth chart to get the rectangle in pixels surrounding a tooth number.  Used to draw the box and the number inside it.</summary>
		public Rectangle GetNumberRecPix(string tooth_id,SizeF labelSizeF) {
			return ConvertRecToPix(GetNumberRecMm(tooth_id,labelSizeF));
			/*
			float xPos=GetTransXpix(tooth_id);
			float yPos=sizeControl.Height/2f;
			if(ToothGraphic.IsMaxillary(tooth_id)) {
				if(Tooth.IsPrimary(tooth_id)) {
					yPos-=25;
				}
				else {
					yPos-=14;
				}
			}
			else {
				if(Tooth.IsPrimary(tooth_id)) {
					yPos+=14;
				}
				else {
					yPos+=3;
				}
			}
			string displayNum =tooth_id;
			//displayNum =OpenDentBusiness.Tooth.GetToothLabel(tooth_id);
			float strWidth=g.MeasureString(displayNum,Font).Width;
			xPos-=strWidth/2f;
			RectangleF rec=new RectangleF(xPos-1,yPos-1,strWidth,12);//this rec has origin at UL
			return rec;*/
		}

		///<summary>Pri or perm tooth numbers are valid.  Only locations of perm teeth are stored.  This also converts mm to screen pixels.  This is currently only used in 2D mode.</summary>
		public float GetTransXpix(string tooth_id) {
			int toothInt=ToothGraphic.IdToInt(tooth_id);
			if(toothInt==-1) {
				throw new ApplicationException("Invalid tooth number: "+tooth_id);//only for debugging
			}
			float xmm=ToothGraphic.GetDefaultOrthoXpos(toothInt);//in +/- mm from center
			return (sizeControl.Width/2)+(xmm*ScaleMmToPix);
				//( SizeOriginalProjection.Width/2f+xmm)*ScaleMmToPix;//*widthControl/WidthProjection;
		}

		///<summary>In control coords rather than mm.  This is not really meaninful except in 2D mode.  It calculates the location of the facial view in order to draw the big red X.</summary>
		public float GetTransYfacialPix(string tooth_id) {
			if(ToothGraphic.IsMaxillary(tooth_id)) {
				return sizeControl.Height/2-(101f*PixelScaleRatio);
			}
			return sizeControl.Height/2+(101f*PixelScaleRatio);
		}

		///<summary>In control coords rather than mm.  This is not really meaninful except in 2D mode.  It calculates the location of the occlusal surface in order to draw the bullseye.</summary>
		public float GetTransYocclusalPix(string tooth_id){//,int heightControl) {
			if(ToothGraphic.IsMaxillary(tooth_id)) {
				return sizeControl.Height/2-(48f*PixelScaleRatio);
			}
			return sizeControl.Height/2+(48f*PixelScaleRatio);
		}

		///<summary>Pri or perm tooth numbers are valid.  Only locations of perm teeth are stored.</summary>
		public float GetTransX(string tooth_id) {
			int toothInt=ToothGraphic.IdToInt(tooth_id);
			if(toothInt==-1) {
				throw new ApplicationException("Invalid tooth number: "+tooth_id);//only for debugging
			}
			return ToothGraphic.GetDefaultOrthoXpos(toothInt);
		}

		public float GetTransYfacial(string tooth_id) {
			float basic=29f;
			if(tooth_id=="6" || tooth_id=="11") {
				return basic+1f;
			}
			if(tooth_id=="7" || tooth_id=="10") {
				return basic+1f;
			}
			else if(tooth_id=="8" || tooth_id=="9") {
				return basic+2f;
			}
			else if(tooth_id=="22" || tooth_id=="27") {
				return -basic-2f;
			}
			else if(tooth_id=="23" || tooth_id=="24" || tooth_id=="25" || tooth_id=="26") {
				return -basic-2f;
			}
			else if(ToothGraphic.IsMaxillary(tooth_id)) {
				return basic;
			}
			return -basic;
		}

		public float GetTransYocclusal(string tooth_id) {
			if(ToothGraphic.IsMaxillary(tooth_id)) {
				return 13f;
			}
			return -13f;
		}

		///<summary>This also adjusts the result to account for a control that is not the same proportion as the original.  Result could be outside the projection area.</summary>
		public PointF PointPixToMm(PointF pixPoint) {
			/*
			float toMmRatio=(float)WidthProjection/(float)widthControl;//mm/pix
			float mmX=(((float)pixPoint.X)*toMmRatio)-((float)WidthProjection)/2f;
			float idealHeightProjection=(float)WidthProjection*(float)SizeOriginalDrawing.Height/(float)SizeOriginalDrawing.Width;
			float actualHeightProjection=(float)WidthProjection*(float)heightControl/(float)widthControl;
			float mmY=(idealHeightProjection)/2f-(((float)pixPoint.Y)*toMmRatio);
			return new PointF(mmX,mmY);*/
			float toMmRatio=1f/ScaleMmToPix;
			float mmX=(((float)(pixPoint.X-RectTarget.X))*toMmRatio)-((float)SizeOriginalProjection.Width)/2f;
			//float idealHeightProjection=(float)WidthProjection*(float)SizeOriginalDrawing.Height/(float)SizeOriginalDrawing.Width;
			//float actualHeightProjection=(float)WidthProjection*(float)heightControl/(float)widthControl;
			float mmY=(SizeOriginalProjection.Height)/2f-(((float)(pixPoint.Y-RectTarget.Y))*toMmRatio);
			return new PointF(mmX,mmY);
		}

		/// <summary>Takes an original db point as originally entered in unscaled control coordinates, and returns coordinates in scene mm's.</summary>
		public PointF PointDrawingPixToMm(PointF pixPoint) {
			float toMmRatio=1f/ScaleMmToPix;
			float mmX=(((float)pixPoint.X*PixelScaleRatio)*toMmRatio)-((float)SizeOriginalProjection.Width)/2f;
			float mmY=((float)SizeOriginalProjection.Height)/2f-(((float)pixPoint.Y*PixelScaleRatio)*toMmRatio);
			return new PointF(mmX,mmY);
		}

		/*
		///<summary>The recPix has origin at UL.  The result has origin at LL.</summary>
		private RectangleF ConvertRecToMm(RectangleF recPix) {
			float w=recPix.Width/ScaleMmToPix;
			float h=recPix.Height/ScaleMmToPix;
			float x=(recPix.X-(float)(sizeControl.Width/2))/ScaleMmToPix;
			float y=(recPix.Bottom-(float)(sizeControl.Height/2))/ScaleMmToPix;
			return new RectangleF(x,y,w,h);			
		}*/


		///<summary>The recMm has origin at LL.  The result has origin at UL and is in control coords.</summary>
		private Rectangle ConvertRecToPix(RectangleF recMm) {
			int w=(int)(recMm.Width*ScaleMmToPix);
			int h=(int)(recMm.Height*ScaleMmToPix);
			int x=(int)(recMm.X*ScaleMmToPix+sizeControl.Width/2);
			int y=(int)((sizeControl.Height/2-recMm.Y*ScaleMmToPix)-h);
			return new Rectangle(x,y,w,h);		
		}

		/*
		///<summary>Always returns a number between 1 and 32.  This isn't perfect, since it only operates on perm teeth, and assumes that any primary tooth will be at the same x pos as its perm tooth.</summary>
		public int GetToothAtPoint(int x,int y) {
			//This version was originally used in 2D chart
			
			float closestDelta=(float)(Width*2);//start it off really big
			int closestTooth=1;
			float toothPos=0;
			float delta=0;
			//float xPos=(float)((float)(x-Width/2)*WidthProjection/(float)Width);//in mm instead of screen coordinates
			if(y<Height/2) {//max
				for(int i=1;i<=16;i++) {
					if(ListToothGraphics[i.ToString()].HideNumber) {
						continue;
					}
					toothPos=GetTransX(i.ToString());//ToothGraphic.GetDefaultOrthoXpos(i);
					if(x>toothPos) {//xPos>toothPos) {
						delta=x-toothPos;
					}
					else {
						delta=toothPos-x;
					}
					if(delta<closestDelta) {
						closestDelta=delta;
						closestTooth=i;
					}
				}
				return closestTooth;
			}
			else {//mand
				for(int i=17;i<=32;i++) {
					if(ListToothGraphics[i.ToString()].HideNumber) {
						continue;
					}
					toothPos=GetTransX(i.ToString());//ToothGraphic.GetDefaultOrthoXpos(i);//in mm.
					if(x>toothPos) {
						delta=x-toothPos;
					}
					else {
						delta=toothPos-x;
					}
					if(delta<closestDelta) {
						closestDelta=delta;
						closestTooth=i;
					}
				}
				return closestTooth;
			}
			return 1;
		}*/


		///<summary>Input is pixel coordinates of control.  Always returns a string, 1 through 32 or A through T.  Primary letters are only returned if that tooth is set as primary.</summary>
		public string GetToothAtPoint(Point point) {
			//This version was originally in OpenGL.
			float closestDelta=(float)(SizeOriginalProjection.Width*2);//start it off really big
			string closestTooth="1";
			float toothPos=0;
			float delta=0;
			//convert x and y to mm.  Use those measurements to match the closest tooth.
			//float xPos=(float)((float)(x-Width/2)*WidthProjection/(float)Width);//in mm instead of screen coordinates
			float xPos=PointPixToMm(point).X;
			float yPos=PointPixToMm(point).Y;
			string perm_id;
			bool isPriArea;//this point is where a primary letter might sometimes show
			bool priShowing;
			bool permShowing;
			bool usePri;//otherwise, use perm
			string tooth_id;
			if(yPos>0) {//maxillary
				for(int i=1;i<=16;i++) {
					perm_id=i.ToString();
					if(i>=4 && i<=13 && !IsPermArea(yPos)){
						isPriArea=true;
					}
					else{
						isPriArea=false;
					}
					//Determine which numbers are showing
					priShowing=false;
					if(ListToothGraphics[perm_id].ShowPrimaryLetter
						&& !ListToothGraphics[Tooth.PermToPri(perm_id)].HideNumber)
					{
						priShowing=true;
					}
					permShowing=true;
					if(ListToothGraphics[perm_id].HideNumber) {
						permShowing=false;
					}
					if(!priShowing && !permShowing) {//if neither # showing
						continue;
					}
					if(priShowing) {
						if(permShowing) {//both showing
							if(isPriArea) {
								usePri=true;
							}
							else {
								usePri=false;
							}
						}
						else {
							usePri=true;
						}
					}
					else {//only perm showing
						usePri=false;
					}
					if(usePri) {
						tooth_id=Tooth.PermToPri(perm_id);
					}
					else {
						tooth_id=perm_id;
					}
					toothPos=ToothGraphic.GetDefaultOrthoXpos(i);
					if(ToothGraphic.IsRight(perm_id)) {
						toothPos+=(int)ListToothGraphics[tooth_id].ShiftM;
					}
					else {
						toothPos-=(int)ListToothGraphics[tooth_id].ShiftM;
					}
					if(xPos>toothPos) {
						delta=xPos-toothPos;
					}
					else {
						delta=toothPos-xPos;
					}
					if(delta<closestDelta) {
						closestDelta=delta;
						closestTooth=tooth_id;
					}
				}
				return closestTooth;
			}
			else {//mandibular
				for(int i=17;i<=32;i++) {
					perm_id=i.ToString();
					if(i>=20 && i<=29 && !IsPermArea(yPos)) {
						isPriArea=true;
					}
					else {
						isPriArea=false;
					}
					//Determine which numbers are showing
					priShowing=false;
					if(ListToothGraphics[perm_id].ShowPrimaryLetter
						&& !ListToothGraphics[Tooth.PermToPri(perm_id)].HideNumber) {
						priShowing=true;
					}
					permShowing=true;
					if(ListToothGraphics[perm_id].HideNumber) {
						permShowing=false;
					}
					if(!priShowing && !permShowing) {//if neither # showing
						continue;
					}
					if(priShowing) {
						if(permShowing) {//both showing
							if(isPriArea) {
								usePri=true;
							}
							else {
								usePri=false;
							}
						}
						else {
							usePri=true;
						}
					}
					else {//only perm showing
						usePri=false;
					}
					if(usePri) {
						tooth_id=Tooth.PermToPri(perm_id);
					}
					else {
						tooth_id=perm_id;
					}
					toothPos=ToothGraphic.GetDefaultOrthoXpos(i);
					if(ToothGraphic.IsRight(perm_id)) {
						toothPos+=(int)ListToothGraphics[tooth_id].ShiftM;
					}
					else {
						toothPos-=(int)ListToothGraphics[tooth_id].ShiftM;
					}
					if(xPos>toothPos) {
						delta=xPos-toothPos;
					}
					else {
						delta=toothPos-xPos;
					}
					if(delta<closestDelta) {
						closestDelta=delta;
						closestTooth=tooth_id;
					}
				}
				return closestTooth;
			}
		}

		///<summary>Input is y position in mm scene coordinates.  Will return true if it's close to the horizontal midline, in the area where the perm tooth numbers are.</summary>
		public bool IsPermArea(float yPos) {
			if(yPos > 5.1f || yPos < -4.4f) {
				return false;
			}
			return true;
		}

		///<summary>When this is used from within a toothchart in response to mouse activity, it is typically followed by explicit drawing instructions that efficiently shows the user which teeth are selected.  When this is used from the wrapper, it's typically followed by an Invalidate().</summary>
		public void SetSelected(string tooth_id,bool setValue) {
			if(setValue) {
				if(!SelectedTeeth.Contains(tooth_id)) {
					SelectedTeeth.Add(tooth_id);
				}
				//DrawNumber(tooth_id,true,false);
			}
			else {
				if(SelectedTeeth.Contains(tooth_id)) {
					SelectedTeeth.Remove(tooth_id);
				}
				//DrawNumber(tooth_id,false,false);
			}
			//RectangleF recMm=TcData.GetNumberRecMm(tooth_id,);
			//Rectangle rec=TcData.ConvertRecToPix(recMm);
		}

		///<summary>When teeth are selected using a sweeping mouse motion, it might be too fast for some intermediate teeth to be included. This method takes the first and last tooth_id's and returns a list including the last one as well as any intermediate teeth.</summary>
		public List<string> GetAffectedTeeth(string startingId, string endingId,float yPos) {
			List<string> affectedTeeth=new List<string>();
			affectedTeeth.Add(endingId);
			int startingOrdinal=Tooth.ToOrdinal(startingId);
			if(Tooth.IsPrimary(startingId)) {
				startingOrdinal=Tooth.ToOrdinal(Tooth.PriToPerm(startingId));
			}
			int endingOrdinal=Tooth.ToOrdinal(endingId);
			if(Tooth.IsPrimary(endingId)) {
				endingOrdinal=Tooth.ToOrdinal(Tooth.PriToPerm(endingId));
			}
			if(Math.Abs(startingOrdinal-endingOrdinal) <= 1) {//if they are not separated by more than one.
				return affectedTeeth;
			}
			if(Tooth.IsMaxillary(startingId)!=Tooth.IsMaxillary(endingId)) {//if they are not in the same arch
				return affectedTeeth;
			}
			bool isInPermArea=IsPermArea(yPos);//close to the horizontal midline
			string permId;
			string priId;//will be blank if invalid
			if(endingOrdinal < startingOrdinal) {
				for(int i=endingOrdinal+1;i<startingOrdinal;i++) {//we're only going after the teeth in between
					permId=Tooth.FromOrdinal(i);
					priId=Tooth.PermToPri(permId);
					//the only situation where the following tests will fail is if set to primary, mouse is in the perm area, and perm is hidden.
					if(priId!=""//it's possible to have a pri number here
						&& ListToothGraphics[permId].ShowPrimaryLetter //and a primary letter is showing
						&& !isInPermArea //and the mouse is in the primary area
						&& !ListToothGraphics[priId].HideNumber) //and the primary tooth number is not hidden
					{
						affectedTeeth.Add(priId);
					}
					else if(!ListToothGraphics[permId].HideNumber){
						affectedTeeth.Add(permId);
					}
				}
			}
			else {
				for(int i=startingOrdinal+1;i<endingOrdinal;i++) {
					permId=Tooth.FromOrdinal(i);
					priId=Tooth.PermToPri(permId);
					if(priId!=""
						&& ListToothGraphics[permId].ShowPrimaryLetter 
						&& !isInPermArea 
						&& !ListToothGraphics[priId].HideNumber) 
					{
						affectedTeeth.Add(priId);
					}
					else if(!ListToothGraphics[permId].HideNumber) {
						affectedTeeth.Add(permId);
					}
				}
			}
			return affectedTeeth;
		}

		///<summary>Use this to test a site (there are 3 sites per tooth face).  If it returns 0, do not draw a furctation.  If it returns a number 1-3, then use GetFurcationPos to know where to put the triangle or V.</summary>
		public int GetFurcationValue(int intTooth,PerioSurf surf){
			for(int i=0;i<ListPerioMeasure.Count;i++){
				if(ListPerioMeasure[i].IntTooth!=intTooth) {
					continue;
				}
				if(ListPerioMeasure[i].SequenceType!=PerioSequenceType.Furcation){
					continue;
				}
				int meas=0;
				if(surf==PerioSurf.MB) {
					meas=ListPerioMeasure[i].MBvalue;
				}
				if(surf==PerioSurf.B) {
					meas=ListPerioMeasure[i].Bvalue;
				}
				if(surf==PerioSurf.DB) {
					meas=ListPerioMeasure[i].DBvalue;
				}
				if(surf==PerioSurf.ML) {
					meas=ListPerioMeasure[i].MLvalue;
				}
				if(surf==PerioSurf.L) {
					meas=ListPerioMeasure[i].Lvalue;
				}
				if(surf==PerioSurf.DL) {
					meas=ListPerioMeasure[i].DLvalue;
				}
				if(meas==-1) {
					return 0;
				}
				return meas;
			}
			return 0;
		}
		
		///<summary>Use GetFurcationValue first.  Then, this method returns the position in mm of the tip of the triangle or V relative to the center of the tooth.</summary>
		public PointF GetFurcationPos(int intTooth,PerioSurf surf) {
			float ysign=1;
			if(Tooth.IsMaxillary(intTooth)) {
				ysign=1;
			}
			else {//mand
				ysign=-1;
			}
			float xshift=GetXShiftPerioSite(intTooth,surf);
			return new PointF(xshift,ysign*9.5f);
		}

		///<summary>Draws the short vertical lines that represent probing depths.  Use this on each site (3 per tooth face).  The z component will be 0.  The coordinates will be relative to the center of the tooth.  The line will always only have one segment.  It will return null if no line to draw at this site.  The color of the line will be pulled from a different method.</summary>
		public LineSimple GetProbingLine(int intTooth,PerioSurf surf,out Color color) {
			color=ColorProbing;
			if(!ListToothGraphics[intTooth.ToString()].Visible && !ListToothGraphics[intTooth.ToString()].IsImplant) {
				return null;
			}
			float xshift=GetXShiftPerioSite(intTooth,surf);
			int gm=0;
			int pd=0;//line length
			for(int i=0;i<ListPerioMeasure.Count;i++) {
				if(ListPerioMeasure[i].IntTooth!=intTooth) {
					continue;
				}
				if(ListPerioMeasure[i].SequenceType==PerioSequenceType.Probing) {
					switch(surf) {
						case PerioSurf.MB:
							pd=ListPerioMeasure[i].MBvalue;
							break;
						case PerioSurf.B:
							pd=ListPerioMeasure[i].Bvalue;
							break;
						case PerioSurf.DB:
							pd=ListPerioMeasure[i].DBvalue;
							break;
						case PerioSurf.ML:
							pd=ListPerioMeasure[i].MLvalue;
							break;
						case PerioSurf.L:
							pd=ListPerioMeasure[i].Lvalue;
							break;
						case PerioSurf.DL:
							pd=ListPerioMeasure[i].DLvalue;
							break;
					}
				}
				if(ListPerioMeasure[i].SequenceType==PerioSequenceType.GingMargin) {
					switch(surf) {
						case PerioSurf.MB:
							//Examples: 0, -1(null), 5, or 105(hyperplasia).  But the null is not even being considered.  So adjusting 100+ vals to -x would work.
							gm=PerioMeasures.AdjustGMVal(ListPerioMeasure[i].MBvalue);//Converts the above examples to 0, 0, 5, and -5.
							break;
						case PerioSurf.B:
							gm=PerioMeasures.AdjustGMVal(ListPerioMeasure[i].Bvalue);
							break;
						case PerioSurf.DB:
							gm=PerioMeasures.AdjustGMVal(ListPerioMeasure[i].DBvalue);
							break;
						case PerioSurf.ML:
							gm=PerioMeasures.AdjustGMVal(ListPerioMeasure[i].MLvalue);
							break;
						case PerioSurf.L:
							gm=PerioMeasures.AdjustGMVal(ListPerioMeasure[i].Lvalue);
							break;
						case PerioSurf.DL:
							gm=PerioMeasures.AdjustGMVal(ListPerioMeasure[i].DLvalue);
							break;
					}
				}
			}
			if(pd==0 || pd==-1) {
				return null;
			}
			if(pd >= RedLimitProbing) {
				color=ColorProbingRed;
			}
			//CAL shouldn't be less than 0, so we need to draw probing lines down to zero if CAL is negative for some reason. (Or maybe we just let this happen so that dentists know something is wrong.
			if(Tooth.IsMaxillary(intTooth)) {
				return new LineSimple(xshift,gm,0,xshift,gm+pd,0);
			}
			else {//mand
				return new LineSimple(xshift,-gm,0,xshift,-(gm+pd),0);
			}
		}

		///<summary>Relative to the center of the tooth. The sign is based on area of the mouth.  The magnitude is based on tooth width.  This will be used for the probing bars and horizontal lines.  Probably also for furcations.</summary>
		private float GetXShiftPerioSite(int intTooth,PerioSurf surf) {
			if(surf==PerioSurf.B || surf==PerioSurf.L) {
				return 0;
			}
			float xdirect=1;
			if(Tooth.IsMaxillary(intTooth)) {
				if(surf==PerioSurf.MB || surf==PerioSurf.ML) {
					if(ToothGraphic.IsRight(intTooth.ToString())) {//UR quadrant
						xdirect=1;
					}
					else {//UL
						xdirect=-1;
					}
				}
				else if(surf==PerioSurf.DB || surf==PerioSurf.DL) {
					if(ToothGraphic.IsRight(intTooth.ToString())) {//UR quadrant
						xdirect=-1;
					}
					else {//UL
						xdirect=1;
					}
				}
			}
			else {//mand
				if(surf==PerioSurf.MB || surf==PerioSurf.ML) {
					if(ToothGraphic.IsRight(intTooth.ToString())) {//LR quadrant
						xdirect=1;
					}
					else {//LL
						xdirect=-1;
					}
				}
				else if(surf==PerioSurf.DB || surf==PerioSurf.DL) {
					if(ToothGraphic.IsRight(intTooth.ToString())) {//LR quadrant
						xdirect=-1;
					}
					else {//LL
						xdirect=1;
					}
				}
			}
			float toothW=ToothGraphic.GetWidth(intTooth);
			float magnitude;
			switch(intTooth) {
				default:
					magnitude=.28f; break;
				case 1:
				case 2:
				case 15:
				case 16:
					magnitude=.32f; break;
				case 17:
				case 32:
				case 18:
				case 31:
					magnitude=.35f; break;
				case 3:
				case 14:
					magnitude=.38f; break;
				case 19:
				case 30:
					magnitude=.37f; break;
			}
			if(ListToothGraphics[intTooth.ToString()].IsImplant) {
				return 2f*xdirect;
			}
			return magnitude*toothW*xdirect;
		}

		///<summary>This gets the entire set of lines for one perio row for one sequence type.  The allowed types are GM, MGJ, and CAL.  Each LineSimple is a series of connected lines.  But the result could have interruptions, so we return a list, each item in the list being continuous.  There may be zero items in the list.  Each line in the list is guaranteed to have at least 2 points in it.</summary>
		public List<LineSimple> GetHorizontalLines(PerioSequenceType sequenceType,bool isMaxillary,bool isBuccal) {
			List<LineSimple> retVal=new List<LineSimple>();
			int startTooth=1;
			int stopTooth=17;//doesn't perform a loop for 17.
			if(!isMaxillary) {
				startTooth=32;//We still go Left to Right, even on mand.
				stopTooth=16;
			}
			LineSimple line=new LineSimple();
			Vertex3f vertex;
			int val1=-1;
			int val2=-1;
			int val3=-1;
			int t=startTooth;
			PerioSurf surf1;
			PerioSurf surf2;
			PerioSurf surf3;
			while(t!=stopTooth){
				if(!ListToothGraphics[t.ToString()].Visible && !ListToothGraphics[t.ToString()].IsImplant) {
					//stop any existing line.
					if(line.Vertices.Count==1) {//if there is already one point, then clear it, because a line can't have one point.
						line.Vertices.Clear();
					}
					if(line.Vertices.Count>1) {//if 2 or more points in the line, then add the line to the result.
						retVal.Add(line);
						line=new LineSimple();//and initialize a new line for future points.
					}
					//increment to next tooth
					if(isMaxillary) {
						t++;
					}
					else {
						t--;
					}
					continue;
				}
				//We are considering 3 points per tooth.  Reinitialize for the new tooth.
				val1=-1;
				val2=-1;
				val3=-1;
				surf1=PerioSurf.None;
				surf2=PerioSurf.None;
				surf3=PerioSurf.None;
				for(int i=0;i<ListPerioMeasure.Count;i++) {
					if(ListPerioMeasure[i].IntTooth!=t) {
						continue;
					}
					if(ListPerioMeasure[i].SequenceType!=sequenceType) {
						continue;
					}
					//so we are now on the specific PerioMeasure for this sequence and tooth.  It contains 6 values, and we will use 3.
					PerioMeasure pmGM=null;
					//We need to draw MGJ as dist from GM, not CEJ
					if(sequenceType==PerioSequenceType.MGJ) {//we only care about this if we are trying to calculate MGJ
						for(int m=0;m<ListPerioMeasure.Count;m++) {
							if(ListPerioMeasure[m].IntTooth==t
								&& ListPerioMeasure[m].SequenceType==PerioSequenceType.GingMargin) 
							{
								pmGM=ListPerioMeasure[m];//get the GM for this same tooth.
								break;
							}
						}
					}
					if(isBuccal) {
						if(ToothGraphic.IsRight(t.ToString())) {
							val1=ListPerioMeasure[i].DBvalue;
							val2=ListPerioMeasure[i].Bvalue;
							val3=ListPerioMeasure[i].MBvalue;
							if(sequenceType==PerioSequenceType.MGJ && pmGM!=null) {
								if(pmGM.DBvalue!=-1) {
									val1+=PerioMeasures.AdjustGMVal(pmGM.DBvalue);
								}
								if(pmGM.Bvalue!=-1) {
									val2+=PerioMeasures.AdjustGMVal(pmGM.Bvalue);
								}
								if(pmGM.MBvalue!=-1) {
									val3+=PerioMeasures.AdjustGMVal(pmGM.MBvalue);
								}
							}
							surf1=PerioSurf.DB;
							surf2=PerioSurf.B;
							surf3=PerioSurf.MB;
						}
						else {//for UL and LL
							val1=ListPerioMeasure[i].MBvalue;
							val2=ListPerioMeasure[i].Bvalue;
							val3=ListPerioMeasure[i].DBvalue;
							if(sequenceType==PerioSequenceType.MGJ && pmGM!=null) {
								if(pmGM.MBvalue!=-1) {
									val1+=PerioMeasures.AdjustGMVal(pmGM.MBvalue);
								}
								if(pmGM.Bvalue!=-1) {
									val2+=PerioMeasures.AdjustGMVal(pmGM.Bvalue);
								}
								if(pmGM.DBvalue!=-1) {
									val3+=PerioMeasures.AdjustGMVal(pmGM.DBvalue);
								}
							}
							surf1=PerioSurf.MB;
							surf2=PerioSurf.B;
							surf3=PerioSurf.DB;
						}
					}
					else {//lingual
						if(ToothGraphic.IsRight(t.ToString())) {
							val1=ListPerioMeasure[i].DLvalue;
							val2=ListPerioMeasure[i].Lvalue;
							val3=ListPerioMeasure[i].MLvalue;
							if(sequenceType==PerioSequenceType.MGJ && pmGM!=null) {
								if(pmGM.DLvalue!=-1) {
									val1+=PerioMeasures.AdjustGMVal(pmGM.DLvalue);
								}
								if(pmGM.Lvalue!=-1) {
									val2+=PerioMeasures.AdjustGMVal(pmGM.Lvalue);
								}
								if(pmGM.MLvalue!=-1) {
									val3+=PerioMeasures.AdjustGMVal(pmGM.MLvalue);
								}
							}
							surf1=PerioSurf.DL;
							surf2=PerioSurf.L;
							surf3=PerioSurf.ML;
						}
						else {//for UL and LL
							val1=ListPerioMeasure[i].MLvalue;
							val2=ListPerioMeasure[i].Lvalue;
							val3=ListPerioMeasure[i].DLvalue;
							if(sequenceType==PerioSequenceType.MGJ && pmGM!=null) {
								if(pmGM.MLvalue!=-1) {
									val1+=PerioMeasures.AdjustGMVal(pmGM.MLvalue);
								}
								if(pmGM.Lvalue!=-1) {
									val2+=PerioMeasures.AdjustGMVal(pmGM.Lvalue);
								}
								if(pmGM.DLvalue!=-1) {
									val3+=PerioMeasures.AdjustGMVal(pmGM.DLvalue);
								}
							}
							surf1=PerioSurf.ML;
							surf2=PerioSurf.L;
							surf3=PerioSurf.DL;
						}
					}
				}
				//We have now filled our 3 points with values and need to evaluate those values.
				//Any or all of the values may still be -1.
				if(val1==-1) {
					//we won't add a point to this line
					if(line.Vertices.Count==1) {//if there is already one point, then clear it, because a line can't have one point.
						line.Vertices.Clear();
					}
					if(line.Vertices.Count>1) {//if 2 or more points in the line, then add the line to the result.
						retVal.Add(line);
						line=new LineSimple();//and initialize a new line for future points.
					}
				}
				else {//just add a point to the current line.
					vertex=new Vertex3f();
					vertex.Z=0;//we don't use z
					if(isMaxillary) {
						//this is safe to run on all sequence types because -1 has already been handled and because other types wouldn't have values > 100.
						//Also safe to process on the vals that are MGJ, calculated above, because if they are ever negative, 
						//it would be an obvious entry error, and the MGJ line would just harmlessly disappear for -1 vals.
						vertex.Y=PerioMeasures.AdjustGMVal(val1);
					}
					else {
						vertex.Y=-PerioMeasures.AdjustGMVal(val1);
					}
					vertex.X=GetXShiftPerioSite(t,surf1)+ToothGraphic.GetDefaultOrthoXpos(t);
					line.Vertices.Add(vertex);
				}
				//val2--------------------------
				if(val2==-1) {
					if(line.Vertices.Count==1) {
						line.Vertices.Clear();
					}
					if(line.Vertices.Count>1) {
						retVal.Add(line);
						line=new LineSimple();
					}
				}
				else {
					vertex=new Vertex3f();
					vertex.Z=0;
					if(isMaxillary) {
						vertex.Y=PerioMeasures.AdjustGMVal(val2);
					}
					else {
						vertex.Y=-PerioMeasures.AdjustGMVal(val2);
					}
					vertex.X=GetXShiftPerioSite(t,surf2)+ToothGraphic.GetDefaultOrthoXpos(t);
					line.Vertices.Add(vertex);
				}
				//val3-------------------------
				if(val3==-1) {
					if(line.Vertices.Count==1) {
						line.Vertices.Clear();
					}
					if(line.Vertices.Count>1) {
						retVal.Add(line);
						line=new LineSimple();
					}
				}
				else {
					vertex=new Vertex3f();
					vertex.Z=0;
					if(isMaxillary) {
						vertex.Y=PerioMeasures.AdjustGMVal(val3);
					}
					else {
						vertex.Y=-PerioMeasures.AdjustGMVal(val3);
					}
					vertex.X=GetXShiftPerioSite(t,surf3)+ToothGraphic.GetDefaultOrthoXpos(t);
					line.Vertices.Add(vertex);
				}
				//increment to next tooth
				if(isMaxillary) {
					t++;
				}
				else {
					t--;
				}
			}
			if(line.Vertices.Count>1) {
				retVal.Add(line);
			}
			return retVal;
		}

		///<summary>For a given tooth and surface, gets a point at which to draw bleeding or suppuration droplet.  Use this on each site (3 per tooth face). The coordinates will be relative to the center of the tooth.  It will return 0,0 if no droplet to draw at this site.  If isBleeding is false, then it gets suppuration.</summary>
		public PointF GetBleedingOrSuppuration(int intTooth,PerioSurf surf,bool isBleeding) {
			if(!ListToothGraphics[intTooth.ToString()].Visible && !ListToothGraphics[intTooth.ToString()].IsImplant) {
				return new PointF(0,0);
			}
			float xshift=GetXShiftPerioSite(intTooth,surf);
			float yshift=-1.5f;//max
			if(!Tooth.IsMaxillary(intTooth)) {
				yshift=1.5f;
			}
			int siteVal=-1;
			for(int i=0;i<ListPerioMeasure.Count;i++) {
				if(ListPerioMeasure[i].IntTooth!=intTooth) {
					continue;
				}
				if(ListPerioMeasure[i].SequenceType!=PerioSequenceType.Bleeding) {
					continue;
				}
				switch(surf) {
					case PerioSurf.MB:
						siteVal=ListPerioMeasure[i].MBvalue;
						break;
					case PerioSurf.B:
						siteVal=ListPerioMeasure[i].Bvalue;
						break;
					case PerioSurf.DB:
						siteVal=ListPerioMeasure[i].DBvalue;
						break;
					case PerioSurf.ML:
						siteVal=ListPerioMeasure[i].MLvalue;
						break;
					case PerioSurf.L:
						siteVal=ListPerioMeasure[i].Lvalue;
						break;
					case PerioSurf.DL:
						siteVal=ListPerioMeasure[i].DLvalue;
						break;
				}
				break;				
			}
			if(siteVal==-1 || siteVal==0) {
				return new PointF(0,0);
			}
			if(isBleeding) {
				if(((BleedingFlags)siteVal & BleedingFlags.Blood) == BleedingFlags.Blood) {
					return new PointF(xshift-.3f,yshift);//shift bleeding points slightly to left.
				}
				else {
					return new PointF(0,0);
				}
			}
			else {//suppuration
				if(((BleedingFlags)siteVal & BleedingFlags.Suppuration) == BleedingFlags.Suppuration) {
					return new PointF(xshift+.3f,yshift);//shift suppuration points slightly to right.
				}
				else {
					return new PointF(0,0);
				}
			}
		}

		///<summary>Returns a series of points that can be used to create a droplet shape for bleeding and suppuration.  The points form a pentagon with a sixth implied point at 0,0.  Use the points and 0,0 to create 5 triangles.  Coordinates are in mm's.  If the droplet needs to be scaled, that will be done inside this method rather than externally.  The droplet will need to be flipped or rotated about the 0,0 point for use in the mandibular.</summary>
		public SharpDX.Vector3[] GetDropletVertices() {
			float scale=1f;
			return new SharpDX.Vector3[] {
				new SharpDX.Vector3(0,scale*.89f,0),//top point
				new SharpDX.Vector3(scale*.34f,scale*.049f,0),//upper right
				new SharpDX.Vector3(scale*.21f,scale*-.35f,0),//lower right
				new SharpDX.Vector3(scale*-.21f,scale*-.35f,0),//lower left
				new SharpDX.Vector3(scale*-.34f,scale*.049f,0)//upper left
			};
		}

		///<summary>Scales given mouse location (mouseX,mouseY) from standard chart size stored in SizeOriginalDrawing to given sizeChart.
		///Rounds each coordinate in the return value to the nearest 10th.</summary>
		public PointF GetPointMouseScaled(float mouseX,float mouseY,Size sizeChart) {
			PointF pointScaled=new PointF(mouseX,mouseY);
			if(!sizeChart.Equals(SizeOriginalDrawing)) {
				pointScaled.X=(float)Math.Round((SizeOriginalDrawing.Width*mouseX)/sizeChart.Width,1);//Multiply first, then divide, for maximum precision.
				pointScaled.Y=(float)Math.Round((SizeOriginalDrawing.Height*mouseY)/sizeChart.Height,1);//Multiply first, then divide, for maximum precision.
			}
			return pointScaled;
		}

	}
}
