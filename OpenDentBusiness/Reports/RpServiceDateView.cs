using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class RpServiceDateView {
		public static DataTable GetData(long patNum,bool isFamily,bool isDetailed) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,isFamily,isDetailed);
			}
			int payPlanVersion=PIn.Int(PrefC.GetStringNoCache(PrefName.PayPlansVersion));
			if(payPlanVersion==0) {
				payPlanVersion=1;
			}
			string command=$@"
				/*top layer gets final columns for display*/
				SELECT
					IF(display.TranDate != '', '',display.ProcDate) AS 'Date',
					display.TranDate AS 'Trans Date',
					display.Patient,
					display.Reference,
					display.Charge,
					display.Credit,
					display.Pvdr,
					display.InsBal,
					display.AcctBal,
					display.Type
				FROM(
				/*cases out data from rows that we need, performing aggregate functions for day and overall totals.*/
				SELECT
					dup.Num,
					IF(dup.Num = 2, 0,core.ProcNum) AS ProcNum,
					(CASE dup.Num
						WHEN 1
							THEN IF(core.Type IN ('Proc','Adj-Att.','PatPay Att.','WriteOff-Att.','InsPay-Att.','PayPlan Charge Att.','PatPay Att. PayPlan','Unallocated'),1,2) -- get allocated items ordered before unallocated ones
						WHEN 2
							THEN IF(core.Type = 'Unallocated',1,2)
						WHEN 3
							THEN 4
						WHEN 4
							THEN 5
					END) AS OrderingNum,
					(CASE dup.Num
						WHEN 1
							THEN core.Type
						WHEN 2
							THEN 'Unallocated'
						WHEN 3
							THEN 'Day Total'
						WHEN 4
							THEN 'Overall Total'
					END) AS 'Type',
					(CASE dup.Num
						WHEN 1
							THEN core.ProcDate
						WHEN 2
							THEN core.ProcDate
						WHEN 3
							THEN core.ProcDate
						WHEN 4
							THEN NULL
					END) AS 'ProcDate',
					(CASE dup.Num
						WHEN 1
							THEN IF(core.Type = 'Proc', '',core.TranDate)
						WHEN 2
							THEN ''
						WHEN 3
							THEN ''
						WHEN 4
							THEN NULL
					END) AS 'TranDate',
					(CASE dup.Num
						WHEN 1
							THEN core.Patient
						WHEN 2
							THEN '' -- intentionaly blank
						WHEN 3
							THEN ''
						WHEN 4
							THEN '' -- intentionally blank
					END) AS 'Patient',
					(CASE dup.Num
						WHEN 1
							THEN IF(core.Type IN ('Adj-Att.','PatPay Att.','WriteOff-Att.','InsPay-Att.','PayPlan Charge Att.','PatPay Att. PayPlan'),CONCAT('  ',core.Reference),core.Reference)
						WHEN 2
							THEN 'Unallocated Trans'
						WHEN 3
							THEN CONCAT(REPEAT(' ',2),'Total for Date')
						WHEN 4
							THEN {(isFamily ? "'Family Total:'" : "'Patient Total:'" )}
					END) AS 'Reference',
					(CASE dup.Num
						WHEN 1
							THEN FORMAT(SUM(core.Charge),2)
						WHEN 2
							THEN '' -- intentionaly blank
						WHEN 3
							THEN FORMAT(SUM(core.Charge),2) -- total for date
						WHEN 4
							THEN FORMAT(SUM(core.Charge),2) -- overall total
					END) AS 'Charge',
					(CASE dup.Num
						WHEN 1
							THEN FORMAT(SUM(core.Credit),2)
						WHEN 2
							THEN '' -- intentionaly blank
						WHEN 3
							THEN FORMAT(SUM(core.Credit),2) -- total for date
						WHEN 4
							THEN FORMAT(SUM(core.Credit),2) -- overall total
					END) AS 'Credit',
					(CASE dup.Num
						WHEN 1
							THEN core.Prov
						WHEN 2
							THEN '' -- intentionaly blank
						WHEN 3
							THEN '' -- intentionally blank
						WHEN 4
							THEN '' -- intentionally blank
					END) AS 'Pvdr',
					(CASE dup.Num
						WHEN 1
							THEN {(isDetailed ? "IF(core.Type = 'Proc', FORMAT(core.InsCredits,2),'')": "''")} 
						WHEN 2
							THEN '' -- intentionally blank
						WHEN 3
							THEN FORMAT(SUM(IF(core.Type = 'Proc', core.InsCredits,0)),2) -- total for date
						WHEN 4
							THEN FORMAT(SUM(IF(core.Type = 'Proc', core.InsCredits,0)),2) -- overall total
					END) AS 'InsBal',
					(CASE dup.Num
						WHEN 1
							THEN {(isDetailed ? "IF(core.Type = 'Proc', FORMAT(core.Charge - (core.ProcCredits + core.InsCredits),2),'')" : "''")}
						WHEN 2
							THEN '' -- intentionally blank
						WHEN 3
							THEN FORMAT(SUM(core.Charge - core.Credit - core.InsCredits),2) -- total for date
						WHEN 4
							THEN FORMAT(SUM(core.Charge - core.Credit - core.InsCredits),2) -- overall total
					END) AS 'AcctBal'
				FROM(
				  {GetCoreQuery(patNum,isFamily,payPlanVersion)}
				) core
				INNER JOIN(
				/*add 2 extra rows to every transaction so we can use the data more than once for per day and overall totals without running the query again*/
					SELECT 1 AS Num
					UNION ALL
					SELECT 2
					UNION ALL
					SELECT 3
					UNION ALL
					SELECT 4
				) dup
					ON IF(dup.Num = 2 AND (core.Type LIKE '%Att.' || core.Type LIKE 'Proc'),FALSE,TRUE) -- only join on the 2 row when there are unallocated transactions on a day. 
				GROUP BY (CASE WHEN dup.Num = 1 -- per transaction
								THEN CONCAT(core.TranNum,'|',core.ProcDate,'|',core.Type) -- unique identifier for transaction rows
							WHEN dup.Num IN (2,3) -- per date of service, including an extra row per date for unallocated transactions identifier
								THEN CONCAT(dup.Num,'-',core.ProcDate) -- unique identifier for date of service row
							WHEN dup.Num = 4 -- ending total for family/patient
								THEN dup.Num -- only need to have one row here
						END)
				) display
				/*ensure final order is what we need it to be*/
				ORDER BY
					ISNULL(display.ProcDate),
					display.ProcDate,
					display.OrderingNum,
					display.Patient,
					display.ProcNum DESC,
					display.TranDate,
					FIELD(display.Type,'Proc','Adj-Att.','PatPay Att.','WriteOff-Att.','InsPay-Att.','PayPlan Charge Att.','PatPay Att. PayPlan','Unallocated','PatPay','WriteOff','Adj','InsPay','PayPlan Credit','Dynamic PayPlan Credit','PayPlan Charge','PatPay PayPlan','Day Total','Overall Total')
			";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetTable(command));
		}

		///<summary>Get core data ordered by procedure date and transactions attached to procs first, with specific ordering for transaction type. 
		///Using aging preference to get payment plan info. Defining transaction type separate from reference for specific ordering.</summary>
		private static string GetCoreQuery(long patNum,bool isFamily,int payPlanVersion) {
			//no remoting role check. Private method.
			string wherePatOrFam=isFamily ? $"patient.Guarantor={POut.Long(patNum)}":$"patient.PatNum={POut.Long(patNum)}";
			#region Procedures
			//Get all completed procedures for patient/family with charges and credits. Also includes separate column for insurance credits only.
			string command=$@"SELECT 'Proc' AS 'Type', 
				procedurelog.ProcNum AS 'TranNum', 
				procedurelog.ProcDate, 
				'0001-01-01' AS 'TranDate', 
				procedurelog.PatNum, 
				patient.FName AS Patient, 
				procedurelog.ProcNum, 
				CONCAT(procedurecode.ProcCode,':',COALESCE(NULLIF(CONCAT('#',procedurelog.ToothNum,'-'),'#-'),NULLIF(CONCAT(IF(LENGTH(procedurelog.ToothRange)>0,'Tth Rng',''),'-'),'-'),''),COALESCE(NULLIF(CONCAT(procedurelog.Surf,'-'),'-'),''),procedurecode.AbbrDesc) AS 'Reference', 
				procedurelog.ProcFee*(procedurelog.BaseUnits+procedurelog.UnitQty)- 
				COALESCE((SELECT SUM(claimproc.WriteOff) FROM claimproc WHERE claimproc.Status={POut.Int((int)ClaimProcStatus.CapComplete)} AND claimproc.ProcNum=procedurelog.ProcNum),0) AS 'Charge', 
				0 AS 'Credit', 
				0 AS 'InsPayEst', 
				(SELECT Abbr FROM provider WHERE provider.ProvNum=procedurelog.ProvNum) AS Prov, 
				(COALESCE((SELECT -SUM(AdjAmt) FROM adjustment WHERE adjustment.ProcNum=procedurelog.ProcNum),0)+ 
					COALESCE((SELECT SUM(SplitAmt) FROM paysplit WHERE paysplit.ProcNum=procedurelog.ProcNum),0)+ 
					COALESCE((SELECT SUM(WriteOff) + SUM(InsPayAmt) FROM claimproc 
						WHERE claimproc.ProcNum=procedurelog.ProcNum 
						AND claimproc.Status IN({POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapClaim)})),0) 
				) AS 'ProcCredits', 
				COALESCE((SELECT SUM(WriteOff) + SUM(InsPayEst) FROM claimproc WHERE claimproc.Status={POut.Int((int)ClaimProcStatus.NotReceived)} 
					AND claimproc.ProcNum=procedurelog.ProcNum 
				),0) AS 'InsCredits' 
				FROM procedurelog 
				INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum 
				INNER JOIN patient ON patient.PatNum=procedurelog.PatNum 
				WHERE procedurelog.ProcStatus={POut.Int((int)ProcStat.C)} 
				AND {wherePatOrFam} ";
			#endregion
			#region Adjustment
			//Get all adjustment for patient/family. Positive adjustment as Charges and negative adjustments as Credits. 
			command+=$@"
				UNION ALL
				SELECT IF(adjustment.ProcNum > 0,'Adj-Att.','Adj') AS 'Type',
				adjustment.AdjNum AS 'TranNum',
				IF(adjustment.ProcDate = '0001-01-01',adjustment.AdjDate,adjustment.ProcDate) AS ProcDate,
				adjustment.AdjDate AS 'TranDate',
				adjustment.PatNum,
				patient.FName AS Patient,
				adjustment.ProcNum,
				CONCAT('Adj- ',(SELECT ItemName FROM definition WHERE definition.DefNum=adjustment.AdjType)) AS 'Reference',
				IF(adjustment.AdjAmt > 0,adjustment.AdjAmt,0) AS 'Charge',
				IF(adjustment.AdjAmt <= 0,-adjustment.AdjAmt,0) AS 'Credit',
				0 AS 'InsPayEst',
				(SELECT Abbr FROM provider WHERE provider.ProvNum=adjustment.ProvNum) AS Prov,
				0 AS 'ProcCredits',
				0 AS 'InsCredits' 
				FROM adjustment 
				INNER JOIN patient ON patient.PatNum=adjustment.PatNum 
				WHERE {wherePatOrFam} ";
			#endregion
			#region Pat Payments
			//Get all payments, except where they should only show inside payplan
			command+=$@"
				UNION ALL 
				SELECT (CASE WHEN paysplit.ProcNum > 0 AND paysplit.PayPlanNum > 0 THEN 'PatPay Att. PayPlan' 
				WHEN paysplit.ProcNum > 0 THEN 'PatPay Att.' 
				WHEN paysplit.ProcNum = 0 AND paysplit.PayPlanNum > 0 
				THEN 'PatPay PayPlan' 
				ELSE 'PatPay' END) AS 'Type', 
				paysplit.SplitNum AS 'TranNum', 
				COALESCE((SELECT ProcDate FROM procedurelog WHERE procedurelog.ProcNum=paysplit.ProcNum), paysplit.DatePay) AS 'ProcDate', 
				paysplit.DatePay AS 'TranDate', 
				paysplit.PatNum, 
				patient.FName AS Patient, 
				paysplit.ProcNum, 
				CONCAT('PatPay- ',IF(paysplit.UnearnedType > 0,'PrePay ',''),COALESCE((SELECT definition.ItemName FROM payment INNER JOIN definition ON definition.DefNum=payment.PayType 
					WHERE payment.PayNum = paysplit.PayNum),'Inc Transfer')) AS 'Reference', 
				0 AS 'Charge', 
				paysplit.SplitAmt AS 'Credit', 
				0 AS 'InsPayEst', 
				(SELECT Abbr FROM provider WHERE provider.ProvNum = paysplit.ProvNum) AS Prov, 
				0 AS 'ProcCredits', 
				0 AS 'InsCredits' 
				FROM paysplit 
				INNER JOIN patient ON patient.PatNum=paysplit.PatNum 
				WHERE {(payPlanVersion==2 ? "TRUE": "paysplit.PayPlanNum=0")} 
				AND {wherePatOrFam} ";
			#endregion
			#region Ins Payments
			//Get all insurance payments
			command+=$@"
				UNION ALL 
				SELECT IF(claimproc.ProcNum > 0,'InsPay-Att.','InsPay') AS 'Type', 
				claimproc.ClaimProcNum AS 'TranNum', 
				IF(claimproc.ProcNum > 0,claimproc.ProcDate,claimproc.DateCP) AS 'ProcDate', 
				claimproc.DateCP AS 'TranDate', 
				claimproc.PatNum, 
				patient.FName AS Patient, 
				claimproc.ProcNum, 
				CONCAT('InsPay- ',COALESCE((SELECT ItemName FROM claimpayment INNER JOIN definition ON definition.DefNum=claimpayment.PayType 
					WHERE claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum),'Not Finalized')) AS 'Reference', 
				0 AS 'Charge', 
				claimproc.InsPayAmt AS 'Credit', 
				0 AS 'InsPayEst', 
				(SELECT Abbr FROM provider WHERE provider.ProvNum=claimproc.ProvNum) AS Prov, 
				0 AS 'ProcCredits', 
				0 AS 'InsCredits' 
				FROM claimproc 
				INNER JOIN patient ON patient.PatNum=claimproc.PatNum 
				WHERE claimproc.Status IN({POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)},{POut.Int((int)ClaimProcStatus.CapClaim)}) 
				AND claimproc.PayPlanNum=0 
				AND claimproc.InsPayAmt!=0 
				AND {wherePatOrFam} ";
			#endregion
			#region WriteOffs
			//Get all writeoffs, estimated and actual. Only will show estimated when claim is outstanding, otherwise shows actual with payment type of payment.
			command+=$@"
				UNION ALL
				SELECT IF(claimproc.ProcNum > 0,'WriteOff-Att.','WriteOff') AS 'Type', 
				CONCAT('W',claimproc.ClaimProcNum) AS 'TranNum', 
				IF(claimproc.ProcNum > 0,claimproc.ProcDate,claimproc.DateCP) AS 'ProcDate', 
				claimproc.DateCP AS 'TranDate', 
				claimproc.PatNum, 
				patient.FName AS Patient, 
				claimproc.ProcNum, 
				(CASE WHEN claimproc.Status={POut.Int((int)ClaimProcStatus.NotReceived)} AND claimproc.InsPayEst > 0  
					THEN CONCAT('Ins Pay Est:',claimproc.InsPayEst,IF(claimproc.WriteOff>0,CONCAT(' WriteOff:',claimproc.WriteOff),'')) 
					WHEN claimproc.Status={POut.Int((int)ClaimProcStatus.NotReceived)} AND claimproc.Writeoff > 0 AND claimproc.InsPayEst <= 0 
					THEN CONCAT('WriteOff Est:',claimproc.WriteOff) 
					ELSE CONCAT('W/O- ',COALESCE((SELECT ItemName FROM claimpayment INNER JOIN definition ON definition.DefNum=claimpayment.PayType 
						WHERE claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum),'')) END) AS 'Reference', 
				0 AS 'Charge', 
				IF(claimproc.Status IN({POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)}),claimproc.WriteOff,0) AS 'Credit', 
				IF(claimproc.Status = 0,claimproc.InsPayEst,0) AS 'InsPayEst', 
				(SELECT Abbr FROM provider WHERE provider.ProvNum = claimproc.ProvNum) AS Prov, 
				0 AS 'ProcCredits', 
				0 AS 'InsCredits' 
				FROM claimproc 
				INNER JOIN patient ON patient.PatNum=claimproc.PatNum 
				WHERE IF(claimproc.Status IN ({POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)}),ABS(claimproc.WriteOff) > 0.005,(ABS(claimproc.WriteOff) > 0.005 OR claimproc.InsPayEst > 0)) 
				AND claimproc.Status IN({POut.Int((int)ClaimProcStatus.NotReceived)},{POut.Int((int)ClaimProcStatus.Received)},{POut.Int((int)ClaimProcStatus.Supplemental)}) 
				AND {wherePatOrFam} ";
			#endregion
			#region PayPlan Version 1
			if(payPlanVersion==1) {
				//get only pay plan credits for pay plan versions 1
				command+=$@"
					UNION ALL 
					SELECT 'PayPlan Credit' AS 'Type', 
					payplan.PayPlanNum AS 'TranNum', 
					payplan.PayPlanDate AS 'ProcDate', 
					payplan.PayPlanDate AS 'TranDate', 
					payplan.PatNum, 
					patient.FName AS Patient, 
					0 AS 'ProcNum', 
					'PayPlan Credit' AS 'Reference', 
					0 AS 'Charge', 
					payplan.CompletedAmt AS 'Credit', 
					0 AS 'InsPayEst', 
					(SELECT Abbr FROM provider WHERE provider.ProvNum = payplancharge.ProvNum) AS Prov, 
					0 AS 'ProcCredits', 
					0 AS 'InsCredits' 
					FROM payplan 
					LEFT JOIN payplancharge ON payplancharge.PayPlanNum=payplan.PayPlanNum AND payplancharge.ChargeType=0 
					INNER JOIN patient ON patient.PatNum=payplan.PatNum 
					WHERE {wherePatOrFam} 
					GROUP BY payplan.PayPlanNum ";
			}
			#endregion
			#region PayPlan Version 2
			else if(payPlanVersion==2) {
				//get all pay plan credits and debits for pay plan version 2
				command+=$@"
					UNION ALL
					SELECT (CASE WHEN payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Debit)} AND payplancharge.ProcNum > 0 THEN 'PayPlan Charge Att.' 
					WHEN payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Debit)} AND payplancharge.ProcNum=0 THEN 'PayPlan Charge' 
					WHEN payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Credit)} THEN 'PayPlan Credit' END) AS 'Type', 
					payplancharge.PayPlanChargeNum AS 'TranNum', 
					payplancharge.ChargeDate AS 'ProcDate', 
					payplancharge.ChargeDate AS 'TranDate', 
					payplancharge.PatNum, 
					patient.FName AS Patient, 
					payplancharge.ProcNum, 
					(CASE WHEN payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Debit)} AND payplancharge.ProcNum > 0 THEN 'PayPlan Charge Att.' 
						WHEN payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Debit)} AND payplancharge.ProcNum=0 THEN 'PayPlan Charge' 
						WHEN payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Credit)} THEN 'PayPlan Credit'END) AS 'Reference', 
					IF(payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Debit)},payplancharge.Principal+payplancharge.Interest,0) AS 'Charge', 
					IF(payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Credit)},payplancharge.Principal,0) AS 'Credit', 
					0 AS 'InsPayEst', 
					(SELECT Abbr FROM provider WHERE provider.ProvNum=payplancharge.ProvNum) AS Prov, 
					0 AS 'ProcCredits', 
					0 AS 'InsCredits' 
					FROM payplancharge 
					LEFT JOIN payplan ON payplan.PayplanNum=payplancharge.PayPlanNum 
					INNER JOIN patient ON patient.PatNum=payplancharge.PatNum 
					WHERE payplancharge.ChargeDate <= CURDATE() 
					AND IF(payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Debit)},payplan.PlanNum=0,TRUE) 
					AND {wherePatOrFam} ";
			}
			#endregion
			#region PayPlan Version 3
			else if(payPlanVersion==3) {
				//get only pay plan credits for pay plan versions 3
				command+=$@"
					UNION ALL
					SELECT 'PayPlan Credit' AS 'Type', 
					payplancharge.PayPlanChargeNum AS 'TranNum', 
					payplancharge.ChargeDate AS 'ProcDate', 
					payplancharge.ChargeDate AS 'TranDate', 
					payplancharge.PatNum, 
					patient.FName AS Patient, 
					payplancharge.ProcNum, 
					'PayPlan Credit' AS 'Reference', 
					0 AS 'Charge', 
					IF(payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Credit)},payplancharge.Principal,0) AS 'Credit', 
					0 AS 'InsPayEst', 
					(SELECT Abbr FROM provider WHERE provider.ProvNum=payplancharge.ProvNum) AS Prov, 
					0 AS 'ProcCredits', 
					0 AS 'InsCredits' 
					FROM payplancharge 
					INNER JOIN patient ON patient.PatNum=payplancharge.PatNum 
					WHERE payplancharge.ChargeType={POut.Int((int)PayPlanChargeType.Credit)} 
					AND {wherePatOrFam} ";
			}
			#endregion
			else { 
				//Version 4 will not show payplan information
			}
			#region Dynamic Payment Plans (Credits) 
			if(payPlanVersion==2 || payPlanVersion==3) {//PayPlanVersions.AgeCreditsAndDebits or PayPlanVersions.AgeCreditsOnly
				//Dynamic payment plan charges will have been picked up by their cooresponding version query above. However, credits for dynamic payment plans
				//are not made from paymentplancharges when using dynamic payment plans,they are their own table. We need to handle the credits here 
				//according to what the user wants to see based on their payment plans version. 
				command+=$@"
					UNION ALL
					SELECT 'Dynamic PayPlan Credit' AS 'Type', 
					payplan.PayPlanNum AS 'TranNum', 
					payplanlink.SecDateTEntry AS 'ProcDate', 
					payplanlink.SecDateTEntry AS 'TranDate', 
					payplan.PatNum, 
					patient.FName AS Patient, 
					0 AS 'ProcNum', 
					'PayPlan Credit' AS 'Reference', 
					0 AS 'Charge', 
					(CASE WHEN ABS(payplanlink.AmountOverride)>0.005 THEN payplanlink.AmountOverride ELSE prodlink.Fee END) AS 'Credit', 
					0 AS 'InsPayEst', 
					(SELECT Abbr FROM provider WHERE provider.ProvNum = prodlink.ProvNum) AS Prov, 
					0 AS 'ProcCredits', 
					0 AS 'InsCredits' 
					FROM payplanlink
					INNER JOIN payplan ON payplan.PayPlanNum = payplanlink.PayPlanNum 
					INNER JOIN patient ON patient.PatNum=payplan.PatNum 
					LEFT JOIN (
						SELECT procedurelog.PatNum,
								(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)
								+COALESCE(procAdj.AdjAmt,0)+COALESCE(procClaimProc.InsPay,0)
								+COALESCE(procClaimProc.WriteOff,0)
								+COALESCE(procSplit.SplitAmt,0)
								) Fee,
								procedurelog.ProcNum Num,payplanlink.LinkType,payplanlink.PayPlanLinkNum,procedurelog.ClinicNum,procedurelog.ProvNum 
						FROM payplanlink 
						INNER JOIN procedurelog ON procedurelog.ProcNum=payplanlink.FKey AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Procedure)} 
						LEFT JOIN (
							SELECT SUM(adjustment.AdjAmt) AdjAmt,adjustment.ProcNum
							FROM adjustment
							WHERE adjustment.PatNum IN ({patNum})
							GROUP BY adjustment.ProcNum
						)procAdj ON procAdj.ProcNum=procedurelog.ProcNum
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
							AND claimproc.PatNum IN ({patNum})
							GROUP BY claimproc.ProcNum
						)procClaimProc ON procClaimProc.ProcNum=procedurelog.ProcNum 
						LEFT JOIN (
							SELECT SUM(paysplit.SplitAmt)*-1 SplitAmt,paysplit.ProcNum
							FROM paysplit
							WHERE paysplit.PayPlanNum=0
							AND paysplit.PatNum IN ({patNum})
							GROUP BY paysplit.ProcNum
						)procSplit ON procSplit.ProcNum=procedurelog.ProcNum
						UNION ALL
						SELECT adjustment.PatNum,adjustment.AdjAmt + COALESCE(adjSplit.SplitAmt,0) Fee,adjustment.AdjNum Num,payplanlink.LinkType
							,payplanlink.PayPlanLinkNum,adjustment.ClinicNum,adjustment.ProvNum
							FROM payplanlink 
							INNER JOIN adjustment ON adjustment.AdjNum=payplanlink.FKey 
								AND payplanlink.LinkType={POut.Int((int)PayPlanLinkType.Adjustment)}
								AND adjustment.ProcNum=0
							LEFT JOIN (
								SELECT SUM(COALESCE(paysplit.SplitAmt,0))*-1 SplitAmt,paysplit.AdjNum
								FROM paysplit
								WHERE paysplit.PayPlanNum=0
								AND paysplit.PatNum IN ({patNum})
								GROUP BY paysplit.AdjNum
							)adjSplit ON adjSplit.AdjNum=adjustment.AdjNum
					) prodlink ON prodlink.PayPlanLinkNum=payplanlink.PayPlanLinkNum 
					WHERE (payplan.Guarantor IN ({patNum}) OR payplan.PatNum IN ({patNum})) ";
				}
			#endregion
			//Order the core data
			command+=@"
				ORDER BY ProcDate,ProcNum DESC, PatNum, TranDate,
				FIELD('Type','Proc','Adj-Att.','PatPay Att.','WriteOff-Att.','InsPay-Att.','PayPlan Charge Att.','PatPay Att. PayPlan','Unallocated','PatPay',
				'WriteOff','Adj','InsPay','PayPlan Credit','Dynamic PayPlan Credit','PayPlan Charge','PatPay PayPlan') ";
			return command;
		}
	}
}
