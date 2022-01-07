using System;

namespace OpenDentBusiness {
	///<summary>Medical lab result.  The EHRLabResult table is structured too tightly with the HL7 standard and should have names that more reflect how 
	///the user will consume the data and for that reason for actual implementation we are using these medlab tables.
	///This table is currently only used for LabCorp, but may be utilized by other third party lab
	///services in the future.  These fields are required for the LabCorp result report, used to link the result to an order,
	///or for linking a parent and child result.  Contains data from the OBX, ZEF, and applicable NTE segments.</summary>
	[Serializable]
	public class MedLabResult:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MedLabResultNum;
		///<summary>FK to medlab.medLabNum.  Each MedLab object can have one or more results pointing to it.</summary>
		public long MedLabNum;
		#region OBX Fields
		///<summary>OBX.3.1 - Observation Identifier.  Reflex results will have the ObsID of the parent in OBR.26 for linking.</summary>
		public string ObsID;
		///<summary>OBX.3.2 - Observation Text.  LabCorp report field "TESTS".  LabCorp test name.</summary>
		public string ObsText;
		///<summary>OBX.3.4 - Alternate Identifier (LOINC).  This is the LOINC code for the observation.
		///When displaying the results, LabCorp requires OBX.3.2, the text name of the test to be displayed, not the LOINC code.
		///But we will store it so we can link to the LOINC code table for reporting purposes.</summary>
		public string ObsLoinc;
		///<summary>OBX.3.5 - Alternate Observation Text (LOINC Description).  The LOINC code description for the observation.
		///We will display OBX.3.2 per LabCorp requirements, but we will store this description for reporting purposes.</summary>
		public string ObsLoincText;
		///<summary>OBX.4 - Observation Sub ID.  Used to aid in the identification of results with the same Observation ID (OBX.3) within a given OBR.
		///This value is used to tie the results to the same organism.  The value in OBX.5.3 tells whether this OBX is the organism, observation, or
		///antibiotic and then the value in OBX.4 links them together as to whether this is for organism #1, organism #2, etc.</summary>
		public string ObsIDSub;
		///<summary>OBX.5.1 - Observation Value.  LabCorp report field "RESULT".
		///Can be null if coded entries, prelims, canceled, or >21 chars and being returned as an attached NTE.
		///"TNP" will be reported for Test Not Performed.  For value >21 chars in length: OBX.2 will be 'TX' for text,
		///OBX.5 will be NULL (empty field), and the value will be in attached NTEs.
		///Examples: Value less than 21 chars:
		///OBX|1|ST|001180^Potassium, Serum^L||K+ is >6.5 mEq/L.||3.5-5.5|A||N|F|19830527||200605040929|01|
		///Value >21 chars:
		///OBX|6|TX|001180^Potassium, Serum^L||||3.5-5.5|||N|C|19830527||200511071406|01|
		///NTE|1|L|Red cells observed in serum. Glucose may be falsely decreased.
		///NTE|2|L|Potassium may be falsely increased.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ObsValue;
		///<summary>Enum:DataSubtype OBX.5.3 - Data Subtype.  Used to identify the coding system. Required if Discrete Microbiology testing is ordered to identify
		///Microbiology Result Type.  Example of use: If OBX.5.3 is ORM, then the observation sub ID in OBX.4 is used to associate the result with
		///a specific organism.  OBX.4 might contain 1, 2, or 3 meaning the result is for organism #1, organism #2, or organism #3.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public DataSubtype ObsSubType;
		///<summary>OBX.6.1 - Identifier.  LabCorp report field "UNITS".  Units of measure, if too large it will be in the NTE segment.</summary>
		public string ObsUnits;
		///<summary>OBX.7 - Reference Ranges.  LabCorp report field "REFERENCE INTERVAL".  Only if applicable.</summary>
		public string ReferenceRange;
		///<summary>Enum:AbnormalFlag OBX.8 - Abnormal Flags.  LabCorp report field "FLAG".  Blank or null is normal.  When this is displayed on the LabCorp report
		///it must be the human readable display name, so for example _gt (>) is displayed as "Panic High" and _lt (&lt;) is "Panic Low".</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public AbnormalFlag AbnormalFlag;
		///<summary>Enum:ResultStatus OBX.11 - Observation Result Status.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public ResultStatus ResultStatus;
		///<summary>OBX.14 - Date/Time of Observation.  yyyyMMddHHmm format in the message, no seconds.
		///Date and time tech entered result into the Lab System.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeObs;
		///<summary>OBX.15 - Producer ID (Producer’s Reference).  LabCorp report field "LAB".  ID of LabCorp Facility responsible for performing the
		///testing.  The Lab Name is supplied in the ZPS segment.</summary>
		public string FacilityID;
		#endregion OBX Fields
		///<summary>FK to document.DocNum.  ZEF.2 - Embedded File.  Each result may have one or more ZEF segments for embedded files.
		///The base-64 text version of the PDF is sent in ZEF.2.  If the file size exceeds 50k, then multiple segments will be sent with 50k blocks
		///of the text.  When processing, we will concatenate all ZEF.2 fields, create the PDF document, store the file in the patient's image folder,
		///and create an entry in the document table.  Then update this field with the pointer to the document table entry.</summary>
		public long DocNum;
		///<summary>NTE.3 at the OBX level.  The NTE segment is repeatable and the Comment Text component is limited to 78 characters.  Multiple NTE
		///segments can be used for longer comments.  All NTE segments at the OBX level will be concatenated and stored in this one field.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;

