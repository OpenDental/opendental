using System;
using System.Collections;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	///<summary>One row per standard report.</summary>
	[Serializable()]
	[CrudTable(IsSynchable = true)]
	public class DisplayReport:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DisplayReportNum;
		///<summary>.</summary>
		public string InternalName;
		///<summary>.</summary>
		public int ItemOrder;
		///<summary>.</summary>
		public string Description;
		///<summary>Enum:DisplayReportCategory 0 - ProdInc; 1 - Daily, 2 - Monthly, 3 - Lists, 4 - PublicHealth, 5 - ArizonaPrimaryCare.</summary>
		public DisplayReportCategory Category;
		///<summary>.</summary>
		public bool IsHidden;
		///<summary>When true and IsHidden is false, will show this report in a pop out sub menu.</summary>
		public bool IsVisibleInSubMenu;

		///<summary>Returns a copy of the display report.</summary>
    public DisplayReport Copy(){
			return (DisplayReport)this.MemberwiseClone(); 
		}


	}

	
		public enum DisplayReportCategory {
		///<summary>0 - Production and Income reports</summary>
		ProdInc,
		///<summary>1 - Daily reports</summary>
		Daily,
		///<summary>2 - Monthly reports</summary>
		Monthly,
		///<summary>3 - List reports</summary>
		Lists,
		///<summary>4 - Public Health reports</summary>
		PublicHealth,
		///<summary>5 - Arizona Primary care reports</summary>
		ArizonaPrimaryCare,
	}
	

	

}









