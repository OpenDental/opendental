using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class RpPayPlan {

		///<summary>If not using clinics then supply an empty list of clinicNums.</summary>
		public static DataSet GetPayPlanTable(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listClinicNums,
			bool hasAllProvs,DisplayPayPlanType displayPayPlanType,bool hideCompletedPlans,bool showFamilyBalance,bool hasDateRange,bool isPayPlanV2) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetDS(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listClinicNums,hasAllProvs,displayPayPlanType,
					hideCompletedPlans,showFamilyBalance,hasDateRange,isPayPlanV2);
			}
			string whereProv="";
			if(!hasAllProvs) {
				whereProv+=" AND payplancharge.ProvNum IN(";
				for(int i=0;i<listProvNums.Count;i++) {
					if(i>0) {
						whereProv+=",";
					}
					whereProv+=POut.Long(listProvNums[i]);
				}
				whereProv+=") ";
			}
			string whereClin="";
			bool hasClinicsEnabled=ReportsComplex.RunFuncOnReportServer(() => Prefs.HasClinicsEnabledNoCache);
			if(hasClinicsEnabled) {//Using clinics
				whereClin+=" AND payplancharge.ClinicNum IN(";
				for(int i=0;i<listClinicNums.Count;i++) {
					if(i>0) {
						whereClin+=",";
					}
					whereClin+=POut.Long(listClinicNums[i]);
				}
				whereClin+=") ";
			}
			DataSet ds=new DataSet();
			DataTable table=new DataTable("Clinic");
			table.Columns.Add("provider");
			table.Columns.Add("guarantor");
			table.Columns.Add("ins");
			table.Columns.Add("princ");
			table.Columns.Add("accumInt");
			table.Columns.Add("paid");
			table.Columns.Add("balance");
			table.Columns.Add("due");
			if(isPayPlanV2) {
				table.Columns.Add("notDue");
			}
			table.Columns.Add("famBal");
			table.Columns.Add("clinicName");
			DataTable tableTotals=new DataTable("Total");
			tableTotals.Columns.Add("clinicName");
			tableTotals.Columns.Add("princ");
			tableTotals.Columns.Add("accumInt");
			tableTotals.Columns.Add("paid");
			tableTotals.Columns.Add("balance");
			tableTotals.Columns.Add("due");
			if(isPayPlanV2) {
				tableTotals.Columns.Add("notDue");
			}
			tableTotals.Columns.Add("famBal");
			DataRow row;
			string datesql="CURDATE()";//This is used to find out how much people owe currently and has nothing to do with the selected range
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				datesql="(SELECT CURRENT_DATE FROM dual)";
			}
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			string command="SELECT FName,LName,MiddleI,PlanNum,Preferred,PlanNum, "
				+"COALESCE((SELECT SUM(Principal+Interest) FROM payplancharge WHERE payplancharge.PayPlanNum=payplan.PayPlanNum "
				+"AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "//for v1, debits are the only ChargeType.
					+"AND ChargeDate <= "+datesql+@"),0) '_accumDue', ";
			command+="COALESCE((SELECT SUM(Interest) FROM payplancharge WHERE payplancharge.PayPlanNum=payplan.PayPlanNum "
				+"AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "//for v1, debits are the only ChargeType.
					+"AND ChargeDate <= "+datesql+@"),0) '_accumInt', ";
			command+=$@"COALESCE((SELECT SUM(SplitAmt) FROM paysplit WHERE paysplit.PayPlanNum=payplan.PayPlanNum ";
			if(listHiddenUnearnedDefNums.Count>0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			command+=@"AND paysplit.PayPlanNum!=0),0) '_paid', 
					COALESCE((SELECT SUM(InsPayAmt) FROM claimproc WHERE claimproc.PayPlanNum=payplan.PayPlanNum 
					AND claimproc.Status IN("
					+POut.Int((int)ClaimProcStatus.Received)+","
					+POut.Int((int)ClaimProcStatus.Supplemental)+","
					+POut.Int((int)ClaimProcStatus.CapClaim)
					+") AND claimproc.PayPlanNum!=0),0) '_insPaid', ";
			command+="COALESCE(CASE "
					//When pay plan isn't dynamic, all charges are already created, so we can sum principal from them to get plans total principal.
					+"WHEN payplan.IsDynamic=0 THEN "
						+"(SELECT SUM(Principal) FROM payplancharge "
						//for v1, debits are the only ChargeType.
						+"WHERE payplancharge.PayPlanNum=payplan.PayPlanNum AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+") "
					//When pay plan is dynamic, we will get it from dppprincipal, a table constructed to calculate total principal for dynamic pay plans.
					+"WHEN payplan.IsDynamic=1 THEN dppprincipal.TotalPrincipal ELSE 0 END,0)'_principal', "
				+"COALESCE((SELECT SUM(Principal) FROM payplancharge WHERE payplancharge.PayPlanNum=payplan.PayPlanNum "
				+"AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Credit)+"),0) '_credits', "//for v1, will always be 0.
				+"COALESCE((SELECT SUM(Principal) FROM payplancharge WHERE payplancharge.PayPlanNum=payplan.PayPlanNum "
				+"AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Credit)+" AND ChargeDate > "+datesql+"),0) '_notDue', "
				+"patient.PatNum PatNum, "
				+"payplancharge.ProvNum ProvNum ";
			if(hasClinicsEnabled) {
				command+=", payplancharge.ClinicNum ClinicNum ";
			}
			//In order to determine if the patient has completely paid off their payment plan we need to get the total amount of interest as of today.
			//Then, after the query has run, we'll add the interest up until today with the total principal for the entire payment plan.
			//For this reason, we cannot use _accumDue which only gets the principle up until today and not the entire payment plan principle.
			command+=",COALESCE((SELECT SUM(Interest) FROM payplancharge WHERE payplancharge.PayPlanNum=payplan.PayPlanNum "
					+"AND payplancharge.ChargeType="+POut.Int((int)PayPlanChargeType.Debit)+" "//for v1, debits are the only ChargeType.
					+"AND ChargeDate <= "+datesql+@"),0) '_interest' "
				+"FROM payplan "
				+"LEFT JOIN patient ON patient.PatNum=payplan.Guarantor "
				+"LEFT JOIN payplancharge ON payplan.PayPlanNum=payplancharge.PayPlanNum "
				+"LEFT JOIN "
					//construct dppprincipal (dynamic payment plan principal) table
					+"(SELECT payplanlink.PayPlanNum,"
						//Sum total production linked to dynamic pay plan for all linked adjustments and procedures 
						+"ROUND(SUM(CASE "
							+"WHEN payplanlink.AmountOverride!=0 THEN payplanlink.AmountOverride "//If override isn't zero, use it in sum
							+"ELSE (CASE "//Otherwise, use adjustment amount or total proc fee for linked production
								+"WHEN payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Adjustment)+" THEN adjustment.AdjAmt "
								+"WHEN payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Procedure)+" "
								+"THEN procedurelog.ProcFee*GREATEST(1,procedurelog.BaseUnits+procedurelog.UnitQty) "
								+"ELSE 0 END)"
							//Factor in non-payplan pay splits, adjustments to procedures, and insurance estimates, payments, writeoffs, and estimated writeoffs
							+"-COALESCE(sumsplit.SumSplit,0)+COALESCE(sumprocadj.SumProcAdj,0)-COALESCE(sumins.SumIns,0) "
							+"END),2) AS 'TotalPrincipal' "
					+"FROM payplanlink "
						+"LEFT JOIN adjustment ON adjustment.AdjNum=payplanlink.FKey AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Adjustment)+" "
						+"LEFT JOIN procedurelog ON procedurelog.ProcNum=payplanlink.FKey AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Procedure)+" "
						+"LEFT JOIN "//Table to sum all paysplits made to linked production outside of pay plan.
							+"(SELECT paysplit.ProcNum,paysplit.AdjNum,SUM(paysplit.SplitAmt) AS 'SumSplit' "
							+"FROM paysplit WHERE paysplit.PayPlanNum=0 AND paysplit.PayPlanChargeNum=0 GROUP BY paysplit.ProcNum,paysplit.AdjNum) "
							+"AS sumsplit ON (payplanlink.FKey=sumsplit.ProcNum AND sumsplit.ProcNum!=0 AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Procedure)+") OR "
							+"(payplanlink.FKey=sumsplit.AdjNum AND sumsplit.AdjNum!=0 AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Adjustment)+") "
						+"LEFT JOIN "//Table to sum all adjustments made to linked procedures.
							+"(SELECT adjustment.ProcNum, SUM(adjustment.AdjAmt) AS 'SumProcAdj' FROM adjustment GROUP BY adjustment.ProcNum) "
							+"AS sumprocadj ON sumprocadj.ProcNum=procedurelog.ProcNum AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Procedure)+" "
						+"LEFT JOIN "//Table to sum all ins estimates, ins payments, estimated writeoffs, and writeoffs to linked procedures.
							+"(SELECT claimproc.ProcNum, SUM(CASE "
								+"WHEN claimproc.Status IN ("+string.Join(",",ClaimProcs.GetInsPaidStatuses().Select(x => POut.Int((int)x)))+") "
								+"THEN claimproc.InsPayAmt+claimproc.WriteOff "
								+"WHEN claimproc.Status IN ("+string.Join(",",ClaimProcs.GetEstimatedStatuses().Select(x => POut.Int((int)x)))+") "
								+"THEN claimproc.InsPayEst+(CASE "
									+"WHEN claimproc.WriteOffEstOverride!=-1 THEN claimproc.WriteOffEstOverride "
									+"WHEN claimproc.WriteOffEst!=-1 THEN claimproc.WriteOffEst "
									+"ELSE 0 END) "
								+"ELSE 0 END) AS 'SumIns' "
							+"FROM claimproc GROUP BY claimproc.ProcNum) AS sumins "
							+"ON procedurelog.ProcNum=sumins.ProcNum AND payplanlink.LinkType="+POut.Int((int)PayPlanLinkType.Procedure)+" "
					//Grouped by PayPlanNum so that we can sum total principal for each dynamic pay plan.
					+"GROUP BY payplanlink.PayPlanNum) AS dppprincipal ON dppprincipal.PayPlanNum=payplan.PayPlanNum AND payplan.IsDynamic=1 "
				+"WHERE TRUE ";//Always include true, so that the WHERE clause may always be present.
			if(hasDateRange) {
				command+="AND payplan.PayPlanDate >= "+POut.Date(dateStart)+" "
				+"AND payplan.PayPlanDate <= "+POut.Date(dateEnd)+" ";
			}
			command+=whereProv
				+whereClin;
			if(displayPayPlanType==DisplayPayPlanType.Insurance) {
				command+="AND payplan.PlanNum!=0 ";
			}
			else if(displayPayPlanType==DisplayPayPlanType.Patient) {
				command+="AND payplan.PlanNum=0 ";
			}
			else if(displayPayPlanType==DisplayPayPlanType.Both) {
				//Do not filter the query at all which will show both insurance and patient payment plan types.
			}
			if(hideCompletedPlans) {
				command+="AND payplan.IsClosed=0 ";
			}
			command+="GROUP BY FName,LName,MiddleI,Preferred,payplan.PayPlanNum ";
			if(hasClinicsEnabled) {
				command+="ORDER BY ClinicNum,LName,FName";
			}
			else {
				command+="ORDER BY LName,FName";
			}
			DataTable raw=ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(command));
			List<Provider> listProvs=ReportsComplex.RunFuncOnReportServer(() => Providers.GetAll());
			List<Clinic> listClinics = new List<Clinic>();
            if(hasClinicsEnabled) {
				listClinics=ReportsComplex.RunFuncOnReportServer(() => Clinics.GetClinicsNoCache());
			}
			//DateTime payplanDate;
			Patient pat;
			double princ;
			double paid;
			double interest;
			double accumDue;
			double notDue;
			decimal famBal=0;
			double princTot=0;
			double paidTot=0;
			double interestTot=0;
			double balanceTot=0;
			double accumDueTot=0;
			double notDueTot=0;
			decimal famBalTot=0;
			string clinicAbbrOld="";
			for(int i=0;i<raw.Rows.Count;i++) {
				princ=PIn.Double(raw.Rows[i]["_principal"].ToString());
				interest=PIn.Double(raw.Rows[i]["_accumInt"].ToString());
				if(raw.Rows[i]["PlanNum"].ToString()=="0") {//pat payplan
					paid=PIn.Double(raw.Rows[i]["_paid"].ToString());
				}
				else {//ins payplan
					paid=PIn.Double(raw.Rows[i]["_insPaid"].ToString());
				}
				accumDue=PIn.Double(raw.Rows[i]["_accumDue"].ToString());
				notDue=PIn.Double(raw.Rows[i]["_notDue"].ToString());
				row=table.NewRow();
				//payplanDate=PIn.PDate(raw.Rows[i]["PayPlanDate"].ToString());
				//row["date"]=raw.Rows[i]["PayPlanDate"].ToString();//payplanDate.ToShortDateString();
				pat=new Patient();
				pat.LName=raw.Rows[i]["LName"].ToString();
				pat.FName=raw.Rows[i]["FName"].ToString();
				pat.MiddleI=raw.Rows[i]["MiddleI"].ToString();
				pat.Preferred=raw.Rows[i]["Preferred"].ToString();
				row["provider"]=Providers.GetLName(PIn.Long(raw.Rows[i]["ProvNum"].ToString()),listProvs);
				row["guarantor"]=pat.GetNameLF();
				if(raw.Rows[i]["PlanNum"].ToString()=="0") {
					row["ins"]="";
				}
				else {
					row["ins"]="X";
				}
				row["princ"]=princ.ToString("f");
				row["accumInt"]=interest.ToString("f");
				row["paid"]=paid.ToString("f");
				row["balance"]=(princ+interest-paid).ToString("f");
				row["due"]=(accumDue-paid).ToString("f");
				if(isPayPlanV2) {
					row["notDue"]=((princ+interest-paid)-(accumDue-paid)).ToString("f");
				}
				if(showFamilyBalance) {
					// this could be done better, by getting a list of guarantors outside of the loop and pulling the family balance value from a list
					// we can implement something like this if a customer experiences slowness from this.
					Family famCur=ReportsComplex.RunFuncOnReportServer(() => Patients.GetFamily(PIn.Long(raw.Rows[i]["PatNum"].ToString())));
					famBal=(decimal)famCur.ListPats[0].BalTotal;
					row["famBal"]=(famBal - (decimal)famCur.ListPats[0].InsEst).ToString("F");
				}
				if(hasClinicsEnabled) {//Using clinics
					string clinicAbbr=Clinics.GetAbbr(PIn.Long(raw.Rows[i]["ClinicNum"].ToString()),listClinics);
					clinicAbbr=(clinicAbbr=="")?Lans.g("FormRpPayPlans","Unassigned"):clinicAbbr;
					if(!String.IsNullOrEmpty(clinicAbbrOld) && clinicAbbr!=clinicAbbrOld) {//Reset all the total values
						DataRow rowTot=tableTotals.NewRow();
						rowTot["clinicName"]=clinicAbbrOld;
						rowTot["princ"]=princTot.ToString();
						rowTot["accumInt"]=interestTot.ToString();
						rowTot["paid"]=paidTot.ToString();
						rowTot["balance"]=balanceTot.ToString();
						rowTot["due"]=accumDueTot.ToString();
						if(isPayPlanV2) {
							rowTot["notDue"]=notDueTot.ToString();
						}	
						rowTot["famBal"]=famBalTot.ToString();
						tableTotals.Rows.Add(rowTot);
						princTot=0;
						paidTot=0;
						interestTot=0;
						accumDueTot=0;
						balanceTot=0;
						notDueTot=0;
						famBalTot=0;
					}
					row["clinicName"]=clinicAbbr;
					clinicAbbrOld=clinicAbbr;
					princTot+=princ;
					paidTot+=paid;
					interestTot+=interest;
					accumDueTot+=(accumDue-paid);
					balanceTot+=(princ+interest-paid);
					notDueTot+=((princ+interest-paid)-(accumDue-paid));
					famBalTot+=famBal;
					if(i==raw.Rows.Count-1) {
						DataRow rowTot=tableTotals.NewRow();
						rowTot["clinicName"]=clinicAbbrOld;
						rowTot["princ"]=princTot.ToString();
						rowTot["accumInt"]=interestTot.ToString();
						rowTot["paid"]=paidTot.ToString();
						rowTot["balance"]=balanceTot.ToString();
						rowTot["due"]=accumDueTot.ToString();
						if(isPayPlanV2) {
							rowTot["notDue"]=notDueTot.ToString();
						}	
						rowTot["famBal"]=famBalTot.ToString();
						tableTotals.Rows.Add(rowTot);
					}
				}
				table.Rows.Add(row);
			}
			ds.Tables.Add(table);
			ds.Tables.Add(tableTotals);
			return ds;
		}

	}

	///<summary>Used to dictate which payment plan types are shown in the payment plan report.</summary>
	public enum DisplayPayPlanType {
		///<summary>0</summary>
		Patient,
		///<summary>1</summary>
		Insurance,
		///<summary>2</summary>
		Both
	}
}
