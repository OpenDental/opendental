using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class RpClaimNotSent {

		public static DataTable GetClaimsNotSent(DateTime fromDate,DateTime toDate,List<long> listClinicNums
			,bool hasClaimTypeExpanded,ClaimNotSentStatuses claimStatusFilter) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),fromDate,toDate,listClinicNums
					,hasClaimTypeExpanded,claimStatusFilter);
			}
			bool hasClinicsEnabled=ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache);
			string command="";
			string whereClin="";
			string claimFilter="";
			if(hasClinicsEnabled && listClinicNums.Count>0) {//construct the IN statement for all of the selected clinics
				whereClin+=" AND claim.ClinicNum IN(" + string.Join(",",listClinicNums)+")";
			}
			if(hasClinicsEnabled) {
				command="SELECT clinic.Abbr AS 'Clinic',";
			}
			else {
				command="SELECT ";
			}
			if(hasClaimTypeExpanded) {
				command+="claim.DateService,(CASE WHEN claim.ClaimType='P' THEN 'Primary' WHEN claim.ClaimType='S' THEN 'Secondary' "
				+"WHEN claim.ClaimType='Cap' THEN 'Capitation' ELSE claim.ClaimType END) AS ClaimType,";
			}
			else {
				command+="claim.DateService,claim.ClaimType,";
			}
			//Claim statuses of Unsent, Hold until pri, and Waiting are considered for "All" in this report.
			string claimStatusAll="AND claim.ClaimStatus IN ('U','H','W')";
			switch(claimStatusFilter) {
				case ClaimNotSentStatuses.Primary:
					claimFilter="AND claim.ClaimType='P' "+claimStatusAll;
					break;
				case ClaimNotSentStatuses.Secondary:
					claimFilter="AND claim.ClaimType='S' "+claimStatusAll;
					break;
				case ClaimNotSentStatuses.Holding:
					claimFilter="AND claim.ClaimStatus='H'";
					break;
				default://All
					claimFilter+=claimStatusAll;
					break;
			}
			string clinJoin="";
			if(hasClinicsEnabled) {
				clinJoin=" LEFT JOIN clinic ON clinic.ClinicNum=claim.ClinicNum";
			}
			command+="(CASE WHEN claim.ClaimStatus='U' THEN 'Unsent' WHEN "
				+"claim.ClaimStatus='H' THEN 'Hold' WHEN claim.ClaimStatus='W' THEN 'WaitQ' ELSE claim.ClaimStatus END) AS ClaimStatus, "
				+"CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) as 'Patient Name',carrier.CarrierName"
				+",claim.ClaimFee,GROUP_CONCAT(procedurecode.ProcCode SEPARATOR ', ') ProcCodes,claim.ClaimNum,claim.PatNum "
				+"FROM patient"
				+" INNER JOIN claim ON claim.PatNum=patient.PatNum"
				+" INNER JOIN claimproc ON claimproc.ClaimNum=claim.ClaimNum"
				+" INNER JOIN procedurelog ON procedurelog.ProcNum=claimproc.ProcNum"
				+" INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum"
				+" INNER JOIN insplan ON insplan.PlanNum=claim.PlanNum"
				+" INNER JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum"
				+clinJoin
				+" WHERE claim.DateService >= "+POut.Date(fromDate)
				+" AND claim.DateService <= "+POut.Date(toDate)
				+whereClin
				+claimFilter
				+" GROUP BY claim.ClaimNum";
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				command+=",claim.Status,patient.LName,patient.FName,patient.MiddleI,carrier.CarrierName,claim.ClaimFee,claim.ClaimNum,claim.PatNum";
			}
			command+=" ORDER BY claim.DateService";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
		}
	}
}
