using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Ehr</summary>
	[Serializable]
	public class ReminderRule:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ReminderRuleNum;
		///<summary>Enum:EhrCriterion Problem,Medication,Allergy,Age,Gender,LabResult.</summary>
		public EhrCriterion ReminderCriterion;
		///<summary>Foreign key to disease.DiseaseDefNum, medicationpat.MedicationNum, or allergy.AllergyDefNum. Will be 0 if Age, Gender, or LabResult are the trigger.</summary>
		public long CriterionFK;
		///<summary>Only used if Age, Gender, or LabResult are the trigger. Examples: "&lt;25"(must include &lt; or &gt;), "Male"/"Female", "INR" (the simple description of the lab test)</summary>
		public string CriterionValue;
		///<summary>Text that will show as the reminder.</summary>
		public string Message;

		///<summary></summary>
		public ReminderRule Clone() {
			return (ReminderRule)this.MemberwiseClone();
		}

		

	}

	///<summary>EhrCriterion: Problem,Medication,Allergy,Age,Gender,LabResult</summary>
	public enum EhrCriterion {
		///<summary>0-DiseaseDef.  Shows as 'problem' because it needs to be human readable.</summary>
		Problem,
		///<summary>1-Medication</summary>
		Medication,
		///<summary>2-AllergyDef</summary>
		Allergy,
		///<summary>3-Age</summary>
		Age,
		///<summary>4-Gender</summary>
		Gender,
		///<summary>5-LabResult</summary>
		LabResult,
		/////<summary>6-ICD9</summary>//now handled by Problem(DiseaseDef)
		//ICD9
	}
}