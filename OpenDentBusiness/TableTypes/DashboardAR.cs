using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>A table just used by the dashboard to store historical AR because it never changes and it takes too long (1 second for each of the 12 dates) to compute on the fly.  One entry per month going back at least 12 months.  This table gets automatically filled the first time that the dashboard is used.  The most recent month also gets added by using the dashboard.</summary>
	[Serializable]
	public class DashboardAR:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DashboardARNum;
		///<summary>This date will always be the last day of a month.</summary>
		public DateTime DateCalc;
		///<summary>Bal_0_30+Bal_31_60+Bal_61_90+BalOver90 for all patients.  This should also exactly equal BalTotal for all patients with positive amounts.  Negative BalTotals are credits, not A/R.</summary>
		public double BalTotal;
		///<summary>Sum of all InsEst for all patients for the month.</summary>
		public double InsEst;
		
		///<summary></summary>
		public DashboardAR Copy(){
			return (DashboardAR)this.MemberwiseClone();
		}

	
	}

	

	


}




















