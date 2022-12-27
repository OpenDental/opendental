using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness.WebTypes.Shared.XWeb;
using WebServiceSerializer;

namespace OpenDentBusiness {
	public class EdgeExpress {

		public static List<EdgeExpressTransType> ListTerminalTransTypes=new List<EdgeExpressTransType>() {
			EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditReturn,EdgeExpressTransType.DebitSale,EdgeExpressTransType.DebitReturn,
			EdgeExpressTransType.CreditOnlineCapture,EdgeExpressTransType.CreditAuth,EdgeExpressTransType.CreditVoid
		};

		public static List<EdgeExpressTransType> ListWebTransTypes=new List<EdgeExpressTransType>() {
			EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditReturn,EdgeExpressTransType.CreditOnlineCapture,
			EdgeExpressTransType.CreditAuth,EdgeExpressTransType.CreditVoid
		};

		///<summary>The amount of time that the eConnector will check hosted pay URLs.</summary>
		private static TimeSpan _formTimeout=TimeSpan.FromMinutes(10);

		#region Private methods
		///<summary>Adds the base params for an EdgeExpress request.</summary>
		private static void WriteEdgeExpressBaseRequest(long clinicNum,EdgeExpressTransType edgeExpressTransactionType,XmlWriter xmlWriter,bool isWebPayment=true,double amount=0) {
			ProgramProperties.GetXWebCreds(clinicNum,out WebPaymentProperties xwebProperties);
			Program progEdge=Programs.GetCur(ProgramName.EdgeExpress);
			Program progXWeb=Programs.GetCur(ProgramName.Xcharge);
			if((!progEdge.Enabled && !progXWeb.Enabled)) {
				throw new ODException("Both XWeb and EdgeExpress are not enabled. Please enable one of these programs.",ODException.ErrorCodes.XWebProgramProperties);
			}
			if(isWebPayment && !xwebProperties.IsPaymentsAllowed) {
				throw new ODException("Clinic or Practice has online payments disabled",ODException.ErrorCodes.XWebProgramProperties);
			}
			xmlWriter.WriteStartElement("REQUEST");
			xmlWriter.WriteElementString("XWEBID",xwebProperties.XWebID);
			xmlWriter.WriteElementString("XWEBTERMINALID",xwebProperties.TerminalID);
			xmlWriter.WriteElementString("XWEBAUTHKEY",xwebProperties.AuthKey);
			xmlWriter.WriteElementString("TRANSACTIONTYPE",edgeExpressTransactionType.ToString().ToUpper());
			ODException.SwallowAnyException(() => Logger.LogVerbose(GetRequestLogText(xwebProperties,amount),subDirectory:"EdgeExpress"));
		}

		public static string GetRequestLogText(WebPaymentProperties xwebProperties,double amount) {
			string terminalID="NULL";
			string authKey="NULL";
			if(xwebProperties!=null) {
				if(xwebProperties.TerminalID!=null) {
					terminalID=xwebProperties.TerminalID;
				}
				if(xwebProperties.AuthKey!=null) {
					//Only display the first half of the AuthKey.
					int countVisibleChars=(xwebProperties.AuthKey.Length / 2);
					authKey=xwebProperties.AuthKey.Substring(0,countVisibleChars) + new string('X',countVisibleChars);
				}
			}
			return $"TerminalID:{terminalID}  AuthKey:{authKey}  Amount:{amount:C}";
		}
		#endregion Private methods

		#region Helper classes
		///<summary>Represents a response from the EdgeExpress RCM program.</summary>
		public class RcmResponse {
			public string DUPLICATECARD;
			public DateTime DATE_TIME;
			public string HOSTRESPONSECODE;
			public string HOSTRESPONSEDESCRIPTION;
			///<summary>Only included if the transaction failed.</summary>
			public string RESPONSE="-1";
			///<summary>Only included if the transaction succeeded.</summary>
			public string RESULT="-1";
			public string RESULTMSG;
			public string APPROVEDAMOUNT;
			public string BATCHNO;
			public string BATCHAMOUNT;
			public string APPROVALCODE;
			public string ACCOUNT;
			public string CARDCODERESPONSE;
			public string CARDTYPE;
			public string CARDBRAND;
			public string CARDBRANDSHORT;
			public string LANGUAGE;
			public string ALIAS;
			public string ENTRYTYPE;
			public string RECEIPTTEXT;
			public string EXPMONTH;
			public string EXPYEAR;
			public string TRANSACTIONID;
			///<summary>Not a part of the response from EdgeExpress.</summary>
			public EdgeExpressTransType TransType;
			///<summary>Whether or not the tranaction was successful.</summary>
			public bool IsSuccess => RESULT=="0";

			///<summary>Returns a string that can be used as the note for a payment.</summary>
			public string GetPayNote() {
				StringBuilder strbNote=new StringBuilder();
				strbNote.AppendLine(Lans.g("EdgeExpress","TRANSACTIONTYPE:")+" "+TransType);
				foreach(FieldInfo fi in GetType().GetFields().Where(x => x.IsPublic).OrderByDescending(x => x.Name==nameof(RESULTMSG))) {
					if(fi.Name.In(nameof(RECEIPTTEXT),nameof(ALIAS),nameof(TransType))
						|| (fi.Name.In(nameof(RESPONSE),nameof(RESULT)) && fi.GetValue(this).ToString()=="-1")) 
					{
						continue;
					}
					object value=fi.GetValue(this);
					if(value==null || (value.GetType()==typeof(string) && string.IsNullOrEmpty(value.ToString()))
						|| (value.GetType()==typeof(DateTime) && ((DateTime)value).Year < 1880)) 
					{
						continue;
					}
					strbNote.AppendLine(fi.Name+": "+value);
				}
				return strbNote.ToString();
			}
		}

