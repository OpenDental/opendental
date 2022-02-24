using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>For EHR, this lets us record medical problems for family members.  These family members will usually not be in our database, and they are just recorded by relationship.</summary>
	[Serializable]
	public class FamilyHealth:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long FamilyHealthNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Enum:FamilyRelationship </summary>
		public FamilyRelationship Relationship;
		///<summary>FK to diseasedef.DiseaseDefNum, which will have a SnoMed associated with it.</summary>
		public long DiseaseDefNum;
		///<summary>Name of the family member.</summary>
		public string PersonName;

		///<summary></summary>
		public FamilyHealth Clone() {
			return (FamilyHealth)this.MemberwiseClone();
		}

	}

	///<summary></summary>
	public enum FamilyRelationship {
		///<summary>0</summary>
		Parent,
		///<summary>1</summary>
		Sibling,
		///<summary>2</summary>
		Offspring,
	}

}