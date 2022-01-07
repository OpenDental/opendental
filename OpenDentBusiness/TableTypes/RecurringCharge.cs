using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>This table holds a record of recurring charges that have been attempted.</summary>
	[Serializable]
	public class RecurringCharge:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long RecurringChargeNum;
		///<summary>FK to patient.PatNum. The patient this recurring charge is for.</summary>
		public long PatNum;
		///<summary>FK to patient.ClinicNum. The clinic this recurring charge is for.</summary>
		public long ClinicNum;
		///<summary>The date time of the charge. </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeCharge;
		///<summary>Enum:RecurringChargeStatus </summary>
		public RecurringChargeStatus ChargeStatus;
		///<summary>The family balance at the time this charge was created.</summary>
		public double FamBal;
		///<summary>The pay plan due at the time this charge was created.</summary>
		public double PayPlanDue;
		///<summary>The sum of the FamBal and PayPlanDue at the time this charge was created.</summary>
		public double TotalDue;
		///<summary>The recurring charge amount from the credit card at the time this charge was created.</summary>
		public double RepeatAmt;
		///<summary>The amount that was charged (or will be charged if the status is NotYetCharged).</summary>
		public double ChargeAmt;
		///<summary>FK to userod.UserNum. The user that processed this charge. Will be 0 if this was done automatically.</summary>
		public long UserNum;
		///<summary>FK to payment.PayNum. The payment created from this charge.</summary>
		public long PayNum;
		///<summary>FK to creditcard.CreditCardNum. The credit card that caused this charge.</summary>
		public long CreditCardNum;
		///<summary>Any error message from processing this charge.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ErrorMsg;

		///<summary></summary>
		public RecurringCharge Copy() {
			return (RecurringCharge)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum RecurringChargeStatus {
		///<summary>0 - The charge has not been attempted yet.</summary>
		[Description("Pending")]
		NotYetCharged,
		///<summary>1 - The charge was successful.</summary>
		[Description("Successful")]
		ChargeSuccessful,
		///<summary>2 - Processing the charge failed.</summary>
		[Description("Failed")]
		ChargeFailed,
		///<summary>3 - Processing the charge failed and was specifically declined.</summary>
		[Description("Declined")]
		ChargeDeclined,
	}
}