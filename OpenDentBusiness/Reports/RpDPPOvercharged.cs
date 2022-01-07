using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using DataConnectionBase;
using CodeBase;

namespace OpenDentBusiness {
	public class RpDPPOvercharged {

		public static DataTable GetDPPOvercharged(DateTime dateStart,DateTime dateEnd,List<long> listClinicNums,List<long> listProvNums,long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listClinicNums,listProvNums,patNum);
			}
			List<int> listClaimProcStatForInsEst=ClaimProcs.GetEstimatedStatuses().Select(x => (int)x).ToList();
			List<int> listClaimProcStatForInsPaid=ClaimProcs.GetInsPaidStatuses().Select(x => (int)x).ToList();
			string query="SELECT payplan.DatePayPlanStart,"
				+"CONCAT(pat.LName,', ',pat.FName) AS 'patientName',CONCAT(guar.LName,', ',guar.FName) AS 'guarName',provider.Abbr AS 'provAbbr',";
				if(PrefC.HasClinicsEnabled) {
					query+="COALESCE(clinic.Abbr,'unassigned') AS 'clinicAbbr',";
				}
				query+="CASE payplanlink.LinkType "
					+"WHEN "+POut.Int((int)PayPlanLinkType.Adjustment)+" THEN 'adjustment' "
					+"WHEN "+POut.Int((int)PayPlanLinkType.Procedure)+" THEN procedurecode.ProcCode "
					+"ELSE '' END AS 'description',"
				+"IF(payplanlink.AmountOverride!=0,'X','') AS 'overridden',"
				+"CASE payplanlink.LinkType "
					+"WHEN "+POut.Int((int)PayPlanLinkType.Adjustment)+" THEN adjustment.AdjAmt "
					+"WHEN "+POut.Int((int)PayPlanLinkType.Procedure)+" THEN ROUND(procedurelog.ProcFee*GREATEST(1,procedurelog.BaseUnits+procedurelog.UnitQty)"
						+"+COALESCE(sumprocadj.SumProcAdj,0)-COALESCE(sumins.SumIns,0),2) "
					+"ELSE 0 END "
				+"AS 'patPortion',"
				+"COALESCE(sumsplitoutside.SumSplitOutside,0) AS 'patPaidOutsidePlan',"
				+"IF(payplanlink.AmountOverride!=0,payplanlink.AmountOverride,ROUND("
					+"CASE payplanlink.LinkType "
						+"WHEN "+POut.Int((int)PayPlanLinkType.Adjustment)+" THEN adjustment.AdjAmt "
						+"WHEN "+POut.Int((int)PayPlanLinkType.Procedure)+" THEN procedurelog.ProcFee*GREATEST(1,procedurelog.BaseUnits+procedurelog.UnitQty)"
							+"+COALESCE(sumprocadj.SumProcAdj,0)-COALESCE(sumins.SumIns,0) "
						+"ELSE 0 END-COALESCE(sumsplitoutside.SumSplitOutside,0)"
				+",2)) AS 'patPortionOnPlan',"
				+"COALESCE(sumpayplancharge.SumPayPlanCharge,0) AS 'planDebits',"
				+"ABS(LEAST(ROUND(IF(payplanlink.AmountOverride!=0,payplanlink.AmountOverride,"
					+"CASE payplanlink.LinkType "
						+"WHEN "+POut.Int((int)PayPlanLinkType.Adjustment)+" THEN adjustment.AdjAmt "
						+"WHEN "+POut.Int((int)PayPlanLinkType.Procedure)+" THEN procedurelog.ProcFee*GREATEST(1,procedurelog.BaseUnits+procedurelog.UnitQty)"
							+"+COALESCE(sumprocadj.SumProcAdj,0)-COALESCE(sumins.SumIns,0) "
						+"ELSE 0 END-COALESCE(sumsplitoutside.SumSplitOutside,0))-COALESCE(sumpayplancharge.SumPayPlanCharge,0)"
				+",2),0)) AS 'amtOvercharged',"
				+"COALESCE(sumsplitonplan.SumSplitOnPlan,0) AS 'patPaidOnPlan',"
				+"ABS(LEAST(ROUND(IF(payplanlink.AmountOverride!=0,payplanlink.AmountOverride,"
					+"CASE payplanlink.LinkType "
						+"WHEN "+POut.Int((int)PayPlanLinkType.Adjustment)+" THEN adjustment.AdjAmt "
						+"WHEN "+POut.Int((int)PayPlanLinkType.Procedure)+" THEN procedurelog.ProcFee*GREATEST(1,procedurelog.BaseUnits+procedurelog.UnitQty)"
							+"+COALESCE(sumprocadj.SumProcAdj,0)-COALESCE(sumins.SumIns,0) "
						+"ELSE 0 END-COALESCE(sumsplitoutside.SumSplitOutside,0))-COALESCE(sumsplitonplan.SumSplitOnPlan,0)"
				+",2),0)) AS 'amtOverpaid',"
				+"pat.PatNum,payplan.PayPlanNum,provider.ProvNum,COALESCE(clinic.ClinicNum,0) AS 'ClinicNum',payplan.Guarantor,payplanlink.LinkType,payplanlink.FKey "
				+"FROM payplanlink "
				+"LEFT JOIN payplan ON payplan.PayPlanNum=payplanlink.PayPlanNum "
				+"LEFT JOIN patient pat ON pat.PatNum=payplan.PatNum "
				+"LEFT JOIN patient guar ON guar.PatNum=payplan.Guarantor "
				+"LEFT JOIN procedurelog ON procedurelog.ProcNum=payplanlink.FKey AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Procedure)+" "
				+"LEFT JOIN adjustment ON adjustment.AdjNum=payplanlink.FKey AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Adjustment)+" "
				+"LEFT JOIN provider ON provider.ProvNum=procedurelog.ProvNum OR provider.ProvNum=adjustment.ProvNum "
				+"LEFT JOIN clinic ON clinic.ClinicNum=procedurelog.ClinicNum OR clinic.ClinicNum=adjustment.ClinicNum "
				+"LEFT JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
				+"LEFT JOIN (SELECT adjustment.ProcNum,SUM(adjustment.AdjAmt) AS 'SumProcAdj' FROM adjustment GROUP BY adjustment.ProcNum) AS sumprocadj "
					+"ON sumprocadj.ProcNum=procedurelog.ProcNum "
				+"LEFT JOIN (SELECT claimproc.ProcNum,"
					+"SUM(CASE "
						+"WHEN claimproc.Status IN ("+string.Join(",",listClaimProcStatForInsPaid)+") THEN claimproc.InsPayAmt+claimproc.WriteOff "
						+"WHEN claimproc.Status IN ("+string.Join(",",listClaimProcStatForInsEst)+") THEN claimproc.InsPayEst+"
							+"CASE "
								+"WHEN claimproc.WriteOffEstOverride!=-1 THEN claimproc.WriteOffEstOverride "
								+"WHEN claimproc.WriteOffEst!=-1 THEN claimproc.WriteOffEst "
								+"ELSE 0 END "
						+"ELSE 0 END) AS 'SumIns' "
					+"FROM claimproc GROUP BY claimproc.ProcNum) AS sumins ON sumins.ProcNum=procedurelog.ProcNum "
				+"LEFT JOIN (SELECT paysplit.ProcNum,paysplit.AdjNum,SUM(paysplit.SplitAmt) AS 'SumSplitOutside' "
					+"FROM paysplit WHERE paysplit.PayPlanNum=0 AND paysplit.PayPlanChargeNum=0 GROUP BY paysplit.ProcNum,paysplit.AdjNum) AS sumsplitoutside "
					+"ON (payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Procedure)+" AND sumsplitoutside.ProcNum=payplanlink.FKey) "
					+"OR (payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Adjustment)+" AND sumsplitoutside.AdjNum=payplanlink.FKey) "
				+"LEFT JOIN (SELECT paysplit.ProcNum,paysplit.AdjNum,paysplit.PayPlanNum,SUM(paysplit.SplitAmt) AS 'SumSplitOnPlan' "
					+"FROM paysplit GROUP BY paysplit.ProcNum,paysplit.AdjNum,paysplit.PayPlanNum) AS sumsplitonplan "
					+"ON ((sumsplitonplan.ProcNum=payplanlink.FKey AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Procedure)+") "
					+"OR (sumsplitonplan.AdjNum=payplanlink.FKey AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Adjustment)+")) "
					+"AND sumsplitonplan.PayPlanNum=payplanlink.PayPlanNum "
				+"LEFT JOIN (SELECT payplancharge.FKey,payplancharge.LinkType,SUM(payplancharge.Principal) AS 'SumPayPlanCharge' "
					+"FROM payplancharge WHERE payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "
					+"GROUP BY payplancharge.FKey,payplancharge.LinkType) AS sumpayplancharge "
					+"ON sumpayplancharge.FKey=payplanlink.FKey AND sumpayplancharge.LinkType=payplanlink.LinkType "
				+"WHERE payplan.DatePayPlanStart BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd);
				if(patNum!=0) {//If no Patient is selected, show all patients
					query+=" AND payplan.PatNum="+POut.Long(patNum);
				}
			DataTable result=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(query));
			//Get all distinct PayPlanNums from resulting table.
			List<long> listPayPlanNumsInTable=result.AsEnumerable().Select(x => x.Field<long>("PayPlanNum")).Distinct().ToList();
			//Remove any PayPlanNums from list that are for plans that aren't overcharged.
			listPayPlanNumsInTable.RemoveAll(x =>
				result.AsEnumerable().Where(y => y.Field<long>("PayPlanNum").Equals(x)).Sum(y => y.Field<double>("planDebits")) <=
				result.AsEnumerable().Where(z => z.Field<long>("PayPlanNum").Equals(x)).Sum(z => z.Field<double>("patPortionOnPlan")));
			//Keep only the rows that are for overcharged pay plans and for production entries that pass the clinic and provider filters.
			List<DataRow> listRows=result.AsEnumerable().Where(x => ListTools.In(x.Field<long>("PayPlanNum"),listPayPlanNumsInTable)
				&& CompareDecimal.IsGreaterThanZero(x.Field<double>("amtOvercharged"))
				&& (listClinicNums.IsNullOrEmpty() || ListTools.In(x.Field<long>("ClinicNum"),listClinicNums)) 
				&& (listProvNums.IsNullOrEmpty() || ListTools.In(x.Field<long>("ProvNum"),listProvNums)))
				.OrderBy(x => x.Field<DateTime>("DatePayPlanStart")).ThenBy(x => x.Field<long>("PayPlanNum")).ToList();
			if(listRows.Any()) {//Need to make sure we have rows before copying to data table, otherwise error occurs.
				result=listRows.CopyToDataTable();
			}
			else {//Just copy structure of table if now rows are present after filtering.
				result=result.Clone();
			}
			return result;
		}
	}
}
