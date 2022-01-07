using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>An EHR appointment observation.  Needed for syndromic surveillance messaging.  Each syndromic message requires at least one observation.</summary>
	[Serializable]
	public class EhrAptObs:TableBase {

		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrAptObsNum;
		///<summary>FK to appointment.AptNum.  There can be an unlimited number of observations per appointment.</summary>
		public long AptNum;
		///<summary>Enum:EhrAptObsIdentifier - Used in HL7 OBX-3 for syndromic surveillance.</summary>
		public EhrAptObsIdentifier IdentifyingCode;
		///<summary>Enum:EhrAptObsType .  Used in HL7 OBX-2 for syndromic surveillance.  Identifies the data type for the observation value in ValReported.</summary>
		public EhrAptObsType ValType;
		///<summary>The value of the observation. The value format must match the ValType.  This field could be text, a datetime, a code, etc..  Used in HL7 OBX-5 for syndromic surveillance.</summary>
		public string ValReported;
		///<summary>Used in HL7 OBX-6 for syndromic surveillance when ValType is Numeric (otherwise left blank).</summary>
		public string UcumCode;
		///<summary>When ValType is Coded, then this contains the code system corresponding to the code in ValReported.  When ValType is not Coded, then this field should be blank.
		///Allowed values are LOINC,SNOMEDCT,ICD9,ICD10.</summary>
		public string ValCodeSystem;

		public EhrAptObs Clone() {
			return (EhrAptObs)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum EhrAptObsIdentifier {
		///<summary>0 - Body temperature:Temp:Enctrfrst:Patient:Qn:	Loinc code 11289-6.</summary>
		BodyTemp,
		///<summary>1 - Illness or injury onset date and time:TmStp:Pt:Patient:Qn:	Loinc code 11368-8.</summary>
		DateIllnessOrInjury,
		///<summary>2 - Age Time Patient Reported	Loinc code 21612-7.</summary>
		PatientAge,
		///<summary>3 - Diagnosis.preliminary:Imp:Pt:Patient:Nom:	Loinc code 44833-2.</summary>
		PrelimDiag,
		///<summary>4 - Triage note:Find:Pt:Emergency department:Doc:	Loinc code 54094-8.</summary>
		TriageNote,
		///<summary>5 - Oxygen saturation:MFr:Pt:BldA:Qn:Pulse oximetry	Loinc code 59408-5.</summary>
		OxygenSaturation,
		///<summary>6 - Chief complaint:Find:Pt:Patient:Nom:Reported	Loinc code 8661-1.</summary>
		CheifComplaint,
		///<summary>7 - Treating Facility Identifier	PHINQUESTION code SS001.</summary>
		TreatFacilityID,
		///<summary>8 - Treating Facility Location	PHINQUESTION code SS002.</summary>
		TreatFacilityLocation,
		///<summary>9 - Facility / Visit Type	PHINQUESTION code SS003.</summary>
		VisitType,
	}

	///<summary></summary>
	public enum EhrAptObsType {
		///<summary>0 - This should only be used with EhrAptObsIdentifier.TreatFacilityLocation.</summary>
		Address,
		///<summary>1</summary>
		Coded,
		///<summary>2</summary>
		DateAndTime,
		///<summary>3</summary>
		Numeric,
		///<summary>4</summary>
		Text,
	}

}
