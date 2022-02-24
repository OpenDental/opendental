using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>An allergy attached to a patient and linked to an AllergyDef.</summary>
	[Serializable]
	public class Allergy:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AllergyNum;
		///<summary>FK to allergydef.AllergyDefNum</summary>
		public long AllergyDefNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>Adverse reaction description.</summary>
		public string Reaction;
		///<summary>True if still an active allergy.  False helps hide it from the list of active allergies.</summary>
		public bool StatusIsActive;
		///<summary>To be used for synch with web server for CertTimelyAccess.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>The historical date that the patient had the adverse reaction to this agent.</summary>
		public DateTime DateAdverseReaction;
		///<summary>Snomed code for reaction.  Optional and independent of the Reaction text field.  Not needed for reporting.  Only used for CCD export/import.</summary>
		public string SnomedReaction;

		///<summary></summary>
		public Allergy Copy() {
			return (Allergy)this.MemberwiseClone();
		}
	}
}