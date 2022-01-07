using EhrLaboratories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	///<summary>For EHR module, lab request that contains all required fields for HL7 Lab Reporting Interface (LRI).</summary>
	[Serializable]
	public class EhrLab:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrLabNum;
		///<summary>FK to patient.PatNum.  PID-3.1</summary>
		public long PatNum;
		/////<summary>FK to EhrLabMessage.EhrLabMessageNum.  Internal use.</summary>
		//public long EhrLabMessageNum;
		#region ORC fields
		///<summary>Always RE unless importing from outside sources.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70119 OrderControlCode;

		#region Order Numbers
		#region Placer Order Num OCR.2 and OBR.2
		///<summary>Placer order number assigned to this lab order, usually assigned by the dental office.  Not the same as EhrLabNum, but similar.  OBR.2.1 and ORC.2.1.</summary>
		public string PlacerOrderNum;
		///<summary>Usually empty, only used if PlacerOrderNum+PlacerUniversalID cannot uniquely identify the lab order.  OBR.2.2 and ORC.2.2.</summary>
		public string PlacerOrderNamespace;
		///<summary>Usually OID root that uniquely identifies the context that makes PlacerOrderNum globally unique.  May be GUID if importing from other sources.   OBR.2.3 and ORC.2.3.</summary>
		public string PlacerOrderUniversalID;
		///<summary>Always "ISO", unless importing from other sources.  OBR.2.4 and ORC.2.4</summary>
		public string PlacerOrderUniversalIDType;
		#endregion

		#region Filler Order Num OCR.3 and OBR.3
		///<summary>Filler order number assigned to this lab order, usually assigned by the laboratory.  Not the same as EhrLabNum, but similar.  OBR.3.1 and ORC.3.1.</summary>
		public string FillerOrderNum;
		///<summary>Usually empty, only used if FillerOrderNum+FillerUniversalID cannot uniquely identify the lab order.  OBR.3.2 and ORC.3.2.</summary>
		public string FillerOrderNamespace;
		///<summary>Usually OID root that uniquely identifies the context that makes FillerOrderNum globally unique.  May be GUID if importing from other sources.  OBR.3.2 and ORC.3.3.</summary>
		public string FillerOrderUniversalID;
		///<summary>Always "ISO", unless importing from other sources.  OBR.3.4 and ORC.3.4</summary>
		public string FillerOrderUniversalIDType;
		#endregion

		#region Placer Group Num OCR.4
		///<summary>[0..1] May be empty.  Placer group number assigned to this lab order, usually assigned by the dental office.  ORC.4.1.</summary>
		public string PlacerGroupNum;
		///<summary>[0..1] Usually empty, only used if PlacerGroupNum+PlacerUniversalID cannot uniquely identify the Group Num.  ORC.4.2.</summary>
		public string PlacerGroupNamespace;
		///<summary>[0..1] Usually OID root that uniquely identifies the context that makes PlacerGroupNum globally unique.  May be GUID if importing from other sources.   ORC.4.3.</summary>
		public string PlacerGroupUniversalID;
		///<summary>[0..1] Always "ISO", unless importing from other sources.  ORC.4.4</summary>
		public string PlacerGroupUniversalIDType;
		#endregion
		#endregion

		#region Ordering Provider ORC.12 and OBR.16
		///<summary>May be provnum or NPI num or any other num, when combined with OrderingProviderIdAssigningAuthority should uniquely identify the provider.  ORC.12.1</summary>
		public string OrderingProviderID;
		///<summary>ORC.12.2</summary>
		public string OrderingProviderLName;
		///<summary>ORC.12.3</summary>
		public string OrderingProviderFName;
		///<summary>Middle names or initials therof.  ORC.12.4</summary>
		public string OrderingProviderMiddleNames;
		///<summary>Example: JR or III.  ORC.12.5</summary>
		public string OrderingProviderSuffix;
		///<summary>Example: DR, Not MD, MD would be stored in an optional field that was not implemented called OrderingProviderDegree.  ORC.12.6</summary>
		public string OrderingProviderPrefix;
		#region Ordering Provider Id Assigning Authority ORC.12.9
		///<summary>Usually empty, "The value of [this field] reflects a local code that represents the combination of [the next two fields]."  ORC.12.9.1</summary>
		public string OrderingProviderAssigningAuthorityNamespaceID;
		///<summary>ISO compliant OID that represents the organization that assigned the unique provider ID.  ORC.12.9.2</summary>
		public string OrderingProviderAssigningAuthorityUniversalID;
		///<summary>Always "ISO", unless importing from outside source.  ORC.12.9.3</summary>
		public string OrderingProviderAssigningAuthorityIDType;
		#endregion
		///<summary>Describes the type of name used.  ORC.12.10</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70200 OrderingProviderNameTypeCode;
		///<summary>Must be value from HL70203 code set, see note at bottom of EhrLab.cs for usage.  ORC.12.13</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70203 OrderingProviderIdentifierTypeCode;
		#endregion

		#endregion ORC
		#region OBR fields
		///<summary>Enumerates the OBR segments within a single message starting with 1.  OBR.1</summary>
		public long SetIdOBR;
		//OBR.PlacerOrderNumber same as above. 
		//OBR.FillerOrderNumber same as above. 

		#region Universal Service Identifier OBR.4
		///<summary>OBR.4.1</summary>
		public string UsiID;
		///<summary>Description of UsiId.  OBR.4.2</summary>
		public string UsiText;
		///<summary>CodeSystem that UsiId came from.  OBR.4.3</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string UsiCodeSystemName;
		///<summary>OBR.4.4</summary>
		public string UsiIDAlt;
		///<summary>Description of UsiIdAlt.  OBR.4.5</summary>
		public string UsiTextAlt;
		///<summary>CodeSystem that UsiId came from.  OBR.4.6</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string UsiCodeSystemNameAlt;
		///<summary>Optional text that describes the original text used to encode the values above.  OBR.4.9</summary>
		public string UsiTextOriginal;
		#endregion

		///<summary>Stored as string in the format YYYY[MM[DD[HH[MM[SS]]]]] where bracketed values are optional.  When time is not known will be valued "0000".  OBR.7.1</summary>
		public string ObservationDateTimeStart;
		///<summary>May be empty.  Stored as string in the format YYYY[MM[DD[HH[MM[SS]]]]] where bracketed values are optional.  OBR.8.1</summary>
		public string ObservationDateTimeEnd;
		///<summary>OBR.11</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70065 SpecimenActionCode;
		///<summary>[0..*]This is not a data column but is stored in a seperate table named EhrLabClinicalInfo.  OBR.13.*</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<EhrLabClinicalInfo> _listRelevantClinicalInformation;
		//OBR.OrderingProvider same as above.
		///<summary>Date Time that the result was stored or last updated.  Stored in the format YYYYMMDDHHmmss.  Required to be accurate to the second.  OBR.22.1</summary>
		public string ResultDateTime;
		///<summary>OBR.25</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70123 ResultStatus;
		#region Parent Result OBR.26
		///<summary>OBR.26.1.1</summary>
		public string ParentObservationID;
		///<summary>Description of ParentObservationId.  OBR.26.1.2</summary>
		public string ParentObservationText;
		///<summary>CodeSystem that ParentObservationId came from.  OBR.26.1.3</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string ParentObservationCodeSystemName;
		///<summary>OBR.26.1.4</summary>
		public string ParentObservationIDAlt;
		///<summary>Description of ParentObservationIdAlt.  OBR.26.1.5</summary>
		public string ParentObservationTextAlt;
		///<summary>CodeSystem that ParentObservationIdAlt came from.  OBR.26.1.6</summary>
		public string ParentObservationCodeSystemNameAlt;
		///<summary>Optional text that describes the original text used to encode the values above.  OBR.26.1.9</summary>
		public string ParentObservationTextOriginal;
		///<summary>OBR.26.2</summary>
		public string ParentObservationSubID;
		#endregion
		///<summary>[0..*]This is not a data column but is stored in a seperate table named EhrLabResultsCopyTo. OBR.28.*</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<EhrLabResultsCopyTo> _listEhrLabResultsCopyTo;
		#region Parent Document/Order OBR.29
		#region Parent Placer Order Num OBR.29.1
		///<summary>Placer order number assigned to this lab order, usually assigned by the dental office.  Not the same as EhrLabNum, but similar.  OBR.29.1.1.</summary>
		public string ParentPlacerOrderNum;
		///<summary>Usually empty, only used if PlacerOrderNum+PlacerUniversalID cannot uniquely identify the lab order.  OBR.29.1.2</summary>
		public string ParentPlacerOrderNamespace;
		///<summary>Usually OID root that uniquely identifies the context that makes PlacerOrderNum globally unique.  May be GUID if importing from other sources.   OBR.29.1.3</summary>
		public string ParentPlacerOrderUniversalID;
		///<summary>Always "ISO", unless importing from other sources.  OBR.29.1.4</summary>
		public string ParentPlacerOrderUniversalIDType;
		#endregion
		#region Parent Filler Order Num OBR.29.2
		///<summary>Filler order number assigned to this lab order, usually assigned by the laboratory.  Not the same as EhrLabNum, but similar.  OBR.29.2.1</summary>
		public string ParentFillerOrderNum;
		///<summary>Usually empty, only used if FillerOrderNum+FillerUniversalID cannot uniquely identify the lab order.  OBR.29.2.2</summary>
		public string ParentFillerOrderNamespace;
		///<summary>Usually OID root that uniquely identifies the context that makes FillerOrderNum globally unique.  May be GUID if importing from other sources.  OBR.29.2.3</summary>
		public string ParentFillerOrderUniversalID;
		///<summary>Always "ISO", unless importing from other sources.  OBR.29.2.4</summary>
		public string ParentFillerOrderUniversalIDType;
		#endregion
		#endregion
		///<summary>"Film with patient."  Technically a coded value from HL70507.  Stored as a bool instead of 7 seperate columns. OBR.49.* is used to set both ListEhrLabResultsHandlingF and ListEhrLabResultsHandlingN.  OBR.49.*</summary>
		public bool ListEhrLabResultsHandlingF;
		///<summary>"Notify provider when ready."  Technically a coded value from HL70507.  Stored as a bool instead of 7 seperate columns. OBR.49.* is used to set both ListEhrLabResultsHandlingF and ListEhrLabResultsHandlingN.  OBR.49.*</summary>
		public bool ListEhrLabResultsHandlingN;
		#endregion OBR
		#region NTE segments pertaining to OBR;  NTE.*
		///<summary>[0..*]This is not a data column but is stored in a seperate table named EhrLabNote. NTE.*</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<EhrLabNote> _listEhrLabNotes;
		#endregion
		#region TQ1 fields.
		///<summary>Enumerates the TQ1 segments within a single message starting with 1.  TQ1.1</summary>
		public long TQ1SetId;
		///<summary>Stored as string in the format YYYY[MM[DD[HH[MM[SS]]]]] where bracketed values are optional.  TQ1.7</summary>
		public string TQ1DateTimeStart;
		///<summary>Stored as string in the format YYYY[MM[DD[HH[MM[SS]]]]] where bracketed values are optional.  TQ1.8</summary>
		public string TQ1DateTimeEnd;
		#endregion
		///<summary>This gets set when a provider is logged in with a valid EHR key and then creates a lab.</summary>
		public bool IsCpoe;
		///<summary>The PID Segment from the HL7 message used to generate or update the lab order.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string OriginalPIDSegment;
		///<summary>[0..*] This is not a data column but is stored in a seperate table named EhrLabResult. OBX.*</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<EhrLabResult> _listEhrLabResults;
		///<summary>[0..*] This is not a data column but is stored in a seperate table named EhrLabSpecimen. SPM.*</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<EhrLabSpecimen> _listEhrLabSpecimen;

		///<summary></summary>
		public EhrLab Copy() {
			return (EhrLab)MemberwiseClone();
		}

		///<summary>Only filled with EhrLabNotes when value is used.  To refresh ListEhrLabResults, set it equal to null or explicitly reassign it using EhrLabResults.GetForLab(EhrLabNum).</summary>
		public List<EhrLabNote> ListEhrLabNotes {
			get {
				if(_listEhrLabNotes==null) {
					if(EhrLabNum==0) {
						_listEhrLabNotes=new List<EhrLabNote>();
					}
					else {
						_listEhrLabNotes=EhrLabNotes.GetForLab(EhrLabNum);
					}
				}
				return _listEhrLabNotes;
			}
			set {
				_listEhrLabNotes=value;
			}
		}

		///<summary>Only filled with EhrLabResults when value is used.  To refresh ListEhrLabResults, set it equal to null or explicitly reassign it using EhrLabResults.GetForLab(EhrLabNum).</summary>
		public List<EhrLabResult> ListEhrLabResults {
			get {
				if(_listEhrLabResults==null) {
					if(EhrLabNum==0) {
						_listEhrLabResults=new List<EhrLabResult>();
					}
					else {
						_listEhrLabResults=EhrLabResults.GetForLab(EhrLabNum);
					}
				}
				return _listEhrLabResults;
			}
			set {
				_listEhrLabResults=value;
			}
		}

		///<summary>Only filled with EhrLabSpecimens when value is used.  To refresh ListEhrLabSpecimens, set it equal to null or explicitly reassign it using EhrLabSpecimens.GetForLab(EhrLabNum).</summary>
		public List<EhrLabSpecimen> ListEhrLabSpecimens {
			get {
				if(_listEhrLabSpecimen==null) {
					if(EhrLabNum==0) {
						_listEhrLabSpecimen=new List<EhrLabSpecimen>();
					}
					else {
						_listEhrLabSpecimen=EhrLabSpecimens.GetForLab(EhrLabNum);
					}
				}
				return _listEhrLabSpecimen;
			}
			set {
				_listEhrLabSpecimen=value;
			}
		}

		public List<EhrLabResultsCopyTo> ListEhrLabResultsCopyTo {
			get {
				if(_listEhrLabResultsCopyTo==null) {
					if(EhrLabNum==0) {
						_listEhrLabResultsCopyTo=new List<EhrLabResultsCopyTo>();
					}
					else {
						_listEhrLabResultsCopyTo=EhrLabResultsCopyTos.GetForLab(EhrLabNum);
					}
				}
				return _listEhrLabResultsCopyTo;
			}
			set {
				_listEhrLabResultsCopyTo=value;
			}
		}

		public List<EhrLabClinicalInfo> ListRelevantClinicalInformations{
			get {
				if(_listRelevantClinicalInformation==null) {
					if(EhrLabNum==0) {
						_listRelevantClinicalInformation=new List<EhrLabClinicalInfo>();
					}
					else {
						_listRelevantClinicalInformation=EhrLabClinicalInfos.GetForLab(EhrLabNum);
					}
				}
				return _listRelevantClinicalInformation;
			}
			set {
				_listRelevantClinicalInformation=value;
			}
		}

		public static string formatDateFromHL7(string hl7dt) {//hl7date time yyyyMMDDhhmmssssss-zzzz
			if(hl7dt==null || hl7dt=="") {
				return "";
			}
			if(!Regex.IsMatch(hl7dt,@"^\d{4}(\d\d(\d\d(\d\d(\d\d(\d\d(\.\d(\d(\d(\d)?)?)?)?)?)?)?)?)?([\+-]\d{4})?$")) {
			//                         yyyy   MM   dd   hh   mm   ss   .s  s  s  s                     +/- zzzz
				return hl7dt;//does not conform. Return whatever garbage was input.
			}
			string retVal="";
			string zone="";
			if(hl7dt.Contains("+") || hl7dt.Contains("-")) {//value contains a time zone.
				zone=hl7dt.Substring(hl7dt.Length-5);//sign plus 4 digits
				hl7dt=hl7dt.Replace(zone,"");
			}
			if(hl7dt.Length>3)  {retVal+=    hl7dt.Substring( 0,4);}//yyyy
			if(hl7dt.Length>5)  {retVal+="-"+hl7dt.Substring( 4,2);}//MM				
			if(hl7dt.Length>7)  {retVal+="-"+hl7dt.Substring( 6,2);}//dd				
			if(hl7dt.Length>9)  {retVal+=" "+hl7dt.Substring( 8,2);}//hh				
			if(hl7dt.Length>11) {retVal+=":"+hl7dt.Substring(10,2);}//mm				
			if(hl7dt.Length>13) {retVal+=":"+hl7dt.Substring(12  );}//ss.ssss	
			return retVal+" "+zone;
		}

		public static string formatDateToHL7(string hrdt) {//human readable date time
			hrdt=hrdt.Trim();
			if(!Regex.IsMatch(hrdt,@"^\d{4}(-\d\d(-\d\d(\s\d\d(:\d\d(:\d\d(\.\d(\d(\d(\d)?)?)?)?)?)?)?)?)?(\s[\+-]\d{4})?$")) {
				//                      yyyy -  MM -  dd    hh :  mm :  ss   .s  s  s  s                       +/- zzzz
				return hrdt;//does not conform. Return whatever garbage was input.
			}
			string zone="";
			if(hrdt[hrdt.Length-5]=='+' || hrdt[hrdt.Length-5]=='-') {//value contains a time zone.
				zone=hrdt.Substring(hrdt.Length-5);//sign plus 4 digits
				hrdt=hrdt.Replace(zone,"");
			}
			return hrdt.Replace("-","").Replace(" ","").Replace(":","")+zone;
		}

	}

}

