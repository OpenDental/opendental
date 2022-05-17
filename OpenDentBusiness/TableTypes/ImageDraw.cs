using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	/// <summary>Image text, lines. drawings, and scales. Attached to either a document or a mount.</summary>
	[Serializable]
	public class ImageDraw:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ImageDrawNum;
		///<summary>FK to document.DocNum</summary>
		public long DocNum;
		///<summary>FK to mount.MountNum</summary>
		public long MountNum;
		///<summary>For text, this is the foreground color. For lines, this is the color, and ColorBack is not used.</summary>
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
		///<summary>This could vary significantly based on the size of the image.  It's always relative to orginal image or mount pixels.</summary>
		public float FontSize;
		///<summary>Enum:ImageDrawType</summary>
		public ImageDrawType DrawType;

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
		ScaleValue
	}
}

















