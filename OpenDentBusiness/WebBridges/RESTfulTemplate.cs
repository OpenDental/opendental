using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace OpenDentBusiness {
	class RESTfulTemplate {
		///<summary>Returns the full URL according to the route/route id(s) given.  RouteIDs must be added in order left to right as they appear
		///in the API call.</summary>
		private static string GetApiUrl(UrlEndpoint endpoint,params string[] listRouteIDs) {
			string authEndpointURL="https://accounts.google.com/o/oauth2/v2/auth";
			//If you need to use Introspection for sandbox environments uncomment the line below and update the Introspection class.
			//string authEndpointURL=Introspection.GetOverride(Introspection.IntrospectionEntity.GoogleUrl,"https://accounts.google.com/o/oauth2/v2/auth");
			if(ODBuild.IsDebug()) {
				authEndpointURL="https://accounts.google.com/o/oauth2/v2/auth";
			}
			switch(endpoint) {
				case UrlEndpoint.Root:
					//Do nothing.  This is to allow someone to quickly grab the URL without having to make a copy+paste reference.
					break;
				case UrlEndpoint.Token:
					authEndpointURL+="/token";
					break;
				default:
					break;
			}
			return authEndpointURL;
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		private static T Request<T>(UrlEndpoint endpoint,HttpMethod method,string authHeader,string body,T responseType,string acceptType = "application/json",params string[] listRouteIDs) {
			using(WebClient client = new WebClient()) {
				client.Headers[HttpRequestHeader.Accept]=acceptType;
				client.Headers[HttpRequestHeader.ContentType]=acceptType;
				client.Headers[HttpRequestHeader.Authorization]=authHeader;
				client.Encoding=UnicodeEncoding.UTF8;
				//Post with Authorization headers and a body comprised of a JSON serialized anonymous type.
				try {
					string res="";
					//Only GET and POST are supported currently.
					if(method==HttpMethod.Get) {
						res=client.DownloadString(GetApiUrl(endpoint,listRouteIDs));
					}
					else if(method==HttpMethod.Post) {
						res=client.UploadString(GetApiUrl(endpoint,listRouteIDs),HttpMethod.Post.Method,body);
					}
					else if(method==HttpMethod.Put) {
						res=client.UploadString(GetApiUrl(endpoint,listRouteIDs),HttpMethod.Put.Method,body);
					}
					else {
						throw new Exception("Unsupported HttpMethod type: "+method.Method);
					}
					if(ODBuild.IsDebug()) {
						if((typeof(T)==typeof(string))) {//If user wants the entire json response as a string
							return (T)Convert.ChangeType(res,typeof(T));
						}
					}
					return JsonConvert.DeserializeAnonymousType(res,responseType);
				}
				catch(WebException wex) {
					if(!(wex.Response is HttpWebResponse)) {
						throw new ODException(Lans.g("Google","Could not connect to the Google server:")+"\r\n"+wex.Message,wex);
					}
					string res="";
					using(var sr = new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						res=sr.ReadToEnd();
					}
					if(string.IsNullOrWhiteSpace(res)) {
						//The response didn't contain a body.  Through my limited testing, it only happens for 401 (Unauthorized) requests.
						if(wex.Response.GetType()==typeof(HttpWebResponse)) {
							HttpStatusCode statusCode=((HttpWebResponse)wex.Response).StatusCode;
							if(statusCode==HttpStatusCode.Unauthorized) {
								throw new ODException(Lans.g("Google","Invalid Google credentials."));
							}
						}
					}
					string errorMsg=wex.Message+(string.IsNullOrWhiteSpace(res) ? "" : "\r\nRaw response:\r\n"+res);
					throw new Exception(errorMsg,wex);//If it got this far and haven't rethrown, simply throw the entire exception.
				}
				catch(Exception ex) {
					//WebClient returned an http status code >= 300
					ex.DoNothing();
					//For now, rethrow error and let whoever is expecting errors to handle them.
					//We may enhance this to care about codes at some point.
					throw;
				}
			}
		}

		private enum UrlEndpoint {
			Root,
			Token,
		}
	}
}
