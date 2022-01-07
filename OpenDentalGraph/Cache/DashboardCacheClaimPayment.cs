using System;
using OpenDentBusiness;
using System.Data;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheClaimPayment:DashboardCacheWithQuery<ClaimPayment> {
		protected override string GetCommand(DashboardFilter filter) {
			string where="";
			if(filter.UseDateFilter) {
				where="DateCP BETWEEN "+POut.Date(filter.DateFrom)+" AND "+POut.Date(filter.DateTo)+" AND ";
			}
			if(filter.UseProvFilter) {
				where+="ProvNum="+POut.Long(filter.ProvNum)+" AND ";
			}
			return
				"SELECT ProvNum,DateCP,SUM(InsPayAmt) AS GrossIncome,ClinicNum "
				+"FROM claimproc "
				+"WHERE "+where+"ClaimPaymentNum<>0 AND InsPayAmt<>0 "
				+"GROUP BY ProvNum,DateCP,ClinicNum ";
		}

		protected override ClaimPayment GetInstanceFromDataRow(DataRow x) {
			//long provNum=x.Field<long>("ProvNum");
			//string provName=DashboardCache.Providers.GetProvName(provNum);
			return new ClaimPayment() {
				ProvNum=PIn.Long(x["ProvNum"].ToString()),
				DateStamp=PIn.DateT(x["DateCP"].ToString()),
				Val=PIn.Double(x["GrossIncome"].ToString()),
				Count=0, //nothing to count for income.
								 //SeriesName=provName,
				ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
			};
		}
	}

	public class ClaimPayment:GraphQuantityOverTime.GraphDataPointClinic { }
}