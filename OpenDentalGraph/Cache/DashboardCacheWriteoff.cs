using System;
using OpenDentBusiness;
using System.Data;

namespace OpenDentalGraph.Cache {
	///<summary>We use the same cache to track regular writeoffs and capitation writeoffs for efficiency.</summary>
	public class DashboardCacheWriteoff:DashboardCacheWithQuery<Writeoff> {
		protected override string GetCommand(DashboardFilter filter) {
			string where="WHERE TRUE ";
			if(filter.UseDateFilter) {
				where+="AND ProcDate BETWEEN "+POut.Date(filter.DateFrom)+" AND "+POut.Date(filter.DateTo)+" ";
			}
			if(filter.UseProvFilter) {
				where+="AND ProvNum="+POut.Long(filter.ProvNum)+" ";
			}
			return
				"SELECT ProcDate,ProvNum,SUM(WriteOff) AS WriteOffs, IF(claimproc.Status="+(int)ClaimProcStatus.CapComplete+",'1','0') AS IsCap, ClinicNum "
				+"FROM claimproc "
				+where
				+"AND claimproc.Status IN ("
				+POut.Int((int)ClaimProcStatus.Received)+","
				+POut.Int((int)ClaimProcStatus.Supplemental)+","
				+POut.Int((int)ClaimProcStatus.NotReceived)+","
				+POut.Int((int)ClaimProcStatus.CapComplete)+") "
				+"GROUP BY ProcDate,ProvNum,(claimproc.Status="+(int)ClaimProcStatus.CapComplete+"),ClinicNum "
				+"HAVING WriteOffs<>0 ";
		}

		protected override Writeoff GetInstanceFromDataRow(DataRow x) {
			//long provNum=x.Field<long>("ProvNum");
			//string seriesName=DashboardCache.Providers.GetProvName(provNum);
			return new Writeoff() {
				ProvNum=PIn.Long(x["ProvNum"].ToString()),
				DateStamp=PIn.DateT(x["ProcDate"].ToString()),
				Val=-PIn.Double(x["WriteOffs"].ToString()),
				Count=0, //count procedures, not writeoffs.
								 //SeriesName=seriesName,
				ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
				IsCap=PIn.Bool(x.Field<string>("IsCap")),

			};
		}
	}

	public class Writeoff:GraphQuantityOverTime.GraphDataPointClinic {
		public bool IsCap;
	}
}