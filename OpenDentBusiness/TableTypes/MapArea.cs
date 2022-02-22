using System;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>MapArea object will be placed on a MapAreaPanel and shown to give a physical layout of a location.</summary>
	[Serializable]
	public class MapArea:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MapAreaNum;
		///<summary>FK to Phone.Extension.  Typically 0.  Only used by HQ and when ItemType is set to Room.</summary>
		public int Extension;
		///<summary>X-position in the clinical map layout.  Indicates how many feet the MapArea should be placed from the left edge.</summary>
		public double XPos;
		///<summary>Y-position in the clinical map layout.  Indicates how many feet the MapArea should be placed from the top edge.</summary>
		public double YPos;
		///<summary>MapArea width measured in feet.  Not allowed to be zero.</summary>
		public double Width;
		///<summary>MapArea height measured in feet.</summary>
		public double Height;
		///<summary>Any text that the user types in.  Only used when ItemType is set to DisplayLabel.  Limit 255 char.</summary>
		public string Description;
		///<summary>Enum:MapItemType 0-Room,1-DisplayLabel</summary>
		public MapItemType ItemType;
		///<summary>The room that this map is in. This is not currently a table. Stored as a JSON serialized list in HQ only pref, HQSerializedMapContainers.</summary>
		public long MapAreaContainerNum;
		
		public MapArea Copy() {
			return (MapArea)this.MemberwiseClone();
		}
	}

	/// <summary>Indicate which type of MapArea we are dealing with.</summary>
	public enum MapItemType {
		///<summary>0 - A MapAreaRoomControl object.</summary>
		Room,
		///<summary>1 - A MapAreaDisplayLabelControl object.</summary>
		DisplayLabel
	}
}
