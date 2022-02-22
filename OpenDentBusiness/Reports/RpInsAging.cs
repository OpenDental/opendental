using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	public class RpInsAging {
		public static DataTable GetInsAgingTable(RpAgingParamObject rpo) {			
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),rpo);
			}
			#region Insurance Aging
			string asOfDateStr=POut.Date(rpo.AsOfDate);
			string thirtyDaysAgo=POut.Date(rpo.AsOfDate.AddDays(-30));
			string sixtyDaysAgo=POut.Date(rpo.AsOfDate.AddDays(-60));
			string ninetyDaysAgo=POut.Date(rpo.AsOfDate.AddDays(-90));
			string patOrGuar=(rpo.IsGroupByFam?"guar":"patient");
			string command="SELECT guarAging.PatNum,";
			if(ReportsComplex.RunFuncOnReportServer(() => Prefs.GetBoolNoCache(PrefName.ReportsShowPatNum))) {
				command+=DbHelper.Concat("guarAging.PatNum","' - '","guarAging.LName","', '","guarAging.FName","' '","guarAging.MiddleI");
			}
			else {
				command+=DbHelper.Concat("guarAging.LName","', '","guarAging.FName","' '","guarAging.MiddleI");
			}
			command+=@" PatName,"+(rpo.IsDetailedBreakdown ? "guarAging.CarrierName,guarAging.CarrierNum,guarAging.GroupName,guarAging.PlanNum," : "")
				+@"guarAging.InsPayEst_0_30,guarAging.InsPayEst_31_60,guarAging.InsPayEst_61_90,guarAging.InsPayEst_90,guarAging.InsPayEst_Total
				FROM (
					SELECT "+patOrGuar+".PatNum,"+patOrGuar+".LName,"+patOrGuar+".FName,"+patOrGuar+@".MiddleI,"
					+(rpo.IsDetailedBreakdown ? "carrier.CarrierName,carrier.CarrierNum,insplan.GroupName,insplan.PlanNum," : "")
					+@"SUM(CASE WHEN cp.ProcDate >= "+thirtyDaysAgo+@" THEN cp.InsPayEst ELSE 0 END) InsPayEst_0_30,
					SUM(CASE WHEN cp.ProcDate < "+thirtyDaysAgo+" AND cp.ProcDate >= "+sixtyDaysAgo+@" THEN cp.InsPayEst ELSE 0 END) InsPayEst_31_60,
					SUM(CASE WHEN cp.ProcDate < "+sixtyDaysAgo+" AND cp.ProcDate >= "+ninetyDaysAgo+@" THEN cp.InsPayEst ELSE 0 END) InsPayEst_61_90,
					SUM(CASE WHEN cp.ProcDate < "+ninetyDaysAgo+@" THEN cp.InsPayEst ELSE 0 END) InsPayEst_90,
					SUM(cp.InsPayEst) InsPayEst_Total
					FROM claimproc cp
					INNER JOIN patient ON patient.PatNum=cp.PatNum"
					+(rpo.IsGroupByFam ? @"
					INNER JOIN patient guar ON patient.Guarantor=guar.PatNum" : "")
					+(rpo.IsDetailedBreakdown ? @"
					LEFT JOIN insplan ON insplan.PlanNum=cp.PlanNum
					LEFT JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum" : "");
			command+=@"
				WHERE cp.Status IN ("+(int)ClaimProcStatus.NotReceived+","+(int)ClaimProcStatus.Received+@")
				AND cp.ProcDate <= "+asOfDateStr+@"
				AND (cp.Status = "+(int)ClaimProcStatus.NotReceived+" OR (cp.Status = "+(int)ClaimProcStatus.Received+" AND cp.DateCP > "+asOfDateStr+")) ";
			if(rpo.IsDetailedBreakdown) {
				if(!string.IsNullOrWhiteSpace(rpo.CarrierNameFilter)) {
					command+=@"
						AND (carrier.CarrierNum IS NULL OR carrier.CarrierName LIKE '%"+rpo.CarrierNameFilter+"%') ";
				}
				if(!string.IsNullOrWhiteSpace(rpo.GroupNameFilter)) {
					command+=@"
						AND (insplan.GroupName IS NULL OR insplan.GroupName LIKE '%"+rpo.GroupNameFilter+"%') ";
				}
			}
			if(rpo.ListBillTypes.Count>0){//if all bill types is selected, list will be empty
				command+="AND "+patOrGuar+".BillingType IN ("+string.Join(",",rpo.ListBillTypes.Select(x => POut.Long(x)))+") ";
			}
			if(rpo.ListProvNums.Count>0) {//if all provs is selected, list will be empty
				command+="AND "+patOrGuar+".PriProv IN ("+string.Join(",",rpo.ListProvNums.Select(x => POut.Long(x)))+") ";
			}
			if(rpo.ListClinicNums.Count>0) {//listClin may contain "Unassigned" clinic with ClinicNum 0, in which case it will also be in the query string
				command+="AND "+patOrGuar+".ClinicNum IN ("+string.Join(",",rpo.ListClinicNums.Select(x => POut.Long(x)))+") ";
			}
			command+="GROUP BY "+patOrGuar+".PatNum"
				+(rpo.GroupByCarrier ? ",carrier.CarrierName" : "")
				+(rpo.GroupByGroupName ? ",insplan.GroupName" : "")+@") guarAging
				WHERE (guarAging.InsPayEst_0_30 > 0.005
					OR guarAging.InsPayEst_31_60 > 0.005
					OR guarAging.InsPayEst_61_90 > 0.005
					OR guarAging.InsPayEst_90 > 0.005
					OR guarAging.InsPayEst_Total > 0.005)
				ORDER BY guarAging.LName,guarAging.FName";
			ReportComplexEvent.Fire(ODEventType.ReportComplex,Lans.g("ReportComplex","Running Insurance Estimate Query..."));
			DataTable insTable = ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
			#endregion Insurance Aging
			#region Regular Aging
			DataTable regAging=new DataTable();
			//Don't run regular aging if detailed breakdown as it can take a long time to run for large customers.
			if(!rpo.IsDetailedBreakdown) {
				regAging=ReportsComplex.RunFuncOnReportServer(() => RpAging.GetAgingTable(rpo));
			}
			#endregion Regular Aging
			#region Merge Insurance and Regular Aging
			DataTable insAgingTable = new DataTable();
			insAgingTable.Columns.Add("PatName");
			if(rpo.IsDetailedBreakdown) {
				insAgingTable.Columns.Add("CarrierName");
				insAgingTable.Columns.Add("GroupName");
			}
			insAgingTable.Columns.Add("InsPayEst_0_30");
			insAgingTable.Columns.Add("InsPayEst_31_60");
			insAgingTable.Columns.Add("InsPayEst_61_90");
			insAgingTable.Columns.Add("InsPayEst_90");
			insAgingTable.Columns.Add("InsPayEst_Total");
			insAgingTable.Columns.Add("PatBal_0_30");
			insAgingTable.Columns.Add("PatBal_31_60");
			insAgingTable.Columns.Add("PatBal_61_90");
			insAgingTable.Columns.Add("PatBal_90");
			insAgingTable.Columns.Add("PatBal_Total");
			insAgingTable.Columns.Add("InsWoChange");
			insAgingTable.Columns.Add("PatBalEst");
			Dictionary<long,DataRow> dictPatInsAgingRows=new Dictionary<long,DataRow>();
			Dictionary<AgingTableRowId,DataRow> dictDetailedInsAgingRows=new Dictionary<AgingTableRowId,DataRow>();
			#region Add All Insurance Aging Rows to Dictionary
			foreach(DataRow insRow in insTable.Rows) {
				DataRow newRow=insAgingTable.NewRow();//create a new row with the structure of the new table
				//copy the ins aging table's values over to the new row and fill the pat bal columns with -insPayEst for the appropriate bucket
				newRow["PatName"]=insRow["PatName"];
				if(rpo.IsDetailedBreakdown) {
					newRow["CarrierName"]=insRow["CarrierName"];
					newRow["GroupName"]=insRow["GroupName"];
				}
				newRow["InsPayEst_0_30"]=insRow["InsPayEst_0_30"];
				newRow["InsPayEst_31_60"]=insRow["InsPayEst_31_60"];
				newRow["InsPayEst_61_90"]=insRow["InsPayEst_61_90"];
				newRow["InsPayEst_90"]=insRow["InsPayEst_90"];
				newRow["InsPayEst_Total"]=insRow["InsPayEst_Total"];
				dictPatInsAgingRows[PIn.Long(insRow["PatNum"].ToString())]=newRow;//PatNum only used to link insAgingRows to regAgingRows
				dictDetailedInsAgingRows[AgingTableRowId.FromDataRow(insRow)]=newRow;//Only used for Detailed report.
			}
			#endregion Add All Insurance Aging Rows to Dictionary
			#region Add Regular Aging Rows and Apply Insurance Estimates to Dictionary
			foreach(DataRow row in regAging.Rows) {
				long patNumCur=PIn.Long(row["PatNum"].ToString());
				DataRow insAgingRow;
				if(dictPatInsAgingRows.TryGetValue(patNumCur,out insAgingRow)) {
					//check to see if that patient exists in the insurance aging report
					insAgingRow["PatBal_0_30"]=PIn.Double(row["Bal_0_30"].ToString()) - PIn.Double(insAgingRow["InsPayEst_0_30"].ToString());
					insAgingRow["PatBal_31_60"]=PIn.Double(row["Bal_31_60"].ToString()) - PIn.Double(insAgingRow["InsPayEst_31_60"].ToString());
					insAgingRow["PatBal_61_90"]=PIn.Double(row["Bal_61_90"].ToString()) - PIn.Double(insAgingRow["InsPayEst_61_90"].ToString());
					insAgingRow["PatBal_90"]=PIn.Double(row["BalOver90"].ToString()) - PIn.Double(insAgingRow["InsPayEst_90"].ToString());
					insAgingRow["PatBal_Total"]=PIn.Double(row["BalTotal"].ToString()) - PIn.Double(insAgingRow["InsPayEst_Total"].ToString());
					insAgingRow["InsWoChange"]=PIn.Double(row["InsWoEst"].ToString());
					insAgingRow["PatBalEst"]=PIn.Double(insAgingRow["PatBal_Total"].ToString()) - PIn.Double(insAgingRow["InsWoChange"].ToString());
				}
				else {//if pat doesn't exist in ins aging report, create a new row with 0.00 insurance values and fill the patient aging values
					insAgingRow=insAgingTable.NewRow();
					insAgingRow["PatName"]=row["PatName"];
					insAgingRow["InsPayEst_0_30"]=PIn.Double("0.00");
					insAgingRow["InsPayEst_31_60"]=PIn.Double("0.00");
					insAgingRow["InsPayEst_61_90"]=PIn.Double("0.00");
					insAgingRow["InsPayEst_90"]=PIn.Double("0.00");
					insAgingRow["InsPayEst_Total"]=PIn.Double("0.00");
					insAgingRow["PatBal_0_30"]=PIn.Double(row["Bal_0_30"].ToString());
					insAgingRow["PatBal_31_60"]=PIn.Double(row["Bal_31_60"].ToString());
					insAgingRow["PatBal_61_90"]=PIn.Double(row["Bal_61_90"].ToString());
					insAgingRow["PatBal_90"]=PIn.Double(row["BalOver90"].ToString());
					insAgingRow["PatBal_Total"]=PIn.Double(row["BalTotal"].ToString());
					insAgingRow["InsWoChange"]=PIn.Double(row["InsWoEst"].ToString());
					insAgingRow["PatBalEst"]=PIn.Double(row["BalTotal"].ToString())-PIn.Double(row["InsWoEst"].ToString())-PIn.Double(row["InsPayEst"].ToString());
					dictPatInsAgingRows[patNumCur]=insAgingRow;
				}
			}
			#endregion Add Regular Aging Rows and Apply Insurance Estimates to Dictionary
			dictPatInsAgingRows=dictPatInsAgingRows.OrderBy(x => x.Value["PatName"]).ToDictionary(x => x.Key,x => x.Value);
			#endregion Merge Insurance and Regular Aging
			#region Add Rows to Table Filtered by Age
			if(rpo.IsDetailedBreakdown) {//Remove patient columns for detailed breakdown
				AddRowsFromDict<AgingTableRowId>(rpo,dictDetailedInsAgingRows,insAgingTable);
				insAgingTable.Columns.Remove("PatBal_0_30");
				insAgingTable.Columns.Remove("PatBal_31_60");
				insAgingTable.Columns.Remove("PatBal_61_90");
				insAgingTable.Columns.Remove("PatBal_90");
				insAgingTable.Columns.Remove("PatBal_Total");
				insAgingTable.Columns.Remove("InsWoChange");
				insAgingTable.Columns.Remove("PatBalEst");
			}
			else {
				AddRowsFromDict<long>(rpo,dictPatInsAgingRows,insAgingTable);
			}
			#endregion Add Rows to Table Filtered by Age
			return insAgingTable;
		}

		private static void AddRowsFromDict<T>(RpAgingParamObject rpo,Dictionary<T,DataRow> dict,DataTable insAgingTable) {
			foreach(DataRow rowCur in dict.Values) {
				double insPayEstTotal = PIn.Double(rowCur["InsPayEst_Total"].ToString());
				double patBalTotal = PIn.Double(rowCur["PatBal_Total"].ToString())+insPayEstTotal;
				if(patBalTotal <= -0.005) {
					insAgingTable.Rows.Add(rowCur);
					continue;
				}
				double insWoChange = PIn.Double(rowCur["InsWoChange"].ToString());
				double patBal0_30 = PIn.Double(rowCur["PatBal_0_30"].ToString())+PIn.Double(rowCur["InsPayEst_0_30"].ToString());
				double patBal31_60 = PIn.Double(rowCur["PatBal_31_60"].ToString())+PIn.Double(rowCur["InsPayEst_31_60"].ToString());
				double patBal61_90 = PIn.Double(rowCur["PatBal_61_90"].ToString())+PIn.Double(rowCur["InsPayEst_61_90"].ToString());
				double patBal90 = PIn.Double(rowCur["PatBal_90"].ToString())+PIn.Double(rowCur["InsPayEst_90"].ToString());
				if((!CompareDouble.IsZero(insPayEstTotal) || !CompareDouble.IsZero(insWoChange)) 
					&& new[] { patBal0_30,patBal31_60,patBal61_90,patBal90 }.All(x => x < 0.005)) 
				{
					insAgingTable.Rows.Add(rowCur);
					continue;
				}
				if(patBal90 >= 0.005//always include if bal over 90
					|| (rpo.AccountAge<=AgeOfAccount.Over60 && patBal61_90 >= 0.005)//if age 60, 30, or Any, include if bal 61 to 90
					|| (rpo.AccountAge<=AgeOfAccount.Over30 && patBal31_60 >= 0.005)//if age 30 or Any, include if bal 31 to 60
					|| (rpo.AccountAge==AgeOfAccount.Any && patBal0_30 >= 0.005))//if Any age, include if bal 0 to 30
				{
					insAgingTable.Rows.Add(rowCur);
				}
			}
		}

		private class AgingTableRowId {
			public long PatNum;
			public long InsPlanNum;
			public long CarrierNum;

			public static AgingTableRowId FromDataRow(DataRow row) {
				AgingTableRowId retVal=new AgingTableRowId();
				if(row.Table.Columns.Contains("PatNum")) {
					retVal.PatNum=PIn.Long(row["PatNum"].ToString());
				}
				if(row.Table.Columns.Contains("PlanNum")) {
					retVal.InsPlanNum=PIn.Long(row["PlanNum"].ToString());
				}
				if(row.Table.Columns.Contains("CarrierNum")) {
					retVal.CarrierNum=PIn.Long(row["CarrierNum"].ToString());
				}
				return retVal;
			}
		}
	}
}
