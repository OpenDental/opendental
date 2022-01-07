using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CodeBase;
using WebServiceSerializer;

namespace OpenDentBusiness {
	public class WebServiceMainHQMockDemo : OpenDentBusiness.WebServiceMainHQ.WebServiceMainHQ, IWebServiceMainHQ {
		///<summary>Stores mock HQ eServiceSignup.IsEnabled for specific eServiceCode/ClinicNum.</summary>
		private static Dictionary<(eServiceCode code,long clinicNum),bool> _dictIsCodeEnabled=new Dictionary<(eServiceCode code,long clinicNum), bool>();
		public Func<string,string> EnableAdditionalFeaturesDelegate;
		public Func<string,string> LogCareCreditTransactionsDelegate;

		public WebServiceMainHQMockDemo() {
			List<long> listClinicNums=new List<long> { 0 };
			listClinicNums.AddRange(Clinics.GetDeepCopy().Select(x => x.ClinicNum));
			foreach(long clinicNum in listClinicNums) {
				//Add any eServiceCodes here that need to have a value other than defaulting to always IsEnabled=true.
				InitIsEnabled(clinicNum,eServiceCode.EmailMassUsage,PrefName.MassEmailStatus,HostedEmailStatus.SignedUp);
				InitIsEnabled(clinicNum,eServiceCode.EmailSecureAccess,PrefName.EmailSecureStatus,HostedEmailStatus.SignedUp);
			}
		}

		private void InitIsEnabled<T>(long clinicNum,eServiceCode code,PrefName prefName,T flagEnabled) where T : struct,Enum {
			if(!_dictIsCodeEnabled.TryGetValue((code,clinicNum),out _)) {
				//Use the current dental office preference is not already configured.
				_dictIsCodeEnabled[(code,clinicNum)]=GetPrefEnum<T>(prefName,clinicNum).HasFlag(flagEnabled);
			}
		}

		private T GetPrefEnum<T>(PrefName prefName,long clinicNum) where T : struct,Enum {
			if(clinicNum==0) {
				return PrefC.GetEnum<T>(prefName);
			}
			else {
				return (T)(object)ClinicPrefs.GetInt(prefName,clinicNum);
			}
		}

		public List<long> GetEServiceClinicsAllowed(List<long> listClinicNums,eServiceCode eService) {
			throw new NotImplementedException();
		}

		public new string EServiceSetup(string officeData) {
			try {
				WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=new WebServiceMainHQProxy.EServiceSetup.SignupOut() {
					EServices=GetEServicesForAll(),
					HasClinics=PrefC.HasClinicsEnabled,
					ListenerTypeInt=(int)ListenerServiceType.ListenerServiceProxy,
					MethodNameInt=(int)WebServiceMainHQProxy.EServiceSetup.SetupMethod.GetSignupOutFull,
					Phones=GetPhonesForAll(),
					Prompts=new List<string>(),
					SignupPortalPermissionInt=(int)SignupPortalPermission.FullPermission,
					SignupPortalUrl=GetHostedUrlForCode(eServiceCode.SignupPortal),
				};
				//Write the response out as a plain string. We will deserialize it on the other side.
				return WebSerializer.SerializePrimitive<string>(WebSerializer.WriteXml(signupOut));
			}
			catch(Exception ex) {
				StringBuilder strbuild=new StringBuilder();
				using(XmlWriter writer=XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(true))) {
					writer.WriteStartElement("Response");
					writer.WriteStartElement("Error");
					writer.WriteString(ex.Message);
					writer.WriteEndElement();
					writer.WriteEndElement();
				}
				return strbuild.ToString();
			}
		}

		///<summary>Returns all possible eServices for every clinic in the database.</summary>
		private List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService> GetEServicesForAll() {
			if(PrefC.HasClinicsEnabled) {
				List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService> listEServices
					=new List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>();
				List<long> listClinicNums=new List<long> { 0 };
				listClinicNums.AddRange(Clinics.GetDeepCopy(true).Select(x => x.ClinicNum));
				foreach(long clinicNum in listClinicNums) {
					listEServices.AddRange(GetEServicesForClinic(clinicNum));
				}
				return listEServices;
			}
			else {
				return GetEServicesForClinic(0);
			}
		}

