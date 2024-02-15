using System;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  All schema changes are done directly on our live database as needed.  MapArea object will be placed on a MapAreaPanel and shown to give a physical layout of a location.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]//or at least it will be soon
	public class MapArea:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MapAreaNum;
		///<summary>FK to phone.Extension.  Typically 0.  Only used by HQ and when ItemType is set to cubicle.</summary>
		public int Extension;
		///<summary>X-position in the clinical map layout.  Indicates how many feet the MapArea should be placed from the left edge.</summary>
		public double XPos;
		///<summary>Y-position in the clinical map layout.  Indicates how many feet the MapArea should be placed from the top edge.</summary>
		public double YPos;
		///<summary>MapArea width measured in feet.  Not allowed to be zero. Completely ignored for label.</summary>
		public double Width;
		///<summary>MapArea height measured in feet. Completely ignored for label.</summary>
		public double Height;
		///<summary>Text that shows for a Label.  For cubicle, it's currently the room, row, and seat, like B2 6:1, which doesn't get displayed.  Limit 255 char.</summary>
		public string Description;
		///<summary>Enum:MapItemType 0-Cubicle,1-Label</summary>
		public MapItemType ItemType;
		///<summary>FK to mapareacontainer.MapAreaContainerNum.</summary>
		public long MapAreaContainerNum;
		///<summary>Only for labels. Default 14.</summary>
		public float FontSize;
		
		public MapArea Copy() {
			return (MapArea)this.MemberwiseClone();
		}
	}

	/// <summary>Indicate which type of MapArea we are dealing with.</summary>
	public enum MapItemType {
		///<summary>0 - A MapCubicle object.</summary>
		Cubicle,
		///<summary>1 - A MapLabel object.</summary>
		Label
	}
}


//ALTER TABLE maparea ADD FontSize float NOT NULL;
//UPDATE maparea SET FontSize=14 WHERE ItemType=1;