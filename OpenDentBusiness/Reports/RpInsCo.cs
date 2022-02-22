using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpInsCo {
		///<summary></summary>
		public static DataTable GetInsCoTable(string carrier) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),carrier);
			}
			string query= "SELECT carrier.CarrierName"
				+",CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) AS SubscriberName,carrier.Phone,"
				+"insplan.Groupname "
				+"FROM insplan,inssub,patient,carrier "//,patplan "//we only include patplan to make sure insurance is active for a patient.  We don't want any info from patplan.
				+"WHERE inssub.Subscriber=patient.PatNum "
				+"AND inssub.PlanNum=insplan.PlanNum "
				+"AND EXISTS (SELECT * FROM patplan WHERE patplan.InsSubNum=inssub.InsSubNum) "
				//+"AND insplan.PlanNum=patplan.PlanNum "
				//+"AND patplan.PatNum=patient.PatNum "
				//+"AND patplan.Ordinal=1 "
				+"AND carrier.CarrierNum=insplan.CarrierNum "
				+"AND carrier.CarrierName LIKE '"+POut.String(carrier)+"%' "
				+"ORDER BY carrier.CarrierName,patient.LName";
			return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
		}	
	}

}