namespace EhrLaboratories {

	///<summary>Specimen Action Code. Constrained to AGLO. OID:2.16.840.1.113883.12.119  HL70369 code:HL70119.  Source HL7 2.5.1 Labratory Reporting Interface documentation.</summary>
	public enum HL70065 {
		///<summary>0 - Add ordered tests to the existing specimen.</summary>
		A,
		///<summary>1 - Generated order; reflex order.</summary>
		G,
		///<summary>2 - Lab to obtain specimen from patient.</summary>
		L,
		///<summary>3 - Specimen obtained by service other than lab.</summary>
		O
	}

	///<summary>Order Control Code.  We only use RE.  OID:2.16.840.1.113883.12.119  HL70369 code:HL70119.  Source phinvads.cdc.gov</summary>
	public enum HL70119 {
		///<summary>0 - Cancel order/service request</summary>
		CA,
		///<summary>1 - Canceled as requested</summary>
		CR,
		///<summary>2 - Change order/service request</summary>
		XO,
		///<summary>3 - Changed as requested</summary>
		XR,
		///<summary>4 - Child order/service</summary>
		CH,
		///<summary>5 - Combined result</summary>
		CN,
		///<summary>6 - Data errors</summary>
		DE,
		///<summary>7 - Discontinue order/service request</summary>
		DC,
		///<summary>8 - Discontinued as requested</summary>
		DR,
		///<summary>9 - Hold order request</summary>
		HD,
		///<summary>10 - Link order/service to patient care problem or goal</summary>
		LI,
		///<summary>11 - New order/service</summary>
		NW,
		///<summary>12 - Notification of order for outside dispense</summary>
		OP,
		///<summary>13 - Notification of replacement order for outside dispense</summary>
		PY,
		///<summary>14 - Number assigned</summary>
		NA,
		///<summary>15 - Observations/Performed Service to follow</summary>
		RE,
		///<summary>16 - On hold as requested</summary>
		HR,
		///<summary>17 - Order/service accepted &amp; OK</summary>
		OK,
		///<summary>18 - Order/service canceled</summary>
		OC,
		///<summary>19 - Order/service changed, unsol.</summary>
		XX,
		///<summary>20 - Order/service discontinued</summary>
		OD,
		///<summary>21 - Order/service held</summary>
		OH,
		///<summary>22 - Order/service refill request approval</summary>
		AF,
		///<summary>23 - Order/service refill request denied</summary>
		DF,
		///<summary>24 - Order/service refilled as requested</summary>
		OF,
		///<summary>25 - Order/service refilled, unsolicited</summary>
		FU,
		///<summary>26 - Order/service released</summary>
		OE,
		///<summary>27 - Order/service replace request</summary>
		RP,
		///<summary>28 - Parent order/service</summary>
		PA,
		///<summary>29 - Previous Results with new order/service</summary>
		PR,
		///<summary>30 - Refill order/service request</summary>
		RF,
		///<summary>31 - Release previous hold</summary>
		RL,
		///<summary>32 - Released as requested</summary>
		OR,
		///<summary>33 - Replaced as requested</summary>
		RQ,
		///<summary>34 - Replaced unsolicited</summary>
		RU,
		///<summary>35 - Replacement order</summary>
		RO,
		///<summary>36 - Request received</summary>
		RR,
		///<summary>37 - Response to send order/service status request</summary>
		SR,
		///<summary>38 - Send order/service number</summary>
		SN,
		///<summary>39 - Send order/service status request</summary>
		SS,
		///<summary>40 - Status changed</summary>
		SC,
		///<summary>41 - Unable to accept order/service</summary>
		UA,
		///<summary>42 - Unable to cancel</summary>
		UC,
		///<summary>43 - Unable to change</summary>
		UX,
		///<summary>44 - Unable to discontinue</summary>
		UD,
		///<summary>45 - Unable to put on hold</summary>
		UH,
		///<summary>46 - Unable to refill</summary>
		UF,
		///<summary>46 - Unable to release</summary>
		UR,
		///<summary>47 - Unable to replace</summary>
		UM,
		///<summary>48 - Unlink order/service from patient care problem or goal</summary>
		UN
	}

