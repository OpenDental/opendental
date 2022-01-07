using System;
using CodeBase;
using DentalXChange.Dps.Pos;
using OpenDentBusiness;
using OpenDentBusiness.PayConnectService;

namespace OpenDental.Bridges {
	///<summary>Methods that can be used when interacting with a PayConnect terminal.</summary>
	public class PayConnectTerminal {

		///<summary>Builds a receipt string for a terminal transaction.</summary>
		public static string BuildReceiptString(PosRequest posRequest,PosResponse posResponse,signatureResponse sigResponse,long clinicNum) {
			string result="";
			int xleft=0;
			int xright=15;
			int xmax=37;
			result+=Environment.NewLine;
			result+=CreditCardUtils.AddClinicToReceipt(clinicNum);
			//Print body
			result+="Date".PadRight(xright-xleft,'.')+DateTime.Now.ToString()+Environment.NewLine;
			result+=Environment.NewLine;
			result+=AddReceiptField("Trans Type",posResponse.TransactionType.ToString());
			result+=Environment.NewLine;
			result+=AddReceiptField("Transaction #",posResponse.ReferenceNumber.ToString());
			result+=AddReceiptField("Account",posResponse.CardNumber);
			result+=AddReceiptField("Card Type",posResponse.CardBrand);
			result+=AddReceiptField("Entry",posResponse.EntryMode);
			result+=AddReceiptField("Auth Code",posResponse.AuthCode);
			result+=AddReceiptField("Result",posResponse.ResponseDescription);
			result+=AddReceiptField("MerchantId",posResponse.MerchantId);
			result+=AddReceiptField("TerminalId",posResponse.TerminalId);
			result+=AddReceiptField("Mode",posResponse.Mode);
			result+=AddReceiptField("CardVerifyMthd",posResponse.CardVerificationMethod);
			if(posResponse.EMV!=null && !string.IsNullOrEmpty(posResponse.EMV.AppId)) {
				result+=AddReceiptField("EMV AppId",posResponse.EMV.AppId);
			}
			if(posResponse.EMV!=null && !string.IsNullOrEmpty(posResponse.EMV.TermVerifResults)) {
				result+=AddReceiptField("EMV TermResult",posResponse.EMV.TermVerifResults);
			}
			if(posResponse.EMV!=null && !string.IsNullOrEmpty(posResponse.EMV.IssuerAppData)) {
				result+=AddReceiptField("EMV IssuerData",posResponse.EMV.IssuerAppData);
			}
			if(posResponse.EMV!=null && !string.IsNullOrEmpty(posResponse.EMV.TransStatusInfo)) {
				result+=AddReceiptField("EMV TransInfo",posResponse.EMV.TransStatusInfo);
			}
			if(posResponse.EMV!=null && !string.IsNullOrEmpty(posResponse.EMV.AuthResponseCode)) {
				result+=AddReceiptField("EMV AuthResp",posResponse.EMV.AuthResponseCode);
			}
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
			if(ListTools.In(posResponse.TransactionType,TransactionType.Refund,TransactionType.Void)) {
				result+="Total Amt".PadRight(xright-xleft,'.')+(posResponse.Amount*-1)+Environment.NewLine;
			}
			else {
				result+="Total Amt".PadRight(xright-xleft,'.')+posResponse.Amount+Environment.NewLine;
			}
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
			result+="I agree to pay the above total amount according to my card issuer/bank agreement."+Environment.NewLine;
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine;
			if(sigResponse==null || sigResponse.Status==null || sigResponse.Status.code!=0) {
				result+="Signature X".PadRight(xmax-xleft,'_');
			}
			else {
				result+="Electronically signed";
			}
			return result;
		}


		///<summary>Returns the field name and value formatted to be added to a receipt string. The fieldName should be less than 15 characters.</summary>
		private static string AddReceiptField(string fieldName,string fieldValue) {
			int xleft=0;
			int xright=15;
			int xmax=37;
			string retStr="";
			fieldValue=fieldValue??"";
			retStr+=fieldName.PadRight(xright-xleft,'.');
			if(fieldValue.Length<xmax-xright) {//Short enough to fit on one line
				retStr+=fieldValue+Environment.NewLine;
			}
			else {//Put the field value on two lines
				retStr+=fieldValue.Substring(0,xmax-xright-1)+Environment.NewLine;
				retStr+="".PadRight(xright,'.')+fieldValue.Substring(xmax-xright-1)+Environment.NewLine;
			}
			return retStr;
		}

		///<summary>Turns a PosResponse into an OpenDentBusiness.PayConnectResponse.</summary>
		public static PayConnectResponse ToPayConnectResponse(PosResponse response) {
			PayConnectResponse pcResponse=new PayConnectResponse();
			if(response != null) {
				pcResponse.AuthCode=response.AuthCode;
				pcResponse.RefNumber=response.ReferenceNumber.ToString();
				pcResponse.Description=response.ResponseDescription;
				pcResponse.StatusCode=response.ResponseCode;
				pcResponse.CardType=response.CardBrand;
			}
			return pcResponse;
		}
	}
}
