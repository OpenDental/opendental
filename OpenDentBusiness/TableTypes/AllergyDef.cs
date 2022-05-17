using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>An allergy definition.  Gets linked to an allergy and patient.  Allergies will not show in CCD messages unless they have a valid Medication (that has an RxNorm) or UniiCode.</summary>
	[Serializable]
	public class AllergyDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AllergyDefNum;
		///<summary>Name of the drug.  User can change this.  If an RxCui is present, the RxNorm string can be pulled from the in-memory table for UI display in addition to the Description.</summary>
		public string Description;
		///<summary>Because user can't delete.</summary>
		public bool IsHidden;
		///<summary>The last date and time this row was altered.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Enum:SnomedAllergy SNOMED Allergy Type Code.  Only used to create CCD in FormSummaryOfCare.</summary>
		public SnomedAllergy SnomedType;
		///<summary>FK to medication.MedicationNum.  Optional, only used with CCD messages.</summary>
		public long MedicationNum;
		///<summary>The Unii code for the Allergen.  Optional, but there must be either a MedicationNum or a UniiCode.  Used to create CCD in FormSummaryOfCare, or set during CCD allergy reconcile.</summary>
		public string UniiCode;
		

		///<summary></summary>
		public AllergyDef Copy() {
			return (AllergyDef)this.MemberwiseClone();
		}

		
	}

	///<summary></summary>
	public enum SnomedAllergy{
		///<summary>0-No SNOMED allergy type code has been assigned.</summary>
		None,
		///<summary>1-Allergy to substance (disorder), code number 418038007.</summary>
		AllergyToSubstance,
		///<summary>2-Drug allergy (disorder), code number 416098002.</summary>
		DrugAllergy,
		///<summary>3-Drug intolerance (disorder), code number 59037007.</summary>
		DrugIntolerance,
		///<summary>4-Food allergy (disorder), code number 414285001.</summary>
		FoodAllergy,
		///<summary>5-Food intolerance (disorder), code number 235719002.</summary>
		FoodIntolerance,
		///<summary>6-Propensity to adverse reactions (disorder), code number 420134006.</summary>
		AdverseReactions,
		///<summary>7-Propensity to adverse reactions to drug (disorder), code number 419511003</summary>
		AdverseReactionsToDrug,
		///<summary>8-Propensity to adverse reactions to food (disorder), code number 418471000.</summary>
		AdverseReactionsToFood,
		///<summary>9-Propensity to adverse reactions to substance (disorder), code number 419199007.</summary>
		AdverseReactionsToSubstance
	}
}