	///<summary>Result Status.  OID:2.16.840.1.113883.12.123  HL70369 code:HL70123.  Source HL7 2.5.1 Labratory Reporting Interface documentation.</summary>
	public enum HL70123 {
		///<summary>0 - Some but not all results available.</summary>
		A,
		///<summary>1 - Correction to results.</summary>
		C,
		///<summary>2 - Final Results; results stored and verified. Can only be changed with a corrected result.</summary>
		F,
		///<summary>3 - No results available; specimen received, procedure incomplete.</summary>
		I,
		///<summary>4 - Order received; specimen not yet received.</summary>
		O,
		///<summary>5 - Preliminary: A verified early result is available, final results not yet obtained.</summary>
		P,
		///<summary>6 - Results stored; not yet verified.</summary>
		R,
		///<summary>7 - No results available; procedure scheduled but not done.</summary>
		S,
		///<summary>8 - No results available; Order canceled.</summary>
		X
	}

	///<summary>Name Type Code.  OID:2.16.840.1.113883.12.200  HL70369 code:HL70200.  Source phinvads.cdc.gov</summary>
	public enum HL70200 {
		///<summary>0 - Adopted Name</summary>
		C,
		///<summary>1 - Alias Name</summary>
		A,
		///<summary>2 - Coded Pseudo-Name to ensure anonymity</summary>
		S,
		///<summary>3 - Display Name</summary>
		D,
		///<summary>4 - Indigenous/Tribal/Community Name</summary>
		T,
		///<summary>5 - Legal Name</summary>
		L,
		///<summary>6 - Licensing Name</summary>
		I,
		///<summary>7 - Maiden Name</summary>
		M,
		///<summary>8 - Name at Birth</summary>
		B,
		///<summary>9 - Name of Partner/Spouse (retained for backward compatibility only)</summary>
		P,
		///<summary>10 - Nickname /_Call me_ Name/Street Name</summary>
		N,
		///<summary>11 - Registered Name (animals only)</summary>
		R,
		///<summary>12 - Unspecified</summary>
		U
	}

	///<summary>Identifier Type.  OID:2.16.840.1.113883.12.203  HL70369 code:HL70203.  Source phinvads.cdc.gov</summary>
	public enum HL70203 {
		///<summary>0 - Account number</summary>
		AN,
		///<summary>1 - Account number Creditor</summary>
		ANC,
		///<summary>2 - Account number debitor</summary>
		AND,
		///<summary>3 - Advanced Practice Registered Nurse number</summary>
		APRN,
		///<summary>4 - American Express</summary>
		AM,
		///<summary>5 - Anonymous identifier</summary>
		ANON,
		///<summary>6 - Bank Account Number</summary>
		BA,
		///<summary>7 - Bank Card Number</summary>
		BC,
		///<summary>8 - Birth registry number</summary>
		BR,
		///<summary>9 - Breed Registry Number</summary>
		BRN,
		///<summary>10 - Cost Center number</summary>
		CC,
		///<summary>11 - County number</summary>
		CY,
		///<summary>12 - Dentist license number</summary>
		DDS,
		///<summary>13 - Diner_s Club card</summary>
		DI,
		///<summary>14 - Discover Card</summary>
		DS,
		///<summary>15 - Doctor number</summary>
		DN,
		///<summary>16 - Donor Registration Number</summary>
		DR,
		///<summary>17 - Driver_s license number</summary>
		DL,
		///<summary>18 - Drug Enforcement Administration registration number</summary>
		DEA,
		///<summary>19 - Drug Furnishing or prescriptive authority Number</summary>
		DFN,
		///<summary>20 - Employee number</summary>
		EI,
		///<summary>21 - Employer number</summary>
		EN,
		///<summary>22 - Facility ID</summary>
		FI,
		///<summary>23 - General ledger number</summary>
		GL,
		///<summary>24 - Guarantor external identifier</summary>
		GN,
		///<summary>25 - Guarantor internal identifier</summary>
		GI,
		///<summary>26 - Health Card Number</summary>
		HC,
		///<summary>27 - Indigenous/Aboriginal</summary>
		IND,
		///<summary>28 - Jurisdictional health number (Canada)</summary>
		JHN,
		///<summary>29 - Labor and industries number</summary>
		LI,
		///<summary>30 - License number</summary>
		LN,
		///<summary>31 - Living Subject Enterprise Number</summary>
		PE,
		///<summary>32 - Local Registry ID</summary>
		LR,
		///<summary>33 - MasterCard</summary>
		MS,
		///<summary>34 - Medical License number</summary>
		MD,
		///<summary>35 - Medical record number</summary>
		MR,
		///<summary>36 - Medicare/CMS (formerly HCFA)_s Universal Physician Identification numbers</summary>
		UPIN,
		///<summary>37 - Member Number</summary>
		MB,
		///<summary>38 - Microchip Number</summary>
		MCN,
		///<summary>39 - Military ID number</summary>
		MI,
		///<summary>40 - National employer identifier</summary>
		NE,
		///<summary>41 - National Health Plan Identifier</summary>
		NH,
		///<summary>42 - National Insurance Organization Identifier</summary>
		NII,
		///<summary>43 - National Insurance Payor Identifier (Payor)</summary>
		NIIP,
		///<summary>44 - National Person Identifier where the xxx is the ISO table 3166 3-character (alphabetic) country code</summary>
		NNxxx,
		///<summary>45 - National provider identifier</summary>
		NPI,
		///<summary>46 - National unique individual identifier</summary>
		NI,
		///<summary>47 - Nurse practitioner number</summary>
		NP,
		///<summary>48 - Optometrist license number</summary>
		OD,
		///<summary>49 - Organization identifier</summary>
		XX,
		///<summary>50 - Osteopathic License number</summary>
		DO,
		///<summary>51 - Passport number</summary>
		PPN,
		///<summary>52 - Patient external identifier</summary>
		PT,
		///<summary>53 - Patient internal identifier</summary>
		PI,
		///<summary>54 - Patient Medicaid number</summary>
		MA,
		///<summary>55 - Patient's Medicare number</summary>
		MC,
		///<summary>56 - Penitentiary/correctional institution Number</summary>
		PCN,
		///<summary>57 - Pension Number</summary>
		PEN,
		///<summary>58 - Permanent Resident Card Number</summary>
		PRC,
		///<summary>59 - Person number</summary>
		PN,
		///<summary>60 - Pharmacist license number</summary>
		RPH,
		///<summary>61 - Physician Assistant number</summary>
		PA,
		///<summary>62 - Podiatrist license number</summary>
		DPM,
		///<summary>63 - Practitioner Medicaid number</summary>
		MCD,
		///<summary>64 - Practitioner Medicare number</summary>
		MCR,
		///<summary>65 - Provider number</summary>
		PRN,
		///<summary>66 - QA number</summary>
		QA,
		///<summary>67 - Railroad Retirement number</summary>
		RR,
		///<summary>68 - Regional registry ID</summary>
		RRI,
		///<summary>69 - Registered Nurse Number</summary>
		RN,
		///<summary>70 - Resource identifier</summary>
		RI,
		///<summary>71 - Social Security number</summary>
		SS,
		///<summary>72 - Specimen Identifier</summary>
		SID,
		///<summary>73 - State license</summary>
		SL,
		///<summary>74 - State registry ID</summary>
		SR,
		///<summary>75 - Subscriber Number</summary>
		SN,
		///<summary>76 - Tax ID number</summary>
		TAX,
		///<summary>77 - Temporary Account Number</summary>
		ANT,
		///<summary>78 - Temporary Living Subject Number</summary>
		PNT,
		///<summary>79 - Temporary Medical Record Number</summary>
		MRT,
		///<summary>80 - Treaty Number/ (Canada)</summary>
		TN,
		///<summary>81 - Unspecified identifier</summary>
		U,
		///<summary>82 - VISA</summary>
		VS,
		///<summary>83 - Visit number</summary>
		VN,
		///<summary>84 - WIC identifier</summary>
		WC,
		///<summary>85 - Workers_ Comp Number</summary>
		WCN
	}

	///<summary>Identifier Type.  OID:2.16.840.1.113883...  HL70369 code:HL70361.  Source phinvads.cdc.gov</summary>
	public enum HL70361 {
		//Used in ACK messages
	}

	///<summary>Identifier Type.  OID:2.16.840.1.113883...  HL70369 code:HL70362.  Source phinvads.cdc.gov</summary>
	public enum HL70362 {
		//Used in ACK messages
	}

