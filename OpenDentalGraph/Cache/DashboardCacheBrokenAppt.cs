using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace OpenDentalGraph.Cache {
	public class DashboardCacheBrokenAppt:DashboardCacheWithQuery<BrokenAppt> {
		protected override string GetCommand(DashboardFilter filter) {
			string where="WHERE AptStatus="+(int)ApptStatus.Broken+" ";
			if(filter.UseDateFilter) {
				where+="AND DATE(AptDateTime) BETWEEN "+POut.Date(filter.DateFrom)+" AND "+POut.Date(filter.DateTo)+" ";
			}
			if(filter.UseProvFilter) {
				where+="AND ProvNum="+POut.Long(filter.ProvNum)+" ";
			}
			return
				"SELECT DATE(AptDateTime) ApptDate,ProvNum,ClinicNum,COUNT(AptNum) ApptCount "
				+"FROM appointment "
				+where
				+"GROUP BY ApptDate,ProvNum,ClinicNum ";
		}

		protected override BrokenAppt GetInstanceFromDataRow(DataRow x) {
			return new BrokenAppt() {
				ProvNum=PIn.Long(x["ProvNum"].ToString()),
				ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
				DateStamp=PIn.DateT(x["ApptDate"].ToString()),
				Count=PIn.Long(x["ApptCount"].ToString()),
				Val=0, //appointments do not have their own value.
			};
		}
	}

	public class BrokenAppt:GraphQuantityOverTime.GraphDataPointClinic {
	}	
}
