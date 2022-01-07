using System;
using System.Collections;

namespace OpenDentBusiness{
	
	///<summary>Simpler than a payment plan.  Does not affect running account balances.  Allows override of finance charges.  Affects the "pay now" on statements.  Only one installmentplan is allowed for a family, attached to guarantor only.  This is loosely enforced.</summary>
	[Serializable]
	public class InstallmentPlan:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InstallmentPlanNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Date payment plan agreement was made.</summary>
		public DateTime DateAgreement;
		///<summary>Date of first payment.</summary>
		public DateTime DateFirstPayment;
		///<summary>Amount of monthly payment.</summary>
		public double MonthlyPayment;
		///<summary>Annual Percentage Rate. e.g. 12.</summary>
		public float APR;
		///<summary>Note</summary>
		public string Note;


		///<summary>Returns a copy of this InstallmentPlan.</summary>
		public InstallmentPlan Copy(){
			return (InstallmentPlan)this.MemberwiseClone();
		}


	}


}