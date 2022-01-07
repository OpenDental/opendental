using System;

namespace OpenDentBusiness{

	///<summary>Attaches a referral to a patient.</summary>
	[Serializable]
	public class RefAttach:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long RefAttachNum;
		///<summary>FK to referral.ReferralNum.</summary>
		public long ReferralNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Order to display in patient info. One-based.  Will be automated more in future.</summary>
		public int ItemOrder;
		///<summary>Date of referral.</summary>
		public DateTime RefDate;
		///<summary>Enum:ReferralType 0=RefTo,1=RefFrom,2=RefCustom.</summary>
		public ReferralType RefType;
		///<summary>Enum:ReferralToStatus 0=None,1=Declined,2=Scheduled,3=Consulted,4=InTreatment,5=Complete.</summary>
		public ReferralToStatus RefToStatus;
		///<summary>Why the patient was referred out, or less commonly, the circumstances of the referral source.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>Used to track ehr events.  All outgoing referrals default to true.  The incoming ones get a popup asking if it's a transition of care.</summary>
		public bool IsTransitionOfCare;
		///<summary>FK to procedurelog.ProcNum</summary>
		public long ProcNum;
		///<summary>.</summary>
		public DateTime DateProcComplete;
		///<summary>FK to provider.ProvNum.  Used when refering out a patient to track the reffering provider for EHR meaningfull use.</summary>
		public long ProvNum;
		///<summary>The datetime this referral attachment was last edited.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;

		///<summary>Returns a copy of this RefAttach.</summary>
		public RefAttach Copy(){
			return (RefAttach)this.MemberwiseClone();
		}


	}

	///<summary></summary>
	public enum ReferralType {
		///<summary></summary>
		RefTo,
		///<summary></summary>
		RefFrom,
		///<summary></summary>
		RefCustom
	}



}













