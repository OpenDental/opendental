using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness.HL7 {
	///<summary></summary>
	public class MedLabv2_3 {

		public static HL7Def GetDeepInternal(HL7Def def) {
			//ok to pass in null
			if(def==null) {//wasn't in the database
				def=new HL7Def();
				def.IsNew=true;
				def.Description="MedLab HL7 v2.3";
				def.ModeTx=ModeTxHL7.Sftp;
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
				def.InternalType=HL7InternalType.MedLabv2_3;
				def.InternalTypeVersion=Assembly.GetAssembly(typeof(Db)).GetName().Version.ToString();
				def.IsEnabled=false;
				def.Note="";
				def.ShowDemographics=HL7ShowDemographics.ChangeAndAdd;//these last four properties will not be editable for a lab interface type
				def.ShowAccount=true;
				def.ShowAppts=true;
				def.IsQuadAsToothNum=false;
			}
			def.hl7DefMessages=new List<HL7DefMessage>();
			HL7DefMessage msg=new HL7DefMessage();
			HL7DefSegment seg=new HL7DefSegment();
			#region Inbound Messages
				#region ORU - Unsolicited Observation Message
				def.AddMessage(msg,MessageTypeHL7.ORU,MessageStructureHL7.ORU_R01,InOutHL7.Incoming,0);
					#region MSH - Message Header
					msg.AddSegment(seg,0,SegmentNameHL7.MSH);
						//Fields-------------------------------------------------------------------------------------------------------------
						//MSH.2, Sending Application.  To identify the LabCorp Lab System sending the results.
						//Possible values for LabCorp (as of their v10.7 specs): '1100' - LabCorp Lab System, 'DIANON' - DIANON Systems,
						//'ADL' - Acupath Diagnostic Laboratories, 'EGL' - Esoterix Genetic Laboratories.
						//For backward compatibility only: 'CMBP', 'LITHOLINK', 'USLABS'
						seg.AddField(2,"sendingApp");
						//MSH.3, Sending Facility.  Identifies the LabCorp laboratory responsible for the client.
						//It could be a LabCorp assigned 'Responsible Lab Code' representing the responsible laboratory or it could be a CLIA number.
						seg.AddField(3,"sendingFacility");
						//MSH.8, Message Type
						seg.AddField(8,"messageType");
						//MSH.9, Message Control ID
						seg.AddField(9,"messageControlId");
					#endregion MSH - Message Header
					#region PID - Patient Identification
					seg=new HL7DefSegment();
					msg.AddSegment(seg,1,SegmentNameHL7.PID);
						//Fields-------------------------------------------------------------------------------------------------------------
						//PID.2, External Patient ID.  LabCorp defines this as 'client' assigned patient id, just like they do PID.4.
						//This should be the Open Dental patient number, sent in outbound PID.4 and returned in PID.2.
						seg.AddField(2,"pat.PatNum");
						//PID.3, Lab Assigned Patient ID.  LabCorp assigned specimen number.
						seg.AddField(3,"labPatID");
						//PID.4, Alternate Patient ID.  LabCorp defines this as a 'client' assigned patient id, just like they do PID.2.
						//This will be in outbound PID.2, returned in PID.4.
						seg.AddField(4,"altPatID");
						//PID.5, Patient Name
						//This will contain the last, first, and middle names as well as the title
						//Example:  LName^FName^MiddleI
						seg.AddField(5,"pat.nameLFM");
						//PID.7, Date/Time of Birth with Age
						//LabCorp uses this for the birthdate as well as the age in years, months, and days of the patient in the format bday^years^months^days.
						//All age components are left padded with 0's, the years is padded to 3 chars, the months and days are padded to 2 chars
						//Example: 19811213^033^02^19
						seg.AddField(7,"patBirthdateAge");
						//PID.8, Patient Gender
						//We use this field to assist the user in selecting a patient if one is not found when importing the message, but we don't store it
						seg.AddField(8,"pat.Gender");
						//PID.18.1, Patient Account Number.  LabCorp assigned account number.  This field is also used to send the Fasting flag in component 7.
						//Fasting flag values are 'Y', 'N', or blank
						//Example: AccountNum^^^BillCode^ABNFlag^SpecimenStatus^FastingFlag
						seg.AddField(18,"accountNum");
						//PID.19, Patient SSN Number
						//We use this field to assist the user in selecting a patient if one is not found when importing the message, but we don't store it
						seg.AddField(19,"pat.SSN");
					#endregion PID - Patient Identification
					#region NK1 - Next of Kin
					//This segment is for future use only, nothing is currently imported from this segment
					seg=new HL7DefSegment();
					msg.AddSegment(seg,2,false,true,SegmentNameHL7.NK1);
						//Fields-------------------------------------------------------------------------------------------------------------
						//NK1.2, Next of Kin Name
						//Example: LName^FName^Middle
						//seg.AddField(2,"nextOfKinName");
						//NK1.4, Next of Kin Address
						//Example: Address^Address2^City^State^Zip
						//seg.AddField(4,"nextOfKinAddress");
						//NK1.5, Next of Kin Phone
						//seg.AddField(5,"nextOfKinPhone");
						seg.hl7DefFields=new List<HL7DefField>();
					#endregion NK1 - Next of Kin
					#region NTE - Notes and Comments
					seg=new HL7DefSegment();
					msg.AddSegment(seg,3,true,true,SegmentNameHL7.NTE);
						//Fields-------------------------------------------------------------------------------------------------------------
						//NTE.2, Comment Source, ID data type
						//LabCorp supported values: 'L' - Laboratory is the source of comment, 'AC' - Accession Comment,
						//'RC' - Result comment, 'RI' - Normal Comment, 'UK' - Undefined comment type
						//We might pull out the source and prepend it to the note, but we won't explicitly add it to the definition or store it separately
						//NTE.3, Comment Text, FT data type (formatted text)
						//Stored in the medlab.NotePat field
						seg.AddField(3,"patNote");
					#endregion NTE - Notes and Comments
					#region ORC - Common Order
					seg=new HL7DefSegment();
					msg.AddSegment(seg,4,true,false,SegmentNameHL7.ORC);
						//Fields-------------------------------------------------------------------------------------------------------------
						//ORC.2, Unique Foreign Accession or Specimen ID
						//Must match the value in OBR.2 and is the ID value sent on the specimen container and is unique per patient order, not test order.
						//ORC.2.2 is the constant value 'LAB'
						//Example: L2435^LAB
						seg.AddField(2,"specimenID");
						//ORC.3, Filler Accession ID.  The LabCorp assigned specimen number.  These are reused on a yearly basis, but used with the client
						//specific specimen ID in ORC.2, these two numbers should uniquely identify a specimen/order.  ORC.3.2 is the constant value 'LAB'.
						//This should match OBR.3.
						//Example: 08599499950^LAB
						seg.AddField(3,"specimenIDFiller");
						//ORC.12, Ordering Provider, XCN Data Type
						//ProvID^ProvLName^ProvFName^ProvMiddleI^^^^SourceTable
						//This field repeats for every ID available for the provider with the SourceTable component identifying the type of ID in each repetition.
						//SourceTable possible values: 'U' - UPIN, 'P' - Provider Number (Medicaid or Commercial Insurance Provider ID),
						//'N' - NPI Number (Required for Third Party Billing), 'L' - Local (Physician ID)
						//Example: A12345^LNAME^FNAME^M^^^^U~23462^LNAME^FNAME^M^^^^L~0123456789^LNAME^FNAME^M^^^^N~1234567890^LNAME^FNAME^M^^^^P
						seg.AddField(12,"orderingProv");
					#endregion ORC - Common Order
					#region OBR - Observation Request
					seg=new HL7DefSegment();
					msg.AddSegment(seg,5,true,false,SegmentNameHL7.OBR);
						//Fields-------------------------------------------------------------------------------------------------------------
						//OBR.2, Unique Foreign Accession or Specimen ID
						//Must match the value in ORC.2 and is the ID value sent on the specimen container and is unique per patient order, not test order.
						//OBR.2.2 is the constant value 'LAB'.
						//Example: L2435^LAB
						seg.AddField(2,"specimenID");
						//OBR.3, Internal Accession ID.  The LabCorp assigned specimen number.  These are reused on a yearly basis, but used with the client
						//specific specimen ID in OBR.2, these two numbers should uniquely identify a specimen/order.  OBR.3.2 is the constant value 'LAB'.
						//This should match ORC.3.
						//Example: 08599499950^LAB
						seg.AddField(3,"specimenIDFiller");
						//OBR.4, Universal Service Identifier, CWE data type
						//This identifies the observation.  This will be the ID and text description of the test, as well as the LOINC code and description.
						//Example: 006072^RPR^L^20507-0^Reagin Ab^LN
						seg.AddField(4,"obsTestID");
						//OBR.7, Observation/Specimen Collection Date/Time
						//Format for LabCorp: yyyyMMddHHmm
						seg.AddField(7,"dateTimeCollected");
						//OBR.9, Collection/Urine Volume
						seg.AddField(9,"totalVolume");
						//OBR.11, Action Code
						//Used to identify the type of result being returned.  'A' - Add on, 'G' - Reflex, Blank for standard results
						seg.AddField(11,"specimenAction");
						//OBR.13, Relevant Clinical Information.  Used for informational purposes.
						seg.AddField(13,"clinicalInfo");
						//OBR.14, Date/Time of Specimen Receipt in Lab.
						//LabCorp format: yyyMMddHHmm
						seg.AddField(14,"dateTimeEntered");
						//OBR.16, Ordering Provider.
						//ProvID^ProvLName^ProvFName^ProvMiddleI^^^^SourceTable
						//This field repeats for every ID available for the provider with the SourceTable component identifying the type of ID in each repetition.
						//SourceTable possible values: 'U' - UPIN, 'P' - Provider Number (Medicaid or Commercial Insurance Provider ID),
						//'N' - NPI Number (Required for Third Party Billing), 'L' - Local (Physician ID)
						//Example: A12345^LNAME^FNAME^M^^^^U~23462^LNAME^FNAME^M^^^^L~0123456789^LNAME^FNAME^M^^^^N~1234567890^LNAME^FNAME^M^^^^P
						seg.AddField(16,"orderingProv");
						//OBR.18, Alternate Specimen ID.
						seg.AddField(18,"specimenIDAlt");
						//OBR.22, Date/Time Observation Reported.
						//LabCorp format: yyyyMMddHHmm
						seg.AddField(22,"dateTimeReported");
						//OBR.24, Producer's Section ID, used by LabCorp to identify the facility responsible for performing the testing.
						//This will be the footnote ID of the ZPS segment and will be used to attach a MedLab object to a MedLabFacility object.
						seg.AddField(24,"facilityID");
						//OBR.25, Order Result Status.
						//LabCorp values: 'F' - Final, 'P' - Preliminary, 'X' - Cancelled, 'C' - Corrected
						seg.AddField(25,"resultStatus");
						//OBR.26, Link to Parent Result.
						//If this is a reflex result, the value from the OBX.3.1 field of the parent result will be here.
						seg.AddField(26,"parentObsID");
						//OBR.29, Link to Parent Order.
						//If this is a reflex test, the value from the OBR.4.1 field of the parent test will be here.
						seg.AddField(29,"parentObsTestID");
					#endregion OBR - Observation Request
					#region NTE - Notes and Comments
					seg=new HL7DefSegment();
					msg.AddSegment(seg,6,true,true,SegmentNameHL7.NTE);
						//Fields-------------------------------------------------------------------------------------------------------------
						//NTE.2, Comment Source, ID data type
						//LabCorp supported values: 'L' - Laboratory is the source of comment, 'AC' - Accession Comment,
						//'RC' - Result comment, 'RI' - Normal Comment, 'UK' - Undefined comment type
						//We might pull out the source and prepend it to the note, but we won't explicitly add it to the definition or store it separately
						//NTE.3, Comment Text, FT data type (formatted text)
						//Stored in the medlab.NoteLab field
						seg.AddField(3,"labNote");
					#endregion NTE - Notes and Comments
					#region OBX - Observation/Result
					seg=new HL7DefSegment();
					msg.AddSegment(seg,7,true,false,SegmentNameHL7.OBX);
						//Fields-------------------------------------------------------------------------------------------------------------
						//OBX.2, Value Type.  This field is not stored explicitly, but it is used to determine the value type of the observation.
						//If this field is 'TX' for text, the value will be >21 chars and will be sent in the attached NTEs.
						seg.AddField(2,"obsValueType");
						//OBX.3, Observation ID.  This field has the same structure as the OBR.4.
						//ID^Text^CodeSystem^AltID^AltIDText^AltIDCodeSystem, the AltID is the LOINC code so the AltIDCodeSystem will be 'LN'
						//Example: 006072^RPR^L^20507-0^Reagin Ab^LN
						seg.AddField(3,"obsID");
						//OBX.4, Observation Sub ID.  This field is used to aid in the identification of results with the same observation ID (OBX.3) within a
						//given OBR.  If OBX.5.3 is 'ORM' (organism) this field will link a result to an organism, whether this is for organism #1, organism #2,
						//or organism #3.
						seg.AddField(4,"obsIDSub");
						//OBX.5, Observation Value.  ObsValue^TypeOfData^DataSubtype^Encoding^Data.  LabCorp report will display OBX.5.1 as the result.
						//For value >21 chars in length: OBX.2 will be 'TX' for text, OBX.5 will be NULL (empty field), and the value will be in attached NTEs.
						//"TNP" will be reported for Test Not Performed.
						seg.AddField(5,"obsValue");
						//OBX.6, Units.
						//Identifier^Text^CodeSystem.  Id is units of measure abbreviation, text is full text version of units, coding system is 'L' (local id).
						seg.AddField(6,"obsUnits");
						//OBX.7, Reference Range.
						seg.AddField(7,"obsRefRange");
						//OBX.8, Abnormal Flags.  For values see enum OpenDentBusiness.AbnormalFlag
						seg.AddField(8,"obsAbnormalFlag");
						//OBX.11, Observation Result Status.
						//LabCorp values: 'F' - Final, 'P' - Preliminary, 'X' - Cancelled, 'C' - Corrected, 'I' - Incomplete
						seg.AddField(11,"resultStatus");
						//OBX.14, Date/Time of Observation.
						//LabCorp format yyyyMMddHHmm
						seg.AddField(14,"dateTimeObs");
						//OBX.15, Producer's ID.
						//For LabCorp this is used to report the facility responsible for performing the testing.  This will hold the lab ID that will reference
						//a ZPS segment with the lab name, address, and director details.  Used to link a MedLabResult object to a MedLabFacility object.
						seg.AddField(15,"facilityID");
					#endregion OBX - Observation/Result
					#region ZEF - Encapsulated Data Format
					seg=new HL7DefSegment();
					msg.AddSegment(seg,8,true,true,SegmentNameHL7.ZEF);
						//Fields-------------------------------------------------------------------------------------------------------------
						//ZEF.1, Sequence Number, 1 through 9999
						seg.AddField(1,"sequenceNum");
						//ZEF.2, Embedded File.
						//Base64 embedded file, sent in 50k blocks and will be concatenated together to and converted back into
						seg.AddField(2,"base64File");
					#endregion ZEF - Encapsulated Data Format
					#region NTE - Notes and Comments
					seg=new HL7DefSegment();
					msg.AddSegment(seg,9,true,true,SegmentNameHL7.NTE);
						//Fields-------------------------------------------------------------------------------------------------------------
						//NTE.2, Comment Source, ID data type
						//LabCorp supported values: 'L' - Laboratory is the source of comment, 'AC' - Accession Comment,
						//'RC' - Result comment, 'RI' - Normal Comment, 'UK' - Undefined comment type
						//We might pull out the source and prepend it to the note, but we won't explicitly add it to the definition or store it separately
						//NTE.3, Comment Text, FT data type (formatted text)
						//Stored in the medlabresult.Note field
						seg.AddField(3,"obsNote");
					#endregion NTE - Notes and Comments
					#region SPM - Specimen
					seg=new HL7DefSegment();
					msg.AddSegment(seg,10,true,true,SegmentNameHL7.SPM);
						//Fields-------------------------------------------------------------------------------------------------------------
						//SPM.2, Specimen ID.
						//Unique ID of the specimen as sent on the specimen container.  Same as the value in ORC.2.
						seg.AddField(2,"specimenID");
						//SPM.4, Specimen Type
						//SPM.8, Specimen Source Site
						//SPM.4, Specimen Source Site Modifier
						//SPM.14, Specimen Description.  Text field used to send additional information about the specimen
						seg.AddField(14,"specimenDescript");
						//SPM.17, Date/Time Specimen Collected
						seg.AddField(17,"dateTimeSpecimen");
					#endregion SPM - Specimen
					#region ZPS - Place of Service
					seg=new HL7DefSegment();
					msg.AddSegment(seg,11,true,false,SegmentNameHL7.ZPS);
						//Fields-------------------------------------------------------------------------------------------------------------
						//ZPS.2, Facility Mnemonic.  Footnote ID for the lab facility used to reference this segment from OBX.15.
						seg.AddField(2,"facilityID");
						//ZPS.3, Facility Name.
						seg.AddField(3,"facilityName");
						//ZPS.4, Facility Address.
						//Address^^City^State^Zip
						seg.AddField(4,"facilityAddress");
						//ZPS.5, Facility Phone.  (LabCorp document says numberic characters only, so we can assume no dashes or parentheses.)
						seg.AddField(5,"facilityPhone");
						//ZPS.7, Facility Director.
						//Title^LName^FName^MiddleI
						seg.AddField(7,"facilityDirector");
					#endregion ZPS - Place of Service
				#endregion ORU - Unsolicited Observation Message
			#endregion Inbound Messages
			#region Outbound Messages
				//Results only interface for now, so no outbound messages yet
				//In the future, we will implement the orders portion of the interface and have outbound ORM messages
			#endregion Outbound Messages
			return def;
		}

	}
}

