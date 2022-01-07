using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness.WebTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebServiceSerializer;

namespace OpenDentBusiness {
	public class PaySimple:WebBase {

		public class PropertyDescs {
			public const string PaySimpleApiUserName="PaySimple API User Name";
			public const string PaySimpleApiKey="PaySimple API Key";
			public const string PaySimplePayTypeCC="PaySimple Payment Type CC";
			public const string PaySimplePayTypeACH="PaySimple Payment Type ACH";
			public const string PaySimplePreventSavingNewCC="PaySimplePreventSavingNewCC";
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and formatted.</summary>
		public static long AddCustomer(string fname,string lname,string idInDb="",long clinicNum=-1) {
			ValidateProgram(clinicNum);
			return PaySimpleApi.PostCustomer(GetAuthHeader(clinicNum),PaySimpleApi.MakeNewCustomerData(fname,lname,idInDb));
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and and formatted.</summary>
		public static ApiResponse AddCreditCard(long customerId,string ccNum,DateTime ccExpDate,string billingZipCode="",long clinicNum=-1) {
			ValidateProgram(clinicNum);
			if(customerId==0) {
				throw new ODException(Lans.g("PaySimple","Invalid PaySimple Customer ID provided: ")+customerId.ToString());
			}
			return PaySimpleApi.PostAccountCreditCard(GetAuthHeader(clinicNum),PaySimpleApi.MakeNewAccountCreditCardData(customerId,ccNum,ccExpDate,PaySimpleApi.GetCardType(ccNum),billingZipCode));
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and formatted.</summary>
		public static ApiResponse AddACHAccount(Patient pat,string routingNumber,string acctNumber,string bankName,bool isCheckings,
			long clinicNum)
		{
			ValidateProgram(clinicNum);
			if(pat==null || pat.PatNum==0) {
				throw new ODException(Lans.g("PaySimple","Error adding account. Patient required."));
			}
			long psCustomerId=GetCustomerIdForPat(pat.PatNum,pat.FName,pat.LName,clinicNum);
			try {
				return AddACHAccount(psCustomerId,routingNumber,acctNumber,bankName,isCheckings,clinicNum);
			}
			catch(PaySimpleException ex) {
				HandlePaySimpleException(ex,psCustomerId);
				throw;
			}
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and formatted.</summary>
		private static ApiResponse AddACHAccount(long customerId,string routingNumber,string acctNumber,string bankName,bool isCheckings,
			long clinicNum)
		{
			ValidateProgram(clinicNum);
			if(customerId==0) {
				throw new ODException(Lans.g("PaySimple","Invalid PaySimple Customer ID provided: ")+customerId.ToString());
			}
			return PaySimpleApi.PostAccountACH(GetAuthHeader(clinicNum),
				PaySimpleApi.MakeNewAccountACHData(customerId,routingNumber,acctNumber,bankName,isCheckings));
		}

		public static ApiResponse GetPayment(long clinicNum,long externalId=0) {
			ValidateProgram(clinicNum);
			return PaySimpleApi.GetPayment(GetAuthHeader(clinicNum),externalId);
		}

		public static List<ApiWebhookResponse> GetAchWebhooks(long clinicNum) {
			ValidateProgram(clinicNum);
			return PaySimpleApi.GetWebhooks(GetAuthHeader(clinicNum,true));
		}

		public static string PostWebhook(long clinicNum,string[] arrayHookTypes,string url) {
			ValidateProgram(clinicNum);
			return PaySimpleApi.PostWebhook(GetAuthHeader(clinicNum,true),PaySimpleApi.MakeNewWebhookData(url,arrayHookTypes));
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and and formatted.
		///If PatNum is 0, we will make a one time payment for an UNKNOWN patient.  This is currently only intended for prepaid insurance cards.
		///Returns the PaymentId given by PaySimple. Paramater lname can be set to carrier name sometimes.</summary>
		public static ApiResponse MakePayment(long patNum,CreditCard cc,decimal payAmt,string ccNum,DateTime ccExpDate,bool isOneTimePayment
			,string billingZipCode="",string cvv="",long clinicNum=-1,string lname="") 
		{
			ValidateProgram(clinicNum);
			if(patNum==0) {
				//MakePaymentNoPat will validate its credentials.
				return MakePaymentNoPat(payAmt,ccNum,ccExpDate,billingZipCode,cvv,clinicNum,lname);
			}
			if((cc==null || string.IsNullOrWhiteSpace(cc.PaySimpleToken)) && (string.IsNullOrWhiteSpace(ccNum) || ccExpDate.Year<DateTime.Today.Year)) {
				throw new ODException(Lans.g("PaySimple","Error making payment"));
			}
			if(cc==null) {
				cc=new CreditCard() {
					PatNum=patNum,
					PaySimpleToken="",
				};
			}
			if(string.IsNullOrWhiteSpace(cc.PaySimpleToken)) {
				Patient patCur=Patients.GetPat(cc.PatNum);
				if(patCur==null) {
					patCur=new Patient() {
						PatNum=patNum,
						FName="",
						LName="",
					};
				}
				long psCustomerId=GetCustomerIdForPat(patCur.PatNum,patCur.FName,patCur.LName,clinicNum);
				ApiResponse apiResponse;
				try {
					apiResponse=AddCreditCard(psCustomerId,ccNum,ccExpDate,billingZipCode,clinicNum);
				}
				catch(PaySimpleException ex) {
					HandlePaySimpleException(ex,psCustomerId);
					throw;
				}
				cc.PaySimpleToken=apiResponse.PaySimpleToken;
				//If the user doesn't want Open Dental to store their account id, we will let them continue entering their CC info.
				if(!isOneTimePayment && cc.CreditCardNum>0) {
					CreditCards.Update(cc);
				}
			}
			return PaySimpleApi.PostPayment(GetAuthHeader(clinicNum),PaySimpleApi.MakeNewPaymentData(PIn.Long(cc.PaySimpleToken),payAmt,cvv),
				CreditCardSource.PaySimple);
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and and formatted.</summary>
		public static ApiResponse MakePaymentByToken(Patient pat,CreditCard cc,decimal payAmt,long clinicNum=-1) {
			ValidateProgram(clinicNum);
			if(cc==null || string.IsNullOrWhiteSpace(cc.PaySimpleToken)) {
				throw new ODException(Lans.g("PaySimple","Error making payment by token"));
			}
			if(cc.CCSource==CreditCardSource.PaySimple) {
				return MakePayment(cc.PatNum,cc,payAmt,"",DateTime.MinValue,false,"","",clinicNum);
			}
			return MakePaymentACH(pat,cc,payAmt,"","","",false,false,clinicNum);
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and and formatted.</summary>
		public static ApiResponse VoidPayment(string paySimplePaymentId,long clinicNum=-1) {
			ValidateProgram(clinicNum);
			if(string.IsNullOrWhiteSpace(paySimplePaymentId)) {
				throw new Exception(Lans.g("PaySimple","Invalid PaySimple Payment ID to void."));
			}
			return PaySimpleApi.PutPaymentVoided(GetAuthHeader(clinicNum),paySimplePaymentId);
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and and formatted.</summary>
		public static ApiResponse ReversePayment(string paySimplePaymentId,long clinicNum=-1) {
			ValidateProgram(clinicNum);
			if(string.IsNullOrWhiteSpace(paySimplePaymentId)) {
				throw new Exception(Lans.g("PaySimple","Invalid PaySimple Payment ID to reverse."));
			}
			return PaySimpleApi.PutPaymentReversed(GetAuthHeader(clinicNum),paySimplePaymentId);
		}

		///<summary>Throws exceptions.  Will purposefully throw ODExceptions that are already translated and and formatted.
		///Returns the PaymentId given by PaySimple.</summary>
		private static ApiResponse MakePaymentNoPat(decimal payAmt,string ccNum,DateTime ccExpDate,string billingZipCode="",string cvv=""
			,long clinicNum=-1,string lname="") 
		{
			ValidateProgram(clinicNum);
			if(string.IsNullOrWhiteSpace(ccNum) || ccExpDate.Year<DateTime.Today.Year) {
				throw new ODException(Lans.g("PaySimple","Error making payment"));
			}
			long psCustomerId=AddCustomer("UNKNOWN",lname,"",clinicNum);
			ApiResponse apiResponse=AddCreditCard(psCustomerId,ccNum,ccExpDate,billingZipCode);
			string accountId=apiResponse.PaySimpleToken;
			return PaySimpleApi.PostPayment(GetAuthHeader(clinicNum),PaySimpleApi.MakeNewPaymentData(PIn.Long(accountId),payAmt,cvv),
				CreditCardSource.PaySimple);
		}

		///<summary>Will throw ODExceptions that are already translated and and formatted.
		///If pat is null, we will make a one time payment for an UNKNOWN patient.  This is currently only intended for prepaid insurance cards.
		///Returns the ApiResponse based on what is given us by PaySimple.</summary>
		public static ApiResponse MakePaymentACH(Patient pat,CreditCard cc,decimal payAmt,string routingNumber,string acctNumber,string bankName,
			bool isCheckings,bool isOneTimePayment,long clinicNum=-1)
		{
			ValidateProgram(clinicNum);
			if(pat==null || pat.PatNum==0) {
				throw new ODException(Lans.g("PaySimple","Error making payment. Patient required."));//No patient is allowed only for prepaid credit cards
			}
			if((cc==null || string.IsNullOrWhiteSpace(cc.PaySimpleToken)) 
				&& (string.IsNullOrWhiteSpace(routingNumber) || string.IsNullOrWhiteSpace(acctNumber) || string.IsNullOrWhiteSpace(bankName)))
			{
				throw new ODException(Lans.g("PaySimple","Error making payment."));
			}
			if(cc==null) {
				cc=new CreditCard() {
					PatNum=pat.PatNum,
					PaySimpleToken="",
				};
			}
			if(string.IsNullOrWhiteSpace(cc.PaySimpleToken)) {
				long psCustomerId=GetCustomerIdForPat(pat.PatNum,pat.FName,pat.LName,clinicNum);
				try {
					ApiResponse apiResponse=AddACHAccount(psCustomerId,routingNumber,acctNumber,bankName,isCheckings,clinicNum);
					cc.PaySimpleToken=apiResponse.PaySimpleToken;
				}
				catch(PaySimpleException ex) {
					HandlePaySimpleException(ex,psCustomerId);
				}
			}
			return PaySimpleApi.PostPayment(GetAuthHeader(clinicNum),PaySimpleApi.MakeNewPaymentACHData(PIn.Long(cc.PaySimpleToken),payAmt),
				CreditCardSource.PaySimpleACH);

		}

		///<summary>Deletes the credit card.</summary>
		public static ApiResponse DeleteCreditCard(CreditCard creditCardCur) {
			ValidateProgram(creditCardCur.ClinicNum);
			if(string.IsNullOrWhiteSpace(creditCardCur.PaySimpleToken)) {
				throw new Exception(Lans.g("PaySimple","Invalid PaySimple Credit Card ID to void."));
			}
			return PaySimpleApi.DeleteAccountCreditCard(GetAuthHeader(creditCardCur.ClinicNum),creditCardCur.PaySimpleToken);
		}

		///<summary>Deletes the ACH account.</summary>
		public static ApiResponse DeleteACHAccount(CreditCard creditCardCur) {
			ValidateProgram(creditCardCur.ClinicNum);
			if(string.IsNullOrWhiteSpace(creditCardCur.PaySimpleToken)) {
				throw new Exception(Lans.g("PaySimple","Invalid PaySimple ACN Account ID to void."));
			}
			return PaySimpleApi.DeleteAccountACH(GetAuthHeader(creditCardCur.ClinicNum),creditCardCur.PaySimpleToken);
		}
		
		///<summary>Returns the Authorization header for the api call, using the passed in clinicNum if provided, otherwise uses the currently selected clinic.</summary>
		private static string GetAuthHeader(long clinicNum=-1,bool isWebhook=false) {
			if(clinicNum==-1) {
				clinicNum=Clinics.ClinicNum;
			}
			string apiUserName=ProgramProperties.GetPropValForClinicOrDefault(Programs.GetCur(ProgramName.PaySimple).ProgramNum
				,PropertyDescs.PaySimpleApiUserName
				,clinicNum);
			string apiKey=ProgramProperties.GetPropValForClinicOrDefault(Programs.GetCur(ProgramName.PaySimple).ProgramNum
				,PropertyDescs.PaySimpleApiKey
				,clinicNum);
			if(ODBuild.IsDebug()) {
				//string apiUserName="APIUser155356";
				//string apiKey="QkQRj8i0QDPOtUBhbTWx7irBrqospeY8RDC4HxW2LD3IDIfo1bcumTMomp7IJbYONjIna84QPwMwfFLMTtZcMJ2Bm4meQIfojgsDrZr5HxAnQkylHJgF7t2XUDoVy6I0";
			}
			if(isWebhook) {
				return PaySimpleApi.GetWebhookAuthHeader(apiUserName,apiKey);
			}
			return PaySimpleApi.GetAuthHeader(apiUserName,apiKey);
		}

		///<summary>Throws exceptions if the PaySimple program or program properties are not valid.
		///If this method doesn't throw an exception, everything is assumed to be valid.</summary>
		private static void ValidateProgram(long clinicNum=-1) {
			if(clinicNum==-1) {
				clinicNum=Clinics.ClinicNum;
			}
			Program progPaySimple=Programs.GetCur(ProgramName.PaySimple);
			if(progPaySimple==null) {
				throw new ODException(Lans.g("PaySimple","PaySimple program does not exist in the database.  Please call support."));
			}
			if(!progPaySimple.Enabled) {
				throw new ODException(Lans.g("PaySimple","PaySimple is not enabled."));
			}
			string apiUserName=ProgramProperties.GetPropValForClinicOrDefault(progPaySimple.ProgramNum,PropertyDescs.PaySimpleApiUserName,clinicNum);
			string apiKey=ProgramProperties.GetPropValForClinicOrDefault(progPaySimple.ProgramNum,PropertyDescs.PaySimpleApiKey,clinicNum);
			string payType=ProgramProperties.GetPropValForClinicOrDefault(progPaySimple.ProgramNum,PropertyDescs.PaySimplePayTypeCC,clinicNum);
			if(string.IsNullOrWhiteSpace(apiUserName) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(payType)) {
				throw new ODException(Lans.g("PaySimple","PaySimple Username, Key, or PayType is empty."));
			}
		}
		
		///<summary>Returns the CustomerId that PaySimple gave us for the given patNum.
		///If addToPaySimpleIfMissing, the PaySimple API will add the given patNum if a link isn't in our database and return the new CustomerId.
		///Otherwise, it will return 0.</summary>
		public static long GetCustomerIdForPat(long patNum,string fname,string lname,long clinicNum=-1) {
			long psCustomerId=PatientLinks.GetPatNumsLinkedFrom(patNum,PatientLinkType.PaySimple).FirstOrDefault();
			if(psCustomerId==0) {//Patient doesn't have a PaySimpleCustomerId
				psCustomerId=AddCustomer(fname,lname,(patNum>0 ? "PatNum: "+patNum.ToString() : ""),clinicNum);
				PatientLinks.Insert(new PatientLink() {
					PatNumFrom=patNum,
					PatNumTo=psCustomerId,
					LinkType=PatientLinkType.PaySimple,
				});
			}
			return psCustomerId;
		}

		public static void HandlePaySimpleException(PaySimpleException exception,long customerId) {
			if(exception.ErrorCode=="InvalidInput" && exception.ErrorMessages.Count==1
				&& exception.ErrorMessages[0]==$"Customer id {customerId} does not exist") 
			{
				exception.ErrorType=PaySimpleError.CustomerDoesNotExist;
				exception.CustomerId=customerId;
			}
			throw exception;
		}

		///<summary>OD response object for PaySimple API method responses.</summary>
		public class ApiResponse {
			///<summary>The transaction that was just processed.</summary>
			public TransType TransType;
			///<summary>The status of the PaySimple response.</summary>
			public string Status;
			///<summary>The returned ProviderAuthCode from PaySimple.</summary>
			public string AuthCode;
			///<summary>The PaySimple Payment ID of the payment that was handled.</summary>
			public string RefNumber;
			///<summary>The PaySimple Account ID of the credit card used to make the payment.</summary>
			public string PaySimpleToken;
			//Commented out CardType.  There isn't a consistent way to get the card type without making extra API calls to PaySimple.
			/////<summary>The issuer of the credit card (E.g. VISA or MASTERCARD).</summary>
			//public string CardType;
			///<summary>Not given from the API.  This is filled after returning back to Open Dental 
			///and uses Open Dental specific things to generate this (ie. clinic info)</summary>
			public string TransactionReceipt;
			public decimal Amount;
			///<summary>Should be either PaySimple or PaySimpleACH.</summary>
			public CreditCardSource CCSource=CreditCardSource.PaySimple;
			/// <summary>Includes information about why a payment failed. </summary>
			public string FailureDescription;
			public bool IsDeclined;
			public string MerchantActionText;

			///<summary>Builds the receipt string for a web service transaction.
			///This method assumes ccExpYear is a 4 digit integer.</summary>
			public void BuildReceiptString(string ccNum,int ccExpMonth,int ccExpYear,string nameOnCard,long clinicNum,bool wasSwiped=false,
				bool isACH=false) 
			{
				string result="";
				int xleft=0;
				int xright=15;
				result+=Environment.NewLine;
				result+=CreditCardUtils.AddClinicToReceipt(clinicNum);
				//Print body
				result+="Date".PadRight(xright-xleft,'.')+DateTime.Now.ToString()+Environment.NewLine;
				result+=Environment.NewLine;
				result+="Trans Type".PadRight(xright-xleft,'.')+this.TransType.ToString()+Environment.NewLine;
				result+=Environment.NewLine;
				result+="Transaction #".PadRight(xright-xleft,'.')+this.RefNumber+Environment.NewLine;
				if(!string.IsNullOrWhiteSpace(nameOnCard)) {
					result+="Name".PadRight(xright-xleft,'.')+nameOnCard+Environment.NewLine;
				}
				result+="Account".PadRight(xright-xleft,'.');
				for(int i = 0;i<ccNum.Length-4;i++) {
					result+="*";
				}
				if(!string.IsNullOrWhiteSpace(ccNum)) {
					result+=ccNum.Substring(ccNum.Length-4)+Environment.NewLine;//last 4 digits of card number only.
				}
				if(ccExpMonth>=0 && ccExpYear>=0) {
					result+="Exp Date".PadRight(xright-xleft,'.')+ccExpMonth.ToString().PadLeft(2,'0')+(ccExpYear%100)+Environment.NewLine;
				}
				result+="Entry".PadRight(xright-xleft,'.')+(wasSwiped ? "Swiped" : "Manual")+Environment.NewLine;
				result+="Auth Code".PadRight(xright-xleft,'.')+this.AuthCode+Environment.NewLine;
				result+="Result".PadRight(xright-xleft,'.')+this.Status+Environment.NewLine;
				result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
				if(ListTools.In(this.TransType,PaySimple.TransType.RETURN,PaySimple.TransType.VOID)) {
					result+="Total Amt".PadRight(xright-xleft,'.')+(this.Amount*-1)+Environment.NewLine;
				}
				else {
					result+="Total Amt".PadRight(xright-xleft,'.')+this.Amount+Environment.NewLine;
				}
				if(this.TransType==TransType.SALE) {
					result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
					if(isACH) {
						result+="I agree to pay the above total amount according to my bank agreement.";
					}
					else {
						result+="I agree to pay the above total amount according to my card issuer/bank agreement.";
					}
				}
				this.TransactionReceipt=result;
			}

			///<summary>Returns the translated and note-formatted string that represents the result of the API call.</summary>
			public string ToNoteString(string clinicDesc="",string entry="",string curUserName="",string expDateStr="",string cardType="") {
				string retVal="";
				if(!string.IsNullOrWhiteSpace(clinicDesc)) {
					retVal+=Lans.g("PaySimple","Clinic")+": "+clinicDesc+Environment.NewLine;
				}
				retVal+=Lans.g("PaySimple","Transaction Type")+": "+Enum.GetName(typeof(TransType),this.TransType)+Environment.NewLine+
					Lans.g("PaySimple","Status")+": "+this.Status+Environment.NewLine+
					Lans.g("PaySimple","Auth Code")+": "+this.AuthCode+Environment.NewLine+
					Lans.g("PaySimple","Amount")+": "+this.Amount+Environment.NewLine+
					Lans.g("PaySimple","PaySimple Account ID")+": "+this.PaySimpleToken+Environment.NewLine+
					Lans.g("PaySimple","PaySimple Transaction Number")+": "+this.RefNumber+Environment.NewLine;
				if(!string.IsNullOrWhiteSpace(entry)) {
					retVal+=Lans.g("PaySimple","Entry")+": "+entry+Environment.NewLine;
				}
				if(!string.IsNullOrWhiteSpace(curUserName)) {
					retVal+=Lans.g("PaySimple","Clerk")+": "+curUserName+Environment.NewLine;
				}
				if(!string.IsNullOrWhiteSpace(expDateStr)) {
					retVal+=Lans.g("PaySimple","Expiration")+": "+expDateStr+Environment.NewLine;
				}
				if(!string.IsNullOrWhiteSpace(cardType)) {
					retVal+=Lans.g("PaySimple","Card Type")+": "+cardType+Environment.NewLine;
				}
				return retVal;
			}
		}

		public class ApiWebhookResponse {
			public string WebhookId;
			public string WebhookUrl;
			public List<string> WebhookTypes;
			public bool IsActive;
		}

		private class PaySimpleApi {

			#region SDK Calls
			///<summary>Throws exceptions for http codes of 300 or more from API call.</summary>
			public static string GetCheckoutToken(string authHeader) {
				var response=Request(ApiRoute.Token,HttpMethod.Post,authHeader,"{}",
					#region ResponseType Object
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] { new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							JwtToken="",
							Expiration="",
						}
					}
					#endregion
				);
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				return response.FullResponse.Response.JwtToken;
			}

			///<summary>Throws exceptions for http codes of 300 or more from API call.</summary>
			public static string GetCustomerToken(string authHeader,long paySimpleCustID) {
				var response=Request(ApiRoute.CustomerToken,HttpMethod.Get,authHeader,"",
					#region ResponseType Object
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] { new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							JwtToken="",
						}
					}
					#endregion
					,paySimpleCustID.ToString()
				);
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				return response.FullResponse.Response.JwtToken;
			}
			#endregion

			///<summary>Throws exceptions for http codes of 300 or more from API call.</summary>
			public static long PostCustomer(string authHeader,string postData) {
				var response=Request(ApiRoute.Customer,HttpMethod.Post,authHeader,postData,
					#region ResponseType Object
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] { new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							MiddleName="",
							AltEmail="",
							AltPhone="",
							MobilePhone="",
							Fax="",
							Website="",
							BillingAddress="",
							ShippingSameAsBilling=true,
							ShippingAddress="",
							Company="",
							Notes="",
							CustomerAccount="",
							FirstName="",
							LastName="",
							Email="",
							Phone="",
							Id=(long)0,
							LastModified="",
							CreatedOn="",
						}
					}
					#endregion
				);
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				return response.FullResponse.Response.Id;
			}

			///<summary>Throws exceptions for http codes of 300 or more from API call.</summary>
			public static ApiResponse PostAccountCreditCard(string authHeader,string postData) {
				var response=Request(ApiRoute.AccountCreditCard,HttpMethod.Post,authHeader,postData,
					#region ResponseType Object
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] { new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							CreditCardNumber="",
							ExpirationDate="",
							Issuer="",
							BillingZipCode="",
							CustomerId=(long)0,
							IsDefault=true,
							Id=395560,
							LastModified="",
							CreatedOn=""
						}
					}
					#endregion
				);
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				return new ApiResponse() {
					Status="",
					AuthCode="",
					PaySimpleToken=response.FullResponse.Response.Id.ToString(),
					RefNumber="",
					TransType=TransType.AUTH,
				};
			}