	///<summary>Coding Systems.  OID:2.16.840.1.113883.12.369  Source phinvads.cdc.gov
	///<para>This enum is not stored directly in the DB because of variable enum values, instead it is used to fill controls to allow users to pick from, or type their own.</para></summary>
	public enum HL70369 {
		///<summary>0 - Local general code (where z is an alphanumeric character). Actual value does not contain an underscore, but enumerations cannot start with a number.
		///<para>Source:Locally defined codes for purpose of sender or receiver. Local codes can be identified by L (for backward compatibility) or 99zzz (where z is an alphanumeric character).</para>
		///<para>Category:General Codes</para>
		///<para>Status:Active</para></summary>
		_99zzz,
		///<summary>1 - Local general code (where z is an alphanumeric character)
		///<para>Source:Locally defined codes for purpose of sender or receiver. Local codes can be identified by L (for backward compatibility) or 99zzz (where z is an alphanumeric character).</para>
		///<para>Category:General Codes</para>
		///<para>Status:Active</para></summary>
		L,
		///<summary>2 - American College of Radiology finding codes
		///<para>Source:Index for Radiological Diagnosis Revised, 3rd Edition 1986, American College of Radiology, Reston, VA.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		ACR,
		///<summary>3 - Table of HL7 Version 3 ActCode values
		///<para>Source:For use in v2.x systems interoperating with V3 systems.  Identical to the code system 2.16.840.1.113883.5.4 ActCode in the Version 3 vocabulary.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		ACTCODE,
		///<summary>4 - Used to indicate that the target of the relationship will be a filtered subset of the total related set of targets. Used when there is a need to limit the number of components to the first, the last, the next, the total, the average or some other filtered or calculated subset.
		///<para>Source:V3 coding system.   Download with V3 materials.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		ACTRELSS,
		///<summary>5 - German Alpha-ID v2006
		///<para>Source:ID of the alphabetical Index ICD-10-GM-2006. Alpha-ID.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		ALPHAID2006,
		///<summary>6 - German Alpha-ID v2007
		///<para>Source:ID of the alphabetical Index ICD-10-GM-2007. Alpha-ID.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		ALPHAID2007,
		///<summary>7 - German Alpha-ID v2008
		///<para>Source:ID of the alphabetical Index ICD-10-GM-2008. Alpha-ID.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		ALPHAID2008,
		///<summary>8 - German Alpha-ID v2009
		///<para>Source:ID of the alphabetical Index ICD-10-GM-2009. Alpha-ID.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		ALPHAID2009,
		///<summary>9 - Australian Medicines Terminology (v2)
		///<para>Source:The national terminology to identify medicines used in Australia, using unique codes to deliver unambiguous, accurate and standardised names for both branded (trade) and generic (medicinal) products.</para>
		///<para>Category:Drug code</para>
		///<para>Status:New</para></summary>
		AMTv2,
		///<summary>10 - HL7 set of units of measure actual code is ANS+, but enumerations cannot contain special characters.
		///<para>Source:HL7 set of units of measure based upon ANSI X3.50 - 1986, ISO 2988-83, and US customary units / see chapter 7, section 7.4.2.6.</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		ANS_,
		///<summary>11 - WHO Adverse Reaction Terms
		///<para>Source:WHO Collaborating Centre for International Drug Monitoring, Box 26, S-751 03, Uppsala, Sweden.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		ART,
		///<summary>12 - ASTM E1238/ E1467 Universal
		///<para>Source:American Society for Testing &amp; Materials and CPT4 (see Appendix X1 of Specification E1238 and Appendix X2 of Specification E1467).</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		AS4,
		///<summary>13 - AS4 Neurophysiology Codes
		///<para>Source:ASTM’s diagnostic codes and test result coding/grading systems for clinical neurophysiology. See ASTM Specification E1467, Appendix 2.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		AS4E,
		///<summary>14 - American Type Culture Collection
		///<para>Source:Reference cultures (microorganisms, tissue cultures, etc.), related biological materials and associated data. American Type Culture Collection, 12301 Parklawn Dr, Rockville MD, 20852. (301) 881-2600.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		ATC,
		///<summary>15 - CPT-4
		///<para>Source:American Medical Association, P.O. Box 10946, Chicago IL  60610.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		C4,
		///<summary>16 - CPT-5  
		///<para>Source:Not currently being worked on, no proposed release date at this time.  American Medical Association, P.O. Box 10946, Chicago IL  60610.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Obsolete</para></summary>
		C5,
		///<summary>17 - College of American Pathologists Electronic Cancer Checklist
		///<para>Source:Each code in this system represents a single line in a database template for the College of American Pathologists Electronic Cancer Checklist (CAP eCC).  Each line and its code corresponds to either a question or an answer selection.  The code is in a decimal format of #########.#########, where each "#" is an optional number.  The nine digits to the right of the Ckey decimal point make up a namespace identifier, which is specific to the center that created the database entries for the checklist line items with their unique Ckey values.   The namespace identifier for SNOMED Terminology Solutions at the College of American Pathologists is 1000043.  All Ckey values in the 2008 release use namespace 1000043.  The digits to the left of the decimal point are a locally assigned sequential key for the ChecklistTemplateItems table in the local CAP eCC database.  These codes are used to specify questions and answers selected in a CAP eCC template for transmission in an HL7 message, as defined by the NAACCR Pathology Workgroup and the CDC/NPCR Reporting Pathology Protocols II (RPP II) project. SNOMED Terminology Solutions, College of American Pathologists, 325 Waukegan Road, Northfield, Illinois, 60093, snomedsolutions@cap.org</para>
		///<para>Category:Specific, Non-drug code</para>
		///<para>Status:New</para></summary>
		CAPECC,
		///<summary>18 - Chemical abstract codes
		///<para>Source:These include unique codes for each unique chemical, including all generic drugs. The codes do not distinguish among different dosing forms. When multiple equivalent CAS numbers exist, use the first one listed in USAN. USAN 1990 and the USP dictionary of drug names, William M. Heller, Ph.D., Executive Editor, United States Pharmacopeial Convention, Inc., 12601 Twinbrook Parkway, Rockville, MD 20852.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		CAS,
		///<summary>19 - Clinical Care Classification system
		///<para>Source:Clinical Care Classification System (formerly Home Health Care Classification system) codes. The Clinical Care Classification (CCC) consists of two terminologies: CCC of Nursing Diagnoses  and CCC of Nursing Interventions both of which are classified by 21 Care Components. Virginia Saba, EdD, RN; Georgetown University School of Nursing; Washington, DC.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		CCC,
		///<summary>20 - CDT-2 Codes
		///<para>Source:American Dental Association’s Current Dental Terminology (CDT-2) code. American Dental Association, 211 E. Chicago Avenue,. Chicago, Illinois 60611.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		CD2,
		///<summary>21 - CDC Analyte Codes
		///<para>Source:Public Health Practice Program Office, Centers for Disease Control and Prevention, 4770 Buford Highway, Atlanta, GA, 30421. Also available via FTP: ftp.cdc.gov/pub/laboratory _info/CLIA and Gopher: gopher.cdc.gov:70/11/laboratory_info/CLIA</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		CDCA,
		///<summary>22 - CDC Emergency Department Acuity
		///<para>Source:Patient Acuity indicates level of care required (Acute, Chronic, Critical)</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		CDCEDACUITY,
		///<summary>23 - CDC Methods/Instruments Codes
		///<para>Source:Public Health Practice Program Office, Centers for Disease Control and Prevention, 4770 Buford Highway, Atlanta, GA, 30421. Also available via FTP</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		CDCM,
		///<summary>24 - CDC National Healthcare Safety Network Codes
		///<para>Source:A set of patient safety and healthcare personnel safety vocabulary concepts and associated identifiers maintained as a code system by the CDC's National Healthcare Safety Network.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		CDCNHSN,
		///<summary>25 - CDC BioSense RT observations (Census) - CDC
		///<para>Source:List of BioSense RT observations (Clinical) used in OBX-3 like Temperature, Bloodpressure and Census related observations.</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		CDCOBS,
		///<summary>26 - CDC PHIN Vocabulary Coding System
		///<para>Source:CDC Public Health Information Network Vocabulary Service (PHIN VS) coding system concepts are used when the public health concepts are not available in the Standard Development Organization(SDO) Vocabulary like SNOMED CT, LOINC, ICD-9, etc.. The concepts in this coding system will be mapped to SDO Vocabulary when it is available.</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		CDCPHINVS,
		///<summary>27 - Race &amp; Ethnicity - CDC
		///<para>Source:The U.S. Centers for Disease Control and Prevention (CDC) has prepared a code set for use in coding race and ethnicity data. This code set is based on current federal standards for classifying data on race and ethnicity, specifically the minimum race and ethnicity categories defined by the U.S. Office of Management and Budget (OMB) and a more detailed set of race and ethnicity categories maintained by the U.S. Bureau of the Census (BC). The main purpose of the code set is to facilitate use of federal standards for classifying data on race and ethnicity when these data are exchanged, stored, retrieved, or analyzed in electronic form. At the same time, the code set can be applied to paper-based record systems to the extent that these systems are used to collect, maintain, and report data on race and ethnicity in accordance with current federal standards.</para>
		///<para>Category:Demographic Code</para>
		///<para>Status:New</para></summary>
		CDCREC,
		///<summary>28 - CDC Surveillance
		///<para>Source:CDC Surveillance Codes. For data unique to specific public health surveillance requirements. Epidemiology Program Office, Centers for Disease Control and Prevention, 1600 Clifton Rd, Atlanta, GA, 30333. (404) 639-3661.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		CDS,
		///<summary>29 - CEN ECG diagnostic codes (Obsolete)
		///<para>Source:CEN ECG diagnostic codes – (Obsolete, retained for backwards compatibility only.  See the entry for the MDC coding system.)</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Obsolete</para></summary>
		CE,
		///<summary>30 - CLIP
		///<para>Source:Codes for radiology reports.  Simon Leeming, Beth Israel Hospital, Boston MA.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		CLP,
		///<summary>31 - CPT Modifier Code
		///<para>Source:Available for the AMA at the address listed for CPT above. These codes are found in Appendix A of CPT 2000 Standard Edition. (CPT 2000 Standard Edition, American Medical Association, Chicago, IL).</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		CPTM,
		///<summary>32 - COSTART
		///<para>Source:International coding system for adverse drug reactions. In the USA, maintained by the FDA, Rockville, MD.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		CST,
		///<summary>33 - CDC Vaccine Codes
		///<para>Source:National Immunization Program, Centers for Disease Control and Prevention, 1660 Clifton Road, Atlanta, GA, 30333</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		CVX,
		///<summary>34 - DICOM Controlled Terminology
		///<para>Source:Codes defined in DICOM Content Mapping Resource. Digital Imaging and Communications in Medicine (DICOM). NEMA Publication PS-3.16 National Electrical Manufacturers Association (NEMA). Rosslyn, VA, 22209. Available at:</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		DCM,
		///<summary>35 - EUCLIDES
		///<para>Source:Available from Euclides Foundation International nv, Excelsiorlaan 4A, B-1930 Zaventem, Belgium; Phone: 32 2 720 90 60.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		E,
		///<summary>36 - Euclides  quantity codes
		///<para>Source:Available from Euclides Foundation International nv (see above)</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		E5,
		///<summary>37 - Euclides Lab method codes
		///<para>Source:Available from Euclides Foundation International nv, Excelsiorlaan 4A, B-1930 Zaventem, Belgium; Phone: 32 2 720 90 60.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		E6,
		///<summary>38 - Euclides Lab equipment codes
		///<para>Source:Available from Euclides Foundation International nv (see above)</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		E7,
		///<summary>39 - Education Level
		///<para>Source:For use in v2.x systems interoperating with V3 systems.  Identical to the code system 2.16.840.1.113883.5.1077 EducationLevel in the Version 3 vocabulary.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		EDLEVEL,
		///<summary>40 - Entity Code
		///<para>Source:For use in v2.x systems interoperating with V3 systems.  Identical to the code system 2.16.840.1.113883.5.1060 EntityCode in the Version 3 vocabulary.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		ENTITYCODE,
		///<summary>41 - Entity Handling Code
		///<para>Source:For use in v2.x systems interoperating with V3 systems.  Identical to the code system 2.16.840.1.113883.5.42 EntityHandling in the Version 3 vocabulary.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		ENTITYHDLG,
		///<summary>42 - Enzyme Codes
		///<para>Source:Enzyme Committee of the International Union of Biochemistry and Molecular Biology. Enzyme Nomenclature: Recommendations on the Nomenclature and Classification of Enzyme-Catalysed Reactions. London: Academic Press, 1992.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		ENZC,
		///<summary>43 - EPA SRS
		///<para>Source:Subset of EPA SRS listing chemicals that are present in ECOTOX, STORET and TRI.</para>
		///<para>Category:Non-Drug Chemical Code</para>
		///<para>Status:New</para></summary>
		EPASRS,
		///<summary>44 - Unique Ingredient Identifier (UNII)
		///<para>Source:The Unique Ingredient Identifier (UNII) generated from the FDA Substance Registration System (SRS).</para>
		///<para>Category:Drug Code</para>
		///<para>Status:New</para></summary>
		FDAUNII,
		///<summary>45 - First DataBank Drug Codes
		///<para>Source:National Drug Data File. Proprietary product of First DataBank, Inc. (800) 633-3453, or http://www.firstdatabank.com.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		FDDC,
		///<summary>46 - First DataBank Diagnostic Codes
		///<para>Source:Used for drug-diagnosis interaction checking. Proprietary product of First DataBank, Inc. As above for FDDC.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		FDDX,
		///<summary>47 - FDA K10
		///<para>Source:Dept. of Health &amp; Human Services, Food &amp; Drug Administration, Rockville, MD 20857. (device &amp; analyte process codes).</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		FDK,
		///<summary>48 - FIPS 5-2 (State)
		///<para>Source:Codes for the Identification of the States, the District of Columbia and the Outlying Areas of the United States, and Associated Areas.</para>
		///<para>Category:Demographic Code</para>
		///<para>Status:New</para></summary>
		FIPS5_2,
		///<summary>49 - FIPS 6-4 (County)
		///<para>Source:Federal Information Processing Standard (FIPS) 6-4 provides the names and codes that represent the counties and other entities treated as equivalent legal and/or statistical subdivisions of the 50 States, the District of Columbia, and the possessions and freely associated areas of the United States.</para>
		///<para>Category:Demographic Code</para>
		///<para>Status:New</para></summary>
		FIPS6_4,
		///<summary>50 - G-DRG German DRG Codes v 2004
		///<para>Source:German Handbook for DRGs. The THREE versions, "2004" , "2005" and "2006" are active</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		GDRG2004,
		///<summary>51 - G-DRG German DRG Codes v 2005
		///<para>Source:German Handbook for DRGs. The THREE versions, "2004" , "2005" and "2006" are active</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		GDRG2005,
		///<summary>52 - G-DRG German DRG Codes v 2006
		///<para>Source:German Handbook for DRGs. The THREE versions, "2004" , "2005" and "2006" are active</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		GDRG2006,
		///<summary>53 - G-DRG German DRG Codes v2007
		///<para>Source:German Handbook for DRGs 2007.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		GDRG2007,
		///<summary>54 - G-DRG German DRG Codes v2008
		///<para>Source:German Handbook for DRGs 2008.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		GDRG2008,
		///<summary>55 - G-DRG German DRG Codes v2008
		///<para>Source:German Handbook for DRGs 2009.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		GDRG2009,
		///<summary>56 - German Major Diagnostic Codes v 1004
		///<para>Source:German Major Diagnostic Codes version "2004"</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		GMDC2004,
		///<summary>57 - German Major Diagnostic Codes v2005
		///<para>Source:</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		GMDC2005,
		///<summary>58 - German Major v2006 Diagnostic Codes
		///<para>Source:</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		GMDC2006,
		///<summary>59 - German Major Diagnostic Codes v2007
		///<para>Source:German Major Diagnostic Codes 2007.</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		GMDC2007,
		///<summary>60 - German Major Diagnostic Codes v2008
		///<para>Source:German Major Diagnostic Codes v2008.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		GMDC2008,
		///<summary>61 - HIBCC
		///<para>Source:Health Industry Business Communications Council, 5110 N. 40th St., Ste 120, Phoenix, AZ 85018.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		HB,
		///<summary>62 - CMS (formerly HCFA)  Common Procedure Coding System
		///<para>Source:HCPCS: contains codes for medical equipment, injectable drugs, transportation services, and other services not found in CPT4. http://www.cms.hhs.gov/MedHCPCSGenInfo/</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		HCPCS,
		///<summary>63 - Health Care Provider Taxonomy
		///<para>Source:Formerly the responsibility of Workgroup 15 (Provider Information) within ANSI ASC X12N, all maintenance is now done by NUCC (turned over in 2001).   Primary distribution is the responsibility of Washington Publishing Company, through its World Wide Web Site http://www.wpc-edi.com.  Requests for new codes or changes may be  done through the same website.  For further information, NUCC may be contacted at: Stephanie Moncada, NUCC Secretary American Medical Association 515 N. State St. Chicago, IL 60610 Email: stephanie.moncada@ama-assn.org</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Updated</para></summary>
		HCPT,
		///<summary>64 - Home Health Care
		///<para>Source:Home Health Care Classification System; Virginia Saba, EdD, RN; Georgetown University School of Nursing; Washington, DC. Superceded by 'CCC' (see above); this entry is retained for backward-compatibility.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		HHC,
		///<summary>65 - Health Outcomes
		///<para>Source:Health Outcomes Institute codes for outcome variables available (with responses) from Stratis Health (formerly Foundation for Health Care Evaluation and Health Outcomes Institute), 2901 Metro Drive, Suite 400, Bloomington, MN, 55425-1525; (612) 854-3306 (voice); (612) 853-8503 (fax); dziegen@winternet.com. See examples in the Implementation Guide.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		HI,
		///<summary>66 - HL7 Defined Codes where nnnn is the HL7 table number
		///<para>Source:Health Level Seven where nnnn is the HL7 table number.   Comment pending from INM.</para>
		///<para>Category:General code</para>
		///<para>Status:Obsolete</para></summary>
		HL7nnnn,
		///<summary>67 - Japanese Nationwide Medicine Code
		///<para>Source:</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		HOT,
		///<summary>68 - CMS (formerly HCFA )Procedure Codes (HCPCS)
		///<para>Source:Health Care Financing Administration (HCFA) Common Procedure Coding System (HCPCS) including modifiers.[1]</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		HPC,
		///<summary>69 - Healthcare Service Location
		///<para>Source:A comprehensive classification of locations and settings where healthcare services are provided. This code system is based on the NHSN location code system that has been developed over a number of years through CDC's interaction with a variety of healthcare facilities and is intended to serve a variety of reporting needs where coding of healthcare service locations is required.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:New</para></summary>
		HSLOC,
		///<summary>70 - ICD-10
		///<para>Source:World Health Publications, Albany, NY.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		I10,
		///<summary>71 - International Classification of Diseases, 10th  Revision, Clinical Modification (ICD-10-CM)
		///<para>Source:ICD-10-CM is a clinical modification of the International Statistical Classification of Diseases and Related Health Problems, 10th revision (ICD-10) published by the United States for reporting diagnosis in morbidity settings. Additional information is available at: http://www.cdc.gov/nchs/icd/icd10cm.htm.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		I10C,
		///<summary>72 - ICD 10 Germany 2004
		///<para>Source:Three code sets exist I10G2004, I10G2005, I10G2006</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		I10G2004,
		///<summary>73 - ICD 10 Germany 2005
		///<para>Source:Three code sets exist I10G2004</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		I10G2005,
		///<summary>74 - ICD 10 Germany 2006
		///<para>Source:Three code sets exist I10G2004</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		I10G2006,
		///<summary>75 - International Classification of Diseases, 10th  Revision, Procedure Coding System (ICD-10-PCS)
		///<para>Source:ICD-10-PCS is a procedure classification published by the United States for classifying procedures performed in hospital inpatient health care settings. Additional information is available at: http://www.cms.gov/Medicare/Coding/ICD10/2013-ICD-10-PCS-GEMs.html.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		I10P,
		///<summary>76 - ICD9
		///<para>Source:World Health Publications, Albany, NY.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		I9,
		///<summary>77 - International Classification of Diseases, 9th  Revision, Clinical Modification (ICD-9-CM)
		///<para>Source:ICD-9-CM is a clinical modification of the World Health Organization's 9th Revision, International Classification of Diseases (ICD-9). ICD-9-CM is the official system of assigning codes to diagnoses and procedures associated with healthcare utilization in the United States. Additional information is available at: http://www.cms.hhs.gov/ICD9ProviderDiagnosticCodes/08_ICD10.asp.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		I9C,
		///<summary>78 - ICD-9CM Diagnosis codes
		///<para>Source:Indicates codes from ICD-9-CM drawn from Volumes 1 and 2, which cover diagnosis codes only.</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		I9CDX,
		///<summary>79 - ICD-9CM Procedure codes
		///<para>Source:Indicates codes from ICD-9-CM drawn from Volume 3, which covers procedure codes only.</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		I9CP,
		///<summary>80 - ISBT
		///<para>Source:Retained for backward compatibility only as of v 2.5. This code value has been superceded by the individual codes IBTnnnn (where nnnn identifies a specific table in ISBT 128).  Tables commencing with IBT are used in transfusion/transplantation and maintained by ICCBBA, PO Box 11309, San Bernardino, CA</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Obsolete</para></summary>
		IBT,
		///<summary>81 - ISBT 128 Standard transfusion/transplantation data items
		///<para>Source:ISBT 128 Standard data items used in transfusion/transplantation and maintained by ICCBBA, PO Box 11309, San Bernardino, CA</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:New</para></summary>
		IBT0001,
		///<summary>82 - ICHPPC-2
		///<para>Source:International Classification of Health Problems in Primary Care, Classification Committee of World Organization of National Colleges, Academies and Academic Associations of General Practitioners (WONCA), 3rd edition. An adaptation of ICD9 intended for use in General Medicine, Oxford University Press.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		IC2,
		///<summary>83 - ICD-10 Australian modification
		///<para>Source:</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		ICD10AM,
		///<summary>84 - ICD-10 Canada
		///<para>Source:</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		ICD10CA,
		///<summary>85 - ICD 10 Germany v2007
		///<para>Source:ICD German modification for 2007.</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		ICD10GM2007,
		///<summary>86 - ICD 10 Germany v2008
		///<para>Source:ICD German modification for 2008.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		ICD10GM2008,
		///<summary>87 - ICD 10 Germany v2009
		///<para>Source:ICD German modification for 2009.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		ICD10GM2009,
		///<summary>88 - International Classification of Diseases for Oncology
		///<para>Source:International Classification of Diseases for Oncology, 2nd Edition. World Health Organization: Geneva, Switzerland, 1990. Order from: College of American Pathologists, 325 Waukegan Road, Northfield, IL, 60093-2750. (847) 446-8800.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		ICDO,
		///<summary>89 - International Classification of Disease for Oncology Second Edition
		///<para>Source:Percy C, VanHolten V, and Muir C, editors. International Classification of Diseases for Oncology. Second Edition. Geneva: World Health Organization; 1990.  The ICD-9 neoplasm structure did not include morphology and there was a growing interest by physicians to establish a coding system for morphology.  ICD-O is used in cancer registries (and other related areas) for coding the topography (site) and morphology of a neoplasm. The topography code uses similar categories as ICD-10 for malignant neoplasms allowing greater specificity for the site of non-malignant neoplasms than in ICD-10.  The topography code consists of an alphabetic character (the letter C) followed by two numeric digits, a decimal point, and a numeric digit. The morphology code consists of a 6-digit numeric code which consists of three parts: histologic type (4-digit), behavior code (1-digit), and grading or differentiation (1-digit). ICD-O-2 is used for tumors diagnosed prior to 2001.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:New</para></summary>
		ICDO2,
		///<summary>90 - International Classification of Disease for Oncology Third Edition
		///<para>Source:Fritz A, Percy C, Jack A, Shanmugaratnam K, Sobin L, Parkin D, et al, editors. International Classification of Diseases for Oncology. Third Edition. Geneva: World Health Organization; 2000. The ICD-9 neoplasm structure did not include morphology and there was a growing interest by physicians to establish a coding system for morphology.  ICD-O is used in cancer registries (and other related areas) for coding the topography (site) and morphology of a neoplasm. The topography code uses similar categories as ICD-10 for malignant neoplasms allowing greater specificity for the site of non-malignant neoplasms than in ICD-10.  The topography code consists of an alphabetic character (the letter C) followed by two numeric digits, a decimal point, and a numeric digit. The morphology code consists of a 6-digit numeric code which consists of three parts: histologic type, behavior code, and grading or differentiation. In the third edition the morphology codes were revised, especially for leukemias and lymphomas. ICD-O-3 is used for tumors diagnosed in 2001 and later.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:New</para></summary>
		ICDO3,
		///<summary>91 - International Classification of Functioning, Disability and Health (ICF)
		///<para>Source:ICF is a classification of those characteristics of health involving functional impairments, activity limitations, or participation restrictions that are often associated with disability. The ICF classification complements the World Health Organization's (WHO) International Classification of Diseases-10th Revision (ICD), which contains information on diagnosis and health condition, but not on functional status. The ICD and ICF constitute the core classifications in the WHO Family of International Classifications (WHO-FIC). Additional information is available at: http://www.cdc.gov/nchs/icd/icf.htm.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		ICF,
		///<summary>92 - ICCS
		///<para>Source:Commission on Professional and Hospital Activities, 1968 Green Road, Ann Arbor, MI 48105.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		ICS,
		///<summary>93 - International Classification of Sleep Disorders
		///<para>Source:International Classification of Sleep Disorders Diagnostic and Coding Manual, 1990, available from American Sleep Disorders Association, 604 Second Street SW, Rochester, MN  55902</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		ICSD,
		///<summary>94 - ISO 2955.83 (units of measure) with HL7 extensions. Actual value is ISO+, but enumerations cannot contain special characters.
		///<para>Source:See chapter 7, section 7.4.2.6</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		ISO_,
		///<summary>95 - ISO 3166-1 Country Codes
		///<para>Source:International Standards Organization standard 3166 contains 3 parts.  Part 1 contains three tables for codes for countries of the world.  These are 2-character alphabetic, 3-character alphabetic, and numeric codes.</para>
		///<para>Category:Demographics</para>
		///<para>Status:New</para></summary>
		ISO3166_1,
		///<summary>96 - ISO 3166-2 Country subdivisions
		///<para>Source:International Standards Organization standard 3166 contains 3 parts.  Part 2 contains a complete breakdown into a relevant level of administrative subdivisions of all countries listed in ISO 3166-1. The code elements used consist of the alpha-2 code element from ISO 3166-1 followed by a separator and a further string of up to three alphanumeric characters e. g. DK-025 for the Danish county Roskilde.</para>
		///<para>Category:Demographics</para>
		///<para>Status:New</para></summary>
		ISO3166_2,
		///<summary>97 - ISO4217 Currency Codes
		///<para>Source:ISO's currency codes, which are based on the ISO country codes are published in the standard ISO 4217:2008 Codes for the representation of currencies and funds.  This International Standard specifies the structure for a three-letter alphabetic code and an equivalent three-digit numeric code for the representation of currencies and funds. For those currencies having minor units, it also shows the decimal relationship between such units and the currency itself.</para>
		///<para>Category:Financial</para>
		///<para>Status:New</para></summary>
		ISO4217,
		///<summary>98 - ISO 639 Language
		///<para>Source:International Standards Organization codes for the representation of names of languages.  ISO 639 provides two sets of language codes, one as a two-character code set (639-1) and another as a three-character code set (639-2) for the representation of names of languages.  ISO 639-3, Codes for the representation of names of languages - Part 3: Alpha-3 code for comprehensive coverage of languages, is a code list that aims to define three-letter identifiers for all known human languages.</para>
		///<para>Category:Demographics</para>
		///<para>Status:New</para></summary>
		ISO639,
		///<summary>99 - ISO Defined Codes where nnnn is the ISO table number. (deprecated)
		///<para>Source:International Standards Organization tables.  This has been deprecated since the ISO numbered standards are not the same as tables, and there are no "ISO table numbers".  Some standards contains tables of values, and some contain more than one table.  In the future, specific tables of values drawn from ISO standards will have explicit entries here in table 0396.  Use the specific entries for identified tables instead of this one.</para>
		///<para>Category:General code</para>
		///<para>Status:Obsolete</para></summary>
		ISOnnnn,
		///<summary>100 - Integrated Taxonomic Information System
		///<para>Source:This is a taxonomic hierarchy for living organisms.</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		ITIS,
		///<summary>101 - IUPAC/IFCC Component Codes
		///<para>Source:Codes used by IUPAC/IFF to identify the component (analyte) measured. Contact Henrik Olesen, as above for IUPP.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		IUPC,
		///<summary>102 - IUPAC/IFCC Property Codes
		///<para>Source:International Union of Pure and Applied Chemistry/International Federation of Clinical Chemistry. The Silver Book: Compendium of terminology and nomenclature of properties in clinical laboratory sciences. Oxford: Blackwell Scientific Publishers, 1995. Henrik Olesen, M.D., D.M.Sc., Chairperson, Department of Clinical Chemistry, KK76.4.2, Rigshospitalet, University Hospital of Copenhagen, DK-2200, Copenhagen. http://inet.uni-c.dk/~qukb7642/</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		IUPP,
		///<summary>103 - JLAC/JSLM, nationwide laboratory code
		///<para>Source:Source: Classification &amp;Coding for Clinical Laboratory. Japanese Society of Laboratory Medicine(JSLM, Old:Japan Society of Clinical Pathology). Version 10, 1997. A multiaxial code  including a analyte code (e.g., Rubella = 5f395), identification code (e.g., virus ab IGG=1431), a specimen code (e.g., serum =023) and a method code (e.g., ELISA = 022)</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		JC10,
		///<summary>104 - Japanese Chemistry
		///<para>Source:Clinical examination classification code. Japan Association of Clinical Pathology. Version 8, 1990. A multiaxial code  including a subject code (e.g., Rubella = 5f395, identification code (e.g., virus ab IGG), a specimen code (e.g., serum =023) and a method code (e.g., ELISA = 022)</para>
		///<para>Category:withdrawn</para>
		///<para>Status:Active</para></summary>
		JC8,
		///<summary>105 - Japanese Image Examination Cache
		///<para>Source:</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		JJ1017,
		///<summary>106 - LanguaL
		///<para>Source:LanguaL stands for "Langua aLimentaria" or "language of food"  LanguaL is a multilingual thesaural system using facetted classification about food.  Terms reprented in PHIN VADS will be limited to the English language version.</para>
		///<para>Category:Food Code</para>
		///<para>Status:New</para></summary>
		LANGUAL,
		///<summary>107 - Local billing code
		///<para>Source:Local billing codes/names (with extensions if needed).</para>
		///<para>Category:General code</para>
		///<para>Status:Active</para></summary>
		LB,
		///<summary>108 - Logical Observation Identifier Names and Codes (LOINC®)
		///<para>Source:Logical Observation Identifiers Names and Codes (LOINC®) provides a set of universal codes and names for identifying laboratory and other clinical observations. One of the main goals of LOINC is to facilitate the exchange and pooling of results for clinical care, outcomes management, and research. LOINC was initiated by Regenstrief Institute research scientists who continue to develop it with the collaboration of the LOINC Committee. The LOINC table, LOINC codes, and LOINC panels and forms file are copyright © 1995-2011, Regenstrief Institute, Inc. and the LOINC Committee and available at no cost (http://loinc.org) under the license at http://loinc.org/terms-of-use. The laboratory portion of the LOINC database contains the usual clinical laboratory categories of chemistry, hematology, serology, microbiology (including parasitology and virology), toxicology; as well as categories for drugs and the cell counts, antibiotic susceptibilities, and more. The clinical portion of the LOINC database includes entries for vital signs, hemodynamics, intake/output, EKG, obstetric ultrasound, cardiac echo, radiology report titles, pulmonary ventilator management, document and section titles, patient assessment instruments (e.g. Glascow Coma Score, PHQ-9 depression scale, CMS-required patient assessment instruments), and other clinical observations.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		LN,
		///<summary>109 - Medicaid
		///<para>Source:Medicaid billing codes/names.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		MCD,
		///<summary>110 - Medicare
		///<para>Source:Medicare billing codes/names.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		MCR,
		///<summary>111 - Medical Device Communication
		///<para>Source:EN ISO/IEEE 11073-10101 Health informatics – Point-of-care medical device communication - Nomenclature</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		MDC,
		///<summary>112 - Medispan Diagnostic Codes
		///<para>Source:Codes Used for drug-diagnosis interaction checking. Proprietary product. Hierarchical drug codes for identifying drugs down to manufacturer and pill size. MediSpan, Inc., 8425 Woodfield Crossing Boulevard, Indianapolis, IN 46240. Tel: (800) 428-4495. URL: http://www.medispan.com/Products/index.aspx?cat=1. As above for MGPI.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		MDDX,
		///<summary>113 - Medical Economics Drug Codes
		///<para>Source:Proprietary Codes for identifying drugs. Proprietary product of Medical Economics Data, Inc. (800) 223-0581.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		MEDC,
		///<summary>114 - MIME Media Type IANA
		///<para>Source:Encoding as defined by MIME (Multipurpose Internet Mail Extensions)</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		MEDIATYPE,
		///<summary>115 - Medical Dictionary for Drug Regulatory Affairs (MEDDRA)
		///<para>Source:Patrick Revelle, Director MSSO 12011 Sunset Hills Road, VAR1/7B52 Reston, VA 20190 Patrick.Revelle@ngc.com http://www.meddramsso.com/MSSOWeb/index.htm</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		MEDR,
		///<summary>116 - Medical Economics Diagnostic Codes
		///<para>Source:Used for drug-diagnosis interaction checking. Proprietary product of Medical Economics Data, Inc. (800) 223-0581.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		MEDX,
		///<summary>117 - Medispan GPI
		///<para>Source:Medispan hierarchical drug codes for identifying drugs down to manufacturer and pill size. Proprietary product of MediSpan, Inc., 8425 Woodfield Crossing Boulevard, Indianapolis, IN 46240. Tel: (800) 428-4495.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		MGPI,
		///<summary>118 - CDC Vaccine Manufacturer Codes
		///<para>Source:As above, for CVX</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		MVX,
		///<summary>119 - Industry (NAICS)
		///<para>Source:The North American Industry Classification System (NAICS) consists of a set of six digit codes that classify and categorize industries.  It also organizes the categories on a production/process-oriented basis.  This new, uniform, industry-wide classification system has been designed as the index for statistical reporting of all economic activities of the U.S., Canada, and Mexico. Mapping is available between SIC 1987 and NAICS 2002 codes at U.S. Census Bureau website. Mapping is also available between NAICS 2002 and NAICS 2007 at U.S. Census Bureau website</para>
		///<para>Category:Demographic Code</para>
		///<para>Status:New</para></summary>
		NAICS,
		///<summary>120 - NCPDP code list for data element nnnn [as used in segment sss]
		///<para>Source:NCPDP maintain code list associated with the specified Data Element (nnnn) and Segment (sss).  The Segment portion is optional if there is no specialization of the Data Element codes between segments.  Examples:   NCPDP1131RES = code set defined for NCPDP data element 1131 as used in the RES segment (Code List Qualifier – Response Code)   NCPDP1131STS = code set defined for NCPDP data element 1131 as used in the STS segment (Code List Qualifier – Reject Code)   NCPDP9701 = code set defined for NCPDP data element 9701 (Individual Relationship, Coded).  No specialization to a segment exists for this data element. National Council for Prescription Drug Programs, 924Ø East Raintree Drive, Scottsdale, AZ  8526Ø. Phone: (48Ø) 477-1ØØØ Fax: (48Ø) 767-1Ø42 e-mail: ncpdp@ncpdp.org www.ncpdp.org</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		NCPDPnnnnsss,
		///<summary>121 - NANDA
		///<para>Source:North American Nursing Diagnosis Association, Philadelphia, PA.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		NDA,
		///<summary>122 - National drug codes
		///<para>Source:These provide unique codes for each distinct drug, dosing form, manufacturer, and packaging. (Available from the National Drug Code Directory, FDA, Rockville, MD, and other sources.)</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		NDC,
		///<summary>123 - NDF-RT (Drug Classification)
		///<para>Source:The National Drug File RT (NDF-RT) is published by the US Veterans' Administration (VA). NDF-RT covers clinical drugs used at the VA. The NCI version of NDF-RT is used by NCI to provide automated terminology access to the Food and Drug Administration (FDA) Structured Product Label (SPL) initiative.</para>
		///<para>Category:Drug Code</para>
		///<para>Status:New</para></summary>
		NDFRT,
		///<summary>124 - Nursing Interventions Classification
		///<para>Source:Iowa Intervention Project, College of Nursing, University of Iowa, Iowa City, Iowa</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		NIC,
		///<summary>125 - Source of Information (Immunization)
		///<para>Source:CDC National Immunization Program's (NIP) defined table to be used in HL7 2.x message RXA-9 for documenting the source of information regarding immunization. E.g. From school, provider,public health agency.</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		NIP001,
		///<summary>126 - Substance refusal reason
		///<para>Source:CDC National Immunization Program's (NIP) defined table to be used in HL7 2.x message RXA-18 for substance refusal reason (reasons for not having vaccination). E.g. Religious exemption, parental decision</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		NIP002,
		///<summary>127 - Vaccination - Contraindications, Precautions, and Immunities
		///<para>Source:CDC National Immunization Program's (NIP) defined table for vaccine contraindications and precautions. E.g. Allergy to egg ingestion, thimerosol</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		NIP004,
		///<summary>128 - Vaccinated at location (facility)
		///<para>Source:CDC National Immunization Program's (NIP) defined table for vaccinated at location (facility). E.g.  Private doctor's office, Public Health Clinic</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		NIP007,
		///<summary>129 - Vaccine purchased with (Type of funding)
		///<para>Source:CDC National Immunization Program's (NIP) defined table enumerates the type of funds used for purchasing vaccine. E.g. Public funds, Military funds</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		NIP008,
		///<summary>130 - Reported adverse event previously
		///<para>Source:CDC National Immunization Program's (NIP) defined table enumerates the authorities to whom the vaccination related adverse events were previously reported. E.g. To health department, To manufacturer</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		NIP009,
		///<summary>131 - VAERS Report type
		///<para>Source:CDC National Immunization Program's (NIP) defined table enumerates the type of report used in VAERS (Vaccination Adverse Event Reporting System). E.g. Initial, Follow-up</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		NIP010,
		///<summary>132 - Notifiable Event (Disease/Condition) Code List
		///<para>Source:List of notifiable events, which includes infectious and non-infectious disease or conditions. This list includes events that are notifiable at the state and national level.</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		NND,
		///<summary>133 - National Provider Identifier
		///<para>Source:Health Care Finance Administration, US Dept. of Health and Human Services, 7500 Security Blvd., Baltimore, MD 21244.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		NPI,
		///<summary>134 - National Uniform Billing Committee Code
		///<para>Source:http://www.nubc.org/</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		NUBC,
		///<summary>135 - Flavors of NULL
		///<para>Source:System of coded values for Flavors of Null, as used in HL7 Version 3 standards.  Identical to the HL7 version 3  coding system 2.16.840.1.113883.5.1008 NullFlavor</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		NULLFL,
		///<summary>136 - German Procedure Codes
		///<para>Source:Source: OPS Operationen- und Prozedurenschlussel. Three versions are active.</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		O301,
		///<summary>137 - OPS Germany 2004
		///<para>Source:Source: OPS Operationen- und Prozedurenschlussel. Three versions are active</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		O3012004,
		///<summary>138 - OPS Germany 2005
		///<para>Source:Source: OPS Operationen- und Prozedurenschlussel. Three versions are active</para>
		///<para>Category:</para>
		///<para>Status:Obsolete</para></summary>
		O3012005,
		///<summary>139 - Ops Germany 2006
		///<para>Source:Source: OPS Operationen- und Prozedurenschlussel. Three versions are active</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		O3012006,
		///<summary>140 - Observation Method Code
		///<para>Source:For use in v2.x systems interoperating with V3 systems.  Identical to the code system 2.16.840.1.113883.5.84 ObservationMethod in the Version 3 vocabulary.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		OBSMETHOD,
		///<summary>141 - Omaha System
		///<para>Source:Omaha Visiting Nurse Association, Omaha, NB.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		OHA,
		///<summary>142 - OPS Germany v2007
		///<para>Source:Source: OPS Operationen- und Prozedurenschlussel 2007.</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		OPS2007,
		///<summary>143 - OPS Germany v2008
		///<para>Source:Source: OPS Operationen- und Prozedurenschlussel 2008.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		OPS2008,
		///<summary>144 - OPS Germany v2008
		///<para>Source:Source: OPS Operationen- und Prozedurenschlussel 2009.</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		OPS2009,
		///<summary>145 - CDC Public Health Information Network (PHIN) Question
		///<para>Source:CDC Public Health Questions used in HL7 Message as observation identifiers. These question or observation identifiers are used in CDC's message implementation guides and will be passed in HL7 OBX-3 or Observation.Code</para>
		///<para>Category:Public Health Code</para>
		///<para>Status:New</para></summary>
		PHINQUESTION,
		///<summary>146 - CDC PHLIP Lab result codes that are not covered in SNOMED at the time of this implementation
		///<para>Source:APHL CDC co-sponsored PHLIP</para>
		///<para>Category:Lab Code</para>
		///<para>Status:New</para></summary>
		PLR,
		///<summary>147 - CDC PHLIP Lab test codes, where LOINC concept is too broad or not yet available, especially as needed for ordering and or lab to lab reporting )
		///<para>Source:APHL CDC co-sponsored PHLIP</para>
		///<para>Category:Lab Code</para>
		///<para>Status:New</para></summary>
		PLT,
		///<summary>148 - POS Codes
		///<para>Source:HCFA Place of Service Codes for Professional Claims (see http://www.cms.hhs.gov/PlaceofServiceCodes/).</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		POS,
		///<summary>149 - Paticipation Mode Code
		///<para>Source:For use in v2.x systems interoperating with V3 systems.  Identical to the code system 2.16.840.1.113883.5.1064 ParticipationMode in the Version 3 vocabulary.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		PRTCPTNMODE,
		///<summary>150 - Read Classification
		///<para>Source:The Read Clinical Classification of Medicine, Park View Surgery, 26 Leicester Rd., Loughborough LE11 2AG (includes drug procedure and other codes, as well as diagnostic codes).</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		RC,
		///<summary>151 - Used initially for contact roles.
		///<para>Source:For use in v2.x systems interoperating with V3 systems.  Identical to the code system 2.16.840.1.113883.5.111 RoleCode in the Version 3 vocabulary.</para>
		///<para>Category:General Codes</para>
		///<para>Status:new</para></summary>
		ROLECLASS,
		///<summary>152 - Participation Mode
		///<para>Source:For use in v2.x systems interoperating with V3 systems.  Identical to the code system 2.16.840.1.113883.5.111 RoleCode in the Version 3 vocabulary.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		ROLECODE,
		///<summary>153 - Specifies the mode, immediate versus deferred or queued, by which a receiver should communicate its receiver responsibilities.
		///<para>Source:V3 coding system, available in RIM download materials.</para>
		///<para>Category:General Codes</para>
		///<para>Status:New</para></summary>
		RSPMODE,
		///<summary>154 - RxNorm
		///<para>Source:RxNorm provides standard names for clinical drugs (active ingredient + strength + dose form) and for dose forms as administered to a patient. It provides links from clinical drugs, both branded and generic, to their active ingredients, drug components (active ingredient + strength), and related brand names. NDCs (National Drug Codes) for specific drug products (where there are often many NDC codes for a single product) are linked to that product in RxNorm. RxNorm links its names to many of the drug vocabularies commonly used in pharmacy management and drug interaction software, including those of First Databank, Micromedex, MediSpan, and Multum. By providing links between these vocabularies, RxNorm can mediate messages between systems not using the same software and vocabulary.RxNorm is one of a suite of designated standards for use in U.S. Federal Government systems for the electronic exchange of clinical health information.</para>
		///<para>Category:Drug Code</para>
		///<para>Status:New</para></summary>
		RXNORM,
		///<summary>155 - SNOMED Clinical Terms
		///<para>Source:SNOMED-CT concept identifier codes. SNOMED International, I325 Waukegan Rd, Northfield, IL, 60093, +1 800-323-4040, mailto:snomed@cap.org  http://www.snomed.org</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		SCT,
		///<summary>156 - SNOMED Clinical Terms alphanumeric codes
		///<para>Source:Used to indicate that the code value is the legacy-style SNOMED alphanumeric codes, rather than the concept identifier codes.  SNOMED International, I325 Waukegan Rd, Northfield, IL, 60093, +1 800-323-4040, mailto:snomed@cap.org  http://www.snomed.org</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		SCT2,
		///<summary>157 - SNOMED- DICOM Microglossary
		///<para>Source:College of American Pathologists, Skokie, IL, 60077-1034. (formerly designated as 99SDM).</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		SDM,
		///<summary>158 - Industry (SIC)
		///<para>Source:Standard Industry Classification - 1987. Use NAICS 2002. This is mainly for mapping and backward compatibility purposes.</para>
		///<para>Category:Demographic Code</para>
		///<para>Status:New</para></summary>
		SIC,
		///<summary>159 - Systemized Nomenclature of Medicine (SNOMED)
		///<para>Source:Systemized Nomenclature of Medicine, 2nd Edition 1984 Vols 1, 2, College of American Pathologists, Skokie, IL.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		SNM,
		///<summary>160 - SNOMED International
		///<para>Source:SNOMED International, 1993 Vols 1-4, College of American Pathologists, Skokie, IL, 60077-1034.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		SNM3,
		///<summary>161 - SNOMED topology codes (anatomic sites)
		///<para>Source:College of American Pathologists, 5202 Old Orchard Road, Skokie, IL 60077-1034.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		SNT,
		///<summary>162 - Occupation (SOC 2000)
		///<para>Source:The 2000 Standard Occupational Classification (SOC) system is used by Federal statistical agencies to classify workers into occupational categories for the purpose of collecting, calculating, or disseminating data.</para>
		///<para>Category:Demographic Code</para>
		///<para>Status:New</para></summary>
		SOC,
		///<summary>163 - Priority (Type) of Visit
		///<para>Source:Source: Official UB-04 Data Specification Manual, published July 2007, by the National Uniform Billing Committee (NUBC), and can be found at http://www.nubc.org. This coding system supersedes UB92 and is effective immediately (July, 2007).</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		UB04FL14,
		///<summary>164 - Point of Origin 
		///<para>Source:Source: Official UB-04 Data Specification Manual, published July 2007, by the National Uniform Billing Committee (NUBC), and can be found at http://www.nubc.org. This coding system supersedes UB92 and is effective immediately (July, 2007).</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		UB04FL15,
		///<summary>165 - Patient Discharge Status
		///<para>Source:Source: Official UB-04 Data Specification Manual, published July 2007, by the National Uniform Billing Committee (NUBC), and can be found at http://www.nubc.org. This coding system supersedes UB92 and is effective immediately (July, 2007).</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		UB04FL17,
		///<summary>166 - Occurrence Code
		///<para>Source:Source: Official UB-04 Data Specification Manual, published July 2007, by the National Uniform Billing Committee (NUBC), and can be found at http://www.nubc.org. This coding system supersedes UB92 and is effective immediately (July, 2007).</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		UB04FL31,
		///<summary>167 - Occurrence Span
		///<para>Source:Source: Official UB-04 Data Specification Manual, published July 2007, by the National Uniform Billing Committee (NUBC), and can be found at http://www.nubc.org. This coding system supersedes UB92 and is effective immediately (July, 2007).</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		UB04FL35,
		///<summary>168 - Value Code
		///<para>Source:Source: Official UB-04 Data Specification Manual, published July 2007, by the National Uniform Billing Committee (NUBC), and can be found at http://www.nubc.org. This coding system supersedes UB92 and is effective immediately (July, 2007).</para>
		///<para>Category:</para>
		///<para>Status:New</para></summary>
		UB04FL39,
		///<summary>169 - UCDS
		///<para>Source:Uniform Clinical Data Systems. Ms. Michael McMullan, Office of Peer Review Health Care Finance Administration, The Meadows East Bldg., 6325 Security Blvd., Baltimore, MD 21207; (301) 966 6851.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		UC,
		///<summary>170 - UCUM code set for units of measure(from Regenstrief)
		///<para>Source:Added by motion of VOCABULARY T.C. 20060308 14-0-3</para>
		///<para>Category:</para>
		///<para>Status:Active</para></summary>
		UCUM,
		///<summary>171 - MDNS
		///<para>Source:Universal Medical Device Nomenclature System. ECRI, 5200 Butler Pike, Plymouth Meeting, PA  19462 USA. Phone: 215-825-6000, Fax: 215-834-1275.</para>
		///<para>Category:Device code</para>
		///<para>Status:Active</para></summary>
		UMD,
		///<summary>172 - Unified Medical Language
		///<para>Source:National Library of Medicine, 8600 Rockville Pike, Bethesda, MD 20894.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		UML,
		///<summary>173 - Universal Product Code
		///<para>Source:The Uniform Code Council. 8163 Old Yankee Road, Suite J, Dayton, OH  45458; (513) 435 3070</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		UPC,
		///<summary>174 - UPIN
		///<para>Source:Medicare/CMS 's (formerly HCFA)  universal physician identification numbers, available from Health Care Financing Administration, U.S. Dept. of Health and Human Services, Bureau of Program Operations, 6325 Security Blvd., Meadows East Bldg., Room 300, Baltimore, MD 21207</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		UPIN,
		///<summary>175 - U.S. Board on Geographic Names (USGS - GNIS)
		///<para>Source:List of populated places(City) from U.S. Geological Survey Geographic Name Information System (USGS GNIS)</para>
		///<para>Category:Demographic Code</para>
		///<para>Status:New</para></summary>
		USGSGNIS,
		///<summary>176 - United States Postal Service
		///<para>Source:Two Letter State and Possession Abbreviations are listed in  Publication 28, Postal Addressing Standards which can be obtained from Address Information Products, National Address Information Center, 6060 Primacy Parkway, Suite 101, Memphis, Tennessee  38188-0001 Questions of comments regarding the publication should be addressed to the Office of Address and Customer Information Systems, Customer and Automation Service Department, US Postal Service, 475 Lenfant Plaza SW Rm 7801, Washington, DC  20260-5902</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:Active</para></summary>
		USPS,
		///<summary>177 - Clinicians are required to track the Vaccine Information Sheet (VIS) that was shared with the recipient of a vaccination.  This code system contains codes that  identify the document type and the owner of the document.
		///<para>Source:More information can be found at the CDC Immunization Standards page at http://www.cdc.gov/vaccines/programs/iis/default.htm.  Content may be downloaded from PHINVADS at https://phinvads.cdc.gov/vads/ViewCodeSystem.action?id=2.16.840.1.113883.6.304 and at http://www.cdc.gov/phin/activities/vocabulary.html.</para>
		///<para>Category:Specific Non-Drug Code</para>
		///<para>Status:active</para></summary>
		VIS,
		///<summary>178 - WHO record # drug codes (6 digit)
		///<para>Source:World Health organization record number code. A unique sequential number is assigned to each unique single component drug and to each multi-component drug. Eight digits are allotted to each such code, six to identify the active agent, and 2 to identify the salt, of single content drugs. Six digits are assigned to each unique combination of drugs in a dispensing unit. The six digit code is identified by W1, the 8 digit code by W2.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		W1,
		///<summary>179 - WHO record # drug codes (8 digit)
		///<para>Source:World Health organization record number code. A unique sequential number is assigned to each unique single component drug and to each multi-component drug. Eight digits are allotted to each such code, six to identify the active agent, and 2 to identify the salt, of single content drugs. Six digits are assigned to each unique combination of drugs in a dispensing unit. The six digit code is identified by W1, the 8 digit code by W2.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		W2,
		///<summary>180 - WHO record # code with ASTM extension
		///<para>Source:With ASTM extensions (see Implementation Guide), the WHO codes can be used to report serum (and other) levels, patient compliance with drug usage instructions, average daily doses and more (see Appendix X1 the Implementation Guide).</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		W4,
		///<summary>181 - WHO ATC
		///<para>Source:WHO’s ATC codes provide a hierarchical classification of drugs by therapeutic class. They are linked to the record number codes listed above.</para>
		///<para>Category:Drug code</para>
		///<para>Status:Active</para></summary>
		WC,
		///<summary>182 - ASC X12 Code List nnnn
		///<para>Source:Code list associated with X12 Data Element nnnn.  Example::     X12DE738 – code set defined for X12 data element 738 (Measurement Qualifier) The Accredited Standards Committee (ASC) X12 www.x12.org</para>
		///<para>Category:General Codes</para>
		///<para>Status:Active</para></summary>
		X12DEnnnn
	}

}