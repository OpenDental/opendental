using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	/// <summary>Optional. Holds the Production Schedule for an OrhtoCase. </summary>
	[Serializable]
	public class OrthoSchedule:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoScheduleNum;
		///<summary>Override for banding date. </summary>
		public DateTime BandingDateOverride;
		///<summary>Override for debond date. </summary>
		public DateTime DebondDateOverride;
		///<summary>Amount to charge for banding procedure.</summary>
		public double BandingAmount;
		///<summary>Used every visit until the total off all visits+BandingAmount+DebondAmount=Fee of linked OrthoCase. </summary>
		public double VisitAmount;
		///<summary>Amount to charge for debond procedure.</summary>
		public double DebondAmount;
		///<summary>Is true if the ortho schedule is active. </summary>
		public bool IsActive;
		///<summary>DateTime the ortho schedule was last modified. Not editable by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;

		///<summary></summary>
		public OrthoSchedule Copy(){
			return (OrthoSchedule)this.MemberwiseClone();
		}

	}
}
