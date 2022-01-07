using System;
using System.Collections.Generic;
using OpenDentBusiness;
using System.Data;
using System.Linq;
using System.Text;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheBrokenProcedure:DashboardCacheWithQuery<BrokenProc> {
		protected override string GetCommand(DashboardFilter filter) {
			string where="WHERE ProcStatus="+(int)ProcStat.C+" ";
			if(filter.UseDateFilter) {
				where="AND DATE(ProcDate) BETWEEN "+POut.Date(filter.DateFrom)+" AND "+POut.Date(filter.DateTo)+" ";
			}
			if(filter.UseProvFilter) {
				where+="AND ProvNum="+POut.Long(filter.ProvNum)+" ";
			}
			return
				"SELECT ProcDate,ProvNum,ClinicNum,COUNT(ProcNum) ProcCount, SUM(ProcFee) ProcFee,ProcCode "
				+"FROM procedurelog "
				+"INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
				+"AND procedurecode.ProcCode IN('D9986','D9987') "
				+where
				+"GROUP BY ProcDate,ProvNum,ClinicNum,ProcCode ";
		}

		protected override BrokenProc GetInstanceFromDataRow(DataRow x) {
			return new BrokenProc() {
				ProvNum=PIn.Long(x["ProvNum"].ToString()),
				ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
				DateStamp=PIn.DateT(x["ProcDate"].ToString()),
				Count=PIn.Long(x["ProcCount"].ToString()),
				Val=PIn.Double(x["ProcFee"].ToString()),
				ProcCode=PIn.String(x["ProcCode"].ToString())
			};
		}
	}

	public class BrokenProc:GraphQuantityOverTime.GraphDataPointClinic {
		public string ProcCode;
	}	
}