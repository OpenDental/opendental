using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using WebServiceSerializer;
using CodeBase;
using System.IO;

namespace OpenDentBusiness {
	public static class WebServiceMainHQProxy {

		///<summary>Get an instance of the WebServicesHQ web service which includes the URL (pulled from PrefC). 
		///Optionally, you can provide the URL. This option should only be used by web apps which don't want to cause a call to PrefC.
		///Currently, OpenDentalREST and WebHostSynch hard-code the URL.</summary>
		public static WebServiceMainHQ.WebServiceMainHQ GetWebServiceMainHQInstance(string webServiceHqUrl="") {
			WebServiceMainHQ.WebServiceMainHQ service=new WebServiceMainHQ.WebServiceMainHQ();
#if ALPHA
			//Most beta applications should still set PrefName.WebServiceHQServerURL to the alpha address. This just makes it more fool proof for Alpha compiled applications.
			service.Url="http://10.10.1.184:49999/alpha/opendentalwebservicehq/webservicemainhq.asmx";
			return service;
#endif
			if(string.IsNullOrEmpty(webServiceHqUrl)) { //Default to the production URL.				
				service.Url=PrefC.GetString(PrefName.WebServiceHQServerURL);
			}
			else { //URL was provided so use that.
				service.Url=webServiceHqUrl;
			}
#if DEBUG
			//Change arguments for debug only.
			//service.Url="http://localhost/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx";//localhost
			service.Url="http://chris/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx"; //Chris has most graciously volunteered to host the test serviceshq web service and db.
			//service.Url="http://10.10.2.18:55018/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx";//Sam's Computer
			//service.Url="http://server184:49999/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx";//The actual server hosting WebServiceMainHQ.
			//service.Url="http://sam/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx"; //Sams computer
			service.Timeout=(int)TimeSpan.FromMinutes(60).TotalMilliseconds;
#endif
			return service;
		}
		
