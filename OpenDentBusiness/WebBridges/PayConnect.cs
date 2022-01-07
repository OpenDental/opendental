using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CodeBase;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace OpenDentBusiness {
	public class PayConnect {

		private static PayConnectService.Credentials GetCredentials(Program prog,long clinicNum){
			PayConnectService.Credentials cred=new PayConnectService.Credentials();
			cred.Username=OpenDentBusiness.ProgramProperties.GetPropVal(prog.ProgramNum,"Username",clinicNum);
			cred.Password=CDT.Class1.TryDecrypt(OpenDentBusiness.ProgramProperties.GetPropVal(prog.ProgramNum,"Password",clinicNum));
			cred.Client="OpenDental2";
			if(ODBuild.IsDebug() || Introspection.IsTestingMode) {
				cred.ServiceID="DCI Web Service ID: 002778";//Testing
			}
			else {
				cred.ServiceID="DCI Web Service ID: 006328";//Production
			}
			cred.version="0310";
			return cred;
		}

		///<summary>Returns the URL within the PayConnectWebServiceURL IntrospectionEntity if introspection is being used regardless of the current solution configuration. Otherwise, returns the prelive URL if in debug mode or the live URL if not debugging.</summary>
		public static string GetMerchantServiceUrl() {
			//Default the URL to the live WSDL.
			string url="https://webservices.dentalxchange.com/merchant/MerchantService?wsdl";
			//Force debug instances to point to the prelive WSDL. Engineers can either change this specific variable OR utilize introspection to override.
			if(ODBuild.IsDebug()) {
				url="https://prelive.dentalxchange.com/merchant/MerchantService?wsdl";
			}
			//Return the url that was set above OR return the value within the PayConnectWebServiceURL IntrospectionEntity if introspection is being used.
			return Introspection.GetOverride(Introspection.IntrospectionEntity.PayConnectWebServiceURL,url);
		}

		///<summary>Parameters starting at authCode are optional, because our eServices probably reference this function as well.</summary>
		public static PayConnectService.creditCardRequest BuildSaleRequest(decimal amount,string cardNumber,int expYear,int expMonth,string nameOnCard,string securityCode,string zip,string magData,PayConnectService.transType transtype,string refNumber,bool tokenRequested,string authCode="",bool isForced=false) {
			PayConnectService.creditCardRequest request=new PayConnectService.creditCardRequest();
			request.Amount=amount;
			request.AmountSpecified=true;
			request.CardNumber=cardNumber;
			request.Expiration=new PayConnectService.expiration();
			request.Expiration.year=expYear;
			request.Expiration.month=expMonth;
			if(magData!=null) { //MagData is the data returned from magnetic card readers. Will only be present if a card was swiped.
				request.MagData=magData;
			}
			request.NameOnCard=nameOnCard;
			request.RefNumber=refNumber;
			request.SecurityCode=securityCode;
			request.TransType=transtype;
			request.Zip=zip;
			request.PaymentTokenRequestedSpecified=true;
			request.PaymentTokenRequested=tokenRequested;
			//request.AuthCode=authCode;//This field does not exist in the WSDL yet.  Dentalxchange will let us know once they finish adding it.
			request.ForceDuplicateSpecified=true;
			request.ForceDuplicate=isForced;
			return request;
		}

		///<summary>Shows a message box on error.</summary>
		public static PayConnectService.transResponse ProcessCreditCard(PayConnectService.creditCardRequest request,long clinicNum,
			Action<string> showError) 
		{
			try {
				Program prog=Programs.GetCur(ProgramName.PayConnect);
				PayConnectService.Credentials cred=GetCredentials(prog,clinicNum);
				PayConnectService.MerchantService ms=new PayConnectService.MerchantService();
				ms.Url=GetMerchantServiceUrl();
				PayConnectService.transResponse response=ms.processCreditCard(cred,request);
				ms.Dispose();
				if(response.Status.code!=0 && response.Status.description.ToLower().Contains("duplicate")) {
					showError(Lans.g("PayConnect","Payment failed")+". \r\n"+Lans.g("PayConnect","Error message from")+" Pay Connect: \""
						+response.Status.description+"\"\r\n"
						+Lans.g("PayConnect","Try using the Force Duplicate checkbox if a duplicate is intended."));
				}
				if(response.Status.code!=0 && response.Status.description.ToLower().Contains("invalid user")) {
					showError(Lans.g("PayConnect","Payment failed")+".\r\n"
						+Lans.g("PayConnect","PayConnect username and password combination invalid.")+"\r\n"
						+Lans.g("PayConnect","Verify account settings by going to")+"\r\n"
						+Lans.g("PayConnect","Setup | Program Links | PayConnect. The PayConnect username and password are probably the same as the DentalXChange login ID and password."));
				}
				if(response.Status.code==170) {
					showError(Lans.g("PayConnect","Invalid token. Generate new token using the 'Generate' button on the PayConnect Setup window."));
				}
				else if(response.Status.code!=0) {//Error
					showError(Lans.g("PayConnect","Payment failed")+". \r\n"+Lans.g("PayConnect","Error message from")+" Pay Connect: \""
						+response.Status.description+"\"");
				}
				return response;
			}
			catch(Exception ex) {
				showError(Lans.g("PayConnect","Payment failed")+". \r\n"+Lans.g("PayConnect","Error message")+": \""+ex.Message+"\"");
			}
			return null;
		}

		public static PayConnectService.signatureResponse ProcessSignature(PayConnectService.signatureRequest sigRequest,long clinicNum,
			Action<string> showError) 
		{
			try {
				Program prog=Programs.GetCur(ProgramName.PayConnect);
				PayConnectService.Credentials cred=GetCredentials(prog,clinicNum);
				PayConnectService.MerchantService ms=new PayConnectService.MerchantService();
				ms.Url=GetMerchantServiceUrl();
				PayConnectService.signatureResponse response=ms.processSignature(cred,sigRequest);
				ms.Dispose();
				if(response.Status.code!=0) {//Error
					showError(Lans.g("PayConnect","Signature capture failed")+". \r\n"+Lans.g("PayConnect","Error message from")+" Pay Connect: \""+response.Status.description+"\"");
				}
				return response;
			}
			catch(Exception ex) {
				showError(Lans.g("PayConnect","Signature capture failed")+". \r\n"+Lans.g("PayConnect","Error message from")+" Open Dental: \""+ex.Message+"\"");
			}
			return null;
		}

		public static bool IsValidCardAndExp(string cardNumber,int expYear,int expMonth,Action<string> showError) {
			bool isValid=false;
			try {
				PayConnectService.expiration pcExp=new PayConnectService.expiration();
				pcExp.year=expYear;
				pcExp.month=expMonth;
				PayConnectService.MerchantService ms=new PayConnectService.MerchantService();
				ms.Url=GetMerchantServiceUrl();
				isValid=(ms.isValidCard(cardNumber) && ms.isValidExpiration(pcExp));
				ms.Dispose();
			}
			catch(Exception ex) {
				showError(Lans.g("PayConnect","Credit Card validation failed")+". \r\n"+Lans.g("PayConnect","Error message from")
					+" Open Dental: \""+ex.Message+"\"");
			}
			return isValid;
		}

		///<summary>Return bool if value passed in is numeric only</summary>
		public static bool IsNumeric(string str) {
			if(str==null) {
				return false;
			}
			Regex objNotWholePattern=new Regex("[^0-9]");
			return !objNotWholePattern.IsMatch(str);
		}

		///<summary>Builds a receipt string for a web service transaction.</summary>
		public static string BuildReceiptString(PayConnectService.creditCardRequest request,PayConnectService.transResponse response,
			PayConnectService.signatureResponse sigResponse,long clinicNum) 
		{
			if(response==null) {
				return "";
			}
			bool doShowSignatureLine=DoShowSignatureLine(sigResponse);
			return BuildReceiptString(request.TransType,response.RefNumber,request.NameOnCard,request.CardNumber,request.MagData,response.AuthCode,
				response.Status.description,response.Messages==null ? null : response.Messages.ToList(),request.Amount,doShowSignatureLine,clinicNum);
		}

		public static string BuildReceiptString(PayConnectService.transType transType,string refNum,string nameOnCard,string cardNumber,
			string magData,string authCode,string statusDescription,List<string> messages,decimal amount,bool doShowSignatureLine,long clinicNum) 
		{
			string result="";
			cardNumber=cardNumber??""; //Prevents null reference exceptions when PayConnectPortal transactions don't have an associated card number
			int xmin=0;
			int xleft=xmin;
			int xright=15;
			int xmax=37;
			result+=Environment.NewLine;
			result+=CreditCardUtils.AddClinicToReceipt(clinicNum);
			//Print body
			result+="Date".PadRight(xright-xleft,'.')+DateTime.Now.ToString()+Environment.NewLine;
			result+=Environment.NewLine;
			result+="Trans Type".PadRight(xright-xleft,'.')+transType+Environment.NewLine;
			result+=Environment.NewLine;
			result+="Transaction #".PadRight(xright-xleft,'.')+refNum+Environment.NewLine;
			result+="Name".PadRight(xright-xleft,'.')+nameOnCard+Environment.NewLine;
			result+="Account".PadRight(xright-xleft,'.');
			for(int i = 0;i<cardNumber.Length-4;i++) {
				result+="*";
			}
			if(cardNumber.Length>=4) {
				result+=cardNumber.Substring(cardNumber.Length-4)+Environment.NewLine;//last 4 digits of card number only.
			}
			result+="Card Type".PadRight(xright-xleft,'.')+CreditCardUtils.GetCardType(cardNumber)+Environment.NewLine;
			result+="Entry".PadRight(xright-xleft,'.')+(String.IsNullOrEmpty(magData) ? "Manual" : "Swiped")+Environment.NewLine;
			result+="Auth Code".PadRight(xright-xleft,'.')+authCode+Environment.NewLine;
			result+="Result".PadRight(xright-xleft,'.')+statusDescription+Environment.NewLine;
			if(messages!=null) {
				string label="Message";
				foreach(string m in messages) {
					result+=label.PadRight(xright-xleft,'.')+m+Environment.NewLine;
					label="";
				}
			}
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
			if(ListTools.In(transType,PayConnectService.transType.RETURN,PayConnectService.transType.VOID)) {
				result+="Total Amt".PadRight(xright-xleft,'.')+(amount*-1)+Environment.NewLine;
			}
			else {
				result+="Total Amt".PadRight(xright-xleft,'.')+amount+Environment.NewLine;
			}
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
			result+="I agree to pay the above total amount according to my card issuer/bank agreement."+Environment.NewLine;
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine;
			if(doShowSignatureLine) {
				result+="Signature X".PadRight(xmax-xleft,'_');
			}
			else {
				result+="Electronically signed";
			}
			return result;
		}

		#region Patient Portal Interface
		///<summary>New OTK is available. Send wakeup event to EConnector so it will start processing immediately.</summary>
		public static EventHandler WakeupWebPaymentsMonitor;

		///<summary>Creates and returns the HPF URL and validation OTK which can be used to make a payment for an unspecified credit card.  Throws exceptions.</summary>
		public static string GetHpfUrlForPayment(Patient pat,string accountToken,string payNote,bool isMobile,double amount,bool saveToken,CreditCardSource ccSource) {
			if(pat==null) {
				throw new ODException("No Patient Found",ODException.ErrorCodes.NoPatientFound);
			}
			if(string.IsNullOrWhiteSpace(accountToken)) {
				throw new ODException("Invalid Account Token",ODException.ErrorCodes.OtkArgsInvalid);
			}
			if(amount<0.00 || amount>99999.99) {
				throw new ODException("Invalid Amount",ODException.ErrorCodes.OtkArgsInvalid);
			}
			if(string.IsNullOrEmpty(payNote)) {
				throw new ODException("Invalid PayNote",ODException.ErrorCodes.OtkArgsInvalid);
			}
			PayConnectResponseWeb responseWeb=new PayConnectResponseWeb() {
				Amount=amount,
				AccountToken=accountToken,
				PatNum=pat.PatNum,
				ProcessingStatus=PayConnectWebStatus.Created,
				PayNote=payNote,
				CCSource=ccSource,
				IsTokenSaved=saveToken,
			};
			PayConnectResponseWebs.Insert(responseWeb);
			try {
				string url;
				PayConnectREST.PostPaymentRequest(responseWeb,out url);
				PayConnectResponseWebs.Update(responseWeb);
				WakeupWebPaymentsMonitor?.Invoke(url,new EventArgs());
				return url;
			}
			catch(Exception e) {
				PayConnectResponseWebs.HandleResponseError(responseWeb,"Error calling PostPaymentRequest: "+e.Message);
				PayConnectResponseWebs.Update(responseWeb);
				throw;
			}
		}

		///<summary>Make a payment using HPF directly.  Throws exceptions.</summary>
		public static void MakePaymentWithAlias(Patient pat,string payNote,double amount,CreditCard cc) {
			if(pat==null) {
				throw new ODException("No Patient Found",ODException.ErrorCodes.NoPatientFound);
			}
			if(amount<0.00 || amount>99999.99) {
				throw new ODException("Invalid Amount",ODException.ErrorCodes.OtkArgsInvalid);
			}
			if(string.IsNullOrEmpty(payNote)) {
				throw new ODException("Invalid PayNote",ODException.ErrorCodes.OtkArgsInvalid);
			}
			if(cc==null) {
				throw new ODException("No Credit Card Found",ODException.ErrorCodes.OtkArgsInvalid);
			}
			if(string.IsNullOrEmpty(cc.PayConnectToken)) {
				throw new ODException("Invalid CC Alias",ODException.ErrorCodes.OtkArgsInvalid);
			}
			//request a PayConnect token, if a token was already saved PayConnect will return the same token,
			//otherwise replace CCNumberMasked with the returned token if the sale successful
			PayConnectService.creditCardRequest payConnectRequest=PayConnect.BuildSaleRequest(
				(decimal)amount,cc.PayConnectToken,cc.CCExpiration.Year,cc.CCExpiration.Month,
				pat.GetNameFLnoPref(),"",cc.Zip,null,
				PayConnectService.transType.SALE,"",true);
			//clinicNumCur could be 0, and the practice level or 'Headquarters' PayConnect credentials would be used for this charge
			PayConnectService.transResponse payConnectResponse=PayConnect.ProcessCreditCard(payConnectRequest,pat.ClinicNum,
				x => throw new ODException(x));
			if(payConnectRequest!=null && payConnectResponse.Status.code==0) {//Success
				string receipt=BuildReceiptString(payConnectRequest.TransType,payConnectResponse.RefNumber,payConnectRequest.NameOnCard,payConnectRequest.CardNumber,
					payConnectRequest.MagData,payConnectResponse.AuthCode,payConnectResponse.Status.description,payConnectResponse.Messages.ToList(),payConnectRequest.Amount,
					false,pat.ClinicNum);
				DateTime dateTimeProcessed=DateTime.Now;
				string formattedNote=Lans.g("PayConnect","Amount:")+" "+amount.ToString("f")+"\r\n"
				+Lans.g("PayConnect","Card Number:")+" "+cc.CCNumberMasked+"\r\n"
				+Lans.g("PayConnect","Transaction ID:")+" "+payConnectRequest.RefNumber+"\r\n"
				+Lans.g("PayConnect","Processed:")+" "+dateTimeProcessed.ToShortDateString()+" "+dateTimeProcessed.ToShortTimeString()+"\r\n"
				+Lans.g("PayConnect","Note:")+" "+payNote;
				long payNum=Payments.InsertFromPayConnect(pat.PatNum,pat.PriProv,pat.ClinicNum,amount,formattedNote,receipt,CreditCardSource.PayConnectPortal);
				PayConnectResponseWeb responseWeb=new PayConnectResponseWeb() {
					Amount=amount,
					PatNum=pat.PatNum,
					ProcessingStatus=PayConnectWebStatus.Completed,
					PayNote=payNote,
					CCSource=cc.CCSource,
					IsTokenSaved=true,
					PayNum=payNum,
					DateTimeCompleted=MiscData.GetNowDateTime(),
					RefNumber=payConnectResponse.RefNumber,
					TransType=PayConnectService.transType.SALE,
					PaymentToken=cc.PayConnectToken,
			};
				//Insert a new payconnectresponse row for historical record in the transaction window.
				PayConnectResponseWebs.Insert(responseWeb);
			}
			else {//Failed
				throw new ODException("Unable to process payment for this credit card: "+(payConnectResponse==null ? "Unknown error" : payConnectResponse.Status.description));
			}
		}

		private static bool DoShowSignatureLine(PayConnectService.signatureResponse sigResponse) {
			if(sigResponse==null || sigResponse.Status==null) {
				return true;//no signature was provided, show line for user to sign manually
			}
			return sigResponse.Status.code!=0;//0 is success, anything else would be a failure to process signature.
		}

		///<summary>Only deletes the credit card from Open Dental.  We currently do not delete PayConnect tokens from PayConnect anywhere like we do with XCharge/XWeb/PaySimple.
		///Throws exceptions.</summary>
		public static void DeleteCreditCard(Patient pat,CreditCard cc) {
			if(pat==null) {
				throw new ODException("No Patient Found",ODException.ErrorCodes.NoPatientFound);
			}
			if(cc==null) {
				throw new ODException("No Credit Card Found",ODException.ErrorCodes.OtkArgsInvalid);
			}
			if(string.IsNullOrEmpty(cc.PayConnectToken)) {
				throw new ODException("Invalid CC Alias",ODException.ErrorCodes.OtkArgsInvalid);
			}
			CreditCards.Delete(cc.CreditCardNum);
			List<CreditCard> creditCards=CreditCards.Refresh(pat.PatNum);
			for(int i=0;i<creditCards.Count;i++) {
				creditCards[i].ItemOrder=creditCards.Count-(i+1);
				CreditCards.Update(creditCards[i]);//Resets ItemOrder.
			}
		}

		///<summary>Putting this functionality in it's own class scope prevents it from being serialized for DTO.
		///Used by EConnector to monitor XWeb gateway.</summary>
		public class Monitor {
			#region Events
			public static EventHandler<Logger.LoggerEventArgs> LoggerEvent;
			///<summary>Any logging event. Sent as Verbose by default but can be sent as any log level.</summary>
			protected static void OnLoggerEvent(string s,LogLevel logLevel = LogLevel.Verbose) {
				if(LoggerEvent!=null) {
					LoggerEvent?.Invoke(null,new Logger.LoggerEventArgs(s,logLevel));
				}
			}
			///<summary>Logging event for a specific PayConnect. Uses json serializer so use sparingly if speed is an issue. 
			///Sent as Verbose by default but can be sent as any log level.</summary>
			protected static void OnLoggerEventForSingleResponse(XWebResponse response,string s,LogLevel logLevel = LogLevel.Verbose) {
				OnLoggerEvent(s+"\r\n\t"+JsonConvert.SerializeObject(response),logLevel);
			}
			#endregion

			///<summary>Look for outstanding OTKs and poll PayConnect for status. Update status and convert to paymentweb where necessary. 
			///Next run interval will be set to very short if there are still outstanding OTKs or long if not.
			///This thread will be awakened instantly if a new OTK becomes available (via PayConnect.WakeupWebPaymentsMonitor event).</summary>
			public static void OnThreadRun(ODThread odThread) {
				//Set next run interval to long by default;
				odThread.TimeIntervalMS=(int)TimeSpan.FromMinutes(1).TotalMilliseconds;
				try {
					//Returns number of OTKs still left in pending state.
					if(ProcessOutstandingTransactions()>0) { //We still have pending OTKs, set next run interval to short.
						odThread.TimeIntervalMS=(int)TimeSpan.FromSeconds(.5).TotalMilliseconds;
					}
				}
				catch(Exception e) {
					OnLoggerEvent(e.Message,LogLevel.Error);
					//Something unforeseen went wrong, throttle the next run interval a bit.
					odThread.TimeIntervalMS=(int)TimeSpan.FromSeconds(10).TotalMilliseconds;
				}
			}

			///<summary>Returns number of pending transactions remaining after completion.</summary>
			private static int ProcessOutstandingTransactions() {
				OnLoggerEvent("Checking for outstanding PaymentTokens.");
				List<PayConnectResponseWeb> listPendingPaymentsAll=PayConnectResponseWebs.GetAllPending();
				//Only process if it's been >= 5 seconds.
				List<PayConnectResponseWeb> listPendingDue=listPendingPaymentsAll.FindAll(x => DateTime.Now.Subtract(x.GetLastPendingUpdateDateTime())>TimeSpan.FromSeconds(5));
				OnLoggerEvent("Found "+listPendingPaymentsAll.Count+" PaymentTokens. "+listPendingDue.Count.ToString()+" are due to be processed.");
				if(listPendingDue.Count<=0) { //None are due this time around but we may have some still pending, return count of those.
					return listPendingPaymentsAll.Count;
				}
				OnLoggerEvent("Processing "+listPendingDue.Count.ToString()+" outstanding PaymentTokens.",LogLevel.Information);
				//Seed total remaining with any that we won't be processing this time around.
				int remaining=listPendingPaymentsAll.Count-listPendingDue.Count;
				foreach(PayConnectResponseWeb responseWebCur in listPendingDue) {
					try {
						//This method will update the responseWebCur with all of the data from the /paymentStatus API call
						PayConnectREST.GetPaymentStatus(responseWebCur);
						switch(responseWebCur.ProcessingStatus) {
							case PayConnectWebStatus.Pending:
								//No new status to report. Try again next time.
								OnLoggerEvent("PaymentToken still pending: "+responseWebCur.PayToken,LogLevel.Information);
								if(DateTime.Now.AddMinutes(30)<responseWebCur.GetLastPendingUpdateDateTime()) {
									//Expire this transaction ourselves after 30 minutes.  In testing, PayConnect expires them after 15 minutes.
									responseWebCur.ProcessingStatus=PayConnectWebStatus.Expired;
									responseWebCur.DateTimeExpired=MiscData.GetNowDateTime();
									OnLoggerEvent("PaymentToken has expired: "+responseWebCur.PayToken,LogLevel.Information);
								}
								break;
							case PayConnectWebStatus.CreatedError:
							case PayConnectWebStatus.PendingError:
								OnLoggerEvent("PaymentToken returned an error when retreiving a status update: "+responseWebCur.PayToken,LogLevel.Information);
								break;
							case PayConnectWebStatus.Expired:
								OnLoggerEvent("PaymentToken has expired: "+responseWebCur.PayToken,LogLevel.Information);
								break;
							case PayConnectWebStatus.Completed:
								OnLoggerEvent("PaymentToken has been completed: "+responseWebCur.PayToken,LogLevel.Information);
								if(responseWebCur.IsTokenSaved) {
									CreditCards.InsertFromPayConnect(responseWebCur);
								}
								Patient pat=Patients.GetPat(responseWebCur.PatNum);
								long clinicNum=0;
								if(PrefC.HasClinicsEnabled) {
									clinicNum=pat.ClinicNum;
								}
								string receipt="";
								if(!string.IsNullOrWhiteSpace(responseWebCur.LastResponseStr)) {
									var pcInfo=Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(responseWebCur.LastResponseStr,
										new { Amount=(decimal)0.0,CreditCardNumber="",Messages=new { Message=new string[0] }, RefNumber="",Status=new { description="" },}
									);
									receipt=BuildReceiptString(PayConnectService.transType.SALE,pcInfo.RefNumber,"",pcInfo.CreditCardNumber,"","",pcInfo.Status?.description??"",pcInfo.Messages.Message.ToList(),pcInfo.Amount,false,clinicNum);
								}
								responseWebCur.PayNum=Payments.InsertFromPayConnect(pat.PatNum,pat.PriProv,clinicNum,responseWebCur.Amount,responseWebCur.GetFormattedNote(true),receipt,responseWebCur.CCSource);
								break;
							case PayConnectWebStatus.Cancelled:
								OnLoggerEvent("PaymentToken has been cancelled: "+responseWebCur.PayToken,LogLevel.Information);
								break;
							case PayConnectWebStatus.Declined:
								OnLoggerEvent("PaymentToken has been declined: "+responseWebCur.PayToken,LogLevel.Information);
								break;
							case PayConnectWebStatus.Created:
							case PayConnectWebStatus.Unknown:
							case PayConnectWebStatus.UnknownError:
							default:
								OnLoggerEvent($"PaymentToken {responseWebCur.PayToken} returned unsupported state: {responseWebCur.ProcessingStatus.ToString()}",LogLevel.Information);
								break;
						}
					}
					catch(Exception e) {
						e.DoNothing();
					}
					finally {
						PayConnectResponseWebs.Update(responseWebCur);
					}
				}
				remaining=listPendingPaymentsAll.FindAll(x => x.ProcessingStatus==PayConnectWebStatus.Pending).Count;
				OnLoggerEvent(remaining.ToString()+" PaymentTokens still pending after processing.",LogLevel.Information);
				return remaining;
			}
		}
		#endregion

		public class WebPaymentProperties {
			public bool IsPaymentsAllowed;
			///<summary>The value of the "Patient Portal Payments Token" program property.
			///This is the account token related to the username/password used for PayConnect.</summary>
			public string Token;
		}
		
		public static class ProgramProperties {
			public const string PayConnectForceRecurringCharge="PayConnectForceRecurringCharge";
			public const string DefaultProcessingMethod="DefaultProcessingMethod";
			public const string PayConnectPreventSavingNewCC="PayConnectPreventSavingNewCC";
			public const string PatientPortalPaymentsEnabled="IsOnlinePaymentsEnabled";
			public const string PatientPortalPaymentsToken="Patient Portal Payments Token";
		}
	}

	public class PayConnectREST {

		///<summary>Returns the account token for PayConnect to use their rest service.
		///Throws exceptions if not successful at retrieving an account token.</summary>
		public static string GetAccountToken(string username,string password) {
			#region Response Object
			var resObj=new {
				AccountToken="",
				Status=new {
					code=-1,
					description="",
				},
				Messages=new {
					Message=new string[0]
				}
			};
			#endregion
			//var res=GetAccountTokenResponseMock(resObj);
			List<string> listHeaders=GetClientRequestHeaders();
			listHeaders.Add("Username: "+username);
			listHeaders.Add("Password: "+password);
			var res=Request(ApiRoute.AccountToken,HttpMethod.Get,listHeaders,"",resObj);
			if(res==null) {
				throw new ODException("Invalid response from PayConnect.");
			}
			int code=-1;
			string codeMsg="";
			if(res.Status!=null) {
				code=res.Status.code;
				codeMsg="Response code: "+res.Status.code+"\r\n";
			}			
			if(code==1000) {
				throw new ODException("Request to PayConnect resulted in an internal server error.\r\n"
					+"This may be due to invalid credentials.  Please check your username and password.");
			}
			if(code>0) {
				string err="Invalid response from PayConnect.\r\nResponse code: "+code;
				if(res.Messages!=null && res.Messages.Message!=null && res.Messages.Message.Length>0) {
					err+="\r\nError retrieving account token.\r\nResponse message(s):\r\n"+string.Join("\r\n",res.Messages.Message);
				}
				throw new ODException(err);
			}
			if(string.IsNullOrWhiteSpace(res.AccountToken)) {
				throw new ODException("Invalid account token was retrieved from PayConnect."+(string.IsNullOrEmpty(codeMsg) ? "" : "\r\n"+codeMsg));
			}
			return res.AccountToken;
		}

		///<summary>Outputs the URL for making a payment through PayConnect.
		///Sets the responseWeb's payment token to be updated.
		///Throws exceptions if the request/response is invalid.</summary>
		public static void PostPaymentRequest(PayConnectResponseWeb responseWeb,out string url) {
			#region Response Object
			var resObj=new {
				PayToken="",
				Url="",
				Status=new {
					code=-1,
					description="",
				},
				Messages=new {
					Message=new string[0]
				}
			};
			#endregion
			//var res=PostPaymentRequestResponseMock(resObj);
			List<string> listHeaders=GetClientRequestHeadersForWebURL();
			listHeaders.Add("AccountToken: "+responseWeb.AccountToken);
			string postBody=JsonConvert.SerializeObject(new {
				Amount=responseWeb.Amount,
				InvoiceNumber=responseWeb.PayConnectResponseWebNum,
			});
			var res=Request(ApiRoute.PaymentRequest,HttpMethod.Post,listHeaders,postBody,resObj);
			if(res==null) {
				throw new ODException("Invalid response from PayConnect.");
			}
			int code=-1;
			string codeMsg="";
			if(res.Status!=null) {
				code=res.Status.code;
				codeMsg="Response code: "+res.Status.code+"\r\n";
			}
			if(code>0) {
				string err="Invalid response from PayConnect.\r\nResponse code: "+code;
				if(res.Messages!=null && res.Messages.Message!=null && res.Messages.Message.Length>0) {
					err+="\r\nError retrieving payment request.\r\nResponse message(s):\r\n"+string.Join("\r\n",res.Messages.Message);
				}
				throw new ODException(err);
			}
			if(string.IsNullOrWhiteSpace(res.PayToken) || string.IsNullOrWhiteSpace(res.Url)) {
				throw new ODException("Invalid payment token or URL was retrieved from PayConnect."+(res.Status==null ? "" : "\r\n"+"Response code: "+res.Status.code));
			}
			url=res.Url;
			responseWeb.PayToken=res.PayToken;
			HandleResponseSuccess(responseWeb,JsonConvert.SerializeObject(res),true);
		}

		///<summary>Poll the existing PayConnectResponseWeb for status changes.
		///This method will update the ResponseJSON/ProcessingStatus with any changes</summary>
		public static void GetPaymentStatus(PayConnectResponseWeb responseWeb) {
			#region Response Object
			var resObj=new {
				Amount=-1.00,
				TransactionType="",
				TransactionStatus="",
				TransactionDate=DateTime.MinValue,
				StatusDescription="",
				//Used to poll for getting the payment status to see if the user has made a payment.
				PayToken="",
				CreditCardNumber="",
				CreditCardExpireDate="",
				TransactionID=-1,
				RefNumber="",
				Pending=true,
				Status=new {
					code=-1,
					description="",
				},
				Messages=new {
					Message=new string[0]
				},
				//Used for future payments with this card.  Do not confuse this with PayToken.
				PaymentToken=new {
					TokenId="",
					Expiration=new {
						month="",
						year="",
					},
					Messages=new {
						Message=new string[0]
					},
				},
			};
			#endregion
			List<string> listHeaders=GetClientRequestHeadersForWebURL();
			listHeaders.Add("AccountToken: "+responseWeb.AccountToken);
			try {
				var res=Request(ApiRoute.PaymentStatus,HttpMethod.Get,listHeaders,"",resObj,$"?payToken={responseWeb.PayToken}");
				if(res==null) {
					PayConnectResponseWebs.HandleResponseError(responseWeb,JsonConvert.SerializeObject(res));
					throw new ODException("Invalid response from PayConnect.");
				}
				int code=-1;
				string codeMsg="";
				if(res.Status!=null) {
					code=res.Status.code;
					codeMsg="Response code: "+res.Status.code+"\r\n";
				}
				if(code>0) {
					string err="Invalid response from PayConnect.\r\nResponse code: "+code;
					if(res.Messages!=null && res.Messages.Message!=null && res.Messages.Message.Length>0) {
						err+="\r\nError retrieving payment status.\r\nResponse message(s):\r\n"+string.Join("\r\n",res.Messages.Message);
					}
					PayConnectResponseWebs.HandleResponseError(responseWeb,JsonConvert.SerializeObject(res));
					throw new ODException(err);
				}
				if(res.Pending) {
					HandleResponseSuccess(responseWeb,JsonConvert.SerializeObject(res),true);
				}
				else if(res.TransactionStatus!=null 
					&& (res.TransactionStatus.Contains("Timeout") || res.TransactionStatus.Contains("Cancelled") || res.TransactionStatus.Contains("Declined"))) 
				{
					responseWeb.LastResponseStr=JsonConvert.SerializeObject(res);
					responseWeb.ProcessingStatus=res.TransactionStatus.Contains("Declined") ? PayConnectWebStatus.Declined
						: (res.TransactionStatus.Contains("Cancelled") ? PayConnectWebStatus.Cancelled : PayConnectWebStatus.Expired);
					responseWeb.DateTimeExpired=MiscData.GetNowDateTime();
				}
				else if(res.TransactionStatus!=null && res.TransactionStatus.Contains("Approved")) {
					string expYear=res.PaymentToken.Expiration.year.Substring(res.PaymentToken.Expiration.year.Length-2);//Last 2 digits only
					string expMonth=res.PaymentToken.Expiration.month.PadLeft(2,'0');//2 digit month with leading 0 if needed
					responseWeb.PaymentToken=res.PaymentToken.TokenId;
					responseWeb.ExpDateToken=expYear+expMonth;//yyMM format
					responseWeb.RefNumber=res.RefNumber;
					responseWeb.TransType=PayConnectService.transType.SALE;
					HandleResponseSuccess(responseWeb,JsonConvert.SerializeObject(res),res.Pending);
				}
				else {
					responseWeb.LastResponseStr=JsonConvert.SerializeObject(res);
					responseWeb.ProcessingStatus=PayConnectWebStatus.Unknown;
					responseWeb.DateTimeLastError=MiscData.GetNowDateTime();
				}
			}
			catch(Exception ex) {
				PayConnectResponseWebs.HandleResponseError(responseWeb,ex.Message);
				throw;
			}
		}

		///<summary>Sets the necessary fields for the given responseWeb object.
		///expDate should be in yyMM format.</summary>
		private static void HandleResponseSuccess(PayConnectResponseWeb responseWeb,string resStr,bool isPending) {
			if(isPending) {
				responseWeb.ProcessingStatus=PayConnectWebStatus.Pending;
				responseWeb.DateTimePending=MiscData.GetNowDateTime();
			}
			else {
				responseWeb.ProcessingStatus=PayConnectWebStatus.Completed;
				responseWeb.DateTimeCompleted=MiscData.GetNowDateTime();
			}
			responseWeb.LastResponseStr=resStr;
		}

		///<summary>These headers use the credentials PayConnect needs to get an accountToken from Open Dental as a merchant.</summary>
		private static List<string> GetClientRequestHeaders() {
			return new List<string>() {
				"Client: OpenDental2",
				$"ServiceID: DCI Web Service ID: {(ODBuild.IsDebug() || Introspection.IsTestingMode ? "002778" : "006328")}",
				"Version: 0310",
			};
		}

		///<summary>These headers use the credentials PayConnect gave us specifically for the URL endpoints (/paymentRequest and /paymentStatus, NOT accountToken)</summary>
		private static List<string> GetClientRequestHeadersForWebURL() {
			return new List<string>() {
				"Client: OpenDentalPortalMS",
				$"ServiceID: {(ODBuild.IsDebug() || Introspection.IsTestingMode ? "72yHWxY8:m15TJn!6yTw" : "z7fohsdUAs287mQh6516")}",
				"Version: 0310",
			};
		}

		private static T GetAccountTokenResponseMock<T>(T responseType) {
			string res=@"
			{
				""AccountToken"":""QIGhT2C1chpt"",
				""Status"": {
					""code"":0,
					""description"":""Operation Successful""
				},
				""Messages"": {
					""Message"":[]
				}
			}";
			return (T)JsonConvert.DeserializeAnonymousType(res,responseType);
		}

		private static T PostPaymentRequestResponseMock<T>(T responseType) {
			string res=@"
			{
				""PaymentToken"": ""5v1JgWCU5hnA"",
				""Url"": ""https://.../pay/payment?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwYXltZW50VG9r..."",
				""Status"": {
					""code"": 0,
					""description"": ""Operation Successful""
				},
				""Messages"": {
					""Message"":[]
				}
			}";
			return (T)JsonConvert.DeserializeAnonymousType(res,responseType);
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		private static T Request<T>(ApiRoute route,HttpMethod method,List<string> listHeaders,string body,T responseType,string queryStr="") {
			using(WebClient client=new WebClient()) {
				client.Headers[HttpRequestHeader.ContentType]="application/json";
				listHeaders.ForEach(x => client.Headers.Add(x));
				client.Encoding=UnicodeEncoding.UTF8;
				try {
					string res="";
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
					if(ODBuild.IsDebug()) {
						if((typeof(T)==typeof(string))) {//If user wants the entire json response as a string
							return (T)Convert.ChangeType(res,typeof(T));
						}
					}
					return JsonConvert.DeserializeAnonymousType(res,responseType);
				}
				catch(WebException wex) {
					string res="";
					if(!(wex.Response is HttpWebResponse)) {
						throw new Exception("Could not connect to the PayConnect server:\r\n"+wex.Message,wex);
					}
					using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						res=sr.ReadToEnd();
					}
					if(string.IsNullOrWhiteSpace(res)) {
						//The response didn't contain a body.  Through my limited testing, it only happens for 401 (Unauthorized) requests.
						if(wex.Response.GetType()==typeof(HttpWebResponse)) {
							HttpStatusCode statusCode=((HttpWebResponse)wex.Response).StatusCode;
							if(statusCode==HttpStatusCode.Unauthorized) {
								throw new ODException(Lans.g("PayConnect","Invalid PayConnect credentials."));
							}
						}
					}
					string errorMsg=wex.Message+(string.IsNullOrWhiteSpace(res) ? "" : "\r\nRaw response:\r\n"+res);
					throw new Exception(errorMsg,wex);//If we got this far and haven't rethrown, simply throw the entire exception.
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

		///<summary>Returns the full URL according to the route/route id given.</summary>
		private static string GetApiUrl(ApiRoute route) {
			string apiUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.PayConnectRestURL,"https://payconnect.dentalxchange.com/pay/rest/PayService");
			if(ODBuild.IsDebug()) {
				apiUrl="https://prelive2.dentalxchange.com/pay/rest/PayService";
			}
			switch(route) {
				case ApiRoute.Root:
					//Do nothing.  This is to allow someone to quickly grab the URL without having to make a copy+paste reference.
					break;
				case ApiRoute.AccountToken:
					apiUrl+="/accountToken";
					break;
				case ApiRoute.PaymentRequest:
					apiUrl+="/paymentRequest";
					break;
				case ApiRoute.PaymentStatus:
					apiUrl+="/paymentStatus";
					break;
				default:
					break;
			}
			return apiUrl;
		}

		private enum ApiRoute {
			Root,
			AccountToken,
			PaymentRequest,
			PaymentStatus,
		}
	}

	///<summary>Response class that can hold information for a web service response or a terminal response.</summary>
	public class PayConnectResponse {
		public string Description;
		public string StatusCode;
		public string AuthCode;
		public string RefNumber;
		public string PaymentToken;
		public DateTime TokenExpiration;
		public string CardType;

		public PayConnectResponse() {
		}
		
		///<summary>For web services.</summary>
		public PayConnectResponse(PayConnectService.transResponse response,PayConnectService.creditCardRequest request) {
			if(response != null) {
				AuthCode=response.AuthCode;
				RefNumber=response.RefNumber;
				if(response.Status != null) {
					Description=response.Status.description;
					StatusCode=response.Status.code.ToString();
				}
				if(response.PaymentToken != null) {
					PaymentToken=response.PaymentToken.TokenId;
					if(response.PaymentToken.Expiration != null) {
						TokenExpiration=new DateTime(response.PaymentToken.Expiration.year,response.PaymentToken.Expiration.month,1);
					}
				}
				CardType=CreditCardUtils.GetCardType(request.CardNumber);
			}
		}		
	}
	
	///<summary>The method of completing PayConnect credit card transactions.</summary>
	public enum PayConnectProcessingMethod {
		///<summary>Performs transactions by making a web call to PayConnect's service.</summary>
		[Description("Web Service")]
		WebService,
		///<summary>Performs the transactions on a credit card terminal hooked up to the computer.</summary>
		[Description("Terminal")]
		Terminal,
	}

}
