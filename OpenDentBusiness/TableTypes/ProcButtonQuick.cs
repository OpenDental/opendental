using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Used to customize quick buttons in the chart module.</summary>
	[Serializable]
	public class ProcButtonQuick:TableBase {
	   ///<summary>Primary Key.</summary>
	   [CrudColumn(IsPriKey=true)]
	   public long ProcButtonQuickNum;
	   ///<summary>Description used for display.</summary>
	   public string Description;
	   ///<summary>FK to procedurecode.ProcCode. </summary>
	   public string CodeValue;
	   ///<summary>Surfaces. </summary>
	   public string Surf;
	   ///<summary>Zero based YPos, row number within panel.</summary>
	   public int YPos;
	   ///<summary>Items within each row are sorted using item order. Smallest item order will be drawn on the left.</summary>
	   public int ItemOrder;
	   ///<summary>If true, this "button" will be displayed as a label.</summary>
	   public bool IsLabel;


	   ///<summary></summary>
	   public ProcButtonQuick Copy() {
		   return (ProcButtonQuick)this.MemberwiseClone();
	   }

		///<summary>Returns true if all members are equal. This compares member values instead of comparing memory addresses.</summary>
		public bool Equals(ProcButtonQuick other) {
			if(ProcButtonQuickNum==other.ProcButtonQuickNum
				&& Description ==other.Description
				&& CodeValue   ==other.CodeValue  
				&& Surf        ==other.Surf		  
				&& ItemOrder   ==other.ItemOrder		  
				&& YPos        ==other.YPos		  
				&& IsLabel     ==other.IsLabel) {
				return true;
			}
			return false;
		}

	}
}