		#endregion Helper classes
		#region RCM Methods
		///<summary>This class uses the locally installed RCM client to send transactions to EdgeExpress via a physical payment terminal.</summary>
		public class RCM {
			///<summary>The URL to send the request to the RCM program running on the local machine. localsystem.paygateway.com resolves to 127.0.0.1.
			///</summary>
			private const string _edgeExpressRCMURL="https://localsystem.paygateway.com:21113/RcmService.svc/Initialize";

			///<summary>Checks the process list of the local machine to see if there is a program called "RCM" currently running.</summary>
			public static bool IsRCMRunning {
				get {
					bool isRCMRunning=false;
					if(ODBuild.IsWeb()) {
						isRCMRunning=ODCloudClient.IsProcessRunning("rcm");
					}
					else {
						try {
							isRCMRunning=Process.GetProcesses().Any(x => x.ProcessName.ToLower().Contains("rcm"));
						}
						catch(Exception ex) {
							ex.DoNothing();
						}
					}
					return isRCMRunning;
				}
			}

			///<summary>Sends the request to EdgeExpress RCM and returns the response.</summary>
			public static RcmResponse SendEdgeExpressRequest(Patient patient,long clinicNum,EdgeExpressTransType edgeExpressTransactionType,bool isWebPayment,double amount = 0,
				bool doPromptForSignature = false,bool doCreateToken = false,string aliasToken = "",string transactionId = "",decimal cashBackAmt = 0,string expDate = "") {
				RcmResponse rcmResponse=null;
				StringBuilder strBldXml=new StringBuilder();
				XmlWriter xmlWriter=XmlWriter.Create(strBldXml);
				WriteEdgeExpressBaseRequest(clinicNum,edgeExpressTransactionType,xmlWriter,isWebPayment,amount);
				AddOtherParamsEdgeExpress(xmlWriter,edgeExpressTransactionType,patient,amount,doPromptForSignature,doCreateToken,aliasToken,transactionId,
					cashBackAmt,expDate);
				string url=$"{_edgeExpressRCMURL}?xl2Parameters={strBldXml}";
				string response;
				if(ODBuild.IsWeb()) {
					//Timeout is 120 seconds because that's how long we set the timeout for PayConnect terminal.
					response=ODCloudClient.DownloadString(url,timeoutSecs: 120,doShowProgressBar: false);
				}
				else {
					using WebClient client=new WebClient();
					response=client.DownloadString(url);
				}
				//Response will look like: 
				//{"Description":"OK","IsSuccessful":true,"RcmResponse":"{\"RESPONSE\":{\"RESPONSE\":\"3\",\"RESULTMSG\":\"Transaction Cancelled\"}}","XmlRcmResponse":null}
				var jsonResponse=new {
					Description="",
					IsSuccessful=false,
					RcmResponse="",
				};
				jsonResponse=JsonConvert.DeserializeAnonymousType(response,jsonResponse);
				if(!jsonResponse.IsSuccessful) {
					throw new ODException("Error from EdgeExpress: "+jsonResponse.Description);
				}
				var innerResponse=new {
					RESPONSE=new RcmResponse(),
				};
				innerResponse=JsonConvert.DeserializeAnonymousType(jsonResponse.RcmResponse,innerResponse);
				rcmResponse=innerResponse.RESPONSE;
				rcmResponse.TransType=edgeExpressTransactionType;
				//},startingMessage:"Processing credit card transaction...");
				return rcmResponse;
			}

			///<summary>Adds additional parameters to the specified transaction type. For EdgeExpress RCM only.</summary>
			private static void AddOtherParamsEdgeExpress(XmlWriter xmlWriter,EdgeExpressTransType edgeExpressTransactionType,Patient patient,
				double amount,bool doPromptForSignature,bool doCreateToken,string aliasToken,string transactionId,decimal cashBackAmt,string expDate) {
				if(edgeExpressTransactionType.In(EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditReturn,EdgeExpressTransType.CreditAuth,
					EdgeExpressTransType.CreditOnlineCapture,EdgeExpressTransType.DebitSale,EdgeExpressTransType.DebitReturn)) {
					xmlWriter.WriteElementString("AMOUNT",amount.ToString());
				}
				//EdgeExpress only Accepts CLERK Id's up to 20 characters long. Payment will fail if UserName is over 20 characters long.
				if(edgeExpressTransactionType.In(EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditReturn,EdgeExpressTransType.CreditAuth)) {
					xmlWriter.WriteElementString("RECEIPTLINEITEMS","Pat"+patient.PatNum);
					xmlWriter.WriteElementString("PROMPTSIGNATURE",doPromptForSignature ? "True" : "False");
					xmlWriter.WriteElementString("CREATEALIAS",doCreateToken ? "TRUE" : "FALSE");
					if(!string.IsNullOrEmpty(aliasToken)) {
						xmlWriter.WriteElementString("ALIAS",aliasToken);
					}
					xmlWriter.WriteElementString("INVOICENO",$"PAT{patient.PatNum}");
					xmlWriter.WriteElementString("CLERK",StringTools.Truncate(Security.CurUser.UserName,20));
				}
				if(edgeExpressTransactionType.In(EdgeExpressTransType.AliasUpdate,EdgeExpressTransType.AliasDelete)) {
					xmlWriter.WriteElementString("ALIAS",aliasToken);
				}
				if(edgeExpressTransactionType.In(EdgeExpressTransType.AliasUpdate)) {
					xmlWriter.WriteElementString("EXPDATE",expDate);
				}
				if(edgeExpressTransactionType.In(EdgeExpressTransType.CreditVoid,EdgeExpressTransType.CreditReturn,EdgeExpressTransType.CreditOnlineCapture)) {
					xmlWriter.WriteElementString("TRANSACTIONID",transactionId);
				}
				if(edgeExpressTransactionType.In(EdgeExpressTransType.CreditOnlineCapture)) {
					xmlWriter.WriteElementString("INVOICENO",$"PAT{patient.PatNum}");
					xmlWriter.WriteElementString("CLERK",StringTools.Truncate(Security.CurUser.UserName,20));
				}
				if(edgeExpressTransactionType.In(EdgeExpressTransType.DebitSale)) {
					xmlWriter.WriteElementString("CASHBACKAMOUNT",cashBackAmt.ToString());
					xmlWriter.WriteElementString("INVOICENO",$"PAT{patient.PatNum}");
					xmlWriter.WriteElementString("CLERK",StringTools.Truncate(Security.CurUser.UserName,20));
				}
				if(edgeExpressTransactionType.In(EdgeExpressTransType.DebitReturn)) {
					xmlWriter.WriteElementString("INVOICENO",$"PAT{patient.PatNum}");
					xmlWriter.WriteElementString("CLERK",StringTools.Truncate(Security.CurUser.UserName,20));
				}
				//Include the patient first and last name with every transaction type if they are available.
				if(!string.IsNullOrWhiteSpace(patient.FName)) {
					xmlWriter.WriteElementString("CUSTOMERFIRSTNAME",patient.FName);
				}
				if(!string.IsNullOrWhiteSpace(patient.LName)) {
					xmlWriter.WriteElementString("CUSTOMERLASTNAME",patient.LName);
				}
				xmlWriter.WriteEndElement();//REQUEST
				xmlWriter.Flush();
			}

