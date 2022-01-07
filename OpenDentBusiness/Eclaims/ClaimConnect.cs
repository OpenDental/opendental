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
using CodeBase;
using System.Linq;
using System.Drawing;
using System.ComponentModel;

namespace OpenDentBusiness.Eclaims {
	/// <summary>
	/// aka ClaimConnect.
	/// </summary>
	public class ClaimConnect {
		///<summary></summary>
		public static string ErrorMessage="";

		public ClaimConnect() {

		}

		///<summary>Returns true if the communications were successful, and false if they failed. If they failed, a rollback will happen automatically by deleting the previously created X12 file. The batchnum is supplied for the possible rollback.</summary>
		public static bool Launch(Clearinghouse clearinghouseClin,int batchNum) {//called from Eclaims.cs. Clinic-level clearinghouse passed in.
			try {
				//Step 1: Post authentication request:
				Version myVersion=new Version(Application.ProductVersion);
				HttpWebRequest webReq;
				WebResponse response;
				StreamReader readStream;
				string str;
				string[] responseParams;
				string status="";
				string group="";
				string userid="";
				string authid="";
				string errormsg="";
				string alertmsg="";
				string curParam="";
				string serverName;
				if(ODBuild.IsDebug()) {
					//For testing
					serverName="https://prelive.dentalxchange.com/dci/upload.svl";
				}
				else {
					//Production
					serverName="https://claimconnect.dentalxchange.com/dci/upload.svl";
				}
				webReq=(HttpWebRequest)WebRequest.Create(serverName);
				string postData=
					"Function=Auth"//CONSTANT; signifies that this is an authentication request
					+"&Source=EDI"//CONSTANT; file format
					+"&Username="+HttpUtility.UrlEncode(clearinghouseClin.LoginID)
					+"&Password="+HttpUtility.UrlEncode(clearinghouseClin.Password)
					+"&UploaderName=OpenDental"//CONSTANT
					+"&UploaderVersion="+myVersion.Major.ToString()+"."+myVersion.Minor.ToString()+"."+myVersion.Build.ToString();//eg 12.3.24
				webReq.KeepAlive=false;
				webReq.Method="POST";
				webReq.ContentType="application/x-www-form-urlencoded";
				webReq.ContentLength=postData.Length;
				ASCIIEncoding encoding=new ASCIIEncoding();
				byte[] bytes=encoding.GetBytes(postData);
				Stream streamOut=webReq.GetRequestStream();
				streamOut.Write(bytes,0,bytes.Length);
				streamOut.Close();
				response=webReq.GetResponse();
				//Process the authentication response:
				readStream=new StreamReader(response.GetResponseStream(),Encoding.ASCII);
				str=readStream.ReadToEnd();
				readStream.Close();
				//MessageBox.Show(str);
				responseParams=str.Split('&');
				for(int i = 0;i<responseParams.Length;i++) {
					curParam=GetParam(responseParams[i]);
					switch(curParam) {
						case "Status":
							status=GetParamValue(responseParams[i]);
							break;
						case "GROUP":
							group=GetParamValue(responseParams[i]);
							break;
						case "UserID":
							userid=GetParamValue(responseParams[i]);
							break;
						case "AuthenticationID":
							authid=GetParamValue(responseParams[i]);
							break;
						case "ErrorMessage":
							errormsg=GetParamValue(responseParams[i]);
							break;
						case "AlertMessage":
							alertmsg=GetParamValue(responseParams[i]);
							break;
						default:
							throw new Exception("Unexpected parameter: "+curParam);
					}
				}
				//Process response for errors:
				if(alertmsg!="") {
					ErrorMessage=alertmsg;
				}
				switch(status) {
					case "0":
						//MessageBox.Show("Authentication successful.");
						break;
					case "1":
						//This status can also indicate an expired credit card on the users account.
						throw new Exception("Authentication failure.  Please verify your login ID and password by visiting\r\n"
							+"Setup | Family/Insurance | Clearinghouses | ClaimConnect.  "
							+"These values are probably the same as your dentalxchange log in.  Error message: "+errormsg);
					case "2":
						throw new Exception("Cannot authenticate at this time. "+errormsg);
					case "3":
						throw new Exception("Invalid authentication request. "+errormsg);
					case "4":
						throw new Exception("Invalid program version. "+errormsg);
					case "5":
						throw new Exception("No customer contract. "+errormsg);
				}
				//Step 2: Post upload request:
				string filePath = ODFileUtils.CombinePaths(clearinghouseClin.ExportPath, ODEnvironment.MachineName);
				string[] fileNames=Directory.GetFiles(filePath);
				if(fileNames.Length>1) {
					for(int f = 0;f<fileNames.Length;f++) {
						File.Delete(fileNames[f]);
					}
					Directory.Delete(filePath);
					throw new ApplicationException("A previous batch submission was found in an incomplete state.  You will need to resubmit your most recent batch as well as this batch.  Also check reports to be certain that all expected claims went through.");
				}
				string fileName=fileNames[0];
				string boundary="------------7d13e425b00d0";
				postData=
					"--"+boundary+"\r\n"
					+"Content-Disposition: form-data; name=\"Function\"\r\n"
					+"\r\n"
					+"Upload\r\n"
					+"--"+boundary+"\r\n"
					+"Content-Disposition: form-data; name=\"Source\"\r\n"
					+"\r\n"
					+"EDI\r\n"
					+"--"+boundary+"\r\n"
					+"Content-Disposition: form-data; name=\"AuthenticationID\"\r\n"
					+"\r\n"
					+authid+"\r\n"
					+"--"+boundary+"\r\n"
					+"Content-Disposition: form-data; name=\"File\"; filename=\""
						+fileName+"\"\r\n"
					+"Content-Type: text/plain\r\n"
					+"\r\n";
				using(StreamReader sr = new StreamReader(fileName)) {
					postData+=sr.ReadToEnd()+"\r\n"
						+"--"+boundary+"--";
				}
				//Debug.Write(postData);
				//MessageBox.Show(postData);
				webReq=(HttpWebRequest)WebRequest.Create(serverName);
				webReq.KeepAlive=false;
				webReq.Method="POST";
				webReq.ContentType="multipart/form-data; boundary="+boundary;
				webReq.ContentLength=postData.Length;
				bytes=encoding.GetBytes(postData);
				streamOut=webReq.GetRequestStream();
				streamOut.Write(bytes,0,bytes.Length);
				streamOut.Close();
				response=webReq.GetResponse();
				//Process the response
				readStream=new StreamReader(response.GetResponseStream(),Encoding.ASCII);
				str=readStream.ReadToEnd();
				readStream.Close();
				errormsg="";
				status="";
				str=str.Replace("\r\n","");
				Debug.Write(str);
				if(str.Length>300) {
					throw new Exception("Unknown lengthy error message received.");
				}
				responseParams=str.Split('&');
				for(int i = 0;i<responseParams.Length;i++) {
					curParam=GetParam(responseParams[i]);
					switch(curParam) {
						case "Status":
							status=GetParamValue(responseParams[i]);
							break;
						case "ErrorMessage":
							errormsg=GetParamValue(responseParams[i]);
							break;
						case "Filename":
						case "Timestamp":
							break;
						case ""://errorMessage blank
							break;
						default:
							throw new Exception("Unexpected parameter: "+curParam+"*");
					}
				}
				switch(status) {
					case "0":
						break;
					case "1":
						throw new Exception("Authentication failure.  Please verify your login ID and password by visiting\r\n"
							+"Setup | Family/Insurance | Clearinghouses | ClaimConnect.  "
							+"These values are probably the same as your dentalxchange log in.  Error message: "+errormsg);
					case "2":
						throw new Exception("Cannot upload at this time. "+errormsg);
				}
				//delete the uploaded claim and machine specific upload folder.
				File.Delete(fileName);
				Directory.Delete(filePath);
			}
			catch(Exception e) {
				ErrorMessage=e.Message;
				x837Controller.Rollback(clearinghouseClin,batchNum);
				return false;
			}
			return true;
		}

