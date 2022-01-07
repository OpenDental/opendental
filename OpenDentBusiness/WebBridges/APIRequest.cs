using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness.WebBridges;

namespace OpenDentBusiness {
	public class APIRequest : IApiRequest {

		public static IApiRequest Inst { get;set; } = new APIRequest();

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		public T SendRequest<T>(string urlEndpoint,HttpMethod method,AuthenticationHeaderValue authHeaderVal,string body) {
				return System.Threading.Tasks.Task.Run(async () => await SendRequestAsync<T>(urlEndpoint,method,authHeaderVal,body))
					.GetAwaiter()
					.GetResult();
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		public async Task<T> SendRequestAsync<T>(string urlEndpoint,HttpMethod method,AuthenticationHeaderValue authHeaderVal,string body) {
			string res="";
			try {
				HttpResponseMessage response;
				using HttpClient client=new HttpClient();
				using(HttpRequestMessage request=new HttpRequestMessage(method,urlEndpoint)) {
					if(authHeaderVal!=null) {
						request.Headers.Authorization=authHeaderVal;
					}
					request.Content=new StringContent(body,Encoding.UTF8,"application/json");
					response=await client.SendAsync(request);
				}
				using(var sr=new StreamReader(response.Content.ReadAsStreamAsync().Result)) {
					res=sr.ReadToEnd();
				}
				response.EnsureSuccessStatusCode();//Throws exception if not successful.
				if(ODBuild.IsDebug() && (typeof(T)==typeof(string))) {//If user wants the entire json response as a string
					return (T)Convert.ChangeType(res,typeof(T));
				}
				return JsonConvert.DeserializeObject<T>(res);
			}
			catch(HttpRequestException hre) {
				string errorMsg=hre.Message+(string.IsNullOrWhiteSpace(res) ? "" : "\r\nRaw response:\r\n"+res);
				var errorJson=new ApiResponseError { Message=errorMsg,Response=res };
				throw new HttpRequestException(JsonConvert.SerializeObject(errorJson),hre);
			}
			catch(Exception ex) {
				//For now, rethrow error and let whoever is expecting errors to handle them.
				//We may enhance this to care about codes at some point.
				throw ex;
			}
		}
	}

	public class ApiResponseError {
		public string Message;
		public string Response;
	}
}
