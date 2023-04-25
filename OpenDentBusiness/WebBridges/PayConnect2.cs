using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using Bridges;
using OpenDentBusiness.com.dentalxchange.webservices;
using Newtonsoft.Json.Converters;

namespace OpenDentBusiness {
	public class PayConnect2 {

		//Makes a call to ODHQ to retrieve the key the first time it is referenced during execution and saves the key in memory for subsequent calls.
		private static string _payConnectApiKey=GetPayConnect2Key();

		public static string GetPayConnect2Key() {
			try {
				string payConnectApiKey=Introspection.GetOverride(Introspection.IntrospectionEntity.PayConnect2Key);
				if(payConnectApiKey=="") {//Only make call to HQ if we are not using introspection.
					payConnectApiKey=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
						.BuildOAuthUrl(PrefC.GetString(PrefName.RegistrationKey),OAuthApplicationNames.PayConnect2.ToString());
				}
				return payConnectApiKey;
			}
			catch(Exception ex) {
				throw new ODException($"Error occured retreiving PayConnect key: {ex.Message}");
			}
		}

		/// <summary>Will throw if unable to convert a double to int.</summary>
		public static int TryConvertDollarsToCents(double amount) {
			try {
				return (int)(amount*100);
			}
			catch(Exception ex) {
				throw new ODException("Unable to convert payment amount for PayConnect transaction.",ex);
			}
		}
		
		/// <summary>Will throw if unable to convert a int to double.</summary>
		public static double TryConvertCentsToDollars(int amount) {
			try {
				return (double)(amount / 100);
			}
			catch(Exception ex) {
				throw new ODException("Unable to convert payment amount for PayConnect transaction.",ex);
			}
		}

		///<summary>Builds a receipt string for a web service transaction.</summary>
		//public static string BuildReceiptString(PayConnectService.creditCardRequest request,PayConnect2Response response,
		//	PayConnectService.signatureResponse sigResponse,long clinicNum) 
		//{
		//	if(response==null) {
		//		return "";
		//	}
		//	bool doShowSignatureLine=DoShowSignatureLine(response.SignatureResponse);
		//	return BuildReceiptString(request.TransType,response.RefNumber,request.NameOnCard,request.CardNumber,request.MagData,response.AuthCode,
		//		response.Status.description,response.Messages==null ? null : response.Messages.ToList(),request.Amount,doShowSignatureLine,clinicNum);
		//}

		//public static string BuildReceiptString(Payment payment,CreditCard creditCard,PayConnect2Response payConnect2Response,
		//	string magData,string authCode,string statusDescription,List<string> messages,decimal amount,bool doShowSignatureLine,long clinicNum) 
		//{
		//	string result="";
		//	string cardnumber=paymentMethod.CardPaymentMethod.CardLast4Digits??""; //Prevents null reference exceptions when PayConnectPortal transactions don't have an associated card number
		//	int xmin=0;
		//	int xleft=xmin;
		//	int xright=15;
		//	int xmax=37;
		//	result+=Environment.NewLine;
		//	result+=CreditCardUtils.AddClinicToReceipt(clinicNum);
		//	//Print body
		//	result+="Date".PadRight(xright-xleft,'.')+DateTime.Now.ToString()+Environment.NewLine;
		//	result+=Environment.NewLine;
		//	result+="Trans Type".PadRight(xright-xleft,'.')+transType+Environment.NewLine;
		//	result+=Environment.NewLine;
		//	result+="Transaction #".PadRight(xright-xleft,'.')+refNum+Environment.NewLine;
		//	result+="Name".PadRight(xright-xleft,'.')+nameOnCard+Environment.NewLine;
		//	result+="Account".PadRight(xright-xleft,'.');
		//	for(int i = 0;i<cardNumber.Length-4;i++) {
		//		result+="*";
		//	}
		//	if(cardNumber.Length>=4) {
		//		result+=cardNumber.Substring(cardNumber.Length-4)+Environment.NewLine;//last 4 digits of card number only.
		//	}
		//	result+="Card Type".PadRight(xright-xleft,'.')+CreditCardUtils.GetCardType(cardNumber)+Environment.NewLine;
		//	result+="Entry".PadRight(xright-xleft,'.')+(String.IsNullOrEmpty(magData) ? "Manual" : "Swiped")+Environment.NewLine;
		//	result+="Auth Code".PadRight(xright-xleft,'.')+authCode+Environment.NewLine;
		//	result+="Result".PadRight(xright-xleft,'.')+statusDescription+Environment.NewLine;
		//	if(messages!=null) {
		//		string label="Message";
		//		foreach(string m in messages) {
		//			result+=label.PadRight(xright-xleft,'.')+m+Environment.NewLine;
		//			label="";
		//		}
		//	}
		//	result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
		//	if(transType.In(PayConnectService.transType.RETURN,PayConnectService.transType.VOID)) {
		//		result+="Total Amt".PadRight(xright-xleft,'.')+(amount*-1)+Environment.NewLine;
		//	}
		//	else {
		//		result+="Total Amt".PadRight(xright-xleft,'.')+amount+Environment.NewLine;
		//	}
		//	result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
		//	result+="I agree to pay the above total amount according to my card issuer/bank agreement."+Environment.NewLine;
		//	result+=Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine;
		//	if(doShowSignatureLine) {
		//		result+="Signature X".PadRight(xmax-xleft,'_');
		//	}
		//	else {
		//		result+="Electronically signed";
		//	}
		//	return result;
		//}

