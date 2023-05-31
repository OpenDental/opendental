extern alias payconnect2DLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using payconnect2DLL::DentalXChange.Dps.Pos;//DXC named the DLLs and their namespaces for versions 1 & 2 the same but we need to support both. We get around naming issues by assigning an alias to the new DLL (payconnect2).

namespace OpenDental {
	///<summary>All code that interacts with DPSPOSv2.DLL should be contained within this class so we don't need to use aliased namespaces in more than one place.</summary>
	class PayConnect2L {

		///<summary>Makes a call to DPSPOSv2.DLL to create a PosRequest for an authorization transaction</summary>
		public static PayConnectResponse CreateAuth(decimal amount,string apiKey,string terminalId,string invoiceNumber = "",bool forceDuplicate=false) {
			PosRequest auth;
			if(invoiceNumber.IsNullOrEmpty()) {
				auth=PosRequest.CreateAuth(amount);
			}
			else {
				auth=PosRequest.CreateAuth(amount,invoiceNumber);
			}
			auth.ForceDuplicate=forceDuplicate;
			PosResponse posResponse=ProcessCreditCard(auth,apiKey,terminalId);
			return PosResponseToPayConnectResponse(posResponse);
		}

		///<summary>Makes a call to DPSPOSv2.DLL to create a PosRequest for a sale transaction</summary>
		public static PayConnectResponse CreateSale(decimal amount,string apiKey,string terminalId,string invoiceNumber = "",bool forceDuplicate=false) {
			PosRequest sale;
			if(invoiceNumber.IsNullOrEmpty()) {
				sale=PosRequest.CreateSale(amount);
			}
			else {
				sale=PosRequest.CreateSale(amount,invoiceNumber);
			}
			sale.ForceDuplicate=forceDuplicate;
			PosResponse posResponse=ProcessCreditCard(sale,apiKey,terminalId);
			return PosResponseToPayConnectResponse(posResponse);
		}

		///<summary>Makes a call to DPSPOSv2.DLL to create a PosRequest for a refund transaction. Refunds are generally used for reversing a payment that happened over 24 hours ago or for refunding a specific amount but we leave it to the user to decide between a refund and a void.</summary>
		public static PayConnectResponse CreateRefund(decimal amount,string apiKey,string terminalId,string referenceNumber,bool forceDuplicate=false) {
			PosRequest refund=PosRequest.CreateRefund(amount,referenceNumber);
			refund.ForceDuplicate=forceDuplicate;
			PosResponse posResponse=ProcessCreditCard(refund,apiKey,terminalId);
			return PosResponseToPayConnectResponse(posResponse);
		}

		///<summary>Makes a call to DPSPOSv2.DLL to create a PosRequest for a Void transaction. Voids are generally used for undoing a payment that happened in the last 24 hours but we leave it to the user to decide between a refund and a void. </summary>
		public static PayConnectResponse CreateVoid(string referenceNumber,string apiKey,string terminalId,bool forceDuplicate=false) {
			PosRequest voidRequest=PosRequest.CreateVoidByReference(referenceNumber);
			voidRequest.ForceDuplicate=forceDuplicate;
			PosResponse posResponse=ProcessCreditCard(voidRequest,apiKey,terminalId);
			return PosResponseToPayConnectResponse(posResponse);
		}

		///<summary>Sends the constructed posRequest to PayConnect who then activates the credit card terminal associated to the passed in ID to process the transaction.</summary>
		public static PosResponse ProcessCreditCard(PosRequest posRequest,string apiKey,string terminalID) {
			return DpsPos.ProcessCreditCard(posRequest,apiKey,terminalID);
		}

		///<summary>Converts the DLL response to a response class in our codebase. This is so no other files need to reference the DPSVOSv2.DLL.</summary>
		public static PayConnectResponse PosResponseToPayConnectResponse(PosResponse response) {
			PayConnectResponse payConnectResponse=new PayConnectResponse();
			payConnectResponse.AuthCode=response.AuthCode;
			payConnectResponse.CardType=response.CardBrand;
			payConnectResponse.CardNumber=response.CardNumber;
			payConnectResponse.CardVerificationMethod=response.CardVerificationMethod;
			payConnectResponse.EMV=new PayConnectResponse.EmvData();
			payConnectResponse.EMV.AppId=response.EMV.AppId;
			payConnectResponse.EMV.TermVerifResults=response.EMV.TermVerifResults;
			payConnectResponse.EMV.IssuerAppData=response.EMV.IssuerAppData;
			payConnectResponse.EMV.TransStatusInfo=response.EMV.TransStatusInfo;
			payConnectResponse.EMV.AuthResponseCode=response.EMV.AuthResponseCode;
			payConnectResponse.EntryMode=response.EntryMode;
			payConnectResponse.MerchantId=response.MerchantId;
			payConnectResponse.Mode=response.Mode;
			payConnectResponse.Amount=response.Amount;
			payConnectResponse.OriginalAmount=response.OriginalAmount;
			payConnectResponse.RefNumber=response.ReferenceNumber;
			payConnectResponse.StatusCode=response.ResponseCode;
			payConnectResponse.Description=response.ResponseDescription;
			payConnectResponse.TerminalId=response.TerminalId;
			return payConnectResponse;
		}
	}
}
