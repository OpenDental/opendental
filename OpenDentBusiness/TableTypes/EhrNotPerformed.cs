using System;

namespace OpenDentBusiness {
	///<summary>For EHR module, these are all the items 'not performed' on patients.  Each row will link to the ehrcode table to retrieve relevant data.  To join this table to the ehrcode table you must join on CodeValue and CodeSystem.  Some items will have associated reasons attached to specify why it was not performed.  Those reasons will also be defined in the ehrcode table, so it may be necessary to join with that table again for the data relevant to the reason.</summary>
	[Serializable]
	public class EhrNotPerformed:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrNotPerformedNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>This will be a FK to a specific code system table identified by the CodeSystem column.  The code for this item from one of the code systems supported.  Examples: 90656 or 442333005.</summary>
		public string CodeValue;
		///<summary>FK to codesystem.CodeSystemName. The code system name for this code.  Possible values are: CPT, CVX, LOINC, SNOMEDCT.</summary>
		public string CodeSystem;
		///<summary>FK to ehrcode.CodeValue.  This code may not exist in the ehrcode table, it may have been chosen from a bigger list of available codes.  In that case, this will be a FK to a specific code system table identified by the CodeSystem column.  The code for the reason the item was not performed from one of the code systems supported.  Examples: 182856006 or 419808006.</summary>
		public string CodeValueReason;
		///<summary>FK to codesystem.CodeSystemName. The code system name for this code.  Possible value is: SNOMEDCT.</summary>
		public string CodeSystemReason;
		///<summary>Relevant notes for this not performed item.  Just in case users want it, does not get reported in EHR quality measure reporting.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>The date and time this item was created.  Can be edited to the date and time the item actually occurred.</summary>
		public DateTime DateEntry;

		///<summary></summary>
		public EhrNotPerformed Copy() {
			return (EhrNotPerformed)MemberwiseClone();
		}

	}

	public enum EhrNotPerformedItem {
		///<summary>0 - BMI Exam, physical exam</summary>
		BMIExam,
		///<summary>1 - Influenza Vaccination, procedure or medication</summary>
		InfluenzaVaccination,
		///<summary>2 - Tobacco Use Screening, risk category/assessment</summary>
		TobaccoScreening,
		///<summary>3 - Current Medications Documented, procedure</summary>
		DocumentCurrentMeds
	}

}
