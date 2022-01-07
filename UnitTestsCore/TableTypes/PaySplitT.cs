using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;
using CodeBase;

namespace UnitTestsCore {
	public class PaySplitT {

		///<summary>Helper method to create a pay split but only requires the minimum amount of parameters (for those of us that are lazy).</summary>
		public static PaySplit CreateOne(long patNum,double splitAmt,long payNum,long provNum,DateTime procDate=default,long procNum=0,
			long clinicNum=0,long payPlanNum=0,long unearnedType=0,long adjNum=0,DateTime datePay=default,long payPlanChargeNum=0,
			PayPlanDebitTypes payPlanDebitType=PayPlanDebitTypes.Unknown)
		{
			return CreateSplit(clinicNum,patNum,payNum,payPlanNum,procDate,procNum,provNum,splitAmt,unearnedType,adjNum,datePay,payPlanChargeNum,payPlanDebitType);
		}

		public static PaySplit CreateSplit(long clinicNum,long patNum,long payNum,long payplanNum,DateTime procDate,long procNum,long provNum,
			double splitAmt,long unearnedType,long adjNum=0,DateTime datePay=default,long payPlanChargeNum=0,
			PayPlanDebitTypes payPlanDebitType=PayPlanDebitTypes.Unknown)
		{
			PaySplit paysplit=new PaySplit() {
				ClinicNum=clinicNum,
				PatNum=patNum,
				PayNum=payNum,
				PayPlanChargeNum=payPlanChargeNum,
				PayPlanDebitType=payPlanDebitType,
				PayPlanNum=payplanNum,
				ProcDate=procDate,
				ProcNum=procNum,
				ProvNum=provNum,
				SplitAmt=splitAmt,
				UnearnedType=unearnedType,
				AdjNum=adjNum,
				DatePay=datePay,
			};
			PaySplits.Insert(paysplit);
			return paysplit;
		}

		public static PaySplit CreatePrepayment(long patNum,int amt,DateTime datePay,long provNum=0,long clinicNum=0) {
			Def unearnedType=Defs.GetDefByExactName(DefCat.PaySplitUnearnedType,"PrePayment")??DefT.CreateDefinition(DefCat.PaySplitUnearnedType,"PrePayment");
			Def payType=Defs.GetDefByExactName(DefCat.PaymentTypes,"Check")??DefT.CreateDefinition(DefCat.PaymentTypes,"Check");
			Payment pay=PaymentT.MakePaymentNoSplits(patNum,amt,datePay,clinicNum:clinicNum,payType:payType.DefNum);
			PaySplit split=new PaySplit();
			split.PayNum=pay.PayNum;
			split.PatNum=pay.PatNum;
			split.DatePay=datePay;
			split.ClinicNum=pay.ClinicNum;
			split.PayPlanNum=0;
			split.ProvNum=provNum;
			split.ProcNum=0;
			split.SplitAmt=amt;
			split.DateEntry=datePay;
			split.UnearnedType=unearnedType.DefNum;
			PaySplits.Insert(split);
			return split;
		}

		///<summary>Use when creating a TP prepayment (that does not necessariliy need to have a proc).</summary>
		public static PaySplit CreateTpPrepayment(long patNum,int amt,DateTime datePay=default,long provNum=0,long clinicNum=0,long procNum=0) {
			if(datePay==default) {
				datePay=DateTime.Today;
			}
			List<long> listHiddenUnearnedTypes=PaySplits.GetHiddenUnearnedDefNums();
			long hiddentUnearnedType;
			if(listHiddenUnearnedTypes.IsNullOrEmpty()) {
				hiddentUnearnedType=DefT.CreateDefinition(DefCat.PaySplitUnearnedType,"Hidden Unearned","X").DefNum;
			}
			else {
				hiddentUnearnedType=listHiddenUnearnedTypes.First();
			}
			List<Def> listPayTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			long payType;
			if(listPayTypes.IsNullOrEmpty()) {
				payType=DefT.CreateDefinition(DefCat.PaymentTypes,"Cache").DefNum;
			}
			else {
				payType=listPayTypes.First().DefNum;
			}
			Payment pay=PaymentT.MakePaymentNoSplits(patNum,amt,datePay,clinicNum:clinicNum,payType:payType);
			PaySplit split=new PaySplit {
				PayNum=pay.PayNum,
				PatNum=pay.PatNum,
				DatePay=datePay,
				ClinicNum=pay.ClinicNum,
				PayPlanNum=0,
				ProvNum=provNum,
				ProcNum=procNum,
				SplitAmt=amt,
				DateEntry=datePay,
				UnearnedType=hiddentUnearnedType
			};
			PaySplits.Insert(split);
			return split;
		}

		public static List<PaySplit> CreatePaySplitsForPrepayment(Procedure proc,double amtToUse,PaySplit prePaySplit=null,Clinic clinic=null,long prov=0) {
			List<PaySplit> retVal=new List<PaySplit>();
			long clinicNum=prePaySplit?.ClinicNum??clinic?.ClinicNum??proc.ClinicNum;
			long provNum=prePaySplit?.ProvNum??prov;
			if(clinic!=null) {
				clinicNum=clinic.ClinicNum;
			}
			if(prov!=0) {
				provNum=prov;
			}
			Def unearnedType=Defs.GetDefByExactName(DefCat.PaySplitUnearnedType,"PrePayment")??DefT.CreateDefinition(DefCat.PaySplitUnearnedType,"PrePayment");
			Def payType=Defs.GetDefByExactName(DefCat.PaymentTypes,"Check")??DefT.CreateDefinition(DefCat.PaymentTypes,"Check");
			Payment pay=PaymentT.MakePaymentNoSplits(proc.PatNum,amtToUse,payDate:DateTime.Now,clinicNum:clinicNum,payType:unearnedType.DefNum);
			PaySplit splitNeg=new PaySplit();
			splitNeg.PatNum=prePaySplit?.PatNum??proc.PatNum;
			splitNeg.PayNum=pay.PayNum;
			splitNeg.ClinicNum=clinicNum;
			splitNeg.ProvNum=provNum;
			splitNeg.SplitAmt=0-amtToUse;
			splitNeg.UnearnedType=prePaySplit?.UnearnedType??unearnedType.DefNum;
			splitNeg.DatePay=DateTime.Now;
			PaySplits.Insert(splitNeg);
			retVal.Add(splitNeg);
			//Make a different paysplit attached to proc and prov they want to use it for.
			PaySplit splitPos=new PaySplit();
			splitPos.PatNum=prePaySplit?.PatNum??proc.PatNum;
			splitPos.PayNum=pay.PayNum;
			splitPos.ProvNum=provNum;
			splitPos.ClinicNum=clinicNum;
			splitPos.SplitAmt=amtToUse;
			splitPos.DatePay=DateTime.Now;
			splitPos.ProcNum=proc.ProcNum;
			PaySplits.Insert(splitPos);
			retVal.Add(splitPos);
			return retVal;
		}
	}
}
