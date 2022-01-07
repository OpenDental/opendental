using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	/// <summary>Each row represents one signed agreement to make payments. </summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class PayPlan:TableBase {
		/// <summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long PayPlanNum;
		/// <summary>FK to patient.PatNum.  The patient who had the treatment done.</summary>
		public long PatNum;
		///<summary>FK to patient.PatNum.  The person responsible for the payments.  Does not need to be in the same family as the patient.  
		///Not necessarily the same as the guarantor on the PayPlanCharge.</summary>
		public long Guarantor;
		/// <summary>Date that the payment plan will display in the account.</summary>
		public DateTime PayPlanDate;
		/// <summary>Annual percentage rate.  eg 18.  This does not take into consideration any late payments, but only the percentage used to calculate the amortization schedule.</summary>
		public double APR;
		///<summary>Generally used to archive the terms when the amortization schedule is created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>FK to insplan.PlanNum.  Will be 0 if standard payment plan.  But if this is being used to track expected insurance payments, then this will be the foreign key to insplan.PlanNum, and Guarantor will be 0.</summary>
		public long PlanNum;
		///<summary>The amount of the treatment that has already been completed.  This should match the sum of the principal amounts for most situations.  But if the procedures have not yet been completed, and the payment plan is to make any sense, then this number must be changed.</summary>
		public double CompletedAmt;
		///<summary>FK to inssub.InsSubNum.  Will be 0 if standard payment plan.  But if this is being used to track expected insurance payments, then this will be the foreign key to inssub.InsSubNum, and Guarantor will be 0.</summary>
		public long InsSubNum;
		///<summary>Enum:PaymentSchedule How often payments are scheduled to be made.</summary>
		public PaymentSchedule PaySchedule;
		///<summary>The number of payments that will be made to complete the payment plan.</summary>
		public int NumberOfPayments;
		///<summary>Payment amount due per payment plan charge.</summary>
		public double PayAmt;
		///<summary>The amount paid toward the payment plan when it was first opened.</summary>
		public double DownPayment;
		///<summary>True if this payment plan is closed.  Closed should not be edited.</summary>
		public bool IsClosed;
		///<summary>The encrypted and bound signature in base64 format.  The signature is bound to the concatenation of the Total Amount,APR,Number of Payments,Payment Amount </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Signature;
		///<summary>True if the signature is in Topaz format rather than OD format.</summary>
		public bool SigIsTopaz;
		///<summary>FK to definition.DefNum</summary>
		public long PlanCategory;
		///<summary>True if this payment plan is a dynamic payment plan, false if it is static.</summary>
		public bool IsDynamic;
		///<summary>Enum:PayPlanFrequency How often charges are created for the payment plan.</summary>
		public PayPlanFrequency ChargeFrequency;
		///<summary>The date of the first payment plan charge. Does not include downpayment.</summary>
		public DateTime DatePayPlanStart;
		///<summary>True if the payment plan is locked. Locked payment plans cannot add production or modify terms. </summary>
		public bool IsLocked;
		///<summary>The date on which the pay plan can begin posting interest charges.</summary>
		public DateTime DateInterestStart;
		///<summary>Enum:DynamicPayPlanTPOptions Indicates the selected mode for how treatment planned procedures are handled by a dynamic payment plan.</summary>
		public DynamicPayPlanTPOptions DynamicPayPlanTPOption;

		///<summary></summary>
		public PayPlan Copy(){
			return (PayPlan)this.MemberwiseClone();
		}

	}

	///<summary></summary>
	public enum PaymentSchedule {
		///<summary>0 - Pay 1 time every month.</summary>
		Monthly,
		///<summary>1 - Pay 1 time every month on a certain day of the week.</summary>
		MonthlyDayOfWeek,
		///<summary>2 - Pay every week per month.</summary>
		Weekly,
		///<summary>3 - Pay every other week per times per month.</summary>
		BiWeekly,
		///<summary>4 - Pay 4 times per year.</summary>
		Quarterly
	}

	///<summary></summary>
	public enum PayPlanVersions {
		///<summary>1</summary>
		[Description("Do Not Age (legacy)")]
		DoNotAge=1,
		///<summary>2</summary>
		[Description("Age Credits and Debits (default)")]
		AgeCreditsAndDebits,
		///<summary>3</summary>
		[Description("Age Credits Only")]
		AgeCreditsOnly,
		///<summary>4</summary>
		[Description("No Charges to Account (rarely used)")]
		NoCharges,
	}

	///<summary></summary>
	public enum PayPlanFrequency {
		///<summary>0 - Weekly </summary>
		[Description("Weekly")]
		Weekly,
		///<summary>1 - Every Other Week</summary>
		[Description("Every Other Week")]
		EveryOtherWeek,
		///<summary>2 - Specific day of month </summary>
		[Description("Specific Day of Month")]
		OrdinalWeekday,
		///<summary>3 - Monthly</summary>
		[Description("Monthly")]
		Monthly,
		///<summary>4 - Quarterly</summary>
		[Description("Quarterly")]
		Quarterly,
	}

	///<summary>Bit wise</summary>
	[Flags]
	public enum PayPlanModes {
		///<summary>0</summary>
		[Description("None")]
		None=0,
		///<summary>1</summary>
		[Description("Patient")]
		Patient=1,
		///<summary>2</summary>
		[Description("Insurance")]
		Insurance=2,
		///<summary>4</summary>
		[Description("Dynamic")]
		Dynamic=4,
	}

	///<summary>Enum that changes how treatment planned procedures are handled in dynamic payment plans.</summary>
	public enum DynamicPayPlanTPOptions {
		///<summary>0</summary>
		[Description("None")]
		None,
		///<summary>1</summary>
		[Description("Optional mode for Dynamic Payment Plans that sets Pay Plan Charges to 0 until the attached planned procedure is complete.")]
		AwaitComplete,
		///<summary>2</summary>
		[Description("Optional mode for Dynamic Payment Plans that treats Treatment Planned Procedures as if they were complete.")]
		TreatAsComplete,
	}

}










