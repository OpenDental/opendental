using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using CodeBase;
using System.Collections.Generic;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	public class ProviderApiDebugWSHQ:IProviderApi {
		public static Func<GetGuarantorUsageRequest,GetGuarantorUsageResponse> GetGuarantorUsageMock;

		///<summary>Sets all the mock functions to their default values. This is only used in Unit testing.</summary>
		public static void ResetDefaults() {
			GetGuarantorUsageMock=(a) => { return new GetGuarantorUsageResponse() { DictionaryStatistics=new Dictionary<string,List<AccountUsage>>() }; };
		}
		
		public ProviderApiDebugWSHQ() { }

		public CreateAccountResponse CreateAccount(CreateAccountRequest request) {
			return new CreateAccountResponse() {
				AccountGuid="DEBUGGUID-"+Guid.NewGuid().ToString(),
				AccountSecret="DEBUGSECRET-"+Guid.NewGuid().ToString(),
			};
		}

		public CreateAccountGuarantorResponse CreateAccountGuarantor(CreateAccountGuarantorRequest request) {
			return new CreateAccountGuarantorResponse() {
				AccountGuarantorGuid="DEBUGGUID-"+Guid.NewGuid().ToString(),
				AccountGuarantorNum=new Random().Next(10000,100000),
			};
		}

		public GetAccountResponse GetAccount(GetAccountRequest request) {
			throw new NotImplementedException();
		}

		public GetAccountGuarantorResponse GetAccountGuarantor(GetAccountGuarantorRequest request) {
			throw new NotImplementedException();
		}

		public GetGuarantorUsageResponse GetGuarantorUsage(GetGuarantorUsageRequest request) {			
			return GetGuarantorUsageMock(request);
		}

		public UpdateAccountGuarantorStatusResponse UpdateAccountGuarantorStatus(UpdateAccountGuarantorStatusRequest request) {
			return new UpdateAccountGuarantorStatusResponse() {
			};
		}

		public UpdateAccountStatusResponse UpdateAccountStatus(UpdateAccountStatusRequest request) {
			return new UpdateAccountStatusResponse() {
			};
		}

		public GetHealthResponse GetHealth(GetHealthRequest request) {
			throw new NotImplementedException();
		}
	}
}
