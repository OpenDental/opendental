using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.PayConnect2_Tests {
	[TestClass]
	public class PayConnect2Tests:TestBase {
		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void PayConnect2_DeserializeRawResponse_ValidStringDeserializesToCorrectEnumValue() {
			//First, setup the test scenario.
			JObject jObjectGetStatusResponse=JObject.Parse(_getStatusResponse);
			jObjectGetStatusResponse["paymentMethod"]["cardPaymentMethod"]["network"]="Visa";
			string stringGetStatusResponse=jObjectGetStatusResponse.ToString();
			//Next, perform the thing you're trying to test.
			PayConnect2.PayConnect2Response response=PayConnect2.DeserializeRawResponse(stringGetStatusResponse,PayConnect2.ApiRoute.GetStatus);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(response.GetStatusResponse.PaymentMethod.CardPaymentMethod.Network,PayConnect2.CardNetwork.Visa);
		}
		
		[TestMethod]
		public void PayConnect2_DeserializeRawResponse_BlankStringDeserializesToDefaultEnumValue() {
			//First, setup the test scenario.
			JObject jObjectGetStatusResponse=JObject.Parse(_getStatusResponse);
			jObjectGetStatusResponse["paymentMethod"]["cardPaymentMethod"]["network"]="";
			string stringGetStatusResponse=jObjectGetStatusResponse.ToString();
			//Next, perform the thing you're trying to test.
			PayConnect2.PayConnect2Response response=PayConnect2.DeserializeRawResponse(stringGetStatusResponse,PayConnect2.ApiRoute.GetStatus);
			//Finally, use one or more asserts to verify the results.
			//CardNetwork.Unrecognized is set as default in CardNetwork Enums JsonConverter attribute.
			Assert.AreEqual(response.GetStatusResponse.PaymentMethod.CardPaymentMethod.Network,PayConnect2.CardNetwork.Unrecognized);
		}

		[TestMethod]
		public void PayConnect2_DeserializeRawResponse_InvalidStringDeserializesToDefaultEnumValue() {
			//First, setup the test scenario.
			JObject jObjectGetStatusResponse=JObject.Parse(_getStatusResponse);
			jObjectGetStatusResponse["paymentMethod"]["cardPaymentMethod"]["network"]="invalidNetworkName";
			string stringGetStatusResponse=jObjectGetStatusResponse.ToString();
			//Next, perform the thing you're trying to test.
			PayConnect2.PayConnect2Response response=PayConnect2.DeserializeRawResponse(stringGetStatusResponse,PayConnect2.ApiRoute.GetStatus);
			//Finally, use one or more asserts to verify the results.
			//CardNetwork.Unrecognized is set as default in CardNetwork Enums JsonConverter attribute.
			Assert.AreEqual(response.GetStatusResponse.PaymentMethod.CardPaymentMethod.Network,PayConnect2.CardNetwork.Unrecognized);
		}

		[TestMethod]
		public void PayConnect2_DeserializeRawResponse_MissingPropertyDeserializesToLowestEnumValue() {
			//First, setup the test scenario.
			JObject jObjectGetStatusResponse=JObject.Parse(_getStatusResponse);
			((JObject)jObjectGetStatusResponse["paymentMethod"]["cardPaymentMethod"]).Remove("network");
			string stringGetStatusResponse=jObjectGetStatusResponse.ToString();
			//Next, perform the thing you're trying to test.
			PayConnect2.PayConnect2Response response=PayConnect2.DeserializeRawResponse(stringGetStatusResponse,PayConnect2.ApiRoute.GetStatus);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(response.GetStatusResponse.PaymentMethod.CardPaymentMethod.Network,PayConnect2.CardNetwork.Unrecognized);//Unrecognized==0
		}
		
		[TestMethod]
		public void PayConnect2_DeserializeRawResponse_NullPropertyDeserializesToLowestEnumValue() {
			//First, setup the test scenario.
			JObject jObjectGetStatusResponse=JObject.Parse(_getStatusResponse);
			jObjectGetStatusResponse["paymentMethod"]["cardPaymentMethod"]["network"]=null;
			string stringGetStatusResponse=jObjectGetStatusResponse.ToString();
			//Next, perform the thing you're trying to test.
			PayConnect2.PayConnect2Response response=PayConnect2.DeserializeRawResponse(stringGetStatusResponse,PayConnect2.ApiRoute.GetStatus);
			//Finally, use one or more asserts to verify the results.
			Assert.AreEqual(response.GetStatusResponse.PaymentMethod.CardPaymentMethod.Network,PayConnect2.CardNetwork.Unrecognized);//Unrecognized==0
		}

		private string _getStatusResponse=@"
		{
			""amountAuthorized"": 1000,
			""amountCaptured"": 1000,   
			""amount"": 1000,
			""transactionId"": 402323,
			""userId"": 807,
			""referenceId"": ""264401456107"",
			""type"": ""Sale"",
			""authCode"": ""PPS066"",
			""terminal"": null,
			""invoiceNumber"": null,
			""frequency"": ""OneTime"",
			""status"": ""Processed"",
			""source"": ""Integration"",
			""gatewayResponse"": {
				""account"": ""9404510976091984"",
				""amount"": ""10.00"",
				""authcode"": ""PPS066"",
				""avsresp"": ""Y"",
				""batchid"": ""143"",
				""bintype"": """",
				""commcard"": ""N"",
				""cvvresp"": ""P"",
				""entrymode"": ""ECommerce"",
				""expiry"": ""1223"",
				""merchid"": ""855000000150"",
				""orderId"": """",
				""receipt"": ""{\""dateTime\"":\""20230921153507\"",\""dba\"":\""Open Dental Retail\"",\""address2\"":\""King of Prussia, PA\"",\""phone\"":\""4845817794\"",\""footer\"":\""\"",\""nameOnCard\"":\""asd\"",\""address1\"":\""\"",\""orderNote\"":\""\"",\""header\"":\""\"",\""items\"":\""\""}"",
				""respcode"": ""000"",
				""respproc"": ""RPCT"",
				""respstat"": ""A"",
				""resptext"": ""Approval"",
				""retref"": ""264401456107"",
				""token"": ""9404510976091984""
			},
			""merchantId"": 66,
			""patientId"": null,
			""parentTransactionId"": null,
			""batchId"": 10092,    
			""paymentMethod"": {
				""paymentMethodId"": 395992,
				""type"": ""Card"",
				""patientId"": null,
				""cardPaymentMethod"": {
					""cardPaymentMethodId"": 395992,
					""paymentMethodId"": 395992,
					""cardHolder"": ""asd"",
					""cardLast4Digits"": ""1984"",
					""expiry"": ""1223"",
					""cardToken"": ""9404510976091984"",
					""network"": ""Visa"",
					""zipCode"": """",
					""updatedAt"": ""2023-09-21T19:35:07.791Z"",
					""createdAt"": ""2023-09-21T19:35:07.791Z""
				},
				""updatedAt"": ""2023-09-21T19:35:07.776Z"",
				""createdAt"": ""2023-09-21T19:35:07.776Z""
			},
			""createdAt"": ""2023-09-21T19:35:07.812Z"",
			""updatedAt"": ""2023-09-21T19:35:07.812Z""
		}";

	}
}
