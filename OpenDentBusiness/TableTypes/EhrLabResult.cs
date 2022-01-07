using System;
using System.Collections.Generic;
using EhrLaboratories;

namespace OpenDentBusiness {
	///<summary>For EHR module, lab result that contains all required fields for HL7 Lab Reporting Interface (LRI).  OBX</summary>
	[Serializable]
	public class EhrLabResult:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrLabResultNum;
		///<summary>FK to ehrlab.EhrLabNum.</summary>
		public long EhrLabNum;
		///<summary>Enumerates the OBX segments within a single message starting with 1.  OBX.1</summary>
		public long SetIdOBX;
		///<summary>This field identifies the data type used for ObservationValue (OBX-5).  OBX.2</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70125 ValueType;


		#region Observation Identifier (Loinc Codes)  OBX.3
		///<summary>"LOINC shall be used as the standard coding system for this field if an appropriate LOINC code exists. Appropriate status is defined in the LOINC Manual Section 11.2 Classification of LOINC Term Status. If a local coding system is in use, a local code should also be sent to help with identification of coding issues. When no valid LOINC exists the local code may be the only code sent.  When populating this field with values, this guide does not give preference to the triplet in which the standard (LOINC) code should appear." OBX.3.1</summary>
		public string ObservationIdentifierID;
		///<summary>Description of ObservationIdentifierId. OBX.3.2</summary>
		public string ObservationIdentifierText;
		///<summary>CodeSystem that ObservationIdentifierId came from. Should be "LN".  OBX.3.3</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string ObservationIdentifierCodeSystemName;
		///<summary>Probably a LoincCode or empty.  OBX.3.4</summary>
		public string ObservationIdentifierIDAlt;
		///<summary>Description of ObservationIdentifierIdAlt.  OBX.3.5</summary>
		public string ObservationIdentifierTextAlt;
		///<summary>CodeSystem that ObservationIdentifierId came from. Should be "LN" or empty.  OBX.3.6</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string ObservationIdentifierCodeSystemNameAlt;
		///<summary>Optional text that describes the original text used to encode the values above.  OBX.3.7</summary>
		public string ObservationIdentifierTextOriginal;
		///<summary>OBX.4</summary>
		public string ObservationIdentifierSub;
		#endregion


		#region Observation Value.  Can be one of 10 different data types, stored in 15 different columns, only 7 of which should be in use one the same observation.  OBX.5
		#region if CE or CWE (Coded Elements) Should be Snomed CT Codes
		///<summary>OBX.5.1</summary>
		public string ObservationValueCodedElementID;
		///<summary>Description of ObservationValueCodedElementId.  OBX.5.2</summary>
		public string ObservationValueCodedElementText;
		///<summary>CodeSystem that ObservationValueCodedElementId came from.  OBX.5.3</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string ObservationValueCodedElementCodeSystemName;
		///<summary>OBX.5.4</summary>
		public string ObservationValueCodedElementIDAlt;
		///<summary>Description of ObservationValueCodedElementIdAlt.  OBX.5.5</summary>
		public string ObservationValueCodedElementTextAlt;
		///<summary>CodeSystem that ObservationValueCodedElementId came from.  OBX.5.6</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string ObservationValueCodedElementCodeSystemNameAlt;
		///<summary>CWE only.  Optional text that describes the original text used to encode the values above.  OBX.5.7</summary>
		public string ObservationValueCodedElementTextOriginal;
		#endregion 
		#region if DT (YYYY[MM[DD]]) or TS(YYYYMMDDHHMMSS)
		///<summary>Stored as string in the formatYYYY[MM[DD]] for DT and YYYYMMDDHHMMSS for TS. Note: this is the lab result value, not the DT the test was performed. OBX.5.1</summary>
		public string ObservationValueDateTime;
		#endregion
		#region if TM (Time)
		///<summary>Note: this is the lab result value, not the time the test was performed. OBX.5.1</summary>
		public TimeSpan ObservationValueTime;
		#endregion
		#region if SN (Structured Numeric) Examples: intervals(^0^-^1), ratios(^1^/^2 or ^1^:^2), inequalities(<^10), or categorical results(2^+)
		///<summary>OBX.5.1</summary>
		public string ObservationValueComparator;
		///<summary>OBX.5.2</summary>
		public double ObservationValueNumber1;
		///<summary>OBX.5.3</summary>
		public string ObservationValueSeparatorOrSuffix;
		///<summary>OBX.5.4</summary>
		public double ObservationValueNumber2;
		#endregion
		#region if NM (Numeric)
		///<summary>OBX.5.1</summary>
		public double ObservationValueNumeric;
		#endregion
		#region if FT, ST, or TX (Text/String Data)
		///<summary>OBX.5.1</summary>
		public string ObservationValueText;
		#endregion
		#endregion


