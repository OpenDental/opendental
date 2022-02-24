using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Mobile {

	/// <summary>A list of diseases that can be assigned to patients.</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class AllergyDefm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long AllergyDefNum;
		///<summary></summary>
		public string Description;
		///<summary>Enum:SnomedAllergy SNOMED Allergy Type Code.</summary>
		public SnomedAllergy Snomed;
		///<summary>FK to Medication.MedicationNum. Optional.</summary>
		public long MedicationNum;

		///<summary></summary>
		public AllergyDefm Copy() {
			return (AllergyDefm)this.MemberwiseClone();
		}
	}
}
