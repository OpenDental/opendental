using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class DashboardLayouts {
		public static List<DashboardLayout> GetDashboardLayout(string dashboardGroupName = "") {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DashboardLayout>>(MethodBase.GetCurrentMethod(),dashboardGroupName);
			}
			string command="SELECT * FROM dashboardlayout";
			List<DashboardLayout> layouts=Crud.DashboardLayoutCrud.SelectMany(command);
			if(!string.IsNullOrEmpty(dashboardGroupName)) { //Limit to a single group.
				layouts=layouts.FindAll(x => x.DashboardGroupName.ToLower()==dashboardGroupName.ToLower());
			}
			//Fill the non-db Cells field.
			List<DashboardCell> cells=DashboardCells.GetAll();
			foreach(DashboardLayout layout in layouts) {
				layout.Cells=cells.FindAll(x => x.DashboardLayoutNum==layout.DashboardLayoutNum);
			}
			return layouts;
		}

		///<summary>Inserts the given dashboard layouts and cells into the database.</summary>
		public static void SetDashboardLayout(List<DashboardLayout> layouts,string dashboardGroupName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),layouts,dashboardGroupName);
				return;
			}
			//Get all old layouts.
			List<DashboardLayout> layoutsDbAll=GetDashboardLayout();
			//Get all old layouts for this group.
			List<DashboardLayout> layoutsDbGroup=layoutsDbAll.FindAll(x => x.DashboardGroupName.ToLower()==dashboardGroupName.ToLower());
			//Delete all cells from old dashboard group.
			layoutsDbGroup.SelectMany(x => x.Cells).ToList().ForEach(x => Crud.DashboardCellCrud.Delete(x.DashboardCellNum));
			//Delete all layouts from old dashboard group.
			layoutsDbGroup.ForEach(x => Crud.DashboardLayoutCrud.Delete(x.DashboardLayoutNum));
			List<DashboardCell> cellsDb=DashboardCells.GetAll();
			foreach(DashboardLayout layout in layouts) {
				layout.DashboardGroupName=dashboardGroupName;
				//Delete old tab if it exists.
				layoutsDbAll
					.FindAll(x => x.DashboardLayoutNum==layout.DashboardLayoutNum)
					.ForEach(x => Crud.DashboardLayoutCrud.Delete(x.DashboardLayoutNum));
				//Delete old cells which belonged to this tab if they exist.
				cellsDb
					.FindAll(x => x.DashboardLayoutNum==layout.DashboardLayoutNum)
					.ForEach(x => Crud.DashboardCellCrud.Delete(x.DashboardCellNum));
				//Insert new tab.
				long layoutNumNew=Crud.DashboardLayoutCrud.Insert(layout);
				//Insert link cells to new tab and insert.
				layout.Cells.ForEach(x => { x.DashboardLayoutNum=layoutNumNew; Crud.DashboardCellCrud.Insert(x); });				
			}
		}

		/*
Only pull out the methods below as you need them.  Otherwise, leave them commented out.

///<summary></summary>
public static List<DashboardLayout> Refresh(long patNum){
	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		return Meth.GetObject<List<DashboardLayout>>(MethodBase.GetCurrentMethod(),patNum);
	}
	string command="SELECT * FROM dashboardlayout WHERE PatNum = "+POut.Long(patNum);
	return Crud.DashboardLayoutCrud.SelectMany(command);
}

///<summary>Gets one DashboardLayout from the db.</summary>
public static DashboardLayout GetOne(long dashboardLayoutNum){
	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
		return Meth.GetObject<DashboardLayout>(MethodBase.GetCurrentMethod(),dashboardLayoutNum);
	}
	return Crud.DashboardLayoutCrud.SelectOne(dashboardLayoutNum);
}

///<summary></summary>
public static long Insert(DashboardLayout dashboardLayout){
	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
		dashboardLayout.DashboardLayoutNum=Meth.GetLong(MethodBase.GetCurrentMethod(),dashboardLayout);
		return dashboardLayout.DashboardLayoutNum;
	}
	return Crud.DashboardLayoutCrud.Insert(dashboardLayout);
}

///<summary></summary>
public static void Update(DashboardLayout dashboardLayout){
	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
		Meth.GetVoid(MethodBase.GetCurrentMethod(),dashboardLayout);
		return;
	}
	Crud.DashboardLayoutCrud.Update(dashboardLayout);
}

///<summary></summary>
public static void Delete(long dashboardLayoutNum) {
	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		Meth.GetVoid(MethodBase.GetCurrentMethod(),dashboardLayoutNum);
		return;
	}
	Crud.DashboardLayoutCrud.Delete(dashboardLayoutNum);
}




*/
	}
}