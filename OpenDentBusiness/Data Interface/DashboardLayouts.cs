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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DashboardLayout>>(MethodBase.GetCurrentMethod(),dashboardGroupName);
			}
			string command="SELECT * FROM dashboardlayout";
			List<DashboardLayout> listDashboardLayouts=Crud.DashboardLayoutCrud.SelectMany(command);
			if(!string.IsNullOrEmpty(dashboardGroupName)) { //Limit to a single group.
				listDashboardLayouts=listDashboardLayouts.FindAll(x => x.DashboardGroupName.ToLower()==dashboardGroupName.ToLower());
			}
			//Fill the non-db Cells field.
			List<DashboardCell> listDashboardCells=DashboardCells.GetAll();
			for(int i=0;i<listDashboardLayouts.Count;i++) {
				listDashboardLayouts[i].Cells=listDashboardCells.FindAll(x => x.DashboardLayoutNum==listDashboardLayouts[i].DashboardLayoutNum);
			}
			return listDashboardLayouts;
		}

		///<summary>Inserts the given dashboard layouts and cells into the database.</summary>
		public static void SetDashboardLayout(List<DashboardLayout> listDashboardLayouts,string dashboardGroupName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDashboardLayouts,dashboardGroupName);
				return;
			}
			//Get all old layouts.
			List<DashboardLayout> listDashboardLayoutsDbAll=GetDashboardLayout();
			//Get all old layouts for this group.
			List<DashboardLayout> listDashboardLayoutsDbGroup=listDashboardLayoutsDbAll.FindAll(x => x.DashboardGroupName.ToLower()==dashboardGroupName.ToLower());
			//Delete all cells from old dashboard group.
			List<DashboardCell> listDashboardCells=listDashboardLayoutsDbGroup.SelectMany(x => x.Cells).ToList();
			for(int i=0;i<listDashboardCells.Count;i++){
				Crud.DashboardCellCrud.Delete(listDashboardCells[i].DashboardCellNum);
			}
			//Delete all layouts from old dashboard group.
			for(int i=0;i<listDashboardLayoutsDbGroup.Count;i++){
				Crud.DashboardLayoutCrud.Delete(listDashboardLayoutsDbGroup[i].DashboardLayoutNum);
			}
			List<DashboardCell> listDashboardCellsDb=DashboardCells.GetAll();
			for(int i=0;i<listDashboardLayouts.Count;i++){
				listDashboardLayouts[i].DashboardGroupName=dashboardGroupName;
				//Delete old tab if it exists.
				listDashboardLayoutsDbAll
					.FindAll(x => x.DashboardLayoutNum==listDashboardLayouts[i].DashboardLayoutNum)
					.ForEach(x => Crud.DashboardLayoutCrud.Delete(x.DashboardLayoutNum));
				//Delete old cells which belonged to this tab if they exist.
				listDashboardCellsDb
					.FindAll(x => x.DashboardLayoutNum==listDashboardLayouts[i].DashboardLayoutNum)
					.ForEach(x => Crud.DashboardCellCrud.Delete(x.DashboardCellNum));
				//Insert new tab.
				long layoutNumNew=Crud.DashboardLayoutCrud.Insert(listDashboardLayouts[i]);
				//Insert link cells to new tab and insert.
				listDashboardLayouts[i].Cells.ForEach(x => { x.DashboardLayoutNum=layoutNumNew; Crud.DashboardCellCrud.Insert(x); });
			}
		}

		/*
Only pull out the methods below as you need them.  Otherwise, leave them commented out.

///<summary></summary>
public static List<DashboardLayout> Refresh(long patNum){
	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		return Meth.GetObject<List<DashboardLayout>>(MethodBase.GetCurrentMethod(),patNum);
	}
	string command="SELECT * FROM dashboardlayout WHERE PatNum = "+POut.Long(patNum);
	return Crud.DashboardLayoutCrud.SelectMany(command);
}

///<summary>Gets one DashboardLayout from the db.</summary>
public static DashboardLayout GetOne(long dashboardLayoutNum){
	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		return Meth.GetObject<DashboardLayout>(MethodBase.GetCurrentMethod(),dashboardLayoutNum);
	}
	return Crud.DashboardLayoutCrud.SelectOne(dashboardLayoutNum);
}

///<summary></summary>
public static long Insert(DashboardLayout dashboardLayout){
	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		dashboardLayout.DashboardLayoutNum=Meth.GetLong(MethodBase.GetCurrentMethod(),dashboardLayout);
		return dashboardLayout.DashboardLayoutNum;
	}
	return Crud.DashboardLayoutCrud.Insert(dashboardLayout);
}

///<summary></summary>
public static void Update(DashboardLayout dashboardLayout){
	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		Meth.GetVoid(MethodBase.GetCurrentMethod(),dashboardLayout);
		return;
	}
	Crud.DashboardLayoutCrud.Update(dashboardLayout);
}

///<summary></summary>
public static void Delete(long dashboardLayoutNum) {
	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		Meth.GetVoid(MethodBase.GetCurrentMethod(),dashboardLayoutNum);
		return;
	}
	Crud.DashboardLayoutCrud.Delete(dashboardLayoutNum);
}




*/
	}
}