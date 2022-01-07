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

		///<summary></summary>
		public MountDef Copy() {
			return (MountDef)this.MemberwiseClone();
		}

		
	}

		



		
	

	

	


}