			///<summary>Creates a credit card alias.</summary>
			public static RcmResponse CreateAlias(Patient patient,long clinicNum,bool isWebPayment) {
				return SendEdgeExpressRequest(patient,clinicNum,EdgeExpressTransType.AliasCreate,isWebPayment,doCreateToken: true);
			}

			///<summary>Updates the expiration date for the alias.</summary>
			public static RcmResponse UpdateAlias(Patient patient,long clinicNum,string aliasToken,DateTime expDate,bool isWebPayment) {
				return SendEdgeExpressRequest(patient,clinicNum,EdgeExpressTransType.AliasUpdate,isWebPayment,aliasToken: aliasToken,expDate: expDate.ToString("MMyy"));
			}

			///<summary>Deletes the alias.</summary>
			public static RcmResponse DeleteAlias(Patient patient,long clinicNum,string aliasToken,bool isWebPayment) {
				return SendEdgeExpressRequest(patient,clinicNum,EdgeExpressTransType.AliasDelete,isWebPayment,aliasToken: aliasToken);
			}

			///<summary>For credit only, not debit. Voids the transaction.</summary>
			public static RcmResponse VoidTransaction(Patient patient,long clinicNum,string transactionId,bool isWebPayment) {
				return SendEdgeExpressRequest(patient,clinicNum,EdgeExpressTransType.CreditVoid,isWebPayment,transactionId: transactionId);
			}
		}
		#endregion RCM Methods
		#region Card Not Present API

		///<summary>This class makes calls to the EdgeExpress Card Not Present API.</summary>
		public class CNP {
			///<summary>Url to setup a payment page and get a OTK.</summary>
			private static string _edgeExpressHostPayUrl {
				get {
					string edgeExpressHostPayUrl="https://ee.paygateway.com/HostPayService/v1/hostpay/transactions/";
					if(ODBuild.IsDebug() || XWebs.UseXWebTestGateway) {
						edgeExpressHostPayUrl="https://ee.test.paygateway.com/HostPayService/v1/hostpay/transactions";
					}
					return Introspection.GetOverride(Introspection.IntrospectionEntity.EdgeExpressHostPay,edgeExpressHostPayUrl);
				}
			}

			///<summary>Url for transactions that do not require a payment page.</summary>
			private static string _edgeExpressDirectPayUrl {
				get {
					string edgeExpressDirectPayUrl="https://ee.paygateway.com/HostPayService/v1/directpay/express";
					if(ODBuild.IsDebug() || XWebs.UseXWebTestGateway) {
						edgeExpressDirectPayUrl="https://ee.test.paygateway.com/HostPayService/v1/directpay/express";
					}
					return Introspection.GetOverride(Introspection.IntrospectionEntity.EdgeExpressDirectPay,edgeExpressDirectPayUrl);
				}
			}

			///<summary>Sends a web request to the XWeb EdgeExpress API. By default the clinic will be set to the Patient's clinic, however passing in	
			///doUseCurrentClinicNum as true will set it to the Clinics.ClinicNum.</summary>
			private static XWebResponse SendEdgeExpressRequest(long patNum,EdgeExpressTransType edgeExpressTransactionType,string url,bool isWebPayment,
				double amount=0,string orderId="",bool doCreateAlias=false,string alias="",string transactionID="",bool allowDuplicates=false,
				string expDate="",bool doUseCurrentClinicNum=false,long clinicNum=-1)
			{
				Patient pat=Patients.GetLim(patNum);
				if(patNum!=pat.PatNum) {
					throw new ODException("Patient not found for PatNum: "+patNum.ToString(),ODException.ErrorCodes.XWebProgramProperties);
				}
				if(PrefC.HasClinicsEnabled) {
					if(clinicNum<=0) {
						clinicNum=pat.ClinicNum;
						if(doUseCurrentClinicNum) {
							clinicNum=Clinics.ClinicNum;
						}
					}
				}
				else {
					clinicNum=0;
				}
				bool isDirectPay=true;
				StringBuilder strBldXml=new StringBuilder();
				using(XmlWriter xmlWriter=XmlWriter.Create(strBldXml)) {
					WriteEdgeExpressBaseRequest(clinicNum,edgeExpressTransactionType,xmlWriter,isWebPayment,amount);
					orderId=string.IsNullOrEmpty(orderId) ? XWebResponses.CreateOrderId() : orderId;
					AddAdditionalTransactionParams(xmlWriter,edgeExpressTransactionType,doCreateAlias,amount,pat,alias,transactionID,orderId,allowDuplicates,expDate);
					//Only host pay calls can display a payment page, direct pay calls will return an error if we send UI parameters.
					if(url==_edgeExpressHostPayUrl) {
						AddUIParams(xmlWriter);
						isDirectPay=false;
					}
					xmlWriter.WriteEndElement();//REQUEST
				}
				string result=XWebs.XWebInputAbs.UploadData(strBldXml.ToString(),url);
				XWebResponse xResponse=CreateEdgeExpressXWebResponse(result,edgeExpressTransactionType,isDirectPay);
				xResponse.OrderId=orderId;
				xResponse.PatNum=patNum;
				xResponse.ProvNum=pat.PriProv;
				xResponse.ClinicNum=clinicNum;
				xResponse.DateTUpdate=DateTime.Now;
				xResponse.TransactionType=edgeExpressTransactionType.ToString();
				return xResponse;
			}

