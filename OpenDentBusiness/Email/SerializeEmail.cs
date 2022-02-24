using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OpenDentBusiness.Email {

	///<summary>Convert EmailRequest and EmailResult to/from string.</summary>
	public class Serializer {
		#region EmailRequest
		public static string SerializeRequest(EmailRequest emailRequest) {
			return Serialize(emailRequest);
		}
		public static EmailRequest DeserializeRequest(string xml) {
			return Deserialize<EmailRequest>(xml);
		}
		#endregion

		#region EmailResult
		public static string SerializeResult(EmailResult emailResult) {
			return Serialize(emailResult);
		}
		public static EmailResult DeserializeResult(string xml) {
			return Deserialize<EmailResult>(xml);
		}
		#endregion

		private static T Deserialize<T>(string toDeserialize) {
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(T));
			using(StringReader textReader=new StringReader(toDeserialize)) {
				T objEscaped=(T)xmlSerializer.Deserialize(textReader);
				return (T)XmlConverter.XmlUnescapeRecursion(typeof(T),objEscaped);
			}
		}

		private static string Serialize<T>(T toSerialize) {
			T escapedObj=(T)XmlConverter.XmlEscapeRecursion(typeof(T),toSerialize);
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(T));
			StringWriter writer=new StringWriter();
			using(XmlWriter textWriter=XmlWriter.Create(writer,new XmlWriterSettings { Indent=false,NewLineHandling=NewLineHandling.None })) {
				xmlSerializer.Serialize(textWriter, toSerialize);
				return writer.ToString();
			}
		}
	}
}