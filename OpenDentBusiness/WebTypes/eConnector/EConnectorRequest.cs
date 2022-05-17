using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenDentBusiness.WebTypes {
	///<summary>This class is used to hold information regarding a customer's eConnector that has attempted to connect to HQ.</summary>
	public class EConnectorRequest : WebBase {
		///<summary>The computer where the eConnector is running.</summary>
		public string CompName;
		///<summary>The public IP address of the network where the eConnector is running.</summary>
		public string PublicIP;
		///<summary>The most recent HQ server time this eConnector requested Proxy Prefs from HQ and was allowed to connect.</summary>
		public DateTime DateTimeHQ;
		///<summary>Should only be set at HQ.</summary>
		public bool IsBlacklisted;
		///<summary>Unique identifier for the eConnector.</summary>
		public string SessionGuid;
		///<summary>The internal network IP address of the computer where the eConnector is running.</summary>
		public string InternalIP;
		

		private static JsonSerializerSettings _jsonSettings=new JsonSerializerSettings() {
			Converters=new List<JsonConverter> {
				new IsoDateTimeConverter { 
					DateTimeFormat="yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffff" //format date with microseconds
				},
			},
		};
		
		///<summary>Returns an empty list if deserialization fails.</summary>
		public static List<EConnectorRequest> DeserializeListFromJson(string jsonString) {
			try {
				return JsonConvert.DeserializeObject<List<EConnectorRequest>>(jsonString,_jsonSettings)??new List<EConnectorRequest>();
			}
			catch(Exception ex) {
				ex.DoNothing();
				return new List<EConnectorRequest>();
			}
		}

		

		public static string SerializeToJson(List<EConnectorRequest> listRequests) {
			return JsonConvert.SerializeObject(listRequests,_jsonSettings);
		}

		///<summary>Get the most recent entry for each eConnector.</summary>
		public static List<EConnectorRequest> GroupByEConnector(List<EConnectorRequest> listRequestsIn) {
			try {
				List<EConnectorRequest> listRequestsOut=OrderRequests(listRequestsIn)
					.GroupBy(x => new { x.SessionGuid,x.PublicIP,x.CompName })
					.Select(x => x.First()).ToList();
				return listRequestsOut;
			}
			catch(Exception e) {
				e.DoNothing();
			}
			return listRequestsIn;
		}

		///<summary>Compares two EConnectorStatistics to determine if they refer to the same eConnector</summary>
		public static bool AreSameEConnector(EConnectorRequest eConnA,EConnectorRequest eConnB) {
			if(eConnA==null || eConnB==null) {
				return false;
			}
			if(!string.IsNullOrWhiteSpace(eConnA.SessionGuid) || !string.IsNullOrWhiteSpace(eConnB.SessionGuid)) {
				//At least one eConnector is on a version that supports SessionGuid.
				return eConnA.SessionGuid==eConnB.SessionGuid;
			}
			//SessionGuids are not initialized(one or both), or SessionGuids do not match. Compare CompName and PublicIP.
			string nameA=eConnA.CompName.ToLower().Trim();
			string ipA=eConnA.PublicIP.ToLower().Trim();
			string nameB=eConnB.CompName.ToLower().Trim();
			string ipB=eConnB.PublicIP.ToLower().Trim();
			return (nameA==nameB && ipA==ipB);
		}

		public static EConnectorRequest GetPrimaryEConnector(List<EConnectorRequest> listRequests) {
			return OrderRequests(listRequests)
				.FirstOrDefault();
		}

		private static IEnumerable<EConnectorRequest> OrderRequests(List<EConnectorRequest> listRequests) {
			return listRequests.OrderByDescending(x => x.DateTimeHQ)
				.ThenBy(x => x.SessionGuid)//Tie breaker 1
				.ThenBy(x => x.PublicIP)//Tie breaker 2
				.ThenBy(x => x.CompName);//Tie breaker 3
		}

		public override string ToString() => JsonConvert.SerializeObject(this,_jsonSettings);
	}
}
