using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Corresponds to one tab full of display fields inside the ortho chart.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class OrthoChartTab:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoChartTabNum;
		///<summary>The description of the tab which shows in the UI.  User editable.</summary>
		public string TabName;
		///<summary>Item order of the tabs.  This is how they will display within the Ortho Chart window.</summary>
		public int ItemOrder;
		///<summary></summary>
		public bool IsHidden;

		public OrthoChartTab Copy() {
			return (OrthoChartTab)this.MemberwiseClone();
		}

	}
}