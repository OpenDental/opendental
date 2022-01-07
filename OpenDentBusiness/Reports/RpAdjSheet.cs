using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpAdjSheet {
		///<summary>If not using clinics then supply an empty list of clinicNums.</summary>
		public static DataTable GetAdjTable(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listClinicNums,
			List<string> listAdjType,bool hasAllClinics,bool hasClinicsEnabled) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listClinicNums,listAdjType,hasAllClinics,hasClinicsEnabled);
			}
			string whereProv="";
			if(listProvNums.Count > 0) {
				whereProv+=" AND adjustment.ProvNum IN("+string.Join(",",listProvNums)+") ";
			}
			string whereClin="";
			if(hasClinicsEnabled && listClinicNums.Count > 0) {//Using clinics
				whereClin+=" AND adjustment.ClinicNum IN("+string.Join(",",listClinicNums)+") ";
			}
			string whereType="";
			if(listAdjType.Count > 0) {
				whereType=" AND adjustment.AdjType IN("+string.Join(",",listAdjType)+") ";
			}
			string query="SELECT adjustment.AdjDate,"
					+DbHelper.Concat("patient.LName","', '","patient.FName","', '","patient.MiddleI")+","
					+"provider.Abbr,";
			if(hasClinicsEnabled) {
				query+="clinic.Abbr,";
			}
			query+="definition.ItemName,adjustment.AdjNote,adjustment.AdjAmt "
				+"FROM adjustment "
				+"INNER JOIN definition ON definition.DefNum=adjustment.AdjType "
				+"INNER JOIN patient ON patient.PatNum=adjustment.PatNum "
			  +"LEFT JOIN provider ON provider.ProvNum=adjustment.ProvNum "
				+"LEFT JOIN clinic ON clinic.ClinicNum=adjustment.ClinicNum "
				+"WHERE adjustment.AdjDate >= "+POut.Date(dateStart)+" AND adjustment.AdjDate <= "+POut.Date(dateEnd)+" ";
			query+=whereProv;
			if(hasClinicsEnabled) {
				query+=whereClin;
			}
			query+=whereType;
			query+="ORDER BY adjustment.AdjDate";
			return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
		}	
	}

}