		//private static bool DoShowSignatureLine(AddSignatureResponse sigResponse) {
		//	if(sigResponse==null || sigResponse.Status!="Processed") {
		//		return true;//no signature was provided or it was not processed by PayConnect, show line for user to sign manually
		//	}
		//	return false;
		//}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and formatted.</summary>
		public static PayConnect2Response PostCreateTransactionByToken(Patient pat,CreditCard cc,int payAmt,long clinicNum) {
			if(cc==null || string.IsNullOrWhiteSpace(cc.PayConnectToken) || pat==null) {
				throw new ODException("Error making payment by token");
			}
			long integrationType=GetIntegrationType(clinicNum);
			PayConnect2Response response;
			if(integrationType==1) {
				CreateSurchargeTransactionRequest req=new CreateSurchargeTransactionRequest();
				req.Frequency=TransactionFrequency.OneTime;
				req.TransType=TransactionType.Sale;
				req.Amount=payAmt;
				req.CardToken=cc.PayConnectToken;
				req.Expiry=cc.PayConnectTokenExp.ToString("MMyy");
				req.CardHolder=pat.GetNameFLnoPref();
				req.ZipCode=cc.Zip;
				response=PostCreateSurchargeTransaction(req,clinicNum);
			}
			else {
				CreateTransactionRequest req=new CreateTransactionRequest();
				req.Frequency=TransactionFrequency.OneTime;
				req.TransType=TransactionType.Sale;
				req.Amount=payAmt;
				req.CardToken=cc.PayConnectToken;
				req.Expiry=cc.PayConnectTokenExp.ToString("MMyy");
				req.CardHolder=pat.GetNameFLnoPref();
				response=PostCreateTransaction(req,clinicNum);
			}
			return response;
		}

		///<summary>Can be used to create a transaction for either a Normal or Surcharge account. Body needs to be ready to send before passing in.</summary>
		public static PayConnect2Response PostCreateTransactionGeneric(string serializedBody,long clinicNum) {
			long integrationType=GetIntegrationType(clinicNum);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			ApiRoute apiRoute=ApiRoute.CreateTransaction;
			if(integrationType==1) {
				apiRoute=ApiRoute.CreateTransactionSurcharge;
			}
			PayConnect2Response response=Request(apiRoute,HttpMethod.Post,listHeaders,serializedBody);
			return response;
		}