		private static string GetParam(string paramAndValue) {
			if(paramAndValue=="") {
				return "";
			}
			string[] pair=paramAndValue.Split('=');
			//if(pair.Length!=2){
			//	throw new Exception("Unexpected parameter from server: "+paramAndValue);
			return pair[0];
		}

		private static string GetParamValue(string paramAndValue) {
			if(paramAndValue=="") {
				return "";
			}
			string[] pair=paramAndValue.Split('=');
			//if(pair.Length!=2){
			//	throw new Exception("Unexpected parameter from server: "+paramAndValue);
			//}
			if(pair.Length==1) {
				return "";
			}
			return pair[1];
		}

		///<summary>Uses the DwsService() endpoint per DentalXChange's request.</summary>
		public static string Benefits270(Clearinghouse clearinghouseClin,string x12message) {
			Dentalxchange2016.Credentials cred=DxcCredentials.GetDentalxchangeCredentials(null,clearinghouseClin);//Null claim because we have a clearinghouse	
			cred.version=Application.ProductVersion;
			Dentalxchange2016.textRequest request=new Dentalxchange2016.textRequest();
			request.Content=HttpUtility.HtmlEncode(x12message);//get rid of ampersands, etc.
			Dentalxchange2016.DwsService service=new Dentalxchange2016.DwsService();
			if(ODBuild.IsDebug()) {
				//service.Url = "https://prelive2.dentalxchange.com/dws/DwsService"; // testing
				service.Url = "https://webservices.dentalxchange.com/dws/DwsService"; // production
			}
			else {
				service.Url = "https://webservices.dentalxchange.com/dws/DwsService"; //always use production. So I don't forget
			}
			string strResponse="";
			try {
				Dentalxchange2016.textResponse response = service.lookupEligibility(cred,request);
				if(response.Content==null) {
					strResponse="This customer is being denied service by Claim Connect with the following error:\r\n"
						+response.Status.code + " - " +response.Status.description+"\r\n"
						+ "Servers may need a few hours before ready to accept new user information.";
				}
				else {
					strResponse=response.Content;
				}
			}
			catch(SoapException ex) {
				strResponse="If this is a new customer, this error might be due to an invalid Username or Password.  Servers may need a few hours before ready to accept new user information.\r\n"
					+"Error message received directly from Claim Connect:  "+ex.Message+"\r\n\r\n"+ex.Detail.InnerText;
			}
			//cleanup response.  Seems to start with \n and 4 spaces.  Ends with trailing \n.
			strResponse=strResponse.TrimStart('\n');
			//strResponse.Replace("\n","");
			strResponse=strResponse.TrimStart(' ');
			//CodeBase.MsgBoxCopyPaste msgbox=new CodeBase.MsgBoxCopyPaste(response.content);
			//msgbox.ShowDialog();
			return strResponse;

			/*
			string strRawResponse="";
			string strRawResponseNormal="ISA*00*          *00*          *30*330989922      *29*AA0989922      *030606*0936*U*00401*000013966*0*T*:~GS*HB*330989922*AA0989922*20030606*0936*13966*X*004010X092~ST*271*0001~BHT*0022*11*ASX012145WEB*20030606*0936~HL*1**20*1~NM1*PR*2*ACME INC*****PI*12345~HL*2*1*21*1~NM1*1P*1*PROVLAST*PROVFIRST****SV*5558006~HL*3*2*22*0~TRN*2*100*1330989922~NM1*IL*1*SMITH*JOHN*B***MI*123456789~REF*6P*XYZ123*GROUPNAME~REF*18*2484568*TEST PLAN NAME~N3*29 FREMONT ST*~N4*PEACE*NY*10023~DMG*D8*19570515*M~DTP*307*RD8*19910712-19920525~EB*1*FAM*30~SE*17*0001~GE*1*13966~IEA*1*000013966~";
			string strRawResponseFailureAuth=@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	<soapenv:Body>
		<soapenv:Fault>
			<faultcode>soapenv:Server.userException</faultcode>
			<faultstring>Authentication failed.</faultstring>
			<faultactor/>
			<detail>
				<string>Authentication failed.</string>
			</detail>
		</soapenv:Fault>
	</soapenv:Body>
</soapenv:Envelope>";
			string strRawResponseFailure997=@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	<soapenv:Body>
		<soapenv:Fault>
			<faultcode>soapenv:Server.userException</faultcode>
			<faultstring>Malformed document sent. Please insure that the format is correct and all required data is present.</faultstring>
			<faultactor/>
			<detail>
				<string>ISA*00*          *00*          *30*330989922      *30*BB0989922      *030606*1351*U*00401*000014066*0*T*:~GS*FA*330989922**20030606*1351*14066*X*004010~ST*997*0001~AK1*HR*100~AK2*276*0001~AK3*DMG*10**8~AK4*2**8*20041210~AK5*R*5~AK9*R*0*0*0*3~SE*8*0001~GE*1*14066~IEA*1*000014066~</string>
			</detail>
		</soapenv:Fault>
	</soapenv:Body>
</soapenv:Envelope>";
			return strRawResponseNormal;*/

			/*
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(strRawResponse);
			//StringReader strReader=new StringReader(strRawResponseNormal);
			//XmlReader xmlReader=XmlReader.Create(strReader);
			//xmlReader...MoveToElement(
			XmlNode node=doc.SelectSingleNode("//lookupEligibilityReturn");
			if(node!=null) {
				return node.InnerText;//271
			}
			node=doc.SelectSingleNode("//detail/string");
			if(node==null) {
				throw new ApplicationException("Returned data not in expected format: "+strRawResponse);
			}
			if(node.InnerText=="Authentication failed.") {
				throw new ApplicationException("Authentication failed.");
			}
			return node.InnerText;//997
			*/
		}