		///<summary>Returns all possible eServices for the clinic passed in.</summary>
		private List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService> GetEServicesForClinic(long clinicNum=0) {
			return new List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>() {
				//GetEServiceForCode(eServiceCode.BugSubmission,clinicNum),
				GetEServiceForCode(eServiceCode.Bundle,clinicNum),
				GetEServiceForCode(eServiceCode.ConfirmationRequest,clinicNum),
				//GetEServiceForCode(eServiceCode.FeaturePortal,clinicNum),
				//GetEServiceForCode(eServiceCode.FHIR,clinicNum),
				//GetEServiceForCode(eServiceCode.HQManager,clinicNum),
				//GetEServiceForCode(eServiceCode.HQProxyService,clinicNum),

				//If you need to test texting signup, uncomment the following lines. Note: this will modify your db (clinics and texting prefs).
				//GetEServiceForCode(eServiceCode.IntegratedTexting,clinicNum),
				//GetEServiceForCode(eServiceCode.IntegratedTextingUsage,clinicNum),

				//GetEServiceForCode(eServiceCode.ListenerService,clinicNum),
				GetEServiceForCode(eServiceCode.MobileWeb,clinicNum),
				//GetEServiceForCode(eServiceCode.OAuth,clinicNum),
				GetEServiceForCode(eServiceCode.PatientPortal,clinicNum),
				GetEServiceForCode(eServiceCode.PatientPortalMakePayment,clinicNum),
				GetEServiceForCode(eServiceCode.PatientPortalViewStatement,clinicNum),
				//GetEServiceForCode(eServiceCode.ResellerPortal,clinicNum),
				//GetEServiceForCode(eServiceCode.ResellerSoftwareOnly,clinicNum),
				GetEServiceForCode(eServiceCode.SignupPortal,clinicNum),
				GetEServiceForCode(eServiceCode.SoftwareUpdate,clinicNum),
				//GetEServiceForCode(eServiceCode.Undefined,clinicNum),
				GetEServiceForCode(eServiceCode.WebForms,clinicNum),
				GetEServiceForCode(eServiceCode.EClipboard,clinicNum),
				GetEServiceForCode(eServiceCode.WebSched,clinicNum),
				GetEServiceForCode(eServiceCode.WebSchedASAP,clinicNum),
				GetEServiceForCode(eServiceCode.WebSchedNewPatAppt,clinicNum),
				GetEServiceForCode(eServiceCode.EmailMassUsage,clinicNum),
				GetEServiceForCode(eServiceCode.EmailSecureAccess,clinicNum),
			};
		}

		private WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService GetEServiceForCode(eServiceCode code,long clinicNum=0) {
			if(!_dictIsCodeEnabled.TryGetValue((code,clinicNum),out bool isEnabled)) {
				isEnabled=true;
			}
			if(code==eServiceCode.IntegratedTexting) {
				return new WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutSms() {
					ClinicNum=clinicNum,
					EServiceCodeInt=(int)code,
					HostedUrl=GetHostedUrlForCode(code),
					HostedUrlPayment="http://debug.hosted.url.payment",//TODO: no idea what to do here.
					IsEnabled=isEnabled,
					CountryCode="US",
					MonthlySmsLimit=20,
					SmsContractDate=DateTime.Today.AddYears(-1),
				};
			}
			return new WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService() {
				ClinicNum=clinicNum,
				EServiceCodeInt=(int)code,
				HostedUrl=GetHostedUrlForCode(code),
				HostedUrlPayment="http://debug.hosted.url.payment",//TODO: no idea what to do here.
				IsEnabled=isEnabled,
			};
		}

		private string GetHostedUrlForCode(eServiceCode code) {
			switch(code) {
				case(eServiceCode.MobileWeb):
					return "http://127.0.0.1:5000/MobileWeb.html";
				case (eServiceCode.PatientPortal):
				case (eServiceCode.PatientPortalMakePayment):
				case (eServiceCode.PatientPortalViewStatement):
					return "http://127.0.0.1:4000/PatientPortal.html";
				case (eServiceCode.SignupPortal):
					return "http://127.0.0.1:8888/SignupPortal.html";
				case (eServiceCode.WebForms):
					return "http://127.0.0.1:3000/WebForms.html";
				case (eServiceCode.WebSched):
				case (eServiceCode.WebSchedASAP):
				case (eServiceCode.WebSchedNewPatAppt):
					return "http://127.0.0.1:8000/WebSched.html";
				case (eServiceCode.EmailMassUsage):
					return "https://www.opendental.com/site/massemail.html";
				case (eServiceCode.EmailSecureAccess):
					return "https://www.opendental.com/site/massemail.html";//todo, update once implemented
				default:
					return "";
			}
		}

