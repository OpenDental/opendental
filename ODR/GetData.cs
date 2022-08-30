using System;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;

namespace ODR{
	///<summary></summary>
	public class GetData{
		///<summary></summary>
		[Obsolete]
		public static string Pref(string prefName) {
			string command="SELECT ValueString FROM preference WHERE PrefName='"+POut.String(prefName)+"'";
			DataConnection dcon=new DataConnection();
			DataTable table=dcon.GetTable(command);
			return table.Rows[0][0].ToString();
		}

		///<Summary>asOfDate is typically 12/31/...  </Summary>
		public static double NetIncomeThisYear(object asOfDateObj) {
			DateTime asOfDate;
			if(asOfDateObj.GetType()==typeof(string)){
				asOfDate=PIn.Date(asOfDateObj.ToString());
			}
			else if(asOfDateObj.GetType()==typeof(DateTime)){
				asOfDate=(DateTime)asOfDateObj;
			}
			else{
				return 0;
			}
			DateTime firstOfYear=new DateTime(asOfDate.Year,1,1);
			string command="SELECT SUM(ROUND(CreditAmt,3)), SUM(ROUND(DebitAmt,3)), AcctType "
			+"FROM journalentry,account "
			+"WHERE journalentry.AccountNum=account.AccountNum "
			+"AND DateDisplayed >= "+POut.Date(firstOfYear)
			+" AND DateDisplayed <= "+POut.Date(asOfDate)
			+" GROUP BY AcctType";
			DataConnection dcon=new DataConnection();
			DataTable table=dcon.GetTable(command);
			double retVal=0;
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i][2].ToString()=="3"//income
					|| table.Rows[i][2].ToString()=="4")//expense
				{
					retVal+=PIn.Double(table.Rows[i][0].ToString());//add credit
					retVal-=PIn.Double(table.Rows[i][1].ToString());//subtract debit
					//if it's an expense, we are subtracting (income-expense), but the signs cancel.
				}
			}
			return retVal;
		}

		///<Summary>Gets sum of all income-expenses for all previous years. asOfDate could be any date</Summary>
		public static double RetainedEarningsAuto(object asOfDateObj) {
			DateTime asOfDate;
			if(asOfDateObj.GetType()==typeof(string)) {
				asOfDate=PIn.Date(asOfDateObj.ToString());
			}
			else if(asOfDateObj.GetType()==typeof(DateTime)) {
				asOfDate=(DateTime)asOfDateObj;
			}
			else {
				return 0;
			}
			DateTime firstOfYear=new DateTime(asOfDate.Year,1,1);
			string command="SELECT SUM(ROUND(CreditAmt,3)), SUM(ROUND(DebitAmt,3)), AcctType "
			+"FROM journalentry,account "
			+"WHERE journalentry.AccountNum=account.AccountNum "
			+"AND DateDisplayed < "+POut.Date(firstOfYear)
			+" GROUP BY AcctType";
			DataConnection dcon=new DataConnection();
			DataTable table=dcon.GetTable(command);
			double retVal=0;
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i][2].ToString()=="3"//income
					|| table.Rows[i][2].ToString()=="4")//expense
				{
					retVal+=PIn.Double(table.Rows[i][0].ToString());//add credit
					retVal-=PIn.Double(table.Rows[i][1].ToString());//subtract debit
					//if it's an expense, we are subtracting (income-expense), but the signs cancel.
				}
			}
			return retVal;
		}


	}

	

}