		#region Units.  Should be UCUM values.  OBX.6
		///<summary>"UCUM (Unified Code for Units of Measure) will be evaluated during the pilot for potential subsequent inclusion. As part of the pilot test, for dimensionless units the UCUM representation could be {string}, e.g., for titer the pilot might use {titer} to test feasibility. When sending units of measure as text, they must be placed in the correct component of OBX-6 (CWE_CRE.9)."  OBX.6.1</summary>
		public string UnitsID;
		///<summary>Description of UnitsId.  OBX.6.2</summary>
		public string UnitsText;
		///<summary>CodeSystem that UnitsId came from. Should be "UCUM".  OBX.6.3</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string UnitsCodeSystemName;
		///<summary>OBX.6.4</summary>
		public string UnitsIDAlt;
		///<summary>Description of UnitsIdAlt.  OBX.6.5</summary>
		public string UnitsTextAlt;
		///<summary>CodeSystem that UnitsId came from.  OBX.6.6</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string UnitsCodeSystemNameAlt;
		///<summary>Optional text that describes the original text used to encode the values above.  OBX.6.7</summary>
		public string UnitsTextOriginal;
		#endregion


		///<summary>"Guidance: It is not appropriate to send the reference range for a result in an associated NTE segment. It would be appropriate to send additional information clarifying the reference range in an NTE associated with this OBX-"  OBX.7</summary>
		public string referenceRange;
		///<summary>Comma Delimited list of Abnormal Flags using HL70078 enum values.  OBX.8.*</summary>
		public string AbnormalFlags;
		///<summary>[0..*] This is not a data column but is stored in a seperate table named EhrLabNote. OBX.*</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<EhrLabNote> _listEhrLabResultNotes;
		///<summary>Coded status of result.  OBX.11</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70085 ObservationResultStatus;
		///<summary>Stored as string in the format YYYYMMDD[HH[MM[SS]]]. "For specimen based test, if it is valued it must be the same as SPM-17.  
		///If SPM-17 is present and relates to the same observation, then OBX-14 must be within the DR range."  OBX.14.1</summary>
		public string ObservationDateTime;
		///<summary>Stored as string in the format YYYYMMDD[HH[MM[SS]]].  "Be as precise as appropriate and available."  OBX.19.1</summary>
		public string AnalysisDateTime;
		#region Performing Organization Name OBX.23
		///<summary>OBX.23.1</summary>
		public string PerformingOrganizationName;
		#region Performing Organization Name Assigning Authority (The Assigning Authority component is used to identify the system, application, organization, etc. that assigned the ID in component 10.)  OBX.23.6
		///<summary>OBX.23.6.1</summary>
		public string PerformingOrganizationNameAssigningAuthorityNamespaceId;
		///<summary>The Assigning Authority component is used to identify the system, application, organization, etc. that assigned the ID in component 10.  OBX.23.6.2</summary>
		public string PerformingOrganizationNameAssigningAuthorityUniversalId;
		///<summary>Should always be "ISO", unless importing.  OBX.23.6.3</summary>
		public string PerformingOrganizationNameAssigningAuthorityUniversalIdType;
		#endregion
		///<summary>OBX.23.7</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70203 PerformingOrganizationIdentifierTypeCode;
		///<summary>OBX.23.10</summary>
		public string PerformingOrganizationIdentifier;
		#endregion
		#region Preforming Organization Address OBX.24
		///<summary>OBX.24.1.1</summary>
		public string PerformingOrganizationAddressStreet;
		///<summary>OBX.24.2</summary>
		public string PerformingOrganizationAddressOtherDesignation;
		///<summary>OBX.24.3</summary>
		public string PerformingOrganizationAddressCity;
		///<summary>USPS Alpha State Codes.  OBX.24.4</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public USPSAlphaStateCode PerformingOrganizationAddressStateOrProvince;
		///<summary>OBX.24.5</summary>
		public string PerformingOrganizationAddressZipOrPostalCode;
		///<summary>Should be the three letter Alpha Code derived from ISO 3166 alpha-3 code set. http://www.nationsonline.org/oneworld/country_code_list.htm OBX.24.6</summary>
		public string PerformingOrganizationAddressCountryCode;
		///<summary>OBX.24.7</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70190 PerformingOrganizationAddressAddressType;
		///<summary>Should be based on FIPS 6-4. We are just importing the string as is. OBX.24.8</summary>
		public string PerformingOrganizationAddressCountyOrParishCode;
		#endregion
		#region Performing Organization Medical Director OBX.25
		///<summary>May be provnum or NPI num or any other num, when combined with MedicalDirectorIdAssigningAuthority should uniquely identify the provider.  OBX.25.1</summary>
		public string MedicalDirectorID;
		///<summary>OBX.25.2</summary>
		public string MedicalDirectorLName;
		///<summary>OBX.25.3</summary>
		public string MedicalDirectorFName;
		///<summary>Middle names or initials therof.  OBX.25.4</summary>
		public string MedicalDirectorMiddleNames;
		///<summary>Example: JR or III.  OBX.25.5</summary>
		public string MedicalDirectorSuffix;
		///<summary>Example: DR, Not MD, MD would be stored in an optional field that was not implemented called MedicalDirectorDegree.  OBX.25.6</summary>
		public string MedicalDirectorPrefix;
		#region Medical Directory Id Assigning Authority OBX.25.9
		///<summary>Usually empty, "The value of [this field] reflects a local code that represents the combination of [the next two fields]."  OBX.25.9.1</summary>
		public string MedicalDirectorAssigningAuthorityNamespaceID;
		///<summary>ISO compliant OID that represents the organization that assigned the unique provider ID.  OBX.25.9.2</summary>
		public string MedicalDirectorAssigningAuthorityUniversalID;
		///<summary>Always "ISO", unless importing from outside source.  OBX.25.9.3</summary>
		public string MedicalDirectorAssigningAuthorityIDType;
		#endregion
		///<summary>Describes the type of name used.  OBX.25.10</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70200 MedicalDirectorNameTypeCode;
		///<summary>Must be value from HL70203 code set, see note at bottom of EhrLab.cs for usage.  OBX.25.13</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70203 MedicalDirectorIdentifierTypeCode;
		#endregion


