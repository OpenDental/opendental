using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using CodeBase;

namespace OpenDentBusiness.Bridges{
	/// <summary></summary>
	public class Pearl{
/*
Pearl result:
    annotations (caries, bone loss, etc)
        category (possible example Caries)
        contourBox (x,y,w,h) (example green square outline)
        polygon (array x,y coords)
        relationships (rows of text in the pink boxes)
            prop_value
            metric_value
        line_segment (x1,x2,y1,y2,size)
        text (x,y,text)
    toothParts (always just background like enamel or dentin. No analysis)
        contour (x,y,w,h) (Unknown purpose. Ask about this)
        polygon (array x,y coords)
        color (including alpha)

Examples:
1. ToothParts.
    Background colors on teeth like dentin, enamel, and bone come in as toothParts. 
    We store them as ImageDraw polygons with some transparency.
2. Annotations. 
   Two examples are listed below.
      They can all have Category, which is a description.
      They can all have relationships, which are pairs of info.
      For example, Dentin 21%, Enamel 79%.
      Those are two relationships. Dentin and Enamel are prop_value. Percentages are metric_value.
      When we store them, each relationship is separated by CR so that all relationships end up as a single chunk of text.
      We will make this text show when hovering over a polygon or contour box.
   A. Caries
      Polygon
      In the same ImageDraw row, we store the relationships as described above.
      So as far as we are concerned, this is exactly the same as a toothPart, but with the addition of an annotation.
   B. Calculus
      Contour box (rectangle outline)
      Since we don't have unfilled polygons, this is stored as a line segment of 4 lines (5 points), ending at the starting point.
      In the same ImageDraw row, we store the relationships as described above.
3. Line with text
    line_segment
    annotation.text contains the text to show by the line.
    We store a line with text as two separate unconnected rows in ImageDraw.
    One ImageDraw row is a line 
    Second ImageDraw row is text. 
    Font is tricky since we get no hits at all about fontsize to use.
    We have added a ProgramProperty with default font height in pixels.
    Users can change this default at any time.
    For now, we will always draw to this default size, and we will not have any way to override per text item. This might change.
    Since we use a global font size, leave ImageDraw.FontSize as 0.

Needs research:
-Toothparts contour has no example.
*/
		//constants for ProgramProperties
		public static string StrTextFont="Text font size";

		//todo: there might be a better way than these public fields
		//public Program Program_;
		//public Patient Patient_;
		///<summary>For a single document, this will be a list of one. For a mount, this will contain all the individual bitmaps in the mount.</summary>
		public List<Bitmap> ListBitmaps;//don't need to dispose since this is just a second reference to same underlying bitmaps.
		///<summary>For a single document. In this case, the MountNum will be zero.</summary>
		public long DocNum;
		///<summary>Only used for a mount. 1:1 with ListBitmaps. We need this to determine the offset for each bitmap in the mount. Since we will get back drawings for an individual bitmap, we need to shift them all. We store drawings for a mount all relative to the mount origin, kind of like a big giant bitmap.</summary>
		public List<MountItem> ListMountItems;
		///<summary>For a mount. In this case, the DocNum will be zero.</summary>
		public long MountNum;

		///<summary>This is the worker thread</summary>
		public void SendOnThread(){
			if(DocNum==0 && MountNum==0){
				throw new Exception("Either DocNum or MountNum must be specified.");
			}
			//immediately send all images for a mount, or one image for a single document
			for(int i=0;i<ListBitmaps.Count;i++){
				if(ListBitmaps[i] is null){
					continue;
				}
				//SendOneToPearl(ListBitmaps[i]);
			}
			//Poll for results until all are returned.
			Thread.Sleep(2000);//simulated wait
			//Once all results are back, insert objects into db.
			//Todo: how do we know which result is which? Is there a primary key field or something?
			if(MountNum==0){
				ProcessOneResult(DocNum,mountNum:0,new Point(0,0));
				return;
			}
			//mount:
			for(int i=0;i<ListMountItems.Count;i++){
				if(ListMountItems[i] is null){
					continue;
				}
				Point pointTranslation=new Point(ListMountItems[i].Xpos,ListMountItems[i].Ypos);
				long mountNum=ListMountItems[i].MountNum;//same for all mountItems
				ProcessOneResult(docNum:0,mountNum,pointTranslation);
			}
		}

