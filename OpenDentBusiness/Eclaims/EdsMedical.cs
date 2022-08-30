using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using CodeBase;
using OpenDentBusiness;
using Tamir.SharpSsh.jsch;

namespace OpenDentBusiness.Eclaims {
	class EdsMedical {
		
		public static string ErrorMessage="";

		///<summary></summary>
		public EdsMedical(){
		}

		///<summary>Returns true if the communications were successful, and false if they failed.  Both sends and retrieves.</summary>
		///<summary>Sends an X12 837 request (eclaim).</summary>
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
							+"<process>transmitMedicalClaim</process>"
							+"<version>1</version>"
						+"</header>"
						+"<body>"
							+"<type>EDI</type>"
							+"<data><![CDATA["+x837message.Replace("\r\n","").Replace("\n","")+"]]></data>"
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
	}
}
