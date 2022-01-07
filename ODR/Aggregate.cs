using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace ODR{
	///<summary></summary>
	public class Aggregate{
		private static decimal runningSum;
		private static string groupByVal;

		///<summary></summary>
		public static string RunningSum(string groupBy,string addValue){
			decimal num=PIn.Decimal(addValue);
			if(groupByVal==null || groupBy!=groupByVal){//if new or changed group
				runningSum=0;
			}
			groupByVal=groupBy;
			runningSum+=num;
			return runningSum.ToString("F");
		}

		///<summary></summary>
		public static string RunningSumForAccounts(object groupBy, object debitAmt, object creditAmt, object acctType) {
			if(debitAmt==null || creditAmt==null){
				return 0.ToString("N");
			}
			try {
				//Cannot read debitAmt and creditAmt as decimals because it makes the general ledger detail report fail.  Simply cast as decimals when doing mathematical operations.
				double debit=(double)debitAmt;//PIn.PDouble(debitAmt);
				double credit=(double)creditAmt;//PIn.PDouble(creditAmt)
				if(groupByVal==null || groupBy.ToString()!=groupByVal) {//if new or changed group
					runningSum=0;
				}
				groupByVal=groupBy.ToString();
				if(TestValue.AccountDebitIsPos(acctType.ToString())) {
					runningSum+=(decimal)debit-(decimal)credit;
				}
				else {
					runningSum+=(decimal)credit-(decimal)debit;
				}
				return runningSum.ToString("N");
			}
			catch {
				return 0.ToString("N");
			}
		}

	}

	

}
