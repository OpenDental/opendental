using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Logical Observation Identifiers Names and Codes (LOINC) used to identify both lab panels and lab results. Widths specified are from LOINC documentation and may not represent length of fields in the Open Dental Database.</summary>
	[Serializable()]
	public class Loinc:TableBase{
		///<summary>Primary key. Internal use only.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LoincNum;
		///<summary>#EULA REQUIRED# Also called LOINC_NUM in the official LOINCDB. Width-10. LOINC244 column 1.</summary>
		public string LoincCode;//LOINC_NUM;
		///<summary>#EULA REQUIRED# First Major axis:component or analyte. Width-255. LOINC244 column 2.</summary>
		public string Component;
		///<summary>#EULA REQUIRED# Second major axis:property observed (e.g., mass vs. substance). Width-30. LOINC244 column 3.</summary>
		public string PropertyObserved;//Property;
		///<summary>#EULA REQUIRED# Third major axis:timing of the measurement (e.g., point in time vs 24 hours). Width-15. LOINC244 column 4.</summary>
		public string TimeAspct;//Time_Aspct;
		///<summary>#EULA REQUIRED# Fourth major axis:type of specimen or system (e.g., serum vs urine). Width-100 LOINC244. column 5.</summary>
		public string SystemMeasured;//System;
		///<summary>#EULA REQUIRED# Fifth major axis:scale of measurement (e.g., qualitative vs. quantitative). Width-30. LOINC244 column 6.</summary>
		public string ScaleType;//Scale_Typ;
		///<summary>#EULA REQUIRED# Sixth major axis:method of measurement. Width-50. LOINC244 column 7.</summary>
		public string MethodType;//Method_Typ;
		///<summary><para>#EULA REQUIRED# Width-10. LOINC244 column 13.</para>
		///<para>ACTIVE = Concept is active. Use at will.</para>
		///<para>TRIAL = Concept is experimental in nature. Use with caution as the concept and associated attributes may change. </para>
		///<para>DISCOURAGED = Concept is not recommended for current use. New mappings to this concept are discouraged; although existing may mappings may continue to be valid in context. Wherever  possible, the superseding concept is indicated in the MAP_TO field in the MAP_TO table (see Table 28b) and should be used instead. </para>
		///<para>DEPRECATED = Concept is deprecated. Concept should not be used, but it is retained in LOINC for historical purposes. Wherever possible, the superseding concept is indicated in the MAP_TO field (see Table 28b) and should be used both for new mappings and updating existing implementations..</para>
		///</summary>
		public string StatusOfCode;//Status;
		///<summary>#EULA REQUIRED# Introduced in version 2.07, this field is a concatenation of the fully specified LOINC name. The field width may change in a future release. Width 40. LOINC244 column 29.</summary>
		public string NameShort;//ShortName;
		///<summary>1=Laboratory class; 2=Clinical class; 3=Claims attachments; 4=Surveys. LOINC244 column 16.</summary>
		public string ClassType;
		///<summary>Y/N field that indicates that units are required when this LOINC is included as an OBX segment in a HIPAA attachment. LOINC244 column 26.</summary>
		public bool UnitsRequired;
		///<summary>Defines term as order only, observation only, or both. A fourth category, Subset, is used for terms that are subsets of a panel but do not represent a package that is known to be orderable we have defined them only to make it easier to maintain panels or other sets within the LOINC construct. LOINC244 column 30.</summary>
		public string OrderObs;
		///<summary>A value in this field means that the content should be delivered in the named field/subfield of the HL7 message. When NULL, the data for this data element should be sent in an OBX segment with this LOINC code stored in OBX-3 and with the value in the OBX-5. Width 50. LOINC244 column 32.</summary>
		public string HL7FieldSubfieldID;//HL7_Field_Subfield_ID;
		///<summary>External copyright holders copyright notice for this LOINC code. LOINC244 column 33.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ExternalCopyrightNotice;//External_Copyright_Notice;
		///<summary>This field contains the LOINC term in a more readable format than the fully specified name. The long common names have been created via a table driven algorithmic process. Most abbreviations and acronyms that are used in the LOINC database have been fully spelled out in English. Width 255. LOINC244 column 35.</summary>
		public string NameLongCommon;//LONG_COMMON_NAME;
		///<summary>The Unified Code for Units of Measure (UCUM) is a code system intended to include all units of measures being contemporarily used in international science, engineering, and business. (www.unitsofmeasure.org ) This field contains example units of measures for this term expressed as UCUM units. Width 255. LOINC244 column 1.</summary>
		public string UnitsUCUM;//Example_UCUM_Units;
		///<summary>Ranking of approximately 2000 common tests performed by laboratories in USA. LOINC244 column 45.</summary>
		public int RankCommonTests;//Common_Test_Rank;
		///<summary>Ranking of approximately 300 common orders performed by laboratories in USA. LOINC244 column 46.</summary>
		public int RankCommonOrders;//Common_Order_Rank;
		
		///<summary></summary>
		public Loinc Clone() {
			return (Loinc)this.MemberwiseClone();
		}

	}

	
}




