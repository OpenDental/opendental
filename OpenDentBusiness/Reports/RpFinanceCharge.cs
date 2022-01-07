using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpFinanceCharge{
		public static DataTable GetFinanceChargeTable(DateTime dateStart,DateTime dateEnd,long finChargeAdjType,List<long> listProvNums,List<long> listBillingDefNums) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,finChargeAdjType,listProvNums,listBillingDefNums);
			}
			string query="SELECT patient.PatNum,"+DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI")+" PatName,patient.Preferred"
				+",adjustment.AdjAmt "
				+"FROM patient "
				+"INNER JOIN adjustment ON patient.PatNum=adjustment.PatNum "
					+"AND adjustment.AdjDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
					+"AND adjustment.AdjType = "+POut.Long(finChargeAdjType)+" ";
				if(listProvNums.Count>0) {
					query+="AND patient.PriProv IN ("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+") ";
				}
				if(listBillingDefNums.Count>0) {
					query+="AND patient.BillingType IN ("+string.Join(",",listBillingDefNums.Select(x => POut.Long(x)))+") ";
				}
				query+="ORDER BY patient.LName,patient.FName,AdjAmt DESC";
			DataTable table=ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
			return table;
		}	
	}

}
