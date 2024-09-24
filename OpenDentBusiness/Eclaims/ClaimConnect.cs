using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Net.Http;
using static OpenDentBusiness.EB271;

namespace OpenDentBusiness.Eclaims {
	/// <summary>
	/// Discussed the following with Navin Narayanan from DentalXChange on 4/29/2024.
	/// DentalXChange has two workflows for sending attachments. The first is using sendCompleteAttachment, where we are able to create, add images, add narrative, and send an attachment in a single call. DXC returns an attachment ID which is stored in the claim table prior to sending the claim. The second workflow uses multiple calls and hits the openAttachment, addImage, addNarrative, and submitAttachment endpoints. We currently use a combination of the two workflows where we begin by calling sendCompleteAttachment and then allow users to add more images by calling addImage. While this is not the exact intention of these workflows DXC assured us that that using the two workflows at the same time is fine. We will soon be making use of the addNarrative endpoint as well. Sending a new narrative to an attachment will overwrite the old narrative.
	/// We have updated the DentalXChange web reference in order to have access to their newer endpoints. Doing so changed the parameters that sendCompleteAttachment takes, as they now required releaseAttachment and releaseAttachmentSpecified. Both of these are bools and will be set to false in order to continue using sendCompleteAttachment as we have been. 
	/// Navin also stated that when we move to XConnect, that we will have these same methods and parameters, so the transition will be seamless.
	/// Derek has a theory that releaseAttachmentSpecified is put in place by VS to allow backwards compatibility. If this bool is false, it will act as if a bool for releaseAttachment has not been passed in. Essentially allowing for two bools to create a state where the parameter is null. This has not been confirmed by DXC.
	/// As of 5/6/2024:
	/// We are now making use of DXC's manual attachment workflow where we call openAttachment, addImage, and submitAttachment. This workflow behaves the same as calling sendCompleteAttachment, but gives us more control over each stage and response in this process. We are changing to this so we have access to the ImageReferenceId of each image sent to DXC.
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
					serverName=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDciURL,"https://claimconnect.dentalxchange.com/dci/upload.svl");
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
		public static string Benefits270(Clearinghouse clearinghouseClin,string x12message,out Etrans etransHtml,Dentalxchange2016.Format outputFormat=Dentalxchange2016.Format.EDI) {
			etransHtml=null;
			Dentalxchange2016.Credentials cred=DxcCredentials.GetDentalxchangeCredentials(null,clearinghouseClin);//Null claim because we have a clearinghouse	
			cred.version=Application.ProductVersion;
			Dentalxchange2016.textRequest request=new Dentalxchange2016.textRequest();
			request.outputFormatSpecified=true;
			request.outputFormat=outputFormat;
			request.Content=HttpUtility.HtmlEncode(x12message);//get rid of ampersands, etc.
			Dentalxchange2016.DwsService service=new Dentalxchange2016.DwsService();
			if(ODBuild.IsDebug()) {
				//service.Url="https://prelive2.dentalxchange.com/dws/DwsService"; // testing
				service.Url="https://webservices.dentalxchange.com/dws/DwsService"; // production
			}
			else {
				// Production URL
				service.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDwsURL,"https://webservices.dentalxchange.com/dws/DwsService");
			}
			string strResponse="";
			try {
				Dentalxchange2016.textResponse response = service.lookupEligibility(cred,request);
				if(response.Content==null) {
					strResponse="This customer is being denied service by Claim Connect with the following error:\r\n"
						+response.Status.code + " - " +response.Status.description+"\r\n"
						+ "Servers may need a few hours before ready to accept new user information.";
					return strResponse;
				}
				strResponse=response.Content;
				if(outputFormat==Dentalxchange2016.Format.HTML) {//response.Content does not need formatting, already in table node.
					etransHtml=Etranss.CreateEtrans(DateTime.Now,clearinghouseClin.HqClearinghouseNum,strResponse,Security.CurUser.UserNum);
					etransHtml.Etype=EtransType.HTML;
					Etranss.Insert(etransHtml);
					return strResponse;
				}
			}
			catch(SoapException ex) {
				strResponse="If this is a new customer, this error might be due to an invalid Username or Password.  Servers may need a few hours before ready to accept new user information.\r\n"
					+"Error message received directly from Claim Connect:  "+ex.Message+"\r\n\r\n"+ex.Detail.InnerText;
			}
			//cleanup EDI response.  Seems to start with \n and 4 spaces.  Ends with trailing \n.
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
				,releaseAttachment:false
				,releaseAttachmentSpecified:false
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

		///<summary>The first step in DXC's manual attachment workflow. Creates a DXC attachment for the claim and returns the AttachmentID. Is capable of taking a narrative. Can throw exceptions.</summary>
		public static string OpenAttachment(Claim claim,string narrative) {
			DentalxchangePartnerService.DeaPartnerService deaPartnerService=new DentalxchangePartnerService.DeaPartnerService();
			deaPartnerService.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDeaURL,"https://webservices.dentalxchange.com/dea/DeaPartnerService");
			if(ODBuild.IsDebug()) {
				deaPartnerService.Url="https://prelive2.dentalxchange.com/dea/DeaPartnerService";
			}
			DentalxchangePartnerService.Attachment attachment=BuildAttachmentRequest(claim,narrative);
			DentalxchangePartnerService.AttachmentReferenceResponse attachmentReferenceResponse=deaPartnerService.openAttachment(DxcCredentials.GetDentalxchangeCredentials(claim),attachment);
			if(attachmentReferenceResponse==null) {
				throw new ODException(Lans.g("ClaimConnect","No response from ClaimConnect was received."));
			}
			if(!attachmentReferenceResponse.MsgSuccess || attachmentReferenceResponse.Status==null || attachmentReferenceResponse.Status.code!=0)
			{
				throw new ODException(attachmentReferenceResponse.Status==null ? Lans.g("ClaimConnect","Invalid ClaimConnect response status") : attachmentReferenceResponse.Status.description);
			}
			if(attachmentReferenceResponse.AttachmentReference==null) {
				throw new ODException(Lans.g("ClaimConnect","Invalid attachment reference received."));
			}
			return attachmentReferenceResponse.AttachmentReference.AttachmentID;
		}

		///<summary>The last step in DXC's manual attachment workflow, although we are still allowed to add more images later. Can throw exceptions.</summary>
		public static void SubmitAttachment(Claim claim) {
			DentalxchangePartnerService.DeaPartnerService deaPartnerService=new DentalxchangePartnerService.DeaPartnerService();
			deaPartnerService.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDeaURL,"https://webservices.dentalxchange.com/dea/DeaPartnerService");
			if(ODBuild.IsDebug()) {
				deaPartnerService.Url="https://prelive2.dentalxchange.com/dea/DeaPartnerService";
			}
			DentalxchangePartnerService.AttachmentReference attachmentReference=new DentalxchangePartnerService.AttachmentReference();
			attachmentReference.AttachmentID=claim.AttachmentID;
			DentalxchangePartnerService.DeaResponse deaResponse=deaPartnerService.submitAttachment(DxcCredentials.GetDentalxchangeCredentials(claim),attachmentReference);
			if(deaResponse==null) {
				throw new ODException(Lans.g("ClaimConnect","No response from ClaimConnect was received."));
			}
			if(!deaResponse.MsgSuccess || deaResponse.Status==null || deaResponse.Status.code!=0) {
				throw new ODException(deaResponse.Status==null ? Lans.g("ClaimConnect","Invalid ClaimConnect response status") : deaResponse.Status.description);
			}
		}

		///<summary>This method is used to add attachments to claims that already have an existing attachmentID. Can throw an exception. If this method is not used then validation will fail. Returns ImageReferenceIds assigned by DXC.</summary>
		public static List<int> AddAttachmentImage(Claim claim,List<ImageAttachment> listImages) {
			DentalxchangePartnerService.DeaPartnerService deaPartnerService=new DentalxchangePartnerService.DeaPartnerService();
			deaPartnerService.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDeaURL,"https://webservices.dentalxchange.com/dea/DeaPartnerService");
			if(ODBuild.IsDebug()) {
				deaPartnerService.Url="https://prelive2.dentalxchange.com/dea/DeaPartnerService";
			}
			DentalxchangePartnerService.AttachmentReference attachmentRef=new DentalxchangePartnerService.AttachmentReference();
			attachmentRef.AttachmentID=claim.AttachmentID;
			List<int> listImageReferenceIds=new List<int>();
			//Can only send one image at a time. Loop through all the images the user is adding.
			foreach(ImageAttachment image in listImages) {
				DentalxchangePartnerService.ImageReferenceResponses imageResponse=deaPartnerService.addImage
					(DxcCredentials.GetDentalxchangeCredentials(claim), attachmentRef, image.ConvertToAttachmentImage());
				if(!imageResponse.MsgSuccess || imageResponse.Status.code!=0) {
					throw new ODException(imageResponse.Status.description);
				}
				listImageReferenceIds.Add(imageResponse.ImageReferenceResponses1[0].ImageReference.ImageReferenceId);
			}
			return listImageReferenceIds;
		}

		///<summary>Add a narrative to a claim that has an existing attachmentID. Will overwrite any narrative sent to DXC. Narrative has a 2000 char limit. Can throw an exception.</summary>
		public static void AddNarrative(Claim claim,string narrative) {
			DentalxchangePartnerService.DeaPartnerService deaPartnerService=new DentalxchangePartnerService.DeaPartnerService();
			deaPartnerService.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDeaURL,"https://webservices.dentalxchange.com/dea/DeaPartnerService");
			if(ODBuild.IsDebug()) {
				deaPartnerService.Url="https://prelive2.dentalxchange.com/dea/DeaPartnerService";
			}
			DentalxchangePartnerService.AttachmentReference attachmentReference=new DentalxchangePartnerService.AttachmentReference();
			attachmentReference.AttachmentID=claim.AttachmentID;
			DentalxchangePartnerService.AttachmentReferenceResponse attachmentReferenceResponse=deaPartnerService.addNarrative(DxcCredentials.GetDentalxchangeCredentials(claim),attachmentReference,narrative);
			if(!attachmentReferenceResponse.MsgSuccess || attachmentReferenceResponse.Status.code!=0) {
				throw new ODException(attachmentReferenceResponse.Status.description);
			}
		}

		///<summary>Deletes the selected attachment images from DXC. Caller should handle images that do not have a valid ImageReferenceId. Can throw exceptions.</summary>
		public static void DeleteImages(Claim claim,List<ClaimAttach> listClaimAttaches) {
			DentalxchangePartnerService.DeaPartnerService deaPartnerService=new DentalxchangePartnerService.DeaPartnerService();
			deaPartnerService.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDeaURL,"https://webservices.dentalxchange.com/dea/DeaPartnerService");
			if(ODBuild.IsDebug()) {
				deaPartnerService.Url="https://prelive2.dentalxchange.com/dea/DeaPartnerService";
			}
			for(int i=0;i<listClaimAttaches.Count;i++) {
				DentalxchangePartnerService.ImageReference imageReference=new DentalxchangePartnerService.ImageReference();
				imageReference.ImageReferenceId=listClaimAttaches[i].ImageReferenceId;
				imageReference.ImageReferenceIdSpecified=true;
				DentalxchangePartnerService.DeaResponse deaResponse=deaPartnerService.deleteImage(DxcCredentials.GetDentalxchangeCredentials(claim),imageReference);
				if(!deaResponse.MsgSuccess || deaResponse.Status.code!=0) {
					throw new ODException(deaResponse.Status.description);
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
		
		public static Clearinghouse GetClearingHouseForClaim(Claim claim) {
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

		///<summary>Calls DxC ClaimConnect's getPayerListService API method. Will upsert any new/existing ElectIDs into the electid table.
		///Runs once a week as part of OpenDentalService.PayerListThread, and only if a ClaimConnect clearinghouse is a default clearinghouse.
		///May throw exceptions, surround with try/catch. Returns a string of error responses, will be empty on success.</summary>
		public static string GetPayerList(Clearinghouse clearinghouse) {
			//No need to check MiddleTierRole; no call to db.
			Dentalxchange2016.Credentials credentials=DxcCredentials.GetDentalxchangeCredentials(null,clearinghouse);//Null claim because we have a clearinghouse	
			credentials.version=Application.ProductVersion;
			Dentalxchange2016.payerListInfoRequest payerListInfoRequest=new Dentalxchange2016.payerListInfoRequest();
			payerListInfoRequest.outputFormatSpecified=true;
			payerListInfoRequest.outputFormat=Dentalxchange2016.Format.XML;
			Dentalxchange2016.DwsService service=new Dentalxchange2016.DwsService();
			if(ODBuild.IsDebug()) {
				service.Url="https://prelive2.dentalxchange.com/dws/DwsService"; //testing
			}
			else {
				service.Url=Introspection.GetOverride(Introspection.IntrospectionEntity.DentalXChangeDwsURL,"https://webservices.dentalxchange.com/dws/DwsService"); //production
			}
			string strResponse="";
			Dentalxchange2016.PayerListInfoResponse payerListInfoResponse=new Dentalxchange2016.PayerListInfoResponse();
			try {
				payerListInfoResponse=service.getPayerListService(credentials,payerListInfoRequest);
			}
			catch(SoapException ex) {
				strResponse="If this is a new customer, this error might be due to an invalid Username or Password.  Servers may need a few hours before ready to accept new user information.\r\n"
					+"Error message received directly from Claim Connect:  "+ex.Message+"\r\n\r\n"+ex.Detail.InnerText;
				return strResponse;
			}
			if(payerListInfoResponse.Status.code==0) {
				ElectIDs.UpsertFromDentalXChange(payerListInfoResponse.Payers);
				return strResponse;
			}
			strResponse=payerListInfoResponse.Status.description;
			return strResponse;
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
			etrans.InsSubNum=claim.InsSubNum;
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

		///<summary>Takes a payer returned from DxC's getPayerListService API method.
		///Determines the values of each Attribute attached to the payer, returning a list of EnumClaimConnectPayerAttributes which are flagged as supported for the payer.</summary>
		public static List<EnumClaimConnectPayerAttributes> GetAttributes(Dentalxchange2016.supportedTransPayer payer) {
			List<EnumClaimConnectPayerAttributes> listClaimConnectPayerAttributes=new List<EnumClaimConnectPayerAttributes>();
			if(payer is null) {
				return listClaimConnectPayerAttributes;
			}
			if(payer.Claim!=null && payer.Claim.isSupported) {
				listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.ClaimIsSupported);
			}
			if(payer.Eligibility!=null) { //The Eligibility field is a special case, where it may contain other fields (Requirements) regardless of isSupported being true or false.
				if(payer.Eligibility.isSupported) {
					listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.EligibilityIsSupported);
				}
				if(payer.Eligibility.Requirements!=null) { //Requirements is a list of "requireBeans" which contain two string fields; a name and a value.
					Dentalxchange2016.requireBean dxcRequireBean=payer.Eligibility.Requirements.FirstOrDefault(x=>x.name=="PatientAndSubscriberReqdForElig");
					if(dxcRequireBean!=null && dxcRequireBean.value=="true") {
						listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.PatientAndSubscriberReqdForElig);
					}
					dxcRequireBean=payer.Eligibility.Requirements.FirstOrDefault(x=>x.name=="PatientReqdForElig");
					if(dxcRequireBean!=null && dxcRequireBean.value=="true") {
						listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.PatientReqdForElig);
					}
				}
			}
			if(payer.Benefits!=null && payer.Benefits.isSupported) {
				listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.BenefitsIsSupported);
			}
			if(payer.ClaimStatus!=null && payer.ClaimStatus.isSupported) {
				listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.ClaimStatusIsSupported);
			}
			if(payer.ERA!=null && payer.ERA.isSupported) {
				listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.ERAIsSupported);
			}
			if(payer.RTClaim!=null && payer.RTClaim.isSupported) {
				listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.RTClaimIsSupported);
			}
			if(payer.DXCAttachment!=null && payer.DXCAttachment.isSupported) {
				listClaimConnectPayerAttributes.Add(EnumClaimConnectPayerAttributes.DXCAttachmentIsSupported);
			}
			return listClaimConnectPayerAttributes;
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
						if(ValidationErrors[i].ToLower().Contains("attachment") || ValidationErrors[i].ToLower().Contains("please upload")) {
							if(ValidationErrors[i].ToLower().Contains("does not support")) {
								return false;
							}
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

		///<summary>Storage class used to aggregate all of the information needed to create the DentalxchangePartnerService.attachmentImage object.</summary>
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

			/// <summary>Caller should dispose of image.</summary>
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
			///ClaimConnect requires the image to be in this format. The caller should dispose of the image.</summary>
			private static byte[] ConvertImageToBytes(Image image) {
				using MemoryStream memoryStream=new MemoryStream();
				using Bitmap bitmap=new Bitmap(image);
				//Save creates a system ref to the resources, preventing proper disposal of image,
				//so we use a second image
				bitmap.Save(memoryStream,System.Drawing.Imaging.ImageFormat.Jpeg);
				return memoryStream.ToArray();
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
		public class XConnect {

		internal static List<Procedure> _listProceduresForPat;
		internal static List<ClaimProc> _listClaimProcsForPat;
		internal static List<ClaimProc> _listClaimProcsForClaim;
		internal static List<InsPlan> _listInsPlans;
		internal static List<PatPlan> _listPatPlans;
		internal static List<InsSub> _listInsSubs;
		internal static List<string> _listDiagnoses;
		internal static Clearinghouse _clearinghouseForClaim;
		internal static InsPlan _insPlanForClaim;
		internal static InsSub _insSubForPat;
		internal static Carrier _carrierForClaim;
		internal static List<Provider> _listProvidersForClaim;
		internal static Claim _claim;
		private static HttpMethod _httpMethod;
		private static HttpClient _httpClient=new HttpClient() { BaseAddress=new Uri("https://api.dentalxchange.com") };

		public XConnect() {
		}

		public static bool IsEnabled(Clearinghouse clearinghouseClin) {
			//The XConnect API Key is entered into the ClaimConnect clearinghouse LocationID field.
			if(clearinghouseClin.CommBridge!=EclaimsCommBridge.ClaimConnect){
				return false;
			}
			if(string.IsNullOrWhiteSpace(clearinghouseClin.LocationID)){
				
				return false;
			}
			return true;
		}

		///<summary>Call before making any API call. This fills necessary fields to complete creation of objects for the call.</summary>
		public static void SetData(Claim claim) {
			_listProceduresForPat=Procedures.Refresh(claim.PatNum);
			_listClaimProcsForPat=ClaimProcs.Refresh(claim.PatNum);
			_listClaimProcsForClaim=ClaimProcs.GetForSendClaim(_listClaimProcsForPat,claim.ClaimNum);
			_listInsSubs=InsSubs.GetListForSubscriber(claim.PatNum);
			_clearinghouseForClaim=ClaimConnect.GetClearingHouseForClaim(claim);
			_insPlanForClaim=InsPlans.GetPlan(claim.PlanNum,new List<InsPlan>());
			_listPatPlans=PatPlans.Refresh(claim.PatNum);
			_carrierForClaim=Carriers.GetCarrier(_insPlanForClaim.CarrierNum);
			List<byte> listDiagnosesVersions=new List<byte>();
			_listDiagnoses=Procedures.GetUniqueDiagnosticCodes(Procedures.GetProcsFromClaimProcs(XConnect._listClaimProcsForClaim),false,listDiagnosesVersions);
			_listProvidersForClaim=Providers.GetProvsByProvNums(_listClaimProcsForClaim.Select(x => x.ProvNum).ToList());
			_claim=claim;
		}

		////<summary>Throws exceptions. Generic API call to XConnect. Give a payload object structured the same as the JSON definition of the API call. Returns an object of the type specified, which must also be structured the same as the JSON definition of the response for the API call. The endpointURL given must be a relative URL. The endpointURL is automatically modified to point to the sandbox in Debug mode.</summary>
		private static T CallAPI<T>(Clearinghouse clearinghouseClinic,string endpointURL,object payload,HttpMethod httpMethod,List<string> queryParameters=null) {
			if(ODBuild.IsDebug()) {//Testing
				endpointURL="/sandbox"+endpointURL;
			}
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add("username",clearinghouseClinic.LoginID);
			_httpClient.DefaultRequestHeaders.Add("password",clearinghouseClinic.Password);
			_httpClient.DefaultRequestHeaders.Add("API-Key",clearinghouseClinic.LocationID);
			JsonSerializerSettings jsonSerializerSettings=new JsonSerializerSettings{NullValueHandling=NullValueHandling.Ignore};
			jsonSerializerSettings.Converters.Add(new StringEnumConverter());
			string bodyJSON=JsonConvert.SerializeObject(payload,Newtonsoft.Json.Formatting.None,jsonSerializerSettings);
			JsonSerializerSettings deserializationSettings=new JsonSerializerSettings { MissingMemberHandling=MissingMemberHandling.Error };
			return APIRequest.Inst.SendRequest<T>(endpointURL,httpMethod,null,bodyJSON,HttpContentType.Json,_httpClient,queryParameters,deserializationSettings);
		}

		///<summary>Throws Exceptions. Returns an error message if validation failed. Otherwise returns an empty string.</summary>
		public static XConnectWebResponse ValidateClaim(Claim claim) {
			XConnectValidateClaim xConnectValidateClaim=new XConnectValidateClaim();
			XConnect.SetData(claim);
			xConnectValidateClaim.claim=XConnectClaim.FromClaim(claim);
			if(xConnectValidateClaim.claim.patient.sequenceCode=="") {
				throw new ODException("XConnect validation only accepts Primary, Secondary and Tertiary claims.");
			}
			_httpMethod=HttpMethod.Post;
			return CallAPI<XConnectWebResponse>(_clearinghouseForClaim,"/claims/validation",xConnectValidateClaim,_httpMethod);
		}

		///<summary>Copied from X837_5010</summary>
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

		///<summary>Gets and converts provider specialty from taxonomy code to XConnect API provider specialty string.</summary>
		public static EnumXConnectProviderSpecialty GetProviderSpecialty(Provider provider) {
			string taxonomy=GetTaxonomy(provider);
			EnumXConnectProviderSpecialty specialty=EnumXConnectProviderSpecialty.GP;
			switch(taxonomy) {
				case "1223G0001X": specialty=EnumXConnectProviderSpecialty.GP; break;
				//case "1223G0001X": specialty=EnumXConnectProviderSpecialty.IMP; break;//Implantology
				//case "1223G0001X": specialty=EnumXConnectProviderSpecialty.OPER; break;//Operative
				//case "1223G0001X": specialty=EnumXConnectProviderSpecialty.RESTO; break;// Restorative
				case "124Q00000X": specialty=EnumXConnectProviderSpecialty.HYG; break;
				case "1223D0001X": specialty=EnumXConnectProviderSpecialty.PUBLIC; break;
				case "1223E0200X": specialty=EnumXConnectProviderSpecialty.ENDO; break;
				//case "1223P0106X": specialty=""; break;//pathology
				//case "1223X0008X": specialty=""; break;//radiology
				//case "1223S0112X": specialty=""; break;//surgery
				case "1223X0400X": specialty=EnumXConnectProviderSpecialty.ORTHO; break;
				case "1223P0221X": specialty=EnumXConnectProviderSpecialty.PEDI; break;
				case "1223P0300X": specialty=EnumXConnectProviderSpecialty.PERI; break;
				case "1223P0700X": specialty=EnumXConnectProviderSpecialty.PROS; break;//Prosth
				case "122400000X": specialty=EnumXConnectProviderSpecialty.DTS; break;//Denturist
				//case "126800000X": specialty=""; break;//Assistant
				//case "126900000X": specialty=""; break;//LabTech
				case "122300000X": specialty=EnumXConnectProviderSpecialty.DEN; break;
				case "1223P0106X": specialty=EnumXConnectProviderSpecialty.ORALMAXP; break;//Oral & Maxillofacial Pathologist 
				case "1223X0008X": specialty=EnumXConnectProviderSpecialty.ORALMAXR; break;//Oral & Maxillofacial Radiologist 
				case "204E00000X": specialty=EnumXConnectProviderSpecialty.ORALMAX1; break;//Oral & Maxillofacial Surgeon 
				case "261QS0112X": specialty=EnumXConnectProviderSpecialty.ORALMAXC; break;//Oral & Maxillofacial Surgery Clinic 
			}
			return specialty;
		}

		///<summary>Turns Claim.PatRelat into an acceptable XConnect patient relationship</summary>
		public static string GetXConnectPatientRelation(Relat relat) {
			string relationship="";
			switch (relat) {
				case Relat.Spouse:
					relationship="01";
					break;
				case Relat.Self:
					relationship="18";
					break;
				case Relat.Child:
					relationship="19";
					break;
				case Relat.Employee:
					relationship="20";
					break;
				case Relat.HandicapDep:
					relationship="22";
					break;
				case Relat.SignifOther:
					relationship="29";
					break;
				case Relat.InjuredPlaintiff:
					relationship="41";
					break;
				case Relat.LifePartner:
					relationship="53";
					break;
				case Relat.Dependent:
					relationship="76";
					break;
				default:
					relationship="20";//Unknown
					break;
			}
			return relationship;
		}
	}

	///<summary></summary>
	public class XConnectAddress {
		///<summary>Required.</summary>
		public string address1;
		///<summary>Optional.</summary>
		public string address2;
		///<summary>Required.</summary>
		public string city;
		///<summary>Required.</summary>
		public string state;
		///<summary>Required.</summary>
		public string zipCode;
		///<summary>Optional.</summary>
		public string country;
		///<summary>Optional.</summary>
		public string phone;
		///<summary>Optional.</summary>
		public string phoneExt;
		///<summary>Optional.</summary>
		public string fax;
		///<summary>Optional.</summary>
		public string organizationName;
		///<summary>Optional.</summary>
		public string firstName;
		///<summary>Optional.</summary>
		public string middleName;
		///<summary>Optional.</summary>
		public string lastName;
		///<summary>Required.</summary>
		public string entityType;
		///<summary>Optional.</summary>
		public string type;

		///<summary>Mimics billing address logic inside X837_5010.</summary>
		public static XConnectAddress BillingAddressFromClinic(Clinic clinic,Provider providerBill) {
			XConnectAddress xconnectAddress=new XConnectAddress();
			if(clinic!=null && clinic.UseBillAddrOnClaims) {
				xconnectAddress.address1=clinic.BillingAddress;
				xconnectAddress.address2=clinic.BillingAddress2;
				xconnectAddress.city=clinic.BillingCity;
				xconnectAddress.state=clinic.BillingState;
				xconnectAddress.zipCode=clinic.BillingZip;
			}
			else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
				xconnectAddress.address1=PrefC.GetString(PrefName.PracticeBillingAddress);
				xconnectAddress.address2=PrefC.GetString(PrefName.PracticeBillingAddress2);
				xconnectAddress.city=PrefC.GetString(PrefName.PracticeBillingCity);
				xconnectAddress.state=PrefC.GetString(PrefName.PracticeBillingST);
				xconnectAddress.zipCode=PrefC.GetString(PrefName.PracticeBillingZip);
			}
			else if(clinic==null) {
				xconnectAddress.address1=PrefC.GetString(PrefName.PracticeAddress);
				xconnectAddress.address2=PrefC.GetString(PrefName.PracticeAddress2);
				xconnectAddress.city=PrefC.GetString(PrefName.PracticeCity);
				xconnectAddress.state=PrefC.GetString(PrefName.PracticeST);
				xconnectAddress.zipCode=PrefC.GetString(PrefName.PracticeZip);
				
			}
			else {
				xconnectAddress.address1=clinic.Address;
				xconnectAddress.address2=clinic.Address2;
				xconnectAddress.city=clinic.City;
				xconnectAddress.state=clinic.State;
				xconnectAddress.zipCode=clinic.Zip;
				xconnectAddress.phone=clinic.Phone;
				//xconnectAddress.phoneExt="";//We don't havea field for this.
				xconnectAddress.fax=clinic.Fax;
			}
			if(providerBill.IsNotPerson) {
				xconnectAddress.entityType=POut.Int((int)EnumXConnectAddressEntityType.Organization);
			}
			else {
				xconnectAddress.entityType=POut.Int((int)EnumXConnectAddressEntityType.Individual);
				xconnectAddress.firstName=providerBill.FName;
				xconnectAddress.lastName=providerBill.LName;
			}
			xconnectAddress.type=POut.Int((int)EnumXConnectAddressType.Default);//Default. Never a paytoAddress always a physical address.
			return xconnectAddress;
		}

		public static XConnectAddress PayToAddressFromClinic(Clinic clinic,Provider providerBill) {
			XConnectAddress xconnectAddress=new XConnectAddress();
			if(clinic!=null && clinic.PayToAddress!="") {
				xconnectAddress.address1=clinic.PayToAddress;
				xconnectAddress.address2=clinic.PayToAddress2;
				xconnectAddress.city=clinic.PayToCity;
				xconnectAddress.state=clinic.PayToState;
				xconnectAddress.zipCode=clinic.PayToZip;
				xconnectAddress.phone=clinic.Phone;
				xconnectAddress.fax=clinic.Fax;
			}
			else { //Use the practice PayToAddress if clinic is null or the clinic's PayToAddress is blank.
				if(PrefC.GetString(PrefName.PracticePayToAddress)!="") {
					xconnectAddress.address1=PrefC.GetString(PrefName.PracticePayToAddress);
					xconnectAddress.address2=PrefC.GetString(PrefName.PracticePayToAddress2);
					xconnectAddress.city=PrefC.GetString(PrefName.PracticePayToCity);
					xconnectAddress.state=PrefC.GetString(PrefName.PracticePayToST);
					xconnectAddress.zipCode=PrefC.GetString(PrefName.PracticePayToZip);
					xconnectAddress.phone=PrefC.GetString(PrefName.PracticePhone);
					xconnectAddress.fax=PrefC.GetString(PrefName.PracticeFax);
				}
			}
			//xconnectAddress.phoneExt=""
			xconnectAddress.entityType=POut.Int((int)EnumXConnectAddressEntityType.Individual);
			if(providerBill.IsNotPerson) {
				xconnectAddress.entityType=POut.Int((int)EnumXConnectAddressEntityType.Organization);
			}
			xconnectAddress.type=POut.Int((int)EnumXConnectAddressType.PayToAddress);
			return xconnectAddress;
		}

		///<summary></summary>
		public static XConnectAddress FromPatient(Patient patient) {
			XConnectAddress xconnectAddress=new XConnectAddress();
			xconnectAddress.address1=patient.Address;
			xconnectAddress.address2=patient.Address2;
			xconnectAddress.city=patient.City;
			xconnectAddress.state=patient.State;
			xconnectAddress.zipCode=patient.Zip;
			xconnectAddress.country=patient.Country;
			xconnectAddress.phone=patient.WirelessPhone;
			xconnectAddress.phoneExt=null;//We don't store this for patients
			//xconnectAddress.fax=//We don't store this for patients
			//xconnectAddress.organizationName=//Not needed for individuals.
			xconnectAddress.firstName=patient.FName;
			xconnectAddress.middleName=patient.MiddleI;
			xconnectAddress.lastName=patient.LName;
			xconnectAddress.entityType=POut.Int((int)EnumXConnectAddressEntityType.Individual);
			xconnectAddress.type=POut.Int((int)EnumXConnectAddressType.Default);//Mark as Default instead of PayToAddress
			return xconnectAddress;
		}

		public static XConnectAddress FromCarrier(Carrier carrier) {
			XConnectAddress xconnectAddress=new XConnectAddress();
			xconnectAddress.address1=carrier.Address;
			xconnectAddress.address2=carrier.Address2;
			xconnectAddress.city=carrier.City;
			xconnectAddress.state=carrier.State;
			xconnectAddress.zipCode=carrier.Zip;
			//xconnectAddress.country=//We don't store this
			xconnectAddress.phone=carrier.Phone;
			//xconnectAddress.phoneExt=//We don't store this
			//xconnectAddress.fax=;//We don't store this
			xconnectAddress.organizationName=carrier.CarrierName;
			//xconnectAddress.firstName=//Not needed for organizations.
			//xconnectAddress.middleName=//Not needed for organizations.
			//xconnectAddress.lastName=//Not needed for organizations.
			xconnectAddress.entityType=POut.Int((int)EnumXConnectAddressEntityType.Organization);
			xconnectAddress.type=POut.Int((int)EnumXConnectAddressType.Default);//Mark as Default instead of PayToAddress
			return xconnectAddress;
		}

	}

	///<summary></summary>
	public class XConnectAttachmentStatus {
		///<summary>Optional.</summary>
		public bool hasAttachment;
		///<summary>Optional.</summary>
		public string dxcAttachmentId;
		///<summary>Optional.</summary>
		public string viewedDate;
		///<summary>Optional.</summary>
		public int status;
		///<summary>Optional.</summary>
		public string description;
	}

	///<summary></summary>
	public class XConnectCategoryCode {
		///<summary>Optional.</summary>
		public string code;
		///<summary>Optional.</summary>
		public string statusCode;
		///<summary>Optional.</summary>
		public string entityCode;
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/ClaimModel</summary>
	public class XConnectClaim {
		///<summary>Required.</summary>
		public XConnectProvider[] providers;
		///<summary>Required.</summary>
		public XConnectPayer payer;
		///<summary>Required.</summary>
		public XConnectPatient patient;
		///<summary>Required.</summary>
		public XConnectPatient subscriber;
		///<summary>Optional.</summary>
		public XConnectPatient[] additionalSubscribers;
		///<summary>Required.</summary>
		public XConnectClaimItem[] items;
		///<summary>Required.</summary>
		public EnumXConnectClaimType type;
		///<summary>Optional. This is DentalXChange’s account number, it’s called Group ID (From Email). We will never need this because it is inferred from API key.</summary>
		public int? dxcGroupId=null;
		///<summary>Optional. Is when sending a claim that is a followup to a PreAuth?</summary>
		public string dxcClaimId;
		///<summary>Required.</summary>
		public string providerClaimId;
		///<summary>Optional.</summary>
		public string dxcAttachmentId;
		///<summary>Required.</summary>
		public bool infoSign;
		///<summary>Required.</summary>
		public bool benefitSign;
		///<summary>Optional.</summary>
		public string neaNumber;
		///<summary>Optional.</summary>
		public string facilityCode;
		///<summary>Optional.</summary>
		public string facilityName;
		///<summary>Optional.</summary>
		public string facilityId;
		///<summary>Optional.</summary>
		public string facilityIdType;
		///<summary>Optional.</summary>
		public string accidentCode;
		///<summary>Optional.</summary>
		public string accidentDate;
		///<summary>Optional.</summary>
		public string acciendentState;
		///<summary>Optional.</summary>
		public string orthoPlacementDate;
		///<summary>Optional.</summary>
		public string orthoTreatmentMonths;
		///<summary>Optional.</summary>
		public string orthoRemainingMonths;
		///<summary>Optional.</summary>
		public bool orthoRelated;
		///<summary>Optional.</summary>
		public string preAuthorizationId;
		///<summary>Optional.</summary>
		public string refferalId; //double check
		///<summary>Optional.</summary>
		public string note;
		///<summary>Optional.</summary>
		public string delayReasonCode;
		///<summary>Optional.</summary>
		public string[] missingTeeth;
		///<summary>Optional.</summary>
		public string[] diagnosisCodes;
		///<summary>Optional.</summary>
		public int? submissionReasonCode=null;
		///<summary>Optional. This is the payer’s assigned unique id for each claim, and is used for submission of claim following a preauthorization (this is not needed for Claim Validation) (From Email).</summary>
		public string documentControlNumber;

		///<summary></summary>
		public static XConnectClaim FromClaim(Claim claim) {
			XConnectClaim xconnectClaim=new XConnectClaim();
			Patient patient=Patients.GetPat(claim.PatNum);
			Clinic clinic=Clinics.GetClinic(claim.ClinicNum);
			Carrier carrier=Carriers.GetCarrier(XConnect._insPlanForClaim.CarrierNum);
			InsSub insSub=XConnect._listInsSubs.FirstOrDefault(x=>x.InsSubNum==claim.InsSubNum);
			Provider providerBill=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvBill);
			Provider providerRendering=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvTreat);
			XConnectProvider xConnectProviderBill=XConnectProvider.FromProvider(providerBill,clinic,carrier.ElectID,EnumXConnectProviderType.BILLING);
			XConnectProvider xConnectProviderRendering=XConnectProvider.FromProvider(providerRendering,clinic,carrier.ElectID,EnumXConnectProviderType.RENDERING);
			xconnectClaim.providers=new XConnectProvider[] { xConnectProviderBill,xConnectProviderRendering };
			PatPlan patPlan=XConnect._listPatPlans.FirstOrDefault(x => x.InsSubNum==insSub.InsSubNum);
			xconnectClaim.patient=XConnectPatient.FromPatient(patient,claim.InsSubNum,patPlan,EnumXConnectPatientMemberType.PATIENT,claim.PatRelat);
			xconnectClaim.subscriber=XConnectPatient.FromPatient(patient,claim.InsSubNum,patPlan,EnumXConnectPatientMemberType.SUBSCRIBER,claim.PatRelat);
			List<XConnectPatient> listXConnectPatientsAdditionalSubs=new List<XConnectPatient>();
			List<long> listPatNums=new List<long>() { patient.PatNum };
			List<InsSub> listInsSubsAdditional=InsSubs.GetListInsSubs(listPatNums);
			List<InsPlan> listInsPlans=InsPlans.GetPlans(listInsSubsAdditional.Select(x => x.PlanNum).ToList());
			listInsPlans.RemoveAll(x => x.CarrierNum==XConnect._insPlanForClaim.CarrierNum);
			if(listInsPlans.Count>0) {
				for(int i=0;i<listInsPlans.Count;i++) {
					string carrierAdditional=Carriers.GetName(listInsPlans[i].CarrierNum);
					InsSub insSubSuscriber=listInsSubsAdditional.FirstOrDefault(x => x.Subscriber==patient.PatNum);
					PatPlan patPlanAdditional=XConnect._listPatPlans.FirstOrDefault(x => x.InsSubNum==insSubSuscriber.InsSubNum);
					XConnectPatient xConnectPatient=XConnectPatient.FromPatient(patient,insSubSuscriber.InsSubNum,patPlanAdditional,EnumXConnectPatientMemberType.ADDITIONAL_SUBSCRIBER,claim.PatRelat);
					if(xConnectPatient.sequenceCode=="") {
						//XConnect cannot handle beyond Tertiary claims.
						continue;
					}
					listXConnectPatientsAdditionalSubs.Add(xConnectPatient);
				}
				xconnectClaim.additionalSubscribers=listXConnectPatientsAdditionalSubs.ToArray();
			}
			xconnectClaim.type=EnumXConnectClaimType.CL;
			if(claim.ClaimType=="PreAuth") {
				xconnectClaim.type=EnumXConnectClaimType.PB;
			}
			//xconnectClaim.dxcClaimId=//TODO: Claim.dxcClaimId is optional. Is when sending a claim that is a followup to a PreAuth?
			xconnectClaim.providerClaimId=Claims.ConvertClaimId(claim,patient);
			if(!string.IsNullOrWhiteSpace(claim.AttachmentID) && !claim.AttachmentID.StartsWith("NEA#")) {
				xconnectClaim.dxcAttachmentId=claim.AttachmentID;
			}
			xconnectClaim.benefitSign=Claims.GetAssignmentOfBenefits(claim,insSub);
			xconnectClaim.infoSign=insSub.ReleaseInfo;
			if(claim.AttachmentID.StartsWith("NEA#")) {
				xconnectClaim.neaNumber=claim.AttachmentID;
			}
			xconnectClaim.facilityCode=X12object.GetPlaceService(claim.PlaceService);
			xconnectClaim.facilityName=providerBill.LName;
			xconnectClaim.facilityId=providerBill.NationalProvID;
			//xconnectClaim.facilityIdType="XX";//NPI
			List<XConnectClaimItem> listXConnectClaimItems=XConnectClaimItem.FromClaim(claim,xconnectClaim.facilityId);
			xconnectClaim.items=listXConnectClaimItems.ToArray();
			bool hasAdjustments=listXConnectClaimItems.Any(x => x.adjustments!=null);
			xconnectClaim.payer=XConnectPayer.FromCarrier(XConnect._carrierForClaim,patient,hasAdjustments);
			if(claim.AccidentRelated=="A") {
				xconnectClaim.accidentCode="AA";
			}
			else if(claim.AccidentRelated=="E") {
				xconnectClaim.accidentCode="EM";
			}
			else if(claim.AccidentRelated=="O") {
				xconnectClaim.accidentCode="OA";
			}
			if(claim.AccidentDate.Year>1880) {
				xconnectClaim.accidentDate=claim.AccidentDate.ToString("yyyy-MM-dd");
			}
			xconnectClaim.acciendentState=claim.AccidentST;
			List<long> listPatNumsForGuarantors=new List<long>();
			List<long> listGuarantors=Patients.GetGuarantorsForPatNums(listPatNumsForGuarantors);
			if(listGuarantors.Count==0) {//They are their own guarantor
				listGuarantors.Add(claim.PatNum);
			}
			PatientNote patientNote=PatientNotes.Refresh(claim.PatNum,listGuarantors[0]);
			xconnectClaim.orthoRelated=claim.IsOrtho;
			if(claim.IsOrtho) {
				if(claim.OrthoDate.Year>1880) {
					xconnectClaim.orthoPlacementDate=claim.OrthoDate.ToString("yyyy-MM-dd");
				}
				if(claim.OrthoTotalM>0) {
					xconnectClaim.orthoTreatmentMonths=claim.OrthoTotalM.ToString();
				}
				if(claim.OrthoRemainM>0) {
					xconnectClaim.orthoRemainingMonths=claim.OrthoRemainM.ToString();
				}
			}
			if(!claim.PriorAuthorizationNumber.IsNullOrEmpty()) {
				xconnectClaim.preAuthorizationId=claim.PriorAuthorizationNumber;
			}
			//xconnectClaim.refferalId=claim.OrigRefNum;//TODO
			if(!claim.ClaimNote.IsNullOrEmpty()) {
				xconnectClaim.note=claim.ClaimNote;
			}
			//xconnectClaim.delayReasonCode=//Optional. We do not have this available.
			List<ToothInitial> listToothInitials=ToothInitials.GetPatientData(claim.PatNum);
			listToothInitials.RemoveAll(x => x.InitialType!=ToothInitialType.Missing);
			xconnectClaim.missingTeeth=listToothInitials.Select(x => x.ToothNum.ToString()).ToArray();
			//diagnosis codes
			xconnectClaim.diagnosisCodes=XConnect._listDiagnoses.ToArray();
			EnumXConnectSubmissionReasonCode enumXConnectSubmissionReasonCode=EnumXConnectSubmissionReasonCode.Original;
			if(claim.CorrectionType==ClaimCorrectionType.Void) {
				enumXConnectSubmissionReasonCode=EnumXConnectSubmissionReasonCode.Void;
			}
			else if(claim.CorrectionType==ClaimCorrectionType.Replacement) {
				enumXConnectSubmissionReasonCode=EnumXConnectSubmissionReasonCode.Replacement;
			}
			xconnectClaim.submissionReasonCode=(int)enumXConnectSubmissionReasonCode;
			//xconnectClaim.documentControlNumber=//TODO.Optional.Not sure how this is different than the providerClaimID.
			return xconnectClaim;
		}

	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/ClaimItemModel This represents a procedure attached to the claim.</summary>
	public class XConnectClaimItem {
		///<summary>Optional</summary>
		public string controlNumber;
		///<summary>Optional</summary>
		public string startDate;
		///<summary>Optional</summary>
		public string endDate;
		///<summary>Required</summary>
		public int quantity;
		///<summary>Required</summary>
		public double fee;
		///<summary>Optional</summary>
		public double? tax=null;
		///<summary>Optional</summary>
		public string prothesisCode;//Only for medical
		///<summary>Optional</summary>
		public string prothesisPlacementDate;//Only for medical
		///<summary>Optional</summary>
		public string quadrant;
		///<summary>Required</summary>
		public string procedureCode;
		///<summary>Optional</summary>
		public string procedureModifier1;
		///<summary>Optional</summary>
		public string procedureModifier2;
		///<summary>Optional</summary>
		public string procedureModifier3;
		///<summary>Optional</summary>
		public string procedureModifier4;
		///<summary>Optional.This is NPI or TIN that identifies the facility / practice location (From Email).</summary>
		public string facilityIdentifier;
		///<summary>Optional</summary>
		public string comment;
		///<summary>Optional.This is for claims with more than one payer. 
		///This is the amount primary payer paid on the claim, when submitting to secondary payer (From Email). </summary>
		public double? amountPaidByPayer=null;
		///<summary>Optional.Date of Payment from amountPaidByPayer (From Email).</summary>
		public string dateOfPayment;
		///<summary>Optional</summary>
		public XConnectTooth[] tooth;
		///<summary>Optional</summary>
		public int[] diagnosisPointers;
		///<summary>Optional.Adjustments based on payment from primary (From Email).
		///DentalXChange says this is an array of Objects but their API says it is a singular object.</summary>
		public XConnectClaimItemAdjustment adjustments;

		///<summary></summary>
		public static List<XConnectClaimItem> FromClaim(Claim claim, string claimFacillityID) {
			List<XConnectClaimItem> listXConnectClaimItems=new List<XConnectClaimItem>();
			List<ProcedureCode> listProcedureCodes=ProcedureCodes.GetAllCodes();
			Patient patient=Patients.GetPat(claim.PatNum);
			List<PatPlan> listPatPlans=PatPlans.GetPatientData(claim.PatNum);
			InsPlan insPlan=insPlan=InsPlans.GetPlan(claim.PlanNum,new List<InsPlan>());
			Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			double claimWriteoffAmt=0;
			double claimDeductibleAmt=0;
			double claimPaidOtherInsAmt=0;
			List<long> listProcNums=XConnect._listClaimProcsForClaim.Select(x => x.ProcNum).Distinct().ToList();
			List<long> listOtherClaimNums=XConnect._listClaimProcsForPat.FindAll(x => x.ClaimNum!=claim.ClaimNum 
				&& x.Status.In(ClaimProcStatus.CapClaim, ClaimProcStatus.Received, ClaimProcStatus.Supplemental)
				&& listProcNums.Contains(x.ProcNum)).Select(x => x.ClaimNum).Distinct().ToList();
			List<ClaimProc> listClaimProcsOther=XConnect._listClaimProcsForPat.FindAll(x => listOtherClaimNums.Contains(x.ClaimNum) && x.ProcNum!=0);
			for(int i=0;i<XConnect._listClaimProcsForClaim.Count;i++) {
				Procedure proc=XConnect._listProceduresForPat.FirstOrDefault(x => x.ProcNum==XConnect._listClaimProcsForClaim[i].ProcNum);
				XConnectClaimItem xconnectClaimItem=new XConnectClaimItem();
				int ordinal=PatPlans.GetOrdinal(claim.InsSubNum,listPatPlans);
				xconnectClaimItem.controlNumber=("x"+proc.ProcNum.ToString()+"/"+ordinal+"/"+insPlan.PlanNum);//Mimics X12 field REF02 version 3
				if(xconnectClaimItem.controlNumber.Length>30) {//copied from X837_5010.cs
					//Even though the field allows 1-50 characters the 837 5010 documentation states:
					//"... the HIPAA maximum requirements to be supported by any reciving system is '30'.
					//Characters beyond 30 are not required to be stored nor returned by any 837-receiving system." page 438 in 837 standard.
					int overflowCount=(xconnectClaimItem.controlNumber.Length-30);
					string insPlanRightMost=insPlan.PlanNum.ToString().Substring(overflowCount);//Remove the leading digits, returns right most digits.
					xconnectClaimItem.controlNumber=("y"+proc.ProcNum.ToString()+"/"+ordinal+"/"+insPlanRightMost);
					//Version 4: Implemented in 19.1,18.4,18.3
				}
				if(proc.ProcDate.Year>1880) {
					xconnectClaimItem.startDate=proc.ProcDate.ToString("yyyy-MM-dd");
					xconnectClaimItem.endDate=proc.ProcDate.ToString("yyyy-MM-dd");
				}
				xconnectClaimItem.quantity=Int32.Parse(proc.Quantity.ToString());
				xconnectClaimItem.fee=XConnect._listClaimProcsForClaim[i].FeeBilled;
				//xconnectClaimItem.tax=proc.TaxAmt//user cannot enter tax. Avatax is internal only.
				if(proc.Prosthesis!="") {
					xconnectClaimItem.prothesisCode=proc.Prosthesis;
				}
				if(proc.DateOriginalProsth.Year > 1880) {
					xconnectClaimItem.prothesisPlacementDate=proc.DateOriginalProsth.ToString("yyyy-MM-dd");
				}
				string quadrant=ConvertProcedureToXConnectQuadrant(proc);
				xconnectClaimItem.quadrant=quadrant==""?null:quadrant;
				xconnectClaimItem.procedureCode=XConnect._listClaimProcsForClaim[i].CodeSent;
				xconnectClaimItem.procedureModifier1=string.IsNullOrEmpty(proc.CodeMod1)?null:proc.CodeMod1;
				xconnectClaimItem.procedureModifier2=string.IsNullOrEmpty(proc.CodeMod2)?null:proc.CodeMod2;
				xconnectClaimItem.procedureModifier3=string.IsNullOrEmpty(proc.CodeMod3)?null:proc.CodeMod3;
				xconnectClaimItem.procedureModifier4=string.IsNullOrEmpty(proc.CodeMod4)?null:proc.CodeMod4;
				//xconnectClaimItem.facilityIdentifier=claimFacillityID;
				if(proc.SiteNum>0) {
					Site site=Sites.GetFirstOrDefault(x => x.SiteNum==proc.SiteNum);
					Provider provider=XConnect._listProvidersForClaim.FirstOrDefault(x => x.ProvNum==site.ProvNum);
					if(provider!=null) {
						xconnectClaimItem.facilityIdentifier=provider.NationalProvID;
					}
				}
				if(!proc.Note.IsNullOrEmpty()) {
					xconnectClaimItem.comment=proc.Note;
				}
				List<ClaimProc> listOtherClaimProcsForItem=listClaimProcsOther.FindAll(x => x.ProcNum==proc.ProcNum);
				if(listOtherClaimProcsForItem.Count>0) {
					xconnectClaimItem.amountPaidByPayer=listOtherClaimProcsForItem.Sum(x => x.InsPayAmt);
					xconnectClaimItem.dateOfPayment=listOtherClaimProcsForItem.Max(x => x.DateCP).ToString("yyyy-MM-dd");
				}
				xconnectClaimItem.tooth=XConnectTooth.FromProc(proc).ToArray();
				List<int> listDiagnosisPointers=new List<int>();
				for(int j=0;j<XConnect._listDiagnoses.Count;j++) {
					if(XConnect._listDiagnoses[j]==xconnectClaimItem.procedureModifier1 
						|| XConnect._listDiagnoses[j]==xconnectClaimItem.procedureModifier2 
						|| XConnect._listDiagnoses[j]==xconnectClaimItem.procedureModifier3 
						|| XConnect._listDiagnoses[j]==xconnectClaimItem.procedureModifier4) 
					{
						listDiagnosisPointers.Add(j+1);
					}
				}
				xconnectClaimItem.diagnosisPointers=listDiagnosisPointers.ToArray();
				double procWriteoffAmt=0;
				double procDeductibleAmt=0;
				double procPaidOtherInsAmt=0;
				EclaimCobInsPaidBehavior cobBehavior=PrefC.GetEnum<EclaimCobInsPaidBehavior>(PrefName.ClaimCobInsPaidBehavior);
				if(carrier.CobInsPaidBehaviorOverride!=EclaimCobInsPaidBehavior.Default) {
					cobBehavior=carrier.CobInsPaidBehaviorOverride;
				}
				bool hasProcedureLevelCob=cobBehavior.In(EclaimCobInsPaidBehavior.ProcedureLevel,EclaimCobInsPaidBehavior.Both);
				XConnectClaimItemAdjustment xConnectClaimItemAdjustment=new XConnectClaimItemAdjustment();
				List<XConnectClaimItemAdjustmentDetail> listXConnectClaimItemAdjustmentDetails=new List<XConnectClaimItemAdjustmentDetail>();
				for(int k=0;k<listOtherClaimProcsForItem.Count;k++) {//All claim procs for patient
					if(ClaimProcs.IsValidClaimAdj(listOtherClaimProcsForItem[k],XConnect._listClaimProcsForClaim[i].ProcNum,XConnect._listClaimProcsForClaim[i].InsSubNum)) 
					{//Adjustment due to other insurance plans.
						XConnectClaimItemAdjustmentDetail xConnectClaimItemAdjustmentDetail=new XConnectClaimItemAdjustmentDetail();
						xConnectClaimItemAdjustmentDetail.adjustmentAmount=listOtherClaimProcsForItem[k].InsPayAmt;
						xConnectClaimItemAdjustmentDetail.quantity=proc.Quantity;
						listXConnectClaimItemAdjustmentDetails.Add(xConnectClaimItemAdjustmentDetail);
						if(hasProcedureLevelCob) {
							double procPatientPortionAmt=Math.Max(0,XConnect._listClaimProcsForClaim[i].FeeBilled-listOtherClaimProcsForItem[k].WriteOff-listOtherClaimProcsForItem[k].DedApplied-procPaidOtherInsAmt);
							//ClaimConnect sometimes expects zero value contractual obligations for line adjustments. Excluding them can cause an error on their end.
							if(procWriteoffAmt>0 || claimWriteoffAmt>0) {
								xConnectClaimItemAdjustmentDetail.reasonCode="45";//CAS02 1/5 Claim Adjustment Reason Code: 45=Charge exceeds fee schedule/maximum allowable or contracted/legislated fee arrangement.
							}
							if(procDeductibleAmt>0) {
								xConnectClaimItemAdjustmentDetail.reasonCode="1";//CAS02 1/5 Claim Adjustment Reason Code: 1=Deductible.
							}
							if(procPatientPortionAmt>0) {
								xConnectClaimItemAdjustmentDetail.reasonCode="3";//CAS02 or CAS05 1/5 Claim Adjustment Reason Code: 3=Co-payment Amount.
							}
						}
					}
				}
				if(listXConnectClaimItemAdjustmentDetails.Count>0) {
					xconnectClaimItem.adjustments=xConnectClaimItemAdjustment;
				}
				listXConnectClaimItems.Add(xconnectClaimItem);
			}
			return listXConnectClaimItems;
		}

		///<summary>Taken from X837_5010.cs</summary>
		private static string GetArea(Procedure proc,ProcedureCode procCode) {
			//"Required when the nomenclature associated with the procedure reported in SV301-2 refers to quadrant or arch
			//and the area of the oral cavity is not uniquely defined by the procedure description.
			//Report individual tooth numbers in one or more TOO segments.
			//Do not use this element for reporting of individual teeth.
			//If it is necessary to report one or more individual teeth, use the Tooth Information (TOO) segment in this loop."
			if(procCode.TreatArea==TreatmentArea.Arch) {
				if(proc.Surf=="U") {
					return "01";
				}
				if(proc.Surf=="L") {
					return "02";
				}
			}
			if(procCode.TreatArea==TreatmentArea.Mouth){
				return "00";
			}
			if(procCode.TreatArea==TreatmentArea.None){
				return "";
			}
			if(procCode.TreatArea==TreatmentArea.Quad) {
				if(proc.Surf=="UR") {
					return "10";
				}
				if(proc.Surf=="UL") {
					return "20";
				}
				if(proc.Surf=="LR") {
					return "40";
				}
				if(proc.Surf=="LL") {
					return "30";
				}
			}
			if(procCode.TreatArea==TreatmentArea.Sextant) {
				return "";
			}
			if(procCode.TreatArea==TreatmentArea.Surf) {
				return "";
			}
			if(procCode.TreatArea==TreatmentArea.Tooth) {
				return "";
			}
			if(procCode.TreatArea==TreatmentArea.ToothRange) {
				return "";
			}
			return "";
		}

		/// <summary>Taken and modified from FormClaimPrint.</summary>
		public static string ConvertProcedureToXConnectQuadrant(Procedure proc) {
			ProcedureCode procCode=ProcedureCodes.GetFirstOrDefault(x => x.CodeNum==proc.CodeNum);
			string area=GetArea(proc,procCode);
			return area;
		}
	}

	public class XConnectClaimItemAdjustment {
		///<summary>Required.</summary>
		public EnumXConnectAdjustmentGroupCode adjustmentGroupCode;
		///<summary>Optional.</summary>
		public XConnectClaimItemAdjustmentDetail[] adjustmentDetails;
		public XConnectClaimItemAdjustment FromAdjustment(Adjustment adjustment) {
			XConnectClaimItemAdjustment xconnectClaimItemAdjustment = new XConnectClaimItemAdjustment();
			return xconnectClaimItemAdjustment;
		}
	}

	public class XConnectClaimItemAdjustmentDetail {
		///<summary>Required.<summary>
		public string reasonCode;
		///<summary>Optional.<summary>
		public double? quantity=null;
		///<summary>Required.<summary>
		public double adjustmentAmount;

	}

	///<summary></summary>
	public class XConnectClaimItemResponse {
		///<summary>Optional.</summary>
		public string controlNumber;
		///<summary>Optional.</summary>
		public string quantity;
		///<summary>Optional.</summary>
		public string startDate;
		///<summary>Optional.</summary>
		public string endDate;
		///<summary>Optional.</summary>
		public XConnectStatus itemStatus;
	}

	///<summary></summary>
	public class  XConnectClaimStatus {
		///<summary>Optional.</summary>
		public string dxcClaimId;
		///<summary>Optional.</summary>
		public XConnectStatus claimStatus;
		///<summary>Optional.</summary>
		public XConnectDXCStatus dxcStatus;
		///<summary>Optional.</summary>
		public XConnectAttachmentStatus attachmentStatus;
		///<summary>Optional.</summary>
		public XConnectClaimItemResponse[] claimItems;
		///<summary>Optional.</summary>
		public string[] warnings;
		///<summary>Optional.</summary>
		public string edi;
		///<summary>Optional.</summary>
		public string xml;
	}

	///<summary></summary>
	public class XConnectDXCStatus {
		///<summary>Optional.</summary>
		public XConnectClaimError[] claimErrors;
		///<summary>Optional.</summary>
		public XConnectClaimItemError[] claimItemErrors;
	}

	///<summary></summary>
	public class XConnectClaimError {
		///<summary>Optional.</summary>
		public string claimPart;
		///<summary>Optional.</summary>
		public string code;
		///<summary>Optional.</summary>
		public string description;
		///<summary>Optional.</summary>
		public string category;
		///<summary>Optional.</summary>
		public int statusCode;
		///<summary>Optional.</summary>
		public string entityId;
	}

	///<summary></summary>
	public class XConnectClaimItemError {
		///<summary>Optional.</summary>
		public int claimItem;
		///<summary>Optional.</summary>
		public string code;
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/PayerModel</summary>
	public class XConnectPayer {
		///<summary>Required</summary>
		public string payerIdCode;
		///<summary>Optional</summary>
		public XConnectAddress address;
		///<summary>Optional</summary>
		public string employerName;
		///<summary>Optional</summary>
		public XConnectPayerCob coordinationOfBenefits;

		public static XConnectPayer FromCarrier(Carrier carrier,Patient patient,bool hasAdjustments) {
			XConnectPayer xconnectPayer=new XConnectPayer();
			xconnectPayer.payerIdCode=carrier.ElectID;
			xconnectPayer.address=XConnectAddress.FromCarrier(carrier);
			xconnectPayer.employerName=Employers.GetName(patient.EmployerNum);
			if(hasAdjustments) {
				xconnectPayer.coordinationOfBenefits=XConnectPayerCob.FromClaim(XConnect._claim);
			}
			return xconnectPayer;
		}
	}

	///<summary></summary>
	public class XConnectPayerCob {
			///<summary>Required.</summary>
		public string datePaid;
			///<summary>Required.</summary>
		public double amountPaid;
			///<summary>Optional.</summary>
		public double amountPaidToPatient;
			///<summary>Optional.</summary>
		public double patientResponsibillity;
			///<summary>Optional.</summary>
		public double totalNonCoveredAmount;

		public static XConnectPayerCob FromClaim(Claim claim) {
			List<ClaimProc> listClaimProcsNotForClaim=XConnect._listClaimProcsForPat.FindAll(x => x.ClaimNum!=claim.ClaimNum &&
			XConnect._listClaimProcsForClaim.Any(y => y.ProcNum==x.ProcNum));
			DateTime datePaidOtherIns=listClaimProcsNotForClaim.Max(x => x.DateCP);
			ClaimProc claimProc=listClaimProcsNotForClaim.FirstOrDefault(x => x.Status.In(ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim,ClaimProcStatus.CapComplete));
			XConnectPayerCob xconnectPayerCob=new XConnectPayerCob();
			Claim claimOther=Claims.GetClaim(claimProc.ClaimNum);
			if(claimOther!=null) {
				xconnectPayerCob.datePaid=datePaidOtherIns.ToString("yyyy-MM-dd");
				xconnectPayerCob.amountPaid=claimOther.InsPayAmt;
			}
			return xconnectPayerCob;
		}
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/PatientModel
	//XConnect Patient object and Suscriber are the same. We have patient and subscriber to account for the X12 spec (From Email). ///</summary>
	public class XConnectPatient {
		///<summary>Optional.</summary>
		public XConnectAddress address;
		///<summary>Optional.</summary>
		public XConnectPayer payer;
		///<summary>Required.</summary>
		public EnumXConnectPatientMemberType memberType;
		///<summary>Required.</summary>
		public EnumXConnectPatientGender gender;
		///<summary>Required.</summary>
		public string memberId;
		///<summary>Required.</summary>
		public string dateOfBirth;
		///<summary>Required.</summary>
		public string sequenceCode;
		///<summary>Optional.</summary>
		public string planName;
		///<summary>Required.</summary>
		public string relationship;
		///<summary>Optional.</summary>
		public string groupNumber;
		///<summary>Optional.</summary>
		public string studentCode;
		///<summary>Optional.</summary>
		public string maritalStatus;
		///<summary>Optional.</summary>
		public string schoolName;
		///<summary>Optional.</summary>
		public string schoolCity;
		///<summary>Optional.</summary>
		public string schoolState;

		///<summary></summary>
		public static XConnectPatient FromPatient(Patient patient,long insSubNum,PatPlan patPlan,EnumXConnectPatientMemberType memberType,Relat relation) {
			InsSub insSub=XConnect._listInsSubs.FirstOrDefault(x => x.InsSubNum==insSubNum);
			XConnectPatient xconnectPatient=new XConnectPatient();
			xconnectPatient.address=XConnectAddress.FromPatient(patient);
			xconnectPatient.payer=XConnectPayer.FromCarrier(XConnect._carrierForClaim,patient,false);//TODO: When reporting the patient is this the plan of the carrier we are validating the claim for?
			xconnectPatient.memberType=memberType;
			string genderAbbrev=patient.Gender.ToString().Substring(0,1);
			EnumXConnectPatientGender gender=EnumXConnectPatientGender.U;
			switch(genderAbbrev) {
				case "M":
					gender=EnumXConnectPatientGender.M;
					break;
				case "F":
					gender=EnumXConnectPatientGender.F;
					break;
				case "U":
					gender=EnumXConnectPatientGender.U;
					break;
				default://Other gender and all other types
					gender=EnumXConnectPatientGender.U;
					break;
			}
			xconnectPatient.gender=gender;
			xconnectPatient.memberId=insSub.SubscriberID;
			xconnectPatient.dateOfBirth=patient.Birthdate.ToString("yyyy-MM-dd");//required
			xconnectPatient.sequenceCode="";
			if(patPlan.Ordinal==1) {
				xconnectPatient.sequenceCode="P";
			}
			else if(patPlan.Ordinal==2) {
				xconnectPatient.sequenceCode="S";
			}
			else if(patPlan.Ordinal==3) {
				xconnectPatient.sequenceCode="T";
			}
			List<long> listInsPlanNums=XConnect._listInsSubs.Select(x => x.PlanNum).ToList();
			List<InsPlan> listInsPlans=InsPlans.GetPlans(listInsPlanNums);
			if(listInsPlans.Count>0) {
				xconnectPatient.planName=Carriers.GetName(listInsPlans[0].CarrierNum);//not sure about this one might be another field I'm missing
			}
			xconnectPatient.relationship=XConnect.GetXConnectPatientRelation(relation);//TODO: When reporting the patient is this the plan of the carrier we are validating the claim for?
			xconnectPatient.studentCode=string.IsNullOrEmpty(patient.StudentStatus)?"N":patient.StudentStatus;
			string maritalStatus=patient.Position.ToString().Substring(0,1);
			maritalStatus=(maritalStatus!="M"||maritalStatus!="S")?"U":maritalStatus;//mark as unknown if not single or married
			xconnectPatient.maritalStatus=maritalStatus;
			xconnectPatient.schoolName=string.IsNullOrEmpty(patient.SchoolName)?null:patient.SchoolName;
			//xconnectPatient.schoolCity=;//Optional. We don't store this information in OD.
			//xconnectPatient.schoolState=;//Optional. We don't store this information in OD.
			return xconnectPatient;
		}

	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/ProviderModel</summary>
	public class XConnectProvider {
		///<summary>Required.</summary>
		public EnumXConnectProviderType type;
		///<summary>Required.</summary>
		public EnumXConnectProviderSpecialty specialty;
		///<summary>Optional.</summary>
		public string licenseNumber;
		///<summary>Optional.</summary>
		public string licenseState;
		///<summary>Optional. NPI needs to be reported in Billing or Rendering providers as in 5010 there is no NPI in PayTo loop (From Email).</summary>
		public string billingNpi;
		///<summary>Optional. NPI needs to be reported in Billing or Rendering providers as in 5010 there is no NPI in PayTo loop (From Email)</summary>
		public string renderingNpi;
		///<summary>Required.</summary>
		public string taxId;
		///<summary>Optional.</summary>
		public string socialSecurityNumber;
		///<summary>Required.</summary>
		public XConnectAddress[] addresses;
		///<summary>Optional.</summary>
		public XConnectProviderCredentials[] credentials;

		///<summary></summary>
		public static XConnectProvider FromProvider(Provider provider,Clinic clinic,string electId,EnumXConnectProviderType type) {
			XConnectProvider xconnectProvider=new XConnectProvider();
			xconnectProvider.type=type;
			xconnectProvider.specialty=XConnect.GetProviderSpecialty(provider);// need to convert this to he matching enums on documentation
			xconnectProvider.licenseNumber=provider.StateLicense;
			xconnectProvider.licenseState=provider.StateWhereLicensed;
			if(type==EnumXConnectProviderType.BILLING) {
				xconnectProvider.billingNpi=provider.NationalProvID;
			}
			else if(type==EnumXConnectProviderType.RENDERING) {
				xconnectProvider.renderingNpi=provider.NationalProvID;
			}
			//NPI needs to be reported in Billing or Rendering providers as in 5010 there is no NPI in PayTo loop. I will review and get back to you on referring provider.
			if(provider.UsingTIN) {
				xconnectProvider.taxId=provider.SSN;
			}
			else {
				xconnectProvider.socialSecurityNumber=provider.SSN;
			}
			List<XConnectAddress> listXConnectAddresses=new List<XConnectAddress>();
			XConnectAddress xconnectAddressBilling=XConnectAddress.BillingAddressFromClinic(clinic,provider);
			listXConnectAddresses.Add(xconnectAddressBilling);
			XConnectAddress xconnectAdddressPayTo;
			if(type==EnumXConnectProviderType.BILLING 
				&& (clinic!=null) 
				&& (clinic.PayToAddress!="" || PrefC.GetString(PrefName.PracticePayToAddress)!="")) {//Add PayToAddress for the billing provider
				xconnectAdddressPayTo=XConnectAddress.PayToAddressFromClinic(clinic,provider);
				listXConnectAddresses.Add(xconnectAdddressPayTo);
			}
			xconnectProvider.addresses=listXConnectAddresses.ToArray();
			List<XConnectProviderCredentials> listXConnectProviderCredentials=new List<XConnectProviderCredentials>();
			listXConnectProviderCredentials.Add(new XConnectProviderCredentials() { type="XX",value=provider.NationalProvID });
			ElectID electID=ElectIDs.GetID(electId);
			if(!provider.MedicaidID.IsNullOrEmpty()) {//TODO: Is MedProv Num for medicaid?
				if(electID!=null && electID.IsMedicaid) {
					listXConnectProviderCredentials.Add(new XConnectProviderCredentials { type="1D",value=provider.MedicaidID });
				}
			}
			if(provider.UsingTIN && !provider.SSN.IsNullOrEmpty()) {
				listXConnectProviderCredentials.Add(new XConnectProviderCredentials() { type="FI",value=provider.SSN });
			}
			if(!provider.StateLicense.IsNullOrEmpty()) {
				listXConnectProviderCredentials.Add(new XConnectProviderCredentials() { type="0B",value=provider.StateLicense });
			}
			else {
				ProviderIdent[] provIdents=ProviderIdents.GetForPayor(provider.ProvNum,electId);
				ProviderIdent provIdentBlueCross=provIdents.FirstOrDefault(x => x.SuppIDType==ProviderSupplementalID.BlueCross);
				if(provIdentBlueCross!=null) {
					listXConnectProviderCredentials.Add(new XConnectProviderCredentials{ type ="1A",value=provIdentBlueCross.IDNumber });
				}
				ProviderIdent proviIdentBlueShield=provIdents.FirstOrDefault(x => x.SuppIDType==ProviderSupplementalID.BlueShield);
				if(proviIdentBlueShield!=null) {
					listXConnectProviderCredentials.Add(new XConnectProviderCredentials{ type ="1B",value=proviIdentBlueShield.IDNumber });
				}
			}
			//We don't store Medicare IDs anywhere. This corredsponds to provider type 1C.
			xconnectProvider.credentials=listXConnectProviderCredentials.ToArray();
			return xconnectProvider;
		}
	}

	///<summary></summary>
	public class XConnectProviderCredentials {
		///<summary>Required.</summary>
		public string type;
		///<summary>Required.</summary>
		public string value;
	}

	///<summary></summary>
		public class XConnectResponseStatus {
		///<summary>Optional.</summary>
		public long code;
		///<summary>Optional.</summary>
		public string description;
	}

	///<summary></summary>
	public class XConnectStatus {
		///<summary>Optional.</summary>
		public double submittedAmount;
		///<summary>Optional.</summary>
		public double paidAmount;
		///<summary>Optional.</summary>
		public string statusDate;
		///<summary>Optional.</summary>
		public string adjudicationDate;
		///<summary>Optional.</summary>
		public string paymentMethod;
		///<summary>Optional.</summary>
		public string paymentDate;
		///<summary>Optional.</summary>
		public string checkNumber;
		///<summary>Optional.</summary>
		public string message;
		///<summary>Optional.</summary>
		public XConnectCategoryCode[] categoryCodes;
	}

	public class XConnectTooth {
		///<summary>Optional.</summary>
		public string surface;
		///<summary>Optional.</summary>
		public string toothNumber;

		public static List<XConnectTooth> FromProc(Procedure procedure) {
			List<XConnectTooth> listXConnectTeeth=new List<XConnectTooth>();
			ProcedureCode procedureCode=ProcedureCodes.GetFirstOrDefault(x => x.CodeNum==procedure.CodeNum);
			TreatmentArea treatmentArea=procedureCode.TreatArea;
			if(treatmentArea==TreatmentArea.Tooth) {
				XConnectTooth xconnectToothSingle=new XConnectTooth();
				if(!procedure.ToothNum.IsNullOrEmpty()) {
					xconnectToothSingle.toothNumber=procedure.ToothNum;
				}
				listXConnectTeeth.Add(xconnectToothSingle);
			}
			else if(treatmentArea==TreatmentArea.Surf) {
				XConnectTooth xconnectTooth=new XConnectTooth();
				if(!procedure.Surf.IsNullOrEmpty()) {
					if(procedure.Surf.Contains("B")) {
						xconnectTooth.surface="B";
					}
					else if(procedure.Surf.Contains("D")) {
						xconnectTooth.surface="D";
					}
					else if(procedure.Surf.Contains("F")) {
						xconnectTooth.surface="F";
					}
					else if(procedure.Surf.Contains("I")) {
						xconnectTooth.surface="I";
					}
					else if(procedure.Surf.Contains("L")) {
						xconnectTooth.surface="L";
					}
					else if(procedure.Surf.Contains("M")) {
						xconnectTooth.surface="M";
					}
					else if(procedure.Surf.Contains("O")) {
						xconnectTooth.surface="O";
					}
				}

			}
			else if(treatmentArea==TreatmentArea.ToothRange) {
				List<string> listToothNums=procedure.ToothRange.Split(',').ToList();
				for(int i=0;i<listToothNums.Count;i++) {
					XConnectTooth xconnectTooth=new XConnectTooth();
					if(!procedure.ToothNum.IsNullOrEmpty()) {
						xconnectTooth.toothNumber=procedure.ToothNum;
					}
					listXConnectTeeth.Add(xconnectTooth);
				}
			}
			return listXConnectTeeth;
		}

	}

	///<summary></summary>
	public class XConnectValidateClaim {
		///<summary>Required.</summary>
		public XConnectClaim claim;
		///<summary>Optional.</summary>
		public bool validateForAttachment;
		///<summary>Optional.</summary>
		public bool matchProvider;
		///<summary>Optional.</summary>
		public bool autoAddProvider;
		///<summary>Optional.</summary>
		public bool saveOnError;
		///<summary>Optional.</summary>
		public bool allowDuplicate;
		///<summary>Optional.</summary>
		public bool writeEdi;
		///<summary>Optional.</summary>
		public bool writeXml;
	}

	///<summary></summary>
	public class XConnectWebResponse {
		///<summary>Optional.</summary>
		public XConnectResponseStatus status;
		///<summary>Optional.</summary>
		public string[] messages;
		///<summary>Optional.</summary>
		public int transactionId;
		///<summary>Optional.</summary>
		public XConnectClaimStatus response;
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/ProviderModel</summary>
	public enum EnumXConnectAddressEntityType {
		/// <summary>Do Not Use.PlaceHolder</summary>
		None,
		///<summary>Individual</summary>
		Individual,
		///<summary>Organization</summary>
		Organization
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/ProviderModel</summary>
	public enum EnumXConnectAddressType {
		Default,
		PayToAddress
	}

	public enum EnumXConnectAdjustmentGroupCode {
		///<summary>Contractual</summary>
		CO,
		///<summary>Correction</summary>
		CR,
		///<summary>Other</summary>
		OA,
		///<summary>Payer Initiated</summary>
		PI,
		///<summary>Patient Responsibility</summary>
		PR
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/ClaimModel</summary>
	public enum EnumXConnectClaimType {
		///<summary>Claim</summary>
		CL,
		///<summary>Encounter</summary>
		EN,
		///<summary>PreTreatment</summary>
		PB,
		///<summary>Subrogation</summary>
		SU
	}

	public enum EnumXConnectPatientGender {
		///<summary>Male</summary>
		M,
		///<summary>Female</summary>
		F,
		///<summary>Unknown</summary>
		U
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/PatientModel</summary>
	public enum EnumXConnectPatientMemberType {
		PATIENT,
		SUBSCRIBER,
		ADDITIONAL_SUBSCRIBER
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/ProviderModel</summary>
	public enum EnumXConnectProviderType {
		BILLING,
		RENDERING,
		REFERRING,
		PAYTO
	}

	///<summary>https://developer.dentalxchange.com/claim-api#tag/ProviderModel</summary>
	public enum EnumXConnectProviderSpecialty {
		GP,
		ENDO,
		ORALMAX,
		ORTHO,
		PEDI,
		PERI,
		PROS,
		PUBLIC,
		HYG,
		DTS,
		DEN,
		ORALMAXP,
		ORALMAXR,
		MSPEC,
		SSPEC,
		IMP,
		OPER,
		RESTO,
		COS,
		DENT,
		ORALMAX1,
		ORALMAXC
	}

	/// <summary>1 = original,6 = corrected, 7 = replacement,8 = void</summary>
	public enum EnumXConnectSubmissionReasonCode {
		Original=1,
		Corrected=6,
		Replacement=7,
		Void=8
	}

}
