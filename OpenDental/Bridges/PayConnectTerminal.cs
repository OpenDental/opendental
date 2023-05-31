using System;
using CodeBase;
using DentalXChange.Dps.Pos;
using OpenDentBusiness;
using OpenDentBusiness.PayConnectService;

namespace OpenDental.Bridges {
	///<summary>Methods that can be used when interacting with a PayConnect terminal.</summary>
	public class PayConnectTerminal {

		///<summary>Builds a receipt string for a terminal transaction.</summary>
		public static string BuildReceiptString(PayConnectResponse pcResponse,bool wasElectronicallySigned,long clinicNum) {
			string result="";
			int xleft=0;
			int xright=15;
			int xmax=37;
			result+=Environment.NewLine;
			result+=CreditCardUtils.AddClinicToReceipt(clinicNum);
			//Print body
			result+="Date".PadRight(xright-xleft,'.')+DateTime.Now.ToString()+Environment.NewLine;
			result+=Environment.NewLine;
			result+=AddReceiptField("Trans Type",pcResponse.TransType.ToString());
			result+=Environment.NewLine;
			result+=AddReceiptField("Transaction #",pcResponse.RefNumber);
			result+=AddReceiptField("Account",pcResponse.CardNumber);
			result+=AddReceiptField("Card Type",pcResponse.CardType);
			result+=AddReceiptField("Entry",pcResponse.EntryMode);
			result+=AddReceiptField("Auth Code",pcResponse.AuthCode);
			result+=AddReceiptField("Result",pcResponse.Description);
			result+=AddReceiptField("MerchantId",pcResponse.MerchantId);
			result+=AddReceiptField("TerminalId",pcResponse.TerminalId);
			result+=AddReceiptField("Mode",pcResponse.Mode);
			result+=AddReceiptField("CardVerifyMthd",pcResponse.CardVerificationMethod);
			if(pcResponse.EMV!=null && !string.IsNullOrEmpty(pcResponse.EMV.AppId)) {
				result+=AddReceiptField("EMV AppId",pcResponse.EMV.AppId);
			}
			if(pcResponse.EMV!=null && !string.IsNullOrEmpty(pcResponse.EMV.TermVerifResults)) {
				result+=AddReceiptField("EMV TermResult",pcResponse.EMV.TermVerifResults);
			}
			if(pcResponse.EMV!=null && !string.IsNullOrEmpty(pcResponse.EMV.IssuerAppData)) {
				result+=AddReceiptField("EMV IssuerData",pcResponse.EMV.IssuerAppData);
			}
			if(pcResponse.EMV!=null && !string.IsNullOrEmpty(pcResponse.EMV.TransStatusInfo)) {
				result+=AddReceiptField("EMV TransInfo",pcResponse.EMV.TransStatusInfo);
			}
			if(pcResponse.EMV!=null && !string.IsNullOrEmpty(pcResponse.EMV.AuthResponseCode)) {
				result+=AddReceiptField("EMV AuthResp",pcResponse.EMV.AuthResponseCode);
			}
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
			Program program=Programs.GetCur(ProgramName.PayConnect);
			string integrationType=ProgramProperties.GetPropVal(program.ProgramNum,"PayConnect2.0 Integration Type: 0 for normal, 1 for surcharge",clinicNum);
			if(integrationType=="1") {//Surcharge integration
				result+=AddReceiptField("Surcharge Fee",pcResponse.AmountSurcharged.ToString("F2"));
			}
			if(pcResponse.TransType.In(PayConnectResponse.TransactionType.Refund,PayConnectResponse.TransactionType.Void)) {
				result+="Trans. Amt".PadRight(xright-xleft,'.')+(pcResponse.Amount*-1)+Environment.NewLine;
			}
			else {
				result+="Trans. Amt".PadRight(xright-xleft,'.')+pcResponse.Amount.ToString("F2")+Environment.NewLine;
			}
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
			result+="I agree to pay the above total amount according to my card issuer/bank agreement."+Environment.NewLine;
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine;
			if(wasElectronicallySigned) {
				result+="Electronically signed";
			}
			else {
				result+="Signature X".PadRight(xmax-xleft,'_');
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
				pcResponse.Amount=response.Amount;
				pcResponse.OriginalAmount=response.OriginalAmount;
				pcResponse.EntryMode=response.EntryMode;
				pcResponse.CardNumber=response.CardNumber;
				pcResponse.MerchantId=response.MerchantId;
				pcResponse.TerminalId=response.TerminalId;
				pcResponse.Mode=response.Mode;
				pcResponse.CardVerificationMethod=response.CardVerificationMethod;
				pcResponse.TransType=(PayConnectResponse.TransactionType)response.TransactionType;
				pcResponse.EMV=new PayConnectResponse.EmvData();
				pcResponse.EMV.AppId=response.EMV.AppId;
				pcResponse.EMV.TermVerifResults=response.EMV.TermVerifResults;
				pcResponse.EMV.IssuerAppData=response.EMV.IssuerAppData;
				pcResponse.EMV.TransStatusInfo=response.EMV.TransStatusInfo;
				pcResponse.EMV.AuthResponseCode=response.EMV.AuthResponseCode;
			}
			return pcResponse;
		}
	}
}
