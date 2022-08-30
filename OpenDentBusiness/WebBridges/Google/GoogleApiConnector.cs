using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using OpenDentBusiness.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GmailApi=Google.Apis.Gmail.v1;

namespace OpenDentBusiness {
	/// <summary>This file is necessary because Google.cs has references to ODHQ and other entities that would need to be included in the
	/// OpenDentalEmail project.  This class handles connecting Google's API to us with as little dependencies as possible.</summary>
	public class GoogleApiConnector {

		///<summary>Helper method that returns a ready-to-use Gmail service that is created based on the user's credentials</summary>
		public static GmailApi.GmailService CreateGmailService(BasicEmailAddress emailAddress) {
			ODGoogleUserCredential credential=new ODGoogleUserCredential() {
				AccessToken=emailAddress.AccessToken,
			};
			BaseClientService.Initializer baseService=new BaseClientService.Initializer(){
				HttpClientInitializer=credential
			};
			return new GmailApi.GmailService(baseService);
		}

		///<summary>This class is a wrapper for Google's UserCredential class, which is tightly coupled with automatic access token retrieving and refreshing, by using the client id/client secret.
		///Since we do not want to expose our client id/client secret, we must implement our own interfaces and pass them into Google to use their API.  When a call fails, we must refresh the token ourselves.</summary>
		public class ODGoogleUserCredential : IConfigurableHttpClientInitializer, IHttpExecuteInterceptor {

			///<summary>The access token to use to send requests to the Google API</summary>
			public string AccessToken { get; set; }

			public ODGoogleUserCredential() { }

			///<summary>This is the required method for IConfigurableHttpClientInitializer.  This matches the logic from Google's UserCredential
			///from their GitHub page</summary>
			public void Initialize(ConfigurableHttpClient httpClient) {
				httpClient.MessageHandler.Credential=this;
			}

			///<summary>This is the required method for IHttpExecuteInterceptor.  This method allows us to hijack our own requests and add headers or other values to the context.
			///Google assumes that this method will attach needed Authorization headers, which we must use a Bearer token for.</summary>
			public System.Threading.Tasks.Task InterceptAsync(HttpRequestMessage request,CancellationToken cancellationToken) {
				BearerToken.AuthorizationHeaderAccessMethod bearerTokenInterceptor=new BearerToken.AuthorizationHeaderAccessMethod();
				//Overwrite the Authorization header with a bearer token.
				bearerTokenInterceptor.Intercept(request,AccessToken);
				//We can't use Task.CompletedTask because we are not on .NET 4.6 at this time.  Simply run an empty task.
				return System.Threading.Tasks.Task.Run(() => { });
			}
		}
	}
}
