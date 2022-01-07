using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness.HL7 {
	///<summary></summary>
	public class InternalHL7v2_6 {

		public static HL7Def GetDeepInternal(HL7Def def) {
			//ok to pass in null
			if(def==null) {//wasn't in the database
				def=new HL7Def();
				def.IsNew=true;
				def.Description="HL7 version 2.6";
				def.ModeTx=ModeTxHL7.File;
				def.IncomingFolder="";
				def.OutgoingFolder="";
				def.IncomingPort="";
				def.OutgoingIpPort="";
				def.SftpInSocket="";
				def.SftpUsername="";
				def.SftpPassword="";
				def.FieldSeparator="|";
				def.ComponentSeparator="^";
				def.SubcomponentSeparator="&";
				def.RepetitionSeparator="~";
				def.EscapeCharacter=@"\";
				def.IsInternal=true;
				def.InternalType=HL7InternalType.HL7v2_6;
				def.InternalTypeVersion=Assembly.GetAssembly(typeof(Db)).GetName().Version.ToString();
				def.IsEnabled=false;
				def.Note="";
				def.ShowDemographics=HL7ShowDemographics.ChangeAndAdd;
				def.ShowAccount=true;
				def.ShowAppts=true;
				def.IsQuadAsToothNum=false;
			}
			def.hl7DefMessages=new List<HL7DefMessage>();
			HL7DefMessage msg=new HL7DefMessage();
			HL7DefSegment seg=new HL7DefSegment();
			#region Inbound Messages
				#region ACK - General Acknowledgment
				def.AddMessage(msg,MessageTypeHL7.ACK,MessageStructureHL7.ADT_A01,InOutHL7.Incoming,0);
					#region MSH - Message Header
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.8, Message Type
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						seg.AddField(9,"messageControlId");
					#endregion MSH - Message Header
					#region MSA - Message Acknowledgment
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.MSA);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSA.1, Acknowledgment Code
						seg.AddField(1,"ackCode");
						//MSA.2, Message Control ID
						seg.AddField(2,"messageControlId");
					#endregion MSA - Message Acknowledgment
				#endregion ACK - General Acknowledgment
				#region ADT - Patient Demographics (Admits, Discharges, and Transfers)
				msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.ADT,MessageStructureHL7.ADT_A01,InOutHL7.Incoming,1);
					#region MSH - Message Header
					seg=new HL7DefSegment();
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
					//HL7 documentation says field 1 is Field Separator.  "This field contains the separator between the segment ID and the first real field.  As such it serves as the separator and defines the character to be used as a separator for the rest of the message." (HL7 v2.6 documentation) The separator is usually | (pipes) and is part of field 0, which is the segment ID followed by a |.  Encoding Characters is the first real field, so it will be numbered starting with 1 in our def.
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.8, Message Type
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						seg.AddField(9,"messageControlId");
					#endregion MSH - Message Header
					#region PID - Patient Identification
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,SegmentNameHL7.PID);//order 2 since the EVN segment is order 1.  We don't process anything out of the EVN segment, so only defined for outgoing messages
						//Fields-------------------------------------------------------------------------------------------------------------
						//PID.2, Patient ID (retained for backward compatibility only).  Defined as 'external' ID but we've always used it as our PatNum for eCW.
						//The standard will be to continue to use PID.2 as the PatNum for incoming messages and attempt to locate the patient if this field is populated.
						seg.AddField(2,"pat.PatNum");
						//PID.3, Patient Identifier List, contains a list of identifiers separated by the repetition character (usually ~)
						//We will use the OID root for the office, stored in the table oidinternal with IDType Patient.
						//The Patient ID data type CX contains the Assigning Authority data type HD - Heirarchic Designator.
						//Within the Assigning Authority, sub-component 2 is the Universal ID that needs to have the OID root for the office.
						//Sub-component 3 of the Assigning Authority, Universal ID Type, will be 'HL7' for our PatNums since our root is registered with HL7.
						//The Patient ID CX type will also have the Identifier Type Code of 'PI', Patient internal identifier.
						//Once located, PatNum is the first component
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&OIDType^PI|
						seg.AddField(3,"patientIds");
						//PID.4, Alternate Patient ID, (retained for backward compatibility only).
						//We've used PID.4 for ChartNumber in the past and will continue to use this as a backup method for locating a patient if the ID in PID.2 is not present
						//or if no patient can be found using that value.
						seg.AddField(4,"pat.ChartNumber");
						//PID.5, Patient Name
						//This will contain the last, first, and middle names as well as the title
						//Example:  LName^FName^MiddleI^^Title
						seg.AddField(5,"pat.nameLFM");
						//PID.7, Date/Time of Birth
						seg.AddField(7,"pat.birthdateTime");
						//PID.8, Administrative Sex
						seg.AddField(8,"pat.Gender");
						//PID.10, Race
						//The old race parsing will still work and matches to an exact string to find the race.  The string can be the only component of the field or the first of many.
						//The old parse translates the string to the PatRaceOld enum, then reconciles them into one or more of the PatRace enums, then adds/deletes entries in the patientrace table.
						//The old race strings are: "American Indian Or Alaska Native", "Asian", "Native Hawaiian or Other Pacific", "Black or African American", "White", "Hispanic", "Other Race"
						//The new race parse accepts the CWE - Coded with Exceptions data type with 9 fields.
						//The 9 fields are: Code^Description^CodeSystem^AlternateCode^AltDescript^AltCodeSystem^CodeSystemVersionID^AltCodeSystemVersionID^OriginalText
						//We will make sure CodeSystem, PID.10.2, is "CDCREC" and use Code, PID.10.0, to find the correct race from the new PatRace enum and update the patientrace table.
						//The race field can repeat, so any number of races can be sent and will add to the patientrace table.
						seg.AddField(10,"pat.Race");
						//PID.11, Patient Address
						//PID.11.20, comment, is used to populate patient.AddrNote with '\.br\' signaling a line break. (the '\' is the default escape character, may be different and parses correctly)
						//Example: ...^^Emergency Contact: Mom Test\.br\Mother\.br\(503)555-1234
						seg.AddField(11,"pat.addressCityStateZip");
						//PID.13, Phone Number - Home, XTN data type (can repeat)
						//This field will also contain the patient's email address as a repetition as well as the WirelessPhone as a repetition
						//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
						//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
						seg.AddField(13,"pat.HmPhone");
						//PID.14, Phone Number - Business, XTN data type (can repeat)
						//We will send just one repetition, the WkPhone, but we will adhere to the data type
						//WPN=Work Number
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(14,"pat.WkPhone");
						//PID.16, Marital Status
						seg.AddField(16,"pat.Position");
						//PID.19, SSN - Patient
						seg.AddField(19,"pat.SSN");
					#endregion PID - Patient Identification
					#region PV1 - Patient Visit
					seg=new HL7DefSegment();
					msg.AddSegment(seg,3,SegmentNameHL7.PV1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PV1.2, Patient Class, IS data type (coded value for user-defined tables)
						//If this field is populated, it will set the patient.GradeLevel if the field can be converted to an integer between 1 and 12
						seg.AddField(2,"pat.GradeLevel");
						//PV1.3, Assigned Patient Location, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						//Used to set patient.ClinicNum if the location description matches clinic.Description
						seg.AddField(3,"pat.location");
						//PV1.7, Attending/Primary Care Doctor, XCN data type, ProviderId^LastName^FirstName^MI
						//In the PV1 segment, the primary provider will be set for the patient
						//The ProviderID component will hold the office's OID for a provider+"."+ProvNum if they send us a ProviderID that we have previously sent them
						//If the first component is not the provider root, we will assume this is an external ID and store it in the oidexternals linked to our internal ProvNum
						seg.AddField(7,"prov.provIdNameLFM");
						//PV1.11, Temporary Location, PL - Person Location data type: PointOfCare^^^^^Person Location Type
						//Using the location type field to identify this field as a "Site (or Grade School)", we will attempt to match the description to a site.Description in the db
						//If exact match is found, set the patient.SiteNum=site.SiteNum
						//Example: |West Salem Elementary^^^^^S| ('S' for site)
						seg.AddField(11,"pat.site");
						//PV1.18, Patient Type, 0 - Unknown, 1 - NoProblems, 2 - NeedsCare, 3 - Urgent
						//Stored in the patient.Urgency field for treatment urgnecy, used in public health screening
						seg.AddField(18,"pat.Urgency");
					#endregion PV1 - Patient Visit
					#region GT1 - Guarantor
					seg=new HL7DefSegment();
					msg.AddSegment(seg,4,false,true,SegmentNameHL7.GT1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//GT1.2, Guarantor Number
						//This field is repeatable and will be handled the same as the patientIdList in the PID segment, so it may contain multiple CX data type identifiers that point to a patient.
						seg.AddField(2,"guarIdList");
						//GT1.3, Guarantor Name
						//This will contain the last, first, and middle names as well as the title
						//Example:  LName^FName^MiddleI^^Title
						seg.AddField(3,"guar.nameLFM");
						//GT1.5, Guarantor Address
						//GT1.5.20, comment, is used to populate patient.AddrNote with '\.br\' signaling a line break.
						//The '\' is the default escape character, may be different in the incoming message and parses correctly
						//Example: ...^^Emergency Contact: Mom Test\.br\Mother\.br\(503)555-1234
						seg.AddField(5,"guar.addressCityStateZip");
						//GT1.6, Guarantor Phone Number - Home
						seg.AddField(6,"guar.HmPhone");
						//GT1.7, Guarantor Phone Number - Business
						seg.AddField(7,"guar.WkPhone");
						//GT1.8, Guarantor Date/Time of Birth
						seg.AddField(8,"guar.birthdateTime");
						//GT1.9, Guarantor Administrative Sex
						seg.AddField(9,"guar.Gender");
						//GT1.12, Guarantor SSN
						seg.AddField(12,"guar.SSN");
					#endregion GT1 - Guarantor
					#region OBX - Observation/Result
					seg=new HL7DefSegment();
					msg.AddSegment(seg,5,true,true,SegmentNameHL7.OBX);
						//Fields-------------------------------------------------------------------------------------------------------------
						//OBX.3, Observation ID, ID^Descript^CodeSystem
						//For adding current medications, the value will be a CWE (OBX.2 should hold the value type 'CWE') with RXNORM as the code system and the RxCui for the ID
						//RxNorm Code^^RXNORM, the Description is ignored
						seg.AddField(3,"medicationRxNorm");
					#endregion OBX - Observation/Result
					#region AL1 - Allergy Information
					seg=new HL7DefSegment();
					msg.AddSegment(seg,6,true,true,SegmentNameHL7.AL1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//AL1.2, Allergen Type Code, DA - Drug allergy, FA - Food allergy, MA - Miscellaneous Allergy
						//For now, we only allow DA for drug allergies
						seg.AddField(2,"allergenType");
						//AL1.3, Allergen Code, currently only RxNorm codes are supported
						//Supplied in CWE data type format with 9 components, of which we only need 2 code^^codeSystem^^^^^^
						//codeSystem must be RxNorm, code must match an existing allergydef.MedicationNum
						seg.AddField(3,"allergenRxNorm");
					#endregion AL1 - Allergy Information
					#region PR1 - Procedures
					seg=new HL7DefSegment();
					msg.AddSegment(seg,7,true,true,SegmentNameHL7.PR1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PR1.3, Procedure Code, CNE data type
						//ProcCode^descript (ignored)^CD2^^^^2014^^Layman Term (ignored)
						//Example: D1351^^CD2^^^^2014
						seg.AddField(3,"proccode.ProcCode");
						//PR1.4, Procedure Description, FT data type (formatted text)
						//Populates the procedure note field (stored in the procnote table). New lines will be the sequence \.br\. Other HL7 reserved characters,
						//namely '|\^~&' (pipe, backslash, caret, tilde, and ampersand respectively), will be escaped with the '\' (backslash) character.
						seg.AddField(4,"proc.Note");
						//PR1.5, Procedure Date/Time, DTM
						seg.AddField(5,"proc.procDateTime");
						//PR1.6, Procedure Functional Type, IS data type (coded value for user-defined tables)
						//Procedure status, sent exactly as displayed by the enum.ToString() (not case sensitive).
						//Supported values: TP=TreatmentPlanned, C=Complete, EO=ExistingOtherProv, EC=ExistingCurrentProv, R=ReferredOut, Cn=Condition
						seg.AddField(6,"proc.Status");
						//PR1.16, Procedure Code Modifier, CNE data type
						//This will hold the treatment area for the procedure (tooth,tooth/surf,quad,sextant,arch,tooth range).
						//If this is empty or blank, default will be mouth and the proccode sent must have a treatment area of mouth or the code will not be inserted.
						//We will validate the information conforms to the tooth numbering system in use by the office and will handle international toothnums.
						//If it is for a tooth surface, it will be 2 subcomponents, toothnum&surface(s)
						//Quad, sextant, and arch will be in subcomponent 2 with the first subcomponent blank
						//Tooth range will be comma-delimited list of tooth numbers in subcomponent 1
						//Examples: 1,2,3,4 or 12&MODL or &UL or &2 (sextant 2)
						seg.AddField(16,"proc.toothSurfRange");
						//PR1.19, Procedure Identifier, EI data type
						//Id^^UniversalId
						//They should send us a unique ID for this procedure with their UniversalId root for a procedure object.
						//If we get this data, we will store our procedurelog.ProcNum linked to the external ID and root in the oidexternals table.
						//This will be useful in preventing multiple copies of the same procedure from being inserted due to duplicate messages.
						//However, if they do not specify a unique ID and root, or if it is not in our oidexternals table linked to our ProcNum with type of Procedure, we will insert a new one.
						//This field could be blank and we will insert a new procedure every time, regardless of whether or not there is already a procedure with this code for this patient.
						seg.AddField(19,"proc.uniqueId");
					#endregion PR1 - Procedures
				#endregion ADT - Patient Demographics (Admits, Discharges, and Transfers)
				#region PPR - Patient Problem
				msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.PPR,MessageStructureHL7.PPR_PC1,InOutHL7.Incoming,2);
					#region MSH - Message Header
					seg=new HL7DefSegment();
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.8, Message Type
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						seg.AddField(9,"messageControlId");
					#endregion MSH - Message Header
					#region PID - Patient Identification
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.PID);//order 1 since PPR's don't have an EVN segment
						//Fields-------------------------------------------------------------------------------------------------------------
						//PID.2, Patient ID (retained for backward compatibility only).  Defined as 'external' ID but we've always used it as our PatNum for eCW.
						//The standard will be to continue to use PID.2 as the PatNum for incoming messages and attempt to locate the patient if this field is populated.
						seg.AddField(2,"pat.PatNum");
						//PID.3, Patient Identifier List, contains a list of identifiers separated by the repetition character (usually ~)
						//We will use the OID root for the office, stored in the table oidinternal with IDType Patient.
						//The Patient ID data type CX contains the Assigning Authority data type HD - Heirarchic Designator.
						//Within the Assigning Authority, sub-component 2 is the Universal ID that needs to have the OID root for the office.
						//Sub-component 3 of the Assigning Authority, Universal ID Type, will be 'HL7' for our PatNums since our root is registered with HL7.
						//The Patient ID CX type will also have the Identifier Type Code of 'PI', Patient internal identifier.
						//Once located, PatNum is the first component
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&OIDType^PI|
						seg.AddField(3,"patientIds");
						//PID.4, Alternate Patient ID, (retained for backward compatibility only).
						//We've used PID.4 for ChartNumber in the past and will continue to use this as a backup method for locating a patient if the ID in PID.2 is not present
						//or if no patient can be found using that value.
						seg.AddField(4,"pat.ChartNumber");
						//PID.5, Patient Name
						//This will contain the last, first, and middle names as well as the title
						//Example:  LName^FName^MiddleI^^Title
						seg.AddField(5,"pat.nameLFM");
						//PID.7, Date/Time of Birth
						seg.AddField(7,"pat.birthdateTime");
						//PID.8, Administrative Sex
						seg.AddField(8,"pat.Gender");
						//PID.10, Race
						//The old race parsing will still work and matches to an exact string to find the race.  The string can be the only component of the field or the first of many.
						//The old parse translates the string to the PatRaceOld enum, then reconciles them into one or more of the PatRace enums, then adds/deletes entries in the patientrace table.
						//The old race strings are: "American Indian Or Alaska Native", "Asian", "Native Hawaiian or Other Pacific", "Black or African American", "White", "Hispanic", "Other Race"
						//The new race parse accepts the CWE - Coded with Exceptions data type with 9 fields.
						//The 9 fields are: Code^Description^CodeSystem^AlternateCode^AltDescript^AltCodeSystem^CodeSystemVersionID^AltCodeSystemVersionID^OriginalText
						//We will make sure CodeSystem, PID.10.2, is "CDCREC" and use Code, PID.10.0, to find the correct race from the new PatRace enum and update the patientrace table.
						//The race field can repeat, so any number of races can be sent and will add to the patientrace table.
						seg.AddField(10,"pat.Race");
						//PID.11, Patient Address
						//PID.11.20, comment, is used to populate patient.AddrNote with '\.br\' signaling a line break. (the '\' is the default escape character, may be different and parses correctly)
						//Example: ...^^Emergency Contact: Mom Test\.br\Mother\.br\(503)555-1234
						seg.AddField(11,"pat.addressCityStateZip");
						//PID.13, Phone Number - Home, XTN data type (can repeat)
						//This field will also contain the patient's email address as a repetition as well as the WirelessPhone as a repetition
						//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
						//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
						seg.AddField(13,"pat.HmPhone");
						//PID.14, Phone Number - Business, XTN data type (can repeat)
						//We will send just one repetition, the WkPhone, but we will adhere to the data type
						//WPN=Work Number
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(14,"pat.WkPhone");
						//PID.16, Marital Status
						seg.AddField(16,"pat.Position");
						//PID.19, SSN - Patient
						seg.AddField(19,"pat.SSN");
					#endregion  PID - Patient Identification
					#region PV1 - Patient Visit
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,false,true,SegmentNameHL7.PV1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PV1.2, Patient Class, IS data type (coded value for user-defined tables)
						//If this field is populated, it will set the patient.GradeLevel if the field can be converted to an integer between 1 and 12
						seg.AddField(2,"pat.GradeLevel");
						//PV1.3, Assigned Patient Location, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						//Used to set patient.ClinicNum if the location description matches clinic.Description
						seg.AddField(3,"pat.location");
						//PV1.7, Attending/Primary Care Doctor, XCN data type, ProviderId^LastName^FirstName^MI^^Abbr
						//In the PV1 segment, the primary provider will be set for the patient if provider is found by ProviderId or by name and abbr
						//A new provider will not be inserted if not found, the default practice provider will be used
						seg.AddField(7,"prov.provIdNameLFM");
						//PV1.11, Temporary Location, PL - Person Location data type: PointOfCare^^^^^Person Location Type
						//Using the location type field to identify this field as a "Site (or Grade School)", we will attempt to match the description to a site.Description in the db
						//If exact match is found, set the patient.SiteNum=site.SiteNum
						//Example: |West Salem Elementary^^^^^S| ('S' for site)
						seg.AddField(11,"pat.site");
						//PV1.18, Patient Type, 0 - Unknown, 1 - NoProblems, 2 - NeedsCare, 3 - Urgent
						//Stored in the patient.Urgency field for treatment urgnecy, used in public health screening
						seg.AddField(18,"pat.Urgency");
					#endregion PV1 - Patient Visit
					#region PRB - Problem Detail
					seg=new HL7DefSegment();
					msg.AddSegment(seg,3,true,false,SegmentNameHL7.PRB);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PRB.1, Action Code, AD-ADD,CO-Correct,DE-Delete,LI-Link,UC-Unchanged,UN-Unlink,UP-Update.  AD/UP are currently supported and are treated the same
						seg.AddField(1,"problemAction");
						//PRB.2, Action Date/Time, DTM data type
						seg.AddField(2,"dateTime.Now");
						//PRB.3, Problem ID, CWE data type
						//Currently only SNOMEDCT codes are supported, code^descript^codeSystem (descript is not required and is ignored)
						//Example: 1234^^SNM
						seg.AddField(3,"problemCode");
						//PRB.4, Problem Instance ID, EI data type
						//Uniquely identifies this instance of a problem
						//If we were to send this, it would be the oidinternal root for problems (root+".5") with the disease.DiseaseNum as the ID
						//We expect the sending software to send an ID with an assigning authority root ID and we will link that to our disease.DiseaseNum in the oidexternals table
						//Example: |76543^^OtherSoftwareRoot.ProblemOID|
						seg.AddField(4,"problemUniqueId");
						//PRB.7, Problem Established Date/Time
						seg.AddField(7,"problemStartDate");
						//PRB.9, Actual Problem Resolution Date/Time
						seg.AddField(9,"problemStopDate");
					#endregion PRB - Problem Detail
				#endregion PPR - Patient Problem
				#region SRM - Schedule Request
				//The only incoming SRM event types we will accept by default will be S03 - Request Appointment Modification or S04 - Request Appointment Cancellation (for now)
				//The message must refer to an appointment already existing in OD by AptNum
				//The S03 message will only be allowed to modify a limited amount of information about the appointment
				//The S04 message will set the appointment status to Broken (enum 5)
				msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.SRM,MessageStructureHL7.SRM_S01,InOutHL7.Incoming,3);
					#region MSH - Message Header
					seg=new HL7DefSegment();
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.8, Message Type
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						seg.AddField(9,"messageControlId");
					#endregion MSH - Message Header
					#region ARQ - Appointment Request Information
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.ARQ);
						//Fields-------------------------------------------------------------------------------------------------------------
						//ARQ.1, Placer Appointment ID.  This will be the other software appointment ID.
						//We will store this in the oidexternals table linked to our AptNum.
						//The first component will be the external application appointment ID (stored as IDExternal)
						//The third component will be the external application assigning authority root ID (stored as RootExternal)
						//Example: |12345^^OtherSoftware.Root^|
						seg.AddField(1,"apt.externalAptID");
						//ARQ.2, Filler Appointment ID, EI data type, EntityID^NamespaceID^UniversalID^UniversalIDType
						//We will expect the first component to be our AptNum
						//We could check to make sure the UniversalID is our OID for an appointment, but for this incoming segment we won't enforce it for now and just accept the first component as AptNum
						//Example: 1234^^2.16.840.1.113883.3.4337.1486.6566.6^HL7
						seg.AddField(2,"apt.AptNum");
					#endregion ARQ - Appointment Request Information
					#region NTE - Notes and Comments
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,true,true,SegmentNameHL7.NTE);
						//Fields-------------------------------------------------------------------------------------------------------------
						//NTE.3, Comment, FT data type (formatted text)
						//We will append this comment to appointment.Note if the Note for this appointment does not already contain the exact text received
						//This is formatted text, so we will allow new lines.
						//As in the address note field (see PID.11) we will accept '\.br\' (where the '\' is the defined escape char, \ by default) to signal a new line.
						seg.AddField(3,"apt.Note");
					#endregion NTE - Notes and Comments
					#region PID - Patient Identification
					seg=new HL7DefSegment();
					msg.AddSegment(seg,3,SegmentNameHL7.PID);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PID.2, Patient ID (retained for backward compatibility only).  Defined as 'external' ID but we've always used it as our PatNum for eCW.
						//The standard will be to continue to use PID.2 as the PatNum for incoming messages and attempt to locate the patient if this field is populated.
						seg.AddField(2,"pat.PatNum");
						//PID.3, Patient Identifier List, contains a list of identifiers separated by the repetition character (usually ~)
						//We will use the OID root for the office, stored in the table oidinternal with IDType Patient.
						//The Patient ID data type CX contains the Assigning Authority data type HD - Heirarchic Designator.
						//Within the Assigning Authority, sub-component 2 is the Universal ID that needs to have the OID root for the office.
						//Sub-component 3 of the Assigning Authority, Universal ID Type, will be 'HL7' for our PatNums since our root is registered with HL7.
						//The Patient ID CX type will also have the Identifier Type Code of 'PI', Patient internal identifier.
						//Once located, PatNum is the first component
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&OIDType^PI|
						seg.AddField(3,"patientIds");
						//PID.4, Alternate Patient ID, (retained for backward compatibility only).
						//We've used PID.4 for ChartNumber in the past and will continue to use this as a backup method for locating a patient if the ID in PID.2 is not present
						//or if no patient can be found using that value.
						seg.AddField(4,"pat.ChartNumber");
						//PID.5, Patient Name
						//This will contain the last, first, and middle names as well as the title
						//Example:  LName^FName^MiddleI^^Title
						seg.AddField(5,"pat.nameLFM");
						//PID.7, Date/Time of Birth
						seg.AddField(7,"pat.birthdateTime");
						//PID.8, Administrative Sex
						seg.AddField(8,"pat.Gender");
						//PID.10, Race
						//The old race parsing will still work and matches to an exact string to find the race.  The string can be the only component of the field or the first of many.
						//The old parse translates the string to the PatRaceOld enum, then reconciles them into one or more of the PatRace enums, then adds/deletes entries in the patientrace table.
						//The old race strings are: "American Indian Or Alaska Native", "Asian", "Native Hawaiian or Other Pacific", "Black or African American", "White", "Hispanic", "Other Race"
						//The new race parse accepts the CWE - Coded with Exceptions data type with 9 fields.
						//The 9 fields are: Code^Description^CodeSystem^AlternateCode^AltDescript^AltCodeSystem^CodeSystemVersionID^AltCodeSystemVersionID^OriginalText
						//We will make sure CodeSystem, PID.10.2, is "CDCREC" and use Code, PID.10.0, to find the correct race from the new PatRace enum and update the patientrace table.
						//The race field can repeat, so any number of races can be sent and will add to the patientrace table.
						seg.AddField(10,"pat.Race");
						//PID.11, Patient Address
						//PID.11.20, comment, is used to populate patient.AddrNote with '\.br\' signaling a line break. (the '\' is the default escape character, may be different and parses correctly)
						//Example: ...^^Emergency Contact: Mom Test\.br\Mother\.br\(503)555-1234
						seg.AddField(11,"pat.addressCityStateZip");
						//PID.13, Phone Number - Home, XTN data type (can repeat)
						//This field will also contain the patient's email address as a repetition as well as the WirelessPhone as a repetition
						//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
						//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
						seg.AddField(13,"pat.HmPhone");
						//PID.14, Phone Number - Business, XTN data type (can repeat)
						//We will send just one repetition, the WkPhone, but we will adhere to the data type
						//WPN=Work Number
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(14,"pat.WkPhone");
						//PID.16, Marital Status
						seg.AddField(16,"pat.Position");
						//PID.19, SSN - Patient
						seg.AddField(19,"pat.SSN");
					#endregion PID - Patient Identification
					#region PV1 - Patient Visit
					seg=new HL7DefSegment();
					msg.AddSegment(seg,4,false,true,SegmentNameHL7.PV1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PV1.2, Patient Class, IS data type (coded value for user-defined tables)
						//If this field is populated, it will set the patient.GradeLevel if the field can be converted to an integer between 1 and 12
						seg.AddField(2,"pat.GradeLevel");
						//PV1.3, Assigned Patient Location, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						//Used to set patient.ClinicNum if the location description matches clinic.Description
						seg.AddField(3,"pat.location");
						//PV1.7, Attending/Primary Care Doctor, XCN data type, ProviderId^LastName^FirstName^MI
						//In the PV1 segment, the primary provider will be set for the patient and the appointment
						//If there is an AIL or AIG segment, the appointment.ProvNum will be set to the provider referenced in that segment.
						seg.AddField(7,"prov.provIdNameLFM");
						//PV1.11, Temporary Location, PL - Person Location data type: PointOfCare^^^^^Person Location Type
						//Using the location type field to identify this field as a "Site (or Grade School)", we will attempt to match the description to a site.Description in the db
						//If exact match is found, set the patient.SiteNum=site.SiteNum
						//Example: |West Salem Elementary^^^^^S| ('S' for site)
						seg.AddField(11,"pat.site");
						//PV1.18, Patient Type, 0 - Unknown, 1 - NoProblems, 2 - NeedsCare, 3 - Urgent
						//Stored in the patient.Urgency field for treatment urgnecy, used in public health screening
						seg.AddField(18,"pat.Urgency");
					#endregion PV1 - Patient Visit
					#region AIG - Appointment Information - General Resource
					//This segment is for setting the provider(s) on the appointment and setting the confirmation status
					//It is repeatable so that the user can send one to set the dentist and one to set the hygienist on the appt
					seg=new HL7DefSegment();
					msg.AddSegment(seg,5,true,true,SegmentNameHL7.AIG);
						//Fields-------------------------------------------------------------------------------------------------------------
						//AIG.3, Resource ID, CWE data type
						//If this is included in the AIG segment for a general resource, we will try to find the provider using the resource ID as the OD ProvNum.
						//If not found by ProvNum, we will try to find the provider using the second component - Text, which should be populated with 'LName, FName',
						//combined with the 4th component - Alternate Identifier, which should be populated with the provider.Abbr.
						//Only if LName, FName, and Abbr all match will we use that provider.
						//When the provider is identified in an AIG or AIP segment, it is used to set the provider on the appointment, not the patient's primary provider.
						//For setting a patient's primary provider, the PV1 segment is used.
						//Example: |1234^Abbott, Sarah^^DrAbbott|
						seg.AddField(3,"prov.provIdName");
						//AIG.4, Resource Type, CWE data type
						//Accepted values are 'd' or 'D' for dentist and 'h' or 'H' for hygienist
						//If included, we will set either the dentist or the hygienist on the appt.
						//If not included, the provider will assumed to refer to the dentist
						seg.AddField(4,"prov.provType");
						//AIG.14, Filler Status Code, CWE data type
						//We will use this to set the confirmation status of the appointment
						//We will use the second component (text) and attempt to match exactly the ItemName (confirm Name) or ItemValue (confirm Abbreviation) from their definition table
						//If there is a match, we will update the confirmation status
						//Example: |^Appointment Confirmed|
						seg.AddField(14,"apt.confirmStatus");
					#endregion AIG - Appointment Information - General Resource
					#region AIL - Appointment Information - Location Resource
					//This segment is used to set the clinic on the appointment.
					//This may differ from the patient's assigned clinic, only the appointment.ClinicNum will be set from this segment.
					//This is not repeatable as we only have one clinic value to assign for the location resource
					seg=new HL7DefSegment();
					msg.AddSegment(seg,6,false,true,SegmentNameHL7.AIL);
						//Fields-------------------------------------------------------------------------------------------------------------
						//AIL.3, Location Resource ID, PL data type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						//Room-Operatory (not set from inbound messages), Facility-Practice (not set from inbound messages), Building-Clinic (used to set apt.ClinicNum)
						//Used to set the appointment.ClinicNum if this is included in the message
						seg.AddField(3,"apt.location");
						//AIL.12, Filler Status Code, CWE data type
						//We will use this to set the confirmation status of the appointment
						//We will use the second component (text) and attempt to match exactly the ItemName (confirm Name) or ItemValue (confirm Abbreviation) from their definition table
						//If there is a match, we will update the confirmation status
						//Example: |^Appointment Confirmed|
						seg.AddField(12,"apt.confirmStatus");
					#endregion AIL - Appointment Information - Location Resource
					#region AIP - Appointment Information - Personnel Resource
					//This segment is used to set the dentist and/or hygienist on an appointment
					//This may differ from the patient's primary provider and it could cause an appointment to be in an operatory with a provider not assigned to that operatory
					seg=new HL7DefSegment();
					msg.AddSegment(seg,7,true,true,SegmentNameHL7.AIP);
						//Fields-------------------------------------------------------------------------------------------------------------
						//AIP.3, Personnel Resource ID, XCN data type
						//ProviderId^LastName^FirstName^MI^Suffix^Prefix
						//According to the HL7 standards, we are free to use the components as needed to identify the provider
						//We will use ProviderID as the OD ProvNum, and if not found in the db we will use LName, FName and the 'Prefix' component as the Abbr
						//Used to set the appointment.ProvNum or appointment.ProvHyg if this is included in the message
						//Field AIP.4 will determine whether it is the dentist or hygienist for the appt
						//This field is repeatable, but if they want to set both the dentist and the hygienist for the same appointment
						//they would have to repeat the whole segment with different provTypes in field 4
						//Example: |1234^Abbott^Sarah^L^DMD^DrAbbott|
						seg.AddField(3,"prov.provIdNameLFM");
						//AIP.4, Resource Type, CWE data type
						//Accepted values are 'd' or 'D' for dentist and 'h' or 'H' for hygienist
						seg.AddField(4,"prov.provType");
						//AIP.12, Filler Status Code, CWE data type
						//We will use this to set the confirmation status of the appointment
						//We will use the second component (text) and attempt to match exactly the ItemName (confirm Name) or ItemValue (confirm Abbreviation) from their definition table
						//If there is a match, we will update the confirmation status
						//Example: |^Appointment Confirmed|
						seg.AddField(12,"apt.confirmStatus");
					#endregion AIP - Appointment Information - Personnel Resource
				#endregion SRM - Schedule Request
			#endregion Inbound Messages
			#region Outbound Messages
				#region ACK - General Acknowledgment
				msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.ACK,MessageStructureHL7.ADT_A01,InOutHL7.Outgoing,4);
					#region MSH - Message Header
					seg=new HL7DefSegment();
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.1, Encoding Characters (DataType.ST)
						seg.AddField(1,"separators^~\\&");
						//MSH.2, Sending Application, HD data type, Namespace ID^Universal ID^Universal ID Type
						//We originally sent 'OD' in this field, but this is a HD data type, so should consist of three components
						//We will send the Open Dental HL7 root assigned to the office and stored in the oidinternal table with the IDType of Root
						//The recommeded value for this field in the table is ^2.16.840.1.113883.3.4337.1486.CustomerPatNum^HL7.
						//If the oidinternal row with IDType=Root does not have an IDRoot assigned, we will revert to sending 'OD'
						seg.AddField(2,"sendingApp");
						//MSH.3, Sending Facility, not used
						//MSH.4, Receiving Application
						//This field will have to be a fixed text field and would have to be modified in a custom definition
						//if they want the outbound messages to have a specific receiving application identifier
						seg.AddField(4,"0361",DataTypeHL7.HD,"","NamespaceID^UniversalID^UniversalIDType");
						//MSH.5, Receiving Facility, not used
						//MSH.6, Message Date and Time (YYYYMMDDHHMMSS)
						seg.AddField(6,"dateTime.Now");
						//MSH.8, Message Type, MSG data type
						//This segment should contain MessageType^EventType^MessageStructure, example SRR^S04^SRR_S04
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						//A GUID that uniquely identifies this message
						seg.AddField(9,"messageControlId");
						//MSH.10, Processing ID, PT data type, Processing ID^Processing Mode
						//Processing mode is optional and not included, we will use P for production (P-Production, T-Training, D-Debugging)
						seg.AddField(10,"0103",DataTypeHL7.PT,"","P");
						//MSH.11, Version ID, VID data type, Version ID^Internationalization Code^International Version ID
						//All components are optional, we will only send Version ID, which is currently 2.6
						seg.AddField(11,"0104",DataTypeHL7.VID,"","2.6");
						//MSH.15, Application Ack Type (AL=Always, NE=Never, ER=Error/reject conditions only, SU=Successful completion only)
						//This is for enhanced acknowledgment mode, which we do not currently support, but we will send our mode just in case
						//With field MSH.15, Accept Ack Type, not present it will default to NE
						//Field MSH.15 set to AL means we will require an ack for every message that is sent, meaning it was processed successfully by the receiving application
						//But MSH.15 not present or null means we do not require a separate accept ACK message, just the accept and validate response meaning the message was accepted and processed
						seg.AddField(15,"0155",DataTypeHL7.ID,"","AL");
					#endregion MSH - Message Header
					#region MSA - Message Acknowledgment
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.MSA);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSA.1, Acknowledgment Code
						seg.AddField(1,"ackCode");
						//MSA.2, Message Control ID
						seg.AddField(2,"messageControlId");
					#endregion MSA - Message Acknowledgment
				#endregion ACK - General Acknowledgment
				#region ADT - Patient Demographics (Admits, Discharges, and Transfers)
				//Outbound ADT messages will be triggered when specific data is changed in OD, but not if the data change is due to an inbound HL7 message.
				//ADT's created when:
				//1. Pressing OK from FormPatientEdit
				//2. Pressing OK from FormPatientAddAll
				//3. Pressing the Set Guarantor button (ContrFamily)
				//4. Pressing the Move button (ContrFamily) to move a patient to a different family
				//5. Retrieving web forms in FormWebForms if a form is for/creates a new patient
				//If the Synch Clone feature is enabled, we would not want the clone to exist in the other software, so we would want to make sure we were not sending ADT's for clones.
				msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.ADT,MessageStructureHL7.ADT_A01,InOutHL7.Outgoing,5);
					#region MSH - Message Header
					seg=new HL7DefSegment();
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.1, Encoding Characters (DataType.ST)
						seg.AddField(1,"separators^~\\&");
						//MSH.2, Sending Application, HD data type, Namespace ID^Universal ID^Universal ID Type
						//We originally sent 'OD' in this field, but this is a HD data type, so should consist of three components
						//We will send the Open Dental HL7 root assigned to the office and stored in the oidinternal table with the IDType of Root
						//The recommeded value for this field in the table is ^2.16.840.1.113883.3.4337.1486.CustomerPatNum^HL7.
						//If the oidinternal row with IDType=Root does not have an IDRoot assigned, we will revert to sending 'OD'
						seg.AddField(2,"sendingApp");
						//MSH.3, Sending Facility, not used
						//MSH.4, Receiving Application
						//This field will have to be a fixed text field and would have to be modified in a custom definition
						//if they want the outbound messages to have a specific receiving application identifier
						seg.AddField(4,"0361",DataTypeHL7.HD,"","NamespaceID^UniversalID^UniversalIDType");
						//MSH.5, Receiving Facility, not used
						//MSH.6, Message Date and Time (YYYYMMDDHHMMSS)
						seg.AddField(6,"dateTime.Now");
						//MSH.8, Message Type, MSG data type
						//This segment should contain MessageType^EventType^MessageStructure, example ADT^A04^ADT_A04
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						//A GUID that uniquely identifies this message
						seg.AddField(9,"messageControlId");
						//MSH.10, Processing ID, PT data type, Processing ID^Processing Mode
						//Processing mode is optional and not included, we will use P for production (P-Production, T-Training, D-Debugging)
						seg.AddField(10,"0103",DataTypeHL7.PT,"","P");
						//MSH.11, Version ID, VID data type, Version ID^Internationalization Code^International Version ID
						//All components are optional, we will only send Version ID, which is currently 2.6
						seg.AddField(11,"0104",DataTypeHL7.VID,"","2.6");
						//MSH.15, Application Ack Type (AL=Always, NE=Never, ER=Error/reject conditions only, SU=Successful completion only)
						//This is for enhanced acknowledgment mode, which we do not currently support, but we will send our mode just in case
						//With field MSH.15, Accept Ack Type, not present it will default to NE
						//Field MSH.15 set to AL means we will require an ack for every message that is sent, meaning it was processed successfully by the receiving application
						//But MSH.15 not present or null means we do not require a separate accept ACK message, just the accept and validate response meaning the message was accepted and processed
						seg.AddField(15,"0155",DataTypeHL7.ID,"","AL");
					#endregion MSH - Message Header
					#region EVN - Event Type
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.EVN);
						//Fields-------------------------------------------------------------------------------------------------------------
						//EVN.1, Event Type, example P03
						//Retained for backward compatibility only, we will not add this field by default
						//EVN.2, Recorded Date/Time
						seg.AddField(2,"dateTime.Now");
						//EVN.4, Event Reason Code (01 - Patient request; 02 - Physician/health practitioner order;03 - Census Management;O - Other;U - Unknown)
						seg.AddField(4,"0062",DataTypeHL7.IS,"","01");
					#endregion EVN - Event Type
					#region PID - Patient Identification
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,SegmentNameHL7.PID);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PID.1, Set ID, SI data type
						//"This field contains the number that identifies this transaction.  For the first occurrence of the segment, the sequence number shall be one" (HL7 v2.6 documentation)
						//We only send 1 PID segment in ADT's so this number will always be 1.
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//PID.2, Patient ID (retained for backward compatibility only).  Defined as 'external' ID, we've always sent the ChartNumber in this field as the eCW patient ID
						//The standard will be to continue to use PID.2 for sending the ChartNumber, which was set from PID.4 of an incoming message.  This could be the other software patient ID.
						seg.AddField(2,"pat.ChartNumber");
						//PID.3, Patient Identifier List, contains a list of identifiers separated by the repetition character (usually ~)
						//PatientID^IDCheckDigit^CheckDigitScheme^AssigningAuthority^IDTypeCode
						//Assigning Authority is a HD data type and is composed of 3 subcomponents, NamespaceID&UniversalID&UniversalIDType
						//Sub-component 2, UniversalID, we will use the OID root for a Patient object stored in the oidinternal table
						//If retrieved from OD it will be 2.16.840.1.113883.3.4337.1486.CustomerNumber.2
						//Sub-component 3, Universal ID Type, will be 'HL7' since our root is registered with HL7.
						//The IDTypeCode will be 'PI', Patient internal identifier.
						//We will also get all of the identifiers in the oidexternals table for the patient and create repetitions for each external ID using the IDExternal and RootExternal
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&^PI|
						seg.AddField(3,"patientIds");
						//PID.4, Alternate Patient ID, (retained for backward compatibility only).
						//We've used PID.4 for sending the OD PatNum in the past and will continue to send this in PID.4
						seg.AddField(4,"pat.PatNum");
						//PID.5, Patient Name
						seg.AddField(5,"pat.nameLFM");
						//PID.7, Date/Time of Birth
						seg.AddField(7,"pat.birthdateTime");
						//PID.8, Administrative Sex
						seg.AddField(8,"pat.Gender");
						//PID.10, Race, CWE - Coded with Exceptions data type
						//This data type is composed of 9 fields, Code^Description^CodeSystem^AlternateCode^AltDescript^AltCodeSystem^CodeSystemVersionID^AltCodeSystemVersionID^OriginalText
						//We will send Code^Description^CodeSystem^^^^CodeSystemVersionID
						//The race field can repeat, so any number of races can be sent and will come from the patientrace table.
						//Example: 2106-3^White^CDCREC^^^^1~2186-5^NotHispanic^CDCREC^^^^1
						seg.AddField(10,"pat.Race");
						//PID.11, Patient Address
						//PID.11.20, comment, will come from the patient.AddrNote column and each new line character will be replaced with '\.br\' where the '\' is the defined escape char
						//Example: 123 Main St^Apt 1^Dallas^OR^97338^^^^^^^^^^^^^^^Emergency Contact: Mom Test1\.br\Mother\.br\(503)623-3072
						seg.AddField(11,"pat.addressCityStateZip");
						//PID.13, Phone Number - Home, XTN data type (can repeat)
						//This field will also contain the patient's email address as a repetition as well as the WirelessPhone as a repetition
						//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
						//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
						seg.AddField(13,"pat.HmPhone");
						//PID.14, Phone Number - Business, XTN data type (can repeat)
						//We will send just one repetition, the WkPhone, but we will adhere to the data type
						//WPN=Work Number
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(14,"pat.WkPhone");
						//PID.16, Marital Status
						//S-Single, M-Married, W-Widowed, D-Divorced
						seg.AddField(16,"pat.Position");
						//PID.19, SSN - Patient
						seg.AddField(19,"pat.SSN");
					#endregion PID - Patient Identification
					#region PV1 - Patient Visit
					seg=new HL7DefSegment();
					msg.AddSegment(seg,3,SegmentNameHL7.PV1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PV1.1, Set ID - PV1
						//See the comment above for the Sequence Number of the PID segment.  Always 1 since we only send one PV1 segment per ADT message.
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//PV1.2, Patient Class, IS data type (coded value for user-defined tables)
						//Suggested values E=Emergency, I=Inpatient, O=Outpatient, P=Preadmit, R=Recurring patient, B=Obstetrics, C=Commercial Account, N=Not Applicable, U=Unkown
						//We will defualt to send 'O' for outpatient for outbound messages, but for incoming we will use this field for pat.GradeLevel if it's an integer from 1-12
						seg.AddField(2,"0004",DataTypeHL7.IS,"","O");
						//PV1.3, Assigned Patient Location, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						seg.AddField(3,"pat.location");
						//PV1.7, Attending/Primary Care Doctor, ProviderId^LastName^FirstName^MI
						seg.AddField(7,"0010",DataTypeHL7.XCN,"prov.provIdNameLFM","");
						//PV1.11, Temporary Location, PL - Person Location data type: PointOfCare^^^^^Person Location Type
						//Filled with the "Site (or Grade School)" value in the database, using 'S' to designate the location type site and the site.Description value
						//If no site assigned in OD, this will be blank as it is optional
						//Example: |West Salem Elementary^^^^^S| ('S' for site)
						seg.AddField(11,"pat.site");
						//PV1.18, Patient Type, IS data type (coded value for user-defined tables)
						//We will send one of the following values retrieved from the patient.Urgency field for treatment urgency: 0 - Unknown, 1 - NoProblems, 2 - NeedsCare, 3 - Urgent
						seg.AddField(18,"pat.Urgency");
						//PV1.19, Visit Number
						//Outbound ADT messages will not have an appointment number, no appointment selected
						//seg.AddField(19,"apt.AptNum");
						//PV1.44, Admit Date/Time
						//Outbound ADT messages will not have a procedure or appt to get the date/time from
						//seg.AddField(44,"proc.procDateTime");
					#endregion PV1 - Patient Visit
					#region IN1 - Insurance
					seg=new HL7DefSegment();
					msg.AddSegment(seg,4,true,true,SegmentNameHL7.IN1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//IN1.1, Set ID (starts with 1)
						//This will be 1 for primary ins and increment for additional repetitions for secondary ins etc.
						seg.AddField(1,"sequenceNum");
						//IN1.2, Insurance Plan ID, CWE data type
						//We do not have unique ID's for insurance plans.
						//We have created an oidinternal ID for insplan, root+.7, and will use this with the insplan.PlanNum to uniquely identify an insplan.
						//Example: 2.16.840.1.113883.3.4337.1486.6566.7.1234
						seg.AddField(2,"insplan.planNum");
						//IN1.3, Insurance Company ID, CX data type
						seg.AddField(3,"carrier.ElectID");
						//IN1.4, Insurance Company Name, XON data type
						seg.AddField(4,"carrier.CarrierName");
						//IN1.5, Insurance Company Address, XAD data type
						//carrier.Address^Address2^City^State^Zip
						seg.AddField(5,"carrier.addressCityStateZip");
						//IN1.7, Insurance Company Phone Number, XTN data type
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(7,"carrier.Phone");
						//IN1.8, Group Number, ST data type
						seg.AddField(8,"insplan.GroupNum");
						//IN1.9, Group Name, XON data type
						seg.AddField(9,"insplan.GroupName");
						//IN1.11, Insured's Group Employer Name, XON data type
						//insplan.EmployerNum to employer.EmpName
						seg.AddField(11,"insplan.empName");
						//IN1.12, Plan Effective Date, DT data type
						seg.AddField(12,"inssub.DateEffective");
						//IN1.13, Plan Expiration Date, DT data type
						seg.AddField(13,"inssub.DateTerm");
						//IN1.15, Plan Type, IS data type
						//insplan.PlanType, Category Percentage, PPO Percentage, Medicaid or Flat Copay, Capitation
						seg.AddField(15,"insplan.PlanType");
						//IN1.16, Name of Insured, XPN data type
						//insplan.InsSubNum->inssub.Subscriber->patient name
						seg.AddField(16,"inssub.subscriberName");
						//IN1.17, Insured's Relationship to Patient, CWE data type
						//SEL-Self, SPO-Spouse, DOM-LifePartner, CHD-Child (PAR-Parent), EME-Employee (EMR-Employer)
						//DEP-HandicapDep (GRD-Guardian), DEP-Dependent, OTH-SignifOther (OTH-Other), OTH-InjuredPlantiff
						//We store relationship to subscriber and they want subscriber's relationship to patient, therefore
						//Relat.Child will return "PAR" for Parent, Relat.Employee will return "EMR" for Employer, and Relat.HandicapDep and Relat.Dependent will return "GRD" for Guardian
						seg.AddField(17,"patplan.subRelationToPat");
						//IN1.18, Insured's Date of Birth, DTM data type
						seg.AddField(18,"inssub.subBirthdate");
						//IN1.19, Insured's Address, XAD data type
						seg.AddField(19,"inssub.subAddrCityStateZip");
						//IN1.20, Assignment of Benefits, IS data type
						//inssub.AssignBen 1-Y, 0-N
						seg.AddField(20,"inssub.AssignBen");
						//IN1.21, Coordination of Benefits, IS data type
						//CO-Coordination, IN-Independent.  Will be CO if more than one patplan
						seg.AddField(21,"insplan.cob");
						//IN1.22, Coordination of Benefits Priority, ST data type
						//1-Primary, 2-Secondary, 3,4,5...
						seg.AddField(22,"patplan.Ordinal");
						//IN1.27, Release of Information Code, IS data type
						//inssub.ReleaseInfo, 1-Y, 2-N
						seg.AddField(27,"inssub.ReleaseInfo");
						//IN1.36, Policy Number, ST data type
						//patplan.PatID, if blank then inssub.SubscriberID
						seg.AddField(36,"patplan.policyNum");
						//IN1.47, Coverage Type, IS data type
						//D-Dental, M-Medical using insplan.IsMedical flag
						seg.AddField(47,"insplan.coverageType");
						//IN1.49, Insured's ID Number, CX data type
						seg.AddField(49,"inssub.SubscriberID");
					#endregion IN1 - Insurance
				#endregion ADT - Patient Demographics (Admits, Discharges, and Transfers)
				#region DFT - Detailed Financial Transaction
				//Outbound DFT messages will be triggered by pressing the tool bar button for this enabled definition.
				//In the future there may be automation for sending these when procedures are set complete.
				msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.DFT,MessageStructureHL7.DFT_P03,InOutHL7.Outgoing,6);
					#region MSH - Message Header
					seg=new HL7DefSegment();
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.1, Encoding Characters (DataType.ST)
						seg.AddField(1,"separators^~\\&");
						//MSH.2, Sending Application, HD data type, Namespace ID^Universal ID^Universal ID Type
						//We originally sent 'OD' in this field, but this is a HD data type, so should consist of three components
						//We will send the Open Dental HL7 root assigned to the office and stored in the oidinternal table with the IDType of Root
						//The recommeded value for this field in the table is ^2.16.840.1.113883.3.4337.1486.CustomerPatNum^HL7.
						//If the oidinternal row with IDType=Root does not have an IDRoot assigned, we will revert to sending 'OD'
						seg.AddField(2,"sendingApp");
						//MSH.3, Sending Facility, not used
						//MSH.4, Receiving Application
						//This field will have to be a fixed text field and would have to be modified in a custom definition
						//if they want the outbound messages to have a specific receiving application identifier
						seg.AddField(4,"0361",DataTypeHL7.HD,"","NamespaceID^UniversalID^UniversalIDType");
						//MSH.5, Receiving Facility, not used
						//MSH.6, Message Date and Time (YYYYMMDDHHMMSS)
						seg.AddField(6,"dateTime.Now");
						//MSH.8, Message Type, MSG data type
						//This segment should contain MessageType^EventType^MessageStructure, example DFT^P03^DFT_P03
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						//A GUID that uniquely identifies this message
						seg.AddField(9,"messageControlId");
						//MSH.10, Processing ID, PT data type, Processing ID^Processing Mode
						//Processing mode is optional and not included, we will use P for production (P-Production, T-Training, D-Debugging)
						seg.AddField(10,"0103",DataTypeHL7.PT,"","P");
						//MSH.11, Version ID, VID data type, Version ID^Internationalization Code^International Version ID
						//All components are optional, we will only send Version ID, which is currently 2.6
						seg.AddField(11,"0104",DataTypeHL7.VID,"","2.6");
						//MSH.15, Application Ack Type (AL=Always, NE=Never, ER=Error/reject conditions only, SU=Successful completion only)
						//This is for enhanced acknowledgment mode, which we do not currently support, but we will send our mode just in case
						//With field MSH.15, Accept Ack Type, not present it will default to NE
						//Field MSH.15 set to AL means we will require an ack for every message that is sent, meaning it was processed successfully by the receiving application
						//But MSH.15 not present or null means we do not require a separate accept ACK message, just the accept and validate response meaning the message was accepted and processed
						seg.AddField(15,"0155",DataTypeHL7.ID,"","AL");
					#endregion MSH - Message Header
					#region EVN - Event Type
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.EVN);
						//Fields-------------------------------------------------------------------------------------------------------------
						//EVN.1, Event Type, example P03
						//Retained for backward compatibility only, we will not add this field by default
						//EVN.2, Recorded Date/Time
						seg.AddField(2,"dateTime.Now");
						//EVN.4, Event Reason Code (01 - Patient request; 02 - Physician/health practitioner order;03 - Census Management;O - Other;U - Unknown)
						seg.AddField(4,"0062",DataTypeHL7.IS,"","01");
					#endregion EVN - Event Type
					#region PID - Patient Identification
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,SegmentNameHL7.PID);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PID.1, Set ID, SI data type
						//"This field contains the number that identifies this transaction.  For the first occurrence of the segment, the sequence number shall be one" (HL7 v2.6 documentation)
						//We only send 1 PID segment in DFT's so this number will always be 1.
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//PID.2, Patient ID (retained for backward compatibility only).  Defined as 'external' ID, we've always sent the ChartNumber in this field as the eCW patient ID
						//The standard will be to continue to use PID.2 for sending the ChartNumber, which was set from PID.4 of an incoming message.  This could be the other software patient ID.
						seg.AddField(2,"pat.ChartNumber");
						//PID.3, Patient Identifier List, contains a list of identifiers separated by the repetition character (usually ~)
						//PatientID^IDCheckDigit^CheckDigitScheme^AssigningAuthority^IDTypeCode
						//Assigning Authority is a HD data type and is composed of 3 subcomponents, NamespaceID&UniversalID&UniversalIDType
						//Sub-component 2, UniversalID, we will use the OID root for a Patient object stored in the oidinternal table
						//If retrieved from OD it will be 2.16.840.1.113883.3.4337.1486.CustomerNumber.2
						//Sub-component 3, Universal ID Type, will be 'HL7' since our root is registered with HL7.
						//The IDTypeCode will be 'PI', Patient internal identifier.
						//We will also get all of the identifiers in the oidexternals table for the patient and create repetitions for each external ID using the IDExternal and RootExternal
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&^PI|
						seg.AddField(3,"patientIds");
						//PID.4, Alternate Patient ID, (retained for backward compatibility only).
						//We've used PID.4 for sending the OD PatNum in the past and will continue to send this in PID.4
						seg.AddField(4,"pat.PatNum");
						//PID.5, Patient Name
						seg.AddField(5,"pat.nameLFM");
						//PID.7, Date/Time of Birth
						seg.AddField(7,"pat.birthdateTime");
						//PID.8, Administrative Sex
						seg.AddField(8,"pat.Gender");
						//PID.10, Race, CWE - Coded with Exceptions data type
						//This data type is composed of 9 fields, Code^Description^CodeSystem^AlternateCode^AltDescript^AltCodeSystem^CodeSystemVersionID^AltCodeSystemVersionID^OriginalText
						//We will send Code^Description^CodeSystem^^^^CodeSystemVersionID
						//The race field can repeat, so any number of races can be sent and will come from the patientrace table.
						//Example: 2106-3^White^CDCREC^^^^1~2186-5^NotHispanic^CDCREC^^^^1
						seg.AddField(10,"pat.Race");
						//PID.11, Patient Address
						//PID.11.20, comment, will come from the patient.AddrNote column and each new line character will be replaced with '\.br\' where the '\' is the defined escape char
						//Example: 123 Main St^Apt 1^Dallas^OR^97338^^^^^^^^^^^^^^^Emergency Contact: Mom Test1\.br\Mother\.br\(503)623-3072
						seg.AddField(11,"pat.addressCityStateZip");
						//PID.13, Phone Number - Home, XTN data type (can repeat)
						//This field will also contain the patient's email address as a repetition as well as the WirelessPhone as a repetition
						//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
						//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
						seg.AddField(13,"pat.HmPhone");
						//PID.14, Phone Number - Business, XTN data type (can repeat)
						//We will send just one repetition, the WkPhone, but we will adhere to the data type
						//WPN=Work Number
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(14,"pat.WkPhone");
						//PID.16, Marital Status
						//S-Single, M-Married, W-Widowed, D-Divorced
						seg.AddField(16,"pat.Position");
						//PID.19, SSN - Patient
						seg.AddField(19,"pat.SSN");
					#endregion PID - Patient Identification
					#region PV1 - Patient Visit
					seg=new HL7DefSegment();
					msg.AddSegment(seg,3,false,true,SegmentNameHL7.PV1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PV1.1, Set ID - PV1
						//See the comment above for the Sequence Number of the PID segment.  Always 1 since we only send one PV1 segment per DFT message.
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//PV1.2, Patient Class, IS data type (coded value for user-defined tables)
						//Suggested values E=Emergency, I=Inpatient, O=Outpatient, P=Preadmit, R=Recurring patient, B=Obstetrics, C=Commercial Account, N=Not Applicable, U=Unkown
						//We will defualt to send 'O' for outpatient for outbound messages, but for incoming we will use this field for pat.GradeLevel if it's an integer from 1-12
						seg.AddField(2,"0004",DataTypeHL7.IS,"","O");
						//PV1.3, Assigned Patient Location, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						seg.AddField(3,"apt.location");
						//PV1.7, Attending/Primary Care Doctor, ProviderId^LastName^FirstName^MI
						seg.AddField(7,"0010",DataTypeHL7.XCN,"prov.provIdNameLFM","");
						//PV1.11, Temporary Location, PL - Person Location data type: PointOfCare^^^^^Person Location Type
						//Filled with the "Site (or Grade School)" value in the database, using 'S' to designate the location type site and the site.Description value
						//If no site assigned in OD, this will be blank as it is optional
						//Example: |West Salem Elementary^^^^^S| ('S' for site)
						seg.AddField(11,"pat.site");
						//PV1.18, Patient Type, IS data type (coded value for user-defined tables)
						//We will send one of the following values retrieved from the patient.Urgency field for treatment urgency: 0 - Unknown, 1 - NoProblems, 2 - NeedsCare, 3 - Urgent
						seg.AddField(18,"pat.Urgency");
						//PV1.19, Visit Number, CX data type, AptNum^CheckDigit^CheckDigitScheme^&AssignAuthorityID&IDType^VN  (VN - Visit Number)
						//We will use AptNum, with check digit scheme M11, with the OIDRoot for an appt object (set in oidinternal), ID Type 'HL7', and 'VN' for Visit Number
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.6&HL7^VN|
						seg.AddField(19,"apt.AptNum");
						//PV1.44, Admit Date/Time
						seg.AddField(44,"proc.procDateTime");
					#endregion PV1 - Patient Visit
					#region FT1 - Financial Transaction Information
					seg=new HL7DefSegment();
					msg.AddSegment(seg,4,true,true,SegmentNameHL7.FT1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//FT1.1, Sequence Number (starts with 1)
						seg.AddField(1,"sequenceNum");
						//FT1.2, Transaction ID
						seg.AddField(2,"proc.ProcNum");
						//FT1.4, Transaction Date (YYYYMMDDHHMMSS)
						seg.AddField(4,"proc.procDateTime");
						//FT1.5, Transaction Posting Date (YYYYMMDDHHMMSS)
						seg.AddField(5,"proc.procDateTime");
						//FT1.6, Transaction Type
						seg.AddFieldFixed(6,DataTypeHL7.IS,"CG");
						//FT1.7, Transaction Code, CWE data type
						//This is a required field and should contain "the code assigned by the institution for the purpose of uniquely identifying the transaction based on the Transaction Type (FT1-6)".
						//We will just send the ProcNum, not sure if any receiving application will use it
						seg.AddField(7,"proc.ProcNum");
						//FT1.10, Transaction Quantity
						seg.AddFieldFixed(10,DataTypeHL7.NM,"1.0");
						//FT1.11, Transaction Amount Extended, CP data type
						//Total fee to charge for this procedure, independent of transaction quantity
						seg.AddField(11,"proc.ProcFee");
						//FT1.12, Transaction Amount Unit, CP data type
						//Fee for this procedure for each transaction quantity
						seg.AddField(12,"proc.ProcFee");
						//FT1.16, Assigned Patient Location, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						//In the FT1 segment, we will get the clinic from the specific procedure, but if not set (0) we will get it from the patient
						//If no clinic on the proc or assigned to the patient, this will be blank
						seg.AddField(16,"proc.location");
						//FT1.19, Diagnosis Code, CWE data type, repeatable if they have more than 1 code on the proc (currently 4 allowed)
						//Example: |520.2^Abnormal tooth size/form^I9C^^^^31^^~521.81^Cracked tooth^I9C^^^^31^^|
						seg.AddField(19,"proc.DiagnosticCode");
						//FT1.20, Performed by Code
						seg.AddField(20,"prov.provIdNameLFM");
						//FT1.21, Ordering Provider
						seg.AddField(21,"prov.provIdNameLFM");
						//FT1.22, Unit Cost (procedure fee)
						seg.AddField(22,"proc.ProcFee");
						//FT1.25, Procedure Code, CNE data type
						//ProcNum^Descript^CD2^^^^2014^^LaymanTerm
						//Example: D0150^comprehensive oral evaluation - new or established patient^CD2^^^^2014^^Comprehensive Exam
						seg.AddField(25,"proccode.ProcCode");
						//FT1.26, Modifiers (treatment area)
						seg.AddField(26,"proc.toothSurfRange");
					#endregion FT1 - Financial Transaction Information
					#region IN1 - Insurance (global across all FT1s)
					seg=new HL7DefSegment();
					msg.AddSegment(seg,5,true,true,SegmentNameHL7.IN1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//IN1.1, Set ID (starts with 1)
						//This will be 1 for primary ins and increment for additional repetitions for secondary ins etc.
						seg.AddField(1,"sequenceNum");
						//IN1.2, Insurance Plan ID, CWE data type
						//We do not have unique ID's for insurance plans.
						//We have created an oidinternal ID for insplan, root+.7, and will use this with the insplan.PlanNum to uniquely identify an insplan.
						//Example: 2.16.840.1.113883.3.4337.1486.6566.7.1234
						seg.AddField(2,"insplan.planNum");
						//IN1.3, Insurance Company ID, CX data type
						seg.AddField(3,"carrier.ElectID");
						//IN1.4, Insurance Company Name, XON data type
						seg.AddField(4,"carrier.CarrierName");
						//IN1.5, Insurance Company Address, XAD data type
						//carrier.Address^Address2^City^State^Zip
						seg.AddField(5,"carrier.addressCityStateZip");
						//IN1.7, Insurance Company Phone Number, XTN data type
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(7,"carrier.Phone");
						//IN1.8, Group Number, ST data type
						seg.AddField(8,"insplan.GroupNum");
						//IN1.9, Group Name, XON data type
						seg.AddField(9,"insplan.GroupName");
						//IN1.11, Insured's Group Employer Name, XON data type
						//insplan.EmployerNum to employer.EmpName
						seg.AddField(11,"insplan.empName");
						//IN1.12, Plan Effective Date, DT data type
						seg.AddField(12,"inssub.DateEffective");
						//IN1.13, Plan Expiration Date, DT data type
						seg.AddField(13,"inssub.DateTerm");
						//IN1.15, Plan Type, IS data type
						//insplan.PlanType, Category Percentage, PPO Percentage, Medicaid or Flat Copay, Capitation
						seg.AddField(15,"insplan.PlanType");
						//IN1.16, Name of Insured, XPN data type
						//insplan.InsSubNum->inssub.Subscriber->patient name
						seg.AddField(16,"inssub.subscriberName");
						//IN1.17, Insured's Relationship to Patient, CWE data type
						//SEL-Self, SPO-Spouse, DOM-LifePartner, CHD-Child (PAR-Parent), EME-Employee (EMR-Employer)
						//DEP-HandicapDep (GRD-Guardian), DEP-Dependent, OTH-SignifOther (OTH-Other), OTH-InjuredPlantiff
						//We store relationship to subscriber and they want subscriber's relationship to patient, therefore
						//Relat.Child will return "PAR" for Parent, Relat.Employee will return "EMR" for Employer, and Relat.HandicapDep and Relat.Dependent will return "GRD" for Guardian
						//We will use the first two components, Identifier^Text
						//Example: |PAR^Parent|
						seg.AddField(17,"patplan.subRelationToPat");
						//IN1.18, Insured's Date of Birth, DTM data type
						seg.AddField(18,"inssub.subBirthdate");
						//IN1.19, Insured's Address, XAD data type
						seg.AddField(19,"inssub.subAddrCityStateZip");
						//IN1.20, Assignment of Benefits, IS data type
						//inssub.AssignBen 1-Y, 0-N
						seg.AddField(20,"inssub.AssignBen");
						//IN1.21, Coordination of Benefits, IS data type
						//CO-Coordination, IN-Independent.  Will be CO if more than one patplan
						seg.AddField(21,"insplan.cob");
						//IN1.22, Coordination of Benefits Priority, ST data type
						//1-Primary, 2-Secondary, 3,4,5...
						seg.AddField(22,"patplan.Ordinal");
						//IN1.27, Release of Information Code, IS data type
						//inssub.ReleaseInfo, 1-Y, 2-N
						seg.AddField(27,"inssub.ReleaseInfo");
						//IN1.36, Policy Number, ST data type
						//patplan.PatID, if blank then inssub.SubscriberID
						seg.AddField(36,"patplan.policyNum");
						//IN1.47, Coverage Type, IS data type
						//D-Dental, M-Medical using insplan.IsMedical flag
						seg.AddField(47,"insplan.coverageType");
						//IN1.49, Insured's ID Number, CX data type
						seg.AddField(49,"inssub.SubscriberID");
					#endregion IN1 - Insurance (global across all FT1s)
				#endregion DFT - Detailed Financial Transaction
				#region SIU - Schedule Information Unsolicited
				//Outbound SIU messages will be triggered when specific data is changed in OD, but not if the data change is due to an inbound HL7 message.
				//SIU's created when creating/modifying/cancelling/breaking an appointment
				msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.SIU,MessageStructureHL7.SIU_S12,InOutHL7.Outgoing,7);
					#region MSH - Message Header
					seg=new HL7DefSegment();
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.1, Encoding Characters (DataType.ST)
						seg.AddField(1,"separators^~\\&");
						//MSH.2, Sending Application, HD data type, Namespace ID^Universal ID^Universal ID Type
						//We originally sent 'OD' in this field, but this is a HD data type, so should consist of three components
						//We will send the Open Dental HL7 root assigned to the office and stored in the oidinternal table with the IDType of Root
						//The recommeded value for this field in the table is ^2.16.840.1.113883.3.4337.1486.CustomerPatNum^HL7.
						//If the oidinternal row with IDType=Root does not have an IDRoot assigned, we will revert to sending 'OD'
						seg.AddField(2,"sendingApp");
						//MSH.3, Sending Facility, not used
						//MSH.4, Receiving Application
						//This field will have to be a fixed text field and would have to be modified in a custom definition
						//if they want the outbound messages to have a specific receiving application identifier
						seg.AddField(4,"0361",DataTypeHL7.HD,"","NamespaceID^UniversalID^UniversalIDType");
						//MSH.5, Receiving Facility, not used
						//MSH.6, Message Date and Time (YYYYMMDDHHMMSS)
						seg.AddField(6,"dateTime.Now");
						//MSH.8, Message Type, MSG data type
						//This segment should contain MessageType^EventType^MessageStructure, example SIU^S12^SIU_S12
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						//A GUID that uniquely identifies this message
						seg.AddField(9,"messageControlId");
						//MSH.10, Processing ID, PT data type, Processing ID^Processing Mode
						//Processing mode is optional and not included, we will use P for production (P-Production, T-Training, D-Debugging)
						seg.AddField(10,"0103",DataTypeHL7.PT,"","P");
						//MSH.11, Version ID, VID data type, Version ID^Internationalization Code^International Version ID
						//All components are optional, we will only send Version ID, which is currently 2.6
						seg.AddField(11,"0104",DataTypeHL7.VID,"","2.6");
						//MSH.15, Application Ack Type (AL=Always, NE=Never, ER=Error/reject conditions only, SU=Successful completion only)
						//This is for enhanced acknowledgment mode, which we do not currently support, but we will send our mode just in case
						//With field MSH.15, Accept Ack Type, not present it will default to NE
						//Field MSH.15 set to AL means we will require an ack for every message that is sent, meaning it was processed successfully by the receiving application
						//But MSH.15 not present or null means we do not require a separate accept ACK message, just the accept and validate response meaning the message was accepted and processed
						seg.AddField(15,"0155",DataTypeHL7.ID,"","AL");
					#endregion MSH - Message Header
					#region SCH - Schedule Activity Information
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.SCH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//SCH.1, Placer Appointment ID, EI data type, OD is the filler application
						//We will store the exernalAptId from an incoming SRM message ARQ segment if they send it to us, so send it here if we have one
						seg.AddField(1,"apt.externalAptID");
						//SCH.2, Filler Appointment ID, EI data type, OD is the filler application
						//We will populate the first component with the AptNum, the third with the OID for an appointment object, the fourth with "HL7"
						//Example: 1234^^2.16.840.1.113883.3.4337.1486.6566.6^HL7
						seg.AddField(2,"apt.AptNum");
						//SCH.5, Schedule ID, CWE data type
						//We will use this to send the operatory name
						seg.AddField(5,"apt.operatory");
						//SCH.7, Appointment Reason
						seg.AddField(7,"apt.Note");
						//SCH.8, Appointment Type, CWE data type
						//Suggested values are Normal - Routine schedule request type, Tentative, or Complete - Request to add a completed appt
						//We will send Normal for all appointment statuses except complete.
						seg.AddField(8,"apt.type");
						//SCH.16, Filler Contact Person, XCN data type
						//The OD user who created/modified the appointment
						seg.AddField(16,"apt.userOD");
						//SCH.20, Entered by Person, XCN data type
						//The OD user who created/modified the appointment
						seg.AddField(20,"apt.userOD");
						//SCH.25, Filler Status Code, CWE data type
						//This will signal to the other software the AptStatus of the appointment
						//Booked - ApptStatus.Scheduled or ApptStatus.ASAP, Complete - ApptStatus.Complete, Cancelled - ApptStatus.Broken or ApptStatus.UnscheduledList, Deleted (when deleted)
						seg.AddField(25,"apt.aptStatus");
					#endregion SCH - Schedule Activity Information
					#region TQ1 - Timing/Quantity
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,false,true,SegmentNameHL7.TQ1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//TQ1.1, Set ID, SI data type
						//Always 1, only one timing specification in our outbound SIU messages
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//TQ1.2, Quantity, CQ data type
						//Always default value 1
						seg.AddFieldFixed(2,DataTypeHL7.CQ,"1");
						//TQ1.6, Service Duration, CQ data type
						//apt.Pattern is filled with X's and /'s, stored in 5 minute increments
						//Example: 60^min&&ANS+, ANS+ is the name of the coding system
						seg.AddField(6,"apt.length");
						//TQ1.7, Start Date/Time, DTM data type
						seg.AddField(7,"apt.AptDateTime");
						//TQ1.8, End Date/Time, DTM data type
						seg.AddField(8,"apt.endAptDateTime");
					#endregion TQ1 - Timing/Quantity
					#region PID - Patient Identification
					seg=new HL7DefSegment();
					msg.AddSegment(seg,3,SegmentNameHL7.PID);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PID.1, Set ID, SI data type
						//"This field contains the number that identifies this transaction.  For the first occurrence of the segment, the sequence number shall be one" (HL7 v2.6 documentation)
						//We only send 1 PID segment in SIU's so this number will always be 1.
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//PID.2, Patient ID (retained for backward compatibility only).  Defined as 'external' ID, we've always sent the ChartNumber in this field as the eCW patient ID
						//The standard will be to continue to use PID.2 for sending the ChartNumber, which was set from PID.4 of an incoming message.  This could be the other software patient ID.
						seg.AddField(2,"pat.ChartNumber");
						//PID.3, Patient Identifier List, contains a list of identifiers separated by the repetition character (usually ~)
						//PatientID^IDCheckDigit^CheckDigitScheme^AssigningAuthority^IDTypeCode
						//Assigning Authority is a HD data type and is composed of 3 subcomponents, NamespaceID&UniversalID&UniversalIDType
						//Sub-component 2, UniversalID, we will use the OID root for a Patient object stored in the oidinternal table
						//If retrieved from OD it will be 2.16.840.1.113883.3.4337.1486.CustomerNumber.2
						//Sub-component 3, Universal ID Type, will be 'HL7' since our root is registered with HL7.
						//The IDTypeCode will be 'PI', Patient internal identifier.
						//We will also get all of the identifiers in the oidexternals table for the patient and create repetitions for each external ID using the IDExternal and RootExternal
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&^PI|
						seg.AddField(3,"patientIds");
						//PID.4, Alternate Patient ID, (retained for backward compatibility only).
						//We've used PID.4 for sending the OD PatNum in the past and will continue to send this in PID.4
						seg.AddField(4,"pat.PatNum");
						//PID.5, Patient Name
						seg.AddField(5,"pat.nameLFM");
						//PID.7, Date/Time of Birth
						seg.AddField(7,"pat.birthdateTime");
						//PID.8, Administrative Sex
						seg.AddField(8,"pat.Gender");
						//PID.10, Race, CWE - Coded with Exceptions data type
						//This data type is composed of 9 fields, Code^Description^CodeSystem^AlternateCode^AltDescript^AltCodeSystem^CodeSystemVersionID^AltCodeSystemVersionID^OriginalText
						//We will send Code^Description^CodeSystem^^^^CodeSystemVersionID
						//The race field can repeat, so any number of races can be sent and will come from the patientrace table.
						//Example: 2106-3^White^CDCREC^^^^1~2186-5^NotHispanic^CDCREC^^^^1
						seg.AddField(10,"pat.Race");
						//PID.11, Patient Address
						//PID.11.20, comment, will come from the patient.AddrNote column and each new line character will be replaced with '\.br\' where the '\' is the defined escape char
						//Example: 123 Main St^Apt 1^Dallas^OR^97338^^^^^^^^^^^^^^^Emergency Contact: Mom Test1\.br\Mother\.br\(503)623-3072
						seg.AddField(11,"pat.addressCityStateZip");
						//PID.13, Phone Number - Home, XTN data type (can repeat)
						//This field will also contain the patient's email address as a repetition as well as the WirelessPhone as a repetition
						//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
						//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
						seg.AddField(13,"pat.HmPhone");
						//PID.14, Phone Number - Business, XTN data type (can repeat)
						//We will send just one repetition, the WkPhone, but we will adhere to the data type
						//WPN=Work Number
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(14,"pat.WkPhone");
						//PID.16, Marital Status
						//S-Single, M-Married, W-Widowed, D-Divorced
						seg.AddField(16,"pat.Position");
						//PID.19, SSN - Patient
						seg.AddField(19,"pat.SSN");
					#endregion PID - Patient Identification
					#region PV1 - Patient Visit
					seg=new HL7DefSegment();
					msg.AddSegment(seg,4,false,true,SegmentNameHL7.PV1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PV1.1, Set ID - PV1
						//See the comment above for the Sequence Number of the PID segment.  Always 1 since we only send one PV1 segment per SIU message.
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//PV1.2, Patient Class, IS data type (coded value for user-defined tables)
						//Suggested values E=Emergency, I=Inpatient, O=Outpatient, P=Preadmit, R=Recurring patient, B=Obstetrics, C=Commercial Account, N=Not Applicable, U=Unkown
						//We will defualt to send 'O' for outpatient for outbound messages, but for incoming we will use this field for pat.GradeLevel if it's an integer from 1-12
						seg.AddField(2,"0004",DataTypeHL7.IS,"","O");
						//PV1.3, Assigned Patient Location, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						seg.AddField(3,"apt.location");
						//PV1.7, Attending/Primary Care Doctor, ProviderId^LastName^FirstName^MI
						seg.AddField(7,"0010",DataTypeHL7.XCN,"prov.provIdNameLFM","");
						//PV1.11, Temporary Location, PL - Person Location data type: PointOfCare^^^^^Person Location Type
						//Filled with the "Site (or Grade School)" value in the database, using 'S' to designate the location type site and the site.Description value
						//If no site assigned in OD, this will be blank as it is optional
						//Example: |West Salem Elementary^^^^^S| ('S' for site)
						seg.AddField(11,"pat.site");
						//PV1.18, Patient Type, IS data type (coded value for user-defined tables)
						//We will send one of the following values retrieved from the patient.Urgency field for treatment urgency: 0 - Unknown, 1 - NoProblems, 2 - NeedsCare, 3 - Urgent
						seg.AddField(18,"pat.Urgency");
						//PV1.19, Visit Number, CX data type, AptNum^CheckDigit^CheckDigitScheme^&AssignAuthorityID&IDType^VN  (VN - Visit Number)
						//We will use AptNum, with check digit scheme M11, with the OIDRoot for an appt object (set in oidinternal), ID Type 'HL7', and 'VN' for Visit Number
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.6&HL7^VN|
						seg.AddField(19,"apt.AptNum");
						//PV1.44, Admit Date/Time
						seg.AddField(44,"apt.AptDateTime");
					#endregion PV1 - Patient Visit
					#region RGS - Resource Group
					seg=new HL7DefSegment();
					msg.AddSegment(seg,5,SegmentNameHL7.RGS);
						//Fields-------------------------------------------------------------------------------------------------------------
						//RGS.1, Set ID, SI data type
						//Always 1, only one resourece group in our outbound SIU's
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//RGS.2, Segment Action Code, ID data type
						//A-Add/Insert, D-Delete, U-Update, X-No Change
						seg.AddField(2,"segmentAction");
					#endregion RGB - Resource Group
					#region AIL - Appointment Information - Location Resource
					//We will use the AIL segment to identify the clinic and operatory for the appointment
					seg=new HL7DefSegment();
					msg.AddSegment(seg,6,false,true,SegmentNameHL7.AIL);
						//Fields-------------------------------------------------------------------------------------------------------------
						//AIL.1, Set ID, SI data type
						//Always 1, only one location identified by our SIU messsage
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//AIL.2, Segment Action Code, ID data type
						//A-Add/Insert, D-Delete, U-Update, X-No Change
						seg.AddField(2,"segmentAction");
						//AIL.3, Location Resource ID, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						seg.AddField(3,"apt.location");
						//AIL.12, Filler Status Code, CWE data type
						//We will use this to send the confirmation status of the appointment
						//We will use the second component (text) and send the ItemName (confirm Name) from the definition table that is set for this appointment
						//Example: |^Appointment Confirmed|
						seg.AddField(12,"apt.confirmStatus");
					#endregion AIL - Appointment Information - Location Resource
					#region AIP - Appointment Information - Personnel Resource
					//We will use the AIP segment to identify the dentist and hygienist on the appointment
					//If there is both a dentist and a hygienist assigned to an appointment, we will send two repetitions of the AIP segment
					seg=new HL7DefSegment();
					msg.AddSegment(seg,7,true,true,SegmentNameHL7.AIP);
						//Fields-------------------------------------------------------------------------------------------------------------
						//AIP.1, Set ID, SI data type
						//This segment can repeat for an appt, sequence 1 will be the dentist, 2 will be the hygienist on the appt
						seg.AddField(1,"sequenceNum");
						//AIP.2, Segment Action Code, ID data type
						//A-Add/Insert, D-Delete, U-Update, X-No Change
						seg.AddField(2,"segmentAction");
						//AIP.3, Personnel Resource ID, XCN data type
						//ProviderId^LastName^FirstName^MI^Suffix^Prefix
						//According to the HL7 standards, we are free to use the components as needed to identify the provider
						//We will use 'ProviderID' to send the OD ProvNum, LName, FName and the 'Prefix' component as the Abbr
						//We will retrieve the provider from the appointment and use AIP.4 to differentiate between the dentist and hygienist
						//Example: |1234^Abbott^Sarah^L^DMD^DrAbbott|
						seg.AddField(3,"prov.provIdNameLFM");
						//AIP.4, Resource Type, CWE data type
						//We will send 'd' for dentist (apt.ProvNum) and 'h' for hygienist (apt.ProvHyg)
						seg.AddField(4,"prov.provType");
						//AIP.12, Filler Status Code, CWE data type
						//We will use this to send the confirmation status of the appointment
						//We will use the second component (text) and send the ItemName (confirm Name) from the definition table that is set for this appointment
						//Example: |^Appointment Confirmed|
						seg.AddField(12,"apt.confirmStatus");
					#endregion AIP - Appointment Information - Personnel Resource
				#endregion SIU - Schedule Information Unsolicited
				#region SRR - Schedule Request Response
				msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.SRR,MessageStructureHL7.SRR_S01,InOutHL7.Outgoing,8);
					#region MSH - Message Header
					seg=new HL7DefSegment();
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.1, Encoding Characters (DataType.ST)
						seg.AddField(1,"separators^~\\&");
						//MSH.2, Sending Application, HD data type, Namespace ID^Universal ID^Universal ID Type
						//We originally sent 'OD' in this field, but this is a HD data type, so should consist of three components
						//We will send the Open Dental HL7 root assigned to the office and stored in the oidinternal table with the IDType of Root
						//The recommeded value for this field in the table is ^2.16.840.1.113883.3.4337.1486.CustomerPatNum^HL7.
						//If the oidinternal row with IDType=Root does not have an IDRoot assigned, we will revert to sending 'OD'
						seg.AddField(2,"sendingApp");
						//MSH.3, Sending Facility, not used
						//MSH.4, Receiving Application
						//This field will have to be a fixed text field and would have to be modified in a custom definition
						//if they want the outbound messages to have a specific receiving application identifier
						seg.AddField(4,"0361",DataTypeHL7.HD,"","NamespaceID^UniversalID^UniversalIDType");
						//MSH.5, Receiving Facility, not used
						//MSH.6, Message Date and Time (YYYYMMDDHHMMSS)
						seg.AddField(6,"dateTime.Now");
						//MSH.8, Message Type, MSG data type
						//This segment should contain MessageType^EventType^MessageStructure, example SRR^S04^SRR_S04
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						//A GUID that uniquely identifies this message
						seg.AddField(9,"messageControlId");
						//MSH.10, Processing ID, PT data type, Processing ID^Processing Mode
						//Processing mode is optional and not included, we will use P for production (P-Production, T-Training, D-Debugging)
						seg.AddField(10,"0103",DataTypeHL7.PT,"","P");
						//MSH.11, Version ID, VID data type, Version ID^Internationalization Code^International Version ID
						//All components are optional, we will only send Version ID, which is currently 2.6
						seg.AddField(11,"0104",DataTypeHL7.VID,"","2.6");
						//MSH.15, Application Ack Type (AL=Always, NE=Never, ER=Error/reject conditions only, SU=Successful completion only)
						//This is for enhanced acknowledgment mode, which we do not currently support, but we will send our mode just in case
						//With field MSH.15, Accept Ack Type, not present it will default to NE
						//Field MSH.15 set to AL means we will require an ack for every message that is sent, meaning it was processed successfully by the receiving application
						//But MSH.15 not present or null means we do not require a separate accept ACK message, just the accept and validate response meaning the message was accepted and processed
						seg.AddField(15,"0155",DataTypeHL7.ID,"","AL");
					#endregion MSH - Message Header
					#region MSA - Message Acknowledgment
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.MSA);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSA.1, Acknowledgment Code
						seg.AddField(1,"ackCode");
						//MSA.2, Message Control ID
						seg.AddField(2,"messageControlId");
					#endregion MSA - Message Acknowledgment
					#region SCH - Schedule Activity Information
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,false,true,SegmentNameHL7.SCH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//SCH.1, Placer Appointment ID, EI data type, OD is the filler application
						//We will store the exernalAptId from an incoming SRM message ARQ segment if they send it to us, so send it here if we have one
						seg.AddField(1,"apt.externalAptID");
						//SCH.2, Filler Appointment ID, EI data type, OD is the filler application
						//AptNum^^AssignAuthorityID^IDType
						//Example: |1234^^2.16.840.1.113883.3.4337.1486.6566.6^HL7|  root.6 is appointment
						seg.AddField(2,"apt.AptNum");
						//SCH.5, Schedule ID, CWE data type
						//We will use this to send the operatory name
						seg.AddField(5,"apt.operatory");
						//SCH.7, Appointment Reason
						seg.AddField(7,"apt.Note");
						//SCH.8, Appointment Type, CWE data type
						//Suggested values are Normal - Routine schedule request type, Tentative, or Complete - Request to add a completed appt
						//We will send Normal for all appointment statuses except complete.
						seg.AddField(8,"apt.type");
						//SCH.16, Filler Contact Person, XCN data type
						//The OD user who created/modified the appointment
						seg.AddField(16,"apt.userOD");
						//SCH.20, Entered by Person, XCN data type
						//The OD user who created/modified the appointment
						seg.AddField(20,"apt.userOD");
						//SCH.25, Filler Status Code, CWE data type
						//This will signal to the other software the AptStatus of the appointment
						//Booked - ApptStatus.Scheduled or ApptStatus.ASAP, Complete - ApptStatus.Complete, Cancelled - ApptStatus.Broken or ApptStatus.UnscheduledList, Deleted (when deleted)
						seg.AddField(25,"apt.aptStatus");
					#endregion SCH - Schedule Activity Information
					#region NTE - Notes and Comments
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,true,true,SegmentNameHL7.NTE);
						//Fields-------------------------------------------------------------------------------------------------------------
						//NTE.1, Set ID, SI data type
						//We will only ever send one NTE segment in outbound messages, so this will always be 1
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//NTE.3, Comment, FT data type (formatted text)
						//We will send the appointment.Note value in this field for outbound messages
						//This is formatted text, so new line characters in the appointment.Note will be replaced with
						//the designated new line character sequence '\.br\' (where the '\' is the defined escape char, \ by default)
						seg.AddField(3,"apt.Note");
					#endregion NTE - Notes and Comments
					#region PID - Patient Identification
					seg=new HL7DefSegment();
					msg.AddSegment(seg,3,SegmentNameHL7.PID);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PID.1, Set ID, SI data type
						//"This field contains the number that identifies this transaction.  For the first occurrence of the segment, the sequence number shall be one" (HL7 v2.6 documentation)
						//We only send 1 PID segment in SRR's so this number will always be 1.
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//PID.2, Patient ID (retained for backward compatibility only).  Defined as 'external' ID, we've always sent the ChartNumber in this field as the eCW patient ID
						//The standard will be to continue to use PID.2 for sending the ChartNumber, which was set from PID.4 of an incoming message.  This could be the other software patient ID.
						seg.AddField(2,"pat.ChartNumber");
						//PID.3, Patient Identifier List, contains a list of identifiers separated by the repetition character (usually ~)
						//PatientID^IDCheckDigit^CheckDigitScheme^AssigningAuthority^IDTypeCode
						//Assigning Authority is a HD data type and is composed of 3 subcomponents, NamespaceID&UniversalID&UniversalIDType
						//Sub-component 2, UniversalID, we will use the OID root for a Patient object stored in the oidinternal table
						//If retrieved from OD it will be 2.16.840.1.113883.3.4337.1486.CustomerNumber.2
						//Sub-component 3, Universal ID Type, will be 'HL7' since our root is registered with HL7.
						//The IDTypeCode will be 'PI', Patient internal identifier.
						//We will also get all of the identifiers in the oidexternals table for the patient and create repetitions for each external ID using the IDExternal and RootExternal
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.2&HL7^PI~7684^8^M11^&Other.Software.OID&^PI|
						seg.AddField(3,"patientIds");
						//PID.4, Alternate Patient ID, (retained for backward compatibility only).
						//We've used PID.4 for sending the OD PatNum in the past and will continue to send this in PID.4
						seg.AddField(4,"pat.PatNum");
						//PID.5, Patient Name
						seg.AddField(5,"pat.nameLFM");
						//PID.7, Date/Time of Birth
						seg.AddField(7,"pat.birthdateTime");
						//PID.8, Administrative Sex
						seg.AddField(8,"pat.Gender");
						//PID.10, Race, CWE - Coded with Exceptions data type
						//This data type is composed of 9 fields, Code^Description^CodeSystem^AlternateCode^AltDescript^AltCodeSystem^CodeSystemVersionID^AltCodeSystemVersionID^OriginalText
						//We will send Code^Description^CodeSystem^^^^CodeSystemVersionID
						//The race field can repeat, so any number of races can be sent and will come from the patientrace table.
						//Example: 2106-3^White^CDCREC^^^^1~2186-5^NotHispanic^CDCREC^^^^1
						seg.AddField(10,"pat.Race");
						//PID.11, Patient Address
						//PID.11.20, comment, will come from the patient.AddrNote column and each new line character will be replaced with '\.br\' where the '\' is the defined escape char
						//Example: 123 Main St^Apt 1^Dallas^OR^97338^^^^^^^^^^^^^^^Emergency Contact: Mom Test1\.br\Mother\.br\(503)623-3072
						seg.AddField(11,"pat.addressCityStateZip");
						//PID.13, Phone Number - Home, XTN data type (can repeat)
						//This field will also contain the patient's email address as a repetition as well as the WirelessPhone as a repetition
						//PRN stands for Primary Residence Number, equipment type: PH is Telephone, CP is Cell Phone, Internet is Internet Address (email)
						//Example: ^PRN^PH^^^503^3635432~^PRN^Internet^someone@somewhere.com~^PRN^CP^^^503^6895555
						seg.AddField(13,"pat.HmPhone");
						//PID.14, Phone Number - Business, XTN data type (can repeat)
						//We will send just one repetition, the WkPhone, but we will adhere to the data type
						//WPN=Work Number
						//Example: ^WPN^PH^^^503^3635432
						seg.AddField(14,"pat.WkPhone");
						//PID.16, Marital Status
						//S-Single, M-Married, W-Widowed, D-Divorced
						seg.AddField(16,"pat.Position");
						//PID.19, SSN - Patient
						seg.AddField(19,"pat.SSN");
					#endregion PID - Patient Identification
					#region PV1 - Patient Visit
					seg=new HL7DefSegment();
					msg.AddSegment(seg,4,false,true,SegmentNameHL7.PV1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PV1.1, Set ID - PV1
						//See the comment above for the Sequence Number of the PID segment.  Always 1 since we only send one PV1 segment per SRR message.
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//PV1.2, Patient Class, IS data type (coded value for user-defined tables)
						//Suggested values E=Emergency, I=Inpatient, O=Outpatient, P=Preadmit, R=Recurring patient, B=Obstetrics, C=Commercial Account, N=Not Applicable, U=Unkown
						//We will defualt to send 'O' for outpatient for outbound messages, but for incoming we will use this field for pat.GradeLevel if it's an integer from 1-15
						seg.AddField(2,"0004",DataTypeHL7.IS,"","O");
						//PV1.3, Assigned Patient Location, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						seg.AddField(3,"apt.location");
						//PV1.7, Attending/Primary Care Doctor, ProviderId^LastName^FirstName^MI
						seg.AddField(7,"0010",DataTypeHL7.XCN,"prov.provIdNameLFM","");
						//PV1.11, Temporary Location, PL - Person Location data type: PointOfCare^^^^^Person Location Type
						//Filled with the "Site (or Grade School)" value in the database, using 'S' to designate the location type site and the site.Description value
						//If no site assigned in OD, this will be blank as it is optional
						//Example: |West Salem Elementary^^^^^S| ('S' for site)
						seg.AddField(11,"pat.site");
						//PV1.18, Patient Type, IS data type (coded value for user-defined tables)
						//We will send one of the following values retrieved from the patient.Urgency field for treatment urgency: 0 - Unknown, 1 - NoProblems, 2 - NeedsCare, 3 - Urgent
						seg.AddField(18,"pat.Urgency");
						//PV1.19, Visit Number, CX data type, AptNum^CheckDigit^CheckDigitScheme^&AssignAuthorityID&IDType^VN  (VN - Visit Number)
						//We will use AptNum, with check digit scheme M11, with the OIDRoot for an appt object (set in oidinternal), ID Type 'HL7', and 'VN' for Visit Number
						//Example: |1234^3^M11^&2.16.840.1.113883.3.4337.1486.6566.6&HL7^VN|
						seg.AddField(19,"apt.AptNum");
						//PV1.44, Admit Date/Time
						seg.AddField(44,"apt.AptDateTime");
					#endregion PV1 - Patient Visit
					#region RGS - Resource Group
					seg=new HL7DefSegment();
					msg.AddSegment(seg,5,SegmentNameHL7.RGS);
						//Fields-------------------------------------------------------------------------------------------------------------
						//RGS.1, Set ID, SI data type
						//Always 1, only one resourece group in our outbound SRR's
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//RGS.2, Segment Action Code, ID data type
						//A-Add/Insert, D-Delete, U-Update, X-No Change
						seg.AddField(2,"segmentAction");
					#endregion RGB - Resource Group
					#region AIL - Appointment Information - Location Resource
					//We will use the AIL segment to identify the clinic and operatory for the appointment
					seg=new HL7DefSegment();
					msg.AddSegment(seg,6,false,true,SegmentNameHL7.AIL);
						//Fields-------------------------------------------------------------------------------------------------------------
						//AIL.1, Set ID, SI data type
						//Always 1, only one location identified by our SRR messsage
						seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
						//AIL.2, Segment Action Code, ID data type
						//A-Add/Insert, D-Delete, U-Update, X-No Change
						seg.AddField(2,"segmentAction");
						//AIL.3, Location Resource ID, PL - Person Location Data Type: Point of Care^Room^^Facility^^Person Location Type
						//Facility is a HD data type, so Namespace&ID&IDType.  We will just use &PracticeTitle with no namespace or IDType.
						//Example: ClinicDescript^OpName^^&PracticeTitle^^C  (C for clinic)
						seg.AddField(3,"apt.location");
						//AIL.12, Filler Status Code, CWE data type
						//We will use this to send the confirmation status of the appointment
						//We will use the second component (text) and send the ItemName (confirm Name) from the definition table that is set for this appointment
						//Example: |^Appointment Confirmed|
						seg.AddField(12,"apt.confirmStatus");
					#endregion AIL - Appointment Information - Location Resource
					#region AIP - Appointment Information - Personnel Resource
					//We will use the AIP segment to identify the dentist and hygienist on the appointment
					//If there is both a dentist and a hygienist assigned to an appointment, we will send two repetitions of the AIP segment
					seg=new HL7DefSegment();
					msg.AddSegment(seg,7,true,true,SegmentNameHL7.AIP);
						//Fields-------------------------------------------------------------------------------------------------------------
						//AIP.1, Set ID, SI data type
						//This segment can repeat for an appt, sequence 1 will be the dentist, 2 will be the hygienist on the appt
						seg.AddField(1,"sequenceNum");
						//AIP.2, Segment Action Code, ID data type
						//A-Add/Insert, D-Delete, U-Update, X-No Change
						seg.AddField(2,"segmentAction");
						//AIP.3, Personnel Resource ID, XCN data type
						//ProviderId^LastName^FirstName^MI^Suffix^Prefix
						//According to the HL7 standards, we are free to use the components as needed to identify the provider
						//We will use 'ProviderID' to send the OD ProvNum, LName, FName and the 'Prefix' component as the Abbr
						//We will retrieve the provider from the appointment and use AIP.4 to differentiate between the dentist and hygienist
						//Example: |1234^Abbott^Sarah^L^DMD^DrAbbott|
						seg.AddField(3,"prov.provIdNameLFM");
						//AIP.4, Resource Type, CWE data type
						//We will send 'd' for dentist (apt.ProvNum) and 'h' for hygienist (apt.ProvHyg)
						seg.AddField(4,"prov.provType");
						//AIP.12, Filler Status Code, CWE data type
						//We will use this to send the confirmation status of the appointment
						//We will use the second component (text) and send the ItemName (confirm Name) from the definition table that is set for this appointment
						//Example: |^Appointment Confirmed|
						seg.AddField(12,"apt.confirmStatus");
					#endregion AIP - Appointment Information - Personnel Resource
				#endregion SRR - Schedule Request Response
			#endregion Outbound Messages
			return def;
		}

	}
}

