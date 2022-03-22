using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class RpNewPatients {

		public static DataTable GetNewPatients(DateTime dateFrom,DateTime dateTo,List<long> listProvNums,bool includeAddress,bool excludeNoProd,bool hasAllProvs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvNums,includeAddress,excludeNoProd,hasAllProvs);
			}
			//used to limit procedurelog in query, getting codenums from the cache so we don't have to join the procedurecode table.
			string missApptProcs = String.Join(",",ProcedureCodes.GetWhere(x => new List<string>{ "D9986","D9987"}.Contains(x.ProcCode)).Select(x => x.CodeNum));
			string addressFields = ",patient.Preferred,patient.Address,patient.Address2,patient.City,patient.State,patient.Zip ";
			string provWhere = $" AND patient.PriProv IN({String.Join(",",listProvNums)}) ";
			string query = $@"SET @pos=0;
				SELECT
				    @pos:=@pos+1 patCount,
				    result.*
				FROM (
				    SELECT
				        dateFirstProc,
				        patient.LName,
				        patient.FName,
				        CONCAT(referral.LName,IF(referral.FName='','',', '),referral.FName) refname,
				        (SELECT
				            SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits))
				        FROM procedurelog
				        WHERE table1.PatNum=procedurelog.PatNum
				            AND procedurelog.ProcStatus={POut.Int((int)ProcStat.C)}
				        ) $HowMuch
								{(includeAddress? addressFields : "")}
				    FROM (
				        SELECT
				            PatNum,
				            (SELECT 
											MIN(pl2.ProcDate) 
										FROM procedurelog pl2 
										WHERE pl2.ProcStatus = {POut.Int((int)ProcStat.C)} 
										AND pl2.PatNum = procedurelog.PatNum 
										AND !FIND_IN_SET(pl2.CodeNum, '{missApptProcs}')
										) dateFirstProc
				        FROM procedurelog
				        WHERE ProcStatus=2
								AND procedurelog.ProcDate BETWEEN DATE({POut.Date(dateFrom)}) AND DATE({POut.Date(dateTo)})
								GROUP BY PatNum
				    ) table1
				    INNER JOIN patient
				        ON table1.PatNum=patient.PatNum
				    LEFT JOIN refattach
				        ON patient.PatNum=refattach.PatNum
				        AND refattach.RefType={POut.Int((int)ReferralType.RefFrom)}
				        AND refattach.ItemOrder=(SELECT MIN(ra.ItemOrder) FROM refattach ra WHERE ra.PatNum=refattach.PatNum AND ra.RefType={POut.Int((int)ReferralType.RefFrom)})
				    LEFT JOIN referral
				        ON referral.ReferralNum=refattach.ReferralNum
				    WHERE DATE(table1.dateFirstProc) BETWEEN DATE({POut.Date(dateFrom)}) AND DATE({POut.Date(dateTo)})
						{(!hasAllProvs? provWhere : "")}
				    GROUP BY patient.LName,patient.FName,patient.PatNum,CONCAT(referral.LName,IF(referral.FName='','',','),referral.FName){(includeAddress? addressFields : "")}
				    ORDER BY dateFirstProc,patient.LName,patient.FName
				) result
				{(excludeNoProd? "HAVING $HowMuch > 0" : "")}";

			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(query));
		}
	}
}
