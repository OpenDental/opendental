using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	/// <summary>These are always attached to a mount. Like a mount, they cannot be edited.  Documents are attached to each MountItem using Document.MountItemNum field.  Image will always be cropped to make it look smaller or bigger if it doesn't exactly match the mount item rectangle ratio.</summary>
	[Serializable()]
	public class MountItem : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MountItemNum ;
		/// <summary>FK to mount.MountNum.</summary>
		public long MountNum;
		/// <summary>The x position, in pixels, of the item on the mount.</summary>
		public int Xpos;
		/// <summary>The y position, in pixels, of the item on the mount.</summary>
		public int Ypos;
		/// <summary>The ordinal position of the item on the mount. 1-indexed because users see it.</summary>
		public int ItemOrder;
		/// <summary>The width, in pixels, of the mount item rectangle.</summary>
		public int Width;
		/// <summary>The height, in pixels, of the mount item rectangle.</summary>
		public int Height;
		///<summary>0,90,180,or 270.</summary>
		public int RotateOnAcquire;
		///<summary>An optional list of tooth numbers. In Db, rigorously formatted as American numbers, and separated by commas.  For display, uses hyphens for sequences.  Very likely supports international tooth numbers, but not tested for that.</summary>
		public string ToothNumbers;

		///<summary></summary>
		public MountItem Copy() {
			return (MountItem)this.MemberwiseClone();
		}

		public override string ToString(){
			return "ItemOrder:"+ItemOrder.ToString();
		}

	}
}
