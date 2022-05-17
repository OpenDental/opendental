using System;
using OpenDentBusiness;
using System.Data;
using System.Collections.Generic;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheAdjustment:DashboardCacheWithQuery<Adjustment> {
		protected override string GetCommand(DashboardFilter filter) {
			string where="";
			List<string> listWhereClauses=new List<string>();
			if(filter.UseDateFilter) {
				listWhereClauses.Add("AdjDate BETWEEN "+POut.Date(filter.DateFrom)+" AND "+POut.Date(filter.DateTo)+" ");
			}
			if(filter.UseProvFilter) {
				listWhereClauses.Add("ProvNum="+POut.Long(filter.ProvNum)+" ");
			}
			if(listWhereClauses.Count>0) {
				where="WHERE "+string.Join("AND ",listWhereClauses);
			}
			return
				"SELECT AdjDate,ProvNum,SUM(AdjAmt) AdjTotal, ClinicNum "
				+"FROM adjustment "
				+where
				+"GROUP BY AdjDate,ProvNum,ClinicNum "
				+"HAVING AdjTotal<>0 "
				+"ORDER BY AdjDate,ProvNum ";
		}

		protected override Adjustment GetInstanceFromDataRow(DataRow x) {
			//long provNum=x.Field<long>("ProvNum");
			//string seriesName=DashboardCache.Providers.GetProvName(provNum);
			return new Adjustment() {
				ProvNum=PIn.Long(x["ProvNum"].ToString()),
				DateStamp=PIn.DateT(x["AdjDate"].ToString()),
				Val=PIn.Double(x["AdjTotal"].ToString()),
				Count=0, //count procedures, not adjustments.			
								 //SeriesName=seriesName,
				ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
			};
		}
	}

	public class Adjustment:GraphQuantityOverTime.GraphDataPointClinic {
	}
}