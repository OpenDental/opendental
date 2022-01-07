using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class RpProcSheet {

		///<summary>If not using clinics then supply an empty list of clinicNums.  listClinicNums must have at least one item if using clinics. Not formatted for display</summary>
		public static DataTable GetIndividualTable(DateTime dateFrom,DateTime dateTo,List<long> listProvNums,List<long> listClinicNums,string procCode,
			bool isAnyClinicMedical,bool hasAllProvs,bool hasClinicsEnabled) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvNums,listClinicNums,procCode,isAnyClinicMedical,hasAllProvs,
					hasClinicsEnabled);
			}
			string query="SELECT procedurelog.ProcDate,"
			  +DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI")+" "
			  +"AS plfname, procedurecode.ProcCode,";
			if(!isAnyClinicMedical) {
				query+="procedurelog.ToothNum,";
			}
			query+="procedurecode.Descript,provider.Abbr,";
			if(hasClinicsEnabled) {
				query+="COALESCE(clinic.Abbr,\"Unassigned\") Clinic,";
			}
			query+="procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)"
				+"-COALESCE(SUM(claimproc.WriteOff),0) $fee "//if no writeoff, then subtract 0
				+"FROM patient "
				+"INNER JOIN procedurelog ON procedurelog.PatNum=patient.PatNum "
				+"INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
				+"INNER JOIN provider ON provider.ProvNum=procedurelog.ProvNum ";
			if(hasClinicsEnabled) {
				query+="LEFT JOIN clinic ON clinic.ClinicNum=procedurelog.ClinicNum ";
			}
			query+="LEFT JOIN claimproc ON procedurelog.ProcNum=claimproc.ProcNum "
				+"AND claimproc.Status="+POut.Int((int)ClaimProcStatus.CapComplete)+" "//only CapComplete writeoffs are subtracted here.
				+"WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" ";
			if(!hasAllProvs) {
				query+="AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(hasClinicsEnabled) {
				query+="AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			if(!string.IsNullOrEmpty(procCode)) {//don't include ProcCode condition if blank, it changes the execution plan and is much slower
				query+="AND UPPER(procedurecode.ProcCode) LIKE '%"+POut.String(procCode.ToUpper())+"%' ";
			}
			query+="AND procedurelog.ProcDate >= " +POut.Date(dateFrom)+" "
				+"AND procedurelog.ProcDate <= " +POut.Date(dateTo)+" "
				+"GROUP BY procedurelog.ProcNum "
				+"ORDER BY procedurelog.ProcDate,plfname,procedurecode.ProcCode,ToothNum";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(query));
		}

		public static DataTable GetGroupedTable(DateTime dateFrom,DateTime dateTo,List<long> listProvNums,List<long> listClinicNums,string procCode,bool hasAllProvs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvNums,listClinicNums,procCode,hasAllProvs);
			}
			string query="SELECT procs.ItemName,procs.ProcCode,procs.Descript,COUNT(*),FORMAT(ROUND(AVG(procs.fee),2),2) $AvgFee,SUM(procs.fee) AS $TotFee "
				+"FROM ( "
				+"SELECT procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits) -COALESCE(SUM(claimproc.WriteOff),0) fee, "
				+"procedurecode.ProcCode,	procedurecode.Descript,	definition.ItemName, definition.ItemOrder "
				+"FROM procedurelog "
				+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				+"INNER JOIN definition ON definition.DefNum=procedurecode.ProcCat "
				+"LEFT JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum AND claimproc.Status="+POut.Int((int)ClaimProcStatus.CapComplete)+" "
				+"WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" ";
			if(!hasAllProvs) {
				query+="AND procedurelog.ProvNum IN ("+String.Join(",",listProvNums)+") ";
			}
			if(ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache)) {
				query+="AND procedurelog.ClinicNum IN ("+String.Join(",",listClinicNums)+") ";
			}
			if(!string.IsNullOrEmpty(procCode)) {//don't include ProcCode condition if blank, it changes the execution plan and is much slower
				query+="AND UPPER(procedurecode.ProcCode) LIKE '%"+POut.String(procCode.ToUpper())+"%' ";
			}
			query+="AND procedurelog.ProcDate >= " +POut.Date(dateFrom)+" "
				+"AND procedurelog.ProcDate <= " +POut.Date(dateTo)+" "
				+"GROUP BY procedurelog.ProcNum ) procs "
				+"GROUP BY procs.ProcCode "
				+"ORDER BY procs.ItemOrder,procs.ProcCode";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(query));
		}

	}
}
