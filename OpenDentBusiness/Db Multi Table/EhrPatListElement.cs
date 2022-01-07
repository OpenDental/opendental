using EhrLaboratories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Used by ehr "Generate Patient Lists".  This object represents one row in the grid when building the report.  Multiple such elements will be ANDed together to automatically generate a query.</summary>
	public class EhrPatListElement {
		///<summary>Birthdate, Disease, Medication, or LabResult</summary>
		public EhrRestrictionType Restriction;
		///<summary>For all 4 types, what to compare against.  Examples:  Birthdate: '50', Disease: '4140' (icd9 code will be followed by wildcard), Medication: 'Lisinopril' (not case sensitive, surrounded by wildcards), LabResult: 'HDL-cholesterol'.</summary>
		public string CompareString;
		///<summary>gt, lt, or equal.  Only used for lab and birthdate.</summary>
		public EhrOperand Operand;
		///<summary>Only used for Lab.  Usually a number.</summary>
		public string LabValue;
		///<summary></summary>
		public bool OrderBy;
	}

	///<summary>Add to end of list, do not change the order.</summary>
	public enum EhrRestrictionType {
		Birthdate,
		Problem,
		Medication,
		LabResult,
		Gender,
		CommPref,
		Allergy
	}

	public enum EhrOperand {
		GreaterThan,
		LessThan,
		Equals
	}

	///<summary>Used by ehr "Generate Patient Lists".  This object represents one row in the grid when building the report.  Multiple such elements will be ANDed together to automatically generate a query.</summary>
	public class EhrPatListElement2014 {
		///<summary>Birthdate, Disease, Medication, or LabResult</summary>
		public EhrRestrictionType Restriction;
		///<summary>For all 4 types, what to compare against.  Examples:  Birthdate: '50', Disease: '4140' (icd9 code will be followed by wildcard), Medication: 'Lisinopril' (not case sensitive, surrounded by wildcards), LabResult: 'HDL-cholesterol'. Allergy:'Allergy - Morphene'. CommPref:'MobilePh'(exact enum names)</summary>
		public string CompareString;
		///<summary>gt, lt, or equal.  Only used for lab and birthdate.</summary>
		public EhrOperand Operand;
		///<summary>Only used for Lab.  Usually a number.</summary>
		public string LabValue;
		///<summary>Only select records after this date, i.e. date of diagnosis, prescription, lab date, etc... If ==null or ==DateTime.MinValue this value is ignored.</summary>
		public DateTime StartDate;
		///<summary>Only select records before this date, i.e. date of diagnosis, prescription, lab date, etc... If ==null or ==DateTime.MinValue this value is ignored.</summary>
		public DateTime EndDate;
		///<summary>Used to determine how the LabValue should be compaired.</summary>
		public HL70125 LabValueType;
		///<summary>Ucum codes. Example: mg/dL</summary>
		public string LabValueUnits;
	}
}
