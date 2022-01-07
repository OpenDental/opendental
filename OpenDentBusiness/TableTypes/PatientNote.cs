using System;
using System.ComponentModel;

namespace OpenDentBusiness{

	///<summary>Essentially more columns in the patient table.  They are stored here because these fields can contain a lot of information, and we want to try to keep the size of the patient table a bit smaller.</summary>
	[Serializable]
	public class PatientNote:TableBase {
		///<summary>FK to patient.PatNum.  Also the primary key for this table. Always one to one relationship with patient table.  A new patient might not have an entry here until needed.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatNum;
		///<summary>Only one note per family stored with guarantor.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate | CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string FamFinancial;
		///<summary>No longer used.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ApptPhone;
		///<summary>Medical Summary</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string Medical;
		///<summary>Service notes</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string Service;
		///<summary>Complete current Medical History</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string MedicalComp;
		///<summary>Shows in the Chart module just below the graphical tooth chart.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string Treatment;
		///<summary>In Case of Emergency Name.</summary>
		public string ICEName;
		///<summary>In Case of Emergency Phone.</summary>
		public string ICEPhone;
		///<summary>-1 by default. Overrides the default number of months for an ortho treatment for this patient.
		///Gets automatically set to the current value found in the pref OrthoClaimMonthsTreatment when the first placement procedure has been completed and this value is -1.
		///This column is an integer instead of a byte because it needs to store -1 so that users can override with the value of 0.
		///When set to -1 the default practice value for the pref OrthoClaimMonthsTreatment is used.</summary>
		public int OrthoMonthsTreatOverride=-1;
		///<summary>Overrides the date of the first ortho procedure for this patient to use for ortho case patients. 
		///If MinDate, then the date is derived by looking at the first ortho procedure for this patient.</summary>
		public DateTime DateOrthoPlacementOverride;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>Enum:PatConsentFlags None=0,ShareMedicationHistoryErx=1, Indicates if the patient consents for DoseSpot to access their medication
		///history, bitwise.</summary>
		public PatConsentFlags Consent;

		public PatientNote Copy() {
			return (PatientNote)this.MemberwiseClone();
		}
	}

	[Flags]
	public enum PatConsentFlags {
		///<summary>0 - None</summary>
		None=0,
		///<summary>1 - Patient consents for eRx to access their medication history</summary>
		[Description("Shared Medication History Erx")]
		ShareMedicationHistoryErx=1
	}
}










