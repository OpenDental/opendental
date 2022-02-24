using System;
using System.Collections;

namespace OpenDentBusiness {

	///<summary>Used to identify the source of payment for a given patient at a given point in time.  As insurance is added and removed, rows should be either automatically inserted into this table, or the user should be prompted to specify what the new payor type is.  The DateStart of one payor type is interpreted as the end date of the previous payor type.  Example: Patient with no insurance may have payortype.SopCode=81 ("SelfPay").  Patient then adds Medicaid insurance and gets a second new PayorType entry with SopCode=2 (Medicaid).</summary>
	[Serializable]
	public class PayorType:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PayorTypeNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Date of the beginning of new payor type.  End date is the DateStart of the next payor type entry.</summary>
		public DateTime DateStart;
		///<summary>FK to sop.SopCode. Examples: 121, 3115, etc. </summary>
		public string SopCode;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;

		///<summary>Returns a copy of this PayorType.</summary>
		public PayorType Clone() {
			return (PayorType)this.MemberwiseClone();
		}

	}
}