		///<summary></summary>
		public MedLabResult Copy() {
			return (MedLabResult)MemberwiseClone();
		}

	}

	///<summary>MedLab Abnormal Flags.  Similar to EhrLabResult.HL70078 abnormal flag enum.</summary>
	public enum AbnormalFlag {
		///<summary>0 - None.  Blank or null value indicates normal result, so no abnormal flag.</summary>
		None,
		///<summary>1 - Panic High.  Actual value is ">" but symbol cannot be used as an enum value.</summary>
		_gt,
		///<summary>2 - Panic Low.  Actual value is "&lt;" but symbol cannot be used as an enum value.</summary>
		_lt,
		///<summary>3 - Abnormal.  Applies to non-numeric results.</summary>
		A,
		///<summary>4 - Critical Abnormal.  Applies to non-numeric results.</summary>
		AA,
		///<summary>5 - Above High Normal.</summary>
		H,
		///<summary>6 - Alert High.</summary>
		HH,
		///<summary>7 - Intermediate.  For Discrete Microbiology susceptibilities only.</summary>
		I,
		///<summary>8 - Below Low Normal.</summary>
		L,
		///<summary>9 - Alert Low.</summary>
		LL,
		///<summary>10 - Negative for Drug Interpretation Codes and Discrete Microbiology.</summary>
		NEG,
		///<summary>11 - Positive for Drug Interpretation Codes and Discrete Microbiology.</summary>
		POS,
		///<summary>12 - Resistant.  For Discrete Microbiology susceptibilities only.</summary>
		R,
		///<summary>13 - Susceptible.  For Discrete Microbiology susceptibilities only.</summary>
		S
	}

	///<summary>Used to identify the coding system. Required if Discrete Microbiology testing is ordered to identify Microbiology Result Type.
	///Example of use: If OBX.5.3 is ORM, then the observation sub ID in OBX.4 is used to associate the result with a specific organism.
	///OBX.4 might contain 1, 2, or 3 meaning the result is for organism #1, organism #2, or organism #3.</summary>
	public enum DataSubtype {
		///<summary>This idicates that we are unable to parse the value from the HL7 message into a data subtype.</summary>
		Unknown,
		///<summary>Antibody (for Discrete Microbiology only)</summary>
		ANT,
		///<summary>Organism identifier (for Discrete Microbiology only)</summary>
		ORM,
		///<summary>Presumptive organism identifier (for Discrete Microbiology only)</summary>
		ORP,
		///<summary>Observation (for Discrete Microbiology only)</summary>
		OBS,
		///<summary>Modifier (for Discrete Microbiology only)</summary>
		MOD,
		///<summary>Local Identifier (default when no Microbiology Result Text)</summary>
		L,
		///<summary>Embedded PDF result type or separate PDF file</summary>
		PDF,
		///<summary>Embedded TIF result type or a separate TIF file</summary>
		TIF
	}

}