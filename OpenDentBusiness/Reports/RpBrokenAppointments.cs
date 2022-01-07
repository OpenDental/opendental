using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpBrokenAppointments {
		///<summary>If not using clinics then supply an empty list of clinicNums.</summary>
		public static DataTable GetBrokenApptTable(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listClinicNums,
			List<long> listAdj,BrokenApptProcedure brokenApptOption,bool hasAllClinics,bool isByProc,bool isByAptStatus,bool isByAdj,bool hasClinicsEnabled) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listClinicNums,listAdj,brokenApptOption,hasAllClinics,isByProc
					,isByAptStatus,isByAdj,hasClinicsEnabled);
			}
			string whereProv="";
			if(listProvNums.Count > 0) {
				if(isByProc) {//Report looking at ADA procedure code D9986
					whereProv=" AND procedurelog.ProvNum IN ("+string.Join(",",listProvNums)+") ";
				}
				else if(isByAdj) {//Report looking at broken appointment adjustments
					whereProv=" AND adjustment.ProvNum IN ("+string.Join(",",listProvNums)+") ";
				}
				else {//Report looking at appointments with a status of 'Broken'
					whereProv=" AND (appointment.ProvNum IN ("+string.Join(",",listProvNums)+") "
						+"OR appointment.ProvHyg IN ("+string.Join(",",listProvNums)+")) ";
				}
			}
			string whereClin="";
			if(hasClinicsEnabled && listClinicNums.Count > 0 && !hasAllClinics) {
				if(isByProc) {//Report looking at ADA procedure code D9986
					whereClin+=" AND procedurelog.ClinicNum IN(";
				}
				else if(isByAdj) {//Report looking at broken appointment adjustments
					whereClin+=" AND adjustment.ClinicNum IN(";
				}
				else {//Report looking at appointments with a status of 'Broken'
					whereClin+=" AND appointment.ClinicNum IN(";
				}
				whereClin+=string.Join(",",listClinicNums)+") ";
			}
			string queryBrokenApts="";
			if(isByProc) {
				queryBrokenApts=ByProceduresQuery(hasClinicsEnabled,dateStart,dateEnd,whereProv,whereClin,brokenApptOption);
			}
			if(isByAdj) {
				queryBrokenApts=ByAdjustmentsQuery(hasClinicsEnabled,dateStart,dateEnd,whereProv,whereClin,listAdj);
			}
			if(isByAptStatus) {
				queryBrokenApts=ByApptStatusQuery(hasClinicsEnabled,dateStart,dateEnd,whereProv,whereClin);
			}
			return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(queryBrokenApts));
		}

		private static string ByProceduresQuery(bool hasClinicsEnabled,DateTime dateStart,DateTime dateEnd,string whereProv,string whereClin,BrokenApptProcedure brokenApptOption) {
			string queryBrokenApts="SELECT procedurelog.ProcDate ProcDate,provider.Abbr Provider,";
				if(brokenApptOption==BrokenApptProcedure.Both) {//Show code when running for both.
					queryBrokenApts+="procedurecode.ProcCode,";
				}
				queryBrokenApts+=DbHelper.Concat("patient.LName","', '","patient.FName")+" Patient, "
				+"procedurelog.ProcFee ProcFee ";
				if(hasClinicsEnabled) {
					queryBrokenApts+=",COALESCE(clinic.Description,'"+POut.String(Lans.g("FormRpBrokenAppointments","Unassigned"))+"') ClinicDesc ";
				}
				queryBrokenApts+=
					"FROM procedurelog ";
				switch(brokenApptOption) {
					case BrokenApptProcedure.None://Just in case.
					case BrokenApptProcedure.Missed:
						queryBrokenApts+="INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum AND procedurecode.ProcCode='D9986' ";
					break;
					case BrokenApptProcedure.Cancelled:
						queryBrokenApts+="INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum AND procedurecode.ProcCode='D9987' ";
					break;
					case BrokenApptProcedure.Both:
						queryBrokenApts+="INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum AND procedurecode.ProcCode IN ('D9986','D9987') ";
					break;
				}
					queryBrokenApts+="INNER JOIN patient ON patient.PatNum=procedurelog.PatNum "
					+"INNER JOIN provider ON provider.ProvNum=procedurelog.ProvNum "
					+whereProv;
				if(hasClinicsEnabled) {
					queryBrokenApts+="LEFT JOIN clinic ON clinic.ClinicNum=procedurelog.ClinicNum ";
				}
				queryBrokenApts+="WHERE procedurelog.ProcDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
					+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" ";
				if(hasClinicsEnabled) {
					queryBrokenApts+=whereClin+" "
						+"ORDER BY clinic.Description,procedurelog.ProcDate,patient.LName,patient.FName";
				}
				else {
					queryBrokenApts+="ORDER BY procedurelog.ProcDate,patient.LName,patient.FName";
				}
			return queryBrokenApts;
		}

		private static string ByAdjustmentsQuery(bool hasClinicsEnabled,DateTime dateStart,DateTime dateEnd,string whereProv,string whereClin,List<long> listAdj) {
			string queryBrokenApts="SELECT adjustment.AdjDate AdjDate,provider.Abbr Provider,"+DbHelper.Concat("patient.LName","', '","patient.FName")+" Patient,"
					+"adjustment.AdjAmt AdjAmt,adjustment.AdjNote AdjNote ";
				if(hasClinicsEnabled) {
					queryBrokenApts+=",COALESCE(clinic.Description,'"+POut.String(Lans.g("FormRpBrokenAppointments","Unassigned"))+"') ClinicDesc ";
				}
				queryBrokenApts+=
					"FROM adjustment "
					+"INNER JOIN patient ON patient.PatNum=adjustment.PatNum "
					+"INNER JOIN provider ON provider.ProvNum=adjustment.ProvNum "
					+whereProv;
				if(hasClinicsEnabled) {
					queryBrokenApts+="LEFT JOIN clinic ON clinic.ClinicNum=adjustment.ClinicNum ";
				}
				queryBrokenApts+="WHERE adjustment.AdjDate BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" ";
				if(listAdj.Count > 0) {
					queryBrokenApts+="AND adjustment.AdjType IN("+string.Join(",",listAdj)+") ";
				}
				if(hasClinicsEnabled) {
					queryBrokenApts+=whereClin+" "
						+"ORDER BY clinic.Description,adjustment.AdjDate,patient.LName,patient.FName";
				}
				else {
					queryBrokenApts+="ORDER BY adjustment.AdjDate,patient.LName,patient.FName";
				}
			return queryBrokenApts;
		}

		private static string ByApptStatusQuery(bool hasClinicsEnabled,DateTime dateStart,DateTime dateEnd,string whereProv,string whereClin) {
			string queryBrokenApts="SELECT "+DbHelper.DateTFormatColumn("appointment.AptDateTime","%m/%d/%Y %H:%i:%s")+" AptDateTime, "
					+""+DbHelper.Concat("patient.LName","', '","patient.FName")+" Patient,doctor.Abbr Doctor,hygienist.Abbr Hygienist, "
					+"appointment.IsHygiene IsHygieneApt ";
				if(hasClinicsEnabled) {
					queryBrokenApts+=",COALESCE(clinic.Description,'"+POut.String(Lans.g("FormRpBrokenAppointments","Unassigned"))+"') ClinicDesc ";//Coalesce is Oracle compatible
				}
				queryBrokenApts+=
					"FROM appointment "
					+"INNER JOIN patient ON appointment.PatNum=patient.PatNum "
					+"LEFT JOIN provider doctor ON doctor.ProvNum=appointment.ProvNum "
					+"LEFT JOIN provider hygienist ON hygienist.ProvNum=appointment.ProvHyg ";
				if(hasClinicsEnabled) {
					queryBrokenApts+="LEFT JOIN clinic ON clinic.ClinicNum=appointment.ClinicNum ";
				}
				queryBrokenApts+=
					"WHERE "+DbHelper.DtimeToDate("appointment.AptDateTime")+" BETWEEN "+POut.Date(dateStart)
					+" AND "+POut.Date(dateEnd)+" "
					+"AND appointment.AptStatus="+POut.Int((int)ApptStatus.Broken)+" "
					+whereProv;
				if(hasClinicsEnabled) {
					queryBrokenApts+=whereClin+" "
						+"ORDER BY clinic.Description,appointment.AptDateTime,patient.LName,patient.FName";
				}
				else {
					queryBrokenApts+="ORDER BY appointment.AptDateTime,patient.LName,patient.FName ";
				}
			return queryBrokenApts;
		}
	}

}
