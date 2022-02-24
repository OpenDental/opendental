using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>A vaccine given to a patient on a date.</summary>
	[Serializable]
	public class VaccinePat:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long VaccinePatNum;
		///<summary>FK to vaccinedef.VaccineDefNum.  Can be 0 if and only if CompletionStatus=NotAdministered, in which case CVX code is assumed to be 998 (not administered) and there is no manufacturer.</summary>
		public long VaccineDefNum;
		///<summary>The datetime that the vaccine was administered.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStart;
		///<summary>Typically set to the same as DateTimeStart.  User can change.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEnd;
		///<summary>Size of the dose of the vaccine.  0 indicates unknown and gets converted to 999 on HL7 output.</summary>
		public float AdministeredAmt;
		///<summary>FK to drugunit.DrugUnitNum. Unit of measurement of the AdministeredAmt.  0 represents null.  When going out in HL7 RXA-7, the units must be valid UCUM or the export will be blocked.
		///Sometime in the future, we may want to convert this column to a string and name it "UcumCode".  For now left alone for backwards compatibility.</summary>
		public long DrugUnitNum;
		///<summary>Optional.  Used in HL7 RXA-9.1.</summary>
		public string LotNumber;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Documentation sometimes required.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>The city where the vaccine was filled.  This can be different than the practice office city for historical vaccine information.  Exported in HL7 ORC-3.</summary>
		public string FilledCity;
		///<summary>The state where the vaccine was filled.  This can be different than the practice office state for historical vaccine infromation.  Exported in HL7 ORC-3.</summary>
		public string FilledST;
		///<summary>Enum:VaccineCompletionStatus Exported in HL7 RXA-20.  Corresponds to HL7 table 0322 (guide page 225).</summary>
		public VaccineCompletionStatus CompletionStatus;
		///<summary>Enum:VaccineAdministrationNote Exported in HL7 RXA-9.  Corresponds to CDC code set NIP001 (http://hl7v2-iz-testing.nist.gov/mu-immunization/).</summary>
		public VaccineAdministrationNote AdministrationNoteCode;
		///<summary>FK to userod.UserNum.  The user that the vaccine was entered by.  May be 0 for vaccines added before this column was created.  Exported in HL7 ORD-10.</summary>
		public long UserNum;
		///<summary>FK to provider.ProvNum.  The provider who ordered the vaccine.  Exported in HL7 ORD-12.</summary>
		public long ProvNumOrdering;
		///<summary>FK to provider.ProvNum.  The provider who administered the vaccine.  Exported in HL7 RXA-10.</summary>
		public long ProvNumAdminister;
		///<summary>The date that the vaccine expires.  Exported in HL7 RXA-16.</summary>
		public DateTime DateExpire;
		///<summary>Enum:VaccineRefusalReason Exported in HL7 RXA-18.  Corresponds to CDC code set NIP002 (http://hl7v2-iz-testing.nist.gov/mu-immunization/).</summary>
		public VaccineRefusalReason RefusalReason;
		///<summary>Enum:VaccineAction Exported in HL7 RXA-21.  Corresponds to HL7 table 0323 (guide page 225).</summary>
		public VaccineAction ActionCode;
		///<summary>Enum:VaccineAdministrationRoute Exported in HL7 RXR-1.  Corresponds to HL7 table 0162 (guide page 200).</summary>
		public VaccineAdministrationRoute AdministrationRoute;
		///<summary>Enum:VaccineAdministrationSite Exported in HL7 RXR-2.  Corresponds to HL7 table 0163 (guide page 201).</summary>
		public VaccineAdministrationSite AdministrationSite;

		///<summary></summary>
		public VaccinePat Copy() {
			return (VaccinePat)this.MemberwiseClone();
		}

	}

	///<summary>Exported in HL7 RXA-20.  Corresponds to HL7 table 0322 (guide page 225).</summary>
	public enum VaccineCompletionStatus {
		///<summary>0 - Code CP.  Default.</summary>
		Complete,
		///<summary>1 - Code RE</summary>
		Refused,
		///<summary>2 - Code NA</summary>
		NotAdministered,
		///<summary>3 - Code PA</summary>
		PartiallyAdministered,
	}

	///<summary>Exported in HL7 RXA-9.  Corresponds to CDC code set NIP001 (http://hl7v2-iz-testing.nist.gov/mu-immunization/).</summary>
	public enum VaccineAdministrationNote {
		///<summary>0 - Code 00.  Default.</summary>
		NewRecord,
		///<summary>1 - Code 01</summary>
		HistoricalSourceUnknown,
		///<summary>2 - Code 02</summary>
		HistoricalOtherProvider,
		///<summary>3 - Code 03</summary>
		HistoricalParentsWrittenRecord,
		///<summary>4 - Code 04</summary>
		HistoricalParentsRecall,
		///<summary>5 - Code 05</summary>
		HistoricalOtherRegistry,
		///<summary>6 - Code 06</summary>
		HistoricalBirthCertificate,
		///<summary>7 - Code 07</summary>
		HistoricalSchoolRecord,
		///<summary>8 - Code 08</summary>
		HistoricalPublicAgency,
	}

	///<summary>Exported in HL7 RXA-18.  Corresponds to CDC code set NIP002 (http://hl7v2-iz-testing.nist.gov/mu-immunization/).</summary>
	public enum VaccineRefusalReason {
		///<summary>0 - No code.  Default.  Not sent in HL7 messages.  Only used in UI.</summary>
		None,
		///<summary>1 - Code 00</summary>
		ParentalDecision,
		///<summary>2 - Code 01</summary>
		ReligiousExemption,
		///<summary>3 - Code 02</summary>
		Other,
		///<summary>4 - Code 03</summary>
		PatientDecision,
	}

	///<summary>Exported in HL7 RXA-21.  Corresponds to HL7 table 0323 (guide page 225).</summary>
	public enum VaccineAction {
		///<summary>0 - Code A.  Default.</summary>
		Add,
		///<summary>1 - Code D</summary>
		Delete,
		///<summary>2 - Code U</summary>
		Update
	}

	///<summary>Exported in HL7 RXR-1.  Corresponds to HL7 table 0162 (guide page 200).</summary>
	public enum VaccineAdministrationRoute {
		///<summary>0 - No code.  Default.  Not sent in HL7 messages.  Used in UI only.</summary>
		None,
		///<summary>1 - Code ID.</summary>
		Intradermal,
		///<summary>2 - Code IM.</summary>
		Intramuscular,
		///<summary>3 - Code NS.</summary>
		Nasal,
		///<summary>4 - Code IV.</summary>
		Intravenous,
		///<summary>5 - Code PO.</summary>
		Oral,
		///<summary>6 - Code OTH.</summary>
		Other,
		///<summary>7 - Code SC.</summary>
		Subcutaneous,
		///<summary>8 - Code TD.</summary>
		Transdermal,
	}

	///<summary>Exported in HL7 RXR-2.  Corresponds to HL7 table 0163 (guide page 201).</summary>
	public enum VaccineAdministrationSite {
		///<summary>0 - No code.  Default.  Not sent in HL7 messages.  Used in UI only.</summary>
		None,
		///<summary>1- Code LT</summary>
		LeftThigh,
		///<summary>2 - Code LA</summary>
		LeftArm,
		///<summary>3 - Code LD</summary>
		LeftDeltoid,
		///<summary>4 - Code LG</summary>
		LeftGluteousMedius,
		///<summary>5 - Code LVL</summary>
		LeftVastusLateralis,
		///<summary>6 - Code LLFA</summary>
		LeftLowerForearm,
		///<summary>7 - Code RA</summary>
		RightArm,
		///<summary>8 - Code RT</summary>
		RightThigh,
		///<summary>9 - Code RVL</summary>
		RightVastusLateralis,
		///<summary>10 - Code RG</summary>
		RightGluteousMedius,
		///<summary>11 - Code RD</summary>
		RightDeltoid,
		///<summary>12 - Code RLFA</summary>
		RightLowerForearm,

	}

}