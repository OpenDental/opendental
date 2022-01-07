using System;

namespace OpenDentBusiness {
	///<summary>Records insurance plan and insurance payment data of a procedure that has been paid on primary or secondary insurance claim made to out of network plans. This data is used to estimate allowed fees and make more accurate insurance estimates.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class InsBlueBook:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InsBlueBookNum;
		///<summary>FK to procedurecode.CodeNum. The code of the procedure.</summary>
		public long ProcCodeNum;
		///<summary>FK to insplan.CarrierNum. The carrier that the insurance plan belongs to.</summary>
		public long CarrierNum;
		///<summary>FK to insplan.PlanNum. The insurance plan for which the claim was made.</summary>
		public long PlanNum;
		///<summary>The insplan.GroupNum. May be blank.</summary>
		public string GroupNum;
		///<summary>The sum of InsPayAmt per claim for received and supplemental claimprocs of the procedure that are associated to the insurance plan.</summary>
		public double InsPayAmt;
		///<summary>The AllowedOverride of the received claimproc on the claim.</summary>
		public double AllowedOverride;
		///<summary>The date and time of entry. Not editable by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTEntry;
		///<summary>FK to procedurelog.ProcNum.</summary>
		public long ProcNum;
		///<summary>The date of service, derived from claimproc.ProcDate of the received claimproc on the claim.</summary>
		public DateTime ProcDate;
		///<summary>The claim.ClaimType. Currently only gathering data for primary and secondary claims, so this will be "P"(Primary) or "S"(Secondary).</summary>
		public string ClaimType;
		///<summary>FK to claim.ClaimNum.</summary>
		public long ClaimNum;

		///<summary>Returns a copy of this InsBlueBook.</summary>
		public InsBlueBook Copy() {
			return (InsBlueBook)this.MemberwiseClone();
		}
	}
}