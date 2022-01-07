using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	///<summary>Always attached to a payment.  Always affects exactly one patient account and one provider.</summary>
	[Serializable]
	[CrudTable(IsSecurityStamped=true,IsSynchable=true,HasBatchWriteMethods=true)]
	public class PaySplit:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SplitNum;
		///<summary>Amount of split.</summary>
		public double SplitAmt;
		///<summary>FK to patient.PatNum.
		///Can be the PatNum of the guarantor if this is a split for a payment plan and the guarantor is in another family.</summary>
		public long PatNum;
		///<summary>DEPRECATED.  No longer used.  In older versions (before 7.0), this was the date that showed on the account.  Frequently the same as the date of the payment, but not necessarily.  Not when the payment was made.</summary>
		public DateTime ProcDate;
		///<summary>FK to payment.PayNum.  Every paysplit must be linked to a payment.</summary>
		public long PayNum;
		///<summary>No longer used.</summary>
		public bool IsDiscount;
		///<summary>No longer used</summary>
		public byte DiscountType;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>FK to payplan.PayPlanNum.  0 if not attached to a payplan.</summary>
		public long PayPlanNum;
		///<summary>Date always in perfect synch with Payment date.</summary>
		public DateTime DatePay;
		///<summary>FK to procedurelog.ProcNum.  0 if not attached to a procedure.</summary>
		public long ProcNum;
		///<summary>Date this paysplit was created.  User not allowed to edit.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime DateEntry;
		///<summary>FK to definition.DefNum.  Usually 0 unless this is a special unearned split.</summary>
		public long UnearnedType;
		///<summary>FK to clinic.ClinicNum.  Can be 0.  Need not match the ClinicNum of the Payment, because a payment can be split between clinics.</summary>
		public long ClinicNum;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		//No SecDateEntry, DateEntry already exists and is set by MySQL when the row is inserted and never updated
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>No longer used.</summary>
		public long FSplitNum;
		///<summary>FK to adjustment.AdjNum.  Can be 0.  Indicates that this paysplit is meant to counteract an Adjustment.</summary>
		public long AdjNum;
		///<summary>FK to payplancharge.PayPlanChargeNum. Can be 0. Indicates that this paysplit is meant to counteract a PayPlanCharge.</summary>
		public long PayPlanChargeNum;
		///<summary>Enum:PayPlanDebitTypes Explicitly specifies what this paysplit should be applied towards in regards to principal or interest.</summary>
		public PayPlanDebitTypes PayPlanDebitType;

		public PaySplit() {
			TagOD=Guid.NewGuid().ToString();//Used to identify PaySplits that have not been entered into the database yet.
		}

		///<summary>Returns a copy of this PaySplit.</summary>
		public PaySplit Copy(){
			return (PaySplit)this.MemberwiseClone();
		}

		public bool IsUnallocated {
			get {
				return (ProcNum==0
					&& AdjNum==0
					&& PayPlanNum==0
					&& PayPlanChargeNum==0
					&& UnearnedType==0);
			}
		}

		///<summary>Determines if this is the same paysplit based on SplitNum or TagOD.</summary>
		public bool IsSame(PaySplit otherSplit) {
			if(this.SplitNum==otherSplit.SplitNum && this.SplitNum!=0) {
				return true;
			}
			if(this.TagOD==otherSplit.TagOD) {
				return true;
			}
			return false;
		}
  }

  public enum SplitManagerPromptType {
    ///<summary>0</summary>
    [Description("Do Not Use")]
    DoNotUse,
    ///<summary>1</summary>
    [Description("Prompt")]
    Prompt,
    ///<summary>2</summary>
    [Description("Forced")]
    Force,
    ///<summary>3</summary>
    [Description("Procedure Forced")]
    ForceProc
  }

	///<summary>Payment plan debit charge types. Used to keep track of payment splits made towards principal or interest.</summary>
	public enum PayPlanDebitTypes {
		///<summary>0 - Legacy splits associated to payment plans did not specify what SplitAmt was applied towards and use this status.</summary>
		Unknown,
		///<summary>1 - Flags a split as a principal only payment.</summary>
		Principal,
		///<summary>2 - Flags a split as an interest only payment.</summary>
		Interest,
	}




}










