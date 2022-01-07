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

namespace OpenDentBusiness.Eclaims {
	public class EDS {

		public static string ErrorMessage="";

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
		///No need to pass in a date as this web call, when clearinghouseClin.IsEraDownloadAllowed is enabled,
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
	}

}