		///<summary>Validates a given claim with ClaimConnect. Returns the results of the API call.
		///Throws an ODException if the user has not enabled attachment sending or if the API call failed for some reason.
		///Callers of this method should catch that scenario. This method does optional file I/O for trouble shooting, do not call from Middle Tier.</summary>
		public static ValidateClaimResponse ValidateClaim(Claim claim,bool doValidateForAttachment) {
			Clearinghouse clearingHouse=GetClearingHouseForClaim(claim);
			if(!clearingHouse.IsAttachmentSendAllowed) {
				throw new ODException(Lans.g("ClaimConnect","Attachment sending is not enabled. Please see our manual for instructions."));
			}
			Dentalxchange2016.DwsService service=new Dentalxchange2016.DwsService();
			Dentalxchange2016.textRequest textRequest=new Dentalxchange2016.textRequest();
			textRequest.outputFormat=Dentalxchange2016.Format.XML;
			textRequest.validateForAttachment=doValidateForAttachment;
			//Generate X12 message text.
			int batchNum=Clearinghouses.GetNextBatchNumber(clearingHouse);
			List<ClaimSendQueueItem> listQueueItems=Claims.GetQueueList(claim.ClaimNum,claim.ClinicNum,0).ToList();
			textRequest.Content=x837Controller.GenerateBatch(clearingHouse,listQueueItems,batchNum,claim.MedType);
			service.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDwsURL,"https://webservices.dentalxchange.com/dws/DwsService");
			if(ODBuild.IsDebug()) {
				service.Url="https://prelive2.dentalxchange.com/dws/DwsService";
			}
			if(PrefC.GetBool(PrefName.SaveDXCSOAPAsXML)) {
				//This code will output the XML into a text file.  This may be needed for ClaimConnect when troubleshooting issues.
				//This XML will be the SOAP body and exclude the header and envelope.
				System.Xml.Serialization.XmlSerializer xml=new System.Xml.Serialization.XmlSerializer(textRequest.GetType());
				try {
					using StreamWriter writer=new StreamWriter(clearingHouse.ExportPath+"Claim"+POut.Long(claim.ClaimNum)+"XML.txt");
					xml.Serialize(writer,textRequest);
				}
				catch(Exception ex) {
					ex.DoNothing();
					throw new ODException(Lans.g("ClaimConnect","Error writing XML to export path. Please verify that the export path has been set."));
				}
			}
			Dentalxchange2016.textResponse response=service.validateClaim(DxcCredentials.GetDentalxchangeCredentials(claim,clearingHouse),textRequest);
			if(response==null) {
				throw new ODException(Lans.g("ClaimConnect","No response from ClaimConnect was received."));
			}
			if(response.Status==null || response.Status.code!=0) {//The API call failed for some reason
				string errorMsg=Lans.g("ClaimConnect","ClaimConnect error:");
				if(response.Status==null) {
					errorMsg+=" Invalid response status";
				}
				else {
					errorMsg+=" "+response.Status.description;
				}
				throw new ODException(errorMsg);
			}
			X277 x277=new X277(response.Content);
			InsertEtransEntry(response.Content,claim,clearingHouse);
			string claimValidationNote="";
			//DentalXChange places the validation note in one of the STC segments.  All of the examples in their documentation showed it would always
			//be in the last STC segment, but recently customer 17202 had a claim where the validation note was put in the 2nd and 3rd STC
			//segments (out of 5), but there was no note in the last STC segment.  Because of this we will now loop through all STC segments and set
			//the validation note to the last STC segment with a non empty note.
			List<X12Segment> listStcSegments=x277.Segments.FindAll(x => x.SegmentID=="STC").ToList();//Each item will be in the same order as the 277.
			for(int i=listStcSegments.Count-1;i>=0;i--) {//Bottom up.
				claimValidationNote=listStcSegments[i].Get(12);//Will return empty string if element 12 is not present.
				if(claimValidationNote!="") {
					break;
				}
			}
			//Create our response object
			ValidateClaimResponse retVal=new ValidateClaimResponse(response.Status.code,response.Status.description,claimValidationNote.Split(','));
			return retVal;
		}