		///<summary></summary>
		public EhrLabResult Copy() {
			return (EhrLabResult)MemberwiseClone();
		}

		public EhrLabResult() {
			AbnormalFlags="";
		}

		///<summary>Only filled with EhrLabResultNotes when value is used.  To refresh ListEhrLabResults, set it equal to null or explicitly reassign it using EhrLabNotes.GetForLabResult(EhrLabResultNum).</summary>
		public List<EhrLabNote> ListEhrLabResultNotes {
			get {
				if(_listEhrLabResultNotes==null) {
					_listEhrLabResultNotes=EhrLabNotes.GetForLabResult(EhrLabResultNum);
				}
				return _listEhrLabResultNotes;
			}
			set {
				_listEhrLabResultNotes=value;
			}
		}

	}

}

namespace EhrLaboratories {
	///<summary>Value Type.  OID:2.16.840.1.113883.12.125  HL70369 code:HL70125.  Source HL7 2.5.1 Labratory Reporting Interface documentation.
	///<para>This enum is also used in FormPatListElementEditEHR2014.cs and assumes the order of this enum does not change. If it does, the combo box filled with these values must also be updated.</para></summary>
	public enum HL70125 {
		///<summary>0 - Coded Entry.
		///<para>Usage: R </para>
		///<para>Comment: When sending text data in OBX-5, use either the ST, TX or FT data types. </para></summary>
		CE,
		///<summary>1 - Coded with Exceptions.
		///<para>Usage: R </para>
		///<para>Data Type Flavor: CWE_CRO </para>
		///<para>Comment: Data type to be used where it is important to communicate the coding system and coding system version with the coded result being reported. Pre-adopted from Version 2.6.  This Implementation Guide has specially constrained versions of the CWE data type in Section 2.2 through 2.4. The CWE_CR format shall be used for OBX-5. When sending text data in OBX-5, use either the ST, TX or FT data types. </para></summary>
		CWE,
		/////<summary>Extended Composite ID With Check Digit.
		/////<para>Usage: O </para>
		/////<para>Data Type Flavor: Varies </para>
		/////<para>Comment: Use the appropriate CX flavor (CX-GU or CX-NG or base standard) depending on the format of the observation value and as agreed to between the sender/receiver. </para></summary>
		//CX,
		///<summary>2 - Date.
		///<para>Usage: R </para></summary>
		DT,
		/////<summary>Encapsulated Data.
		/////<para>Usage: O </para>
		/////<para>Comment: When using the Source Application ID component it should use the HD data type formatting considerations outlined in the base standard, not the constrained HD definitions in this IG. </para></summary>
		//ED,
		///<summary>3 - Formatted Text (Display).
		///<para>Usage: R </para>
		///<para>Comment: Field using the FT data type to carry a text result value. This is intended for display. The text may contain formatting escape sequences as described in the data types section. Numeric results and numeric results with units of measure should not be reported as text. These should be reported as NM or SN numeric results, with the units of measure in OBX-6. </para></summary>
		FT,
		///<summary>4 - Numeric.
		///<para>Usage: R </para>
		///<para>Comment: Field using the NM data type to carry a numeric result value. The only non-numeric characters allowed in this field are a leading plus (+) or minus (-) sign. The structured numeric (SN) data type should be used for conveying inequalities, ranges, ratios, etc. The units for the numeric value should be reported in OBX-6. </para></summary>
		NM,
		/////<summary>Reference Pointer.
		/////<para>Usage: O </para>
		/////<para>Comment: When using the Application ID component it should use the HD data type formatting considerations outlined in the base standard, not the constrained HD definitions in this IG. </para></summary>
		//RP,
		///<summary>5 - Structured Numeric.
		///<para>Usage: R </para>
		///<para>Comment: Field using the SN data type to carry a structured numeric result value. Structured numeric include intervals (^0^-^1), ratios (^1^/^2 or ^1^: ^2), inequalities (&lt;^10), or categorical results (2^+). The units for the structured numeric value should be reported in OBX-6. </para></summary>
		SN,
		///<summary>6 - String Data.
		///<para>Usage: R </para>
		///<para>Comment: Field using the ST data type to carry a short text result value. Numeric results and numeric results with units of measure should not be reported as text. These shall be reported as NM or SN numeric results, with the units of measure in OBX-6. </para></summary>
		ST,
		///<summary>7 - Time.
		///<para>Usage: R </para>
		///<para>Comment: The timezone offset shall adhere to the use of the TimeZone Offset profile. </para></summary>
		TM,
		///<summary>8 - Time Stamp (Date and Time).
		///<para>Usage: R </para>
		///<para>Data Type Flavor: TS_0 </para>
		///<para>Comment: The timezone offset shall adhere to the use of the TimeZone Offset profile and associated discussion if the granularity involves hh or “more”. </para></summary>
		TS,
		///<summary>9 - Text Data (Display).
		///<para>Usage: R </para>
		///<para>Comment: Field using the TX data type to carry a text result value this is intended for display. Numeric results and numeric results with units of measure should not be reported as text. These should be reported as NM or SN numeric results, with the units of measure in OBX-6. </para></summary>
		TX 
	}

