using System;

namespace OpenDentBusiness {
	///<summary>An intervention ordered or performed.  Examples: smoking cessation and weightloss counseling.  Links to a definition in the ehrcode table using the CodeValue and CodeSystem.</summary>
	[Serializable]
	public class Intervention:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InterventionNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to provider.ProvNum. </summary>
		public long ProvNum;
		///<summary>FK to ehrcode.CodeValue.  This code may not exist in the ehrcode table, it may have been chosen from a bigger list of available codes.  In that case, this will be a FK to a specific code system table identified by the CodeSystem column.  The code for this item from one of the code systems supported.  Examples: V65.3 or 418995006.</summary>
		public string CodeValue;
		///<summary>FK to codesystem.CodeSystemName. The code system name for this code.  Possible values are: CPT, HCPCS, ICD9CM, ICD10CM, and SNOMEDCT.</summary>
		public string CodeSystem;
		///<summary>User-entered details about the intervention for this patient.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>The date of the intervention.</summary>
		public DateTime DateEntry;
		///<summary>Enum:InterventionCodeSet AboveNormalWeight, BelowNormalWeight, TobaccoCessation, Nutrition, PhysicalActivity, Dialysis.</summary>
		public InterventionCodeSet CodeSet;
		///<summary>Indicates whether the intervention was offered/recommended to the patient and the patient declined the treatment/referral.</summary>
		public bool IsPatDeclined;

		///<summary></summary>
		public Intervention Copy() {
			return (Intervention)MemberwiseClone();
		}

	}
	
	///<summary>Value sets for interventions.  This will limit the codes in FormInterventionEdit to a smaller list of codes and allow us to recommend codes to meet specific CQMs.</summary>
	public enum InterventionCodeSet {
		///<summary>0 - Above Normal Weight Follow-up/Referrals where weight assessment may occur</summary>
		AboveNormalWeight,
		///<summary>1 - Below Normal Weight Follow-up/Referrals where weight assessment may occur</summary>
		BelowNormalWeight,
		///<summary>2 - Counseling for Nutrition</summary>
		Nutrition,
		///<summary>3 - Counseling for Physical Activity</summary>
		PhysicalActivity,
		///<summary>4 - Tobacco Use Cessation Counseling</summary>
		TobaccoCessation,
		///<summary>5 - Dialysis Education/Other Services Related to Dialysis</summary>
		Dialysis
	}
}
