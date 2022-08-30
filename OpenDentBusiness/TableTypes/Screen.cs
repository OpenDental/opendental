using System;

namespace OpenDentBusiness{

	///<summary>Used in public health.  This screening table is meant to be general purpose.  It is compliant with the popular Basic Screening Survey.  It is also designed with minimal foreign keys and can be easily adapted to a tablet PC.  This table can be used with only the screengroup table, but is more efficient if provider, school, and county tables are also available.</summary>
	[Serializable]
	public class Screen:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long ScreenNum;
		///<summary>Enum:PatientGender </summary>
		public PatientGender Gender;
		///<summary>Enum:PatientRaceOld and ethnicity.</summary>
		public PatientRaceOld RaceOld;
		///<summary>Enum:PatientGrade </summary>
		public PatientGrade GradeLevel;
		///<summary>Age of patient at the time the screening was done. Faster than recording birthdates.</summary>
		public byte Age;
		///<summary>Enum:TreatmentUrgency</summary>
		public TreatmentUrgency Urgency;
		///<summary>Enum:YN Set to true if patient has cavities.</summary>
		public YN HasCaries;
		///<summary>Enum:YN Set to true if patient needs sealants.</summary>
		public YN NeedsSealants;
		///<summary>Enum:YN</summary>
		public YN CariesExperience;
		///<summary>Enum:YN</summary>
		public YN EarlyChildCaries;
		///<summary>Enum:YN</summary>
		public YN ExistingSealants;
		///<summary>Enum:YN</summary>
		public YN MissingAllTeeth;
		///<summary>Optional</summary>
		public DateTime Birthdate;
		///<summary>FK to screengroup.ScreenGroupNum.</summary>
		public long ScreenGroupNum;
		///<summary>The order of this item within its group.</summary>
		public int ScreenGroupOrder;
		///<summary>.</summary>
		public string Comments;
		///<summary>FK to screenpat.ScreenPatNum.</summary>
		public long ScreenPatNum;
		///<summary>FK to sheet.SheetNum</summary>
		public long SheetNum;

		///<summary>Returns a copy of this Screen.</summary>
		public Screen Copy() {
			return (Screen)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	[Flags]
	public enum ScreenChartType {
		///<summary></summary>
		TP=1,
		///<summary></summary>
		C=2
	}

	

}













