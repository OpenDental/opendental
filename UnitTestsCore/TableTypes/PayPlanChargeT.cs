using OpenDentBusiness;
using System;

namespace UnitTestsCore {
	public class PayPlanChargeT {

		public static PayPlanCharge CreateOne(long payPlanNum,long guarantor,long patNum,DateTime chargeDate,double principal,double interest=0,
			string note="",long provNum=0,long clinicNum=0,PayPlanChargeType chargeType=PayPlanChargeType.Debit,long procNum=0,bool doInsert=true)
		{
			PayPlanCharge charge=new PayPlanCharge();
			charge.PayPlanNum=payPlanNum;
			charge.Guarantor=guarantor;
			charge.PatNum=patNum;
			charge.ChargeDate=chargeDate;
			charge.Principal=principal;
			charge.Interest=interest;
			charge.Note=note;
			charge.ProvNum=provNum;
			charge.ClinicNum=clinicNum;
			charge.ChargeType=chargeType;
			charge.ProcNum=procNum;
			if(doInsert) {
				PayPlanCharges.Insert(charge);
			}
			return charge;
		}

		public static PayPlanCharge CreateNegativeCreditForAdj(long patNum,long payPlanNum,double negAdjAmt,long provNum=0,long clinicNum=0) {
			PayPlanCharge txOffset=new PayPlanCharge() {
				ClinicNum=clinicNum,
				ChargeDate=DateTime.Now.Date,
				ChargeType=PayPlanChargeType.Credit,//needs to be saved as a credit to show in Tx Form
				Guarantor=patNum,
				Note="Adjustment",
				PatNum=patNum,
				PayPlanNum=payPlanNum,
				Principal=negAdjAmt,
				ProcNum=0,
				ProvNum=provNum,
			};
			return txOffset;
		}

	}	
}
