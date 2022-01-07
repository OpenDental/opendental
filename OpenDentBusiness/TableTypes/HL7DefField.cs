using System;
using System.Collections.Generic;

namespace OpenDentBusiness{
	///<summary>Multiple fields per segment.</summary>
	[Serializable()]
	public class HL7DefField:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long HL7DefFieldNum;
		///<summary>FK to hl7deffield.HL7DefSegmentNum</summary>
		public long HL7DefSegmentNum;
		///<summary>Position within the segment.</summary>
		public int OrdinalPos;
		///<summary>HL7 table Id, if applicable. Example: 0234. Example: 1234/2345.  DataType will be ID.</summary>
		public string TableId;
		///<summary>The DataTypeHL7 enum will be unlinked from the db by storing as string in db. As it's loaded into OD, it will become an enum.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public DataTypeHL7 DataType;
		///<summary>User will get to pick from a list of fields that we will maintain. Example: guar.nameLFM, prov.provIdName, or pat.addressCityStateZip.  See below for the full list.  This will be blank if this is a fixed text field.</summary>
		public string FieldName;
		///<summary>User will need to insert fixed text for some fields.  Either FixedText or FieldName will have a value, not both.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string FixedText;

		///<summary></summary>
		public HL7DefField Clone() {
			return (HL7DefField)this.MemberwiseClone();
		}

	}

	public class FieldNameAndType{
		public string Name;
		public string TableId;
		public DataTypeHL7 DataType;

		public FieldNameAndType(string name,DataTypeHL7 datatype) {
			Name=name;
			DataType=datatype;
			TableId="";
		}

		public FieldNameAndType(string name,DataTypeHL7 datatype,string tableid) {
			Name=name;
			DataType=datatype;
			TableId=tableid;
		}

		public static List<FieldNameAndType> GetFullList() {
			List<FieldNameAndType> retVal=new List<FieldNameAndType>();
			retVal.Add(new FieldNameAndType("accountNum",DataTypeHL7.CX));//used by LabCorp to send the accountNum and fasting flag in position 7
			retVal.Add(new FieldNameAndType("ackCode",DataTypeHL7.ID,"0008"));
			retVal.Add(new FieldNameAndType("allergenRxNorm",DataTypeHL7.CWE));//Example: RxNormCode^^RXNORM^^^^^^
			retVal.Add(new FieldNameAndType("allergenType",DataTypeHL7.CWE,"0127"));//DA - Drug Allergy, FA - Food Allergy, MA - Miscellaneous Allergy
			retVal.Add(new FieldNameAndType("altPatID",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("apt.AptDateTime",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("apt.AptNum",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("apt.aptStatus",DataTypeHL7.CWE,"0278"));
			retVal.Add(new FieldNameAndType("apt.confirmStatus",DataTypeHL7.CWE,"0278"));
			retVal.Add(new FieldNameAndType("apt.endAptDateTime",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("apt.externalAptID",DataTypeHL7.EI));
			retVal.Add(new FieldNameAndType("apt.length",DataTypeHL7.CQ));
			retVal.Add(new FieldNameAndType("apt.lengthStartEnd",DataTypeHL7.TQ));
			retVal.Add(new FieldNameAndType("apt.location",DataTypeHL7.PL));//Example: ClinicDescript^OpName^^PracticeTitle^^c  (c for clinic), for inbound, used to set an apt.ClinicNum but not update pat.ClinicNum
			retVal.Add(new FieldNameAndType("apt.Note",DataTypeHL7.FT));
			retVal.Add(new FieldNameAndType("apt.operatory",DataTypeHL7.CWE));//Schedule ID
			retVal.Add(new FieldNameAndType("apt.type",DataTypeHL7.CWE,"0277"));//Normal or Complete
			retVal.Add(new FieldNameAndType("apt.userOD",DataTypeHL7.CWE));//Filler contact person and entered by person
			retVal.Add(new FieldNameAndType("base64File",DataTypeHL7.ST));//base64 representation of an embedded file
			retVal.Add(new FieldNameAndType("carrier.addressCityStateZip",DataTypeHL7.XAD));
			retVal.Add(new FieldNameAndType("carrier.CarrierName",DataTypeHL7.XON));
			retVal.Add(new FieldNameAndType("carrier.ElectID",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("carrier.Phone",DataTypeHL7.XTN));
			retVal.Add(new FieldNameAndType("clinicalInfo",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("dateTime.Now",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("dateTimeCollected",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("dateTimeEntered",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("dateTimeObs",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("dateTimeReported",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("dateTimeSpecimen",DataTypeHL7.DR));
			retVal.Add(new FieldNameAndType("eventType",DataTypeHL7.ID,"0003"));
			retVal.Add(new FieldNameAndType("facilityID",DataTypeHL7.CE));
			retVal.Add(new FieldNameAndType("facilityName",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("facilityAddress",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("facilityPhone",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("facilityDirector",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("guar.addressCityStateZip",DataTypeHL7.XAD));
			retVal.Add(new FieldNameAndType("guar.birthdateTime",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("guar.ChartNumber",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("guar.Gender",DataTypeHL7.IS));
			retVal.Add(new FieldNameAndType("guar.HmPhone",DataTypeHL7.XTN));
			retVal.Add(new FieldNameAndType("guar.nameLFM",DataTypeHL7.XPN));
			retVal.Add(new FieldNameAndType("guar.PatNum",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("guar.SSN",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("guar.WkPhone",DataTypeHL7.XTN));
			retVal.Add(new FieldNameAndType("guarIdList",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("insplan.cob",DataTypeHL7.IS,"0173"));//CO-Coordination, IN-Independent
			retVal.Add(new FieldNameAndType("insplan.coverageType",DataTypeHL7.IS,"0309"));//M-Medical, D-Dental
			retVal.Add(new FieldNameAndType("insplan.empName",DataTypeHL7.XON));
			retVal.Add(new FieldNameAndType("insplan.GroupName",DataTypeHL7.XON));
			retVal.Add(new FieldNameAndType("insplan.GroupNum",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("insplan.planNum",DataTypeHL7.CWE));//root.7.PlanNum
			retVal.Add(new FieldNameAndType("insplan.PlanType",DataTypeHL7.IS,"0086"));//Category Percentage, PPO Percentage, Medicaid or Flat Copay, Capitation
			retVal.Add(new FieldNameAndType("inssub.AssignBen",DataTypeHL7.IS,"0135"));//0-N, 1-Y
			retVal.Add(new FieldNameAndType("inssub.DateEffective",DataTypeHL7.DT));
			retVal.Add(new FieldNameAndType("inssub.DateTerm",DataTypeHL7.DT));
			retVal.Add(new FieldNameAndType("inssub.ReleaseInfo",DataTypeHL7.IS,"0093"));//0-N, 1-Y
			retVal.Add(new FieldNameAndType("inssub.subAddrCityStateZip",DataTypeHL7.XAD));
			retVal.Add(new FieldNameAndType("inssub.subBirthdate",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("inssub.SubscriberID",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("inssub.subscriberName",DataTypeHL7.XPN));
			retVal.Add(new FieldNameAndType("labNote",DataTypeHL7.FT));//this is used by LabCorp and will be in the medlab.NoteLab column
			retVal.Add(new FieldNameAndType("labPatID",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("medicationRxNorm",DataTypeHL7.CWE));//code^descript^codeSystem, Example: RxNorm Code^^RXNORM, descript is ignored
			retVal.Add(new FieldNameAndType("messageControlId",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("messageType",DataTypeHL7.MSG));
			retVal.Add(new FieldNameAndType("messageTypeNoStruct",DataTypeHL7.MSG));
			retVal.Add(new FieldNameAndType("obsAbnormalFlag",DataTypeHL7.ID,"0078"));
			retVal.Add(new FieldNameAndType("obsID",DataTypeHL7.CE));
			retVal.Add(new FieldNameAndType("obsIDSub",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("obsNote",DataTypeHL7.FT));//this is used by LabCorp and will be in the medlabresult.Note column
			retVal.Add(new FieldNameAndType("obsRefRange",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("obsTestID",DataTypeHL7.CE));
			retVal.Add(new FieldNameAndType("obsUnits",DataTypeHL7.CE));
			retVal.Add(new FieldNameAndType("obsValue",DataTypeHL7.Varied));
			retVal.Add(new FieldNameAndType("obsValueType",DataTypeHL7.ID,"0125"));
			retVal.Add(new FieldNameAndType("orderingProv",DataTypeHL7.XCN));
			retVal.Add(new FieldNameAndType("parentObsID",DataTypeHL7.PRL));
			retVal.Add(new FieldNameAndType("parentObsTestID",DataTypeHL7.EIP));
			retVal.Add(new FieldNameAndType("pat.addressCityStateZip",DataTypeHL7.XAD));
			retVal.Add(new FieldNameAndType("pat.birthdateTime",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("pat.ChartNumber",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("pat.FeeSched",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("pat.Gender",DataTypeHL7.IS,"0001"));//M,F,U,etc.
			retVal.Add(new FieldNameAndType("pat.GradeLevel",DataTypeHL7.IS,"0004"));//integer in range 1-12
			retVal.Add(new FieldNameAndType("pat.HmPhone",DataTypeHL7.XTN));
			retVal.Add(new FieldNameAndType("pat.HmPhone_V2_3",DataTypeHL7.XTN));//Allows PID-13.2 to contain 'NET' for email or 'WPN' for work phone.  Any other value in PID-13.2, process same as pat.HmPhone.  Example for NET: |^NET^^john@somewhere.com|, for WPN: |^WPN^PH^^^503^5551234|
			retVal.Add(new FieldNameAndType("pat.location",DataTypeHL7.PL));//Example: ClinicDescript^OpName^^PracticeTitle^^c  (c for clinic), for inbound, will set both pat.ClinicNum and apt.ClinicNum if an appointment is identified in the message
			retVal.Add(new FieldNameAndType("pat.location_V2_3",DataTypeHL7.PL));//Example: ^ClinicDescript, for inbound, works the same as pat.location except uses PV1-3.2 for the clinic name instead of PV1-3.1
			retVal.Add(new FieldNameAndType("pat.nameLFM",DataTypeHL7.XPN));
			retVal.Add(new FieldNameAndType("pat.PatNum",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("pat.Position",DataTypeHL7.CWE,"0002"));
			retVal.Add(new FieldNameAndType("pat.Race",DataTypeHL7.CWE,"0005"));//Example: |2028-9^Asian^CDCREC|
			retVal.Add(new FieldNameAndType("pat.Race_V2_3",DataTypeHL7.CWE,"0005"));//Same as pat.Race, except CDCREC not required in PID-10.3.  Example: |2028-9^Asian|
			retVal.Add(new FieldNameAndType("pat.site",DataTypeHL7.PL));//Example: ClinicDescript^OpName^^PracticeTitle^^s (s for site)
			retVal.Add(new FieldNameAndType("pat.SSN",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("pat.WkPhone",DataTypeHL7.XTN));
			retVal.Add(new FieldNameAndType("pat.Urgency",DataTypeHL7.IS,"0018"));//0 - Unknown, 1 - NoProblems, 2 - NeedsCare, 3 - Urgent
			retVal.Add(new FieldNameAndType("patBirthdateAge",DataTypeHL7.DTM));//atually not a DTM data type since LabCorp uses bday^years^months^days
			retVal.Add(new FieldNameAndType("patNote",DataTypeHL7.FT));//this is used by LabCorp and will be in the medlab.NotePat column
			retVal.Add(new FieldNameAndType("patientIds",DataTypeHL7.CX));
			retVal.Add(new FieldNameAndType("patplan.Ordinal",DataTypeHL7.ST));//1,2,3...
			retVal.Add(new FieldNameAndType("patplan.policyNum",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("patplan.subRelationToPat",DataTypeHL7.CWE,"0063"));//SEL-Self, SPO-Spouse, DOM-LifePartner, CHD-Child (return PAR-Parent), EME-Employee (return EMR-Employer), DEP-HandicapDep (return GRD-Guardian), OTH-SignifOther, OTH-InjuredPlantiff, OTH-Dependent (return GRD-Guardian)
			retVal.Add(new FieldNameAndType("pdfDescription",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("pdfDataAsBase64",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("problemAction",DataTypeHL7.ID,"0287"));//AD-ADD,CO-Correct,DE-Delete,LI-Link,UC-Unchanged,UN-Unlink,UP-Update.  AD/UP are currently supported
			retVal.Add(new FieldNameAndType("problemCode",DataTypeHL7.CWE));
			retVal.Add(new FieldNameAndType("problemStartDate",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("problemStopDate",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("problemUniqueId",DataTypeHL7.EI));
			retVal.Add(new FieldNameAndType("proc.DiagnosticCode",DataTypeHL7.CWE,"0051"));
			retVal.Add(new FieldNameAndType("proc.location",DataTypeHL7.PL));//Example: ClinicDescript^OpName^^PracticeTitle^^c  (c for clinic), for outbound FT1 segments, clinic from proc unless not set then from patient
			retVal.Add(new FieldNameAndType("proc.Note",DataTypeHL7.FT));
			retVal.Add(new FieldNameAndType("proc.procDateTime",DataTypeHL7.DTM));
			retVal.Add(new FieldNameAndType("proc.ProcFee",DataTypeHL7.CP));
			retVal.Add(new FieldNameAndType("proc.ProcNum",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("proc.Status",DataTypeHL7.IS));
			retVal.Add(new FieldNameAndType("proc.toothSurfRange",DataTypeHL7.CNE,"0340"));
			retVal.Add(new FieldNameAndType("proc.uniqueId",DataTypeHL7.EI));
			retVal.Add(new FieldNameAndType("proccode.ProcCode",DataTypeHL7.CNE,"0088"));
			retVal.Add(new FieldNameAndType("prov.provIdName",DataTypeHL7.XCN));//Example: |IdNum^FamilyName, GivenName MiddleI^^Abbrev|
			retVal.Add(new FieldNameAndType("prov.provIdName_V2_3",DataTypeHL7.XCN));//Just like prov.provIdName except doesn't require abbreviation component.  Example: |IdNum^FamilyName, GivenName MiddleI|
			retVal.Add(new FieldNameAndType("prov.provIdNameLFM",DataTypeHL7.XCN));//Provider id table is user defined table and different number depending on what segment it is pulled from.  Example: FT1 Performed By Code table is 0084, PV1 Attending Doctor is table 0010
			retVal.Add(new FieldNameAndType("prov.provIdNameLFM_V2_3",DataTypeHL7.XCN));//Just like prov.provIdNameLFM except doesn't require abbreviation component.  Example PV1-7 field: |IdNum^FamilyName^GivenName^MiddleNameOrInitial|
			retVal.Add(new FieldNameAndType("prov.provType",DataTypeHL7.CWE,"0182"));//accepted values: 'd' or 'D' for dentist, 'h' or 'H' for hygienist
			retVal.Add(new FieldNameAndType("resultStatus",DataTypeHL7.ID,"0123"));
			retVal.Add(new FieldNameAndType("segmentAction",DataTypeHL7.ID,"0206"));//A-Add/Insert, D-Delete, U-Update, X-No Change
			retVal.Add(new FieldNameAndType("sendingApp",DataTypeHL7.HD,"0361"));//the Open Dental HL7 root assigned to the office and stored in the oidinternal table with the IDType of Root
			retVal.Add(new FieldNameAndType("sendingFacility",DataTypeHL7.HD,"0362"));
			retVal.Add(new FieldNameAndType("separators^~\\&",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("sequenceNum",DataTypeHL7.SI));
			retVal.Add(new FieldNameAndType("specimenAction",DataTypeHL7.ID,"0065"));
			retVal.Add(new FieldNameAndType("specimenDescript",DataTypeHL7.ST));
			retVal.Add(new FieldNameAndType("specimenID",DataTypeHL7.EI));
			retVal.Add(new FieldNameAndType("specimenIDFiller",DataTypeHL7.EI));
			retVal.Add(new FieldNameAndType("specimenIDAlt",DataTypeHL7.EI));
			retVal.Add(new FieldNameAndType("totalVolume",DataTypeHL7.CQ));
			return retVal;
		}

		public static DataTypeHL7 GetTypeFromName(string name) {
			List<FieldNameAndType> list=GetFullList();
			for(int i=0;i<list.Count;i++) {
				if(list[i].Name==name) {
					return list[i].DataType;
				}
			}
			throw new ApplicationException("Unknown field name: "+name);
		}

		public static string GetTableNumFromName(string name) {
			List<FieldNameAndType> list=GetFullList();
			for(int i=0;i<list.Count;i++) {
				if(list[i].Name==name) {
					return list[i].TableId;
				}
			}
			throw new ApplicationException("Unknown field name: "+name);
		}
	}

	///<summary>Data types are listed in HL7 docs section 2.15.  The items in this enumeration can be freely rearranged without damaging the database.  But can't change spelling or remove existing item.</summary>
	public enum DataTypeHL7 {
		///<summary>Coded element.  Has been replaced in v2.6 with CNE and CWE, retained for backward compatibility only.</summary>
		CE,
		///<summary>Coded with no exceptions.  Examples: ProcCode (Dxxxx) or TreatmentArea like tooth^surface</summary>
		CNE,
		///<summary>Composite price.  Example: 125.00</summary>
		CP,
		///<summary>Composite quantity with units.  Example: 123.7^kg</summary>
		CQ,
		///<summary>Coded with exceptions.  Example: Race: American Indian or Alaska Native,Asian,Black or African American,Native Hawaiian or Other Pacific,White, Hispanic,Other Race.</summary>
		CWE,
		///<summary>Extended composite ID with check digit.  Example: patient.PatNum or patient.ChartNumber or appointment.AptNum.</summary>
		CX,
		///<summary>Date/Time Range.  DateTimeStart&amp;DateTimeEnd.</summary>
		DR,
		///<summary>Date.  yyyyMMdd.  4,6,or 8</summary>
		DT,
		///<summary>Date/Time.  yyyyMMddHHmmss etc.  Allowed 4,6,8,10,12,14.  Possibly more, but unlikely.</summary>
		DTM,
		///<summary>Entity identifier.  Example: appointment.AptNum</summary>
		EI,
		///<summary>Entity identifier pair.</summary>
		EIP,
		///<summary>Formatted text data.  We support new lines identified by '\.br\' (where '\' is the defined escape char, \ is the default)</summary>
		FT,
		///<summary>Hierarchic designator.  Application identifier.  Example: "OD" for OpenDental.</summary>
		HD,
		///<summary>Coded value for HL7 defined tables.  Must include TableId.  Example: 0003 is eCW's event type table id.</summary>
		ID,
		///<summary>Coded value for user-defined tables.  Example: Administrative Sex, F=Female, M=Male,U=Unknown.</summary>
		IS,
		///<summary>Message type.  Example: composed of messageType^eventType like DFT^P03</summary>
		MSG,
		///<summary>Numeric.  Example: transaction quantity of 1.0</summary>
		NM,
		///<summary>Person Location.  ^^^^^Person Location Type^^^Location Description.  Example: ^^^^^S^^^West Salem Elementary.  S=site (or grade school) description.</summary>
		PL,
		///<summary>Parent Result Link.</summary>
		PRL,
		///<summary>Processing type.  Examples: P-Production, T-Test.</summary>
		PT,
		///<summary>Sequence ID.  Example: for repeating segments number that begins with 1.</summary>
		SI,
		///<summary>String, alphanumeric.  Example: SSN or patient.FeeSchedule</summary>
		ST,
		///<summary>Timing quantity.  Example: for eCW appointment ^^duration^startTime^endTime like ^^1200^20120101112000^20120101114000 for 20 minute (1200 second) appointment starting at 11:20 on 01/01/2012</summary>
		TQ,
		///<summary>Used if the data type for a field can vary based on other factors.  For example, when the field is a test observation value,
		///the data type depends on the type of test and how the results of the test are reported.</summary>
		Varied,
		///<summary>Version identifier.  Example: 2.3</summary>
		VID,
		///<summary>Extended address.  Example: Addr1^Addr2^City^State^Zip like 120 Main St.^Suite 3^Salem^OR^97302</summary>
		XAD,
		///<summary>Extended composite ID number and name for person.  Example: provider.EcwID^provider.LName^provider.FName^provider.MI</summary>
		XCN,
		///<summary>Extended composite ID number and name for organizations.</summary>
		XON,
		///<summary>Extended person name.  Composite data type.  Example: LName^FName^MI).</summary>
		XPN,
		///<summary>Extended telecommunication number.  Example: 5033635432</summary>
		XTN
	}

	
}