		///<summary>Any calls to WebServiceMainHQ must go through this method. The payload created here will be digested and extracted to OpenDentalWebServiceHQ.PayloadArgs.
		///If any args are null or not provided then they will be retrieved from Prefs.</summary>
		///<param name="registrationKey">An Open Dental distributed registration key that HQ has on file.  Do not include hyphens.</param>
		///<param name="practiceTitle">Any string is acceptable.</param>
		///<param name="practicePhone">Any string is acceptable.</param>
		///<param name="programVersion">Typically major.minor.build.revision.  E.g. 12.4.58.0</param>
		///<param name="payloadContentxAsXml">Use CreateXmlWriterSettings(true) to create your payload xml. Outer-most xml element MUST be labeled 'Payload'.</param>
		///<param name="serviceCode">Used on case by case basis to validate that customer is registered for the given service.</param>
		///<returns>An XML string that can be passed into a WebServiceHQ web method.</returns>
		public static string CreateWebServiceHQPayload(
			string payloadContentxAsXml,eServiceCode serviceCode,string registrationKey=null,string practiceTitle=null,string practicePhone=null,string programVersion=null) 
		{
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,CreateXmlWriterSettings(false))) {
				writer.WriteStartElement("Request");
				writer.WriteStartElement("Credentials");
				writer.WriteStartElement("RegistrationKey");
				writer.WriteString(registrationKey??PrefC.GetString(PrefName.RegistrationKey));
				writer.WriteEndElement();
				writer.WriteStartElement("PracticeTitle");
				writer.WriteString(practiceTitle??PrefC.GetString(PrefName.PracticeTitle));
				writer.WriteEndElement();
				writer.WriteStartElement("PracticePhone");
				writer.WriteString(practicePhone??PrefC.GetString(PrefName.PracticePhone));
				writer.WriteEndElement();
				writer.WriteStartElement("ProgramVersion");
				writer.WriteString(programVersion??PrefC.GetString(PrefName.ProgramVersion));
				writer.WriteEndElement();
				writer.WriteStartElement("ServiceCode");
				writer.WriteString(serviceCode.ToString());
				writer.WriteEndElement();
				writer.WriteEndElement(); //Credentials
				writer.WriteRaw(payloadContentxAsXml);
				writer.WriteEndElement(); //Request
			}
			return strbuild.ToString();
		}

		///<summary>Creates an XML string for the payload of the provided content. Currently only useful if you have one thing to include in the payload.
		///</summary>
		public static string CreatePayloadContent<T>(T content,string tagName) {
			System.Xml.Serialization.XmlSerializer xmlListConfirmationRequestSerializer=new System.Xml.Serialization.XmlSerializer(typeof(T));
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,WebServiceMainHQProxy.CreateXmlWriterSettings(true))) {
				writer.WriteStartElement("Payload");
				writer.WriteStartElement(tagName);
				xmlListConfirmationRequestSerializer.Serialize(writer,content);
				writer.WriteEndElement();	
				writer.WriteEndElement(); //Payload	
			}
			return strbuild.ToString();
		}

		public static T DeserializeOutput<T>(string resultXml,string tagName) {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(resultXml);
			//Validate output.
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				throw new Exception(node.InnerText);
			}
			node=doc.SelectSingleNode("//"+tagName);
			if(node==null) {
				throw new Exception("Output node not found: "+tagName);
			}
			T retVal;
			using(XmlReader reader=XmlReader.Create(new StringReader(node.InnerXml))) {
				XmlSerializer serializer=new XmlSerializer(typeof(T));
				retVal=(T)serializer.Deserialize(reader);
			}
			if(retVal==null) {
				throw new Exception("Output node invalid: "+tagName);
			}
			return retVal;
		}

		public static XmlWriterSettings CreateXmlWriterSettings(bool omitXmlDeclaration) {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = ("    ");
			settings.OmitXmlDeclaration=omitXmlDeclaration;
			return settings;
		}

		public static ListenerServiceType SetEConnectorOnOff(bool isListening) {
			return WebSerializer.DeserializePrimitiveOrThrow<ListenerServiceType>(
					GetWebServiceMainHQInstance().SetEConnectorType(WebSerializer.SerializePrimitive<string>(PrefC.GetString(PrefName.RegistrationKey)),isListening));
		}

		///<summary>Throws exceptions.</summary>
		public static void BuildWebSchedNewPatApptURLs(List<long> listClinicNums,Action<XmlNode> aNodeParsed) {
			//Make a web call to HQ to get the URLs for all the clinics.
			string response=GetWebServiceMainHQInstance()
					.BuildWebSchedNewPatApptURLs(PrefC.GetString(PrefName.RegistrationKey),String.Join("|",listClinicNums));
			//Parse Response
			XmlDocument doc=new XmlDocument();
			XmlNode nodeError=null;
			XmlNode nodeResponse=null;
			XmlNodeList nodeURLs=null;
			//Invalid web service response passed in.  Node will be null and will throw correctly.
			ODException.SwallowAnyException(() => {
				doc.LoadXml(response);
				nodeError=doc.SelectSingleNode("//Error");
				nodeResponse=doc.SelectSingleNode("//GetWebSchedURLsResponse");
			});
			#region Error Handling
			if(nodeError!=null||nodeResponse==null) {
				string error=Lans.g("WebSched","There was an error with the web request.  Please try again or give us a call.");
				//Either something went wrong or someone tried to get cute and use our Web Sched service when they weren't supposed to.
				if(nodeError!=null) {
					error+="\r\n"+Lans.g("WebSched","Error Details")+":\r\n"+nodeError.InnerText;
				}
				throw new Exception(error);
			}
			nodeURLs=doc.GetElementsByTagName("URL");
			if(nodeURLs==null) {
				throw new Exception("Invalid response from server recieved.");
			}
			#endregion
			//At this point we know we got a valid response from our web service.
			//Loop through all the URL nodes that were returned.
			foreach(XmlNode node in nodeURLs) {
				aNodeParsed(node);
			}
		}
		
		#region EService setup
		///<summary>Called by local practice db to query HQ for EService setup info. Must remain very lite and versionless. Will be used by signup portal.
		///If HasClinics==true then any SignupOut.EServices entries where ClinicNum==0 are invalid and should be ignored.
		///If HasClinics==false then SignupOut.EServices should only pay attention items where ClinicNum==0.
		///This list is kept completely unfiltered by ClinicNum for forward compatibility reasons. 
		///The ClinicNum 0 items are always used by the Signup portal to determine default signup preferences.
		///However, these items are only used for validation and billing in the case where HasClinics==true.</summary>
		public static EServiceSetup.SignupOut GetEServiceSetupFull(
			SignupPortalPermission permission,
			long clinicNum,
			string programVersionStr,
			List<EServiceSetup.SignupIn.ClinicLiteIn> clinics) {
			return ReadXml<EServiceSetup.SignupOut>
				(
					WebSerializer.DeserializePrimitiveOrThrow<string>
					(
						GetWebServiceMainHQInstance().EServiceSetup
						(
							CreateWebServiceHQPayload
							(
								WriteXml(new EServiceSetup.SignupIn() {
									MethodNameInt=(int)EServiceSetup.SetupMethod.GetSignupOutFull,
									HasClinics=PrefC.HasClinicsEnabled,
									ClinicNum=clinicNum,
									ProgramVersionStr=programVersionStr,
									SignupPortalPermissionInt=(int)permission,
									Clinics=clinics
								}),eServiceCode.Undefined
							)
						)
					)
				);
		}

		///<summary>Called by local practice db to query HQ for EService setup info. Must remain very lite and versionless. Will be used by signup portal.
		///If any args are null or not provided then they will be retrieved from Prefs.</summary>
		public static EServiceSetup.SignupOut GetEServiceSetupLite(
			SignupPortalPermission permission,string registrationKey=null,string practiceTitle=null,string practicePhone=null,string programVersion=null) 
		{
			return ReadXml<EServiceSetup.SignupOut>
				(
					WebSerializer.DeserializePrimitiveOrThrow<string>
					(
						GetWebServiceMainHQInstance().EServiceSetup
						(
							CreateWebServiceHQPayload
							(
								WriteXml(new EServiceSetup.SignupIn() {
									MethodNameInt=(int)EServiceSetup.SetupMethod.GetEServiceSetupLite,
									SignupPortalPermissionInt=(int)permission,
								}),
								eServiceCode.Undefined,
								//null is allowed for the rest. Will get converted to RegKey Pref.
								registrationKey, 
								practiceTitle,
								practicePhone,
								programVersion
							)
						)
					)
				);
		}

		///<summary>Called by local practice db to query HQ for EService setup info. Must remain very lite and versionless. 
		///This method will determine if clinics are enabled and HQ will include ClinicNum 0 in the output without it first being included in the input.
		///If clinics not enabled and an emptly list is passed in, then this method will automatically validate against the practice clinic (ClinicNum 0).</summary>
		public static List<long> GetEServiceClinicsAllowed(List<long> listClinicNums,eServiceCode eService) {
			EServiceSetup.SignupOut signupOut=ReadXml<EServiceSetup.SignupOut>
			(
				WebSerializer.DeserializePrimitiveOrThrow<string>
				(
					GetWebServiceMainHQInstance().EServiceSetup
					(
						CreateWebServiceHQPayload
						(
							WriteXml(new EServiceSetup.SignupIn() {
								MethodNameInt=(int)EServiceSetup.SetupMethod.ValidateClinics,
								HasClinics=PrefC.HasClinicsEnabled,
								SignupPortalPermissionInt=(int)SignupPortalPermission.ReadOnly,
								Clinics=listClinicNums.Select(x => new EServiceSetup.SignupIn.ClinicLiteIn() { ClinicNum=x }).ToList(),
							}),
							eService
						)
					)
				)
			);
			//Include the "Practice" clinic if the calling method didn't specifically provide ClinicNum 0.
			if(!signupOut.HasClinics && !listClinicNums.Contains(0)) {
				listClinicNums.Add(0);
			}
			return signupOut.EServices
				//We only care about the input eService.
				.FindAll(x => x.EService==eService)
				//Must be included in the HQ output list in order to be considered valid.
				.FindAll(x => listClinicNums.Any(y => y == x.ClinicNum))
				.Select(x => x.ClinicNum).ToList();
		}

		///<summary>Makes web service call to HQ and validates if the given eService is allowed for the given clinic.
		///Set clinicNum==0 to check for a non-clinics practice. Returns true if allowed; otherwise returns false.
		///This is just a wrapper for GetEServiceClinicsAllowed().</summary>
		public static bool IsEServiceClinicAllowed(long clinicNum,eServiceCode eService) {
			return GetEServiceClinicsAllowed(new List<long> { clinicNum },eService).Any(x => x==clinicNum);
		}

		///<summary>Makes web service call to HQ and validates if the given eService is allowed for the practice.
		///True indicates that at least 1 clinic and/or the practice iteself is signed up.
		///Use IsEServiceClinicAllowed() or GetEServiceClinicsAllowed() if you care about specific clinics.
		///This is just a wrapper for GetEServiceClinicsAllowed().</summary>
		public static bool IsEServiceAllowed(eServiceCode eService) {
			return GetEServiceClinicsAllowed(Clinics.ListLong.Select(x => x.ClinicNum).ToList(),eService).Count>0;
		}

		///<summary>Used to send EServiceClinic(s) to and from HQ. Must remain very lite and versionless. Will be used by signup portal.</summary>
		public class EServiceSetup {
			///<summary>Input of WebServiceHQ.EServiceSetup() web method.</summary>
			public class SignupIn {
				///<summary>All clinics belonging to local db. Required for lite version? NO.</summary>
				public List<ClinicLiteIn> Clinics;
				///<summary>Try to convert this to SignupPortalPermission enum in OD. If not found then omit this list item. Required for all versions? YES.</summary>
				public int SignupPortalPermissionInt;
				///<summary>Local DB ClinicNum. Can be 0. Required for lite version? NO.</summary>
				public long ClinicNum;
				///<summary>Should be convertible to Version. Required for lite version? NO.</summary>
				public string ProgramVersionStr;
				///<summary>Indicates if this practice has the clinics feature turned on.</summary>
				public bool HasClinics=false;
				///<summary>Try to convert this to SetupMethod enum in OD. This flag will determine which method is called by WebServiceHQ. Required for all versions? YES.</summary>
				public int MethodNameInt;

				[XmlIgnore]
				public SetupMethod MethodName {
					get {
						if(Enum.IsDefined(typeof(SetupMethod),MethodNameInt)) {
							return (SetupMethod)MethodNameInt;
						}
						return SetupMethod.Undefined;
					}
				}

				[XmlIgnore]
				public SignupPortalPermission PortalPermission {
					get {
						if(Enum.IsDefined(typeof(SignupPortalPermission),SignupPortalPermissionInt)) {
							return (SignupPortalPermission)SignupPortalPermissionInt;
						}
						return SignupPortalPermission.Denied;
					}
				}
				
				[XmlIgnore]
				public Version ProgramVersion {
					get {
						Version ret;
						if(!Version.TryParse(ProgramVersionStr,out ret)) {
							ret=new Version(0,0);
						}
						return ret;
					}
				}

				///<summary>Lite version of HQ EServiceClinic table. Versionless so keep simple and do not remove or rename fields.</summary>
				public class ClinicLiteIn {
					public long ClinicNum;
					public string ClinicTitle;
					public bool IsHidden;
				}
			}

			///<summary>Output of WebServiceHQ.EServiceSetup() web method. Set SignupIn.IsLiteVersion=true for lite version.</summary>
			public class SignupOut {
				///<summary>What type of listener is being used by this office. Included in lite version? YES.</summary>
				public int ListenerTypeInt;
				///<summary>Clinic based description of EService signup status. 
				///Full version will included SignupOutSms for IntegratedTexting entries. 
				///Lite version includes only SignupOutEService objects, even for IntegratedTexting. Included in lite version? YES.</summary>
				public List<SignupOutEService> EServices;
				///<summary>Any phones owned by this reg key. Included in lite version? NO.</summary>
				public List<SignupOutPhone> Phones;
				///<summary>Navigates to signup portal using given SignupIn inputs. Included in lite version? NO.</summary>
				public string SignupPortalUrl;
				///<summary>Indicates if this practice has the clinics feature turned on.</summary>
				public bool HasClinics=false;
				///<summary>Try to convert this to SignupPortalPermission enum in OD. If not found then omit this list item. Required for lite version? NO.</summary>
				public int SignupPortalPermissionInt;
				///<summary>Try to convert this to SetupMethod enum in OD. This flag will determine which method is called by WebServiceHQ. Required for all versions? YES.</summary>
				public int MethodNameInt;

				[XmlIgnore]
				public SetupMethod MethodName {
					get {
						if(Enum.IsDefined(typeof(SetupMethod),MethodNameInt)) {
							return (SetupMethod)MethodNameInt;
						}
						return SetupMethod.Undefined;
					}
				}

				[XmlIgnore]
				public SignupPortalPermission PortalPermission {
					get {
						if(Enum.IsDefined(typeof(SignupPortalPermission),SignupPortalPermissionInt)) {
							return (SignupPortalPermission)SignupPortalPermissionInt;
						}
						return SignupPortalPermission.Denied;
					}
				}

				[XmlIgnore]
				public ListenerServiceType ListenerType {
					get {
						if(Enum.IsDefined(typeof(ListenerServiceType),ListenerTypeInt)) {
							return (ListenerServiceType)ListenerTypeInt;
						}
						return ListenerServiceType.DisabledByHQ;
					}
				}

				public class SignupOutSms:SignupOutEService {
					///<summary>Monthly amount spent on texting which this clinic does not want to exceed.</summary>
					public double MonthlySmsLimit;
					///<summary>The start date of the texting service.</summary>
					public DateTime SmsContractDate;
					///<summary>Country code linked to this clinic for the purpose of Integrated Texting. String version of ISO31661.</summary>
					public string CountryCode;
				}

				///<summary>Lite version of HQ EServiceSignup table. Versionless so keep simple and do not remove or rename fields.</summary>
				[XmlInclude(typeof(SignupOutSms))] //Allows sub-class to be serialized without custom serializer.
				public class SignupOutEService {
					public long ClinicNum;
					///<summary>Try to convert this to eServiceCode enum in OD. If not found then omit this list item.</summary>
					public int EServiceCodeInt;
					///<summary>URL used for this EService for this clinic. Only applies in some scenarios, otherwise empty.</summary>
					public string HostedUrl;
					///<summary>From HQ RepeatCharge and HQ EServiceSignup.</summary>
					public bool IsEnabled;

					[XmlIgnore]
					public eServiceCode EService {
						get {
							if(Enum.IsDefined(typeof(eServiceCode),EServiceCodeInt)) {
								return (eServiceCode)EServiceCodeInt;
							}
							return eServiceCode.Undefined;
						}
					}
				}

				///<summary>Lite version of SmsPhone table. Versionless so keep simple and do not remove or rename fields.</summary>
				public class SignupOutPhone {
					public long ClinicNum;
					public string PhoneNumber;
					public string CountryCode;
					public DateTime DateTimeActive;
					public DateTime DateTimeInactive;
					public string InactiveCode;
					public bool IsActivated;
				}				
			}

			///<summary>Order matters for serialization. Do not change order.</summary>
			public enum SetupMethod {
				Undefined,
				GetSignupOutFull,
				GetEServiceSetupLite,
				ValidateClinics,
			}
		}
		#endregion

		private static string GetNodeNameFromType(Type t) {
			return t.Name+"Xml";
		}

		public static T ReadXml<T>(string xml) where T : new() {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(xml);
			XmlNode node=doc.SelectSingleNode("//"+GetNodeNameFromType(typeof(T)));
			if(node==null) {
				throw new ApplicationException(GetNodeNameFromType(typeof(T))+" node not present.");
			}
			T ret=default(T); XmlSerializer serializer=new XmlSerializer(typeof(T));
			using(XmlReader reader = XmlReader.Create(new System.IO.StringReader(node.InnerXml))) {
				ret=(T)serializer.Deserialize(reader);
			}
			if(ret==null) {
				ret=new T();
			}
			return ret;
		}

		public static string WriteXml<T>(T input) {
			XmlSerializer serializer=new XmlSerializer(typeof(T)); StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer = XmlWriter.Create(strbuild,WebServiceMainHQProxy.CreateXmlWriterSettings(true))) {
				writer.WriteStartElement(GetNodeNameFromType(typeof(T)));
				serializer.Serialize(writer,input);
				writer.WriteEndElement();
			}
			return strbuild.ToString();
		}
	}


}
