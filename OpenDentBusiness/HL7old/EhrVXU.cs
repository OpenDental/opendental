using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace OpenDentBusiness.HL7 {
	///<summary>A VXU message is an Unsolicited Vaccination Record Update.  It is a message sent out from Open Dental detailing vaccinations that were given.
	///Implementation based on HL7 version 2.5.1 Immunization Messaging Release 1.4 08/01/2012.  Data types defined on page 52.
	///To view specific HL7 table definitions, see http://hl7.org/implement/standards/fhir/terminologies-v2.html. </summary>
	public class EhrVXU {

		///<summary>Set in constructor and must not be modified.</summary>
		private Patient _pat;
		///<summary>Set in constructor and must not be modified.</summary>
		private List<VaccinePat> _vaccines;
		///<summary>The entire message object after it is successfully built.</summary>
		private MessageHL7 _msg;
		///<summary>Helper variable.</summary>
		private SegmentHL7 _seg;
		///<summary>A variable which is used in multiple places but always has the same value. At class level for convenience.</summary>
		private string cityWhereEntered="";
		///<summary>A variable which is used in multiple places but always has the same value. At class level for convenience.</summary>
		private string stateWhereEntered="";
		private string _sendingFacilityName="";

		///<summary>Creates the Message object and fills it with data.  Vaccines must all be for the same patient.
		///A list of vaccines is passed in so the user can select a subset of vaccines to send for the patient.
		///Throws an exception if validation fails.</summary>
		public EhrVXU(Patient pat,List<VaccinePat> vaccines) {
			string errors=Validate(pat,vaccines);
			if(errors!="") {
				throw new Exception(errors);
			}
			_pat=pat;
			_vaccines=vaccines;
			InitializeVariables();
			BuildMessage();
		}

		private void InitializeVariables() {
			_sendingFacilityName=PrefC.GetString(PrefName.PracticeTitle);
			cityWhereEntered=PrefC.GetString(PrefName.PracticeCity);
			stateWhereEntered=PrefC.GetString(PrefName.PracticeST);
			if(PrefC.HasClinicsEnabled && _pat.ClinicNum!=0) {//Using clinics and a clinic is assigned.
				Clinic clinic=Clinics.GetClinic(_pat.ClinicNum);
				_sendingFacilityName=clinic.Description;
				cityWhereEntered=clinic.City;
				stateWhereEntered=clinic.State;
			}
		}
		
		private void BuildMessage() {
			_msg=new MessageHL7(MessageTypeHL7.VXU);//Message format for VXU is on guide page 160.
			MSH();//MSH segment. Required.  Cardinality [1..1].
			//SFT segment. Optional. Cardinality [0..*].  Undefined and may be locally specified.
			PID();//PID segment.  Required.  Cardinality [1..1].
			PD1();//PD1 segment.  Required if known.  Cardinality [0..1].
			NK1();//NK1 segment.  Required if known.  Cardinality [0..1].
			//PV1 segment.  Optional.  Cardinality [0..1].  Undefined and may be locally specified.
			//PV2 segment.  Optional.  Cardinality [0..1].  Undefined and may be locally specified.
			//GT1 segment.  Optional.  Cardinality [0..*].  Undefined and may be locally specified.
			//Begin Insurance group.  Optional.  Cardinality [0..*].
			//IN1 segment.  Optional.  Cardinality [0..1].  Undefined and may be locally specified.  Guide page 102.
			//IN2 segment.  Optional.  Cardinality [0..1].  Undefined and may be locally specified.  Guide page 102.
			//IN3 segment.  Optional.  Cardinality [0..1].  Undefined and may be locally specified.  Guide page 102.
			//End Insurance group.
			//Begin Order group.
			for(int i=0;i<_vaccines.Count;i++) {
				ORC(_vaccines[i]);//ORC segment.  Required.  Cardinality [1..1].
				//TQ1 segment.  Optional.  Cardinality [0..1]. Undefined and may be locally specified.
				//TQ2 segment.  Optional.  Cardinality [0..1]. Undefined and may be locally specified.
				RXA(_vaccines[i]);//RXA segment.  Required.  Cardinality [1..1].
				RXR(_vaccines[i]);//RXR segment.  Required if known.  Cardinality [0..1].
				OBX(_vaccines[i]);//OBX segment.  Required if known.  Cardinality [0..*].
				NTE();//NTE segment.  Required if known.  Cardinality [0..1].
			}
			//End Order group.
		}
		
		///<summary>Message Segment Header segment.  Required.  Defines intent, source, destination and syntax of the message.  Guide page 104.</summary>
		private void MSH() {
			_seg=new SegmentHL7("MSH"
				+"|"//MSH-1 Field Separator (|).  Required (length 1..1).
				+@"^~\&"//MSH-2 Encoding Characters.  Required (length 4..4).  Component separator (^), then field repetition separator (~), then escape character (\), then Sub-component separator (&).
				+"|Open Dental"//MSH-3 Sending Application.  Required if known (length unspecified).  Value set HL70361  (guide page 229, "no suggested values defined").  Type HD (guide page 65).
				+"|"+_sendingFacilityName//MSH-4 Sending Facility.  Required if known (length unspecified).  Value set HL70362 (guide page 229, "no suggested values defined").  Type HD (guide page 65).
				+"|"//MSH-5 Receiving Application.  Required if known (length unspecified).  Value set HL70361 (guide page 229, "no suggested values defined").  Type HD (guide page 65).
				+"|EHR Facility"//MSH-6 Receiving Facility.  Required if known (length unspecified).  Value set HL70362 (guide page 229, "no suggested values defined").  Type HD (guide page 65).
				+"|"+DateTime.Now.ToString("yyyyMMddHHmmss")//MSH-7 Date/Time of Message.  Required (length 12..19).
				+"|"//MSH-8 Security.  Optional (length unspecified).
				+"|VXU^V04^VXU_V04"//MSH-9 Message Type. Required (length unspecified).
				+"|OD-"+DateTime.Now.ToString("yyyyMMddHHmmss")+"-"+CodeBase.MiscUtils.CreateRandomAlphaNumericString(14)//MSH-10 Message Control ID.  Required (length 1..199).  Our IDs are 32 characters.
				+"|P"//MSH-11 Processing ID.  Required (length unspecified).  P=production.
				+"|2.5.1"//MSH-12 Version ID.  Required (length unspecified).  Must be exactly "2.5.1".
				+"|"//MSH-13 Sequence Number.  Optional (length unspecified).
				+"|"//MSH-14 Continuation Pointer.  Optional (length unspecified).
				+"|NE"//MSH-15 Accept Acknowledgement Type.  Required if known (length unspecified).  Value set HL70155 (AL=Always, ER=Error/rejection conditions only, NE=Never, SU=Successful completion only).
				+"|NE"//MSH-16 Application Acknowledgement Type.  Required if known (length unspecified).  Value set HL70155 (AL=Always, ER=Error/rejection conditions only, NE=Never, SU=Successful completion only).
				//MSH-17 Country Code.  Optional (length unspecified).  Value set HL70399.  Default value is USA.
				//MSH-18 Character Set.  Optional (length unspecified).
				//MSH-19 Principal Language Of Message.  Optional (length unspecified).
				//MSH-20 Alternate Character Set Handling Scheme.  Optional (length unspecified).
				//MSH-21 Message Profile Identifier.  Required if MSH9 component 1 is set to "QBP" or "RSP.  In our case, this field is not required.
			);
			_msg.Segments.Add(_seg);
		}

		///<summary>Next of Kin segment.  Required if known.  Guide page 111.</summary>
		private void NK1() {
			List<Guardian> listGuardians=Guardians.Refresh(_pat.PatNum);
			for(int i=0;i<listGuardians.Count;i++) {//One NK1 segment for each relationship.
				_seg=new SegmentHL7(SegmentNameHL7.NK1);
				_seg.SetField(0,"NK1");
				_seg.SetField(1,(i+1).ToString());//NK1-1 Set ID.  Required (length unspecified).  Cardinality [1..1].
				//NK-2 Name.  Required (length unspecified).  Type XPN (guide page 82).  Cardinarlity [1..1].
				Patient patNextOfKin=Patients.GetPat(listGuardians[i].PatNumGuardian);
				WriteXPN(2,patNextOfKin.FName,patNextOfKin.LName,patNextOfKin.MiddleI,"L");
				//NK1-3 Relationship.  Required.  Cardinality [1..1].  Value set HL70063 (guide page 196).  Type CE.
				GuardianRelationship relat=listGuardians[i].Relationship;
				string strRelatCode="";
				if(relat==GuardianRelationship.Brother) {
					strRelatCode="BRO";
				}
				else if(relat==GuardianRelationship.CareGiver) {
					strRelatCode="CGV";
				}
				else if(relat==GuardianRelationship.Child) {
					strRelatCode="CHD";//This relationship type is not documented in the guide, but it is defined as part of HL7 0063 in a more recent publication.  We added because it seemed like a basic relationship type.
				}
				else if(relat==GuardianRelationship.Father) {
					strRelatCode="FTH";
				}
				else if(relat==GuardianRelationship.FosterChild) {
					strRelatCode="FCH";
				}
				else if(relat==GuardianRelationship.Friend) {
					strRelatCode="FND";//This relationship type is not documented in the guide, but it is defined as part of HL7 0063 in a more recent publication.  We added because it seemed like a basic relationship type.
				}
				else if(relat==GuardianRelationship.Grandchild) {
					strRelatCode="GCH";//This relationship type is not documented in the guide, but it is defined as part of HL7 0063 in a more recent publication.  We added because it seemed like a basic relationship type.
				}
				else if(relat==GuardianRelationship.Grandfather) { //This status is from our older guardian implementation.
					strRelatCode="GRD";//Grandparent
				}
				else if(relat==GuardianRelationship.Grandmother) { //This status is from our older guardian implementation.
					strRelatCode="GRD";//Grandparent
				}
				else if(relat==GuardianRelationship.Grandparent) {
					strRelatCode="GRD";
				}
				else if(relat==GuardianRelationship.Guardian) {
					strRelatCode="GRD";
				}
				else if(relat==GuardianRelationship.LifePartner) {
					strRelatCode="DOM";//This relationship type is not documented in the guide, but it is defined as part of HL7 0063 in a more recent publication.  We added because it seemed like a basic relationship type.
				}
				else if(relat==GuardianRelationship.Mother) {
					strRelatCode="MTH";
				}
				else if(relat==GuardianRelationship.Other) {
					strRelatCode="OTH";
				}
				else if(relat==GuardianRelationship.Parent) {
					strRelatCode="PAR";
				}
				else if(relat==GuardianRelationship.Self) {
					strRelatCode="SEL";
				}
				else if(relat==GuardianRelationship.Sibling) {
					strRelatCode="SIB";
				}
				else if(relat==GuardianRelationship.Sister) {
					strRelatCode="SIS";
				}
				else if(relat==GuardianRelationship.Sitter) { //This status is from our older guardian implementation.
					strRelatCode="CGV";//Caregiver
				}
				else if(relat==GuardianRelationship.Spouse) {
					strRelatCode="SPO";
				}
				else if(relat==GuardianRelationship.Stepchild) {
					strRelatCode="SCH";
				}
				else if(relat==GuardianRelationship.Stepfather) { //This status is from our older guardian implementation.
					strRelatCode="PAR";//parent
				}
				else if(relat==GuardianRelationship.Stepmother) { //This status is from our older guardian implementation.
					strRelatCode="PAR";//parent
				}
				else {
					continue;//Skip the entire segment if the relationship is not known.  Should not happen.
				}
				WriteCE(3,strRelatCode,relat.ToString(),"HL70063");
				//NK-4 Address.  Required if known (length unspecified).  Cardinality [0..1].  Type XAD (guide page 74).  The first instance must be the primary address.
				WriteXAD(4,patNextOfKin.Address,patNextOfKin.Address2,patNextOfKin.City,patNextOfKin.State,patNextOfKin.Zip);
				//NK-5 Phone Number.  Required if known.  Cardinality [0..*].  Type XTN (guide page 84).  The first instance shall be the primary phone number.
				WriteXTN(5,"PRN","PH","F",patNextOfKin.HmPhone,"PRN","CP","",patNextOfKin.WirelessPhone);
				//NK-6 Business Phone Number.  Optional.  Type XTN (guide page 84).
				WriteXTN(6,"WPN","PH","",patNextOfKin.WkPhone);
				//NK-7 Contact Role.  Optional.
				//NK-8 Start Date.  Optional.
				//NK-9 End Date.  Optional.
				//NK-10 Next of Kin/Associated Parties Job Title.  Optional.
				//NK-11 Next of Kin/Associated Parties Job Code/Class.  Optional.
				//NK-12 Next of Kin/Associated Parties Employee Number.  Optional.
				//NK-13 Organization Name - NK1.  Optional.
				//NK-14 Marital Status.  Optional.
				//NK-15 Administrative Sex.  Optional.
				//NK-16 Date/Time of Birth.  Optional.
				//NK-17 Living Dependency.  Optional.
				//NK-18 Ambulatory Status.  Optional.
				//NK-19 Citizenship.  Optional.
				//NK-20 Primary Language.  Optional.
				//NK-21 Living Arrangement.  Optional.
				//NK-22 Publicity Code.  Optional.
				//NK-23 Protection Indicator.  Optional.
				//NK-24 Student Indicator.  Optional.
				//NK-25 Religion.  Optional.
				//NK-26 Mother's Maiden Name.  Optional.
				//NK-27 Nationality.  Optional.
				//NK-28 Ethnic Group.  Optional.
				//NK-29 Contact Reason.  Optional.
				//NK-30 Contact Person's Name.  Optional.
				//NK-31 Contact Person's Telephone Number.  Optional.
				//NK-32 Contact Person's Address.  Optional.
				//NK-33 Next of Kin/Associated Party's Identifiers.  Optional.
				//NK-34 Job Status.  Optional.
				//NK-35 Race.  Optional.
				//NK-36 Handicap.  Optional.
				//NK-37 Contact Person Social Security Number.  Optional.
				//NK-38 Next of Kin Birth Place.  Optional.
				//NK-39 VIP Indicator.  Optional.
				_msg.Segments.Add(_seg);
			}
		}

		///<summary>Note segment.  Required if known.  Guide page 116.</summary>
		private void NTE() {
		}

		///<summary>Observation Result segment.  Required if known.  The basic format is question and answer.  Guide page 116.</summary>
		private void OBX(VaccinePat vaccine) {
			List<VaccineObs> listVaccineObservations=VaccineObses.GetForVaccine(vaccine.VaccinePatNum);
			for(int i=0;i<listVaccineObservations.Count;i++) {
				VaccineObs vaccineObs=listVaccineObservations[i];
				_seg=new SegmentHL7(SegmentNameHL7.OBX);
				_seg.SetField(0,"OBX");
				_seg.SetField(1,(i+1).ToString());//OBX-1 Set ID - OBX.  Required (length 1..4).  Cardinality [1..1].
				//OBX-2 Value Type.  Required (length 2..3).  Cardinality [1..1].  Value Set HL70125 (constrained, not in guide).  CE=Coded Entry,DT=Date,NM=Numeric,ST=String,TS=Time Stamp (Date & Time).
				if(vaccineObs.ValType==VaccineObsType.Dated) {
					_seg.SetField(2,"DT");
				}
				else if(vaccineObs.ValType==VaccineObsType.Numeric) {
					_seg.SetField(2,"NM");
				}
				else if(vaccineObs.ValType==VaccineObsType.Text) {
					_seg.SetField(2,"ST");
				}
				else if(vaccineObs.ValType==VaccineObsType.DateAndTime) {
					_seg.SetField(2,"TS");
				}
				else { //vaccineObs.ValType==VaccineObsType.Coded
					_seg.SetField(2,"CE");
				}
				//OBX-3 Observation Identifier.  Required.  Cardinality [1..1].  Value set NIP003 (25 items).  Type CE.  Purpose is to pose the question that is answered by OBX-5.
				if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DatePublished) {
					WriteCE(3,"29768-9","Date vaccine information statement published","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DatePresented) {
					WriteCE(3,"29769-7","Date vaccine information statement presented","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DatePrecautionExpiration) {
					WriteCE(3,"30944-3","Date of vaccination temporary contraindication and or precaution expiration","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.Precaution) {
					WriteCE(3,"30945-0","Vaccination contraindication and or precaution","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DatePrecautionEffective) {
					WriteCE(3,"30946-8","Date vaccination contraindication and or precaution effective","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.TypeOf) {
					WriteCE(3,"30956-7","Vaccine Type","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.FundsPurchasedWith) {
					WriteCE(3,"30963-3","Funds vaccine purchased with","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DoseNumber) {
					WriteCE(3,"30973-2","Dose number","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.NextDue) {
					WriteCE(3,"30979-9","Vaccines due next","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DateDue) {
					WriteCE(3,"30980-7","Date vaccine due","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DateEarliestAdminister) {
					WriteCE(3,"30981-5","Earliest date to give","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.ReasonForcast) {
					WriteCE(3,"30982-3","Reason applied by forcast logic to project this vaccine","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.Reaction) {
					WriteCE(3,"31044-1","Reaction","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.ComponentType) {
					WriteCE(3,"38890-0","Vaccine component type","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.TakeResponseType) {
					WriteCE(3,"46249-9","Vaccination take-response type","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DateTakeResponse) {
					WriteCE(3,"46250-7","Vaccination take-response date","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.ScheduleUsed) {
					WriteCE(3,"59779-9","Immunization schedule used","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.Series) {
					WriteCE(3,"59780-7","Immunization series","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DoseValidity) {
					WriteCE(3,"59781-5","Dose validity","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.NumDosesPrimary) {
					WriteCE(3,"59782-3","Number of doses in primary immunization series","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.StatusInSeries) {
					WriteCE(3,"59783-1","Status in immunization series","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.DiseaseWithImmunity) {
					WriteCE(3,"59784-9","Disease with presumed immunity","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.Indication) {
					WriteCE(3,"59785-6","Indication for Immunization","LN");
				}
				else if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.FundPgmEligCat) {
					WriteCE(3,"64994-7","Vaccine funding program eligibility category","LN");
				}
				else { //vaccineObs.IdentifyingCode==VaccineObsIdentifier.DocumentType
					WriteCE(3,"69764-9","Document type","LN");
				}
				//OBX-4 Observation Sub-ID.  Required (length 1..20).  Cardinality [1..1].  Type ST.
				if(vaccineObs.VaccineObsNumGroup==0) {
					_seg.SetField(4,vaccineObs.VaccineObsNum.ToString());
				}
				else {//vaccineObs.VaccineObsNumGroup!=0
					_seg.SetField(4,vaccineObs.VaccineObsNumGroup.ToString());
				}
				//OBX-5 Observation Value.  Required. Cardinality [1..1].  Value set varies, depending on the value of OBX-2 (Use type CE if OBX-2 is "CE", otherwise treat as a string).  Purpose is to answer the quesiton posed by OBX-3.
				if(vaccineObs.ValType==VaccineObsType.Coded) {
					string codeDescript=vaccineObs.ValReported.Trim();//If we do not know the description, then the code will also be placed into the description. The testing tool required non-empty entries.
					if(vaccineObs.ValCodeSystem==VaccineObsValCodeSystem.CVX) {
						Cvx cvx=Cvxs.GetByCode(vaccineObs.ValReported);
						codeDescript=cvx.Description;
					}
					else if(vaccineObs.ValCodeSystem==VaccineObsValCodeSystem.HL70064) {
						if(vaccineObs.ValReported.ToUpper()=="V01") {
							codeDescript="Not VFC eligible";
						}
						else if(vaccineObs.ValReported.ToUpper()=="V02") {
							codeDescript="VFC eligible-Medicaid/Medicaid Managed Care";
						}
						else if(vaccineObs.ValReported.ToUpper()=="V03") {
							codeDescript="VFC eligible- Uninsured";
						}
						else if(vaccineObs.ValReported.ToUpper()=="V04") {
							codeDescript="VFC eligible- American Indian/Alaskan Native";
						}
						else if(vaccineObs.ValReported.ToUpper()=="V05") {
							codeDescript="VFC eligible-Federally Qualified Health Center Patient (under-insured)";
						}
						else if(vaccineObs.ValReported.ToUpper()=="V06") {
							codeDescript="Deprecated [VFC eligible- State specific eligibility (e.g. S-CHIP plan)]";
						}
						else if(vaccineObs.ValReported.ToUpper()=="V07") {
							codeDescript="Local-specific eligibility";
						}
						else if(vaccineObs.ValReported.ToUpper()=="V08") {
							codeDescript="Deprecated [Not VFC eligible-underinsured]";
						}
					}
					WriteCE(5,vaccineObs.ValReported.Trim(),codeDescript,vaccineObs.ValCodeSystem.ToString());
				}
				else if(vaccineObs.ValType==VaccineObsType.Dated) {
					DateTime dateVal=DateTime.Parse(vaccineObs.ValReported.Trim());
					_seg.SetField(5,dateVal.ToString("yyyyMMdd"));
				}
				else if(vaccineObs.ValType==VaccineObsType.Numeric) {
					_seg.SetField(5,vaccineObs.ValReported.Trim());
				}
				else if(vaccineObs.ValType==VaccineObsType.DateAndTime) {
					DateTime dateVal=DateTime.Parse(vaccineObs.ValReported.Trim());
					string strDateOut=dateVal.ToString("yyyyMMdd");
					//The testing tool threw errors when there were trailing zeros, even though technically valid.
					if(dateVal.Second>0) {
						strDateOut+=dateVal.ToString("HHmmss");
					}
					else if(dateVal.Minute>0) {
						strDateOut+=dateVal.ToString("HHmm");
					}
					else if(dateVal.Hour>0) {
						strDateOut+=dateVal.ToString("HH");
					}
					_seg.SetField(5,strDateOut);
				}
				else { //vaccineObs.ValType==VaccineObsType.Text
					_seg.SetField(5,vaccineObs.ValReported);
				}
				//OBX-6 Units.  Required if OBX-2 is "NM" or "SN" (SN appears to be missing from definition).
				if(vaccineObs.ValType==VaccineObsType.Numeric) {
					Ucum ucum=Ucums.GetByCode(vaccineObs.UcumCode);
					WriteCE(6,ucum.UcumCode,ucum.Description,"UCUM");
				}
				//OBX-7 References Range.  Optional.
				//OBX-8 Abnormal Flags.  Optional.
				//OBX-9 Probability.  Optional.
				//OBX-10 Nature of Abnormal Test.  Optional.
				_seg.SetField(11,"F");//OBX-11 Observation Result Status.  Required (length 1..1).  Cardinality [1..1].  Value set HL70085 (constrained, guide page 198).  We are expected to use value F=Final.
				//OBX-12 Effective Date of Reference Range Values.  Optional.
				//OBX-13 User Defined Access Checks.  Optional.
				//OBX-14 Date/Time of the Observation.  Required if known.  Cardinality [0..1].
				if(vaccineObs.DateObs.Year>1880) {
					_seg.SetField(14,vaccineObs.DateObs.ToString("yyyyMMdd"));
				}
				//OBX-15 Producer's Reference.  Optional.
				//OBX-16 Responsible Observer.  Optional.
				//OBX-17 Observation Method.  Required if OBX-3.1 is “64994-7”.  Value set CDCPHINVS. Type CE.
				if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.FundPgmEligCat) {
					_seg.SetField(17,vaccineObs.MethodCode.Trim(),"","CDCPHINVS");
				}
				//OBX-18 Equipment Instance Identifier.  Optional.
				//OBX-19 Date/Time of the Analysis.  Optional.
				//OBX-20 Reserved for harmonization with V2.6.  Optional.
				//OBX-21 Reserved for harmonization with V2.6.  Optional.
				//OBX-22 Reserved for harmonization with V2.6.  Optional.
				//OBX-23 Performing Organization Name.  Optional.
				//OBX-24 Performing Organization Address.  Optional.
				//OBX-25 Performing Organization Medical Director.  Optional.
				_msg.Segments.Add(_seg);
			}
		}
		
		///<summary>Order Request segment.  Required.  Guide page 126.</summary>
		private void ORC(VaccinePat vaccine) {
			_seg=new SegmentHL7(SegmentNameHL7.ORC);
			_seg.SetField(0,"ORC");
			_seg.SetField(1,"RE");//ORC-1 Order Control.  Required (length 2).  Cardinality [1..1].  Value set HL70119.  The only allowed value is "RE".
			//ORC-2 Placer Order Number.  Required if known.  Cardinality [0..1].  Type EI (guid page 62).
			//ORC-3 Filler Order Number.  Required.  Cardinality [0..1].  Type EI (guid page 62).  "Shall be the unique ID of the sending system."  The city and state are used to record the region where the vaccine record was filled.
			WriteEI(3,vaccine.VaccinePatNum.ToString(),vaccine.FilledCity.Trim(),vaccine.FilledST.Trim());
			//ORC-4 Placer Group Number.  Optional.
			//ORC-5 Order Status.  Optional.
			//ORD-6 Response Flag.  Optional.
			//ORD-7 Quantity/Timing.  No longer used.
			//ORD-8 Parent.  Optional.
			//ORD-9 Date/Time of Transaction.  Optional.
			//ORD-10 Entered By.  Required if known.  Cardinality [0..1].  Type XCN.  This is the person that entered the immunization record into the system.
			Userod userod=Userods.GetUser(vaccine.UserNum);//Can be null if vaccine.UserNum=0 for older records before the vaccine.UserNum column existed.
			if(userod!=null) {
				if(userod.ProvNum!=0) {
					Provider provEnteredBy=Providers.GetProv(userod.ProvNum);
					WriteXCN(10,provEnteredBy.FName,provEnteredBy.LName,provEnteredBy.MI,vaccine.UserNum.ToString(),cityWhereEntered,stateWhereEntered,"D");
				}
				else if(userod.EmployeeNum!=0) {
					Employee employee=Employees.GetEmp(userod.EmployeeNum);
					WriteXCN(10,employee.FName,employee.LName,employee.MiddleI,vaccine.UserNum.ToString(),cityWhereEntered,stateWhereEntered,"D");
				}
			}
			//ORD-11 Verified By.  Optional.
			//ORD-12 Ordering Provider.  Required if known. Cardinality [0..1].  Type XCN.  This shall be the provider ordering the immunization.  It is expected to be empty if the immunization record is transcribed from a historical record.
			Provider provOrdering=Providers.GetProv(vaccine.ProvNumOrdering);//Can be null if vaccine.ProvNumOrdering is zero.
			if(provOrdering!=null) {
				WriteXCN(12,provOrdering.FName,provOrdering.LName,provOrdering.MI,provOrdering.ProvNum.ToString(),cityWhereEntered,stateWhereEntered,"L");
			}
			//ORD-13 Enterer's Location.  Optional.
			//ORD-14 Call Back Phone Number.  Optional.
			//ORD-15 Order Effective Date/Time.  Optional.
			//ORD-16 Order Control Code Reason.  Optional.
			//ORD-17 Entering Organization.  Optional.
			//ORD-18 Entering Device.  Optional.
			//ORD-19 Action By.  Optional.
			//ORD-20 Advanced Beneficiary Notice Code.  Optional.
			//ORD-21 Ordering Facility Name.  Optional.
			//ORD-22 Ordering Facility Address.  Optional.
			//ORD-23 Ordering Facility.  Optional.
			//ORD-24 Order Provider Address.  Optional.
			//ORD-25 Order Status Modifier.  Optional.
			//ORD-26 Advanced Beneficiary Notice Override Reason.  Optional.
			//ORD-27 Filler's Expected Availability Date/Time.  Optional.
			//ORD-28 Confidentiality Code.  Optional.
			//ORD-29 Order Type.  Optional.
			//ORD-30 Enterer Authorization Mode.  Optional.
			//ORD-31 Parent Universal Service Modifier.  Optional.
			_msg.Segments.Add(_seg);
		}

		///<summary>Patient Demographic segment.  Required if known.  Additional demographics.  Guide page 132.</summary>
		private void PD1() {
			_seg=new SegmentHL7(SegmentNameHL7.PD1);
			_seg.SetField(0,"PD1");
			//PD1-1 Living Dependency.  Optional.  Cardinality [0..1].
			//PD1-2 Living Arrangement.  Optional.  Cardinality [0..1].
			//PD1-3 Patient Primary Facility.  Optional.  Cardinality [0..1].
			//PD1-4 Patient Primary Care Provider Name & ID Number.  Optional.  Cardinality [0..1].
			//PD1-5 Student Indicator.  Optional.  Cardinality [0..1].
			//PD1-6 Handicap.  Optional.  Cardinality [0..1].
			//PD1-7 Living Will Code.  Optional.  Cardinality [0..1].
			//PD1-8 Organ Donor Code.  Optional.  Cardinality [0..1].
			//PD1-9 Separate Bill.  Optional.  Cardinality [0..1].
			//PD1-10 Duplicate Patient.  Optional.  Cardinality [0..1].
			//PD1-11 Publicity Code.  Required if known (length 2..2).  Cardinality [0..1].  Value set HL70215 (guide page 209).  Type CE.
			if(_pat.PreferRecallMethod==ContactMethod.DoNotCall) {
				WriteCE(11,"07","Recall only - no calls","HL70215");
			}
			else if(_pat.PreferRecallMethod==ContactMethod.None) {
				WriteCE(11,"01","No reminder/Recall","HL70215");
			}
			else {
				WriteCE(11,"02","Reminder/Recall - any method","HL70215");
			}
			//PD1-12 Protection Indicator.  Required if known (length 1..1).  Cardinality [0..1].  Value set HL70136 (guide page 199).  Allowed values are "Y" for yes, "N" for no, or blank for unknown.
			EhrPatient ehrPatient=EhrPatients.Refresh(_pat.PatNum);
			if(ehrPatient.VacShareOk==YN.Yes) {
				_seg.SetField(12,"N");//Do not protect.
			}
			else if(ehrPatient.VacShareOk==YN.No) {
				_seg.SetField(12,"Y");//Protect
			}
			//PD1-13 Protection Indicator Date Effective.  Required if PD1-12 is not blank (length unspecified).  Cardinality [0..1].
			if(ehrPatient.VacShareOk!=YN.Unknown) {
				_seg.SetField(13,_pat.DateTStamp.ToString("yyyyMMdd"));
			}
			//PD1-14 Place of Worship.  Optional (length unspecified).  Cardinality [0..1].
			//PD1-15 Advance Directive Code.  Optional (length unspecified).  Cardinality [0..1].
			//PD1-16 Immunization Registry Status.  Required if known (length unspecified).  Cardinality [0..1].  Value set HL70441 (guide page 232).  The word "registry" refers to the EHR.
			if(_pat.PatStatus==PatientStatus.Patient) {
				_seg.SetField(16,"A");//Active
			}
			else {
				_seg.SetField(16,"I");//Inactive--Unspecified
			}
			//PD1-17 Immunization Registry Status Effective Date.  Required if PD1-16 is not blank.  Cardinality [0..1].
			_seg.SetField(17,DateTime.Today.ToString("yyyyMMdd"));
			//PD1-18 Publicity Code Effective Date.  Required if PD1-11 is not blank.
			_seg.SetField(18,DateTime.Today.ToString("yyyyMMdd"));
			//PD1-19 Military Branch.  Optional.
			//PD1-20 Military Rank/Grade.  Optional.
			//PD1-21 Military Status.  Optional.
			_msg.Segments.Add(_seg);
		}

		///<summary>Patient Identifier segment.  Required.  Guide page 137.</summary>
		private void PID() {
			_seg=new SegmentHL7(SegmentNameHL7.PID);
			_seg.SetField(0,"PID");
			_seg.SetField(1,"1");//PID-1 Set ID - PID.  Required if known.  Cardinality [0..1].  Must be "1" for the first occurrence.  Not sure why there would ever be more than one.
			//PID-2 Patient ID.  No longer used.
			//PID-3 Patient Identifier List.  Required.  Cardinality [1..*].  Type CX (see guide page 58 for type definition).
			_seg.SetField(3,
				_pat.PatNum.ToString(),//PID-3.1 ID Number.  Required (length 1..15).
				"",//PID-3.2 Check Digit.  Optional (length 1..1).
				"",//PID-3.3 Check Digit Scheme.  Required if PID-3.2 is specified.  Not required for our purposes.  Value set HL70061.
				"Open Dental",//PID-3.4 Assigning Authority.  Required.  Value set HL70363.
				"MR"//PID-3.5 Identifier Type Code.  Required (length 2..5).  Value set HL70203.  MR=medical record number.
				//PID-3.6 Assigning Facility.  Optional (length undefined).
				//PID-3.7 Effective Date.  Optional (length 4..8).
				//PID-3.8 Expiration Date.  Optional (length 4..8).
				//PID-3.9 Assigning Jurisdiction.  Optional (length undefined).
				//PID-3.10 Assigning Agency or Department.  Optional (length undefined).
			);
			if(_pat.SSN.Trim()!="") {
				_seg.RepeatField(3,
					_pat.SSN.Trim(),//PID-3.1 ID Number.  Required (length 1..15).
					"",//PID-3.2 Check Digit.  Optional (length 1..1).
					"",//PID-3.3 Check Digit Scheme.  Required if PID-3.2 is specified.  Not required for our purposes.  Value set HL70061.
					"Open Dental",//PID-3.4 Assigning Authority.  Required.  Value set HL70363.
					"SS"//PID-3.5 Identifier Type Code.  Required (length 2..5).  Value set HL70203.  SS=Social Security Number.
						//PID-3.6 Assigning Facility.  Optional (length undefined).
						//PID-3.7 Effective Date.  Optional (length 4..8).
						//PID-3.8 Expiration Date.  Optional (length 4..8).
						//PID-3.9 Assigning Jurisdiction.  Optional (length undefined).
						//PID-3.10 Assigning Agency or Department.  Optional (length undefined).
				);
			}
			//PID-4 Alternate Patient ID - 00106.  No longer used.
			WriteXPN(5,_pat.FName,_pat.LName,_pat.MiddleI,"L");//PID-5 Patient Name.  Required (length unspecified).  Cardinality [1..*].  Type XPN.  The first repetition must contain the legal name.
			EhrPatient ehrPatient=EhrPatients.Refresh(_pat.PatNum);
			WriteXPN(6,ehrPatient.MotherMaidenFname,ehrPatient.MotherMaidenLname,"","M");//PID-6 Mother's Maiden Name.  Required if known (length unspecified).  Cardinality [0..1].  Type XPN.
			//PID-7 Date/Time of Birth.  Required.  Cardinality [1..1].  We must specify "UNK" if unknown.
			if(_pat.Birthdate.Year<1880) {
				_seg.SetField(7,"UNK");
			}
			else {
				_seg.SetField(7,_pat.Birthdate.ToString("yyyyMMdd"));
			}
			WriteGender(8,_pat.Gender);//PID-8 Administrative Sex.  Required if known.  Cardinality [0..1].  Value set HL70001.
			//PID-9 Patient Alias.  No longer used.
			//PID-10 Race.  Required if known.  Cardinality [0..*].  Value set HL70005 (guide page 194).  Each race definition must be type CE.  Type CE is defined on guide page 53.
			List<PatientRace> listPatientRaces=PatientRaces.GetForPatient(_pat.PatNum);
			List<PatientRace> listPatRacesFiltered=new List<PatientRace>();
			bool isHispanicOrLatino=false;
			for(int i=0;i<listPatientRaces.Count;i++) {
				if(listPatientRaces[i].IsEthnicity) {
					if(listPatientRaces[i].CdcrecCode=="2186-5") {//Not hispanic
						//Nothing to do. Flag is set to false by default.
					}
					else if(listPatientRaces[i].CdcrecCode!=PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE) {
						isHispanicOrLatino=true;//All other ethnicities are Hispanic
					}
				}
				else {
					if(listPatientRaces[i].CdcrecCode==PatientRace.DECLINE_SPECIFY_RACE_CODE) {
						listPatRacesFiltered.Clear();
						break;
					}
					listPatRacesFiltered.Add(listPatientRaces[i]);
				}
			}
			for(int i=0;i<listPatRacesFiltered.Count;i++) {
				string strRaceCode=listPatRacesFiltered[i].CdcrecCode;
				string strRaceName=listPatRacesFiltered[i].Description;
				_seg.SetOrRepeatField(10,
					strRaceCode,//PID-10.1 Identifier.  Required (length 1..50).
					strRaceName,//PID-10.2  Text.  Required if known (length 1..999). Human readable text that is not further used.
					"HL70005"//PID-10.3 Name of Coding System.  Required (length 1..20).
					//PID-10.4 Alternate Identifier.  Required if known (length 1..50).
					//PID-10.5 Alternate Text.  Required if known (length 1..999).
					//PID-10.6 Name of Alternate Coding system.  Required if PID-10.4 is not blank.
				);
			}
			//PID-11 Patient Address.  Required if known (length unspecified).  Cardinality [0..*].  Type XAD (guide page 74).  First repetition must be the primary address.
			WriteXAD(11,_pat.Address,_pat.Address2,_pat.City,_pat.State,_pat.Zip);
			//PID-12 County Code.  No longer used.
			//PID-13 Phone Number - Home.  Required if known (length unspecified).  Cardinality [0..*].  Type XTN (guide page 84).  The first number must be the primary number (if email only, then blank phone followed by email).
			WriteXTN(13,"PRN","PH","F",_pat.HmPhone,"PRN","CP","",_pat.WirelessPhone,"NET","Internet","",_pat.Email);
			//PID-14 Phone Number - Business.  Optional.
			//PID-15 Primary Language.  Optional.
			//PID-16 Marital Status.  Optional.
			//PID-17 Religion.  Optional.
			//PID-18 Patient Account Number.  Optional.
			//PID-19 SSN Number - Patient.  No longer used.
			//PID-20 Driver's License Number - Patient.  No longer used.
			//PID-21 Mother's Identifier.  No longer used.
			//PID-22 Ethnic Group.  Required if known (length unspecified).  Cardinality [0..1].  Value set HL70189 (guide page 201).  Type CE.
			if(listPatRacesFiltered.Count>0) {//The user specified a race and ethnicity and did not select the decline to specify option.
				if(isHispanicOrLatino) {
					WriteCE(22,"2135-2","Hispanic or Latino","CDCREC");
				}
				else {//Not hispanic or latino.
					WriteCE(22,"2186-5","not Hispanic or Latino","CDCREC");
				}
			}
			//PID-23 Birth Place.  Optional.  Cardinaility [0..1].
			//PID-24 Multiple Birth Indicator.  Optional.  Cardinaility [0..1].
			//PID-25 Birth Order.  Required when PID-24 is set to "Y".  Cardinaility [0..1].
			//PID-26 Citizenship.  Optional.  Cardinaility [0..1].
			//PID-27 Veterans Military Status.  Optional.  Cardinaility [0..1].
			//PID-28 Nationality.  Optional.  Cardinaility [0..1].
			//PID-29 Patient Death Date and Time.  Required if PID-30 is set to "Y".  Cardinaility [0..1].  Date is required, time is not.
			//PID-30 Patient Death Indicator.  Required if known.  Cardinaility [0..1].  Value set HL70136.  Set this field to "Y" if the death date and time year is greater than 1880, otherwise do not set.
			//PID-31 Identity Unknown.  Optional.  Cardinaility [0..1].
			//PID-32 Identity Reliability Code.  Optional.  Cardinaility [0..1].
			//PID-33 Last Update Date/Time.  Optional.  Cardinaility [0..1].
			//PID-34 Last Update Facility.  Optional.  Cardinaility [0..1].
			//PID-35 Species Code.  Optional.  Cardinaility [0..1].
			//PID-36 Breed Code.  Optional.  Cardinaility [0..1].
			//PID-37 Strain.  Optional.  Cardinaility [0..1].
			//PID-38 Production Class Code.  Optional.  Cardinaility [0..1].
			//PID-39 Tribal Citizenship.  Optional.  Cardinaility [0..1].
			_msg.Segments.Add(_seg);
		}

		///<summary>Pharmacy/Treatment Administration segment.  Required.  Guide page 149.</summary>
		private void RXA(VaccinePat vaccine) {
			_seg=new SegmentHL7(SegmentNameHL7.RXA);
			_seg.SetField(0,"RXA");
			_seg.SetField(1,"0");//RXA-1 Give Sub-ID Counter.  Required.  Must be "0".
			_seg.SetField(2,"1");//RXA-2 Administration Sub-ID Counter.  Required.  Must be "1".
			_seg.SetField(3,vaccine.DateTimeStart.ToString("yyyyMMddHHmm"));//RXA-3 Date/Time Start of Administration.  Required.  This segment can also be used to planned vaccinations.
			if(vaccine.DateTimeEnd.Year>1880) {
				_seg.SetField(4,vaccine.DateTimeEnd.ToString("yyyyMMddHHmm"));//RXA-4 Date/Time End of Administration.  Required if known.  Must be same as RXA-3 or blank.  UI forces RXA-4 and RXA-3 to be equal.  This would be blank if for a planned vaccine.
			}
			//RXA-5 Administered Code.  Required.  Cardinality [1..1].  Type CE (guide page 53).  Must be a CVX code.
			VaccineDef vaccineDef=null;
			if(vaccine.CompletionStatus==VaccineCompletionStatus.NotAdministered) {
				WriteCE(5,"998","no vaccine administered","CVX");
			}
			else {
				vaccineDef=VaccineDefs.GetOne(vaccine.VaccineDefNum);
				Cvx cvx=Cvxs.GetByCode(vaccineDef.CVXCode);
				WriteCE(5,cvx.CvxCode,cvx.Description,"CVX");
			}
			//RXA-6 Administered Amount.  Required (length 1..20).  If amount is not known or not meaningful, then use "999".
			if(vaccine.AdministeredAmt>0) {
				_seg.SetField(6,vaccine.AdministeredAmt.ToString());
			}
			else {
				_seg.SetField(6,"999");//Registries that do not collect administered amount should record the value as "999".
			}
			//RXA-7 Administered Units.  Required if RXA-6 is not "999".  Cadinality [0..1].  Type CE (guide page 53).  Value set HL70396 (guide page 231).  Must be UCUM coding.
			if(vaccine.AdministeredAmt>0 && vaccine.DrugUnitNum!=0) {
				DrugUnit drugUnit=DrugUnits.GetOne(vaccine.DrugUnitNum);
				Ucum ucum=Ucums.GetByCode(drugUnit.UnitIdentifier);
				WriteCE(7,ucum.UcumCode,ucum.Description,"UCUM");//UCUM is not in table HL70396, but it there was a note stating that it was required in the guide and UCUM was required in the test cases.
			}
			//RXA-8 Administered Dosage Form.  Optional.
			//RXA-9 Administration Notes.  Required if RXA-20 is "CP" or "PA".  Value set NIP 0001.  Type CE.
			if(vaccine.CompletionStatus==VaccineCompletionStatus.Complete || vaccine.CompletionStatus==VaccineCompletionStatus.PartiallyAdministered) {
				if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.NewRecord) {
					WriteCE(9,"00","New immunization record","NIP001");
				}
				else if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.HistoricalSourceUnknown) {
					WriteCE(9,"01","Historical information - source unspecified","NIP001");
				}
				else if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.HistoricalOtherProvider) {
					WriteCE(9,"02","Historical information - from other provider","NIP001");
				}
				else if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.HistoricalParentsWrittenRecord) {
					WriteCE(9,"03","Historical information - from parent's written record","NIP001");
				}
				else if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.HistoricalParentsRecall) {
					WriteCE(9,"04","Historical information - from parent's recall","NIP001");
				}
				else if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.HistoricalOtherRegistry) {
					WriteCE(9,"05","Historical information - from other registry","NIP001");
				}
				else if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.HistoricalBirthCertificate) {
					WriteCE(9,"06","Historical information - from birth certificate","NIP001");
				}
				else if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.HistoricalSchoolRecord) {
					WriteCE(9,"07","Historical information - from school record","NIP001");
				}
				else if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.HistoricalPublicAgency) {
					WriteCE(9,"08","Historical information - from public agency","NIP001");
				}
			}
			//RXA-10 Administering Provider.  Required if known.  Type XCN.  This is the person who gave the administration or the vaccinaton.  It is not the ordering clinician.
			Provider provAdministering=Providers.GetProv(vaccine.ProvNumAdminister);//Can be null when vaccine.ProvNumAdminister is zero.
			if(provAdministering!=null) {
				WriteXCN(10,provAdministering.FName,provAdministering.LName,provAdministering.MI,provAdministering.ProvNum.ToString(),cityWhereEntered,stateWhereEntered,"L");
			}
			//RXA-11 Administered-at Location.  Required if known.  Type LA2 (guide page 68).  This is the clinic/site where the vaccine was administered.
			WriteLA2(11,_sendingFacilityName);
			//RXA-12 Administered Per (Time Unit).  Optional.
			//RXA-13 Administered Strength.  Optional.
			//RXA-14 Administered Strength Units.  Optional.
			//RXA-15 Substance Lot Number.  Required if the value in RXA-9.1 is "00".  We decided not to send this field if NotAdministered because we observed such behavior in the testing tool.
			if(vaccine.CompletionStatus!=VaccineCompletionStatus.NotAdministered && vaccine.LotNumber.Trim()!="") {
				_seg.SetField(15,vaccine.LotNumber.Trim());
			}
			//RXA-16 Substance Expiration Date.  Required if RXA-15 is not blank.  Must include at least year and month, but day is not required.  We decided not to send this field if NotAdministered because we observed such behavior in the testing tool.
			if(vaccine.CompletionStatus!=VaccineCompletionStatus.NotAdministered && vaccine.DateExpire.Year>1880) {
				_seg.SetField(16,vaccine.DateExpire.ToString("yyyyMMdd"));
			}
			//RXA-17 Substance Manufacturer Name.  Requred if RXA-9.1 is "00".  Cardinality [0..*].  Value set MVX.  Type CE.
			if(vaccine.CompletionStatus!=VaccineCompletionStatus.NotAdministered && vaccineDef.DrugManufacturerNum!=0) {
				DrugManufacturer manufacturer=DrugManufacturers.GetOne(vaccineDef.DrugManufacturerNum);
				WriteCE(17,manufacturer.ManufacturerCode,manufacturer.ManufacturerName,"MVX");
			}
			//RXA-18 Substance/Treatment Refusal Reason.  Required if RXA-20 is "RE".  Cardinality [0..*].  Required when RXA-20 is "RE", otherwise do not send.  Value set NIP002.
			if(vaccine.RefusalReason==VaccineRefusalReason.ParentalDecision) {
				WriteCE(18,"00","Parental decision","NIP002");
			}
			else if(vaccine.RefusalReason==VaccineRefusalReason.ReligiousExemption) {
				WriteCE(18,"01","Religious exemption","NIP002");
			}
			else if(vaccine.RefusalReason==VaccineRefusalReason.Other) {
				WriteCE(18,"02",vaccine.Note,"NIP002");//The reason is required instead of a generic description for this particular situation.
			}
			else if(vaccine.RefusalReason==VaccineRefusalReason.PatientDecision) {
				WriteCE(18,"03","Patient decision","NIP002");
			}
			//RXA-19 Indication.  Optional.
			//RXA-20 Completion Status.  Required if known (length 2..2).  Value set HL70322 (guide page 225).  CP=Complete, RE=Refused, NA=Not Administered, PA=Partially Administered.
			if(vaccine.CompletionStatus==VaccineCompletionStatus.Refused) {
				_seg.SetField(20,"RE");
			}
			else if(vaccine.CompletionStatus==VaccineCompletionStatus.NotAdministered) {
				_seg.SetField(20,"NA");
			}
			else if(vaccine.CompletionStatus==VaccineCompletionStatus.PartiallyAdministered) {
				_seg.SetField(20,"PA");
			}
			else {//Complete (default)
				_seg.SetField(20,"CP");
			}
			//RXA-21 Action code.  Required if known (length 2..2).  Value set HL70323 (guide page 225).  A=Add, D=Delete, U=Update.
			if(vaccine.ActionCode==VaccineAction.Add) {
				_seg.SetField(21,"A");
			}
			else if(vaccine.ActionCode==VaccineAction.Delete) {
				_seg.SetField(21,"D");
			}
			else if(vaccine.ActionCode==VaccineAction.Update) {
				_seg.SetField(21,"U");
			}
			//RXA-22 System Entry Date/Time.  Optional.
			//RXA-23 Administered Drug Strength.  Optional.
			//RXA-24 Administered Drug Strength Volume Units.  Optional.
			//RXA-25 Administered Barcode Identifier.  Optional.
			//RXA-26 Pharmacy Order Type.  Optional.
			_msg.Segments.Add(_seg);
		}

		///<summary>Pharmacy/Treatment Route segment.  Required if known.  Guide page 158.</summary>
		private void RXR(VaccinePat vaccine) {
			if(vaccine.AdministrationRoute==VaccineAdministrationRoute.None) {
				return;//Unspecified.  Therefore unknown and the entire segment is not required.
			}
			_seg=new SegmentHL7(SegmentNameHL7.RXR);
			_seg.SetField(0,"RXR");
			//RXR-1 Route.  Required.  Cardinality [1..1].  Value set HL70162 (guide page 200). Type CE (guide page 53).
			if(vaccine.AdministrationRoute==VaccineAdministrationRoute.Intradermal) {
				WriteCE(1,"ID","Intradermal","HL70162");
			}
			else if(vaccine.AdministrationRoute==VaccineAdministrationRoute.Intramuscular) {
				WriteCE(1,"IM","Intramuscular","HL70162");
			}
			else if(vaccine.AdministrationRoute==VaccineAdministrationRoute.Nasal) {
				WriteCE(1,"NS","Nasal","HL70162");
			}
			else if(vaccine.AdministrationRoute==VaccineAdministrationRoute.Intravenous) {
				WriteCE(1,"IV","Intravenous","HL70162");
			}
			else if(vaccine.AdministrationRoute==VaccineAdministrationRoute.Oral) {
				WriteCE(1,"PO","Oral","HL70162");
			}
			else if(vaccine.AdministrationRoute==VaccineAdministrationRoute.Subcutaneous) {
				WriteCE(1,"SC","Subcutaneous","HL70162");
			}
			else if(vaccine.AdministrationRoute==VaccineAdministrationRoute.Transdermal) {
				WriteCE(1,"TD","Transdermal","HL70162");
			}
			else {//Other
				WriteCE(1,"OTH","Other","HL70162");
			}
			//RXR-2 Administration Site.  Required if known.  Cardinality [0..1].  Value set HL70163 (guide page 201, details where the vaccine was physically administered on the patient's body).
			if(vaccine.AdministrationSite==VaccineAdministrationSite.LeftThigh) {
				WriteCE(2,"LT","LeftThigh","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.LeftVastusLateralis) {
				WriteCE(2,"LVL","LeftVastusLateralis","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.LeftGluteousMedius) {
				WriteCE(2,"LG","LeftGluteousMedius","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.LeftArm) {
				WriteCE(2,"LA","LeftArm","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.LeftDeltoid) {
				WriteCE(2,"LD","LeftDeltoid","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.LeftLowerForearm) {
				WriteCE(2,"LLFA","LeftLowerForearm","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.RightThigh) {
				WriteCE(2,"RT","RightThigh","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.RightVastusLateralis) {
				WriteCE(2,"RVL","RightVastusLateralis","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.RightGluteousMedius) {
				WriteCE(2,"RG","RightGluteousMedius","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.RightArm) {
				WriteCE(2,"RA","RightArm","HL70163");
			}			
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.RightDeltoid) {
				WriteCE(2,"RD","RightArm","HL70163");
			}
			else if(vaccine.AdministrationSite==VaccineAdministrationSite.RightLowerForearm) {
				WriteCE(2,"RLFA","RightLowerForearm","HL70163");
			}			
			//RXR-3 Administration Device.  Optional.
			//RXR-4 Administration Method.  Optional.
			//RXR-5 Routing Instruction.  Optional.
			//RXR-6 Administration Site Modifier.  Optional.
			_msg.Segments.Add(_seg);
		}

		public string GenerateMessage() {
			return _msg.ToString();
		}

		#region Field Helpers

		///<summary>Type CE (guide page 53).  Writes a coded element into the fieldIndex field of the current segment.</summary>
		private void WriteCE(int fieldIndex,string strIdentifier,string strText,string strNameCodeSystem) {
			_seg.SetField(fieldIndex,
				strIdentifier,//CE.1 Identifier.  Required (length 1..50).
				strText,//CE.2 Text.  Required if known (length 1..999). Human readable text that is not further used.
				strNameCodeSystem//CE.3 Name of Coding System.  Required (length 1..20).
				//CE.4 Alternate Identifier.  Required if known (length 1..50).
				//CE.5 Alternate Text.  Required if known (length 1..999).
				//CE.6 Name of Alternate Coding system.  Required if CE.4 is not blank.
			);
		}

		///<summary>Type EI (guid page 62).  Writes an Entity Identifier (order number) into the fieldIndex field of the current segment.</summary>
		private void WriteEI(int fieldIndex,string identifier,string city,string state) {
			_seg.SetField(fieldIndex,
				identifier,//EI.1 Entity Identifier.  Required (length 1..199).
				GetAssigningAuthority(city,state)//EI.2 Namespace ID.  Required if EI.3 is blank (length 1..20).  Value set HL70363 (guide page 229, 3 letter abbreviation for US state, US city, or US territory).
				//EI.3 Universal ID.  Required if EI.1 is blank (length 1..199).
				);//EI.4 Universal ID Type.  Required if EI.3 is not blank (length 6..6).  Value set HL70301 (guide page 224).  Must be "ISO" or blank.
		}

		///<summary>Corresponds to table HL70363 (guide page 229).</summary>
		private string GetAssigningAuthority(string city,string state) {
			string code="";//A value from Value set HL70363 (guide page 229, 3 letter abbreviation for US state, US city, or US territory).
			string st=state.Trim().ToUpper();
			string c=city.Trim().ToUpper().Replace(" ","");
			code=st+"A";//Most of the codes are just the state code followed by an 'A'.  This includes American territories and districts. http://www.itl.nist.gov/fipspubs/fip5-2.htm
			if(st=="IL" && c=="CHICAGO") { //CHICAGO ILLINOIS
				code="CHA";//CHICAGO has thier own code.
			}
			else if(st=="NY" && c=="NEWYORK") { //NEW YORK NEW YORK
				code="BAA";//NEW YORK CITY has their own code.
			}
			else if(st=="PA" && c=="PHILADELPHIA") { //PHILADELPHIA PENNSYLVANIA
				code="PHA";//Philadelphia has their own code.
			}
			else if(st=="PW") { //REPUBLIC PALAU (American territory)
				code="RPA";//This code is one that does not match the pattern for the rest of the codes.
			}
			else if(st=="TX" && c=="SANANTONIO") { //SAN ANTONIO TEXAS
				code="TBA";//SAN ANTONIO has their own code.
			}
			else if(st=="TX" && c=="HOUSTON") { //HOUSTON TEXAS
				code="THA";//HOUSTON has their own code.
			}
			return code;
		}

		///<summary>Type IS (guide page 68).  Writes a string corresponding to table HL70001 (guide page 193) into the fieldIndex field for the current segment.</summary>
		private void WriteGender(int fieldIndex,PatientGender gender) {
			string strGenderCode="U";//unknown
			if(gender==PatientGender.Female) {
				strGenderCode="F";
			}
			if(gender==PatientGender.Male) {
				strGenderCode="M";
			}
			_seg.SetField(fieldIndex,strGenderCode);
		}

		///<summary>Type LA2 (guide page 68).  Writes facility information into the fieldIndex field of the current segment.</summary>
		private void WriteLA2(int fieldIndex,string facilityName) {
			_seg.SetField(fieldIndex,
				"",//LA2.1 Point of Care.  Optional.
				"",//LA2.2 Room.  Optional.
				"",//LA2.3 Bed.  Optional.
				//LA2.4 Facility.  Required.  Type HD (guide page 66).
				facilityName//LA2.4.1 Namespace ID.  Required when LA2.4.2 is blank.  Value sets HL70300 (guide page 224), HL70361 (guide page 229), HL70362 (guide page 229), HL70363 (guide page 229).  Value set used depends on usage.
				//LA2.4.2 Universal ID.  Required when LA2.4.1 is blank.
				//LA2.4.3 Universal ID Type.  Required when LA2.4.2 is not blank.  Value set HL70301 (guide page 224).
				//LA2.5 Location Status.  Optional.
				//LA2.6 Patient Location Type.  Optional.
				//LA2.7 Building.  Optional.
				//LA2.8 Floor.  Optional.
				//LA2.9 Street Address.  Optional.
				//LA2.10 Other Designation.  Optional.
				//LA2.11 City.  Optional.
				//LA2.12 State or Province.  Optional.
				//LA2.13 Zip or Postal Code.  Optional.
				//LA2.14 Country.  Optional.
				//LA2.15 Address Type.  Optional.
				//LA2.16 Other Geographic Designation.  Optional.
				);
		}

		///<summary>Type XAD (guide page 74).  Writes an extended address into the fieldIndex field for the current segment.</summary>
		private void WriteXAD(int fieldIndex,string address1,string address2,string city,string state,string zip) {
			_seg.SetField(fieldIndex,
				address1,//XAD.1 Street Address.  Required if known (length unspecified).  Data type SAD (guide page 72).  The SAD type only requires the first sub-component.
				address2,//XAD.2 Other Designation.  Required if known (length 1..120).
				city,//XAD.3 City.  Required if known (length 1..50).
				state,//XAD.4 State or Province.  Required if known (length 1..50).
				zip,//XAD.5 Zip or Postal Code.  Required if known (length 1..12).
				"USA",//XAD.6 Country.  Required if known (length 3..3).  Value set HL70399.  Defaults to USA.
				"L"//XAD.7 Address Type.  Required (length 1..3).  Value set HL70190 (guide page 202).  C=Current or temporary,P=Permanent,M=Mailing,B=Firm/Business,O=Office,H=Home,L=Legal address,etc...
				//XAD.8 Other Geographic Designation.  Optional.
				//XAD.9 County/Parish Code.  Optional.
				//XAD.10 Census Tract.  Optional.
				//XAD.11 Address Representation Code.  Optional.
				//XAD.12 Address Validity Range.  No longer used.
				//XAD.13 Effective Date.  Optional.
				//XAD.14 Expiration Date.  Optional.
			);
		}

		///<summary>Type XCN (guide page 77).  Writes user name and id into the fieldIndex field for the current segment.
		///Either the fName and lName must be specified, or id and city and state must be specified. All fields may be specified.
		///Allowed values for nameTypeCode: A=Alias name,L=Legal name,D=Display name,M=Maiden name,C=Adopted name,B=Name at birth,P=Name of partner/spouse,U=Unspecified.</summary>
		private void WriteXCN(int fieldIndex,string fName,string lName,string middleI,string id,string city,string state,string nameTypeCode) {
			bool hasName=false;
			if(fName!="" && lName!="") {
				hasName=true;
			}
			bool hasId=false;
			string idModified="";
			string assigningAuthority="";
			if(id!="" && city!="" && state!="") {//All 3 fields must be present, or none of them should be sent.
				hasId=true;
				idModified=id;
				assigningAuthority=GetAssigningAuthority(city,state);
			}
			if(!hasName && !hasId) {
				return;//Nothing valid to write.
			}
			_seg.SetField(fieldIndex,
				idModified,//XCN.1 ID Number.  Required if XCN.2 and XCN.3 are blank.
				lName,//XCN.2 Family Name.  Required if known.
				fName,//XCN.3 Given Name.  Required if known (length 1..30).
				middleI,//XCN.4 Second and Further Given Names or Initials Thereof.  Required if known (length 1..30).
				"",//XCN.5 Suffix.  Optional.
				"",//XCN.6 Prefix.  Optional.
				"",//XCN.7 Degree.  No longer used.
				"",//XCN.8 Source Table.  Optional.
				assigningAuthority,//XCN.9 Assigning Authority.  Required if XCN.1 is not blank.  Value set HL70363 (guide page 229).
				nameTypeCode//XCN.10 Name Type Code.  Required if known (length 1..1).  Value set HL70200 (guide page 203).  A=Alias name,L=Legal name,D=Display name,M=Maiden name,C=Adopted name,B=Name at birth,P=Name of partner/spouse,U=Unspecified.
				//XCN.11 Identifier Check Digit.  Optional.
				//XCN.12 Check Digit Scheme.  Required if XCN.11 is not blank.
				//XCN.13 Identifier Type Code.  Optional.
				//XCN.14 Assigning Facility.  Optional.
				//XCN.15 Name Representation Code.  Optional.
				//XCN.16 Name Context.  Optional.
				//XCN.17 Name Validity Range.  No longer used.
				//XCN.18 Name Assembly Order.  No longer used.
				//XCN.19 Effective Date.  Optional.
				//XCN.20 Expiration Date.  Optional.
				//XCN.21 Professional Suffix.  Optional.
				//XCN.22 Assigning Jurisdiction.  Optional.
				//XCN.23 Assinging Agency or Department.  Optional.
				);
		}

		///<summary>Type XTN (guide page 84).  Writes a phone number or other contact information (such as email address) into the fieldIndex field for the current segment.
		///The arrayContactInfo params list must contain 4 parameters for each piece of contact information, in the following order:
		///1) Telecommunication Use Code from value set HL70201 (guide page 203).
		///2) Telecommunication Equipment Type from value set HL70202 (guide page 203).
		///3) The value "F" to force the field to be written in all cases or empty string to only write the field if the contact information is present.
		///4) The contact infomration (phone number or email address).
		///Can specify 0 or more contacts. The first valid phone number in the list will be written and the other phone numbers will be ignored.</summary>
		private void WriteXTN(int fieldIndex,params string[] arrayContactInfo) {
			int contactCount=0;
			for(int i=0;i<arrayContactInfo.Length;i+=4) {
				string strTeleUseCode=arrayContactInfo[i];
				string strTeleEquipType=arrayContactInfo[i+1];
				string strForce=arrayContactInfo[i+2];
				string strContactInfo=arrayContactInfo[i+3].Trim();
				if(strContactInfo=="" && strForce!="F") {//When the contact info is blank and the information is not forced, then do not output.
					continue;
				}
				contactCount++;
				if(strTeleUseCode=="NET") {//Email address.
					if(contactCount==1) {
						_seg.SetField(fieldIndex,
							"",//XTN.1 Telephone Number.  No longer used.
							strTeleUseCode,//XTN.2 Telecommunication Use Code.  Required.  Value set HL70201 (guide page 203).
							strTeleEquipType,//XTN.3 Telecommunication Equipment Type.  Required if known.  Value set HL70202 (guide page 203).
							strContactInfo//XTN.4 Email Address.  Required when XTN.2 is set to "NET" (length 1..199).
							//XTN.5 Country Code.  Optional.
							//XTN.6 Area/City Code.  Required when XTN.2 is NOT set to "NET" (length 5..5).
							//XTN.7 Local Number.  Required when XTN.2 is NOT set to "NET" (length 7..7).
							//XTN.8 Extension.  Optional.
							//XTN.9 Any Text.  Optional.
							//XTN.10 Extension Prefix.  Optional.
							//XTN.11 Speed Dial Code.  Optional.
							//XTN.12 Unformatted Telephone Number.  Optional.
						);
					}
					else {//At least one contact has already been specified (even if blank).  Repeat the field.  In testing, we were required to make a blank phone number preceed an email address.  This block makes it happen.
						_seg.RepeatField(fieldIndex,
							"",//XTN.1 Telephone Number.  No longer used.
							strTeleUseCode,//XTN.2 Telecommunication Use Code.  Required.  Value set HL70201 (guide page 203).
							strTeleEquipType,//XTN.3 Telecommunication Equipment Type.  Required if known.  Value set HL70202 (guide page 203).
							strContactInfo//XTN.4 Email Address.  Required when XTN.2 is set to "NET" (length 1..199).
							//XTN.5 Country Code.  Optional.
							//XTN.6 Area/City Code.  Required when XTN.2 is NOT set to "NET" (length 5..5).
							//XTN.7 Local Number.  Required when XTN.2 is NOT set to "NET" (length 7..7).
							//XTN.8 Extension.  Optional.
							//XTN.9 Any Text.  Optional.
							//XTN.10 Extension Prefix.  Optional.
							//XTN.11 Speed Dial Code.  Optional.
							//XTN.12 Unformatted Telephone Number.  Optional.
						);
					}
				}
				else {//Phone number.
					string strPhone=TidyPhone(strContactInfo);
					if(strPhone=="") {//Either forced and empty, or the phone number is invalid.
						continue;
					}
					if(contactCount==1) {
						_seg.SetField(fieldIndex,
							"",//XTN.1 Telephone Number.  No longer used.
							strTeleUseCode,//XTN.2 Telecommunication Use Code.  Required.  Value set HL70201 (guide page 203).
							strTeleEquipType,//XTN.3 Telecommunication Equipment Type.  Required if known.  Value set HL70202 (guide page 203).
							"",//XTN.4 Email Address.  Required when XTN.2 is set to "NET" (length 1..199).
							"",//XTN.5 Country Code.  Optional.
							strPhone.Substring(0,3),//XTN.6 Area/City Code.  Required when XTN.2 is NOT set to "NET" (length 5..5).
							strPhone.Substring(3)//XTN.7 Local Number.  Required when XTN.2 is NOT set to "NET" (length 7..7).
							//XTN.8 Extension.  Optional.
							//XTN.9 Any Text.  Optional.
							//XTN.10 Extension Prefix.  Optional.
							//XTN.11 Speed Dial Code.  Optional.
							//XTN.12 Unformatted Telephone Number.  Optional.
						);
					}
					else {//At least one contact has already been specified (even if blank).  Repeat the field.  In testing, we were required to make a blank phone number preceed an email address.  This block allows a phone number after a blank phone number.
						_seg.RepeatField(fieldIndex,
							"",//XTN.1 Telephone Number.  No longer used.
							strTeleUseCode,//XTN.2 Telecommunication Use Code.  Required.  Value set HL70201 (guide page 203).
							strTeleEquipType,//XTN.3 Telecommunication Equipment Type.  Required if known.  Value set HL70202 (guide page 203).
							"",//XTN.4 Email Address.  Required when XTN.2 is set to "NET" (length 1..199).
							"",//XTN.5 Country Code.  Optional.
							strPhone.Substring(0,3),//XTN.6 Area/City Code.  Required when XTN.2 is NOT set to "NET" (length 5..5).
							strPhone.Substring(3)//XTN.7 Local Number.  Required when XTN.2 is NOT set to "NET" (length 7..7).
							//XTN.8 Extension.  Optional.
							//XTN.9 Any Text.  Optional.
							//XTN.10 Extension Prefix.  Optional.
							//XTN.11 Speed Dial Code.  Optional.
							//XTN.12 Unformatted Telephone Number.  Optional.
						);
					}
				}
			}
		}

		///<summary>Removes any characters from the phone number which are not digits.  Returns empty string if the phone number is invalid.</summary>
		private string TidyPhone(string phoneRaw) {
			string strDigits="";
			for(int j=0;j<phoneRaw.Length;j++) {
				if(!Char.IsNumber(phoneRaw,j)) {
					continue;
				}
				if(strDigits=="" && phoneRaw.Substring(j,1)=="1") {
					continue;//skip leading 1.
				}
				strDigits+=phoneRaw.Substring(j,1);
			}
			if(strDigits.Length!=10) {
				return "";//The phone number is invalid.
			}
			return strDigits;
		}

		///<summary>Type XPN (guide page 82).  Writes an person's name into the fieldIndex field for the current segment.
		///The fName and lName cannot be blank.
		///The middleI may be blank.
		///nameTypeCode can be one of: A=Alias Name,L=Legal Name,D=Display Name,M=Maiden Name,C=Adopted Name,B=Name at birth,P=Name of partner/spouse,U=Unspecified.</summary>
		private void WriteXPN(int fieldIndex,string fName,string lName,string middleI,string nameTypeCode) {
			if(fName.Trim()=="" && lName.Trim()=="") {
				return;
			}
			_seg.SetField(fieldIndex,
				lName.Trim(),//XPN.1 Family Name.  Required (length 1..50).  Type FN (guide page 64).  Cardinality [1..1].  The FN type only requires the last name field and it is the first field.
				fName.Trim(),//XPN.2 Given Name.  Required (length 1..30).  Cardinality [1..1].
				middleI,//XPN.3 Second and Further Given Names or Initials Thereof (middle name).  Required if known (length 1..30). 
				"",//XPN.4 Suffix.  Optional.
				"",//XPN.5 Prefix.  Optional.
				"",//XPN.6 Degree.  No longer used.
				nameTypeCode//XPN.7 Name Type Code.  Required if known (length 1..1).  Value set HL70200 (guide page 203).  A=Alias Name,L=Legal Name,D=Display Name,M=Maiden Name,C=Adopted Name,B=Name at birth,P=Name of partner/spouse,U=Unspecified.
				//XPN.8 Name Representation Code.  Optional.
				//XPN.9 Name Context.  Optional.
				//XPN.10 Name Validity Range.  No longer used.
				//XPN.11 Name Assembly Order.  Optional.
				//XPN.12 Effective Date.  Optional.
				//XPN.13 Expiration Date.  Optional.
				//XPN.14 Professional Suffix.  Optional.
				);
		}

		#endregion Field Helpers

		public static string Validate(Patient pat,List<VaccinePat> vaccines) {
			StringBuilder sb=new StringBuilder();
			if(vaccines.Count==0) {
				WriteError(sb,"Must be at least one vaccine.");
			}
			for(int i=0;i<vaccines.Count;i++) {
				VaccinePat vaccine=vaccines[i];
				string vaccineName="not administered";
				VaccineDef vaccineDef=null;
				if(vaccine.CompletionStatus!=VaccineCompletionStatus.NotAdministered) {//Some fields are not used when the vaccine was not administered.
					vaccineDef=VaccineDefs.GetOne(vaccine.VaccineDefNum);
					vaccineName=vaccineDef.VaccineName;
					if(!Cvxs.CodeExists(vaccineDef.CVXCode)) {
						WriteError(sb,"Invalid CVX code '"+vaccineDef.CVXCode+"' for vaccine '"+vaccineDef.VaccineName+"'");
					}
					if(vaccineDef.DrugManufacturerNum!=0) {
						DrugManufacturer manufacturer=DrugManufacturers.GetOne(vaccineDef.DrugManufacturerNum);
						//manufacturer.ManufacturerCode;//TODO: Consider validating MVX codes here. We do not currently store MVX codes.
					}
					if(vaccine.AdministrationNoteCode==VaccineAdministrationNote.NewRecord && vaccine.LotNumber.Trim()=="") {
						WriteError(sb,"Missing lot number.  Required for new records.");
					}
					if(vaccine.FilledCity.Trim()=="") {
						WriteError(sb,"Missing filled city.");
					}
				}
				if(vaccine.AdministeredAmt>0 && vaccine.DrugUnitNum==0) {
					WriteError(sb,"Drug unit missing.  Required when administered amount is specified.");
				}
				if(vaccine.AdministeredAmt>0 && vaccine.DrugUnitNum!=0) {
					DrugUnit drugUnit=DrugUnits.GetOne(vaccine.DrugUnitNum);
					Ucum ucum=Ucums.GetByCode(drugUnit.UnitIdentifier);
					if(ucum==null) {
						WriteError(sb,"Drug unit invalid UCUM code.");
					}
				}
				List<string> stateCodes=new List<string>(new string[] {
					//50 States.
					"AK","AL","AR","AZ","CA","CO","CT","DE","FL","GA",
					"HI","IA","ID","IL","IN","KS","KY","LA","MA","MD",
					"ME","MI","MN","MO","MS","MT","NC","ND","NE","NH",
					"NJ","NM","NV","NY","OH","OK","OR","PA","RI","SC",
					"SD","TN","TX","UT","VA","VT","WA","WI","WV","WY",
					//US Districts
					"DC",
					//US territories. Reference http://www.itl.nist.gov/fipspubs/fip5-2.htm
					"AS","FM","GU","MH","MP","PR","VI",//UM and PW are excluded here, because they are not allowed in HL7 table 0363.
				});
				if(stateCodes.IndexOf(vaccine.FilledST.Trim().ToUpper())==-1) {
					WriteError(sb,"Filled state must be 2 letter state or territory code for the United States.");
				}
				if(vaccine.LotNumber.Trim()!="" && vaccine.DateExpire.Year<1880) {
					WriteError(sb,"Missing date expiration.");
				}
				if(vaccine.CompletionStatus==VaccineCompletionStatus.Refused && vaccine.RefusalReason==VaccineRefusalReason.None) {
					WriteError(sb,"Missing refusal reason.");
				}
				if(vaccine.CompletionStatus!=VaccineCompletionStatus.Refused && vaccine.RefusalReason!=VaccineRefusalReason.None) {
					WriteError(sb,"Since a refusal reason was specified, completion status must be Refused.");
				}
				if(PrefC.HasClinicsEnabled && pat.ClinicNum!=0) {//Using clinics and a clinic is assigned.
					Clinic clinic=Clinics.GetClinic(pat.ClinicNum);
					if(stateCodes.IndexOf(clinic.State.ToUpper())==-1) {
						WriteError(sb,"Clinic '"+clinic.Description+"' state must be 2 letter state or territory code for the United States.");
					}
					if(clinic.City.Trim()=="") {
						WriteError(sb,"Missing clinic '"+clinic.Description+"' city.");
					}
				}
				else {
					if(stateCodes.IndexOf(PrefC.GetString(PrefName.PracticeST).ToUpper())==-1) {
						WriteError(sb,"Practice state must be 2 letter state or territory code for the United States.");
					}
					if(PrefC.GetString(PrefName.PracticeCity).Trim()=="") {
						WriteError(sb,"Missing practice city.");
					}
				}
				if(vaccine.DateTimeStart.Year>1880 && vaccine.DateTimeEnd.Year>1880 && vaccine.DateTimeStart!=vaccine.DateTimeEnd) {
					WriteError(sb,"Stop time must be blank or equal to start time.");
				}
				List<VaccineObs> listVaccineObservations=VaccineObses.GetForVaccine(vaccine.VaccinePatNum);
				for(int j=0;j<listVaccineObservations.Count;j++) {
					VaccineObs vaccineObs=listVaccineObservations[j];
					if(vaccineObs.ValReported.Trim()=="") {
						WriteError(sb,"Missing value for observation with type '"+vaccineObs.ValType.ToString()+"' attached to vaccine '"+vaccineName+"'");
					}
					Ucum ucum=Ucums.GetByCode(vaccineObs.UcumCode);
					if(ucum==null && vaccineObs.ValType==VaccineObsType.Numeric) {
						WriteError(sb,"Invalid unit code (must be UCUM) for observation with type '"+vaccineObs.ValType.ToString()+"' attached to vaccine '"+vaccineName+"'");
					}
					if(vaccineObs.IdentifyingCode==VaccineObsIdentifier.FundPgmEligCat && vaccineObs.MethodCode.Trim()=="") {
						WriteError(sb,"Missing method code for observation with type '"+vaccineObs.ValType.ToString()+"' attached to vaccine '"+vaccineName+"'");
					}
					if(vaccineObs.ValType==VaccineObsType.Coded) {
						//Any value is allowed.
					}
					else if(vaccineObs.ValType==VaccineObsType.Dated) {
						try {
							DateTime.Parse(vaccineObs.ValReported);
						}
						catch(Exception) {
							WriteError(sb,"Observation value is '"+vaccineObs.ValReported+"'.  Must be a valid date for vaccine '"+vaccineName+"'");
						}
					}
					else if(vaccineObs.ValType==VaccineObsType.DateAndTime) {
						try {
							DateTime.Parse(vaccineObs.ValReported);
						}
						catch(Exception) {
							WriteError(sb,"Observation value is '"+vaccineObs.ValReported+"'.  Must be a valid date and time for vaccine '"+vaccineName+"'");
						}
					}
					else if(vaccineObs.ValType==VaccineObsType.Numeric) {
						try {
							double.Parse(vaccineObs.ValReported);
						}
						catch(Exception) {
							WriteError(sb,"Observation value is '"+vaccineObs.ValReported+"'.  Must be a valid number for vaccine '"+vaccineName+"'");
						}
					}
					else if(vaccineObs.ValType==VaccineObsType.Text) {
						//Any value is allowed.
					}
					else { //DateAndTime
						try {
							DateTime.Parse(vaccineObs.ValReported);
						}
						catch(Exception) {
							WriteError(sb,"Observation value is '"+vaccineObs.ValReported+"'.  Must be a valid date and time for vaccine '"+vaccineName+"'");
						}
					}
				}
			}
			return sb.ToString();
		}

		private static void WriteError(StringBuilder sb,string message) {
			if(sb.Length>0) {
				sb.Append("\r\n");
			}
			sb.Append(message);
		}

		#region Examples

		//The following examples are from MU1. The 2 examples below have been edited slightly for our purposes.  They still pass validation.

		//example 1:
		/*
MSH|^~\&|Open Dental||||20110316102457||VXU^V04^VXU_V04|OD-110316102457117|P|2.5.1
PID|||9817566735^^^MPI&2.16.840.1.113883.19.3.2.1&ISO^MR||Johnson^Philip||20070526|M||2106-3^White^HL70005|3345 Elm Street^^Aurora^CO^80011^^M||^PRN^^^^303^5548889|||||||||N^Not Hispanic or Latino^HL70189
ORC|RE
RXA|0|1|201004051600|201004051600|33^Pneumococcal Polysaccharide^CVX|0.5|ml^milliliter^ISO+||||||||1039A||MSD^Merck^HL70227||||A
		 */

		//example7 has two vaccines:
		/*
MSH|^~\&|EHR Application|EHR Facility|PH Application|PH Facility|20110316102838||VXU^V04^VXU_V04|NIST-110316102838387|P|2.5.1
PID|||787478017^^^MPI&2.16.840.1.113883.19.3.2.1&ISO^MR||James^Wanda||19810430|F||2106-3^White^HL70005|574 Wilkins Road^^Shawville^Pennsylvania^16873^^M||^PRN^^^^814^5752819|||||||||N^Not Hispanic or Latino^HL70189
ORC|RE
RXA|0|1|201004051600|201004051600|52^Hepatitis A^HL70292|1|ml^milliliter^ISO+||||||||HAB9678V1||SKB^GLAXOSMITHKLINE^HL70227||||A
ORC|RE
RXA|0|1|201007011330|201007011330|03^Measles Mumps Rubella^HL70292|999|||||||||||||||A
		 */

		#endregion Examples

	}
}