	///<summary>Abnormal Flags.  OID:2.16.840.1.113883.12.78  HL70369 code:HL70078.  Source phinvads.cdc.gov</summary>
	public enum HL70078 {
		///<summary>0 - Abnormal
		///<para>Applies to non-numeric results.</para></summary>
		A,
		///<summary>1 - Above absolute high-off instrument scale.  Actual value is "&gt;" but symbol cannot be used as an enum value.</summary>
		_gt,
		///<summary>2 - Above high normal</summary>
		H,
		///<summary>3 - Above upper panic limits</summary>
		HH,
		///<summary>4 - Below absolute low-off instrument scale.  Actual value is "&lt;" but symbol cannot be used as an enum value.</summary>
		_lt,
		///<summary>5 - Below low normal</summary>
		L,
		///<summary>6 - Below lower panic limits</summary>
		LL,
		///<summary>7 - Better
		///<para>Use when direction not relevant.</para></summary>
		B,
		///<summary>8 - Intermediate
		///<para>Indicates for microbiology susceptibilities only.</para></summary>
		I,
		///<summary>9 - Moderately susceptible
		///<para>Indicates for microbiology susceptibilities only</para></summary>
		MS,
		///<summary>10 - No range defined, or normal ranges don't apply.  Actual value is "null" but is a reserved word in c#</summary>
		_null,
		///<summary>11 - Normal
		///<para>Applies to non-numeric results.</para></summary>
		N,
		///<summary>12 - Resistant
		///<para>Indicates for microbiology susceptibilities only.</para></summary>
		R,
		///<summary>13 - Significant change down</summary>
		D,
		///<summary>14 - Significant change up</summary>
		U,
		///<summary>15 - Susceptible
		///<para>Indicates for microbiology susceptibilities only.</para></summary>
		S,
		///<summary>16 - Very abnormal
		///<para>Applies to non-numeric units, analogous to panic limits for numeric units.</para></summary>
		AA,
		///<summary>17 - Very susceptible
		///<para>Indicates for microbiology susceptibilities only.</para></summary>
		VS,
		///<summary>18 - Worse
		///<para>Use when direction not relevant.</para></summary>
		W
	}

