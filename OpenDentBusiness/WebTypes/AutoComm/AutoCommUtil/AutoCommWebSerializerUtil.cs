using Newtonsoft.Json;
using OpenDentBusiness.WebTypes.AutoComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceSerializer;

namespace OpenDentBusiness {
	public static class AutoCommWebSerializerUtil {
		private static JsonSerializerSettings _jsonSettings=new JsonSerializerSettings() {
			TypeNameHandling=TypeNameHandling.Objects
		};
		///<summary>Serializes a list to a Json string and creates a success response payload. 
		///Enables the usage of base and derived classes for payloads.</summary>
		public static string CreateSuccessResponseAutoCommList<T>(List<T> list) {
			string json=JsonConvert.SerializeObject(list,typeof(List<T>),_jsonSettings);
			string payload=PayloadHelper.CreateSuccessResponse(
				new List<PayloadItem> {
				new PayloadItem(json,"AutoCommJsonString")
			});
			return payload;
		}

		///<summary>Serializes a list to a Json string and creates a payload for SendToHq(). 
		///Enables the usage of base and derived classes for payloads.</summary>
		public static string CreatePayloadAutoCommList<T>(List<T> list,eServiceCode code) {
			string json=JsonConvert.SerializeObject(list,typeof(List<T>),_jsonSettings);
			string payload=PayloadHelper.CreatePayload(
				new List<PayloadItem> {
					new PayloadItem(json,"AutoCommJsonString")
				},
				code
			);
			return payload;
		}
		
		///<summary>Deserializes a payload to a Json string. The Json string is Deserialized to list of Generics. 
		///Enables the usage of base and derived classes for payloads.</summary>
		public static List<T> DeserializeAutoCommList<T>(string payload) {
			string json=WebSerializer.DeserializeTag<string>(payload,"AutoCommJsonString");
			List<T> listAutoComms=JsonConvert.DeserializeObject<List<T>>(json,_jsonSettings);
			return listAutoComms;
		}
	}
}
