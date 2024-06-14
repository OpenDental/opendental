using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness.WebBridges;

namespace OpenDentBusiness {
	public class APIRequest : IApiRequest {

		public static IApiRequest Inst { get;set; } = new APIRequest();

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		public T SendRequest<T>(string urlEndpoint,HttpMethod method,AuthenticationHeaderValue authHeaderVal,string body,HttpContentType contentType) {
				return System.Threading.Tasks.Task.Run(async () => await SendRequestAsync<T>(urlEndpoint,method,authHeaderVal,body,contentType))
					.GetAwaiter()
					.GetResult();
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater. Any type that gets passed in and wants the status code must have a JSON property attribute 'ResponseCode'.</summary>
		public async Task<T> GetRequestAsync<T>(string urlEndpoint,HttpClient clientOverride=null) {
			return await SendRequestAsync<T>(urlEndpoint,HttpMethod.Get,null,null,HttpContentType.None,clientOverride);
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater. Any type that gets passed in and wants the status code must have a JSON property attribute 'ResponseCode'.</summary>
		public async Task<T> PostRequestAsync<T>(string urlEndpoint,string body,HttpContentType contentType=HttpContentType.Json,HttpClient clientOverride=null) {
			return await SendRequestAsync<T>(urlEndpoint,HttpMethod.Post,null,body,contentType,clientOverride);
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater. Any type that gets passed in and wants the status code must have a JSON property attribute 'ResponseCode'.</summary>
		public async Task<T> SendRequestAsync<T>(string urlEndpoint,HttpMethod method,AuthenticationHeaderValue authHeaderVal,string body,HttpContentType contentType=HttpContentType.Json,HttpClient clientOverride=null) {
			string res="";
			HttpResponseMessage response=new HttpResponseMessage();
			HttpClient client=clientOverride ?? new HttpClient();
			bool disposeClient=clientOverride==null;// Dispose client only if it was created here
			try {
				using(HttpRequestMessage request=new HttpRequestMessage(method,urlEndpoint)) {
					if(authHeaderVal!=null) {
						request.Headers.Authorization=authHeaderVal;
					}
					if(method!=HttpMethod.Get) {
						if(contentType==HttpContentType.Json) {
							request.Content=new StringContent(body,Encoding.UTF8,"application/json");
						}
						if(contentType==HttpContentType.UrlEncoded) {
							//Deserialize body to get x-www-form-urlencoded format, then update all values to be strings since deserialization does not handle that.
							Dictionary<string,string> dictKeyValuePairs=JsonConvert.DeserializeObject<Dictionary<string,object>>(body)
								.ToDictionary(x => x.Key,x => x.Value == null ? "" : x.Value.ToString());
							request.Content=new FormUrlEncodedContent(dictKeyValuePairs.Select(x => x));
						}
					}
					response=await client.SendAsync(request);
				}
				using(var sr=new StreamReader(await response.Content.ReadAsStreamAsync())) {
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
				var errorJson=new ApiResponseError { Message=errorMsg,Response=res,ResponseStatus=(int)response.StatusCode };
				throw new HttpRequestException(JsonConvert.SerializeObject(errorJson),hre);
			}
			catch(Exception ex) {
				//For now, rethrow error and let whoever is expecting errors to handle them.
				//We may enhance this to care about codes at some point.
				throw ex;
			}
			finally {
				if(disposeClient) {
					client.Dispose();// Dispose client only if it was created within this method
				}
			}
		}
	}

	public class ApiResponseError {
		public string Message;
		public string Response;
		public int ResponseStatus;
	}
	public enum HttpContentType {
		///<summary>0 - None, we don't send any content.</summary>
		None,
		///<summary>1 - application/json.</summary>
		Json,
		///<summmary>2 - application/x-www-form-urlencoded.</summmary>
		UrlEncoded
	}
}
