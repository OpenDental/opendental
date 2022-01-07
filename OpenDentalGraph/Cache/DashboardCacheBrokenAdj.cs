using System;
using OpenDentBusiness;
using System.Data;
using System.Collections.Generic;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheBrokenAdj:DashboardCacheWithQuery<BrokenAdj> {
		protected override string GetCommand(DashboardFilter filter) {
			string where="";
			List<string> listWhereClauses=new List<string>();
			if(filter.UseDateFilter) {
				listWhereClauses.Add("DATE(AdjDate) BETWEEN "+POut.Date(filter.DateFrom)+" AND "+POut.Date(filter.DateTo)+" ");
			}
			if(filter.UseProvFilter) {
				listWhereClauses.Add("ProvNum="+POut.Long(filter.ProvNum)+" ");
			}
			if(listWhereClauses.Count>0) {
				where="WHERE "+string.Join("AND ",listWhereClauses);
			}
			return
				"SELECT AdjDate,ProvNum,COUNT(AdjNum) AdjCount,ClinicNum,AdjType, SUM(AdjAmt) AdjAmt "
				+"FROM adjustment "
				+"INNER JOIN definition ON definition.DefNum=adjustment.AdjType "
				+"AND definition.ItemValue = '+' "
				+where
				+"GROUP BY AdjDate,ProvNum,ClinicNum,AdjType "
				+"ORDER BY AdjDate,ProvNum,ClinicNum ";
		}

		protected override BrokenAdj GetInstanceFromDataRow(DataRow x) {
			return new BrokenAdj() {
				ProvNum=PIn.Long(x["ProvNum"].ToString()),
				ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
				DateStamp=PIn.Date(x["AdjDate"].ToString()),
				Val=PIn.Double(x["AdjAmt"].ToString()),
				AdjType=PIn.Long(x["AdjType"].ToString()),
				Count=PIn.Long(x["AdjCount"].ToString()),
			};
		}
	}

	public class BrokenAdj:GraphQuantityOverTime.GraphDataPointClinic {
		public long AdjType;

	}
}