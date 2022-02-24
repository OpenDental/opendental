using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class PaymentT {
		public static Payment MakePayment(long patNum,double payAmt,DateTime payDate=default,long payPlanNum=0,long provNum=0,long procNum=0,
			long payType=0,long clinicNum=0,long unearnedType=0,bool isRecurringCharge=false,DateTime recurringChargeDate=default,string externalId=""
			,long adjNum=0,bool doInsert=true,List<PaySplit> listSplits=null,long payPlanChargeNum=0) 
		{
			payDate=payDate==default ? DateTime.Today : payDate;
			Payment payment=new Payment();
			payment.PatNum=patNum;
			payment.PayDate=payDate;
			payment.PayAmt=payAmt;
			payment.PayType=payType;
			payment.ClinicNum=clinicNum;
			payment.DateEntry=payDate;
			payment.IsRecurringCC=isRecurringCharge;
			payment.RecurringChargeDate=recurringChargeDate;
			payment.ExternalId=externalId;
			if(doInsert) {
				Payments.Insert(payment);
			}
			PaySplit split=new PaySplit();
			split.PayNum=payment.PayNum;
			split.PatNum=payment.PatNum;
			split.DatePay=payDate;
			split.ClinicNum=payment.ClinicNum;
			split.PayPlanNum=payPlanNum;
			split.PayPlanChargeNum=payPlanChargeNum;
			split.AdjNum=adjNum;
			split.ProvNum=provNum;
			split.ProcNum=procNum;
			split.SplitAmt=payAmt;
			split.DateEntry=payDate;
			split.UnearnedType=unearnedType;
			if(doInsert) {
				PaySplits.Insert(split);
			}
			listSplits?.Add(split);
			return payment;
		}

		///<summary>Use this to test auto-split or income transfer logic.  This makes no splits, just the payment shell.</summary>
		public static Payment MakePaymentNoSplits(long patNum,double payAmt,DateTime payDate=default(DateTime),bool isNew=false,long payType=0
			,long clinicNum=0,bool doInsert=true)
		{
			if(payDate==default(DateTime)) {
				payDate=DateTime.Today;
			}
			Payment payment=new Payment();
			payment.PatNum=patNum;
			payment.PayDate=payDate;
			payment.PayAmt=payAmt;
			payment.IsNew=isNew;
			payment.ClinicNum=clinicNum;
			payment.PayType=payType;
			if(doInsert) {
				Payments.Insert(payment);
			}
			return payment;
		}

		public static Payment MakePaymentForPrepayment(Patient pat,Clinic clinic) {
			Payment paymentCur=new Payment();
			paymentCur.PayDate=DateTime.Today;
			paymentCur.PatNum=pat.PatNum;
			paymentCur.ClinicNum=clinic.ClinicNum;
			paymentCur.DateEntry=DateTime.Today;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			if(listDefs.Count>0) {
				paymentCur.PayType=listDefs[0].DefNum;
			}
			paymentCur.PaymentSource=CreditCardSource.None;
			paymentCur.ProcessStatus=ProcessStat.OfficeProcessed;
			paymentCur.PayAmt=0;
			Payments.Insert(paymentCur);
			return paymentCur;
		}

		///<summary>Transfers unallocated to unearned (if present) and inserts those results into the database. Then performs transfer.
		///This is the best representation of what the income transfer window currently does.</summary>
		public static PaymentEdit.IncomeTransferData BalanceAndIncomeTransfer(long patNum,Family fam=null,Payment regularTransferPayment=null
			,List<PayPlanCharge> payPlanCharges=null) 
		{
			if(fam==null) {
				fam=Patients.GetFamily(patNum);
			}
			if(regularTransferPayment==null) {
				regularTransferPayment=MakePaymentNoSplits(patNum,0);
			}
			if(payPlanCharges==null) {
				payPlanCharges=new List<PayPlanCharge>();
			}
			#region claim fix and transfer
			//both of these methods have objects that get immediately inserted into the database. While testing, a spcific call wil need to be made to delete.
			ClaimProcs.FixClaimsNoProcedures(fam.ListPats.Select(x => x.PatNum).ToList());//make dummy procedures and claimprocs for claims missing procs.
			ClaimProcs.TransferClaimsAsTotalToProcedures(fam.ListPats.Select(x => x.PatNum).ToList());//transfer AsTotals into claim procedures
			#endregion
			#region generate charges and credits for account
			//go through the logic that constructs the charges for the income transfer manager
			PaymentEdit.ConstructResults results=PaymentEdit.ConstructAndLinkChargeCredits(fam.ListPats.Select(x => x.PatNum).ToList(),
				patNum,new List<PaySplit>(),regularTransferPayment,new List<AccountEntry>(),isIncomeTxfr:true);
			#endregion
			//begin transfer loops - Does not insert objects into database from this method. Testing method needs to insert.
			if(!PaymentEdit.TryCreateIncomeTransfer(results.ListAccountCharges,DateTime.Today,out PaymentEdit.IncomeTransferData incomeTransferData)) {
				throw new ODException(incomeTransferData.StringBuilderErrors.ToString().TrimEnd());
			}
			return incomeTransferData;
		}

	}
}
