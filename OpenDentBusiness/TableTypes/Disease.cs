using System;
using System.Collections;

namespace OpenDentBusiness {

	/// <summary>Each row is one disease that one patient has.  Now called a problem in the UI.  Must have a DiseaseDefNum.</summary>
	[Serializable]
	public class Disease:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DiseaseNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>FK to diseasedef.DiseaseDefNum.  The disease description is in that table.</summary>
		public long DiseaseDefNum;
		///<summary>Any note about this disease that is specific to this patient.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PatNote;
		///<summary>The last date and time this row was altered.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		//This column was removed 
		/////<summary>FK to icd9.ICD9Num.  Will be zero if DiseaseDefNum has a value.</summary>
		//public long ICD9Num;
		///<summary>Enum:ProblemStatus Active=0, Resolved=1, Inactive=2.</summary>
		public ProblemStatus ProbStatus;
		///<summary>Date that the disease was diagnosed.  Can be minval if unknown.</summary>
		public DateTime DateStart;
		///<summary>Date that the disease was set resolved or inactive.  Will be minval if still active.  ProbStatus should be used to determine if it is active or not.</summary>
		public DateTime DateStop;
		///<summary>FK to snomed.SnomedCode.  Used in EHR CCD export/import only.  Must be one of the following SNOMED codes:
		///Problem/Concern (55607006 or blank), Finding (404684003), Complaint (409586006), Dignosis (282291009), Condition (64572001), FunctionalLimitation (248536006), Symptom (418799008).</summary>
		public string SnomedProblemType;
		///<summary>Enum:FunctionalStatus  Used to export EHR CCD functional status and/or cognitive status information only.</summary>
		public FunctionalStatus FunctionStatus;

		///<summary></summary>
		public Disease Copy() {
			return (Disease)this.MemberwiseClone();
		}

		
		
		
		
	}

	///<summary>Used in EHR to export patient functional and cognitive statuses on CCD documents.</summary>
	public enum FunctionalStatus {
		///<summary>0 - Default value.  If not using EHR, then each diseasedef will use this value.</summary>
		Problem,
		///<summary>1 - This clinical statement contains details of an evaluation or assessment of a patient’s cognitive status. The evaluation may include assessment of a patient's mood, memory, and ability to make decisions. The statement will include, if present, supporting caregivers, non-medical devices, and the time period for which the evaluation and assessment were performed.</summary>
		CognitiveResult,
		///<summary>2 - A cognitive status problem observation is a clinical statement that describes a patient's cognitive condition, findings or symptoms. Examples of cognitive problem observations are inability to recall, amnesia, dementia, and aggressive behavior. A cognitive problem observation is a finding or medical condition. This is different from a cognitive result observation, which is a response to a question that provides insight to the patient's cognitive status. It reflects findings that provide information about a medical condition, while a result observation reflects responses to questions in a cognitive test or those that provide information about a person's judgement, comprehension ability, and response speed.</summary>
		CognitiveProblem,
		///<summary>3 - This clinical statement represents details of an evaluation or assessment of a patient’s functional status. The evaluation may include assessment of a patient's  language, vision, hearing, activities of daily living, behavior, general function, mobility and self-care status. The statement will include, if present, supporting caregivers, non-medical devices, and the time period for which the evaluation and assessment were performed.</summary>
		FunctionalResult,
		///<summary>4 - A functional status problem observation is a clinical statement that represents a patient’s functional perfomance and ability.</summary>
		FunctionalProblem,
	}



		
	

	

	


}










