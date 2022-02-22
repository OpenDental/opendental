using System;

namespace OpenDentBusiness {
	///<summary>This table stores intermediate family aged balances just prior to updating the patient table.  Once the aging calculations are finished
	///and the patient table is updated, this table is truncated.  At the start of the aging calculations this table is checked and if there are existing
	///rows, we will notify the user and force them to decide whether an aging calculation has already begun or an error happened that prevented the
	///calculations from finishing and the rows are left over and can be deleted.</summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class FamAging:TableBase {
		///<summary>FK to patient.PatNum.  Also the primary key for this table.  Always the PatNum for the Guarantor of a family.  A guarantor may not
		///exist in this table if the family does not have a balance.  i.e. If a PatNum is not in this table, the aged balance columns on the patient table
		///are set to 0, so either the patient is not the guarantor or the family has a zero balance.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatNum;
		///<summary>Aged balance from 0 to 30 days old. Aging numbers are for entire family.  Only stored with guarantor.</summary>
		public double Bal_0_30;
		///<summary>Aged balance from 31 to 60 days old. Aging numbers are for entire family.  Only stored with guarantor.</summary>
		public double Bal_31_60;
		///<summary>Aged balance from 61 to 90 days old. Aging numbers are for entire family.  Only stored with guarantor.</summary>
		public double Bal_61_90;
		///<summary>Aged balance over 90 days old. Aging numbers are for entire family.  Only stored with guarantor.</summary>
		public double BalOver90;
		///<summary>Insurance Estimate for entire family.  Only stored with guarantor.</summary>
		public double InsEst;
		///<summary>Total balance for entire family before insurance estimate.  Not the same as the sum of the 4 aging balances because this can be 
		///negative.  Only stored with guarantor.</summary>
		public double BalTotal;
		///<summary>Amount "due now" for all payment plans such that someone in this family is the payment plan guarantor.  
		///This is the total of all payment plan charges past due (taking into account the PayPlansBillInAdvanceDays setting) subtract the amount 
		///already paid for the payment plans.  Only stored with family guarantor.</summary>
		public double PayPlanDue;

	}

}