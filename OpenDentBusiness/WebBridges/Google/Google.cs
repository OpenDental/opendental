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
using System.Diagnostics;
using System.Security.Cryptography;
using OpenDentBusiness.UI;

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
					authEndpointURL="https://www.googleapis.com/oauth2/v4/token";
					break;
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

		///<summary>Used for exchanging an auth code for tokens, and for requesting a new access token with a refresh token.
		///Sends the data needed to WebServiceMainHQ which builds and sends the request.</summary>
		private static GoogleToken GetAccessTokenHqOrThrow(string code,bool isRefresh,string redirectUri=null,string codeVerifier=null) {
			List<PayloadItem> listPayloadItems=new List<PayloadItem>();
			listPayloadItems.Add(new PayloadItem(code,"Code"));
			listPayloadItems.Add(new PayloadItem(isRefresh,"IsRefreshToken"));
			if(redirectUri!=null) {
				//The redirectUri points to a port on the user's computer. It is fine to send this to WebServiceMainHQ
				//because Google just compares it to the redirectUri that was sent with the previous auth request as an additional security measure.
				listPayloadItems.Add(new PayloadItem(redirectUri,"RedirectUri"));
			}
			if(codeVerifier!=null) {
				listPayloadItems.Add(new PayloadItem(codeVerifier,"CodeVerifier"));
			}
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

		///<summary>Builds and makes the auth code request and exchanges the auth code for tokens.
		///After Starting the listener and using this class, CloseListener() must be called.</summary>
		[Serializable()]
		public class AuthorizationRequest {
			///<summary>The encryption method used to create the CodeChallenge. Sent with the auth code request. "S256" represents the SHA256 hashing method.</summary>
			private const string _CODE_CHALLENGE_METHOD="S256";
			/// <summary>49215 is the lowest port number within the range designated for dynamic use by the Internet Assigned Numbers Authority (IANA)
			///See section 4.4 at this link: https://www.ietf.org/archive/id/draft-cotton-tsvwg-iana-ports-00.html#privateports</summary>
			private const int _MIN_PORT=49152;
			/// <summary>65535 is the highest port number within the range designated for dynamic use by the Internet Assigned Numbers Authority (IANA)
			///See section 4.4 at this link: https://www.ietf.org/archive/id/draft-cotton-tsvwg-iana-ports-00.html#privateports</summary>
			private const int _MAX_PORT=65535;
			///<summary>Listens on a localhost port for a response from Google that contains the auth code.
			///After using an AuthorizationRequest, CloseListener() must be called to close this.</summary>
			private HttpListener _listener;
			///<summary>A random 32 byte string, base64 encoded. Sent with the auth request. Google returns it with their response
			///so we can confirm that thier response is for our application's request.</summary>
			private string _state;
			///<summary>A random 32 byte string, base64 encoded. Sent with the token request that follows the auth code request.
			///Google verifies that both requests were made by our app by applying the CODE_CHALLENGE_METHOD to the CodeVerifier
			///and confirming that the result is the CodeChallenge.</summary>
			private string _codeVerifier;
			///<summary>The CodeVerifier, encrypted with the CODE_CHALLENGE_METHOD, base64 encoded. Sent with the auth code request.</summary>
			private string _codeChallenge;
			///<summary>The url used to request an auth code.</summary>
			private string _url;

			///<summary>Throws exceptions
			///If not already listening, searches for an available port to start an HTTP listener on.
			///Method was adapted from the following Stackoverflow answer: https://stackoverflow.com/a/46666370</summary>
			public void StartListener() {
				if(_listener!=null && _listener.IsListening) {
					return;
				}
				for(int i=_MIN_PORT;i<_MAX_PORT;i++) {
					if(ODEnvironment.IsCloudServer) {
						try {
							if(!ODCloudClient.ODCloudAuthGoogleListener($"http://{IPAddress.Loopback}:{i}/")) {
								continue;
							}
							return;
						}
						catch(Exception) {
							return;
						}
					}
					else {
						_listener=new HttpListener();
						_listener.Prefixes.Add($"http://{IPAddress.Loopback}:{i}/");
						try {
							_listener.Start();
							return;
						}
						catch(Exception ex) {
							//If the port is not available, an error is thrown and the listener disposes itself.
							//Do nothing because we will keep trying until we run out of ports in our range.
							ex.DoNothing();
						}
					}
				}
				throw new ODException($"Could not find an available port for the HttpListener. Ports {_MIN_PORT} to {_MAX_PORT} were tried.");
			}

			///<summary>Throws exceptions. StartListener() must be called before calling this.
			///Sends request for an auth code and gets the response from Google.
			///Shows a message in the browser indicating to user that they can close it and return to Open Dental.
			///Exchanges the auth code for tokens and returns them. The GoogleToken returned may contain an error message from WebServiceMainHQ.
			///If you are done with the AuthorizationRequest after calling this, be sure to call CloseListener().</summary>
			public GoogleToken MakeAccessTokenRequest(string emailAddress) {
				if(ODEnvironment.IsCloudServer) {
					if(!ODCloudClient.CheckIsListening()) {
						throw new ODException("An attempt to request tokens was made before starting the HttpListener.");
					}
				}
				else {
					if(_listener==null || !_listener.IsListening) {
						throw new ODException("An attempt to request tokens was made before starting the HttpListener.");
					}
				}
				_state=RandomDataBase64Url(32);
				_codeVerifier=RandomDataBase64Url(32);
				_codeChallenge=Base64urlencodeNoPadding(Sha256(_codeVerifier));
				BuildAuthorizationUrl(emailAddress);
				if(ODCloudClient.IsAppStream) {
					ODCloudClient.LaunchFileWithODCloudClient(_url);
				}
				else{
					Process.Start(_url);
				}
				string code="";
				if(ODEnvironment.IsCloudServer) {
					string GoogleAuthCodeResponseHtml=Properties.Resources.GoogleAuthCodeResponseHtml;
					ODCloudClient.HttpListenerGetContext();
					while(string.IsNullOrWhiteSpace(code)) {
						code=ODCloudClient.SendListenerResponse(GoogleAuthCodeResponseHtml,_state);
						Thread.Sleep(100);
					}
				}
				else {
					HttpListenerContext context=_listener.GetContext();
					SendListenerResponse(context);
					code=GetAuthCodeFromContextOrThrow(context);
				}
				GoogleToken token=Google.GetAccessTokenHqOrThrow(code,isRefresh:false,GetRedirectUri(),_codeVerifier);
				return token;
			}

			///<summary>Closes the HttpListener if it is not null. If you close the listener but intend to use this AuthorizationRequest again,
			///you must call StartListener() again.</summary>
			public void CloseListener() {
				if(ODEnvironment.IsCloudServer) {
					ODCloudClient.CloseListener();
				}
				else {
					if(_listener!=null) {
						_listener.Close();
						_listener=null;
					}	
				}
			}

			///<summary>Returns URI-safe data with a given input length.
			///Reference: https://github.com/googlesamples/oauth-apps-for-windows</summary>
			private string RandomDataBase64Url(uint length) {
				RNGCryptoServiceProvider rng=new RNGCryptoServiceProvider();
				byte[] bytes=new byte[length];
				rng.GetBytes(bytes);
				return Base64urlencodeNoPadding(bytes);
			}

			///<summary>Base64url no-padding encodes the given input buffer.
			///Reference: https://github.com/googlesamples/oauth-apps-for-windows</summary>
			private string Base64urlencodeNoPadding(byte[] buffer) {
				string base64=Convert.ToBase64String(buffer);
				//Converts base64 to base64url.
				base64=base64.Replace("+","-");
				base64=base64.Replace("/","_");
				//Strips padding.
				base64=base64.Replace("=","");
				return base64;
			}

			///<summary>Returns the SHA256 hash of the input string.
			///Reference: https://github.com/googlesamples/oauth-apps-for-windows</summary>
			private byte[] Sha256(string inputStirng) {
				byte[] bytes=Encoding.ASCII.GetBytes(inputStirng);
				SHA256Managed sha256=new SHA256Managed();
				return sha256.ComputeHash(bytes);
			}

			///<summary>Calls methods that throw exceptions. Gets some query parameters from WebServiceMainHQ.</summary>
			private void BuildAuthorizationUrl(string emailAddress) {
				StringBuilder stringBuilder=new StringBuilder();
				stringBuilder.Append(Google.GetApiUrl(UrlEndpoint.Root));
				stringBuilder.Append("?");
				//This will throw if user doesn't have OD reg key. Includes the OD client ID for google and all other params that never change.
				stringBuilder.Append(GetQueryParamsFromHQ());
				stringBuilder.Append("&login_hint="+emailAddress);
				stringBuilder.Append("&state="+_state);
				stringBuilder.Append("&code_challenge="+_codeChallenge);
				stringBuilder.Append("&code_challenge_method="+_CODE_CHALLENGE_METHOD);
				stringBuilder.Append("&redirect_uri="+Uri.EscapeDataString(GetRedirectUri()));
				_url=stringBuilder.ToString();
			}

			///<summary>Throws exceptions. Requests some query parameters for auth request URL from WebServiceMainHQ.
			///Will throw if the customer's registration key is not valid.</summary>
			private string GetQueryParamsFromHQ() {
				try {
					string response=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
						.BuildOAuthUrl(PrefC.GetString(PrefName.RegistrationKey),OAuthApplicationNames.GoogleLoopbackIpAddressFlow.ToString());
					//The first character in xml is always '<'
					//This method has now returned xml meaning the web method has returned an xml payload error message the deserializer will not know how to handle.
					if(response.Trim().First()=='<') {
						XmlDocument xml=new XmlDocument();
						xml.LoadXml(response);
						XmlNode errorNode=xml.SelectSingleNode("//Error");
						if(errorNode==null) {
							throw new Exception("No error message returned from server.");
						}
						throw new Exception(errorNode.InnerText);
					}
					return JsonConvert.DeserializeObject(response).ToString();
				}
				catch(Exception ex) {
					throw new ApplicationException("Error occurred retrieving Authorization Url: "+ex.Message);
				}
			}

			///<summary>Returns the first Prefix of the HttpListener which should be our redirect URI.</summary>
			private string GetRedirectUri() {
				string redirectUri;
				if(ODEnvironment.IsCloudServer) {
					redirectUri = ODCloudClient.GetRedirectUri();
				}
				else {
					redirectUri=_listener.Prefixes.AsEnumerable().FirstOrDefault();
				}
				if(redirectUri==null) {
						redirectUri="";
					}
				return redirectUri;
			}

			///<summary>Sends a response to Google via the HttpListenerContext.
			///This HTML will display in the browser, letting the user know they can close it and return to Open Dental.
			///Adapted from code in Google's sample app:https://github.com/googlesamples/oauth-apps-for-windows</summary>
			private void SendListenerResponse(HttpListenerContext context) {
				string GoogleAuthCodeResponseHtml=Properties.Resources.GoogleAuthCodeResponseHtml;
				byte[] buffer=Encoding.UTF8.GetBytes(GoogleAuthCodeResponseHtml);
				context.Response.ContentLength64=buffer.Length;
				context.Response.OutputStream.Write(buffer,0,buffer.Length);
				context.Response.OutputStream.Close();
			}

			///<summary>Throws an exception if Google returned an error or did not return an auth code or state.
			///Also throws if the returned state does not match ours. Otherwise, returns the auth code.
			///Adapted from code in Google's sample app:https://github.com/googlesamples/oauth-apps-for-windows</summary>
			private string GetAuthCodeFromContextOrThrow(HttpListenerContext context) {
				// Checks for errors.
				string error=context.Request.QueryString.Get("error");
				string authCode=context.Request.QueryString.Get("code");
				string incomingState=context.Request.QueryString.Get("state");
				if(error.IsNullOrEmpty()) {
					if(authCode.IsNullOrEmpty() || incomingState.IsNullOrEmpty()) {
						error=$"Malformed authorization response. {context.Request.QueryString}";
					}
					//Compares the receieved state to the expected value, to ensure that
					//Open Dental made the request which resulted in authorization.
					else if(incomingState!=_state) {
						error=$"Received request with invalid state ({incomingState})";
					}
				}
				if(!error.IsNullOrEmpty()) {
					throw new ODException(error);
				}
				return authCode;
			}
		}//End AuthorizationRequest class.
	}//End Google class.

	[Serializable()]
	public class GoogleToken {
		public string AccessToken;
		public string RefreshToken;
		public string ErrorMessage;

		public GoogleToken(string access,string refresh,string error="") {
			AccessToken=access;
			RefreshToken=refresh;
			ErrorMessage=error;
		}

		///<summary>Required for serialization.</summary>
		public GoogleToken() {
			AccessToken="";
			RefreshToken="";
			ErrorMessage="";
		}
	}
}
