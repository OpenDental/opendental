using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	/// <summary>Template for each new mount.  But there is no linking of the mount back to this mountDef.  These can be freely deleted, renamed, moved, etc. without affecting any patient info.</summary>
	[Serializable()]
	public class MountDef : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MountDefNum;
		/// <summary>.</summary>
		public string Description;
		/// <summary>The order that the mount defs will show in various lists.</summary>
		public int ItemOrder;
		/// <summary>The width of the mount, in pixels.</summary>
		public int Width;
		/// <summary>Height of the mount, in pixels.</summary>
		public int Height;
		///<summary>Color of the mount background.  Typically white for photos and black for radiographs.</summary>
		[XmlIgnore]
		public Color ColorBack;
		///<summary>Color of drawings and text.  Typically black for photos and white for radiographs.</summary>
		[XmlIgnore]
		public Color ColorFore;
		///<summary>Color of drawing text background.  Typically white for photos and black for radiographs. Transparent is allowed.</summary>
		[XmlIgnore]
		public Color ColorTextBack;
		///<summary>Scale, decimal places, and units, separated by spaces.  Example: "123.4 0 mm". The first two are required; units is optional.  When a mount is created, this is converted into an ImageDraw of type ScaleValue.</summary>
		public string ScaleValue;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorBack",typeof(int))]
		public int ColorBackXml {
			get {
				return ColorBack.ToArgb();
			}
			set {
				ColorBack = Color.FromArgb(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorFore",typeof(int))]
		public int ColorForeXml {
			get {
				return ColorFore.ToArgb();
			}
			set {
				ColorFore = Color.FromArgb(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorTextBack",typeof(int))]
		public int ColorTextBackXml {
			get {
				return ColorTextBack.ToArgb();
			}
			set {
				ColorTextBack = Color.FromArgb(value);
			}
		}

		///<summary></summary>
		public MountDef Copy() {
			return (MountDef)this.MemberwiseClone();
		}

		public float GetScale(){
			if(ScaleValue is null){
				return 0;
			}
			string[] stringArray=ScaleValue.Split(' ');
			if(stringArray.Length>0){
				return PIn.Float(stringArray[0]);
			}
			return 0;
		}

		public int GetDecimals(){
			if(ScaleValue is null){
				return 0;
			}
			string[] stringArray=ScaleValue.Split(' ');
			if(stringArray.Length>1){
				return PIn.Int(stringArray[1]);
			}
			return 0;
		}

		public string GetScaleUnits(){
			if(ScaleValue is null){
				return "";
			}
			string[] stringArray=ScaleValue.Split(' ');
			if(stringArray.Length==3){
				return stringArray[2];
			}
			return "";
		}

		public void SetScale(float scale,int decimals,string units){
			ScaleValue=scale.ToString()+" "+decimals.ToString();
			if(units!=null && units!=""){
				ScaleValue+=" "+units;
			}
		}
		
	}

		



		
	

	

	


}










