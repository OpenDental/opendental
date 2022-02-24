using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		///<summary>The URL to send the request to the RCM program running on the local machine. localsystem.paygateway.com resolves to 127.0.0.1.
		///</summary>
		private const string _edgeExpressRCMURL="https://localsystem.paygateway.com:21113/RcmService.svc/Initialize";
		///<summary>Url to setup a payment page and get a OTK.</summary>
		private static string _edgeExpressHostPayUrl {
			get {
				if(ODBuild.IsDebug() || XWebs.UseXWebTestGateway) {
					return "https://ee.test.paygateway.com/HostPayService/v1/hostpay/transactions";
				}
				return "https://ee.paygateway.com/HostPayService/v1/hostpay/transactions/";
			}
		}

		///<summary>Url for transactions that do not require a payment page.</summary>
		private static string _edgeExpressDirectPayUrl {
			get {
				if(ODBuild.IsDebug() || XWebs.UseXWebTestGateway) {
					return "https://ee.test.paygateway.com/HostPayService/v1/directpay/express";
				}
				return "https://ee.paygateway.com/HostPayService/v1/directpay/express";
			}
		}

		///<summary>The amount of time that the eConnector will check hosted pay URLs.</summary>
		private static TimeSpan _formTimeout=TimeSpan.FromMinutes(10);

		///<summary>Creates and returns the EdgeExpress URL and validation OTK which can be used to make a payment for an unspecified credit card.
		///</summary>
		public static XWebResponse GetEdgeExpressUrlForPayment(long patNum,string payNote,double amount,bool createAlias,CreditCardSource ccSource,bool isWebPayment) {
			//No need to check RemotingRole;no call to db.
			if(ccSource!=CreditCardSource.XWeb && ccSource!=CreditCardSource.XWebPortalLogin) {
				throw new ODException("Invalid CreditCardSource: "+ccSource.ToString(),ODException.ErrorCodes.OtkArgsInvalid);
			}
			//Validate the amount.
			if(amount<0.00 || amount>99999.99) {
				throw new ODException("Invalid Amount",ODException.ErrorCodes.OtkArgsInvalid);
			}
			if(string.IsNullOrEmpty(payNote)) {
				throw new ODException("Invalid PayNote",ODException.ErrorCodes.OtkArgsInvalid);
			}
			XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.CreditSale,_edgeExpressHostPayUrl,isWebPayment,amount,
				doCreateAlias:createAlias);
			response.Amount=amount;
			response.PayNote=payNote;
			response.CCSource=ccSource;
			FinishEdgeExpressUrlRequest(response);
			return response;
		}

		///<summary>Creates and returns the EdgeExpress URL and validation OTK which can be used to create a credit card alias.</summary>
		public static XWebResponse GetEdgeExpressUrlForCreditCardAlias(long patNum,bool isWebPayment) {
			//No need to check RemotingRole;no call to db.
			XWebResponse response=SendEdgeExpressRequest(patNum,EdgeExpressTransType.CreditAuth,_edgeExpressHostPayUrl,isWebPayment,amount:0,doCreateAlias:true);
			FinishEdgeExpressUrlRequest(response);
			return response;
		}

		///<summary>Makes a web request to X-Charge to get the status for the OTK passed in.  Throws exceptions.</summary>
		public static XWebResponse GetOtkStatus(long patNum,string orderId,bool isWebPayment) {
			//No need to check RemotingRole;no call to db.
			return SendEdgeExpressRequest(patNum,EdgeExpressTransType.QueryPayment,_edgeExpressDirectPayUrl,isWebPayment,orderId: orderId);
		}

		///<summary>Creates a credit card alias.</summary>
		public static RcmResponse CreateAlias(long patNum,long clinicNum,bool isWebPayment) {
			return SendEdgeExpressCloudRequest(patNum,clinicNum,EdgeExpressTransType.AliasCreate,isWebPayment,doCreateToken:true);
		}

		///<summary>Updates the expiration date for the alias.</summary>
		public static RcmResponse UpdateAlias(long patNum,long clinicNum,string aliasToken,DateTime expDate,bool isWebPayment) {
			return SendEdgeExpressCloudRequest(patNum,clinicNum,EdgeExpressTransType.AliasUpdate,isWebPayment,aliasToken:aliasToken,expDate:expDate.ToString("MMyy"));
		}

		///<summary>Deletes the alias.</summary>
		public static RcmResponse DeleteAlias(long patNum,long clinicNum,string aliasToken,bool isWebPayment) {
			return SendEdgeExpressCloudRequest(patNum,clinicNum,EdgeExpressTransType.AliasDelete,isWebPayment,aliasToken:aliasToken);
		}

		///<summary>For credit only, not debit. Voids the transaction.</summary>
		public static RcmResponse VoidTransaction(long patNum,long clinicNum,string transactionId,bool isWebPayment) {
			return SendEdgeExpressCloudRequest(patNum,clinicNum,EdgeExpressTransType.CreditVoid,isWebPayment,transactionId:transactionId);
		}

		///<summary>Sends the request to EdgeExpress RCM and returns the response.</summary>
		public static RcmResponse SendEdgeExpressCloudRequest(long patNum,long clinicNum,EdgeExpressTransType edgeExpressTransactionType,bool isWebPayment,double amount=0,
			bool doPromptForSignature=false,bool doCreateToken=false,string aliasToken="",string transactionId="",decimal cashBackAmt=0,string expDate="") 
		{
			RcmResponse rcmResponse=null;
			StringBuilder strBldXml=new StringBuilder();
			XmlWriter xmlWriter=XmlWriter.Create(strBldXml);
			WriteEdgeExpressBaseRequest(clinicNum,edgeExpressTransactionType,xmlWriter,isWebPayment);
			AddOtherParamsEdgeExpressCloud(xmlWriter,edgeExpressTransactionType,patNum,amount,doPromptForSignature,doCreateToken,aliasToken,transactionId,
				cashBackAmt,expDate);
			string url=$"{_edgeExpressRCMURL}?xl2Parameters={strBldXml}";
			string response;
			if(ODBuild.IsWeb()) {
				//Timeout is 120 seconds because that's how long we set the timeout for PayConnect terminal.
				response=ODCloudClient.DownloadString(url,timeoutSecs:120,doShowProgressBar:false);
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

		#region Private methods
		///<summary>Sends a web request to the XWeb EdgeExpress API.</summary>
		private static XWebResponse SendEdgeExpressRequest(long patNum,EdgeExpressTransType edgeExpressTransactionType,string url,bool isWebPayment,double amount=0,
			string orderId="",bool doCreateAlias=false) 
		{
			Patient pat=Patients.GetPat(patNum);
			if(pat==null) {
				throw new ODException("Patient not found for PatNum: "+patNum.ToString(),ODException.ErrorCodes.XWebProgramProperties);
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=pat.ClinicNum;
			}
			StringBuilder strBldXml=new StringBuilder();
			using(XmlWriter xmlWriter=XmlWriter.Create(strBldXml)) {
				WriteEdgeExpressBaseRequest(clinicNum,edgeExpressTransactionType,xmlWriter,isWebPayment);
				orderId=string.IsNullOrEmpty(orderId) ? XWebResponses.CreateOrderId() : orderId;
				xmlWriter.WriteElementString("ORDERID",orderId);
				AddOtherParamsEdgeExpressCartNotPresent(xmlWriter,edgeExpressTransactionType,doCreateAlias,amount,pat);
				xmlWriter.WriteEndElement();//REQUEST
			}
			string result=XWebs.XWebInputAbs.UploadData(strBldXml.ToString(),url);
			XWebResponse xResponse=CreateEdgeExpressXWebResponse(result,edgeExpressTransactionType);
			xResponse.OrderId=orderId;
			xResponse.PatNum=patNum;
			xResponse.ProvNum=pat.PriProv;
			xResponse.ClinicNum=clinicNum;
			xResponse.DateTUpdate=DateTime.Now;
			xResponse.TransactionType=edgeExpressTransactionType.ToString();
			XWebs.OnWakeupMonitor(xResponse,new EventArgs());
			return xResponse;
		}

		///<summary>Adds the base params for an EdgeExpress request.</summary>
		private static void WriteEdgeExpressBaseRequest(long clinicNum,EdgeExpressTransType edgeExpressTransactionType,XmlWriter xmlWriter,bool isWebPayment = true) {
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
		}

		///<summary>Adds additional parameters to the specified transaction type. For EdgeExpress Card Not Present only.</summary>
		private static void AddOtherParamsEdgeExpressCartNotPresent(XmlWriter xmlWriter,EdgeExpressTransType edgeExpressTransactionType,bool doCreateAlias,
			double amount,Patient pat) {
			if(ListTools.In(edgeExpressTransactionType,EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditAuth)) {
				xmlWriter.WriteElementString("AMOUNT",amount.ToString());
				xmlWriter.WriteElementString("CUSTOMERNAME",pat.GetNameFLnoPref());
				if(doCreateAlias) {
					xmlWriter.WriteElementString("CREATEALIAS","true");
				}
				xmlWriter.WriteStartElement("HOSTPAYSETTING");
				xmlWriter.WriteStartElement("POSDEVICE");
				xmlWriter.WriteElementString("TYPE","KEYED");
				xmlWriter.WriteEndElement();//POSDEVICE
				xmlWriter.WriteStartElement("RETURNOPTION");
				if(ODBuild.IsDebug()) {
					xmlWriter.WriteElementString("RETURNURL","http://localhost/OpenDentalWebLander/PortalPayDone.aspx");
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
		}

		///<summary>Adds additional parameters to the specified transaction type. For EdgeExpress Cloud only.</summary>
		private static void AddOtherParamsEdgeExpressCloud(XmlWriter xmlWriter,EdgeExpressTransType edgeExpressTransactionType,long patNum,
			double amount,bool doPromptForSignature,bool doCreateToken,string aliasToken,string transactionId,decimal cashBackAmt,string expDate) 
		{
			if(ListTools.In(edgeExpressTransactionType,EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditReturn,EdgeExpressTransType.CreditAuth,
				EdgeExpressTransType.CreditOnlineCapture,EdgeExpressTransType.DebitSale,EdgeExpressTransType.DebitReturn)) 
			{
				xmlWriter.WriteElementString("AMOUNT",amount.ToString());
			}
			if(ListTools.In(edgeExpressTransactionType,EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditReturn,EdgeExpressTransType.CreditAuth)) {
				xmlWriter.WriteElementString("RECEIPTLINEITEMS","Pat"+patNum);
				xmlWriter.WriteElementString("PROMPTSIGNATURE",doPromptForSignature ? "True" : "False");
				xmlWriter.WriteElementString("CREATEALIAS",doCreateToken ? "TRUE" : "FALSE");
				if(!string.IsNullOrEmpty(aliasToken)) {
					xmlWriter.WriteElementString("ALIAS",aliasToken);
				}
			}
			if(ListTools.In(edgeExpressTransactionType,EdgeExpressTransType.AliasUpdate,EdgeExpressTransType.AliasDelete)) {
				xmlWriter.WriteElementString("ALIAS",aliasToken);
			}
			if(ListTools.In(edgeExpressTransactionType,EdgeExpressTransType.AliasUpdate)) {
				xmlWriter.WriteElementString("EXPDATE",expDate);
			}
			if(ListTools.In(edgeExpressTransactionType,EdgeExpressTransType.CreditVoid,EdgeExpressTransType.CreditReturn,EdgeExpressTransType.CreditOnlineCapture)) {
				xmlWriter.WriteElementString("TRANSACTIONID",transactionId);
			}
			if(ListTools.In(edgeExpressTransactionType,EdgeExpressTransType.DebitSale)) {
				xmlWriter.WriteElementString("CASHBACKAMOUNT",cashBackAmt.ToString());
			}
			xmlWriter.WriteEndElement();//REQUEST
			xmlWriter.Flush();
		}

		///<summary>Converts the XML string result from the EdgeExpress API to an XWebResponse.</summary>
		private static XWebResponse CreateEdgeExpressXWebResponse(string result,EdgeExpressTransType edgeExpressTransactionType) {
			XWebResponse xResponse=new XWebResponse();
			if(ListTools.In(edgeExpressTransactionType,EdgeExpressTransType.CreditSale,EdgeExpressTransType.CreditAuth)) {
				xResponse.OTK=WebSerializer.DeserializeNode(result,"SESSIONTOKEN");
				xResponse.HpfUrl=WebSerializer.DeserializeNode(result,"PAYPAGEURL");
				xResponse.TransactionStatus=XWebTransactionStatus.EdgeExpressPending;
			}
			if(edgeExpressTransactionType==EdgeExpressTransType.QueryPayment) {
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
			return xResponse;
		}

		///<summary>Converts the XML string result from the EdgeExpress API for a QueryPayment to an XWebResponse.</summary>
		private static XWebResponse ConvertEdgeExpressResponse(string result) {
			EdgeExpressResponse eeResponse;
			using(StringReader sr=new StringReader(result)) {
				//XWeb's xml references this class as RESULT but OD wants to deserialize to a class called EdgeExpressResponse. 
				//We must explicitly specific the XmlRoot node name here to make that conversion.	
				eeResponse=(EdgeExpressResponse)new XmlSerializer(typeof(EdgeExpressResponse),new XmlRootAttribute("RESULT")).Deserialize(sr);
			}
			XWebResponse xResponse=new XWebResponse {
				AccountExpirationDate=new DateTime(2000+eeResponse.EXPYEAR,eeResponse.EXPMONTH,1),
				Alias=eeResponse.ALIAS,
				Amount=(double)eeResponse.APPROVEDAMOUNT,
				ApprovalCode=eeResponse.APPROVALCODE,
				BatchNum=eeResponse.BATCHNO,
				CardBrand=eeResponse.CARDBRAND,
				CardCodeResponse=eeResponse.CARDCODERESPONSE,
				CardType=eeResponse.CARDTYPE,
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

		///<summary>Inserts the response to the db and wakes up the monitor thread.</summary>
		private static void FinishEdgeExpressUrlRequest(XWebResponse response) {
			response.HpfExpiration=DateTime.Now.Add(_formTimeout);
			XWebResponses.Insert(response);
			XWebs.OnWakeupMonitor(response,new EventArgs());
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
					if(ListTools.In(fi.Name,nameof(RECEIPTTEXT),nameof(ALIAS),nameof(TransType))
						|| (ListTools.In(fi.Name,nameof(RESPONSE),nameof(RESULT)) && fi.GetValue(this).ToString()=="-1")) 
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
