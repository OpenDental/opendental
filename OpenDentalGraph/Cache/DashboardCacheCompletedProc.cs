using System;
using OpenDentBusiness;
using System.Data;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheCompletedProc:DashboardCacheWithQuery<CompletedProc> {
		protected override string GetCommand(DashboardFilter filter) {
			string where="WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" ";
			if(filter.UseDateFilter) {
				where+="AND procedurelog.ProcDate BETWEEN "+POut.Date(filter.DateFrom)+" AND "+POut.Date(filter.DateTo)+" ";
			}
			if(filter.UseProvFilter) {
				where+="AND ProvNum="+POut.Long(filter.ProvNum)+" ";
			}
			return
				"SELECT procedurelog.ProcDate,procedurelog.ProvNum,procedurelog.ClinicNum, "
				+"SUM(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)) AS GrossProd, "
				+"COUNT(procedurelog.ProcNum) AS ProcCount  "
				+"FROM procedurelog "
				+where
				+"GROUP BY procedurelog.ProcDate,procedurelog.ProvNum,procedurelog.ClinicNum ";
		}

		protected override CompletedProc GetInstanceFromDataRow(DataRow x) {
			//long provNum=x.Field<long>("ProvNum");
			//string seriesName=DashboardCache.Providers.GetProvName(provNum);
			return new CompletedProc() {
				ProvNum=PIn.Long(x["ProvNum"].ToString()),
				DateStamp=PIn.DateT(x["ProcDate"].ToString()),
				Val=PIn.Double(x["GrossProd"].ToString()),
				Count=PIn.Long(x["ProcCount"].ToString()),
				//SeriesName=seriesName,
				ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
			};
		}
	}

	public class CompletedProc:GraphQuantityOverTime.GraphDataPointClinic { }
}