	///<summary>Observation Result Status.  OID:2.16.840.1.113883.12.85  HL70369 code:HL70085.  Source phinvads.cdc.gov</summary>
	public enum HL70085 {
		///<summary>0 - Deletes the OBX record</summary>
		D,
		///<summary>1 - Final results; Can only be changed with a corrected result.</summary>
		F,
		///<summary>2 - Not asked; used to affirmatively document that the observation identified in the OBX was not sought when the universal service ID in OBR-4 implies that it would be sought.</summary>
		N,
		///<summary>3 - Order detail description only (no result)</summary>
		O,
		///<summary>4 - Partial results</summary>
		S,
		///<summary>5 - Post original as wrong, e.g., transmitted for wrong patient</summary>
		W,
		///<summary>6 - Preliminary results</summary>
		P,
		///<summary>7 - Record coming over is a correction and thus replaces a final result</summary>
		C,
		///<summary>8 - Results cannot be obtained for this observation</summary>
		X,
		///<summary>9 - Results entered -- not verified</summary>
		R,
		///<summary>10 - Results status change to final without retransmitting results already sent as _preliminary._  E.g., radiology changes status from preliminary to final</summary>
		U,
		///<summary>11 - Specimen in lab; results pending</summary>
		I
	}

	///<summary>Address Type.  OID:2.16.840.1.113883.12.190  Source phinvads.cdc.gov</summary>
	public enum HL70190 {
		///<summary>0 - Bad address</summary>
		BA,
		///<summary>1 - Birth (nee) (birth address, not otherwise specified)</summary>
		N,
		///<summary>2 - Birth delivery location (address where birth occurred)</summary>
		BDL,
		///<summary>3 - Country Of Origin</summary>
		F,
		///<summary>4 - Current Or Temporary</summary>
		C,
		///<summary>5 - Firm/Business</summary>
		B,
		///<summary>6 - Home</summary>
		H,
		///<summary>7 - Legal Address</summary>
		L,
		///<summary>8 - Mailing</summary>
		M,
		///<summary>9 - Office</summary>
		O,
		///<summary>10 - Permanent</summary>
		P,
		///<summary>11 - Registry home. 
		///<para>Refers to the information system, typically managed by a public health agency, that stores patient information such as immunization histories or cancer data, regardless of where the patient obtains Services.</para></summary>
		RH,
		///<summary>12 - Residence at birth (home address at time of birth)</summary>
		BR

	}

	///<summary>(Not Implemented) Observation Result Status.  OID:2.16.840.1  Source phinvads.cdc.gov</summary>
	public enum HL70399 {
		//Not implemented. Instead we just import whatever was sent to us.
	}