		public void SetEServiceCodeEnabled(eServiceCode code,long clinicNum,bool isEnabled) {
			_dictIsCodeEnabled[(code,clinicNum)]=isEnabled;
		}

		private List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutPhone> GetPhonesForAll() {
			//This will cause the local debug db not to sync the SmsPhone table. This just means that SmsPhone table can't be trusted in debug mode.
			//If you need to test "real" VLNs, just make a list here that includes known VLNs that are actually owned by HQ for debugging.
			return new List<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutPhone>();
		}

		public new string EmailHostingSignup(string officeData) {
			long clinicNum=WebSerializer.DeserializeTag<long>(officeData,"ClinicNum");
			bool doSignupMassEmail=WebSerializer.DeserializeTag<bool>(officeData,"DoSignupMassEmail");
			bool doSignupSecureEmail=WebSerializer.DeserializeTag<bool>(officeData,"DoSignupSecureEmail");
			_dictIsCodeEnabled[(eServiceCode.EmailMassUsage,clinicNum)]=doSignupMassEmail;
			_dictIsCodeEnabled[(eServiceCode.EmailSecureAccess,clinicNum)]=doSignupSecureEmail;
			return PayloadHelper.CreateSuccessResponse(new List<PayloadItem> {
				new PayloadItem("guid","AccountGUID"),
				new PayloadItem("secret","AccountSecret"),
			});
		}

		public new string EmailHostingChangeClinicStatus(string officeData) {
			return PayloadHelper.CreateSuccessResponse("Success","ChangeClinicStatusResponse");
		}

		public new string EnableAdditionalFeatures(string officeData) {
			if(EnableAdditionalFeaturesDelegate is null) {
				return base.EnableAdditionalFeatures(officeData);
			}
			return EnableAdditionalFeaturesDelegate(officeData);
		}

		public new string LogCareCreditTransaction(string officeData) {
			if(LogCareCreditTransactionsDelegate is null) {
				return base.EnableAdditionalFeatures(officeData);
			}
			return LogCareCreditTransactionsDelegate(officeData);

		}

    public new string GenerateShortGUIDs(string officeData) {
				try {
					long clinicNum=WebSerializer.DeserializeTag<long>(officeData,"ClinicNum");
					int numToGet=WebSerializer.DeserializeTag<int>(officeData,"NumberShortGUIDsToGet");
					int numForSms;
					try {
						numForSms=WebSerializer.DeserializeTag<int>(officeData,"NumberShortGUIDsForSMS");
					}
					catch(Exception ex) {//Older version that doesn't indicate how many are needed for SMS. This field will only be present in 19.1 and later.
						numForSms=numToGet;//Assume the worst case that all URLs need to be for SMS.
						ex.DoNothing();
					}
					int countCreatedForSms=0;
					List<WebServiceMainHQProxy.ShortGuidResult> listShortGuids=new List<WebServiceMainHQProxy.ShortGuidResult>();
					for(int i=0;i<numToGet;i++) {
						string shortGuid=$"fakeguid"+numToGet.ToString();
						bool isForSms=false;
						if(countCreatedForSms < numForSms) {
							isForSms=true;
							countCreatedForSms++;
						}
						listShortGuids.Add(new WebServiceMainHQProxy.ShortGuidResult {
							ShortGuid=shortGuid,
							ShortURL="http://shorturl.com/"+shortGuid,
							MediumURL="http://mediumurl.com/"+shortGuid,
							IsForSms=isForSms,
						});
					}
					return PayloadHelper.CreateSuccessResponse(listShortGuids,"ListShortGuidResults");
				}
				catch(Exception ex) {
					return PayloadHelper.CreateErrorResponse(ex,"Error occurred while attempting to Generate Short GUIDs.");
				}
    }

		public new string GetMobileSettings(string officeData) {
			throw new NotImplementedException();
		}

		public new string GetMobileSettings2FA(string officeData) {
			throw new NotImplementedException();
		}

		public new string UpsertMobileSettings(string officeData) {
			throw new NotImplementedException();
		}
	}
}
