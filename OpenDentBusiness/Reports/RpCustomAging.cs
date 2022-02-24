using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using System.ComponentModel;

namespace OpenDentBusiness {
	public class RpCustomAging {
		public static List<AgingPat> GetAgingList(AgingOptions ageOptions) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<AgingPat>>(MethodBase.GetCurrentMethod(),ageOptions);
			}
			string command = @"SELECT pat.FName, pat.LName, pat.PatNum, ";
			if(ageOptions.AgeCredits) {
				command+=@"
				ROUND(tSums.ChargesOver90-tSums.CreditsOver90,3) BalOver90,
				ROUND(tSums.Charges_61_90-tSums.Credits_61_90,3) Bal_61_90,
				ROUND(tSums.Charges_31_60-tSums.Credits_31_60,3) Bal_31_60,
				ROUND(tSums.Charges_0_30-tSums.Credits_0_30,3) Bal_0_30,
				ROUND(tSums.TotalCharges-tSums.TotalCredits,3) BalTotal
				FROM (";
			}
			else {
				command+=@"ROUND(CASE WHEN tSums.TotalCredits >= tSums.ChargesOver90 THEN 0 
					ELSE tSums.ChargesOver90-tSums.TotalCredits END,3) BalOver90,
				ROUND(CASE WHEN tSums.TotalCredits <= tSums.ChargesOver90 THEN tSums.Charges_61_90 
					WHEN tSums.ChargesOver90 + tSums.Charges_61_90 <= tSums.TotalCredits THEN 0 
					ELSE tSums.ChargesOver90 + tSums.Charges_61_90-tSums.TotalCredits END,3) Bal_61_90,
				ROUND(CASE WHEN tSums.TotalCredits < tSums.ChargesOver90 + tSums.Charges_61_90 THEN tSums.Charges_31_60 
					WHEN tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 <= tSums.TotalCredits THEN 0 
					ELSE tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60-tSums.TotalCredits END,3) Bal_31_60,
				ROUND(CASE WHEN tSums.TotalCredits < tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 THEN tSums.Charges_0_30 
					WHEN tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 + tSums.Charges_0_30 <= tSums.TotalCredits THEN 0 
					ELSE tSums.ChargesOver90 + tSums.Charges_61_90 + tSums.Charges_31_60 + tSums.Charges_0_30-tSums.TotalCredits END,3) Bal_0_30,
				ROUND(tSums.TotalCharges-tSums.TotalCredits,3) BalTotal
				FROM (";
			}
			command += @"
					SELECT patient.PatNum,  patient.Guarantor,
					SUM(CASE WHEN trans.TranAmount > 0 AND trans.TranDate >= "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 30 DAY THEN trans.TranAmount ELSE 0 END) Charges_0_30,
					SUM(CASE WHEN trans.TranAmount > 0 AND trans.TranDate BETWEEN "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 60 DAY AND "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 31 DAY THEN trans.TranAmount ELSE 0 END) Charges_31_60,
					SUM(CASE WHEN trans.TranAmount > 0 AND trans.TranDate BETWEEN "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 90 DAY AND "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 61 DAY THEN trans.TranAmount ELSE 0 END) Charges_61_90,
					SUM(CASE WHEN trans.TranAmount > 0 AND trans.TranDate < "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 90 DAY THEN trans.TranAmount ELSE 0 END) ChargesOver90,
					SUM(CASE WHEN trans.TranAmount > 0 THEN trans.TranAmount ELSE 0 END) AS TotalCharges,
					-SUM(CASE WHEN trans.TranAmount < 0 AND trans.TranDate >= "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 30 DAY THEN trans.TranAmount ELSE 0 END) Credits_0_30,
					-SUM(CASE WHEN trans.TranAmount < 0 AND trans.TranDate BETWEEN "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 60 DAY AND "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 31 DAY THEN trans.TranAmount ELSE 0 END) Credits_31_60,
					-SUM(CASE WHEN trans.TranAmount < 0 AND trans.TranDate BETWEEN "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 90 DAY AND "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 61 DAY THEN trans.TranAmount ELSE 0 END) Credits_61_90,
					-SUM(CASE WHEN trans.TranAmount < 0 AND trans.TranDate < "+POut.Date(ageOptions.DateAsOf)+@"-INTERVAL 90 DAY THEN trans.TranAmount ELSE 0 END) CreditsOver90,
					-SUM(CASE WHEN trans.TranAmount < 0 THEN trans.TranAmount ELSE 0 END) AS TotalCredits
					FROM patient
					INNER JOIN (";
			string transQueries = "";
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.ProcedureFees)) {
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetProcAgingQuery(ageOptions);
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.Adjustments)) {
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetAdjAgingQuery(ageOptions);
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PayPlanCharges) 
				|| ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PayPlanCredits)) 
			{
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetPayPlanAgingQuery(ageOptions);
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PayPlanCredits)) {//for dynamic payment plan credits
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetDynamicPayPlanCredits(ageOptions);
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PatPayments)) {
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetPatPayAgingQuery(ageOptions);
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.InsPayments)) {
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetInsPayAgingQuery(ageOptions);
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.InsEsts)) {
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetInsEstAgingQuery(ageOptions);
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.Writeoffs)) {
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetWriteoffAgingQuery(ageOptions);
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.WriteoffEsts)) {
				transQueries += String.IsNullOrWhiteSpace(transQueries) ? "" : " UNION ALL ";
				transQueries += GetWriteoffEstAgingQuery(ageOptions);
			}
			command += transQueries + @"
				)trans ON patient.PatNum = trans.PatNum
			WHERE TRUE ";
			if(ageOptions.ListBillTypes != null && ageOptions.ListBillTypes.Count > 0) {
				command+=" AND patient.BillingType IN ("+String.Join(",",ageOptions.ListBillTypes.Select(x => x.DefNum))+") ";
			}
			if(ageOptions.ListProvs != null && ageOptions.ListProvs.Count>0) {
				command+=@" AND patient.PriProv IN ("+String.Join(",",ageOptions.ListProvs.Select(x => x.ProvNum))+") ";
			}
			if(ageOptions.ListClins != null && ageOptions.ListClins.Count>0) {
				command+=@" AND patient.ClinicNum IN ("+String.Join(",",ageOptions.ListClins.Select(x => x.ClinicNum))+") ";
			}
			if(ageOptions.FamGroup == AgingOptions.FamilyGrouping.Individual) {
				command+=@" GROUP BY patient.PatNum 
					)tSums 
					INNER JOIN patient pat on pat.PatNum = tSums.PatNum ";
			}
			else {
				command+=@" GROUP BY patient.Guarantor 
					)tSums
					INNER JOIN patient pat on pat.PatNum = tSums.Guarantor ";
			}
			if(ageOptions.ExcludeInactive) {//made to match the way regular aging looks a patient status. 
				command+=" AND pat.PatStatus != "+(int)PatientStatus.Inactive+" ";
			}
			if(ageOptions.ExcludeArchive) {
				command+=" AND pat.PatStatus != "+(int)PatientStatus.Archived+" ";
			}
			if(ageOptions.ExcludeBadAddress) {
				command+=" AND pat.Zip != '' ";
			}
			command+="HAVING TRUE	";
			if(ageOptions.AgeAccount == AgeOfAccount.Over30) {
				command+=@" AND (Bal_31_60 != 0 OR Bal_61_90 != 0 OR BalOver90 != 0) ";
			}
			else if(ageOptions.AgeAccount == AgeOfAccount.Over60) {
				command+=@" AND (Bal_61_90 != 0 OR BalOver90 != 0) ";
			}
			else if(ageOptions.AgeAccount == AgeOfAccount.Over90) {
				command+=@" AND (BalOver90 != 0) ";
			}
			if(ageOptions.NegativeBalOptions == AgingOptions.NegativeBalAgingOptions.Exclude) {
				command +=" AND BalTotal > 0 ";
			}
			else if(ageOptions.NegativeBalOptions == AgingOptions.NegativeBalAgingOptions.Include) {
				command +=" AND BalTotal != 0 ";
			}
			else {
				command +=" AND BalTotal < 0 ";
			}
			command += " ORDER BY pat.LName, pat.FName ";
			DataTable table = Db.GetTable(command);
			List<AgingPat> retVal = new List<AgingPat>();
			foreach(DataRow row in table.Rows) {
				Patient patLim = new Patient() {
					PatNum = PIn.Long(row["PatNum"].ToString()),
					FName = PIn.String(row["FName"].ToString()),
					LName = PIn.String(row["LName"].ToString()),
				};
				AgingPat agingPatCur = new AgingPat() {
					Pat = patLim,
					BalZeroThirty = PIn.Double(row["Bal_0_30"].ToString()),
					BalThirtySixty = PIn.Double(row["Bal_31_60"].ToString()),
					BalSixtyNinety= PIn.Double(row["Bal_61_90"].ToString()),
					BalOverNinety = PIn.Double(row["BalOver90"].ToString()),
					BalTotal = PIn.Double(row["BalTotal"].ToString()),
				};
				retVal.Add(agingPatCur);
			}
			return retVal;
		}

		private static string GetProcAgingQuery(AgingOptions ageOptions) {
			return @"
				SELECT 'Proc' TranType,pl.PatNum,pl.ProcDate TranDate,pl.ProcFee*(pl.UnitQty+pl.BaseUnits) TranAmount 
				FROM procedurelog pl 
				WHERE pl.ProcStatus="+(int)ProcStat.C+@"
				AND pl.ProcFee != 0 
				AND pl.ProcDate <= " +POut.Date(ageOptions.DateAsOf) + " ";
		}

		private static string GetAdjAgingQuery(AgingOptions ageOptions) {
			return @"
				SELECT 'Adj' TranType,adj.PatNum,adj.AdjDate TranDate,adj.AdjAmt TranAmount 
				FROM adjustment adj
				WHERE adj.AdjAmt != 0 
				AND adj.AdjDate <= " +POut.Date(ageOptions.DateAsOf)+ " ";
		}

		private static string GetPayPlanAgingQuery(AgingOptions ageOptions) {
			string chargeTypeInclude = "";
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PayPlanCharges)) {
				chargeTypeInclude+=(int)PayPlanChargeType.Debit;
			}
			if(ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PayPlanCredits)) {
				if(chargeTypeInclude!="") {
					chargeTypeInclude+=",";
				}
				chargeTypeInclude+=(int)PayPlanChargeType.Credit;
			}
			chargeTypeInclude = "("+chargeTypeInclude +") ";
			string command;
			command=@"
					SELECT 'PPComplete' TranType,ppc.Guarantor PatNum,ppc.ChargeDate TranDate,
					(CASE WHEN ppc.ChargeType != "+POut.Int((int)PayPlanChargeType.Debit)+@" THEN -ppc.Principal 
					WHEN pp.PlanNum=0 THEN ppc.Principal+ppc.Interest ELSE 0 END) TranAmount
					FROM payplancharge ppc 
					LEFT JOIN payplan pp ON pp.PayPlanNum=ppc.PayPlanNum 
					WHERE ppc.ChargeDate <= "+POut.Date(ageOptions.DateAsOf)  +@"
					AND ppc.ChargeType IN " + chargeTypeInclude + " ";
			return command;
		}

		private static string GetDynamicPayPlanCredits(AgingOptions ageOptions) {
			if(!ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PayPlanCredits)) {//we already checked for this, shouldn't happen but just in case.
				return "";
			}
			string command=@$"
				SELECT 'PayPlanLink' TranType,prodlink.PatNum PatNum,DATE(payplanlink.SecDateTEntry) TranDate,
					(CASE WHEN payplanlink.AmountOverride=0 THEN -prodlink.Fee ELSE -payplanlink.AmountOverride END) TranAmount
				FROM payplanlink
				LEFT JOIN ( 
					SELECT procedurelog.PatNum,(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)
								+COALESCE(procAdj.AdjAmt,0)+COALESCE(procClaimProc.InsPay,0)
								+COALESCE(procClaimProc.WriteOff,0)
								+COALESCE(procSplit.SplitAmt,0)
							) Fee,payplanlink.PayPlanLinkNum LinkNum,procedurelog.ProcNum,procedurelog.ProcDate AgeDate 
					FROM payplanlink
					INNER JOIN payplan ON payplanlink.PayPlanNum=payplan.PayPlanNum
					INNER JOIN procedurelog ON procedurelog.ProcNum=payplanlink.FKey
						AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Procedure)}
						AND !(payplan.dynamicPayPlanTPOption={POut.Int((int)DynamicPayPlanTPOptions.AwaitComplete)} AND procedurelog.ProcStatus!={POut.Int((int)ProcStat.C)})
					LEFT JOIN (
						SELECT SUM(adjustment.AdjAmt) AdjAmt,adjustment.ProcNum,adjustment.PatNum,adjustment.ProvNum,adjustment.ClinicNum
						FROM adjustment 
						GROUP BY adjustment.ProcNum,adjustment.PatNum,adjustment.ProvNum,adjustment.ClinicNum
					)procAdj ON procAdj.ProcNum=procedurelog.ProcNum
						AND procAdj.PatNum=procedurelog.PatNum
						AND procAdj.ProvNum=procedurelog.ProvNum
						AND procAdj.ClinicNum=procedurelog.ClinicNum
					LEFT JOIN (
						SELECT SUM(COALESCE((CASE WHEN claimproc.Status IN (
								{POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapComplete)}
							) THEN claimproc.InsPayAmt 
								WHEN claimproc.InsEstTotalOverride!=-1 THEN claimproc.InsEstTotalOverride ELSE claimproc.InsPayEst END),0)*-1) InsPay
							,SUM(COALESCE((CASE WHEN claimproc.Status IN (
								{POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapComplete)}
							)	THEN claimproc.WriteOff 
								WHEN claimproc.WriteOffEstOverride!=-1 THEN claimproc.WriteOffEstOverride 
								WHEN claimproc.WriteOffEst!=-1 THEN claimproc.WriteOffEst ELSE 0 END),0)*-1) WriteOff
							,claimproc.ProcNum
						FROM claimproc 
						WHERE claimproc.Status!={POut.Int((int)ClaimProcStatus.Preauth)} 
						GROUP BY claimproc.ProcNum
					)procClaimProc ON procClaimProc.ProcNum=procedurelog.ProcNum 
					LEFT JOIN (
						SELECT SUM(paysplit.SplitAmt)*-1 SplitAmt,paysplit.ProcNum
						FROM paysplit
						WHERE paysplit.PayPlanNum=0 
						GROUP BY paysplit.ProcNum
					)procSplit ON procSplit.ProcNum=procedurelog.ProcNum
					UNION ALL 
					SELECT adjustment.PatNum,adjustment.AdjAmt + COALESCE(adjSplit.SplitAmt,0) Fee,payplanlink.PayPlanLinkNum LinkNum
							,0 ProcNum,DATE('0001-01-01') AgeDate 
					FROM payplanlink 
					INNER JOIN adjustment ON adjustment.AdjNum=payplanlink.FKey AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Adjustment)} 
					LEFT JOIN (
							SELECT SUM(COALESCE(paysplit.SplitAmt,0))*-1 SplitAmt,paysplit.AdjNum
							FROM paysplit
							WHERE paysplit.PayPlanNum=0 
							GROUP BY paysplit.AdjNum
					)adjSplit ON adjSplit.AdjNum=adjustment.AdjNum
				) prodlink ON prodlink.LinkNum=payplanlink.PayPlanLinkNum 
				WHERE prodlink.AgeDate <= {POut.Date(ageOptions.DateAsOf)} ";
			return command;
		}

		///<summary>If the user has opted to age credits and debits for payment plans, all payments are included.
		///Otherwise, only payments not attached to payment plans are included.
		///This is determined by the user's choice for this particular report, NOT their practice-wide preference.</summary>
		private static string GetPatPayAgingQuery(AgingOptions ageOptions) {
			string command = @"
				SELECT 'PatPay' TranType,ps.PatNum,ps.DatePay TranDate,-ps.SplitAmt TranAmount 
				FROM paysplit ps
				WHERE ps.SplitAmt != 0
				AND ps.DatePay <= " +POut.Date(ageOptions.DateAsOf) + " ";
			if(!ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PayPlanCharges) 
				|| !ageOptions.AgingInc.HasFlag(AgingOptions.AgingInclude.PayPlanCredits)) {
				command+=@"
					AND ps.PayPlanNum = 0 ";
			}
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).Where(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			if(listHiddenUnearnedDefNums.Count > 0) {
				command+="AND ps.UnearnedType NOT IN ("+string.Join(",",listHiddenUnearnedDefNums)+") ";
			}
			return command;
		}

		private static string GetInsPayAgingQuery(AgingOptions ageOptions) {
			string command = @"
				SELECT 'InsPay' TranType,cp.PatNum,cp.DateCP TranDate,-cp.InsPayAmt TranAmount 
				FROM claimproc cp
				WHERE cp.Status IN ("+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.Supplemental)+","
					+POut.Int((int)ClaimProcStatus.CapClaim)+","+POut.Int((int)ClaimProcStatus.CapComplete)+@") 
				AND cp.InsPayAmt != 0 
				AND cp.PayPlanNum = 0 
				AND cp.DateCP <= " +POut.Date(ageOptions.DateAsOf);
			return command;
		}

		private static string GetInsEstAgingQuery(AgingOptions ageOptions) {
			string command = @"
				SELECT 'InsEst' TranType,cp.PatNum,cp.DateCP TranDate,-cp.InsPayEst TranAmount 
				FROM claimproc cp
				WHERE cp.Status = "+POut.Int((int)ClaimProcStatus.NotReceived)+@" 
				AND cp.InsPayEst != 0 
				AND cp.DateCP <= " +POut.Date(ageOptions.DateAsOf);
			return command;
		}

		private static string GetWriteoffAgingQuery(AgingOptions ageOptions) {
			List<ClaimProcStatus> listClaimProcStatus=new List<ClaimProcStatus>() {
				ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim,ClaimProcStatus.CapComplete
			};
			string statusIn=string.Join(",",listClaimProcStatus.Select(x => POut.Int((int)x)));
			string command;
			if(ageOptions.WriteoffOptions == PPOWriteoffDateCalc.InsPayDate){
				command="SELECT 'Writeoff' TranType,"
					+"cp.PatNum,"
					+"cp.DateCP TranDate,"
					+"-cp.Writeoff TranAmount "
					+"FROM claimproc cp "
					+"WHERE cp.Status IN ("+statusIn+") "
					+"AND cp.WriteOff != 0 "
					+"AND cp.DateCP <= "+POut.Date(ageOptions.DateAsOf);
			}		
			else if(ageOptions.WriteoffOptions==PPOWriteoffDateCalc.ProcDate) {
				command="SELECT 'Writeoff' TranType,"
					+"cp.PatNum,"
					+"cp.ProcDate TranDate,"
					+"-cp.Writeoff TranAmount "
					+"FROM claimproc cp "
					+"WHERE cp.Status IN ("+statusIn+") "
					+"AND cp.WriteOff != 0 "
					+"AND cp.ProcDate <= "+POut.Date(ageOptions.DateAsOf);
			}	
			else { //AgingOptions.WriteoffAgingOptions.ClaimPayDate
				command="SELECT 'Writeoff' TranType,"
					+"cp.PatNum,"
					+"claimsnapshot.DateTEntry TranDate,"
					+"-claimsnapshot.Writeoff TranAmount "
					+"FROM claimproc cp "
					+"INNER JOIN claimsnapshot ON cp.ClaimProcNum=claimsnapshot.ClaimProcNum "
					+"WHERE cp.Status IN ("+statusIn+") "
					+"AND claimsnapshot.WriteOff != 0 "
					+"AND claimsnapshot.DateTEntry <= "+POut.Date(ageOptions.DateAsOf);
			}
			return command;
		}

		private static string GetWriteoffEstAgingQuery(AgingOptions ageOptions) {
			//This will add up claims that have been received on a date later than the run date.  This is to attempt to maintain historical information.
			//This logic matches a lot of our custom queries and can be removed if it becomes an issue, as it doesn't exist in our internal reports currently.
			string command = @"
				SELECT 'WriteoffEst' TranType,cp.PatNum,cp.ProcDate TranDate,
				-cp.WriteOff TranAmount 
				FROM claimproc cp
				WHERE cp.PatNum != 0
				AND ((cp.Status="+(int)ClaimProcStatus.NotReceived+@")
					OR (cp.Status="+(int)ClaimProcStatus.Received+@" AND cp.DateCP>" +POut.Date(ageOptions.DateAsOf)+@"))
				AND cp.ProcDate <= " +POut.Date(ageOptions.DateAsOf);
			return command;
		}

	}
	
	///<summary>Holds a single patient's aging information for a certain AsOfDate, given the options defined in AgingOptions.</summary>
	public class AgingPat {
		public Patient Pat;
		public double BalZeroThirty;
		public double BalThirtySixty;
		public double BalSixtyNinety;
		public double BalOverNinety;
		public double BalTotal;
	}

	[Serializable()]
	public class AgingOptions {
		public DateTime DateAsOf;
		public AgingInclude AgingInc;
		public PPOWriteoffDateCalc WriteoffOptions;
		public FamilyGrouping FamGroup;
		public AgeOfAccount AgeAccount;
		public NegativeBalAgingOptions NegativeBalOptions;
		public List<Provider> ListProvs;
		public List<Clinic> ListClins;
		public List<Def> ListBillTypes;
		public bool ExcludeInactive;
		public bool ExcludeArchive;
		public bool ExcludeBadAddress;
		public bool AgeCredits;

		[Flags]
		public enum AgingInclude {
			None = 0,
			[Description("Procedure Fees")]
			ProcedureFees = 1,
			Adjustments = 2,
			[Description("Pay Plan Charges")]
			PayPlanCharges = 4,
			[Description("Pay Plan Credits")]
			PayPlanCredits = 8,
			[Description("Patient Payments")]
			PatPayments = 16,
			[Description("Insurance Payments")]
			InsPayments = 32,
			[Description("Insurance Estimates")]
			InsEsts = 64,
			Writeoffs = 128,
			[Description("Writeoff Estimates")]
			WriteoffEsts = 256,
		}

		public enum FamilyGrouping {
			Family,
			Individual
		}

		public enum NegativeBalAgingOptions {
			Exclude,
			Include,
			ShowOnly
		}
	}
}
