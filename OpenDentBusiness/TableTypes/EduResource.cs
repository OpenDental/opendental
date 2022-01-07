using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>EHR education resource.  Only one of the 3 FK fields will be used at a time (DiseaseDefNum, MedicationNum, or LabResultID).  The other two will be blank.   Displays a clickable URL if the patient meets certain criteria.  </summary>
	[Serializable]
	public class EduResource:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EduResourceNum;
		///<summary>FK to diseasedef.DiseaseDefNum.  This now also handles ICD9s and Snomeds via the entry in DiseaseDef.</summary>
		public long DiseaseDefNum;
		///<summary>FK to medication.MedicationNum. </summary>
		public long MedicationNum;
		///<summary>FK to labresult.TestID. </summary>
		public string LabResultID;
		///<summary>Used for display in the grid.</summary>
		public string LabResultName;
		///<summary>String, example &lt;43. Must start with &lt; or &gt; followed by int.  Only used if FK LabResultID is used.</summary>
		public string LabResultCompare;
		///<summary>.</summary>
		public string ResourceUrl;
		///<summary>FK to ehrmeasureevent.CodeValueResult when ehrmeasureevent.EventType=EhrMeasureEventType.TobaccoUseAssessed (8).</summary>
		public string SmokingSnoMed;
		/////<summary>FK to icd9.ICD9Num.</summary>//this is now obtained by pointing to a DiseaseDef which has an ICD9
		//public long Icd9Num;

		///<summary></summary>
		public EduResource Copy() {
			return (EduResource)this.MemberwiseClone();
		}

	}
}