			///<summary>Adds additional parameters to the specified transaction type. For EdgeExpress Card Not Present only.</summary>
			private static void AddAdditionalTransactionParams(XmlWriter xmlWriter,EdgeExpressTransType edgeExpressTransactionType,bool doCreateAlias,
				double amount,Patient pat,string alias,string transactionID,string orderId,bool allowDuplicates=false,string expDate="")
			{
				//EdgeExpress CNP only Accepts CLERK Id's up to 20 characters long. Payment will fail if UserName is over 20 characters long. The CNP API guide(2019) states that it accepts up to 50, but this is not true according to testing.
				switch(edgeExpressTransactionType) {
					case EdgeExpressTransType.CreditSale:
					case EdgeExpressTransType.CreditAuth:
						xmlWriter.WriteElementString("ORDERID",orderId);
						xmlWriter.WriteElementString("AMOUNT",amount.ToString());
						xmlWriter.WriteElementString("CUSTOMERNAME",pat.GetNameFLnoPref());
						if(doCreateAlias) {
							xmlWriter.WriteElementString("CREATEALIAS","true");
						}
						if(!alias.IsNullOrEmpty()) {
							xmlWriter.WriteElementString("ALIAS",alias);
						}
						xmlWriter.WriteElementString("ALLOWDUPLICATES",allowDuplicates.ToString());
						xmlWriter.WriteElementString("INVOICENO",$"PAT{pat.PatNum}");
						if(Security.CurUser!=null && Security.CurUser.EServiceType==EServiceTypes.None) {//Not an eService, a valid user is logged in.
							xmlWriter.WriteElementString("CLERK",StringTools.Truncate(Security.CurUser.UserName,20));
						}
						break;
					case EdgeExpressTransType.CreditReturn:
						xmlWriter.WriteElementString("TRANSACTIONID",transactionID);
						xmlWriter.WriteElementString("AMOUNT",amount.ToString());
						if(doCreateAlias) {
							xmlWriter.WriteElementString("CREATEALIAS","true");
						}
						if(!alias.IsNullOrEmpty()) {
							xmlWriter.WriteElementString("ALIAS",alias);
						}
						xmlWriter.WriteElementString("INVOICENO",$"PAT{pat.PatNum}");
						if(Security.CurUser!=null && Security.CurUser.EServiceType==EServiceTypes.None) {//Not an eService, a valid user is logged in.
							xmlWriter.WriteElementString("CLERK",StringTools.Truncate(Security.CurUser.UserName,20));
						}
						break;
					case EdgeExpressTransType.CreditVoid:
						xmlWriter.WriteElementString("TRANSACTIONID",transactionID);
						xmlWriter.WriteElementString("AMOUNT",amount.ToString());
						if(!alias.IsNullOrEmpty()) {
							xmlWriter.WriteElementString("ALIAS",alias);
						}
						xmlWriter.WriteElementString("INVOICENO",$"PAT{pat.PatNum}");
						break;
					case EdgeExpressTransType.CreditOnlineCapture://Force
						xmlWriter.WriteElementString("AMOUNT",amount.ToString());
						xmlWriter.WriteElementString("TRANSACTIONID",transactionID);
						xmlWriter.WriteElementString("INVOICENO",$"PAT{pat.PatNum}");
						if(Security.CurUser!=null && Security.CurUser.EServiceType==EServiceTypes.None) {//Not an eService, a valid user is logged in.
							xmlWriter.WriteElementString("CLERK",StringTools.Truncate(Security.CurUser.UserName,20));
						}
						break;
					case EdgeExpressTransType.QueryPayment:
						xmlWriter.WriteElementString("ORDERID",orderId);
						break;
					case EdgeExpressTransType.AliasUpdate:
						xmlWriter.WriteElementString("ALIAS",alias);
						xmlWriter.WriteElementString("EXPDATE",expDate);
						break;
					case EdgeExpressTransType.AliasDelete:
						xmlWriter.WriteElementString("ALIAS",alias);
						break;
					case EdgeExpressTransType.DebitSale:
					case EdgeExpressTransType.DebitReturn:
						if(Security.CurUser!=null && Security.CurUser.EServiceType==EServiceTypes.None) {//Not an eService, a valid user is logged in.
							xmlWriter.WriteElementString("CLERK",StringTools.Truncate(Security.CurUser.UserName,20));
						}
						break;
				}
			}

