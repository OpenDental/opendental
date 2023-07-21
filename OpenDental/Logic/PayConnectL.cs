using System;
using System.Collections.Generic;
using System.Drawing;
using CodeBase;
using OpenDentBusiness;
using MigraDoc.DocumentObjectModel;
using PayConnectService = OpenDentBusiness.PayConnectService;


namespace OpenDental {
	class PayConnectL {
		/// <summary>Only used to void or refund transactions from PayConnectPortal. Creates new cloned payment and paysplits for the refund or void.
		/// Returns true if the transaction was successful, otherwise false.</summary
		public static bool VoidOrRefundPayConnectPortalTransaction(PayConnectResponseWeb payConnectResponseWeb,Payment payment,PayConnectService.transType transType,string strRefNum,decimal amount) {
			if(!transType.In(PayConnectService.transType.RETURN,PayConnectService.transType.VOID)) {
				MsgBox.Show("PayConnectL","Invalid transaction type. Please contact support for assistance.");
				return false;
			}
			List<PaySplit> listPaySplits=PaySplits.GetForPayment(payment.PayNum);
			PayConnectService.creditCardRequest creditCardRequest=new PayConnectService.creditCardRequest();
			PayConnectResponse payConnectResponse=null;
			string receiptStr="";
			CreditCard creditCard=CreditCards.GetOneWithPayConenctToken(payConnectResponseWeb.PaymentToken);
			if(creditCard==null) {
				MsgBox.Show("PayConnectL","Patient was not logged in for this payment, you must go through your payment merchant's portal to process this request.");
				return false;
			}
			creditCardRequest=PayConnect.BuildSaleRequest(amount,creditCard.PayConnectToken,creditCard.PayConnectTokenExp.Year,
				creditCard.PayConnectTokenExp.Month,"","","","",transType,strRefNum,false);
			PayConnectService.transResponse transResponse=PayConnect.ProcessCreditCard(creditCardRequest,payment.ClinicNum,x => MsgBox.Show(x));
			payConnectResponse=PayConnectREST.ToPayConnectResponse(transResponse,creditCardRequest);
			receiptStr=PayConnect.BuildReceiptString(creditCardRequest,transResponse,null,payment.ClinicNum);
			if(payConnectResponse==null || payConnectResponse.StatusCode!="0") {//error in transaction
				if(payConnectResponse==null) {
					MsgBox.Show("PayConnectL","An unexpected error occurred when attempting to process this transaction. Please try again.");
				}
				else {
					MsgBox.Show(payConnectResponse.Description+". Error Code: "+payConnectResponse.StatusCode);
				}
				return false;
			}
			//Record a new payment for the voided transaction
			string payNote=Lan.g("PayConnectL","Transaction Type")+": "+Enum.GetName(typeof(PayConnectService.transType),transType)
				+Environment.NewLine+Lan.g("PayConnectL","Status")+": "+payConnectResponse.Description+Environment.NewLine
				+Lan.g("PayConnectL","Amount")+": "+amount.ToString("C")+Environment.NewLine
				+Lan.g("PayConnectL","Auth Code")+": "+payConnectResponse.AuthCode+Environment.NewLine
				+Lan.g("PayConnectL","Ref Number")+": "+payConnectResponse.RefNumber;
			Payment paymentClone=Payments.InsertVoidPayment(payment,listPaySplits,receiptStr,payNote,payConnectResponseWeb.CCSource,payAmt:(double)amount);
			paymentClone.PayDate=DateTime.Now;
			PayConnectResponseWeb payConnectResponseWebNew=new PayConnectResponseWeb();
			payConnectResponseWebNew.PatNum=payment.PatNum;
			payConnectResponseWebNew.PayNum=paymentClone.PayNum;
			payConnectResponseWebNew.CCSource=payConnectResponseWeb.CCSource;
			payConnectResponseWebNew.Amount=paymentClone.PayAmt;
			payConnectResponseWebNew.PayNote=Lan.g("PayConnectL",paymentClone.PayNote+Environment.NewLine+"From within Open Dental Proper.");
			payConnectResponseWebNew.ProcessingStatus=PayConnectWebStatus.Completed;
			payConnectResponseWebNew.DateTimeEntry=DateTime.Now;
			payConnectResponseWebNew.DateTimeCompleted=DateTime.Now;
			payConnectResponseWebNew.IsTokenSaved=false;
			payConnectResponseWebNew.RefNumber=transResponse.RefNumber;
			payConnectResponseWebNew.TransType=transType;
			payConnectResponseWebNew.PaymentToken=payConnectResponseWeb.PaymentToken;
			PayConnectResponseWebs.Insert(payConnectResponseWebNew);
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,paymentClone.PatNum,
				Patients.GetLim(paymentClone.PatNum).GetNameLF()+", "+paymentClone.PayAmt.ToString("c"));
			return true;
		}
	}
}
