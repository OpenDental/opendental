using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

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
		/// <summary>The ordinal position of the item on the mount. 1-indexed because users see it. 0 if TestShowing has a value.</summary>
		public int ItemOrder;
		///<summary>0,90,180,or 270.</summary>
		public int RotateOnAcquire;
		/// <summary>An optional list of tooth numbers. In Db, rigorously formatted as American numbers, and separated by commas.  For display, uses hyphens for sequences.  Very likely supports international tooth numbers, but not tested for that.</summary>
		public string ToothNumbers;
		///<summary>Instead of an image, a mount item can show text. In this case, ItemOrder=0. Text color and background will be the mount default.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string TextShowing;
		///<summary>This could vary significantly based on the size of the mount.  It's always relative to mount pixels.</summary>
		public float FontSize;

		///<summary></summary>
		public MountItemDef Copy() {
			return (MountItemDef)this.MemberwiseClone();
		}

		public override string ToString(){
			if(TextShowing!=""){
				return TextShowing;
			}
			return ItemOrder.ToString();
		}

	}

		



		
	

	

	


}










