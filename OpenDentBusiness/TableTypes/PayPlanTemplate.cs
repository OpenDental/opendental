using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	///<summary>A template of payplan terms that can be copied to a payment plan.  Only used for dynamic payment plans, not patient payment plans.</summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class PayPlanTemplate:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long PayPlanTemplateNum;
		///<summary>The name of the Pay Plan Template.</summary>
		public string PayPlanTemplateName;
		///<summary>FK to clinic.ClinicNum. Can be 0.</summary>
		public long ClinicNum;
		///<summary>Annual percentage rate.  eg 18.</summary>
		public double APR;
		///<summary>The number of payments before interest is applied.</summary>
		public int InterestDelay;
		///<summary>The total payment amount due for each period.</summary>
		public double PayAmt;
		///<summary>The total number of periods for the payment plan. If the Pay Plan is dynamic and NumberOfPayments is not 0 then this is only used to calculate the PayAmt. After the PayAmt is calculated, NumberOfPayments is set to 0.</summary>
		public int NumberOfPayments;
		///<summary>Enum:PayPlanFrequency How often charges are created for the payment plan. Monthly, weekly, etc.  Only for Dynamic Payment Plans.</summary>
		public PayPlanFrequency ChargeFrequency;
		///<summary>The amount paid toward the payment plan when it was first opened.</summary>
		public double DownPayment;
		///<summary>Enum:DynamicPayPlanTPOptions Indicates the selected mode for how treatment planned procedures are handled by a dynamic payment plan.  None, AwaitComplete, or TreatAsComplete.</summary>
		public DynamicPayPlanTPOptions DynamicPayPlanTPOption;
		///<summary>A detailed note of the terms shows for future reference. Any changes made to the terms will be added to the note. Other notes can be added as needed.</summary>
		public string Note;
		///<summary>Templates can not be deleted, but can be hidden if not needed any more.</summary>
		public bool IsHidden;

		///<summary></summary>
		public PayPlanTemplate Copy(){
			return (PayPlanTemplate)this.MemberwiseClone();
		}
	}
}










