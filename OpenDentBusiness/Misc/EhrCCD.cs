using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using OpenDentBusiness.FileIO;

namespace OpenDentBusiness {
	public class EhrCCD {

		///<summary>OID: 2.16.840.1.113883.6.96</summary>
		private const string strCodeSystemSnomed="2.16.840.1.113883.6.96";
		///<summary>SNOMED CT</summary>
		private const string strCodeSystemNameSnomed="SNOMED CT";
		///<summary>OID: 2.16.840.1.113883.6.88</summary>
		private const string strCodeSystemRxNorm="2.16.840.1.113883.6.88";
		///<summary>RxNorm</summary>
		private const string strCodeSystemNameRxNorm="RxNorm";
		///<summary>OID: 2.16.840.1.113883.6.1</summary>
		private const string strCodeSystemLoinc="2.16.840.1.113883.6.1";
		///<summary>LOINC</summary>
		private const string strCodeSystemNameLoinc="LOINC";
		///<summary>OID: 2.16.840.1.113883.12.292</summary>
		private const string strCodeSystemCvx="2.16.840.1.113883.12.292";
		///<summary>CVX</summary>
		private const string strCodeSystemNameCvx="CVX";
		///<summary>OID: 2.16.840.1.113883.6.12</summary>
		private const string strCodeSystemCpt4="2.16.840.1.113883.6.12";
		///<summary>CPT-4</summary>
		private const string strCodeSystemNameCpt4="CPT-4";
		///<summary>OID: 2.16.840.1.113883.6.104</summary>
		private const string strCodeSystemIcd9="2.16.840.1.113883.6.104";
		///<summary>ICD9</summary>
		private const string strCodeSystemNameIcd9="ICD9";
		///<summary>OID: 2.16.840.1.113883.6.4</summary>
		private const string strCodeSystemIcd10="2.16.840.1.113883.6.4";
		///<summary>ICD10</summary>
		private const string strCodeSystemNameIcd10="ICD10";
		///<summary>OID: 2.16.840.1.113883.6.101</summary>
		private const string strCodeSystemNucc="2.16.840.1.113883.6.101";
		///<summary>NUCC</summary>
		private const string strCodeSystemNameNucc="NUCC";
		///<summary>OID: 2.16.840.1.113883.4.9</summary>
		private const string strCodeSystemUnii="2.16.840.1.113883.4.9";
		///<summary>UNII</summary>
		private const string strCodeSystemNameUnii="UNII";
		///<summary>OID: 2.16.840.1.113883.6.13</summary>
		private const string strCodeSystemCdt="2.16.840.1.113883.6.13";
		///<summary>CDT codes (ADA codes).</summary>
		private const string strCodeSystemNameCdt="cdt-ADAcodes";

		///<summary>Set each time GenerateCCD() is called. Used by helper functions to avoid sending the patient as a parameter to each helper function.</summary>
		private Patient _patOutCcd=null;
		///<summary>Set each time ValidateAll and ValidateAllergy is called.</summary>
		private static List<Allergy> _listAllergiesFiltered;
		///<summary>Set each time ValidateAll and ValidateEncounter is called.</summary>
		private static List<Encounter> _listEncountersFiltered;
		///<summary>Set each time ValidateAll and ValidateFunctionalStatus is called.</summary>
		private static List<Disease> _listProblemsFuncFiltered;
		///<summary>Set each time ValidateAll and ValidateImmunization is called.</summary>
		private static List<VaccinePat> _listVaccinePatsFiltered;
		///<summary>Set each time ValidateAll and ValidateMedication is called.</summary>
		private static List<MedicationPat> _listMedPatsFiltered;
		///<summary>Set each time ValidateAll and ValidatePlanOfCare is called.</summary>
		private static List<EhrCarePlan> _listEhrCarePlansFiltered;
		///<summary>Set each time ValidateAll and ValidateProblem is called.</summary>
		private static List<Disease> _listProblemsFiltered;
		///<summary>Set each time ValidateAll and ValidateProcedure is called.</summary>
		private static List<Procedure> _listProcsFiltered;
		///<summary>Set each time ValidateAll and ValidateLabResult is called.</summary>
		private static List<EhrLabResult> _listLabResultFiltered;
		///<summary>Set each time ValidateAll and ValidateSocialHistory is called.</summary>
		private static List<EhrMeasureEvent> _listEhrMeasureEventsFiltered;
		///<summary>Set each time ValidateAll and ValidateVitalSign is called.</summary>
		private static List<Vitalsign> _listVitalSignsFiltered;
		///<summary>Instantiated each time GenerateCCD() is called. Used by helper functions to avoid sending the writer as a parameter to each helper function.</summary>
		private XmlWriter _w=null;
		///<summary>Instantiated each time GenerateCCD() is called. Used to generate unique "id" element "root" attribute identifiers. The Ids in this list are random alpha-numeric and 32 characters in length.</summary>
		private HashSet<string> _hashCcdIds;
		///<summary>Instantiated each time GenerateCCD() is called. Used to generate unique "id" element "root" attribute identifiers. The Ids in this list are random GUIDs which are 36 characters in length.</summary>
		private HashSet<string> _hashCcdGuids;

		#region Private Constructor

		///<summary>Constructor is private to limit instantiation to internal use only. All access to this class is static, however, there are private member variables which are used by each instance for ease of access.</summary>
		private EhrCCD() { }

		#endregion

		#region CCD Creation

		///<summary>Generates a Clinical Summary XML document with an appropriate referral string.  Sections can be included/excluded.  Output is for the date specified, but only applied to specific sections.  Throws an exception if validation fails.</summary>
		public static string GenerateClinicalSummary(Patient pat,bool hasAllergy,bool hasEncounter,bool hasFunctionalStatus,bool hasImmunization,bool hasMedication,bool hasPlanOfCare,bool hasProblem,bool hasProcedure,bool hasReferral,bool hasResult,bool hasSocialHistory,bool hasVitalSign,string instructions,DateTime date) {
			string referralReason="Summary of previous appointment requested.";
			EhrCCD ccd=new EhrCCD();
			return ccd.GenerateCCD(pat,referralReason,hasAllergy,hasEncounter,hasFunctionalStatus,hasImmunization,hasMedication,hasPlanOfCare,hasProblem,hasProcedure,hasReferral,hasResult,hasSocialHistory,hasVitalSign,instructions,date);
		}

		///<summary>Generates a Summary of Care XML document with an appropriate referral string. Throws an exception if validation fails.</summary>
		public static string GenerateSummaryOfCare(Patient pat,bool canValidate=true) {
			string referralReason="Transfer of care to another provider.";
			return GenerateCCD(pat,referralReason,canValidate:canValidate);
		}

		///<summary>Generates an Electronic Copy XML document with an appropriate referral string. Throws an exception if validation fails.</summary>
		public static string GenerateElectronicCopy(Patient pat) {
			string referralReason="Patient requested a copy of medical records.";
			return GenerateCCD(pat,referralReason);
		}

		///<summary>Generates a Patient Export XML document with an appropriate referral string. Throws an exception if validation fails.</summary>
		public static string GeneratePatientExport(Patient pat) {
			string referralReason="Patient requested export.";
			return GenerateCCD(pat,referralReason);
		}

		///<summary>Throws an exception if validation fails.</summary>
		private static string GenerateCCD(Patient pat,string referralReason,bool canValidate=true) {
			EhrCCD ccd=new EhrCCD();
			return ccd.GenerateCCD(pat,referralReason,true,true,true,true,true,true,true,true,true,true,true,true,null,DateTime.MinValue,canValidate:canValidate);
		}

