using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using CodeBase;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	public class ProviderApi : IProviderApi {
		
		private string _endpoint;

		private HttpClient _client { get; }=new HttpClient();

		public ProviderApi(string guid,string secret,string emailHostingEndpoint) {
			_client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Basic",Convert.ToBase64String(Encoding.UTF8.GetBytes(guid+":"+secret)));
			_endpoint=emailHostingEndpoint;
#if DEBUG
			ServicePointManager.ServerCertificateValidationCallback += (sender,cert,chain,sslPolicyErrors) => true;
#endif
		}

		///<summary>Creates a new account. ExternalID must be unique for this account guarantor.</summary>
		public CreateAccountResponse CreateAccount(CreateAccountRequest request) {
			return RunApiCall<CreateAccountResponse>(request,"POST",$"v1/account/");
		}

		///<summary>Gets an AccountGuarantor for the given primary key.</summary>
		public GetAccountResponse GetAccount(GetAccountRequest request) {
			return RunApiCall<GetAccountResponse>(request,"GET",$"v1/account/{request.AccountNum.ToString()}");
		}

		///<summary>Updates the status for the given account.</summary>
		public UpdateAccountStatusResponse UpdateAccountStatus(UpdateAccountStatusRequest request) {
			return RunApiCall<UpdateAccountStatusResponse>(request,"POST",$"v1/account/status");
		}

		///<summary>Creates a new account guarantor. ExternalID must be unique for this provider.</summary>
		public CreateAccountGuarantorResponse CreateAccountGuarantor(CreateAccountGuarantorRequest request) {
			return RunApiCall<CreateAccountGuarantorResponse>(request,"POST",$"v1/accountguarantor/");
		}

		///<summary>Gets an AccountGuarantor for the given primary key.</summary>
		public GetAccountGuarantorResponse GetAccountGuarantor(GetAccountGuarantorRequest request) {
			return RunApiCall<GetAccountGuarantorResponse>(request,"GET",$"v1/accountguarantor/{request.AccountGuarantorNum.ToString()}");
		}

		///<summary>Returns all of the usage statistics for the given external IDs. If an external ID is not in the list, it will not be returned in the result.</summary>
		public GetGuarantorUsageResponse GetGuarantorUsage(GetGuarantorUsageRequest request) {
			return RunApiCall<GetGuarantorUsageResponse>(request,"POST",$"v1/accountguarantor/usage");
		}

		///<summary>Updates the status of all account's associated to this AccountGuarantor.</summary>
		public UpdateAccountGuarantorStatusResponse UpdateAccountGuarantorStatus(UpdateAccountGuarantorStatusRequest request) {
			return RunApiCall<UpdateAccountGuarantorStatusResponse>(request,"POST",$"v1/accountguarantor/status");
		}

		///<summary>Returns health status for the various Secure Email services.</summary>
		public GetHealthResponse GetHealth(GetHealthRequest request) {
			return RunApiCall<GetHealthResponse>(request,"GET",$"v1/health/");
		}

		private T RunApiCall<T>(object requestObj,string httpType,string path) where T : class,new() {
			T retVal=null;
			Exception ex=null;
			System.Threading.Tasks.Task.Run(async () => {
				try {
					string payload=JsonConvert.SerializeObject(requestObj);
					HttpResponseMessage message=null;
					if(httpType=="GET") {
						//Don't use the request object for GET.
						message=await _client.GetAsync(_endpoint+path);
					}
					else if(httpType=="POST") {
						message=await _client.PostAsync(_endpoint+path,new StringContent(payload,Encoding.UTF8,"application/json"));
					}
					else if(httpType=="PUT") {
						message=await _client.PutAsync(_endpoint+path,new StringContent(payload,Encoding.UTF8,"application/json"));
					}
					else if(httpType=="DELETE") {
						//Don't use the request object for DELETE.
						message=await _client.DeleteAsync(_endpoint+path);
					}
					else {
						throw new ApplicationException($@"Unknown {nameof(httpType)}: {httpType}");
					}
					string body=await message.Content.ReadAsStringAsync();
					var result=JsonConvert.DeserializeAnonymousType(body,new {
						Information="",
						Content=default(T),
					});
					if(!message.IsSuccessStatusCode) {
						throw new ApplicationException($"HttpStatusCode returned: {(int)message.StatusCode} - {message.StatusCode.ToString()}\nInformation: {result?.Information??"NULL RESULT"}");
					}
					//If we made it here and Content is null, they had nothing to return. We will return a new instance so that no one gets an unexpected null result.
					retVal=result?.Content??new T();
				}
				catch(Exception e) {
					ex=e;
				}
			}).GetAwaiter().GetResult();
			if(ex!=null) {
				throw ex;
			}
			return retVal;
		}
	}
}
