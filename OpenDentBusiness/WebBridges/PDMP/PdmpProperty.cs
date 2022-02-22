using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>A collection of specific properties for PDMP links. Used to more easily pass information around</summary>
	public class PdmpProperty {
		#region PDMP properties
		public const string PdmpUserName="PDMP Username";
		public const string PdmpPassword="PDMP Password";
		public const string PdmpUrl="PDMP Url";
		public const string PdmpFacilityID="PDMP FacilityID";
		public const string PdmpProvLicenseField="PDMP Provider License Field";
		#endregion
		#region Appris properties
		public const string ApprissAuthToken="Appriss Client Key";
		public const string ApprissAuthPassword="Appriss Client Password";
		public const string ApprissUserName="Appriss Username";
		public const string ApprissPassword="Appriss Password";
		public const string ApprissUrl= "Appriss Url";
		public const string ApprissFacilityID="Appriss FacilityID";
		public string ApprissClientKey;
		public string ApprissClientPassword;
		#endregion
		public string FacilityId;
		public string Password;
		public string Username;
		public string ProvLicenseType;
		public string ProvLicenseNum;
		public string Url;
		public string Dea;
		public string StateAbbr;
		public Provider PdmpProv;
		public Patient PdmpPat;

		#region Methods used by both bridges 
		///<summary>Delegate function that determines necessary values for each state are set for provider and patient</summary>
		private delegate bool PropertyValidator(PdmpProperty prop,out string error);
		private Dictionary<string,List<PropertyValidator>> _dictStateRequirements=new Dictionary<string,List<PropertyValidator>>() {
			{ "CA", new List<PropertyValidator>{TryValidateDea,TryValidateProvName,TryValidatePatBirthday,TryValidateUrl } },
			{ "IL", new List<PropertyValidator>{TryValidateDea,TryValidateUrl,TryValidatePatBirthday } },
			{ "KY", new List<PropertyValidator>{TryValidateDea,TryValidateProvName,TryValidatePatSsn,TryValidatePatBirthday,TryValidateUrl } },
			{ "MD", new List<PropertyValidator>{TryValidateDea,TryValidateProvName,TryValidateProvLicense,TryValidatePatAddress,TryValidatePatBirthday ,TryValidateUrl} },
			{ "UT", new List<PropertyValidator>{TryValidateDea,TryValidateProvName,TryValidatePatBirthday,TryValidateUrl } },
			{ "WA", new List<PropertyValidator>{TryValidateProvName,TryValidateProvLicense,TryValidatePatBirthday,TryValidateUrl } }
		};

		///<summary>Uses current program and clinic num to get properties for whichever pdmp bridge user is connecting to.</summary>
		public void LoadPropertiesForProgram(Program programCur,long clinicNum) {
			//both programs need this set ahead of time
			ProvLicenseType=ProgramProperties.GetPropVal(programCur.ProgramNum,PdmpProvLicenseField);
			if(string.IsNullOrWhiteSpace(ProvLicenseType)) {
				ProvLicenseType=nameof(ProviderClinic.StateLicense);
			}
			bool useRxID=!string.IsNullOrWhiteSpace(ProvLicenseType) && ProvLicenseType==nameof(ProviderClinic.StateRxID);
			ProvLicenseNum=ProviderClinics.GetStateLicenseForProv(PdmpProv.ProvNum,StateAbbr,clinicNum,useRxID);
			if(programCur.ProgName==ProgramName.PDMP.ToString()) {
				LoadPDMPProps(programCur.ProgramNum);
			}
			else if(programCur.ProgName==ProgramName.Appriss.ToString()) {
				LoadApprissProps(programCur.ProgramNum);
			}
			if(Introspection.IsTestingMode) {
				LoadDebugOverrides(programCur);
			}
		}

		///<summary>Checks each PDMP program bridge and makes sure we have everything we need to initiate a connection with website. Can throw</summary>
		public void Validate(Program progCur) {
			string error="";
			if(PdmpPat is null) {
				error=Lans.g("PDMP","No patient selected.\r\n");
			}
			if(PdmpProv is null) {
				error+=Lans.g("PDMP","User is not associated with a valid provider.\r\n");
			}
			if(!string.IsNullOrWhiteSpace(error)) {
				//Something prior to this has gone horribly wrong since all of the above information should have already been validated.
				throw new ODException(error.ToString());
			}
			if(progCur.ProgName==ProgramName.PDMP.ToString()) {
				error=ValidatePDMP();
			}
			else if(progCur.ProgName==ProgramName.Appriss.ToString()) {
				error=ValidateAppriss();
			}
			if(!error.IsNullOrEmpty()) {
				throw new ODException(error);
			}
		}

		///<summary>Private validate method takes in a list of delegates and makes sure neccessary fields are set. If there is no issue with the properties being passed in then an empty string is returned</summary>
		private string Validate(List<PropertyValidator> listValidators) {
			StringBuilder err=new StringBuilder();
			foreach(PropertyValidator validator in listValidators) {
				if(!validator(this,out string propErr)) {
					err.AppendLine(propErr);
				}
			}
			return err.ToString();
		}

		private static bool TryValidateDea(PdmpProperty prop,out string error) {
			error="";
			StringBuilder deaErr=new StringBuilder();
			if(prop.Dea.IsNullOrEmpty()) {
				deaErr.AppendLine(Lans.g("PDMP","DEA number is required for provider."));
			}
			if(prop.Dea.Length!=9) {
				deaErr.AppendLine(Lans.g("PDMP","DEA number must be comprised of 9 alphanumeric characters."));
			}
			error=deaErr.ToString();
			return string.IsNullOrWhiteSpace(error);
		}

		private static bool TryValidateProvName(PdmpProperty prop, out string error) {
			error="";
			if(prop.PdmpProv.FName.IsNullOrEmpty() || prop.PdmpProv.LName.IsNullOrEmpty()) {
			 error=Lans.g("PDMP","Provider must have a first and last name.");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		private static bool TryValidateProvLicense(PdmpProperty prop, out string error) {
			error="";
			if(prop.ProvLicenseNum.IsNullOrEmpty()) {
				error=Lans.g("PDMP","Provider's license is required.");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		private static bool TryValidatePatSsn(PdmpProperty prop,out string error) {
			error="";
			if(prop.PdmpPat.SSN.IsNullOrEmpty()) {
				error=Lans.g("PDMP","Patient's social security number is required.");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		private static bool TryValidatePatAddress(PdmpProperty prop,out string error) {
			Patient pat=prop.PdmpPat;
			error="";
			if(pat.Address.IsNullOrEmpty() || pat.City.IsNullOrEmpty() || pat.State.IsNullOrEmpty()) {
				error=Lans.g("PDMP","Patient's full address is required");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		private static bool TryValidatePatBirthday(PdmpProperty prop,out string error) {
			error="";
			if(prop.PdmpPat.Birthdate.Year<=1880) {
				error=Lans.g("PDMP","Patient's birthday is invalid.");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		private static bool TryValidateUrl(PdmpProperty prop,out string error) {
			string url=prop.Url;
			error="";
			if(url.IsNullOrEmpty()) {
				error=Lans.g("PDMP","Program url was not set. Please contact support.");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		private static bool TryValidatePatZip(PdmpProperty prop, out string error) {
			error="";
			if(prop.PdmpPat.Zip.IsNullOrEmpty()) {
				error=Lans.g("PDMP","Patient's zipcode is invalid");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		public static bool TryValidateState(PdmpProperty prop,out string error) {
			error="";
			if(!Regex.IsMatch(prop.StateAbbr,
				"^(?:(A[KLRZ]|C[AOT]|D[CE]|FL|GA|HI|I[ADLN]|K[SY]|LA|M[ADEINOST]|N[CDEHJMVY]|O[HKR]|P[AR]|RI|S[CD]|T[NX]|UT|V[AIT]|W[AIVY]))$")) 
			{
				error=Lans.g("PDMP","State abbrieviation is invalid.");
			}
			return string.IsNullOrWhiteSpace(error);
		}
		#endregion

		#region Logicoy methods
		///<summary>Loads properties for Logicoy PDMP</summary>
		private void LoadPDMPProps(long progNum) {
			string stateFull=GetStateFull();
			string facilityIdPropDesc=GetPropDesc(stateFull,PdmpFacilityID);
			string userNamePropDesc=GetPropDesc(stateFull,PdmpUserName);
			string passwordPropDesc=GetPropDesc(stateFull,PdmpPassword);
			string urlPropDesc=GetPropDesc(stateFull,PdmpUrl);
			FacilityId=ProgramProperties.GetPropVal(progNum,facilityIdPropDesc);
			Username=ProgramProperties.GetPropVal(progNum,userNamePropDesc);
			Password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(progNum,passwordPropDesc));
			Url=ProgramProperties.GetPropVal(progNum,urlPropDesc);
		}

		///<summary>Logicoy requires different information from state to state and so extra validation needs to be made on its behalf. See documentation</summary>
		private string ValidatePDMP() {
			if(!_dictStateRequirements.TryGetValue(StateAbbr,out List<PropertyValidator> listValidators)) {
				return (Lans.g("PDMP","Invalid state abbreviation.\r\n"));
			}
			return Validate(listValidators);
		}

		///<summary>Logicoy has different credentials for each state it supports. Helper method that joins full state name and property description.</summary>
		private static string GetPropDesc(string state,string propDesc) {
			return state+" "+propDesc;
		}

		///<summary>Attempts to get full state name based on abbreviation. Returns true if successful.</summary>
		private string GetStateFull() {
			string stateFull="";
			switch(StateAbbr) {//keep alphabetical
				case "CA":
					stateFull="California";
					break;
				case "IL":
					stateFull="Illinois";
					break;
				case "KY":
					stateFull="Kentucky";
					break;
				case "MD":
					stateFull="Maryland";
					break;
				case "UT":
					stateFull="Utah";
					break;
				case "WA":
					stateFull="Washington";
					break;
				default:
					throw new ApplicationException(Lans.g("PDMP","Unable to parse full name of state from state abbreviation."));
			}
			return stateFull;
		}

		#endregion

		#region Appriss methods
		///<summary>Loads properties for Appriss PDMP</summary>
		private void LoadApprissProps(long progNum) {
			Username=ProgramProperties.GetPropVal(progNum,ApprissUserName);
			Password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(progNum,ApprissPassword));
			Url=ProgramProperties.GetPropVal(progNum,ApprissUrl);
			ApprissClientKey=ProgramProperties.GetPropVal(progNum,ApprissAuthToken);
			ApprissClientPassword=ProgramProperties.GetPropVal(progNum,ApprissAuthPassword);
			FacilityId=ProgramProperties.GetPropVal(progNum,ApprissFacilityID);
		}

		private bool TryValidateKey(PdmpProperty property,out string error) {
			error="";
			if(Introspection.IsTestingMode) {
				return true;
			}
			CDT.Class1.Decrypt(property.ApprissClientKey,out string decryptedKey);
			if(string.IsNullOrWhiteSpace(decryptedKey)) {
				error=Lans.g("PDMP","Security certificate was not set. Please contact support.");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		private bool TryValidatePassword(PdmpProperty property,out string error) {
			error="";
			if(Introspection.IsTestingMode) {
				return true;
			}
			CDT.Class1.Decrypt(property.ApprissClientPassword,out string decryptedPassword);
			if(string.IsNullOrWhiteSpace(decryptedPassword)) {
				error=Lans.g("PDMP","Security certificate password was not set. Please contact support.");
			}
			return string.IsNullOrWhiteSpace(error);
		}

		private string ValidateAppriss() {
			List<PropertyValidator> listApprissRequirements=new List<PropertyValidator>() {
				TryValidateDea,TryValidateProvName,TryValidateUrl,TryValidateProvLicense,TryValidatePatBirthday,TryValidatePatZip,TryValidateState,
				TryValidateKey,TryValidatePassword
			};
			return Validate(listApprissRequirements);
		}

		#endregion region

		#region Debug Load
		///<summary>If we're in debug mode, this will grab the property values we need via introspection</summary>
		private void LoadDebugOverrides(Program progCur) {
			if(progCur.ProgName==ProgramName.PDMP.ToString()) {
				switch(StateAbbr) {
					case "IL":
						Username=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUserIL,Username);
						Password=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestPasswordIL,Password);
						FacilityId=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestFacilityIdIL,FacilityId);
						Url=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUrlIL,Url);
						break;
					case "CA":
						Username=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUserCA,Username);
						Password=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestPasswordCA,Password);
						FacilityId=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestFacilityIdCA,FacilityId);
						Url=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUrlCA,Url);
						break;
					case "WA":
						Username=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUserWA,Username);
						Password=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestPasswordWA,Password);
						FacilityId=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestFacilityIdWA,FacilityId);
						Url=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUrlWA,Url);
						break;
					case "MD":
						Username=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUserMD,Username);
						Password=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestPasswordMD,Password);
						FacilityId=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestFacilityIdMD,FacilityId);
						Url=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUrlMD,Url);
						break;
					case "UT":
						Username=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUserUT,Username);
						Password=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestPasswordUT,Password);
						FacilityId=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestFacilityIdUT,FacilityId);
						Url=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUrlUT,Url);
						break;
					case "KY":
						Username=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUserKY,Username);
						Password=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestPasswordKY,Password);
						FacilityId=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestFacilityIdKY,FacilityId);
						Url=Introspection.GetOverride(Introspection.IntrospectionEntity.PDMPTestUrlKY,Url);
						break;
				}
			}
			if(progCur.ProgName==ProgramName.Appriss.ToString()) {
				Username=Introspection.GetOverride(Introspection.IntrospectionEntity.ApprissTestUser,Username);
				Password=Introspection.GetOverride(Introspection.IntrospectionEntity.ApprissTestPassword,Password);
				Url=Introspection.GetOverride(Introspection.IntrospectionEntity.ApprissTestUrl,Url);
			}
		}
		#endregion
	}
}