			///<summary>Throws exceptions for http codes of 300 or more from API call.</summary>
			public static ApiResponse PostPayment(string authHeader,string postData,CreditCardSource ccSource) {
				//var response=MockDeclinedPaymentResponse(
				var response=Request(ApiRoute.Payment,HttpMethod.Post,authHeader,postData,
					#region ResponseType Object
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] { new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							CustomerId=(long)0,
							CustomerFirstName="",
							CustomerLastName="",
							CustomerCompany="",
							ReferenceId=(long)0,
							Status="Authorized",
							RecurringScheduleId=(long)0,
							PaymentType="CC",
							PaymentSubType="MOTO",
							ProviderAuthCode="Approved",
							TraceNumber="",
							PaymentDate="",
							ReturnDate="",
							EstimatedSettleDate="",
							ActualSettledDate="",
							CanVoidUntil="",
							FailureData=new {
								Code="",
								Description="",
								MerchantActionText="",
								IsDecline=false
							},
							AccountId=(long)0,
							InvoiceId="",
							Amount=(decimal)0.0,
							IsDebit=false,
							InvoiceNumber="123AB",//Can have alpha characters
							PurchaseOrderNumber="",
							OrderId="",
							Description="",
							Latitude="",
							Longitude="",
							SuccessReceiptOptions="",
							FailureReceiptOptions="",
							Id=(long)0,
							LastModified="",
							CreatedOn="",
						}
					}
					#endregion
				);
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				if(!response.FullResponse.Response.ProviderAuthCode.ToLower().Contains("approved")) {
					string errorMsg;
					if(response.FullResponse.Response.FailureData!=null && response.FullResponse.Response.FailureData.IsDecline) {
						errorMsg="Payment was declined by PaySimple.\r\n"
							+"Message From PaySimple: "+response.FullResponse.Response.FailureData.Description+"\r\n"
							+"Merchant Action Text from PaySimple: "+response.FullResponse.Response.FailureData.MerchantActionText;
					}
					else {
						errorMsg="Payment was not approved by PaySimple.  Auth Code from PaySimple:\r\n"+response.FullResponse.Response.ProviderAuthCode;
					}
					throw new Exception(errorMsg);
				}
				return new ApiResponse() {
					Status=response.FullResponse.Response.Status,
					AuthCode=response.FullResponse.Response.ProviderAuthCode,
					PaySimpleToken=response.FullResponse.Response.AccountId.ToString(),
					RefNumber=response.FullResponse.Response.Id.ToString(),
					TransType=TransType.SALE,
					Amount=response.FullResponse.Response.Amount,
					CCSource=ccSource,
				};
			}

