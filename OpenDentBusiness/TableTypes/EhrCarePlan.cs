using System;

namespace OpenDentBusiness{
	[Serializable]
	public class EhrCarePlan:TableBase {

		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrCarePlanNum;
		///<summary>FK to patient.PatNum. </summary>
		public long PatNum;
		///<summary>Snomed code describing the type of educational instruction provided.  Limited to terms descending from the Snomed 409073007 (Education Hierarchy).</summary>
		public string SnomedEducation;
		///<summary>Instructions provided to the patient.</summary>
		public string Instructions;
		///<summary>This field does not help much with care plan instructions, but will be more helpful for other types of care plans if we expand in the future (for example, planned procedures).  We also saw examples where this date was included in the human readable part of a CCD, but not in the machine readable part.</summary>
		public DateTime DatePlanned;

		///<summary></summary>
		public EhrCarePlan Clone() {
			return (EhrCarePlan)this.MemberwiseClone();
		}

	}
}
