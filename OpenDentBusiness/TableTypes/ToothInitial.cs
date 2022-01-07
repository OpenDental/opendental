using System;
using System.Collections;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	/// <summary>Used to track missing teeth, primary teeth, movements, and drawings.</summary>
	[Serializable]
	[CrudTable(IsLargeTable=true)]
	public class ToothInitial:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ToothInitialNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>1-32 or A-Z. Supernumeraries not supported here yet.</summary>
		public string ToothNum;
		///<summary>Enum:ToothInitialType</summary>
		public ToothInitialType InitialType;
		///<summary>Shift in mm, or rotation / tipping in degrees.</summary>
		public float Movement;
		///<summary>Point data for a drawing segment.  The format would look similar to this: 45,68;48,70;49,72;0,0;55,88;etc.  It's simply a sequence of points, separated by semicolons.  Only positive numbers are used.  0,0 is the upper left of the tooth chart, and the lower right is at 410,307.  This scale of 410,307 is always used, regardless of how the tooth chart control is scaled for viewing.  Floats with tenths can be included.  If the pen is picked up, it becomes a new segment, so a new row in the database.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string DrawingSegment;
		///<summary>.</summary>
		[XmlIgnore]
		public Color ColorDraw;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>For text not associated with any tooth. The location of the text within 410,307 is incorporated into this string.  Example: 25.3,123.8;This shows.  Carriage returns etc are not supported.  ColorDraw is used.</summary>
		public string DrawText;

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

		///<summary></summary>
		public ToothInitial Copy(){
			return (ToothInitial)this.MemberwiseClone();
		}

		///<summary>toothinitial.DrawText is two components: point;string.  This gets the string.</summary>
		public string GetTextString(){
			if(!DrawText.Contains(";")){
				return "";
			}
			string[] strArray=DrawText.Split(';');
			return strArray[1];
		}

		///<summary>toothinitial.DrawText is two components: point;string.  This gets the point.</summary>
		public PointF GetTextPoint(){
			if(!DrawText.Contains(";")){
				return new PointF();
			}
			string[] strArray=DrawText.Split(';');
			if(!Regex.IsMatch(strArray[0],@"^\d+\.?\d*,\d+\.?\d*$")){//"#.#,#.#"
				return new PointF();
			}
			string[] xy=strArray[0].Split(',');
			float x=float.Parse(xy[0]);
			float y=float.Parse(xy[1]);
			return new PointF(x,y);		
		}

		///<summary>toothinitial.DrawText is two components: point;string.  This sets both.</summary>
		public void SetText(PointF location,string text){
			DrawText=location.X.ToString()+","+location.Y.ToString()+";"+text;
		}
	

	}

	///<summary></summary>
	public enum ToothInitialType{
		///<summary>0</summary>
		Missing,
		///<summary>1 - Also hides the number.  Number can be primary or permanent.</summary>
		Hidden,
		///<summary>2 - Only used with 1-32.  "sets" this tooth as a primary tooth.  The result is that the primary tooth shows in addition to the perm, and that the letter shows in addition to the number.  It also does a Shift0 -12 and some other handy movements.  Even if this is set to true, there can be a separate entry for a missing primary tooth; this would be almost equivalent to not even setting the tooth as primary, but would also allow user to select the letter.</summary>
		Primary,
		///<summary>3 - Mesial mm</summary>
		ShiftM,
		///<summary>4 - Occlusal/incisal mm</summary>
		ShiftO,
		///<summary>5 - Buccal aka Labial mm</summary>
		ShiftB,
		///<summary>6 - Clockwise as viewed from occlusal/incisal.</summary>
		Rotate,
		///<summary>7 - Mesial degrees</summary>
		TipM,
		///<summary>8 - Buccal degrees</summary>
		TipB,
		///<summary>9 - One segment of a drawing.</summary>
		Drawing,
		///<summary>10 - Location and string, combined</summary>
		Text
	}




}

















