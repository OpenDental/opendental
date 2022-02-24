using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WebServiceSerializer;

namespace OpenDentBusiness {
	public class Google {

		///<summary>Returns the full URL according to the route/route id(s) given.  RouteIDs must be added in order left to right as they appear
		///in the API call.</summary>
		private static string GetApiUrl(UrlEndpoint endpoint) {
			string authEndpointURL="https://accounts.google.com/o/oauth2/v2/auth";
			switch(endpoint) {
				case UrlEndpoint.Root:
					//Do nothing.  This is to allow someone to quickly grab the URL without having to make a copy+paste reference.
					break;
				case UrlEndpoint.AccessToken:
				case UrlEndpoint.RefreshToken:
					authEndpointURL="https://oauth2.googleapis.com/token";
					break;
				default:
					break;
			}
			return authEndpointURL;
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		private static T Request<T>(UrlEndpoint endpoint,HttpMethod method,string body,T responseType,string acceptType="application/x-www-form-urlencoded") {
			using(WebClient client=new WebClient()) {
				client.Headers[HttpRequestHeader.Accept]=acceptType;
				client.Headers[HttpRequestHeader.ContentType]=acceptType;
				client.Encoding=UnicodeEncoding.UTF8;
				//Post with Authorization headers and a body comprised of a JSON serialized anonymous type.
				try {
					string res="";
					if(method==HttpMethod.Get) {
						res=client.DownloadString(GetApiUrl(endpoint));
					}
					else if(method==HttpMethod.Post) {
						res=client.UploadString(GetApiUrl(endpoint),HttpMethod.Post.Method,body);
					}
					else if(method==HttpMethod.Put) {
						res=client.UploadString(GetApiUrl(endpoint),HttpMethod.Put.Method,body);
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
						throw new ODException("Could not connect to the Google server:"+"\r\n"+wex.Message,wex);
					}
					string res="";
					using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						res=sr.ReadToEnd();
					}
					if(string.IsNullOrWhiteSpace(res)) {
						//The response didn't contain a body.  Through my limited testing, it only happens for 401 (Unauthorized) requests.
						if(wex.Response.GetType()==typeof(HttpWebResponse)) {
							HttpStatusCode statusCode=((HttpWebResponse)wex.Response).StatusCode;
							if(statusCode==HttpStatusCode.Unauthorized) {
								throw new ODException("Invalid Google credentials.");
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

		///<summary>Attempts to retrieve OAuth authorization Url for Google.</summary>
		public static string GetGoogleAuthorizationUrl(string emailAddress) {
			try {
				string url=GetApiUrl(UrlEndpoint.Root);
				string response=WebServiceMainHQProxy.GetWebServiceMainHQInstance().BuildOAuthUrl(PrefC.GetString(PrefName.RegistrationKey),OAuthApplicationNames.Google.ToString());
				if(response.Trim().First()=='<') {//The first character in xml is always '<'
					//This method has now returned xml meaning the web method has returned an xml payload error message the deserializer will not know how to handle.
					XmlDocument xml=new XmlDocument();
					xml.LoadXml(response);
					XmlNode errorNode=xml.SelectSingleNode("//Error");
					if(errorNode==null) {
						throw new Exception("No error message returned from server.");
					}
					throw new Exception(errorNode.InnerText);
				}
				url+="?"+JsonConvert.DeserializeObject(response);
				url+="&login_hint="+emailAddress;
				return url;
			}
			catch(Exception ex) {
				throw new ApplicationException("Error occured retrieving Authorization Url: "+ex.Message);
			}
		}

		///<summary>Gets OAuth Access and Refresh tokens for an auth code.
		///The auth code is a temporary code that gives Open Dental access to use the email address that was signed into to grant us access.</summary>
		public static GoogleToken MakeAccessTokenRequest(string code) {
			try {
				return GetAccessTokenHqOrThrow(code,false);
			}
			catch(Exception ex) {
				return new GoogleToken("","",ex.Message);
			}
		}

		private static GoogleToken GetAccessTokenHqOrThrow(string code,bool isRefresh) {
			List<PayloadItem> listPayloadItems=new List<PayloadItem> {
				new PayloadItem(code,"Code"),
				new PayloadItem(isRefresh,"IsRefreshToken"),
			};
			string officeData=PayloadHelper.CreatePayload(PayloadHelper.CreatePayloadContent(listPayloadItems),eServiceCode.OAuth);
			string result;
			try {
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
					.GetGoogleAccessToken(officeData);
				return JsonConvert.DeserializeObject<GoogleToken>(result);
			}
			catch(Exception ex) {
				throw ex;//Purposefully rethrow here so we can handle it out a level.
			}
		}

		///<summary>Gets OAuth Access and Refresh tokens for an auth code.  Called from WebServiceMainHQ.</summary>
		public static GoogleToken GetAccessToken(string body) {
			try {
				var resObj=Request(UrlEndpoint.AccessToken,HttpMethod.Post,body,new { access_token="",expires_in=0,refresh_token="",scope="",token_type=""});
				return new GoogleToken(resObj.access_token,resObj.refresh_token);
			}
			catch(Exception e) {
				return new GoogleToken("","",e.Message);
			}
		}

		///<summary>Gets OAuth Access and Refresh tokens for an email adress.</summary>
		public static string MakeRefreshAccessTokenRequest(string refreshToken) {
			try {
				GoogleToken resObj=GetAccessTokenHqOrThrow(refreshToken,true);
				return resObj.AccessToken;
			}
			catch(Exception e) {
				throw e;//Purposefully rethrow here so we can handle it out a level.
			}
		}

		private enum UrlEndpoint {
			Root,
			AccessToken,
			RefreshToken,
		}

	}//End Google class.

	public class GoogleToken {
		public string AccessToken;
		public string RefreshToken;
		public string ErrorMessage;

		public GoogleToken(string access,string refresh,string error="") {
			AccessToken=access;
			RefreshToken=refresh;
			ErrorMessage=error;
		}

		//For serialization.
		public GoogleToken() {
			AccessToken="";
			RefreshToken="";
			ErrorMessage="";
		}
	}
}
