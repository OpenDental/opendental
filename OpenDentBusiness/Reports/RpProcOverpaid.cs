using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	public class RpProcOverpaid {
		public static DataTable GetOverPaidProcs(long patNum,List<long> listProvNums,List<long> listClinics,DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,listProvNums,listClinics,dateStart,dateEnd);
			}
			List<long> listHiddenUnearnedDefNums=ReportsComplex.RunFuncOnReportServer(() => 
				Defs.GetDefsNoCache(DefCat.PaySplitUnearnedType).FindAll(x => !string.IsNullOrEmpty(x.ItemValue)).Select(x => x.DefNum).ToList()
			);
			#region Completed Procs
			string command="SELECT ";
			if(PrefC.GetBool(PrefName.ReportsShowPatNum)) {
				command+=DbHelper.Concat("CAST(patient.PatNum AS CHAR)","'-'","patient.LName","', '","patient.FName","' '","patient.MiddleI");
			}
			else {
				command+=DbHelper.Concat("patient.LName","', '","patient.FName","' '","patient.MiddleI");
			}
			command+=@" AS 'patientName', 
				procedurelog.ProcDate,
				procedurecode.ProcCode,
				procedurelog.ToothNum,
				provider.Abbr,
				(procedurelog.ProcFee*(procedurelog.UnitQty+procedurelog.BaseUnits)) AS fee,
				patient.PatNum,
				procedurelog.ProcNum
				FROM procedurelog
				INNER JOIN patient ON patient.PatNum=procedurelog.PatNum
				INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum
				INNER JOIN provider ON provider.ProvNum=procedurelog.ProvNum
				WHERE procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" AND "
				+DbHelper.BetweenDates("procedurelog.ProcDate",dateStart,dateEnd)+" "
				+"AND procedurelog.ProcFee>0 ";
			if(listProvNums!=null && listProvNums.Count > 0) {
				command+="AND procedurelog.ProvNum IN ("+string.Join(",",listProvNums.Select(x => POut.Long(x)))+") ";
			}
			if(listClinics!=null && listClinics.Count > 0) {
				command+="AND procedurelog.ClinicNum IN ("+string.Join(",",listClinics.Select(x => POut.Long(x)))+") ";
			}
			if(patNum>0) {
				command+="AND procedurelog.PatNum="+POut.Long(patNum)+" ";
			}
			command+="ORDER BY procedurelog.ProcDate,patientName,procedurecode.ProcCode,provider.Abbr";
			DataTable rawCompletedProcTable=Db.GetTable(command);
			Dictionary<long,DataRow> dictCompletedProcRows=rawCompletedProcTable.Select().ToDictionary(x => PIn.Long(x["ProcNum"].ToString()));
			#endregion
			DataTable table=new DataTable();
			if(dictCompletedProcRows.Count==0) {
				return table;
			}
			#region ClaimProcs
			List<long> listPatNums=rawCompletedProcTable.Select().Select(x => PIn.Long(x["PatNum"].ToString())).Distinct().ToList();
			command=@"SELECT MIN(claimproc.ProcNum) ProcNum,MIN(claimproc.PatNum) PatNum,MIN(claimproc.ProcDate) ProcDate,SUM(claimproc.InsPayAmt) insPayAmt,
				SUM(claimproc.Writeoff) writeoff
				FROM claimproc
				WHERE claimproc.Status NOT IN("+string.Join(",",new List<int>{ (int)ClaimProcStatus.Preauth,
				(int)ClaimProcStatus.CapEstimate,(int)ClaimProcStatus.CapComplete,(int)ClaimProcStatus.Estimate,(int)ClaimProcStatus.InsHist }
					.Select(x => POut.Int(x)))+") "
				+"AND "+DbHelper.BetweenDates("claimproc.ProcDate",dateStart,dateEnd)+" "
				+"AND claimproc.PatNum IN("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+") "
				+@"GROUP BY claimproc.ProcNum
				HAVING SUM(claimproc.InsPayAmt+claimproc.Writeoff)>0
				ORDER BY NULL";
			Dictionary<long,DataRow> dictClaimProcRows=Db.GetTable(command).Select().ToDictionary(x => PIn.Long(x["ProcNum"].ToString()));
			#endregion
			#region Patient Payments
			command=@"SELECT paysplit.ProcNum,SUM(paysplit.SplitAmt) ptAmt
				FROM paysplit
				WHERE paysplit.ProcNum>0
				AND paysplit.PatNum IN("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+$@") ";
			if(listHiddenUnearnedDefNums.Count>0) {
				command+=$"AND paysplit.UnearnedType NOT IN ({string.Join(",",listHiddenUnearnedDefNums)}) ";
			}
			command+=@"
				GROUP BY paysplit.ProcNum
				ORDER BY NULL";
			Dictionary<long,DataRow> dictPatPayRows=Db.GetTable(command).Select().ToDictionary(x => PIn.Long(x["ProcNum"].ToString()));
			#endregion
			#region Adjustments
			command=@"SELECT adjustment.ProcNum,SUM(adjustment.AdjAmt) AdjAmt
				FROM adjustment
				WHERE adjustment.ProcNum>0
				AND adjustment.PatNum IN("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+@")
				GROUP BY adjustment.ProcNum
				ORDER BY NULL";
			Dictionary<long,DataRow> dictAdjRows=Db.GetTable(command).Select().ToDictionary(x => PIn.Long(x["ProcNum"].ToString()));
			#endregion
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("patientName");
			table.Columns.Add("ProcDate",typeof(DateTime));
			table.Columns.Add("ProcCode");
			table.Columns.Add("ToothNum");
			table.Columns.Add("Abbr");
			table.Columns.Add("fee");
			table.Columns.Add("insPaid");
			table.Columns.Add("wo");
			table.Columns.Add("ptPaid");
			table.Columns.Add("adjAmt");
			table.Columns.Add("overPay");
			table.Columns.Add("PatNum");
			DataRow row;
			foreach(KeyValuePair<long,DataRow> kvp in dictCompletedProcRows) {
				long procNum=kvp.Key;
				decimal procFeeAmt=PIn.Decimal(kvp.Value["fee"].ToString());
				decimal insPaidAmt=0;
				decimal woAmt=0;
				decimal ptPaidAmt=0;
				decimal adjAmt=0;
				if(dictClaimProcRows.ContainsKey(procNum)) {
					insPaidAmt=PIn.Decimal(dictClaimProcRows[procNum]["insPayAmt"].ToString());
					woAmt=PIn.Decimal(dictClaimProcRows[procNum]["writeoff"].ToString());
				}
				if(dictPatPayRows.ContainsKey(procNum)) {
					ptPaidAmt=PIn.Decimal(dictPatPayRows[procNum]["ptAmt"].ToString());
				}
				if(dictAdjRows.ContainsKey(procNum)) {
					adjAmt=PIn.Decimal(dictAdjRows[procNum]["AdjAmt"].ToString());
				}
				decimal overPay=procFeeAmt-insPaidAmt-woAmt-ptPaidAmt+adjAmt;
				if(!CompareDecimal.IsLessThanZero(overPay)) {
					continue;//No overpayment. Not need to continue;
				}
				row=table.NewRow();
				row["patientName"]=PIn.String(kvp.Value["patientName"].ToString());
				row["ProcDate"]=PIn.Date(kvp.Value["ProcDate"].ToString());
				row["ProcCode"]=PIn.String(kvp.Value["ProcCode"].ToString());
				row["ToothNum"]=PIn.String(kvp.Value["ToothNum"].ToString());
				row["Abbr"]=PIn.String(kvp.Value["Abbr"].ToString()); ;
				row["fee"]=procFeeAmt.ToString();
				row["insPaid"]=insPaidAmt.ToString();
				row["wo"]=woAmt.ToString();
				row["ptPaid"]=ptPaidAmt.ToString();
				row["adjAmt"]=adjAmt.ToString();
				row["overPay"]=overPay.ToString();
				row["PatNum"]=PIn.Long(kvp.Value["PatNum"].ToString());
				table.Rows.Add(row);
			}
			return table;
		}
	}
}
