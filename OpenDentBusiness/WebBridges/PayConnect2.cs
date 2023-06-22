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
using System.Windows.Controls;

namespace OpenDentBusiness {
	public class PayConnect2 {

		/// <summary>Do not access directly, use GetPayConnect2ApiKey() to ensure that this variable has been properly initialized.</summary>
		private static string _payConnectApiKey;
		/// <summary>Do not access directly, use GetPayConnect2DLLKey() to ensure that this variable has been properly initialized.</summary>
		private static string _payConnectDllKey;

		/// <summary>Initializes the _payConnectApiKey variable if it has not been already.</summary>
		public static string GetPayConnect2ApiKey() {
			if(_payConnectApiKey.IsNullOrEmpty()) {
				try {
					_payConnectApiKey=Introspection.GetOverride(Introspection.IntrospectionEntity.PayConnect2Key);
					if(_payConnectApiKey.IsNullOrEmpty()) {//Only make call to HQ if we are not using introspection.
						_payConnectApiKey=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
							.BuildOAuthUrl(PrefC.GetString(PrefName.RegistrationKey),OAuthApplicationNames.PayConnect2.ToString());
					}
				}	
				catch(Exception ex) {
					throw new ODException($"Error occurred retrieving PayConnect key: {ex.Message}");
				}
			}
			return _payConnectApiKey;
		}

