using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpInsOverpaid {
		///<summary>If not using clinics then supply an empty list of clinicNums.  listClinicNums must have at least one item if using clinics.</summary>
		public static DataTable GetInsuranceOverpaid(DateTime dateStart,DateTime dateEnd,List<long> listClinicNums,bool groupByProc) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listClinicNums,groupByProc);
			}
			string query=$@"
					SELECT 
						CONCAT(patient.LName,', ',patient.FName) patname
						,procedurelog.ProcDate
						,SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)) $sumfee
						,cp.PayAmt $PaidAndWriteoff
					FROM procedurelog
					INNER JOIN (
						SELECT 
							claimproc.ProcNum
							,claimproc.PatNum
							,claimproc.ProcDate
							,SUM(claimproc.InsPayAmt+claimproc.Writeoff) PayAmt
						FROM claimproc
						WHERE claimproc.Status IN (1,4,5,7)
						AND claimproc.ProcDate BETWEEN DATE({POut.Date(dateStart)}) AND DATE({POut.Date(dateEnd)}) ";
			if(groupByProc) {
				query+="GROUP BY claimproc.ProcNum ";
            }
            else {
				query+="GROUP BY claimproc.PatNum, claimproc.ProcDate ";
			}
			query+=@"HAVING SUM(claimproc.InsPayAmt+claimproc.Writeoff)>0/*ProcFee must be >0 and PayAmt must be >ProcFee, ergo PayAmt must be >0*/
								ORDER BY NULL
					) cp ";
            if(groupByProc) {
				query+="ON cp.ProcNum=procedurelog.ProcNum ";
            }
            else {
				query+=@"ON cp.PatNum = procedurelog.PatNum
						AND cp.ProcDate = procedurelog.ProcDate ";
            }
			query+=$@"
					INNER JOIN patient ON patient.PatNum=procedurelog.PatNum
					WHERE procedurelog.ProcDate BETWEEN DATE({POut.Date(dateStart)}) AND DATE({POut.Date(dateEnd)})
						AND procedurelog.ProcStatus=2
						AND procedurelog.ProcFee>0 ";
			if(listClinicNums.Count>0) {
				query+=$"AND procedurelog.ClinicNum IN({string.Join(",",listClinicNums)}) ";
			}
            if(groupByProc) {
				query+="GROUP BY procedurelog.ProcNum ";
			}
            else {
                query+="GROUP BY procedurelog.PatNum, procedurelog.ProcDate ";
            }
            query+=@"HAVING ROUND($sumfee,3) < ROUND($PaidAndWriteoff,3)
					 ORDER BY patient.LName,patient.FName,procedurelog.ProcDate";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(query));
		}
	}
}