using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>A group of ortho hardware that allows for faster entry than one tooth at a time. Changes to this table do not affect any patient records.</summary>
	[Serializable]
	public class OrthoRx:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoRxNum;
		///<summary>FK to orthohardwarespec.OrthoHardwareSpecNum. Description comes from here.</summary>
		public long OrthoHardwareSpecNum;
		///<summary>The description used for picking the prescription from a list.</summary>
		public string Description;
		///<summary>Tooth numbers stored here are always stored in Universal (1-32) notation. They are displayed to the user as Palmer notation. For brackets and elastics, always use tooth numbers separated by commas, like 2,3,4,5,6. For wires, must use a range like 2-15.</summary>
		public string ToothRange;
		///<summary>0 indexed. User controls it with arrows.</summary>
		public int ItemOrder;
		
		///<summary></summary>
		public OrthoRx Copy() {
			return (OrthoRx)this.MemberwiseClone();
		}

	}
}
