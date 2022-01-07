using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class DashboardARs{
		///<summary>Gets all rows gt= dateFrom.</summary>
		public static List<DashboardAR> Refresh(DateTime dateFrom){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DashboardAR>>(MethodBase.GetCurrentMethod(),dateFrom);
			}
			string command="SELECT * FROM dashboardar WHERE DateCalc >= "+POut.Date(dateFrom);
			return ReportsComplex.RunFuncOnReportServer(() => Crud.DashboardARCrud.SelectMany(command));
		}

		///<summary></summary>
		public static long Insert(DashboardAR dashboardAR){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				dashboardAR.DashboardARNum=Meth.GetLong(MethodBase.GetCurrentMethod(),dashboardAR);
				return dashboardAR.DashboardARNum;
			}
			return Crud.DashboardARCrud.Insert(dashboardAR);
		}

		///<summary>Dashboardar is safe to truncate because it gets refilled as needed and there are no FKeys to any other table.</summary>
		public static void Truncate() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command = "TRUNCATE dashboardar";
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one DashboardAR from the db.</summary>
		public static DashboardAR GetOne(long dashboardARNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<DashboardAR>(MethodBase.GetCurrentMethod(),dashboardARNum);
			}
			return Crud.DashboardARCrud.SelectOne(dashboardARNum);
		}

		///<summary></summary>
		public static void Update(DashboardAR dashboardAR){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dashboardAR);
				return;
			}
			Crud.DashboardARCrud.Update(dashboardAR);
		}

		///<summary></summary>
		public static void Delete(long dashboardARNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dashboardARNum);
				return;
			}
			string command= "DELETE FROM dashboardar WHERE DashboardARNum = "+POut.Long(dashboardARNum);
			Db.NonQ(command);
		}
		*/
	}
}