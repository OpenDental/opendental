using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;
using WebServiceSerializer;

namespace OpenDentBusiness {
	///<summary>This class provides helper methods when creating payloads to send to HQ hosted web services (e.g. WebServiceMainHQ.asmx and SheetsSynch.asmx)</summary>
	public class PayloadHelper {
		
		///<summary>Returns an XML payload that includes common information required by most HQ hosted services (e.g. reg key, program version, etc).</summary>
		///<param name="registrationKey">An Open Dental distributed registration key that HQ has on file.  Do not include hyphens.</param>
		///<param name="practiceTitle">Any string is acceptable.</param>
		///<param name="practicePhone">Any string is acceptable.</param>
		///<param name="programVersion">Typically major.minor.build.revision.  E.g. 12.4.58.0</param>
		///<param name="payloadContentxAsXml">Use CreateXmlWriterSettings(true) to create your payload xml. Outer-most xml element MUST be labeled 'Payload'.</param>
		///<param name="serviceCode">Used on case by case basis to validate that customer is registered for the given service.</param>
		///<returns>An XML string that can be passed into an HQ hosted web method.</returns>
		public static string CreatePayload(
			string payloadContentxAsXml,eServiceCode serviceCode,string registrationKey=null,string practiceTitle=null,string practicePhone=null,string programVersion=null) 
		{
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(false))) {
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

		public static string CreatePayloadWebHostSynch(string registrationKey,params PayloadItem[] listPayloadItems){
			return CreatePayload(PayloadHelper.CreatePayloadContent(listPayloadItems.ToList()),eServiceCode.WebHostSynch,registrationKey,"","","");
		}
		
		///<summary>Returns an XML payload that includes common information required by most HQ hosted services (e.g. reg key, program version, etc).</summary>
		///<param name="registrationKey">An Open Dental distributed registration key that HQ has on file.  Do not include hyphens.</param>
		///<param name="practiceTitle">Any string is acceptable.</param>
		///<param name="practicePhone">Any string is acceptable.</param>
		///<param name="programVersion">Typically major.minor.build.revision.  E.g. 12.4.58.0</param>
		///<param name="listPayloadItems">All items that need to be included with the payload.</param>
		///<param name="serviceCode">Used on case by case basis to validate that customer is registered for the given service.</param>
		///<returns>An XML string that can be passed into an HQ hosted web method.</returns>
		public static string CreatePayload(
			List<PayloadItem> listPayloadItems,eServiceCode serviceCode,string registrationKey=null,string practiceTitle=null,string practicePhone=null,string programVersion=null) 
		{
			return CreatePayload(PayloadHelper.CreatePayloadContent(listPayloadItems),serviceCode,registrationKey,practiceTitle,practicePhone,programVersion);
		}

		///<summary>Creates an XML string for the payload of the provided content. Currently only useful if you have one thing to include in the payload.
		///The root element will be Payload, followed by a tagName element, finished with the entire serialized version of content.</summary>
		public static string CreatePayloadContent(object content,string tagName) {
			return CreatePayloadContent(new List<PayloadItem>() { new PayloadItem(content,tagName) });
		}

		///<summary>Creates an XML string for the payload of the provided content. The list passed in is a tuple where the first item is the content to
		///be serialized and the second item is the tag name for the content.</summary>
		public static string CreatePayloadContent(List<PayloadItem> listPayloadItems) {
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(true))) {
				writer.WriteStartElement("Payload");
				foreach(PayloadItem payLoadItem in listPayloadItems) {
					XmlSerializer xmlListConfirmationRequestSerializer=new XmlSerializer(payLoadItem.Content.GetType());
					writer.WriteStartElement(payLoadItem.TagName);
					xmlListConfirmationRequestSerializer.Serialize(writer,payLoadItem.Content);
					writer.WriteEndElement();
				}
				writer.WriteEndElement(); //Payload	
			}
			return strbuild.ToString();
		}

		///<summary>Returns an XML string that contains the given content only.</summary>
		public static string CreateSuccessResponse<T>(T content) {
			StringBuilder strbuild=new StringBuilder();
			XmlSerializer xmlContentSerializer=new XmlSerializer(typeof(T));
			using(XmlWriter writer=XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(false))) {
				xmlContentSerializer.Serialize(writer,content);
			}
			return strbuild.ToString();
		}

		///<summary>Returns XML string with a Response element that contains the given content with nodeName. Called from OpenDentalWebApps.</summary>
		public static string CreateSuccessResponse<T>(T content,string nodeName) {
			StringBuilder strbuild=new StringBuilder();
			XmlSerializer xmlContentSerializer=new XmlSerializer(typeof(T));
			using(XmlWriter writer=XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(false))) {
				writer.WriteStartElement("Response");
				writer.WriteStartElement(nodeName);
				xmlContentSerializer.Serialize(writer,content);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			return strbuild.ToString();
		}

		///<summary>Creates an XML string with a Response element for each of the provided content. The list passed in is a tuple where the first item is
		///the content to be serialized and the second item is the tag name for the content.</summary>
		public static string CreateSuccessResponse(List<PayloadItem> listPayloadItems) {
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(true))) {
				writer.WriteStartElement("Response");
				foreach(PayloadItem payLoadItem in listPayloadItems) {
					XmlSerializer xmlListConfirmationRequestSerializer=new XmlSerializer(payLoadItem.Content.GetType());
					writer.WriteStartElement(payLoadItem.TagName);
					xmlListConfirmationRequestSerializer.Serialize(writer,payLoadItem.Content);
					writer.WriteEndElement();
				}
				writer.WriteEndElement(); //Response	
			}
			return strbuild.ToString();
		}

		///<summary>Returns Response-Error payload with given error text. Application exceptions are thrown by various places in the validation code above. 
		///The message will be human ledgible and relevant to the error. All other exceptions were unexpected and the error message would not provide any value to the end user.
		///Called from OpenDentalWebApps.</summary>
		public static string CreateErrorResponse(Exception e,string defaultError) {
			if(e==null || !(e is ApplicationException)) {
				return CreateErrorResponse(defaultError);
			}
			return CreateErrorResponse(e.Message);
		}

		///<summary>Returns Response-Error payload with given error text. Called from OpenDentalWebApps.</summary>
		public static string CreateErrorResponse(string error) {
			StringBuilder strbuild=new StringBuilder();
			using(XmlWriter writer=XmlWriter.Create(strbuild,WebSerializer.CreateXmlWriterSettings(false))) {
				writer.WriteStartElement("Response");
				writer.WriteStartElement("Error");
				writer.WriteString(error);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			return strbuild.ToString();
		}

		///<summary>Throws an exception if there is an XML node title 'Error'.</summary>
		public static void CheckForError(string xmlResult) {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(xmlResult);
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				throw new Exception(node.InnerText);
			}
		}

	}

	public class PayloadItem:Tuple<object,string> {
		public object Content { get { return Item1; } }
		public string TagName { get { return Item2; } }

		public PayloadItem(object content,string tagName) : base(content,tagName) {
		}
	}
}
