using PdfSharp.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	/// <summary>Image text, lines. drawings, and scales. Attached to either a document or a mount. Drawings are in pixel coordinates of original image prior to any cropping or rotating. For a mount, coordinates are relative to the entire mount. Drawings do not get changed when cropping or rotating are changed. The result is that drawings always stay on the image in exactly the original location, and they move with the image.</summary>
	[Serializable]
	public class ImageDraw:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ImageDrawNum;
		///<summary>FK to document.DocNum</summary>
		public long DocNum;
		///<summary>FK to mount.MountNum</summary>
		public long MountNum;
		///<summary>For text, this is the foreground color. For lines, this is the color, and ColorBack is not used. For polygons, this is the fill color. No transparency component.</summary>
		[XmlIgnore]
		public Color ColorDraw;
		///<summary>Background color for text. Can be Transparent (0,255,255,255)=16777215.</summary>
		[XmlIgnore]
		public Color ColorBack;
		///<summary>Point data for a drawing segment.  The format would look similar to this: 45.2,68.1;48,70;49,72;0,0;55,88;etc.  It's simply a sequence of points, separated by semicolons.  Only positive floats are used, rounded to one decimal place.  0,0 is the upper left of the image or mount.  Cropping is ignored.  If the pen is picked up, it becomes a new segment, so a new row in the database.  Or, if this is DrawType.ScaleValue, then this field stores scale, decimal places, and units, separated by spaces.  Example: "123.4 0 mm". The first two are required; units is optional.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string DrawingSegment;
		///<summary>The location of the text in pixels is incorporated into this string.  Example: 25,123;This shows.  Carriage returns etc are not supported.  ColorDraw and FontSize are also used.  Unlike tooth initial, this does not support floats.</summary>
		public string DrawText;
		///<summary>This could vary significantly based on the size of the image.  It's always relative to orginal image or mount pixels. Always 0 for Pearl.</summary>
		public float FontSize;
		///<summary>Enum:ImageDrawType</summary>
		public ImageDrawType DrawType;
		///<summary>Enum:EnumImageAnnotVendor 0: Open Dental drawings and text, 1:Pearl AI annotations.</summary>
		public EnumImageAnnotVendor ImageAnnotVendor;
		///<summary>Extra space for any text. Currently only used for Pearl annotation categories, relationship properties, and relationship values, which are all stored as a single chunk of user readable text drawn straight to screen when hovering.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Details;
		/// <summary>Enum:Pearl.EnumCategoryOD This is how we hide and show layers for Pearl objects in the Imaging module.</summary>
		public Pearl.EnumCategoryOD PearlLayer;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorDraw",typeof(int))]
		public int ColorDrawXml {
			get {
				return ColorDraw.ToArgb();
			}
			set {
				ColorDraw=Color.FromArgb(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorBack",typeof(int))]
		public int ColorBackXml {
			get {
				return ColorBack.ToArgb();
			}
			set {
				ColorBack=Color.FromArgb(value);
			}
		}

		///<summary></summary>
		public ImageDraw Copy(){
			return (ImageDraw)this.MemberwiseClone();
		}

		///<summary>This centralizes handling of our global font size for Pearl.</summary>
		public float GetFontSize(){
			float fontSize=FontSize;
			if(Programs.IsEnabled(ProgramName.Pearl)){
				string strFontHeight=ProgramProperties.GetPropVal(ProgramName.Pearl,Bridges.Pearl.PEARL_FONT_SIZE_PROPERTY);
				try{
					fontSize=float.Parse(strFontHeight);
				}
				catch{
				}
			}
			if(fontSize<1){
				return 1;//pt size really shouldn't be smaller than one, which is only 1.3 pixels.
			}
			if(fontSize>100){
				return 100;//That's getting absurd at 133 pixels.
			}
			return fontSize;
		}

		///<summary>imagedraw.DrawText is two components: point;string.  This gets the string.</summary>
		public string GetTextString(){
			if(!DrawText.Contains(";")){
				return "";
			}
			string[] strArray=DrawText.Split(';');
			return strArray[1];
		}

		///<summary>imagedraw.DrawText is two components: point;string.  This gets the point.</summary>
		public Point GetTextPoint(){
			if(!DrawText.Contains(";")){
				return new Point();
			}
			string[] strArray=DrawText.Split(';');
			if(!Regex.IsMatch(strArray[0],@"^\d+\.?\d*,\d+\.?\d*$")){//"#.#,#.#"
				return new Point();
			}
			string[] xy=strArray[0].Split(',');
			float x=float.Parse(xy[0]);
			float y=float.Parse(xy[1]);
			return new Point((int)x,(int)y);		
		}

		
		///<summary>imagedraw.DrawText is two components: point;string.  This gets the point. Transaltes from pixels used by us to points used by PDFs.</summary>
		public XPoint GetTextPointPDF(){
			Point point=GetTextPoint();
			return new XPoint(GraphicsHelper.PixelsToPoints(point.X),GraphicsHelper.PixelsToPoints(point.Y));
		}

		///<summary>imagedraw.DrawText is two components: point;string.  This sets both.</summary>
		public void SetLocAndText(Point point,string text){
			DrawText=point.X.ToString()+","+point.Y.ToString()+";"+text;
		}

		///<summary>imagedraw.DrawText is two components: point;string.  This sets the location, keeping the existing text.</summary>
		public void SetLoc(Point point){
			string str=GetTextString();
			DrawText=point.X.ToString()+","+point.Y.ToString()+";"+str;
		}

		public void SetDrawingSegment(List<PointF> listPointFs){
			DrawingSegment="";
			for(int i=0;i<listPointFs.Count;i++){
				if(i>0){
					DrawingSegment+=";";
				}
				DrawingSegment+=listPointFs[i].X.ToString("0.#")//tenth, if present
					+","
					+listPointFs[i].Y.ToString("0.#");
			}
		}

		public List<PointF> GetPoints(){
			string[] stringArray=DrawingSegment.Split(';');
			List<PointF> listPointFs=new List<PointF>();
			for(int i=0;i<stringArray.Length;i++){
				string[] stringArray2=stringArray[i].Split(',');
				if(stringArray2.Length!=2){
					return new List<PointF>();
				}
				float x=0;
				try{
					x=Convert.ToSingle(stringArray2[0]);
				}
				catch{ }
				float y=0;
				try{
					y=Convert.ToSingle(stringArray2[1]);
				}
				catch{ }
				PointF pointF=new PointF(x,y);
				listPointFs.Add(pointF);
			}
			return listPointFs;
		}

		/// <summary>Gets the points from DrawingSegment and translates them from pixel coordinates to point coordinates for PDFs.</summary>
		public List<XPoint> GetPointsForPDF(){
			List<PointF> listPoints=GetPoints();
			List<XPoint> listXPoints=new List<XPoint>();
			for(int i=0;i<listPoints.Count;i++){
				XPoint xpoint=new XPoint(GraphicsHelper.PixelsToPoints(listPoints[i].X),GraphicsHelper.PixelsToPoints(listPoints[i].Y));
				listXPoints.Add(xpoint);
			}
			return listXPoints;
		}

		public void SetScale(float scale,int decimals,string units){
			DrawingSegment=scale.ToString()+" "+decimals.ToString();
			if(units!=null && units!=""){
				DrawingSegment+=" "+units;
			}
		}

		public float GetScale(){
			if(DrawingSegment is null){
				return 0;
			}
			string[] stringArray=DrawingSegment.Split(' ');
			if(stringArray.Length>0){
				return PIn.Float(stringArray[0]);
			}
			return 0;
		}

		public int GetDecimals(){
			if(DrawingSegment is null){
				return 0;
			}
			string[] stringArray=DrawingSegment.Split(' ');
			if(stringArray.Length>1){
				return PIn.Int(stringArray[1]);
			}
			return 0;
		}

		public string GetScaleUnits(){
			if(DrawingSegment is null){
				return "";
			}
			string[] stringArray=DrawingSegment.Split(' ');
			if(stringArray.Length==3){
				return stringArray[2];
			}
			return "";
		}
		

	}

	///<summary></summary>
	public enum ImageDrawType{
		///<summary>0 - Location and string, combined</summary>
		Text,
		///<summary>1 - A series of straight lines, stored the same as a pen drawing.</summary>
		Line,
		///<summary>2 - One continuous segment of a drawing.</summary>
		Pen,
		///<summary>3 - Stores a float, decimals, and units in the drawing segement. Only one of this type is allowed per image or mount.</summary>
		ScaleValue,
		///<summary>4 - A series of connected points forming the outline of a closed polygon. Stored same as pen drawing. Polygons only have a fill color, not any outline color.</summary>
		Polygon
	}

	///<summary></summary>
	public enum EnumImageAnnotVendor{
		///<summary>0 - Open Dental drawings and text.</summary>
		OpenDental,
		///<summary>1 - Pearl AI annotations.</summary>
		Pearl
	}
}

















