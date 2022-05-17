using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class RpWriteoffSheet {
		///<summary>If not using clinics then supply an empty list of clinicNums.</summary>
		public static DataTable GetWriteoffTable(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listClinicNums
			,bool hasAllClinics,bool hasClinicsEnabled,PPOWriteoffDateCalc writeoffPayType) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listClinicNums,hasAllClinics
					,hasClinicsEnabled,writeoffPayType);
			}
			string whereProv="";
			if(listProvNums.Count > 0) {
				whereProv+=" AND claimproc.ProvNum IN("+string.Join(",",listProvNums)+") ";
			}
			string whereClin="";
			if(hasClinicsEnabled && listClinicNums.Count > 0) {//Using clinics
				whereClin+=" AND claimproc.ClinicNum IN("+string.Join(",",listClinicNums)+") ";
			}
			string query="SET @FromDate="+POut.Date(dateStart)+", @ToDate="+POut.Date(dateEnd)+";";
			if(writeoffPayType==PPOWriteoffDateCalc.InsPayDate) {
				query+="SELECT "+DbHelper.DtimeToDate("claimproc.DateCP")+" date,"
					+DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI")+","
					+"carrier.CarrierName,"
					+"provider.Abbr,";
				if(hasClinicsEnabled) {
					query+="IF(clinic.IsHidden,CONCAT(clinic.Abbr,'("+POut.String(Lans.g("FormRpWriteoffSheet","hidden"))+")'),clinic.Abbr) Clinic,";
				}
				query+="SUM(claimproc.WriteOff) $writeoff, "
					+"claimproc.ClaimNum "
					+"FROM claimproc "//,insplan,patient,carrier,provider "
					+"LEFT JOIN insplan ON claimproc.PlanNum = insplan.PlanNum "
					+"LEFT JOIN patient ON claimproc.PatNum = patient.PatNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"LEFT JOIN provider ON provider.ProvNum = claimproc.ProvNum "
					+"LEFT JOIN clinic ON clinic.ClinicNum=claimproc.ClinicNum "
					+"WHERE claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+") "
					+whereProv
					+whereClin
					+"AND "+DbHelper.DtimeToDate("claimproc.DateCP")+" >= @FromDate "
					+"AND "+DbHelper.DtimeToDate("claimproc.DateCP")+" <= @ToDate "
					+"AND (claimproc.WriteOff > .0001 OR claimproc.WriteOff < -.0001) "
					+"GROUP BY claimproc.ProvNum,claimproc.DateCP,claimproc.ClinicNum,claimproc.PatNum "
					+"ORDER BY claimproc.DateCP,claimproc.PatNum";
			}
			else if(writeoffPayType==PPOWriteoffDateCalc.ProcDate) {	//Means PPOWiteoffDateCalc==ProcDate, so we use the procedure date.
				query+="SELECT "+DbHelper.DtimeToDate("claimproc.ProcDate")+" date, "
					+DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI")+", "
					+"carrier.CarrierName, "
					+"provider.Abbr,";
				if(hasClinicsEnabled) {
					query+="IF(clinic.IsHidden,CONCAT(clinic.Abbr,'("+POut.String(Lans.g("FormRpWriteoffSheet","hidden"))+")'),clinic.Abbr) Clinic,";
				}
				query+="SUM(claimproc.WriteOff) $writeoff "
					+"FROM claimproc "
					+"LEFT JOIN insplan ON claimproc.PlanNum = insplan.PlanNum "
					+"LEFT JOIN patient ON claimproc.PatNum = patient.PatNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"LEFT JOIN provider ON provider.ProvNum = claimproc.ProvNum "
					+"LEFT JOIN clinic ON clinic.ClinicNum=claimproc.ClinicNum "
					+"WHERE claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "
					+whereProv
					+whereClin
					+"AND "+DbHelper.DtimeToDate("claimproc.ProcDate")+" >= @FromDate "
					+"AND "+DbHelper.DtimeToDate("claimproc.ProcDate")+" <= @ToDate "
					+"AND (claimproc.WriteOff > .0001 OR claimproc.WriteOff < -.0001) "
					+"GROUP BY claimproc.ProvNum,claimproc.ProcDate,claimproc.ClinicNum,claimproc.PatNum "
					+"ORDER BY claimproc.ProcDate,claimproc.PatNum";
			}
			else {//writeoffPayType==PPOWriteoffDateCalc.ClaimPayDate
				query+=$@"SELECT
									writeoff.date,
									writeoff.patient,
									writeoff.CarrierName,
									writeoff.Abbr,
									{(hasClinicsEnabled? "writeoff.Clinic,":"" )}
									writeoff.Estimate,
									writeoff.Adjustment,
									writeoff.Estimate + writeoff.Adjustment $writeoff
									FROM(
										SELECT -- estimates
											DATE(claimsnapshot.DateTEntry) 'date',
											CONCAT(patient.LName,', ',patient.FName,' ',patient.MiddleI) patient,
											carrier.CarrierName,
											provider.Abbr,
											IF(clinic.IsHidden,CONCAT(clinic.Abbr,'({POut.String(Lans.g("FormRpWriteoffSheet","hidden"))})'),clinic.Abbr) Clinic,		
											SUM(COALESCE(NULLIF(claimsnapshot.WriteOff,-1),0)) 'Estimate',
											0 'Adjustment',
											claimproc.ProvNum,
											claimproc.ClinicNum,
											claimproc.PatNum
										FROM claimproc
										INNER JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum
										LEFT JOIN insplan ON claimproc.PlanNum = insplan.PlanNum
										LEFT JOIN patient ON claimproc.PatNum = patient.PatNum
										LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum
										LEFT JOIN provider ON provider.ProvNum = claimproc.ProvNum
										LEFT JOIN clinic ON clinic.ClinicNum=claimproc.ClinicNum
										WHERE claimproc.Status IN({(int)ClaimProcStatus.Received},{(int)ClaimProcStatus.Supplemental},{(int)ClaimProcStatus.NotReceived}) -- not recieved, receieved, supplemental
										{whereProv}
										{whereClin}
										AND claimsnapshot.DateTEntry BETWEEN @FromDate AND @ToDate + INTERVAL 1 DAY
										AND claimsnapshot.WriteOff > 0
										GROUP BY claimproc.ProvNum,claimsnapshot.DateTEntry,claimproc.ClinicNum,claimproc.PatNum
									
										UNION ALL
									
										SELECT -- adjustments
											claimproc.DateCP as 'date',
											CONCAT(patient.LName,', ',patient.FName,' ',patient.MiddleI) patient,
											carrier.CarrierName,
											provider.Abbr,
											IF(clinic.IsHidden,CONCAT(clinic.Abbr,'({POut.String(Lans.g("FormRpWriteoffSheet","hidden"))})'),clinic.Abbr) Clinic,
											0 'Estimate',
											SUM(COALESCE(NULLIF(claimproc.WriteOff,-1),0)-COALESCE(NULLIF(claimsnapshot.WriteOff,-1),0)) 'Adjustment',
											claimproc.ProvNum,
											claimproc.ClinicNum,
											claimproc.PatNum
										FROM claimproc
										LEFT JOIN claimsnapshot ON claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum
										LEFT JOIN insplan ON claimproc.PlanNum = insplan.PlanNum
										LEFT JOIN patient ON claimproc.PatNum = patient.PatNum
										LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum
										LEFT JOIN provider ON provider.ProvNum = claimproc.ProvNum
										LEFT JOIN clinic ON clinic.ClinicNum=claimproc.ClinicNum
										WHERE claimproc.Status IN({(int)ClaimProcStatus.Received},{(int)ClaimProcStatus.Supplemental}) -- received, supplemental
										{whereProv}
										{whereClin}
										AND claimproc.DateCP BETWEEN @FromDate AND @ToDate
										AND ABS(COALESCE(NULLIF(claimproc.WriteOff,-1),0)-COALESCE(NULLIF(claimsnapshot.WriteOff,-1),0)) > 0.005
										GROUP BY claimproc.ProvNum,claimproc.ProcDate,claimproc.ClinicNum,claimproc.PatNum
										ORDER BY date,PatNum
									) writeoff";
			}
			return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
		}	
	}

}
