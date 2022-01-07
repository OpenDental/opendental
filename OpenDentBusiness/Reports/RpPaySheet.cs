using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness {
	public class RpPaySheet {

		///<summary>If not using clinics then supply an empty list of clinicNums.  listClinicNums must have at least one item if using clinics.</summary>
		public static DataTable GetInsTable(DateTime dateFrom,DateTime dateTo,List<long> listProvNums,List<long> listClinicNums,
			List<long> listInsuranceTypes,List<long> listClaimPayGroups,bool hasAllProvs,bool hasAllClinics,bool hasInsuranceTypes,bool isGroupedByPatient,
			bool hasAllClaimPayGroups,bool doShowProvSeparate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvNums,listClinicNums,listInsuranceTypes,listClaimPayGroups,
					hasAllProvs,hasAllClinics,hasInsuranceTypes,isGroupedByPatient,hasAllClaimPayGroups,doShowProvSeparate);
			}
			string whereProv="";
			if(!hasAllProvs) {
				whereProv+=" AND claimproc.ProvNum IN(";
				for(int i=0;i<listProvNums.Count;i++) {
					if(i>0) {
						whereProv+=",";
					}
					whereProv+=POut.Long(listProvNums[i]);
				}
				whereProv+=") ";
			}
			string whereClin="";
			//reports should no longer use the cache
			bool hasClinicsEnabled=ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache);
			if(hasClinicsEnabled) {
				whereClin+=" AND claimproc.ClinicNum IN(";
				for(int i=0;i<listClinicNums.Count;i++) {
					if(i>0) {
						whereClin+=",";
					}
					whereClin+=POut.Long(listClinicNums[i]);
				}
				whereClin+=") ";
			}
			string whereClaimPayGroup="";
			if(!hasAllClaimPayGroups){
				whereClaimPayGroup=" AND PayGroup IN ("+String.Join(",",listClaimPayGroups)+") ";
			}
			string queryIns=
				@"SELECT claimproc.DateCP,carrier.CarrierName,MAX("
					+DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI")+@") lfname,GROUP_CONCAT(DISTINCT provider.Abbr) Provider, ";
			if(hasClinicsEnabled) {
				queryIns+="clinic.Abbr Clinic, ";
			}
			queryIns+=@"claimpayment.CheckNum,SUM(claimproc.InsPayAmt) amt,claimproc.ClaimNum,claimpayment.PayType,COUNT(DISTINCT claimproc.PatNum) countPats 
				FROM claimproc
				LEFT JOIN insplan ON claimproc.PlanNum = insplan.PlanNum 
				LEFT JOIN patient ON claimproc.PatNum = patient.PatNum
				LEFT JOIN carrier ON carrier.CarrierNum = insplan.CarrierNum
				LEFT JOIN provider ON provider.ProvNum=claimproc.ProvNum
				LEFT JOIN claimpayment ON claimproc.ClaimPaymentNum = claimpayment.ClaimPaymentNum ";
			if(hasClinicsEnabled) {
				queryIns+="LEFT JOIN clinic ON clinic.ClinicNum=claimproc.ClinicNum ";
			}
			queryIns+="WHERE (claimproc.Status=1 OR claimproc.Status=4) "//received or supplemental
				+whereProv
				+whereClin
				+whereClaimPayGroup
				+"AND claimpayment.CheckDate >= "+POut.Date(dateFrom)+" "
				+"AND claimpayment.CheckDate <= "+POut.Date(dateTo)+" ";
			if(!hasInsuranceTypes && listInsuranceTypes.Count>0) {
				queryIns+="AND claimpayment.PayType IN (";
				for(int i=0;i<listInsuranceTypes.Count;i++) {
					if(i>0) {
						queryIns+=",";
					}
					queryIns+=POut.Long(listInsuranceTypes[i]);
				}
				queryIns+=") ";
			}
			queryIns+=@"GROUP BY claimproc.DateCP,claimproc.ClaimPaymentNum,";
			if(doShowProvSeparate) {
				queryIns+=@"provider.ProvNum,";
			}
			if(hasClinicsEnabled) {
				queryIns+="claimproc.ClinicNum,clinic.Abbr,";
			}
			queryIns+="carrier.CarrierName,claimpayment.CheckNum";
			if(isGroupedByPatient) {
				queryIns+=",patient.PatNum";
			}
			queryIns+=" ORDER BY claimpayment.PayType,claimproc.DateCP,lfname";
			if(!hasInsuranceTypes && listInsuranceTypes.Count==0) {
				queryIns=DbHelper.LimitOrderBy(queryIns,0);
			}
			DataTable table=ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(queryIns));
			foreach(DataRow row in table.Rows) {
				//If there is more than one patient attached to a check, we will append an asterisk to the end.
				int countPats=PIn.Int(row["countPats"].ToString());
				if(countPats > 1) {
					row["lfname"]=row["lfname"].ToString().TrimEnd()+"*";
				}
			}
			table.Columns.Remove("countPats");//Remove this column because we don't want it to show on the report
			return table;
		}

		///<summary>If not using clinics, or for all clinics with clinics enabled, supply an empty list of clinicNums.  If the user is restricted, for all
		///clinics supply only those clinics for which the user has permission to access, otherwise it will be run for all clinics.</summary>
		public static DataTable GetPatTable(DateTime dateFrom,DateTime dateTo,List<long> listProvNums,List<long> listClinicNums,List<long> listPatientTypes,
			bool hasAllProvs,bool hasAllClinics,bool hasPatientTypes,bool isGroupedByPatient,bool isUnearnedIncluded,bool doShowProvSeparate,
			bool doShowHiddenTPUnearned) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listProvNums,listClinicNums,listPatientTypes,hasAllProvs,hasAllClinics,
					hasPatientTypes,isGroupedByPatient,isUnearnedIncluded,doShowProvSeparate,doShowHiddenTPUnearned);
			}
			//reports should no longer use the cache
			bool hasClinicsEnabled=ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache);
			List<long> listHiddenUnearnedDefNums=new List<long>();
			if(!doShowHiddenTPUnearned) {
				listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() =>
					Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
				);
			}
			//patient payments-----------------------------------------------------------------------------------------
			//the selected columns have to remain in this order due to the way the report complex populates the returned sheet
			string queryPat="SELECT payment.PayDate DatePay,"
				+"MAX("+DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI")+") lfname,GROUP_CONCAT(DISTINCT provider.Abbr),";
			if(hasClinicsEnabled) {
				queryPat+="clinic.Abbr clinicAbbr,";
			}
			queryPat+="payment.CheckNum,SUM(COALESCE(paysplit.SplitAmt,0)) amt,payment.PayNum,ItemName,payment.PayType "
				+"FROM payment "
				+"LEFT JOIN paysplit ON payment.PayNum=paysplit.PayNum "
				+"LEFT JOIN patient ON payment.PatNum=patient.PatNum "
				+"LEFT JOIN provider ON paysplit.ProvNum=provider.ProvNum "
				+"LEFT JOIN definition ON payment.PayType=definition.DefNum ";
			if(hasClinicsEnabled) {
				queryPat+="LEFT JOIN clinic ON clinic.ClinicNum=paysplit.ClinicNum ";
			}
			queryPat+="WHERE payment.PayDate BETWEEN "+POut.Date(dateFrom)+" AND "+POut.Date(dateTo)+" ";
			if(hasClinicsEnabled && listClinicNums.Count>0) {
				queryPat+="AND paysplit.ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			if(!hasAllProvs && listProvNums.Count>0) {
				queryPat+="AND paysplit.ProvNum IN("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+") ";
			}
			if(!hasPatientTypes && listPatientTypes.Count>0) {
				queryPat+="AND payment.PayType IN ("+string.Join(",",listPatientTypes.Select(x => POut.Long(x)))+") ";
			}
			if(!isUnearnedIncluded) {//UnearnedType of 0 means the paysplit is NOT unearned
				queryPat+="AND paysplit.UnearnedType=0 ";
			}
			else if(listHiddenUnearnedDefNums.Count>0 && !doShowHiddenTPUnearned) {//Include unearned but not of the TP type. 
				queryPat+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			queryPat+="GROUP BY payment.PayNum,payment.PayDate,payment.CheckNum,definition.ItemName,payment.PayType ";
			if(doShowProvSeparate) {
				queryPat+=",provider.ProvNum ";
			}
			if(hasClinicsEnabled) {
				queryPat+=",clinic.Abbr ";
			}
			if(isGroupedByPatient) {
				queryPat+=",patient.PatNum ";
			}
			queryPat+="ORDER BY payment.PayType,payment.PayDate,lfname";
			if(!hasPatientTypes && listPatientTypes.Count==0) {
				queryPat=DbHelper.LimitOrderBy(queryPat,0);
			}
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(queryPat));
		}

	}
}