			public static ApiResponse GetPayment(string authHeader,long externalId) {
				#region Response
				var response =Request(ApiRoute.Payment,HttpMethod.Get,authHeader,"",
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] {new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							CustomerId=(long)0,
							CustomerFirstName="",
							CustomerLastName="",
							CustomerCompany="",
							ReferenceId=(long)0,
							Status="Authorized",
							RecurringScheduleId=(long)0,
							PaymentType="CC",
							PaymentSubType="MOTO",
							ProviderAuthCode="Approved",
							TraceNumber="",
							PaymentDate="",
							ReturnDate="",
							EstimatedSettleDate="",
							EstimatedDepositDate="",
							ActualSettledDate="",
							CanVoidUntil="",
							FailureData=new {
								Code="",
								Description="",
								MerchantActionText="",
								IsDecline=false
							},
							AccountId=(long)0,
							InvoiceId="",
							Amount=(decimal)0.0,
							IsDebit=false,
							InvoiceNumber="123AB",//Can have alpha characters
							PurchaseOrderNumber="",
							OrderId="",
							Description="",
							Latitude="",
							Longitude="",
							SuccessReceiptOptions="",
							FailureReceiptOptions="",
							Id=(long)0,
							LastModified="",
							CreatedOn="",
						}
					},externalId==0?"":externalId.ToString()
				);
				#endregion
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				ApiResponse paySimpleResponse=new ApiResponse() {
					Status=response.FullResponse.Response.Status,
					AuthCode=response.FullResponse.Response.ProviderAuthCode,
					PaySimpleToken=response.FullResponse.Response.AccountId.ToString(),
					RefNumber=response.FullResponse.Response.Id.ToString(),
					Amount=response.FullResponse.Response.Amount
				};
				if(response.FullResponse.Response.FailureData!=null) {
					paySimpleResponse.FailureDescription=response.FullResponse.Response.FailureData.Description;
					paySimpleResponse.IsDeclined=response.FullResponse.Response.FailureData.IsDecline;
					paySimpleResponse.MerchantActionText=response.FullResponse.Response.FailureData.MerchantActionText;
				}
				return paySimpleResponse;
			}

			///<summary>Gets all webhook subscriptions</summary>
			public static List<ApiWebhookResponse> GetWebhooks(string authHeader) {
				var response=Request(ApiRoute.AllWebhooks,HttpMethod.Get,authHeader,"",
				#region Response
					new {
						total_item_count=(long)0,
						data=new [] { new {
								id="",
								url="",
								event_types=new [] {""},
								is_active=false,								
							}
						}
					}
				);
				#endregion
				if(response==null || response.FullResponse==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				List<ApiWebhookResponse> listWebhooks=new List<ApiWebhookResponse>();
				if(response.FullResponse.total_item_count==0) {
					//no items were in the list, webhooks do not exist
					return listWebhooks;
				}
				for(int i=0;i<response.FullResponse.total_item_count;i++) {
					if(response.FullResponse.data.Length<=i || response.FullResponse.data[i]==null) {
						throw new Exception("Unexpected response from PaySimple: "+response.RawResponse);
					}
					ApiWebhookResponse webhookData=new ApiWebhookResponse();
					webhookData.WebhookId=response.FullResponse.data[i].id;
					webhookData.WebhookUrl=response.FullResponse.data[i].url;
					webhookData.IsActive=response.FullResponse.data[i].is_active;
					webhookData.WebhookTypes=new List<string>();
					if(response.FullResponse.data[i].event_types==null) {
						throw new Exception("Unexpected response from PaySimple: "+response.RawResponse);
					}
					for(int j=0;j<response.FullResponse.data[i].event_types.Count();j++) {
						webhookData.WebhookTypes.Add(response.FullResponse.data[i].event_types[j]);
					}
					listWebhooks.Add(webhookData);
				}
				return listWebhooks;
			}

			public static string PostWebhook(string authHeader,string postData) {
				var response=Request(ApiRoute.Webhook,HttpMethod.Post,authHeader,postData,
					new {
						data=new {
							id="",
						}
					}
				);
				if(response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				return response.FullResponse.data.id;
			}

			///<summary>The following mock results are per PaySimple's Declined result documentation</summary>
			private static RequestResponse<T> MockDeclinedPaymentResponse<T>(T resType) {
				string resStr=@"
					{
						""Meta"": {
							""Errors"": null,
							""HttpStatus"": ""Created"",
							""HttpStatusCode"": 201,
							""PagingDetails"": null
						},
						""Response"": {
							""CustomerId"": 8051100,
							""CustomerFirstName"": ""John"",
							""CustomerLastName"": ""Doe"",
							""CustomerCompany"": ""Great Plumbing"",
							""ReferenceId"": 0,
							""Status"": ""Failed"",
							""RecurringScheduleId"": 0,
							""PaymentType"": ""CC"",
							""PaymentSubType"": ""Moto"",
							""ProviderAuthCode"": ""    DECLINE     "",
							""TraceNumber"": ""05"",
							""PaymentDate"": ""2018-05-09T06:00:00Z"",
							""ReturnDate"": null,
							""EstimatedSettleDate"": null,
							""EstimatedDepositDate"": null,
							""ActualSettledDate"": ""2018-05-09T06:00:00Z"",
							""CanVoidUntil"": ""2018-05-10T02:45:00Z"",
							""FailureData"": {
								""Code"": ""4005"",
								""Description"": ""Do Not Honor"",
								""MerchantActionText"": ""Try a different payment method"",
								""IsDecline"": true
							},
							""RequiresReceipt"": false,
							""CVV"": null,
							""AccountId"": 7320843,
							""InvoiceId"": null,
							""Amount"": 9999.01,
							""IsDebit"": false,
							""InvoiceNumber"": null,
							""PurchaseOrderNumber"": null,
							""OrderId"": null,
							""Description"": null,
							""Latitude"": null,
							""Longitude"": null,
							""SuccessReceiptOptions"": null,
							""FailureReceiptOptions"": null,
							""Id"": 29124043,
							""LastModified"": ""2018-05-09T21:03:45Z"",
							""CreatedOn"": ""2018-05-09T21:03:45Z""
						}
					}";
				return new RequestResponse<T>(JsonConvert.DeserializeAnonymousType(resStr,resType),resStr);
			}

			///<summary>Throws exceptions for http codes of 300 or more from API call.</summary>
			public static ApiResponse PutPaymentVoided(string authHeader,string paymentId) {
				var response=Request(ApiRoute.PaymentVoided,HttpMethod.Put,authHeader,"",
					#region ResponseType Object
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] { new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							CustomerId=(long)0,
							CustomerFirstName="",
							CustomerLastName="",
							CustomerCompany="",
							ReferenceId=(long)0,
							Status="Authorized",
							RecurringScheduleId=(long)0,
							PaymentType="CC",
							PaymentSubType="MOTO",
							ProviderAuthCode="Approved",
							TraceNumber="",
							PaymentDate="",
							ReturnDate="",
							EstimatedSettleDate="",
							ActualSettledDate="",
							CanVoidUntil="",
							FailureData=new {
								Code="",
								Description="",
								MerchantActionText=""
							},
							AccountId=(long)0,
							InvoiceId="",
							Amount=(decimal)0.0,
							IsDebit=false,
							InvoiceNumber="123AB",//Can have alpha characters
							PurchaseOrderNumber="",
							OrderId="",
							Description="",
							Latitude="",
							Longitude="",
							SuccessReceiptOptions="",
							FailureReceiptOptions="",
							Id=(long)0,
							LastModified="",
							CreatedOn="",
						}
					}
					#endregion
					,paymentId
				);
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				if(response.FullResponse.Response.Status!="Voided") {
					throw new ODException(Lans.g("PaySimple","Payment could not be voided.  Please try again."));
				}
				return new ApiResponse() {
					Status=response.FullResponse.Response.Status,
					AuthCode=response.FullResponse.Response.ProviderAuthCode,
					PaySimpleToken=response.FullResponse.Response.AccountId.ToString(),
					RefNumber=response.FullResponse.Response.Id.ToString(),
					TransType=TransType.VOID,
					Amount=response.FullResponse.Response.Amount,
				};
			}

			///<summary>Throws exceptions for http codes of 300 or more from API call.</summary>
			public static ApiResponse PutPaymentReversed(string authHeader,string paymentId) {
				var response=Request(ApiRoute.PaymentReversed,HttpMethod.Put,authHeader,"",
					#region ResponseType Object
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] { new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							CustomerId=(long)0,
							CustomerFirstName="",
							CustomerLastName="",
							CustomerCompany="",
							ReferenceId=(long)0,
							Status="Authorized",
							RecurringScheduleId=(long)0,
							PaymentType="CC",
							PaymentSubType="MOTO",
							ProviderAuthCode="Approved",
							TraceNumber="",
							PaymentDate="",
							ReturnDate="",
							EstimatedSettleDate="",
							ActualSettledDate="",
							CanVoidUntil="",
							FailureData=new {
								Code="",
								Description="",
								MerchantActionText=""
							},
							AccountId=(long)0,
							InvoiceId="",
							Amount=(decimal)0.0,
							IsDebit=false,
							InvoiceNumber="123AB",//Can have alpha characters
							PurchaseOrderNumber="",
							OrderId="",
							Description="",
							Latitude="",
							Longitude="",
							SuccessReceiptOptions="",
							FailureReceiptOptions="",
							Id=(long)0,
							LastModified="",
							CreatedOn="",
						}
					}
					#endregion
					,paymentId
				);
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				if(response.FullResponse.Response.Status!="ReversePosted") {
					throw new ODException(Lans.g("PaySimple","Payment could not be reversed.  Please try again."));
				}
				return new ApiResponse() {
					Status=response.FullResponse.Response.Status,
					AuthCode=response.FullResponse.Response.ProviderAuthCode,
					PaySimpleToken=response.FullResponse.Response.AccountId.ToString(),
					RefNumber=response.FullResponse.Response.ReferenceId.ToString(),//TODO:  Check that this ID is actually the same as paymentID.  I would prefer to get it from PaySimple instead of my parameter,
					TransType=TransType.RETURN,
					Amount=response.FullResponse.Response.Amount,
				};
			}

			///<summary>Throws exceptions for http codes of 300 or more from API call.</summary>
			public static ApiResponse PostAccountACH(string authHeader,string postData) {
				var response=Request(ApiRoute.AccountACH,HttpMethod.Post,authHeader,postData,
					#region ResponseType Object
					new {
						Meta=new {
							Errors=new {
								ErrorCode="InvalidInput",
								ErrorMessages=new [] { new {
										Field="",
										Message="",
									}
								}
							},
							HttpStatus="",
							HttpStatusCode="",
							PagingDetails="",
						},
						Response=new {
							IsCheckingAccount=true,
							RoutingNumber="",
							AccountNumber="",
							BankName="",
							CustomerId=(long)0,
							IsDefault=true,
							Id=395560,
							LastModified="",
							CreatedOn=""
						}
					}
					#endregion
				);
				if(response==null || response.FullResponse==null || response.FullResponse.Response==null) {
					throw new Exception("Unexpected response from PaySimple"+(response!=null ? ": "+response.RawResponse : ""));
				}
				return new ApiResponse() {
					Status="",
					AuthCode="",
					PaySimpleToken=response.FullResponse.Response.Id.ToString(),
					RefNumber="",
					TransType=TransType.AUTH,
					CCSource=CreditCardSource.PaySimpleACH,
				};
			}

			///<summary>Throws exceptions if http code from API call is not 204.</summary>
			public static ApiResponse DeleteAccountCreditCard(string authHeader,string accountId) {
				return DeleteAccount(authHeader,accountId,ApiRoute.AccountCreditCard);
			}

			///<summary>Throws exceptions if http code from API call is not 204.</summary>
			public static ApiResponse DeleteAccountACH(string authHeader,string accountId) {
				return DeleteAccount(authHeader,accountId,ApiRoute.AccountACH);
			}

			///<summary>Throws exceptions if http code from API call is not 204.</summary>
			private static ApiResponse DeleteAccount(string authHeader,string accountId,ApiRoute route) {
				var response=Request(route,HttpMethod.Delete,authHeader,"","",accountId);
				return new ApiResponse() {
					Status="",
					AuthCode="",
					PaySimpleToken="",
					RefNumber="",
					TransType=TransType.DELETE,
				};
			}

			///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
			private static RequestResponse<T> Request<T>(ApiRoute route,HttpMethod method,string authHeader,string body,T responseType,string routeId="") {
				using(WebClient client=new WebClient()) {
					client.Headers[HttpRequestHeader.Accept]="application/json";
					client.Headers[HttpRequestHeader.ContentType]="application/json";
					client.Headers[HttpRequestHeader.Authorization]=authHeader;
					client.Encoding=UnicodeEncoding.UTF8;
					//Post with Authorization headers and a body comprised of a JSON serialized anonymous type.
					try {
						string res="";
						if(method==HttpMethod.Get) {
							res=client.DownloadString(GetApiUrl(route,routeId));
						}
						else if(method==HttpMethod.Post) {
							res=client.UploadString(GetApiUrl(route,routeId),HttpMethod.Post.Method,body);
						}
						else if(method==HttpMethod.Put) {
							res=client.UploadString(GetApiUrl(route,routeId),HttpMethod.Put.Method,body);
						}
						else if(method==HttpMethod.Delete) {
							//PaySimple doesn't return a response body, and WebClient doesn't allow us to get the status from the underlying response.
							HttpWebRequest request=(HttpWebRequest)WebRequest.Create(GetApiUrl(route,routeId));
							request.Method=HttpMethod.Delete.Method;
							request.Accept="application/json";
							request.ContentType="application/json";
							request.Headers.Add(HttpRequestHeader.Authorization,authHeader);
							HttpWebResponse response=(HttpWebResponse)request.GetResponse();
							res=new StreamReader(response.GetResponseStream()).ReadToEnd();
							if(response.StatusCode!=HttpStatusCode.NoContent) {
								throw new Exception("Error deleting: "+response.StatusCode+" - "+res);
							}
						}
						else {
							throw new Exception("Unsupported HttpMethod type: "+method.Method);
						}
						if(ODBuild.IsDebug()) {
							if((typeof(T)==typeof(string))) {//If user wants the entire json response as a string
								return new RequestResponse<T>((T)Convert.ChangeType(res,typeof(T)),res);
							}
						}
						return new RequestResponse<T>(JsonConvert.DeserializeAnonymousType(res,responseType),res);
					}
					catch(WebException wex) {
						string res="";
						using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
							res=sr.ReadToEnd();
						}
						if(string.IsNullOrWhiteSpace(res)) {
							//The response didn't contain a body.  Through my limited testing, it only happens for 401 (Unauthorized) requests.
							if(wex.Response.GetType()==typeof(HttpWebResponse)) {
								HttpStatusCode statusCode=((HttpWebResponse)wex.Response).StatusCode;
								if(statusCode==HttpStatusCode.Unauthorized) {
									throw new ODException(Lans.g("PaySimple","Invalid PaySimple credentials.  Check your Username and Key and try again."));
								}
							}
						}
						else {
							HandleWebException(res);
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

			///<summary>Includes the deserialized request object along with the raw response from PaySimple.</summary>
			public class RequestResponse<T> {
				public T FullResponse;
				public string RawResponse;

				public RequestResponse(T response,string rawResponse) {
					FullResponse=response;
					RawResponse=rawResponse;
				}
			}			

#region MakePostData

			public static string MakeNewPaymentData(long accountId,decimal amt,string cvv="") {
				return JsonConvert.SerializeObject(new {
					//Required fields:
					AccountId=accountId,
					Amount=amt,
					//Optional fields:
					//IsDebit=true,
					CVV=cvv,
					//PaymentSubType="MOTO",
					//InvoiceId="",
					//InvoiceNumber="",
					//PurchaseOrderNumber = "",
					//OrderId = "",
					//Description = payment.PayNote,
					//Latitude="",
					//Longitude="",
					//SuccessReceiptOptions="",
					//SendToCustomer=false,//Dictates if a receipt gets emailed to the customer after the payment is made
					//SendToOtherAddresses="",//A specific email (or emails, surrounded in brackets separated by commas) to send the receipt to.
					//FailureReceiptOptions="",
					//SendToCustomer=false,
					//SendToOtherAddresses="",
				});
			}

			public static string MakeNewPaymentACHData(long accountId,decimal amt) {
				return JsonConvert.SerializeObject(new {
					//Required fields:
					AccountId=accountId,
					Amount=amt,
					//Optional fields:
					//IsDebit=false,
					//CVV="",
					PaymentSubType="PPD",//Prearranged Payment and Deposit
					//InvoiceId="",
					//InvoiceNumber="",
					//PurchaseOrderNumber = "",
					//OrderId = "",
					//Description = payment.PayNote,
					//Latitude="",
					//Longitude="",
					//SuccessReceiptOptions="",
					//SendToCustomer=false,//Dictates if a receipt gets emailed to the customer after the payment is made
					//SendToOtherAddresses="",//A specific email (or emails, surrounded in brackets separated by commas) to send the receipt to.
					//FailureReceiptOptions="",
					//SendToCustomer=false,
					//SendToOtherAddresses="",
				});
			}

			public static string MakeNewCustomerData(string fname,string lname,string idInDb="") {
				return JsonConvert.SerializeObject(new {
					//Required fields:
					FirstName=fname,
					LastName=lname,
					ShippingSameAsBilling=true,//Hardcoded because we don't support this
					//Optional fields:
					//BillingAddress=new {
					//	StreetAddress1="",
					//	StreetAddress2="",
					//	City="",
					//	StateCode="",
					//	ZipCode="",
					//	Country="",
					//},
					//ShippingAddress="",
					//Company="",
					//Notes="",
					CustomerAccount=idInDb,
					//Email="",
					//Phone="",
				});
			}

			public static string MakeNewAccountCreditCardData(long customerId,string ccNum,DateTime ccExpDate,int issuer,string billingZipCode="") {
				return JsonConvert.SerializeObject(new {
					//Required fields:
					CustomerId=customerId,
					CreditCardNumber=ccNum,
					ExpirationDate=ccExpDate.ToString("MM/yyyy"),
					Issuer=issuer,
					IsDefault=false,
					//Optional fields:
					BillingZipCode=billingZipCode,
				});
			}

			public static string MakeNewAccountACHData(long customerId,string routingNumber,string acctNumber,string bankName,bool isCheckings) {
				return JsonConvert.SerializeObject(new {
					//Required fields:
					CustomerId=customerId,
					RoutingNumber=routingNumber,
					AccountNumber=acctNumber,
					BankName=bankName,
					IsCheckingAccount=isCheckings,
					IsDefault=false,
				});
			}

			public static string MakeNewWebhookData(string url,string[] eventTypes,bool isActive=true) {
				return JsonConvert.SerializeObject(new {
					Url=url,
					IsActive=isActive,
					Event_Types=eventTypes
				});
			}

			#endregion
			public static string GetWebhookAuthHeader(string apiUserName,string apiKey) {
				return string.Format("basic {0}:{1}",apiUserName,apiKey);
			}

			public static string GetAuthHeader(string apiUserName,string apiKey) {
				string nowAsString=DateTime.Now.ToString(@"yyyy-MM-ddTHH\:mm\:sszzz");//This matches the PaySimple documentation for C#.
				string hash="";
				Encoding encoding=Encoding.UTF8;
				using(System.Security.Cryptography.HMACSHA256 hmac = new System.Security.Cryptography.HMACSHA256(encoding.GetBytes(apiKey))) {
					byte[] hashBytes=hmac.ComputeHash(encoding.GetBytes(nowAsString));
					hash=Convert.ToBase64String(hashBytes);
				}
				return string.Format("PSSERVER AccessId = {0}; Timestamp = {1}; Signature = {2}",apiUserName,nowAsString,hash);
			}

			///<summary>Takes a credit card number and returns the issuer id from it to match PaySimple's expected values</summary>
			public static int GetCardType(string ccNum) {
				int retVal=0;
				string cardTypeName=CreditCardUtils.GetCardType(ccNum);
				if(cardTypeName=="VISA") {
					retVal=12;
				}
				else if(cardTypeName=="MASTERCARD") {
					retVal=13;
				}
				else if(cardTypeName=="AMEX") {
					retVal=14;
				}
				else if(cardTypeName=="DISCOVER") {
					retVal=15;
				}
				else {
					throw new Exception("Unsupported Card Type");
				}
				return retVal;
			}

			///<summary>Returns the full URL according to the route/route id given.</summary>
			private static string GetApiUrl(ApiRoute route,string routeId="") {
				string apiUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.PaySimpleApiURL,"https://api.paysimple.com");
				if(ODBuild.IsDebug()) {
					apiUrl="https://sandbox-api.paysimple.com";
				}
				if(!(route==ApiRoute.Webhook || route==ApiRoute.AllWebhooks)) {
					apiUrl+="/v4";
				}
				switch(route) {
					case ApiRoute.Root:
						//Do nothing.  This is to allow someone to quickly grab the URL without having to make a copy+paste reference.
						break;
					case ApiRoute.Token:
						apiUrl+="/checkouttoken";
						break;
					case ApiRoute.Customer:
						apiUrl+="/customer";
						break;
					case ApiRoute.AccountCreditCard:
						apiUrl+="/account/creditcard"+(routeId!="" ? "/"+routeId : "");
						break;
					case ApiRoute.Payment:
						apiUrl+="/payment"+(routeId!=""?"/"+routeId:"");
						break;
					case ApiRoute.PaymentReversed:
						apiUrl+="/payment/"+routeId+"/reverse";
						break;
					case ApiRoute.PaymentVoided:
						apiUrl+="/payment/"+routeId+"/void";
						break;
					case ApiRoute.CustomerToken:
						apiUrl+="/customer/"+routeId+"/token";
						break;
					case ApiRoute.AccountACH:
						apiUrl+="/account/ach"+(routeId!="" ? "/"+routeId : "");
						break;
					case ApiRoute.AllWebhooks://To get all webhooks
						apiUrl+="/ps/webhook/subscriptions";
						break;
					case ApiRoute.Webhook://To POST one webhook
						apiUrl+="/ps/webhook/subscription";
						break;
					default:
						break;
				}
				return apiUrl;
			}

			private static void HandleWebException(string paySimpleErrorJson) {
				var errorObj=JsonConvert.DeserializeAnonymousType(paySimpleErrorJson,new {
					Meta=new {
						Errors=new {
							ErrorCode="InvalidInput",
							ErrorMessages=new[] {new {
									Field="",
									Message="",
								}
							}
						},
						HttpStatus="",
						HttpStatusCode=HttpStatusCode.NotFound,
						PagingDetails="",
					}
				});
				try {
					//Make assumptions that every response from PaySimple matches their API documentation schema.
					//If something fails we should default to the exception that was originally thrown in the calling method.
					var metaObj=errorObj.Meta;
					var errors=metaObj.Errors;
					StringBuilder strbError=new StringBuilder();
					strbError.AppendLine("PaySimple ErrorCode:  "+errors.ErrorCode);
					List<string> listErrorMessages=new List<string>();
					if(errors.ErrorMessages.Length>0) {
						strbError.AppendLine("PaySimple Error Message(s):");
						errors.ErrorMessages.ToList().ForEach(x => {
							strbError.AppendLine(x.Message);
							listErrorMessages.Add(x.Message);
						});
					}
					//Purposefully not Lans.g and throwing PaySimpleException.  I don't want this to look like a generic exception being caught.
					throw new PaySimpleException(strbError.ToString(),errors.ErrorCode,listErrorMessages);
				}
				catch(PaySimpleException ex) {
					ex.DoNothing();
					throw;//Re-throw the stringbuilder above.
				}
				catch(Exception e) {
					e.DoNothing();//The calling method should throw if this doesn't.
				}
			}

			private enum ApiRoute {
				///<summary>Base URL, no route</summary>
				Root,
				///<summary>CheckoutToken</summary>
				Token,
				///<summary>Customer</summary>
				Customer,
				///<summary>Account/CreditCard</summary>
				AccountCreditCard,
				///<summary>Payment</summary>
				Payment,
				///<summary>Payment/PaymentId/Reverse</summary>
				PaymentReversed,
				///<summary>Payment/PaymentId/Void</summary>
				PaymentVoided,
				///<summary>Customer/Token</summary>
				CustomerToken,
				///<summary>Account/ACH</summary>
				AccountACH,
				///<summary>/ps/webhook/subscriptions</summary>
				AllWebhooks,
				///<summary>/ps/webhook/subscription</summary>
				Webhook,
			}
		}

		public enum TransType {
			///<summary>Used to make a payment.</summary>
			SALE,
			///<summary>Used to add a credit card.</summary>
			AUTH,
			///<summary>Used to reverse a payment.</summary>
			RETURN,
			///<summary>Used to cancel a payment.</summary>
			VOID,
			///<summary>Used to cancel a credit card or ACH account.</summary>
			DELETE,
		}
	}

	///<summary>Exception class for errors returned from PayConnect. The Message field is a human-readable error that can be shown to the user.
	///</summary>
	public class PaySimpleException:ApplicationException {
		///<summary>Error code returned from PaySimple.</summary>
		public string ErrorCode;
		///<summary>Error messages returned from PaySimple.</summary>
		public List<string> ErrorMessages;
		///<summary>The type of error.</summary>
		public PaySimpleError ErrorType;
		///<summary>The PaySimple customer id sent in this request.</summary>
		public long CustomerId;

		public PaySimpleException(string message,string errorCode,List<string> errorMessages) : base(message) {
			ErrorCode=errorCode;
			ErrorMessages=errorMessages;
		}
	}

	///<summary>The PaySimple errors that we handle differently.</summary>
	public enum PaySimpleError {
		///<summary>We have not classified this error.</summary>
		NotSpecified,
		///<summary>A Customer object does not exist in PaySimple with the Customer id we provided.</summary>
		CustomerDoesNotExist,
	}
}
