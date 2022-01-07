using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Vaccine observation.  There may be multiple vaccine observations for each vaccine.</summary>
	[Serializable]
	public class VaccineObs:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long VaccineObsNum;
		///<summary>FK to vaccinepat.VaccinePatNum. </summary>
		public long VaccinePatNum;
		///<summary>Enum:VaccineObsType Coded, Dated, Numeric, Text, DateAndTime.  Used in HL7 OBX-2.</summary>
		public VaccineObsType ValType;
		///<summary>Enum:VaccineObsIdentifier  Identifies the observation question.  Used in HL7 OBX-3.</summary>
		public VaccineObsIdentifier IdentifyingCode;
		///<summary>The observation value.  The type of the value depends on the ValType.  Used in HL7 OBX-5.</summary>
		public string ValReported;
		///<summary>Enum:VaccineObsValCodeSystem  CVX, HL70064.  The observation value code system when ValType is Coded.  Used in HL7 OBX-5.</summary>
		public VaccineObsValCodeSystem ValCodeSystem;
		///<summary>FK to vaccineobs.VaccineObsNum.  All vaccineobs records with matching GroupId are in the same group.  Set to 0 if this vaccine observation is not part of a group.  Used in HL7 OBX-4.</summary>
		public long VaccineObsNumGroup;
		///<summary>Used in HL7 OBX-6.</summary>
		public string UcumCode;
		///<summary>Date of observation.  Used in HL7 OBX-14.</summary>
		public DateTime DateObs;
		///<summary>Code from code set CDCPHINVS (this code system is not yet fully defined, so user has to enter manually).  Used in HL7 OBX-17.  Only required when IdentifyingCode is FundPgmEligCat.</summary>
		public string MethodCode;

		public VaccineObs Clone() {
			return (VaccineObs)this.MemberwiseClone();
		}
	}

	///<summary>Corresponds to HL7 table 0125.</summary>
	public enum VaccineObsType {
		///<summary>0 - Code CE.  Coded entry. (default)</summary>
		Coded,
		///<summary>1 - Code DT.  Date (no time).</summary>
		Dated,
		///<summary>2 - Code NM.  Numeric.</summary>
		Numeric,
		///<summary>3 - Code ST.  String.</summary>
		Text,
		///<summary>4 - Code TS.  Date and time.</summary>
		DateAndTime
	}

	///<summary>Corresponds to HL7 value set NIP003 (http://hl7v2-iz-testing.nist.gov/mu-immunization/).
	///This code set is a subset of LOINC codes.  Used in HL7 OBX-3.</summary>
	public enum VaccineObsIdentifier {
		///<summary>0 - LOINC code 29768-9.  Date vaccine information statement published:TmStp:Pt:Patient:Qn: (default)</summary>
		DatePublished,
		///<summary>1 - LOINC code 29769-7.  Date vaccine information statement presented:TmStp:Pt:Patient:Qn:</summary>
		DatePresented,
		///<summary>2 - LOINC code 30944-3.  Date of vaccination temporary contraindication and or precaution expiration:TmStp:Pt:Patient:Qn:</summary>
		DatePrecautionExpiration,
		///<summary>3 - LOINC code 30945-0.  Vaccination contraindication and or precaution:Find:Pt:Patient:Nom:</summary>
		Precaution,
		///<summary>4 - LOINC code 30946-8.  Date vaccination contraindication and or precaution effective:TmStp:Pt:Patient:Qn:</summary>
		DatePrecautionEffective,
		///<summary>5 - LOINC code 30956-7.  Type:ID:Pt:Vaccine:Nom:</summary>
		TypeOf,
		///<summary>6 - LOINC code 30963-3.  Funds vaccine purchased with:Find:Pt:Patient:Nom:</summary>
		FundsPurchasedWith,
		///<summary>7 - LOINC code 30973-2.  Dose number:Num:Pt:Patient:Qn:</summary>
		DoseNumber,
		///<summary>8 - LOINC code 30979-9.  Vaccines due next:Cmplx:Pt:Patient:Set:</summary>
		NextDue,
		///<summary>9 - LOINC code 30980-7.  Date vaccine due:TmStp:Pt:Patient:Qn:</summary>
		DateDue,
		///<summary>10 - LOINC code 30981-5.  Earliest date to give:TmStp:Pt:Patient:Qn:</summary>
		DateEarliestAdminister,
		///<summary>11 - LOINC code 30982-3.  Reason applied by forcast logic to project this vaccine:Find:Pt:Patient:Nom:</summary>
		ReasonForcast,
		///<summary>12 - LOINC code 31044-1.  Reaction:Find:Pt:Patient:Nom:</summary>
		Reaction,
		///<summary>13 - LOINC code 38890-0.  Vaccine component type:ID:Pt:Vaccine:Nom:</summary>
		ComponentType,
		///<summary>14 - LOINC code 46249-9.  Vaccination take-response type:Prid:Pt:Patient:Nom:</summary>
		TakeResponseType,
		///<summary>15 - LOINC code 46250-7.  Vaccination take-response date:TmStp:Pt:Patient:Qn:</summary>
		DateTakeResponse,
		///<summary>16 - LOINC code 59779-9.  Immunization schedule used:Find:Pt:Patient:Nom:</summary>
		ScheduleUsed,
		///<summary>17 - LOINC code 59780-7.  Immunization series:Find:Pt:Patient:Nom:</summary>
		Series,
		///<summary>18 - LOINC code 59781-5.  Dose validity:Find:Pt:Patient:Ord:</summary>
		DoseValidity,
		///<summary>19 - LOINC code 59782-3.  Number of doses in primary immunization series:Num:Pt:Patient:Qn:</summary>
		NumDosesPrimary,
		///<summary>20 - LOINC code 59783-1.  Status in immunization series:Find:Pt:Patient:Nom:</summary>
		StatusInSeries,
		///<summary>21 - LOINC code 59784-9.  Disease with presumed immunity:Find:Pt:Patient:Nom:</summary>
		DiseaseWithImmunity,
		///<summary>22 - LOINC code 59785-6.  Indication for Immunization:Find:Pt:Patient:Nom:</summary>
		Indication,
		///<summary>23 - LOINC code 64994-7.  Vaccine fund pgm elig cat</summary>
		FundPgmEligCat,
		///<summary>24 - LOINC code 69764-9.  Document type</summary>
		DocumentType,
	}

	///<summary>Used in HL7 OBX-5.</summary>
	public enum VaccineObsValCodeSystem {
		///<summary>0 (default)</summary>
		CVX,
		///<summary>1</summary>
		HL70064,
		///<summary>2</summary>
		SCT,
	}

}
