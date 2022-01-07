using System;
using System.Collections;

namespace OpenDentBusiness {

	///<summary>Many-to-many relationship connecting Rx with DiseaseDef, AllergyDef, or Medication.  Only one of those links may be specified in a single row; the other two will be 0.</summary>
	[Serializable]
	public class RxAlert:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long RxAlertNum;
		///<summary>FK to rxdef.RxDefNum.  This alert is to be shown when user attempts to write an Rx for this RxDef.</summary>
		public long RxDefNum;
		///<summary>FK to diseasedef.DiseaseDefNum.  Only if DrugProblem interaction.  This is compared against disease.DiseaseDefNum using PatNum.  Drug-Problem (they call it Drug-Diagnosis) checking is also performed in NewCrop.</summary>
		public long DiseaseDefNum;
		///<summary>FK to allergydef.AllergyDefNum.  Only if DrugAllergy interaction.  Compared against allergy.AllergyDefNum using PatNum.  Drug-Allergy checking is also perfomed in NewCrop.</summary>
		public long AllergyDefNum;
		///<summary>FK to medication.MedicationNum.  Only if DrugDrug interaction.  This will be compared against medicationpat.MedicationNum using PatNum.  Drug-Drug checking is also performed in NewCrop.</summary>
		public long MedicationNum;
		///<summary>This is typically blank, so a default message will be displayed by OD.  But if this contains a message, then this message will be used instead.</summary>
		public string NotificationMsg;
		///<summary>False by default.  Set to true to flag the drug-drug or drug-allergy intervention as high significance.</summary>
		public bool IsHighSignificance;

		///<summary></summary>
		public RxAlert Copy() {
			return (RxAlert)this.MemberwiseClone();
		}

		
		
	}

		



		
	

	

	


}










