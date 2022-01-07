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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
					query+="clinic.Abbr Clinic,";
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
					query+="clinic.Abbr Clinic,";
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
				query+="SELECT "+DbHelper.DtimeToDate("claimsnapshot.DateTEntry")+" date, "
					+DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI")+", "
					+"carrier.CarrierName, "
					+"provider.Abbr,";
				if(hasClinicsEnabled) {
					query+="clinic.Abbr Clinic,";
				}
				query+="SUM("+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+"), "
					+"-SUM(IF(claimproc.Status="+(int)ClaimProcStatus.NotReceived+",0,"+DbHelper.IfNull("NULLIF(claimsnapshot.WriteOff, -1)","0",false)+"-claimproc.WriteOff)), "
					+"SUM(claimproc.WriteOff) $writeoff "
					+"FROM claimproc "
					+"LEFT JOIN insplan ON claimproc.PlanNum = insplan.PlanNum "
					+"LEFT JOIN patient ON claimproc.PatNum = patient.PatNum "
					+"LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum "
					+"LEFT JOIN provider ON provider.ProvNum = claimproc.ProvNum "
					+"LEFT JOIN clinic ON clinic.ClinicNum=claimproc.ClinicNum "
					+"INNER JOIN claimsnapshot on claimsnapshot.ClaimProcNum=claimproc.ClaimProcNum "	//use claimsnapshot DateTEntry instead of claimproc
					+"WHERE claimproc.Status IN ("+(int)ClaimProcStatus.Received+","+(int)ClaimProcStatus.Supplemental+","+(int)ClaimProcStatus.NotReceived+") "
					+whereProv
					+whereClin
					+"AND "+DbHelper.DtimeToDate("claimsnapshot.DateTEntry")+" >= @FromDate "
					+"AND "+DbHelper.DtimeToDate("claimsnapshot.DateTEntry")+" <= @ToDate "
					+"AND claimsnapshot.WriteOff > 0 "
					+"GROUP BY claimproc.ProvNum,claimsnapshot.DateTEntry,claimproc.ClinicNum,claimproc.PatNum "
					+"ORDER BY claimsnapshot.DateTEntry,claimproc.PatNum";
			}
			return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
		}	
	}

}
