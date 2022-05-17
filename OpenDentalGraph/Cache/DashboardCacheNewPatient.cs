using System;
using OpenDentBusiness;
using System.Data;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheNewPatient:DashboardCacheWithQuery<NewPatient> {
		protected override string GetCommand(DashboardFilter filter) {
			string where="ProcStatus="+POut.Int((int)ProcStat.C);
			if(filter.UseProvFilter) {
				where+=" AND ProvNum="+POut.Long(filter.ProvNum);
			}
			string cmd=
				"SELECT PatNum, MIN(ProcDate) FirstProc, ClinicNum, ProvNum "
				+"FROM procedurelog USE INDEX(indexPNPSCN) "
				+"INNER JOIN procedurecode ON procedurecode.CodeNum = procedurelog.CodeNum "
												+"AND procedurecode.ProcCode NOT IN ('D9986','D9987')"
				+"WHERE "+where+" "
				+"GROUP BY PatNum";
			return cmd;
		}

		protected override NewPatient GetInstanceFromDataRow(DataRow x) {
			return new NewPatient() {
				DateStamp=PIn.DateT(x["FirstProc"].ToString()),
				Count=1, //Each row counts as 1.
				Val=0, //there are no fees
				SeriesName="All", //Only 1 series.
				ProvNum=PIn.Long(x["ProvNum"].ToString()),
				ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
			};
		}

		protected override bool AllowQueryDateFilter() {
			return false;
		}
	}

	public class NewPatient:GraphQuantityOverTime.GraphDataPointClinic { }
}