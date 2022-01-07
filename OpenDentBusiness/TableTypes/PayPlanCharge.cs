using System;
using System.Collections;
using System.Xml.Serialization;
using Newtonsoft.Json;
using CodeBase;

namespace OpenDentBusiness{

	///<summary>One of the dated charges attached to a payment plan.  This has nothing to do with payments, but rather just causes the amount due to increase on the date of the charge.  The amount of the charge is the sum of the principal and the interest.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true,HasBatchWriteMethods=true)]
	public class PayPlanCharge:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PayPlanChargeNum;
		///<summary>FK to payplan.PayPlanNum.</summary>
		public long PayPlanNum;
		///<summary>FK to patient.PatNum.  The guarantor account that each charge will affect.  Does not have to match the guarantor of the payment plan.
		///This column doesn't even have to point to a guarantor at all.  
		///E.g. Credits and Closeout debits will be linked to the patient, not guarantor.</summary>
		public long Guarantor;
		///<summary>FK to patient.PatNum.  The patient account that the principal gets removed from.</summary>
		public long PatNum;
		///<summary>The date that the charge will show on the patient account.  Any charge with a future date will not show on the account yet and will not affect the balance.</summary>
		public DateTime ChargeDate;
		///<summary>For Debits, this is the principal charge amount.  For Credits (version 2 only), then this is the credit amount.</summary>
		public double Principal;
		///<summary>For Debits, this is the interest portion of this payment.  Always 0 for Credits.</summary>
		public double Interest;
		///<summary>Any note about this particular payment plan charge</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>FK to provider.ProvNum.  Since there is no ProvNum field at the payplan level, the provider must be the same for all payplancharges.  
		///It's initially assigned as the patient priProv.  Payments applied should be to this provnum, 
		///although the current user interface does not help with this.</summary>
		public long ProvNum;
		///<summary>FK to clinic.ClinicNum.  Since there is no ClincNum field at the payplan level, the clinic must be the same for all payplancharges.  It's initially assigned using the patient clinic.  Payments applied should be to this clinic, although the current user interface does not help with this.</summary>
		public long ClinicNum;
		///<summary>Enum:PayPlanChargeType The charge type of the payment plan. 0 - Debit, 1 - Credit.  Only relevant for those on Payment Plan Version 2.</summary>
		public PayPlanChargeType ChargeType;
		///<summary>FK to procedurelog.ProcNum.  The procedure that this payplancharge is attached to.  Only applies to credits.
		///Always 0 for debits.  Can be 0 for credits not attached to a procedure.</summary>
		public long ProcNum;
		///<summary>DateTime payplancharge was added to the payplan. Not editable by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>DateTime payplancharge was edited. Not editable by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>FK to statement.StatementNum.  Only used when the statement in an invoice.</summary>
		public long StatementNum;
		///<summary>Only present for dynamic payment plans. Contains FKey of the link type.</summary>
		public long FKey;
		///<summary>Enum:PayPlanLinkType Only present for dynamic payment plans. </summary>
		public PayPlanLinkType LinkType;
		///<summary>Set to true if this charge is created to offset an overcharge</summary>
		public bool IsOffset;

		///<summary>True if this payplan charge is a credit adjustment. A credit adjustment will reduce the amount of the payment plan. However, can be true if a positive or negative dynamic payment plan adjustment.</summary>
		[XmlIgnore,JsonIgnore]
		public bool IsCreditAdjustment {
			get {
				//Any patient payment plan credit that is negative and not attached to anything is considered an adjustment.
				//Any dynamic payment plan credit with a LinkType of Adjustment is obviously an adjustment as well.
				if((ChargeType==PayPlanChargeType.Credit && LinkType==PayPlanLinkType.None && ProcNum==0 && CompareDouble.IsLessThanZero(Principal))
					|| (ChargeType==PayPlanChargeType.Credit && LinkType==PayPlanLinkType.Adjustment))
				{
					return true;
				}
				return false;
			}
		}

		///<summary>True if this payplan charge is a debit adjustment. A debit adjustment will maintain the amount of the payment plan but reduce the amount due. It is as if the patient paid this amount toward the payment plan. However, can be true if a positive or negative dynamic payment plan adjustment.</summary>
		[XmlIgnore,JsonIgnore]
		public bool IsDebitAdjustment {
			get {
				//Any patient payment plan debit that is negative and not attached to anything is considered an adjustment.
				//Any dynamic payment plan debit with a LinkType of Adjustment is obviously an adjustment as well.
				if((ChargeType==PayPlanChargeType.Debit && LinkType==PayPlanLinkType.None && CompareDouble.IsLessThanZero(Principal))
					|| (ChargeType==PayPlanChargeType.Debit && LinkType==PayPlanLinkType.Adjustment))
				{
					return true;
				}
				return false;
			}
		}

		///<summary></summary>
		public PayPlanCharge Copy(){
			return (PayPlanCharge)this.MemberwiseClone();
		}


	}
	public enum PayPlanChargeType {
		///<summary>0 - Debit</summary>
		Debit,
		///<summary>1 - Credit</summary>
		Credit
	}
}





