		public static string GetPayConnect2DLLKey() {
			if(_payConnectDllKey.IsNullOrEmpty()) {
				try {
					_payConnectDllKey=Introspection.GetOverride(Introspection.IntrospectionEntity.PayConnect2DLLKey);
					if(_payConnectDllKey.IsNullOrEmpty()) {//Only make call to HQ if we are not using introspection.
						_payConnectDllKey=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
							.BuildOAuthUrl(PrefC.GetString(PrefName.RegistrationKey),OAuthApplicationNames.PayConnect2.ToString());
					}
				}	
				catch(Exception ex) {
					throw new ODException($"Error occurred retrieving PayConnect key: {ex.Message}");
				}
			}
			return _payConnectDllKey;
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and formatted.</summary>
		public static PayConnect2Response PostCreateTransactionByToken(Patient pat,CreditCard cc,int payAmtInCents,long clinicNum,TransactionType transactionType=TransactionType.Sale) {
			if(cc==null || string.IsNullOrWhiteSpace(cc.PayConnectToken) || pat==null) {
				throw new ODException("Error making payment by token");
			}
			return PostCreateTransactionByToken(pat,cc.CCExpiration.ToString("MMyy"),cc.PayConnectToken,cc.Zip,payAmtInCents,clinicNum,transactionType);
		}

		public static PayConnect2Response PostCreateTransactionByToken(Patient pat,string ccExpiration,string token,string zipCode,int payAmtInCents,long clinicNum,TransactionType transactionType=TransactionType.Sale) {
			long integrationType=GetIntegrationType(clinicNum);
			PayConnect2Response response;
			if(integrationType==1) {
				CreateSurchargeTransactionRequest req=new CreateSurchargeTransactionRequest();
				req.Frequency=TransactionFrequency.OneTime;
				req.TransType=transactionType;
				req.Amount=payAmtInCents;
				req.CardToken=token;
				req.Expiry=ccExpiration;
				req.CardHolder=pat.GetNameFLnoPref();
				req.ZipCode=zipCode;
				response=PostCreateSurchargeTransaction(req,clinicNum);
			}
			else {
				CreateTransactionRequest req=new CreateTransactionRequest();
				req.Frequency=TransactionFrequency.OneTime;
				req.TransType=transactionType;
				req.Amount=payAmtInCents;
				req.CardToken=token;
				req.Expiry=ccExpiration;
				req.CardHolder=pat.GetNameFLnoPref();
				response=PostCreateTransaction(req,clinicNum);
			}
			return response;
		}

		public static PayConnect2Response PostCreateTransaction(CreateTransactionRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings { NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.CreateTransaction,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PostCreateSurchargeTransaction(CreateSurchargeTransactionRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.CreateTransactionSurcharge,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PutVoidWithReferenceID(VoidReferenceIDRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.VoidTransaction,HttpMethod.Put,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PutVoidWithInvoiceNumber(VoidInvoiceNumberRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.VoidTransaction,HttpMethod.Put,listHeaders,body);
			return response;
		}

		//Deprecated May 1st 2023 per DXC
		//public static PayConnect2Response PostRefund(RefundRequest requestBody,long clinicNum) {
		//	string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
		//	List<string> listHeaders=GetHeadersForApi(clinicNum);
		//	PayConnect2Response response=Request(ApiRoute.RefundTransaction,HttpMethod.Post,listHeaders,body);
		//	return response;
		//}

		public static PayConnect2Response PostRefundWithReferenceID(RefundReferenceIDRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.RefundTransaction,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PostRefundWithInvoiceNumber(RefundInvoiceNumberRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.RefundTransaction,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PutSignatureWithReferenceID(AddSignatureReferenceIDRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.AddSignature,HttpMethod.Put,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PutSignatureWithTransactionID(AddSignatureTransactionIDRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
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
			requestBody.ClientType="Patient";//This forces Surcharge fees to always show in the iFrame.
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.EmbedSession,HttpMethod.Post,listHeaders,body);
			return response;
		}

		public static PayConnect2Response PostCreateTerminalTransaction(CreateTerminalTransactionRequest requestBody,long clinicNum) {
			string body=JsonConvert.SerializeObject(requestBody,new JsonSerializerSettings {NullValueHandling=NullValueHandling.Ignore});
			List<string> listHeaders=GetHeadersForApi(clinicNum);
			PayConnect2Response response=Request(ApiRoute.CreateTerminalTransaction,HttpMethod.Post,listHeaders,body);
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
			retVal.Add("API-Key: "+GetPayConnect2ApiKey());
			retVal.Add("Secret: "+GetApiSecretForClinic(clinicNum));
			return retVal;
		}

		public static string GetApiSecretForClinic(long clinicNum) {
			Program prog=Programs.GetCur(ProgramName.PayConnect);
			return ProgramProperties.GetPropVal(prog.ProgramNum,"API Secret",clinicNum);
		}

		///<summary>Returns value of Integration Type program property. 0= normal account, 1= Surcharge account.</summary>
		public static long GetIntegrationType(long clinicNum) {
			Program prog=Programs.GetCur(ProgramName.PayConnect);
			return PIn.Long(ProgramProperties.GetPropVal(prog.ProgramNum,"PayConnect2.0 Integration Type: 0 for normal, 1 for surcharge",clinicNum));
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
						response.ErrorResponse=new ErrorResponse();
						response.ErrorResponse.Error="Error connecting to the PayConnect server:\r\n"+wex.Message;
						response.ErrorResponse.ErrorType=ErrorType.Response;
						response.ResponseType=ResponseType.Error;
					}
					else {
						using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
							res=sr.ReadToEnd();
						}
						response.ErrorResponse=JsonConvert.DeserializeObject<ErrorResponse>(res);
						response.ResponseType=ResponseType.Error;
						response.httpStatusCode=((HttpWebResponse)wex.Response).StatusCode;
					}
					
				}
				catch(JsonException jEx) {
					response.ErrorResponse=new ErrorResponse();
					response.ErrorResponse.Error="Error processing PayConnect response:\r\nResponse from PayConnect:\r\n"+res;
					response.ErrorResponse.ErrorType=ErrorType.Response;
					response.ResponseType=ResponseType.Error;
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
				case ApiRoute.CreateTransactionSurcharge:
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
					apiUrl+=$"/transactions/signature";
					break;
				case ApiRoute.EmbedSession:
					//routeId[0]=transactionID
					apiUrl+=$"/embeds/sessions";
					break;
				case ApiRoute.GetStatus:
					//routeId[0]=transactionID
					apiUrl+=$"/transactions/status";
					break;
				case ApiRoute.CreateTerminalTransaction:
					apiUrl+=$"/terminals/transactions";
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
			CreateTerminalTransaction,
		}

		#region Converters

		/// <summary>Can throw exceptions if deserialization fails.</summary>
		public static PayConnect2Response DeserializeRawResponse(string rawResponse,ApiRoute route) {
			PayConnect2Response payConnect2Response=new PayConnect2Response();
			JsonSerializerSettings settings=new JsonSerializerSettings();
        	settings.NullValueHandling=NullValueHandling.Ignore;
        	settings.MissingMemberHandling=MissingMemberHandling.Ignore;
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
				case ApiRoute.CreateTerminalTransaction:
					payConnect2Response.TerminalTransactionResponse=JsonConvert.DeserializeObject<TerminalTransactionResponse>(rawResponse,settings);
					payConnect2Response.ResponseType=ResponseType.TerminalTransaction;
					break;
				default:
						break;
			}
			return payConnect2Response;
		}

		/// <summary>Throws exceptions. The PayConnect2 API accepts an integer amount field in total cents i.e. $13.62 should be sent as 1362. This method takes in a double as that is what Open Dental generally uses to store dollar amounts and multiplies it by 100 to move the decimal 2 places and then casts to an int.</summary>
		public static int FormatAmountForApi(double amount) {
			try {
				return (int)(amount*100);
			}
			catch(Exception ex) {
				throw new ODException("Unable to convert transaction amount for PayConnect.",ex);
			}
		}

		/// <summary>Open Dental stores a double for monetary amounts. This method takes in an int as PayConnect2 represents money in cents and divides it by 100 to move the decimal 2 places and then casts to a double.</summary>
		public static double FormatAmountForOpenDental(int amount) {
			return ((double)amount)/100;
		}

		///<summary>Converts an any response we may have gotten from the API to the old PayConnectResponse class.</summary>
		public static PayConnectResponse ApiResponseToPayConnectResponse(PayConnect2Response response) {
			PayConnectResponse payConnectResponse=new PayConnectResponse();
			switch(response.ResponseType) {
				case ResponseType.CreateTransaction:
					CreateTransactionResponse transactionResponse=response.TransactionResponse;
					payConnectResponse.Description=transactionResponse.Status;
					payConnectResponse.StatusCode="0";//0 indicates success
					payConnectResponse.RefNumber=transactionResponse.ReferenceId;
					payConnectResponse.Amount=((decimal)transactionResponse.Amount)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					payConnectResponse.AuthCode=transactionResponse.AuthCode;
					payConnectResponse.MerchantId=transactionResponse.MerchantId.ToString();
					payConnectResponse.PaymentToken=transactionResponse.PaymentMethod.CardPaymentMethod.CardToken;
					payConnectResponse.CardType=transactionResponse.PaymentMethod.CardPaymentMethod.Network.ToString();
					payConnectResponse.CardNumber=transactionResponse.PaymentMethod.CardPaymentMethod.CardLast4Digits;
					payConnectResponse.TransType=PayConnectResponse.TransactionType.Sale;
					break;
				case ResponseType.CreateSurchargeTransaction:
					CreateSurchargeTransactionResponse transactionSurchargeResponse=response.TransactionSurchargeResponse;
					payConnectResponse.Description=transactionSurchargeResponse.Status;
					payConnectResponse.StatusCode="0";//0 indicates success
					payConnectResponse.RefNumber=transactionSurchargeResponse.ReferenceId;
					payConnectResponse.Amount=((decimal)transactionSurchargeResponse.Amount)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					payConnectResponse.SurchargePercent=transactionSurchargeResponse.SurchargePercent;
					payConnectResponse.AmountSurcharged=((decimal)transactionSurchargeResponse.AmountSurcharged)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					payConnectResponse.AuthCode=transactionSurchargeResponse.AuthCode;
					payConnectResponse.MerchantId=transactionSurchargeResponse.MerchantId.ToString();
					payConnectResponse.PaymentToken=transactionSurchargeResponse.PaymentMethod.CardPaymentMethod.CardToken;
					payConnectResponse.CardType=transactionSurchargeResponse.PaymentMethod.CardPaymentMethod.Network.ToString();
					payConnectResponse.CardNumber=transactionSurchargeResponse.PaymentMethod.CardPaymentMethod.CardLast4Digits;
					payConnectResponse.TransType=PayConnectResponse.TransactionType.Sale;
					break;
				case ResponseType.Error:
					//Happens when user tries to process a void past the allowed time limit.
					if(response.ErrorResponse==null) {
						payConnectResponse.TransType=PayConnectResponse.TransactionType.Unknown;
						payConnectResponse.Description=response.httpStatusCode.ToString();
						//Just needs to be not 0 to represent an error occured.
						payConnectResponse.StatusCode="";
						break;
					}
					ErrorResponse errorResponse=response.ErrorResponse;
					payConnectResponse.TransType=PayConnectResponse.TransactionType.Unknown;
					payConnectResponse.Description=errorResponse.Error.ToString();
					payConnectResponse.StatusCode="";
					break;
				case ResponseType.AddSignature:
					AddSignatureResponse addSignatureResponse=response.SignatureResponse;
					payConnectResponse.Description=addSignatureResponse.Status;
					payConnectResponse.StatusCode="0";//0 indicates success
					payConnectResponse.RefNumber=addSignatureResponse.ReferenceId;
					payConnectResponse.Amount=((decimal)addSignatureResponse.Amount)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					payConnectResponse.AuthCode=addSignatureResponse.AuthCode;
					payConnectResponse.MerchantId=addSignatureResponse.MerchantId.ToString();
					payConnectResponse.PaymentToken=addSignatureResponse.PaymentMethod.CardPaymentMethod.CardToken;
					payConnectResponse.CardType=addSignatureResponse.PaymentMethod.CardPaymentMethod.Network.ToString();
					payConnectResponse.CardNumber=addSignatureResponse.PaymentMethod.CardPaymentMethod.CardLast4Digits;
					payConnectResponse.TransType=PayConnectResponse.TransactionType.Unknown;

					break;
				case ResponseType.Void:
					VoidResponse voidResponse=response.VoidResponse;
					payConnectResponse.Description=voidResponse.Status;
					payConnectResponse.StatusCode="0";//0 indicates success
					payConnectResponse.RefNumber=voidResponse.ReferenceId;
					payConnectResponse.Amount=((decimal)voidResponse.AmountVoided)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					payConnectResponse.AuthCode=voidResponse.AuthCode;
					payConnectResponse.MerchantId=voidResponse.MerchantId.ToString();
					payConnectResponse.PaymentToken=voidResponse.PaymentMethod.CardPaymentMethod.CardToken;
					payConnectResponse.CardType=voidResponse.PaymentMethod.CardPaymentMethod.Network.ToString();
					payConnectResponse.CardNumber=voidResponse.PaymentMethod.CardPaymentMethod.CardLast4Digits;
					payConnectResponse.TransType=PayConnectResponse.TransactionType.Void;
					payConnectResponse.AmountSurcharged=voidResponse.AmountSurcharged;
					break;
				case ResponseType.Refund:
					RefundResponse refundResponse=response.RefundResponse;
					payConnectResponse.Description=refundResponse.Status;
					payConnectResponse.StatusCode="0";//0 indicates success
					payConnectResponse.RefNumber=refundResponse.ReferenceId;
					payConnectResponse.Amount=((decimal)refundResponse.AmountRefunded)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					payConnectResponse.AuthCode=refundResponse.AuthCode;
					payConnectResponse.MerchantId=refundResponse.MerchantId.ToString();
					if(refundResponse.PaymentMethod!=null) {
						payConnectResponse.PaymentToken=refundResponse.PaymentMethod.CardPaymentMethod.CardToken;
						payConnectResponse.CardType=refundResponse.PaymentMethod.CardPaymentMethod.Network.ToString();
						payConnectResponse.CardNumber=refundResponse.PaymentMethod.CardPaymentMethod.CardLast4Digits;
					}
					payConnectResponse.TransType=PayConnectResponse.TransactionType.Refund;
					break;
				case ResponseType.IFrame:
					iFrameResponse iFrameResponse=response.iFrameResponse;
					payConnectResponse.Description=iFrameResponse.Status;
					if(iFrameResponse.Status.ToLower()!="success") {
						payConnectResponse.StatusCode="";
						break;
					}
					payConnectResponse.PaymentToken=iFrameResponse.Response.CardToken;
					payConnectResponse.RefNumber=iFrameResponse.Response.ReferenceId;
					int year=int.Parse(iFrameResponse.Response.ExpiryYear);
					int month=int.Parse(iFrameResponse.Response.ExpiryMonth);
					payConnectResponse.TokenExpiration=new DateTime(year,month,1);
					payConnectResponse.Amount=((decimal)iFrameResponse.Response.Amount)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					if(iFrameResponse.Status.ToLower()=="success") {
						payConnectResponse.StatusCode="0";
					}
					payConnectResponse.SurchargePercent=iFrameResponse.Response.SurchargePercent;
					payConnectResponse.AmountSurcharged=((decimal)iFrameResponse.Response.AmountSurcharged)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					break;
				case ResponseType.TerminalTransaction:
					TerminalTransactionResponse terminalTransactionResponse=response.TerminalTransactionResponse;
					payConnectResponse.Description=terminalTransactionResponse.Status;
					payConnectResponse.StatusCode="0";//0 indicates success
					payConnectResponse.RefNumber=terminalTransactionResponse.ReferenceId;
					payConnectResponse.Amount=((decimal)terminalTransactionResponse.Amount)/100;//PayConnect sends amount as total cents, convert back to OD decimal amounts.
					payConnectResponse.AuthCode=terminalTransactionResponse.AuthCode;
					payConnectResponse.MerchantId=terminalTransactionResponse.MerchantId.ToString();
					payConnectResponse.PaymentToken=terminalTransactionResponse.PaymentMethod.CardPaymentMethod.CardToken;
					payConnectResponse.CardType=terminalTransactionResponse.PaymentMethod.CardPaymentMethod.Network.ToString();
					payConnectResponse.CardNumber=terminalTransactionResponse.PaymentMethod.CardPaymentMethod.CardLast4Digits;
					payConnectResponse.TransType=PayConnectResponse.TransactionType.Authorize;
					if(terminalTransactionResponse.TransType==TransactionType.Sale) {
						payConnectResponse.TransType=PayConnectResponse.TransactionType.Sale;
					}
					break;
				default:
					break;
			}
			return payConnectResponse;
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

		//Deprecated May 1st 2023 per DXC
		//[DataContract]
		//public class RefundRequest {
		//	///<summary>Identidy if transaction is Recurring or OneTime</summary>
		//	[DataMember(Name = "frequency")]
		//	public TransactionFrequency Frequency;
		//	///<summary>Type of transaction</summary>
		//	[DataMember(Name = "type")]
		//	public TransactionType TransType;
		//	///<summary>Amount in cents</summary>
		//	[DataMember(Name = "amount")]
		//	public int Amount;
		//	///<summary>Tokenized card number</summary>
		//	[DataMember(Name = "cardToken")]
		//	public string CardToken;
		//	///<summary>Expiry in MMYY or YYYYMM format</summary>
		//	[DataMember(Name = "expiry")]
		//	public string Expiry;
		//	///<summary>Security code for the card</summary>
		//	[DataMember(Name = "cvv",IsRequired=false)]
		//	public string Cvv;
		//	///<summary>Full name of the card holder</summary>
		//	[DataMember(Name = "cardHolder")]
		//	public string CardHolder;
		//	///<summary>Invoice number to identify the transaction</summary>
		//	[DataMember(Name = "invoiceNumber",IsRequired=false)]
		//	public string InvoiceNumber;
		//	///<summary>Patient for the payment.</summary>
		//	[DataMember(Name = "patient",IsRequired=false)]
		//	public PayConnectPatient Patient;
		//}

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
			public int? Amount;
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
			///<summary>The consumer of the iFrame. Set to "Patient" to display surcharge info in iFrame.</summary>
			[DataMember(Name = "clientType")]
			public string ClientType;
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

		[DataContract]
		public class TransactionStatusRequest {
			///<summary>Reference ID of the transaction</summary>
			[DataMember(Name = "referenceId",IsRequired=false)]
			public string ReferenceId;
			///<summary>ID of the transaction</summary>
			[DataMember(Name = "transactionId",IsRequired=false)]
			public string TransactionId;
		}

		[DataContract]
		public class CreateTerminalTransactionRequest {
			///<summary>Identifies if transaction is Recurring or OneTime</summary>
			[DataMember(Name = "frequency"),JsonConverter(typeof(StringEnumConverter))]
			public TransactionFrequency Frequency;
			///<summary>Type of transaction</summary>
			[DataMember(Name = "type"),JsonConverter(typeof(StringEnumConverter))]
			public TransactionType TransType;
			///<summary>Amount in cents</summary>
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Terminal ID for processing the transaction.</summary>
			[DataMember(Name = "terminal")]
			public string Terminal;
			///<summary>Full name of the card holder</summary>
			[DataMember(Name = "signature")]
			public bool Signature;
			///<summary>Invoice number to identify the transaction</summary>
			[DataMember(Name = "invoiceNumber",IsRequired=false)]
			public string InvoiceNumber;
			///<summary>Patient for the payment.</summary>
			[DataMember(Name = "patient",IsRequired=false)]
			public PayConnectPatient Patient;
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
			public iFrameResponse iFrameResponse;
			public TerminalTransactionResponse TerminalTransactionResponse;
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
			///<summary>Amount surcharged in cents</summary>
			[DataMember(Name = "amountSurcharged")]
			public int AmountSurcharged;
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
			[DataMember(Name = "amount")]
			public int Amount;
			///<summary>Amount captured in cents</summary>
			[DataMember(Name = "amountCaptured")]
			public int AmountCaptured;
			///<summary>Amount surcharged in cents</summary>
			[DataMember(Name = "amountSurcharged")]
			public int AmountSurcharged;
			///<summary>Percentage of the amount surcharged</summary>
			[DataMember(Name = "surchargePercent")]
			public int SurchargePercent;
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
		public class TerminalTransactionResponse {
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
			///<summary>Base64 encoded signature image. Can be null.</summary>
			[DataMember(Name = "signature")]
			public string Signature;
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
		public class PaymentMethod {
			public int paymentMethodId;
			[DataMember(Name = "type")]
			public PaymentMethodType MethodType;
			[DataMember(Name = "patientId")]
			public int? PatientId;
			[DataMember(Name = "createdAt")]
			public string CreatedAt;
			[DataMember(Name = "updatedAt")]
			public string UpdatedAt;
			[DataMember(Name = "cardPaymentMethod")]
			public CardPaymentMethod CardPaymentMethod;

			public enum PaymentMethodType {
				Card,
				ACH,
			}
		}

		[DataContract]
		public class CardPaymentMethod {
			[DataMember(Name = "cardPaymentMethodId")]
			public int CardPaymentMethodId;
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
			[DataMember(Name = "address1")]
			public string Address1;
			///<summary>Street line 2 in address</summary>
			[DataMember(Name = "address2")]
			public string Address2;
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
		/// <summary>any type of request body error will return a VALIDATION error - PayConnect Dev Team.</summary>
		[DataContract]
		public class ValidationError {
			[DataMember(Name = "properties")]
			public string ErrorProperties;
		}

		public class ValidationErrorBody {
			[DataMember(Name = "message")]
			public string Message;
			[DataMember(Name = "param")]
			public string Param;
			[DataMember(Name = "location")]
			public string Location;
			[DataMember(Name = "value")]
			public string Value;
		}

		[DataContract]
		public class iFrameResponse {
			[DataMember(Name="status")]
			public string Status;
			[DataMember(Name="response")]
			public iFrameSuccessResponse Response;
		}

		[DataContract]
		public class iFrameSuccessResponse {
			[DataMember(Name="cardToken")]
			public string CardToken;
			[DataMember(Name="cardHolder")]
			public string CardHolder;
			[DataMember(Name="expiryMonth")]
			public string ExpiryMonth;
			[DataMember(Name="expiryYear")]
			public string ExpiryYear;
			[DataMember(Name="zipCode")]
			public string ZipCode;
			[DataMember(Name="invoiceNumber")]
			public string InvoiceNumber;
			[DataMember(Name="patientFirstName")]
			public string PatientFirstName;
			[DataMember(Name="patientLastName")]
			public string PatientLastName;
			[DataMember(Name="amount")]
			public int Amount;
			[DataMember(Name="referenceId")]
			public string ReferenceId;
			///<summary>Amount surcharged in cents</summary>
			[DataMember(Name="amountSurcharged")]
			public int AmountSurcharged;
			///<summary>Percentage of the amount surcharged</summary>
			[DataMember(Name="surchargePercent")]
			public int SurchargePercent;
		}
		#endregion Response Objects

		[JsonConverter(typeof(StringEnumConverter))]
		public enum TransactionType {
			Sale,
			AuthorizeOnly,
			Refund,
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum TransactionFrequency {
			OneTime,
			Recurring,
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum TransactionSource {
			ThirdParty,
			Portal,
			Terminal,
			Integration,
			Text2Pay,
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum CardNetwork {
			Amex,
			Discover,
			Mastercard,
			[Description("Non-co-branded debit card")]
			NonCoBrandedDebitCard,
			Visa,
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum IframeType {
			Tokenizer,
			Payment,
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum ErrorType {
			[Description("RESPONSE")]
			Response,
			[Description("VALIDATION")]
			Validation,
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum ResponseType {
			[Description("Add Signature")]
			AddSignature,
			[Description("Transaction")]
			CreateTransaction,
			[Description("Transaction")]
			CreateSurchargeTransaction,
			[Description("Error")]
			Error,
			[Description("Refund")]
			Refund,
			[Description("Void")]
			Void,
			[Description("Embed Session")]
			EmbedSession,
			[Description("iFrame")]
			IFrame,
			[Description("Get Status")]
			GetStatus,
			TerminalTransaction,
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
