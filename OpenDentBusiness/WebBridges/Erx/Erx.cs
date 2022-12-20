using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	public class Erx {

		public class PropertyDescs {
			public static string ErxOption="eRx Option";
			public static string ClinicID="DoseSpot Clinic ID";
			public static string ClinicKey="DoseSpot Clinic Key";
		}

		public static ErxOption GetErxOption() {
			Program program=Programs.GetCur(ProgramName.eRx);
			if(program==null) {
				throw new ODException(Lans.g("eRx","The eRx bridge is missing from the database."));
			}
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(program.ProgramNum);
			ProgramProperty programProperty=listProgramProperties.FirstOrDefault(x => x.PropertyDesc==PropertyDescs.ErxOption);
			if(programProperty==null) {
				throw new ODException(Lans.g("eRx","The eRx Option program property is missing from the database."));
			}
			return PIn.Enum<ErxOption>(programProperty.PropertyValue);
		}

		///<summary>If this value is found in a MedicationPat or RxPat's ErxGuid column, it is a med/rx created in Open Dental, NOT imported from an eRx solution.</summary>
		public static string OpenDentalErxPrefix="OD_SelfReported";

		///<summary>If this value is found in a MedicationPat or RxPat's ErxGuid column, it is a med/rx created in DoseSpot with the "Patient Reported" medication option.
		///These should never be imported as prescriptions, per Brad.</summary>
		public static string DoseSpotPatReportedPrefix="DS_PatientReported";
		///<summary>If this value is found in a MedicationPat or RxPat's ErxGuid column, it was created in Open Dental and has not yet been sent to DoseSpot. 
		///The ID after this prefix is the primary key for a medicationpat. This allows us to keep the medicationpat and rxpat connected and switch both over to
		///the correct external ID after syncing with DoseSpot.</summary>
		public static string UnsentPrefix="Unsent";

		public static long InsertOrUpdateErxMedication(RxPat rxPatOld,RxPat rxPat,long rxCui,string strDrugName,string strGenericName,bool isProv,bool canInsertRx=true) {
			if(rxPatOld==null) {
				if(canInsertRx) {
					rxPat.IsNew=true;//Might not be necessary, but does not hurt.
					rxPat.IsErxOld=false;
					SecurityLogs.MakeLogEntry(Permissions.RxCreate,rxPat.PatNum,"eRx automatically created: "+rxPat.Drug);
					RxPats.Insert(rxPat);
				}
			}
			else {//The prescription was already in our database. Update it.
				rxPat.RxNum=rxPatOld.RxNum;
				//Preserve the pharmacy on the existing prescription, in case the user set the value manually.
				//We do not pull pharmacy back from eRx yet.
				rxPat.PharmacyNum=rxPatOld.PharmacyNum;
				rxPat.ProvNum=rxPatOld.ProvNum;
				if(rxPatOld.IsErxOld) {
					rxPat.IsErxOld=true;
					rxPat.SendStatus=RxSendStatus.SentElect;//To maintain backward compatibility.
				}
				RxPats.Update(rxPat);
			}
			//If rxCui==0, then third party eRx option (DoseSpot, NewCrop, etc) did not provide an RxCui.  
			//Attempt to locate an RxCui using the other provided drug information.  An RxCui is not required for our program.  
			//Meds missing an RxCui are not exported in CCD messages.
			if(rxCui==0 && strDrugName!="") {
				List<RxNorm> listRxNorms=RxNorms.GetListByCodeOrDesc(strDrugName,true,true);//Exact case insensitive match ignoring numbers.
				if(listRxNorms.Count>0) {
					rxCui=PIn.Long(listRxNorms[0].RxCui);
				}
			}
			//If rxCui==0, then third party eRx option (DoseSpot, NewCrop, etc) did not provide an RxCui and we could not locate an RxCui by DrugName
			//Try searching by GenericName.
			if(rxCui==0 && strGenericName!="") {
				List<RxNorm> listRxNorms=RxNorms.GetListByCodeOrDesc(strGenericName,true,true);//Exact case insensitive match ignoring numbers.
				if(listRxNorms.Count>0) {
					rxCui=PIn.Long(listRxNorms[0].RxCui);
				}
			}
			//If rxCui==0, then third party eRx option (DoseSpot, NewCrop, etc) did not provide an RxCui and we could not 
			//locate an RxCui by DrugName or GenericName.
			if(rxCui==0) {
				//We may need to enhance in future to support more advanced RxNorm searches.
				//For example: DrugName=Cafatine, DrugInfo=Cafatine 1 mg-100 mg Tab, GenericName=ergotamine-caffeine.
				//This drug could not be found by DrugName nor GenericName, but could be found when the GenericName was split by non-alpha characters, 
				//then the words in the generic name were swapped.
				//Namely, "caffeine ergotamine" is in the RxNorm table.
			}
			//MedicationNum of 0, because we do not want to bloat the medication list in OD.
			//In this special situation, we instead set the MedDescript, RxCui and ErxGuid columns.
			return MedicationPats.InsertOrUpdateMedOrderForRx(rxPat,rxCui,isProv,canInsertRx);
		}

		///<summary>Throws exception if anything about the practice information is not valid.
		///All intended exceptions are ODExceptions and should be translated by the caller.</summary>
		public static void ValidatePracticeInfo() {
			string practicePhone=PrefC.GetString(PrefName.PracticePhone);
			if(!Regex.IsMatch(practicePhone,"^[0-9]{10}$")) {//"^[0-9]{10}(x[0-9]+)?$")) {
				throw new ODException(Lans.g("Erx","Practice phone must be exactly 10 digits."));
			}
			if(practicePhone.StartsWith("555")) {
				throw new ODException(Lans.g("Erx","Practice phone cannot start with 555."));
			}
			if(Regex.IsMatch(practicePhone,"^[0-9]{3}555[0-9]{4}$")) {
				throw new ODException(Lans.g("Erx","Practice phone cannot contain 555 in the middle 3 digits."));
			}
			string practiceFax=PrefC.GetString(PrefName.PracticeFax);
			if(!Regex.IsMatch(practiceFax,"^[0-9]{10}(x[0-9]+)?$")) {
				throw new ODException(Lans.g("Erx","Practice fax must be exactly 10 digits."));
			}
			if(practiceFax.StartsWith("555")) {
				throw new ODException(Lans.g("Erx","Practice fax cannot start with 555."));
			}
			if(Regex.IsMatch(practiceFax,"^[0-9]{3}555[0-9]{4}$")) {
				throw new ODException(Lans.g("Erx","Practice fax cannot contain 555 in the middle 3 digits."));
			}
			if(PrefC.GetString(PrefName.PracticeAddress)=="") {
				throw new ODException(Lans.g("Erx","Practice address blank."));
			}
			if(Regex.IsMatch(PrefC.GetString(PrefName.PracticeAddress),@".*( |^)P\.?O\.? .*",RegexOptions.IgnoreCase)) {
				throw new ODException(Lans.g("Erx","Practice address cannot be a PO BOX."));
			}
			if(PrefC.GetString(PrefName.PracticeCity)=="") {
				throw new ODException(Lans.g("Erx","Practice city blank."));
			}
			if(!USlocales.IsValidAbbr(PrefC.GetString(PrefName.PracticeST))) {
				throw new ODException(Lans.g("Erx","Practice state abbreviation invalid."));
			}
			string practiceZip=Regex.Replace(PrefC.GetString(PrefName.PracticeZip),"[^0-9]*","");//Zip with all non-numeric characters removed.
			if(practiceZip.Length!=9) {
				throw new ODException(Lans.g("Erx","Practice zip must be 9 digits."));
			}
		}

		///<summary>Throws exception if anything about the practice information is not valid.
		///All intended exceptions are Exceptions and are already translated.</summary>
		public static void ValidateClinic(Clinic clinic) {
			if(clinic==null) {
				throw new Exception("Clinic not found");
			}
			if(!Regex.IsMatch(clinic.Phone,"^[0-9]{10}?$")) {
				throw new ODException(Lans.g("Erx","Clinic phone must be exactly 10 digits")+": "+clinic.Description);
			}
			if(clinic.Phone.StartsWith("555")) {
				throw new ODException(Lans.g("Erx","Clinic phone cannot start with 555")+": "+clinic.Description);
			}
			if(Regex.IsMatch(clinic.Phone,"^[0-9]{3}555[0-9]{4}$")) {
				throw new ODException(Lans.g("Erx","Clinic phone cannot contain 555 in the middle 3 digits")+": "+clinic.Description);
			}
			if(!Regex.IsMatch(clinic.Fax,"^[0-9]{10}?$")) {
				throw new ODException(Lans.g("Erx","Clinic fax must be exactly 10 digits")+": "+clinic.Description);
			}
			if(clinic.Fax.StartsWith("555")) {
				throw new ODException(Lans.g("Erx","Clinic fax cannot start with 555")+": "+clinic.Description);
			}
			if(Regex.IsMatch(clinic.Fax,"^[0-9]{3}555[0-9]{4}$")) {
				throw new ODException(Lans.g("Erx","Clinic fax cannot contain 555 in the middle 3 digits")+": "+clinic.Description);
			}
			if(clinic.Address=="") {
				throw new ODException(Lans.g("Erx","Clinic address blank")+": "+clinic.Description);
			}
			if(Regex.IsMatch(clinic.Address,@".*( |^)P\.?O\.? .*",RegexOptions.IgnoreCase)) {
				throw new ODException(Lans.g("Erx","Clinic address cannot be a PO BOX")+": "+clinic.Description);
			}
			if(clinic.City=="") {
				throw new ODException(Lans.g("Erx","Clinic city blank")+": "+clinic.Description);
			}
			if(!USlocales.IsValidAbbr(clinic.State)) {
				throw new ODException(Lans.g("Erx","Clinic state abbreviation invalid")+": "+clinic.Description);
			}
			string clinicZip=Regex.Replace(clinic.Zip,"[^0-9]*","");//Zip with all non-numeric characters removed.
			if(clinicZip.Length!=9) {
				throw new ODException(Lans.g("Erx","Clinic zip must be 9 digits")+": "+clinic.Description);
			}
		}

		///<summary>Throws exception if anything about the provider information is not valid.
		///All intended exceptions are Exceptions and are already translated.</summary>
		public static void ValidateProv(Provider provider,Clinic clinic=null) {
			if(provider==null) {
				throw new ODException(Lans.g("Erx","Provider not found"));
			}
			ProviderClinic providerClinic=ProviderClinics.GetOneOrDefault(provider.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
			if(provider.IsErxEnabled==ErxEnabledStatus.Disabled) {
				throw new ODException(Lans.g("Erx","Erx is disabled for provider")+": "+provider.Abbr+".  "+Lans.g("Erx","To enable, edit provider in Lists | Providers and acknowledge Electronic Prescription fees."));
			}
			if(provider.IsHidden) {
				throw new ODException(Lans.g("Erx","Provider")+": "+provider.Abbr+" "+Lans.g("Erx","is hidden")+".  "+Lans.g("Erx","Unhide the provider to use Erx features."));
			}
			if(provider.IsNotPerson) {
				throw new ODException(Lans.g("Erx","Provider must be a person")+": "+provider.Abbr);
			}
			string fName=provider.FName.Trim();
			if(fName=="") {
				throw new ODException(Lans.g("Erx","Provider first name missing")+": "+provider.Abbr);
			}
			if(Regex.Replace(fName,"[^A-Za-z'\\- ]*","")!=fName) {
				throw new ODException(Lans.g("Erx","Provider first name can only contain letters, dashes, apostrophes, or spaces.")+": "+provider.Abbr);
			}
			string lName=provider.LName.Trim();
			if(lName=="") {
				throw new ODException(Lans.g("Erx","Provider last name missing")+": "+provider.Abbr);
			}
			if(Regex.Replace(lName,"[^A-Za-z'\\- ]*","")!=lName) { //Will catch situations such as "Dale Jr. III" and "Ross DMD".
				throw new ODException(Lans.g("Erx","Provider last name can only contain letters, dashes, apostrophes, or spaces.  Use the suffix box for I, II, III, Jr, or Sr")+": "+provider.Abbr);
			}
			//prov.Suffix is not validated here. In ErxXml.cs, the suffix is converted to the appropriate suffix enumeration value, or defaults to DDS if the suffix does not make sense.
			string deaNum=provider.DEANum;
			if(providerClinic!=null) {
				deaNum=providerClinic.DEANum;
			}
			if(deaNum.ToLower()!="none" && !Regex.IsMatch(deaNum,"^[A-Za-z]{2}[0-9]{7}$")) {
				throw new ODException(Lans.g("Erx","Provider DEA Number must be 2 letters followed by 7 digits.  If no DEA Number, enter NONE.")+": "+provider.Abbr);
			}
			string npi=Regex.Replace(provider.NationalProvID,"[^0-9]*","");//NPI with all non-numeric characters removed.
			if(npi.Length!=10) {
				throw new ODException(Lans.g("Erx","Provider NPI must be exactly 10 digits")+": "+provider.Abbr);
			}
			if(providerClinic==null || providerClinic.StateLicense=="") {
				throw new ODException(Lans.g("Erx","Provider state license missing")+": "+provider.Abbr);
			}
			if(providerClinic==null || !USlocales.IsValidAbbr(providerClinic.StateWhereLicensed)) {
				throw new ODException(Lans.g("Erx","Provider state where licensed invalid")+": "+provider.Abbr);
			}
		}

		///<summary>Throws exception if anything about the patient is not valid.
		///All intended exceptions are ODExceptions and are already translated.</summary>
		public static void ValidatePat(Patient patient) {
			if(patient==null) {
				throw new Exception(Lans.g("Erx","Patient not found."));
			}
			if(patient.Birthdate.Year<1880) {
				throw new ODException(Lans.g("Erx","Patient birthdate missing."));
			}
			if(patient.State!="" && !USlocales.IsValidAbbr(patient.State)) {
				throw new ODException(Lans.g("Erx","Patient state abbreviation invalid"));
			}
			if(patient.Zip!="" && !Regex.IsMatch(patient.Zip,@"^[0-9]{5}\-?([0-9]{4})?$")) {//Blank, or #####, or #####-####, or #########
				throw new ODException(Lans.g("Erx","Patient zip invalid"));
			}
		}

		///<summary>An employee user is assumed to be a proxy user in DoseSpot/Legacy. Returns true if a.) No prov is associated to user or b.) provider is associated but it is a secondary provider with blank NPI.</summary>
		public static bool IsUserAnEmployee(Userod userod) {
			bool isEmployee=false;
			if(userod.EmployeeNum==0) {//The current user does not have an employee associated.
				isEmployee=false;
			}
			else if(userod.ProvNum==0) {//The current user has an employee associated and no provider associated.
				isEmployee=true;
			}
			else {//Both an employee and provider are associated to the current user.
				Provider provider=Providers.GetProv(userod.ProvNum);
				if(provider.IsSecondary && provider.NationalProvID=="") {
					isEmployee=true;
				}
			}
			return isEmployee;
		}

		///<summary>Returns true if the passed in erxGuid is in a format that is consistent with DoseSpot.</summary>
		public static bool IsFromDoseSpot(string erxGuid) {
			int val=0;
			int.TryParse(erxGuid,out val);
			return !IsManualRx(erxGuid) && !IsTwoWayIntegrated(erxGuid) && val>0;
		}

		///<summary>Returns true if the passed in erxGuid is in a format that is consistent with NewCrop.</summary>
		public static bool IsFromNewCrop(string erxGuid) {
			Guid val=Guid.Empty;
			Guid.TryParse(erxGuid,out val);
			return !IsManualRx(erxGuid) && !IsTwoWayIntegrated(erxGuid) && val!=Guid.Empty;
		}

		///<summary>Returns true if the passed in erxGuid is in a format that is consistent with a third party eRx solution (ie. NewCrop/DoseSpot)</summary>
		public static bool IsFromErx(string erxGuid) {
			return IsFromDoseSpot(erxGuid) || IsFromNewCrop(erxGuid);
		}

		///<summary>Returns true if the passed in erxGuid is in a format that is consistent with Open Dental</summary>
		public static bool IsManualRx(string erxGuid) {
			return string.IsNullOrWhiteSpace(erxGuid);
		}

		///<summary>Returns true if the passed in erxGuid is in a format that is consistent with having sent the eRx to the third party solution</summary>
		public static bool IsTwoWayIntegrated(string erxGuid) {
			return erxGuid.StartsWith(OpenDentalErxPrefix);
		}

		///<summary>Returns true if the passed in erxGuid is in a format that is consistent with having retreived the eRx from DoseSpot as a Patient Recorded medication.</summary>
		public static bool IsDoseSpotPatReported(string erxGuid) {
			return erxGuid.StartsWith(DoseSpotPatReportedPrefix);
		}

		public static bool IsNpiValid(string npi) {
			npi=Regex.Replace(npi,"[^0-9]*","");//NPI with all non-numeric characters removed.
			if(npi.Length!=10) {
				return false;
			}
			return true;
		}

		public static string GetRxDoseSpotUrl(string stringSSOQuery) {
			string doseSpotUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.DoseSpotSingleSignOnURL,"https://my.dosespot.com/LoginSingleSignOn.aspx");
			if(ODBuild.IsDebug()) {
				doseSpotUrl="https://my.staging.dosespot.com/LoginSingleSignOn.aspx?b=2";
			}
			//Don't use the post data, just tack on query string to the URI and do a normal navigate.
			if(!doseSpotUrl.Contains("?") && stringSSOQuery[0]=='&') {
				stringSSOQuery="?"+stringSSOQuery.Substring(1);
			}
			return doseSpotUrl+stringSSOQuery;
		}

		public static string GetLegacyUrl() {
			string newCropUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.NewCropRxEntryURL,"https://secure.newcropaccounts.com/interfacev7/rxentry.aspx");
			if(ODBuild.IsDebug()) {
				newCropUrl="https://preproduction.newcropaccounts.com/interfaceV7/rxentry.aspx";
			}
			return newCropUrl;
		}

		///<summary>Sends the provider information to NewCrop without allowing the provider to use NewCrop.</summary>
		public static void ComposeNewRxNewCropWebRequest(byte[] postDataBytes) {
			string newCropUrl=GetLegacyUrl();
			WebRequest webRequest=WebRequest.Create(newCropUrl);
			webRequest.Method="POST";
			webRequest.ContentType="application/x-www-form-urlencoded";
			webRequest.ContentLength=postDataBytes.Length;
			Stream stream=webRequest.GetRequestStream();
			stream.Write(postDataBytes,0,postDataBytes.Length);
			stream.Close();
			WebResponse webResponse=webRequest.GetResponse();
			webResponse.Close();
		}
	}

	///<summary>This Enumeration should match the enumeration in WebServiceCustomerUpdates.ProviderErx.
	///Any changes made to this enum need to also be changed there.</summary>
	public enum ErxOption {
		///<summary>0.</summary>
		NewCrop,
		///<summary>1.</summary>
		DoseSpot,
		///<summary>2.</summary>
		DoseSpotWithNewCrop,
	}

	///<summary>Used by Erx to determine if the provider or clinic has been enabled at ODHQ.
	///Enum is in this file because it belongs to multiple tables.
	///This enum is copied to WebServiceCustomerUpdates/ClinicErx.cs.  See there for detailed comments.</summary>
	public enum ErxStatus {
		///<summary>0.</summary>
		Disabled,
		///<summary>1.</summary>
		Enabled,
		///<summary>2.</summary>
		Undefined,
		///<summary>3.</summary>
		PendingAccountId,
		///<summary>4.</summary>
		NeedsManualAccountId,
		///<summary>5.</summary>
		PendingEmail,
		///<summary>6.</summary>
		PendingPodio,
		///<summary>7.</summary>
		PendingEconnTransmit,
		///<summary>8.</summary>
		InTransitToEconn,
		///<summary>9.</summary>
		NeedsManualOfficeContact,
		///<summary>10.</summary>
		NeedsErxId,
	}

}
