using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary>Insert, Update, Delete are all managed by DashboardLayouts. The 2 classes are tightly coupled and should not be modified separately.</summary>
	public class DashboardCells{
		///<summary></summary>
		public static List<DashboardCell> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DashboardCell>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM dashboardcell";
			return Crud.DashboardCellCrud.SelectMany(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static long Insert(DashboardCell dashboardCell) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				dashboardCell.DashboardCellNum=Meth.GetLong(MethodBase.GetCurrentMethod(),dashboardCell);
				return dashboardCell.DashboardCellNum;
			}
			return Crud.DashboardCellCrud.Insert(dashboardCell);
		}

		///<summary></summary>
		public static void Delete(long dashboardCellNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dashboardCellNum);
				return;
			}
			Crud.DashboardCellCrud.Delete(dashboardCellNum);
		}

		///<summary>Gets one DashboardCell from the db.</summary>
		public static DashboardCell GetOne(long dashboardCellNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<DashboardCell>(MethodBase.GetCurrentMethod(),dashboardCellNum);
			}
			return Crud.DashboardCellCrud.SelectOne(dashboardCellNum);
		}


		///<summary></summary>
		public static void Update(DashboardCell dashboardCell){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dashboardCell);
				return;
			}
			Crud.DashboardCellCrud.Update(dashboardCell);
		}

		

		

		
		*/
	}
}