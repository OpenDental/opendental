using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpReceivablesBreakdown {
		///<summary></summary>
		public static DataTable GetRecvBreakdownTable(DateTime dateStart,List<long> listProvNums,bool isWriteoffPay,bool isPayPlan2,string wDate
			,string eDate,string bDate,string tableName) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,listProvNums,isWriteoffPay,isPayPlan2,wDate,eDate,bDate,tableName);
			}
			//-------------------------------------------------------------------------------------//
			// Create temperary tables for sorting data
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			string query="";
			string whereProv="";//used as the provider portion of the where clauses.
											//each whereProv needs to be set up separately for each query
			switch(tableName) {
				case "TableCharge":
					whereProv="";
					if(listProvNums.Count != 0) {
						whereProv+=" AND ProvNum IN("+string.Join(",",listProvNums)+") ";
					}
					//This query should only get called when PrefName.PayPlansVersion==AgeCreditsAndDebits
					query = "SELECT TranDate, SUM(Amt) Amt "
									+ "FROM ( "
										+ "SELECT procedurelog.ProcDate TranDate, "
										+ "SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)) Amt "
										+ "FROM procedurelog "
										+ "WHERE procedurelog.ProcDate >= '" + bDate + "' "
										+ "AND procedurelog.ProcDate < '" + eDate + "' "
										+ "AND procedurelog.ProcStatus = "+POut.Int((int)ProcStat.C)+" "
										+ whereProv
										+ "GROUP BY procedurelog.ProcDate ";
					query += "UNION ALL "
										+ "SELECT payplancharge.ChargeDate TranDate, "
										+ "-SUM(payplancharge.Principal) Amt "
										+ "FROM payplancharge "
										+ "WHERE payplancharge.ChargeDate >= '" + bDate + "' "
										+ "AND payplancharge.ChargeDate < '" + eDate + "' "
										+ "AND payplancharge.ChargeType = "+POut.Int((int)PayPlanChargeType.Credit)+" "
										+ whereProv
										+ "GROUP BY payplancharge.ChargeDate ";
					query += ")tran "
									+ "GROUP BY TranDate "
									+ "ORDER BY TranDate";
					break;
				case "TablePayPlanCharge":
					if(isPayPlan2) {
						whereProv="";
						if(listProvNums.Count != 0) {
						  whereProv+=" AND ProvNum IN("+string.Join(",",listProvNums)+") ";
						}
						query="SELECT payplancharge.ChargeDate, SUM(payplancharge.Principal + payplancharge.Interest) Amt "
						+ "FROM payplancharge "
						+ "WHERE payplancharge.ChargeType = " +POut.Int((int)PayPlanChargeType.Debit) +" "
						+ "AND payplancharge.ChargeDate >= '" + bDate + "' "
						+ "AND payplancharge.ChargeDate < '" + eDate + "' "
						+ whereProv
						+ "GROUP BY payplancharge.ChargeDate "
						+ "ORDER BY payplancharge.ChargeDate ";
					}
					break;
				case "TableCapWriteoff":
					whereProv = "";
					if(listProvNums.Count != 0) {
						whereProv+=" AND claimproc.ProvNum IN("+string.Join(",",listProvNums)+") ";
					}
					if(isWriteoffPay) {
						query = "SELECT DateCP, "
											+ "SUM(WriteOff) FROM claimproc WHERE "
											+ "DateCP >= '" + bDate + "' "
											+ "AND DateCP < '" + eDate + "' "
											+ "AND Status = '7' "//CapComplete
											+ whereProv
											+ " GROUP BY DateCP "
											+ "ORDER BY DateCP";
					}
					else {
						query = "SELECT ProcDate, "
											+ "SUM(WriteOff) FROM claimproc WHERE "
											+ "ProcDate >= '" + bDate + "' "
											+ "AND ProcDate < '" + eDate + "' "
											+ "AND Status = '7' "//CapComplete
											+ whereProv
											+ " GROUP BY ProcDate "
											+ "ORDER BY ProcDate";
					}
					break;
				case "TableInsWriteoff":
					whereProv = "";
					if(listProvNums.Count != 0) {
						whereProv+=" AND ProvNum IN("+string.Join(",",listProvNums)+") ";
					}
					if(isWriteoffPay) {
						query = "SELECT DateCP, "
											+ "SUM(WriteOff) FROM claimproc WHERE "
											+ "DateCP >= '" + bDate + "' "
											+ "AND DateCP < '" + eDate + "' "
											+ "AND Status IN (1,4,5) "//Received, supplemental, capclaim. Otherwise, it's only an estimate. 7-CapCompl handled above.
											+ whereProv
											+ " GROUP BY DateCP "
											+ "ORDER BY DateCP";
					}
					else {
						query = "SELECT ProcDate, "
											+ "SUM(WriteOff) FROM claimproc WHERE "
											+ "ProcDate >= '" + bDate + "' "
											+ "AND ProcDate < '" + eDate + "' "
											+ "AND Status IN (0,1,4,5) " //Notreceived, received, supplemental, capclaim. 7-CapCompl handled above.
											+ whereProv
											+ " GROUP BY ProcDate "
											+ "ORDER BY ProcDate";
					}
					break;
				case "TablePay":
					whereProv = "";
					if(listProvNums.Count != 0) {
						whereProv+=" AND paysplit.ProvNum IN("+string.Join(",",listProvNums)+") ";
					}
					query="SELECT paysplit.DatePay,SUM(paysplit.splitamt) FROM paysplit "
							+ "WHERE paysplit.DatePay >= '" + bDate + "' "
							+ "AND paysplit.DatePay < '" + eDate + "' ";
							if(listHiddenUnearnedDefNums.Count>0) {
								query+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
							}
							if(!isPayPlan2) {
								query+="AND paysplit.PayPlanNum=0 ";
							}
							query+=whereProv
							+ " GROUP BY paysplit.DatePay ORDER BY DatePay";
					break;
				case "TableIns":
					whereProv = "";
					if(listProvNums.Count != 0) {
						whereProv+=" AND claimproc.ProvNum IN("+string.Join(",",listProvNums)+") ";
					}
					query = "SELECT DateCP,SUM(InsPayamt) "
									+ "FROM claimproc WHERE "
									+ "Status IN (1,4,5,7) "//Received, supplemental, capclaim, capcomplete.
									+ "AND DateCP >= '" + bDate + "' "
									+ "AND DateCP < '" + eDate + "' ";
									if(!isPayPlan2) {
										query+= "AND claimproc.PayPlanNum=0 ";
									}
									query+= whereProv
									+ " GROUP BY DateCP ORDER BY DateCP";

					break;
				case "TableAdj":
					whereProv = "";
					if(listProvNums.Count != 0) {
						whereProv+=" AND ProvNum IN("+string.Join(",",listProvNums)+") ";
					}
					query = "SELECT adjdate, SUM(adjamt) FROM adjustment WHERE "
									+ "adjdate >= '" + bDate + "' "
									+ "AND adjdate < '" + eDate + "' "
									+ whereProv
									+ " GROUP BY adjdate ORDER BY adjdate";
					break;
				case "TableProduction":
					whereProv="";
					if(listProvNums.Count != 0) {
						whereProv+=" AND ProvNum IN("+string.Join(",",listProvNums)+") ";
					}
					query="SELECT TranDate, SUM(Amt) Amt "
						+"FROM ( "
						+"SELECT procedurelog.ProcDate TranDate, "
						+"SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)) Amt "
						+"FROM procedurelog "
						+"WHERE procedurelog.ProcDate >= '" + bDate + "' "
						+"AND procedurelog.ProcDate < '" + eDate + "' "
						+"AND procedurelog.ProcStatus = "+POut.Int((int)ProcStat.C)+" "
						+whereProv
						+"GROUP BY procedurelog.ProcDate "
						+")tran "
						+"GROUP BY TranDate "
						+"ORDER BY TranDate ";
					break;
				case "TablePayPlanCredit":
					whereProv="";
					if(listProvNums.Count != 0) {
						whereProv+=" AND ProvNum IN("+string.Join(",",listProvNums)+") ";
					}
					query="SELECT payplancharge.ChargeDate TranDate, "
						+"SUM(payplancharge.Principal) Amt "
						+"FROM payplancharge "
						+"WHERE payplancharge.ChargeDate >= '" + bDate + "' "
						+"AND payplancharge.ChargeDate < '" + eDate + "' "
						+"AND payplancharge.ChargeType = "+POut.Int((int)PayPlanChargeType.Credit)+" "
						+whereProv
						+"GROUP BY payplancharge.ChargeDate "
						+"ORDER BY payplancharge.ChargeDate ";
					break;
				}
				return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
		}	
	}	
}
