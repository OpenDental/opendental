using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpPrescriptions {
		///<summary></summary>
		public static DataTable GetPrescriptionTable(bool isRadioPatient, string inputText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),isRadioPatient,inputText);
			}
			string query="SELECT CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),"+
				"' '),patient.MiddleI),rxpat.rxdate,"
				+"rxpat.drug,rxpat.sig,rxpat.disp,provider.abbr FROM patient,rxpat,provider "
				+"WHERE patient.patnum=rxpat.patnum AND provider.provnum=rxpat.provnum ";
			if(isRadioPatient){
				query+="AND patient.lname like '"+POut.String(inputText)+"%'"
	        +" ORDER BY patient.lname,patient.fname,rxpat.rxdate";		
			}
			else{
				query+="AND rxpat.drug like '"+POut.String(inputText)+"%'"
			    +" ORDER BY patient.lname,rxpat.drug,rxpat.rxdate";
			}
			return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
		}	
	}

}
