using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	public class ErxXml {

		///<summary>Only called from Chart for now.  No validation is performed here.  Validate before calling.  There are many validtion checks, including the NPI must be exactly 10 digits.</summary>
		public static string BuildNewCropClickThroughXml(Provider prov,Employee emp,Patient pat,out NCScript ncScript) {
			string deaNumDefault=ProviderClinics.GetDEANum(prov.ProvNum);
			ncScript=new NCScript();
			ncScript.Credentials=new CredentialsType();
			ncScript.Credentials.partnerName=NewCrop.NewCropPartnerName;
			ncScript.Credentials.name=NewCrop.NewCropAccountName;
			ncScript.Credentials.password=NewCrop.NewCropAccountPasssword;
			ncScript.Credentials.productName=NewCrop.NewCropProductName;
			ncScript.Credentials.productVersion=NewCrop.NewCropProductVersion;
			ncScript.UserRole=new UserRoleType();
			bool isMidlevel=false;
			if(emp==null) {//Provider
				if(prov.IsSecondary) {//Mid-level provider
					isMidlevel=true;
					//Secondary (HYG) providers go accross to NewCrop as midlevel providers for now to satisfy the Ohio prescriber requirements.
					//HYG providers are not normally able to click through to NewCrop because they do not have an NPI number and an NPI is required.
					//In the future, instead of using the IsSecondary flag as as workaround, we should instead create a new field on the provider table
					//or perhaps the userod table to allow the user to select the type of provider.
					ncScript.UserRole.user=UserType.MidlevelPrescriber;
					ncScript.UserRole.role=RoleType.midlevelPrescriber;
				}
				else {//Fully licensed provider
					ncScript.UserRole.user=UserType.LicensedPrescriber;
					ncScript.UserRole.role=RoleType.doctor;
				}
			}
			else {//Employee
				ncScript.UserRole.user=UserType.Staff;
				ncScript.UserRole.role=RoleType.nurse;
			}
			ncScript.Destination=new DestinationType();
			ncScript.Destination.requestedPage=RequestedPageType.compose;//This is the tab that the user will want 90% of the time.
			string practiceTitle=Tidy(PrefC.GetString(PrefName.PracticeTitle),50);//May be blank.
			string practicePhone=PrefC.GetString(PrefName.PracticePhone);//Validated to be 10 digits within the chart.
			string practiceFax=PrefC.GetString(PrefName.PracticeFax);//Validated to be 10 digits within the chart.
			string practiceAddress=PrefC.GetString(PrefName.PracticeAddress);//Validated to exist in chart.
			string practiceAddress2=PrefC.GetString(PrefName.PracticeAddress2);//May be blank.
			string practiceCity=PrefC.GetString(PrefName.PracticeCity);//Validated to exist in chart.
			string practiceState=PrefC.GetString(PrefName.PracticeST).ToUpper();//Validated to be a US state code in chart.
			string practiceZip=Regex.Replace(PrefC.GetString(PrefName.PracticeZip),"[^0-9]*","");//Zip with all non-numeric characters removed. Validated to be 9 digits in chart.
			string practiceZip4=practiceZip.Substring(5);//Last 4 digits of zip.
			practiceZip=practiceZip.Substring(0,5);//First 5 digits of zip.
			string country="US";//Always United States for now.
			//if(CultureInfo.CurrentCulture.Name.Length>=2) {
			//  country=CultureInfo.CurrentCulture.Name.Substring(CultureInfo.CurrentCulture.Name.Length-2);
			//}
			ncScript.Account=new AccountTypeRx();
			//Each LicensedPrescriberID must be unique within an account. Since we send ProvNum for LicensedPrescriberID, each OD database must have a unique AccountID.
			ncScript.Account.ID=PrefC.GetString(PrefName.NewCropAccountId);//Customer account number then a dash then a random alpha-numeric string of 3 characters, followed by 2 digits.
			ncScript.Account.accountName=practiceTitle;//May be blank.
			ncScript.Account.siteID="1";//Always send 1.  For each AccountID/SiteID pair, a separate database will be created in NewCrop.
			ncScript.Account.AccountAddress=new AddressType();
			ncScript.Account.AccountAddress.address1=practiceAddress;//Validated to exist in chart.
			ncScript.Account.AccountAddress.address2=practiceAddress2;//May be blank.
			ncScript.Account.AccountAddress.city=practiceCity;//Validated to exist in chart.
			ncScript.Account.AccountAddress.state=practiceState;//Validated to be a US state code in chart.
			ncScript.Account.AccountAddress.zip=practiceZip;//Validated to be 9 digits in chart. First 5 digits go in this field.
			ncScript.Account.AccountAddress.zip4=practiceZip4;//Validated to be 9 digits in chart. Last 4 digits go in this field.
			ncScript.Account.AccountAddress.country=country;//Validated above.
			ncScript.Account.accountPrimaryPhoneNumber=practicePhone;//Validated to be 10 digits within the chart.
			ncScript.Account.accountPrimaryFaxNumber=practiceFax;//Validated to be 10 digits within the chart.
			ncScript.Location=new LocationType();
			ProviderClinic provClinic=ProviderClinics.GetOne(prov.ProvNum,0);//Default providerclinic.  This is needed for offices that don't use clinics.
			if(!PrefC.HasClinicsEnabled
				|| (!PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected) && pat.ClinicNum==0)
				|| (PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected) && Clinics.ClinicNum==0 && pat.ClinicNum==0))
			{ //No clinic.
				ncScript.Location.ID="0";//Always 0, since clinicnums must be >= 1, will never overlap with a clinic if the office turns clinics on after first use.
				ncScript.Location.locationName=practiceTitle;//May be blank.
				ncScript.Location.LocationAddress=new AddressType();
				ncScript.Location.LocationAddress.address1=practiceAddress;//Validated to exist in chart.
				ncScript.Location.LocationAddress.address2=practiceAddress2;//May be blank.
				ncScript.Location.LocationAddress.city=practiceCity;//Validated to exist in chart.
				ncScript.Location.LocationAddress.state=practiceState;//Validated to be a US state code in chart.
				ncScript.Location.LocationAddress.zip=practiceZip;//Validated to be 9 digits in chart. First 5 digits go in this field.
				ncScript.Location.LocationAddress.zip4=practiceZip4;//Validated to be 9 digits in chart. Last 4 digits go in this field.
				ncScript.Location.LocationAddress.country=country;//Validated above.
				ncScript.Location.primaryPhoneNumber=practicePhone;//Validated to be 10 digits within the chart.
				ncScript.Location.primaryFaxNumber=practiceFax;//Validated to be 10 digits within the chart.
				ncScript.Location.pharmacyContactNumber=practicePhone;//Validated to be 10 digits within the chart.
			}
			else { //Using clinics.
				Clinic clinic=null;
				if(PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected) && Clinics.ClinicNum!=0) {
					clinic=Clinics.GetClinic(Clinics.ClinicNum);
				}
				else {
					clinic=Clinics.GetClinic(pat.ClinicNum);
				}
				provClinic=ProviderClinics.GetOneOrDefault(prov.ProvNum,clinic.ClinicNum);
				ncScript.Location.ID=clinic.ClinicNum.ToString();//A positive integer.
				ncScript.Location.locationName=clinic.Description;//May be blank.
				ncScript.Location.LocationAddress=new AddressType();
				ncScript.Location.LocationAddress.address1=clinic.Address;//Validated to exist in chart.
				ncScript.Location.LocationAddress.address2=clinic.Address2;//May be blank.
				ncScript.Location.LocationAddress.city=clinic.City;//Validated to exist in chart.
				ncScript.Location.LocationAddress.state=clinic.State.ToUpper();//Validated to be a US state code in chart.
				string clinicZip=Regex.Replace(clinic.Zip,"[^0-9]*","");//Zip with all non-numeric characters removed. Validated to be 9 digits in chart.
				string clinicZip4=clinicZip.Substring(5);//Last 4 digits of zip.
				clinicZip=clinicZip.Substring(0,5);//First 5 digits of zip.
				ncScript.Location.LocationAddress.zip=clinicZip;//Validated to be 9 digits in chart. First 5 digits go in this field.
				ncScript.Location.LocationAddress.zip4=clinicZip4;//Validated to be 9 digits in chart. Last 4 digits go in this field.
				ncScript.Location.LocationAddress.country=country;//Validated above.
				ncScript.Location.primaryPhoneNumber=clinic.Phone;//Validated to be 10 digits within the chart.
				ncScript.Location.primaryFaxNumber=clinic.Fax;//Validated to be 10 digits within the chart.
				ncScript.Location.pharmacyContactNumber=clinic.Phone;//Validated to be 10 digits within the chart.
			}
			//Each unique provider ID sent to NewCrop will cause a billing charge.
			//Some customer databases have provider duplicates, because they have one provider record per clinic with matching NPIs.
			//We send NPI as the ID to prevent extra NewCrop charges.
			//Conversation with NewCrop:
			//Question: If one of our customers clicks through to NewCrop with 2 different LicensedPrescriber.ID values, 
			//          but with the same provider name and NPI, will Open Dental be billed twice or just one time for the NPI used?
			//Answer:   "They would be billed twice. The IDs you send us should always be maintained and unique. 
			//          Users are always identified by LicensedPrescriber ID, since their name or credentials could potentially change."
			if(isMidlevel) {
				ncScript.MidlevelPrescriber=new MidlevelPrescriberType();
				ncScript.MidlevelPrescriber.ID=prov.NationalProvID;
				//UPIN is obsolete
				ncScript.MidlevelPrescriber.LicensedPrescriberName=NewCrop.GetPersonNameForProvider(prov);
				if(deaNumDefault.ToLower()=="none") {
					ncScript.MidlevelPrescriber.dea="NONE";
				}
				else {
					ncScript.MidlevelPrescriber.dea=deaNumDefault;
				}
				if(provClinic!=null) {
					if(provClinic.DEANum!=deaNumDefault) {
						//Only set the locationDea if it is different than the default.
						ncScript.MidlevelPrescriber.locationDea=provClinic.DEANum;
					}
					ncScript.MidlevelPrescriber.licenseState=provClinic.StateWhereLicensed.ToUpper();//Validated to be a US state code in the chart.
					ncScript.MidlevelPrescriber.licenseNumber=provClinic.StateLicense;//Validated to exist in chart.
				}
				ncScript.MidlevelPrescriber.npi=prov.NationalProvID;//Validated to be 10 digits in chart.
			}
			else {//Licensed presriber
				ncScript.LicensedPrescriber=new LicensedPrescriberType();
				ncScript.LicensedPrescriber.ID=prov.NationalProvID;
				//UPIN is obsolete
				ncScript.LicensedPrescriber.LicensedPrescriberName=NewCrop.GetPersonNameForProvider(prov);
				if(deaNumDefault.ToLower()=="none") {
					ncScript.LicensedPrescriber.dea="NONE";
				}
				else {
					ncScript.LicensedPrescriber.dea=deaNumDefault;
				}
				if(provClinic!=null) {
					if(provClinic.DEANum!=deaNumDefault) {
						//Only set the locationDea if it is different than the default.
						ncScript.LicensedPrescriber.locationDea=provClinic.DEANum;
					}
					ncScript.LicensedPrescriber.licenseState=provClinic.StateWhereLicensed.ToUpper();//Validated to be a US state code in the chart.
					ncScript.LicensedPrescriber.licenseNumber=provClinic.StateLicense;//Validated to exist in chart.
				}
				ncScript.LicensedPrescriber.npi=prov.NationalProvID;//Validated to be 10 digits in chart.
				//ncScript.LicensedPrescriber.freeformCredentials=;//This is where DDS and DMD should go, but we don't support this yet. Probably not necessary anyway.
			}
			if(emp!=null) {
				ncScript.Staff=new StaffType();
				ncScript.Staff.ID="emp"+emp.EmployeeNum.ToString();//A positive integer. Returned in the ExternalUserID field when retreiving prescriptions from NewCrop. Also, provider ID is returned in the same field if a provider created the prescription, so that we can create a distintion between employee IDs and provider IDs.
				ncScript.Staff.StaffName=new PersonNameType();
				ncScript.Staff.StaffName.first=emp.FName;//First name or last name will not be blank. Validated in Chart.
				ncScript.Staff.StaffName.last=emp.LName;//First name or last name will not be blank. Validated in Chart.
				ncScript.Staff.StaffName.middle=emp.MiddleI;//May be blank.
			}
			ncScript.Patient=new PatientType();
			ncScript.Patient.ID=pat.PatNum.ToString();//A positive integer.
			ncScript.Patient.PatientName=new PersonNameType();
			ncScript.Patient.PatientName.last=pat.LName;//Validated to exist in Patient Edit window.
			ncScript.Patient.PatientName.first=pat.FName;//May be blank.
			ncScript.Patient.PatientName.middle=pat.MiddleI;//May be blank.
			ncScript.Patient.medicalRecordNumber=pat.PatNum.ToString();//A positive integer.
			//NewCrop specifically requested that we do not send SSN.
			//ncScript.Patient.socialSecurityNumber=Regex.Replace(pat.SSN,"[^0-9]*","");//Removes all non-numerical characters.
			ncScript.Patient.PatientAddress=new AddressOptionalType();
			ncScript.Patient.PatientAddress.address1=pat.Address;//May be blank.
			ncScript.Patient.PatientAddress.address2=pat.Address2;//May be blank.
			ncScript.Patient.PatientAddress.city=pat.City;//May be blank.
			ncScript.Patient.PatientAddress.state=pat.State.ToUpper();//May be blank. Validated in chart to be blank or to be a valid US state code.
			//For some reason, NewCrop will fail to load if a 9 digit zip code is sent.
			//One customer had all 9 digit zip codes entered for their patients, so we added code here to only send the first 5 digits of the zip.
			//Patient zip is validated in Chart to be blank, or #####, or #####-####, or #########.
			if(pat.Zip=="") {
				ncScript.Patient.PatientAddress.zip="";//Blank is allowed.
			}
			else {//5 or 9 digit zip. Formats are #####, or #####-####, or #########.
				ncScript.Patient.PatientAddress.zip=pat.Zip.Substring(0,5);//First 5 digts only.
			}
			ncScript.Patient.PatientAddress.country=country;//Validated above.
			ncScript.Patient.PatientContact=new ContactType();
			//ncScript.Patient.PatientContact.backOfficeFax=;//We do not have a field to pull this information from.
			//ncScript.Patient.PatientContact.backOfficeTelephone=;//We do not have a field to pull this information from.
			ncScript.Patient.PatientContact.cellularTelephone=pat.WirelessPhone.TrimStart('1');//May be blank. Required to be 10 digits per NewCrop.
			ncScript.Patient.PatientContact.email=pat.Email;//May be blank, or may also contain multiple email addresses separated by commas.
			//ncScript.Patient.PatientContact.fax=;//We do not have a field to pull this information from.
			ncScript.Patient.PatientContact.homeTelephone=pat.HmPhone.TrimStart('1');//May be blank. Required to be 10 digits per NewCrop.
			//ncScript.Patient.PatientContact.pagerTelephone=;//We do not have a field to pull this information from.
			ncScript.Patient.PatientContact.workTelephone=pat.WkPhone.TrimStart('1');//May be blank. Required to be 10 digits per NewCrop.
			ncScript.Patient.PatientCharacteristics=new PatientCharacteristicsType();
			ncScript.Patient.PatientCharacteristics.dob=pat.Birthdate.ToString("yyyyMMdd");//DOB must be in CCYYMMDD format.
			if(pat.Gender==PatientGender.Male) {
				ncScript.Patient.PatientCharacteristics.gender=GenderType.M;
			}
			else if(pat.Gender==PatientGender.Female) {
				ncScript.Patient.PatientCharacteristics.gender=GenderType.F;
			}
			else {
				ncScript.Patient.PatientCharacteristics.gender=GenderType.U;
			}
			ncScript.Patient.PatientCharacteristics.genderSpecified=true;
			ncScript.Patient.PatientDiagnosis=GetPatDiagnoses(pat.PatNum);
			foreach(Vitalsign vitalsign in Vitalsigns.Refresh(pat.PatNum).OrderByDescending(x => x.DateTaken)){
				if(vitalsign.Height!=0 && ncScript.Patient.PatientCharacteristics.height.IsNullOrEmpty()) {//Only set once
					ncScript.Patient.PatientCharacteristics.height=vitalsign.Height.ToString();
					ncScript.Patient.PatientCharacteristics.heightUnits="inches";//No HeightUnitType field like the one for weight below
				}
				if(vitalsign.Weight!=0 && ncScript.Patient.PatientCharacteristics.weight.IsNullOrEmpty()) {//Only set once
					ncScript.Patient.PatientCharacteristics.weight=vitalsign.Weight.ToString();
					ncScript.Patient.PatientCharacteristics.weightUnits=WeightUnitType.lbs1;
					ncScript.Patient.PatientCharacteristics.weightUnitsSpecified=true;
				}
			}
			//NewCrop programmer's comments regarding other fields we are not currently using (these fields are sent back when fetching prescriptions in the Chart):
			//ExternalPrescriptionId = your unique identifier for the prescription, only to be used if you are generating the prescription on your own UI.
			//	This is referenced by NewCrop, and cannot be populated with any other value. 
			//EncounterIdentifier = unique ID for the patient visit (e.g. John Doe, 11/11/2013).
			//	This is used by NewCrop for reporting events against a visit, but otherwise does not impact the session. 
			//EpisodeIdentifier = unique ID for the patient’s issue (e.g. John Doe’s broken leg) which may include multiple visits.
			//	Currently not used by NewCrop except for being echoed back; it is possible this functionality would be expanded in the future based on its intent as noted. 
			//ExternalSource = a codified field noting the origin of the prescription. This may not be used.
			//Serialize
			MemoryStream memoryStream=new MemoryStream();
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(NCScript));
			xmlSerializer.Serialize(memoryStream,ncScript);
			byte[] memoryStreamInBytes=memoryStream.ToArray();
			return Encoding.UTF8.GetString(memoryStreamInBytes,0,memoryStreamInBytes.Length);
		}

		///<summary>Gets all diagnoses for procedures in the patient's history which are complete or treatment planned.
		///This list is given to NewCrop and is presented to the user in a combobox so the user can select the appropriate code(s).</summary>
		private static PatientDiagnosisType[] GetPatDiagnoses(long patNum) {
			List<PatientDiagnosisType> listPatDiagnosis=new List<PatientDiagnosisType>();
			List<string> listIcd10Codes=new List<string>();
			Procedures.Refresh(patNum).FindAll(x => x.IcdVersion==10 && (x.ProcStatus==ProcStat.C || x.ProcStatus==ProcStat.TP)).ForEach(x => {
				listIcd10Codes.Add(x.DiagnosticCode);
				listIcd10Codes.Add(x.DiagnosticCode2);
				listIcd10Codes.Add(x.DiagnosticCode3);
				listIcd10Codes.Add(x.DiagnosticCode4);
			});
			listIcd10Codes=listIcd10Codes.FindAll(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
			//If customer calls stating that the ICD10 codes are not being sent to NewCrop,
			//then the customer can fix by downloading the ICD10 codeset through the import tool.
			Icd10s.GetByCodes(listIcd10Codes).ForEach(x => {
				listPatDiagnosis.Add(new PatientDiagnosisType() {
					diagnosisID=x.Icd10Code,
					diagnosisType=DiagnosisType.ICD10,
					diagnosisName=x.Description,
				});
			});
			return listPatDiagnosis.ToArray();
		}

		public static byte[] BuildNewCropPostDataBytes(Provider prov,Employee emp,Patient pat,out string clickThroughXml) {
			clickThroughXml=BuildNewCropClickThroughXml(prov,emp,pat,out _);
			string xmlBase64=System.Web.HttpUtility.HtmlEncode(Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(clickThroughXml)));
			xmlBase64=xmlBase64.Replace("+","%2B");//A common base 64 character which needs to be escaped within URLs.
			xmlBase64=xmlBase64.Replace("/","%2F");//A common base 64 character which needs to be escaped within URLs.
			xmlBase64=xmlBase64.Replace("=","%3D");//Base 64 strings usually end in '=', but parameters also use '=' so we must escape.
			String postdata="RxInput=base64:"+xmlBase64;
			return System.Text.Encoding.UTF8.GetBytes(postdata);
		}

		///<summary>Throws exceptions for invalid Patient data.</summary>
		public static byte[] BuildDoseSpotPostDataBytes(string clinicID,string clinicKey,string userID,string onBehalfOfUserId,Patient pat,out string queryString) {
			queryString=DoseSpot.GetSingleSignOnQueryString(clinicID,clinicKey,userID,onBehalfOfUserId,pat);
			return ASCIIEncoding.ASCII.GetBytes(queryString);
		}

		///<summary>Throws exceptions for invalid Patient data.</summary>
		public static byte[] BuildDoseSpotPostDataBytesRefillsErrors(string clinicID,string clinicKey,string userID,out string queryString) {
			queryString=DoseSpot.GetSingleSignOnQueryString(clinicID,clinicKey,userID,"",null);
			return ASCIIEncoding.ASCII.GetBytes(queryString);
		}

		///<summary>Cleans supplied string to conform to XML standards.  Certain special characters are problematic for NewCrop even when converted
		///to valid XML.  For example, an input of "This &amp; That" can be converted to a valid XML string of "This &amp; That", however NewCrop has an 
		///aplhanumeric constraint on many fields in the XML.  The string "This &amp; That" violates the aplhanumeric contraint due to the ';'</summary>
		private static string Tidy(string input,int length) {
			//https://msdn.microsoft.com/en-us/library/system.security.securityelement.escape(v=vs.110).aspx
			string result=input.Replace("<","");//Remove '<' characters, since they would violate alphanumeric constraints, even if converted to valid XML as "&lt;".
			result=result.Replace(">","");//Remove '>' characters, since they would violate alphanumeric constraints, even if converted to valid XML as "&gt;".
			result=result.Replace("\"","");//Remove '"' characters, since they would violate alphanumeric constraints, even if converted to valid XML as "&quot;".
			result=result.Replace("'","");//Remove '\'' characters, since they would violate alphanumeric constraints, even if converted to valid XML as "&apos;".
			result=result.Replace("&","and");//Replace '&' characters, since they would violate alphanumeric constraints, even if converted to valid XML as "&amp;".
			if(result.Length>length) {
				result=result.Substring(0,length);
			}
			return result;
		}

	}
}