		/// <summary>Either DocNum or MountNum must be specified.</summary>
		private void ProcessOneResult(long docNum,long mountNum,Point pointTranslation){
			//Simulated polygon with annotation ===========================================================================================
			ImageDraw imageDraw=new ImageDraw();
			imageDraw.DocNum=docNum;
			imageDraw.MountNum=mountNum;
			imageDraw.ColorDraw=Color.FromArgb(alpha:80,Color.Red);
			imageDraw.DrawType=ImageDrawType.Polygon;
			List<PointF> listPointFs=new List<PointF>();
			//make a crooked square as demo
			listPointFs.Add(new PointF(45.2f,68.1f));//UL
			listPointFs.Add(new PointF(148,70));//UR
			listPointFs.Add(new PointF(145,172));//LR
			listPointFs.Add(new PointF(48,170));//LL
			for(int i=0;i<listPointFs.Count;i++){
				listPointFs[i]=new PointF(listPointFs[i].X+pointTranslation.X,listPointFs[i].Y+pointTranslation.Y);
			}
			imageDraw.SetDrawingSegment(listPointFs); 
			imageDraw.ImageAnnotVendor=EnumImageAnnotVendor.Pearl;
			imageDraw.Details="Caries\r\nDentin 21%\r\nEnamel 79%";
			ImageDraws.Insert(imageDraw);
			//Simulated line with measurement number on it:=====================================================================
			imageDraw=new ImageDraw();
			imageDraw.DocNum=docNum;
			imageDraw.MountNum=mountNum;
			imageDraw.ColorDraw=Color.Blue;
			imageDraw.DrawType=ImageDrawType.Line;
			listPointFs=new List<PointF>();
			//two points for a somewhat vertical line
			listPointFs.Add(new PointF(240,71));
			listPointFs.Add(new PointF(249,173));
			for(int i=0;i<listPointFs.Count;i++){
				listPointFs[i]=new PointF(listPointFs[i].X+pointTranslation.X,listPointFs[i].Y+pointTranslation.Y);
			}
			imageDraw.SetDrawingSegment(listPointFs); 
			imageDraw.ImageAnnotVendor=EnumImageAnnotVendor.Pearl;
			ImageDraws.Insert(imageDraw);
			//text:
			imageDraw=new ImageDraw();
			imageDraw.DocNum=docNum;
			imageDraw.MountNum=mountNum;
			imageDraw.ColorDraw=Color.Black;
			imageDraw.DrawType=ImageDrawType.Text;
			Point pointText=new Point(250,115);
			pointText=new Point(pointText.X+pointTranslation.X,pointText.Y+pointTranslation.Y);
			imageDraw.SetLocAndText(pointText,"5.4 mm"); 
			//do not set font
			imageDraw.ImageAnnotVendor=EnumImageAnnotVendor.Pearl;
			ImageDraws.Insert(imageDraw);
			//Simulated contourBox with annotation ===========================================================================================
			imageDraw=new ImageDraw();
			imageDraw.DocNum=docNum;
			imageDraw.MountNum=mountNum;
			imageDraw.ColorDraw=Color.LightGreen;
			imageDraw.DrawType=ImageDrawType.Line;
			listPointFs=new List<PointF>();
			//closed square
			listPointFs.Add(new PointF(300,70));//UL
			listPointFs.Add(new PointF(400,70));//UR
			listPointFs.Add(new PointF(400,170));//LR
			listPointFs.Add(new PointF(300,170));//LL
			listPointFs.Add(new PointF(300,70));//return to top so that it looks closed.
			for(int i=0;i<listPointFs.Count;i++){
				listPointFs[i]=new PointF(listPointFs[i].X+pointTranslation.X,listPointFs[i].Y+pointTranslation.Y);
			}
			imageDraw.SetDrawingSegment(listPointFs); 
			imageDraw.ImageAnnotVendor=EnumImageAnnotVendor.Pearl;
			imageDraw.Details="Periapical Radiolucency";
			ImageDraws.Insert(imageDraw);
		}

	}
}