		///<summary>Throws an exception if validation fails.</summary>
		private string GenerateCCD(Patient pat,string referralReason,bool hasAllergy,bool hasEncounter,bool hasFunctionalStatus,bool hasImmunization,bool hasMedication,
			bool hasPlanOfCare,bool hasProblem,bool hasProcedure,bool hasReferral,bool hasResult,bool hasSocialHistory,bool hasVitalSign,string instructions,
			DateTime date,bool canValidate=true) 
		{
			Medications.RefreshCache();
			string strErrors="";
			if(canValidate) {
				strErrors=ValidateAll(pat,date);
			}
			else {
				if(pat==null) {
					pat=new Patient();
					pat.Gender=PatientGender.Unknown;
				}
				//pat.PatNum
				if(string.IsNullOrWhiteSpace(pat.SSN)) {
					pat.SSN="";
				}
				if(string.IsNullOrWhiteSpace(pat.Address)) {
					pat.Address="";
				}
				if(string.IsNullOrWhiteSpace(pat.Address2)) {
					pat.Address2="";
				}
				if(string.IsNullOrWhiteSpace(pat.City)) {
					pat.City="";
				}
				if(string.IsNullOrWhiteSpace(pat.State)) {
					pat.State="";
				}
				if(string.IsNullOrWhiteSpace(pat.WirelessPhone)) {
					pat.WirelessPhone="";
				}
				if(string.IsNullOrWhiteSpace(pat.HmPhone)) {
					pat.HmPhone="";
				}
				if(string.IsNullOrWhiteSpace(pat.WkPhone)) {
					pat.WkPhone="";
				}
				if(string.IsNullOrWhiteSpace(pat.FName)) {
					pat.FName="";
				}
				if(string.IsNullOrWhiteSpace(pat.MiddleI)) {
					pat.MiddleI="";
				}
				if(string.IsNullOrWhiteSpace(pat.LName)) {
					pat.LName="";
				}
				if(string.IsNullOrWhiteSpace(pat.Title)) {
					pat.Title="";
				}
				if(string.IsNullOrWhiteSpace(pat.Language)) {
					pat.Language="";
				}
				//Gender
				//Birthdate
				//Position
			}
			if(strErrors!="") {
				throw new ApplicationException(strErrors);
			}
			_patOutCcd=pat;
			_hashCcdIds=new HashSet<string>();//The IDs only need to be unique within each CCD document.
			_hashCcdGuids=new HashSet<string>();//The UUIDs only need to be unique within each CCD document.
			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Encoding=Encoding.UTF8;
			xmlSettings.OmitXmlDeclaration=true;
			xmlSettings.Indent=true;
			xmlSettings.IndentChars="   ";
			StringBuilder strBuilder=new StringBuilder();
			using(_w=XmlWriter.Create(strBuilder,xmlSettings)) {
				#region ClinicalDocument
				_w.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n");
				_w.WriteProcessingInstruction("xml-stylesheet","type=\"text/xsl\" href=\"ccd.xsl\"");
				_w.WriteWhitespace("\r\n");
				_w.WriteStartElement("ClinicalDocument","urn:hl7-org:v3");
				_w.WriteAttributeString("xmlns","xsi",null,"http://www.w3.org/2001/XMLSchema-instance");
				_w.WriteAttributeString("xsi","noNamespaceSchemaLocation",null,"Registry_Payment.xsd");
				_w.WriteAttributeString("xsi","schemaLocation",null,"urn:hl7-org:v3 http://xreg2.nist.gov:8080/hitspValidation/schema/cdar2c32/infrastructure/cda/C32_CDA.xsd");
				#region US Realm Header - page 44
				StartAndEnd("realmCode","code","US");
				StartAndEnd("typeId","root","2.16.840.1.113883.1.3","extension","POCD_HD000040");//template id to assert use of the CCD standard
				_w.WriteComment("US General Header Template");
				TemplateId("2.16.840.1.113883.10.20.22.1.1");
				_w.WriteComment("Conforms to CCD requirements");
				TemplateId("2.16.840.1.113883.10.20.22.1.2");
				Guid();
				StartAndEnd("code","code","34133-9","codeSystemName",strCodeSystemNameLoinc,"codeSystem",strCodeSystemLoinc,"displayName","Summarization of Episode Note");
				_w.WriteElementString("title","Continuity of Care Document");
				TimeElement("effectiveTime",DateTime.Now);
				StartAndEnd("confidentialityCode","code","N","codeSystem","2.16.840.1.113883.5.25");//Fixed value.  Confidentiality Code System.  Codes: N=(Normal), R=(Restricted),V=(Very Restricted)
				StartAndEnd("languageCode","code","en-US");
				#region recordTarget--------------------------------------------------------------------------------------------------------------------------
				Start("recordTarget");
				Start("patientRole");
				//TODO: We might need to assign a global GUID for each office so that the patient can be uniquely identified anywhere in the world.
				StartAndEnd("id","extension",pat.PatNum.ToString(),"root",OIDInternals.GetForType(IdentifierType.Patient).IDRoot);
				if(pat.SSN.Length==9) {
					StartAndEnd("id","extension",pat.SSN,"root","2.16.840.1.113883.4.1");//TODO: We might need to assign a global GUID for each office so that the patient can be uniquely identified anywhere in the world.
				}
				AddressUnitedStates(pat.Address,pat.Address2,pat.City,pat.State);//Validated
				if(pat.WirelessPhone.Trim()!="") {//There is at least one phone, due to validation.
					StartAndEnd("telecom","use","HP","value","tel:"+pat.WirelessPhone.Trim());
				}
				else if(pat.HmPhone.Trim()!="") {
					StartAndEnd("telecom","use","HP","value","tel:"+pat.HmPhone.Trim());
				}
				else if(pat.WkPhone.Trim()!="") {
					StartAndEnd("telecom","use","HP","value","tel:"+pat.WkPhone.Trim());
				}
				Start("patient");
				#region Patient Name - page 77
				Start("name","use","L");
				_w.WriteElementString("given",pat.FName.Trim());//Validated
				if(pat.MiddleI!="") {
					_w.WriteElementString("given",pat.MiddleI.Trim());
				}
				_w.WriteElementString("family",pat.LName.Trim());//Validated
				if(pat.Title!="") {
					Start("suffix","qualifier","TITLE");
					_w.WriteString(pat.Title);
					End("suffix");
				}
				End("name");
				#endregion Patinet Name
				#region Patient Gender - page 52
				string strGender="UN";//Undifferentiated
				if(pat.Gender==PatientGender.Female) {
					strGender="F";
				}
				else if(pat.Gender==PatientGender.Male) {
					strGender="M";
				}
				#endregion Patient Gender
				StartAndEnd("administrativeGenderCode","code",strGender,"codeSystem","2.16.840.1.113883.5.1");//Will always be present, because there are only 3 options and the user is forced to make a choice in the UI.
				DateElement("birthTime",pat.Birthdate);//Validated
				if(pat.Position==PatientPosition.Divorced) {
					StartAndEnd("maritalStatusCode","code","D","displayName","Divorced","codeSystem","2.16.840.1.113883.5.2","codeSystemName","MaritalStatusCode");
				}
				else if(pat.Position==PatientPosition.Married) {
					StartAndEnd("maritalStatusCode","code","M","displayName","Married","codeSystem","2.16.840.1.113883.5.2","codeSystemName","MaritalStatusCode");
				}
				else if(pat.Position==PatientPosition.Widowed) {
					StartAndEnd("maritalStatusCode","code","W","displayName","Widowed","codeSystem","2.16.840.1.113883.5.2","codeSystemName","MaritalStatusCode");
				}
				else {//Single and child
					StartAndEnd("maritalStatusCode","code","S","displayName","Never Married","codeSystem","2.16.840.1.113883.5.2","codeSystemName","MaritalStatusCode");
				}
				List<PatientRace> listPatientRaces=PatientRaces.GetForPatient(pat.PatNum);
				PatientRace patRace=listPatientRaces.FirstOrDefault(x => !x.IsEthnicity) ?? new PatientRace(pat.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE);
				if(patRace.CdcrecCode!=PatientRace.DECLINE_SPECIFY_RACE_CODE) {//The patient did not decline to specify.
					StartAndEnd("raceCode","code",patRace.CdcrecCode,"displayName",patRace.Description,"codeSystem","2.16.840.1.113883.6.238","codeSystemName","Race &amp; Ethnicity - CDC");					
				}
				bool isEthnicitySpecified=listPatientRaces.Any(x => x.IsEthnicity && x.CdcrecCode!=PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE);
				if(isEthnicitySpecified) {
					bool isHispanicOrLatino=listPatientRaces.Any(x => x.IsEthnicity && x.CdcrecCode!=PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE 
						&& x.CdcrecCode!="2186-5");//Cdcrec code for Not Hispanic
					if(isHispanicOrLatino) {
						StartAndEnd("ethnicGroupCode","code","2135-2","displayName","Hispanic or Latino","codeSystem","2.16.840.1.113883.6.238","codeSystemName","Race &amp; Ethnicity - CDC");
					}
					else {//Not hispanic
						StartAndEnd("ethnicGroupCode","code","2186-5","displayName","Not Hispanic or Latino","codeSystem","2.16.840.1.113883.6.238","codeSystemName","Race &amp; Ethnicity - CDC");
					}
				}
				if(_patOutCcd.Language.Trim().Length==3) {
					//This segment is optional, but we added it because it seems to be important to Drummond.
					//We can only allow 3 letter ISO-3 codes. It is possible that the user manually typed something which is 3 characters and is not an ISO code,
					//but we will enhance for that situation later. This should catch 98% of all situations for now.					
					Start("languageCommunication");
					StartAndEnd("languageCode","code",pat.Language.Trim().ToLower());
					End("languageCommunication");
				}
				End("patient");
				End("patientRole");
				End("recordTarget");
				#endregion recordTarget
				#region author--------------------------------------------------------------------------------------------------------------------------------
				//The author element represents the creator of the clinical document.  The author may be a device, or a person.  Section 2.1.2, page 65
				//pat.PrivProv cannot be zero, because of validation below.
				Provider provAuthor=GetCleanProvider(pat.PriProv,canValidate);//Uses primary provider, the primary provider cannot have the IsNotPerson set to true so they must have a first name.
				Start("author");
				TimeElement("time",DateTime.Now);
				Start("assignedAuthor");
				StartAndEnd("id","extension",provAuthor.NationalProvID,"root","2.16.840.1.113883.4.6");//Validated NPI. TODO: We might need to assign a global GUID for each office so that the provider can be uniquely identified anywhere in the world.
				StartAndEnd("code","code",GetTaxonomy(provAuthor),"codeSystem",strCodeSystemNucc,"codeSystemName",strCodeSystemNameNucc);
				AddressUnitedStates(PrefC.GetString(PrefName.PracticeAddress),PrefC.GetString(PrefName.PracticeAddress2),PrefC.GetString(PrefName.PracticeCity),PrefC.GetString(PrefName.PracticeST));//Validated
				string strPracticePhone=PrefC.GetString(PrefName.PracticePhone);//Validated
				strPracticePhone=strPracticePhone.Substring(0,3)+"-"+strPracticePhone.Substring(3,3)+"-"+strPracticePhone.Substring(6);
				StartAndEnd("telecom","use","WP","value","tel:"+strPracticePhone);//Validated
				Start("assignedPerson");
				Start("name");
				_w.WriteElementString("given",provAuthor.FName.Trim());//Validated
				_w.WriteElementString("family",provAuthor.LName.Trim());//Validated
				End("name");
				End("assignedPerson");
				End("assignedAuthor");
				End("author");
				#endregion author
				#region custodian-----------------------------------------------------------------------------------------------------------------------------
				//"Represents the organization in charge of maintaining the document." Section 2.1.5, page 72
				//The custodian is the steward that is entrusted with the care of the document. Every CDA document has exactly one custodian.
				Provider provCustodian=GetCleanProvider(PrefC.GetLong(PrefName.PracticeDefaultProv),canValidate);
				Start("custodian");
				Start("assignedCustodian");
				Start("representedCustodianOrganization");
				StartAndEnd("id","extension",provCustodian.NationalProvID,"root","2.16.840.1.113883.4.6");//Validated NPI. We might need to assign a global GUID for each office so that the provider can be uniquely identified anywhere in the world.
				string custodianTitle=PrefC.GetString(PrefName.PracticeTitle);
				string custodianAddress=PrefC.GetString(PrefName.PracticeAddress);//Validated
				string custodianAddress2=PrefC.GetString(PrefName.PracticeAddress2);//Validated
				string custodianCity=PrefC.GetString(PrefName.PracticeCity);//Validated
				string custodianState=PrefC.GetString(PrefName.PracticeST);//Validated
				string custodianPhone=strPracticePhone;
				if(PrefC.HasClinicsEnabled && _patOutCcd.ClinicNum!=0) {
					Clinic clinicCustodian=Clinics.GetClinic(_patOutCcd.ClinicNum);
					custodianTitle=clinicCustodian.Description;
					custodianAddress=clinicCustodian.Address;//Validated
					custodianAddress2=clinicCustodian.Address2;//Validated
					custodianCity=clinicCustodian.City;//Validated
					custodianState=clinicCustodian.State;//Validated
					custodianPhone=clinicCustodian.Phone;//Validated
				}
				_w.WriteElementString("name",custodianTitle);//Validated
				StartAndEnd("telecom","use","WP","value","tel:"+custodianPhone);//Validated
				AddressUnitedStates(custodianAddress,custodianAddress2,custodianCity,custodianState);//Validated
				End("representedCustodianOrganization");
				End("assignedCustodian");
				End("custodian");
				#endregion custodian
				#region legalAuthenticator--------------------------------------------------------------------------------------------------------------------
				//This element identifies the single person legally responsible for the document and must be present if the document has been legally authenticated.
				Provider provLegal=GetCleanProvider(PrefC.GetLong(PrefName.PracticeDefaultProv),canValidate);			
				if(!provLegal.IsNotPerson) {
					Start("legalAuthenticator");
					TimeElement("time",DateTime.Now);
					StartAndEnd("signatureCode","code","S");
					Start("assignedEntity");
					if(pat.PriProv>0) {
						provLegal=Providers.GetProv(pat.PriProv);
					}
					StartAndEnd("id","root","2.16.840.1.113883.4.6","extension",provLegal.NationalProvID);//Validated NPI. We might need to assign a global GUID for each office so that the provider can be uniquely identified anywhere in the world.
					string legalAuthAddress=PrefC.GetString(PrefName.PracticeAddress);//Validated
					string legalAuthAddress2=PrefC.GetString(PrefName.PracticeAddress2);//Validated
					string legalAuthCity=PrefC.GetString(PrefName.PracticeCity);//Validated
					string legalAuthState=PrefC.GetString(PrefName.PracticeST);//Validated
					string legalAuthPhone=strPracticePhone;
					if(PrefC.HasClinicsEnabled && _patOutCcd.ClinicNum!=0) {
						Clinic clinicAuth=Clinics.GetClinic(_patOutCcd.ClinicNum);
						legalAuthAddress=clinicAuth.Address;//Validated
						legalAuthAddress2=clinicAuth.Address2;//Validated
						legalAuthCity=clinicAuth.City;//Validated
						legalAuthState=clinicAuth.State;//Validated
						legalAuthPhone=clinicAuth.Phone;//Validated
					}
					AddressUnitedStates(legalAuthAddress,legalAuthAddress2,legalAuthCity,legalAuthState);//Validated
					StartAndEnd("telecom","use","WP","value","tel:"+legalAuthPhone);//Validated
					Start("assignedPerson");
					Start("name");
					_w.WriteElementString("given",provLegal.FName.Trim());//Validated
					_w.WriteElementString("family",provLegal.LName.Trim());//Validated
					End("name");
					End("assignedPerson");
					End("assignedEntity");
					End("legalAuthenticator");
				}
				#endregion legalAuthenticator
				#region documentationOf-----------------------------------------------------------------------------------------------------------------------
				Start("documentationOf","typeCode","DOC");
				Start("serviceEvent","classCode","PCPR");
				Start("effectiveTime");
				TimeElement("low",DateTime.Now);
				TimeElement("high",DateTime.Now);
				End("effectiveTime");
				Provider provPri=GetCleanProvider(_patOutCcd.PriProv,canValidate);//Cannot be zero, because of validation below.
				if(!provPri.IsNotPerson) {
					Start("performer","typeCode","PRF");
					Start("assignedEntity");
					if(provPri==null) {
						provPri=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
					}
					StartAndEnd("id","root","2.16.840.1.113883.4.6","extension",provPri.NationalProvID);//Validated NPI. We might need to assign a global GUID for each office so that the provider can be uniquely identified anywhere in the world.
					AddressUnitedStates(PrefC.GetString(PrefName.PracticeAddress),PrefC.GetString(PrefName.PracticeAddress2),PrefC.GetString(PrefName.PracticeCity),PrefC.GetString(PrefName.PracticeST));//Validated
					StartAndEnd("telecom","use","WP","value","tel:"+strPracticePhone);//Validated
					Start("assignedPerson");
					Start("name");
					_w.WriteElementString("given",provPri.FName.Trim());//Validated
					_w.WriteElementString("family",provPri.LName.Trim());//Validated
					End("name");
					End("assignedPerson");
					End("assignedEntity");
					End("performer");
				}
				End("serviceEvent");
				End("documentationOf");
				#endregion documentationOf
				#endregion US Realm Header
				#region Body - The required and optional sections within the body of a CCD are documented starting on page 83.
				_w.WriteComment(@"
=====================================================================================================
Body
=====================================================================================================");
				Start("component");
				Start("structuredBody");
				if(!canValidate) {
					//Skip ones that have validation errors. Set bool to false to skip.
					if(!string.IsNullOrWhiteSpace(ValidateAllergy(pat))) {
						hasAllergy=false;
					}
					if(!string.IsNullOrWhiteSpace(ValidateEncounter(pat,date))) {
						hasEncounter=false;
					}
					if(!string.IsNullOrWhiteSpace(ValidateFunctionalStatus(pat))) {
						hasFunctionalStatus=false;
					}
					if(!string.IsNullOrWhiteSpace(ValidateImmunization(pat))) {
						hasImmunization=false;
					}
					//Instructions
					if(!string.IsNullOrWhiteSpace(ValidateMedication(pat))) {
						hasMedication=false;
					}
					if(!string.IsNullOrWhiteSpace(ValidatePlanOfCare(pat,date))) {
						hasPlanOfCare=false;
					}
					if(!string.IsNullOrWhiteSpace(ValidateProblem(pat))) {
						hasProblem=false;
					}
					if(!string.IsNullOrWhiteSpace(ValidateProcedure(pat,date))) {
						hasProcedure=false;
					}
					//Reason for Referral
					if(!string.IsNullOrWhiteSpace(ValidateLabResults(pat))) {
						hasResult=false;
					}
					if(!string.IsNullOrWhiteSpace(ValidateSocialHistory(pat))) {
						hasSocialHistory=false;
					}
					if(!string.IsNullOrWhiteSpace(ValidateVitalsSign(pat,date))) {
						hasVitalSign=false;
					}
				}
				GenerateCcdSectionAllergies(hasAllergy);
				GenerateCcdSectionEncounters(hasEncounter);
				GenerateCcdSectionFunctionalStatus(hasFunctionalStatus);
				GenerateCcdSectionImmunizations(hasImmunization);
				GenerateCcdSectionInstructions(instructions);
				GenerateCcdSectionMedications(hasMedication);
				GenerateCcdSectionPlanOfCare(hasPlanOfCare);
				GenerateCcdSectionProblems(hasProblem);
				GenerateCcdSectionProcedures(hasProcedure);
				GenerateCcdSectionReasonForReferral(hasReferral,referralReason);
				GenerateCcdSectionResults(hasResult);//Lab Results
				GenerateCcdSectionSocialHistory(hasSocialHistory);
				GenerateCcdSectionVitalSigns(hasVitalSign);
				End("structuredBody");
				End("component");
				#endregion Body
				End("ClinicalDocument");
				#endregion ClinicalDocument
			}
			SecurityLogs.MakeLogEntry(Permissions.Copy,pat.PatNum,"CCD generated");			//Create audit log entry.
			return strBuilder.ToString();
		}

		///<summary>Returns a provider with missing fields if validation is disabled.</summary>
		private Provider GetCleanProvider(long provNum,bool canValidate) {
			Provider provider=Providers.GetProv(provNum);
			if(canValidate) {
				return provider;
			}
			if(provider==null) {
				provider=new Provider();
			}
			//IsNotPerson
			if(string.IsNullOrWhiteSpace(provider.NationalProvID)) {
				provider.NationalProvID="";
			}
			if(string.IsNullOrWhiteSpace(provider.FName)) {
				provider.FName="";
			}
			if(string.IsNullOrWhiteSpace(provider.LName)) {
				provider.LName="";
			}
			return provider;
		}

		///<summary>Helper for GenerateCCD() and GenerateCcdSectionEncounters(). Exactly the same taxonomy codes used for X12 in eclaims.</summary>
		public static string GetTaxonomy(Provider provider) {
			if(provider.TaxonomyCodeOverride!="") {
				return provider.TaxonomyCodeOverride;
			}
			string spec="1223G0001X";//general
			Def provSpec=Defs.GetDef(DefCat.ProviderSpecialties,provider.Specialty);
			if(provSpec==null) {
				return spec;
			}
			switch(provSpec.ItemName) {
				case "General": spec="1223G0001X"; break;
				case "Hygienist": spec="124Q00000X"; break;
				case "PublicHealth": spec="1223D0001X"; break;
				case "Endodontics": spec="1223E0200X"; break;
				case "Pathology": spec="1223P0106X"; break;
				case "Radiology": spec="1223X0008X"; break;
				case "Surgery": spec="1223S0112X"; break;
				case "Ortho": spec="1223X0400X"; break;
				case "Pediatric": spec="1223P0221X"; break;
				case "Perio": spec="1223P0300X"; break;
				case "Prosth": spec="1223P0700X"; break;
				case "Denturist": spec="122400000X"; break;
				case "Assistant": spec="126800000X"; break;
				case "LabTech": spec="126900000X"; break;
			}
			return spec;
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionAllergies(bool hasAllergy) {
			_w.WriteComment(@"
=====================================================================================================
Allergies
=====================================================================================================");
			AllergyDef allergyDef;
			List<Allergy> listAllergiesFiltered=new List<Allergy>();
			if(!hasAllergy) {
				listAllergiesFiltered=new List<Allergy>();
			}
			else {
				listAllergiesFiltered=_listAllergiesFiltered;
			}
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.6.1");//page 230 Allergy template with required entries.
			_w.WriteComment("Allergies section template");
			StartAndEnd("code","code","48765-2","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Allergies");
			_w.WriteElementString("title","Allergies and Adverse Reactions");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listAllergiesFiltered.Count>0 && hasAllergy) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Substance");
				_w.WriteElementString("th","Reaction");
				_w.WriteElementString("th","Allergy Type");
				_w.WriteElementString("th","Status");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listAllergiesFiltered.Count;i++) {
					Allergy allergy=listAllergiesFiltered[i];
					if(allergy.PatNum==0) {
						allergyDef=new AllergyDef();
					}
					else {
						allergyDef=AllergyDefs.GetOne(allergy.AllergyDefNum);
					}
					Start("tr");
					//if(allergyDef.SnomedAllergyTo!="") {//Is Snomed allergy.
					//	Snomed snomedAllergyTo=Snomeds.GetByCode(allergyDef.SnomedAllergyTo);
					//	_w.WriteElementString("td",snomedAllergyTo.SnomedCode+" - "+snomedAllergyTo.Description);
					//}
					//else {//Medication allergy
					Medication med;
					if(allergyDef.MedicationNum==0) {
						if(allergyDef.UniiCode=="") {
							_w.WriteElementString("td","");
						}
						else {
							_w.WriteElementString("td",allergyDef.UniiCode+" - "+allergyDef.Description);
						}
					}
					else {
						med=Medications.GetMedication(allergyDef.MedicationNum);
						_w.WriteElementString("td",med.RxCui.ToString()+" - "+med.MedName);
					}
					//}
					_w.WriteElementString("td",allergy.Reaction);
					_w.WriteElementString("td",AllergyDefs.GetSnomedAllergyDesc(allergyDef.SnomedType));
					_w.WriteElementString("td",allergy.StatusIsActive?"Active":"Inactive");
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listAllergiesFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				Allergy al=new Allergy();
				listAllergiesFiltered.Add(al);
			}
			for(int i=0;i<listAllergiesFiltered.Count;i++) {
				Allergy allergy=listAllergiesFiltered[i];
				if(allergy.PatNum==0) {
					allergyDef=new AllergyDef();
				}
				else {
					allergyDef=AllergyDefs.GetOne(allergy.AllergyDefNum);
				}
				string allergyType="";
				string allergyTypeName="";
				#region Allergy Type
				if(allergyDef.SnomedType==SnomedAllergy.AdverseReactionsToDrug) {
					allergyType="419511003";
					allergyTypeName="Propensity to adverse reaction to drug";
				}
				else if(allergyDef.SnomedType==SnomedAllergy.AdverseReactionsToFood) {
					allergyType="418471000";
					allergyTypeName="Propensity to adverse reaction to food";
				}
				else if(allergyDef.SnomedType==SnomedAllergy.AdverseReactionsToSubstance) {
					allergyType="419199007";
					allergyTypeName="Propensity to adverse reaction to substance";
				}
				else if(allergyDef.SnomedType==SnomedAllergy.AllergyToSubstance) {
					allergyType="418038007";
					allergyTypeName="Allergy to substance";
				}
				else if(allergyDef.SnomedType==SnomedAllergy.DrugAllergy) {
					allergyType="416098002";
					allergyTypeName="Drug allergy";
				}
				else if(allergyDef.SnomedType==SnomedAllergy.DrugIntolerance) {
					allergyType="59037007";
					allergyTypeName="Drug intolerance";
				}
				else if(allergyDef.SnomedType==SnomedAllergy.FoodAllergy) {
					allergyType="414285001";
					allergyTypeName="Food allergy";
				}
				else if(allergyDef.SnomedType==SnomedAllergy.FoodIntolerance) {
					allergyType="235719002";
					allergyTypeName="Food intolerance";
				}
				else if(allergyDef.SnomedType==SnomedAllergy.AdverseReactions) {
					allergyType="420134006";
					allergyTypeName="Adverse reaction";
				}
				else {
					allergyType="";
					allergyTypeName="None";
				}
				#endregion
				Start("entry","typeCode","DRIV");
				Start("act","classCode","ACT","moodCode","EVN");
				TemplateId("2.16.840.1.113883.10.20.22.4.30");//Allergy Problem Act template
				Guid();
				StartAndEnd("code","code","48765-2","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Allergies and adverse reactions");
				//statusCode values allowed: active, suspended, aborted, completed.
				if(allergy.StatusIsActive) {
					StartAndEnd("statusCode","code","active");
				}
				else {
					StartAndEnd("statusCode","code","completed");
				}
				Start("effectiveTime");
				if(allergy.DateTStamp.Year<1880) {
					StartAndEnd("low","nullFlavor","UNK");
					StartAndEnd("high","nullFlavor","UNK");
				}
				else if(allergy.StatusIsActive) {
					StartAndEnd("low","value",allergy.DateTStamp.ToString("yyyyMMdd"));
					StartAndEnd("high","nullFlavor","UNK");
				}
				else {
					StartAndEnd("low","nullFlavor","UNK");
					StartAndEnd("high","value",allergy.DateTStamp.ToString("yyyyMMdd"));
				}
				End("effectiveTime");
				Start("entryRelationship","typeCode","SUBJ");
				Start("observation","classCode","OBS","moodCode","EVN");
				_w.WriteComment("Allergy Observation template");
				TemplateId("2.16.840.1.113883.10.20.22.4.7");
				Guid();
				StartAndEnd("code","code","ASSERTION","codeSystem","2.16.840.1.113883.5.4");//Fixed Value
				StartAndEnd("statusCode","code","completed");//fixed value (required)
				StartAndEnd("effectiveTime","nullFlavor","UNK");//We have no field to store the date the allergy became active. DateTStamp is not the same as the active date.
				Start("value");
				_w.WriteAttributeString("xsi","type",null,"CD");
				if(allergyDef.SnomedType==SnomedAllergy.None) {
					Attribs("nullFlavor","UNK");
				}
				else {
					Attribs("code",allergyType,"displayName",allergyTypeName,"codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
				}
				End("value");
				Start("participant","typeCode","CSM");
				Start("participantRole","classCode","MANU");
				Start("playingEntity","classCode","MMAT");
				//pg. 331 item 9:
				//In an allergy to a specific medication the code SHALL be selected from the ValueSet 2.16.840.1.113883.3.88.12.80.16 Medication Brand Name (code system: RxNorm 2.16.840.1.113883.6.88); Example: 205734		RxNorm	Amoxicillin 25 MG/ML Oral Suspension [Amoxil]
				//Or the ValueSet 2.16.840.1.113883.3.88.12.80.17 Medication Clinical Drug (code system: RxNorm 2.16.840.1.113883.6.88). Example: 313850	RxNorm	Amoxicillin 40 MG/ML Oral Suspensionv 
				//In an allergy to a class of medications the code SHALL be selected from the ValueSet 2.16.840.1.113883.3.88.12.80.18 Medication Drug Class (code system: NDF-RT 2.16.840.1.113883.3.26.1.5). Example: 2-Propanol, Inhibitors
				//In an allergy to a food or other substance the code SHALL be selected from the ValueSet 2.16.840.1.113883.3.88.12.80.20 Ingredient Name (code system: Unique Ingredient Identifier (UNII) 2.16.840.1.113883.4.9). Example: Peanut, Red 40
				if(allergyDef.MedicationNum==0) {//Unique Ingredient Identifier (UNII codes)
					if(allergyDef.UniiCode=="") {
						StartAndEnd("code","nullFlavor","UNK");
					}
					else {
						StartAndEnd("code","code",allergyDef.UniiCode,"displayName",allergyDef.Description,"codeSystem",strCodeSystemUnii,"codeSystemName",strCodeSystemNameUnii);
					}
				}
				//else if() {//Medication Drug Class (NDF-RT codes) //TODO: We need a UI box for this in allergy def.
				//Current work around is (per js on 10/02/2013):
				//If using eRx, search for a class such as NSAID.
				//If that's not an option, pick a common medication in that class such as Ibuprofen, and the eRx will automatically list cross-sensitivity and allergy warnings for any other related medication.
				//The allergy alerts built into OD do not have that rich database available.
				//If you are not using paper Rx from within OD, then you could enter the allergy however it makes sense, either NSAID or Ibuprofen.
				//If you are using paper Rx and you are trying to generate allergy warnings, then you also enter any specific medications that you might prescribe.
				//For example, you might enter an allergy for Vicoprofen.
				//}
				else {//Medication Brand Name or Medication Clinical Drug (RxNorm codes)
					Medication med=Medications.GetMedication(allergyDef.MedicationNum);
					StartAndEnd("code","code",med.RxCui.ToString(),"displayName",med.MedName,"codeSystem",strCodeSystemRxNorm,"codeSystemName",strCodeSystemNameRxNorm);
				}
				End("playingEntity");
				End("participantRole");
				End("participant");
				Start("entryRelationship","typeCode","SUBJ","inversionInd","true");
				Start("observation","classCode","OBS","moodCode","EVN");
				_w.WriteComment("Allergy Status Observation template");
				TemplateId("2.16.840.1.113883.10.20.22.4.28");
				StartAndEnd("code","code","33999-4","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Status");
				StartAndEnd("statusCode","code","completed");//fixed value (required)
				string status=allergy.StatusIsActive?"Active":"Inactive";
				if(allergy.AllergyNum==0) {
					Start("value");
					_w.WriteAttributeString("xsi","type",null,"CE");
					Attribs("nullFlavor","UNK");
					End("value");
				}
				else if(status=="Active") {
					Start("value");
					_w.WriteAttributeString("xsi","type",null,"CE");
					Attribs("code","55561003","codeSystem",strCodeSystemSnomed,"displayName",status);
					End("value");
				}
				else {
					Start("value");
					_w.WriteAttributeString("xsi","type",null,"CE");
					Attribs("code","73425007","codeSystem",strCodeSystemSnomed,"displayName",status);
					End("value");
				}
				End("observation");
				End("entryRelationship");
				Start("entryRelationship","typeCode","SUBJ","inversionInd","true");
				Start("observation","classCode","OBS","moodCode","EVN");
				_w.WriteComment("Reaction Observation template");
				TemplateId("2.16.840.1.113883.10.20.22.4.9");
				Guid();
				StartAndEnd("code","code","ASSERTION","codeSystem","2.16.840.1.113883.5.4");
				StartAndEnd("statusCode","code","completed");//fixed value (required)
				Start("effectiveTime");
				if(allergy.DateTStamp.Year<1880) {
					StartAndEnd("low","nullFlavor","UNK");
				}
				else if(allergy.StatusIsActive) {
					StartAndEnd("low","value",allergy.DateTStamp.ToString("yyyyMMdd"));
				}
				else {
					StartAndEnd("low","nullFlavor","UNK");
				}
				End("effectiveTime");
				if(String.IsNullOrEmpty(allergy.SnomedReaction)) {
					Start("value");
					_w.WriteAttributeString("xsi","type",null,"CD");
					Attribs("nullFlavor","UNK");
					End("value");
				}
				else {
					Start("value");
					_w.WriteAttributeString("xsi","type",null,"CD");
					Attribs("code",allergy.SnomedReaction,"codeSystem",strCodeSystemSnomed,"displayName",allergy.Reaction);
					End("value");
				}
				End("observation");
				End("entryRelationship");
				End("observation");
				End("entryRelationship");
				End("act");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionEncounters(bool hasEncounter) {
			_w.WriteComment(@"
=====================================================================================================
Encounters
=====================================================================================================");
			List<Encounter> listEncountersFiltered;
			if(!hasEncounter) {
				listEncountersFiltered=new List<Encounter>();
			}
			else {
				listEncountersFiltered=_listEncountersFiltered;
			}
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.22.1");//Encounters section with coded entries required.
			_w.WriteComment("Encounters section template");//(Page 227)
			StartAndEnd("code","code","46240-8","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","History of encounters");
			_w.WriteElementString("title","Encounters");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listEncountersFiltered.Count>0 && hasEncounter) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Performer");
				_w.WriteElementString("th","Observation");
				_w.WriteElementString("th","Date");
				_w.WriteElementString("th","Notes");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listEncountersFiltered.Count;i++) {
					Start("tr");
					if(listEncountersFiltered[i].ProvNum==0) {
						_w.WriteElementString("td","");
					}
					else {
						_w.WriteElementString("td",Providers.GetProv(listEncountersFiltered[i].ProvNum).GetFormalName());
					}
					Snomed snomedDiagnosis=Snomeds.GetByCode(listEncountersFiltered[i].CodeValue);
					if(snomedDiagnosis==null) {//Could be null if the code was imported from another EHR.
						_w.WriteElementString("td","");
					}
					else {
						_w.WriteElementString("td",snomedDiagnosis.SnomedCode+" - "+snomedDiagnosis.Description);
					}
					if(listEncountersFiltered[i].DateEncounter.Year<1880) {
						_w.WriteElementString("td","");
					}
					else {
						DateText("td",listEncountersFiltered[i].DateEncounter);
					}
					_w.WriteElementString("td",listEncountersFiltered[i].Note);
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listEncountersFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				Encounter enc=new Encounter();
				listEncountersFiltered.Add(enc);
			}
			for(int i=0;i<listEncountersFiltered.Count;i++) {
				Start("entry","typeCode","DRIV");
				Start("encounter","classCode","ENC","moodCode","EVN");
				TemplateId("2.16.840.1.113883.10.20.22.4.49");
				_w.WriteComment("Encounter Activity Template");//(Page 358)
				Guid();
				StartAndEnd("code","code","99212","displayName","Outpatient Visit","codeSystemName","CPT-4");//CPT-4 is required. Valid codes are 99201 through 99607.
				if(listEncountersFiltered[i].DateEncounter.Year<1880) {
					StartAndEnd("effectiveTime","nullFlavor","UNK");
				}
				else {
					StartAndEnd("effectiveTime","value",listEncountersFiltered[i].DateEncounter.ToString("yyyyMMdd"));
				}
				Provider prov=Providers.GetProv(listEncountersFiltered[i].ProvNum);
				if(prov!=null && !prov.IsNotPerson) {
					Start("performer");
					Start("assignedEntity");
					Guid();
					_w.WriteComment("Performer Information");
					if(listEncountersFiltered[i].ProvNum==0) {
						StartAndEnd("code","nullFlavor","UNK");
					}
					else {
						prov=Providers.GetProv(listEncountersFiltered[i].ProvNum);
						StartAndEnd("code","code",GetTaxonomy(prov),"codeSystem",strCodeSystemNucc,"codeSystemName",strCodeSystemNameNucc);
					}
					//The assignedPerson element might not be allowed here. If that is the case, then performer is useless, because it would only contain the specialty code. Our HTML output shows the prov name.
					Start("assignedPerson");
					if(listEncountersFiltered[i].ProvNum==0) {
						StartAndEnd("name","nullFlavor","UNK");
					}
					else {
						Start("name");
						if(prov.IsNotPerson) {
							_w.WriteElementString("given","");//Not needed for CCD
						}
						else {
							_w.WriteElementString("given",prov.FName.Trim());
						}
						_w.WriteElementString("family",prov.LName.Trim());
						End("name");
					}
					End("assignedPerson");
					End("assignedEntity");
					End("performer");
				}
				//Possibly add an Instructions Template
				bool isInversion=false;//Specifies that the problem was or was not observed. All problems are "observed" for now.
				Start("entryRelationship","typeCode","SUBJ","inversionInd",isInversion?"true":"false");
				Start("act","classCode","ACT","moodCode","EVN");
				_w.WriteComment("Encounter Diagnosis Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.80");//(Page 362)
				Guid();
				Start("code");
				_w.WriteAttributeString("xsi","type",null,"CE");
				Attribs("code","29308-4","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Encounter Diagnosis");
				End("code");
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				if(listEncountersFiltered[i].DateEncounter.Year<1880) {
					StartAndEnd("low","nullFlavor","UNK");
				}
				else {
					DateElement("low",listEncountersFiltered[i].DateEncounter);
				}
				End("effectiveTime");
				Start("entryRelationship","typeCode","SUBJ","inversionInd",isInversion?"true":"false");
				Start("observation","classCode","OBS","moodCode","EVN","negationInd",isInversion?"true":"false");
				_w.WriteComment("Problem Observation Template");
				TemplateId("2.16.840.1.113883.10.20.22.4.4");//(Page 466)
				Guid();
				StartAndEnd("code","code","409586006","codeSystem",strCodeSystemSnomed,"displayName","Complaint");
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				if(listEncountersFiltered[i].DateEncounter.Year<1880) {
					StartAndEnd("low","nullFlavor","UNK");
				}
				else {
					DateElement("low",listEncountersFiltered[i].DateEncounter);
				}
				//"If the problem is known to be resolved, but the date of resolution is not known, 
				//then the high element SHALL be present, and the nullFlavor attribute SHALL be set to 'UNK'.
				//Therefore, the existence of an high element within a problem does indicate that the problem has been resolved."
				End("effectiveTime");
				Snomed snomedDiagnosis=Snomeds.GetByCode(listEncountersFiltered[i].CodeValue);
				if(snomedDiagnosis==null) {
					Start("value");
					_w.WriteAttributeString("xsi","type",null,"CD");
					Attribs("nullFlavor","UNK");
					End("value");
				}
				else {
					Start("value");
					_w.WriteAttributeString("xsi","type",null,"CD");
					//The format only allows SNOMED and ICD10 code systems. If we support ICD10 in the future, then the value must be specified in a special manner. SNOMED appears to be preferred. See the guide for details.
					//Snomed snomedDiagnosis=Snomeds.GetByCode(listEncountersFiltered[i].CodeValue);
					Attribs("code",snomedDiagnosis.SnomedCode,"codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed,"displayName",snomedDiagnosis.Description);
					End("value");
				}
				End("observation");
				End("entryRelationship");
				End("act");
				End("entryRelationship");
				End("encounter");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionFunctionalStatus(bool hasFunctionalStatus) {
			_w.WriteComment(@"
=====================================================================================================
Functional and Cognitive Status
=====================================================================================================");
			List<Disease> listProblemsFiltered;
			if(!hasFunctionalStatus) {
				listProblemsFiltered=new List<Disease>();
			}
			else {
				listProblemsFiltered=_listProblemsFuncFiltered;
			}
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.14");//Functional Status section. There is only one allowed template id.
			_w.WriteComment("Functional Status section template");//(Page 232)
			StartAndEnd("code","code","47420-5","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Functional Status");
			_w.WriteElementString("title","Functional Status");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listProblemsFiltered.Count>0 && hasFunctionalStatus) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Condition");
				_w.WriteElementString("th","Effective Dates");
				_w.WriteElementString("th","Condition Status");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listProblemsFiltered.Count;i++) {
					DiseaseDef diseaseDef=null;
					Snomed snomedProblem=null;
					if(listProblemsFiltered[i].DiseaseDefNum>0) {
						diseaseDef=DiseaseDefs.GetItem(listProblemsFiltered[i].DiseaseDefNum);
						if(diseaseDef!=null && !String.IsNullOrEmpty(diseaseDef.SnomedCode)) {
							snomedProblem=Snomeds.GetByCode(diseaseDef.SnomedCode);
						}
					}
					Start("tr");
					if(diseaseDef==null || snomedProblem==null) {
						_w.WriteElementString("td","");
					}
					else {
						_w.WriteElementString("td",snomedProblem.SnomedCode+" - "+snomedProblem.Description);
					}
					if(listProblemsFiltered[i].FunctionStatus==FunctionalStatus.FunctionalResult || listProblemsFiltered[i].FunctionStatus==FunctionalStatus.CognitiveResult) {
						DateText("td",listProblemsFiltered[i].DateStart);
					}
					else {//functional problem and cognitive problem
						if(listProblemsFiltered[i].DateStop.Year>1880) {
							_w.WriteElementString("td",listProblemsFiltered[i].DateStart.ToString("yyyyMMdd")+" to "+listProblemsFiltered[i].DateStop.ToString("yyyyMMdd"));
						}
						else {
							DateText("td",listProblemsFiltered[i].DateStart);
						}
					}
					_w.WriteElementString("td","Completed");
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listProblemsFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				Disease dis=new Disease();
				dis.FunctionStatus=FunctionalStatus.FunctionalProblem;//Just needs a version other than problem.
				listProblemsFiltered.Add(dis);
			}
			for(int i=0;i<listProblemsFiltered.Count;i++) {
				DiseaseDef diseaseDef=null;
				Snomed snomedProblem=null;
				if(listProblemsFiltered[i].PatNum!=0) {
					diseaseDef=DiseaseDefs.GetItem(listProblemsFiltered[i].DiseaseDefNum);
					snomedProblem=Snomeds.GetByCode(diseaseDef.SnomedCode);
				}
				if(diseaseDef==null) {
					diseaseDef=new DiseaseDef();
				}
				if(snomedProblem==null) {
					snomedProblem=new Snomed();
				}
				Start("entry","typeCode","DRIV");
				Start("observation","classCode","OBS","moodCode","EVN");
				if(listProblemsFiltered[i].FunctionStatus==FunctionalStatus.FunctionalResult) {
					TemplateId("2.16.840.1.113883.10.20.22.4.67");//(Page 383)
					_w.WriteComment("Functional Status Result Observation");
					Guid();
					StartAndEnd("code","code","54744-8","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					StartAndEnd("statusCode","code","completed");
					if(listProblemsFiltered[i].DateStart.Year<1880) {
						StartAndEnd("effectiveTime","nullFlavor","UNK");
					}
					else {
						DateElement("effectiveTime",listProblemsFiltered[i].DateStart);
					}
					if(String.IsNullOrEmpty(snomedProblem.SnomedCode)) {
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"CD");
						Attribs("nullFlavor","UNK");
						End("value");
					}
					else {
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"CD");
						Attribs("code",snomedProblem.SnomedCode,"displayName",snomedProblem.Description,"codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
						End("value");
					}
				}
				else if(listProblemsFiltered[i].FunctionStatus==FunctionalStatus.CognitiveResult) {
					TemplateId("2.16.840.1.113883.10.20.22.4.74");//(Page 342)
					_w.WriteComment("Cognitive Status Result Observation");
					Guid();
					StartAndEnd("code","code","5249-2","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
					StartAndEnd("statusCode","code","completed");
					if(listProblemsFiltered[i].DateStart.Year<1880) {
						StartAndEnd("effectiveTime","nullFlavor","UNK");
					}
					else {
						DateElement("effectiveTime",listProblemsFiltered[i].DateStart);
					}
					if(String.IsNullOrEmpty(snomedProblem.SnomedCode)) {
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"CD");
						Attribs("nullFlavor","UNK");
						End("value");
					}
					else {
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"CD");
						Attribs("code",snomedProblem.SnomedCode,"displayName",snomedProblem.Description,"codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
						End("value");
					}
				}
				else if(listProblemsFiltered[i].FunctionStatus==FunctionalStatus.FunctionalProblem) {
					TemplateId("2.16.840.1.113883.10.20.22.4.68");//(Page 379)
					_w.WriteComment("Functional Status Problem Observation");
					Guid();
					StartAndEnd("code","code","404684003","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed,"displayName","Finding of Functional Performance and Activity");
					StartAndEnd("statusCode","code","completed");
					Start("effectiveTime");
					if(listProblemsFiltered[i].DateStart.Year<1880) {
						StartAndEnd("low","nullFlavor","UNK");
					}
					else {
						DateElement("low",listProblemsFiltered[i].DateStart);
					}
					//"If the problem is known to be resolved, but the date of resolution is not known, then the high element SHALL be present, and 
					//the nullFlavor attribute SHALL be set to 'UNK'. Therefore, the existence of an high element within a problem does indicate that the problem has been resolved."
					if(listProblemsFiltered[i].DateStop.Year<1880) {
						StartAndEnd("high","nullFlavor","UNK");
					}
					else {
						DateElement("high",listProblemsFiltered[i].DateStop);
					}
					End("effectiveTime");
					if(String.IsNullOrEmpty(snomedProblem.SnomedCode)) {
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"CD");
						Attribs("nullFlavor","UNK");
						End("value");
					}
					else {
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"CD");
						Attribs("code",snomedProblem.SnomedCode,"displayName",snomedProblem.Description,"codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
						End("value");
					}
				}
				else if(listProblemsFiltered[i].FunctionStatus==FunctionalStatus.CognitiveProblem) {
					TemplateId("2.16.840.1.113883.10.20.22.4.73");//(Page 336)
					_w.WriteComment("Cognitive Status Problem Observation");
					Guid();
					StartAndEnd("code","code","373930000","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed,"displayName","Cognitive Function Finding");
					StartAndEnd("statusCode","code","completed");
					//"If the problem is known to be resolved, but the date of resolution is not known, then the high element SHALL be present, and 
					//the nullFlavor attribute SHALL be set to 'UNK'. Therefore, the existence of a high element within a problem does indicate that the problem has been resolved."
					Start("effectiveTime");
					if(listProblemsFiltered[i].DateStart.Year<1880) {
						StartAndEnd("low","nullFlavor","UNK");
					}
					else {
						DateElement("low",listProblemsFiltered[i].DateStart);
					}
					if(listProblemsFiltered[i].DateStop.Year<1880) {
						StartAndEnd("high","nullFlavor","UNK");
					}
					else {
						DateElement("high",listProblemsFiltered[i].DateStop);
					}
					End("effectiveTime");
					if(String.IsNullOrEmpty(snomedProblem.SnomedCode)) {
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"CD");
						Attribs("nullFlavor","UNK");
						End("value");
					}
					else {
						Start("value");
						_w.WriteAttributeString("xsi","type",null,"CD");
						Attribs("code",snomedProblem.SnomedCode,"displayName",snomedProblem.Description,"codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
						End("value");
					}
				}
				End("observation");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionImmunizations(bool hasImmunization) {
			_w.WriteComment(@"
=====================================================================================================
Immunizations
=====================================================================================================");
			List<VaccinePat> listVaccinePatsFiltered;
			if(!hasImmunization) {
				listVaccinePatsFiltered=new List<VaccinePat>();
			}
			else {
				listVaccinePatsFiltered=_listVaccinePatsFiltered;
			}
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.2.1");//Immunizations section with coded entries required.
			_w.WriteComment("Immunizations section template");
			StartAndEnd("code","code","11369-6","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","History of immunizations");
			_w.WriteElementString("title","Immunizations");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listVaccinePatsFiltered.Count>0 && hasImmunization) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Vaccine");
				_w.WriteElementString("th","Date");
				_w.WriteElementString("th","Status");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listVaccinePatsFiltered.Count;i++) {
					VaccineDef vaccineDef;
					if(listVaccinePatsFiltered[i].VaccineDefNum==0) {
						vaccineDef=new VaccineDef();
					}
					else {
						vaccineDef=VaccineDefs.GetOne(listVaccinePatsFiltered[i].VaccineDefNum);
					}
					Start("tr");
					Cvx cvx;
					if(String.IsNullOrEmpty(vaccineDef.CVXCode)) {
						cvx=new Cvx();
					}
					else {
						cvx=Cvxs.GetOneFromDb(vaccineDef.CVXCode);
					}
					if(String.IsNullOrEmpty(cvx.CvxCode)) {
						_w.WriteElementString("td","");
					}
					else {
						_w.WriteElementString("td",cvx.CvxCode+" - "+cvx.Description);
					}
					if(listVaccinePatsFiltered[i].DateTimeStart.Year<1880) {
						_w.WriteElementString("td","");
					}
					else {
						DateText("td",listVaccinePatsFiltered[i].DateTimeStart);
					}
					_w.WriteElementString("td","Completed");
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listVaccinePatsFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				VaccinePat vacPat=new VaccinePat();
				listVaccinePatsFiltered.Add(vacPat);
			}
			for(int i=0;i<listVaccinePatsFiltered.Count;i++) {
				VaccineDef vaccineDef;
				if(listVaccinePatsFiltered[i].VaccinePatNum==0) {
					vaccineDef=new VaccineDef();
				}
				else {
					vaccineDef=VaccineDefs.GetOne(listVaccinePatsFiltered[i].VaccineDefNum);
				}
				Start("entry","typeCode","DRIV");
				Start("substanceAdministration","classCode","SBADM","moodCode","EVN","negationInd",(listVaccinePatsFiltered[i].CompletionStatus==VaccineCompletionStatus.NotAdministered)?"true":"false");
				TemplateId("2.16.840.1.113883.10.20.22.4.52");
				_w.WriteComment("Immunization Activity Template");
				Guid();
				StartAndEnd("statusCode","code","completed");
				Start("effectiveTime");
				_w.WriteAttributeString("xsi","type",null,"IVL_TS");
				if(listVaccinePatsFiltered[i].DateTimeStart.Year<1880) {
					Attribs("nullFlavor","UNK");
				}
				else {
					Attribs("value",listVaccinePatsFiltered[i].DateTimeStart.ToString("yyyyMMdd"));
				}
				End("effectiveTime");
				Start("consumable");
				Start("manufacturedProduct","classCode","MANU");
				TemplateId("2.16.840.1.113883.10.20.22.4.54");
				_w.WriteComment("Immunization Medication Information");
				Start("manufacturedMaterial");
				if(String.IsNullOrEmpty(vaccineDef.CVXCode)) {
					StartAndEnd("code","nullFlavor","UNK");
				}
				else {
					Cvx cvx=Cvxs.GetOneFromDb(vaccineDef.CVXCode);
					StartAndEnd("code","code",cvx.CvxCode,"codeSystem",strCodeSystemCvx,"displayName",cvx.Description,"codeSystemName",strCodeSystemNameCvx);
				}
				End("manufacturedMaterial");
				End("manufacturedProduct");
				End("consumable");
				//Possibly add an Instructions Template
				End("substanceAdministration");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionInstructions(string instructions) {
			if(instructions==null) {
				return;
			}
			_w.WriteComment(@"
=====================================================================================================
Instructions
=====================================================================================================");
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.45");//Instructions template
			_w.WriteComment("Instructions section template");
			StartAndEnd("code","code","69730-0","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","History of immunizations");
			_w.WriteElementString("title","Instructions");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.\
			if(instructions=="") {
				_w.WriteString("No instructions given");
			}
			else {
				_w.WriteString(instructions);
			}
			End("text");
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionMedications(bool hasMedication) {
			_w.WriteComment(@"
=====================================================================================================
Medications
=====================================================================================================");
			List<MedicationPat> listMedPatsFiltered=new List<MedicationPat>();
			if(!hasMedication) {
				listMedPatsFiltered=new List<MedicationPat>();
			}
			else {
				listMedPatsFiltered=_listMedPatsFiltered;
			}
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.1.1");//Medication section with coded entries required.
			_w.WriteComment("Medications section template");
			StartAndEnd("code","code","10160-0","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","History of medication use");
			_w.WriteElementString("title","Medications");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listMedPatsFiltered.Count>0 && hasMedication) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Medication");
				_w.WriteElementString("th","Directions");
				_w.WriteElementString("th","Start Date");
				_w.WriteElementString("th","End Date");
				_w.WriteElementString("th","Status");
				_w.WriteElementString("th","Indications");
				_w.WriteElementString("th","Fill Instructions");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listMedPatsFiltered.Count;i++) {
					string strMedName=listMedPatsFiltered[i].MedDescript;
					if(listMedPatsFiltered[i].MedicationNum!=0) {
						strMedName=Medications.GetNameOnly(listMedPatsFiltered[i].MedicationNum);
					}
					Start("tr");
					if(listMedPatsFiltered[i].RxCui==0) {
						_w.WriteElementString("td","");
					}
					else {
						_w.WriteElementString("td",listMedPatsFiltered[i].RxCui+" - "+strMedName);//Medication
					}
					_w.WriteElementString("td",listMedPatsFiltered[i].PatNote);//Directions
					if(listMedPatsFiltered[i].DateStart.Year<1880) {
						_w.WriteElementString("td","");//Directions
					}
					else {
						DateText("td",listMedPatsFiltered[i].DateStart);//Start Date
					}
					if(listMedPatsFiltered[i].DateStop.Year<1880) {
						_w.WriteElementString("td","");//Directions
					}
					else {
						DateText("td",listMedPatsFiltered[i].DateStop);//End Date
					}
					_w.WriteElementString("td",MedicationPats.IsMedActive(listMedPatsFiltered[i])?"Active":"Inactive");//Status
					_w.WriteElementString("td","");//Indications (The conditions which make the medication necessary). We do not record this information anywhere.
					_w.WriteElementString("td","");//Fill Instructions (Generic substitution allowed or not). We do not record this information anywhere.
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listMedPatsFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				MedicationPat medPat=new MedicationPat();
				listMedPatsFiltered.Add(medPat);
			}
			for(int i=0;i<listMedPatsFiltered.Count;i++) {
				string strMedName="";
				if(listMedPatsFiltered[i].MedDescript!=null) {
					strMedName=listMedPatsFiltered[i].MedDescript;//This might be blank, for example not from NewCrop.
				}
				if(listMedPatsFiltered[i].MedicationNum!=0) {//If NewCrop, this will be 0.  Also might be zero in the future when we start allowing freeform medications.
					strMedName=Medications.GetNameOnly(listMedPatsFiltered[i].MedicationNum);
				}
				Start("entry","typeCode","DRIV");
				Start("substanceAdministration","classCode","SBADM","moodCode","EVN");
				TemplateId("2.16.840.1.113883.10.20.22.4.16");
				_w.WriteComment("Medication activity template");
				Guid();
				if(String.IsNullOrEmpty(listMedPatsFiltered[i].PatNote)) {
					StartAndEnd("text","nullFlavor","UNK");
				}
				else {
					_w.WriteElementString("text",listMedPatsFiltered[i].PatNote);
				}
				StartAndEnd("statusCode","code","active");
				Start("effectiveTime");
				_w.WriteAttributeString("xsi","type",null,"IVL_TS");
				if(listMedPatsFiltered[i].DateStart.Year<1880) {
					StartAndEnd("low","nullFlavor","UNK");
				}
				else {
					DateElement("low",listMedPatsFiltered[i].DateStart);//Only one of these dates can be null, because of our filter above.
				}
				if(listMedPatsFiltered[i].DateStop.Year<1880) {
					StartAndEnd("high","nullFlavor","UNK");
				}
				else {
					DateElement("high",listMedPatsFiltered[i].DateStop);//Only one of these dates can be null, because of our filter above.
				}
				End("effectiveTime");
				Start("consumable");
				Start("manufacturedProduct","classCode","MANU");
				TemplateId("2.16.840.1.113883.10.20.22.4.23");//Medication Information template.
				Guid();
				Start("manufacturedMaterial");
				//The code must be an RxNorm.
				if(listMedPatsFiltered[i].RxCui==0) {
					Start("code","nullFlavor","UNK");
				}
				else {
					Start("code","code",listMedPatsFiltered[i].RxCui.ToString(),"codeSystem",strCodeSystemRxNorm,"displayName",strMedName,"codeSystemName",strCodeSystemNameRxNorm);
				}
				End("code");
				End("manufacturedMaterial");
				End("manufacturedProduct");
				End("consumable");
				End("substanceAdministration");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionPlanOfCare(bool hasPlanOfCare) {
			_w.WriteComment(@"
=====================================================================================================
Care Plan
=====================================================================================================");
			List<EhrCarePlan> listEhrCarePlansAll=EhrCarePlans.Refresh(_patOutCcd.PatNum);
			List<EhrCarePlan> listEhrCarePlansFiltered;
			if(!hasPlanOfCare) {
				listEhrCarePlansFiltered=new List<EhrCarePlan>();
			}
			else {
				listEhrCarePlansFiltered=_listEhrCarePlansFiltered;
			}
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.10");//Only one template id allowed (unlike other sections).
			_w.WriteComment("Plan of Care section template");
			StartAndEnd("code","code","18776-5","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Treatment plan");
			_w.WriteElementString("title","Care Plan");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listEhrCarePlansFiltered.Count>0 && hasPlanOfCare) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Planned Activity");
				_w.WriteElementString("th","Planned Date");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listEhrCarePlansFiltered.Count;i++) {
					Start("tr");
					Snomed snomedEducation;
					snomedEducation=Snomeds.GetByCode(listEhrCarePlansFiltered[i].SnomedEducation);
					if(snomedEducation==null) {
						snomedEducation=new Snomed();
					}
					if(String.IsNullOrEmpty(snomedEducation.Description)) {
						if(String.IsNullOrEmpty(listEhrCarePlansFiltered[i].Instructions)) {
							_w.WriteElementString("td","");//Planned Activity
						}
						else {
							_w.WriteElementString("td","Goal: ; Instructions: "+listEhrCarePlansFiltered[i].Instructions);//Planned Activity
						}
					}
					else {
						_w.WriteElementString("td","Goal: "+snomedEducation.SnomedCode+" - "+snomedEducation.Description+"; Instructions: "+listEhrCarePlansFiltered[i].Instructions);//Planned Activity
					}
					if(listEhrCarePlansFiltered[i].DatePlanned.Year<1880) {
						_w.WriteElementString("td","");
					}
					else {
						DateText("td",listEhrCarePlansFiltered[i].DatePlanned);//Planned Date
					}
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listEhrCarePlansFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				EhrCarePlan eCP=new EhrCarePlan();
				listEhrCarePlansFiltered.Add(eCP);
			}
			for(int i=0;i<listEhrCarePlansFiltered.Count;i++) {
				Start("entry","typeCode","DRIV");
				Start("act","classCode","ACT","moodCode","INT");
				TemplateId("2.16.840.1.113883.10.20.22.4.20");
				_w.WriteComment("Instructions template");
				Start("code");
				_w.WriteAttributeString("xsi","type",null,"CE");
				Snomed snomedEducation=Snomeds.GetByCode(listEhrCarePlansFiltered[i].SnomedEducation);
				if(snomedEducation==null) {
					Attribs("nullFlavor","UNK");
				}
				else {
					Attribs("code",snomedEducation.SnomedCode,"codeSystem",strCodeSystemSnomed,"displayName",snomedEducation.Description);
				}
				End("code");
				if(listEhrCarePlansFiltered[i].Instructions=="") {
					StartAndEnd("text","nullFlavor","UNK");
				}
				else {
					_w.WriteElementString("text",listEhrCarePlansFiltered[i].Instructions);
				}
				StartAndEnd("statusCode","code","completed");
				End("act");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().  Problem section.</summary>
		private void GenerateCcdSectionProblems(bool hasProblem) {
			_w.WriteComment(@"
=====================================================================================================
Problems
=====================================================================================================");
			string snomedProblemType="55607006";
			List<Disease> listProblemsFiltered;
			if(!hasProblem) {
				listProblemsFiltered=new List<Disease>();
			}
			else {
				listProblemsFiltered=_listProblemsFiltered;
			}
			string status="Inactive";
			string statusCode="73425007";
			string statusOther="active";
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.5.1");//Problems section with coded entries required.
			_w.WriteComment("Problems section template");
			StartAndEnd("code","code","11450-4","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Problem list");
			_w.WriteElementString("title","Problems");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listProblemsFiltered.Count>0 && hasProblem) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Problem");
				_w.WriteElementString("th","Date Start");
				_w.WriteElementString("th","Date End");
				_w.WriteElementString("th","Status");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listProblemsFiltered.Count;i++) {
					DiseaseDef diseaseDef;
					if(listProblemsFiltered[i].DiseaseDefNum==0) {
						diseaseDef=new DiseaseDef();
					}
					else {
						diseaseDef=DiseaseDefs.GetItem(listProblemsFiltered[i].DiseaseDefNum);
					}
					Start("tr");
					if(String.IsNullOrEmpty(diseaseDef.SnomedCode)) {
						_w.WriteElementString("td","");
					}
					else {
						_w.WriteElementString("td",diseaseDef.SnomedCode+" - "+diseaseDef.DiseaseName);
					}
					if(listProblemsFiltered[i].DateStart.Year<1880) {
						_w.WriteElementString("td","");//Directions
					}
					else {
						DateText("td",listProblemsFiltered[i].DateStart);//Start Date
					}
					if(listProblemsFiltered[i].DateStop.Year<1880) {
						_w.WriteElementString("td","");//Directions
					}
					else {
						DateText("td",listProblemsFiltered[i].DateStop);//End Date
					}
					if(listProblemsFiltered[i].ProbStatus==ProblemStatus.Active) {
						status="Active";
						statusCode="55561003";
						statusOther="active";
					}
					else if(listProblemsFiltered[i].ProbStatus==ProblemStatus.Inactive) {
						status="Inactive";
						statusCode="73425007";
						statusOther="completed";
					}
					else {
						status="Resolved";
						statusCode="413322009";
						statusOther="completed";
					}
					_w.WriteElementString("td",status);
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			//Start("text");
			//StartAndEnd("content","ID","problems");
			//Start("list","listType","ordered");
			//for(int i=0;i<listProblemsFiltered.Count;i++) {//Fill Problems Table
			//	DiseaseDef diseaseDef=DiseaseDefs.GetItem(listProblemsFiltered[i].DiseaseDefNum);
			//	Start("item");
			//	_w.WriteString(diseaseDef.SnomedCode+" - "+diseaseDef.DiseaseName+" : "+"Status - ");
			//	if(listProblemsFiltered[i].ProbStatus==ProblemStatus.Active) {
			//		_w.WriteString("Active");
			//		status="Active";
			//		statusCode="55561003";
			//		statusOther="active";
			//	}
			//	else if(listProblemsFiltered[i].ProbStatus==ProblemStatus.Inactive) {
			//		_w.WriteString("Inactive");
			//		status="Inactive";
			//		statusCode="73425007";
			//		statusOther="completed";
			//	}
			//	else {
			//		_w.WriteString("Resolved");
			//		status="Resolved";
			//		statusCode="413322009";
			//		statusOther="completed";
			//	}
			//	End("item");
			//}
			//End("list");
			//End("text");
			if(listProblemsFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				Disease dis=new Disease();
				listProblemsFiltered.Add(dis);
			}
			for(int i=0;i<listProblemsFiltered.Count;i++) {//Fill Problems Info
				DiseaseDef diseaseDef;
				if(listProblemsFiltered[i].DiseaseDefNum==0) {
					diseaseDef=new DiseaseDef();
				}
				else {
					diseaseDef=DiseaseDefs.GetItem(listProblemsFiltered[i].DiseaseDefNum);
				}
				Start("entry","typeCode","DRIV");
				Start("act","classCode","ACT","moodCode","EVN");
				_w.WriteComment("Problem Concern Act template");//Concern Act Section
				TemplateId("2.16.840.1.113883.10.20.22.4.3");
				Guid();
				StartAndEnd("code","code","CONC","codeSystem","2.16.840.1.113883.5.6","displayName","Concern");
				StartAndEnd("statusCode","code",statusOther);//Allowed values: active, suspended, aborted, completed.
				Start("effectiveTime");
				if(listProblemsFiltered[i].DateStart.Year<1880) {
					StartAndEnd("low","nullFlavor","UNK");
				}
				else {
					DateElement("low",listProblemsFiltered[i].DateStart);
				}
				if(listProblemsFiltered[i].DateStop.Year<1880) {
					StartAndEnd("high","nullFlavor","UNK");
				}
				else {
					DateElement("high",listProblemsFiltered[i].DateStop);
				}
				End("effectiveTime");
				Start("entryRelationship","typeCode","SUBJ");
				Start("observation","classCode","OBS","moodCode","EVN");
				_w.WriteComment("Problem Observation template");//Observation Section
				TemplateId("2.16.840.1.113883.10.20.22.4.4");
				Guid();
				StartAndEnd("code","code",snomedProblemType,"codeSystem",strCodeSystemSnomed,"displayName","Problem");
				StartAndEnd("statusCode","code","completed");//Allowed values: completed.
				Start("effectiveTime");
				if(listProblemsFiltered[i].DateStart.Year<1880) {
					StartAndEnd("low","nullFlavor","UNK");
				}
				else {
					DateElement("low",listProblemsFiltered[i].DateStart);
				}
				End("effectiveTime");
				Start("value");
				_w.WriteAttributeString("xsi","type",null,"CD");
				if(String.IsNullOrEmpty(diseaseDef.SnomedCode)) {
					Attribs("nullFlavor","UNK");
				}
				else {
					Attribs("code",diseaseDef.SnomedCode,"codeSystem",strCodeSystemSnomed,"displayName",diseaseDef.DiseaseName);
				}
				End("value");
				Start("entryRelationship","typeCode","REFR");
				Start("observation","classCode","OBS","moodCode","EVN");
				_w.WriteComment("Status Observation template");//Status Observation Section
				TemplateId("2.16.840.1.113883.10.20.22.4.6");
				Start("code");
				_w.WriteAttributeString("xsi","type",null,"CE");
				Attribs("code","33999-4","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Status");
				End("code");
				StartAndEnd("statusCode","code","completed");//Allowed values: completed.
				Start("value");
				_w.WriteAttributeString("xsi","type",null,"CD");
				Attribs("code",statusCode,"codeSystem",strCodeSystemSnomed,"displayName",status);
				End("value");
				End("observation");
				End("entryRelationship");
				End("observation");
				End("entryRelationship");
				End("act");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionProcedures(bool hasProcedure) {
			_w.WriteComment(@"
=====================================================================================================
Procedures
=====================================================================================================");
			List<Procedure> listProcsFiltered;
			if(!hasProcedure) {
				listProcsFiltered=new List<Procedure>();
			}
			else {
				listProcsFiltered=_listProcsFiltered;
			}
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.7.1");//Procedures section with coded entries required (Page 285).
			_w.WriteComment("Procedures section template");
			StartAndEnd("code","code","47519-4","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","History of procedures");
			_w.WriteElementString("title","Procedures");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listProcsFiltered.Count>0 && hasProcedure) {
				bool hasBodySite=false;
				for(int i=0;i<listProcsFiltered.Count;i++) {
					Snomed snomedBodySite=Snomeds.GetByCode(listProcsFiltered[i].SnomedBodySite);//snomedBodySite can be null if procCode.SnomedBodySite is blank or invalid.
					if(snomedBodySite!=null) {
						hasBodySite=true;
						break;
					}
				}
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Procedure");
				if(hasBodySite) {
					_w.WriteElementString("th","Body Site");
				}
				_w.WriteElementString("th","Date");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listProcsFiltered.Count;i++) {
					ProcedureCode procCode=ProcedureCodes.GetProcCode(listProcsFiltered[i].CodeNum);
					Start("tr");
					if(Regex.IsMatch(procCode.ProcCode,"^D[0-9]{4}$")) {//CDT code (ADA code)
						_w.WriteElementString("td",procCode.ProcCode+" - "+procCode.Descript);
					}
					else if(Regex.IsMatch(procCode.ProcCode,"^[0-9]{5}$")) {//CPT-4 code (medical code)
						_w.WriteElementString("td",procCode.ProcCode+" - "+procCode.Descript);
					}
					else if(Snomeds.CodeExists(procCode.ProcCode)) {//The SNOMED CT code system contains numerical codes which are between 6 and 18 digits in length as far as we know. Should not overlap CPT codes.
						Snomed snomed=Snomeds.GetByCode(procCode.ProcCode);
						_w.WriteElementString("td",snomed.SnomedCode+" - "+snomed.Description);
					}
					else {//Unknown code.  Output a "blank" procedure row as required by CCD standard.
						_w.WriteElementString("td","");
					}
					if(hasBodySite) {
						Snomed snomedBodySite=Snomeds.GetByCode(listProcsFiltered[i].SnomedBodySite);//snomedBodySite can be null if procCode.SnomedBodySite is blank or invalid.
						if(snomedBodySite==null) {
							_w.WriteElementString("td","");
						}
						else {
							_w.WriteElementString("td",snomedBodySite.SnomedCode+" - "+snomedBodySite.Description);
						}
					}
					if(listProcsFiltered[i].ProcDate.Year<1880) {
						_w.WriteElementString("td","");
					}
					else {
						DateText("td",listProcsFiltered[i].ProcDate);
					}
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listProcsFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				Procedure proc=new Procedure();
				listProcsFiltered.Add(proc);
			}
			for(int i=0;i<listProcsFiltered.Count;i++) {
				ProcedureCode procCode=ProcedureCodes.GetProcCode(listProcsFiltered[i].CodeNum);
				Start("entry","typeCode","DRIV");
				Start("procedure","classCode","PROC","moodCode","EVN");
				TemplateId("2.16.840.1.113883.10.20.22.4.14");//Procedure Activity Section (Page 487).
				_w.WriteComment("Procedure Activity Template");
				Guid();
				//"This code in a procedure activity SHOULD be selected from LOINC (codeSystem 2.16.840.1.113883.6.1) or SNOMED CT (CodeSystem: 2.16.840.1.113883.6.96),
				//and MAY be selected from CPT-4 (CodeSystem: 2.16.840.1.113883.6.12), ICD9 Procedures (CodeSystem: 2.16.840.1.113883.6.104),
				//ICD10 Procedure Coding System (CodeSystem: 2.16.840.1.113883.6.4) (CONF:7657)."
				//In November of 2013, ONC addopted CDT into EHR certification. http://ehrintelligence.com/2013/11/05/ehr-adoption-may-be-easier-for-dentists-with-new-onc-ruling/
				//The CDT OID is 2.16.840.1.113883.6.13 and the code system description is cdt-ADAcodes.
				if(procCode.ProcCode==null) {//Happens when listProcsFiltered[i].CodeNum is invalid or 0 (we create a procedure with CodeNum=0 above this loop if the procedure list is empty).
					StartAndEnd("code","nullFlavor","UNK");//Unknown code.  Output a "blank" procedure row as required by CCD standard.
				}
				else if(Regex.IsMatch(procCode.ProcCode,"^D[0-9]{4}$")) {//CDT code (ADA code)
					StartAndEnd("code","code",procCode.ProcCode,"codeSystem",strCodeSystemCdt,"displayName",procCode.Descript,"codeSystemName",strCodeSystemNameCdt);
				}
				else if(Regex.IsMatch(procCode.ProcCode,"^[0-9]{5}$")) {//CPT-4 code (medical code)
					StartAndEnd("code","code",procCode.ProcCode,"codeSystem",strCodeSystemCpt4,"displayName",procCode.Descript,"codeSystemName",strCodeSystemNameCpt4);
				}
				else if(Snomeds.CodeExists(procCode.ProcCode)) {//The SNOMED CT code system contains numerical codes which are between 6 and 18 digits in length as far as we know. Should not overlap CPT codes.
					Snomed snomed=Snomeds.GetByCode(procCode.ProcCode);
					StartAndEnd("code","code",snomed.SnomedCode,"codeSystem",strCodeSystemSnomed,"displayName",snomed.Description,"codeSystemName",strCodeSystemNameSnomed);
				}
				else {//Unknown code.  Output a "blank" procedure row as required by CCD standard.
					StartAndEnd("code","nullFlavor","UNK");
				}
				StartAndEnd("statusCode","code","completed");//Allowed values: completed, active, aborted, cancelled.
				if(listProcsFiltered[i].ProcDate.Year<1880) {
					StartAndEnd("effectiveTime","nullFlavor","UNK");
				}
				else {
					DateElement("effectiveTime",listProcsFiltered[i].ProcDate);
				}
				End("procedure");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionReasonForReferral(bool hasReferral,string referralReason) {
			_w.WriteComment(@"
=====================================================================================================
Reason for Referral
=====================================================================================================");
			Start("component");
			Start("section");
			TemplateId("1.3.6.1.4.1.19376.1.5.3.1.3.1");
			_w.WriteComment("Reason for Referral Section Template");
			StartAndEnd("code","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"code","42349-1","displayName","Reason for Referral");
			_w.WriteElementString("title","Reason for Referral");
			Start("text");
			if(!hasReferral) {
				_w.WriteElementString("paragraph","None");
			}
			else {
				_w.WriteElementString("paragraph",referralReason);
			}
			End("text");
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().  Exports Labs.</summary>
		private void GenerateCcdSectionResults(bool hasSectionResult) {
			_w.WriteComment(@"
=====================================================================================================
Laboratory Test Results
=====================================================================================================");
			List<EhrLabResult> listLabResultFiltered;
			if(!hasSectionResult) {
				listLabResultFiltered=new List<EhrLabResult>();
			}
			else {
				listLabResultFiltered=_listLabResultFiltered;
			}
			EhrLab labPanel;
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.3.1");//page 309 Results section with coded entries required.
			_w.WriteComment("Diagnostic Results section template");
			StartAndEnd("code","code","30954-2","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Results");
			_w.WriteElementString("title","Diagnostic Results");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listLabResultFiltered.Count>0 && hasSectionResult) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","LOINC Code");
				_w.WriteElementString("th","Test");
				_w.WriteElementString("th","Result");
				_w.WriteElementString("th","Abnormal Flag");
				_w.WriteElementString("th","Date Performed");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listLabResultFiltered.Count;i++) {
					Loinc labLoinc=Loincs.GetByCode(listLabResultFiltered[i].ObservationIdentifierID);
					string value="";
					switch(listLabResultFiltered[i].ValueType) {
						case EhrLaboratories.HL70125.CE:
							break;
						case EhrLaboratories.HL70125.CWE:
							break;
						case EhrLaboratories.HL70125.DT:
							break;
						case EhrLaboratories.HL70125.FT:
							break;
						case EhrLaboratories.HL70125.NM:
							value=listLabResultFiltered[i].ObservationValueNumeric.ToString();
							break;
						case EhrLaboratories.HL70125.SN:
							break;
						case EhrLaboratories.HL70125.ST:
							break;
						case EhrLaboratories.HL70125.TM:
							break;
						case EhrLaboratories.HL70125.TS:
							break;
						case EhrLaboratories.HL70125.TX:
							break;
					}
					Start("tr");
					_w.WriteElementString("td",listLabResultFiltered[i].ObservationIdentifierID);//LOINC Code
					if(labLoinc==null) {
						_w.WriteElementString("td",listLabResultFiltered[i].ObservationIdentifierText);//Test
					}
					else {
						_w.WriteElementString("td",labLoinc.NameShort);//Test
					}
					_w.WriteElementString("td",value+" "+listLabResultFiltered[i].UnitsID);//Result
					_w.WriteElementString("td",listLabResultFiltered[i].AbnormalFlags);//Abnormal Flag
					if(String.Compare(Regex.Match("input string",@"^\d{0,4}").Value.PadLeft(4,'0'),"1880")!=-1) {
						_w.WriteElementString("td","");//Test
					}
					else {
						_w.WriteElementString("td",listLabResultFiltered[i].ObservationDateTime);
					}
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listLabResultFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				EhrLabResult labR=new EhrLabResult();
				listLabResultFiltered.Add(labR);
			}
			for(int i=0;i<listLabResultFiltered.Count;i++) {
				if(listLabResultFiltered[i].EhrLabNum==0) {
					labPanel=new EhrLab();
				}
				else {
					labPanel=EhrLabs.GetOne(listLabResultFiltered[i].EhrLabNum);
				}
				Loinc labLoinc=Loincs.GetByCode(listLabResultFiltered[i].ObservationIdentifierID);
				string value="";
				switch(listLabResultFiltered[i].ValueType) {
					case EhrLaboratories.HL70125.CE:
						break;
					case EhrLaboratories.HL70125.CWE:
						break;
					case EhrLaboratories.HL70125.DT:
						break;
					case EhrLaboratories.HL70125.FT:
						break;
					case EhrLaboratories.HL70125.NM:
						value=listLabResultFiltered[i].ObservationValueNumeric.ToString();
						break;
					case EhrLaboratories.HL70125.SN:
						break;
					case EhrLaboratories.HL70125.ST:
						break;
					case EhrLaboratories.HL70125.TM:
						break;
					case EhrLaboratories.HL70125.TS:
						break;
					case EhrLaboratories.HL70125.TX:
						break;
				}
				Start("entry","typeCode","DRIV");
				Start("organizer","classCode","BATTERY","moodCode","EVN");
				StartAndEnd("templateId","root","2.16.840.1.113883.10.20.22.4.1");
				_w.WriteComment("Result organizer template");
				Guid();
				if(String.IsNullOrEmpty(labPanel.UsiID)) {
					StartAndEnd("code","nullFlavor","NA");//Null allowed for this code.
				}
				else {
					StartAndEnd("code","code",labPanel.UsiID,"codeSystem",strCodeSystemLoinc,"displayName",labPanel.UsiText);//Code systems allowed: LOINC, or other "local codes".
				}
				StartAndEnd("statusCode","code","completed");//page 532 Allowed values: aborted, active, cancelled, completed, held, suspended.
				Start("component");
				Start("observation","classCode","OBS","moodCode","EVN");
				TemplateId("2.16.840.1.113883.10.20.22.4.2");
				_w.WriteComment("Result observation template");
				Guid();
				if(String.IsNullOrEmpty(listLabResultFiltered[i].ObservationIdentifierID)) {
					StartAndEnd("code","nullFlavor","UNK");
				}
				else if(labLoinc==null) {
					StartAndEnd("code","code",listLabResultFiltered[i].ObservationIdentifierID,"displayName",listLabResultFiltered[i].ObservationIdentifierText,"codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
				}
				else {
					StartAndEnd("code","code",listLabResultFiltered[i].ObservationIdentifierID,"displayName",labLoinc.NameLongCommon,"codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc);
				}
				StartAndEnd("statusCode","code","completed");//Allowed values: aborted, active, cancelled, completed, held, or suspended.
				DateTime dateTimeEffective=DateTimeFromString(listLabResultFiltered[i].ObservationDateTime);
				if(dateTimeEffective.Year<1880) {
					StartAndEnd("effectiveTime","nullFlavor","UNK");
				}
				else {
					StartAndEnd("effectiveTime","value",listLabResultFiltered[i].ObservationDateTime);
				}
				Start("value");
				_w.WriteAttributeString("xsi","type",null,"PQ");
				if(value==null || value=="") {
					Attribs("nullFlavor","UNK");
				}
				else {
					Attribs("value",value,"unit",listLabResultFiltered[i].UnitsID);
				}
				End("value");
				StartAndEnd("interpretationCode","code","N","codeSystem","2.16.840.1.113883.5.83");
				Start("referenceRange");
				Start("observationRange");
				if(String.IsNullOrEmpty(listLabResultFiltered[i].referenceRange)) {
					StartAndEnd("text","nullFlavor","UNK");
				}
				else {
					_w.WriteElementString("text",listLabResultFiltered[i].referenceRange);
				}
				End("observationRange");
				End("referenceRange");
				End("observation");
				End("component");
				End("organizer");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCCD().  Exports smoking and pregnancy information.</summary>
		private void GenerateCcdSectionSocialHistory(bool hasSocialHistory) {
			_w.WriteComment(@"
=====================================================================================================
Social History
=====================================================================================================");
			List<EhrMeasureEvent> listEhrMeasureEventsFiltered;
			if(!hasSocialHistory) {
				listEhrMeasureEventsFiltered=new List<EhrMeasureEvent>();
			}
			else {
				listEhrMeasureEventsFiltered=_listEhrMeasureEventsFiltered;
			}
			listEhrMeasureEventsFiltered.Sort(CompareEhrMeasureEvents);
			//The pattern for this section is special. We do not have any lists to use in this section.
			//The section will always be present with at least one entry, beacuse we know the patient smoking status (which includes the UnknownIfEver option).
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.17");//Social History section (page 311). Only one template. No entries are required.
			_w.WriteComment("Social History section template");
			StartAndEnd("code","code","29762-2","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Social History");
			_w.WriteElementString("title","Social History");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listEhrMeasureEventsFiltered.Count>0 && hasSocialHistory) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Social History Element");
				_w.WriteElementString("th","Description");
				_w.WriteElementString("th","Effective Dates");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listEhrMeasureEventsFiltered.Count;i++) {
					Start("tr");
					_w.WriteElementString("td","Smoking");
					Snomed snomedSmoking=Snomeds.GetByCode(listEhrMeasureEventsFiltered[i].CodeValueResult);
					if(snomedSmoking==null) {//Could be null if the code was imported from another EHR.
						_w.WriteElementString("td","");
					}
					else {
						_w.WriteElementString("td",snomedSmoking.SnomedCode+" - "+snomedSmoking.Description);
					}
					DateTime dateTimeLow=listEhrMeasureEventsFiltered[i].DateTEvent;
					DateTime dateTimeHigh=DateTime.Now;
					if(i<listEhrMeasureEventsFiltered.Count-1) {//There is another smoking event after this one (remember, they are sorted by date).
						dateTimeHigh=listEhrMeasureEventsFiltered[i+1].DateTEvent.AddDays(-1);//The day before the next smoking event.
						if(dateTimeHigh<dateTimeLow) {
							dateTimeHigh=dateTimeLow;//Just in case the user entered two measures for the same date.
						}
					}
					if(dateTimeLow.Year<1880) {
						if(dateTimeHigh.Year<1880) {
							_w.WriteElementString("td","");
						}
						else {
							_w.WriteElementString("td",dateTimeHigh.ToString("yyyyMMdd"));
						}
					}
					else if(dateTimeHigh.Year<1880) {
						if(dateTimeLow.Year<1880) {
							_w.WriteElementString("td","");
						}
						else {
							_w.WriteElementString("td",dateTimeLow.ToString("yyyyMMdd"));
						}
					}
					else {
						_w.WriteElementString("td",dateTimeLow.ToString("yyyyMMdd")+" to "+dateTimeHigh.ToString("yyyyMMdd"));
					}
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listEhrMeasureEventsFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				EhrMeasureEvent eME=new EhrMeasureEvent();
				listEhrMeasureEventsFiltered.Add(eME);
			}
			for(int i=0;i<listEhrMeasureEventsFiltered.Count;i++) {
				//Pregnancy Observation Template could easily be added in the future, but for now it is skipped (Page 453)
				Start("entry","typeCode","DRIV");
				Start("observation","classCode","OBS","moodCode","EVN");
				TemplateId("2.16.840.1.113883.10.20.22.4.78");//Smoking Status Observation Section (Page 519).
				_w.WriteComment("Smoking Status Observation Template");
				StartAndEnd("code","code","ASSERTION","codeSystem","2.16.840.1.113883.5.4");
				StartAndEnd("statusCode","code","completed");//Allowed values: completed.
				//The effectiveTime/low element must be present. If the patient is an ex-smoker (8517006), the effectiveTime/high element must also be present. For simplicity, we always export both low and high dates.
				DateTime dateTimeLow=listEhrMeasureEventsFiltered[i].DateTEvent;
				DateTime dateTimeHigh=DateTime.Now;
				if(i<listEhrMeasureEventsFiltered.Count-1) {//There is another smoking event after this one (remember, they are sorted by date).
					dateTimeHigh=listEhrMeasureEventsFiltered[i+1].DateTEvent.AddDays(-1);//The day before the next smoking event.
					if(dateTimeHigh<dateTimeLow) {
						dateTimeHigh=dateTimeLow;//Just in case the user entered two measures for the same date.
					}
				}
				Start("effectiveTime");
				if(dateTimeLow.Year<1880) {
					StartAndEnd("low","nullFlavor","UNK");
				}
				else {
					DateElement("low",dateTimeLow);
				}
				if(dateTimeHigh.Year<1880) {
					StartAndEnd("high","nullFlavor","UNK");
				}
				else {
					DateElement("high",dateTimeHigh);
				}
				End("effectiveTime");
				Start("value");
				_w.WriteAttributeString("xsi","type",null,"CD");
				Snomed snomedSmoking=Snomeds.GetByCode(listEhrMeasureEventsFiltered[i].CodeValueResult);
				if(snomedSmoking==null) {
					Attribs("nullFlavor","UNK");
				}
				else {
					Attribs("code",snomedSmoking.SnomedCode,"displayName",snomedSmoking.Description,"codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed);
				}
				End("value");
				End("observation");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCcdSectionSocialHistory().  Sort function.  Currently sorts by date ascending.</summary>
		private int CompareEhrMeasureEvents(EhrMeasureEvent ehrMeasureEventL,EhrMeasureEvent ehrMeasureEventR) {
			if(ehrMeasureEventL.DateTEvent<ehrMeasureEventR.DateTEvent) {
				return -1;
			}
			else if(ehrMeasureEventL.DateTEvent>ehrMeasureEventR.DateTEvent) {
				return 1;
			}
			return 0;//equal
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void GenerateCcdSectionVitalSigns(bool hasVitalSign) {//Currently just a skeleton
			_w.WriteComment(@"
=====================================================================================================
Vital Signs
=====================================================================================================");
			List<Vitalsign> listVitalSignsFiltered;
			if(!hasVitalSign) {
				listVitalSignsFiltered=new List<Vitalsign>();
			}
			else {
				listVitalSignsFiltered=_listVitalSignsFiltered;
			}
			Start("component");
			Start("section");
			TemplateId("2.16.840.1.113883.10.20.22.2.4.1");//Vital signs section with coded entries required.
			_w.WriteComment("Vital Signs section template");
			StartAndEnd("code","code","8716-3","codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Vital Signs");
			_w.WriteElementString("title","Vital Signs");
			Start("text");//The following text will be parsed as html with a style sheet to be human readable.
			if(listVitalSignsFiltered.Count>0 && hasVitalSign) {
				Start("table","width","100%","border","1");
				Start("thead");
				Start("tr");
				_w.WriteElementString("th","Date");
				_w.WriteElementString("th","Height");
				_w.WriteElementString("th","Weight");
				_w.WriteElementString("th","BMI");
				_w.WriteElementString("th","Blood Pressure");
				End("tr");
				End("thead");
				Start("tbody");
				for(int i=0;i<listVitalSignsFiltered.Count;i++) {
					Vitalsign vitalsign=listVitalSignsFiltered[i];
					Start("tr");
					if(vitalsign.DateTaken.Year<1880) {
						_w.WriteElementString("td","");
					}
					else {
						DateText("td",vitalsign.DateTaken);
					}
					if(vitalsign.Height>0) {
						_w.WriteElementString("td",vitalsign.Height.ToString("f0")+" in");
					}
					else {
						_w.WriteElementString("td","");
					}
					if(vitalsign.Weight>0) {
						_w.WriteElementString("td",vitalsign.Weight.ToString("f0")+" lbs");
					}
					else {
						_w.WriteElementString("td","");
					}
					float bmi=Vitalsigns.CalcBMI(vitalsign.Weight,vitalsign.Height);//will be 0 if either wight is 0 or height is 0.
					if(bmi>0) {
						_w.WriteElementString("td",bmi.ToString("f1")+" lbs/in");
					}
					else {
						_w.WriteElementString("td","");
					}
					if(vitalsign.BpSystolic>0 && vitalsign.BpDiastolic>0) {
						_w.WriteElementString("td",vitalsign.BpSystolic.ToString("f0")+"/"+vitalsign.BpDiastolic.ToString("f0"));
					}
					else {
						_w.WriteElementString("td","");
					}
					End("tr");
				}
				End("tbody");
				End("table");
			}
			else {
				_w.WriteString("None");
			}
			End("text");
			if(listVitalSignsFiltered.Count==0) {//If there are no entries in the filtered list, then we want to add a dummy entry since at least one is required.
				Vitalsign viS=new Vitalsign();
				listVitalSignsFiltered.Add(viS);
			}
			for(int i=0;i<listVitalSignsFiltered.Count;i++) {//Fill Vital Signs Info
				Vitalsign vitalsign=listVitalSignsFiltered[i];
				Start("entry","typeCode","DRIV");
				Start("organizer","classCode","CLUSTER","moodCode","EVN");
				_w.WriteComment("Vital Signs Organizer template");//Vital Signs Organizer
				TemplateId("2.16.840.1.113883.10.20.22.4.26");
				Guid();
				StartAndEnd("code","code","46680005","codeSystem",strCodeSystemSnomed,"codeSystemName",strCodeSystemNameSnomed,"displayName","Vital signs");
				StartAndEnd("statusCode","code","completed");
				if(vitalsign.DateTaken.Year<1880) {
					StartAndEnd("effectiveTime","nullFlavor","UNK");
				}
				else {
					DateElement("effectiveTime",vitalsign.DateTaken);
				}
				GenerateCcdVitalSign("8302-2",vitalsign.DateTaken,vitalsign.Height,"in");//Height
				GenerateCcdVitalSign("3141-9",vitalsign.DateTaken,vitalsign.Weight,"lbs");//Weight
				float bmi=Vitalsigns.CalcBMI(vitalsign.Weight,vitalsign.Height);//will be 0 if either wight is 0 or height is 0.
				GenerateCcdVitalSign("39156-5",vitalsign.DateTaken,bmi,"lbs/in");//BMI
				GenerateCcdVitalSign("8480-6",vitalsign.DateTaken,vitalsign.BpSystolic,"mmHg");//Blood Pressure Systolic
				GenerateCcdVitalSign("8462-4",vitalsign.DateTaken,vitalsign.BpDiastolic,"mmHg");//Blood Pressure Diastolic
				End("organizer");
				End("entry");
			}
			End("section");
			End("component");
		}

		///<summary>Helper for GenerateCcdSectionVitalSigns(). Writes on observation. 
		///Allowed vital sign observation template LOINC codes (strLoincObservationCode):
		///9279-1		Respiratory Rate
		///8867-4		Heart Rate
		///2710-2		O2 % BldC Oximetry,
		///8480-6		BP Systolic
		///8462-4		BP Diastolic
		///8310-5		Body Temperature,
		///8302-2		Height
		///8306-3		Height (Lying)
		///8287-5		Head Circumference,
		///3141-9		Weight Measured
		///39156-5	BMI (Body Mass Index)
		///3140-1 BSA (Body Surface Area)</summary>
		private void GenerateCcdVitalSign(string strLoincObservationCode,DateTime dateTimeObservation,float observationValue,string observationUnits) {
			Start("component");
			Start("observation","classCode","OBS","moodCode","EVN");
			_w.WriteComment("Vital Sign Observation template");//Vital Sign Observation Section
			TemplateId("2.16.840.1.113883.10.20.22.4.27");
			Guid();
			StartAndEnd("code","code",strLoincObservationCode,"codeSystem",strCodeSystemLoinc,"codeSystemName",strCodeSystemNameLoinc,"displayName","Height");
			StartAndEnd("statusCode","code","completed");//Allowed values: completed.
			if(dateTimeObservation.Year<1880) {
				StartAndEnd("effectiveTime","nullFlavor","UNK");
			}
			else {
				DateElement("effectiveTime",dateTimeObservation);
			}
			Start("value");
			_w.WriteAttributeString("xsi","type",null,"PQ");
			if(observationValue==0) {
				Attribs("nullFlavor","UNK");
			}
			else {
				Attribs("value",observationValue.ToString("f0"),"unit",observationUnits);
			}
			End("value");
			End("observation");
			End("component");
		}

		///<summary>Helper for GenerateCCD(). Builds an "id" element and writes a random 32 character alpha-numeric string into the "root" attribute.</summary>
		private void Id() {
			string id=MiscUtils.CreateRandomAlphaNumericString(32);
			while(_hashCcdIds.Contains(id)) {
				id=MiscUtils.CreateRandomAlphaNumericString(32);
			}
			_hashCcdIds.Add(id);
			StartAndEnd("id","root",id);
		}

		///<summary>Helper for GenerateCCD(). Builds an "id" element and writes a 36 character GUID string into the "root" attribute.
		///An example of how the uid might look: "20cf14fb-B65c-4c8c-A54d-b0cca834C18c"</summary>
		private void Guid() {
			Guid uuid=System.Guid.NewGuid();
			while(_hashCcdGuids.Contains(uuid.ToString())) {
				uuid=System.Guid.NewGuid();
			}
			_hashCcdGuids.Add(uuid.ToString());
			StartAndEnd("id","root",uuid.ToString());
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void TemplateId(string rootNumber) {
			_w.WriteStartElement("templateId");
			_w.WriteAttributeString("root",rootNumber);
			_w.WriteEndElement();
		}

		///<summary>Helper for GenerateCCD().</summary>
		private void TemplateId(string rootNumber,string authorityName) {
			_w.WriteStartElement("templateId");
			_w.WriteAttributeString("root",rootNumber);
			_w.WriteAttributeString("assigningAuthorityName",authorityName);
			_w.WriteEndElement();
		}

		///<summary>Helper for GenerateCCD().  Performs a WriteStartElement, followed by any attributes.  Attributes must be in pairs: name, value.</summary>
		private void Start(string elementName,params string[] attributes) {
			_w.WriteStartElement(elementName);
			for(int i=0;i<attributes.Length;i+=2) {
				_w.WriteAttributeString(attributes[i],attributes[i+1]);
			}
		}

		///<summary>Helper for GenerateCCD().  Performs a WriteEndElement.  The specified elementName is for readability only.</summary>
		private void End(string elementName) {
			_w.WriteEndElement();
		}

		///<summary>Helper for GenerateCCD().  Performs a WriteStartElement, followed by any attributes, followed by a WriteEndElement.  Attributes must be in pairs: name, value.</summary>
		private void StartAndEnd(string elementName,params string[] attributes) {
			_w.WriteStartElement(elementName);
			for(int i=0;i<attributes.Length;i+=2) {
				_w.WriteAttributeString(attributes[i],attributes[i+1]);
			}
			_w.WriteEndElement();
		}

		///<summary>Helper for GenerateCCD().  Performs a WriteAttributeString for each attribute.  Attributes must be in pairs: name, value.</summary>
		private void Attribs(params string[] attributes) {
			for(int i=0;i<attributes.Length;i+=2) {
				_w.WriteAttributeString(attributes[i],attributes[i+1]);
			}
		}

		///<summary>Use for HTML tables. Writes the element strElement name and writes the dateTime string in the required date format.  Will not write if year is before 1880.</summary>
		private void DateText(string strElementName,DateTime dateTime) {
			Start(strElementName);
			if(dateTime.Year>1880) {
				_w.WriteString(dateTime.ToString("yyyyMMdd"));
			}
			End(strElementName);
		}

		///<summary>Use for XML. Writes the element strElement name and writes the dateTime in the required date format into the value attribute.
		///Will write nullFlavor="UNK" instead of value if year is before 1880.</summary>
		private void DateElement(string strElementName,DateTime dateTime) {
			Start(strElementName);
			if(dateTime.Year<1880) {
				Attribs("nullFlavor","UNK");
			}
			else {
				Attribs("value",dateTime.ToString("yyyyMMdd"));
			}
			End(strElementName);
		}

		///<summary>Writes the element strElement name and writes the dateTime in the required date format into the value attribute.
		///Will write nullFlavor="UNK" instead of value if year is before 1880.</summary>
		private void TimeElement(string strElementName,DateTime dateTime) {
			Start(strElementName);
			if(dateTime.Year<1880) {
				Attribs("nullFlavor","UNK");
			}
			else {
				Attribs("value",dateTime.ToString("yyyyMMddHHmmsszzz").Replace(":",""));
			}
			End(strElementName);
		}

		private void AddressUnitedStates(string strAddress1,string strAddress2,string strCity,string strState) {
			Start("addr","use","HP");
			_w.WriteElementString("streetAddressLine",strAddress1);
			if(strAddress2!="") {
				_w.WriteElementString("streetAddressLine",strAddress2);
			}
			_w.WriteElementString("city",strCity);
			_w.WriteElementString("state",strState);
			_w.WriteElementString("country","United States");
			End("addr");
		}

		///<summary>Does validation on the filtered lists. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateAll(Patient pat,DateTime date) {
			string err="";
			err=err+ValidateSettings();
			err=err+ValidatePatient(pat);
			err=err+ValidateAllergy(pat);
			err=err+ValidateEncounter(pat,date);
			err=err+ValidateFunctionalStatus(pat);
			err=err+ValidateImmunization(pat);
			err=err+ValidateLabResults(pat);
			err=err+ValidateMedication(pat);
			err=err+ValidatePlanOfCare(pat,date);
			err=err+ValidateProblem(pat);
			err=err+ValidateProcedure(pat,date);
			err=err+ValidateSocialHistory(pat);
			err=err+ValidateVitalsSign(pat,date);
			return err;
		}

		///<summary>Checks data values for preferences and provider information to ensure required data is available for CCD creation.
		///Returns empty string if no errors, otherwise returns a string containing error messages.</summary>
		public static string ValidateSettings() {
			string strErrors="";
			if(PrefC.GetString(PrefName.PracticeTitle).Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing practice title.";
			}
			if(PrefC.GetString(PrefName.PracticePhone).Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing practice phone.";
			}
			if(PrefC.GetString(PrefName.PracticeAddress).Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing practice address line 1.";
			}
			if(PrefC.GetString(PrefName.PracticeCity).Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing practice city.";
			}
			if(PrefC.GetString(PrefName.PracticeST).Trim().Length!=2) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Invalid practice state.  Must be two letters.";
			}
			Provider provDefault=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			if(provDefault.FName.Trim()=="" && !provDefault.IsNotPerson) {//Have a first name and is a person.
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provDefault.Abbr+" first name.";
			}
			if(provDefault.LName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provDefault.Abbr+" last name.";
			}
			if(provDefault.NationalProvID.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provDefault.Abbr+" NPI.";
			}
			if(Snomeds.GetCodeCount()==0) {
				//TODO: We need to replace this check with a more extensive check which validates the SNOMED codes that will actually go out on the CCD to ensure they are in our database.
				//One way a SNOMED code could get into the patinet record without being in the snomed table, would be via an imported CCD. We might get around this issue by simply
				//exporting the code without the description, because the descriptions are usually optional.
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing SNOMED codes.  Go to Setup | Chart | EHR | Code System Importer to download.";
			}
			if(Cvxs.GetCodeCount()==0) {
				//TODO: We need to replace this check with a more extensive check which validates the CVX codes that will actually go out on the CCD to ensure they are in our database.
				//One way a CVX code could get into the patinet record without being in the snomed table, would be via an imported CCD. We might get around this issue by simply
				//exporting the code without the description, because the descriptions are usually optional.
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing CVX codes.  Go to Setup | Chart | EHR | Code System Importer to download.";
			}
			return strErrors;
		}

		///<summary>Checks data values for pat as well as primary provider information to ensure required data is available for CCD creation.
		///Returns empty string if no errors, otherwise returns a string containing error messages.</summary>
		public static string ValidatePatient(Patient pat) {
			string strErrors="";
			if(pat.PriProv==0) {//this should never happen
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Primary provider must be set.";
			}
			if(pat.FName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient first name.";
			}
			if(pat.LName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient last name.";
			}
			if(pat.Address.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient address line 1.";
			}
			if(pat.City.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient city.";
			}
			if(pat.State.Trim().Length!=2) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Invalid patient state.  Must be two letters.";
			}
			if(pat.Birthdate.Year<1880) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient birth date.";
			}
			if(pat.HmPhone.Trim()=="" && pat.WirelessPhone.Trim()=="" && pat.WkPhone.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing patient phone. Must have home, wireless, or work phone.";
			}
			if(PrefC.HasClinicsEnabled && pat.ClinicNum!=0) {
				Clinic clinic=Clinics.GetClinic(pat.ClinicNum);
				if(clinic.Description.Trim()=="") {
					if(strErrors!="") {
						strErrors+="\r\n";
					}
					strErrors+="Missing clinic description.";
				}
				if(clinic.Phone.Trim()=="") {
					if(strErrors!="") {
						strErrors+="\r\n";
					}
					strErrors+="Missing clinic '"+clinic.Description+"' phone.";
				}
				if(clinic.Address.Trim()=="") {
					if(strErrors!="") {
						strErrors+="\r\n";
					}
					strErrors+="Missing clinic '"+clinic.Description+"' address line 1.";
				}
				if(clinic.City.Trim()=="") {
					if(strErrors!="") {
						strErrors+="\r\n";
					}
					strErrors+="Missing clinic '"+clinic.Description+"' city.";
				}
				if(clinic.State.Trim().Length!=2) {
					if(strErrors!="") {
						strErrors+="\r\n";
					}
					strErrors+="Invalid clinic '"+clinic.Description+"' state.  Must be two letters.";
				}
			}
			Provider provPractice=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			if(provPractice.FName.Trim()=="" && !provPractice.IsNotPerson) {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provPractice.Abbr+" first name.";
			}
			if(provPractice.LName.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provPractice.Abbr+" last name.";
			}
			if(provPractice.NationalProvID.Trim()=="") {
				if(strErrors!="") {
					strErrors+="\r\n";
				}
				strErrors+="Missing provider "+provPractice.Abbr+" NPI.";
			}
			if(pat.PriProv>0 && pat.PriProv!=PrefC.GetLong(PrefName.PracticeDefaultProv)) {
				Provider provPri=Providers.GetProv(pat.PriProv);
				if(provPri.FName.Trim()=="" && !provPri.IsNotPerson) {
					if(strErrors!="") {
						strErrors+="\r\n";
					}
					strErrors+="Missing provider "+provPri.Abbr+" first name.";
				}
				if(provPri.LName.Trim()=="") {
					if(strErrors!="") {
						strErrors+="\r\n";
					}
					strErrors+="Missing provider "+provPri.Abbr+" last name.";
				}
				if(provPri.NationalProvID.Trim()=="") {
					if(strErrors!="") {
						strErrors+="\r\n";
					}
					strErrors+="Missing provider "+provPri.Abbr+" NPI.";
				}
			}
			return strErrors;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateAllergy(Patient pat) {
			FilterAllergy(pat);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of allergies. Also runs validation.</summary>
		private static void FilterAllergy(Patient patCur) {
			//TODO: Add validation for UNII codes once the table has been implemented.
			AllergyDef allergyDef;
			List<Allergy> listAllergiesAll=Allergies.Refresh(patCur.PatNum);
			List<Allergy> listAllergiesFiltered=new List<Allergy>();
			for(int i=0;i<listAllergiesAll.Count;i++) {
				allergyDef=AllergyDefs.GetOne(listAllergiesAll[i].AllergyDefNum);
				bool isMedAllergy=false;
				if(allergyDef.MedicationNum!=0) {
					Medication med=Medications.GetMedication(allergyDef.MedicationNum);
					if(med.RxCui!=0) {
						isMedAllergy=true;
					}
				}
				if(allergyDef.SnomedType!=SnomedAllergy.AdverseReactionsToDrug && allergyDef.SnomedType!=SnomedAllergy.DrugAllergy && allergyDef.SnomedType!=SnomedAllergy.DrugIntolerance) {
					isMedAllergy=false;
				}
				//bool isSnomedAllergy=false;
				//if(allergyDef.SnomedAllergyTo!="") {
				//	isSnomedAllergy=true;
				//}
				if(!isMedAllergy) {// && !isSnomedAllergy) {
					if(allergyDef.UniiCode=="") {
						continue;//TODO: We need to add support for Ndf-RT 
					}
				}
				listAllergiesFiltered.Add(listAllergiesAll[i]);
			}
			_listAllergiesFiltered=listAllergiesFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateEncounter(Patient pat,DateTime date) {
			FilterEncounter(pat,date);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of encounters. Also runs validation.</summary>
		private static void FilterEncounter(Patient patCur,DateTime date) {
			List<Encounter> listEncountersAll=Encounters.Refresh(patCur.PatNum);
			List<Encounter> listEncountersFiltered=new List<Encounter>();
			for(int i=0;i<listEncountersAll.Count;i++) {
				if(listEncountersAll[i].CodeSystem!="SNOMEDCT") {
					continue;//The format only allows SNOMED codes and ICD10 codes. We do not have a way to enter ICD10 codes, and SNOMED appears to be the preferred code system anyway.
				}
				if(date!=DateTime.MinValue && listEncountersAll[i].DateEncounter.Date!=date.Date) {
					continue;//If there is a date restriction, then ignore encounters which are not on the given date.
				}
				listEncountersFiltered.Add(listEncountersAll[i]);
			}
			_listEncountersFiltered=listEncountersFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateFunctionalStatus(Patient pat) {
			FilterFunctionalStatus(pat);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of encounters. Also runs validation.</summary>
		private static void FilterFunctionalStatus(Patient patCur) {
			string snomedProblemType="55607006";
			List<Disease> listProblemsAll=Diseases.Refresh(patCur.PatNum);
			List<Disease> listProblemsFiltered=new List<Disease>();
			for(int i=0;i<listProblemsAll.Count;i++) {
				if(listProblemsAll[i].SnomedProblemType!="" && listProblemsAll[i].SnomedProblemType!=snomedProblemType) {
					continue;//Not a "problem".
				}
				if(listProblemsAll[i].FunctionStatus==FunctionalStatus.Problem) {
					continue;//Is a standard problem, not a cognitive or functional problem.
				}
				listProblemsFiltered.Add(listProblemsAll[i]);
			}
			_listProblemsFuncFiltered=listProblemsFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateImmunization(Patient pat) {
			FilterImmunization(pat);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of vaccines. Also runs validation.</summary>
		private static void FilterImmunization(Patient patCur) {
			List<VaccinePat> listVaccinePatsAll=VaccinePats.Refresh(patCur.PatNum);
			List<VaccinePat> listVaccinePatsFiltered=new List<VaccinePat>();
			for(int i=0;i<listVaccinePatsAll.Count;i++) {
				//No Filters for this
				listVaccinePatsFiltered.Add(listVaccinePatsAll[i]);
			}
			_listVaccinePatsFiltered=listVaccinePatsFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateMedication(Patient pat) {
			FilterMedication(pat);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of medications. Also runs validation.</summary>
		private static void FilterMedication(Patient patCur) {
			List<MedicationPat> listMedPatsAll=MedicationPats.Refresh(patCur.PatNum,true);
			List<MedicationPat> listMedPatsFiltered=new List<MedicationPat>();
			for(int i=0;i<listMedPatsAll.Count;i++) {
				//No filters currently
				listMedPatsFiltered.Add(listMedPatsAll[i]);
			}
			_listMedPatsFiltered=listMedPatsFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidatePlanOfCare(Patient pat,DateTime date) {
			FilterPlanOfCare(pat,date);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of care plans. Also runs validation.</summary>
		private static void FilterPlanOfCare(Patient patCur,DateTime date) {
			List<EhrCarePlan> listEhrCarePlansAll=EhrCarePlans.Refresh(patCur.PatNum);
			List<EhrCarePlan> listEhrCarePlansFiltered=new List<EhrCarePlan>();
			for(int i=0;i<listEhrCarePlansAll.Count;i++) {
				if(date!=DateTime.MinValue && listEhrCarePlansAll[i].DatePlanned.Date<date.Date) {
					continue;//Exclude care plans which are in the past if a date limitation is specified.
				}
				listEhrCarePlansFiltered.Add(listEhrCarePlansAll[i]);
			}
			_listEhrCarePlansFiltered=listEhrCarePlansFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateProblem(Patient pat) {
			FilterProblem(pat);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of problems. Also runs validation.</summary>
		private static void FilterProblem(Patient patCur) {
			string snomedProblemType="55607006";
			List<Disease> listProblemsAll=Diseases.Refresh(patCur.PatNum);
			List<Disease> listProblemsFiltered=new List<Disease>();
			for(int i=0;i<listProblemsAll.Count;i++) {
				if(listProblemsAll[i].SnomedProblemType!="" && listProblemsAll[i].SnomedProblemType!=snomedProblemType) {
					continue;//Not a "problem".
				}
				if(listProblemsAll[i].FunctionStatus!=FunctionalStatus.Problem) {
					continue;//Not a "problem".
				}
				listProblemsFiltered.Add(listProblemsAll[i]);
			}
			_listProblemsFiltered=listProblemsFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateProcedure(Patient pat,DateTime date) {
			FilterProcedure(pat,date);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of procedures. Also runs validation.</summary>
		private static void FilterProcedure(Patient patCur,DateTime date) {
			List<Procedure> listProcsAll=Procedures.Refresh(patCur.PatNum);
			List<Procedure> listProcsFiltered=new List<Procedure>();
			for(int i=0;i<listProcsAll.Count;i++) {
				ProcedureCode procCode=ProcedureCodes.GetProcCode(listProcsAll[i].CodeNum);
				if(listProcsAll[i].ProcStatus==ProcStat.D) {
					continue;//Ignore deleted procedures.
				}
				if(listProcsAll[i].ProcStatus==ProcStat.TP) {
					continue;//Ignore treatment planned procedures.  These procedures should be sent out in the Care Plan section in the future.  We are not required to send treatment planned items.
				}
				if(listProcsAll[i].ProcStatus==ProcStat.R) {
					continue;//Ignore procedures referred out.  It is the responsibility of the treating dentist to record work they have performed.
				}
				if(date!=DateTime.MinValue && listProcsAll[i].ProcDate.Date!=date.Date) {
					continue;//Ignore procedures which are not on the given date if a date limitation was specified.
				}
				listProcsFiltered.Add(listProcsAll[i]);
			}
			_listProcsFiltered=listProcsFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateLabResults(Patient pat) {
			FilterLabResults(pat);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of lab results. Also runs validation.</summary>
		private static void FilterLabResults(Patient patCur) {
			List<EhrLabResult> listLabResultAll=EhrLabResults.GetAllForPatient(patCur.PatNum);
			List<EhrLabResult> listLabResultFiltered=new List<EhrLabResult>();
			for(int i=0;i<listLabResultAll.Count;i++) {
				if(listLabResultAll[i].ObservationIdentifierID=="") {
					continue;//Blank codes not allowed in format.
				}
				listLabResultFiltered.Add(listLabResultAll[i]);
			}
			_listLabResultFiltered=listLabResultFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateSocialHistory(Patient pat) {
			FilterSocialHistory(pat);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of procedures. Also runs validation.</summary>
		private static void FilterSocialHistory(Patient patCur) {
			List<EhrMeasureEvent> listEhrMeasureEventsAll=EhrMeasureEvents.Refresh(patCur.PatNum);
			List<EhrMeasureEvent> listEhrMeasureEventsFiltered=new List<EhrMeasureEvent>();
			for(int i=0;i<listEhrMeasureEventsAll.Count;i++) {
				if(listEhrMeasureEventsAll[i].EventType!=EhrMeasureEventType.TobaccoUseAssessed) {
					continue;
				}
				if(listEhrMeasureEventsAll[i].CodeSystemResult!="SNOMEDCT") {
					continue;//The user is currently only allowed to pick SNOMED smoking statuses. This is here in case we add more code system in the future, to prevent the format from breaking until we enhance.
				}
				listEhrMeasureEventsFiltered.Add(listEhrMeasureEventsAll[i]);
			}
			_listEhrMeasureEventsFiltered=listEhrMeasureEventsFiltered;
		}

		///<summary>Does validation on the filtered list. NEEDS TO BE ENHANCED.</summary>
		private static string ValidateVitalsSign(Patient pat,DateTime date) {
			FilterVitalSign(pat,date);
			//TODO: Add validation
			return "";
		}

		///<summary>Filters list of procedures. Also runs validation.</summary>
		private static void FilterVitalSign(Patient patCur,DateTime date) {
			List<Vitalsign> listVitalSignsAll=Vitalsigns.Refresh(patCur.PatNum);
			List<Vitalsign> listVitalSignsFiltered=new List<Vitalsign>();
			for(int i=0;i<listVitalSignsAll.Count;i++) {
				Vitalsign vitalsign=listVitalSignsAll[i];
				//Each of the vital sign values are optional, so we must skip filter out empty vital signs.
				float bmi=Vitalsigns.CalcBMI(vitalsign.Weight,vitalsign.Height);//will be 0 if either wight is 0 or height is 0.
				if(vitalsign.Height==0 && vitalsign.Weight==0 && bmi==0 && (vitalsign.BpSystolic==0 || vitalsign.BpDiastolic==0)) {
					continue;//Nothing to report.
				}
				if(date!=DateTime.MinValue && vitalsign.DateTaken.Date!=date.Date) {
					continue;//Ignore vital signs which are not on the given date if a date limitation was specified.
				}
				listVitalSignsFiltered.Add(listVitalSignsAll[i]);
			}
			_listVitalSignsFiltered=listVitalSignsFiltered;
		}

		#endregion CCD Creation

		#region CCD Reading

		///<summary>Returns the PatNum for the unique patient who matches the patient name and birthdate within the CCD document xmlDocCcd.
		///Returns 0 if there are no patient matches.  Returns the first match if there are multiple matches (unlikely).</summary>
		public static long GetCCDpat(XmlDocument xmlDocCcd) {
			XmlNodeList xmlNodeList=xmlDocCcd.GetElementsByTagName("patient");//According to the CCD documentation, there should be one patient node, or no patient node.
			if(xmlNodeList.Count==0) {
				return 0;//No patient node, therefore no patient to match.
			}
			XmlNode xmlNodePat=xmlNodeList[0];
			string fName="";
			string lName="";
			DateTime birthDate=DateTime.MinValue;
			for(int i=0;i<xmlNodePat.ChildNodes.Count;i++) {
				if(xmlNodePat.ChildNodes[i].Name.Trim().ToLower()=="name") {
					XmlNode xmlNodeName=xmlNodePat.ChildNodes[i];
					for(int j=0;j<xmlNodeName.ChildNodes.Count;j++) {
						if(xmlNodeName.ChildNodes[j].Name.Trim().ToLower()=="given") {
							if(fName=="") {//The first and middle names are both referred to as "given" name.  The first given name is the patient's first name.
								fName=xmlNodeName.ChildNodes[j].InnerText.Trim();
							}
						}
						else if(xmlNodeName.ChildNodes[j].Name.Trim().ToLower()=="family") {
							lName=xmlNodeName.ChildNodes[j].InnerText.Trim();
						}
					}
				}
				else if(xmlNodePat.ChildNodes[i].Name.Trim().ToLower()=="birthtime") {
					XmlNode xmlNodeBirthtime=xmlNodePat.ChildNodes[i];
					for(int j=0;j<xmlNodeBirthtime.Attributes.Count;j++) {
						if(xmlNodeBirthtime.Attributes[j].Name.Trim().ToLower()!="value") {
							continue;
						}
						string strBirthTimeVal=xmlNodeBirthtime.Attributes[j].Value;
						int birthYear=int.Parse(strBirthTimeVal.Substring(0,4));//Year will always be in the first 4 digits of the date, for all flavors of the HL7 TS type.
						int birthMonth=1;
						if(strBirthTimeVal.Length>=6) {
							birthMonth=int.Parse(strBirthTimeVal.Substring(4,2));//If specified, month will always be in the 5th-6th digits of the date, for all flavors of the HL7 TS type.
						}
						int birthDay=1;
						if(strBirthTimeVal.Length>=8) {
							birthDay=int.Parse(strBirthTimeVal.Substring(6,2));//If specified, day will always be in the 7th-8th digits of the date, for all flavors of the HL7 TS type.
						}
						birthDate=new DateTime(birthYear,birthMonth,birthDay);
					}
				}
			}
			//A match cannot be made unless the CCD message includes first and last name as well as a specified birthdate, 
			//because we do not want to automatically attach Direct messages to patients unless we are certain that the match makes sense.
			//The user can always manually attach the incoming Direct message to a patient if the automatic matching fails, so it is good that the automatic matching is strict.
			//Automatic matching is not required for EHR, but it is "highly recommended when possible" according to Drummond.
			if(lName=="" || fName=="" || birthDate.Year<1880) {
				return 0;
			}
			return Patients.GetPatNumByNameAndBirthday(lName,fName,birthDate);
		}

		///<summary>Recursive. Returns all nodes matching the specified tag name (case insensitive) which also have all of the specified attributes (case sensitive names).
		///Attributes must be listed in pairs by attribute name then attribute value.</summary>
		private static List<XmlNode> GetNodesByTagNameAndAttributes(XmlNode xmlNode,string strTagName,params string[] arrayAttributes) {
			//Test the current node for tag name and attributes.
			List<XmlNode> retVal=new List<XmlNode>();
			if(xmlNode.Name.Trim().ToLower()==strTagName.Trim().ToLower()) {//Tag name match.
				bool isAttributeMatch=true;
				for(int i=0;i<arrayAttributes.Length;i+=2) {
					string strAttributeName=arrayAttributes[i];
					string strAttributeValue=arrayAttributes[i+1];
					if(xmlNode.Attributes[strAttributeName].Value.Trim().ToLower()!=strAttributeValue.Trim().ToLower()) {
						isAttributeMatch=false;
						break;
					}
				}
				if(isAttributeMatch) {
					retVal.Add(xmlNode);
				}
			}
			//Test child nodes.
			for(int i=0;i<xmlNode.ChildNodes.Count;i++) {
				retVal.AddRange(GetNodesByTagNameAndAttributes(xmlNode.ChildNodes[i],strTagName,arrayAttributes));
			}
			return retVal;
		}

		private static List<XmlNode> GetParentNodes(List<XmlNode> listXmlNodes) {
			List<XmlNode> retVal=new List<XmlNode>();
			for(int i=0;i<listXmlNodes.Count;i++) {
				retVal.Add(listXmlNodes[i].ParentNode);
			}
			return retVal;
		}

		///<summary>Calls GetNodesByTagNameAndAttributes() for each item in listXmlNode.</summary>
		private static List<XmlNode> GetNodesByTagNameAndAttributesFromList(List<XmlNode> listXmlNode,string strTagName,params string[] arrayAttributes) {
			List<XmlNode> retVal=new List<XmlNode>();
			for(int i=0;i<listXmlNode.Count;i++) {
				retVal.AddRange(GetNodesByTagNameAndAttributes(listXmlNode[i],strTagName,arrayAttributes));
			}
			return retVal;
		}

		private static DateTime DateTimeFromString(string strDateTime) {
			if(strDateTime==null) {
				return DateTime.MinValue;
			}
			string strDateTimeFormat="";
			if(strDateTime.Length==19) {
				strDateTimeFormat="yyyyMMddHHmmsszzz";
			}
			else if(strDateTime.Length==17) {
				strDateTimeFormat="yyyyMMddHHmmzzz";
			}
			else if(strDateTime.Length==8) {
				strDateTimeFormat="yyyyMMdd";
			}
			else if(strDateTime.Length==6) {
				strDateTimeFormat="yyyyMM";
			}
			else if(strDateTime.Length==4) {
				strDateTimeFormat="yyyy";
			}
			try {
				return DateTime.ParseExact(strDateTime,strDateTimeFormat,CultureInfo.CurrentCulture.DateTimeFormat);
			}
			catch {
			}
			return DateTime.MinValue;
		}

		///<summary>Gets the start date (aka low node) from the effectiveTime node passed in.  Returns the date time value set in the low node if present.  If low node does not exist, it returns the value within the effectiveTime node.  Returns MinValue if low attribute is "nullflavor" or if parsing fails.</summary>
		private static DateTime GetEffectiveTimeLow(XmlNode xmlNodeEffectiveTime) {
			DateTime strEffectiveTimeValue=DateTime.MinValue;
			List<XmlNode> listLowVals=GetNodesByTagNameAndAttributes(xmlNodeEffectiveTime,"low");
			if(listLowVals.Count>0 && listLowVals[0].Attributes["nullFlavor"]!=null) {
				return strEffectiveTimeValue;
			}
			if(listLowVals.Count>0 && listLowVals[0].Attributes["value"]!=null) {
				strEffectiveTimeValue=DateTimeFromString(listLowVals[0].Attributes["value"].Value);
			}
			else if(xmlNodeEffectiveTime.Attributes["value"]!=null) {
				strEffectiveTimeValue=DateTimeFromString(xmlNodeEffectiveTime.Attributes["value"].Value);
			}
			return strEffectiveTimeValue;
		}

		private static DateTime GetEffectiveTimeHigh(XmlNode xmlNodeEffectiveTime) {
			DateTime strEffectiveTimeValue=DateTime.MinValue;
			List<XmlNode> listLowVals=GetNodesByTagNameAndAttributes(xmlNodeEffectiveTime,"high");
			if(listLowVals.Count>0 && listLowVals[0].Attributes["nullFlavor"]!=null) {
				return strEffectiveTimeValue;
			}
			if(listLowVals.Count>0 && listLowVals[0].Attributes["value"]!=null) {
				strEffectiveTimeValue=DateTimeFromString(listLowVals[0].Attributes["value"].Value);
			}
			else {
				//We do not take the string from the xmlNodeEffectiveTime value attribute, because we need to be careful importing high/stop dates.
				//The examples suggest that th xmlNodeEffectiveTime value attribute will contain the minimum datetime.
			}
			return strEffectiveTimeValue;
		}

		///<summary>Fills listMedicationPats and listMedications using the information found in the CCD document xmlDocCcd.  Does NOT insert any records into the db.</summary>
		public static void GetListMedicationPats(XmlDocument xmlDocCcd,List<MedicationPat> listMedicationPats) {
			//The length of listMedicationPats and listMedications will be the same. The information in listMedications might have duplicates.
			//Neither list of objects will be inserted into the db, so there will be no primary or foreign keys.
			//List<XmlNode> listMedicationDispenseTemplates=GetNodesByTagNameAndAttributes(xmlDocCcd,"templateId","root","2.16.840.1.113883.10.20.22.4.18");//Medication Dispense template.
			List<XmlNode> listMedicationDispenseTemplates=GetNodesByTagNameAndAttributes(xmlDocCcd,"templateId","root","2.16.840.1.113883.10.20.22.4.16");//Medication Activity template.
			List<XmlNode> listSupply=GetParentNodes(listMedicationDispenseTemplates);//POCD_HD00040.xls line 485
			for(int i=0;i<listSupply.Count;i++) {
				//We have to start fairly high in the tree so that we can get the effective time if it is available.
				List<XmlNode> xmlNodeEffectiveTimes=GetNodesByTagNameAndAttributes(listSupply[i],"effectiveTime");//POCD_HD00040.xls line 492. Not required.
				DateTime dateTimeEffectiveLow=DateTime.MinValue;
				DateTime dateTimeEffectiveHigh=DateTime.MinValue;
				if(xmlNodeEffectiveTimes.Count>0) {
					XmlNode xmlNodeEffectiveTime=xmlNodeEffectiveTimes[0];
					dateTimeEffectiveLow=GetEffectiveTimeLow(xmlNodeEffectiveTime);
					dateTimeEffectiveHigh=GetEffectiveTimeHigh(xmlNodeEffectiveTime);
				}
				List<XmlNode> listMedicationActivityTemplates=GetNodesByTagNameAndAttributes(listSupply[i],"templateId","root","2.16.840.1.113883.10.20.22.4.23");//Medication Activity template.
				List<XmlNode> listProducts=GetParentNodes(listMedicationActivityTemplates);//List of manufaturedProduct and/or manufacturedLabeledDrug. POCD_HD00040.xls line 472.
				List<XmlNode> listCodes=GetNodesByTagNameAndAttributesFromList(listProducts,"code");
				for(int j=0;j<listCodes.Count;j++) {
					XmlNode xmlNodeCode=listCodes[j];
					if(xmlNodeCode.Attributes["nullFlavor"]!=null) {
						continue;
					}
					string strCode=xmlNodeCode.Attributes["code"].Value;
					string strMedDescript=xmlNodeCode.Attributes["displayName"].Value;
					if(xmlNodeCode.Attributes["codeSystem"].Value!=strCodeSystemRxNorm) {
						continue;//We can only import RxNorms, because we have nowhere to pull in any other codes at this time (for example, SNOMED).
					}
					MedicationPat medicationPat=new MedicationPat();
					medicationPat.IsNew=true;//Needed for reconcile window to know this record is not in the db yet.
					medicationPat.RxCui=PIn.Long(strCode);
					medicationPat.MedDescript=strMedDescript;
					medicationPat.DateStart=dateTimeEffectiveLow;
					medicationPat.DateStop=dateTimeEffectiveHigh;
					listMedicationPats.Add(medicationPat);
				}
			}
			List<MedicationPat> listMedicationPatsNoDupes=new List<MedicationPat>();
			bool isDupe;
			for(int index=0;index<listMedicationPats.Count;index++) {
				isDupe=false;
				for(int index2=0;index2<listMedicationPatsNoDupes.Count;index2++) {
					if(listMedicationPatsNoDupes[index2].RxCui==listMedicationPats[index].RxCui) {
						isDupe=true;
						break;
					}
				}
				if(!isDupe) {
					listMedicationPatsNoDupes.Add(listMedicationPats[index]);
				}
			}
			listMedicationPats.Clear();
			for(int dupeIndex=0;dupeIndex<listMedicationPatsNoDupes.Count;dupeIndex++) {
				listMedicationPats.Add(listMedicationPatsNoDupes[dupeIndex]);
			}
		}

		///<summary>Fills listDiseases and listDiseaseDef using the information found in the CCD document xmlDocCcd.  Does NOT insert any records into the db.</summary>
		public static void GetListDiseases(XmlDocument xmlDocCcd,List<Disease> listDiseases,List<DiseaseDef> listDiseaseDef) {
			//The length of listDiseases and listDiseaseDef will be the same. The information in listDiseaseDef might have duplicates.
			//Neither list of objects will be inserted into the db, so there will be no primary or foreign keys.
			List<XmlNode> listProblemActTemplate=GetNodesByTagNameAndAttributes(xmlDocCcd,"templateId","root","2.16.840.1.113883.10.20.22.4.3");// problem act template.
			List<XmlNode> listProbs=GetParentNodes(listProblemActTemplate);
			for(int i=0;i<listProbs.Count;i++) {
				//We have to start fairly high in the tree so that we can get the effective time if it is available.
				List<XmlNode> xmlNodeEffectiveTimes=GetNodesByTagNameAndAttributes(listProbs[i],"effectiveTime");
				DateTime dateTimeEffectiveLow=DateTime.MinValue;
				DateTime dateTimeEffectiveHigh=DateTime.MinValue;
				if(xmlNodeEffectiveTimes.Count>0) {
					XmlNode xmlNodeEffectiveTime=xmlNodeEffectiveTimes[0];
					dateTimeEffectiveLow=GetEffectiveTimeLow(xmlNodeEffectiveTime);
					dateTimeEffectiveHigh=GetEffectiveTimeHigh(xmlNodeEffectiveTime);
				}
				List<XmlNode> listProblemObservTemplate=GetNodesByTagNameAndAttributes(listProbs[i],"templateId","root","2.16.840.1.113883.10.20.22.4.4");// problem act template.
				List<XmlNode> listProbObs=GetParentNodes(listProblemObservTemplate);
				List<XmlNode> listTypeCodes=GetNodesByTagNameAndAttributesFromList(listProbObs,"code");
				List<XmlNode> listCodes=GetNodesByTagNameAndAttributesFromList(listProbObs,"value");
				if(listCodes[0].Attributes["nullFlavor"]!=null) {
					continue;
				}
				string probType=listTypeCodes[0].Attributes["code"].Value;
				string probCode=listCodes[0].Attributes["code"].Value;
				string probName=listCodes[0].Attributes["displayName"].Value;
				List<XmlNode> listStatusObservTemplate=GetNodesByTagNameAndAttributes(listProbs[i],"templateId","root","2.16.840.1.113883.10.20.22.4.6");// Status Observation template.
				List<XmlNode> listStatusObs=GetParentNodes(listStatusObservTemplate);
				List<XmlNode> listActive=GetNodesByTagNameAndAttributesFromList(listStatusObs,"value");
				Disease dis=new Disease();
				dis.SnomedProblemType=probType;
				dis.DateStart=dateTimeEffectiveLow;
				dis.IsNew=true;
				if(listActive.Count>0 && listActive[0].Attributes["code"].Value=="55561003") {//Active (qualifier value)
					dis.ProbStatus=ProblemStatus.Active;
				}
				else if(listActive.Count>0 && listActive[0].Attributes["code"].Value=="413322009") {//Problem resolved (finding)
					dis.ProbStatus=ProblemStatus.Resolved;
					dis.DateStop=dateTimeEffectiveHigh;
				}
				else {
					dis.ProbStatus=ProblemStatus.Inactive;
					dis.DateStop=dateTimeEffectiveHigh;
				}
				listDiseases.Add(dis);
				DiseaseDef disD=new DiseaseDef();
				disD.IsHidden=false;
				disD.IsNew=true;
				disD.SnomedCode=probCode;
				disD.DiseaseName=probName;
				listDiseaseDef.Add(disD);
			}
		}

		///<summary>Fills listAllergies and listAllergyDefs using the information found in the CCD document xmlDocCcd.  Inserts a medication in the db corresponding to the allergy.</summary>
		public static void GetListAllergies(XmlDocument xmlDocCcd,List<Allergy> listAllergies,List<AllergyDef> listAllergyDefs) {
			//The length of listAllergies and listAllergyDefs will be the same. The information in listAllergyDefs might have duplicates.
			//Neither list of objects will be inserted into the db, so there will be no primary or foreign keys.
			List<XmlNode> listAllergyProblemActTemplate=GetNodesByTagNameAndAttributes(xmlDocCcd,"templateId","root","2.16.840.1.113883.10.20.22.4.30");//Allergy problem act template.
			List<XmlNode> listActs=GetParentNodes(listAllergyProblemActTemplate);
			for(int i=0;i<listActs.Count;i++) {
				//We have to start fairly high in the tree so that we can get the effective time if it is available.
				List<XmlNode> xmlNodeEffectiveTimes=GetNodesByTagNameAndAttributes(listActs[i],"effectiveTime");//POCD_HD00040.xls line 492. Not required.
				DateTime dateTimeEffectiveLow=DateTime.MinValue;
				DateTime dateTimeEffectiveHigh=DateTime.MinValue;
				if(xmlNodeEffectiveTimes.Count>0) {
					XmlNode xmlNodeEffectiveTime=xmlNodeEffectiveTimes[0];
					dateTimeEffectiveLow=GetEffectiveTimeLow(xmlNodeEffectiveTime);
					dateTimeEffectiveHigh=GetEffectiveTimeHigh(xmlNodeEffectiveTime);
				}
				List<XmlNode> listAllergyObservationTemplates=GetNodesByTagNameAndAttributes(listActs[i],"templateId","root","2.16.840.1.113883.10.20.22.4.7");//Allergy observation template.
				List<XmlNode> listAllergy=GetParentNodes(listAllergyObservationTemplates);//List of Allergy Observations.
				List<XmlNode> listCodes=GetNodesByTagNameAndAttributesFromList(listAllergy,"value");
				#region Determine if Active
				bool isActive=true;
				string strStatus="";
				List<XmlNode> listAllergyObservationTemplatesActive=GetNodesByTagNameAndAttributes(listActs[i],"templateId","root","2.16.840.1.113883.10.20.22.4.28");//Allergy observation template.
				List<XmlNode> listAllergyActive=GetParentNodes(listAllergyObservationTemplatesActive);//List of Allergy Observations.
				List<XmlNode> listCodesActive=GetNodesByTagNameAndAttributesFromList(listAllergyActive,"value");
				if(listCodesActive.Count>0) {
					listCodes.Remove(listCodesActive[0]);
					XmlNode xmlNodeCode=listCodesActive[0];
					if(xmlNodeCode.Attributes["nullFlavor"]!=null) {
						continue;
					}
					strStatus=xmlNodeCode.Attributes["code"].Value;
					if(xmlNodeCode.Attributes["codeSystem"].Value!=strCodeSystemSnomed) {
						continue;//We can only import Snomeds
					}
					isActive=(strStatus=="55561003");//Active (qualifier value)
				}
				#endregion
				#region Find Reaction Snomed
				List<XmlNode> listAllergyStatusObservationTemplates=GetNodesByTagNameAndAttributes(listActs[i],"templateId","root","2.16.840.1.113883.10.20.22.4.9");//Allergy status observation template.
				List<XmlNode> listAllergyStatus=GetParentNodes(listAllergyStatusObservationTemplates);//List of Allergy Observations.
				List<XmlNode> listAlgCodes=GetNodesByTagNameAndAttributesFromList(listAllergyStatus,"value");
				for(int j=0;j<listAlgCodes.Count;j++) {
					listCodes.Remove(listAlgCodes[j]);
					XmlNode xmlNodeCode=listAlgCodes[j];
					string strCodeReaction=xmlNodeCode.Attributes["code"].Value;
					string strAlgStatusDescript=xmlNodeCode.Attributes["displayName"].Value;
					if(xmlNodeCode.Attributes["codeSystem"].Value!=strCodeSystemSnomed) {
						continue;//We can only import Snomeds
					}
					Allergy allergy=new Allergy();
					allergy.IsNew=true;//Needed for reconcile window to know this record is not in the db yet.
					allergy.SnomedReaction=PIn.String(strCodeReaction);
					allergy.Reaction=PIn.String(strAlgStatusDescript);
					allergy.DateAdverseReaction=dateTimeEffectiveLow;
					allergy.StatusIsActive=isActive;
					listAllergies.Add(allergy);
				}
				#endregion
				#region Remove Severe Reaction
				List<XmlNode> listAllergySevereTemplates=GetNodesByTagNameAndAttributes(listActs[i],"templateId","root","2.16.840.1.113883.10.20.22.4.8");//Allergy observation template.
				List<XmlNode> listAllergySevere=GetParentNodes(listAllergySevereTemplates);//List of Allergy Observations.
				List<XmlNode> listCodesSevere=GetNodesByTagNameAndAttributesFromList(listAllergySevere,"value");
				for(int j=0;j<listCodesSevere.Count;j++) {
					listCodes.Remove(listCodesSevere[j]);
					XmlNode xmlNodeCode=listCodesSevere[j];
					string strCodeReaction=xmlNodeCode.Attributes["code"].Value;
					string strAlgStatusDescript=xmlNodeCode.Attributes["displayName"].Value;
					if(xmlNodeCode.Attributes["codeSystem"].Value!=strCodeSystemSnomed) {
						continue;//We can only import Snomeds
					}
				}
				#endregion
				#region Find RxNorm or Snomed
				string allergyDefName="";
				Medication med=new Medication();
				List<XmlNode> listRxCodes=GetNodesByTagNameAndAttributesFromList(listAllergy,"code");
				List<Medication> allergyMeds=new List<Medication>();
				for(int j=0;j<listRxCodes.Count;j++) {
					XmlNode xmlNodeCode=listRxCodes[j];
					if(xmlNodeCode.Attributes[0].Name!="code") {
						continue;
					}
					if(xmlNodeCode.Attributes["codeSystem"].Value!=strCodeSystemRxNorm) {
						continue;//We only want RxNorms here.
					}
					string strCodeRx=xmlNodeCode.Attributes["code"].Value;
					string strRxName=xmlNodeCode.Attributes["displayName"].Value;//Look into this being required or not.
					allergyDefName=strRxName;
					med=Medications.GetMedicationFromDbByRxCui(PIn.Long(strCodeRx));
					if(med==null) {
						med=new Medication();
						med.MedName=strRxName;
						med.RxCui=PIn.Long(strCodeRx);
						Medications.Insert(med);
						med.GenericNum=med.MedicationNum;
						Medications.Update(med);
					}
					allergyMeds.Add(med);
				}
				#endregion
				for(int j=0;j<listCodes.Count;j++) {
					XmlNode xmlNodeCode=listCodes[j];
					string strCode=xmlNodeCode.Attributes["code"].Value;
					if(xmlNodeCode.Attributes["codeSystem"].Value!=strCodeSystemSnomed) {
						continue;//We can only import Snomeds
					}
					AllergyDef allergyDef=new AllergyDef();
					allergyDef.IsNew=true;//Needed for reconcile window to know this record is not in the db yet.
					if(med.MedicationNum!=0) {
						allergyDef.MedicationNum=med.MedicationNum;
					}
					//else {TODO: Change to Unii
					//	allergyDef.SnomedAllergyTo=PIn.String(strCode);
					//}
					allergyDef.Description=allergyDefName;
					allergyDef.IsHidden=false;
					allergyDef.MedicationNum=allergyMeds[j].MedicationNum;
					#region Snomed type determination
					if(strCode=="419511003") {
						allergyDef.SnomedType=SnomedAllergy.AdverseReactionsToDrug;
					}
					else if(strCode=="418471000") {
						allergyDef.SnomedType=SnomedAllergy.AdverseReactionsToFood;
					}
					else if(strCode=="419199007") {
						allergyDef.SnomedType=SnomedAllergy.AdverseReactionsToSubstance;
					}
					else if(strCode=="418038007") {
						allergyDef.SnomedType=SnomedAllergy.AllergyToSubstance;
					}
					else if(strCode=="416098002") {
						allergyDef.SnomedType=SnomedAllergy.DrugAllergy;
					}
					else if(strCode=="59037007") {
						allergyDef.SnomedType=SnomedAllergy.DrugIntolerance;
					}
					else if(strCode=="414285001") {
						allergyDef.SnomedType=SnomedAllergy.FoodAllergy;
					}
					else if(strCode=="235719002") {
						allergyDef.SnomedType=SnomedAllergy.FoodIntolerance;
					}
					else if(strCode=="420134006") {
						allergyDef.SnomedType=SnomedAllergy.AdverseReactions;
					}
					else {
						allergyDef.SnomedType=SnomedAllergy.None;
					}
					#endregion
					listAllergyDefs.Add(allergyDef);
				}
			}
		}

		#endregion CCD Reading

		#region Helpers

		public static bool IsCCD(string strXml) {
			XmlDocument doc=new XmlDocument();
			try {
				doc.LoadXml(strXml);
			}
			catch {
				return false;
			}
			if(doc.DocumentElement.Name.ToLower()=="clinicaldocument") {//CCD and CCDA
				return true;
			}
			else if(doc.DocumentElement.Name.ToLower()=="continuityofcarerecord" || doc.DocumentElement.Name.ToLower()=="ccr:continuityofcarerecord") {//CCR
				return true;
			}
			return false;
		}

		public static bool IsCcdEmailAttachment(EmailAttach emailAttach) {
			string strFilePathAttach=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),emailAttach.ActualFileName);
			if(Path.GetExtension(strFilePathAttach).ToLower()!=".xml") {
				return false;
			}
			string strTextXml=FileAtoZ.ReadAllText(strFilePathAttach);
			if(!EhrCCD.IsCCD(strTextXml)) {
				return false;
			}
			return true;
		}

		public static bool HasCcdEmailAttachment(EmailMessage emailMessage) {
			if(emailMessage.Attachments==null) {
				return false;
			}
			for(int i=0;i<emailMessage.Attachments.Count;i++) {
				if(EhrCCD.IsCcdEmailAttachment(emailMessage.Attachments[i])) {
					return true;
				}
			}
			return false;
		}


		#endregion Helpers

	}
}
