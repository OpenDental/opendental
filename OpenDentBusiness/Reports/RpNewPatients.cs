using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class RpNewPatients {

		public static DataTable GetNewPatients(DateTime dateFrom,DateTime dateTo,List<long> listProvNums,bool includeAddress,bool excludeNoProd,bool hasAllProvs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvNums,includeAddress,excludeNoProd,hasAllProvs);
			}
			string query=@"SET @pos=0;
				SELECT @pos:=@pos+1 patCount, result.* FROM (SELECT dateFirstProc,patient.LName,patient.FName,"
				+DbHelper.Concat("referral.LName","IF(referral.FName='','',', ')","referral.FName")+" refname,"
				+"SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)) ";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				query+="$HowMuch";
			}
			else { //Oracle needs quotes.
				query+="\"$HowMuch\"";
			}
			if(includeAddress) {
				query+=",patient.Preferred,patient.Address,patient.Address2,patient.City,patient.State,patient.Zip";
			}
			query+=" FROM "
				+"(SELECT PatNum, MIN(ProcDate) dateFirstProc FROM procedurelog "
				+"INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
				+"AND ProcCode NOT IN ('D9986','D9987') "/*Do not count missed or canceled appointments*/
				+"WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" GROUP BY PatNum "
				+"HAVING dateFirstProc BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+") table1 "
				+"INNER JOIN patient ON table1.PatNum=patient.PatNum "
				+"LEFT JOIN procedurelog ON patient.PatNum=procedurelog.PatNum AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+"LEFT JOIN refattach ON patient.PatNum=refattach.PatNum AND refattach.RefType="+POut.Int((int)ReferralType.RefFrom)+" "
				+"AND refattach.ItemOrder=("
					+"SELECT MIN(ra.ItemOrder) FROM refattach ra WHERE ra.PatNum=refattach.PatNum AND ra.RefType="+POut.Int((int)ReferralType.RefFrom)
				+") "
				+"LEFT JOIN referral ON referral.ReferralNum=refattach.ReferralNum ";
			if(!hasAllProvs) {
				query+="WHERE patient.PriProv IN ("+String.Join(",",listProvNums)+") ";
			}
			query+="GROUP BY patient.LName,patient.FName,patient.PatNum,"+DbHelper.Concat("referral.LName","IF(referral.FName='','',',')","referral.FName");
			if(includeAddress) {
				query+=",patient.Preferred,patient.Address,patient.Address2,patient.City,patient.State,patient.Zip";
			}
			if(excludeNoProd) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					query+=" HAVING $HowMuch > 0";
				}
				else {//Oracle needs quotes.
					query+=" HAVING \"$HowMuch\" > 0";
				}
			}
			query+=" ORDER BY dateFirstProc,patient.LName,patient.FName) result";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(query));
		}
	}
}