			private static void AddUIParams(XmlWriter xmlWriter) {
				xmlWriter.WriteStartElement("HOSTPAYSETTING");
					xmlWriter.WriteStartElement("POSDEVICE");
					xmlWriter.WriteElementString("TYPE","KEYED");
					xmlWriter.WriteEndElement();//POSDEVICE
					xmlWriter.WriteStartElement("RETURNOPTION");
					if(ODBuild.IsDebug()) {
					xmlWriter.WriteElementString("RETURNURL","https://www.patientviewer.com/PortalPayDone.aspx"); //use this line for debugging easier
					//xmlWriter.WriteElementString("RETURNURL","http://localhost/OpenDentalWebLander/PortalPayDone.aspx");
				}
					else {
						xmlWriter.WriteElementString("RETURNURL","https://www.patientviewer.com/PortalPayDone.aspx");
					}
					xmlWriter.WriteElementString("RETURNTARGET","_self");//without this line, the top level window will be redirected to RETURNURL
					xmlWriter.WriteEndElement();//RETURNOPTION
					xmlWriter.WriteElementString("DISABLEFRAMING","false");//allows us to put the page in an iframe.
					xmlWriter.WriteStartElement("CUSTOMIZATION");
					xmlWriter.WriteStartElement("PAGE");
					xmlWriter.WriteStartElement("BILLINGFIRSTNAME");
					xmlWriter.WriteElementString("VISIBLE","false");//They've already entered their name
					xmlWriter.WriteEndElement();//BILLINGFIRSTNAME
					xmlWriter.WriteStartElement("BILLINGMIDDLENAME");
					xmlWriter.WriteElementString("VISIBLE","false");//They've already entered their name
					xmlWriter.WriteEndElement();//BILLINGMIDDLENAME
					xmlWriter.WriteStartElement("BILLINGLASTNAME");
					xmlWriter.WriteElementString("VISIBLE","false");//They've already entered their name
					xmlWriter.WriteEndElement();//BILLINGLASTNAME
					xmlWriter.WriteStartElement("BILLINGCOMPANY");
					xmlWriter.WriteElementString("VISIBLE","false");
					xmlWriter.WriteEndElement();//BILLINGCOMPANY
					xmlWriter.WriteStartElement("BILLINGCUSTOMERTITLE");
					xmlWriter.WriteElementString("VISIBLE","false");
					xmlWriter.WriteEndElement();//BILLINGCUSTOMERTITLE
					xmlWriter.WriteEndElement();//PAGE
					xmlWriter.WriteEndElement();//CUSTOMIZATION
					xmlWriter.WriteEndElement();//HOSTPAYSETTING
			}

			///<summary>Sets the expiration and inserts the response to the db.</summary>
			private static void FinishEdgeExpressUrlRequest(XWebResponse response) {
				response.HpfExpiration=DateTime.Now.Add(_formTimeout);
				XWebResponses.Insert(response);
			}

			///<summary>Converts the XML string result from the EdgeExpress API to an XWebResponse. Throws exceptions.</summary>
			private static XWebResponse CreateEdgeExpressXWebResponse(string result,EdgeExpressTransType edgeExpressTransactionType,bool isDirectPay) {
				XWebResponse xResponse=new XWebResponse();
				if(edgeExpressTransactionType.In(EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditAuth)) {
					//The fields in the response depend on which URL is used to make the payment.
					if(isDirectPay) {
						xResponse=ConvertEdgeExpressResponse(result);
					}
					else {
						xResponse.OTK=WebSerializer.DeserializeNode(result,"SESSIONTOKEN");
						xResponse.HpfUrl=WebSerializer.DeserializeNode(result,"PAYPAGEURL");
						xResponse.TransactionStatus=XWebTransactionStatus.EdgeExpressPending;
					}
				}
				else if(edgeExpressTransactionType==EdgeExpressTransType.QueryPayment) {
					string responseCode=WebSerializer.DeserializeNode(result,"RESPONSECODE");
					XWebResponseCodes responseCodeEnum=PIn.Enum<XWebResponseCodes>(responseCode,defaultEnumOption:XWebResponseCodes.Undefined);
					if(responseCodeEnum==XWebResponseCodes.InvalidReferenceError) {
						//XWeb gives this code before the patient completes the transaction. They also give this code when the OrderId doesn't exist.
						xResponse.XWebResponseCode=XWebResponseCodes.Pending;
					}
					else {
						xResponse=ConvertEdgeExpressResponse(result);
					}
				}
				else if(edgeExpressTransactionType.In(EdgeExpressTransType.CreditVoid,
					EdgeExpressTransType.CreditReturn,
					EdgeExpressTransType.CreditOnlineCapture)
				) {
					string responseCode=WebSerializer.DeserializeNode(result,"RESPONSECODE");
					XWebResponseCodes responseCodeEnum=PIn.Enum<XWebResponseCodes>(responseCode,defaultEnumOption:XWebResponseCodes.Undefined);
					if(responseCodeEnum==XWebResponseCodes.InvalidReferenceError) {
						string transaction=WebSerializer.DeserializeNode(result,"TRANSACTIONID");
						string desc=WebSerializer.DeserializeNode(result,"RESPONSEDESCRIPTION");
						throw new ODException("Unable to process transaction: "+transaction+Environment.NewLine+desc);
					}
					xResponse=ConvertEdgeExpressResponse(result);
				}
				else if(edgeExpressTransactionType==EdgeExpressTransType.AliasDelete) {
					xResponse.XWebResponseCode=PIn.Enum<XWebResponseCodes>(WebSerializer.DeserializeNode(result,"RESPONSECODE"));
					xResponse.ResponseDescription=WebSerializer.DeserializeNode(result,"RESPONSEDESCRIPTION");
					if(xResponse.XWebResponseCode==XWebResponseCodes.AliasSuccess) {
						xResponse.TransactionStatus=XWebTransactionStatus.EdgeExpressAliasDeleted;//Not Tracked by eConnector.
					}
					else {
						xResponse.TransactionStatus=XWebTransactionStatus.EdgeExpressPending;
					}
				}
				else if(edgeExpressTransactionType==EdgeExpressTransType.AliasUpdate) {
					xResponse.XWebResponseCode=PIn.Enum<XWebResponseCodes>(WebSerializer.DeserializeNode(result,"RESPONSECODE"));
					xResponse.ResponseDescription=WebSerializer.DeserializeNode(result,"RESPONSEDESCRIPTION");
					if(xResponse.XWebResponseCode==XWebResponseCodes.AliasSuccess) {
						xResponse.TransactionStatus=XWebTransactionStatus.EdgeExpressAliasUpdated;//Not Tracked by eConnector.
					}
					else {
						xResponse.TransactionStatus=XWebTransactionStatus.EdgeExpressPending;
					}
				}
				return xResponse;
			}

