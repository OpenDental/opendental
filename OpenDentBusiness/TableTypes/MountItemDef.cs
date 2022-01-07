using System;
using System.Collections;

namespace OpenDentBusiness{
	/// <summary>These are always attached to mountdefs.  Can be deleted without any problems.</summary>
	[Serializable()]
	public class MountItemDef : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MountItemDefNum;
		/// <summary>FK to mountdef.MountDefNum.</summary>
		public long MountDefNum;
		/// <summary>The x position, in pixels, of the item on the mount.</summary>
		public int Xpos;
		/// <summary>The y position, in pixels, of the item on the mount.</summary>
		public int Ypos;
		/// <summary>Width, in pixels, of the item rectangle on the mount.  Any cropping, rotating, etc, will all be defined in the original image itself.</summary>
		public int Width;
		/// <summary>Height, in pixels, of the item rectangle on the mount.  Any cropping, rotating, etc, will all be defined in the original image itself.</summary>
		public int Height;
		/// <summary>The ordinal position of the item on the mount. 1-indexed because users see it.</summary>
		public int ItemOrder;
		///<summary>0,90,180,or 270.</summary>
		public int RotateOnAcquire;
		/// <summary>An optional list of tooth numbers. In Db, rigorously formatted as American numbers, and separated by commas.  For display, uses hyphens for sequences.  Very likely supports international tooth numbers, but not tested for that.</summary>
		public string ToothNumbers;

		///<summary></summary>
		public MountItemDef Copy() {
			return (MountItemDef)this.MemberwiseClone();
		}

		
	}

		



		
	

	

	


}










