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
			if(!ListTools.In(transType,PayConnectService.transType.RETURN,PayConnectService.transType.VOID)) {
				return false;
			}
			List<PaySplit> listPaySplits=PaySplits.GetForPayment(payment.PayNum);
			PayConnectService.creditCardRequest _payConnectRequest=new PayConnectService.creditCardRequest();
			PayConnectResponse response=null;
			string receiptStr="";
			_payConnectRequest.TransType=transType;
			_payConnectRequest.RefNumber=refNum;
			_payConnectRequest.Amount=amount;
			PayConnectService.transResponse transResponse=PayConnect.ProcessCreditCard(_payConnectRequest,payment.ClinicNum,x => MsgBox.Show(x));
			response=new PayConnectResponse(transResponse,_payConnectRequest);
			receiptStr=PayConnect.BuildReceiptString(_payConnectRequest,transResponse,null,payment.ClinicNum);
			if(response==null || response.StatusCode!="0") {//error in transaction
				return false;
			}
			//Record a new payment for the voided transaction
			Payment clonePayment=payment.Clone();
			clonePayment.PayAmt*=-1; //The negated amount of the original payment
			clonePayment.PayDate=DateTime.Now;
			clonePayment.Receipt=receiptStr;
			clonePayment.PayNote=Lan.g("PayConnectL","Transaction Type")+": "+Enum.GetName(typeof(PayConnectService.transType),transType)
				+Environment.NewLine+Lan.g("PayConnectL","Status")+": "+response.Description+Environment.NewLine
				+Lan.g("PayConnectL","Amount")+": "+clonePayment.PayAmt+Environment.NewLine
				+Lan.g("PayConnectL","Auth Code")+": "+response.AuthCode+Environment.NewLine
				+Lan.g("PayConnectL","Ref Number")+": "+response.RefNumber;
			clonePayment.PaymentSource=pcResponseWeb.CCSource;
			clonePayment.ProcessStatus=ProcessStat.OfficeProcessed;
			clonePayment.PayNum=Payments.Insert(clonePayment);
			List<PaySplit> listClonedPaySplits=new List<PaySplit>();
			foreach(PaySplit paySplit in listPaySplits) {
				PaySplit copy=paySplit.Copy();
				copy.SplitAmt*=-1;
				copy.PayNum=clonePayment.PayNum;
				copy.DatePay=clonePayment.PayDate;
				listClonedPaySplits.Add(copy);
			}
			PaySplits.InsertMany(listClonedPaySplits);
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