			///<summary>Converts the XML string result from the EdgeExpress API for a QueryPayment to an XWebResponse.</summary>
			private static XWebResponse ConvertEdgeExpressResponse(string result) {
				EdgeExpressResponse eeResponse;
				using(StringReader sr = new StringReader(result)) {
					//XWeb's xml references this class as RESULT but OD wants to deserialize to a class called EdgeExpressResponse. 
					//We must explicitly specific the XmlRoot node name here to make that conversion.	
					eeResponse=(EdgeExpressResponse)new XmlSerializer(typeof(EdgeExpressResponse),new XmlRootAttribute("RESULT")).Deserialize(sr);
				}
				//if we don't get a valid exp date back in response, set AccountExpirationDate to the default "0001-01-01"
				DateTime expDate = DateTime.MinValue;
				if(eeResponse.EXPYEAR >= 0 && eeResponse.EXPMONTH > 0) {
					expDate=new DateTime(2000+eeResponse.EXPYEAR,eeResponse.EXPMONTH,1);
				}
				XWebResponse xResponse=new XWebResponse {
					AccountExpirationDate=expDate,
					Alias=eeResponse.ALIAS,
					Amount=(double)eeResponse.APPROVEDAMOUNT,
					ApprovalCode=eeResponse.APPROVALCODE,
					BatchNum=eeResponse.BATCHNO,
					CardBrand=eeResponse.CARDBRAND,
					CardCodeResponse=eeResponse.CARDCODERESPONSE,
					CardType=eeResponse.CARDTYPE,
					CCSource=CreditCardSource.EdgeExpressCNP,
					MaskedAcctNum=eeResponse.MASKEDCARDNUMBER,
					ProcessorResponse=eeResponse.PROCESSORRESPONSE,
					ReceiptID=eeResponse.RECEIPTID,
					ResponseCode=eeResponse.RESPONSECODE,
					ResponseDescription=eeResponse.RESPONSEDESCRIPTION,
					TransactionID=eeResponse.TRANSACTIONID,
					TransactionType=eeResponse.TRANSACTIONTYPE,
					XWebResponseCode=XWebResponse.ConvertResponseCode(eeResponse.RESPONSECODE),
				};
				return xResponse;
			}

			///<summary>Creates and returns the EdgeExpress URL and validation OTK which can be used to make a payment for an unspecified credit card.
			///</summary>
			public static XWebResponse GetUrlForPaymentPage(long patNum,string payNote,double amount,bool createAlias,
				CreditCardSource ccSource,bool isWebPayment,string alias="",bool allowDuplicates=false,string email="",string logGuid="",
				long paymentNum=0,long clinicNum=-1)
			{
				//No need to check MiddleTierRole;no call to db.
				if(!ccSource.In(CreditCardSource.XWeb,CreditCardSource.XWebPortalLogin,CreditCardSource.EdgeExpressCNP)) {
					throw new ODException("Invalid CreditCardSource: "+ccSource.ToString(),ODException.ErrorCodes.OtkArgsInvalid);
				}
				//Validate the amount.
				if(amount<0.00 || amount>99999.99) {
					throw new ODException("Invalid Amount",ODException.ErrorCodes.OtkArgsInvalid);
				}
				try {
					XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.CreditSale,_edgeExpressHostPayUrl,isWebPayment,						amount,doCreateAlias:createAlias,alias:alias,allowDuplicates:allowDuplicates,clinicNum:clinicNum);
					response.Amount=amount;
					response.PayNote=payNote;
					response.CCSource=ccSource;
					response.EmailResponse=email;
					response.LogGuid=logGuid;
					response.PaymentNum = paymentNum;
					FinishEdgeExpressUrlRequest(response);
					return response;
				}
				catch(Exception ex) {
					throw new ODException("Error creating payment request.",ex);
				}
			}

			///<summary>Makes a payment using the Direct Pay URL. Does not display a webpage and an alias is required.</summary>
			public static XWebResponse ProcessPaymentDirect(long patNum,string payNote,double amount,CreditCardSource ccSource,
				bool isWebPayment,string alias,bool allowDuplicates,List<CreditCardSource> listCreditCardSourcesEdgeExpress,
				long clinicNum=-1) 
			{
				//No need to check MiddleTierRole;no call to db.
				if(!listCreditCardSourcesEdgeExpress.Contains(ccSource)) {
					throw new ODException("Invalid CreditCardSource: "+ccSource.ToString(),ODException.ErrorCodes.OtkArgsInvalid);
				}
				//Validate the amount.
				if(amount<0.00 || amount>99999.99) {
					throw new ODException("Invalid Amount",ODException.ErrorCodes.OtkArgsInvalid);
				}
				try {
					XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.CreditSale,_edgeExpressDirectPayUrl,isWebPayment,amount,			doCreateAlias:false,alias:alias,allowDuplicates:allowDuplicates,clinicNum:clinicNum);
					response.Amount=amount;
					response.PayNote=payNote;
					response.CCSource=ccSource;
					FinishEdgeExpressUrlRequest(response);
					return response;
				}
				catch(Exception ex) {
					throw new ODException("Error creating payment request.",ex);
				}
			}

