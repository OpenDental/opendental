using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace OpenDentBusiness.Eclaims {
	public class EDS {

		public static string ErrorMessage="";
		private const string VALIDATE_ATTACHMENT_URL="https://web2.edsedi.com/api/validateattachment";
		private const string ATTACHMENTS_URL="https://web2.edsedi.com/api/attachments";

		///<summary></summary>
		public EDS() {

		}

		///<summary>Sends an X12 270 request and returns X12 271 response or an error message.</summary>
		public static string Benefits270(Clearinghouse clearinghouseClin,string x12message,out Etrans etransHtml) {//called from x270Controller. Clinic-level clearinghouse passed in.
			string retVal="";
			etransHtml=null;
			try {
				HttpWebRequest webReq;
				WebResponse webResponseXml;
				//Production URL.  For testing, set username to 'test' and password to 'test'.
				//When the username and password are both set to 'test', the X12 270 request will be ignored and just the transmission will be verified.
				webReq=(HttpWebRequest)WebRequest.Create("https://web2.edsedi.com/eds/Transmit_Request");
				webReq.KeepAlive=false;
				webReq.Method="POST";
				webReq.ContentType="text/xml";
				string postDataXml="<?xml version=\"1.0\" encoding=\"us-ascii\"?>"
					+"<content>"
						+"<header>"
							+"<userId>"+clearinghouseClin.LoginID+"</userId>"
							+"<pass>"+clearinghouseClin.Password+"</pass>"
							+"<process>transmitEligibility</process>"
							+"<version>1</version>"
						+"</header>"
						+"<body>"
							+"<type>EDI</type>"//Can only be EDI
							+"<data><![CDATA["+x12message.Replace("\r\n","").Replace("\n","")+"]]></data>"
							+"<returnType>EDI</returnType>"//Can be EDI, HTML, or EDI.HTML, but should mimic the above type
						+"</body>"
					+"</content>";
				ASCIIEncoding encoding=new ASCIIEncoding();
				byte[] arrayXmlBytes=encoding.GetBytes(postDataXml);
				Stream streamOut=webReq.GetRequestStream();
				streamOut.Write(arrayXmlBytes,0,arrayXmlBytes.Length);
				streamOut.Close();
				webResponseXml=webReq.GetResponse();
				//Process the response
				StreamReader readStream=new StreamReader(webResponseXml.GetResponseStream(),Encoding.ASCII);
				string responseXml=readStream.ReadToEnd();
				readStream.Close();
				XmlDocument xmlDoc=new XmlDocument();
				xmlDoc.LoadXml(responseXml);
				XmlNode nodeErrorCode=xmlDoc.SelectSingleNode(@"content/body/ERROR_CODE");
				if(nodeErrorCode!=null && nodeErrorCode.InnerText.ToString()!="0") {
					throw new Exception("Error Code: "+nodeErrorCode.InnerText+" - "+xmlDoc.SelectSingleNode(@"content/body/ERROR_MSG").InnerText.ToString());
				}
				nodeErrorCode=xmlDoc.SelectSingleNode(@"content/error/code");
				if(nodeErrorCode!=null && nodeErrorCode.InnerText != "0") {
					throw new Exception("Error Code: "+nodeErrorCode.InnerText+" - "+xmlDoc.SelectSingleNode(@"content/error/description").InnerText);
				}
				string htmlMessage=xmlDoc.SelectSingleNode(@"content/body/htmlData")?.InnerText.ToString();//can be null
				if(!string.IsNullOrEmpty(htmlMessage)) {
					etransHtml=Etranss.CreateEtrans(DateTime.Now,clearinghouseClin.HqClearinghouseNum,htmlMessage,Security.CurUser.UserNum);
					etransHtml.Etype=EtransType.HTML;
					Etranss.Insert(etransHtml);
				}
				retVal=xmlDoc.SelectSingleNode(@"content/body/ediData").InnerText.ToString();
			}
			catch(Exception e) {
				retVal=e.Message;
			}
			return retVal;
		}

		public static bool Launch(Clearinghouse clearinghouseClin,string x837message) {
			try {
				HttpWebRequest webReq;
				WebResponse webResponseXml;
				webReq=(HttpWebRequest)WebRequest.Create("https://web2.edsedi.com/eds/Transmit_Request");
				webReq.KeepAlive=false;
				webReq.Method="POST";
				webReq.ContentType="text/xml";
				string postDataXml="<?xml version=\"1.0\" encoding=\"us-ascii\"?>"
					+"<content>"
						+"<header>"
							+"<userId>"+clearinghouseClin.LoginID+"</userId>"
							+"<pass>"+clearinghouseClin.Password+"</pass>"
							+"<process>transmitClaim</process>"
							+"<version>2</version>"
						+"</header>"
						+"<body>"
							+"<type>EDI</type>"
							+"<data><![CDATA["+x837message.Replace("\r\n","").Replace("\n","")+"]]></data>"
							+"<returnType>XML</returnType>"
						+"</body>"
					+"</content>";
				ASCIIEncoding encoding=new ASCIIEncoding();
				byte[] arrayXmlBytes=encoding.GetBytes(postDataXml);
				Stream streamOut=webReq.GetRequestStream();
				streamOut.Write(arrayXmlBytes,0,arrayXmlBytes.Length);
				streamOut.Close();
				webResponseXml=webReq.GetResponse();
				//Process the response
				StreamReader readStream=new StreamReader(webResponseXml.GetResponseStream(),Encoding.ASCII);
				string responseXml=readStream.ReadToEnd();
				readStream.Close();
				XmlDocument xmlDoc=new XmlDocument();
				xmlDoc.LoadXml(responseXml);
				XmlNode nodeErrorCode=xmlDoc.SelectSingleNode(@"content/error");
				if(nodeErrorCode!=null) {
					ErrorMessage="Error Code: "+nodeErrorCode.SelectSingleNode("code").InnerText+" - "+nodeErrorCode.SelectSingleNode("description").InnerText;
					return false;
				}
			}
			catch(Exception e) {
					ErrorMessage=e.Message;
					return false;
			}
			return true;
		}

		///<summary>Attempts to retrieve an End of Day report from EDS.
		///No need to pass in a date as this web call will retrieve a 277 containing all data since last called.</summary>
		public static bool Retrieve277s(Clearinghouse clearinghouseClin,IODProgressExtended progress) {
			progress=progress??new ODProgressExtendedNull();
			progress.UpdateProgress(Lans.g(progress.LanThis,"Contacting web server and downloading reports"),"reports","17%",17);
			bool retVal=false;
			if(progress.IsPauseOrCancel()) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
				return false;
			}
			progress.UpdateProgress(Lans.g(progress.LanThis,"Downloading 277s"),"reports","33%",33);
			retVal=Retrieve277s(clearinghouseClin);
			if(retVal) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Retrieved 277s successfully."));
			}
			else {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Retrieving 277s was unsuccessful."));
			}
			return retVal;
		}

		///<summary>Attempts to retrieve an End of Day report from EDS.
		///No need to pass in a date as this web call, when clearinghouse.IsEraDownloadAllowed is enabled,
		///will retrieve an 835 containing all data since last called.</summary>
		public static bool Retrieve835s(Clearinghouse clearinghouseClin,IODProgressExtended progress) {
			if(clearinghouseClin.IsEraDownloadAllowed==EraBehaviors.None) {
					return true;
			}
			progress=progress??new ODProgressExtendedNull();
			progress.UpdateProgress(Lans.g(progress.LanThis,"Contacting web server and downloading reports"),"reports","40%",40);
			if(progress.IsPauseOrCancel()) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
				return false;
			}
			progress.UpdateProgress(Lans.g(progress.LanThis,"Downloading ERAs"),"reports","50%",50);
			if(progress.IsPauseOrCancel()) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
				return false;
			}
			bool retVal=Retrieve835s(clearinghouseClin);
			if(retVal) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Retrieved 835s successfully."));
			}
			else {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Retrieving 835s was unsuccessful."));
			}
			return retVal;
		}
		
		public static bool Retrieve277s(Clearinghouse clearinghouseClin) {
			try {
				HttpWebRequest webReq;
				WebResponse webResponseXml;
				webReq=(HttpWebRequest)WebRequest.Create("https://web2.edsedi.com/eds/Transmit_Request");
				webReq.KeepAlive=false;
				webReq.Method="POST";
				webReq.ContentType="text/xml";
				string postDataXml="<?xml version=\"1.0\" encoding=\"us-ascii\"?>"
					+"<content>"
						+"<header>"
							+"<userId>"+clearinghouseClin.LoginID+"</userId>"
							+"<pass>"+clearinghouseClin.Password+"</pass>"
							+"<process>requestEndofDay</process>"
							+"<version>3</version>"
						+"</header>"
						+"<body>"
							+"<responseType>277</responseType>"
						+"</body>"
					+"</content>";
				ASCIIEncoding encoding=new ASCIIEncoding();
				byte[] arrayXmlBytes=encoding.GetBytes(postDataXml);
				Stream streamOut=webReq.GetRequestStream();
				streamOut.Write(arrayXmlBytes,0,arrayXmlBytes.Length);
				streamOut.Close();
				webResponseXml=webReq.GetResponse();
				//Process the response
				StreamReader readStream=new StreamReader(webResponseXml.GetResponseStream(),Encoding.ASCII);
				string responseXml=readStream.ReadToEnd();
				readStream.Close();
				XmlDocument xmlDoc=new XmlDocument();
				xmlDoc.LoadXml(responseXml);
				XmlNode nodeErrorCode=xmlDoc.SelectSingleNode(@"content/error");
				if(nodeErrorCode!=null) {
					ErrorMessage="Error Code: "+nodeErrorCode.SelectSingleNode("code").InnerText+" - "+nodeErrorCode.SelectSingleNode("description").InnerText;
					return false;
				}
				XmlNode nodeResponseFile=xmlDoc.SelectSingleNode(@"content/body/responseData");
				string exportFilePath=CodeBase.ODFileUtils.CombinePaths(clearinghouseClin.ResponsePath,DateTime.Now.ToString("yyyyMMddhhmmss")+".txt");
				byte[] reportFileDataBytes=Encoding.UTF8.GetBytes(nodeResponseFile.InnerText);
				File.WriteAllBytes(exportFilePath,reportFileDataBytes);
			}
			catch(Exception e) {
					ErrorMessage=e.Message;
					return false;
			}
			return true;
		}
		
		public static bool Retrieve835s(Clearinghouse clearinghouseClin) {
			try {
				HttpWebRequest webReq;
				WebResponse webResponseXml;
				webReq=(HttpWebRequest)WebRequest.Create("https://web2.edsedi.com/eds/Transmit_Request");
				webReq.KeepAlive=false;
				webReq.Method="POST";
				webReq.ContentType="text/xml";
				string postDataXml="<?xml version=\"1.0\" encoding=\"us-ascii\"?>"
					+"<content>"
						+"<header>"
							+"<userId>"+clearinghouseClin.LoginID+"</userId>"
							+"<pass>"+clearinghouseClin.Password+"</pass>"
							+"<process>listRemits</process>"
							+"<version>2</version>"
						+"</header>"
						+"<body>"
							+"<eraBatchId></eraBatchId>"//"Leave blank for open or the eraBatchId to repull".
						+"</body>"
					+"</content>";
				ASCIIEncoding encoding=new ASCIIEncoding();
				byte[] arrayXmlBytes=encoding.GetBytes(postDataXml);
				Stream streamOut=webReq.GetRequestStream();
				streamOut.Write(arrayXmlBytes,0,arrayXmlBytes.Length);
				streamOut.Close();
				webResponseXml=webReq.GetResponse();
				//Process the response
				StreamReader readStream=new StreamReader(webResponseXml.GetResponseStream(),Encoding.ASCII);
				string responseXml=readStream.ReadToEnd();
				readStream.Close();
				XmlDocument xmlDoc=new XmlDocument();
				xmlDoc.LoadXml(responseXml);
				XmlNode nodeErrorCode=xmlDoc.SelectSingleNode(@"content/error");
				if(nodeErrorCode!=null) {
					ErrorMessage="Error Code: "+nodeErrorCode.SelectSingleNode("code").InnerText+" - "+nodeErrorCode.SelectSingleNode("description").InnerText;
					return false;
				}
				string eraBatchId=xmlDoc.SelectSingleNode(@"content/body/eraBatchId").InnerText;
				string data835=xmlDoc.SelectSingleNode(@"content/body/eraData").InnerText;
				string exportFilePath=CodeBase.ODFileUtils.CombinePaths(clearinghouseClin.ResponsePath,DateTime.Now.ToString("yyyyMMddhhmmss")+"-"+eraBatchId+".txt");
				byte[] reportFileDataBytes=Encoding.UTF8.GetBytes(data835);
				File.WriteAllBytes(exportFilePath,reportFileDataBytes);
			}
			catch(Exception e) {
					ErrorMessage=e.Message;
					return false;
			}
			return true;
		}
	 
		///<summary>Upserts electids returned by calling EDS' payer list web service. Returns an empty string on success. Otherwise, returns an error string.</summary>
		public static string GetPayerList() {
			string strResponse="";
			XmlDocument xmlDoc=new XmlDocument();
			XmlNodeList xmlNodeList=null;
			try {
				xmlDoc.Load("https://web2.edsedi.com/eds/List_Payers");
				XmlNode nodeErrorCode=xmlDoc.SelectSingleNode(@"content/error");
				if(nodeErrorCode!=null) {
					strResponse="Error Code: "+nodeErrorCode.SelectSingleNode("code").InnerText+" - "+nodeErrorCode.SelectSingleNode("description").InnerText;
					return strResponse;
				}
				xmlNodeList=xmlDoc.SelectNodes("//payers/payerRecord");
			}
			catch(Exception e) {
				strResponse=e.Message;
				return strResponse;
			}
			List<IdNameAttributes> listIdNameAttributes=new List<IdNameAttributes>();
			for(int i=0;i<xmlNodeList.Count;i++) {
				XmlNode xmlNodePayer=xmlNodeList.Item(i);
				IdNameAttributes idNameAttribute=new IdNameAttributes();
				idNameAttribute.ID=xmlNodePayer.SelectSingleNode("./id").InnerText;
				idNameAttribute.Name=xmlNodePayer.SelectSingleNode("./name").InnerText;
				idNameAttribute.Attributes=String.Join(",",GetAttributes(xmlNodePayer).Select(x => (int)x));
				listIdNameAttributes.Add(idNameAttribute);
			}
			ElectIDs.UpsertFromEDS(listIdNameAttributes);
			return strResponse;
		}

		///<summary>Takes a payer returned from EDS' payer list API method.
		///Determines the values of each Attribute attached to the payer, returning a list of EnumEDSPayerAttributes which are flagged as supported for the payer.</summary>
		public static List<EnumEDSPayerAttributes> GetAttributes(XmlNode xmlNodePayer) {
			List<EnumEDSPayerAttributes> listEDSPayerAttributes=new List<EnumEDSPayerAttributes>();
			if(xmlNodePayer is null) {
				return listEDSPayerAttributes;
			}
			string defaultClaimTP=xmlNodePayer.SelectSingleNode("./defaultClaimTP").InnerText;
			if(defaultClaimTP=="ELEC") {
				listEDSPayerAttributes.Add(EnumEDSPayerAttributes.DefaultClaimTP);
			}
			string realtimeClaimTP=xmlNodePayer.SelectSingleNode("./realtimeClaimTP").InnerText;
			if(realtimeClaimTP=="Y") {
				listEDSPayerAttributes.Add(EnumEDSPayerAttributes.RealtimeClaimTP);
			}
			string eligibilityTP=xmlNodePayer.SelectSingleNode("./eligibilityTP").InnerText;
			if(eligibilityTP=="Y") {
				listEDSPayerAttributes.Add(EnumEDSPayerAttributes.EligibilityTP);
			}
			string ERATP=xmlNodePayer.SelectSingleNode("./ERATP").InnerText;
			if(ERATP=="Y") {
				listEDSPayerAttributes.Add(EnumEDSPayerAttributes.ERATP);
			}
			string claimEnrollment=xmlNodePayer.SelectSingleNode("./claimEnrollment").InnerText;
			if(claimEnrollment=="Y") {
				listEDSPayerAttributes.Add(EnumEDSPayerAttributes.ClaimEnrollment);
			}
			string eraEnrollment=xmlNodePayer.SelectSingleNode("./eraEnrollment").InnerText;
			if(eraEnrollment=="Y") {
				listEDSPayerAttributes.Add(EnumEDSPayerAttributes.ERAEnrollment);
			}
			string payerType=xmlNodePayer.SelectSingleNode("./payerType").InnerText;
			if(payerType=="D") {
				listEDSPayerAttributes.Add(EnumEDSPayerAttributes.PayerType);
			}
			return listEDSPayerAttributes;
		}

		///<summary>Throws exceptions. Returns a list of responses indicating whether or not EDS requires allows attachments for the carrier(s)/proccode(s) associated with this claim. This method is the first step of a three step process to add attachments to an EDS claim. Only if attachments are required for this claim are we allowed to proceed to step 2.</summary>
		public static ListPayerResponses ValidateClaim(Claim claim) {
			string strJson=CreateValidateAttachmentJSON(claim);
			string jsonReturn=MakeWebRequest(VALIDATE_ATTACHMENT_URL,strJson,claim.ClinicNum);
			return JsonConvert.DeserializeObject<ListPayerResponses>(jsonReturn);
		}

		/// <summary>Throws exceptions. Helper method that will make a call to EDS' API using the passed in endpoint and the JSON content. Returns the JSON response from the API on success.</summary>
		private static string MakeWebRequest(string url,string strJson, long clinicNum) {
			Clearinghouse clearinghouse=GetClearinghouse(clinicNum);
			string authenticationString=Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clearinghouse.LoginID}:{clearinghouse.Password}"));
			using WebClient webClient=new WebClient();
			webClient.Headers.Add("Authorization", $"Basic {authenticationString}");
			webClient.Headers[HttpRequestHeader.ContentType]="application/json";
			string response=webClient.UploadString(url,"POST",strJson);
			return response;
		}

		/// <summary>Throws exceptions. Returns a JSON blob containing all the necessary claim info to ask EDS if this claim requires attachments. </summary>
		private static string CreateValidateAttachmentJSON(Claim claim) {
			List<ProcedureCode> listProcedureCodes=ProcedureCodes.GetForClaim(claim.ClaimNum);
			List<Carrier> listCarriers=Carriers.GetForClaim(claim);
			PMSLocation pmsLocation=GetPMSLocation(claim);
			ListPayers listOfPayers=new ListPayers();
			listOfPayers.Payers=new List<Payer>();
			Payer payer;
			Clearinghouse clearingHouseEDS=GetClearinghouse(claim.ClinicNum);
			for(int i=0;i<listCarriers.Count;i++) {
				payer=new Payer();
				payer.ID=POut.Long(listCarriers[i].CarrierNum);
				payer.Date=DateTime.Now;
				payer.PayerId=listCarriers[i].ElectID;
				payer.LocationId=clearingHouseEDS.LocationID;
				payer.PMSLocation=pmsLocation;
				payer.ProcedureCodes=listProcedureCodes.Select(x => new Procedure{ProcId=x.ProcCode }).ToList();
				listOfPayers.Payers.Add(payer);
			}
			return JsonConvert.SerializeObject(listOfPayers,new JsonSerializerSettings {
				DateFormatString="MM/dd/yyyy"
			});
		}

		/// <summary>Returns a PMS location populated with information about the clinic that is associated with this claim. If the office is not using clinics, uses practice information.</summary>
		private static PMSLocation GetPMSLocation(Claim claim) {
			PMSLocation pmsLocation=new PMSLocation();
			if(PrefC.HasClinicsEnabled && claim.ClinicNum > 0) {
				Clinic clinic=Clinics.GetClinic(claim.ClinicNum);
				if(clinic!=null) {
					pmsLocation=CreatePMSLocation(clinic.ClinicNum,clinic.Abbr,clinic.Address,clinic.Address2,clinic.City,clinic.State,clinic.Zip,clinic.Phone);
				}
			}
			else {//Clinic 0
				pmsLocation=CreatePMSLocation(id:0,PrefC.GetString(PrefName.PracticeTitle),
					PrefC.GetString(PrefName.PracticeAddress),
					PrefC.GetString(PrefName.PracticeAddress2),
					PrefC.GetString(PrefName.PracticeCity),
					PrefC.GetString(PrefName.PracticeST),
					PrefC.GetString(PrefName.PracticeZip),
					PrefC.GetString(PrefName.PracticePhone));
			}
			return pmsLocation;
		}

		/// <summary>Creates a new PMSLocation based on the given parameters.</summary>
		private static PMSLocation CreatePMSLocation(long id,string name, string address, string address2, string city, string state, string zipcode, string phoneNumber) {
			PMSLocation pmsLocation=new PMSLocation();
			pmsLocation.ID=id;
			pmsLocation.Name=name;
			pmsLocation.Address1=address;
			pmsLocation.Address2=address2;
			pmsLocation.City=city;
			pmsLocation.State=state;
			pmsLocation.Zipcode=zipcode;
			pmsLocation.PhoneNumber=TelephoneNumbers.ReFormat(phoneNumber);
			return pmsLocation;
		}

		/// <summary>Throws exceptions. The second part of EDS' three part system for sending attachments. Returns the unique AttachmentID on EDS' side that will be used as a 'folder' to store attachments for the given claim. AttachmentIDs are only available if EDS requires attachments for this claim (see ValidateClaim).</summary>
		public static AttachmentIDResponse GetAttachmentID(Claim claim) {
			string strJson=CreateGetAttachmentIdJSON(claim);
			string jsonReturn=MakeWebRequest(ATTACHMENTS_URL,strJson,claim.ClinicNum);
			return JsonConvert.DeserializeObject<AttachmentIDResponse>(jsonReturn);
		}

		/// <summary>Throws an exception if unable to locate the clearinghouse associated with EDS. Otherwise returns the clearinghouse associated with EDS.</summary>
		private static Clearinghouse GetClearinghouse(long clinicNum) {
			Clearinghouse clearinghousehq=Clearinghouses.GetFirstOrDefault(x => x.CommBridge==EclaimsCommBridge.EDS && x.ClinicNum==0);
			if(clearinghousehq==null) {
				throw new ODException("Unable to locate EDS clearinghouse.");
			}
			Clearinghouse clearinghoustClin=Clearinghouses.OverrideFields(clearinghousehq,clinicNum);
			return clearinghoustClin;
		}
		
		/// <summary>Throws exceptions. Returns a JSON blob with all the info needed to request an attachmentid from EDS. </summary>
		private static string CreateGetAttachmentIdJSON(Claim claim) {
			Clearinghouse clearinghouse=GetClearinghouse(claim.ClinicNum);
			PMSLocation pmsLocation=GetPMSLocation(claim);
			PatPlan patPlan=PatPlans.GetPatPlansForPat(claim.PatNum).FirstOrDefault();
			InsSub insSub=null;
			if(patPlan!=null) {
				insSub=InsSubs.GetOne(patPlan.InsSubNum);
			}
			Provider providerBill=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvBill);
			Provider providerTreat=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvTreat);
			Patient patient=Patients.GetPat(claim.PatNum);
			Patient patientInsured=Patients.GetPat(insSub.Subscriber);
			Carrier carrier = Carriers.GetForClaim(claim).FirstOrDefault();
			string error=ValidateAttatchmentResources(patPlan,insSub,providerBill,providerTreat,patient,patientInsured,carrier);
			if(!string.IsNullOrEmpty(error)) {
				throw new ODException("Unable to retrieve attachment ID for claim:\r\n"+error);
			}
			AttachmentIDRequest attachmentIDRequest=new AttachmentIDRequest();
			attachmentIDRequest.ProviderEntityType=EDS.EnumProviderEntityType.Person;//Default the entity type to person
			attachmentIDRequest.VendorClaimId=claim.ClaimIdentifier;
			attachmentIDRequest.PayerId=carrier.ElectID;
			attachmentIDRequest.PayerName=carrier.CarrierName;
			attachmentIDRequest.BillingProviderTaxId=providerBill.SSN;
			attachmentIDRequest.BillingProviderNpi=providerBill.NationalProvID;
			attachmentIDRequest.BillingProviderLastName=providerBill.LName;
			attachmentIDRequest.BillingProviderFirstName=providerBill.FName;
			attachmentIDRequest.BillingProviderTaxonomyCode=X12Generator.GetTaxonomy(providerBill);
			attachmentIDRequest=SetBillingProviderAddress(attachmentIDRequest,claim);
			attachmentIDRequest.RenderingProviderNpi=providerTreat.NationalProvID;
			attachmentIDRequest.RenderingProviderLastName=providerTreat.LName;
			attachmentIDRequest.RenderingProviderFirstName=providerTreat.FName;
			attachmentIDRequest.PatientControlNumber=patient.PatNum.ToString();
			attachmentIDRequest.PatientLastName=patient.LName;
			attachmentIDRequest.PatientFirstName=patient.FName;
			attachmentIDRequest.PatientDob=patient.Birthdate;
			attachmentIDRequest.InsuredId=insSub.SubscriberID;
			attachmentIDRequest.InsuredFirstName=patientInsured.FName;
			attachmentIDRequest.InsuredLastName=patientInsured.LName;
			attachmentIDRequest.InsuredDob=patientInsured.Birthdate;
			attachmentIDRequest.ServiceDate=claim.DateService;
			attachmentIDRequest.TotalChargedAmount=claim.ClaimFee;
			attachmentIDRequest.LocationId=clearinghouse.LocationID;
			attachmentIDRequest.PMSLocation=pmsLocation;
			return JsonConvert.SerializeObject(attachmentIDRequest,new JsonSerializerSettings {
				DateFormatString="MM/dd/yyyy",
			});
		}

		/// <summary>Helper method to validate all the objects used to create an AttachmentID request. Returns error message(s) if any of the passed in arguments are invalid, otherwise returns an empty string.</summary>
		private static string ValidateAttatchmentResources(PatPlan patPlan,InsSub insSub,Provider providerBill,Provider providerTreat,Patient patient,Patient patientInsured,Carrier carrier) {
			StringBuilder stringBuilder=new StringBuilder();
			if(insSub==null || patientInsured==null || patPlan==null) {
				stringBuilder.AppendLine("Insurance subscriber not found");
			}
			if(providerBill==null) {
				stringBuilder.AppendLine("Billing provider not found");
			}
			else {//Valid provider so check EDS required fields
				if(!providerBill.UsingTIN) {
					stringBuilder.AppendLine("Billing provider Tax ID Number is required");
				}
				if(providerBill.SSN.Length!=9) {
					stringBuilder.AppendLine("Billing provider Tax ID Number must be 9 digits");
				}
				if(providerBill.NationalProvID.Length!=10) {
					stringBuilder.AppendLine("Billing provider National Provider ID must be 10 digits");
				}
			}
			if(providerTreat==null) {
				stringBuilder.AppendLine("Treating provider not found");
			}
			else {//Valid provider so check EDS required fields
				if(providerTreat.NationalProvID.Length!=10) {
					stringBuilder.AppendLine("Treating provider National Provider ID must be 10 digits");
				}
			}
			if(patient==null) {
				stringBuilder.AppendLine("Patient not found");
			}
			if(carrier==null) {
				stringBuilder.AppendLine("Insurance carrier not found");
			}
			return stringBuilder.ToString();
		}

		/// <summary>Sets the provider billing address. Throws exception if ZipCode is not 9 digits since EDS requires that format.</summary>
		private static AttachmentIDRequest SetBillingProviderAddress(AttachmentIDRequest attachmentIDRequest,Claim claim) {
			if(attachmentIDRequest==null) {
				return attachmentIDRequest;
			}
			string billingAddress;
			string billingCity;
			string billingState;
			string billingZip;
			Clinic clinic=Clinics.GetClinic(claim.ClinicNum);
			if(clinic==null || !PrefC.HasClinicsEnabled || claim.ClinicNum <= 0) {
				if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
					billingAddress=PrefC.GetString(PrefName.PracticeBillingAddress)+" "+PrefC.GetString(PrefName.PracticeBillingAddress2);
					billingCity=PrefC.GetString(PrefName.PracticeBillingCity);
					billingState=PrefC.GetString(PrefName.PracticeBillingST);
					billingZip=PrefC.GetString(PrefName.PracticeBillingZip);
				}
				else {
					billingAddress=PrefC.GetString(PrefName.PracticeAddress)+" "+PrefC.GetString(PrefName.PracticeAddress2);
					billingCity=PrefC.GetString(PrefName.PracticeCity);
					billingState=PrefC.GetString(PrefName.PracticeST);
					billingZip=PrefC.GetString(PrefName.PracticeZip);
				}
			}
			else {
				if(clinic.UseBillAddrOnClaims) {
					billingAddress=clinic.BillingAddress+" "+clinic.BillingAddress2;
					billingCity=clinic.BillingCity;
					billingState=clinic.BillingState;
					billingZip=clinic.BillingZip;
				}
				else {
					billingAddress=clinic.Address+" "+clinic.Address2;
					billingCity=clinic.City;
					billingState=clinic.State;
					billingZip=clinic.Zip;
				}
			}
			if(billingZip.Length!=9) {
				throw new ODException("Error: EDS requires the billing zipcode to be 9 digits.");
			}
			attachmentIDRequest.BillingProviderStreetAddress=billingAddress;
			attachmentIDRequest.BillingProviderCity=billingCity;
			attachmentIDRequest.BillingProviderState=billingState;
			attachmentIDRequest.BillingProviderZipCode=billingZip;
			return attachmentIDRequest;
		}

		///<summary>Throws exceptions. Saves the provided list of attachments to EDS and returns the response from EDS. This is the third and final step for sending attachments to EDS and can only be done when steps 1 & 2 are complete (ValidateClaim and GetAttachmentID).</summary>
		public static EDS.SaveAttachmentsResponse SaveAttachments(string attachmentId,List<ImageAttachment> listImageAttachments,long clinicNum) {
			string strJson=CreateSaveAttachmentsJSON(listImageAttachments);
			string jsonResponse=MakeWebRequest(ATTACHMENTS_URL+@$"/{attachmentId}/images",strJson,clinicNum);
			return JsonConvert.DeserializeObject<EDS.SaveAttachmentsResponse>(jsonResponse);
		}

		///<summary>Returns a JSON blob with all the needed info to save the given list of attachments to EDS.</summary>
		private static string CreateSaveAttachmentsJSON(List<ImageAttachment> listImageAttachments) {
			SaveAttachmentsRequest saveAttachmentsRequest=new SaveAttachmentsRequest();
			saveAttachmentsRequest.RequestType="JSON";
			saveAttachmentsRequest.EdsClaimId=null;
			saveAttachmentsRequest.ImageCount=listImageAttachments.Count;
			saveAttachmentsRequest.Images=listImageAttachments;
			return JsonConvert.SerializeObject(saveAttachmentsRequest,new JsonSerializerSettings {
				DateFormatString="MM/dd/yyyy",
			});
		}

		/// <summary>Represents the numeric value (1-16) that EDS correlates to different types of documents that can be attached to a claim.</summary>
		public enum EnumDocumentTypeCode {
			/// <summary>0</summary>
			[Description("Unknown")]
			Unknown=0,
			/// <summary>1</summary>
			[Description("EOB or COB")]
			EOB=1,
			/// <summary>2</summary>
			[Description("Narrative")]
			Narrative,
			/// <summary>3</summary>
			[Description("Student Verification")]
			StudentVerification,
			/// <summary>4</summary>
			[Description("Referral Form")]
			ReferralForm,
			/// <summary>5</summary>
			[Description("Diagnosis")]
			Diagnosis,
			/// <summary>6</summary>
			[Description("Reports")]
			Reports,
			/// <summary>7</summary>
			[Description("Periodontal Charts")]
			PeriodontalCharts,
			/// <summary>8</summary>
			[Description("Progress Notes")]
			ProgressNotes,
			/// <summary>9</summary>
			[Description("Intraoral Image")]
			IntraoralImage,
			/// <summary>10</summary>
			[Description("Pre/Post-op FMX")]
			FMX,
			/// <summary>11</summary>
			[Description("Bitewings")]
			Bitewings,
			/// <summary>12</summary>
			[Description("Pre/Post-op Periapical")]
			Periapical,
			/// <summary>13</summary>
			[Description("Pre/Post-op Panoramic Film")]
			PanoramicFilm,
			/// <summary>14</summary>
			[Description("Partial Mount")]
			PartialMount,
			/// <summary>15</summary>
			[Description("Cephalometric")]
			Cephalometric,
			/// <summary>16</summary>
			[Description("Radiographic Images")]
			XRay
		}

		/// <summary>The enitiy of a provider. Used when requesting an atachment id to save attachments to.</summary>
		public enum EnumProviderEntityType {
			///<summary>0</summary>
			[Description("Person")]
			Person,
			///<summary>1</summary>
			[Description("Group")]
			Group
		}


		/// <summary>Helpful container class used to store information about EDS attachments.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class ImageAttachment {
			[JsonIgnore]
			public string FileDisplayName;
			[JsonIgnore]
			public string FileNameActual;
			public EnumDocumentTypeCode DocumentTypeCode;
			public string OrientationCode;
			public int PageCount;
			public long FileSize;
			public DateTime FileDate;
			public byte[] FileData;
			public string FileExt;
			public string Narrative;

			///<summary>Creates a new ImageAttachment using the passed in parameters.</summary>
			public static ImageAttachment Create(string fileName,DateTime dateTimeCreated,EnumDocumentTypeCode documentTypeCode,Image imageClaim,string narrative,bool isRightOriented=false) {
				if(fileName.IsNullOrEmpty() ||
					dateTimeCreated==DateTime.MinValue ||
					imageClaim==null)
				{
					return new ImageAttachment();
				}
				ImageAttachment imageAttachment=new ImageAttachment();
				imageAttachment.FileDate=dateTimeCreated;
				imageAttachment.DocumentTypeCode=documentTypeCode;
				imageAttachment.FileData=ConvertImageToBytes(imageClaim);
				imageAttachment.PageCount=imageClaim.GetFrameCount(FrameDimension.Page);
				imageAttachment.FileSize=imageAttachment.FileData.LongLength;
				imageAttachment.FileExt=GetImageExtension(imageClaim);
				imageAttachment.Narrative=narrative;
				imageAttachment.FileDisplayName=fileName;
				if(isRightOriented) {
					imageAttachment.OrientationCode="Right";
				}
				else {
					imageAttachment.OrientationCode="Left";
				}		
				return imageAttachment;
			}

			/// <summary>Helper method to parse out the image attachment extension (e.g. .JPEG). If the extension can't be determined from the image.RawFormat, then the default is JPEG.</summary>
			private static string GetImageExtension(Image image) {
				ImageCodecInfo imageCodecInfo=ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID==image.RawFormat.Guid);
				if(imageCodecInfo==null) {//Typically only happens when dealing with in-memory bitmaps (e.g. snipped image) that haven't been saved to disk yet. Default to JPEG.
					imageCodecInfo=ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID==ImageFormat.Jpeg.Guid);
				}
				//FileNameExtension refers to all the file extensions that this image could have in the format *.{extension};*.{extension}; etc. We'll just use the first extension.
				string fileExtension=imageCodecInfo.FilenameExtension.Split(";",StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
				if(fileExtension==null) {
					return ".jpeg";//The FileExtension list did not have any entries. Not likely, but just incase default to .jpeg.
				}
				return fileExtension.TrimStart('*').ToLower();//Trim off the extra characters so we're left with just the extension itself.
			}

			///<summary>Takes an image and converts it to a base64 byte representation. EDS requires the image to be in this format when sending attachments.</summary>
			private static byte[] ConvertImageToBytes(Image image) {
				using MemoryStream memoryStream=new MemoryStream();
				ImageFormat imageFormat=image.RawFormat;
				ImageCodecInfo imageCodecInfo=ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID==image.RawFormat.Guid);
				if(imageCodecInfo==null) {//Typically only happens when dealing with in-memory bitmaps (e.g. snipped image) that haven't been saved to disk yet. Default to JPEG.
					imageFormat=ImageFormat.Jpeg;
				}
				using Bitmap bitmap=new Bitmap(image);
				bitmap.Save(memoryStream,imageFormat);
				return memoryStream.ToArray();
			}
		}

		///<summary>A container object that models the response EDS returns when we ask if a claim requires attachments.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class PayerResponse {
			public string ID;
			public bool ClaimLevelResponse;
			public List<ProcedureResponse> ProcedureResponses;
		}

		/// <summary>A procedure level response that will indicate if attachemnts are required or not for this procedure, and if so what type of attachemnts are needed. </summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class ProcedureResponse {
			public string ProcID;
			public List<ProcedureDocument> Documents;
			public bool Response;
			public string Comments;
		}

		/// <summary>A response from EDS that indicates the type of document required for this claim and the reason it is required. </summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class ProcedureDocument {
			[JsonConverter(typeof(StringEnumConverter))]
			public EnumDocumentTypeCode DocumentTypeCode;
			public string AttachmentReason;
		}

		/// <summary>An individual payer along with the practice information for the office and the procedurecodes being sent. Used when asking EDS if a claim requires attachments </summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class Payer {
			public string ID;
			public DateTime Date;
			public string PayerId;
			public string LocationId;
			public PMSLocation PMSLocation;
			public List<Procedure> ProcedureCodes;
		}

		/// <summary>The practice information for the location sending attachments. If clinics are turned on should reflect the clinic sending the claim, otherwise will reflect information for the practice.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class PMSLocation {
			public long ID;
			public string Name;
			public string Address1;
			public string Address2;
			public string City;
			public string State;
			public string Zipcode;
			public string PhoneNumber;
		}

		/// <summary>Container class to hold procedurecodes when asking EDS if a claim requires attachments.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class Procedure {
			public string ProcId;
		}

		/// <summary>Wraper class to ensure that the list of payers sent to EDS to ask if a claim requires attachments is named correctly in hte JSON body of the request.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class ListPayers {
			public List<Payer> Payers;
		}

		/// <summary>Wraper class to ensure that the response EDS returns can be deserialized correctly when we ask if a claim requires attachments.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class ListPayerResponses {
			public List<PayerResponse> Payers;
		}

		/// <summary>Container class to hold all the fields necessary to request an attachmentid from EDS </summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class AttachmentIDRequest {
			public string VendorClaimId;
			public string PayerId;
			public string PayerName;
			[JsonConverter(typeof(StringEnumConverter))]
			public EnumProviderEntityType ProviderEntityType;
			/// <summary>9 digits</summary>
			public string BillingProviderTaxId;
			/// <summary>10 digits</summary>
			public string BillingProviderNpi;
			public string BillingProviderLastName;
			public string BillingProviderFirstName;
			public string BillingProviderTaxonomyCode;
			public string BillingProviderStreetAddress;
			public string BillingProviderCity;
			public string BillingProviderState;
			/// <summary>9 digits</summary>
			public string BillingProviderZipCode;
			/// <summary>10 digits</summary>
			public string RenderingProviderNpi;
			public string RenderingProviderLastName;
			public string RenderingProviderFirstName;
			public string PatientControlNumber;
			public string PatientLastName;
			public string PatientFirstName;
			public DateTime PatientDob;
			public string InsuredId;
			public string InsuredLastName;
			public string InsuredFirstName;
			public DateTime InsuredDob;
			public DateTime ServiceDate;
			public double TotalChargedAmount;
			public string LocationId;
			public PMSLocation PMSLocation;
		}

		/// <summary>Container class to help with the deserialization of EDS' response when we request an attachmentid.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class AttachmentIDResponse {
			public string AttachmentID;
			public string Response;
		}

		/// <summary>Container class to hold all the information needed to save attachments to an attachmentid through EDS' attachment service.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class SaveAttachmentsRequest {
			public string RequestType;
			public string EdsClaimId;
			public int ImageCount;
			public List<ImageAttachment> Images;
		}

		/// <summary>Container class that represents EDS' response after image attachments are saved.</summary>
		[JsonObject(NamingStrategyType=typeof(CamelCaseNamingStrategy))]
		public class SaveAttachmentsResponse {
			public string AttachmentID;
			public int ImageCount;
			public string Response;
		}
	}
}
