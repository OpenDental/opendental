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
		public static bool VoidOrRefundPayConnectPortalTransaction(PayConnectResponseWeb pcResponseWeb,Payment payment,PayConnectService.transType transType,string refNum,decimal amount) {
			if(!transType.In(PayConnectService.transType.RETURN,PayConnectService.transType.VOID)) {
				MsgBox.Show("PayConnectL","Invalid transaction type. Please contact support for assistance.");
				return false;
			}
			List<PaySplit> listPaySplits=PaySplits.GetForPayment(payment.PayNum);
			PayConnectService.creditCardRequest _payConnectRequest=new PayConnectService.creditCardRequest();
			PayConnectResponse response=null;
			string receiptStr="";
			CreditCard creditCard=CreditCards.GetOneWithPayConenctToken(pcResponseWeb.PaymentToken);
			if(creditCard==null) {
				MsgBox.Show("PayConnectL","Patient was not logged in for this payment, you must go through your payment merchant's portal to process this request.");
				return false;
			}
			_payConnectRequest=PayConnect.BuildSaleRequest(amount,creditCard.PayConnectToken,creditCard.PayConnectTokenExp.Year,
				creditCard.PayConnectTokenExp.Month,"","","","",transType,refNum,false);
			PayConnectService.transResponse transResponse=PayConnect.ProcessCreditCard(_payConnectRequest,payment.ClinicNum,x => MsgBox.Show(x));
			response=PayConnectREST.ToPayConnectResponse(transResponse,_payConnectRequest);
			receiptStr=PayConnect.BuildReceiptString(_payConnectRequest,transResponse,null,payment.ClinicNum);
			if(response==null || response.StatusCode!="0") {//error in transaction
				if(response==null) {
					MsgBox.Show("PayConnectL","An unexpected error occurred when attempting to process this transaction. Please try again.");
				}
				else {
					MsgBox.Show(response.Description+". Error Code: "+response.StatusCode);
				}
				return false;
			}
			//Record a new payment for the voided transaction
			string payNote=Lan.g("PayConnectL","Transaction Type")+": "+Enum.GetName(typeof(PayConnectService.transType),transType)
				+Environment.NewLine+Lan.g("PayConnectL","Status")+": "+response.Description+Environment.NewLine
				+Lan.g("PayConnectL","Amount")+": "+amount.ToString("C")+Environment.NewLine
				+Lan.g("PayConnectL","Auth Code")+": "+response.AuthCode+Environment.NewLine
				+Lan.g("PayConnectL","Ref Number")+": "+response.RefNumber;
			Payment clonePayment=Payments.InsertVoidPayment(payment,listPaySplits,receiptStr,payNote,pcResponseWeb.CCSource,payAmt:(double)amount);
			clonePayment.PayDate=DateTime.Now;
			PayConnectResponseWeb newPCResponseWeb=new PayConnectResponseWeb() {
				PatNum=payment.PatNum,
				PayNum=clonePayment.PayNum,
				CCSource=pcResponseWeb.CCSource,
				Amount=clonePayment.PayAmt,
				PayNote=Lan.g("PayConnectL",clonePayment.PayNote+Environment.NewLine+"From within Open Dental Proper."),
				ProcessingStatus=PayConnectWebStatus.Completed,
				DateTimeEntry=DateTime.Now,
				DateTimeCompleted=DateTime.Now,
				IsTokenSaved=false,
				RefNumber=transResponse.RefNumber,
				TransType=transType,
				PaymentToken=pcResponseWeb.PaymentToken,
			};
			PayConnectResponseWebs.Insert(newPCResponseWeb);
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,clonePayment.PatNum,
				Patients.GetLim(clonePayment.PatNum).GetNameLF()+", "+clonePayment.PayAmt.ToString("c"));
			return true;
		}
	}
}