		///<summary>Uses the Dentalxchange API to create an attachment for the given claim. Returns the attachmentID from 
		///Dentalxchange's server. Will throw an ODException if the operation is not successful or the claim has invalid fkeys.
		///Callers of this method should handle this scenario.</summary>
		public static string CreateAttachment(List<ImageAttachment> listImages,string narrative,Claim claim) {
			DentalxchangePartnerService.DeaPartnerService service=new DentalxchangePartnerService.DeaPartnerService();
			service.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDeaURL,"https://webservices.dentalxchange.com/dea/DeaPartnerService");
			if(ODBuild.IsDebug()) {
				service.Url="https://prelive2.dentalxchange.com/dea/DeaPartnerService";
			}
			//Convert listImages to AttachmentImage[]
			DentalxchangePartnerService.AttachmentImage[] arrayAttachments=new DentalxchangePartnerService.AttachmentImage[listImages.Count];
			for(int i=0;i<listImages.Count;i++) {
				arrayAttachments[i]=listImages[i].ConvertToAttachmentImage();
			}
			#region Xml Serialization
			if(PrefC.GetBool(PrefName.SaveDXCSOAPAsXML)) {
				Clearinghouse clearingHouse=GetClearingHouseForClaim(claim);
				using StreamWriter writer1=new StreamWriter(clearingHouse.ExportPath+"Claim"+POut.Long(claim.ClaimNum)+"CredentialsXML.txt");
				//Before running the serialization you will need to change the class DxcCredentials to public.
				System.Xml.Serialization.XmlSerializer xml1=new System.Xml.Serialization.XmlSerializer(DxcCredentials.GetDentalxchangeCredentials(claim).GetType());
				xml1.Serialize(writer1,DxcCredentials.GetDentalxchangeCredentials(claim));
				using StreamWriter writer2=new StreamWriter(clearingHouse.ExportPath+"Claim"+POut.Long(claim.ClaimNum)+"AttachmentRequestXML.txt");
				System.Xml.Serialization.XmlSerializer xml2=new System.Xml.Serialization.XmlSerializer(BuildAttachmentRequest(claim,narrative).GetType());
				xml2.Serialize(writer2,BuildAttachmentRequest(claim,narrative));
				using StreamWriter writer3=new StreamWriter(clearingHouse.ExportPath+"Claim"+POut.Long(claim.ClaimNum)+"AttachmentArrayXML.txt");
				System.Xml.Serialization.XmlSerializer xml3=new System.Xml.Serialization.XmlSerializer(arrayAttachments.GetType());
				xml3.Serialize(writer3,arrayAttachments);
			}
			#endregion
			//We don't try-catch the helper because the implementation of this method should already be in a try-catch.
			DentalxchangePartnerService.AttachmentReferenceResponse response=service.sendCompleteAttachment(
				DxcCredentials.GetDentalxchangeCredentials(claim)
				,BuildAttachmentRequest(claim,narrative)
				,arrayAttachments
			);
			//Not sure which one to trust
			if(response==null) {
				throw new ODException(Lans.g("ClaimConnect","No response from ClaimConnect was received."));
			}
			if(!response.MsgSuccess || response.Status==null || response.Status.code!=0) {
				throw new ODException(response.Status==null ? Lans.g("ClaimConnect","Invalid ClaimConnect response status") : response.Status.description);
			}
			if(response.AttachmentReference==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid attachment reference received."));
			}
			return response.AttachmentReference.AttachmentID;
		}

		///<summary>This method is used to add attachments to claims that already have an existing attachmentID. Can throw an exception. If this method is not used then validation will fail.</summary>
		public static void AddAttachment(Claim claim,List<ImageAttachment> listImages) {
			DentalxchangePartnerService.DeaPartnerService deaPartnerService=new DentalxchangePartnerService.DeaPartnerService();
			deaPartnerService.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDeaURL,"https://webservices.dentalxchange.com/dea/DeaPartnerService");
			if(ODBuild.IsDebug()) {
				deaPartnerService.Url="https://prelive2.dentalxchange.com/dea/DeaPartnerService";
			}
			DentalxchangePartnerService.AttachmentReference attachmentRef=new DentalxchangePartnerService.AttachmentReference();
			attachmentRef.AttachmentID=claim.AttachmentID;
			//Can only send one image at a time. Loop through all the images the user is adding.
			foreach(ImageAttachment image in listImages) {
				DentalxchangePartnerService.ImageReferenceResponses imageResponse=deaPartnerService.addImage
					(DxcCredentials.GetDentalxchangeCredentials(claim), attachmentRef, image.ConvertToAttachmentImage());
				if(!imageResponse.MsgSuccess || imageResponse.Status.code!=0) {
					throw new ODException(imageResponse.Status.description);
				}
			}
		}

		///<summary>Helper method to construct the attachment request object.
		///This method throws when claim fkeys are invalid.
		///Callers of this method should consider this scenario.</summary>
		private static DentalxchangePartnerService.Attachment BuildAttachmentRequest(Claim claim,string narrative) {
			DentalxchangePartnerService.Attachment attachment=new DentalxchangePartnerService.Attachment();
			//Clinic on claim
			Clinic clinic=Clinics.GetClinic(claim.ClinicNum);
			if(!PrefC.HasClinicsEnabled) {
				clinic=null;//If the practice isn't using clinics, but the claim is associated to a real clinic, pretend it isn't.
			}
			//Billing provider
			Provider prov=Providers.GetProv(claim.ProvBill);
			if(prov==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid provider associated to claim."));
			}
			//Patient on the claim
			Patient pat=Patients.GetPat(claim.PatNum);
			if(pat==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid patient associated to claim."));
			}
			//Inssub
			InsSub insSub=InsSubs.GetOne(claim.InsSubNum);
			if(insSub==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid insurance subscriber associated to claim."));
			}
			//Insplan
			InsPlan insPlan=InsPlans.GetPlan(claim.PlanNum,null);
			if(insPlan==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid insurance plan associated to claim."));
			}
			//Carrier
			Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			if(carrier==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid carrier associated to claim."));
			}
			if(carrier.ElectID.Length<2) {
				throw new ODException(Lans.g("ClaimConnect","Invalid ElectID."));
			}
			//Subscriber
			Patient subscriber=Patients.GetPat(insSub.Subscriber);
			if(carrier==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid patient subscriber associated to claim."));
			}
			attachment.BillProviderFirstName=prov.FName;
			attachment.BillProviderLastName=prov.LName;
			attachment.BillProviderNpi=prov.NationalProvID;
			attachment.BillProviderTaxonomy=X12Generator.GetTaxonomy(prov);
			if(prov.UsingTIN) {
				attachment.BillProviderTaxID=prov.SSN;
			}
			else {
				attachment.BillProviderTaxID="";
			}
			//Billing info
			string billingAddress1="";
			string billingAddress2="";
			string billingCity="";
			string billingState="";
			string billingZip="";
			if(clinic==null) {
				if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
					billingAddress1=PrefC.GetString(PrefName.PracticeBillingAddress);
					billingAddress2=PrefC.GetString(PrefName.PracticeBillingAddress2);
					billingCity=PrefC.GetString(PrefName.PracticeBillingCity);
					billingState=PrefC.GetString(PrefName.PracticeBillingST);
					billingZip=PrefC.GetString(PrefName.PracticeBillingZip);
				}
				else {
					billingAddress1=PrefC.GetString(PrefName.PracticeAddress);
					billingAddress2=PrefC.GetString(PrefName.PracticeAddress2);
					billingCity=PrefC.GetString(PrefName.PracticeCity);
					billingState=PrefC.GetString(PrefName.PracticeST);
					billingZip=PrefC.GetString(PrefName.PracticeZip);
				}
			}
			else {
				if(clinic.UseBillAddrOnClaims) {
					billingAddress1=clinic.BillingAddress;
					billingAddress2=clinic.BillingAddress2;
					billingCity=clinic.BillingCity;
					billingState=clinic.BillingState;
					billingZip=clinic.BillingZip;
				}
				else {
					billingAddress1=clinic.Address;
					billingAddress2=clinic.Address2;
					billingCity=clinic.City;
					billingState=clinic.State;
					billingZip=clinic.Zip;
				}
			}
			attachment.BillProviderAdd1=billingAddress1;
			attachment.BillProviderAdd2=billingAddress2;
			attachment.BillProviderCity=billingCity;
			attachment.BillProviderState=billingState;
			attachment.BillProviderZip=billingZip;
			attachment.PatientFirstName=pat.FName;
			attachment.PatientLastName=pat.LName;
			attachment.PatientDOB=pat.Birthdate;
			attachment.SubscriberId=insSub.SubscriberID;
			attachment.SubscriberFirstName=subscriber.FName;
			attachment.SubscriberLastName=subscriber.LName;
			attachment.ProviderClaimID=TruncateClaimIdentifierIfNeeded(claim.ClaimIdentifier);
			attachment.PayerIdCode=carrier.ElectID;
			if(claim.DateService.Year>1880) {
				attachment.DateOfService=claim.DateService;
				attachment.DateOfServiceSpecified=true;
			}
			attachment.Narrative=narrative;
			if(clinic==null) {//Clinics disabled or Headquarters
				attachment.RenderingProviderAdd1=PrefC.GetString(PrefName.PracticeAddress);
				attachment.RenderingProviderAdd2=PrefC.GetString(PrefName.PracticeAddress2);
				attachment.RenderingProviderCity=PrefC.GetString(PrefName.PracticeCity);
				attachment.RenderingProviderState=PrefC.GetString(PrefName.PracticeST);
				attachment.RenderingProviderZip=PrefC.GetString(PrefName.PracticeZip);
			}
			else {//Clinic enabled
				attachment.RenderingProviderAdd1=clinic.Address;
				attachment.RenderingProviderAdd2=clinic.Address2;
				attachment.RenderingProviderCity=clinic.City;
				attachment.RenderingProviderState=clinic.State;
				attachment.RenderingProviderZip=clinic.Zip;
			}
			return attachment;
		}

		///<summary>Mimics X837_5010.Sout with maxL set to 20. This behavior is also identical to X837_4010.</summary>
		private static string TruncateClaimIdentifierIfNeeded(string inputStr) {
			inputStr=inputStr.Trim();//removes leading and trailing spaces.
			if(inputStr.Length>20) {
				inputStr=inputStr.Substring(0,20);
			}
			return inputStr;
		}
		
		private static Clearinghouse GetClearingHouseForClaim(Claim claim) {
			InsPlan insPlan=InsPlans.GetPlan(claim.PlanNum,null);
			if(insPlan==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid insurance plan associated to claim."));
			}
			Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			if(carrier==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid carrier associated to claim."));
			}
			if(carrier.ElectID.Length<2) {
				throw new ODException(Lans.g("ClaimConnect","Invalid ElectID."));
			}
			//Fill clearing house with HQ fields
			long clearingHouseNum=Clearinghouses.AutomateClearinghouseHqSelection(carrier.ElectID,claim.MedType);
			Clearinghouse clearingHouse=Clearinghouses.GetClearinghouse(clearingHouseNum);
			//Refill clearingHouse with clinic specific fields
			return Clearinghouses.OverrideFields(clearingHouse,claim.ClinicNum);
		}

		///<summary>Creates an Etrans entry into the database for the response we get back from the ValidateClaim() method.</summary>
		private static void InsertEtransEntry(string messageText,Claim claim,Clearinghouse clearingHouse) {
			//Make entry for EtransMessageText
			EtransMessageText etransMessageText=new EtransMessageText();
			etransMessageText.MessageText=messageText;
			EtransMessageTexts.Insert(etransMessageText);
			//Make Etrans entry and attach the EtransMessageText
			Etrans etrans=new Etrans();
			etrans.ClaimNum=claim.ClaimNum;
			etrans.PatNum=claim.PatNum;
			etrans.UserNum=Security.CurUser.UserNum;
			etrans.DateTimeTrans=DateTime.Now;
			etrans.ClearingHouseNum=clearingHouse.ClearinghouseNum;
			etrans.Etype=EtransType.DXCAttachments;
			etrans.PlanNum=claim.PlanNum;
			etrans.EtransMessageTextNum=etransMessageText.EtransMessageTextNum;
			Etranss.Insert(etrans);
		}

		public static bool Retrieve(Clearinghouse clearinghouse,IODProgressExtended progress=null) {
			ErrorMessage="";
			if(clearinghouse.IsEraDownloadAllowed==EraBehaviors.None) {//Do not download.
				ErrorMessage="Clearinghouse is not setup to retrieve ERAs.";
				return false;
			}
			progress=progress??new ODProgressExtendedNull();
			progress.UpdateProgress(Lans.g(progress.LanThis,"Contacting web server and downloading reports"),"reports","17%",17);
			if(progress.IsPauseOrCancel()) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
					return false;
			}
			Dentalxchange2016.Credentials cred=new Dentalxchange2016.Credentials();
			if(PrefC.GetBool(PrefName.CustomizedForPracticeWeb)) {//even though they currently use code from a different part of the program.
				cred.Client="Practice-Web";
				cred.ServiceID="DCI Web Service ID: 001513";
			}
			else {
				cred.Client="OpenDental";
				cred.ServiceID="DCI Web Service ID: 002778";
			}
			cred.Username=clearinghouse.LoginID;
			cred.Password=clearinghouse.Password;
			Dentalxchange2016.unProcessedEraRequest request=new Dentalxchange2016.unProcessedEraRequest();
			Dentalxchange2016.DwsService service=new Dentalxchange2016.DwsService();
			service.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDwsURL,"https://webservices.dentalxchange.com/dws/DwsService");
			if(ODBuild.IsDebug()) {
				service.Url="https://prelive2.dentalxchange.com/dws/DwsService";
			}
			List<string> listEraStrings=new List<string>();
			try {
				Dentalxchange2016.UnProcessedEraResponse response;
				do {
					response=service.getUnProcessedEra(cred,request);
					if(response.Status.code==0 && response.ClaimPaymentAdvice.EdiContent!=null) {
						listEraStrings.Add(response.ClaimPaymentAdvice.EdiContent);//X12 835 ERA raw content
					}
				} while(response.Status.code==0 && response.ClaimPaymentAdvice!=null && response.ClaimPaymentAdvice.AdditionalEraExists);
				if(response.Status.code!=0) {//!=Approved
																		 //If the following error message changes, then also see if FormClaimReports.RetrieveReports() needs to change as well.
					ErrorMessage="Era request unsuccessful."
						+"\r\nError message received directly from Claim Connect: "+response.Status.code
						+"\r\n\r\n"+response.Status.description;
					return false;
				}
				/*
				Code Description
				0    Approved
				1    Operation Failed
				90   Invalid Group
				100  Invalid User
				110  Invalid Client
				120  Service Not Allowed
				130  User Not Allowed
				140  Invalid PMS
				150  Service Not Contracted by User
				1000 Internal server error has occurred. The problem is being investigated.
				2000 Generic Host Error
				2001 Malformed document sent. Please insure that the format is correct and all required data is present.
				2002 Deficient request - required data is missing.
				2003 No insurers found for this selection.
				2004 Unable to contact the payer at this time. Please try again later.
				2005 Only the relationship self is supported at this time.
				2007 Original request patient info reflected back
				2008 Claim submission failed.
				*/
			}
			catch(Exception ex) {
				ErrorMessage=Lans.g(progress.LanThis,"If this is a new customer, this error might be due to an invalid Username or Password.  "
					+"Servers may need a few hours before ready to accept new user information.")+"\r\n"
					+Lans.g(progress.LanThis,"Error message received directly from Claim Connect:")+"  "+ex.ToString();
				return false;
			}
			progress.UpdateProgress(Lans.g(progress.LanThis,"Web server contact successful."));
			string path=clearinghouse.ResponsePath;
			progress.UpdateProgress(Lans.g(progress.LanThis,"Writing files"),"reports","40%",40);
			if(progress.IsPauseOrCancel()) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
				return false;
			}
			//write each message to a distinct file in the export path.	
			listEraStrings.ForEach(x => File.WriteAllText(ODFileUtils.CreateRandomFile(path,".txt"),x));
			progress.UpdateProgress(Lans.g(progress.LanThis,"Files written successfully."));
			progress.UpdateProgress(Lans.g(progress.LanThis,"Finalizing"),"reports","50%",50);
			if(progress.IsPauseOrCancel()) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
				return false;
			}
			return true;
		}

		///<summary>This enum represents the possible ImageTypeCode values sent through DentalXChange. 
		///If any additional values are added then the ImageAttachment.ConvertToAttachmentImage() method must also be modified.</summary>
		public enum ImageTypeCode {
			///<summary>0</summary>
			[Description("Referral Form")]
			ReferralForm,
			///<summary>1</summary>
			[Description("Diagnostic Report")]
			DiagnosticReport,
			///<summary>2</summary>
			[Description("Explanation of Benefits")]
			ExplanationOfBenefits,
			///<summary>3</summary>
			[Description("Other Attachments")]
			OtherAttachments,
			///<summary>4</summary>
			[Description("Periodontal Charts")]
			PeriodontalCharts,
			///<summary>5</summary>
			[Description("X-Rays")]
			XRays,
			///<summary>6</summary>
			[Description("Dental Models")]
			DentalModels,
			///<summary>7</summary>
			[Description("Radiology Reports")]
			RadiologyReports,
			///<summary>8</summary>
			[Description("Intra-Oral Photograph")]
			IntraOralPhotograph,
			///<summary>9</summary>
			[Description("Narrative")]
			Narrative,
		}

		///<summary>Helper class that parses out the error messages from the API response object</summary>
		public class ValidateClaimResponse {
			public int ResponseCode;
			public string ResponseDescription;
			public string[] ValidationErrors;

			///<summary>Per Dentalxchange's documentation the flag for whether the claim requires an
			///attachment will always be a string response like "ATTACHMENT IS REQUIRED".</summary>
			public bool IsAttachmentRequired {
				get {
					for(int i=0;i<ValidationErrors.Length;i++) {
						if(ValidationErrors[i].ToLower().Contains("attachment")) {
							return true;
						}
					}
					return false;
				}
			}

			///<summary>Dentalxchange does not provide a field to indicate whether the claim is valid or not.
			///Therefore we have defined a claim to be valid if the request was received and no additional
			///information has been requested in the 277 response.</summary>
			public bool _isValidClaim {
				get {
					//If the API call was successful, and we only have one EMPTY entry in validationErrors
					//then we know that for this claim DXC does not need any additional attachments.
					if(ResponseCode==0 && ValidationErrors.Length<=1 && ValidationErrors[0]=="") {
						return true;
					}
					return false;
				}
			}

			public ValidateClaimResponse(int responseCode,string responseDescription,string[] validationErrors) {
				ResponseCode=responseCode;
				ResponseDescription=responseDescription;
				ValidationErrors=validationErrors;
			}
		}

		///<summary>Helper class used to aggregate all of the information needed to create the DentalxchangePartnerService.attachmentImage object.</summary>
		public class ImageAttachment {
			///<summary>See enum ImageTypeCode</summary>
			public ImageTypeCode ImageType;
			///<summary>Required format for sending the image to Dentalxchange.</summary>
			public byte[] ImageFileAsBase64;
			public string ImageFileNameDisplay;
			public string ImageFileNameActual;
			public string ImageOrientationType;
			public DateTime ImageDate;
			///<summary>Set when the image is saved to the patient's images module.
			///Used to look up when previewing.</summary>
			public long DocNum;

			public ImageAttachment() {
			}

			///<summary>Converts this helper class into the corresponding ClaimConnect AttachmentImage class with all of the same values.</summary>
			public DentalxchangePartnerService.AttachmentImage ConvertToAttachmentImage() {
				DentalxchangePartnerService.AttachmentImage attachImage=new DentalxchangePartnerService.AttachmentImage();
				switch(ImageType) {
					case ImageTypeCode.ReferralForm:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.B4;
						break;
					case ImageTypeCode.DiagnosticReport:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.DG;
						break;
					case ImageTypeCode.ExplanationOfBenefits:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.EB;
						break;
					case ImageTypeCode.PeriodontalCharts:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.P6;
						break;
					case ImageTypeCode.XRays:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.RB;
						break;
					case ImageTypeCode.DentalModels:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.DA;
						break;
					case ImageTypeCode.RadiologyReports:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.RR;
						break;
					case ImageTypeCode.IntraOralPhotograph:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.XP;
						break;
					case ImageTypeCode.Narrative:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.Item03;
						break;
					//If the image type code is somehow mangled just mark the attachment as 'Other'. This ensures there is always an image type code set.
					default:
						attachImage.ImageTypeCode=DentalxchangePartnerService.imageTypeCode.OZ;
						break;
				}
				//This MUST be set so that the ImageTypeCode is serialized in the API call.
				attachImage.ImageTypeCodeSpecified=true;
				attachImage.ImageFileAsBase64=ImageFileAsBase64;
				attachImage.ImageFileName=ImageFileNameDisplay;
				if(ImageOrientationType=="left") {
					attachImage.ImageOrientationType=DentalxchangePartnerService.orientationType.LEFT;
				}
				else {
					attachImage.ImageOrientationType=DentalxchangePartnerService.orientationType.RIGHT;
				}
				//Only send the image orientation if the image is an x-ray. (Per Tom at DXC)
				if(attachImage.ImageTypeCode==DentalxchangePartnerService.imageTypeCode.RB) {
					attachImage.ImageOrientationTypeSpecified=true;
				}
				attachImage.ImageDate=ImageDate;
				attachImage.ImageDateSpecified=true;
				return attachImage;
			}

			public static ImageAttachment Create(string fileName,DateTime createdDate,ImageTypeCode typeCodeImage,Image imageClaim,bool rightOrientation=true) {
				ImageAttachment imageAttachment=new ImageAttachment();
				imageAttachment.ImageFileNameDisplay=fileName;
				imageAttachment.ImageDate=createdDate;
				imageAttachment.ImageType=typeCodeImage;
				if(rightOrientation) {
					imageAttachment.ImageOrientationType="right";
				}
				else {
					imageAttachment.ImageOrientationType="left";
				}
				imageAttachment.ImageFileAsBase64=ConvertImageToBytes(imageClaim);
				return imageAttachment;
			}

			///<summary>Takes the user's image they want to send with their claim and converts it to a base64 byte representation.
			///ClaimConnect requires the image to be in this format.</summary>
			private static byte[] ConvertImageToBytes(Image image) {
				using MemoryStream m=new MemoryStream();
				image.Save(m,System.Drawing.Imaging.ImageFormat.Jpeg);
				return m.ToArray();
			}
		}

		///<summary>This is a helper class that allows us to avoid duplicating rigid code to set DXC credentials for both of their webservice api endpoints.</summary>
		public class DxcCredentials {
			public string Client;
			public string ServiceID;
			public string Username;
			public string Password;

			public static implicit operator Dentalxchange2016.Credentials(DxcCredentials cred) {
				return new Dentalxchange2016.Credentials {
					Client=cred.Client,
					ServiceID=cred.ServiceID,
					Username=cred.Username,
					Password=cred.Password,
				};
			}

			public static implicit operator DentalxchangePartnerService.Credentials(DxcCredentials cred) {
				return new DentalxchangePartnerService.Credentials {
					Client=cred.Client,
					ServiceID=cred.ServiceID,
					Username=cred.Username,
					Password=cred.Password,
				};
			}

			///<summary>Creates the credentials needed for all Dentalxchange methods.
			///This is needed because there are 2 separate web services that use these credentials.
			///DxcCredentials has implict operators to fix this nuance</summary>
			public static DxcCredentials GetDentalxchangeCredentials(Claim claim,Clearinghouse clearHouse=null) {
				Clearinghouse clearingHouse=clearHouse;
				if(clearingHouse==null) {//No clearing house provided.
					clearingHouse=GetClearingHouseForClaim(claim);
				}
				DxcCredentials cred=new DxcCredentials();
				if(PrefC.GetBool(PrefName.CustomizedForPracticeWeb)) {//even though they currently use code from a different part of the program.
					cred.Client="Practice-Web";
					cred.ServiceID="DCI Web Service ID: 001513";
				}
				else {
					cred.Client="OpenDental";
					cred.ServiceID="DCI Web Service ID: 002778";
				}
				cred.Username=clearingHouse.LoginID;
				cred.Password=clearingHouse.Password;
				return cred;
			}
		}
	}

	/*
	[System.Web.Services.WebServiceBindingAttribute(Name="MyMathSoap",Namespace="http://www.contoso.com/")]
	public class MyMath:System.Web.Services.Protocols.SoapHttpClientProtocol {
		
		[System.Diagnostics.DebuggerStepThroughAttribute()]
		public MyMath() {
			this.Url = "http://www.contoso.com/math.asmx";
		}

		[System.Diagnostics.DebuggerStepThroughAttribute()]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.contoso.com/Add",RequestNamespace="http://www.contoso.com/",ResponseNamespace="http://www.contoso.com/",Use=System.Web.Services.Description.SoapBindingUse.Literal,ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public int Add(int num1,int num2) {
			object[] results = this.Invoke("Add",new object[] {num1,
                        num2});
			return ((int)(results[0]));
		}

		[System.Diagnostics.DebuggerStepThroughAttribute()]
		public System.IAsyncResult BeginAdd(int num1,int num2,System.AsyncCallback callback,object asyncState) {
			return this.BeginInvoke("Add",new object[] {num1,
                        num2},callback,asyncState);
		}

		[System.Diagnostics.DebuggerStepThroughAttribute()]
		public int EndAdd(System.IAsyncResult asyncResult) {
			object[] results = this.EndInvoke(asyncResult);
			return ((int)(results[0]));
		}
	}*/

}