			///<summary>Creates and returns the EdgeExpress URL and validation OTK which can be used to create a credit card alias. Uses CreditAuth transaction type.</summary>
			public static XWebResponse GetUrlForCreditCardAlias(long patNum,CreditCardSource ccSource,bool isWebPayment,double amount=0,
				bool doCreateAlias=true,bool doUseCurrentClinicNum=false)
			{
				//No need to check MiddleTierRole;no call to db.
				try {
					XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.CreditAuth,_edgeExpressHostPayUrl,isWebPayment,amount,doCreateAlias:doCreateAlias,
						doUseCurrentClinicNum:doUseCurrentClinicNum);
					response.CCSource=ccSource;
					FinishEdgeExpressUrlRequest(response);
					return response;
				}
				catch(Exception ex) {
					throw new ODException("Error creating token request.",ex);
				}
			}

			///<summary>Tells EdgeExpress to delete their saved copy of the credit card that matches the supplied token.</summary>
			public static XWebResponse UpdateAlias(long patNum,string alias,DateTime expDate) {
				//No need to check MiddleTierRole;no call to db.
				try {
					XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.AliasUpdate,_edgeExpressDirectPayUrl,false,alias:alias,expDate:expDate.ToString("MMyy"));
					FinishEdgeExpressUrlRequest(response);
					return response;
				}
				catch(Exception ex) {
					throw new ODException("Error updating card. "+ex.Message,ex);
				}
			}

			///<summary>Tells EdgeExpress to delete their saved copy of the credit card that matches the supplied token.</summary>
			public static XWebResponse DeleteAlias(long patNum,string alias) {
				//No need to check MiddleTierRole;no call to db.
				try {
					XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.AliasDelete,_edgeExpressDirectPayUrl,false,alias:alias);
					FinishEdgeExpressUrlRequest(response);
					return response;
				}
				catch(Exception ex) {
					throw new ODException("Error deleting card. "+ex.Message,ex);
				}
			}

			public static XWebResponse VoidTransaction(long patNum,string transactionID,double amount,bool isWebPayment) {
				//No need to check MiddleTierRole;no call to db.
				try {
					XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.CreditVoid,_edgeExpressDirectPayUrl,isWebPayment,amount:amount,
						doCreateAlias:false,transactionID:transactionID);
					FinishEdgeExpressUrlRequest(response);
					return response;
				}
				catch(Exception ex) {
					throw new ODException("Error creating void request. "+ex.Message,ex);
				}
			}

			public static XWebResponse ReturnTransaction(long patNum,string transactionID,double amount,bool isWebPayment) {
				//No need to check MiddleTierRole;no call to db.
				try {
					XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.CreditReturn,_edgeExpressDirectPayUrl,isWebPayment,amount:amount,
						doCreateAlias:false,transactionID:transactionID);
					FinishEdgeExpressUrlRequest(response);
					return response;
				}
				catch(Exception ex) {
					throw new ODException("Error creating return request.",ex);
				}
			}

			public static XWebResponse ForceTransaction(long patNum,string transactionID,double amount,bool isWebPayment) {
				//No need to check MiddleTierRole;no call to db.
				try {
					XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.CreditOnlineCapture,_edgeExpressDirectPayUrl,isWebPayment,amount:amount,
						doCreateAlias:false,transactionID:transactionID);
					FinishEdgeExpressUrlRequest(response);
					return response;
				}
				catch(Exception ex) {
					throw new ODException("Error creating return request.",ex);
				}
			}

			///<summary>Makes a web request to EdgeExpress to get the status for the OTK passed in.  Throws exceptions.</summary>
			public static XWebResponse GetOtkStatus(long patNum,string orderId,bool isWebPayment,long clinicNum=-1) {
				//No need to check MiddleTierRole;no call to db.
				try {
					return SendEdgeExpressRequest(patNum,EdgeExpressTransType.QueryPayment,_edgeExpressDirectPayUrl,isWebPayment,orderId:orderId,clinicNum:clinicNum);
				}
				catch(Exception ex) {
					throw new ODException("Error querying transaction information.",ex);
				}
			}

			public static string BuildReceiptString(XWebResponse response,bool doShowSignatureLine) {

				if(Enum.TryParse<EdgeExpressTransType>(response.TransactionType,out EdgeExpressTransType transType)) {
					return BuildReceiptString(transType,response.TransactionID,response.MaskedAcctNum,
						response.ApprovalCode,response.PayNote,(decimal)response.Amount,doShowSignatureLine,response.ClinicNum);
				}
				else {
					return "Error creating receipt string.";
				}
			}

			public static string BuildReceiptString(EdgeExpressTransType transType,string transactionID,string cardNumber,
				string approvalCode,string statusDescription,decimal amount,bool doShowSignatureLine,long clinicNum)
			{
				string result="";
				cardNumber=cardNumber??"";
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
				//TODO add a decline line
				result+="Transaction ID".PadRight(xright-xleft,'.')+transactionID+Environment.NewLine;
				result+="Account".PadRight(xright-xleft,'.');
				for(int i = 0;i<cardNumber.Length-4;i++) {
					result+="*";
				}
				if(cardNumber.Length>=4) {
					result+=cardNumber.Substring(cardNumber.Length-4)+Environment.NewLine;//last 4 digits of card number only.
				}
				result+="Card Type".PadRight(xright-xleft,'.')+CreditCardUtils.GetCardType(cardNumber)+Environment.NewLine;
				//result+="Entry".PadRight(xright-xleft,'.')+(String.IsNullOrEmpty(magData) ? "Manual" : "Swiped")+Environment.NewLine;
				result+="Approval Code".PadRight(xright-xleft,'.')+approvalCode+Environment.NewLine;
				result+="Result".PadRight(xright-xleft,'.')+statusDescription+Environment.NewLine;
				result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
				if(transType.In(EdgeExpressTransType.CreditReturn,EdgeExpressTransType.CreditVoid)) {
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


			///<summary>Similar to XWebs.ProcessOutstandingTransactions but only runs for a single XWebResponse. Used in the event that they don't have a running eConnector.
			///Handles the creation of CreditCards if an alias is returned.</summary>
			public static XWebResponse ProcessTransaction(XWebResponse xWebResponseOld,Payment existingPayment=null,bool doCreatePayment=true) {
				XWebResponse xWebResponseUpdate=xWebResponseOld;
				try {
					//Get the OTK status from the gateway.
					XWebResponse xWebResponseNew=GetOtkStatus(xWebResponseOld.PatNum,xWebResponseOld.OrderId,false,existingPayment.ClinicNum);
					if(xWebResponseNew.XWebResponseCode==XWebResponseCodes.Pending || xWebResponseNew.XWebResponseCode==XWebResponseCodes.Undefined) { //No new status to report. Try again next time.
						if(DateTime.Now>xWebResponseOld.HpfExpiration.AddMinutes(5)) {
							//EdgeExpress will return 814 "Invalid Reference Error" indefinitely if the patient doesn't finish the transaction.
							xWebResponseUpdate.TransactionStatus=XWebTransactionStatus.EdgeExpressExpired;
						}
						return xWebResponseUpdate;
					}
					//We got this far so we at least got a valid gateway response. This is now the response that we will update in the db.
					xWebResponseUpdate=xWebResponseNew;
					//ResponseCode came over as an int from XML. We need to convert that the XWebResponseCode enum here. Note we are using the otkOutput.ResponseCode (not the otkOld.ResponseCode).
					xWebResponseUpdate.XWebResponseCode=XWebResponse.ConvertResponseCode(xWebResponseUpdate.ResponseCode);
					//Save existing fields from the old db version to the new db version.						
					xWebResponseUpdate.SetPersistentFields(
						xWebResponseOld.XWebResponseNum,
						xWebResponseOld.TransactionType,
						xWebResponseOld.PatNum,
						xWebResponseOld.ProvNum,
						xWebResponseOld.ClinicNum,
						xWebResponseOld.Amount,
						xWebResponseOld.OTK,
						xWebResponseOld.HpfUrl,
						xWebResponseOld.HpfExpiration,
						xWebResponseOld.DebugError,
						xWebResponseOld.PayNote,
						xWebResponseOld.CCSource);
					//The credit card's ExpDate will only be valid when we actually completed a payment.
					if(!string.IsNullOrEmpty(xWebResponseUpdate.ExpDate)) {
						xWebResponseUpdate.AccountExpirationDate=XWebResponse.ConvertExpDate(xWebResponseUpdate.ExpDate);
					}
					switch(xWebResponseUpdate.XWebResponseCode) {
						case XWebResponseCodes.Approval:
							if(!string.IsNullOrEmpty(xWebResponseUpdate.Alias)) {
								//Create a new credit card with this alias.
								CreditCards.InsertFromXWeb(xWebResponseUpdate);
							}
							//If we already have a payment, just update the note and insert a paysplit.
							if(existingPayment!=null) {
								xWebResponseUpdate.PaymentNum=existingPayment.PayNum;
							}
							//Insert Payment, PaySplit, and set FK.
							else if(doCreatePayment) {
								xWebResponseUpdate.PaymentNum=Payments.InsertFromXWeb(
								//todo: create a formatted receipt to show the web user after the payment has been accepted
								xWebResponseUpdate.PatNum,xWebResponseUpdate.ProvNum,xWebResponseUpdate.ClinicNum,xWebResponseUpdate.Amount,
								xWebResponseUpdate.GetFormattedNote(true),//If we ever allow returns or voids from Patient Portal, we will need to change this argument.
								"",xWebResponseUpdate.CCSource);
							}
							xWebResponseUpdate.TransactionStatus=XWebTransactionStatus.EdgeExpressCompletePaymentApproved;
							break;
						case XWebResponseCodes.AliasSuccess:
						case XWebResponseCodes.ZeroDollarAuthApproval:
							//Create a new credit card with this alias.
							CreditCards.InsertFromXWeb(xWebResponseUpdate);
							xWebResponseUpdate.TransactionStatus=XWebTransactionStatus.EdgeExpressCompleteAliasCreated;
							break;
						case XWebResponseCodes.ExpiredWithoutApproval:
							//Nothing to do here. Just let the XWebResponseCode get updated so we don't check next time.
							xWebResponseUpdate.TransactionStatus=XWebTransactionStatus.EdgeExpressExpired;
							break;
						case XWebResponseCodes.Declined:
							break;
						case XWebResponseCodes.PartialApproval:
						case XWebResponseCodes.OtkSuccess:
						case XWebResponseCodes.Undefined:
						case XWebResponseCodes.Pending:
						default:
							throw new ODException("Unsupported XWebResponseCode for GetOtkStatus: "+xWebResponseUpdate.XWebResponseCode.ToString());
					}
				}
				catch(Exception e) {
					if( //Had been pending so only officially stop polling if it's been plenty of time past when the OTK would expire. 
						//There may be server hickups or other unforeseen one-time errors so keep polling until it is officially past time when we can possibly get a valid response.
						xWebResponseOld.TransactionStatus==XWebTransactionStatus.EdgeExpressPending &&
						//It is well past when the HPF would have expired.
						DateTime.Now>xWebResponseOld.HpfExpiration.AddMinutes(5))
					{
						//It would be very rare to meet both of these conditions but just in case, let's stop monitoring.
						xWebResponseUpdate.TransactionStatus=XWebTransactionStatus.EdgeExpressMonitoringError;
					}
					//Hold this error for troubleshooting later.
					xWebResponseUpdate.DebugError=e.Message;
				}
				finally { //Always update, even if there were no changes. This is to update the value of DateTUpdate.
					xWebResponseUpdate.DateTUpdate=DateTime.Now;
					XWebResponses.Update(xWebResponseUpdate);
				}
				return xWebResponseUpdate;
			}
		}
		#endregion Card Not Present API
	}

	public enum EdgeExpressApiType {
		Terminal,
		Web,
	}

	///<summary>The type of transaction for the EdgeExpress API.</summary>
	public enum EdgeExpressTransType {
		[Description("Purchase")]
		CreditSale,
		[Description("Return")]
		CreditReturn,
		///<summary>Queries that status of a payment using its OTK (session token).</summary>
		QueryPayment,
		[Description("Debit Purchase")]
		DebitSale,
		[Description("Debit Return")]
		DebitReturn,
		[Description("Force")]
		CreditOnlineCapture,
		[Description("Pre-Authorization")]
		CreditAuth,
		[Description("Void")]
		CreditVoid,
		AliasCreate,
		AliasUpdate,
		AliasDelete,
	}

}