	///<summary>Three digit county codes based on FIPS 6-4, AKA HL70289.  OID:2.16.840.1.114222.4.11.829  Source phinvads.cdc.gov</summary>
	public enum USCountyCode {
		//Not Implemented. Instead we just import whatever was sent to us.
	}

	///<summary>USPS Alpha State Code.  Source: http://www.itl.nist.gov/fipspubs/fip5-2.htm </summary>
	public enum USPSAlphaStateCode {
		///<summary>0 - Alabama</summary>
		AL,
		///<summary>1 - Alaska</summary>
		AK,
		///<summary>2 - Arizona</summary>
		AZ,
		///<summary>3 - Arkansas</summary>
		AR,
		///<summary>4 - California</summary>
		CA,
		///<summary>5 - Colorado</summary>
		CO,
		///<summary>6 - Connecticut</summary>
		CT,
		///<summary>7 - Delaware</summary>
		DE,
		///<summary>8 - District of Columbia</summary>
		DC,
		///<summary>9 - Florida</summary>
		FL,
		///<summary>10 - Georgia</summary>
		GA,
		///<summary>11 - Hawaii</summary>
		HI,
		///<summary>12 - Idaho</summary>
		ID,
		///<summary>13 - Illinois</summary>
		IL,
		///<summary>14 - Indiana</summary>
		IN,
		///<summary>15 - Iowa</summary>
		IA,
		///<summary>16 - Kansas</summary>
		KS,
		///<summary>17 - Kentucky</summary>
		KY,
		///<summary>18 - Louisiana</summary>
		LA,
		///<summary>19 - Maine</summary>
		ME,
		///<summary>20 - Maryland</summary>
		MD,
		///<summary>21 - Massachusetts</summary>
		MA,
		///<summary>22 - Michigan</summary>
		MI,
		///<summary>23 - Minnesota</summary>
		MN,
		///<summary>24 - Mississippi</summary>
		MS,
		///<summary>25 - Missouri</summary>
		MO,
		///<summary>26 - Montana</summary>
		MT,
		///<summary>27 - Nebraska</summary>
		NE,
		///<summary>28 - Nevada</summary>
		NV,
		///<summary>29 - New Hampshire</summary>
		NH,
		///<summary>30 - New Jersey</summary>
		NJ,
		///<summary>31 - New Mexico</summary>
		NM,
		///<summary>32 - New York</summary>
		NY,
		///<summary>33 - North Carolina</summary>
		NC,
		///<summary>34 - North Dakota</summary>
		ND,
		///<summary>35 - Ohio</summary>
		OH,
		///<summary>36 - Oklahoma</summary>
		OK,
		///<summary>37 - Oregon</summary>
		OR,
		///<summary>38 - Pennsylvania</summary>
		PA,
		///<summary>39 - Rhode Island</summary>
		RI,
		///<summary>40 - South Carolina</summary>
		SC,
		///<summary>41 - South Dakota</summary>
		SD,
		///<summary>42 - Tennessee</summary>
		TN,
		///<summary>43 - Texas</summary>
		TX,
		///<summary>44 - Utah</summary>
		UT,
		///<summary>45 - Vermont</summary>
		VT,
		///<summary>46 - Virginia</summary>
		VA,
		///<summary>47 - Washington</summary>
		WA,
		///<summary>48 - West Virginia</summary>
		WV,
		///<summary>49 - Wisconsin</summary>
		WI,
		///<summary>50 - Wyoming</summary>
		WY,
		///<summary>51 - American Samoa</summary>
		AS,
		///<summary>52 - Federated States of Micronesia</summary>
		FM,
		///<summary>53 - Guam</summary>
		GU,
		///<summary>54 - Marshall Islands</summary>
		MH,
		///<summary>55 - Northern Mariana Islands</summary>
		MP,
		///<summary>56 - Palau</summary>
		PW,
		///<summary>57 - Puerto Rico</summary>
		PR,
		///<summary>58 - U.S. Minor Outlying Islands</summary>
		UM,
		///<summary>59 - Virgin Islands of the U.S.</summary>
		VI
	}
}