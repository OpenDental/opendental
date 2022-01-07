using System;
using System.Collections;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	/// <summary>Image annotations and lines. Attached to either a document or a mount.</summary>
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
		///<summary>Background color for text. Can be Transparent (0,255,255,255).</summary>
		[XmlIgnore]
		public Color ColorBack;
		///<summary>Point data for a drawing segment.  The format would look similar to this: 45,68;48,70;49,72;0,0;55,88;etc.  It's simply a sequence of points, separated by semicolons.  Only positive numbers are used.  0,0 is the upper left of the image or mount.  Cropping is ignored. Floats with tenths can be included.  If the pen is picked up, it becomes a new segment, so a new row in the database.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string DrawingSegment;
		///<summary>The location of the text in pixels is incorporated into this string.  Example: 25,123;This shows.  Carriage returns etc are not supported.  ColorDraw and FontSize are also used.  Unlike tooth initial, this does not support floats.</summary>
		public string DrawText;
		///<summary>This could vary significantly based on the size of the image.</summary>
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
		public void SetLocAndText(Point location,string text){
			DrawText=location.X.ToString()+","+location.Y.ToString()+";"+text;
		}

		///<summary>imagedraw.DrawText is two components: point;string.  This sets the location, keeping the existing text.</summary>
		public void SetLoc(Point location){
			string str=GetTextString();
			DrawText=location.X.ToString()+","+location.Y.ToString()+";"+str;
		}
		

	}

	///<summary></summary>
	public enum ImageDrawType{
		///<summary>0 - Location and string, combined</summary>
		Text,
		///<summary>1 - A series of straight lines, stored the same as a pen drawing.</summary>
		Line,
		///<summary>2 - One continuous segment of a drawing.</summary>
		Pen		
	}
}

















