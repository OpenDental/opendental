using CodeBase;
using System;
using System.Collections.Generic;
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
			Program progCur=Programs.GetCur(ProgramName.eRx);
			if(progCur==null) {
				throw new ODException(Lans.g("eRx","The eRx bridge is missing from the database."));
			}
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(progCur.ProgramNum);
			ProgramProperty propCur=listProgramProperties.FirstOrDefault(x => x.PropertyDesc==PropertyDescs.ErxOption);
			if(propCur==null) {
				throw new ODException(Lans.g("eRx","The eRx Option program property is missing from the database."));
			}
			return PIn.Enum<ErxOption>(propCur.PropertyValue);
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

		public static long InsertOrUpdateErxMedication(RxPat rxOld,RxPat rx,long rxCui,string strDrugName,string strGenericName,bool isProv,bool canInsertRx=true) {
			if(rxOld==null) {
				if(canInsertRx) {
					rx.IsNew=true;//Might not be necessary, but does not hurt.
					rx.IsErxOld=false;
					SecurityLogs.MakeLogEntry(Permissions.RxCreate,rx.PatNum,"eRx automatically created: "+rx.Drug);
					RxPats.Insert(rx);
				}
			}
			else {//The prescription was already in our database. Update it.
				rx.RxNum=rxOld.RxNum;
				//Preserve the pharmacy on the existing prescription, in case the user set the value manually.
				//We do not pull pharmacy back from eRx yet.
				rx.PharmacyNum=rxOld.PharmacyNum;
				if(rxOld.IsErxOld) {
					rx.IsErxOld=true;
					rx.SendStatus=RxSendStatus.SentElect;//To maintain backward compatibility.
				}
				RxPats.Update(rx);
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
			return MedicationPats.InsertOrUpdateMedOrderForRx(rx,rxCui,isProv,canInsertRx);
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
			if(Regex.IsMatch(PrefC.GetString(PrefName.PracticeAddress),".*P\\.?O\\.? .*",RegexOptions.IgnoreCase)) {
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
			if(Regex.IsMatch(clinic.Address,".*P\\.?O\\.? .*",RegexOptions.IgnoreCase)) {
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
		public static void ValidateProv(Provider prov,Clinic clinic=null) {
			if(prov==null) {
				throw new ODException(Lans.g("Erx","Provider not found"));
			}
			ProviderClinic provClinic=ProviderClinics.GetOneOrDefault(prov.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
			if(prov.IsErxEnabled==ErxEnabledStatus.Disabled) {
				throw new ODException(Lans.g("Erx","Erx is disabled for provider")+": "+prov.Abbr+".  "+Lans.g("Erx","To enable, edit provider in Lists | Providers and acknowledge Electronic Prescription fees."));
			}
			if(prov.IsHidden) {
				throw new ODException(Lans.g("Erx","Provider")+": "+prov.Abbr+" "+Lans.g("Erx","is hidden")+".  "+Lans.g("Erx","Unhide the provider to use Erx features."));
			}
			if(prov.IsNotPerson) {
				throw new ODException(Lans.g("Erx","Provider must be a person")+": "+prov.Abbr);
			}
			string fname=prov.FName.Trim();
			if(fname=="") {
				throw new ODException(Lans.g("Erx","Provider first name missing")+": "+prov.Abbr);
			}
			if(Regex.Replace(fname,"[^A-Za-z'\\- ]*","")!=fname) {
				throw new ODException(Lans.g("Erx","Provider first name can only contain letters, dashes, apostrophes, or spaces.")+": "+prov.Abbr);
			}
			string lname=prov.LName.Trim();
			if(lname=="") {
				throw new ODException(Lans.g("Erx","Provider last name missing")+": "+prov.Abbr);
			}
			if(Regex.Replace(lname,"[^A-Za-z'\\- ]*","")!=lname) { //Will catch situations such as "Dale Jr. III" and "Ross DMD".
				throw new ODException(Lans.g("Erx","Provider last name can only contain letters, dashes, apostrophes, or spaces.  Use the suffix box for I, II, III, Jr, or Sr")+": "+prov.Abbr);
			}
			//prov.Suffix is not validated here. In ErxXml.cs, the suffix is converted to the appropriate suffix enumeration value, or defaults to DDS if the suffix does not make sense.
			string deaNum=prov.DEANum;
			if(provClinic!=null) {
				deaNum=provClinic.DEANum;
			}
			if(deaNum.ToLower()!="none" && !Regex.IsMatch(deaNum,"^[A-Za-z]{2}[0-9]{7}$")) {
				throw new ODException(Lans.g("Erx","Provider DEA Number must be 2 letters followed by 7 digits.  If no DEA Number, enter NONE.")+": "+prov.Abbr);
			}
			string npi=Regex.Replace(prov.NationalProvID,"[^0-9]*","");//NPI with all non-numeric characters removed.
			if(npi.Length!=10) {
				throw new ODException(Lans.g("Erx","Provider NPI must be exactly 10 digits")+": "+prov.Abbr);
			}
			if(provClinic==null || provClinic.StateLicense=="") {
				throw new ODException(Lans.g("Erx","Provider state license missing")+": "+prov.Abbr);
			}
			if(provClinic==null || !USlocales.IsValidAbbr(provClinic.StateWhereLicensed)) {
				throw new ODException(Lans.g("Erx","Provider state where licensed invalid")+": "+prov.Abbr);
			}
		}

		///<summary>Throws exception if anything about the patient is not valid.
		///All intended exceptions are ODExceptions and are already translated.</summary>
		public static void ValidatePat(Patient pat) {
			if(pat==null) {
				throw new Exception(Lans.g("Erx","Patient not found."));
			}
			if(pat.Birthdate.Year<1880) {
				throw new ODException(Lans.g("Erx","Patient birthdate missing."));
			}
			if(pat.State!="" && !USlocales.IsValidAbbr(pat.State)) {
				throw new ODException(Lans.g("Erx","Patient state abbreviation invalid"));
			}
			if(pat.Zip!="" && !Regex.IsMatch(pat.Zip,@"^[0-9]{5}\-?([0-9]{4})?$")) {//Blank, or #####, or #####-####, or #########
				throw new ODException(Lans.g("Erx","Patient zip invalid"));
			}
		}

		///<summary>An employee user is assumed to be a proxy user in DoseSpot/Legacy. Returns true if a.) No prov is associated to user or b.) provider is associated but it is a secondary provider with blank NPI.</summary>
		public static bool IsUserAnEmployee(Userod user) {
			bool isEmp=false;
			if(user.EmployeeNum==0) {//The current user does not have an employee associated.
				isEmp=false;
			}
			else if(user.ProvNum==0) {//The current user has an employee associated and no provider associated.
				isEmp=true;
			}
			else {//Both an employee and provider are associated to the current user.
				Provider provUser=Providers.GetProv(user.ProvNum);
				if(provUser.IsSecondary && provUser.NationalProvID=="") {
					isEmp=true;
				}
			}
			return isEmp;
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
	}

	///<summary>This Enumeration should match the enumeration in WebServiceCustomerUpdates.ProviderErx.
	///Any changes made to this enum need to also be changed there.</summary>
	public enum ErxOption {
		///<summary>0.</summary>
		Legacy,
		///<summary>1.</summary>
		DoseSpot,
		///<summary>2.</summary>
		DoseSpotWithLegacy,
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