		public static PayConnect2Response PostCreateTransaction(CreateTransactionRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings { NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.CreateTransaction,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PostCreateSurchargeTransaction(CreateSurchargeTransactionRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.CreateTransactionSurcharge,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PutVoidWithReferenceID(VoidReferenceIDRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.VoidTransaction,HttpMethod.Put,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PutVoidWithInvoiceNumber(VoidInvoiceNumberRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.VoidTransaction,HttpMethod.Put,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PostRefund(RefundRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.RefundTransaction,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PostRefundWithReferenceID(RefundReferenceIDRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.RefundTransaction,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PostRefundWithInvoiceNumber(RefundInvoiceNumberRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.RefundTransaction,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PutSignatureWithReferenceID(AddSignatureReferenceIDRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.AddSignature,HttpMethod.Put,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PutSignatureWithTransactionID(AddSignatureTransactionIDRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody);
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.AddSignature,HttpMethod.Put,listHeaders,body);
			return response;
		}

		public static PayConnect2Response GetTransactionStatus(long clinicNum,string refNumber,int transactionId=0) {
			string queryStr="";
			if(!string.IsNullOrEmpty(refNumber)) {
				queryStr=$"?referenceId={refNumber}";
			}
			if(transactionId>0) {
				queryStr=(string.IsNullOrEmpty(queryStr) ? "?":"&")+$"transactionId={transactionId}";
			}
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.GetStatus,HttpMethod.Get,listHeaders,"",queryStr);
			return response;
		}

		public static PayConnect2Response PostEmbedSession(EmbedSessionRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new StringEnumConverter());
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.EmbedSession,HttpMethod.Post,listHeaders,body);
			return response;
		}

		///<summary>Handles Debug/Introspection overrides</summary>
		public static string GetApiBaseUrl() {
			string apiUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.PayConnectRestURL,"https://api.dentalxchange.com/payments");
			if(ODBuild.IsDebug()) {
				apiUrl="https://staging-api.dentalxchange.com/payments";
			}
			return apiUrl;
		}

		public static List<string> GetHeadersForApi(long clinicNum) {
			List<string> retVal=new List<string>();
			retVal.Add("API-Key: "+_payConnectApiKey);
			retVal.Add("Secret: "+GetApiSecretForClinic(clinicNum));
			return retVal;
		}

		public static string GetApiSecretForClinic(long clinicNum) {
			Program prog=Programs.GetCur(ProgramName.PayConnect);
			return ProgramProperties.GetPropVal(prog.ProgramNum,"API Secret");
		}

		///<summary>Returns value of Integration Type program property. 0= normal account, 1= Surcharge account.</summary>
		public static long GetIntegrationType(long clinicNum) {
			Program prog=Programs.GetCur(ProgramName.PayConnect);
			return PIn.Long(ProgramProperties.GetPropVal(prog.ProgramNum,"Integration Type"));
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		private static PayConnect2Response Request(ApiRoute route,HttpMethod method,List<string> listHeaders,string body,string queryStr="") {
			if(Mock!=null) {
				return MockRequestResponse();
			}
			using(WebClient client=new WebClient()) {
				client.Headers[HttpRequestHeader.ContentType]="application/json";
				listHeaders.ForEach(x => client.Headers.Add(x));
				client.Encoding=UnicodeEncoding.UTF8;
				PayConnect2Response response=new PayConnect2Response();
				string res="";
				try {
					if(method==HttpMethod.Get) {
						res=client.DownloadString(GetApiUrl(route)+queryStr);
					}
					else if(method==HttpMethod.Post) {
						res=client.UploadString(GetApiUrl(route)+queryStr,HttpMethod.Post.Method,body);
					}
					else if(method==HttpMethod.Put) {
						res=client.UploadString(GetApiUrl(route)+queryStr,HttpMethod.Put.Method,body);
					}
					else {
						throw new Exception("Unsupported HttpMethod type: "+method.Method);
					}
					response=DeserializeRawResponse(res,route);
					///Per DXC documentation states that all repsonse codes should be 200 if successful:
					///https://developer.dentalxchange.com/payment-api
					response.httpStatusCode=HttpStatusCode.OK;
				}
				catch(WebException wex) {
					if(!(wex.Response is HttpWebResponse)) {
						throw new Exception("Error connecting to the PayConnect server:\r\n"+wex.Message,wex);
					}
					using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						res=sr.ReadToEnd();
					}
					response.ErrorResponse=JsonConvert.DeserializeObject<ErrorResponse>(res);
					response.ResponseType=ResponseType.Error;
					response.httpStatusCode=((HttpWebResponse)wex.Response).StatusCode;
				}
				catch(JsonException jEx) {
					throw new Exception("Error processing PayConnect response:\r\nResponse from PayConnect:\r\n"+res,jEx);
				}
				catch(Exception ex) {
					//WebClient returned an http status code >= 300
					ex.DoNothing();
					//For now, rethrow error and let whoever is expecting errors to handle them.
					//We may enhance this to care about codes at some point.
					throw;
				}
				return response;
			}
		}

		///<summary>Returns the full URL according to the route/route ids given.</summary>
		private static string GetApiUrl(ApiRoute route,params string[] arrayRouteIDs) {
			string apiUrl=GetApiBaseUrl();
			switch(route) {
				case ApiRoute.Root:
					//Do nothing.  This is to allow someone to quickly grab the URL without having to make a copy+paste reference.
					break;
				case ApiRoute.CreateTransaction:
					apiUrl+="/transactions";
					break;
				case ApiRoute.VoidTransaction:
					apiUrl+="/transactions/voids";
					break;
				case ApiRoute.RefundTransaction:
					apiUrl+="/transactions/refunds";
					break;
				case ApiRoute.AddSignature:
					//routeId[0]=transactionID
					apiUrl+=$"/transactions/{arrayRouteIDs[0]}/signature";
					break;
				case ApiRoute.EmbedSession:
					//routeId[0]=transactionID
					apiUrl+=$"/embeds/sessions";
					break;
				case ApiRoute.GetStatus:
					//routeId[0]=transactionID
					apiUrl+=$"/transactions/status";
					break;
				default:
					break;
			}
			return apiUrl;
		}

		public enum ApiRoute {
			Root,
			AddSignature,
			EmbedSession,
			CreateTransaction,
			CreateTransactionSurcharge,//Same route as CreateTransaction but helps us to differentiate for serializing/deserializing
			RefundTransaction,
			VoidTransaction,
			GetStatus,
		}

		#region Converters

		/// <summary>Can throw exceptions if deserialization fails.</summary>
		public static PayConnect2Response DeserializeRawResponse(string rawResponse,ApiRoute route) {
			PayConnect2Response payConnect2Response=new PayConnect2Response();
			var settings=new JsonSerializerSettings {
        NullValueHandling=NullValueHandling.Ignore,
        MissingMemberHandling=MissingMemberHandling.Ignore
      };
			switch(route) {
					case ApiRoute.CreateTransaction:
					payConnect2Response.TransactionResponse=JsonConvert.DeserializeObject<CreateTransactionResponse>(rawResponse,settings);
					payConnect2Response.ResponseType=ResponseType.CreateTransaction;
						break;
				case ApiRoute.CreateTransactionSurcharge:
					payConnect2Response.TransactionSurchargeResponse=JsonConvert.DeserializeObject<CreateSurchargeTransactionResponse>(rawResponse,settings);
					payConnect2Response.ResponseType=ResponseType.CreateSurchargeTransaction;
						break;
				case ApiRoute.VoidTransaction:
					payConnect2Response.VoidResponse=JsonConvert.DeserializeObject<VoidResponse>(rawResponse,settings);
					payConnect2Response.ResponseType=ResponseType.Void;
						break;
				case ApiRoute.RefundTransaction:
					payConnect2Response.RefundResponse=JsonConvert.DeserializeObject<RefundResponse>(rawResponse,settings);
					payConnect2Response.ResponseType=ResponseType.Refund;
						break;
				case ApiRoute.AddSignature:
					payConnect2Response.SignatureResponse=JsonConvert.DeserializeObject<AddSignatureResponse>(rawResponse,settings);
					payConnect2Response.ResponseType=ResponseType.AddSignature;
						break;
				case ApiRoute.EmbedSession:
					payConnect2Response.EmbedSessionResponse=JsonConvert.DeserializeObject<EmbedSessionResponse>(rawResponse,settings);
					payConnect2Response.ResponseType=ResponseType.EmbedSession;
						break;
				case ApiRoute.GetStatus:
					payConnect2Response.GetStatusResponse=JsonConvert.DeserializeObject<GetStatusResponse>(rawResponse,settings);
					payConnect2Response.ResponseType=ResponseType.GetStatus;
					break;
				default:
						break;
				}
			return payConnect2Response;
		}
		#endregion Converters


		#region Request/Response Objects
		#region Request Objects

		[DataContract]
		public class CreateTransactionRequest {
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency"),JsonConverter(typeof(StringEnumConverter))]
			public TransactionFrequency Frequency;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type"),JsonConverter(typeof(StringEnumConverter))]
			public TransactionType TransType;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Tokenized card number</summary>
			[DataMember(Name = "cardToken")]
			public string CardToken;
			///<summary>Expiry in MMYY or YYYYMM format</summary>
			[DataMember(Name = "expiry")]
			public string Expiry;
			///<summary>Security code for the card</summary>
			[DataMember(Name = "cvv",IsRequired=false)]
			public string Cvv;
			///<summary>Full name of the card holder</summary>
			[DataMember(Name = "cardHolder")]
			public string CardHolder;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber",IsRequired=false)]
			public string InvoiceNumber;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient",IsRequired=false)]
			public PayConnectPatient Patient;

			//public CreateTransactionRequest(CreditCard creditCard, Patient patient,Payment payment, TransactionType transactionType, TransactionFrequency transactionFrequency=TransactionFrequency.OneTime) {
			//	Frequency=transactionFrequency;//Should always be OneTime, recurring payments are handled within OD.
			//	TransType=transactionType;

			//}
		}

		[DataContract]
		public class CreateSurchargeTransactionRequest {
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency")]
			public TransactionFrequency Frequency;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type")]
			public TransactionType TransType;
			///<summary>Amount in cents. NOTE: the PayConnect API accepts decimals but it behaves in a very strange manner when a decimal is sent.</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Tokenized card number</summary>
			[DataMember(Name = "cardToken")]
			public string CardToken;
			///<summary>Expiry in MMYY or YYYYMM format</summary>
			[DataMember(Name = "expiry")]
			public string Expiry;
			///<summary>Security code for the card</summary>
			[DataMember(Name = "cvv",IsRequired=false)]
			public string Cvv;
			///<summary>Full name of the card holder</summary>
			[DataMember(Name = "cardHolder")]
			public string CardHolder;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber",IsRequired=false)]
			public string InvoiceNumber;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient",IsRequired=false)]
			PayConnectPatient Patient;
			///<summary>ZipCode of the card holder</summary>
			[DataMember(Name = "zipCode")]
			public string ZipCode;
		}

		[DataContract]
		public class VoidInvoiceNumberRequest {
			///<summary>Invoice Number</summary>
			[DataMember(Name = "invoiceNumber")]
			public string InvoiceNumber;
		}

		[DataContract]
		public class VoidReferenceIDRequest {
			///<summary>ReferenceID</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
		}

		[DataContract]
		public class RefundRequest {
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency")]
			public TransactionFrequency Frequency;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type")]
			public TransactionType TransType;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Tokenized card number</summary>
			[DataMember(Name = "cardToken")]
			public string CardToken;
			///<summary>Expiry in MMYY or YYYYMM format</summary>
			[DataMember(Name = "expiry")]
			public string Expiry;
			///<summary>Security code for the card</summary>
			[DataMember(Name = "cvv",IsRequired=false)]
			public string Cvv;
			///<summary>Full name of the card holder</summary>
			[DataMember(Name = "cardHolder")]
			public string CardHolder;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber",IsRequired=false)]
			public string InvoiceNumber;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient",IsRequired=false)]
			public PayConnectPatient Patient;
		}

		public class RefundInvoiceNumberRequest {
			///<summary>Invoice Number</summary>
			[DataMember(Name = "invoiceNumber")]
			public string InvoiceNumber;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount",IsRequired=false)]
			public int Amount;
		}

		[DataContract]
		public class RefundReferenceIDRequest {
			///<summary>ReferenceID</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount",IsRequired=false)]
			public int Amount;
		}

		[DataContract]
		public class AddSignatureReferenceIDRequest {
			///<summary>Reference ID of the transaction to assign the signature</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
			///<summary>Base64 string representation of the signature image</summary>
			[DataMember(Name = "signature")]
			public string Signature;
		}

		[DataContract]
		public class AddSignatureTransactionIDRequest {
			///<summary>ID of the transaction to assign the signature</summary>
			[DataMember(Name = "transactionId")]
			public string TransactionId;
			///<summary>Base64 string representation of the signature image</summary>
			[DataMember(Name = "signature")]
			public string Signature;
		}

		[DataContract]
		public class EmbedSessionRequest {
			///<summary>Base64 string representation of the signature image. Default is Tokenizer. Required.</summary>
			[DataMember(Name = "type")]
			public IframeType Type;
			///<summary>Amount in cents. If the amount is provided, Payment iFrame will disable the amount field.</summary>
			[DataMember(Name = "amount",IsRequired=false)]
			public int Amount;
			///<summary>Provide fields for patient information. Defaults to false.</summary>
			[DataMember(Name = "usePatient",IsRequired=false)]
			public bool UsePatient;
			///<summary>Provide a field for invoice number. Defaults to false.</summary>
			[DataMember(Name = "useInvoiceNumber",IsRequired=false)]
			public bool UseInvoiceNumber;
			///<summary>Custom submit button label for the iFrame. Default: "Process"</summary>
			[DataMember(Name = "buttonLabel",IsRequired=false)]
			public string ButtonLabel;
		}

		public class TransactionStatusRequest {
			///<summary>Reference ID of the transaction</summary>
			[DataMember(Name = "referenceId",IsRequired=false)]
			public string ReferenceId;
			///<summary>ID of the transaction</summary>
			[DataMember(Name = "transactionId",IsRequired=false)]
			public string TransactionId;
		}

		#endregion Request Objects
		#region Response Objects

		///<summary>Wrapper response class for passing API response on the Open Dental side. Check ResponseType to see if it contains your expected type or an error.</summary>
		public class PayConnect2Response {
			public ResponseType ResponseType;
			public AddSignatureResponse SignatureResponse;
			public CreateTransactionResponse TransactionResponse;
			public CreateSurchargeTransactionResponse TransactionSurchargeResponse;
			public ErrorResponse ErrorResponse;
			public RefundResponse RefundResponse;
			public VoidResponse VoidResponse;
			public EmbedSessionResponse EmbedSessionResponse;
			public GetStatusResponse GetStatusResponse;
			public HttpStatusCode httpStatusCode;
		}

		[DataContract]
		public class CreateTransactionResponse {
			///<summary>Amount authorized in cents</summary>
			[DataMember(Name = "amountAuthorized")]
			public int AmountAuthorized;
			///<summary>Amount captured in cents</summary>
			[DataMember(Name = "amountCaptured")]
			public int AmountCaptured;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Internal transaction id</summary>
			[DataMember(Name = "transactionId")]
			public int TransactionId;
			///<summary>The user who ran the transaction</summary>
			[DataMember(Name = "userId")]
			public int UserId;
			///<summary>Transaction reference number</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type")]
			public TransactionType TransType;
			///<summary>Authorization code send by the gateway</summary>
			[DataMember(Name = "authCode")]
			public string AuthCode;
			///<summary>Ternimal ID, if the transaction was run fron a terminal</summary>
			[DataMember(Name = "terminal")]
			public string Terminal;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber")]
			public string InvoiceNumber;
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency")]
			public TransactionFrequency Frequency;
			///<summary>Status</summary>
			[DataMember(Name = "status")]
			public string Status;
			///<summary>Transaction origination point</summary>
			[DataMember(Name = "source")]
			public TransactionSource Source;
			///<summary>Raw response form the gateway</summary>
			[DataMember(Name = "gatewayResponse")]
			public object GatewayResponse;
			///<summary>Merchant identification number</summary>
			[DataMember(Name = "merchantId")]
			public int MerchantId;
			///<summary>Patient identification number. From testing can be null.</summary>
			[DataMember(Name = "patientId")]
			public int? PatientId;
			///<summary>Returned if the transaction is refund or void</summary>
			[DataMember(Name = "parentTransactionId")]
			public int? ParentTransactionId;
			///<summary>Batch identification number the transaction belongs to</summary>
			[DataMember(Name = "batchId")]
			public int BatchId;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient")]
			PayConnectPatient Patient;
			///<summary></summary>
			[DataMember(Name = "paymentMethod")]
			public PaymentMethod PaymentMethod;
			///<summary>Date the transaction was created</summary>
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			///<summary>Date the transaction was updated</summary>
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
		}

		[DataContract]
		public class CreateSurchargeTransactionResponse {
			///<summary>Amount authorized in cents</summary>
			[DataMember(Name = "amountAuthorized")]
			public int AmountAuthorized;
			///<summary>Amount captured in cents</summary>
			[DataMember(Name = "amountCaptured")]
			public int AmountCaptured;
			///<summary>Amount surcharged in cents</summary>
			[DataMember(Name = "amountSurcharged")]
			public int AmountSurcharged;
			///<summary>Percentage of the amount surcharged</summary>
			[DataMember(Name = "surchargePercent")]
			public int SurchargePercent;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Internal transaction id</summary>
			[DataMember(Name = "transactionId")]
			public int TransactionId;
			///<summary>The user who ran the transaction</summary>
			[DataMember(Name = "userId")]
			public int UserId;
			///<summary>Transaction reference number</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type")]
			public TransactionType TransType;
			///<summary>Authorization code send by the gateway</summary>
			[DataMember(Name = "authCode")]
			public string AuthCode;
			///<summary>Ternimal ID, if the transaction was run fron a terminal</summary>
			[DataMember(Name = "terminal")]
			public string Terminal;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber")]
			public string InvoiceNumber;
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency")]
			public TransactionFrequency Frequency;
			///<summary>Status</summary>
			[DataMember(Name = "status")]
			public string Status;
			///<summary>Transaction origination point</summary>
			[DataMember(Name = "source")]
			public TransactionSource Source;
			///<summary>Raw response form the gateway</summary>
			[DataMember(Name = "gatewayResponse")]
			public object GatewayResponse;
			///<summary>Merchant identification number</summary>
			[DataMember(Name = "merchantId")]
			public int MerchantId;
			///<summary>Patient identification number</summary>
			[DataMember(Name = "patientId")]
			public int PatientId;
			///<summary>Returned if the transaction is refund or void</summary>
			[DataMember(Name = "parentTransactionId")]
			public int ParentTransactionId;
			///<summary>Batch identification number the transaction belongs to</summary>
			[DataMember(Name = "batchId")]
			public int BatchId;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient")]
			PayConnectPatient Patient;
			///<summary></summary>
			[DataMember(Name = "paymentMethod")]
			public PaymentMethod PaymentMethod;
			///<summary>Date the transaction was created</summary>
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			///<summary>Date the transaction was updated</summary>
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
		}

		[DataContract]
		public class VoidResponse {
			///<summary>Amount authorized in cents</summary>
			[DataMember(Name = "amountAuthorized")]
			public int AmountAuthorized;
			///<summary>Amount captured in cents</summary>
			[DataMember(Name = "amountCaptured")]
			public int AmountCaptured;
			///<summary>Amount voided in cents</summary>
			[DataMember(Name = "amountVoided")]
			public int AmountVoided;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Internal transaction id</summary>
			[DataMember(Name = "transactionId")]
			public int TransactionId;
			///<summary>The user who ran the transaction</summary>
			[DataMember(Name = "userId")]
			public int UserId;
			///<summary>Transaction reference number</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type")]
			public TransactionType TransType;
			///<summary>Authorization code send by the gateway</summary>
			[DataMember(Name = "authCode")]
			public string AuthCode;
			///<summary>Ternimal ID, if the transaction was run fron a terminal</summary>
			[DataMember(Name = "terminal")]
			public string Terminal;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber")]
			public string InvoiceNumber;
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency")]
			public TransactionFrequency Frequency;
			///<summary>Status</summary>
			[DataMember(Name = "status")]
			public string Status;
			///<summary>Transaction origination point</summary>
			[DataMember(Name = "source")]
			public TransactionSource Source;
			///<summary>Raw response form the gateway</summary>
			[DataMember(Name = "gatewayResponse")]
			public object GatewayResponse;
			///<summary>Merchant identification number</summary>
			[DataMember(Name = "merchantId")]
			public int MerchantId;
			///<summary>Patient identification number</summary>
			[DataMember(Name = "patientId")]
			public int PatientId;
			///<summary>Returned if the transaction is refund or void</summary>
			[DataMember(Name = "parentTransactionId")]
			public int ParentTransactionId;
			///<summary>Batch identification number the transaction belongs to</summary>
			[DataMember(Name = "batchId")]
			public int BatchId;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient")]
			PayConnectPatient Patient;
			///<summary></summary>
			[DataMember(Name = "paymentMethod")]
			public PaymentMethod PaymentMethod;
			///<summary>Date the transaction was created</summary>
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			///<summary>Date the transaction was updated</summary>
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
		}

		[DataContract]
		public class RefundResponse {
			///<summary>Amount authorized in cents</summary>
			[DataMember(Name = "amountAuthorized")]
			public int AmountAuthorized;
			///<summary>Amount captured in cents</summary>
			[DataMember(Name = "amountCaptured")]
			public int AmountCaptured;
			///<summary>Amount refunded in cents</summary>
			[DataMember(Name = "amountRefunded")]
			public int AmountRefunded;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Internal transaction id</summary>
			[DataMember(Name = "transactionId")]
			public int TransactionId;
			///<summary>The user who ran the transaction</summary>
			[DataMember(Name = "userId")]
			public int UserId;
			///<summary>Transaction reference number</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type")]
			public TransactionType TransType;
			///<summary>Authorization code send by the gateway</summary>
			[DataMember(Name = "authCode")]
			public string AuthCode;
			///<summary>Ternimal ID, if the transaction was run fron a terminal</summary>
			[DataMember(Name = "terminal")]
			public string Terminal;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber")]
			public string InvoiceNumber;
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency")]
			public TransactionFrequency Frequency;
			///<summary>Status</summary>
			[DataMember(Name = "status")]
			public string Status;
			///<summary>Transaction origination point</summary>
			[DataMember(Name = "source")]
			public TransactionSource Source;
			///<summary>Raw response form the gateway</summary>
			[DataMember(Name = "gatewayResponse")]
			public object GatewayResponse;
			///<summary>Merchant identification number</summary>
			[DataMember(Name = "merchantId")]
			public int MerchantId;
			///<summary>Patient identification number</summary>
			[DataMember(Name = "patientId")]
			public int PatientId;
			///<summary>Returned if the transaction is refund or void</summary>
			[DataMember(Name = "parentTransactionId")]
			public int ParentTransactionId;
			///<summary>Batch identification number the transaction belongs to</summary>
			[DataMember(Name = "batchId")]
			public int BatchId;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient")]
			PayConnectPatient Patient;
			///<summary></summary>
			[DataMember(Name = "paymentMethod")]
			public PaymentMethod PaymentMethod;
			///<summary>Date the transaction was created</summary>
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			///<summary>Date the transaction was updated</summary>
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
		}

		public class AddSignatureResponse {
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Internal transaction id</summary>
			[DataMember(Name = "transactionId")]
			public int TransactionId;
			///<summary>The user who ran the transaction</summary>
			[DataMember(Name = "userId")]
			public int UserId;
			///<summary>Transaction reference number</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type")]
			public TransactionType TransType;
			///<summary>Authorization code send by the gateway</summary>
			[DataMember(Name = "authCode")]
			public string AuthCode;
			///<summary>Ternimal ID, if the transaction was run fron a terminal</summary>
			[DataMember(Name = "terminal")]
			public string Terminal;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber")]
			public string InvoiceNumber;
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency")]
			public TransactionFrequency Frequency;
			///<summary>Status</summary>
			[DataMember(Name = "status")]
			public string Status;
			///<summary>Transaction origination point</summary>
			[DataMember(Name = "source")]
			public TransactionSource Source;
			///<summary>Raw response form the gateway</summary>
			[DataMember(Name = "gatewayResponse")]
			public object GatewayResponse;
			///<summary>Merchant identification number</summary>
			[DataMember(Name = "merchantId")]
			public int MerchantId;
			///<summary>Patient identification number</summary>
			[DataMember(Name = "patientId")]
			public int PatientId;
			///<summary>Returned if the transaction is refund or void</summary>
			[DataMember(Name = "parentTransactionId")]
			public int ParentTransactionId;
			///<summary>Batch identification number the transaction belongs to</summary>
			[DataMember(Name = "batchId")]
			public int BatchId;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient")]
			PayConnectPatient Patient;
			///<summary></summary>
			[DataMember(Name = "paymentMethod")]
			public PaymentMethod PaymentMethod;
			///<summary>Date the transaction was created</summary>
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			///<summary>Date the transaction was updated</summary>
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
		}

		[DataContract]
		public class GetStatusResponse {
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amountAuthorized")]
			public int AmountAuthorized;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amountCaptured")]
			public int AmountCaptured;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amountSurcharged")]
			public int AmountSurcharged;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "surchargePercent")]
			public int SurchargePercent;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Internal transaction id</summary>
			[DataMember(Name = "transactionId")]
			public int TransactionId;
			///<summary>The user who ran the transaction</summary>
			[DataMember(Name = "userId")]
			public int UserId;
			///<summary>Transaction reference number</summary>
			[DataMember(Name = "referenceId")]
			public string ReferenceId;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type")]
			public TransactionType TransType;
			///<summary>Authorization code send by the gateway</summary>
			[DataMember(Name = "authCode")]
			public string AuthCode;
			///<summary>Ternimal ID, if the transaction was run fron a terminal</summary>
			[DataMember(Name = "terminal")]
			public string Terminal;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber")]
			public string InvoiceNumber;
			///<summary>Identidy if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency")]
			public TransactionFrequency Frequency;
			///<summary>Status</summary>
			[DataMember(Name = "status")]
			public string Status;
			///<summary>Transaction origination point</summary>
			[DataMember(Name = "source")]
			public TransactionSource Source;
			///<summary>Raw response form the gateway</summary>
			[DataMember(Name = "gatewayResponse")]
			public object GatewayResponse;
			///<summary>Merchant identification number</summary>
			[DataMember(Name = "merchantId")]
			public int MerchantId;
			///<summary>Patient identification number</summary>
			[DataMember(Name = "patientId")]
			public int PatientId;
			///<summary>Returned if the transaction is refund or void</summary>
			[DataMember(Name = "parentTransactionId")]
			public int ParentTransactionId;
			///<summary>Batch identification number the transaction belongs to</summary>
			[DataMember(Name = "batchId")]
			public int BatchId;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient")]
			PayConnectPatient Patient;
			///<summary></summary>
			[DataMember(Name = "paymentMethod")]
			public PaymentMethod PaymentMethod;
			///<summary>Date the transaction was created</summary>
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			///<summary>Date the transaction was updated</summary>
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
		}

		public class EmbedSessionResponse {
			///<summary>Secure session URL to be added in the iFrame</summary>
			[DataMember(Name = "url")]
			public string Url;
		}

		[DataContract]
		public class PaymentMethod {
			public int paymentMedthodId;
			[DataMember(Name = "type")]
			public PaymentMethodType MethodType;
			[DataMember(Name = "patientId")]
			public int? PatientId;
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
			[DataMember(Name = "cardPaymentMedthod")]
			public CardPaymentMethod CardPaymentMethod;

			public enum PaymentMethodType {
				Card,
				ACH,
			}
		}

		[DataContract]
		public class CardPaymentMethod {
			[DataMember(Name = "cardPaymentMedthodId")]
			public int CardPaymentMedthodId;
			[DataMember(Name = "paymentMethodId")]
			public int PaymentMethodId;
			[DataMember(Name = "cardHolder")]
			public string CardHolder;
			[DataMember(Name = "cardLast4Digits")]
			public string CardLast4Digits;
			[DataMember(Name = "expiry")]
			public string Expiry;
			[DataMember(Name = "cardToken")]
			public string CardToken;
			[DataMember(Name = "network")]
			public CardNetwork Network;
			[DataMember(Name = "zipCode")]
			public string ZipCode;
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
			
		}

		public class PayConnectPatient {
			///<summary>Firstname of patient. Required.</summary>
			[DataMember(Name = "firstName")]
			public string FirstName;
			///<summary>LastName of patient. Required.</summary>
			[DataMember(Name = "lastName")]
			public string LastName;
			///<summary>Street line 1 in address</summary>
			[DataMember(Name = "streetAddress1")]
			public string StreetAddress1;
			///<summary>Street line 2 in address</summary>
			[DataMember(Name = "streetAddress2")]
			public string StreetAddress2;
			///<summary>City in address</summary>
			[DataMember(Name = "city")]
			public string City;
			///<summary>State in address</summary>
			[DataMember(Name = "state")]
			public string State;
			///<summary>ZipCode in address</summary>
			[DataMember(Name = "zipCode")]
			public string ZipCode;
			///<summary>Phone number of patient</summary>
			[DataMember(Name = "phoneNumber")]
			public string PhoneNumber;
			///<summary>Email of patient</summary>
			[DataMember(Name = "email")]
			public string Email;
		}

		[DataContract]
		public class ErrorResponse {
			[DataMember(Name = "errorType")]
			public ErrorType ErrorType;
			///<summary>Will be of type ValidationError or ReponseError</summary>
			[DataMember(Name = "error")]
			public object Error;
		}

		[DataContract]
		public class ResponseError {
			[DataMember(Name = "message")]
			public string Message;
			[DataMember(Name = "code")]
			public int Code;
		}

		[DataContract]
		public class ValidationError {
			[DataMember(Name = "type")]
			public object ErrorType;
			[DataMember(Name = "properties")]
			public object Properties;
		}
		#endregion Response Objects

		public enum TransactionType {
			Sale,
			AuthorizeOnly,
		}

		public enum TransactionFrequency {
			OneTime,
			Recurring,
		}

		public enum TransactionSource {
			ThirdParty,
			Portal,
			Terminal,
			Integration,
		}

		public enum CardNetwork {
			Amex,
			Discover,
			Mastercard,
			[Description("Non-co-branded debit card")]
			NonCoBrandedDebitCard,
			Visa,
		}

		public enum IframeType {
			Tokenizer,
			Payment,
		}

		public enum ErrorType {
			[Description("RESPONSE")]
			Response,
			[Description("VALIDATION")]
			Validation,
		}

		public enum ResponseType {
			[Description("Add Signature")]
			AddSignature,
			[Description("Transaction")]
			CreateTransaction,
			[Description("Transaction")]
			CreateSurchargeTransaction,
			[Description("RESPONSE")]
			Error,
			Refund,
			Void,
			EmbedSession,
			GetStatus,
		}
		#endregion Request/Response Objects

		#region Mocks for UnitTesting

		public static MockPayConnect2 Mock;
		public class MockPayConnect2 {
			public bool IsPostCreateTransactionRequest;
			public PayConnect2Response Response;
		}

		private static PayConnect2Response MockRequestResponse() {
			return Mock.Response;
		}
		#endregion
	}
}
