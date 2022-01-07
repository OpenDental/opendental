using System;
using System.Collections.Generic;

namespace OpenDentBusiness {
	///<summary>Each tab in the dashboard has a corresponding DashboardLayout. DashboardLayout and DashboardCell work in conjunction to form the dashboard layout.</summary>
	[Serializable]
	public class DashboardLayout:TableBase {
		///<summary>PK.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DashboardLayoutNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>FK to usergroup.UserGroupNum.</summary>
		public long UserGroupNum;
		///<summary>Text shown in the tab header.</summary>
		public string DashboardTabName;
		///<summary>Orders the tabs in the tab control. 0 based.</summary>
		public int DashboardTabOrder;
		///<summary>Number of rows for this DashboardLayout. Min value of 1.</summary>
		public int DashboardRows;
		///<summary>Number of columns for this DashboardLayout. Min value of 1.</summary>
		public int DashboardColumns;
		///<summary>Groups multiple DashboardLayout(s) together.</summary>
		public string DashboardGroupName;		

		[CrudColumn(IsNotDbColumn=true)]
		public List<DashboardCell> Cells=new List<DashboardCell>();

		///<summary></summary>
		public DashboardLayout Copy() {
			return (DashboardLayout)this.MemberwiseClone();
		}
	}
}
